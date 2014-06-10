using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Catsland.Core {
    public class CatMath {

        // from: http://www.cnblogs.com/graphics/archive/2010/08/09/1795348.html

        // put it here for a moment
        // Determine whether a ray intersect with a triangle
        // Parameters
        // orig: origin of the ray
        // dir: direction of the ray
        // v0, v1, v2: vertices of triangle
        // t(out): weight of the intersection for the ray
        // u(out), v(out): barycentric coordinate of intersection

        public static bool IntersectTriangle(Vector3 orig, Vector3 dir,
            Vector3 v0, Vector3 v1, Vector3 v2,
            out float t, out float u, out float v) {
            t = 0.0f;
            u = 0.0f;
            v = 0.0f;
            // E1
            Vector3 E1 = v1 - v0;

            // E2
            Vector3 E2 = v2 - v0;

            // P
            Vector3 P = Vector3.Cross(dir, E2);

            // determinant
            float det = Vector3.Dot(E1, P);

            // keep det > 0, modify T accordingly
            Vector3 T;
            if (det > 0) {
                T = orig - v0;
            }
            else {
                T = v0 - orig;
                det = -det;
            }

            // If determinant is near zero, ray lies in plane of triangle
            if (det < 0.0001f)
                return false;

            // Calculate u and make sure u <= 1
            u = Vector3.Dot(T, P);
            if (u < 0.0f || u > det)
                return false;

            // Q
            Vector3 Q = Vector3.Cross(T, E1);

            // Calculate v and make sure u + v <= 1
            v = Vector3.Dot(dir, Q);
            if (v < 0.0f || u + v > det)
                return false;

            // Calculate t, scale parameters, ray intersects triangle
            t = Vector3.Dot(E2, Q);

            float fInvDet = 1.0f / det;
            t *= fInvDet;
            u *= fInvDet;
            v *= fInvDet;

            return true;
        }

        //　Returns　Euler　angles　that　point　from　one　point　to　another　
        public static Vector3 AngleTo(Vector3 from, Vector3 location) {
            Vector3 angle = new Vector3();
            Vector3 v3 = Vector3.Normalize(location - from);

            angle.X = (float)Math.Asin(v3.Y);
            angle.Y = (float)Math.Atan2((double)-v3.X, (double)-v3.Z);

            return angle;
        }

        //　Converts　a　Quaternion　to　Euler　angles　(X　=　Yaw,　Y　=　Pitch,　Z　=　Roll)　
        public static Vector3 QuaternionToEulerAngleVector3(Quaternion rotation) {
            Vector3 rotationaxes = new Vector3();
            Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);

            rotationaxes = AngleTo(new Vector3(), forward);

            if (rotationaxes.X == MathHelper.PiOver2) {
                rotationaxes.Y = (float)Math.Atan2((double)up.X, (double)up.Z);
                rotationaxes.Z = 0;
            }
            else if (rotationaxes.X == -MathHelper.PiOver2) {
                rotationaxes.Y = (float)Math.Atan2((double)-up.X, (double)-up.Z);
                rotationaxes.Z = 0;
            }
            else {
                up = Vector3.Transform(up, Matrix.CreateRotationY(-rotationaxes.Y));
                up = Vector3.Transform(up, Matrix.CreateRotationX(-rotationaxes.X));

                rotationaxes.Z = (float)Math.Atan2((double)-up.Z, (double)up.Y);
            }

            return rotationaxes;
        }

        //　Converts　a　Rotation　Matrix　to　a　quaternion,　then　into　a　Vector3　containing　
        //　Euler　angles　(X:　Pitch,　Y:　Yaw,　Z:　Roll)　
        public static Vector3 MatrixToEulerAngleVector3(Matrix Rotation) {
            Vector3 translation, scale;
            Quaternion rotation;

            Rotation.Decompose(out　scale, out　rotation, out　translation);

            Vector3 eulerVec = QuaternionToEulerAngleVector3(rotation);

            return eulerVec;
        }



    }
}
