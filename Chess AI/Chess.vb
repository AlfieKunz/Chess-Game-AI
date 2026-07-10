'Working Version of Chess, along with AI using MiniMax and Alpha Beta Pruning. Created by Alfie Kunz - 8158.
Imports System.Threading

'This class forms the main User Interface and the basic game of chess, along with its rules. It will control moving pieces,
'importing / exporting positions, manipulating the board + its visual design ext, along with converting a user’s actions onto
'a computer-friendly interface (for the AI to use). This also instantiates the AI class and interacts with it.
Public Class Chess 'ew- danny
    Private Structure PieceInfo 'Contains information about a piece that the user is moving on the GUI
        Dim IsMovingPiece As Boolean 'is the piece being moved by the user?
        Dim LegalMoves() As String 'Array containing the legal Moves of the piece being dragged.
        Dim StartPoint As Point
        Dim MidPoint As Point
        Dim EndPoint As Point
    End Structure

    Private SharedAlgorithms As New CoreMethods 'Calls all objects from the CoreMethods class.

    'Here are the computer's primary "eyes" - MasterBoard (which contains each piece's location on the board) and,
    'derived from this, Master TrueFalse Tables for each player. These Tables display information such as Legal
    'Moves for the players' kings, along with pinned pieces and their location, and make handling Move Generation,
    'Checks and Evaluations much easier and more efficient.
    Private MasterBoard(7, 7), MasterWhiteTFTable(7, 7), MasterBlackTFTable(7, 7) As Char
    Private GameRunning As Boolean = True
    Private ReadOnly GameMode As Byte = 3 '1 = 1-Player, 2 = 2-Player, 3 = Analysis, 4 = Puzzles, 5 = Coordinate Practice, 6 = Move Practice.
    'Public WithEvents TrainingTimer As New Timer

    'FEN = Standard Chess notation that displays where all the pieces on the board are supposed to go.
    Private StartingFEN As String = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
    Private CurrentFEN, PreviousFEN, InvalidInput As String
    Private PlayerTurn As Boolean = True 'True = Is White's Turn, False = Is Black's Turn
    Private UserPlayer As Boolean = True 'Represents what colour the user is playing with.
    Private MasterWKPos, MasterBKPos As String 'Loaction of Kings.
    Private MasterEnPassant As String 'Location of EnPassant Square

    'Data for castling & checks for each player.
    Private MasterWCanCastle, MasterBCanCastle As New CanCastle
    Private MasterWInCheck, MasterBInCheck As New InCheck

    Private ReadOnly OpeningBook(100000, 1)
    Private ReadOnly PieceArray(47) As PictureBox 'Contains all the PictureBoxes on the board.
    Private PieceMoving As PieceInfo
    Private PrimaryColour As Color 'Colour representing the dark squares on the Chessboard.
    Private SecondaryColour As Color 'Colour representing the light squares on the Chessboard.
    Private SquareHistory(3, 1) As SByte 'Array containing the previously used squares on the board (for prev moves).
    Private LegalMoveSquares(7, 7) As Boolean 'Array containing the coordinates of where a piece can move on the board,
    '... generated when the user clicks on a piece. Used when redrawing the checkerboard pattern.
    Private OrientForWhite As Boolean = True 'Represents which way the board is flipped.
    Private AnimationSpeed As Byte = 3 'Represents the speed of the piece-moving animation: 0 = Off, 1 = Fast, 2 = Medium, 3 = Slow
    Private GeneralOptions As String = "TTTTFF" '6-character string that represents the configuration of the program.
    Private FixedSearchDepth As Byte = 0 'Number representing the fixed depth the AI will search to (0 = off).
    'Index: 0 = Sound, 1 = Opening Animation, 2 = Board Highlights, 3 = Piece Highlights, 4 = Invisible Pieces, 5 = Hammad Mode (bad AI).

    'Data structures that bridge the gap between user and AI.
    Private AbsoluteDepth As SByte 'Estimate of the optimal depth used for searching.
    Private TimeForSearch As Decimal = 10 'Time the AIs are allowed to search for.
    Private CurrentDepth As Byte 'Highest depth successfully searched to.
    Private CurrentMove As String 'Move of the highest depth successfully searched to.
    Private CurrentEvaluation As String 'Eval of the highest depth successfully searched to.
    Private TerminateSearch As Boolean 'Set to True if the user preemptively aborts the AI search.
    Private ComputerIsSearching As Boolean = False
    Private UseQuiescence As Boolean = True 'Determines whether or not the AI will use Quiescence in its searching.
    'Please see my project report for information about Quiescence.

    'Below are the sets of AIs (that will all run simultaneously), along with their corresponding Move Outputs.
    'Also contains data such as which AIs have moved to a higher depth, and which have finished their search
    '(so that GUI controls can be updated).
    Private MainAI, AI2, AI3, AI4, AI5 As New AI()
    Private AIBestMoves(6) As Move
    Private AIMovedToHigherDepth(1), AIFinishedSearch As Boolean
    Private AIStopwatch As New Stopwatch 'For timing AI searches.

    Private TrainingTimerTicks As UInt16
    Private Const MovesPerPosition As Byte = 3
    Private TrainingMovesCompleted As Byte
    Private MovesInPosition(200, 1) As String

    'Leaderboard info for training games: 0 = Index, 1 = Name, 2 = Score, 3 = Date Achieved.
    Dim WLeaderBoard(9, 2), BLeaderBoard(9, 2) As String

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
    Private ReadOnly Sound_321Go As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\3210Effect.wav"
    }




    Public Sub New()
        InitializeComponent() ' This call is required by the designer.
        StartingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
    End Sub

    Public Sub New(ByRef InputBook(,) As String) 'Call used for a standard game of chess (usually in Analysis Mode).
        InitializeComponent() ' This call is required by the designer.
        StartingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
        'Creates opening book.
        Dim BookLength As UInt32 = 2 * (Val(InputBook(0, 0)) + 1)
        Array.Copy(InputBook, OpeningBook, BookLength)
        UseBook.Enabled = True
    End Sub

    'The below constructor method rearranges the location of objects on the form (to save space on the screen).
    'This will be called for 1-Player Games and 2-Player Games.
    Public Sub New(ByVal Mode As Byte, ByVal UserStartingFEN As String, ByVal Difficulty As Byte, ByVal PlayAsWhite As Boolean, ByRef InputBook(,) As String)
        InitializeComponent() ' This call is required by the designer.
        GameMode = Mode
        'Edits the size of the forms, and modifies the obejct now off-screen.
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
        AIEndlessMode.Visible = False
        UseBook.Visible = False
        SettingsBtn.Location = New Point(40, 520)
        ExitBtn.Location = New Point(181, 520)
        Credits.Location = New Point(78.5, 575)
        StartingFEN = UserStartingFEN
        If Mode = 1 Then 'Mode-Specific Movement - 1P Mode.
            'Hides / sets appropriate objects.
            FlipperButton.Visible = False
            AutoFlipper.Visible = False
            ProgressBar.Location = New Point(46, 250)
            CurrentAIDepth.Location = New Point(ProgressBar.Location.X + 10, ProgressBar.Location.Y + 80)
            CurrentAIMove.Location = New Point(CurrentAIDepth.Location.X, CurrentAIDepth.Location.Y + 30)
            CurrentAIEval.Location = New Point(CurrentAIDepth.Location.X, CurrentAIDepth.Location.Y + 60)
            UserPlayer = PlayAsWhite
            'Difficulty generator.
            If InputBook(0, 0) <> "ERROR" Then UseBook.Checked = True
            If Difficulty = 1 Then
                TimeForSearch = 1
                UseQuiescence = False
            ElseIf Difficulty = 2 Then
                TimeForSearch = 3
                UseQuiescence = False
            ElseIf Difficulty = 3 Then
                TimeForSearch = 5
                UseQuiescence = False
            ElseIf Difficulty = 4 Then
                TimeForSearch = 5
                UseQuiescence = True
            ElseIf Difficulty = 5 Then
                TimeForSearch = 10
                UseQuiescence = True
            Else
                TimeForSearch = 30
                UseQuiescence = True
            End If
            'Creates opening book.
            If UseBook.Checked Then
                Dim BookLength As UInt16 = 2 * (Val(InputBook(0, 0)) + 1)
                Array.Copy(InputBook, OpeningBook, BookLength)
            End If

        ElseIf Mode = 2 Then '2P Mode
            FlipperButton.Top -= 100
            AutoFlipper.Top -= 100
            'Hides / sets appropriate objects.
            ProgressBar.Visible = False
            CurrentAIEval.Visible = False
            AutoFlipper.Checked = True
            CurrentAIDepth.Visible = False
            CurrentAIMove.Visible = False
            CurrentAIEval.Visible = False
        ElseIf Mode = 4 Then 'Puzzles mode

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
            GameRunning = False
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

        'Sets up more objects...
        UndoFENChange.Visible = False
        CheckLabel.Text = "                    "
        For n = 0 To 3
            SquareHistory(n, 0) = -1
            SquareHistory(n, 1) = -1
        Next

        LoadUserProfile()

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
                SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
                SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
                If GameMode <> 3 Then CustomisationForm.Close() 'Closes the customisation form.
                CheckChecker(False)
                FENIsValid = True
            Catch ex As Exception 'FEN is not valid
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

        If Not FENIsValid AndAlso Application.OpenForms().OfType(Of Chess).Any Then 'If customisation form hasn't been closed.
            'Sets the board to the standard FEN postion, and forms variables & objects.
            CustomisationForm.Close()
            StartingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
            MasterBoard = SharedAlgorithms.FENConverter(StartingFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
            DisplayPieces()
            SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
            'Resets Black's TFTable.
            Array.Copy(SharedAlgorithms.MasterTrueTable, MasterBlackTFTable, 64)
        End If

        If Me.IsHandleCreated Then 'If this form is open...
            If Not UserPlayer Then FlipBoard() 'Flips board in user's favour.
            If MasterWInCheck.IsInCheck AndAlso Not PlayerTurn Then
                'If White is in check, and it is black to move, then black can take white's king. This is illegal.
                GameRunning = False
                Console.WriteLine("The Game has Ended. Cause = Invalid Check.")
            ElseIf MasterBInCheck.IsInCheck AndAlso PlayerTurn Then
                'If Black is in check, and it is white to move, then white can take black's king. This is illegal.
                Console.WriteLine("The Game has Ended. Cause = Invalid Check.")
                GameRunning = False
            ElseIf Math.Abs(Val(MasterWKPos(0)) - Val(MasterBKPos(0))) <= 1 AndAlso Math.Abs(Val(MasterWKPos(1)) - Val(MasterBKPos(1))) <= 1 Then
                'Kings are too close together.
                GameRunning = False
                Console.WriteLine("The Game has Ended. Cause = Invalid King Positions.")
                CheckLabel.Text = " Stalemate! "
                If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
            Else
                Console.Clear()
                Console.WriteLine("The Game has Begun.")
            End If

            'Creates our AI (for legal move generation).
            CurrentFEN = StartingFEN
            MainAI.Reconfigure(CurrentFEN)

            StartupStopwatch.Stop()
            Console.WriteLine("Startup Time: " & StartupStopwatch.Elapsed.TotalMilliseconds & " Milliseconds." & vbCrLf)
            StartupStopwatch.Reset()

            If GameMode < 3 Then
                If GameMode = 1 AndAlso Not (UserPlayer = PlayerTurn) Then
                    'The AI needs to make the first move.
                    InstantiateAIs()
                    PreviousFEN = CurrentFEN
                Else 'Prevents the user from undoing the starting move.
                    PreviousFEN = StartingFEN
                End If
                InputTextBox.Text = StartingFEN
                InputTextBox.Select(InputTextBox.Text.Length, 0) 'Stops the TextBox from being highlighted.
            ElseIf GameMode >= 5 Then
                Me.WLeaderBoardGrid.ClearSelection()
            Else
                PreviousFEN = StartingFEN 'if CurrentFEN = PreviousFEN then the user cannot undo a move.
            End If
            EnforceEndStates() 'Checks for end positions in the starting FEN.
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
            Dim temp As String = TempGeneralOptions(5) 'Tests that the General Options are the correct length.
            'Invisible pieces are not supported in the training modes - forcibly disable this control.
            If GameMode > 3 Then TempGeneralOptions = TempGeneralOptions.Remove(4, 1).Insert(4, "F")
            FixedSearchDepth = Val(LineInput(1))
        Catch ex As Exception
            'Default settings.
            ColourScheme = "def"
            AnimationSpeed = 3
            TempGeneralOptions = "TTTTFF"
            FixedSearchDepth = 0
            Console.WriteLine("Error in retrieving User Profile - reverting to default settings.")
        End Try
        FileClose(1)

        'Toggles Hammad Mode.
        If GeneralOptions(5) <> TempGeneralOptions(5) Then
            If TempGeneralOptions(5) = "T" Then MainAI.HammadMode = True : Else MainAI.HammadMode = False
        End If

        'Calibrates General Options
        If GeneralOptions(4) <> TempGeneralOptions(4) Then
            'Invisible pieces setting modified...
            GeneralOptions = TempGeneralOptions
            If TempGeneralOptions(4) = "T" Then
                'Removes images from pieces.
                For x = 0 To 47
                    PieceArray(x).Image = Nothing
                Next
            Else
                'Restores images to pieces.
                AssignImagesToPieces()
                CheckChecker(True)
            End If
        ElseIf GeneralOptions(3) <> TempGeneralOptions(3) AndAlso GeneralOptions(4) = "F" Then
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
                If LegalMoveSquares(x, y) AndAlso GeneralOptions(3) = "T" Then
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
                    If LegalMoveSquares(x, y) AndAlso GeneralOptions(3) = "T" Then 'Normal colour is filled at the centre of the legal move square to produce a green / blue highlight.
                        Dim Square2 As New Rectangle((75 * x) + 5, (75 * y) + 5, 65, 65)
                        Using Brush As New SolidBrush(SecondaryColour)
                            g.FillRectangle(Brush, Square2)
                        End Using
                        'If the given square matches a SquareHistory coordinate, then it is coloured in a different way.
                    ElseIf ((x = SquareHistory(0, 0) AndAlso y = SquareHistory(0, 1)) OrElse (x = SquareHistory(1, 0) AndAlso y = SquareHistory(1, 1))) AndAlso GeneralOptions(2) = "T" Then
                        Using Brush As New SolidBrush(Color.YellowGreen) 'SquareHistory secondary colour.
                            g.FillRectangle(Brush, Square)
                        End Using
                    Else 'Is a normal square - colour with user's seconday colour.
                        Using Brush As New SolidBrush(SecondaryColour)
                            g.FillRectangle(Brush, Square)
                        End Using
                    End If
                Else 'Identical code but for the dark squares (uses different colours).
                    If LegalMoveSquares(x, y) AndAlso GeneralOptions(3) = "T" Then
                        Dim Square2 As New Rectangle((75 * x) + 5, (75 * y) + 5, 65, 65)
                        Using Brush As New SolidBrush(PrimaryColour)
                            g.FillRectangle(Brush, Square2)
                        End Using
                    ElseIf ((x = SquareHistory(0, 0) AndAlso y = SquareHistory(0, 1)) OrElse (x = SquareHistory(1, 0) AndAlso y = SquareHistory(1, 1))) AndAlso GeneralOptions(2) = "T" Then
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
        'Gives the starting square for a piece a distinctive colour, when the user clicks on one.
        If PieceMoving.IsMovingPiece AndAlso GeneralOptions(3) = "T" Then 'the piece is being moved by the user.
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
        Console.WriteLine(vbCrLf & "Board Position:")
        For y = 0 To 7
            For x = 0 To 7
                Console.Write(MasterBoard(x, y))
            Next
            Console.WriteLine()
        Next
        Console.WriteLine()
        Console.WriteLine("WhiteTFTable:")
        For y = 0 To 7
            For x = 0 To 7
                Console.Write(MasterWhiteTFTable(x, y))
            Next
            Console.WriteLine()
        Next
        Console.WriteLine("BlackTFTable:")
        For y = 0 To 7
            For x = 0 To 7
                Console.Write(MasterBlackTFTable(x, y))
            Next
            Console.WriteLine()
        Next
        Console.WriteLine("WKPos: " & MasterWKPos & ". BKPos: " & MasterBKPos)
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
        Dim MaxWQueens As Byte = 0
        Dim MaxBQueens As Byte = 0
        Dim WKingCount As Byte = 0
        Dim BKingCount As Byte = 0
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
        Catch ex As Exception
            'Unable to format moves - assume input is invalid.
            If OutputToTextBox Then
                InvalidInput = InputTextBox.Text
                UndoFENChange.Visible = True
                InputTextBox.Text = "Move Unable to be Formatted. Please Input a valid set of Moves and try again."
            Else
                Console.WriteLine("Move Unable to be Formatted.")
            End If
            Exit Sub
        End Try
        FormattedPGN = FormattedPGN.TrimEnd(" ")
        If OutputToTextBox Then InputTextBox.Text = FormattedPGN 'Outputs formatted PGN to InputTextBox.

        FormattedPGN &= " "
        Dim TempPGNMove As String
        Dim TempMove As New Move
        Dim FirstMove As Boolean = True
        Dim IsLegalMove As Boolean
        Do
            If Not FirstMove Then Thread.Sleep(25) 'Allows the user to appreciate the moves being made.
            For n = 0 To FormattedPGN.Length - 1
                If FormattedPGN(n) = " " Then
                    'Isolates the PGN Move.
                    TempPGNMove = FormattedPGN.Substring(0, n)
                    FormattedPGN = FormattedPGN.Substring(n + 1, FormattedPGN.Length - n - 1)
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
                            Console.WriteLine("Move (" & TempPGNMove & ") is " & OutputResponce & ". Please Input a valid set of Moves and try again.")
                        End If
                        Exit Sub
                    End Try

                    If Not UndoFENChange.Visible Then 'Move is valid.
                        'Updates TFTable for appropriate player.
                        If PlayerTurn Then
                            MasterWInCheck.NotInCheck() 'Player is no longer in check.
                            SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
                        Else
                            MasterBInCheck.NotInCheck() 'Player is no longer in check.
                            SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
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
                        MainAI.Reconfigure(CurrentFEN) 'Recalibrates AI.
                        EnforceEndStates()

                        Exit For
                    End If
                End If

            Next

        Loop While Len(FormattedPGN) > 0 AndAlso GameRunning
        OutputDebugInfo()
    End Sub

    'Sends the FEN a user types in to be converted & displayed.
    Private Sub InputButton_Click() Handles InputButton.Click
        InputTextBox.Text = InputTextBox.Text.Trim(" ")
        If InputTextBox.Text <> "" AndAlso InputTextBox.Text <> CurrentFEN AndAlso Not ComputerIsSearching Then
            'Checks for first form of validation (if the 2nd letter is "o", then that usually implies that 
            'the TextBox contains an error message.
            If InputTextBox.Text(1) <> "o" Then
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
                        SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
                        SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
                        If GeneralOptions(0) = "T" Then Sound_Move.Play()
                        CheckChecker(False)

                        If MasterWInCheck.IsInCheck AndAlso Not PlayerTurn Then
                            'If White is in check, and it is black to move, then black can take white's king. This is illegal.
                            GameRunning = False
                            Console.WriteLine("The Game has Ended. Cause = Invalid Check.")
                        ElseIf MasterBInCheck.IsInCheck AndAlso PlayerTurn Then
                            'If Black is in check, and it is white to move, then white can take black's king. This is illegal.
                            Console.WriteLine("The Game has Ended. Cause = Invalid Check.")
                            GameRunning = False
                        ElseIf Math.Abs(Val(MasterWKPos(0)) - Val(MasterBKPos(0))) <= 1 AndAlso Math.Abs(Val(MasterWKPos(1)) - Val(MasterBKPos(1))) <= 1 Then
                            'Kings are too close together.
                            GameRunning = False
                            Console.WriteLine("The Game has Ended. Cause = Invalid King Positions.")
                            CheckLabel.Text = " Stalemate! "
                            If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
                        End If

                        'Recalibrates AI and resets AI move info.
                        MainAI.Reconfigure(CurrentFEN)
                        CurrentDepth = 2
                        CurrentMove = "-"
                        CurrentEvaluation = "-"
                        CurrentAIDepth.Text = "Current Depth: -"
                        CurrentAIMove.Text = "Current Move: -"
                        CurrentAIEval.Text = "Evaluation: -"
                        EnforceEndStates()
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
        If Not ComputerIsSearching Then
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
                SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
            Else
                SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
            End If
            CheckChecker(False)
            If (PlayerTurn Xor OrientForWhite) AndAlso Not (GameMode = 1 AndAlso (UserPlayer Xor Not OrientForWhite)) Then FlipBoard()
            GameRunning = True
            If GameMode < 3 Then
                InputTextBox.Text = StartingFEN
                InputTextBox.Select(InputTextBox.Text.Length, 0) 'Stops the TextBox from being highlighted.
            End If

            'Recalibrates AI and resets AI move info.
            MainAI.Reconfigure(CurrentFEN)
            CurrentDepth = 2
            CurrentMove = "-"
            CurrentEvaluation = "-"
            CurrentAIDepth.Text = "Current Depth: -"
            CurrentAIMove.Text = "Current Move: -"
            CurrentAIEval.Text = "Evaluation: -"
            EnforceEndStates()
            Console.Clear()
            If GeneralOptions(0) = "T" Then Sound_Move.Play()
        End If
    End Sub

    'Outputs the current FEN into the FEN textbox.
    Private Sub FENExport_Click() Handles FENExport.Click
        InputTextBox.Text = CurrentFEN
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
                SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
            Else
                SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
            End If
            If GeneralOptions(0) = "T" Then Sound_Move.Play()
            CheckChecker(False)

            If MasterWInCheck.IsInCheck AndAlso Not PlayerTurn Then
                'If White is in check, and it is black to move, then black can take white's king. This is illegal.
                GameRunning = False
                Console.WriteLine("The Game has Ended. Cause = Invalid Check.")
            ElseIf MasterBInCheck.IsInCheck AndAlso PlayerTurn Then
                'If Black is in check, and it is white to move, then white can take black's king. This is illegal.
                Console.WriteLine("The Game has Ended. Cause = Invalid Check.")
                GameRunning = False
            ElseIf Math.Abs(Val(MasterWKPos(0)) - Val(MasterBKPos(0))) <= 1 AndAlso Math.Abs(Val(MasterWKPos(1)) - Val(MasterBKPos(1))) <= 1 Then
                'Kings are too close together.
                GameRunning = False
                Console.WriteLine("The Game has Ended. Cause = Invalid King Positions.")
                CheckLabel.Text = " Stalemate! "
                If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
            Else
                If GeneralOptions(0) = "T" Then Sound_Move.Play()
            End If

            If GameMode < 3 Then InputTextBox.Text = CurrentFEN
            'Recalibrates AI, then checks for end positions.
            MainAI.Reconfigure(CurrentFEN)
            EnforceEndStates()
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
        If (UseQuiescence AndAlso UserTimeBar.Value < 4) OrElse Not UseQuiescence AndAlso UserTimeBar.Value < 2 Then
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
                    Temp = InputBox("Please input how long you want the AI to search for (in seconds)." & vbCrLf & vbCrLf & "(Min Time = 0.1s, Max Time = 600s)", "Time Inputter")
                    If Temp = "" OrElse Temp = " " Then 'Cancel / Exit button was pressed - abort.
                        UserTime = TimeForSearch
                        Exit While
                    End If
                    UserTime = Math.Round(CDec(Temp), 1)
                    If UserTime >= 0.1 AndAlso UserTime <= 600 Then
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

            'Edits GUI objects, and updates TimeForSearch.
            UserTimeBox.Select(UserTimeBox.Text.Length, 0)
            UserTimeBar.Value = 2 * Math.Round(Math.Min(UserTime, 30))
            UserTimeBox.Text = "Time For Search: " & UserTime & " seconds."
            TimeForSearch = UserTime
        End If
    End Sub
    Private Sub QuiescenceBox_CheckedChanged() Handles QuiescenceBox.CheckedChanged
        'Toggles Quiescence depending on the state of the box.
        If QuiescenceBox.Checked Then UseQuiescence = True : Else UseQuiescence = False
        If (UseQuiescence AndAlso UserTimeBar.Value < 4) OrElse Not UseQuiescence AndAlso UserTimeBar.Value < 2 Then
            UserTimeBox.BackColor = Color.LightCoral
        Else
            UserTimeBox.BackColor = Color.WhiteSmoke
        End If
    End Sub

    'Subroutine that terminates the AI search, and displays on the board the AI's current move.
    Private Sub AITerminator_Click() Handles AITerminator.Click
        TerminateSearch = True
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
            Console.WriteLine("Error when retrieving White's Leaderboard data.")
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
            Console.WriteLine("Error when retrieving Black's Leaderboard data.")
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
    Private Sub TrainingTimer_Tick() Handles TrainingTimer.Tick
        TrainingTimerTicks += 1
        'Updates Timer.
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
                            If System.Text.RegularExpressions.Regex.IsMatch(NewName, "^[A-Za-z ]+$") Then 'Only accepts letters & spaces.
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
        While True
            'Picks a random line from the file.
            NewFEN = System.IO.File.ReadAllLines(Application.StartupPath & "\Assets\RandomFENs.txt")(Math.Truncate(Rnd() * FENsInFile))
            'Applies that FEN to Masterboard and calibrates board info.
            MasterBoard = SharedAlgorithms.FENConverter(NewFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
            SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
            'If White is not in check, generate all the legal moves in the position.
            If Not MasterWInCheck.IsInCheck Then
                MovesInPosition = MainAI.CreateMoves(MasterBoard, True, MasterWhiteTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterWCanCastle, MasterEnPassant, False, 0)
                If MovesInPosition(0, 0) > MovesPerPosition * 3 Then Exit While 'If the number of legal moves in the position is > MovesPerPosition * 3 then the position is valid (lowers chance of moves being repeted).
            End If
            NoOfRetries += 1
        End While

        MainAI.Reconfigure(NewFEN)
        Console.WriteLine("New Position: " & NewFEN)
        If NoOfRetries > 0 Then Console.WriteLine("Number of Retries: " & NoOfRetries)
        Return MovesInPosition
    End Function

    'Function that retieves a random move in the position (for the Move Training mode).
    Private Function GenerateNewTrainingMove() As String
        Dim TempMove As New Move
        While True
            'Retrieves random move from MovesInPosition, then contructs TempMove using this data.
            Dim RndIndex As Byte = Math.Truncate(Rnd() * (MovesInPosition(0, 0) - 1)) + 1
            TempMove.OldMoveX = MovesInPosition(RndIndex, 0)(0)
            TempMove.OldMoveY = MovesInPosition(RndIndex, 0)(1)
            TempMove.NewMoveX = MovesInPosition(RndIndex, 1)(0)
            TempMove.NewMoveY = MovesInPosition(RndIndex, 1)(1)
            'Converts this Move to standard chess notation. If this is different to the current move then accept & return it.
            GenerateNewTrainingMove = SharedAlgorithms.MoveConverter(MasterBoard, TempMove, MasterEnPassant)
            If MoveDisplayer.Text <> GenerateNewTrainingMove Then Return GenerateNewTrainingMove
        End While
    End Function




    'Subroutine that reveals the settings menu to the user.
    Private Sub SettingsBtn_Click() Handles SettingsBtn.Click
        'Creates and reveals the Settings Form.
        Dim SettingsMenu As New Settings(GameMode <= 3)
        SettingsMenu.Show()
    End Sub

    'Button that displays information about the training modes.
    Private Sub InfoBtn_Click() Handles InfoBtn.Click
        If GameMode = 5 Then
            MsgBox("• In this training game, you will be given a chess coordinate (in standard chess notation), and will be tasked with clicking the corresponding square on the chessboard as fast as you can." & vbCrLf & "• If you click the correct square, it will light up in green, and you will be given a new coordinate to click." & vbCrLf & "• Click the most coordinates you can in 20 seconds, and try to get a place on the leaderboard!" & vbCrLf & "• Clicking the 'Flip Board' button will rotate the board 180°, so that each coordinate will be from black's point of view.", vbInformation + vbApplicationModal, "Instructions")
        Else
            MsgBox("• In this training game, you will be given a random chess position along with a possible move (in standard chess notation). You will need to play that move as fast as you can." & vbCrLf & "• If you make the correct move, that square will light up in green, and you will be given a new move to make." & vbCrLf & "• After " & MovesPerPosition & " moves are made in a position, a new random position will be generated." & vbCrLf & "• Complete the most moves you can in 30 seconds, and try to get a place on the leaderboard!" & vbCrLf & "• Clicking the 'Flip Board' button will rotate the board 180°, so that each move will be from black's point of view." & vbCrLf & vbCrLf & "• Note: Regardless of whether the board is flipped or not, only the white pieces will need to be moved.", vbInformation + vbApplicationModal, "Instructions")
        End If
    End Sub

    'Button that returns the user back to the Main Menu. 
    Private Sub ExitBtn_Click() Handles ExitBtn.Click
        'Locates MainMenu Form and shows it.
        Dim Menu As Form = CType(Application.OpenForms("MainMenu"), MainMenu)
        'Closes the settings form, if it exists.
        Dim Settings As Form = CType(Application.OpenForms("Settings"), Settings)
        If Settings IsNot Nothing Then Settings.Close()
        Me.Close() 'Closes the current form.
        Menu.Show()
    End Sub

    'Button that displays the credits information onto the screen (in the form of a pop-up).
    Private Sub Credits_Click() Handles Credits.Click
        MsgBox(Strings.StrDup(10, " ") & "Chess Game & Artificial Intelligence (v6.2)" & vbCrLf & Strings.StrDup(21, " ") & "Created by Alfie Kunz (8158)" & vbCrLf & Strings.StrDup(22, " ") & "of Beckfoot School (37101)" & vbCrLf & "Project used for the AQA GCE Computer Science NEA" & vbCrLf & Strings.StrDup(35, " ") & "(2021 - 2023)", vbInformation + vbApplicationModal, "Credits")
    End Sub



    'The below three subroutines control the drag & drop mechanics for the pieces, and translates a user's move onto the board.
    Private Sub Piece_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles WK1.MouseDown, WQ1.MouseDown, WB1.MouseDown, WB2.MouseDown, WN1.MouseDown, WN2.MouseDown, WR1.MouseDown, WR2.MouseDown, WP1.MouseDown, WP2.MouseDown, WP3.MouseDown, WP4.MouseDown, WP5.MouseDown, WP6.MouseDown, WP7.MouseDown, WP8.MouseDown, BK1.MouseDown, BQ1.MouseDown, BB1.MouseDown, BB2.MouseDown, BN1.MouseDown, BN2.MouseDown, BR1.MouseDown, BR2.MouseDown, BP1.MouseDown, BP2.MouseDown, BP3.MouseDown, BP4.MouseDown, BP5.MouseDown, BP6.MouseDown, BP7.MouseDown, BP8.MouseDown, WQ8.MouseDown, WQ7.MouseDown, WQ6.MouseDown, WQ5.MouseDown, WQ4.MouseDown, WQ3.MouseDown, BQ6.MouseDown, BQ8.MouseDown, BQ5.MouseDown, BQ7.MouseDown, BQ2.MouseDown, BQ4.MouseDown, BQ3.MouseDown, WQ2.MouseDown, WQ9.MouseDown, BQ9.MouseDown
        'Subroutine that activates the drag + drog mechanic if the user clicks on a piece.
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso GameRunning AndAlso Not ComputerIsSearching Then
            'If the piece corresponds to the player allowed to move...
            If (sender.name(0) = "W" AndAlso PlayerTurn) OrElse (sender.name(0) = "B" AndAlso (Not PlayerTurn)) Then
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
                'Notes location of the piece for drag & drop mechanics.
                PieceMoving.StartPoint = sender.location
                PieceMoving.MidPoint = sender.PointToScreen(New Point(e.X, e.Y))
                Checkerboard.Refresh()
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

                If IsLegalMove AndAlso GameMode <> 6 Then
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
                        SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, SharedAlgorithms.NotInCheck, MasterBInCheck, MasterEnPassant)
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
                        SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, SharedAlgorithms.NotInCheck, MasterEnPassant)
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
                    MainAI.Reconfigure(CurrentFEN) 'Recalibrates AI before checking for end states.
                    EnforceEndStates()
                    If CurrentFEN = PreviousFEN Then Exit Sub 'Stops AI from running if it is the start position.
                    OutputDebugInfo()
                    If GameMode < 3 Then
                        InputTextBox.Text = CurrentFEN
                        If GameMode = 1 Then InstantiateAIs()
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
        If GeneralOptions(4) = "F" Then
            If MasterWInCheck.IsInCheck AndAlso GeneralOptions(3) = "T" Then
                WK1.Image = Image.FromFile(Application.StartupPath & "\Images\Default\WKingCheck.png")
            Else
                WK1.Image = Image.FromFile(Application.StartupPath & "\Images\Default\WKing.png")
            End If
            If MasterBInCheck.IsInCheck AndAlso GeneralOptions(3) = "T" Then
                BK1.Image = Image.FromFile(Application.StartupPath & "\Images\Default\BKingCheck.png")
            Else
                BK1.Image = Image.FromFile(Application.StartupPath & "\Images\Default\BKing.png")
            End If
        End If
    End Sub

    'Subroutine that animates a piece move (given a Move).
    Private Sub AnimateMove(ByVal TempMove As Move)
        'Creates TempPiece (the piece that is moving) and ReplacedPiece (the piece that is being captured).
        Dim TempPiece As PictureBox = CoorToPieceConverter(TempMove.OldMoveX, TempMove.OldMoveY)
        Dim ReplacedPiece As PictureBox
        If TempMove.NewMoveX & TempMove.NewMoveY = MasterEnPassant Then
            ReplacedPiece = CoorToPieceConverter(TempMove.NewMoveX, TempMove.NewMoveY - (2 * Val(PlayerTurn) + 1))
        Else
            ReplacedPiece = CoorToPieceConverter(TempMove.NewMoveX, TempMove.NewMoveY)
        End If

        If Not OrientForWhite Then
            TempMove.OldMoveX = 7 - TempMove.OldMoveX
            TempMove.OldMoveY = 7 - TempMove.OldMoveY
            TempMove.NewMoveX = 7 - TempMove.NewMoveX
            TempMove.NewMoveY = 7 - TempMove.NewMoveY
        End If

        'Creates movement vector.
        Dim XMovement As Decimal = 75 * (TempMove.NewMoveX - TempMove.OldMoveX)
        Dim YMovement As Decimal = 75 * (TempMove.NewMoveY - TempMove.OldMoveY)
        Dim Constant As Decimal
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

        'Pawn promotion control:
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
    End Sub




    'Algorithm which calculates the Overall Depth the AI will run at. Takes into account the Total Material Count
    'on the board and calculates Depth using the formula y = -m*ln(x-a) + c (the higher the Material Count, the lower
    'the Depth).
    Private Sub CalculateAbsoluteDepth()
        If FixedSearchDepth = 0 Then
            'Counts the material on the board.
            Dim TotalMaterial As SByte() = SharedAlgorithms.CountMaterial(MasterBoard)
            'TotalMaterial(0) = White's total material, TotalMaterial(1) = Black's total material.
            'If the user is using Quiescence, then we knock off 0.5 from the depth (as Quiescence is slightly slower).
            Dim DepthAlgorithm As Byte = Math.Truncate(-2 * Math.Log(TotalMaterial(0) + TotalMaterial(1) + 1, 4) + 10.5 + Val(UseQuiescence))


            'Calculates all the pseudo-legal moves of the opposing player.
            Dim EnemyLegalMoves(200, 1) As String
            If PlayerTurn Then
                EnemyLegalMoves = MainAI.CreateMoves(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterBCanCastle, MasterEnPassant, False, 0)
            Else
                EnemyLegalMoves = MainAI.CreateMoves(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterWCanCastle, MasterEnPassant, False, 0)
            End If
            Dim TotalMoves As Byte = Val(MainAI.BasePieceMoves(0, 0)) + Val(EnemyLegalMoves(0, 0))
            Dim TotalCaptureMoves As Byte = Val(MainAI.BasePieceMoves(0, 1)) + Val(EnemyLegalMoves(0, 1))


            'If the previous AI search resulted in a forced Checkmate being found, the depth is limited to only the
            'depth that is required to achieve that Checkmate. This saves on a lot of unnecessary processing, as
            'forced Checkmates are unavoidable.
            If CurrentEvaluation <> "-" AndAlso Math.Abs(Val(CurrentEvaluation)) > 900 Then
                AbsoluteDepth = CurrentDepth - (Math.Abs(Val(CurrentEvaluation)) - 999)
            Else
                AbsoluteDepth = DepthAlgorithm

                'AbsoluteDepth = Math.Truncate(-2 * Math.Log(TotalMoves, 2) + 18)
                'If UseQuiescence Then AbsoluteDepth -= Math.Truncate(Math.Log(TotalCaptureMoves + 1, 2)) + 0.5

            End If
            'The Math.Max function is used to ensure that any AI's Depth never gets lower than 2. Along with general quality
            'improvements (as a depth of 2 can be completed instantly), this also prevents the depth from reaching 1, 0, or
            'negative - all of which would cause errors / invalid results when searching.
            AbsoluteDepth = Math.Max(AbsoluteDepth, 4)
        Else 'AbsoluteDepth = the depth set by the user
            AbsoluteDepth = FixedSearchDepth
        End If
        Console.WriteLine("Absolute Depth = " & AbsoluteDepth)
    End Sub

    'Function that uses the binary search to find a given FEN in the opening book.
    Private Function FindPositionInBook(ByVal FEN As String) As UInt32
        Dim LB As UInt32 = 1 'Lower Bound of Binary Search.
        Dim UB As UInt32 = Val(OpeningBook(0, 0)) 'Upper Bound of Binary Search.
        Dim MidPoint As UInt32
        While LB <= UB
            MidPoint = (UB + LB) \ 2
            If FEN = OpeningBook(MidPoint, 0) Then
                Return MidPoint
            ElseIf FEN > OpeningBook(MidPoint, 0) Then
                LB = MidPoint + 1
            Else
                UB = MidPoint - 1
            End If
        End While
        Return 0
    End Function


    'Algorithm that creates all the threads & AIs that will search on a position. Handles time management + overtime,
    'interactions with the GUI chessboard, live updates via GUI objects, selecting best moves, and more.
    Private Sub InstantiateAIs() Handles AIMoveBtn.Click
        If GameRunning AndAlso Not ComputerIsSearching Then
            Dim IsLate As Boolean 'Represents whether the computer has used more time than it was originally allocated.
            Dim BestMove As Move = MainAI.CheckForEndState() 'Ensures that the position is valid, and that the
            'AIs won't crash whilst searching on the position.
            If BestMove.EndState = "f" OrElse BestMove.EndState = "o" Then
                Dim IndexInBook As UInt32
                If UseBook.Checked Then IndexInBook = FindPositionInBook(CurrentFEN) 'Finds the index of the opening book that the position exists in.
                If BestMove.EndState = "o" Then
                    'Move was forced - make move instantly without searching.
                    Console.WriteLine(vbCrLf & "Search Aborted - Only 1 Move in Position.")
                    CurrentAIDepth.Text = "Current Depth: 1"
                    CurrentAIMove.Text = "Current Move: " & SharedAlgorithms.MoveConverter(MasterBoard, BestMove, MasterEnPassant) & " (FORCED)."
                ElseIf IndexInBook > 0 Then 'Position found in book - play move instantly.
                    Dim PossibleMoves As String = OpeningBook(IndexInBook, 1)
                    Console.WriteLine(vbCrLf & "Search Aborted - Position Found in Opening Book.")
                    Console.WriteLine("Moves Available: " & PossibleMoves & ". Picking random move...")
                    Dim BookMoves As New List(Of String) 'Creates list containing all the opening moves (constructed from PossibleMoves).
                    Dim PreviousCommaIndex As Int16 = -1
                    For n = 1 To PossibleMoves.Length - 1
                        If PossibleMoves(n) = "," Then
                            BookMoves.Add(PossibleMoves.Substring(PreviousCommaIndex + 1, n - PreviousCommaIndex - 1))
                            PreviousCommaIndex = n
                        ElseIf n = PossibleMoves.Length - 1 Then 'Is the last index - add the final move.
                            BookMoves.Add(PossibleMoves.Substring(PreviousCommaIndex + 1, PossibleMoves.Length - PreviousCommaIndex - 1))
                        End If
                    Next
                    'Choose random opening move, then convert this move into BestMove.
                    Dim RND As New Random()
                    Dim ChosenBookMove As String = BookMoves(Math.Truncate(RND.Next(0, BookMoves.Count)))
                    If PlayerTurn Then
                        BestMove = SharedAlgorithms.ConvertToMove(ChosenBookMove, MasterBoard, True, MasterWKPos, MasterWhiteTFTable)
                    Else
                        BestMove = SharedAlgorithms.ConvertToMove(ChosenBookMove, MasterBoard, False, MasterBKPos, MasterBlackTFTable)
                    End If
                    CurrentAIDepth.Text = "Current Depth: 0"
                    CurrentAIMove.Text = "Current Move: " & ChosenBookMove & " (Book)."
                    CurrentAIEval.Text = "Evaluation: 0"
                Else 'no forced or book move found - call AI.
                    'Instantiations of threads (in a List of tasks format).
                    Dim Tasks As New List(Of Task) With {
                        .Capacity = 6
                    }
                    If FixedSearchDepth = 0 Then Tasks.Add(New Task(AddressOf InitialiseThread1))
                    If FixedSearchDepth = 0 Then Tasks.Add(New Task(AddressOf InitialiseThread2))
                    If FixedSearchDepth = 0 Then Tasks.Add(New Task(AddressOf InitialiseThread3))
                    If FixedSearchDepth = 0 Then Tasks.Add(New Task(AddressOf InitialiseThread4))
                    Tasks.Add(New Task(AddressOf InitialiseThread5))
                    If FixedSearchDepth > 0 Then
                        For i = 0 To 3
                            AIBestMoves(i).EndState = "a"
                        Next
                    End If

                    CalculateAbsoluteDepth()
                    AIMovedToHigherDepth(0) = False
                    AIMovedToHigherDepth(1) = False
                    'Resets GUI objects & changes cursor design.
                    Me.Cursor = Cursors.AppStarting
                    ProgressBar.ForeColor = Color.FromArgb(0, 192, 0)
                    ProgressBar.Value = 0
                    UserTimeBar.Enabled = False
                    QuiescenceBox.Enabled = False
                    'Erases the information from the last AI search.
                    CurrentDepth = 2
                    CurrentMove = "-"
                    CurrentEvaluation = "-"
                    TerminateSearch = False
                    AITerminator.Visible = True
                    AIFinishedSearch = True

                    ComputerIsSearching = True
                    Console.WriteLine(vbCrLf & "The AI is now searching..." & vbCrLf & "Time for Search = " & TimeForSearch & " Seconds. Quiescence Mode: " & UseQuiescence & vbCrLf)
                    AIStopwatch.Reset()
                    AIStopwatch.Start()
                    'Starts all tasks simultaneously.
                    Tasks.All(Function(t As Task)
                                  t.Start()
                                  Return True
                              End Function)
                    'While any of the 5 threads are running...
                    While Tasks.Any(Function(t As Task) Not (t.Status = TaskStatus.Canceled OrElse t.Status = TaskStatus.Faulted OrElse t.Status = TaskStatus.RanToCompletion))
                        Application.DoEvents() 'Allows the user to interact with the GUI (limited).
                        'Updates progress bar to reflect time.
                        If IsLate Then
                            '2x speed with red colour.
                            ProgressBar.Value = Math.Min((Math.Max(AIStopwatch.ElapsedMilliseconds - TimeForSearch * 1000, 0) / 5) / TimeForSearch, 100)
                        Else 'Normal speed with green colour.
                            ProgressBar.Value = Math.Min(((AIStopwatch.ElapsedMilliseconds / 10) / TimeForSearch), 100)
                        End If

                        If AIStopwatch.ElapsedMilliseconds / 1000 > TimeForSearch OrElse TerminateSearch Then 'Time's up!
                            If Not MainAI.ABORT Then
                                'Aborts all AIs (apart from AI of lowest depth if it has not finished).
                                If AIMovedToHigherDepth(0) Then AI2.ABORT = True
                                AI3.ABORT = True
                                MainAI.ABORT = True
                                AI4.ABORT = True
                                AI5.ABORT = True
                                IsLate = True
                                ProgressBar.ForeColor = Color.Red 'Red progress bar - visual indication to user that time is up.
                            End If
                            If AIStopwatch.ElapsedMilliseconds / 1000 > Math.Min(TimeForSearch * 1.5, TimeForSearch + 10000) OrElse TerminateSearch Then
                                '1.5 * time (or extra 10s) has expired - terminate all AIs immediately!
                                If Not AI2.ABORT Then AI2.ABORT = True
                                If Not AI3.ABORT Then AI3.ABORT = True
                                If Not MainAI.ABORT Then MainAI.ABORT = True
                                If Not AI4.ABORT Then AI4.ABORT = True
                                If Not AI5.ABORT Then AI5.ABORT = True
                            End If
                        ElseIf AIFinishedSearch Then 'Updates GUI controls on latest search & redraws checkerboard.
                            Checkerboard.Refresh()
                            CurrentAIDepth.Text = "Current Depth: " & CurrentDepth
                            CurrentAIMove.Text = "Current Move: " & CurrentMove
                            If CurrentEvaluation <> "-" AndAlso Math.Abs(Val(CurrentEvaluation)) > 900 Then
                                'Mating pattern has been found.
                                CurrentAIEval.Text = "Evaluation: Mate in " & (CurrentDepth - (Math.Abs(Val(CurrentEvaluation)) - 1000)) \ 2
                            Else
                                CurrentAIEval.Text = "Evaluation: " & CurrentEvaluation
                            End If
                            AIFinishedSearch = False
                        End If
                        Thread.Sleep(5) 'Allows more processing time to be spent on the AIs.
                    End While
                    AIStopwatch.Stop()
                    If TerminateSearch Then Console.WriteLine("Search Terminated Preemptively (by user).")

                    'Select the best move of the highest depth possible (if that search has not been aborted).
                    If AIMovedToHigherDepth(1) AndAlso AIBestMoves(6).EndState <> "a" Then
                        BestMove = AIBestMoves(6)
                    ElseIf AIMovedToHigherDepth(0) AndAlso AIBestMoves(5).EndState <> "a" Then
                        BestMove = AIBestMoves(5)
                    ElseIf AIBestMoves(4).EndState <> "a" Then
                        BestMove = AIBestMoves(4)
                    ElseIf AIBestMoves(3).EndState <> "a" Then
                        BestMove = AIBestMoves(3)
                    ElseIf AIBestMoves(2).EndState <> "a" Then
                        BestMove = AIBestMoves(2)
                    ElseIf AIBestMoves(1).EndState <> "a" Then
                        BestMove = AIBestMoves(1)
                    ElseIf AIBestMoves(0).EndState <> "a" Then
                        BestMove = AIBestMoves(0)
                    Else 'No AI have completed their search - start a new search at a depth of 2 (without Quiescence).
                        'Sets the result of this new to be the best move.
                        BestMove = MainAI.Search(2, False)
                        CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, BestMove, MasterEnPassant)
                    End If
                    Console.WriteLine(vbCr & "Search Complete. Total Time: " & AIStopwatch.Elapsed.TotalMilliseconds & " Milliseconds.")

                    'Updates GUI labels (containing info on search).
                    AITerminator.Visible = False
                    TerminateSearch = False
                    CurrentAIDepth.Text = "Current Depth: " & CurrentDepth
                    CurrentEvaluation = BestMove.Score
                    CurrentAIMove.Text = "Current Move: " & CurrentMove
                    If CurrentEvaluation <> "-" AndAlso Math.Abs(Val(CurrentEvaluation)) > 900 Then
                        'Mating pattern has been found - update GUI accordingly
                        CurrentAIEval.Text = "Evaluation: Mate in " & (CurrentDepth - (Math.Abs(Val(CurrentEvaluation)) - 1000)) \ 2
                    Else
                        CurrentAIEval.Text = "Evaluation: " & CurrentEvaluation
                    End If
                    ProgressBar.ForeColor = Color.FromArgb(0, 192, 0)
                    ProgressBar.Value = 0

                End If

                If Me.IsHandleCreated Then 'If form is still open...
                    GameRunning = True
                    'Makes the best move on the board, and updates the correct TFTable.

                    AnimateMove(BestMove)
                    If PlayerTurn Then
                        MasterWInCheck.NotInCheck() 'Player is no longer in check.
                        MakeMove(MasterBoard, BestMove.OldMoveX, BestMove.OldMoveY, BestMove.NewMoveX, BestMove.NewMoveY, MasterWCanCastle, MasterWKPos, MasterEnPassant, GeneralOptions(0) = "T")
                        SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
                    Else
                        MasterBInCheck.NotInCheck() 'Player is no longer in check.
                        MakeMove(MasterBoard, BestMove.OldMoveX, BestMove.OldMoveY, BestMove.NewMoveX, BestMove.NewMoveY, MasterBCanCastle, MasterBKPos, MasterEnPassant, GeneralOptions(0) = "T")
                        SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
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
                    If GameMode = 3 Then
                        PreviousFEN = CurrentFEN
                    Else
                        InputTextBox.Text = SharedAlgorithms.ConvertToFEN(MasterBoard, MasterWCanCastle, MasterBCanCastle, MasterEnPassant, PlayerTurn)
                    End If
                    CurrentFEN = SharedAlgorithms.ConvertToFEN(MasterBoard, MasterWCanCastle, MasterBCanCastle, MasterEnPassant, PlayerTurn)
                    MainAI.Reconfigure(CurrentFEN) 'Recalibrates AI.
                    'Checks if the new position is in the opening book. If so then output these moves.
                    If UseBook.Checked AndAlso GameMode = 3 AndAlso IndexInBook > 0 Then
                        IndexInBook = FindPositionInBook(CurrentFEN) 'Finds the index of the opening book that the position exists in.
                        If IndexInBook = 0 Then
                            Console.WriteLine("No Longer in Opening Book.")
                        Else
                            'Retrieves all the moves in the opening book.
                            Dim PossibleMoves As String = OpeningBook(IndexInBook, 1)
                            Console.WriteLine("Moves In Opening Book (in Current Position): " & PossibleMoves & ".")
                        End If
                    End If

                    'Resets GUI objects & cursor design.
                    Me.Cursor = Cursors.Default
                    UserTimeBar.Enabled = True
                    QuiescenceBox.Enabled = True
                    ComputerIsSearching = False
                    EnforceEndStates() 'Checks for end states found from the AI's move.
                    OutputDebugInfo()
                    'The below lines fix a bug, where the AI wouldn't make a move after the user restarts their game (1P mode only).
                    If AIEndlessMode.Checked OrElse (GameMode = 1 AndAlso CurrentFEN = PreviousFEN AndAlso Not UserPlayer = PlayerTurn) Then
                        Thread.Sleep(5) 'Allows the system to recalibrate (to prevent spam).
                        'If UserPlayer Xor OrientForWhite Then FlipBoard()
                        InstantiateAIs()
                    End If

                End If
            End If
        End If
    End Sub

    'Subroutines controlling each thread / AI of particular depth.
    Private Sub InitialiseThread1()
        'Creates and starts the AI.
        AIBestMoves(0) = AI2.Search(AbsoluteDepth - 2, UseQuiescence)
        If Not AI2.ABORT Then
            'Outputs the move if the move has completed successfully.
            Console.WriteLine("Depth Of " & AbsoluteDepth - 2 & " Completed in: " & AIStopwatch.ElapsedMilliseconds & " Milliseconds." & vbCrLf)
            AmmendLegalMoveSuares(AIBestMoves(0))
            AIFinishedSearch = True
            CurrentDepth = AbsoluteDepth - 2
            CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, AIBestMoves(0), MasterEnPassant)
            CurrentEvaluation = AIBestMoves(0).Score
            If Math.Abs(AIBestMoves(0).Score) > 900 Then
                'Checkmating sequence found - terminate all other AIs.
                AI3.ABORT = True
                MainAI.ABORT = True
                AI4.ABORT = True
                AI5.ABORT = True
            ElseIf AIStopwatch.ElapsedMilliseconds / 500 < TimeForSearch Then
                'Starts a new search at a higher depth (if there is >50% left on the timer).
                AIMovedToHigherDepth(0) = True
                AIBestMoves(5) = AI2.Search(AbsoluteDepth + 3, UseQuiescence)
                If Not AI2.ABORT Then
                    'Outputs the move if the move has completed successfully.
                    Console.WriteLine("Depth Of " & AbsoluteDepth + 3 & " Completed in: " & AIStopwatch.ElapsedMilliseconds & " Milliseconds." & vbCrLf)
                    AmmendLegalMoveSuares(AIBestMoves(5))
                    AIFinishedSearch = True
                    CurrentDepth = AbsoluteDepth + 3
                    CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, AIBestMoves(5), MasterEnPassant)
                    CurrentEvaluation = AIBestMoves(5).Score
                    If Math.Abs(AIBestMoves(5).Score) > 900 Then
                        'Checkmating sequence found - terminate all other AIs.
                        AI3.ABORT = True
                        MainAI.ABORT = True
                        AI4.ABORT = True
                        AI5.ABORT = True
                    End If
                End If
            End If
        End If
    End Sub
    'The below 4 subroutines are very similar to the one above - just for different depths.
    '(also note that, apart from the immediately below subroutine, the below algorithms do
    'not contain the 'increase to higher depth once completed' mechanism).
    Private Sub InitialiseThread2()
        AIBestMoves(1) = AI3.Search(AbsoluteDepth - 1, UseQuiescence)
        If Not AI3.ABORT Then
            Console.WriteLine("Depth Of " & AbsoluteDepth - 1 & " Completed in: " & AIStopwatch.ElapsedMilliseconds & " Milliseconds." & vbCrLf)
            AmmendLegalMoveSuares(AIBestMoves(1))
            AIFinishedSearch = True
            CurrentDepth = AbsoluteDepth - 1
            CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, AIBestMoves(1), MasterEnPassant)
            CurrentEvaluation = AIBestMoves(1).Score
            If Math.Abs(AIBestMoves(1).Score) > 900 Then
                MainAI.ABORT = True
                AI4.ABORT = True
                AI5.ABORT = True
                AI2.ABORT = True
            ElseIf AIStopwatch.ElapsedMilliseconds / 500 < TimeForSearch Then
                AIMovedToHigherDepth(1) = True
                AIBestMoves(6) = AI3.Search(AbsoluteDepth + 4, UseQuiescence)
                If Not AI3.ABORT Then
                    Console.WriteLine("Depth Of " & AbsoluteDepth + 4 & " Completed in: " & AIStopwatch.ElapsedMilliseconds & " Milliseconds." & vbCrLf)
                    AmmendLegalMoveSuares(AIBestMoves(6))
                    AIFinishedSearch = True
                    CurrentDepth = AbsoluteDepth + 4
                    CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, AIBestMoves(6), MasterEnPassant)
                    CurrentEvaluation = AIBestMoves(6).Score
                    If Math.Abs(AIBestMoves(6).Score) > 900 Then
                        MainAI.ABORT = True
                        AI4.ABORT = True
                        AI5.ABORT = True
                        AI2.ABORT = True
                    End If
                End If
            End If
        End If
    End Sub
    Private Sub InitialiseThread3()
        AIBestMoves(2) = MainAI.Search(AbsoluteDepth, UseQuiescence)
        If Not MainAI.ABORT Then
            Console.WriteLine("Depth Of " & AbsoluteDepth & " Completed in: " & AIStopwatch.ElapsedMilliseconds & " Milliseconds." & vbCrLf)
            AmmendLegalMoveSuares(AIBestMoves(2))
            AIFinishedSearch = True
            CurrentDepth = AbsoluteDepth
            CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, AIBestMoves(2), MasterEnPassant)
            CurrentEvaluation = AIBestMoves(2).Score
            If Math.Abs(AIBestMoves(2).Score) > 900 Then
                AI4.ABORT = True
                AI5.ABORT = True
                AI2.ABORT = True
                AI3.ABORT = True
            End If
        End If
    End Sub
    Private Sub InitialiseThread4()
        AIBestMoves(3) = AI4.Search(AbsoluteDepth + 1, UseQuiescence)
        If Not AI4.ABORT Then
            Console.WriteLine("Depth Of " & AbsoluteDepth + 1 & " Completed in: " & AIStopwatch.ElapsedMilliseconds & " Milliseconds." & vbCrLf)
            AmmendLegalMoveSuares(AIBestMoves(3))
            AIFinishedSearch = True
            CurrentDepth = AbsoluteDepth + 1
            CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, AIBestMoves(3), MasterEnPassant)
            CurrentEvaluation = AIBestMoves(3).Score
            If Math.Abs(AIBestMoves(3).Score) > 900 Then
                AI5.ABORT = True
                AI2.ABORT = True
                AI3.ABORT = True
                MainAI.ABORT = True
            End If
        End If
    End Sub
    Private Sub InitialiseThread5()
        Dim i As Byte
        If FixedSearchDepth = 0 Then i = 2 Else i = 0
        AIBestMoves(4) = AI5.Search(AbsoluteDepth + i, UseQuiescence)
        If Not AI5.ABORT Then
            Console.WriteLine("Depth Of " & AbsoluteDepth + i & " Completed in: " & AIStopwatch.ElapsedMilliseconds & " Milliseconds." & vbCrLf)
            AmmendLegalMoveSuares(AIBestMoves(4))
            AIFinishedSearch = True
            CurrentDepth = AbsoluteDepth + i
            CurrentMove = SharedAlgorithms.MoveConverter(MasterBoard, AIBestMoves(4), MasterEnPassant)
            CurrentEvaluation = AIBestMoves(4).Score
            If Math.Abs(AIBestMoves(4).Score) > 900 Then
                AI2.ABORT = True
                AI3.ABORT = True
                MainAI.ABORT = True
                AI2.ABORT = True
            End If
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
                    Board(NewCoorX, NewCoorY + 1) = " "
                    If MakeSounds Then Sound_Capture.Play()
                ElseIf OldCoorY = 6 AndAlso NewCoorY = 4 AndAlso (Board(Math.Max(NewCoorX - 1, 0), 4) = "p" OrElse Board(Math.Min(NewCoorX + 1, 7), 4) = "p") Then
                    'EnPassant Creation.
                    EnPassant = NewCoorX & 5
                    HasEnPassanted = True
                End If
                'If piece is a Rook, part of Castling is disabled (depending on which Rook has moved).
            ElseIf TempPiece = "R" AndAlso CanCastle.CanICastle Then
                If OldCoorX = 0 AndAlso OldCoorY = 7 Then
                    CanCastle.QS = False
                ElseIf OldCoorX = 7 AndAlso OldCoorY = 7 Then
                    CanCastle.KS = False
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
                    Board(NewCoorX, NewCoorY - 1) = " "
                    If MakeSounds Then Sound_Capture.Play()
                ElseIf OldCoorY = 1 AndAlso NewCoorY = 3 AndAlso (Board(Math.Max(NewCoorX - 1, 0), 3) = "P" OrElse Board(Math.Min(NewCoorX + 1, 7), 3) = "P") Then
                    'EnPassant Creation.
                    EnPassant = NewCoorX & 2
                    HasEnPassanted = True
                End If
            ElseIf TempPiece = "r" AndAlso CanCastle.CanICastle Then
                If OldCoorX = 0 AndAlso OldCoorY = 0 Then
                    CanCastle.QS = False
                ElseIf OldCoorX = 7 AndAlso OldCoorY = 0 Then
                    CanCastle.KS = False
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
    Private Sub EnforceEndStates()
        Dim TempMove = MainAI.CheckForEndState()
        Select Case TempMove.EndState
            Case "c" 'Position is checkmate.
                CheckLabel.Text = "Checkmate!"
                If GeneralOptions(0) = "T" Then Sound_Checkmate.Play()
                GameRunning = False
                Console.WriteLine("The Game has Ended. Cause = Checkmate.")
                If GameMode < 3 Then NotifyGameEnd("c")
                Exit Sub
            Case "s" 'Position is stalemate.
                CheckLabel.Text = " Stalemate! "
                If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
                GameRunning = False
                Console.WriteLine("The Game has Ended. Cause = Stalemate.")
                If GameMode < 3 Then NotifyGameEnd("d")
                Exit Sub
        End Select

        'Draw by insufficient material is then checked for. In principle, if it is physically impossible for one
        'player to checkmate the other (such as king vs king, or king vs king + a knight / bishop), then the
        'position is delared 'dead' and the game ends in a draw.
        Dim TempMaterialCount As SByte() = SharedAlgorithms.CountMaterial(MasterBoard)
        If TempMaterialCount(0) + TempMaterialCount(1) = 0 Then 'only kings remain.
            CheckLabel.Text = "     Draw!     "
            If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
            GameRunning = False
            Console.WriteLine("The Game has Ended. Cause = Draw by Insufficient Material (K v K).")
            If GameMode < 3 Then NotifyGameEnd("d")
        ElseIf TempMaterialCount(0) + TempMaterialCount(1) = 3 Then 'could be king vs king + knight / bishop.
            For y = 0 To 7
                For x = 0 To 7
                    'scans for knights / bishops.
                    If UCase(MasterBoard(x, y)) = "B" OrElse UCase(MasterBoard(x, y)) = "N" Then
                        'Game ends in a draw.
                        CheckLabel.Text = "     Draw!     "
                        If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
                        GameRunning = False
                        Console.WriteLine("The Game now Ended. Cause = Draw by Insufficient Material (K v K+B/N).")
                        If GameMode < 3 Then NotifyGameEnd("d")
                        Exit Sub
                    End If
                Next
            Next
        ElseIf TempMaterialCount(0) = 3 AndAlso TempMaterialCount(1) = 3 Then 'only possibility is king + bishop vs king + bishop (of same type).
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
    End Sub

    'Notifies the user if the game has concluded - giving them an appropriate message and an option to play again.
    'If they deny, then they are sent back to the main menu.
    Private Sub NotifyGameEnd(ByVal EndState As Char)
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
