/////////////////////////////////////////////////////////////////////////////
// EstimatorTag.cs

using System;

namespace Estimator
{
  public class EstimatorTag
  {
    public enum tag_type
    {
      unknown_tag = 0,
      linear_tag = 1,
      area_tag = 2,
      volume_tag = 3,
      item_tag = 4,
    }

    public static tag_type TagType(int t)
    {
      tag_type type = tag_type.unknown_tag;
      switch (t)
      {
        case 1:
          type = tag_type.linear_tag;
          break;
        case 2:
          type = tag_type.area_tag;
          break;
        case 3:
          type = tag_type.volume_tag;
          break;
        case 4:
          type = tag_type.item_tag;
          break;
      }
      return type;
    }

    // Member variables
    private string m_id;
    private string m_description;
    private tag_type m_type;

    // Constructor
    public EstimatorTag()
    {
      m_id = "";
      m_description = "";
      m_type = tag_type.unknown_tag;
    }

    // Constructor
    public EstimatorTag(string id, string description, tag_type type)
    {
      Create(id, description, type);
    }

    // Constructor
    public EstimatorTag(string[] items)
    {
      Create(items);
    }

    // Creates an EstimatorTag object
    public bool Create(string id, string description, tag_type type)
    {
      m_id = id;
      m_id.Trim();
      m_description = description;
      m_description.Trim();
      m_type = type;
      return IsValid();
    }

    // Creates an EstimatorTag object
    public bool Create(string[] items)
    {
      if (items.Length == 3)
      {
        m_id = items[0];
        m_id.Trim();
        m_description = items[1];
        m_description.Trim();
        m_type = TagType(Int32.Parse(items[2]));
        return IsValid();
      }
      return false;
    }

    // Validates an EstimatorTag object
    public bool IsValid()
    {
      if (m_id.Length > 0 && m_description.Length > 0)
      {
        switch (m_type)
        {
          case tag_type.linear_tag:
          case tag_type.area_tag:
          case tag_type.volume_tag:
          case tag_type.item_tag:
            return true;
        }
      }
      return false;
    }

    public string Id()
    {
      return m_id;
    }

    public string Description()
    {
      return m_description;
    }

    public tag_type Type()
    {
      return m_type;
    }
  }
}
