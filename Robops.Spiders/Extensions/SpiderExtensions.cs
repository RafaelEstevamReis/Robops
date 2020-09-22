using Net.RafaelEstevam.Spider;
using Net.RafaelEstevam.Spider.Wrappers.HTML;
using System;
using System.Collections.Generic;

namespace Robops.Spiders.Extensions
{
    public static class SpiderExtensions
    {
        public static void AddPages(this SimpleSpider spider, IEnumerable<Anchor> iA, Net.RafaelEstevam.Spider.Link source)
        {
            foreach (var a in iA) AddPage(spider, a, source);
        }
        public static void AddPage(this SimpleSpider spider, Anchor a, Net.RafaelEstevam.Spider.Link source)
        {
            var url = a.Href.Replace("\t", "")
                            .Replace("\r", "")
                            .Replace("\n", "")
                            .Replace("&amp;", "&");
            var uri = new Uri(source, url);
            spider.AddPage(uri, source);
        }
    }
}
