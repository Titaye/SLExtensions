#region Header

//---------------------------------------------------------------------
// <copyright file="MatrixHelper.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//---------------------------------------------------------------------

#endregion Header

namespace SLExtensions.Manipulation
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Adds transformation methods to the Matrix class.
    /// </summary>
    public static class MatrixHelper
    {
        #region Methods

        /// <summary>
        /// Rotates the given matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="angle"></param>
        public static void Rotate(ref Matrix matrix, double angle)
        {
            Matrix rotationMatrix = CreateRotationMatrix(angle, 0, 0);
            matrix = Multiply(ref matrix, ref rotationMatrix);
        }

        /// <summary>
        /// Rotates the given matrix around the specified center.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="angle"></param>
        /// <param name="center"></param>
        public static void RotateAt(ref Matrix matrix, double angle, Point center)
        {
            Matrix rotationMatrix = CreateRotationMatrix(angle, center.X, center.Y);
            matrix = Multiply(ref matrix, ref rotationMatrix);
        }

        /// <summary>
        /// Translates the given matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="translateX"></param>
        /// <param name="translateY"></param>
        public static void Translate(ref Matrix matrix, double translateX, double translateY)
        {
            matrix.OffsetX += translateX;
            matrix.OffsetY += translateY;
        }

        /// <summary>
        /// Creates a rotation matrix.
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <returns></returns>
        private static Matrix CreateRotationMatrix(double degrees, double centerX, double centerY)
        {
            double radians = degrees * Math.PI / 180;
            double sin = Math.Sin(radians);
            double cos = Math.Cos(radians);
            double offsetX = (centerX * (1.0 - cos)) + (centerY * sin);
            double offsetY = (centerY * (1.0 - cos)) - (centerX * sin);
            Matrix matrix = new Matrix(cos, sin, -sin, cos, offsetX, offsetY);
            return matrix;
        }

        /// <summary>
        /// Multiplies matrices.
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        private static Matrix Multiply(ref Matrix matrix1, ref Matrix matrix2)
        {
            return new Matrix((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21),
                (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22),
                (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21),
                (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22),
                ((matrix1.OffsetX * matrix2.M11) + (matrix1.OffsetY * matrix2.M21)) + matrix2.OffsetX,
                ((matrix1.OffsetX * matrix2.M12) + (matrix1.OffsetY * matrix2.M22)) + matrix2.OffsetY);
        }

        #endregion Methods
    }
}