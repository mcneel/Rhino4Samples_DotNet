Imports RMA.Rhino
Imports RMA.OpenNURBS
Imports RMA.Rhino.RhUtil

'''<summary>
''' A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
''' DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
'''</summary>
Public Class SampleVbBrepPlaneSplit
  Inherits RMA.Rhino.MRhinoCommand

  '''<summary>
  ''' Rhino tracks commands by their unique ID. Every command must have a unique id.
  ''' The Guid created by the project wizard is unique. You can create more Guids using
  ''' the "Create Guid" tool in the Tools menu.
  '''</summary>
  '''<returns>The id for this command</returns>
  Public Overrides Function CommandUUID() As System.Guid
    Return New Guid("{293b5d48-7f1c-42fe-9e34-b61a0723126f}")
  End Function

  '''<returns>The command name as it appears on the Rhino command line</returns>
  Public Overrides Function EnglishCommandName() As String
    Return "SampleVbBrepPlaneSplit"
  End Function

  '''<summary> This gets called when when the user runs this command.</summary>
  Public Overrides Function RunCommand(ByVal context As RMA.Rhino.IRhinoCommandContext) As RMA.Rhino.IRhinoCommand.result

    ' First, collect all the BReps to split.
    Dim get_brep As New MRhinoGetObject
    get_brep.SetCommandPrompt("Breps to split")
    get_brep.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.surface_object Or IRhinoGetObject.GEOMETRY_TYPE_FILTER.polysrf_object)

    Select Case get_brep.GetObjects(1, 0)
      Case IRhinoGet.result.object
        'ok, we've got a set of breps, we'll return to this later, first we need to ask for the cutting plane.
      Case Else
        Return IRhinoCommand.result.cancel
    End Select

    'Get the final plane
    Dim P As IOnPlane = Nothing
    Select Case Me.GetPlane(P)
      Case IRhinoCommand.result.success
        'ok, we are set to go
      Case Else
        Return IRhinoCommand.result.cancel
    End Select

    'Iterate over all object references
    For i As Int32 = 0 To get_brep.ObjectCount - 1
      'Get the BRep and the Boundingbox
      Dim brep As IOnBrep = get_brep.Object(i).Brep()
      Dim bbox As New OnBoundingBox(brep.BoundingBox())

      'Grow the boundingbox in all directions
      'If the boundingbox is flat (zero volume or even zero area) then the RhinoPlaneThroughBox method will fail.
      bbox.m_min.x -= 1.0
      bbox.m_min.y -= 1.0
      bbox.m_min.z -= 1.0
      bbox.m_max.x += 1.0
      bbox.m_max.y += 1.0
      bbox.m_max.z += 1.0

      'Create a new plane surface which is guaranteed to intersect the brep
      Dim srf_plane As OnPlaneSurface = RhUtil.RhinoPlaneThroughBox(P, bbox)

      If (srf_plane Is Nothing) Then
        'This is rare, it will most likely not happen unless either the plane or the boundingbox are invalid
        RhUtil.RhinoApp.Print("Cutting plane could not be constructed..." & vbCrLf)
      Else
        'Split the BRep with the cutting plane
        Dim pieces As OnBrep() = Nothing
        If (RhUtil.RhinoBrepSplit(brep, srf_plane.BrepForm(), context.m_doc.AbsoluteTolerance, pieces)) Then
          'We have a successful spit, delete the original and add the new objects
          For Each piece As OnBrep In pieces
            context.m_doc.AddBrepObject(piece)
          Next

          context.m_doc.AddSurfaceObject(srf_plane)
          context.m_doc.DeleteObject(get_brep.Object(i))

        Else
          RhUtil.RhinoApp.Print("Plane does not intersect BRep..." & vbCrLf)
        End If
      End If
    Next

    context.m_doc.Regen()
    Return IRhinoCommand.result.success
  End Function

  'Wraps all the UI for the Plane getter.
  'Returns Success if the plane is valid, otherwise Cancel
  Protected Function GetPlane(ByRef iPlane As IOnPlane) As IRhinoCommand.result
    'The RhGetPlane class is actually 3 classes in 1. Depending on which points (A, B, C) are Nothing, it behaves differently.
    'These three points will be picked by the user, where A is the plane origin, B identifies the x-direction and C the y-direction
    Dim A As On3dPoint = Nothing
    Dim B As On3dPoint = Nothing
    Dim C As On3dPoint = Nothing

    Dim get_plane As RhGetPlane

    'Create a new instance of RhGetPlane using three Nothings
    get_plane = New RhGetPlane(A, B, C)
    Select Case get_plane.GetPoint()
      Case IRhinoGet.result.point
        'ok, we've got the plane origin
        A = New On3dPoint(get_plane.A)

      Case Else
        Return IRhinoCommand.result.cancel
    End Select

    'Create a new instance of RhGetPlane using the known origin point. B and C are still Nothing
    get_plane = New RhGetPlane(A, B, C)
    Select Case get_plane.GetPoint()
      Case IRhinoGet.result.point
        'ok, we've got the plane x-direction.
        B = New On3dPoint(get_plane.B)

        'If A and B are the same point, we must abort
        If (A.DistanceTo(B) = 0.0) Then Return IRhinoCommand.result.failure

      Case Else
        Return IRhinoCommand.result.cancel
    End Select

    'Create a new instance of RhGetPlane using the known origin point and x-axis. C is still Nothing
    get_plane = New RhGetPlane(A, B, C)
    Select Case get_plane.GetPoint()
      Case IRhinoGet.result.point
        'ok, we've got the plane y-direction
        C = New On3dPoint(get_plane.C)

        'If A, B or C are coincident, we must abort
        If (A.DistanceTo(C) = 0.0) Then Return IRhinoCommand.result.failure
        If (B.DistanceTo(C) = 0.0) Then Return IRhinoCommand.result.failure
      Case Else
        Return IRhinoCommand.result.cancel
    End Select

    iPlane = New OnPlane(get_plane.Plane)
    Return IRhinoCommand.result.success
  End Function
End Class

''' <summary>
''' Custom MRhinoGetPoint-inherited class
''' </summary>
Public Class RhGetPlane
  Inherits MRhinoGetPoint

  Protected Enum GetPlaneProgress
    Origin = 0
    XAxis = 1
    YAxis = 2
  End Enum

  Protected m_A, m_B, m_C As On3dPoint
  Protected m_P As OnPlane
  Protected m_progress As GetPlaneProgress

  Public Sub New(ByVal nA As On3dPoint, ByVal nB As On3dPoint, ByVal nC As On3dPoint)
    'Set all points. These could all be Nothing
    Me.m_A = nA
    Me.m_B = nB
    Me.m_C = nC

    If (Me.m_A Is Nothing) Then
      'This means we have to start at the bottom
      Me.SetCommandPrompt("Origin of cutting plane")
      Me.m_progress = GetPlaneProgress.Origin

    ElseIf (Me.m_B Is Nothing) Then
      'We've got an origin, but no x-axis direction
      Me.SetCommandPrompt("X-Direction of cutting plane")
      Me.m_progress = GetPlaneProgress.XAxis
      MyBase.SetBasePoint(Me.m_A)

    ElseIf (Me.m_C Is Nothing) Then
      'We've got an origin and an x-axis direction, but no y-axis direction yet
      Me.SetCommandPrompt("Y-Direction of cutting plane")
      Me.m_progress = GetPlaneProgress.YAxis

      'We need to constrain the picker to the plane which goes through A and is perpendicular to AB
      MyBase.Constrain(New OnPlane(Me.m_A, Me.m_B - Me.m_A))
      MyBase.SetBasePoint(Me.m_A)
    End If

  End Sub

  Public ReadOnly Property A() As On3dPoint
    Get
      Return Me.m_A
    End Get
  End Property
  Public ReadOnly Property B() As On3dPoint
    Get
      Return Me.m_B
    End Get
  End Property
  Public ReadOnly Property C() As On3dPoint
    Get
      Return Me.m_C
    End Get
  End Property
  Public ReadOnly Property Plane() As OnPlane
    Get
      Return Me.m_P
    End Get
  End Property

  'Calculates the plane through A, AB or ABC depending on the mode of the picker
  Public Function ComputePlane() As Boolean
    Me.m_P = Nothing

    Select Case Me.m_progress
      Case GetPlaneProgress.Origin
        'If we're picking the origin, create a horizontal plane through A
        If (Me.A Is Nothing) Then Return False
        Me.m_P = New OnPlane(Me.A, New On3dVector(1, 0, 0), New On3dVector(0, 1, 0))

      Case GetPlaneProgress.XAxis
        'If we're picking the x-axis, create a plane through A and perpendicular to AB
        If (Me.B Is Nothing) Then Return False
        If (Me.A.DistanceTo(Me.B) = 0.0) Then Return False

        'We know the x-direction of the plane (A-B), but the y-direction is undercostrained
        Dim v_x As On3dVector = Me.B - Me.A
        Dim v_y As On3dVector

        'We'll make a vector which is definitely different from v_x, and the get the cross product to invent a y-axis direction.
        If (v_x.x = 0.0 And v_x.y = 0.0) Then
          v_y = New On3dVector(1, 0, 0)
        Else
          v_y = New On3dVector(0, 0, 1)
        End If

        v_y = OnUtil.ON_CrossProduct(v_x, v_y)
        Me.m_P = New OnPlane(Me.m_A, v_x, v_y)

      Case GetPlaneProgress.YAxis
        'If we're picking the y-axis, create a simple plane
        If (Me.C Is Nothing) Then Return False
        If (Me.A.DistanceTo(Me.C) = 0.0) Then Return False
        If (Me.B.DistanceTo(Me.C) = 0.0) Then Return False

        Me.m_P = New OnPlane(Me.A, Me.B - Me.A, Me.C - Me.A)

    End Select

    Return True
  End Function

  Public Overrides Sub OnMouseMove(ByVal vp As RMA.Rhino.MRhinoViewport, ByVal nFlags As UInteger, ByVal point As RMA.OpenNURBS.IOn3dPoint, ByVal view_wnd_point As System.Drawing.Point)
    'Depending on picking mode, copy the point into the apropriate slot
    Select Case Me.m_progress
      Case GetPlaneProgress.Origin
        Me.m_A = New On3dPoint(point)
      Case GetPlaneProgress.XAxis
        Me.m_B = New On3dPoint(point)
      Case GetPlaneProgress.YAxis
        Me.m_C = New On3dPoint(point)
    End Select

    'Update the cached plane definition
    Me.ComputePlane()
  End Sub

  Public Overrides Sub DynamicDraw(ByVal hdc As System.IntPtr, ByVal viewport As RMA.Rhino.MRhinoViewport, ByVal pt As RMA.OpenNURBS.IOn3dPoint)
    'First, draw all points and dotted lines.
    If (Me.A IsNot Nothing) Then
      viewport.DrawPoint(Me.A)
    End If

    If (Me.B IsNot Nothing) Then
      viewport.DrawPoint(Me.B)
      viewport.DrawDottedLine(Me.A, Me.B)
    End If

    If (Me.C IsNot Nothing) Then
      viewport.DrawPoint(Me.C)
      viewport.DrawDottedLine(Me.A, Me.C)
    End If

    'Then, if the plane definition is not nothing, draw a construction plane.
    If (Me.Plane IsNot Nothing) Then

      Dim c_plane As New On3dmConstructionPlane
      Dim g_size As Double = 1.0

      c_plane.m_plane = Me.Plane
      c_plane.m_grid_line_count = 10
      c_plane.m_grid_thick_frequency = 5
      c_plane.m_grid_spacing = g_size

      Dim col_thin As UInt32 = New OnColor(RhinoApp.AppSettings.GridSettings.m_thin_line_color)
      Dim col_thick As UInt32 = New OnColor(RhinoApp.AppSettings.GridSettings.m_thick_line_color)
      Dim col_x As UInt32 = New OnColor(RhinoApp.AppSettings.GridSettings.m_xaxis_color)
      Dim col_y As UInt32 = New OnColor(RhinoApp.AppSettings.GridSettings.m_yaxis_color)

      viewport.DrawConstructionPlane(c_plane, True, True, col_thin, col_thick, col_x, col_y)
    End If
  End Sub
End Class