using System;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsLeaderTest : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{81a9101c-391c-4d10-b46d-20a425fe5aaa}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsLeaderTest";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select object");
      go.GetObjects(1, 1);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      MRhinoObjRef obj_ref = go.Object(0);
      IRhinoObject obj = obj_ref.Object();
      if (null == obj)
        return IRhinoCommand.result.failure;

      // 1.) Try the object test
      RhUtil.RhinoApp().Print("Object Test: ");
      if (IsLeader(obj))
        RhUtil.RhinoApp().Print("object is a leader.\n");
      else
        RhUtil.RhinoApp().Print("object is not a leader.\n");

      // 2.) Try the GUID test
      RhUtil.RhinoApp().Print("GUID Test: ");
      if (IsLeader(obj.Attributes().m_uuid))
        RhUtil.RhinoApp().Print("object is a leader.\n");
      else
        RhUtil.RhinoApp().Print("object is not a leader.\n");

      // 3.) Try the geometry test
      RhUtil.RhinoApp().Print("Geometry Test: ");
      if (IsLeader(obj_ref.Geometry()))
        RhUtil.RhinoApp().Print("object is a leader.\n");
      else
        RhUtil.RhinoApp().Print("object is not a leader.\n");

      return IRhinoCommand.result.success;
    }

    bool IsLeader(IRhinoObject obj)
    {
      bool rc = false;
      if (null != obj)
      {
        IRhinoAnnotationLeader leader_obj = MRhinoAnnotationLeader.ConstCast(obj);
        if (null != leader_obj)
          rc = true;
      }
      return rc;
    }

    bool IsLeader(Guid guid)
    {
      bool rc = false;
      MRhinoObjRef obj_ref = new MRhinoObjRef(guid);
      IRhinoObject obj = obj_ref.Object();
      if (null != obj)
        rc = IsLeader(obj);
      return rc;
    }

    bool IsLeader(IOnGeometry geom)
    {
      bool rc = false;
      IOnLeader2 leader = OnLeader2.ConstCast(geom);
      if (null != leader)
        rc = true;
      return rc;
    }
  }
}

