using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Detector.Motion;
using System.Diagnostics;
using Detector.Tracking;
using WebCam;
using System.Drawing.Drawing2D;
//using OpenCVDotNet;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Flann;
using Emgu.CV.Geodetic;
using Emgu.CV.ML.Structure;
using Emgu.CV.ML.MlEnum;
using Emgu.CV.ML;
using Emgu.CV.Reflection;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.VideoSurveillance;


namespace Detector.Motion
{
    public partial class Form1 : Form
    {
        //CVCapture cam = new CVCapture();
        Capture cam = new Capture(0);
       
        
        MotionDetector detector = new MotionDetector();
        ObjectTracker tracker = new ObjectTracker();
        Detector.Helper.ImageHelpers helper = new Detector.Helper.ImageHelpers();
        public Form1()
        {
            InitializeComponent();
        }

        public Image<Gray, Byte> OptimizeBlobs(Image<Bgr, Byte> img)
        {
            // can improve image quality, but expensive if real-time capture
            img._EqualizeHist();

            // convert img to temporary HSV object
            Image<Hsv, Byte> imgHSV = img.Convert<Hsv, Byte>();

            // break down HSV
            Image<Gray, Byte>[] channels = imgHSV.Split();
            Image<Gray, Byte> imgHSV_saturation = channels[1];   // saturation channel
            Image<Gray, Byte> imgHSV_value = channels[2];   // value channel

            //use the saturation and value channel to filter noise. [you will need to tweak these values]
            Image<Gray, Byte> saturationFilter = imgHSV_saturation.InRange(new Gray(0), new Gray(80));
            Image<Gray, Byte> valueFilter = imgHSV_value.InRange(new Gray(200), new Gray(255));

            // combine the filters to get the final image to process.
            Image<Gray, byte> imgTarget = valueFilter.And(saturationFilter);

            return imgTarget;
        }

        private void Form1_Load(object senderr, EventArgs ee)
        {

            tracker.NewObjectTracked += delegate(ObjectTrackedArgs args)
            {
                ObjectTracked obj = args.Object;
                lbHistory.Items.Add("Tacking: " + obj.ID.ToString() + " (" + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString() + ")");
                lbHistory.SetSelected(lbHistory.Items.Count - 1, true);
                lbHistory.SetSelected(lbHistory.Items.Count - 1, false);
            };

            tracker.LostTrackedObject += delegate(ObjectTrackedArgs args)
            {
                ObjectTracked obj = args.Object;
                lbHistory.Items.Add("Lost: " + obj.ID.ToString() + " (" + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString() + ")");
                lbHistory.SetSelected(lbHistory.Items.Count - 1, true);
                lbHistory.SetSelected(lbHistory.Items.Count - 1, false);
            };
            

        }

        void tracker_NewObjectTracked(ObjectTrackedArgs oa)
        {
            throw new NotImplementedException();
        }

        private void cbOn_CheckedChanged(object sender, EventArgs e)
        {
            pbLast.Image = pbCurrent.Image;
            tmrCheckMotion.Enabled = cbOn.Checked;
            detector.IgnoreMotion = pbIgnoreMotion.Image;
        }

        private void tbSpeed_Validating(object sender, CancelEventArgs e)
        {
            tmrCheckMotion.Interval = int.Parse( tbSpeed.Text );
        }

        private void tbDifference_Validating(object sender, CancelEventArgs e)
        {
            detector.Difference = int.Parse(tbDifference.Text);
        }


        private Image[] frames = new Image[6];

        private void tmrCheckMotion_Tick(object sender, EventArgs e)
        {
            int N = 7;
            int aperature_size = N;
            double lowThresh = 50;
            double highThresh = 255;

            //CVImage cv_out;
            DateTime start = DateTime.Now;
            //using (CVImage nextframe = cam.QueryFrame())
            //{
                
                //pbCurrent.Image = nextframe.ToBitmap();
            //}

            Image<Bgr, byte> frame = cam.QuerySmallFrame();
            



            pbCurrent.Image = frame.ToBitmap();

            if (pbCurrent.Image == null || pbLast.Image == null)
                return;
            
            Color[] cols = {
                               Color.Blue,
                               Color.Green,
                               Color.Orange,
                               Color.Purple,
                               Color.Yellow,
                               Color.Violet,
                               Color.DarkGreen,
                               Color.DarkBlue,
                               Color.DarkMagenta,
                               Color.DarkSeaGreen
                           };
            
            Bitmap blur = new Bitmap(pbLast.Image);
            Bitmap __cur = new Bitmap(pbCurrent.Image);
            helper.MotionBlur(ref blur, ref __cur, 40);
            pbLast.Image = blur;

            Bitmap cur_img = new Bitmap(pbCurrent.Image);
            //if (frames[frames.Length - 1] != null)
            //{
            Bitmap bmp = new Bitmap(pbCurrent.Image);
            detector.SetNextImages(pbLast.Image, pbCurrent.Image);
            Target[] targs = new List<Target>(detector.GetTargets()).ToArray();
            tracker.UpdateTargets(targs);

            

            string lable_data = "";
            #region DrawMotion
            foreach (Target t in targs)
            {

                helper.DrawBox(t.X - detector.MaxJoinDistance,
                    t.Y - detector.MaxJoinDistance,
                    t.SizeX + 2 * detector.MaxJoinDistance,
                    t.SizeY + 2 * detector.MaxJoinDistance,
                    ref bmp, Color.FromArgb(255, 255, 0, 0),false);

            }
            #endregion
            
            #region DrawTargets
            ObjectTracked[] objs = new List<ObjectTracked>(tracker.GetObjects()).ToArray();
            lable_data += "Tracking " + objs.Length.ToString() + " objects\n";
            foreach (ObjectTracked obj in objs)
            {
                lable_data += "ID: " + obj.ID + "\n";
                double a = 255 - (((DateTime.Now - obj.LastSeen)).TotalMilliseconds / tracker.UnseenRemovalLimit)*255 ;
                Color col;
                col = cols[Math.Min(cols.Length - 1, obj.ID)];
                if( (DateTime.Now - obj.LastSeen).TotalMilliseconds > 500) // hell, we are no longer activeley tracking this guy 
                    col = Color.FromArgb((int)a, 255, 0, 0);
                

                helper.DrawBox(obj, ref bmp, col);
                //helper.DrawBox(obj.Position.X, obj.Position.Y, obj.Size.X, obj.Size.Y, ref bmp, col);
            }
             #endregion
            
            double ms = ( DateTime.Now - start ).TotalMilliseconds;
            lblData.Text = "Frametime: " + ms.ToString() + " (" + ( Math.Round( 1000 / ms ) ).ToString() + ") " + lable_data;
           
            pbMotion.Image = bmp;
            //}
            //frames[0] = pbCurrent.Image;

            //for (int i = frames.Length - 2; i > -1; i--)
            //    frames[i + 1] = frames[i];
            //pbLast.Image = frames[frames.Length - 1];
        }

        private void tbDifference_Scroll(object sender, EventArgs e)
        {
            detector.Difference = tbDifference.Value;
        }
        private Point startdrag;
        private void pbIgnoreMotion_MouseDown(object sender, MouseEventArgs e)
        {
            startdrag = e.Location;
        }

        private void pbIgnoreMotion_MouseUp(object sender, MouseEventArgs e)
        {
            if (pbIgnoreMotion.Image == null)
            {
                pbIgnoreMotion.Image = pbCurrent.Image;
                return;
            }
            Bitmap ebmp = new Bitmap(pbIgnoreMotion.Image);

            float perX = (float)startdrag.X / (float)pbIgnoreMotion.Width;
            float perY = (float)startdrag.Y / (float)pbIgnoreMotion.Height;

            float perX_end = (float)e.X / (float)pbIgnoreMotion.Width;
            float perY_end = (float)e.Y / (float)pbIgnoreMotion.Height;

            int x = (int)((float)cam.Width * perX);
            int y = (int)((float)cam.Height * perY);

            int ex = (int)((float)cam.Width * perX_end);
            int ey = (int)((float)cam.Width * perY_end);

            helper.DrawBox(x, y, ex - x, ey - y, ref ebmp, Color.White, true);

            pbIgnoreMotion.Image = ebmp;
            detector.IgnoreMotion = ebmp;
        }

        private void pbIgnoreMotion_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            pbIgnoreMotion.Image = pbCurrent.Image;
            detector.IgnoreMotion = pbCurrent.Image;
        }


    }
}
