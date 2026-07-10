Imports System.IO
Imports System.Text

'Class that holds all the methods referring to GUI controls in the Chess form (ie: buttons, text boxes, sliders, etc).
Partial Public Class Chess

    'Sends the FEN a user types in to be converted & displayed.
    Private Sub InputButton_Click() Handles InputButton.Click
        InputTextBox.Text = InputTextBox.Text.Trim(" ")
        If InputTextBox.Text <> "" AndAlso InputTextBox.Text <> CurrentFEN AndAlso Not ComputerIsSearching Then
            'Checks for first form of validation (if the 2nd letter is "o", then that usually implies that 
            'the TextBox contains an error message.
            If InputTextBox.Text.Length > 1 AndAlso InputTextBox.Text(1) <> "o" Then
                'Performs tests on the input to determine whether the user is inputting a FEN or a series of PGN moves.
                If GameRunning AndAlso (InputTextBox.Text.IndexOf("/") = -1 OrElse (InputTextBox.Text.IndexOf(".") > -1 OrElse (InputTextBox.Text.IndexOf(" ") > 1 AndAlso InputTextBox.Text.IndexOf(" ") < 10))) Then
                    'Can assume that input is a move / moves.
                    EnterMovesIntoSystem(InputTextBox.Text, True)
                ElseIf FENErrorDetection(InputTextBox.Text, True, "") Then 'Is a Valid FEN.
                    If UndoFENChange.Visible = True Then UndoFENChange.Visible = False
                    'Resets Check and Castling Properties.
                    MasterWInCheck.NotInCheck()
                    MasterBInCheck.NotInCheck()
                    Dim TempFEN As String = PreviousFEN
                    'Try displaying the FEN on the board graphically. If this fails, then the FEN is invalid.
                    'In this case, the board is reset to the previous position.
                    Try
                        PreviousFEN = CurrentFEN
                        CurrentFEN = InputTextBox.Text
                        MasterBoard = SharedAlgorithms.FENConverter(InputTextBox.Text, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
                        DisplayPieces()
                    Catch ex As Exception
                        InvalidInput = InputTextBox.Text
                        CurrentFEN = PreviousFEN
                        PreviousFEN = TempFEN
                        MasterBoard = SharedAlgorithms.FENConverter(CurrentFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
                        DisplayPieces()
                        UndoFENChange.Visible = True
                        InputTextBox.Text = "Position Rejected - Invalid FEN Code. Please Input a Genuinine FEN and try again."
                    End Try

                    If UndoFENChange.Visible = False Then 'Therefore FEN is valid.
                        Console.Clear()
                        'Edits location of Previously Used Squares.
                        SquareHistory(2, 0) = SquareHistory(0, 0)
                        SquareHistory(2, 1) = SquareHistory(0, 1)
                        SquareHistory(3, 0) = SquareHistory(1, 0)
                        SquareHistory(3, 1) = SquareHistory(1, 1)
                        SquareHistory(0, 0) = -1
                        SquareHistory(0, 1) = -1
                        SquareHistory(1, 0) = -1
                        SquareHistory(1, 1) = -1
                        If PlayerTurn Xor OrientForWhite Then
                            FlipBoard()
                        Else
                            Checkerboard.Refresh()
                        End If
                        GameRunning = True
                        'Resets TrueFalse Tables, then checks for Checks.
                        If PlayerTurn Then
                            SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterEnPassant)
                        Else
                            SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterBInCheck, MasterEnPassant)
                        End If
                        CheckChecker(False)
                        'Final logical detection of invalid board positions.
                        If Not CheckForInvalidGameStates() AndAlso GeneralOptions(0) = "T" Then Sound_Move.Play()

                        'Recalibrates AI and resets AI move info.
                        MainAI.Reconfigure(CurrentFEN, True)
                        CurrentDepth = 2
                        CurrentMove = "-"
                        CurrentEvaluation = "-"
                        CurrentAIDepth.Text = "Current Depth: -"
                        CurrentAIMove.Text = "Current Move: -"
                        CurrentAIEval.Text = "Evaluation: -"

                        'Resets BoardHistory (as the position is new), then check for end states in the position.
                        BoardHistory.Clear()
                        EnforceEndStates(False)
                        OutputDebugInfo()
                    End If
                End If
            End If
        End If
    End Sub

    'Button which resets the FEN in the InputTextBox, in case it is invalid.
    Private Sub UndoFENChange_Click(sender As Object, e As EventArgs) Handles UndoFENChange.Click
        InputTextBox.Text = InvalidInput
        sender.Visible = False
    End Sub

    'Resets the board to the starting position and sets white to move.
    Private Sub Reset_Btn_Click() Handles Reset_Btn.Click
        If Not (ComputerIsSearching OrElse CurrentFEN = StartingFEN) Then
            PreviousFEN = CurrentFEN
            CurrentFEN = StartingFEN
            'Resets Check Properties.
            MasterWInCheck.NotInCheck()
            MasterBInCheck.NotInCheck()
            'Can assume that the StartingFEN is valid, so we display it graphically.
            MasterBoard = SharedAlgorithms.FENConverter(CurrentFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
            DisplayPieces()
            'Edits location of Previously Used Squares.
            SquareHistory(2, 0) = SquareHistory(0, 0)
            SquareHistory(2, 1) = SquareHistory(0, 1)
            SquareHistory(3, 0) = SquareHistory(1, 0)
            SquareHistory(3, 1) = SquareHistory(1, 1)
            SquareHistory(0, 0) = -1
            SquareHistory(0, 1) = -1
            SquareHistory(1, 0) = -1
            SquareHistory(1, 1) = -1
            Checkerboard.Refresh()
            'Resets TrueFalse Table, then check for Checks.
            If PlayerTurn Then
                SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterEnPassant)
            Else
                SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterBInCheck, MasterEnPassant)
            End If
            CheckChecker(False)
            If (PlayerTurn Xor OrientForWhite) AndAlso Not (GameMode = 1 AndAlso (UserPlayer Xor Not OrientForWhite)) Then FlipBoard()
            GameRunning = True
            If GameMode < 3 Then
                InputTextBox.Text = StartingFEN
                InputTextBox.Select(InputTextBox.Text.Length, 0) 'Stops the TextBox from being highlighted.
            End If

            'Recalibrates AI and resets AI move info.
            MainAI.Reconfigure(CurrentFEN, True)
            CurrentDepth = 2
            CurrentMove = "-"
            CurrentEvaluation = "-"
            CurrentAIDepth.Text = "Current Depth: -"
            CurrentAIMove.Text = "Current Move: -"
            CurrentAIEval.Text = "Evaluation: -"

            'Resets BoardHistory (as the position is new), then check for end states in the position.
            BoardHistory.Clear()
            Console.Clear()
            EnforceEndStates(False)
            OutputDebugInfo()
            If GeneralOptions(0) = "T" Then Sound_Move.Play()
        End If
    End Sub

    'Outputs the current FEN into the FEN textbox.
    Private Sub FENExport_Click() Handles FENExport.Click
        Dim TempFEN As String = CurrentFEN
        'Trims TempFEN to remove its fullmove number, then appends the size of the BoardHistory array
        '(representing how many moves have been made in the position).
        For n = TempFEN.Length - 2 To 0 Step -1
            If TempFEN(n) = " " Then
                TempFEN = TempFEN.Substring(0, n + 1)
                Exit For
            End If
        Next
        TempFEN &= (BoardHistory.GetSize() + 1) \ 2
        InputTextBox.Text = TempFEN
    End Sub

    'Converts the board to the previous FEN Position.
    Private Sub UndoMove_Click() Handles UndoMove.Click
        'If CurrentFEN = PreviousFEN then it is implied that it is the starting position.
        'The user should not be able to undo in this circumstance.
        If Not ComputerIsSearching AndAlso Not (CurrentFEN = PreviousFEN) Then
            Dim TempSH(1, 1) As SByte 'Temp Square History array.
            Dim TempFEN As String = CurrentFEN
            CurrentFEN = PreviousFEN
            PreviousFEN = TempFEN
            MasterWInCheck.NotInCheck()
            MasterBInCheck.NotInCheck()
            'Converts the FEN to a board position, and displays it.
            MasterBoard = SharedAlgorithms.FENConverter(CurrentFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
            DisplayPieces()
            'Swaps location of Previously Used Squares.
            Array.Copy(SquareHistory, TempSH, 4)
            SquareHistory(0, 0) = SquareHistory(2, 0)
            SquareHistory(0, 1) = SquareHistory(2, 1)
            SquareHistory(1, 0) = SquareHistory(3, 0)
            SquareHistory(1, 1) = SquareHistory(3, 1)
            SquareHistory(2, 0) = TempSH(0, 0)
            SquareHistory(2, 1) = TempSH(0, 1)
            SquareHistory(3, 0) = TempSH(1, 0)
            SquareHistory(3, 1) = TempSH(1, 1)
            If AutoFlipper.Checked Then
                FlipBoard()
            Else
                Checkerboard.Refresh() 'This makes the newly-changed Previously used Squares visible to the user.
            End If

            GameRunning = True
            'Resets TrueFalse Tables, then checks for Checks.
            If PlayerTurn Then
                SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterEnPassant)
            Else
                SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterBInCheck, MasterEnPassant)
            End If
            If GeneralOptions(0) = "T" Then Sound_Move.Play()
            CheckChecker(False)
            'Final logical detection of invalid board positions.
            If Not CheckForInvalidGameStates() AndAlso GeneralOptions(0) = "T" Then Sound_Move.Play()

            If GameMode < 3 Then FENExport_Click()
            'Recalibrates AI, then checks for end positions.
            MainAI.Reconfigure(CurrentFEN, False)
            BoardHistory.Swap()
            EnforceEndStates(False)
            OutputDebugInfo()
        End If
    End Sub

    'Subroutine which rotates the board 180 degreees in favour of the other player.
    Private Sub FlipBoard() Handles FlipperButton.Click
        OrientForWhite = Not OrientForWhite
        'Reorientates Previously used squares.
        SquareHistory(0, 0) = 7 - SquareHistory(0, 0)
        SquareHistory(0, 1) = 7 - SquareHistory(0, 1)
        SquareHistory(1, 0) = 7 - SquareHistory(1, 0)
        SquareHistory(1, 1) = 7 - SquareHistory(1, 1)
        SquareHistory(2, 0) = 7 - SquareHistory(2, 0)
        SquareHistory(2, 1) = 7 - SquareHistory(2, 1)
        SquareHistory(3, 0) = 7 - SquareHistory(3, 0)
        SquareHistory(3, 1) = 7 - SquareHistory(3, 1)
        'If the AI is searching, rotate the location of the LegalMoveSqures.
        If ComputerIsSearching Then
            Dim TempVar As Boolean
            For y = 0 To 3
                For x = 0 To 7
                    TempVar = LegalMoveSquares(x, y)
                    LegalMoveSquares(x, y) = LegalMoveSquares(7 - x, 7 - y)
                    LegalMoveSquares(7 - x, 7 - y) = TempVar
                Next
            Next
        End If
        'Displays the new board on the screen and redraws the checkerboard.
        DisplayPieces()
        Checkerboard.Refresh()
        If GameMode >= 5 Then
            'Toggles which training game leaderboard is shown, and stop the visible one from being highlighted.
            If OrientForWhite Then
                BLeaderBoardGrid.Visible = False
                WLeaderBoardGrid.Visible = True
                Me.WLeaderBoardGrid.ClearSelection()
            Else
                WLeaderBoardGrid.Visible = False
                BLeaderBoardGrid.Visible = True
                Me.BLeaderBoardGrid.ClearSelection()
            End If
        End If
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
        'Gives the UserTimeBox a red highlight if the time allocated might not be enough for a search to complete (estimate).
        If (SearchSettings.UseQuiescence AndAlso UserTimeBar.Value < 4) OrElse Not SearchSettings.UseQuiescence AndAlso UserTimeBar.Value < 2 Then
            UserTimeBox.BackColor = Color.LightCoral
        Else
            UserTimeBox.BackColor = Color.WhiteSmoke
        End If
        TimeForSearch = Math.Max(UserTimeBar.Value / 2, 0.1)
    End Sub
    'Subroutine that allows the user to enter in the time for the AI to search - the user's input
    'is reflected on the TimeBar and the TimeBox. Includes error detection.
    Private Sub UserTimeBox_Click() Handles UserTimeBox.Click
        If Not ComputerIsSearching Then
            Dim Temp As String
            Dim UserTime As Decimal
            While True
                Try
                    'Displays text box popup to user, where they can enter their time.
                    Temp = InputBox("Please input how long you want the AI to search for (in seconds):" & vbCrLf & vbCrLf & "Min Time = 0.1s, Max Time = 600s." & vbCrLf & "0 = Infinity (Indefinite Search)", "Time Inputter")
                    If Temp = "" OrElse Temp = " " Then 'Cancel / Exit button was pressed - abort.
                        UserTime = TimeForSearch
                        Exit While
                    End If
                    UserTime = Math.Round(CDec(Temp), 1)
                    If UserTime = 0 OrElse (UserTime >= 0.1 AndAlso UserTime <= 600) Then
                        Exit While 'Inside range - input has passed all the checks :).
                    Else 'Input was outside the given range - displays appropriate message.
                        If MsgBox("Invalid Number - Please make sure your input is in the correct range.", vbCritical + vbRetryCancel + vbApplicationModal) = 2 Then
                            'User has cancelled - revert back to original time.
                            UserTime = TimeForSearch
                            Exit While
                        End If
                    End If
                Catch ex As Exception 'Input was not a decimal / integer number - displays appropriate message.
                    If MsgBox("Invalid Entry - Please make sure that your input is in the correct format.", vbCritical + vbRetryCancel + vbApplicationModal) = 2 Then
                        'User has cancelled - revert back to original time.
                        UserTime = TimeForSearch
                        Exit While
                    End If
                End Try
            End While

            If UserTime = 0 Then UserTime = Decimal.MaxValue
            'Edits GUI objects, and updates TimeForSearch.
            UserTimeBox.Select(UserTimeBox.Text.Length, 0)
            UserTimeBar.Value = 2 * Math.Round(Math.Min(UserTime, 30))
            If UserTime = Decimal.MaxValue Then
                UserTimeBox.Text = "Time For Search: Infinity."
            Else
                UserTimeBox.Text = "Time For Search: " & UserTime & " seconds."
            End If
            TimeForSearch = UserTime
        End If
        Me.ActiveControl = Nothing 'Stops the cursor from appearing in the box.
    End Sub

    'Subroutines that control the Quiescence and Piece Heat Map Boxes.
    Private Sub QuiescenceBox_CheckedChanged() Handles QuiescenceBox.CheckedChanged
        'Toggles Quiescence depending on the state of the box.
        If QuiescenceBox.Checked Then SearchSettings.UseQuiescence = True : Else SearchSettings.UseQuiescence = False
        'Colours UserTimeBox red if the TimeForSearch setting the user has chosen is likely to not be enough time for the AI
        'to compelete a search in.
        If (SearchSettings.UseQuiescence AndAlso UserTimeBar.Value < 4) OrElse Not SearchSettings.UseQuiescence AndAlso UserTimeBar.Value < 2 Then
            UserTimeBox.BackColor = Color.LightCoral
        Else
            UserTimeBox.BackColor = Color.WhiteSmoke
        End If
    End Sub
    Private Sub PieceHeatMapBox_CheckedChanged() Handles PieceHeatMapBox.CheckedChanged
        'Toggles Piece Heat Maps depending on the state of the box.
        If PieceHeatMapBox.Checked Then SearchSettings.UsePieceHeatMaps = True : Else SearchSettings.UsePieceHeatMaps = False
    End Sub

    'Subroutine that terminates the AI search, and displays on the board the AI's current move.
    Private Sub AITerminator_Click() Handles AITerminator.Click
        TerminateSearch = True
    End Sub




    'Subroutine that reveals the settings menu to the user.
    Private Sub SettingsBtn_Click() Handles SettingsBtn.Click
        'Creates and reveals the Settings Form.
        Dim SettingsMenu As New Settings(GameMode <= 3, GameMode < 3, GameMode > 1 AndAlso GameMode <= 3)
        SettingsMenu.Show()
    End Sub

    'Button that returns the user back to the Main Menu. 
    Private Sub ExitBtn_Click() Handles ExitBtn.Click
        If MainAI IsNot Nothing Then MainAI.ABORTSearch()
        If GameMode = 4 Then PuzzleSampleDatabase = Nothing : GC.Collect() 'Clears the puzzle database to save memory.
        'Locates MainMenu Form and shows it.
        Dim Menu As Form = CType(Application.OpenForms("MainMenu"), MainMenu)
        'Closes the settings form, if it exists.
        Dim Settings As Form = CType(Application.OpenForms("Settings"), Settings)
        If Settings IsNot Nothing Then Settings.Close()
        Console.Clear()
        Me.Close() 'Closes the current form.
        Menu.Show()
    End Sub

    'Button that displays the credits information onto the screen (in the form of a pop-up).
    Private Sub Credits_Click() Handles Credits.Click
        Dim CanRetrieveStats As Boolean
        'Loads the Lifetime stats file, then assigns each line to their appropriate variable.
        Dim LifetimePositions, LifetimeTranspositions, LifetimeCheckmates As UInt32
        Try
            Using SR As New StreamReader(Application.StartupPath & "\Assets\AIStats.txt", Encoding.UTF8, True)
                LifetimePositions = SR.ReadLine()
                LifetimeTranspositions = SR.ReadLine()
                LifetimeCheckmates = SR.ReadLine()
            End Using
            CanRetrieveStats = True
        Catch ex As Exception
            'An error occured when reading from the file.
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Unable to retrieve AI lifetime stats.")
            Console.ResetColor()
        End Try

        Dim CreditsMessage As String = Strings.StrDup(10, " ") & "Chess Game & Artificial Intelligence (v8.0)" & vbCrLf & Strings.StrDup(21, " ") & "Created by Alfie Kunz (8158)" & vbCrLf & Strings.StrDup(22, " ") & "of Beckfoot School (37101)" & vbCrLf & "Project used for the AQA GCE Computer Science NEA" & vbCrLf & Strings.StrDup(35, " ") & "(2021 - 2023)"
        If CanRetrieveStats Then
            MsgBox(CreditsMessage & vbCrLf & vbCrLf & vbCrLf & "Lifetime AI Statistics:" & vbCrLf & "Positions Searched: " & LifetimePositions.ToString("N0") & vbCrLf & "Transpositions Found: " & LifetimeTranspositions.ToString("N0") & vbCrLf & "Checkmates Made: " & LifetimeCheckmates.ToString("N0"), vbInformation + vbApplicationModal, "Credits")
        Else
            MsgBox(CreditsMessage, vbInformation + vbApplicationModal, "Credits")
        End If

    End Sub

End Class
