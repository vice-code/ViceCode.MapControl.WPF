using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Maps.MapExtras
{
    internal static class VectorMath
    {
        public const double DegreesPerRadian = 57.2957795130823;
        public const double RadiansPerDegree = 0.0174532925199433;

        public static void ClipConvexPolygon(
          RectangularSolid clipBounds,
          Point4D[] poly,
          Point2D[] polyTextureCoords,
          int polyVertexCount,
          Point4D[] clippedPoly,
          Point2D[] clippedPolyTextureCoords,
          out int clippedPolyVertexCount,
          Point4D[] tempVertexBuffer,
          Point2D[] tempTextureCoordBuffer)
        {
            if (polyVertexCount < 3 || poly is null || (poly.Length < polyVertexCount || clippedPoly is null) || (clippedPoly.Length < polyVertexCount + 6 || tempVertexBuffer is null || tempVertexBuffer.Length < polyVertexCount + 6))
                throw new ArgumentException("polygon arrays must have sufficient capacity");
            if (polyTextureCoords is object && (polyTextureCoords.Length < polyVertexCount || clippedPolyTextureCoords is null || (clippedPolyTextureCoords.Length < polyVertexCount + 6 || tempTextureCoordBuffer is null) || tempTextureCoordBuffer.Length < polyVertexCount + 6))
                throw new ArgumentException("polygon arrays must have sufficient capacity");
            Swap(ref tempVertexBuffer, ref clippedPoly);
            Swap(ref tempTextureCoordBuffer, ref clippedPolyTextureCoords);
            var left1 = tempVertexBuffer;
            var left2 = tempTextureCoordBuffer;
            var num1 = polyVertexCount;
            clippedPolyVertexCount = 0;
            var p0Idx1 = num1 - 1;
            for (var p1Idx = 0; p1Idx < num1; ++p1Idx)
            {
                var BC0 = poly[p0Idx1].X - clipBounds.X * poly[p0Idx1].W;
                var BC1 = poly[p1Idx].X - clipBounds.X * poly[p1Idx].W;
                GenericClipAgainstPlane(clippedPoly, clippedPolyTextureCoords, ref clippedPolyVertexCount, poly, polyTextureCoords, p0Idx1, p1Idx, BC0, BC1);
                p0Idx1 = p1Idx;
            }
            if (clippedPolyVertexCount == 0)
                return;
            Swap(ref left1, ref clippedPoly);
            Swap(ref left2, ref clippedPolyTextureCoords);
            var num2 = clippedPolyVertexCount;
            clippedPolyVertexCount = 0;
            var p0Idx2 = num2 - 1;
            for (var p1Idx = 0; p1Idx < num2; ++p1Idx)
            {
                var BC0 = (clipBounds.X + clipBounds.SizeX) * left1[p0Idx2].W - left1[p0Idx2].X;
                var BC1 = (clipBounds.X + clipBounds.SizeX) * left1[p1Idx].W - left1[p1Idx].X;
                GenericClipAgainstPlane(clippedPoly, clippedPolyTextureCoords, ref clippedPolyVertexCount, left1, left2, p0Idx2, p1Idx, BC0, BC1);
                p0Idx2 = p1Idx;
            }
            if (clippedPolyVertexCount == 0)
                return;
            Swap(ref left1, ref clippedPoly);
            Swap(ref left2, ref clippedPolyTextureCoords);
            var num3 = clippedPolyVertexCount;
            clippedPolyVertexCount = 0;
            var p0Idx3 = num3 - 1;
            for (var p1Idx = 0; p1Idx < num3; ++p1Idx)
            {
                var BC0 = left1[p0Idx3].Y - clipBounds.Y * left1[p0Idx3].W;
                var BC1 = left1[p1Idx].Y - clipBounds.Y * left1[p1Idx].W;
                GenericClipAgainstPlane(clippedPoly, clippedPolyTextureCoords, ref clippedPolyVertexCount, left1, left2, p0Idx3, p1Idx, BC0, BC1);
                p0Idx3 = p1Idx;
            }
            if (clippedPolyVertexCount == 0)
                return;
            Swap(ref left1, ref clippedPoly);
            Swap(ref left2, ref clippedPolyTextureCoords);
            var num4 = clippedPolyVertexCount;
            clippedPolyVertexCount = 0;
            var p0Idx4 = num4 - 1;
            for (var p1Idx = 0; p1Idx < num4; ++p1Idx)
            {
                var BC0 = (clipBounds.Y + clipBounds.SizeY) * left1[p0Idx4].W - left1[p0Idx4].Y;
                var BC1 = (clipBounds.Y + clipBounds.SizeY) * left1[p1Idx].W - left1[p1Idx].Y;
                GenericClipAgainstPlane(clippedPoly, clippedPolyTextureCoords, ref clippedPolyVertexCount, left1, left2, p0Idx4, p1Idx, BC0, BC1);
                p0Idx4 = p1Idx;
            }
            if (clippedPolyVertexCount == 0)
                return;
            Swap(ref left1, ref clippedPoly);
            Swap(ref left2, ref clippedPolyTextureCoords);
            var num5 = clippedPolyVertexCount;
            clippedPolyVertexCount = 0;
            var p0Idx5 = num5 - 1;
            for (var p1Idx = 0; p1Idx < num5; ++p1Idx)
            {
                var BC0 = left1[p0Idx5].Z - clipBounds.Z * left1[p0Idx5].W;
                var BC1 = left1[p1Idx].Z - clipBounds.Z * left1[p1Idx].W;
                GenericClipAgainstPlane(clippedPoly, clippedPolyTextureCoords, ref clippedPolyVertexCount, left1, left2, p0Idx5, p1Idx, BC0, BC1);
                p0Idx5 = p1Idx;
            }
            if (clippedPolyVertexCount == 0)
                return;
            Swap(ref left1, ref clippedPoly);
            Swap(ref left2, ref clippedPolyTextureCoords);
            var num6 = clippedPolyVertexCount;
            clippedPolyVertexCount = 0;
            var p0Idx6 = num6 - 1;
            for (var p1Idx = 0; p1Idx < num6; ++p1Idx)
            {
                var BC0 = (clipBounds.Z + clipBounds.SizeZ) * left1[p0Idx6].W - left1[p0Idx6].Z;
                var BC1 = (clipBounds.Z + clipBounds.SizeZ) * left1[p1Idx].W - left1[p1Idx].Z;
                GenericClipAgainstPlane(clippedPoly, clippedPolyTextureCoords, ref clippedPolyVertexCount, left1, left2, p0Idx6, p1Idx, BC0, BC1);
                p0Idx6 = p1Idx;
            }
        }

        private static void GenericClipAgainstPlane(
          Point4D[] clippedPoly,
          Point2D[] clippedPolyTextureCoords,
          ref int clippedPolyVertexCount,
          Point4D[] clippedPolyCurrent,
          Point2D[] clippedPolyTextureCoordsCurrent,
          int p0Idx,
          int p1Idx,
          double BC0,
          double BC1)
        {
            if (BC1 >= 0.0)
            {
                if (BC0 < 0.0)
                {
                    var alpha = BC0 / (BC0 - BC1);
                    clippedPoly[clippedPolyVertexCount] = Point4D.Lerp(clippedPolyCurrent[p0Idx], clippedPolyCurrent[p1Idx], alpha);
                    if (clippedPolyTextureCoords is object)
                        clippedPolyTextureCoords[clippedPolyVertexCount] = Point2D.Lerp(clippedPolyTextureCoordsCurrent[p0Idx], clippedPolyTextureCoordsCurrent[p1Idx], alpha);
                    ++clippedPolyVertexCount;
                }
                clippedPoly[clippedPolyVertexCount] = clippedPolyCurrent[p1Idx];
                if (clippedPolyTextureCoords is object)
                    clippedPolyTextureCoords[clippedPolyVertexCount] = clippedPolyTextureCoordsCurrent[p1Idx];
                ++clippedPolyVertexCount;
            }
            else
            {
                if (BC0 < 0.0)
                    return;
                var alpha = BC0 / (BC0 - BC1);
                clippedPoly[clippedPolyVertexCount] = Point4D.Lerp(clippedPolyCurrent[p0Idx], clippedPolyCurrent[p1Idx], alpha);
                if (clippedPolyTextureCoords is object)
                    clippedPolyTextureCoords[clippedPolyVertexCount] = Point2D.Lerp(clippedPolyTextureCoordsCurrent[p0Idx], clippedPolyTextureCoordsCurrent[p1Idx], alpha);
                ++clippedPolyVertexCount;
            }
        }

        public static int Clamp(int value, int minimum, int maximum)
        {
            if (value < minimum)
                return minimum;
            if (value <= maximum)
                return value;
            return maximum;
        }

        public static long Clamp(long value, long minimum, long maximum)
        {
            if (value < minimum)
                return minimum;
            if (value <= maximum)
                return value;
            return maximum;
        }

        public static int CeilLog2(long value)
        {
            var num1 = value;
            var num2 = 0;
            if (num1 >= 4294967296L)
            {
                num1 >>= 32;
                num2 += 32;
            }
            if (num1 >= 65536L)
            {
                num1 >>= 16;
                num2 += 16;
            }
            if (num1 >= 256L)
            {
                num1 >>= 8;
                num2 += 8;
            }
            if (num1 >= 16L)
            {
                num1 >>= 4;
                num2 += 4;
            }
            if (num1 >= 4L)
            {
                num1 >>= 2;
                num2 += 2;
            }
            if (num1 >= 2L)
                ++num2;
            return num2 + (value > 1L << num2 ? 1 : 0);
        }

        public static long DivPow2RoundUp(long value, int power) => DivRoundUp(value, 1L << power);

        public static long DivRoundUp(long value, long denominator) => (value + denominator - 1L) / denominator;

        public static double Clamp(double d, double min, double max)
        {
            if (double.IsNaN(d))
                return max;
            if (d < min)
                return min;
            if (d > max)
                return max;
            return d;
        }

        public static double NormalizeAngle(double angle) => (angle % 360.0 + 360.0) % 360.0;

        public static double AngleDelta(double angle1, double angle2)
        {
            var val1 = NormalizeAngle(angle1 - angle2);
            return Math.Min(val1, 360.0 - val1);
        }

        public static Point3D Add(Point3D point1, Point3D point2) => new Point3D(point1.X + point2.X, point1.Y + point2.Y, point1.Z + point2.Z);

        public static Point3D Subtract(Point3D point1, Point3D point2) => new Point3D(point1.X - point2.X, point1.Y - point2.Y, point1.Z - point2.Z);

        public static Point Add(Point point1, Point point2) => new Point(point1.X + point2.X, point1.Y + point2.Y);

        public static Point Subtract(Point point1, Point point2) => new Point(point1.X - point2.X, point1.Y - point2.Y);

        public static Point Multiply(Point point, double scalar) => new Point(point.X * scalar, point.Y * scalar);

        public static Point3D Multiply(Point3D point, double scalar) => new Point3D(point.X * scalar, point.Y * scalar, point.Z * scalar);

        public static double Distance(Point3D from, Point3D to) => GetLength(Subtract(to, from));

        public static double Distance(Point from, Point to) => GetLength(Subtract(to, from));

        public static double GetLength(Point3D point) => Math.Sqrt(point.X * point.X + point.Y * point.Y + point.Z * point.Z);

        public static double GetLength(Point point) => Math.Sqrt(point.X * point.X + point.Y * point.Y);

        public static Point3D Lerp(Point3D point1, Point3D point2, double weight) => new Point3D(point1.X + (point2.X - point1.X) * weight, point1.Y + (point2.Y - point1.Y) * weight, point1.Z + (point2.Z - point1.Z) * weight);

        public static Point3D Normalize(Point3D point)
        {
            var length = GetLength(point);
            if (length == 0.0)
                return point;
            var num = 1.0 / length;
            return new Point3D(point.X * num, point.Y * num, point.Z * num);
        }

        public static double Dot(Point3D point1, Point3D point2) => point1.X * point2.X + point1.Y * point2.Y + point1.Z * point2.Z;

        public static Point3D Cross(Point3D left, Point3D right) => new Point3D(left.Y * right.Z - left.Z * right.Y, left.Z * right.X - left.X * right.Z, left.X * right.Y - left.Y * right.X);

        public static Matrix Multiply(Matrix matrix1, Matrix matrix2) => new Matrix(matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21, matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22, matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21, matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22, matrix1.OffsetX * matrix2.M11 + matrix1.OffsetY * matrix2.M21 + matrix2.OffsetX, matrix1.OffsetX * matrix2.M12 + matrix1.OffsetY * matrix2.M22 + matrix2.OffsetY);

        public static Matrix Invert(Matrix matrix)
        {
            var num1 = matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21;
            if (num1 == 0.0)
                throw new InvalidOperationException("Matrix invert failed, determinant is 0.");
            var num2 = 1.0 / num1;
            return new Matrix(matrix.M22 * num2, -1.0 * matrix.M12 * num2, -1.0 * matrix.M21 * num2, matrix.M11 * num2, (matrix.OffsetY * matrix.M21 - matrix.OffsetX * matrix.M22) * num2, (matrix.OffsetX * matrix.M12 - matrix.OffsetY * matrix.M11) * num2);
        }

        public static Matrix InferTransform(Rect from, Rect to)
        {
            var num1 = (to.Right - to.X) / (from.Right - from.X);
            var num2 = (to.Bottom - to.Y) / (from.Bottom - from.Y);
            var identity = Matrix.Identity;
            identity.M11 = num1;
            identity.M22 = num2;
            identity.OffsetX = -num1 * from.X + to.X;
            identity.OffsetY = -num2 * from.Y + to.Y;
            return identity;
        }

        public static Matrix UnitToPoints(Point to1, Point to2, Point to3) => new Matrix(to2.X - to1.X, to2.Y - to1.Y, to3.X - to1.X, to3.Y - to1.Y, to1.X, to1.Y);

        public static Matrix Conversion(
          Point from1,
          Point from2,
          Point from3,
          Point to1,
          Point to2,
          Point to3) => Multiply(Invert(UnitToPoints(from1, from2, from3)), UnitToPoints(to1, to2, to3));

        public static Point3D Transform(Matrix3D matrix, Point3D point)
        {
            var num1 = point.X * matrix.M14 + point.Y * matrix.M24 + point.Z * matrix.M34 + matrix.M44;
            var num2 = num1 == 1.0 ? 1.0 : 1.0 / num1;
            return new Point3D((point.X * matrix.M11 + point.Y * matrix.M21 + point.Z * matrix.M31 + matrix.OffsetX) * num2, (point.X * matrix.M12 + point.Y * matrix.M22 + point.Z * matrix.M32 + matrix.OffsetY) * num2, (point.X * matrix.M13 + point.Y * matrix.M23 + point.Z * matrix.M33 + matrix.OffsetZ) * num2);
        }

        public static Point4D Transform(Matrix3D matrix, Point4D point) => new Point4D(point.X * matrix.M11 + point.Y * matrix.M21 + point.Z * matrix.M31 + point.W * matrix.OffsetX, point.X * matrix.M12 + point.Y * matrix.M22 + point.Z * matrix.M32 + point.W * matrix.OffsetY, point.X * matrix.M13 + point.Y * matrix.M23 + point.Z * matrix.M33 + point.W * matrix.OffsetZ, point.X * matrix.M14 + point.Y * matrix.M24 + point.Z * matrix.M34 + point.W * matrix.M44);

        public static Matrix3D TranslationMatrix3D(Point3D offset) => new Matrix3D(1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, offset.X, offset.Y, offset.Z, 1.0);

        public static Matrix3D TranslationMatrix3D(
          double offsetX,
          double offsetY,
          double offsetZ) => new Matrix3D(1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, offsetX, offsetY, offsetZ, 1.0);

        public static Matrix3D ScalingMatrix3D(double valueX, double valueY, double valueZ) => new Matrix3D(valueX, 0.0, 0.0, 0.0, 0.0, valueY, 0.0, 0.0, 0.0, 0.0, valueZ, 0.0, 0.0, 0.0, 0.0, 1.0);

        public static Matrix3D RotationMatrix3DX(double angle)
        {
            var m23 = Math.Sin(angle);
            var num = Math.Cos(angle);
            return new Matrix3D(1.0, 0.0, 0.0, 0.0, 0.0, num, m23, 0.0, 0.0, -m23, num, 0.0, 0.0, 0.0, 0.0, 1.0);
        }

        public static Matrix3D RotationMatrix3DY(double angle)
        {
            var m31 = Math.Sin(angle);
            var num = Math.Cos(angle);
            return new Matrix3D(num, 0.0, -m31, 0.0, 0.0, 1.0, 0.0, 0.0, m31, 0.0, num, 0.0, 0.0, 0.0, 0.0, 1.0);
        }

        public static Matrix3D RotationMatrix3DZ(double angle)
        {
            var m12 = Math.Sin(angle);
            var num = Math.Cos(angle);
            return new Matrix3D(num, m12, 0.0, 0.0, -m12, num, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0);
        }

        public static Matrix3D ConvertMatrixToMatrix3D(Matrix matrix) => new Matrix3D(matrix.M11, matrix.M12, 0.0, 0.0, matrix.M21, matrix.M22, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, matrix.OffsetX, matrix.OffsetY, 0.0, 1.0);

        public static Matrix3D GetAxisRedefinitionMatrix(
          Point3D xAxis,
          Point3D yAxis,
          Point3D zAxis) => new Matrix3D(xAxis.X, yAxis.X, zAxis.X, 0.0, xAxis.Y, yAxis.Y, zAxis.Y, 0.0, xAxis.Z, yAxis.Z, zAxis.Z, 0.0, 0.0, 0.0, 0.0, 1.0);

        public static Matrix3D ProjectOnPlane(Matrix3D projectToWorld, Plane3D plane)
        {
            var matrix3D = projectToWorld;
            var num1 = plane.A * matrix3D.M11 + plane.B * matrix3D.M12 + plane.C * matrix3D.M13 - plane.D * matrix3D.M14;
            var num2 = plane.A * matrix3D.M21 + plane.B * matrix3D.M22 + plane.C * matrix3D.M23 - plane.D * matrix3D.M24;
            var num3 = plane.A * matrix3D.M31 + plane.B * matrix3D.M32 + plane.C * matrix3D.M33 - plane.D * matrix3D.M34;
            var num4 = plane.A * matrix3D.OffsetX + plane.B * matrix3D.OffsetY + plane.C * matrix3D.OffsetZ - plane.D * matrix3D.M44;
            if (num3 == 0.0)
                throw new InvalidOperationException();
            var num5 = -1.0 / num3;
            return new Matrix3D(1.0, 0.0, num1 * num5, 0.0, 0.0, 1.0, num2 * num5, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, num4 * num5, 1.0) * projectToWorld;
        }

        public static Matrix3D PerspectiveMatrix3D(
          double fieldOfViewY,
          double aspectRatio,
          double zNearPlane,
          double zFarPlane)
        {
            var m22 = 1.0 / Math.Tan(fieldOfViewY / 2.0);
            var m11 = m22 / aspectRatio;
            var num = zFarPlane - zNearPlane;
            return new Matrix3D(m11, 0.0, 0.0, 0.0, 0.0, m22, 0.0, 0.0, 0.0, 0.0, zFarPlane / num, 1.0, 0.0, 0.0, -zNearPlane * zFarPlane / num, 0.0);
        }

        public static Point[] GetCorners(Rect rect) => new Point[4] { new Point(rect.X, rect.Y), new Point(rect.Right, rect.Y), new Point(rect.Right, rect.Bottom), new Point(rect.X, rect.Bottom) };

        public static Point3D IntersectLineToPlane(
      Point3D linePoint1,
      Point3D linePoint2,
      Plane3D plane)
        {
            var point = Subtract(linePoint2, linePoint1);
            var num1 = plane.D - plane.A * linePoint1.X - plane.B * linePoint1.Y - plane.C * linePoint1.Z;
            var num2 = plane.A * point.X + plane.B * point.Y + plane.C * point.Z;
            if (num2 == 0.0)
                throw new InvalidOperationException();
            var scalar = num1 / num2;
            return Add(linePoint1, Multiply(point, scalar));
        }

        public static double LinePointDistanceSquared(
          Point2D line0,
          Point2D line1,
          Point2D point,
          out bool inLineSegment)
        {
            var num = ((point.X - line0.X) * (line1.X - line0.X) + (point.Y - line0.Y) * (line1.Y - line0.Y)) / Point2D.DistanceSquared(line0, line1);
            inLineSegment = num >= 0.0 && num <= 1.0;
            return Point2D.DistanceSquared(new Point2D(line0.X + num * (line1.X - line0.X), line0.Y + num * (line1.Y - line0.Y)), point);
        }

        public static void Swap<T>(ref T left, ref T right)
        {
            var obj = left;
            left = right;
            right = obj;
        }

        public static Uri TileSourceGetUriWrapper(MapControl.WPF.TileSource tileSource, TileId tileId) => tileSource.GetUri((int)tileId.X, (int)tileId.Y, tileId.LevelOfDetail - 8);

        public static bool OrientedBoundingBoxIntersectsAxisAlignedBoundingBox(
      Point2D orientedBBox0,
      Point2D orientedBBox1,
      double orientedBBoxWidth,
      Rect axisAlignedBBox)
        {
            if (orientedBBoxWidth <= 0.0)
                throw new ArgumentException("box must have positive width");
            var point2D1 = Point2D.Normalize(orientedBBox1 - orientedBBox0);
            var point2D2 = orientedBBoxWidth * 0.5 * point2D1;
            var point2D3 = new Point2D(-point2D2.Y, point2D2.X);
            var point2DArray = new Point2D[2][] { new Point2D[4] { orientedBBox0 + point2D3 - point2D2, orientedBBox1 + point2D3 + point2D2, orientedBBox1 - point2D3 + point2D2, orientedBBox0 - point2D3 - point2D2 }, new Point2D[4] { new Point2D(axisAlignedBBox.Left, axisAlignedBBox.Top), new Point2D(axisAlignedBBox.Right, axisAlignedBBox.Top), new Point2D(axisAlignedBBox.Right, axisAlignedBBox.Bottom), new Point2D(axisAlignedBBox.Left, axisAlignedBBox.Bottom) } };
            var left = point2DArray[0];
            var right1 = point2DArray[1];
            for (var index1 = 0; index1 < 1; ++index1)
            {
                var point2D4 = left[1] - left[0];
                var right2 = left[3] - left[0];
                var right3 = 1.0 / point2D4.LengthSquared() * point2D4;
                right2 = 1.0 / right2.LengthSquared() * right2;
                var num1 = Point2D.Dot(left[0], right3);
                var num2 = Point2D.Dot(left[0], right2);
                for (var index2 = 0; index2 < 2; ++index2)
                {
                    var right4 = index2 == 0 ? right3 : right2;
                    var num3 = index2 == 0 ? num1 : num2;
                    var num4 = double.MaxValue;
                    var num5 = double.MinValue;
                    var num6 = Point2D.Dot(right1[0], right4);
                    if (num6 < num4)
                        num4 = num6;
                    if (num6 > num5)
                        num5 = num6;
                    var num7 = Point2D.Dot(right1[1], right4);
                    if (num7 < num4)
                        num4 = num7;
                    if (num7 > num5)
                        num5 = num7;
                    var num8 = Point2D.Dot(right1[2], right4);
                    if (num8 < num4)
                        num4 = num8;
                    if (num8 > num5)
                        num5 = num8;
                    var num9 = Point2D.Dot(right1[3], right4);
                    if (num9 < num4)
                        num4 = num9;
                    if (num9 > num5)
                        num5 = num9;
                    if (num4 - num3 > 1.0 || num5 - num3 < 0.0)
                        return false;
                }
                Swap(ref left, ref right1);
            }
            return true;
        }
    }
}
