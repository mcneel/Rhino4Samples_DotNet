<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SampleVbObjectListUserControl
  Inherits System.Windows.Forms.UserControl

  'UserControl overrides dispose to clean up the component list.
  <System.Diagnostics.DebuggerNonUserCode()> _
  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    Try
      If disposing AndAlso components IsNot Nothing Then
        components.Dispose()
      End If
    Finally
      MyBase.Dispose(disposing)
    End Try
  End Sub

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()> _
  Private Sub InitializeComponent()
    Me.components = New System.ComponentModel.Container
    Me.listBox = New System.Windows.Forms.ListBox
    Me.selectTimer = New System.Windows.Forms.Timer(Me.components)
    Me.deselectTimer = New System.Windows.Forms.Timer(Me.components)
    Me.SuspendLayout()
    '
    'listBox
    '
    Me.listBox.Dock = System.Windows.Forms.DockStyle.Fill
    Me.listBox.FormattingEnabled = True
    Me.listBox.HorizontalScrollbar = True
    Me.listBox.Location = New System.Drawing.Point(0, 0)
    Me.listBox.Name = "listBox"
    Me.listBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
    Me.listBox.Size = New System.Drawing.Size(150, 212)
    Me.listBox.Sorted = True
    Me.listBox.TabIndex = 0
    '
    'selectTimer
    '
    '
    'deselectTimer
    '
    '
    'SampleVbObjectListUserControl
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.Controls.Add(Me.listBox)
    Me.Name = "SampleVbObjectListUserControl"
    Me.Size = New System.Drawing.Size(150, 219)
    Me.ResumeLayout(False)

  End Sub
  Friend WithEvents listBox As System.Windows.Forms.ListBox
  Friend WithEvents selectTimer As System.Windows.Forms.Timer
  Friend WithEvents deselectTimer As System.Windows.Forms.Timer

End Class
