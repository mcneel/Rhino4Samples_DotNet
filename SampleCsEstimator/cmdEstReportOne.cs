using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace Estimator
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class cmdEstReportOne : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{c0f5c392-977c-4b08-9744-b2e68c415348}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "EstReportOne";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select tagged object to report");
      go.EnableSubObjectSelect(false);
      go.GetObjects(1, 1);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      IRhinoObject obj = go.Object(0).Object();
      if (null == obj)
        return IRhinoCommand.result.failure;

      IOnGeometry geo = obj.Geometry();
      if (null == geo)
        return IRhinoCommand.result.failure;

      string[] string_array = null;
      if (0 == EstimatorHelpers.GetData(obj, ref string_array))
      {
        RhUtil.RhinoApp().Print("No Estimator tag data found.\n");
        return IRhinoCommand.result.nothing;
      }

      string filename = null;

      SaveFileDialog sd = new SaveFileDialog();
      sd.DefaultExt = "xml";
      sd.Filter = "XML file (*.xml)|*.xml|All files (*.*)|*.*";
      sd.AddExtension = true;
      sd.RestoreDirectory = true;
      sd.Title = "Save";
      if (sd.ShowDialog() == DialogResult.OK)
        filename = sd.FileName;
      sd.Dispose();
      sd = null;

      if (null == filename)
        return IRhinoCommand.result.cancel;

      XmlTextWriter writer = new XmlTextWriter(filename, Encoding.UTF8);
      writer.Formatting = Formatting.Indented;
      writer.WriteStartDocument();
      writer.WriteComment("Saved on " + DateTime.Now);

      // Write root element
      writer.WriteStartElement("Estimator");
      writer.WriteAttributeString("Version", "1.0");

      // Write object
      writer.WriteStartElement("Object");
      writer.WriteAttributeString("Type", geo.ObjectType().ToString());
      writer.WriteElementString("Uuid", obj.Attributes().m_uuid.ToString());
      if (obj.Attributes().m_name.Length > 0)
        writer.WriteElementString("Name", obj.Attributes().m_name);
      else
        writer.WriteElementString("Name", "(none)");

      // Write object length
      double length = EstimatorHelpers.GetLength(obj);
      if (length > 0.0)
        writer.WriteElementString("Length", length.ToString());
      else
        writer.WriteElementString("Length", "n/a");

      double tol = context.m_doc.AbsoluteTolerance();

      // Write object area
      double area = EstimatorHelpers.GetArea(obj, tol);
      if (area > 0.0)
        writer.WriteElementString("Area", area.ToString());
      else
        writer.WriteElementString("Area", "n/a");

      // Write object volume
      double volume = EstimatorHelpers.GetVolume(obj);
      if (volume > 0.0)
        writer.WriteElementString("Volume", volume.ToString());
      else
        writer.WriteElementString("Volume", "n/a");

      // Write object tags
      writer.WriteStartElement("Tags");
      for (int i = 0; i < string_array.Length; i++)
        writer.WriteElementString("Tag", string_array[i]);

      writer.WriteEndElement(); // Tags

      writer.WriteEndElement(); // Object

      writer.WriteEndElement(); // Estimator

      writer.WriteEndDocument();
      writer.Flush();
      writer.Close();

      return IRhinoCommand.result.success;
    }
  }
}

