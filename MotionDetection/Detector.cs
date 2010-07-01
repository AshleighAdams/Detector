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
using System.Drawing;
using System.Drawing.Imaging;


namespace Detector
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

    /// <summary>
    /// The engine in which detects the motion and put it in an easy to use format
    /// </summary>
    public class Detector
    {
        /// <summary>
        /// Compare to images to one another
        /// </summary>
        /// <param name="last_img">The previous image</param>
        /// <param name="cur_img">The current image</param>
        public Detector(Image last_img, Image cur_img)
        {
            _last_img = last_img;
            _cur_img = cur_img;
        }
        /// <summary>
        /// The engine in which detects the motion and put it in an easy to use format
        /// </summary>
        public Detector()
        {
            
        }

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
         *          are we not in range of another target
         *              create a new target
         *          else
         *              if required expand the target X/Y/SizeX/SizeY
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

            // Pixels now contain where there is movement in x,y
            Target[] targs = new Target[10];
            int targ;
            int movement;
            for (int y = 0; y < cur_img.Height; y++)    // yes this is messy, look above at the pesudo code for how and what this is doing
                for (int x = 0; x < cur_img.Width; x++)
                {
                    movement = pixels[x, y];
                    if (movement == 1)
                    {
                        targ = InRangeOfTarget(ref targs, x, y);
                        if (targ == -1)
                        {
                            if (NotInsideOfTarget(ref targs, x, y))
                            {
                                for (int j = 0; j < targs.Length; j++)
                                {
                                    if (targs[j] == null)
                                    {
                                        targs[j] = new Target(x, y, 1, 1);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (x < targs[targ].X)
                                targs[targ].X = x;
                            if (y < targs[targ].Y)
                                targs[targ].Y = y;
                            if (x > targs[targ].SizeX + targs[targ].X)
                                targs[targ].SizeX = x - targs[targ].X;
                            if (y > targs[targ].SizeY + targs[targ].Y)
                                targs[targ].SizeY = y - targs[targ].Y;
                        }
                    }
                }

                foreach (Target t in targs)
                {
                    if (t == null || t.SizeX < _noisereduction || t.SizeY < _noisereduction)
                        break;
                    yield return t;
                }
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
                foreach (Target t in targs)
                {
                    if (t == null)
                        break;
                    if (                        // TODO: Add the end of the box check too
                        t.X > x &&
                        t.Y > y &&
                        x < t.X + t.SizeX &&
                        y < t.Y + t.SizeY)
                    {
                        return false;
                    }
                }
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
        private int InRangeOfTarget(ref Target[] targs, int x, int y)
        {
            try
            {
                int i = 0;
                foreach (Target t in targs)
                {
                    if (t == null) // we should only get here if there were no targets matched
                        return -1;
                    // Check distance from cornders and that its not near the edge blah blah so on...
                    if (Distance(x, y, t.X, t.Y) < _maxjoindistance ||                    // are we inrange of the other target?
                        Distance(x, y, t.SizeX + t.X, t.SizeY + t.Y) < _maxjoindistance ||
                        Distance(x, y, t.SizeX + t.X, t.Y) < _maxjoindistance ||
                        Distance(x, y, t.X, t.SizeY + t.Y) < _maxjoindistance ||
                            (
                                x < t.X + t.SizeX + _maxjoindistance &&
                                y < t.Y + t.SizeY + _maxjoindistance &&
                                x > t.X - _maxjoindistance &&
                                y > t.Y - _maxjoindistance
                            )
                        )
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
        private int _noisereduction = 5;
        private int _maxjoindistance = 30;
        private int _Difference = 50;
        private Image _last_img = null;
        private Image _cur_img = null;
        private byte[,] IgnoreMotionPixels;
        private Image _ignoremotion;
        #endregion

        private void Error(string error)
        {
            #if DEBUG
            throw new Exception(error);
            #endif
        }
    }
}
