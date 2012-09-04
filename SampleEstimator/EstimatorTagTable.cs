/////////////////////////////////////////////////////////////////////////////
// EstimatorTagTable.cs

using System;
using System.Collections.Generic;

namespace Estimator
{
  public class EstimatorTagTable
  {
    private List<EstimatorTag> m_a;

    public EstimatorTagTable()
    {
      m_a = new List<EstimatorTag>(64);
    }

    public int TagCount()
    {
      return m_a.Count;
    }

    public EstimatorTag this[int index]
    {
      get
      {
        EstimatorTag tag = new EstimatorTag();
        if (-1 < index && index < m_a.Count)
          tag = m_a[index];
        return tag;
      }
    }

    public EstimatorTag Tag(int index)
    {
      EstimatorTag tag = new EstimatorTag();
      if (-1 < index && index < m_a.Count)
        tag = m_a[index];
      return tag;
    }

    public int FindTag(string id)
    {
      if (id.Length > 0)
      {
        for (int i = 0; i < m_a.Count; i++)
        {
          if (String.Compare(id, m_a[i].Id(), true) == 0)
            return i;
        }
      }
      return -1;
    }

    public int FindDescription(string description)
    {
      if (description.Length > 0)
      {
        for (int i = 0; i < m_a.Count; i++)
        {
          if (String.Compare(m_a[i].Description(), description, true) == 0)
            return i;
        }
      }
      return -1;
    }

    public int AddTag(EstimatorTag tag)
    {
      if (tag.IsValid())
      {
        int index = FindTag(tag.Id());
        if (index < 0)
        {
          m_a.Add(tag);
          return m_a.Count - 1;
        }
      }
      return -1;
    }

    public int GetTags(EstimatorTag.tag_type type, ref List<EstimatorTag> tags )
    {
      int tags_counts = tags.Count;
      if (type != EstimatorTag.tag_type.unknown_tag)
      {
        for (int i = 0; i < m_a.Count; i++)
        {
          if (m_a[i].Type() == type)
            tags.Add(m_a[i]);
        }
      }

      return tags.Count - tags_counts;
    }
  }
}