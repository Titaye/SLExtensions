//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Xml;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Logging;

    /// <summary>
    /// This class loads a module configuration file, if available. It supports
    /// loading the config file from both a web Url and from a resource in the
    /// assembly. See the included Properties\Configuration.xml file for an example
    /// of the entries and format.
    /// </summary>
    internal static class ConfigurationLoader
    {
#region *** XML file download
        
        /// <summary>
        /// The Url location of the configuration file
        /// </summary>
        private static string sm_ConfigUrl;

        /// <summary>
        /// The name of the resource in the current 
        /// </summary>
        private static string sm_ConfigAssembly;

        /// <summary>
        /// This is used to track whether or not we can change the configuration.
        /// Config needs to be loaded before we call SetSource() on the MediaElement.
        /// </summary>
        private static bool sm_fConfigurationAllowed = true;

        /// <summary>
        /// Gets or sets a value indicating whether the configuration is valid
        /// </summary>
        public static bool IsConfigurationAllowed
        {
            get
            {
                return sm_fConfigurationAllowed;
            }

            set
            {
                sm_fConfigurationAllowed = value;
            }
        }

        /// <summary>
        /// Load the configuration from a Url
        /// </summary>
        /// <param name="url">the url to load</param>
        public static void LoadFromUrl(string url)
        {
            if (url != null)
            {
                if (sm_ConfigAssembly != null)
                {
                    Tracer.Trace(
                        TraceChannel.Error,
                        "Configuration Assembly was set before:{0} ignoring URL:{1}",
                        sm_ConfigAssembly,
                        url);

                    return;
                }

                if (!sm_fConfigurationAllowed)
                {
                    Tracer.Trace(TraceChannel.Error, "Configuration URL can only be set before Media Source, ignoring " + url);

                    return;
                }

                sm_ConfigUrl = url;

                OpenConfigUrlAsync();
            }
        }

        /// <summary>
        /// Load a configuration file from an assembly resource
        /// </summary>
        /// <param name="resource">the name of the resource to load</param>
        public static void LoadFromAssembly(string resource)
        {
            if (resource != null)
            {
                if (sm_ConfigUrl != null)
                {
                    Tracer.Trace(
                        TraceChannel.Error,
                        "Configuration URL was set before:{0} ignoring Assembly:{1}",
                        sm_ConfigUrl, 
                        resource);

                    return;
                }

                if (!sm_fConfigurationAllowed)
                {
                    Tracer.Trace(TraceChannel.Error, "Configuration URL can only be set before Media Source, ignoring " + resource);

                    return;
                }

                sm_ConfigAssembly = resource;

                OpenConfigAssembly();
            }
        }
        
        /// <summary>
        /// Helper function which opens the configuration url
        /// </summary>
        private static void OpenConfigUrlAsync()
        {
            try
            {
                WebClient client = new WebClient();
                client.OpenReadCompleted += new OpenReadCompletedEventHandler(ConfigUrl_OpenReadCompleted);
                Uri source = new Uri(sm_ConfigUrl, UriKind.RelativeOrAbsolute);
                client.OpenReadAsync(source);
            }
            catch
            {
                Tracer.Trace(TraceChannel.Error, "OpenConfigUrlAsync failed: {0}", sm_ConfigUrl);                
                sm_ConfigUrl = null;
                throw;
            }
        }

        /// <summary>
        /// Completion function for Url reading
        /// </summary>
        /// <param name="sender">object that sent this event</param>
        /// <param name="e">event args</param>
        private static void ConfigUrl_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            bool failed = true;

            if (!e.Cancelled && e.Error == null)
            {
                if (e.Result != null)
                {
                    failed = false;

                    // Will set parameters into a static class
                    ParseConfigUrl(e.Result);

                    e.Result.Close();
                }
            }

            if (failed)
            {
                Tracer.Trace(
                    TraceChannel.Error, 
                    "configUrl_OpenReadCompleted failed: canceled:{0} Error:{1} Url:{2}", 
                    e.Cancelled, 
                    e.Error, 
                    sm_ConfigUrl);

                sm_ConfigUrl = null;
            }
        }
        
#endregion *** XML file download

#region *** XML file from assembly

        /// <summary>
        /// Helper function which loads the configuration from a resource in the assembly
        /// </summary>
        private static void OpenConfigAssembly()
        {
            XmlReader configXml = XmlReader.Create(sm_ConfigAssembly);
            ParseConfig(configXml);
        }
        
#endregion *** XML file from assembly
        
#region *** XML file parsing

        /// <summary>
        /// Read in the xml config from a stream
        /// </summary>
        /// <param name="configUrlStream">stream to read</param>
        private static void ParseConfigUrl(Stream configUrlStream)
        {
            XmlReader configXml = XmlReader.Create(configUrlStream);

            ParseConfig(configXml);
        }

        /// <summary>
        /// Read in the xml config from the assembly string
        /// </summary>
        private static void ParseConfigAssembly()
        {
            XmlReader configXml = XmlReader.Create(sm_ConfigAssembly);

            ParseConfig(configXml);
        }

        /// <summary>
        /// Do the actual parsing logic. See the sample configuration file for an example
        /// of what's in there.
        /// </summary>
        /// <param name="configXml">xml to parse</param>
        private static void ParseConfig(XmlReader configXml)
        {
            if (!configXml.Read())
            {
                Tracer.Trace(TraceChannel.Error, "Configuration does not have a single parsable element");
                
                return;
            }

            if (!configXml.IsStartElement("ExpressionEncoderVideoConfig"))
            {
                Tracer.Trace(TraceChannel.Error, "Configuration root element must be <ExpressionEncoderVideoConfig>");
                
                return;
            }

            if (!ValidateVersion(configXml))
            {
                return;
            }

            while (configXml.Read())
            {
                if (DoSkipThis(configXml))
                {
                    continue;
                }

                if (configXml.IsStartElement("Heuristics"))
                {
                    ParseConfigHeuristics(configXml);
                }
                else if (configXml.IsStartElement("Playback"))
                {
                    ParsePlayback(configXml);
                }
                else
                {
                    // Ignore unknown areas
                }

                if (EndElement(configXml, "ExpressionEncoderVideoConfig"))
                {
                    break;
                }
            }
        }
        
        /// <summary>
        /// Parse the Heuristics section of the config file
        /// </summary>
        /// <param name="configXml">the xml we are parsing</param>
        private static void ParseConfigHeuristics(XmlReader configXml)
        {
            while (configXml.Read())
            {
                if (DoSkipThis(configXml))
                {
                    continue;
                }

                if (configXml.IsStartElement("Network"))
                {
                    ParseConfigHeuristicsNetwork(configXml);
                }

                if (EndElement(configXml, "Heuristics"))
                {
                    break;
                }
            }
        }
        
        /// <summary>
        /// Parse all of the network settings
        /// </summary>
        /// <param name="configXml">xml chunk for the network setings</param>
        private static void ParseConfigHeuristicsNetwork(XmlReader configXml)
        {
            while (configXml.Read())
            {
                if (DoSkipThis(configXml))
                {
                    continue;
                }

                if (configXml.IsStartElement("MaxBufferSize"))
                {
                    string seconds = configXml.GetAttribute("Seconds");
                    if (seconds != null)
                    {
                        Configuration.Heuristics.Network.MaxBufferSize = Convert.ToInt32(seconds, CultureInfo.InvariantCulture);
                        Trace("MaxBufferSize:" + seconds); 
                    }
                }
                else if (configXml.IsStartElement("UpperBufferFullness"))
                {
                    string seconds = configXml.GetAttribute("Seconds");
                    if (seconds != null)
                    {
                        Configuration.Heuristics.Network.UpperBufferFullness = Convert.ToInt32(seconds, CultureInfo.InvariantCulture);
                        Trace("UpperBufferFullness:" + seconds); 
                    }
                }
                else if (configXml.IsStartElement("LowerBufferFullness"))
                {
                    string seconds = configXml.GetAttribute("Seconds");
                    if (seconds != null)
                    {
                        Configuration.Heuristics.Network.LowerBufferFullness = Convert.ToInt32(seconds, CultureInfo.InvariantCulture);
                        Trace("LowerBufferFullness:" + seconds); 
                    }
                }
                else if (configXml.IsStartElement("PanicBufferFullness"))
                {
                    string seconds = configXml.GetAttribute("Seconds");
                    if (seconds != null)
                    {
                        Configuration.Heuristics.Network.PanicBufferFullness = Convert.ToInt32(seconds, CultureInfo.InvariantCulture);
                        Trace("PanicBufferFullness:" + seconds); 
                    }
                }
                else if (configXml.IsStartElement("TryImprovingBitRatePeriod"))
                {
                    string seconds = configXml.GetAttribute("Seconds");
                    if (seconds != null)
                    {
                        Configuration.Heuristics.Network.TryImprovingBitratePeriod = Convert.ToInt32(seconds, CultureInfo.InvariantCulture);
                        Trace("TryImprovingBitRatePeriod:" + seconds); 
                    }
                }
                else if (configXml.IsStartElement("CacheBandwidthFactor"))
                {
                    string value = configXml.GetAttribute("Value");
                    if (value != null)
                    {
                        Configuration.Heuristics.Network.CacheBandwidthFactor = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                        Trace("CacheBandwidthFactor:" + value); 
                    }
                }
                else if (configXml.IsStartElement("CacheBandwidthMin"))
                {
                    string kbps = configXml.GetAttribute("Kbps");
                    if (kbps != null)
                    {
                        Configuration.Heuristics.Network.CacheBandwidthMin = Convert.ToInt32(kbps, CultureInfo.InvariantCulture) * 1000;
                        Trace("CacheBandwidthMin:" + kbps); 
                    }
                }

                if (EndElement(configXml, "Network"))
                {
                    break;
                }
            }
        }
        
        /// <summary>
        /// Parse the playback section
        /// </summary>
        /// <param name="configXml">the xml we are parsing</param>
        private static void ParsePlayback(XmlReader configXml)
        {
            while (configXml.Read())
            {
                if (DoSkipThis(configXml))
                {
                    continue;
                }

                if (configXml.IsStartElement("MaxMissingOrCorruptedChunks"))
                {
                    string count = configXml.GetAttribute("Count");
                    if (count != null)
                    {
                        Configuration.Playback.MaxMissingOrCorruptedChunks = Convert.ToInt32(count, CultureInfo.InvariantCulture);
                        Trace("MaxMissingOrCorruptedChunks:" + count);
                    }
                }

                if (EndElement(configXml, "Playback"))
                {
                    break;
                }
            }
        }

#endregion *** XML file parsing

#region *** Helper functions
        
        /// <summary>
        /// Validate the version tag from the config file
        /// </summary>
        /// <param name="configXml">config file to parse</param>
        /// <returns>true if valid config version</returns>
        private static bool ValidateVersion(XmlReader configXml)
        {
            bool isValid = false;
            int majorVersion = -1;
            int minorVersion = -1;
            
            string value = configXml.GetAttribute("MajorVersion");
            if (value != null)
            {
                // Major version used for validation
                majorVersion = Convert.ToInt32(value, CultureInfo.InvariantCulture);

                if (Configuration.MajorVersion == majorVersion)
                {
                    isValid = true;
                }
            }

            value = configXml.GetAttribute("MinorVersion");
            if (value != null)
            {
                // Minor version logged in the user feedback
                minorVersion = Convert.ToInt32(value, CultureInfo.InvariantCulture);

                Configuration.MinorVersion = minorVersion;
            }

            if (isValid)
            {
                Trace("Valid ConfigVersion:" + majorVersion.ToString(CultureInfo.InvariantCulture) +
                      "." + minorVersion.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                Trace("Invalid ConfigVersion:" +
                      majorVersion.ToString(CultureInfo.InvariantCulture) +
                      "." + minorVersion.ToString(CultureInfo.InvariantCulture));
                
                Trace("Using ConfigVersion:" +
                      Configuration.MajorVersion.ToString(CultureInfo.InvariantCulture) +
                      "." + Configuration.MinorVersion.ToString(CultureInfo.InvariantCulture));                
            }
            
            return isValid;
        }
        
        /// <summary>
        /// Skip any whitespace
        /// </summary>
        /// <param name="configXml">xml to parse</param>
        /// <returns>true if skipped</returns>
        private static bool DoSkipThis(XmlReader configXml)
        {
            if (configXml.NodeType == XmlNodeType.Whitespace ||
                configXml.NodeType == XmlNodeType.Comment)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Is this an end tag?
        /// </summary>
        /// <param name="configXml">config to parse</param>
        /// <param name="name">tag to close</param>
        /// <returns>true if closed</returns>
        private static bool EndElement(XmlReader configXml, string name)
        {
            if (configXml.NodeType == XmlNodeType.EndElement &&
                configXml.Name == name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Trace helper
        /// </summary>
        /// <param name="log">trace message</param>
        private static void Trace(string log)
        {
            MS.Internal.Expression.Encoder.AdaptiveStreaming.Heuristic.HeuristicsImpl.NhTrace("CONF", "{0}", log);
        }        
#endregion *** Helper functions        
    }
}

