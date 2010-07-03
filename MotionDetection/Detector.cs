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
    public class MotionDetector
    {
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
        private int _maxjoindistance = 15;
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

namespace Detector.Tracking
{
    public class ObjectTracked
    {
        public ObjectTracked(int ID, Point Pos, Rectangle Size)
        {
            _Position = Pos;
            _Size = Size;
            _StartedTracking = DateTime.Now;
            _LastSeen = DateTime.Now;
            _Velocity = new Point(0, 0);
            _LastPos = _Position;
            _LastSize = _Size;
            _ID = ID;
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
        /// The position of the object (When set it automaticly updates velocity
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
                _Velocity.X = _Position.X - _LastPos.X;
                _Velocity.Y = _Position.Y - _LastPos.Y;
                _LastSeen = DateTime.Now;
            }
        }
        /// <summary>
        /// The position it was at last
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

        /// <summary>
        /// Get the score of the target to check if it is the correct one
        /// </summary>
        /// <param name="t">The Target</param>
        /// <returns>Score</returns>
        public int GetScore(Target t)  /////////// FINISH ME!
        {
            // 100 is MaxBadScoreDistance
            float score_pos = 1 - Math.Min(1, Distance(t.X,t.Y,_Position.X,_Position.Y) / 150); // score algo with max being 1 for all of them
            
            // 50 is MaxBadScoreVelocity
            float vel_x = 1 - Math.Min(1, Math.Abs(t.X - _Position.X) / 50);
            float vel_y = 1 - Math.Min(1, Math.Abs(t.X - _Position.X) / 50);

            // 50 is MaxBadScoreSize
            float size_x = 1 - Math.Min(1, Math.Abs(t.SizeX - _Size.X) / 50);
            float size_y = 1 - Math.Min(1, Math.Abs(t.SizeY - _Size.Y) / 50);

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
        public ObjectTracker()
        {
        }

        #region Events

        public delegate void NewObjectTrackedHandeler(ObjectTrackedArgs oa);
        public event NewObjectTrackedHandeler NewObjectTracked;

        public delegate void LostTrackedObjectHandeler(ObjectTrackedArgs oa);
        public event LostTrackedObjectHandeler LostTrackedObject;
        
        #endregion

        /// <summary>
        /// Update all the targets
        /// </summary>
        /// <param name="targs">Array of the targets</param>
        public void UpdateTargets(Target[] targs)
        {
            _targets = targs;
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
                foreach (ObjectTracked obj in _objects_tracked)
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
                    best_scorer.Position = new Point(t.X, t.Y);
                    best_scorer.Size = new Rectangle(t.SizeX, t.SizeY, 0, 0);
                }
                else
                {
                    ObjectTracked new_obj = new ObjectTracked(_i++, new Point(t.X,t.Y),new Rectangle(t.SizeX,t.SizeY,0,0));
                    _objects_tracked.AddLast(new_obj);
                    if (NewObjectTracked != null)
                    {
                        ObjectTrackedArgs args = new ObjectTrackedArgs(new_obj);
                        NewObjectTracked(args);
                    }
                }
            }
            LinkedListNode<ObjectTracked> cur = _objects_tracked.First;
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
                    _objects_tracked.Remove(last);
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
        private LinkedList<ObjectTracked> _objects_tracked = new LinkedList<ObjectTracked>();
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
        public void DrawBox(int X, int Y, int width, int height, ref Bitmap bmp, Color col)
        {
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            for (int x = X; x < X + width; x++)
            {
                if (x >= bmp.Width)
                    break;
                SetPixel(ref bmp_data, x, Y, col);
                SetPixel(ref bmp_data, x, Math.Min(bmp.Height - 1, Y + height), col);
            }
            for (int y = Y; y < Y + height; y++)
            {
                if (y >= bmp.Height)
                    break;
                SetPixel(ref bmp_data, X, y, col);
                SetPixel(ref bmp_data, Math.Min(bmp.Width - 1, X + width), y, col);
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
            DrawBox(t.X, t.Y, t.SizeX, t.SizeY, ref bmp, col);
        }
        /// <summary>
        /// Draw a box around an object
        /// </summary>
        /// <param name="obj">Object to draw around</param>
        /// <param name="bmp">The bitmap to draw to</param>
        /// <param name="col">Color</param>
        public void DrawBox(ObjectTracked obj, ref Bitmap bmp, Color col)
        {
            DrawBox(obj.Position.X, obj.Position.Y, obj.Size.X, obj.Size.Y, ref bmp, col);
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