using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Html_Serializer
{
    public class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public string[] Tags { get; set; }
        public string[] VoidTags { get; set; }
        private HtmlHelper()
        {

            Tags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("HtmlTags.json"));
            VoidTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("HtmlVoidTags.json"));
        }
    }
}
