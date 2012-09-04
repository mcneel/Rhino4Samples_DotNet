using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsExtractEdgeFaces : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{2d344627-5aba-46e5-9426-ffe82b3a3486}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsExtractEdgeFaces";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select edge curve");
      go.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.edge_object);
      go.GetObjects(1, 1);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      IRhinoObject obj = go.Object(0).Object();
      IOnBrep brep = go.Object(0).Brep();
      IOnBrepEdge edge = go.Object(0).Edge();
      if (null == obj || null == brep || null == edge)
        return IRhinoCommand.result.failure;

      MRhinoObjectAttributes attribs = new MRhinoObjectAttributes(obj.Attributes());
      if (attribs.GroupCount() > 0)
        attribs.RemoveFromAllGroups();

      for (int i = 0; i < edge.TrimCount(); i++)
      {
        IOnBrepTrim trim = edge.Trim(i);
        if (null != trim)
        {
          IOnBrepFace face = trim.Face();
          if (null != face)
          {
            OnBrep face_brep = brep.DuplicateFace(face.m_face_index, true);
            if (null != face_brep)
            {
              MRhinoBrepObject face_brep_obj = context.m_doc.AddBrepObject(face_brep, attribs);
              if (null != face_brep_obj)
                face_brep_obj.Select();
            }
          }
        }
      }

      context.m_doc.Redraw();

      return IRhinoCommand.result.success;
    }
  }
}

