using System;
using System.Collections.Generic;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsMeshVolume : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{3eccbcf3-1298-4b07-9415-40bb7e917c1f}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsMeshVolume";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select solid meshes for volume calculation");
      go.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.mesh_object);
      go.SetGeometryAttributeFilter(IRhinoGetObject.GEOMETRY_ATTRIBUTE_FILTER.closed_mesh);
      go.EnableSubObjectSelect(false);
      go.EnableGroupSelect();
      go.GetObjects(1, 0);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      List<IOnMesh> meshes = new List<IOnMesh>();
      for (int i = 0; i < go.ObjectCount(); i++)
      {
        IOnMesh mesh = go.Object(i).Mesh();
        if (mesh != null)
          meshes.Add(mesh);
      }
      if (meshes.Count == 0)
        return IRhinoCommand.result.nothing;

      OnBoundingBox bbox = new OnBoundingBox();
      for (int i = 0; i < meshes.Count; i++)
        meshes[i].GetBoundingBox(ref bbox, 1);
      On3dPoint base_point = bbox.Center();

      double total_volume = 0.0;
      double total_error_estimate = 0.0;
      string msg;
      for (int i = 0; i < meshes.Count; i++)
      {
        double error_estimate = 0.0;
        double volume = meshes[i].Volume(base_point, ref error_estimate);
        msg = string.Format("Mesh {0} = {1:f} (+/- {2:f}\n", i, volume, error_estimate);
        RhUtil.RhinoApp().Print(msg);
        total_volume += volume;
        total_error_estimate += error_estimate;
      }

      msg = string.Format("Total volume = {0:f} (+/- {1:f})\n",
                          total_volume,
                          total_error_estimate);
      RhUtil.RhinoApp().Print(msg);

      return IRhinoCommand.result.success;
    }
  }
}

