using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TreeGUI
{
    static class Extensions
    {

        public static string SafeString(this string str)
        {
            return str == null ? "" : str;
        }

        public static IEnumerable<T> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> recursiveFunc)
        { 
            if (source != null)
            {
                foreach (var mainItem in source)
                {
                    yield return mainItem;

                    IEnumerable<T> recursiveSequence = (recursiveFunc(mainItem) ?? new T[] { }).SelectRecursive(recursiveFunc);

                    if (recursiveSequence != null)
                    {
                        foreach (var recursiveItem in recursiveSequence)
                        {
                            yield return recursiveItem;
                        }
                    }
                }
            }
        }

        public static List<T> SelectRecursiveList<T>(this List<T> source, Func<T, IEnumerable<T>> recursiveFunc)
        {
            return SelectRecursive(source as IEnumerable<T>, recursiveFunc).ToList() ;
        }

        public static void SetNodeIndex(this TreeNode node, int Idx)
        {
            if (node.Index == Idx)
                return;
                TreeNode parent = node.Parent;
                node.Remove();
                parent.Nodes.Insert(Idx, node);
        }

        public static int HoursCount(this TimeSpan span)
        {
            return span.Days * 24 + span.Hours;
        }
    }

}
