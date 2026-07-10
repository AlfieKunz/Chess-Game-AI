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
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Chess))
        InputTextBox = New TextBox()
        InputButton = New Button()
        AIMoveBtn = New Button()
        Reset_Btn = New Button()
        FENExport = New Button()
        CheckLabel = New Label()
        CurrentAIEval = New Label()
        UndoMove = New Button()
        Credits = New Label()
        UndoFENChange = New Button()
        ProgressBar = New ProgressBar()
        SettingsBtn = New Button()
        BP1 = New PictureBox()
        BR1 = New PictureBox()
        BN1 = New PictureBox()
        BB1 = New PictureBox()
        BQ1 = New PictureBox()
        BK1 = New PictureBox()
        WP1 = New PictureBox()
        WR1 = New PictureBox()
        WN1 = New PictureBox()
        WB1 = New PictureBox()
        WQ1 = New PictureBox()
        WK1 = New PictureBox()
        Checkerboard = New PictureBox()
        FlipperButton = New Button()
        AutoFlipper = New CheckBox()
        ExitBtn = New Button()
        UserTimeBox = New TextBox()
        UserTimeBar = New TrackBar()
        CurrentAIMove = New Label()
        CurrentAIDepth = New Label()
        AITerminator = New Button()
        AIEndlessMode = New CheckBox()
        TrainingStart = New Button()
        TimerLabel = New Label()
        MoveDisplayer = New Label()
        WLeaderBoardGrid = New DataGridView()
        UserIndex = New DataGridViewTextBoxColumn()
        Username = New DataGridViewTextBoxColumn()
        UserScore = New DataGridViewTextBoxColumn()
        UserDate = New DataGridViewTextBoxColumn()
        BLeaderBoardGrid = New DataGridView()
        DataGridViewTextBoxColumn1 = New DataGridViewTextBoxColumn()
        DataGridViewTextBoxColumn2 = New DataGridViewTextBoxColumn()
        DataGridViewTextBoxColumn3 = New DataGridViewTextBoxColumn()
        DataGridViewTextBoxColumn4 = New DataGridViewTextBoxColumn()
        TrainingScore = New Label()
        InfoBtn = New Button()
        UseBook = New CheckBox()
        NextPuzzleBtn = New Button()
        HintBtn = New Button()
        GiveUpBtn = New Button()
        AIPuzzleInfoLabel = New Label()
        RatingLabel = New Label()
        LostRatingLabel = New Label()
        GainedRatingLabel = New Label()
        RatingHeader = New Label()
        PuzzleRatingLabel = New Label()
        AutoAdvanceBox = New TextBox()
        AutoAdvanceBar = New TrackBar()
        AutoAdvanceOnComplete = New CheckBox()
        AutoAdvanceLabel = New Label()
        HumanModeBtn = New RadioButton()
        AIModeBtn = New RadioButton()
        ResetRatingsBtn = New Button()
        PGNExport = New Button()
        AutoOutputterPanel = New Panel()
        PGNAutoOutputter = New RadioButton()
        FENAutoOutputter = New RadioButton()
        NodeTestBtn = New Button()
        NodeTestStopBtn = New Button()
        AutoResetter = New CheckBox()
        BoardEditorBtn = New Button()
        BoardEditCancelBtn = New Button()
        BoardEditBlackMove = New RadioButton()
        BoardEditWhiteMove = New RadioButton()
        BoardEditPanel = New Panel()
        BoardEditTipLabel = New Label()
        BoardEditEnPassantLabel = New Label()
        BoardEditBLabel = New Label()
        BoardEditWLabel = New Label()
        BoardEditBQSBox = New CheckBox()
        BoardEditWQSBox = New CheckBox()
        BoardEditBKSBox = New CheckBox()
        BoardEditWKSBox = New CheckBox()
        RemoteModeBtn = New Button()
        ChessTimer = New Timer(components)
        BoardEditDiscardBtn = New Button()
        AISettingsPanel = New Panel()
        AISettingResetBtn = New Button()
        AISettingsBox = New ComboBox()
        CType(BP1, ComponentModel.ISupportInitialize).BeginInit()
        CType(BR1, ComponentModel.ISupportInitialize).BeginInit()
        CType(BN1, ComponentModel.ISupportInitialize).BeginInit()
        CType(BB1, ComponentModel.ISupportInitialize).BeginInit()
        CType(BQ1, ComponentModel.ISupportInitialize).BeginInit()
        CType(BK1, ComponentModel.ISupportInitialize).BeginInit()
        CType(WP1, ComponentModel.ISupportInitialize).BeginInit()
        CType(WR1, ComponentModel.ISupportInitialize).BeginInit()
        CType(WN1, ComponentModel.ISupportInitialize).BeginInit()
        CType(WB1, ComponentModel.ISupportInitialize).BeginInit()
        CType(WQ1, ComponentModel.ISupportInitialize).BeginInit()
        CType(WK1, ComponentModel.ISupportInitialize).BeginInit()
        CType(Checkerboard, ComponentModel.ISupportInitialize).BeginInit()
        CType(UserTimeBar, ComponentModel.ISupportInitialize).BeginInit()
        CType(WLeaderBoardGrid, ComponentModel.ISupportInitialize).BeginInit()
        CType(BLeaderBoardGrid, ComponentModel.ISupportInitialize).BeginInit()
        CType(AutoAdvanceBar, ComponentModel.ISupportInitialize).BeginInit()
        AutoOutputterPanel.SuspendLayout()
        BoardEditPanel.SuspendLayout()
        AISettingsPanel.SuspendLayout()
        SuspendLayout()
        ' 
        ' InputTextBox
        ' 
        InputTextBox.Location = New Point(9, 12)
        InputTextBox.Multiline = True
        InputTextBox.Name = "InputTextBox"
        InputTextBox.Size = New Size(282, 60)
        InputTextBox.TabIndex = 2
        ' 
        ' InputButton
        ' 
        InputButton.Location = New Point(66, 78)
        InputButton.Name = "InputButton"
        InputButton.Size = New Size(171, 23)
        InputButton.TabIndex = 3
        InputButton.Text = "Click to Submit FEN / Move(s):"
        InputButton.UseVisualStyleBackColor = True
        ' 
        ' AIMoveBtn
        ' 
        AIMoveBtn.Font = New Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AIMoveBtn.Location = New Point(945, 278)
        AIMoveBtn.Name = "AIMoveBtn"
        AIMoveBtn.Size = New Size(209, 57)
        AIMoveBtn.TabIndex = 4
        AIMoveBtn.Text = "Make Computer Move:"
        AIMoveBtn.UseVisualStyleBackColor = True
        ' 
        ' Reset_Btn
        ' 
        Reset_Btn.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Reset_Btn.Location = New Point(9, 110)
        Reset_Btn.Name = "Reset_Btn"
        Reset_Btn.Size = New Size(133, 43)
        Reset_Btn.TabIndex = 5
        Reset_Btn.Text = "Reset Board"
        Reset_Btn.UseVisualStyleBackColor = True
        ' 
        ' FENExport
        ' 
        FENExport.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        FENExport.Location = New Point(150, 110)
        FENExport.Name = "FENExport"
        FENExport.Size = New Size(70, 43)
        FENExport.TabIndex = 7
        FENExport.Text = "Export FEN"
        FENExport.UseVisualStyleBackColor = True
        ' 
        ' CheckLabel
        ' 
        CheckLabel.AutoSize = True
        CheckLabel.BackColor = Color.White
        CheckLabel.Font = New Font("Microsoft Sans Serif", 15.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        CheckLabel.Location = New Point(985, 12)
        CheckLabel.Name = "CheckLabel"
        CheckLabel.Size = New Size(126, 25)
        CheckLabel.TabIndex = 8
        CheckLabel.Text = "Checkmate!"
        ' 
        ' CurrentAIEval
        ' 
        CurrentAIEval.AutoSize = True
        CurrentAIEval.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        CurrentAIEval.Location = New Point(955, 463)
        CurrentAIEval.Name = "CurrentAIEval"
        CurrentAIEval.Size = New Size(93, 16)
        CurrentAIEval.TabIndex = 9
        CurrentAIEval.Text = "Evaluation: -"
        ' 
        ' UndoMove
        ' 
        UndoMove.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        UndoMove.Location = New Point(9, 171)
        UndoMove.Name = "UndoMove"
        UndoMove.Size = New Size(282, 43)
        UndoMove.TabIndex = 7
        UndoMove.Text = "Undo Last Action"
        UndoMove.UseVisualStyleBackColor = True
        ' 
        ' Credits
        ' 
        Credits.AutoSize = True
        Credits.BackColor = Color.Transparent
        Credits.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Credits.ForeColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        Credits.Location = New Point(1035, 575)
        Credits.Name = "Credits"
        Credits.Size = New Size(154, 16)
        Credits.TabIndex = 10
        Credits.Text = "Created by Alfie Kunz"
        ' 
        ' UndoFENChange
        ' 
        UndoFENChange.Font = New Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        UndoFENChange.Location = New Point(245, 78)
        UndoFENChange.Name = "UndoFENChange"
        UndoFENChange.Size = New Size(23, 23)
        UndoFENChange.TabIndex = 11
        UndoFENChange.Text = "↺"
        UndoFENChange.UseVisualStyleBackColor = True
        ' 
        ' ProgressBar
        ' 
        ProgressBar.ForeColor = Color.FromArgb(CByte(0), CByte(192), CByte(0))
        ProgressBar.Location = New Point(945, 343)
        ProgressBar.Name = "ProgressBar"
        ProgressBar.Size = New Size(209, 25)
        ProgressBar.Style = ProgressBarStyle.Continuous
        ProgressBar.TabIndex = 13
        ' 
        ' SettingsBtn
        ' 
        SettingsBtn.Font = New Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        SettingsBtn.ImageAlign = ContentAlignment.MiddleRight
        SettingsBtn.Location = New Point(95, 425)
        SettingsBtn.Name = "SettingsBtn"
        SettingsBtn.Size = New Size(113, 40)
        SettingsBtn.TabIndex = 15
        SettingsBtn.Text = "Settings:"
        SettingsBtn.UseVisualStyleBackColor = True
        ' 
        ' BP1
        ' 
        BP1.BackColor = Color.Transparent
        BP1.Image = CType(resources.GetObject("BP1.Image"), Image)
        BP1.InitialImage = Nothing
        BP1.Location = New Point(518, 258)
        BP1.Name = "BP1"
        BP1.Size = New Size(75, 75)
        BP1.TabIndex = 0
        BP1.TabStop = False
        ' 
        ' BR1
        ' 
        BR1.BackColor = Color.Transparent
        BR1.Image = CType(resources.GetObject("BR1.Image"), Image)
        BR1.InitialImage = Nothing
        BR1.Location = New Point(437, 257)
        BR1.Name = "BR1"
        BR1.Size = New Size(75, 75)
        BR1.TabIndex = 0
        BR1.TabStop = False
        ' 
        ' BN1
        ' 
        BN1.BackColor = Color.Transparent
        BN1.Image = CType(resources.GetObject("BN1.Image"), Image)
        BN1.InitialImage = Nothing
        BN1.Location = New Point(356, 258)
        BN1.Name = "BN1"
        BN1.Size = New Size(75, 75)
        BN1.TabIndex = 0
        BN1.TabStop = False
        ' 
        ' BB1
        ' 
        BB1.BackColor = Color.Transparent
        BB1.Image = CType(resources.GetObject("BB1.Image"), Image)
        BB1.InitialImage = Nothing
        BB1.Location = New Point(518, 177)
        BB1.Name = "BB1"
        BB1.Size = New Size(75, 75)
        BB1.TabIndex = 0
        BB1.TabStop = False
        ' 
        ' BQ1
        ' 
        BQ1.BackColor = Color.Transparent
        BQ1.Image = CType(resources.GetObject("BQ1.Image"), Image)
        BQ1.InitialImage = Nothing
        BQ1.Location = New Point(437, 177)
        BQ1.Name = "BQ1"
        BQ1.Size = New Size(75, 75)
        BQ1.TabIndex = 0
        BQ1.TabStop = False
        ' 
        ' BK1
        ' 
        BK1.BackColor = Color.Transparent
        BK1.Image = CType(resources.GetObject("BK1.Image"), Image)
        BK1.InitialImage = Nothing
        BK1.Location = New Point(356, 177)
        BK1.Name = "BK1"
        BK1.Size = New Size(75, 75)
        BK1.TabIndex = 0
        BK1.TabStop = False
        ' 
        ' WP1
        ' 
        WP1.BackColor = Color.Transparent
        WP1.Image = CType(resources.GetObject("WP1.Image"), Image)
        WP1.InitialImage = Nothing
        WP1.Location = New Point(518, 96)
        WP1.Name = "WP1"
        WP1.Size = New Size(75, 75)
        WP1.TabIndex = 0
        WP1.TabStop = False
        ' 
        ' WR1
        ' 
        WR1.BackColor = Color.Transparent
        WR1.Image = CType(resources.GetObject("WR1.Image"), Image)
        WR1.InitialImage = Nothing
        WR1.Location = New Point(437, 96)
        WR1.Name = "WR1"
        WR1.Size = New Size(75, 75)
        WR1.TabIndex = 0
        WR1.TabStop = False
        ' 
        ' WN1
        ' 
        WN1.BackColor = Color.Transparent
        WN1.Image = CType(resources.GetObject("WN1.Image"), Image)
        WN1.InitialImage = Nothing
        WN1.Location = New Point(356, 96)
        WN1.Name = "WN1"
        WN1.Size = New Size(75, 75)
        WN1.TabIndex = 0
        WN1.TabStop = False
        ' 
        ' WB1
        ' 
        WB1.BackColor = Color.Transparent
        WB1.Image = CType(resources.GetObject("WB1.Image"), Image)
        WB1.InitialImage = Nothing
        WB1.Location = New Point(518, 15)
        WB1.Name = "WB1"
        WB1.Size = New Size(75, 75)
        WB1.TabIndex = 0
        WB1.TabStop = False
        ' 
        ' WQ1
        ' 
        WQ1.BackColor = Color.Transparent
        WQ1.Image = CType(resources.GetObject("WQ1.Image"), Image)
        WQ1.InitialImage = Nothing
        WQ1.Location = New Point(437, 15)
        WQ1.Name = "WQ1"
        WQ1.Size = New Size(75, 75)
        WQ1.TabIndex = 0
        WQ1.TabStop = False
        ' 
        ' WK1
        ' 
        WK1.BackColor = Color.Transparent
        WK1.Image = CType(resources.GetObject("WK1.Image"), Image)
        WK1.InitialImage = Nothing
        WK1.Location = New Point(356, 15)
        WK1.Name = "WK1"
        WK1.Size = New Size(75, 75)
        WK1.TabIndex = 0
        WK1.TabStop = False
        ' 
        ' Checkerboard
        ' 
        Checkerboard.BackColor = Color.Transparent
        Checkerboard.Location = New Point(300, 0)
        Checkerboard.Name = "Checkerboard"
        Checkerboard.Size = New Size(600, 600)
        Checkerboard.TabIndex = 1
        Checkerboard.TabStop = False
        ' 
        ' FlipperButton
        ' 
        FlipperButton.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        FlipperButton.Location = New Point(34, 520)
        FlipperButton.Name = "FlipperButton"
        FlipperButton.Size = New Size(232, 43)
        FlipperButton.TabIndex = 18
        FlipperButton.Text = "Flip Board"
        FlipperButton.UseVisualStyleBackColor = True
        ' 
        ' AutoFlipper
        ' 
        AutoFlipper.AutoSize = True
        AutoFlipper.Location = New Point(39, 569)
        AutoFlipper.Name = "AutoFlipper"
        AutoFlipper.Size = New Size(221, 17)
        AutoFlipper.TabIndex = 19
        AutoFlipper.Text = "Flip Board Automatically After Each Move"
        AutoFlipper.UseVisualStyleBackColor = True
        ' 
        ' ExitBtn
        ' 
        ExitBtn.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        ExitBtn.Location = New Point(1009, 515)
        ExitBtn.Name = "ExitBtn"
        ExitBtn.Size = New Size(78, 40)
        ExitBtn.TabIndex = 22
        ExitBtn.Text = "Exit to Main Menu"
        ExitBtn.UseVisualStyleBackColor = True
        ' 
        ' UserTimeBox
        ' 
        UserTimeBox.BackColor = Color.WhiteSmoke
        UserTimeBox.Cursor = Cursors.Hand
        UserTimeBox.Location = New Point(962, 122)
        UserTimeBox.Name = "UserTimeBox"
        UserTimeBox.ReadOnly = True
        UserTimeBox.Size = New Size(164, 20)
        UserTimeBox.TabIndex = 23
        UserTimeBox.Text = "Time For Search: 10 Seconds"
        UserTimeBox.TextAlign = HorizontalAlignment.Center
        ' 
        ' UserTimeBar
        ' 
        UserTimeBar.LargeChange = 10
        UserTimeBar.Location = New Point(962, 146)
        UserTimeBar.Maximum = 60
        UserTimeBar.Name = "UserTimeBar"
        UserTimeBar.Size = New Size(164, 45)
        UserTimeBar.TabIndex = 25
        UserTimeBar.Value = 30
        ' 
        ' CurrentAIMove
        ' 
        CurrentAIMove.AutoSize = True
        CurrentAIMove.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        CurrentAIMove.Location = New Point(955, 433)
        CurrentAIMove.Name = "CurrentAIMove"
        CurrentAIMove.Size = New Size(111, 16)
        CurrentAIMove.TabIndex = 9
        CurrentAIMove.Text = "Current Move: -"
        ' 
        ' CurrentAIDepth
        ' 
        CurrentAIDepth.AutoSize = True
        CurrentAIDepth.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        CurrentAIDepth.Location = New Point(955, 403)
        CurrentAIDepth.Name = "CurrentAIDepth"
        CurrentAIDepth.Size = New Size(114, 16)
        CurrentAIDepth.TabIndex = 9
        CurrentAIDepth.Text = "Current Depth: -"
        ' 
        ' AITerminator
        ' 
        AITerminator.Font = New Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AITerminator.Location = New Point(945, 278)
        AITerminator.Name = "AITerminator"
        AITerminator.Size = New Size(209, 57)
        AITerminator.TabIndex = 27
        AITerminator.Text = "Terminate Search Early:"
        AITerminator.UseVisualStyleBackColor = True
        AITerminator.Visible = False
        ' 
        ' AIEndlessMode
        ' 
        AIEndlessMode.AutoSize = True
        AIEndlessMode.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AIEndlessMode.Location = New Point(1011, 213)
        AIEndlessMode.Name = "AIEndlessMode"
        AIEndlessMode.Size = New Size(63, 19)
        AIEndlessMode.TabIndex = 26
        AIEndlessMode.Text = "AI vs AI"
        AIEndlessMode.TextAlign = ContentAlignment.MiddleCenter
        AIEndlessMode.UseVisualStyleBackColor = True
        ' 
        ' TrainingStart
        ' 
        TrainingStart.Font = New Font("Microsoft Sans Serif", 14.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        TrainingStart.Location = New Point(9, 12)
        TrainingStart.Name = "TrainingStart"
        TrainingStart.Size = New Size(282, 43)
        TrainingStart.TabIndex = 7
        TrainingStart.Text = "Start!"
        TrainingStart.UseVisualStyleBackColor = True
        TrainingStart.Visible = False
        ' 
        ' TimerLabel
        ' 
        TimerLabel.AutoSize = True
        TimerLabel.Font = New Font("Microsoft Sans Serif", 14.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        TimerLabel.Location = New Point(31, 70)
        TimerLabel.Name = "TimerLabel"
        TimerLabel.Size = New Size(235, 24)
        TimerLabel.TabIndex = 9
        TimerLabel.Text = "Time Left: 20.0 Seconds"
        TimerLabel.Visible = False
        ' 
        ' MoveDisplayer
        ' 
        MoveDisplayer.Font = New Font("Microsoft Sans Serif", 72F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        MoveDisplayer.Location = New Point(9, 203)
        MoveDisplayer.Name = "MoveDisplayer"
        MoveDisplayer.Size = New Size(284, 152)
        MoveDisplayer.TabIndex = 9
        MoveDisplayer.Text = "h8"
        MoveDisplayer.TextAlign = ContentAlignment.MiddleCenter
        MoveDisplayer.Visible = False
        ' 
        ' WLeaderBoardGrid
        ' 
        WLeaderBoardGrid.AllowUserToAddRows = False
        WLeaderBoardGrid.AllowUserToDeleteRows = False
        WLeaderBoardGrid.AllowUserToResizeColumns = False
        WLeaderBoardGrid.AllowUserToResizeRows = False
        WLeaderBoardGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        WLeaderBoardGrid.Columns.AddRange(New DataGridViewColumn() {UserIndex, Username, UserScore, UserDate})
        WLeaderBoardGrid.Enabled = False
        WLeaderBoardGrid.Location = New Point(9, 155)
        WLeaderBoardGrid.MultiSelect = False
        WLeaderBoardGrid.Name = "WLeaderBoardGrid"
        WLeaderBoardGrid.ReadOnly = True
        WLeaderBoardGrid.RowHeadersVisible = False
        WLeaderBoardGrid.RowHeadersWidth = 21
        WLeaderBoardGrid.ScrollBars = ScrollBars.None
        WLeaderBoardGrid.ShowEditingIcon = False
        WLeaderBoardGrid.Size = New Size(282, 240)
        WLeaderBoardGrid.TabIndex = 28
        WLeaderBoardGrid.TabStop = False
        WLeaderBoardGrid.Visible = False
        ' 
        ' UserIndex
        ' 
        UserIndex.Frozen = True
        UserIndex.HeaderText = "W"
        UserIndex.MinimumWidth = 6
        UserIndex.Name = "UserIndex"
        UserIndex.ReadOnly = True
        UserIndex.Width = 21
        ' 
        ' Username
        ' 
        Username.Frozen = True
        Username.HeaderText = "Name"
        Username.MinimumWidth = 6
        Username.Name = "Username"
        Username.ReadOnly = True
        Username.Width = 130
        ' 
        ' UserScore
        ' 
        UserScore.Frozen = True
        UserScore.HeaderText = "Score"
        UserScore.MinimumWidth = 6
        UserScore.Name = "UserScore"
        UserScore.ReadOnly = True
        UserScore.Width = 50
        ' 
        ' UserDate
        ' 
        UserDate.Frozen = True
        UserDate.HeaderText = "Date"
        UserDate.MinimumWidth = 6
        UserDate.Name = "UserDate"
        UserDate.ReadOnly = True
        UserDate.Width = 80
        ' 
        ' BLeaderBoardGrid
        ' 
        BLeaderBoardGrid.AllowUserToAddRows = False
        BLeaderBoardGrid.AllowUserToDeleteRows = False
        BLeaderBoardGrid.AllowUserToResizeColumns = False
        BLeaderBoardGrid.AllowUserToResizeRows = False
        BLeaderBoardGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        BLeaderBoardGrid.Columns.AddRange(New DataGridViewColumn() {DataGridViewTextBoxColumn1, DataGridViewTextBoxColumn2, DataGridViewTextBoxColumn3, DataGridViewTextBoxColumn4})
        BLeaderBoardGrid.Enabled = False
        BLeaderBoardGrid.Location = New Point(9, 155)
        BLeaderBoardGrid.MultiSelect = False
        BLeaderBoardGrid.Name = "BLeaderBoardGrid"
        BLeaderBoardGrid.ReadOnly = True
        BLeaderBoardGrid.RowHeadersVisible = False
        BLeaderBoardGrid.RowHeadersWidth = 21
        BLeaderBoardGrid.ScrollBars = ScrollBars.None
        BLeaderBoardGrid.ShowEditingIcon = False
        BLeaderBoardGrid.Size = New Size(282, 240)
        BLeaderBoardGrid.TabIndex = 29
        BLeaderBoardGrid.TabStop = False
        BLeaderBoardGrid.Visible = False
        ' 
        ' DataGridViewTextBoxColumn1
        ' 
        DataGridViewTextBoxColumn1.Frozen = True
        DataGridViewTextBoxColumn1.HeaderText = "B"
        DataGridViewTextBoxColumn1.MinimumWidth = 6
        DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        DataGridViewTextBoxColumn1.ReadOnly = True
        DataGridViewTextBoxColumn1.Width = 21
        ' 
        ' DataGridViewTextBoxColumn2
        ' 
        DataGridViewTextBoxColumn2.Frozen = True
        DataGridViewTextBoxColumn2.HeaderText = "Name"
        DataGridViewTextBoxColumn2.MinimumWidth = 6
        DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        DataGridViewTextBoxColumn2.ReadOnly = True
        DataGridViewTextBoxColumn2.Width = 130
        ' 
        ' DataGridViewTextBoxColumn3
        ' 
        DataGridViewTextBoxColumn3.Frozen = True
        DataGridViewTextBoxColumn3.HeaderText = "Score"
        DataGridViewTextBoxColumn3.MinimumWidth = 6
        DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
        DataGridViewTextBoxColumn3.ReadOnly = True
        DataGridViewTextBoxColumn3.Width = 50
        ' 
        ' DataGridViewTextBoxColumn4
        ' 
        DataGridViewTextBoxColumn4.Frozen = True
        DataGridViewTextBoxColumn4.HeaderText = "Date"
        DataGridViewTextBoxColumn4.MinimumWidth = 6
        DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4"
        DataGridViewTextBoxColumn4.ReadOnly = True
        DataGridViewTextBoxColumn4.Width = 80
        ' 
        ' TrainingScore
        ' 
        TrainingScore.BackColor = Color.Transparent
        TrainingScore.Font = New Font("Microsoft Sans Serif", 27.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        TrainingScore.Location = New Point(114, 330)
        TrainingScore.Name = "TrainingScore"
        TrainingScore.Size = New Size(74, 52)
        TrainingScore.TabIndex = 30
        TrainingScore.Text = "0"
        TrainingScore.TextAlign = ContentAlignment.MiddleCenter
        TrainingScore.Visible = False
        ' 
        ' InfoBtn
        ' 
        InfoBtn.Font = New Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        InfoBtn.Location = New Point(7, 570)
        InfoBtn.Name = "InfoBtn"
        InfoBtn.Size = New Size(18, 23)
        InfoBtn.TabIndex = 31
        InfoBtn.Text = "?"
        InfoBtn.TextAlign = ContentAlignment.MiddleLeft
        InfoBtn.UseVisualStyleBackColor = True
        InfoBtn.Visible = False
        ' 
        ' UseBook
        ' 
        UseBook.AutoSize = True
        UseBook.Enabled = False
        UseBook.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        UseBook.Location = New Point(977, 188)
        UseBook.Name = "UseBook"
        UseBook.Size = New Size(136, 19)
        UseBook.TabIndex = 26
        UseBook.Text = "Use Opening Book?"
        UseBook.TextAlign = ContentAlignment.MiddleCenter
        UseBook.UseVisualStyleBackColor = True
        ' 
        ' NextPuzzleBtn
        ' 
        NextPuzzleBtn.Font = New Font("Microsoft Sans Serif", 14.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        NextPuzzleBtn.Location = New Point(9, 124)
        NextPuzzleBtn.Name = "NextPuzzleBtn"
        NextPuzzleBtn.Size = New Size(282, 43)
        NextPuzzleBtn.TabIndex = 7
        NextPuzzleBtn.Text = "Next Puzzle"
        NextPuzzleBtn.UseVisualStyleBackColor = True
        NextPuzzleBtn.Visible = False
        ' 
        ' HintBtn
        ' 
        HintBtn.Font = New Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        HintBtn.Location = New Point(9, 75)
        HintBtn.Name = "HintBtn"
        HintBtn.Size = New Size(133, 43)
        HintBtn.TabIndex = 5
        HintBtn.Text = "Hint"
        HintBtn.UseVisualStyleBackColor = True
        HintBtn.Visible = False
        ' 
        ' GiveUpBtn
        ' 
        GiveUpBtn.Font = New Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        GiveUpBtn.Location = New Point(150, 75)
        GiveUpBtn.Name = "GiveUpBtn"
        GiveUpBtn.Size = New Size(141, 43)
        GiveUpBtn.TabIndex = 7
        GiveUpBtn.Text = "Give Up"
        GiveUpBtn.UseVisualStyleBackColor = True
        GiveUpBtn.Visible = False
        ' 
        ' AIPuzzleInfoLabel
        ' 
        AIPuzzleInfoLabel.AutoSize = True
        AIPuzzleInfoLabel.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AIPuzzleInfoLabel.Location = New Point(35, 332)
        AIPuzzleInfoLabel.Name = "AIPuzzleInfoLabel"
        AIPuzzleInfoLabel.Size = New Size(228, 20)
        AIPuzzleInfoLabel.TabIndex = 9
        AIPuzzleInfoLabel.Text = "AI is Searching on the Puzzle..."
        AIPuzzleInfoLabel.TextAlign = ContentAlignment.MiddleCenter
        AIPuzzleInfoLabel.Visible = False
        ' 
        ' RatingLabel
        ' 
        RatingLabel.Font = New Font("Microsoft Sans Serif", 36F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        RatingLabel.Location = New Point(79, 249)
        RatingLabel.Name = "RatingLabel"
        RatingLabel.Size = New Size(136, 55)
        RatingLabel.TabIndex = 9
        RatingLabel.Text = "1000"
        RatingLabel.TextAlign = ContentAlignment.MiddleCenter
        RatingLabel.Visible = False
        ' 
        ' LostRatingLabel
        ' 
        LostRatingLabel.AutoSize = True
        LostRatingLabel.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        LostRatingLabel.Location = New Point(71, 296)
        LostRatingLabel.Name = "LostRatingLabel"
        LostRatingLabel.Size = New Size(32, 20)
        LostRatingLabel.TabIndex = 9
        LostRatingLabel.Text = "-10"
        LostRatingLabel.TextAlign = ContentAlignment.MiddleCenter
        LostRatingLabel.Visible = False
        ' 
        ' GainedRatingLabel
        ' 
        GainedRatingLabel.AutoSize = True
        GainedRatingLabel.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        GainedRatingLabel.Location = New Point(188, 296)
        GainedRatingLabel.Name = "GainedRatingLabel"
        GainedRatingLabel.Size = New Size(36, 20)
        GainedRatingLabel.TabIndex = 9
        GainedRatingLabel.Text = "+10"
        GainedRatingLabel.TextAlign = ContentAlignment.MiddleCenter
        GainedRatingLabel.Visible = False
        ' 
        ' RatingHeader
        ' 
        RatingHeader.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        RatingHeader.Location = New Point(100, 225)
        RatingHeader.Name = "RatingHeader"
        RatingHeader.Size = New Size(98, 20)
        RatingHeader.TabIndex = 9
        RatingHeader.Text = "Your Rating:"
        RatingHeader.TextAlign = ContentAlignment.MiddleCenter
        RatingHeader.Visible = False
        ' 
        ' PuzzleRatingLabel
        ' 
        PuzzleRatingLabel.AutoSize = True
        PuzzleRatingLabel.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        PuzzleRatingLabel.Location = New Point(86, 51)
        PuzzleRatingLabel.Name = "PuzzleRatingLabel"
        PuzzleRatingLabel.Size = New Size(122, 16)
        PuzzleRatingLabel.TabIndex = 9
        PuzzleRatingLabel.Text = "Puzzle Rating: 1000"
        PuzzleRatingLabel.TextAlign = ContentAlignment.MiddleCenter
        PuzzleRatingLabel.Visible = False
        ' 
        ' AutoAdvanceBox
        ' 
        AutoAdvanceBox.BackColor = Color.WhiteSmoke
        AutoAdvanceBox.Cursor = Cursors.Hand
        AutoAdvanceBox.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AutoAdvanceBox.Location = New Point(167, 438)
        AutoAdvanceBox.Name = "AutoAdvanceBox"
        AutoAdvanceBox.ReadOnly = True
        AutoAdvanceBox.Size = New Size(120, 22)
        AutoAdvanceBox.TabIndex = 23
        AutoAdvanceBox.Text = "Never"
        AutoAdvanceBox.TextAlign = HorizontalAlignment.Center
        AutoAdvanceBox.Visible = False
        ' 
        ' AutoAdvanceBar
        ' 
        AutoAdvanceBar.LargeChange = 2
        AutoAdvanceBar.Location = New Point(167, 464)
        AutoAdvanceBar.Maximum = 14
        AutoAdvanceBar.Name = "AutoAdvanceBar"
        AutoAdvanceBar.Size = New Size(120, 45)
        AutoAdvanceBar.TabIndex = 25
        AutoAdvanceBar.Value = 14
        AutoAdvanceBar.Visible = False
        ' 
        ' AutoAdvanceOnComplete
        ' 
        AutoAdvanceOnComplete.AutoSize = True
        AutoAdvanceOnComplete.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AutoAdvanceOnComplete.Location = New Point(8, 413)
        AutoAdvanceOnComplete.Name = "AutoAdvanceOnComplete"
        AutoAdvanceOnComplete.Size = New Size(280, 19)
        AutoAdvanceOnComplete.TabIndex = 26
        AutoAdvanceOnComplete.Text = "Automatically Advance When Correct / Incorrect"
        AutoAdvanceOnComplete.TextAlign = ContentAlignment.MiddleCenter
        AutoAdvanceOnComplete.UseVisualStyleBackColor = True
        AutoAdvanceOnComplete.Visible = False
        ' 
        ' AutoAdvanceLabel
        ' 
        AutoAdvanceLabel.AutoSize = True
        AutoAdvanceLabel.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AutoAdvanceLabel.Location = New Point(5, 438)
        AutoAdvanceLabel.Name = "AutoAdvanceLabel"
        AutoAdvanceLabel.Size = New Size(158, 15)
        AutoAdvanceLabel.TabIndex = 9
        AutoAdvanceLabel.Text = "Automatically Advance After:"
        AutoAdvanceLabel.TextAlign = ContentAlignment.MiddleCenter
        AutoAdvanceLabel.Visible = False
        ' 
        ' HumanModeBtn
        ' 
        HumanModeBtn.AutoSize = True
        HumanModeBtn.Checked = True
        HumanModeBtn.Location = New Point(22, 390)
        HumanModeBtn.Name = "HumanModeBtn"
        HumanModeBtn.Size = New Size(123, 17)
        HumanModeBtn.TabIndex = 32
        HumanModeBtn.TabStop = True
        HumanModeBtn.Text = "Human Puzzle Mode"
        HumanModeBtn.UseVisualStyleBackColor = True
        HumanModeBtn.Visible = False
        ' 
        ' AIModeBtn
        ' 
        AIModeBtn.AutoSize = True
        AIModeBtn.Location = New Point(166, 390)
        AIModeBtn.Name = "AIModeBtn"
        AIModeBtn.Size = New Size(99, 17)
        AIModeBtn.TabIndex = 33
        AIModeBtn.Text = "AI Puzzle Mode"
        AIModeBtn.UseVisualStyleBackColor = True
        AIModeBtn.Visible = False
        ' 
        ' ResetRatingsBtn
        ' 
        ResetRatingsBtn.Location = New Point(247, 565)
        ResetRatingsBtn.Name = "ResetRatingsBtn"
        ResetRatingsBtn.Size = New Size(52, 34)
        ResetRatingsBtn.TabIndex = 34
        ResetRatingsBtn.Text = "Reset Ratings"
        ResetRatingsBtn.TextAlign = ContentAlignment.TopCenter
        ResetRatingsBtn.UseVisualStyleBackColor = True
        ResetRatingsBtn.Visible = False
        ' 
        ' PGNExport
        ' 
        PGNExport.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        PGNExport.Location = New Point(221, 110)
        PGNExport.Name = "PGNExport"
        PGNExport.Size = New Size(70, 43)
        PGNExport.TabIndex = 7
        PGNExport.Text = "Export PGN"
        PGNExport.UseVisualStyleBackColor = True
        ' 
        ' AutoOutputterPanel
        ' 
        AutoOutputterPanel.Controls.Add(PGNAutoOutputter)
        AutoOutputterPanel.Controls.Add(FENAutoOutputter)
        AutoOutputterPanel.Location = New Point(45, 76)
        AutoOutputterPanel.Name = "AutoOutputterPanel"
        AutoOutputterPanel.Size = New Size(214, 24)
        AutoOutputterPanel.TabIndex = 35
        AutoOutputterPanel.Visible = False
        ' 
        ' PGNAutoOutputter
        ' 
        PGNAutoOutputter.AutoSize = True
        PGNAutoOutputter.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        PGNAutoOutputter.Location = New Point(124, 0)
        PGNAutoOutputter.Name = "PGNAutoOutputter"
        PGNAutoOutputter.Size = New Size(90, 19)
        PGNAutoOutputter.TabIndex = 1
        PGNAutoOutputter.Text = "Output PGN"
        PGNAutoOutputter.UseVisualStyleBackColor = True
        ' 
        ' FENAutoOutputter
        ' 
        FENAutoOutputter.AutoSize = True
        FENAutoOutputter.Checked = True
        FENAutoOutputter.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        FENAutoOutputter.Location = New Point(3, 0)
        FENAutoOutputter.Name = "FENAutoOutputter"
        FENAutoOutputter.Size = New Size(88, 19)
        FENAutoOutputter.TabIndex = 0
        FENAutoOutputter.TabStop = True
        FENAutoOutputter.Text = "Output FEN"
        FENAutoOutputter.UseVisualStyleBackColor = True
        ' 
        ' NodeTestBtn
        ' 
        NodeTestBtn.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        NodeTestBtn.Location = New Point(62, 338)
        NodeTestBtn.Name = "NodeTestBtn"
        NodeTestBtn.Size = New Size(175, 49)
        NodeTestBtn.TabIndex = 27
        NodeTestBtn.Text = "Perform Node Test:"
        NodeTestBtn.UseVisualStyleBackColor = True
        ' 
        ' NodeTestStopBtn
        ' 
        NodeTestStopBtn.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        NodeTestStopBtn.Location = New Point(62, 338)
        NodeTestStopBtn.Name = "NodeTestStopBtn"
        NodeTestStopBtn.Size = New Size(175, 49)
        NodeTestStopBtn.TabIndex = 27
        NodeTestStopBtn.Text = "Terminate Node Test:"
        NodeTestStopBtn.UseVisualStyleBackColor = True
        NodeTestStopBtn.Visible = False
        ' 
        ' AutoResetter
        ' 
        AutoResetter.AutoSize = True
        AutoResetter.Font = New Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AutoResetter.ForeColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        AutoResetter.Location = New Point(970, 232)
        AutoResetter.Name = "AutoResetter"
        AutoResetter.Size = New Size(150, 17)
        AutoResetter.TabIndex = 36
        AutoResetter.Text = "Automatically Reset Game"
        AutoResetter.UseVisualStyleBackColor = True
        AutoResetter.Visible = False
        ' 
        ' BoardEditorBtn
        ' 
        BoardEditorBtn.Font = New Font("Microsoft Sans Serif", 10.8F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        BoardEditorBtn.Location = New Point(53, 238)
        BoardEditorBtn.Name = "BoardEditorBtn"
        BoardEditorBtn.Size = New Size(194, 43)
        BoardEditorBtn.TabIndex = 18
        BoardEditorBtn.Text = "Board Editor Mode:"
        BoardEditorBtn.UseVisualStyleBackColor = True
        ' 
        ' BoardEditCancelBtn
        ' 
        BoardEditCancelBtn.Font = New Font("Microsoft Sans Serif", 10.8F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        BoardEditCancelBtn.Location = New Point(158, 238)
        BoardEditCancelBtn.Name = "BoardEditCancelBtn"
        BoardEditCancelBtn.Size = New Size(90, 43)
        BoardEditCancelBtn.TabIndex = 18
        BoardEditCancelBtn.Text = "Cancel Changes:"
        BoardEditCancelBtn.UseVisualStyleBackColor = True
        BoardEditCancelBtn.Visible = False
        ' 
        ' BoardEditBlackMove
        ' 
        BoardEditBlackMove.AutoSize = True
        BoardEditBlackMove.Font = New Font("Microsoft Sans Serif", 10.2F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        BoardEditBlackMove.Location = New Point(150, 2)
        BoardEditBlackMove.Name = "BoardEditBlackMove"
        BoardEditBlackMove.Size = New Size(114, 21)
        BoardEditBlackMove.TabIndex = 1
        BoardEditBlackMove.Text = "Black to Move"
        BoardEditBlackMove.UseVisualStyleBackColor = True
        ' 
        ' BoardEditWhiteMove
        ' 
        BoardEditWhiteMove.AutoSize = True
        BoardEditWhiteMove.Checked = True
        BoardEditWhiteMove.Font = New Font("Microsoft Sans Serif", 10.2F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        BoardEditWhiteMove.Location = New Point(4, 2)
        BoardEditWhiteMove.Name = "BoardEditWhiteMove"
        BoardEditWhiteMove.Size = New Size(116, 21)
        BoardEditWhiteMove.TabIndex = 0
        BoardEditWhiteMove.TabStop = True
        BoardEditWhiteMove.Text = "White to Move"
        BoardEditWhiteMove.TextAlign = ContentAlignment.MiddleCenter
        BoardEditWhiteMove.UseVisualStyleBackColor = True
        ' 
        ' BoardEditPanel
        ' 
        BoardEditPanel.Controls.Add(BoardEditTipLabel)
        BoardEditPanel.Controls.Add(BoardEditEnPassantLabel)
        BoardEditPanel.Controls.Add(BoardEditBLabel)
        BoardEditPanel.Controls.Add(BoardEditWLabel)
        BoardEditPanel.Controls.Add(BoardEditBQSBox)
        BoardEditPanel.Controls.Add(BoardEditWQSBox)
        BoardEditPanel.Controls.Add(BoardEditBKSBox)
        BoardEditPanel.Controls.Add(BoardEditWKSBox)
        BoardEditPanel.Controls.Add(BoardEditBlackMove)
        BoardEditPanel.Controls.Add(BoardEditWhiteMove)
        BoardEditPanel.Location = New Point(906, 7)
        BoardEditPanel.Name = "BoardEditPanel"
        BoardEditPanel.Size = New Size(272, 135)
        BoardEditPanel.TabIndex = 36
        BoardEditPanel.Visible = False
        ' 
        ' BoardEditTipLabel
        ' 
        BoardEditTipLabel.Location = New Point(-5, 112)
        BoardEditTipLabel.Margin = New Padding(2, 0, 2, 0)
        BoardEditTipLabel.Name = "BoardEditTipLabel"
        BoardEditTipLabel.Size = New Size(271, 19)
        BoardEditTipLabel.TabIndex = 4
        BoardEditTipLabel.Text = "Tip: Hold CTRL when Moving a Piece to Duplicate it."
        BoardEditTipLabel.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' BoardEditEnPassantLabel
        ' 
        BoardEditEnPassantLabel.Location = New Point(0, 92)
        BoardEditEnPassantLabel.Margin = New Padding(2, 0, 2, 0)
        BoardEditEnPassantLabel.Name = "BoardEditEnPassantLabel"
        BoardEditEnPassantLabel.Size = New Size(263, 19)
        BoardEditEnPassantLabel.TabIndex = 4
        BoardEditEnPassantLabel.Text = "Right-Click on a Square to Flag it as En-Passant."
        BoardEditEnPassantLabel.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' BoardEditBLabel
        ' 
        BoardEditBLabel.AutoSize = True
        BoardEditBLabel.Location = New Point(148, 29)
        BoardEditBLabel.Margin = New Padding(2, 0, 2, 0)
        BoardEditBLabel.Name = "BoardEditBLabel"
        BoardEditBLabel.Size = New Size(107, 13)
        BoardEditBLabel.TabIndex = 3
        BoardEditBLabel.Text = "Black Castling Rules:"
        ' 
        ' BoardEditWLabel
        ' 
        BoardEditWLabel.AutoSize = True
        BoardEditWLabel.Location = New Point(2, 29)
        BoardEditWLabel.Margin = New Padding(2, 0, 2, 0)
        BoardEditWLabel.Name = "BoardEditWLabel"
        BoardEditWLabel.Size = New Size(108, 13)
        BoardEditWLabel.TabIndex = 3
        BoardEditWLabel.Text = "White Castling Rules:"
        ' 
        ' BoardEditBQSBox
        ' 
        BoardEditBQSBox.AutoSize = True
        BoardEditBQSBox.Location = New Point(150, 68)
        BoardEditBQSBox.Margin = New Padding(2)
        BoardEditBQSBox.Name = "BoardEditBQSBox"
        BoardEditBQSBox.Size = New Size(82, 17)
        BoardEditBQSBox.TabIndex = 2
        BoardEditBQSBox.Text = "Queen Side"
        BoardEditBQSBox.UseVisualStyleBackColor = True
        ' 
        ' BoardEditWQSBox
        ' 
        BoardEditWQSBox.AutoSize = True
        BoardEditWQSBox.Location = New Point(4, 68)
        BoardEditWQSBox.Margin = New Padding(2)
        BoardEditWQSBox.Name = "BoardEditWQSBox"
        BoardEditWQSBox.Size = New Size(82, 17)
        BoardEditWQSBox.TabIndex = 2
        BoardEditWQSBox.Text = "Queen Side"
        BoardEditWQSBox.UseVisualStyleBackColor = True
        ' 
        ' BoardEditBKSBox
        ' 
        BoardEditBKSBox.AutoSize = True
        BoardEditBKSBox.Location = New Point(150, 47)
        BoardEditBKSBox.Margin = New Padding(2)
        BoardEditBKSBox.Name = "BoardEditBKSBox"
        BoardEditBKSBox.Size = New Size(71, 17)
        BoardEditBKSBox.TabIndex = 2
        BoardEditBKSBox.Text = "King Side"
        BoardEditBKSBox.UseVisualStyleBackColor = True
        ' 
        ' BoardEditWKSBox
        ' 
        BoardEditWKSBox.AutoSize = True
        BoardEditWKSBox.Location = New Point(4, 47)
        BoardEditWKSBox.Margin = New Padding(2)
        BoardEditWKSBox.Name = "BoardEditWKSBox"
        BoardEditWKSBox.Size = New Size(71, 17)
        BoardEditWKSBox.TabIndex = 2
        BoardEditWKSBox.Text = "King Side"
        BoardEditWKSBox.UseVisualStyleBackColor = True
        ' 
        ' RemoteModeBtn
        ' 
        RemoteModeBtn.Enabled = False
        RemoteModeBtn.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        RemoteModeBtn.Location = New Point(62, 174)
        RemoteModeBtn.Name = "RemoteModeBtn"
        RemoteModeBtn.Size = New Size(175, 49)
        RemoteModeBtn.TabIndex = 27
        RemoteModeBtn.Text = "Connect To Interface"
        RemoteModeBtn.UseVisualStyleBackColor = True
        RemoteModeBtn.Visible = False
        ' 
        ' BoardEditDiscardBtn
        ' 
        BoardEditDiscardBtn.FlatStyle = FlatStyle.Flat
        BoardEditDiscardBtn.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        BoardEditDiscardBtn.Location = New Point(137, 289)
        BoardEditDiscardBtn.Name = "BoardEditDiscardBtn"
        BoardEditDiscardBtn.Size = New Size(26, 26)
        BoardEditDiscardBtn.TabIndex = 37
        BoardEditDiscardBtn.Text = "X"
        BoardEditDiscardBtn.UseVisualStyleBackColor = True
        BoardEditDiscardBtn.Visible = False
        ' 
        ' AISettingsPanel
        ' 
        AISettingsPanel.BackColor = Color.White
        AISettingsPanel.Controls.Add(AISettingResetBtn)
        AISettingsPanel.Location = New Point(954, 93)
        AISettingsPanel.Name = "AISettingsPanel"
        AISettingsPanel.Size = New Size(180, 38)
        AISettingsPanel.TabIndex = 38
        AISettingsPanel.Visible = False
        ' 
        ' AISettingResetBtn
        ' 
        AISettingResetBtn.Location = New Point(9, 4)
        AISettingResetBtn.Name = "AISettingResetBtn"
        AISettingResetBtn.Size = New Size(164, 23)
        AISettingResetBtn.TabIndex = 0
        AISettingResetBtn.Text = "Reset AI Settings"
        AISettingResetBtn.UseVisualStyleBackColor = True
        ' 
        ' AISettingsBox
        ' 
        AISettingsBox.DropDownHeight = 1
        AISettingsBox.DropDownStyle = ComboBoxStyle.DropDownList
        AISettingsBox.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        AISettingsBox.FormattingEnabled = True
        AISettingsBox.IntegralHeight = False
        AISettingsBox.Items.AddRange(New Object() {"Modify AI Settings:"})
        AISettingsBox.Location = New Point(954, 71)
        AISettingsBox.MaxDropDownItems = 1
        AISettingsBox.Name = "AISettingsBox"
        AISettingsBox.Size = New Size(180, 23)
        AISettingsBox.TabIndex = 39
        AISettingsBox.TabStop = False
        ' 
        ' Chess
        ' 
        AutoScaleDimensions = New SizeF(6F, 13F)
        AutoScaleMode = AutoScaleMode.Font
        AutoSizeMode = AutoSizeMode.GrowAndShrink
        BackColor = Color.FromArgb(CByte(155), CByte(155), CByte(155))
        ClientSize = New Size(1184, 600)
        Controls.Add(AISettingsBox)
        Controls.Add(AISettingsPanel)
        Controls.Add(BoardEditDiscardBtn)
        Controls.Add(BoardEditPanel)
        Controls.Add(AutoResetter)
        Controls.Add(AutoOutputterPanel)
        Controls.Add(ResetRatingsBtn)
        Controls.Add(AIModeBtn)
        Controls.Add(HumanModeBtn)
        Controls.Add(InfoBtn)
        Controls.Add(TrainingScore)
        Controls.Add(RemoteModeBtn)
        Controls.Add(NodeTestStopBtn)
        Controls.Add(NodeTestBtn)
        Controls.Add(AITerminator)
        Controls.Add(UseBook)
        Controls.Add(AIEndlessMode)
        Controls.Add(AutoAdvanceOnComplete)
        Controls.Add(AutoAdvanceBar)
        Controls.Add(UserTimeBar)
        Controls.Add(AutoAdvanceBox)
        Controls.Add(UserTimeBox)
        Controls.Add(ExitBtn)
        Controls.Add(AutoFlipper)
        Controls.Add(BoardEditCancelBtn)
        Controls.Add(BoardEditorBtn)
        Controls.Add(FlipperButton)
        Controls.Add(SettingsBtn)
        Controls.Add(ProgressBar)
        Controls.Add(UndoFENChange)
        Controls.Add(Credits)
        Controls.Add(GainedRatingLabel)
        Controls.Add(LostRatingLabel)
        Controls.Add(AutoAdvanceLabel)
        Controls.Add(RatingHeader)
        Controls.Add(PuzzleRatingLabel)
        Controls.Add(AIPuzzleInfoLabel)
        Controls.Add(RatingLabel)
        Controls.Add(TimerLabel)
        Controls.Add(CurrentAIDepth)
        Controls.Add(CurrentAIMove)
        Controls.Add(CurrentAIEval)
        Controls.Add(CheckLabel)
        Controls.Add(NextPuzzleBtn)
        Controls.Add(TrainingStart)
        Controls.Add(UndoMove)
        Controls.Add(GiveUpBtn)
        Controls.Add(PGNExport)
        Controls.Add(FENExport)
        Controls.Add(HintBtn)
        Controls.Add(Reset_Btn)
        Controls.Add(AIMoveBtn)
        Controls.Add(InputButton)
        Controls.Add(InputTextBox)
        Controls.Add(BP1)
        Controls.Add(BR1)
        Controls.Add(BN1)
        Controls.Add(BB1)
        Controls.Add(BQ1)
        Controls.Add(BK1)
        Controls.Add(WP1)
        Controls.Add(WR1)
        Controls.Add(WN1)
        Controls.Add(WB1)
        Controls.Add(WQ1)
        Controls.Add(WK1)
        Controls.Add(Checkerboard)
        Controls.Add(BLeaderBoardGrid)
        Controls.Add(WLeaderBoardGrid)
        Controls.Add(MoveDisplayer)
        Font = New Font("Microsoft Sans Serif", 8.25F)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MaximumSize = New Size(1200, 639)
        Name = "Chess"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Chess Game & Artificial Intelligence"
        CType(BP1, ComponentModel.ISupportInitialize).EndInit()
        CType(BR1, ComponentModel.ISupportInitialize).EndInit()
        CType(BN1, ComponentModel.ISupportInitialize).EndInit()
        CType(BB1, ComponentModel.ISupportInitialize).EndInit()
        CType(BQ1, ComponentModel.ISupportInitialize).EndInit()
        CType(BK1, ComponentModel.ISupportInitialize).EndInit()
        CType(WP1, ComponentModel.ISupportInitialize).EndInit()
        CType(WR1, ComponentModel.ISupportInitialize).EndInit()
        CType(WN1, ComponentModel.ISupportInitialize).EndInit()
        CType(WB1, ComponentModel.ISupportInitialize).EndInit()
        CType(WQ1, ComponentModel.ISupportInitialize).EndInit()
        CType(WK1, ComponentModel.ISupportInitialize).EndInit()
        CType(Checkerboard, ComponentModel.ISupportInitialize).EndInit()
        CType(UserTimeBar, ComponentModel.ISupportInitialize).EndInit()
        CType(WLeaderBoardGrid, ComponentModel.ISupportInitialize).EndInit()
        CType(BLeaderBoardGrid, ComponentModel.ISupportInitialize).EndInit()
        CType(AutoAdvanceBar, ComponentModel.ISupportInitialize).EndInit()
        AutoOutputterPanel.ResumeLayout(False)
        AutoOutputterPanel.PerformLayout()
        BoardEditPanel.ResumeLayout(False)
        BoardEditPanel.PerformLayout()
        AISettingsPanel.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()

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
    Friend WithEvents InfoBtn As Button
    Friend WithEvents UseBook As System.Windows.Forms.CheckBox
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
    Friend WithEvents RemoteModeBtn As Button
    Friend WithEvents ChessTimer As Timer
    Friend WithEvents BoardEditDiscardBtn As Button
    Friend WithEvents AISettingsPanel As Panel
    Friend WithEvents AISettingsBox As ComboBox
    Friend WithEvents AISettingResetBtn As Button
End Class
