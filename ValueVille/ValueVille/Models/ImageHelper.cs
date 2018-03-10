using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Web.Mvc;

namespace ValueVille.Models
{
    public static class ImageHelper
    {

        public static MvcHtmlString Image(this HtmlHelper helper, byte[] src, string altText, string height)
        {
            var newsrc = string.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(src));
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", newsrc);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("height", height);

            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString ImageSrc(this HtmlHelper helper, byte[] src)
        {
            var newsrc = string.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(src));

            return MvcHtmlString.Create(newsrc.ToString());
        }


    }



}

