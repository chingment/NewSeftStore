using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using MyWeiXinSdk;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Task
{
    public class Task4Tim2SenvivProvider : BaseService, IJob
    {
        public readonly string TAG = "Task4Tim2SenvivProvider";
        public DateTime Convert2DateTime(string str)
        {
            try
            {
                var dt1 = DateTime.Parse("1970-01-01T00:00:00+08:00");

                var dt = DateTime.Parse(str);

                if (dt < dt1)
                    return dt1;

                return dt;
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var senvivUsers = SdkFactory.Senviv.GetUserList();

                foreach (var senvivUser in senvivUsers)
                {
                    var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.Id == senvivUser.userid).FirstOrDefault();
                    if (d_SenvivUser == null)
                    {
                        d_SenvivUser = new Entity.SenvivUser();
                        d_SenvivUser.Id = senvivUser.userid;
                        d_SenvivUser.DeptId = senvivUser.deptid;
                        d_SenvivUser.Code = senvivUser.code;
                        d_SenvivUser.WechatId = senvivUser.wechatid;
                        d_SenvivUser.Mobile = senvivUser.mobile;
                        d_SenvivUser.Email = senvivUser.Email;
                        d_SenvivUser.Pwd = senvivUser.pwd;
                        d_SenvivUser.Nick = senvivUser.nick;
                        d_SenvivUser.Account = senvivUser.account;
                        d_SenvivUser.HeadImgurl = senvivUser.headimgurl;
                        d_SenvivUser.Language = senvivUser.language;
                        d_SenvivUser.Sex = senvivUser.sex;
                        d_SenvivUser.Birthday = senvivUser.birthday;
                        d_SenvivUser.Height = senvivUser.height;
                        d_SenvivUser.Weight = senvivUser.weight;
                        d_SenvivUser.TargetValue = senvivUser.TargetValue;
                        d_SenvivUser.Remarks = senvivUser.remarks;
                        d_SenvivUser.LastloginTime = Convert2DateTime(senvivUser.lastloginTime);
                        d_SenvivUser.LoginCount = senvivUser.loginCount;
                        d_SenvivUser.LastReportId = senvivUser.lastReportId;
                        d_SenvivUser.LastReportTime = Convert2DateTime(senvivUser.lastReportTime);
                        d_SenvivUser.Details = senvivUser.details;
                        d_SenvivUser.Status = senvivUser.status;
                        d_SenvivUser.SAS = senvivUser.SAS;
                        d_SenvivUser.BreathingMachine = senvivUser.BreathingMachine;
                        d_SenvivUser.Perplex = senvivUser.Perplex;
                        d_SenvivUser.OtherPerplex = senvivUser.OtherPerplex;
                        d_SenvivUser.Medicalhistory = senvivUser.Medicalhistory;
                        d_SenvivUser.OtherFamilyhistory = senvivUser.OtherFamilyhistory;
                        d_SenvivUser.Medicine = senvivUser.Medicine;
                        d_SenvivUser.OtherMedicine = senvivUser.OtherMedicine;
                        d_SenvivUser.Creator = IdWorker.Build(IdType.EmptyGuid);
                        d_SenvivUser.CreateTime = Convert2DateTime(senvivUser.createtime);
                        d_SenvivUser.MendTime = Convert2DateTime(senvivUser.updateTime);
                        CurrentDb.SenvivUser.Add(d_SenvivUser);
                        CurrentDb.SaveChanges();
                    }

                    var products = senvivUser.products;
                    if (products != null)
                    {

                        foreach (var product in products)
                        {
                            var d_SenvivUserProduct = CurrentDb.SenvivUserProduct.Where(m => m.Id == product._id).FirstOrDefault();
                            if (d_SenvivUserProduct == null)
                            {
                                d_SenvivUserProduct = new Entity.SenvivUserProduct();
                                d_SenvivUserProduct.Id = product._id;
                                d_SenvivUserProduct.SvUserId = d_SenvivUser.Id;
                                d_SenvivUserProduct.DeptName = product.deptName;
                                d_SenvivUserProduct.Sn = product.sn;
                                d_SenvivUserProduct.QrcodeUrl = product.qrcodeUrl;
                                d_SenvivUserProduct.DeviceQRCode = product.deviceQRCode;
                                d_SenvivUserProduct.TcpAddress = product.tcpAddress;
                                d_SenvivUserProduct.WebUrl = product.webUrl;
                                d_SenvivUserProduct.LastOnlineTime = Convert2DateTime(product.LastOnlineTime);
                                d_SenvivUserProduct.Model = product.model;
                                d_SenvivUserProduct.BindTime = Convert2DateTime(product.bindtime);
                                d_SenvivUserProduct.Status = product.status;
                                d_SenvivUserProduct.Version = product.version;
                                d_SenvivUserProduct.Remarks = product.remarks;
                                d_SenvivUserProduct.Batch = product.batch;
                                d_SenvivUserProduct.Scmver = product.scmver;
                                d_SenvivUserProduct.Sovn = product.sovn;
                                d_SenvivUserProduct.Epromvn = product.epromvn;
                                d_SenvivUserProduct.Imsi = product.imsi;
                                d_SenvivUserProduct.Longitude = product.longitude;
                                d_SenvivUserProduct.Latitude = product.latitude;
                                d_SenvivUserProduct.DeptId = product.deptid;
                                d_SenvivUserProduct.Creator = IdWorker.Build(IdType.EmptyGuid);
                                d_SenvivUserProduct.CreateTime = Convert2DateTime(senvivUser.createtime);
                                CurrentDb.SenvivUserProduct.Add(d_SenvivUserProduct);
                                CurrentDb.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(TAG, ex);
            }
        }
    }
}
