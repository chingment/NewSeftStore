using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.IotTerm
{
    public class ProductService : BaseService
    {

        public IResult2 Add(string merchId, RopProductAdd rop)
        {
            var result = new CustomJsonResult2();


            if (string.IsNullOrEmpty(rop.name))
            {
                return new CustomJsonResult2(ResultCode.Failure, "商品名称不能为空");
            }


            if (rop.display_img_urls == null)
            {
                return new CustomJsonResult2(ResultCode.Failure, "至少上传一张商品图片");
            }

            if (rop.spec_items == null)
            {
                return new CustomJsonResult2(ResultCode.Failure, "至少录入一种规格");
            }
            else
            {
                var item_Len = rop.spec_items.Count;

                foreach (var sku in rop.spec_skus)
                {
                    if (sku.spec_val.Count != item_Len)
                    {
                        return new CustomJsonResult2(ResultCode.Failure, string.Format("商品编码：{0}，规格值不符合", sku.cum_code));
                    }
                }
            }


            List<string> skuIds = new List<string>();

            using (TransactionScope ts = new TransactionScope())
            {
                var d_Spu = new PrdSpu();
                d_Spu.Id = IdWorker.Build(IdType.NewGuid);
                d_Spu.MerchId = merchId;
                d_Spu.Name = rop.name.Trim2();
                d_Spu.SpuCode = rop.spu_code.Trim2();
                //d_Spu.KindIds = string.Join(",", rop.KindIds.ToArray());
                //d_Spu.KindId1 = rop.KindIds[0];
                //d_Spu.KindId2 = rop.KindIds[1];
                //d_Spu.KindId3 = rop.KindIds[2];
                d_Spu.PinYinIndex = CommonUtil.GetPingYinIndex(d_Spu.Name);
                d_Spu.DisplayImgUrls = ImgSet.Convert(rop.display_img_urls).ToJsonString();
                d_Spu.MainImgUrl = ImgSet.GetMain_O(d_Spu.DisplayImgUrls);
                d_Spu.DetailsDes = ImgSet.Convert(rop.details_des).ToJsonString();
                d_Spu.BriefDes = rop.brief_des.Trim2();
                d_Spu.IsTrgVideoService = false;
                d_Spu.IsRevService = false;
                d_Spu.IsSupRentService = false;
                d_Spu.IsMavkBuy = false;//[{"name":"单规格","value":[{"name":"规格"}]}]
                d_Spu.SupReceiveMode = E_SupReceiveMode.SelfTakeByDevice;
                //d_Spu.CharTags = rop.CharTags.ToJsonString();

                List<object> spec_items = new List<object>();
                for (var i = 0; i < rop.spec_items.Count; i++)
                {
                    string name = rop.spec_items[i];

                    List<object> spec_vals = new List<object>();

                    for (var j = 0; j < rop.spec_skus.Count; j++)
                    {
                        var spec_val = rop.spec_skus[j].spec_val[i];

                        spec_vals.Add(new { name = spec_val });
                    }


                    spec_items.Add(new { name = name, value = spec_vals });

                }

                d_Spu.SpecItems = spec_items.ToJsonString();


                //d_Spu.SupplierId = rop.SupplierId;


                d_Spu.Creator = merchId;
                d_Spu.CreateTime = DateTime.Now;

                List<object> spec_skus = new List<object>();

                foreach (var sku in rop.spec_skus)
                {
                    if (string.IsNullOrEmpty(sku.cum_code))
                    {
                        return new CustomJsonResult2(ResultCode.Failure, "该商品编码不能为空");
                    }

                    if (sku.sale_price <= 0)
                    {
                        return new CustomJsonResult2(ResultCode.Failure, "该商品价格必须大于0");
                    }


                    var isExtitSkuCode = CurrentDb.PrdSku.Where(m => m.MerchId == merchId && m.CumCode == sku.cum_code).FirstOrDefault();
                    if (isExtitSkuCode != null)
                    {
                        return new CustomJsonResult2(ResultCode.Failure, "该商品编码已经存在");
                    }

                    var d_Sku = new PrdSku();
                    d_Sku.Id = IdWorker.Build(IdType.NewGuid);
                    d_Sku.MerchId = d_Spu.MerchId;
                    d_Sku.SpuId = d_Spu.Id;
                    d_Sku.BarCode = sku.bar_code.Trim2();
                    d_Sku.CumCode = sku.cum_code.Trim2();

                    List<SpecDes> specDess = new List<SpecDes>();

                    for (var i = 0; i < sku.spec_val.Count; i++)
                    {
                        var specDes = new SpecDes();

                        specDes.Name = rop.spec_items[i];
                        specDes.Value = sku.spec_val[i];

                        specDess.Add(specDes);
                    }

                    d_Sku.Name = BizFactory.ProductSku.GetSkuSpecCombineName(d_Spu.Name, specDess);
                    d_Sku.PinYinIndex = CommonUtil.GetPingYinIndex(d_Sku.Name);
                    d_Sku.SpecDes = specDess.ToJsonString();
                    d_Sku.SpecIdx = string.Join(",", specDess.Select(m => m.Value));
                    d_Sku.SalePrice = sku.sale_price;
                    d_Sku.Creator = merchId;
                    d_Sku.CreateTime = DateTime.Now;
                    CurrentDb.PrdSku.Add(d_Sku);
                    CurrentDb.SaveChanges();

                    skuIds.Add(d_Sku.Id);

                    spec_skus.Add(new { sku_id = d_Sku.Id, cum_code = d_Sku.CumCode });
                }


                CurrentDb.PrdSpu.Add(d_Spu);
                CurrentDb.SaveChanges();
                ts.Complete();

                var ret = new { spu_id = d_Spu.Id, spec_skus = spec_skus };
                result = new CustomJsonResult2(ResultCode.Success, "保存成功", ret);
            }

            if (result.Code == ResultCode.Success)
            {
                CacheServiceFactory.Product.GetSkuInfo(merchId, skuIds.ToArray());
                MqFactory.Global.PushOperateLog(merchId, AppId.MERCH, merchId, EventCode.PrdProductAdd, string.Format("新建商品（{0}）成功", rop.name), rop);
            }

            return result;
        }

        public IResult2 Edit(string merchId, RopProductEdit rop)
        {
            return null;
        }
    }
}
