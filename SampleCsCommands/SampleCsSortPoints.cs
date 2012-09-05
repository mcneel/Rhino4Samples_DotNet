using System;
using System.Collections.Generic;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  /// <summary>
  /// Utility array class used for sorting 3-D points
  /// </summary>
  class Rhino3dPointList : List<RMA.OpenNURBS.ValueTypes.Point3d>
  {
    // Default constructor
    public Rhino3dPointList()
    {
    }

    // Copy constructor
    public Rhino3dPointList(Rhino3dPointList source)
    {
      this.AddRange(source);
    }

    // Construct from a RMA.OpenNURBS.On3dPointArray object
    public Rhino3dPointList(On3dPointArray points)
    {
      Add(points);
    }

    // Adds a point from coordinates
    public void Add(double x, double y, double z)
    {
      Add(new RMA.OpenNURBS.ValueTypes.Point3d(x, y, z));
    }

    // Adds a point from a RMA.OpenNURBS.On3dPoint object
    public void Add(On3dPoint pt)
    {
      Add(pt.x, pt.y, pt.z);
    }

    // Adds one or more points from a RMA.OpenNURBS.On3dPointArray object
    public void Add(On3dPointArray points)
    {
      if (points != null)
      {
        int point_count = points.Count();
        int total_count = this.Count + point_count;
        this.Capacity = total_count;
        for (int i = 0; i < point_count; i++)
          Add(points.get_ValueAt(i));
      }
    }

    // Returns a new RMA.OpenNURBS.On3dPointArray object
    public On3dPointArray ToPointArray()
    {
      int count = this.Count;
      On3dPointArray points = new On3dPointArray(count);
      for (int i = 0; i < count; i++)
        points.Append(this[i]);
      return points;
    }

    // Prints the contents of the array to the Rhino command line
    public void Print()
    {
      for (int i = 0; i < Count; i++)
        MRhinoApp.WriteLine(this[i].ToString());
    }

    public enum sort_method
    {
      sort_xyz = 0,
      sort_xzy,
      sort_yxz,
      sort_yzx,
      sort_zxy,
      sort_zyx
    };

    // Sorts the array
    public void Sort(Rhino3dPointList.sort_method method, bool bAscending)
    {
      if (this.Count >= 2)
      {
        switch (method)
        {
          case Rhino3dPointList.sort_method.sort_xyz:
            {
              if (bAscending)
                Sort(compare_points_ascending_xyz);
              else
                Sort(compare_points_descending_xyz);
            }
            break;

          case Rhino3dPointList.sort_method.sort_xzy:
            {
              if (bAscending)
                Sort(compare_points_ascending_xzy);
              else
                Sort(compare_points_descending_xzy);
            }
            break;

          case Rhino3dPointList.sort_method.sort_yxz:
            {
              if (bAscending)
                Sort(compare_points_ascending_yxz);
              else
                Sort(compare_points_descending_yxz);
            }
            break;

          case Rhino3dPointList.sort_method.sort_yzx:
            {
              if (bAscending)
                Sort(compare_points_ascending_yzx);
              else
                Sort(compare_points_descending_yzx);
            }
            break;

          case Rhino3dPointList.sort_method.sort_zxy:
            {
              if (bAscending)
                Sort(compare_points_ascending_zxy);
              else
                Sort(compare_points_descending_zxy);
            }
            break;

          case Rhino3dPointList.sort_method.sort_zyx:
            {
              if (bAscending)
                Sort(compare_points_ascending_zyx);
              else
                Sort(compare_points_descending_zyx);
            }
            break;
        }
      }
    }

    // Culls duplicate entries
    public void Cull(double tolerance)
    {
      int count = this.Count;
      if (count >= 2)
      {
        if (tolerance < 0.0)
          tolerance = 0.0;

        this.Sort(Rhino3dPointList.sort_method.sort_xyz, true);

        RMA.OpenNURBS.ValueTypes.Point3d pt = new RMA.OpenNURBS.ValueTypes.Point3d();
        for (int i = count - 1; i >= 0; i--)
        {
          if (pt.DistanceTo(this[i]) <= tolerance)
            this.RemoveAt(i);
          else
            pt = this[i];
        }
      }
    }

    private static int compare_points_ascending_xyz(RMA.OpenNURBS.ValueTypes.Point3d a, RMA.OpenNURBS.ValueTypes.Point3d b)
    {
      if ((a.X < b.X) ? true : ((a.X == b.X) ? ((a.Y < b.Y) ? true : (a.Y == b.Y && a.Z < b.Z) ? true : false) : false)) return -1;
      if ((b.X < a.X) ? true : ((b.X == a.X) ? ((b.Y < a.Y) ? true : (b.Y == a.Y && b.Z < a.Z) ? true : false) : false)) return 1;
      return 0;
    }

    private static int compare_points_ascending_xzy(RMA.OpenNURBS.ValueTypes.Point3d a, RMA.OpenNURBS.ValueTypes.Point3d b)
    {
      if ((a.X < b.X) ? true : ((a.X == b.X) ? ((a.Z < b.Z) ? true : (a.Z == b.Z && a.Y < b.Y) ? true : false) : false)) return -1;
      if ((b.X < a.X) ? true : ((b.X == a.X) ? ((b.Z < a.Z) ? true : (b.Z == a.Z && b.Y < a.Y) ? true : false) : false)) return 1;
      return 0;
    }

    private static int compare_points_ascending_yxz(RMA.OpenNURBS.ValueTypes.Point3d a, RMA.OpenNURBS.ValueTypes.Point3d b)
    {
      if ((a.Y < b.Y) ? true : ((a.Y == b.Y) ? ((a.X < b.X) ? true : (a.X == b.X && a.Z < b.Z) ? true : false) : false)) return -1;
      if ((b.Y < a.Y) ? true : ((b.Y == a.Y) ? ((b.X < a.X) ? true : (b.X == a.X && b.Z < a.Z) ? true : false) : false)) return 1;
      return 0;
    }

    private static int compare_points_ascending_yzx(RMA.OpenNURBS.ValueTypes.Point3d a, RMA.OpenNURBS.ValueTypes.Point3d b)
    {
      if ((a.Y < b.Y) ? true : ((a.Y == b.Y) ? ((a.Z < b.Z) ? true : (a.Z == b.Z && a.X < b.X) ? true : false) : false)) return -1;
      if ((b.Y < a.Y) ? true : ((b.Y == a.Y) ? ((b.Z < a.Z) ? true : (b.Z == a.Z && b.X < a.X) ? true : false) : false)) return 1;
      return 0;
    }

    private static int compare_points_ascending_zxy(RMA.OpenNURBS.ValueTypes.Point3d a, RMA.OpenNURBS.ValueTypes.Point3d b)
    {
      if ((a.Z < b.Z) ? true : ((a.Z == b.Z) ? ((a.X < b.X) ? true : (a.X == b.X && a.Y < b.Y) ? true : false) : false)) return -1;
      if ((b.Z < a.Z) ? true : ((b.Z == a.Z) ? ((b.X < a.X) ? true : (b.X == a.X && b.Y < a.Y) ? true : false) : false)) return 1;
      return 0;
    }

    private static int compare_points_ascending_zyx(RMA.OpenNURBS.ValueTypes.Point3d a, RMA.OpenNURBS.ValueTypes.Point3d b)
    {
      if ((a.Z < b.Z) ? true : ((a.Z == b.Z) ? ((a.Y < b.Y) ? true : (a.Y == b.Y && a.X < b.X) ? true : false) : false)) return -1;
      if ((b.Z < a.Z) ? true : ((b.Z == a.Z) ? ((b.Y < a.Y) ? true : (b.Y == a.Y && b.X < a.X) ? true : false) : false)) return 1;
      return 0;
    }

    private static int compare_points_descending_xyz(RMA.OpenNURBS.ValueTypes.Point3d a, RMA.OpenNURBS.ValueTypes.Point3d b)
    {
      if ((b.X < a.X) ? true : ((b.X == a.X) ? ((b.Y < a.Y) ? true : (b.Y == a.Y && b.Z < a.Z) ? true : false) : false)) return -1;
      if ((a.X < b.X) ? true : ((a.X == b.X) ? ((a.Y < b.Y) ? true : (a.Y == b.Y && a.Z < b.Z) ? true : false) : false)) return 1;
      return 0;
    }

    private static int compare_points_descending_xzy(RMA.OpenNURBS.ValueTypes.Point3d a, RMA.OpenNURBS.ValueTypes.Point3d b)
    {
      if ((b.X < a.X) ? true : ((b.X == a.X) ? ((b.Z < a.Z) ? true : (b.Z == a.Z && b.Y < a.Y) ? true : false) : false)) return -1;
      if ((a.X < b.X) ? true : ((a.X == b.X) ? ((a.Z < b.Z) ? true : (a.Z == b.Z && a.Y < b.Y) ? true : false) : false)) return 1;
      return 0;
    }

    private static int compare_points_descending_yxz(RMA.OpenNURBS.ValueTypes.Point3d a, RMA.OpenNURBS.ValueTypes.Point3d b)
    {
      if ((b.Y < a.Y) ? true : ((b.Y == a.Y) ? ((b.X < a.X) ? true : (b.X == a.X && b.Z < a.Z) ? true : false) : false)) return -1;
      if ((a.Y < b.Y) ? true : ((a.Y == b.Y) ? ((a.X < b.X) ? true : (a.X == b.X && a.Z < b.Z) ? true : false) : false)) return 1;
      return 0;
    }

    private static int compare_points_descending_yzx(RMA.OpenNURBS.ValueTypes.Point3d a, RMA.OpenNURBS.ValueTypes.Point3d b)
    {
      if ((b.Y < a.Y) ? true : ((b.Y == a.Y) ? ((b.Z < a.Z) ? true : (b.Z == a.Z && b.X < a.X) ? true : false) : false)) return -1;
      if ((a.Y < b.Y) ? true : ((a.Y == b.Y) ? ((a.Z < b.Z) ? true : (a.Z == b.Z && a.X < b.X) ? true : false) : false)) return 1;
      return 0;
    }

    private static int compare_points_descending_zxy(RMA.OpenNURBS.ValueTypes.Point3d a, RMA.OpenNURBS.ValueTypes.Point3d b)
    {
      if ((b.Z < a.Z) ? true : ((b.Z == a.Z) ? ((b.X < a.X) ? true : (b.X == a.X && b.Y < a.Y) ? true : false) : false)) return -1;
      if ((a.Z < b.Z) ? true : ((a.Z == b.Z) ? ((a.X < b.X) ? true : (a.X == b.X && a.Y < b.Y) ? true : false) : false)) return 1;
      return 0;
    }

    private static int compare_points_descending_zyx(RMA.OpenNURBS.ValueTypes.Point3d a, RMA.OpenNURBS.ValueTypes.Point3d b)
    {
      if ((b.Z < a.Z) ? true : ((b.Z == a.Z) ? ((b.Y < a.Y) ? true : (b.Y == a.Y && b.X < a.X) ? true : false) : false)) return -1;
      if ((a.Z < b.Z) ? true : ((a.Z == b.Z) ? ((a.Y < b.Y) ? true : (a.Y == b.Y && a.X < b.X) ? true : false) : false)) return 1;
      return 0;
    }
  }

  ///////////////////////////////////////////////////////////////////////////
  ///////////////////////////////////////////////////////////////////////////

  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsSortPoints : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{72af6823-629c-4bad-b78c-a2d7f3f13b1b}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsSortPoints";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      Rhino3dPointList points = new Rhino3dPointList();
      points.Add(new On3dPoint(0, 0, 0));
      points.Add(new On3dPoint(0, 0, 1));
      points.Add(new On3dPoint(0, 1, 0));
      points.Add(new On3dPoint(0, 1, 1));
      points.Add(new On3dPoint(1, 0, 0));
      points.Add(new On3dPoint(1, 0, 1));
      points.Add(new On3dPoint(1, 1, 0));
      points.Add(new On3dPoint(1, 1, 1));

      RhUtil.RhinoApp().Print("Before sort...\n");
      points.Print();

      points.Sort(Rhino3dPointList.sort_method.sort_xyz, true);
      RhUtil.RhinoApp().Print("Sort ascending...\n");
      points.Print();

      points.Sort(Rhino3dPointList.sort_method.sort_xyz, false);
      RhUtil.RhinoApp().Print("Sort descending...\n");
      points.Print();

      context.m_doc.AddPointCloudObject(points.ToPointArray());
      context.m_doc.Redraw(); 
      
      return IRhinoCommand.result.success;
    }
  }
}

