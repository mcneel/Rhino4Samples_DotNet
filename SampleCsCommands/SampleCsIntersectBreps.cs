using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsIntersectBreps : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{7b026e38-ce0c-418d-833e-2e802f2c032a}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsIntersectBreps";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select two surfaces or polysurfacs to intersect");
      go.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.surface_object);
      go.EnableSubObjectSelect(false);
      go.GetObjects(2, 2);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      IOnBrep B0 = go.Object(0).Brep();
      IOnBrep B1 = go.Object(1).Brep();
      if (null == B0 || null == B1)
        return IRhinoCommand.result.failure;

      OnCurve[] curves = null;
      On3dPointArray points = null;
      bool rc = RhUtil.RhinoIntersectBreps(B0, B1, context.m_doc.AbsoluteTolerance(), out curves, out points);
      if (
        false == rc        || 
        null == curves     ||
        0 == curves.Length ||
        null == points     ||
        0 == curves.Length 
        )
      {
        RhUtil.RhinoApp().Print("No intersections found.\n");
        return IRhinoCommand.result.nothing;
      }

      if (null != curves)
      {
        for (int i = 0; i < curves.Length; i++)
          context.m_doc.AddCurveObject(curves[i]);
      }

      if (null != points)
      {
        for (int i = 0; i < points.Count(); i++)
          context.m_doc.AddPointObject(points[i]);
      }

      context.m_doc.Redraw();

      return IRhinoCommand.result.success;
    }
  }
}

