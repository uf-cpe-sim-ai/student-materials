using System;
using System.Windows.Forms;

namespace GameAI.GamePlaying
{
	/// <summary>
	/// Summary description for OptionsDialog.
	/// </summary>
	public class OptionsDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabControl optionsTabControl;
		private System.Windows.Forms.TabPage displayTabPage;
		private System.Windows.Forms.CheckBox showValidMovesCheckBox;
		private System.Windows.Forms.CheckBox previewMovesCheckBox;
		private System.Windows.Forms.CheckBox animateMovesCheckBox;
		private System.Windows.Forms.Label boardColorLabel;
		private System.Windows.Forms.Panel boardColorPanel;
		private System.Windows.Forms.Button boardColorButton;
		private System.Windows.Forms.Label validColorLabel;
		private System.Windows.Forms.Panel validColorPanel;
		private System.Windows.Forms.Button validColorButton;
		private System.Windows.Forms.Label activeColorLabel;
		private System.Windows.Forms.Panel activeColorPanel;
		private System.Windows.Forms.Button activeColorButton;
		private System.Windows.Forms.Label moveIndicatorColorLabel;
		private System.Windows.Forms.Panel moveIndicatorColorPanel;
		private System.Windows.Forms.Button moveIndicatorColorButton;
		private System.Windows.Forms.TabPage playersTabPage;
		private System.Windows.Forms.Panel blackPlayerPanel;
		private System.Windows.Forms.RadioButton blackPlayerComputerRadioButton;
		private System.Windows.Forms.RadioButton blackPlayerUserRadioButton;
		private System.Windows.Forms.Panel whitePlayerPanel;
		private System.Windows.Forms.RadioButton whitePlayerComputerRadioButton;
		private System.Windows.Forms.RadioButton whitePlayerUserRadioButton;
		private System.Windows.Forms.Label blackPlayerLabel;
		private System.Windows.Forms.Label whitePlayerLabel;
		private System.Windows.Forms.TabPage difficultyTabPage;
		private System.Windows.Forms.Button restoreDefaultsButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		/// 
		private System.ComponentModel.Container components = null;

		// The game options.
		public Options Options;
		private GroupBox groupBox2;
		private GroupBox groupBox1;
		private TableLayoutPanel tableLayoutPanel1;
		private TrackBar blackDifficultyTB;
		private TrackBar whiteDifficultyTB;
		private Label blackDifficultyLBL;
		private Label whiteDifficultyLBL;
		private RadioButton whiteExampleAIRadioButton;
		private RadioButton blackExampleAIRadioButton;

		// An array to store custom colors added by the user.
		private static int[] customColors = new int[] {};

		public OptionsDialog(Options options)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// Create a copy of the given game options.
			this.Options = new Options(options);

			// Set the form controls based on those options.
			this.MapOptionsToControls();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.optionsTabControl = new System.Windows.Forms.TabControl();
            this.displayTabPage = new System.Windows.Forms.TabPage();
            this.previewMovesCheckBox = new System.Windows.Forms.CheckBox();
            this.moveIndicatorColorButton = new System.Windows.Forms.Button();
            this.moveIndicatorColorPanel = new System.Windows.Forms.Panel();
            this.moveIndicatorColorLabel = new System.Windows.Forms.Label();
            this.animateMovesCheckBox = new System.Windows.Forms.CheckBox();
            this.validColorButton = new System.Windows.Forms.Button();
            this.validColorPanel = new System.Windows.Forms.Panel();
            this.validColorLabel = new System.Windows.Forms.Label();
            this.activeColorButton = new System.Windows.Forms.Button();
            this.activeColorPanel = new System.Windows.Forms.Panel();
            this.activeColorLabel = new System.Windows.Forms.Label();
            this.boardColorButton = new System.Windows.Forms.Button();
            this.boardColorPanel = new System.Windows.Forms.Panel();
            this.boardColorLabel = new System.Windows.Forms.Label();
            this.showValidMovesCheckBox = new System.Windows.Forms.CheckBox();
            this.playersTabPage = new System.Windows.Forms.TabPage();
            this.whitePlayerPanel = new System.Windows.Forms.Panel();
            this.whitePlayerUserRadioButton = new System.Windows.Forms.RadioButton();
            this.whiteExampleAIRadioButton = new System.Windows.Forms.RadioButton();
            this.whitePlayerComputerRadioButton = new System.Windows.Forms.RadioButton();
            this.whitePlayerLabel = new System.Windows.Forms.Label();
            this.blackPlayerPanel = new System.Windows.Forms.Panel();
            this.blackPlayerUserRadioButton = new System.Windows.Forms.RadioButton();
            this.blackExampleAIRadioButton = new System.Windows.Forms.RadioButton();
            this.blackPlayerComputerRadioButton = new System.Windows.Forms.RadioButton();
            this.blackPlayerLabel = new System.Windows.Forms.Label();
            this.difficultyTabPage = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.blackDifficultyLBL = new System.Windows.Forms.Label();
            this.blackDifficultyTB = new System.Windows.Forms.TrackBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.whiteDifficultyLBL = new System.Windows.Forms.Label();
            this.whiteDifficultyTB = new System.Windows.Forms.TrackBar();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.restoreDefaultsButton = new System.Windows.Forms.Button();
            this.optionsTabControl.SuspendLayout();
            this.displayTabPage.SuspendLayout();
            this.playersTabPage.SuspendLayout();
            this.whitePlayerPanel.SuspendLayout();
            this.blackPlayerPanel.SuspendLayout();
            this.difficultyTabPage.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blackDifficultyTB)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.whiteDifficultyTB)).BeginInit();
            this.SuspendLayout();
            // 
            // optionsTabControl
            // 
            this.optionsTabControl.Controls.Add(this.displayTabPage);
            this.optionsTabControl.Controls.Add(this.playersTabPage);
            this.optionsTabControl.Controls.Add(this.difficultyTabPage);
            this.optionsTabControl.Location = new System.Drawing.Point(8, 16);
            this.optionsTabControl.Name = "optionsTabControl";
            this.optionsTabControl.SelectedIndex = 0;
            this.optionsTabControl.Size = new System.Drawing.Size(288, 216);
            this.optionsTabControl.TabIndex = 0;
            // 
            // displayTabPage
            // 
            this.displayTabPage.Controls.Add(this.previewMovesCheckBox);
            this.displayTabPage.Controls.Add(this.moveIndicatorColorButton);
            this.displayTabPage.Controls.Add(this.moveIndicatorColorPanel);
            this.displayTabPage.Controls.Add(this.moveIndicatorColorLabel);
            this.displayTabPage.Controls.Add(this.animateMovesCheckBox);
            this.displayTabPage.Controls.Add(this.validColorButton);
            this.displayTabPage.Controls.Add(this.validColorPanel);
            this.displayTabPage.Controls.Add(this.validColorLabel);
            this.displayTabPage.Controls.Add(this.activeColorButton);
            this.displayTabPage.Controls.Add(this.activeColorPanel);
            this.displayTabPage.Controls.Add(this.activeColorLabel);
            this.displayTabPage.Controls.Add(this.boardColorButton);
            this.displayTabPage.Controls.Add(this.boardColorPanel);
            this.displayTabPage.Controls.Add(this.boardColorLabel);
            this.displayTabPage.Controls.Add(this.showValidMovesCheckBox);
            this.displayTabPage.Location = new System.Drawing.Point(4, 22);
            this.displayTabPage.Name = "displayTabPage";
            this.displayTabPage.Size = new System.Drawing.Size(280, 190);
            this.displayTabPage.TabIndex = 0;
            this.displayTabPage.Text = "Display";
            // 
            // previewMovesCheckBox
            // 
            this.previewMovesCheckBox.Location = new System.Drawing.Point(144, 10);
            this.previewMovesCheckBox.Name = "previewMovesCheckBox";
            this.previewMovesCheckBox.Size = new System.Drawing.Size(104, 24);
            this.previewMovesCheckBox.TabIndex = 1;
            this.previewMovesCheckBox.Text = "Preview moves";
            // 
            // moveIndicatorColorButton
            // 
            this.moveIndicatorColorButton.Location = new System.Drawing.Point(175, 157);
            this.moveIndicatorColorButton.Name = "moveIndicatorColorButton";
            this.moveIndicatorColorButton.Size = new System.Drawing.Size(75, 23);
            this.moveIndicatorColorButton.TabIndex = 14;
            this.moveIndicatorColorButton.Text = "Select...";
            this.moveIndicatorColorButton.Click += new System.EventHandler(this.moveIndicatorColorButton_Click);
            // 
            // moveIndicatorColorPanel
            // 
            this.moveIndicatorColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.moveIndicatorColorPanel.Location = new System.Drawing.Point(143, 160);
            this.moveIndicatorColorPanel.Name = "moveIndicatorColorPanel";
            this.moveIndicatorColorPanel.Size = new System.Drawing.Size(16, 16);
            this.moveIndicatorColorPanel.TabIndex = 13;
            // 
            // moveIndicatorColorLabel
            // 
            this.moveIndicatorColorLabel.AutoSize = true;
            this.moveIndicatorColorLabel.Location = new System.Drawing.Point(27, 162);
            this.moveIndicatorColorLabel.Name = "moveIndicatorColorLabel";
            this.moveIndicatorColorLabel.Size = new System.Drawing.Size(106, 13);
            this.moveIndicatorColorLabel.TabIndex = 12;
            this.moveIndicatorColorLabel.Text = "Move indicator color:";
            // 
            // animateMovesCheckBox
            // 
            this.animateMovesCheckBox.Location = new System.Drawing.Point(16, 34);
            this.animateMovesCheckBox.Name = "animateMovesCheckBox";
            this.animateMovesCheckBox.Size = new System.Drawing.Size(104, 24);
            this.animateMovesCheckBox.TabIndex = 2;
            this.animateMovesCheckBox.Text = "Animate moves";
            // 
            // validColorButton
            // 
            this.validColorButton.Location = new System.Drawing.Point(175, 93);
            this.validColorButton.Name = "validColorButton";
            this.validColorButton.Size = new System.Drawing.Size(75, 23);
            this.validColorButton.TabIndex = 8;
            this.validColorButton.Text = "Select...";
            this.validColorButton.Click += new System.EventHandler(this.validColorButton_Click);
            // 
            // validColorPanel
            // 
            this.validColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.validColorPanel.Location = new System.Drawing.Point(143, 96);
            this.validColorPanel.Name = "validColorPanel";
            this.validColorPanel.Size = new System.Drawing.Size(16, 16);
            this.validColorPanel.TabIndex = 7;
            // 
            // validColorLabel
            // 
            this.validColorLabel.AutoSize = true;
            this.validColorLabel.Location = new System.Drawing.Point(45, 98);
            this.validColorLabel.Name = "validColorLabel";
            this.validColorLabel.Size = new System.Drawing.Size(88, 13);
            this.validColorLabel.TabIndex = 6;
            this.validColorLabel.Text = "Valid move color:";
            // 
            // activeColorButton
            // 
            this.activeColorButton.Location = new System.Drawing.Point(175, 125);
            this.activeColorButton.Name = "activeColorButton";
            this.activeColorButton.Size = new System.Drawing.Size(75, 23);
            this.activeColorButton.TabIndex = 11;
            this.activeColorButton.Text = "Select...";
            this.activeColorButton.Click += new System.EventHandler(this.activeColorButton_Click);
            // 
            // activeColorPanel
            // 
            this.activeColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.activeColorPanel.Location = new System.Drawing.Point(143, 128);
            this.activeColorPanel.Name = "activeColorPanel";
            this.activeColorPanel.Size = new System.Drawing.Size(16, 16);
            this.activeColorPanel.TabIndex = 10;
            // 
            // activeColorLabel
            // 
            this.activeColorLabel.AutoSize = true;
            this.activeColorLabel.Location = new System.Drawing.Point(32, 130);
            this.activeColorLabel.Name = "activeColorLabel";
            this.activeColorLabel.Size = new System.Drawing.Size(101, 13);
            this.activeColorLabel.TabIndex = 9;
            this.activeColorLabel.Text = "Active square color:";
            // 
            // boardColorButton
            // 
            this.boardColorButton.Location = new System.Drawing.Point(175, 61);
            this.boardColorButton.Name = "boardColorButton";
            this.boardColorButton.Size = new System.Drawing.Size(75, 23);
            this.boardColorButton.TabIndex = 5;
            this.boardColorButton.Text = "Select...";
            this.boardColorButton.Click += new System.EventHandler(this.boardColorButton_Click);
            // 
            // boardColorPanel
            // 
            this.boardColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.boardColorPanel.Location = new System.Drawing.Point(143, 64);
            this.boardColorPanel.Name = "boardColorPanel";
            this.boardColorPanel.Size = new System.Drawing.Size(16, 16);
            this.boardColorPanel.TabIndex = 4;
            // 
            // boardColorLabel
            // 
            this.boardColorLabel.AutoSize = true;
            this.boardColorLabel.Location = new System.Drawing.Point(71, 66);
            this.boardColorLabel.Name = "boardColorLabel";
            this.boardColorLabel.Size = new System.Drawing.Size(64, 13);
            this.boardColorLabel.TabIndex = 3;
            this.boardColorLabel.Text = "Board color:";
            // 
            // showValidMovesCheckBox
            // 
            this.showValidMovesCheckBox.Location = new System.Drawing.Point(16, 10);
            this.showValidMovesCheckBox.Name = "showValidMovesCheckBox";
            this.showValidMovesCheckBox.Size = new System.Drawing.Size(120, 24);
            this.showValidMovesCheckBox.TabIndex = 0;
            this.showValidMovesCheckBox.Text = "Show valid moves";
            // 
            // playersTabPage
            // 
            this.playersTabPage.Controls.Add(this.whitePlayerPanel);
            this.playersTabPage.Controls.Add(this.blackPlayerPanel);
            this.playersTabPage.Location = new System.Drawing.Point(4, 22);
            this.playersTabPage.Name = "playersTabPage";
            this.playersTabPage.Size = new System.Drawing.Size(280, 190);
            this.playersTabPage.TabIndex = 1;
            this.playersTabPage.Text = "Players";
            // 
            // whitePlayerPanel
            // 
            this.whitePlayerPanel.Controls.Add(this.whitePlayerUserRadioButton);
            this.whitePlayerPanel.Controls.Add(this.whiteExampleAIRadioButton);
            this.whitePlayerPanel.Controls.Add(this.whitePlayerComputerRadioButton);
            this.whitePlayerPanel.Controls.Add(this.whitePlayerLabel);
            this.whitePlayerPanel.Location = new System.Drawing.Point(40, 99);
            this.whitePlayerPanel.Name = "whitePlayerPanel";
            this.whitePlayerPanel.Size = new System.Drawing.Size(200, 82);
            this.whitePlayerPanel.TabIndex = 5;
            // 
            // whitePlayerUserRadioButton
            // 
            this.whitePlayerUserRadioButton.Location = new System.Drawing.Point(88, 54);
            this.whitePlayerUserRadioButton.Name = "whitePlayerUserRadioButton";
            this.whitePlayerUserRadioButton.Size = new System.Drawing.Size(104, 24);
            this.whitePlayerUserRadioButton.TabIndex = 1;
            this.whitePlayerUserRadioButton.Text = "User";
            // 
            // whiteExampleAIRadioButton
            // 
            this.whiteExampleAIRadioButton.Location = new System.Drawing.Point(88, 6);
            this.whiteExampleAIRadioButton.Name = "whiteExampleAIRadioButton";
            this.whiteExampleAIRadioButton.Size = new System.Drawing.Size(104, 24);
            this.whiteExampleAIRadioButton.TabIndex = 0;
            this.whiteExampleAIRadioButton.Text = "Example AI";
            // 
            // whitePlayerComputerRadioButton
            // 
            this.whitePlayerComputerRadioButton.Location = new System.Drawing.Point(88, 30);
            this.whitePlayerComputerRadioButton.Name = "whitePlayerComputerRadioButton";
            this.whitePlayerComputerRadioButton.Size = new System.Drawing.Size(104, 24);
            this.whitePlayerComputerRadioButton.TabIndex = 0;
            this.whitePlayerComputerRadioButton.Text = "Student AI";
            // 
            // whitePlayerLabel
            // 
            this.whitePlayerLabel.AutoSize = true;
            this.whitePlayerLabel.Location = new System.Drawing.Point(8, 12);
            this.whitePlayerLabel.Name = "whitePlayerLabel";
            this.whitePlayerLabel.Size = new System.Drawing.Size(69, 13);
            this.whitePlayerLabel.TabIndex = 4;
            this.whitePlayerLabel.Text = "White player:";
            // 
            // blackPlayerPanel
            // 
            this.blackPlayerPanel.Controls.Add(this.blackPlayerUserRadioButton);
            this.blackPlayerPanel.Controls.Add(this.blackExampleAIRadioButton);
            this.blackPlayerPanel.Controls.Add(this.blackPlayerComputerRadioButton);
            this.blackPlayerPanel.Controls.Add(this.blackPlayerLabel);
            this.blackPlayerPanel.Location = new System.Drawing.Point(40, 9);
            this.blackPlayerPanel.Name = "blackPlayerPanel";
            this.blackPlayerPanel.Size = new System.Drawing.Size(200, 79);
            this.blackPlayerPanel.TabIndex = 3;
            // 
            // blackPlayerUserRadioButton
            // 
            this.blackPlayerUserRadioButton.Location = new System.Drawing.Point(88, 54);
            this.blackPlayerUserRadioButton.Name = "blackPlayerUserRadioButton";
            this.blackPlayerUserRadioButton.Size = new System.Drawing.Size(104, 24);
            this.blackPlayerUserRadioButton.TabIndex = 2;
            this.blackPlayerUserRadioButton.Text = "User";
            // 
            // blackExampleAIRadioButton
            // 
            this.blackExampleAIRadioButton.Location = new System.Drawing.Point(88, 6);
            this.blackExampleAIRadioButton.Name = "blackExampleAIRadioButton";
            this.blackExampleAIRadioButton.Size = new System.Drawing.Size(104, 24);
            this.blackExampleAIRadioButton.TabIndex = 0;
            this.blackExampleAIRadioButton.Text = "Example AI";
            // 
            // blackPlayerComputerRadioButton
            // 
            this.blackPlayerComputerRadioButton.Location = new System.Drawing.Point(88, 30);
            this.blackPlayerComputerRadioButton.Name = "blackPlayerComputerRadioButton";
            this.blackPlayerComputerRadioButton.Size = new System.Drawing.Size(104, 24);
            this.blackPlayerComputerRadioButton.TabIndex = 1;
            this.blackPlayerComputerRadioButton.Text = "Student AI";
            // 
            // blackPlayerLabel
            // 
            this.blackPlayerLabel.AutoSize = true;
            this.blackPlayerLabel.Location = new System.Drawing.Point(9, 12);
            this.blackPlayerLabel.Name = "blackPlayerLabel";
            this.blackPlayerLabel.Size = new System.Drawing.Size(68, 13);
            this.blackPlayerLabel.TabIndex = 2;
            this.blackPlayerLabel.Text = "Black player:";
            // 
            // difficultyTabPage
            // 
            this.difficultyTabPage.Controls.Add(this.tableLayoutPanel1);
            this.difficultyTabPage.Location = new System.Drawing.Point(4, 22);
            this.difficultyTabPage.Name = "difficultyTabPage";
            this.difficultyTabPage.Size = new System.Drawing.Size(280, 190);
            this.difficultyTabPage.TabIndex = 2;
            this.difficultyTabPage.Text = "Difficulty";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(280, 190);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.blackDifficultyLBL);
            this.groupBox1.Controls.Add(this.blackDifficultyTB);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(274, 89);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Black Player";
            // 
            // blackDifficultyLBL
            // 
            this.blackDifficultyLBL.AutoSize = true;
            this.blackDifficultyLBL.Location = new System.Drawing.Point(6, 67);
            this.blackDifficultyLBL.Name = "blackDifficultyLBL";
            this.blackDifficultyLBL.Size = new System.Drawing.Size(160, 13);
            this.blackDifficultyLBL.TabIndex = 1;
            this.blackDifficultyLBL.Text = "Look Ahead Depth: 0 - Beginner";
            // 
            // blackDifficultyTB
            // 
            this.blackDifficultyTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blackDifficultyTB.LargeChange = 1;
            this.blackDifficultyTB.Location = new System.Drawing.Point(6, 19);
            this.blackDifficultyTB.Maximum = 2;
            this.blackDifficultyTB.Name = "blackDifficultyTB";
            this.blackDifficultyTB.Size = new System.Drawing.Size(263, 45);
            this.blackDifficultyTB.TabIndex = 0;
            this.blackDifficultyTB.Scroll += new System.EventHandler(this.blackDifficultyTB_Scroll);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.whiteDifficultyLBL);
            this.groupBox2.Controls.Add(this.whiteDifficultyTB);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 98);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(274, 89);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "White Player";
            // 
            // whiteDifficultyLBL
            // 
            this.whiteDifficultyLBL.AutoSize = true;
            this.whiteDifficultyLBL.Location = new System.Drawing.Point(6, 67);
            this.whiteDifficultyLBL.Name = "whiteDifficultyLBL";
            this.whiteDifficultyLBL.Size = new System.Drawing.Size(160, 13);
            this.whiteDifficultyLBL.TabIndex = 1;
            this.whiteDifficultyLBL.Text = "Look Ahead Depth: 0 - Beginner";
            // 
            // whiteDifficultyTB
            // 
            this.whiteDifficultyTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.whiteDifficultyTB.LargeChange = 1;
            this.whiteDifficultyTB.Location = new System.Drawing.Point(6, 19);
            this.whiteDifficultyTB.Maximum = 2;
            this.whiteDifficultyTB.Name = "whiteDifficultyTB";
            this.whiteDifficultyTB.Size = new System.Drawing.Size(263, 45);
            this.whiteDifficultyTB.TabIndex = 0;
            this.whiteDifficultyTB.Scroll += new System.EventHandler(this.whiteDifficultyTB_Scroll);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(136, 240);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(221, 240);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            // 
            // restoreDefaultsButton
            // 
            this.restoreDefaultsButton.Location = new System.Drawing.Point(8, 240);
            this.restoreDefaultsButton.Name = "restoreDefaultsButton";
            this.restoreDefaultsButton.Size = new System.Drawing.Size(96, 23);
            this.restoreDefaultsButton.TabIndex = 3;
            this.restoreDefaultsButton.Text = "Restore Defaults";
            this.restoreDefaultsButton.Click += new System.EventHandler(this.restoreDefaultsButton_Click);
            // 
            // OptionsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(304, 276);
            this.ControlBox = false;
            this.Controls.Add(this.restoreDefaultsButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.optionsTabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.optionsTabControl.ResumeLayout(false);
            this.displayTabPage.ResumeLayout(false);
            this.displayTabPage.PerformLayout();
            this.playersTabPage.ResumeLayout(false);
            this.whitePlayerPanel.ResumeLayout(false);
            this.whitePlayerPanel.PerformLayout();
            this.blackPlayerPanel.ResumeLayout(false);
            this.blackPlayerPanel.PerformLayout();
            this.difficultyTabPage.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blackDifficultyTB)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.whiteDifficultyTB)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		//
		// Sets the form controls based on the current game options.
		//
		private void MapOptionsToControls()
		{
			this.showValidMovesCheckBox.Checked    = this.Options.ShowValidMoves;
			this.previewMovesCheckBox.Checked      = this.Options.PreviewMoves;
			this.animateMovesCheckBox.Checked      = this.Options.AnimateMoves;
			this.boardColorPanel.BackColor         = this.Options.BoardColor;
			this.validColorPanel.BackColor         = this.Options.ValidMoveColor;
			this.activeColorPanel.BackColor        = this.Options.ActiveSquareColor;
			this.moveIndicatorColorPanel.BackColor = this.Options.MoveIndicatorColor;

			this.blackPlayerComputerRadioButton.Checked = this.Options.ComputerPlaysBlack & (!this.Options.BlackUsesExampleAI);
			this.blackPlayerUserRadioButton.Checked     = !this.Options.ComputerPlaysBlack;
			this.blackExampleAIRadioButton.Checked     = this.Options.ComputerPlaysBlack & this.Options.BlackUsesExampleAI;
			this.whitePlayerComputerRadioButton.Checked = this.Options.ComputerPlaysWhite & (!this.Options.WhiteUsesExampleAI);
			this.whitePlayerUserRadioButton.Checked     = !this.Options.ComputerPlaysWhite;
			this.whiteExampleAIRadioButton.Checked     = this.Options.ComputerPlaysWhite & this.Options.WhiteUsesExampleAI;

			switch(this.Options.BlackDifficulty)
			{
				case 1:
					this.blackDifficultyTB.Value = 1;
					this.blackDifficultyLBL.Text = "Look Ahead Depth: 1 - Intermediate";
					break;
				case 3:
					this.blackDifficultyTB.Value = 2;
					this.blackDifficultyLBL.Text = "Look Ahead Depth: 3 - Advanced";
					break;
				case 5:
					this.blackDifficultyTB.Value = 2;
					this.blackDifficultyLBL.Text = "Look Ahead Depth: 3 - Advanced";
					this.Options.BlackDifficulty = 3;
					break;
				default:
					this.blackDifficultyTB.Value = 0;
					this.blackDifficultyLBL.Text = "Look Ahead Depth: 0 - Beginner";
					break;
			}
			switch (this.Options.WhiteDifficulty)
			{
				case 1:
					this.whiteDifficultyTB.Value = 1;
					this.whiteDifficultyLBL.Text = "Look Ahead Depth: 1 - Intermediate";
					break;
				case 3:
					this.whiteDifficultyTB.Value = 2;
					this.whiteDifficultyLBL.Text = "Look Ahead Depth: 3 - Advanced";
					break;
				case 5:
					this.whiteDifficultyTB.Value = 2;
					this.whiteDifficultyLBL.Text = "Look Ahead Depth: 3 - Advanced";
					this.Options.WhiteDifficulty = 3;
					break;
				default:
					this.whiteDifficultyTB.Value = 0;
					this.whiteDifficultyLBL.Text = "Look Ahead Depth: 0 - Beginner";
					break;
			}

			this.Refresh();
		}

		//
		// Sets the game options based on the current state of the form
		// controls.
		//
		private void MapControlsToOptions()
		{
			this.Options.ShowValidMoves     = this.showValidMovesCheckBox.Checked;
			this.Options.PreviewMoves       = this.previewMovesCheckBox.Checked;
			this.Options.AnimateMoves       = this.animateMovesCheckBox.Checked;
			this.Options.BoardColor         = this.boardColorPanel.BackColor;
			this.Options.ValidMoveColor     = this.validColorPanel.BackColor;
			this.Options.ActiveSquareColor  = this.activeColorPanel.BackColor;
			this.Options.MoveIndicatorColor = this.moveIndicatorColorPanel.BackColor;

			if (this.blackPlayerComputerRadioButton.Checked)
			{
				this.Options.ComputerPlaysBlack = true;
				this.Options.BlackUsesExampleAI = false;
			}
			else if (this.blackExampleAIRadioButton.Checked)
			{
				this.Options.ComputerPlaysBlack = true;
				this.Options.BlackUsesExampleAI = true;
			}
			else
				this.Options.ComputerPlaysBlack = false;
			if (this.whitePlayerComputerRadioButton.Checked)
			{
				this.Options.ComputerPlaysWhite = true;
				this.Options.WhiteUsesExampleAI = false;
			}
			else if (this.whiteExampleAIRadioButton.Checked)
			{
				this.Options.ComputerPlaysWhite = true;
				this.Options.WhiteUsesExampleAI = true;
			}
			else
				this.Options.ComputerPlaysWhite = false;

			this.Options.BlackDifficulty = Math.Max(blackDifficultyTB.Value * 2 - 1, 0);
			this.Options.WhiteDifficulty = Math.Max(whiteDifficultyTB.Value * 2 - 1, 0);
		}

		// ===================================================================
		// Event handlers for the color select buttons.
		// ===================================================================

		private void boardColorButton_Click(object sender, System.EventArgs e)
		{
			// Open a color dialog.
			ColorDialog dlg = new ColorDialog();
			dlg.Color = this.boardColorPanel.BackColor;
			dlg.CustomColors = OptionsDialog.customColors;

			// Set the board color based on that selection.
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				this.boardColorPanel.BackColor = dlg.Color;
				this.boardColorPanel.Refresh();
				OptionsDialog.customColors = dlg.CustomColors;
			}
		}

		private void validColorButton_Click(object sender, System.EventArgs e)
		{
			// Open a color dialog.
			ColorDialog dlg = new ColorDialog();
			dlg.Color = this.validColorPanel.BackColor;
			dlg.CustomColors = OptionsDialog.customColors;

			// Set the valid move color based on that selection.
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				this.validColorPanel.BackColor = dlg.Color;
				this.validColorPanel.Refresh();
				OptionsDialog.customColors = dlg.CustomColors;
			}
		}

		private void activeColorButton_Click(object sender, System.EventArgs e)
		{
			// Open a color dialog.
			ColorDialog dlg = new ColorDialog();
			dlg.Color = this.activeColorPanel.BackColor;
			dlg.CustomColors = OptionsDialog.customColors;

			// Set the active square color based on that selection.
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				this.activeColorPanel.BackColor = dlg.Color;
				this.activeColorPanel.Refresh();
				OptionsDialog.customColors = dlg.CustomColors;
			}
		}

		private void moveIndicatorColorButton_Click(object sender, System.EventArgs e)
		{
			// Open a color dialog.
			ColorDialog dlg = new ColorDialog();
			dlg.Color = this.moveIndicatorColorPanel.BackColor;
			dlg.CustomColors = OptionsDialog.customColors;

			// Set the move indicator color based on that selection.
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				this.moveIndicatorColorPanel.BackColor = dlg.Color;
				this.moveIndicatorColorPanel.Refresh();
				OptionsDialog.customColors = dlg.CustomColors;
			}
		}

		// ===================================================================
		// Event handlers for the form buttons.
		// ===================================================================

		private void restoreDefaultsButton_Click(object sender, System.EventArgs e)
		{
			// Reset the game options to their defaults.
			this.Options.RestoreDefaults();

			// Update the form controls.
			this.MapOptionsToControls();
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			// Set game options based on the form control values.
			this.MapControlsToOptions();
		}

		private void blackDifficultyTB_Scroll(object sender, EventArgs e)
		{
			switch(blackDifficultyTB.Value)
			{
				case 0:
					this.blackDifficultyLBL.Text = "Look Ahead Depth: 0 - Beginner";
					break;
				case 1:
					this.blackDifficultyLBL.Text = "Look Ahead Depth: 1 - Intermediate";
					break;
				case 2:
					this.blackDifficultyLBL.Text = "Look Ahead Depth: 3 - Advanced";
					break;
			}
		}

		private void whiteDifficultyTB_Scroll(object sender, EventArgs e)
		{
			switch (whiteDifficultyTB.Value)
			{
				case 0:
					this.whiteDifficultyLBL.Text = "Look Ahead Depth: 0 - Beginner";
					break;
				case 1:
					this.whiteDifficultyLBL.Text = "Look Ahead Depth: 1 - Intermediate";
					break;
				case 2:
					this.whiteDifficultyLBL.Text = "Look Ahead Depth: 3 - Advanced";
					break;
			}
		}
	}
}
