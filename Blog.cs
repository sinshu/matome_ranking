using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MatomeRanking
{
    public class Blog
    {
        private string rssUri;
        private string uri;
        private string title;
        private ICollection<Article> articles;

        public Blog(string rssUri)
        {
            this.rssUri = rssUri;
            articles = new Article[] { };
            Reload();
        }

        private void Reload()
        {
            if (Settings.Debug) Console.WriteLine(rssUri);

            XmlDocument document = new XmlDocument();
            document.Load(rssUri);

            uri = document.GetElementsByTagName("link")[0].InnerText;
            title = document.GetElementsByTagName("title")[0].InnerText;

            var newArticles = new List<Article>();

            bool isRss2 = document.DocumentElement.Name == "rss";
            XmlNodeList itemNodes = document.GetElementsByTagName("item");
            foreach (XmlNode itemNode in itemNodes)
            {
                XmlElement itemElement = (XmlElement)itemNode;
                string articleUri = itemElement.GetElementsByTagName("link")[0].InnerText;
                XmlNodeList dateNodes = itemElement.GetElementsByTagName(isRss2 ? "pubDate" : "dc:date");
                DateTime articleDate = DateTime.Parse(dateNodes[0].InnerText);
                string articleTitle = itemElement.GetElementsByTagName("title")[0].InnerText;
                Article newArticle = new Article(this, articleUri, articleDate, articleTitle);
                newArticles.Add(newArticle);
            }

            if (DateTime.Now - newArticles.First().Date >= TimeSpan.FromDays(30))
            {
                Console.WriteLine("警告: " + title + "(" + rssUri +")は1ヵ月以上更新されていない");
            }

            articles = newArticles;
        }

        public string Uri
        {
            get
            {
                return uri;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
        }

        public ICollection<Article> Articles
        {
            get
            {
                return articles;
            }
        }
    }
}
