namespace SLExtensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Xml;
    using System.Xml.Linq;

    public static class IsolatedStorageExtensions
    {
        #region Methods

        public static string GetApplicationKeyValue(this IsolatedStorageFile store, string filename, string key)
        {
            var list = GetApplicationKeyValuePairs(store, filename);
            if (list == null)
                return null;

            return (from kv in list
                    where kv.Key == key
                    select kv.Value).FirstOrDefault();
        }

        public static IList<KeyValuePair<string, string>> GetApplicationKeyValuePairs(this IsolatedStorageFile store, string filename)
        {
            if (store.FileExists(filename))
            {
                //XmlReader need to be in UI thread
                //XmlReader reader = XmlReader.Create(store.OpenFile(filename, FileMode.OpenOrCreate, FileAccess.Read));
                //XElement docElement = XDocument.ReadFrom(reader) as XElement;
                XDocument docElement;
                using (StreamReader streamReader = new StreamReader(store.OpenFile(filename, FileMode.OpenOrCreate, FileAccess.Read)))
                {
                    docElement = XDocument.Parse(streamReader.ReadToEnd());
                }

                var result = from entry in docElement.Elements("entries").Elements("entry")
                             select new KeyValuePair<string, string>(entry.GetAttribute("key"), entry.GetAttribute("value"));
                return result.ToList();
            }
            else
                return null;
        }

        public static void SaveApplicationKeyValuePairs(this IsolatedStorageFile store, string filename, params KeyValuePair<string, string>[] entries)
        {
            SaveApplicationKeyValuePairs(store, filename, (IEnumerable<KeyValuePair<string, string>>)entries);
        }

        public static void SaveApplicationKeyValuePairs(this IsolatedStorageFile store, string filename, IEnumerable<KeyValuePair<string, string>> entries)
        {
            XDocument doc = null;
            if (entries != null)
            {
                doc = new XDocument(new XElement("entries",
                                                (from e in entries
                                                 select new XElement("entry",
                                                             new XAttribute("key", e.Key),
                                                             new XAttribute("value", e.Value))).ToArray()
                                                            )
                                            );
            }

            if (store.FileExists(filename))
                store.DeleteFile(filename);

            if (doc != null)
            {
                using (IsolatedStorageFileStream stream = store.CreateFile(filename))
                {
                    doc.Save(stream);
                    stream.Flush();
                }
            }
        }

        public static void SetApplicationKeyValue(this IsolatedStorageFile store, string filename, string key, string value)
        {
            var kv = GetApplicationKeyValuePairs(store, filename) ?? new List<KeyValuePair<string, string>>();

            for (int i = kv.Count - 1; i >= 0; i--)
            {
                if (kv[i].Key == key)
                    kv.RemoveAt(i);
            }

            kv.Add(new KeyValuePair<string, string>(key, value));
            store.SaveApplicationKeyValuePairs(filename, kv);
        }

        #endregion Methods
    }
}