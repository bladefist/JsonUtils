using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Xamasoft.JsonClassGenerator
{
    public static class JsonExtentions
    {
        public static string Test()
        {
            return "asf";
        }

        private static IEnumerable<JToken> WalkTokens(JToken node)
        {
            if (node == null)
                yield break;
            yield return node;
            foreach (var child in node.Children())
                foreach (var childNode in WalkTokens(child))
                    yield return childNode;
        }

        public static HashSet<string> ToHashSet(this JToken root)
        {
            var results = WalkTokens(root);

            var dictionary = new HashSet<string>();

            foreach (var result in results)
            {
                foreach (var child in result.Children<JProperty>())
                    dictionary.Add(child.Name);
            }

            return dictionary;
        }
    }
}
