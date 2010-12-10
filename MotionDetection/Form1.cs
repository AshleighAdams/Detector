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
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.IO;
using System.Threading;
using Gif.Components;
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
        LinkedList<AnimationHandeler> trackingobjs = new LinkedList<AnimationHandeler>();
        
        //CVCapture cam = new CVCapture();
        Capture cam = new Capture(0);
        
        
        MotionDetector detector = new MotionDetector();
        ObjectTracker tracker = new ObjectTracker();
        Detector.Helper.ImageHelpers helper = new Detector.Helper.ImageHelpers();
        public Form1()
        {
            InitializeComponent();
            //cam.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, 480);
            //cam.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, 640);
            
            
        }
        private Bitmap GetImage(ref Bitmap frame, ObjectTracked obj)
        {
            Bitmap img = new Bitmap(frame);
            DateTime now = DateTime.Now;
            string str = now.TimeOfDay.ToString();

            Graphics gfx = Graphics.FromImage(img);
            Font fnt = new Font(FontFamily.GenericMonospace, 10f);
            gfx.DrawString(str, fnt, Brushes.Red, new PointF(1f, 
                    img.Height - fnt.Height - 2f
                ));
            
            

            int x, y, w, h;
            x = (int)(obj.PosX * img.Size.Width);
            y = (int)(obj.PosY * img.Size.Height);

            w = (int)(obj.SizeX * img.Size.Width);
            h = (int)(obj.SizeY * img.Size.Height);

            helper.DrawBox(x, y, w, h, ref img, Color.Red, false);

            return img;
        }
        private string GetFile(ObjectTracked obj)
        {
            string date = DateTime.Now.ToString();
            date = date.Replace("/", "-");
            date = date.Replace(" ", "\\"); // date and time are seperated by a space, this will newfolder them
            date = date.Replace(":", ".");

            string folderDate = Path.GetDirectoryName(date);
            string path = "C:\\Users\\C0BRA\\Documents\\My Dropbox\\Public\\Detection\\" + folderDate;
            date = date.Replace(folderDate, "");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + date + " ID " + obj.ID.ToString() + ".gif";
        }

        private void SaveImageThread(AnimationHandeler hand, bool removeAndReturn)
        {
            if (removeAndReturn)
            {
                trackingobjs.Remove(hand);
                return;
            }
            string FileName = GetFile(hand.Obj);

            Bitmap[] bmps = hand.bitmaps.ToArray<Bitmap>();
            trackingobjs.Remove(hand);
            AnimatedGifEncoder gif = new AnimatedGifEncoder();

            gif.Start(FileName);
            gif.SetDelay(50);
            gif.SetRepeat(0);
            
            foreach (Bitmap bmp in bmps)
                gif.AddFrame(bmp);

            gif.Finish();
            
        }

        private Bitmap HiResFrame;

        private void Form1_Load(object senderr, EventArgs ee)
        {
            //serial.Open();
            tracker.NewObjectTracked += delegate(ObjectTrackedArgs args)
            {
                ObjectTracked obj = args.Object;
                lbHistory.Items.Add("Tacking: " + obj.ID.ToString() + " (" + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString() + ")");
                lbHistory.SetSelected(lbHistory.Items.Count - 1, true);
                lbHistory.SetSelected(lbHistory.Items.Count - 1, false);

                trackingobjs.AddLast(new AnimationHandeler(obj));
                
                //"04/12/2010 03:14:46"
                
            };

            tracker.UpdateTrackedObject += delegate(ObjectTrackedArgs args)
            {
                Bitmap img = new Bitmap(HiResFrame);
                
                foreach(AnimationHandeler hand in trackingobjs)
                    if(hand.Obj == args.Object)
                    {
                        hand.bitmaps.AddLast(GetImage(ref img, args.Object));
                        break;
                    }
            };

            tracker.LostTrackedObject += delegate(ObjectTrackedArgs args)
            {
                ObjectTracked obj = args.Object;
                lbHistory.Items.Add("Lost: " + obj.ID.ToString() + " (" + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString() + ")");
                lbHistory.SetSelected(lbHistory.Items.Count - 1, true);
                lbHistory.SetSelected(lbHistory.Items.Count - 1, false);
                bool removeAndReturn = false;
                if (!cbSave.Checked)
                {
                    trackingobjs.Remove(new AnimationHandeler(obj));
                    removeAndReturn = true;
                }
                
                TimeSpan ts = (DateTime.Now - obj.LastSeen) - obj.LifeTime;
                int ms = (int)Math.Abs(ts.TotalMilliseconds);
                this.Text = ms.ToString();
                if (ms < 200) // this must be a mistake
                {
                    trackingobjs.Remove(new AnimationHandeler(obj));
                    removeAndReturn = true;
                }

                foreach (AnimationHandeler hand in trackingobjs)
                    if (hand.Obj == args.Object)
                    {
                        Thread thrd = new Thread(delegate()
                            {
                                SaveImageThread(hand, removeAndReturn);
                            });
                        thrd.Start();
                        //SaveImageThread(hand, removeAndReturn);
                        break;
                    }

                
            };

            //detector.Difference = 20;
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
        private int[] AvgMisses = new int[50];
        int AvgMisses_pos = 0;
        private void tmrCheckMotion_Tick(object sender, EventArgs e)
        {

            
            int N = 7;
            int aperature_size = N;
            
            //CVImage cv_out;
            DateTime start = DateTime.Now;
            //using (CVImage nextframe = cam.QueryFrame())
            //{
                
                //pbCurrent.Image = nextframe.ToBitmap();
            //}

            Image<Bgr, byte> frame = cam.QuerySmallFrame();

            HiResFrame = cam.QueryFrame().ToBitmap();

            tracker.SetFrameSize(pbCurrent.Width, pbCurrent.Height);

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


            bmp = detector.motionpic;
            string lable_data = "";
            #region DrawMotion
            foreach (Target t in targs)
            {

                helper.DrawBox(t,
                    ref bmp, Color.FromArgb(255, 255, 0, 0));

            }
            #endregion
            
            #region DrawTargets
            ObjectTracked[] objs = new List<ObjectTracked>(tracker.GetObjects()).ToArray();
            lable_data += "Tracking " + objs.Length.ToString() + " objects\n";
            int count = 0;
            ObjectTracked besttarget = null;
            foreach (ObjectTracked obj in objs)
            {
                lable_data += "ID: " + obj.ID + " OJR: " + obj.ObjectRecogForwads.ToString() + ":" + obj.ObjectRecogLefts.ToString() + ":" + obj.ObjectRecogRights.ToString() + "\n";
                double a = 255 - (((DateTime.Now - obj.LastSeen)).TotalMilliseconds / tracker.UnseenRemovalLimit)*255 ;
                a = Math.Min(255, Math.Max(0, a));
                Color col;
                col = cols[Math.Min(cols.Length - 1, obj.ID)];
                if( (DateTime.Now - obj.LastSeen).TotalMilliseconds > 500) // hell, we are no longer activeley tracking this guy 
                    col = Color.FromArgb((int)a, 255, 0, 0);
                count++;
                if (besttarget == null)
                    besttarget = obj;
                else
                {
                    if (Math.Abs(obj.Velocity.X) + Math.Abs(obj.Velocity.Y)
                        >
                        Math.Abs(besttarget.Velocity.X) + Math.Abs(besttarget.Velocity.Y))
                    {
                        besttarget = obj;
                    }
                }
                //helper.DrawBox(obj, ref bmp, col);
                //helper.DrawBox(obj.Position.X, obj.Position.Y, obj.Size.X, obj.Size.Y, ref bmp, col);
            }
             #endregion

            #region MoveServo

            if (besttarget != null)
            {
                double per_x = ((double)besttarget.Position.X / (double)bmp.Width);
                double per_y = ((double)besttarget.Position.Y / (double)bmp.Height);

                double pitch = Scale(per_y, 0, 1, MinY, MaxY);
                double yaw = Scale(per_x, 0, 1, MinX, MaxX);

                SetServoPos((int)yaw, (int)pitch);
            }
            #endregion

            double ms = ( DateTime.Now - start ).TotalMilliseconds;
            lblData.Text = "Frametime: " + ms.ToString() + " (" + ( Math.Round( 1000 / ms ) ).ToString() + ") " + lable_data + " Ammount of noise: " + detector.BadTargets.ToString();
           
            pbMotion.Image = bmp;

            AvgMisses[AvgMisses_pos] = detector.BadTargets;
            AvgMisses_pos = (AvgMisses_pos + 1) % AvgMisses.Length;

            float avg = 0f;
            if (cbAuto.Checked && tracker.ObjectsTracked.Count == 0)
            {
                foreach (int value in AvgMisses)
                    avg += (float)value;
                avg /= (float)AvgMisses.Length;

                if (avg == 0)
                    detector.Difference--;
                else if (avg > 0.15)
                    detector.Difference++;
                tbDifference.Value = Math.Max(1, Math.Min(159, detector.Difference));
                lblData.Text += " Avg Noise: " + avg.ToString("0.00");
            }
            

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
        {/*
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
            detector.IgnoreMotion = ebmp;*/
        }

        private void pbIgnoreMotion_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            pbIgnoreMotion.Image = pbCurrent.Image;
            detector.IgnoreMotion = pbCurrent.Image;
        }
        private void SaveBMP(Bitmap bmp, string path) // now 'ref' parameter
        {
            try
            {
                //bmp.Save(path);
            }
            catch
            {
                /*Bitmap bitmap = new Bitmap(bmp.Width, bmp.Height, bmp.PixelFormat);
                Graphics g = Graphics.FromImage(bitmap);
                g.DrawImage(bmp, new Point(0, 0));
                g.Dispose();
                bmp.Dispose();
                //bitmap.Save(path);
                bmp = bitmap; // preserve clone */
            }
        }

        public void SetServoPos(int Yaw, int Pitch)
        {
            byte[] DATA = new byte[2];
            DATA[0] = (byte)Scale(Yaw, 0, 180, 0, 127);
            DATA[1] = (byte)Scale(Pitch, 0, 180, 127, 255);
            //serial.Write(DATA, 0, DATA.Length);
        }
        /// <summary>
        /// Scale a value between one set and the other
        /// </summary>
        /// <param name="value">The value you want to scale</param>
        /// <param name="min1">This is its minimum value</param>
        /// <param name="max1">This is its maximum value</param>
        /// <param name="min2">The minimum you want</param>
        /// <param name="max2">The maximum you want</param>
        /// <returns>The scaled value</returns>
        public double Scale(double value, double min1, double max1, double min2, double max2) // we can use my func to find the real servo pos and the servo pos from the pic
        {
            if (value < min1) value = min1;
            if (value > max1) value = max1;
            double VP = (value - min1) / (max1 - min1);
            return ((max2 - min2) * VP + min2);
        }



        private double MinY = 60;
        private double MaxY = 120;

        private double MinX = 60;
        private double MaxX = 120;

        private void btnShowPorts_Click(object sender, EventArgs e)
        {
            string [] ports = SerialPort.GetPortNames();
            string msg = "";

            foreach (string port in ports)
            {
                msg += port + "\n";
            }
            MessageBox.Show(msg);
        }

        private void pbCurrent_MouseMove(object sender, MouseEventArgs e)
        {
            if (cbManual.Checked)
            {
                double per_x = ((double)e.X / (double)pbCurrent.Width);
                double per_y = ((double)e.Y / (double)pbCurrent.Height);

                double pitch = Scale(per_y, 0, 1, MinY, MaxY);
                double yaw = Scale(per_x, 0, 1, MinX, MaxX);

                SetServoPos((int)yaw, (int)pitch);
                System.Threading.Thread.Sleep(100);
            }
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            MaxX = tbMaxX.Value;
            SetServoPos(tbMaxX.Value, tbMaxY.Value);
        }

        private void tbMinX_Scroll(object sender, EventArgs e)
        {
            MinX = tbMinX.Value;
            SetServoPos(tbMinX.Value, tbMinY.Value);
        }

        private void tbMinY_Scroll(object sender, EventArgs e)
        {
            MinY = tbMinY.Value;
            SetServoPos(tbMinX.Value, tbMinY.Value);
        }

        private void tbMaxY_Scroll(object sender, EventArgs e)
        {
            MaxY = tbMaxY.Value;
            SetServoPos(tbMaxX.Value, tbMaxY.Value);
        }

        private void pbCurrent_Click(object sender, EventArgs e)
        {

        }

        private void btnCalLoad_Click(object sender, EventArgs e)
        {
            
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }


    }
}
