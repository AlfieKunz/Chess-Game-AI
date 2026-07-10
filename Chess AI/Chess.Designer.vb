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
        Me.FENTextBox = New System.Windows.Forms.TextBox()
        Me.FENButton = New System.Windows.Forms.Button()
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
        Me.ColourChangerButton = New System.Windows.Forms.Button()
        Me.WP8 = New System.Windows.Forms.PictureBox()
        Me.WP7 = New System.Windows.Forms.PictureBox()
        Me.WP6 = New System.Windows.Forms.PictureBox()
        Me.WP5 = New System.Windows.Forms.PictureBox()
        Me.WP4 = New System.Windows.Forms.PictureBox()
        Me.WP3 = New System.Windows.Forms.PictureBox()
        Me.WP2 = New System.Windows.Forms.PictureBox()
        Me.BP8 = New System.Windows.Forms.PictureBox()
        Me.BP4 = New System.Windows.Forms.PictureBox()
        Me.BP7 = New System.Windows.Forms.PictureBox()
        Me.BP3 = New System.Windows.Forms.PictureBox()
        Me.BP6 = New System.Windows.Forms.PictureBox()
        Me.BP2 = New System.Windows.Forms.PictureBox()
        Me.BP5 = New System.Windows.Forms.PictureBox()
        Me.BP1 = New System.Windows.Forms.PictureBox()
        Me.BR2 = New System.Windows.Forms.PictureBox()
        Me.BR1 = New System.Windows.Forms.PictureBox()
        Me.BN2 = New System.Windows.Forms.PictureBox()
        Me.BN1 = New System.Windows.Forms.PictureBox()
        Me.BB2 = New System.Windows.Forms.PictureBox()
        Me.BB1 = New System.Windows.Forms.PictureBox()
        Me.BQ6 = New System.Windows.Forms.PictureBox()
        Me.BQ2 = New System.Windows.Forms.PictureBox()
        Me.BQ9 = New System.Windows.Forms.PictureBox()
        Me.BQ8 = New System.Windows.Forms.PictureBox()
        Me.BQ4 = New System.Windows.Forms.PictureBox()
        Me.BQ5 = New System.Windows.Forms.PictureBox()
        Me.BQ7 = New System.Windows.Forms.PictureBox()
        Me.BQ3 = New System.Windows.Forms.PictureBox()
        Me.BQ1 = New System.Windows.Forms.PictureBox()
        Me.BK1 = New System.Windows.Forms.PictureBox()
        Me.WP1 = New System.Windows.Forms.PictureBox()
        Me.WR2 = New System.Windows.Forms.PictureBox()
        Me.WR1 = New System.Windows.Forms.PictureBox()
        Me.WN2 = New System.Windows.Forms.PictureBox()
        Me.WN1 = New System.Windows.Forms.PictureBox()
        Me.WB2 = New System.Windows.Forms.PictureBox()
        Me.WB1 = New System.Windows.Forms.PictureBox()
        Me.WQ9 = New System.Windows.Forms.PictureBox()
        Me.WQ8 = New System.Windows.Forms.PictureBox()
        Me.WQ7 = New System.Windows.Forms.PictureBox()
        Me.WQ6 = New System.Windows.Forms.PictureBox()
        Me.WQ5 = New System.Windows.Forms.PictureBox()
        Me.WQ4 = New System.Windows.Forms.PictureBox()
        Me.WQ3 = New System.Windows.Forms.PictureBox()
        Me.WQ2 = New System.Windows.Forms.PictureBox()
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
        Me.ColourChanger.SuspendLayout()
        CType(Me.WP8, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WP7, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WP6, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WP5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WP4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WP3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WP2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BP8, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BP4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BP7, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BP3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BP6, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BP2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BP5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BP1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BR2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BR1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BN2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BN1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BB2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BB1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BQ6, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BQ2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BQ9, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BQ8, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BQ4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BQ5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BQ7, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BQ3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BQ1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BK1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WP1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WR2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WR1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WN2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WN1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WB2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WB1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WQ9, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WQ8, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WQ7, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WQ6, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WQ5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WQ4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WQ3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WQ2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WQ1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WK1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Checkerboard, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.UserTimeBar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WLeaderBoardGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BLeaderBoardGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'FENTextBox
        '
        Me.FENTextBox.Location = New System.Drawing.Point(9, 12)
        Me.FENTextBox.Multiline = True
        Me.FENTextBox.Name = "FENTextBox"
        Me.FENTextBox.Size = New System.Drawing.Size(282, 35)
        Me.FENTextBox.TabIndex = 2
        '
        'FENButton
        '
        Me.FENButton.Location = New System.Drawing.Point(95, 53)
        Me.FENButton.Name = "FENButton"
        Me.FENButton.Size = New System.Drawing.Size(113, 23)
        Me.FENButton.TabIndex = 3
        Me.FENButton.Text = "Click to Set FEN:"
        Me.FENButton.UseVisualStyleBackColor = True
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
        Me.FENExport.Size = New System.Drawing.Size(141, 43)
        Me.FENExport.TabIndex = 7
        Me.FENExport.Text = "Export to FEN"
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
        Me.CurrentAIEval.Location = New System.Drawing.Point(955, 280)
        Me.CurrentAIEval.Name = "CurrentAIEval"
        Me.CurrentAIEval.Size = New System.Drawing.Size(94, 16)
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
        Me.Credits.Size = New System.Drawing.Size(155, 16)
        Me.Credits.TabIndex = 10
        Me.Credits.Text = "Created by Alfie Kunz"
        '
        'UndoFENChange
        '
        Me.UndoFENChange.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UndoFENChange.Location = New System.Drawing.Point(213, 53)
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
        'ColourChangerButton
        '
        Me.ColourChangerButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ColourChangerButton.Location = New System.Drawing.Point(95, 300)
        Me.ColourChangerButton.Name = "ColourChangerButton"
        Me.ColourChangerButton.Size = New System.Drawing.Size(113, 40)
        Me.ColourChangerButton.TabIndex = 15
        Me.ColourChangerButton.Text = "Change Board Colour Scheme:"
        Me.ColourChangerButton.UseVisualStyleBackColor = True
        '
        'WP8
        '
        Me.WP8.BackColor = System.Drawing.Color.Transparent
        Me.WP8.Image = CType(resources.GetObject("WP8.Image"), System.Drawing.Image)
        Me.WP8.InitialImage = Nothing
        Me.WP8.Location = New System.Drawing.Point(864, 81)
        Me.WP8.Name = "WP8"
        Me.WP8.Size = New System.Drawing.Size(75, 75)
        Me.WP8.TabIndex = 0
        Me.WP8.TabStop = False
        '
        'WP7
        '
        Me.WP7.BackColor = System.Drawing.Color.Transparent
        Me.WP7.Image = CType(resources.GetObject("WP7.Image"), System.Drawing.Image)
        Me.WP7.InitialImage = Nothing
        Me.WP7.Location = New System.Drawing.Point(783, 81)
        Me.WP7.Name = "WP7"
        Me.WP7.Size = New System.Drawing.Size(75, 75)
        Me.WP7.TabIndex = 0
        Me.WP7.TabStop = False
        '
        'WP6
        '
        Me.WP6.BackColor = System.Drawing.Color.Transparent
        Me.WP6.Image = CType(resources.GetObject("WP6.Image"), System.Drawing.Image)
        Me.WP6.InitialImage = Nothing
        Me.WP6.Location = New System.Drawing.Point(702, 81)
        Me.WP6.Name = "WP6"
        Me.WP6.Size = New System.Drawing.Size(75, 75)
        Me.WP6.TabIndex = 0
        Me.WP6.TabStop = False
        '
        'WP5
        '
        Me.WP5.BackColor = System.Drawing.Color.Transparent
        Me.WP5.Image = CType(resources.GetObject("WP5.Image"), System.Drawing.Image)
        Me.WP5.InitialImage = Nothing
        Me.WP5.Location = New System.Drawing.Point(621, 81)
        Me.WP5.Name = "WP5"
        Me.WP5.Size = New System.Drawing.Size(75, 75)
        Me.WP5.TabIndex = 0
        Me.WP5.TabStop = False
        '
        'WP4
        '
        Me.WP4.BackColor = System.Drawing.Color.Transparent
        Me.WP4.Image = CType(resources.GetObject("WP4.Image"), System.Drawing.Image)
        Me.WP4.InitialImage = Nothing
        Me.WP4.Location = New System.Drawing.Point(540, 81)
        Me.WP4.Name = "WP4"
        Me.WP4.Size = New System.Drawing.Size(75, 75)
        Me.WP4.TabIndex = 0
        Me.WP4.TabStop = False
        '
        'WP3
        '
        Me.WP3.BackColor = System.Drawing.Color.Transparent
        Me.WP3.Image = CType(resources.GetObject("WP3.Image"), System.Drawing.Image)
        Me.WP3.InitialImage = Nothing
        Me.WP3.Location = New System.Drawing.Point(459, 81)
        Me.WP3.Name = "WP3"
        Me.WP3.Size = New System.Drawing.Size(75, 75)
        Me.WP3.TabIndex = 0
        Me.WP3.TabStop = False
        '
        'WP2
        '
        Me.WP2.BackColor = System.Drawing.Color.Transparent
        Me.WP2.Image = CType(resources.GetObject("WP2.Image"), System.Drawing.Image)
        Me.WP2.InitialImage = Nothing
        Me.WP2.Location = New System.Drawing.Point(378, 81)
        Me.WP2.Name = "WP2"
        Me.WP2.Size = New System.Drawing.Size(75, 75)
        Me.WP2.TabIndex = 0
        Me.WP2.TabStop = False
        '
        'BP8
        '
        Me.BP8.BackColor = System.Drawing.Color.Transparent
        Me.BP8.Image = CType(resources.GetObject("BP8.Image"), System.Drawing.Image)
        Me.BP8.InitialImage = Nothing
        Me.BP8.Location = New System.Drawing.Point(864, 243)
        Me.BP8.Name = "BP8"
        Me.BP8.Size = New System.Drawing.Size(75, 75)
        Me.BP8.TabIndex = 0
        Me.BP8.TabStop = False
        '
        'BP4
        '
        Me.BP4.BackColor = System.Drawing.Color.Transparent
        Me.BP4.Image = CType(resources.GetObject("BP4.Image"), System.Drawing.Image)
        Me.BP4.InitialImage = Nothing
        Me.BP4.Location = New System.Drawing.Point(540, 243)
        Me.BP4.Name = "BP4"
        Me.BP4.Size = New System.Drawing.Size(75, 75)
        Me.BP4.TabIndex = 0
        Me.BP4.TabStop = False
        '
        'BP7
        '
        Me.BP7.BackColor = System.Drawing.Color.Transparent
        Me.BP7.Image = CType(resources.GetObject("BP7.Image"), System.Drawing.Image)
        Me.BP7.InitialImage = Nothing
        Me.BP7.Location = New System.Drawing.Point(783, 243)
        Me.BP7.Name = "BP7"
        Me.BP7.Size = New System.Drawing.Size(75, 75)
        Me.BP7.TabIndex = 0
        Me.BP7.TabStop = False
        '
        'BP3
        '
        Me.BP3.BackColor = System.Drawing.Color.Transparent
        Me.BP3.Image = CType(resources.GetObject("BP3.Image"), System.Drawing.Image)
        Me.BP3.InitialImage = Nothing
        Me.BP3.Location = New System.Drawing.Point(459, 243)
        Me.BP3.Name = "BP3"
        Me.BP3.Size = New System.Drawing.Size(75, 75)
        Me.BP3.TabIndex = 0
        Me.BP3.TabStop = False
        '
        'BP6
        '
        Me.BP6.BackColor = System.Drawing.Color.Transparent
        Me.BP6.Image = CType(resources.GetObject("BP6.Image"), System.Drawing.Image)
        Me.BP6.InitialImage = Nothing
        Me.BP6.Location = New System.Drawing.Point(702, 243)
        Me.BP6.Name = "BP6"
        Me.BP6.Size = New System.Drawing.Size(75, 75)
        Me.BP6.TabIndex = 0
        Me.BP6.TabStop = False
        '
        'BP2
        '
        Me.BP2.BackColor = System.Drawing.Color.Transparent
        Me.BP2.Image = CType(resources.GetObject("BP2.Image"), System.Drawing.Image)
        Me.BP2.InitialImage = Nothing
        Me.BP2.Location = New System.Drawing.Point(378, 243)
        Me.BP2.Name = "BP2"
        Me.BP2.Size = New System.Drawing.Size(75, 75)
        Me.BP2.TabIndex = 0
        Me.BP2.TabStop = False
        '
        'BP5
        '
        Me.BP5.BackColor = System.Drawing.Color.Transparent
        Me.BP5.Image = CType(resources.GetObject("BP5.Image"), System.Drawing.Image)
        Me.BP5.InitialImage = Nothing
        Me.BP5.Location = New System.Drawing.Point(621, 243)
        Me.BP5.Name = "BP5"
        Me.BP5.Size = New System.Drawing.Size(75, 75)
        Me.BP5.TabIndex = 0
        Me.BP5.TabStop = False
        '
        'BP1
        '
        Me.BP1.BackColor = System.Drawing.Color.Transparent
        Me.BP1.Image = CType(resources.GetObject("BP1.Image"), System.Drawing.Image)
        Me.BP1.InitialImage = Nothing
        Me.BP1.Location = New System.Drawing.Point(297, 243)
        Me.BP1.Name = "BP1"
        Me.BP1.Size = New System.Drawing.Size(75, 75)
        Me.BP1.TabIndex = 0
        Me.BP1.TabStop = False
        '
        'BR2
        '
        Me.BR2.BackColor = System.Drawing.Color.Transparent
        Me.BR2.Image = CType(resources.GetObject("BR2.Image"), System.Drawing.Image)
        Me.BR2.InitialImage = Nothing
        Me.BR2.Location = New System.Drawing.Point(864, 162)
        Me.BR2.Name = "BR2"
        Me.BR2.Size = New System.Drawing.Size(75, 75)
        Me.BR2.TabIndex = 0
        Me.BR2.TabStop = False
        '
        'BR1
        '
        Me.BR1.BackColor = System.Drawing.Color.Transparent
        Me.BR1.Image = CType(resources.GetObject("BR1.Image"), System.Drawing.Image)
        Me.BR1.InitialImage = Nothing
        Me.BR1.Location = New System.Drawing.Point(783, 162)
        Me.BR1.Name = "BR1"
        Me.BR1.Size = New System.Drawing.Size(75, 75)
        Me.BR1.TabIndex = 0
        Me.BR1.TabStop = False
        '
        'BN2
        '
        Me.BN2.BackColor = System.Drawing.Color.Transparent
        Me.BN2.Image = CType(resources.GetObject("BN2.Image"), System.Drawing.Image)
        Me.BN2.InitialImage = Nothing
        Me.BN2.Location = New System.Drawing.Point(702, 162)
        Me.BN2.Name = "BN2"
        Me.BN2.Size = New System.Drawing.Size(75, 75)
        Me.BN2.TabIndex = 0
        Me.BN2.TabStop = False
        '
        'BN1
        '
        Me.BN1.BackColor = System.Drawing.Color.Transparent
        Me.BN1.Image = CType(resources.GetObject("BN1.Image"), System.Drawing.Image)
        Me.BN1.InitialImage = Nothing
        Me.BN1.Location = New System.Drawing.Point(621, 162)
        Me.BN1.Name = "BN1"
        Me.BN1.Size = New System.Drawing.Size(75, 75)
        Me.BN1.TabIndex = 0
        Me.BN1.TabStop = False
        '
        'BB2
        '
        Me.BB2.BackColor = System.Drawing.Color.Transparent
        Me.BB2.Image = CType(resources.GetObject("BB2.Image"), System.Drawing.Image)
        Me.BB2.InitialImage = Nothing
        Me.BB2.Location = New System.Drawing.Point(540, 162)
        Me.BB2.Name = "BB2"
        Me.BB2.Size = New System.Drawing.Size(75, 75)
        Me.BB2.TabIndex = 0
        Me.BB2.TabStop = False
        '
        'BB1
        '
        Me.BB1.BackColor = System.Drawing.Color.Transparent
        Me.BB1.Image = CType(resources.GetObject("BB1.Image"), System.Drawing.Image)
        Me.BB1.InitialImage = Nothing
        Me.BB1.Location = New System.Drawing.Point(459, 162)
        Me.BB1.Name = "BB1"
        Me.BB1.Size = New System.Drawing.Size(75, 75)
        Me.BB1.TabIndex = 0
        Me.BB1.TabStop = False
        '
        'BQ6
        '
        Me.BQ6.BackColor = System.Drawing.Color.Transparent
        Me.BQ6.Image = CType(resources.GetObject("BQ6.Image"), System.Drawing.Image)
        Me.BQ6.InitialImage = Nothing
        Me.BQ6.Location = New System.Drawing.Point(621, 405)
        Me.BQ6.Name = "BQ6"
        Me.BQ6.Size = New System.Drawing.Size(75, 75)
        Me.BQ6.TabIndex = 0
        Me.BQ6.TabStop = False
        '
        'BQ2
        '
        Me.BQ2.BackColor = System.Drawing.Color.Transparent
        Me.BQ2.Image = CType(resources.GetObject("BQ2.Image"), System.Drawing.Image)
        Me.BQ2.InitialImage = Nothing
        Me.BQ2.Location = New System.Drawing.Point(297, 405)
        Me.BQ2.Name = "BQ2"
        Me.BQ2.Size = New System.Drawing.Size(75, 75)
        Me.BQ2.TabIndex = 0
        Me.BQ2.TabStop = False
        '
        'BQ9
        '
        Me.BQ9.BackColor = System.Drawing.Color.Transparent
        Me.BQ9.Image = CType(resources.GetObject("BQ9.Image"), System.Drawing.Image)
        Me.BQ9.InitialImage = Nothing
        Me.BQ9.Location = New System.Drawing.Point(864, 405)
        Me.BQ9.Name = "BQ9"
        Me.BQ9.Size = New System.Drawing.Size(75, 75)
        Me.BQ9.TabIndex = 0
        Me.BQ9.TabStop = False
        '
        'BQ8
        '
        Me.BQ8.BackColor = System.Drawing.Color.Transparent
        Me.BQ8.Image = CType(resources.GetObject("BQ8.Image"), System.Drawing.Image)
        Me.BQ8.InitialImage = Nothing
        Me.BQ8.Location = New System.Drawing.Point(783, 405)
        Me.BQ8.Name = "BQ8"
        Me.BQ8.Size = New System.Drawing.Size(75, 75)
        Me.BQ8.TabIndex = 0
        Me.BQ8.TabStop = False
        '
        'BQ4
        '
        Me.BQ4.BackColor = System.Drawing.Color.Transparent
        Me.BQ4.Image = CType(resources.GetObject("BQ4.Image"), System.Drawing.Image)
        Me.BQ4.InitialImage = Nothing
        Me.BQ4.Location = New System.Drawing.Point(459, 405)
        Me.BQ4.Name = "BQ4"
        Me.BQ4.Size = New System.Drawing.Size(75, 75)
        Me.BQ4.TabIndex = 0
        Me.BQ4.TabStop = False
        '
        'BQ5
        '
        Me.BQ5.BackColor = System.Drawing.Color.Transparent
        Me.BQ5.Image = CType(resources.GetObject("BQ5.Image"), System.Drawing.Image)
        Me.BQ5.InitialImage = Nothing
        Me.BQ5.Location = New System.Drawing.Point(540, 405)
        Me.BQ5.Name = "BQ5"
        Me.BQ5.Size = New System.Drawing.Size(75, 75)
        Me.BQ5.TabIndex = 0
        Me.BQ5.TabStop = False
        '
        'BQ7
        '
        Me.BQ7.BackColor = System.Drawing.Color.Transparent
        Me.BQ7.Image = CType(resources.GetObject("BQ7.Image"), System.Drawing.Image)
        Me.BQ7.InitialImage = Nothing
        Me.BQ7.Location = New System.Drawing.Point(702, 405)
        Me.BQ7.Name = "BQ7"
        Me.BQ7.Size = New System.Drawing.Size(75, 75)
        Me.BQ7.TabIndex = 0
        Me.BQ7.TabStop = False
        '
        'BQ3
        '
        Me.BQ3.BackColor = System.Drawing.Color.Transparent
        Me.BQ3.Image = CType(resources.GetObject("BQ3.Image"), System.Drawing.Image)
        Me.BQ3.InitialImage = Nothing
        Me.BQ3.Location = New System.Drawing.Point(378, 405)
        Me.BQ3.Name = "BQ3"
        Me.BQ3.Size = New System.Drawing.Size(75, 75)
        Me.BQ3.TabIndex = 0
        Me.BQ3.TabStop = False
        '
        'BQ1
        '
        Me.BQ1.BackColor = System.Drawing.Color.Transparent
        Me.BQ1.Image = CType(resources.GetObject("BQ1.Image"), System.Drawing.Image)
        Me.BQ1.InitialImage = Nothing
        Me.BQ1.Location = New System.Drawing.Point(378, 162)
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
        Me.BK1.Location = New System.Drawing.Point(297, 162)
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
        Me.WP1.Location = New System.Drawing.Point(297, 81)
        Me.WP1.Name = "WP1"
        Me.WP1.Size = New System.Drawing.Size(75, 75)
        Me.WP1.TabIndex = 0
        Me.WP1.TabStop = False
        '
        'WR2
        '
        Me.WR2.BackColor = System.Drawing.Color.Transparent
        Me.WR2.Image = CType(resources.GetObject("WR2.Image"), System.Drawing.Image)
        Me.WR2.InitialImage = Nothing
        Me.WR2.Location = New System.Drawing.Point(864, 0)
        Me.WR2.Name = "WR2"
        Me.WR2.Size = New System.Drawing.Size(75, 75)
        Me.WR2.TabIndex = 0
        Me.WR2.TabStop = False
        '
        'WR1
        '
        Me.WR1.BackColor = System.Drawing.Color.Transparent
        Me.WR1.Image = CType(resources.GetObject("WR1.Image"), System.Drawing.Image)
        Me.WR1.InitialImage = Nothing
        Me.WR1.Location = New System.Drawing.Point(783, 0)
        Me.WR1.Name = "WR1"
        Me.WR1.Size = New System.Drawing.Size(75, 75)
        Me.WR1.TabIndex = 0
        Me.WR1.TabStop = False
        '
        'WN2
        '
        Me.WN2.BackColor = System.Drawing.Color.Transparent
        Me.WN2.Image = CType(resources.GetObject("WN2.Image"), System.Drawing.Image)
        Me.WN2.InitialImage = Nothing
        Me.WN2.Location = New System.Drawing.Point(702, 0)
        Me.WN2.Name = "WN2"
        Me.WN2.Size = New System.Drawing.Size(75, 75)
        Me.WN2.TabIndex = 0
        Me.WN2.TabStop = False
        '
        'WN1
        '
        Me.WN1.BackColor = System.Drawing.Color.Transparent
        Me.WN1.Image = CType(resources.GetObject("WN1.Image"), System.Drawing.Image)
        Me.WN1.InitialImage = Nothing
        Me.WN1.Location = New System.Drawing.Point(621, 0)
        Me.WN1.Name = "WN1"
        Me.WN1.Size = New System.Drawing.Size(75, 75)
        Me.WN1.TabIndex = 0
        Me.WN1.TabStop = False
        '
        'WB2
        '
        Me.WB2.BackColor = System.Drawing.Color.Transparent
        Me.WB2.Image = CType(resources.GetObject("WB2.Image"), System.Drawing.Image)
        Me.WB2.InitialImage = Nothing
        Me.WB2.Location = New System.Drawing.Point(540, 0)
        Me.WB2.Name = "WB2"
        Me.WB2.Size = New System.Drawing.Size(75, 75)
        Me.WB2.TabIndex = 0
        Me.WB2.TabStop = False
        '
        'WB1
        '
        Me.WB1.BackColor = System.Drawing.Color.Transparent
        Me.WB1.Image = CType(resources.GetObject("WB1.Image"), System.Drawing.Image)
        Me.WB1.InitialImage = Nothing
        Me.WB1.Location = New System.Drawing.Point(459, 0)
        Me.WB1.Name = "WB1"
        Me.WB1.Size = New System.Drawing.Size(75, 75)
        Me.WB1.TabIndex = 0
        Me.WB1.TabStop = False
        '
        'WQ9
        '
        Me.WQ9.BackColor = System.Drawing.Color.Transparent
        Me.WQ9.Image = CType(resources.GetObject("WQ9.Image"), System.Drawing.Image)
        Me.WQ9.InitialImage = Nothing
        Me.WQ9.Location = New System.Drawing.Point(864, 324)
        Me.WQ9.Name = "WQ9"
        Me.WQ9.Size = New System.Drawing.Size(75, 75)
        Me.WQ9.TabIndex = 0
        Me.WQ9.TabStop = False
        '
        'WQ8
        '
        Me.WQ8.BackColor = System.Drawing.Color.Transparent
        Me.WQ8.Image = CType(resources.GetObject("WQ8.Image"), System.Drawing.Image)
        Me.WQ8.InitialImage = Nothing
        Me.WQ8.Location = New System.Drawing.Point(783, 324)
        Me.WQ8.Name = "WQ8"
        Me.WQ8.Size = New System.Drawing.Size(75, 75)
        Me.WQ8.TabIndex = 0
        Me.WQ8.TabStop = False
        '
        'WQ7
        '
        Me.WQ7.BackColor = System.Drawing.Color.Transparent
        Me.WQ7.Image = CType(resources.GetObject("WQ7.Image"), System.Drawing.Image)
        Me.WQ7.InitialImage = Nothing
        Me.WQ7.Location = New System.Drawing.Point(702, 324)
        Me.WQ7.Name = "WQ7"
        Me.WQ7.Size = New System.Drawing.Size(75, 75)
        Me.WQ7.TabIndex = 0
        Me.WQ7.TabStop = False
        '
        'WQ6
        '
        Me.WQ6.BackColor = System.Drawing.Color.Transparent
        Me.WQ6.Image = CType(resources.GetObject("WQ6.Image"), System.Drawing.Image)
        Me.WQ6.InitialImage = Nothing
        Me.WQ6.Location = New System.Drawing.Point(621, 324)
        Me.WQ6.Name = "WQ6"
        Me.WQ6.Size = New System.Drawing.Size(75, 75)
        Me.WQ6.TabIndex = 0
        Me.WQ6.TabStop = False
        '
        'WQ5
        '
        Me.WQ5.BackColor = System.Drawing.Color.Transparent
        Me.WQ5.Image = CType(resources.GetObject("WQ5.Image"), System.Drawing.Image)
        Me.WQ5.InitialImage = Nothing
        Me.WQ5.Location = New System.Drawing.Point(540, 324)
        Me.WQ5.Name = "WQ5"
        Me.WQ5.Size = New System.Drawing.Size(75, 75)
        Me.WQ5.TabIndex = 0
        Me.WQ5.TabStop = False
        '
        'WQ4
        '
        Me.WQ4.BackColor = System.Drawing.Color.Transparent
        Me.WQ4.Image = CType(resources.GetObject("WQ4.Image"), System.Drawing.Image)
        Me.WQ4.InitialImage = Nothing
        Me.WQ4.Location = New System.Drawing.Point(459, 324)
        Me.WQ4.Name = "WQ4"
        Me.WQ4.Size = New System.Drawing.Size(75, 75)
        Me.WQ4.TabIndex = 0
        Me.WQ4.TabStop = False
        '
        'WQ3
        '
        Me.WQ3.BackColor = System.Drawing.Color.Transparent
        Me.WQ3.Image = CType(resources.GetObject("WQ3.Image"), System.Drawing.Image)
        Me.WQ3.InitialImage = Nothing
        Me.WQ3.Location = New System.Drawing.Point(378, 324)
        Me.WQ3.Name = "WQ3"
        Me.WQ3.Size = New System.Drawing.Size(75, 75)
        Me.WQ3.TabIndex = 0
        Me.WQ3.TabStop = False
        '
        'WQ2
        '
        Me.WQ2.BackColor = System.Drawing.Color.Transparent
        Me.WQ2.Image = CType(resources.GetObject("WQ2.Image"), System.Drawing.Image)
        Me.WQ2.InitialImage = Nothing
        Me.WQ2.Location = New System.Drawing.Point(297, 324)
        Me.WQ2.Name = "WQ2"
        Me.WQ2.Size = New System.Drawing.Size(75, 75)
        Me.WQ2.TabIndex = 0
        Me.WQ2.TabStop = False
        '
        'WQ1
        '
        Me.WQ1.BackColor = System.Drawing.Color.Transparent
        Me.WQ1.Image = CType(resources.GetObject("WQ1.Image"), System.Drawing.Image)
        Me.WQ1.InitialImage = Nothing
        Me.WQ1.Location = New System.Drawing.Point(378, 0)
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
        Me.WK1.Location = New System.Drawing.Point(297, 0)
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
        Me.ExitBtn.Location = New System.Drawing.Point(1009, 505)
        Me.ExitBtn.Name = "ExitBtn"
        Me.ExitBtn.Size = New System.Drawing.Size(78, 40)
        Me.ExitBtn.TabIndex = 22
        Me.ExitBtn.Text = "Exit to Main Menu"
        Me.ExitBtn.UseVisualStyleBackColor = True
        '
        'UserTimeBox
        '
        Me.UserTimeBox.BackColor = System.Drawing.Color.WhiteSmoke
        Me.UserTimeBox.Location = New System.Drawing.Point(962, 340)
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
        Me.UserTimeBar.Location = New System.Drawing.Point(962, 364)
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
        Me.CurrentAIMove.Location = New System.Drawing.Point(955, 250)
        Me.CurrentAIMove.Name = "CurrentAIMove"
        Me.CurrentAIMove.Size = New System.Drawing.Size(112, 16)
        Me.CurrentAIMove.TabIndex = 9
        Me.CurrentAIMove.Text = "Current Move: -"
        '
        'CurrentAIDepth
        '
        Me.CurrentAIDepth.AutoSize = True
        Me.CurrentAIDepth.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CurrentAIDepth.Location = New System.Drawing.Point(955, 220)
        Me.CurrentAIDepth.Name = "CurrentAIDepth"
        Me.CurrentAIDepth.Size = New System.Drawing.Size(115, 16)
        Me.CurrentAIDepth.TabIndex = 9
        Me.CurrentAIDepth.Text = "Current Depth: -"
        '
        'QuiescenceBox
        '
        Me.QuiescenceBox.AutoSize = True
        Me.QuiescenceBox.Checked = True
        Me.QuiescenceBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.QuiescenceBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.QuiescenceBox.Location = New System.Drawing.Point(983, 405)
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
        Me.AIEndlessMode.Location = New System.Drawing.Point(1009, 430)
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
        Me.UserIndex.Name = "UserIndex"
        Me.UserIndex.ReadOnly = True
        Me.UserIndex.Width = 21
        '
        'Username
        '
        Me.Username.Frozen = True
        Me.Username.HeaderText = "Name"
        Me.Username.Name = "Username"
        Me.Username.ReadOnly = True
        Me.Username.Width = 130
        '
        'UserScore
        '
        Me.UserScore.Frozen = True
        Me.UserScore.HeaderText = "Score"
        Me.UserScore.Name = "UserScore"
        Me.UserScore.ReadOnly = True
        Me.UserScore.Width = 50
        '
        'UserDate
        '
        Me.UserDate.Frozen = True
        Me.UserDate.HeaderText = "Date"
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
        Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        Me.DataGridViewTextBoxColumn1.ReadOnly = True
        Me.DataGridViewTextBoxColumn1.Width = 21
        '
        'DataGridViewTextBoxColumn2
        '
        Me.DataGridViewTextBoxColumn2.Frozen = True
        Me.DataGridViewTextBoxColumn2.HeaderText = "Name"
        Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        Me.DataGridViewTextBoxColumn2.ReadOnly = True
        Me.DataGridViewTextBoxColumn2.Width = 130
        '
        'DataGridViewTextBoxColumn3
        '
        Me.DataGridViewTextBoxColumn3.Frozen = True
        Me.DataGridViewTextBoxColumn3.HeaderText = "Score"
        Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
        Me.DataGridViewTextBoxColumn3.ReadOnly = True
        Me.DataGridViewTextBoxColumn3.Width = 50
        '
        'DataGridViewTextBoxColumn4
        '
        Me.DataGridViewTextBoxColumn4.Frozen = True
        Me.DataGridViewTextBoxColumn4.HeaderText = "Date"
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
        'TrainingTimer
        '
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
        Me.UseBook.Location = New System.Drawing.Point(977, 455)
        Me.UseBook.Name = "UseBook"
        Me.UseBook.Size = New System.Drawing.Size(136, 19)
        Me.UseBook.TabIndex = 26
        Me.UseBook.Text = "Use Opening Book?"
        Me.UseBook.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.UseBook.UseVisualStyleBackColor = True
        '
        'Chess
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(155, Byte), Integer), CType(CType(155, Byte), Integer), CType(CType(155, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1184, 600)
        Me.Controls.Add(Me.InfoBtn)
        Me.Controls.Add(Me.TrainingScore)
        Me.Controls.Add(Me.BLeaderBoardGrid)
        Me.Controls.Add(Me.WLeaderBoardGrid)
        Me.Controls.Add(Me.AITerminator)
        Me.Controls.Add(Me.UseBook)
        Me.Controls.Add(Me.AIEndlessMode)
        Me.Controls.Add(Me.QuiescenceBox)
        Me.Controls.Add(Me.UserTimeBar)
        Me.Controls.Add(Me.UserTimeBox)
        Me.Controls.Add(Me.ExitBtn)
        Me.Controls.Add(Me.AutoFlipper)
        Me.Controls.Add(Me.FlipperButton)
        Me.Controls.Add(Me.ColourChangerButton)
        Me.Controls.Add(Me.ProgressBar)
        Me.Controls.Add(Me.UndoFENChange)
        Me.Controls.Add(Me.Credits)
        Me.Controls.Add(Me.MoveDisplayer)
        Me.Controls.Add(Me.TimerLabel)
        Me.Controls.Add(Me.CurrentAIDepth)
        Me.Controls.Add(Me.CurrentAIMove)
        Me.Controls.Add(Me.CurrentAIEval)
        Me.Controls.Add(Me.CheckLabel)
        Me.Controls.Add(Me.TrainingStart)
        Me.Controls.Add(Me.UndoMove)
        Me.Controls.Add(Me.FENExport)
        Me.Controls.Add(Me.Reset_Btn)
        Me.Controls.Add(Me.AIMoveBtn)
        Me.Controls.Add(Me.FENButton)
        Me.Controls.Add(Me.FENTextBox)
        Me.Controls.Add(Me.WP8)
        Me.Controls.Add(Me.WP7)
        Me.Controls.Add(Me.WP6)
        Me.Controls.Add(Me.WP5)
        Me.Controls.Add(Me.WP4)
        Me.Controls.Add(Me.WP3)
        Me.Controls.Add(Me.WP2)
        Me.Controls.Add(Me.BP8)
        Me.Controls.Add(Me.BP4)
        Me.Controls.Add(Me.BP7)
        Me.Controls.Add(Me.BP3)
        Me.Controls.Add(Me.BP6)
        Me.Controls.Add(Me.BP2)
        Me.Controls.Add(Me.BP5)
        Me.Controls.Add(Me.BP1)
        Me.Controls.Add(Me.BR2)
        Me.Controls.Add(Me.BR1)
        Me.Controls.Add(Me.BN2)
        Me.Controls.Add(Me.BN1)
        Me.Controls.Add(Me.BB2)
        Me.Controls.Add(Me.BB1)
        Me.Controls.Add(Me.BQ6)
        Me.Controls.Add(Me.BQ2)
        Me.Controls.Add(Me.BQ9)
        Me.Controls.Add(Me.BQ8)
        Me.Controls.Add(Me.BQ4)
        Me.Controls.Add(Me.BQ5)
        Me.Controls.Add(Me.BQ7)
        Me.Controls.Add(Me.BQ3)
        Me.Controls.Add(Me.BQ1)
        Me.Controls.Add(Me.BK1)
        Me.Controls.Add(Me.WP1)
        Me.Controls.Add(Me.WR2)
        Me.Controls.Add(Me.WR1)
        Me.Controls.Add(Me.WN2)
        Me.Controls.Add(Me.WN1)
        Me.Controls.Add(Me.WB2)
        Me.Controls.Add(Me.WB1)
        Me.Controls.Add(Me.WQ9)
        Me.Controls.Add(Me.WQ8)
        Me.Controls.Add(Me.WQ7)
        Me.Controls.Add(Me.WQ6)
        Me.Controls.Add(Me.WQ5)
        Me.Controls.Add(Me.WQ4)
        Me.Controls.Add(Me.WQ3)
        Me.Controls.Add(Me.WQ2)
        Me.Controls.Add(Me.WQ1)
        Me.Controls.Add(Me.WK1)
        Me.Controls.Add(Me.Checkerboard)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(1200, 639)
        Me.Name = "Chess"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Chess Game & Artificial Intelligence"
        Me.ColourChanger.ResumeLayout(False)
        CType(Me.WP8, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WP7, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WP6, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WP5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WP4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WP3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WP2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BP8, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BP4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BP7, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BP3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BP6, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BP2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BP5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BP1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BR2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BR1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BN2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BN1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BB2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BB1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BQ6, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BQ2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BQ9, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BQ8, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BQ4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BQ5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BQ7, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BQ3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BQ1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BK1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WP1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WR2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WR1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WN2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WN1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WB2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WB1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WQ9, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WQ8, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WQ7, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WQ6, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WQ5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WQ4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WQ3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WQ2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WQ1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WK1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Checkerboard, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.UserTimeBar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WLeaderBoardGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BLeaderBoardGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents WK1 As System.Windows.Forms.PictureBox
    Friend WithEvents WQ1 As System.Windows.Forms.PictureBox
    Friend WithEvents WB1 As System.Windows.Forms.PictureBox
    Friend WithEvents WB2 As System.Windows.Forms.PictureBox
    Friend WithEvents WN1 As System.Windows.Forms.PictureBox
    Friend WithEvents WN2 As System.Windows.Forms.PictureBox
    Friend WithEvents WR1 As System.Windows.Forms.PictureBox
    Friend WithEvents WR2 As System.Windows.Forms.PictureBox
    Friend WithEvents WP1 As System.Windows.Forms.PictureBox
    Friend WithEvents WP2 As System.Windows.Forms.PictureBox
    Friend WithEvents WP3 As System.Windows.Forms.PictureBox
    Friend WithEvents WP4 As System.Windows.Forms.PictureBox
    Friend WithEvents WP5 As System.Windows.Forms.PictureBox
    Friend WithEvents WP6 As System.Windows.Forms.PictureBox
    Friend WithEvents WP7 As System.Windows.Forms.PictureBox
    Friend WithEvents WP8 As System.Windows.Forms.PictureBox
    Friend WithEvents BK1 As System.Windows.Forms.PictureBox
    Friend WithEvents BQ1 As System.Windows.Forms.PictureBox
    Friend WithEvents BB1 As System.Windows.Forms.PictureBox
    Friend WithEvents BN1 As System.Windows.Forms.PictureBox
    Friend WithEvents BR1 As System.Windows.Forms.PictureBox
    Friend WithEvents BB2 As System.Windows.Forms.PictureBox
    Friend WithEvents BN2 As System.Windows.Forms.PictureBox
    Friend WithEvents BR2 As System.Windows.Forms.PictureBox
    Friend WithEvents BP1 As System.Windows.Forms.PictureBox
    Friend WithEvents BP2 As System.Windows.Forms.PictureBox
    Friend WithEvents BP3 As System.Windows.Forms.PictureBox
    Friend WithEvents BP4 As System.Windows.Forms.PictureBox
    Friend WithEvents BP5 As System.Windows.Forms.PictureBox
    Friend WithEvents BP6 As System.Windows.Forms.PictureBox
    Friend WithEvents BP7 As System.Windows.Forms.PictureBox
    Friend WithEvents BP8 As System.Windows.Forms.PictureBox
    Friend WithEvents Checkerboard As System.Windows.Forms.PictureBox
    Friend WithEvents FENTextBox As TextBox
    Friend WithEvents FENButton As Button
    Friend WithEvents WQ2 As System.Windows.Forms.PictureBox
    Friend WithEvents WQ3 As System.Windows.Forms.PictureBox
    Friend WithEvents WQ4 As System.Windows.Forms.PictureBox
    Friend WithEvents WQ5 As System.Windows.Forms.PictureBox
    Friend WithEvents WQ6 As System.Windows.Forms.PictureBox
    Friend WithEvents WQ7 As System.Windows.Forms.PictureBox
    Friend WithEvents WQ8 As System.Windows.Forms.PictureBox
    Friend WithEvents BQ3 As System.Windows.Forms.PictureBox
    Friend WithEvents BQ4 As System.Windows.Forms.PictureBox
    Friend WithEvents BQ2 As System.Windows.Forms.PictureBox
    Friend WithEvents BQ7 As System.Windows.Forms.PictureBox
    Friend WithEvents BQ5 As System.Windows.Forms.PictureBox
    Friend WithEvents BQ8 As System.Windows.Forms.PictureBox
    Friend WithEvents BQ6 As System.Windows.Forms.PictureBox
    Friend WithEvents AIMoveBtn As Button
    Friend WithEvents Reset_Btn As Button
    Friend WithEvents FENExport As Button
    Friend WithEvents CheckLabel As Label
    Friend WithEvents CurrentAIEval As Label
    Friend WithEvents UndoMove As System.Windows.Forms.Button
    Friend WithEvents Credits As Label
    Friend WithEvents UndoFENChange As System.Windows.Forms.Button
    Friend WithEvents ProgressBar As ProgressBar
    Friend WithEvents WQ9 As System.Windows.Forms.PictureBox
    Friend WithEvents BQ9 As System.Windows.Forms.PictureBox
    Friend WithEvents ColourChangerButton As System.Windows.Forms.Button
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
End Class
