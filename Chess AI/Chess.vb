'Working Version of Chess, along with AI using MiniMax and Alpha Beta Pruning. Created by Alfie Kunz - 8158.
Imports System.Threading
Imports System.Text.RegularExpressions
Imports System.Security.Cryptography
Imports System.Diagnostics.Eventing.Reader
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.IO
Imports System.Text
Imports System.Runtime
Imports System.Reflection
Imports System.Runtime.Remoting.Messaging
Imports Microsoft.VisualBasic.Devices
Imports System.Windows.Input
Imports System.Diagnostics.Eventing
Imports Microsoft.SqlServer

'This class forms the CORE User Interface and the basic game of chess, along with its rules. It will control moving pieces,
'importing / exporting positions, manipulating the board + its visual design ext, along with converting a user’s actions onto
'a computer-friendly interface (for the AI to use). This also instantiates the AI class and interacts with it.
Partial Public Class Chess 'ew- danny

    Private SharedAlgorithms As New CoreMethods 'Calls all objects from the CoreMethods class.
    Private BoardHistory As New GameHistory
    Private GeneralOptions As String = GlobalConstants.DefaultGeneralOptions


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
    Private MasterWInCheck, MasterBInCheck As Byte


    'Sets up sound effects. All sounds used by Chess.com
    Private ReadOnly Sound_Move As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Assets\Sounds\Chess_Move.wav"
    }
    Private ReadOnly Sound_Capture As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Assets\Sounds\Chess_Capture.wav"
    }
    Private ReadOnly Sound_Check As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Assets\Sounds\Chess_Check.wav"
    }
    Private ReadOnly Sound_Castle As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Assets\Sounds\Chess_Castle.wav"
    }
    Private ReadOnly Sound_Checkmate As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Assets\Sounds\Chess_Checkmate.wav"
    }
    Private ReadOnly Sound_Stalemate As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Assets\Sounds\Chess_Stalemate.wav"
    }
    Private ReadOnly Sound_GameOver As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Assets\Sounds\Chess_GameOver.wav"
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
        Me.New(Mode, UserStartingFEN, 0, New AISearchSettings(), False, True, Nothing)
    End Sub

    Public Sub New(ByRef InputBook As List(Of OpeningBookEntry)) 'Call used for a standard game of chess (usually in Analysis Mode).
        GameRunning = False
        InitializeComponent() ' This call is required by the designer.
        'Loads the opening book into the system.
        If InputBook IsNot Nothing AndAlso InputBook.Count > 0 Then OpeningBook.AddRange(InputBook) : UseBook.Enabled = True
    End Sub

    'The below constructor method rearranges the location of objects on the form (to save space on the screen).
    'This will be called for 1-Player Games and 2-Player Games.
    Public Sub New(ByVal Mode As Byte, ByVal UserStartingFEN As String, ByVal UserTimeForSearch As Decimal, ByVal UserSearchSettings As AISearchSettings, ByVal UserAICanSearchOnUsersTurn As Boolean, ByVal PlayAsWhite As Boolean, ByRef InputBook As List(Of OpeningBookEntry))
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
        PGNExport.Visible = False
        UndoMove.Top -= 70
        BoardEditorBtn.Visible = False
        CheckLabel.Location = New Point(89, 138)
        UserTimeBar.Visible = False
        UserTimeBox.Visible = False
        QuiescenceBox.Visible = False
        PieceHeatMapBox.Visible = False
        UseBook.Visible = False
        AIEndlessMode.Visible = False
        NodeTestBtn.Visible = False
        SettingsBtn.Location = New Point(40, 520)
        ExitBtn.Location = New Point(181, 520)
        Credits.Location = New Point(78.5, 575)


        If Mode = 1 Then 'Mode-Specific Movement - 1P Mode.
            'Hides / sets appropriate objects.
            AutoOutputterPanel.Visible = True
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
            SearchSettings.CopyFrom(UserSearchSettings)
            AICanSearchOnUsersTurn = UserAICanSearchOnUsersTurn

        ElseIf Mode = 2 Then '2P Mode
            AutoOutputterPanel.Visible = True
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
            'Selects, then loads a random puzzle database from the Database folder.
            Static RND As New Random()
            Dim PuzzleFileNames As String() = Directory.GetFiles(Application.StartupPath & "\Assets\Puzzle Database\", "*.txt")
            Dim SampleIndex As Byte = RND.Next(1, PuzzleFileNames.Length)

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
                Console.WriteLine("Puzzle Database " & SampleIndex & " Successfully Retrieved in:  " & Math.Round(Timer.Elapsed.TotalMilliseconds, 1) & "ms.")
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
            CheckLabel.Top += 36
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
        DoubleBuffered = True 'For smoother piece moving animations.
        Console.OutputEncoding = System.Text.Encoding.UTF8
        Randomize()

        'Assigns each piece to PieceArray and sets the 'board' picturebox as their parent.
        PieceArray.Add(WK1)
        PieceArray.Add(WQ1)
        PieceArray.Add(WB1)
        PieceArray.Add(WN1)
        PieceArray.Add(WR1)
        PieceArray.Add(WP1)

        PieceArray.Add(BK1)
        PieceArray.Add(BQ1)
        PieceArray.Add(BB1)
        PieceArray.Add(BN1)
        PieceArray.Add(BR1)
        PieceArray.Add(BP1)

        For Each Piece In PieceArray
            Checkerboard.Controls.Add(Piece)
            'Adds handles for each of the PictureBoxes, to handle user movement.
            AddHandler Piece.MouseDown, AddressOf Piece_MouseDown
            AddHandler Piece.MouseMove, AddressOf Piece_MouseMove
            AddHandler Piece.MouseUp, AddressOf Piece_MouseUp
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
                    SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, SharedAlgorithms.ConvertStringToBitCoor(MasterWKPos), MasterWInCheck, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant))
                Else
                    SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, SharedAlgorithms.ConvertStringToBitCoor(MasterBKPos), MasterBInCheck, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant))
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
            SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, SharedAlgorithms.ConvertStringToBitCoor(MasterWKPos), MasterWInCheck, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant))
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
            MainAI.ConfigureSettings(SearchSettings, False)
            MasterZobristValue = SharedAlgorithms.ZobristHashPosition(MasterBoard, PlayerTurn, MasterWCanCastle, MasterBCanCastle, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant))

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
        EnforceEndStates(False) 'Checks for end positions in the starting FEN.
        InitialMemoryUsage = (Process.GetCurrentProcess()).WorkingSet64 'Calibrates the initial memory usage of the program.
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
            PlayNextPuzzleMove(0, True)
            RunAIOnPuzzle()
        ElseIf GameMode >= 5 Then
            Me.WLeaderBoardGrid.ClearSelection() 'Stops the Leaderboard from being highlighted.
        Else
            PreviousFEN = StartingFEN 'if CurrentFEN = PreviousFEN then the user cannot undo a move.
        End If
    End Sub



    'Subroutine that assigns all the PictureBoxes with their associated images.
    Private Sub AssignImagesToPieces()
        'All images are taken from the 'cburnett' piece theme of Lichess.org - an open source, charity chess website.
        'Please see here for information about using these assets: https://lichess.org/contact#help-authorize.
        Dim Colour As String = "W"
        For x = 0 To 11 Step 6
            PieceArray(x).Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\" & Colour & "King.png")
            PieceArray(x + 1).Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\" & Colour & "Queen.png")
            PieceArray(x + 2).Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\" & Colour & "Bishop.png")
            PieceArray(x + 3).Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\" & Colour & "Knight.png")
            PieceArray(x + 4).Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\" & Colour & "Rook.png")
            PieceArray(x + 5).Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\" & Colour & "Pawn.png")
            Colour = "B"
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
            AnimationSpeed = GlobalConstants.DefaultAnimationSpeed
            TempGeneralOptions = GlobalConstants.DefaultGeneralOptions
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
        If GeneralOptions(7) <> TempGeneralOptions(7) Then
            If TempGeneralOptions(7) = "T" Then
                SearchSettings.ReturnBestMove = False
                If CheckLabel.Text = "Checkmate!" Then CheckLabel.Text = " king dead. "
            Else
                SearchSettings.ReturnBestMove = True
                If CheckLabel.Text = " king dead. " Then CheckLabel.Text = "Checkmate!"
            End If
        End If

        'Calibrates General Options
        If GeneralOptions(6) <> TempGeneralOptions(6) Then
            'Invisible pieces setting modified...
            GeneralOptions = TempGeneralOptions
            If TempGeneralOptions(6) = "T" Then
                'Removes images from pieces.
                For Each Box In PieceArray
                    Box.Image = Nothing
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



    'Outputs the board position, along with the TFTables & King positions (to the console).
    'Used for debugging purposes.
    Private Sub OutputDebugInfo()
        Console.ForegroundColor = ConsoleColor.DarkYellow
        'Outputs the current FEN.
        Console.WriteLine(vbCrLf & vbCrLf & "FEN: " & CurrentFEN)
        Console.ResetColor()
        Console.Write(" Board:" & New String(" ", 6))
        If PlayerTurn Then Console.WriteLine("WhiteTFTable:") Else Console.WriteLine("BlackTFTable:")

        'If the board is flipped in black's favour, rotate the output of the board (and the TFTable) 180 degrees.
        Dim StartValue, EndValue, StepValue As SByte
        If OrientForWhite Then
            StartValue = 0
            EndValue = 7
            StepValue = 1
        Else
            StartValue = 7
            EndValue = 0
            StepValue = -1
        End If

        For y = StartValue To EndValue Step StepValue
            For x = StartValue To EndValue Step StepValue
                'Outputs the board position.
                If MasterBoard(x, y) = " " Then
                    If (x + y) Mod 2 = 0 Then
                        'Square is a light-coloured square.
                        Console.ForegroundColor = ConsoleColor.White
                    Else 'Square is a dark-coloured square.
                        Console.ForegroundColor = ConsoleColor.Gray
                    End If
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

            'Outputs the TFTable of the player to move.
            Console.Write(New String(" ", 7))
            For x = StartValue To EndValue Step StepValue
                'Colours the indexes depending if it is True, False, or the position of the player's king.
                If PlayerTurn Then
                    If x & y = MasterWKPos Then
                        Console.ForegroundColor = ConsoleColor.DarkYellow
                    ElseIf MasterWInCheck >= 128 AndAlso ((x << 3) Or y) = (MasterWInCheck And 63) Then
                        Console.ForegroundColor = ConsoleColor.White
                    ElseIf MasterWhiteTFTable(x, y) = "T" Then
                        Console.ForegroundColor = ConsoleColor.Green
                    ElseIf MasterWhiteTFTable(x, y) = "F" Then
                        Console.ForegroundColor = ConsoleColor.Red
                    Else
                        Console.ForegroundColor = ConsoleColor.Blue
                    End If
                    Console.Write(MasterWhiteTFTable(x, y))
                Else
                    If x & y = MasterBKPos Then
                        Console.ForegroundColor = ConsoleColor.DarkYellow
                    ElseIf MasterBInCheck >= 128 AndAlso ((x << 3) Or y) = (MasterBInCheck And 63) Then
                        Console.ForegroundColor = ConsoleColor.White
                    ElseIf MasterBlackTFTable(x, y) = "T" Then
                        Console.ForegroundColor = ConsoleColor.Green
                    ElseIf MasterBlackTFTable(x, y) = "F" Then
                        Console.ForegroundColor = ConsoleColor.Red
                    Else
                        Console.ForegroundColor = ConsoleColor.Blue
                    End If
                    Console.Write(MasterBlackTFTable(x, y))
                End If
            Next
            Console.ResetColor()

            'Outputs the information of the player.
            If Not OrientForWhite Then y = 7 - y
            If y Mod 2 = 0 Then
                Console.Write(New String(" ", 7))
                If y = 0 Then
                    Console.Write("Player: ")
                    If PlayerTurn Then
                        Console.ForegroundColor = ConsoleColor.White
                        Console.WriteLine("White")
                    Else
                        Console.ForegroundColor = ConsoleColor.DarkGray
                        Console.WriteLine("Black")
                    End If
                End If
                If y = 2 Then Console.WriteLine("WKPos: " & SharedAlgorithms.CoorToPGNConverter(MasterWKPos))
                If y = 4 Then Console.WriteLine("BKPos: " & SharedAlgorithms.CoorToPGNConverter(MasterBKPos))
                If y = 6 Then Console.WriteLine("En Passant: " & SharedAlgorithms.CoorToPGNConverter(MasterEnPassant))
            Else
                Console.WriteLine()
            End If
            If Not OrientForWhite Then y = 7 - y
        Next

        'Outputs the Hash Value.
        Console.ForegroundColor = ConsoleColor.Magenta
        Console.WriteLine("Zobrist Hash Value: " & MasterZobristValue.ToString("X16")) 'Outputs Zobrist Key in Hexadecimal.
        Console.ResetColor()
    End Sub



    'Function that detects some errors in a user-entered FEN string, given the below rules:
    Private Function FENErrorDetection(ByVal FEN As String, ByVal OutputToBox As Boolean, ByRef FENErrorMessage As String) As Boolean
        'If OutputToBox = True then the error message is stored in InputTextBox. If it is false then the message is stored in FENErrorMessage.
        Dim MaxWQueens, MaxBQueens, WKingCount, BKingCount As Byte
        If FEN.Length > 22 AndAlso FEN.Length < 88 Then 'Length validation
            'Checks if each player has a king.
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
                    FENErrorMessage = "Position Rejected - Invalid number of Kings."
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
        If MasterWInCheck >= 128 AndAlso Not PlayerTurn Then
            'If White is in check, and it is black to move, then black can take white's king. This is illegal.
            GameRunning = False
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("The Game has Ended. Cause = Invalid Check.")
            CheckForInvalidGameStates = True
        ElseIf MasterBInCheck >= 128 AndAlso PlayerTurn Then
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
    Private Sub EnterMovesIntoSystem(ByVal Moves As String, ByVal NeedsFormatting As Boolean, OutputToTextBox As Boolean)
        'Formats PGN Moves. Some PGN Moves also contain move numbers - my program needs to remove these to interpret the moves.
        Dim FormattedPGN As String
        If NeedsFormatting Then
            Try
                If Moves.IndexOf(" ") = -1 Then Moves = Moves.Replace(",", " ")
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
        End If


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
                            TempMove = SharedAlgorithms.ConvertToMove(TempPGNMove, MasterBoard, True, SharedAlgorithms.ConvertStringToBitCoor(MasterWKPos), MasterWhiteTFTable)
                            If TempMove.EndState = "a" Then
                                Throw New Exception("Invalid Move.")
                            ElseIf TempMove.EndState = "c" Then
                                Throw New Exception("Collision.")
                            End If
                            'Checks for all the legal moves that this piece can make, then compare these to the user's input move.
                            PieceMoving.LegalMoves = MainAI.ReturnPiecesLegalMoves(TempMove.OldMoveX, TempMove.OldMoveY)
                            IsLegalMove = False
                            If PieceMoving.LegalMoves IsNot Nothing Then
                                For m = 0 To PieceMoving.LegalMoves.Length - 1
                                    If TempMove.NewMoveX & TempMove.NewMoveY = PieceMoving.LegalMoves(m) Then
                                        'Move found - break out of loop.
                                        IsLegalMove = True
                                        Exit For
                                    End If
                                Next
                            End If
                            If Not IsLegalMove Then Throw New Exception("Illegal Move.")
                            'Performs the move.
                            AnimateMove(TempMove)
                            MakeMove(MasterBoard, TempMove, MasterWCanCastle, MasterWKPos, MasterEnPassant, GeneralOptions(0) = "T")
                        Else 'Similar code for the black pieces.
                            TempMove = SharedAlgorithms.ConvertToMove(TempPGNMove, MasterBoard, False, SharedAlgorithms.ConvertStringToBitCoor(MasterBKPos), MasterBlackTFTable)
                            If TempMove.EndState = "a" Then
                                Throw New Exception("Invalid Move.")
                            ElseIf TempMove.EndState = "c" Then
                                Throw New Exception("Collision.")
                            End If
                            PieceMoving.LegalMoves = MainAI.ReturnPiecesLegalMoves(TempMove.OldMoveX, TempMove.OldMoveY)
                            IsLegalMove = False
                            If PieceMoving.LegalMoves IsNot Nothing Then
                                For m = 0 To PieceMoving.LegalMoves.Length - 1
                                    If TempMove.NewMoveX & TempMove.NewMoveY = PieceMoving.LegalMoves(m) Then
                                        IsLegalMove = True
                                        Exit For
                                    End If
                                Next
                            End If
                            If Not IsLegalMove Then Throw New Exception("Illegal Move.")
                            AnimateMove(TempMove)
                            MakeMove(MasterBoard, TempMove, MasterBCanCastle, MasterBKPos, MasterEnPassant, GeneralOptions(0) = "T")
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
                            If TempMove.EndState = "c" Then
                                InputTextBox.Text = "Move (" & TempPGNMove & ") has Multiple Interpretations. Consider adding constraint(s) (eg: Nbd7)..."
                            Else
                                InputTextBox.Text = "Move (" & TempPGNMove & ") is " & OutputResponce & ". Please Input a valid set of Moves and try again."
                            End If
                        Else
                            Console.ForegroundColor = ConsoleColor.DarkRed
                            If TempMove.EndState = "c" Then
                                Console.WriteLine("Move (" & TempPGNMove & ") has Multiple Interpretations. Consider adding constraint(s) (eg: Nbd7)...")
                            Else
                                Console.WriteLine("Move (" & TempPGNMove & ") is " & OutputResponce & ".")
                            End If
                            Console.ResetColor()
                        End If
                        Exit Sub
                    End Try

                    If Not UndoFENChange.Visible Then 'Move is valid.
                        'Updates TFTable for appropriate player.
                        If PlayerTurn Then
                            MasterWInCheck = 0 'Player is no longer in check.
                            SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, SharedAlgorithms.ConvertStringToBitCoor(MasterBKPos), MasterBInCheck, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant))
                        Else
                            MasterBInCheck = 0 'Player is no longer in check.
                            SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, SharedAlgorithms.ConvertStringToBitCoor(MasterWKPos), MasterWInCheck, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant))
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
                        If FirstMove Then PreviousFEN = CurrentFEN
                        CurrentFEN = SharedAlgorithms.ConvertToFEN(MasterBoard, MasterWCanCastle, MasterBCanCastle, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant), PlayerTurn)
                        MainAI.Reconfigure(CurrentFEN, False) 'Recalibrates AI.
                        EnforceEndStates(FirstMove)
                        'If the player has been put in check, and has not been checkmated, then add the + symbol to the end of the move.
                        If (MasterWInCheck >= 128 OrElse MasterBInCheck >= 128) AndAlso GameRunning Then TempPGNMove &= "+"
                        BoardHistory.PushPGN(TempPGNMove)

                        FirstMove = False
                        Exit For
                    End If
                End If

            Next

        Loop While Len(FormattedPGN) > 0 AndAlso GameRunning
        'Once all the moves have been made, output the new board position.
        OutputDebugInfo()
    End Sub





    'Subroutine that makes a move on the board, given coordinates. Includes castling (& rights) and pawn promotion.
    'Note: This algorithm is very similar to the MakeMove subroutine in the CoreMethods Class, however here material
    'count is not taken into account, and there is an option for sounds to be played when a move is made. This is removed
    'in the CoreMethods version of this algorithm for efficiency purposes for MiniMax (less IF statements).
    Private Sub MakeMove(ByVal Board(,) As Char, ByVal TempMove As Move, ByRef CanCastle As CanCastle, ByRef KPos As String, ByRef EnPassant As String, ByVal MakeSounds As Boolean)
        Dim OldCoorX As String = TempMove.OldMoveX
        Dim OldCoorY As String = TempMove.OldMoveY
        Dim NewCoorX As String = TempMove.NewMoveX
        Dim NewCoorY As String = TempMove.NewMoveY

        Dim TempPiece As Char = Board(OldCoorX, OldCoorY)
        Dim HasEnPassanted As Boolean
        If MakeSounds Then Sound_Move.Play()
        If Char.IsUpper(TempPiece) Then
            If TempPiece = "P" Then
                'Code for Promoting Pawns and En Passant. Also increments the material count.
                If NewCoorY = 0 Then
                    TempPiece = If(TempMove.EndState = "", "Q", TempMove.EndState)
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
                    TempPiece = If(TempMove.EndState = "", "q", LCase(TempMove.EndState))
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




    'Checks for Checkmate & Stalemate are calculated by running the opponent AI on a special mode,
    'where the AI runs to a depth of 1 but makes no moves on the board. This determines if the opponent
    'actually has any legal moves (or if there is only 1 forced move), and if they don't, then then the
    'game will end. This algorithm also checks for draws by insufficient material. If the procedure does
    'detect any of these end states, then it stops the game and/or notifies the user (depending on gamemode).
    Private Function EnforceEndStates(ByVal NormalMove As Boolean) As Char
        'Adds the new board position's hash value to the Game History.
        MasterZobristValue = SharedAlgorithms.ZobristHashPosition(MasterBoard, PlayerTurn, MasterWCanCastle, MasterBCanCastle, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant))
        BoardHistory.PushZobrist(MasterZobristValue, NormalMove) 'NormalMove specifies if the move is a move that the user or the AI has made,
        'or whether it is a move formed by, for example, undoing the board position. Depending on which it is, GameHistory will store
        'the previous state to its buffer (allowing the state to be undone, if needed).

        Dim TempMove As New Move
        TempMove = MainAI.CheckForEndState() 'Checks for endstates in the current position.
        Select Case TempMove.EndState
            Case "c" 'Position is checkmate.
                If GeneralOptions(7) = "T" Then
                    'When I told Hammad that I had created "Hammad Mode" for him, he was annoyed that I didn't
                    'include his famous catchphrase 'king dead' when a checkmate occured. Therefore I decided to
                    'honour his legacy lmao by including it here.
                    CheckLabel.Text = " king dead. "
                Else
                    CheckLabel.Text = "Checkmate!"
                End If
                If GeneralOptions(0) = "T" Then Sound_Checkmate.Play()
                GameRunning = False
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("The Game has Ended. Cause = Checkmate.")
                If GameMode < 3 Then NotifyGameEnd("c")
                Return "c"
            Case "s" 'Position is stalemate.
                CheckLabel.Text = " Stalemate! "
                If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
                GameRunning = False
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("The Game has Ended. Cause = Stalemate.")
                If GameMode < 3 Then NotifyGameEnd("d")
                Return "s"
        End Select

        'Draw by insufficient material is then checked for. In principle, if it is physically impossible for one
        'player to checkmate the other (such as king vs king, or king vs king + a knight / bishop), then the
        'position is delared 'dead' and the game ends in a draw.
        Dim TempMaterialCount As UInt16() = SharedAlgorithms.CountMaterial(MasterBoard)
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
                        Return "d"
                    End If
                Next
            Next
        ElseIf TempMaterialCount(0) = SharedAlgorithms.ReturnPieceValue("B") AndAlso TempMaterialCount(1) = SharedAlgorithms.ReturnPieceValue("B") Then 'only possibility is king + bishop vs king + bishop (of same type).
            Dim NoOfBishopsFound As SByte
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
                                    Return "d"
                                End If
                        End Select
                        NoOfBishopsFound += 1
                    ElseIf Not (UCase(MasterBoard(x, y)) = "K" OrElse UCase(MasterBoard(x, y)) = " ") Then
                        'Another piece found - exit search.
                        NoOfBishopsFound = -1
                        Exit For
                    End If
                Next
                If NoOfBishopsFound = -1 Then Exit For
            Next
        End If

        'Checks for three-fold repetition.
        If BoardHistory.CheckNoOfZobristOccurances(MasterZobristValue) = 3 Then
            'Position has ocurred three times - end game.
            CheckLabel.Text = "     Draw!     "
            If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
            GameRunning = False
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("The Game has Ended. Cause = Draw by Three-fold Repetition.")
            If GameMode < 3 Then NotifyGameEnd("d")
        End If
        Return TempMove.EndState
    End Function

    'Notifies the user if the game has concluded - giving them an appropriate message and an option to play again.
    'If they deny, then they are sent back to the main menu.
    Private Sub NotifyGameEnd(ByVal EndState As Char)
        Console.ResetColor()
        Dim Text As String
        If GameMode = 1 Then
            If EndState = "c" Then
                'Checks if the player or the AI is the winner of the game.
                If (MasterWInCheck >= 128 AndAlso Not UserPlayer) OrElse (MasterBInCheck >= 128 AndAlso UserPlayer) Then
                    If GeneralOptions(7) = "T" Then
                        Text = "wahaaaaaayyyyyy!!!!"
                    Else
                        Text = "Congratulations - You have Beaten the AI!"
                    End If
                Else
                    If GeneralOptions(7) = "T" Then
                        Text = "haha, loser."
                    Else
                        Text = "Unfortunately, the AI has Beaten you."
                    End If
                End If
            Else
                Text = "The Game has Concluded in a Draw!"
            End If
        Else 'Gives unbias comment.
            Text = "The Game has Concluded"
            If EndState = "c" Then
                If MasterBInCheck >= 128 Then
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
        Dim UserOption As Byte = MsgBox(Text & vbCr & "Would you Like to Play Again?", vbInformation + vbYesNoCancel + vbApplicationModal)
        If UserOption = 6 Then
            'User wants to play again - reset the board.
            Reset_Btn_Click()
            PreviousFEN = CurrentFEN
        ElseIf UserOption = 7 Then 'Quit.
            ExitBtn_Click()
        ElseIf GameMode < 3 Then
            UndoMove.Enabled = False
        End If
    End Sub

End Class
