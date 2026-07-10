'Class that allows the user to customise their one-player game of chess.
'Called from the main menu, and is involved in instantiating the Chess form.
Public Class OnePlayerCustomisation

    'Creates opening book (as received from Main Menu).
    Private ReadOnly OpeningBook(50000, 1) As String
    Public Sub New(ByRef InputBook(,) As String)
        InitializeComponent() ' This call is required by the designer.
        OpeningBook(0, 0) = InputBook(0, 0)
        If OpeningBook(0, 0) <> "ERROR" Then
            Dim BookLength As UInt16 = 2 * (Val(InputBook(0, 0)) + 1)
            Array.Copy(InputBook, OpeningBook, BookLength)
        End If
    End Sub

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
        Dim PlayAsWhite As Boolean
        If PosBtn2.Checked AndAlso FENTextBox.Text <> "" Then
            UserStartingFEN = FENTextBox.Text
        Else
            UserStartingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
        End If
        If White.Checked Then 'Will they play as white?
            PlayAsWhite = True
        Else
            PlayAsWhite = False
        End If
        'Instantiates a new game of Chess.
        Dim ChessGame As New Chess(1, UserStartingFEN, DifficultySlider.Value, PlayAsWhite, OpeningBook)
        ChessGame.Show()
    End Sub


    'Code for Radio buttons.
    Private Sub PosBtn1_Checked() Handles PosBtn1.CheckedChanged
        FENTextBox.Enabled = False
    End Sub
    Private Sub PosBtn2_Checked() Handles PosBtn2.CheckedChanged
        FENTextBox.Enabled = True
    End Sub

    'Button that displays information regarding the various AI difficulties.
    Private Sub Button1_Click() Handles InfoBtn.Click
        MsgBox("Beginner: 1s per search, Quiescence off, Opening Book off." & vbCrLf & "Easy: 3s per search, Quiescence off, Opening Book on." & vbCrLf & "Medium: 5s per search, Quiescence off, Opening Book on." & vbCrLf & "Hard: 5s per search, Quiescence on, Opening Book on." & vbCrLf & "Expert: 10s per search, Quiescence on, Opening Book on." & vbCrLf & "Pain: 30s per search, Quiescence on, Opening Book on.", vbInformation + vbApplicationModal, "AI Difficulty Information")
    End Sub
    'Button that takes the user back to the main menu.
    Private Sub BackBtn_Click() Handles BackBtn.Click
        'Locates MainMenu Form.
        Dim Menu As Form = CType(Application.OpenForms("MainMenu"), MainMenu)
        Me.Close()
        Menu.Show()
    End Sub

    'Contols difficulty slider (as well as changing the cursor).
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
        ElseIf DifficultySlider.Value = 5 Then
            AIDiffLabel2.Text = "Expert"
            AIDiffLabel2.ForeColor = Color.DarkRed
        Else
            AIDiffLabel2.Text = "Pain"
            AIDiffLabel2.ForeColor = Color.Black
        End If
    End Sub

End Class