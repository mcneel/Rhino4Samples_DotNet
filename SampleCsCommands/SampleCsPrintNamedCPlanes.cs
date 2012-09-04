using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsPrintNamedCPlanes : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{bbe2fde1-77c2-40f0-8a9d-f4e2d153c669}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsPrintNamedCPlanes";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      for (int i = 0; ; i++)
      {
        IOn3dmConstructionPlane cplane = context.m_doc.Properties().NamedConstructionPlane(i);
        if (null == cplane)
          break;
        else
          RhUtil.RhinoApp().Print(string.Format("Named CPlane {0} = {1}\n", i, cplane.m_name));
      }
      return IRhinoCommand.result.success;
    }
  }
}

