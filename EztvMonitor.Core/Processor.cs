using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Configuration;

namespace EztvMonitor.Core
{
    public class Processor
    {
        public const string SEARCHESFILENAME = "searches.xml";
        public bool ProcessorAlive { get; set; }

        public string Url { get; set; }
        public string TorrentPath { get; set; }
        public string ExecutionPath { get; set; }

        public TorrentCollection NewTorrents { get; set; }

        public TorrentCollection ProcessedTorrents { get; set; }

        public SearchCollection SearchCollection { get; set; }

        private ManualResetEvent _stopEvent;

        private Thread _collectionThread;
        private Thread _processingThread;

        public void Start()
        {
            ProcessorAlive = true;

            ExecutionPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            ExecutionPath = System.IO.Path.GetDirectoryName(ExecutionPath) + "\\";

            NewTorrents = new TorrentCollection();
            ProcessedTorrents = new TorrentCollection(ExecutionPath + "ProcessedTorrents.xml");

            SearchCollection = SearchCollection.Load(ExecutionPath + SEARCHESFILENAME);

            Url = ConfigurationManager.AppSettings["Url"];
            if (String.IsNullOrWhiteSpace(Url))
            {
                throw new Exception("No URL supplied!");
            }

            TorrentPath = ConfigurationManager.AppSettings["TorrentPath"];
            if (!Directory.Exists(TorrentPath))
            {
                Directory.CreateDirectory(TorrentPath);
            }

            _stopEvent = new ManualResetEvent(false);
            //Create threads
            _collectionThread = new Thread(Collection) { IsBackground = true, Name = "CollectionThread" };
            _processingThread = new Thread(Processing) { IsBackground = true, Name = "ProcessThread" };

            //Start Threads
            _collectionThread.Start();
            _processingThread.Start();
        }

        public void Stop()
        {
            //Kill Process
            ProcessorAlive = false;

            _stopEvent.Set();

            //Wait for threads to tidy up aborted processes 
            _collectionThread.Join();
            _processingThread.Join();
        }

        protected void Monitor()
        {
            while (ProcessorAlive)
            {
                Sleep(5000);
                if (DateTime.Now.Second <= 5)
                {
                    SearchCollection = SearchCollection.Load(SEARCHESFILENAME);
                }
            }
        }

        protected void Collection()
        {
            string tempfile = ExecutionPath + "temp.file";

            while (ProcessorAlive)
            {
                try
                {
                    var tempCollection = new TorrentCollection();
                    var uri = new Uri(Url);
                    var request = (HttpWebRequest)WebRequest.Create(uri);
                    var response = (HttpWebResponse)request.GetResponse();
                    var rStream = response.GetResponseStream();
                    if (rStream != null)
                    {
                        var reader = new StreamReader(rStream, Encoding.Default);
                        using (Stream fileStream = File.OpenWrite(tempfile))
                        {
                            using (var sw = new StreamWriter(fileStream, Encoding.Default))
                            {
                                sw.Write(reader.ReadToEnd());
                                sw.Flush();
                                sw.Close();
                            }
                        }

                        var lines = File.ReadAllLines(tempfile);

                        for (var i = 0; i < lines.Length; i++)
                        {
                            var torrent = new Torrent();

                            if (lines[i].IndexOf("forum_header_border") > -1)
                            {
                                while (lines[i].IndexOf("epinfo") < 0)
                                {
                                    i++;
                                }
                                int startindex = lines[i].IndexOf("title=\"", 0);
                                if (startindex > -1)
                                {
                                    int length = lines[i].IndexOf("\"", startindex + 10) - startindex;
                                    torrent.Name = lines[i].Substring(startindex + 7, length - 7);
                                }
                                while (lines[i].IndexOf("download_1") < 0)
                                {
                                    i++;
                                }

                                var splitlinks = lines[i].Replace("</a>", "|").Split('|');

                                foreach (
                                    var link in
                                        splitlinks.Where(
                                            link => link.IndexOf("download") > -1 && link.IndexOf(".torrent") > -1))
                                {
                                    startindex = link.IndexOf("href=\"", 0);
                                    if (startindex > -1)
                                    {
                                        var length = link.IndexOf("\"", startindex + 8) - startindex;
                                        torrent.Url = link.Substring(startindex + 6, length - 6);
                                        break;
                                    }
                                }
                            }

                            if (torrent.Name != null)
                            {
                                if (SearchCollection.Any(sk => torrent.Name.ContainsSearchKey(sk)))
                                {
                                    tempCollection.Add(torrent);
                                }
                            }
                        }
                    }

                    NewTorrents.AddRange(tempCollection);
                    tempCollection.Clear();
                }
                catch (Exception ex)
                {
                    Logger.LogMessage(System.Diagnostics.EventLogEntryType.Error, GlobalResources.EVENTSOURCE , ex.GetMessage("Collection"), 0);
                }
                Sleep(10000);
            }
        }

        protected void Processing()
        {
            while (ProcessorAlive)
            {
                try
                {
                    lock (NewTorrents)
                    {
                        foreach (var torrent in NewTorrents.Where(nt => !ProcessedTorrents.Any(pt => nt.Name == pt.Name)))
                        {
                            try
                            {
                                var uri = new Uri(torrent.Url);
                                var request = (HttpWebRequest)WebRequest.Create(uri);
                                var response = (HttpWebResponse)request.GetResponse();
                                using (var rStream = response.GetResponseStream())
                                {
                                    if (rStream != null)
                                    {
                                        var reader = new StreamReader(rStream, Encoding.Default);
                                        Stream fileStream = File.OpenWrite(TorrentPath + torrent.Name + ".torrent");
                                        using (var sw = new StreamWriter(fileStream, Encoding.Default))
                                        {
                                            sw.Write(reader.ReadToEnd());
                                            sw.Flush();
                                            sw.Close();
                                        }
                                    }
                                }

                                ProcessedTorrents.AddProcessed(torrent);
                            }
                            catch (Exception ex)
                            {
                                Logger.LogMessage(System.Diagnostics.EventLogEntryType.Error, GlobalResources.EVENTSOURCE, ex.GetMessage("Processing.InForEach"), 0);
                            }
                        }

                        NewTorrents.Clear();
                    }

                    Sleep(10000);
                }
                catch (Exception ex)
                {
                    Logger.LogMessage(System.Diagnostics.EventLogEntryType.Error, GlobalResources.EVENTSOURCE, ex.GetMessage("Processing.OutsideForEach"), 1);
                }
            }
        }

        private void Sleep(int milliseconds)
        {
            _stopEvent.WaitOne(milliseconds, false);
        }

    }
}
