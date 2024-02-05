using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Contains methods for translating vectors and degrees and any additional math methods needed by this pack.
    /// </summary>
    public static class Math2D
    {
       /// <summary>
       /// Returns a random 2D rotation value between 0 and 360
       /// </summary>
        public static Quaternion Random2DRotation
        {
            get
            {
                return Quaternion.Euler(0, 0, Random.Range(0, 361));
            }
        }

        public static Vector3 Vector2ToVector3(Vector2 vector2)
        {
            return new Vector3(vector2.x, vector2.y, 0.0f);
        }

        public static Vector3 Vector2ToVector3(Vector2 vector2, float zValue)
        {
            return new Vector3(vector2.x, vector2.y, zValue);
        }

        /// <summary>
        /// Returns a vector 2 translated from a floating angle.<br></br>
        /// An angle of 0 would return a vector of (1,0)<br></br>
        /// An angle of 90 would return a vector of (0,1)<br></br>
        /// An angle of 180 would return a vector of (-1,0)<br></br>
        /// An angle of 270 would return a vector of (0,-1)<br></br>
        /// </summary>
        /// <param name="angle">The euler angle in degrees that you wish to translate.</param>
        /// <returns>The translated vector2.</returns>
        public static Vector2 DegreeToVector2(float angle)
        {
            angle *= Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        /// <summary>
        /// Returns a vector 3 translated from a floating angle and a Z value.<br></br>
        /// An angle of 0 would return a vector of (1,0)<br></br>
        /// An angle of 90 would return a vector of (0,1)<br></br>
        /// An angle of 180 would return a vector of (-1,0)<br></br>
        /// An angle of 270 would return a vector of (0,-1)<br></br>
        /// </summary>
        /// <param name="angle">The euler angle in degrees that you wish to translate.</param>
        /// <param name="zValue">The z value of the vector3</param>
        /// <returns>The translated vector3.</returns>
        public static Vector3 DegreeToVector3(float angle, float zValue)
        {
            angle *= Mathf.Deg2Rad;

            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), zValue);
        }

        /// <summary>
        /// Returns a vector 3 translated from a floating angle.<br></br>
        /// An angle of 0 would return a vector of (1,0)<br></br>
        /// An angle of 90 would return a vector of (0,1)<br></br>
        /// An angle of 180 would return a vector of (-1,0)<br></br>
        /// An angle of 270 would return a vector of (0,-1)<br></br>
        /// </summary>
        /// <param name="angle">The euler angle in degrees that you wish to translate.</param>
        /// <returns>The translated vector3.</returns>
        public static Vector3 DegreeToVector3(float angle)
        {
            return DegreeToVector3(angle, 0);
        }


        /// <summary>
        /// Translate a vector2 into an angle degree.<br></br>
        /// A vector of (1,0) returns a degree of 0
        /// A vector of (0,1) returns a degree of 90
        /// A vector of (-1,0) returns a degree of 180
        /// A vector of (0,-1) returns a degree of 270
        /// </summary>
        /// <param name="vector">The vector you wish to translate into a degree.</param>
        /// <returns>The translate angle in degrees.</returns>
        public static float VectorToDegree(Vector2 vector)
        {
            // Checks when vector Z component equals zero to avoid dividing by zero. 
            if(vector.x == 0.0f)
            {
                if(vector.y >= 0.0f)
                {
                    return 90f;
                }
                else
                {
                    return 270f;
                }
            }          
                
            float angle = Mathf.Rad2Deg * (Mathf.Atan(vector.y / vector.x));

            // If the angle is still in the first quarter then it is returned as it is. 
            if (vector.x > 0 && vector.y >= 0)
            {
                return angle;
            }              

            // If the angle is in the second or the third quarter returns the angle + 180.
            if (vector.x < 0)
            {
                return angle + 180f;
            }                

            // If the angle in the fourth quarter return the angle + 360. 
            return angle + 360f;
        }

        /// <summary>
        /// Translate a vector3 into an angle degree.<br></br>
        /// A vector of (1,0) returns a degree of 0
        /// A vector of (0,1) returns a degree of 90
        /// A vector of (-1,0) returns a degree of 180
        /// A vector of (0,-1) returns a degree of 270
        /// </summary>
        /// <param name="vector">The vector you wish to translate into a degree.</param>
        /// <returns>The translate angle in degrees.</returns>
        public static float VectorToDegree(Vector3 vector)
        {
            return VectorToDegree(new Vector2(vector.x, vector.y));
        }

        /// <summary>
        /// An exponential lerp function which can be used to accelerate reaching the max value 
        /// via t instead of the standard Mathf linear lerp.
        /// </summary>
        /// <param name="min">The start value.</param>
        /// <param name="max">The end value.</param>
        /// <param name="t">The interpolation value between 2 floats.</param>
        /// <returns></returns>
        public static float LerpExpo(float min, float max, float t)
        {
            if (t >= 1.0f)
            {
                t = 1.0f;
            }                
            else if (t <= 0.0f)
            {
                t = 0.0f;
            }              

            float difference = max - min;

            return min + difference * t * t;
        }

        /// <summary>
        /// Provided a minimum and maximum value boundries and a value in between them, this value is 
        /// remaped into a value between 0 and 1, 1 being if the value equals the max value and 0 if 
        /// it equals the min value.
        /// </summary>
        /// <param name="value">The value you wish to remap into a value between (0 to 1)</param>
        /// <param name="minValue">The minimum value boundry.</param>
        /// <param name="maxValue">The maximum value boundry.</param>
        /// <returns>The remapped value.</returns>
        public static float Remap01(float value, float minValue, float maxValue)
        {
            if (value <= minValue)
            {
                return minValue;
            }
            if (value >= maxValue)
            {
                return maxValue;
            }

            return (value - minValue) / (maxValue - minValue);
        }
    }
}