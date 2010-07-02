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

namespace Detector.Motion
{
    public partial class Form1 : Form
    {
        WebCam.WebCam webcam;
        MotionDetector detector = new MotionDetector();
        ObjectTracker tracker = new ObjectTracker();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webcam = new WebCam.WebCam();
            webcam.InitializeWebCam(ref pbCurrent);
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

            Bitmap blur = new Bitmap(pbLast.Image);
            Bitmap __cur = new Bitmap(pbCurrent.Image);
            MotionBlur(ref blur, ref __cur,20);
            pbLast.Image = blur;

            Bitmap cur_img = new Bitmap(pbCurrent.Image);
            //if (frames[frames.Length - 1] != null)
            //{
            Bitmap bmp = new Bitmap(pbCurrent.Image);
            detector.SetNextImages(pbLast.Image, pbCurrent.Image);
            tracker.UpdateTargets(new List<Target>(detector.GetTargets()).ToArray());

            foreach (ObjectTracked obj in tracker.GetObjects())
            {

                for (int x = obj.Position.X; x < obj.Position.X + obj.Size.X; x++)
                {
                    if (x >= cur_img.Width)
                        break;
                    bmp.SetPixel(x, obj.Position.Y, Color.Red);
                    bmp.SetPixel(x, Math.Min(cur_img.Height - 1, obj.Position.Y + obj.Size.Y), Color.Red);
                }
                for (int y = obj.Position.Y; y < obj.Position.Y + obj.Size.Y; y++)
                {
                    if (y >= cur_img.Height)
                        break;
                    bmp.SetPixel(obj.Position.X, y, Color.Red);
                    bmp.SetPixel(Math.Min(cur_img.Width - 1, obj.Position.X + obj.Size.X), y, Color.Red);
                }
                
            }
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

        private unsafe void MotionBlur(ref Bitmap imagetoblur, ref Bitmap current, int ammount)
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

    }
}
