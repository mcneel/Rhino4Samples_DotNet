using System;
using System.Collections.Generic;
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
  public class cmdEstReportAll : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{089c4435-762e-48e8-a304-55ab7ea5090c}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "EstReportAll";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      List<MRhinoObject> object_list = new List<MRhinoObject>();

      MRhinoObjectIterator it = new MRhinoObjectIterator(
        IRhinoObjectIterator.object_state.undeleted_objects,
        IRhinoObjectIterator.object_category.active_and_reference_objects);

      foreach (MRhinoObject obj in it)
      {
        string data = null;
        if (EstimatorHelpers.GetData(obj, ref data) > 0)
          object_list.Add(obj);
      }

      if (0 == object_list.Count)
      {
        RhUtil.RhinoApp().Print("No objects with Estimator tag data found.\n");
        return IRhinoCommand.result.nothing;
      }

      string filename = null;

      SaveFileDialog sd = new SaveFileDialog();
      sd.DefaultExt = "csv";
      sd.Filter = "CSV file (*.csv)|*.csv|XML file (*.xml)|*.xml";
      sd.AddExtension = true;
      sd.RestoreDirectory = true;
      sd.Title = "Save";
      if (sd.ShowDialog() == DialogResult.OK)
        filename = sd.FileName;
      sd.Dispose();
      sd = null;

      if (null == filename)
        return IRhinoCommand.result.cancel;

      bool bXml = false;
      if (Path.GetExtension(filename) == ".xml")
        bXml = true;

      if (bXml)
      {
        XmlTextWriter writer = new XmlTextWriter(filename, Encoding.UTF8);
        writer.Formatting = Formatting.Indented;
        writer.WriteStartDocument();
        writer.WriteComment("Saved on " + DateTime.Now);

        // Write root element
        writer.WriteStartElement("Estimator");
        writer.WriteAttributeString("Version", "1.0");

        // Write objects element
        writer.WriteStartElement("Objects");
        writer.WriteAttributeString("Count", object_list.Count.ToString());

        for (int i = 0; i < object_list.Count; i++)
        {
          MRhinoObject obj = object_list[i];
          if (null == obj)
            continue;

          IOnGeometry geo = obj.Geometry();
          if (null == geo)
            continue;

          string[] string_array = null;
          if (0 == EstimatorHelpers.GetData(obj, ref string_array))
            continue;

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
          for (int j = 0; j < string_array.Length; j++)
            writer.WriteElementString("Tag", string_array[j]);

          writer.WriteEndElement(); // Tags

          writer.WriteEndElement(); // Object
        }

        writer.WriteEndElement(); // Objects

        writer.WriteEndElement(); // Estimator

        writer.WriteEndDocument();
        writer.Flush();
        writer.Close();
      }
      else
      {
        TextWriter writer = new StreamWriter(filename);

        for (int i = 0; i < object_list.Count; i++)
        {
          MRhinoObject obj = object_list[i];
          if (null == obj)
            continue;

          IOnGeometry geo = obj.Geometry();
          if (null == geo)
            continue;

          string[] string_array = null;
          if (0 == EstimatorHelpers.GetData(obj, ref string_array))
            continue;

          StringBuilder sb = new StringBuilder();
          sb.Append(geo.ObjectType().ToString());
          sb.Append(",");
          sb.Append(obj.Attributes().m_uuid.ToString());
          sb.Append(",");

          double length = EstimatorHelpers.GetLength(obj);
          if (length > 0.0)
            sb.Append(length.ToString());
          else
            sb.Append("n/a");
          sb.Append(",");

          double tol = context.m_doc.AbsoluteTolerance();
          double area = EstimatorHelpers.GetArea(obj, tol);
          if (area > 0.0)
            sb.Append(area.ToString());
          else
            sb.Append("n/a");
          sb.Append(",");

          double volume = EstimatorHelpers.GetVolume(obj);
          if (volume > 0.0)
            sb.Append(volume.ToString());
          else
            sb.Append("n/a");

          for (int j = 0; j < string_array.Length; j++)
          {
            sb.Append(",");
            sb.Append(string_array[j]);
          }

          writer.WriteLine(sb.ToString());
        }

        // close the stream
        writer.Close();
      }

      return IRhinoCommand.result.success;
    }
  }
}

