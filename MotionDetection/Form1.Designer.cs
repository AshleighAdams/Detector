namespace Detector.Motion
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pbLast = new System.Windows.Forms.PictureBox();
            this.pbCurrent = new System.Windows.Forms.PictureBox();
            this.pbMotion = new System.Windows.Forms.PictureBox();
            this.tmrCheckMotion = new System.Windows.Forms.Timer(this.components);
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnSource = new System.Windows.Forms.Button();
            this.btnFormat = new System.Windows.Forms.Button();
            this.btnContinue = new System.Windows.Forms.Button();
            this.cbOn = new System.Windows.Forms.CheckBox();
            this.tbSpeed = new System.Windows.Forms.TextBox();
            this.tbDifference = new System.Windows.Forms.TrackBar();
            this.pbIgnoreMotion = new System.Windows.Forms.PictureBox();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pbLast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCurrent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMotion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDifference)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIgnoreMotion)).BeginInit();
            this.SuspendLayout();
            // 
            // pbLast
            // 
            this.pbLast.Image = ((System.Drawing.Image)(resources.GetObject("pbLast.Image")));
            this.pbLast.Location = new System.Drawing.Point(12, 12);
            this.pbLast.Name = "pbLast";
            this.pbLast.Size = new System.Drawing.Size(170, 120);
            this.pbLast.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbLast.TabIndex = 0;
            this.pbLast.TabStop = false;
            // 
            // pbCurrent
            // 
            this.pbCurrent.Image = ((System.Drawing.Image)(resources.GetObject("pbCurrent.Image")));
            this.pbCurrent.Location = new System.Drawing.Point(12, 242);
            this.pbCurrent.Name = "pbCurrent";
            this.pbCurrent.Size = new System.Drawing.Size(346, 246);
            this.pbCurrent.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCurrent.TabIndex = 1;
            this.pbCurrent.TabStop = false;
            // 
            // pbMotion
            // 
            this.pbMotion.Location = new System.Drawing.Point(364, 12);
            this.pbMotion.Name = "pbMotion";
            this.pbMotion.Size = new System.Drawing.Size(640, 480);
            this.pbMotion.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbMotion.TabIndex = 3;
            this.pbMotion.TabStop = false;
            // 
            // tmrCheckMotion
            // 
            this.tmrCheckMotion.Tick += new System.EventHandler(this.tmrCheckMotion_Tick);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 138);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(93, 138);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 7;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnSource
            // 
            this.btnSource.Location = new System.Drawing.Point(174, 138);
            this.btnSource.Name = "btnSource";
            this.btnSource.Size = new System.Drawing.Size(75, 23);
            this.btnSource.TabIndex = 8;
            this.btnSource.Text = "Source";
            this.btnSource.UseVisualStyleBackColor = true;
            this.btnSource.Click += new System.EventHandler(this.btnSource_Click);
            // 
            // btnFormat
            // 
            this.btnFormat.Location = new System.Drawing.Point(255, 138);
            this.btnFormat.Name = "btnFormat";
            this.btnFormat.Size = new System.Drawing.Size(75, 23);
            this.btnFormat.TabIndex = 9;
            this.btnFormat.Text = "Format";
            this.btnFormat.UseVisualStyleBackColor = true;
            this.btnFormat.Click += new System.EventHandler(this.btnFormat_Click);
            // 
            // btnContinue
            // 
            this.btnContinue.Location = new System.Drawing.Point(12, 167);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(75, 23);
            this.btnContinue.TabIndex = 10;
            this.btnContinue.Text = "Continue";
            this.btnContinue.UseVisualStyleBackColor = true;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // cbOn
            // 
            this.cbOn.AutoSize = true;
            this.cbOn.Location = new System.Drawing.Point(93, 173);
            this.cbOn.Name = "cbOn";
            this.cbOn.Size = new System.Drawing.Size(40, 17);
            this.cbOn.TabIndex = 11;
            this.cbOn.Text = "On";
            this.cbOn.UseVisualStyleBackColor = true;
            this.cbOn.CheckedChanged += new System.EventHandler(this.cbOn_CheckedChanged);
            // 
            // tbSpeed
            // 
            this.tbSpeed.Location = new System.Drawing.Point(139, 171);
            this.tbSpeed.Name = "tbSpeed";
            this.tbSpeed.Size = new System.Drawing.Size(100, 20);
            this.tbSpeed.TabIndex = 12;
            this.tbSpeed.Text = "100";
            this.tbSpeed.Validating += new System.ComponentModel.CancelEventHandler(this.tbSpeed_Validating);
            // 
            // tbDifference
            // 
            this.tbDifference.LargeChange = 10;
            this.tbDifference.Location = new System.Drawing.Point(12, 196);
            this.tbDifference.Maximum = 160;
            this.tbDifference.Minimum = 5;
            this.tbDifference.Name = "tbDifference";
            this.tbDifference.Size = new System.Drawing.Size(156, 45);
            this.tbDifference.TabIndex = 15;
            this.tbDifference.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbDifference.Value = 60;
            this.tbDifference.Scroll += new System.EventHandler(this.tbDifference_Scroll);
            // 
            // pbIgnoreMotion
            // 
            this.pbIgnoreMotion.Image = ((System.Drawing.Image)(resources.GetObject("pbIgnoreMotion.Image")));
            this.pbIgnoreMotion.Location = new System.Drawing.Point(188, 12);
            this.pbIgnoreMotion.Name = "pbIgnoreMotion";
            this.pbIgnoreMotion.Size = new System.Drawing.Size(170, 120);
            this.pbIgnoreMotion.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbIgnoreMotion.TabIndex = 16;
            this.pbIgnoreMotion.TabStop = false;
            this.pbIgnoreMotion.Click += new System.EventHandler(this.pbIgnoreMotion_Click);
            // 
            // ofd
            // 
            this.ofd.FileName = "IgnoreMotionBitmap";
            this.ofd.Filter = "BMP Files|*.bmp|All Files|*.*";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 500);
            this.Controls.Add(this.pbIgnoreMotion);
            this.Controls.Add(this.tbDifference);
            this.Controls.Add(this.tbSpeed);
            this.Controls.Add(this.cbOn);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.btnFormat);
            this.Controls.Add(this.btnSource);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.pbMotion);
            this.Controls.Add(this.pbCurrent);
            this.Controls.Add(this.pbLast);
            this.Name = "Form1";
            this.Text = "Motion Detection";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbLast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCurrent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMotion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDifference)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIgnoreMotion)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbLast;
        private System.Windows.Forms.PictureBox pbCurrent;
        private System.Windows.Forms.PictureBox pbMotion;
        private System.Windows.Forms.Timer tmrCheckMotion;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnSource;
        private System.Windows.Forms.Button btnFormat;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.CheckBox cbOn;
        private System.Windows.Forms.TextBox tbSpeed;
        private System.Windows.Forms.TrackBar tbDifference;
        private System.Windows.Forms.PictureBox pbIgnoreMotion;
        private System.Windows.Forms.OpenFileDialog ofd;
    }
}

