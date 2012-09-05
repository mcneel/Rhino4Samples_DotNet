Imports RMA.Rhino
Imports RMA.OpenNURBS
Imports RMA.Rhino.RhUtil

'''<summary>
''' A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
''' DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
'''</summary>
Public Class SampleVbDumpInstanceDefinitionTree
  Inherits RMA.Rhino.MRhinoCommand

  '''<summary>
  ''' Rhino tracks commands by their unique ID. Every command must have a unique id.
  ''' The Guid created by the project wizard is unique. You can create more Guids using
  ''' the "Create Guid" tool in the Tools menu.
  '''</summary>
  '''<returns>The id for this command</returns>
  Public Overrides Function CommandUUID() As System.Guid
    Return New Guid("{c04e2b25-0680-4b1d-983f-38bd30a9ea30}")
  End Function

  '''<returns>The command name as it appears on the Rhino command line</returns>
  Public Overrides Function EnglishCommandName() As String
    Return "SampleVbDumpInstanceDefinitionTree"
  End Function

  '''<summary> This gets called when when the user runs this command.</summary>
  Public Overrides Function RunCommand(ByVal context As RMA.Rhino.IRhinoCommandContext) As RMA.Rhino.IRhinoCommand.result

    Dim idef_table As MRhinoInstanceDefinitionTable = context.m_doc.m_instance_definition_table
    Dim idef_count As Integer = idef_table.InstanceDefinitionCount()
    If (idef_count = 0) Then
      RhinoApp().Print("No instance definitions found." + vbCrLf)
      Return IRhinoCommand.result.nothing
    End If

    Dim writer As New System.IO.StringWriter()
    Dim dump As New OnTextLog(writer)
    dump.SetIndentSize(4)

    For i As Integer = 0 To idef_count - 1
      DumpInstanceDefinition(idef_table(i), dump, True)
    Next

    RhUtil.RhinoApp().Print(writer.ToString() + vbCrLf)

    Return IRhinoCommand.result.success

  End Function

  Public Sub DumpInstanceDefinition(ByVal idef As IRhinoInstanceDefinition, ByRef dump As OnTextLog, ByVal bRoot As Boolean)

    If (idef IsNot Nothing AndAlso idef.IsDeleted() = False) Then

      Dim node As String
      If (bRoot = True) Then
        node = ChrW(&H2500)
      Else
        node = ChrW(&H2514)
      End If
      dump.Print(String.Format("{0} Instance definition {1} = {2}" + vbCrLf, node, idef.Index(), idef.Name()))

      Dim idef_object_count As Integer = idef.ObjectCount()
      If (idef_object_count > 0) Then
        dump.PushIndent()
        For i As Integer = 0 To idef_object_count - 1
          Dim obj As IRhinoObject = idef.Object(i)
          If (obj IsNot Nothing) Then
            Dim iref As IRhinoInstanceObject = MRhinoInstanceObject.ConstCast(obj)
            If (iref IsNot Nothing) Then
              DumpInstanceDefinition(iref.InstanceDefinition(), dump, False) ' Recursive...
            Else
              dump.Print(String.Format("{0} Object {1} = {2}" + vbCrLf, ChrW(&H2514), i, obj.ShortDescription(False)))
            End If
          End If
        Next
        dump.PopIndent()
      End If

    End If

  End Sub

End Class
