Imports System.Runtime
Imports System.Threading

'Class containing all the data for allowing the AI to interact with my Chess System, along with the GUI. Contains methods for initiating an AI search,
'book move handling, multithreading, GUI interactions, etc.
Partial Public Class Chess
    'Data structures that bridge the gap between user and AI.
    Private StartingDepth As SByte 'Estimate of the optimal depth used for searching.
    Private FixedSearchDepth As Byte = 0 'Number representing the fixed depth the AI wi
    'll search to (0 = off).
    Private TimeForSearch As Decimal = 10 'Time the AIs are allowed to search for.
    Private CurrentDepth As Byte 'Highest depth successfully searched to.
    Private CurrentMove As String 'Move of the highest depth successfully searched to.
    Private CurrentEvaluation As String 'Eval of the highest depth successfully searched to.

    Private TerminateSearch As Boolean 'Set to True if the user preemptively aborts the AI search.
    Private ComputerIsSearching As Boolean = False 'Set to True whilst the AI is searching - ensures that the user cannot make moves.
    Private AICanSearchOnUsersTurn, AIIsSearchingOnUsersTurn As Boolean 'Signifies whether or not the AI is able to search on the position on the user's
    'turn in 1-Player Mode. This fills the Transposition Table with useful information regarding the position, so that its search
    'will be faster on its turn.

    Private PieceIsMoving As Boolean 'Represents if the AnimatePiece() method is running.
    Private AIIsLate As Boolean 'Represents whether the AI has used more time than it was originally allocated.


    'Chess AI, along with appropriate objects.
    Private MainAI As AI
    Private SearchSettings As New AISearchSettings() 'User's chosen settings for the AI's search.
    Private AIStopwatch As New Stopwatch 'For timing AI searches.
    Private AIBestMove As Move
    Private AIFinishedSearch As Boolean 'Set to True when a given search has been complete, so that GUI elements can be updated.
    Private InitialMemoryUsage As UInt64 'Variable that keeps track of how much memory the program uses when it boots up (in Bytes).
    'Used to determine how much memory is being used by the AI during its search, for memory management tools.

    Private ReadOnly OpeningBook As New List(Of OpeningBookEntry)





    'Algorithm which calculates the Starting Depth the AI will run at. Takes into account the Total Material Count
    'on the board and calculates Depth using the formula y = -m*ln(x-a) + c (the higher the Material Count, the lower
    'the Starting Depth).
    Private Sub CalculateStartingDepth()
        If FixedSearchDepth = 0 Then
            'Counts the material on the board.
            Dim MaterialArray As UInt16() = SharedAlgorithms.CountMaterial(MasterBoard)
            Dim TotalMaterial As Int16 = (MaterialArray(0) + MaterialArray(1)) / 100
            'TotalMaterial(0) = White's total material, TotalMaterial(1) = Black's total material.
            Dim DepthAlgorithm As Int16 = Math.Truncate(-2 * Math.Log(TotalMaterial + 2, 2.5) + 10 + (Val(SearchSettings.UseQuiescence) + 0.5 * Val(SearchSettings.UsePieceHeatMaps)) * TotalMaterial / 25)

            'If the previous AI search resulted in a forced Checkmate being found, the depth is limited to only the
            'depth that is required to achieve that Checkmate. This saves on a lot of unnecessary processing, as
            'forced Checkmates are unavoidable.
            If CurrentEvaluation <> "-" AndAlso Math.Abs(Val(CurrentEvaluation)) >= 295 Then
                StartingDepth = Math.Min((30000 - (Math.Abs(Val(CurrentEvaluation)) * 100)), CurrentDepth) - 3
            Else
                StartingDepth = DepthAlgorithm
            End If
            'The Math.Max function is used to ensure that any AI's Depth never gets lower than 2. Along with general quality
            'improvements (as a depth of 2 can usually be completed instantly), this also prevents the depth from reaching 1, 0, or
            'negative - all of which would cause errors / invalid results when searching.
            StartingDepth = Math.Max(StartingDepth, 2)
        Else 'StartingDepth = the depth set by the user
            StartingDepth = FixedSearchDepth
        End If
        Console.ForegroundColor = ConsoleColor.Blue
        Console.WriteLine("Starting Depth = " & StartingDepth)
    End Sub

    'Function that uses the binary search to find a given FEN in the opening book.
    Private Function FindPositionInBook(ByVal FEN As String) As UInt32
        Dim LB As Integer = 0 'Lower Bound of Binary Search.
        Dim UB As Integer = OpeningBook.Count - 1 'Upper Bound of Binary Search.
        Dim MidPoint As UInt32
        While LB <= UB
            MidPoint = (UB + LB) \ 2
            If FEN = OpeningBook(MidPoint).GetFEN() Then
                Return MidPoint
            ElseIf FEN > OpeningBook(MidPoint).GetFEN() Then
                LB = MidPoint + 1
            Else
                UB = MidPoint - 1
            End If
        End While
        Return 0 'FEN could not be located in the opening book.
    End Function




    'Subroutine that performs on the current position: returns the best move in the position by either calling the opening book,
    'returning the only move in the position, or calling the AI to search on the position. After calculating its move,
    'the subroutine makes this move on the board.
    Private Sub InitialiseAISystem() Handles AIMoveBtn.Click
        If AIEndlessMode.Checked AndAlso AutoResetter.Checked AndAlso Not GameRunning Then Reset_Btn_Click() 'Enables AI Endless Mode (across multiple games).
        If GameRunning AndAlso Not ComputerIsSearching Then
            If ClickMoveMode Then ClickMoveMode = False : ResetLMS(True)
            Dim BestMove As Move = MainAI.CheckForEndState() 'Ensures that the position is valid, and that the
            'AI will (hopefully) not crash whilst searching on the position.
            Dim PGNMove As String 'Represents the PGN equivilant of the move that the AI makes.
            If BestMove.EndState = "f" OrElse BestMove.EndState = "o" Then
                Dim IndexInBook As UInt32
                If UseBook.Checked Then IndexInBook = FindPositionInBook(CurrentFEN) 'Finds the index of the opening book that the position exists in.
                Console.WriteLine()
                If BestMove.EndState = "o" Then
                    'Move was forced - make move instantly without searching.
                    Console.WriteLine(vbCrLf & "Search Aborted - Only 1 Move in Position.")
                    CurrentAIDepth.Text = "Current Depth: 1"
                    Dim MoveString As String
                    If PlayerTurn Then
                        MoveString = SharedAlgorithms.MoveConverter(MasterBoard, BestMove, True, MasterWKPos, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant), MasterWhiteTFTable)
                    Else
                        MoveString = SharedAlgorithms.MoveConverter(MasterBoard, BestMove, False, MasterBKPos, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant), MasterBlackTFTable)
                    End If
                    CurrentAIMove.Text = "Current Move: " & MoveString & " (FORCED)."
                ElseIf IndexInBook > 0 Then 'Position found in book - play move from book instantly.

                    'Fully instantiates the entry in the opening book, so that we can retrieve its moves.
                    If Not OpeningBook(IndexInBook).GetComputed() Then OpeningBook(IndexInBook).ComputeEntry()
                    Console.ForegroundColor = ConsoleColor.DarkYellow
                    Console.WriteLine(vbCrLf & "Search Aborted - Position Found in Opening Book.")
                    Console.WriteLine("Moves Available: " & OpeningBook(IndexInBook).ReturnAllMoves() & ". Picking random move...")
                    'Locates a random book move from the position, then converts this to a Move (for the AI's purpose).
                    PGNMove = OpeningBook(IndexInBook).ReturnRndMove()
                    If PlayerTurn Then
                        BestMove = SharedAlgorithms.ConvertToMove(PGNMove, MasterBoard, True, SharedAlgorithms.ConvertStringToBitCoor(MasterWKPos), MasterWhiteTFTable)
                    Else
                        BestMove = SharedAlgorithms.ConvertToMove(PGNMove, MasterBoard, False, SharedAlgorithms.ConvertStringToBitCoor(MasterBKPos), MasterBlackTFTable)
                    End If
                    CurrentAIDepth.Text = "Current Depth: 0"
                    CurrentAIMove.Text = "Current Move: " & PGNMove & " (Book)."
                    CurrentAIEval.Text = "Evaluation: -"

                Else
                    'no forced or book move found - call AI.
                    BestMove = InitialiseAI(TimeForSearch >= 0.5)
                    'Converts the AI's move into the PGN format.
                    If PlayerTurn Then
                        PGNMove = SharedAlgorithms.MoveConverter(MasterBoard, BestMove, True, MasterWKPos, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant), MasterWhiteTFTable)
                    Else
                        PGNMove = SharedAlgorithms.MoveConverter(MasterBoard, BestMove, False, MasterBKPos, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant), MasterBlackTFTable)
                    End If
                    'Console.WriteLine("Transposition Table Full-ness: " & MainAI.GetPercentageTranspositionTableFilled() * 100 & "%.")
                End If
                Console.ResetColor()

                If Me.IsHandleCreated Then 'If form is still open...
                    GameRunning = True
                    'Makes the AI's move on the board.
                    AnimateMove(BestMove)
                    CalibrateCoreSystemsForMove(BestMove, GameMode <> 1)
                    MainAI.Reconfigure(CurrentFEN, False) 'Recalibrates AI.

                    'Checks if the new position is in the opening book. If so then output these moves.
                    If UseBook.Checked AndAlso GameMode = 3 AndAlso IndexInBook > 0 Then
                        IndexInBook = FindPositionInBook(CurrentFEN) 'Finds the index of the opening book that the position exists in.
                        Console.ForegroundColor = ConsoleColor.DarkYellow
                        If IndexInBook = 0 Then
                            Console.WriteLine("No Longer in Opening Book.")
                        Else
                            'Retrieves all the moves in the opening book.
                            If Not OpeningBook(IndexInBook).GetComputed() Then OpeningBook(IndexInBook).ComputeEntry()
                            Console.WriteLine("Moves In Opening Book (in Current Position): " & OpeningBook(IndexInBook).ReturnAllMoves() & ".")
                        End If
                        Console.ResetColor()
                    End If

                    'Resets GUI objects & cursor design.
                    Me.Cursor = Cursors.Default
                    UserTimeBar.Enabled = True
                    QuiescenceBox.Enabled = True
                    PieceHeatMapBox.Enabled = True
                    ComputerIsSearching = False
                    Application.DoEvents() 'Allows the GUI to update after the AI's move.
                    Dim NextPositionState As Char = EnforceEndStates(GameMode <> 1) 'Checks for end states found from the AI's move.
                    'As two moves are made in one game 'state', we do not copy BoardHistory to its Buffer (so we don't false-trigger 3FR).
                    'If the player has been put in check, and has not been checkmated, then add the + symbol to the end of the move.
                    If (MasterWInCheck >= 128 OrElse MasterBInCheck >= 128) AndAlso GameRunning Then PGNMove &= "+"
                    BoardHistory.PushPGN(PGNMove) 'Adds this new move to the History of the game.
                    If GameMode < 3 Then
                        If PGNAutoOutputter.Checked Then PGNExport_Click() Else FENExport_Click()
                    End If

                    OutputDebugInfo()

                    'The below lines fix a bug, where the AI wouldn't make a move after the user restarts their game (1P mode only).
                    If AIEndlessMode.Checked OrElse (GameMode = 1 AndAlso CurrentFEN = PreviousFEN AndAlso Not UserPlayer = PlayerTurn) Then
                        Thread.Sleep(1) 'Allows the system to recalibrate (to prevent spam).
                        InitialiseAISystem()
                    Else

                        If GameRunning AndAlso GameMode = 1 AndAlso AICanSearchOnUsersTurn AndAlso IndexInBook = 0 AndAlso NextPositionState = "f" Then
                            'The AI is not in its book, and the new position has multiple moves. Allow it to search on the position in the background.
                            SearchSettings.OutputToConsole = False
                            MainAI.ConfigureSettings(SearchSettings, True)
                            'As the AI's last search has already populated the Transposition Table with entries from depth
                            '= 2 -> CurrentDepth, start searching at CurrentDepth - 1, so that unnecessary work isn't repeated.
                            StartingDepth = Math.Max(CurrentDepth - 1, 2)
                            'Creates a new thread for the AI to run on, then starts the AI's search process.
                            Dim AIThread As New Task(AddressOf VariableAISearchHandler)
                            AIIsSearchingOnUsersTurn = True
                            Console.WriteLine(vbCrLf)

                            AIStopwatch.Restart()
                            AIThread.Start()
                        End If
                    End If

                End If
            End If
        End If
    End Sub

    'Algorithm that creates all the threads & AIs that will search on a position. Handles time management & overtime,
    'interactions with the GUI chessboard, live updates via GUI objects, selecting best moves, and more.
    Private Function InitialiseAI(ByVal UpdateAllGUI As Boolean) As Move
        Dim BestMove As New Move
        'Creates the AI thread (to enable backgound searching / multithreading).
        Dim AIThread As Task
        If FixedSearchDepth = 0 Then
            AIThread = New Task(AddressOf VariableAISearchHandler)
        Else
            AIThread = New Task(AddressOf FixedAISearchHandler)
        End If

        CalculateStartingDepth()
        'Resets GUI objects & changes cursor design.
        Me.Cursor = Cursors.AppStarting
        ProgressBar.ForeColor = Color.FromArgb(0, 192, 0)
        ProgressBar.Value = 0
        AIIsLate = False
        UserTimeBar.Enabled = False
        QuiescenceBox.Enabled = False
        PieceHeatMapBox.Enabled = False
        'Erases the information from the last AI search.
        AIBestMove.EndState = "a"
        CurrentDepth = 2
        CurrentMove = "-"
        CurrentEvaluation = "-"
        TerminateSearch = False
        AITerminator.Visible = True
        AIFinishedSearch = False

        'Prepares the AI for its search.
        MainAI.AddBoardHistory(BoardHistory.GetZobristArray())
        MainAI.ConfigureSettings(SearchSettings, True)

        'Begins the search.
        ComputerIsSearching = True
        Console.ForegroundColor = ConsoleColor.DarkCyan
        Console.Write(vbCrLf & "The AI is now searching..." & vbCrLf & "Search Time = ")
        If TimeForSearch = Decimal.MaxValue Then
            Console.Write("Infinity.")
        Else
            Console.Write(TimeForSearch & " Seconds.")
        End If
        Console.WriteLine(" Quiescence: " & SearchSettings.UseQuiescence & ", Heat Maps: " & SearchSettings.UsePieceHeatMaps & "." & vbCrLf)
        Console.ResetColor()

        GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency 'Relaxes garbage collection during the MiniMax search.
        AIStopwatch.Restart()

        'Starts the AI's thread.
        AIThread.Start()

        'Whilst the AI thread is running...
        While Not AIThread.IsCompleted
            Thread.Sleep(5) 'Allows more processing time to be spent on the AIs.

            If UpdateAllGUI Then Application.DoEvents() 'Allows the user to interact with the GUI (limited).
            'Updates progress bar to reflect time.
            If UpdateAllGUI AndAlso TimeForSearch <> Decimal.MaxValue Then
                If AIIsLate Then
                    '2x speed with red colour.
                    ProgressBar.Value = Math.Min((Math.Max(AIStopwatch.ElapsedMilliseconds - TimeForSearch * 1000, 0) / 5) / TimeForSearch, 100)
                Else 'Normal speed with green colour.
                    ProgressBar.Value = Math.Min(((AIStopwatch.ElapsedMilliseconds / 10) / TimeForSearch), 100)
                End If
            End If

            If TerminateSearch OrElse AIStopwatch.ElapsedMilliseconds / 1000 > TimeForSearch OrElse Not Me.IsHandleCreated Then 'Time's up!
                If AIBestMove.EndState = "a" Then
                    'There has been no move performed in the current amount of time - enable overtime.
                    AIIsLate = True
                    ProgressBar.ForeColor = Color.Red 'Red progress bar - visual indication to user that time is up.
                Else
                    'Force the AI to finish after its current search.
                    MainAI.ABORTSearch()
                End If
                '1.5 * time (or extra 10s) has expired - terminate AI immediately!
                If TerminateSearch OrElse AIStopwatch.ElapsedMilliseconds / 1000 > Math.Min(TimeForSearch * 1.5, TimeForSearch + 10000) OrElse Not Me.IsHandleCreated Then MainAI.ABORTSearch()

            ElseIf AIFinishedSearch Then 'A new depth has been searched to - update GUI controls on latest search & redraw checkerboard.
                If UpdateAllGUI Then
                    AmmendLegalMoveSuares(AIBestMove)
                    Checkerboard.Refresh()
                    If SearchSettings.UseQuiescence Then
                        CurrentAIDepth.Text = "Current Depth: " & CurrentDepth & " - " & MainAI.GetHighestQuiescenceDepth()
                    Else
                        CurrentAIDepth.Text = "Current Depth: " & CurrentDepth
                    End If
                    CurrentAIMove.Text = "Current Move: " & CurrentMove
                    If CurrentEvaluation <> "-" AndAlso Math.Abs(Val(CurrentEvaluation)) >= 295 Then
                        'Mating pattern has been found by AI.
                        CurrentAIEval.Text = "Evaluation: Mate in " & (30000 - (Math.Abs(Val(CurrentEvaluation)) * 100)) \ 2
                    Else
                        CurrentAIEval.Text = "Evaluation: " & CurrentEvaluation
                    End If
                End If
                AIFinishedSearch = False
            End If
        End While

        AIStopwatch.Stop()
        If TerminateSearch Then Console.ForegroundColor = ConsoleColor.Red : Console.WriteLine("Search Terminated Preemptively (by user).")
        GCSettings.LatencyMode = GCLatencyMode.Interactive

        'Select the best move of the highest depth possible (if that search has not been aborted).
        If AIBestMove.EndState <> "a" Then
            BestMove = AIBestMove
        Else 'The AI has not successfully completed a search - start a new search at a depth of 2 (without Quiescence).
            'Sets the result of this new move to be the best move.
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine(vbCrLf & "No Search Completed in Allocated time - Performing Shallow Search...")
            BestMove = MainAI.PerformTestSearch()
            If PlayerTurn Then
                CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, BestMove, True, MasterWKPos, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant), MasterWhiteTFTable)
            Else
                CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, BestMove, False, MasterBKPos, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant), MasterBlackTFTable)
            End If
        End If
        Console.ForegroundColor = ConsoleColor.DarkCyan
        'Outputs time of given search.
        Console.WriteLine(vbCrLf & "Search Complete. Total Time: " & AIStopwatch.Elapsed.TotalMilliseconds.ToString("N0") & " Milliseconds.")

        'Updates GUI labels (containing info on search).
        AITerminator.Visible = False
        TerminateSearch = False

        'Outputs the information of the AI's final search to the GUI labels.
        If SearchSettings.UseQuiescence Then
            CurrentAIDepth.Text = "Current Depth: " & CurrentDepth & " - " & MainAI.GetHighestQuiescenceDepth
        Else
            CurrentAIDepth.Text = "Current Depth: " & CurrentDepth
        End If
        CurrentEvaluation = BestMove.Score
        CurrentAIMove.Text = "Current Move: " & CurrentMove
        If CurrentEvaluation <> "-" AndAlso Math.Abs(Val(CurrentEvaluation)) > 295 Then
            'Mating pattern has been found - update GUI accordingly.
            CurrentAIEval.Text = "Evaluation: Mate in " & (30000 - (Math.Abs(Val(CurrentEvaluation)) * 100)) \ 2
        Else
            CurrentAIEval.Text = "Evaluation: " & CurrentEvaluation
        End If

        'Resets Progress bar.
        ProgressBar.ForeColor = Color.FromArgb(0, 192, 0)
        ProgressBar.Value = 0

        'Asks the AI to update its lifetime stats, then removes its information about the game history
        '(so that we can change that information as needed without the AI returning draw scores via three-fold repetition).
        MainAI.OutputStatsToFile()
        MainAI.RemoveBoardHistory(BoardHistory.GetZobristArray())

        Return BestMove
    End Function



    'Subroutines controlling the AI thread. Variable Search = the AI starts at a depth of StartingDepth, and increments by one after each search (iterative deepening).
    'Fixed Search = the AI only performs one search, but at the specified depth that the user provided.
    Private Sub VariableAISearchHandler()
        Dim AICurrentMove As New Move
        Dim CurrentSearchStopwatch As New Stopwatch
        Dim CurrentAIDepth As Byte = StartingDepth
        Dim MemoryUsage As UInt64
        While CurrentAIDepth < 100
            CurrentSearchStopwatch.Restart()
            'Performs a new search using iterative deepening. For all searches apart from the first, feed the previous search's best
            'move into the new search. This is so that this previous best move can be searched first, resulting in more AlphaBeta prunes.
            If CurrentAIDepth = StartingDepth Then
                AICurrentMove = MainAI.Search(CurrentAIDepth)
            Else
                AICurrentMove = MainAI.Search(CurrentAIDepth, AIBestMove)
            End If
            CurrentSearchStopwatch.Stop()

            If AICurrentMove.EndState = "a" OrElse (MainAI.GetABORTState() AndAlso CurrentAIDepth = StartingDepth) Then
                'If the AI has not completed a single search when its search process is ABORTed, or if then current
                'search did not yield a valid move, do not save the current move, and exit the search process.
                Exit While
            Else
                'Outputs the timings of the current search.
                AIBestMove = AICurrentMove
                Console.ForegroundColor = ConsoleColor.DarkGreen
                Console.Write("Depth Of " & CurrentAIDepth)
                If SearchSettings.UseQuiescence Then Console.Write("-" & MainAI.GetHighestQuiescenceDepth())
                Console.WriteLine(" Completed in: " & AIStopwatch.ElapsedMilliseconds.ToString("N0") & " Milliseconds (" & CurrentSearchStopwatch.ElapsedMilliseconds.ToString("N0") & "ms)")
                Console.ResetColor()


                'Memory Management tool: as the AI searches, the Transposition Table consumes more and more memory.
                'If left, the system will consume too much memory, and will crash. To prevent this,
                'after each branch is searched, we check how much memory is being consumed by the program,
                'and if it exceeds a pre-defined limit, we reset the Transposition Table, to free up memory.
                'We also call a full garbage collection on the system if the memory useage is somewhat high.
                If TimeForSearch = Decimal.MaxValue Then
                    MemoryUsage = Math.Max((Process.GetCurrentProcess()).WorkingSet64 - InitialMemoryUsage, 0)
                    If MemoryUsage >= GlobalConstants.MemoryThreshold Then
                        If MemoryUsage >= 2 * GlobalConstants.MemoryThreshold Then
                            Console.ForegroundColor = ConsoleColor.DarkRed
                            Console.WriteLine("Memory Limit Exceeded (Usage = " & (MemoryUsage / (1024 * 1024)).ToString("N0") & "MB). Resetting Transposition Table to free up Memory...")
                            Console.ResetColor()
                            MainAI.ResetTranspositionTable()
                            MainAI.AddBoardHistory(BoardHistory.GetZobristArray())
                        End If
                        GC.Collect()
                    End If
                End If


                If GameMode = 4 Then
                    If HandleAIPuzzleGuess() Then Exit While
                Else
                    'Edits the GUI for the completed search.
                    If PlayerTurn Then
                        CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, AIBestMove, True, MasterWKPos, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant), MasterWhiteTFTable)
                    Else
                        CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, AIBestMove, False, MasterBKPos, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant), MasterBlackTFTable)
                    End If
                    CurrentEvaluation = AIBestMove.Score
                    CurrentDepth = CurrentAIDepth
                    AIFinishedSearch = True
                    'Search resulted in a checkmate, or the time to search has expired - exit search process.
                    If Math.Abs(AIBestMove.Score) >= 295 OrElse MainAI.GetABORTState() OrElse AIIsLate Then
                        Exit While
                    End If
                End If
                If Not AIIsSearchingOnUsersTurn Then Console.WriteLine()
                CurrentAIDepth += 1

                'If the previous depth took longer than a second to complete, then enable the OutputMoveDebugInfo
                'Search Setting (as there will be less frequent writes to the console.
                If CurrentSearchStopwatch.Elapsed.TotalSeconds >= 1 Then MainAI.EnableMoveDebugInfo()

            End If
        End While
    End Sub
    Private Sub FixedAISearchHandler()
        'Begins the search at the specific depth specified by either the user, or by CalculateStartingDepth().
        AIBestMove = MainAI.Search(StartingDepth)
        If Not (MainAI.GetABORTState() OrElse AIBestMove.EndState = "a") Then
            'Outputs the information of the AI's search time.
            Console.ForegroundColor = ConsoleColor.DarkGreen
            Console.Write("Depth Of " & StartingDepth)
            If SearchSettings.UseQuiescence Then Console.Write("-" & MainAI.GetHighestQuiescenceDepth())
            Console.Write(" Completed in: " & AIStopwatch.ElapsedMilliseconds.ToString("N0") & " Milliseconds." & vbCrLf)
            Console.ResetColor()
            'Updates GUI elements.
            If PlayerTurn Then
                CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, AIBestMove, True, MasterWKPos, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant), MasterWhiteTFTable)
            Else
                CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, AIBestMove, False, MasterBKPos, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant), MasterBlackTFTable)
            End If
            CurrentEvaluation = AIBestMove.Score
            CurrentDepth = StartingDepth

            Dim MemoryUsage As UInt64 = (Process.GetCurrentProcess()).WorkingSet64 - InitialMemoryUsage
            If MemoryUsage >= GlobalConstants.MemoryThreshold Then
                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.WriteLine("Memory Limit Exceeded (Usage = " & (MemoryUsage / (1024 * 1024)).ToString("N0") & "MB). Resetting Transposition Table to free up Memory...")
                Console.ResetColor()
                MainAI.ResetTranspositionTable()
            End If
        End If
    End Sub

End Class
