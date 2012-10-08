using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RMA.OpenNURBS;
using RMA.Rhino;

namespace SampleCsRhinoScriptExtension
{
  class SampleCsRhinoScriptUtils
  {
    /// <summary>
    /// Convert an object to an On3dPoint
    /// </summary>
    static public bool ConvertToOn3dPoint(object pointObj, ref On3dPoint point)
    {
      bool rc = false;
      Array pointArr = pointObj as Array;
      if (null != pointArr && 3 == pointArr.Length)
      {
        try
        {
          if (2 == pointArr.Length)
          {
            point.x = Convert.ToDouble(pointArr.GetValue(0));
            point.y = Convert.ToDouble(pointArr.GetValue(1));
            point.z = 0.0;
          }
          else if (3 == pointArr.Length)
          {
            point.x = Convert.ToDouble(pointArr.GetValue(0));
            point.y = Convert.ToDouble(pointArr.GetValue(1));
            point.z = Convert.ToDouble(pointArr.GetValue(2));
          }
          rc = point.IsValid();
        }
        catch
        {
          // Suppress System.InvalidCastException
        }
      }
      return rc;
    }

    /// <summary>
    /// Convert an object to an On3dPointArray
    /// </summary>
    static public bool ConvertToOn3dPointArray(object pointsObj, ref On3dPointArray points)
    {
      bool rc = false;
      int pointsCount = points.Count();
      Array pointsArr = pointsObj as Array;
      if (null != pointsArr)
      {
        for (int i = 0; i < pointsArr.Length; i++)
        {
          On3dPoint point = new On3dPoint();
          if (SampleCsRhinoScriptUtils.ConvertToOn3dPoint(pointsArr.GetValue(i), ref point))
            points.Append(point);
        }
        rc = (points.Count() - pointsCount > 0);
      }
      return rc;
    }
  }
}
