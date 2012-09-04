using System;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  /// <summary>
  /// SampleCsColoredMeshConduit
  /// </summary>
  public class SampleCsColoredMeshConduit : MRhinoDisplayConduit
  {
    uint m_runtime_object_serial_number;
    ArrayOnColor m_mesh_face_colors;
    Random m_random;

    public SampleCsColoredMeshConduit(uint runtime_object_serial_number)
      : base(new MSupportChannels(MSupportChannels.SC_DRAWOBJECT), false)
    {
      m_runtime_object_serial_number = runtime_object_serial_number;
      m_mesh_face_colors = null;
      m_random = new Random();
    }

    public override bool ExecConduit(ref MRhinoDisplayPipeline dp, uint nChannel, ref bool bTerminate)
    {
      if (nChannel == MSupportChannels.SC_DRAWOBJECT)
      {
        if (null != m_pChannelAttrs.m_pObject)
        {
          if (m_pChannelAttrs.m_pObject.m_runtime_object_serial_number == m_runtime_object_serial_number)
          {
            if (null == m_mesh_face_colors)
            {
              IRhinoMeshObject mesh_object = MRhinoMeshObject.ConstCast(m_pChannelAttrs.m_pObject);
              if (null != mesh_object)
              {
                int mesh_face_count = mesh_object.Mesh().FaceCount();
                m_mesh_face_colors = new ArrayOnColor(mesh_face_count);
                for (int i = 0; i < mesh_face_count; i++)
                {
                  OnColor color = new OnColor(m_random.Next(0, 255), m_random.Next(0, 255), m_random.Next(0, 255));
                  m_mesh_face_colors.Append(color);
                }
              }
            }

            if (null != m_mesh_face_colors)
            {
              m_pChannelAttrs.m_bDrawObject = false;

              MDisplayPipelineAttributes da = new MDisplayPipelineAttributes();
              da.m_bShadeSurface = true;
              da.m_pMaterial.m_FrontMaterial.SetTransparency(0.0);
              da.m_pMaterial.m_BackMaterial.SetTransparency(0.0);
              da.m_pMaterial.m_FrontMaterial.m_bFlatShaded = true;
              da.m_pMaterial.m_BackMaterial.m_bFlatShaded = true;

              dp.EnableDepthWriting(true);

              if (da.m_bCullBackfaces)
                dp.PushCullFaceMode(1);

              for (int i = 0; i < m_mesh_face_colors.Count(); i++)
              {
                da.m_pMaterial.m_FrontMaterial.m_diffuse = m_mesh_face_colors[i];
                da.m_pMaterial.m_BackMaterial.m_diffuse = m_mesh_face_colors[i];
                dp.DrawFace(m_pChannelAttrs.m_pObject, i, da);
              }

              if (da.m_bCullBackfaces)
                dp.PopCullFaceMode();
            }
          }
        }
      }
      return true;
    }
  }

  ///////////////////////////////////////////////////////////////////////////
  ///////////////////////////////////////////////////////////////////////////

  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsColoredMesh : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{e4e04c54-e7dc-423e-aa93-e11e83ef9cac}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsColoredMesh";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select mesh");
      go.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.mesh_object);
      go.GetObjects(1, 1);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      IRhinoMeshObject mesh_object = MRhinoMeshObject.ConstCast(go.Object(0).Object());
      if (null == mesh_object)
        return IRhinoCommand.result.failure;

      SampleCsColoredMeshConduit conduit = new SampleCsColoredMeshConduit(mesh_object.m_runtime_object_serial_number);
      conduit.Enable();
      context.m_doc.Regen();

      MRhinoGetString gs = new MRhinoGetString();
      gs.SetCommandPrompt("Press <Enter> to continue");
      gs.AcceptNothing();
      gs.GetString();

      conduit.Disable();
      context.m_doc.Regen();

      return IRhinoCommand.result.success;
    }
  }
}

