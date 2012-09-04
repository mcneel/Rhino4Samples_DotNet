/////////////////////////////////////////////////////////////////////////////
// EstimatorHelpers.cs

using System;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace Estimator
{
  class EstimatorHelpers
  {
    /// <summary>
    /// Adds a single string datum to an 
    /// object's attribute user string data.
    /// </summary>
    public static bool AddData(IRhinoObject obj, string string_datum)
    {
      if (obj != null && string_datum.Length > 0)
      {
        string[] string_array = null;
        if (GetData(obj, ref string_array) > 0)
        {
          int num_added = 0;
          for (int i = 0; i < string_array.Length; i++)          
          {
            if (string_array[i].IndexOf(string_datum) >= 0)
            {
              num_added++;
              break;
            }
          }

          if (num_added > 0)
          {
            // Copy array
            string[] new_string_array = new string[string_array.Length + 1];
            Array.Copy(string_array, new_string_array, string_array.Length);
            // Add to end of copy
            new_string_array[new_string_array.Length - 1] = string_datum;
            // Sort copy
            Array.Sort(new_string_array);
            // Set data
            return SetData(obj, new_string_array);
          }
        }
        else
        {
          return SetData(obj, string_datum);
        }
      }
      return false;
    }

    /// <summary>
    /// Adds an array of string data to an
    /// object's attribute user string data.
    /// </summary>
    public static bool AddData(IRhinoObject obj, string[] string_data)
    {
      if (obj != null && string_data.Length > 0)
      {
        string[] string_array = null;
        if (GetData(obj, ref string_array) > 0)
        {
          int string_array_length = string_array.Length;

          for (int i = 0; i < string_data.Length; i++)
          {
            bool found = false;
            for (int j = 0; j < string_array.Length; j++)
            {
              if (string_array[j].IndexOf(string_data[i]) >= 0)
              {
                found = true;
                break;
              }
            }

            if (!found)
            {
              // Copy array
              string[] new_string_array = new string[string_array.Length + 1];
              Array.Copy(string_array, new_string_array, string_array.Length);
              // Add to end of copy
              new_string_array[new_string_array.Length - 1] = string_data[i];
              // Replace original with copy
              string_array = new_string_array;
            }
          }

          if (string_array.Length > string_array_length)
          {
            // Sort array
            Array.Sort(string_array);
            // Set data
            return SetData(obj, string_array);
          }
        }
        else
        {
          return SetData(obj, string_data);
        }
      }
      return false;
    }

    /// <summary>
    /// Removes a single string datum from an
    /// object's attribute user string data.
    /// </summary>
    public static bool RemoveData(IRhinoObject obj, string string_datum)
    {
      if (obj != null && string_datum.Length > 0)
      {
        string[] string_array = null;
        if (GetData(obj, ref string_array) > 0)
        {
          int num_removed = 0;
          for (int i = 0; i < string_array.Length; i++)
          {
            if (string_array[i].IndexOf(string_datum) >= 0)
            {
              string_array[i] = String.Empty;
              num_removed++;
              break;
            }
          }

          if (num_removed > 0)
          {
            if (num_removed == string_array.Length)
              return SetData(obj, String.Empty);
            return SetData(obj, string_array);
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Removes an array of string data from an
    /// object's attribute user string data.
    /// </summary>
    public static bool RemoveData(IRhinoObject obj, string[] string_data)
    {
      if (obj != null && string_data.Length > 0)
      {
        string[] string_array = null;
        if (GetData(obj, ref string_array) > 0)
        {
          int num_removed = 0;
          for (int i = 0; i < string_data.Length; i++)
          {
            for (int j = 0; j < string_array.Length; j++)
            {
              if (string_array[i].IndexOf(string_data[j]) >= 0)
              {
                string_array[i] = String.Empty;
                num_removed++;
                break;
              }
            }
          }

          if (num_removed > 0)
          {
            if (num_removed == string_array.Length)
              return SetData(obj, String.Empty);
            return SetData(obj, string_array);
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Removes all string data
    /// object's attribute user string data.
    /// </summary>
    public static bool RemoveAllData(IRhinoObject obj)
    {
      if (obj != null)
        return SetData(obj, String.Empty);
      return false;
    }

    /// <summary>
    /// Low level function to get an array of string data from an
    /// object's attribute user string data.
    /// </summary>
    public static int GetData(IRhinoObject obj, ref string[] string_array)
    {
      string string_data = String.Empty;
      if (GetData(obj, ref string_data) > 0)
      {
        char[] delims = new char[] { ';' };
        string_array = string_data.Split(delims, StringSplitOptions.RemoveEmptyEntries);
        return string_array.Length;
      }
      return 0;
    }

    /// <summary>
    /// Low level function to get formattted string data from an
    /// object's attribute user string data.
    /// </summary>
    public static int GetData(IRhinoObject obj, ref string string_data)
    {
      if (obj != null)
      {
        if (obj.Attributes().GetUserString(GetKey(), ref string_data))
          return string_data.Length;
      }
      return 0;
    }

    /// <summary>
    /// Low level function to set an array of string data to an
    /// object's attribute user string data.
    /// </summary>
    public static bool SetData(IRhinoObject obj, string[] string_array)
    {
      if (obj != null && string_array.Length > 0)
      {
        string string_data = String.Empty;
        for (int i = 0; i < string_array.Length; i++)
        {
          if (string_array[i].Length > 0)
          {
            if (i > 0 && string_data.Length > 0)
              string_data += ";";
            string_data += string_array[i];
          }
        }
        return SetData(obj, string_data);
      }
      return false;
    }

    /// <summary>
    /// Low level function to set formattted string data to an
    /// object's attribute user string data.
    /// </summary>
    public static bool SetData(IRhinoObject obj, string string_data)
    {
      if (obj != null)
      {
        MRhinoDoc doc = obj.Document();
        if (doc != null)
        {
          MRhinoObjectAttributes atts = new MRhinoObjectAttributes(obj.Attributes());
          if (atts.SetUserString(GetKey(), string_data))
            return doc.ModifyObjectAttributes(new MRhinoObjRef(obj), atts);
        }
      }
      return false;
    }

    /// <summary>
    /// Returns the string key to use when accessing an
    /// object's attribute user string data.
    /// </summary>
    public static string GetKey()
    {
      System.Guid plugin_id = RhUtil.GetPlugInInstance().PlugInID();
      return plugin_id.ToString();
    }

    /// <summary>
    /// Calculates the length of an object
    /// </summary>
    public static double GetLength(IRhinoObject obj)
    {
      double length = 0.0;
      if (null != obj)
      {
        IOnCurve crv = OnCurve.ConstCast(obj.Geometry());
        if (null != crv)
          crv.GetLength(ref length);
      }
      return length;
    }

    /// <summary>
    /// Calculates the area of an object
    /// </summary>
    public static double GetArea(IRhinoObject obj, double tol)
    {
      if (null != obj)
      {
        IOnCurve crv = OnCurve.ConstCast(obj.Geometry());
        if (null != crv)
          return GetCurveArea(crv, tol);

        IOnSurface srf = OnSurface.ConstCast(obj.Geometry());
        if (null != srf)
          return GetSurfaceArea(srf);

        IOnBrep brep = OnBrep.ConstCast(obj.Geometry());
        if (null != brep)
          return GetBrepArea(brep);

        IOnMesh mesh = OnMesh.ConstCast(obj.Geometry());
        if (null != mesh)
          return GetMeshArea(mesh);

      }
      return 0.0;
    }

    /// <summary>
    /// Calculates the volume of an object
    /// </summary>
    public static double GetVolume(IRhinoObject obj)
    {
      if (null != obj && obj.IsSolid())
      {
        IOnSurface srf = OnSurface.ConstCast(obj.Geometry());
        if (null != srf)
          return GetSurfaceVolume(srf);

        IOnBrep brep = OnBrep.ConstCast(obj.Geometry());
        if (null != brep)
          return GetBrepVolume(brep);

        IOnMesh mesh = OnMesh.ConstCast(obj.Geometry());
        if (null != mesh)
          return GetMeshVolume(mesh);
      }
      return 0.0;
    }

    private static double GetCurveArea(IOnCurve crv, double tol)
    {
      double area = 0.0;
      if (null != crv && crv.IsClosed())
      {
        OnPlane plane = new OnPlane();
        if (crv.IsPlanar(plane, tol))
        {
          OnBoundingBox bbox = crv.BoundingBox();
          On3dPoint point = plane.ClosestPointTo(bbox.Center());
          OnMassProperties mp = new OnMassProperties();
          if (crv.AreaMassProperties(point, plane.Normal(), ref mp))
            area = Math.Abs(mp.Area());
        }
      }
      return area;
    }

    private static double GetSurfaceArea(IOnSurface srf)
    {
      double area = 0.0;
      if (null != srf)
      {
        OnMassProperties mp = new OnMassProperties();
        if (srf.AreaMassProperties(ref mp, true))
          area = Math.Abs(mp.Area());
      }
      return area;
    }

    private static double GetBrepArea(IOnBrep brep)
    {
      double area = 0.0;
      if (null != brep)
      {
        OnMassProperties mp = new OnMassProperties();
        if (brep.AreaMassProperties(ref mp, true))
          area = Math.Abs(mp.Area());
      }
      return area;
    }

    private static double GetMeshArea(IOnMesh mesh)
    {
      double area = 0.0;
      if (null != mesh)
      {
        OnMassProperties mp = new OnMassProperties();
        if (mesh.AreaMassProperties(ref mp, true))
          area = Math.Abs(mp.Area());
      }
      return area;
    }

    private static double GetSurfaceVolume(IOnSurface srf)
    {
      double volume = 0.0;
      if (null != srf)
      {
        OnMassProperties mp = new OnMassProperties();
        if (srf.VolumeMassProperties(ref mp, true))
          volume = Math.Abs(mp.Volume());
      }
      return volume;
    }

    private static double GetBrepVolume(IOnBrep brep)
    {
      double volume = 0.0;
      if (null != brep)
      {
        OnMassProperties mp = new OnMassProperties();
        if (brep.VolumeMassProperties(ref mp, true))
          volume = Math.Abs(mp.Volume());
      }
      return volume;
    }

    private static double GetMeshVolume(IOnMesh mesh)
    {
      double volume = 0.0;
      if (null != mesh)
      {
        OnMassProperties mp = new OnMassProperties();
        if (mesh.VolumeMassProperties(ref mp, true))
          volume = Math.Abs(mp.Volume());
      }
      return volume;
    }
  }
}