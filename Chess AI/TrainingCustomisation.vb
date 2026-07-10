'Form that handles the various training options in my program. Responcible from taking the user from the MainMenu to the Chess form.
Public Class TrainingCustomisation

    Private Sub TrainingCustomisation_Load() Handles MyBase.Load
        'Calibrates buttons.
        PuzzlesBtn.FlatAppearance.BorderSize = 0
        CoorBtn.FlatAppearance.BorderSize = 0
        MoveBtn.FlatAppearance.BorderSize = 0
    End Sub


    'Button that takes the user back to the main menu.
    Private Sub BackBtn_Click() Handles BackBtn.Click
        'Locates MainMenu Form.
        Dim Menu As Form = CType(Application.OpenForms("MainMenu"), MainMenu)
        Me.Close()
        Menu.Show()
    End Sub

    'Subroutines that handle the user clicking on each of the training modes - instantiating a valid Chess form.
    Private Sub PuzzlesBtn_Click() Handles PuzzlesBtn.Click
        Dim ChessGame As New Chess(4)
        'If the puzzle database cannot be retrieved, the Chess form will dispose itself, and the user should be taken back to the main menu.
        If ChessGame.IsDisposed Then
            Me.Close()
        Else
            ChessGame.Show()
        End If
    End Sub

    Private Sub CoorBtn_Click() Handles CoorBtn.Click
        Dim ChessGame As New Chess(5)
        ChessGame.Show()
    End Sub

    Private Sub MoveBtn_Click() Handles MoveBtn.Click
        Dim ChessGame As New Chess(6)
        ChessGame.Show()
    End Sub
End Class