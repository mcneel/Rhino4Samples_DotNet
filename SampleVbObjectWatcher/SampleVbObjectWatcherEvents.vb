Imports RMA.Rhino

Public Class SampleVbObjectWatcherEvents
  Inherits MRhinoEventWatcher

  Public Overrides Sub OnAddObject(ByRef doc As RMA.Rhino.MRhinoDoc, ByRef obj As RMA.Rhino.MRhinoObject)
    RhUtil.RhinoApp.Print("Event watcher caught OnAddObject" + vbCrLf)
    ' this is a good place perform any logic that you see necessary in your plug-in
  End Sub

  Public Overrides Sub OnDeleteObject(ByRef doc As RMA.Rhino.MRhinoDoc, ByRef obj As RMA.Rhino.MRhinoObject)
    RhUtil.RhinoApp.Print("Event watcher caught OnDeleteObject" + vbCrLf)
    ' this is a good place perform any logic that you see necessary in your plug-in
  End Sub

  Public Overrides Sub OnReplaceObject(ByRef doc As RMA.Rhino.MRhinoDoc, ByRef old_object As RMA.Rhino.MRhinoObject, ByRef new_object As RMA.Rhino.MRhinoObject)
    RhUtil.RhinoApp.Print("Event watcher caught OnReplaceObject" + vbCrLf)
    ' this is a good place perform any logic that you see necessary in your plug-in
  End Sub

  Public Overrides Sub OnUnDeleteObject(ByRef doc As RMA.Rhino.MRhinoDoc, ByRef [object] As RMA.Rhino.MRhinoObject)
    RhUtil.RhinoApp.Print("Event watcher caught OnUnDeleteObject" + vbCrLf)
    ' this is a good place perform any logic that you see necessary in your plug-in
  End Sub
End Class
