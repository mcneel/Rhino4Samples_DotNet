Imports RMA.OpenNURBS
Imports RMA.Rhino

Public Class PackConduit
  Inherits MRhinoDisplayConduit

  Protected m_List As PackCircles

  Public Sub New(ByVal nList As PackCircles)
    MyBase.New(Channels, True)
    Me.m_List = nList
  End Sub

  Public Shared Function Channels() As MSupportChannels
    Return New MSupportChannels(MSupportChannels.SC_CALCBOUNDINGBOX Or MSupportChannels.SC_POSTDRAWOBJECTS)
  End Function

  Public Overrides Function ExecConduit(ByRef dp As RMA.Rhino.MRhinoDisplayPipeline, _
                                        ByVal current_channel As UInteger, _
                                        ByRef termination_flag As Boolean) As Boolean
    If Me.m_List Is Nothing Then Return True

    Select Case current_channel
      Case MSupportChannels.SC_CALCBOUNDINGBOX
        Dim box As OnBoundingBox = Me.m_List.BoundingBox

        With MyBase.m_pChannelAttrs.m_BoundingBox
          .m_min.x = Math.Min(.m_min.x, box.Min.x)
          .m_max.x = Math.Max(.m_max.x, box.Max.x)
          .m_min.y = Math.Min(.m_min.y, box.Min.y)
          .m_max.y = Math.Max(.m_max.y, box.Max.y)
          .m_min.z = Math.Min(.m_min.z, box.Min.z)
          .m_max.z = Math.Max(.m_max.z, box.Max.z)
        End With

      Case MSupportChannels.SC_POSTDRAWOBJECTS
        Me.m_List.Draw(dp)
    End Select

    Return True
  End Function
End Class

Public Class PackCircles
  Protected m_Circles() As PackingCircle
  Protected m_Base As On3dPoint
  Protected m_Box As OnBoundingBox

  'Generates a number of Packing circles in the constructor.
  'Random distribution is linear
  Public Sub New(ByVal ptBase As On3dPoint, ByVal intCount As Int32, _
                 ByVal minRadius As Double, ByVal maxRadius As Double)

    Me.m_Base = New On3dPoint(ptBase)

    ReDim Me.m_Circles(intCount - 1)
    Dim Rnd As New Random

    For i As Int32 = 0 To intCount - 1
      Dim nCenter As New On3dPoint(Me.m_Base.x + Rnd.NextDouble * minRadius, _
                                   Me.m_Base.y + Rnd.NextDouble * minRadius, _
                                   Me.m_Base.z)
      Dim nRadius As Double = minRadius + Rnd.NextDouble * (maxRadius - minRadius)
      Me.m_Circles(i) = New PackingCircle(nCenter, nRadius)
    Next

    DestroyBoundingBoxCache()
  End Sub

  'Add all circles to the document
  Public Sub Add()
    For Each C As PackingCircle In Me.m_Circles
      C.Add()
    Next
  End Sub

  'Draw all circles using a pipeline
  Public Sub Draw(ByVal dp As MRhinoDisplayPipeline)
    For Each C As PackingCircle In Me.m_Circles
      C.Draw(dp)
    Next
  End Sub

  'Randomize the order of all circles
  Protected Sub Jiggle()
    Dim Rnd As New Random
    Dim arrSort(Me.m_Circles.Length - 1) As Double

    For i As Int32 = 0 To Me.m_Circles.Length - 1
      arrSort(i) = Rnd.NextDouble
    Next

    Array.Sort(arrSort, Me.m_Circles)
  End Sub

  'Reorder the circles from furthest to nearest the boundingbox center
  Protected Sub Sort()
    Dim arrSort(Me.m_Circles.Length - 1) As Double

    For i As Int32 = 0 To Me.m_Circles.Length - 1
      arrSort(i) = -Me.m_Base.DistanceTo(Me.m_Circles(i).Center)
    Next

    Array.Sort(arrSort, Me.m_Circles)
  End Sub

  'Move all circles towards the boundingbox center
  Protected Sub Contract(ByVal Damping As Double)
    If Damping < 0.01 Then Return

    For Each C As PackingCircle In Me.m_Circles
      Dim vT As On3dVector = Me.m_Base - C.Center
      vT *= Damping
      C.Translate(vT)
    Next
  End Sub

  Public Enum PackingAlgorithm
    SimplePack = 0
    FastPack = 1
    DoublePack = 2
    RandomPack = 3

  End Enum

  'Perform a packing iteration
  Public Function Pack(ByVal pAlgorithm As PackingAlgorithm, Optional ByVal iDamping As Double = 1.0) As Boolean
    Dim rc As Boolean = False
    For Each C As PackingCircle In Me.m_Circles
      C.ResetMotion()
    Next

    Select Case pAlgorithm
      Case PackingAlgorithm.SimplePack, PackingAlgorithm.FastPack, PackingAlgorithm.DoublePack
        Me.Sort()
      Case PackingAlgorithm.RandomPack
        Me.Jiggle()
    End Select

    Select Case pAlgorithm
      Case PackingAlgorithm.FastPack, PackingAlgorithm.RandomPack, PackingAlgorithm.SimplePack
        For i As Int32 = 0 To Me.m_Circles.Length - 2
          For j As Int32 = i + 1 To Me.m_Circles.Length - 1
            rc = rc Or Me.m_Circles(i).FastPack(Me.m_Circles(j))
          Next
        Next

      Case PackingAlgorithm.DoublePack
        For i As Int32 = 0 To Me.m_Circles.Length - 2
          For j As Int32 = i + 1 To Me.m_Circles.Length - 1
            rc = rc Or Me.m_Circles(i).DoublePack(Me.m_Circles(j))
          Next
        Next

    End Select

    Select Case pAlgorithm
      Case PackingAlgorithm.DoublePack, PackingAlgorithm.FastPack, PackingAlgorithm.RandomPack
        Me.Contract(iDamping)
    End Select

    DestroyBoundingBoxCache()
    Return rc
  End Function

  'Calculate the boundingbox of all circles
  Public Function BoundingBox() As OnBoundingBox
    If Me.m_Box IsNot Nothing Then Return Me.m_Box

    Dim x() As Double = {Double.MaxValue, Double.MinValue}
    Dim y() As Double = {Double.MaxValue, Double.MinValue}
    Dim z() As Double = {Double.MaxValue, Double.MinValue}

    For Each C As PackingCircle In Me.m_Circles
      x(0) = Math.Min(x(0), C.plane.origin.x - C.radius)
      x(1) = Math.Max(x(1), C.plane.origin.x + C.radius)

      y(0) = Math.Min(y(0), C.plane.origin.y - C.radius)
      y(1) = Math.Max(y(1), C.plane.origin.y + C.radius)

      z(0) = Math.Min(z(0), C.plane.origin.z - C.radius)
      z(1) = Math.Max(z(1), C.plane.origin.z + C.radius)
    Next

    Me.m_Box = New OnBoundingBox(New On3dPoint(x(0), y(0), z(0)), _
                                 New On3dPoint(x(1), y(1), z(1)))
    Return Me.m_Box
  End Function

  'Erase the boundingbox cache
  Public Sub DestroyBoundingBoxCache()
    Me.m_Box = Nothing
  End Sub
End Class

Public Class PackingCircle
  Inherits OnCircle

  'If the circle is in motion, it will be drawn red
  Protected m_IsInMotion As Boolean = False

  'A number of constructors for easy instancing. I only use one for now.
  Public Sub New(ByVal iCircle As OnCircle)
    MyBase.New(iCircle)
  End Sub
  Public Sub New(ByVal iCenter As On3dPoint, ByVal Radius As Double)
    MyBase.New(OnPlane.World_xy, iCenter, Radius)
  End Sub
  Public Sub New(ByVal iPlane As OnPlane, ByVal Radius As Double)
    MyBase.New(iPlane, Radius)
  End Sub

  'Reset the motion flag to <no motion>. This should be called whenever a new iteration starts.
  Public Sub ResetMotion()
    Me.m_IsInMotion = False
  End Sub

  'Compare this circle to another circle and move this circle in case of an overlap
  Public Function FastPack(ByVal iOther As PackingCircle) As Boolean
    Dim A As IOn3dPoint = Me.Center
    Dim B As IOn3dPoint = iOther.Center

    Dim d As Double = (A.x - B.x) * (A.x - B.x) + _
                      (A.y - B.y) * (A.y - B.y)
    Dim r As Double = Me.radius + iOther.radius

    If (d < ((r * r) - 0.01 * RhUtil.RhinoApp.ActiveDoc.AbsoluteTolerance)) Then
      'If the above line evaluates to TRUE, we have an overlap
      Dim V As New On3dVector(A.x - B.x, A.y - B.y, 0)
      V.Unitize()
      V *= (r - Math.Sqrt(d))

      Me.Translate(V)
      Me.m_IsInMotion = True

      Return True
    Else
      Return False
    End If
  End Function

  'Compare this circle to another circle and moves both circles in case of an overlap
  Public Function DoublePack(ByVal iOther As PackingCircle) As Boolean
    Dim A As IOn3dPoint = Me.Center
    Dim B As IOn3dPoint = iOther.Center

    Dim d As Double = (A.x - B.x) * (A.x - B.x) + _
                      (A.y - B.y) * (A.y - B.y)
    Dim r As Double = Me.radius + iOther.radius

    If (d < ((r * r) - 0.01 * RhUtil.RhinoApp.ActiveDoc.AbsoluteTolerance)) Then
      'If the above line evaluates to TRUE, we have an overlap
      Dim V As New On3dVector(A.x - B.x, A.y - B.y, 0)
      V.Unitize()
      V *= 0.5 * (r - Math.Sqrt(d))

      Me.Translate(V)

      V.Reverse()
      iOther.Translate(V)

      Me.m_IsInMotion = True

      Return True
    Else
      Return False
    End If
  End Function




  'The ADD function adds the current circle to Rhino. If this function is called successively, it will also
  'remove the last created object. Since I now use a Conduit to display the progress, this feature is no
  'longer used, but it is handy none the less.
  Protected m_UUID As Guid
  Public Function Add() As Boolean
    Dim iObj As IRhinoObject = RhUtil.RhinoApp.ActiveDoc.LookupObject(Me.m_UUID)
    If iObj IsNot Nothing Then RhUtil.RhinoApp.ActiveDoc.DeleteObject(New MRhinoObjRef(iObj))

    iObj = RhUtil.RhinoApp.ActiveDoc.AddCurveObject(Me)
    Me.m_UUID = iObj.Attributes.m_uuid
  End Function

  'A constant color used to indicate circles in motion (dark red)
  Protected Shared m_MotionColor As New OnColor(200, 0, 0)

  'Draw this circle using a Pipeline.
  Public Sub Draw(ByVal dp As MRhinoDisplayPipeline)
    If Me.m_IsInMotion Then
      Dim iOldColor As OnColor = dp.GetRhinoVP.DrawColor
      dp.GetRhinoVP.SetDrawColor(m_MotionColor)
      dp.SetCurveThickness(2)

      dp.GetRhinoVP.DrawCircle(Me)

      dp.SetCurveThickness(1)
      dp.GetRhinoVP.SetDrawColor(iOldColor)
    Else
      dp.GetRhinoVP.DrawCircle(Me)
    End If
  End Sub
End Class