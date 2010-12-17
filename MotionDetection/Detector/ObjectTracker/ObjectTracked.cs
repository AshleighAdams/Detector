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

        public int Score
        {
            get
            {
                return _Score;
            }
            set
            {
                _Score = value;
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
                Point center = this.Center();
                Point lastcenter = this.LastCenter();
                PosX = (float)center.X / (float)ImgSize.Width;
                PosY = (float)center.Y / (float)ImgSize.Height;

                if (!PositionWasFaked)
                {
                    _Velocity.X = center.X - lastcenter.X;
                    _Velocity.Y = center.Y - lastcenter.Y;
                    _LastSeen = DateTime.Now;
                    avg_x += _Velocity.X / 5; // 10 is the avg value
                    avg_y += _Velocity.Y / 5;
                }
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


        private int avg_x = 0;
        private int avg_y = 0;
        /// <summary>
        /// The velocity the object is traveling
        /// </summary>
        public Point AvgVelocity
        {
            get
            {
                return new Point(avg_x, avg_y);
            }
        }

        /// <summary>
        /// Get the score of the target to check if it is the correct one
        /// </summary>
        /// <param name="t">The Target</param>
        /// <returns>Score as a percentage, 0 to 100</returns>
        public int GetScore(Target t)  /////////// FINISH ME!
        {
            // 100 is MaxBadScoreDistance
            float score_pos = 1f - Math.Min(1, Distance(t.X, t.Y, _Position.X + _Velocity.X, _Position.Y + _Velocity.Y) / 150f); // score algo with max being 1 for all of them

            // 50 is MaxBadScoreVelocity
            //float vel_x = 1f - Math.Min(1, Math.Abs(t.X - _Position.X) / 50f);
            //float vel_y = 1f - Math.Min(1, Math.Abs(t.Y - _Position.Y) / 50f);

            // 50 is MaxBadScoreSize
            float size_x = 1f - Math.Min(1, Math.Abs(t.SizeX - _Size.X) / 100f);
            float size_y = 1f - Math.Min(1, Math.Abs(t.SizeY - _Size.Y) / 100f);

            float score_size = (size_x + size_y) / 2f;
            //float score_vel = (vel_x + vel_y) / 2f;

            return (int)(((score_pos + score_size) / 2f) * 100f);
        }

        /// <summary>
        /// Was the position updated with FakeUpdatePos()
        /// </summary>
        public bool PositionWasFaked = false;

        /// <summary>
        /// Fake the target movements
        /// </summary>
        public void FakeUpdatePos()
        {
            PositionWasFaked = true;
            int x = _Position.X + _Velocity.X;
            int y = _Position.Y + _Velocity.Y;
            this.Position = new Point(x, y);
        }

        /// <summary>
        /// Center of an object
        /// </summary>
        public Point Center()
        {
            int x = this._Position.X + (this._Size.X / 2);
            int y = this._Position.Y + (this._Size.Y / 2);
            return new Point(x, y);
        }

        public Point LastCenter()
        {
            int x = this._LastPos.X + (this._LastSize.X / 2);
            int y = this._LastPos.Y + (this._LastSize.Y / 2);
            return new Point(x, y);
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
        private int _Score;
        private Point _Position;
        private Rectangle _Size;
        private Point _Velocity;
        private Point _LastPos;
        private Rectangle _LastSize;
        private int _ID;
        #endregion
    }
}
