
using CGUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class SubtractingEars : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            List<Point> verticess = new List<Point>(points);

            if (lines.Count > 0)
            {
                List<Line> ordered = new List<Line>();
                ordered.Add(lines[0]);
                Line l1 = ordered[ordered.Count - 1];
                lines .Remove(l1);
                while (lines.Count > 0)
                {

                    //Line next = lines.Find(l => l.Start.Equals(l1.End));));
                    Line next = lines.Find(l => Math.Sqrt(Math.Pow(l.Start.X - l1.End.X, 2) + Math.Pow(l.Start.Y - l1.End.Y, 2)) < 5);
                    if (next == null)
                    {
                        next = lines.Find(l => Math.Sqrt(Math.Pow(l.End.X - l1.End.X, 2) + Math.Pow(l.End.Y - l1.End.Y, 2)) < 5);
                        if (next != null)
                        {
                            Point ps = next.Start;
                            Point pe = next.End;
                            next.Start = pe;
                            next.End = ps;
                        }


                    }
                    ordered.Add(next);
                    l1 = next;
                    lines.Remove(next);
                }


                verticess.Clear();
                foreach (var line in ordered)
                {
                    verticess.Add(line.Start);
                }
            }
            if (polygons.Count > 0)
            {
                verticess.Clear();
                foreach (var line in polygons[0].lines)
                {
                    verticess.Add(line.Start);
                }
            }
            if (points.Count > 0)
            {
                verticess.Clear();
                foreach (var point in points)
                {
                    verticess.Add(point);
                }
            }

            if (clockwise(verticess))
            {
                verticess.Reverse();
            }
            //0 for convex, 1 for concave
            Dictionary<Point, int> vertexType = new Dictionary<Point, int>();

            LinkedList<Point> list = new LinkedList<Point>(verticess);

            LinkedListNode<Point> current = list.First;

            while (current != null)
            {
                if (IsConvex(current, list))
                {
                    vertexType[current.Value] = 0; // Convex
                }
                else
                {
                    vertexType[current.Value] = 1; //Concave
                }

                current = current.Next;
            }


            Dictionary<Point, Boolean> ear = new Dictionary<Point, Boolean>();
            for (int i = 0; i < verticess.Count; ++i)
            {
                Boolean Ear = true;
                if (vertexType[verticess[i]] == 0)//convex
                {
                    Point a = verticess[(i - 1 + verticess.Count) % verticess.Count];
                    Point b = verticess[i];
                    Point c = verticess[(i + 1) % verticess.Count];
                    for (int j = 0; j < verticess.Count; ++j)
                    {
                        if (vertexType[verticess[j]] == 1)//concave
                        {
                            Point con = verticess[j];


                            Ear = IsEar(con, a, b, c);

                            if (!Ear) break;

                        }
                    }
                    if (Ear)
                    {
                        ear[verticess[i]] = true;
                    }
                    else
                    {
                        ear[verticess[i]] = false;
                    }
                }
            }


            current = list.First;
            while (list.Count > 3 && current != null)
            {
                LinkedListNode<Point> nextt = current.Next ?? list.First;
                if (ear.ContainsKey(current.Value) && ear[current.Value])
                {

                    LinkedListNode<Point> prevv = current.Previous ?? list.Last;
                    Point prev = current.Previous != null ? current.Previous.Value : list.Last.Value;
                    Point next = current.Next != null ? current.Next.Value : list.First.Value;
                    outLines.Add(new Line(prev, next));
                    list.Remove(current);
                    ear.Remove(current.Value);
                    //handle prev
                    Boolean isConvex = IsConvex(prevv ?? list.Last, list);
                    if (isConvex)
                    {
                        Point a, b, c;
                        if (prevv.Previous == null)
                        {
                            a = list.Last.Value;
                        }
                        else
                        {
                            a = prevv.Previous.Value;
                        }
                        if (prevv.Next == null)
                        {
                            c = list.First.Value;
                        }
                        else
                        {
                            c = prevv.Next.Value;
                        }
                        b = prevv.Value;

                        vertexType[prevv.Value] = 0;
                        Boolean isEar = true;
                        for (int j = 0; j < verticess.Count; ++j)
                        {
                            if (vertexType[verticess[j]] == 1)//concave
                            {
                                Point con = verticess[j];
                                isEar = IsEar(con, a, b, c);
                                if (!isEar)
                                    break;
                            }
                        }
                        if (isEar)
                        {
                            ear[prevv.Value] = true;
                        }
                        else
                        {
                            ear[prevv.Value] = false;
                        }
                    }
                    else
                    {
                        vertexType[prevv.Value] = 1;
                        ear[prevv.Value] = false;
                    }

                    //handle next 
                    isConvex = IsConvex(nextt ?? list.First, list);
                    if (isConvex)
                    {
                        Point a, b, c;
                        if (nextt.Previous == null)
                        {
                            a = list.Last.Value;
                        }
                        else
                        {
                            a = nextt.Previous.Value;
                        }
                        if (nextt.Next == null)
                        {
                            c = list.First.Value;
                        }
                        else
                        {
                            c = nextt.Next.Value;
                        }
                        b = nextt.Value;

                        vertexType[nextt.Value] = 0;
                        Boolean isEar = true;
                        for (int j = 0; j < verticess.Count; ++j)
                        {
                            if (vertexType[verticess[j]] == 1)//concave
                            {
                                Point con = verticess[j];
                                isEar = IsEar(con, a, b, c);
                                if (!isEar)
                                    break;
                            }
                        }
                        if (isEar)
                        {
                            ear[nextt.Value] = true;
                        }
                        else
                        {
                            ear[nextt.Value] = false;
                        }
                    }
                    else
                    {
                        vertexType[nextt.Value] = 1;
                        ear[nextt.Value] = false;
                    }
                }
                current = nextt;
            }
        }
        private bool IsEar(Point p, Point a, Point b, Point c)
        {
            if (HelperMethods.PointInTriangle(p, a, b, c) == Enums.PointInPolygon.Inside)
            {
                return false;
            }
            return true;
        }
        private bool IsConvex(LinkedListNode<Point> node, LinkedList<Point> list)
        {
            Point pPrev = (node.Previous ?? list.Last).Value;
            Point pCurr = node.Value;
            Point pNext = (node.Next ?? list.First).Value;

            Line edgeEntering = new Line(pPrev, pCurr);

            return HelperMethods.CheckTurn(edgeEntering, pNext) == Enums.TurnType.Left;
        }
        public bool clockwise(List<Point> points)
        {
            double sum = 0;
            Point y;
            for (int i = 0; i < points.Count; i++)
            {

                if (i == points.Count - 1)
                {
                    y = points[0];
                }
                else y = points[i + 1];

                sum += HelperMethods.CrossProduct(points[i], y);
            }


            return sum < 0;
        }
        public override string ToString()
        {
            return "Subtracting Ears";
        }
    }
}