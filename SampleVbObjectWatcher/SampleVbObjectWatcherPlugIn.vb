Imports RMA.Rhino

'''<summary>
''' Every Rhino.NET Plug-In must have one and only one MRhinoPlugIn derived
''' class. DO NOT create an instance of this class. It is the responsibility
''' of Rhino.NET to create an instance of this class and register it with Rhino.
'''</summary>
Public Class SampleVbObjectWatcherPlugIn
  Inherits RMA.Rhino.MRhinoUtilityPlugIn

  '''<summary>
  ''' Rhino tracks plug-ins by their unique ID. Every plug-in must have a unique id.
  ''' The Guid created by the project wizard is unique. You can create more Guids using
  ''' the "Create Guid" tool in the Tools menu.
  '''</summary>
  '''<returns>The id for this plug-in</returns>
  Public Overrides Function PlugInID() As System.Guid
    Return New System.Guid("{81e486c8-8988-407b-bb33-7be9c915f0d5}")
  End Function

  '''<returns>Plug-In name as displayed in the plug-in manager dialog</returns>
  Public Overrides Function PlugInName() As String
    Return "SampleVbObjectWatcher"
  End Function

  '''<returns>Version information for this plug-in</returns>
  Public Overrides Function PlugInVersion() As String
    Return "1.0.0.0"
  End Function

  '''<summary>
  ''' Called after the plug-in is loaded and the constructor has been run.
  ''' This is a good place to perform any significant initialization,
  ''' license checking, and so on.  This function must return 1 for
  ''' the plug-in to continue to load.
  '''</summary>
  '''<returns>
  '''  1 = initialization succeeded, let the plug-in load
  '''  0 = unable to initialize, don't load plug-in and display an error dialog
  ''' -1 = unable to initialize, don't load plug-in and do not display an error
  '''      dialog. Note: OnUnloadPlugIn will not be called
  '''</returns>
  Public Overrides Function OnLoadPlugIn() As Integer
    Return 1
  End Function

  '''<summary>
  ''' Called when the plug-in is about to be unloaded.  After this
  ''' function is called, the plug-in will be disposed.
  '''</summary>
  Public Overrides Sub OnUnloadPlugIn()
    ' TODO: Add plug-in cleanup code here.
  End Sub

  
End Class
