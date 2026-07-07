using System;

namespace WMS_UnitedTracors_Blazor.Helpers
{
    public static class WibHelper
    {
        public static DateTime Today => DateTime.UtcNow.AddHours(7).Date;
        public static DateTime Now => DateTime.UtcNow.AddHours(7);
        
        public static string Format(DateTime? dt, string format = "dd MMM yyyy HH:mm")
        {
            if (!dt.HasValue) return "-";
            return $"{dt.Value.ToString(format)} WIB";
        }
    }
}
