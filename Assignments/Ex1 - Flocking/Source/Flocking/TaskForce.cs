using System.Collections.Generic;
using System.Drawing;

using AI.SteeringBehaviors.Core;
using AI.SteeringBehaviors.StudentAI;

namespace AI.SteeringBehaviors
{
    public class TaskForce : Flock, ITaskForce
    {
        #region Properties
        public Color Color { get; set; }
        #endregion

        #region Constructors
        public TaskForce()
        {
            AlignmentStrength = CohesionStrength = SeparationStrength = 1.0f;
            FlockRadius = 50.0f;

            Color = ShipFactory.Instance.GetRandomColor();
            Boids = new List<MovingObject>();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Adds a new randomized ship of type MovingObject.
        /// </summary>
        public void AddNewShip(Vector3 position)
        {
            Boids.Add(ShipFactory.Instance.GetAShipAroundLocation(position));
        }

        /// <summary>
        /// Remove a ship from this task force.
        /// </summary>
        public void TrimShip()
        {
            if (0 != Boids.Count)
            {
                ShipFactory.Instance.ReturnAShip(Boids[Boids.Count - 1]);
                Boids.RemoveAt(Boids.Count - 1);
            }
        }


        /// <summary>
        /// Sets the number of ships in this task force to the given amount.
        /// New ships will spawn at around the average position.
        /// </summary>
        /// <param name="Count">The desired number of ships.</param>
        public void SetShipAmount(int Count)
        {
            while (Count > Boids.Count)
                AddNewShip(AveragePosition);
            while (Count < Boids.Count)
                TrimShip();
        }

        /// <summary>
        /// Searches this task force for an instance of the given ship.
        /// </summary>
        /// <param name="ship">The ship to look for</param>
        /// <returns>Returns true if the given ship is in this task force.</returns>
        public bool ContainsShip(MovingObject ship)
        {
            return Boids.Contains(ship);
        }

        /// <summary>
        /// Remove all ships from this task force.
        /// </summary>
        public void RemoveAllShips()
        {
            foreach (MovingObject ship in Boids)
            {
                ShipFactory.Instance.ReturnAShip(ship);
            }
            Boids.Clear();
        }

        /// <summary>
        /// Update all ships' positions in this task force
        /// </summary>
        /// <param name="deltaTime">The </param>
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        /// <summary>
        /// Make this task force a copy of another.
        /// </summary>
        /// <param name="copy">The other task force.</param>
        /// <remarks>Does not deep copy the list of ships.</remarks>
        public void CopyFrom(ITaskForce copy)
        {
            Boids = copy.Boids;
            AlignmentStrength = copy.AlignmentStrength;
            CohesionStrength = copy.CohesionStrength;
            SeparationStrength = copy.SeparationStrength;
            Color = copy.Color;
        }

        public override string ToString()
        {
            return "Task Force of " + Boids.Count + " ships. Color {A:" + Color.A.ToString() + " R:" + Color.R.ToString() +
                " G:" + Color.G.ToString() + " B:" + Color.B.ToString() + "}";
        }

        #endregion
    }
}
