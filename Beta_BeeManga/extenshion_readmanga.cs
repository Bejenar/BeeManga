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
using System.Text.RegularExpressions;

namespace Beta_BeeManga
{
    class extenshion_readmanga : IHtmlParser
    {
        string basicUrl = "http://readmanga.me/search/advanced?";
        string titleName;
        List<string> searchResultTitles;
        List<string> searchResultUrls;
        List<string> searchResultImg;
        List<Chapter> chapters; // <url>


        public void DownloadChapter(Chapter chapter)
        {
            chapter.pageUrl = new List<string>();
            string s = getHtml(chapter.url);
            MatchCollection myMatchCollection = Regex.Matches(s, @"rm_h.init(\s*(.+?)\s*);");
            string str = "";

            foreach (Match m in myMatchCollection)
            {
                str = m.Groups[1].Value;
            }


            var charsToRemove = new string[] { "(", ")", "[", "]", "'", "\"" };
            foreach (var c in charsToRemove)
            {
                str = str.Replace(c, string.Empty);
            }

            //Console.WriteLine(str);
            myMatchCollection = Regex.Matches(str, @"http://\s*(.+?)\s*,\d");
            List<string> pages = new List<string>();
            int i = 0;
            foreach (Match m in myMatchCollection)
            {
                pages.Add("http://" + m.Groups[1].Value);
                pages[i++] = pages.Last().Replace(",", string.Empty);
            }

            i = 1;
            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            {
                foreach (var p in pages)
                {
                    chapter.pageUrl.Add(p);
                    client.Headers.Add("User-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " +
                                 "Windows NT 5.2; .NET CLR 1.0.3705;)");
                    client.Headers.Add("Referer", chapter.url);
                    Console.WriteLine(p);
                    Console.WriteLine(Translit(chapter.title));
                    //if (!Directory.Exists(chapter_url))
                    //    Directory.CreateDirectory(chapter_url);
                    if (p[p.Length - 2] == 'n')
                    {
                        if (!File.Exists(@"C:\Users\jion9\source\repos\BeeManga\Beta_BeeManga\bin\Debug\Reading\" + chapter.title + "_" + (i + 1).ToString() + ".png"))
                            client.DownloadFile(new Uri(p), @"C:\Users\jion9\source\repos\BeeManga\Beta_BeeManga\bin\Debug\Reading\" + chapter.title + "_" + i++.ToString() + ".png");
                    }
                    else
                    {
                        if (!File.Exists(@"C:\Users\jion9\source\repos\BeeManga\Beta_BeeManga\bin\Debug\Reading\" + chapter.title + "_" + (i + 1).ToString() + ".jpg"))
                            client.DownloadFile(new Uri(p), @"C:\Users\jion9\source\repos\BeeManga\Beta_BeeManga\bin\Debug\Reading\" + chapter.title + "_" + i++.ToString() + ".jpg");
                    }
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

            var r_href = doc.DocumentNode.SelectNodes("//div[@class='expandable chapters-link']//table[@class='table table-hover']//tr//td/a");
            var r_time = doc.DocumentNode.SelectNodes("//div[@class='expandable chapters-link']//table[@class='table table-hover']//tr//td[@class='hidden-xxs']");

            for (int i = 0; i < r_href.Count; i++)
            {
                buffer = new Chapter();
                //Console.WriteLine(r.Attributes["href"].Value);
                buffer.url = "http://readmanga.me" + r_href[i].Attributes["href"].Value;
                Regex trimmer = new Regex(@"\s\s+");
                buffer.title = Program.Translit(trimmer.Replace(r_href[i].InnerText, " "));
                //buffer.time = r_time[i].InnerHtml.Trim(' ');
                buffer.time = " ";
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
            parameters["q"] = searchString;
            uriBuilder.Query = parameters.ToString();
            string finalUrl = uriBuilder.Uri.AbsoluteUri;

            string s = getHtml(finalUrl);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(s);

            var r_href = doc.DocumentNode.SelectNodes("//div[@class='tiles row']//div[@class='tile col-sm-6 ']//div[@class='img']/a");
            foreach (var u in r_href)
            {
                searchResultUrls.Add("http://readmanga.me" + u.Attributes["href"].Value);
            }
            var r_title = doc.DocumentNode.SelectNodes("//div[@class='tiles row']//div[@class='tile col-sm-6 ']//div[@class='img']//a/img");
            foreach (var t in r_title)
            {
                searchResultTitles.Add(t.Attributes["title"].Value);
                searchResultImg.Add(t.Attributes["data-original"].Value);
            }

            
        }

        public void ShowTitles()
        {
            for (int i = 0; i < searchResultTitles.Count; i++)
            {
                Console.WriteLine("===" + (i + 1).ToString() + "===\n" + searchResultTitles[i] + " -> " + searchResultUrls[i] + "\n");
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
