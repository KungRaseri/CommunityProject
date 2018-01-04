using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Data.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumVal"></param>
        /// <returns></returns>
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());

            return memInfo.First().GetCustomAttributes().OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumVal"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum enumVal)
        {
            return enumVal.GetAttributeOfType<DisplayAttribute>()?.Name ?? enumVal.ToString();
        }
    }
}
