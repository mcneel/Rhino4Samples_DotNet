Imports RMA.Rhino
Imports RMA.OpenNURBS
Imports RMA.Rhino.RhUtil

'''<summary>
''' A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
''' DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
'''</summary>
Public Class SampleVbDumpInstanceDefinitions
  Inherits RMA.Rhino.MRhinoCommand

  '''<summary>
  ''' Rhino tracks commands by their unique ID. Every command must have a unique id.
  ''' The Guid created by the project wizard is unique. You can create more Guids using
  ''' the "Create Guid" tool in the Tools menu.
  '''</summary>
  '''<returns>The id for this command</returns>
  Public Overrides Function CommandUUID() As System.Guid
    Return New Guid("{7ff7f8a7-fbe3-456a-8998-1dd2cae9cd4c}")
  End Function

  '''<returns>The command name as it appears on the Rhino command line</returns>
  Public Overrides Function EnglishCommandName() As String
    Return "SampleVbDumpInstanceDefinitions"
  End Function

  '''<summary> This gets called when when the user runs this command.</summary>
  Public Overrides Function RunCommand(ByVal context As RMA.Rhino.IRhinoCommandContext) As RMA.Rhino.IRhinoCommand.result

    Dim idef_table As MRhinoInstanceDefinitionTable = context.m_doc.m_instance_definition_table
    Dim idef_count As Integer = idef_table.InstanceDefinitionCount()
    If (idef_count = 0) Then
      RhUtil.RhinoApp().Print("No instance definitions found." + vbCrLf)
      Return IRhinoCommand.result.nothing
    End If

    Dim num_printed As Integer = 0
    For i As Integer = 0 To idef_count - 1
      Dim idef As IRhinoInstanceDefinition = idef_table(i)
      If (idef IsNot Nothing AndAlso idef.IsDeleted() = False) Then
        Dim idef_name As String = idef.Name()
        RhUtil.RhinoApp().Print(String.Format("Instance definition {0} = {1}" + vbCrLf, num_printed, idef_name))
        num_printed += 1
      End If
    Next

    Return IRhinoCommand.result.success

  End Function
End Class
