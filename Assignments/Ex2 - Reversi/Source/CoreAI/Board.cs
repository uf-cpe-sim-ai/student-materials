//
// Modified: 2011/04/26, Derek Bliss (Full Sail University)
// Modified: 2019/12/20, Jeremiah Blanchard (University of Florida)
namespace GameAI.GamePlaying.Core
{
    /// <summary>
    /// Class representing the reversi game board.
    /// </summary>
    public class Board
    {
        // These constants represent the contents of a board square.
        public static readonly int Black = -1;
        public static readonly int Empty =  0;
        public static readonly int White =  1;
        public static readonly int Width = 8;
        public static readonly int Height = 8;

        // These counts reflect the current board situation.
        public int BlackCount
        {
            get { return this.blackCount; }
        }
        public int WhiteCount
        {
            get { return this.whiteCount; }
        }
        public int EmptyCount
        {
            get { return this.emptyCount; }
        }
        public int Score
        {
            get { return whiteCount - blackCount; }
        }
        public int this[int row, int col]
        {
            get { return squares[row, col]; }
        }

        // Internal counts.
        private int blackCount;
        private int whiteCount;
        private int emptyCount;

        // This two-dimensional array represents the squares on the board.
        private int[,] squares;

        //
        // Creates a new, empty Board object.
        //
        public Board()
        {
            // Create the squares and safe disc map.
            this.squares = new int[Height, Width];

            // Clear the board and map.
            int i, j;
            for (i = 0; i < Height; i++)
                for (j = 0; j < Width; j++)
                {
                    this.squares[i, j] = Board.Empty;
                }

            // Update the counts.
            this.UpdateCounts();
        }

        //
        // Creates a new Board object by copying an existing one.
        //
        public Board(Board board)
        {
            // Create the squares and map.
            this.squares = new int[Height, Width];

            // Copy the given board.
            int i, j;
            for (i = 0; i < Height; i++)
                for (j = 0; j < Width; j++)
                {
                    this.squares[i, j] = board.squares[i, j];
                }

            // Copy the counts.
            this.blackCount = board.blackCount;
            this.whiteCount = board.whiteCount;
            this.emptyCount = board.emptyCount;
        }

        //
        // Sets this Board as a copy of board.
        //
        public void Copy(Board board)
        {
            // Copy the given board.
            int i, j;
            for (i = 0; i < Height; i++)
                for (j = 0; j < Width; j++)
                {
                    this.squares[i, j] = board.squares[i, j];
                }

            // Copy the counts.
            this.blackCount = board.blackCount;
            this.whiteCount = board.whiteCount;
            this.emptyCount = board.emptyCount;
        }

        //
        // Sets a board with the initial game set-up.
        //
        public void SetForNewGame()
        {
            // Clear the board.
            int i, j;
            for (i = 0; i < 8; i++)
                for (j = 0; j < 8; j++)
                {
                    this.squares[i, j] = Board.Empty;
                }

            // Set two black and two white discs in the center.
            this.squares[3, 3] = White;
            this.squares[3, 4] = Black;
            this.squares[4, 3] = Black;
            this.squares[4, 4] = White;

            // Update the counts.
            this.UpdateCounts();
        }

        //
        // Returns the contents of a given board square.
        //
        public int GetTile(int row, int col)
        {
            return this.squares[row, col];
        }

        //
        // Places a disc for the player on the board and flips any outflanked
        // opponents.
        // Note: For performance reasons, it does NOT check that the move is
        // valid.
        //
        public void MakeMove(int color, int row, int col)
        {
            // Set the disc on the square.
            this.squares[row, col] = color;

            // Flip any flanked opponents.
            int dr, dc;
            int r, c;
            for (dr = -1; dr <= 1; dr++)
                for (dc = -1; dc <= 1; dc++)
                    // Are there any outflanked opponents?
                    if (!(dr == 0 && dc == 0) && IsOutflanking(color, row, col, dr, dc))
                    {
                        r = row + dr;
                        c = col + dc;
                        // Flip 'em.
                        while (this.squares[r, c] == -color)
                        {
                            this.squares[r, c] = color;
                            r += dr;
                            c += dc;
                        }
                    }

            // Update the counts.
            this.UpdateCounts();
        }

        //
        // Determines if the player can make any valid move on the board.
        //
        public bool HasAnyValidMove(int color)
        {
            // Check all board positions for a valid move.
            int r, c;
            for (r = 0; r < 8; r++)
                for (c = 0; c < 8; c++)
                    if (this.IsValidMove(color, r, c))
                        return true;

            // None found.
            return false;
        }

        //
        // Determines if the game is over (ie, no valid moves for either plaer.)
        //
        public bool IsTerminalState()
        {
            return (!HasAnyValidMove(Black) && !HasAnyValidMove(White));
        }

        //
        // Determines if a specific move is valid for the player.
        //
        public bool IsValidMove(int color, int row, int col)
        {
            // The square must be empty.
            if (this.squares[row, col] != Board.Empty)
                return false;

            // Must be able to flip at least one opponent disc.
            int dr, dc;
            for (dr = -1; dr <= 1; dr++)
                for (dc = -1; dc <= 1; dc++)
                    if (!(dr == 0 && dc == 0) && this.IsOutflanking(color, row, col, dr, dc))
                        return true;

            // No opponents could be flipped.
            return false;
        }

        //
        // Returns the number of valid moves a player can make on the board.
        //
        public int GetValidMoveCount(int color)
        {
            int n = 0;

            // Check all board positions.
            int i, j;
            for (i = 0; i < Height; i++)
                for (j = 0; j < Width; j++)
                    // If the move is valid for the color, bump the count.
                    if (this.IsValidMove(color, i, j))
                        n++;
            return n;
        }

        //
        // Given a player move and a specific direction, determines if any
        // opponent discs will be outflanked.
        // Note: For performance reasons the direction values are NOT checked
        // for validity (dr and dc may be one of -1, 0 or 1 but both should
        // not be zero).
        //
        private bool IsOutflanking(int color, int row, int col, int dr, int dc)
        {
            // Move in the given direction as long as we stay on the board and
            // land on a disc of the opposite color.
            int r = row + dr;
            int c = col + dc;
            while (r >= 0 && r < Height && c >= 0 && c < Width && this.squares[r, c] == -color)
            {
                r += dr;
                c += dc;
            }

            // If we ran off the board, only moved one space or didn't land on
            // a disc of the same color, return false.
            if (r < 0 || r > (Height - 1) || c < 0 || c > (Width - 1) || (r - dr == row && c - dc == col) || this.squares[r, c] != color)
                return false;

            // Otherwise, return true;
            return true;
        }

        //
        // Updates the board counts and safe disc map.
        // Note: MUST be called after any changes to the board contents.
        //
        private void UpdateCounts()
        {
            // Reset all counts.
            this.blackCount = 0;
            this.whiteCount = 0;
            this.emptyCount = 0;

            int i, j;

            // Tally the counts.
            for (i = 0; i < Height; i++)
                for (j = 0; j < Width; j++)
                {
                    // Update the counts.
                    if (this.squares[i, j] == Board.Black)
                    {
                        this.blackCount++;
                    }
                    else if (this.squares[i, j] == Board.White)
                    {
                        this.whiteCount++;
                    }
                    else
                        this.emptyCount++;
                }
        }
    }
}