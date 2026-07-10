<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class OnePlayerCustomisation
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(OnePlayerCustomisation))
        StartBtn = New Button()
        PosBtn1 = New RadioButton()
        PosBtn2 = New RadioButton()
        FENTextBox = New TextBox()
        BackBtn = New Button()
        DifficultySlider = New TrackBar()
        AIDiffLabel1 = New Label()
        AIDiffLabel2 = New Label()
        Panel1 = New Panel()
        Panel2 = New Panel()
        White = New RadioButton()
        RNDSide = New RadioButton()
        Black = New RadioButton()
        InfoBtn = New Button()
        UseBook = New CheckBox()
        PieceHeatMapBox = New CheckBox()
        QuiescenceBox = New CheckBox()
        UserTimeBar = New TrackBar()
        UserTimeBox = New TextBox()
        AISearchOnUsersTurnBox = New CheckBox()
        AIBox = New CheckBox()
        AdvancedSearchTimeBox = New CheckBox()
        CType(DifficultySlider, ComponentModel.ISupportInitialize).BeginInit()
        Panel1.SuspendLayout()
        Panel2.SuspendLayout()
        CType(UserTimeBar, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' StartBtn
        ' 
        StartBtn.Cursor = Cursors.Hand
        StartBtn.Font = New Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        StartBtn.Location = New Point(98, 258)
        StartBtn.Name = "StartBtn"
        StartBtn.Size = New Size(90, 40)
        StartBtn.TabIndex = 0
        StartBtn.Text = "Start!"
        StartBtn.UseVisualStyleBackColor = True
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
        ' FENTextBox
        ' 
        FENTextBox.Enabled = False
        FENTextBox.Location = New Point(15, 70)
        FENTextBox.Multiline = True
        FENTextBox.Name = "FENTextBox"
        FENTextBox.Size = New Size(250, 35)
        FENTextBox.TabIndex = 3
        FENTextBox.Text = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
        ' 
        ' BackBtn
        ' 
        BackBtn.Font = New Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        BackBtn.Location = New Point(12, 296)
        BackBtn.Name = "BackBtn"
        BackBtn.Size = New Size(23, 23)
        BackBtn.TabIndex = 12
        BackBtn.Text = "↺"
        BackBtn.UseVisualStyleBackColor = True
        ' 
        ' DifficultySlider
        ' 
        DifficultySlider.LargeChange = 1
        DifficultySlider.Location = New Point(15, 151)
        DifficultySlider.Maximum = 7
        DifficultySlider.Minimum = 1
        DifficultySlider.Name = "DifficultySlider"
        DifficultySlider.Size = New Size(250, 45)
        DifficultySlider.TabIndex = 13
        DifficultySlider.TickStyle = TickStyle.TopLeft
        DifficultySlider.Value = 2
        ' 
        ' AIDiffLabel1
        ' 
        AIDiffLabel1.AutoSize = True
        AIDiffLabel1.Font = New Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AIDiffLabel1.Location = New Point(42, 126)
        AIDiffLabel1.Name = "AIDiffLabel1"
        AIDiffLabel1.Size = New Size(83, 18)
        AIDiffLabel1.TabIndex = 14
        AIDiffLabel1.Text = "AI Difficulty:"
        AIDiffLabel1.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' AIDiffLabel2
        ' 
        AIDiffLabel2.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        AIDiffLabel2.ForeColor = Color.Green
        AIDiffLabel2.Location = New Point(149, 127)
        AIDiffLabel2.Name = "AIDiffLabel2"
        AIDiffLabel2.Size = New Size(85, 20)
        AIDiffLabel2.TabIndex = 15
        AIDiffLabel2.Text = "Beginner"
        AIDiffLabel2.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' Panel1
        ' 
        Panel1.Controls.Add(PosBtn1)
        Panel1.Controls.Add(PosBtn2)
        Panel1.Location = New Point(7, 7)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(200, 57)
        Panel1.TabIndex = 17
        ' 
        ' Panel2
        ' 
        Panel2.Controls.Add(White)
        Panel2.Controls.Add(RNDSide)
        Panel2.Controls.Add(Black)
        Panel2.Location = New Point(3, 199)
        Panel2.Name = "Panel2"
        Panel2.Size = New Size(279, 26)
        Panel2.TabIndex = 18
        ' 
        ' White
        ' 
        White.AutoSize = True
        White.Location = New Point(3, 3)
        White.Name = "White"
        White.Size = New Size(90, 17)
        White.TabIndex = 16
        White.Text = "Play as White"
        White.UseVisualStyleBackColor = True
        ' 
        ' RNDSide
        ' 
        RNDSide.AutoSize = True
        RNDSide.Checked = True
        RNDSide.Location = New Point(193, 3)
        RNDSide.Name = "RNDSide"
        RNDSide.Size = New Size(86, 17)
        RNDSide.TabIndex = 16
        RNDSide.TabStop = True
        RNDSide.Text = "Play as RND"
        RNDSide.UseVisualStyleBackColor = True
        ' 
        ' Black
        ' 
        Black.AutoSize = True
        Black.Location = New Point(98, 3)
        Black.Name = "Black"
        Black.Size = New Size(89, 17)
        Black.TabIndex = 16
        Black.Text = "Play as Black"
        Black.UseVisualStyleBackColor = True
        ' 
        ' InfoBtn
        ' 
        InfoBtn.Font = New Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        InfoBtn.Location = New Point(254, 296)
        InfoBtn.Name = "InfoBtn"
        InfoBtn.Size = New Size(18, 23)
        InfoBtn.TabIndex = 19
        InfoBtn.Text = "?"
        InfoBtn.TextAlign = ContentAlignment.MiddleLeft
        InfoBtn.UseVisualStyleBackColor = True
        ' 
        ' UseBook
        ' 
        UseBook.AutoSize = True
        UseBook.Checked = True
        UseBook.CheckState = CheckState.Checked
        UseBook.Font = New Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        UseBook.Location = New Point(85, 228)
        UseBook.Name = "UseBook"
        UseBook.Size = New Size(122, 17)
        UseBook.TabIndex = 27
        UseBook.Text = "Use Opening Book?"
        UseBook.TextAlign = ContentAlignment.MiddleCenter
        UseBook.UseVisualStyleBackColor = True
        ' 
        ' PieceHeatMapBox
        ' 
        PieceHeatMapBox.AutoSize = True
        PieceHeatMapBox.Checked = True
        PieceHeatMapBox.CheckState = CheckState.Checked
        PieceHeatMapBox.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        PieceHeatMapBox.Location = New Point(69, 292)
        PieceHeatMapBox.Name = "PieceHeatMapBox"
        PieceHeatMapBox.Size = New Size(152, 19)
        PieceHeatMapBox.TabIndex = 30
        PieceHeatMapBox.Text = "Use Piece Heat Maps?"
        PieceHeatMapBox.TextAlign = ContentAlignment.MiddleCenter
        PieceHeatMapBox.UseVisualStyleBackColor = True
        PieceHeatMapBox.Visible = False
        ' 
        ' QuiescenceBox
        ' 
        QuiescenceBox.AutoSize = True
        QuiescenceBox.Checked = True
        QuiescenceBox.CheckState = CheckState.Checked
        QuiescenceBox.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        QuiescenceBox.Location = New Point(81, 267)
        QuiescenceBox.Name = "QuiescenceBox"
        QuiescenceBox.Size = New Size(123, 19)
        QuiescenceBox.TabIndex = 31
        QuiescenceBox.Text = "Use Quiescence?"
        QuiescenceBox.TextAlign = ContentAlignment.MiddleCenter
        QuiescenceBox.UseVisualStyleBackColor = True
        QuiescenceBox.Visible = False
        ' 
        ' UserTimeBar
        ' 
        UserTimeBar.LargeChange = 10
        UserTimeBar.Location = New Point(60, 226)
        UserTimeBar.Maximum = 60
        UserTimeBar.Name = "UserTimeBar"
        UserTimeBar.Size = New Size(164, 45)
        UserTimeBar.TabIndex = 29
        UserTimeBar.Value = 20
        UserTimeBar.Visible = False
        ' 
        ' UserTimeBox
        ' 
        UserTimeBox.BackColor = Color.WhiteSmoke
        UserTimeBox.Location = New Point(60, 202)
        UserTimeBox.Name = "UserTimeBox"
        UserTimeBox.ReadOnly = True
        UserTimeBox.Size = New Size(164, 20)
        UserTimeBox.TabIndex = 28
        UserTimeBox.Text = "Time For Search: 5 Seconds"
        UserTimeBox.TextAlign = HorizontalAlignment.Center
        UserTimeBox.Visible = False
        ' 
        ' AISearchOnUsersTurnBox
        ' 
        AISearchOnUsersTurnBox.AutoSize = True
        AISearchOnUsersTurnBox.Checked = True
        AISearchOnUsersTurnBox.CheckState = CheckState.Checked
        AISearchOnUsersTurnBox.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AISearchOnUsersTurnBox.Location = New Point(55, 317)
        AISearchOnUsersTurnBox.Name = "AISearchOnUsersTurnBox"
        AISearchOnUsersTurnBox.Size = New Size(180, 19)
        AISearchOnUsersTurnBox.TabIndex = 30
        AISearchOnUsersTurnBox.Text = "AI Searches in Background?"
        AISearchOnUsersTurnBox.TextAlign = ContentAlignment.MiddleCenter
        AISearchOnUsersTurnBox.UseVisualStyleBackColor = True
        AISearchOnUsersTurnBox.Visible = False
        ' 
        ' AIBox
        ' 
        AIBox.AutoSize = True
        AIBox.Checked = True
        AIBox.CheckState = CheckState.Checked
        AIBox.Location = New Point(217, 24)
        AIBox.Name = "AIBox"
        AIBox.Size = New Size(58, 17)
        AIBox.TabIndex = 32
        AIBox.Text = "Use AI"
        AIBox.UseVisualStyleBackColor = True
        AIBox.Visible = False
        ' 
        ' AdvancedSearchTimeBox
        ' 
        AdvancedSearchTimeBox.AutoSize = True
        AdvancedSearchTimeBox.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AdvancedSearchTimeBox.Location = New Point(53, 342)
        AdvancedSearchTimeBox.Name = "AdvancedSearchTimeBox"
        AdvancedSearchTimeBox.Size = New Size(184, 19)
        AdvancedSearchTimeBox.TabIndex = 30
        AdvancedSearchTimeBox.Text = "Use Advanced Search Time?"
        AdvancedSearchTimeBox.TextAlign = ContentAlignment.MiddleCenter
        AdvancedSearchTimeBox.UseVisualStyleBackColor = True
        AdvancedSearchTimeBox.Visible = False
        ' 
        ' OnePlayerCustomisation
        ' 
        AutoScaleDimensions = New SizeF(6F, 13F)
        AutoScaleMode = AutoScaleMode.Font
        AutoSizeMode = AutoSizeMode.GrowAndShrink
        BackColor = Color.LightGray
        ClientSize = New Size(284, 361)
        Controls.Add(AIBox)
        Controls.Add(AdvancedSearchTimeBox)
        Controls.Add(AISearchOnUsersTurnBox)
        Controls.Add(PieceHeatMapBox)
        Controls.Add(QuiescenceBox)
        Controls.Add(UserTimeBar)
        Controls.Add(UserTimeBox)
        Controls.Add(UseBook)
        Controls.Add(InfoBtn)
        Controls.Add(Panel2)
        Controls.Add(Panel1)
        Controls.Add(AIDiffLabel2)
        Controls.Add(AIDiffLabel1)
        Controls.Add(DifficultySlider)
        Controls.Add(BackBtn)
        Controls.Add(FENTextBox)
        Controls.Add(StartBtn)
        Font = New Font("Microsoft Sans Serif", 8.25F)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "OnePlayerCustomisation"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Customise Game!"
        CType(DifficultySlider, ComponentModel.ISupportInitialize).EndInit()
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        Panel2.ResumeLayout(False)
        Panel2.PerformLayout()
        CType(UserTimeBar, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()

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
    Friend WithEvents UseBook As CheckBox
    Friend WithEvents PieceHeatMapBox As CheckBox
    Friend WithEvents QuiescenceBox As CheckBox
    Friend WithEvents UserTimeBar As TrackBar
    Friend WithEvents UserTimeBox As TextBox
    Friend WithEvents RNDSide As RadioButton
    Friend WithEvents AISearchOnUsersTurnBox As CheckBox
    Friend WithEvents AIBox As CheckBox
    Friend WithEvents AdvancedSearchTimeBox As CheckBox
End Class
