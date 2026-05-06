using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{
    class SweepLine : Algorithm
    {
        private HashSet<(double, double)> foundIntersections = new HashSet<(double, double)>();

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            var eventQueue = new SortedSet<Event>(new EventComparer());
            foundIntersections.Clear();

            foreach (var l in lines)
            {
                if (l.Start.X > l.End.X)
                {
                    Point temp = l.Start;
                    l.Start = l.End;
                    l.End = temp;
                }
                else if (Math.Abs(l.Start.X - l.End.X) < 1e-9 && Math.Abs(l.Start.Y - l.End.Y) < 1e-9)
                {
                    Point temp = l.Start;
                    l.Start = l.End;
                    l.End = temp;
                }
                eventQueue.Add(new Event(l.Start, EventType.START, l));
                eventQueue.Add(new Event(l.End, EventType.END, l));
            }

            var status = new StatusStructure();

            while (eventQueue.Count > 0)
            {
                Event q = eventQueue.Min;
                eventQueue.Remove(q);

                handleEventPoint(q, eventQueue, status, ref outPoints);
            }
        }

        public void handleEventPoint(Event e, SortedSet<Event> eventQueue, StatusStructure status, ref List<Point> outPoints)
        {
            double x = e.Location.X;

            if (e.Type == EventType.START)
            {
                status.Add(e.Segment1, x);
                Line above = status.FindAbove(e.Segment1, x);
                Line below = status.FindBelow(e.Segment1, x);

                FindNewEvent(e.Segment1, above, e.Location, eventQueue);
                FindNewEvent(e.Segment1, below, e.Location, eventQueue);
            }
            else if (e.Type == EventType.END)
            {
                Line above = status.FindAbove(e.Segment1, x);
                Line below = status.FindBelow(e.Segment1, x);

                status.Remove(e.Segment1, x);

                if (above != null && below != null)
                    FindNewEvent(above, below, e.Location, eventQueue);
            }
            else if (e.Type == EventType.INTERSECTION)
            {
                var key = (Math.Round(e.Location.X, 7), Math.Round(e.Location.Y, 7));
                if (!foundIntersections.Contains(key))
                {
                    outPoints.Add(e.Location);
                    foundIntersections.Add(key);
                }

                status.SwapLines(e.Segment1, e.Segment2, x);

                FindNewEvent(e.Segment1, status.FindBelow(e.Segment1, x), e.Location, eventQueue);
                FindNewEvent(e.Segment1, status.FindAbove(e.Segment1, x), e.Location, eventQueue);


                FindNewEvent(e.Segment2, status.FindBelow(e.Segment2, x), e.Location, eventQueue);
                FindNewEvent(e.Segment2, status.FindAbove(e.Segment2, x), e.Location, eventQueue);
            }
        }

        public void FindNewEvent(Line s1, Line s2, Point p, SortedSet<Event> eventQueue)
        {
            if (s1 == null || s2 == null) return;
            Point intersection = GetIntersection(s1, s2);

            if (intersection != null)
            {
                if (intersection.X > p.X + 1e-9 || (Math.Abs(intersection.X - p.X) < 1e-9 && intersection.Y > p.Y + 1e-9))
                {
                    eventQueue.Add(new Event(intersection, EventType.INTERSECTION, s1, s2));
                }
            }
        }

        public Point GetIntersection(Line s1, Line s2)
        {
            double x1 = s1.Start.X, y1 = s1.Start.Y, x2 = s1.End.X, y2 = s1.End.Y;
            double x3 = s2.Start.X, y3 = s2.Start.Y, x4 = s2.End.X, y4 = s2.End.Y;

            double den = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (Math.Abs(den) < 1e-9) return null;

            double intersectX = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / den;
            double intersectY = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / den;

            Point p = new Point(intersectX, intersectY);

            if (OnSegment(p, s1) && OnSegment(p, s2))
                return p;

            return null;
        }

        private bool OnSegment(Point p, Line l)
        {
            return p.X >= Math.Min(l.Start.X, l.End.X) - 1e-9 && p.X <= Math.Max(l.Start.X, l.End.X) + 1e-9 &&
                   p.Y >= Math.Min(l.Start.Y, l.End.Y) - 1e-9 && p.Y <= Math.Max(l.Start.Y, l.End.Y) + 1e-9;
        }

        public override string ToString() => "Sweep Line";
    }

    public enum EventType { START, END, INTERSECTION }

    public class Event
    {
        public Point Location { get; set; }
        public EventType Type { get; set; }
        public Line Segment1 { get; set; }
        public Line Segment2 { get; set; }

        public Event(Point loc, EventType type, Line s1, Line s2 = null)
        {
            Location = loc;
            Type = type;
            Segment1 = s1;
            Segment2 = s2;
        }
    }

    public class EventComparer : IComparer<Event>
    {
        public int Compare(Event e1, Event e2)
        {
            if (Math.Abs(e1.Location.X - e2.Location.X) > 1e-9)
                return e1.Location.X.CompareTo(e2.Location.X);
            if (Math.Abs(e1.Location.Y - e2.Location.Y) > 1e-9)
                return e1.Location.Y.CompareTo(e2.Location.Y);
            return e1.Type.CompareTo(e2.Type);
        }
    }

    public class StatusStructure
    {
        private AVLTree<Line> tree;
        public StatusComparer Comparer { get; private set; }

        public StatusStructure()
        {
            Comparer = new StatusComparer();
            tree = new AVLTree<Line>(Comparer);
        }

        public void Add(Line s, double x)
        {
            Comparer.SweepLineX = x;
            tree.Insert(s);
        }

        public void Remove(Line s, double x)
        {
            Comparer.SweepLineX = x;
            tree.Delete(s);
        }

        public Line FindAbove(Line s, double x)
        {
            Comparer.SweepLineX = x;
            return tree.FindSuccessor(s);
        }

        public Line FindBelow(Line s, double x)
        {
            Comparer.SweepLineX = x;
            return tree.FindPredecessor(s);
        }

        public void SwapLines(Line s1, Line s2)
        {
            SwapLines(s1, s2, Comparer.SweepLineX);
        }

        public void SwapLines(Line s1, Line s2, double x)
        {
            Comparer.SweepLineX = x - 1e-8;
            tree.Delete(s1);
            tree.Delete(s2);

            Comparer.SweepLineX = x + 1e-8;
            tree.Insert(s1);
            tree.Insert(s2);

            Comparer.SweepLineX = x; 
        }
    }
    public class StatusComparer : IComparer<Line>
    {
        public double SweepLineX { get; set; }

        public int Compare(Line s1, Line s2)
        {
            if (ReferenceEquals(s1, s2)) return 0;

            double y1 = GetYAt(s1, SweepLineX);
            double y2 = GetYAt(s2, SweepLineX);

            if (Math.Abs(y1 - y2) > 1e-9)
                return y1.CompareTo(y2);

            double m1 = GetSlope(s1);
            double m2 = GetSlope(s2);

            if (Math.Abs(m1 - m2) > 1e-9)
                return m1.CompareTo(m2);

            return s1.GetHashCode().CompareTo(s2.GetHashCode());
        }

        private double GetYAt(Line l, double x)
        {
            if (Math.Abs(l.End.X - l.Start.X) < 1e-9)
                return Math.Max(l.Start.Y, l.End.Y);

            double m = GetSlope(l);
            return l.Start.Y + m * (x - l.Start.X);
        }

        private double GetSlope(Line l)
        {
            if (Math.Abs(l.End.X - l.Start.X) < 1e-9)
                return double.MaxValue; // Vertical line
            return (l.End.Y - l.Start.Y) / (l.End.X - l.Start.X);
        }
    }
}