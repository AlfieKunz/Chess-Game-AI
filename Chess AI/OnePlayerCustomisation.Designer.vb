<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OnePlayerCustomisation
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(OnePlayerCustomisation))
        Me.StartBtn = New System.Windows.Forms.Button()
        Me.PosBtn1 = New System.Windows.Forms.RadioButton()
        Me.PosBtn2 = New System.Windows.Forms.RadioButton()
        Me.FENTextBox = New System.Windows.Forms.TextBox()
        Me.BackBtn = New System.Windows.Forms.Button()
        Me.DifficultySlider = New System.Windows.Forms.TrackBar()
        Me.AIDiffLabel1 = New System.Windows.Forms.Label()
        Me.AIDiffLabel2 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.White = New System.Windows.Forms.RadioButton()
        Me.Black = New System.Windows.Forms.RadioButton()
        Me.InfoBtn = New System.Windows.Forms.Button()
        Me.UseBook = New System.Windows.Forms.CheckBox()
        CType(Me.DifficultySlider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'StartBtn
        '
        Me.StartBtn.Cursor = System.Windows.Forms.Cursors.Hand
        Me.StartBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StartBtn.Location = New System.Drawing.Point(98, 258)
        Me.StartBtn.Name = "StartBtn"
        Me.StartBtn.Size = New System.Drawing.Size(90, 40)
        Me.StartBtn.TabIndex = 0
        Me.StartBtn.Text = "Start!"
        Me.StartBtn.UseVisualStyleBackColor = True
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
        'FENTextBox
        '
        Me.FENTextBox.Enabled = False
        Me.FENTextBox.Location = New System.Drawing.Point(15, 70)
        Me.FENTextBox.Multiline = True
        Me.FENTextBox.Name = "FENTextBox"
        Me.FENTextBox.Size = New System.Drawing.Size(250, 35)
        Me.FENTextBox.TabIndex = 3
        Me.FENTextBox.Text = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
        '
        'BackBtn
        '
        Me.BackBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BackBtn.Location = New System.Drawing.Point(12, 296)
        Me.BackBtn.Name = "BackBtn"
        Me.BackBtn.Size = New System.Drawing.Size(23, 23)
        Me.BackBtn.TabIndex = 12
        Me.BackBtn.Text = "↺"
        Me.BackBtn.UseVisualStyleBackColor = True
        '
        'DifficultySlider
        '
        Me.DifficultySlider.LargeChange = 1
        Me.DifficultySlider.Location = New System.Drawing.Point(15, 151)
        Me.DifficultySlider.Maximum = 6
        Me.DifficultySlider.Minimum = 1
        Me.DifficultySlider.Name = "DifficultySlider"
        Me.DifficultySlider.Size = New System.Drawing.Size(250, 45)
        Me.DifficultySlider.TabIndex = 13
        Me.DifficultySlider.TickStyle = System.Windows.Forms.TickStyle.TopLeft
        Me.DifficultySlider.Value = 1
        '
        'AIDiffLabel1
        '
        Me.AIDiffLabel1.AutoSize = True
        Me.AIDiffLabel1.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AIDiffLabel1.Location = New System.Drawing.Point(42, 126)
        Me.AIDiffLabel1.Name = "AIDiffLabel1"
        Me.AIDiffLabel1.Size = New System.Drawing.Size(83, 18)
        Me.AIDiffLabel1.TabIndex = 14
        Me.AIDiffLabel1.Text = "AI Difficulty:"
        Me.AIDiffLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'AIDiffLabel2
        '
        Me.AIDiffLabel2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AIDiffLabel2.ForeColor = System.Drawing.Color.Green
        Me.AIDiffLabel2.Location = New System.Drawing.Point(149, 127)
        Me.AIDiffLabel2.Name = "AIDiffLabel2"
        Me.AIDiffLabel2.Size = New System.Drawing.Size(85, 20)
        Me.AIDiffLabel2.TabIndex = 15
        Me.AIDiffLabel2.Text = "Beginner"
        Me.AIDiffLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.PosBtn1)
        Me.Panel1.Controls.Add(Me.PosBtn2)
        Me.Panel1.Location = New System.Drawing.Point(7, 7)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(200, 57)
        Me.Panel1.TabIndex = 17
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.White)
        Me.Panel2.Controls.Add(Me.Black)
        Me.Panel2.Location = New System.Drawing.Point(31, 199)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(214, 26)
        Me.Panel2.TabIndex = 18
        '
        'White
        '
        Me.White.AutoSize = True
        Me.White.Checked = True
        Me.White.Location = New System.Drawing.Point(3, 3)
        Me.White.Name = "White"
        Me.White.Size = New System.Drawing.Size(90, 17)
        Me.White.TabIndex = 16
        Me.White.TabStop = True
        Me.White.Text = "Play as White"
        Me.White.UseVisualStyleBackColor = True
        '
        'Black
        '
        Me.Black.AutoSize = True
        Me.Black.Location = New System.Drawing.Point(125, 3)
        Me.Black.Name = "Black"
        Me.Black.Size = New System.Drawing.Size(89, 17)
        Me.Black.TabIndex = 16
        Me.Black.Text = "Play as Black"
        Me.Black.UseVisualStyleBackColor = True
        '
        'InfoBtn
        '
        Me.InfoBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.InfoBtn.Location = New System.Drawing.Point(254, 296)
        Me.InfoBtn.Name = "InfoBtn"
        Me.InfoBtn.Size = New System.Drawing.Size(18, 23)
        Me.InfoBtn.TabIndex = 19
        Me.InfoBtn.Text = "?"
        Me.InfoBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.InfoBtn.UseVisualStyleBackColor = True
        '
        'UseBook
        '
        Me.UseBook.AutoSize = True
        Me.UseBook.Checked = True
        Me.UseBook.CheckState = System.Windows.Forms.CheckState.Checked
        Me.UseBook.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UseBook.Location = New System.Drawing.Point(85, 228)
        Me.UseBook.Name = "UseBook"
        Me.UseBook.Size = New System.Drawing.Size(122, 17)
        Me.UseBook.TabIndex = 27
        Me.UseBook.Text = "Use Opening Book?"
        Me.UseBook.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.UseBook.UseVisualStyleBackColor = True
        '
        'OnePlayerCustomisation
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.LightGray
        Me.ClientSize = New System.Drawing.Size(284, 331)
        Me.Controls.Add(Me.UseBook)
        Me.Controls.Add(Me.InfoBtn)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.AIDiffLabel2)
        Me.Controls.Add(Me.AIDiffLabel1)
        Me.Controls.Add(Me.DifficultySlider)
        Me.Controls.Add(Me.BackBtn)
        Me.Controls.Add(Me.FENTextBox)
        Me.Controls.Add(Me.StartBtn)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "OnePlayerCustomisation"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Customise Game!"
        CType(Me.DifficultySlider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StartBtn As System.Windows.Forms.Button
    Friend WithEvents PosBtn1 As System.Windows.Forms.RadioButton
    Friend WithEvents PosBtn2 As System.Windows.Forms.RadioButton
    Friend WithEvents FENTextBox As System.Windows.Forms.TextBox
    Friend WithEvents BackBtn As System.Windows.Forms.Button
    Friend WithEvents DifficultySlider As System.Windows.Forms.TrackBar
    Friend WithEvents AIDiffLabel1 As System.Windows.Forms.Label
    Friend WithEvents AIDiffLabel2 As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents White As System.Windows.Forms.RadioButton
    Friend WithEvents Black As System.Windows.Forms.RadioButton
    Friend WithEvents InfoBtn As Button
    Friend WithEvents UseBook As System.Windows.Forms.CheckBox
End Class
