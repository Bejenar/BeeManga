using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beta_BeeManga
{
    public class Manga
    {
        public string title;
        public string url;
        public string thumbnailUrl;
        //public List<Chapter> chapters;
        public List<Chapter> chapters;
        public IHtmlParser helper;

        public Manga(string title, string url, string thumbnailUrl, IHtmlParser helper)
        {
            this.title = title;
            this.url = url;
            this.thumbnailUrl = thumbnailUrl;
            this.helper = helper;
            this.chapters = null;
        }
    }
}
