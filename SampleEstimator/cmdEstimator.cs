/////////////////////////////////////////////////////////////////////////////
// EstimatorCommand.cs

using RMA.Rhino;
using RMA.OpenNURBS;

namespace Estimator
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// A command wizard can be found in visual studio when adding a new item to the project.
  /// </summary>
  public class cmdEstimator : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{1c05f5a6-cf1c-435b-8895-6cf9cdfeddfa}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "Estimator";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      RhUtil.RhinoApp().Print("Estimator plug-in loaded.\n");
      return IRhinoCommand.result.success;
    }
  }
}

