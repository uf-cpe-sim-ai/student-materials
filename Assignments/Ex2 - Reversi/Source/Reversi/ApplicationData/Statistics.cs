using GameAI.GamePlaying.Core;

//
// Modified: 2011/04/26, Derek Bliss (Full Sail University)
// Modified: 2019/12/20, Jeremiah Blanchard (University of Florida)
namespace GameAI.GamePlaying
{
	/// <summary>
	/// Summary description for Statistics.
	/// </summary>
	public class Statistics
	{
		// Define the game statistics.
		public int BlackWins;
		public int WhiteWins;
		public int OverallDraws;
		public int BlackTotalScore;
		public int WhiteTotalScore;
		public int ComputerWins;
		public int UserWins;
		public int VsComputerDraws;
		public int ComputerTotalScore;
		public int UserTotalScore;

		//
		// Creates a new Statistics object will all counts set to zero.
		//
		public Statistics()
		{
			//
			// TODO: Add constructor logic here
			//

			// Initialize the game statistics.
			this.Reset();
		}

		//
		// Creates a new Statistics object by copying and existing one.
		//
		public Statistics(Statistics statistics)
		{
			this.BlackWins          = statistics.BlackWins;
			this.WhiteWins          = statistics.WhiteWins;
			this.OverallDraws       = statistics.OverallDraws;
			this.BlackTotalScore    = statistics.BlackTotalScore;
			this.WhiteTotalScore    = statistics.WhiteTotalScore;
			this.ComputerWins       = statistics.ComputerWins;
			this.UserWins           = statistics.UserWins;
			this.VsComputerDraws    = statistics.VsComputerDraws;
			this.ComputerTotalScore = statistics.ComputerTotalScore;
			this.UserTotalScore     = statistics.UserTotalScore;
		}

		//
		// Resets the game statistics.
		//
		public void Reset()
		{
			// Set all counts to zero.
			this.BlackWins          = 0;
			this.WhiteWins          = 0;
			this.OverallDraws       = 0;
			this.BlackTotalScore    = 0;
			this.WhiteTotalScore    = 0;
			this.ComputerWins       = 0;
			this.UserWins           = 0;
			this.VsComputerDraws    = 0;
			this.ComputerTotalScore = 0;
			this.UserTotalScore     = 0;
		}

		//
		// Updates the game statistics.
		//
		public void Update(int blackScore, int whiteScore, int computerColor, int userColor)
		{

			// Update the overall Black vs. White counts.
			this.BlackTotalScore += blackScore;
			this.WhiteTotalScore += whiteScore;
			if (blackScore > whiteScore)
				this.BlackWins++;
			else if (whiteScore > blackScore)
				this.WhiteWins++;
			else
				this.OverallDraws++;

			// Update the Computer vs. User counts, if applicable.
			if (computerColor == Board.Empty || userColor == Board.Empty)
				return;

			// Assign scores.
			int computerScore = blackScore;
			int userScore     = whiteScore;
			if (computerColor == Board.White)
			{
				computerScore = whiteScore;
				userScore     = blackScore;
			}
			// Update the counts.
			this.ComputerTotalScore += computerScore;
			this.UserTotalScore     += userScore;
			if (computerScore > userScore)
				this.ComputerWins++;
			else if (userScore > computerScore)
				this.UserWins++;
			else
				this.VsComputerDraws++;
		}
	}
}
