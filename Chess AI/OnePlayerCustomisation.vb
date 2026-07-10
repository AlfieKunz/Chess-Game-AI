Public Class OnePlayerCustomisation

    Private Sub StartBtn_MouseEnter() Handles StartBtn.MouseEnter
        'Grows the Button.
        StartBtn.Size = New Size(112.5, 50)
        StartBtn.Left -= 11.25
        StartBtn.Top -= 5
        StartBtn.Font = New Font("Microsoft Sans Serif", 21, FontStyle.Bold)
    End Sub
    Private Sub StartBtn_MouseLeave() Handles StartBtn.MouseLeave
        'Shrinks the Button.
        StartBtn.Size = New Size(90, 40)
        StartBtn.Left += 11.25
        StartBtn.Top += 5
        StartBtn.Font = New Font("Microsoft Sans Serif", 16, FontStyle.Bold)
    End Sub
    Private Sub StartBtn_Click() Handles StartBtn.Click
        Dim UserStartingFEN As String
        Dim PlayAsWhite As Boolean
        If PosBtn2.Checked AndAlso FENTextBox.Text <> "" Then
            UserStartingFEN = FENTextBox.Text
        Else
            UserStartingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
        End If
        If White.Checked Then
            PlayAsWhite = True
        Else
            PlayAsWhite = False
        End If
        Dim ChessGame As New Chess(1, UserStartingFEN, DifficultySlider.Value, PlayAsWhite)
        ChessGame.Show()
    End Sub


    Private Sub PosBtn1_Checked() Handles PosBtn1.CheckedChanged
        FENTextBox.Enabled = False
    End Sub
    Private Sub PosBtn2_Checked() Handles PosBtn2.CheckedChanged
        FENTextBox.Enabled = True
    End Sub

    Private Sub BackBtn_Click() Handles BackBtn.Click
        Dim Menu As Form = CType(Application.OpenForms("MainMenu"), MainMenu)
        Me.Close()
        Menu.Show()
    End Sub

    Private Sub DifficultySlider_MouseDown() Handles DifficultySlider.MouseDown
        Me.Cursor = New Cursor(New System.IO.MemoryStream(My.Resources.Cursor16))
    End Sub
    Private Sub DifficultySlider_MouseUp() Handles DifficultySlider.MouseUp
        Me.Cursor = Cursors.Default
    End Sub
    Private Sub DifficultySlider_ValueChanged() Handles DifficultySlider.ValueChanged
        If DifficultySlider.Value = 1 Then
            AIDiffLabel2.Text = "Beginner"
            AIDiffLabel2.ForeColor = Color.Green
        ElseIf DifficultySlider.Value = 2 Then
            AIDiffLabel2.Text = "Easy"
            AIDiffLabel2.ForeColor = Color.Goldenrod
        ElseIf DifficultySlider.Value = 3 Then
            AIDiffLabel2.Text = "Medium"
            AIDiffLabel2.ForeColor = Color.DarkOrange
        ElseIf DifficultySlider.Value = 4 Then
            AIDiffLabel2.Text = "Hard"
            AIDiffLabel2.ForeColor = Color.Red
        Else
            AIDiffLabel2.Text = "Pain"
            AIDiffLabel2.ForeColor = Color.DarkRed
        End If
    End Sub

End Class