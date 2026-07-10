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
    Private MasterBoard(7, 7) As Char
    Private MasterWhiteTFTable(7, 7), MasterBlackTFTable(7, 7) As Char
    Private GameRunning As Boolean = True
    Private ReadOnly GameMode As Byte = 3 '1 = 1-Player, 2 = 2-Player, 3 = Analysis

    'FEN = Standard Chess notation that displays where all the pieces on the board are supposed to go.
    Private StartingFEN As String = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
    Private CurrentFEN, PreviousFEN, InvalidFEN As String
    Private PlayerTurn As Boolean = True 'True = Is White's Turn, False = Is Black's Turn
    Private UserPlayer As Boolean = True 'Represents what colour the user is playing with.
    Private MasterWKPos, MasterBKPos As String 'Loaction of Kings.
    Private MasterEnPassant As String 'Location of EnPassant Square

    'Data for castling & checks for each player.
    Private MasterWCanCastle As New CanCastle
    Private MasterBCanCastle As New CanCastle
    Private MasterWInCheck As New InCheck
    Private MasterBInCheck As New InCheck

    Private ReadOnly PieceArray(47) As PictureBox 'Contains all the PictureBoxes on the board.
    Private PieceMoving As PieceInfo
    Private PrimaryColour As Color 'Colour representing the dark squares on the Chessboard.
    Private SecondaryColour As Color 'Colour representing the light squares on the Chessboard.
    Private SquareHistory(3, 1) As SByte 'Array containing the previously used squares on the board (for prev moves).
    Private LegalMoveSquares(7, 7) As Boolean 'Array containing the coordinates of where a piece can move on the board,
    '... generated when the user clicks on a piece. Used when redrawing the checkerboard pattern.
    Private OrientForWhite As Boolean = True 'Represents which way the board is flipped.

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
    Private MainAI As New AI()
    Private AI2 As New AI()
    Private AI3 As New AI()
    Private AI4 As New AI()
    Private AI5 As New AI()
    Private AIBestMoves(6) As Move
    Private AIMovedToHigherDepth(1) As Boolean
    Private AIFinishedSearch As Boolean
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




    Public Sub New() 'Call used for a standard game of chess (usually in Analysis Mode).
        InitializeComponent() ' This call is required by the designer.
        StartingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
    End Sub

    'The below constructor method rearranges the location of objects on the form (to save space on the screen).
    'This will be called for 1-Player Games and 2-Player Games.
    Public Sub New(ByVal Mode As Byte, ByVal UserStartingFEN As String, ByVal Difficulty As Byte, ByVal PlayAsWhite As Boolean)
        InitializeComponent() ' This call is required by the designer.
        GameMode = Mode
        'Edits the size of the forms, and modifies the obejct now off-screen.
        Me.Size = New Size(916, 639)
        AIMoveBtn.Visible = False
        FENTextBox.ReadOnly = True
        Reset_Btn.Visible = False
        FENButton.Visible = False
        FENExport.Visible = False
        UndoMove.Top -= 80
        CheckLabel.Location = New Point(89, 128)
        UserTimeBar.Visible = False
        UserTimeBox.Visible = False
        QuiescenceBox.Visible = False
        AIEndlessMode.Visible = False
        ColourChangerButton.Location = New Point(40, 520)
        ExitBtn.Location = New Point(181, 520)
        Credits.Location = New Point(78.5, 575)
        StartingFEN = UserStartingFEN
        If Mode = 1 Then 'Mode-Specific Movement.
            'Hides / sets appropriate objects.
            FlipperButton.Visible = False
            AutoFlipper.Visible = False
            ProgressBar.Location = New Point(46, 250)
            CurrentAIDepth.Location = New Point(ProgressBar.Location.X + 10, ProgressBar.Location.Y + 80)
            CurrentAIMove.Location = New Point(CurrentAIDepth.Location.X, CurrentAIDepth.Location.Y + 30)
            CurrentAIEval.Location = New Point(CurrentAIDepth.Location.X, CurrentAIDepth.Location.Y + 60)
            UserPlayer = PlayAsWhite
            'Difficulty generator.
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
                TimeForSearch = 20
                UseQuiescence = True
            End If
        Else
            FlipperButton.Top -= 100
            AutoFlipper.Top -= 100
            'Hides / sets appropriate objects.
            ProgressBar.Visible = False
            CurrentAIEval.Visible = False
            AutoFlipper.Checked = True
            CurrentAIDepth.Visible = False
            CurrentAIMove.Visible = False
            CurrentAIEval.Visible = False
        End If
    End Sub


    Private Sub Form_Load() Handles Me.Load
        Application.EnableVisualStyles()
        Dim StartupStopwatch As New Stopwatch
        StartupStopwatch.Start()
        'Sets the colour scheme for the board (from txt file). If all else fails, use the default settings.
        Dim ColourScheme As String
        Try
            FileOpen(1, Application.StartupPath & "\ColourPreferences.txt", OpenMode.Input)
            ColourScheme = LineInput(1)
        Catch ex As Exception
            ColourScheme = "def"
        End Try
        FileClose(1)
        If LCase(ColourScheme) = "blu" Then
            PrimaryColour = Color.LightSteelBlue
            SecondaryColour = Color.AliceBlue
        ElseIf LCase(ColourScheme) = "bl2" Then
            PrimaryColour = Color.SlateGray
            SecondaryColour = Color.Silver
        ElseIf LCase(ColourScheme) = "gld" Then
            PrimaryColour = Color.Goldenrod
            SecondaryColour = Color.LemonChiffon
        ElseIf LCase(ColourScheme) = "grn" Then
            PrimaryColour = Color.DarkSeaGreen
            SecondaryColour = Color.LavenderBlush
        ElseIf LCase(ColourScheme) = "ppl" Then
            PrimaryColour = Color.MediumPurple
            SecondaryColour = Color.MistyRose
        ElseIf LCase(ColourScheme) = "mon" Then
            PrimaryColour = Color.DimGray
            SecondaryColour = Color.Silver
        Else 'def
            PrimaryColour = Color.Peru
            SecondaryColour = Color.FloralWhite
        End If

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

        'Assigns all the PictureBoxes with their associated images.
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

        'Sets up more objects...
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
        End If
        If FENErrorDetection(StartingFEN, False, FENErrorMessage) Then 'First step of error-detection.
            Try
                'Forms the chessboard, along with the required variables.
                MasterBoard = SharedAlgorithms.FENConverter(StartingFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
                DisplayPieces()
                'Forms the TFTable for the players.
                SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
                SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
                If GameMode < 3 Then CustomisationForm.Close() 'Closes the customisation form.
                CheckChecker()
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
            ElseIf Math.Abs(CStr(MasterWKPos(0)) - CStr(MasterBKPos(0))) <= 1 AndAlso Math.Abs(CStr(MasterWKPos(1)) - CStr(MasterBKPos(1))) <= 1 Then
                'Kings are too close together.
                GameRunning = False
                Console.WriteLine("The Game has Ended. Cause = Invalid King Positions.")
                CheckLabel.Text = " Stalemate! "
                Sound_Stalemate.Play()
            Else
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
                FENTextBox.Text = StartingFEN
                FENTextBox.Select(FENTextBox.Text.Length, 0) 'Stops the TextBox from being highlighted.
            Else
                PreviousFEN = StartingFEN 'if CurrentFEN = PreviousFEN then the user cannot undo a move.
            End If
            EnforceEndStates() 'Checks for end positions in the starting FEN.
        End If
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
                If LegalMoveSquares(x, y) Then
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
                    If LegalMoveSquares(x, y) Then 'Normal colour is filled at the centre of the legal move square to produce a green / blue highlight.
                        Dim Square2 As New Rectangle((75 * x) + 5, (75 * y) + 5, 65, 65)
                        Using Brush As New SolidBrush(SecondaryColour)
                            g.FillRectangle(Brush, Square2)
                        End Using
                        'If the given square matches a SquareHistory coordinate, then it is coloured in a different way.
                    ElseIf (x = SquareHistory(0, 0) AndAlso y = SquareHistory(0, 1)) OrElse (x = SquareHistory(1, 0) AndAlso y = SquareHistory(1, 1)) Then
                        Using Brush As New SolidBrush(Color.YellowGreen) 'SquareHistory secondary colour.
                            g.FillRectangle(Brush, Square)
                        End Using
                    Else 'Is a normal square - colour with user's seconday colour.
                        Using Brush As New SolidBrush(SecondaryColour)
                            g.FillRectangle(Brush, Square)
                        End Using
                    End If
                Else 'Identical code but for the dark squares (uses different colours).
                    If LegalMoveSquares(x, y) Then
                        Dim Square2 As New Rectangle((75 * x) + 5, (75 * y) + 5, 65, 65)
                        Using Brush As New SolidBrush(PrimaryColour)
                            g.FillRectangle(Brush, Square2)
                        End Using
                    ElseIf (x = SquareHistory(0, 0) AndAlso y = SquareHistory(0, 1)) OrElse (x = SquareHistory(1, 0) AndAlso y = SquareHistory(1, 1)) Then
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
        If PieceMoving.IsMovingPiece Then 'the piece is being moved by the user.
            Dim Square As New Rectangle(PieceMoving.StartPoint.X, PieceMoving.StartPoint.Y, 75, 75)
            Using Brush As New SolidBrush(Color.LightCoral)
                g.FillRectangle(Brush, Square)
            End Using
        End If
    End Sub

    'Subroutine that adjusts the values on the LegalMoveSquares array when a given move is being made
    '(for illustration purposes when the AI is deciding between moves).
    Private Sub AmmendLegalMoveSuares(ByVal TempMove As Move)
        'Resets LMS array.
        For y = 0 To 7
            For x = 0 To 7
                LegalMoveSquares(x, y) = False
            Next
        Next
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
                        CurrentPiece &= UCase(MasterBoard(x, y)) & CStr(Counter)
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
                CurrentPiece &= UCase(MasterBoard(CoorX, CoorY)) & CStr(Counter)
                'Searches for piece that has not been placed on the board (that matches CurrentPiece / TempPiece's name + location), and moves its match into place on the board.
                TempPiece = Me.Controls.Find(CurrentPiece, True).Single()
                If OrientForWhite AndAlso (CInt(TempPiece.Location.X) = CoorX * 75 AndAlso CInt(TempPiece.Location.Y) = CoorY * 75) Then
                    Exit While
                ElseIf Not OrientForWhite AndAlso (CInt(TempPiece.Location.X) = (7 - CoorX) * 75 AndAlso CInt(TempPiece.Location.Y) = (7 - CoorY) * 75) Then
                    Exit While
                End If
                'If not found, the search continues with an increasing last digit (eg: the program seaches for BP1, then BP2, then BP3 ext until it finds its match.)
                Counter += 1
            End While
        End If
        Return TempPiece
    End Function



    'Subroutine that detects some errors in a user-entered FEN string, given the below rules:
    Private Function FENErrorDetection(ByVal FEN As String, ByVal OutputToBox As Boolean, ByRef FENErrorMessage As String) As Boolean
        'If OutputToBox = True then the error message is stored in FENTextBox. If it is false then the message is stored in FENErrorMessage.
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
                    InvalidFEN = FEN
                    FENTextBox.Text = "Position Rejected - Invalid number of Kings. Please Input a Genuinine FEN and try again."
                    UndoFENChange.Visible = True
                Else 'Returns message as string.
                    FENErrorMessage = "Position Rejected - No King."
                End If
                Return False
            ElseIf MaxWQueens > 9 OrElse MaxBQueens > 9 Then
                If OutputToBox Then
                    InvalidFEN = FEN
                    FENTextBox.Text = "Position Rejected - Too many Pawns / Queens. Please Input a Genuinine FEN and try again."
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
                InvalidFEN = FEN
                FENTextBox.Text = "Position Rejected - Invalid FEN Length. Please Input a Genuinine FEN and try again."
                UndoFENChange.Visible = True
            Else
                FENErrorMessage = "Position Rejected - Invalid FEN Length."
            End If
            Return False
        End If
    End Function

    'Sends the FEN a user types in to be converted & displayed.
    Private Sub FENButton_Click() Handles FENButton.Click
        If FENTextBox.Text <> "" AndAlso FENTextBox.Text <> CurrentFEN AndAlso Not ComputerIsSearching Then
            'Checks for first form of validation (if the 2nd letter is "o", then that usually implies that 
            'the TextBox contains an error message.
            If FENTextBox.Text(1) <> "o" AndAlso FENErrorDetection(FENTextBox.Text, True, "") Then
                If UndoFENChange.Visible = True Then UndoFENChange.Visible = False
                'Resets Check and Castling Properties.
                MasterWInCheck.NotInCheck()
                MasterBInCheck.NotInCheck()
                Dim TempFEN As String = PreviousFEN
                'Try displaying the FEN on the board graphically. If this fails, then the FEN is invalid.
                'In this case, the board is reset to the previous position.
                Try
                    PreviousFEN = CurrentFEN
                    CurrentFEN = FENTextBox.Text
                    MasterBoard = SharedAlgorithms.FENConverter(FENTextBox.Text, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
                    DisplayPieces()
                Catch ex As Exception
                    InvalidFEN = FENTextBox.Text
                    CurrentFEN = PreviousFEN
                    PreviousFEN = TempFEN
                    MasterBoard = SharedAlgorithms.FENConverter(CurrentFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
                    DisplayPieces()
                    UndoFENChange.Visible = True
                    FENTextBox.Text = "Position Rejected - Invalid FEN Code. Please Input a Genuinine FEN and try again."
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
                    Sound_Move.Play()
                    CheckChecker()

                    If MasterWInCheck.IsInCheck AndAlso Not PlayerTurn Then
                        'If White is in check, and it is black to move, then black can take white's king. This is illegal.
                        GameRunning = False
                        Console.WriteLine("The Game has Ended. Cause = Invalid Check.")
                    ElseIf MasterBInCheck.IsInCheck AndAlso PlayerTurn Then
                        'If Black is in check, and it is white to move, then white can take black's king. This is illegal.
                        Console.WriteLine("The Game has Ended. Cause = Invalid Check.")
                        GameRunning = False
                    ElseIf Math.Abs(CStr(MasterWKPos(0)) - CStr(MasterBKPos(0))) <= 1 AndAlso Math.Abs(CStr(MasterWKPos(1)) - CStr(MasterBKPos(1))) <= 1 Then
                        'Kings are too close together.
                        GameRunning = False
                        Console.WriteLine("The Game has Ended. Cause = Invalid King Positions.")
                        CheckLabel.Text = " Stalemate! "
                        Sound_Stalemate.Play()
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
    End Sub

    'Button which resets the FEN in the FENTextBox, in case it is invalid.
    Private Sub UndoFENChange_Click(sender As Object, e As EventArgs) Handles UndoFENChange.Click
        FENTextBox.Text = InvalidFEN
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
            CheckChecker()
            If PlayerTurn Xor OrientForWhite Then FlipBoard()
            GameRunning = True
            If GameMode < 3 Then
                FENTextBox.Text = StartingFEN
                FENTextBox.Select(FENTextBox.Text.Length, 0) 'Stops the TextBox from being highlighted.
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
            Sound_Move.Play()
        End If
    End Sub

    'Outputs the current FEN into the FEN textbox.
    Private Sub FENExport_Click() Handles FENExport.Click
        FENTextBox.Text = CurrentFEN
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
            Sound_Move.Play()
            CheckChecker()

            If MasterWInCheck.IsInCheck AndAlso Not PlayerTurn Then
                'If White is in check, and it is black to move, then black can take white's king. This is illegal.
                GameRunning = False
                Console.WriteLine("The Game has Ended. Cause = Invalid Check.")
            ElseIf MasterBInCheck.IsInCheck AndAlso PlayerTurn Then
                'If Black is in check, and it is white to move, then white can take black's king. This is illegal.
                Console.WriteLine("The Game has Ended. Cause = Invalid Check.")
                GameRunning = False
            ElseIf Math.Abs(CStr(MasterWKPos(0)) - CStr(MasterBKPos(0))) <= 1 AndAlso Math.Abs(CStr(MasterWKPos(1)) - CStr(MasterBKPos(1))) <= 1 Then
                'Kings are too close together.
                GameRunning = False
                Console.WriteLine("The Game has Ended. Cause = Invalid King Positions.")
                CheckLabel.Text = " Stalemate! "
                Sound_Stalemate.Play()
            Else
                Sound_Move.Play()
            End If

            If GameMode < 3 Then FENTextBox.Text = CurrentFEN
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
    End Sub

    'Subroutines Controlling each Colour Scheme Option change. When each button is clicked,
    'the Primary & Secondary colours are appended, and these preferences are written to a file.
    Private Sub ColourChangerButton_Click() Handles ColourChangerButton.Click
        'Reveals drop-down menu.
        ColourChanger.Show(ColourChangerButton, 0, ColourChangerButton.Height)
    End Sub
    Private Sub DefColourChanger() Handles Def.Click
        'Sets colours and redraws the checkerboard.
        PrimaryColour = Color.Peru
        SecondaryColour = Color.FloralWhite
        Checkerboard.Refresh()
        'Writes the colour preferences to a file (creates the file if one does not exist).
        FileOpen(1, Application.StartupPath & "\ColourPreferences.txt", OpenMode.Output)
        PrintLine(1, "def")
        FileClose(1)
    End Sub
    Private Sub LBluColourChanger() Handles LBlu.Click
        PrimaryColour = Color.LightSteelBlue
        SecondaryColour = Color.AliceBlue
        Checkerboard.Refresh()
        FileOpen(1, Application.StartupPath & "\ColourPreferences.txt", OpenMode.Output)
        PrintLine(1, "blu")
        FileClose(1)
    End Sub
    Private Sub DBluColourChanger() Handles DBlu.Click
        PrimaryColour = Color.SlateGray
        SecondaryColour = Color.LightGray
        Checkerboard.Refresh()
        FileOpen(1, Application.StartupPath & "\ColourPreferences.txt", OpenMode.Output)
        PrintLine(1, "bl2")
        FileClose(1)
    End Sub
    Private Sub GldColourChanger() Handles Gld.Click
        PrimaryColour = Color.Goldenrod
        SecondaryColour = Color.LemonChiffon
        Checkerboard.Refresh()
        FileOpen(1, Application.StartupPath & "\ColourPreferences.txt", OpenMode.Output)
        PrintLine(1, "gld")
        FileClose(1)
    End Sub
    Private Sub GrnColourChanger() Handles Grn.Click
        PrimaryColour = Color.DarkSeaGreen
        SecondaryColour = Color.LavenderBlush
        Checkerboard.Refresh()
        FileOpen(1, Application.StartupPath & "\ColourPreferences.txt", OpenMode.Output)
        PrintLine(1, "grn")
        FileClose(1)
    End Sub
    Private Sub PplColourChanger() Handles Ppl.Click
        PrimaryColour = Color.MediumPurple
        SecondaryColour = Color.MistyRose
        Checkerboard.Refresh()
        FileOpen(1, Application.StartupPath & "\ColourPreferences.txt", OpenMode.Output)
        PrintLine(1, "ppl")
        FileClose(1)
    End Sub
    Private Sub MonColourChanger() Handles Mon.Click
        PrimaryColour = Color.DimGray
        SecondaryColour = Color.Silver
        Checkerboard.Refresh()
        FileOpen(1, Application.StartupPath & "\ColourPreferences.txt", OpenMode.Output)
        PrintLine(1, "mon")
        FileClose(1)
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

    'Button that returns the user back to the Main Menu. 
    Private Sub ExitBtn_Click() Handles ExitBtn.Click
        'Locates MainMenu Form and shows it.
        Dim Menu As Form = CType(Application.OpenForms("MainMenu"), MainMenu)
        Me.Close() 'Closes the current form.
        Menu.Show()
    End Sub

    'Button that displays the credits information onto the screen (in the form of a pop-up).
    Private Sub Credits_Click() Handles Credits.Click
        MsgBox(Strings.StrDup(10, " ") & "Chess Game & Artificial Intelligence (v5.2)" & vbCrLf & Strings.StrDup(21, " ") & "Created by Alfie Kunz (8158)" & vbCrLf & Strings.StrDup(22, " ") & "of Beckfoot School (37101)" & vbCrLf & "Project used for the AQA GCE Computer Science NEA" & vbCrLf & Strings.StrDup(35, " ") & "(2021 - 2023)", vbInformation + vbApplicationModal, "Credits")
    End Sub



    'The below three subroutines control the drag & drop mechanics for the pieces, and translates a user's move onto the board.
    Private Sub Piece_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles WK1.MouseDown, WQ1.MouseDown, WB1.MouseDown, WB2.MouseDown, WN1.MouseDown, WN2.MouseDown, WR1.MouseDown, WR2.MouseDown, WP1.MouseDown, WP2.MouseDown, WP3.MouseDown, WP4.MouseDown, WP5.MouseDown, WP6.MouseDown, WP7.MouseDown, WP8.MouseDown, BK1.MouseDown, BQ1.MouseDown, BB1.MouseDown, BB2.MouseDown, BN1.MouseDown, BN2.MouseDown, BR1.MouseDown, BR2.MouseDown, BP1.MouseDown, BP2.MouseDown, BP3.MouseDown, BP4.MouseDown, BP5.MouseDown, BP6.MouseDown, BP7.MouseDown, BP8.MouseDown, WQ8.MouseDown, WQ7.MouseDown, WQ6.MouseDown, WQ5.MouseDown, WQ4.MouseDown, WQ3.MouseDown, BQ6.MouseDown, BQ8.MouseDown, BQ5.MouseDown, BQ7.MouseDown, BQ2.MouseDown, BQ4.MouseDown, BQ3.MouseDown, WQ2.MouseDown, WQ9.MouseDown, BQ9.MouseDown
        'Subroutine that activates the drag + drog mechanic if the user clicks on a piece.
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso GameRunning AndAlso Not ComputerIsSearching Then
            'If the piece corresponds to the player allowed to move...
            If (CStr(sender.name)(0) = "W" AndAlso PlayerTurn) OrElse (CStr(sender.name)(0) = "B" AndAlso (Not PlayerTurn)) Then
                Me.Cursor = New Cursor(New System.IO.MemoryStream(My.Resources.Cursor16)) 'Can be Cursor32 or Cursor192
                'Generates the Legal Moves for the piece the user has picked up (using the AI).
                If OrientForWhite Then
                    PieceMoving.LegalMoves = MainAI.ReturnPiecesLegalMoves(sender.location.x / 75, sender.location.y / 75)
                    'Updates LegalMoveSquares array.
                    For n = 1 To CInt(PieceMoving.LegalMoves(0))
                        LegalMoveSquares(CStr(PieceMoving.LegalMoves(n)(0)), CStr(PieceMoving.LegalMoves(n)(1))) = True
                    Next
                Else
                    PieceMoving.LegalMoves = MainAI.ReturnPiecesLegalMoves(7 - (sender.location.x / 75), 7 - (sender.location.y / 75))
                    'Updates LegalMoveSquares array.
                    For n = 1 To CInt(PieceMoving.LegalMoves(0))
                        LegalMoveSquares(7 - CStr(PieceMoving.LegalMoves(n)(0)), 7 - CStr(PieceMoving.LegalMoves(n)(1))) = True
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
            For y = 0 To 7
                For x = 0 To 7
                    LegalMoveSquares(x, y) = False
                Next
            Next
            Checkerboard.Refresh()
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
                For n = 1 To CInt(PieceMoving.LegalMoves(0))
                    If CStr(TempPoint.X & TempPoint.Y) = PieceMoving.LegalMoves(n) Then
                        IsLegalMove = True
                        Exit For
                    End If
                Next

                If IsLegalMove Then
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
                            Sound_Castle.Play()
                            'Adjusts rook position.
                            If OrientForWhite Then
                                CoorToPieceConverter(7, TempPoint.Y).Left -= 150
                            Else
                                CoorToPieceConverter(7, TempPoint.Y).Left += 150
                            End If
                            MasterBoard(5, TempPoint.Y) = CStr(MasterBoard(7, TempPoint.Y))
                            MasterBoard(7, TempPoint.Y) = " "
                        ElseIf (TempPoint.X & TempPoint.Y) = ((PieceMoving.StartPoint.X / 75) - 2 & PieceMoving.StartPoint.Y / 75) Then
                            'Queenside castling.
                            JustCastled = True
                            Sound_Castle.Play()
                            'Adjusts rook position.
                            If OrientForWhite Then
                                CoorToPieceConverter(0, TempPoint.Y).Left += 225
                            Else
                                CoorToPieceConverter(0, TempPoint.Y).Left -= 225
                            End If
                            MasterBoard(3, TempPoint.Y) = CStr(MasterBoard(0, TempPoint.Y))
                            MasterBoard(0, TempPoint.Y) = " "
                        End If
                    End If

                    'If the move is a capture, the required sound is played and the captured piece is removed from the board.
                    If MasterBoard(TempPoint.X, TempPoint.Y) <> " " Then
                        Sound_Capture.Play()
                        ReplacedPiece = CoorToPieceConverter(TempPoint.X, TempPoint.Y) 'Finds captured PictureBox.
                        ReplacedPiece.Visible = False
                        ReplacedPiece.Location = New Point(-100, -100)
                    ElseIf JustCastled = False Then
                        Sound_Move.Play()
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
                    If CStr(sender.name)(0) = "W" Then
                        MasterBoard(TempPoint.X, TempPoint.Y) = UCase((CStr(sender.name))(1))
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
                                Sound_Capture.Play()
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
                        MasterBoard(TempPoint.X, TempPoint.Y) = LCase((CStr(sender.name))(1))
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
                                Sound_Capture.Play()
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
                    CheckChecker()

                    'Starts the next turn by updating the Current FEN and checking for end positions.
                    PlayerTurn = Not PlayerTurn
                    PreviousFEN = CurrentFEN
                    CurrentFEN = SharedAlgorithms.ConvertToFEN(MasterBoard, MasterWCanCastle, MasterBCanCastle, MasterEnPassant, PlayerTurn)
                    MainAI.Reconfigure(CurrentFEN) 'Recalibrates AI before checking for end states.
                    EnforceEndStates()
                    If CurrentFEN = PreviousFEN Then Exit Sub 'Stops AI from running if it is the start position.
                    OutputDebugInfo()
                    If GameMode < 3 Then
                        FENTextBox.Text = CurrentFEN
                        If GameMode = 1 Then InstantiateAIs()
                    End If

                Else 'Resets piece to previous position
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
    Private Sub CheckChecker()
        If MasterWInCheck.IsInCheck OrElse MasterBInCheck.IsInCheck Then
            Sound_Check.Play()
            CheckLabel.Text = "    Check!    "
        Else
            CheckLabel.Text = "                    "
        End If
        'Modifies king picture accordingly.
        If MasterWInCheck.IsInCheck Then
            WK1.Image = Image.FromFile(Application.StartupPath & "\Images\Default\WKingCheck.png")
        Else
            WK1.Image = Image.FromFile(Application.StartupPath & "\Images\Default\WKing.png")
        End If
        If MasterBInCheck.IsInCheck Then
            BK1.Image = Image.FromFile(Application.StartupPath & "\Images\Default\BKingCheck.png")
        Else
            BK1.Image = Image.FromFile(Application.StartupPath & "\Images\Default\BKing.png")
        End If
    End Sub



    'Algorithm which calculates the Overall Depth the AI will run at. Takes into account the Total Material Count
    'on the board and calculates Depth using the formula y = -m*ln(x-a) + c (the higher the Material Count, the lower
    'the Depth).
    Private Sub CalculateAbsoluteDepth()
        'Counts the material on the board.
        Dim TotalMaterial As SByte() = SharedAlgorithms.CountMaterial(MasterBoard)
        'TotalMaterial(0) = White's total material, TotalMaterial(1) = Black's total material.
        'If the user is using Quiescence, then we knock off 0.5 from the depth (as Quiescence is slightly slower).
        Dim DepthAlgorithm As Byte = Math.Truncate(-2 * Math.Log(TotalMaterial(0) + TotalMaterial(1) + 1, 4) + 10 + (0.5 * CInt(UseQuiescence)))

        'If the previous AI search resulted in a forced Checkmate being found, the depth is limited to only the
        'depth that is required to achieve that Checkmate. This saves on a lot of unnecessary processing, as
        'forced Checkmates are unavoidable.
        If CurrentEvaluation <> "-" AndAlso Math.Abs(CInt(CurrentEvaluation)) > 900 Then
            AbsoluteDepth = CurrentDepth - (Math.Abs(CInt(CurrentEvaluation)) - 999)
        Else
            AbsoluteDepth = DepthAlgorithm
        End If
        'The Math.Max function is used to ensure that any AI's Depth never gets lower than 2. Along with general quality
        'improvements (as a depth of 2 can be completed instantly), this also prevents the depth from reaching 1, 0, or
        'negative - all of which would cause errors / invalid results when searching.
        AbsoluteDepth = Math.Max(AbsoluteDepth, 4)
        Console.WriteLine("Absolute Depth = " & AbsoluteDepth)
    End Sub


    'Algorithm that creates all the threads & AIs that will search on a position. Handles time management + overtime,
    'interactions with the GUI chessboard, live updates via GUI objects, selecting best moves, and more.
    Private Sub InstantiateAIs() Handles AIMoveBtn.Click
        If GameRunning AndAlso Not ComputerIsSearching Then
            Dim IsLate As Boolean 'Represents whether the computer has used more time than it was originally allocated.
            Dim BestMove As Move = MainAI.CheckForEndState() 'Ensures that the position is valid, and that the
            'AIs won't crash whilst searching on the position.
            If BestMove.EndState = "f" OrElse BestMove.EndState = "o" Then
                If BestMove.EndState = "f" Then 'no forced move found.
                    'Instantiations of threads (in a List of tasks format).
                    Dim Tasks As New List(Of Task) With {
                        .Capacity = 6
                    }
                    Tasks.Add(New Task(AddressOf InitialiseThread1))
                    Tasks.Add(New Task(AddressOf InitialiseThread2))
                    Tasks.Add(New Task(AddressOf InitialiseThread3))
                    Tasks.Add(New Task(AddressOf InitialiseThread4))
                    Tasks.Add(New Task(AddressOf InitialiseThread5))

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
                            If CurrentEvaluation <> "-" AndAlso Math.Abs(CInt(CurrentEvaluation)) > 900 Then
                                'Mating pattern has been found.
                                CurrentAIEval.Text = "Evaluation: Mate in " & Math.Truncate((CurrentDepth - (Math.Abs(CInt(CurrentEvaluation)) - 1000)) / 2)
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
                    If CurrentEvaluation <> "-" AndAlso Math.Abs(CInt(CurrentEvaluation)) > 900 Then
                        'Mating pattern has been found - update GUI accordingly
                        CurrentAIEval.Text = "Evaluation: Mate in " & Math.Truncate((CurrentDepth - (Math.Abs(CInt(CurrentEvaluation)) - 1000)) / 2)
                    Else
                        CurrentAIEval.Text = "Evaluation: " & CurrentEvaluation
                    End If

                Else 'Move was forced - make move instantly without searching.
                    Console.WriteLine(vbCrLf & "Search Aborted - Only 1 Move in Position.")
                    CurrentAIDepth.Text = "Current Depth: 1"
                    CurrentAIMove.Text = "Current Move: " & SharedAlgorithms.MoveConverter(MasterBoard, BestMove, MasterEnPassant) & " (FORCED)."
                End If

                If Me.IsHandleCreated Then 'If form is still open...
                    GameRunning = True
                    'Makes the best move on the board, and updates the correct TFTable.
                    If PlayerTurn Then
                        MasterWInCheck.NotInCheck() 'Player is no longer in check.
                        MakeMove(MasterBoard, BestMove.OldMoveX, BestMove.OldMoveY, BestMove.NewMoveX, BestMove.NewMoveY, MasterWCanCastle, MasterWKPos, MasterEnPassant, True)
                        SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
                    Else
                        MasterBInCheck.NotInCheck() 'Player is no longer in check.
                        MakeMove(MasterBoard, BestMove.OldMoveX, BestMove.OldMoveY, BestMove.NewMoveX, BestMove.NewMoveY, MasterBCanCastle, MasterBKPos, MasterEnPassant, True)
                        SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
                    End If
                    DisplayPieces()

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
                    For y = 0 To 7
                        For x = 0 To 7
                            LegalMoveSquares(x, y) = False
                        Next
                    Next
                    If AutoFlipper.Checked Then
                        FlipBoard()
                    Else
                        Checkerboard.Refresh()
                    End If
                    CheckChecker()

                    'Ends the turn, then calculates the new position's FEN.
                    PlayerTurn = Not PlayerTurn
                    If GameMode = 3 Then
                        PreviousFEN = CurrentFEN
                    Else
                        FENTextBox.Text = SharedAlgorithms.ConvertToFEN(MasterBoard, MasterWCanCastle, MasterBCanCastle, MasterEnPassant, PlayerTurn)
                    End If
                    CurrentFEN = SharedAlgorithms.ConvertToFEN(MasterBoard, MasterWCanCastle, MasterBCanCastle, MasterEnPassant, PlayerTurn)
                    MainAI.Reconfigure(CurrentFEN) 'Recalibrates AI.

                    'Resets GUI objects & cursor design.
                    Me.Cursor = Cursors.Default
                    ProgressBar.ForeColor = Color.FromArgb(0, 192, 0)
                    ProgressBar.Value = 0
                    UserTimeBar.Enabled = True
                    QuiescenceBox.Enabled = True
                    ComputerIsSearching = False
                    EnforceEndStates() 'Checks for end states found from the AI's move.
                    OutputDebugInfo()
                    'The below lines fix a bug, where the AI wouldn't make a move after the user restarts their game (1P mode only).
                    If AIEndlessMode.Checked OrElse (GameMode = 1 AndAlso CurrentFEN = PreviousFEN AndAlso Not UserPlayer = PlayerTurn) Then
                        Thread.Sleep(5) 'Allows the system to recalibrate (to prevent spam).
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
        AIBestMoves(4) = AI5.Search(AbsoluteDepth + 2, UseQuiescence)
        If Not AI5.ABORT Then
            Console.WriteLine("Depth Of " & AbsoluteDepth + 2 & " Completed in: " & AIStopwatch.ElapsedMilliseconds & " Milliseconds." & vbCrLf)
            AmmendLegalMoveSuares(AIBestMoves(4))
            AIFinishedSearch = True
            CurrentDepth = AbsoluteDepth + 2
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
                Sound_Checkmate.Play()
                GameRunning = False
                Console.WriteLine("The Game has Ended. Cause = Checkmate.")
                If GameMode < 3 Then NotifyGameEnd("c")
                Exit Sub
            Case "s" 'Position is stalemate.
                CheckLabel.Text = " Stalemate! "
                Sound_Stalemate.Play()
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
            Sound_Stalemate.Play()
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
                        Sound_Stalemate.Play()
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
                                    Sound_Stalemate.Play()
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
