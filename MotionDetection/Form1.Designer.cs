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
            this.serial = new System.IO.Ports.SerialPort(this.components);
            this.cbManual = new System.Windows.Forms.CheckBox();
            this.btnShowPorts = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tbMinX = new System.Windows.Forms.TrackBar();
            this.tbMaxY = new System.Windows.Forms.TrackBar();
            this.tbMinY = new System.Windows.Forms.TrackBar();
            this.tbMaxX = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCalLoad = new System.Windows.Forms.Button();
            this.btnCalSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbLast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCurrent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMotion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDifference)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIgnoreMotion)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbMinX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMaxY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMinY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMaxX)).BeginInit();
            this.SuspendLayout();
            // 
            // pbLast
            // 
            this.pbLast.Location = new System.Drawing.Point(12, 7);
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
            this.pbCurrent.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbCurrent_MouseMove);
            this.pbCurrent.Click += new System.EventHandler(this.pbCurrent_Click);
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
            this.cbOn.Location = new System.Drawing.Point(6, 6);
            this.cbOn.Name = "cbOn";
            this.cbOn.Size = new System.Drawing.Size(40, 17);
            this.cbOn.TabIndex = 11;
            this.cbOn.Text = "On";
            this.cbOn.UseVisualStyleBackColor = true;
            this.cbOn.CheckedChanged += new System.EventHandler(this.cbOn_CheckedChanged);
            // 
            // tbSpeed
            // 
            this.tbSpeed.Location = new System.Drawing.Point(119, 3);
            this.tbSpeed.Name = "tbSpeed";
            this.tbSpeed.Size = new System.Drawing.Size(43, 20);
            this.tbSpeed.TabIndex = 12;
            this.tbSpeed.Text = "50";
            this.tbSpeed.Validating += new System.ComponentModel.CancelEventHandler(this.tbSpeed_Validating);
            // 
            // tbDifference
            // 
            this.tbDifference.LargeChange = 10;
            this.tbDifference.Location = new System.Drawing.Point(6, 26);
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
            this.pbIgnoreMotion.Location = new System.Drawing.Point(188, 7);
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
            this.lbHistory.Location = new System.Drawing.Point(168, 3);
            this.lbHistory.Name = "lbHistory";
            this.lbHistory.Size = new System.Drawing.Size(164, 69);
            this.lbHistory.TabIndex = 18;
            // 
            // serial
            // 
            this.serial.PortName = "COM3";
            // 
            // cbManual
            // 
            this.cbManual.AutoSize = true;
            this.cbManual.Location = new System.Drawing.Point(52, 6);
            this.cbManual.Name = "cbManual";
            this.cbManual.Size = new System.Drawing.Size(61, 17);
            this.cbManual.TabIndex = 19;
            this.cbManual.Text = "Manual";
            this.cbManual.UseVisualStyleBackColor = true;
            // 
            // btnShowPorts
            // 
            this.btnShowPorts.Location = new System.Drawing.Point(36, 48);
            this.btnShowPorts.Name = "btnShowPorts";
            this.btnShowPorts.Size = new System.Drawing.Size(97, 23);
            this.btnShowPorts.TabIndex = 20;
            this.btnShowPorts.Text = "Show COM Ports";
            this.btnShowPorts.UseVisualStyleBackColor = true;
            this.btnShowPorts.Click += new System.EventHandler(this.btnShowPorts_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 133);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(346, 103);
            this.tabControl1.TabIndex = 21;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnShowPorts);
            this.tabPage1.Controls.Add(this.cbOn);
            this.tabPage1.Controls.Add(this.cbManual);
            this.tabPage1.Controls.Add(this.lbHistory);
            this.tabPage1.Controls.Add(this.tbDifference);
            this.tabPage1.Controls.Add(this.tbSpeed);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(338, 77);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Settings";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnCalSave);
            this.tabPage2.Controls.Add(this.btnCalLoad);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.tbMinY);
            this.tabPage2.Controls.Add(this.tbMaxY);
            this.tabPage2.Controls.Add(this.tbMinX);
            this.tabPage2.Controls.Add(this.tbMaxX);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(338, 77);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Calibrate";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tbMinX
            // 
            this.tbMinX.Location = new System.Drawing.Point(6, 29);
            this.tbMinX.Maximum = 180;
            this.tbMinX.Name = "tbMinX";
            this.tbMinX.Size = new System.Drawing.Size(160, 45);
            this.tbMinX.TabIndex = 0;
            this.tbMinX.TickFrequency = 0;
            this.tbMinX.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbMinX.Scroll += new System.EventHandler(this.tbMinX_Scroll);
            // 
            // tbMaxY
            // 
            this.tbMaxY.Location = new System.Drawing.Point(172, 57);
            this.tbMaxY.Maximum = 180;
            this.tbMaxY.Name = "tbMaxY";
            this.tbMaxY.Size = new System.Drawing.Size(160, 45);
            this.tbMaxY.TabIndex = 1;
            this.tbMaxY.TickFrequency = 0;
            this.tbMaxY.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbMaxY.Scroll += new System.EventHandler(this.tbMaxY_Scroll);
            // 
            // tbMinY
            // 
            this.tbMinY.Location = new System.Drawing.Point(6, 57);
            this.tbMinY.Maximum = 180;
            this.tbMinY.Name = "tbMinY";
            this.tbMinY.Size = new System.Drawing.Size(160, 45);
            this.tbMinY.TabIndex = 2;
            this.tbMinY.TickFrequency = 0;
            this.tbMinY.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbMinY.Scroll += new System.EventHandler(this.tbMinY_Scroll);
            // 
            // tbMaxX
            // 
            this.tbMaxX.Location = new System.Drawing.Point(172, 29);
            this.tbMaxX.Maximum = 180;
            this.tbMaxX.Name = "tbMaxX";
            this.tbMaxX.Size = new System.Drawing.Size(160, 45);
            this.tbMaxX.TabIndex = 3;
            this.tbMaxX.TickFrequency = 0;
            this.tbMaxX.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbMaxX.Scroll += new System.EventHandler(this.trackBar4_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Top Left of cam";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(207, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Bottom Right of cam";
            // 
            // btnCalLoad
            // 
            this.btnCalLoad.Location = new System.Drawing.Point(137, 3);
            this.btnCalLoad.Name = "btnCalLoad";
            this.btnCalLoad.Size = new System.Drawing.Size(29, 23);
            this.btnCalLoad.TabIndex = 22;
            this.btnCalLoad.Text = "LD";
            this.btnCalLoad.UseVisualStyleBackColor = true;
            this.btnCalLoad.Click += new System.EventHandler(this.btnCalLoad_Click);
            // 
            // btnCalSave
            // 
            this.btnCalSave.Location = new System.Drawing.Point(172, 3);
            this.btnCalSave.Name = "btnCalSave";
            this.btnCalSave.Size = new System.Drawing.Size(29, 23);
            this.btnCalSave.TabIndex = 23;
            this.btnCalSave.Text = "SV";
            this.btnCalSave.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 500);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblData);
            this.Controls.Add(this.pbIgnoreMotion);
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
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbMinX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMaxY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMinY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMaxX)).EndInit();
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
        private System.IO.Ports.SerialPort serial;
        private System.Windows.Forms.CheckBox cbManual;
        private System.Windows.Forms.Button btnShowPorts;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TrackBar tbMaxX;
        private System.Windows.Forms.TrackBar tbMinY;
        private System.Windows.Forms.TrackBar tbMaxY;
        private System.Windows.Forms.TrackBar tbMinX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCalSave;
        private System.Windows.Forms.Button btnCalLoad;
    }
}

