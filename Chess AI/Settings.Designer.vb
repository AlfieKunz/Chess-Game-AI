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
        Me.Piece = New System.Windows.Forms.PictureBox()
        Me.Checkerboard = New System.Windows.Forms.PictureBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.SpeedSetter = New System.Windows.Forms.TrackBar()
        CType(Me.Piece, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Checkerboard, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SpeedSetter, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Piece
        '
        Me.Piece.BackColor = System.Drawing.Color.Transparent
        Me.Piece.Image = CType(resources.GetObject("Piece.Image"), System.Drawing.Image)
        Me.Piece.InitialImage = Nothing
        Me.Piece.Location = New System.Drawing.Point(502, 255)
        Me.Piece.Name = "Piece"
        Me.Piece.Size = New System.Drawing.Size(150, 150)
        Me.Piece.TabIndex = 3
        Me.Piece.TabStop = False
        '
        'Checkerboard
        '
        Me.Checkerboard.BackColor = System.Drawing.Color.Transparent
        Me.Checkerboard.Location = New System.Drawing.Point(69, 12)
        Me.Checkerboard.Name = "Checkerboard"
        Me.Checkerboard.Size = New System.Drawing.Size(600, 216)
        Me.Checkerboard.TabIndex = 2
        Me.Checkerboard.TabStop = False
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(303, 328)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'SpeedSetter
        '
        Me.SpeedSetter.LargeChange = 3
        Me.SpeedSetter.Location = New System.Drawing.Point(69, 306)
        Me.SpeedSetter.Maximum = 3
        Me.SpeedSetter.Name = "SpeedSetter"
        Me.SpeedSetter.Size = New System.Drawing.Size(195, 45)
        Me.SpeedSetter.TabIndex = 1
        '
        'Settings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(155, Byte), Integer), CType(CType(155, Byte), Integer), CType(CType(155, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.SpeedSetter)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Piece)
        Me.Controls.Add(Me.Checkerboard)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(816, 489)
        Me.Name = "Settings"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Settings"
        CType(Me.Piece, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Checkerboard, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SpeedSetter, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Piece As PictureBox
    Friend WithEvents Checkerboard As PictureBox
    Friend WithEvents Button1 As Button
    Friend WithEvents SpeedSetter As TrackBar
End Class
