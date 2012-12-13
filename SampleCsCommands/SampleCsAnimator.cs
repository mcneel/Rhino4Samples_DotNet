using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsAnimator
{
  /// <summary>
  /// Display conduit used by the SampleCsAnimator command
  /// </summary>
  class SampleCsAnimatorConduit : MRhinoDisplayConduit
  {
    public OnXform m_xform = new OnXform(1);
    public List<IRhinoObject> m_objects = new List<IRhinoObject>(16);

    public SampleCsAnimatorConduit()
      : base(new MSupportChannels(MSupportChannels.SC_CALCBOUNDINGBOX | MSupportChannels.SC_DRAWOVERLAY), false)
    {
    }

    public override bool ExecConduit(ref MRhinoDisplayPipeline pipeline, uint channel, ref bool terminate)
    {
      if (MSupportChannels.SC_CALCBOUNDINGBOX == channel)
      {
        // If you are dynamically drawing objects, then we must implement the 
        // this channel to add to the overall scene bounding box. This will make
        // Rhino adjust its clipping planes to include our geometry.
        for (int i = 0; i < m_objects.Count; i++)
        {
          IRhinoObject obj = m_objects[i];
          if (null != obj)
          {
            OnBoundingBox bbox = obj.BoundingBox();
            bbox.Transform(m_xform);
            m_pChannelAttrs.m_BoundingBox.Union(bbox);
          }
        }
      }
      else if (MSupportChannels.SC_DRAWOVERLAY == channel)
      {
        // This channel is where the drawing takes place.
        for (int i = 0; i < m_objects.Count; i++)
        {
          IRhinoObject obj = m_objects[i];
          if (null != obj)
          {
            pipeline.SetObjectColor(obj.ObjectDrawColor());
            pipeline.DrawObject(obj, m_xform);
          }
        }
      }
      return true;
    }
  }

  ///////////////////////////////////////////////////////////////////////////
  ///////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// A command wizard can be found in visual studio when adding a new item to the project.
  /// </summary>
  public class SampleCsAnimator : RMA.Rhino.MRhinoCommand
  {
    int m_max_steps = 20;

    /// <summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    /// </summary>
    /// <returns>
    /// The id for this command
    /// </returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{25a7f32c-2f12-4817-ab14-87fe1d41a43d}");
    }

    /// <returns>
    /// The command name as it appears on the Rhino command line
    /// </returns>
    public override string EnglishCommandName()
    {
      return "SampleCsAnimator";
    }

    /// <summary> 
    /// This gets called when when the user runs this command.
    /// </summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      // Select objects to animate
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select objects to animate");
      go.GetObjects(1, 0);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      // Select path curve
      MRhinoGetObject gc = new MRhinoGetObject();
      gc.SetCommandPrompt("Select path curve");
      gc.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.curve_object);
      gc.SetGeometryAttributeFilter(IRhinoGetObject.GEOMETRY_ATTRIBUTE_FILTER.open_curve);
      gc.EnableDeselectAllBeforePostSelect(false);
      gc.GetObjects(1, 1);
      if (gc.CommandResult() != IRhinoCommand.result.success)
        return gc.CommandResult();

      // Get the curve
      IOnCurve crv = gc.Object(0).Curve();
      if (null == crv )
        return IRhinoCommand.result.failure;

      // Create an array of normalized curve parameters
      List<double> slist = new List<double>(m_max_steps);
      for (int i = 0; i < m_max_steps; i++ )
      {
        double s = (double)i / ( (double)m_max_steps - 1 );
        slist.Add(s);
      }

      // Get the real parameters along the curve
      double[] tlist = new double[m_max_steps];
      if (!crv.GetNormalizedArcLengthPoints(slist.ToArray(), ref tlist))
        return IRhinoCommand.result.failure;

      // Create the display conduit
      SampleCsAnimatorConduit conduit = new SampleCsAnimatorConduit();

      // Get points along curve
      On3dPoint start = new On3dPoint(crv.PointAtStart());
      List<On3dPoint> plist = new List<On3dPoint>(tlist.Length);
      for (int i = 0; i < m_max_steps; i++)
      {
        On3dPoint pt = new On3dPoint(crv.PointAt(tlist[i]));
        plist.Add(pt);
      }

      // Hide objects and add them to conduit's object array
      for (int i = 0; i < go.ObjectCount(); i++ )
      {
        MRhinoObjRef objref = go.Object(i);
        context.m_doc.HideObject(objref);
        conduit.m_objects.Add(objref.Object());
      }

      // Do animation...
      conduit.Enable();

      for (int i = 0; i < m_max_steps; i++)
      {
        On3dVector v = plist[i] - start;
        conduit.m_xform.Translation(v);
        context.m_doc.Redraw();
        Thread.Sleep(100);
      }

      for (int i = m_max_steps - 1; i >= 0; i--)
      {
        On3dVector v = plist[i] - start;
        conduit.m_xform.Translation(v);
        if (0 != i)
        {
          context.m_doc.Redraw();
          Thread.Sleep(100);
        }
      }

      conduit.Disable();

      // Show hidden objects
      for (int i = 0; i < go.ObjectCount(); i++)
      {
        MRhinoObjRef objref = go.Object(i);
        context.m_doc.ShowObject(objref);
      }

      context.m_doc.Redraw();

      return IRhinoCommand.result.success;
    }
  }
}

