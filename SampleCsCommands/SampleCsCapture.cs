using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsCapture : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{d3df62cd-494e-4e8c-804c-b03303df8624}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsCapture";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoView view = RhUtil.RhinoApp().ActiveView();
      if (view == null)
        return IRhinoCommand.result.nothing;

      MRhinoGetOption go = new MRhinoGetOption();
      go.SetCommandPrompt("Capture Method");
      go.SetCommandPromptDefault("ViewCapture");
      int viewcap = go.AddCommandOption(new MRhinoCommandOptionName("ViewCapture"));
      int screencap = go.AddCommandOption(new MRhinoCommandOptionName("ScreenCapture"));
      go.GetOption();
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      System.Drawing.Bitmap bmp = null;

      if (go.Option().m_option_index == viewcap)
      {
        MRhinoDisplayPipeline pipeline = view.DisplayPipeline();
        int left = 0, right = 0, bot = 0, top = 0;
        view.MainViewport().VP().GetScreenPort(ref left, ref right, ref bot, ref top);
        int w = right - left;
        int h = bot - top;
        bmp = new System.Drawing.Bitmap(w, h);
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
        MDisplayPipelineAttributes attr = new MDisplayPipelineAttributes(pipeline.DisplayAttrs());
        bool rc = pipeline.DrawToDC(g, w, h, attr);
        g.Dispose();
        if (!rc)
          bmp = null;
      }
      else
      {
        bmp = new System.Drawing.Bitmap(1, 1);
        bool rc = view.ScreenCaptureToBitmap(ref bmp, true, false);
        if (!rc)
          bmp = null;
      }

      if (bmp != null)
      {
        string mydir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        string path = System.IO.Path.Combine(mydir, "capture.png");
        bmp.Save(path);
        return IRhinoCommand.result.success;
      }

      return IRhinoCommand.result.failure;
    }
  }
}

