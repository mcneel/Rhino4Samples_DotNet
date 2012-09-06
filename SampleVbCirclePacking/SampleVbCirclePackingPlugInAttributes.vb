'''<summary>
''' The following class is required for all Rhino.NET plug-ins.
''' These are used to display plug-in information in the plug-in manager.
''' Any string will work for these attributes, so if you don't have a fax
''' number it is OK to enter something like "none"
'''</summary>
Public Class SampleVbCirclePackingPlugInAttributes
  Inherits RMA.Rhino.MRhinoPlugInAttributes

  Public Overrides Function Address() As String
    Return "123 Programmer Lane"+vbCrLf+"City"+vbCrLf+"State"+vbCrLf+"12345-6789"
  End Function

  Public Overrides Function Country() As String
    Return "USA"
  End Function

  Public Overrides Function Email() As String
    Return "product.support@mycompany.com"
  End Function

  Public Overrides Function Fax() As String
    Return "987.654.3210"
  End Function

  Public Overrides Function Organization() As String
    Return "Company Name"
  End Function

  Public Overrides Function Phone() As String
    Return "123.456.7890"
  End Function

  Public Overrides Function UpdateURL() As String
    Return "http://updates.mycompany.com"
  End Function

  Public Overrides Function Website() As String
    Return "http://www.mycompany.com"
  End Function
End Class