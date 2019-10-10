using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarParkingVehicleQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarParkingVehicleQueryModel : AopObject
    {
        /// <summary>
        /// 支付宝用户车辆ID，系统唯一
        /// </summary>
        [XmlElement("car_id")]
        public string CarId { get; set; }
    }
}
