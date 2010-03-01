#region Header

// ---
// For details and tutorials: http://code.msdn.microsoft.com/silverlightut/

#endregion Header

namespace SLMedia.Core.Test
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MediaControllerTest
    {
        #region Methods

        [TestMethod]
        public void TestMarkerActive()
        {
            MarkerSelector selector = new MarkerSelector();
            selector.Markers.Add(new Marker { Position = TimeSpan.FromSeconds(1) });
            selector.Markers.Add(new Marker { Position = TimeSpan.FromSeconds(2) });
            selector.Markers.Add(new Marker { Position = TimeSpan.FromSeconds(3) });

            var list = new List<IMarkerSelector>();
            Dictionary<IMarkerSelector, int> cache = new Dictionary<IMarkerSelector, int>();

            MediaController.SetActiveMarkerForSelector(list, selector, TimeSpan.FromSeconds(0.5), cache);
            Assert.IsNull(selector.ActiveMarker);
            MediaController.SetActiveMarkerForSelector(list, selector, TimeSpan.FromSeconds(1), cache);
            Assert.AreEqual(selector.Markers[0], selector.ActiveMarker);
            MediaController.SetActiveMarkerForSelector(list, selector, TimeSpan.FromSeconds(1.5), cache);
            Assert.AreEqual(selector.Markers[0], selector.ActiveMarker);
            MediaController.SetActiveMarkerForSelector(list, selector, TimeSpan.FromSeconds(3.5), cache);
            Assert.AreEqual(selector.Markers[2], selector.ActiveMarker);

            cache[selector] = 0;
            list.Add(selector);
            MediaController.SetActiveMarkerForSelector(list, selector, TimeSpan.FromSeconds(0.5), cache);
            Assert.IsNull(selector.ActiveMarker);
            list.Add(selector);
            MediaController.SetActiveMarkerForSelector(list, selector, TimeSpan.FromSeconds(1), cache);
            Assert.AreEqual(selector.Markers[0], selector.ActiveMarker);
            list.Add(selector);
            MediaController.SetActiveMarkerForSelector(list, selector, TimeSpan.FromSeconds(1.5), cache);
            Assert.AreEqual(selector.Markers[0], selector.ActiveMarker);
            list.Add(selector);
            MediaController.SetActiveMarkerForSelector(list, selector, TimeSpan.FromSeconds(3.5), cache);
            Assert.AreEqual(selector.Markers[2], selector.ActiveMarker);

            cache[selector] = 3;
            list.Add(selector);
            MediaController.SetActiveMarkerForSelector(list, selector, TimeSpan.FromSeconds(0.5), cache);
            Assert.IsNull(selector.ActiveMarker);
            list.Add(selector);
            MediaController.SetActiveMarkerForSelector(list, selector, TimeSpan.FromSeconds(1), cache);
            Assert.AreEqual(selector.Markers[0], selector.ActiveMarker);
            list.Add(selector);
            MediaController.SetActiveMarkerForSelector(list, selector, TimeSpan.FromSeconds(1.5), cache);
            Assert.AreEqual(selector.Markers[0], selector.ActiveMarker);
            list.Add(selector);
            MediaController.SetActiveMarkerForSelector(list, selector, TimeSpan.FromSeconds(3.5), cache);
            Assert.AreEqual(selector.Markers[2], selector.ActiveMarker);
        }

        [TestMethod]
        public void TestMarkerCompare()
        {
            List<IMarker> markers = new List<IMarker>();
            markers.Add(new Marker { Position = TimeSpan.FromSeconds(1) });
            markers.Add(new Marker { Position = TimeSpan.FromSeconds(2) });
            markers.Add(new Marker { Position = TimeSpan.FromSeconds(3) });

            Assert.AreEqual(1, MediaController.CompareMarkerPosition(markers, 0, TimeSpan.FromSeconds(0.5)));
            Assert.AreEqual(0, MediaController.CompareMarkerPosition(markers, 0, TimeSpan.FromSeconds(1)));
            Assert.AreEqual(0, MediaController.CompareMarkerPosition(markers, 0, TimeSpan.FromSeconds(1.5)));
            Assert.AreEqual(1, MediaController.CompareMarkerPosition(markers, 1, TimeSpan.FromSeconds(1.5)));
            Assert.AreEqual(-1, MediaController.CompareMarkerPosition(markers, 0, TimeSpan.FromSeconds(2.5)));
            Assert.AreEqual(0, MediaController.CompareMarkerPosition(markers, 2, TimeSpan.FromSeconds(3.5)));
        }

        #endregion Methods
    }
}