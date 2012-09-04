using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsFieldOfView : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{0eb98952-6bcc-41a9-bb5e-d723e8ecd2da}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsFieldOfView";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoView view = RhUtil.RhinoApp().ActiveView();
      if (null != view)
      {
        // 1/2 of smallest subtended view angle
        double half_smallest_angle = 0.0;
        if (view.ActiveViewport().VP().GetCameraAngle(ref half_smallest_angle))
        {
          string fov = string.Empty;
          RhUtil.RhinoFormatNumber(half_smallest_angle * (180.0 / OnUtil.On_PI), ref fov);
          RhUtil.RhinoApp().Print(string.Format("Field of view =  {0} degrees.\n", fov));
        }
      }
      return IRhinoCommand.result.success;
    }
  }
}

