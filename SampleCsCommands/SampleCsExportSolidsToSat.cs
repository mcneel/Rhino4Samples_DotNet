using System;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsExportSolidsToSat : RMA.Rhino.MRhinoScriptCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{2b52507e-b945-46e5-973d-1d10b0fcefee}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsExportSolidsToSat";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      int count = 0;

      context.m_doc.UnselectAll();

      MRhinoObjectIterator it = new MRhinoObjectIterator(
        IRhinoObjectIterator.object_state.normal_objects, 
        IRhinoObjectIterator.object_category.active_objects
        );

      IRhinoObject obj = null;
      for (obj = it.First(); null != obj; obj = it.Next())
      {
        if (obj.IsSolid())
        {
          IOn.object_type type = obj.ObjectType();
          if (type == IOn.object_type.surface_object || type == IOn.object_type.brep_object)
          {
            obj.Select(true);

            string fname = string.Format("{0}\\rhino_sat_export_{1}.sat", path, count++);
            string script = string.Format("_-Export \"{0}\" Inventor _Enter", fname);
            RhUtil.RhinoApp().RunScript(script, 1);

            obj.Select(false);
          }
        }
      }

      context.m_doc.Redraw();

     
      return IRhinoCommand.result.success;
    }
  }
}

