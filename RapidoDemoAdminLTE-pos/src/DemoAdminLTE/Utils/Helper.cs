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
        public static string ToTdHtmlRawValues(this ICollection<SensorValue> sensorValues)
        {
            string str = "";
            if (sensorValues != null)
            {
                foreach (SensorValue sensorValue in (IEnumerable<SensorValue>)sensorValues.OrderBy<SensorValue, int>((Func<SensorValue, int>)(o => o.Sensor.Id)))
                    str = str + (object)sensorValue.Value + "</td><td>";
            }
            return str ?? "";
        }

    }
}
