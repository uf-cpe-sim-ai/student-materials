// ===================================================================
// Game AI code.
// Note: These are executed in the worker thread.
// ===================================================================
//
// Modified: 2009/09/28, Jeremiah Blanchard (Full Sail)
// Modified: 2011/04/26, Derek Bliss (Full Sail University)
// Modified: 2019/12/20, Jeremiah Blanchard (University of Florida)
namespace GameAI.GamePlaying.Core
{
    public class Agent
    {
        // AI agent's behavior.
        public Behavior behavior { protected get; set; }

        // AI agent's knowledge.
        public int Color { get; set; }
        public int LookAheadDepth { get; set; }
        public ComputerMove Move { get; set; }

        // Defines the difficulty settings.
        public enum Difficulty
        {
            None         = 0,
            Beginner     = 0,
            Intermediate = 1,
            Advanced     = 3,
            Expert       = 5
        }

        //
        // Agent constructor
        //
        public Agent()
        {
            behavior = null;
            LookAheadDepth = 0;
            Move = null;
            Color = 0;
        }

        //
        // This function will determine a move based on the behavior supplied
        // to this agent. This function returns null on failure.
        //
        public ComputerMove DetermineMove(Board _board)
        {
            // Run the behavior to determine a move. On failure, returns null.
            Move = null;
            if (behavior != null)
                Move = behavior.Run(Color, _board, LookAheadDepth);

            if (Move != null && _board.IsValidMove(Color, Move.row, Move.column))
                return Move;

            throw new System.NotImplementedException("Behavior returned null or invalid move due to incorrect implementation");
        }
    }
}
