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

            outLines.Clear();
            outPolygons.Clear();
            outPoints.Clear();


            if (points == null)          //O(nlogn)
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

            //pick the min point in y axis
            int n = points.Count;
            for (int i = 0; i < n; i++)
            {
                if (points[i].Y < mn.Y || (points[i].Y == mn.Y && points[i].X < mn.X))
                {
                    mn = points[i];
                }
            }
            //handle duplicates
            List<Point> temp = new List<Point>();
            for(int i = 0;i<n;++i)
            {
                if (!temp.Contains(points[i]))
                {
                    temp.Add(points[i]);
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

            n= points.Count;
            for (int i = 1; i < n; i++)
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
        double CalcAngle(Point N, Point p)
        {
            return Math.Atan2(p.Y - N.Y, p.X - N.X);
        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
