using System;
using System.IO;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsReadPointsFile : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{d77f7c9a-43a9-4929-a23c-2dcea4e6ea55}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsReadPointsFile";
    }

    ///<summary> 
    /// This gets called when when the user runs this command.
    ///</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetFileDialog dlg = new MRhinoGetFileDialog();
      if (dlg.DisplayFileDialog(IRhinoGetFileDialog.file_dialog_type.open_text_file_dialog))
      {
        string filename = dlg.FileName();
        string line = "";
        RhUtil.RhinoApp().Print(string.Format("Opening file {0}.\n", filename));

        char[] delim = new char[2];
        delim[0] = ','; delim[1] = '\0';
        
        On3dPointArray points = new On3dPointArray(8);

        StreamReader reader = new StreamReader(filename);
        line = reader.ReadLine();

        while(line != null && line.Length > 0)
        {
          string[] substrs = line.Split(delim);
          if(substrs.Length == 3)
          {
            On3dPoint p = new On3dPoint();
            p.x = double.Parse(substrs[0]);
            p.y = double.Parse(substrs[1]);
            p.z = double.Parse(substrs[2]);
            points.Append(p);
          }
          line = reader.ReadLine();
        }

        OnNurbsCurve curve = RhUtil.RhinoInterpCurve(3, points, null, null, 1);
        context.m_doc.AddCurveObject(curve);
        reader.Close();
      }

      context.m_doc.Redraw();

      return IRhinoCommand.result.success;
    }
  }
}

