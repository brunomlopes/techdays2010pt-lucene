import clr
clr.AddReferenceToFile("HtmlAgilityPack")

from System.IO import Directory, Path, File
from HtmlAgilityPack import HtmlWeb, HtmlEntity
from System.Text import Encoding
import itertools

def full_url_for(url):
    if not url.startswith("/"):
        url = "/"+ url
    return "http://www.techdays2010.com" + url

class Session(object):
    def __init__(self, name, link):
        self.name = name
        self.link = link
        try:
            session_html = HtmlWeb().Load(full_url_for(link))
            session_details_node = session_html.DocumentNode.SelectSingleNode("//div[@id='SessionDetails']")
            description_texts = [HtmlEntity.DeEntitize(n.InnerText).strip() for n in session_details_node.ChildNodes 
                                 if n.InnerText.strip() != "" and n.Name in ["p","#text"] and len(n.Attributes) == 0]
            self.description = " ".join(description_texts)
        except:
            print "Error with session at %s" % link
            raise
    def __str__(self):
        return self.name
    def __repr__(self):
        return "<SessionLink @ %d '%s'->'%s'>" % (id(self), self.name, self.link)

class Speaker(object):
    def __init__(self, node):
        self.name = HtmlEntity.DeEntitize(node.SelectNodes("./h2")[0].InnerText.strip())
        titleNode = node.SelectNodes("./p[@class='title']")
        if titleNode != None:
            self.title = titleNode[0].InnerText.strip()
        else:
            self.title = ""
        self.link = node.SelectNodes("./h2/a")[0].Attributes["href"].Value
        self.photo_link = node.SelectSingleNode("./a[@class='Photo']/img").Attributes["src"].Value
        self.fetch_bio_and_sessions()
    def fetch_bio_and_sessions(self):
        speaker_page = HtmlWeb().Load(full_url_for(self.link), "GET")
        speaker_details_node = speaker_page.DocumentNode.SelectSingleNode("//div[@id='SpeakerDetails']")
        self.bio = HtmlEntity.DeEntitize(speaker_details_node.ChildNodes[6].InnerText).Trim()
        session_links = speaker_details_node.SelectNodes("./p/a")
        try:
            if session_links != None:
                self.sessions = [Session(HtmlEntity.DeEntitize(a.InnerText.Trim()), a.Attributes["href"].Value) for a in speaker_details_node.SelectNodes("./p/a")
                                 if a.Attributes["href"].Value.startswith("/Event/Session/Details/")]
            else:
                self.sessions = []
        except:
            print "Error fetching sessions for speaker %s" % repr(self.name)
            raise
    def __str__(self): 
        return "Speaker '%s'" % self.name
    def __repr__(self): 
        return "Speaker %s" % repr(self.name)


def dump_speakers():
    html = HtmlWeb().Load(full_url_for("/Event/Speaker/Index"), "GET")

    speaker_nodes = html.DocumentNode.SelectNodes("//div[@class='Speaker']")
    
    speaker_directory = Path.GetFullPath("speakers")
    for speaker in (Speaker(node) for node in speaker_nodes):
        print "Dumping %s" % repr(speaker)
        speaker_dir = Path.Combine(speaker_directory, speaker.name)
        if not Directory.Exists(speaker_dir): Directory.CreateDirectory(speaker_dir)
        File.WriteAllText(Path.Combine(speaker_dir, "bio.txt"), speaker.bio, Encoding.UTF8)
        File.WriteAllText(Path.Combine(speaker_dir, "link.txt"), speaker.link, Encoding.UTF8)
        File.WriteAllText(Path.Combine(speaker_dir, "photo_link.txt"), speaker.photo_link, Encoding.UTF8)
        File.WriteAllText(Path.Combine(speaker_dir, "title.txt"), speaker.title, Encoding.UTF8)
        for i, session in enumerate(speaker.sessions):
            session_dir = Path.Combine(speaker_dir, "session_%d" % i)
            if not Directory.Exists(session_dir): Directory.CreateDirectory(session_dir)
            File.WriteAllText(Path.Combine(session_dir, "name.txt"), session.name, Encoding.UTF8)
            File.WriteAllText(Path.Combine(session_dir, "link.txt"), session.link, Encoding.UTF8)
            File.WriteAllText(Path.Combine(session_dir, "description.txt"), session.description, Encoding.UTF8)


dump_speakers()

