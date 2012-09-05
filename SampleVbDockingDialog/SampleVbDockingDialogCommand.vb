Imports RMA.Rhino
Imports RMA.OpenNURBS
Imports RMA.Rhino.RhUtil

'''<summary>
''' A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
''' DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
'''</summary>
Public Class SampleVbDockingDialogCommand
  Inherits RMA.Rhino.MRhinoCommand

  '''<summary>
  ''' Rhino tracks commands by their unique ID. Every command must have a unique id.
  ''' The Guid created by the project wizard is unique. You can create more Guids using
  ''' the "Create Guid" tool in the Tools menu.
  '''</summary>
  '''<returns>The id for this command</returns>
  Public Overrides Function CommandUUID() As System.Guid
    Return New Guid("{001e774a-34a3-48ab-a75b-bdc91065aac0}")
  End Function

  '''<returns>The command name as it appears on the Rhino command line</returns>
  Public Overrides Function EnglishCommandName() As String
    Return "SampleVbDockingDialog"
  End Function

  '''<summary> This gets called when when the user runs this command.</summary>
  Public Overrides Function RunCommand(ByVal context As RMA.Rhino.IRhinoCommandContext) As RMA.Rhino.IRhinoCommand.result

    Dim id As System.Guid = SampleVbDockingDialogDockBar.ID()
    Dim bVisible As Boolean = RMA.UI.MRhinoDockBarManager.IsDockBarVisible(id)

    Dim prompt As String
    If bVisible Then
      prompt = String.Format("{0} window is visible. New value", EnglishCommandName())
    Else
      prompt = String.Format("{0} window is hidden. New value", EnglishCommandName())
    End If

    Dim go As New MRhinoGetOption()
    go.SetCommandPrompt(prompt)
    Dim h_option As Integer = go.AddCommandOption(New MRhinoCommandOptionName("Hide"))
    Dim s_option As Integer = go.AddCommandOption(New MRhinoCommandOptionName("Show"))
    Dim t_option As Integer = go.AddCommandOption(New MRhinoCommandOptionName("Toggle"))
    go.GetOption()
    If go.CommandResult() <> IRhinoCommand.result.success Then
      Return go.CommandResult()
    End If

    Dim opt As IRhinoCommandOption = go.[Option]()
    If opt Is Nothing Then
      Return IRhinoCommand.result.failure
    End If

    Dim option_index As Integer = opt.m_option_index
    If h_option = option_index Then
      If bVisible Then
        RMA.UI.MRhinoDockBarManager.ShowDockBar(id, False, False)
      End If
    ElseIf s_option = option_index Then
      If Not bVisible Then
        RMA.UI.MRhinoDockBarManager.ShowDockBar(id, True, False)
      End If
    ElseIf t_option = option_index Then
      If bVisible Then
        RMA.UI.MRhinoDockBarManager.ShowDockBar(id, False, False)
      Else
        RMA.UI.MRhinoDockBarManager.ShowDockBar(id, True, False)
      End If
    End If

    Return IRhinoCommand.result.success
  End Function

End Class
