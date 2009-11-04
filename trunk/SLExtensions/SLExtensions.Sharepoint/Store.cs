namespace SLExtensions.Sharepoint
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions;

    public static class Store
    {
        #region Fields

        private static IsolatedStorageFile _storage;
        private static Dictionary<string, SharepointList> cache = new Dictionary<string, SharepointList>();

        private static object storageSync = new object();

        #endregion Fields

        #region Properties


        public static IsolatedStorageFile Storage
        {
            get
            {
                if (_storage == null)
                    _storage = IsolatedStorageFile.GetUserStoreForApplication();
                return _storage;
            }
        }

        private static IEnumerable<Type> KnownTypes
        {
            get
            {
                return ColumnTypes.RegisteredColumnTypes.Values.Concat(new Type[] { ColumnTypes.FallbackColumnType });
            }
        }

        #endregion Properties

        #region Methods

        public static void ClearAll()
        {
            lock (storageSync)
            {
                cache.Clear();
                Storage.Remove();
                _storage = null;
            }
        }

        public static void IncreaseQuota(long size)
        {
            lock (storageSync)
            {
                long quota = size;
                if (Store.Storage.Quota < quota)
                    Store.Storage.IncreaseQuotaTo(quota);
            }
        }

        public static Dictionary<string, byte[]> ReadDownloadedData(string listName, string id)
        {
            lock (storageSync)
            {
                var nomalizedName = NormalizeName(listName);
                var file = nomalizedName + "/dl/" + id + ".dat";

                if (!Storage.FileExists(file))
                    return null;

                DataContractSerializer dldataserializer = new DataContractSerializer(typeof(Dictionary<string, byte[]>));

                using (var stream = Storage.OpenFile(file, System.IO.FileMode.Open))
                {
                    return (Dictionary<string, byte[]>)dldataserializer.ReadObject(stream);
                }
            }
        }

        public static TemplateDataBase[] ReadListData(SharepointList list)
        {
            lock (storageSync)
            {
                var nomalizedName = NormalizeName(list.Name);
                if (!Storage.DirectoryExists(nomalizedName))
                {
                    return null;
                }

                var fileName = nomalizedName + "/data.dat";
                if (!Storage.FileExists(fileName))
                    return null;

                DataContractSerializer dataserializer = new DataContractSerializer(typeof(TemplateDataBase[]), KnownTypes);

                try
                {
                    using (var stream = Storage.OpenFile(fileName, System.IO.FileMode.Open))
                    {
                        return (TemplateDataBase[])dataserializer.ReadObject(stream);
                    }
                }
                catch
                {
                    Storage.DeleteFile(fileName);
                    return null;
                }
            }
        }

        public static T ReadListMetadata<T>(string listName)
            where T : SharepointList, new()
        {
            lock (storageSync)
            {
                T spl;
                SharepointList splCached;
                if (cache.TryGetValue(listName, out splCached)
                    && splCached is T)
                {
                    return (T)splCached;
                }

                spl = GetListMetadata<T>(listName);
                cache[listName] = spl;
                return spl;
            }
        }

        public static void SaveListData(SharepointList list, ICollection<TemplateDataBase> data)
        {
            lock (storageSync)
            {
                var nomalizedName = NormalizeName(list.Name);

                DataContractSerializer serializer = new DataContractSerializer(typeof(SharepointList), KnownTypes);

                Storage.CreateDirectory(nomalizedName);

                var filename = nomalizedName + "/metadata.dat";
                if (Storage.FileExists(filename))
                    Storage.DeleteFile(filename);

                var datafilename = nomalizedName + "/data.dat";
                if (Storage.FileExists(datafilename))
                    Storage.DeleteFile(datafilename);

                var dlfolder = nomalizedName + "/dl/";

                if (!Storage.DirectoryExists(dlfolder))
                {
                    Storage.CreateDirectory(dlfolder);
                }
                else
                {
                    string[] dlFiles = Storage.GetFileNames(dlfolder + "*.*");
                    foreach (var toBeDeleted in dlFiles.Except(from s in data
                                                               select s.Id + ".dat"))
                    {
                        Storage.DeleteFile(dlfolder + toBeDeleted);
                    }
                }

                using (var stream = Storage.OpenFile(filename, System.IO.FileMode.CreateNew))
                {
                    serializer.WriteObject(stream, list);
                }

                DataContractSerializer dataserializer = new DataContractSerializer(typeof(TemplateDataBase[]), KnownTypes);
                using (var stream = Storage.OpenFile(datafilename, System.IO.FileMode.CreateNew))
                {
                    dataserializer.WriteObject(stream, data.ToArray());
                }

                DataContractSerializer dldataserializer = new DataContractSerializer(typeof(Dictionary<string, byte[]>));
                foreach (var item in data)
                {
                    if (item.DownloadedData != null
                        && item.DownloadedData.Count > 0)
                    {
                        var dldatafilename = dlfolder + item.Id + ".dat";
                        if (Storage.FileExists(dldatafilename))
                            Storage.DeleteFile(dldatafilename);

                        using (var stream = Storage.OpenFile(dldatafilename, System.IO.FileMode.CreateNew))
                        {
                            dldataserializer.WriteObject(stream, item.DownloadedData);
                        }
                    }
                    item.DownloadedData = null;
                }
            }
        }

        private static T CreateEmptyList<T>(string listName)
            where T : SharepointList, new()
        {
            return new T { Name = listName };
        }

        private static void DeleteListData(string listName)
        {
            lock (storageSync)
            {
                var nomalizedName = NormalizeName(listName);
                if (Storage.DirectoryExists(nomalizedName))
                {
                    var dataFileName = nomalizedName + "/data.dat";
                    if (Storage.FileExists(dataFileName))
                        Storage.DeleteFile(dataFileName);

                    var dlfolder = nomalizedName + "/dl/";
                    if (Storage.DirectoryExists(dlfolder))
                    {
                        foreach (var item in Storage.GetFileNames(dlfolder + "*.*"))
                        {
                            Storage.DeleteFile(dlfolder + item);
                        }
                        Storage.DeleteDirectory(dlfolder);
                    }

                    Storage.DeleteDirectory(nomalizedName);
                }
            }
        }

        private static T GetListMetadata<T>(string listName)
            where T : SharepointList, new()
        {
            lock (storageSync)
            {
                var nomalizedName = NormalizeName(listName);
                if (!Storage.DirectoryExists(nomalizedName))
                {
                    return CreateEmptyList<T>(listName);
                }

                DataContractSerializer serializer = new DataContractSerializer(typeof(T), KnownTypes);

                var fileName = nomalizedName + "/metadata.dat";
                if (!Storage.FileExists(fileName))
                {
                    DeleteListData(listName);
                    return CreateEmptyList<T>(listName);
                }

                using (var stream = Storage.OpenFile(fileName, System.IO.FileMode.Open))
                {
                    try
                    {
                        var storedlist = (T)serializer.ReadObject(stream);
                        return storedlist;
                    }
                    catch
                    {
                        return CreateEmptyList<T>(listName);
                    }
                }
            }
        }

        private static string NormalizeName(string name)
        {
            return Uri.EscapeUriString(name);
        }

        #endregion Methods
    }
}