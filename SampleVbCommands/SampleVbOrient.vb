Imports RMA.Rhino
Imports RMA.OpenNURBS
Imports RMA.Rhino.RhUtil

''' <summary>
''' Dynamic transformation helper
''' </summary>
Public Class SampleVbRotationXform
  Inherits RMA.Rhino.MRhinoGetXform

  Private m_face As IOnBrepFace
  Private m_source_plane As OnPlane
  Private m_target_plane As New OnPlane

  Public Sub SetBrepFace(ByRef face As RMA.OpenNURBS.IOnBrepFace)
    m_face = face
  End Sub

  Public Sub SetSourcePlane(ByVal plane As OnPlane)
    m_source_plane = plane
  End Sub

  Public Overrides Function CalculateTransform(ByRef vp As RMA.Rhino.MRhinoViewport, ByVal pt As RMA.OpenNURBS.IOn3dPoint, ByRef xform As RMA.OpenNURBS.OnXform) As Boolean

    xform.Identity()

    If (m_face Is Nothing) Then
      Return False
    End If

    Dim u As Double = 0.0
    Dim v As Double = 0.0

    If (True = m_face.GetClosestPoint(pt, u, v)) Then
      If (True = m_face.FrameAt(u, v, m_target_plane)) Then
        ' If the face's orientation is opposite of natural surface orientation, 
        ' then flip the plane.
        If (True = m_face.m_bRev) Then
          m_target_plane.yaxis.Reverse()
          m_target_plane.zaxis.Reverse()
          m_target_plane.UpdateEquation()
        End If
      End If
    End If

    xform.Rotation(m_source_plane, m_target_plane)
    Return xform.IsValid()

  End Function

End Class


'''<summary>
''' A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
''' DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
'''</summary>
Public Class SampleVbOrient
  Inherits RMA.Rhino.MRhinoCommand

  '''<summary>
  ''' Rhino tracks commands by their unique ID. Every command must have a unique id.
  ''' The Guid created by the project wizard is unique. You can create more Guids using
  ''' the "Create Guid" tool in the Tools menu.
  '''</summary>
  '''<returns>The id for this command</returns>
  Public Overrides Function CommandUUID() As System.Guid
    Return New Guid("{bb1b0f46-c690-4094-87bc-a27e95d6381e}")
  End Function

  '''<returns>The command name as it appears on the Rhino command line</returns>
  Public Overrides Function EnglishCommandName() As String
    Return "SampleVbOrient"
  End Function

  '''<summary> This gets called when when the user runs this command.</summary>
  Public Overrides Function RunCommand(ByVal context As RMA.Rhino.IRhinoCommandContext) As RMA.Rhino.IRhinoCommand.result
    ' Select objects to orient
    Dim go As New MRhinoGetObject()
    go.SetCommandPrompt("Select objects to orient")
    go.EnableSubObjectSelect(False)
    go.EnableGroupSelect(True)
    go.GetObjects(1, 0)
    If (go.CommandResult() <> IRhinoCommand.result.success) Then
      Return go.CommandResult()
    End If

    ' Point to orient from
    Dim gp As New MRhinoGetPoint()
    gp.SetCommandPrompt("Point to orient from")
    gp.GetPoint()
    If (gp.CommandResult() <> IRhinoCommand.result.success) Then
      Return gp.CommandResult()
    End If

    ' Define source plane
    Dim view As MRhinoView = gp.View()
    If (view Is Nothing) Then Return IRhinoCommand.result.failure

    Dim plane As New OnPlane(view.ActiveViewport().ConstructionPlane().m_plane)
    plane.SetOrigin(gp.Point())

    ' Surface to orient on
    Dim gs As New MRhinoGetObject()
    gs.SetCommandPrompt("Surface to orient on")
    gs.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.surface_object)
    gs.EnableSubObjectSelect(True)
    gs.EnableDeselectAllBeforePostSelect(False)
    gs.EnableOneByOnePostSelect()
    gs.GetObjects(1, 1)
    If (gs.CommandResult() <> IRhinoCommand.result.success) Then
      Return gs.CommandResult()
    End If

    Dim objref As MRhinoObjRef = gs.Object(0)
    ' Get selected surface object
    Dim obj As IRhinoObject = objref.Object()
    If (obj Is Nothing) Then Return IRhinoCommand.result.failure
    ' Get selected surface (face)
    Dim face As IOnBrepFace = objref.Face()
    If (face Is Nothing) Then Return IRhinoCommand.result.failure
    ' Unselect surface
    obj.Select(False)

    ' Point on surface to orient to
    Dim gr As New SampleVbRotationXform()
    gr.SetCommandPrompt("Point on surface to orient to")
    gr.Constrain(face)
    gr.AppendObjects(go)
    gr.SetBrepFace(face)
    gr.SetSourcePlane(plane)
    gr.GetXform()
    If (gr.CommandResult() <> IRhinoCommand.result.success) Then
      Return gr.CommandResult()
    End If

    view = gr.View()
    If (view Is Nothing) Then Return IRhinoCommand.result.failure

    Dim xform As New OnXform
    If (True = gr.CalculateTransform(view.ActiveViewport(), gr.Point(), xform)) Then
      For i As Integer = 0 To go.ObjectCount() - 1
        context.m_doc.TransformObject(go.Object(i), xform, False)
      Next
      context.m_doc.Redraw()
    End If

    Return IRhinoCommand.result.success
  End Function
End Class
