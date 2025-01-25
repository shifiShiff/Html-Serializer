using Html_Serializer;
using HtmlSerializer;
using System.Text.RegularExpressions;
using System.Xml.Linq;

static string FirstWordInString(string s)
{
    return s.Trim().Split(' ')[0];
}
static void BuildTree(List<string> htmLines, HtmlElement rootElement)
{
    HtmlElement currentElement = rootElement;
    string tagName, remainingContent;

    //סוגי תגיות
    var AllTags = HtmlHelper.Instance.AllTags;
    var SelfClosingTags = HtmlHelper.Instance.SelfClosingTags;

    foreach (var line in htmLines.Skip(1))
    {
        if (line.StartsWith('/')) //מקרה של תגית סוגרת או סוף ה HTML 
        {
            if (FirstWordInString(line.Substring(1)) == "html")
            {
                rootElement = rootElement.Children[0];
                return;
            }
            currentElement = currentElement.Parent;
        }
        else
        {
            tagName = FirstWordInString(line);
            if (AllTags.Contains(tagName) || SelfClosingTags.Contains(tagName)) //תגית כלשהיא
            {

                //עדכון העץ
                HtmlElement newElement = new HtmlElement();
                currentElement.Children.Add(newElement);
                newElement.Parent = currentElement;
                currentElement = newElement;

                currentElement.Name = tagName;

                remainingContent = line.Substring(tagName.Length).Trim();
                var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(remainingContent);

                foreach (Match match in attributes) //חלוקה ל attributes
                {
                    if (match.Groups[1].Value == "id")
                        currentElement.Id = match.Groups[2].Value;

                    else if (match.Groups[1].Value == "class")
                        currentElement.Classes = match.Groups[2].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

                    else //מאפיין אחר לא ID & Classes
                        currentElement.Attributes.Add(match.Groups[1].Value, match.Groups[2].Value);
                }

                if (SelfClosingTags.Contains(tagName) || remainingContent.EndsWith('/'))
                    currentElement = currentElement.Parent;

            }
            else // InnerHtml
            {
                currentElement.InnerHtml = line;
            }
        }
    }
}

var html = await Load(" https://hebrewbooks.org/beis ");

// ניקוי רווחים מיותרים
//string cleanHtml = new Regex("[\\t\\n\\r\\v\\f]").Replace(html, "");
//cleanHtml = Regex.Replace(cleanHtml, @"[ ]{2,}", "");
//var htmLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0);
var cleanHtml = new Regex("\\s").Replace(html, " ");
var tagMatches = Regex.Matches(cleanHtml, @"<\/?([a-zA-Z][a-zA-Z0-9]*)\b[^>]*>|([^<]+)").Where(l => !String.IsNullOrWhiteSpace(l.Value));

var htmLines = new List<string>();
foreach (Match item in tagMatches)
{
    string tag = item.Value.Trim();
    if (tag.StartsWith('<'))
        tag = tag.Trim('<', '>');
    htmLines.Add(tag);
}

//יצירת אובייקט השורש 
HtmlElement rootElement = new HtmlElement();

//בניית העץ
BuildTree(htmLines, rootElement);

var res = rootElement.FindElementsBySelector(Selector.ConvertToSelector("ul.nav.navbar-nav"));
Selector selector = Selector.ConvertToSelector("div a.inactBG");

HashSet<HtmlElement> elements = rootElement.FindElementsBySelector(selector);
foreach (HtmlElement element in elements)
{
    Console.WriteLine(element);
    Console.WriteLine("**********");
}
Console.WriteLine(elements.Count);


Console.ReadLine();

async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}


