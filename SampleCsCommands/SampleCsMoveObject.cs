using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsMoveObject : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{831a1cc5-bb50-4a28-a125-6f92dbfbdadc}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsMoveObject";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select object to move");
      go.EnableSubObjectSelect(false);
      go.GetObjects(1, 1);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      MRhinoGetPoint gp = new MRhinoGetPoint();
      gp.SetCommandPrompt("Point to move from");
      gp.GetPoint();
      if (gp.CommandResult() != IRhinoCommand.result.success)
        return gp.CommandResult();

      On3dPoint pointFrom = gp.Point();

      gp.SetCommandPrompt("Point to move to");
      gp.SetBasePoint(pointFrom);
      gp.DrawLineFromPoint(pointFrom, true);
      gp.GetPoint();
      if (gp.CommandResult() != IRhinoCommand.result.success)
        return gp.CommandResult();

      On3dPoint pointTo = gp.Point();

      On3dVector dir = new On3dVector(pointTo - pointFrom);
      if (dir.IsTiny())
        return IRhinoCommand.result.nothing;

      OnXform xform = new OnXform();
      xform.Translation(dir);

      MRhinoObjRef objectRef = go.Object(0);
      context.m_doc.TransformObject(ref objectRef, xform);
      context.m_doc.Redraw();

      return IRhinoCommand.result.success;
    }
  }
}

