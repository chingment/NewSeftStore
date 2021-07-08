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
        public List<ImgSet> ConvertImgSet(List<string> imgs)
        {
            List<ImgSet> imgSets = new List<ImgSet>();

            for (var i = 0; i < imgs.Count; i++)
            {
                var imgSet = new ImgSet();
                if (i == 0)
                {
                    imgSet.IsMain = true;
                }
                imgSet.Name = "";
                imgSet.Url = imgs[i];

                imgSets.Add(imgSet);
            }
            return imgSets;
        }

        public List<SpecItem> ConvertSpecItems(List<string> spec_items, List<spec_sku> spec_skus)
        {
            var specItems = new List<SpecItem>();


            for (var i = 0; i < spec_items.Count; i++)
            {
                string name = spec_items[i];

                List<SpecItemValue> specItemValues = new List<SpecItemValue>();

                for (var j = 0; j < spec_skus.Count; j++)
                {
                    var spec_val = spec_skus[j].spec_val[i];

                    specItemValues.Add(new SpecItemValue { Name = spec_val });
                }

                specItems.Add(new SpecItem { Name = name, Value = specItemValues });

            }

            return specItems;
        }

        public List<SpecDes> ConvertSpecDes(List<string> spec_items, List<string> spec_val)
        {

            List<SpecDes> specDess = new List<SpecDes>();

            for (var i = 0; i < spec_val.Count; i++)
            {
                var specDes = new SpecDes();

                specDes.Name = spec_items[i];
                specDes.Value = spec_val[i];

                specDess.Add(specDes);
            }

            return specDess;
        }

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

            if (rop.spec_items == null || rop.spec_items.Count == 0)
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
                var display_img_urls = ConvertImgSet(rop.display_img_urls);
                d_Spu.DisplayImgUrls = display_img_urls.ToJsonString();
                d_Spu.MainImgUrl = display_img_urls[0].Url;
                d_Spu.DetailsDes = ConvertImgSet(rop.details_des).ToJsonString();
                d_Spu.BriefDes = rop.brief_des.Trim2();
                d_Spu.IsTrgVideoService = false;
                d_Spu.IsRevService = false;
                d_Spu.IsSupRentService = false;
                d_Spu.IsMavkBuy = false;
                d_Spu.SupReceiveMode = E_SupReceiveMode.SelfTakeByDevice;
                d_Spu.CharTags = null;
                d_Spu.SpecItems = ConvertSpecItems(rop.spec_items, rop.spec_skus).ToJsonString();
                d_Spu.SupplierId = null;
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
                        return new CustomJsonResult2(ResultCode.Failure, string.Format("该商品编码:{0},须要大于零", sku.cum_code));
                    }


                    var isExtitSkuCode = CurrentDb.PrdSku.Where(m => m.MerchId == merchId && m.CumCode == sku.cum_code).FirstOrDefault();
                    if (isExtitSkuCode != null)
                    {
                        return new CustomJsonResult2(ResultCode.Failure, string.Format("该商品编码:{0},已经存在", sku.cum_code));
                    }

                    var d_Sku = new PrdSku();
                    d_Sku.Id = IdWorker.Build(IdType.NewGuid);
                    d_Sku.MerchId = d_Spu.MerchId;
                    d_Sku.SpuId = d_Spu.Id;
                    d_Sku.BarCode = sku.bar_code.Trim2();
                    d_Sku.CumCode = sku.cum_code.Trim2();
                    var specDess = ConvertSpecDes(rop.spec_items, sku.spec_val);
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
                MqFactory.Global.PushOperateLog(merchId, AppId.MERCH, merchId, EventCode.product_add, string.Format("新建商品（{0}）成功", rop.name), rop);
            }

            return result;
        }

        public IResult2 Edit(string merchId, RopProductEdit rop)
        {
            var result = new CustomJsonResult2();

            if (string.IsNullOrEmpty(rop.spu_id))
            {
                return new CustomJsonResult2(ResultCode.Failure, "商品Id不能为空");
            }

            if (string.IsNullOrEmpty(rop.name))
            {
                return new CustomJsonResult2(ResultCode.Failure, "商品名称不能为空");
            }


            if (rop.display_img_urls == null)
            {
                return new CustomJsonResult2(ResultCode.Failure, "至少上传一张商品图片");
            }

            if (rop.spec_items == null || rop.spec_items.Count == 0)
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

            //先删除缓存

            CacheServiceFactory.Product.RemoveSpuInfo(merchId, rop.spu_id);


            List<string> skuIds = new List<string>();


            using (TransactionScope ts = new TransactionScope())
            {
                var d_Spu = CurrentDb.PrdSpu.Where(m => m.Id == rop.spu_id).FirstOrDefault();

                d_Spu.Name = rop.name;
                d_Spu.SpuCode = rop.spu_code;
                //d_Spu.KindIds = string.Join(",", rop.KindIds.ToArray());
                //d_Spu.KindId1 = rop.KindIds[0];
                //d_Spu.KindId2 = rop.KindIds[1];
                //d_Spu.KindId3 = rop.KindIds[2];
                d_Spu.PinYinIndex = CommonUtil.GetPingYinIndex(d_Spu.Name);
                d_Spu.BriefDes = rop.brief_des;
                d_Spu.DetailsDes = ConvertImgSet(rop.details_des).ToJsonString();

                var display_img_urls = ConvertImgSet(rop.display_img_urls);
                d_Spu.DisplayImgUrls = display_img_urls.ToJsonString();
                d_Spu.MainImgUrl = display_img_urls[0].Url;


                d_Spu.SpecItems = ConvertSpecItems(rop.spec_items, rop.spec_skus).ToJsonString();

                d_Spu.Mender = merchId;
                d_Spu.MendTime = DateTime.Now;

                List<object> spec_skus = new List<object>();

                foreach (var sku in rop.spec_skus)
                {
                    if (CommonUtil.IsEmpty(sku.sku_id))
                    {
                        return new CustomJsonResult2(ResultCode.Failure, "该商品sku_id不能为空");
                    }

                    if (CommonUtil.IsEmpty(sku.cum_code))
                    {
                        return new CustomJsonResult2(ResultCode.Failure, "该商品cum_code不能为空");
                    }

                    var isExtitSkuCode = CurrentDb.PrdSku.Where(m => m.MerchId == merchId && m.Id != sku.sku_id && m.CumCode == sku.cum_code).FirstOrDefault();
                    if (isExtitSkuCode != null)
                    {
                        return new CustomJsonResult2(ResultCode.Failure, string.Format("该商品编码:{0},已经存在", sku.cum_code));
                    }

                    var d_Sku = CurrentDb.PrdSku.Where(m => m.Id == sku.sku_id).FirstOrDefault();
                    if (d_Sku != null)
                    {
                        d_Sku.Name = BizFactory.ProductSku.GetSkuSpecCombineName(d_Spu.Name, d_Sku.SpecDes.ToJsonObject<List<SpecDes>>());
                        d_Sku.PinYinIndex = CommonUtil.GetPingYinIndex(d_Sku.Name);

                        var specDess = ConvertSpecDes(rop.spec_items, sku.spec_val);

                        d_Sku.SpecDes = specDess.ToJsonString();
                        d_Sku.SpecIdx = string.Join(",", specDess.Select(m => m.Value));

                        d_Sku.CumCode = sku.cum_code;
                        d_Sku.BarCode = sku.bar_code;
                        d_Sku.SalePrice = sku.sale_price;
                        d_Sku.Mender = merchId;
                        d_Sku.MendTime = DateTime.Now;

                        var d_SellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.SkuId == d_Sku.Id).ToList();
                        foreach (var d_SellChannelStock in d_SellChannelStocks)
                        {
                            d_SellChannelStock.SalePrice = sku.sale_price;
                            d_SellChannelStock.IsOffSell = false;
                            d_SellChannelStock.Mender = merchId;
                            d_SellChannelStock.MendTime = DateTime.Now;
                        }

                    }

                    skuIds.Add(sku.sku_id);
                    spec_skus.Add(new { sku_id = d_Sku.Id, cum_code = d_Sku.CumCode });
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                var ret = new { spu_id = d_Spu.Id, spec_skus = spec_skus };

                result = new CustomJsonResult2(ResultCode.Success, "保存成功", ret);
            }

            if (result.Code == ResultCode.Success)
            {
                CacheServiceFactory.Product.RemoveSpuInfo(merchId, rop.spu_id);
                CacheServiceFactory.Product.GetSkuInfo(merchId, skuIds.ToArray());
                MqFactory.Global.PushOperateLog(merchId, AppId.MERCH, merchId, EventCode.product_edit, string.Format("保存商品（{0}）信息成功", rop.name), rop);

            }


            return result;
        }
    }
}
