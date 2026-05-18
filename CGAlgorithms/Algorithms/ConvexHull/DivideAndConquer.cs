using CGAlgorithms;
using CGUtilities;
using System.Collections.Generic;
using System;
using System.Linq;

public class DivideAndConquer : Algorithm
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
                if (points[j].X < points[minIndex].X || (points[j].X == points[minIndex].X && points[j].Y < points[minIndex].Y))
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
            {
                points.RemoveAt(k);
            }

            else
            {
                k++;
            }
        }

        outPoints = Divide(points);
    }


    public static int LargestX(List<Point> points)
    {
        int x = 0;
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i].X > points[x].X || (points[i].X == points[x].X && points[i].Y > points[x].Y))
            {
                x = i;
            }
        }

        return x;
    }

    public static int SmallestX(List<Point> points)
    {
        int x = 0;
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i].X < points[x].X || (points[i].X == points[x].X && points[i].Y < points[x].Y))
            {
                x = i;
            }
        }
        return x;
    }

    public List<Point> Merge(List<Point> Lift, List<Point> Right)
    {
        int LeftCount = Lift.Count;
        int RightCount = Right.Count;
        bool Change;
        int UpperRightIndex = SmallestX(Right);
        int UpperLeftIndex = LargestX(Lift);
        int OldRight = (RightCount + UpperRightIndex - 1) % RightCount;
        int NextToLeft = (UpperLeftIndex + 1) % LeftCount;

        do
        {
            do
            {
                if (HelperMethods.CheckTurn(new Line(Right[UpperRightIndex], Lift[UpperLeftIndex]), Lift[NextToLeft]) == Enums.TurnType.Right)
                {
                    UpperLeftIndex = NextToLeft;
                    NextToLeft = (UpperLeftIndex + 1) % LeftCount;
                    Change = false;
                }
                else
                {
                    Change = true;
                }

            } while (!Change);

            if (Change == true && (HelperMethods.CheckTurn(new Line(Right[UpperRightIndex], Lift[UpperLeftIndex]), Lift[NextToLeft]) == Enums.TurnType.Colinear))
            {
                UpperLeftIndex = NextToLeft;
            }
            NextToLeft = (UpperLeftIndex + 1) % LeftCount;

            while (HelperMethods.CheckTurn(new Line(Lift[UpperLeftIndex], Right[UpperRightIndex]), Right[OldRight]) == Enums.TurnType.Left)
            {
                UpperRightIndex = OldRight;
                OldRight = (RightCount + UpperRightIndex - 1) % RightCount;

                Change = false;
            }

            if (Change == true && (HelperMethods.CheckTurn(new Line(Lift[UpperLeftIndex], Right[UpperRightIndex]), Right[OldRight]) == Enums.TurnType.Colinear))
            {
                UpperRightIndex = OldRight;
                OldRight = (RightCount + UpperRightIndex - 1) % RightCount;
            }
        } while (Change == false);

        int LowerLeftIndex = LargestX(Lift);
        int LowerRightIndex = SmallestX(Right);
        int OldLeft = (LeftCount + LowerLeftIndex - 1) % LeftCount;
        int NextToRight = (LowerRightIndex + 1) % RightCount;

        do
        {
            Change = true;
            while (HelperMethods.CheckTurn(new Line(Right[LowerRightIndex], Lift[LowerLeftIndex]), Lift[OldLeft]) == Enums.TurnType.Left)
            {
                LowerLeftIndex = OldLeft;
                OldLeft = (LeftCount + LowerLeftIndex - 1) % LeftCount;
                Change = false;
            }

            if (Change == true && (HelperMethods.CheckTurn(new Line(Right[LowerRightIndex], Lift[LowerLeftIndex]), Lift[OldLeft]) == Enums.TurnType.Colinear))
            {
                LowerLeftIndex = OldLeft;
                OldLeft = (LeftCount + LowerLeftIndex - 1) % LeftCount;
            }

            while (HelperMethods.CheckTurn(new Line(Lift[LowerLeftIndex], Right[LowerRightIndex]), Right[NextToRight]) == Enums.TurnType.Right)
            {
                Change = false;
                LowerRightIndex = NextToRight;
                NextToRight = (LowerRightIndex + 1) % RightCount;
            }

            if (Change == true && (HelperMethods.CheckTurn(new Line(Lift[LowerLeftIndex], Right[LowerRightIndex]), Right[NextToRight]) == Enums.TurnType.Colinear))
            {
                LowerRightIndex = NextToRight;
                NextToRight = (LowerRightIndex + 1) % RightCount;
            }
        } while (Change == false);

        List<Point> NewPoints = new List<Point>();
        int ind = UpperLeftIndex;
        if (!NewPoints.Contains(Lift[UpperLeftIndex]))
        {
            NewPoints.Add(Lift[UpperLeftIndex]);
        }

        while (ind != LowerLeftIndex)
        {
            ind = (ind + 1) % LeftCount;
            if (!NewPoints.Contains(Lift[ind]))
            {
                NewPoints.Add(Lift[ind]);
            }
        }

        ind = LowerRightIndex;
        if (!NewPoints.Contains(Right[LowerRightIndex]))
        {
            NewPoints.Add(Right[LowerRightIndex]);
        }

        while (ind != UpperRightIndex)
        {
            ind = (ind + 1) % RightCount;
            if (!NewPoints.Contains(Right[ind]))
            {
                NewPoints.Add(Right[ind]);
            }
        }

        return NewPoints;
    }

    public List<Point> Divide(List<Point> Points)
    {
        if (Points.Count == 1)
        {
            return Points;
        }

        List<Point> r = new List<Point>();
        List<Point> l = new List<Point>();

        int i = 0;
        do
        {
            l.Add(Points[i]);
            i++;
        } while (i < Points.Count / 2);

        i = Points.Count / 2;
        do
        {
            r.Add(Points[i]);
            i++;
        } while (i < Points.Count);

        List<Point> Left = Divide(l);
        List<Point> Right = Divide(r);

        return Merge(Left, Right);
    }



    public override string ToString()
    {
        return "Convex Hull - Divide & Conquer";
    }

}
