using Box2DX.Common;
using Microsoft.Xna.Framework;

namespace Game1
{
    public static class GameUtils
    {
        /// <summary>
        /// converts degrees to radians
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double ToRadians(double degrees)
        {
            return degrees * System.Math.PI / 180.0;
        }

        /// <summary>
        /// calculates the distance between 2 points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float DistanceFrom(Vec2 a, Vec2 b)
        {
            return (float)System.Math.Sqrt(System.Math.Pow(a.X - b.X, 2) + System.Math.Pow(a.Y - b.Y, 2));
        }

        /// <summary>
        /// converts the given vector distance into a rotation in degrees
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float Vec2ToRotation(Vec2 v)
        {
            var degrees = System.Math.Atan2(v.Y, v.X) * 180 / System.Math.PI;
            //TODO:  this amount should take into account the default rotation of the object
            degrees -= 270;
            /*if(v.Y >= 0 && v.X >= 0)
            {
                degrees += 90;
            }
            else if(v.Y >= 0 && v.X <= 0)
            {
                degrees += 180;
            }
            else if(v.Y <= 0 && v.X <= 0)
            {
                degrees += 270;
            }
            */

            return (float) degrees;
        }

        /// <summary>
        /// Takes an angle in degrees and converts to a x,y distance vector
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Vec2 RotationToVec2(float r)
        {
            //TODO:  is there a better way to do this?
            r = r % 360;

            if (r < 0)
            {
                r = 360 + r;
            }

            var rotation = System.Math.Abs(r);
            int xSign, ySign;
            double x, y;

            //break rotation down into right triangles 
            if (rotation >= 0 && rotation <= 90)
            {
                xSign = 1;
                ySign = -1;
                if (rotation + 45 > 90)
                {
                    x = System.Math.Cos(ToRadians(90 - rotation));
                    y = System.Math.Sin(ToRadians(90 - rotation));
                }
                else
                {
                    x = System.Math.Sin(ToRadians(rotation));
                    y = System.Math.Cos(ToRadians(rotation));
                }
            }
            else if (rotation > 90 && rotation <= 180)
            {
                xSign = 1;
                ySign = 1;
                if (rotation + 45 > 180)
                {
                    x = System.Math.Sin(ToRadians(180 - rotation));
                    y = System.Math.Cos(ToRadians(180 - rotation));
                }
                else
                {
                    x = System.Math.Cos(ToRadians(rotation - 90));
                    y = System.Math.Sin(ToRadians(rotation - 90));
                }
            }
            else if (rotation > 180 && rotation <= 270)
            {
                xSign = -1;
                ySign = 1;
                if (rotation + 45 > 270)
                {
                    x = System.Math.Cos(ToRadians(270 - rotation));
                    y = System.Math.Sin(ToRadians(270 - rotation));
                }
                else
                {
                    x = System.Math.Sin(ToRadians(rotation - 180));
                    y = System.Math.Cos(ToRadians(rotation - 180));
                }
            }
            else
            {
                xSign = -1;
                ySign = -1;
                if (rotation + 45 > 360)
                {
                    x = System.Math.Sin(ToRadians(360 - rotation));
                    y = System.Math.Cos(ToRadians(360 - rotation));
                }
                else
                {
                    x = System.Math.Cos(ToRadians(rotation - 270));
                    y = System.Math.Sin(ToRadians(rotation - 270));
                }
            }

            return new Vec2((float)x * xSign, (float)y * ySign);
        }

        /// <summary>
        /// converts the given physics vector to a graphics vector type
        /// </summary>
        /// <param name="gfxVector"></param>
        /// <returns></returns>
        public static Vec2 PhysicsVec(Vector2 gfxVector)
        {
            return new Vec2(((float)gfxVector.X) * (1.0f / GameData.Instance.PixelsPerMeter), ((float)gfxVector.Y) * (1.0f / GameData.Instance.PixelsPerMeter));
        }

        /// <summary>
        /// converts the given physics vector to a graphics vector type
        /// </summary>
        /// <param name="physicsVec"></param>
        /// <returns></returns>
        public static Vector2 GraphicsVec(Vec2 physicsVec)
        {
            return new Vector2()
            {
                X = physicsVec.X * GameData.Instance.PixelsPerMeter,
                Y = physicsVec.Y * GameData.Instance.PixelsPerMeter
            };
        }
    }
}
