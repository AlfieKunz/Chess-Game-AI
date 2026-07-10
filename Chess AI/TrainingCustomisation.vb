Public Class TrainingCustomisation

    Private Sub TrainingCustomisation_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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

    Private Sub PuzzlesBtn_Click(sender As Object, e As EventArgs) Handles PuzzlesBtn.Click
        MsgBox("Coming soon!")
    End Sub

    Private Sub CoorBtn_Click(sender As Object, e As EventArgs) Handles CoorBtn.Click
        Dim ChessGame As New Chess(5, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 0, True, Nothing)
        ChessGame.Show()
    End Sub

    Private Sub MoveBtn_Click(sender As Object, e As EventArgs) Handles MoveBtn.Click
        Dim ChessGame As New Chess(6, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 0, True, Nothing)
        ChessGame.Show()
    End Sub
End Class