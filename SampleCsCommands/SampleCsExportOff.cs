using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsExportOff
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// A command wizard can be found in visual studio when adding a new item to the project.
  /// </summary>
  public class SampleCsExportOff : RMA.Rhino.MRhinoCommand
  {
    OnMeshParameters _mesh_parameters;
    int _mesh_ui_style;

    public SampleCsExportOff()
    {
      _mesh_parameters = new OnMeshParameters();
      _mesh_ui_style = 0; // 0 = simple dialog 
    }

    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{2b4da4e6-84f3-4fa2-be33-792eb28abe87}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsExportOff";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select surface, polysurface, or mesh to export");
      go.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.surface_object | IRhinoGetObject.GEOMETRY_TYPE_FILTER.polysrf_object | IRhinoGetObject.GEOMETRY_TYPE_FILTER.mesh_object);
      go.GetObjects(1, 1);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      MRhinoObjRef obj_ref = go.Object(0);

      IRhinoObject obj = obj_ref.Object();
      if (null == obj)
        return IRhinoCommand.result.failure;

      List<IRhinoObject> obj_list = new List<IRhinoObject>();
      obj_list.Add(obj);

      ArrayMRhinoObjectMesh mesh_list = new ArrayMRhinoObjectMesh(obj_list.Count);
      OnMeshParameters mesh_parameters = _mesh_parameters;
      int mesh_ui_style = !context.IsInteractive() ? 2 : _mesh_ui_style;

      IRhinoCommand.result res = RhUtil.RhinoMeshObjects(obj_list.ToArray(), ref mesh_parameters, ref mesh_ui_style, ref mesh_list);
      if (res == IRhinoCommand.result.success)
      {
        if (mesh_ui_style >= 0 && mesh_ui_style <= 1)
          _mesh_ui_style = mesh_ui_style;
        _mesh_parameters = mesh_parameters;
      }
      else
      {
        RhUtil.RhinoApp().Print("No mesh to export.\n");
        return res;
      }

      string filename = string.Empty;

      if (context.IsInteractive())
      {
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Title = "Export";
        dialog.Filter = "Geomview files|*.off";
        dialog.InitialDirectory = DirectoryManager.DefaultDirectory(DirectoryManager.FileTypes.ftExport);
        if (dialog.ShowDialog() != DialogResult.OK)
          return IRhinoCommand.result.cancel;

        filename = dialog.FileName;
      }
      else
      {
        MRhinoGetString gs = new MRhinoGetString();
        gs.SetCommandPrompt("Export file name");
        gs.GetString();
        if (gs.CommandResult() != IRhinoCommand.result.success)
          return gs.CommandResult();

        filename = gs.String().Trim();
      }

      try
      {
        OnMesh mesh = mesh_list.First().GetMesh();

        int vertex_count = mesh.VertexCount();
        int face_count = mesh.FaceCount();
        int edge_count = mesh.Topology().m_tope.Count();

        System.IO.StreamWriter file = new System.IO.StreamWriter(filename);

        // Write out the first line of the file header
        file.WriteLine("OFF");

        // Write the header information 
        file.WriteLine(string.Format("{0} {1} {2}", vertex_count, face_count, edge_count));
        
        file.WriteLine();

        // Write out all the vertices in order
        for (int i = 0; i < vertex_count; i++)
        {
          On3fPoint p = mesh.m_V[i];
          file.WriteLine(string.Format("{0} {1} {2}", p.x.ToString("F"), p.y.ToString("F"), p.z.ToString("F")));
        }

        file.WriteLine();

        // Write out all the faces
        for (int i = 0; i < face_count; i++)
        {
          OnMeshFace f = mesh.m_F[i];
          if (f.IsQuad())
            file.WriteLine(string.Format("4 {0} {1} {2} {3}", f.get_vi(0), f.get_vi(1), f.get_vi(2), f.get_vi(3)));
          else
            file.WriteLine(string.Format("3 {0} {1} {2}", f.get_vi(0), f.get_vi(1), f.get_vi(2)));
        }

        file.Close();
      }
      catch (Exception e)
      {
        RhUtil.RhinoApp().Print(string.Format("{0}\n", e.Message));
      }
      
      return IRhinoCommand.result.success;
    }
  }
}

