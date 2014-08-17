using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace MatomeRanking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (Settings.Debug)
            {
                CreateRankingHtml(0);
            }
            else
            {
                var userName = args[0];
                var password = args[1];
                var uploadUri = args[2];

                CreateRankingHtml(1);
                Upload(userName, password, uploadUri);

                while (true)
                {
                    CreateRankingHtml(Settings.ReloadInterval);
                    Upload(userName, password, uploadUri);
                    Thread.Sleep(60 * 1000);
                }
            }
        }

        private static void Upload(string userName, string password, string uploadUri)
        {
            if (Settings.Debug) return;
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.Credentials = new NetworkCredential(userName, password);
                    webClient.UploadFile(uploadUri, Settings.HtmlFileName);
                }
                Console.WriteLine("アップロード完了(" + DateTime.Now + ")");
            }
            catch (Exception e)
            {
                Console.WriteLine("アップロード失敗(" + DateTime.Now + ")");
                Console.WriteLine(e);
            }
        }

        private static void CreateRankingHtml(int interval)
        {
            var rssList = GetRssList();
            var blogs = GetBlogsFromRssList(rssList, interval);
            var container = new ArticleContainer();
            foreach (var blog in blogs)
            {
                foreach (var article in blog.Articles)
                {
                    container.Add(article);
                }
            }
            var rankingData = GetRankingData(container);
            OutputHtml(rankingData);
        }

        public static ICollection<string> GetRssList()
        {
            var rssList = new List<string>();
            using (var reader = new StreamReader(Settings.RssListFileName, Encoding.GetEncoding(Settings.TextEncoding)))
            {
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    if (line == "") break;
                    rssList.Add(line);
                }
            }
            for (var i = 0; i < rssList.Count; i++)
            {
                for (var j = 0; j < rssList.Count; j++)
                {
                    if (i != j && rssList[i] == rssList[j])
                    {
                        throw new Exception("RSSリストが重複している(" + rssList[i] + ")");
                    }
                }
            }
            return rssList;
        }

        private static ICollection<Blog> GetBlogsFromRssList(ICollection<string> rssList, int interval)
        {
            var blogs = new List<Blog>();
            foreach (var rss in rssList)
            {
                try
                {
                    Blog blog = new Blog(rss);
                    blogs.Add(blog);
                }
                catch
                {
                    Console.WriteLine("RSSの読み込みに失敗(" + rss + ")");
                }
                Thread.Sleep(1000 * interval);
            }
            return blogs;
        }

        private static Article[][] GetRankingData(ArticleContainer container)
        {
            var data = container.Bundles.Where(b => b.Articles.Count >= 2).Select(b => b.Articles.OrderByDescending(a => a.Date.Ticks).ToArray());
            data = data.OrderByDescending(aa => aa.Length).ThenByDescending(aa => aa[0].Date.Ticks).Take(Settings.MaxRank);
            return data.ToArray();
        }

        private static void OutputHtml(Article[][] rankingData)
        {
            using (StreamWriter writer = new StreamWriter(Settings.HtmlFileName, false, Encoding.GetEncoding(Settings.TextEncoding)))
            {
                HtmlUtility.BeginHtml(writer);
                HtmlUtility.WriteRankingData(writer, rankingData);
                HtmlUtility.EndHtml(writer);
            }
        }
    }
}
