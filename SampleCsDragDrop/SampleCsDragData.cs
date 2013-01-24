using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace SampleCsDragDrop
{
  /// <summary>
  /// The SampleCsDragData class
  /// </summary>
  public class SampleCsDragData : ISerializable
  {
    private string _dragString = null;

    /// <summary>
    /// Constructor
    /// </summary>
    public SampleCsDragData(string dragString)
    {
      _dragString = dragString;
    }

    /// <summary>
    /// Get data from info packet
    /// </summary>
    protected SampleCsDragData(SerializationInfo info, StreamingContext context)
    { 
      _dragString = info.GetString("SampleCsDragDropString");
    }

    /// <summary>
    /// Save data to info packet
    /// </summary>
    [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    { 
      info.AddValue("SampleCsDragDropString", _dragString);
    }

    /// <summary>
    /// Member data getter
    /// </summary>
    public string DragString
    {
      get
      {
        return _dragString;
      }
    }
  }
}
