using System;
using System.Collections.Generic;
using Detector.Tracking;
using System.Text;
using System.Drawing;

namespace Detector.Motion
{
    class AnimationHandeler
    {
        public LinkedList<Bitmap> bitmaps = new LinkedList<Bitmap>();
        public ObjectTracked Obj;
        public AnimationHandeler(ObjectTracked obj)
        {
            Obj = obj;
        }
        public static bool operator ==(AnimationHandeler a, AnimationHandeler b)
        {
            return a.Obj == b.Obj;
        }
        public static bool operator !=(AnimationHandeler a, AnimationHandeler b)
        {
            return a.Obj != b.Obj;
        }
        public override int GetHashCode()
        {
            return Obj.GetHashCode();
        }
        
    }
}
