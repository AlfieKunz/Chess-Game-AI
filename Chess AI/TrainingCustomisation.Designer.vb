<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TrainingCustomisation
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TrainingCustomisation))
        BackBtn = New Button()
        PuzzlesBtn = New Button()
        CoorBtn = New Button()
        MoveBtn = New Button()
        SuspendLayout()
        ' 
        ' BackBtn
        ' 
        BackBtn.Font = New Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        BackBtn.Location = New Point(10, 246)
        BackBtn.Name = "BackBtn"
        BackBtn.Size = New Size(23, 23)
        BackBtn.TabIndex = 13
        BackBtn.Text = "↺"
        BackBtn.UseVisualStyleBackColor = True
        ' 
        ' PuzzlesBtn
        ' 
        PuzzlesBtn.BackColor = Color.Silver
        PuzzlesBtn.BackgroundImageLayout = ImageLayout.None
        PuzzlesBtn.Cursor = Cursors.Hand
        PuzzlesBtn.FlatStyle = FlatStyle.Flat
        PuzzlesBtn.Font = New Font("Microsoft Sans Serif", 18F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        PuzzlesBtn.Location = New Point(10, 10)
        PuzzlesBtn.Name = "PuzzlesBtn"
        PuzzlesBtn.Size = New Size(262, 60)
        PuzzlesBtn.TabIndex = 39
        PuzzlesBtn.Text = "Puzzles"
        PuzzlesBtn.UseVisualStyleBackColor = False
        ' 
        ' CoorBtn
        ' 
        CoorBtn.BackColor = Color.Silver
        CoorBtn.BackgroundImageLayout = ImageLayout.None
        CoorBtn.Cursor = Cursors.Hand
        CoorBtn.FlatStyle = FlatStyle.Flat
        CoorBtn.Font = New Font("Microsoft Sans Serif", 18F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        CoorBtn.Location = New Point(10, 90)
        CoorBtn.Name = "CoorBtn"
        CoorBtn.Size = New Size(262, 60)
        CoorBtn.TabIndex = 39
        CoorBtn.Text = "Coordinate Practice"
        CoorBtn.UseVisualStyleBackColor = False
        ' 
        ' MoveBtn
        ' 
        MoveBtn.BackColor = Color.Silver
        MoveBtn.BackgroundImageLayout = ImageLayout.None
        MoveBtn.Cursor = Cursors.Hand
        MoveBtn.FlatStyle = FlatStyle.Flat
        MoveBtn.Font = New Font("Microsoft Sans Serif", 18F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        MoveBtn.Location = New Point(10, 170)
        MoveBtn.Name = "MoveBtn"
        MoveBtn.Size = New Size(262, 60)
        MoveBtn.TabIndex = 39
        MoveBtn.Text = "Move Practice"
        MoveBtn.UseVisualStyleBackColor = False
        ' 
        ' TrainingCustomisation
        ' 
        AutoScaleDimensions = New SizeF(6F, 13F)
        AutoScaleMode = AutoScaleMode.Font
        AutoSizeMode = AutoSizeMode.GrowAndShrink
        BackColor = Color.LightGray
        ClientSize = New Size(284, 281)
        Controls.Add(MoveBtn)
        Controls.Add(CoorBtn)
        Controls.Add(PuzzlesBtn)
        Controls.Add(BackBtn)
        Font = New Font("Microsoft Sans Serif", 8.25F)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "TrainingCustomisation"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Training Options"
        ResumeLayout(False)

    End Sub

    Friend WithEvents BackBtn As Button
    Friend WithEvents PuzzlesBtn As Button
    Friend WithEvents CoorBtn As Button
    Friend WithEvents MoveBtn As Button
End Class
