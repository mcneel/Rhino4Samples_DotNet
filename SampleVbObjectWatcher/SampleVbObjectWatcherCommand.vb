Imports RMA.Rhino
Imports RMA.OpenNURBS
Imports RMA.Rhino.RhUtil

'''<summary>
''' A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
''' DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
'''</summary>
Public Class SampleVbObjectWatcherCommand
  Inherits RMA.Rhino.MRhinoCommand

  Private m_watcher As SampleVbObjectWatcherEvents = Nothing

  '''<summary>
  ''' Rhino tracks commands by their unique ID. Every command must have a unique id.
  ''' The Guid created by the project wizard is unique. You can create more Guids using
  ''' the "Create Guid" tool in the Tools menu.
  '''</summary>
  '''<returns>The id for this command</returns>
  Public Overrides Function CommandUUID() As System.Guid
    Return New Guid("{422e06d0-9f5d-40a4-be6e-dd2747eb1708}")
  End Function

  '''<returns>The command name as it appears on the Rhino command line</returns>
  Public Overrides Function EnglishCommandName() As String
    Return "SampleVbObjectWatcher"
  End Function

  '''<summary> This gets called when when the user runs this command.</summary>
  Public Overrides Function RunCommand(ByVal context As RMA.Rhino.IRhinoCommandContext) As RMA.Rhino.IRhinoCommand.result

    If (m_watcher Is Nothing) Then
      m_watcher = New SampleVbObjectWatcherEvents()
      'the watcher has to be registered once before it can be used
      m_watcher.Register()
    End If

    If (m_watcher.IsEnabled()) Then
      RhinoApp.Print("Disabling custom event watcher" + vbCrLf)
      m_watcher.Enable(False)
    Else
      RhinoApp.Print("Enabling custom event watcher" + vbCrLf)
      m_watcher.Enable(True)
    End If

    Return IRhinoCommand.result.success

  End Function

End Class
