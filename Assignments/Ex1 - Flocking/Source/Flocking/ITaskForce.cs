using System.Collections.Generic;
using System.Drawing;

using AI.SteeringBehaviors.Core;

namespace AI.SteeringBehaviors
{
    public interface ITaskForce
    {
        Color Color { get; set; }
        float AlignmentStrength { get; set; }
        float CohesionStrength { get; set; }
        float SeparationStrength { get; set; }
        List<MovingObject> Boids { get; }
        Vector3 AveragePosition { get; set; }
        float FlockRadius { get; set; }

        void AddNewShip(Vector3 position);
        void TrimShip();
        void SetShipAmount(int Count);
        bool ContainsShip(MovingObject ship);
        void RemoveAllShips();
        void Update(float deltaTime);
        void CopyFrom(ITaskForce copy);
    }
}
