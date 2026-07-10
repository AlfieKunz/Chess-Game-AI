'Class that allows the user to customise their one-player game of chess.
'Called from the main menu, and is involved in instantiating the Chess form.
Public Class OnePlayerCustomisation

    'Creates opening book (as received from Main Menu) - will be passed onto the Chess form.
    Private ReadOnly OpeningBook As New List(Of OpeningBookEntry)
    Private IsInCustomLayout As Boolean

    Public Sub New(ByRef InputBook As List(Of OpeningBookEntry))
        InitializeComponent() ' This call is required by the designer.
        Randomize()
        If InputBook IsNot Nothing AndAlso InputBook.Count > 0 Then OpeningBook.AddRange(InputBook) Else UseBook.Enabled = False
    End Sub

    'Functionality for the Start Button.
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
        'Controls for a custom starting position (if left blank, assume the standard starting position).
        If PosBtn2.Checked AndAlso FENTextBox.Text <> "" Then
            UserStartingFEN = FENTextBox.Text
        Else
            UserStartingFEN = GlobalConstants.InitialFENPosition
        End If

        'If the user has clicked the "Play as White" radio button, or 50% of the time if they have clicked the
        '"Play as RND", the user will play as White.
        If White.Checked OrElse (RNDSide.Checked AndAlso Math.Round(Rnd()) = 1) Then
            PlayAsWhite = True
        Else
            PlayAsWhite = False
        End If

        'Calibrates the user's AI difficulty settings.
        Dim UserTimeForSearch As Decimal
        Dim UserSearchSettings As New AISearchSettings
        UserSearchSettings.OutputPath = False
        Dim UserAICanSearchOnUsersTurn As Boolean

        If Not UseBook.Checked Then OpeningBook.Clear()
        If DifficultySlider.Value = 1 Then
            'User has selected a custom difficulty - choose their settings based on their options.
            UserTimeForSearch = Math.Max(UserTimeBar.Value / 2, 0.1)
            UserSearchSettings.UseQuiescence = QuiescenceBox.Checked
            UserSearchSettings.UsePieceHeatMaps = PieceHeatMapBox.Checked
            UserAICanSearchOnUsersTurn = AISearchOnUsersTurnBox.Checked
        ElseIf DifficultySlider.Value = 2 Then
            UserTimeForSearch = 1
            UserSearchSettings.UseQuiescence = False
            UserSearchSettings.UsePieceHeatMaps = False
        ElseIf DifficultySlider.Value = 3 Then
            UserTimeForSearch = 3
            UserSearchSettings.UseQuiescence = False
        ElseIf DifficultySlider.Value = 4 Then
            UserTimeForSearch = 3
            UserSearchSettings.UsePieceHeatMaps = False
        ElseIf DifficultySlider.Value = 5 Then
            UserTimeForSearch = 5
        ElseIf DifficultySlider.Value = 6 Then
            UserTimeForSearch = 10
            UserAICanSearchOnUsersTurn = True
        Else
            UserTimeForSearch = 30
            UserAICanSearchOnUsersTurn = True
        End If

        'Initialises a new game of Chess.
        Dim ChessGame As New Chess(1, UserStartingFEN, UserTimeForSearch, UserSearchSettings, UserAICanSearchOnUsersTurn, PlayAsWhite, OpeningBook)
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
        MsgBox("• Beginner: 1s per search, Quiescence off, Piece Heat Maps off." & vbCrLf & "• Easy: 3s per search, Quiescence off, Piece Heat Maps on." & vbCrLf & "• Medium: 3s per search, Quiescence on, Piece Heat Maps off." & vbCrLf & "• Hard: 5s per search, Quiescence on, Piece Heat Maps on." & vbCrLf & "• Expert: 10s per search, Quiescence on, Piece Heat Maps on." & vbCrLf & "• Pain: 30s per search, Quiescence on, Piece Heat Maps on." & vbCrLf & "• (From difficulties 'Expert' and onwards, the AI will search on the position in the background, on the User's turn.)", vbInformation + vbApplicationModal, "AI Difficulty Information")
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

    'Sub that handles the Difficulty slider control - handles custom difficulty modes, along with colouring
    'the AIDiff Label.
    Private Sub DifficultySlider_ValueChanged() Handles DifficultySlider.ValueChanged
        If DifficultySlider.Value = 1 Then
            'The user has entered custom AI difficulty mode.
            IsInCustomLayout = True
            AIDiffLabel2.Text = "Custom"
            AIDiffLabel2.ForeColor = Color.RoyalBlue
            'Modifies the layout of the screen to add these new customisation options, by moving
            'object down.
            Dim Scalar As UInt16 = 155
            Me.Height += Scalar 'Makes the form grow in height.
            Panel2.Top += Scalar
            UseBook.Top += Scalar
            StartBtn.Top += Scalar
            InfoBtn.Top += Scalar
            BackBtn.Top += Scalar

            'Toggles the custom difficulty options.
            UserTimeBox.Visible = True
            UserTimeBar.Visible = True
            QuiescenceBox.Visible = True
            PieceHeatMapBox.Visible = True
            AISearchOnUsersTurnBox.Visible = True

        ElseIf DifficultySlider.Value = 2 Then
            AIDiffLabel2.Text = "Beginner"
            If IsInCustomLayout Then ResetLayout()
            AIDiffLabel2.ForeColor = Color.Green
        ElseIf DifficultySlider.Value = 3 Then
            AIDiffLabel2.Text = "Easy"
            If IsInCustomLayout Then ResetLayout()
            AIDiffLabel2.ForeColor = Color.Goldenrod
        ElseIf DifficultySlider.Value = 4 Then
            AIDiffLabel2.Text = "Medium"
            If IsInCustomLayout Then ResetLayout()
            AIDiffLabel2.ForeColor = Color.DarkOrange
        ElseIf DifficultySlider.Value = 5 Then
            AIDiffLabel2.Text = "Hard"
            If IsInCustomLayout Then ResetLayout()
            AIDiffLabel2.ForeColor = Color.Red
        ElseIf DifficultySlider.Value = 6 Then
            AIDiffLabel2.Text = "Expert"
            If IsInCustomLayout Then ResetLayout()
            AIDiffLabel2.ForeColor = Color.DarkRed
        ElseIf DifficultySlider.Value = 7 Then
            AIDiffLabel2.Text = "Pain"
            If IsInCustomLayout Then ResetLayout()
            AIDiffLabel2.ForeColor = Color.Black
        End If
    End Sub

    'Subroutine that resets the layout of the screen, for when the difficulty slider is not set to 'Custom'.
    Private Sub ResetLayout()
        'Hides the difficulty customisation options.
        UserTimeBox.Visible = False
        UserTimeBar.Visible = False
        QuiescenceBox.Visible = False
        PieceHeatMapBox.Visible = False
        AISearchOnUsersTurnBox.Visible = False

        'Resets all objects back to their initial values.
        Me.Height = 370
        Panel2.Top = 199
        UseBook.Top = 228
        StartBtn.Top = 258
        BackBtn.Top = 296
        InfoBtn.Top = 296
        IsInCustomLayout = False
    End Sub

    'Subroutines that alter the design of the cursor when the user interacts with the TimeBar slider.
    Private Sub UserTimeBar_MouseDown() Handles UserTimeBar.MouseDown
        'Custom cursor - "Open Hand" Pointer used on MacOS.
        'https://support.apple.com/en-gb/guide/mac-help/mh35695/mac
        Me.Cursor = New Cursor(New System.IO.MemoryStream(My.Resources.Cursor16))
    End Sub
    Private Sub UserTimeBar_MouseUp() Handles UserTimeBar.MouseUp
        Me.Cursor = Cursors.Default
    End Sub

    'Subroutine that turns the value of the time slider (in Analysis Mode) into the time for an AI to search.
    'Also updates TimeBox.
    Private Sub UserTimeBar_ValueChanged() Handles UserTimeBar.ValueChanged
        'Updates UserTimeBox.
        UserTimeBox.Text = "Time For Search: " & Math.Max(UserTimeBar.Value / 2, 0.1) & " seconds."
        'Colours UserTimeBox red if the TimeForSearch setting the user has chosen is likely to not be enough time for the AI
        'to compelete a search in.
        If (QuiescenceBox.Checked AndAlso UserTimeBar.Value < 4) OrElse Not QuiescenceBox.Checked AndAlso UserTimeBar.Value < 2 Then
            UserTimeBox.BackColor = Color.LightCoral
        Else
            UserTimeBox.BackColor = Color.WhiteSmoke
        End If
    End Sub

    'Subroutines that control the Quiescence and Piece Heat Map Boxes.
    Private Sub QuiescenceBox_CheckedChanged() Handles QuiescenceBox.CheckedChanged
        'Toggles Quiescence depending on the state of the box.
        If (QuiescenceBox.Checked AndAlso UserTimeBar.Value < 4) OrElse Not QuiescenceBox.Checked AndAlso UserTimeBar.Value < 2 Then
            UserTimeBox.BackColor = Color.LightCoral
        Else
            UserTimeBox.BackColor = Color.WhiteSmoke
        End If
    End Sub

End Class