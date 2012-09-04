using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsPrintSurfacePoints : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{18749982-55ed-4205-bb1e-3b41dd575e5f}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsPrintSurfacePoints";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select surface");
      go.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.surface_object);
      go.EnableSubObjectSelect(false);
      go.GetObjects(1, 1);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      IOnSurface srf = go.Object(0).Surface();
      if (null == srf)
        return IRhinoCommand.result.failure;

      IOnNurbsSurface ns = srf.NurbsSurface();
      if (null == ns)
      {
        RhUtil.RhinoApp().Print("Not a NURBS surface.\n");
        return IRhinoCommand.result.nothing;
      }

      On3dPoint cv = new On3dPoint();
      string str = string.Empty;
      for (int u = 0; u < ns.CVCount(0); u++)
      {
        for (int v = 0; v < ns.CVCount(1); v++)
        {
          if (ns.GetCV(u, v, ref cv))
          {
            str = string.Empty;
            RhUtil.RhinoFormatPoint(cv, ref str);
            RhUtil.RhinoApp().Print(string.Format("CV({0},{1}) = {2}\n", u, v, str));
          }
        }
      }

      return IRhinoCommand.result.success;
    }
  }
}

