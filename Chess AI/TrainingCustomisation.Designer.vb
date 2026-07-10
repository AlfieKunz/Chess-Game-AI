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
        Me.BackBtn = New System.Windows.Forms.Button()
        Me.PuzzlesBtn = New System.Windows.Forms.Button()
        Me.CoorBtn = New System.Windows.Forms.Button()
        Me.MoveBtn = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'BackBtn
        '
        Me.BackBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BackBtn.Location = New System.Drawing.Point(10, 246)
        Me.BackBtn.Name = "BackBtn"
        Me.BackBtn.Size = New System.Drawing.Size(23, 23)
        Me.BackBtn.TabIndex = 13
        Me.BackBtn.Text = "↺"
        Me.BackBtn.UseVisualStyleBackColor = True
        '
        'PuzzlesBtn
        '
        Me.PuzzlesBtn.BackColor = System.Drawing.Color.Silver
        Me.PuzzlesBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.PuzzlesBtn.Cursor = System.Windows.Forms.Cursors.Hand
        Me.PuzzlesBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.PuzzlesBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PuzzlesBtn.Location = New System.Drawing.Point(10, 10)
        Me.PuzzlesBtn.Name = "PuzzlesBtn"
        Me.PuzzlesBtn.Size = New System.Drawing.Size(262, 60)
        Me.PuzzlesBtn.TabIndex = 39
        Me.PuzzlesBtn.Text = "Puzzles"
        Me.PuzzlesBtn.UseVisualStyleBackColor = False
        '
        'CoorBtn
        '
        Me.CoorBtn.BackColor = System.Drawing.Color.Silver
        Me.CoorBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.CoorBtn.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CoorBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.CoorBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CoorBtn.Location = New System.Drawing.Point(10, 90)
        Me.CoorBtn.Name = "CoorBtn"
        Me.CoorBtn.Size = New System.Drawing.Size(262, 60)
        Me.CoorBtn.TabIndex = 39
        Me.CoorBtn.Text = "Coordinate Practice"
        Me.CoorBtn.UseVisualStyleBackColor = False
        '
        'MoveBtn
        '
        Me.MoveBtn.BackColor = System.Drawing.Color.Silver
        Me.MoveBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.MoveBtn.Cursor = System.Windows.Forms.Cursors.Hand
        Me.MoveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.MoveBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MoveBtn.Location = New System.Drawing.Point(10, 170)
        Me.MoveBtn.Name = "MoveBtn"
        Me.MoveBtn.Size = New System.Drawing.Size(262, 60)
        Me.MoveBtn.TabIndex = 39
        Me.MoveBtn.Text = "Move Practice"
        Me.MoveBtn.UseVisualStyleBackColor = False
        '
        'TrainingCustomisation
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.LightGray
        Me.ClientSize = New System.Drawing.Size(284, 281)
        Me.Controls.Add(Me.MoveBtn)
        Me.Controls.Add(Me.CoorBtn)
        Me.Controls.Add(Me.PuzzlesBtn)
        Me.Controls.Add(Me.BackBtn)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "TrainingCustomisation"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Training Options"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents BackBtn As Button
    Friend WithEvents PuzzlesBtn As Button
    Friend WithEvents CoorBtn As Button
    Friend WithEvents MoveBtn As Button
End Class
