using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RMA.OpenNURBS;
using RMA.Rhino;

namespace SampleCsRhinoScriptExtension
{
  [System.Runtime.InteropServices.ComVisible(true)]
  public class SampleCsRhinoScriptObject
  {
    /// <summary>
    /// Return an sample integer
    /// </summary>
    public int GetInteger()
    {
      return 24;
    }

    /// <summary>
    /// Return a sample double
    /// </summary>
    public double GetDouble()
    {
      // Returns a double-precision number
      return OnUtil.On_PI;
    }

    /// <summary>
    /// Return a sample string
    /// </summary>
    public string GetString()
    {
      return "Hello RhinoScript!";
    }

    /// <summary>
    /// Return a sample 3-D point
    /// </summary>
    public object GetPoint()
    {
      ArrayList pt = new ArrayList(3);
      pt.Add(2.0);
      pt.Add(1.0);
      pt.Add(0.0);
      return pt.ToArray();
    }

    /// <summary>
    /// Returns a sample array of 3-D points
    /// </summary>
    public object GetPoints()
    {
      ArrayList pts = new ArrayList();

      ArrayList p0 = new ArrayList();
      p0.Add(0.0);
      p0.Add(0.0);
      p0.Add(0.0);
      pts.Add(p0.ToArray());

      ArrayList p1 = new ArrayList();
      p1.Add(10.0);
      p1.Add(0.0);
      p1.Add(0.0);
      pts.Add(p1.ToArray());

      ArrayList p2 = new ArrayList();
      p2.Add(10.0);
      p2.Add(10.0);
      p2.Add(0.0);
      pts.Add(p2.ToArray());

      ArrayList p3 = new ArrayList();
      p3.Add(0.0);
      p3.Add(10.0);
      p3.Add(0.0);
      pts.Add(p3.ToArray());

      return pts.ToArray();
    }

    /// <summary>
    /// Add a 3-D point to the document
    /// </summary>
    public object AddPoint(object pointObj)
    {
      On3dPoint point = new On3dPoint();
      if (SampleCsRhinoScriptUtils.ConvertToOn3dPoint(pointObj, ref point))
      {
        MRhinoDoc doc = RhUtil.RhinoApp().ActiveDoc();
        if (null != doc)
        {
          MRhinoObject rhinoObj = doc.AddPointObject(point);
          if (null != rhinoObj)
          {
            doc.Redraw();
            return rhinoObj.ModelObjectId().ToString();
          }
        }
      }
      return null;
    }

    /// <summary>
    /// Add an array of 3-D points to the document
    /// </summary>
    public object AddPoints(object pointsObj)
    {
      On3dPointArray points = new On3dPointArray();
      if (SampleCsRhinoScriptUtils.ConvertToOn3dPointArray(pointsObj, ref points))
      {
        MRhinoDoc doc = RhUtil.RhinoApp().ActiveDoc();
        if (null != doc)
        {
          ArrayList objectIds = new ArrayList();
          for (int i = 0; i < points.Count(); i++)
          {
            MRhinoObject rhinoObj = doc.AddPointObject(points[i]);
            if (null != rhinoObj)
              objectIds.Add(rhinoObj.ModelObjectId().ToString());
          }
          if (objectIds.Count > 0)
          {
            doc.Redraw();
            return objectIds.ToArray();
          }
        }
      }
      return null;
    }
  }
}
