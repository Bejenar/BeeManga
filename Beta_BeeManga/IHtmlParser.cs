using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beta_BeeManga
{
    public interface IHtmlParser
    {
        void SearchByName(string searchString);
        void GetChaptersList(string titleUrl);
        void DownloadChapter(Chapter chapter);
        void ShowTitles();

        List<string> getTitles();
        List<string> getUrls();
        List<string> getImg();
        List<Chapter> getChapters();
    }
}
