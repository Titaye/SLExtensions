using System;
using System.Net;
using System.Linq;
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
    public class ManipulationBehavior : Behavior<FrameworkElement>, IDisposable
    {
        #region private fields
        private const float DefaultDpi = 96.0f;

        // deceleration: inches/second squared 
        private const float Deceleration = 10.0f * DefaultDpi / (1000.0f * 1000.0f);
        private const float ExpansionDeceleration = 16.0f * DefaultDpi / (1000.0f * 1000.0f);

        // angular deceleration: degrees/second squared
        private const float AngularDeceleration = 270.0f / 180.0f * (float)Math.PI / (1000.0f * 1000.0f);

        // minimum/maximum flick velocities
        private const float MinimumFlickVelocity = 2.0f * DefaultDpi / 1000.0f;                      // =2 inches per sec
        private const float MinimumAngularFlickVelocity = 45.0f / 180.0f * (float)Math.PI / 1000.0f; // =45 degrees per sec
        private const float MinimumExpansionFlickVelocity = 2.0f * DefaultDpi / 1000.0f;             // =2 inches per sec
        private const float MaximumFlickVelocityFactor = 15f;

        // minimum/maximum radius
        private const float MinimumRadius = 60;
        private const float MaximumRadius = 240;

        // indicates if the mouse is captured or not
        private bool isMouseCaptured;

        // manipulation and inertia processors
        private ManipulationProcessor2D manipulationProcessor;
        private InertiaProcessor2D inertiaProcessor;
        private DispatcherTimer inertiaTimer;
        #endregion

        private bool canRotate = true;

        public bool CanRotate
        {
            get
            {
                return this.canRotate;
            }

            set
            {
                if (this.canRotate != value)
                {
                    this.canRotate = value;
                    RefreshManipulations();
                }
            }
        }

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




        #region mouse handlers
        /// <summary>
        /// Here when the mouse goes down on the item.
        /// </summary>
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            // ignore mouse if there is at least on
            if (!TouchHelper.AreAnyTouches && AssociatedObject.CaptureMouse())
            {
                this.isMouseCaptured = true;
                ProcessMouse(e);
                BringToFront();
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
            List<Manipulator2D> manipulators = new List<Manipulator2D>();
            manipulators.Add(new Manipulator2D(
                 0,
                 (float)(position.X),
                 (float)(position.Y)));

            this.manipulationProcessor.ProcessManipulators(
               Timestamp,
               manipulators);
        }
        #endregion

        #region touch handlers
        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchPoint.TouchDevice.Capture(this.AssociatedObject))
            {
                BringToFront();
            }

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

            if (e.TouchPoints.Count() <= 2)
            {
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
        /// Gets or sets the radius of the object, in pixels.
        /// </summary>
        private double Radius { get; set; }

        /// <summary>
        /// Gets or sets whether the pivot is active.
        /// </summary>
        public bool IsPivotActive
        {
            get
            {
                return this.manipulationProcessor.Pivot != null;
            }
            set
            {
                UpdatePivot(value);
            }
        }


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

        #region handlers for controls


        /// <summary>
        /// Here when the pivot button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPivotClick(object sender, RoutedEventArgs e)
        {
            IsPivotActive = !IsPivotActive;
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

        /// <summary>
        /// Here when manipulation starts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnManipulationStarted(object sender, Manipulation2DStartedEventArgs e)
        {
            StopInertia();
        }

        /// <summary>
        /// Here when manipulation gives a delta.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnManipulationDelta(object sender, Manipulation2DDeltaEventArgs e)
        {
            Move(new Point(e.OriginX, e.OriginY),
                new Vector(e.Delta.TranslationX, e.Delta.TranslationY),
                e.Delta.Rotation,
                e.Delta.ScaleX);
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

            // set angular velocity if rotation flicks are allowed
            if (CanRotate && Math.Abs(angularVelocity) >= MinimumAngularFlickVelocity)
            {
                const float maximumAngularFlickVelocity = MaximumFlickVelocityFactor * MinimumAngularFlickVelocity;
                if (Math.Abs(angularVelocity) > maximumAngularFlickVelocity)
                {
                    angularVelocity = angularVelocity > 0 ? maximumAngularFlickVelocity : -maximumAngularFlickVelocity;
                }
                startFlick = true;
                inertiaProcessor.RotationBehavior.InitialVelocity = angularVelocity;
            }
            else
            {
                inertiaProcessor.RotationBehavior.InitialVelocity = 0.0f;
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
                inertiaProcessor.ExpansionBehavior.InitialRadius = (float)Radius;
            }
            else
            {
                inertiaProcessor.ExpansionBehavior.InitialVelocityX = 0.0f;
                inertiaProcessor.ExpansionBehavior.InitialVelocityY = 0.0f;
                inertiaProcessor.ExpansionBehavior.InitialRadius = 1.0f;
            }

            if (startFlick)
            {
                this.inertiaTimer.Start();
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
        /// Moves the item as specified.
        /// </summary>
        private void Move(
            Point manipulationOrigin,
            Vector deltaTranslation,
            double deltaRotationInRadians,
            double deltaScale)
        {
            // apply rotation and scale constrains if needed,
            // for the scale make sure that the result radius is within Minimum..MaximumRadius interval
            deltaScale = Math.Max(Math.Min(deltaScale, MaximumRadius / Radius), MinimumRadius / Radius);

            // adjust translation
            if (CanTranslate)
            {
                AdjustTranslation(Center, manipulationOrigin, ref deltaTranslation, deltaRotationInRadians, deltaScale);
            }
            else
            {
                deltaTranslation = new Vector();
            }

            // apply container constrains on translation
            UIElement parent = AssociatedObject.Parent as UIElement;
            if (parent != null)
            {
                Point newCenter = deltaTranslation + Center;
                Size parentSize = parent.RenderSize;
                newCenter.X = Math.Max(0, Math.Min(newCenter.X, parentSize.Width));
                newCenter.Y = Math.Max(0, Math.Min(newCenter.Y, parentSize.Height));
                deltaTranslation = Vector.Subtruct(newCenter, Center);
            }

            // position the item
            double deltaRotationInDegrees = deltaRotationInRadians * 180.0 / Math.PI;
            Move(deltaTranslation + Center, deltaRotationInDegrees + Orientation, Radius * deltaScale);
        }

        /// <summary>
        /// Performs the actual move.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="orientation"></param>
        /// <param name="radius"></param>
        public void Move(Point center, double orientation, double radius)
        {
            // change center and manipulation pivot
            Center = center;
            UpdatePivot(IsPivotActive);

            // change orientation
            Orientation = orientation;

            // change item's width and height
            Radius = radius;
            AssociatedObject.Width = 2 * radius;
            AssociatedObject.Height = 2 * radius;

            // calculate transformation matrix
            Matrix matrix = Matrix.Identity;

            // apply rotation
            if (Orientation != 0)
            {
                MatrixHelper.Rotate(ref matrix, Orientation);
            }

            // apply translation,
            // determine the correct offset for the render transform.
            Vector offset = CalculateRenderOffset(Center, Orientation, AssociatedObject.Width, AssociatedObject.Height, AssociatedObject.RenderTransformOrigin);
            MatrixHelper.Translate(ref matrix, offset.X, offset.Y);

            // apply RenderTransform
            AssociatedObject.RenderTransform = new MatrixTransform
            {
                Matrix = matrix
            };
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
        /// Adjusts translation delta due to rotaion and/or scale are done around manipulation origin which can be 
        /// different from the item's Center.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="manipulationOrigin"></param>
        /// <param name="deltaTranslation"></param>
        /// <param name="deltaRotationInRadians"></param>
        /// <param name="deltaScale"></param>
        private static void AdjustTranslation(Point center,
            Point manipulationOrigin,
            ref Vector deltaTranslation,
            double deltaRotationInRadians,
            double deltaScale)
        {
            Vector offsetToCenter = Vector.Subtruct(center, manipulationOrigin) + deltaTranslation;

            // Adjust item position based on change in rotation
            if (deltaRotationInRadians != 0)
            {
                Vector rotatedOffsetToCenter = offsetToCenter;
                rotatedOffsetToCenter.Rotate(deltaRotationInRadians);
                deltaTranslation += rotatedOffsetToCenter - offsetToCenter;
            }

            // Any scaling could cause the layout rect to shift, so adjust the translation accordingly.
            if (deltaScale != 1.0)
            {
                Vector scaledOffsetToCenter = offsetToCenter * deltaScale;
                deltaTranslation += scaledOffsetToCenter - offsetToCenter;
            }
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
                if (CanRotate)
                    manipulation |= Manipulations2D.Rotate;
                return manipulation;
            }
        }
        #endregion

        #region misc
        /// <summary>
        /// Updates ZIndex for all children of the item's parent and brings this item to the front.
        /// </summary>
        private void BringToFront()
        {
            Canvas parent = AssociatedObject.Parent as Canvas;
            if (parent == null)
            {
                return;
            }

            // clone and sort according to the current ZIndex, make sure that "this item" is at the end of the list
            UIElement[] clone = new UIElement[parent.Children.Count];
            parent.Children.CopyTo(clone, 0);
            Array.Sort(clone, delegate(UIElement e1, UIElement e2)
            {
                if (object.ReferenceEquals(e1, this))
                {
                    return int.MaxValue;
                }

                if (object.ReferenceEquals(this, e2))
                {
                    return int.MinValue;
                }

                return Canvas.GetZIndex(e1) - Canvas.GetZIndex(e2);
            });

            // update ZIndex for all children
            for (int i = 0; i < clone.Length; i++)
            {
                Canvas.SetZIndex(clone[i], i);
            }
        }

        /// <summary>
        /// Updates pivot positions.
        /// </summary>
        /// <param name="isPivotActive"></param>
        private void UpdatePivot(bool isPivotActive)
        {
            if (this.manipulationProcessor != null)
            {
                this.manipulationProcessor.Pivot = isPivotActive ?
                    new ManipulationPivot2D { X = (float)Center.X, Y = (float)Center.Y, Radius = (float)Radius } :
                    null;
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
            this.inertiaProcessor.RotationBehavior.DesiredDeceleration = AngularDeceleration;
            this.inertiaProcessor.ExpansionBehavior.DesiredDeceleration = ExpansionDeceleration;
            this.inertiaProcessor.Delta += OnManipulationDelta;
            this.inertiaProcessor.Completed += OnInertiaCompleted;

            this.inertiaTimer = new DispatcherTimer();
            this.inertiaTimer.Interval = TimeSpan.FromMilliseconds(30);
            this.inertiaTimer.Tick += OnTimerTick;

            this.AssociatedObject.RenderTransformOrigin = new Point(0.5, 0.5);

            Move(new Point(0, 0), 0, 100);
            IsPivotActive = true;

            this.AssociatedObject.MouseLeftButtonUp += OnMouseUp;
            this.AssociatedObject.MouseLeftButtonDown += OnMouseDown;
            this.AssociatedObject.MouseMove += OnMouseMove;
            this.AssociatedObject.LostMouseCapture += OnLostMouseCapture;

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
