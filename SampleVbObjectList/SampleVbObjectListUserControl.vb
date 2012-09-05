Imports RMA.Rhino

Public Class SampleVbObjectListUserControl

  Public Sub New()

    ' This call is required by the Windows Form Designer.
    InitializeComponent()
    ' Add any initialization after the InitializeComponent() call.

    RegisterHandlers(Me.Visible())

  End Sub

  Public Shared Function BarID() As Guid
    Return New Guid("{35E677B5-3584-4E5C-BAFA-8E7FFA85B402}")
  End Function

  Private Sub RegisterHandlers(ByVal bRegister As Boolean)
    If bRegister Then
      ' Document closed and opened
      AddHandler MRhinoEventWatcher.CloseDocument, AddressOf MRhinoEventWatcher_CloseDocument
      AddHandler MRhinoEventWatcher.NewDocument, AddressOf MRhinoEventWatcher_NewDocument
      AddHandler MRhinoEventWatcher.EndOpenDocument, AddressOf MRhinoEventWatcher_EndOpenDocument
      ' Objects added or deleted
      AddHandler MRhinoEventWatcher.AddObject, AddressOf MRhinoEventWatcher_AddObject
      AddHandler MRhinoEventWatcher.DeleteObject, AddressOf MRhinoEventWatcher_DeleteObject
      ' Objects selected or deselected
      AddHandler MRhinoEventWatcher.SelectObject, AddressOf MRhinoEventWatcher_SelectObject
      AddHandler MRhinoEventWatcher.SelectObjects, AddressOf MRhinoEventWatcher_SelectObjects
      AddHandler MRhinoEventWatcher.DeselectObject, AddressOf MRhinoEventWatcher_DeselectObject
      AddHandler MRhinoEventWatcher.DeselectObjects, AddressOf MRhinoEventWatcher_DeselectObjects
      AddHandler MRhinoEventWatcher.DeselectAllObjects, AddressOf MRhinoEventWatcher_DeselectAllObjects
      AddAllObjects()
    Else
      ' Document closed and opened
      RemoveHandler MRhinoEventWatcher.CloseDocument, AddressOf MRhinoEventWatcher_CloseDocument
      RemoveHandler MRhinoEventWatcher.NewDocument, AddressOf MRhinoEventWatcher_NewDocument
      RemoveHandler MRhinoEventWatcher.EndOpenDocument, AddressOf MRhinoEventWatcher_EndOpenDocument
      ' Objects added or deleted
      RemoveHandler MRhinoEventWatcher.AddObject, AddressOf MRhinoEventWatcher_AddObject
      RemoveHandler MRhinoEventWatcher.DeleteObject, AddressOf MRhinoEventWatcher_DeleteObject
      ' Objects selected or deselected
      RemoveHandler MRhinoEventWatcher.SelectObject, AddressOf MRhinoEventWatcher_SelectObject
      RemoveHandler MRhinoEventWatcher.SelectObjects, AddressOf MRhinoEventWatcher_SelectObjects
      RemoveHandler MRhinoEventWatcher.DeselectObject, AddressOf MRhinoEventWatcher_DeselectObject
      RemoveHandler MRhinoEventWatcher.DeselectObjects, AddressOf MRhinoEventWatcher_DeselectObjects
      RemoveHandler MRhinoEventWatcher.DeselectAllObjects, AddressOf MRhinoEventWatcher_DeselectAllObjects
      RemoveAllObjects()
    End If
  End Sub

  Public Sub MRhinoEventWatcher_CloseDocument(ByVal doc As MRhinoDoc)
    RemoveAllObjects()
  End Sub

  Public Sub MRhinoEventWatcher_NewDocument(ByVal doc As MRhinoDoc)
    AddAllObjects()
  End Sub

  Public Sub MRhinoEventWatcher_EndOpenDocument(ByVal doc As MRhinoDoc, ByVal filename As String, ByVal bMerge As Boolean, ByVal bReference As Boolean)
    AddAllObjects()
  End Sub

  Public Sub MRhinoEventWatcher_AddObject(ByVal doc As MRhinoDoc, ByVal obj As IRhinoObject)
    If obj IsNot Nothing Then
      If obj.Attributes().IsVisible() Then
        Me.listBox.Items.Add(obj.Attributes().m_uuid.ToString())
      End If
    End If
  End Sub

  Public Sub MRhinoEventWatcher_DeleteObject(ByVal doc As MRhinoDoc, ByVal obj As IRhinoObject)
    If (obj IsNot Nothing) Then
      Me.listBox.Items.Remove(obj.Attributes().m_uuid.ToString())
    End If
  End Sub

  Public Sub MRhinoEventWatcher_SelectObject(ByVal doc As MRhinoDoc, ByVal obj As IRhinoObject)
    Me.selectTimer.Enabled = True
  End Sub

  Public Sub MRhinoEventWatcher_SelectObjects(ByVal doc As MRhinoDoc, ByVal obj() As IRhinoObject)
    Me.selectTimer.Enabled = True
  End Sub

  Public Sub MRhinoEventWatcher_DeselectObject(ByVal doc As MRhinoDoc, ByVal obj As IRhinoObject)
    Me.deselectTimer.Enabled = True
  End Sub

  Public Sub MRhinoEventWatcher_DeselectObjects(ByVal doc As MRhinoDoc, ByVal obj() As IRhinoObject)
    Me.deselectTimer.Enabled = True
  End Sub

  Public Sub MRhinoEventWatcher_DeselectAllObjects(ByVal doc As RMA.Rhino.MRhinoDoc, ByVal count As Integer)
    Me.deselectTimer.Enabled = True
  End Sub

  Private Sub AddAllObjects()
    RemoveAllObjects()

    Dim it As New MRhinoObjectIterator( _
              IRhinoObjectIterator.object_state.undeleted_objects, _
              IRhinoObjectIterator.object_category.active_and_reference_objects)
    it.IncludeLights()

    For Each obj As MRhinoObject In it
      If obj.Attributes().IsVisible() Then
        Me.listBox.Items.Add(obj.Attributes().m_uuid.ToString())
      End If
    Next
  End Sub

  Private Sub RemoveAllObjects()
    Me.listBox.Items.Clear()
  End Sub

  Private Sub TestObjectListUserControl_VisibleChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.VisibleChanged
    RegisterHandlers(Me.Visible())
  End Sub

  Private Sub listBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles listBox.SelectedIndexChanged
    ' TODO: items in the listbox were selected or unselected.
  End Sub

  Private Sub selectTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles selectTimer.Tick
    Me.selectTimer.Enabled = False
    ' TODO: objects in Rhino were selected.
  End Sub

  Private Sub deselectTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles deselectTimer.Tick
    Me.deselectTimer.Enabled = False
    ' TODO: objects in Rhino were deselected.
  End Sub
End Class
