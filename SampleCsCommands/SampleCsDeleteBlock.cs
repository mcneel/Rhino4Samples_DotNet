using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsDeleteBlock : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{75097c0f-1f08-40dd-8ce0-06845eadb4bd}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsDeleteBlock";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      // Prompt for the instance (block) name
      MRhinoGetString gs = new MRhinoGetString();
      gs.SetCommandPrompt("Name of block to delete");
      gs.GetString();
      if (gs.CommandResult() != IRhinoCommand.result.success)
        return gs.CommandResult();

      string idef_name = gs.String().Trim();
      if (string.IsNullOrEmpty(idef_name))
        return IRhinoCommand.result.nothing;

      // Find the instance definition by name
      int idef_index = context.m_doc.m_instance_definition_table.FindInstanceDefinition(idef_name, true);
      if (idef_index < 0 || idef_index >= context.m_doc.m_instance_definition_table.InstanceDefinitionCount())
      {
        RhUtil.RhinoApp().Print(string.Format("Block definition \"{0}\" not found.\n", idef_name));
        return IRhinoCommand.result.nothing;
      }

      // Verify the instance definition can be deleted
      IRhinoInstanceDefinition idef = context.m_doc.m_instance_definition_table[idef_index];
      if (idef.IsReference())
      {
        RhUtil.RhinoApp().Print(string.Format("Block definition \"{0}\" is from a reference file.\n", idef_name));
        return IRhinoCommand.result.nothing;
      }

      // Get all of instance references
      IRhinoInstanceObject[] iref_object_list = null;
      int iref_count = idef.GetReferences(out iref_object_list);

      // Try deleting the instance references
      int num_deleted = 0;
      for (int i = 0; i < iref_count; i++)
      {
        IRhinoInstanceObject iref = iref_object_list[i];
        if (null == iref)
          continue;

        if (iref.IsDeleted() || iref.IsReference())
          continue;

        MRhinoObjRef obj_ref = new MRhinoObjRef(iref_object_list[i]);
        if (context.m_doc.DeleteObject(obj_ref, true))
          num_deleted++;
      }

      if (num_deleted > 0)
        context.m_doc.Redraw();

      RhUtil.RhinoApp().Print(string.Format("{0} \"{1}\" block(s) found, {2} deleted.\n", iref_count, idef_name, num_deleted));

      // Try deleting the instance definition
      bool iref_deleted = false;
      if (num_deleted == iref_count)
      {
        if (context.m_doc.m_instance_definition_table.DeleteInstanceDefinition(idef_index, true, false))
          iref_deleted = true;
      }

      if (iref_deleted)
        RhUtil.RhinoApp().Print(string.Format("Block definition \"{0}\" deleted.\n", idef_name));
      else
        RhUtil.RhinoApp().Print(string.Format("Block definition \"{0}\" not deleted.\n", idef_name));

      return IRhinoCommand.result.success;
    }
  }
}

