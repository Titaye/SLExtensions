namespace SLMedia.Core
{
    using System;
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

    using SLExtensions.Input;

    public class PositionMarkerCommandBinder
    {
        #region Fields

        // Using a DependencyProperty as the backing store for Item.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemProperty = 
            DependencyProperty.RegisterAttached("Item", typeof(IMediaItem), typeof(PositionMarkerCommandBinder), new PropertyMetadata(ItemChangedCallback));

        // Using a DependencyProperty as the backing store for MarkerSelectorMetadataKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerSelectorMetadataKeyProperty = 
            DependencyProperty.RegisterAttached("MarkerSelectorMetadataKey", typeof(string), typeof(PositionMarkerCommandBinder), new PropertyMetadata(MarkerSelectorMetadataKeyChangedCallback));

        // Using a DependencyProperty as the backing store for MarkerSelectorMetadataValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerSelectorMetadataValueProperty = 
            DependencyProperty.RegisterAttached("MarkerSelectorMetadataValue", typeof(object), typeof(PositionMarkerCommandBinder), new PropertyMetadata(MarkerSelectorMetadataValueChangedCallback));

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty = 
            DependencyProperty.RegisterAttached("Position", typeof(Position), typeof(PositionMarkerCommandBinder), new PropertyMetadata(PositionChangedCallback));

        // Using a DependencyProperty as the backing store for ActiveMarker.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ActiveMarkerProperty = 
            DependencyProperty.RegisterAttached("ActiveMarker", typeof(IMarker), typeof(PositionMarkerCommandBinder), new PropertyMetadata(ActiveMarkerChangedCallback));

        #endregion Fields

        #region Enumerations

        public enum Position
        {
            Active,
            PreviousActive,
            NextActive
        }

        #endregion Enumerations

        #region Methods

        public static IMediaItem GetItem(DependencyObject obj)
        {
            return (IMediaItem)obj.GetValue(ItemProperty);
        }

        public static string GetMarkerSelectorMetadataKey(DependencyObject obj)
        {
            return (string)obj.GetValue(MarkerSelectorMetadataKeyProperty);
        }

        public static object GetMarkerSelectorMetadataValue(DependencyObject obj)
        {
            return (object)obj.GetValue(MarkerSelectorMetadataValueProperty);
        }

        public static Position GetPosition(DependencyObject obj)
        {
            return (Position)obj.GetValue(PositionProperty);
        }

        public static void SetItem(DependencyObject obj, IMediaItem value)
        {
            obj.SetValue(ItemProperty, value);
        }

        public static void SetMarkerSelectorMetadataKey(DependencyObject obj, string value)
        {
            obj.SetValue(MarkerSelectorMetadataKeyProperty, value);
        }

        public static void SetMarkerSelectorMetadataValue(DependencyObject obj, object value)
        {
            obj.SetValue(MarkerSelectorMetadataValueProperty, value);
        }

        public static void SetPosition(DependencyObject obj, Position value)
        {
            obj.SetValue(PositionProperty, value);
        }

        internal static IMarker GetActiveMarker(DependencyObject obj)
        {
            return (IMarker)obj.GetValue(ActiveMarkerProperty);
        }

        internal static void SetActiveMarker(DependencyObject obj, IMarker value)
        {
            obj.SetValue(ActiveMarkerProperty, value);
        }

        private static void ActiveMarkerChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = GetSelector((FrameworkElement)d);
            var activeMarker = GetActiveMarker(d);
            if(selector == null
                || activeMarker == null)
            {
                CommandService.SetCommandParameter(d, null);
                return;
            }

            object position = d.GetValue(PositionProperty);
            Position p = Position.Active;
            if (position != null)
            {
                p = (Position)position;
            }

            switch (p)
            {
                default:
                case Position.Active:
                    CommandService.SetCommandParameter(d, activeMarker.Position);
                    break;
                case Position.PreviousActive:
                    {
                        var idx = selector.Markers.IndexOf(activeMarker);
                        if (idx > 0)
                            CommandService.SetCommandParameter(d, selector.Markers[idx - 1].Position);
                        else
                            CommandService.SetCommandParameter(d, null);
                    } break;
                case Position.NextActive:
                    {
                        var idx = selector.Markers.IndexOf(activeMarker);
                        if (idx < selector.Markers.Count - 1)
                            CommandService.SetCommandParameter(d, selector.Markers[idx + 1].Position);
                        else
                            CommandService.SetCommandParameter(d, null);
                    } break;
            }
        }

        private static void BindActiveMarker(FrameworkElement d)
        {
            IMarkerSelector selector = GetSelector(d);

            if (selector == null)
            {
                d.ClearValue(ActiveMarkerProperty);
            }
            else
            {
                d.SetBinding(ActiveMarkerProperty, new System.Windows.Data.Binding("ActiveMarker") { Source = selector });
            }
        }

        private static IMarkerSelector GetSelector(FrameworkElement d)
        {
            var key = GetMarkerSelectorMetadataKey(d);
            var item = GetItem(d);
            var value = GetMarkerSelectorMetadataValue(d);
            IMarkerSelector selector = null;
            if (!string.IsNullOrEmpty(key) && item != null)
            {
                if (value == null)
                {
                    selector = (from s in item.MarkerSelectors
                                where s.Metadata != null
                                && s.Metadata.ContainsKey(key)
                                select s).FirstOrDefault();
                }
                else
                {
                    selector = (from s in item.MarkerSelectors
                                where s.Metadata != null
                                && s.Metadata.ContainsKey(key)
                                && value.Equals(s.Metadata[key])
                                select s).FirstOrDefault();
                }
            }
            return selector;
        }

        private static void ItemChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindActiveMarker((FrameworkElement)d);
        }

        private static void MarkerSelectorMetadataKeyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindActiveMarker((FrameworkElement)d);
        }

        private static void MarkerSelectorMetadataValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindActiveMarker((FrameworkElement)d);
        }

        private static void PositionChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion Methods
    }
}