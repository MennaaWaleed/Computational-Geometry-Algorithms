using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
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

            Point mn = new Point(Int32.MaxValue, Int32.MaxValue);

            //pick the minimum point in the y axis (if there are more than one, the minimum in the x axis) and the start point of the convex 
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Y < mn.Y || (points[i].Y == mn.Y && points[i].X < mn.X))
                {
                    mn = points[i];
                }
            }
            List<Point> temp = new List<Point>();
            foreach (Point p in points)
            {
                if (!temp.Contains(p))
                {
                    temp.Add(p);
                }
            }
            points = temp;
            points.Remove(mn);
            //Sort 
            points = points.OrderBy(p => CalcAngle(mn, p)).ToList();

            Stack<Point> s = new Stack<Point>();
            s.Push(mn);
            s.Push(points[0]);

            points.Add(mn);
            for (int i = 1; i < points.Count; i++)
            {
                if (s.Count < 2)
                {
                    s.Push(points[i]);
                    continue;
                }

                Point a = s.Peek();
                s.Pop();
                Point b = s.Peek();
                s.Pop();
                Line line = new Line(b, a);
                while (HelperMethods.CheckTurn(line, points[i]) != Enums.TurnType.Left)
                {
                    if (s.Count <= 0)
                    {
                        break;
                    }
                    a = b;
                    b = s.Peek();
                    s.Pop();
                    line = new Line(b, a);
                }
                s.Push(b);
                s.Push(a);
                s.Push(points[i]);
            }
            s.Pop();
            while (s.Count > 0)
            {
                outPoints.Add(s.Peek());
                s.Pop();
            }
        }
        double CalcAngle(Point pivot, Point p)
        {
            return Math.Atan2(p.Y - pivot.Y, p.X - pivot.X);
        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
