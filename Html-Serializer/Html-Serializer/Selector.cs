using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Serializer
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public Selector Parent { get; set; }
        public Selector Child { get; set; }


        public static Selector ConvertToSelector(string query)
        {
            var queryParts = query.Split(' ');
            Selector rootSelector = null;
            Selector currentSelector = null;
            var AllTags = HtmlHelper.Instance.Tags;
            foreach (var part in queryParts)
            {
                var newSelector = new Selector();
                if (rootSelector == null)
                {
                    rootSelector = newSelector;
                    currentSelector = rootSelector;
                }
                else
                {
                    currentSelector.Child = newSelector;
                    newSelector.Parent = currentSelector;
                    currentSelector = newSelector;
                }
                var partsTag = part.Split(new[] { '#', '.' }, StringSplitOptions.None);

                for (int i = 0; i < partsTag.Length; i++)
                {
                    if (partsTag[i].Length > 0 && !string.IsNullOrEmpty(partsTag[i]))
                    {
                        if (part.Contains('#' + partsTag[i]))
                            currentSelector.Id = partsTag[i];
                        else if (part.Contains('.' + partsTag[i]))
                            currentSelector.Classes.Add(partsTag[i]);
                        else
                        {
                            if (HtmlHelper.Instance.Tags.Contains(partsTag[i]) || HtmlHelper.Instance.VoidTags.Contains(partsTag[i]))
                            {
                                currentSelector.TagName = partsTag[i];
                            }
                        }
                    }

                }
            }
            return rootSelector;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Parent != null) sb.AppendLine("Parent: \n" + Parent.ToString());
            sb.AppendLine($"Name: {TagName}");
            if (Id != null) sb.AppendLine($"Id: {Id}");
            if (Classes.Count > 0)
            {
                sb.AppendLine("Classes:");
                foreach (var clas in Classes)
                {
                    sb.AppendLine("\t- " + clas);
                }
            }
            if (Child != null)
            {
                sb.AppendLine("Child: ");
                sb.AppendLine("\t||");
                sb.AppendLine("\t\\/");
            }
            return sb.ToString();
        }

    }
}
