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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using sle = SLExtensions.Controls;
using Microsoft.Silverlight.Testing.UnitTesting.UI;
using Microsoft.Silverlight.Testing;
namespace SLExtensions.Test
{
    [TestClass]
    public class SeparatorTest : SilverlightTest
    {

        [TestMethod]
        [Asynchronous]
        public void TestHorizontalStackPanel()
        {
            var container = new System.Windows.Controls.StackPanel { Orientation = Orientation.Horizontal };
            var testee = new sle.Separator();
            bool isLoaded = false;
            testee.Loaded += (sender, args) => isLoaded = true;
            EnqueueCallback(() => container.Children.Add(testee));
            EnqueueCallback(() => this.Silverlight.TestSurface.Children.Add(container));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => Assert.AreEqual(Orientation.Horizontal, testee.Orientation));
            EnqueueTestComplete();          
        }
        [TestMethod]
        [Asynchronous]
        public void TestVerticalStackPanel()
        {
            bool isLoaded = false;
            var container = new System.Windows.Controls.StackPanel { Orientation = Orientation.Vertical };
            var testee = new sle.Separator();
            testee.Loaded += (sender, args) => isLoaded = true;
            EnqueueCallback(() => container.Children.Add(testee));
            EnqueueCallback(() => this.Silverlight.TestSurface.Children.Add(container));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => Assert.AreEqual(Orientation.Vertical, testee.Orientation));
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void TestLeftGrid()
        {
            bool isLoaded = false;
            var container = new System.Windows.Controls.Grid ();
            var testee = new sle.Separator{ HorizontalAlignment = HorizontalAlignment.Left};
            testee.Loaded += (sender, args) => isLoaded = true;
            EnqueueCallback(() => container.Children.Add(testee));
            EnqueueCallback(() => this.Silverlight.TestSurface.Children.Add(container));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => Assert.AreEqual(Orientation.Horizontal, testee.Orientation));
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void TestRightGrid()
        {
            bool isLoaded = false;
            var container = new System.Windows.Controls.Grid ();
            var testee = new sle.Separator{ HorizontalAlignment = HorizontalAlignment.Right};
            testee.Loaded += (sender, args) => isLoaded = true;
            EnqueueCallback(() => container.Children.Add(testee));
            EnqueueCallback(() => this.Silverlight.TestSurface.Children.Add(container));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => Assert.AreEqual(Orientation.Horizontal, testee.Orientation));
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void TestBottomGrid()
        {
            bool isLoaded = false;
            var container = new System.Windows.Controls.Grid ();
            var testee = new sle.Separator{VerticalAlignment = VerticalAlignment.Bottom};
            testee.Loaded += (sender, args) => isLoaded = true;
            EnqueueCallback(() => container.Children.Add(testee));
            EnqueueCallback(() => this.Silverlight.TestSurface.Children.Add(container));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => Assert.AreEqual(Orientation.Vertical, testee.Orientation));
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void TestTopGrid()
        {
            bool isLoaded = false;
            var container = new System.Windows.Controls.Grid ();
            var testee = new sle.Separator{VerticalAlignment = VerticalAlignment.Top};
            testee.Loaded += (sender, args) => isLoaded = true;
            EnqueueCallback(() => container.Children.Add(testee));
            EnqueueCallback(() => this.Silverlight.TestSurface.Children.Add(container));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => Assert.AreEqual(Orientation.Vertical, testee.Orientation));
            EnqueueTestComplete();
        }

        // These two test fail because of a Layout Bug in SLE StackPanel. This should be investigated.

        //[TestMethod]
        //[Asynchronous]
        //public void TestHorizontalSLEStackPanel()
        //{
        //    var container = new sle.StackPanel { Orientation = Orientation.Horizontal,Interval=0 };
        //    var testee = new sle.Separator();
        //    bool isLoaded = false;
        //    testee.Loaded += (sender, args) => isLoaded = true;
        //    EnqueueCallback(() => container.Children.Add(testee));
        //    EnqueueCallback(() => this.Silverlight.TestSurface.Children.Add(container));
        //    EnqueueConditional(() => isLoaded);
        //    EnqueueCallback(() => Assert.AreEqual(Orientation.Horizontal, testee.Orientation));
        //    EnqueueTestComplete();
        //}
        //[TestMethod]
        //[Asynchronous]
        //public void TestVerticalSLEStackPanel()
        //{
        //    bool isLoaded = false;
        //    var container = new sle.StackPanel { Orientation = Orientation.Vertical, Interval = 0 };
        //    var testee = new sle.Separator();
        //    testee.Loaded += (sender, args) => isLoaded = true;
        //    EnqueueCallback(() => container.Children.Add(testee));
        //    EnqueueCallback(() => this.Silverlight.TestSurface.Children.Add(container));
        //    EnqueueConditional(() => isLoaded);
        //    EnqueueCallback(() => Assert.AreEqual(Orientation.Vertical, testee.Orientation));
        //    EnqueueTestComplete();
        

    }
}
