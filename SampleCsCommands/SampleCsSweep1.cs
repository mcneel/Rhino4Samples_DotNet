using System;
using System.Collections.Generic;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsSweep1 : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{561ed9eb-fafa-454f-a7fa-1e5977821d5c}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsSweep1";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      // Define geometry for rail and shape curves
      On3dPoint start_point = new On3dPoint(0, 0, 0);
      On3dPoint end_point = new On3dPoint(10, 0, 0);
      OnPlane plane = new OnPlane(OnUtil.On_yz_plane);

      plane.SetOrigin(start_point);
      OnCircle circle0 = new OnCircle(plane, 5);

      plane.SetOrigin(end_point);
      OnCircle circle1 = new OnCircle(plane, 2);

      // Build rail and shape curves
      OnLineCurve line_curve = new OnLineCurve(start_point, end_point);
      OnArcCurve arc_curve0 = new OnArcCurve(circle0);
      OnArcCurve arc_curve1 = new OnArcCurve(circle1);

      // Build poly edge curve
      MRhinoPolyEdge rail_curve = new MRhinoPolyEdge();
      rail_curve.Create(line_curve);

      // Create sweep arguments
      MArgsRhinoSweep1 args = new MArgsRhinoSweep1();

      // Add rail curve and related settings
      args.m_rail_curve = rail_curve;
      args.m_bHaveRailPickPoint = false;
      args.m_bClosed = false;
      args.m_bUsePivotPoint = false;

      // Add shape curves
      List<OnCurve> shape_curves = new List<OnCurve>();
      shape_curves.Add(arc_curve0);
      shape_curves.Add(arc_curve1);
      args.m_shape_curves = shape_curves.ToArray();

      // Specify parameters on rail closest to shapes
      args.m_rail_params.Append(rail_curve.Domain()[0]);
      args.m_rail_params.Append(rail_curve.Domain()[1]);

      // No starting sweep point
      args.set_m_bUsePoints(0, 0);
      // No ending sweep point
      args.set_m_bUsePoints(1, 0);
      // 0 = Freeform 
      args.m_style = 0;
      // 0 = Do Not Simplify
      args.m_simplify = 0;
      // Sample point count for rebuilding shapes
      args.m_rebuild_count = -1;
      // 0 = don't miter
      args.m_miter_type = 0;
      // Standard tolerances
      args.m_refit_tolerance = context.m_doc.AbsoluteTolerance();
      args.m_sweep_tolerance = context.m_doc.AbsoluteTolerance();
      args.m_angle_tolerance = context.m_doc.AngleToleranceRadians();

      OnBrep[] breps = null;
      if (RhUtil.RhinoSweep1(ref args, out breps))
      {
        for (int i = 0; i < breps.Length; i++)
          context.m_doc.AddBrepObject(breps[i]);
        context.m_doc.Redraw();
      }

      return IRhinoCommand.result.success;
    }
  }
}

