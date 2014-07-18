/**
 * WellKnowBinary
 *
 * Copyright 2012 Laurent Dupuis
 * http://www.dupuis.me/
 * 
 * Source: http://www.dupuis.me/node/28
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either 
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public 
 * License along with this library.  If not, see <http://www.gnu.org/licenses/>.
 * 
 **/


using System;

namespace geocoderNet.WellKnowBinary
{
    public class Point
    {
        public Point(double x, double y) { X = x; Y = y; }

        public double X { get; private set; }
        public double Y { get; private set; }
    }

    public class LinearRing
    {
        public LinearRing(Point[] points) { Points = points; }
        public Point[] Points { get; private set; }
    }


    public enum WkbGeometryType : uint
    {
        WkbPoint = 1,
        WkbLineString = 2,
        WkbPolygon = 3,
        WkbMultiPoint = 4,
        WkbMultiLineString = 5,
        WkbMultiPolygon = 6,
        WkbGeometryCollection = 7
    };

    public abstract class WkbShape
    {
        public abstract WkbGeometryType Type { get; }
    }

    public class WkbPoint : WkbShape
    {
        public override WkbGeometryType Type { get { return WkbGeometryType.WkbPoint; } }

        public WkbPoint(Point p) { P = p; }
        public Point P { get; private set; }
    }

    public class WkbLineString : WkbShape
    {
        public override WkbGeometryType Type { get { return WkbGeometryType.WkbLineString; } }

        public WkbLineString(Point[] points) { Points = points; }
        public Point[] Points { get; private set; }
    }

    public class WkbPolygon : WkbShape
    {
        public override WkbGeometryType Type { get { return WkbGeometryType.WkbPolygon; } }

        public WkbPolygon(LinearRing[] rings) { Rings = rings; }
        public LinearRing[] Rings { get; private set; }
    }

    public class WkbMultiPoint : WkbShape
    {
        public override WkbGeometryType Type { get { return WkbGeometryType.WkbMultiPoint; } }

        public WkbMultiPoint(WkbPoint[] points) { Points = points; }
        public WkbPoint[] Points { get; private set; }
    }

    public class WkbMultiLineString : WkbShape
    {
        public override WkbGeometryType Type { get { return WkbGeometryType.WkbMultiLineString; } }

        public WkbMultiLineString(WkbLineString[] lineString) { LineString = lineString; }
        public WkbLineString[] LineString { get; private set; }
    }

    public class WkbMultiPolygon : WkbShape
    {
        public override WkbGeometryType Type { get { return WkbGeometryType.WkbMultiPolygon; } }

        public WkbMultiPolygon(WkbPolygon[] polygons) { Polygons = polygons; }
        public WkbPolygon[] Polygons { get; private set; }
    }

    public class WkbGeometryCollection : WkbShape
    {
        public override WkbGeometryType Type { get { return WkbGeometryType.WkbGeometryCollection; } }

        public WkbGeometryCollection(WkbShape[] shapes) { Shapes = shapes; }
        public WkbShape[] Shapes { get; private set; }

    }


    public static class WkbDecoder
    {
        static WkbPoint ParsePoint(byte[] wkb, ref int pos)
        {
            if (wkb[pos] != 1) throw new Exception("Sorry, only Little Endian format is supported");
            var type = BitConverter.ToUInt32(wkb, pos + 1);
            if (type != (uint)WkbGeometryType.WkbPoint) throw new Exception("Invalid object type");
            pos += 5;

            var point = new Point(
                BitConverter.ToDouble(wkb, pos),
                BitConverter.ToDouble(wkb, pos + 8)
            );
            pos += 16;
            return new WkbPoint(point);
        }

        static WkbLineString ParseLineString(byte[] wkb, ref int pos)
        {
            if (wkb[pos] != 1) throw new Exception("Sorry, only Little Endian format is supported");
            var type = BitConverter.ToUInt32(wkb, pos + 1);
            if (type != (uint)WkbGeometryType.WkbLineString) throw new Exception("Invalid object type");
            var nbPoints = BitConverter.ToUInt32(wkb, pos + 5);
            pos += 9;

            var points = new Point[nbPoints];
            for (var i = 0; i < nbPoints; ++i)
            {
                points[i] = new Point(
                    BitConverter.ToDouble(wkb, pos),
                    BitConverter.ToDouble(wkb, pos + 8)
                );
                pos += 16;
            }

            return new WkbLineString(points);
        }

        static WkbPolygon ParsePolygon(byte[] wkb, ref int pos)
        {
            if (wkb[pos] != 1) throw new Exception("Sorry, only Little Endian format is supported");
            var type = BitConverter.ToUInt32(wkb, pos + 1);
            if (type != (uint)WkbGeometryType.WkbPolygon) throw new Exception("Invalid object type");
            var nbRings = BitConverter.ToUInt32(wkb, pos + 5);
            pos += 9;

            var rings = new LinearRing[nbRings];
            for (var r = 0; r < nbRings; ++r)
            {
                var nbPoints = BitConverter.ToUInt32(wkb, pos); pos += 4;
                var points = new Point[nbPoints];
                for (var i = 0; i < nbPoints; ++i)
                {
                    points[i] = new Point(
                        BitConverter.ToDouble(wkb, pos),
                        BitConverter.ToDouble(wkb, pos + 8)
                    );
                    pos += 16;
                }
                rings[r] = new LinearRing(points);
            }
            return new WkbPolygon(rings);
        }

        static WkbMultiPoint ParseMultiPoint(byte[] wkb, ref int pos)
        {
            if (wkb[pos] != 1) throw new Exception("Sorry, only Little Endian format is supported");
            var type = BitConverter.ToUInt32(wkb, pos + 1);
            if (type != (uint)WkbGeometryType.WkbMultiPoint) throw new Exception("Invalid object type");
            var nbPoints = BitConverter.ToUInt32(wkb, pos + 5);
            pos += 9;

            var points = new WkbPoint[nbPoints];
            for (var i = 0; i < nbPoints; ++i)
            {
                points[i] = ParsePoint(wkb, ref pos);
            }

            return new WkbMultiPoint(points);
        }

        static WkbMultiLineString ParseMultiLineString(byte[] wkb, ref int pos)
        {
            if (wkb[pos] != 1) throw new Exception("Sorry, only Little Endian format is supported");
            var type = BitConverter.ToUInt32(wkb, pos + 1);
            if (type != (uint)WkbGeometryType.WkbMultiLineString) throw new Exception("Invalid object type");
            var nbLineStrings = BitConverter.ToUInt32(wkb, pos + 5);
            pos += 9;

            var lineStrings = new WkbLineString[nbLineStrings];
            for (var i = 0; i < nbLineStrings; ++i)
            {
                lineStrings[i] = ParseLineString(wkb, ref pos);
            }

            return new WkbMultiLineString(lineStrings);
        }

        static WkbMultiPolygon ParseMultiPolygon(byte[] wkb, ref int pos)
        {
            if (wkb[pos] != 1) throw new Exception("Sorry, only Little Endian format is supported");
            var type = BitConverter.ToUInt32(wkb, pos + 1);
            if (type != (uint)WkbGeometryType.WkbMultiPolygon) throw new Exception("Invalid object type");
            var nbPolygons = BitConverter.ToUInt32(wkb, pos + 5);
            pos += 9;

            var polygons = new WkbPolygon[nbPolygons];
            for (var r = 0; r < nbPolygons; ++r)
            {
                polygons[r] = ParsePolygon(wkb, ref pos);
            }

            return new WkbMultiPolygon(polygons);
        }

        static WkbGeometryCollection ParseGeometryCollection(byte[] wkb, ref int pos)
        {
            if (wkb[pos] != 1) throw new Exception("Sorry, only Little Endian format is supported");
            var type = BitConverter.ToUInt32(wkb, pos + 1);
            if (type != (uint)WkbGeometryType.WkbGeometryCollection) throw new Exception("Invalid object type");
            var nbShapes = BitConverter.ToUInt32(wkb, pos + 5);
            pos += 9;

            var shapes = new WkbShape[nbShapes];
            for (var r = 0; r < nbShapes; ++r)
            {
                shapes[r] = ParseShape(wkb, ref pos);
            }

            return new WkbGeometryCollection(shapes);
        }


        static public WkbShape ParseShape(byte[] wkb, ref int pos)
        {
            var type = BitConverter.ToUInt32(wkb, pos + 1);
            switch (type)
            {
                case (uint)WkbGeometryType.WkbPoint:
                    return ParsePoint(wkb, ref pos);
                case (uint)WkbGeometryType.WkbLineString:
                    return ParseLineString(wkb, ref pos);
                case (uint)WkbGeometryType.WkbPolygon:
                    return ParsePolygon(wkb, ref pos);

                case (uint)WkbGeometryType.WkbMultiPoint:
                    return ParseMultiPoint(wkb, ref pos);
                case (uint)WkbGeometryType.WkbMultiLineString:
                    return ParseMultiLineString(wkb, ref pos);
                case (uint)WkbGeometryType.WkbMultiPolygon:
                    return ParseMultiPolygon(wkb, ref pos);

                case (uint)WkbGeometryType.WkbGeometryCollection:
                    return ParseGeometryCollection(wkb, ref pos);

                default:
                    throw new Exception("Unsupported type");
            }
        }

        static public WkbShape Parse(byte[] wkb)
        {
            var pos = 0;
            return ParseShape(wkb, ref pos);
        }
    }
}