using System;
using System.Collections;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsPrintCurvePoints : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{ac1dd7af-7a23-4a6c-afb3-b155a6aac4e8}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsPrintCurvePoints";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select curve");
      go.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.curve_object);
      go.EnableSubObjectSelect(false);
      go.GetObjects(1, 1);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      IOnCurve curve = go.Object(0).Curve();
      if (null == curve)
        return IRhinoCommand.result.failure;

      // Several types of OnCurve objects can have the form of a polyline,
      // including OnLineCurve, a degree 1 OnNurbsCurve, ON_PolylineCurve,
      // and ON_PolyCurve (whose segments form a polyline). OnCurve.IsPolyline
      // tests a curve to see if it can be represented as a polyline.
      ArrayOn3dPoint points = new ArrayOn3dPoint(64);
      int point_count = curve.IsPolyline(points);
      if (point_count > 0)
      {
        string point_str = string.Empty;
        for (int i = 0; i < point_count; i++)
        {
          point_str = string.Empty;
          RhUtil.RhinoFormatPoint(points[i], ref point_str);
          RhUtil.RhinoApp().Print(string.Format("Point {0} = {1}\n", i, point_str));
        }
      }

      return IRhinoCommand.result.success;
    }
  }
}

