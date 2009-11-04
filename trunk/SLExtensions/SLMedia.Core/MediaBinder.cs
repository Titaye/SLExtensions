namespace SLMedia.Core
{
    using System;
    using System.Collections.Generic;
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

    using SLExtensions;

    public static class MediaBinder
    {
        #region Fields

        //#region Fields
        // Using a DependencyProperty as the backing store for MediaElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaElementProperty = 
            DependencyProperty.RegisterAttached("MediaElement", typeof(string), typeof(MediaBinder), new PropertyMetadata(MediaElementChangedCallback));

        // Using a DependencyProperty as the backing store for MultiScaleImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MultiScaleImageProperty = 
            DependencyProperty.RegisterAttached("MultiScaleImage", typeof(string), typeof(MediaBinder), new PropertyMetadata(MultiScaleImageChangedCallback));

        // Using a DependencyProperty as the backing store for Next.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextProperty = 
            DependencyProperty.RegisterAttached("Next", typeof(string), typeof(MediaBinder), new PropertyMetadata(NextChangedCallback));

        // Using a DependencyProperty as the backing store for PictureDisplay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PictureDisplayProperty = 
            DependencyProperty.RegisterAttached("PictureDisplay", typeof(string), typeof(MediaBinder), new PropertyMetadata(PictureDisplayChangedCallback));

        // Using a DependencyProperty as the backing store for Previous.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousProperty = 
            DependencyProperty.RegisterAttached("Previous", typeof(string), typeof(MediaBinder), new PropertyMetadata(PreviousChangedCallback));

        // Using a DependencyProperty as the backing store for StateControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateControlProperty = 
            DependencyProperty.RegisterAttached("StateControl", typeof(string), typeof(MediaBinder), new PropertyMetadata(StateControlChangedCallback));

        // Using a DependencyProperty as the backing store for Subscription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubscriptionProperty = 
            DependencyProperty.RegisterAttached("Subscription", typeof(MediaBinderSubscription), typeof(MediaBinder), null);

        // Using a DependencyProperty as the backing store for Subscription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentPubProperty =
            DependencyProperty.RegisterAttached("ContentPub", typeof(string), typeof(MediaBinder), new PropertyMetadata(ContentPubChangedCallback));

        private static List<WeakReference> registeredSubscriptions;

        #endregion Fields

        #region Constructors

        static MediaBinder()
        {
            registeredSubscriptions = new List<WeakReference>();
        }

        #endregion Constructors

        #region Methods

        public static string GetMediaElement(DependencyObject obj)
        {
            return (string)obj.GetValue(MediaElementProperty);
        }

        public static string GetMultiScaleImage(DependencyObject obj)
        {
            return (string)obj.GetValue(MultiScaleImageProperty);
        }

        public static string GetNext(DependencyObject obj)
        {
            return (string)obj.GetValue(NextProperty);
        }

        public static string GetPictureDisplay(DependencyObject obj)
        {
            return (string)obj.GetValue(PictureDisplayProperty);
        }

        public static string GetPrevious(DependencyObject obj)
        {
            return (string)obj.GetValue(PreviousProperty);
        }

        public static string GetContentPub(DependencyObject obj)
        {
            return (string)obj.GetValue(ContentPubProperty);
        }

        public static string GetStateControl(DependencyObject obj)
        {
            return (string)obj.GetValue(StateControlProperty);
        }

        public static MediaBinderSubscription GetSubscription(string newSubscriptionName)
        {
            MediaBinderSubscription subscription = (from s in MediaBinder.registeredSubscriptions
                                                    where s.IsAlive
                                                    let sub = ((MediaBinderSubscription)s.Target)
                                                    where sub.Name == newSubscriptionName
                                                    select sub).FirstOrDefault();
            if (subscription == null)
            {
                subscription = new MediaBinderSubscription(newSubscriptionName);
                MediaBinder.registeredSubscriptions.Add(new WeakReference(subscription));
            }
            return subscription;
        }

        public static void SetMediaElement(DependencyObject obj, string value)
        {
            obj.SetValue(MediaElementProperty, value);
        }

        public static void SetMultiScaleImage(DependencyObject obj, string value)
        {
            obj.SetValue(MultiScaleImageProperty, value);
        }

        public static void SetNext(DependencyObject obj, string value)
        {
            obj.SetValue(NextProperty, value);
        }

        public static void SetPictureDisplay(DependencyObject obj, string value)
        {
            obj.SetValue(PictureDisplayProperty, value);
        }

        public static void SetPrevious(DependencyObject obj, string value)
        {
            obj.SetValue(PreviousProperty, value);
        }

        public static void SetContentPub(DependencyObject obj, string value)
        {
            obj.SetValue(ContentPubProperty, value);
        }

        public static void SetStateControl(DependencyObject obj, string value)
        {
            obj.SetValue(StateControlProperty, value);
        }

        private static MediaBinderSubscription EnsureSubscription(DependencyObject obj, string newSubscriptionName)
        {
            MediaBinderSubscription subscription = GetSubscription(newSubscriptionName);
            SetSubscription(obj, subscription);
            return subscription;
        }

        private static MediaBinderSubscription GetSubscription(DependencyObject obj)
        {
            return (MediaBinderSubscription)obj.GetValue(SubscriptionProperty);
        }

        private static void MediaElementChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string newSubscriptionName = e.NewValue as string;
            if (string.IsNullOrEmpty(newSubscriptionName))
            {
                MediaBinderSubscription subscription = GetSubscription(d);
                if (subscription != null)
                {
                    subscription.MediaElement = null;
                }

                SetSubscription(d, null);
            }
            else
            {
                MediaBinderSubscription subscription = EnsureSubscription(d, newSubscriptionName);
                subscription.MediaElement = d as MediaElement;
            }
        }

        private static void MultiScaleImageChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string newSubscriptionName = e.NewValue as string;
            if (string.IsNullOrEmpty(newSubscriptionName))
            {
                MediaBinderSubscription subscription = GetSubscription(d);
                if (subscription != null)
                {
                    subscription.MultiScaleImage = null;
                }

                SetSubscription(d, null);
            }
            else
            {
                MediaBinderSubscription subscription = EnsureSubscription(d, newSubscriptionName);
                subscription.MultiScaleImage = d as MultiScaleImage;
            }
        }

        private static void NextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string newSubscriptionName = e.NewValue as string;
            if (string.IsNullOrEmpty(newSubscriptionName))
            {
                MediaBinderSubscription subscription = GetSubscription(d);
                if (subscription != null)
                {
                    subscription.NextElement = null;
                }

                SetSubscription(d, null);
            }
            else
            {
                MediaBinderSubscription subscription = EnsureSubscription(d, newSubscriptionName);
                subscription.NextElement = d as FrameworkElement;
            }
        }

        private static void PictureDisplayChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string newSubscriptionName = e.NewValue as string;
            if (string.IsNullOrEmpty(newSubscriptionName))
            {
                MediaBinderSubscription subscription = GetSubscription(d);
                if (subscription != null)
                {
                    subscription.PictureDisplayElement = null;
                }

                SetSubscription(d, null);
            }
            else
            {
                MediaBinderSubscription subscription = EnsureSubscription(d, newSubscriptionName);
                subscription.PictureDisplayElement = d as FrameworkElement;
            }
        }

        private static void PreviousChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string newSubscriptionName = e.NewValue as string;
            if (string.IsNullOrEmpty(newSubscriptionName))
            {
                MediaBinderSubscription subscription = GetSubscription(d);
                if (subscription != null)
                {
                    subscription.PreviousElement = null;
                }

                SetSubscription(d, null);
            }
            else
            {
                MediaBinderSubscription subscription = EnsureSubscription(d, newSubscriptionName);
                subscription.PreviousElement = d as FrameworkElement;
            }
        }

        private static void ContentPubChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string newSubscriptionName = e.NewValue as string;
            if (string.IsNullOrEmpty(newSubscriptionName))
            {
                MediaBinderSubscription subscription = GetSubscription(d);
                if (subscription != null)
                {
                    subscription.ContentPub = null;
                }

                SetSubscription(d, null);
            }
            else
            {
                MediaBinderSubscription subscription = EnsureSubscription(d, newSubscriptionName);
                subscription.ContentPub = d as ContentControl;
            }
        }

        private static void SetSubscription(DependencyObject obj, MediaBinderSubscription value)
        {
            obj.SetValue(SubscriptionProperty, value);
        }


        private static void StateControlChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string newSubscriptionName = e.NewValue as string;
            if (string.IsNullOrEmpty(newSubscriptionName))
            {
                MediaBinderSubscription subscription = GetSubscription(d);
                if (subscription != null)
                {
                    subscription.StateControl = null;
                }

                SetSubscription(d, null);
            }
            else
            {
                MediaBinderSubscription subscription = EnsureSubscription(d, newSubscriptionName);
                subscription.StateControl = d as Control;
            }
        }



        public static string GetPlay(DependencyObject obj)
        {
            return (string)obj.GetValue(PlayProperty);
        }

        public static void SetPlay(DependencyObject obj, string value)
        {
            obj.SetValue(PlayProperty, value);
        }

        // Using a DependencyProperty as the backing store for Play.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayProperty =
            DependencyProperty.RegisterAttached("Play", typeof(string), typeof(MediaBinder), new PropertyMetadata(PlayPropertyChangedCallback));

        private static void PlayPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string newSubscriptionName = e.NewValue as string;
            if (string.IsNullOrEmpty(newSubscriptionName))
            {
                MediaBinderSubscription subscription = GetSubscription(d);
                if (subscription != null)
                {
                    subscription.PlayElement = null;
                }

                SetSubscription(d, null);
            }
            else
            {
                MediaBinderSubscription subscription = EnsureSubscription(d, newSubscriptionName);
                subscription.PlayElement = d as FrameworkElement;
            }
        }

        public static string GetStop(DependencyObject obj)
        {
            return (string)obj.GetValue(StopProperty);
        }

        public static void SetStop(DependencyObject obj, string value)
        {
            obj.SetValue(StopProperty, value);
        }

        // Using a DependencyProperty as the backing store for Stop.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StopProperty =
            DependencyProperty.RegisterAttached("Stop", typeof(string), typeof(MediaBinder), new PropertyMetadata(StopPropertyChangedCallback));

        private static void StopPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string newSubscriptionName = e.NewValue as string;
            if (string.IsNullOrEmpty(newSubscriptionName))
            {
                MediaBinderSubscription subscription = GetSubscription(d);
                if (subscription != null)
                {
                    subscription.StopElement = null;
                }

                SetSubscription(d, null);
            }
            else
            {
                MediaBinderSubscription subscription = EnsureSubscription(d, newSubscriptionName);
                subscription.StopElement = d as FrameworkElement;
            }
        }

        public static string GetPause(DependencyObject obj)
        {
            return (string)obj.GetValue(PauseProperty);
        }

        public static void SetPause(DependencyObject obj, string value)
        {
            obj.SetValue(PauseProperty, value);
        }

        // Using a DependencyProperty as the backing store for Pause.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PauseProperty =
            DependencyProperty.RegisterAttached("Pause", typeof(string), typeof(MediaBinder), new PropertyMetadata(PausePropertyChangedCallback));

        private static void PausePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string newSubscriptionName = e.NewValue as string;
            if (string.IsNullOrEmpty(newSubscriptionName))
            {
                MediaBinderSubscription subscription = GetSubscription(d);
                if (subscription != null)
                {
                    subscription.PauseElement = null;
                }

                SetSubscription(d, null);
            }
            else
            {
                MediaBinderSubscription subscription = EnsureSubscription(d, newSubscriptionName);
                subscription.PauseElement = d as FrameworkElement;
            }
        }
        #endregion Methods
    }
}