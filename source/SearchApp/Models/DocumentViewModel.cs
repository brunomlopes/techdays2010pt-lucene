namespace SearchApp.Models
{
    public class DocumentViewModel
    {
        public DocumentViewModel(string name, string description, string link)
        {
            Name = name;
            Description = description.Replace("\n","<br/>");
            Link = "http://www.techdays2010.com"+link;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Link { get; private set; }
    }
}
