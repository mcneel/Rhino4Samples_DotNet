using System;
using System.Collections.Generic;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsSelectByName : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{aa84e353-9939-4957-833f-00b9e959740a}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsSelectByName";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetString gs = new MRhinoGetString();
      gs.SetCommandPrompt("Object name to select");
      gs.GetString();
      if (gs.CommandResult() != IRhinoCommand.result.success)    
        return gs.CommandResult();  
      
      string name = gs.String().Trim();
      if (string.IsNullOrEmpty(name))    
        return IRhinoCommand.result.nothing;

      MRhinoObjectIterator it = new MRhinoObjectIterator(
        IRhinoObjectIterator.object_state.normal_objects, 
        IRhinoObjectIterator.object_category.active_and_reference_objects  
        );  
      
      int num_selected = 0;  
      IRhinoObject obj = null;  
      for (obj = it.First(); null != obj; obj = it.Next())
      {    
        if (name.Equals(obj.Attributes().m_name, StringComparison.OrdinalIgnoreCase))
        {      
          obj.Select(true, true, true);      
          num_selected++;    
        }  
      }  
      
      if (0 == num_selected)    
        RhUtil.RhinoApp().Print("0 objects selected\n" );  
      else if (1 == num_selected)
        RhUtil.RhinoApp().Print("1 object selected\n");
      else
        RhUtil.RhinoApp().Print(string.Format("{0} objects selected\n", num_selected));
      
      if (0 < num_selected)
        context.m_doc.Redraw();

      return IRhinoCommand.result.success;
    }
  }
}

