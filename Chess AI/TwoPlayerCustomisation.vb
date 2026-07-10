Public Class TwoPlayerCustomisation

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
        If PosBtn2.Checked AndAlso FENTextBox.Text <> "" Then
            UserStartingFEN = FENTextBox.Text
        Else
            UserStartingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
        End If
        Dim ChessGame As New Chess(2, UserStartingFEN, 0, True)
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

    Private Sub PosBtn2_Checked(sender As Object, e As EventArgs)

    End Sub
    Private Sub PosBtn1_Checked(sender As Object, e As EventArgs)

    End Sub
End Class