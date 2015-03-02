using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  /// <summary>
  /// Static class for performing an alternative to the Graham scan that uses a linear lexographic 
  /// sort of the point set by the x- and y-coordinates.
  /// </summary>
  public static class ChainConvexHull
  {
    #region Vertex

    /// <summary>
    /// Represents an ordered pair of floating-point x- and y-coordinates that defines a point in 
    /// a two-dimensional plane. Because the algorithms and helpers require a point, this is commonly 
    /// used to bridge the gap between multiple definitions of a float Point structures. It is assumed
    /// that this structure will be converted to and from the desired similair structure when needed.
    /// </summary>
    private struct Vertex
    {
      /// <summary>
      /// Represents a new instance of the Vector class with member data left uninitialized.
      /// </summary>
      public static readonly Vertex Empty;

      private float x;
      private float y;

      /// <summary>
      /// Initializes a new instance of the Vector class with the specified coordinates.
      /// </summary>
      /// <param name="xy">The value to assign to both x and y.</param>
      public Vertex(float xy)
        : this(xy, xy)
      {

      }

      /// <summary>
      /// Initializes a new instance of the Vector class with the specified coordinates.
      /// </summary>
      /// <param name="x">The horizontal position of the point.</param>
      /// <param name="y">The vertical position of the point.</param>
      public Vertex(float x, float y)
      {
        this.x = x;
        this.y = y;
      }

      /// <summary>
      /// Gets a value indicating whether this Vector is empty
      /// </summary>
      public bool IsEmpty
      {
        get { return ((this.x == 0f) && (this.y == 0f)); }
      }

      /// <summary>
      /// Gets or sets the x-coordinate of this Vector
      /// </summary>
      public float X
      {
        get { return this.x; }
        set { this.x = value; }
      }

      /// <summary>
      /// Gets or sets the y-coordinate of this Vector.
      /// </summary>
      public float Y
      {
        get { return this.y; }
        set { this.y = value; }
      }

      /// <summary>
      /// Compares two Vector structures. The result specifies whether the values of the X and Y properties of the 
      /// two Vector structures are equal.
      /// </summary>
      /// <param name="left">A Vector to compare.</param>
      /// <param name="right">A Vector to compare.</param>
      /// <returns>true if the X and Y values of the left and right Vector structures are equal; otherwise, false.</returns>
      public static bool operator ==(Vertex left, Vertex right)
      {
        return ((left.X == right.X) && (left.Y == right.Y));
      }

      /// <summary>
      /// Compares two Vector structures. The result specifies whether the values of the X and Y properties of the 
      /// two Vector structures are not equal.
      /// </summary>
      /// <param name="left">A Vector to compare.</param>
      /// <param name="right">A Vector to compare.</param>
      /// <returns>true if the X and Y values of the left and right Vector structures are not equal; otherwise, false.</returns>
      public static bool operator !=(Vertex left, Vertex right)
      {
        return !(left == right);
      }

      /// <summary>
      /// Specifies whether this Vector contains the same coordinates as the specified Object.
      /// </summary>
      /// <param name="obj">The object to test.</param>
      /// <returns>true if obj is a Vector and has the same coordinates as this point.</returns>
      public override bool Equals(object obj)
      {
        if (!(obj is Vertex))
        {
          return false;
        }

        Vertex tf = (Vertex)obj;
        return (((tf.X == this.X) && (tf.Y == this.Y)));
      }

      /// <summary>
      /// Returns a hash code for this Vector structure.
      /// </summary>
      /// <returns>An integer value that specifies a hash value for this Vector structure.</returns>
      public override int GetHashCode()
      {
        return base.GetHashCode();
      }

      /// <summary>
      /// Converts this Vector to a human readable string
      /// </summary>
      /// <returns>A string that represents this Vector.</returns>
      public override string ToString()
      {
        return string.Format("{{X={0}, Y={1}}}", new object[] { this.x, this.y });
      }

      static Vertex()
      {
        Empty = new Vertex();
      }
    }

    #endregion

    /// <summary>
    /// Andrew's monotone chain 2D convex hull algorithm.
    /// </summary>
    /// <typeparam name="T">The type of object.</typeparam>
    /// <param name="P">An array of 2D points presorted by increasing x- and y-coordinates</param>
    /// <param name="getX">The function to use to retrieve the x value.</param>
    /// <param name="getY">The function to use to retrieve the y value.</param>
    /// <param name="fromXY">A function used to create a new object of type T</param>
    /// <param name="n">the number of points in P[]</param>
    /// <param name="H">an array of the convex hull vertices (max is n)</param>
    /// <returns>the number of points in H[]</returns>
    public static int ComputeHull<T>(T[] P, Func<T, float> getX, Func<T, float> getY, 
      Func<float, float, T> fromXY, int n, out T[] H)
    {
      Vertex[] vertices;
      int retval = ComputeHull(P.Select(t => new Vertex(getX(t), getY(t))).ToArray(), n, out vertices);
      H = vertices.Select(v => fromXY(v.X, v.Y)).ToArray();
      return retval;
    }

    /// <summary>
    /// Andrew's monotone chain 2D convex hull algorithm.
    /// </summary>
    /// <param name="P">an array of 2D points presorted by increasing x- and y-coordinates</param>
    /// <param name="n">the number of points in P[]</param>
    /// <param name="H">an array of the convex hull vertices (max is n)</param>
    /// <returns>the number of points in H[]</returns>
    private static int ComputeHull(Vertex[] P, int n, out Vertex[] H)
    {
      // the output array H[] will be used as the stack
      int bot = 0, top = (-1);  // indices for bottom and top of the stack
      int i;                // array scan index
      H = new Vertex[P.Length];

      // Get the indices of points with min x-coord and min|max y-coord
      int minmin = 0, minmax;
      float xmin = P[0].X;
      for (i = 1; i < n; i++)
        if (P[i].X != xmin) break;
      minmax = i - 1;
      if (minmax == n - 1)
      {       // degenerate case: all x-coords == xmin
        H[++top] = P[minmin];
        if (P[minmax].Y != P[minmin].Y) // a nontrivial segment
          H[++top] = P[minmax];
        H[++top] = P[minmin];           // add polygon endpoint
        return top + 1;
      }

      // Get the indices of points with max x-coord and min|max y-coord
      int maxmin, maxmax = n - 1;
      float xmax = P[n - 1].X;
      for (i = n - 2; i >= 0; i--)
        if (P[i].X != xmax) break;
      maxmin = i + 1;

      // Compute the lower hull on the stack H
      H[++top] = P[minmin];      // push minmin point onto stack
      i = minmax;
      while (++i <= maxmin)
      {
        // the lower line joins P[minmin] with P[maxmin]
        if (isLeft(P[minmin], P[maxmin], P[i]) >= 0 && i < maxmin)
          continue;          // ignore P[i] above or on the lower line

        while (top > 0)        // there are at least 2 points on the stack
        {
          // test if P[i] is left of the line at the stack top
          if (isLeft(H[top - 1], H[top], P[i]) > 0)
            break;         // P[i] is a new hull vertex
          else
            top--;         // pop top point off stack
        }
        H[++top] = P[i];       // push P[i] onto stack
      }

      // Next, compute the upper hull on the stack H above the bottom hull
      if (maxmax != maxmin)      // if distinct xmax points
        H[++top] = P[maxmax];  // push maxmax point onto stack
      bot = top;                 // the bottom point of the upper hull stack
      i = maxmin;
      while (--i >= minmax)
      {
        // the upper line joins P[maxmax] with P[minmax]
        if (isLeft(P[maxmax], P[minmax], P[i]) >= 0 && i > minmax)
          continue;          // ignore P[i] below or on the upper line

        while (top > bot)    // at least 2 points on the upper stack
        {
          // test if P[i] is left of the line at the stack top
          if (isLeft(H[top - 1], H[top], P[i]) > 0)
            break;         // P[i] is a new hull vertex
          else
            top--;         // pop top point off stack
        }
        H[++top] = P[i];       // push P[i] onto stack
      }
      if (minmax != minmin)
        H[++top] = P[minmin];  // push joining endpoint onto stack

      return top + 1;
    }

    /// <summary>
    /// tests if a point is Left|On|Right of an infinite line.
    /// </summary>
    /// <param name="P0">Point 1.</param>
    /// <param name="P1">Point 2.</param>
    /// <param name="P2">Point 3.</param>
    /// <returns>
    /// &gt;0 for P2 left of the line through P0 and P1
    /// =0 for P2 on the line
    /// &lt;0 for P2 right of the line
    /// </returns>
    private static float isLeft(Vertex P0, Vertex P1, Vertex P2)
    {
      return (P1.X - P0.X) * (P2.Y - P0.Y) - (P2.X - P0.X) * (P1.Y - P0.Y);
    }
  }
}
