// <copyright file="LayerHost.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Layers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// The layer host that will display layeritem. It allows zooming and panning its children.
    /// </summary>
    [TemplatePart(Name = LayerHost.RootElementName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = LayerHost.LayersHostElementName, Type = typeof(Panel))]
    [ContentProperty("LayerSources")]
    public class LayerHost : Control, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Is mouse move dependency property
        /// </summary>
        public static readonly DependencyProperty IsMouseMoveEnabledProperty = 
            DependencyProperty.Register(
                "IsMouseMoveEnabled",
                typeof(bool),
                typeof(LayerHost),
                null);

        /// <summary>
        /// LayerDefinitions dependency property
        /// </summary>
        public static readonly DependencyProperty LayerDefinitionsProperty = 
            DependencyProperty.Register(
                "LayerDefinitions",
                typeof(ObservableCollection<LayerDefinition>),
                typeof(LayerHost),
                new PropertyMetadata((s, e) => ((LayerHost)s).OnLayerDefinitionsChanged((ObservableCollection<LayerDefinition>)e.OldValue, (ObservableCollection<LayerDefinition>)e.NewValue)));

        /// <summary>
        /// Layer name attached property
        /// </summary>
        public static readonly DependencyProperty LayerProperty = 
            DependencyProperty.RegisterAttached(
                "Layer",
                typeof(string),
                typeof(LayerHost),
                new PropertyMetadata(OnLayerChanged));

        /// <summary>
        /// Layer source dependency property
        /// </summary>
        public static readonly DependencyProperty LayerSourcesProperty = 
            DependencyProperty.Register(
                "LayerSources",
                typeof(ObservableCollection<LayerSource>),
                typeof(LayerHost),
                new PropertyMetadata((s, e) => ((LayerHost)s).OnLayerSourcesChanged((ObservableCollection<LayerSource>)e.OldValue, (ObservableCollection<LayerSource>)e.NewValue)));

        /// <summary>
        /// Zoom dependency propety
        /// </summary>
        private static readonly DependencyProperty ZoomProperty = 
            DependencyProperty.Register(
                "Zoom",
                typeof(double),
                typeof(LayerHost),
                new PropertyMetadata((s, e) => ((LayerHost)s).OnZoomChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// Template layers host element name
        /// </summary>
        private const string LayersHostElementName = "LayersHost";

        /// <summary>
        /// Template root element name
        /// </summary>
        private const string RootElementName = "RootElement";

        /// <summary>
        /// Clip geometry
        /// </summary>
        private RectangleGeometry clip;

        /// <summary>
        /// default layer for holding items when no layer name is attached.
        /// </summary>
        private Canvas defaultLayer;

        /// <summary>
        /// Is mouse down captured
        /// </summary>
        private bool ismouseDownCaptured;

        /// <summary>
        /// store last mouse position
        /// </summary>
        private Point lastMouseMovePoint;

        /// <summary>
        /// local scale transform object
        /// </summary>
        private ScaleTransform layerHostScaleTransform;

        /// <summary>
        /// store desired scaleX
        /// </summary>
        private double layerHostScaleTransformScaleX = 1;

        /// <summary>
        /// store desired scaleY
        /// </summary>
        private double layerHostScaleTransformScaleY = 1;

        /// <summary>
        /// local trasnformation group object
        /// </summary>
        private TransformGroup layerHostTransform;

        /// <summary>
        /// local translate transform object
        /// </summary>
        private TranslateTransform layerHostTranslateTransform;

        /// <summary>
        /// store desired translateX
        /// </summary>
        private double layerHostTranslateTransformX = 0;

        /// <summary>
        /// store desired translateY
        /// </summary>
        private double layerHostTranslateTransformY = 0;

        /// <summary>
        /// store layers by names
        /// </summary>
        private Dictionary<string, Canvas> layers = new Dictionary<string, Canvas>();

        /// <summary>
        /// template layers host element
        /// </summary>
        private Canvas layersHostElement;

        /// <summary>
        /// when <c>true</c> prevents the scale assignation when zoom property changes
        /// </summary>
        private bool lockZoom;

        /// <summary>
        /// last mouse down coord
        /// </summary>
        private Point mouseDownPoint;

        /// <summary>
        /// store translate xy when mouse down occured
        /// </summary>
        private Point mouseDownTransatePoint;

        /// <summary>
        /// template root element
        /// </summary>
        private FrameworkElement rootElement;

        /// <summary>
        /// Has been the OnApplyTemplate called
        /// </summary>
        private bool templateLoaded = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerHost"/> class.
        /// </summary>
        public LayerHost()
        {
            DefaultStyleKey = typeof(LayerHost);
            this.layerHostTranslateTransform = new TranslateTransform();
            this.layerHostScaleTransform = new ScaleTransform();

            this.layerHostTransform = new TransformGroup();
            this.layerHostTransform.Children.Add(this.layerHostScaleTransform);
            this.layerHostTransform.Children.Add(this.layerHostTranslateTransform);

            this.LayerSources = new ObservableCollection<LayerSource>();
            this.LayerDefinitions = new ObservableCollection<LayerDefinition>();
            this.IsMouseMoveEnabled = true;
            this.Zoom = 1;

            this.SizeChanged += this.Layer_SizeChanged;
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mouse move enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mouse move enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsMouseMoveEnabled
        {
            get { return (bool)GetValue(IsMouseMoveEnabledProperty); }
            set { SetValue(IsMouseMoveEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets the layer definitions.
        /// </summary>
        /// <value>The layer definitions.</value>
        public ObservableCollection<LayerDefinition> LayerDefinitions
        {
            get { return (ObservableCollection<LayerDefinition>)GetValue(LayerDefinitionsProperty); }
            set { SetValue(LayerDefinitionsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the layer sources.
        /// </summary>
        /// <value>The layer sources.</value>
        public ObservableCollection<LayerSource> LayerSources
        {
            get { return (ObservableCollection<LayerSource>)GetValue(LayerSourcesProperty); }
            set { SetValue(LayerSourcesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the zoom.
        /// </summary>
        /// <value>The zoom value.</value>
        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the layer name of a dependency object
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <returns>The layer name.</returns>
        public static string GetLayer(DependencyObject d)
        {
            return d.GetValue(LayerProperty) as string;
        }

        /// <summary>
        /// Sets a layername on a dependency object
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="name">The layer name.</param>
        public static void SetLayer(DependencyObject d, string name)
        {
            d.SetValue(LayerProperty, name);
        }

        /// <summary>
        /// Gets the layer contents.
        /// </summary>
        /// <param name="layerName">Name of the layer.</param>
        /// <returns>the layer content</returns>
        public IEnumerable<UIElement> GetLayerContents(string layerName)
        {
            Canvas layer;
            if (!this.layers.TryGetValue(layerName, out layer))
            {
                return new List<UIElement>();
            }

            return new List<UIElement>(layer.Children);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.rootElement = this.GetTemplateChild(RootElementName) as Panel;
            this.layersHostElement = this.GetTemplateChild(LayersHostElementName) as Canvas;

            this.defaultLayer = new Canvas();
            if (this.layersHostElement != null)
            {
                this.layersHostElement.Children.Add(this.defaultLayer);
            }

            this.templateLoaded = true;

            if (this.layersHostElement != null)
            {
                this.SetLayerHostTransform();
            }

            this.LoadLayers();
            this.LoadLayerSources();
        }

        /// <summary>
        /// Set the top left point
        /// </summary>
        /// <param name="newPoint">Point in pixel at the current zoom level to be moved on the top left corner</param>
        /// <param name="animate">if set to <c>true</c> uses an animation</param>
        public void SetTopLeftPoint(Point newPoint, bool animate)
        {
            SetTopLeftPoint(newPoint, animate, Zoom);
        }

        /// <summary>
        /// Set the top left point
        /// </summary>
        /// <param name="newPoint">Point in pixel at the given zoom level to be moved on the top left corner</param>
        /// <param name="animate">if set to <c>true</c> uses an animation</param>
        /// <param name="zoom">the zoom level to compute position</param>
        public void SetTopLeftPoint(Point newPoint, bool animate, double zoom)
        {
            this.layerHostTranslateTransformX = -newPoint.X / zoom;
            this.layerHostTranslateTransformY = -newPoint.Y / zoom;

            if (!animate)
            {
                this.layerHostTranslateTransform.X = this.layerHostTranslateTransformX;
                this.layerHostTranslateTransform.Y = this.layerHostTranslateTransformY;
            }
            else
            {
                Storyboard zoomAnimtion = new Storyboard();

                CreateTimeLine(this.layerHostTranslateTransformX, zoomAnimtion, this.layerHostTranslateTransform, "(TranslateTransform.X)");
                CreateTimeLine(this.layerHostTranslateTransformY, zoomAnimtion, this.layerHostTranslateTransform, "(TranslateTransform.Y)");

                zoomAnimtion.Completed += delegate
                {
                    this.ClipLayers();
                };
                this.ClipLayers();
                zoomAnimtion.Begin();
            }
        }

        /// <summary>
        /// Transforms a pixel point to a host point.
        /// </summary>
        /// <param name="layerPoint">The layer point.</param>
        /// <returns>the host point</returns>
        public Point TransformLayerToLocalPixel(Point layerPoint)
        {
            return this.TransformLayerToLocalPixel(layerPoint, this.layerHostScaleTransformScaleX);
        }

        /// <summary>
        /// Transforms a pixel point to a host point.
        /// </summary>
        /// <param name="layerPoint">The layer point.</param>
        /// <param name="scale">the scale.</param>
        /// <returns>the host point</returns>
        public Point TransformLayerToLocalPixel(Point layerPoint, double scale)
        {
            return new Point(layerPoint.X * scale + this.layerHostTranslateTransformX, layerPoint.Y * scale + this.layerHostTranslateTransformY);
        }

        /// <summary>
        /// Transforms a local point to layer pixel point.
        /// </summary>
        /// <param name="localPoint">The local point.</param>
        /// <returns>the layer point</returns>
        public Point TransformLocalToLayerPixel(Point localPoint)
        {
            return this.TransformLocalToLayerPixel(localPoint, this.layerHostScaleTransformScaleX);
        }

        /// <summary>
        /// Transforms a local point to layer pixel point.
        /// </summary>
        /// <param name="localPoint">The local point.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>the layer point</returns>
        public Point TransformLocalToLayerPixel(Point localPoint, double scale)
        {
            return new Point((localPoint.X - this.layerHostTranslateTransformX) / scale, (localPoint.Y - this.layerHostTranslateTransformY) / scale);
        }

        /// <summary>
        /// Zooms and let the localInvariantPoint at the same position.
        /// </summary>
        /// <param name="newZoom">The new zoom.</param>
        /// <param name="localInvariantPoint">The local invariant point. This point will remain at the same location on the screen.</param>
        /// <param name="animate">if set to <c>true</c> use an animation.</param>
        public void ZoomAndInvariantLocal(double newZoom, Point localInvariantPoint, bool animate)
        {
            Point layerInvariantPoint = this.TransformLocalToLayerPixel(localInvariantPoint);

            Point newZoomInvariantPoint = new Point(layerInvariantPoint.X * newZoom, layerInvariantPoint.Y * newZoom);
            Point newLayerTopLeft = new Point(newZoomInvariantPoint.X - localInvariantPoint.X, newZoomInvariantPoint.Y - localInvariantPoint.Y);

            this.layerHostTranslateTransformX = -newLayerTopLeft.X;
            this.layerHostTranslateTransformY = -newLayerTopLeft.Y;
            this.layerHostScaleTransformScaleX = newZoom;
            this.layerHostScaleTransformScaleY = newZoom;

            if (!animate)
            {
                this.layerHostTranslateTransform.X = this.layerHostTranslateTransformX;
                this.layerHostTranslateTransform.Y = this.layerHostTranslateTransformY;
                this.Zoom = newZoom;
            }
            else
            {
                this.lockZoom = true;
                try
                {
                    Storyboard zoomAnimtion = new Storyboard();

                    CreateTimeLine(this.layerHostTranslateTransformX, zoomAnimtion, this.layerHostTranslateTransform, "(TranslateTransform.X)");
                    CreateTimeLine(this.layerHostTranslateTransformY, zoomAnimtion, this.layerHostTranslateTransform, "(TranslateTransform.Y)");

                    CreateTimeLine(this.layerHostScaleTransformScaleX, zoomAnimtion, this.layerHostScaleTransform, "(ScaleTransform.ScaleX)");
                    CreateTimeLine(this.layerHostScaleTransformScaleY, zoomAnimtion, this.layerHostScaleTransform, "(ScaleTransform.ScaleY)");

                    zoomAnimtion.Completed += delegate
                    {
                        this.ClipLayers();
                        this.OnPropertyChanged(this.GetPropertyName(n => n.Zoom));
                    };

                    this.Zoom = newZoom;
                    this.ClipLayers();
                    zoomAnimtion.Begin();

                }
                finally
                {
                    this.lockZoom = false;
                }
            }
        }

        /// <summary>
        /// When implemented in a derived class, provides the behavior for the "Arrange" pass of Silverlight layout.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);
            if (this.clip == null)
            {
                this.clip = new RectangleGeometry();
                this.Clip = this.clip;
            }

            this.clip.Rect = new Rect(0, 0, size.Width, size.Height);
            return size;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled)
            {
                return;
            }

            if (this.IsMouseMoveEnabled)
            {
                this.ismouseDownCaptured = this.CaptureMouse();
                this.mouseDownPoint = e.GetPosition(this);
                this.mouseDownTransatePoint = new Point(this.layerHostTranslateTransformX, this.layerHostTranslateTransformY);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (e.Handled)
            {
                return;
            }

            if (this.ismouseDownCaptured)
            {
                this.ReleaseMouseCapture();
            }

            this.ismouseDownCaptured = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            this.lastMouseMovePoint = e.GetPosition(this);

            if (this.IsMouseMoveEnabled && this.ismouseDownCaptured)
            {
                Point newPoint = e.GetPosition(this);
                double deltaX = (newPoint.X - this.mouseDownPoint.X);
                double deltaY = (newPoint.Y - this.mouseDownPoint.Y);

                this.layerHostTranslateTransformX = this.mouseDownTransatePoint.X + deltaX;
                this.layerHostTranslateTransformY = this.mouseDownTransatePoint.Y + deltaY;

                this.layerHostTranslateTransform.X = this.layerHostTranslateTransformX;
                this.layerHostTranslateTransform.Y = this.layerHostTranslateTransformY;

                // Debug.WriteLine("delta " + deltaX + " " + deltaY + "     " + layerHostTranslateTransformX + " " + layerHostTranslateTransformY + "\n" + layerHostTranslateTransform.X + "  " + layerHostTranslateTransform.Y);
                this.ClipLayers();

            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            double newZoom = this.Zoom;
            if (this.IsMouseMoveEnabled)
            {
                if (e.Delta > 0)
                {
                    newZoom *= 1.10;
                }
                else
                {
                    newZoom /= 1.10;
                }
            }

            this.ZoomAndInvariantLocal(newZoom, this.lastMouseMovePoint, true);
        }

        /// <summary>
        /// Called when a property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// add a zoom animation timeline.
        /// </summary>
        /// <param name="val">The end value of the timeline.</param>
        /// <param name="sb">The storyboard.</param>
        /// <param name="target">The target of the animation.</param>
        /// <param name="property">The property to animate.</param>
        private static void CreateTimeLine(double val, Storyboard sb, DependencyObject target, string property)
        {
            DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
            sb.Children.Add(anim);

            Storyboard.SetTarget(anim, target);
            Storyboard.SetTargetProperty(anim, new PropertyPath(property));

            SplineDoubleKeyFrame kf = new SplineDoubleKeyFrame();
            anim.KeyFrames.Add(kf);

            // kf.KeySpline = new KeySpline();
            // kf.KeySpline.ControlPoint1 = new Point(0, 0.7);
            // kf.KeySpline.ControlPoint2 = new Point(0.3, 1);
            kf.Value = val;
            kf.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100));
        }

        /// <summary>
        /// called when the layer changed.
        /// </summary>
        /// <param name="d">The dependency object source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Adds the layer definitions.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void AddLayerDefinitions(IEnumerable<LayerDefinition> newValue)
        {
            if (!this.templateLoaded)
            {
                return;
            }

            foreach (var item in newValue)
            {
                Canvas cv = new Canvas();
                this.layers.Add(item.LayerName, cv);
                if (this.layersHostElement != null)
                {
                    this.layersHostElement.Children.Add(cv);
                }
            }
        }

        /// <summary>
        /// Adds the layer source.
        /// </summary>
        /// <param name="item">The layer source item.</param>
        private void AddLayerSource(LayerSource item)
        {
            if (!this.templateLoaded)
            {
                return;
            }

            if (this.layersHostElement != null)
            {
                item.LayerHost = this;

                string layerName = GetLayer(item);
                Canvas layer;
                if (!string.IsNullOrEmpty(layerName) && this.layers.TryGetValue(layerName, out layer))
                {
                    layer.Children.Add(item);
                }
                else
                {
                    this.defaultLayer.Children.Add(item);
                }
            }
        }

        /// <summary>
        /// Adds the layer sources.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void AddLayerSources(IEnumerable<LayerSource> newValue)
        {
            foreach (var item in newValue)
            {
                this.AddLayerSource(item);
            }
        }

        /// <summary>
        /// Clips the layers.
        /// </summary>
        private void ClipLayers()
        {
            // Compute clipRect in layerCoordinates
            Rect clipRect = new Rect(
                -this.layerHostTranslateTransformX / this.layerHostScaleTransformScaleX,
                -this.layerHostTranslateTransformY / this.layerHostScaleTransformScaleY,
                this.ActualWidth / this.layerHostScaleTransformScaleX,
                this.ActualHeight / this.layerHostScaleTransformScaleY);

            foreach (var item in this.LayerSources)
            {
                item.ClipHost(clipRect, this.Zoom);
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the layerDefinitions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void LayerDefinitions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                this.RemoveLayerDefinitions(e.OldItems.Cast<LayerDefinition>());
            }

            if (e.NewItems != null)
            {
                this.AddLayerDefinitions(e.NewItems.Cast<LayerDefinition>());
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of the Layer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void Layer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ClipLayers();
        }

        /// <summary>
        /// Loads the layer sources.
        /// </summary>
        private void LoadLayerSources()
        {
            foreach (var item in this.LayerSources)
            {
                this.AddLayerSource(item);
            }
        }

        /// <summary>
        /// Loads the layers.
        /// </summary>
        private void LoadLayers()
        {
            this.AddLayerDefinitions(this.LayerDefinitions);
        }

        /// <summary>
        /// Handles the CollectionChanged event of the MapSources control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void MapSources_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                this.AddLayerSources(e.NewItems.Cast<LayerSource>());
            }

            if (e.OldItems != null)
            {
                this.RemoveLayerSources(e.OldItems.Cast<LayerSource>());
            }

            this.ClipLayers();
        }

        /// <summary>
        /// called when the layer definitions changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnLayerDefinitionsChanged(ObservableCollection<LayerDefinition> oldValue, ObservableCollection<LayerDefinition> newValue)
        {
            this.OnPropertyChanged(this.GetPropertyName(n => n.LayerDefinitions));

            if (oldValue != null)
            {
                newValue.CollectionChanged -= this.LayerDefinitions_CollectionChanged;
                this.RemoveLayerDefinitions(oldValue);
            }

            if (newValue != null)
            {
                this.AddLayerDefinitions(newValue);
                newValue.CollectionChanged += this.LayerDefinitions_CollectionChanged;
            }
        }

        /// <summary>
        /// called when the layer sources changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnLayerSourcesChanged(ObservableCollection<LayerSource> oldValue, ObservableCollection<LayerSource> newValue)
        {
            if (oldValue != null)
            {
                oldValue.CollectionChanged -= this.MapSources_CollectionChanged;
                this.RemoveLayerSources(oldValue);
            }

            if (newValue != null)
            {
                newValue.CollectionChanged += this.MapSources_CollectionChanged;
                this.AddLayerSources(newValue);
            }

            this.OnPropertyChanged(this.GetPropertyName(n => n.LayerSources));
        }

        /// <summary>
        /// called when the zoom changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnZoomChanged(double oldValue, double newValue)
        {
            this.layerHostScaleTransformScaleX = newValue;
            this.layerHostScaleTransformScaleY = newValue;

            if (!this.lockZoom)
            {
                this.layerHostScaleTransform.ScaleX = this.layerHostScaleTransformScaleX;
                this.layerHostScaleTransform.ScaleY = this.layerHostScaleTransformScaleY;
                this.ClipLayers();
            }

            this.OnPropertyChanged(this.GetPropertyName(n => n.Zoom));
        }

        /// <summary>
        /// Removes the layer definitions.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        private void RemoveLayerDefinitions(IEnumerable<LayerDefinition> oldValue)
        {
            if (!this.templateLoaded)
            {
                return;
            }

            foreach (var item in oldValue)
            {
                if (this.layersHostElement != null)
                {
                    this.layersHostElement.Children.Remove(this.layers[item.LayerName]);
                }

                this.layers.Remove(item.LayerName);
            }
        }

        /// <summary>
        /// Removes the layer source.
        /// </summary>
        /// <param name="item">The layersource item.</param>
        private void RemoveLayerSource(LayerSource item)
        {
            if (!this.templateLoaded)
            {
                return;
            }

            Panel parent = item.Parent as Panel;
            if (parent != null)
            {
                parent.Children.Remove(item);
            }
        }

        /// <summary>
        /// Removes the layer sources.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        private void RemoveLayerSources(IEnumerable<LayerSource> oldValue)
        {
            foreach (var item in oldValue)
            {
                this.RemoveLayerSource(item);
            }
        }

        /// <summary>
        /// Sets the layer host transform.
        /// </summary>
        private void SetLayerHostTransform()
        {
            this.layersHostElement.RenderTransformOrigin = new Point();
            this.layersHostElement.RenderTransform = this.layerHostTransform;
        }

        #endregion Methods
    }
}