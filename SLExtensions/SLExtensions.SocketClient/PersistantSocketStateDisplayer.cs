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

namespace SLExtensions.SocketClient
{

    [TemplateVisualState(GroupName = "ConnectionState", Name = "Connected")]
    [TemplateVisualState(GroupName = "ConnectionState", Name = "Connecting")]
    [TemplateVisualState(GroupName = "ConnectionState", Name = "Disconnected")]
    public class PersistantSocketStateDisplayer : Control
    {
        public PersistantSocketStateDisplayer()
        {
            DefaultStyleKey = typeof(PersistantSocketStateDisplayer);
        }


        /// <summary>
        /// PersistantSocketController to display
        /// </summary>
        public PersistantSocketController PersistantSocketController
        {
            get { return (PersistantSocketController)GetValue(PersistantSocketProperty); }
            set { SetValue(PersistantSocketProperty, value); }
        }

        public static readonly DependencyProperty PersistantSocketProperty =
            DependencyProperty.Register("PersistantSocket", typeof(PersistantSocketController), typeof(PersistantSocketStateDisplayer), new PropertyMetadata(
                delegate(DependencyObject sender, DependencyPropertyChangedEventArgs args)
                {
                    var asDisplayer = sender as PersistantSocketStateDisplayer;
                    asDisplayer.OnPersistantSocketChanged(args.OldValue as PersistantSocketController, args.NewValue as PersistantSocketController);
                }));

        protected virtual void OnPersistantSocketChanged(PersistantSocketController oldVal, PersistantSocketController newVal)
        {
            if (oldVal != null)
                oldVal.PropertyChanged -= _psock_PropertyChanged;

            if (newVal != null)
                newVal.PropertyChanged += _psock_PropertyChanged;

            ChangeState(false);
        }

        /// <summary>
        /// update the visualState
        /// </summary>
        private void ChangeState(bool trans)
        {
            if (PersistantSocketController == null)
            {
                VisualStateManager.GoToState(this, "Disconnected", trans);
                ToolTipService.SetToolTip(this, "Déconnecté");
            }

            else
            {
                string state = "Disconnected";
                switch (PersistantSocketController.State)
                {
                    case PersistantSocketState.Connecting:
                        state = "Connecting";
                        break;
                    case PersistantSocketState.Connected:
                        state = "Connected";
                        break;
                }
                VisualStateManager.GoToState(this, state, trans);
            }
        }
        void _psock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ChangeState(true);
        }
    }
}
