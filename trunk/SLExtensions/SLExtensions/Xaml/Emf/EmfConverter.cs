using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;

using SLExtensions.IO;

namespace SLExtensions.Xaml.Emf
{
    /// <summary>
    /// Converts Enhanced Metafile streams to XAML.
    /// </summary>
    public class EmfConverter
        : ConverterBase
    {
        private const short META_SETBKCOLOR = 0x0201;
        private const short META_SETBKMODE = 0x0102;
        private const short META_SETMAPMODE = 0x0103;
        private const short META_SETROP2 = 0x0104;
        private const short META_SETPOLYFILLMODE = 0x0106;
        private const short META_SETSTRETCHBLTMODE = 0x0107;
        private const short META_SETTEXTCOLOR = 0x0209;
        private const short META_SETTEXTCHAREXTRA = 0x0108;
        private const short META_SETWINDOWORG = 0x020B;
        private const short META_SETWINDOWEXT = 0x020C;
        private const short META_SETVIEWPORTORG = 0x020D;
        private const short META_SETVIEWPORTEXT = 0x020E;
        private const short META_OFFSETWINDOWORG = 0x020F;
        private const short META_SCALEWINDOWEXT = 0x0410;
        private const short META_OFFSETVIEWPORTORG = 0x0211;
        private const short META_SCALEVIEWPORTEXT = 0x0412;
        private const short META_LINETO = 0x0213;
        private const short META_MOVETO = 0x0214;
        private const short META_EXCLUDECLIPRECT = 0x0415;
        private const short META_INTERSECTCLIPRECT = 0x0416;
        private const short META_ARC = 0x0817;
        private const short META_ELLIPSE = 0x0418;
        private const short META_FLOODFILL = 0x0419;
        private const short META_PIE = 0x081A;
        private const short META_RECTANGLE = 0x041B;
        private const short META_ROUNDRECT = 0x061C;
        private const short META_PATBLT = 0x061D;
        private const short META_SAVEDC = 0x001E;
        private const short META_SETPIXEL = 0x041F;
        private const short META_OFFSETCLIPRGN = 0x0220;
        private const short META_POLYGON = 0x0324;
        private const short META_POLYLINE = 0x0325;
        private const short META_ESCAPE = 0x0626;
        private const short META_RESTOREDC = 0x0127;
        private const short META_FILLREGION = 0x0228;
        private const short META_FRAMEREGION = 0x0429;
        private const short META_INVERTREGION = 0x012A;
        private const short META_PAINTREGION = 0x012B;
        private const short META_SELECTCLIPREGION = 0x012C;
        private const short META_SELECTOBJECT = 0x012D;
        private const short META_SETTEXTALIGN = 0x012E;
        private const short META_CHORD = 0x0830;
        private const short META_SETMAPPERFLAGS = 0x0231;
        private const short META_TEXTOUT = 0x0521;
        private const short META_EXTTEXTOUT = 0x0a32;
        private const short META_SETDIBTODEV = 0x0d33;
        private const short META_POLYPOLYGON = 0x0538;
        private const short META_DIBBITBLT = 0x0940;
        private const short META_DIBSTRETCHBLT = 0x0b41;
        private const short META_EXTFLOODFILL = 0x0548;
        private const short META_DELETEOBJECT = 0x01f0;
        private const short META_CREATEPENINDIRECT = 0x02FA;
        private const short META_CREATEFONTINDIRECT = 0x02FB;
        private const short META_CREATEBRUSHINDIRECT = 0x02FC;
        private const short META_CREATEREGION = 0x06FF;
        private const short META_STRETCHDIB = 0x0f43;
        private const short META_SETTEXTJUSTIFICATION = 0x020A;
        private const short META_BITBLT = 0x0922;
        private const short META_STRETCHBLT = 0x0B23;
        private const short META_CREATEPATTERNBRUSH = 0x01F9;
        private const short META_SELECTPALETTE = 0x0234;
        private const short META_REALIZEPALETTE = 0x0035;
        private const short META_ANIMATEPALETTE = 0x0436;
        private const short META_SETPALENTRIES = 0x0037;
        private const short META_RESIZEPALETTE = 0x0139;
        private const short META_CREATEPALETTE = 0x00f7;
        private const short META_SETRELABS = 0x0105;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmfConverter"/> class.
        /// </summary>
        public EmfConverter()
        {
        }

        /// <summary>
        /// Writes the XAML from specified EMF input stream.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        public override void ToXaml(Stream input, XmlWriter output)
        {
            if (input == null) {
                throw new ArgumentNullException("input");
            }

            byte[] header;

            if (!ByteUtility.TryRead(input, 40, out header) || ReadUInt32(header, 0) != 0x9ac6cdd7) {
                throw new ConverterException(Resource.ExceptionNoEmfStream);
            }

            // EMF special header
            short handle = ReadInt16(header, 4);
            short left = ReadInt16(header, 6);
            short top = ReadInt16(header, 8);
            short right = ReadInt16(header, 10);
            short bottom = ReadInt16(header, 12);
            short twipsPerInch = ReadInt16(header, 14);
            int reserved = ReadInt32(header, 16);
            short checksum = ReadInt16(header, 20);

            // EMF meta header
            short fileType = ReadInt16(header, 22);
            short headerSize = ReadInt16(header, 24);
            short version = ReadInt16(header, 26);
            int fileSize = ReadInt32(header, 28);
            short numberOfObjects = ReadInt16(header, 32);
            int maxRecordSize = ReadInt32(header, 34);
            short noParameters = ReadInt16(header, 38);

            EmfContext context = new EmfContext();

            double width = right - left;
            double height = bottom - top;

            context.ViewPortExt = new Point(width, height);
            context.Scale = 1D / 15D * 1440 / twipsPerInch;     // converts twips to pixels

            WriteStartElement(output, "Canvas");
            WriteAttribute(output, "Width", width * context.Scale);
            WriteAttribute(output, "Height", height * context.Scale);

            while (ReadRecord(input, context, output)) ;

            WriteEndElement(output);
        }

        private bool ReadRecord(Stream stream, EmfContext context, XmlWriter output)
        {
            byte[] header = ByteUtility.Read(stream, 6);
            int size = ReadInt32(header, 0);
            short function = ReadInt16(header, 4);

            if (function == 0) {
                return false;
            }

            byte[] parameters = ByteUtility.Read(stream, (int)((size - 3) * 2));

            if (function == META_SETBKCOLOR) {
                context.BackgroundColor = ReadColor(parameters, 0);
            }
            else if (function == META_SETBKMODE) {
                context.BackgroundMode = ReadInt16(parameters, 0);
            }
            else if (function == META_SETMAPMODE) {
                context.MappingMode = ReadInt16(parameters, 0);
            }
            else if (function == META_SETROP2) {
                HandleNotImplemented("META_SETROP2", parameters);
            }
            else if (function == META_SETPOLYFILLMODE) {
                context.PolyFillMode = ReadInt16(parameters, 0);
            }
            else if (function == META_SETSTRETCHBLTMODE) {
                HandleNotImplemented("META_SETSTRETCHBLTMODE", parameters);
            }
            else if (function == META_SETTEXTCOLOR) {
                context.TextColor = ReadColor(parameters, 0);
            }
            else if (function == META_SETTEXTCHAREXTRA) {
                HandleNotImplemented("META_SETTEXTCHAREXTRA", parameters);
            }
            else if (function == META_SETWINDOWORG) {
                context.WindowOrg = ReadPoint(parameters, 0, false, null);
            }
            else if (function == META_SETWINDOWEXT) {
                context.WindowExt = ReadPoint(parameters, 0, false, null);
            }
            else if (function == META_SETVIEWPORTORG) {
                context.ViewPortOrg = ReadPoint(parameters, 0, false, null);
            }
            else if (function == META_SETVIEWPORTEXT) {
                context.ViewPortExt = ReadPoint(parameters, 0, false, null);
            }
            else if (function == META_OFFSETWINDOWORG) {
                context.OffsetWindowOrg = ReadPoint(parameters, 0, false, null);
            }
            else if (function == META_SCALEWINDOWEXT) {
                //context.ScaleWindowExt = new Scale(parameters);
                HandleNotImplemented("META_EXCLUDECLIPRECT", parameters);
            }
            else if (function == META_OFFSETVIEWPORTORG) {
                context.OffsetViewPortOrg = ReadPoint(parameters, 0, false, null);
            }
            else if (function == META_SCALEVIEWPORTEXT) {
                //context.ScaleViewPortExt = new Scale(parameters);
                HandleNotImplemented("META_EXCLUDECLIPRECT", parameters);
            }
            else if (function == META_LINETO) {
                WriteLine(parameters, context, output);
            }
            else if (function == META_MOVETO) {
                context.CurrentPosition = ReadPoint(parameters, 0, false, context);
            }
            else if (function == META_EXCLUDECLIPRECT) {
                HandleNotImplemented("META_EXCLUDECLIPRECT", parameters);
            }
            else if (function == META_INTERSECTCLIPRECT) {
                HandleNotImplemented("META_INTERSECTCLIPRECT", parameters);
            }
            else if (function == META_ARC){
                WriteArc(parameters, context, output);
            }
            else if (function == META_ELLIPSE) {
                WriteEllipse(parameters, context, output);
            }
            else if (function == META_FLOODFILL) {
                HandleNotImplemented("META_FLOODFILL", parameters);
            }
            else if (function == META_PIE) {
                HandleNotImplemented("META_PIE", parameters);
            }
            else if (function == META_RECTANGLE) {
                WriteRectangle(parameters, context, output);
            }
            else if (function == META_ROUNDRECT) {
                HandleNotImplemented("META_ROUNDRECT", parameters);
            }
            else if (function == META_PATBLT) {
                HandleNotImplemented("META_PATBLT", parameters);
            }
            else if (function == META_SAVEDC) {
                HandleNotImplemented("META_SAVEDC", parameters);
            }
            else if (function == META_SETPIXEL) {
                HandleNotImplemented("META_SETPIXEL", parameters);
            }
            else if (function == META_OFFSETCLIPRGN) {
                HandleNotImplemented("META_OFFSETCLIPRGN", parameters);
            }
            else if (function == META_POLYGON) {
                WritePolygon(parameters, context, output);
            }
            else if (function == META_POLYLINE) {
                WritePolyline(parameters, context, output);
            }
            else if (function == META_ESCAPE) {
                HandleNotImplemented("META_ESCAPE", parameters);
            }
            else if (function == META_RESTOREDC) {
                HandleNotImplemented("META_RESTOREDC", parameters);
            }
            else if (function == META_FILLREGION) {
                HandleNotImplemented("META_FILLREGION", parameters);
            }
            else if (function == META_FRAMEREGION) {
                HandleNotImplemented("META_FRAMEREGION", parameters);
            }
            else if (function == META_INVERTREGION) {
                HandleNotImplemented("META_INVERTREGION", parameters);
            }
            else if (function == META_PAINTREGION) {
                HandleNotImplemented("META_PAINTREGION", parameters);
            }
            else if (function == META_SELECTCLIPREGION) {
                short index = ReadInt16(parameters, 0);
                context.SelectObject(index);
            }
            else if (function == META_SELECTOBJECT) {
                short index = ReadInt16(parameters, 0);
                context.SelectObject(index);
            }
            else if (function == META_SETTEXTALIGN) {
                HandleNotImplemented("META_SETTEXTALIGN", parameters);
            }
            else if (function == META_CHORD) {
                HandleNotImplemented("META_CHORD", parameters);
            }
            else if (function == META_SETMAPPERFLAGS) {
                HandleNotImplemented("META_SETMAPPERFLAGS", parameters);
            }
            else if (function == META_TEXTOUT) {
                HandleNotImplemented("META_TEXTOUT", parameters);
            }
            else if (function == META_EXTTEXTOUT) {
                HandleNotImplemented("META_EXTTEXTOUT", parameters);
            }
            else if (function == META_SETDIBTODEV) {
                HandleNotImplemented("META_SETDIBTODEV", parameters);
            }
            else if (function == META_POLYPOLYGON) {
                WritePolyPolygon(parameters, context, output);
            }
            else if (function == META_DIBBITBLT) {
                HandleNotImplemented("META_DIBBITBLT", parameters);
            }
            else if (function == META_DIBSTRETCHBLT) {
                HandleNotImplemented("META_DIBSTRETCHBLT", parameters);
            }
            else if (function == META_EXTFLOODFILL) {
                HandleNotImplemented("META_EXTFLOODFILL", parameters);
            }
            else if (function == META_DELETEOBJECT) {
                short index = ReadInt16(parameters, 0);
                context.RemoveObject(index);
            }
            else if (function == META_CREATEPENINDIRECT) {
                context.AddObject(new Pen(parameters));
            }
            else if (function == META_CREATEFONTINDIRECT) {
                context.AddObject(new object());

                HandleNotImplemented("META_CREATEFONTINDIRECT", parameters);
            }
            else if (function == META_CREATEBRUSHINDIRECT) {
                context.AddObject(new Brush(parameters));
            }
            else if (function == META_CREATEREGION) {
                context.AddObject(new object());

                HandleNotImplemented("META_CREATEREGION", parameters);
            }
            else if (function == META_STRETCHDIB) {
                HandleNotImplemented("META_STRETCHDIB", parameters);
            }
            else if (function == META_SETTEXTJUSTIFICATION) {
                HandleNotImplemented("META_SETTEXTJUSTIFICATION", parameters);
            }
            else if (function == META_BITBLT) {
                HandleNotImplemented("META_BITBLT", parameters);
            }
            else if (function == META_STRETCHBLT) {
                HandleNotImplemented("META_STRETCHBLT", parameters);
            }
            else if (function == META_CREATEPATTERNBRUSH) {
                context.AddObject(new object());

                HandleNotImplemented("META_CREATEPATTERNBRUSH", parameters);
            }
            else if (function == META_SELECTPALETTE) {
                short index = ReadInt16(parameters, 0);
                context.SelectObject(index);
            }
            else if (function == META_REALIZEPALETTE) {
                HandleNotImplemented("META_REALIZEPALETTE", parameters);
            }
            else if (function == META_ANIMATEPALETTE) {
                HandleNotImplemented("META_ANIMATEPALETTE", parameters);
            }
            else if (function == META_SETPALENTRIES) {
                HandleNotImplemented("META_SETPALENTRIES", parameters);
            }
            else if (function == META_RESIZEPALETTE) {
                HandleNotImplemented("META_RESIZEPALETTE", parameters);
            }
            else if (function == META_CREATEPALETTE) {
                context.AddObject(new object());

                HandleNotImplemented("META_CREATEPALETTE", parameters);
            }
            else if (function == META_SETRELABS) {
                HandleNotImplemented("META_SETRELABS", parameters);
            }
            else {
                OnWarning(Resource.MessageEmfFunctionUnknown, function);
            }

            return true;
        }

        private void HandleNotImplemented(string functionName, byte[] parameters)
        {
            OnWarning(Resource.MessageEmfFunctionNotSupported, functionName);
        }

        private void WriteLine(byte[] parameters, EmfContext context, XmlWriter output)
        {
            Point to = ReadPoint(parameters, 0, false, context);

            WriteStartElement(output, "Line");
            WriteAttribute(output, "X1", context.CurrentPosition.X);
            WriteAttribute(output, "Y1", context.CurrentPosition.Y);
            WriteAttribute(output, "X2", to.X);
            WriteAttribute(output, "Y2", to.Y);
            WriteStroke(context.Pen, output);
            WriteEndElement(output);
        }

        private void WriteArc(byte[] parameters, EmfContext context, XmlWriter output)
        {
            Point end = ReadPoint(parameters, 0, false, context);
            Point start = ReadPoint(parameters, 4, false, context);
            Point rightBottom = ReadPoint(parameters, 8, false, context);
            Point leftTop = ReadPoint(parameters, 12, false, context);

            WriteStartElement(output, "Path");
            WriteStroke(context.Pen, output);
            //WriteFill(context.Brush, output);
            WriteStartElement(output, "Path.Data");
            WriteStartElement(output, "PathGeometry");
            WriteStartElement(output, "PathGeometry.Figures");
            WriteStartElement(output, "PathFigure");
            WriteAttribute(output, "StartPoint", start);
            WriteStartElement(output, "PathFigure.Segments");
            WriteStartElement(output, "ArcSegment");
            WriteAttribute(output, "Size", new Size(rightBottom.X - leftTop.X, rightBottom.Y - leftTop.Y));
            WriteAttribute(output, "Point", end);
            WriteEndElement(output);
            WriteEndElement(output);
            WriteEndElement(output);
            WriteEndElement(output);
            WriteEndElement(output);
            WriteEndElement(output);
            WriteEndElement(output);
        }

        private void WriteEllipse(byte[] parameters, EmfContext context, XmlWriter output)
        {
            Point rightBottom = ReadPoint(parameters, 0, false, context);
            Point leftTop = ReadPoint(parameters, 4, false, context);

            WriteStartElement(output, "Ellipse");
            WriteAttribute(output, "Canvas.Left", leftTop.X);
            WriteAttribute(output, "Canvas.Top", leftTop.Y);
            WriteAttribute(output, "Width", rightBottom.X - leftTop.X);
            WriteAttribute(output, "Height", rightBottom.Y - leftTop.Y);
            WriteStroke(context.Pen, output);
            WriteFill(context.Brush, output);
            WriteEndElement(output);
        }

        private void WriteRectangle(byte[] parameters, EmfContext context, XmlWriter output)
        {
            Point rightBottom = ReadPoint(parameters, 0, false, context);
            Point leftTop = ReadPoint(parameters, 4, false, context);

            WriteStartElement(output, "Rectangle");
            WriteAttribute(output, "Canvas.Left", leftTop.X);
            WriteAttribute(output, "Canvas.Top", leftTop.Y);
            WriteAttribute(output, "Width", rightBottom.X - leftTop.X);
            WriteAttribute(output, "Height", rightBottom.Y - leftTop.Y);
            WriteStroke(context.Pen, output);
            WriteFill(context.Brush, output);
            WriteEndElement(output);
        }

        private void WritePolyline(byte[] parameters, EmfContext context, XmlWriter output)
        {
            int count = ReadInt16(parameters, 0);
            string points = ReadPoints(parameters, 2, count, context);

            WriteStartElement(output, "Polyline");
            WriteStroke(context.Pen, output);
            WriteAttribute(output, "Points", points);
            WriteEndElement(output);
        }

        private void WritePolygon(byte[] parameters, EmfContext context, XmlWriter output)
        {
            int count = ReadInt16(parameters, 0);
            string points = ReadPoints(parameters, 2, count, context);

            WritePolygon(points, context, output);
        }

        private void WritePolyPolygon(byte[] parameters, EmfContext context, XmlWriter output)
        {
            int count = ReadInt16(parameters, 0);
            int position = count * 2 + 2;
            for (int i = 0; i < count; i++) {
                int pointsCount = ReadInt16(parameters, (i * 2) + 2);
                string points = ReadPoints(parameters, position, pointsCount, context);

                position += 4 * pointsCount;

                WritePolygon(points, context, output);
            }
        }

        private void WritePolygon(string points, EmfContext context, XmlWriter output)
        {
            WriteStartElement(output, "Polygon");
            WriteStroke(context.Pen, output);
            WriteFill(context.Brush, output);
            WriteAttribute(output, "Points", points);
            WriteEndElement(output);
        }

        private void WriteStroke(Pen pen, XmlWriter output)
        {
            if (pen != null) {
                double thickness = .5;
                string strokeDashArray = null;

                if (pen.Style == Pen.PS_SOLID || pen.Width > 1) {
                    thickness = (double)pen.Width / 2;
                    if (pen.Width == 0) {
                        thickness = .5;
                    }
                }
                else if (pen.Style == Pen.PS_DASH) {
                    strokeDashArray = "6,6";
                }
                else if (pen.Style == Pen.PS_DOT) {
                    strokeDashArray = "6,6";
                }
                else if (pen.Style == Pen.PS_DASHDOT) {
                    strokeDashArray = "6,3";
                }
                else if (pen.Style == Pen.PS_DASHDOTDOT) {
                    strokeDashArray = "6,3,3";
                }
                else {
                    return;
                }

                WriteAttribute(output, "Stroke", pen.Color);
                WriteAttribute(output, "StrokeThickness", thickness);
                if (strokeDashArray != null) {
                    WriteAttribute(output, "StrokeDashArray", strokeDashArray);
                }
            }
        }

        private void WriteFill(Brush brush, XmlWriter output)
        {
            if (brush != null) {
                if (brush.Style == Brush.BS_SOLID) {
                    WriteAttribute(output, "Fill", brush.Color);
                }
                else if (brush.Style == Brush.BS_HATCHED) {
                    //TODO: implement
                }
            }
        }

        internal static string ReadPoints(byte[] parameters, int index, int count, EmfContext context)
        {
            StringBuilder points = new StringBuilder();
            for (int i = index; i < parameters.Length; i += 4) {
                if (i > index) {
                    points.Append(' ');
                }

                Point p = ReadPoint(parameters, i, true, context);

                points.Append(p.Format());
            }

            return points.ToString();
        }

        internal static Int16 ReadInt16(byte[] parameters, int index)
        {
            return ByteUtility.ReadInt16(parameters, index, ByteOrder.LittleEndian);
        }

        internal static Int32 ReadInt32(byte[] parameters, int index)
        {
            return ByteUtility.ReadInt32(parameters, index, ByteOrder.LittleEndian);
        }

        internal static long ReadUInt32(byte[] parameters, int index)
        {
            return ByteUtility.ReadUInt32(parameters, index, ByteOrder.LittleEndian);
        }

        internal static Point ReadPoint(byte[] parameters, int index, bool xy, EmfContext context)
        {
            int x = ReadInt16(parameters, index);
            int y = ReadInt16(parameters, index + 2);

            if (!xy) {
                int c = x;
                x = y;
                y = c;
            }

            if (context != null) {
                return context.Translate(x, y);
            }

            return new Point(x, y);
        }

        internal static Size ReadSize(byte[] parameters, int index)
        {
            int height = ReadInt16(parameters, index);
            int width = ReadInt16(parameters, index + 2);

            return new Size(width, height);
        }

        internal static Color ReadColor(byte[] parameters, int index)
        {
            int c = ReadInt32(parameters, index);

            return Color.FromArgb(0xff, (byte)(c & 0xff), (byte)((c >> 8) & 0xff), (byte)((c >> 16) & 0xff));
        }
    }
}
