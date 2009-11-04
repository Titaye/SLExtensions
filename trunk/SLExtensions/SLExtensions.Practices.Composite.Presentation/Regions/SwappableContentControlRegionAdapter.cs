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
using Microsoft.Practices.Composite.Presentation.Regions;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.Composite.Presentation;
using SLExtensions.Controls;
using System.Collections.Specialized;

namespace SLExtensions.Practices.Composite.Presentation.Regions
{
    public class SwappableContentControlRegionAdapter : RegionAdapterBase<SwappableContentControl>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ContentControlRegionAdapter"/>.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        public SwappableContentControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        /// <summary>
        /// Adapts a <see cref="ContentControl"/> to an <see cref="IRegion"/>.
        /// </summary>
        /// <param name="region">The new region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        protected override void Adapt(IRegion region, SwappableContentControl regionTarget)
        {
            bool contentIsSet = regionTarget.Content != null;
#if !SILVERLIGHT
            contentIsSet = contentIsSet || (BindingOperations.GetBinding(regionTarget, ContentControl.ContentProperty) != null);
#endif
            if (contentIsSet)
            {
                throw new InvalidOperationException("Resources.ContentControlHasContentException");
            }

            region.ActiveViews.CollectionChanged += delegate
            {
                var content = region.ActiveViews.FirstOrDefault();
                Func<UIElement> creationDelegate = content as Func<UIElement>;
                if (creationDelegate != null)
                {
                    content = creationDelegate();
                }
                regionTarget.Content = content;
            };

            region.Views.CollectionChanged +=
                (sender, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Add && region.ActiveViews.Count() == 0)
                    {
                        region.Activate(e.NewItems[0]);
                    }
                };
        }

        /// <summary>
        /// Creates a new instance of <see cref="SingleActiveRegion"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="SingleActiveRegion"/>.</returns>
        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}