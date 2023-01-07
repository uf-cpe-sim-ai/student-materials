using System.Drawing;
using GameAI.GamePlaying.Core;

//
// Modified: 2011/04/26, Derek Bliss (Full Sail University)
// Modified: 2019/12/20, Jeremiah Blanchard (University of Florida)
namespace GameAI.GamePlaying
{
	/// <summary>
	/// Summary description for Options.
	/// </summary>
	public class Options
	{

		// Define the game options.
		public bool  ShowValidMoves;
		public bool  PreviewMoves;
		public bool  AnimateMoves;
		public Color BoardColor;
		public Color ValidMoveColor;
		public Color ActiveSquareColor;
		public Color MoveIndicatorColor;
		public bool  ComputerPlaysBlack;
		public bool  ComputerPlaysWhite;
		public int   BlackDifficulty;
        public int   WhiteDifficulty;
        public bool  BlackUsesExampleAI;
        public bool  WhiteUsesExampleAI;

		//
		// Creates a new Options object using the defaults.
		//
		public Options()
		{
			//
			// TODO: Add constructor logic here
			//

			// Initialize the game options to their default values.
			RestoreDefaults();
		}

		//
		// Creates a new Options object by copying an existing one.
		//
		public Options(Options options)
		{
			ShowValidMoves         = options.ShowValidMoves;
			PreviewMoves           = options.PreviewMoves;
			AnimateMoves           = options.AnimateMoves;
			BoardColor             = options.BoardColor;
			ValidMoveColor         = options.ValidMoveColor;
			ActiveSquareColor      = options.ActiveSquareColor;
			MoveIndicatorColor     = options.MoveIndicatorColor;
			ComputerPlaysBlack     = options.ComputerPlaysBlack;
			ComputerPlaysWhite     = options.ComputerPlaysWhite;
            BlackDifficulty        = options.BlackDifficulty;
            WhiteDifficulty        = options.WhiteDifficulty;
            BlackUsesExampleAI    = options.BlackUsesExampleAI;
            WhiteUsesExampleAI    = options.WhiteUsesExampleAI;
        }

		//
		// Restores all game options to their default values.
		//
		public void RestoreDefaults()
		{
			ShowValidMoves         = true;
			AnimateMoves           = true;
			PreviewMoves           = false;
			BoardColor             = SquareControl.NormalBackColorDefault;
			ValidMoveColor         = SquareControl.ValidMoveBackColorDefault;
			ActiveSquareColor      = SquareControl.ActiveSquareBackColorDefault;
			MoveIndicatorColor     = SquareControl.MoveIndicatorColorDefault;
			ComputerPlaysBlack     = false;
			ComputerPlaysWhite     = true;
            BlackDifficulty        = (int)Agent.Difficulty.Intermediate;
            WhiteDifficulty        = (int)Agent.Difficulty.Intermediate;
            BlackUsesExampleAI    = true;
            WhiteUsesExampleAI    = true;
        }
	}
}
