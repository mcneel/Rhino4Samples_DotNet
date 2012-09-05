using System;
using System.Collections;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsSaveView : RMA.Rhino.MRhinoCommand
  {
    static SampleCsSaveView _instance;
    Stack _saved_views;

    /// <summary>
    /// Public constructor
    /// </summary>
    public SampleCsSaveView()
      : base()
    {
      _instance = this;
      _saved_views = new Stack();
    }

    /// <summary>
    /// Returns the one and only instance of this command class
    /// </summary>
    public static SampleCsSaveView Instance
    {
      get { return _instance; }
    }

    /// <summary>
    /// Pops a saved view off of the saved view stack
    /// </summary>
    public On3dmView PopView()
    {
      On3dmView view = null;
      try
      {
        object obj = _saved_views.Pop();
        if (null != obj)
          view = (On3dmView)obj;
      }
      catch
      {
      }
      return view;
    }

    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{70E50AED-B710-43D5-BA9D-66308E4F4CBD}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsSaveView";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoView rhino_view = RhUtil.RhinoApp().ActiveView();
      if (null == rhino_view)
        return IRhinoCommand.result.failure;

      On3dmView view = new On3dmView(rhino_view.ActiveViewport().View());
      view.m_wallpaper_image.Default();
      _saved_views.Push(view);

      int count = _saved_views.Count;
      if (1 == count)
        RhUtil.RhinoApp().Print("The saved view stack has 1 entry.\n");
      else
        RhUtil.RhinoApp().Print(string.Format("The saved view stack has {0} entries.\n", count));

      return IRhinoCommand.result.success;
    }
  }

  ///////////////////////////////////////////////////////////////////////////
  ///////////////////////////////////////////////////////////////////////////

  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsRestoreView : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{6181347B-828D-4815-A918-1B1142F8CA81}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsRestoreView";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoView rhino_view = RhUtil.RhinoApp().ActiveView();
      if (null == rhino_view)
        return IRhinoCommand.result.failure;

      On3dmView saved_view = SampleCsSaveView.Instance.PopView();
      if (null == saved_view)
      {
        RhUtil.RhinoApp().Print("No saved views to restore.\n");
        return IRhinoCommand.result.nothing;
      }

      MRhinoViewport viewport = rhino_view.ActiveViewport();

      int scr_left = 0, scr_right = 0, scr_bottom = 0, scr_top = 0;
      viewport.VP().GetScreenPort(ref scr_left, ref scr_right, ref scr_bottom, ref scr_top);

      On3dmView new_view = new On3dmView(saved_view);
      new_view.m_vp.SetScreenPort(scr_left, scr_right, scr_bottom, scr_top);

      double aspect = 0.0;
      if (new_view.m_vp.GetScreenPortAspect(ref aspect))
        new_view.m_vp.SetFrustumAspect(aspect);

      // Some optional parameters that you might want to control
      // when restoring a saved view
      bool bSavedViewSetsConstructionPlane = true;
      bool bSavedViewSetsProjection = true;
      bool bSavedViewSetsTraceImage = true;

      // Keep existing construction plane?
      if (!bSavedViewSetsConstructionPlane)
        new_view.m_cplane = new On3dmConstructionPlane(viewport.ConstructionPlane());

      // Keep existing projection?
      if (!bSavedViewSetsProjection)
        new_view.m_vp.SetProjection(viewport.VP().Projection());

      // Copy parameters from existing view that you might want to keep
      // from current view
      new_view.m_position = new On3dmViewPosition(viewport.View().m_position);
      new_view.m_trace_image = new On3dmViewTraceImage(viewport.View().m_trace_image);
      new_view.m_wallpaper_image = new On3dmWallpaperImage(viewport.View().m_wallpaper_image);
      new_view.m_clipping_planes = new ArrayOnClippingPlaneInfo(viewport.View().m_clipping_planes);
      new_view.m_display_mode = viewport.DisplayMode();
      new_view.m_display_mode_id = viewport.View().m_display_mode_id;

      // Do the view restoration
      viewport.SetView(new_view);

      // Keep existing trace image?
      if (!bSavedViewSetsTraceImage)
        viewport.SetTraceImage(saved_view.m_trace_image);

      // Append the current view onto Rhino's view stack
      viewport.PushViewProjection();

      // Redraw
      rhino_view.Redraw();

      return IRhinoCommand.result.success;
    }
  }
}

