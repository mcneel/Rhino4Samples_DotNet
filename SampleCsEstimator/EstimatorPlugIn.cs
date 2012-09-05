/////////////////////////////////////////////////////////////////////////////
// EstimatorPlugIn.cs

using System;
using System.IO;
using System.Net;
using System.Text;
using RMA.Rhino;

namespace Estimator
{
  ///<summary>
  /// Every Rhino.NET Plug-In must have one and only one MRhinoPlugIn derived
  /// class. DO NOT create an instance of this class. It is the responsibility
  /// of Rhino.NET to create an instance of this class and register it with Rhino.
  ///</summary>
  public class EstimatorPlugIn : RMA.Rhino.MRhinoUtilityPlugIn
  {
    public EstimatorTagTable m_tag_table;

    public EstimatorPlugIn()
    {
      m_tag_table = new EstimatorTagTable();
    }

    ///<summary>
    /// Rhino tracks plug-ins by their unique ID. Every plug-in must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    /// </summary>
    /// <returns>The id for this plug-in</returns>
    public override System.Guid PlugInID()
    {
      return new System.Guid("{b92ef8f8-bcc8-4ae1-ad42-e348b9cf2460}");
    }

    /// <returns>Plug-In name as displayed in the plug-in manager dialog</returns>
    public override string PlugInName()
    {
      return "Estimator";
    }

    ///<returns>Version information for this plug-in</returns>
    public override string PlugInVersion()
    {
      return "1.0.0.0";
    }

    ///<summary>
    /// Called after the plug-in is loaded and the constructor has been run.
    /// This is a good place to perform any significant initialization,
    /// license checking, and so on.  This function must return 1 for
    /// the plug-in to continue to load.
    ///</summary>
    ///<returns>
    ///  1 = initialization succeeded, let the plug-in load
    ///  0 = unable to initialize, don't load plug-in and display an error dialog
    /// -1 = unable to initialize, don't load plug-in and do not display an error
    ///      dialog. Note: OnUnloadPlugIn will not be called
    ///</returns>
    public override int OnLoadPlugIn()
    {
      //ReadDelimitedFile();
      ReadWebDelimitedFile();
      return 1;
    }

    ///<summary>
    /// Called when the plug-in is about to be unloaded.  After this
    /// function is called, the plug-in will be disposed.
    ///</summary>
    public override void OnUnloadPlugIn()
    {
      // TODO: Add plug-in cleanup code here.
    }

    private bool ReadWebDelimitedFile()
    {
      bool rc = false;

      try
      {
        // Create a request for the URL
        WebRequest request = WebRequest.Create("http://www.mcneel.com/users/dale/sample.txt");
        // If required by the server, set the credentials
        request.Credentials = CredentialCache.DefaultCredentials;
        
        // Get the response
        WebResponse response = request.GetResponse();
        
        // Get the stream containing content returned by the server
        Stream dataStream = response.GetResponseStream();

        // Open the stream using a StreamReader for easy access
        StreamReader reader = new StreamReader(dataStream);

        // Parse the stream
        rc = ParseDelimitedFile(reader);

        // Cleanup the streams and the response
        reader.Close();
        dataStream.Close();
        response.Close();
      }
      catch
      {
      }

      return rc;
    }

    private bool ReadDelimitedFile()
    {
      bool rc = false;

      string location = System.Reflection.Assembly.GetExecutingAssembly().Location;

      StringBuilder str = new StringBuilder();
      str.Append(Path.GetDirectoryName(location));
      str.Append("\\sample.txt");

      string fname = str.ToString();

      if (File.Exists(fname))
      {
        try
        {
          StreamReader reader = new StreamReader(fname);
          rc = ParseDelimitedFile(reader);
          reader.Close();
        }
        catch
        {
        }
      }

      return rc;
    }

    private bool ParseDelimitedFile(StreamReader reader)
    {
      bool rc = false;

      if (null == reader)
        return rc;

      try
      {
        do
        {
          string line = reader.ReadLine();
          if (line.Length > 0) // Skip empty lines
          {
            if (line[0] != ';') // Skip comment lines
            {
              string[] items = line.Split(';');
              if (items.Length == 3)
              {
                EstimatorTag tag = new EstimatorTag(items);
                if (tag.IsValid())
                  m_tag_table.AddTag(tag);
              }
            }
          }
        }
        while (reader.Peek() != -1);

        rc = true;
      }

      catch
      {
      }

      return rc;
    }

  }
}
