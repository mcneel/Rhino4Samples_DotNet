<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SampleVbDockingDialogUserControl
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
    Me.btnHello = New System.Windows.Forms.Button
    Me.SuspendLayout()
    '
    'btnHello
    '
    Me.btnHello.Location = New System.Drawing.Point(18, 17)
    Me.btnHello.Name = "btnHello"
    Me.btnHello.Size = New System.Drawing.Size(75, 23)
    Me.btnHello.TabIndex = 0
    Me.btnHello.Text = "Hello"
    Me.btnHello.UseVisualStyleBackColor = True
    '
    'SampleVbDockingDialogUserControl
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.Controls.Add(Me.btnHello)
    Me.Name = "SampleVbDockingDialogUserControl"
    Me.Size = New System.Drawing.Size(195, 279)
    Me.ResumeLayout(False)

  End Sub
  Friend WithEvents btnHello As System.Windows.Forms.Button

End Class
