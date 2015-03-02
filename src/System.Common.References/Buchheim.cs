using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  /// <summary>
  /// 
  /// </summary>
  public class BuchNodeWrapper : IEnumerable<BuchNodeWrapper>
  {
    private object mNode;
    private List<BuchNodeWrapper> mNodes;

    /// <summary>
    /// 
    /// </summary>
    public object Node { get { return mNode; } }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <param name="getChildren"></param>
    public BuchNodeWrapper(object node, Func<object, IEnumerable<object>> getChildren)
    {
      mNode = node;
      mNodes = getChildren(mNode)
        .Select(n => new BuchNodeWrapper(n, getChildren))
        .ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator<BuchNodeWrapper> GetEnumerator()
    {
      foreach (var n in mNodes)
        yield return n;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      foreach (var n in mNodes)
        yield return n;
    }
  }

  /// <summary>
  /// http://billmill.org/pymag-trees/
  /// </summary>
  public static class Buchheim
  {
    public static BuchheimNode Layout(BuchNodeWrapper tree)
    {
      var dt = firstwalk(new BuchheimNode(tree));
      var min = second_walk(dt);
      if (min.HasValue && min < 0)
        third_walk(dt, -min.Value);
      return dt;
    }

    static void third_walk(BuchheimNode tree, float n)
    {
      tree.x += n;
      foreach (var c in tree.children)
        third_walk(c, n);
    }

    static BuchheimNode firstwalk(BuchheimNode v, float distance = 1)
    {
      if (v.children.Length == 0)
      {
        if (v.lmost_sibling)
          v.x = v.lbrother().x + distance;
        else
          v.x = 0;
      }
      else
      {
        var default_ancestor = v.children[0];
        foreach (var c in v.children)
        {
          firstwalk(c);
          default_ancestor = apportion(c, default_ancestor, distance);
        }
        execute_shifts(v);

        var midpoint = (v.children.First().x + v.children.Last().x) / 2f;

        var ell = v.children.First();
        var arr = v.children.Last();
        var w = v.lbrother();
        if (w)
        {
          v.x = w.x + distance;
          v.mod = v.x - midpoint;
        }
        else
        {
          v.x = midpoint;
        }
      }

      return v;
    }

    static BuchheimNode apportion(BuchheimNode v, BuchheimNode default_ancestor, float distance)
    {
      var w = v.lbrother();
      if (w != null)
      {
        var vir = v;
        var vor = v;
        var vil = w;
        var vol = v.lmost_sibling;

        var sir = v.mod;
        var sor = v.mod;
        var sil = vil.mod;
        var sol = vol.mod;

        while (vil.right() && vir.left())
        {
          vil = vil.right();
          vir = vir.left();
          vol = vol.left();
          vor = vor.right();
          vor.ancestor = v;
          var shift = (vil.x + sil) - (vir.x + sir) + distance;
          if (shift > 0)
          {
            move_subtree(get_ancestor(vil, v, default_ancestor), v, shift);
            sir = sir + shift;
            sor = sor + shift;
          }
          sil += vil.mod;
          sir += vir.mod;
          sol += vol.mod;
          sor += vor.mod;
        }
        if (vil.right() && !vor.right())
        {
          vor.thread = vil.right();
          vor.mod += sil - sor;
        }
        else
        {
          if (vir.left() && !vol.left())
          {
            vol.thread = vir.left();
            vol.mod += sir - sol;
          }
          default_ancestor = v;
        }
      }
      return default_ancestor;
    }

    static void move_subtree(BuchheimNode wl, BuchheimNode wr, float shift)
    {
      var subtrees = wr.number - wl.number;
      wr.change -= shift / subtrees;
      wr.shift += shift;
      wl.change += shift / subtrees;
      wr.x += shift;
      wr.mod += shift;
    }

    static void execute_shifts(BuchheimNode v)
    {
      float shift = 0, change = 0;
      foreach (var w in v.children.Reverse())
      {
        w.x += shift;
        w.mod += shift;
        change += w.change;
        shift += w.shift + change;
      }
    }

    static BuchheimNode get_ancestor(BuchheimNode vil, BuchheimNode v, BuchheimNode default_ancestor)
    {
      if (v.parent.children.Contains(vil.ancestor))
        return vil.ancestor;
      else
        return default_ancestor;
    }

    static float? second_walk(BuchheimNode v, float m = 0, int depth = 0, float? min = null)
    {
      v.x += m;
      v.y = depth;

      if (min == null || v.x < min)
        min = v.x;

      foreach (var w in v.children)
        min = second_walk(w, m + v.mod, depth + 1, min);

      return min;
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public class BuchheimNode
  {
    public float x;
    public int y;
    public BuchNodeWrapper node;
    public BuchheimNode[] children;
    public BuchheimNode parent;
    public BuchheimNode thread;
    public float mod;
    public BuchheimNode ancestor;
    public float change;
    public float shift;
    private BuchheimNode _lmost_sibling;
    public BuchheimNode lmost_sibling { get { return init_lmost_sibling(); } }
    public int number;

    public BuchheimNode(BuchNodeWrapper node, BuchheimNode parent = null, int depth = 0, int number = 1)
    {
      this.x = -1;
      this.y = depth;
      this.node = node;
      this.children = node
        .Select((c, i) => new BuchheimNode(c, this, depth + 1, i + 1))
        .ToArray();
      this.parent = parent;
      this.thread = null;
      this.mod = 0;
      this.ancestor = this;
      this.change = 0;
      this.shift = 0;
      this._lmost_sibling = null;
      this.number = number;
    }

    public BuchheimNode left()
    {
      if (this.thread != null) return this.thread;
      if (this.children.Any()) return this.children.First();
      return null;
    }

    public BuchheimNode right()
    {
      if (this.thread != null) return this.thread;
      if (this.children.Any()) return this.children.Last();
      return null;
    }

    public BuchheimNode lbrother()
    {
      BuchheimNode n = null;
      if (this.parent)
      {
        foreach (var node in this.parent.children)
        {
          if (node == this) return n;
          else n = node;
        }
      }
      return n;
    }

    private BuchheimNode init_lmost_sibling()
    {
      if (!this._lmost_sibling && this.parent && this != this.parent.children[0])
        this._lmost_sibling = this.parent.children[0];
      return this._lmost_sibling;
    }

    public static implicit operator bool(BuchheimNode node)
    {
      return node != null;
    }
  }
}