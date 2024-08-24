using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DemoAdminLTE.Models;

namespace DemoAdminLTE.Utils
{
    public static class Helper
    {
        public static string ToStringEnUs(this double value, string format = null)
        {
            if (string.IsNullOrEmpty(format))
            {
                return value.ToString(CultureInfo.CreateSpecificCulture("en-US"));
            }
            return value.ToString(format, CultureInfo.CreateSpecificCulture("en-US"));
        }

        public static long ToEpochTime(this DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }
        public static string ToTdHtmlRawValues(this ICollection<SensorValues> sensorValues)
        {
            string str = "";
            if (sensorValues != null)
            {
                foreach (SensorValues sensorValue in (IEnumerable<SensorValues>)sensorValues.OrderBy<SensorValues, int>((Func<SensorValues, int>)(o => o.station_id)))
                    str = str + (object)sensorValue.sensor_values + "</td><td>";
            }
            return str ?? "";
        }

    }
}
