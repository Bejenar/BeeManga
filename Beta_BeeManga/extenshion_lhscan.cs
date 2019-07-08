using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Web;
using static Beta_BeeManga.Program;
using System.Net;
using System.IO;

namespace Beta_BeeManga
{
    public class extenshion_lhscan : IHtmlParser
    {
        string basicUrl = "http://lhscan.net/manga-list.html?";
        string titleName;
        List<string> searchResultTitles;
        List<string> searchResultUrls;
        List<string> searchResultImg;
        List<Chapter> chapters; // <url>


        public void DownloadChapter(Chapter chapter)
        {
            string s = getHtml(chapter.url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(s);
            chapter.pageUrl = new List<string>();

            var pages = doc.DocumentNode.SelectNodes("//div[@class='chapter-content']//img[@class='chapter-img']");           
            int i = 1;
            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            {
                foreach (var p in pages)
                {
                    chapter.pageUrl.Add(p.Attributes["src"].Value);
                    client.Headers.Add("User-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " +
                                 "Windows NT 5.2; .NET CLR 1.0.3705;)");
                    client.Headers.Add("Referer", chapter.url);
                    Console.WriteLine(p.Attributes["src"].Value);
                    //if (!Directory.Exists(chapter_url))
                    //    Directory.CreateDirectory(chapter_url);
                    if(!File.Exists(@"C:\Users\jion9\source\repos\BeeManga\Beta_BeeManga\bin\Debug\Reading\" + chapter.title + "_" + (i+1).ToString() + ".jpg"))
                    client.DownloadFile(new Uri(p.Attributes["src"].Value), @"C:\Users\jion9\source\repos\BeeManga\Beta_BeeManga\bin\Debug\Reading\" + chapter.title + "_" + i++.ToString() + ".jpg");
                }
            }
        }

        public void GetChaptersList(string titleUrl)
        {
            chapters = new List<Chapter>();
            Chapter buffer = new Chapter();

            string s = getHtml(titleUrl);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(s);

            var r_href = doc.DocumentNode.SelectNodes("//div[@class='tab-text']//table[@class='table table-hover']//tbody//tr//td/a");
            var r_time = doc.DocumentNode.SelectNodes("//div[@class='tab-text']//table[@class='table table-hover']//tbody//tr//td//i/time");

            for(int i = 0; i < r_href.Count; i++)
            {
                buffer = new Chapter();
                //Console.WriteLine(r.Attributes["href"].Value);
                buffer.url = "https://lhscan.net/" + r_href[i].Attributes["href"].Value;
                buffer.title = r_href[i].Attributes["title"].Value;
                buffer.time = r_time[i].InnerHtml;
                buffer.helper = this;

                chapters.Add(buffer);

            }


        }

        public void SearchByName(string searchString)
        {
            searchResultTitles = new List<string>();
            searchResultUrls = new List<string>();
            searchResultImg = new List<string>();

            var uriBuilder = new UriBuilder(basicUrl);
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["name"] = searchString;
            uriBuilder.Query = parameters.ToString();
            string finalUrl = uriBuilder.Uri.AbsoluteUri;

            string s = getHtml(finalUrl);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(s);

            var pages_input = doc.DocumentNode.SelectSingleNode("//div[@class='btn-group']//button[@class='btn btn-sm btn-info']");
            string pages = pages_input.InnerText;
            var pages_number = pages.Split(' ');
            int pages_count = Convert.ToInt32(pages_number[3]);

            for (int i = 1; i <= pages_count; i++)
            {
                s = getHtml(finalUrl + "&page=" + i.ToString());
                doc.LoadHtml(s);
                var urlResult = doc.DocumentNode.SelectNodes("//div[@class='col-lg-12 col-md-12 row-list']//div[@class='media']/a"); // all urls
                var titleResult = doc.DocumentNode.SelectNodes("//div[@class='col-lg-12 col-md-12 row-list']//div[@class='media']//a[@class='pull-left link-list']/img"); // all names

                foreach (var t in titleResult)
                {
                    searchResultTitles.Add(t.Attributes["alt"].Value);
                    searchResultImg.Add(t.Attributes["src"].Value);
                }
                foreach (var u in urlResult)
                {
                    searchResultUrls.Add("https://lhscan.net/" + u.Attributes["href"].Value);
                }
            }
        }

        public void ShowTitles()
        {
            for(int i = 0; i < searchResultTitles.Count; i++)
            {
                Console.WriteLine("===" + (i+1).ToString() + "===\n" +searchResultTitles[i] + " -> " + searchResultUrls[i] + "\n");
            }
        }

        public List<string> getTitles()
        {
            return searchResultTitles;
        }

        public List<string> getUrls()
        {
            return searchResultUrls;
        }

        public List<string> getImg()
        {
            return searchResultImg;
        }

        public List<Chapter> getChapters()
        {
            return chapters;
        }
    }
}
