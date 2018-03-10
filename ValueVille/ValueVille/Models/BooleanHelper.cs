using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ValueVille.Models
{
    public static class BooleanHelper
    {
        public static MvcHtmlString TrueOrFalse(this HtmlHelper helper, bool showProduct)
        {
            var toReturn = showProduct?"Yes":"No";

            return new MvcHtmlString(toReturn);
        }
    }
}