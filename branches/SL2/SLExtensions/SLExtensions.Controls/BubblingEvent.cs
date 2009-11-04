// <copyright file="BubblingEvent.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    #region Delegates

    /// <summary>
    /// BubblingEventHandler delegate
    /// </summary>
    /// <param name="sender">the sender</param>
    /// <param name="args">the args to pass.</param>
    /// <typeparam name="T">a <see cref="BubblingEventArgs"/> type</typeparam>
    public delegate void BubblingEventHandler<T>(object sender, T args)
        where T : BubblingEventArgs;

    #endregion Delegates

    /// <summary>
    /// Half-implemented extensible routed event system.
    /// Declare a routed event with the syntax such as:
    /// <example>
    ///		public static readonly BubblingEvent&lt;ContextMenuEventArgs&gt; ContextMenuEvent = new BubblingEvent&lt;ContextMenuEventArgs&gt;("ContextMenuEventArgs", RoutingStrategy.Bubble);
    ///	</example>
    ///	
    /// Register a type handler for the event:
    /// <example>
    ///		static Page() {
    ///			ContextMenuGenerator.ContextMenuEvent.RegisterClassHandler(typeof(Page), Page.HandleContextMenuEvent, false);
    ///		}
    /// </example>
    /// </summary>
    /// <typeparam name="T">a <see cref="BubblingEventArgs"/> type</typeparam>
    public class BubblingEvent<T>
        where T : BubblingEventArgs
    {
        #region Fields

        /// <summary>
        /// dictionary holding registered types
        /// </summary>
        private Dictionary<Type, BubblingEventRegistration<T>> registeredTypes = new Dictionary<Type, BubblingEventRegistration<T>>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BubblingEvent&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="name">The name .</param>
        /// <param name="routingStrategy">The routing strategy.</param>
        public BubblingEvent(string name, RoutingStrategy routingStrategy)
        {
            this.Name = name;
            this.RoutingStrategy = routingStrategy;
        }

        #endregion Constructors

        #region Delegates

        public delegate bool CanRaise(FrameworkElement element);

        #endregion Delegates

        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name value.</value>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the routing strategy.
        /// </summary>
        /// <value>The routing strategy.</value>
        public RoutingStrategy RoutingStrategy
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Raises the event.
        /// </summary>
        /// <param name="evt">The event.</param>
        /// <param name="element">The element.</param>
        public void RaiseEvent(T evt, FrameworkElement element, CanRaise canRaiseDlg)
        {
            switch (this.RoutingStrategy)
            {
                case RoutingStrategy.Bubble:
                    this.RaiseBubblingEvent(evt, element, canRaiseDlg);
                    break;
                case RoutingStrategy.Tunnel:
                    this.RaiseTunnelingEvent(evt, element, canRaiseDlg);
                    break;
                case RoutingStrategy.Direct:
                    this.RaiseDirectEvent(evt, element, canRaiseDlg);
                    break;
            }
        }

        /// <summary>
        /// Registers the class handler.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="handler">The handler.</param>
        public void RegisterClassHandler(Type classType, EventHandler<T> handler)
        {
            this.RegisterClassHandler(classType, handler, false);
        }

        /// <summary>
        /// Registers the class handler.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="handledEventsToo">if set to <c>true</c> [handled events too].</param>
        public void RegisterClassHandler(Type classType, EventHandler<T> handler, bool handledEventsToo)
        {
            if (!this.registeredTypes.ContainsKey(classType))
            {
                this.registeredTypes[classType] = new BubblingEventRegistration<T>(classType, handler, handledEventsToo);
            }
        }

        /// <summary>
        /// Gets the delegates.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="walkTree">if set to <c>true</c> follow the control tree.</param>
        /// <returns>the list of BubblingEventDelegate</returns>
        private IList<BubblingEventDelegate<T>> GetDelegates(FrameworkElement element, bool walkTree, CanRaise canRaise)
        {
            List<BubblingEventDelegate<T>> delegates = new List<BubblingEventDelegate<T>>();

            while (element != null)
            {
                Type classType = element.GetType();
                while (classType != typeof(object) && classType != null)
                {
                    BubblingEventRegistration<T> registration;
                    if (this.registeredTypes.TryGetValue(classType, out registration))
                    {
            if((canRaise != null && canRaise(element)) || canRaise == null )
                        delegates.Add(new BubblingEventDelegate<T>(element, registration));
                    }

                    classType = classType.BaseType;
                }

                element = System.Windows.Media.VisualTreeHelper.GetParent(element) as FrameworkElement;

                if (!walkTree)
                {
                    break;
                }
            }

            return delegates;
        }

        /// <summary>
        /// Raises the bubbling event.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="element">The element.</param>
        private void RaiseBubblingEvent(T args, FrameworkElement element, CanRaise canRaise)
        {
            IList<BubblingEventDelegate<T>> delegates = this.GetDelegates(element, true, canRaise);

            for (int i = 0; i < delegates.Count; ++i)
            {
                delegates[i].Invoke(args);
            }
        }

        /// <summary>
        /// Raises the direct event.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="element">The element.</param>
        private void RaiseDirectEvent(T args, FrameworkElement element, CanRaise canRaise)
        {
            IList<BubblingEventDelegate<T>> delegates = this.GetDelegates(element, false, canRaise);

            for (int i = delegates.Count - 1; i >= 0; --i)
            {
                delegates[i].Invoke(args);
            }
        }

        /// <summary>
        /// Raises the tunneling event.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="element">The element.</param>
        private void RaiseTunnelingEvent(T args, FrameworkElement element, CanRaise canRaise)
        {
            IList<BubblingEventDelegate<T>> delegates = this.GetDelegates(element, true, canRaise);

            for (int i = delegates.Count - 1; i >= 0; --i)
            {
                delegates[i].Invoke(args);
            }
        }

        #endregion Methods
    }
}