using RMA.Rhino;

namespace SampleCsDragDrop
{
  /// <summary>
  /// The SampleCsDragDropPlugIn class
  /// </summary>
  public class SampleCsDragDropPlugIn : RMA.Rhino.MRhinoUtilityPlugIn
  {
    private SampleCsDropTarget _dropTarget = null;

    /// <summary>
    /// The id for this plug-in.
    /// </summary>
    public override System.Guid PlugInID()
    {
      return new System.Guid("{ad028425-e687-46c7-b7bd-fdfbef48bc0f}");
    }

    /// <summary>
    /// Plug-In name as displayed in the plug-in manager dialog
    /// </summary>
    public override string PlugInName()
    {
      return "SampleCsDragDrop";
    }

    /// <summary>
    /// Version information for this plug-in
    /// </summary>
    public override string PlugInVersion()
    {
      return "1.0.0.0";
    }

    /// <summary>
    /// Called after the plug-in is loaded and the constructor has been run.
    /// This is a good place to perform any significant initialization,
    /// license checking, and so on.  This function must return 1 for
    /// the plug-in to continue to load.
    /// </summary>
    public override int OnLoadPlugIn()
    {
      // Step 1: Create instance of our drop target
      _dropTarget = new SampleCsDropTarget();
      if (null != _dropTarget)
      {
        // Step 2: Configure the drop target
        _dropTarget.EnableAllowDropOnObject(true);
        _dropTarget.EnableAllowDropOnRhinoView(true);
        _dropTarget.EnableAllowDropOnRhinoLayerListControl(true);
        _dropTarget.EnableDeselectAllOnDrag(true);

        // Step 3: Enable the drop target
        _dropTarget.EnableRhinoDropTarget(true);
      }

      return 1;
    }

    /// <summary>
    /// Called when the plug-in is about to be unloaded.  After this
    /// function is called, the plug-in will be disposed.
    /// </summary>
    public override void OnUnloadPlugIn()
    {
      // TODO: Add plug-in cleanup code here.
    }
  }
}
