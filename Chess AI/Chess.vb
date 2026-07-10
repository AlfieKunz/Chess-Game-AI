'Working Version of Chess, along with AI using MiniMax and Alpha Beta Pruning. Created by Alfie Kunz - 8158.
Imports System.Threading
Imports System.Text.RegularExpressions
Imports System.Security.Cryptography
Imports System.Diagnostics.Eventing.Reader
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.IO
Imports System.Text

'This class forms the main User Interface and the basic game of chess, along with its rules. It will control moving pieces,
'importing / exporting positions, manipulating the board + its visual design ext, along with converting a user’s actions onto
'a computer-friendly interface (for the AI to use). This also instantiates the AI class and interacts with it.
Partial Public Class Chess 'ew- danny
    Private Structure PieceInfo 'Contains information about a piece that the user is moving on the GUI.
        Dim IsMovingPiece As Boolean 'is the piece being moved by the user?
        Dim Piece As PictureBox 'Specifies the exact object that is being moved.
        Dim LegalMoves() As String 'Array containing the legal Moves of the piece being dragged.
        Dim LockedPiece As String 'Contains the location of the piece that is locked by the user (applies if Touch Move is enabled).
        Dim StartPoint As Point
        Dim MidPoint As Point
        Dim EndPoint As Point
    End Structure
    Private PieceMoving As PieceInfo

    Private SharedAlgorithms As New CoreMethods 'Calls all objects from the CoreMethods class.
    Private BoardHistory As New GameHistory

    'Here are the computer's primary "eyes" - MasterBoard (which contains each piece's location on the board) and,
    'derived from this, Master TrueFalse Tables for each player. These Tables display information such as Legal
    'Moves for the players' kings, along with pinned pieces and their location, and make handling Move Generation,
    'Checks and Evaluations much easier and more efficient.
    Private MasterBoard(7, 7), MasterWhiteTFTable(7, 7), MasterBlackTFTable(7, 7) As Char
    Private GameRunning As Boolean
    Private GameMode As Byte = 3 '1 = 1-Player, 2 = 2-Player, 3 = Analysis, 4 = Puzzles, 5 = Coordinate Practice, 6 = Move Practice.

    'FEN = Standard Chess notation that displays where all the pieces on the board are supposed to go.
    Private StartingFEN As String = GlobalConstants.InitialFENPosition
    Private CurrentFEN, PreviousFEN, InvalidInput As String
    Private PlayerTurn As Boolean = True 'True = Is White's Turn, False = Is Black's Turn
    Private UserPlayer As Boolean = True 'Represents what colour the user is playing with.
    Private MasterWKPos, MasterBKPos As String 'Loaction of Kings.
    Private MasterEnPassant As String 'Location of EnPassant Square
    Private MasterZobristValue As UInt64 'Zobrist value of the current position (a 'unique' value of a board position, that can be easily calculated)

    'Data for castling & checks for each player.
    Private MasterWCanCastle, MasterBCanCastle As New CanCastle
    Private MasterWInCheck, MasterBInCheck As New InCheck

    Private ReadOnly OpeningBook As New List(Of OpeningBookEntry)
    Private ReadOnly PieceArray(47) As PictureBox 'Contains all the PictureBoxes on the board.
    Private PrimaryColour As Color 'Colour representing the dark squares on the Chessboard.
    Private SecondaryColour As Color 'Colour representing the light squares on the Chessboard.
    Private SquareHistory(3, 1) As SByte 'Array containing the previously used squares on the board (for prev moves).
    Private LegalMoveSquares(7, 7) As Boolean 'Array containing the coordinates of where a piece can move on the board,
    '... generated when the user clicks on a piece. Used when redrawing the checkerboard pattern.
    Private OrientForWhite As Boolean = True 'Represents which way the board is flipped.
    Private AnimationSpeed As Byte = 3 'Represents the speed of the piece-moving animation: 0 = Off, 1 = Fast, 2 = Medium, 3 = Slow.
    Private GeneralOptions As String = "TTTTTFFF" '8-character string that represents the configuration of the program.
    'Index: 0 = Sound, 1 = Opening Animation, 2 = Large Opening Book, 3 = Board Highlights, 4 = Piece Highlights, 5 = Touch Move, 6 = Invisible Pieces, 7 = Hammad Mode (bad AI).

    Private FixedSearchDepth As Byte = 0 'Number representing the fixed depth the AI will search to (0 = off).
    'Index: 0 = Sound, 1 = Opening Animation, 2 = Board Highlights, 3 = Piece Highlights, 4 = Touch Move, 5 = Invisible Pieces, 6 = Hammad Mode (bad AI).

    'Data structures that bridge the gap between user and AI.
    Private StartingDepth As SByte 'Estimate of the optimal depth used for searching.
    Private TimeForSearch As Decimal = 10 'Time the AIs are allowed to search for.
    Private CurrentDepth As Byte 'Highest depth successfully searched to.
    Private CurrentMove As String 'Move of the highest depth successfully searched to.
    Private CurrentEvaluation As String 'Eval of the highest depth successfully searched to.
    Private TerminateSearch As Boolean 'Set to True if the user preemptively aborts the AI search.
    Private ComputerIsSearching As Boolean = False 'Set to True whilst the AI is searching - ensures that the user cannot make moves.
    Private PieceIsMoving As Boolean 'Represents if the AnimatePiece() method is running.
    Private SearchSettings As New AISearchSettings 'User's chosen settings for the AI's search.

    'Chess AI, along with appropriate objects.
    Private MainAI As AI
    Private AIBestMove As Move
    Private AIFinishedSearch As Boolean 'Set to True when a given search has been complete, so that GUI elements can be updated.
    Private AIStopwatch As New Stopwatch 'For timing AI searches.


    'Sets up sound effects. All sounds used by Chess.com
    Private ReadOnly Sound_Move As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_Move.wav"
    }
    Private ReadOnly Sound_Capture As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_Capture.wav"
    }
    Private ReadOnly Sound_Check As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_Check.wav"
    }
    Private ReadOnly Sound_Castle As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_Castle.wav"
    }
    Private ReadOnly Sound_Checkmate As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_Checkmate.wav"
    }
    Private ReadOnly Sound_Stalemate As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_Stalemate.wav"
    }
    Private ReadOnly Sound_GameOver As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_GameOver.wav"
    }

    'Note: to calibrate systems after each move...
    '   AnimateMove() if needed
    '   Current player Is no longer In check
    '   MakeMove() (dependant on player & circumstances)
    '   FixTFTable of opposite player.
    '   Copy SquareHistory index 0&1 To index 2&3
    '   Copy New move into index 0&1 of SquareHistory (making sure to flip for Not OrientForWhite)
    '   ResetLMS() if AI move
    '   FlipBoard() if autoflipper Is on, Or otherwise just do Checkerboard.Refresh()
    '   CheckChecker()
    '   Reverse PlayerTurn
    '   PreviousFEN = CurrentFEN, then calculate New CurrentFEN
    '   Reconfigure AI
    '   EnforceENdStates()
    '   Output FEN To input box For specific game modes.
    '   OutputDebugInfo()


    'Constructor methods for the Chess Class.
    Public Sub New()
        Me.New(New List(Of OpeningBookEntry))
    End Sub
    Public Sub New(ByVal Mode As SByte)
        Me.New(Mode, GlobalConstants.InitialFENPosition)
    End Sub
    Public Sub New(ByVal Mode As Byte, ByVal UserStartingFEN As String)
        Me.New(Mode, UserStartingFEN, 0, False, False, True, Nothing)
    End Sub

    Public Sub New(ByRef InputBook As List(Of OpeningBookEntry)) 'Call used for a standard game of chess (usually in Analysis Mode).
        GameRunning = False
        InitializeComponent() ' This call is required by the designer.
        'Loads the opening book into the system.
        If InputBook IsNot Nothing AndAlso InputBook.Count > 0 Then OpeningBook.AddRange(InputBook) : UseBook.Enabled = True
        SharedAlgorithms.SetDefaultAISearchSettings(SearchSettings) 'Calibrates initial, default settings.
        End Sub

    'The below constructor method rearranges the location of objects on the form (to save space on the screen).
    'This will be called for 1-Player Games and 2-Player Games.
    Public Sub New(ByVal Mode As Byte, ByVal UserStartingFEN As String, ByVal UserTimeForSearch As Decimal, ByVal UserQuiescence As Boolean, ByVal UserPieceHeatMaps As Boolean, ByVal PlayAsWhite As Boolean, ByRef InputBook As List(Of OpeningBookEntry))
        Me.New(InputBook)
        GameMode = Mode
        StartingFEN = UserStartingFEN

        'Shrinks the size of the forms, and modifies the obejct now off-screen to better suit the GUI.
        Me.Size = New Size(916, 639)
        AIMoveBtn.Visible = False
        InputTextBox.ReadOnly = True
        Reset_Btn.Visible = False
        InputButton.Visible = False
        FENExport.Visible = False
        UndoMove.Top -= 80
        CheckLabel.Location = New Point(89, 128)
        UserTimeBar.Visible = False
        UserTimeBox.Visible = False
        QuiescenceBox.Visible = False
        PieceHeatMapBox.Visible = False
        UseBook.Visible = False
        AIEndlessMode.Visible = False
        SettingsBtn.Location = New Point(40, 520)
        ExitBtn.Location = New Point(181, 520)
        Credits.Location = New Point(78.5, 575)


        If Mode = 1 Then 'Mode-Specific Movement - 1P Mode.
            'Hides / sets appropriate objects.
            FlipperButton.Visible = False
            AutoFlipper.Visible = False
            ProgressBar.Location = New Point(46, 250)
            CurrentAIDepth.Location = New Point(ProgressBar.Location.X + 10, ProgressBar.Location.Y + 80)
            CurrentAIMove.Location = New Point(CurrentAIDepth.Location.X, CurrentAIDepth.Location.Y + 30)
            CurrentAIEval.Location = New Point(CurrentAIDepth.Location.X, CurrentAIDepth.Location.Y + 60)
            UserPlayer = PlayAsWhite
            'Calibrates AI settings based on the user's requirements.
            If InputBook.Count > 0 Then UseBook.Checked = True
            TimeForSearch = UserTimeForSearch
            SearchSettings.UseQuiescence = UserQuiescence
            SearchSettings.UsePieceHeatMaps = UserPieceHeatMaps
            SearchSettings.OutputPath = False

        ElseIf Mode = 2 Then '2P Mode
            FlipperButton.Top -= 100
            AutoFlipper.Top -= 100
            'Hides / sets appropriate objects, so that only the relevant objects are displayed.
            ProgressBar.Visible = False
            CurrentAIEval.Visible = False
            AutoFlipper.Checked = True
            CurrentAIDepth.Visible = False
            CurrentAIMove.Visible = False
            CurrentAIEval.Visible = False
        ElseIf Mode = 4 Then 'Puzzles mode
            'Selects, then loads a random puzzle database from the Database folder (1-16).
            Static RND As New Random()
            Dim SampleIndex As Byte = RND.Next(1, 17)

            Dim Timer As New Stopwatch
            Dim TempEntry As PuzzleEntry
            Try
                Timer.Start()
                'Opens puzzle database from file using StreamReader.
                Using SR As New StreamReader(Application.StartupPath & "\Assets\Puzzle Database\" & CStr(SampleIndex) & ".txt", Encoding.UTF8, True, 16384)
                    While Not SR.EndOfStream
                        TempEntry = New PuzzleEntry(SR.ReadLine())
                        PuzzleSampleDatabase.Add(TempEntry)
                    End While
                End Using
                'Loads extra hard puzzles from database.
                Using SR As New StreamReader(Application.StartupPath & "\Assets\Puzzle Database\Extra Hard Puzzles.txt", Encoding.UTF8, True, 4096)
                    While Not SR.EndOfStream
                        TempEntry = New PuzzleEntry(SR.ReadLine())
                        PuzzleSampleDatabase.Add(TempEntry)
                    End While
                End Using
                Timer.Stop()
                Console.ForegroundColor = ConsoleColor.Green
                Console.WriteLine("Puzzle Database " & SampleIndex & " Successfully Retrieved in: " & Math.Round(Timer.Elapsed.TotalMilliseconds, 1) & "ms.")
            Catch ex As Exception 'Could not successfully retrieve database - exit to main menu.
                Timer.Stop()
                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.WriteLine("Error when retrieving Puzzle Database.")
                MsgBox("Error: Unable to retrive the Puzzle Database." & vbCrLf & "Returning to the Main Menu...", vbCritical + vbOKOnly + vbApplicationModal)
                ExitBtn_Click()
                Exit Sub
            End Try

            LoadPuzzleRatings()
            SearchSettings.OutputToConsole = False
            TimeForSearch = Decimal.MaxValue 'Makes sure there isn't a puzzle timer on the initial puzzle.
            StartingFEN = GetRndPuzzle() 'Sets Random Puzzle to be starting position.

            'Calibrates objects specific to the puzzle mode.
            UndoMove.Visible = False
            CheckLabel.Top += 46
            FlipperButton.Visible = False
            AutoFlipper.Visible = False
            ProgressBar.Location = New Point(10, 470)
            ProgressBar.Size = New Size(150, 25)
            CurrentAIEval.Visible = False
            CurrentAIDepth.Visible = False
            CurrentAIMove.Visible = False
            CurrentAIEval.Visible = False
            PuzzleRatingLabel.Visible = True
            HintBtn.Visible = True
            GiveUpBtn.Visible = True
            RatingHeader.Visible = True
            RatingLabel.Visible = True
            LostRatingLabel.Visible = True
            GainedRatingLabel.Visible = True
            AIPuzzleInfoLabel.Visible = True
            HumanModeBtn.Visible = True
            AIModeBtn.Visible = True
            AutoAdvanceOnComplete.Visible = True
            AutoAdvanceLabel.Visible = True
            AutoAdvanceBox.Visible = True
            AutoAdvanceBar.Visible = True
            InfoBtn.Visible = True
            ResetRatingsBtn.Visible = True

        ElseIf Mode = 5 OrElse Mode = 6 Then 'Coordinate / Move Training Mode.
            'Hides / sets appropriate objects.
            InputTextBox.Visible = False
            UndoMove.Visible = False
            CheckLabel.Visible = False
            FlipperButton.Top -= 60
            AutoFlipper.Visible = False
            ProgressBar.Visible = False
            CurrentAIEval.Visible = False
            CurrentAIDepth.Visible = False
            CurrentAIMove.Visible = False
            CurrentAIEval.Visible = False
            TrainingStart.Visible = True
            TimerLabel.Visible = True
            WLeaderBoardGrid.Visible = True
            InfoBtn.Visible = True
            'Calibrates Leaderboards.
            RetrieveLeaderboards()
            If GameMode = 6 Then TimerLabel.Text = "Time Left: 30.0 Seconds"
        End If
    End Sub


    'Subroutine that sets up the chess system.
    Private Sub Form_Load() Handles Me.Load
        Dim StartupStopwatch As New Stopwatch
        StartupStopwatch.Start()
        Application.EnableVisualStyles()
        Console.OutputEncoding = System.Text.Encoding.UTF8
        Randomize()

        'Assigns each piece to PieceArray and sets the 'board' picturebox as their parent.
        PieceArray(0) = WK1
        PieceArray(1) = WQ1
        PieceArray(2) = WB1
        PieceArray(3) = WB2
        PieceArray(4) = WN1
        PieceArray(5) = WN2
        PieceArray(6) = WR1
        PieceArray(7) = WR2
        PieceArray(8) = WP1
        PieceArray(9) = WP2
        PieceArray(10) = WP3
        PieceArray(11) = WP4
        PieceArray(12) = WP5
        PieceArray(13) = WP6
        PieceArray(14) = WP7
        PieceArray(15) = WP8
        PieceArray(16) = BK1
        PieceArray(17) = BQ1
        PieceArray(18) = BB1
        PieceArray(19) = BB2
        PieceArray(20) = BN1
        PieceArray(21) = BN2
        PieceArray(22) = BR1
        PieceArray(23) = BR2
        PieceArray(24) = BP1
        PieceArray(25) = BP2
        PieceArray(26) = BP3
        PieceArray(27) = BP4
        PieceArray(28) = BP5
        PieceArray(29) = BP6
        PieceArray(30) = BP7
        PieceArray(31) = BP8
        PieceArray(32) = WQ2
        PieceArray(33) = WQ3
        PieceArray(34) = WQ4
        PieceArray(35) = WQ5
        PieceArray(36) = WQ6
        PieceArray(37) = WQ7
        PieceArray(38) = WQ8
        PieceArray(39) = WQ9
        PieceArray(40) = BQ2
        PieceArray(41) = BQ3
        PieceArray(42) = BQ4
        PieceArray(43) = BQ5
        PieceArray(44) = BQ6
        PieceArray(45) = BQ7
        PieceArray(46) = BQ8
        PieceArray(47) = BQ9
        For x = 0 To 47
            Checkerboard.Controls.Add(PieceArray(x))
        Next

        'Sets up more essential objects...
        UndoFENChange.Visible = False
        CheckLabel.Text = "                    "
        For n = 0 To 3
            SquareHistory(n, 0) = -1
            SquareHistory(n, 1) = -1
        Next

        'If the FEN is valid, we send the start postion to be converted to the Board array, then display the pieces on the board.
        'If it is not valid, we replace the FEN with the Default StartingFEN.
        Dim FENErrorMessage As String = ""
        Dim FENIsValid As Boolean = False
        Dim CustomisationForm As Form = Nothing 'We first locate the cusomisation form (if the user used it).
        If GameMode = 1 Then
            CustomisationForm = CType(Application.OpenForms("OnePlayerCustomisation"), OnePlayerCustomisation)
        ElseIf GameMode = 2 Then
            CustomisationForm = CType(Application.OpenForms("TwoPlayerCustomisation"), TwoPlayerCustomisation)
        ElseIf GameMode > 3 Then
            CustomisationForm = CType(Application.OpenForms("TrainingCustomisation"), TrainingCustomisation)
        End If
        If FENErrorDetection(StartingFEN, False, FENErrorMessage) Then 'First step of error-detection.
            Try
                'Forms the chessboard, along with the required variables.
                MasterBoard = SharedAlgorithms.FENConverter(StartingFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
                DisplayPieces()
                'Forms the TFTable for the players.
                If PlayerTurn Then
                    SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterEnPassant)
                Else
                    SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterBInCheck, MasterEnPassant)
                End If
                If GameMode <> 3 Then CustomisationForm.Close() 'Closes the customisation form.
                CheckChecker(False)
                FENIsValid = True
            Catch ex As Exception 'FEN is not valid, as the board could not be constructed - provide error message.
                If MsgBox("Starting Position Rejected - Invalid FEN Code. Please Input a Genuinine FEN and try again." & vbCr & "Please Click 'Retry' to enter another FEN, or Click 'Cancel' to Start the Game with the Standard Starting Position.", vbCritical + vbRetryCancel + vbApplicationModal) = 4 Then
                    'The user wants to enter another FEN.
                    Me.Close()
                End If
            End Try
        Else
            'Displays the appropriate error message, as stated by the FENErrorDetection Sub.
            If MsgBox("Starting " & FENErrorMessage & vbCr & "Please Click 'Retry' to Enter Another FEN, or Click 'Cancel' to Start the Game with the Standard Starting Position.", vbCritical + vbRetryCancel + vbApplicationModal) = 4 Then
                'The user wants to enter another FEN.
                Me.Close()
            End If
        End If

        'Calibrates the board using the standard starting position.
        If Not FENIsValid AndAlso Application.OpenForms().OfType(Of Chess).Any Then 'If customisation form hasn't been closed.
            'Sets the board to the standard FEN postion, and forms variables & objects.
            CustomisationForm.Close()
            StartingFEN = GlobalConstants.InitialFENPosition
            MasterBoard = SharedAlgorithms.FENConverter(StartingFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
            DisplayPieces()
            SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterEnPassant)
            'Resets Black's TFTable.
            Array.Copy(SharedAlgorithms.MasterTrueTable, MasterBlackTFTable, 64)
        End If



        If Me.IsHandleCreated Then 'If this form is open...
            'Flips board in user's favour (but must be for the opposite player for puzzles).
            If GameMode = 4 Then
                If PlayerTurn Then FlipBoard()
            Else
                If Not UserPlayer Then FlipBoard()
            End If

            'Final logical detection of invalid board positions.
            If Not CheckForInvalidGameStates() Then
                'Everything is valid - begin the game as normal.
                Console.ForegroundColor = ConsoleColor.Green
                GameRunning = True
                Console.WriteLine(vbCrLf & vbCrLf & "The Game has Begun.")
                Console.ResetColor()
            End If

            'Creates our AI (for legal move generation).
            CurrentFEN = StartingFEN
            MainAI = New AI(CurrentFEN)
            MainAI.ConfigureSettings(SearchSettings)
            MasterZobristValue = SharedAlgorithms.ZobristHashPosition(MasterBoard, PlayerTurn, MasterWCanCastle, MasterBCanCastle, MasterEnPassant)

            LoadUserProfile()

            StartupStopwatch.Stop()
            Console.WriteLine("Startup Time: " & StartupStopwatch.Elapsed.TotalMilliseconds & " Milliseconds.")
            If GameMode <= 3 Then OutputDebugInfo()

            AddHandler Me.Shown, AddressOf Form1_Load_AfterWindowShown 'After all the core elements have been configured, show the
            'form window to the user, then run the non-core elements of the class.
        End If
    End Sub

    'Subroutine containing all the code that will be performed upon boot-up, after the Chess form has been shown to the user.
    Private Sub Form1_Load_AfterWindowShown()
        If GameMode < 3 Then
            If GameMode = 1 AndAlso Not (UserPlayer = PlayerTurn) Then
                'The AI needs to make the first move.
                InitialiseAISystem()
                PreviousFEN = CurrentFEN
            Else 'Prevents the user from undoing the starting move.
                PreviousFEN = StartingFEN
            End If
            InputTextBox.Text = StartingFEN
            InputTextBox.Select(InputTextBox.Text.Length, 0) 'Stops the TextBox from being highlighted.
        ElseIf GameMode = 4 Then
            InputTextBox.Select(InputTextBox.Text.Length, 0) 'Stops the TextBox from being highlighted.
        ElseIf GameMode >= 5 Then
            Me.WLeaderBoardGrid.ClearSelection() 'Stops the Leaderboard from being highlighted.
        Else
            PreviousFEN = StartingFEN 'if CurrentFEN = PreviousFEN then the user cannot undo a move.
        End If
        EnforceEndStates(False) 'Checks for end positions in the starting FEN.
        If GameMode = 1 Then
            'The previous line duplicates the Starting FEN into BoardHistory, so we need to remove it.
            BoardHistory.Pop()
        ElseIf GameMode = 4 Then
            'Plays the first move of the puzzle, then calibrates the AI.
            PlayNextPuzzleMove(0, True)
            RunAIOnPuzzle()
        End If

    End Sub


    'Subroutine that assigns all the PictureBoxes with their associated images.
    Private Sub AssignImagesToPieces()
        'All images are taken from the 'cburnett' piece theme of Lichess.org - an open source, charity chess website.
        'Please see here for information about using these assets: https://lichess.org/contact#help-authorize.
        Dim Colour As String = "W"
        For x = 0 To 16 Step 16
            PieceArray(x).Image = Image.FromFile(Application.StartupPath & "\Images\Default\" & Colour & "King.png")
            PieceArray(x + 1).Image = Image.FromFile(Application.StartupPath & "\Images\Default\" & Colour & "Queen.png")
            For n = x + 2 To x + 3
                PieceArray(n).Image = Image.FromFile(Application.StartupPath & "\Images\Default\" & Colour & "Bishop.png")
            Next
            For n = x + 4 To x + 5
                PieceArray(n).Image = Image.FromFile(Application.StartupPath & "\Images\Default\" & Colour & "Knight.png")
            Next
            For n = x + 6 To x + 7
                PieceArray(n).Image = Image.FromFile(Application.StartupPath & "\Images\Default\" & Colour & "Rook.png")
            Next
            For n = x + 8 To x + 15
                PieceArray(n).Image = Image.FromFile(Application.StartupPath & "\Images\Default\" & Colour & "Pawn.png")
            Next
            Colour = "B"
        Next
        For x = 32 To 39
            PieceArray(x).Image = Image.FromFile(Application.StartupPath & "\Images\Default\WQueen.png")
        Next
        For x = 40 To 47
            PieceArray(x).Image = Image.FromFile(Application.StartupPath & "\Images\Default\BQueen.png")
        Next
    End Sub

    'Subroutine that calibrates the settings of the user. Can be called by the settings form when settings are modified.
    Public Sub LoadUserProfile()
        'Sets the program settings (from txt file). If all else fails, use the default settings.
        Dim ColourScheme, TempGeneralOptions As String
        Try
            FileOpen(1, Application.StartupPath & "\Assets\User\UserProfile.txt", OpenMode.Input)
            ColourScheme = LineInput(1)
            AnimationSpeed = Val(LineInput(1))
            TempGeneralOptions = LineInput(1)
            Dim temp As String = TempGeneralOptions(7) 'Tests that the General Options are the correct length.
            'Touch Move rule is only supported in the 1-Player and 2-Player modes - disable the control otherwise.
            If GameMode >= 3 Then TempGeneralOptions = TempGeneralOptions.Remove(5, 1).Insert(5, "F")
            'Invisible pieces are not supported in the training modes - forcibly disable this control.
            If GameMode > 3 Then TempGeneralOptions = TempGeneralOptions.Remove(6, 1).Insert(6, "F")
            If GameMode > 1 Then FixedSearchDepth = Val(LineInput(1))
        Catch ex As Exception
            'Default settings.
            ColourScheme = "def"
            AnimationSpeed = 3
            TempGeneralOptions = "TTTTTFFF"
            FixedSearchDepth = 0
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Error in retrieving User Profile - reverting to default settings.")
            Console.ResetColor()
        End Try
        FileClose(1)

        If GeneralOptions(5) <> TempGeneralOptions(5) Then
            PieceMoving.LockedPiece = "" 'Resets locked piece.
            UndoMove.Enabled = Not (TempGeneralOptions(5) = "T")
        End If
        'Toggles Hammad Mode.
        If GeneralOptions(7) <> TempGeneralOptions(7) Then SearchSettings.ReturnBestMove = Not (TempGeneralOptions(7) = "T")

        'Calibrates General Options
        If GeneralOptions(6) <> TempGeneralOptions(6) Then
            'Invisible pieces setting modified...
            GeneralOptions = TempGeneralOptions
            If TempGeneralOptions(6) = "T" Then
                'Removes images from pieces.
                For x = 0 To 47
                    PieceArray(x).Image = Nothing
                Next
            Else
                'Restores images to pieces.
                AssignImagesToPieces()
                CheckChecker(True)
            End If
        ElseIf GeneralOptions(4) <> TempGeneralOptions(4) AndAlso GeneralOptions(6) = "F" Then
            GeneralOptions = TempGeneralOptions
            CheckChecker(True) 'Restores / Removes king highlights.
        Else
            GeneralOptions = TempGeneralOptions
        End If

        'Sets Primary & Secondary Colour depending on ColourScheme.
        Select Case LCase(ColourScheme)
            Case "blu"
                PrimaryColour = Color.LightSteelBlue
                SecondaryColour = Color.AliceBlue
            Case "bl2"
                PrimaryColour = Color.SlateGray
                SecondaryColour = Color.Silver
            Case "gld"
                PrimaryColour = Color.Goldenrod
                SecondaryColour = Color.LemonChiffon
            Case "grn"
                PrimaryColour = Color.DarkSeaGreen
                SecondaryColour = Color.LavenderBlush
            Case "red"
                PrimaryColour = Color.FromArgb(195, 55, 55)
                SecondaryColour = Color.LavenderBlush
            Case "ppl"
                PrimaryColour = Color.MediumPurple
                SecondaryColour = Color.MistyRose
            Case "mon"
                PrimaryColour = Color.DimGray
                SecondaryColour = Color.Silver
            Case Else 'def
                PrimaryColour = Color.Peru
                SecondaryColour = Color.FloralWhite
        End Select
        Checkerboard.Refresh()
    End Sub



    'Subroutine that creates checkerboard pattern. (Alternates between light and dark colours,
    'and includes controls for Previously Used Squares, Legal Move Squares, and more.)
    Private Sub CreateBoard(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Checkerboard.Paint
        Dim g As Graphics = e.Graphics
        Dim isLight As Boolean = True
        For x = 0 To 7
            For y = 0 To 7
                Dim Square As New Rectangle(75 * x, 75 * y, 75, 75)
                'If the coordinates are set to True in the LegalMoveSquare, then that square should be coloured differently, so
                'that the user can see legal moves. Square given either a green or blue outline (depending on the colour scheme).
                If LegalMoveSquares(x, y) AndAlso GeneralOptions(4) = "T" Then
                    If PrimaryColour = Color.DarkSeaGreen Then 'is green colour scheme - colour blue.
                        Using Brush As New SolidBrush(Color.DarkTurquoise)
                            g.FillRectangle(Brush, Square)
                        End Using
                    Else 'is another colour scheme - colour green.
                        Using Brush As New SolidBrush(Color.LimeGreen)
                            g.FillRectangle(Brush, Square)
                        End Using
                    End If
                End If
                If isLight Then
                    If LegalMoveSquares(x, y) AndAlso GeneralOptions(4) = "T" Then 'Normal colour is filled at the centre of the legal move square to produce a green / blue highlight.
                        Dim Square2 As New Rectangle((75 * x) + 5, (75 * y) + 5, 65, 65)
                        Using Brush As New SolidBrush(SecondaryColour)
                            g.FillRectangle(Brush, Square2)
                        End Using
                        'If the given square matches a SquareHistory coordinate, then it is coloured in a different way.
                    ElseIf ((x = SquareHistory(0, 0) AndAlso y = SquareHistory(0, 1)) OrElse (x = SquareHistory(1, 0) AndAlso y = SquareHistory(1, 1))) AndAlso GeneralOptions(3) = "T" Then
                        Using Brush As New SolidBrush(Color.YellowGreen) 'SquareHistory secondary colour.
                            g.FillRectangle(Brush, Square)
                        End Using
                    Else 'Is a normal square - colour with user's seconday colour.
                        Using Brush As New SolidBrush(SecondaryColour)
                            g.FillRectangle(Brush, Square)
                        End Using
                    End If
                Else 'Identical code but for the dark squares (uses different colours).
                    If LegalMoveSquares(x, y) AndAlso GeneralOptions(4) = "T" Then
                        Dim Square2 As New Rectangle((75 * x) + 5, (75 * y) + 5, 65, 65)
                        Using Brush As New SolidBrush(PrimaryColour)
                            g.FillRectangle(Brush, Square2)
                        End Using
                    ElseIf ((x = SquareHistory(0, 0) AndAlso y = SquareHistory(0, 1)) OrElse (x = SquareHistory(1, 0) AndAlso y = SquareHistory(1, 1))) AndAlso GeneralOptions(3) = "T" Then
                        Using Brush As New SolidBrush(Color.OliveDrab) 'SquareHistory primary colour.
                            g.FillRectangle(Brush, Square)
                        End Using
                    Else
                        Using Brush As New SolidBrush(PrimaryColour) 'User's primary colour.
                            g.FillRectangle(Brush, Square)
                        End Using
                    End If
                End If
                isLight = Not isLight 'Produces alternating pattern.
            Next
            isLight = Not isLight
        Next
        'Gives the starting square for a piece a distinctive colour, when the user clicks on one, or when the piece is locked (touch move).
        If PieceMoving.LockedPiece <> "" Then
            Dim Square As New Rectangle(Val(PieceMoving.LockedPiece(0)) * 75, Val(PieceMoving.LockedPiece(1)) * 75, 75, 75)
            Using Brush As New SolidBrush(Color.LightCoral)
                g.FillRectangle(Brush, Square)
            End Using
        ElseIf PieceMoving.IsMovingPiece AndAlso GeneralOptions(4) = "T" Then 'the piece is being moved by the user.
            Dim Square As New Rectangle(PieceMoving.StartPoint.X, PieceMoving.StartPoint.Y, 75, 75)
            Using Brush As New SolidBrush(Color.LightCoral)
                g.FillRectangle(Brush, Square)
            End Using
        End If
    End Sub

    Private Sub ResetLMS(ByVal Refresh As Boolean)
        For y = 0 To 7
            For x = 0 To 7
                LegalMoveSquares(x, y) = False
            Next
        Next
        If Refresh Then Checkerboard.Refresh()
    End Sub


    'Algorithm that displays all the pieces on the board, given a board array.
    Private Sub DisplayPieces()
        Dim CurrentPiece As String
        Dim TempPiece As PictureBox
        Dim Counter As Byte
        'Hides all pieces.
        For x = 0 To 47
            PieceArray(x).Location = New Point(-100, -100)
            PieceArray(x).Visible = False
        Next
        For y = 7 To 0 Step -1
            For x = 0 To 7
                'For each piece on the board...
                If MasterBoard(x, y) <> " " Then
                    Counter = 1
                    While True
                        'In MasterBoard(), a lowercase letter represents a black piece, and an uppercase letter represents a white piece.
                        If Char.IsLower(MasterBoard(x, y)) Then
                            CurrentPiece = "B"
                        Else
                            CurrentPiece = "W"
                        End If
                        'CurrentPiece is formed, which has the same name as a corresponding PictureBox on the board.
                        CurrentPiece &= UCase(MasterBoard(x, y)) & Counter
                        'Searches for piece that has not been placed on the board (that matches CurrentPiece / TempPiece's name), and moves its match into place on the board.
                        TempPiece = Me.Controls.Find(CurrentPiece, True).Single()
                        If TempPiece.Visible = False Then
                            If OrientForWhite Then
                                TempPiece.Location = New Point(((x) * 75), ((75 * y)))
                            Else 'Location flipped 180 degrees.
                                TempPiece.Location = New Point(((7 - x) * 75), ((75 * (7 - y))))
                            End If

                            TempPiece.Visible = True
                            Exit While
                        End If
                        'If not found, the search continues with an increasing last digit (eg: the program seaches for BP1, then BP2, then BP3 ext until it finds its match.)
                        Counter += 1
                    End While
                End If
            Next
        Next
    End Sub

    'Outputs the board position, along with the TFTables & King positions (to the console).
    'Used for debugging purposes.
    Private Sub OutputDebugInfo()
        Dim Player As String
        Console.ForegroundColor = ConsoleColor.DarkYellow
        'Outputs the current FEN.
        Console.WriteLine(vbCrLf & vbCrLf & "Board Position: " & CurrentFEN)
        For y = 0 To 7
            For x = 0 To 7
                'Outputs the board position.
                If MasterBoard(x, y) = " " Then
                    Console.ResetColor()
                    Console.Write(ChrW(&H25AB)) 'Empty cell symbol.
                Else
                    'Colours piece depending if it is white's or black's piece.
                    If Char.IsUpper(MasterBoard(x, y)) Then
                        Console.ForegroundColor = ConsoleColor.White
                    Else
                        Console.ForegroundColor = ConsoleColor.DarkGray
                    End If
                    Console.Write(MasterBoard(x, y))
                End If
            Next
            Console.ResetColor()
            'Outputs the information of the player.
            If y = 0 Then
                If PlayerTurn Then Player = "White" Else Player = "Black"
                Console.Write(New String(" ", 4) & "Player Turn: " & Player)
            End If
            If y = 2 Then Console.Write(New String(" ", 4) & "WKPos: " & SharedAlgorithms.PosToCoorConverter(MasterWKPos))
            If y = 4 Then Console.Write(New String(" ", 4) & "BKPos: " & SharedAlgorithms.PosToCoorConverter(MasterBKPos))
            If y = 6 Then Console.Write(New String(" ", 4) & "En Passant: " & SharedAlgorithms.PosToCoorConverter(MasterEnPassant))
            Console.WriteLine()
        Next
        'Outputs the TFTables of each player.
        Console.WriteLine()
        Console.WriteLine("WhiteTFT:" & New String(" ", 5) & "BlackTFT:")
        For y = 0 To 7
            For x = 0 To 7
                'Colours the indexes depending if it is True, False, or the position of the player's king.
                If x & y = MasterWKPos Then
                    Console.ForegroundColor = ConsoleColor.DarkYellow
                ElseIf MasterWhiteTFTable(x, y) = "T" Then
                    Console.ForegroundColor = ConsoleColor.Green
                ElseIf MasterWhiteTFTable(x, y) = "F" Then
                    Console.ForegroundColor = ConsoleColor.Red
                Else
                    Console.ForegroundColor = ConsoleColor.Blue
                End If
                Console.Write(MasterWhiteTFTable(x, y))
            Next
            Console.Write(New String(" ", 6))
            For x = 0 To 7
                If x & y = MasterBKPos Then
                    Console.ForegroundColor = ConsoleColor.DarkYellow
                ElseIf MasterBlackTFTable(x, y) = "T" Then
                    Console.ForegroundColor = ConsoleColor.Green
                ElseIf MasterBlackTFTable(x, y) = "F" Then
                    Console.ForegroundColor = ConsoleColor.Red
                Else
                    Console.ForegroundColor = ConsoleColor.Blue
                End If
                Console.Write(MasterBlackTFTable(x, y))
            Next
            Console.WriteLine()
        Next
        'Outputs the Hash Value.
        Console.ForegroundColor = ConsoleColor.Magenta
        Console.WriteLine("Hash Value: " & MasterZobristValue)
        Console.ResetColor()
    End Sub


    'Alternate version of DisplayPieces which is more efficient and only converts a single piece in
    'MasterBoard to its PictureBox counterpart on the board.
    Private Function CoorToPieceConverter(ByVal CoorX As Int16, ByVal CoorY As Int16) As PictureBox
        Dim CurrentPiece As String
        Dim TempPiece As PictureBox = Nothing
        Dim Counter As Byte
        If MasterBoard(CoorX, CoorY) <> " " Then
            Counter = 1
            While True
                'In MasterBoard(), a lowercase letter represents a black piece, and an uppercase letter represents a white piece.
                If Char.IsLower(MasterBoard(CoorX, CoorY)) Then
                    CurrentPiece = "B"
                Else
                    CurrentPiece = "W"
                End If
                'CurrentPiece is formed, which has the same name as a corresponding PictureBox on the board.
                CurrentPiece &= UCase(MasterBoard(CoorX, CoorY)) & Counter
                'Searches for piece that has not been placed on the board (that matches CurrentPiece / TempPiece's name + location), and moves its match into place on the board.
                TempPiece = Me.Controls.Find(CurrentPiece, True).Single()
                If OrientForWhite AndAlso (TempPiece.Location.X = CoorX * 75 AndAlso TempPiece.Location.Y = CoorY * 75) Then
                    Exit While
                ElseIf Not OrientForWhite AndAlso (TempPiece.Location.X = (7 - CoorX) * 75 AndAlso TempPiece.Location.Y = (7 - CoorY) * 75) Then
                    Exit While
                End If
                'If not found, the search continues with an increasing last digit (eg: the program seaches for BP1, then BP2, then BP3 ext until it finds its match.)
                Counter += 1
            End While
        End If
        Return TempPiece
    End Function



    'Function that detects some errors in a user-entered FEN string, given the below rules:
    Private Function FENErrorDetection(ByVal FEN As String, ByVal OutputToBox As Boolean, ByRef FENErrorMessage As String) As Boolean
        'If OutputToBox = True then the error message is stored in InputTextBox. If it is false then the message is stored in FENErrorMessage.
        Dim MaxWQueens, MaxBQueens, WKingCount, BKingCount As Byte
        If FEN.Length > 22 AndAlso FEN.Length < 88 Then 'Length validation
            'Checks if each player has a king, and if they don't have too many queens & pawns (that could promote to a queen) - creating too
            'many queens on the board and therefore potentially overloading the PictureBox system.
            For n = 0 To Len(FEN) - 1
                If FEN(n) = " " Then Exit For 'Early exit clause.
                If FEN(n) = "P" OrElse FEN(n) = "Q" Then
                    MaxWQueens += 1
                ElseIf FEN(n) = "p" OrElse FEN(n) = "q" Then
                    MaxBQueens += 1
                ElseIf FEN(n) = "K" Then
                    WKingCount += 1
                ElseIf FEN(n) = "k" Then
                    BKingCount += 1
                End If
            Next
            'Displays appropriate error messages.
            If Not (WKingCount = 1 AndAlso BKingCount = 1) Then
                If OutputToBox Then
                    InvalidInput = FEN
                    InputTextBox.Text = "Position Rejected - Invalid number of Kings. Please Input a Genuinine FEN and try again."
                    UndoFENChange.Visible = True
                Else 'Returns message as string.
                    FENErrorMessage = "Position Rejected - No King."
                End If
                Return False
            ElseIf MaxWQueens > 9 OrElse MaxBQueens > 9 Then
                If OutputToBox Then
                    InvalidInput = FEN
                    InputTextBox.Text = "Position Rejected - Too many Pawns / Queens. Please Input a Genuinine FEN and try again."
                    UndoFENChange.Visible = True
                Else
                    FENErrorMessage = "Position Rejected - Too many Pawns / Queens."
                End If
                Return False
            End If
            'Has passed all tests :).
            Return True
        Else
            If OutputToBox Then
                InvalidInput = FEN
                InputTextBox.Text = "Position Rejected - Invalid FEN Length. Please Input a Genuinine FEN and try again."
                UndoFENChange.Visible = True
            Else
                FENErrorMessage = "Position Rejected - Invalid FEN Length."
            End If
            Return False
        End If
    End Function

    'Function that checks for invalid board positions - part of FEN Error Detection: checks for invalid checks & king positions.
    'Returns True if the game state is invalid.
    Private Function CheckForInvalidGameStates() As Boolean
        If MasterWInCheck.IsInCheck AndAlso Not PlayerTurn Then
            'If White is in check, and it is black to move, then black can take white's king. This is illegal.
            GameRunning = False
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("The Game has Ended. Cause = Invalid Check.")
            CheckForInvalidGameStates = True
        ElseIf MasterBInCheck.IsInCheck AndAlso PlayerTurn Then
            'If Black is in check, and it is white to move, then white can take black's king. This is illegal.
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("The Game has Ended. Cause = Invalid Check.")
            GameRunning = False
            CheckForInvalidGameStates = True
        ElseIf Math.Abs(Val(MasterWKPos(0)) - Val(MasterBKPos(0))) <= 1 AndAlso Math.Abs(Val(MasterWKPos(1)) - Val(MasterBKPos(1))) <= 1 Then
            'Kings are too close together.
            GameRunning = False
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("The Game has Ended. Cause = Invalid King Positions.")
            CheckLabel.Text = " Stalemate! "
            If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
            CheckForInvalidGameStates = True
        Else
            CheckForInvalidGameStates = False
        End If
        Console.ResetColor()
    End Function


    'Subroutine that formats & validates any moves being entered into the system via the InputTextBox, along with performing the move itself.
    'Supports multiple moves.
    Private Sub EnterMovesIntoSystem(ByVal Moves As String, ByVal OutputToTextBox As Boolean)
        'Formats PGN Moves. Some PGN Moves also contain move numbers - my program needs to remove these to interpret the moves.
        Dim FormattedPGN As String
        Try
            For n = 0 To Moves.Length - 1
                Select Case Moves(n)
                    Case "0" To "9"
                        If n = Moves.Length - 1 OrElse Not (Moves(n + 1) = "." OrElse Moves(n + 1) = "-" OrElse Moves(n + 1) = "/" OrElse (Moves(n + 1) >= "0" AndAlso Moves(n + 1) <= "9")) Then FormattedPGN &= Moves(n)
                    Case "#", "-", "/"
                        'End of the PGN sequence.
                        If Moves(n) = "-" Then
                            If Moves(n + 1) = "O" Then
                                FormattedPGN &= Moves(n)
                                Exit Select
                            End If
                        End If
                        Exit For
                    Case "."
                        If n = Moves.Length - 1 Then FormattedPGN = FormattedPGN.TrimEnd(" ")
                    Case " "
                        If Moves(n - 1) <> "." Then FormattedPGN &= Moves(n)
                    Case Else
                        FormattedPGN &= Moves(n)
                End Select
            Next
            If FormattedPGN = "" Then Throw New Exception("Invalid Input.")
        Catch ex As Exception
            'Unable to format moves - assume input is invalid.
            If OutputToTextBox Then
                InvalidInput = InputTextBox.Text
                UndoFENChange.Visible = True
                InputTextBox.Text = "Move Unable to be Formatted. Please Input a valid set of Moves and try again."
            Else
                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.WriteLine("Move Unable to be Formatted.")
                Console.ResetColor()
            End If
            Exit Sub
        End Try
        FormattedPGN = FormattedPGN.TrimEnd(" ")
        If OutputToTextBox Then InputTextBox.Text = FormattedPGN 'Outputs formatted PGN to InputTextBox.

        'Begins to interpret the moves.
        FormattedPGN &= " "
        Dim TempPGNMove As String
        Dim TempMove As New Move
        Dim FirstMove As Boolean = True
        Dim IsLegalMove As Boolean
        Do
            If Not FirstMove Then Thread.Sleep(25) 'Gives the user time to appreciate the moves being made.
            For n = 0 To FormattedPGN.Length - 1
                If FormattedPGN(n) = " " Then
                    'Isolates the PGN Move.
                    TempPGNMove = FormattedPGN.Substring(0, n)
                    FormattedPGN = FormattedPGN.Substring(n + 1)
                    Try
                        If PlayerTurn Then
                            'Attempts to convert the PGN move to my program's Move format.
                            TempMove = SharedAlgorithms.ConvertToMove(TempPGNMove, MasterBoard, True, MasterWKPos, MasterWhiteTFTable)
                            If TempMove.EndState = "a" Then Throw New Exception("Invalid Move.")
                            'Checks for all the legal moves that this piece can make, then compare these to the user's input move.
                            PieceMoving.LegalMoves = MainAI.ReturnPiecesLegalMoves(TempMove.OldMoveX, TempMove.OldMoveY)
                            IsLegalMove = False
                            For m = 1 To Val(PieceMoving.LegalMoves(0))
                                If TempMove.NewMoveX & TempMove.NewMoveY = PieceMoving.LegalMoves(m) Then
                                    'Move found - break out of loop.
                                    IsLegalMove = True
                                    Exit For
                                End If
                            Next
                            If Not IsLegalMove Then Throw New Exception("Illegal Move.")
                            'Performs the move.
                            AnimateMove(TempMove)
                            MakeMove(MasterBoard, TempMove.OldMoveX, TempMove.OldMoveY, TempMove.NewMoveX, TempMove.NewMoveY, MasterWCanCastle, MasterWKPos, MasterEnPassant, GeneralOptions(0) = "T")
                        Else 'Similar code for the black pieces.
                            TempMove = SharedAlgorithms.ConvertToMove(TempPGNMove, MasterBoard, False, MasterBKPos, MasterBlackTFTable)
                            If TempMove.EndState = "a" Then Throw New Exception("Invalid Move.")
                            PieceMoving.LegalMoves = MainAI.ReturnPiecesLegalMoves(TempMove.OldMoveX, TempMove.OldMoveY)
                            IsLegalMove = False
                            For m = 1 To Val(PieceMoving.LegalMoves(0))
                                If TempMove.NewMoveX & TempMove.NewMoveY = PieceMoving.LegalMoves(m) Then
                                    IsLegalMove = True
                                    Exit For
                                End If
                            Next
                            If Not IsLegalMove Then Throw New Exception("Illegal Move.")
                            AnimateMove(TempMove)
                            MakeMove(MasterBoard, TempMove.OldMoveX, TempMove.OldMoveY, TempMove.NewMoveX, TempMove.NewMoveY, MasterBCanCastle, MasterBKPos, MasterEnPassant, GeneralOptions(0) = "T")
                        End If
                    Catch ex As Exception
                        'Unable to perform move - assume move is invalid.
                        'Revert position to undo the effects of the invalid move.
                        MasterBoard = SharedAlgorithms.FENConverter(CurrentFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
                        DisplayPieces()
                        If Not FirstMove Then OutputDebugInfo()
                        'Constructs and outputs the error message.
                        Dim OutputResponce As String
                        If TempMove.EndState = "a" Then OutputResponce = "Invalid" Else OutputResponce = "Illegal"
                        If OutputToTextBox Then
                            InvalidInput = InputTextBox.Text
                            UndoFENChange.Visible = True
                            InputTextBox.Text = "Move (" & TempPGNMove & ") is " & OutputResponce & ". Please Input a valid set of Moves and try again."
                        Else
                            Console.ForegroundColor = ConsoleColor.DarkRed
                            Console.WriteLine("Move (" & TempPGNMove & ") is " & OutputResponce & ". Please Input a valid set of Moves and try again.")
                            Console.ResetColor()
                        End If
                        Exit Sub
                    End Try

                    If Not UndoFENChange.Visible Then 'Move is valid.
                        'Updates TFTable for appropriate player.
                        If PlayerTurn Then
                            MasterWInCheck.NotInCheck() 'Player is no longer in check.
                            SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterBInCheck, MasterEnPassant)
                        Else
                            MasterBInCheck.NotInCheck() 'Player is no longer in check.
                            SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterEnPassant)
                        End If

                        'Update Previously Used Squares, then flips the board if necessary.
                        If GameMode = 3 AndAlso FirstMove Then
                            SquareHistory(2, 0) = SquareHistory(0, 0)
                            SquareHistory(2, 1) = SquareHistory(0, 1)
                            SquareHistory(3, 0) = SquareHistory(1, 0)
                            SquareHistory(3, 1) = SquareHistory(1, 1)
                        End If
                        SquareHistory(0, 0) = TempMove.NewMoveX
                        SquareHistory(0, 1) = TempMove.NewMoveY
                        SquareHistory(1, 0) = TempMove.OldMoveX
                        SquareHistory(1, 1) = TempMove.OldMoveY
                        If Not OrientForWhite Then
                            SquareHistory(0, 0) = 7 - SquareHistory(0, 0)
                            SquareHistory(0, 1) = 7 - SquareHistory(0, 1)
                            SquareHistory(1, 0) = 7 - SquareHistory(1, 0)
                            SquareHistory(1, 1) = 7 - SquareHistory(1, 1)
                        End If
                        'Resets LegalMoveSquares.
                        ResetLMS(False)
                        Checkerboard.Refresh()
                        CheckChecker(False)

                        'Ends the turn, then calculates the new position's FEN.
                        PlayerTurn = Not PlayerTurn
                        If FirstMove Then PreviousFEN = CurrentFEN : FirstMove = False
                        CurrentFEN = SharedAlgorithms.ConvertToFEN(MasterBoard, MasterWCanCastle, MasterBCanCastle, MasterEnPassant, PlayerTurn)
                        MainAI.Reconfigure(CurrentFEN, False) 'Recalibrates AI.
                        EnforceEndStates(True)

                        Exit For
                    End If
                End If

            Next

        Loop While Len(FormattedPGN) > 0 AndAlso GameRunning
        'Once all the moves have been made, output the new board position.
        OutputDebugInfo()
    End Sub





    'The below three subroutines control the drag & drop mechanics for the pieces, and translates a user's move onto the board.
    Private Sub Piece_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles WK1.MouseDown, WQ1.MouseDown, WB1.MouseDown, WB2.MouseDown, WN1.MouseDown, WN2.MouseDown, WR1.MouseDown, WR2.MouseDown, WP1.MouseDown, WP2.MouseDown, WP3.MouseDown, WP4.MouseDown, WP5.MouseDown, WP6.MouseDown, WP7.MouseDown, WP8.MouseDown, BK1.MouseDown, BQ1.MouseDown, BB1.MouseDown, BB2.MouseDown, BN1.MouseDown, BN2.MouseDown, BR1.MouseDown, BR2.MouseDown, BP1.MouseDown, BP2.MouseDown, BP3.MouseDown, BP4.MouseDown, BP5.MouseDown, BP6.MouseDown, BP7.MouseDown, BP8.MouseDown, WQ8.MouseDown, WQ7.MouseDown, WQ6.MouseDown, WQ5.MouseDown, WQ4.MouseDown, WQ3.MouseDown, BQ6.MouseDown, BQ8.MouseDown, BQ5.MouseDown, BQ7.MouseDown, BQ2.MouseDown, BQ4.MouseDown, BQ3.MouseDown, WQ2.MouseDown, WQ9.MouseDown, BQ9.MouseDown
        'Subroutine that activates the drag + drog mechanic if the user clicks on a piece.
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso GameRunning AndAlso Not ComputerIsSearching AndAlso Not PieceIsMoving Then
            'If the piece corresponds to the player allowed to move...
            If (sender.name(0) = "W" AndAlso PlayerTurn) OrElse (sender.name(0) = "B" AndAlso (Not PlayerTurn)) Then
                'Checks if the piece is locked by the user. If it is not, then the user is able to proceed.
                If PieceMoving.LockedPiece = "" OrElse PieceMoving.LockedPiece = (sender.location.x / 75) & (sender.location.y / 75) Then
                    Me.Cursor = New Cursor(New System.IO.MemoryStream(My.Resources.Cursor16)) 'Can be Cursor32 or Cursor192
                    'Generates the Legal Moves for the piece the user has picked up (using the AI).
                    ResetLMS(False)
                    If OrientForWhite Then
                        PieceMoving.LegalMoves = MainAI.ReturnPiecesLegalMoves(sender.location.x / 75, sender.location.y / 75)
                        'Updates LegalMoveSquares array.
                        For n = 1 To Val(PieceMoving.LegalMoves(0))
                            LegalMoveSquares(Val(PieceMoving.LegalMoves(n)(0)), Val(PieceMoving.LegalMoves(n)(1))) = True
                        Next
                    Else
                        PieceMoving.LegalMoves = MainAI.ReturnPiecesLegalMoves(7 - (sender.location.x / 75), 7 - (sender.location.y / 75))
                        'Updates LegalMoveSquares array.
                        For n = 1 To Val(PieceMoving.LegalMoves(0))
                            LegalMoveSquares(7 - Val(PieceMoving.LegalMoves(n)(0)), 7 - Val(PieceMoving.LegalMoves(n)(1))) = True
                        Next
                    End If
                    'Creates the PieceMoving values, then redraws the checkerboard (creates LegalMoveSquares).
                    sender.bringtofront()
                    PieceMoving.IsMovingPiece = True
                    PieceMoving.Piece = sender
                    'Notes location of the piece for drag & drop mechanics.
                    PieceMoving.StartPoint = sender.location
                    PieceMoving.MidPoint = sender.PointToScreen(New Point(e.X, e.Y))
                    'If the user is using Touch Move, and the piece has >= 1 legal move, lock that piece so that the user cannot move any piece.
                    If GeneralOptions(5) = "T" AndAlso Val(PieceMoving.LegalMoves(0)) > 0 Then PieceMoving.LockedPiece = (sender.location.x / 75) & (sender.location.y / 75)
                    Checkerboard.Refresh()
                Else
                    'Stops the piece from being moved abruptly.
                    PieceMoving.StartPoint = sender.location
                End If
            Else
                'Stops the piece from being moved abruptly.
                PieceMoving.StartPoint = sender.location
            End If
        End If
    End Sub

    Private Sub Piece_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles WK1.MouseMove, WQ1.MouseMove, WB1.MouseMove, WB2.MouseMove, WN1.MouseMove, WN2.MouseMove, WR1.MouseMove, WR2.MouseMove, WP1.MouseMove, WP2.MouseMove, WP3.MouseMove, WP4.MouseMove, WP5.MouseMove, WP6.MouseMove, WP7.MouseMove, WP8.MouseMove, BK1.MouseMove, BQ1.MouseMove, BB1.MouseMove, BB2.MouseMove, BN1.MouseMove, BN2.MouseMove, BR1.MouseMove, BR2.MouseMove, BP1.MouseMove, BP2.MouseMove, BP3.MouseMove, BP4.MouseMove, BP5.MouseMove, BP6.MouseMove, BP7.MouseMove, BP8.MouseMove, WQ2.MouseMove, WQ3.MouseMove, WQ4.MouseMove, WQ5.MouseMove, WQ6.MouseMove, WQ7.MouseMove, WQ8.MouseMove, BQ2.MouseMove, BQ3.MouseMove, BQ4.MouseMove, BQ5.MouseMove, BQ6.MouseMove, BQ7.MouseMove, BQ8.MouseMove, WQ9.MouseMove, BQ9.MouseMove
        If PieceMoving.IsMovingPiece Then
            'The piece is bound to the position of the user's mouse.
            PieceMoving.EndPoint = sender.PointToScreen(New Point(e.X, e.Y))
            sender.Left += (PieceMoving.EndPoint.X - PieceMoving.MidPoint.X)
            sender.Top += (PieceMoving.EndPoint.Y - PieceMoving.MidPoint.Y)
            PieceMoving.MidPoint = PieceMoving.EndPoint
        End If
    End Sub

    Private Sub Piece_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles WK1.MouseUp, WQ1.MouseUp, WB1.MouseUp, WB2.MouseUp, WN1.MouseUp, WN2.MouseUp, WR1.MouseUp, WR2.MouseUp, WP1.MouseUp, WP2.MouseUp, WP3.MouseUp, WP4.MouseUp, WP5.MouseUp, WP6.MouseUp, WP7.MouseUp, WP8.MouseUp, BK1.MouseUp, BQ1.MouseUp, BB1.MouseUp, BB2.MouseUp, BN1.MouseUp, BN2.MouseUp, BR1.MouseUp, BR2.MouseUp, BP1.MouseUp, BP2.MouseUp, BP3.MouseUp, BP4.MouseUp, BP5.MouseUp, BP6.MouseUp, BP7.MouseUp, BP8.MouseUp, WQ2.MouseUp, WQ3.MouseUp, WQ4.MouseUp, WQ5.MouseUp, WQ6.MouseUp, WQ7.MouseUp, WQ8.MouseUp, BQ2.MouseUp, BQ3.MouseUp, BQ4.MouseUp, BQ5.MouseUp, BQ6.MouseUp, BQ7.MouseUp, BQ8.MouseUp, WQ9.MouseUp, BQ9.MouseUp
        Dim JustCastled, IsLegalMove As Boolean
        Dim ReplacedPiece As PictureBox = Nothing
        Dim TempPoint As Point
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso PieceMoving.IsMovingPiece Then
            Me.Cursor = Cursors.Default 'Resets cursor.
            'Disables drag & drop mechanic and sets the piece's position on the board (into the centre of the square).
            PieceMoving.IsMovingPiece = False
            sender.location = New Point(sender.location.X + 37.5 - (sender.location.X + 37.5) Mod 75, sender.location.y + 37.5 - (sender.location.y + 37.5) Mod 75)
            'Resets LegalMoveSquares array & redraws checkerboard.
            ResetLMS(True)
            'If the user has actually moved the piece, and the move is legal, the program continues.
            If sender.location <> PieceMoving.StartPoint Then

                TempPoint.X = sender.location.x / 75
                TempPoint.Y = sender.location.y / 75
                If Not OrientForWhite Then 'Flips coordinates 180 degrees.
                    TempPoint.X = 7 - TempPoint.X
                    TempPoint.Y = 7 - TempPoint.Y
                    PieceMoving.StartPoint.X = 525 - PieceMoving.StartPoint.X
                    PieceMoving.StartPoint.Y = 525 - PieceMoving.StartPoint.Y
                End If
                'Checks if the user's move is in the list of that piece's legal moves.
                IsLegalMove = False
                For n = 1 To Val(PieceMoving.LegalMoves(0))
                    If TempPoint.X & TempPoint.Y = PieceMoving.LegalMoves(n) Then
                        IsLegalMove = True
                        Exit For
                    End If
                Next

                If IsLegalMove AndAlso GameMode = 4 Then
                    'Checks if the move matches the correct puzzle move.
                    'Retrieves the correct puzzle move.
                    Dim CorrectMove As New Move
                    CorrectMove = PuzzleSampleDatabase(CurrentPuzzleIndex).GetMove(NoOfPuzzleMovesComplete * 2 + 1)

                    If Not ((CorrectMove.OldMoveX = PieceMoving.StartPoint.X / 75 AndAlso CorrectMove.OldMoveY = PieceMoving.StartPoint.Y / 75) AndAlso (CorrectMove.NewMoveX = TempPoint.X AndAlso CorrectMove.NewMoveY = TempPoint.Y)) Then
                        'User's move does not match the correct puzzle move, and is therefore incorrect.
                        Sound_Incorrect.Play()
                        If Not NextPuzzleBtn.Visible Then CalculateRatingLost(True, True) 'Calculates & Enforces rating loss.
                        LostRatingLabel.ForeColor = Color.Red
                        If AutoAdvanceOnComplete.Checked Then NextPuzzleBtn_Click() Else NextPuzzleBtn.Visible = True
                        IsLegalMove = False
                    End If

                End If

                If IsLegalMove AndAlso GameMode <> 6 Then
                    PieceMoving.LockedPiece = ""
                    'Resets checking rules.
                    If PlayerTurn Then
                        MasterWInCheck.NotInCheck()
                    Else
                        MasterBInCheck.NotInCheck()
                    End If
                    If UCase(sender.name(1)) = "K" Then
                        'Disables Castling if the King has moved.
                        If sender.name(0) = "W" Then
                            MasterWKPos = TempPoint.X & TempPoint.Y
                            MasterWCanCastle.CannotCastle()
                        Else
                            MasterBKPos = TempPoint.X & TempPoint.Y
                            MasterBCanCastle.CannotCastle()
                        End If
                        'Code for Castling (Queen Side & King Side).
                        If (TempPoint.X & TempPoint.Y) = ((PieceMoving.StartPoint.X / 75) + 2 & PieceMoving.StartPoint.Y / 75) Then
                            JustCastled = True
                            If GeneralOptions(0) = "T" Then Sound_Castle.Play()
                            'Adjusts rook position.
                            If OrientForWhite Then
                                CoorToPieceConverter(7, TempPoint.Y).Left -= 150
                            Else
                                CoorToPieceConverter(7, TempPoint.Y).Left += 150
                            End If
                            MasterBoard(5, TempPoint.Y) = MasterBoard(7, TempPoint.Y)
                            MasterBoard(7, TempPoint.Y) = " "
                        ElseIf (TempPoint.X & TempPoint.Y) = ((PieceMoving.StartPoint.X / 75) - 2 & PieceMoving.StartPoint.Y / 75) Then
                            'Queenside castling.
                            JustCastled = True
                            If GeneralOptions(0) = "T" Then Sound_Castle.Play()
                            'Adjusts rook position.
                            If OrientForWhite Then
                                CoorToPieceConverter(0, TempPoint.Y).Left += 225
                            Else
                                CoorToPieceConverter(0, TempPoint.Y).Left -= 225
                            End If
                            MasterBoard(3, TempPoint.Y) = MasterBoard(0, TempPoint.Y)
                            MasterBoard(0, TempPoint.Y) = " "
                        End If
                    End If

                    'If the move is a capture, the required sound is played and the captured piece is removed from the board.
                    If MasterBoard(TempPoint.X, TempPoint.Y) <> " " Then
                        If GeneralOptions(0) = "T" Then Sound_Capture.Play()
                        ReplacedPiece = CoorToPieceConverter(TempPoint.X, TempPoint.Y) 'Finds captured PictureBox.
                        ReplacedPiece.Visible = False
                        ReplacedPiece.Location = New Point(-100, -100)
                    ElseIf JustCastled = False Then
                        If GeneralOptions(0) = "T" Then Sound_Move.Play()
                    End If
                    'MasterBoard is updated by clearing the piece's previous piosition and reallocating its current position.
                    MasterBoard(PieceMoving.StartPoint.X / 75, PieceMoving.StartPoint.Y / 75) = " "

                    'Updates SquareHistory array,
                    SquareHistory(2, 0) = SquareHistory(0, 0)
                    SquareHistory(2, 1) = SquareHistory(0, 1)
                    SquareHistory(3, 0) = SquareHistory(1, 0)
                    SquareHistory(3, 1) = SquareHistory(1, 1)
                    If OrientForWhite Then
                        SquareHistory(0, 0) = PieceMoving.StartPoint.X / 75
                        SquareHistory(0, 1) = PieceMoving.StartPoint.Y / 75
                        SquareHistory(1, 0) = TempPoint.X
                        SquareHistory(1, 1) = TempPoint.Y
                    Else
                        SquareHistory(0, 0) = 7 - (PieceMoving.StartPoint.X / 75)
                        SquareHistory(0, 1) = 7 - (PieceMoving.StartPoint.Y / 75)
                        SquareHistory(1, 0) = 7 - TempPoint.X
                        SquareHistory(1, 1) = 7 - TempPoint.Y
                    End If

                    'Handles moving a piece to its new coordinates, along with special pawn moves.
                    If sender.name(0) = "W" Then
                        MasterBoard(TempPoint.X, TempPoint.Y) = UCase(sender.name(1))
                        'If a pawn has made it to the end of the board, it is promoted to a Queen.
                        If sender.name(1) = "P" Then
                            If TempPoint.Y = 0 Then
                                'Pawn promotion - finds a queen to replace the pawn.
                                For n = 1 To 9
                                    ReplacedPiece = Me.Controls.Find("WQ" & n.ToString, True).Single()
                                    If ReplacedPiece.Visible = False Then Exit For
                                Next
                                ReplacedPiece.Location = New Point(sender.location.X, sender.location.Y)
                                ReplacedPiece.Visible = True
                                ReplacedPiece.BringToFront()
                                'Replaces the Board index with a queen, then removes the pawn PictureBox from the screen.
                                MasterBoard(TempPoint.X, TempPoint.Y) = "Q"
                                sender.Visible = False
                                sender.Location = New Point(-100, -100)
                            ElseIf TempPoint.X & TempPoint.Y = MasterEnPassant Then
                                'Rules for En Passant - removes the piece behind it.
                                If GeneralOptions(0) = "T" Then Sound_Capture.Play()
                                ReplacedPiece = CoorToPieceConverter(TempPoint.X, TempPoint.Y + 1)
                                ReplacedPiece.Visible = False
                                ReplacedPiece.Location = New Point(-100, -100)
                                MasterBoard(TempPoint.X, (TempPoint.Y) + 1) = " "
                            End If
                        End If
                        'If the rook has moved, then part of castling is disabled.
                        If sender.name(1) = "R" Then
                            If sender.name(2) = "1" Then
                                MasterWCanCastle.QS = False
                            Else
                                MasterWCanCastle.KS = False
                            End If
                        End If
                        'The TrueFalse Table is generated for the Black Pieces.
                        SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterBInCheck, MasterEnPassant)
                    Else
                        MasterBoard(TempPoint.X, TempPoint.Y) = LCase(sender.name(1))
                        'Identical Code as above but for the black pieces.
                        If sender.name(1) = "P" Then
                            If TempPoint.Y = 7 Then
                                For n = 1 To 9
                                    ReplacedPiece = Me.Controls.Find("BQ" & n.ToString, True).Single()
                                    If ReplacedPiece.Visible = False Then Exit For
                                Next
                                ReplacedPiece.Location = New Point(sender.location.X, sender.location.Y)
                                ReplacedPiece.Visible = True
                                MasterBoard(TempPoint.X, TempPoint.Y) = "q"
                                sender.Visible = False
                                sender.Location = New Point(-100, -100)
                            ElseIf TempPoint.X & TempPoint.Y = MasterEnPassant Then
                                If GeneralOptions(0) = "T" Then Sound_Capture.Play()
                                ReplacedPiece = CoorToPieceConverter(TempPoint.X, TempPoint.Y - 1)
                                ReplacedPiece.Visible = False
                                ReplacedPiece.Location = New Point(-100, -100)
                                MasterBoard(TempPoint.X, (TempPoint.Y) - 1) = " "
                            End If
                        End If
                        If sender.name(1) = "R" Then
                            If sender.name(2) = "1" Then
                                MasterBCanCastle.QS = False
                            Else
                                MasterBCanCastle.KS = False
                            End If
                        End If
                        'The TrueFalse Table is generated for the White Pieces.
                        SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterEnPassant)
                    End If

                    'En Passant Generation.
                    If Not OrientForWhite Then
                        PieceMoving.StartPoint.X = 525 - PieceMoving.StartPoint.X
                        PieceMoving.StartPoint.Y = 525 - PieceMoving.StartPoint.Y
                    End If
                    If sender.name(1) = "P" AndAlso Math.Abs(sender.location.Y - PieceMoving.StartPoint.Y) = 150 Then
                        If sender.name(0) = "W" AndAlso (MasterBoard(Math.Max((TempPoint.X) - 1, 0), TempPoint.Y) = "p" OrElse MasterBoard(Math.Min((TempPoint.X) + 1, 7), TempPoint.Y) = "p") Then
                            MasterEnPassant = TempPoint.X & 5
                        ElseIf sender.name(0) = "B" AndAlso (MasterBoard(Math.Max((TempPoint.X) - 1, 0), TempPoint.Y) = "P" OrElse MasterBoard(Math.Min((TempPoint.X) + 1, 7), TempPoint.Y) = "P") Then
                            MasterEnPassant = TempPoint.X & 2
                        Else 'No En Passant exists.
                            MasterEnPassant = "-"
                        End If
                    Else 'Remove En Passant.
                        MasterEnPassant = "-"
                    End If

                    If AutoFlipper.Checked Then
                        FlipBoard()
                    Else 'Redraws checkerboard.
                        Checkerboard.Refresh()
                    End If
                    'Checks if any player is in check, and if so notifies the player with audio and visuals.
                    CheckChecker(False)

                    'Starts the next turn by updating the Current FEN and checking for end positions.
                    PlayerTurn = Not PlayerTurn
                    PreviousFEN = CurrentFEN
                    CurrentFEN = SharedAlgorithms.ConvertToFEN(MasterBoard, MasterWCanCastle, MasterBCanCastle, MasterEnPassant, PlayerTurn)
                    If GameMode <> 4 Then
                        MainAI.Reconfigure(CurrentFEN, False) 'Recalibrates AI before checking for end states.
                        EnforceEndStates(True)
                    End If
                    If CurrentFEN = PreviousFEN AndAlso UserPlayer = PlayerTurn Then Exit Sub 'Stops AI from running if it is the start position (and it is the user's turn).

                    If GameMode <= 3 Then
                        OutputDebugInfo()
                        If GameMode < 3 Then
                            FENExport_Click()
                        End If
                        If GameMode = 1 Then InitialiseAISystem()
                    ElseIf GameMode = 4 Then
                        'Move matches the correct puzzle move, so either play the next move of the puzzle, or flag the puzzle as complete.
                        CancelAIPuzzleSearch()
                        MainAI.Reconfigure(CurrentFEN, False) 'Recalibrates AI before checking for end states.
                        EnforceEndStates(True)
                        PuzzleMoveCompleted()
                    End If

                Else
                    If IsLegalMove Then
                        'The user is playing the Move Training Game, and the move they entered may be correct - test move.
                        Dim TempMove As New Move 'Represents the user's move.
                        TempMove.OldMoveX = PieceMoving.StartPoint.X / 75
                        TempMove.OldMoveY = PieceMoving.StartPoint.Y / 75
                        TempMove.NewMoveX = TempPoint.X
                        TempMove.NewMoveY = TempPoint.Y
                        If SharedAlgorithms.MoveConverter(MasterBoard, TempMove, MasterEnPassant) = MoveDisplayer.Text Then
                            'The move is correct - notify the user.
                            TrainingScore.Text = CInt(TrainingScore.Text) + 1
                            If GeneralOptions(0) = "T" Then Sound_Check.Play()
                            TrainingMovesCompleted += 1
                            If TrainingMovesCompleted = MovesPerPosition Then
                                'Generate new position & move.
                                TrainingMovesCompleted = 0
                                MovesInPosition = RetrieveRandomPosition()
                                DisplayPieces()
                                MoveDisplayer.Text = GenerateNewTrainingMove()
                                Exit Sub
                            Else
                                'Generate new move and light up correct square.
                                If OrientForWhite Then LegalMoveSquares(TempPoint.X, TempPoint.Y) = True Else LegalMoveSquares(7 - TempPoint.X, 7 - TempPoint.Y) = True
                                Checkerboard.Refresh()
                                MoveDisplayer.Text = GenerateNewTrainingMove()
                            End If
                        End If
                    End If
                    'Resets piece to previous position
                    If Not OrientForWhite Then
                        PieceMoving.StartPoint.X = 525 - PieceMoving.StartPoint.X
                        PieceMoving.StartPoint.Y = 525 - PieceMoving.StartPoint.Y
                    End If
                    sender.location = PieceMoving.StartPoint
                End If
            End If
        End If
    End Sub


    'Subroutine which updates the Sprites / Gamestate Textbox depending on whether a player is in check (or not).
    Private Sub CheckChecker(ByVal OnlyEditKing As Boolean)
        If Not OnlyEditKing Then
            If MasterWInCheck.IsInCheck OrElse MasterBInCheck.IsInCheck Then
                If GeneralOptions(0) = "T" Then Sound_Check.Play()
                CheckLabel.Text = "    Check!    "
            Else
                CheckLabel.Text = "                    "
            End If
        End If
        'Modifies king picture accordingly.
        If GeneralOptions(6) = "F" Then
            If MasterWInCheck.IsInCheck AndAlso GeneralOptions(4) = "T" Then
                WK1.Image = Image.FromFile(Application.StartupPath & "\Images\Default\WKingCheck.png")
            Else
                WK1.Image = Image.FromFile(Application.StartupPath & "\Images\Default\WKing.png")
            End If
            If MasterBInCheck.IsInCheck AndAlso GeneralOptions(4) = "T" Then
                BK1.Image = Image.FromFile(Application.StartupPath & "\Images\Default\BKingCheck.png")
            Else
                BK1.Image = Image.FromFile(Application.StartupPath & "\Images\Default\BKing.png")
            End If
        End If
    End Sub



    'Subroutine that animates a piece move (given a Move).
    Private Sub AnimateMove(ByVal TempMove As Move)
        PieceIsMoving = True
        'Locates TempPiece (the piece that is moving) and ReplacedPiece (the piece that is being captured).
        Dim TempPiece As PictureBox = CoorToPieceConverter(TempMove.OldMoveX, TempMove.OldMoveY)
        Dim ReplacedPiece As PictureBox
        If TempMove.NewMoveX & TempMove.NewMoveY = MasterEnPassant Then
            'The captured pawn is located just behind the capturing pawn.
            ReplacedPiece = CoorToPieceConverter(TempMove.NewMoveX, TempMove.NewMoveY - (2 * Val(PlayerTurn) + 1))
        Else
            ReplacedPiece = CoorToPieceConverter(TempMove.NewMoveX, TempMove.NewMoveY)
        End If

        '"Flips" the move 180 degrees if the board is flipped in black's favour.
        If Not OrientForWhite Then
            TempMove.OldMoveX = 7 - TempMove.OldMoveX
            TempMove.OldMoveY = 7 - TempMove.OldMoveY
            TempMove.NewMoveX = 7 - TempMove.NewMoveX
            TempMove.NewMoveY = 7 - TempMove.NewMoveY
        End If

        'Creates movement vector.
        Dim XMovement As Decimal = 75 * (TempMove.NewMoveX - TempMove.OldMoveX) 'Represents delta x.
        Dim YMovement As Decimal = 75 * (TempMove.NewMoveY - TempMove.OldMoveY) 'Represents delta y.
        Dim Constant As Decimal 'Represents how many iterations to perform (more iterations = smoother movement).
        If AnimationSpeed = 0 Then
            Constant = 1
        ElseIf AnimationSpeed = 1 Then
            Constant = 25
        Else
            Constant = 75
        End If

        'Moves the piece pixel by pixel towards its destination - updating the board as it does so.
        TempPiece.BringToFront()
        For x = 1 To Constant
            TempPiece.Left += XMovement / Constant
            TempPiece.Top += YMovement / Constant
            If AnimationSpeed = 2 Then TempPiece.Refresh() Else Application.DoEvents()
        Next

        'Pawn promotion control.
        If TempPiece.Name(1) = "P" AndAlso TempMove.NewMoveY = -7 * Val(PlayerTurn Xor OrientForWhite) Then
            'Finds an available queen.
            Dim QueenPiece As PictureBox
            For n = 1 To 9
                QueenPiece = Me.Controls.Find(TempPiece.Name(0) & "Q" & n.ToString, True).Single()
                If QueenPiece.Visible = False Then Exit For
            Next
            'Removes the pawn and places the queen at the required coorinates.
            QueenPiece.Location = New Point(75 * TempMove.NewMoveX, 75 * TempMove.NewMoveY)
            QueenPiece.Visible = True
            TempPiece.Visible = False
            TempPiece.Location = New Point(-100, -100)
        End If
        If ReplacedPiece IsNot Nothing Then 'Is a capture move - remove the captured piece.
            ReplacedPiece.Visible = False
            ReplacedPiece.Location = New Point(-100, -100)
        ElseIf TempPiece.Name(1) = "K" AndAlso (MasterWCanCastle.CanICastle OrElse MasterBCanCastle.CanICastle) Then
            If Not OrientForWhite Then
                TempMove.OldMoveX = 7 - TempMove.OldMoveX
                TempMove.OldMoveY = 7 - TempMove.OldMoveY
                TempMove.NewMoveX = 7 - TempMove.NewMoveX
                TempMove.NewMoveY = 7 - TempMove.NewMoveY
            End If
            'Castling Controls.
            If (TempMove.NewMoveX & TempMove.NewMoveY) = (TempMove.OldMoveX + 2 & TempMove.OldMoveY) Then
                'Adjusts rook position.
                If OrientForWhite Then
                    CoorToPieceConverter(7, TempMove.NewMoveY).Left -= 150
                Else
                    CoorToPieceConverter(7, TempMove.NewMoveY).Left += 150
                End If
            ElseIf (TempMove.NewMoveX & TempMove.NewMoveY) = (TempMove.OldMoveX - 2 & TempMove.OldMoveY) Then
                'Adjusts rook position.
                If OrientForWhite Then
                    CoorToPieceConverter(0, TempMove.NewMoveY).Left += 225
                Else
                    CoorToPieceConverter(0, TempMove.NewMoveY).Left -= 225
                End If
            End If

        End If
        PieceIsMoving = False 'Animation has finished - allow user to move another piece.
    End Sub





    'Algorithm which calculates the Starting Depth the AI will run at. Takes into account the Total Material Count
    'on the board and calculates Depth using the formula y = -m*ln(x-a) + c (the higher the Material Count, the lower
    'the Starting Depth).
    Private Sub CalculateStartingDepth()
        If FixedSearchDepth = 0 Then
            'Counts the material on the board.
            Dim MaterialArray As Int16() = SharedAlgorithms.CountMaterial(MasterBoard)
            Dim TotalMaterial As Byte = (MaterialArray(0) + MaterialArray(1)) / 100
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
        Dim UB As Integer = OpeningBook.Count 'Upper Bound of Binary Search.
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
        If GameRunning AndAlso Not ComputerIsSearching Then
            Dim BestMove As Move = MainAI.CheckForEndState() 'Ensures that the position is valid, and that the
            'AIs won't crash whilst sea rching on the position.
            If BestMove.EndState = "f" OrElse BestMove.EndState = "o" Then
                Dim IndexInBook As UInt32
                If UseBook.Checked Then IndexInBook = FindPositionInBook(CurrentFEN) 'Finds the index of the opening book that the position exists in.
                Console.WriteLine()
                If BestMove.EndState = "o" Then
                    'Move was forced - make move instantly without searching.
                    Console.WriteLine(vbCrLf & "Search Aborted - Only 1 Move in Position.")
                    CurrentAIDepth.Text = "Current Depth: 1"
                    CurrentAIMove.Text = "Current Move: " & SharedAlgorithms.MoveConverter(MasterBoard, BestMove, MasterEnPassant) & " (FORCED)."
                ElseIf IndexInBook > 0 Then 'Position found in book - play move from book instantly.

                    'Fully instantiates the entry in the opening book, so that we can retrieve its moves.
                    If Not OpeningBook(IndexInBook).GetComputed() Then OpeningBook(IndexInBook).ComputeEntry()
                    Console.ForegroundColor = ConsoleColor.DarkYellow
                    Console.WriteLine(vbCrLf & "Search Aborted - Position Found in Opening Book.")
                    Console.WriteLine("Moves Available: " & OpeningBook(IndexInBook).ReturnAllMoves() & ". Picking random move...")
                    'Locates a random book move from the position, then converts that to a user-friendly move.
                    Dim ChosenBookMove As String = OpeningBook(IndexInBook).ReturnRndMove()
                    If PlayerTurn Then
                        BestMove = SharedAlgorithms.ConvertToMove(ChosenBookMove, MasterBoard, True, MasterWKPos, MasterWhiteTFTable)
                    Else
                        BestMove = SharedAlgorithms.ConvertToMove(ChosenBookMove, MasterBoard, False, MasterBKPos, MasterBlackTFTable)
                    End If
                    CurrentAIDepth.Text = "Current Depth: 0"
                    CurrentAIMove.Text = "Current Move: " & ChosenBookMove & " (Book)."
                    CurrentAIEval.Text = "Evaluation: 0"

                Else
                    'no forced or book move found - call AI.
                    BestMove = InitialiseAI()
                End If
                Console.ResetColor()

                If Me.IsHandleCreated Then 'If form is still open...
                    GameRunning = True
                    'Makes the best move on the board, and updates the correct TFTable.

                    AnimateMove(BestMove)
                    If PlayerTurn Then
                        MasterWInCheck.NotInCheck() 'Player is no longer in check.
                        MakeMove(MasterBoard, BestMove.OldMoveX, BestMove.OldMoveY, BestMove.NewMoveX, BestMove.NewMoveY, MasterWCanCastle, MasterWKPos, MasterEnPassant, GeneralOptions(0) = "T")
                        SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterBInCheck, MasterEnPassant)
                    Else
                        MasterBInCheck.NotInCheck() 'Player is no longer in check.
                        MakeMove(MasterBoard, BestMove.OldMoveX, BestMove.OldMoveY, BestMove.NewMoveX, BestMove.NewMoveY, MasterBCanCastle, MasterBKPos, MasterEnPassant, GeneralOptions(0) = "T")
                        SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterEnPassant)
                    End If

                    'Update Previously Used Squares, then flips the board if necessary.
                    If GameMode = 3 Then
                        SquareHistory(2, 0) = SquareHistory(0, 0)
                        SquareHistory(2, 1) = SquareHistory(0, 1)
                        SquareHistory(3, 0) = SquareHistory(1, 0)
                        SquareHistory(3, 1) = SquareHistory(1, 1)
                    End If
                    SquareHistory(0, 0) = BestMove.NewMoveX
                    SquareHistory(0, 1) = BestMove.NewMoveY
                    SquareHistory(1, 0) = BestMove.OldMoveX
                    SquareHistory(1, 1) = BestMove.OldMoveY
                    If Not OrientForWhite Then
                        SquareHistory(0, 0) = 7 - SquareHistory(0, 0)
                        SquareHistory(0, 1) = 7 - SquareHistory(0, 1)
                        SquareHistory(1, 0) = 7 - SquareHistory(1, 0)
                        SquareHistory(1, 1) = 7 - SquareHistory(1, 1)
                    End If
                    'Resets LegalMoveSquares.
                    ResetLMS(False)
                    If AutoFlipper.Checked Then
                        FlipBoard()
                    Else
                        Checkerboard.Refresh()
                    End If
                    CheckChecker(False)

                    'Ends the turn, then calculates the new position's FEN.
                    PlayerTurn = Not PlayerTurn
                    If GameMode = 3 Then PreviousFEN = CurrentFEN
                    CurrentFEN = SharedAlgorithms.ConvertToFEN(MasterBoard, MasterWCanCastle, MasterBCanCastle, MasterEnPassant, PlayerTurn)
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
                    EnforceEndStates(Not GameMode = 1) 'Checks for end states found from the AI's move.
                    'As two moves are made in one game 'state', we do not copy BoardHistory to its Buffer (so we don't false-trigger 3FR).
                    If GameMode <> 3 Then FENExport_Click()
                    OutputDebugInfo()
                    'The below lines fix a bug, where the AI wouldn't make a move after the user restarts their game (1P mode only).
                    If AIEndlessMode.Checked OrElse (GameMode = 1 AndAlso CurrentFEN = PreviousFEN AndAlso Not UserPlayer = PlayerTurn) Then
                        Thread.Sleep(5) 'Allows the system to recalibrate (to prevent spam).
                        'If UserPlayer Xor OrientForWhite Then FlipBoard()
                        InitialiseAISystem()
                    End If

                End If
            End If
        End If
    End Sub

    'Algorithm that creates all the threads & AIs that will search on a position. Handles time management & overtime,
    'interactions with the GUI chessboard, live updates via GUI objects, selecting best moves, and more.
    Private Function InitialiseAI() As Move
        Dim BestMove As New Move
        Dim IsLate As Boolean 'Represents whether the computer has used more time than it was originally allocated.
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
        MainAI.AddBoardHistory(BoardHistory.GetArray())
        MainAI.ConfigureSettings(SearchSettings)

        'Begins the search.
        ComputerIsSearching = True
        Console.ForegroundColor = ConsoleColor.DarkCyan
        If TimeForSearch = Decimal.MaxValue Then
            Console.Write(vbCrLf & "The AI is now searching..." & vbCrLf & "Search Time = Infinity. ")
        Else
            Console.Write(vbCrLf & "The AI is now searching..." & vbCrLf & "Search Time = " & TimeForSearch & " Seconds. ")
        End If
        Console.WriteLine("Quiescence: " & SearchSettings.UseQuiescence & ", Heat Maps: " & SearchSettings.UsePieceHeatMaps & "." & vbCrLf)
        Console.ResetColor()
        GC.Collect() 'Calls a Garbage Collection just before the AI begins searching - ensures that the program's
        'memory is in the best state possible before using MiniMax (which can be very resource intensive).

        AIStopwatch.Reset()
        AIStopwatch.Start()

        'Starts the AI's thread.
        AIThread.Start()

        'Whilst the AI thread is running...
        While Not AIThread.IsCompleted
            Application.DoEvents() 'Allows the user to interact with the GUI (limited).
            'Updates progress bar to reflect time.
            If IsLate Then
                '2x speed with red colour.
                ProgressBar.Value = Math.Min((Math.Max(AIStopwatch.ElapsedMilliseconds - TimeForSearch * 1000, 0) / 5) / TimeForSearch, 100)
            Else 'Normal speed with green colour.
                ProgressBar.Value = Math.Min(((AIStopwatch.ElapsedMilliseconds / 10) / TimeForSearch), 100)
            End If

            If TerminateSearch OrElse AIStopwatch.ElapsedMilliseconds / 1000 > TimeForSearch OrElse Not Me.IsHandleCreated Then 'Time's up!
                If AIBestMove.EndState = "a" Then
                    'There has been no move performed in the current amount of time - enable overtime.
                    IsLate = True
                    ProgressBar.ForeColor = Color.Red 'Red progress bar - visual indication to user that time is up.
                Else
                    'Force the AI to finish after its current search.
                    MainAI.ABORTSearch()
                End If
                '1.5 * time (or extra 10s) has expired - terminate AI immediately!
                If TerminateSearch OrElse AIStopwatch.ElapsedMilliseconds / 1000 > Math.Min(TimeForSearch * 1.5, TimeForSearch + 10000) OrElse Not Me.IsHandleCreated Then MainAI.ABORTSearch()

            ElseIf AIFinishedSearch Then 'A new depth has been searched to - update GUI controls on latest search & redraw checkerboard.
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
                AIFinishedSearch = False
            End If
            Thread.Sleep(5) 'Allows more processing time to be spent on the AIs.
        End While
        AIStopwatch.Stop()
        If TerminateSearch Then Console.ForegroundColor = ConsoleColor.Red : Console.WriteLine("Search Terminated Preemptively (by user).")

        'Select the best move of the highest depth possible (if that search has not been aborted).
        If AIBestMove.EndState <> "a" Then
            BestMove = AIBestMove
        Else 'The AI has not successfully completed a search - start a new search at a depth of 2 (without Quiescence).
            'Sets the result of this new move to be the best move.
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("No Search Completed in Allocated time - Performing Shallow Search...")
            BestMove = MainAI.Search(2)
            CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, BestMove, MasterEnPassant)
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
        MainAI.RemoveBoardHistory(BoardHistory.GetArray())

        Return BestMove
    End Function



    'Subroutines controlling the AI thread. Variable Search = the AI starts at a depth of StartingDepth, and increments by one after each search (iterative deepening).
    'Fixed Search = the AI only performs one search, but at the specified depth that the user provided.
    Private Sub VariableAISearchHandler()
        Dim AICurrentMove As New Move
        Dim CurrentSearchStopwatch As New Stopwatch
        Dim CurrentAIDepth As Byte = StartingDepth
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
            If AICurrentMove.EndState = "a" Then
                'Search cannot be used (no valid / reliable) move found - exit search process.
                Exit While
            Else
                'Outputs the timings of the current search.
                AIBestMove = AICurrentMove
                Console.ForegroundColor = ConsoleColor.DarkGreen
                Console.Write("Depth Of " & CurrentAIDepth)
                If SearchSettings.UseQuiescence Then Console.Write("-" & MainAI.GetHighestQuiescenceDepth())
                Console.WriteLine(" Completed in: " & AIStopwatch.ElapsedMilliseconds.ToString("N0") & " Milliseconds (" & CurrentSearchStopwatch.ElapsedMilliseconds.ToString("N0") & "ms)")
                Console.ResetColor()

                If GameMode = 4 Then
                    If MainAI.GetABORTState() Then
                        'Time to search on puzzle has expired - stop the search process.
                        AIStopwatch.Stop()
                        Exit While
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
                            Exit While
                        Else
                            'AI's move does not match the correct puzzle move - flag as incorrect.
                            Console.ForegroundColor = ConsoleColor.Red
                            Console.WriteLine("No")
                            Console.ResetColor()
                        End If
                    End If
                Else
                    'Edits the GUI for the completed search.
                    AmmendLegalMoveSuares(AIBestMove)
                    CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, AIBestMove, MasterEnPassant)
                    CurrentEvaluation = AIBestMove.Score
                    CurrentDepth = CurrentAIDepth
                    AIFinishedSearch = True
                    'Search resulted in a checkmate, or the time to search has expired - exit search process.
                    If Math.Abs(AIBestMove.Score) >= 295 OrElse MainAI.GetABORTState() Then Exit While
                End If
                Console.WriteLine()

                CurrentAIDepth += 1
                'If CurrentSearchStopwatch.ElapsedMilliseconds < 10 Then CurrentAIDepth += 1 'Skips a depth if the previous search was (essentially) instant.
            End If
        End While
    End Sub
    Private Sub FixedAISearchHandler()
        'Begins the search at the specific depth specified by either the user, or by CalculateStartingDepth()
        AIBestMove = MainAI.Search(StartingDepth)
        If AIBestMove.EndState <> "a" Then
            'Outputs the information of the AI's search time.
            Console.ForegroundColor = ConsoleColor.DarkGreen
            Console.Write("Depth Of " & StartingDepth)
            If SearchSettings.UseQuiescence Then Console.Write("-" & MainAI.GetHighestQuiescenceDepth())
            Console.Write(" Completed in: " & AIStopwatch.ElapsedMilliseconds.ToString("N0") & " Milliseconds." & vbCrLf)
            Console.ResetColor()
            'Updates GUI elements.
            CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, AIBestMove, MasterEnPassant)
            CurrentEvaluation = AIBestMove.Score
            CurrentDepth = StartingDepth
            AIFinishedSearch = True
        End If
    End Sub



    'Subroutine that makes a move on the board, given coordinates. Includes castling (& rights) and pawn promotion.
    'Note: This algorithm is very similar to the MakeMove subroutine in the CoreMethods Class, however here material
    'count is not taken into account, and there is an option for sounds to be played when a move is made. This is removed
    'in the CoreMethods version of this algorithm for efficiency purposes for MiniMax (less IF statements).
    Private Sub MakeMove(ByVal Board(,) As Char, ByVal OldCoorX As String, ByVal OldCoorY As String, ByVal NewCoorX As String, ByVal NewCoorY As String, ByRef CanCastle As CanCastle, ByRef KPos As String, ByRef EnPassant As String, ByVal MakeSounds As Boolean)
        Dim TempPiece As Char = Board(OldCoorX, OldCoorY)
        Dim HasEnPassanted As Boolean
        If MakeSounds Then Sound_Move.Play()
        If Char.IsUpper(TempPiece) Then
            If TempPiece = "P" Then
                'Code for Promoting Pawns and En Passant. Also increments the material count.
                If NewCoorY = 0 Then
                    TempPiece = "Q"
                ElseIf NewCoorX & NewCoorY = EnPassant Then
                    Board(NewCoorX, 3) = " "
                    If MakeSounds Then Sound_Capture.Play()
                ElseIf OldCoorY = 6 AndAlso NewCoorY = 4 AndAlso (Board(Math.Max(NewCoorX - 1, 0), 4) = "p" OrElse Board(Math.Min(NewCoorX + 1, 7), 4) = "p") Then
                    'EnPassant Creation.
                    EnPassant = NewCoorX & 5
                    HasEnPassanted = True
                End If
                'If piece is a Rook, part of Castling is disabled (depending on which Rook has moved).
            ElseIf TempPiece = "R" AndAlso CanCastle.CanICastle Then
                If CanCastle.KS AndAlso OldCoorX = 7 AndAlso OldCoorY = 7 Then
                    CanCastle.KS = False
                ElseIf CanCastle.QS AndAlso OldCoorX = 0 AndAlso OldCoorY = 7 Then
                    CanCastle.QS = False
                End If
            ElseIf TempPiece = "K" Then
                KPos = NewCoorX & NewCoorY
                'Code for Castling.
                If CanCastle.KS AndAlso NewCoorX = "6" AndAlso NewCoorY = "7" Then
                    Board(5, 7) = "R"
                    Board(7, 7) = " "
                    If MakeSounds Then Sound_Castle.Play()
                ElseIf CanCastle.QS AndAlso NewCoorX = "2" AndAlso NewCoorY = "7" Then
                    Board(0, 7) = " "
                    Board(3, 7) = "R"
                    If MakeSounds Then Sound_Castle.Play()
                End If
                CanCastle.CannotCastle()
            End If
        Else
            'Identical Code for the Black Pieces.
            If TempPiece = "p" Then
                If NewCoorY = 7 Then
                    TempPiece = "q"
                ElseIf NewCoorX & NewCoorY = EnPassant Then
                    Board(NewCoorX, 4) = " "
                    If MakeSounds Then Sound_Capture.Play()
                ElseIf OldCoorY = 1 AndAlso NewCoorY = 3 AndAlso (Board(Math.Max(NewCoorX - 1, 0), 3) = "P" OrElse Board(Math.Min(NewCoorX + 1, 7), 3) = "P") Then
                    'EnPassant Creation.
                    EnPassant = NewCoorX & 2
                    HasEnPassanted = True
                End If
            ElseIf TempPiece = "r" AndAlso CanCastle.CanICastle Then
                If CanCastle.KS AndAlso OldCoorX = 7 AndAlso OldCoorY = 0 Then
                    CanCastle.KS = False
                ElseIf CanCastle.QS AndAlso OldCoorX = 0 AndAlso OldCoorY = 0 Then
                    CanCastle.QS = False
                End If
            ElseIf TempPiece = "k" Then
                KPos = NewCoorX & NewCoorY
                If CanCastle.KS AndAlso NewCoorX = "6" AndAlso NewCoorY = "0" Then
                    Board(5, 0) = "r"
                    Board(7, 0) = " "
                    If MakeSounds Then Sound_Castle.Play()
                ElseIf CanCastle.QS AndAlso NewCoorX = "2" AndAlso NewCoorY = "0" Then
                    Board(0, 0) = " "
                    Board(3, 0) = "r"
                    If MakeSounds Then Sound_Castle.Play()
                End If
                CanCastle.CannotCastle()
            End If
        End If

        If Not (EnPassant = "-" OrElse HasEnPassanted) Then EnPassant = "-" 'Removal of EnPassant (if required).
        'At the end of the subroutine, the Piece is placed at the new coordinates, and the old position is cleared.
        'If the new position contains a piece, then the material count is updated for only that piece.
        If Board(NewCoorX, NewCoorY) <> " " AndAlso MakeSounds Then Sound_Capture.Play()
        Board(NewCoorX, NewCoorY) = TempPiece
        Board(OldCoorX, OldCoorY) = " "
    End Sub


    'Subroutine that adjusts the values on the LegalMoveSquares array when a given move is being made
    '(for illustration purposes when the AI is deciding between moves).
    Private Sub AmmendLegalMoveSuares(ByVal TempMove As Move)
        ResetLMS(False)
        'Sets specific squares on the array (start & end square of the AI's move).
        If OrientForWhite Then
            LegalMoveSquares(TempMove.OldMoveX, TempMove.OldMoveY) = True
            LegalMoveSquares(TempMove.NewMoveX, TempMove.NewMoveY) = True
        Else
            LegalMoveSquares(7 - TempMove.OldMoveX, 7 - TempMove.OldMoveY) = True
            LegalMoveSquares(7 - TempMove.NewMoveX, 7 - TempMove.NewMoveY) = True
        End If
    End Sub


    'Checks for Checkmate & Stalemate are calculated by running the opponent AI on a special mode,
    'where the AI runs to a depth of 1 but makes no moves on the board. This determines if the opponent
    'actually has any legal moves (or if there is only 1 forced move), and if they don't, then then the
    'game will end. This algorithm also checks for draws by insufficient material. If the procedure does
    'detect any of these end states, then it stops the game and/or notifies the user (depending on gamemode).
    Private Sub EnforceEndStates(ByVal NormalMove As Boolean)
        Dim TempMove = MainAI.CheckForEndState() 'Checks for endstates in the current position.
        Select Case TempMove.EndState
            Case "c" 'Position is checkmate.
                CheckLabel.Text = "Checkmate!"
                If GeneralOptions(0) = "T" Then Sound_Checkmate.Play()
                GameRunning = False
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("The Game has Ended. Cause = Checkmate.")
                If GameMode < 3 Then NotifyGameEnd("c")
                Exit Sub
            Case "s" 'Position is stalemate.
                CheckLabel.Text = " Stalemate! "
                If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
                GameRunning = False
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("The Game has Ended. Cause = Stalemate.")
                If GameMode < 3 Then NotifyGameEnd("d")
                Exit Sub
        End Select

        'Draw by insufficient material is then checked for. In principle, if it is physically impossible for one
        'player to checkmate the other (such as king vs king, or king vs king + a knight / bishop), then the
        'position is delared 'dead' and the game ends in a draw.
        Dim TempMaterialCount As Int16() = SharedAlgorithms.CountMaterial(MasterBoard)
        If TempMaterialCount(0) + TempMaterialCount(1) = 0 Then 'only kings remain.
            CheckLabel.Text = "     Draw!     "
            If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
            GameRunning = False
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("The Game has Ended. Cause = Draw by Insufficient Material (K v K).")
            If GameMode < 3 Then NotifyGameEnd("d")
        ElseIf TempMaterialCount(0) + TempMaterialCount(1) = SharedAlgorithms.ReturnPieceValue("B") Then 'could be king vs king + knight / bishop.
            For y = 0 To 7
                For x = 0 To 7
                    'scans for knights / bishops.
                    If UCase(MasterBoard(x, y)) = "B" OrElse UCase(MasterBoard(x, y)) = "N" Then
                        'Game ends in a draw.
                        CheckLabel.Text = "     Draw!     "
                        If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
                        GameRunning = False
                        Console.ForegroundColor = ConsoleColor.Red
                        Console.WriteLine("The Game has Ended. Cause = Draw by Insufficient Material (K v K+B/N).")
                        If GameMode < 3 Then NotifyGameEnd("d")
                        Exit Sub
                    End If
                Next
            Next
        ElseIf TempMaterialCount(0) = SharedAlgorithms.ReturnPieceValue("B") AndAlso TempMaterialCount(1) = SharedAlgorithms.ReturnPieceValue("B") Then 'only possibility is king + bishop vs king + bishop (of same type).
            Dim NoOfBishopsFound As Byte
            Dim BishopType As Boolean 'True = Light, False = Dark
            For y = 0 To 7
                For x = 0 To 7
                    If UCase(MasterBoard(x, y)) = "B" Then
                        Select Case NoOfBishopsFound
                            Case 0
                                'Calculates if the bishop is on a light square or a dark square.
                                If (x Mod 2 = 0 AndAlso y Mod 2 = 0) OrElse (x Mod 2 = 1 AndAlso y Mod 2 = 1) Then
                                    BishopType = True
                                Else
                                    BishopType = False
                                End If
                            Case 1
                                'If this bishop is on the same colour complex (light / dark) as the previous bishop,
                                'then we end the game.
                                If (x Mod 2 = 0 AndAlso y Mod 2 = 0 AndAlso BishopType) OrElse (x Mod 2 = 1 AndAlso y Mod 2 = 1 AndAlso BishopType) OrElse (x Mod 2 = 1 AndAlso y Mod 2 = 0 AndAlso Not BishopType) OrElse (x Mod 2 = 0 AndAlso y Mod 2 = 1 AndAlso Not BishopType) Then
                                    CheckLabel.Text = "     Draw!     "
                                    If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
                                    GameRunning = False
                                    Console.ForegroundColor = ConsoleColor.Red
                                    Console.WriteLine("The Game has now Ended. Cause = Draw by Insufficient Material (K v K+B+B).")
                                    If GameMode < 3 Then NotifyGameEnd("d")
                                End If
                        End Select
                        NoOfBishopsFound += 1
                    ElseIf Not (UCase(MasterBoard(x, y)) = "K" OrElse UCase(MasterBoard(x, y)) = " ") Then
                        'Another piece found - exit search.
                        Exit Sub
                    End If
                Next
            Next
        End If

        'Adds the new board position's hash value to the Game History.
        MasterZobristValue = SharedAlgorithms.ZobristHashPosition(MasterBoard, PlayerTurn, MasterWCanCastle, MasterBCanCastle, MasterEnPassant)
        BoardHistory.Push(MasterZobristValue, NormalMove) 'NormalMove specifies if the move is a move that the user or the AI has made,
        'or whether it is a move formed by, for example, undoing the board position. Depending on which it is, GameHistory will store
        'the previous state to its buffer (allowing the state to be undone, if needed).

        'Checks for three-fold repetition.
        If BoardHistory.CheckNoOfOccurances(MasterZobristValue) = 3 Then
            'Position has ocurred three times - end game.
            CheckLabel.Text = "     Draw!     "
            If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
            GameRunning = False
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("The Game has Ended. Cause = Draw by Three-fold Repetition.")
            If GameMode < 3 Then NotifyGameEnd("d")
            Exit Sub
        End If
    End Sub

    'Notifies the user if the game has concluded - giving them an appropriate message and an option to play again.
    'If they deny, then they are sent back to the main menu.
    Private Sub NotifyGameEnd(ByVal EndState As Char)
        Console.ResetColor()
        Dim Text As String
        If GameMode = 1 Then
            If EndState = "c" Then
                'Checks if the player or the AI is the winner of the game.
                If (MasterWInCheck.IsInCheck AndAlso Not UserPlayer) OrElse (MasterBInCheck.IsInCheck AndAlso UserPlayer) Then
                    Text = "Congratulations - You have Beaten the AI!"
                Else
                    Text = "Unfortunately, the AI has Beaten you."
                End If
            Else
                Text = "The Game has Concluded in a Draw!"
            End If
        Else 'Gives unbias comment.
            Text = "The Game has Concluded"
            If EndState = "c" Then
                If MasterBInCheck.IsInCheck Then
                    Text &= ", with White Victorious!"
                Else
                    Text &= ", with Black Victorious!"
                End If
            Else
                Text &= " in a Draw!"
            End If
        End If

        Application.DoEvents() 'Finishes making the move.
        Thread.Sleep(1000)
        'Displays popup message.
        If MsgBox(Text & vbCr & "Would you Like to Play Again?", vbInformation + vbYesNo + vbApplicationModal) = 6 Then
            'User wants to play again - reset the board.
            Reset_Btn_Click()
            PreviousFEN = CurrentFEN
        Else 'Quit.
            ExitBtn_Click()
        End If
    End Sub

End Class
