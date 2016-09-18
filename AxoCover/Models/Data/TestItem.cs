﻿using AxoCover.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AxoCover.Models.Data
{
  public abstract class TestItem
  {
    public string Name { get; private set; }

    public TestItemKind Kind { get; private set; }

    public TestItem Parent { get; private set; }

    private List<TestItem> _children = new List<TestItem>();
    public IEnumerable<TestItem> Children { get { return _children; } }

    public int TestCount
    {
      get
      {
        return Kind == TestItemKind.Method ? 1 : Children.Sum(p => p.TestCount);
      }
    }

    public string FullName
    {
      get
      {
        return Parent == null || Parent is TestProject ? Name : Parent.FullName + "." + Name;
      }
    }

    public TestItem(TestItem parent, string name, TestItemKind kind)
    {
      Name = name;
      Kind = kind;
      Parent = parent;
      if (parent != null)
      {
        parent._children.OrderedAdd(this, (a, b) => StringComparer.OrdinalIgnoreCase.Compare(a.Name, b.Name));
      }
    }

    public void Remove()
    {
      Parent._children.Remove(this);
      Parent = null;
    }

    public static bool operator ==(TestItem a, TestItem b)
    {
      if ((object)a == null || (object)b == null)
      {
        return ReferenceEquals(a, b);
      }
      else
      {
        return a.Name == b.Name && a.Kind == b.Kind;
      }
    }

    public static bool operator !=(TestItem a, TestItem b)
    {
      return !(a == b);
    }

    public override bool Equals(object obj)
    {
      return obj is TestItem && this == obj as TestItem;
    }

    public override int GetHashCode()
    {
      return Name.GetHashCode() ^ Kind.GetHashCode();
    }
  }

  public static class TestItemHelper
  {
    public static T GetParent<T>(this TestItem testItem)
      where T : TestItem
    {
      while (testItem != null && !(testItem is T))
      {
        testItem = testItem.Parent;
      }
      return testItem as T;
    }
  }
}
