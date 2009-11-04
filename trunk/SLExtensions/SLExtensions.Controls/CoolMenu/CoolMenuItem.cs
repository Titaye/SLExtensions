using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace SLExtensions.Controls
{
    /// <summary>
    /// Represents an indivdual menu item in CoolMenu.
    /// </summary>
    public class CoolMenuItem : ContentControl
    {
        internal ItemsControl ParentItemsControl { get; set; }

        /// <summary>
        /// Initializes a new instance of a CoolMenuItem.
        /// </summary>
        public CoolMenuItem()
        {
            this.DefaultStyleKey = typeof(CoolMenuItem);

            this.MouseEnter += CoolMenuItem_MouseEnter;
            this.MouseLeave += CoolMenuItem_MouseLeave;
            this.MouseLeftButtonDown += CoolMenuItem_MouseLeftButtonDown;
            this.MouseLeftButtonUp += CoolMenuItem_MouseLeftButtonUp;
        }

        void CoolMenuItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CoolMenu cm = this.ParentItemsControl as CoolMenu;
            int index = CoolMenu.GetGenerator(cm).IndexFromContainer(this);
            cm.OnItemMouseUp(index);
        }

        void CoolMenuItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CoolMenu cm = this.ParentItemsControl as CoolMenu;
            int index = CoolMenu.GetGenerator(cm).IndexFromContainer(this);
            cm.OnItemMouseDown(index);
        }

        void CoolMenuItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CoolMenu cm = this.ParentItemsControl as CoolMenu;
            int index = CoolMenu.GetGenerator(cm).IndexFromContainer(this);
            cm.OnMouseEnter(index);
        }

        void CoolMenuItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CoolMenu cm = this.ParentItemsControl as CoolMenu;
            int index = CoolMenu.GetGenerator(cm).IndexFromContainer(this);
            cm.OnItemMouseLeave(index);
        }
    }
}
