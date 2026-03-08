using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
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
            if (points.Count == 2)
            {
                outPoints.Add(points[0]);
                outPoints.Add(points[1]);
                return;
            }


            HashSet<Point> rm = new HashSet<Point>();

            for (int i = 0; i < points.Count; ++i)
            {
                Point p = points[i];

                for (int j = 0; j < points.Count; ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    Point a = points[j];
                    for (int k = j + 1; k < points.Count; ++k)
                    {
                        if (i == k)
                        {
                            continue;
                        }
                        Point b = points[k];
                        for (int l = k + 1; l < points.Count; ++l)
                        {
                            if (i == l)
                            {
                                continue;
                            }
                            Point c = points[l];

                            if ((p.X == a.X && p.Y == a.Y) || (p.X == b.X && p.Y == b.Y) || (p.X == c.X && p.Y == c.Y))
                            {
                                continue;
                            }

                            var turn = HelperMethods.PointInTriangle(p, a, b, c);
                            if (turn == Enums.PointInPolygon.Inside)
                            {
                                rm.Add(p);
                                break;
                            }
                            else if (turn == Enums.PointInPolygon.OnEdge)
                            {
                                rm.Add(p);
                                break;
                            }

                        }
                    }
                }
            }


            for (int i = 0; i < points.Count; ++i)
            {
                if (rm.Contains(points[i]))
                {
                    points.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < points.Count; i++)
            {
                if (!outPoints.Contains(points[i]))
                    outPoints.Add(points[i]);
            }
            return;
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
