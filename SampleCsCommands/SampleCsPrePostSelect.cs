using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SamplePrePostSelect : RMA.Rhino.MRhinoCommand
  {
    double _dValue = 1.0;
    int _nValue = 1;

    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{dcaf644b-0363-44b2-8077-59051e5be6e0}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsPrePostSelect";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      int nValue = _nValue;
      double dValue = _dValue;

      MRhinoGetObject go = new MRhinoGetObject();
      go.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.curve_object);
      go.EnableGroupSelect(true);
      go.EnableSubObjectSelect(false);
      go.EnableClearObjectsOnEntry(false);
      go.EnableUnselectObjectsOnExit(false);
      go.EnableDeselectAllBeforePostSelect(false);

      bool bHavePreselectedObjects = false;

      for (; ; )
      {
        go.ClearCommandOptions();

        int dOptionIndex = go.AddCommandOptionNumber(
          new MRhinoCommandOptionName("Double"), 
          new MRhinoGet.DoubleOption(dValue), 
          "Double value", false, 0.1, 99.9
          );

        int nOptionIndex = go.AddCommandOptionInteger(
          new MRhinoCommandOptionName("Integer"), 
          new MRhinoGet.IntegerOption(nValue), 
          "Integer value", 1, 99
          );

        IRhinoGet.result res = go.GetObjects(1, 0);

        if (res == IRhinoGet.result.option)
        {
          IRhinoCommandOption commandOption = go.Option();
          if (null != commandOption)
          {
            int optionIndex = commandOption.m_option_index;
            if (optionIndex == nOptionIndex)
              nValue = (int)commandOption.m_number_option_value;
            else if (optionIndex == dOptionIndex)
              dValue = commandOption.m_number_option_value;
          }

          go.EnablePreSelect(false);
          continue;
        }

        else if (res != IRhinoGet.result.@object)
          return IRhinoCommand.result.cancel;

        if (go.ObjectsWerePreSelected())
        {
          bHavePreselectedObjects = true;
          go.EnablePreSelect(false);
          continue;
        }

        break;
      }

      if (bHavePreselectedObjects)
      {
        // Normally, pre-selected objects will remain selected, when a
        // command finishes, and post-selected objects will be unselected.
        // This this way of picking, it is possible to have a combination
        // of pre-selected and post-selected. So, to make sure everything
        // "looks the same", lets unselect everything before finishing
        // the command.
        for (int i = 0; i < go.ObjectCount(); i++)
        {
          IRhinoObject rhinoObject = go.Object(i).Object();
          if (null != rhinoObject)
            rhinoObject.Select(false);
        }
        context.m_doc.Redraw();
      }

      int objectCount = go.ObjectCount();
      _dValue = dValue;
      _nValue = nValue;

      RhUtil.RhinoApp().Print(string.Format("Select object count = {0}\n", objectCount));
      RhUtil.RhinoApp().Print(string.Format("Value of double = {0}\n", _dValue));
      RhUtil.RhinoApp().Print(string.Format("Value of integer = {0}\n", _nValue));

      return IRhinoCommand.result.success;
    }
  }
}

