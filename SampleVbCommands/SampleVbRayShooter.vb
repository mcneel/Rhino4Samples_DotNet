Imports System
Imports RMA.Rhino
Imports RMA.OpenNURBS
Imports RMA.Rhino.RhUtil

'''<summary>
''' A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
''' DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
'''</summary>
Public Class SampleVbRayShooter
  Inherits RMA.Rhino.MRhinoCommand

  '''<summary>
  ''' Rhino tracks commands by their unique ID. Every command must have a unique id.
  ''' The Guid created by the project wizard is unique. You can create more Guids using
  ''' the "Create Guid" tool in the Tools menu.
  '''</summary>
  '''<returns>The id for this command</returns>
  Public Overrides Function CommandUUID() As System.Guid
    Return New Guid("{b5bd7725-e8ca-4e26-b8ee-3bf25fdf35f2}")
  End Function

  '''<returns>The command name as it appears on the Rhino command line</returns>
  Public Overrides Function EnglishCommandName() As String
    Return "SampleVbRayShooter"
  End Function

  '''<summary> This gets called when when the user runs this command.</summary>
  Public Overrides Function RunCommand(ByVal context As RMA.Rhino.IRhinoCommandContext) As RMA.Rhino.IRhinoCommand.result
    Dim gs As New MRhinoGetObject()
    gs.SetCommandPrompt("Select landmark for distance analysis")
    gs.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.polysrf_object Or IRhinoGetObject.GEOMETRY_TYPE_FILTER.surface_object)
    gs.GetObjects(1, 1)
    If (gs.CommandResult() <> IRhinoCommand.result.success) Then
      Return gs.CommandResult()
    End If

    Dim snode_list As New System.Collections.Generic.List(Of IOnSurfaceTree)()

    Dim brep As IOnBrep = gs.Object(0).Brep()
    If (brep IsNot Nothing) Then
      For fi As Integer = 0 To brep.m_F.Count() - 1
        Dim s As IOnSurfaceTree = brep.m_F(fi).SurfaceTree()
        If (s IsNot Nothing) Then snode_list.Add(s)
      Next
    End If

    If (snode_list.Count < 1) Then
      RhUtil.RhinoApp().Print(String.Format("No suitable landmark was selected." + vbCrLf))
      Return IRhinoCommand.result.nothing
    End If

    Dim L As New OnLine()

    Dim gp As New MRhinoGetPoint()
    gp.SetCommandPrompt("Start of ray")
    gp.GetPoint()
    If (gp.CommandResult() = IRhinoCommand.result.success) Then
      L.from = gp.Point()
    Else
      Return gp.CommandResult()
    End If

    gp.SetCommandPrompt("Ray direction")
    gp.SetBasePoint(L.from)
    gp.DrawLineFromPoint(L.from, True)
    gp.GetPoint()
    If (gp.CommandResult() = IRhinoCommand.result.success) Then
      L.to = gp.Point()
    Else
      Return gp.CommandResult()
    End If

    If (L.IsValid() = False) OrElse (L.Length() <= OnUtil.On_ZERO_TOLERANCE) Then
      Return IRhinoCommand.result.nothing
    End If

    Dim P As New On3dPoint(L.from) ' start of ray
    Dim D As New On3dVector(L.Direction()) 'ray direction

    Dim line As New OnLine() ' path of ray
    line.from = P

    Dim ray As New OnRayShooter()
    Dim hit As New OnX_EVENT()
    Dim rc As Boolean = ray.Shoot(P, D, snode_list.ToArray(), hit)
    If (rc = True) Then
      line.to = hit.m_pointB(0) ' surface point
      RhUtil.RhinoApp().Print(String.Format("Distance from start of ray to landmark is {0}." + vbCrLf, line.Length()))
      context.m_doc.AddPointObject(line.from)
      context.m_doc.AddPointObject(line.to)
      context.m_doc.Redraw()
    Else
      RhUtil.RhinoApp().Print(String.Format("Ray did not intersect with landmark." + vbCrLf))
    End If

    Return IRhinoCommand.result.success
  End Function
End Class
