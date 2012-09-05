Imports RMA.Rhino
Imports RMA.OpenNURBS
Imports RMA.Rhino.RhUtil

'''<summary>
''' A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
''' DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
'''</summary>
Public Class SampleVbDupEdge
  Inherits RMA.Rhino.MRhinoCommand

  '''<summary>
  ''' Rhino tracks commands by their unique ID. Every command must have a unique id.
  ''' The Guid created by the project wizard is unique. You can create more Guids using
  ''' the "Create Guid" tool in the Tools menu.
  '''</summary>
  '''<returns>The id for this command</returns>
  Public Overrides Function CommandUUID() As System.Guid
    Return New Guid("{8aa2d8b5-417c-41bb-8989-ed836cc76003}")
  End Function

  '''<returns>The command name as it appears on the Rhino command line</returns>
  Public Overrides Function EnglishCommandName() As String
    Return "SampleVbDupEdge"
  End Function

  '''<summary> This gets called when when the user runs this command.</summary>
  Public Overrides Function RunCommand(ByVal context As RMA.Rhino.IRhinoCommandContext) As RMA.Rhino.IRhinoCommand.result
    Dim go As New MRhinoGetObject()
    go.SetCommandPrompt("Select edge curve")
    go.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.curve_object)
    go.SetGeometryAttributeFilter(IRhinoGetObject.GEOMETRY_ATTRIBUTE_FILTER.edge_curve)
    go.EnableSubObjectSelect(True)
    go.GetObjects(1, 1)
    If (go.CommandResult() <> IRhinoCommand.result.success) Then
      Return go.CommandResult()
    End If

    Dim object_ref As MRhinoObjRef = go.Object(0)

    Dim edge As IOnBrepEdge = object_ref.Edge()
    If (edge Is Nothing) Then
      Return IRhinoCommand.result.failure
    End If

    Dim brep As IOnBrep = edge.Brep()
    If (brep Is Nothing) Then
      Return IRhinoCommand.result.failure
    End If

    Dim curve As OnCurve = edge.DuplicateCurve()
    If (curve IsNot Nothing) Then
      If (brep.m_T(edge.m_ti(0)).m_bRev3d = True) Then
        curve.Reverse()
      End If
      context.m_doc.AddCurveObject(curve)
      context.m_doc.Redraw()
    End If

    Return IRhinoCommand.result.success

  End Function
End Class
