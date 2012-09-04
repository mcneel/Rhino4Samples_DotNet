using RMA.Rhino;
using RMA.OpenNURBS;
using System;
using System.Collections.Generic;

namespace Estimator
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class cmdEstReportItemCounts : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{5689af8f-c756-45e9-8d63-96ba38006ead}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "EstReportItemCounts";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      EstimatorPlugIn plugin = RMA.Rhino.RhUtil.GetPlugInInstance() as EstimatorPlugIn;
      Dictionary<string, int> map = new Dictionary<string, int>();

      MRhinoObjectIterator it = new MRhinoObjectIterator(
        IRhinoObjectIterator.object_state.undeleted_objects,
        IRhinoObjectIterator.object_category.active_and_reference_objects);

      foreach (MRhinoObject obj in it)
      {
        string[] string_array = null;
        if (0 == EstimatorHelpers.GetData(obj, ref string_array))
          continue;

        for (int i = 0; i < string_array.Length; i++)
        {
          string tag = string_array[i];
          int index = plugin.m_tag_table.FindTag(tag);
          if (index >= 0)
          {
            if (plugin.m_tag_table.Tag(index).Type() == EstimatorTag.tag_type.item_tag)
            {
              if (!map.ContainsKey(tag))
                map.Add(tag, 1);
              else
                map[tag]++;
            }
          }
        }
      }

      if (map.Count > 0)
      {
        foreach (KeyValuePair<string, int> kvp in map)
          RhUtil.RhinoApp().Print(String.Format("Item = {0}, Count = {1}\n", kvp.Key, kvp.Value));
      }
      else
      {
        RhUtil.RhinoApp().Print("No Estimator item tag data found.\n");
      }

      
      return IRhinoCommand.result.success;
    }
  }
}

