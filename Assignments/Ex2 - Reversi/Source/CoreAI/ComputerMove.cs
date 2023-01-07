//
// Modified: 2011/04/26, Derek Bliss (Full Sail University)
// Modified: 2019/12/20, Jeremiah Blanchard (University of Florida)
namespace GameAI.GamePlaying.Core
{
    /// <summary>
    /// Class for holding a look ahead move and rank.
    /// </summary>
    public class ComputerMove
    {
        // Defines a structure for holding a look ahead move and rank.
        public int row;
        public int column;
        public int rank;

        public ComputerMove(int row, int column)
        {
            this.row = row;
            this.column = column;
            this.rank = 0;
        }
    }
}