using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Interactivity;
using System.Windows.Input.Manipulations;
using System.Windows.Threading;
using System.Collections.Generic;

namespace SLExtensions.Manipulation
{
    public class DeepzoomManipulationBehavior : Behavior<MultiScaleImage>, IDisposable
    {
        #region private fields
        private const float DefaultDpi = 96.0f;

        // deceleration: inches/second squared 
        private const float Deceleration = 10.0f * DefaultDpi / (1000.0f * 1000.0f);
        private const float ExpansionDeceleration = 16.0f * DefaultDpi / (1000.0f * 1000.0f);

       // minimum/maximum flick velocities
        private const float MinimumFlickVelocity = 2.0f * DefaultDpi / 1000.0f;                      // =2 inches per sec
        private const float MinimumAngularFlickVelocity = 45.0f / 180.0f * (float)Math.PI / 1000.0f; // =45 degrees per sec
        private const float MinimumExpansionFlickVelocity = 2.0f * DefaultDpi / 1000.0f;             // =2 inches per sec
        private const float MaximumFlickVelocityFactor = 15f;

        // indicates if the mouse is captured or not
        private bool isMouseCaptured;

        // manipulation and inertia processors
        private ManipulationProcessor2D manipulationProcessor;
        private InertiaProcessor2D inertiaProcessor;
        private DispatcherTimer inertiaTimer;
        #endregion

        private void RefreshManipulations()
        {
            this.manipulationProcessor.SupportedManipulations = SupportedManipulations;
            StopInertia();
        }

        private bool canTranslate = true;

        public bool CanTranslate
        {
            get
            {
                return this.canTranslate;
            }

            set
            {
                if (this.canTranslate != value)
                {
                    this.canTranslate = value;
                    RefreshManipulations();
                }
            }
        }

        private bool canScale = true;
        public bool CanScale
        {
            get { return this.canScale; }
            set
            {
                if (this.canScale != value)
                {
                    this.canScale = value;
                    RefreshManipulations();
                }
            }
        }

        public event EventHandler ViewportChanged;

        protected virtual void OnViewportChanged()
        {
            if (ViewportChanged != null)
            {
                ViewportChanged(this, EventArgs.Empty);
            }
        }



        private Point mouseDownPosition;
        private Point mouseDownViewportOrigin;
        #region mouse handlers
        /// <summary>
        /// Here when the mouse goes down on the item.
        /// </summary>
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            // ignore mouse if there is at least on
            if (!TouchHelper.AreAnyTouches && AssociatedObject.CaptureMouse())
            {
                StopInertia();

                this.isMouseCaptured = true;
                this.mouseDownPosition = e.GetPosition(AssociatedObject.Parent as FrameworkElement);
                this.mouseDownViewportOrigin = AssociatedObject.ViewportOrigin;
                ProcessMouse(e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Here when the mouse goes up.
        /// </summary>
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.isMouseCaptured)
            {
                AssociatedObject.ReleaseMouseCapture();
                e.Handled = true;
            }
        }


        /// <summary>
        /// Here when the mouse moves.
        /// </summary>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (this.isMouseCaptured)
            {
                if (TouchHelper.AreAnyTouches)
                {
                    // ignore mouse if there are any touches
                    AssociatedObject.ReleaseMouseCapture();
                }
                else
                {
                    ProcessMouse(e);
                }
            }
        }

        #region HandleMouseWheel

        public bool HandleMouseWheel
        {
            get
            {
                return (bool)GetValue(HandleMouseWheelProperty);
            }

            set
            {
                SetValue(HandleMouseWheelProperty, value);
            }
        }

        /// <summary>
        /// HandleMouseWheel depedency property.
        /// </summary>
        public static readonly DependencyProperty HandleMouseWheelProperty =
            DependencyProperty.Register(
                "HandleMouseWheel",
                typeof(bool),
                typeof(DeepzoomManipulationBehavior),
                new PropertyMetadata(true));

        #endregion HandleMouseWheel

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (HandleMouseWheel)
            {
                var mousePosition = e.GetPosition(AssociatedObject);
                MouseWheel(mousePosition, e.Delta);
                e.Handled = true;
            }
        }

        public void MouseWheel(Point mousePosition, double delta)
        {
            var logicalPosition = AssociatedObject.ElementToLogicalPoint(mousePosition);

            if (delta > 0)
            {
                AssociatedObject.ZoomAboutLogicalPoint(1.2, logicalPosition.X, logicalPosition.Y);
            }
            else
                AssociatedObject.ZoomAboutLogicalPoint(0.8, logicalPosition.X, logicalPosition.Y);
        }

        /// <summary>
        /// Here when we've lost mouse capture.
        /// </summary>
        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (this.isMouseCaptured)
            {
                this.manipulationProcessor.ProcessManipulators(Timestamp, null);
                this.isMouseCaptured = false;
            }
        }

        public bool MouseUseManipulators { get; set; }
	

        /// <summary>
        /// Process a mouse event. Note: mouse and touches at the same time are not supported.
        /// </summary>
        /// <param name="e"></param>
        private void ProcessMouse(MouseEventArgs e)
        {
            UIElement parent = AssociatedObject.Parent as UIElement;
            if (parent == null)
            {
                return;
            }

            Point position = e.GetPosition(parent);
            if (MouseUseManipulators)
            {
                List<Manipulator2D> manipulators = new List<Manipulator2D>();
                manipulators.Add(new Manipulator2D(
                     0,
                     (float)(position.X),
                     (float)(position.Y)));

                this.manipulationProcessor.ProcessManipulators(
                   Timestamp,
                   manipulators);
            }
            else
            {
                var deltax = (position.X - this.mouseDownPosition.X);
                var deltay = (position.Y - this.mouseDownPosition.Y);

                var x = deltax / AssociatedObject.ActualWidth * AssociatedObject.ViewportWidth;
                var y = (deltay / AssociatedObject.AspectRatio) / (AssociatedObject.ActualWidth / AssociatedObject.AspectRatio) * AssociatedObject.ViewportWidth;

                var pt = new Point(mouseDownViewportOrigin.X - x, mouseDownViewportOrigin.Y - y);
                AssociatedObject.ViewportOrigin = pt;
                OnViewportChanged();
            }            
        }
        #endregion

      
        #region touch handlers

        public void CaptureTouch(TouchDevice td)
        {
            td.Capture(this.AssociatedObject);
        }

        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            CaptureTouch(e.TouchPoint.TouchDevice);
        }

        private void OnCapturedTouchReported(object sender, TouchReportedEventArgs e)
        {
            UIElement parent = AssociatedObject.Parent as UIElement;
            if (parent == null)
            {
                return;
            }

            UIElement root = Application.Current.RootVisual;
            if (root == null)
            {
                return;
            }

            // get transformation to convert positions to the parent's coordinate system
            GeneralTransform transform = root.TransformToVisual(parent);
            List<Manipulator2D> manipulators = null;

            foreach (TouchPoint touchPoint in e.TouchPoints)
            {
                Point position = touchPoint.Position;

                // convert to the parent's coordinate system
                position = transform.Transform(position);

                // create a manipulator
                Manipulator2D manipulator = new Manipulator2D(
                    touchPoint.TouchDevice.Id,
                    (float)(position.X),
                    (float)(position.Y));

                if (manipulators == null)
                {
                    // lazy initialization
                    manipulators = new List<Manipulator2D>();
                }
                manipulators.Add(manipulator);
            }

            // process manipulations
            this.manipulationProcessor.ProcessManipulators(
                Timestamp,
                manipulators);
        }
        #endregion

        #region item properties
        /// <summary>
        /// Gets the center of the item, in container coordinates.
        /// </summary>
        private Point Center { get; set; }

        /// <summary>
        /// Gets or sets the orientation of the object, in degrees.
        /// </summary>
        private double Orientation { get; set; }


        /// <summary>
        /// Gets the current timestamp.
        /// </summary>
        private static long Timestamp
        {
            get
            {
                // The question of what tick source to use is a difficult
                // one in general, but for purposes of this test app,
                // DateTime ticks are good enough.
                return DateTime.UtcNow.Ticks;
            }
        }
        #endregion

        #region manipulation handlers
        /// <summary>
        /// Stops inertia.
        /// </summary>
        private void StopInertia()
        {
            if (this.inertiaProcessor.IsRunning)
            {
                this.inertiaProcessor.Complete(Timestamp);
            }

            //  always stop the timer
            this.inertiaTimer.Stop();
        }

        private bool lastUseSpringState = false;
        /// <summary>
        /// Here when manipulation starts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnManipulationStarted(object sender, Manipulation2DStartedEventArgs e)
        {
            StopInertia();
            lastUseSpringState = AssociatedObject.UseSprings;
            AssociatedObject.UseSprings = false;
        }

        /// <summary>
        /// Here when manipulation gives a delta.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnManipulationDelta(object sender, Manipulation2DDeltaEventArgs e)
        {
            var x = e.Delta.TranslationX / AssociatedObject.ActualWidth * AssociatedObject.ViewportWidth;
            var y = (e.Delta.TranslationY / AssociatedObject.AspectRatio) / (AssociatedObject.ActualWidth / AssociatedObject.AspectRatio) * AssociatedObject.ViewportWidth;

            var pt = new Point(AssociatedObject.ViewportOrigin.X - x, AssociatedObject.ViewportOrigin.Y - y);
            AssociatedObject.ViewportOrigin = pt;

            var manipulationOrigin = AssociatedObject.ElementToLogicalPoint(new Point(e.OriginX, e.OriginY));
            AssociatedObject.ZoomAboutLogicalPoint(e.Delta.ScaleX, manipulationOrigin.X, manipulationOrigin.Y);
            OnViewportChanged();
        }


        /// <summary>
        /// Here when manipulation completes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnManipulationCompleted(object sender, Manipulation2DCompletedEventArgs e)
        {
            // Get the inital inertia values
            Vector initialVelocity = new Vector(e.Velocities.LinearVelocityX, e.Velocities.LinearVelocityY);
            float angularVelocity = e.Velocities.AngularVelocity;
            float expansionVelocity = e.Velocities.ExpansionVelocityX;

            bool startFlick = false;

            // Rotate and scale around the center of the item
            inertiaProcessor.InitialOriginX = (float)Center.X;
            inertiaProcessor.InitialOriginY = (float)Center.Y;

            // set initial velocity if translate flicks are allowed
            double velocityLengthSquared = initialVelocity.LengthSquared;
            if (CanTranslate && velocityLengthSquared > MinimumFlickVelocity * MinimumFlickVelocity)
            {
                const double maximumLengthSquared = MaximumFlickVelocityFactor * MinimumFlickVelocity * MinimumFlickVelocity;
                if (velocityLengthSquared > maximumLengthSquared)
                {
                    initialVelocity = Math.Sqrt(maximumLengthSquared / velocityLengthSquared) * initialVelocity;
                }

                startFlick = true;
                inertiaProcessor.TranslationBehavior.InitialVelocityX = (float)initialVelocity.X;
                inertiaProcessor.TranslationBehavior.InitialVelocityY = (float)initialVelocity.Y;
            }
            else
            {
                inertiaProcessor.TranslationBehavior.InitialVelocityX = 0.0f;
                inertiaProcessor.TranslationBehavior.InitialVelocityY = 0.0f;
            }

            // set expansion velocity if scale flicks are allowed
            if (CanScale && Math.Abs(expansionVelocity) >= MinimumExpansionFlickVelocity)
            {
                const float maximumExpansionFlickVelocity = MaximumFlickVelocityFactor * MinimumExpansionFlickVelocity;
                if (Math.Abs(expansionVelocity) >= maximumExpansionFlickVelocity)
                {
                    expansionVelocity = expansionVelocity > 0 ? maximumExpansionFlickVelocity : -maximumExpansionFlickVelocity;
                }
                startFlick = true;
                inertiaProcessor.ExpansionBehavior.InitialVelocityX = expansionVelocity;
                inertiaProcessor.ExpansionBehavior.InitialVelocityY = expansionVelocity;
                //inertiaProcessor.ExpansionBehavior.InitialRadius = (float)Radius;
            }
            else
            {
                inertiaProcessor.ExpansionBehavior.InitialVelocityX = 0.0f;
                inertiaProcessor.ExpansionBehavior.InitialVelocityY = 0.0f;
                //inertiaProcessor.ExpansionBehavior.InitialRadius = 1.0f;
            }

            if (startFlick)
            {
                this.inertiaProcessor.Process(Timestamp);
                this.inertiaTimer.Start();
            }
            else
            {
                this.AssociatedObject.UseSprings = lastUseSpringState;
            }
        }

        /// <summary>
        /// Here when manipulation completes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnInertiaCompleted(object sender, Manipulation2DCompletedEventArgs e)
        {
            this.inertiaTimer.Stop();
            AssociatedObject.UseSprings = lastUseSpringState;
        }

        /// <summary>
        /// Here when the inertia timer ticks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            this.inertiaProcessor.Process(Timestamp);
        }


        /// <summary>
        ///  Determines the correct offset for a render transform on an item with the given properties.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="orientation"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="renderTransformOrigin"></param>
        /// <returns></returns>
        private static Vector CalculateRenderOffset(Point center, double orientation, double width, double height,
            Point renderTransformOrigin)
        {
            // Find the center point based on the orientation, the size of the item,
            // and the RenderTransformOrigin. This tells us exactly where the center
            // of the item is rendered with respect to the upper left corner of the
            // item's layout rect.
            Point renderOrigin = new Point(width * renderTransformOrigin.X, height * renderTransformOrigin.Y);

            Matrix matrix = Matrix.Identity;
            MatrixHelper.RotateAt(ref matrix, orientation, renderOrigin);
            Point renderedCenter = matrix.Transform(new Point(width * 0.5, height * 0.5));

            // Use the rendered center to determine the desired offset for the transform.
            return Vector.Subtruct(center, renderedCenter);
        }


        /// <summary>
        /// Get the manipulations we should currently be supporting.
        /// </summary>
        private Manipulations2D SupportedManipulations
        {
            get
            {
                var manipulation = Manipulations2D.None;
                if (CanTranslate)
                    manipulation |= Manipulations2D.Translate;
                if (CanScale)
                    manipulation |= Manipulations2D.Scale;
                return manipulation;
            }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool dispose)
        {
            // if Dispose is not called, the TouchHelper cleans it up on the next touch frame
            TouchHelper.EnableInput(false/*enable*/);
        }
        #endregion
        protected override void OnAttached()
        {
            this.manipulationProcessor = new ManipulationProcessor2D(SupportedManipulations);
            this.manipulationProcessor.Started += OnManipulationStarted;
            this.manipulationProcessor.Delta += OnManipulationDelta;
            this.manipulationProcessor.Completed += OnManipulationCompleted;

            this.inertiaProcessor = new InertiaProcessor2D();
            this.inertiaProcessor.TranslationBehavior.DesiredDeceleration = Deceleration;
            this.inertiaProcessor.ExpansionBehavior.DesiredDeceleration = ExpansionDeceleration;
            this.inertiaProcessor.Delta += OnManipulationDelta;
            this.inertiaProcessor.Completed += OnInertiaCompleted;

            this.inertiaTimer = new DispatcherTimer();
            this.inertiaTimer.Interval = TimeSpan.FromMilliseconds(30);
            this.inertiaTimer.Tick += OnTimerTick;

            this.AssociatedObject.MouseLeftButtonUp += OnMouseUp;
            this.AssociatedObject.MouseLeftButtonDown += OnMouseDown;
            this.AssociatedObject.MouseMove += OnMouseMove;
            this.AssociatedObject.LostMouseCapture += OnLostMouseCapture;
            this.AssociatedObject.MouseWheel += OnMouseWheel;
            this.AssociatedObject.ViewportChanged += delegate { OnViewportChanged(); };

            TouchHelper.AddHandlers(this.AssociatedObject, new TouchHandlers
            {
                TouchDown = OnTouchDown,
                CapturedTouchReported = OnCapturedTouchReported,
            });
            TouchHelper.EnableInput(true);
            base.OnAttached();
        }
    }
}
