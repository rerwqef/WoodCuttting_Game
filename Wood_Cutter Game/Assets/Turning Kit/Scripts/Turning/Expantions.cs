using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class Expantions
{
    public static float Eps = 0.000001f;

    // Rounding points
    public static float Round(float f)
    {
        return Mathf.RoundToInt(f / Eps) * Eps;
    }
    public static Vector2 Round(Vector2 f)
    {
        f.x = Round(f.x);
        f.y = Round(f.y);
        return f;
    }

    // Clockwise bypass
    public static bool HasСlockwise(List<Vector2> con)
    {
        bool _b = false;

        if (con.Count > 0)
        {
            Vector2 min = con[0];
            int id_min = 0;

            int i = 0;
            foreach (Vector2 v2 in con)
            {
                if (v2.x < min.x)
                {
                    min = v2;
                    id_min = i;
                }
                i++;
            }

            int id_prev = (id_min - 1 < 0) ? con.Count - 1 : id_min - 1;
            int id_next = (id_min + 1 >= con.Count) ? 0 : id_min + 1;

            if ((con[id_prev].x - con[id_min].x) * (con[id_next].y - con[id_min].y)
                - (con[id_prev].y - con[id_min].y) * (con[id_next].x - con[id_min].x) > 0)
                _b = true;
        }

        return _b;
    }

    // Check if the points lie on the segment
    public static bool HasPointLies(Vector2 point_start, Vector2 point_end, Vector2 point)
    {
        return Mathf.Abs(Vector2.Distance(point_start, point_end) - Vector2.Distance(point_start, point)
            - Vector2.Distance(point_end, point)) <= Eps;
    }

    // Line intersection point
    public static Vector3 getPointOfIntersection(Vector2 _p1, Vector2 _p2, Vector2 _p3, Vector2 _p4)
    {
        Vector2 p1 = Round(_p1);
        Vector2 p2 = Round(_p2);

        Vector2 p3 = Round(_p3);
        Vector2 p4 = Round(_p4);

        Vector2 pos = Vector2.zero;

        float denominator = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);
        if (denominator == 0)
            // lines are parallel
            // If both the numerator and denominator are equal to zero, then the lines coincide.
            return new Vector3(0, 0, -1);
        else
        {
            float u12 = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denominator;
            float u34 = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denominator;
            pos = p1 + (p2 - p1) * u12;

            // If u12 and u34 are on the interval [0,1], then the segments have an intersection point
            if (u12 < 0 || u12 > 1)
                //the intersection point is not on the segment p1, p2
                return new Vector3(0, 0, -1);
            if (u34 < 0 || u34 > 1)
                //the intersection point is not on the segment p3, p4
                return new Vector3(0, 0, -1);
        }
        return pos;
    }

    // Check if the points are equal
    public static bool Equals(Vector2 A, Vector2 B)
    {
        return Vector2.Distance(A, B) <= Eps;
    }
    public static bool Equals(float A, float B)
    {
        return Mathf.Abs(A - B) <= Eps;
    }

    // If the point is inside the contour
    public static bool HasIntersections(Vector2 point, List<Vector2> contour, bool isCross = false)
    {
        bool hasIntersections = true;
        int intersections_num;
        bool cur_under;
        bool prev_under;
        Vector2 a;
        Vector2 b;
        float t;

        if (contour.Count > 0)
        {
            int i_prev = contour.Count - 1;
            intersections_num = 0;
            prev_under = contour[i_prev].y < point.y;
            for (int i = 0; i < contour.Count; i++)
            {
                cur_under = contour[i].y < point.y;

                a = contour[i_prev] - point;
                b = contour[i] - point;

                t = (a.x * (b.y - a.y) - a.y * (b.x - a.x));
                if (cur_under && !prev_under && t >= 0)
                    intersections_num += 1;
                if (!cur_under && prev_under && t <= 0)
                    intersections_num += 1;

                if (Equals(point, contour[i_prev]) || Equals(point, contour[i]) || HasPointLies(contour[i_prev], contour[i], point))
                    return isCross;

                i_prev = i;
                prev_under = cur_under;
            }

            if ((intersections_num & 1) == 0)
                hasIntersections = false;
        }
        else
            hasIntersections = false;

        return hasIntersections;
    }

    // If the segments intersect, then returns the intersection point
    public static bool GetCross(Vector2 pR0, Vector2 pR1, Vector2 pB0, Vector2 pB1, ref Vector2 crossP)
    {
        bool HasCross = false;

        if (!Equals(pR0, pR1) && !Equals(pB0, pB1))
        {
            Vector3 crossPoint = getPointOfIntersection(pR0, pR1, pB0, pB1);

            if (crossPoint.z != 0)
            {
                if (HasPointLies(pR0, pR1, pB0))
                    crossPoint = pB0;
                else if (HasPointLies(pR0, pR1, pB1))
                    crossPoint = pB1;
            }

            if (crossPoint.z != 0)
            {
                if (HasPointLies(pB0, pB1, pR0))
                    crossPoint = pR0;
                else if (HasPointLies(pB0, pB1, pR1))
                    crossPoint = pR1;
            }

            if (crossPoint.z != -1)// && !Equals(pR1, crossPoint) && !Equals(pB1, crossPoint))
            {
                crossPoint.z = 0;

                HasCross = true;
                crossP = crossPoint;
            }
        }
        return HasCross;
    }

    // find the point in the path and return its index
    public static int GetIndex(Vector2 point, List<Vector2> contour)
    {
        int index = -1;
        for (int i = 0; i < contour.Count; i++)
        {
            if (Equals(point, contour[i]))
            {
                index = i;
                break;
            }
        }
        return index;
    }
}
