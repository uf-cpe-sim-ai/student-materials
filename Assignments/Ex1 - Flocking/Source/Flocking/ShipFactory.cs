using System;
using System.Collections.Generic;
using System.Drawing;
using AI.SteeringBehaviors.Core;

namespace AI.SteeringBehaviors
{
    /// <summary>
    /// Class used to limit garbage collection.
    /// </summary>
    public class ShipFactory
    {
        #region Fields

        static ShipFactory instance = new ShipFactory();

        Queue<MovingObject> inventory;
        Random rng;

        #endregion

        #region Properties
        public static ShipFactory Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region Constructors

        ShipFactory()
        {
            inventory = new Queue<MovingObject>(100);
            rng = new Random();
        }

        #endregion

        #region Methods

        public MovingObject GetAShipAroundLocation(Vector3 position)
        {
            MovingObject ship = null;
            if (0 == inventory.Count)
            {
                ship = new MovingObject();
            }
            else
            {
                ship = inventory.Dequeue();
            }
            double heading = rng.NextDouble() * Math.PI * 2;
            Vector3 velocity = Vector3.Empty;
            velocity.X = Convert.ToSingle(Math.Cos(heading));
            velocity.Y = Convert.ToSingle(Math.Sin(heading));
            ship.Velocity = velocity * Convert.ToSingle(
                (rng.NextDouble() * ship.MaxSpeed / 2 +
                ship.MaxSpeed / 4));
            ship.Position = position;
            ship.Update(0.04f);
            ship.CollisionRadius = 8;
            return ship;
        }

        public void ReturnAShip(MovingObject ship)
        {
            if(null != ship)
                inventory.Enqueue(ship);
        }

        public Color GetRandomColor()
        {
            return Color.FromArgb(rng.Next(128) + 127, rng.Next(128) + 127, rng.Next(128) + 127);
        }

        #endregion
    }
}
