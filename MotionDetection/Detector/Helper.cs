/*
 * ---------------------------------------------------
 *      Copyright 2010 Mitchel Collins - XiaTek
 *                http://XiaTek.org
 *     Released Open Source under the BSD License
 *     Please report any bugs to c0bra@xiatek.org
 * ---------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using Detector.Motion;
using Detector.Tracking;

namespace Detector.Helper
{

    public class ImageHelpers
    {
        /// <summary>
        /// Provides some nice functions that can save you a lot of time
        /// </summary>
        public ImageHelpers()
        {

        }
        /// <summary>
        /// Blurs an image with another image with a multiplier
        /// </summary>
        /// <param name="imagetoblur">The image that will be modified</param>
        /// <param name="current">The current image to blur with</param>
        /// <param name="ammount">Blur over how many frames</param>
        public unsafe void MotionBlur(ref Bitmap imagetoblur, ref Bitmap current, int ammount)
        {
            BitmapData blur = imagetoblur.LockBits(new Rectangle(0, 0, imagetoblur.Width, imagetoblur.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData cur = current.LockBits(new Rectangle(0, 0, current.Width, current.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            byte* row_current;
            byte* row_blur;
            int R, G, B;
            for (int y = 0; y < cur.Height; y++)
                for (int x = 0; x < cur.Width; x++)
                {
                    row_current = (byte*)cur.Scan0.ToPointer() + (cur.Stride * y);
                    row_blur = (byte*)blur.Scan0.ToPointer() + (blur.Stride * y);
                    B = x * 3 + 0;
                    G = x * 3 + 1;
                    R = x * 3 + 2;


                    row_blur[R] = (byte)(row_blur[R] + (byte)Math.Ceiling( ((double)row_current[R] - (double)row_blur[R]) / (double)ammount ));
                    row_blur[G] = (byte)(row_blur[G] + (byte)Math.Ceiling(((double)row_current[G] - (double)row_blur[G]) / (double)ammount));
                    row_blur[B] = (byte)(row_blur[B] + (byte)Math.Ceiling(((double)row_current[B] - (double)row_blur[B]) / (double)ammount));
                }
            imagetoblur.UnlockBits(blur);
            current.UnlockBits(cur);
        }
        /// <summary>
        /// Draw a box around the co-ords with a color
        /// </summary>
        /// <param name="X">X</param>
        /// <param name="Y">Y</param>
        /// <param name="width">Width of the box</param>
        /// <param name="height">Height of the box</param>
        /// <param name="bmp">The bitmap to draw to</param>
        /// <param name="col">Color to draw with</param>
        public void DrawBox(int X, int Y, int width, int height, ref Bitmap bmp, Color col, bool fill)
        {
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            if (!fill)
            {
                for (int x = X + 1; x < X + width; x++)
                {
                    SetPixel(ref bmp_data, x, Y, col);
                    SetPixel(ref bmp_data, x, Math.Min(bmp.Height - 1, Y + height), col);
                }
                for (int y = Y; y < Y + height + 1; y++)
                {
                    SetPixel(ref bmp_data, X, y, col);
                    SetPixel(ref bmp_data, Math.Min(bmp.Width - 1, X + width), y, col);
                }
            }
            else
            {
                for (int x = X; x < X + width; x++)
                    for (int y = Y; y < Y + height; y++)
                        SetPixel(ref bmp_data, x, y, col);
            }
            bmp.UnlockBits(bmp_data);
        }
        /// <summary>
        /// Draw a box around a target
        /// </summary>
        /// <param name="t">The target to draw a box around</param>
        /// <param name="bmp">The bitmap to draw to</param>
        /// <param name="col">Color</param>
        public void DrawBox(Target t, ref Bitmap bmp, Color col)
        {
            DrawBox(t.X, t.Y, t.SizeX, t.SizeY, ref bmp, col, false);
        }
        /// <summary>
        /// Draw a box around an object
        /// </summary>
        /// <param name="obj">Object to draw around</param>
        /// <param name="bmp">The bitmap to draw to</param>
        /// <param name="col">Color</param>
        public void DrawBox(ObjectTracked obj, ref Bitmap bmp, Color col)
        {
            DrawBox(obj.Position.X, obj.Position.Y, obj.Size.X, obj.Size.Y, ref bmp, col, false);
        }
        /// <summary>
        /// Sets the pixel to a cetain color (Aplha works)
        /// </summary>
        /// <param name="bmp">The bitmap data</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="col">Color to draw with</param>
        public unsafe void SetPixel(ref BitmapData bmp, int x, int y, Color col)
        {
            if (x >= bmp.Width || x <= 0 || y >= bmp.Height || y <= 0)
                return;
            byte* row = (byte*)bmp.Scan0.ToPointer() + (bmp.Stride * y);
            int B = x * 3 + 0;
            int G = x * 3 + 1;
            int R = x * 3 + 2;

            float a = Math.Max(0, Math.Min(255, ((float)col.A / (float)255)));
            row[R] = (byte)Math.Min(255, row[R] + col.R * a); // a is a value from 0 to 1
            row[G] = (byte)Math.Min(255, row[G] + col.G * a);
            row[B] = (byte)Math.Min(255, row[B] + col.B * a);
        }
    }

}