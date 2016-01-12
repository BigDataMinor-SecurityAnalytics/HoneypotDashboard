using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileWatcher {

    internal class LogReader {
        private readonly DataAccess LogAccess;
        private Dictionary<string, string> XmlModel;
        private XmlReaderSettings ReaderSettings;

        public LogReader(DataAccess logAccess) {
            LogAccess = logAccess;

            XmlModel = new Dictionary<string, string>() {
                ["Nr"] = "./@id",
                ["RemoteDomain"] = "./client/@domain",
                ["RemoteIp"] = "./client/@ip",
                ["RemotePort"] = "./client/@port",
                ["LocalIp"] = "./host/@ip",
                ["LocalPort"] = "./host/@port",
                ["TargetService"] = "./@name",
                ["Protocol"] = "./@protocol",
                ["Content"] = "./sent",
                ["ContentSizeBytes"] = "./sentBytes",
                ["Severity"] = "./@severity",
                ["Time"] = "./start"
            };

            ReaderSettings = new XmlReaderSettings() {
                ConformanceLevel = ConformanceLevel.Fragment
            };
        }

        public Exception AddLogToDB(string logPath) {
            try {
                var doc = new XmlDocument();
                string content = string.Empty;

                using(var fs = new FileStream(logPath, FileMode.Open)) {
                    using(var sr = new StreamReader(fs)) {
                        content = sr.ReadToEnd();
                    }
                }

                content = $"<events>{content}</events>";
                doc.LoadXml(content);

                foreach(XmlNode logEvent in doc.DocumentElement.SelectNodes("./event")) {
                    var nr = int.Parse(logEvent.SelectSingleNode(XmlModel["Nr"]).InnerText.Trim('"'));

                    if(LogAccess.Context.LogEntries.All(entry => entry.Nr != nr)) {
                        var newEntry = EntryFromXml(logEvent);
                        LogAccess.Context.LogEntries.Add(newEntry);
                    }
                }

                LogAccess.SaveChanges();

                return null;
            } catch(Exception e) {
                return e;
            }
        }

        public LogEntry EntryFromXml(XmlNode node) {
            var newEntry = new LogEntry();
            foreach(var propXPathPair in XmlModel.Where(kvp => !string.IsNullOrEmpty(kvp.Value))) {
                var propInfo = newEntry.GetType().GetProperty(propXPathPair.Key);
                if(propInfo != null) {
                    var xmlVal = node.SelectSingleNode(propXPathPair.Value)?.InnerText?.Trim('"');
                    if(xmlVal != null) {
                        //Convert XML to needed property
                        object convertedVal;
                        if(propInfo.PropertyType == typeof(string)) {
                            convertedVal = xmlVal;
                        } else if(propInfo.PropertyType == typeof(int)) {
                            convertedVal = int.Parse(xmlVal);
                        } else if(propInfo.PropertyType == typeof(DateTime)) {
                            convertedVal = DateTime.ParseExact(xmlVal, "yyyy-MM-dd HH:mm:ss:fff", CultureInfo.InvariantCulture);
                        } else {
                            throw new NotSupportedException($"Don't know how to cast {propXPathPair.Key} = {propXPathPair.Value} to {propInfo.PropertyType}");
                        }

                        //Set property
                        propInfo.SetValue(newEntry, convertedVal);
                    }
                }
            }

            try {
                if(!string.IsNullOrEmpty(newEntry.RemoteIp)) {
                    var gottenCountry = IPLocate.GetIpCountry(newEntry.RemoteIp);
                    newEntry.Country = gottenCountry;
                }
            } catch(Exception e) {
                newEntry.Country = string.Empty;
            }

            return newEntry;
        }
    }
}