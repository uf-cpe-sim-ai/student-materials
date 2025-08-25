using System;

namespace AI.SteeringBehaviors.Core
{
    [Serializable]
    public struct Vector3
    {
        /// <summary>
        /// Retrieves or sets the x component of a 3-D vector.
        /// </summary>
        public float X;
        /// <summary>
        /// Retrieves or sets the y component of a 3-D vector.
        /// </summary>
        public float Y;
        /// <summary>
        /// Retrieves or sets the z component of a 3-D vector.
        /// </summary>
        public float Z;

        /// <summary>
        /// Initializes a new instance of the Vector3 class.
        /// </summary>
        /// <param name="valueX">Initial Vector3.X value.</param>
        /// <param name="valueY">Initial Vector3.Y value.</param>
        /// <param name="valueZ">Initial Vector3.Z value.</param>
        public Vector3(float valueX, float valueY, float valueZ)
        {
            X = valueX;
            Y = valueY;
            Z = valueZ;
        }

        /// <summary>
        /// Negates the vector.
        /// </summary>
        /// <param name="vec">Source Vector3 structure.</param>
        /// <returns>The Vector3 structure that is the result of the operation.</returns>
        public static Vector3 operator -(Vector3 vec)
        {
            return new Vector3(-vec.X, -vec.Y, -vec.Z);
        }

        /// <summary>
        /// Subtracts two 3-D vectors.
        /// </summary>
        /// <param name="left">The Vector3 structure to the left of the subtraction operator.</param>
        /// <param name="right">The Vector3 structure to the right of the subtraction operator.</param>
        /// <returns>Resulting Vector3 structure.</returns>
        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - left.Z);
        }

        /// <summary>
        /// Compares the current instance of a class to another instance to determine
        /// whether they are different.
        /// </summary>
        /// <param name="left">The Vector3 structure to the left of the inequality operator.</param>
        /// <param name="right">The Vector3 structure to the right of the inequality operator.</param>
        /// <returns>
        /// Value that is true if the objects are different, or false if they are the
        /// same.
        /// </returns>
        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines the product of a single value and a 3-D vector.
        /// </summary>
        /// <param name="right">Source System.Single structure.</param>
        /// <param name="left">Source Vector3 structure.</param>
        /// <returns>
        /// A Vector3 structure that is the product of the left
        /// and right parameters.
        /// </returns>
        public static Vector3 operator *(float right, Vector3 left)
        {
            return new Vector3(left.X * right, left.Y * right, left.Z * right);
        }
        
        /// <summary>
        /// Determines the product of a single value and a 3-D vector.
        /// </summary>
        /// <param name="left">Source Vector3 structure.</param>
        /// <param name="right">Source System.Single structure.</param>
        /// <returns>
        /// A Vector3 structure that is the product of the left
        /// and right parameters.
        /// </returns>
        public static Vector3 operator *(Vector3 left, float right)
        {
            return new Vector3(left.X *right, left.Y * right, left.Z * right);
        }

        /// <summary>
        /// Determines the product of the reciprical of a single value and a 3-D vector.
        /// </summary>
        /// <param name="left">Source Vector3 structure.</param>
        /// <param name="right">Source System.Single structure.</param>
        /// <returns>
        /// A Vector3 structure that is the product of the left
        /// and the reciprocal of the right parameters.
        /// </returns>
        public static Vector3 operator /(Vector3 left, float right)
        {
            return left * (1/right);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="left">Source Vector3 structure.</param>
        /// <param name="right">Source Vector3 structure.</param>
        /// <returns>A Vector3 structure that contains the sum of the parameters.</returns>
        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            return new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        /// <summary>
        /// Compares the current instance of a class to another instance to determine
        /// whether they are the same.
        /// </summary>
        /// <param name="left">The Vector3 structure to the left of the equality operator.</param>
        /// <param name="right">The Vector3 structure to the right of the equality operator.</param>
        /// <returns>Value that is true if the objects are the same, or false if they are different.</returns>
        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        /// <summary>
        /// Determine if this object equals another object.
        /// </summary>
        /// <param name="obj">Object to compare against.</param>
        /// <returns>True if the values are the same.</returns>
        public override bool Equals(object obj)
        {
            return obj is Vector3 && this == (Vector3)obj;
        }

        /// <summary>
        /// Determine if this 3-D vector equals another 3-D vector.
        /// </summary>
        /// <param name="vec">Vector to compare against.</param>
        /// <returns>True if the values are the same.</returns>
        public bool Equals(Vector3 vec)
        {
            return this == vec;
        }

        /// <summary>
        /// Retrieves a hash code for this object.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return new {X, Y, Z}.GetHashCode();
        }

        /// <summary>
        /// Retrieves an origin vector.
        /// </summary>
        public static Vector3 Empty { get{return new Vector3(0, 0, 0);} }

        /// <summary>
        /// Retrieves an origin vector.
        /// </summary>
        public static Vector3 Zero { get { return Empty; } }

        /// <summary>
        /// Returns the square of the length of this vector.
        /// </summary>
        public float LengthSquared { get { return X * X + Y * Y + Z * Z; } }

        /// <summary>
        /// Returns the length of this vector.
        /// </summary>
        public float Length { get { return Convert.ToSingle(Math.Sqrt(LengthSquared)); } }

        /// <summary>
        /// Normalizes this 3-D vector.
        /// </summary>
        public void Normalize()
        {
            if(!float.IsNaN(Length) && Length != 0)
                this = this / Length;
        }

        /// <summary>
        /// Returns the normalized version of a 3-D vector.
        /// </summary>
        /// <param name="vec">Source Vector3 structure.</param>
        /// <returns>A Vector3 structure that contains the normalized values of the parameter.</returns>
        public static Vector3 Normalize(Vector3 vec)
        {
            return vec / vec.Length;
        }

    }
}
