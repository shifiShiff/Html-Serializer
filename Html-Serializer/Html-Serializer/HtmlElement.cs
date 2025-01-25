using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Serializer
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }

        //public List<string> Attributes { get; set; } = new List<string>();

        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<string> Classes { get; set; } = new List<string>();
        public string InnerHtml { get; set; } = "";
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();

        
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);
            while (!(queue.Count == 0))
            {
                HtmlElement current = queue.Dequeue();
                yield return current;
                foreach (var child in current.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }


        public HashSet<HtmlElement> FindElementsBySelector(Selector selector)
        {
            HashSet<HtmlElement> res = new HashSet<HtmlElement>();
            ReFindElementsBySelector(this, selector, res);
            return res;
        }
        private void ReFindElementsBySelector(HtmlElement currentElement, Selector selector, HashSet<HtmlElement> res)
        {
            foreach (var element in currentElement.Descendants())
            {
                if (Same(element, selector))
                {
                    if (selector.Child == null)
                    {
                        res.Add(element);
                        continue;
                    }
                    ReFindElementsBySelector(element, selector.Child, res);
                }
            }
        }

        private bool Same(HtmlElement currentElement, Selector selector)
        {
            if (!string.IsNullOrEmpty(selector.TagName) && currentElement.Name != selector.TagName)
                return false;
            if (!string.IsNullOrEmpty(selector.Id) && currentElement.Id != selector.Id)
                return false;
            if (selector.Classes.Any() && !selector.Classes.All(c => currentElement.Classes.Contains(c)))
                return false;
            return true;
        }
        public override string ToString()
        {
            string s = "\nName: " + Name + "\n Id: " + Id + "\n";
            foreach (string atr in Attributes.Keys)
            {
                s += "[" + atr + ": " + Attributes[atr] + "] ";
            }
            if (Classes.Count != 0)
            {
                s += "\nClass:";
                foreach (string class1 in Classes)
                {
                    s += " " + class1;
                }
            }
            s += "\nInnerHtml: " + InnerHtml;
            return s;
        }
    }
}
