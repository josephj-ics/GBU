<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UpDownControl
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
        Me.btnUpArrow = New Kofax.Connector.Text.ArrowButton
        Me.btnDownArrow = New Kofax.Connector.Text.ArrowButton
        Me.SuspendLayout()
        '
        'btnUpArrow
        '
        Me.btnUpArrow.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnUpArrow.Location = New System.Drawing.Point(7, 2)
        Me.btnUpArrow.Name = "btnUpArrow"
        Me.btnUpArrow.ScrollButton = System.Windows.Forms.ScrollButton.Min
        Me.btnUpArrow.Size = New System.Drawing.Size(27, 24)
        Me.btnUpArrow.TabIndex = 0
        Me.btnUpArrow.UseVisualStyleBackColor = True
        '
        'btnDownArrow
        '
        Me.btnDownArrow.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnDownArrow.Location = New System.Drawing.Point(7, 32)
        Me.btnDownArrow.Name = "btnDownArrow"
        Me.btnDownArrow.ScrollButton = System.Windows.Forms.ScrollButton.Down
        Me.btnDownArrow.Size = New System.Drawing.Size(27, 24)
        Me.btnDownArrow.TabIndex = 1
        Me.btnDownArrow.UseVisualStyleBackColor = True
        '
        'UpDownControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnDownArrow)
        Me.Controls.Add(Me.btnUpArrow)
        Me.Name = "UpDownControl"
        Me.Size = New System.Drawing.Size(38, 59)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnUpArrow As Kofax.Connector.Text.ArrowButton
    Friend WithEvents btnDownArrow As Kofax.Connector.Text.ArrowButton

End Class
