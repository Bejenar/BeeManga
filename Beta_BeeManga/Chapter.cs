using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beta_BeeManga
{
    public class Chapter
    {
        public IHtmlParser helper;
        public List<string> pageUrl;

        public string title;
        public string url;
        public string time;
     
        
        public static Chapter ByTitle(Manga manga, string Title)
        {
            foreach(Chapter c in manga.chapters)
            {
                if (c.title == Title)
                    return c;
            }

            return new Chapter();
        }
    }
}
