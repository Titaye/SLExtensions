namespace SLExtensions
{
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

    public struct Vector
    {
        #region Constructors

        public Vector(double x, double y)
            : this()
        {
            X = x;
            Y = y;
        }

        #endregion Constructors

        #region Properties

        public double Length
        {
            get
            {
                return Math.Sqrt(LengthSquared);
            }
        }

        public double LengthSquared
        {
            get
            {
                return (this.X * this.X) + (this.Y * this.Y);
            }
        }

        public double X
        {
            get; set;
        }

        public double Y
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public static explicit operator Size(Vector vector)
        {
            return new Size(Math.Abs(vector.X), Math.Abs(vector.Y));
        }

        public static explicit operator Point(Vector vector)
        {
            return new Point(vector.X, vector.Y);
        }

        public static bool operator !=(Vector vector1, Vector vector2)
        {
            return !(vector1 == vector2);
        }

        public static Vector operator *(Vector vector, double scalar)
        {
            return new Vector(vector.X * scalar, vector.Y * scalar);
        }

        public static Vector operator *(double scalar, Vector vector)
        {
            return new Vector(vector.X * scalar, vector.Y * scalar);
        }

        public static double operator *(Vector vector1, Vector vector2)
        {
            return ((vector1.X * vector2.X) + (vector1.Y * vector2.Y));
        }

        public static Vector operator +(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.X + vector2.X, vector1.Y + vector2.Y);
        }

        public static Point operator +(Vector vector, Point point)
        {
            return new Point(point.X + vector.X, point.Y + vector.Y);
        }

        public static Vector operator -(Vector vector)
        {
            return new Vector(-vector.X, -vector.Y);
        }

        public static Vector operator -(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.X - vector2.X, vector1.Y - vector2.Y);
        }

        public static Vector operator /(Vector vector, double scalar)
        {
            return (Vector)(vector * (1.0 / scalar));
        }

        public static bool operator ==(Vector vector1, Vector vector2)
        {
            return ((vector1.X == vector2.X) && (vector1.Y == vector2.Y));
        }

        public static Vector Add(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.X + vector2.X, vector1.Y + vector2.Y);
        }

        public static Point Add(Vector vector, Point point)
        {
            return new Point(point.X + vector.X, point.Y + vector.Y);
        }

        public static double AngleBetween(Vector vector1, Vector vector2)
        {
            double y = (vector1.X * vector2.Y) - (vector2.X * vector1.Y);
            double x = (vector1.X * vector2.X) + (vector1.Y * vector2.Y);
            return (Math.Atan2(y, x) * 57.295779513082323);
        }

        public static double CrossProduct(Vector vector1, Vector vector2)
        {
            return ((vector1.X * vector2.Y) - (vector1.Y * vector2.X));
        }

        public static double Determinant(Vector vector1, Vector vector2)
        {
            return ((vector1.X * vector2.Y) - (vector1.Y * vector2.X));
        }

        public static Vector Divide(Vector vector, double scalar)
        {
            return (Vector)(vector * (1.0 / scalar));
        }

        public static bool Equals(Vector vector1, Vector vector2)
        {
            return (vector1.X.Equals(vector2.X) && vector1.Y.Equals(vector2.Y));
        }

        public static Vector Multiply(Vector vector, double scalar)
        {
            return new Vector(vector.X * scalar, vector.Y * scalar);
        }

        public static Vector Multiply(double scalar, Vector vector)
        {
            return new Vector(vector.X * scalar, vector.Y * scalar);
        }

        public static double Multiply(Vector vector1, Vector vector2)
        {
            return ((vector1.X * vector2.X) + (vector1.Y * vector2.Y));
        }

        public static Vector Subtract(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.X - vector2.X, vector1.Y - vector2.Y);
        }

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is Vector))
            {
                return false;
            }
            Vector vector = (Vector)o;
            return Equals(this, vector);
        }

        public bool Equals(Vector value)
        {
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            return (this.X.GetHashCode() ^ this.Y.GetHashCode());
        }

        public void Negate()
        {
            this.X = -this.X;
            this.Y = -this.Y;
        }

        public void Normalize()
        {
            this = (Vector)(this / Math.Max(Math.Abs(this.X), Math.Abs(this.Y)));
            this = (Vector)(this / this.Length);
        }

        #endregion Methods
    }
}