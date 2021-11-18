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
    public class Task4Tim2SenvivUsersProvider : BaseService, IJob
    {
        public readonly string TAG = "Task4Tim2SenvivUsersProvider";
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
                        d_SenvivUser.WxOpenId = senvivUser.wechatid;
                        d_SenvivUser.PhoneNumber = senvivUser.mobile;
                        d_SenvivUser.NickName = senvivUser.nick;
                        d_SenvivUser.FullName = senvivUser.account;
                        d_SenvivUser.Avatar = senvivUser.headimgurl;
                        d_SenvivUser.Sex = senvivUser.sex;
                        d_SenvivUser.Birthday = Convert2DateTime(senvivUser.birthday);
                        d_SenvivUser.Height = senvivUser.height;
                        d_SenvivUser.Weight = senvivUser.weight;
                        d_SenvivUser.TargetValue = senvivUser.TargetValue;
                        d_SenvivUser.LastReportId = senvivUser.lastReportId;
                        d_SenvivUser.LastReportTime = Convert2DateTime(senvivUser.lastReportTime);
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
                        CurrentDb.SenvivUser.Add(d_SenvivUser);
                        CurrentDb.SaveChanges();
                    }
                    else
                    {
                        d_SenvivUser.DeptId = senvivUser.deptid;
                        d_SenvivUser.WxOpenId = senvivUser.wechatid;
                        d_SenvivUser.PhoneNumber = senvivUser.mobile;
                        d_SenvivUser.NickName = senvivUser.nick;
                        d_SenvivUser.FullName = senvivUser.account;
                        d_SenvivUser.Avatar = senvivUser.headimgurl;
                        d_SenvivUser.Sex = senvivUser.sex;
                        d_SenvivUser.Birthday = Convert2DateTime(senvivUser.birthday);
                        d_SenvivUser.Height = senvivUser.height;
                        d_SenvivUser.Weight = senvivUser.weight;
                        d_SenvivUser.TargetValue = senvivUser.TargetValue;
                        d_SenvivUser.LastReportId = senvivUser.lastReportId;
                        d_SenvivUser.LastReportTime = Convert2DateTime(senvivUser.lastReportTime);
                        d_SenvivUser.SAS = senvivUser.SAS;
                        d_SenvivUser.BreathingMachine = senvivUser.BreathingMachine;
                        d_SenvivUser.Perplex = senvivUser.Perplex;
                        d_SenvivUser.OtherPerplex = senvivUser.OtherPerplex;
                        d_SenvivUser.Medicalhistory = senvivUser.Medicalhistory;
                        d_SenvivUser.OtherFamilyhistory = senvivUser.OtherFamilyhistory;
                        d_SenvivUser.Medicine = senvivUser.Medicine;
                        d_SenvivUser.OtherMedicine = senvivUser.OtherMedicine;
                        d_SenvivUser.MendTime = DateTime.Now;
                        d_SenvivUser.Mender = IdWorker.Build(IdType.EmptyGuid);
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
                                d_SenvivUserProduct.CreateTime = DateTime.Now;
                                CurrentDb.SenvivUserProduct.Add(d_SenvivUserProduct);
                                CurrentDb.SaveChanges();
                            }
                        }
                    }
                }


                var senvivBoxs = SdkFactory.Senviv.GetBoxList();
                foreach (var senvivBox in senvivBoxs)
                {
                    var d_Device = CurrentDb.Device.Where(m => m.Id == senvivBox.sn).FirstOrDefault();
                    if (d_Device == null)
                    {
                        d_Device = new Entity.Device();
                        d_Device.Id = senvivBox.sn;
                        d_Device.Name = "非接触式生命体征检测仪";
                        d_Device.Type = "senvivlite";
                        d_Device.ImeiId = senvivBox.imei;
                        d_Device.CurUseMerchId = "88273829";
                        d_Device.Model = senvivBox.model;
                        d_Device.Lat = float.Parse(senvivBox.latitude);
                        d_Device.Lng = float.Parse(senvivBox.longitude);
                        d_Device.AppVersionName = senvivBox.version;
                        d_Device.Creator = IdWorker.Build(IdType.EmptyGuid);
                        d_Device.CreateTime = DateTime.Now;
                        CurrentDb.Device.Add(d_Device);
                        CurrentDb.SaveChanges();

                        var d_MerchDevice = new Entity.MerchDevice();
                        d_MerchDevice.Id = IdWorker.Build(IdType.NewGuid);
                        d_MerchDevice.MerchId = d_Device.CurUseMerchId;
                        d_MerchDevice.DeviceId = d_Device.Id;
                        d_MerchDevice.Creator = d_Device.Creator;
                        d_MerchDevice.CreateTime = d_Device.CreateTime;
                        CurrentDb.MerchDevice.Add(d_MerchDevice);
                        CurrentDb.SaveChanges();
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
