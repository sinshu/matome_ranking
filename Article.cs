using System;
using System.Text;

namespace MatomeRanking
{
    public class Article
    {
        private Blog blog;
        private string uri;
        private DateTime date;
        private string title;
        private string normalizedTitle;

        internal Article(Blog blog, string uri, DateTime date, string title)
        {
            this.blog = blog;
            this.uri = uri;
            this.date = date;
            this.title = title;
            normalizedTitle = NormalizeTitle(title);

            if (this.date > DateTime.Now)
            {
                this.date = DateTime.Now;
            }
        }

        public static bool AreSame(Article x, Article y)
        {
            int length = Math.Max(x.NormalizedTitle.Length, y.NormalizedTitle.Length);
            if (length < 5)
            {
                return false;
            }

            double d1 = 1 - (double)Math.Min(x.NormalizedTitle.Length, y.NormalizedTitle.Length)
                / Math.Max(x.NormalizedTitle.Length, y.NormalizedTitle.Length);
            if (d1 >= Settings.StringDistanceThreshold)
            {
                return false;
            }

            double d2 = (double)TextUtility.GetLevenshteinDistance(x.NormalizedTitle, y.NormalizedTitle) / length;
            if (d2 >= Settings.StringDistanceThreshold)
            {
                return false;
            }

            return true;
        }

        private static string NormalizeTitle(string title)
        {
            string s = TextUtility.Normalize(title).Replace("w", "");
            StringBuilder sb = new StringBuilder();
            char prev = '\0';
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != prev)
                {
                    sb.Append(s[i]);
                    prev = s[i];
                    count = 1;
                }
                else
                {
                    if (count < 3)
                    {
                        sb.Append(s[i]);
                        count++;
                    }
                }
            }
            return sb.ToString();
        }

        public Blog Blog
        {
            get
            {
                return blog;
            }
        }

        public string Uri
        {
            get
            {
                return uri;
            }
        }

        public DateTime Date
        {
            get
            {
                return date;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
        }

        public string NormalizedTitle
        {
            get
            {
                return normalizedTitle;
            }
        }
    }
}
