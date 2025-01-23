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

        private const string IdAttribute = "id";
        private const string ClassAttribute = "class";

        public void AddAttributes(string attribute)
        {
            if (!attribute.Contains('=')) return;
            string[] key_value = attribute.Split('=');
            string key = key_value[0];
            string value = string.Join(" ", key_value.Skip(1));
            if (key.Equals(IdAttribute))
            {
                Id = value.Trim('"');
            }
            else if (key.Equals(ClassAttribute))
            {
                string[] classes = value.Split(' ');
                foreach (var clas in classes)
                {
                    Classes.Add(clas.Trim('"'));
                }
            }
            else
                Attributes.Add(key, value.Trim('"'));
        }


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
        public IEnumerable<HtmlElement> Query(Selector selector)
        {
            var set = new HashSet<HtmlElement>();
            FindElementBySelector(selector, set, this.Descendants());
            return set;
        }


        private void FindElementBySelector(Selector selector, HashSet<HtmlElement> list, IEnumerable<HtmlElement> elements)
        {
            if (selector == null || elements == null || !elements.Any())
                return;

            foreach (var item in elements)
            {
                if (CheckSelector(item, selector))
                {
                    if (selector.Child == null)
                        list.Add(item);
                    FindElementBySelector(selector.Child, list, item.Descendants());
                }
            }
        }

        public bool CheckSelector(HtmlElement element, Selector selector)
        {
            if (selector.Id != null && !selector.Id.Equals(element.Id))
                return false;
            if (selector.TagName != null && selector.TagName != element.Name)
                return false;
            if (selector.Classes.Count > 0 && !selector.Classes.All(c => element.Classes.Contains(c)))
                return false;
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Name: {Name}");
            if (Id != null) sb.AppendLine($"Id: {Id}");
            if (Classes.Count > 0)
            {
                sb.AppendLine("Classes:")
                    .AppendLine(string.Join(Environment.NewLine, Classes.Select(c => "\t- " + c)));

            }
            if (Attributes.Count > 0)
            {
                sb.AppendLine("Attributes: ")
                    .AppendLine(string.Join(Environment.NewLine, Attributes.Select(attr => $"- {attr.Key}: {attr.Value}")));

            }
            if (InnerHtml.Length > 0) sb.AppendLine("InnerHTML " + InnerHtml);
            if (Parent != null) sb.AppendLine("Parent: " + Parent.Name);
            if (Children.Count > 0)
            {
                sb.AppendLine("Children: ");
                foreach (var child in Children)
                {
                    sb.AppendLine("\t- " + child.Name);
                }
            }
            return sb.ToString();
        }
    }
}
