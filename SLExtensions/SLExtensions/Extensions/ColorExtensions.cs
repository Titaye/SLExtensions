namespace SLExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public static class ColorExtensions
    {
        #region Fields

        private static readonly Regex ColorRegex = new Regex("#?(?<a>[0-9a-f]{2})?(?<r>[0-9a-f]{2})(?<g>[0-9a-f]{2})(?<b>[0-9a-f]{2})", RegexOptions.IgnoreCase);

        private static Color? _AliceBlue;
        private static Color? _AntiqueWhite;
        private static Color? _Aqua;
        private static Color? _Aquamarine;
        private static Color? _Azure;
        private static Color? _Beige;
        private static Color? _Bisque;
        private static Color? _Black;
        private static Color? _BlanchedAlmond;
        private static Color? _Blue;
        private static Color? _BlueViolet;
        private static Color? _Brown;
        private static Color? _BurlyWood;
        private static Color? _CadetBlue;
        private static Color? _Chartreuse;
        private static Color? _Chocolate;
        private static Color? _Coral;
        private static Color? _CornflowerBlue;
        private static Color? _Cornsilk;
        private static Color? _Crimson;
        private static Color? _Cyan;
        private static Color? _DarkBlue;
        private static Color? _DarkCyan;
        private static Color? _DarkGoldenRod;
        private static Color? _DarkGray;
        private static Color? _DarkGreen;
        private static Color? _DarkKhaki;
        private static Color? _DarkMagenta;
        private static Color? _DarkOliveGreen;
        private static Color? _DarkOrchid;
        private static Color? _DarkRed;
        private static Color? _DarkSalmon;
        private static Color? _DarkSeaGreen;
        private static Color? _DarkSlateBlue;
        private static Color? _DarkSlateGray;
        private static Color? _DarkTurquoise;
        private static Color? _DarkViolet;
        private static Color? _Darkorange;
        private static Color? _DeepPink;
        private static Color? _DeepSkyBlue;
        private static Color? _DimGray;
        private static Color? _DodgerBlue;
        private static Color? _FireBrick;
        private static Color? _FloralWhite;
        private static Color? _ForestGreen;
        private static Color? _Fuchsia;
        private static Color? _Gainsboro;
        private static Color? _GhostWhite;
        private static Color? _Gold;
        private static Color? _GoldenRod;
        private static Color? _Gray;
        private static Color? _Green;
        private static Color? _GreenYellow;
        private static Color? _HoneyDew;
        private static Color? _HotPink;
        private static Color? _IndianRed;
        private static Color? _Indigo;
        private static Color? _Ivory;
        private static Color? _Khaki;
        private static Color? _Lavender;
        private static Color? _LavenderBlush;
        private static Color? _LawnGreen;
        private static Color? _LemonChiffon;
        private static Color? _LightBlue;
        private static Color? _LightCoral;
        private static Color? _LightCyan;
        private static Color? _LightGoldenRodYellow;
        private static Color? _LightGreen;
        private static Color? _LightGrey;
        private static Color? _LightPink;
        private static Color? _LightSalmon;
        private static Color? _LightSeaGreen;
        private static Color? _LightSkyBlue;
        private static Color? _LightSlateGray;
        private static Color? _LightSteelBlue;
        private static Color? _LightYellow;
        private static Color? _Lime;
        private static Color? _LimeGreen;
        private static Color? _Linen;
        private static Color? _Magenta;
        private static Color? _Maroon;
        private static Color? _MediumAquaMarine;
        private static Color? _MediumBlue;
        private static Color? _MediumOrchid;
        private static Color? _MediumPurple;
        private static Color? _MediumSeaGreen;
        private static Color? _MediumSlateBlue;
        private static Color? _MediumSpringGreen;
        private static Color? _MediumTurquoise;
        private static Color? _MediumVioletRed;
        private static Color? _MidnightBlue;
        private static Color? _MintCream;
        private static Color? _MistyRose;
        private static Color? _Moccasin;
        private static Color? _NavajoWhite;
        private static Color? _Navy;
        private static Color? _OldLace;
        private static Color? _Olive;
        private static Color? _OliveDrab;
        private static Color? _Orange;
        private static Color? _OrangeRed;
        private static Color? _Orchid;
        private static Color? _PaleGoldenRod;
        private static Color? _PaleGreen;
        private static Color? _PaleTurquoise;
        private static Color? _PaleVioletRed;
        private static Color? _PapayaWhip;
        private static Color? _PeachPuff;
        private static Color? _Peru;
        private static Color? _Pink;
        private static Color? _Plum;
        private static Color? _PowderBlue;
        private static Color? _Purple;
        private static Color? _Red;
        private static Color? _RosyBrown;
        private static Color? _RoyalBlue;
        private static Color? _SaddleBrown;
        private static Color? _Salmon;
        private static Color? _SandyBrown;
        private static Color? _SeaGreen;
        private static Color? _SeaShell;
        private static Color? _Sienna;
        private static Color? _Silver;
        private static Color? _SkyBlue;
        private static Color? _SlateBlue;
        private static Color? _SlateGray;
        private static Color? _Snow;
        private static Color? _SpringGreen;
        private static Color? _SteelBlue;
        private static Color? _Tan;
        private static Color? _Teal;
        private static Color? _Thistle;
        private static Color? _Tomato;
        private static Color? _Turquoise;
        private static Color? _Violet;
        private static Color? _Wheat;
        private static Color? _White;
        private static Color? _WhiteSmoke;
        private static Color? _Yellow;
        private static Color? _YellowGreen;
        private static Dictionary<string, Color> colorCache = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase);

        #endregion Fields

        #region Properties

        public static Color AliceBlue
        {
            get
            {
                if (!_AliceBlue.HasValue)
                    _AliceBlue = GetColor("#F0F8FF");
                return _AliceBlue.Value;
            }
        }

        public static Color AntiqueWhite
        {
            get
            {
                if (!_AntiqueWhite.HasValue)
                    _AntiqueWhite = GetColor("#FAEBD7");
                return _AntiqueWhite.Value;
            }
        }

        public static Color Aqua
        {
            get
            {
                if (!_Aqua.HasValue)
                    _Aqua = GetColor("#00FFFF");
                return _Aqua.Value;
            }
        }

        public static Color Aquamarine
        {
            get
            {
                if (!_Aquamarine.HasValue)
                    _Aquamarine = GetColor("#7FFFD4");
                return _Aquamarine.Value;
            }
        }

        public static Color Azure
        {
            get
            {
                if (!_Azure.HasValue)
                    _Azure = GetColor("#F0FFFF");
                return _Azure.Value;
            }
        }

        public static Color Beige
        {
            get
            {
                if (!_Beige.HasValue)
                    _Beige = GetColor("#F5F5DC");
                return _Beige.Value;
            }
        }

        public static Color Bisque
        {
            get
            {
                if (!_Bisque.HasValue)
                    _Bisque = GetColor("#FFE4C4");
                return _Bisque.Value;
            }
        }

        public static Color Black
        {
            get
            {
                if (!_Black.HasValue)
                    _Black = GetColor("#000000");
                return _Black.Value;
            }
        }

        public static Color BlanchedAlmond
        {
            get
            {
                if (!_BlanchedAlmond.HasValue)
                    _BlanchedAlmond = GetColor("#FFEBCD");
                return _BlanchedAlmond.Value;
            }
        }

        public static Color Blue
        {
            get
            {
                if (!_Blue.HasValue)
                    _Blue = GetColor("#0000FF");
                return _Blue.Value;
            }
        }

        public static Color BlueViolet
        {
            get
            {
                if (!_BlueViolet.HasValue)
                    _BlueViolet = GetColor("#8A2BE2");
                return _BlueViolet.Value;
            }
        }

        public static Color Brown
        {
            get
            {
                if (!_Brown.HasValue)
                    _Brown = GetColor("#A52A2A");
                return _Brown.Value;
            }
        }

        public static Color BurlyWood
        {
            get
            {
                if (!_BurlyWood.HasValue)
                    _BurlyWood = GetColor("#DEB887");
                return _BurlyWood.Value;
            }
        }

        public static Color CadetBlue
        {
            get
            {
                if (!_CadetBlue.HasValue)
                    _CadetBlue = GetColor("#5F9EA0");
                return _CadetBlue.Value;
            }
        }

        public static Color Chartreuse
        {
            get
            {
                if (!_Chartreuse.HasValue)
                    _Chartreuse = GetColor("#7FFF00");
                return _Chartreuse.Value;
            }
        }

        public static Color Chocolate
        {
            get
            {
                if (!_Chocolate.HasValue)
                    _Chocolate = GetColor("#D2691E");
                return _Chocolate.Value;
            }
        }

        public static Color Coral
        {
            get
            {
                if (!_Coral.HasValue)
                    _Coral = GetColor("#FF7F50");
                return _Coral.Value;
            }
        }

        public static Color CornflowerBlue
        {
            get
            {
                if (!_CornflowerBlue.HasValue)
                    _CornflowerBlue = GetColor("#6495ED");
                return _CornflowerBlue.Value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cornsilk")]
        public static Color Cornsilk
        {
            get
            {
                if (!_Cornsilk.HasValue)
                    _Cornsilk = GetColor("#FFF8DC");
                return _Cornsilk.Value;
            }
        }

        public static Color Crimson
        {
            get
            {
                if (!_Crimson.HasValue)
                    _Crimson = GetColor("#DC143C");
                return _Crimson.Value;
            }
        }

        public static Color Cyan
        {
            get
            {
                if (!_Cyan.HasValue)
                    _Cyan = GetColor("#00FFFF");
                return _Cyan.Value;
            }
        }

        public static Color DarkBlue
        {
            get
            {
                if (!_DarkBlue.HasValue)
                    _DarkBlue = GetColor("#00008B");
                return _DarkBlue.Value;
            }
        }

        public static Color DarkCyan
        {
            get
            {
                if (!_DarkCyan.HasValue)
                    _DarkCyan = GetColor("#008B8B");
                return _DarkCyan.Value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GoldenRod")]
        public static Color DarkGoldenRod
        {
            get
            {
                if (!_DarkGoldenRod.HasValue)
                    _DarkGoldenRod = GetColor("#B8860B");
                return _DarkGoldenRod.Value;
            }
        }

        public static Color DarkGray
        {
            get
            {
                if (!_DarkGray.HasValue)
                    _DarkGray = GetColor("#A9A9A9");
                return _DarkGray.Value;
            }
        }

        public static Color DarkGreen
        {
            get
            {
                if (!_DarkGreen.HasValue)
                    _DarkGreen = GetColor("#006400");
                return _DarkGreen.Value;
            }
        }

        public static Color DarkKhaki
        {
            get
            {
                if (!_DarkKhaki.HasValue)
                    _DarkKhaki = GetColor("#BDB76B");
                return _DarkKhaki.Value;
            }
        }

        public static Color DarkMagenta
        {
            get
            {
                if (!_DarkMagenta.HasValue)
                    _DarkMagenta = GetColor("#8B008B");
                return _DarkMagenta.Value;
            }
        }

        public static Color DarkOliveGreen
        {
            get
            {
                if (!_DarkOliveGreen.HasValue)
                    _DarkOliveGreen = GetColor("#556B2F");
                return _DarkOliveGreen.Value;
            }
        }

        public static Color DarkOrchid
        {
            get
            {
                if (!_DarkOrchid.HasValue)
                    _DarkOrchid = GetColor("#9932CC");
                return _DarkOrchid.Value;
            }
        }

        public static Color DarkRed
        {
            get
            {
                if (!_DarkRed.HasValue)
                    _DarkRed = GetColor("#8B0000");
                return _DarkRed.Value;
            }
        }

        public static Color DarkSalmon
        {
            get
            {
                if (!_DarkSalmon.HasValue)
                    _DarkSalmon = GetColor("#E9967A");
                return _DarkSalmon.Value;
            }
        }

        public static Color DarkSeaGreen
        {
            get
            {
                if (!_DarkSeaGreen.HasValue)
                    _DarkSeaGreen = GetColor("#8FBC8F");
                return _DarkSeaGreen.Value;
            }
        }

        public static Color DarkSlateBlue
        {
            get
            {
                if (!_DarkSlateBlue.HasValue)
                    _DarkSlateBlue = GetColor("#483D8B");
                return _DarkSlateBlue.Value;
            }
        }

        public static Color DarkSlateGray
        {
            get
            {
                if (!_DarkSlateGray.HasValue)
                    _DarkSlateGray = GetColor("#2F4F4F");
                return _DarkSlateGray.Value;
            }
        }

        public static Color DarkTurquoise
        {
            get
            {
                if (!_DarkTurquoise.HasValue)
                    _DarkTurquoise = GetColor("#00CED1");
                return _DarkTurquoise.Value;
            }
        }

        public static Color DarkViolet
        {
            get
            {
                if (!_DarkViolet.HasValue)
                    _DarkViolet = GetColor("#9400D3");
                return _DarkViolet.Value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Darkorange")]
        public static Color Darkorange
        {
            get
            {
                if (!_Darkorange.HasValue)
                    _Darkorange = GetColor("#FF8C00");
                return _Darkorange.Value;
            }
        }

        public static Color DeepPink
        {
            get
            {
                if (!_DeepPink.HasValue)
                    _DeepPink = GetColor("#FF1493");
                return _DeepPink.Value;
            }
        }

        public static Color DeepSkyBlue
        {
            get
            {
                if (!_DeepSkyBlue.HasValue)
                    _DeepSkyBlue = GetColor("#00BFFF");
                return _DeepSkyBlue.Value;
            }
        }

        public static Color DimGray
        {
            get
            {
                if (!_DimGray.HasValue)
                    _DimGray = GetColor("#696969");
                return _DimGray.Value;
            }
        }

        public static Color DodgerBlue
        {
            get
            {
                if (!_DodgerBlue.HasValue)
                    _DodgerBlue = GetColor("#1E90FF");
                return _DodgerBlue.Value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "FireBrick")]
        public static Color FireBrick
        {
            get
            {
                if (!_FireBrick.HasValue)
                    _FireBrick = GetColor("#B22222");
                return _FireBrick.Value;
            }
        }

        public static Color FloralWhite
        {
            get
            {
                if (!_FloralWhite.HasValue)
                    _FloralWhite = GetColor("#FFFAF0");
                return _FloralWhite.Value;
            }
        }

        public static Color ForestGreen
        {
            get
            {
                if (!_ForestGreen.HasValue)
                    _ForestGreen = GetColor("#228B22");
                return _ForestGreen.Value;
            }
        }

        public static Color Fuchsia
        {
            get
            {
                if (!_Fuchsia.HasValue)
                    _Fuchsia = GetColor("#FF00FF");
                return _Fuchsia.Value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gainsboro")]
        public static Color Gainsboro
        {
            get
            {
                if (!_Gainsboro.HasValue)
                    _Gainsboro = GetColor("#DCDCDC");
                return _Gainsboro.Value;
            }
        }

        public static Color GhostWhite
        {
            get
            {
                if (!_GhostWhite.HasValue)
                    _GhostWhite = GetColor("#F8F8FF");
                return _GhostWhite.Value;
            }
        }

        public static Color Gold
        {
            get
            {
                if (!_Gold.HasValue)
                    _Gold = GetColor("#FFD700");
                return _Gold.Value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GoldenRod")]
        public static Color GoldenRod
        {
            get
            {
                if (!_GoldenRod.HasValue)
                    _GoldenRod = GetColor("#DAA520");
                return _GoldenRod.Value;
            }
        }

        public static Color Gray
        {
            get
            {
                if (!_Gray.HasValue)
                    _Gray = GetColor("#808080");
                return _Gray.Value;
            }
        }

        public static Color Green
        {
            get
            {
                if (!_Green.HasValue)
                    _Green = GetColor("#008000");
                return _Green.Value;
            }
        }

        public static Color GreenYellow
        {
            get
            {
                if (!_GreenYellow.HasValue)
                    _GreenYellow = GetColor("#ADFF2F");
                return _GreenYellow.Value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "HoneyDew")]
        public static Color HoneyDew
        {
            get
            {
                if (!_HoneyDew.HasValue)
                    _HoneyDew = GetColor("#F0FFF0");
                return _HoneyDew.Value;
            }
        }

        public static Color HotPink
        {
            get
            {
                if (!_HotPink.HasValue)
                    _HotPink = GetColor("#FF69B4");
                return _HotPink.Value;
            }
        }

        public static Color IndianRed
        {
            get
            {
                if (!_IndianRed.HasValue)
                    _IndianRed = GetColor("#CD5C5C");
                return _IndianRed.Value;
            }
        }

        public static Color Indigo
        {
            get
            {
                if (!_Indigo.HasValue)
                    _Indigo = GetColor("#4B0082");
                return _Indigo.Value;
            }
        }

        public static Color Ivory
        {
            get
            {
                if (!_Ivory.HasValue)
                    _Ivory = GetColor("#FFFFF0");
                return _Ivory.Value;
            }
        }

        public static Color Khaki
        {
            get
            {
                if (!_Khaki.HasValue)
                    _Khaki = GetColor("#F0E68C");
                return _Khaki.Value;
            }
        }

        public static Color Lavender
        {
            get
            {
                if (!_Lavender.HasValue)
                    _Lavender = GetColor("#E6E6FA");
                return _Lavender.Value;
            }
        }

        public static Color LavenderBlush
        {
            get
            {
                if (!_LavenderBlush.HasValue)
                    _LavenderBlush = GetColor("#FFF0F5");
                return _LavenderBlush.Value;
            }
        }

        public static Color LawnGreen
        {
            get
            {
                if (!_LawnGreen.HasValue)
                    _LawnGreen = GetColor("#7CFC00");
                return _LawnGreen.Value;
            }
        }

        public static Color LemonChiffon
        {
            get
            {
                if (!_LemonChiffon.HasValue)
                    _LemonChiffon = GetColor("#FFFACD");
                return _LemonChiffon.Value;
            }
        }

        public static Color LightBlue
        {
            get
            {
                if (!_LightBlue.HasValue)
                    _LightBlue = GetColor("#ADD8E6");
                return _LightBlue.Value;
            }
        }

        public static Color LightCoral
        {
            get
            {
                if (!_LightCoral.HasValue)
                    _LightCoral = GetColor("#F08080");
                return _LightCoral.Value;
            }
        }

        public static Color LightCyan
        {
            get
            {
                if (!_LightCyan.HasValue)
                    _LightCyan = GetColor("#E0FFFF");
                return _LightCyan.Value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GoldenRod")]
        public static Color LightGoldenRodYellow
        {
            get
            {
                if (!_LightGoldenRodYellow.HasValue)
                    _LightGoldenRodYellow = GetColor("#FAFAD2");
                return _LightGoldenRodYellow.Value;
            }
        }

        public static Color LightGreen
        {
            get
            {
                if (!_LightGreen.HasValue)
                    _LightGreen = GetColor("#90EE90");
                return _LightGreen.Value;
            }
        }

        public static Color LightGrey
        {
            get
            {
                if (!_LightGrey.HasValue)
                    _LightGrey = GetColor("#D3D3D3");
                return _LightGrey.Value;
            }
        }

        public static Color LightPink
        {
            get
            {
                if (!_LightPink.HasValue)
                    _LightPink = GetColor("#FFB6C1");
                return _LightPink.Value;
            }
        }

        public static Color LightSalmon
        {
            get
            {
                if (!_LightSalmon.HasValue)
                    _LightSalmon = GetColor("#FFA07A");
                return _LightSalmon.Value;
            }
        }

        public static Color LightSeaGreen
        {
            get
            {
                if (!_LightSeaGreen.HasValue)
                    _LightSeaGreen = GetColor("#20B2AA");
                return _LightSeaGreen.Value;
            }
        }

        public static Color LightSkyBlue
        {
            get
            {
                if (!_LightSkyBlue.HasValue)
                    _LightSkyBlue = GetColor("#87CEFA");
                return _LightSkyBlue.Value;
            }
        }

        public static Color LightSlateGray
        {
            get
            {
                if (!_LightSlateGray.HasValue)
                    _LightSlateGray = GetColor("#778899");
                return _LightSlateGray.Value;
            }
        }

        public static Color LightSteelBlue
        {
            get
            {
                if (!_LightSteelBlue.HasValue)
                    _LightSteelBlue = GetColor("#B0C4DE");
                return _LightSteelBlue.Value;
            }
        }

        public static Color LightYellow
        {
            get
            {
                if (!_LightYellow.HasValue)
                    _LightYellow = GetColor("#FFFFE0");
                return _LightYellow.Value;
            }
        }

        public static Color Lime
        {
            get
            {
                if (!_Lime.HasValue)
                    _Lime = GetColor("#00FF00");
                return _Lime.Value;
            }
        }

        public static Color LimeGreen
        {
            get
            {
                if (!_LimeGreen.HasValue)
                    _LimeGreen = GetColor("#32CD32");
                return _LimeGreen.Value;
            }
        }

        public static Color Linen
        {
            get
            {
                if (!_Linen.HasValue)
                    _Linen = GetColor("#FAF0E6");
                return _Linen.Value;
            }
        }

        public static Color Magenta
        {
            get
            {
                if (!_Magenta.HasValue)
                    _Magenta = GetColor("#FF00FF");
                return _Magenta.Value;
            }
        }

        public static Color Maroon
        {
            get
            {
                if (!_Maroon.HasValue)
                    _Maroon = GetColor("#800000");
                return _Maroon.Value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "AquaMarine")]
        public static Color MediumAquaMarine
        {
            get
            {
                if (!_MediumAquaMarine.HasValue)
                    _MediumAquaMarine = GetColor("#66CDAA");
                return _MediumAquaMarine.Value;
            }
        }

        public static Color MediumBlue
        {
            get
            {
                if (!_MediumBlue.HasValue)
                    _MediumBlue = GetColor("#0000CD");
                return _MediumBlue.Value;
            }
        }

        public static Color MediumOrchid
        {
            get
            {
                if (!_MediumOrchid.HasValue)
                    _MediumOrchid = GetColor("#BA55D3");
                return _MediumOrchid.Value;
            }
        }

        public static Color MediumPurple
        {
            get
            {
                if (!_MediumPurple.HasValue)
                    _MediumPurple = GetColor("#9370D8");
                return _MediumPurple.Value;
            }
        }

        public static Color MediumSeaGreen
        {
            get
            {
                if (!_MediumSeaGreen.HasValue)
                    _MediumSeaGreen = GetColor("#3CB371");
                return _MediumSeaGreen.Value;
            }
        }

        public static Color MediumSlateBlue
        {
            get
            {
                if (!_MediumSlateBlue.HasValue)
                    _MediumSlateBlue = GetColor("#7B68EE");
                return _MediumSlateBlue.Value;
            }
        }

        public static Color MediumSpringGreen
        {
            get
            {
                if (!_MediumSpringGreen.HasValue)
                    _MediumSpringGreen = GetColor("#00FA9A");
                return _MediumSpringGreen.Value;
            }
        }

        public static Color MediumTurquoise
        {
            get
            {
                if (!_MediumTurquoise.HasValue)
                    _MediumTurquoise = GetColor("#48D1CC");
                return _MediumTurquoise.Value;
            }
        }

        public static Color MediumVioletRed
        {
            get
            {
                if (!_MediumVioletRed.HasValue)
                    _MediumVioletRed = GetColor("#C71585");
                return _MediumVioletRed.Value;
            }
        }

        public static Color MidnightBlue
        {
            get
            {
                if (!_MidnightBlue.HasValue)
                    _MidnightBlue = GetColor("#191970");
                return _MidnightBlue.Value;
            }
        }

        public static Color MintCream
        {
            get
            {
                if (!_MintCream.HasValue)
                    _MintCream = GetColor("#F5FFFA");
                return _MintCream.Value;
            }
        }

        public static Color MistyRose
        {
            get
            {
                if (!_MistyRose.HasValue)
                    _MistyRose = GetColor("#FFE4E1");
                return _MistyRose.Value;
            }
        }

        public static Color Moccasin
        {
            get
            {
                if (!_Moccasin.HasValue)
                    _Moccasin = GetColor("#FFE4B5");
                return _Moccasin.Value;
            }
        }

        public static Color NavajoWhite
        {
            get
            {
                if (!_NavajoWhite.HasValue)
                    _NavajoWhite = GetColor("#FFDEAD");
                return _NavajoWhite.Value;
            }
        }

        public static Color Navy
        {
            get
            {
                if (!_Navy.HasValue)
                    _Navy = GetColor("#000080");
                return _Navy.Value;
            }
        }

        public static Color OldLace
        {
            get
            {
                if (!_OldLace.HasValue)
                    _OldLace = GetColor("#FDF5E6");
                return _OldLace.Value;
            }
        }

        public static Color Olive
        {
            get
            {
                if (!_Olive.HasValue)
                    _Olive = GetColor("#808000");
                return _Olive.Value;
            }
        }

        public static Color OliveDrab
        {
            get
            {
                if (!_OliveDrab.HasValue)
                    _OliveDrab = GetColor("#6B8E23");
                return _OliveDrab.Value;
            }
        }

        public static Color Orange
        {
            get
            {
                if (!_Orange.HasValue)
                    _Orange = GetColor("#FFA500");
                return _Orange.Value;
            }
        }

        public static Color OrangeRed
        {
            get
            {
                if (!_OrangeRed.HasValue)
                    _OrangeRed = GetColor("#FF4500");
                return _OrangeRed.Value;
            }
        }

        public static Color Orchid
        {
            get
            {
                if (!_Orchid.HasValue)
                    _Orchid = GetColor("#DA70D6");
                return _Orchid.Value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GoldenRod")]
        public static Color PaleGoldenRod
        {
            get
            {
                if (!_PaleGoldenRod.HasValue)
                    _PaleGoldenRod = GetColor("#EEE8AA");
                return _PaleGoldenRod.Value;
            }
        }

        public static Color PaleGreen
        {
            get
            {
                if (!_PaleGreen.HasValue)
                    _PaleGreen = GetColor("#98FB98");
                return _PaleGreen.Value;
            }
        }

        public static Color PaleTurquoise
        {
            get
            {
                if (!_PaleTurquoise.HasValue)
                    _PaleTurquoise = GetColor("#AFEEEE");
                return _PaleTurquoise.Value;
            }
        }

        public static Color PaleVioletRed
        {
            get
            {
                if (!_PaleVioletRed.HasValue)
                    _PaleVioletRed = GetColor("#D87093");
                return _PaleVioletRed.Value;
            }
        }

        public static Color PapayaWhip
        {
            get
            {
                if (!_PapayaWhip.HasValue)
                    _PapayaWhip = GetColor("#FFEFD5");
                return _PapayaWhip.Value;
            }
        }

        public static Color PeachPuff
        {
            get
            {
                if (!_PeachPuff.HasValue)
                    _PeachPuff = GetColor("#FFDAB9");
                return _PeachPuff.Value;
            }
        }

        public static Color Peru
        {
            get
            {
                if (!_Peru.HasValue)
                    _Peru = GetColor("#CD853F");
                return _Peru.Value;
            }
        }

        public static Color Pink
        {
            get
            {
                if (!_Pink.HasValue)
                    _Pink = GetColor("#FFC0CB");
                return _Pink.Value;
            }
        }

        public static Color Plum
        {
            get
            {
                if (!_Plum.HasValue)
                    _Plum = GetColor("#DDA0DD");
                return _Plum.Value;
            }
        }

        public static Color PowderBlue
        {
            get
            {
                if (!_PowderBlue.HasValue)
                    _PowderBlue = GetColor("#B0E0E6");
                return _PowderBlue.Value;
            }
        }

        public static Color Purple
        {
            get
            {
                if (!_Purple.HasValue)
                    _Purple = GetColor("#800080");
                return _Purple.Value;
            }
        }

        public static Color Red
        {
            get
            {
                if (!_Red.HasValue)
                    _Red = GetColor("#FF0000");
                return _Red.Value;
            }
        }

        public static Color RosyBrown
        {
            get
            {
                if (!_RosyBrown.HasValue)
                    _RosyBrown = GetColor("#BC8F8F");
                return _RosyBrown.Value;
            }
        }

        public static Color RoyalBlue
        {
            get
            {
                if (!_RoyalBlue.HasValue)
                    _RoyalBlue = GetColor("#4169E1");
                return _RoyalBlue.Value;
            }
        }

        public static Color SaddleBrown
        {
            get
            {
                if (!_SaddleBrown.HasValue)
                    _SaddleBrown = GetColor("#8B4513");
                return _SaddleBrown.Value;
            }
        }

        public static Color Salmon
        {
            get
            {
                if (!_Salmon.HasValue)
                    _Salmon = GetColor("#FA8072");
                return _Salmon.Value;
            }
        }

        public static Color SandyBrown
        {
            get
            {
                if (!_SandyBrown.HasValue)
                    _SandyBrown = GetColor("#F4A460");
                return _SandyBrown.Value;
            }
        }

        public static Color SeaGreen
        {
            get
            {
                if (!_SeaGreen.HasValue)
                    _SeaGreen = GetColor("#2E8B57");
                return _SeaGreen.Value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SeaShell")]
        public static Color SeaShell
        {
            get
            {
                if (!_SeaShell.HasValue)
                    _SeaShell = GetColor("#FFF5EE");
                return _SeaShell.Value;
            }
        }

        public static Color Sienna
        {
            get
            {
                if (!_Sienna.HasValue)
                    _Sienna = GetColor("#A0522D");
                return _Sienna.Value;
            }
        }

        public static Color Silver
        {
            get
            {
                if (!_Silver.HasValue)
                    _Silver = GetColor("#C0C0C0");
                return _Silver.Value;
            }
        }

        public static Color SkyBlue
        {
            get
            {
                if (!_SkyBlue.HasValue)
                    _SkyBlue = GetColor("#87CEEB");
                return _SkyBlue.Value;
            }
        }

        public static Color SlateBlue
        {
            get
            {
                if (!_SlateBlue.HasValue)
                    _SlateBlue = GetColor("#6A5ACD");
                return _SlateBlue.Value;
            }
        }

        public static Color SlateGray
        {
            get
            {
                if (!_SlateGray.HasValue)
                    _SlateGray = GetColor("#708090");
                return _SlateGray.Value;
            }
        }

        public static Color Snow
        {
            get
            {
                if (!_Snow.HasValue)
                    _Snow = GetColor("#FFFAFA");
                return _Snow.Value;
            }
        }

        public static Color SpringGreen
        {
            get
            {
                if (!_SpringGreen.HasValue)
                    _SpringGreen = GetColor("#00FF7F");
                return _SpringGreen.Value;
            }
        }

        public static Color SteelBlue
        {
            get
            {
                if (!_SteelBlue.HasValue)
                    _SteelBlue = GetColor("#4682B4");
                return _SteelBlue.Value;
            }
        }

        public static Color Tan
        {
            get
            {
                if (!_Tan.HasValue)
                    _Tan = GetColor("#D2B48C");
                return _Tan.Value;
            }
        }

        public static Color Teal
        {
            get
            {
                if (!_Teal.HasValue)
                    _Teal = GetColor("#008080");
                return _Teal.Value;
            }
        }

        public static Color Thistle
        {
            get
            {
                if (!_Thistle.HasValue)
                    _Thistle = GetColor("#D8BFD8");
                return _Thistle.Value;
            }
        }

        public static Color Tomato
        {
            get
            {
                if (!_Tomato.HasValue)
                    _Tomato = GetColor("#FF6347");
                return _Tomato.Value;
            }
        }

        public static Color Turquoise
        {
            get
            {
                if (!_Turquoise.HasValue)
                    _Turquoise = GetColor("#40E0D0");
                return _Turquoise.Value;
            }
        }

        public static Color Violet
        {
            get
            {
                if (!_Violet.HasValue)
                    _Violet = GetColor("#EE82EE");
                return _Violet.Value;
            }
        }

        public static Color Wheat
        {
            get
            {
                if (!_Wheat.HasValue)
                    _Wheat = GetColor("#F5DEB3");
                return _Wheat.Value;
            }
        }

        public static Color White
        {
            get
            {
                if (!_White.HasValue)
                    _White = GetColor("#FFFFFF");
                return _White.Value;
            }
        }

        public static Color WhiteSmoke
        {
            get
            {
                if (!_WhiteSmoke.HasValue)
                    _WhiteSmoke = GetColor("#F5F5F5");
                return _WhiteSmoke.Value;
            }
        }

        public static Color Yellow
        {
            get
            {
                if (!_Yellow.HasValue)
                    _Yellow = GetColor("#FFFF00");
                return _Yellow.Value;
            }
        }

        public static Color YellowGreen
        {
            get
            {
                if (!_YellowGreen.HasValue)
                    _YellowGreen = GetColor("#9ACD32");
                return _YellowGreen.Value;
            }
        }

        #endregion Properties

        #region Methods

        public static Color? GetColor(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Match m = ColorRegex.Match(value);
                if (m.Success)
                {
                    byte a = 255;
                    if (m.Groups["a"].Success)
                        a = byte.Parse(m.Groups["a"].Value, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte r = byte.Parse(m.Groups["r"].Value, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte g = byte.Parse(m.Groups["g"].Value, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte b = byte.Parse(m.Groups["b"].Value, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    return Color.FromArgb(a, r, g, b);
                }
                else
                {
                    Color cachedColor;
                    if (colorCache.TryGetValue(value, out cachedColor))
                    {
                        return cachedColor;
                    }

                    Color? color = null;
                    // Create a brush per Colors property
                    var colorProperty = typeof(Colors).GetProperty(value, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreCase);
                    if (colorProperty != null && colorProperty.PropertyType == typeof(Color))
                    {
                        color = (Color)colorProperty.GetValue(null, null);
                    }

                    if (color == null)
                    {
                        colorProperty = typeof(ColorExtensions).GetProperty(value, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreCase);
                        if (colorProperty != null && colorProperty.PropertyType == typeof(Color))
                        {
                            color = (Color)colorProperty.GetValue(null, null);
                        }
                    }

                    if (color != null)
                    {
                        colorCache[value] = color.Value;
                        return color;
                    }
                }
            }

            return null;
        }

        #endregion Methods
    }
}