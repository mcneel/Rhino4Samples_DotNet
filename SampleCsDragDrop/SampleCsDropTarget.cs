using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RMA.Rhino;

namespace SampleCsDragDrop
{
  /// <summary>
  /// The SampleCsDropTarget class
  /// </summary>
  public class SampleCsDropTarget : MRhinoDropTarget
  {
    /// <summary>
    /// Extract SampleCsDragDropData from DataObject
    /// </summary>
    public SampleCsDragData GetSampleCsDragData(DataObject data)
    {
      if (null != data && data.GetDataPresent(typeof(SampleCsDragData)))
        return data.GetData(typeof(SampleCsDragData)) as SampleCsDragData;
      return null;
    }

    /// <summary>
    /// The first plug-in that returns true will get control of the drag and
    /// drop operation.  You should check to see if your data is included in
    /// the specified DataObject and only return true if it is.
    /// </summary>
    public override bool SupportDataObject(DataObject data)
    {
      return (null != GetSampleCsDragData(data));
    }

    /// <summary>
    /// This method is called when the drag and drop operation is completed and
    /// the item being dragged was dropped on a valid, top level Rhino object.
    /// </summary>
    public override bool OnDropOnObject(IRhinoObjRef objRef, MRhinoView rhinoView, DataObject data, DragDropEffects dropEffect, Point point)
    {
      SampleCsDragData dragData = GetSampleCsDragData(data);
      if (null == dragData)
        return false;

      MessageBox.Show(
        RhUtil.RhinoApp().MainWnd(),
        "String \"" + dragData.DragString + "\" Dropped on object",
        "SampleCsDragDrop",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information
        );

      return true;
    }
    /// <summary>
    /// This method is called when the drag and drop operation is completed and
    /// the item being dragged was dropped on the Rhino layer list control.
    /// </summary>
    public override bool OnDropOnLayerListCtrl(IWin32Window layerListCtrl, int layerIndex, DataObject data, DragDropEffects dropEffect, Point point)
    {
      SampleCsDragData dragData = GetSampleCsDragData(data);
      if (null == dragData)
        return false;

      if (layerIndex < 0)
      {
        MessageBox.Show(
          RhUtil.RhinoApp().MainWnd(),
          "String \"" + dragData.DragString + "\" Dropped on layer list control, not on a layer",
          "SampleCsDragDrop",
          MessageBoxButtons.OK,
          MessageBoxIcon.Information
          );
      }
      else
      {
        MRhinoDoc doc = RhUtil.RhinoApp().ActiveDoc();
        if (null != doc && layerIndex < doc.m_layer_table.LayerCount())
        {
          MessageBox.Show(
            RhUtil.RhinoApp().MainWnd(),
            "String \"" + dragData.DragString + "\" Dropped on layer \"" + doc.m_layer_table[layerIndex].m_name + "\"",
            "SampleCsDragDrop",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
            );
        }
      }

      return true;
    }
    /// <summary>
    /// This method is called when the drag and drop operation is completed
    /// and the item being dragged was dropped on a Rhino view and did not
    /// land on a selectable object.
    /// </summary>
    public override bool OnDropOnRhinoView(MRhinoView rhinoView, DataObject data, DragDropEffects effect, Point point)
    {
      SampleCsDragData dragData = GetSampleCsDragData(data);
      if (null == dragData)
        return false;

      MessageBox.Show(
        RhUtil.RhinoApp().MainWnd(),
        "String \"" + dragData.DragString + "\" Dropped on Rhino View",
        "SampleCsDragDrop",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information
        );

      return true;
    }
  }
}
