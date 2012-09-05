using RMA.Rhino;
using RMA.OpenNURBS;

namespace ZAnalysis
{
  public class ZAnalysisOff : RMA.Rhino.MRhinoCommand
  {
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{11e7b591-9bec-4580-8c54-511c6f892a9e}");
    }

    public override string EnglishCommandName()
    {
      return "ZAnalysisOff";
    }

    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoObjectIterator it = new MRhinoObjectIterator(
        IRhinoObjectIterator.object_state.undeleted_objects, 
        IRhinoObjectIterator.object_category.active_and_reference_objects
        );

      for (MRhinoObject obj = it.First(); null != obj; obj = it.Next())
        obj.EnableAnalysisMode(ZAnalysisVAM.ZANALYSIS_VAM_ID, false);

      context.m_doc.Redraw();
      RhUtil.RhinoApp().Print("Z-Analysis is off.\n");

      return IRhinoCommand.result.success;
    }
  }
}

