using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Growflo.Integration.Core.Extensions
{
    public static class ExtensionHelper
    {
        public static string GetString(this DataRow row, string columnName, string defaultValue = "")
        {
            object value = row[columnName];

            if (value == null || value == DBNull.Value)
                return defaultValue;
            else
                return value.ToString();
        }

        public static DateTime GetDateTime(this DataRow row, string columnName)
        {
            object value = row[columnName];

            return (DateTime)row[columnName];
        }
    }
}
