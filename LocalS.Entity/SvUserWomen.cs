using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_SvUserWomenDeliveryWay
    {
        None = 0,
        NaturalLabour = 1,
        Cesarean = 2
    }

    [Table("SvUserWomen")]
    public class SvUserWomen
    {
        [Key]
        public string Id { get; set; }
        public string SvUserId { get; set; }
        //怀孕时间
        public DateTime PregnancyTime { get; set; }
        //分娩时间
        public DateTime DeliveryTime { get; set; }
        //分娩后异常
        public string DeliveryExState { get; set; }
        //主诉症状
        public string ChiefComplaint { get; set; }
        //孕期合并症
        public string Complication { get; set; }
        //分娩方式
        public E_SvUserWomenDeliveryWay DeliveryWay { get; set; }
        public string CesareanReason { get; set; }
        public string CesareanIncision { get; set; }
        //产钳
        public bool IsForcep { get; set; }
        public string FmsSituation { get; set; }
        public bool FmsIsUreter { get; set; }
        public string FmsMembrane { get; set; }
        public string FoodProhibited { get; set; }
        public string MentalState { get; set; }
        public string FaceState { get; set; }
        public string RyHeight { get; set; }
        public string RyWeight { get; set; }
        public string RyTemperature { get; set; }
        public string RyHeartbeat { get; set; }
        public string RyBreathe { get; set; }
        public string RyBpLow { get; set; }
        public string RyBpHigh { get; set; }
        public string RyHeartState { get; set; }
        public string RyLungState { get; set; }
        public string RyUterusHeight { get; set; }
        public string RyUcState { get; set; }
        public string RyLMammaState { get; set; }
        public string RyLNipleState { get; set; }
        public string RyLBreastState { get; set; }
        public string RyRbState { get; set; }
        public string RyRNipleState { get; set; }
        public string RyRBreastState { get; set; }
        public string UresisState { get; set; }
        public string DiachoresisState { get; set; }
    }
}
