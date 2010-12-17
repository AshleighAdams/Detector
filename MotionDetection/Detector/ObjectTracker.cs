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
            float vel_x = 1f - Math.Min(1, Math.Abs(t.X - _Position.X) / 50f);
            float vel_y = 1f - Math.Min(1, Math.Abs(t.Y - _Position.Y) / 50f);

            // 50 is MaxBadScoreSize
            float size_x = 1f - Math.Min(1, Math.Abs(t.SizeX - _Size.X) / 100f);
            float size_y = 1f - Math.Min(1, Math.Abs(t.SizeY - _Size.Y) / 100f);

            float score_size = (size_x + size_y) / 2f;
            float score_vel = (vel_x + vel_y) / 2f;

            return (int)(((score_pos + score_size + score_vel) / 3f) * 100f);
        }

        /// <summary>
        /// Center of an object
        /// </summary>
        public Point Center()
        {
            int x = this._Position.X + (this._Size.X/2);
            int y = this._Position.Y + (this._Size.Y/2);
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
    /// <summary>
    /// Arguments to pass the object
    /// </summary>
    public class ObjectTrackedArgs : System.EventArgs
    {
        private ObjectTracked obj;
        public ObjectTrackedArgs(ObjectTracked ot)
        {
            obj = ot;
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
                    best_scorer.Position = new Point(t.X, t.Y);
                    best_scorer.Size = new Rectangle(t.SizeX, t.SizeY, 0, 0);
                    best_scorer.Score = best_score;
                    ObjectTrackedArgs args = new ObjectTrackedArgs(best_scorer);
                    UpdateTrackedObject(args);
                }
                else
                {
                    ObjectTracked new_obj = new ObjectTracked(_i++,
                        new Point(t.X, t.Y),
                        new Rectangle(t.SizeX, t.SizeY, 0, 0),
                        new Rectangle(0, 0, FrameSizeX, FrameSizeY)
                        );
                    
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
            _i = bigest_id + 1;

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
        private int _min_score = 60;
        private int _i;
        private int _miliseconds_unseen_till_removed = 3000;
        private Target[] _targets;
        public LinkedList<ObjectTracked> ObjectsTracked = new LinkedList<ObjectTracked>();
    }
}
