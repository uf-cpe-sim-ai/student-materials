namespace AI.SteeringBehaviors.Core
{
    public class BaseObject
    {
        #region Properties

        public Vector3 Position { get; set; }
        public float CollisionRadius { get; set; }
         
        #endregion

        #region Constructors
        public BaseObject()
        {
            Position = Vector3.Empty;
            CollisionRadius = 0;
        }
        #endregion
    }
}
