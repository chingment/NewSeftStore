using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using Lumos.BLL;
using Lumos.Redis;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LocalS.BLL.Task
{

    public enum Task4TimType
    {
        Unknow = 0,
        Order2CheckPay = 1
    }

    public class Task4Tim2GlobalProvider : BaseDbContext, IJob
    {
        private static readonly string key = "task4Tim2Global";

        public void Enter(Task4TimType type,string id, DateTime expireTime, object data)
        {
            var d = new TaskData();
            d.Id = id;
            d.Type = type;
            d.ExpireTime = expireTime;
            d.Data = data;
            RedisManager.Db.HashSetAsync(key, d.Id,d.ToJsonString(), StackExchange.Redis.When.Always);
        }

        public void Exit(string id)
        {
            RedisManager.Db.HashDelete(key, id);
        }

        public static List<TaskData> GetList()
        {
            List<TaskData> list = new List<TaskData>();
            var hs = RedisManager.Db.HashGetAll(key);

            var d = (from i in hs select i).ToList();

            foreach (var item in d)
            {
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TaskData>(item.Value);
                list.Add(obj);
            }
            return list;
        }

        public void Execute(IJobExecutionContext context)
        {
            #region 检查支付状态
            try
            {
                var lists = GetList();
                LogUtil.Info(string.Format("共有{0}条记录需要检查状态", lists.Count));
                if (lists.Count > 0)
                {
                    LogUtil.Info(string.Format("开始执行订单查询,时间：{0}", DateTime.Now));
                    foreach (var m in lists)
                    {
                        switch (m.Type)
                        {
                            case Task4TimType.Order2CheckPay:
                                #region 检查支付状态
                                var order = m.Data.ToJsonObject<Order>();
                                LogUtil.Info(string.Format("查询订单号：{0}", order.Sn));
                                //判断支付过期时间
                                if (m.ExpireTime.AddMinutes(1) >= DateTime.Now)
                                {
                                    //未过期查询支付状态
                                    bool isPaySuccessed = false;
                                    string content = "";
                                    switch (order.PayCaller)
                                    {
                                        case E_OrderPayCaller.WechatByNative:
                                            var wxPaAppInfoConfig = BizFactory.Merch.GetWxPaAppInfoConfig(order.MerchId);
                                            content = SdkFactory.Wx.OrderQuery(wxPaAppInfoConfig, order.Sn);
                                            break;
                                        case E_OrderPayCaller.WechatByMp:
                                            var wxMpAppInfoConfig = BizFactory.Merch.GetWxMpAppInfoConfig(order.MerchId);
                                            content = SdkFactory.Wx.OrderQuery(wxMpAppInfoConfig, order.Sn);
                                            break;
                                    }

                                    LogUtil.Info(string.Format("订单号：{0},查询支付结果文件:{1}", order.Sn, content));

                                    BizFactory.Order.PayResultNotify(GuidUtil.Empty(), E_OrderNotifyLogNotifyFrom.OrderQuery, content, order.Sn, out isPaySuccessed);

                                    if (isPaySuccessed)
                                    {
                                        Task4Factory.Global.Exit(m.Id);
                                        LogUtil.Info(string.Format("订单号：{0},支付成功,删除缓存", order.Sn));
                                    }
                                }
                                else
                                {
                                    //已过期，取消订单
                                    var rt = BizFactory.Order.Cancle(GuidUtil.Empty(), order.Id, "订单支付有效时间过期");
                                    if (rt.Result == ResultType.Success)
                                    {
                                        LogUtil.Info(string.Format("订单号：{0},支付超时,取消订单，删除缓存", order.Sn));
                                    }
                                }
                                #endregion 
                                break;
                        }
                    }

                    LogUtil.Info(string.Format("结束执行订单查询,时间:{0}", DateTime.Now));
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("全局定时任务发生异常", ex);
            }
            #endregion
        }

        public class TaskData
        {
            public string Id { get; set; }
            public Task4TimType Type { get; set; }
            public DateTime ExpireTime { get; set; }
            public object Data { get; set; }
        }
    }
}
