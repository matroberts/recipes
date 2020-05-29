using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NUnit.Framework;

namespace tools
{
    [TestFixture]
    public class Admin
    {
        public string SiteRootPath { get; } = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\..\site");
        public string IndexTemplate { get; } = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\index-template.html");


        /*
         * These are the admin scripts for the robertsfamiliyrecipes website
         *
         * 1. MakeIndex         - makes the index page                                  - you need to run this after making a new post, or if you want to change the homepage in another way
         * 2. MakeSitemap       - makes the search engine sitemap                       - you need to run this after making a new post
         */

        [Test]
        public void MakeIndex()
        {


            var postLinks = new StringBuilder();
            foreach (var path in Directory.EnumerateFiles(Path.Combine(SiteRootPath, "mains"), "*.*", SearchOption.AllDirectories))
            {
                var doc = new HtmlDocument();
                doc.Load(path);
                var title = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;
                var description = doc.DocumentNode.SelectSingleNode("//head/meta[@name='description']")?.Attributes["content"].Value ?? "";
                var filename = Path.GetFileName(path);
                postLinks.AppendLine($"<dt><a href=\"./mains/{filename}\">{title}</a></dt><dd>{description}</dd>");
            }

            string content = $@"
<dl>
{postLinks}
</dl>
            ";

            var template = File.ReadAllText(IndexTemplate);
            var newpost = template
                .Replace("TODO-CONTENT", content)
                .Replace("TODO-DATE", DateTime.UtcNow.ToString("yyyy-MM-dd"))
                ;

            File.WriteAllText(Path.Combine(SiteRootPath, "index.html"), newpost, new UTF8Encoding(true));

        }

        [Test, Ignore("")]
        public void MakeSitemap()
        {
            var extraLinks = new List<string>()
            {
                "https://moleseyhill.com/code/Pomodoro/Timer.htm",
                "https://moleseyhill.com/code/RsVsShrtCt/Resharper-VisualStudio-Shortcuts.html",
                "https://moleseyhill.com/code/Typing/Lesson.htm",
            };

            var excludedFiles = new List<string>()
            {
                "404.html",
            };

            var canonicalLinks = new List<string>();
            foreach (var file in Directory.GetFiles(SiteRootPath).Where(f => excludedFiles.Contains(Path.GetFileName(f)) == false).Where(f => f.EndsWith("html") == true).OrderByDescending(f => f))
            {
                var doc = new HtmlDocument();
                doc.Load(Path.Combine(SiteRootPath, file), new UTF8Encoding(true));
                doc.OptionEmptyCollection = true;

                var canonicalLink = doc.DocumentNode.SelectSingleNode("//head/link[@rel='canonical']").Attributes["href"].Value;
                canonicalLinks.Add(canonicalLink);
            }

            canonicalLinks.AddRange(extraLinks);
            File.WriteAllLines(Path.Combine(SiteRootPath, "sitemap.txt"), canonicalLinks, new UTF8Encoding(true));
        }
    }
}
