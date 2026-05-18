using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            outLines.Clear();
            outPolygons.Clear();
            outPoints.Clear();


            if (points.Count < 3)
            {
                outPoints = new List<Point>(points);
                return;
            }

            for (int i = 0; i < points.Count - 1; i++)
            {
                int minIndex = i;

                for (int j = i + 1; j < points.Count; j++)
                {
                    if (points[j].X < points[minIndex].X ||
                       (points[j].X == points[minIndex].X && points[j].Y < points[minIndex].Y))
                    {
                        minIndex = j;
                    }
                }


                if (minIndex != i)
                {
                    Point temp = points[i];
                    points[i] = points[minIndex];
                    points[minIndex] = temp;
                }
            }
            int k = 0;
            while (k < points.Count - 1)
            {
                if (points[k].Equals(points[k + 1]))
                    points.RemoveAt(k);

                else
                    k++;

            }


            List<Point> hull = new List<Point>();
            hull.Add(points[0]);
            hull.Add(points[1]);
            hull.Add(points[2]);


            if (HelperMethods.CheckTurn(new Line(hull[0], hull[1]), hull[2]) == Enums.TurnType.Right)
            {
                var temp = hull[1];
                hull[1] = hull[2];
                hull[2] = temp;
            }


            for (int i = 3; i < points.Count; i++)
            {
                Point p = points[i];


                bool inside = true;
                for (int j = 0; j < hull.Count; j++)
                {
                    Point a = hull[j];
                    Point b = hull[(j + 1) % hull.Count];

                    if (HelperMethods.CheckTurn(new Line(a, b), p) == Enums.TurnType.Right)
                    {
                        inside = false;
                        break;
                    }
                }

                if (inside) continue;


                int left = -1, right = -1;

                for (int j = 0; j < hull.Count; j++)
                {
                    Point prev = hull[(j - 1 + hull.Count) % hull.Count];
                    Point curr = hull[j];
                    Point next = hull[(j + 1) % hull.Count];

                    var turn1 = HelperMethods.CheckTurn(new Line(prev, curr), p);
                    var turn2 = HelperMethods.CheckTurn(new Line(curr, next), p);


                    if ((turn1 != Enums.TurnType.Right && turn2 == Enums.TurnType.Right))
                        left = j;

                    if ((turn1 == Enums.TurnType.Right && turn2 != Enums.TurnType.Right))
                        right = j;
                }


                List<Point> newHull = new List<Point>();

                newHull.Add(hull[left]);
                newHull.Add(p);
                newHull.Add(hull[right]);

                int idx = (right + 1) % hull.Count;
                while (idx != left)
                {
                    newHull.Add(hull[idx]);
                    idx = (idx + 1) % hull.Count;
                }

                hull = newHull;
            }
            List<Point> cleanedHull = new List<Point>();

            int n = hull.Count;

            for (int i = 0; i < n; i++)
            {
                Point prev = hull[(i - 1 + n) % n];
                Point curr = hull[i];
                Point next = hull[(i + 1) % n];

                if (HelperMethods.CheckTurn(new Line(prev, curr), next) == Enums.TurnType.Colinear)
                {

                    if (HelperMethods.PointOnSegment(curr, prev, next))
                    {
                        continue;
                    }
                }

                cleanedHull.Add(curr);
            }

            hull = cleanedHull;
            for (int i = 0; i < hull.Count; i++)
            {
                if (!outPoints.Contains(hull[i]))
                    outPoints.Add(hull[i]);
            }

        }
        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}
