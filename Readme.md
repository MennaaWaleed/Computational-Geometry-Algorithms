# Computational Geometry Algorithms

A Computational Geometry project that implements and visualizes multiple classic geometry algorithms using C#. The project focuses on convex hull construction, polygon triangulation, and line sweep techniques.

## Implemented Algorithms

### Convex Hull Algorithms
- Extreme Points
- Extreme Segments
- Jarvis March (Gift Wrapping)
- Graham Scan
- QuickHull
- Incremental Algorithm
- Divide and Conquer Algorithm

### Polygon Algorithms
- Subtracting Ears (Ear Clipping)

### Line Sweep Algorithms
- Sweeping Line Algorithm

---

# Algorithms Overview

## 1. Extreme Points
Checks every point and determines whether it lies inside a triangle formed by other points. Non-interior points are part of the convex hull.

### Time Complexity
- O(n^4)

---

## 2. Extreme Segments
Checks every segment between pairs of points and determines whether all other points lie on one side of the segment.

### Time Complexity
- O(n^3)

---

## 3. Jarvis March (Gift Wrapping)
Starts from the leftmost point and repeatedly selects the next point that forms the most counterclockwise turn.

### Time Complexity
- O(nh)

Where:
- n = number of points
- h = number of hull points

---

## 4. Graham Scan
Sorts points based on polar angle relative to a pivot point and uses a stack to maintain the convex hull.

### Time Complexity
- O(n log n)

---

## 5. QuickHull
A divide-and-conquer convex hull algorithm similar in concept to QuickSort.

### Time Complexity
- Average: O(n log n)
- Worst Case: O(n^2)

---

## 6. Incremental Algorithm
Builds the convex hull incrementally by inserting points one at a time and updating the hull.

### Time Complexity
- O(n log n)

---

## 7. Divide and Conquer
Divides the set of points into smaller subsets, computes hulls recursively, then merges them.

### Time Complexity
- O(n log n)

---

## 8. Sweeping Line Algorithm
Uses a moving sweep line with a dynamic status structure to process geometric events efficiently.

### Features
- AVLTree used for active status structure
- Efficient event handling

---

## 9. Subtracting Ears (Ear Clipping)
Triangulates a polygon by repeatedly removing ears.

### Time Complexity
- O(n^2)

---

# Technologies Used

- C#
- .NET
- Computational Geometry Concepts
- AVL Trees

---

# Project Structure

```text
COMPUTATIONAL-GEOMETRY-ALGORITHMS/
│
├── CGAlgorithms/
│   │
│   ├── Algorithms/
│   │   ├── ConvexHull/
│   │   ├── PolygonTriangulation/
│   │   └── SegmentIntersection/
│   │
│   ├── bin/
│   ├── obj/
│   ├── Properties/
│   │
│   ├── Algorithm.cs
│   └── CGAlgorithms.csproj
│
├── CGAlgorithmsUnitTest/
├── CGUI/
├── CGUtilities/
├── CGUtilitiesUnitTest/
├── TestResults/
│
├── .gitignore
├── CGPackage.sln
├── LICENSE
└── README.md