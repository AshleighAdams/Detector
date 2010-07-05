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
            this.pbLast = new System.Windows.Forms.PictureBox();
            this.pbCurrent = new System.Windows.Forms.PictureBox();
            this.pbMotion = new System.Windows.Forms.PictureBox();
            this.tmrCheckMotion = new System.Windows.Forms.Timer(this.components);
            this.cbOn = new System.Windows.Forms.CheckBox();
            this.tbSpeed = new System.Windows.Forms.TextBox();
            this.tbDifference = new System.Windows.Forms.TrackBar();
            this.pbIgnoreMotion = new System.Windows.Forms.PictureBox();
            this.lblData = new System.Windows.Forms.Label();
            this.lbHistory = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbLast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCurrent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMotion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDifference)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIgnoreMotion)).BeginInit();
            this.SuspendLayout();
            // 
            // pbLast
            // 
            this.pbLast.Location = new System.Drawing.Point(12, 12);
            this.pbLast.Name = "pbLast";
            this.pbLast.Size = new System.Drawing.Size(170, 120);
            this.pbLast.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbLast.TabIndex = 0;
            this.pbLast.TabStop = false;
            // 
            // pbCurrent
            // 
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
            // cbOn
            // 
            this.cbOn.AutoSize = true;
            this.cbOn.Location = new System.Drawing.Point(12, 161);
            this.cbOn.Name = "cbOn";
            this.cbOn.Size = new System.Drawing.Size(40, 17);
            this.cbOn.TabIndex = 11;
            this.cbOn.Text = "On";
            this.cbOn.UseVisualStyleBackColor = true;
            this.cbOn.CheckedChanged += new System.EventHandler(this.cbOn_CheckedChanged);
            // 
            // tbSpeed
            // 
            this.tbSpeed.Location = new System.Drawing.Point(139, 159);
            this.tbSpeed.Name = "tbSpeed";
            this.tbSpeed.Size = new System.Drawing.Size(43, 20);
            this.tbSpeed.TabIndex = 12;
            this.tbSpeed.Text = "50";
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
            this.pbIgnoreMotion.Location = new System.Drawing.Point(188, 12);
            this.pbIgnoreMotion.Name = "pbIgnoreMotion";
            this.pbIgnoreMotion.Size = new System.Drawing.Size(170, 120);
            this.pbIgnoreMotion.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbIgnoreMotion.TabIndex = 16;
            this.pbIgnoreMotion.TabStop = false;
            this.pbIgnoreMotion.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pbIgnoreMotion_MouseDoubleClick);
            this.pbIgnoreMotion.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbIgnoreMotion_MouseDown);
            this.pbIgnoreMotion.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbIgnoreMotion_MouseUp);
            // 
            // lblData
            // 
            this.lblData.AutoSize = true;
            this.lblData.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblData.ForeColor = System.Drawing.Color.Red;
            this.lblData.Location = new System.Drawing.Point(378, 24);
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(0, 18);
            this.lblData.TabIndex = 17;
            // 
            // lbHistory
            // 
            this.lbHistory.FormattingEnabled = true;
            this.lbHistory.Location = new System.Drawing.Point(188, 141);
            this.lbHistory.Name = "lbHistory";
            this.lbHistory.Size = new System.Drawing.Size(170, 95);
            this.lbHistory.TabIndex = 18;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 500);
            this.Controls.Add(this.lbHistory);
            this.Controls.Add(this.lblData);
            this.Controls.Add(this.pbIgnoreMotion);
            this.Controls.Add(this.tbDifference);
            this.Controls.Add(this.tbSpeed);
            this.Controls.Add(this.cbOn);
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
        private System.Windows.Forms.CheckBox cbOn;
        private System.Windows.Forms.TextBox tbSpeed;
        private System.Windows.Forms.TrackBar tbDifference;
        private System.Windows.Forms.PictureBox pbIgnoreMotion;
        private System.Windows.Forms.Label lblData;
        private System.Windows.Forms.ListBox lbHistory;
    }
}

