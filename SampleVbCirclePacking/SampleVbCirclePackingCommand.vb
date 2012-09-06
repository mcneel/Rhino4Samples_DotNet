Imports RMA.Rhino
Imports RMA.OpenNURBS
Imports RMA.Rhino.RhUtil

'''<summary>
''' A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
''' DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
'''</summary>
Public Class SampleVbCirclePackingCommand
  Inherits RMA.Rhino.MRhinoCommand

  Protected Shared intCount As Int32 = 100
  Protected Shared maxIterations As Int32 = 10000
  Protected Shared minRadius As Double = 0.1
  Protected Shared maxRadius As Double = 1.0
  Protected Shared bPackAlgorithm As PackCircles.PackingAlgorithm = PackCircles.PackingAlgorithm.FastPack

  '''<summary>
  ''' Rhino tracks commands by their unique ID. Every command must have a unique id.
  ''' The Guid created by the project wizard is unique. You can create more Guids using
  ''' the "Create Guid" tool in the Tools menu.
  '''</summary>
  '''<returns>The id for this command</returns>
  Public Overrides Function CommandUUID() As System.Guid
    Return New Guid("{a9c09cb3-dc19-401f-87da-57cc1a896c82}")
  End Function

  '''<returns>The command name as it appears on the Rhino command line</returns>
  Public Overrides Function EnglishCommandName() As String
    Return "SampleVbCirclePacking"
  End Function

  '''<summary> This gets called when when the user runs this command.</summary>
  Public Overrides Function RunCommand(ByVal context As RMA.Rhino.IRhinoCommandContext) As RMA.Rhino.IRhinoCommand.result
    Dim ptBase As On3dPoint

    Do
      Dim nGetOptions As New MRhinoGetPoint
      nGetOptions.SetCommandPrompt("Center of fitting solution")
      nGetOptions.AddCommandOption(New MRhinoCommandOptionName("Count"), intCount)
      nGetOptions.AddCommandOption(New MRhinoCommandOptionName("MinRadius"), minRadius)
      nGetOptions.AddCommandOption(New MRhinoCommandOptionName("MaxRadius"), maxRadius)
      nGetOptions.AddCommandOption(New MRhinoCommandOptionName("IterationLimit"), maxIterations)

      Select Case bPackAlgorithm
        Case PackCircles.PackingAlgorithm.FastPack
          nGetOptions.AddCommandOption(New MRhinoCommandOptionName("Packing"), New MRhinoCommandOptionValue("Fast"))
        Case PackCircles.PackingAlgorithm.DoublePack
          nGetOptions.AddCommandOption(New MRhinoCommandOptionName("Packing"), New MRhinoCommandOptionValue("Double"))
        Case PackCircles.PackingAlgorithm.RandomPack
          nGetOptions.AddCommandOption(New MRhinoCommandOptionName("Packing"), New MRhinoCommandOptionValue("Random"))
        Case PackCircles.PackingAlgorithm.SimplePack
          nGetOptions.AddCommandOption(New MRhinoCommandOptionName("Packing"), New MRhinoCommandOptionValue("Simple"))
      End Select

      nGetOptions.AcceptNumber(True)

      Select Case nGetOptions.GetPoint
        Case IRhinoGet.result.point
          ptBase = New On3dPoint(nGetOptions.Point)
          Exit Do

        Case IRhinoGet.result.number
          Dim newCount As Int32 = Convert.ToInt32(nGetOptions.Number)
          If newCount < 10 Then
            RhUtil.RhinoApp.Print("A minimum of 2 circles if required" & vbCrLf)
          Else
            intCount = newCount
          End If

        Case IRhinoGet.result.option
          Select Case nGetOptions.Option.m_english_option_name
            Case "Count"
              RhUtil.RhinoGetInteger("Number of circles?", True, intCount, 2)

            Case "MinRadius"
              RhUtil.RhinoGetNumber("Minimum circle radius", True, False, minRadius, 0.001)

            Case "MaxRadius"
              RhUtil.RhinoGetNumber("Maximum circle radius", True, False, maxRadius, 0.001)

            Case "Packing"
              Dim nGetPack As New MRhinoGetOption
              nGetPack.SetCommandPrompt("Packing algorithm")

              Select Case bPackAlgorithm
                Case PackCircles.PackingAlgorithm.FastPack
                  nGetPack.SetDefaultString("Fast")
                Case PackCircles.PackingAlgorithm.DoublePack
                  nGetPack.SetDefaultString("Double")
                Case PackCircles.PackingAlgorithm.RandomPack
                  nGetPack.SetDefaultString("Random")
                Case PackCircles.PackingAlgorithm.SimplePack
                  nGetPack.SetDefaultString("Simple")
              End Select

              nGetPack.AddCommandOption(New MRhinoCommandOptionName("Fast"))
              nGetPack.AddCommandOption(New MRhinoCommandOptionName("Double"))
              nGetPack.AddCommandOption(New MRhinoCommandOptionName("Random"))
              nGetPack.AddCommandOption(New MRhinoCommandOptionName("Simple"))
              nGetPack.AddCommandOption(New MRhinoCommandOptionName("Help"))

AskTheQuestion:
              Select Case nGetPack.GetOption
                Case IRhinoGet.result.option
                  Select Case nGetPack.Option.m_english_option_name
                    Case "Fast"
                      bPackAlgorithm = PackCircles.PackingAlgorithm.FastPack
                    Case "Double"
                      bPackAlgorithm = PackCircles.PackingAlgorithm.DoublePack
                    Case "Random"
                      bPackAlgorithm = PackCircles.PackingAlgorithm.RandomPack
                    Case "Simple"
                      bPackAlgorithm = PackCircles.PackingAlgorithm.SimplePack
                    Case "Help"

                      Dim sHelp As String = ""
                      sHelp &= "Fast: fast packing prevents collisions by moving" & vbCrLf & _
                               "one circle away from all its intersectors." & vbCrLf & _
                               "After every collision iteration, all circles" & vbCrLf & _
                               "are moved towards the centre of the packing" & vbCrLf & _
                               "to reduce the amount of wasted space. Collision" & vbCrLf & _
                               "detection proceeds from the center outwards." & vbCrLf & vbCrLf

                      sHelp &= "Double: similar to FastPacking, except that both" & vbCrLf & _
                               "circles are moved in case of a collision." & vbCrLf & vbCrLf

                      sHelp &= "Random: similar to FastPacking, except that" & vbCrLf & _
                               "collision detection is randomized rather than" & vbCrLf & _
                               "sorted." & vbCrLf & vbCrLf

                      sHelp &= "Simple: similar to FastPacking, but without a" & vbCrLf & _
                               "contraction pass after every collision iteration."

                      MsgBox(sHelp, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, "Packing algorithm description")

                      GoTo AskTheQuestion
                  End Select
                Case Else
              End Select

            Case "IterationLimit"
              RhUtil.RhinoGetInteger("Maximum number of allowed iterations", True, maxIterations, 100)

          End Select

        Case Else
          Return IRhinoCommand.result.cancel
      End Select
    Loop

    Dim allCircles As New PackCircles(ptBase, intCount, minRadius, maxRadius)
    Dim fitConduit As New PackConduit(allCircles)

    Dim sDamping As Double = 0.1

    For i As Int32 = 1 To maxIterations
      RMA.Rhino.RhUtil.RhinoApp.SetCommandPrompt(String.Format("Performing circle packing iteration {0}...  (Press Shift+Ctrl to abort)", i))

      Dim iKeys As Windows.Forms.Keys = Windows.Forms.Control.ModifierKeys
      If iKeys = (Windows.Forms.Keys.Control Or Windows.Forms.Keys.Shift) Then
        RhUtil.RhinoApp.Print(String.Format("Circle fitting process aborted at iteration {0}...", i) & vbCrLf)
        Exit For
      End If

      If Not allCircles.Pack(bPackAlgorithm, sDamping) Then
        RhUtil.RhinoApp.Print(String.Format("Circle fitting process completed at iteration {0}...", i) & vbCrLf)
        Exit For
      End If

      sDamping *= 0.98
      RhUtil.RhinoApp.ActiveDoc.Regen()
      RMA.Rhino.RhUtil.RhinoApp.Wait(1)
    Next

    fitConduit.Disable()
    fitConduit.Dispose()
    fitConduit = Nothing

    allCircles.Add()

    RhUtil.RhinoApp.ActiveDoc.Regen()
    Return IRhinoCommand.result.success
  End Function
End Class
