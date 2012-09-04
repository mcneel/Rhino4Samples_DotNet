using System;
using System.Collections.Generic;
using System.Text;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsExplodeHatch : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{4e304f5a-6d98-4fb4-999f-56216efab724}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsExplodeHatch";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select hatch to explode");
      go.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.hatch_object);
      go.GetObjects(1, 1);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      IRhinoHatch hatchObject = MRhinoHatch.ConstCast(go.Object(0).Object());
      if (null == hatchObject)
        return IRhinoCommand.result.failure;

      List<Guid> objectGuids = new List<Guid>();
      int objectGuids_Count = ExplodeHatch(context.m_doc, hatchObject.Attributes().m_uuid, ref objectGuids);

      return IRhinoCommand.result.success;
    }

    public int ExplodeHatch(MRhinoDoc doc, Guid hatchGuid, ref List<Guid> objectGuids)
    {
      int objectGuids_Count = objectGuids.Count;

      if (null != doc)
      {
        IRhinoHatch hatchObject = MRhinoHatch.ConstCast(doc.LookupObject(hatchGuid));
        if (null != hatchObject)
        {
          MRhinoObject[] subObjects = null;
          int subObjects_Count = hatchObject.GetSubObjects(out subObjects);
          if (0 < subObjects_Count || null != subObjects)
          {
            for (int i = 0; i < subObjects_Count; i++)
            {
              bool rc = doc.AddObject(subObjects[i]);
              if (rc)
                objectGuids.Add(subObjects[i].Attributes().m_uuid);
            }
          }
        }
      }

      return objectGuids.Count - objectGuids_Count;
    }
  }
}

