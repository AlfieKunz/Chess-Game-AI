<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TwoPlayerCustomisation
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TwoPlayerCustomisation))
        Me.BackBtn = New System.Windows.Forms.Button()
        Me.FENTextBox = New System.Windows.Forms.TextBox()
        Me.StartBtn = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.PosBtn1 = New System.Windows.Forms.RadioButton()
        Me.PosBtn2 = New System.Windows.Forms.RadioButton()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'BackBtn
        '
        Me.BackBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BackBtn.Location = New System.Drawing.Point(12, 176)
        Me.BackBtn.Name = "BackBtn"
        Me.BackBtn.Size = New System.Drawing.Size(23, 23)
        Me.BackBtn.TabIndex = 17
        Me.BackBtn.Text = "↺"
        Me.BackBtn.UseVisualStyleBackColor = True
        '
        'FENTextBox
        '
        Me.FENTextBox.Enabled = False
        Me.FENTextBox.Location = New System.Drawing.Point(15, 70)
        Me.FENTextBox.Multiline = True
        Me.FENTextBox.Name = "FENTextBox"
        Me.FENTextBox.Size = New System.Drawing.Size(250, 35)
        Me.FENTextBox.TabIndex = 16
        Me.FENTextBox.Text = GlobalConstants.StartingFENPosition
        '
        'StartBtn
        '
        Me.StartBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StartBtn.Location = New System.Drawing.Point(98, 138)
        Me.StartBtn.Name = "StartBtn"
        Me.StartBtn.Size = New System.Drawing.Size(90, 40)
        Me.StartBtn.TabIndex = 13
        Me.StartBtn.Text = "Start!"
        Me.StartBtn.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.PosBtn1)
        Me.Panel1.Controls.Add(Me.PosBtn2)
        Me.Panel1.Location = New System.Drawing.Point(7, 7)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(200, 57)
        Me.Panel1.TabIndex = 18
        '
        'PosBtn1
        '
        Me.PosBtn1.AutoSize = True
        Me.PosBtn1.Checked = True
        Me.PosBtn1.Location = New System.Drawing.Point(3, 3)
        Me.PosBtn1.Name = "PosBtn1"
        Me.PosBtn1.Size = New System.Drawing.Size(159, 17)
        Me.PosBtn1.TabIndex = 1
        Me.PosBtn1.TabStop = True
        Me.PosBtn1.Text = "Use Normal Starting Position"
        Me.PosBtn1.UseVisualStyleBackColor = True
        '
        'PosBtn2
        '
        Me.PosBtn2.AutoSize = True
        Me.PosBtn2.Location = New System.Drawing.Point(3, 26)
        Me.PosBtn2.Name = "PosBtn2"
        Me.PosBtn2.Size = New System.Drawing.Size(161, 17)
        Me.PosBtn2.TabIndex = 2
        Me.PosBtn2.Text = "Use Custom Starting Position"
        Me.PosBtn2.UseVisualStyleBackColor = True
        '
        'TwoPlayerCustomisation
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.LightGray
        Me.ClientSize = New System.Drawing.Size(284, 211)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.BackBtn)
        Me.Controls.Add(Me.FENTextBox)
        Me.Controls.Add(Me.StartBtn)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "TwoPlayerCustomisation"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Customise Game!"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BackBtn As System.Windows.Forms.Button
    Friend WithEvents FENTextBox As System.Windows.Forms.TextBox
    Friend WithEvents StartBtn As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents PosBtn1 As System.Windows.Forms.RadioButton
    Friend WithEvents PosBtn2 As System.Windows.Forms.RadioButton
End Class
