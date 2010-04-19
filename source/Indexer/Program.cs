using System.Collections.Generic;
using System.IO;
using Common;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace Indexer
{
    class Program
    {
        static void Main(string[] args)
        {
            var indexWriter = new IndexWriter(Configuration.IndexDirectory,
                                              new StandardAnalyzer(),
                                              true,
                                              IndexWriter.MaxFieldLength.UNLIMITED);

            var indexedSessions = new HashSet<string>();

            foreach(var speakerDirectory in Directory.GetDirectories(Configuration.SpeakersDirectory))
            {
                var document = DocumentForSpeaker(speakerDirectory);

                indexWriter.AddDocument(document);

                foreach (var sessionDirectory in Directory.GetDirectories(speakerDirectory))
                {
                    var link = File.ReadAllText(Path.Combine(sessionDirectory, "link.txt"));
                    if(indexedSessions.Contains(link))
                    {
                        continue;
                    }
                    indexedSessions.Add(link);
                    var sessionDocument = DocumentForSession(link, sessionDirectory);
                    indexWriter.AddDocument(sessionDocument);
                }
            }
            indexWriter.Close();
        }

        private static Document DocumentForSpeaker(string speakerDirectory)
        {
            var document = new Document();
            var speakerName = Path.GetFileName(speakerDirectory);

            var speakerNameField = new Field(Configuration.Fields.Name,
                                             new StringReader(speakerName));
            document.Add(speakerNameField);
                
            var speakerTypeField = new Field(Configuration.Fields.DocumentType,
                                             new StringReader(DocumentTypes.Speaker.ToString()));
            document.Add(speakerTypeField);

            var speakerBioField = new Field(Configuration.Fields.Description,
                                            new StreamReader(Path.Combine(speakerDirectory,"bio.txt")));
            document.Add(speakerBioField);
                
            var linkField = new Field(Configuration.Fields.Link,
                                      new StreamReader(Path.Combine(speakerDirectory,"link.txt")));
            document.Add(linkField);
            return document;
        }

        private static Document DocumentForSession(string link, string sessionDirectory)
        {
            var sessionDocument = new Document();
            var sessionNameField = new Field(Configuration.Fields.Name,
                                             new StreamReader(Path.Combine(sessionDirectory, "name.txt")));
            sessionDocument.Add(sessionNameField);

            var sessionTypeField = new Field(Configuration.Fields.DocumentType,
                                             new StringReader(DocumentTypes.Session.ToString()));
            sessionDocument.Add(sessionTypeField);


            var sessionDescription = new Field(Configuration.Fields.Description,
                                      new StreamReader(Path.Combine(sessionDirectory, "description.txt")));
            sessionDocument.Add(sessionDescription);

            var linkField = new Field(Configuration.Fields.Link,
                                      new StringReader(link));
            sessionDocument.Add(linkField);
            return sessionDocument;
        }
    }
}
