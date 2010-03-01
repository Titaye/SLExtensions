namespace SLMedia.PlaylistProvider.MSNVideo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Xml.Linq;

    using SLExtensions;

    using SLMedia.Core;

    //    <video xmlns="urn:schemas-microsoft-com:msnvideo:catalog" publishState="Published" lcid="1036" version="3">
    //  <uuid>be5bf55e-2a89-4314-91ff-f44cbcade1fe</uuid>
    //  <providerId>be5bf55e-2a89-4314-91ff-f44cbcade1fe</providerId>
    //  <csId>FRFR_msncinema</csId>
    //  <source friendlyName="MSN Cinéma">FRFR_msncinema</source>
    //  <pageGroup>MSVF10</pageGroup>
    //  <title>Blindness (bande annonce)</title>
    //  <description>Le pays est frappé par une épidémie de cécité qui se propage à une vitesse fulgurante. Les premiers contaminés sont mis en quarantaine dans un hôpital désaffecté où ils sont rapidement livrés à eux-mêmes. Seule une femme n'a pas été touchée par la " blancheur lumineuse ". Elle va les guider pour échapper aux instincts les plus vils et leur faire reprendre espoir en la condition humaine. Le 8 OCTOBRE AU CINEMA</description>
    //  <startDate>2008-09-18T02:24:17Z</startDate>
    //  <activeEndDate>2099-12-31T00:00:00Z</activeEndDate>
    //  <searchableEndDate>2099-12-31T00:00:00Z</searchableEndDate>
    //  <archiveEndDate>2099-12-31T00:00:00Z</archiveEndDate>
    //- <tags>
    //  <tag market="fr-fr" namespace="MSNVideo_Top_Cat">FRFR_cinetv</tag>
    //  <tag market="fr-fr" namespace="MSNVideo_Top_Cat">frfr_msncinema</tag>
    //  <tag market="fr-fr" namespace="MSNVideo_Top_Cat">frfrcinema_frfrcinema_ba</tag>
    //  <tag market="fr-fr" namespace="VC_Source">FRFR_msncinema:FRFR_msncinema</tag>
    //  <tag market="fr-fr" namespace="VC_Supplier">FRFR_msncinema</tag>
    //  </tags>
    //- <videoFiles>
    //+ <videoFile formatCode="1002" msnFileId="B193B737-74C0-4602-984A-DA6169862032">
    //  <uri>mms://msnvideofr.wmod.llnwd.net/a2926/d1/frfr_cinema/ba_blindness.wmv</uri>
    //  </videoFile>
    //- <videoFile formatCode="1003" msnFileId="F45584EA-953B-43FE-9A65-DAC850F24816">
    //  <uri>http://msnvideofr.vo.llnwd.net/d1/frfr_cinema/ba_blindness.flv</uri>
    //  </videoFile>
    //  </videoFiles>
    //- <files>
    //- <file formatCode="2001" msnFileId="729A6FF8-3B3C-453E-A877-F450EA06FAA2" height="240" width="426">
    //  <uri>http://content2.catalog.video.msn.com/e2/ft/share1/faa2/0/426x240_00036_153.jpg</uri>
    //  </file>
    //- <file formatCode="2007">
    //  <uri>http://img4.catalog.video.msn.com/image.aspx?uuid=be5bf55e-2a89-4314-91ff-f44cbcade1fe&w=136&h=102</uri>
    //  </file>
    //- <file formatCode="2009">
    //  <uri>http://img4.catalog.video.msn.com/image.aspx?uuid=be5bf55e-2a89-4314-91ff-f44cbcade1fe&w=400&h=300</uri>
    //  </file>
    //  </files>
    //- <extendedXml>
    //  <relatedLinks />
    //  </extendedXml>
    //- <usage>
    //  <usageItem counterType="1" hourlyCount="2" hourlyChange="0" dailyCount="15" weeklyCount="15" monthlyCount="15" totalCount="15" totalAverage="1.266667" />
    //  </usage>
    //  </video>
    public class Video : SLMedia.Video.VideoItem
    { 
        #region Fields

        public const string NamespaceMsnVideoCatalog = "urn:schemas-microsoft-com:msnvideo:catalog";

        private const string UriSchemeMMS = "mms://";

        #endregion Fields

        #region Properties


        private IEnumerable<File> files;

        public IEnumerable<File> Files
        {
            get
            {
                return this.files;
            }

            set
            {
                if (this.files != value)
                {
                    this.files = value;
                    if (value != null)
                        Thumbnail = (from f in value
                                     select Convert.ToString(f.Url)).FirstOrDefault();
                    else
                        Thumbnail = null;
                }
            }
        }

        
        [ScriptableMemberAttribute]
        public string LCID
        {
            get;
            set;
        }

        [ScriptableMemberAttribute]
        public string MSNSource
        {
            get;
            set;
        }

        [ScriptableMemberAttribute]
        public string MSNSourceFriendlyName
        {
            get;
            set;
        }

        public string UUID
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }

        private IEnumerable<VideoFile> videos;

        public IEnumerable<VideoFile> Videos
        {
            get
            {
                return this.videos;
            }

            set
            {
                if (this.videos != value)
                {
                    this.videos = value;
                    if (value != null)
                        Source = (from v in value
                                  where v.Url.Scheme == UriSchemeMMS
                                  || System.IO.Path.GetExtension(v.Url.LocalPath) == ".wmv"
                                  select Convert.ToString(v.Url)).FirstOrDefault();
                    else
                        Source = null;
                }
            }
        }

        #endregion Properties

        #region Methods

        public static Video[] FromXml(XDocument doc)
        {
            return (from videos in doc.Elements(XName.Get("videos", NamespaceMsnVideoCatalog))
                   from v in videos.Elements(XName.Get("video", NamespaceMsnVideoCatalog))
                   select new Video
                   {
                       LCID = v.GetAttribute("lcid"),
                       Version = v.GetAttribute("version"),
                       UUID = v.GetElementValue(XName.Get("uuid", NamespaceMsnVideoCatalog)),
                       MSNSource = v.GetElementValue(XName.Get("source", NamespaceMsnVideoCatalog)),
                       MSNSourceFriendlyName = v.GetAttribute(XName.Get("source", NamespaceMsnVideoCatalog), "friendlyName"),
                       Title = v.GetElementValue(XName.Get("title", NamespaceMsnVideoCatalog)),
                       Description = v.GetElementValue(XName.Get("description", NamespaceMsnVideoCatalog)),
                       Videos = (from videoFiles in v.Elements(XName.Get("videoFiles", NamespaceMsnVideoCatalog))
                                 from videoFile in videoFiles.Elements(XName.Get("videoFile", NamespaceMsnVideoCatalog))
                                 select VideoFile.FromXml(videoFile)).ToArray(),
                       Files = (from files in v.Elements(XName.Get("files", NamespaceMsnVideoCatalog))
                                from file in files.Elements(XName.Get("file", NamespaceMsnVideoCatalog))
                                select File.FromXml(file)).ToArray(),
                       Categories = new SLExtensions.Collections.ObjectModel.ObservableCollection<Category>((from tags in v.Elements(XName.Get("tags", NamespaceMsnVideoCatalog))
                                     from tag in tags.Elements(XName.Get("tag", NamespaceMsnVideoCatalog))
                                     select new Category { Name = tag.Value }))
                   }).ToArray();
        }

        #endregion Methods
    }
}