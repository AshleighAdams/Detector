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
using Detector.Motion;
using Detector.Tracking;

namespace Detector.Motion
{
    class PathDirection
    {
        private double Sin(int ang)
        {
            double ret = Math.PI * (double)ang / 180.0;
            return Math.Sin(ret);
        }
        private double Cos(int ang)
        {
            double ret = Math.PI * (double)ang / 180.0;
            return Math.Cos(ret);
        }

        public int X = 0;
        public int Y = 0;
        public int _CurrentAngle = 0;
        public int CurrentAngle
        {
            get { return _CurrentAngle; }
            set{_CurrentAngle = value % 360;}
        }
        public PathDirection(int x, int y)
        {
            X = x; Y = y;
        }

        public static PathDirection operator +(PathDirection a, PathDirection b)
        {
            return new PathDirection(a.X + b.X, a.Y + b.Y);
        }
        public static PathDirection operator -(PathDirection a, PathDirection b)
        {
            return new PathDirection(a.X - b.X, a.Y - b.Y);
        }

        public PathDirection Angle(int ang)
        {
            int x = (int)Math.Round(Sin(ang + CurrentAngle));
            int y = (int)Math.Round(Cos(ang + CurrentAngle)); // sin 0 = 0, 0 = up

            return this + new PathDirection(x, y * -1);
        }

        public void Move(int angle)
        {
            CurrentAngle = CurrentAngle + angle;
            int x = (int)Math.Round(Sin(CurrentAngle));
            int y = (int)Math.Round(Cos(CurrentAngle));
            X += x;
            Y -= y; // going up y becomes negative
        }
        public override string ToString()
        {
            return X.ToString() + " : " + Y.ToString() + " @ " + CurrentAngle.ToString();
        }

    }
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
        /// <summary>
        /// Ammount of turns when tracing a path around the target.
        /// </summary>
        public int Left, Right, Forward;
        public void ShapeDetection()
        {
            int lefts = 0;
            int rights = 0;
            int fwd = 0;

            int startx = 0;
            int starty = this.SizeY / 2;
            
            for (int findx = 0; findx < this.SizeX; findx++)
                if (Shape[findx, starty] == 1)
                {
                    startx = findx - 1;
                    break;
                }

            PathDirection dir = new PathDirection(startx, starty);
            dir.CurrentAngle = 0; // up = 0, right = 90, left = - 90
            int flips_inarow = 0;
            while (true)
            {
                PathDirection right = dir.Angle(90);
                PathDirection forward = dir.Angle(0);
                PathDirection left = dir.Angle(-90);
                if (Shape[right.X, right.Y] == 0)
                {
                    dir.Move(90);
                    flips_inarow = 0;
                    rights++;
                }
                else if (Shape[forward.X, forward.Y] == 0)
                {
                    dir.Move(0);
                    flips_inarow = 0;
                    fwd++;
                }
                else if (Shape[left.X, left.Y] == 0)
                {
                    dir.Move(-90);
                    flips_inarow = 0;
                    lefts++;
                }
                else
                {
                    dir.CurrentAngle *= -1;// flips its angle, rotation by 180
                    lefts += 2; // equilivent to 2 lefts
                    flips_inarow++;
                    if (flips_inarow > 5)
                        break;
                }

                if (dir.X == startx && dir.Y == starty)
                    break;
            }
            Left = lefts;
            Right = rights;
            Forward = fwd;
            #region PEUSUDO CODE
            //algorithm des.
            /*direction = up
             *while not reached start point
             * can we move right?
             *   move right and set new direction
             * can we move forward
             *  move forward
             * can we move left
             *  move forward and set new direction
             * if we cant move left
             *  turn 180 degrees
             *end
             * 
            */
            #endregion
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
        private int _noisereduction = 4;
        private int _maxjoindistance = 10;
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

namespace Detector.Tracking
{
    public class ObjectTracked
    {

        public static bool operator ==(ObjectTracked obj1, ObjectTracked obj2)
        {
            if ((object)obj1 == null && (object)obj2 == null)
                return true;
            if ((object)obj1 == null || (object)obj2 == null)
                return false;
            return obj1.ID == obj2.ID;
        }

        public static bool operator !=(ObjectTracked obj1, ObjectTracked obj2)
        {
            if ((object)obj1 == null && (object)obj2 == null)
                return false;
            if ((object)obj1 == null || (object)obj2 == null)
                return true;
            return obj1.ID != obj2.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.ID;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        private Rectangle ImgSize;
        /// <summary>
        /// Represents an object that is being tracked
        /// </summary>
        /// <param name="ID">The new ID of the object</param>
        /// <param name="Pos">Position</param>
        /// <param name="Size">Size</param>
        public ObjectTracked(int ID, Point Pos, Rectangle Size, Rectangle imgSize)
        {
            _Position = Pos;

            PosX = (float)Pos.X / (float)imgSize.Width;
            PosY = (float)Pos.Y / (float)imgSize.Height;
            SizeX = (float)Size.X / (float)imgSize.Width;
            SizeY = (float)Size.Y / (float)imgSize.Height;

            _Size = Size;
            _StartedTracking = DateTime.Now;
            _LastSeen = DateTime.Now;
            _Velocity = new Point(0, 0);
            _LastPos = _Position;
            _LastSize = _Size;
            _ID = ID;
            ImgSize = imgSize;
        }

        #region Properties

        /// <summary>
        /// When the object was started to be tracked
        /// </summary>
        public DateTime StartedTracking
        {
            get
            {
                return _StartedTracking;
            }
            set
            {
                _StartedTracking = value;
            }
        }
        /// <summary>
        /// How long the object has been tracked
        /// </summary>
        public TimeSpan LifeTime
        {
            get
            {
                return DateTime.Now - _StartedTracking;
            }
        }
        /// <summary>
        /// The position of the object (When set it automaticly updates velocity)
        /// </summary>
        public Point Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _LastPos = _Position;
                _Position = value;
                PosX = (float)_Position.X / (float)ImgSize.Width;
                PosY = (float)_Position.Y / (float)ImgSize.Height;
                _Velocity.X = _Position.X - _LastPos.X;
                _Velocity.Y = _Position.Y - _LastPos.Y;
                _LastSeen = DateTime.Now;
            }
        }
        /// <summary>
        /// Percent across the image
        /// </summary>
        public float PosX;
        /// <summary>
        /// Percent across the image
        /// </summary>
        public float PosY;
        /// <summary>
        /// The position it was at last
        /// </summary>
        public float SizeX;
        /// <summary>
        /// The size it was at last
        /// </summary>
        public float SizeY;
        /// <summary>
        /// The size it was at last
        /// </summary>
        public Point LastPos
        {
            get
            {
                return _LastPos;
            }
        }
        /// <summary>
        /// The size of the object
        /// </summary>
        public Rectangle Size
        {
            get
            {
                return _Size;
            }
            set
            {
                _LastSize = _Size;
                _Size = value;
                SizeX = (float)_Size.X / (float)ImgSize.Width;
                SizeY = (float)_Size.Y / (float)ImgSize.Height;
            }
        }
        /// <summary>
        /// The size previos size of the 
        /// </summary>
        public Rectangle LastSize
        {
            get
            {
                return _LastSize;
            }
        }
        /// <summary>
        /// Unique Identifier
        /// </summary>
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }
        /// <summary>
        /// The time the object was last seen
        /// </summary>
        public DateTime LastSeen
        {
            get
            {
                return _LastSeen;
            }
        }
        /// <summary>
        /// The velocity the object is traveling
        /// </summary>
        public Point Velocity
        {
            get
            {
                return _Velocity;
            }
        }

        public int ObjectRecogLefts = 0;
        public int ObjectRecogRights = 0;
        public int ObjectRecogForwads = 0;

        /// <summary>
        /// Get the score of the target to check if it is the correct one
        /// </summary>
        /// <param name="t">The Target</param>
        /// <returns>Score</returns>
        public int GetScore(Target t)  /////////// FINISH ME!
        {
            // 100 is MaxBadScoreDistance
            float score_pos = 1 - Math.Min(1, Distance(t.X,t.Y,_Position.X+_Velocity.X,_Position.Y+_Velocity.Y) / 150); // score algo with max being 1 for all of them
            
            // 50 is MaxBadScoreVelocity
            float vel_x = 1 - Math.Min(1, Math.Abs(t.X - _Position.X) / 50);
            float vel_y = 1 - Math.Min(1, Math.Abs(t.X - _Position.X) / 50);

            // 50 is MaxBadScoreSize
            float size_x = 1 - Math.Min(1, Math.Abs(t.SizeX - _Size.X) / 100);
            float size_y = 1 - Math.Min(1, Math.Abs(t.SizeY - _Size.Y) / 100);

            float score_size = (size_x+size_y)/2;
            float score_vel = (vel_x+vel_y)/2;

            return (int)( (score_pos+score_size+score_vel)/3 ) * 100;
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
            int xx = Math.Abs(x - a);
            int yy = Math.Abs(y - b);
            return (int)Math.Sqrt(xx * xx + yy * yy);
        }


        private DateTime _StartedTracking;
        private DateTime _LastSeen;
        private Point _Position;
        private Rectangle _Size;
        private Point _Velocity;
        private Point _LastPos;
        private Rectangle _LastSize;
        private int _ID;
        #endregion
    }
    /// <summary>
    /// Arguments to pass the object
    /// </summary>
    public class ObjectTrackedArgs : System.EventArgs
    {
        private ObjectTracked obj;
        public ObjectTrackedArgs(ObjectTracked ot)
        {
            obj=ot;
        }
        public ObjectTracked Object
        {
            get
            {
                return obj;
            }
        }
    }

    public class ObjectTracker
    {
        /// <summary>
        /// A wrapper for the motion detection algorithm for tracking the targets
        /// </summary>
        public ObjectTracker()
        {
        }

        #region Events

        public delegate void NewObjectTrackedHandeler(ObjectTrackedArgs oa);
        public event NewObjectTrackedHandeler NewObjectTracked;

        public delegate void LostTrackedObjectHandeler(ObjectTrackedArgs oa);
        public event LostTrackedObjectHandeler LostTrackedObject;

        public delegate void UpdateTrackedObjectHandeler(ObjectTrackedArgs oa);
        public event UpdateTrackedObjectHandeler UpdateTrackedObject;
        
        #endregion

        /// <summary>
        /// Update all the targets
        /// </summary>
        /// <param name="targs">Array of the targets</param>
        public void UpdateTargets(Target[] targs)
        {
            _targets = targs;
        }

        public int FrameSizeX = 0;
        public int FrameSizeY = 0;
        /// <summary>
        /// For % pos
        /// </summary>
        public void SetFrameSize(int x, int y)
        {
            FrameSizeX = x;
            FrameSizeY = y;
        }

        /// <summary>
        /// Get the tracked objects
        /// </summary>
        /// <returns>Objects</returns>
        public IEnumerable<ObjectTracked> GetObjects()
        {
            int count = 0; // wether we got anything or nothing
            foreach (Target t in _targets)
            {
                int best_score = 0;     // does this object have a target to belong to? well find out with these vars for later checking
                ObjectTracked best_scorer = null;
                int score;
                foreach (ObjectTracked obj in ObjectsTracked)
                {
                    score = obj.GetScore(t);
                    if (score > _min_score) // we will check if its more than the threshhold later
                    {
                        best_score = score;
                        best_scorer = obj;
                    }
                }
                if (best_score > _min_score)
                {
                    // woot we got the obj's target
                    best_scorer.ObjectRecogForwads = t.Forward;
                    best_scorer.ObjectRecogLefts = t.Left;
                    best_scorer.ObjectRecogRights = t.Right;

                    best_scorer.Position = new Point(t.X, t.Y);
                    best_scorer.Size = new Rectangle(t.SizeX, t.SizeY, 0, 0);
                    ObjectTrackedArgs args = new ObjectTrackedArgs(best_scorer);
                    UpdateTrackedObject(args);
                }
                else
                {
                    ObjectTracked new_obj = new ObjectTracked(_i++,
                        new Point(t.X,t.Y),
                        new Rectangle(t.SizeX,t.SizeY,0,0),
                        new Rectangle(0,0,FrameSizeX, FrameSizeY)
                        );
                    new_obj.ObjectRecogForwads = t.Forward;
                    new_obj.ObjectRecogLefts = t.Left;
                    new_obj.ObjectRecogRights = t.Right;
                    ObjectsTracked.AddLast(new_obj);
                    if (NewObjectTracked != null)
                    {
                        ObjectTrackedArgs args = new ObjectTrackedArgs(new_obj);
                        NewObjectTracked(args);
                    }
                }
            }
            LinkedListNode<ObjectTracked> cur = ObjectsTracked.First;
            int bigest_id = 0;
            while (cur != null)
            {
                // check _i and make it as small as possible
                if (cur.Value.ID > bigest_id)
                    bigest_id = cur.Value.ID;

                int ms_ago = (int)(DateTime.Now - cur.Value.LastSeen).TotalMilliseconds;
                if (ms_ago <= _miliseconds_unseen_till_removed)
                {
                    yield return cur.Value;
                    count++;
                }
                else
                {
                    if (LostTrackedObject != null)
                    {
                        ObjectTrackedArgs args = new ObjectTrackedArgs(cur.Value);
                        LostTrackedObject(args);
                    }
                    

                    LinkedListNode<ObjectTracked> last = cur;
                    cur = cur.Next;
                    ObjectsTracked.Remove(last);
                }

                if (cur != null)
                {
                    cur = cur.Next;
                }
            }
            _i = bigest_id+1;
            
            if (count <= 0)
                yield break;
        }

        

        #region Properties
        /// <summary>
        /// The minimuim object recognition until a new object is created
        /// </summary>
        public int MinimuimObjectScore
        {
            get
            {
                return _min_score;
            }
            set
            {
                _min_score = value;
            }
        }
        /// <summary>
        /// The number of miliseconds the target will go untracked before it is removed
        /// </summary>
        public int UnseenRemovalLimit
        {
            get
            {
                return _miliseconds_unseen_till_removed;
            }
            set
            {
                _miliseconds_unseen_till_removed = value;
            }
        }
        #endregion
        private int _min_score = 10;
        private int _i;
        private int _miliseconds_unseen_till_removed = 3000;
        private Target[] _targets;
        public LinkedList<ObjectTracked> ObjectsTracked = new LinkedList<ObjectTracked>();
    }
}

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

                    row_blur[R] = (byte)((int)row_blur[R] + (((int)row_current[R] - (int)row_blur[R]) / ammount));
                    row_blur[G] = (byte)((int)row_blur[G] + (((int)row_current[G] - (int)row_blur[G]) / ammount));
                    row_blur[B] = (byte)((int)row_blur[B] + (((int)row_current[B] - (int)row_blur[B]) / ammount));
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