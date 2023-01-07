// comment this to turn off student output
#define ShowStudentOutput


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using GameAI.GamePlaying.Core;
using GameAI.GamePlaying.ExampleAI;

namespace GameAI.GamePlaying
{
	/// <summary>
	/// Summary description for Reversi.
	/// </summary>
	public class ReversiForm : System.Windows.Forms.Form
	{
		// Main menu.
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem gameMenuItem;
		private System.Windows.Forms.MenuItem newGameMenuItem;
		private System.Windows.Forms.MenuItem resignGameMenuItem;
		private System.Windows.Forms.MenuItem gameSeparator1MenuItem;
		private System.Windows.Forms.MenuItem optionsMenuItem;
		private System.Windows.Forms.MenuItem statisticsMenuItem;
		private System.Windows.Forms.MenuItem gameSeparator2MenuItem;
		private System.Windows.Forms.MenuItem exitMenuItem;
		private System.Windows.Forms.MenuItem moveMenuItem;
		private System.Windows.Forms.MenuItem undoMoveMenuItem;
		private System.Windows.Forms.MenuItem undoAllMovesMenuItem;
		private System.Windows.Forms.MenuItem redoMoveMenuItem;
		private System.Windows.Forms.MenuItem redoAllMovesMenuItem;
		private System.Windows.Forms.MenuItem moveSeparatorMenuItem;
		private System.Windows.Forms.MenuItem resumePlayMenuItem;
		private System.Windows.Forms.MenuItem helpMenuItem;
		private System.Windows.Forms.MenuItem helpTopicsMenuItem;
		private System.Windows.Forms.MenuItem helpSeparatorMenuItem;
		private System.Windows.Forms.MenuItem aboutMenuItem;

		// Tool bar.
		private System.Windows.Forms.ToolBar playToolBar;
		private System.Windows.Forms.ImageList playImageList;
		private System.Windows.Forms.ToolBarButton newGameToolBarButton;
		private System.Windows.Forms.ToolBarButton resignGameToolBarButton;
		private System.Windows.Forms.ToolBarButton separatorToolBarButton;
		private System.Windows.Forms.ToolBarButton undoAllMovesToolBarButton;
		private System.Windows.Forms.ToolBarButton undoMoveToolBarButton;
		private System.Windows.Forms.ToolBarButton resumePlayToolBarButton;
		private System.Windows.Forms.ToolBarButton redoMoveToolBarButton;
		private System.Windows.Forms.ToolBarButton redoAllMovesToolBarButton;

		// Board display.
		private System.Windows.Forms.Panel boardPanel;
		private System.Windows.Forms.Label cornerLabel;
		private System.Windows.Forms.Panel squaresPanel;

		// Information display.
		private System.Windows.Forms.Panel infoPanel;
		private System.Windows.Forms.Label whiteTextLabel;
		private System.Windows.Forms.Label whiteCountLabel;
		private System.Windows.Forms.Label blackTextLabel;
		private System.Windows.Forms.Label blackCountLabel;
		private System.Windows.Forms.Label currentColorTextLabel;
		private System.Windows.Forms.Panel currentColorPanel;
		private System.Windows.Forms.ListView moveListView;
		private System.Windows.Forms.ColumnHeader moveNullColumn;
		private System.Windows.Forms.ColumnHeader moveNumberColumn;
		private System.Windows.Forms.ColumnHeader movePlayerColumn;
		private System.Windows.Forms.ColumnHeader movePositionColumn;

		// Status display.
		private System.Windows.Forms.Panel statusPanel;
		private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ProgressBar statusProgressBar;
        private IContainer components;

		// This enumeration should match the button order on the tool bar.
		public enum ToolBarButton
		{
			NewGame,
			ResignGame,
			Separator,
			UndoAllMoves,
			UndoMove,
			ResumePlay,
			RedoMove,
			RedoAllMoves
		}

		// Defines the game states.
		private enum GameState
		{
			GameOver,			// The game is over (also used for the initial state).
			InMoveAnimation,	// A move has been made and the animation is active.
			InPlayerMove,		// Waiting for the user to make a move.
			InComputerMove,		// Waiting for the computer to make a move.
			MoveCompleted		// A move has been completed (including the animation, if active).
		}

		// The game board.
		private Board board;
		private Label[] colLabels, rowLabels;
		private SquareControl[,] squareControls;

		// Game options.
		private Options options = new Options();

		// Game statistics.
		private Statistics statistics = new Statistics();

		// Game parameters.
		private GameState gameState;
		private int       currentColor;
		private int       moveNumber;

		// This timer is used to animate moves during game play.
		private System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
		private static readonly int animationTimerInterval = 50;

		// AI agent instance.
		private Agent[] agents;

		// Defines a thread for running the computer move look ahead.
		private Thread calculateComputerMoveThread;

		// For converting column numbers to letters and vice versa.
		private static String alpha = "ABCDEFGH";

		// For getting moves via the keyboard.
		private int keyedColNumber;
		private int keyedRowNumber;

		// Defines a structure for holding move history data.
		private struct MoveRecord
		{
			public Board        board;
			public int          currentColor;
			public ListViewItem moveListItem;

			public MoveRecord(Board board, int currentColor, ListViewItem moveListItem)
			{
				this.board        = new Board(board);
				this.currentColor = currentColor;
				this.moveListItem = moveListItem;
			}
		}

		// Defines an array for storing the move history.
		private ArrayList moveHistory;

		// Used to track which player made the last move.
		private int lastMoveColor;

		// Used to suspend computer play while moves are undone/redone.
		private bool isComputerPlaySuspended;

		// Used to save statistics when undoing moves in a completed game.
		private Statistics oldStatistics;

		// For tracking the window location and size.
		private Rectangle windowSettings;

		// For loading and saving program settings.
		private ProgramSettings settings;
		private static readonly string programSettingsFileName = "Reversi.xml";

		// The help file name.
		private static readonly string helpFileName = "Reversi.chm";

        // Example AI Behavior
        private MinimaxExample exampleAIBehavior = new MinimaxExample();

        // Student Behavior
        private StudentAI studentBehavior = new StudentAI();

		public ReversiForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// Create the game board.
			this.board = new Board();

			// Create the AI agent.
			this.agents = new Agent[2];
			this.agents[0] = new Agent();
            if (options.BlackUsesExampleAI)
                this.agents[0].behavior = exampleAIBehavior;
            else
                this.agents[0].behavior = studentBehavior;
			this.agents[1] = new Agent();
            if (options.WhiteUsesExampleAI)
                this.agents[1].behavior = exampleAIBehavior;
            else
                this.agents[1].behavior = studentBehavior;

			// Create the controls for each square, add them to the squares
			// panel and set up event handling.
			this.squareControls = new SquareControl[8, 8];
			int i, j;
			for (i = 0; i < 8; i++)
				for (j = 0; j < 8; j++)
				{
					// Create it.
					this.squareControls[i, j] = new SquareControl(i, j);
					// Position it.
					this.squareControls[i, j].Left = j * this.squareControls[i, j].Width;
					this.squareControls[i, j].Top  = i * this.squareControls[i, j].Height;
					// Add it.
					this.squaresPanel.Controls.Add(this.squareControls[i, j]);
					// Set up event handling for it.
					this.squareControls[i, j].MouseMove  += new MouseEventHandler(this.SquareControl_MouseMove);
					this.squareControls[i, j].MouseLeave += new EventHandler(this.SquareControl_MouseLeave);
					this.squareControls[i, j].Click      += new EventHandler(this.SquareControl_Click);
				}

			// Create the column and row labels.
			this.colLabels = new Label[8];
			for (i = 0; i < 8; i++)
			{
				// Create a column label.
				this.colLabels[i] = new Label();

				// Set its display properties.
				this.colLabels[i].Text = ReversiForm.alpha.Substring(i, 1);
				this.colLabels[i].BackColor = this.cornerLabel.BackColor;
				this.colLabels[i].ForeColor = this.cornerLabel.ForeColor;
				this.colLabels[i].TextAlign = ContentAlignment.MiddleCenter;

				// Set its size and position.
				this.colLabels[i].Width = this.squareControls[0, 0].Width;
				this.colLabels[i].Height = this.cornerLabel.Height;
				this.colLabels[i].Left = this.cornerLabel.Width + i * this.colLabels[0].Width;
				this.colLabels[i].Top = 0;

				// Add it.
				this.boardPanel.Controls.Add(this.colLabels[i]);
			}
			this.rowLabels = new Label[8];
			for (i = 0; i < 8; i++)
			{
				// Create a row label.
				this.rowLabels[i] = new Label();

				// Set its display properties.
				this.rowLabels[i].Text      = (i + 1).ToString();
				this.rowLabels[i].BackColor = this.cornerLabel.BackColor;
				this.rowLabels[i].ForeColor = this.cornerLabel.ForeColor;
				this.rowLabels[i].TextAlign = ContentAlignment.MiddleCenter;

				// Set its size and position.
				this.rowLabels[i].Width  = this.cornerLabel.Height;
				this.rowLabels[i].Height = this.squareControls[0, 0].Height;
				this.rowLabels[i].Left   = 0;
				this.rowLabels[i].Top    = this.cornerLabel.Height + i * this.rowLabels[0].Width;

				// Add it.
				this.boardPanel.Controls.Add(this.rowLabels[i]);
			}

			// Initialize the game state.
			this.gameState = ReversiForm.GameState.GameOver;

			// Initialize the animation timer.
			this.animationTimer.Interval = ReversiForm.animationTimerInterval;
			this.animationTimer.Tick += new EventHandler(this.AnimateMove);

			// Initialize the window settings.
			this.windowSettings = new Rectangle(
				this.DesktopLocation.X,
				this.DesktopLocation.Y,
				this.ClientSize.Width,
				this.ClientSize.Height);

			// Load any saved program settings.
			this.settings = new ProgramSettings(ReversiForm.programSettingsFileName);
			this.LoadProgramSettings();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReversiForm));
            this.infoPanel = new System.Windows.Forms.Panel();
            this.currentColorPanel = new System.Windows.Forms.Panel();
            this.whiteCountLabel = new System.Windows.Forms.Label();
            this.whiteTextLabel = new System.Windows.Forms.Label();
            this.currentColorTextLabel = new System.Windows.Forms.Label();
            this.blackCountLabel = new System.Windows.Forms.Label();
            this.blackTextLabel = new System.Windows.Forms.Label();
            this.moveListView = new System.Windows.Forms.ListView();
            this.moveNullColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.moveNumberColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.movePlayerColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.movePositionColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.squaresPanel = new System.Windows.Forms.Panel();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.gameMenuItem = new System.Windows.Forms.MenuItem();
            this.newGameMenuItem = new System.Windows.Forms.MenuItem();
            this.resignGameMenuItem = new System.Windows.Forms.MenuItem();
            this.gameSeparator1MenuItem = new System.Windows.Forms.MenuItem();
            this.optionsMenuItem = new System.Windows.Forms.MenuItem();
            this.statisticsMenuItem = new System.Windows.Forms.MenuItem();
            this.gameSeparator2MenuItem = new System.Windows.Forms.MenuItem();
            this.exitMenuItem = new System.Windows.Forms.MenuItem();
            this.moveMenuItem = new System.Windows.Forms.MenuItem();
            this.undoMoveMenuItem = new System.Windows.Forms.MenuItem();
            this.redoMoveMenuItem = new System.Windows.Forms.MenuItem();
            this.undoAllMovesMenuItem = new System.Windows.Forms.MenuItem();
            this.redoAllMovesMenuItem = new System.Windows.Forms.MenuItem();
            this.moveSeparatorMenuItem = new System.Windows.Forms.MenuItem();
            this.resumePlayMenuItem = new System.Windows.Forms.MenuItem();
            this.helpMenuItem = new System.Windows.Forms.MenuItem();
            this.helpTopicsMenuItem = new System.Windows.Forms.MenuItem();
            this.helpSeparatorMenuItem = new System.Windows.Forms.MenuItem();
            this.aboutMenuItem = new System.Windows.Forms.MenuItem();
            this.boardPanel = new System.Windows.Forms.Panel();
            this.cornerLabel = new System.Windows.Forms.Label();
            this.statusProgressBar = new System.Windows.Forms.ProgressBar();
            this.statusLabel = new System.Windows.Forms.Label();
            this.statusPanel = new System.Windows.Forms.Panel();
            this.playToolBar = new System.Windows.Forms.ToolBar();
            this.newGameToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.resignGameToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.separatorToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.undoAllMovesToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.undoMoveToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.resumePlayToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.redoMoveToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.redoAllMovesToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.playImageList = new System.Windows.Forms.ImageList(this.components);
            this.infoPanel.SuspendLayout();
            this.boardPanel.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // infoPanel
            // 
            this.infoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoPanel.Controls.Add(this.currentColorPanel);
            this.infoPanel.Controls.Add(this.whiteCountLabel);
            this.infoPanel.Controls.Add(this.whiteTextLabel);
            this.infoPanel.Controls.Add(this.currentColorTextLabel);
            this.infoPanel.Controls.Add(this.blackCountLabel);
            this.infoPanel.Controls.Add(this.blackTextLabel);
            this.infoPanel.Controls.Add(this.moveListView);
            this.infoPanel.Location = new System.Drawing.Point(292, 32);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(169, 276);
            this.infoPanel.TabIndex = 3;
            // 
            // currentColorPanel
            // 
            this.currentColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.currentColorPanel.Location = new System.Drawing.Point(88, 56);
            this.currentColorPanel.Name = "currentColorPanel";
            this.currentColorPanel.Size = new System.Drawing.Size(16, 16);
            this.currentColorPanel.TabIndex = 5;
            this.currentColorPanel.Visible = false;
            // 
            // whiteCountLabel
            // 
            this.whiteCountLabel.Location = new System.Drawing.Point(80, 32);
            this.whiteCountLabel.Name = "whiteCountLabel";
            this.whiteCountLabel.Size = new System.Drawing.Size(24, 13);
            this.whiteCountLabel.TabIndex = 3;
            this.whiteCountLabel.Text = "0";
            this.whiteCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // whiteTextLabel
            // 
            this.whiteTextLabel.AutoSize = true;
            this.whiteTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.whiteTextLabel.Location = new System.Drawing.Point(40, 32);
            this.whiteTextLabel.Name = "whiteTextLabel";
            this.whiteTextLabel.Size = new System.Drawing.Size(41, 13);
            this.whiteTextLabel.TabIndex = 2;
            this.whiteTextLabel.Text = "White: ";
            // 
            // currentColorTextLabel
            // 
            this.currentColorTextLabel.AutoSize = true;
            this.currentColorTextLabel.Location = new System.Drawing.Point(32, 56);
            this.currentColorTextLabel.Name = "currentColorTextLabel";
            this.currentColorTextLabel.Size = new System.Drawing.Size(47, 13);
            this.currentColorTextLabel.TabIndex = 4;
            this.currentColorTextLabel.Text = "Current: ";
            this.currentColorTextLabel.Visible = false;
            // 
            // blackCountLabel
            // 
            this.blackCountLabel.Location = new System.Drawing.Point(80, 8);
            this.blackCountLabel.Name = "blackCountLabel";
            this.blackCountLabel.Size = new System.Drawing.Size(24, 13);
            this.blackCountLabel.TabIndex = 1;
            this.blackCountLabel.Text = "0";
            this.blackCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // blackTextLabel
            // 
            this.blackTextLabel.AutoSize = true;
            this.blackTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blackTextLabel.Location = new System.Drawing.Point(40, 8);
            this.blackTextLabel.Name = "blackTextLabel";
            this.blackTextLabel.Size = new System.Drawing.Size(40, 13);
            this.blackTextLabel.TabIndex = 0;
            this.blackTextLabel.Text = "Black: ";
            // 
            // moveListView
            // 
            this.moveListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.moveListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.moveListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.moveNullColumn,
            this.moveNumberColumn,
            this.movePlayerColumn,
            this.movePositionColumn});
            this.moveListView.FullRowSelect = true;
            this.moveListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.moveListView.HideSelection = false;
            this.moveListView.Location = new System.Drawing.Point(2, 88);
            this.moveListView.Name = "moveListView";
            this.moveListView.Size = new System.Drawing.Size(164, 198);
            this.moveListView.TabIndex = 6;
            this.moveListView.TabStop = false;
            this.moveListView.UseCompatibleStateImageBehavior = false;
            this.moveListView.View = System.Windows.Forms.View.Details;
            // 
            // moveNullColumn
            // 
            this.moveNullColumn.Text = "";
            this.moveNullColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.moveNullColumn.Width = 0;
            // 
            // moveNumberColumn
            // 
            this.moveNumberColumn.Text = "#";
            this.moveNumberColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.moveNumberColumn.Width = 32;
            // 
            // movePlayerColumn
            // 
            this.movePlayerColumn.Text = "Player";
            this.movePlayerColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.movePlayerColumn.Width = 52;
            // 
            // movePositionColumn
            // 
            this.movePositionColumn.Text = "Position";
            this.movePositionColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.movePositionColumn.Width = 62;
            // 
            // squaresPanel
            // 
            this.squaresPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.squaresPanel.Location = new System.Drawing.Point(16, 16);
            this.squaresPanel.Name = "squaresPanel";
            this.squaresPanel.Size = new System.Drawing.Size(253, 256);
            this.squaresPanel.TabIndex = 1;
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.gameMenuItem,
            this.moveMenuItem,
            this.helpMenuItem});
            // 
            // gameMenuItem
            // 
            this.gameMenuItem.Index = 0;
            this.gameMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.newGameMenuItem,
            this.resignGameMenuItem,
            this.gameSeparator1MenuItem,
            this.optionsMenuItem,
            this.statisticsMenuItem,
            this.gameSeparator2MenuItem,
            this.exitMenuItem});
            this.gameMenuItem.ShowShortcut = false;
            this.gameMenuItem.Text = "&Game";
            // 
            // newGameMenuItem
            // 
            this.newGameMenuItem.Index = 0;
            this.newGameMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            this.newGameMenuItem.Text = "&New Game";
            this.newGameMenuItem.Click += new System.EventHandler(this.newGameMenuItem_Click);
            // 
            // resignGameMenuItem
            // 
            this.resignGameMenuItem.Enabled = false;
            this.resignGameMenuItem.Index = 1;
            this.resignGameMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
            this.resignGameMenuItem.Text = "&Resign Game";
            this.resignGameMenuItem.Click += new System.EventHandler(this.resignGameMenuItem_Click);
            // 
            // gameSeparator1MenuItem
            // 
            this.gameSeparator1MenuItem.Index = 2;
            this.gameSeparator1MenuItem.Text = "-";
            // 
            // optionsMenuItem
            // 
            this.optionsMenuItem.Index = 3;
            this.optionsMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.optionsMenuItem.Text = "&Options...";
            this.optionsMenuItem.Click += new System.EventHandler(this.optionsMenuItem_Click);
            // 
            // statisticsMenuItem
            // 
            this.statisticsMenuItem.Index = 4;
            this.statisticsMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.statisticsMenuItem.Text = "&Statistics...";
            this.statisticsMenuItem.Click += new System.EventHandler(this.statisticsMenuItem_Click);
            // 
            // gameSeparator2MenuItem
            // 
            this.gameSeparator2MenuItem.Index = 5;
            this.gameSeparator2MenuItem.Text = "-";
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Index = 6;
            this.exitMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // moveMenuItem
            // 
            this.moveMenuItem.Index = 1;
            this.moveMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.undoMoveMenuItem,
            this.redoMoveMenuItem,
            this.undoAllMovesMenuItem,
            this.redoAllMovesMenuItem,
            this.moveSeparatorMenuItem,
            this.resumePlayMenuItem});
            this.moveMenuItem.ShowShortcut = false;
            this.moveMenuItem.Text = "&Move";
            // 
            // undoMoveMenuItem
            // 
            this.undoMoveMenuItem.Enabled = false;
            this.undoMoveMenuItem.Index = 0;
            this.undoMoveMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
            this.undoMoveMenuItem.Text = "&Undo Move";
            this.undoMoveMenuItem.Click += new System.EventHandler(this.undoMoveMenuItem_Click);
            // 
            // redoMoveMenuItem
            // 
            this.redoMoveMenuItem.Enabled = false;
            this.redoMoveMenuItem.Index = 1;
            this.redoMoveMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
            this.redoMoveMenuItem.Text = "&Redo Move";
            this.redoMoveMenuItem.Click += new System.EventHandler(this.redoMoveMenuItem_Click);
            // 
            // undoAllMovesMenuItem
            // 
            this.undoAllMovesMenuItem.Enabled = false;
            this.undoAllMovesMenuItem.Index = 2;
            this.undoAllMovesMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftZ;
            this.undoAllMovesMenuItem.Text = "U&ndo All Moves";
            this.undoAllMovesMenuItem.Click += new System.EventHandler(this.undoAllMovesmenuItem_Click);
            // 
            // redoAllMovesMenuItem
            // 
            this.redoAllMovesMenuItem.Enabled = false;
            this.redoAllMovesMenuItem.Index = 3;
            this.redoAllMovesMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftY;
            this.redoAllMovesMenuItem.Text = "Re&do All Moves";
            this.redoAllMovesMenuItem.Click += new System.EventHandler(this.redoAllMovesMenuItem_Click);
            // 
            // moveSeparatorMenuItem
            // 
            this.moveSeparatorMenuItem.Index = 4;
            this.moveSeparatorMenuItem.Text = "-";
            // 
            // resumePlayMenuItem
            // 
            this.resumePlayMenuItem.Enabled = false;
            this.resumePlayMenuItem.Index = 5;
            this.resumePlayMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
            this.resumePlayMenuItem.Text = "Resume &Play";
            this.resumePlayMenuItem.Click += new System.EventHandler(this.resumePlayMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.Index = 2;
            this.helpMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.helpTopicsMenuItem,
            this.helpSeparatorMenuItem,
            this.aboutMenuItem});
            this.helpMenuItem.ShowShortcut = false;
            this.helpMenuItem.Text = "&Help";
            // 
            // helpTopicsMenuItem
            // 
            this.helpTopicsMenuItem.Index = 0;
            this.helpTopicsMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlH;
            this.helpTopicsMenuItem.Text = "&Help Topics";
            this.helpTopicsMenuItem.Click += new System.EventHandler(this.helpTopicsMenuItem_Click);
            // 
            // helpSeparatorMenuItem
            // 
            this.helpSeparatorMenuItem.Index = 1;
            this.helpSeparatorMenuItem.Text = "-";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Index = 2;
            this.aboutMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this.aboutMenuItem.Text = "&About";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // boardPanel
            // 
            this.boardPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boardPanel.BackColor = System.Drawing.SystemColors.Control;
            this.boardPanel.Controls.Add(this.cornerLabel);
            this.boardPanel.Controls.Add(this.squaresPanel);
            this.boardPanel.Location = new System.Drawing.Point(8, 32);
            this.boardPanel.Name = "boardPanel";
            this.boardPanel.Size = new System.Drawing.Size(269, 272);
            this.boardPanel.TabIndex = 2;
            // 
            // cornerLabel
            // 
            this.cornerLabel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.cornerLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.cornerLabel.Location = new System.Drawing.Point(0, 0);
            this.cornerLabel.Name = "cornerLabel";
            this.cornerLabel.Size = new System.Drawing.Size(16, 16);
            this.cornerLabel.TabIndex = 0;
            // 
            // statusProgressBar
            // 
            this.statusProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.statusProgressBar.BackColor = System.Drawing.SystemColors.ControlLight;
            this.statusProgressBar.Location = new System.Drawing.Point(347, 2);
            this.statusProgressBar.Name = "statusProgressBar";
            this.statusProgressBar.Size = new System.Drawing.Size(104, 16);
            this.statusProgressBar.Step = 1;
            this.statusProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.statusProgressBar.TabIndex = 1;
            this.statusProgressBar.Visible = false;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(16, 2);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 13);
            this.statusLabel.TabIndex = 0;
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusPanel
            // 
            this.statusPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.statusPanel.Controls.Add(this.statusProgressBar);
            this.statusPanel.Controls.Add(this.statusLabel);
            this.statusPanel.Location = new System.Drawing.Point(8, 312);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(453, 24);
            this.statusPanel.TabIndex = 4;
            // 
            // playToolBar
            // 
            this.playToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.newGameToolBarButton,
            this.resignGameToolBarButton,
            this.separatorToolBarButton,
            this.undoAllMovesToolBarButton,
            this.undoMoveToolBarButton,
            this.resumePlayToolBarButton,
            this.redoMoveToolBarButton,
            this.redoAllMovesToolBarButton});
            this.playToolBar.Divider = false;
            this.playToolBar.DropDownArrows = true;
            this.playToolBar.ImageList = this.playImageList;
            this.playToolBar.Location = new System.Drawing.Point(0, 0);
            this.playToolBar.Name = "playToolBar";
            this.playToolBar.ShowToolTips = true;
            this.playToolBar.Size = new System.Drawing.Size(469, 26);
            this.playToolBar.TabIndex = 1;
            this.playToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.playToolBar_ButtonClick);
            // 
            // newGameToolBarButton
            // 
            this.newGameToolBarButton.ImageIndex = 0;
            this.newGameToolBarButton.Name = "newGameToolBarButton";
            this.newGameToolBarButton.ToolTipText = "New Game";
            // 
            // resignGameToolBarButton
            // 
            this.resignGameToolBarButton.Enabled = false;
            this.resignGameToolBarButton.ImageIndex = 1;
            this.resignGameToolBarButton.Name = "resignGameToolBarButton";
            this.resignGameToolBarButton.ToolTipText = "Resign Game";
            // 
            // separatorToolBarButton
            // 
            this.separatorToolBarButton.Name = "separatorToolBarButton";
            this.separatorToolBarButton.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // undoAllMovesToolBarButton
            // 
            this.undoAllMovesToolBarButton.Enabled = false;
            this.undoAllMovesToolBarButton.ImageIndex = 2;
            this.undoAllMovesToolBarButton.Name = "undoAllMovesToolBarButton";
            this.undoAllMovesToolBarButton.ToolTipText = "Undo All Moves";
            // 
            // undoMoveToolBarButton
            // 
            this.undoMoveToolBarButton.Enabled = false;
            this.undoMoveToolBarButton.ImageIndex = 3;
            this.undoMoveToolBarButton.Name = "undoMoveToolBarButton";
            this.undoMoveToolBarButton.ToolTipText = "Undo Move";
            // 
            // resumePlayToolBarButton
            // 
            this.resumePlayToolBarButton.Enabled = false;
            this.resumePlayToolBarButton.ImageIndex = 4;
            this.resumePlayToolBarButton.Name = "resumePlayToolBarButton";
            this.resumePlayToolBarButton.ToolTipText = "Resume Play";
            // 
            // redoMoveToolBarButton
            // 
            this.redoMoveToolBarButton.Enabled = false;
            this.redoMoveToolBarButton.ImageIndex = 5;
            this.redoMoveToolBarButton.Name = "redoMoveToolBarButton";
            this.redoMoveToolBarButton.ToolTipText = "Redo Move";
            // 
            // redoAllMovesToolBarButton
            // 
            this.redoAllMovesToolBarButton.Enabled = false;
            this.redoAllMovesToolBarButton.ImageIndex = 6;
            this.redoAllMovesToolBarButton.Name = "redoAllMovesToolBarButton";
            this.redoAllMovesToolBarButton.ToolTipText = "Redo All Moves";
            // 
            // playImageList
            // 
            this.playImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("playImageList.ImageStream")));
            this.playImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.playImageList.Images.SetKeyName(0, "");
            this.playImageList.Images.SetKeyName(1, "");
            this.playImageList.Images.SetKeyName(2, "");
            this.playImageList.Images.SetKeyName(3, "");
            this.playImageList.Images.SetKeyName(4, "");
            this.playImageList.Images.SetKeyName(5, "");
            this.playImageList.Images.SetKeyName(6, "");
            // 
            // ReversiForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(469, 345);
            this.Controls.Add(this.playToolBar);
            this.Controls.Add(this.boardPanel);
            this.Controls.Add(this.infoPanel);
            this.Controls.Add(this.statusPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Menu = this.mainMenu;
            this.Name = "ReversiForm";
            this.Text = "Reversi";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.ReversiForm_Closing);
            this.Closed += new System.EventHandler(this.ReversiForm_Closed);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ReversiForm_KeyPress);
            this.Move += new System.EventHandler(this.ReversiForm_Move);
            this.Resize += new System.EventHandler(this.ReversiForm_Resize);
            this.infoPanel.ResumeLayout(false);
            this.infoPanel.PerformLayout();
            this.boardPanel.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
            this.statusPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// Use this function to setup which stream to use for the console window.
		/// </summary>
		/// <param name="stream">stream to use for the console window; use System.IO.Stream.Null to prevent output.</param>
		private static void SetupConsoleStream(System.IO.Stream stream)
		{
            //MinMaxBehavior.Console = new System.IO.StreamWriter(stream);
            //MinMaxBehavior.Console.AutoFlush = true;
            //if (stream != System.IO.Stream.Null)
            //    Console.SetOut(MinMaxBehavior.Console);
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
#if(ShowStudentOutput)
				SetupConsoleStream(Console.OpenStandardOutput());
#else
				SetupConsoleStream(System.IO.Stream.Null);
#endif
			Application.Run(new ReversiForm());
		}

		// ===================================================================
		// This code handles game play.
		// ===================================================================

		//
		// Starts a new game.
		//
		private void StartGame()
		{
			this.StartGame(false);
		}

		//
		// Starts a new game or, optionally, restarts an ended game.
		//
		private void StartGame(bool isRestart)
		{
			// Enable/disable the menu items and tool bar buttons as
			// appropriate.
			this.newGameMenuItem.Enabled    = this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.NewGame].Enabled    = false;
			this.resignGameMenuItem.Enabled = this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.ResignGame].Enabled = true;
			if (!isRestart)
			{
				this.undoMoveMenuItem.Enabled =
					this.undoAllMovesMenuItem.Enabled =
					this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.UndoMove].Enabled =
					this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.UndoAllMoves].Enabled = false;
				this.redoMoveMenuItem.Enabled =
					this.redoAllMovesMenuItem.Enabled =
					this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.RedoMove].Enabled =
					this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.RedoAllMoves].Enabled = false;
				this.resumePlayMenuItem.Enabled =
					this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.ResumePlay].Enabled   = false;
			}

			// Initialize the information display.
			this.currentColorTextLabel.Visible = true;
			this.currentColorPanel.Visible     = true;

			// Initialize a new game.
			if (!isRestart)
			{
				// Initialize the move list.
				this.moveNumber = 1;
				this.moveListView.Items.Clear();
				this.moveListView.Refresh();

				// Initialize the move history.
				this.moveHistory = new ArrayList(60);

				// Initialize the last move color.
				this.lastMoveColor = Board.Empty;

				// Clear the suspend computer play flag.
				this.isComputerPlaySuspended = false;

				// Initialize the board.
				this.board.SetForNewGame();
				this.UpdateBoardDisplay();

				// Initialize the status display.
				this.statusLabel.Text = "";
				this.statusPanel.Refresh();

				// Set the first player.
				this.currentColor = Board.Black;
			}

			// Start the first turn.
			this.StartTurn();
		}

		//
		// Ends the current game.
		//
		private void EndGame()
		{
			this.EndGame(false);
		}

		//
		// Ends the current game, optionally by player resignation.
		//
		private void EndGame(bool isResignation)
		{
			// Set the game state.
			this.gameState = ReversiForm.GameState.GameOver;

			// Stop the game timer.
			this.animationTimer.Stop();

			// Enable/disable the menu items and tool bar buttons as
			// appropriate.
			this.newGameMenuItem.Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.NewGame].Enabled      = true;
			this.resignGameMenuItem.Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.ResignGame].Enabled   = false;
			this.undoMoveMenuItem.Enabled =
				this.undoAllMovesMenuItem.Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.UndoMove].Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.UndoAllMoves].Enabled = false;
			this.redoMoveMenuItem.Enabled =
				this.redoAllMovesMenuItem.Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.RedoMove].Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.RedoAllMoves].Enabled = false;
			this.resumePlayMenuItem.Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.ResumePlay].Enabled   = false;

			// Clear the current player indicator display.
			this.currentColorTextLabel.Visible = false;
			this.currentColorPanel.BackColor   = Color.Empty;
			this.currentColorPanel.Visible     = false;

			// Hide the status progress bar.
			this.statusProgressBar.Visible = false;
			this.statusPanel.Refresh();

			// For a computer vs. user game, determine who played what color.
			int computerColor = Board.Empty;
			int userColor     = Board.Empty;
			if (this.IsComputerPlayer(Board.Black) && !this.IsComputerPlayer(Board.White))
			{
				computerColor = Board.Black;
				userColor = Board.White;
			}
			if (this.IsComputerPlayer(Board.White) && !this.IsComputerPlayer(Board.Black))
			{
				computerColor = Board.White;
				userColor = Board.Black;
			}

			// Save the current statistics, in case the game is restarted.
			this.oldStatistics = new Statistics(this.statistics);

			// Handle a resignation.
			if (isResignation)
			{
				// For computer vs. computer game, just update the status
				// message.
				if (this.IsComputerPlayer(Board.Black) && this.IsComputerPlayer(Board.White))
					this.statusLabel.Text = "Game aborted.";
				else
				{
					// Determine which player is resigning. In a computer vs.
					// user game, the computer will never resign so it must be
					// the user. In a user vs. user game we'll assume it is
					// the current player.
					int resigningColor = this.currentColor;
					if (this.IsComputerPlayer(Board.Black) || this.IsComputerPlayer(Board.White))
						resigningColor = userColor;

					// Update the status message and record the game as a 64-0
					// loss for the resigning player.
					if (resigningColor == Board.Black)
					{
						this.statusLabel.Text = "Black resigns.";
						this.statistics.Update(0, 64, computerColor, userColor);
					}
					else
					{
						this.statusLabel.Text = "White resigns.";
						this.statistics.Update(64, 0, computerColor, userColor);
					}
				}
			}

			// Handle an end game.
			else
			{
				// Update the status message.
				if (this.board.BlackCount > this.board.WhiteCount)
					this.statusLabel.Text = "Black wins.";
				else if (this.board.WhiteCount > this.board.BlackCount)
					this.statusLabel.Text = "White wins.";
				else
					this.statusLabel.Text = "Draw.";

				// Record the result.
				this.statistics.Update(this.board.BlackCount, this.board.WhiteCount, computerColor, userColor);
			}

			// Update the status display.
			this.statusPanel.Refresh();

			// Re-enable the undo move-related menu items and tool bar
			// buttons.
			this.undoMoveMenuItem.Enabled =
				this.undoAllMovesMenuItem.Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.UndoMove].Enabled =
				this.undoAllMovesMenuItem.Enabled = this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.UndoAllMoves].Enabled = true;
		}

		//
		// Sets up for the current player to make a move or ends the game if
		// neither player can make a valid move.
		//
		private void StartTurn()
		{
			// If the current player cannot make a valid move, forfeit the turn.
			if (!this.board.HasAnyValidMove(this.currentColor))
			{
				// Switch back to the other player.
				this.currentColor *= -1;

				// If the original player cannot make a valid move either, the game is over.
				if (!this.board.HasAnyValidMove(this.currentColor))
				{
					this.EndGame();
					return;
				}
			}

			// Set the player text for the status display.
			string playerText = String.Format("{0}'s", (this.currentColor == Board.Black ? "Black" : "White"));
			if ((this.options.ComputerPlaysBlack && !this.options.ComputerPlaysWhite) ||
				(this.options.ComputerPlaysWhite && !this.options.ComputerPlaysBlack))
				playerText = (this.IsComputerPlayer(this.currentColor) ? "My" : "Your");

			// Update the turn display.
			if (this.currentColor == Board.Black)
				this.currentColorPanel.BackColor = Color.Black;
			else
				this.currentColorPanel.BackColor = Color.White;
			this.currentColorPanel.Refresh();

			// If the current color is under computer control, set up for a
			// computer move.
			if (this.IsComputerPlayer(this.currentColor))
			{
				// Set the game state.
				this.gameState = ReversiForm.GameState.InComputerMove;

				// Check if computer play is currently suspended.
				if (this.isComputerPlaySuspended)
				{
					// Enable the "Resume Play" menu item and tool bar button
					// so that the user can resume the game.
					this.resumePlayMenuItem.Enabled =
						this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.ResumePlay].Enabled = true;

					// Set the status display.
					string shortCutText = this.resumePlayMenuItem.Shortcut.ToString();
					System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex("(Alt|Ctrl|Shift)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
					shortCutText = re.Replace(shortCutText, "$1+");
					this.statusLabel.Text = String.Format("{0} move... (Suspended, press {1} to resume play.)", playerText, shortCutText);
					this.statusProgressBar.Visible = false;
				}
				else
				{
					// Set the status display.
					this.statusLabel.Text = String.Format("{0} move, thinking... ", playerText);
					this.statusProgressBar.Minimum = 0;
					this.statusProgressBar.Maximum = this.board.GetValidMoveCount(this.currentColor);
					this.statusProgressBar.Value = 0;

					// Position the progress bar just to the right of the text and make it visible.
					this.statusProgressBar.Left = this.statusLabel.Left + this.statusLabel.Width;
					this.statusProgressBar.Visible = true;

					// Start a separate thread to perform the computer's move.
					this.calculateComputerMoveThread = new Thread(new ThreadStart(this.CalculateComputerMove));
					this.calculateComputerMoveThread.IsBackground = true;
					this.calculateComputerMoveThread.Priority = System.Threading.ThreadPriority.Lowest;
					this.calculateComputerMoveThread.Name = "Calculate Computer Move";
					this.calculateComputerMoveThread.Start();
				}
			}

			// Otherwise, set up for a user move.
			else
			{
				// Set the game state.
				this.gameState = ReversiForm.GameState.InPlayerMove;

				// Reset the keyed column and row numbers.
				this.keyedColNumber = -1;
				this.keyedRowNumber = -1;

				// Show valid moves, if that option is active.
				if (this.options.ShowValidMoves)
				{
					this.HighlightValidMoves();
					this.squaresPanel.Refresh();
				}

				// Update the status display.
				this.statusLabel.Text = String.Format("{0} move...", playerText);
				this.statusProgressBar.Visible = false;

				// Set focus on the form so it will receive key presses.
				this.Focus();
			}

			// Update the status display.
			this.statusPanel.Refresh();
		}

		//
		// Determines if a given color is being played by the computer.
		//
		private bool IsComputerPlayer(int color)
		{
			return ((this.options.ComputerPlaysBlack && color == Board.Black) ||
					(this.options.ComputerPlaysWhite && color == Board.White));
		}

		//
		// Makes a move for the current player.
		//
		private void MakeMove(int row, int col)
		{
			// Clean up the move history to ensure that it contains only the
			// moves made prior to this one.
			while (this.moveHistory.Count > this.moveNumber - 1)
				this.moveHistory.RemoveAt(this.moveHistory.Count - 1);

			// Add the move to the move list.
			string color = "Black";
			if (this.currentColor == Board.White)
				color = "White";
			string[] subItems =
			{
				String.Empty,
				this.moveNumber.ToString(),
				color,
				(alpha[col] + (row + 1).ToString())
			};
			ListViewItem listItem = new ListViewItem(subItems);
			this.moveListView.Items.Add(listItem);

			// If necessary, scroll the list to bring the last move into view.
			this.moveListView.EnsureVisible(this.moveListView.Items.Count - 1);

			// Add this move to the move history.
			this.moveHistory.Add(new MoveRecord(this.board, this.currentColor, listItem));

			// Enable/disable the move-related menu items and tool bar buttons as
			// appropriate.
			this.undoMoveMenuItem.Enabled =
				this.undoAllMovesMenuItem.Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.UndoMove].Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.UndoAllMoves].Enabled = true;
			this.redoMoveMenuItem.Enabled =
				this.redoAllMovesMenuItem.Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.RedoMove].Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.RedoAllMoves].Enabled = false;

			// Bump the move number.
			this.moveNumber++;

			// Update the status display.
			this.statusLabel.Text = "";
			this.statusProgressBar.Visible = false;
			this.statusPanel.Refresh();

			// Clear any board square highlighting.
			this.UnHighlightSquares();

			// Make a copy of the board (for doing move animation).
			Board oldBoard = new Board(this.board);

			// Make the move on the board.
			this.board.MakeMove(this.currentColor, row, col);

			// If the animate move option is active, set up animation for the
			// affected discs.
			if (this.options.AnimateMoves)
			{
				int i, j;
				for (i = 0; i < 8; i++)
					for (j = 0; j < 8; j++)
					{
						// Mark the newly added disc.
						if (i == row && j == col)
							this.squareControls[i, j].IsNew = true;
						else
						{
							// Initialize animation for the discs that were
							// flipped.
							if (this.board.GetTile(i, j) != oldBoard.GetTile(i, j))
								this.squareControls[i, j].AnimationCounter = SquareControl.AnimationStart;
						}
					}
			}

			// Update the display to reflect the board changes.
			this.UpdateBoardDisplay();

			// Save the player color.
			this.lastMoveColor = this.currentColor;

			// If the animate moves option is active, start the animation.
			if (this.options.AnimateMoves)
			{
				this.gameState = ReversiForm.GameState.InMoveAnimation;
				this.animationTimer.Start();
			}

			// Otherwise, end the move.
			else
				this.EndMove();
		}

		//
		// Called when a move has been completed (including any animation) to
		// start the next turn.
		//
		private void EndMove()
		{
			// Set the game state.
			this.gameState = ReversiForm.GameState.MoveCompleted;

			// Switch players and start the next turn.
			this.currentColor *= -1;
			this.StartTurn();
		}

		//
		// Updates the animation of a move.
		//
		private void AnimateMove(Object sender, EventArgs e)
		{
			// Lock the board to prevent race conditions.
			lock (this.board)
			{
				// If a move is being animated, advance the animation counters on
				// the square controls.
				if (this.gameState == ReversiForm.GameState.InMoveAnimation)
				{
					bool isComplete = true;
					int i, j;
					for (i = 0; i < 8; i++)
						for (j = 0; j < 8; j++)
							if (this.squareControls[i, j].AnimationCounter > SquareControl.AnimationStop)
							{
								this.squareControls[i, j].AnimationCounter--;
								isComplete = false;
							}

					// Refresh the display.
					this.squaresPanel.Refresh();

					// If the animation is complete, end the move.
					if (isComplete)
					{
						this.StopMoveAnimation();
						this.UpdateBoardDisplay();
						this.EndMove();
					}
				}
			}
		}

		//
		// Stops animation of a move and resets the squares.
		//
		private void StopMoveAnimation()
		{
			// Stop the animation timer.
			this.animationTimer.Stop();

			// Reset the animation counters and new disc flag on all squares.
			SquareControl squareControl;
			int i, j;
			for (i = 0; i < 8; i++)
				for (j = 0; j < 8; j++)
				{
					squareControl = (SquareControl) this.squaresPanel.Controls[i * 8 + j];
					squareControl.AnimationCounter = SquareControl.AnimationStop;
					squareControl.IsNew = false;
				}
		}

		//
		// Makes a player-controlled move for the current color.
		//
		private void MakePlayerMove(int row, int col)
		{
			// Allow the computer to resume play.
			this.isComputerPlaySuspended = false;

			// Make the move.
			this.MakeMove(row, col);
		}

		// ===================================================================
		// Code to handle computer moves.
		// ===================================================================

		//
		// Cancels the computer move thread, if it is active.
		//
		private void KillComputerMoveThread()
		{
			if (this.calculateComputerMoveThread == null || this.calculateComputerMoveThread.ThreadState == ThreadState.Stopped)
				return;

			try
			{
				this.calculateComputerMoveThread.Abort();
				this.calculateComputerMoveThread.Join();
			}
			catch (Exception)
			{}
			finally
			{
				this.calculateComputerMoveThread = null;
			}
		}

		//
		// Define delegates for callbacks from the worker thread.
		//
		public delegate void UpdateStatusProgressDelegate();
		public delegate void MakeComputerMoveDelegate(int row, int col);

		//
		// Updates the status progress bar.
		// Note: Called from the worker thread.
		//
		private void UpdateStatusProgress()
		{
			// Increase the progress bar value by one.
			if (this.statusProgressBar.Value < this.statusProgressBar.Maximum)
			{
				this.statusProgressBar.Value++;
				this.statusProgressBar.Refresh();
			}
		}

		//
		// Makes a computer-controlled move for the current color.
		// Note: Called from the worker thread.
		//
		private void MakeComputerMove(int row, int col)
		{
			// Lock the board to prevent a race condition while performing the
			// move.
			lock (this.board)
			{
				// Make the move.
				this.MakeMove(row, col);
			}
		}

		//
		// Calculates a computer move.
		// Note: Executed in the worker thread.
		//
		private void CalculateComputerMove()
		{
			// Load the AI parameters.
			this.agents[0].LookAheadDepth = this.options.BlackDifficulty;
			this.agents[0].Color = Board.Black;

			this.agents[1].LookAheadDepth = options.WhiteDifficulty;
			this.agents[1].Color = Board.White;

			// Find the best available move.
			ComputerMove move = this.agents[(this.currentColor < 0? 0 : 1)].DetermineMove(board);

			// Perform a callback to make the move.
			Object[] args = { move.row, move.column };
			MakeComputerMoveDelegate moveDelegate = new MakeComputerMoveDelegate(this.MakeComputerMove);
			this.BeginInvoke(moveDelegate, args);
		}

		// ===================================================================
		// Code to handle undo/redo of moves.
		// ===================================================================

		//
		// Restores the game to the state it was in before the specified move
		// was made.
		//
		private void RestoreGameAt(int n)
		{
			// Get the move record.
			MoveRecord item = (MoveRecord) this.moveHistory[n];

			// Stop any animation.
			this.StopMoveAnimation();

			// Clear any board square highlighting.
			this.UnHighlightSquares();

			// Restore the board and update the display.
			this.board = new Board(item.board);
			this.UpdateBoardDisplay();

			// Restore the current player.
			this.currentColor = item.currentColor;

			// Restore the move list.
			this.moveListView.Items.Clear();
			for (int i = 0; i < n; i++)
			{
				item = (MoveRecord) this.moveHistory[i];
				this.moveListView.Items.Add(item.moveListItem);
			}
			if (this.moveListView.Items.Count > 0)
				this.moveListView.EnsureVisible(this.moveListView.Items.Count - 1);
			else
				this.moveListView.Refresh();

			// Set the current move number.
			this.moveNumber = n + 1;

			// Enable/disable the move-related menu items and tool bar buttons
			// as appropriate.
			this.undoMoveMenuItem.Enabled =
				this.undoAllMovesMenuItem.Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.UndoMove].Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.UndoAllMoves].Enabled = (this.moveNumber > 1);
			this.redoMoveMenuItem.Enabled =
				this.redoAllMovesMenuItem.Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.RedoMove].Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.RedoAllMoves].Enabled = (this.moveNumber < this.moveHistory.Count);
			this.resumePlayMenuItem.Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.ResumePlay].Enabled   = false;

			// Suspend computer play.
			this.isComputerPlaySuspended = true;
		}

		//
		// Undoes the previous move or all moves.
		//
		private void UndoMove(bool undoAll)
		{
			// Save the current game state so we'll know if we need to perform
			// a restart.
			GameState oldGameState = this.gameState;

			// Stop the computer move thread, if active.
			this.KillComputerMoveThread();

			// Lock the board to prevent any changes by an active computer
			// move.
			lock (this.board)
			{
				// When undoing the last move, we need to save the current
				// board and player color in the move history so that it can
				// be restored if the move is redone.
				if (this.moveHistory.Count < this.moveNumber)
				{
					// Add the data to the move history.
					this.moveHistory.Add(new MoveRecord(this.board, -this.lastMoveColor, new ListViewItem()));
				}

				// Undo either the previous move or all moves.
				this.RestoreGameAt((undoAll ? 0 : this.moveNumber - 2));
			}

			// If the game was over, restore the statistics and restart it.
			if (oldGameState == ReversiForm.GameState.GameOver)
			{
				this.statistics = new Statistics(this.oldStatistics);
				this.StartGame(true);
			}

			// Otherwise, start play at that move.
			else
				this.StartTurn();
		}

		//
		// Redoes the next move or all moves.
		//
		private void RedoMove(bool redoAll)
		{
			// Redo either the next move or all moves.
			this.RestoreGameAt((redoAll ? this.moveHistory.Count - 1 : this.moveNumber));

			// Start play at that move.
			this.StartTurn();
		}

		// ===================================================================
		// Code to handle the board display.
		// ===================================================================

		//
		// Updates the display to reflect the current game board.
		//
		private void UpdateBoardDisplay()
		{
			// Set counts.
			this.blackCountLabel.Text = this.board.BlackCount.ToString();
			this.blackCountLabel.Refresh();
			this.whiteCountLabel.Text = this.board.WhiteCount.ToString();
			this.whiteCountLabel.Refresh();

			// Map the current game board to the square controls.
			SquareControl squareControl;
			int i, j;
			for (i = 0; i < 8; i++)
				for (j = 0; j < 8; j++)
				{
					squareControl = (SquareControl) this.squaresPanel.Controls[i * 8 + j];
					squareControl.Contents = this.board.GetTile(i, j);
					squareControl.PreviewContents = Board.Empty;
				}

			// Redraw the board.
			this.squaresPanel.Refresh();
		}

		//
		// Highlights the board squares that represent valid moves for the
		// current player.
		//
		private void HighlightValidMoves()
		{
			// Check each square.
			SquareControl squareControl;
			int i, j;
			for (i = 0; i < 8; i++)
				for (j = 0; j < 8; j++)
				{
					squareControl = (SquareControl) this.squaresPanel.Controls[i * 8 + j];
					if (this.board.IsValidMove(this.currentColor, i, j))
						squareControl.IsValid = true;
					else
						squareControl.IsValid = false;
				}
		}

		//
		// Removes any highlighting from all the board squares.
		//
		private void UnHighlightSquares()
		{
			// Clear the flags on each square.
			SquareControl squareControl;
			int i, j;
			for (i = 0; i < 8; i++)
				for (j = 0; j < 8; j++)
				{
					squareControl = (SquareControl) this.squaresPanel.Controls[i * 8 + j];
					squareControl.IsActive = false;
					squareControl.IsValid  = false;
					squareControl.IsNew    = false;
				}
		}

		//
		// Sets the board square colors based on the current game options.
		//
		private void SetSquareControlColors()
		{
			SquareControl.ActiveSquareBackColor = this.options.ActiveSquareColor;
			SquareControl.NormalBackColor       = this.options.BoardColor;
			SquareControl.MoveIndicatorColor    = this.options.MoveIndicatorColor;
			SquareControl.ValidMoveBackColor    = this.options.ValidMoveColor;
		}

		//====================================================================
		// These functions to handle the loading and saving of the program
		// settings.
		//====================================================================

		//
		// Loads any saved program settings.
		//
		private void LoadProgramSettings()
		{
			// Load the saved window settings and resize the window.
			try
			{
				// Load the saved window settings.
				int left   = System.Int32.Parse(this.settings.GetValue("Window", "Left"));
				int top    = System.Int32.Parse(this.settings.GetValue("Window", "Top"));
				int width  = System.Int32.Parse(this.settings.GetValue("Window", "Width"));
				int height = System.Int32.Parse(this.settings.GetValue("Window", "Height"));

				// Reposition and resize the window.
				this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
				this.DesktopLocation = new Point(left, top);
				this.ClientSize      = new Size(width, height);

				// Load saved options.
				this.options.ShowValidMoves     = (bool) System.Boolean.Parse(this.settings.GetValue("Options", "ShowValidMoves"));
				this.options.PreviewMoves       = (bool) System.Boolean.Parse(this.settings.GetValue("Options", "PreviewMoves"));
				this.options.AnimateMoves       = (bool) System.Boolean.Parse(this.settings.GetValue("Options", "AnimateMoves"));
				this.options.BoardColor         = Color.FromArgb(System.Int32.Parse(this.settings.GetValue("Options", "BoardColor")));
				this.options.ValidMoveColor     = Color.FromArgb(System.Int32.Parse(this.settings.GetValue("Options", "ValidMoveColor")));
				this.options.ActiveSquareColor  = Color.FromArgb(System.Int32.Parse(this.settings.GetValue("Options", "ActiveSquareColor")));
				this.options.MoveIndicatorColor = Color.FromArgb(System.Int32.Parse(this.settings.GetValue("Options", "MoveIndicatorColor")));
				this.options.ComputerPlaysBlack = (bool) System.Boolean.Parse(this.settings.GetValue("Options", "ComputerPlaysBlack"));
				this.options.ComputerPlaysWhite = (bool) System.Boolean.Parse(this.settings.GetValue("Options", "ComputerPlaysWhite"));
				this.options.BlackDifficulty = (int)System.Int32.Parse(this.settings.GetValue("Options", "BlackDifficulty"));
				this.options.WhiteDifficulty = (int)System.Int32.Parse(this.settings.GetValue("Options", "WhiteDifficulty"));

				// Set the square control colors based on options loaded.
				this.SetSquareControlColors();

				// Load saved statistics.
				this.statistics.BlackWins          = System.Int32.Parse(settings.GetValue("Statistics", "BlackWins"));
				this.statistics.WhiteWins          = System.Int32.Parse(settings.GetValue("Statistics", "WhiteWins"));
				this.statistics.OverallDraws       = System.Int32.Parse(settings.GetValue("Statistics", "OverallDraws"));
				this.statistics.BlackTotalScore    = System.Int32.Parse(settings.GetValue("Statistics", "BlackTotalScore"));
				this.statistics.WhiteTotalScore    = System.Int32.Parse(settings.GetValue("Statistics", "WhiteTotalScore"));
				this.statistics.ComputerWins       = System.Int32.Parse(settings.GetValue("Statistics", "ComputerWins"));
				this.statistics.UserWins           = System.Int32.Parse(settings.GetValue("Statistics", "UserWins"));
				this.statistics.VsComputerDraws    = System.Int32.Parse(settings.GetValue("Statistics", "VsComputerDraws"));
				this.statistics.ComputerTotalScore = System.Int32.Parse(settings.GetValue("Statistics", "ComputerTotalScore"));
				this.statistics.UserTotalScore     = System.Int32.Parse(settings.GetValue("Statistics", "UserTotalScore"));
			}
			catch (Exception)
			{}
		}

		//
		// Saves the current program settings.
		//
		private void SaveProgramSettings()
		{
			// Save window settings.
			this.settings.SetValue("Window", "Left", this.windowSettings.Left.ToString());
			this.settings.SetValue("Window", "Top", this.windowSettings.Top.ToString());
			this.settings.SetValue("Window", "Width", this.windowSettings.Width.ToString());
			this.settings.SetValue("Window", "Height", this.windowSettings.Height.ToString());

			// Save game options.
			this.settings.SetValue("Options", "ShowValidMoves", this.options.ShowValidMoves.ToString());
			this.settings.SetValue("Options", "PreviewMoves", this.options.PreviewMoves.ToString());
			this.settings.SetValue("Options", "AnimateMoves", this.options.AnimateMoves.ToString());
			this.settings.SetValue("Options", "BoardColor", SquareControl.NormalBackColor.ToArgb().ToString());
			this.settings.SetValue("Options", "ValidMoveColor", SquareControl.ValidMoveBackColor.ToArgb().ToString());
			this.settings.SetValue("Options", "ActiveSquareColor", SquareControl.ActiveSquareBackColor.ToArgb().ToString());
			this.settings.SetValue("Options", "MoveIndicatorColor", SquareControl.MoveIndicatorColor.ToArgb().ToString());
			this.settings.SetValue("Options", "ComputerPlaysBlack", this.options.ComputerPlaysBlack.ToString());
			this.settings.SetValue("Options", "ComputerPlaysWhite", this.options.ComputerPlaysWhite.ToString());
			this.settings.SetValue("Options", "BlackDifficulty", this.options.BlackDifficulty.ToString());
			this.settings.SetValue("Options", "WhiteDifficulty", this.options.WhiteDifficulty.ToString());

			// Save statistics.
			this.settings.SetValue("Statistics", "BlackWins", this.statistics.BlackWins.ToString());
			this.settings.SetValue("Statistics", "WhiteWins", this.statistics.WhiteWins.ToString());
			this.settings.SetValue("Statistics", "OverallDraws", this.statistics.OverallDraws.ToString());
			this.settings.SetValue("Statistics", "BlackTotalScore", this.statistics.BlackTotalScore.ToString());
			this.settings.SetValue("Statistics", "WhiteTotalScore", this.statistics.WhiteTotalScore.ToString());
			this.settings.SetValue("Statistics", "ComputerWins", this.statistics.ComputerWins.ToString());
			this.settings.SetValue("Statistics", "UserWins", this.statistics.UserWins.ToString());
			this.settings.SetValue("Statistics", "VsComputerDraws", this.statistics.VsComputerDraws.ToString());
			this.settings.SetValue("Statistics", "ComputerTotalScore", this.statistics.ComputerTotalScore.ToString());
			this.settings.SetValue("Statistics", "UserTotalScore", this.statistics.UserTotalScore.ToString());

			// Save the program settings.
			this.settings.Save();
		}

		//====================================================================
		// Event handlers for the form.
		//====================================================================

		//
		// Handles a window close.
		//
		private void ReversiForm_Closed(object sender, System.EventArgs e)
		{
			// Stop the computer move thread, if active.
			this.KillComputerMoveThread();

			// Save the current program settings.
			this.SaveProgramSettings();
		}

		//
		// Handles a window close request. If a game is active, it will prompt
		// for confirmation first.
		//
		private void ReversiForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			bool doClose = true;

			// Prompt for confirmation if a game is in progress.
			if (this.gameState != ReversiForm.GameState.GameOver)
			{
				// Create and show the confirm dialog, saving the result.
				ConfirmDialog dlg = new ConfirmDialog("Exit the program?");
				if (dlg.ShowDialog(this) != DialogResult.Yes)
					doClose = false;
				dlg.Dispose();
			}

			// Cancel the close request if necessary.
			if (!doClose)
				e.Cancel = true;
		}

		//
		// Handles a key press.
		//
		private void ReversiForm_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			// Check the game state to ensure it is the user's turn.
			if (this.gameState != ReversiForm.GameState.InPlayerMove)
				return;

			// Convert the key character to column/row number.
			string s = e.KeyChar.ToString().ToUpper();
			int col = ReversiForm.alpha.IndexOf(s);
			int row;
			try
			{
				row = System.Int32.Parse(s) - 1;
			}
			catch
			{
				row = -1;
			}

			// Is the key a valid column letter?
			if (col >= 0 && col < 8)
			{
				// Yes; store it, clear the keyed row number and exit.
				this.keyedColNumber = col;
				this.keyedRowNumber = -1;
				return;
			}

			// If we don't have a valid column number yet, exit.
			if (this.keyedColNumber < 0 || this.keyedColNumber > 7)
				return;

			// We have a valid column number. Is the key a valid row number?
			if (row < 0 || row > 7)
			{
				// No, reset both and exit.
				this.keyedColNumber = -1;
				this.keyedRowNumber = -1;
				return;
			}
			else
				this.keyedRowNumber = row;

			// The keyed column and row numbers are valid. Is the position a
			// valid move?
			if (this.board.IsValidMove(this.currentColor, this.keyedRowNumber, this.keyedColNumber))
			{
				// Yes, make it.
				this.MakePlayerMove(this.keyedRowNumber, this.keyedColNumber);
			}
			else
			{
				// No, reset both.
				this.keyedColNumber = -1;
				this.keyedRowNumber = -1;
			}
		}

		//
		// Handles a window move.
		//
		private void ReversiForm_Move(object sender, System.EventArgs e)
		{
			// If the window has not been minimized or maximized, save its location.
			if (this.WindowState == FormWindowState.Normal)
			{
				this.windowSettings.X = this.DesktopLocation.X;
				this.windowSettings.Y = this.DesktopLocation.Y;
			}
		}

		//
		// Handles a window resize.
		//
		private void ReversiForm_Resize(object sender, System.EventArgs e)
		{
			// Determine the size each square should be within the board.
			int l = (int) (Math.Min(this.squaresPanel.Width, this.squaresPanel.Height) / 8);
			l = Math.Max(l, 8);

			// Resize and reposition each square control.
			int i, j;
			for (i = 0; i < 8; i++)
				for (j = 0; j < 8; j++)
				{
					this.squareControls[i, j].Width  = l;
					this.squareControls[i, j].Height = l;
					this.squareControls[i, j].Left   = j * l;
					this.squareControls[i, j].Top    = i * l;
				}

			// Fix the column and row headers.
			for (i = 0; i < 8; i++)
			{
				this.colLabels[i].Width  = l;
				this.colLabels[i].Left   = this.cornerLabel.Width  + i * l;
				this.rowLabels[i].Height = l;
				this.rowLabels[i].Top    = this.cornerLabel.Height + i * l;
			}

			// Fix the info panel height to align it's bottom with the bottom row of squares.
			this.infoPanel.Height = 8 * l + this.colLabels[0].Height;

			// Position the progress bar just to the right of the satus text.
			this.statusProgressBar.Left = this.statusLabel.Left + this.statusLabel.Width;

			// If the window has not been minimized or maximized, save it size.
			if (this.WindowState == FormWindowState.Normal)
			{
				this.windowSettings.Width = this.ClientSize.Width;
				this.windowSettings.Height = this.ClientSize.Height;
			}
		}

		//====================================================================
		// Event handlers for the menu items.
		//====================================================================

		//
		// Handles a "New Game" click.
		//
		private void newGameMenuItem_Click(object sender, System.EventArgs e)
		{
			// Start a new game.
			this.StartGame();
		}

		//
		// Handles a "Resign Game" click.
		//
		private void resignGameMenuItem_Click(object sender, System.EventArgs e)
		{
			bool doEnd = true;

			// Prompt for confirmation if a game is in progress.
			if (this.gameState != ReversiForm.GameState.GameOver)
			{
				// Create and show the confirm dialog, saving the result.
				ConfirmDialog dlg = new ConfirmDialog("Resign this game?");
				if (dlg.ShowDialog(this) != DialogResult.Yes)
					doEnd = false;

				dlg.Dispose();
			}

			// End the game if the request was not cancelled.
			if (doEnd)
			{
				// Stop the computer move thread, if active.
				this.KillComputerMoveThread();

				// Stop any active animation and reset the board display.
				this.StopMoveAnimation();
				this.UnHighlightSquares();
				this.UpdateBoardDisplay();

				// End the game with the resignation flag set.
				this.EndGame(true);
			}
		}

		//
		// Handles an "Options..." click.
		//
		private void optionsMenuItem_Click(object sender, System.EventArgs e)
		{
			// Create the options dialog and set the option controls according
			// to the current game options.
			OptionsDialog dlg = new OptionsDialog(this.options);

			// Show the options dialog and if the "OK" button was pressed,
			// update the game options.
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				// Update the game options that are safe to change using the
				// values from the dialog.
				this.options.ShowValidMoves         = dlg.Options.ShowValidMoves;
				this.options.PreviewMoves           = dlg.Options.PreviewMoves;
				this.options.AnimateMoves           = dlg.Options.AnimateMoves;
				this.options.BoardColor             = dlg.Options.BoardColor;
				this.options.ValidMoveColor         = dlg.Options.ValidMoveColor;
				this.options.ActiveSquareColor      = dlg.Options.ActiveSquareColor;
				this.options.MoveIndicatorColor     = dlg.Options.MoveIndicatorColor;
				this.options.BlackDifficulty        = dlg.Options.BlackDifficulty;
				this.options.WhiteDifficulty        = dlg.Options.WhiteDifficulty;
                this.options.BlackUsesExampleAI    = dlg.Options.BlackUsesExampleAI;
                this.options.WhiteUsesExampleAI    = dlg.Options.WhiteUsesExampleAI;
                if (this.options.BlackUsesExampleAI)
                    this.agents[0].behavior = exampleAIBehavior;
                else
                    this.agents[0].behavior = studentBehavior;
                if (this.options.WhiteUsesExampleAI)
                    this.agents[1].behavior = exampleAIBehavior;
                else
                    this.agents[1].behavior = studentBehavior;

				// Set the square control colors based on the current color options.
				this.SetSquareControlColors();

				// If a game is currently in progress, special handling is
				// needed for changes to the player options.
				if (this.gameState != ReversiForm.GameState.GameOver)
				{
					// Lock the board to prevent race conditions.
					lock (this.board)
					{
						// If a move is currently being animated, complete it
						// now.
						if (this.gameState == ReversiForm.GameState.InMoveAnimation)
						{
							// Stop animation and end the move.
							this.StopMoveAnimation();
							this.UpdateBoardDisplay();
							this.EndMove();
						}

						// Clear any board square highlighting.
						this.UnHighlightSquares();

						// If the changes to the player options affect the
						// current player, restart the current turn.
						if (dlg.Options.ComputerPlaysBlack != this.options.ComputerPlaysBlack ||
							dlg.Options.ComputerPlaysWhite != this.options.ComputerPlaysWhite)
						{
							// Kill any currently active computer move.
							this.KillComputerMoveThread();

							// Set the player options.
							this.options.ComputerPlaysBlack = dlg.Options.ComputerPlaysBlack;
							this.options.ComputerPlaysWhite = dlg.Options.ComputerPlaysWhite;

							// If no moves have been made yet, set the current color
							// to black.
							if (this.lastMoveColor == Board.Empty)
								this.currentColor = Board.Black;

							// Update the board display.
							this.squaresPanel.Refresh();

							// Restart the current turn.
							this.StartTurn();
						}

						// Otherwise, set the player options and update the
						// board display.
						else
						{
							// Set the player options.
							this.options.ComputerPlaysBlack = dlg.Options.ComputerPlaysBlack;
							this.options.ComputerPlaysWhite = dlg.Options.ComputerPlaysWhite;

							// Highlight valid moves, if appropriate.
							if (this.options.ShowValidMoves && !this.IsComputerPlayer(this.currentColor))
								this.HighlightValidMoves();

							// Update the board display.	
							this.squaresPanel.Refresh();
						}
					}
				}

				// A game is not in progress so just set the player options.
				else
				{
					this.options.ComputerPlaysBlack = dlg.Options.ComputerPlaysBlack;
					this.options.ComputerPlaysWhite = dlg.Options.ComputerPlaysWhite;

					// Update the board display, in case the colors have been
					// changed.
					this.squaresPanel.Refresh();
				}
			}
	
			dlg.Dispose();
		}

		//
		// Handles a "Statistics..." click.
		//
		private void statisticsMenuItem_Click(object sender, System.EventArgs e)
		{
			// Create and show the statistics dialog.
			StatisticsDialog dlg = new StatisticsDialog(this.statistics);
			dlg.ShowDialog(this);
			dlg.Dispose();
		}

		//
		// Handles an "Exit" click.
		//
		private void exitMenuItem_Click(object sender, System.EventArgs e)
		{
			// Close the form.
			this.Close();
		}

		//
		// Handles an "Undo Move" click.
		//
		private void undoMoveMenuItem_Click(object sender, System.EventArgs e)
		{
			// Undo the previous move.
			this.UndoMove(false);
		}

		//
		// Handles a "Redo Move" click.
		//
		private void redoMoveMenuItem_Click(object sender, System.EventArgs e)
		{
			// Redo the next move.
			this.RedoMove(false);
		}

		//
		// Handles an "Undo All Moves" click.
		//
		private void undoAllMovesmenuItem_Click(object sender, System.EventArgs e)
		{
			// Undo all moves.
			this.UndoMove(true);
		}

		//
		// Handles a "Redo All Moves" click.
		//
		private void redoAllMovesMenuItem_Click(object sender, System.EventArgs e)
		{
			// Redo all moves.
			this.RedoMove(true);
		}

		//
		// Handles a "Resume Play" click.
		//
		private void resumePlayMenuItem_Click(object sender, System.EventArgs e)
		{
			// Disable the "Redo Move," "Redo All Moves" and "Resume Play"
			// menu items and tool bar buttons.
			this.redoMoveMenuItem.Enabled =
				this.redoAllMovesMenuItem.Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.RedoMove].Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.RedoAllMoves].Enabled = false;
			this.resumePlayMenuItem.Enabled =
				this.playToolBar.Buttons[(int) ReversiForm.ToolBarButton.ResumePlay].Enabled   = false;

			// Set the game state.
			this.gameState = ReversiForm.GameState.MoveCompleted;

			// Clear the suspend computer play flag and restart the turn.
			this.isComputerPlaySuspended = false;
			this.StartTurn();
		}

		//
		// Handles a "Help Topics" click.
		//
		private void helpTopicsMenuItem_Click(object sender, System.EventArgs e)
		{
			// If the help file exists, show it. Otherwise, display an error
			// message.
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(ReversiForm.helpFileName);
			if (fileInfo.Exists)
				Help.ShowHelp(this, ReversiForm.helpFileName);
			else
				MessageBox.Show(this,
					String.Format("Help file '{0}' not found.", ReversiForm.helpFileName),
					"File Not Found",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
		}

		//
		// Handles an "About" click.
		//
		private void aboutMenuItem_Click(object sender, System.EventArgs e)
		{
			// Create and show the about dialog.
			AboutDialog dlg = new AboutDialog();
			dlg.ShowDialog(this);
			dlg.Dispose();
		}

		//====================================================================
		// Event handlers for the tool bar.
		//====================================================================

		//
		// Handles a button click on the tool bar.
		//
		private void playToolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			// Determine which button was clicked and simulate a click on the
			// corresponding menu item.
			switch(this.playToolBar.Buttons.IndexOf(e.Button))
			{
				case (int) ReversiForm.ToolBarButton.NewGame:
					this.newGameMenuItem.PerformClick();
					break;
				case (int) ReversiForm.ToolBarButton.ResignGame:
					this.resignGameMenuItem.PerformClick();
					break;
				case (int) ReversiForm.ToolBarButton.UndoAllMoves:
					this.undoAllMovesMenuItem.PerformClick();
					break;
				case (int) ReversiForm.ToolBarButton.UndoMove:
					this.undoMoveMenuItem.PerformClick();
					break;
				case (int) ReversiForm.ToolBarButton.ResumePlay:
					this.resumePlayMenuItem.PerformClick();
					break;
				case (int) ReversiForm.ToolBarButton.RedoMove:
					this.redoMoveMenuItem.PerformClick();
					break;
				case (int) ReversiForm.ToolBarButton.RedoAllMoves:
					this.redoAllMovesMenuItem.PerformClick();
					break;
				default:
					break;
			}
		}

		// ===================================================================
		// Event handlers for the square controls.
		// ===================================================================

		//
		// Handles a mouse move on a board square.
		//
		private void SquareControl_MouseMove(object sender, MouseEventArgs e)
		{
			// Check the game state to ensure that it is the user's turn.
			if (this.gameState != ReversiForm.GameState.InPlayerMove)
				return;

			SquareControl squareControl = (SquareControl) sender;

			// If the square is a valid move for the current player,
			// indicate it.
			if (this.board.IsValidMove(this.currentColor, squareControl.Row, squareControl.Col))
			{
				// 
				if (!squareControl.IsActive && squareControl.PreviewContents == Board.Empty)
				{
					// If the show valid moves option is active, mark the
					// square.
					if (this.options.ShowValidMoves)
					{
						squareControl.IsActive = true;

						// If the preview moves option is not active, update
						// the square display now.
						if (!this.options.PreviewMoves)
							squareControl.Refresh();
					}

					// If the preview moves option is active, mark the
					// appropriate squares.
					if (this.options.PreviewMoves)
					{
						// Create a temporary board to make the move on.
						Board board = new Board(this.board);
						board.MakeMove(this.currentColor, squareControl.Row, squareControl.Col);

						// Set up the move preview.
						for (int i = 0; i < 8; i++)
							for (int j = 0; j < 8; j++)
								if (board.GetTile(i, j) != this.board.GetTile(i, j))
								{
									// Set and update the square display.
									this.squareControls[i, j].PreviewContents = board.GetTile(i, j);
									this.squareControls[i, j].Refresh();
								}
					}
				}

				// Change the cursor.
				squareControl.Cursor = System.Windows.Forms.Cursors.Hand;
			}
		}

		//
		// Handles a mouse leave on a board square.
		//
		private void SquareControl_MouseLeave(object sender, System.EventArgs e)
		{
			SquareControl squareControl = (SquareControl) sender;

			// If the square is currently active, deactivate it.
			if (squareControl.IsActive)
			{
				squareControl.IsActive = false;
				squareControl.Refresh();
			}

			// If the move is being previewed, clear all affected squares.

			if (squareControl.PreviewContents != Board.Empty)
			{
				// Clear the move preview.
				for (int i = 0; i < 8; i++)
					for (int j = 0; j < 8; j++)
						if (this.squareControls[i, j].PreviewContents != Board.Empty)
						{
							this.squareControls[i, j].PreviewContents = Board.Empty;
							this.squareControls[i, j].Refresh();
						}
			}

			// Restore the cursor.
			squareControl.Cursor = System.Windows.Forms.Cursors.Default;
		}

		//
		// Handles a click on a board square.
		//
		private void SquareControl_Click(object sender, System.EventArgs e)
		{
			// Check the game state to ensure it's the user's turn.
			if (this.gameState != ReversiForm.GameState.InPlayerMove)
				return;

			SquareControl squareControl = (SquareControl) sender;

			// If the move is valid, make it.
			if (this.board.IsValidMove(this.currentColor, squareControl.Row, squareControl.Col))
			{
				// Restore the cursor.
				squareControl.Cursor = System.Windows.Forms.Cursors.Default;

				// Make the move.
				this.MakePlayerMove(squareControl.Row, squareControl.Col);
			}
		}
	}
}