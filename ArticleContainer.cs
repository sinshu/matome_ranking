using System;
using System.Collections.Generic;
using System.Linq;

namespace MatomeRanking
{
    public class ArticleContainer
    {
        private ICollection<Bundle> bundles;

        public ArticleContainer()
        {
            bundles = new List<Bundle>();
        }

        public void Add(Article article)
        {
            Bundle targetBundle = null;
            foreach (Bundle bundle in bundles)
            {
                foreach (Article existingArticle in bundle.Articles)
                {
                    if (Article.AreSame(existingArticle, article))
                    {
                        targetBundle = bundle;
                    }
                }
            }
            if (targetBundle == null)
            {
                bundles.Add(new Bundle(article));
            }
            else
            {
                targetBundle.Articles.Add(article);
            }
        }

        public ICollection<Bundle> Bundles
        {
            get
            {
                return bundles;
            }
        }

        public class Bundle
        {
            private ICollection<Article> articles;

            public Bundle(Article article)
            {
                articles = new List<Article>();
                articles.Add(article);
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
}
