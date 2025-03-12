using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml;
using static Shisaku_no_Hashi.Data.Models.DataCaptureTypes;

namespace Shisaku_no_Hashi.Pages.Queue
{
    public class IndexModel : PageModel
    {
        public List<PostData> Posts { get; set; }

        public void OnGet()
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "dataloaderweb", "queue");
            Posts = ParseQueueFiles(directoryPath);
        }
         
        private List<PostData> ParseQueueFiles(string directoryPath)
        {
            var posts = new List<PostData>();

            try
            {
                var files = Directory.GetFiles(directoryPath, "*.", SearchOption.AllDirectories)
                    .Where(f => !Path.HasExtension(f));

                foreach (var file in files)
                {
                    try
                    {
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load(file);

                        var postNode = xmlDoc.SelectSingleNode("/post");
                        if (postNode != null)
                        {
                            var post = new PostData
                            {
                                Account = postNode.SelectSingleNode("acc")?.InnerText,
                                Username = postNode.SelectSingleNode("usr")?.InnerText,
                                SessionId = postNode.SelectSingleNode("sid")?.InnerText,
                                Universe = postNode.SelectSingleNode("univ")?.InnerText,
                                Locale = postNode.SelectSingleNode("loc")?.InnerText,
                                SceneId = postNode.SelectSingleNode("sc/id")?.InnerText,
                                Lobby = postNode.SelectSingleNode("lobby")?.InnerText,
                                Events = new List<EventData>()
                            };

                            var eventNodes = postNode.SelectNodes("ev");
                            foreach (XmlNode ev in eventNodes)
                            {
                                var eventData = new EventData
                                {
                                    EventType = (EventTypes)Enum.Parse(typeof(EventTypes), ev.Attributes["type"]?.Value.ToString()),
                                    StartTime = DateTime.Parse(ev.SelectSingleNode("time/st")?.InnerText ?? DateTime.MinValue.ToString()),
                                    EndTime = ev.SelectSingleNode("time/en") != null ?
                                        DateTime.Parse(ev.SelectSingleNode("time/en").InnerText) : null
                                };

                                var objNode = ev.SelectSingleNode("obj");
                                if (objNode != null)
                                {
                                    eventData.Object = new ObjectData
                                    {
                                        Id = objNode.SelectSingleNode("id")?.InnerText,
                                        Content = objNode.SelectSingleNode("con")?.InnerText,
                                        Instance = objNode.SelectSingleNode("ins")?.InnerText
                                    };
                                }

                                post.Events.Add(eventData);
                            }

                            posts.Add(post);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing file {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing directory: {ex.Message}");
            }

            return posts;
        }
    }
}
