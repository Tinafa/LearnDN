using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XUtliPoolLib;
using UnityEngine;

namespace XMainClient
{
    class XCommon : XSingleton<XCommon>
    {
        public XCommon()
        {
            _idx = 5;
        }
        private int _idx = 0;

        private System.Random _random = new System.Random(DateTime.Now.Millisecond);

        private int _new_id = 0;

        public int New_id
        {
            get { return ++_new_id; }
        }

        public long UniqueToken
        {
            get { return DateTime.Now.Ticks + New_id; }
        }

        public StringBuilder sb = new StringBuilder();

        public uint XHash(string str)
        {
            if (str == null) return 0;

            uint hash = 0;
            for (int i = 0; i < str.Length; i++)
            {
                hash = (hash << _idx) + hash + str[i];
            }

            return hash;
        }

        public uint XHash(StringBuilder str)
        {
            if (str == null) return 0;

            uint hash = 0;
            for (int i = 0; i < str.Length; i++)
            {
                hash = (hash << _idx) + hash + str[i];
            }

            return hash;
        }

        public uint XHashLowerRelpaceDot(uint hash, string str)
        {
            if (str == null) return hash;

            for (int i = 0; i < str.Length; i++)
            {
                char c = char.ToLower(str[i]);
                if (c == '/' || c == '\\')
                    c = '.';
                hash = (hash << _idx) + hash + c;
            }

            return hash;
        }
        public uint XHash(uint hash, string str)
        {
            if (str == null) return hash;

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                hash = (hash << _idx) + hash + c;
            }

            return hash;
        }

        /*
         * check whether the specific circle is intersected by the line.
         * the first intersection point from begin is returned by value t,
         * which means the distance from begin-point.
         */
        public bool Intersection(Vector2 begin, Vector2 end, Vector2 center, float radius, out float t)
        {
            t = 0;

            float sqrtR = radius * radius;

            Vector2 a = center - begin;
            float cdist = a.sqrMagnitude;

            if (cdist < sqrtR)
            {
                //inside circle
                return true;
            }
            else
            {
                Vector2 d = end - begin;

                if (d.sqrMagnitude > 0)
                {
                    float project = Mathf.Sqrt(cdist) * Mathf.Cos(Vector2.Angle(a, d));

                    if (project >= 0)
                    {
                        float m = cdist - project * project;
                        if (m < sqrtR)
                        {
                            float q = sqrtR - m;
                            t = project - Mathf.Sqrt(q);

                            return true;
                        }
                    }
                }
            }

            return false;
        }


        private float CrossProduct(float x1, float z1, float x2, float z2)
        {
            return x1 * z2 - x2 * z1;
        }

        public bool IsLineSegmentCross(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2)
        {
            //fast detect
            if (Mathf.Min(p1.x, p2.x) <= Mathf.Max(q1.x, q2.x) &&
                Mathf.Min(q1.x, q2.x) <= Mathf.Max(p1.x, p2.x) &&
                Mathf.Min(p1.z, p2.z) <= Mathf.Max(q1.z, q2.z) &&
                Mathf.Min(q1.z, q2.z) <= Mathf.Max(p1.z, p2.z))
            {
                //( p1 - q1 ) * ( q2 - q1 )
                float p1xq = CrossProduct(p1.x - q1.x, p1.z - q1.z,
                                           q2.x - q1.x, q2.z - q1.z);
                //( p2 - q1 ) * ( q2 - q1 )
                float p2xq = CrossProduct(p2.x - q1.x, p2.z - q1.z,
                                           q2.x - q1.x, q2.z - q1.z);

                //( q1 - p1 ) * ( p2 - p1 )
                float q1xp = CrossProduct(q1.x - p1.x, q1.z - p1.z,
                                           p2.x - p1.x, p2.z - p1.z);
                //( q2 - p1 ) * ( p2 - p1 )
                float q2xp = CrossProduct(q2.x - p1.x, q2.z - p1.z,
                                           p2.x - p1.x, p2.z - p1.z);

                return ((p1xq * p2xq <= 0) && (q1xp * q2xp <= 0));
            }


            Vector3 p1x = Vector3.Project(p1, Vector3.up);

            return false;
        }
    }
}
