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

namespace Detector.Motion
{
    /// <summary>
    /// Data of a target
    /// </summary>
    public class Target
    {
        /// <summary>
        /// Set the infomation for the target
        /// </summary>
        /// <param name="x">Postion X the target starts</param>
        /// <param name="y">Postion Y the target starts</param>
        /// <param name="size_x">The width of the target</param>
        /// <param name="size_y">The length of the target</param>
        public Target(int x, int y, int size_x, int size_y) 
        {
            this._x = x;
            this._y = y;
            this._size_x = size_x;
            this._size_y = size_y;
        }
        public byte[,] Shape;
        public void ShapeDetection()
        {
        }
        

        #region Properties
        public int X
        {
            get
            {
                return this._x;
            }
            set
            {
                this._x = value;
            }
        }
        public int Y
        {
            get
            {
                return this._y;
            }
            set
            {
                this._y = value;
            }
        }
        public int SizeX
        {
            get
            {
                return this._size_x;
            }
            set
            {
                this._size_x = value;
            }
        }
        public int SizeY
        {
            get
            {
                return this._size_y;
            }
            set
            {
                this._size_y = value;
            }
        }
        #endregion
        private int _x = 0;
        private int _y = 0;
        private int _size_x = 0;
        private int _size_y = 0;
    }

    public class MotionHelper
    {
        public int Height, Width;
        public int MinX, MinY, MaxX, MaxY;
        public byte[,] Motion;
        public byte[,] Shape;
        public MotionHelper(byte[,] motion, int width, int height, int seedx, int seedy)
        {
            Height = height;
            Width = width;
            Motion = motion;
            MinX = seedx;
            MaxX = seedx;
            MinY = seedy;
            MaxY = seedy;
        }
    }

    /// <summary>
    /// The engine in which detects the motion and put it in an easy to use format
    /// </summary>
    public class MotionDetector
    {
        /// <summary>
        /// Recursave function for scanlines
        /// </summary>
        public void DoNextScanLine(int x, int y, ref MotionHelper motion)
        {

            //   2 3 4
            // 2 O O O  
            // 3 O X O  
            // 4 O O O
            if (x >= motion.Width || y >= motion.Height)
                return;
            if (motion.Motion[x, y] == 0)
                return;         

            if (x > motion.MaxX)
                motion.MaxX = x;
            if (x < motion.MinX)
                motion.MinX = x;
            if (y > motion.MaxY)
                motion.MaxY = y;
            if (y < motion.MinY)
                motion.MinY = y;

            int y1;

            //draw current scanline from start position to the top
            y1 = y;
            while (y1 < motion.Height && motion.Motion[x, y1] == 1)
            {
                motion.Motion[x, y1] = 2;
                if (y1 > motion.MaxY)
                    motion.MaxY = y1;
                y1++;
            }

            //draw current scanline from start position to the bottom
            y1 = y - 1;
            while (y1 >= 0 && motion.Motion[x, y1] == 1)
            {
                motion.Motion[x, y1] = 2;
                if (y1 < motion.MinY)
                    motion.MinY = y1;
                y1--;
            }

            //test for new scanlines to the left
            y1 = y;
            while (y1 < motion.Height && (motion.Motion[x, y1] == 2))
            {
                
                if (x > 0 && motion.Motion[x -1, y1] == 1)
                {
                    DoNextScanLine(x - 1, y1, ref motion);
                }
                y1++;
            }
            y1 = y - 1;
            while (y1 >= 0 && (motion.Motion[x, y1] == 2))
            {
                if (x > 0 && motion.Motion[x - 1, y1] == 1)
                {
                    DoNextScanLine(x - 1, y1, ref motion);
                }
                y1--;
            }

            //test for new scanlines to the right 
            y1 = y;
            while (y1 < motion.Height && (motion.Motion[x, y1] == 2))
            {
                if (x < motion.Width - 1 && motion.Motion[x + 1, y1] == 1)
                {
                    DoNextScanLine(x + 1, y1, ref motion);
                }
                y1++;
            }
            y1 = y - 1;
            while (y1 >= 0 && (motion.Motion[x, y1] == 2))
            {
                if (x < motion.Width - 1 && motion.Motion[x + 1, y1] == 1)
                {
                    DoNextScanLine(x + 1, y1, ref motion);
                }
                y1--;
            }
        }

        /// <summary>
        /// Uses a FloodFill scanline algorithm to get a targets bounds
        /// </summary>
        public MotionHelper GetBoundsFromMotion(ref byte[,] motion, Point size, Point seed)
        {
            MotionHelper helper = new MotionHelper(motion, size.X, size.Y, seed.X, seed.Y);
            DoNextScanLine(seed.X, seed.Y, ref helper); // +2 and +1 are for creating an empty  box around the whole shape, (1's never at the end /start of an array)
            helper.Shape = new byte[helper.MaxX - helper.MinX + 4, helper.MaxY - helper.MinY + 4];

            for (int x = helper.MinX; x < helper.MaxX; x++)
                for (int y = helper.MinY; y < helper.MaxY; y++)
                    if (helper.Motion[x, y] == 2)
                    {
                        helper.Shape[x - helper.MinX + 2, y - helper.MinY + 2] = 1;
                    }

            return helper; 
        }

        /// <summary>
        /// Compare to images to one another
        /// </summary>
        /// <param name="last_img">The previous image</param>
        /// <param name="cur_img">The current image</param>
        public MotionDetector(Image last_img, Image cur_img)
        {
            _last_img = last_img;
            _cur_img = cur_img;
        }
        /// <summary>
        /// The engine in which detects the motion and put it in an easy to use format
        /// </summary>
        public MotionDetector()
        {
            
        }
        public Bitmap motionpic;
        // This is the main part of the algorithm, I supplied some pesudo code to help you understand it
        /// <summary>
        /// Get the current target
        /// </summary>
        /// <returns>All the current targets found</returns>
        #region Pesudo Code
        /*
         * Get motion into new image motion
         * for each x and y do
         *      does the current pixel contain motion
         *          plant a seed and get target size
         *          is it in the middle of another target? if not
         *              create a new target
         *          end
         *      end
         * end
         * for each target
         *      if our size is less than a predefined value then
         *          break
         *      add target to the returns
         * end
         */
        #endregion
        
        public IEnumerable<Target> GetTargets()
        {
            
            Bitmap cur_img = new Bitmap(_cur_img);
            Bitmap last_img = new Bitmap(_last_img);
            // Data will be copied into here for each X and Y
            byte[,] pixels = new byte[cur_img.Width, cur_img.Height];

            BitmapData curimg = cur_img.LockBits(
                new Rectangle(0, 0, cur_img.Width, cur_img.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData lastimg = last_img.LockBits(
                new Rectangle(0, 0, last_img.Width, last_img.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            Bitmap motion = cur_img;    // derive motion from the image that will be used for the motion pixels then copy the data into it
            motion.UnlockBits(SubtractPixels(curimg, lastimg, ref pixels));
            last_img.UnlockBits(lastimg);   // free it
            motionpic = motion;
            // Pixels now contain where there is movement in x,y
            LinkedList<Target> targs = new LinkedList<Target>();
            for (int y = 0; y < cur_img.Height; y++)    // yes this is messy, look above at the pesudo code for how and what this is doing
                for (int x = 0; x < cur_img.Width; x++)
                {
                    int movement = pixels[x, y];
                    if (movement == 1)
                    {
                        MotionHelper helper = this.GetBoundsFromMotion(ref pixels,
                            new Point(motion.Width, motion.Height),
                            new Point(x, y));
                        
                        Target newtarg = new Target(helper.MinX, helper.MinY,
                            helper.MaxX - helper.MinX,
                            helper.MaxY - helper.MinY);
                        
                        newtarg.Shape = helper.Shape;
                        newtarg.ShapeDetection();

                        float scalex = newtarg.SizeX / _noisereduction;
                        float scaley = newtarg.SizeY / _noisereduction;
                        if (scalex + scaley > 1.75f)
                            targs.AddLast(newtarg);
                        else
                            _BadTargets++;
                    }
                }
            return targs;
        }

        /// <summary>
        /// Is the target inside of a specific X and Y
        /// </summary>
        /// <param name="targs">Target Array</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns></returns>
        private bool NotInsideOfTarget(ref Target[] targs, int x, int y)
        {
            try
            {
                int res = InRangeOfTarget(ref targs, x, y);
                return (res == -1);
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
            return true;
        }

        /// <summary>
        /// Is the pixel inside of the ignore position?
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns></returns>
        private bool IsInsideIgnoreArea(int x, int y)
        {
            if (IgnoreMotionPixels == null || _ignoremotion == null)    // Might not be used so these will be null
                return false;
            if (x >= _ignoremotion.Width ||
                y >= _ignoremotion.Height)
                return false;
            return (IgnoreMotionPixels[x,y] == 1);
        }

        /// <summary>
        /// Checks to see if movement pixels are in range of a target
        /// </summary>
        /// <param name="targs">Target array</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns></returns>
        private int InRangeOfTarget(ref Target[] targs, int x, int y) ////// This is a bottle neck!!! Speed me up.
        {
            try
            {
                int i = 0;
                foreach (Target t in targs)
                {
                    if (t == null) // we should only get here if there were no targets matched
                        return -1;
                    // Check distance from cornders and that its not near the edge blah blah so on...
                    int X = t.X - _maxjoindistance;
                    int Y = t.Y - _maxjoindistance;
                    int eX = X + t.SizeX + 2 * _maxjoindistance;
                    int eY = Y + t.SizeY + 2 * _maxjoindistance;

                    if (x > X && x < eX &&
                        y > Y && y < eY)
                    {
                        return i;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
            return -1;
        }

        /// <summary>
        /// Calculate the distance between to points
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="a">X</param>
        /// <param name="b">Y</param>
        /// <returns></returns>
        private int Distance(int x, int y, int a, int b)
        {
            int xx = Math.Abs( x - a );
            int yy = Math.Abs( y - b );
            return (int)Math.Sqrt(xx * xx + yy * yy);
        }

        // this function was used on the recursave algorithm but i changed from that to enable multipul tracking, new one is much better and this feater is here for if anyone wants it
        /// <summary>
        /// Gets the average color in a cerain area
        /// </summary>
        /// <param name="bdata">Bitmap data</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <returns>Average greyscale color</returns>
        private unsafe int GetAverage(BitmapData bdata,int x, int y, int width, int height)
        {
            
            long tot = 0;
            long sum = 0;

            width = Math.Min(bdata.Width-x, width+x);
            height = Math.Min(bdata.Height-y, height+y);
            /*
            bmp = new Bitmap(img);
            bdata = bmp.LockBits(new Rectangle(x, y, width, height),
                ImageLockMode.ReadOnly, 
                PixelFormat.Format24bppRgb);

            */
            byte* row;
            byte blue;
            byte green;
            byte red;
            for (int Y = y; Y < height+y; Y++)
                for (int X = x; X < width+x; X++) 
                {
                    row = (byte*)bdata.Scan0.ToPointer() + (bdata.Stride * Y);

                    blue = row[X * 3 + 0];     // X, Pixel size, + 0/1/2 for color BGR
                    green = row[X * 3 + 1];
                    red = row[X * 3 + 2];

                    sum += (long)(red + green + blue);
                    tot++;
                };
            
            return (int)(sum/tot);
        }

        /// <summary>
        /// Subtract any pixels that havent changed, set those that have to 255
        /// </summary>
        /// <param name="curimg">Current image</param>
        /// <param name="lastimg">Last image</param>
        /// <param name="pixeldata">2 Dem byte array to place movement</param>
        /// <returns></returns>
        public unsafe BitmapData SubtractPixels(BitmapData curimg, BitmapData lastimg, ref byte[,] pixeldata)
        {
            // any pixels that differ over a certain ammount will be set to white, else black
            if (curimg.Height != lastimg.Height || curimg.Width != lastimg.Width)
                Error("Image sizes are not the same");
            if (curimg.PixelFormat != lastimg.PixelFormat)
                Error("PixelFormat is not the same");
            try
            {
                byte* row_current;
                byte* row_last;
                int R, G, B;
                byte r, g, b, avg;
                for (int y = 0; y < curimg.Height; y++)
                    for (int x = 0; x < curimg.Width; x++)
                    {
                        row_current = (byte*)curimg.Scan0.ToPointer() + (curimg.Stride * y);
                        row_last = (byte*)lastimg.Scan0.ToPointer() + (lastimg.Stride * y);
                        B = x * 3 + 0;
                        G = x * 3 + 1;
                        R = x * 3 + 2;

                        b = (byte)Math.Abs(row_last[B] - row_current[B]);
                        g = (byte)Math.Abs(row_last[G] - row_current[G]);
                        r = (byte)Math.Abs(row_last[R] - row_current[R]);

                        avg = (byte)((r + g + b) / 3);

                        if ((r > _Difference ||
                            g > _Difference ||
                            b > _Difference ) &&
                            IsInsideIgnoreArea(x, y) == false
                            )
                        {
                            row_current[B] = 255;
                            row_current[G] = 255;
                            row_current[R] = 255;
                            pixeldata[x, y] = 1;
                        }
                        else
                        {
                            row_current[B] = 0;
                            row_current[G] = 0;
                            row_current[R] = 0;
                            pixeldata[x, y] = 0;
                        }
                    };
            }
            catch (Exception ex) // We dont want to halt any execution...
            {
                Error(ex.Message);
            }
            return curimg;
        }
        
        #region Properties
        /// <summary>
        /// The accuracy of detecting motion (helps reduce noise)
        /// </summary>
        public int Difference
        {
            get
            {
                return this._Difference;
            }
            set
            {
                this._Difference = value;
            }
        }
        private int _BadTargets;
        /// <summary>
        /// Ammount of "small" targets have been removed
        /// </summary>
        public int BadTargets
        {
            get { return _BadTargets; }
        }
        /// <summary>
        /// White pixels represent areas to ignore motion
        /// </summary>
        public Image IgnoreMotion
        {
            get
            {
                return _ignoremotion;
            }
            set
            {
                _ignoremotion = value;

                UpdateIgnoreMotion();
            }
        }
        /// <summary>
        /// Set the next images to compare
        /// </summary>
        /// <param name="Last">Image to compare to</param>
        /// <param name="Current">Image at this moment in time</param>
        public void SetNextImages(Image Last, Image Current)
        {
            if (Last.Width != Current.Width || Last.Height != Current.Height)
                Error("Image sizes differ");
            _BadTargets = 0;
            _last_img = Last;
            _cur_img = Current;
        }

        /// <summary>
        /// The ammount to reduce noise by (removes targets smaller than this size)
        /// </summary>
        public int NoiseReduction
        {
            get
            {
                return _noisereduction;
            }
            set
            {
                _noisereduction = value;
            }
        }
        /// <summary>
        /// The maximum distance to join seperate targets together
        /// </summary>
        public int MaxJoinDistance
        {
            get
            {
                return _maxjoindistance;
            }
            set
            {
                _maxjoindistance = value;
            }
        }

        /// <summary>
        /// Set the byte array up for fast access to thes pixels
        /// </summary>
        public unsafe void UpdateIgnoreMotion()
        {
            try
            {
                Bitmap bmp = new Bitmap(_ignoremotion);
                BitmapData pixels = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                byte* row;
                int B;
                int G;
                int R;
                IgnoreMotionPixels = new byte[bmp.Width, bmp.Height];
                for (int y = 0; y < bmp.Height; y++)
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        row = (byte*)pixels.Scan0.ToPointer() + (pixels.Stride * y);
                        B = x * 3 + 0;
                        G = x * 3 + 1;
                        R = x * 3 + 2;
                        if (row[B] == 255 ||
                            row[R] == 255 ||
                            row[G] == 255)
                            IgnoreMotionPixels[x, y] = 1;
                        else
                            IgnoreMotionPixels[x, y] = 0;
                    }

                bmp.UnlockBits(pixels);
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }
        private int _noisereduction = 2;
        private int _maxjoindistance = 10; // TODO REMOVE REFRENCES
        private int _Difference = 50;
        private Image _last_img = null;
        private Image _cur_img = null;
        private byte[,] IgnoreMotionPixels;
        private Image _ignoremotion;
        #endregion
        /// <summary>
        /// Error without halting on release builds
        /// </summary>
        /// <param name="error">String for the exception</param>
        private void Error(string error)
        {
            #if DEBUG
            //throw new Exception(error);
            #endif
        }
    }
}
