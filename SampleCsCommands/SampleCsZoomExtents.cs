using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsZoomExtents : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{e76a7866-a3b3-454d-9bc7-15653921eb90}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsZoomExtents";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoView[] view_list = null;
      context.m_doc.GetViewList(ref view_list, true, false);
      if (null == view_list)
        return IRhinoCommand.result.failure;

      foreach (MRhinoView view in view_list)
        RhUtil.RhinoDollyExtentsSelected(view, true);

      return IRhinoCommand.result.success;
    }
  }
}

