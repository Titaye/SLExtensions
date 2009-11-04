// <copyright file="MediaPlayer.cs" company="Microsoft">
//     Copyright © Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>Implements the MediaPlayer class</summary>
// <author>Microsoft Expression Encoder Team</author>
namespace ExpressionMediaPlayer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// This class represents the base class for a MediaPlayer control.
    /// </summary>
    [TemplatePart(Name=MediaPlayer.StretchBox, Type=typeof(FrameworkElement))]
    [TemplatePart(Name=MediaPlayer.VideoWindow, Type=typeof(FrameworkElement))]
    [TemplatePart(Name=MediaPlayer.MediaElement, Type = typeof(MediaElement))]
    [TemplatePart(Name=MediaPlayer.ButtonStart, Type = typeof(Button))]
    [TemplatePart(Name=MediaPlayer.ButtonPlayPause, Type = typeof(Button))]
    [TemplatePart(Name=MediaPlayer.ElementPause, Type = typeof(FrameworkElement))]
    [TemplatePart(Name=MediaPlayer.ElementPlay, Type = typeof(FrameworkElement))]
    [TemplatePart(Name=MediaPlayer.ButtonPrevious, Type = typeof(Button))]
    [TemplatePart(Name=MediaPlayer.ButtonNext, Type = typeof(Button))]
    [TemplatePart(Name=MediaPlayer.ButtonStop, Type = typeof(Button))]
    [TemplatePart(Name=MediaPlayer.ButtonFullScreen, Type = typeof(Button))]
    [TemplatePart(Name=MediaPlayer.ButtonMute, Type = typeof(Button))]
    [TemplatePart(Name=MediaPlayer.ButtonClosedCaptions, Type = typeof(Button))]
    [TemplatePart(Name=MediaPlayer.ButtonVolumeDown, Type = typeof(RepeatButton))]
    [TemplatePart(Name=MediaPlayer.ButtonVolumeUp, Type = typeof(RepeatButton))]
    [TemplatePart(Name=MediaPlayer.ButtonPlaylist, Type = typeof(ToggleButton))]
    [TemplatePart(Name=MediaPlayer.ButtonChapter, Type = typeof(ToggleButton))]
    [TemplatePart(Name=MediaPlayer.SliderPosition, Type = typeof(Slider))]
    [TemplatePart(Name=MediaPlayer.SliderVolume, Type = typeof(Slider))]
    [TemplatePart(Name=MediaPlayer.ListBoxPlaylist, Type = typeof(ListBox))]
    [TemplatePart(Name=MediaPlayer.ListBoxChapters, Type = typeof(ListBox))]
    [TemplatePart(Name=MediaPlayer.TextblockErrorMessage, Type = typeof(TextBlock))]
    [TemplatePart(Name=MediaPlayer.ClosedCaptionBackground, Type = typeof(Rectangle))]
    public class MediaPlayer : Control
    {
        #region DP definitions
        /// <summary>
        /// Using a DependencyProperty as the backing store for Playlist.  This enables animation, styling, binding, etc...        
        /// </summary>
        public static readonly DependencyProperty PlaylistProperty = 
            DependencyProperty.Register("Playlist", typeof(PlaylistCollection), typeof(MediaPlayer), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for PlaybackPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PlaybackPositionProperty =
            DependencyProperty.Register("PlaybackPosition", typeof(Double), typeof(MediaPlayer), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for PlaybackPositionText.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PlaybackPositionTextProperty =
            DependencyProperty.Register("PlaybackPositionText", typeof(String), typeof(MediaPlayer), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for PlaybackDuration.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PlaybackDurationProperty =
            DependencyProperty.Register("PlaybackDuration", typeof(Double), typeof(MediaPlayer), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for PlaybackDurationText.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PlaybackDurationTextProperty =
            DependencyProperty.Register("PlaybackDurationText", typeof(String), typeof(MediaPlayer), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for BufferingPercent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BufferingPercentProperty =
            DependencyProperty.Register("BufferingPercent", typeof(Double), typeof(MediaPlayer), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for PosterImageSource.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PosterImageSourceProperty =
            DependencyProperty.Register("PosterImageSource", typeof(String), typeof(MediaPlayer), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for PosterImageMaxWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PosterImageMaxWidthProperty =
            DependencyProperty.Register("PosterImageMaxWidth", typeof(Double), typeof(MediaPlayer), new PropertyMetadata(Double.PositiveInfinity));

        /// <summary>
        /// Using a DependencyProperty as the backing store for PosterImageMaxHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PosterImageMaxHeightProperty =
            DependencyProperty.Register("PosterImageMaxHeight", typeof(Double), typeof(MediaPlayer), new PropertyMetadata(Double.PositiveInfinity));

        /// <summary>
        /// Using a DependencyProperty as the backing store for BufferingControlVisibility  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BufferingControlVisibilityProperty =
            DependencyProperty.Register("BufferingControlVisibility", typeof(Visibility), typeof(MediaPlayer), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for PosterControlVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PosterControlVisibilityProperty =
            DependencyProperty.Register("PosterControlVisibility", typeof(Visibility), typeof(MediaPlayer), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for DownloadOffsetPercent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DownloadOffsetPercentProperty =
            DependencyProperty.Register("DownloadOffsetPercent", typeof(Double), typeof(MediaPlayer), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for BufferingPercent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DownloadPercentProperty =
            DependencyProperty.Register("DownloadPercent", typeof(Double), typeof(MediaPlayer), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for PlaybackPositionText.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CaptionTextProperty =
            DependencyProperty.Register("CaptionText", typeof(String), typeof(MediaPlayer), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for CaptionsEnabled.
        /// </summary>
        public static readonly DependencyProperty CaptionsVisibilityProperty =
            DependencyProperty.Register("CaptionsVisibility", typeof(Visibility), typeof(MediaPlayer), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Using a DependencyProperty as the backing store for CaptionsButtonVisibilityProperty.  This may show/hide the closed caption button.
        /// </summary>
        public static readonly DependencyProperty CaptionsButtonVisibilityProperty =
            DependencyProperty.Register("CaptionsButtonVisibility", typeof(Visibility), typeof(MediaPlayer), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Using a DependencyProperty as the backing store for UserBackgroundColor.
        /// </summary>
        public static readonly DependencyProperty UserBackgroundColorProperty =
            DependencyProperty.Register("UserBackgroundColor", typeof(Brush), typeof(MediaPlayer), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for DisplayTimecode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DisplayTimeCodeProperty =
            DependencyProperty.Register("DisplayTimeCode", typeof(bool), typeof(MediaPlayer), new PropertyMetadata(false));

        /// <summary>
        /// Using a DependencyProperty as the backing store for HideOnClick.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HideOnClickProperty =
            DependencyProperty.RegisterAttached("HideOnClick", typeof(bool), typeof(MediaPlayer), new PropertyMetadata(false));

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanStep.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanStepProperty =
            DependencyProperty.Register("CanStep", typeof(bool), typeof(MediaPlayer), new PropertyMetadata(false));
        #endregion

        #region Private fields
        /// <summary>
        /// String for the stretch box template element.
        /// </summary>
        private const string StretchBox = "stretchBox";

        /// <summary>
        /// String for the video window template element.
        /// </summary>
        private const string VideoWindow = "videoWindow";

        /// <summary>
        /// String for the media element template item.
        /// </summary>
        private const string MediaElement = "mediaElement";

        /// <summary>
        /// String for the start button template element.
        /// </summary>
        private const string ButtonStart = "buttonStart";

        /// <summary>
        /// String for the play pause button template element.
        /// </summary>
        private const string ButtonPlayPause = "buttonPlayPause";

        /// <summary>
        /// String for the pause element.
        /// </summary>
        private const string ElementPause = "elementPause";

        /// <summary>
        /// String for the play element.
        /// </summary>
        private const string ElementPlay = "elementPlay";
        
        /// <summary>
        /// String for the previous button template element.
        /// </summary>
        private const string ButtonPrevious = "buttonPrevious";

        /// <summary>
        /// String for the next button template element.
        /// </summary>
        private const string ButtonNext = "buttonNext";

        /// <summary>
        /// String for the stop button template element.
        /// </summary>
        private const string ButtonStop = "buttonStop";

        /// <summary>
        /// String for the full screen template element.
        /// </summary>
        private const string ButtonFullScreen = "buttonFullScreen";

        /// <summary>
        /// String for the mute button template element.
        /// </summary>
        private const string ButtonMute = "buttonMute";

        /// <summary>
        /// String for the closed captions button template element.
        /// </summary>
        private const string ButtonClosedCaptions = "buttonClosedCaptions";

        /// <summary>
        /// String for the volume down button template element.
        /// </summary>
        private const string ButtonVolumeDown = "buttonVolumeDown";

        /// <summary>
        /// String for the volume up button template element.
        /// </summary>
        private const string ButtonVolumeUp = "buttonVolumeUp";

        /// <summary>
        /// String for the playlist button template element.
        /// </summary>
        private const string ButtonPlaylist = "buttonPlaylist";

        /// <summary>
        /// String for the chapter button template element.
        /// </summary>
        private const string ButtonChapter = "buttonChapter";

        /// <summary>
        /// String for the volume slider template element.
        /// </summary>
        private const string SliderVolume = "sliderVolume";

        /// <summary>
        /// String for the step forwards button template element.
        /// </summary>
        private const string ButtonStepForwards = "buttonStepForwards";

        /// <summary>
        /// String for the step backwards template element.
        /// </summary>
        private const string ButtonStepBackwards = "buttonStepBackwards";

        /// <summary>
        /// String for the position slider template element.
        /// </summary>
        private const string SliderPosition = "sliderPosition";

        /// <summary>
        /// String for the playlist list box template element.
        /// </summary>
        private const string ListBoxPlaylist = "listBoxPlaylist";

        /// <summary>
        /// String for the chapters list box template element.
        /// </summary>
        private const string ListBoxChapters = "listBoxChapters";

        /// <summary>
        /// String for the error messages template element.
        /// </summary>
        private const string TextblockErrorMessage = "textblockErrorMessage";

        /// <summary>
        /// String for the closed captions background template element.
        /// </summary>
        private const string ClosedCaptionBackground = "closedCaptionBackground";

        /// <summary>
        /// Minimum time to skip when seeking.
        /// </summary>
        private const double MinDelta = 5.0;

        /// <summary>
        /// Buffer for skip padding.
        /// </summary>
        private const double SkipBuffer = 1.0;

        /// <summary>
        /// The increment for skipping steps.
        /// </summary>
        private const double SkipSteps = 10.0;

        /// <summary>
        /// Default volume.
        /// </summary>
        private const Double VolumeDefault = 0.5;

        /// <summary>
        /// Threshold for volume muting.
        /// </summary>
        private const Double VolumeMuteThreshold = 0.01;

        /// <summary>
        /// Marker type element name.
        /// </summary>
        private const String MarkerType = "NAME";

        /// <summary>
        /// Caption type element name.
        /// </summary>
        private const String CaptionType = "CAPTION";

        /// <summary>
        /// Stretch box framework element.
        /// </summary>
        private FrameworkElement m_elementStretchBox;

        /// <summary>
        /// VideoWindow framework element -- used for internal sizing calculations
        /// </summary>
        private FrameworkElement m_elementVideoWindow;
        
        /// <summary>
        /// The main media element for playing audio and video.
        /// </summary>
        private MediaElement m_mediaElement;

        /// <summary>
        /// The start button.
        /// </summary>
        private Button m_buttonStart;

        /// <summary>
        /// Pause and play button.
        /// </summary>
        private Button m_buttonPlayPause;

        /// <summary>
        /// Framework element for the play button.
        /// </summary>
        private FrameworkElement m_elementPlay;

        /// <summary>
        /// Framework element for the pause button.
        /// </summary>
        private FrameworkElement m_elementPause;

        /// <summary>
        /// The previous button.
        /// </summary>
        private Button m_buttonPrevious;

        /// <summary>
        /// The next button.
        /// </summary>
        private Button m_buttonNext;

        /// <summary>
        /// The stop button.
        /// </summary>
        private Button m_buttonStop;

        /// <summary>
        /// Step forwards button.
        /// </summary>
        private Button m_buttonStepForwards;

        /// <summary>
        /// Step backwards button.
        /// </summary>
        private Button m_buttonStepBackwards;

        /// <summary>
        /// Button which toggles the playlist control.
        /// </summary>
        private ToggleButton m_buttonPlaylist;

        /// <summary>
        /// Button which toggles the chapter control.
        /// </summary>
        private ToggleButton m_buttonChapter;

        /// <summary>
        /// Full screen button.
        /// </summary>
        private Button m_buttonFullScreen;

        /// <summary>
        /// Button which toggles muting the audio stream.
        /// </summary>
        private ToggleButton m_buttonMute;

        /// <summary>
        /// Button which toggles closed captions.
        /// </summary>
        private ToggleButton m_buttonClosedCaptions;

        /// <summary>
        /// Button which turns the volume down.
        /// </summary>
        private RepeatButton m_buttonVolumeDown;

        /// <summary>
        /// Button which turns the volume up.
        /// </summary>
        private RepeatButton m_buttonVolumeUp;

        /// <summary>
        /// Volume slider.
        /// </summary>
        private SensitiveSlider m_sliderVolume;

        /// <summary>
        /// Position slider.
        /// </summary>
        private SensitiveSlider m_sliderPosition;

        /// <summary>
        /// ListBox for the playlist control.
        /// </summary>
        private ListBox m_listBoxPlaylist;

        /// <summary>
        /// ListBox for the chapters control.
        /// </summary>
        private ListBox m_listBoxChapters;

        /// <summary>
        /// Dispatch timer for posting UI messages.
        /// </summary>
        private DispatcherTimer m_timer;

        /// <summary>
        /// Message block for error messages.
        /// </summary>
        private TextBlock m_textBlockErrorMessage;

        /// <summary>
        /// Rectangle for closed captions.
        /// </summary>
        private Rectangle m_rectCaptionBackground;

        /// <summary>
        /// The index of the current playlist item.
        /// </summary>
        private int m_currentPlaylistIndex;

        /// <summary>
        /// Set when  the current playlist item uses adpative streaming.
        /// </summary>
        private bool m_currentItemIsAdaptive;

        /// <summary>
        /// The index of the current chapter item.
        /// </summary>
        private int m_currentChapterIndex;

        /// <summary>
        /// Current play state.
        /// </summary>
        private bool m_inPlayState;

        /// <summary>
        /// Play state when user started dragging the SliderPosition thumb.
        /// </summary>
        private bool m_inPlayStateBeforeSliderPositionDrag;

        /// <summary>
        /// The current media stream source in use (null if not currently using one).
        /// </summary>
        private MediaStreamSource m_mediaStreamSource;

        /// <summary>
        /// Flag for updating the download progress control.
        /// </summary>
        private bool m_downloadProgressNeedsUpdating;

        /// <summary>
        /// The last time the media element was clicked.
        /// </summary>
        private DateTime m_lastMediaElementClick = new DateTime(0);

        /// <summary>
        /// Dispatch timer for fading out controls.
        /// </summary>
        private DispatcherTimer m_timerControlFadeOut;

        /// <summary>
        /// Flag for auto playing (cached).
        /// </summary>
        private bool m_autoPlayCache;

        /// <summary>
        /// Flag for auto loading (cached).
        /// </summary>
        private bool m_autoLoadCache;

        /// <summary>
        /// The current strecth mode.
        /// </summary>
        private System.Windows.Media.Stretch m_stretchMode = System.Windows.Media.Stretch.None;

        /// <summary>
        /// Current control state.
        /// </summary>
        private String currentControlState;

        /// <summary>
        /// Desired control state.
        /// </summary>
        private String desiredControlState;

        /// <summary>
        /// Flag which tracks whether we should perform a seek on the next tick.
        /// </summary>
        private bool m_seekOnNextTick;

        /// <summary>
        /// The position to seek to when m_seekOnNextTick is set.
        /// </summary>
        private double m_seekOnNextTickPosition;

        /// <summary>
        /// Flag which tracks whether we should goto a different next playlist item on the next tick.
        /// </summary>
        private bool m_goToItemOnNextTick;

        /// <summary>
        /// The item to goto when m_goToItemOnNextTick is set.
        /// </summary>
        private int m_goToItemOnNextTickIndex;

        /// <summary>
        /// Flag for tracking mute status in volume slider control.
        /// </summary>
        private bool m_mutedCache;

        /// <summary>
        /// Current unmuted volume.
        /// </summary>
        private Double m_dblUnMutedVolume = 0.5;
        #endregion

        /// <summary>
        /// Initializes a new instance of the MediaPlayer class.
        /// </summary>
        public MediaPlayer()
        {
            DefaultStyleKey = typeof(MediaPlayer);
            SetValue(PlaylistProperty, new PlaylistCollection());
            m_currentPlaylistIndex = -1;
            SetPlaybackPosition(0.0);
            SetPlaybackDuration(0.0);
            TimeSpan timeZero = new TimeSpan();
            SetPlaybackPositionText (TimeSpanStringConverter.ConvertToString(timeZero, ConverterModes.NoMilliseconds));
            SetPlaybackDurationText(TimeSpanStringConverter.ConvertToString(timeZero, ConverterModes.NoMilliseconds));

            SetBufferingPercent(0);
            SetDownloadOffsetPercent(0);
            SetDownloadPercent(0);
        }

        #region events
        /// <summary>
        /// Event which fires when the state of this MediaPlayer changes.
        /// </summary>
        public event RoutedEventHandler StateChanged;

        /// <summary>
        /// Event which fires when a marker is reached.
        /// </summary>
        public event TimelineMarkerRoutedEventHandler MarkerReached;
 
        #endregion

        #region properties

        /// <summary>
        /// Gets the list of items to play.
        /// </summary>
        [System.ComponentModel.Category("Media")]
        public PlaylistCollection Playlist
        {
            get { return (PlaylistCollection)GetValue(PlaylistProperty); }
        }

        /// <summary>
        /// Gets the index of the item currently selected.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public int CurrentPlaylistIndex
        {
            get { return m_currentPlaylistIndex; }
        }

        /// <summary>
        /// Gets the current PlaylistItem selected.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public PlaylistItem CurrentPlaylistItem
        {
            get
            {
                if (m_currentPlaylistIndex >= 0 && m_currentPlaylistIndex < Playlist.Count)
                {
                    return Playlist[m_currentPlaylistIndex];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the current playback position.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public Double PlaybackPosition
        {
            get
            {
                return (Double)GetValue(PlaybackPositionProperty);
            }
        }

        /// <summary>
        /// Gets the text of the playback position.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public String PlaybackPositionText
        {
            get { return (String)GetValue(PlaybackPositionTextProperty); }
        }

        /// <summary>
        /// Gets the duration of the current playlist item.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public Double PlaybackDuration
        {
            get
            {
                return (Double)GetValue(PlaybackDurationProperty);
            }
        }

        /// <summary>
        /// Gets the current duration as a string.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public String PlaybackDurationText
        {
            get { return (String)GetValue(PlaybackDurationTextProperty); }
        }

        /// <summary>
        /// Gets the current buffering percent.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public Double BufferingPercent
        {
            get { return (Double)GetValue(BufferingPercentProperty); }
        }

        /// <summary>
        /// Gets the source of the current poster image.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public String PosterImageSource
        {
            get { return (String)GetValue(PosterImageSourceProperty); }
        }

        /// <summary>
        /// Gets the max width of the current poster image.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public Double PosterImageMaxWidth
        {
            get { return (Double)GetValue(PosterImageMaxWidthProperty); }
        }

        /// <summary>
        /// Gets the max height of the current poster image.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public Double PosterImageMaxHeight
        {
            get { return (Double)GetValue(PosterImageMaxHeightProperty); }
        }

        /// <summary>
        /// Gets the visibility for the buffering control.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public Visibility BufferingControlVisibility
        {
            get { return (Visibility)GetValue(BufferingControlVisibilityProperty); }
        }

        /// <summary>
        /// Gets the visibility of the poster image control.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public Visibility PosterControlVisibility
        {
            get { return (Visibility)GetValue(PosterControlVisibilityProperty); }
        }       

        /// <summary>
        /// Gets the dowloading offset percent.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public Double DownloadOffsetPercent
        {
            get { return (Double)GetValue(DownloadOffsetPercentProperty); }
        }

        /// <summary>
        /// Gets the downloaded percent.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public Double DownloadPercent
        {
            get { return (Double)GetValue(DownloadPercentProperty); }
        }

        /// <summary>
        /// Gets the visibility of the captions button.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public Visibility CaptionsButtonVisibility
        {
            get
            {
                return (Visibility)GetValue(CaptionsButtonVisibilityProperty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we are displaying the time code.
        /// </summary>
        [System.ComponentModel.Category("Media")]
        public Boolean DisplayTimeCode
        {
            get 
            { 
                return (Boolean)GetValue(DisplayTimeCodeProperty); 
            }

            set 
            { 
                SetValue(DisplayTimeCodeProperty, value);
                    if (m_timer == null)
                    {
                        if (value)
                        {
                            CreatePositionTimer(new TimeSpan(0, 0, 0, 0, 40));
                        } 
                        else
                        {
                            CreatePositionTimer(new TimeSpan(0, 0, 0, 0, (6 * 1001 / 30))); // 6 NTSC frames
                        }
                    }
                    else
                    {
                        if (value) 
                        {
                            m_timer.Interval = new TimeSpan(0, 0, 0, 0, 40);
                        } 
                        else 
                        {
                        }
                    }
                }
        }

        /// <summary>
        /// Gets the caption text.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public String CaptionText
        {
            get
            {
                return (String)GetValue(CaptionTextProperty);
            }
        }

        /// <summary>
        /// Gets or sets the visibility of captions.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public Visibility CaptionsVisibility
        {
            get { return (Visibility)GetValue(CaptionsVisibilityProperty); }
            set { SetValue(CaptionsVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the user background color.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public Brush UserBackgroundColor
        {
            get { return (Brush)GetValue(UserBackgroundColorProperty); }
            set { SetValue(UserBackgroundColorProperty, value); }
        }

        /// <summary>
        /// Gets the current media element state.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public MediaElementState CurrentState
        {
            get
            {
                if (m_mediaElement != null)
                {
                    return m_mediaElement.CurrentState;
                }

                return MediaElementState.Stopped;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the media element can step.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool CanStep
        {
            get { return (bool)GetValue(CanStepProperty); }
        }

        /// <summary>
        /// Gets or sets the factory for creating a custom media stream source for adaptive streaming.
        /// </summary>
        public IMediaStreamSourceFactory MediaStreamSourceFactory { get; set; }

        /// <summary>
        /// Gets the media stream source for the currently playing item, or null if not
        /// using one.
        /// </summary>
        protected MediaStreamSource CurrentMediaStreamSource
        {
            get
            {
                return m_mediaStreamSource;
            }
        }

        /// <summary>
        /// Gets the current media element we are using.
        /// </summary>
        protected MediaElement CurrentMediaElement
        {
            get
            {
                return m_mediaElement;
            }
        }

        /// <summary>
        /// Sets the playback position of the position slider.
        /// </summary>
        /// <param name="value">The new playback position.</param>
        protected void SetPlaybackPosition(double value)
        {
            SetValue(PlaybackPositionProperty, value);
            if (((m_sliderPosition != null) && (!m_sliderPosition.IsDragging))  // don't update slider position while the user is dragging
            && (!m_seekOnNextTick) )                                            // don't update the slider position while there is a pending seek -- as this can generate an additional seek request
            {
                // update silder position
                if (m_sliderPosition.Value != value)
                {
                    Debug.WriteLine("set PlaybackPosition updating slider position:" + value.ToString(CultureInfo.CurrentCulture));
                    m_sliderPosition.ValueChanged -= OnSliderPositionChanged; // Don't generate a seek
                    m_sliderPosition.Value = value;
                    m_sliderPosition.ValueChanged += OnSliderPositionChanged;
                }
            }
               
            // update position text as well
            UpdatePositionDisplay();
        }

        /// <summary>
        /// Sets the playback duration of the position slider.
        /// </summary>
        /// <param name="value">The new playback duration.</param>
        protected void SetPlaybackDuration(double value)
        {
            SetValue(PlaybackDurationProperty, value);
            if (m_sliderPosition != null)
            {
                m_sliderPosition.ValueChanged -= OnSliderPositionChanged; // Don't generate a seek
                m_sliderPosition.Value = 0.0;
                m_sliderPosition.Maximum = PlaybackDuration;
                m_sliderPosition.ValueChanged += OnSliderPositionChanged;
            }
        }

        /// <summary>
        /// Sets the playback position text.
        /// </summary>
        /// <param name="value">New text for the playback position.</param>
        protected void SetPlaybackPositionText(string value)
        {
            SetValue(PlaybackPositionTextProperty, value);
        }

        /// <summary>
        /// Sets the playback duration text.
        /// </summary>
        /// <param name="value">The new duration text.</param>
        protected void SetPlaybackDurationText(string value)
        {
            SetValue(PlaybackDurationTextProperty, value);
        }

        /// <summary>
        /// Sets the buffering percentage.
        /// </summary>
        /// <param name="value">New value for the buffering percent.</param>
        protected void SetBufferingPercent(double value)
        {
            SetValue(BufferingPercentProperty, value);
        }

        /// <summary>
        /// Sets the poster image.
        /// </summary>
        /// <param name="value">New poster image.</param>
        protected void SetPosterImageSource(string value)
        {
            SetValue(PosterImageSourceProperty, value);
        }

        /// <summary>
        /// Sets the poster image width.
        /// </summary>
        /// <param name="value">New poster image width.</param>
        protected void SetPosterImageMaxWidth(Double value)
        {
            SetValue(PosterImageMaxWidthProperty, value);
        }

        /// <summary>
        /// Sets the poster image height.
        /// </summary>
        /// <param name="value">New poster image height.</param>
        protected void SetPosterImageMaxHeight(Double value)
        {
            SetValue(PosterImageMaxHeightProperty, value);
        }

        /// <summary>
        /// Sets the visibility of the buffering control.
        /// </summary>
        /// <param name="value">New visibility for the buffering control.</param>
        protected void SetBufferingControlVisibility(Visibility value)
        {
            SetValue(BufferingControlVisibilityProperty, value);
        }

        /// <summary>
        /// Sets the visibility for the poster image.
        /// </summary>
        /// <param name="value">New visibility for the poster image.</param>
        protected void SetPosterControlVisibility(Visibility value)
        {
            SetValue(PosterControlVisibilityProperty, value);
        }

        /// <summary>
        /// Sets the download offset percent.
        /// </summary>
        /// <param name="value">New value for the download offset.</param>
        protected void SetDownloadOffsetPercent(double value)
        {
            SetValue(DownloadOffsetPercentProperty, value);
        }

        /// <summary>
        /// Sets the download percent.
        /// </summary>
        /// <param name="value">New value for the download percent.</param>
        protected void SetDownloadPercent(double value)
        {
            SetValue(DownloadPercentProperty, value);
        }

        /// <summary>
        /// Sets the caption text.
        /// </summary>
        /// <param name="value">New value for the caption text.</param>
        protected void SetCaptionText(string value)
        {
            SetValue(CaptionTextProperty, value);
            if (m_rectCaptionBackground != null)
            {
                if (String.IsNullOrEmpty(value))
                {
                    m_rectCaptionBackground.Height = 0.0;
                }
                else
                {
                    m_rectCaptionBackground.Height = Double.NaN;
                }
            }
        }

        /// <summary>
        /// Sets the visibility of the captions button.
        /// </summary>
        /// <param name="value">New visibility of the captions button.</param>
        protected void SetCaptionsButtonVisibility(Visibility value)
        {
            SetValue(CaptionsButtonVisibilityProperty, value);
        }

        #endregion

        #region Attached Properties
        /// <summary>
        /// Gets the current value of the hide on click property.
        /// </summary>
        /// <param name="obj">Dependency property object.</param>
        /// <returns>Flag indicating the status of the hide on click properry.</returns>
        public static bool GetHideOnClick(DependencyObject obj)
        {
            return (bool)obj.GetValue(HideOnClickProperty);
        }

        /// <summary>
        /// Sets the value of the hide on click property.
        /// </summary>
        /// <param name="obj">Dependency property object.</param>
        /// <param name="value">The new value of the hide on click property.</param>
        public static void SetHideOnClick(DependencyObject obj, bool value)
        {
            obj.SetValue(HideOnClickProperty, value);
        }
        #endregion

        #region Utilties

        /// <summary>
        /// Toggles full screen mode.
        /// </summary>
        public static void ToggleFullScreen()
        {
            Application.Current.Host.Content.IsFullScreen = !(Application.Current.Host.Content.IsFullScreen);
        }

        /// <summary>
        /// Returns a path from a Uri and a file name.
        /// </summary>
        /// <param name="uri">Source Uri.</param>
        /// <param name="fileName">Source file name.</param>
        /// <returns>Destination Uri.</returns>
        public static Uri PathFromUri(Uri uri, String fileName)
        {
            Debug.WriteLine("PathFromUri:" + fileName);
            Uri tmp;
            if ( Uri.TryCreate(fileName,UriKind.Absolute, out tmp))
            {
                Debug.WriteLine("Absolute Path!" + fileName);

                return tmp;
            }

            Debug.WriteLine("Relative Path!" + fileName);

            UriBuilder urib = new UriBuilder(uri);

            urib.Path = System.Uri.UnescapeDataString(uri.AbsolutePath.Substring(0, uri.AbsolutePath.LastIndexOf("/", StringComparison.Ordinal)) + "/" + (string)fileName);

            return urib.Uri;
        }

        /// <summary>
        /// Returns a System.Windows.Media.Color from a color format string.
        /// </summary>
        /// <param name="color">String describing the color.</param>
        /// <returns>The new System.Windows.Media.Color.</returns>
        public static Color ColorFromString(String color)
        {
            UInt32 uiValue = UInt32.Parse(color.Substring(1), System.Globalization.NumberStyles.AllowHexSpecifier | System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            byte a = (byte)((uiValue >> 0x18) & 0xFF);
            byte r = (byte)((uiValue >> 0x10) & 0xFF);
            byte g = (byte)((uiValue >> 0x08) & 0xFF);
            byte b = (byte)((uiValue) & 0xFF);
            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Unescapes an escaped string.
        /// </summary>
        /// <param name="escaped">The escaped string.</param>
        /// <returns>The new unescaped string.</returns>
        public static String UNEscape(String escaped)
        {
            String tmp = System.Uri.UnescapeDataString(escaped);
            tmp = tmp.Replace("%21", "!");
            tmp = tmp.Replace("%26", "&");
            tmp = tmp.Replace("%27", "\'");
            tmp = tmp.Replace("%2C", ",");
            tmp = tmp.Replace("\\\"", "\"");
            tmp = tmp.Replace("\\\\", "\\");
            return tmp;
        }

        #endregion

        #region public methods
        /// <summary>
        /// Plays the current item in the playlist.
        /// </summary>
        public void Play()
        {
            if (!m_inPlayState)
            {
                TogglePlayPause();
            }
        }

        /// <summary>
        /// Pauses the current playlist item.
        /// </summary>
        public void Pause()
        {
            if (m_inPlayState)
            {
                TogglePlayPause();
            }
        }

        /// <summary>
        /// Stops the current playlist item.
        /// </summary>
        public void Stop()
        {
            OnButtonClickStop(null, null);
        }

        /// <summary>
        /// Goes to the next playlist item at the next ui update interval
        /// </summary>
        public void GoToPlaylistItemOnNextTick(int playlistItemIndex)
        {
            if (!m_goToItemOnNextTick) // don't set it if already set
            {
                m_goToItemOnNextTick = true;
                m_goToItemOnNextTickIndex = playlistItemIndex;
            }
        }

        /// <summary>
        /// Goes to the next playlist item.
        /// </summary>
        public void GoToNextPlaylistItem()
        {
            GoToPlaylistItem(m_currentPlaylistIndex + 1);
        }

        /// <summary>
        /// Goes to the next playlist item.
        /// </summary>
        public void GoToToPreviousPlaylistItem()
        {
            GoToPlaylistItem(m_currentPlaylistIndex - 1);
        }

        /// <summary>
        /// Goes to a playlist item.
        /// </summary>
        /// <param name="playlistItemIndex">The index of the playlist item to go to.</param>
        public void GoToPlaylistItem(int playlistItemIndex)
        {
            Debug.WriteLine("GoToPlaylistItem: " + playlistItemIndex.ToString(CultureInfo.CurrentCulture));
            if (playlistItemIndex >= 0 && playlistItemIndex < Playlist.Count)
            {
                m_currentChapterIndex = 0;

                bool canSkipReset = m_currentPlaylistIndex == playlistItemIndex;
                if (canSkipReset)
                {
                    switch (m_mediaElement.CurrentState)
                    {
                        case MediaElementState.Closed:
                        case MediaElementState.Stopped:
                            canSkipReset = false;
                            break;
                        default:
                            break;
                    }
                }

                if (!canSkipReset)
                {
                    m_currentPlaylistIndex = playlistItemIndex;

                    if (m_listBoxChapters != null)
                    {
                        m_listBoxChapters.ItemsSource = Playlist[m_currentPlaylistIndex].Chapters;
                    }

                    // Update window title with playlist item title (or filename)
                    if (HtmlPage.Document != null)
                    {
                        string newTitle = string.Empty;
                        if (!string.IsNullOrEmpty(Playlist[m_currentPlaylistIndex].Title))
                        {
                            newTitle = Playlist[m_currentPlaylistIndex].Title;
                        }
                        else
                        {
                            if (Playlist[m_currentPlaylistIndex].IsAdaptiveStreaming)
                            {
                                newTitle = System.IO.Path.GetDirectoryName(Playlist[m_currentPlaylistIndex].MediaUrl.OriginalString);
                                newTitle = System.IO.Path.GetFileName(newTitle);
                            }
                            else
                            {
                                newTitle = System.IO.Path.GetFileName(Playlist[m_currentPlaylistIndex].MediaUrl.OriginalString);
                            }
                        }

                        HtmlPage.Document.SetProperty("title", newTitle);
                    }

                    // Attach media source to the MediaElement 
                    m_currentItemIsAdaptive = Playlist[m_currentPlaylistIndex].IsAdaptiveStreaming;
                    if (m_currentItemIsAdaptive)
                    {
                        // The old source will get cleaned up when the media element
                        // closes it, so we do not have to explicitly close it here.
                        // Use our factory method to create it
                        if (MediaStreamSourceFactory != null)
                        {
                            m_mediaStreamSource = MediaStreamSourceFactory.Create(m_mediaElement, Playlist[m_currentPlaylistIndex].MediaUrl);
                            m_mediaElement.SetSource(m_mediaStreamSource);
                        }
                    }
                    else
                    {
                        m_mediaElement.Source = Playlist[m_currentPlaylistIndex].MediaUrl;

                        // Set the media stream source to null because we are not using it
                        m_mediaStreamSource = null;
                    }

                    // Update and show the poster frame for the current item
                    DisplayPoster(m_currentPlaylistIndex);
                }

                // Ensure play starts at the beginning of the new item
                SeekToTime(0);

                // Start playing or Pausing the item depending on user settings and current play state.
                if (m_inPlayState || m_autoPlayCache)
                {
                    InternalPlay();
                }
                else if (m_autoLoadCache)
                {
                    InternalPause();
                }
                else
                {
                    // Display the start button when user options niether start nor load the video on page load. 
                    if (m_buttonStart != null)
                    {
                        m_buttonStart.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else if (playlistItemIndex >= Playlist.Count)
            {
                // Reached end -- flag that playback is paused.
                m_inPlayState = false;
            }
        }
        #endregion

        #region TemplateHandlers

        /// <summary>
        /// Overrides base.OnApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UnhookHandlers();

            GetTemplateChildren();

            ConfigureBinding();
            ApplyProperties();
            HookHandlers();
        }

        /// <summary>
        /// Gets the child elements of the template.
        /// </summary>
        public void GetTemplateChildren()
        {
            m_elementStretchBox = GetTemplateChild(StretchBox) as FrameworkElement;
            m_elementVideoWindow = GetTemplateChild(VideoWindow) as FrameworkElement;
            m_mediaElement = GetTemplateChild(MediaElement) as MediaElement;
            m_buttonStart = GetTemplateChild(ButtonStart) as Button;
            m_buttonPlayPause = GetTemplateChild(ButtonPlayPause) as Button;
            m_elementPause = GetTemplateChild(ElementPause) as FrameworkElement;
            m_elementPlay = GetTemplateChild(ElementPlay) as FrameworkElement;
            m_buttonPrevious = GetTemplateChild(ButtonPrevious) as Button;
            m_buttonNext = GetTemplateChild(ButtonNext) as Button;
            m_buttonStop = GetTemplateChild(ButtonStop) as Button;
            m_buttonFullScreen = GetTemplateChild(ButtonFullScreen) as Button;
            m_sliderPosition = GetTemplateChild(SliderPosition) as SensitiveSlider;
            m_buttonMute = GetTemplateChild(ButtonMute) as ToggleButton;
            m_buttonClosedCaptions = GetTemplateChild(ButtonClosedCaptions) as ToggleButton;
            m_buttonVolumeDown = GetTemplateChild(ButtonVolumeDown) as RepeatButton;
            m_buttonVolumeUp = GetTemplateChild(ButtonVolumeUp) as RepeatButton;
            m_sliderVolume = GetTemplateChild(SliderVolume) as SensitiveSlider;
            m_buttonPlaylist = GetTemplateChild(ButtonPlaylist) as ToggleButton;
            m_buttonChapter = GetTemplateChild(ButtonChapter) as ToggleButton;
            m_listBoxPlaylist = GetTemplateChild(ListBoxPlaylist) as ListBox;
            m_listBoxChapters = GetTemplateChild(ListBoxChapters) as ListBox;
            m_textBlockErrorMessage = GetTemplateChild(TextblockErrorMessage) as TextBlock;
            m_rectCaptionBackground = GetTemplateChild(ClosedCaptionBackground) as Rectangle;
            m_buttonStepForwards = GetTemplateChild(ButtonStepForwards) as Button;
            m_buttonStepBackwards = GetTemplateChild(ButtonStepBackwards) as Button;
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Handler for the OnStartup event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        public virtual void OnStartup(object sender, StartupEventArgs e)
        {
            String strInitValue;
            if (e.InitParams.TryGetValue("playlist", out strInitValue))
            {
                try
                {
                    Playlist.Clear();
                    Playlist.ParseXml(HtmlPage.Document.DocumentUri, strInitValue);
                }
                catch (System.Xml.XmlException xe)
                {
                    Debug.WriteLine("XML Parsing Error:" + xe.ToString());
                }
                catch (NullReferenceException)
                {
                }

                if (e.InitParams.TryGetValue("autoplay", out strInitValue))
                {
                    try
                    {
                        m_autoPlayCache = Convert.ToBoolean(strInitValue, CultureInfo.CurrentCulture);
                    }
                    catch (System.FormatException)
                    {
                        m_autoPlayCache = true;
                    }
                }
                else
                {
                    m_autoPlayCache = true; // fake/preview mode
                }

                if (e.InitParams.TryGetValue("autoload", out strInitValue))
                {
                    try
                    {
                        m_autoLoadCache = Convert.ToBoolean(strInitValue, CultureInfo.CurrentCulture);
                    }
                    catch (System.FormatException)
                    {
                        m_autoLoadCache = true;
                    }
                }
                else
                {
                    m_autoLoadCache = true; // fake/preview mode
                }

                if (e.InitParams.TryGetValue("muted", out strInitValue))
                {
                    try
                    {
                        m_mutedCache = Convert.ToBoolean(strInitValue, CultureInfo.CurrentCulture);
                    }
                    catch (System.FormatException)
                    {
                        m_mutedCache = false;
                    }
                }
                else
                {
                    m_mutedCache = true; // fake/preview mode
                }

                if (e.InitParams.TryGetValue("stretchmode", out strInitValue))
                {
                    int stretchMode = 0;
                    try
                    {
                        stretchMode = Convert.ToInt32(strInitValue, CultureInfo.CurrentCulture);
                    }
                    catch (System.FormatException)
                    {
                        stretchMode = 0;
                    }

                    switch (stretchMode)
                    {
                        default:
                        case 0:
                            m_stretchMode = Stretch.None;
                            break;
                        case 1:
                            m_stretchMode = Stretch.Uniform;
                            break;
                        case 2:
                            m_stretchMode = Stretch.UniformToFill;
                            break;
                        case 3:
                            m_stretchMode = Stretch.Fill;
                            break;
                    }
                }

                if (e.InitParams.TryGetValue("background", out strInitValue))
                {
                    try
                    {
                        UserBackgroundColor = new SolidColorBrush(ColorFromString(strInitValue));
                    }
                    catch (System.FormatException)
                    {
                        UserBackgroundColor = new SolidColorBrush(ColorFromString("#FF0000FF"));
                    }
                }

                if (e.InitParams.TryGetValue("displaytimecode", out strInitValue))
                {
                    try
                    {
                        DisplayTimeCode = bool.Parse(strInitValue);
                    }
                    catch (System.FormatException)
                    {
                        DisplayTimeCode = false;   
                    }
                }

                if (e.InitParams.TryGetValue("enablecaptions", out strInitValue))
                {
                    try
                    {
                        SetCaptionsButtonVisibility(bool.Parse(strInitValue) ? Visibility.Visible : Visibility.Collapsed);
                    }
                    catch (System.FormatException)
                    {
                        SetCaptionsButtonVisibility(Visibility.Visible);
                    }

                    CaptionsVisibility = CaptionsButtonVisibility;
                }
            }
        }

        /// <summary>
        /// Event handler for the playlist clicked event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnButtonClickPlaylist(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = sender as ToggleButton;
            VisualStateManager.GoToState(this, (btn.IsChecked == true) ? "showPlaylist" : "hidePlaylist", true);
        }

        /// <summary>
        /// Event handler for the chapter button event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnButtonClickChapter(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = sender as ToggleButton;
            VisualStateManager.GoToState(this, (btn.IsChecked == true) ? "showChapters" : "hideChapters", true);
        }

        /// <summary>
        /// Update the UI to show the buffering controls if needed.
        /// </summary>
        /// 
        void UpdateBufferingControls()
        {
            // control the visiblity of the buffering control
            if ((m_currentPlaylistIndex >= 0)
            && (m_currentPlaylistIndex < Playlist.Count)
            && (!Playlist[m_currentPlaylistIndex].IsAdaptiveStreaming)
            && (MediaElementState.Buffering == m_mediaElement.CurrentState))
            {
                if (BufferingControlVisibility != Visibility.Visible)
                {
                    SetBufferingControlVisibility(Visibility.Visible);
                    VisualStateManager.GoToState(this, "showBuffering", true);
                }

                SetBufferingPercent(m_mediaElement.BufferingProgress * ProgressConst.Progress2Percent);
            }
            else
            {
                if (BufferingControlVisibility != Visibility.Collapsed)
                {
                    SetBufferingControlVisibility(Visibility.Collapsed);
                    VisualStateManager.GoToState(this, "hideBuffering", true);
                }
            }
        }

        /// <summary>
        /// Event handler for a Timer tick.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnTimerTick(object sender, EventArgs e)
        {
            Debug.WriteLine("Enter OnTimerTick");
            if (m_mediaElement == null)
            {
                Debug.WriteLine("Bail OnTimerTick");
                return;
            }

            if (m_goToItemOnNextTick)
            {
                Debug.WriteLine("OnTimerTick:GoToPlaylistItem");
                m_goToItemOnNextTick = false;
                GoToPlaylistItem(m_goToItemOnNextTickIndex);
            }

            if (m_seekOnNextTick)
            {
                Debug.WriteLine("OnTimerTick:DoActualSeek");
                DoActualSeek();
            }

            // Don't update position based on media element while the user is dragging the slider
            if (m_sliderPosition == null || !m_sliderPosition.IsDragging)
            {
                // while playing or paused -- get it from the media element
                switch (m_mediaElement.CurrentState)
                {
                    case MediaElementState.Playing:
                    case MediaElementState.Paused:
                        Double position = m_mediaElement.Position.TotalSeconds;
                        Debug.WriteLine("OnTimerTick:Updating position:" + position.ToString(CultureInfo.CurrentCulture));
                        SetPlaybackPosition(position);
                        break;
                    default:
                        break;
                }
            }

            // If chapter list is visible -- Update selected chapter
            // Set Selection onto matching chapterlist item
            if ((m_listBoxChapters != null)
            && (m_listBoxChapters.Visibility == Visibility.Visible)
            && (m_listBoxChapters.SelectedIndex != m_currentChapterIndex)
            && (m_currentChapterIndex >= 0)
            && (m_currentChapterIndex < m_listBoxChapters.Items.Count))
            {
                m_listBoxChapters.SelectionChanged -= OnListBoxSelectionChangedChapters; // avoid clicked-on selection change logic
                m_listBoxChapters.SelectedIndex = m_currentChapterIndex;
                m_listBoxChapters.SelectionChanged += OnListBoxSelectionChangedChapters;

                // Scroll current chapter into view
                Object objCurrentChapterItem = m_listBoxChapters.Items[m_currentChapterIndex];
                if (objCurrentChapterItem != null)
                {
                    try
                    {
                        Debug.WriteLine("OnTimerTick:m_listBoxChapters.ScrollIntoView:" + objCurrentChapterItem.ToString());
                        m_listBoxChapters.ScrollIntoView(objCurrentChapterItem);
                    }
                    catch (NullReferenceException nre)
                    {
                        Debug.WriteLine(nre.ToString());
                        Debug.WriteLine(nre.StackTrace.ToString());
                    }
                }
            }

            // If the Playlist is visible -- Update selected item
            // Set Selection onto matching Playlist item
            if ((m_listBoxPlaylist != null)
            && (m_listBoxPlaylist.Visibility == Visibility.Visible)
            && (m_listBoxPlaylist.SelectedIndex != m_currentPlaylistIndex)
            && (m_currentPlaylistIndex >= 0)
            && (m_currentPlaylistIndex < Playlist.Count)
            && (m_currentPlaylistIndex < m_listBoxPlaylist.Items.Count))
            {
                m_listBoxPlaylist.SelectionChanged -= OnListBoxSelectionChangedPlaylist; // avoid clicked-on selection change logic
                m_listBoxPlaylist.SelectedIndex = m_currentPlaylistIndex;
                m_listBoxPlaylist.SelectionChanged += OnListBoxSelectionChangedPlaylist;

                // Scroll current playlist item into view
                Object objCurrentPlaylistItem = m_listBoxPlaylist.Items[m_currentPlaylistIndex];
                if (objCurrentPlaylistItem != null)
                {
                    try
                    {
                        Debug.WriteLine("OnTimerTick:m_listBoxPlaylist.ScrollIntoView:" + objCurrentPlaylistItem.ToString());
                        m_listBoxPlaylist.ScrollIntoView(objCurrentPlaylistItem);
                    }
                    catch (NullReferenceException nre)
                    {
                        Debug.WriteLine(nre.ToString());
                        Debug.WriteLine(nre.StackTrace.ToString());
                    }
                }
            }

            if (m_downloadProgressNeedsUpdating)
            {
                m_downloadProgressNeedsUpdating = false;
                Double downloadProgress = m_mediaElement.DownloadProgress;
                Debug.WriteLine("OnTimerTick:DownloadProgress:" + downloadProgress.ToString(CultureInfo.CurrentCulture));
                SetDownloadPercent(downloadProgress * ProgressConst.Progress2Percent);
                SetDownloadOffsetPercent(m_mediaElement.DownloadProgressOffset * ProgressConst.Progress2Percent);
            }

            UpdateBufferingControls();

            // Restore play state after dragging slider position.
            if (m_sliderPosition != null)
            {
                if (!m_sliderPosition.IsDragging && m_inPlayStateBeforeSliderPositionDrag)
                {
                    Debug.WriteLine("**** Resuming playback");
                    m_inPlayStateBeforeSliderPositionDrag = false;
                    InternalPlay();
                }
            }

            Debug.WriteLine("Exit OnTimerTick");
        }

        /// <summary>
        /// Event handler for the control fade out event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnTimerControlFadeOutTick(object sender, EventArgs e)
        {
            if(currentControlState != desiredControlState)
            {
                GoToControlState(desiredControlState);
            }

            SetDesiredControlState();
        }

        /// <summary>
        /// Event handler for the slider position changed event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnSliderPositionChanged(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnSliderPositionChanged:" + m_sliderPosition.Value.ToString(CultureInfo.CurrentCulture));
            SeekToTime(m_sliderPosition.Value);
        }

        /// <summary>
        /// Handle DragStart event.
        /// </summary>
        /// <param name="sender">Source object, Thumb.</param>
        /// <param name="e">Drag start args.</param>
        private void OnSliderPositionDragStarted(object sender, DragStartedEventArgs e)
        {
            Debug.WriteLine("OnSliderPositionDragStarted m_inPlayState=" + m_inPlayState.ToString(CultureInfo.CurrentCulture));
            if (m_inPlayState)
            {
                m_inPlayStateBeforeSliderPositionDrag = true;
                InternalPause();
            }
        }

        /// <summary>
        /// Handle DragCompleted event.
        /// </summary>.
        /// <param name="sender">Source object, Thumb.</param>
        /// <param name="e">Drag completed args.</param>
        private void OnSliderPositionDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Debug.WriteLine("OnSliderPositionDragCompleted m_inPlayStateBeforeSliderPositionDrag=" + m_inPlayStateBeforeSliderPositionDrag.ToString(CultureInfo.CurrentCulture));
            Debug.WriteLine("new pos:" + m_sliderPosition.Value.ToString(CultureInfo.CurrentCulture));
            SeekToTime(m_sliderPosition.Value);
            DoActualSeek();
        }

        /// <summary>
        /// Event handler for the media element state changed event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnMediaElementCurrentStateChanged(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMediaElementCurrentStateChanged:" + e.ToString() + " NewState:" + m_mediaElement.CurrentState.ToString());

            MediaElementState currentState = m_mediaElement.CurrentState; 
            switch (currentState)
            {
                case MediaElementState.Playing:
                case MediaElementState.Opening:
                case MediaElementState.Buffering:
                case MediaElementState.AcquiringLicense:
                    {
                        m_elementPlay.Visibility = Visibility.Collapsed;
                        m_elementPause.Visibility = Visibility.Visible;
                        break;
                    }

                case MediaElementState.Paused:
                case MediaElementState.Stopped:
                case MediaElementState.Closed:
                    {
                        m_elementPlay.Visibility = Visibility.Visible;
                        m_elementPause.Visibility = Visibility.Collapsed;
                        break;
                    }

                default:
                    break;
            }

            UpdateBufferingControls();

            if (StateChanged != null)
            {
                StateChanged(sender, e);
            }
        }

        /// <summary>
        /// Event handler for the media opened event from the media element.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnMediaElementMediaOpened(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMediaElementMediaOpened:" + e.ToString());

            ShowErrorMessage(null);

            DisplayPoster(-1);
            PerformResize();

            SetPlaybackDuration(m_mediaElement.NaturalDuration.TimeSpan.TotalSeconds);

            UpdateDurationDisplay();

            if (m_inPlayState)
            {
                InternalPlay();
            }

            UpdateCanStep();
        }

        /// <summary>
        /// Event handler for the media ended event from the media element.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnMediaElementMediaEnded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMediaElementMediaEnded:" + e.ToString());
            GoToPlaylistItemOnNextTick(m_currentPlaylistIndex + 1);
        }

        /// <summary>
        /// Event handler for the media failed event from the media element.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnMediaElementMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine("OnMediaElementMediaFailed:" + e.ToString());

            string strErrorMessage = string.Empty;
            if (m_mediaElement.Source != null)
            {
                strErrorMessage = m_mediaElement.Source.ToString();
            }
            else if (m_currentPlaylistIndex >= 0 && m_currentPlaylistIndex < Playlist.Count)
            {
                strErrorMessage = Playlist[m_currentPlaylistIndex].MediaUrl.ToString();
            }

            strErrorMessage += "\r\n" + e.ErrorException.ToString();

            if (m_currentPlaylistIndex >= 0 && m_currentPlaylistIndex < Playlist.Count && Playlist[m_currentPlaylistIndex].IsAdaptiveStreaming)
            {
                strErrorMessage += "\r\nRequires output to be hosted on a web server running IIS 7.0 with the Smooth Streaming handler installed and a Silverlight 2 template that supports Smooth Streaming.";                
            }

            ShowErrorMessage(strErrorMessage);
        }

        /// <summary>
        /// Event handler for the mouse down event from the media element.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnMediaElementMouseDown(object sender, RoutedEventArgs e)
        {
            if ((DateTime.Now - m_lastMediaElementClick).TotalMilliseconds < 300)
            {
                TogglePlayPause();
                ToggleFullScreen();
            }
            else
            {
                TogglePlayPause();
            }

            m_lastMediaElementClick = DateTime.Now;
        }

        /// <summary>
        /// Event handler for the mouse moved event from the stretch box.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnStretchBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (Application.Current.Host.Content.IsFullScreen)
            {
                GoToControlState("exitFullScreen");
            }
        }

        /// <summary>
        /// Event handler for the download progress event from the media element.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnMediaElementDownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMediaElementDownloadProgressChanged:" + e.ToString());

            m_downloadProgressNeedsUpdating = true;
        }

        /// <summary>
        /// Click handler for the start button.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnButtonClickStart(object sender, RoutedEventArgs e)
        {
            GoToPlaylistItem(0);
            InternalPlay();
        }

        /// <summary>
        /// Click handler for the play and pause button.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnButtonClickPlayPause(object sender, RoutedEventArgs e)
        {
            // If the big "play" button is shown -- restart playback from the 1st item in the playlist 
            if (m_buttonStart != null && m_buttonStart.Visibility == Visibility.Visible)
            {
                OnButtonClickStart(sender, e);
                return;
            }

            TogglePlayPause();
        }

        /// <summary>
        /// Click handler for the stop button.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnButtonClickStop(object sender, RoutedEventArgs e)
        {
            m_inPlayState = false;
            m_currentPlaylistIndex = 0;
            DisplayPoster(m_currentPlaylistIndex);
            if (m_mediaElement != null)
            {
                m_mediaElement.Stop();
                m_mediaElement.AutoPlay = false;
                m_mediaElement.Source = null;
            }

            if (m_buttonStart != null)
            {
                m_buttonStart.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Click handler for the previous button.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnButtonClickPrevious(object sender, RoutedEventArgs e)
        {
            SeekToPreviousItem();
        }

        /// <summary>
        /// Click handler for the next button.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnButtonClickNext(object sender, RoutedEventArgs e)
        {
            SeekToNextItem();
        }

        /// <summary>
        /// Increments the volume by the given amount.
        /// </summary>
        /// <param name="dblVolumeIncrement">Amount to increment the volume.</param>
        void VolumeIncrement(double dblVolumeIncrement)
        {
            if (UNMute())
            {
                double dblVolume = m_mediaElement.Volume;
                dblVolume = Math.Min(1.0, Math.Max(0.0, dblVolume + dblVolumeIncrement));
                m_mediaElement.Volume = dblVolume;
                m_sliderVolume.Value = dblVolume;
                m_dblUnMutedVolume = dblVolume;
            }
        }

        /// <summary>
        /// Click handler for the volume down button.
        /// </summary>
        /// <param name="sender">Source of this event.</param>
        /// <param name="e">Event args.</param>
        void OnButtonClickVolumeDown(object sender, RoutedEventArgs e)
        {
            VolumeIncrement(-m_sliderVolume.SmallChange);
        }

        /// <summary>
        /// Click handler for the volume up button.
        /// </summary>
        /// <param name="sender">Source of this event.</param>
        /// <param name="e">Event args.</param>
        void OnButtonClickVolumeUp(object sender, RoutedEventArgs e)
        {
            VolumeIncrement(m_sliderVolume.SmallChange);
        }

        /// <summary>
        /// Event handler for the volume changed event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnSliderVolumeChanged(object sender, RoutedEventArgs e)
        {
            if (UNMute())
            {
                m_mediaElement.Volume = m_sliderVolume.Value;
            }
        }

        /// <summary>
        /// Unmute the volume.
        /// </summary>
        /// <returns>Returns true.</returns>
        bool UNMute()
        {
            if ((m_buttonMute.IsChecked == null) || (true == m_buttonMute.IsChecked))
            {
                UNCacheVolumeLevel();
                m_buttonMute.IsChecked = false;
            }

            return true;
        }

        /// <summary>
        /// Caches the current volume level.
        /// </summary>
        void CacheVolumeLevel()
        {
            m_mutedCache = true;
            m_dblUnMutedVolume = m_mediaElement.Volume;
            m_mediaElement.Volume = 0.0;
            m_sliderVolume.ValueChanged -= OnSliderVolumeChanged;
            m_sliderVolume.Value = 0.0;
            m_sliderVolume.ValueChanged += OnSliderVolumeChanged;
        }

        /// <summary>
        /// Uncaches the current volume level.
        /// </summary>
        void UNCacheVolumeLevel()
        {
            m_mutedCache = false;
            if (m_dblUnMutedVolume < VolumeMuteThreshold)
            {
                m_dblUnMutedVolume = VolumeDefault;
            }

            m_mediaElement.Volume = m_dblUnMutedVolume;
            m_sliderVolume.ValueChanged -= OnSliderVolumeChanged;
            m_sliderVolume.Value = m_dblUnMutedVolume;
            m_sliderVolume.ValueChanged += OnSliderVolumeChanged;
        }

        /// <summary>
        /// Click handler for the Mute button.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        void OnButtonClickMute(object sender, RoutedEventArgs e)
        {
            if (m_mutedCache || m_mediaElement.Volume < VolumeMuteThreshold)
            {
                UNCacheVolumeLevel();
            }
            else
            {
                CacheVolumeLevel();
            }
        }

        /// <summary>
        /// Click handler for the closed captions button.
        /// </summary>
        /// <param name="sender">Source of this event.</param>
        /// <param name="e">Event args.</param>
        void OnButtonClickClosedCaptions(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = (ToggleButton) sender;            
            CaptionsVisibility = (toggleButton.IsChecked==true)?Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Event handler for the step forwards event.
        /// </summary>
        /// <param name="sender">Source of this event.</param>
        /// <param name="e">Event args.</param>
        void OnButtonStepForwards(object sender, RoutedEventArgs e)
        {
            if((m_currentPlaylistIndex >= 0)
            && (m_currentPlaylistIndex < Playlist.Count)
            && (Playlist[m_currentPlaylistIndex] != null) 
            && (Playlist[m_currentPlaylistIndex].FrameRate != SmpteFrameRate.Unknown)
            && (m_mediaElement.Position.TotalSeconds < m_mediaElement.NaturalDuration.TimeSpan.TotalSeconds))
            {
                TimeCode oneFrame = new TimeCode("00:00:00:01", Playlist[m_currentPlaylistIndex].FrameRate);

                TimeCode current = TimeCode.FromAbsoluteTime((Double)m_mediaElement.Position.TotalSeconds, Playlist[m_currentPlaylistIndex].FrameRate);

                TimeCode newPosition = current.Add(oneFrame);

                SeekToTime(newPosition.TotalSeconds);
            }
        }

        /// <summary>
        /// Event handler for the step backwards button.
        /// </summary>
        /// <param name="sender">Source of this event.</param>
        /// <param name="e">Event args.</param>
        void OnButtonStepBackwards(object sender, RoutedEventArgs e)
        {
            if ((m_currentPlaylistIndex >= 0)
            && (m_currentPlaylistIndex < Playlist.Count)
            && (Playlist[m_currentPlaylistIndex] != null)
            && (Playlist[m_currentPlaylistIndex].FrameRate != SmpteFrameRate.Unknown)
            && (m_mediaElement.Position.TotalSeconds > 0))
            {
                TimeCode oneFrame = new TimeCode("00:00:00:01", Playlist[m_currentPlaylistIndex].FrameRate);

                TimeCode current = TimeCode.FromAbsoluteTime((Double)m_mediaElement.Position.TotalSeconds, Playlist[m_currentPlaylistIndex].FrameRate);

                TimeCode newPosition = current.Subtract(oneFrame);

                SeekToTime(newPosition.TotalSeconds);
            }
        }

        /// <summary>
        /// Event handler for the resized event.
        /// </summary>
        /// <param name="sender">Source of this event.</param>
        /// <param name="e">Event args.</param>
        void OnResized(object sender, EventArgs e)
        {
            PerformResize();
        }

        /// <summary>
        /// Event handler for the full screen changed event.
        /// </summary>
        /// <param name="sender">Source of this event.</param>
        /// <param name="e">Event args.</param>
        void OnFullScreenChanged(object sender, EventArgs e)
        {
            PerformResize();
            if (Application.Current.Host.Content.IsFullScreen)
            {
                m_mediaElement.Stretch = Stretch.Uniform;
            }
            else
            {
                m_mediaElement.Stretch = m_stretchMode;
            }
        }

        /// <summary>
        /// Click handler for the full screen button.
        /// </summary>
        /// <param name="sender">Source of this event.</param>
        /// <param name="e">Event args.</param>
        void OnButtonClickFullScreen(object sender, RoutedEventArgs e)
        {
            ToggleFullScreen();
        }

        /// <summary>
        /// Event handler for changing the playlist item.
        /// </summary>
        /// <param name="sender">Source of this event.</param>
        /// <param name="e">Event args.</param>
        void OnListBoxSelectionChangedPlaylist(object sender, RoutedEventArgs e)
        {
            if (m_listBoxPlaylist != null && GetHideOnClick(m_listBoxPlaylist))
            {
                if (m_buttonPlaylist != null)
                {
                    m_buttonPlaylist.IsChecked = false;
                }

                VisualStateManager.GoToState(this, "hidePlaylist", true);
            }

            GoToPlaylistItem(m_listBoxPlaylist.SelectedIndex);
        }

        /// <summary>
        /// Event handler for the chapters item changed event.
        /// </summary>
        /// <param name="sender">Source of this event.</param>
        /// <param name="e">Event args.</param>
        void OnListBoxSelectionChangedChapters(object sender, RoutedEventArgs e)
        {
            if (m_listBoxChapters != null && GetHideOnClick(m_listBoxChapters))
            {
                if (m_buttonChapter != null)
                {
                    m_buttonChapter.IsChecked = false;
                }

                VisualStateManager.GoToState(this, "hideChapters", true);
            }

            m_currentChapterIndex = m_listBoxChapters.SelectedIndex;
            if ((m_currentPlaylistIndex >= 0)
            && (m_currentPlaylistIndex < Playlist.Count)
            && (m_currentChapterIndex >= 0) 
            && (m_currentChapterIndex < Playlist[m_currentPlaylistIndex].Chapters.Count))
            {
                SeekToTime(Playlist[m_currentPlaylistIndex].Chapters[m_currentChapterIndex].Position);
            }
        }

        /// <summary>
        /// Event handler for the marker reached event from the media element.
        /// </summary>
        /// <param name="sender">Source of this event.</param>
        /// <param name="e">Event args.</param>
        void OnMediaElementMarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            Debug.WriteLine("OnMediaElementMarkerReached:" + TimeSpanStringConverter.ConvertToString(e.Marker.Time, ConverterModes.TenthSecond) + "->" + e.Marker.Text + "<-");
            Debug.WriteLine("MarkerType:" + e.Marker.Type.ToString());

            //Test if this a "Marker" vs a "Caption"
            if ((m_listBoxChapters != null) && e.Marker.Type.Equals(MarkerType))
            {
                // compute current chapter index from playback position
                m_currentChapterIndex = ChapterIndexFromPosition(e.Marker.Time);
            }

            // Display marker or caption text in the caption area
            String type = e.Marker.Type.ToUpper(CultureInfo.InvariantCulture);
            if (type.Equals(CaptionType))
            {
                SetCaptionText(e.Marker.Text);
            }

            if (MarkerReached != null)
            {
                MarkerReached(sender, e);
            }
        }

        /// <summary>
        /// Helper routine for playing the current item.
        /// </summary>
        void InternalPlay()
        {
            if (m_buttonStart != null)
            {
                m_buttonStart.Visibility = Visibility.Collapsed;
            }

            if (m_mediaElement != null)
            {
                m_mediaElement.Play();
                m_inPlayState = true;
            }
        }

        /// <summary>
        /// Helper routine for pausing the current item.
        /// </summary>
        void InternalPause()
        {
            if (m_buttonStart != null)
            {
                m_buttonStart.Visibility = Visibility.Collapsed;
            }

            if (m_mediaElement != null)
            {
                m_mediaElement.Pause();
            }

            m_inPlayState = false;
        }

        #endregion

        #region ProtectedUtilities

        /// <summary>
        /// Updates the position display.
        /// </summary>
        protected void UpdatePositionDisplay()
        {
            if (DisplayTimeCode)
            {
                if ((m_currentPlaylistIndex >= 0)
                && (m_currentPlaylistIndex < Playlist.Count)
                && (Playlist[m_currentPlaylistIndex].FrameRate != SmpteFrameRate.Unknown))
                {
                    TimeCode tc = TimeCode.FromAbsoluteTime(PlaybackPosition, Playlist[m_currentPlaylistIndex].FrameRate);
                    SetPlaybackPositionText(tc.ToString());
                    return;
                }
            }

            SetPlaybackPositionText(TimeSpanStringConverter.ConvertToString(TimeSpan.FromSeconds(PlaybackPosition), ConverterModes.NoMilliseconds));
        }

        /// <summary>
        /// Updates the duration display.
        /// </summary>
        protected void UpdateDurationDisplay()
        {
            if (DisplayTimeCode)
            {
                if ((m_currentPlaylistIndex >= 0)
                && (m_currentPlaylistIndex < Playlist.Count)
                && (Playlist[m_currentPlaylistIndex].FrameRate != SmpteFrameRate.Unknown))
                {
                    TimeCode tc = TimeCode.FromAbsoluteTime(PlaybackDuration, Playlist[m_currentPlaylistIndex].FrameRate);
                    SetPlaybackDurationText(tc.ToString());
                    return;
                }
            }

            SetPlaybackDurationText(TimeSpanStringConverter.ConvertToString(m_mediaElement.NaturalDuration.TimeSpan, ConverterModes.NoMilliseconds));
        }

        /// <summary>
        /// Shows an error message.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        protected void ShowErrorMessage(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                if (m_textBlockErrorMessage != null)
                {
                     m_textBlockErrorMessage.Visibility = Visibility.Collapsed;
                }

                return;
            }

            if (m_textBlockErrorMessage != null)
            {
                m_textBlockErrorMessage.Text = message;
                m_textBlockErrorMessage.Visibility = Visibility.Visible;
            }
            else
            {
                HtmlPage.Window.Alert(message);
            }
        }

        /// <summary>
        /// Creates a new position timer.
        /// </summary>
        /// <param name="interval">Interval of the timer.</param>
        protected void CreatePositionTimer(TimeSpan interval)
        {
            if (m_timer == null)
            {
                m_timer = new DispatcherTimer();
                m_timer.Interval = interval; // 6 NTSC frames
                m_timer.Tick += new EventHandler(OnTimerTick);
            }
        }

        /// <summary>
        /// Hooks our event handlers.
        /// </summary>
        protected virtual void HookHandlers()
        {
            CreatePositionTimer(new TimeSpan(0, 0, 0, 0, (6 * 1001 / 30)));

            m_timer.Start();

            if (m_timerControlFadeOut == null)
            {
                m_timerControlFadeOut = new DispatcherTimer();
                m_timerControlFadeOut.Interval = new TimeSpan(0, 0, 0, 2, 0); // 2 seconds
                m_timerControlFadeOut.Tick += new EventHandler(OnTimerControlFadeOutTick);
            }

            m_timerControlFadeOut.Start();

            if (Application.Current != null)
            {
                Application.Current.Host.Content.FullScreenChanged += OnFullScreenChanged;
                Application.Current.Host.Content.Resized += OnResized;
            }

            if (m_mediaElement != null)
            {
                m_mediaElement.MediaFailed += OnMediaElementMediaFailed;
                m_mediaElement.MediaOpened += OnMediaElementMediaOpened;
                m_mediaElement.MediaEnded += OnMediaElementMediaEnded;
                m_mediaElement.CurrentStateChanged += OnMediaElementCurrentStateChanged;
                m_mediaElement.MarkerReached += OnMediaElementMarkerReached;
                m_mediaElement.DownloadProgressChanged += OnMediaElementDownloadProgressChanged;
                m_mediaElement.MouseLeftButtonDown += OnMediaElementMouseDown;
            }

            if (m_elementStretchBox != null)
            {
                m_elementStretchBox.MouseMove += OnStretchBoxMouseMove;
            }

            if (m_buttonStart != null)
            {
                m_buttonStart.Click += OnButtonClickStart;
            }

            if (m_buttonPlayPause != null)
            {
                m_buttonPlayPause.Click += OnButtonClickPlayPause;
            }

            if (m_buttonStop != null)
            {
                m_buttonStop.Click += OnButtonClickStop;
            }

            if (m_buttonPrevious != null)
            {
                m_buttonPrevious.Click += OnButtonClickPrevious;
            }

            if (m_buttonNext != null)
            {
                m_buttonNext.Click += OnButtonClickNext;
            }

            if (m_buttonMute != null)
            {
                m_buttonMute.Click += OnButtonClickMute;
            }

            if (m_buttonStepBackwards != null)
            {
                m_buttonStepBackwards.Click += OnButtonStepBackwards;
            }

            if (m_buttonStepForwards != null)
            {
                m_buttonStepForwards.Click += OnButtonStepForwards;
            }

	        if (m_buttonClosedCaptions != null)
            {
                m_buttonClosedCaptions.Click += OnButtonClickClosedCaptions;
            }

            if (m_buttonVolumeDown != null)
            {
                m_buttonVolumeDown.Click += OnButtonClickVolumeDown;
            }

            if (m_buttonVolumeUp != null)
            {
                m_buttonVolumeUp.Click += OnButtonClickVolumeUp;
            }

            if (m_buttonFullScreen != null)
            {
                m_buttonFullScreen.Click += OnButtonClickFullScreen;
            }

            if (m_buttonPlaylist != null)
            {
                m_buttonPlaylist.Click += OnButtonClickPlaylist;
            }

            if (m_listBoxPlaylist != null)
            {
                m_listBoxPlaylist.SelectionChanged += OnListBoxSelectionChangedPlaylist;
            }

            if (m_listBoxChapters != null)
            {
                m_listBoxChapters.SelectionChanged += OnListBoxSelectionChangedChapters;
            }

            if (m_buttonChapter != null)
            {
                m_buttonChapter.Click += OnButtonClickChapter;
            }

            if (m_sliderPosition != null)
            {
                m_sliderPosition.ValueChanged += OnSliderPositionChanged;
                m_sliderPosition.DragStarted += OnSliderPositionDragStarted;
                m_sliderPosition.DragCompleted += OnSliderPositionDragCompleted;
            }

            if (m_sliderVolume != null && m_mediaElement != null)
            {
                m_sliderVolume.ValueChanged += OnSliderVolumeChanged;
            }
        }

        /// <summary>
        /// Unhooks our event handlers.
        /// </summary>
        protected virtual void UnhookHandlers()
        {
            if (m_mediaElement != null)
            {
                m_mediaElement.MediaFailed -= OnMediaElementMediaFailed;
                m_mediaElement.MediaOpened -= OnMediaElementMediaOpened;
                m_mediaElement.MediaEnded -= OnMediaElementMediaEnded;
                m_mediaElement.CurrentStateChanged -= OnMediaElementCurrentStateChanged;
                m_mediaElement.MarkerReached -= OnMediaElementMarkerReached;
                m_mediaElement.DownloadProgressChanged -= OnMediaElementDownloadProgressChanged;
                m_mediaElement.MouseLeftButtonDown -= OnMediaElementMouseDown;
            }

            if (m_elementStretchBox != null)
            {
                m_elementStretchBox.MouseMove -= OnStretchBoxMouseMove;
            }

            if (m_buttonStart != null)
            {
                m_buttonStart.Click -= OnButtonClickStart;
            }

            if (m_buttonPlayPause != null)
            {
                m_buttonPlayPause.Click -= OnButtonClickPlayPause;
            }

            if (m_buttonStop != null)
            {
                m_buttonStop.Click -= OnButtonClickStop;
            }

            if (m_buttonPrevious != null)
            {
                m_buttonPrevious.Click -= OnButtonClickPrevious;
            }

            if (m_buttonNext != null)
            {
                m_buttonNext.Click -= OnButtonClickNext;
            }

            if (m_buttonMute != null)
            {
                m_buttonMute.Click -= OnButtonClickMute;
            }

            if (m_buttonClosedCaptions != null)
            {
                m_buttonClosedCaptions.Click -= OnButtonClickClosedCaptions;
            }

            if (m_buttonVolumeDown != null)
            {
                m_buttonVolumeDown.Click -= OnButtonClickVolumeDown;
            }

            if (m_buttonVolumeUp != null)
            {
                m_buttonVolumeUp.Click -= OnButtonClickVolumeUp;
            }

            if (m_buttonFullScreen != null)
            {
                m_buttonFullScreen.Click -= OnButtonClickFullScreen;
            }

            if (m_buttonStepBackwards != null)
            {
                m_buttonStepBackwards.Click -= OnButtonStepBackwards;
            }

            if (m_buttonStepForwards != null)
            {
                m_buttonStepForwards.Click -= OnButtonStepForwards;
            }

            if (m_buttonPlaylist != null)
            {
                m_buttonPlaylist.Click -= OnButtonClickPlaylist;
            }

            if (m_listBoxPlaylist != null)
            {
                m_listBoxPlaylist.SelectionChanged -= OnListBoxSelectionChangedPlaylist;
            }

            if (m_listBoxChapters != null)
            {
                m_listBoxChapters.SelectionChanged -= OnListBoxSelectionChangedChapters;
            }

            if (m_buttonChapter != null)
            {
                m_buttonChapter.Click -= OnButtonClickChapter;
            }

            if (m_sliderPosition != null)
            {
                m_sliderPosition.ValueChanged -= OnSliderPositionChanged;
                m_sliderPosition.DragStarted -= OnSliderPositionDragStarted;
                m_sliderPosition.DragCompleted -= OnSliderPositionDragCompleted;
            }

            if (m_sliderVolume != null)
            {
                m_sliderVolume.ValueChanged -= OnSliderVolumeChanged;
            }

            if (m_timer != null)
            {
                m_timer.Stop();
            }
        }

        /// <summary>
        /// Configures the binding for the playlist control.
        /// </summary>
        protected void ConfigureBinding()
        {
            if (m_listBoxPlaylist != null)
            {
                m_listBoxPlaylist.ItemsSource = this.Playlist;
            }
        }

        /// <summary>
        /// Applies our cached properties.
        /// </summary>
        protected void ApplyProperties()
        {
            m_elementPause.Visibility = Visibility.Collapsed;

            if (m_mediaElement != null && (Playlist.Count > 0))
            {
                m_mediaElement.AutoPlay = m_autoPlayCache;               
                m_mediaElement.Stretch = m_stretchMode;
                if (m_autoLoadCache || m_autoPlayCache)
                {
                    GoToPlaylistItem(0);
                }
                else
                {
                    DisplayPoster(0);
                }
            }

            if (m_buttonStart != null)
            {
                if (m_autoPlayCache)
                {
                    m_buttonStart.Visibility = Visibility.Collapsed;
                }
                else
                {
                    m_buttonStart.Visibility = Visibility.Visible;
                }
            }

            if (m_sliderVolume != null && m_mediaElement != null)
            {
                m_sliderVolume.Minimum = 0;
                m_sliderVolume.Maximum = 1;
                m_sliderVolume.SmallChange = 0.1;
                m_sliderVolume.LargeChange = 0.2;
                if (m_mutedCache)
                {
                    m_sliderVolume.Value = 0.0;
                    m_mediaElement.Volume = 0.0;
                }
                else
                {
                    m_sliderVolume.Value = VolumeDefault;
                }

                m_mediaElement.Volume = m_sliderVolume.Value;
            }

            if (m_buttonMute != null)
            {
                m_buttonMute.IsChecked = m_mutedCache;
            }

            PerformResize();
        }

        /// <summary>
        /// Adjust the size of the poster image to match the displayed video .
        /// </summary>
        protected void AdjustPosterSize(int playlistItemIndex)
        {
            if (PosterControlVisibility == Visibility.Visible)
            {
                if ((m_stretchMode == System.Windows.Media.Stretch.None) 
                && (!Application.Current.Host.Content.IsFullScreen)
                && (playlistItemIndex >= 0) 
                && (playlistItemIndex < Playlist.Count)
                && (Playlist[playlistItemIndex].Width > 0)
                && (Playlist[playlistItemIndex].Height > 0))
                {
                    SetPosterImageMaxWidth(Playlist[playlistItemIndex].Width);
                    SetPosterImageMaxHeight(Playlist[playlistItemIndex].Height);
                }
                else
                {
                    SetPosterImageMaxWidth(Double.PositiveInfinity);
                    SetPosterImageMaxHeight(Double.PositiveInfinity);
                }
            }
        }

        /// <summary>
        /// Displays a poster image for a playlist item.
        /// </summary>
        /// <param name="playlistItemIndex">Index of the item to display a poster image for.</param>
        protected void DisplayPoster(int playlistItemIndex)
        {
            if (playlistItemIndex >= 0 && playlistItemIndex < Playlist.Count)
            {                              
                SetPosterImageSource(Playlist[playlistItemIndex].ThumbSource);
                AdjustPosterSize(playlistItemIndex);
                VisualStateManager.GoToState(this, "showPosterFrame", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "hidePosterFrame", true);
            }
        }

        /// <summary>
        /// Seeks to the given time.
        /// </summary>
        /// <param name="seconds">Time to seek to.</param>
        protected void SeekToTime(double seconds)
        {
            // collapse / defer seeks
            Debug.WriteLine("SeekToTime:" + seconds.ToString(CultureInfo.CurrentCulture));
            m_seekOnNextTick = true;
            m_seekOnNextTickPosition = seconds;
        }
       
        /// <summary>
        /// Performs the actual seek.
        /// </summary>
        protected void DoActualSeek()
        {
            Debug.WriteLine("DoActualSeek:" + m_seekOnNextTickPosition.ToString(CultureInfo.CurrentCulture));
            if (!m_mediaElement.CanSeek || !m_seekOnNextTick )
            {
                return;
            }

            // Don't attempt to seek unless the element is actually playing or paused
            switch (m_mediaElement.CurrentState)
            {
                case MediaElementState.Playing:
                case MediaElementState.Paused:
                    break;
                case MediaElementState.Opening:
                case MediaElementState.Buffering:
                case MediaElementState.AcquiringLicense:
                case MediaElementState.Stopped:
                case MediaElementState.Closed:
                default:
                    Debug.WriteLine("Defering while media isn't ready!");
                    return;
            }

            if (m_sliderPosition != null)
            {
                if (m_sliderPosition.IsDragging && m_currentItemIsAdaptive)
                {
                    Debug.WriteLine("Defering Seek during Drag!");
                    return;
                }
            }

            // Finaly go ahead and seek!
            ClearCaptionText();
            m_seekOnNextTick = false;
            double seconds = Math.Min(PlaybackDuration, Math.Max(0.0, m_seekOnNextTickPosition));
            Debug.WriteLine("**** Seeking Media Element seconds=" + seconds.ToString(CultureInfo.CurrentCulture));
            Debug.WriteLine("**** m_seekOnNextTickPosition=" + m_seekOnNextTickPosition.ToString(CultureInfo.CurrentCulture));

            TimeSpan newPosition = TimeSpan.FromSeconds(seconds);
            m_mediaElement.Position = newPosition;

            // update chapter index (listbox selection will be updated in the TimerTick code)
            m_currentChapterIndex = ChapterIndexFromPosition(newPosition);
        }

        /// <summary>
        /// Skips forwards or backwards.
        /// </summary>
        /// <param name="direction">Direction to skip.</param>
        protected void SkipTime(int direction)
        {
            double delta = Math.Max(MinDelta, PlaybackDuration / SkipSteps);
            double skipbuffer = (delta - SkipBuffer);
            double newposition = PlaybackPosition + (delta * direction);
            if (newposition < -(skipbuffer))
            {
                GoToToPreviousPlaylistItem();
            }
            else if (newposition > (PlaybackDuration + skipbuffer))
            {
                GoToNextPlaylistItem();
            }
            else
            {
                SeekToTime(newposition);
            }
        }

        /// <summary>
        /// Seeks to the next playlist item.
        /// </summary>
        protected void SeekToNextItem()
        {
            if (!SeekToChapterPoint(m_currentChapterIndex + 1))
            {
                SkipTime(1);
            }
        }

        /// <summary>
        /// Seeks to the previous playlist item.
        /// </summary>
        protected void SeekToPreviousItem()
        {
            if (!SeekToChapterPoint(m_currentChapterIndex - 1))
            {
                SkipTime(-1);
            }
        }

        /// <summary>
        /// Finds a chapter index from a position.
        /// </summary>
        /// <param name="position">The position we are looking for.</param>
        /// <returns>The index of the chapter item for this position.</returns>
        protected int ChapterIndexFromPosition(TimeSpan position)
        {
            double seconds = position.TotalSeconds;

            int indexChapter = 0;
            if ((m_currentPlaylistIndex >= 0) && (m_currentPlaylistIndex < Playlist.Count))
            {
                while (indexChapter < Playlist[m_currentPlaylistIndex].Chapters.Count && Playlist[m_currentPlaylistIndex].Chapters[indexChapter].Position < seconds)
                {
                    indexChapter++;
                }
            }

            return indexChapter;
        }

        /// <summary>
        /// Seeks to a chapter point.
        /// </summary>
        /// <param name="chapterIndex">The index of the chapter point to seek to.</param>
        /// <returns>true if we found the index, false otherwise.</returns>
        protected bool SeekToChapterPoint(int chapterIndex)
        {
            Debug.WriteLine("SeekToChapterPoint: " + chapterIndex.ToString(CultureInfo.CurrentCulture) + " of playlist item:" + m_currentPlaylistIndex.ToString(CultureInfo.CurrentCulture));
            if ((m_currentPlaylistIndex >= 0) && (m_currentPlaylistIndex < Playlist.Count))
            {
                if ((chapterIndex >= 0) && (chapterIndex < Playlist[m_currentPlaylistIndex].Chapters.Count))
                {
                    m_currentChapterIndex = chapterIndex;
                    m_listBoxChapters.SelectedIndex = m_currentChapterIndex;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Toggles our play/pause state.
        /// </summary>
        protected void TogglePlayPause()
        {
            // Change current play state depending on current state.
            if (m_mediaElement != null)
            {
                switch (m_mediaElement.CurrentState)
                {
                    case MediaElementState.AcquiringLicense:
                    case MediaElementState.Buffering:
                    case MediaElementState.Individualizing:
                    case MediaElementState.Playing:
                    case MediaElementState.Opening:
                        InternalPause();
                        break;
                    case MediaElementState.Paused:
                        //If at end of last playlist item items -- rewind to beginning of 1st item
                        if (m_currentPlaylistIndex >= (Playlist.Count - 1))
                        {
                            Double positionCurrent = m_mediaElement.Position.TotalSeconds;
                            if (positionCurrent >= PlaybackDuration)
                            {
                                m_inPlayState = true;
                                GoToPlaylistItem(0);
                            }
                        }

                        InternalPlay();
                        break;
                    case MediaElementState.Closed:
                    case MediaElementState.Stopped:
                        GoToPlaylistItem(m_currentPlaylistIndex);
                        InternalPlay();
                        break;
                }
            }
        }

        #endregion

        #region PrivateUtilityMethods

        /// <summary>
        /// Goes to our control state.
        /// </summary>
        /// <param name="controlState">Control state to go to.</param>
        private void GoToControlState(String controlState)
        {
            m_timerControlFadeOut.Stop();
            VisualStateManager.GoToState(this, controlState, true);
            currentControlState = controlState;
            m_timerControlFadeOut.Start();
        }

        /// <summary>
        /// Sets the desired control state.
        /// </summary>
        private void SetDesiredControlState()
        {
            if ((Application.Current != null))
            {
                if (Application.Current.Host.Content.IsFullScreen)
                {
                    desiredControlState = "enterFullScreen";
                }
                else
                {
                    desiredControlState = "exitFullScreen";
                }
            }
        }

        /// <summary>
        /// Performs a resize.
        /// </summary>
        private void PerformResize()
        {
            if ((Application.Current != null))
            {
                if (Application.Current.Host.Content.IsFullScreen)
                {
                    this.HorizontalAlignment = HorizontalAlignment.Stretch;
                    this.VerticalAlignment = VerticalAlignment.Stretch;
                }
            }

            SetDesiredControlState();
            AdjustPosterSize(((m_currentPlaylistIndex>=0)?(m_currentPlaylistIndex):(0)));// special case for adjusting poster size between normal and full screen mode prior to playing the 1st item.

            // Apply Uniform scaling -- if needed to avoid clipping the video
            if ((m_stretchMode == Stretch.None)
            && (!Application.Current.Host.Content.IsFullScreen)
            && (m_mediaElement != null)
            && (m_elementVideoWindow != null))
            {
                if ((m_elementVideoWindow.ActualWidth  < PosterImageMaxWidth)
                ||  (m_elementVideoWindow.ActualHeight < PosterImageMaxHeight))
                {
                    m_mediaElement.Stretch = Stretch.Uniform;
                }
                else
                {
                    m_mediaElement.Stretch = m_stretchMode;
                }
            }
        }

        /// <summary>
        /// Clears our caption text.
        /// </summary>
        private void ClearCaptionText()
        {
            SetCaptionText(string.Empty);
        }

        /// <summary>
        /// Updates the can step property.
        /// </summary>
        void UpdateCanStep()
        {
            if ((m_mediaElement != null)
            && (m_mediaElement.CanSeek)
            && (m_currentPlaylistIndex >= 0)
            && (m_currentPlaylistIndex < Playlist.Count)
            && (Playlist[m_currentPlaylistIndex].FrameRate != SmpteFrameRate.Unknown)
            && (!Playlist[m_currentPlaylistIndex].IsAdaptiveStreaming)) //stepping isn't compatible with AdaptiveStreaming MSS currently
            {
                SetValue(CanStepProperty, true);
                return;
            }

            SetValue(CanStepProperty, false);
        }

        #endregion

        /// <summary>
        /// This class contains constants for progress reporting.
        /// </summary>
        private sealed class ProgressConst
        {
            /// <summary>
            /// Maximum progress in the control.
            /// </summary>
            public const double MaxProgress = 1.0;

            /// <summary>
            /// Maximum progress percent.
            /// </summary>
            public const double MaxPercent = 100.0;

            /// <summary>
            /// Converts a progress to a percent.
            /// </summary>
            public const double Progress2Percent = 100.0;

            /// <summary>
            /// Prevents a default instance of the ProgressConst class from being created.
            /// </summary>
            private ProgressConst()
            {
            }
        }
    }
}

