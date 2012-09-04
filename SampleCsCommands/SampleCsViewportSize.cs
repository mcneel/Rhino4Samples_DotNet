using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsViewportSize : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{625e4657-3c2d-41f3-a05d-749ce1fcd562}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsViewportSize";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoView view = RhUtil.RhinoApp().ActiveView();
      if (null != view)
      {
        int left = 0, right = 0, bottom = 0, top = 0;
        view.ActiveViewport().VP().GetScreenPort(ref left, ref right, ref bottom, ref top);

        string name = view.ActiveViewport().Name();
        int width = right - left;
        int height = bottom - top;
        RhUtil.RhinoApp().Print(string.Format("Name = {0}: Width = {1}, Height = {2}\n", name, width, height));
      }

      return IRhinoCommand.result.cancel;
    }
  }
}

