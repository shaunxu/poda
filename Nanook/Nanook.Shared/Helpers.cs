using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Nanook.Shared
{
    public static class Helpers
    {
        public static string Combine(this IEnumerable<string> source, char separator)
        {
            var list = source.ToList();
            var count = list.Count;
            var result = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                if (i < count - 1)
                {
                    result.AppendFormat("{0}{1}", list[i], separator);
                }
                else
                {
                    result.Append(list[i]);
                }
            }
            return result.ToString();
        }

        public static MvcHtmlString TextBlock(this HtmlHelper helper, string value)
        {
            value = value.Replace("\n", "<br />");
            return new MvcHtmlString(value);
        }
    }
}
