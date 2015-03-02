using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  public delegate bool BSPNodeCallback(BSPNode node, object userData);

  public sealed class BSPNode
  {
    public static implicit operator bool(BSPNode node)
    {
      return node != null;
    }

    private BSPNode next;
    private BSPNode father;
    private BSPNode sons;

    private int x, y, width, height;
    private int position, level;
    private bool horizontal;
    private Random randomizer;

    public int X { get { return x; } }
    public int Y { get { return y; } }
    public int Width { get { return width; } }
    public int Height { get { return height; } }
    public int Level { get { return level; } }
    public int Position { get { return position; } }
    public bool Horizontal { get { return horizontal; } }

    public BSPNode Left { get { return sons; } }
    public BSPNode Right { get { return sons != null ? sons.next : null; } }
    public BSPNode Father { get { return father; } }
    public bool IsLeaf { get { return sons == null; } }

    public BSPNode()
      : this(0, 0, 0, 0)
    {

    }

    public BSPNode(int x, int y, int width, int height)
    {
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;
      this.level = 0;
      this.randomizer = new Random();
    }

    private BSPNode(BSPNode father, bool left)
    {
      if (father.horizontal)
      {
        x = father.x;
        width = father.width;
        y = left ? father.y : father.position;
        height = left ? father.position - y : father.y + father.height - father.position;
      }
      else
      {
        y = father.y;
        height = father.height;
        x = left ? father.x : father.position;
        width = left ? father.position - x : father.x + father.width - father.position;
      }
      level = father.level + 1;
    }

    public void Add(BSPNode node)
    {
      node.father = this;
      BSPNode lastson = sons;
      while (lastson && lastson.next)
        lastson = lastson.next;

      if (lastson)
        lastson.next = node;
      else
        sons = node;
    }

    public void Clear()
    {
      BSPNode node = sons;
      while (node)
      {
        BSPNode nextNode = node.next;
        node.Clear();
        node = nextNode;
      }
      sons = null;
    }

    public bool ForEach(BSPNodeCallback callback, object userData = null)
    {
      var left = Left;
      if (left && !left.ForEach(callback, userData)) return false;

      if (!callback(this, userData)) return false;

      var right = Right;
      if (right && !right.ForEach(callback, userData)) return false;

      return true;
    }

    public bool Contains(int px, int py)
    {
      return (px >= x && py >= y && px < x + width && py < y + height);
    }

    public BSPNode Find(int px, int py)
    {
      if (!Contains(px, py)) return null;
      if (!IsLeaf)
      {
        var left = Left;
        if (left.Contains(px, px)) return left.Find(px, py);

        var right = Right;
        if (right.Contains(px, py)) return right.Find(px, py);
      }
      return this;
    }

    public void Resize(int x, int y, int width, int height)
    {
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;

      var left = Left;
      var right = Right;

      if (left)
      {
        if (horizontal)
        {
          left.Resize(x, y, width, position - y);
          right.Resize(x, position, width, y + height - position);
        }
        else
        {
          left.Resize(x, y, position - x, height);
          right.Resize(position, y, x + width - position, height);
        }
      }
    }

    public void SplitOnce(bool horizontal, int position)
    {
      this.horizontal = horizontal;
      this.position = position;

      Add(new BSPNode(this, true));
      Add(new BSPNode(this, false));
    }

    public void SplitRecursive(int nb, int minHSize, int minVSize, float maxHRatio, float maxVRatio)
    {
      if (nb == 0 || (width < 2 * minHSize && height < 2 * minVSize)) return;
      bool horiz;

      // promote square rooms
      if (height < 2 * minVSize || width > height * maxHRatio) horiz = false;
      else if (width < 2 * minHSize || height > width * maxVRatio) horiz = true;
      else horiz = randomizer.Next() % 2 == 0;
      int position;
      if (horiz)
      {
        position = randomizer.Next(y + minVSize, (y + height - minVSize) + 1);
      }
      else
      {
        position = randomizer.Next(x + minHSize, (x + width - minHSize) + 1);
      }

      SplitOnce(horiz, position);

      Left.SplitRecursive(nb - 1, minHSize, minVSize, maxHRatio, maxVRatio);
      Right.SplitRecursive(nb - 1, minHSize, minVSize, maxHRatio, maxVRatio);
    }
  }
}