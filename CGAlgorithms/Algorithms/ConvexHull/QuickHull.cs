using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons,
            ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            outLines.Clear();
            outPolygons.Clear();
            outPoints.Clear();


            if (points == null)
                return;
            if (points.Count == 0)
            {
                return;
            }
            if (points.Count == 1)
            {
                outPoints = new List<Point>();
                outPoints.Add(points[0]);
                return;
            }
            if (points.Count == 2)
            {
                outPoints = new List<Point>();
                outPoints.Add(points[0]);
                outPoints.Add(points[1]);
                return;
            }
            outPoints = new List<Point>();
            Point mn = points[0];
            Point mx = points[0];
            int X = points.Count;
            for (int i = 1; i < X; i++)
            {
                if (points[i].X < mn.X)
                    mn = points[i];

                if (points[i].X > mx.X)
                    mx = points[i];
            }

            List<Point> l = new List<Point>();
            List<Point> r = new List<Point>();

            Line line = new Line(mn, mx);

            int n = points.Count;
            for(int i = 0; i < n; i++)
            {
                var side = HelperMethods.CheckTurn(line, points[i]);
                if (side == Enums.TurnType.Left)
                    l.Add(points[i]);
                else if (side == Enums.TurnType.Right)
                    r.Add(points[i]);
            }
            
           
            List<Point> up = Hull(l, mn, mx);
            List<Point> down = Hull(r, mx, mn);
            outPoints.Add(mn);
            n = up.Count;
            for (int i = 0; i < n; i++)
            {
                outPoints.Add(up[i]);
            }
            outPoints.Add(mx);
            n = down.Count;
            for (int i = 0; i < n; i++)
            {
                outPoints.Add(down[i]);
            }

            List<Point> u = new List<Point>();
            n=outPoints.Count;
            int m = u.Count;
            for (int i = 0; i < n; i++)
            {
                bool found = false;

                for (int j = 0; j < m; j++)
                {
                    if (outPoints[i].Equals(u[j]))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    u.Add(outPoints[i]);
                }
            }
            outPoints = u;
        }

        private List<Point> Hull(List<Point> ps, Point a, Point b)
        {
            List<Point> result = new List<Point>();

            if (ps.Count == 0)
                return result;

            double mx = -1;
            Point temp = null;

            int n = ps.Count;
            for(int i = 0; i < n; i++)
            {
                
                Point ab = new Point(b.X - a.X, b.Y - a.Y);
                Point ap = new Point(ps[i].X - a.X, ps[i].Y - a.Y);
                double val = HelperMethods.CrossProduct(ab, ap);

                if (val > mx)
                {
                    mx = val;
                    temp = ps[i];
                }
            }

            List<Point> l1 = new List<Point>();
            List<Point> l2 = new List<Point>();

            Line d1 = new Line(a, temp);
            Line d2 = new Line(temp, b);

            n = ps.Count;
            for(int i = 0; i < n; i++)
            {
                if (ps[i] == temp) continue;

                if (HelperMethods.CheckTurn(d1, ps[i]) == Enums.TurnType.Left)
                    l1.Add(ps[i]);
                else if (HelperMethods.CheckTurn(d2, ps[i]) == Enums.TurnType.Left)
                    l2.Add(ps[i]);
            }

            var p1 = Hull(l1, a, temp);
            var p2 = Hull(l2, temp, b);

            n = p1.Count;
            for (int i = 0; i < n; i++)
            {
                result.Add(p1[i]);
            }
            result.Add(temp);
            n = p2.Count;
            for (int i = 0; i < n; i++)
            {
                result.Add(p2[i]);
            }
            return result;
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}