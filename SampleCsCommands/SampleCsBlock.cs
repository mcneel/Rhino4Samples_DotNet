using System;
using System.Collections.Generic;
using System.Text;
using RMA.Rhino;
using RMA.UI;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsBlock : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{33099bf2-b9d1-453e-a27a-12093b37465f}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsBlock";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      // Step 1, select objects to include in the instance definition
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select objects to define block");
      go.EnableReferenceObjectSelect(false);
      go.EnableSubObjectSelect(false);
      go.EnableGroupSelect(true);
      // Phantoms, grips, lights, etc., cannot be in instance definitions.
      uint forbidden_geometry_filter = (uint)(IRhinoGetObject.GEOMETRY_TYPE_FILTER.light_object |
                                              IRhinoGetObject.GEOMETRY_TYPE_FILTER.grip_object |
                                              IRhinoGetObject.GEOMETRY_TYPE_FILTER.phantom_object
                                              );
      uint geometry_filter = forbidden_geometry_filter ^ (uint)IRhinoGetObject.GEOMETRY_TYPE_FILTER.any_object;
      go.SetGeometryFilter(geometry_filter);
      go.GetObjects(1, 0);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      // Step 2, select base point
      MRhinoGetPoint gp = new MRhinoGetPoint();
      gp.SetCommandPrompt("Block base point");
      gp.GetPoint();
      if (gp.CommandResult() != IRhinoCommand.result.success)
        return gp.CommandResult();

      On3dPoint base_point = gp.Point();

      // Step 3, get instance definition name
      MRhinoGetString gs = new MRhinoGetString();
      gs.SetCommandPrompt("Block definition name");
      gs.SetDefaultString(GetUnusedInstanceDefinitionName(context.m_doc));
      gs.GetString();
      if (gs.CommandResult() != IRhinoCommand.result.success)
        return gs.CommandResult();

      string idef_name = gs.String().Trim();
      if (string.IsNullOrEmpty(idef_name))
        return IRhinoCommand.result.nothing;

      // Step 4, verify objects
      int found_index = context.m_doc.m_instance_definition_table.FindInstanceDefinition(idef_name);
      List<IRhinoObject> objects = new List<IRhinoObject>();

      bool bQuietly = context.IsInteractive() ? false : true;

      for (int i = 0; i < go.ObjectCount(); i++)
      {
        IRhinoObject obj = go.Object(i).Object();
        if (obj == null)
          continue;

        // Probably don't need to do this...
        if (0 != (forbidden_geometry_filter & (uint)obj.ObjectType()))
          continue;

        if (obj.ObjectType() == IOn.object_type.instance_reference)
        {
          IRhinoInstanceObject iref_obj = MRhinoInstanceObject.ConstCast(obj);
          if (iref_obj != null)
          {
            if (found_index >= 0 && iref_obj.UsesDefinition(found_index) > 0)
            {
              if (!bQuietly)
                RhUtil.RhinoApp().Print("Unable to create block.\n");
              return IRhinoCommand.result.failure;
            }
          }
        }
        objects.Add(obj);
      }

      // Step 5, create instance definition
      OnInstanceDefinition idef = new OnInstanceDefinition();
      idef.SetName(idef_name);

      int idef_index = CreateInstanceDefinition(context.m_doc, idef, base_point, objects, bQuietly);
      if (idef_index < 0)
        return IRhinoCommand.result.failure;

      // Step 6, create the instance reference
      OnXform xform = new OnXform();
      xform.Translation(base_point - new On3dPoint(OnUtil.On_origin));
      IRhinoInstanceObject inst_obj = context.m_doc.m_instance_definition_table.CreateInstanceObject(idef_index, xform);
      if (inst_obj != null)
      {
        inst_obj.Select(true);
      }
      else
      {
        if (!bQuietly)
          RhUtil.RhinoApp().Print("Error creating block.\n");
        return IRhinoCommand.result.failure;
      }

      // Step 7, delete existing geometry
      for (int i = 0; i < objects.Count; i++)
        context.m_doc.DeleteObject(new MRhinoObjRef(objects[i]));

      context.m_doc.Redraw();

      return IRhinoCommand.result.success;
    }

    string GetUnusedInstanceDefinitionName(MRhinoDoc doc)
    {
      string idef_name = string.Empty;
      doc.m_instance_definition_table.GetUnusedInstanceDefinitionName(ref idef_name);
      return idef_name;
    }

    // Identify subsets of the instance definition fields.
    // These bits are used to determine which fields to set
    // when an OnInstanceDefinition class is used to modify
    // an existing instance definition.
    enum IDEF_SETTINGS : int
    {
      no_idef_settings = 0x0,
      idef_name_setting = 0x1,  // m_name
      idef_description_setting = 0x2,  // m_description
      idef_url_setting = 0x4,  // all m_url_* fields
      idef_units_setting = 0x8,  // m_us and m_unit_scale
      idef_source_archive_setting = 0x10,  // all m_source_* fields
      idef_userdata_setting = 0x20,  // all m_source_* fields
      all_idef_settings = 0xFFFF
    }

    int CreateInstanceDefinition(MRhinoDoc doc, IOnInstanceDefinition idef, IOn3dPoint point, List<IRhinoObject> objects, bool bQuietly)
    {
      int found_index = doc.m_instance_definition_table.FindInstanceDefinition(idef.Name(), true);
      List<IRhinoObject> idef_objects = new List<IRhinoObject>();

      OnXform xform = new OnXform();
      xform.Translation(new On3dPoint(OnUtil.On_origin) - point);

      for (int i = 0; i < objects.Count; i++)
      {
        IRhinoObject obj = objects[i];
        if (obj == null)
          continue;

        if (obj.ObjectType() == IOn.object_type.light_object ||
            obj.ObjectType() == IOn.object_type.grip_object ||
            obj.ObjectType() == IOn.object_type.phantom_object
          )
          continue;

        // Transform geometry and attributes, but do not add to the document
        MRhinoObject dupe = doc.TransformObject(obj, xform, false, false, false);
        if (dupe != null)
        {
          if (doc.AddObject(dupe, false, true))
            idef_objects.Add(dupe);
        }
      }

      if (idef_objects.Count < 1)
        return -1;

      int idef_index = -1;

      if (found_index < 0)
        idef_index = doc.m_instance_definition_table.AddInstanceDefinition(idef, idef_objects.ToArray(), false, bQuietly);
      else if (doc.m_instance_definition_table.ModifyInstanceDefinitionGeometry(found_index, idef_objects.ToArray(), bQuietly))
      {
        idef_index = found_index;
        doc.m_instance_definition_table.ModifyInstanceDefinition(idef, idef_index, (uint)IDEF_SETTINGS.all_idef_settings, bQuietly);

        if (!bQuietly)
        {
          IRhinoInstanceObject[] iref_object_list = null;
          int iref_count = doc.m_instance_definition_table[found_index].GetReferences(out iref_object_list);
          if (iref_count > 0)
            RhUtil.RhinoApp().Print(string.Format("{0} Instances of block \"{1}\" have been updated.\n", iref_count, idef.Name()));
        }
      }

      if (idef_index < 0 && !bQuietly)
      {
        string message = (found_index < 0) ? "Error creating block.\n" : "Error modifying block.\n";
        RhUtil.RhinoApp().Print(message);
      }

      return idef_index;
    }
  }
}

