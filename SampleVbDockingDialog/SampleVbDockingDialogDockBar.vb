Imports System
Imports System.Windows.Forms
Imports RMA.Rhino
Imports RMA.UI

Public Class SampleVbDockingDialogDockBar
  Inherits MRhinoUiDockBar

  ''' <summary>
  ''' Public constructor
  ''' </summary>
  Public Sub New()
    MyBase.New()
  End Sub

  ''' <summary>
  ''' Public constructor
  ''' </summary>
  Public Sub New(ByVal id As System.Guid, ByVal name As String, ByVal control As System.Windows.Forms.Control)
    MyBase.New(id, name, control)
  End Sub

  ''' <summary>
  ''' Dockbar Id
  ''' </summary>
  Public Shared Function ID() As System.Guid
    Return New System.Guid("{5DA846D3-1D0C-40E4-A0DE-719681188F07}")
  End Function

End Class
