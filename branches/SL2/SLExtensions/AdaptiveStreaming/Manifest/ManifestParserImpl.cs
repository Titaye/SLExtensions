//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Manifest
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows.Media;
    using System.Xml;
    using Microsoft.Expression.Encoder.AdaptiveStreaming;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities;

    /// <summary>
    /// This class implements a default manifest parser
    /// </summary>
    internal class ManifestParserImpl : IManifestParser
    {
        /// <summary>
        /// The type of a marker in a TimelineMarker
        /// </summary>
        private const string MarkerType = "NAME";

        /// <summary>
        /// The root element of the manifest
        /// </summary>
        private const string ManifestRootElement = "SmoothStreamingMedia";

        /// <summary>
        /// The major version attribute
        /// </summary>
        private const string ManifestMajorVersionAttribute = "MajorVersion";

        /// <summary>
        /// The minor version attribute
        /// </summary>
        private const string ManifestMinorVersionAttribute = "MinorVersion";

        /// <summary>
        /// The duration attribute
        /// </summary>
        private const string ManifestDurationAttribute = "Duration";

        /// <summary>
        /// Stream index attribute
        /// </summary>
        private const string StreamIndexElement = "StreamIndex";

        private const string StreamIndexTypeAttribute = "Type";
        private const string StreamIndexSubtypeAttribute = "Subtype";
        private const string StreamIndexChunksAttribute = "Chunks";
        private const string StreamIndexUrlAttribute = "Url";
        private const string StreamIndexLanguageAttribute = "Language";

        private const string QualityLevelElement = "QualityLevel";
        private const string QualityLevelBitrateAttribute = "Bitrate";
        private const string QualityLevelFourCCAttribute = "FourCC";
        private const string QualityLevelWidthAttribute = "Width";
        private const string QualityLevelHeightAttribute = "Height";
        private const string QualityLevelCodecPrivateDataAttribute = "CodecPrivateData";
        private const string QualityLevelWaveFormatExAttribute = "WaveFormatEx";

        /// <summary>
        /// Default implementation of the ParseManifest function.
        /// </summary>
        /// <param name="manifestStream">The manifest stream to parse</param>
        /// <param name="manifestUrl">The url of the stream we are parsing</param>
        /// <returns>A ManifestInfo representing this manifest</returns>
        public override ManifestInfo ParseManifest(Stream manifestStream, Uri manifestUrl)
        {
            ManifestInfo info = new ManifestInfo();

            try
            {
                int streamId = 0;
                XmlReader manifest = XmlReader.Create(manifestStream);
                if (!manifest.Read())
                {
                    throw new AdaptiveStreamingException("Manifest does not have a single parsable element.");
                }

                // Check to make sure the first element is <MediaIndex>
                if (!manifest.IsStartElement(ManifestRootElement))
                {
                    throw new AdaptiveStreamingException("Manifest root element must be <" +ManifestRootElement + ">");
                }

                // Get manifest version, do not overwrite -1 if not present
                string major = manifest.GetAttribute(ManifestMajorVersionAttribute);
                string minor = manifest.GetAttribute(ManifestMinorVersionAttribute);
                if (major != null)
                {
                    info.ManifestMajorVersion = Convert.ToInt32(major, CultureInfo.InvariantCulture);
                }

                if (major != null)
                {
                    info.ManifestMinorVersion = Convert.ToInt32(minor, CultureInfo.InvariantCulture);
                }

                // Pull the duration out of the maifest
                string duration = manifest.GetAttribute(ManifestDurationAttribute);

                if (duration != null)
                {
                    info.MediaAttributes.Add(MediaSourceAttributesKeys.Duration, duration.Trim());                                
                }
                else
                {
                    throw new AdaptiveStreamingException("Manifest root must contain a duration attribute");
                }

                // Base URL of the manifest for chunks, we basically cut the manifest file name, e.g. "mbr/manifest.elive" will become "mbr/"
                string manifestBaseUrl = string.Empty;
                int cut1 = manifestUrl.AbsoluteUri.LastIndexOf('/');
                int cut2 = manifestUrl.AbsoluteUri.LastIndexOf('\\');

                if (cut1 < cut2)
                {
                    cut1 = cut2;
                }

                if (cut1 >= 0)
                {
                    manifestBaseUrl = manifestUrl.AbsoluteUri.Substring(0, cut1 + 1);
                }

                while (manifest.Read())
                {
                    if (manifest.IsStartElement(StreamIndexElement))
                    {
                        StreamInfo si = ParseStreamInfo(manifest, streamId, manifestBaseUrl, info);
                        if (si != null)
                        {
                            info.AddStream(si);
                            streamId++;
                        }
                    }

                    // We explicitly ignore content that we don't understand, as well as whitespace, comments etc.
                }

                if (streamId < 1)
                {
                    throw new AdaptiveStreamingException("At least one media stream is needed, none are declared in the manifest.");
                }

                info.Valid = true;
            }
            catch (AdaptiveStreamingException)
            {
                info.Valid = false;
                throw;
            }
            finally
            {
                manifestStream.Close();
            }

            return info;
        }

        /// <summary>
        /// Parse the stream section of the manifest
        /// </summary>
        /// <param name="manifest">The XML dom of the stream section in the manifest</param>
        /// <param name="streamId">The Id of the stream we are parsing</param>
        /// <param name="manifestBaseUrl">the url of the manifest we are parsing</param>
        /// <param name="manifestInfo">the manifest we are parsing</param>
        /// <returns>A StreamInfo describing the stream at streamId, or null if one was not found</returns>
        private static StreamInfo ParseStreamInfo(XmlReader manifest, int streamId, string manifestBaseUrl, ManifestInfo manifestInfo)
        {
            string mediaTypeStr = manifest.GetAttribute(StreamIndexTypeAttribute);
            bool bHaveFirstBitrate = false;

            // Pick out text types since we handle those separately
            if (mediaTypeStr.ToUpper(CultureInfo.InvariantCulture).Equals("TEXT"))
            {
                // Parse the text stream and return null
                ParseTextStream(manifest, manifestInfo);
                return null;
            }

            string baseUrl = manifest.GetAttribute(StreamIndexUrlAttribute);
            int numberOfChunks = Convert.ToInt32(manifest.GetAttribute(StreamIndexChunksAttribute), CultureInfo.InvariantCulture);
            if (mediaTypeStr == null || baseUrl == null || numberOfChunks < 1)
            {
                throw new AdaptiveStreamingException("Stream description in the manifest " + streamId.ToString(CultureInfo.InvariantCulture) + " is missing mandatory attributes (media type, subtype, base URL or number of chunks)");
            }

            MediaStreamType mediaType = mediaTypeStr.ToUpper(CultureInfo.InvariantCulture).Equals("VIDEO") ? MediaStreamType.Video : mediaTypeStr.ToUpper(CultureInfo.InvariantCulture).Equals("AUDIO") ? MediaStreamType.Audio : MediaStreamType.Script;
            if (mediaType == MediaStreamType.Script)
            {
                throw new AdaptiveStreamingException("Stream media type in manifest may be 'audio' or 'video' only");
            }

            if (!baseUrl.ToUpper(CultureInfo.InvariantCulture).StartsWith("HTTP://", StringComparison.OrdinalIgnoreCase))
            {
                baseUrl = manifestBaseUrl + baseUrl;
            }

            // Get the language attribute
            string language = manifest.GetAttribute(StreamIndexLanguageAttribute);

            if (language == null)
            {
                language = string.Empty;
            }

            StreamInfo info = new StreamInfo(baseUrl, language, numberOfChunks, mediaType, streamId);
            ulong maxBitrate = 0;
            int displayAspectRatioWidth = 0;
            int displayAspectRatioHeight = 0;
            int maxBitrateWidth = 0;
            int maxBitrateHeight = 0;
            bool bIsVideoStream = true;

            while (manifest.Read())
            {
                // Get the available bitrates
                if (manifest.IsStartElement(QualityLevelElement))   
                {
                    // Missing or malformed attribute kbps will result in failure, which is what we want
                    ulong bitrate = Convert.ToUInt64(manifest.GetAttribute(QualityLevelBitrateAttribute), CultureInfo.InvariantCulture);
                    Dictionary<MediaStreamAttributeKeys, string> attributes = new Dictionary<MediaStreamAttributeKeys, string>(4);

                    // Get the FourCC for this quality level
                    string fourCC = manifest.GetAttribute(QualityLevelFourCCAttribute);

                    if (fourCC != null)
                    {
                        attributes.Add(MediaStreamAttributeKeys.VideoFourCC, fourCC);
                    }

                    // Get the width of this stream
                    string width = manifest.GetAttribute(QualityLevelWidthAttribute);

                    if (width != null)
                    {
                        attributes.Add(MediaStreamAttributeKeys.Width, width);
                    }

                    // Get the height of this stream
                    string height = manifest.GetAttribute(QualityLevelHeightAttribute);

                    if (height != null)
                    {
                        attributes.Add(MediaStreamAttributeKeys.Height, height);
                    }

                    // Get the video codec data
                    string codecPrivateData = manifest.GetAttribute(QualityLevelCodecPrivateDataAttribute);

                    if (codecPrivateData != null)
                    {
                        attributes.Add(MediaStreamAttributeKeys.CodecPrivateData, codecPrivateData);
                    }

                    // Get the wave format ex. Note we will only have one (codec private data) or the other
                    // (wave format ex)
                    string waveFormatEx = manifest.GetAttribute(QualityLevelWaveFormatExAttribute);

                    if (waveFormatEx != null)
                    {
                        if (codecPrivateData != null)
                        {
                            throw new AdaptiveStreamingException("Cannot have both a CodecPrivateData and a WaveFormatEx attribute in the same QualityLevel element.");
                        }
                        bIsVideoStream = false;
                        attributes.Add(MediaStreamAttributeKeys.CodecPrivateData, waveFormatEx);
                    }

                    if (!bHaveFirstBitrate)
                    {
                        bHaveFirstBitrate = true;

                        if (bIsVideoStream)
                        {
                            displayAspectRatioHeight = int.Parse(height, CultureInfo.InvariantCulture);
                            displayAspectRatioWidth = int.Parse(width, CultureInfo.InvariantCulture);
                        }
                    }

                    // Add this bitrate and these attributes to the stream info
                    info.AddBitrate(bitrate, attributes);

                    if (bitrate > maxBitrate)
                    {
                        maxBitrate = bitrate;

                        if (bIsVideoStream)
                        {
                            maxBitrateHeight = int.Parse(height, CultureInfo.InvariantCulture);
                            maxBitrateWidth = int.Parse(width, CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (manifest.IsStartElement("c"))
                {
                    // Getting chunk information
                    int id = 0;
                    try
                    {
                        // Missing or malformed attributes n or d will result in failure, which is what we want
                        id = Convert.ToInt32(manifest.GetAttribute("n"), CultureInfo.InvariantCulture);

                        // Ignore out-of-range chunk id's to simplify experimental manifest tinkering (truncation for test purposes).
                        if (id < info.NumberOfChunksInStream)
                        {
                            // Add a new media chunk to our stream info
                            ulong chunkDuration = Convert.ToUInt64(manifest.GetAttribute("d"), CultureInfo.InvariantCulture);
                            info.AddMediaChunk(id, chunkDuration);
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        throw new AdaptiveStreamingException(String.Format(CultureInfo.InvariantCulture, "Bad manifest format: chunk ID {0} is out of range.", id), e);
                    }
                }
                else if (manifest.Name.Equals("StreamIndex"))
                {
                    break;
                }

                // We explicitly ignore content that we don't understand, as well as whitespace, comments etc.
            }

            // Let's fix up the aspect ratio of the highest bitrate stream. We need to find the
            // combination that gives us the largest buffer size.
            IDictionary<MediaStreamAttributeKeys, string> mediaAttributes = info.GetAttributesForBitrate(maxBitrate);
            if(bIsVideoStream)
            {
                // First try the width
                int testWidth = displayAspectRatioWidth * maxBitrateHeight;
                testWidth = (int)((double)(testWidth) / (double)displayAspectRatioHeight);
            
                // Now round it up to the nearest four
                testWidth += 3;
                testWidth -= testWidth % 4;

                // Now try the height
                int testHeight = displayAspectRatioHeight * maxBitrateWidth;
                testHeight = (int)((double)(testHeight) / (double)displayAspectRatioWidth);

                // Now round it up to the nearest four
                testHeight+= 3;
                testHeight -= testHeight % 4;

                // Calculate the buffer sizes
                int bufferSizeOriginal = maxBitrateWidth * maxBitrateHeight;
                int bufferSizeWidth = testWidth * maxBitrateHeight;
                int bufferSizeHeight = testHeight * maxBitrateWidth;

                if (bufferSizeWidth >= bufferSizeHeight && bufferSizeWidth >= bufferSizeOriginal)
                {
                    maxBitrateWidth = testWidth;
                }
                else if (bufferSizeHeight >= bufferSizeWidth && bufferSizeHeight >= bufferSizeOriginal)
                {
                    maxBitrateHeight = testHeight;
                }

                mediaAttributes.Remove(MediaStreamAttributeKeys.Width);
                mediaAttributes.Remove(MediaStreamAttributeKeys.Height);
                mediaAttributes.Add(MediaStreamAttributeKeys.Width, maxBitrateWidth.ToString(CultureInfo.InvariantCulture));
                mediaAttributes.Add(MediaStreamAttributeKeys.Height, maxBitrateHeight.ToString(CultureInfo.InvariantCulture));
            }

            // Set the description to be the highest bitrate item.
            info.Description = new MediaStreamDescription(info.MediaType, mediaAttributes);
            info.Valid = true;

            return info;
        }

        /// <summary>
        /// Parse our text streams
        /// </summary>
        /// <param name="manifest">the xml we are parsing</param>
        /// <param name="manifestInfo">the manifest info object we are returning</param>
        private static void ParseTextStream(XmlReader manifest, ManifestInfo manifestInfo)
        {
            // Parse all of the entries
            while (manifest.Read())
            {
                if (manifest.Name.Equals("Marker"))
                {
                    string markerTime = manifest.GetAttribute("Time");
                    string markerValue = manifest.GetAttribute("Value");

                    TimelineMarker marker = new TimelineMarker();
                    marker.Type = MarkerType;
                    marker.Time = new TimeSpan((long)Convert.ToUInt64(markerTime, CultureInfo.InvariantCulture));
                    marker.Text = markerValue;
                    manifestInfo.Markers.Add(marker);
                }
                else if (manifest.Name.Equals("ScriptCommand"))
                {
                    string markerTime = manifest.GetAttribute("Time");
                    string markerType = manifest.GetAttribute("Type");
                    string markerCommand = manifest.GetAttribute("Command");

                    TimelineMarker marker = new TimelineMarker();
                    marker.Type = markerType;
                    marker.Time = new TimeSpan((long)Convert.ToUInt64(markerTime, CultureInfo.InvariantCulture));
                    marker.Text = markerCommand;
                    manifestInfo.Markers.Add(marker);
                }
                else if (manifest.Name.Equals("StreamIndex"))
                {
                    break;
                }
            }
        }
    }
}
