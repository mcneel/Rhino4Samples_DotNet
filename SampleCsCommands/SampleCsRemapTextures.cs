using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsRemapTextures : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{157a1c6f-96bf-429f-8f2a-60975370b614}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsRemapTextures";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      // Verify that we have materials to remap textures
      MRhinoMaterialTable materialTable = context.m_doc.m_material_table;
      int materialCount = materialTable.MaterialCount();
      if (0 == materialCount)
      {
        RhUtil.RhinoApp().Print("No material found.\n");
        return IRhinoCommand.result.nothing;
      }

      // Prompt the user for the new texture folder
      FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
      folderBrowser.Description = "Select new texture folder";
      folderBrowser.RootFolder = System.Environment.SpecialFolder.Desktop;
      DialogResult result = folderBrowser.ShowDialog();
      if (result != DialogResult.OK)
        return IRhinoCommand.result.cancel;

      // The folder to search fo textures
      string searchFolder = folderBrowser.SelectedPath;

      // We will cache the search results (for performance)
      Dictionary<string, string> textureMap = new Dictionary<string, string>();

      // Used for reporting the results of the remap
      int numMaterials = 0;
      int numTextures = 0;
      int numRemapped = 0;

      // Iterate through the material table
      for (int i = 0; i < materialCount; i++)
      {
        // Get a material
        IRhinoMaterial oldMaterial = materialTable[i];

        // Validate...
        if (null == oldMaterial)
          continue;

        // .. and validate...
        if (oldMaterial.IsDeleted() || oldMaterial.IsReference())
          continue;

        numMaterials++;

        // .. and validate.
        int textureCount = oldMaterial.m_textures.Count();
        if (0 == textureCount)
          continue;

        numTextures += textureCount;

        // Copy the material
        OnMaterial newMaterial = new OnMaterial(oldMaterial);
        bool bModify = false;

        for (int j = 0; j < textureCount; j++)
        {
          string texturePath = newMaterial.m_textures[j].m_filename;
          string textureName = Path.GetFileName(texturePath).ToLower();

          // See if the material to remap has already been found
          string mapValue;
          if (textureMap.TryGetValue(textureName, out mapValue))
          {
            // Do the remap
            newMaterial.m_textures[j].m_filename = mapValue;
            numRemapped++;
            bModify = true;
          }
          else
          {
            // The material to remap has not been found (already), so go searching
            string[] searchResults = Directory.GetFiles(searchFolder, textureName, SearchOption.AllDirectories);
            if (null != searchResults && searchResults.Length > 0)
            {
              // Do the remap (use the first one found...)
              newMaterial.m_textures[j].m_filename = searchResults[0];
              // Add the results of the search to the dictionary
              textureMap.Add(textureName, searchResults[0]);
              numRemapped++;
              bModify = true;
            }
            else
            {
              // Oops...
              RhUtil.RhinoApp().Print(string.Format("Unable to remap {0}\n", texturePath));
            }
          }
        }

        // Modify the existing material by replacing with with the new one
        if (bModify)
          materialTable.ModifyMaterial(newMaterial, oldMaterial.m_material_index);
      }

      // Remapping textures requries a full redraw
      if (numRemapped > 0)
        context.m_doc.Regen();

      // Report the results
      if (1 == numMaterials)
        RhUtil.RhinoApp().Print(string.Format("1 material found\n", numMaterials));
      else
        RhUtil.RhinoApp().Print(string.Format("{0} materials found\n", numMaterials));

      if (1 == numTextures)
        RhUtil.RhinoApp().Print(string.Format("1 texture found\n", numTextures));
      else
        RhUtil.RhinoApp().Print(string.Format("{0} textures found\n", numTextures));

      if (1 == numRemapped)
        RhUtil.RhinoApp().Print(string.Format("1 texture remapped\n", numRemapped));
      else
        RhUtil.RhinoApp().Print(string.Format("{0} textures remapped\n", numRemapped));

      return IRhinoCommand.result.success; 
    }
  }
}

