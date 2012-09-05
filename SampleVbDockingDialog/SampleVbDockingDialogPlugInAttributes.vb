'''<summary>
''' The following class is required for all Rhino.NET plug-ins.
''' These are used to display plug-in information in the plug-in manager.
''' Any string will work for these attributes, so if you don't have a fax
''' number it is OK to enter something like "none"
'''</summary>
Public Class SampleVbDockingDialogPlugInAttributes
  Inherits RMA.Rhino.MRhinoPlugInAttributes

  Public Overrides Function Address() As String
    Return "3670 Woodland Park Avenue North" + vbCrLf + "Seattle" + vbCrLf + "WA" + vbCrLf + "98115"
  End Function

  Public Overrides Function Country() As String
    Return "United States"
  End Function

  Public Overrides Function Email() As String
    Return "devsupport@mcneel.com"
  End Function

  Public Overrides Function Fax() As String
    Return "206-545-7321"
  End Function

  Public Overrides Function Organization() As String
    Return "Robert McNeel & Associates"
  End Function

  Public Overrides Function Phone() As String
    Return "206-545-6877"
  End Function

  Public Overrides Function UpdateURL() As String
    Return "https://github.com/mcneel"
  End Function

  Public Overrides Function Website() As String
    Return "http://www.rhino3d.com"
  End Function
End Class