using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Detector;
using System.Diagnostics;
using WebCam;

namespace Detector
{
    public partial class Form1 : Form
    {
        WebCam.WebCam webcam;
        Detector detector = new Detector();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webcam = new WebCam.WebCam();
            webcam.InitializeWebCam(ref pbCurrent);
        }

        private void tmrSetPlace_Tick(object sender, EventArgs e)
        {
            pbLast.Image = pbCurrent.Image;
            tmrSetPlace.Enabled = false;
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
            Bitmap cur_img = new Bitmap(pbCurrent.Image);

            if (frames[frames.Length - 1] != null)
            {
                Bitmap bmp = new Bitmap(pbCurrent.Image);
                detector.SetNextImages(pbLast.Image, pbCurrent.Image);
                foreach (Target t in detector.GetTargets())
                {

                    for (int x = t.X; x < t.X + t.SizeX; x++)
                    {
                        if (x >= cur_img.Width)
                            break;
                        bmp.SetPixel(x, t.Y, Color.Red);
                        bmp.SetPixel(x, Math.Min( cur_img.Height-1, t.Y + t.SizeY), Color.Red);
                    }
                    for (int y = t.Y; y < t.Y + t.SizeY; y++)
                    {
                        if (y >= cur_img.Height)
                            break;
                        bmp.SetPixel(t.X, y, Color.Red);
                        bmp.SetPixel(Math.Min( cur_img.Width-1, t.X + t.SizeX ), y, Color.Red);
                    }
                    
                }
                pbMotion.Image = bmp;
            }
            frames[0] = pbCurrent.Image;

            for (int i = frames.Length - 2; i > -1; i--)
                frames[i + 1] = frames[i];
            pbLast.Image = frames[frames.Length - 1];
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
                detector.IgnoreMotion = pbIgnoreMotion.Image;
            }
        }

        

    }
}
