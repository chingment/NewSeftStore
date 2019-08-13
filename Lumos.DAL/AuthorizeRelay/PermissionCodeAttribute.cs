using System;


namespace Lumos.DbRelay
{
    /// <summary>
    /// 权限属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class PermissionCodeAttribute : Attribute
    {
        public string PId { get; set; }
    }
}
