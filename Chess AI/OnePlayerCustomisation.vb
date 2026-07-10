'Class that allows the user to customise their one-player game of chess.
'Called from the main menu, and is involved in instantiating the Chess form.
Imports System.IO

Public Class OnePlayerCustomisation

    'Creates opening book (as received from Main Menu) - will be passed onto the Chess form.
    Private ReadOnly OpeningBook As New List(Of OpeningBookEntry)
    Private IsInCustomLayout As Boolean
    Private RemoteMode As Boolean

    Private AIDiffLabelBufferText As String
    Private AIDiffLabelBufferColour As Color
    Private TimeForSearch As Decimal


    Public Sub New(ByRef InputBook As List(Of OpeningBookEntry))
        InitializeComponent() ' This call is required by the designer.
        Randomize()
        If InputBook IsNot Nothing AndAlso InputBook.Count > 0 Then OpeningBook.AddRange(InputBook) Else UseBook.Enabled = False
        Me.Height = 370
        AIDiffLabel2.Text = "Beginner"
        AIDiffLabel2.ForeColor = Color.Green
    End Sub

    Public Sub New(ByRef InputBook As List(Of OpeningBookEntry), ByVal UserRemoteMode As Boolean)
        Me.New(InputBook)
        If UserRemoteMode Then
            RemoteMode = True
            AIBox.Visible = True
            RNDSide.Visible = False
            White.Checked = True
            AdvancedSearchTimeBox.Checked = True
            White.Left += 47
            Black.Left += 47

            'Allow the user to be able to easily toggle the AdvancedSearchTime, by having it as an option next to the OpeningBook Toggle.
            AdvancedSearchTimeBox.Top -= 92
            AdvancedSearchTimeBox.Visible = True
            Me.Height += 25 'Makes the form grow in height.
            StartBtn.Top += 25
            InfoBtn.Top += 25
            BackBtn.Top += 25
        End If
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

        'If the user has clicked the "Play as White" radio button, or 50% of the time if they have clicked the
        '"Play as RND", the user will play as White.
        If White.Checked OrElse (RNDSide.Checked AndAlso Math.Round(Rnd()) = 1) Then
            PlayAsWhite = True
        Else
            PlayAsWhite = False
        End If

        'Controls for a custom starting position (if left blank, assume the standard starting position).
        If PosBtn2.Checked AndAlso FENTextBox.Text <> "" Then
            UserStartingFEN = FENTextBox.Text
            If RemoteMode Then
                Dim FENPieces As String = UserStartingFEN.Substring(0, UserStartingFEN.IndexOf(" "))
                Dim Pieces() As Char = {"n", "q"}
                If Not PlayAsWhite Then
                    For i = 0 To Pieces.Length - 1
                        Pieces(i) = UCase(Pieces(i))
                    Next
                End If
                For Each Piece In Pieces
                    If FENPieces.IndexOf(Piece) < 0 Then
                        If MsgBox("Error: When using Remote Mode, an enemy Knight & Queen must be Present, to Correctly Handle Pawn Promotions. If you proceed with this FEN, Promotions to Queens will be assumed - do you want to proceed?", vbCritical + vbYesNo + vbApplicationModal) = 7 Then Exit Sub
                        Exit For
                    End If
                Next
            End If
        Else
            UserStartingFEN = GlobalConstants.StartingFENPosition
        End If

        'Calibrates the user's AI difficulty settings.
        Dim UserTimeForSearch As Decimal
        Dim UserSearchSettings As New AISearchSettings With {
            .OutputPath = RemoteMode
        }
        Dim UserAICanSearchOnUsersTurn As Boolean

        If Not UseBook.Checked Then OpeningBook.Clear()
        If DifficultySlider.Value = 1 Then
            'User has selected a custom difficulty - choose their settings based on their options.
            UserTimeForSearch = TimeForSearch
            UserSearchSettings.UseQuiescence = QuiescenceBox.Checked
            UserSearchSettings.UsePieceHeatMaps = PieceHeatMapBox.Checked
            UserAICanSearchOnUsersTurn = AISearchOnUsersTurnBox.Checked
        ElseIf DifficultySlider.Value = 2 Then
            'Beginner AI. 0.1s per search. Makes the AI as bad as possible...
            UserTimeForSearch = 0.1
            UserSearchSettings.UseQuiescence = False
            UserSearchSettings.UsePieceHeatMaps = False
            UserSearchSettings.UseTranspositionTable = False
            UserSearchSettings.StableSearch = True
            UserSearchSettings.NullMoveRValue = Int16.MaxValue
            UserSearchSettings.AspirationWindowWidth = 0
            UserSearchSettings.UseBitMasks = False
            UserSearchSettings.UsePVS = False
        ElseIf DifficultySlider.Value = 3 Then
            'Easy AI. 0.5s per search. Same as Beginner; with PieceHeatMaps & Null Move Pruning turned on.
            UserTimeForSearch = 0.5
            UserSearchSettings.UseQuiescence = False
            UserSearchSettings.UseTranspositionTable = False
            UserSearchSettings.StableSearch = True
            UserSearchSettings.AspirationWindowWidth = 0
            UserSearchSettings.UseBitMasks = False
            UserSearchSettings.UsePVS = False
        ElseIf DifficultySlider.Value = 4 Then
            'Medium AI. 1s per search. Same as Beginner; with Quiescence, TranspositionTable & Aspiration Windows turned on.
            UserTimeForSearch = 1
            UserSearchSettings.UsePieceHeatMaps = False
            UserSearchSettings.StableSearch = True
            UserSearchSettings.NullMoveRValue = Int16.MaxValue
            UserSearchSettings.UseBitMasks = False
            UserSearchSettings.UsePVS = False
        ElseIf DifficultySlider.Value = 5 Then
            'Hard AI. 2s per search. Same as Medium; with PieceHeatMaps and Null-Pruning turned on.
            UserTimeForSearch = 2
            UserSearchSettings.StableSearch = True
            UserSearchSettings.UseBitMasks = False
            UserSearchSettings.UsePVS = False
        ElseIf DifficultySlider.Value = 6 Then
            'Expert AI. 5s per search. Same as Hard; with Dynamic Depths, Pawn Bitboards, PVS and Opponent Thinking Time turned on.
            UserTimeForSearch = 5
            UserAICanSearchOnUsersTurn = True
        Else 'Pain AI. Same as Expert, but with 6x thinking time.
            UserTimeForSearch = 30
            UserAICanSearchOnUsersTurn = True
        End If

        'Initialises a new game of Chess.
        Dim ChessGame As Chess
        If RemoteMode Then
            If AIBox.Checked Then
                ChessGame = New Chess(True, UserStartingFEN, PlayAsWhite, OpeningBook, UserTimeForSearch, UserSearchSettings, UserAICanSearchOnUsersTurn, AdvancedSearchTimeBox.Checked)
            Else
                ChessGame = New Chess(True, UserStartingFEN, PlayAsWhite, OpeningBook)
            End If
        Else
            ChessGame = New Chess(1, UserStartingFEN, UserTimeForSearch, UserSearchSettings, UserAICanSearchOnUsersTurn, AdvancedSearchTimeBox.Checked, PlayAsWhite, OpeningBook)
        End If
        ChessGame.Show()
    End Sub


    'Code for Radio buttons.
    Private Sub PosBtn1_Checked() Handles PosBtn1.CheckedChanged
        FENTextBox.Enabled = False
    End Sub
    Private Sub PosBtn2_Checked() Handles PosBtn2.CheckedChanged
        FENTextBox.Enabled = True
    End Sub



    Private Sub AIBox_CheckedChanged() Handles AIBox.CheckedChanged
        If AIBox.Checked Then
            AIDiffLabel2.Text = AIDiffLabelBufferText
            AIDiffLabel2.ForeColor = AIDiffLabelBufferColour
            If DifficultySlider.Value = 1 Then MoveLayoutDown()
        Else
            AIDiffLabelBufferText = AIDiffLabel2.Text
            AIDiffLabelBufferColour = AIDiffLabel2.ForeColor
            AIDiffLabel2.Text = "N/A"
            AIDiffLabel2.ForeColor = Color.Black
            If DifficultySlider.Value = 1 Then ResetLayout()
        End If
        DifficultySlider.Enabled = AIBox.Checked
        If OpeningBook IsNot Nothing Then UseBook.Enabled = AIBox.Checked
    End Sub


    'Button that displays information regarding the various AI difficulties.
    Private Sub InfoBtn_Click() Handles InfoBtn.Click
        MsgBox("• Beginner: 0.1s per search. Turns off Quiescence, PieceHeatMaps, TranspositionTable, Dynamic Depths, and Null-Pruning." & vbCrLf &
               "• Easy: 0.5s per search. Same as Beginner; with PieceHeatMaps turned on." & vbCrLf &
               "• Medium: 1s per search. Same as Beginner; with Quiescence and TranspositionTable turned on." & vbCrLf &
               "• Hard: 2s per search. Same as Medium; with PieceHeatMaps and Null-Pruning turned on." & vbCrLf &
               "• Expert: 5s per search. Same as Hard; with Dynamic Depths, Pawn Bitboards, and Opponent Thinking Time turned on." & vbCrLf &
               "• Pain: Same as Expert, but with 'Deep Search' Mode on - 30s per search (good luck ;D).",
               vbInformation + vbApplicationModal, "AI Difficulty Information")
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
            MoveLayoutDown()
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


    'Modifies the layout of the screen to add these new customisation options, by moving
    'object down.
    Private Sub MoveLayoutDown()
        MoveBottomControlsDown(If(RemoteMode, 155, 180))
        'Toggles the custom difficulty options.
        UserTimeBox.Visible = True
        UserTimeBar.Visible = True
        QuiescenceBox.Visible = True
        PieceHeatMapBox.Visible = True
        AISearchOnUsersTurnBox.Visible = True
        If Not RemoteMode Then AdvancedSearchTimeBox.Visible = True
    End Sub
    Private Sub MoveBottomControlsDown(ByVal Pixels As Integer)
        Me.Height += Pixels 'Makes the form grow in height.
        Panel2.Top += Pixels
        UseBook.Top += Pixels
        StartBtn.Top += Pixels
        InfoBtn.Top += Pixels
        BackBtn.Top += Pixels
        If RemoteMode Then AdvancedSearchTimeBox.Top += Pixels
    End Sub

    'Subroutine that resets the layout of the screen, for when the difficulty slider is not set to 'Custom'.
    Private Sub ResetLayout()
        'Hides the difficulty customisation options.
        UserTimeBox.Visible = False
        UserTimeBar.Visible = False
        QuiescenceBox.Visible = False
        PieceHeatMapBox.Visible = False
        AISearchOnUsersTurnBox.Visible = False
        If Not RemoteMode Then AdvancedSearchTimeBox.Visible = False

        'Resets all objects back to their initial values.
        MoveBottomControlsDown(-If(RemoteMode, 155, 180))
        'Default Values:
        'Me.Height = 370
        'Panel2.Top = 199
        'UseBook.Top = 228
        'StartBtn.Top = 258
        'BackBtn.Top = 296
        'InfoBtn.Top = 296
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
        Select Case UserTimeBar.Value
            Case 0 'Lowest Possible Time For Search.
                TimeForSearch = 0.1
            Case < 20 'Between 0.25 and 5 seconds, increment in 0.25 seconds.
                TimeForSearch = UserTimeBar.Value / 4
            Case < 50 'Between 5 and 20 seconds, increment in 0.5 seconds.
                TimeForSearch = (UserTimeBar.Value - 10) / 2
            Case Else 'Between 20 and 30 seconds, increment in 1 second.
                TimeForSearch = UserTimeBar.Value - 30
        End Select
        UserTimeBox.Text = $"Time For Search: {TimeForSearch} seconds."

        'Colours UserTimeBox red if the TimeForSearch setting the user has chosen is likely to not be enough time for the AI
        'to compelete a search in.
        ChangeUserTimeBarBackColour()
    End Sub

    'Subroutines that control the Quiescence and Piece Heat Map Boxes.
    Private Sub ChangeUserTimeBarBackColour() Handles QuiescenceBox.CheckedChanged, PieceHeatMapBox.CheckedChanged
        Dim ThresholdValue As Integer = 1
        If QuiescenceBox.Checked Then ThresholdValue += 3
        If PieceHeatMapBox.Checked Then ThresholdValue += 2
        'Gives the UserTimeBox a red highlight if the time allocated might not be enough for a search to complete (estimate).
        UserTimeBox.BackColor = If(UserTimeBar.Value < Math.Min(ThresholdValue, 5), Color.LightCoral, Color.WhiteSmoke)
    End Sub

End Class
