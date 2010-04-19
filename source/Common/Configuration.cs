namespace Common
{
    public enum DocumentTypes { Speaker, Session }

    public class Configuration
    {
        public static string SpeakersDirectory = @"T:\speakers";
        public static string IndexDirectory = @"T:\index";
        public class Fields
        {
            public static string DocumentType = "document_type";
            public static string Name = "name";
            public static string Description = "description";
            public static string Link = "link";
        }
    }
}