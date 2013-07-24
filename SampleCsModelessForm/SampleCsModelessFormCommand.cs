using System;
using System.Windows.Forms;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsModelessForm
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// A command wizard can be found in visual studio when adding a new item to the project.
  /// </summary>
  public class SampleCsModelessFormCommand : RMA.Rhino.MRhinoCommand
  {
    public static SampleCsModelessFormCommand _instance;
    public static SampleCsForm _form;

    /// <summary>
    /// Public constructor
    /// </summary>
    public SampleCsModelessFormCommand()
    {
      _instance = this;
    }

    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{22525bb5-8e5b-4eee-9ac0-d1f03019cbb3}");
    }

    /// <returns>
    /// The command name as it appears on the Rhino command line
    /// </returns>
    public override string EnglishCommandName()
    {
      return "SampleCsModelessForm";
    }

    /// <summary>
    /// The only instance of this command.
    /// </summary>
    public static SampleCsModelessFormCommand Instance
    {
      get { return _instance; }
    }

    /// <summary> 
    /// This gets called when when the user runs this command.
    /// </summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      if (null != _form)
        return IRhinoCommand.result.nothing;

      _form = new SampleCsForm();
      _form.FormClosed += new FormClosedEventHandler(SampleCsForm_FormClosed);
      _form.Show(RhUtil.RhinoApp().MainWnd());

      return IRhinoCommand.result.success;
    }

    void SampleCsForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      if (null != _form)
        _form = null;
    }
  }
}

