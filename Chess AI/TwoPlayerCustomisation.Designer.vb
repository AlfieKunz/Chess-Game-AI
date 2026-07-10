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
        BackBtn = New Button()
        FENTextBox = New TextBox()
        StartBtn = New Button()
        Panel1 = New Panel()
        PosBtn1 = New RadioButton()
        PosBtn2 = New RadioButton()
        Panel1.SuspendLayout()
        SuspendLayout()
        ' 
        ' BackBtn
        ' 
        BackBtn.Font = New Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        BackBtn.Location = New Point(12, 176)
        BackBtn.Name = "BackBtn"
        BackBtn.Size = New Size(23, 23)
        BackBtn.TabIndex = 17
        BackBtn.Text = "↺"
        BackBtn.UseVisualStyleBackColor = True
        ' 
        ' FENTextBox
        ' 
        FENTextBox.Enabled = False
        FENTextBox.Location = New Point(15, 70)
        FENTextBox.Multiline = True
        FENTextBox.Name = "FENTextBox"
        FENTextBox.Size = New Size(250, 35)
        FENTextBox.TabIndex = 16
        FENTextBox.Text = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
        ' 
        ' StartBtn
        ' 
        StartBtn.Font = New Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        StartBtn.Location = New Point(98, 138)
        StartBtn.Name = "StartBtn"
        StartBtn.Size = New Size(90, 40)
        StartBtn.TabIndex = 13
        StartBtn.Text = "Start!"
        StartBtn.UseVisualStyleBackColor = True
        ' 
        ' Panel1
        ' 
        Panel1.Controls.Add(PosBtn1)
        Panel1.Controls.Add(PosBtn2)
        Panel1.Location = New Point(7, 7)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(200, 57)
        Panel1.TabIndex = 18
        ' 
        ' PosBtn1
        ' 
        PosBtn1.AutoSize = True
        PosBtn1.Checked = True
        PosBtn1.Location = New Point(3, 3)
        PosBtn1.Name = "PosBtn1"
        PosBtn1.Size = New Size(159, 17)
        PosBtn1.TabIndex = 1
        PosBtn1.TabStop = True
        PosBtn1.Text = "Use Normal Starting Position"
        PosBtn1.UseVisualStyleBackColor = True
        ' 
        ' PosBtn2
        ' 
        PosBtn2.AutoSize = True
        PosBtn2.Location = New Point(3, 26)
        PosBtn2.Name = "PosBtn2"
        PosBtn2.Size = New Size(161, 17)
        PosBtn2.TabIndex = 2
        PosBtn2.Text = "Use Custom Starting Position"
        PosBtn2.UseVisualStyleBackColor = True
        ' 
        ' TwoPlayerCustomisation
        ' 
        AutoScaleDimensions = New SizeF(6F, 13F)
        AutoScaleMode = AutoScaleMode.Font
        AutoSizeMode = AutoSizeMode.GrowAndShrink
        BackColor = Color.LightGray
        ClientSize = New Size(284, 211)
        Controls.Add(Panel1)
        Controls.Add(BackBtn)
        Controls.Add(FENTextBox)
        Controls.Add(StartBtn)
        Font = New Font("Microsoft Sans Serif", 8.25F)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "TwoPlayerCustomisation"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Customise Game!"
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()

    End Sub
    Friend WithEvents BackBtn As System.Windows.Forms.Button
    Friend WithEvents FENTextBox As System.Windows.Forms.TextBox
    Friend WithEvents StartBtn As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents PosBtn1 As System.Windows.Forms.RadioButton
    Friend WithEvents PosBtn2 As System.Windows.Forms.RadioButton
End Class
