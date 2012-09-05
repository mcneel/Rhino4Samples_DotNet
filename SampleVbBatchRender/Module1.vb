Imports System
Imports System.IO

Module Module1

  Sub Main(ByVal cmdArgs() As String)

    If (cmdArgs.Length <> 1) Then
      Console.WriteLine("Syntax: BatchRender <filename>")
      Exit Sub
    End If

    Dim strPath As String = cmdArgs(0).Trim()
    If (strPath.Length = 0) Then
      Console.WriteLine("Syntax: BatchRender <filename>")
      Exit Sub
    End If

    If (False = File.Exists(strPath)) Then
      Console.WriteLine("BatchRender Error: file not found - " + strPath)
      Exit Sub
    End If

    If (0 = InStr(LCase(strPath), ".3dm")) Then
      Console.WriteLine("BatchRender Error: not a Rhino file - " + strPath)
      Exit Sub
    End If

    Dim oRhino As Rhino4.Rhino4Application
    On Error Resume Next
    oRhino = CreateObject("Rhino4.Application")
    If (Err.Number <> 0) Then
      Console.WriteLine("BatchRender Error: cannot create Rhino4.Application")
      Exit Sub
    End If

    Dim oRhinoScript As RhinoScript4.RhinoScript = Nothing
    Dim nCount As Integer = 0
    Do While (nCount < 10)
      On Error Resume Next
      oRhinoScript = oRhino.GetScriptObject
      If (Err.Number <> 0) Then
        Err.Clear()
        System.Threading.Thread.Sleep(500)
        nCount = nCount + 1
      Else
        Exit Do
      End If
    Loop

    If (oRhinoScript Is Nothing) Then
      Console.WriteLine("BatchRender Error: cannot get RhinoScript4.RhinoScript")
      oRhino = Nothing
      Exit Sub
    End If

    Console.WriteLine("Processing " & strPath & "...")
    Dim strBitmap As String = Replace(strPath, ".3dm", ".png", 1, -1, 1)
    oRhinoScript.DocumentModified(False)
    oRhinoScript.Command("_-Open " & Chr(34) & strPath & Chr(34))
    oRhinoScript.Command("_4View")
    oRhinoScript.Command("_4View")
    oRhinoScript.Command("_Zoom _All _Extents")
    oRhinoScript.Command("_Zoom _All _Extents")
    oRhinoScript.Command("_SetActiveViewport Perspective")
    oRhinoScript.Command("_RenderedViewport")
    oRhinoScript.Sleep(1000)
    oRhinoScript.Command("_-ViewCaptureToFile " & Chr(34) & strBitmap & Chr(34) & " _Enter")
    oRhinoScript.DocumentModified(False)
    oRhinoScript.Command("_Exit")
    Console.WriteLine("  Done")

    Console.Write("Cleaning up...")
    oRhinoScript = Nothing
    oRhino = Nothing
    Console.WriteLine("done!")

  End Sub

End Module
