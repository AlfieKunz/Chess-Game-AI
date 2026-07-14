Imports System.Diagnostics.Eventing.Reader
Imports System.Reflection
Imports System.Reflection.Metadata
Imports System.Runtime
Imports System.Threading

'Class containing all the data for allowing the AI to interact with my Chess System, along with the GUI. Contains methods for initiating an AI search,
'book move handling, multithreading, GUI interactions, etc.
Partial Public Class Chess 'AI Handles

    'Chess AI, along with appropriate objects.
    Private MainAI As AI
    Private SearchSettings As New AISearchSettings() 'User's chosen settings for the AI's search.
    Private AISettingsFields As FieldInfo() 'Defines all the different fields in the AISearchSettings class, so the user can chanage them easily.

    'Data structures that bridge the gap between user and AI.
    Public Structure AIHandleInfo
        Public StartingDepth As Integer 'Estimate of the optimal depth used for searching.
        Public FixedSearchDepth As Byte 'Number representing the fixed depth the AI will search to (0 = off).
        Public TimeForSearch As Decimal 'Time the AI is allowed to search for.
        Public AdvancedTimeForSearch As Decimal 'Buffer that holds the original TimeForSearch, when the user is using 'Advanced Search Time' Mode.
        Public JustMadePremove As Boolean

        Public CurrentDepth As Integer 'Highest depth successfully searched to.
        Public CurrentMove As String 'Move of the highest depth successfully searched to.
        Public CurrentEvaluation As String 'Eval of the highest depth successfully searched to.

        Public AIIsLate As Boolean 'Represents whether the AI has used more time than it was originally allocated.
        Public AIPanicMode As SByte 'If the AI is forced to search for longer, or less time, on a position, we set this variable to 1 / 2.
        'This allows the GUI to update the progress bar, and then set this to -1 / -2 for safe keeping. Is reset after every move.

        Public AIStopwatch As Stopwatch 'For timing AI searches.
        Public AIBestMove As Move
        Public AIFinishedSearch As Boolean 'Set to True when a given search has been complete, so that GUI elements can be updated.
        Public InitialMemoryUsage As UInt64 'Variable that keeps track of how much memory the program uses when it boots up (in Bytes).
        'Used to determine how much memory is being used by the AI during its search, for memory management tools.

        Public FENToResetTo As String 'Represents if the user has activated the AI before making a move on the board (resetting the board
        'or changing the FEN resets this to the Starting Position). Used for knowing what position to reset to for AI AutoResetter.

        Public MoveWasAIPredicted As Boolean 'Was the last move entered into the system the move predicted by the Transposition Table of the AI?
    End Structure

    Private AIHandles As New AIHandleInfo With {
    .FixedSearchDepth = 0,
    .TimeForSearch = 10,
    .AIStopwatch = New Stopwatch
    }

    Private TerminateSearch As Boolean 'Set to True if the user preemptively aborts the AI search.
    Private ComputerIsSearching As Boolean = False 'Set to True whilst the AI is searching - ensures that the user cannot make moves.
    Private AICanSearchOnUsersTurn, AIIsSearchingOnUsersTurn As Boolean 'Signifies whether or not the AI is able to search on the position on the user's
    'turn in 1-Player Mode. This fills the Transposition Table with useful information regarding the position, so that its search
    'will be faster on its turn.





    'Algorithm which calculates the Starting Depth the AI will run at. Takes into account the Total Material Count
    'on the board and calculates Depth using the formula y = -m*ln(x-a) + c (the higher the Material Count, the lower
    'the Starting Depth).
    Private Sub CalculateAIStartingDepth()
        If AIHandles.FixedSearchDepth = 0 Then
            'Counts the material on the board.
            Dim MaterialArray As Integer() = Helper.CountMaterial(MasterBoard)
            Dim TotalMaterial As Integer = (MaterialArray(0) + MaterialArray(1)) \ 100
            'TotalMaterial(0) = White's total material, TotalMaterial(1) = Black's total material.
            Dim DepthAlgorithm As Integer = Math.Truncate(-2 * Math.Log(TotalMaterial + 2, 2.5) + 10 + (Val(SearchSettings.UseQuiescence) + 0.5 * Val(SearchSettings.UsePieceHeatMaps)) * TotalMaterial / 25)

            'If the previous AI search resulted in a forced Checkmate being found, the depth is limited to only the
            'depth that is required to achieve that Checkmate. This saves on a lot of unnecessary processing, as
            'forced Checkmates are unavoidable.
            If AIHandles.CurrentEvaluation <> "-" AndAlso Math.Abs(Val(AIHandles.CurrentEvaluation)) >= 295 Then
                AIHandles.StartingDepth = Math.Min((30000 - (Math.Abs(Val(AIHandles.CurrentEvaluation)) * 100)), AIHandles.CurrentDepth) - 4
            Else
                AIHandles.StartingDepth = DepthAlgorithm
            End If
            'The Math.Max function is used to ensure that any AI's Depth never gets lower than 2. Along with general quality
            'improvements (as a depth of 2 can usually be completed instantly), this also prevents the depth from reaching 1, 0, or
            'negative - all of which would cause errors / invalid results when searching.
            AIHandles.StartingDepth = Math.Max(AIHandles.StartingDepth, 2)
        Else 'AIHandles.StartingDepth = the depth set by the user
            AIHandles.StartingDepth = AIHandles.FixedSearchDepth
        End If
        Console.ForegroundColor = ConsoleColor.Blue
        Console.WriteLine("Starting Depth = " & AIHandles.StartingDepth)
    End Sub


    'Algorithm that sets the TimeForSearch if the user is using the 'Advanced Search Time' Mode. This sets the search time to be a random,
    'human-like, and intelligent value that is representative of the position & AI data.
    Private Sub SetAdvancedSearchTime()
        If Not (AIHandles.AdvancedTimeForSearch = 0 OrElse AIHandles.TimeForSearch = Decimal.MaxValue) Then
            Static RND As New Random()
            Dim DeltaTime As Double = AIHandles.AdvancedTimeForSearch * 0.66
            Dim RNDValue As Double = RND.NextDouble()
            Dim NewTime As Double
            Select Case RNDValue
                Case < 0.1
                    'Really short think (~10x faster).
                    RNDValue += 9
                    NewTime = AIHandles.AdvancedTimeForSearch / RNDValue
                Case > 0.9
                    'Really deep think (~5x slower).
                    RNDValue += 4
                    NewTime = AIHandles.AdvancedTimeForSearch * RNDValue
                Case Else
                    NewTime = RNDValue * 2 * DeltaTime + AIHandles.AdvancedTimeForSearch - DeltaTime
                    'Extends the search time if the user has a lot of possible moves (and vice versa).
                    Dim NoLegalMoves As Integer = MainAI.GetNoOfLegalMoves()
                    NewTime *= 0.75 * Math.Log10(NoLegalMoves + 5)
                    'Gives the AI longer to think on the moves after pre-moves (ie: book / forced moves), to get the TranspositionTable better calibrated.
                    If AIHandles.JustMadePremove Then NewTime *= 1.2
            End Select

            'Give the AI longer to think if the position is complex, ie lots of captures and checks available.
            NewTime *= (MainAI.NumCapturesThreatsInBasePos * MainAI.NumCapturesThreatsInBasePos / 70) + 0.75
            'If the move that the user just made was in the AI's last PV line (ie: the move stored in the AI's TT for the previous position), 
            'our opponent is presumed to be playing optimally - give the AI more time.
            If AIHandles.MoveWasAIPredicted Then NewTime *= 1.5

            AIHandles.TimeForSearch = Math.Round(Math.Max(NewTime, 0.05), 2) 'Caps the search time at 50ms.
            AIHandles.JustMadePremove = False
        End If
    End Sub

    Private Sub WaitAdvancedSearchPremoveTime(ByVal Code As Char)
        If AIHandles.AdvancedTimeForSearch >= 0.5 Then
            Static RND As New Random()
            Dim RNDValue As Double = RND.NextDouble()
            Dim WeightedRNDValue As Double = RNDValue * RNDValue * Math.Min(AIHandles.AdvancedTimeForSearch / 4, 2)
            If Code = "b" Then
                Dim PremoveCount As UInt16 = (BoardHistory.GetSize() + 1US) \ 2US
                If PremoveCount <= 1 Then WeightedRNDValue *= 0.2 Else WeightedRNDValue += 0.1 * PremoveCount
            ElseIf Code = "o" Then
                WeightedRNDValue *= 0.2
            End If
            If WeightedRNDValue >= 0.1 Then
                Dim WaitTimer As New Stopwatch
                WaitTimer.Start()
                While WaitTimer.ElapsedMilliseconds <= WeightedRNDValue * 1000 AndAlso Not TerminateSearch
                    Thread.Sleep(5)
                    Application.DoEvents()
                End While
                WaitTimer.Stop()
            End If
        End If
        AIHandles.JustMadePremove = True
    End Sub


    'Function that uses the binary search to find a given FEN in the opening book.
    Private Function FindPositionInBook(ByVal FEN As String) As UInt32
        'Note that the FEN can very often have its full & have move count different, and in the opening book, _all_ FENs end in 0 1.
        FEN = Helper.StripFENOfMoveCounts(FEN) & " 0 1"
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
        If AIEndlessMode.Checked AndAlso AutoResetter.Checked AndAlso Not GameRunning Then AcceptFENIntoSystem(AIHandles.FENToResetTo, False) 'Enables AI Endless Mode (across multiple games).
        If GameRunning AndAlso (Not ComputerIsSearching OrElse AIEndlessMode.Checked) Then
            If ClickMoveMode Then ClickMoveMode = False : ResetLMS(True)
            Dim BestMove As Move = MainAI.CheckForEndState() 'Ensures that the position is valid, and that the
            'AI will (hopefully) not crash whilst searching on the position.
            Dim PGNMove As String = "" 'Represents the PGN equivilant of the move that the AI makes.
            TerminateSearch = False
            If BestMove.Code = "f" OrElse BestMove.Code = "o" Then
                Dim IndexInBook As UInt32
                If UseBook.Checked Then IndexInBook = FindPositionInBook(CurrentFEN) 'Finds the index of the opening book that the position exists in.
                If BestMove.Code = "o" Then
                    'Move was forced - make move instantly without searching.
                    Console.WriteLine(vbCrLf & "Search Aborted - Only 1 Move in Position.")
                    CurrentAIDepth.Text = "Current Depth: 1"
                    If PlayerTurn Then
                        PGNMove = Helper.MoveConverter(MasterBoard, BestMove, True, MasterWKPos, Helper.ConvertStringToBitCoor(MasterEnPassant), MasterWhiteTFTable)
                    Else
                        PGNMove = Helper.MoveConverter(MasterBoard, BestMove, False, MasterBKPos, Helper.ConvertStringToBitCoor(MasterEnPassant), MasterBlackTFTable)
                    End If
                    CurrentAIMove.Text = "Current Move: " & PGNMove & " (FORCED)."
                    WaitAdvancedSearchPremoveTime("o")
                ElseIf IndexInBook > 0 Then 'Position found in book - play move from book instantly.

                    'Fully instantiates the entry in the opening book, so that we can retrieve its moves.
                    If Not OpeningBook(IndexInBook).GetComputed() Then OpeningBook(IndexInBook).ComputeEntry()
                    Console.ForegroundColor = ConsoleColor.DarkYellow
                    Console.WriteLine(vbCrLf & "Search Aborted - Position Found in Opening Book.")
                    Console.WriteLine("Moves Available: " & OpeningBook(IndexInBook).ReturnAllMoves() & ". Picking random move...")
                    'Locates a random book move from the position, then converts this to a Move (for the AI's purpose).
                    PGNMove = OpeningBook(IndexInBook).ReturnRndMove()
                    If PlayerTurn Then
                        BestMove = Helper.ConvertToMove(PGNMove, MasterBoard, True, Helper.ConvertStringToBitCoor(MasterWKPos), MasterWhiteTFTable)
                    Else
                        BestMove = Helper.ConvertToMove(PGNMove, MasterBoard, False, Helper.ConvertStringToBitCoor(MasterBKPos), MasterBlackTFTable)
                    End If
                    CurrentAIDepth.Text = "Current Depth: 0"
                    CurrentAIMove.Text = "Current Move: " & PGNMove & " (Book)."
                    CurrentAIEval.Text = "Evaluation: -"
                    WaitAdvancedSearchPremoveTime("b")

                Else
                    'no forced or book move found - call AI.
                    BestMove = InitialiseAI()

                    If BestMove.Code <> "t" Then
                        'Converts the AI's move into the PGN format.
                        If PlayerTurn Then
                            PGNMove = Helper.MoveConverter(MasterBoard, BestMove, True, MasterWKPos, Helper.ConvertStringToBitCoor(MasterEnPassant), MasterWhiteTFTable)
                        Else
                            PGNMove = Helper.MoveConverter(MasterBoard, BestMove, False, MasterBKPos, Helper.ConvertStringToBitCoor(MasterEnPassant), MasterBlackTFTable)
                        End If
                    End If
                    'Console.WriteLine("Transposition Table Full-ness: " & MainAI.GetPercentageTranspositionTableFilled() * 100 & "%.")
                End If
                Console.ForegroundColor = ConsoleColor.White

                If Me.IsHandleCreated AndAlso Not IsClosing Then 'If form is still open...
                    Dim UpdateAllGUI As Boolean = AIHandles.TimeForSearch >= 0.5
                    GameRunning = True
                    If BestMove.Code = "t" Then
                        ResetLMS(True)
                    Else
                        'Makes the AI's move on the board.
                        AnimateMove(BestMove)
                        CalibrateCoreSystemsForMove(BestMove, GameMode <> 1, Not UpdateAllGUI)
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
                            Console.ForegroundColor = ConsoleColor.White
                        End If
                    End If

                    Application.DoEvents() 'Allows the GUI to update after the AI's move.

                    If BestMove.Code <> "t" Then
                        'If the player has been put in check, and has not been checkmated, then add the + symbol to the end of the move.
                        If (MasterWInCheck >= 128 OrElse MasterBInCheck >= 128) AndAlso GameRunning Then PGNMove &= "+"
                        BoardHistory.PushPGN(PGNMove, GameMode <> 1) 'Adds this new move to the History of the game.
                        Dim NextPositionState As Char = EnforceEndStates() 'Checks for end states found from the AI's move.
                        'As two moves are made in one game 'state', we do not copy BoardHistory to its Buffer (so we don't false-trigger 3FR).
                        OutputDebugInfo(Not UpdateAllGUI)
                        If GameMode < 3 Then
                            If PGNAutoOutputter.Checked Then PGNExport_Click() Else FENExport_Click()
                        End If

                        'The below lines fix a bug, where the AI wouldn't make a move after the user restarts their game (1P mode only).
                        If AIEndlessMode.Checked OrElse (GameMode = 1 AndAlso CurrentFEN = PreviousFEN AndAlso Not UserPlayer = PlayerTurn) Then
                            Thread.Sleep(1) 'Allows the system to recalibrate (to prevent spam).
                            Me.BeginInvoke(New Action(AddressOf InitialiseAISystem)) 'Runs the AI again, in such a way that does not add InitialiseAISystem()
                            'to the stack for every move (causing stack overflows).
                        Else
                            'Resets GUI objects & cursor design.
                            UserTimeBar.Enabled = True
                            ComputerIsSearching = False
                            If UpdateAllGUI Then
                                Me.Cursor = Cursors.Default
                                Me.Text = GlobalConstants.ProgramName
                            End If

                            If GameRunning AndAlso GameMode <= 1 Then
                                'It is the user's turn to make a move - notify them by changing the program's title.
                                If GameMode = 1 AndAlso UpdateAllGUI Then Me.Text = "[Your Turn...]  " + GlobalConstants.ProgramName

                                If AICanSearchOnUsersTurn AndAlso IndexInBook = 0 AndAlso NextPositionState <> "o" Then
                                    'The AI is not in its book, and the new position has multiple moves. Allow it to search on the position in the background.
                                    SearchSettings.OutputToConsole = False
                                    MainAI.ConfigureSettings(SearchSettings, True)
                                    'As the AI's last search has already populated the Transposition Table with entries from depth
                                    '= 2 -> AIHandles.CurrentDepth, start searching at AIHandles.CurrentDepth - 2, so that unnecessary work isn't repeated.
                                    AIHandles.StartingDepth = Math.Max(AIHandles.CurrentDepth - 2, 2)
                                    'Creates a new thread for the AI to run on, then starts the AI's search process.
                                    Dim AIThread As New Task(AddressOf VariableAISearchHandler)
                                    AIIsSearchingOnUsersTurn = True
                                    Console.WriteLine(vbCrLf)

                                    AIHandles.AIStopwatch.Restart()
                                    AIThread.Start()
                                End If
                            End If
                        End If
                    Else
                        'Resets GUI objects & cursor design.
                        UserTimeBar.Enabled = True
                        ComputerIsSearching = False
                        If UpdateAllGUI Then
                            Me.Cursor = Cursors.Default
                            Me.Text = GlobalConstants.ProgramName
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    'Algorithm that creates all the threads & AIs that will search on a position. Handles time management & overtime,
    'interactions with the GUI chessboard, live updates via GUI objects, selecting best moves, and more.
    Private Function InitialiseAI() As Move
        SetAdvancedSearchTime()
        Dim UpdateAllGUI As Boolean = AIHandles.TimeForSearch >= 0.5
        Dim BestMove As New Move
        'Creates the AI thread (to enable backgound searching / multithreading).
        Dim AIThread As Task
        If AIHandles.FixedSearchDepth = 0 Then
            AIThread = New Task(AddressOf VariableAISearchHandler)
        Else
            AIThread = New Task(AddressOf FixedAISearchHandler)
        End If

        CalculateAIStartingDepth()
        'Resets GUI objects & changes cursor design.
        If UpdateAllGUI Then Me.Cursor = Cursors.AppStarting
        ProgressBar.ForeColor = Color.FromArgb(0, 192, 0)
        ProgressBar.Value = 0
        AIHandles.AIIsLate = False
        AIHandles.AIPanicMode = 0
        'Erases the information from the last AI search.
        AIHandles.AIBestMove.Code = "a"c
        AIHandles.CurrentDepth = 2
        AIHandles.CurrentMove = "-"
        AIHandles.CurrentEvaluation = "-"
        AITerminator.Visible = True
        AIHandles.AIFinishedSearch = False

        'Prepares the AI for its search.
        MainAI.AddBoardHistory(BoardHistory.GetZobristArray(), BoardHistory.GetHalfSize)
        MainAI.ConfigureSettings(SearchSettings, True)
        MainAI.SetDetailedMoveOutput(UpdateAllGUI)

        'Begins the search.
        ComputerIsSearching = True
        If UpdateAllGUI Then Me.Text = "[Thinking...]  " & GlobalConstants.ProgramName
        Console.ForegroundColor = ConsoleColor.DarkCyan
        Console.Write(vbCrLf & "The AI is now searching..." & vbCrLf & "Search Time = ")
        If AIHandles.TimeForSearch = Decimal.MaxValue Then
            Console.Write("Infinity.")
        Else
            Console.Write(AIHandles.TimeForSearch & " Seconds.")
        End If
        Console.WriteLine(" Quiescence: " & SearchSettings.UseQuiescence & ", Heat Maps: " & SearchSettings.UsePieceHeatMaps & "." & vbCrLf)
        Console.ForegroundColor = ConsoleColor.White

        GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency 'Relaxes garbage collection during the NegaMax search.
        AIHandles.AIStopwatch.Restart()

        'Starts the AI's thread.
        AIThread.Start()

        'Whilst the AI thread is running...
        While Not AIThread.IsCompleted
            Thread.Sleep(5) 'Allows more processing time to be spent on the AIs.

            If UpdateAllGUI Then Application.DoEvents() 'Allows the user to interact with the GUI (limited).
            'Updates progress bar to reflect time.
            If UpdateAllGUI AndAlso AIHandles.TimeForSearch <> Decimal.MaxValue Then
                If AIHandles.AIIsLate Then
                    '2x speed with red colour.
                    ProgressBar.Value = Math.Min((Math.Max(AIHandles.AIStopwatch.ElapsedMilliseconds - AIHandles.TimeForSearch * 1000, 0) / 5) / AIHandles.TimeForSearch, 100)
                Else 'Normal speed with green colour.
                    ProgressBar.Value = Math.Min(((AIHandles.AIStopwatch.ElapsedMilliseconds / 10) / AIHandles.TimeForSearch), 100)
                End If
            End If

            If TerminateSearch OrElse AIHandles.AIStopwatch.ElapsedMilliseconds / 1000 > AIHandles.TimeForSearch OrElse Not Me.IsHandleCreated Then 'Time's up!
                If AIHandles.AIBestMove.Code = "a" OrElse (AIHandles.AdvancedTimeForSearch <> 0 AndAlso MainAI.CheckIfInAspirationBreak()) Then
                    'There has been no move performed in the current amount of time - enable overtime.
                    AIHandles.AIIsLate = True
                    ProgressBar.ForeColor = Color.Red 'Red progress bar - visual indication to user that time is up.
                Else
                    'Force the AI to finish after its current search.
                    MainAI.ABORTSearch()
                End If
                '1.5 * time (or extra 10s) has expired - terminate AI immediately!
                If TerminateSearch OrElse AIHandles.AIStopwatch.ElapsedMilliseconds / 1000 > Math.Min(AIHandles.TimeForSearch * 1.5, AIHandles.TimeForSearch + 10000) OrElse Not Me.IsHandleCreated Then MainAI.ABORTSearch()

            ElseIf AIHandles.AIFinishedSearch Then 'A new depth has been searched to - update GUI controls on latest search & redraw checkerboard.
                If UpdateAllGUI Then
                    AmmendLegalMoveSuares(AIHandles.AIBestMove)
                    Checkerboard.Refresh()
                    If SearchSettings.UseQuiescence Then
                        CurrentAIDepth.Text = "Current Depth: " & AIHandles.CurrentDepth & " - " & MainAI.GetHighestQuiescenceDepth()
                    Else
                        CurrentAIDepth.Text = "Current Depth: " & AIHandles.CurrentDepth
                    End If
                    CurrentAIMove.Text = "Current Move: " & AIHandles.CurrentMove
                    If AIHandles.CurrentEvaluation <> "-" AndAlso Math.Abs(Val(AIHandles.CurrentEvaluation)) >= 295 Then
                        'Mating pattern has been found by AI.
                        CurrentAIEval.Text = "Evaluation: Mate in " & (30000 - (Math.Abs(Val(AIHandles.CurrentEvaluation)) * 100)) \ 2
                    Else
                        CurrentAIEval.Text = "Evaluation: " & AIHandles.CurrentEvaluation
                    End If
                    If AIHandles.AIPanicMode > 0 Then
                        ProgressBar.ForeColor = If(AIHandles.AIPanicMode = 1, Color.Red, Color.Gold)
                        AIHandles.AIPanicMode *= -1
                    End If
                End If
                AIHandles.AIFinishedSearch = False
            End If
        End While

        AIHandles.AIStopwatch.Stop()
        If TerminateSearch Then Console.ForegroundColor = ConsoleColor.Red : Console.WriteLine("Search Terminated Preemptively (by user).")
        GCSettings.LatencyMode = GCLatencyMode.Interactive

        'Select the best move of the highest depth possible (if that search has not been aborted).
        If AIHandles.AIBestMove.Code = "a" Then
            'The AI has not successfully completed a search - start a new search at a depth of 2 (without Quiescence).
            'Sets the result of this new move to be the best move.
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine(vbCrLf & "No Search Completed in Allocated time - Performing Shallow Search...")
            BestMove = MainAI.PerformTestSearch()
            If PlayerTurn Then
                AIHandles.CurrentMove = Helper.MoveConverter(MasterBoard, BestMove, True, MasterWKPos, Helper.ConvertStringToBitCoor(MasterEnPassant), MasterWhiteTFTable)
            Else
                AIHandles.CurrentMove = Helper.MoveConverter(MasterBoard, BestMove, False, MasterBKPos, Helper.ConvertStringToBitCoor(MasterEnPassant), MasterBlackTFTable)
            End If
        ElseIf AIHandles.AIBestMove.Code = "t" Then
            BestMove.SetEmptyMove()
        Else
            BestMove = AIHandles.AIBestMove
        End If
        Console.ForegroundColor = ConsoleColor.DarkCyan
        'Outputs time of given search.
        Console.WriteLine(vbCrLf & "Search Complete. Total Time: " & AIHandles.AIStopwatch.Elapsed.TotalMilliseconds.ToString("N0") & " Milliseconds.")

        'Updates GUI labels (containing info on search).
        If GameMode > 0 Then AITerminator.Visible = False
        TerminateSearch = False

        'Outputs the information of the AI's final search to the GUI labels.
        If SearchSettings.UseQuiescence Then
            CurrentAIDepth.Text = "Current Depth: " & AIHandles.CurrentDepth & " - " & MainAI.GetHighestQuiescenceDepth
        Else
            CurrentAIDepth.Text = "Current Depth: " & AIHandles.CurrentDepth
        End If
        AIHandles.CurrentEvaluation = BestMove.Score * If(PlayerTurn, 1, -1) 'Multiplies by -1 for the black pieces, as NegaMax will always return scores relative to its current player.
        CurrentAIMove.Text = "Current Move: " & AIHandles.CurrentMove
        If AIHandles.CurrentEvaluation <> "-" AndAlso Math.Abs(Val(AIHandles.CurrentEvaluation)) > 295 Then
            'Mating pattern has been found - update GUI accordingly.
            CurrentAIEval.Text = "Evaluation: Mate in " & (30000 - (Math.Abs(Val(AIHandles.CurrentEvaluation)) * 100)) \ 2
        Else
            CurrentAIEval.Text = "Evaluation: " & AIHandles.CurrentEvaluation
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



    'Subroutines controlling the AI thread. Variable Search = the AI starts at a depth of AIHandles.StartingDepth, and increments by one after each search (iterative deepening).
    'Fixed Search = the AI only performs one search, but at the specified depth that the user provided.
    Private Sub VariableAISearchHandler()
        Dim AICurrentMove As Move
        Dim CurrentSearchStopwatch As New Stopwatch
        Dim CurrentAIDepth As Integer = AIHandles.StartingDepth
        Dim MemoryUsage As UInt64
        Dim PreviousEvaluation As Double
        While CurrentAIDepth < 100
            CurrentSearchStopwatch.Restart()
            'Performs a new search using iterative deepening. For all searches apart from the first, feed the previous search's best
            'move into the new search. This is so that this previous best move can be searched first, resulting in more AlphaBeta prunes.
            If CurrentAIDepth = AIHandles.StartingDepth Then
                AICurrentMove = MainAI.Search(CurrentAIDepth)
            Else
                AICurrentMove = MainAI.Search(CurrentAIDepth, AIHandles.AIBestMove)
            End If
            CurrentSearchStopwatch.Stop()

            If AICurrentMove.Code = "a" OrElse AICurrentMove.Code = "t" OrElse (MainAI.GetABORTState() AndAlso CurrentAIDepth = AIHandles.StartingDepth) Then
                'If the AI has not completed a single search when its search process is ABORTed, or if then current
                'search did not yield a valid move, do not save the current move, and exit the search process.
                If AICurrentMove.Code = "t" Then AIHandles.AIBestMove = AICurrentMove
                Exit While
            Else
                'Outputs the timings of the current search.
                If CurrentAIDepth > AIHandles.StartingDepth + 2 Then PreviousEvaluation = AIHandles.AIBestMove.Score
                AIHandles.AIBestMove = AICurrentMove
                Console.ForegroundColor = ConsoleColor.DarkGreen
                Console.Write("Depth Of " & CurrentAIDepth)
                If SearchSettings.UseQuiescence Then Console.Write("-" & MainAI.GetHighestQuiescenceDepth())
                Console.WriteLine(" Completed in: " & AIHandles.AIStopwatch.ElapsedMilliseconds.ToString("N0") & " Milliseconds (" & CurrentSearchStopwatch.ElapsedMilliseconds.ToString("N0") & "ms)")
                Console.ForegroundColor = ConsoleColor.White


                'Memory Management tool: as the AI searches, the Transposition Table consumes more and more memory.
                'If left, the system will consume too much memory, and will crash. To prevent this,
                'after each branch is searched, we check how much memory is being consumed by the program,
                'and if it exceeds a pre-defined limit, we reset the Transposition Table, to free up memory.
                'We also call a full garbage collection on the system if the memory useage is somewhat high.
                If AIHandles.TimeForSearch = Decimal.MaxValue Then
                    MemoryUsage = Math.Max((Process.GetCurrentProcess()).WorkingSet64 - AIHandles.InitialMemoryUsage, 0)
                    If MemoryUsage >= GlobalConstants.MemoryThreshold Then
                        If MemoryUsage >= 2 * GlobalConstants.MemoryThreshold Then
                            Console.ForegroundColor = ConsoleColor.DarkRed
                            Console.WriteLine("Memory Limit Exceeded (Usage = " & (MemoryUsage / (1024 * 1024)).ToString("N0") & "MB). Resetting Transposition Table to free up Memory...")
                            Console.ForegroundColor = ConsoleColor.White
                            MainAI.ResetTranspositionTable()
                            MainAI.AddBoardHistory(BoardHistory.GetZobristArray(), BoardHistory.GetHalfSize)
                        End If
                        GC.Collect()
                    End If
                End If


                If GameMode = 4 Then
                    If HandleAIPuzzleGuess() Then Exit While
                Else
                    'Edits the GUI for the completed search.
                    If PlayerTurn Then
                        AIHandles.CurrentMove = Helper.MoveConverter(MasterBoard, AIHandles.AIBestMove, True, MasterWKPos, Helper.ConvertStringToBitCoor(MasterEnPassant), MasterWhiteTFTable)
                    Else
                        AIHandles.CurrentMove = Helper.MoveConverter(MasterBoard, AIHandles.AIBestMove, False, MasterBKPos, Helper.ConvertStringToBitCoor(MasterEnPassant), MasterBlackTFTable)
                    End If
                    AIHandles.CurrentDepth = CurrentAIDepth
                    AIHandles.CurrentEvaluation = AIHandles.AIBestMove.Score * If(PlayerTurn, 1, -1)
                    AIHandles.AIFinishedSearch = True

                    If MainAI.GetABORTState() OrElse AIHandles.AIIsLate Then
                        'The time to search has expired - exit search process.
                        Exit While
                    ElseIf Math.Abs(AIHandles.AIBestMove.Score) >= 295 AndAlso CurrentAIDepth >= CInt(100 * (299.99 - Math.Abs(AIHandles.AIBestMove.Score))) Then
                        'A checkmating pattern has been found, and we can assume that it is the fastest pattern (as, by enforcing the above Depth requirement,
                        'we know that the effects of search extensions has not produced a slower mate).
                        Exit While
                    End If

                    'Checks if the score has changed significantly, compared to the previous depth. If it has, we dynamically change the AI's remaining Time to Search.
                    Dim DeltaScoreNeededForPanicMode As Double = 0.75
                    If AIHandles.AdvancedTimeForSearch <> 0 AndAlso CurrentAIDepth > AIHandles.StartingDepth + 2 AndAlso Math.Abs(PreviousEvaluation - AICurrentMove.Score) >= DeltaScoreNeededForPanicMode AndAlso Not MainAI.GetABORTState() AndAlso AIHandles.AIPanicMode = 0 AndAlso SearchSettings.UseQuiescence Then
                        Console.ForegroundColor = ConsoleColor.Magenta
                        If PlayerTurn Xor (PreviousEvaluation < AICurrentMove.Score) Then
                            'If under 1/3 of the search has completed, the AI still has enough time to figure it out.
                            If AIHandles.AIStopwatch.ElapsedMilliseconds / 1000 > 0.33 * AIHandles.TimeForSearch Then
                                'We have had a signnificant drop in move evaluation. Give the AI more time to think, to hopefully resolve this issue.
                                AIHandles.TimeForSearch += 2 * (AIHandles.TimeForSearch - (AIHandles.AIStopwatch.ElapsedMilliseconds / 1000))
                                Console.WriteLine("Major Evaluation Drop Detected. Giving the AI Longer to Search...")
                                AIHandles.AIPanicMode = 1
                            End If
                        ElseIf AIHandles.AIStopwatch.ElapsedMilliseconds / 1000 < 0.67 * AIHandles.TimeForSearch Then
                            'The AI's last search found a way to dramatically increase its position. Reduce the future search time, as we are probably aight :D.
                            'We only do this if the AI has still got >33% left to go, as otherwise this will have little effect.
                            AIHandles.TimeForSearch -= 0.5 * (AIHandles.TimeForSearch - (AIHandles.AIStopwatch.ElapsedMilliseconds / 1000))
                            Console.WriteLine("Major Evaluation Raise Detected. Shortening AI Search Time...")
                            AIHandles.AIPanicMode = 2
                        End If
                        Console.ForegroundColor = ConsoleColor.White
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
        'Begins the search at the specific depth specified by either the user, or by CalculateAIHandles.StartingDepth().
        AIHandles.AIBestMove = MainAI.Search(AIHandles.StartingDepth)
        If Not (MainAI.GetABORTState() OrElse AIHandles.AIBestMove.Code = "a") Then
            'Outputs the information of the AI's search time.
            Console.ForegroundColor = ConsoleColor.DarkGreen
            Console.Write("Depth Of " & AIHandles.StartingDepth)
            If SearchSettings.UseQuiescence Then Console.Write("-" & MainAI.GetHighestQuiescenceDepth())
            Console.Write(" Completed in: " & AIHandles.AIStopwatch.ElapsedMilliseconds.ToString("N0") & " Milliseconds." & vbCrLf)
            Console.ForegroundColor = ConsoleColor.White
            'Updates GUI elements.
            If PlayerTurn Then
                AIHandles.CurrentMove = Helper.MoveConverter(MasterBoard, AIHandles.AIBestMove, True, MasterWKPos, Helper.ConvertStringToBitCoor(MasterEnPassant), MasterWhiteTFTable)
            Else
                AIHandles.CurrentMove = Helper.MoveConverter(MasterBoard, AIHandles.AIBestMove, False, MasterBKPos, Helper.ConvertStringToBitCoor(MasterEnPassant), MasterBlackTFTable)
            End If
            AIHandles.CurrentEvaluation = AIHandles.AIBestMove.Score * If(PlayerTurn, 1, -1)
            AIHandles.CurrentDepth = AIHandles.StartingDepth

            Dim MemoryUsage As UInt64 = (Process.GetCurrentProcess()).WorkingSet64 - AIHandles.InitialMemoryUsage
            If MemoryUsage >= GlobalConstants.MemoryThreshold Then
                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.WriteLine("Memory Limit Exceeded (Usage = " & (MemoryUsage / (1024 * 1024)).ToString("N0") & "MB). Resetting Transposition Table to free up Memory...")
                Console.ForegroundColor = ConsoleColor.White
                MainAI.ResetTranspositionTable()
            End If
        End If
    End Sub

End Class
