using System;
using System.Collections.Generic;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleExportMesh
{
  ///<summary>
  /// Every Rhino.NET Plug-In must have one and only one MRhinoPlugIn derived
  /// class. DO NOT create an instance of this class. It is the responsibility
  /// of Rhino.NET to create an instance of this class and register it with Rhino.
  ///</summary>
  public class SampleExportMeshPlugIn : RMA.Rhino.MRhinoFileExportPlugIn
  {
    OnMeshParameters _mesh_parameters;
    int _mesh_ui_style;

    /// <summary>
    /// Public constructor
    /// </summary>
    public SampleExportMeshPlugIn()
    {
      _mesh_parameters = new OnMeshParameters();
      _mesh_ui_style = 0; // 0 = simple dialog 
    }

    ///<summary>
    /// Rhino tracks plug-ins by their unique ID. Every plug-in must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    /// </summary>
    public override System.Guid PlugInID()
    {
      return new System.Guid("{9f10dc93-e1f9-47bf-9a76-368fbe31618c}");
    }

    /// <summary>
    /// Plug-in name display string. This name is displayed by Rhino when
    /// loading the plug-in, in the plug-in help menu, and in the Rhino 
    /// interface for managing plug-ins.
    /// </summary>
    public override string PlugInName()
    {
      return "SampleExportMesh";
    }

    /// <summary>
    /// Plug-in version display string. This name is displayed by Rhino 
    /// when loading the plug-in and in the Rhino interface for managing
    /// plug-ins.
    /// </summary>
    public override string PlugInVersion()
    {
      return "1.0.0.0";
    }

    ///<summary>
    /// Called after the plug-in is loaded and the constructor has been run.
    /// This is a good place to perform any significant initialization,
    /// license checking, and so on.  This function must return 1 for
    /// the plug-in to continue to load.
    ///</summary>
    ///<returns>
    ///  1 = initialization succeeded, let the plug-in load
    ///  0 = unable to initialize, don't load plug-in and display an error dialog
    /// -1 = unable to initialize, don't load plug-in and do not display an error
    ///      dialog. Note: OnUnloadPlugIn will not be called
    ///</returns>
    public override int OnLoadPlugIn()
    {
      return 1;
    }

    ///<summary>
    /// Called when the plug-in is about to be unloaded.  After this
    /// function is called, the plug-in will be disposed.
    ///</summary>
    public override void OnUnloadPlugIn()
    {
      // TODO: Add plug-in cleanup code here.
    }

    /// <summary>
    /// When Rhino gets ready to display either the save or export file dialog,
    /// it calls AddFileType() once for each loaded file export plug-in.
    /// </summary>
    public override void AddFileType(ref ArrayMRhinoFileType extensions, ref IRhinoFileWriteOptions opts)
    {
      MRhinoFileType filetype = new MRhinoFileType(PlugInID(), "Sample Mesh (*.mesh)", "mesh");
      extensions.Append(filetype);
    }

    /// <summary>
    /// Rhino calls WriteFile() to write document geometry to an external file.
    /// </summary>
    public override int WriteFile(string filename, int index, ref MRhinoDoc doc, ref IRhinoFileWriteOptions options)
    {
      int rc = 0; // false

      // Are we saving or exporting?
      bool bExport = options.Mode(IRhinoFileWriteOptions.ModeFlag.SelectedMode );
      // Are we in interactive or scripted mode?
      bool bScript = options.Mode(IRhinoFileWriteOptions.ModeFlag.BatchMode);

      List<IRhinoObject> objects = new List<IRhinoObject>();

      // Get objects to save/export
      MRhinoObjectIterator it = new MRhinoObjectIterator(doc, IRhinoObjectIterator.object_state.undeleted_objects);
      if (bExport)
      {
        it.EnableSelectedFilter();
        it.EnableVisibleFilter();
      }

      // Do the iteration...
      MRhinoObject obj = null;
      for (obj = it.First(); null != obj; obj = it.Next())
        objects.Add(obj);

      ArrayMRhinoObjectMesh meshes = new ArrayMRhinoObjectMesh(objects.Count);
      OnMeshParameters mesh_parameters = _mesh_parameters;
      int mesh_ui_style = ( bScript ) ? 2 : _mesh_ui_style;

      // Get the meshes to save/export
      IRhinoCommand.result res = RhUtil.RhinoMeshObjects(objects.ToArray(), ref mesh_parameters, ref mesh_ui_style, ref meshes);
      if ( res == IRhinoCommand.result.success)
      {
        if (mesh_ui_style >= 0 && mesh_ui_style <= 1)
          _mesh_ui_style = mesh_ui_style;
        _mesh_parameters = mesh_parameters;
      }
      else 
      {
        if (bExport)
          RhUtil.RhinoApp().Print("No meshes to export.\n");
        else
          RhUtil.RhinoApp().Print("No meshes to save.\n");
        return rc;
      }

      try
      {
        // Open the file
        System.IO.StreamWriter file = new System.IO.StreamWriter(filename);

        // Write mesh count
        file.WriteLine(string.Format("meshcount={0}\n", meshes.Count()));

        // Write each mesh
        for (int i = 0; i < meshes.Count(); i++)
        {
          MRhinoObjectMesh obj_mesh = meshes[i];
          OnMesh mesh = obj_mesh.GetMesh();
          if (null != mesh)
          {
            // Write mesh number
            file.WriteLine(string.Format("mesh={0}\n", i));

            // Write mesh vertex count
            file.WriteLine(string.Format("vertexcount={0}\n", mesh.m_V.Count()));

            // Write mesh face count
            file.WriteLine(string.Format("facecount={0}\n", mesh.m_F.Count()));

            // Write mesh vertices
            for (int vi = 0; vi < mesh.m_V.Count(); vi++)
            {
              On3fPoint p = mesh.m_V[vi];
              file.WriteLine(string.Format("vertex=({0},{1},{2})\n", p.x, p.y, p.z));
            }

            // Write mesh faces
            for (int fi = 0; fi < mesh.m_F.Count(); fi++)
            {
              OnMeshFace f = mesh.m_F[fi];
              file.WriteLine(string.Format("face=({0},{1},{2},{3})\n", f.get_vi(0), f.get_vi(1), f.get_vi(2), f.get_vi(3)));
            }
          }
        }

        file.Close();

        rc = 1; // true
      }
      catch(Exception e)
      {
        RhUtil.RhinoApp().Print(string.Format("{0}\n", e.Message));
      }

      return rc;
    }

  }
}
