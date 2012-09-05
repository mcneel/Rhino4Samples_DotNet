using RMA.Rhino;
using RMA.OpenNURBS;

namespace ZAnalysis
{
  public class ZAnalysisOn : RMA.Rhino.MRhinoCommand
  {
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{6fa032de-49eb-429e-b1d1-6334c5181181}");
    }

    public override string EnglishCommandName()
    {
      return "ZAnalysisOn";
    }

    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select objects for Z analysis");
      go.SetGeometryFilter(
        IRhinoGetObject.GEOMETRY_TYPE_FILTER.surface_object |
        IRhinoGetObject.GEOMETRY_TYPE_FILTER.polysrf_object |
        IRhinoGetObject.GEOMETRY_TYPE_FILTER.mesh_object
        );
      go.GetObjects(1, 0);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      int count = 0;
      for (int i = 0; i < go.ObjectCount(); i++)
      {
        IRhinoObject obj = go.Object(i).Object();
        if (null == obj)
          continue;

        if (obj.InAnalysisMode(ZAnalysisVAM.ZANALYSIS_VAM_ID))
          // This object is already in Z analysis mode
          continue;

        if (obj.EnableAnalysisMode(ZAnalysisVAM.ZANALYSIS_VAM_ID, true))
          // A new object is in Z analysis mode
          count++;
      }

      context.m_doc.Redraw();
      RhUtil.RhinoApp().Print(string.Format("{0} objects were put into Z-Analysis mode.\n", count));

      return IRhinoCommand.result.success;
    }
  }
}

