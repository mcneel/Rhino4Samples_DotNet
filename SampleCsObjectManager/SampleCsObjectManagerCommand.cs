using System;
using RMA.Rhino;
using RMA.OpenNURBS;
using RMA.UI;

namespace SampleCsObjectManager
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// A command wizard can be found in visual studio when adding a new item to the project.
  /// </summary>
  public class SampleCsObjectManagerCommand : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{730b5dab-7e94-4662-ae3c-71dbefc70ea2}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsObjectManager";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      Guid id = SampleCsObjectManagerDockBar.ID();
      bool bVisible = MRhinoDockBarManager.IsDockBarVisible(id);

      string prompt;
      if (bVisible)
        prompt = string.Format("{0} window is visible. New value", EnglishCommandName());
      else
        prompt = string.Format("{0} window is hidden. New value", EnglishCommandName());

      MRhinoGetOption go = new MRhinoGetOption();
      go.SetCommandPrompt(prompt);
      int h_option = go.AddCommandOption(new MRhinoCommandOptionName("Hide"));
      int s_option = go.AddCommandOption(new MRhinoCommandOptionName("Show"));
      int t_option = go.AddCommandOption(new MRhinoCommandOptionName("Toggle"));
      go.GetOption();
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      IRhinoCommandOption opt = go.Option();
      if (opt == null)
        return IRhinoCommand.result.failure;

      int option_index = opt.m_option_index;
      if (h_option == option_index)
      {
        if (bVisible)
          MRhinoDockBarManager.ShowDockBar(id, false, false);
      }
      else if (s_option == option_index)
      {
        if (!bVisible)
          MRhinoDockBarManager.ShowDockBar(id, true, false);
      }
      else if (t_option == option_index)
      {
        if (bVisible)
          MRhinoDockBarManager.ShowDockBar(id, false, false);
        else
          MRhinoDockBarManager.ShowDockBar(id, true, false);
      }

      return IRhinoCommand.result.success;
    }
  }
}

