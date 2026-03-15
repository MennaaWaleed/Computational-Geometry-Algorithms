using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if(points == null)
                return;
            if(points.Count == 0)
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
            if(points.Count == 3)
            {
                outPoints = new List<Point>();
                outPoints.Add(points[0]);
                outPoints.Add(points[1]);
                outPoints.Add(points[2]);
                return;
            }


            Point s = new Point(Int32.MaxValue, Int32.MaxValue);
            Point mn=new Point(Int32.MaxValue, Int32.MaxValue);

            //pick the minimum point in the y axis (if there are more than one, the minimum in the x axis) and the start point of the convex 
            int idx =-1;
            for(int i = 0; i < points.Count; i++)
            {
                if (points[i].Y < s.Y || (points[i].Y == s.Y && points[i].X < s.X))
                {
                    s = points[i];
                    idx= i;
                }
            }


            outPoints = new List<Point>();
            outPoints.Add(s);

            //the first point in the convex
            mn = s;
            
            List<Point> randList = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                if(i!=idx)
                    randList.Add(points[i]);
            }

            Point random;
            


            while (true)
            {
                
                s = outPoints.Last();
                if(randList.Count == 0)
                {
                    return;
                }
                random =randList[0];
                if(mn==random)
                {
                    random = randList[1];
                }
                for (int i = 0; i < points.Count; i++)
                {
                    var turn = HelperMethods.CheckTurn(s.Vector(random), s.Vector(points[i]));
                    if (turn == Enums.TurnType.Right)
                    {
                        random = points[i];
                    }
                    else if(turn == Enums.TurnType.Colinear)
                    {
                        if (HelperMethods.PointOnSegment(points[i], s, random))
                        {
                            continue;
                        }
                        else if(HelperMethods.PointOnSegment(random, s, points[i]))
                        {
                            random = points[i];
                        }
                    }

                }
                outPoints.Add(random);
                randList.Remove(random);
                if (outPoints.Last() == mn) break;
            }
            outPoints.Remove(outPoints[outPoints.Count - 1]);

        }
        
        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
