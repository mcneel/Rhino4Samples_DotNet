using System;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsSpin : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{9459d3cd-3869-44ad-a224-acb2afda3203}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsSpin";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoView view = RhUtil.RhinoApp().ActiveView();
      if (null == view)
        return IRhinoCommand.result.failure;

      MRhinoViewport vp = view.ActiveViewport();
      MRhinoDisplayPipeline dp = view.DisplayPipeline();
      MDisplayPipelineAttributes da = view.DisplayAttributes();

      // Prevent capturing of the frame buffer on every update
      MRhinoDisplayPipeline.EnableFrameBufferCapture(false);
      // Prevent the draw list from updating on every frame
      dp.FreezeDrawing(true);

      int dir = 0; // 0 = Right, 1 = Left, 2 = Down, and 3 = Up
      int frame_count = 100;
      double delta_angle = 5.0 * (Math.PI / 180.0);

      for (int i = 0; i < frame_count; i++)
      {
        switch (dir)
        {
          case 0:
            vp.LeftRightRotate(delta_angle);
            break;
          case 1:
            vp.LeftRightRotate(-delta_angle);
            break;
          case 2:
            vp.DownUpRotate(delta_angle);
            break;
          case 3:
            vp.DownUpRotate(-delta_angle);
            break;
        }
        //dp.DrawFrameBuffer(da);
        view.Redraw();
        RhUtil.RhinoApp().Wait(0);
      }

      dp.FreezeDrawing(false);
      MRhinoDisplayPipeline.EnableFrameBufferCapture(true);

      view.Redraw();

      return IRhinoCommand.result.success;
    }
  }
}

