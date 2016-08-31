using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace EztvMonitor.Core
{
    public class Torrent
    {
        public string Url { get; set; }
        public string Name { get; set; }

        public XElement Serialize()
        {
            return new XElement("torrent",
                                new XElement("url", Url),
                                new XElement("name", Name));
        }

        public static Torrent Deserialize(XElement doc)
        {
            return new Torrent
                       {
                           Url = doc.Element("url").Value,
                           Name = doc.Element("name").Value
                       };
        }
    }

    public class TorrentCollection : List<Torrent>
    {
        public XDocument XmlDocument { get; set; }
        public string FileName { get; set; }

        public TorrentCollection()
        {

        }

        public TorrentCollection(string fileName)
        {
            this.XmlDocument = File.Exists(fileName) ? LoadDocument(fileName) : CreateDocument(fileName);
            this.FileName = fileName;
        }

        public void AddProcessed(Torrent torrent)
        {
            this.Add(torrent);
            XmlDocument.Element("torrents").Add(torrent.Serialize());
            XmlDocument.Save(FileName);

            Logger.LogMessage(System.Diagnostics.EventLogEntryType.Information, GlobalResources.EVENTSOURCE, "Torrent Saved: " + torrent.Name, 0);
        }

        private XDocument LoadDocument(string fileName)
        {
            var doc = XDocument.Load(fileName);

            this.AddRange(doc.Descendants("torrent").Select(Torrent.Deserialize));

            return doc;
        }

        private static XDocument CreateDocument(string fileName)
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            doc.Add(new XElement("torrents"));

            doc.Save(fileName);
            return doc;
        }
    }
}
