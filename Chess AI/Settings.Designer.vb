<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Settings
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Settings))
        Checkerboard = New PictureBox()
        AnimationTester = New Button()
        SpeedSetter = New TrackBar()
        WP3 = New PictureBox()
        WP2 = New PictureBox()
        WP1 = New PictureBox()
        WN1 = New PictureBox()
        WB1 = New PictureBox()
        WR1 = New PictureBox()
        ColourLabel = New Label()
        Label3 = New Label()
        Label4 = New Label()
        ResetBtn = New Button()
        BackBtn = New Button()
        AnimationInfo = New Label()
        PictureBox3 = New PictureBox()
        PictureBox4 = New PictureBox()
        Def = New RadioButton()
        ColourSelectorL = New PictureBox()
        Blu = New RadioButton()
        Bl2 = New RadioButton()
        Gld = New RadioButton()
        Grn = New RadioButton()
        Red = New RadioButton()
        Ppl = New RadioButton()
        Mon = New RadioButton()
        ColourPanel = New Panel()
        ColourSelectorR = New PictureBox()
        SoundBtn = New CheckBox()
        AnimationBtn = New CheckBox()
        BoardBtn = New CheckBox()
        PieceBtn = New CheckBox()
        InvisBtn = New CheckBox()
        AnimationPieceInfo = New Label()
        HammadBtn = New CheckBox()
        FixedSearchBtn = New CheckBox()
        FixedSearchBox = New TextBox()
        TouchMoveBtn = New CheckBox()
        OpeningBookBtn = New CheckBox()
        CType(Checkerboard, ComponentModel.ISupportInitialize).BeginInit()
        CType(SpeedSetter, ComponentModel.ISupportInitialize).BeginInit()
        CType(WP3, ComponentModel.ISupportInitialize).BeginInit()
        CType(WP2, ComponentModel.ISupportInitialize).BeginInit()
        CType(WP1, ComponentModel.ISupportInitialize).BeginInit()
        CType(WN1, ComponentModel.ISupportInitialize).BeginInit()
        CType(WB1, ComponentModel.ISupportInitialize).BeginInit()
        CType(WR1, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureBox3, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureBox4, ComponentModel.ISupportInitialize).BeginInit()
        CType(ColourSelectorL, ComponentModel.ISupportInitialize).BeginInit()
        ColourPanel.SuspendLayout()
        CType(ColourSelectorR, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Checkerboard
        ' 
        Checkerboard.BackColor = Color.Transparent
        Checkerboard.Location = New Point(343, 58)
        Checkerboard.Name = "Checkerboard"
        Checkerboard.Size = New Size(225, 225)
        Checkerboard.TabIndex = 2
        Checkerboard.TabStop = False
        ' 
        ' AnimationTester
        ' 
        AnimationTester.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AnimationTester.Location = New Point(398, 350)
        AnimationTester.Name = "AnimationTester"
        AnimationTester.Size = New Size(115, 42)
        AnimationTester.TabIndex = 4
        AnimationTester.Text = "Test Animation:"
        AnimationTester.UseVisualStyleBackColor = True
        ' 
        ' SpeedSetter
        ' 
        SpeedSetter.LargeChange = 1
        SpeedSetter.Location = New Point(343, 290)
        SpeedSetter.Maximum = 4
        SpeedSetter.Name = "SpeedSetter"
        SpeedSetter.Size = New Size(224, 45)
        SpeedSetter.TabIndex = 1
        ' 
        ' WP3
        ' 
        WP3.BackColor = Color.Transparent
        WP3.Image = CType(resources.GetObject("WP3.Image"), Image)
        WP3.InitialImage = Nothing
        WP3.Location = New Point(492, 53)
        WP3.Name = "WP3"
        WP3.Size = New Size(75, 75)
        WP3.TabIndex = 5
        WP3.TabStop = False
        ' 
        ' WP2
        ' 
        WP2.BackColor = Color.Transparent
        WP2.Image = CType(resources.GetObject("WP2.Image"), Image)
        WP2.InitialImage = Nothing
        WP2.Location = New Point(418, 53)
        WP2.Name = "WP2"
        WP2.Size = New Size(75, 75)
        WP2.TabIndex = 6
        WP2.TabStop = False
        ' 
        ' WP1
        ' 
        WP1.BackColor = Color.Transparent
        WP1.Image = CType(resources.GetObject("WP1.Image"), Image)
        WP1.InitialImage = Nothing
        WP1.Location = New Point(343, 53)
        WP1.Name = "WP1"
        WP1.Size = New Size(75, 75)
        WP1.TabIndex = 7
        WP1.TabStop = False
        ' 
        ' WN1
        ' 
        WN1.BackColor = Color.Transparent
        WN1.Image = CType(resources.GetObject("WN1.Image"), Image)
        WN1.InitialImage = Nothing
        WN1.Location = New Point(418, 128)
        WN1.Name = "WN1"
        WN1.Size = New Size(75, 75)
        WN1.TabIndex = 8
        WN1.TabStop = False
        ' 
        ' WB1
        ' 
        WB1.BackColor = Color.Transparent
        WB1.Image = CType(resources.GetObject("WB1.Image"), Image)
        WB1.InitialImage = Nothing
        WB1.Location = New Point(492, 128)
        WB1.Name = "WB1"
        WB1.Size = New Size(75, 75)
        WB1.TabIndex = 9
        WB1.TabStop = False
        ' 
        ' WR1
        ' 
        WR1.BackColor = Color.Transparent
        WR1.Image = CType(resources.GetObject("WR1.Image"), Image)
        WR1.InitialImage = Nothing
        WR1.Location = New Point(343, 127)
        WR1.Name = "WR1"
        WR1.Size = New Size(75, 75)
        WR1.TabIndex = 10
        WR1.TabStop = False
        ' 
        ' ColourLabel
        ' 
        ColourLabel.AutoSize = True
        ColourLabel.Font = New Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold Or FontStyle.Underline, GraphicsUnit.Point, CByte(0))
        ColourLabel.Location = New Point(32, 9)
        ColourLabel.Name = "ColourLabel"
        ColourLabel.Size = New Size(248, 25)
        ColourLabel.TabIndex = 16
        ColourLabel.Text = "Board Colour Scheme:"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold Or FontStyle.Underline, GraphicsUnit.Point, CByte(0))
        Label3.Location = New Point(331, 9)
        Label3.Name = "Label3"
        Label3.Size = New Size(263, 25)
        Label3.TabIndex = 16
        Label3.Text = "Piece Animation Speed:"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Font = New Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold Or FontStyle.Underline, GraphicsUnit.Point, CByte(0))
        Label4.Location = New Point(652, 9)
        Label4.Name = "Label4"
        Label4.Size = New Size(195, 25)
        Label4.TabIndex = 17
        Label4.Text = "General Settings:"
        ' 
        ' ResetBtn
        ' 
        ResetBtn.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        ResetBtn.Location = New Point(696, 353)
        ResetBtn.Name = "ResetBtn"
        ResetBtn.Size = New Size(114, 36)
        ResetBtn.TabIndex = 11
        ResetBtn.Text = "Reset Program"
        ResetBtn.UseVisualStyleBackColor = True
        ' 
        ' BackBtn
        ' 
        BackBtn.Font = New Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        BackBtn.Location = New Point(849, 376)
        BackBtn.Name = "BackBtn"
        BackBtn.Size = New Size(23, 23)
        BackBtn.TabIndex = 18
        BackBtn.Text = "↺"
        BackBtn.UseVisualStyleBackColor = True
        ' 
        ' AnimationInfo
        ' 
        AnimationInfo.AutoSize = True
        AnimationInfo.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AnimationInfo.Location = New Point(343, 322)
        AnimationInfo.Name = "AnimationInfo"
        AnimationInfo.Size = New Size(230, 16)
        AnimationInfo.TabIndex = 19
        AnimationInfo.Text = "Off       VFast       Fast     Medium    Slow"
        ' 
        ' PictureBox3
        ' 
        PictureBox3.BackColor = Color.FromArgb(CByte(50), CByte(50), CByte(50))
        PictureBox3.Location = New Point(300, -4)
        PictureBox3.Name = "PictureBox3"
        PictureBox3.Size = New Size(10, 420)
        PictureBox3.TabIndex = 23
        PictureBox3.TabStop = False
        ' 
        ' PictureBox4
        ' 
        PictureBox4.BackColor = Color.FromArgb(CByte(50), CByte(50), CByte(50))
        PictureBox4.Location = New Point(604, -4)
        PictureBox4.Name = "PictureBox4"
        PictureBox4.Size = New Size(10, 420)
        PictureBox4.TabIndex = 24
        PictureBox4.TabStop = False
        ' 
        ' Def
        ' 
        Def.AutoSize = True
        Def.CheckAlign = ContentAlignment.TopLeft
        Def.Checked = True
        Def.FlatStyle = FlatStyle.Popup
        Def.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Def.Location = New Point(4, 4)
        Def.Name = "Def"
        Def.Size = New Size(63, 19)
        Def.TabIndex = 15
        Def.TabStop = True
        Def.Text = "Default"
        Def.UseVisualStyleBackColor = True
        ' 
        ' ColourSelectorL
        ' 
        ColourSelectorL.BackColor = Color.Transparent
        ColourSelectorL.BackgroundImage = CType(resources.GetObject("ColourSelectorL.BackgroundImage"), Image)
        ColourSelectorL.InitialImage = Nothing
        ColourSelectorL.Location = New Point(0, 70)
        ColourSelectorL.Name = "ColourSelectorL"
        ColourSelectorL.Size = New Size(110, 70)
        ColourSelectorL.TabIndex = 25
        ColourSelectorL.TabStop = False
        ' 
        ' Blu
        ' 
        Blu.AutoSize = True
        Blu.CheckAlign = ContentAlignment.TopLeft
        Blu.FlatStyle = FlatStyle.Popup
        Blu.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Blu.Location = New Point(4, 29)
        Blu.Name = "Blu"
        Blu.Size = New Size(67, 19)
        Blu.TabIndex = 15
        Blu.TabStop = True
        Blu.Text = "Blue (L)"
        Blu.UseVisualStyleBackColor = True
        ' 
        ' Bl2
        ' 
        Bl2.AutoSize = True
        Bl2.CheckAlign = ContentAlignment.TopLeft
        Bl2.FlatStyle = FlatStyle.Popup
        Bl2.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Bl2.Location = New Point(4, 54)
        Bl2.Name = "Bl2"
        Bl2.Size = New Size(69, 19)
        Bl2.TabIndex = 15
        Bl2.TabStop = True
        Bl2.Text = "Blue (D)"
        Bl2.UseVisualStyleBackColor = True
        ' 
        ' Gld
        ' 
        Gld.AutoSize = True
        Gld.CheckAlign = ContentAlignment.TopLeft
        Gld.FlatStyle = FlatStyle.Popup
        Gld.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Gld.Location = New Point(4, 79)
        Gld.Name = "Gld"
        Gld.Size = New Size(50, 19)
        Gld.TabIndex = 15
        Gld.TabStop = True
        Gld.Text = "Gold"
        Gld.UseVisualStyleBackColor = True
        ' 
        ' Grn
        ' 
        Grn.AutoSize = True
        Grn.CheckAlign = ContentAlignment.TopLeft
        Grn.FlatStyle = FlatStyle.Popup
        Grn.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Grn.Location = New Point(4, 104)
        Grn.Name = "Grn"
        Grn.Size = New Size(58, 19)
        Grn.TabIndex = 15
        Grn.TabStop = True
        Grn.Text = "Green"
        Grn.UseVisualStyleBackColor = True
        ' 
        ' Red
        ' 
        Red.AutoSize = True
        Red.CheckAlign = ContentAlignment.TopLeft
        Red.FlatStyle = FlatStyle.Popup
        Red.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Red.Location = New Point(4, 129)
        Red.Name = "Red"
        Red.Size = New Size(47, 19)
        Red.TabIndex = 15
        Red.TabStop = True
        Red.Text = "Red"
        Red.UseVisualStyleBackColor = True
        ' 
        ' Ppl
        ' 
        Ppl.AutoSize = True
        Ppl.CheckAlign = ContentAlignment.TopLeft
        Ppl.FlatStyle = FlatStyle.Popup
        Ppl.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Ppl.Location = New Point(4, 154)
        Ppl.Name = "Ppl"
        Ppl.Size = New Size(60, 19)
        Ppl.TabIndex = 15
        Ppl.TabStop = True
        Ppl.Text = "Purple"
        Ppl.UseVisualStyleBackColor = True
        ' 
        ' Mon
        ' 
        Mon.AutoSize = True
        Mon.CheckAlign = ContentAlignment.TopLeft
        Mon.FlatStyle = FlatStyle.Popup
        Mon.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Mon.Location = New Point(4, 179)
        Mon.Name = "Mon"
        Mon.Size = New Size(49, 19)
        Mon.TabIndex = 15
        Mon.Text = "Gray"
        Mon.UseVisualStyleBackColor = True
        ' 
        ' ColourPanel
        ' 
        ColourPanel.BackColor = Color.Transparent
        ColourPanel.Controls.Add(Def)
        ColourPanel.Controls.Add(Blu)
        ColourPanel.Controls.Add(Gld)
        ColourPanel.Controls.Add(Grn)
        ColourPanel.Controls.Add(Red)
        ColourPanel.Controls.Add(Ppl)
        ColourPanel.Controls.Add(Mon)
        ColourPanel.Controls.Add(Bl2)
        ColourPanel.Location = New Point(113, 90)
        ColourPanel.Name = "ColourPanel"
        ColourPanel.Size = New Size(76, 202)
        ColourPanel.TabIndex = 26
        ' 
        ' ColourSelectorR
        ' 
        ColourSelectorR.BackColor = Color.Transparent
        ColourSelectorR.BackgroundImage = CType(resources.GetObject("ColourSelectorR.BackgroundImage"), Image)
        ColourSelectorR.InitialImage = Nothing
        ColourSelectorR.Location = New Point(190, 70)
        ColourSelectorR.Name = "ColourSelectorR"
        ColourSelectorR.Size = New Size(110, 70)
        ColourSelectorR.TabIndex = 27
        ColourSelectorR.TabStop = False
        ' 
        ' SoundBtn
        ' 
        SoundBtn.AutoSize = True
        SoundBtn.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        SoundBtn.Location = New Point(637, 48)
        SoundBtn.Name = "SoundBtn"
        SoundBtn.Size = New Size(154, 20)
        SoundBtn.TabIndex = 28
        SoundBtn.Text = "Enable Sound Effects"
        SoundBtn.UseVisualStyleBackColor = True
        ' 
        ' AnimationBtn
        ' 
        AnimationBtn.AutoSize = True
        AnimationBtn.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AnimationBtn.Location = New Point(637, 82)
        AnimationBtn.Name = "AnimationBtn"
        AnimationBtn.Size = New Size(169, 20)
        AnimationBtn.TabIndex = 28
        AnimationBtn.Text = "Play Opening Animation"
        AnimationBtn.UseVisualStyleBackColor = True
        ' 
        ' BoardBtn
        ' 
        BoardBtn.AutoSize = True
        BoardBtn.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        BoardBtn.Location = New Point(637, 150)
        BoardBtn.Name = "BoardBtn"
        BoardBtn.Size = New Size(174, 20)
        BoardBtn.TabIndex = 28
        BoardBtn.Text = "Display Board Highlights"
        BoardBtn.UseVisualStyleBackColor = True
        ' 
        ' PieceBtn
        ' 
        PieceBtn.AutoSize = True
        PieceBtn.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        PieceBtn.Location = New Point(637, 184)
        PieceBtn.Name = "PieceBtn"
        PieceBtn.Size = New Size(172, 20)
        PieceBtn.TabIndex = 28
        PieceBtn.Text = "Display Piece Highlights"
        PieceBtn.UseVisualStyleBackColor = True
        ' 
        ' InvisBtn
        ' 
        InvisBtn.AutoSize = True
        InvisBtn.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        InvisBtn.Location = New Point(637, 252)
        InvisBtn.Name = "InvisBtn"
        InvisBtn.Size = New Size(120, 20)
        InvisBtn.TabIndex = 28
        InvisBtn.Text = "Invisible Pieces"
        InvisBtn.UseVisualStyleBackColor = True
        ' 
        ' AnimationPieceInfo
        ' 
        AnimationPieceInfo.AutoSize = True
        AnimationPieceInfo.Font = New Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AnimationPieceInfo.Location = New Point(369, 38)
        AnimationPieceInfo.Name = "AnimationPieceInfo"
        AnimationPieceInfo.Size = New Size(170, 13)
        AnimationPieceInfo.TabIndex = 16
        AnimationPieceInfo.Text = "Click a Piece to Test its Animation!"
        ' 
        ' HammadBtn
        ' 
        HammadBtn.AutoSize = True
        HammadBtn.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        HammadBtn.Location = New Point(637, 320)
        HammadBtn.Name = "HammadBtn"
        HammadBtn.Size = New Size(209, 20)
        HammadBtn.TabIndex = 28
        HammadBtn.Text = "'Hammad AI' Mode  (Unstable)"
        HammadBtn.UseVisualStyleBackColor = True
        ' 
        ' FixedSearchBtn
        ' 
        FixedSearchBtn.AutoSize = True
        FixedSearchBtn.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        FixedSearchBtn.Location = New Point(637, 286)
        FixedSearchBtn.Name = "FixedSearchBtn"
        FixedSearchBtn.Size = New Size(195, 20)
        FixedSearchBtn.TabIndex = 28
        FixedSearchBtn.Text = "Fixed AI Search:           Depth:"
        FixedSearchBtn.UseVisualStyleBackColor = True
        ' 
        ' FixedSearchBox
        ' 
        FixedSearchBox.Enabled = False
        FixedSearchBox.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        FixedSearchBox.Location = New Point(829, 285)
        FixedSearchBox.Name = "FixedSearchBox"
        FixedSearchBox.RightToLeft = RightToLeft.No
        FixedSearchBox.Size = New Size(35, 22)
        FixedSearchBox.TabIndex = 29
        FixedSearchBox.Text = "0"
        FixedSearchBox.TextAlign = HorizontalAlignment.Center
        ' 
        ' TouchMoveBtn
        ' 
        TouchMoveBtn.AutoSize = True
        TouchMoveBtn.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        TouchMoveBtn.Location = New Point(637, 218)
        TouchMoveBtn.Name = "TouchMoveBtn"
        TouchMoveBtn.Size = New Size(132, 20)
        TouchMoveBtn.TabIndex = 28
        TouchMoveBtn.Text = "Touch Move Rule"
        TouchMoveBtn.UseVisualStyleBackColor = True
        ' 
        ' OpeningBookBtn
        ' 
        OpeningBookBtn.AutoSize = True
        OpeningBookBtn.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        OpeningBookBtn.Location = New Point(637, 116)
        OpeningBookBtn.Name = "OpeningBookBtn"
        OpeningBookBtn.Size = New Size(216, 20)
        OpeningBookBtn.TabIndex = 28
        OpeningBookBtn.Text = "Use Small Book  (Faster Loads)"
        OpeningBookBtn.UseVisualStyleBackColor = True
        ' 
        ' Settings
        ' 
        AutoScaleDimensions = New SizeF(6F, 13F)
        AutoScaleMode = AutoScaleMode.Font
        AutoSizeMode = AutoSizeMode.GrowAndShrink
        BackColor = Color.FromArgb(CByte(155), CByte(155), CByte(155))
        ClientSize = New Size(884, 411)
        Controls.Add(FixedSearchBox)
        Controls.Add(FixedSearchBtn)
        Controls.Add(HammadBtn)
        Controls.Add(TouchMoveBtn)
        Controls.Add(InvisBtn)
        Controls.Add(PieceBtn)
        Controls.Add(BoardBtn)
        Controls.Add(OpeningBookBtn)
        Controls.Add(AnimationBtn)
        Controls.Add(SoundBtn)
        Controls.Add(ColourSelectorR)
        Controls.Add(ColourPanel)
        Controls.Add(PictureBox4)
        Controls.Add(PictureBox3)
        Controls.Add(AnimationInfo)
        Controls.Add(BackBtn)
        Controls.Add(Label4)
        Controls.Add(AnimationPieceInfo)
        Controls.Add(Label3)
        Controls.Add(ColourLabel)
        Controls.Add(ResetBtn)
        Controls.Add(WR1)
        Controls.Add(WP3)
        Controls.Add(WP2)
        Controls.Add(WP1)
        Controls.Add(WN1)
        Controls.Add(WB1)
        Controls.Add(SpeedSetter)
        Controls.Add(AnimationTester)
        Controls.Add(Checkerboard)
        Controls.Add(ColourSelectorL)
        Font = New Font("Microsoft Sans Serif", 8.25F)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "Settings"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Settings"
        CType(Checkerboard, ComponentModel.ISupportInitialize).EndInit()
        CType(SpeedSetter, ComponentModel.ISupportInitialize).EndInit()
        CType(WP3, ComponentModel.ISupportInitialize).EndInit()
        CType(WP2, ComponentModel.ISupportInitialize).EndInit()
        CType(WP1, ComponentModel.ISupportInitialize).EndInit()
        CType(WN1, ComponentModel.ISupportInitialize).EndInit()
        CType(WB1, ComponentModel.ISupportInitialize).EndInit()
        CType(WR1, ComponentModel.ISupportInitialize).EndInit()
        CType(PictureBox3, ComponentModel.ISupportInitialize).EndInit()
        CType(PictureBox4, ComponentModel.ISupportInitialize).EndInit()
        CType(ColourSelectorL, ComponentModel.ISupportInitialize).EndInit()
        ColourPanel.ResumeLayout(False)
        ColourPanel.PerformLayout()
        CType(ColourSelectorR, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()

    End Sub
    Friend WithEvents Checkerboard As PictureBox
    Friend WithEvents AnimationTester As Button
    Friend WithEvents SpeedSetter As TrackBar
    Friend WithEvents WP3 As System.Windows.Forms.PictureBox
    Friend WithEvents WP2 As System.Windows.Forms.PictureBox
    Friend WithEvents WP1 As System.Windows.Forms.PictureBox
    Friend WithEvents WN1 As System.Windows.Forms.PictureBox
    Friend WithEvents WB1 As System.Windows.Forms.PictureBox
    Friend WithEvents WR1 As System.Windows.Forms.PictureBox
    Friend WithEvents ColourLabel As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents ResetBtn As System.Windows.Forms.Button
    Friend WithEvents BackBtn As System.Windows.Forms.Button
    Friend WithEvents AnimationInfo As System.Windows.Forms.Label
    Friend WithEvents PictureBox3 As PictureBox
    Friend WithEvents PictureBox4 As PictureBox
    Friend WithEvents Def As RadioButton
    Friend WithEvents ColourSelectorL As PictureBox
    Friend WithEvents Blu As RadioButton
    Friend WithEvents Bl2 As RadioButton
    Friend WithEvents Gld As RadioButton
    Friend WithEvents Grn As RadioButton
    Friend WithEvents Red As RadioButton
    Friend WithEvents Ppl As RadioButton
    Friend WithEvents Mon As RadioButton
    Friend WithEvents ColourPanel As Panel
    Friend WithEvents ColourSelectorR As PictureBox
    Friend WithEvents SoundBtn As CheckBox
    Friend WithEvents AnimationBtn As CheckBox
    Friend WithEvents BoardBtn As CheckBox
    Friend WithEvents PieceBtn As CheckBox
    Friend WithEvents InvisBtn As CheckBox
    Friend WithEvents AnimationPieceInfo As Label
    Friend WithEvents HammadBtn As CheckBox
    Friend WithEvents FixedSearchBtn As System.Windows.Forms.CheckBox
    Friend WithEvents FixedSearchBox As System.Windows.Forms.TextBox
    Friend WithEvents TouchMoveBtn As CheckBox
    Friend WithEvents OpeningBookBtn As CheckBox
End Class
