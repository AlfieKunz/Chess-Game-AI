Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

'Class holding all the attributes & methods used in the training modes of my chess program. Part of the Chess form.
Partial Public Class Chess
    Private TrainingTimerTicks As UInt16

    'Below are all the attributes relating to the Puzzle training mode.
    Private PuzzleSampleDatabase As New List(Of PuzzleEntry) 'Puzzle Database, sorted by puzzle rating.
    Private CurrentPuzzleIndex As UInt32 'Represents a pointer in the Database of current puzzle that is being used.
    Private UserPuzzleRating, AIPuzzleRating As Int16 'Represents the current rating of both the user and the AI.
    Private UserPuzzleMode As Boolean = True 'True = the user is playing the puzzle. False = the AI is playing the puzzle.
    Private UserTakenHint As Boolean 'Represents if the user has clicked the 'Hint' button.
    Private NoOfPuzzleMovesComplete As Byte 'Represents the number of moves the user / the AI has successfully played (note that
    'a puzzle is often comprised of multiple moves).
    Private PuzzleTimeForSearch As Decimal 'Represents the total time the user / the AI has to make a single puzzle move (before
    'the puzzle is automatically deemed incorrect).

    'Below are all the attributes relating to the Coordinate & Move training modes.
    Private Const MovesPerPosition As Byte = 3 'A constant referring to the number of moves the user needs to make before
    'a new random position is chosen.
    Private TrainingMovesCompleted As Byte 'Represents the number of moves the user has completed on the position.
    Private MovesInPosition(,) As String 'Represents the full number of legal moves in the position.
    'Leaderboard info for training games: 0 = Index, 1 = Name, 2 = Score, 3 = Date Achieved.
    Private WLeaderBoard(9, 2), BLeaderBoard(9, 2) As String


    'Sets up sound effects.
    Private ReadOnly Sound_321Go As New Media.SoundPlayer With {
    .SoundLocation = Application.StartupPath & "\Sounds\3210Effect.wav"
}
    Private ReadOnly Sound_Correct As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_Correct.wav"
    }
    Private ReadOnly Sound_Incorrect As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_Incorrect.wav"
    }



    'Subroutine which runs every time the TrainingTimer ticks (every 100ms). Used both to determine how long
    'the user has left to complete the puzzle move, and to determine how long the user has left in the
    'move & coordinate training games.
    Private Sub TrainingTimer_Tick() Handles TrainingTimer.Tick
        TrainingTimerTicks += 1
        'Updates Timer.
        If GameMode = 4 Then
            HandlePuzzleModeTimerTick()
        Else
            HandleCoordinateAndMoveModeTimerTick()
        End If
    End Sub



    'Below are all the methods for the puzzle training mode.

    'Subroutine which loads the User & AI puzzle ratings from the PuzzleStats file.
    Public Sub LoadPuzzleRatings()
        Try
            Using SR As New StreamReader(Application.StartupPath & "\Assets\User\PuzzleStats.txt", Encoding.UTF8, True)
                'Ensures that the Ratings are within the general confines of the lowest & highest rated puzzle
                'in the Puzzle databse (50 rating points in both ways).
                UserPuzzleRating = Math.Max(Math.Min(Val(SR.ReadLine()), 3000), 650) '650 = min puzzle rating.
                AIPuzzleRating = Math.Max(Math.Min(Val(SR.ReadLine()), 3000), 650) '3000 = max puzzle rating.
            End Using
        Catch ex As Exception
            'Unable to retrieve the puzzle ratings - reset them to their default values.
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Error when retrieving Puzzle Stats. Resetting...")
            UserPuzzleRating = 1500
            AIPuzzleRating = 1500
        End Try
        Console.ResetColor()
    End Sub


    'Function that retrieves a random puzzle from the database, using the puzzle's rating as an identifier.
    Private Function GetRndPuzzle() As String
        'If the last puzzle has been successfully completed, remove it from the database, so that it
        'won't be tested on the user / the AI again.
        If NextPuzzleBtn.Visible Then PuzzleSampleDatabase.RemoveAt(CurrentPuzzleIndex)

        'Sets the min & max puzzle ratings that can be retrieved from the database.
        Dim LowerRatingBound, UpperRatingBound As UInt32
        Dim LowerBound, UpperBound As UInt32
        If UserPuzzleMode Then
            LowerRatingBound = UserPuzzleRating - 50
            UpperRatingBound = UserPuzzleRating + 50
        Else
            LowerRatingBound = AIPuzzleRating - 50
            UpperRatingBound = AIPuzzleRating + 50
        End If

        'Gets the index of the puzzle in the databse that has the lowest possible rating in the range, along with the
        'index of the puzzle that has the greatest possible rating in the range.
        For n = 0 To PuzzleSampleDatabase.Count - 1
            If LowerRatingBound <= PuzzleSampleDatabase(n).GetRating() Then
                LowerBound = n
                Exit For
            End If
        Next
        For n = LowerBound + 1 To PuzzleSampleDatabase.Count - 1
            If UpperRatingBound < PuzzleSampleDatabase(n).GetRating() Then
                UpperBound = n - 1
                Exit For
            ElseIf n = PuzzleSampleDatabase.Count - 1 Then
                'All puzzles >= LowerBound can be used - set upperbound to the final puzzle in the database.
                UpperBound = n
            End If
        Next

        If LowerBound >= UpperBound Then
            'There does not exist a puzzle in the database that is within the range of the user's / the AI's rating.
            'Provide appropriate error message, and return the user to the Main Menu.
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Unable to retrieve a puzzle. LB = " & LowerBound & ", UB = " & UpperBound & ".")
            Console.ResetColor()
            MsgBox("Error: Unable to retrive a Puzzle from the Database." & vbCrLf & "This likely means that you've exhausted all the puzzles in the sample database, which I didn't even know was possible tbh hahah." & vbCrLf & "To generate new puzzles, please re-enter Puzzle Mode from the Main Menu...", vbCritical + vbOKOnly + vbApplicationModal)
            ExitBtn_Click()
        Else
            'Retrive a random puzzle from the database that resides within the confines of LowerBound and UpperBound.
            Static RNDGen As New Random()
            CurrentPuzzleIndex = RNDGen.Next(LowerBound, UpperBound + 1)
            PuzzleSampleDatabase(CurrentPuzzleIndex).ComputeEntry() 'Fully constructs the entry, so we can perform calculations on it.

            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("New Puzzle:  Index " & CurrentPuzzleIndex & ", with FEN " & PuzzleSampleDatabase(CurrentPuzzleIndex).GetFEN() & ", and rating " & PuzzleSampleDatabase(CurrentPuzzleIndex).GetRating() & ".")
            Console.ResetColor()

            'Updates Labels to reflect this new puzzle, along with it's rating.
            PuzzleRatingLabel.Text = "Puzzle Rating: " & PuzzleSampleDatabase(CurrentPuzzleIndex).GetRating()
            If UserPuzzleMode Then
                RatingLabel.Text = UserPuzzleRating
            Else
                RatingLabel.Text = AIPuzzleRating
            End If
            'Calculates how much rating the user / the AI would gain / lose for getting the puzzle correct / incorrect.
            'Updates the appropriate labels with this information.
            LostRatingLabel.Text = "-" & CStr(CalculateRatingLost(UserPuzzleMode, False))
            GainedRatingLabel.Text = "+" & CStr(CalculateRatingGained(UserPuzzleMode, False))

            Return PuzzleSampleDatabase(CurrentPuzzleIndex).GetFEN()
        End If
    End Function

    'Subroutine that handles the user clicking the NextPuzzle button - handles the generation of a new puzzle,
    'updating the GUI objects, resetting the AI and puzzle timer, prepares the position for the new puzzle,
    'then plays the first move of the puzzle.
    Private Sub NextPuzzleBtn_Click() Handles NextPuzzleBtn.Click
        'Stops the AI from searching on the position, and disables the puzzle timer.
        CancelAIPuzzleSearch()
        ResetPuzzleTimer()
        If AutoAdvanceOnComplete.Checked Then Thread.Sleep(450) 'Allows the correct / incorrect sound to play.

        'Changes the mode from User to AI (or vice versa), if neeeded.
        If HumanModeBtn.Checked <> UserPuzzleMode Then
            If HumanModeBtn.Checked Then
                UserPuzzleMode = True
                ComputerIsSearching = False
                RatingHeader.Text = "Your Rating:"
            Else
                UserPuzzleMode = False
                ComputerIsSearching = True 'Prevents the user from being able to make moves whilst the AI
                'is searching on the puzzle.
                HintBtn.Enabled = False
                RatingHeader.Text = "AI's Rating:"
            End If
        End If

        'Resets the puzzle attributes, and retrieves a new puzzle from the database.
        UserTakenHint = False
        CurrentFEN = GetRndPuzzle()
        NoOfPuzzleMovesComplete = 0
        NextPuzzleBtn.Visible = False

        'Calibrates the board, along with its objects, for use on this new puzzle,
        MasterBoard = SharedAlgorithms.FENConverter(CurrentFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
        'Resets TrueFalse Tables.
        If PlayerTurn Then
            MasterBInCheck.NotInCheck() 'Player is no longer in check.
            SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterEnPassant)
        Else
            MasterWInCheck.NotInCheck() 'Player is no longer in check.
            SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterBInCheck, MasterEnPassant)
        End If
        'Resets location of Previously Used Squares.
        SquareHistory(0, 0) = -1
        SquareHistory(0, 1) = -1
        SquareHistory(1, 0) = -1
        SquareHistory(1, 1) = -1

        'Flips the board so that it will always be positioned in the user's favour.
        If Not (PlayerTurn Xor OrientForWhite) Then FlipBoard() Else Checkerboard.Refresh()
        CheckChecker(False)

        'Resets labels.
        LostRatingLabel.ForeColor = SystemColors.ControlText
        GainedRatingLabel.ForeColor = SystemColors.ControlText
        AIPuzzleInfoLabel.Visible = True

        'Displays the board onto the GUI, then plays the first move of the puzzle.
        DisplayPieces()
        MainAI.ResetTranspositionTable()
        PlayNextPuzzleMove(0, True)
        If UserPuzzleMode Then HintBtn.Enabled = True
        GiveUpBtn.Enabled = True
        GameRunning = True

        RunAIOnPuzzle() 'Runs the AI on the puzzle.
    End Sub


    'Subroutine which plays a specific move of the puzzle on the board, then restarts the puzzle timer.
    Private Sub PlayNextPuzzleMove(ByVal MoveIndex As Byte, ByVal OutputToConsole As Boolean)
        'Retrieves the specific puzzle move (specified by MoveIndex), and plays it on the board.
        Dim NextMove As New Move
        NextMove = (PuzzleSampleDatabase(CurrentPuzzleIndex).GetAllMoves())(MoveIndex)
        AnimateMove(NextMove)

        'Calibrates the board after this move.
        If PlayerTurn Then
            MasterWInCheck.NotInCheck() 'Player is no longer in check.
            MakeMove(MasterBoard, NextMove.OldMoveX, NextMove.OldMoveY, NextMove.NewMoveX, NextMove.NewMoveY, MasterWCanCastle, MasterWKPos, MasterEnPassant, GeneralOptions(0) = "T")
            SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterBInCheck, MasterEnPassant)
        Else
            MasterBInCheck.NotInCheck() 'Player is no longer in check.
            MakeMove(MasterBoard, NextMove.OldMoveX, NextMove.OldMoveY, NextMove.NewMoveX, NextMove.NewMoveY, MasterBCanCastle, MasterBKPos, MasterEnPassant, GeneralOptions(0) = "T")
            SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterEnPassant)
        End If
        SquareHistory(0, 0) = NextMove.NewMoveX
        SquareHistory(0, 1) = NextMove.NewMoveY
        SquareHistory(1, 0) = NextMove.OldMoveX
        SquareHistory(1, 1) = NextMove.OldMoveY
        If Not OrientForWhite Then
            SquareHistory(0, 0) = 7 - SquareHistory(0, 0)
            SquareHistory(0, 1) = 7 - SquareHistory(0, 1)
            SquareHistory(1, 0) = 7 - SquareHistory(1, 0)
            SquareHistory(1, 1) = 7 - SquareHistory(1, 1)
        End If
        Checkerboard.Refresh()
        CheckChecker(False)

        PlayerTurn = Not PlayerTurn
        'Outputs the new FEN to the MoveInput box.
        CurrentFEN = SharedAlgorithms.ConvertToFEN(MasterBoard, MasterWCanCastle, MasterBCanCastle, MasterEnPassant, PlayerTurn)
        FENExport_Click()

        MainAI.Reconfigure(CurrentFEN, False) 'Recalibrates the AI in preparation for the new puzzle.
        EnforceEndStates(True)
        If OutputToConsole Then OutputDebugInfo() 'The board position will not be outputted if, for example, this subroutine
        'has been called by the user clicking the GiveUp button - causing the user's / the AI's puzzle move to be made for them.

        'Starts the puzzle stopwatch.
        If TimeForSearch <> Decimal.MaxValue Then
            TrainingTimerTicks = 0
            TrainingTimer.Enabled = True
            TrainingTimer.Start() 'Starts timer. Every tick = 100ms.
        End If
    End Sub


    'Subroutine that handles the user / the AI playing the correct puzzle move. Gives the user / the AI a rating gain if
    'appropriate, and plays the next move of the puzzle.
    Private Sub PuzzleMoveCompleted()
        NoOfPuzzleMovesComplete += 1
        ResetPuzzleTimer() 'Resets the puzzle timer.
        If NoOfPuzzleMovesComplete * 2 >= PuzzleSampleDatabase(CurrentPuzzleIndex).GetMoveCount() Then
            'The last move of the puzzle has just been played; the puzzle is complete.
            If Not UserPuzzleMode Then AIStopwatch.Stop() : UpdateAIPuzzleInfoLabel(True) 'Stops the AI's search.
            If GeneralOptions(0) = "T" Then Sound_Correct.Play()
            'If the NextPuzzle Button is visible, then the user has had help in completing the puzzle - therefore don't award rating.
            If Not NextPuzzleBtn.Visible Then CalculateRatingGained(UserPuzzleMode, True) : GainedRatingLabel.ForeColor = Color.Green : Application.DoEvents()
            'Stops the user from modifying the board, or from affecting the (now complete) puzzle.
            HintBtn.Enabled = False
            GiveUpBtn.Enabled = False
            GameRunning = False
            If UserPuzzleMode Then AIPuzzleInfoLabel.Visible = False
            If AutoAdvanceOnComplete.Checked Then
                'Moves onto the next puzzle automatically.
                NextPuzzleBtn_Click()
            Else
                NextPuzzleBtn.Visible = True
            End If
        Else
            'Plays the next move of the puzzle, then makes the AI search on the puzzle.
            PlayNextPuzzleMove(NoOfPuzzleMovesComplete * 2, True)
            RunAIOnPuzzle()
        End If
    End Sub



    'Controls for the Hint button. When toggled, the starting square of the piece to move is highlighed to the user,
    'and all other pieces are blocked from being able to move.
    Private Sub HintBtn_Click() Handles HintBtn.Click
        HintBtn.Enabled = False
        UserTakenHint = True

        'Retrives the next (correct) move of the puzzle, and extracts its starting square.
        Dim CorrectMove As New Move
        CorrectMove = PuzzleSampleDatabase(CurrentPuzzleIndex).GetMove(NoOfPuzzleMovesComplete * 2 + 1)
        'Locks this starting piece.
        If OrientForWhite Then
            PieceMoving.LockedPiece = CorrectMove.OldMoveX & CorrectMove.OldMoveY
        Else
            PieceMoving.LockedPiece = (7 - CorrectMove.OldMoveX) & (7 - CorrectMove.OldMoveY)
        End If
        Checkerboard.Refresh()
        'When the user has taken a hint, the potential rating gained is halved. Reflect this on the GainedRating Label.
        GainedRatingLabel.Text = "+" & CStr(CalculateRatingGained(UserPuzzleMode, False))
    End Sub

    'Button that handles either the user clicking the GiveUp Button, or the user / the AI running out of time on the puzzle move.
    Private Sub GiveUpBtn_Click() Handles GiveUpBtn.Click
        If Not PieceIsMoving Then 'Prevents the position from changing if an animation is taking place.
            CancelAIPuzzleSearch()
            If GeneralOptions(0) = "T" Then Sound_Incorrect.Play()
            Thread.Sleep(100) 'Gives the sound enough time to play before the position changes.
            CalculateRatingLost(UserPuzzleMode, GiveUpBtn.Enabled) 'Decrements the user's / the AI's puzzle rating.
            'Note that the user / the AI should not be penelised multiple times per puzzle, so if the GiveUp Button
            'is not enabled (representing that the puzzle is already incorrect), then the rating should not change.
            LostRatingLabel.ForeColor = Color.Red
            GiveUpBtn.Enabled = False
            If UserTakenHint Then PieceMoving.LockedPiece = "" 'Unlocks the Hint Starting Piece.
            'Plays the correct move in the puzzle, then flags the move as being complete.
            PlayNextPuzzleMove(NoOfPuzzleMovesComplete * 2 + 1, False)
            If AutoAdvanceOnComplete.Checked Then
                NextPuzzleBtn_Click() 'Automatically advance to the next puzzle.
            Else
                NextPuzzleBtn.Visible = True
                PuzzleMoveCompleted()
            End If
        End If
    End Sub



    'Functions that calculate the rating gains / losses from getting the current puzzle correct / incorrect.
    Private Function CalculateRatingGained(ByVal UseUserRating As Boolean, ByVal CarryOutOperation As Boolean) As Int16
        Dim DeltaRating As Int16 'Difference in the user's / the AI's rating, and the puzzle's rating.
        If UseUserRating Then
            DeltaRating = PuzzleSampleDatabase(CurrentPuzzleIndex).GetRating() - UserPuzzleRating
        Else
            DeltaRating = PuzzleSampleDatabase(CurrentPuzzleIndex).GetRating() - AIPuzzleRating
        End If
        'Uses a weak exponential function to calculate the change in rating. More rating points are gained
        'for getting difficult puzzles correct, and few points are gained for getting easy puzzles correct.
        CalculateRatingGained = Math.Round((1.02 ^ DeltaRating) * 12 + 3)
        'Halves the potential rating gain if the user has taken a hint.
        If UserTakenHint Then CalculateRatingGained = Math.Truncate(CalculateRatingGained / 2)

        If CarryOutOperation Then
            'Actually give the user / the AI rating. Caps max rating at 3000.
            If UseUserRating Then
                UserPuzzleRating = Math.Min(UserPuzzleRating + CalculateRatingGained, 3000)
            Else
                AIPuzzleRating = Math.Min(AIPuzzleRating + CalculateRatingGained, 3000)
            End If
            'Outputs the user's / the AI's new puzzle rating, then saves them to the PuzzleStats file.
            Console.ForegroundColor = ConsoleColor.Green
            Console.Write("Puzzle correct. Change in Rating = " & CalculateRatingGained & ". New Rating = ")
            If UseUserRating Then Console.WriteLine(UserPuzzleRating) Else Console.WriteLine(AIPuzzleRating)
            Console.ResetColor()
            SavePuzzleRatings()
        End If

        Return CalculateRatingGained
    End Function
    Private Function CalculateRatingLost(ByVal UseUserRating As Boolean, ByVal CarryOutOperation As Boolean) As Int16
        Dim DeltaRating As Int16 'Difference in the user's / the AI's rating, and the puzzle's rating.
        If UseUserRating Then
            DeltaRating = PuzzleSampleDatabase(CurrentPuzzleIndex).GetRating() - UserPuzzleRating
        Else
            DeltaRating = PuzzleSampleDatabase(CurrentPuzzleIndex).GetRating() - AIPuzzleRating
        End If
        'Uses a weak exponential function to calculate the change in rating. More rating points are lost
        'for getting easy puzzles wrong, and few points are lost for getting difficult puzzles wrong.
        CalculateRatingLost = Math.Round((1.02 ^ -DeltaRating) * 12 + 3)

        If CarryOutOperation Then
            'Actually give the user / the AI rating. Caps min rating at 650.
            If UseUserRating Then
                UserPuzzleRating = Math.Max(UserPuzzleRating - CalculateRatingLost, 650)
            Else
                AIPuzzleRating = Math.Max(AIPuzzleRating - CalculateRatingLost, 650)
            End If
            'Outputs the user's / the AI's new puzzle rating, then saves them to the PuzzleStats file.
            Console.ForegroundColor = ConsoleColor.Red
            Console.Write("Puzzle incorrect. Change in Rating = " & CalculateRatingLost & ". New Rating = ")
            If UseUserRating Then Console.WriteLine(UserPuzzleRating) Else Console.WriteLine(AIPuzzleRating)
            Console.ResetColor()
            SavePuzzleRatings()
        End If

        Return CalculateRatingLost
    End Function

    'Subroutine that saves the user's and the AI's puzzle ratings to the PuzzleStats txt file.
    Private Sub SavePuzzleRatings()
        Using SR As New StreamWriter(Application.StartupPath & "\Assets\User\PuzzleStats.txt")
            SR.WriteLine(UserPuzzleRating)
            SR.WriteLine(AIPuzzleRating)
        End Using
    End Sub


    'Subroutines that handle the Puzzle timer.
    Private Sub ResetPuzzleTimer()
        'Stops the Timer from ticking, recalibrates the timer & its objects,
        'and sets how long the user / the AI has to make each puzzle move.
        TrainingTimer.Enabled = False
        ProgressBar.Value = 0
        ProgressBar.ForeColor = Color.FromArgb(0, 192, 0)
        TimeForSearch = PuzzleTimeForSearch
    End Sub
    Private Sub HandlePuzzleModeTimerTick()
        If Math.Max(TimeForSearch - 10, TimeForSearch * 0.6) * 10 <= TrainingTimerTicks Then
            'Only 20% (or 10s, whichever is larger) of the puzzle's time remains - colour the progress bar red to signify.
            If ProgressBar.ForeColor = Color.FromArgb(0, 192, 0) Then ProgressBar.ForeColor = Color.Red
            If TrainingTimerTicks >= TimeForSearch * 10 AndAlso Not PieceIsMoving Then
                'The total time to complete the puzzle move has expired - set the puzzle as being incorrect.
                TrainingTimer.Enabled = False
                'Resets Moved Piece, if the user is currently holding a piece.
                If PieceMoving.IsMovingPiece Then
                    PieceMoving.IsMovingPiece = False
                    PieceMoving.Piece.Location = New Point(PieceMoving.StartPoint)
                    Me.Cursor = Cursors.Default 'Resets cursor.
                    ResetLMS(True)
                End If
                GiveUpBtn_Click()
                Exit Sub
            End If
        End If
        'Increments the Progress bar to reflect how much time has expired.
        ProgressBar.Value = Math.Min(Math.Round((TrainingTimerTicks / TimeForSearch) * 10), 100)
    End Sub



    'Algorithms which handle the AI's controls on the puzzle.
    Private Sub RunAIOnPuzzle()
        Console.WriteLine()
        'Calibrates the AI for the puzzle.
        MainAI.AddBoardHistory(BoardHistory.GetArray())
        CalculateStartingDepth()

        'Creates a new thread for the AI to run on, then starts the AI's search process.
        Dim AIThread As New Task(AddressOf VariableAISearchHandler)
        AIStopwatch.Restart()
        AIThread.Start()
    End Sub

    'Function which tests the AI's current move against the puzzle's correct move. If the function returns True, then
    'the AI will stop its search (due to it getting the move correct, or its time expiring).
    Private Function HandleAIPuzzleGuess() As Boolean
        If MainAI.GetABORTState() Then
            'Time to search on puzzle has expired - stop the search process.
            AIStopwatch.Stop()
            Return True
        Else
            'Checks if the AI's move matches the puzzle's correct move.
            'Fetches the correct puzzle move.
            Dim CorrectMove As New Move
            CorrectMove = PuzzleSampleDatabase(CurrentPuzzleIndex).GetMove(NoOfPuzzleMovesComplete * 2 + 1)
            Console.Write("Correct Puzzle Move: ")
            If Math.Abs(AIBestMove.Score) >= 295 OrElse ((CorrectMove.OldMoveX = AIBestMove.OldMoveX AndAlso CorrectMove.OldMoveY = AIBestMove.OldMoveY) AndAlso (CorrectMove.NewMoveX = AIBestMove.NewMoveX AndAlso CorrectMove.NewMoveY = AIBestMove.NewMoveY)) Then
                'AI's move is correct (or is a checkmating pattern, which we assume is correct; sometimes there are puzzles
                'containing multiple ways to checkmate, but only one method is accepted, so if our AI produces a valid
                'checkmating pattern then we just say that the move is correct).
                If UserPuzzleMode Then AIStopwatch.Stop()
                Console.ForegroundColor = ConsoleColor.Green
                Console.WriteLine("Yes" & vbCrLf)
                Console.ResetColor()
                Me.Invoke(Sub()
                              'Notify to the system that the AI's move is correct - meaning that GUI elements
                              'are updated, and the next puzzle move is played (if AIPuzzleMode is enabled).
                              UpdateAIPuzzleInfoLabel(True)
                              If Not UserPuzzleMode Then
                                  CancelAIPuzzleSearch()
                                  PlayNextPuzzleMove(NoOfPuzzleMovesComplete * 2 + 1, False)
                                  PuzzleMoveCompleted()
                              End If
                          End Sub)
                Return True
            Else
                'AI's move does not match the correct puzzle move - flag as incorrect.
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("No")
                Console.ResetColor()
            End If
        End If
        Return False 'Move is incorrect - continue searching.
    End Function

    Private Sub CancelAIPuzzleSearch()
        'Stops the AI from searching on the position.
        MainAI.ABORTSearch()
        Thread.Sleep(10) 'Gives the AI enough time to ABORT its search.
        'Removes the information about the puzzle from the AI's memory
        MainAI.RemoveBoardHistory(BoardHistory.GetArray())
        MainAI.OutputStatsToFile()
        UpdateAIPuzzleInfoLabel(False)
    End Sub

    'Subroutine which updates the AIPuzzle Information label, so that the user can receive information about the AI's search
    'on the puzzle.
    Private Sub UpdateAIPuzzleInfoLabel(ByVal PuzzleCorrect As Boolean)
        If PuzzleCorrect Then
            AIPuzzleInfoLabel.Text = "AI Solved the Puzzle in " & AIStopwatch.ElapsedMilliseconds.ToString("N0") & "ms."
        Else
            AIPuzzleInfoLabel.Text = "AI is Searching on the Puzzle..."
        End If
    End Sub



    'Subroutines which handle the Auto Advance Options. When the user / the AI has got the puzzle correct / incorrect,
    'the Auto Advance Option will automatically move onto the next puzzle.
    Private Sub AutoAdvanceOnComplete_CheckedChanged() Handles AutoAdvanceOnComplete.CheckedChanged
        'Moves onto the next puzzle if the current puzzle is flagged as being correct / incorrect.
        If AutoAdvanceOnComplete.Checked AndAlso NextPuzzleBtn.Visible Then NextPuzzleBtn_Click()
    End Sub
    Private Sub AutoAdvanceBar_ValueChanged() Handles AutoAdvanceBar.ValueChanged
        'Uses the value of AutoAdvance Bar to output the Puzzle's time to compelete each move to the AutoAdvance Textbox.
        'Also updates the Puzzle's time for search attribute.
        If AutoAdvanceBar.Value = 14 Then
            'Max value = timer is disabled.
            PuzzleTimeForSearch = Decimal.MaxValue
            AutoAdvanceBox.Text = "Never."
        ElseIf AutoAdvanceBar.Value = 0 Then
            PuzzleTimeForSearch = 5
            AutoAdvanceBox.Text = "5 seconds."
        ElseIf AutoAdvanceBar.Value = 1 Then
            PuzzleTimeForSearch = 10
            AutoAdvanceBox.Text = "10 seconds."
        Else
            If AutoAdvanceBar.Value = 13 Then
                PuzzleTimeForSearch = 600 '10 Minutes search time.
            ElseIf AutoAdvanceBar.Value = 12 Then
                PuzzleTimeForSearch = 450 '7.5 Minute search time.
            Else
                'Sets PuzzleTimeForSearch between 1 minute and 5 minutes.
                PuzzleTimeForSearch = (AutoAdvanceBar.Value - 1) * 30
            End If

            'Changes the AutoAdvance Textbox to reflect the new value of PuzzleTimeForSearch.
            If AutoAdvanceBar.Value = 2 Then
                AutoAdvanceBox.Text = "30 seconds."
            ElseIf AutoAdvanceBar.Value = 3 Then
                AutoAdvanceBox.Text = "1 minute."
            Else
                AutoAdvanceBox.Text = PuzzleTimeForSearch / 60 & " minutes."
            End If
        End If
    End Sub

    'Algorithms that change the user's cursor if they are dragging the slider.
    Private Sub AutoAdvanceBar_MouseDown() Handles AutoAdvanceBar.MouseDown
        'Custom cursor - "Open Hand" Pointer used on MacOS.
        'https://support.apple.com/en-gb/guide/mac-help/mh35695/mac
        Me.Cursor = New Cursor(New System.IO.MemoryStream(My.Resources.Cursor16))
    End Sub
    Private Sub AutoAdvanceBar_MouseUp() Handles AutoAdvanceBar.MouseUp
        Me.Cursor = Cursors.Default
    End Sub


    'Subroutine which resets the user's and the AI's puzzle ratings to their default values.
    Private Sub ResetRatingsBtn_Click() Handles ResetRatingsBtn.Click
        'Only resets the ratings if a puzzle is not being completed.
        If NextPuzzleBtn.Visible Then
            'Confirms that the user really does want to reset the ratings.
            If MsgBox("WARNING: You are attempting to reset the puzzle ratings." & vbCrLf & "This will erase both the User Puzzle Rating and the AI Puzzle Rating, and reset them back to their default values of 1500." & vbCrLf & vbCrLf & "THIS CANNOT BE REVERSED." & vbCrLf & vbCrLf & "Would you like to proceed?", vbExclamation + vbYesNo + vbApplicationModal, "Reset Puzzle Ratings") = 6 Then
                'Delete the user puzzle rating file.
                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.WriteLine("Deleting file...")
                Dim UserDirectory As String = Application.StartupPath & "\Assets\User\PuzzleStats.txt"
                If File.Exists(UserDirectory) Then
                    File.Delete(UserDirectory)
                End If
                'Resets the puzzle ratings to their default values, then plays the next move of the puzzle (which will be
                'calibrated for the change in rating).
                LoadPuzzleRatings()
                NextPuzzleBtn_Click()
            End If
        End If
    End Sub








    'Subroutine that retrieves the data from the leaderboard text files, and stores them in their appropriate arrays.
    Private Sub RetrieveLeaderboards()
        Dim TempLine As String
        Dim Counter As Byte
        'Attempts to retrieve White's leaderboard.
        Try
            'Fetches the correct leaderboard (based on the gamemode).
            If GameMode = 5 Then
                FileOpen(1, Application.StartupPath & "\Assets\User\CoordinatePracticeLeaderboardW.txt", OpenMode.Input)
            Else
                FileOpen(1, Application.StartupPath & "\Assets\User\MovePracticeLeaderboardW.txt", OpenMode.Input)
            End If
            While Not EOF(1)
                'Constructs White's leaderboard from the line input (in the format Name*Date Score).
                TempLine = LineInput(1)
                WLeaderBoard(Counter, 0) = TempLine.Substring(0, TempLine.IndexOf("*")) 'Name
                TempLine = TempLine.Substring(TempLine.IndexOf("*") + 1, TempLine.Length - TempLine.IndexOf("*") - 1)
                WLeaderBoard(Counter, 2) = TempLine.Substring(0, TempLine.IndexOf(" ")) 'Score
                TempLine = TempLine.Substring(TempLine.IndexOf(" ") + 1, TempLine.Length - TempLine.IndexOf(" ") - 1)
                WLeaderBoard(Counter, 1) = TempLine.Substring(0, TempLine.Length) 'Date
                Counter += 1
            End While
        Catch ex As Exception 'Unable to retrieve leaderboard.
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Error when retrieving White's Leaderboard data.")
            Console.ResetColor()
        End Try
        FileClose(1)
        'Attempts to retrieve Black's leaderboard (similar code).
        Counter = 0
        Try
            If GameMode = 5 Then
                FileOpen(2, Application.StartupPath & "\Assets\User\CoordinatePracticeLeaderboardB.txt", OpenMode.Input)
            Else
                FileOpen(2, Application.StartupPath & "\Assets\User\MovePracticeLeaderboardB.txt", OpenMode.Input)
            End If
            While Not EOF(2)
                TempLine = LineInput(2)
                BLeaderBoard(Counter, 0) = TempLine.Substring(0, TempLine.IndexOf("*"))
                TempLine = TempLine.Substring(TempLine.IndexOf("*") + 1, TempLine.Length - TempLine.IndexOf("*") - 1)
                BLeaderBoard(Counter, 2) = TempLine.Substring(0, TempLine.IndexOf(" "))
                TempLine = TempLine.Substring(TempLine.IndexOf(" ") + 1, TempLine.Length - TempLine.IndexOf(" ") - 1)
                BLeaderBoard(Counter, 1) = TempLine.Substring(0, TempLine.Length)
                Counter += 1
            End While
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Error when retrieving Black's Leaderboard data.")
            Console.ResetColor()
        End Try
        FileClose(2)

        UpdateLeaderboards()
    End Sub

    'Subroutine that writes the data in the leaderboard arrays to the leaderboard text files.
    Private Sub SaveToLeaderboards(ByVal isWhite As Boolean)
        If isWhite Then
            'Calls the correct file (or creates it if it doesn't exist).
            If GameMode = 5 Then
                FileOpen(1, Application.StartupPath & "\Assets\User\CoordinatePracticeLeaderboardW.txt", OpenMode.Output)
            Else
                FileOpen(1, Application.StartupPath & "\Assets\User\MovePracticeLeaderboardW.txt", OpenMode.Output)
            End If
            'Writes to file in the format "Name*Date Score".
            For x = 0 To 9
                If WLeaderBoard(x, 0) Is Nothing Then Exit For
                PrintLine(1, WLeaderBoard(x, 0) & "*" & WLeaderBoard(x, 2) & " " & WLeaderBoard(x, 1))
            Next
        Else 'Similar code for the black leaderboard..
            If GameMode = 5 Then
                FileOpen(1, Application.StartupPath & "\Assets\User\CoordinatePracticeLeaderboardB.txt", OpenMode.Output)
            Else
                FileOpen(1, Application.StartupPath & "\Assets\User\MovePracticeLeaderboardB.txt", OpenMode.Output)
            End If
            For x = 0 To 9
                If BLeaderBoard(x, 0) Is Nothing Then Exit For
                PrintLine(1, BLeaderBoard(x, 0) & "*" & BLeaderBoard(x, 2) & " " & BLeaderBoard(x, 1))
            Next
        End If
        FileClose(1)
    End Sub

    'Subroutine that updates the DataGridView boxes to match their corresponding leaderboard array (for training modes).
    Private Sub UpdateLeaderboards()
        Dim TempIndex As Byte = 1
        If WLeaderBoard(0, 0) IsNot Nothing Then
            WLeaderBoardGrid.Rows.Clear() 'Deletes leaderboard, then adds the correct data from the array.
            WLeaderBoardGrid.Rows.Add(New String() {1, WLeaderBoard(0, 0), WLeaderBoard(0, 1), WLeaderBoard(0, 2)})
            For x = 1 To 9
                If WLeaderBoard(x, 0) Is Nothing Then Exit For
                If Val(WLeaderBoard(x - 1, 1)) > Val(WLeaderBoard(x, 1)) Then TempIndex = x + 1 'Tie resolved - update Index (represents the score's place on the leaderboard).
                WLeaderBoardGrid.Rows.Add(New String() {TempIndex, WLeaderBoard(x, 0), WLeaderBoard(x, 1), WLeaderBoard(x, 2)})
            Next
            Me.WLeaderBoardGrid.ClearSelection() 'Stops the first item in the leaderboard from being highlighted.
        End If
        'Similar code for black's leaderboard.
        If BLeaderBoard(0, 0) IsNot Nothing Then
            TempIndex = 1
            BLeaderBoardGrid.Rows.Clear()
            BLeaderBoardGrid.Rows.Add(New String() {1, BLeaderBoard(0, 0), BLeaderBoard(0, 1), BLeaderBoard(0, 2)})
            For x = 1 To 9
                If BLeaderBoard(x, 0) Is Nothing Then Exit For
                If Val(BLeaderBoard(x - 1, 1)) > Val(BLeaderBoard(x, 1)) Then TempIndex = x + 1
                BLeaderBoardGrid.Rows.Add(New String() {TempIndex, BLeaderBoard(x, 0), BLeaderBoard(x, 1), BLeaderBoard(x, 2)})
            Next
            Me.BLeaderBoardGrid.ClearSelection()
        End If
    End Sub


    'Subroutine that handles user inputs in the coordinate practice trianing mode.
    Private Sub Checkerboard_Click(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Checkerboard.Click
        If GameMode = 5 AndAlso GameRunning Then
            'Calculates the board square that the user has clicked.
            Dim MouseLocation As Point = Me.PointToClient(MousePosition)
            Dim XLocation As Byte = (MouseLocation.X - Checkerboard.Location.X) \ 75
            Dim YLocation As Byte = (MouseLocation.Y - Checkerboard.Location.Y) \ 75
            If Not OrientForWhite Then XLocation = 7 - XLocation : YLocation = 7 - YLocation
            'Checks if this square matches the objective move.
            If XLocation = Asc(MoveDisplayer.Text(0)) - 97 AndAlso YLocation = 8 - Val(MoveDisplayer.Text(1)) Then
                'Move is correct - increment score and generate new move.
                TrainingScore.Text = CInt(TrainingScore.Text) + 1
                If GeneralOptions(0) = "T" Then Sound_Check.Play()
                While True
                    Dim NewCoordinate As String = Chr(Math.Truncate(Rnd() * 8) + 97) & (Math.Truncate((Rnd() * 8)) + 1)
                    If MoveDisplayer.Text <> NewCoordinate Then MoveDisplayer.Text = NewCoordinate : Exit While
                End While
                ResetLMS(True)
                If OrientForWhite Then LegalMoveSquares(XLocation, YLocation) = True Else LegalMoveSquares(7 - XLocation, 7 - YLocation) = True
                Checkerboard.Refresh()
            End If
        End If
    End Sub

    'Function that returns a random FEN found in the RandomFENs file.
    Private Function RetrieveRandomPosition() As String(,)
        Dim NewFEN As String
        Const FENsInFile As UInt16 = 10000
        Dim NoOfRetries As Byte
        Try
            While True
                'Picks a random line from the file.
                NewFEN = System.IO.File.ReadAllLines(Application.StartupPath & "\Assets\RandomFENs.txt")(Math.Truncate(Rnd() * FENsInFile))
                'Applies that FEN to Masterboard and calibrates board info.
                MasterBoard = SharedAlgorithms.FENConverter(NewFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
                MasterWInCheck.NotInCheck()
                SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterEnPassant)
                'If White is not in check, generate all the legal moves in the position.
                MainAI.Reconfigure(NewFEN, True)
                If Not MasterWInCheck.IsInCheck Then
                    MovesInPosition = MainAI.GetLegalMoves()
                    If MovesInPosition.GetUpperBound(0) >= MovesPerPosition * 3 Then Exit While 'If the number of legal moves in the position is > MovesPerPosition * 3 then the position is valid (lowers chance of moves being repeted).
                End If
                NoOfRetries += 1
                If NoOfRetries = 20 Then Throw New Exception("Too many attempts.")
            End While
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Unable to find a valid Training Position in the databse.")
            Console.ResetColor()
            MsgBox("Error: Unable to retrieve a valid position from the database." & vbCrLf & "Returning to the Main Menu...", vbCritical + vbOKOnly + vbApplicationModal)
            ExitBtn_Click()
        End Try

        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("New Position: " & NewFEN)
        If NoOfRetries > 0 Then Console.ForegroundColor = ConsoleColor.Red : Console.WriteLine("Number of Retries: " & NoOfRetries)
        Console.ResetColor()
        Return MovesInPosition
    End Function

    'Function that retieves a random move in the position (for the Move Training mode).
    Private Function GenerateNewTrainingMove() As String
        Dim TempMove As New Move
        While True
            'Retrieves random move from MovesInPosition, then contructs TempMove using this data.
            Dim RndIndex As Byte = Math.Truncate(Rnd() * (MovesInPosition.GetUpperBound(0) + 1))
            TempMove.OldMoveX = MovesInPosition(RndIndex, 0)(0)
            TempMove.OldMoveY = MovesInPosition(RndIndex, 0)(1)
            TempMove.NewMoveX = MovesInPosition(RndIndex, 1)(0)
            TempMove.NewMoveY = MovesInPosition(RndIndex, 1)(1)
            'Converts this Move to standard chess notation. If this is different to the current move then accept & return it.
            GenerateNewTrainingMove = SharedAlgorithms.MoveConverter(MasterBoard, TempMove, MasterEnPassant)
            If MoveDisplayer.Text <> GenerateNewTrainingMove Then Return GenerateNewTrainingMove
        End While
    End Function



    'Algorithm that begins a training game.
    Private Sub TrainingStart_Click() Handles TrainingStart.Click
        'Prepares timer & objects.
        TrainingTimer.Enabled = False
        If GeneralOptions(0) = "T" Then Sound_321Go.Play() 'Countdown sound.
        ResetLMS(True)
        TrainingScore.Visible = False
        If GameMode = 5 Then
            TimerLabel.Text = "Time Left: 20.0 Seconds"
        Else
            TimerLabel.Text = "Time Left: 30.0 Seconds"
        End If
        TimerLabel.ForeColor = Color.Black
        If OrientForWhite Then
            WLeaderBoardGrid.Visible = False
        Else
            BLeaderBoardGrid.Visible = False
        End If
        Console.WriteLine()
        'Starts countdown.
        MoveDisplayer.Visible = True
        MoveDisplayer.Text = "3"
        Application.DoEvents()
        Thread.Sleep(1000)
        Application.DoEvents()
        FlipperButton.Visible = False
        MoveDisplayer.Text = "2"
        Application.DoEvents()
        Thread.Sleep(1000)
        Application.DoEvents()
        MoveDisplayer.Text = "1"

        If GameMode = 5 Then 'Clears the board.
            For y = 0 To 7
                For x = 0 To 7
                    MasterBoard(x, y) = " "
                Next
            Next
        ElseIf GameMode = 6 Then 'Generates a new position.
            TrainingMovesCompleted = 0
            MovesInPosition = RetrieveRandomPosition()
        End If

        'Starts the game.
        DisplayPieces()
        Application.DoEvents()
        Thread.Sleep(1000)
        TrainingStart.Text = "Reset!"
        If GameMode = 5 Then 'Generates new coordinate.
            MoveDisplayer.Text = Chr(Math.Truncate(Rnd() * 8) + 97) & (Math.Truncate((Rnd() * 8)) + 1)
        Else 'Generates new move.
            MoveDisplayer.Text = GenerateNewTrainingMove()
        End If
        TrainingScore.Text = "0"
        TrainingScore.Visible = True
        GameRunning = True
        TrainingTimerTicks = 0
        TrainingTimer.Enabled = True
        TrainingTimer.Start() 'Starts timer. Every tick = 100ms.
    End Sub

    'Subroutine that handles the Training Mode Timer. Also involves ending games and updating leaderboards accordingly.
    Private Sub HandleCoordinateAndMoveModeTimerTick()
        TimerLabel.Text = "Time Left: " & Math.Round(20 + ((GameMode + 1) Mod 2) * 10 - (TrainingTimerTicks / 10), 1) & " Seconds"
        If TrainingTimerTicks >= 150 + ((GameMode + 1) Mod 2) * 100 Then
            '5 seconds remaining - turn timer red.
            TimerLabel.ForeColor = Color.Red
            If TrainingTimerTicks >= 200 + ((GameMode + 1) Mod 2) * 100 Then
                'Time's up! Notify the user.
                TimerLabel.Text = "Time Left: 0.0 Seconds"
                Application.DoEvents()
                GameRunning = False
                TrainingTimer.Enabled = False
                If GeneralOptions(0) = "T" Then Sound_GameOver.Play()

                If Val(TrainingScore.Text) > 0 AndAlso OrientForWhite AndAlso Val(TrainingScore.Text) > WLeaderBoard(9, 1) OrElse Not OrientForWhite AndAlso Val(TrainingScore.Text) > BLeaderBoard(9, 1) Then
                    'The user's score put them in the top 10 - give the user the option to add their name & score to the leaderboard.
                    Dim NewName As String
                    Thread.Sleep(1000)
                    My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Asterisk) 'Information Popup box sound.
                    While True
                        NewName = InputBox("Congratulations - you're on the leaderboard!" & vbCrLf & vbCrLf & "To save your score, please enter your name in the below box:", "Coordinate Practice")
                        If Not (NewName = "" OrElse NewName = " ") Then 'The user has entered a name.
                            If Regex.IsMatch(NewName, "[A-Za-z ]+") Then 'Only accepts letters & spaces.
                                'The move is valid - add the user's score to the leaderboard.
                                If OrientForWhite Then 'Edits white's leaderboard.
                                    For x = 9 To -1 Step -1
                                        'Finds the right index to place the player's score.
                                        If x = -1 OrElse (WLeaderBoard(x, 0) IsNot Nothing AndAlso Val(TrainingScore.Text) <= WLeaderBoard(x, 1)) Then
                                            For n = 8 To x + 1 Step -1
                                                If WLeaderBoard(n, 0) IsNot Nothing Then
                                                    'Shifts all successive scores one index down (as they have been beaten).
                                                    WLeaderBoard(n + 1, 0) = WLeaderBoard(n, 0)
                                                    WLeaderBoard(n + 1, 1) = WLeaderBoard(n, 1)
                                                    WLeaderBoard(n + 1, 2) = WLeaderBoard(n, 2)
                                                End If
                                            Next
                                            'Puts the score data in the leaderboard.
                                            WLeaderBoard(x + 1, 0) = NewName
                                            WLeaderBoard(x + 1, 1) = Val(TrainingScore.Text)
                                            WLeaderBoard(x + 1, 2) = DateTime.Now.ToString("dd'/'MM'/'yyyy")
                                            Exit For
                                        End If
                                    Next
                                    SaveToLeaderboards(True)
                                Else 'Edits black's leaderboard (similar code).
                                    For x = 9 To -1 Step -1
                                        If x = -1 OrElse (BLeaderBoard(x, 0) IsNot Nothing AndAlso Val(TrainingScore.Text) <= BLeaderBoard(x, 1)) Then
                                            For n = 8 To x + 1 Step -1
                                                If BLeaderBoard(n, 0) IsNot Nothing Then
                                                    BLeaderBoard(n + 1, 0) = BLeaderBoard(n, 0)
                                                    BLeaderBoard(n + 1, 1) = BLeaderBoard(n, 1)
                                                    BLeaderBoard(n + 1, 2) = BLeaderBoard(n, 2)
                                                End If
                                            Next
                                            BLeaderBoard(x + 1, 0) = NewName
                                            BLeaderBoard(x + 1, 1) = Val(TrainingScore.Text)
                                            BLeaderBoard(x + 1, 2) = DateTime.Now.ToString("dd'/'MM'/'yyyy")
                                            Exit For
                                        End If
                                    Next
                                    SaveToLeaderboards(False)
                                End If

                                UpdateLeaderboards()
                                Exit While
                            Else
                                'Invalid entry.
                                If MsgBox("Invalid Entry - Please make sure your input contains only letters and spaces.", vbCritical + vbRetryCancel + vbApplicationModal) = 2 Then
                                    'User has cancelled.
                                    Exit While
                                End If
                            End If
                        Else
                            Exit While
                        End If
                    End While
                End If

                'Resets trianing objects for the next game.
                MoveDisplayer.Visible = False
                TrainingStart.Text = "Start!"
                If GameMode = 5 Then
                    TimerLabel.Text = "Time Left: 20.0 Seconds"
                Else
                    'Stops pieces from moving (bug fix).
                    If PieceMoving.IsMovingPiece Then
                        PieceMoving.IsMovingPiece = False
                        Me.Cursor = Cursors.Default
                    End If
                    TimerLabel.Text = "Time Left: 30.0 Seconds"
                End If
                TrainingScore.Visible = False
                FlipperButton.Visible = True
                If OrientForWhite Then
                    WLeaderBoardGrid.Visible = True
                Else
                    BLeaderBoardGrid.Visible = True
                End If
                TimerLabel.ForeColor = Color.Black
                ResetLMS(True)
                'Resets the board.
                MasterBoard = SharedAlgorithms.FENConverter(StartingFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
                DisplayPieces()
            End If
        End If
    End Sub





    'Button that displays information about the training modes.
    Private Sub InfoBtn_Click() Handles InfoBtn.Click
        If GameMode = 4 Then
            MsgBox("• In this training mode, you will be given a chess position, where the side to move only has one winning move. You will need to find that winning move, and play it on the board." & vbCrLf & "• Getting a puzzle correct will increase your rating, getting it incorrect will decrease your rating." & vbCrLf & "• If you cannot find the move(s), you can either take a hint (which displays the piece to move, but will halve the potential rating gain), or give up to reveal the move." & vbCrLf & "• The AI will search on the position in the background, and (if the toggle is set to AI mode) will automatically play its moves on the board." & vbCrLf & "• Note: when modifying settings, your changes will be implemented on the next move / puzzle." & vbCrLf & "• Please note that you can only reset the puzzle ratings once the current puzzle has been solved / deemed as being incorrect.", vbInformation + vbApplicationModal, "Instructions")
        ElseIf GameMode = 5 Then
            MsgBox("• In this training game, you will be given a chess coordinate (in standard chess notation), and will be tasked with clicking the corresponding square on the chessboard as fast as you can." & vbCrLf & "• If you click the correct square, it will light up in green, and you will be given a new coordinate to click." & vbCrLf & "• Click the most coordinates you can in 20 seconds, and try to get a place on the leaderboard!" & vbCrLf & "• Clicking the 'Flip Board' button will rotate the board 180°, so that each coordinate will be from black's point of view.", vbInformation + vbApplicationModal, "Instructions")
        Else
            MsgBox("• In this training game, you will be given a random chess position along with a possible move (in standard chess notation). You will need to play that move as fast as you can." & vbCrLf & "• If you make the correct move, that square will light up in green, and you will be given a new move to make." & vbCrLf & "• After " & MovesPerPosition & " moves are made in a position, a new random position will be generated." & vbCrLf & "• Complete the most moves you can in 30 seconds, and try to get a place on the leaderboard!" & vbCrLf & "• Clicking the 'Flip Board' button will rotate the board 180°, so that each move will be from black's point of view." & vbCrLf & vbCrLf & "• Note: Regardless of whether the board is flipped or not, only the white pieces will need to be moved.", vbInformation + vbApplicationModal, "Instructions")
        End If
    End Sub

End Class
