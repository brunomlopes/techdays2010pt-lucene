using System.Collections.Generic;
using System.IO;
using Common;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Br;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Indexer
{
    class Program
    {
        public static string[] PortugueseStopWords =
            {
                "a", "à", "ainda", "alem", "ambas", "ambos", "antes",
                "ao", "aonde", "aos", "apos", "aquele", "aqueles",
                "as", "assim", "com", "como", "contra", "contudo",
                "cuja", "cujas", "cujo", "cujos", "da", "das", "de",
                "dela", "dele", "deles", "demais", "depois", "desde",
                "desta", "deste", "dispoe", "dispoem", "diversa",
                "diversas", "diversos", "do", "dos", "durante", "e", "é", 
                "ela", "elas", "ele", "eles", "em", "entao", "entre",
                "essa", "essas", "esse", "esses", "esta", "estas",
                "este", "estes", "ha", "isso", "isto", "logo", "mais",
                "mas", "mediante", "menos", "mesma", "mesmas", "mesmo",
                "mesmos", "na", "nas", "nao", "não", "nas", "nem", "nesse", "neste",
                "nos", "no", "o", "os", "ou", "outra", "outras", "outro", "outros",
                "pela", "pelas", "pelo", "pelos", "perante", "pois", "por",
                "porque", "portanto", "proprio", "propios", "quais", "qual",
                "qualquer", "quando", "quanto", "que", "quem", "quer", "se",
                "seja", "sem", "sendo", "seu", "seus", "sob", "sobre", "sua",
                "suas", "tal", "tambem", "teu", "teus", "toda", "todas", "todo",
                "todos", "tua", "tuas", "tudo", "um", "uma", "umas", "uns"
            };

        static void Main(string[] args)
        {
            var indexWriter = new IndexWriter(new Lucene.Net.Store.SimpleFSDirectory(new DirectoryInfo(Configuration.IndexDirectory)),
                                              new BrazilianAnalyzer(PortugueseStopWords),
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
                                             speakerName,
                                             Field.Store.YES, Field.Index.ANALYZED);
            document.Add(speakerNameField);
                
            var speakerTypeField = new Field(Configuration.Fields.DocumentType,
                                             DocumentTypes.Speaker.ToString(),
                                             Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS);
            document.Add(speakerTypeField);

            var speakerBioField = new Field(Configuration.Fields.Description,
                                            new StreamReader(Path.Combine(speakerDirectory,"bio.txt")).ReadToEnd(),
                                            Field.Store.YES, Field.Index.ANALYZED);
            document.Add(speakerBioField);
                
            var linkField = new Field(Configuration.Fields.Link,
                                      new StreamReader(Path.Combine(speakerDirectory,"link.txt")).ReadToEnd(),
                                      Field.Store.YES, Field.Index.ANALYZED);
            document.Add(linkField);
            return document;
        }

        private static Document DocumentForSession(string link, string sessionDirectory)
        {
            var sessionDocument = new Document();
            var sessionNameField = new Field(Configuration.Fields.Name,
                                             new StreamReader(Path.Combine(sessionDirectory, "name.txt")).ReadToEnd(),
                                             Field.Store.YES, Field.Index.ANALYZED);
            sessionDocument.Add(sessionNameField);

            var sessionTypeField = new Field(Configuration.Fields.DocumentType,
                                             DocumentTypes.Session.ToString(),
                                             Field.Store.YES, Field.Index.ANALYZED);
            sessionDocument.Add(sessionTypeField);


            var sessionDescription = new Field(Configuration.Fields.Description,
                                               new StreamReader(Path.Combine(sessionDirectory, "description.txt")).ReadToEnd(),
                                               Field.Store.YES, Field.Index.ANALYZED);
            sessionDocument.Add(sessionDescription);

            var linkField = new Field(Configuration.Fields.Link,
                                      link,
                                      Field.Store.YES, Field.Index.ANALYZED);
            sessionDocument.Add(linkField);

            return sessionDocument;
        }
    }
}
