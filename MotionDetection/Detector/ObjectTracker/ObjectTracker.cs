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
        public LinkedList<ObjectTracked> GetObjects()
        {
            LinkedList<ObjectTracked> retval = new LinkedList<ObjectTracked>();
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
                    best_scorer.PositionWasFaked = false;
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
                    retval.AddLast(cur.Value);
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

            // Update positions / Velocity
            foreach (ObjectTracked obj in ObjectsTracked)
            {
                if ((DateTime.Now - obj.LastSeen).TotalMilliseconds > 50.0)
                {
                    obj.FakeUpdatePos();
                }
            }

            return retval;
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
