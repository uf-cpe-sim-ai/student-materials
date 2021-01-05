using System;

namespace AI.SteeringBehaviors.Core
{
    public class MovingObject : BaseObject
    {
        #region Properties

        public float MaxSpeed { get; set; }
        public Vector3 Velocity { get; set; }
        public float SafeRadius { get; set; }
        
        public float Heading
        {
            get
            {
                float result = 0;
                if (Velocity.X == 0 && Velocity.Y == 0)
                    return result;
                Vector3 normalizedVector = Vector3.Normalize(Velocity);
                if (normalizedVector.Y < 0)
                    result = -Convert.ToSingle(Math.Acos(normalizedVector.X));
                else
                    result = Convert.ToSingle(Math.Acos(normalizedVector.X));
                if (Single.IsNaN(result) || Single.IsInfinity(result))
                    result = 0;
                return result;
            }
        }
        #endregion

        #region Constructors
        public MovingObject()
        {
            MaxSpeed = 200.0f;
            Velocity = Vector3.Empty;
            SafeRadius = 50.0f;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Updates this object's position based on change in time and velocity.
        /// </summary>
        /// <param name="deltaTime">seconds since the last Update call</param>
        public virtual void Update(float deltaTime)
        {
            if (Velocity.LengthSquared > MaxSpeed * MaxSpeed)
            {
                Velocity = Vector3.Normalize(Velocity);
                Velocity *= MaxSpeed;
            }
            Position += Velocity * deltaTime;
        }

        #endregion
    }
}
