#region

using System;
using UnityEngine;

#endregion

public static class VectorHelper
{
    private static bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
    {
        if (q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) &&
            q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y))
            return true;

        return false;
    }

// To find orientation of ordered triplet (p, q, r). 
// The function returns following values 
// 0 --> p, q and r are colinear 
// 1 --> Clockwise 
// 2 --> Counterclockwise 
    private static int Orientation(Vector2 p, Vector2 q, Vector2 r)
    {
        // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ 
        // for details of below formula. 
        var val = (q.y - p.y) * (r.x - q.x) -
                  (q.x - p.x) * (r.y - q.y);

        if (val == 0f)
            return 0; // colinear 

        return val > 0 ? 1 : 2; // clock or counterclock wise 
    }

// The main function that returns true if line segment 'p1q1' 
// and 'p2q2' intersect. 
    private static bool DoIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
    {
        // Find the four orientations needed for general and 
        // special cases 
        var o1 = Orientation(p1, q1, p2);
        var o2 = Orientation(p1, q1, q2);
        var o3 = Orientation(p2, q2, p1);
        var o4 = Orientation(p2, q2, q1);

        // General case 
        if (o1 != o2 && o3 != o4)
            return true;

        // Special Cases 
        // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
        if (o1 == 0 && OnSegment(p1, p2, q1))
            return true;

        // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
        if (o2 == 0 && OnSegment(p1, q2, q1))
            return true;

        // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
        if (o3 == 0 && OnSegment(p2, p1, q2))
            return true;

        // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
        if (o4 == 0 && OnSegment(p2, q1, q2))
            return true;

        return false; // Doesn't fall in any of the above cases 
    }

// Driver code 
    public static bool CheckIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
    {
        return DoIntersect(p1, q1, p2, q2);
    }

    public static bool IsPointInPolygon(Vector3 p, Vector3[] polygon)
    {
        if (polygon == null || polygon.Length < 3)
            return false;

        double minX = polygon[0].x;
        double maxX = polygon[0].x;
        double minY = polygon[0].y;
        double maxY = polygon[0].y;
        for (var i = 1; i < polygon.Length; i++)
        {
            Vector2 q = polygon[i];
            minX = Math.Min(q.x, minX);
            maxX = Math.Max(q.x, maxX);
            minY = Math.Min(q.y, minY);
            maxY = Math.Max(q.y, maxY);
        }

        if (p.x < minX || p.x > maxX || p.y < minY || p.y > maxY)
            return false;

        var inside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            if (polygon[i].y > p.y != polygon[j].y > p.y &&
                p.x < (polygon[j].x - polygon[i].x) * (p.y - polygon[i].y) /
                (polygon[j].y - polygon[i].y) + polygon[i].x)
                inside = !inside;

        return inside;
    }

    public static bool IsLeft(this Vector3 current, Vector3 target)
    {
        var dot = current.x * -target.z + current.z * target.x;
        return dot < 0;
    }

    public static Vector3 RotateVector(this Vector3 target, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.up) * target;
    }

    public static float DistanceTo(this Vector3 self, Vector3 target)
    {
        return 0f;
    }
}