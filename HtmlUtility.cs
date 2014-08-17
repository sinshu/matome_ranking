using System;
using System.IO;
using System.Text;

namespace MatomeRanking
{
    public static class HtmlUtility
    {
        private const string daysOfWeek = "日月火水木金土";

        public static string GetDateString(DateTime date)
        {
            return date.ToString("yyyy/MM/dd") + "(" + daysOfWeek[(int)date.DayOfWeek] + ") " + date.ToString("HH:mm:ss");
        }

        public static string Escape(string s)
        {
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        public static string CreateLink(string uri, string name)
        {
            return "<a href=\"" + Escape(uri) + "\" target=\"_blank\">" + Escape(name) + "</a>";
        }

        public static string CreateLink(string uri, string name, string linkClass)
        {
            return "<a href=\"" + Escape(uri) + "\" target=\"_blank\" class=\"" + linkClass + "\">" + Escape(name) + "</a>";
        }

        public static void BeginHtml(TextWriter writer)
        {
            writer.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\">");
            writer.WriteLine("<html lang=\"ja\">");
            writer.WriteLine("<head>");
            writer.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=" + Settings.TextEncoding + "\">");
            writer.WriteLine("<meta http-equiv=\"Content-Style-Type\" content=\"text/css\">");
            writer.WriteLine("<title>" + Settings.Title + "</title>");
            writer.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"style.css\">");
            writer.WriteLine("</head>");
            writer.WriteLine("<body>");
            DoInsertion(writer);
            writer.WriteLine("<div class=\"titlebox\">");
            writer.WriteLine("<table>");
            writer.WriteLine("  <tr><td class=\"maintitle\">オワタらんきんぐ ＼(^o^)／</td></tr>");
            writer.WriteLine("  <tr><td class=\"subtitle\">～ よくまとめられるネタのまとめ ～</td></tr>");
            writer.WriteLine("</table>");
            writer.WriteLine("</div>");
        }

        public static void DoInsertion(TextWriter writer)
        {
            if (Settings.Debug) return;
            using (var reader = new StreamReader(Settings.InsertionFileName, Encoding.GetEncoding(Settings.TextEncoding)))
            {
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    writer.WriteLine(line);
                }
            }
        }

        public static void EndHtml(TextWriter writer)
        {
            writer.WriteLine("</body>");
            writer.WriteLine("</html>");
        }

        public static void WriteRankingData(TextWriter writer, Article[][] rankingData)
        {
            writer.WriteLine("<table class=\"ranking\">");
            var rank = 1;
            foreach (var articles in rankingData)
            {
                writer.WriteLine("  <tr><td class=\"ranknumber\">" + rank + "位</td><td class=\"ranktitle\">" + CreateLink(articles[0].Uri, articles[0].Title, "ranktitle") + "</td></tr>");
                writer.WriteLine("  <tr><td></td><td class=\"message\">↓のまとめサイトでまとめられました。</td></tr>");
                writer.WriteLine("  <tr>");
                writer.WriteLine("    <td></td>");
                writer.WriteLine("    <td>");
                writer.WriteLine("      <ul>");
                foreach (var article in articles)
                {
                    writer.WriteLine("        <li><span class=\"date\">" + GetDateString(article.Date) + "</span> " + CreateLink(article.Uri, article.Blog.Title) + "</li>");
                }
                writer.WriteLine("      </ul>");
                writer.WriteLine("    </td>");
                writer.WriteLine("  </tr>");
                rank++;
            }
            writer.WriteLine("</table>");
        }
    }
}
