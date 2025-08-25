namespace AI.SteeringBehaviors.ApplicationPresentation
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.renderControl = new AI.SteeringBehaviors.ApplicationPresentation.RenderControl();
            this.label5 = new System.Windows.Forms.Label();
            this.flockRadiusSelector = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numShipsSelector = new System.Windows.Forms.NumericUpDown();
            this.studentImplementationButton = new System.Windows.Forms.RadioButton();
            this.exampleImplementationButton = new System.Windows.Forms.RadioButton();
            this.FPSInfo = new System.Windows.Forms.Label();
            this.totalShipsInfo = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.separationWeightInfo = new System.Windows.Forms.Label();
            this.cohesionWeightInfo = new System.Windows.Forms.Label();
            this.alignmentWeightInfo = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.separationWeightSelector = new System.Windows.Forms.TrackBar();
            this.cohesionWeightSelector = new System.Windows.Forms.TrackBar();
            this.alignmentWeightSelector = new System.Windows.Forms.TrackBar();
            this.colorButton = new System.Windows.Forms.Button();
            this.removeTaskForceButton = new System.Windows.Forms.Button();
            this.addTaskForceButton = new System.Windows.Forms.Button();
            this.taskForceSelector = new System.Windows.Forms.ListBox();
            this.updateTimer = new System.Timers.Timer();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.flockRadiusSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numShipsSelector)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.separationWeightSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cohesionWeightSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alignmentWeightSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updateTimer)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.renderControl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.flockRadiusSelector);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.numShipsSelector);
            this.splitContainer1.Panel2.Controls.Add(this.studentImplementationButton);
            this.splitContainer1.Panel2.Controls.Add(this.exampleImplementationButton);
            this.splitContainer1.Panel2.Controls.Add(this.FPSInfo);
            this.splitContainer1.Panel2.Controls.Add(this.totalShipsInfo);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.colorButton);
            this.splitContainer1.Panel2.Controls.Add(this.removeTaskForceButton);
            this.splitContainer1.Panel2.Controls.Add(this.addTaskForceButton);
            this.splitContainer1.Panel2.Controls.Add(this.taskForceSelector);
            this.splitContainer1.Size = new System.Drawing.Size(1264, 600);
            this.splitContainer1.SplitterDistance = 900;
            this.splitContainer1.TabIndex = 0;
            // 
            // renderControl
            // 
            this.renderControl.Location = new System.Drawing.Point(0, 0);
            this.renderControl.MinimumSize = new System.Drawing.Size(900, 600);
            this.renderControl.Name = "renderControl";
            this.renderControl.Size = new System.Drawing.Size(900, 600);
            this.renderControl.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(127, 237);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Flock Radius:";
            // 
            // flockRadiusSelector
            // 
            this.flockRadiusSelector.Location = new System.Drawing.Point(205, 235);
            this.flockRadiusSelector.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.flockRadiusSelector.Name = "flockRadiusSelector";
            this.flockRadiusSelector.Size = new System.Drawing.Size(62, 20);
            this.flockRadiusSelector.TabIndex = 26;
            this.flockRadiusSelector.ValueChanged += new System.EventHandler(this.flockRadiusSelector_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 237);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Ships:";
            // 
            // numShipsSelector
            // 
            this.numShipsSelector.Location = new System.Drawing.Point(57, 235);
            this.numShipsSelector.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numShipsSelector.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numShipsSelector.Name = "numShipsSelector";
            this.numShipsSelector.Size = new System.Drawing.Size(64, 20);
            this.numShipsSelector.TabIndex = 24;
            this.numShipsSelector.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numShipsSelector.ValueChanged += new System.EventHandler(this.numShipsSelector_ValueChanged);
            // 
            // studentImplementationButton
            // 
            this.studentImplementationButton.AutoSize = true;
            this.studentImplementationButton.Location = new System.Drawing.Point(6, 500);
            this.studentImplementationButton.Name = "studentImplementationButton";
            this.studentImplementationButton.Size = new System.Drawing.Size(136, 17);
            this.studentImplementationButton.TabIndex = 21;
            this.studentImplementationButton.Text = "Student Implementation";
            this.studentImplementationButton.UseVisualStyleBackColor = true;
            // 
            // exampleImplementationButton
            // 
            this.exampleImplementationButton.AutoSize = true;
            this.exampleImplementationButton.Checked = true;
            this.exampleImplementationButton.Location = new System.Drawing.Point(170, 500);
            this.exampleImplementationButton.Name = "exampleImplementationButton";
            this.exampleImplementationButton.Size = new System.Drawing.Size(178, 17);
            this.exampleImplementationButton.TabIndex = 22;
            this.exampleImplementationButton.TabStop = true;
            this.exampleImplementationButton.Text = "Example Implementation";
            this.exampleImplementationButton.UseVisualStyleBackColor = true;
            this.exampleImplementationButton.CheckedChanged += new System.EventHandler(this.exampleImplementationButton_CheckedChanged);
            // 
            // FPSInfo
            // 
            this.FPSInfo.AutoSize = true;
            this.FPSInfo.Location = new System.Drawing.Point(15, 534);
            this.FPSInfo.Name = "FPSInfo";
            this.FPSInfo.Size = new System.Drawing.Size(57, 13);
            this.FPSInfo.TabIndex = 20;
            this.FPSInfo.Text = "FPS: 0000";
            // 
            // totalShipsInfo
            // 
            this.totalShipsInfo.AutoSize = true;
            this.totalShipsInfo.Location = new System.Drawing.Point(15, 558);
            this.totalShipsInfo.Name = "totalShipsInfo";
            this.totalShipsInfo.Size = new System.Drawing.Size(96, 13);
            this.totalShipsInfo.TabIndex = 19;
            this.totalShipsInfo.Text = "Total Ships: 00000";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.separationWeightInfo);
            this.groupBox1.Controls.Add(this.cohesionWeightInfo);
            this.groupBox1.Controls.Add(this.alignmentWeightInfo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.separationWeightSelector);
            this.groupBox1.Controls.Add(this.cohesionWeightSelector);
            this.groupBox1.Controls.Add(this.alignmentWeightSelector);
            this.groupBox1.Location = new System.Drawing.Point(6, 278);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(354, 206);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Strength Weights";
            // 
            // separationWeightInfo
            // 
            this.separationWeightInfo.AutoSize = true;
            this.separationWeightInfo.Location = new System.Drawing.Point(288, 147);
            this.separationWeightInfo.Name = "separationWeightInfo";
            this.separationWeightInfo.Size = new System.Drawing.Size(28, 13);
            this.separationWeightInfo.TabIndex = 4;
            this.separationWeightInfo.Text = "1.00";
            // 
            // cohesionWeightInfo
            // 
            this.cohesionWeightInfo.AutoSize = true;
            this.cohesionWeightInfo.Location = new System.Drawing.Point(288, 96);
            this.cohesionWeightInfo.Name = "cohesionWeightInfo";
            this.cohesionWeightInfo.Size = new System.Drawing.Size(28, 13);
            this.cohesionWeightInfo.TabIndex = 3;
            this.cohesionWeightInfo.Text = "1.00";
            // 
            // alignmentWeightInfo
            // 
            this.alignmentWeightInfo.AutoSize = true;
            this.alignmentWeightInfo.Location = new System.Drawing.Point(288, 43);
            this.alignmentWeightInfo.Name = "alignmentWeightInfo";
            this.alignmentWeightInfo.Size = new System.Drawing.Size(28, 13);
            this.alignmentWeightInfo.TabIndex = 2;
            this.alignmentWeightInfo.Text = "1.00";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Separation";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Cohesion";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Alignment";
            // 
            // separationWeightSelector
            // 
            this.separationWeightSelector.LargeChange = 25;
            this.separationWeightSelector.Location = new System.Drawing.Point(82, 132);
            this.separationWeightSelector.Maximum = 500;
            this.separationWeightSelector.Name = "separationWeightSelector";
            this.separationWeightSelector.Size = new System.Drawing.Size(200, 45);
            this.separationWeightSelector.TabIndex = 0;
            this.separationWeightSelector.TickFrequency = 25;
            this.separationWeightSelector.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.separationWeightSelector.Value = 100;
            this.separationWeightSelector.ValueChanged += new System.EventHandler(this.separationWeightSelector_ValueChanged);
            // 
            // cohesionWeightSelector
            // 
            this.cohesionWeightSelector.LargeChange = 25;
            this.cohesionWeightSelector.Location = new System.Drawing.Point(82, 81);
            this.cohesionWeightSelector.Maximum = 500;
            this.cohesionWeightSelector.Name = "cohesionWeightSelector";
            this.cohesionWeightSelector.Size = new System.Drawing.Size(200, 45);
            this.cohesionWeightSelector.TabIndex = 0;
            this.cohesionWeightSelector.TickFrequency = 25;
            this.cohesionWeightSelector.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.cohesionWeightSelector.Value = 100;
            this.cohesionWeightSelector.ValueChanged += new System.EventHandler(this.cohesionWeightSelector_ValueChanged);
            // 
            // alignmentWeightSelector
            // 
            this.alignmentWeightSelector.LargeChange = 25;
            this.alignmentWeightSelector.Location = new System.Drawing.Point(82, 30);
            this.alignmentWeightSelector.Maximum = 500;
            this.alignmentWeightSelector.Name = "alignmentWeightSelector";
            this.alignmentWeightSelector.Size = new System.Drawing.Size(200, 45);
            this.alignmentWeightSelector.TabIndex = 0;
            this.alignmentWeightSelector.TickFrequency = 25;
            this.alignmentWeightSelector.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.alignmentWeightSelector.Value = 100;
            this.alignmentWeightSelector.ValueChanged += new System.EventHandler(this.alignmentWeightSelector_ValueChanged);
            // 
            // colorButton
            // 
            this.colorButton.Location = new System.Drawing.Point(273, 232);
            this.colorButton.Name = "colorButton";
            this.colorButton.Size = new System.Drawing.Size(75, 23);
            this.colorButton.TabIndex = 15;
            this.colorButton.Text = "Color";
            this.colorButton.UseVisualStyleBackColor = true;
            this.colorButton.Click += new System.EventHandler(this.colorButton_Click);
            // 
            // removeTaskForceButton
            // 
            this.removeTaskForceButton.AutoSize = true;
            this.removeTaskForceButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.removeTaskForceButton.Location = new System.Drawing.Point(240, 3);
            this.removeTaskForceButton.Name = "removeTaskForceButton";
            this.removeTaskForceButton.Size = new System.Drawing.Size(108, 23);
            this.removeTaskForceButton.TabIndex = 13;
            this.removeTaskForceButton.Text = "Remove Taskforce";
            this.removeTaskForceButton.UseVisualStyleBackColor = true;
            this.removeTaskForceButton.Click += new System.EventHandler(this.removeTaskForceButton_Click);
            // 
            // addTaskForceButton
            // 
            this.addTaskForceButton.AutoSize = true;
            this.addTaskForceButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.addTaskForceButton.Location = new System.Drawing.Point(3, 3);
            this.addTaskForceButton.Name = "addTaskForceButton";
            this.addTaskForceButton.Size = new System.Drawing.Size(87, 23);
            this.addTaskForceButton.TabIndex = 14;
            this.addTaskForceButton.Text = "Add Taskforce";
            this.addTaskForceButton.UseVisualStyleBackColor = true;
            this.addTaskForceButton.Click += new System.EventHandler(this.addTaskForceButton_Click);
            // 
            // taskForceSelector
            // 
            this.taskForceSelector.FormattingEnabled = true;
            this.taskForceSelector.Location = new System.Drawing.Point(6, 32);
            this.taskForceSelector.Name = "taskForceSelector";
            this.taskForceSelector.Size = new System.Drawing.Size(354, 186);
            this.taskForceSelector.TabIndex = 12;
            this.taskForceSelector.SelectedIndexChanged += new System.EventHandler(this.taskForceSelector_SelectedIndexChanged);
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 10D;
            this.updateTimer.SynchronizingObject = this;
            this.updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.updateTimer_Elapsed);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 600);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "Steering Behaviors";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.flockRadiusSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numShipsSelector)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.separationWeightSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cohesionWeightSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alignmentWeightSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updateTimer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private RenderControl renderControl;
        private System.Windows.Forms.RadioButton studentImplementationButton;
        private System.Windows.Forms.RadioButton exampleImplementationButton;
        private System.Windows.Forms.Label FPSInfo;
        private System.Windows.Forms.Label totalShipsInfo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label separationWeightInfo;
        private System.Windows.Forms.Label cohesionWeightInfo;
        private System.Windows.Forms.Label alignmentWeightInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar separationWeightSelector;
        private System.Windows.Forms.TrackBar cohesionWeightSelector;
        private System.Windows.Forms.TrackBar alignmentWeightSelector;
        private System.Windows.Forms.Button colorButton;
        private System.Windows.Forms.Button removeTaskForceButton;
        private System.Windows.Forms.Button addTaskForceButton;
        private System.Windows.Forms.ListBox taskForceSelector;
        private System.Timers.Timer updateTimer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numShipsSelector;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown flockRadiusSelector;
    }
}

