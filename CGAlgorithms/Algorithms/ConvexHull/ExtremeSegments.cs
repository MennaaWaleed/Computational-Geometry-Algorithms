using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points == null)
                return;
            if (points.Count == 0)
            {
                return;
            }
            outPoints = new List<Point>();
            if (points.Count == 1)
            {
                outPoints.Add(points[0]);
                return;
            }

            HashSet<Point> rm = new HashSet<Point>();

            for (int i = 0; i < points.Count; i++)
            {
                Point a = points[i];
                for (int j = i + 1; j < points.Count; j++)
                {

                    if (points[j].X == points[i].X && points[j].Y == points[i].Y) continue;

                    Point b = points[j];
                    //check if the line ab is extreme
                    int left = 0, right = 0, colinear = 0;
                    int numOfPoints = points.Count - 2;
                    for (int k = 0; k < points.Count; k++)
                    {

                        if (k == i || k == j) continue;
                        Point c = points[k];
                        if (c.X == a.X && c.Y == a.Y || c.X == b.X && c.Y == b.Y)
                        {

                            colinear++; continue;
                        }


                        var turn = HelperMethods.CheckTurn(a.Vector(b), a.Vector(c));
                        if (turn == Enums.TurnType.Left)
                        {
                            left++;
                        }
                        else if (turn == Enums.TurnType.Right)
                        {
                            right++;
                        }
                        else
                        {
                            colinear++;
                            if (HelperMethods.PointOnSegment(c, a, b))
                            {
                                rm.Add(c);
                            }
                            else if (HelperMethods.PointOnSegment(a, b, c))
                            {
                                rm.Add(a);
                            }
                            else if (HelperMethods.PointOnSegment(b, a, c))
                            {
                                rm.Add(b);
                            }
                        }
                    }
                    if (left + colinear == numOfPoints || right + colinear == numOfPoints)
                    {
                        if (!outPoints.Contains(a))
                            outPoints.Add(a);
                        if (!outPoints.Contains(b))
                            outPoints.Add(b);
                    }
                }
            }
            for (int i = 0; i < outPoints.Count; i++)
            {
                if (rm.Contains(outPoints[i]))
                {
                    outPoints.RemoveAt(i);
                    i--;
                }
            }

            return;
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}