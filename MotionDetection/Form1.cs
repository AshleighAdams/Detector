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

namespace Detector.Motion
{
    public partial class Form1 : Form
    {
        WebCam.WebCam webcam;
        MotionDetector detector = new MotionDetector();
        ObjectTracker tracker = new ObjectTracker();
        Detector.Helper.ImageHelpers helper = new Detector.Helper.ImageHelpers();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webcam = new WebCam.WebCam();
            webcam.InitializeWebCam(ref pbCurrent);

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
       

        private void btnStart_Click(object sender, EventArgs e)
        {
            webcam.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            webcam.Start();
        }

        private void btnFormat_Click(object sender, EventArgs e)
        {
            webcam.ResolutionSetting();
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            webcam.AdvanceSetting();
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            webcam.Continue();
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
            DateTime start = DateTime.Now;
            Bitmap blur = new Bitmap(pbLast.Image);
            Bitmap __cur = new Bitmap(pbCurrent.Image);
            helper.MotionBlur(ref blur, ref __cur, 20);
            pbLast.Image = blur;

            Bitmap cur_img = new Bitmap(pbCurrent.Image);
            //if (frames[frames.Length - 1] != null)
            //{
            Bitmap bmp = new Bitmap(pbCurrent.Image);
            detector.SetNextImages(pbLast.Image, pbCurrent.Image);
            Target[] targs = new List<Target>(detector.GetTargets()).ToArray();
            tracker.UpdateTargets(targs);
            
            #region DrawMotion
            /*foreach (Target t in targs)
            {
                
                

            }*/
            #endregion

            #region DrawTargets
            string lable_data = "";
            ObjectTracked[] objs = new List<ObjectTracked>(tracker.GetObjects()).ToArray();
            lable_data += "Tracking " + objs.Length.ToString() + " objects\n";
            foreach (ObjectTracked obj in objs)
            {
                lable_data += "ID: " + obj.ID + "\n";
                Point vel = obj.Velocity;
                lable_data += "\tVel  X: " + vel.X + "  Y: " + vel.Y + "\n";
                double a = 255 - (((DateTime.Now - obj.LastSeen)).TotalMilliseconds / tracker.UnseenRemovalLimit)*255 ;
                Color col;
                col = Color.FromArgb(255, 0, 255, 0);
                if( (DateTime.Now - obj.LastSeen).TotalMilliseconds > 500) // hell, we are no longer activeley tracking this guy 
                    col = Color.FromArgb((int)a, 255, 0, 0);
                
                
                
                helper.DrawBox(obj, ref bmp, col);
                //helper.DrawBox(obj.Position.X, obj.Position.Y, obj.Size.X, obj.Size.Y, ref bmp, col);
            }
            double ms = (DateTime.Now - start).TotalMilliseconds;
            lblData.Text = "Frametime: " + ms.ToString() + " (" + (Math.Round(1000/ms)).ToString() + ") " + lable_data;
            #endregion

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

        private void pbIgnoreMotion_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pbIgnoreMotion.ImageLocation = ofd.FileName;
                System.Threading.Thread.Sleep(50);
                detector.IgnoreMotion = pbIgnoreMotion.Image;
            }
        }

    }
}
