using RMA.Rhino;
using RMA.OpenNURBS;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Estimator
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class cmdEstTagItem : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{aecde586-2dd8-4636-99fe-c4c362c5a02a}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "EstTagItem";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select objects to apply item tag");
      go.EnableSubObjectSelect(false);
      go.GetObjects(1, 0);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      EstimatorTagForm form = new EstimatorTagForm();
      form.m_str_title = "Add Item Tag";
      form.m_str_prompt = "Select an item tag.";
      form.m_type = EstimatorTag.tag_type.item_tag;

      DialogResult rc = form.ShowDialog();
      if (rc == DialogResult.Cancel)
        return IRhinoCommand.result.cancel;

      List<string> tag_strings = new List<string>(form.m_selected_tags.Count);

      EstimatorPlugIn plugin = RMA.Rhino.RhUtil.GetPlugInInstance() as EstimatorPlugIn;
      int i = 0;

      for (i = 0; i < form.m_selected_tags.Count; i++)
      {
        int index = form.m_selected_tags[i];
        tag_strings.Add(plugin.m_tag_table[index].Id());
      }

      for (i = 0; i < go.ObjectCount(); i++)
      {
        EstimatorHelpers.RemoveAllData(go.Object(i).Object());
        EstimatorHelpers.AddData(go.Object(i).Object(), tag_strings.ToArray());
      }

      return IRhinoCommand.result.success;
    }
  }
}

