using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace EztvMonitor.Core
{
    public class Search
    {
        public Search(string key)
        {
            this.SearchKey = key;
            SplitKey = new Dictionary<string, bool>();
            key.Split('|').ToList<string>().ForEach(s =>
            {
                if (s.StartsWith("!"))
                {
                    SplitKey.Add(s.TrimStart('!'), false);
                }
                else
                {
                    SplitKey.Add(s, true);
                }
            });
        }

        public string SearchKey { get; set; }
        public Dictionary<string, bool> SplitKey { get; set; }
    }

    public class SearchCollection : List<Search>
    {
        public static SearchCollection Load(string fileName)
        {
            var collection = new SearchCollection();

            var xdoc = XDocument.Load(fileName);

            collection.AddRange(xdoc.Descendants("search").Select(xs => new Search(xs.Value)));

            return collection;
        }
    }

    public static class StringExtentions
    {
        public static bool ContainsSearchKey(this string pattern, Search key)
        {
            return key.SplitKey.All(k => k.Value == pattern.ToLower().Contains(k.Key.ToLower()));
        }
    }
}
