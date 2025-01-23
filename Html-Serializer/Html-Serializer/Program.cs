
using Html_Serializer;

using System.Text.RegularExpressions;

static string GetFirstWord(string str)
{
    if (string.IsNullOrWhiteSpace(str))
    {
        return string.Empty;
    }

    string[] words = str.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    return words.Length > 0 ? words[0] : string.Empty;
}

static string RemoveFirstWord(string input)
{
    if (string.IsNullOrWhiteSpace(input))
    {
        return string.Empty;
    }

    string[] words = input.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    return string.Join(" ", words.Skip(1));
}

static HtmlElement CreateElementsTree(string html)
{

    var cleanHtml = Regex.Replace(html, "[\\s]", " ").Trim();
    var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => !string.IsNullOrEmpty(s));

    HtmlElement root = new HtmlElement() { Name = "root" };
    HtmlElement current = root;

    foreach (var line in htmlLines)
    {
        string tagName = GetFirstWord(line.Trim());
        if (string.IsNullOrEmpty(tagName)) continue;
        if (tagName.Equals("/html", StringComparison.OrdinalIgnoreCase))
        {
            return root;
        }
        if (tagName.StartsWith("/"))
        {
            current = current.Parent ?? root;
        }
        else if (HtmlHelper.Instance.Tags.Contains(tagName))
        {
            HtmlElement child = new HtmlElement() { Name = tagName };
            // Extract attributes.
            var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(RemoveFirstWord(line));
            foreach (Match attribute in attributes)
            {
                child.AddAttributes(attribute.Value);
            }
            current.Children.Add(child);
            child.Parent = current;

            if (!HtmlHelper.Instance.VoidTags.Contains(tagName))
            {
                current = child;
            }
        }
        else
        {
            current.InnerHtml += line.Trim();
        }
    }
    return root;
}

//var html = await Load("https://forum.netfree.link/category/1/%D7%94%D7%9B%D7%A8%D7%96%D7%95%D7%AA ");
var html = await Load("https://chani-k.co.il/sherlok-game/");
//var html = await Load("https://learn.malkabruk.co.il/practicode/projects/pract-2/#_3");
//var html = await Load("https://hebrewbooks.org/beis");


Selector selector = Selector.ConvertQuery("img #logo_copyright_img");

HtmlElement root = new HtmlElement();
root = CreateElementsTree(html);

var result = root.Query(selector);
result.ToList().ForEach(element => { Console.WriteLine(element.ToString()); });


Console.ReadLine();
async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
