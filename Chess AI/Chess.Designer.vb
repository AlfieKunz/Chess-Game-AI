<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Chess
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Chess))
        Me.InputTextBox = New System.Windows.Forms.TextBox()
        Me.InputButton = New System.Windows.Forms.Button()
        Me.AIMoveBtn = New System.Windows.Forms.Button()
        Me.Reset_Btn = New System.Windows.Forms.Button()
        Me.FENExport = New System.Windows.Forms.Button()
        Me.CheckLabel = New System.Windows.Forms.Label()
        Me.CurrentAIEval = New System.Windows.Forms.Label()
        Me.UndoMove = New System.Windows.Forms.Button()
        Me.Credits = New System.Windows.Forms.Label()
        Me.UndoFENChange = New System.Windows.Forms.Button()
        Me.ProgressBar = New System.Windows.Forms.ProgressBar()
        Me.ColourChanger = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.Def = New System.Windows.Forms.ToolStripMenuItem()
        Me.LBlu = New System.Windows.Forms.ToolStripMenuItem()
        Me.DBlu = New System.Windows.Forms.ToolStripMenuItem()
        Me.Gld = New System.Windows.Forms.ToolStripMenuItem()
        Me.Grn = New System.Windows.Forms.ToolStripMenuItem()
        Me.Ppl = New System.Windows.Forms.ToolStripMenuItem()
        Me.Red = New System.Windows.Forms.ToolStripMenuItem()
        Me.Mon = New System.Windows.Forms.ToolStripMenuItem()
        Me.SettingsBtn = New System.Windows.Forms.Button()
        Me.BP1 = New System.Windows.Forms.PictureBox()
        Me.BR1 = New System.Windows.Forms.PictureBox()
        Me.BN1 = New System.Windows.Forms.PictureBox()
        Me.BB1 = New System.Windows.Forms.PictureBox()
        Me.BQ1 = New System.Windows.Forms.PictureBox()
        Me.BK1 = New System.Windows.Forms.PictureBox()
        Me.WP1 = New System.Windows.Forms.PictureBox()
        Me.WR1 = New System.Windows.Forms.PictureBox()
        Me.WN1 = New System.Windows.Forms.PictureBox()
        Me.WB1 = New System.Windows.Forms.PictureBox()
        Me.WQ1 = New System.Windows.Forms.PictureBox()
        Me.WK1 = New System.Windows.Forms.PictureBox()
        Me.Checkerboard = New System.Windows.Forms.PictureBox()
        Me.FlipperButton = New System.Windows.Forms.Button()
        Me.AutoFlipper = New System.Windows.Forms.CheckBox()
        Me.ExitBtn = New System.Windows.Forms.Button()
        Me.UserTimeBox = New System.Windows.Forms.TextBox()
        Me.UserTimeBar = New System.Windows.Forms.TrackBar()
        Me.CurrentAIMove = New System.Windows.Forms.Label()
        Me.CurrentAIDepth = New System.Windows.Forms.Label()
        Me.QuiescenceBox = New System.Windows.Forms.CheckBox()
        Me.AITerminator = New System.Windows.Forms.Button()
        Me.AIEndlessMode = New System.Windows.Forms.CheckBox()
        Me.TrainingStart = New System.Windows.Forms.Button()
        Me.TimerLabel = New System.Windows.Forms.Label()
        Me.MoveDisplayer = New System.Windows.Forms.Label()
        Me.WLeaderBoardGrid = New System.Windows.Forms.DataGridView()
        Me.UserIndex = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Username = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.UserScore = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.UserDate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BLeaderBoardGrid = New System.Windows.Forms.DataGridView()
        Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TrainingScore = New System.Windows.Forms.Label()
        Me.TrainingTimer = New System.Windows.Forms.Timer(Me.components)
        Me.InfoBtn = New System.Windows.Forms.Button()
        Me.UseBook = New System.Windows.Forms.CheckBox()
        Me.PieceHeatMapBox = New System.Windows.Forms.CheckBox()
        Me.NextPuzzleBtn = New System.Windows.Forms.Button()
        Me.HintBtn = New System.Windows.Forms.Button()
        Me.GiveUpBtn = New System.Windows.Forms.Button()
        Me.AIPuzzleInfoLabel = New System.Windows.Forms.Label()
        Me.RatingLabel = New System.Windows.Forms.Label()
        Me.LostRatingLabel = New System.Windows.Forms.Label()
        Me.GainedRatingLabel = New System.Windows.Forms.Label()
        Me.RatingHeader = New System.Windows.Forms.Label()
        Me.PuzzleRatingLabel = New System.Windows.Forms.Label()
        Me.AutoAdvanceBox = New System.Windows.Forms.TextBox()
        Me.AutoAdvanceBar = New System.Windows.Forms.TrackBar()
        Me.AutoAdvanceOnComplete = New System.Windows.Forms.CheckBox()
        Me.AutoAdvanceLabel = New System.Windows.Forms.Label()
        Me.HumanModeBtn = New System.Windows.Forms.RadioButton()
        Me.AIModeBtn = New System.Windows.Forms.RadioButton()
        Me.ResetRatingsBtn = New System.Windows.Forms.Button()
        Me.PGNExport = New System.Windows.Forms.Button()
        Me.AutoOutputterPanel = New System.Windows.Forms.Panel()
        Me.PGNAutoOutputter = New System.Windows.Forms.RadioButton()
        Me.FENAutoOutputter = New System.Windows.Forms.RadioButton()
        Me.NodeTestBtn = New System.Windows.Forms.Button()
        Me.NodeTestStopBtn = New System.Windows.Forms.Button()
        Me.AutoResetter = New System.Windows.Forms.CheckBox()
        Me.BoardEditorBtn = New System.Windows.Forms.Button()
        Me.BoardEditCancelBtn = New System.Windows.Forms.Button()
        Me.BoardEditBlackMove = New System.Windows.Forms.RadioButton()
        Me.BoardEditWhiteMove = New System.Windows.Forms.RadioButton()
        Me.BoardEditPanel = New System.Windows.Forms.Panel()
        Me.BoardEditTipLabel = New System.Windows.Forms.Label()
        Me.BoardEditEnPassantLabel = New System.Windows.Forms.Label()
        Me.BoardEditBLabel = New System.Windows.Forms.Label()
        Me.BoardEditWLabel = New System.Windows.Forms.Label()
        Me.BoardEditBQSBox = New System.Windows.Forms.CheckBox()
        Me.BoardEditWQSBox = New System.Windows.Forms.CheckBox()
        Me.BoardEditBKSBox = New System.Windows.Forms.CheckBox()
        Me.BoardEditWKSBox = New System.Windows.Forms.CheckBox()
        Me.ColourChanger.SuspendLayout()
        CType(Me.BP1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BR1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BN1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BB1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BQ1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BK1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WP1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WR1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WN1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WB1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WQ1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WK1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Checkerboard, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.UserTimeBar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WLeaderBoardGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BLeaderBoardGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AutoAdvanceBar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.AutoOutputterPanel.SuspendLayout()
        Me.BoardEditPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'InputTextBox
        '
        Me.InputTextBox.Location = New System.Drawing.Point(9, 12)
        Me.InputTextBox.Multiline = True
        Me.InputTextBox.Name = "InputTextBox"
        Me.InputTextBox.Size = New System.Drawing.Size(282, 35)
        Me.InputTextBox.TabIndex = 2
        '
        'InputButton
        '
        Me.InputButton.Location = New System.Drawing.Point(66, 53)
        Me.InputButton.Name = "InputButton"
        Me.InputButton.Size = New System.Drawing.Size(171, 23)
        Me.InputButton.TabIndex = 3
        Me.InputButton.Text = "Click to Submit FEN / Move(s):"
        Me.InputButton.UseVisualStyleBackColor = True
        '
        'AIMoveBtn
        '
        Me.AIMoveBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AIMoveBtn.Location = New System.Drawing.Point(945, 85)
        Me.AIMoveBtn.Name = "AIMoveBtn"
        Me.AIMoveBtn.Size = New System.Drawing.Size(209, 57)
        Me.AIMoveBtn.TabIndex = 4
        Me.AIMoveBtn.Text = "Make Computer Move:"
        Me.AIMoveBtn.UseVisualStyleBackColor = True
        '
        'Reset_Btn
        '
        Me.Reset_Btn.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Reset_Btn.Location = New System.Drawing.Point(9, 85)
        Me.Reset_Btn.Name = "Reset_Btn"
        Me.Reset_Btn.Size = New System.Drawing.Size(133, 43)
        Me.Reset_Btn.TabIndex = 5
        Me.Reset_Btn.Text = "Reset Board"
        Me.Reset_Btn.UseVisualStyleBackColor = True
        '
        'FENExport
        '
        Me.FENExport.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FENExport.Location = New System.Drawing.Point(150, 85)
        Me.FENExport.Name = "FENExport"
        Me.FENExport.Size = New System.Drawing.Size(70, 43)
        Me.FENExport.TabIndex = 7
        Me.FENExport.Text = "Export FEN"
        Me.FENExport.UseVisualStyleBackColor = True
        '
        'CheckLabel
        '
        Me.CheckLabel.AutoSize = True
        Me.CheckLabel.BackColor = System.Drawing.Color.White
        Me.CheckLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CheckLabel.Location = New System.Drawing.Point(987, 12)
        Me.CheckLabel.Name = "CheckLabel"
        Me.CheckLabel.Size = New System.Drawing.Size(126, 25)
        Me.CheckLabel.TabIndex = 8
        Me.CheckLabel.Text = "Checkmate!"
        '
        'CurrentAIEval
        '
        Me.CurrentAIEval.AutoSize = True
        Me.CurrentAIEval.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CurrentAIEval.Location = New System.Drawing.Point(955, 270)
        Me.CurrentAIEval.Name = "CurrentAIEval"
        Me.CurrentAIEval.Size = New System.Drawing.Size(93, 16)
        Me.CurrentAIEval.TabIndex = 9
        Me.CurrentAIEval.Text = "Evaluation: -"
        '
        'UndoMove
        '
        Me.UndoMove.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UndoMove.Location = New System.Drawing.Point(9, 146)
        Me.UndoMove.Name = "UndoMove"
        Me.UndoMove.Size = New System.Drawing.Size(282, 43)
        Me.UndoMove.TabIndex = 7
        Me.UndoMove.Text = "Undo Last Action"
        Me.UndoMove.UseVisualStyleBackColor = True
        '
        'Credits
        '
        Me.Credits.AutoSize = True
        Me.Credits.BackColor = System.Drawing.Color.Transparent
        Me.Credits.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Credits.ForeColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.Credits.Location = New System.Drawing.Point(1035, 575)
        Me.Credits.Name = "Credits"
        Me.Credits.Size = New System.Drawing.Size(154, 16)
        Me.Credits.TabIndex = 10
        Me.Credits.Text = "Created by Alfie Kunz"
        '
        'UndoFENChange
        '
        Me.UndoFENChange.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UndoFENChange.Location = New System.Drawing.Point(245, 53)
        Me.UndoFENChange.Name = "UndoFENChange"
        Me.UndoFENChange.Size = New System.Drawing.Size(23, 23)
        Me.UndoFENChange.TabIndex = 11
        Me.UndoFENChange.Text = "↺"
        Me.UndoFENChange.UseVisualStyleBackColor = True
        '
        'ProgressBar
        '
        Me.ProgressBar.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.ProgressBar.Location = New System.Drawing.Point(945, 150)
        Me.ProgressBar.Name = "ProgressBar"
        Me.ProgressBar.Size = New System.Drawing.Size(209, 25)
        Me.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.ProgressBar.TabIndex = 13
        '
        'ColourChanger
        '
        Me.ColourChanger.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.ColourChanger.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.Def, Me.LBlu, Me.DBlu, Me.Gld, Me.Grn, Me.Ppl, Me.Red, Me.Mon})
        Me.ColourChanger.Name = "ContextMenuStrip1"
        Me.ColourChanger.Size = New System.Drawing.Size(136, 180)
        '
        'Def
        '
        Me.Def.Name = "Def"
        Me.Def.Size = New System.Drawing.Size(135, 22)
        Me.Def.Text = "Default"
        '
        'LBlu
        '
        Me.LBlu.Name = "LBlu"
        Me.LBlu.Size = New System.Drawing.Size(135, 22)
        Me.LBlu.Text = "Blue (Light)"
        '
        'DBlu
        '
        Me.DBlu.Name = "DBlu"
        Me.DBlu.Size = New System.Drawing.Size(135, 22)
        Me.DBlu.Text = "Blue (Dark)"
        '
        'Gld
        '
        Me.Gld.Name = "Gld"
        Me.Gld.Size = New System.Drawing.Size(135, 22)
        Me.Gld.Text = "Gold"
        '
        'Grn
        '
        Me.Grn.Name = "Grn"
        Me.Grn.Size = New System.Drawing.Size(135, 22)
        Me.Grn.Text = "Green"
        '
        'Ppl
        '
        Me.Ppl.Name = "Ppl"
        Me.Ppl.Size = New System.Drawing.Size(135, 22)
        Me.Ppl.Text = "Purple"
        '
        'Red
        '
        Me.Red.Name = "Red"
        Me.Red.Size = New System.Drawing.Size(135, 22)
        Me.Red.Text = "Red"
        '
        'Mon
        '
        Me.Mon.Name = "Mon"
        Me.Mon.Size = New System.Drawing.Size(135, 22)
        Me.Mon.Text = "Grey"
        '
        'SettingsBtn
        '
        Me.SettingsBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SettingsBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.SettingsBtn.Location = New System.Drawing.Point(95, 410)
        Me.SettingsBtn.Name = "SettingsBtn"
        Me.SettingsBtn.Size = New System.Drawing.Size(113, 40)
        Me.SettingsBtn.TabIndex = 15
        Me.SettingsBtn.Text = "Settings:"
        Me.SettingsBtn.UseVisualStyleBackColor = True
        '
        'BP1
        '
        Me.BP1.BackColor = System.Drawing.Color.Transparent
        Me.BP1.Image = CType(resources.GetObject("BP1.Image"), System.Drawing.Image)
        Me.BP1.InitialImage = Nothing
        Me.BP1.Location = New System.Drawing.Point(518, 258)
        Me.BP1.Name = "BP1"
        Me.BP1.Size = New System.Drawing.Size(75, 75)
        Me.BP1.TabIndex = 0
        Me.BP1.TabStop = False
        '
        'BR1
        '
        Me.BR1.BackColor = System.Drawing.Color.Transparent
        Me.BR1.Image = CType(resources.GetObject("BR1.Image"), System.Drawing.Image)
        Me.BR1.InitialImage = Nothing
        Me.BR1.Location = New System.Drawing.Point(437, 257)
        Me.BR1.Name = "BR1"
        Me.BR1.Size = New System.Drawing.Size(75, 75)
        Me.BR1.TabIndex = 0
        Me.BR1.TabStop = False
        '
        'BN1
        '
        Me.BN1.BackColor = System.Drawing.Color.Transparent
        Me.BN1.Image = CType(resources.GetObject("BN1.Image"), System.Drawing.Image)
        Me.BN1.InitialImage = Nothing
        Me.BN1.Location = New System.Drawing.Point(356, 258)
        Me.BN1.Name = "BN1"
        Me.BN1.Size = New System.Drawing.Size(75, 75)
        Me.BN1.TabIndex = 0
        Me.BN1.TabStop = False
        '
        'BB1
        '
        Me.BB1.BackColor = System.Drawing.Color.Transparent
        Me.BB1.Image = CType(resources.GetObject("BB1.Image"), System.Drawing.Image)
        Me.BB1.InitialImage = Nothing
        Me.BB1.Location = New System.Drawing.Point(518, 96)
        Me.BB1.Name = "BB1"
        Me.BB1.Size = New System.Drawing.Size(75, 75)
        Me.BB1.TabIndex = 0
        Me.BB1.TabStop = False
        '
        'BQ1
        '
        Me.BQ1.BackColor = System.Drawing.Color.Transparent
        Me.BQ1.Image = CType(resources.GetObject("BQ1.Image"), System.Drawing.Image)
        Me.BQ1.InitialImage = Nothing
        Me.BQ1.Location = New System.Drawing.Point(437, 96)
        Me.BQ1.Name = "BQ1"
        Me.BQ1.Size = New System.Drawing.Size(75, 75)
        Me.BQ1.TabIndex = 0
        Me.BQ1.TabStop = False
        '
        'BK1
        '
        Me.BK1.BackColor = System.Drawing.Color.Transparent
        Me.BK1.Image = CType(resources.GetObject("BK1.Image"), System.Drawing.Image)
        Me.BK1.InitialImage = Nothing
        Me.BK1.Location = New System.Drawing.Point(356, 96)
        Me.BK1.Name = "BK1"
        Me.BK1.Size = New System.Drawing.Size(75, 75)
        Me.BK1.TabIndex = 0
        Me.BK1.TabStop = False
        '
        'WP1
        '
        Me.WP1.BackColor = System.Drawing.Color.Transparent
        Me.WP1.Image = CType(resources.GetObject("WP1.Image"), System.Drawing.Image)
        Me.WP1.InitialImage = Nothing
        Me.WP1.Location = New System.Drawing.Point(518, 176)
        Me.WP1.Name = "WP1"
        Me.WP1.Size = New System.Drawing.Size(75, 75)
        Me.WP1.TabIndex = 0
        Me.WP1.TabStop = False
        '
        'WR1
        '
        Me.WR1.BackColor = System.Drawing.Color.Transparent
        Me.WR1.Image = CType(resources.GetObject("WR1.Image"), System.Drawing.Image)
        Me.WR1.InitialImage = Nothing
        Me.WR1.Location = New System.Drawing.Point(437, 177)
        Me.WR1.Name = "WR1"
        Me.WR1.Size = New System.Drawing.Size(75, 75)
        Me.WR1.TabIndex = 0
        Me.WR1.TabStop = False
        '
        'WN1
        '
        Me.WN1.BackColor = System.Drawing.Color.Transparent
        Me.WN1.Image = CType(resources.GetObject("WN1.Image"), System.Drawing.Image)
        Me.WN1.InitialImage = Nothing
        Me.WN1.Location = New System.Drawing.Point(356, 177)
        Me.WN1.Name = "WN1"
        Me.WN1.Size = New System.Drawing.Size(75, 75)
        Me.WN1.TabIndex = 0
        Me.WN1.TabStop = False
        '
        'WB1
        '
        Me.WB1.BackColor = System.Drawing.Color.Transparent
        Me.WB1.Image = CType(resources.GetObject("WB1.Image"), System.Drawing.Image)
        Me.WB1.InitialImage = Nothing
        Me.WB1.Location = New System.Drawing.Point(518, 15)
        Me.WB1.Name = "WB1"
        Me.WB1.Size = New System.Drawing.Size(75, 75)
        Me.WB1.TabIndex = 0
        Me.WB1.TabStop = False
        '
        'WQ1
        '
        Me.WQ1.BackColor = System.Drawing.Color.Transparent
        Me.WQ1.Image = CType(resources.GetObject("WQ1.Image"), System.Drawing.Image)
        Me.WQ1.InitialImage = Nothing
        Me.WQ1.Location = New System.Drawing.Point(437, 15)
        Me.WQ1.Name = "WQ1"
        Me.WQ1.Size = New System.Drawing.Size(75, 75)
        Me.WQ1.TabIndex = 0
        Me.WQ1.TabStop = False
        '
        'WK1
        '
        Me.WK1.BackColor = System.Drawing.Color.Transparent
        Me.WK1.Image = CType(resources.GetObject("WK1.Image"), System.Drawing.Image)
        Me.WK1.InitialImage = Nothing
        Me.WK1.Location = New System.Drawing.Point(356, 15)
        Me.WK1.Name = "WK1"
        Me.WK1.Size = New System.Drawing.Size(75, 75)
        Me.WK1.TabIndex = 0
        Me.WK1.TabStop = False
        '
        'Checkerboard
        '
        Me.Checkerboard.BackColor = System.Drawing.Color.Transparent
        Me.Checkerboard.Location = New System.Drawing.Point(300, 0)
        Me.Checkerboard.Name = "Checkerboard"
        Me.Checkerboard.Size = New System.Drawing.Size(600, 600)
        Me.Checkerboard.TabIndex = 1
        Me.Checkerboard.TabStop = False
        '
        'FlipperButton
        '
        Me.FlipperButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FlipperButton.Location = New System.Drawing.Point(34, 520)
        Me.FlipperButton.Name = "FlipperButton"
        Me.FlipperButton.Size = New System.Drawing.Size(232, 43)
        Me.FlipperButton.TabIndex = 18
        Me.FlipperButton.Text = "Flip Board"
        Me.FlipperButton.UseVisualStyleBackColor = True
        '
        'AutoFlipper
        '
        Me.AutoFlipper.AutoSize = True
        Me.AutoFlipper.Location = New System.Drawing.Point(39, 569)
        Me.AutoFlipper.Name = "AutoFlipper"
        Me.AutoFlipper.Size = New System.Drawing.Size(221, 17)
        Me.AutoFlipper.TabIndex = 19
        Me.AutoFlipper.Text = "Flip Board Automatically After Each Move"
        Me.AutoFlipper.UseVisualStyleBackColor = True
        '
        'ExitBtn
        '
        Me.ExitBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ExitBtn.Location = New System.Drawing.Point(1009, 515)
        Me.ExitBtn.Name = "ExitBtn"
        Me.ExitBtn.Size = New System.Drawing.Size(78, 40)
        Me.ExitBtn.TabIndex = 22
        Me.ExitBtn.Text = "Exit to Main Menu"
        Me.ExitBtn.UseVisualStyleBackColor = True
        '
        'UserTimeBox
        '
        Me.UserTimeBox.BackColor = System.Drawing.Color.WhiteSmoke
        Me.UserTimeBox.Cursor = System.Windows.Forms.Cursors.Hand
        Me.UserTimeBox.Location = New System.Drawing.Point(962, 320)
        Me.UserTimeBox.Name = "UserTimeBox"
        Me.UserTimeBox.ReadOnly = True
        Me.UserTimeBox.Size = New System.Drawing.Size(164, 20)
        Me.UserTimeBox.TabIndex = 23
        Me.UserTimeBox.Text = "Time For Search: 10 Seconds"
        Me.UserTimeBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'UserTimeBar
        '
        Me.UserTimeBar.LargeChange = 10
        Me.UserTimeBar.Location = New System.Drawing.Point(962, 344)
        Me.UserTimeBar.Maximum = 60
        Me.UserTimeBar.Name = "UserTimeBar"
        Me.UserTimeBar.Size = New System.Drawing.Size(164, 45)
        Me.UserTimeBar.TabIndex = 25
        Me.UserTimeBar.Value = 20
        '
        'CurrentAIMove
        '
        Me.CurrentAIMove.AutoSize = True
        Me.CurrentAIMove.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CurrentAIMove.Location = New System.Drawing.Point(955, 240)
        Me.CurrentAIMove.Name = "CurrentAIMove"
        Me.CurrentAIMove.Size = New System.Drawing.Size(111, 16)
        Me.CurrentAIMove.TabIndex = 9
        Me.CurrentAIMove.Text = "Current Move: -"
        '
        'CurrentAIDepth
        '
        Me.CurrentAIDepth.AutoSize = True
        Me.CurrentAIDepth.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CurrentAIDepth.Location = New System.Drawing.Point(955, 210)
        Me.CurrentAIDepth.Name = "CurrentAIDepth"
        Me.CurrentAIDepth.Size = New System.Drawing.Size(114, 16)
        Me.CurrentAIDepth.TabIndex = 9
        Me.CurrentAIDepth.Text = "Current Depth: -"
        '
        'QuiescenceBox
        '
        Me.QuiescenceBox.AutoSize = True
        Me.QuiescenceBox.Checked = True
        Me.QuiescenceBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.QuiescenceBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.QuiescenceBox.Location = New System.Drawing.Point(983, 385)
        Me.QuiescenceBox.Name = "QuiescenceBox"
        Me.QuiescenceBox.Size = New System.Drawing.Size(123, 19)
        Me.QuiescenceBox.TabIndex = 26
        Me.QuiescenceBox.Text = "Use Quiescence?"
        Me.QuiescenceBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.QuiescenceBox.UseVisualStyleBackColor = True
        '
        'AITerminator
        '
        Me.AITerminator.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AITerminator.Location = New System.Drawing.Point(945, 85)
        Me.AITerminator.Name = "AITerminator"
        Me.AITerminator.Size = New System.Drawing.Size(209, 57)
        Me.AITerminator.TabIndex = 27
        Me.AITerminator.Text = "Terminate Search Early:"
        Me.AITerminator.UseVisualStyleBackColor = True
        Me.AITerminator.Visible = False
        '
        'AIEndlessMode
        '
        Me.AIEndlessMode.AutoSize = True
        Me.AIEndlessMode.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AIEndlessMode.Location = New System.Drawing.Point(1009, 460)
        Me.AIEndlessMode.Name = "AIEndlessMode"
        Me.AIEndlessMode.Size = New System.Drawing.Size(70, 19)
        Me.AIEndlessMode.TabIndex = 26
        Me.AIEndlessMode.Text = "AI vs AI?"
        Me.AIEndlessMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.AIEndlessMode.UseVisualStyleBackColor = True
        '
        'TrainingStart
        '
        Me.TrainingStart.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TrainingStart.Location = New System.Drawing.Point(9, 12)
        Me.TrainingStart.Name = "TrainingStart"
        Me.TrainingStart.Size = New System.Drawing.Size(282, 43)
        Me.TrainingStart.TabIndex = 7
        Me.TrainingStart.Text = "Start!"
        Me.TrainingStart.UseVisualStyleBackColor = True
        Me.TrainingStart.Visible = False
        '
        'TimerLabel
        '
        Me.TimerLabel.AutoSize = True
        Me.TimerLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TimerLabel.Location = New System.Drawing.Point(31, 70)
        Me.TimerLabel.Name = "TimerLabel"
        Me.TimerLabel.Size = New System.Drawing.Size(235, 24)
        Me.TimerLabel.TabIndex = 9
        Me.TimerLabel.Text = "Time Left: 20.0 Seconds"
        Me.TimerLabel.Visible = False
        '
        'MoveDisplayer
        '
        Me.MoveDisplayer.Font = New System.Drawing.Font("Microsoft Sans Serif", 72.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MoveDisplayer.Location = New System.Drawing.Point(9, 203)
        Me.MoveDisplayer.Name = "MoveDisplayer"
        Me.MoveDisplayer.Size = New System.Drawing.Size(284, 152)
        Me.MoveDisplayer.TabIndex = 9
        Me.MoveDisplayer.Text = "h8"
        Me.MoveDisplayer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.MoveDisplayer.Visible = False
        '
        'WLeaderBoardGrid
        '
        Me.WLeaderBoardGrid.AllowUserToAddRows = False
        Me.WLeaderBoardGrid.AllowUserToDeleteRows = False
        Me.WLeaderBoardGrid.AllowUserToResizeColumns = False
        Me.WLeaderBoardGrid.AllowUserToResizeRows = False
        Me.WLeaderBoardGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.WLeaderBoardGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.UserIndex, Me.Username, Me.UserScore, Me.UserDate})
        Me.WLeaderBoardGrid.Enabled = False
        Me.WLeaderBoardGrid.Location = New System.Drawing.Point(9, 155)
        Me.WLeaderBoardGrid.MultiSelect = False
        Me.WLeaderBoardGrid.Name = "WLeaderBoardGrid"
        Me.WLeaderBoardGrid.ReadOnly = True
        Me.WLeaderBoardGrid.RowHeadersVisible = False
        Me.WLeaderBoardGrid.RowHeadersWidth = 21
        Me.WLeaderBoardGrid.ScrollBars = System.Windows.Forms.ScrollBars.None
        Me.WLeaderBoardGrid.ShowEditingIcon = False
        Me.WLeaderBoardGrid.Size = New System.Drawing.Size(282, 240)
        Me.WLeaderBoardGrid.TabIndex = 28
        Me.WLeaderBoardGrid.TabStop = False
        Me.WLeaderBoardGrid.Visible = False
        '
        'UserIndex
        '
        Me.UserIndex.Frozen = True
        Me.UserIndex.HeaderText = "W"
        Me.UserIndex.MinimumWidth = 6
        Me.UserIndex.Name = "UserIndex"
        Me.UserIndex.ReadOnly = True
        Me.UserIndex.Width = 21
        '
        'Username
        '
        Me.Username.Frozen = True
        Me.Username.HeaderText = "Name"
        Me.Username.MinimumWidth = 6
        Me.Username.Name = "Username"
        Me.Username.ReadOnly = True
        Me.Username.Width = 130
        '
        'UserScore
        '
        Me.UserScore.Frozen = True
        Me.UserScore.HeaderText = "Score"
        Me.UserScore.MinimumWidth = 6
        Me.UserScore.Name = "UserScore"
        Me.UserScore.ReadOnly = True
        Me.UserScore.Width = 50
        '
        'UserDate
        '
        Me.UserDate.Frozen = True
        Me.UserDate.HeaderText = "Date"
        Me.UserDate.MinimumWidth = 6
        Me.UserDate.Name = "UserDate"
        Me.UserDate.ReadOnly = True
        Me.UserDate.Width = 80
        '
        'BLeaderBoardGrid
        '
        Me.BLeaderBoardGrid.AllowUserToAddRows = False
        Me.BLeaderBoardGrid.AllowUserToDeleteRows = False
        Me.BLeaderBoardGrid.AllowUserToResizeColumns = False
        Me.BLeaderBoardGrid.AllowUserToResizeRows = False
        Me.BLeaderBoardGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.BLeaderBoardGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn1, Me.DataGridViewTextBoxColumn2, Me.DataGridViewTextBoxColumn3, Me.DataGridViewTextBoxColumn4})
        Me.BLeaderBoardGrid.Enabled = False
        Me.BLeaderBoardGrid.Location = New System.Drawing.Point(9, 155)
        Me.BLeaderBoardGrid.MultiSelect = False
        Me.BLeaderBoardGrid.Name = "BLeaderBoardGrid"
        Me.BLeaderBoardGrid.ReadOnly = True
        Me.BLeaderBoardGrid.RowHeadersVisible = False
        Me.BLeaderBoardGrid.RowHeadersWidth = 21
        Me.BLeaderBoardGrid.ScrollBars = System.Windows.Forms.ScrollBars.None
        Me.BLeaderBoardGrid.ShowEditingIcon = False
        Me.BLeaderBoardGrid.Size = New System.Drawing.Size(282, 240)
        Me.BLeaderBoardGrid.TabIndex = 29
        Me.BLeaderBoardGrid.TabStop = False
        Me.BLeaderBoardGrid.Visible = False
        '
        'DataGridViewTextBoxColumn1
        '
        Me.DataGridViewTextBoxColumn1.Frozen = True
        Me.DataGridViewTextBoxColumn1.HeaderText = "B"
        Me.DataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        Me.DataGridViewTextBoxColumn1.ReadOnly = True
        Me.DataGridViewTextBoxColumn1.Width = 21
        '
        'DataGridViewTextBoxColumn2
        '
        Me.DataGridViewTextBoxColumn2.Frozen = True
        Me.DataGridViewTextBoxColumn2.HeaderText = "Name"
        Me.DataGridViewTextBoxColumn2.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        Me.DataGridViewTextBoxColumn2.ReadOnly = True
        Me.DataGridViewTextBoxColumn2.Width = 130
        '
        'DataGridViewTextBoxColumn3
        '
        Me.DataGridViewTextBoxColumn3.Frozen = True
        Me.DataGridViewTextBoxColumn3.HeaderText = "Score"
        Me.DataGridViewTextBoxColumn3.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
        Me.DataGridViewTextBoxColumn3.ReadOnly = True
        Me.DataGridViewTextBoxColumn3.Width = 50
        '
        'DataGridViewTextBoxColumn4
        '
        Me.DataGridViewTextBoxColumn4.Frozen = True
        Me.DataGridViewTextBoxColumn4.HeaderText = "Date"
        Me.DataGridViewTextBoxColumn4.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4"
        Me.DataGridViewTextBoxColumn4.ReadOnly = True
        Me.DataGridViewTextBoxColumn4.Width = 80
        '
        'TrainingScore
        '
        Me.TrainingScore.BackColor = System.Drawing.Color.Transparent
        Me.TrainingScore.Font = New System.Drawing.Font("Microsoft Sans Serif", 27.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TrainingScore.Location = New System.Drawing.Point(114, 330)
        Me.TrainingScore.Name = "TrainingScore"
        Me.TrainingScore.Size = New System.Drawing.Size(74, 52)
        Me.TrainingScore.TabIndex = 30
        Me.TrainingScore.Text = "0"
        Me.TrainingScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TrainingScore.Visible = False
        '
        'InfoBtn
        '
        Me.InfoBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.InfoBtn.Location = New System.Drawing.Point(7, 570)
        Me.InfoBtn.Name = "InfoBtn"
        Me.InfoBtn.Size = New System.Drawing.Size(18, 23)
        Me.InfoBtn.TabIndex = 31
        Me.InfoBtn.Text = "?"
        Me.InfoBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.InfoBtn.UseVisualStyleBackColor = True
        Me.InfoBtn.Visible = False
        '
        'UseBook
        '
        Me.UseBook.AutoSize = True
        Me.UseBook.Enabled = False
        Me.UseBook.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UseBook.Location = New System.Drawing.Point(977, 435)
        Me.UseBook.Name = "UseBook"
        Me.UseBook.Size = New System.Drawing.Size(136, 19)
        Me.UseBook.TabIndex = 26
        Me.UseBook.Text = "Use Opening Book?"
        Me.UseBook.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.UseBook.UseVisualStyleBackColor = True
        '
        'PieceHeatMapBox
        '
        Me.PieceHeatMapBox.AutoSize = True
        Me.PieceHeatMapBox.Checked = True
        Me.PieceHeatMapBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.PieceHeatMapBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PieceHeatMapBox.Location = New System.Drawing.Point(970, 410)
        Me.PieceHeatMapBox.Name = "PieceHeatMapBox"
        Me.PieceHeatMapBox.Size = New System.Drawing.Size(152, 19)
        Me.PieceHeatMapBox.TabIndex = 26
        Me.PieceHeatMapBox.Text = "Use Piece Heat Maps?"
        Me.PieceHeatMapBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.PieceHeatMapBox.UseVisualStyleBackColor = True
        '
        'NextPuzzleBtn
        '
        Me.NextPuzzleBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.NextPuzzleBtn.Location = New System.Drawing.Point(9, 124)
        Me.NextPuzzleBtn.Name = "NextPuzzleBtn"
        Me.NextPuzzleBtn.Size = New System.Drawing.Size(282, 43)
        Me.NextPuzzleBtn.TabIndex = 7
        Me.NextPuzzleBtn.Text = "Next Puzzle"
        Me.NextPuzzleBtn.UseVisualStyleBackColor = True
        Me.NextPuzzleBtn.Visible = False
        '
        'HintBtn
        '
        Me.HintBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HintBtn.Location = New System.Drawing.Point(9, 75)
        Me.HintBtn.Name = "HintBtn"
        Me.HintBtn.Size = New System.Drawing.Size(133, 43)
        Me.HintBtn.TabIndex = 5
        Me.HintBtn.Text = "Hint"
        Me.HintBtn.UseVisualStyleBackColor = True
        Me.HintBtn.Visible = False
        '
        'GiveUpBtn
        '
        Me.GiveUpBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GiveUpBtn.Location = New System.Drawing.Point(150, 75)
        Me.GiveUpBtn.Name = "GiveUpBtn"
        Me.GiveUpBtn.Size = New System.Drawing.Size(141, 43)
        Me.GiveUpBtn.TabIndex = 7
        Me.GiveUpBtn.Text = "Give Up"
        Me.GiveUpBtn.UseVisualStyleBackColor = True
        Me.GiveUpBtn.Visible = False
        '
        'AIPuzzleInfoLabel
        '
        Me.AIPuzzleInfoLabel.AutoSize = True
        Me.AIPuzzleInfoLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AIPuzzleInfoLabel.Location = New System.Drawing.Point(35, 332)
        Me.AIPuzzleInfoLabel.Name = "AIPuzzleInfoLabel"
        Me.AIPuzzleInfoLabel.Size = New System.Drawing.Size(228, 20)
        Me.AIPuzzleInfoLabel.TabIndex = 9
        Me.AIPuzzleInfoLabel.Text = "AI is Searching on the Puzzle..."
        Me.AIPuzzleInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.AIPuzzleInfoLabel.Visible = False
        '
        'RatingLabel
        '
        Me.RatingLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 36.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RatingLabel.Location = New System.Drawing.Point(79, 249)
        Me.RatingLabel.Name = "RatingLabel"
        Me.RatingLabel.Size = New System.Drawing.Size(136, 55)
        Me.RatingLabel.TabIndex = 9
        Me.RatingLabel.Text = "1000"
        Me.RatingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.RatingLabel.Visible = False
        '
        'LostRatingLabel
        '
        Me.LostRatingLabel.AutoSize = True
        Me.LostRatingLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LostRatingLabel.Location = New System.Drawing.Point(71, 296)
        Me.LostRatingLabel.Name = "LostRatingLabel"
        Me.LostRatingLabel.Size = New System.Drawing.Size(32, 20)
        Me.LostRatingLabel.TabIndex = 9
        Me.LostRatingLabel.Text = "-10"
        Me.LostRatingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.LostRatingLabel.Visible = False
        '
        'GainedRatingLabel
        '
        Me.GainedRatingLabel.AutoSize = True
        Me.GainedRatingLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GainedRatingLabel.Location = New System.Drawing.Point(188, 296)
        Me.GainedRatingLabel.Name = "GainedRatingLabel"
        Me.GainedRatingLabel.Size = New System.Drawing.Size(36, 20)
        Me.GainedRatingLabel.TabIndex = 9
        Me.GainedRatingLabel.Text = "+10"
        Me.GainedRatingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.GainedRatingLabel.Visible = False
        '
        'RatingHeader
        '
        Me.RatingHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RatingHeader.Location = New System.Drawing.Point(100, 225)
        Me.RatingHeader.Name = "RatingHeader"
        Me.RatingHeader.Size = New System.Drawing.Size(98, 20)
        Me.RatingHeader.TabIndex = 9
        Me.RatingHeader.Text = "Your Rating:"
        Me.RatingHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.RatingHeader.Visible = False
        '
        'PuzzleRatingLabel
        '
        Me.PuzzleRatingLabel.AutoSize = True
        Me.PuzzleRatingLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PuzzleRatingLabel.Location = New System.Drawing.Point(86, 51)
        Me.PuzzleRatingLabel.Name = "PuzzleRatingLabel"
        Me.PuzzleRatingLabel.Size = New System.Drawing.Size(122, 16)
        Me.PuzzleRatingLabel.TabIndex = 9
        Me.PuzzleRatingLabel.Text = "Puzzle Rating: 1000"
        Me.PuzzleRatingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.PuzzleRatingLabel.Visible = False
        '
        'AutoAdvanceBox
        '
        Me.AutoAdvanceBox.BackColor = System.Drawing.Color.WhiteSmoke
        Me.AutoAdvanceBox.Cursor = System.Windows.Forms.Cursors.Hand
        Me.AutoAdvanceBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AutoAdvanceBox.Location = New System.Drawing.Point(167, 438)
        Me.AutoAdvanceBox.Name = "AutoAdvanceBox"
        Me.AutoAdvanceBox.ReadOnly = True
        Me.AutoAdvanceBox.Size = New System.Drawing.Size(120, 22)
        Me.AutoAdvanceBox.TabIndex = 23
        Me.AutoAdvanceBox.Text = "Never"
        Me.AutoAdvanceBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.AutoAdvanceBox.Visible = False
        '
        'AutoAdvanceBar
        '
        Me.AutoAdvanceBar.LargeChange = 2
        Me.AutoAdvanceBar.Location = New System.Drawing.Point(167, 464)
        Me.AutoAdvanceBar.Maximum = 14
        Me.AutoAdvanceBar.Name = "AutoAdvanceBar"
        Me.AutoAdvanceBar.Size = New System.Drawing.Size(120, 45)
        Me.AutoAdvanceBar.TabIndex = 25
        Me.AutoAdvanceBar.Value = 14
        Me.AutoAdvanceBar.Visible = False
        '
        'AutoAdvanceOnComplete
        '
        Me.AutoAdvanceOnComplete.AutoSize = True
        Me.AutoAdvanceOnComplete.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AutoAdvanceOnComplete.Location = New System.Drawing.Point(8, 413)
        Me.AutoAdvanceOnComplete.Name = "AutoAdvanceOnComplete"
        Me.AutoAdvanceOnComplete.Size = New System.Drawing.Size(280, 19)
        Me.AutoAdvanceOnComplete.TabIndex = 26
        Me.AutoAdvanceOnComplete.Text = "Automatically Advance When Correct / Incorrect"
        Me.AutoAdvanceOnComplete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.AutoAdvanceOnComplete.UseVisualStyleBackColor = True
        Me.AutoAdvanceOnComplete.Visible = False
        '
        'AutoAdvanceLabel
        '
        Me.AutoAdvanceLabel.AutoSize = True
        Me.AutoAdvanceLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AutoAdvanceLabel.Location = New System.Drawing.Point(5, 438)
        Me.AutoAdvanceLabel.Name = "AutoAdvanceLabel"
        Me.AutoAdvanceLabel.Size = New System.Drawing.Size(158, 15)
        Me.AutoAdvanceLabel.TabIndex = 9
        Me.AutoAdvanceLabel.Text = "Automatically Advance After:"
        Me.AutoAdvanceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.AutoAdvanceLabel.Visible = False
        '
        'HumanModeBtn
        '
        Me.HumanModeBtn.AutoSize = True
        Me.HumanModeBtn.Checked = True
        Me.HumanModeBtn.Location = New System.Drawing.Point(22, 390)
        Me.HumanModeBtn.Name = "HumanModeBtn"
        Me.HumanModeBtn.Size = New System.Drawing.Size(123, 17)
        Me.HumanModeBtn.TabIndex = 32
        Me.HumanModeBtn.TabStop = True
        Me.HumanModeBtn.Text = "Human Puzzle Mode"
        Me.HumanModeBtn.UseVisualStyleBackColor = True
        Me.HumanModeBtn.Visible = False
        '
        'AIModeBtn
        '
        Me.AIModeBtn.AutoSize = True
        Me.AIModeBtn.Location = New System.Drawing.Point(166, 390)
        Me.AIModeBtn.Name = "AIModeBtn"
        Me.AIModeBtn.Size = New System.Drawing.Size(99, 17)
        Me.AIModeBtn.TabIndex = 33
        Me.AIModeBtn.Text = "AI Puzzle Mode"
        Me.AIModeBtn.UseVisualStyleBackColor = True
        Me.AIModeBtn.Visible = False
        '
        'ResetRatingsBtn
        '
        Me.ResetRatingsBtn.Location = New System.Drawing.Point(247, 565)
        Me.ResetRatingsBtn.Name = "ResetRatingsBtn"
        Me.ResetRatingsBtn.Size = New System.Drawing.Size(52, 34)
        Me.ResetRatingsBtn.TabIndex = 34
        Me.ResetRatingsBtn.Text = "Reset Ratings"
        Me.ResetRatingsBtn.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.ResetRatingsBtn.UseVisualStyleBackColor = True
        Me.ResetRatingsBtn.Visible = False
        '
        'PGNExport
        '
        Me.PGNExport.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PGNExport.Location = New System.Drawing.Point(221, 85)
        Me.PGNExport.Name = "PGNExport"
        Me.PGNExport.Size = New System.Drawing.Size(70, 43)
        Me.PGNExport.TabIndex = 7
        Me.PGNExport.Text = "Export PGN"
        Me.PGNExport.UseVisualStyleBackColor = True
        '
        'AutoOutputterPanel
        '
        Me.AutoOutputterPanel.Controls.Add(Me.PGNAutoOutputter)
        Me.AutoOutputterPanel.Controls.Add(Me.FENAutoOutputter)
        Me.AutoOutputterPanel.Location = New System.Drawing.Point(45, 51)
        Me.AutoOutputterPanel.Name = "AutoOutputterPanel"
        Me.AutoOutputterPanel.Size = New System.Drawing.Size(214, 24)
        Me.AutoOutputterPanel.TabIndex = 35
        Me.AutoOutputterPanel.Visible = False
        '
        'PGNAutoOutputter
        '
        Me.PGNAutoOutputter.AutoSize = True
        Me.PGNAutoOutputter.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PGNAutoOutputter.Location = New System.Drawing.Point(124, 0)
        Me.PGNAutoOutputter.Name = "PGNAutoOutputter"
        Me.PGNAutoOutputter.Size = New System.Drawing.Size(90, 19)
        Me.PGNAutoOutputter.TabIndex = 1
        Me.PGNAutoOutputter.Text = "Output PGN"
        Me.PGNAutoOutputter.UseVisualStyleBackColor = True
        '
        'FENAutoOutputter
        '
        Me.FENAutoOutputter.AutoSize = True
        Me.FENAutoOutputter.Checked = True
        Me.FENAutoOutputter.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FENAutoOutputter.Location = New System.Drawing.Point(3, 0)
        Me.FENAutoOutputter.Name = "FENAutoOutputter"
        Me.FENAutoOutputter.Size = New System.Drawing.Size(88, 19)
        Me.FENAutoOutputter.TabIndex = 0
        Me.FENAutoOutputter.TabStop = True
        Me.FENAutoOutputter.Text = "Output FEN"
        Me.FENAutoOutputter.UseVisualStyleBackColor = True
        '
        'NodeTestBtn
        '
        Me.NodeTestBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.NodeTestBtn.Location = New System.Drawing.Point(62, 318)
        Me.NodeTestBtn.Name = "NodeTestBtn"
        Me.NodeTestBtn.Size = New System.Drawing.Size(175, 49)
        Me.NodeTestBtn.TabIndex = 27
        Me.NodeTestBtn.Text = "Perform Node Test:"
        Me.NodeTestBtn.UseVisualStyleBackColor = True
        '
        'NodeTestStopBtn
        '
        Me.NodeTestStopBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.NodeTestStopBtn.Location = New System.Drawing.Point(62, 318)
        Me.NodeTestStopBtn.Name = "NodeTestStopBtn"
        Me.NodeTestStopBtn.Size = New System.Drawing.Size(175, 49)
        Me.NodeTestStopBtn.TabIndex = 27
        Me.NodeTestStopBtn.Text = "Terminate Node Test:"
        Me.NodeTestStopBtn.UseVisualStyleBackColor = True
        Me.NodeTestStopBtn.Visible = False
        '
        'AutoResetter
        '
        Me.AutoResetter.AutoSize = True
        Me.AutoResetter.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AutoResetter.ForeColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.AutoResetter.Location = New System.Drawing.Point(970, 479)
        Me.AutoResetter.Name = "AutoResetter"
        Me.AutoResetter.Size = New System.Drawing.Size(150, 17)
        Me.AutoResetter.TabIndex = 36
        Me.AutoResetter.Text = "Automatically Reset Game"
        Me.AutoResetter.UseVisualStyleBackColor = True
        Me.AutoResetter.Visible = False
        '
        'BoardEditorBtn
        '
        Me.BoardEditorBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BoardEditorBtn.Location = New System.Drawing.Point(53, 213)
        Me.BoardEditorBtn.Name = "BoardEditorBtn"
        Me.BoardEditorBtn.Size = New System.Drawing.Size(194, 43)
        Me.BoardEditorBtn.TabIndex = 18
        Me.BoardEditorBtn.Text = "Board Editor Mode:"
        Me.BoardEditorBtn.UseVisualStyleBackColor = True
        '
        'BoardEditCancelBtn
        '
        Me.BoardEditCancelBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BoardEditCancelBtn.Location = New System.Drawing.Point(158, 213)
        Me.BoardEditCancelBtn.Name = "BoardEditCancelBtn"
        Me.BoardEditCancelBtn.Size = New System.Drawing.Size(90, 43)
        Me.BoardEditCancelBtn.TabIndex = 18
        Me.BoardEditCancelBtn.Text = "Cancel Changes:"
        Me.BoardEditCancelBtn.UseVisualStyleBackColor = True
        Me.BoardEditCancelBtn.Visible = False
        '
        'BoardEditBlackMove
        '
        Me.BoardEditBlackMove.AutoSize = True
        Me.BoardEditBlackMove.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BoardEditBlackMove.Location = New System.Drawing.Point(150, 2)
        Me.BoardEditBlackMove.Name = "BoardEditBlackMove"
        Me.BoardEditBlackMove.Size = New System.Drawing.Size(114, 21)
        Me.BoardEditBlackMove.TabIndex = 1
        Me.BoardEditBlackMove.Text = "Black to Move"
        Me.BoardEditBlackMove.UseVisualStyleBackColor = True
        '
        'BoardEditWhiteMove
        '
        Me.BoardEditWhiteMove.AutoSize = True
        Me.BoardEditWhiteMove.Checked = True
        Me.BoardEditWhiteMove.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BoardEditWhiteMove.Location = New System.Drawing.Point(4, 2)
        Me.BoardEditWhiteMove.Name = "BoardEditWhiteMove"
        Me.BoardEditWhiteMove.Size = New System.Drawing.Size(116, 21)
        Me.BoardEditWhiteMove.TabIndex = 0
        Me.BoardEditWhiteMove.TabStop = True
        Me.BoardEditWhiteMove.Text = "White to Move"
        Me.BoardEditWhiteMove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.BoardEditWhiteMove.UseVisualStyleBackColor = True
        '
        'BoardEditPanel
        '
        Me.BoardEditPanel.Controls.Add(Me.BoardEditTipLabel)
        Me.BoardEditPanel.Controls.Add(Me.BoardEditEnPassantLabel)
        Me.BoardEditPanel.Controls.Add(Me.BoardEditBLabel)
        Me.BoardEditPanel.Controls.Add(Me.BoardEditWLabel)
        Me.BoardEditPanel.Controls.Add(Me.BoardEditBQSBox)
        Me.BoardEditPanel.Controls.Add(Me.BoardEditWQSBox)
        Me.BoardEditPanel.Controls.Add(Me.BoardEditBKSBox)
        Me.BoardEditPanel.Controls.Add(Me.BoardEditWKSBox)
        Me.BoardEditPanel.Controls.Add(Me.BoardEditBlackMove)
        Me.BoardEditPanel.Controls.Add(Me.BoardEditWhiteMove)
        Me.BoardEditPanel.Location = New System.Drawing.Point(911, 5)
        Me.BoardEditPanel.Name = "BoardEditPanel"
        Me.BoardEditPanel.Size = New System.Drawing.Size(272, 135)
        Me.BoardEditPanel.TabIndex = 36
        Me.BoardEditPanel.Visible = False
        '
        'BoardEditTipLabel
        '
        Me.BoardEditTipLabel.Location = New System.Drawing.Point(-5, 112)
        Me.BoardEditTipLabel.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.BoardEditTipLabel.Name = "BoardEditTipLabel"
        Me.BoardEditTipLabel.Size = New System.Drawing.Size(271, 19)
        Me.BoardEditTipLabel.TabIndex = 4
        Me.BoardEditTipLabel.Text = "Tip: Hold CTRL when Moving a Piece to Duplicate it."
        Me.BoardEditTipLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BoardEditEnPassantLabel
        '
        Me.BoardEditEnPassantLabel.Location = New System.Drawing.Point(0, 92)
        Me.BoardEditEnPassantLabel.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.BoardEditEnPassantLabel.Name = "BoardEditEnPassantLabel"
        Me.BoardEditEnPassantLabel.Size = New System.Drawing.Size(263, 19)
        Me.BoardEditEnPassantLabel.TabIndex = 4
        Me.BoardEditEnPassantLabel.Text = "Right-Click on a Square to Flag it as En-Passant."
        Me.BoardEditEnPassantLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BoardEditBLabel
        '
        Me.BoardEditBLabel.AutoSize = True
        Me.BoardEditBLabel.Location = New System.Drawing.Point(148, 29)
        Me.BoardEditBLabel.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.BoardEditBLabel.Name = "BoardEditBLabel"
        Me.BoardEditBLabel.Size = New System.Drawing.Size(107, 13)
        Me.BoardEditBLabel.TabIndex = 3
        Me.BoardEditBLabel.Text = "Black Castling Rules:"
        '
        'BoardEditWLabel
        '
        Me.BoardEditWLabel.AutoSize = True
        Me.BoardEditWLabel.Location = New System.Drawing.Point(2, 29)
        Me.BoardEditWLabel.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.BoardEditWLabel.Name = "BoardEditWLabel"
        Me.BoardEditWLabel.Size = New System.Drawing.Size(108, 13)
        Me.BoardEditWLabel.TabIndex = 3
        Me.BoardEditWLabel.Text = "White Castling Rules:"
        '
        'BoardEditBQSBox
        '
        Me.BoardEditBQSBox.AutoSize = True
        Me.BoardEditBQSBox.Location = New System.Drawing.Point(150, 68)
        Me.BoardEditBQSBox.Margin = New System.Windows.Forms.Padding(2)
        Me.BoardEditBQSBox.Name = "BoardEditBQSBox"
        Me.BoardEditBQSBox.Size = New System.Drawing.Size(82, 17)
        Me.BoardEditBQSBox.TabIndex = 2
        Me.BoardEditBQSBox.Text = "Queen Side"
        Me.BoardEditBQSBox.UseVisualStyleBackColor = True
        '
        'BoardEditWQSBox
        '
        Me.BoardEditWQSBox.AutoSize = True
        Me.BoardEditWQSBox.Location = New System.Drawing.Point(4, 68)
        Me.BoardEditWQSBox.Margin = New System.Windows.Forms.Padding(2)
        Me.BoardEditWQSBox.Name = "BoardEditWQSBox"
        Me.BoardEditWQSBox.Size = New System.Drawing.Size(82, 17)
        Me.BoardEditWQSBox.TabIndex = 2
        Me.BoardEditWQSBox.Text = "Queen Side"
        Me.BoardEditWQSBox.UseVisualStyleBackColor = True
        '
        'BoardEditBKSBox
        '
        Me.BoardEditBKSBox.AutoSize = True
        Me.BoardEditBKSBox.Location = New System.Drawing.Point(150, 47)
        Me.BoardEditBKSBox.Margin = New System.Windows.Forms.Padding(2)
        Me.BoardEditBKSBox.Name = "BoardEditBKSBox"
        Me.BoardEditBKSBox.Size = New System.Drawing.Size(71, 17)
        Me.BoardEditBKSBox.TabIndex = 2
        Me.BoardEditBKSBox.Text = "King Side"
        Me.BoardEditBKSBox.UseVisualStyleBackColor = True
        '
        'BoardEditWKSBox
        '
        Me.BoardEditWKSBox.AutoSize = True
        Me.BoardEditWKSBox.Location = New System.Drawing.Point(4, 47)
        Me.BoardEditWKSBox.Margin = New System.Windows.Forms.Padding(2)
        Me.BoardEditWKSBox.Name = "BoardEditWKSBox"
        Me.BoardEditWKSBox.Size = New System.Drawing.Size(71, 17)
        Me.BoardEditWKSBox.TabIndex = 2
        Me.BoardEditWKSBox.Text = "King Side"
        Me.BoardEditWKSBox.UseVisualStyleBackColor = True
        '
        'Chess
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(155, Byte), Integer), CType(CType(155, Byte), Integer), CType(CType(155, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1182, 592)
        Me.Controls.Add(Me.BoardEditPanel)
        Me.Controls.Add(Me.AutoResetter)
        Me.Controls.Add(Me.AutoOutputterPanel)
        Me.Controls.Add(Me.ResetRatingsBtn)
        Me.Controls.Add(Me.AIModeBtn)
        Me.Controls.Add(Me.HumanModeBtn)
        Me.Controls.Add(Me.InfoBtn)
        Me.Controls.Add(Me.TrainingScore)
        Me.Controls.Add(Me.NodeTestStopBtn)
        Me.Controls.Add(Me.NodeTestBtn)
        Me.Controls.Add(Me.AITerminator)
        Me.Controls.Add(Me.UseBook)
        Me.Controls.Add(Me.AIEndlessMode)
        Me.Controls.Add(Me.PieceHeatMapBox)
        Me.Controls.Add(Me.AutoAdvanceOnComplete)
        Me.Controls.Add(Me.QuiescenceBox)
        Me.Controls.Add(Me.AutoAdvanceBar)
        Me.Controls.Add(Me.UserTimeBar)
        Me.Controls.Add(Me.AutoAdvanceBox)
        Me.Controls.Add(Me.UserTimeBox)
        Me.Controls.Add(Me.ExitBtn)
        Me.Controls.Add(Me.AutoFlipper)
        Me.Controls.Add(Me.BoardEditCancelBtn)
        Me.Controls.Add(Me.BoardEditorBtn)
        Me.Controls.Add(Me.FlipperButton)
        Me.Controls.Add(Me.SettingsBtn)
        Me.Controls.Add(Me.ProgressBar)
        Me.Controls.Add(Me.UndoFENChange)
        Me.Controls.Add(Me.Credits)
        Me.Controls.Add(Me.GainedRatingLabel)
        Me.Controls.Add(Me.LostRatingLabel)
        Me.Controls.Add(Me.AutoAdvanceLabel)
        Me.Controls.Add(Me.RatingHeader)
        Me.Controls.Add(Me.PuzzleRatingLabel)
        Me.Controls.Add(Me.AIPuzzleInfoLabel)
        Me.Controls.Add(Me.RatingLabel)
        Me.Controls.Add(Me.TimerLabel)
        Me.Controls.Add(Me.CurrentAIDepth)
        Me.Controls.Add(Me.CurrentAIMove)
        Me.Controls.Add(Me.CurrentAIEval)
        Me.Controls.Add(Me.CheckLabel)
        Me.Controls.Add(Me.NextPuzzleBtn)
        Me.Controls.Add(Me.TrainingStart)
        Me.Controls.Add(Me.UndoMove)
        Me.Controls.Add(Me.GiveUpBtn)
        Me.Controls.Add(Me.PGNExport)
        Me.Controls.Add(Me.FENExport)
        Me.Controls.Add(Me.HintBtn)
        Me.Controls.Add(Me.Reset_Btn)
        Me.Controls.Add(Me.AIMoveBtn)
        Me.Controls.Add(Me.InputButton)
        Me.Controls.Add(Me.InputTextBox)
        Me.Controls.Add(Me.BP1)
        Me.Controls.Add(Me.BR1)
        Me.Controls.Add(Me.BN1)
        Me.Controls.Add(Me.BB1)
        Me.Controls.Add(Me.BQ1)
        Me.Controls.Add(Me.BK1)
        Me.Controls.Add(Me.WP1)
        Me.Controls.Add(Me.WR1)
        Me.Controls.Add(Me.WN1)
        Me.Controls.Add(Me.WB1)
        Me.Controls.Add(Me.WQ1)
        Me.Controls.Add(Me.WK1)
        Me.Controls.Add(Me.Checkerboard)
        Me.Controls.Add(Me.BLeaderBoardGrid)
        Me.Controls.Add(Me.WLeaderBoardGrid)
        Me.Controls.Add(Me.MoveDisplayer)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(1200, 638)
        Me.Name = "Chess"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Chess Game & Artificial Intelligence"
        Me.ColourChanger.ResumeLayout(False)
        CType(Me.BP1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BR1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BN1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BB1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BQ1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BK1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WP1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WR1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WN1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WB1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WQ1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WK1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Checkerboard, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.UserTimeBar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WLeaderBoardGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BLeaderBoardGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AutoAdvanceBar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.AutoOutputterPanel.ResumeLayout(False)
        Me.AutoOutputterPanel.PerformLayout()
        Me.BoardEditPanel.ResumeLayout(False)
        Me.BoardEditPanel.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents WK1 As System.Windows.Forms.PictureBox
    Friend WithEvents WQ1 As System.Windows.Forms.PictureBox
    Friend WithEvents WB1 As System.Windows.Forms.PictureBox
    Friend WithEvents WN1 As System.Windows.Forms.PictureBox
    Friend WithEvents WR1 As System.Windows.Forms.PictureBox
    Friend WithEvents WP1 As System.Windows.Forms.PictureBox
    Friend WithEvents BK1 As System.Windows.Forms.PictureBox
    Friend WithEvents BQ1 As System.Windows.Forms.PictureBox
    Friend WithEvents BB1 As System.Windows.Forms.PictureBox
    Friend WithEvents BN1 As System.Windows.Forms.PictureBox
    Friend WithEvents BR1 As System.Windows.Forms.PictureBox
    Friend WithEvents BP1 As System.Windows.Forms.PictureBox
    Friend WithEvents Checkerboard As System.Windows.Forms.PictureBox
    Friend WithEvents InputTextBox As TextBox
    Friend WithEvents InputButton As Button
    Friend WithEvents AIMoveBtn As Button
    Friend WithEvents Reset_Btn As Button
    Friend WithEvents FENExport As Button
    Friend WithEvents CheckLabel As Label
    Friend WithEvents CurrentAIEval As Label
    Friend WithEvents UndoMove As System.Windows.Forms.Button
    Friend WithEvents Credits As Label
    Friend WithEvents UndoFENChange As System.Windows.Forms.Button
    Friend WithEvents ProgressBar As ProgressBar
    Friend WithEvents SettingsBtn As System.Windows.Forms.Button
    Friend WithEvents ColourChanger As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents Def As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LBlu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DBlu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Gld As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Grn As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Ppl As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Mon As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FlipperButton As System.Windows.Forms.Button
    Friend WithEvents AutoFlipper As System.Windows.Forms.CheckBox
    Friend WithEvents ExitBtn As Button
    Friend WithEvents UserTimeBox As TextBox
    Friend WithEvents UserTimeBar As TrackBar
    Friend WithEvents CurrentAIMove As Label
    Friend WithEvents CurrentAIDepth As Label
    Friend WithEvents QuiescenceBox As CheckBox
    Friend WithEvents AITerminator As Button
    Friend WithEvents AIEndlessMode As System.Windows.Forms.CheckBox
    Friend WithEvents Red As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TrainingStart As Button
    Friend WithEvents TimerLabel As Label
    Friend WithEvents MoveDisplayer As Label
    Friend WithEvents WLeaderBoardGrid As DataGridView
    Friend WithEvents BLeaderBoardGrid As DataGridView
    Friend WithEvents UserIndex As DataGridViewTextBoxColumn
    Friend WithEvents Username As DataGridViewTextBoxColumn
    Friend WithEvents UserScore As DataGridViewTextBoxColumn
    Friend WithEvents UserDate As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn2 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn3 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn4 As DataGridViewTextBoxColumn
    Friend WithEvents TrainingScore As Label
    Friend WithEvents TrainingTimer As Timer
    Friend WithEvents InfoBtn As Button
    Friend WithEvents UseBook As System.Windows.Forms.CheckBox
    Friend WithEvents PieceHeatMapBox As CheckBox
    Friend WithEvents NextPuzzleBtn As Button
    Friend WithEvents HintBtn As Button
    Friend WithEvents GiveUpBtn As Button
    Friend WithEvents AIPuzzleInfoLabel As Label
    Friend WithEvents RatingLabel As Label
    Friend WithEvents LostRatingLabel As Label
    Friend WithEvents GainedRatingLabel As Label
    Friend WithEvents RatingHeader As Label
    Friend WithEvents PuzzleRatingLabel As Label
    Friend WithEvents AutoAdvanceBox As TextBox
    Friend WithEvents AutoAdvanceBar As TrackBar
    Friend WithEvents AutoAdvanceOnComplete As CheckBox
    Friend WithEvents AutoAdvanceLabel As Label
    Friend WithEvents HumanModeBtn As RadioButton
    Friend WithEvents AIModeBtn As RadioButton
    Friend WithEvents ResetRatingsBtn As Button
    Friend WithEvents PGNExport As Button
    Friend WithEvents AutoOutputterPanel As Panel
    Friend WithEvents PGNAutoOutputter As RadioButton
    Friend WithEvents FENAutoOutputter As RadioButton
    Friend WithEvents NodeTestBtn As Button
    Friend WithEvents NodeTestStopBtn As Button
    Friend WithEvents AutoResetter As CheckBox
    Friend WithEvents BoardEditorBtn As Button
    Friend WithEvents BoardEditCancelBtn As Button
    Friend WithEvents BoardEditBlackMove As RadioButton
    Friend WithEvents BoardEditWhiteMove As RadioButton
    Friend WithEvents BoardEditPanel As Panel
    Friend WithEvents BoardEditEnPassantLabel As Label
    Friend WithEvents BoardEditBLabel As Label
    Friend WithEvents BoardEditWLabel As Label
    Friend WithEvents BoardEditBQSBox As CheckBox
    Friend WithEvents BoardEditWQSBox As CheckBox
    Friend WithEvents BoardEditBKSBox As CheckBox
    Friend WithEvents BoardEditWKSBox As CheckBox
    Friend WithEvents BoardEditTipLabel As Label
End Class
