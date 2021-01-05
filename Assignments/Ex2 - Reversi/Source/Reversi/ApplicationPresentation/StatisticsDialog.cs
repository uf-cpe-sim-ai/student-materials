using System.Windows.Forms;

namespace GameAI.GamePlaying
{
	/// <summary>
	/// Summary description for StatisticsDialog.
	/// </summary>
	public class StatisticsDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox overallGroupBox;
		private System.Windows.Forms.Label blackWinsTextLabel;
		private System.Windows.Forms.Label blackWinsLabel;
		private System.Windows.Forms.Label whiteWinsTextLabel;
		private System.Windows.Forms.Label whiteWinsLabel;
		private System.Windows.Forms.Label overallDrawTextLabel;
		private System.Windows.Forms.Label overallDrawsLabel;
		private System.Windows.Forms.Label overallTotalScoreTextLabel;
		private System.Windows.Forms.Label blackTotalScoreTextLabel;
		private System.Windows.Forms.Label blackTotalScoreLabel;
		private System.Windows.Forms.Label whiteTotalScoreTextLabel;
		private System.Windows.Forms.Label whiteTotalScoreLabel;
		private System.Windows.Forms.GroupBox vsComputerGroupBox;
		private System.Windows.Forms.Label computerTextLabel;
		private System.Windows.Forms.Label computerWinsLabel;
		private System.Windows.Forms.Label userTextLabel;
		private System.Windows.Forms.Label userWinsLabel;
		private System.Windows.Forms.Label vsComputerDrawsTextLabel;
		private System.Windows.Forms.Label vsComputerDrawsLabel;
		private System.Windows.Forms.Label vsComputerTotalScoreTextLabel;
		private System.Windows.Forms.Label computerTotalScoreTextLabel;
		private System.Windows.Forms.Label computerTotalScoreLabel;
		private System.Windows.Forms.Label userTotalScoreTextLabel;
		private System.Windows.Forms.Label userTotalScoreLabel;
		private System.Windows.Forms.Button resetButton;
		private System.Windows.Forms.Button closeButton;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private Statistics statistics;

		public StatisticsDialog(Statistics statistics)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// Create a copy of the given game statistics.
			this.statistics = statistics;

			// Set the form controls based on those statistics.
			this.MapStatisticsToControls();

			// Activate the "Close" button.
			this.closeButton.Select();
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
			this.overallGroupBox = new System.Windows.Forms.GroupBox();
			this.whiteTotalScoreLabel = new System.Windows.Forms.Label();
			this.whiteTotalScoreTextLabel = new System.Windows.Forms.Label();
			this.blackTotalScoreLabel = new System.Windows.Forms.Label();
			this.blackTotalScoreTextLabel = new System.Windows.Forms.Label();
			this.overallTotalScoreTextLabel = new System.Windows.Forms.Label();
			this.overallDrawsLabel = new System.Windows.Forms.Label();
			this.overallDrawTextLabel = new System.Windows.Forms.Label();
			this.whiteWinsLabel = new System.Windows.Forms.Label();
			this.whiteWinsTextLabel = new System.Windows.Forms.Label();
			this.blackWinsLabel = new System.Windows.Forms.Label();
			this.blackWinsTextLabel = new System.Windows.Forms.Label();
			this.vsComputerGroupBox = new System.Windows.Forms.GroupBox();
			this.userTotalScoreLabel = new System.Windows.Forms.Label();
			this.userTotalScoreTextLabel = new System.Windows.Forms.Label();
			this.computerTotalScoreLabel = new System.Windows.Forms.Label();
			this.computerTotalScoreTextLabel = new System.Windows.Forms.Label();
			this.vsComputerTotalScoreTextLabel = new System.Windows.Forms.Label();
			this.vsComputerDrawsLabel = new System.Windows.Forms.Label();
			this.vsComputerDrawsTextLabel = new System.Windows.Forms.Label();
			this.userWinsLabel = new System.Windows.Forms.Label();
			this.userTextLabel = new System.Windows.Forms.Label();
			this.computerWinsLabel = new System.Windows.Forms.Label();
			this.computerTextLabel = new System.Windows.Forms.Label();
			this.closeButton = new System.Windows.Forms.Button();
			this.resetButton = new System.Windows.Forms.Button();
			this.overallGroupBox.SuspendLayout();
			this.vsComputerGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// overallGroupBox
			// 
			this.overallGroupBox.Controls.Add(this.whiteTotalScoreLabel);
			this.overallGroupBox.Controls.Add(this.whiteTotalScoreTextLabel);
			this.overallGroupBox.Controls.Add(this.blackTotalScoreLabel);
			this.overallGroupBox.Controls.Add(this.blackTotalScoreTextLabel);
			this.overallGroupBox.Controls.Add(this.overallTotalScoreTextLabel);
			this.overallGroupBox.Controls.Add(this.overallDrawsLabel);
			this.overallGroupBox.Controls.Add(this.overallDrawTextLabel);
			this.overallGroupBox.Controls.Add(this.whiteWinsLabel);
			this.overallGroupBox.Controls.Add(this.whiteWinsTextLabel);
			this.overallGroupBox.Controls.Add(this.blackWinsLabel);
			this.overallGroupBox.Controls.Add(this.blackWinsTextLabel);
			this.overallGroupBox.Location = new System.Drawing.Point(9, 8);
			this.overallGroupBox.Name = "overallGroupBox";
			this.overallGroupBox.Size = new System.Drawing.Size(271, 120);
			this.overallGroupBox.TabIndex = 0;
			this.overallGroupBox.TabStop = false;
			this.overallGroupBox.Text = "Overall";
			// 
			// whiteTotalScoreLabel
			// 
			this.whiteTotalScoreLabel.Location = new System.Drawing.Point(216, 68);
			this.whiteTotalScoreLabel.Name = "whiteTotalScoreLabel";
			this.whiteTotalScoreLabel.Size = new System.Drawing.Size(40, 13);
			this.whiteTotalScoreLabel.TabIndex = 10;
			this.whiteTotalScoreLabel.Text = "0";
			this.whiteTotalScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// whiteTotalScoreTextLabel
			// 
			this.whiteTotalScoreTextLabel.AutoSize = true;
			this.whiteTotalScoreTextLabel.Location = new System.Drawing.Point(170, 68);
			this.whiteTotalScoreTextLabel.Name = "whiteTotalScoreTextLabel";
			this.whiteTotalScoreTextLabel.Size = new System.Drawing.Size(36, 16);
			this.whiteTotalScoreTextLabel.TabIndex = 9;
			this.whiteTotalScoreTextLabel.Text = "White:";
			// 
			// blackTotalScoreLabel
			// 
			this.blackTotalScoreLabel.Location = new System.Drawing.Point(216, 42);
			this.blackTotalScoreLabel.Name = "blackTotalScoreLabel";
			this.blackTotalScoreLabel.Size = new System.Drawing.Size(40, 13);
			this.blackTotalScoreLabel.TabIndex = 8;
			this.blackTotalScoreLabel.Text = "0";
			this.blackTotalScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// blackTotalScoreTextLabel
			// 
			this.blackTotalScoreTextLabel.AutoSize = true;
			this.blackTotalScoreTextLabel.Location = new System.Drawing.Point(171, 42);
			this.blackTotalScoreTextLabel.Name = "blackTotalScoreTextLabel";
			this.blackTotalScoreTextLabel.Size = new System.Drawing.Size(35, 16);
			this.blackTotalScoreTextLabel.TabIndex = 7;
			this.blackTotalScoreTextLabel.Text = "Black:";
			// 
			// overallTotalScoreTextLabel
			// 
			this.overallTotalScoreTextLabel.AutoSize = true;
			this.overallTotalScoreTextLabel.Location = new System.Drawing.Point(141, 16);
			this.overallTotalScoreTextLabel.Name = "overallTotalScoreTextLabel";
			this.overallTotalScoreTextLabel.Size = new System.Drawing.Size(65, 16);
			this.overallTotalScoreTextLabel.TabIndex = 6;
			this.overallTotalScoreTextLabel.Text = "Total Score:";
			// 
			// overallDrawsLabel
			// 
			this.overallDrawsLabel.Location = new System.Drawing.Point(104, 94);
			this.overallDrawsLabel.Name = "overallDrawsLabel";
			this.overallDrawsLabel.Size = new System.Drawing.Size(24, 13);
			this.overallDrawsLabel.TabIndex = 5;
			this.overallDrawsLabel.Text = "0";
			this.overallDrawsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// overallDrawTextLabel
			// 
			this.overallDrawTextLabel.AutoSize = true;
			this.overallDrawTextLabel.Location = new System.Drawing.Point(52, 94);
			this.overallDrawTextLabel.Name = "overallDrawTextLabel";
			this.overallDrawTextLabel.Size = new System.Drawing.Size(39, 16);
			this.overallDrawTextLabel.TabIndex = 4;
			this.overallDrawTextLabel.Text = "Draws:";
			// 
			// whiteWinsLabel
			// 
			this.whiteWinsLabel.Location = new System.Drawing.Point(104, 68);
			this.whiteWinsLabel.Name = "whiteWinsLabel";
			this.whiteWinsLabel.Size = new System.Drawing.Size(24, 13);
			this.whiteWinsLabel.TabIndex = 3;
			this.whiteWinsLabel.Text = "0";
			this.whiteWinsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// whiteWinsTextLabel
			// 
			this.whiteWinsTextLabel.AutoSize = true;
			this.whiteWinsTextLabel.Location = new System.Drawing.Point(29, 68);
			this.whiteWinsTextLabel.Name = "whiteWinsTextLabel";
			this.whiteWinsTextLabel.Size = new System.Drawing.Size(62, 16);
			this.whiteWinsTextLabel.TabIndex = 2;
			this.whiteWinsTextLabel.Text = "White wins:";
			// 
			// blackWinsLabel
			// 
			this.blackWinsLabel.Location = new System.Drawing.Point(104, 42);
			this.blackWinsLabel.Name = "blackWinsLabel";
			this.blackWinsLabel.Size = new System.Drawing.Size(24, 13);
			this.blackWinsLabel.TabIndex = 1;
			this.blackWinsLabel.Text = "0";
			this.blackWinsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// blackWinsTextLabel
			// 
			this.blackWinsTextLabel.AutoSize = true;
			this.blackWinsTextLabel.Location = new System.Drawing.Point(30, 42);
			this.blackWinsTextLabel.Name = "blackWinsTextLabel";
			this.blackWinsTextLabel.Size = new System.Drawing.Size(61, 16);
			this.blackWinsTextLabel.TabIndex = 0;
			this.blackWinsTextLabel.Text = "Black wins:";
			// 
			// vsComputerGroupBox
			// 
			this.vsComputerGroupBox.Controls.Add(this.userTotalScoreLabel);
			this.vsComputerGroupBox.Controls.Add(this.userTotalScoreTextLabel);
			this.vsComputerGroupBox.Controls.Add(this.computerTotalScoreLabel);
			this.vsComputerGroupBox.Controls.Add(this.computerTotalScoreTextLabel);
			this.vsComputerGroupBox.Controls.Add(this.vsComputerTotalScoreTextLabel);
			this.vsComputerGroupBox.Controls.Add(this.vsComputerDrawsLabel);
			this.vsComputerGroupBox.Controls.Add(this.vsComputerDrawsTextLabel);
			this.vsComputerGroupBox.Controls.Add(this.userWinsLabel);
			this.vsComputerGroupBox.Controls.Add(this.userTextLabel);
			this.vsComputerGroupBox.Controls.Add(this.computerWinsLabel);
			this.vsComputerGroupBox.Controls.Add(this.computerTextLabel);
			this.vsComputerGroupBox.Location = new System.Drawing.Point(9, 136);
			this.vsComputerGroupBox.Name = "vsComputerGroupBox";
			this.vsComputerGroupBox.Size = new System.Drawing.Size(271, 120);
			this.vsComputerGroupBox.TabIndex = 1;
			this.vsComputerGroupBox.TabStop = false;
			this.vsComputerGroupBox.Text = "vs. Computer";
			// 
			// userTotalScoreLabel
			// 
			this.userTotalScoreLabel.Location = new System.Drawing.Point(216, 64);
			this.userTotalScoreLabel.Name = "userTotalScoreLabel";
			this.userTotalScoreLabel.Size = new System.Drawing.Size(40, 13);
			this.userTotalScoreLabel.TabIndex = 10;
			this.userTotalScoreLabel.Text = "0";
			this.userTotalScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// userTotalScoreTextLabel
			// 
			this.userTotalScoreTextLabel.AutoSize = true;
			this.userTotalScoreTextLabel.Location = new System.Drawing.Point(175, 64);
			this.userTotalScoreTextLabel.Name = "userTotalScoreTextLabel";
			this.userTotalScoreTextLabel.Size = new System.Drawing.Size(31, 16);
			this.userTotalScoreTextLabel.TabIndex = 9;
			this.userTotalScoreTextLabel.Text = "User:";
			// 
			// computerTotalScoreLabel
			// 
			this.computerTotalScoreLabel.Location = new System.Drawing.Point(216, 40);
			this.computerTotalScoreLabel.Name = "computerTotalScoreLabel";
			this.computerTotalScoreLabel.Size = new System.Drawing.Size(40, 13);
			this.computerTotalScoreLabel.TabIndex = 8;
			this.computerTotalScoreLabel.Text = "0";
			this.computerTotalScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// computerTotalScoreTextLabel
			// 
			this.computerTotalScoreTextLabel.AutoSize = true;
			this.computerTotalScoreTextLabel.Location = new System.Drawing.Point(149, 40);
			this.computerTotalScoreTextLabel.Name = "computerTotalScoreTextLabel";
			this.computerTotalScoreTextLabel.Size = new System.Drawing.Size(57, 16);
			this.computerTotalScoreTextLabel.TabIndex = 7;
			this.computerTotalScoreTextLabel.Text = "Computer:";
			// 
			// vsComputerTotalScoreTextLabel
			// 
			this.vsComputerTotalScoreTextLabel.AutoSize = true;
			this.vsComputerTotalScoreTextLabel.Location = new System.Drawing.Point(141, 16);
			this.vsComputerTotalScoreTextLabel.Name = "vsComputerTotalScoreTextLabel";
			this.vsComputerTotalScoreTextLabel.Size = new System.Drawing.Size(65, 16);
			this.vsComputerTotalScoreTextLabel.TabIndex = 6;
			this.vsComputerTotalScoreTextLabel.Text = "Total Score:";
			// 
			// vsComputerDrawsLabel
			// 
			this.vsComputerDrawsLabel.Location = new System.Drawing.Point(104, 88);
			this.vsComputerDrawsLabel.Name = "vsComputerDrawsLabel";
			this.vsComputerDrawsLabel.Size = new System.Drawing.Size(24, 13);
			this.vsComputerDrawsLabel.TabIndex = 5;
			this.vsComputerDrawsLabel.Text = "0";
			this.vsComputerDrawsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// vsComputerDrawsTextLabel
			// 
			this.vsComputerDrawsTextLabel.AutoSize = true;
			this.vsComputerDrawsTextLabel.Location = new System.Drawing.Point(52, 88);
			this.vsComputerDrawsTextLabel.Name = "vsComputerDrawsTextLabel";
			this.vsComputerDrawsTextLabel.Size = new System.Drawing.Size(39, 16);
			this.vsComputerDrawsTextLabel.TabIndex = 4;
			this.vsComputerDrawsTextLabel.Text = "Draws:";
			// 
			// userWinsLabel
			// 
			this.userWinsLabel.Location = new System.Drawing.Point(104, 64);
			this.userWinsLabel.Name = "userWinsLabel";
			this.userWinsLabel.Size = new System.Drawing.Size(24, 13);
			this.userWinsLabel.TabIndex = 3;
			this.userWinsLabel.Text = "0";
			this.userWinsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// userTextLabel
			// 
			this.userTextLabel.AutoSize = true;
			this.userTextLabel.Location = new System.Drawing.Point(34, 64);
			this.userTextLabel.Name = "userTextLabel";
			this.userTextLabel.Size = new System.Drawing.Size(57, 16);
			this.userTextLabel.TabIndex = 2;
			this.userTextLabel.Text = "User wins:";
			// 
			// computerWinsLabel
			// 
			this.computerWinsLabel.Location = new System.Drawing.Point(104, 40);
			this.computerWinsLabel.Name = "computerWinsLabel";
			this.computerWinsLabel.Size = new System.Drawing.Size(24, 13);
			this.computerWinsLabel.TabIndex = 1;
			this.computerWinsLabel.Text = "0";
			this.computerWinsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// computerTextLabel
			// 
			this.computerTextLabel.AutoSize = true;
			this.computerTextLabel.Location = new System.Drawing.Point(8, 40);
			this.computerTextLabel.Name = "computerTextLabel";
			this.computerTextLabel.Size = new System.Drawing.Size(83, 16);
			this.computerTextLabel.TabIndex = 0;
			this.computerTextLabel.Text = "Computer wins:";
			// 
			// closeButton
			// 
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.closeButton.Location = new System.Drawing.Point(205, 264);
			this.closeButton.Name = "closeButton";
			this.closeButton.TabIndex = 3;
			this.closeButton.Text = "Close";
			// 
			// resetButton
			// 
			this.resetButton.Location = new System.Drawing.Point(120, 264);
			this.resetButton.Name = "resetButton";
			this.resetButton.TabIndex = 2;
			this.resetButton.Text = "Reset";
			this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
			// 
			// StatisticsDialog
			// 
			this.AcceptButton = this.closeButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.closeButton;
			this.ClientSize = new System.Drawing.Size(290, 295);
			this.ControlBox = false;
			this.Controls.Add(this.resetButton);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.vsComputerGroupBox);
			this.Controls.Add(this.overallGroupBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StatisticsDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Statistics";
			this.overallGroupBox.ResumeLayout(false);
			this.vsComputerGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		//
		// Sets the form controls based on the current game statistics.
		//
		public void MapStatisticsToControls()
		{
			// Get and display the game statistics.
			this.blackWinsLabel.Text = this.statistics.BlackWins.ToString();
			this.whiteWinsLabel.Text = this.statistics.WhiteWins.ToString();
			this.overallDrawsLabel.Text = this.statistics.OverallDraws.ToString();
			this.blackTotalScoreLabel.Text = this.statistics.BlackTotalScore.ToString();
			this.whiteTotalScoreLabel.Text = this.statistics.WhiteTotalScore.ToString();
			this.computerWinsLabel.Text = this.statistics.ComputerWins.ToString();
			this.userWinsLabel.Text = this.statistics.UserWins.ToString();
			this.vsComputerDrawsLabel.Text = this.statistics.VsComputerDraws.ToString();
			this.computerTotalScoreLabel.Text = this.statistics.ComputerTotalScore.ToString();
			this.userTotalScoreLabel.Text = this.statistics.UserTotalScore.ToString();

			// Redraw the display.
			this.Refresh();
		}

		// ===================================================================
		// Event handlers for the form buttons.
		// ===================================================================

		//
		// Resets the game statistics when the "Reset" button is clicked.
		//
		private void resetButton_Click(object sender, System.EventArgs e)
		{
			// Prompt for confirmation.
			ConfirmDialog dlg = new ConfirmDialog("Reset statistics?");
			if (dlg.ShowDialog(this) == DialogResult.Yes)
			{
				// Reset and display the statistics.
				this.statistics.Reset();
				this.MapStatisticsToControls();
			}

			dlg.Dispose();
		}
	}
}
