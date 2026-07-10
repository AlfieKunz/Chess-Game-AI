'Class that allows the user to customise their two-player game of chess.
'Called from the main menu, and is involved in instantiating the Chess form.
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
        'Takes the user to their Chess game, which varies based on their preferences.
        Dim UserStartingFEN As String
        If PosBtn2.Checked AndAlso FENTextBox.Text <> "" Then
            UserStartingFEN = FENTextBox.Text
        Else
            UserStartingFEN = GlobalConstants.StartingFENPosition
        End If
        'Instantiates a new game of Chess.
        Dim ChessGame As New Chess(2, UserStartingFEN)
        ChessGame.Show()
    End Sub


    'Code for Radio buttons.
    Private Sub PosBtn1_Checked() Handles PosBtn1.CheckedChanged
        FENTextBox.Enabled = False
    End Sub
    Private Sub PosBtn2_Checked() Handles PosBtn2.CheckedChanged
        FENTextBox.Enabled = True
    End Sub

    'Button that takes the user back to the main menu.
    Private Sub BackBtn_Click() Handles BackBtn.Click
        'Locates MainMenu Form.
        Dim Menu As Form = CType(Application.OpenForms("MainMenu"), MainMenu)
        Me.Close()
        Menu.Show()
    End Sub

End Class
