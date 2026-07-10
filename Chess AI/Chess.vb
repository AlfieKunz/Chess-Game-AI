Imports System.Threading
Public Class Chess 'i call thy Bryson because you are very bad.

    'Working Version of Chess, along with AI using MiniMax and Alpha Beta Pruning. All created by Alfie Kunz.

    'Here are the computer's primary "eyes" - MasterBoard (which contains each piece's location on the board) and,
    'derived from this, Master TrueFalse Tables for each player. These Tables display information such as Legal
    'Moves for the players' kings, along with pinned pieces and their location, and make handling Move Generation,
    'Checks and Evaluations much easier and more efficient.
    Protected GameRunning As Boolean = True
    Protected MasterBoard(7, 7) As Char
    Protected MasterWhiteTFTable(7, 7) As Char
    Protected MasterBlackTFTable(7, 7) As Char

    'TrueTables are just TrueFalse Tables containing only the letter T. Very useful for resetting TrueFalse Tables
    'and debugging.
    Protected MasterTrueTable(7, 7) As Char
    Protected TrueTable(7, 7) As Char
    ReadOnly PieceArray(45) As PictureBox
    'Standard Chess notation that displays where all the pieces on the board are supposed to go.
    ReadOnly StartingFEN As String = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
    Protected CurrentFEN As String = StartingFEN
    Protected PreviousFEN As String = StartingFEN
    Protected InvalidFEN As String
    Protected PlayerTurn As Boolean
    Protected MasterEnPassant As String
    Protected MasterWKPos As String
    Protected MasterBKPos As String
    Protected MasterMaterialCount As Int16
    Protected AbsoluteDepth As SByte
    Protected PositionsSearched As Integer = 0
    Protected ComputerIsSearching As Boolean = False
    Protected Progress As Byte

    Private MovingPiece As Boolean = False
    Private StartPoint As Point
    Private MidPoint As Point
    Private EndPoint As Point
    Private LegalMoves As String()


    Protected PieceValue(9) As Decimal 'Array Containing the Value or Weight of each Piece.

    'Structures containing information about castling + checks.
    Structure CanCastle
        Dim KS As Boolean 'King Side Castling
        Dim QS As Boolean 'Queen Side Castling
    End Structure
    Protected MasterWCanCastle As CanCastle
    Protected MasterBCanCastle As CanCastle
    Protected CannotCastle As CanCastle
    Structure InCheck
        Dim IsInCheck As Boolean
        Dim Piece1 As String
        Dim DoubleCheck As Boolean
    End Structure
    Protected MasterWInCheck As InCheck
    Protected MasterBInCheck As InCheck
    Protected NotInCheck As InCheck
    Structure NewMove
        Dim Score As Decimal
        Dim OldMoveX As String
        Dim OldMoveY As String
        Dim NewMoveX As String
        Dim NewMoveY As String
    End Structure

    'Sets up sound effects.
    ReadOnly Sound_Startup As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Chess_Startup.wav"
    }
    ReadOnly Sound_Move As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Chess_Move.wav"
    }
    ReadOnly Sound_Capture As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Chess_Capture.wav"
    }
    ReadOnly Sound_Check As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Chess_Check.wav"
    }
    ReadOnly Sound_Castle As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Chess_Castle.wav"
    }
    ReadOnly Sound_Checkmate As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Chess_Checkmate.wav"
    }
    ReadOnly Sound_Stalemate As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Chess_Stalemate.wav"
    }

    Protected Stopwatch As New Stopwatch





    Public Sub Form1_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Application.EnableVisualStyles()
        'Assigns each piece to PieceArray and sets the 'board' picturebox as their parent.
        Stopwatch.Start()
        'Checkerboard.Location = New Point(0, 0)
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
        PieceArray(39) = BQ2
        PieceArray(40) = BQ3
        PieceArray(41) = BQ4
        PieceArray(42) = BQ5
        PieceArray(43) = BQ6
        PieceArray(44) = BQ7
        PieceArray(45) = BQ8

        For x = 0 To 45
            Checkerboard.Controls.Add(PieceArray(x))
        Next
        UndoFENChange.Visible = False
        'Sends the starting FEN to be converted and displayed on the screen, then resets check / castling properties.
        MasterBoard = FENConverter(StartingFEN)
        CannotCastle.KS = False
        CannotCastle.QS = False
        NotInCheck.IsInCheck = False
        NotInCheck.Piece1 = " "
        NotInCheck.DoubleCheck = False
        MasterWInCheck.Piece1 = " "
        MasterWInCheck.DoubleCheck = False
        MasterBInCheck.Piece1 = " "
        MasterBInCheck.DoubleCheck = False

        'Sets PieceValues variables using a Hash Function (Upper Case letter --> ASCII, then MOD 11). This
        'creates() a unique index / row in the PieceValue array for each piece and its corresponding weight,
        'so its value can be searched up quickly. PieceValue(Asc(UCase(Board(x, y))) Mod 11)
        PieceValue(0) = 3 'Bishop Weight
        PieceValue(1) = 3 'Knight Weight
        PieceValue(3) = 1 'Pawn Weight
        PieceValue(4) = 9 'Queen Weight
        PieceValue(5) = 5 'Rook Weight
        PieceValue(9) = 0 'King Weight

        'Creates TrueTables, then displays the pieces on the board.
        For x = 0 To 7
            For y = 0 To 7
                MasterTrueTable(x, y) = "T"
            Next
        Next
        Array.Copy(MasterTrueTable, TrueTable, 64)
        FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
        Array.Copy(MasterTrueTable, MasterBlackTFTable, 64)
        CalculateAbsoluteDepth()
        MasterMaterialCount = CountMaterial(MasterBoard, False)
        CurrentEval.Text = MasterMaterialCount
        CheckLabel.Text = "                    "
        DisplayPieces()
        Sound_Startup.Play()

        Stopwatch.Stop()
        Console.WriteLine("Startup Time: " & Stopwatch.Elapsed.TotalMilliseconds & " Milliseconds." & vbCrLf)
        Stopwatch.Reset()
    End Sub

    Private Sub CreateBoard(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Checkerboard.Paint
        Dim g As Graphics = e.Graphics
        Dim isLight As Boolean = True
        'Creates checkerboard pattern. (Alternates between light and dark colours.)
        For x = 0 To 7
            For y = 0 To 7
                Dim square As New Rectangle(0 + (75 * x), ((75 * y)), 75, 75) '+300
                If isLight Then
                    Using Brush1 As New SolidBrush(Color.FloralWhite)
                        g.FillRectangle(Brush1, square)
                    End Using
                Else
                    Using Brush2 As New SolidBrush(Color.Peru)
                        g.FillRectangle(Brush2, square)
                    End Using
                End If
                isLight = Not isLight
            Next
            isLight = Not isLight
        Next
    End Sub

    Private Sub DisplayPieces()
        Dim CurrentPiece As String
        Dim TempPiece As PictureBox
        Dim n As Byte
        'Hides all pieces.
        For x = 0 To 45
            PieceArray(x).Location = New Point(-100, -100)
            PieceArray(x).Visible = False
        Next
        For y = 7 To 0 Step -1
            For x = 0 To 7
                If MasterBoard(x, y) <> " " Then
                    n = 1
                    While True
                        'In MasterBoard(), a lowercase letter represents a black piece, and an uppercase letter represents a white piece.
                        If MasterBoard(x, y) = LCase(MasterBoard(x, y)) Then
                            CurrentPiece = "B"
                        Else
                            CurrentPiece = "W"
                        End If
                        'CurrentPiece is created, which has the same name as a corresponding piece on the board.
                        CurrentPiece &= UCase(MasterBoard(x, y)) & CStr(n)
                        'Searches for piece that has not been placed on the board (that matches CurrentPiece / TempPiece's name), and moves its match into place on the board.
                        TempPiece = Me.Controls.Find(CurrentPiece, True).Single()
                        If TempPiece.Visible = False Then
                            TempPiece.Location = New Point(((x) * 75), ((75 * y)))
                            TempPiece.Visible = True
                            Exit While
                        End If
                        'If not found, the search continues with an increasing last digit (eg: the program seaches for BP1, then BP2, then BP3 ext until it finds its match.)
                        n += 1
                    End While
                End If
            Next
        Next
        'Console.WriteLine(PlayerTurn)
    End Sub


    'Alternate version of DisplayPieces() which is more efficient and only converts a single piece in
    'MasterBoard() to its piece counterpart on the board.
    Private Function MarrToPieceConv(ByVal CoorX As Int16, ByVal CoorY As Int16, ByVal PieceColour As Char) As PictureBox
        Dim CurrentPiece As String
        Dim TempPiece As PictureBox
        Dim n As Byte
        If MasterBoard(CoorX / 75, CoorY / 75) <> " " Then
            n = 1
            While True
                'In MasterBoard(), a lowercase letter represents a black piece, and an uppercase letter represents a white piece.
                If MasterBoard(CoorX / 75, CoorY / 75) = LCase(MasterBoard(CoorX / 75, CoorY / 75)) Then
                    CurrentPiece = "B"
                Else
                    CurrentPiece = "W"
                End If
                'CurrentPiece is created, which has the same name as a corresponding piece on the board.
                CurrentPiece &= UCase(MasterBoard(CoorX / 75, CoorY / 75)) & CStr(n)
                'Searches for piece that has not been placed on the board (that matches CurrentPiece / TempPiece's name + location), and moves its match into place on the board.
                TempPiece = Me.Controls.Find(CurrentPiece, True).Single()
                If CInt(TempPiece.Location.X) = CoorX AndAlso CInt(TempPiece.Location.Y) = CoorY AndAlso TempPiece.Name(0) <> PieceColour Then
                    Exit While
                End If
                'If not found, the search continues with an increasing last digit (eg: the program seaches for BP1, then BP2, then BP3 ext until it finds its match.)
                n += 1
            End While
        End If
        '#Disable Warning BC42104 'Variable is used before it has been assigned a value
        Return TempPiece
        '#Enable Warning BC42104 'Variable is used before it has been assigned a value
    End Function


    'Function which puts the required pieces in a FEN board position into an 8x8 board array.
    Private Function FENConverter(ByVal FEN As String) As Char(,)
        MasterWCanCastle.KS = False
        MasterWCanCastle.QS = False
        MasterBCanCastle.KS = False
        MasterBCanCastle.QS = False
        MasterEnPassant = "-"
        Dim x As Byte = 0
        Dim y As Byte = 0
        Dim tempArray(7, 7) As Char
        Dim SpaceLocation As Byte
        For n = 0 To Len(FEN) - 1
            Select Case FEN(n)
                Case "/"
                    y += 1
                    x = 0
                Case "A" To "Z", "a" To "z"
                    tempArray(x, y) = FEN(n)
                    x += 1
                    If FEN(n) = "K" Then
                        MasterWKPos = (x - 1) & y
                    ElseIf FEN(n) = "k" Then
                        MasterBKPos = (x - 1) & y
                    End If
                Case "0" To "8"
                    For m = 1 To CInt(CStr((FEN(n))))
                        tempArray(x, y) = " "
                        x += 1
                    Next
                Case " "
                    'Characters after the SPACE contain info about castling, who's turn it is, En Passant and more.
                    SpaceLocation = FEN.IndexOf(" ")
                    If FEN(SpaceLocation + 1) = "w" Then
                        PlayerTurn = True
                    Else
                        PlayerTurn = False
                    End If
                    FEN = Microsoft.VisualBasic.Right(FEN, Len(FEN) - SpaceLocation - 3)
                    For m = 0 To Len(FEN) - 1
                        Select Case FEN(m)
                            Case "K"
                                MasterWCanCastle.KS = True
                            Case "Q"
                                MasterWCanCastle.QS = True
                            Case "k"
                                MasterBCanCastle.KS = True
                            Case "q"
                                MasterBCanCastle.QS = True
                            Case "a" To "h"
                                MasterEnPassant = Asc(FEN(m)) - 97 & 8 - CStr(FEN(m + 1))
                        End Select
                    Next
                    Exit For
            End Select
        Next
        Return tempArray
    End Function


    'Function which converts the current board position into its FEN counterpart.
    Private Sub ConvertToFEN()
        Dim Counter As Byte = 0
        CurrentFEN = ""
        For y = 0 To 7
            For x = 0 To 7
                If MasterBoard(x, y) = " " Then
                    Counter += 1
                Else
                    If Counter > 0 Then
                        CurrentFEN &= Counter
                        Counter = 0
                    End If
                    CurrentFEN &= MasterBoard(x, y)
                End If
            Next
            If Counter > 0 Then CurrentFEN &= Counter
            CurrentFEN &= "/"
            Counter = 0
        Next
        CurrentFEN = CurrentFEN.TrimEnd("/")
        If PlayerTurn Then
            CurrentFEN &= " w "
        Else
            CurrentFEN &= " b "
        End If
        If MasterWCanCastle.KS Then CurrentFEN &= "K"
        If MasterWCanCastle.QS Then CurrentFEN &= "Q"
        If MasterBCanCastle.KS Then CurrentFEN &= "k"
        If MasterBCanCastle.QS Then CurrentFEN &= "q"
        If CurrentFEN.EndsWith(" ") Then CurrentFEN &= "-"
        If MasterEnPassant <> "-" Then
            CurrentFEN &= " " & Chr(CStr(MasterEnPassant(0)) + 97) & 8 - CStr(MasterEnPassant(1)) & " 0 1"
        Else
            CurrentFEN &= " - 0 1"
        End If
    End Sub



    'Function which counts up all the material (and their values) on the board and subtracts black's total from
    'white's total. Rough metric as to who is winning.
    Private Function CountMaterial(ByRef Board(,) As Char, ByRef TotalMaterial As Boolean) As Int16
        Dim TempPiece As Char
        Dim WhiteMaterial As Int16
        Dim BlackMaterial As Int16
        CountMaterial = 0
        For y = 0 To 7
            For x = 0 To 7
                If Board(x, y) <> " " Then
                    TempPiece = Board(x, y)
                    If Char.IsUpper(Board(x, y)) Then
                        WhiteMaterial += PieceValue(Asc(UCase(Board(x, y))) Mod 11)
                    Else
                        BlackMaterial += PieceValue(Asc(UCase(Board(x, y))) Mod 11)
                    End If
                End If
            Next
        Next
        If Not TotalMaterial Then
            CountMaterial = WhiteMaterial - BlackMaterial
        Else
            CountMaterial = WhiteMaterial + BlackMaterial
        End If
    End Function


    'Sends the FEN a user types in to be converted & displayed.
    Private Sub FENButton_Click() Handles FENButton.Click
        'Resets Check and Castling Properties.
        If FENTextBox.Text <> "" AndAlso FENTextBox.Text <> CurrentFEN AndAlso Not ComputerIsSearching Then
            If UndoFENChange.Visible = True Then UndoFENChange.Visible = False
            Console.Clear()
            MasterWInCheck.IsInCheck = False
            MasterWInCheck.Piece1 = " "
            MasterWInCheck.DoubleCheck = False
            MasterBInCheck.IsInCheck = False
            MasterBInCheck.Piece1 = " "
            MasterBInCheck.DoubleCheck = False
            Dim TempFEN As String = PreviousFEN
            'If the FEN is invalid, then the board is reset to the previous position.
            Try
                PreviousFEN = CurrentFEN
                CurrentFEN = FENTextBox.Text
                MasterBoard = FENConverter(FENTextBox.Text)
                DisplayPieces()
            Catch
                CurrentFEN = PreviousFEN
                PreviousFEN = TempFEN
                MasterBoard = FENConverter(CurrentFEN)
                DisplayPieces()
                If FENTextBox.Text <> "Position Rejected - Invalid FEN Code. Please Input a Genuinine FEN and try again." Then InvalidFEN = FENTextBox.Text
                UndoFENChange.Visible = True
                FENTextBox.Text = "Position Rejected - Invalid FEN Code. Please Input a Genuinine FEN and try again."
            End Try
            GameRunning = True
            'Resets TrueFalse Tables, then checks for Checks.
            FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
            FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
            CheckChecker()
            If MasterWInCheck.IsInCheck Then
                If Not PlayerTurn Then GameRunning = False
                Sound_Check.Play()
            ElseIf MasterBInCheck.IsInCheck Then
                If PlayerTurn Then GameRunning = False
                Sound_Check.Play()
            Else
                Sound_Move.Play()
            End If
            MasterMaterialCount = CountMaterial(MasterBoard, True)
            If MasterMaterialCount = 0 Then
                CheckLabel.Text = "     Draw!     "
                Sound_Stalemate.Play()
                GameRunning = False
            ElseIf MasterMaterialCount = 3 Then
                For y = 0 To 7
                    For x = 0 To 7
                        If UCase(MasterBoard(x, y)) = "B" OrElse UCase(MasterBoard(x, y)) = "N" Then
                            CheckLabel.Text = "     Draw!     "
                            Sound_Stalemate.Play()
                            GameRunning = False
                            Exit For
                        End If
                    Next
                    If Not GameRunning Then Exit For
                Next
            End If
            MasterMaterialCount = CountMaterial(MasterBoard, False)
            CurrentEval.Text = MasterMaterialCount


            AbsoluteDepth = 0
            If PlayerTurn Then
                WhiteAIMove_Click()
            Else
                BlackAIMove_Click()
            End If
            CalculateAbsoluteDepth()
        End If
    End Sub

    'Button which resets the FEN in case it is invalid.
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
            MasterWInCheck.IsInCheck = False
            MasterWInCheck.Piece1 = " "
            MasterWInCheck.DoubleCheck = False
            MasterBInCheck.IsInCheck = False
            MasterBInCheck.Piece1 = " "
            MasterBInCheck.DoubleCheck = False
            MasterBoard = FENConverter(CurrentFEN)
            DisplayPieces()
            'Resets TrueFalse Table, then check for Checks.
            FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
            CheckChecker()
            MasterMaterialCount = 0
            CurrentEval.Text = MasterMaterialCount
            GameRunning = True
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
        If Not ComputerIsSearching Then
            Dim TempFEN As String
            TempFEN = CurrentFEN
            CurrentFEN = PreviousFEN
            PreviousFEN = TempFEN
            MasterWInCheck.IsInCheck = False
            MasterWInCheck.Piece1 = " "
            MasterWInCheck.DoubleCheck = False
            MasterBInCheck.IsInCheck = False
            MasterBInCheck.Piece1 = " "
            MasterBInCheck.DoubleCheck = False
            MasterBoard = FENConverter(CurrentFEN)
            DisplayPieces()
            GameRunning = True
            'Resets TrueFalse Tables, then checks for Checks.
            FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
            FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
            Sound_Move.Play()
            CheckChecker()
            If MasterWInCheck.IsInCheck Then
                If Not PlayerTurn Then GameRunning = False
                Sound_Check.Play()
            ElseIf MasterBInCheck.IsInCheck Then
                If PlayerTurn Then GameRunning = False
                Sound_Check.Play()
            Else
                Sound_Move.Play()
            End If
            MasterMaterialCount = CountMaterial(MasterBoard, True)
            If MasterMaterialCount = 0 Then
                CheckLabel.Text = "     Draw!     "
                Sound_Stalemate.Play()
                GameRunning = False
            ElseIf MasterMaterialCount = 3 Then
                For y = 0 To 7
                    For x = 0 To 7
                        If UCase(MasterBoard(x, y)) = "B" OrElse UCase(MasterBoard(x, y)) = "N" Then
                            CheckLabel.Text = "     Draw!     "
                            Sound_Stalemate.Play()
                            GameRunning = False
                            Exit For
                        End If
                    Next
                    If Not GameRunning Then Exit For
                Next
            End If
            MasterMaterialCount = CountMaterial(MasterBoard, False)
            CurrentEval.Text = MasterMaterialCount

            AbsoluteDepth = 0
            If PlayerTurn Then
                WhiteAIMove_Click()
            Else
                BlackAIMove_Click()
            End If
            CalculateAbsoluteDepth()
        End If
    End Sub





    'The next 2 functions (WhitePieceLegalMoves and BlackPieceLegalMoves) receive a single piece on the board,
    'and determines all its posible moves - assigning them to the array LegalMoveArray. Alongside this, it also
    'builds to one of the two TrueFalse Tables using the squares which a piece attacks as markers. In this table,
    'the program checks for pinned pieces using the Enemy King's Position - extending a piece's line of sight if
    'the King is on the same axis as it. If it finds a King, then the Pinned Piece is labelled from 0-3, which
    'represent which direction it is pinned from (0 = vertical, 1 = btm left to top right diagonal, 2 = horizontal,
    '3 = top left to btm right diagonal). This is useful as a pinned pawn pinned vertically can still move
    'upwardly, but cannot take an enemy piece diagonally (which would expose the king), so I have also implemented
    'the logic for pieces moving depending on if / where they are pinned from. I am using the System.ValueTuple
    'Package to return 2 arrays (LegalMoveArray & ...TrueTableResult from each function.
    Function WhitePieceLegalMoves(ByRef Board(,) As Char, ByVal CoorX As SByte, ByVal CoorY As SByte, ByRef WhiteTFTable(,) As Char, ByRef BlackTFTable(,) As Char, ByRef BKPos As String, ByRef BInCheck As InCheck, ByRef WInCheck As InCheck, ByRef WCanCastle As CanCastle, ByRef EnPassant As String) As String()
        Dim LegalMoveArray(27) As String
        Dim n As Byte = 1
        Dim StartY As SByte
        Dim EndY As SByte
        Dim StartX As SByte
        Dim EndX As SByte
        Dim CheckingPiece As String = " "

        'If the King is in a Double Check situation, then only King Moves will be accepted.
        'This makes checks much more efficient.
        If WInCheck.DoubleCheck AndAlso Board(CoorX, CoorY) <> "K" Then
            Return LegalMoveArray
            Exit Function
        End If
        If Board(CoorX, CoorY) = "K" Then 'Legal Moves for the King (along with Castling).
            StartX = Math.Max(CoorX - 1, 0)
            EndX = Math.Min(CoorX + 1, 7)
            StartY = Math.Max(CoorY - 1, 0)
            EndY = Math.Min(CoorY + 1, 7)
            For y = StartY To EndY
                For x = StartX To EndX
                    BlackTFTable(x, y) = "F"
                    If Not Char.IsUpper(Board(x, y)) AndAlso WhiteTFTable(x, y) = "T" Then
                        LegalMoveArray(n) = (x) & (y)
                        n += 1
                    End If
                Next
            Next
            If WCanCastle.KS AndAlso WInCheck.IsInCheck = False Then
                If Board(CoorX + 1, CoorY) = " " AndAlso Board(CoorX + 2, CoorY) = " " AndAlso Board(CoorX + 3, CoorY) = "R" AndAlso WhiteTFTable(CoorX + 1, CoorY) = "T" AndAlso WhiteTFTable(CoorX + 2, CoorY) = "T" Then
                    LegalMoveArray(n) = (CoorX + 2) & (CoorY)
                    n += 1
                End If
            End If
            If WCanCastle.QS AndAlso WInCheck.IsInCheck = False Then
                If Board(CoorX - 1, CoorY) = " " AndAlso Board(CoorX - 2, CoorY) = " " AndAlso Board(CoorX - 3, CoorY) = " " AndAlso Board(CoorX - 4, CoorY) = "R" AndAlso WhiteTFTable(CoorX - 1, CoorY) = "T" AndAlso WhiteTFTable(CoorX - 2, CoorY) = "T" Then
                    LegalMoveArray(n) = (CoorX - 2) & (CoorY)
                    n += 1
                End If
            End If


        ElseIf Board(CoorX, CoorY) = "P" Then 'Legal Moves for the Pawn (along with First Moves).
            If WhiteTFTable(CoorX, CoorY) <> "2" Then
                If WhiteTFTable(CoorX, CoorY) <> "1" AndAlso WhiteTFTable(CoorX, CoorY) <> "3" Then
                    If Board(CoorX, CoorY - 1) = " " Then
                        LegalMoveArray(n) = CoorX & CoorY - 1
                        n += 1
                        If CoorY = 6 AndAlso Board(CoorX, 4) = " " Then
                            LegalMoveArray(n) = CoorX & 4
                            n += 1
                        End If
                    End If
                End If
                If WhiteTFTable(CoorX, CoorY) <> "0" Then
                    If WhiteTFTable(CoorX, CoorY) <> "1" AndAlso CoorX > 0 Then
                        If BlackTFTable(CoorX - 1, CoorY - 1) = "T" Then BlackTFTable(CoorX - 1, CoorY - 1) = "F"
                        If Char.IsLower(Board(CoorX - 1, CoorY - 1)) OrElse (CoorX - 1 & CoorY - 1 = EnPassant AndAlso WhiteTFTable(CoorX, CoorY) <> "4") Then
                            LegalMoveArray(n) = CoorX - 1 & CoorY - 1
                            If Board(CoorX - 1, CoorY - 1) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                            End If
                            n += 1
                        End If
                    End If
                    If WhiteTFTable(CoorX, CoorY) <> "3" AndAlso CoorX < 7 Then
                        If BlackTFTable(CoorX + 1, CoorY - 1) = "T" Then BlackTFTable(CoorX + 1, CoorY - 1) = "F"
                        If Char.IsLower(Board(CoorX + 1, CoorY - 1)) OrElse (CoorX + 1 & CoorY - 1 = EnPassant AndAlso WhiteTFTable(CoorX, CoorY) <> "4") Then
                            LegalMoveArray(n) = CoorX + 1 & CoorY - 1
                            If Board(CoorX + 1, CoorY - 1) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                            End If
                            n += 1
                        End If
                    End If
                End If
            End If


        ElseIf Board(CoorX, CoorY) = "N" Then 'Legal Moves for the Knight. (If pinned, then it cannot move at all.)
            If WhiteTFTable(CoorX, CoorY) >= "5" Then
                If CoorX >= 2 Then
                    For x = -1 To 1 Step 2
                        If (CoorY + x) >= 0 AndAlso (CoorY + x) <= 7 Then
                            If BlackTFTable(CoorX - 2, CoorY + x) = "T" Then BlackTFTable(CoorX - 2, CoorY + x) = "F"
                            If Not Char.IsUpper(Board(CoorX - 2, CoorY + x)) Then
                                LegalMoveArray(n) = (CoorX - 2) & (CoorY + x)
                                If Board(CoorX - 2, CoorY + x) = "k" Then
                                    BInCheck.IsInCheck = True
                                    CheckingPiece = CoorX & CoorY
                                    Exit For
                                End If
                                n += 1
                            End If
                        End If
                    Next
                End If
                If CoorX <= 5 Then
                    For x = -1 To 1 Step 2
                        If (CoorY + x) >= 0 AndAlso (CoorY + x) <= 7 Then
                            If BlackTFTable(CoorX + 2, CoorY + x) = "T" Then BlackTFTable(CoorX + 2, CoorY + x) = "F"
                            If Not Char.IsUpper(Board(CoorX + 2, CoorY + x)) Then
                                LegalMoveArray(n) = (CoorX + 2) & (CoorY + x)
                                If Board(CoorX + 2, CoorY + x) = "k" Then
                                    BInCheck.IsInCheck = True
                                    CheckingPiece = CoorX & CoorY
                                    Exit For
                                End If
                                n += 1
                            End If
                        End If
                    Next
                End If
                If CoorY >= 2 Then
                    For x = -1 To 1 Step 2
                        If (CoorX + x) >= 0 AndAlso (CoorX + x) <= 7 Then
                            If BlackTFTable(CoorX + x, CoorY - 2) = "T" Then BlackTFTable(CoorX + x, CoorY - 2) = "F"
                            If Not Char.IsUpper(Board(CoorX + x, CoorY - 2)) Then
                                LegalMoveArray(n) = (CoorX + x) & (CoorY - 2)
                                If Board(CoorX + x, CoorY - 2) = "k" Then
                                    BInCheck.IsInCheck = True
                                    CheckingPiece = CoorX & CoorY
                                    Exit For
                                End If
                                n += 1
                            End If
                        End If
                    Next
                End If
                If CoorY <= 5 Then
                    For x = -1 To 1 Step 2
                        If (CoorX + x) >= 0 AndAlso (CoorX + x) <= 7 Then
                            If BlackTFTable(CoorX + x, CoorY + 2) = "T" Then BlackTFTable(CoorX + x, CoorY + 2) = "F"
                            If Not Char.IsUpper(Board(CoorX + x, CoorY + 2)) Then
                                LegalMoveArray(n) = (CoorX + x) & (CoorY + 2)
                                If Board(CoorX + x, CoorY + 2) = "k" Then
                                    BInCheck.IsInCheck = True
                                    CheckingPiece = CoorX & CoorY
                                    Exit For
                                End If
                                n += 1
                            End If
                        End If
                    Next
                End If
            End If


        ElseIf Board(CoorX, CoorY) = "R" OrElse Board(CoorX, CoorY) = "Q" Then 'Legal Moves for the Rook / part of Queen.
            If WhiteTFTable(CoorX, CoorY) <> "1" AndAlso WhiteTFTable(CoorX, CoorY) <> "3" Then
                If WhiteTFTable(CoorX, CoorY) <> "0" Then
                    For x = (CoorX + 1) To 7
                        If BlackTFTable(x, CoorY) = "T" Then BlackTFTable(x, CoorY) = "F"
                        If Char.IsUpper(Board(x, CoorY)) Then
                            If Board(x, CoorY) = "P" AndAlso x & CoorY + 1 = EnPassant Then
                                For m = x + 2 To 7
                                    If Board(m, CoorY) = "k" Then
                                        BlackTFTable(x + 1, CoorY) = "4"
                                        Exit For
                                    End If
                                    If Board(m, CoorY) <> " " Then Exit For
                                Next
                            End If
                            Exit For
                        Else
                            LegalMoveArray(n) = (x) & (CoorY)
                            If Board(x, CoorY) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If BlackTFTable(Math.Min(x + 1, 7), CoorY) = "T" Then BlackTFTable(Math.Min(x + 1, 7), CoorY) = "F"
                                Exit For
                            End If
                            n += 1
                            If Char.IsLower(Board(x, CoorY)) AndAlso CInt(CStr(BKPos(0))) > x AndAlso CInt(CStr(BKPos(1))) = CoorY Then
                                For m = x + 1 To 7
                                    If Board(m, CoorY) = "k" Then
                                        BlackTFTable(x, CoorY) = "2"
                                        Exit For
                                    End If
                                    If Board(m, CoorY) <> " " Then
                                        If Board(m, CoorY) = "P" AndAlso m & CoorY + 1 = EnPassant Then
                                            For a = m + 1 To 7
                                                If Board(a, CoorY) = "k" Then
                                                    BlackTFTable(x, CoorY) = "4"
                                                    Exit For
                                                End If
                                                If Board(a, CoorY) <> " " Then Exit For
                                            Next
                                        End If
                                        Exit For
                                    End If
                                Next
                            End If

                            If Board(x, CoorY) <> " " Then Exit For
                        End If
                    Next

                    For x = (CoorX - 1) To 0 Step -1
                        If BlackTFTable(x, CoorY) = "T" Then BlackTFTable(x, CoorY) = "F"
                        If Char.IsUpper(Board(x, CoorY)) Then
                            If Board(x, CoorY) = "P" AndAlso x & CoorY + 1 = EnPassant Then
                                For m = x - 2 To 0 Step -1
                                    If Board(m, CoorY) = "k" Then
                                        BlackTFTable(x - 1, CoorY) = "4"
                                        Exit For
                                    End If
                                    If Board(m, CoorY) <> " " Then Exit For
                                Next
                            End If
                            Exit For
                        Else
                            LegalMoveArray(n) = (x) & (CoorY)
                            If Board(x, CoorY) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If BlackTFTable(Math.Max(x - 1, 0), CoorY) = "T" Then BlackTFTable(Math.Max(x - 1, 0), CoorY) = "F"
                                Exit For
                            End If
                            n += 1
                            If Char.IsLower(Board(x, CoorY)) AndAlso CInt(CStr(BKPos(0))) < x AndAlso CInt(CStr(BKPos(1))) = CoorY Then
                                For m = x - 1 To 0 Step -1
                                    If Board(m, CoorY) = "k" Then
                                        BlackTFTable(x, CoorY) = "2"
                                        Exit For
                                    End If
                                    If Board(m, CoorY) <> " " Then
                                        If Board(m, CoorY) = "P" AndAlso m & CoorY + 1 = EnPassant Then
                                            For a = m - 1 To 0 Step -1
                                                If Board(a, CoorY) = "k" Then
                                                    BlackTFTable(x, CoorY) = "4"
                                                    Exit For
                                                End If
                                                If Board(a, CoorY) <> " " Then Exit For
                                            Next
                                        End If
                                        Exit For
                                    End If
                                Next
                            End If

                            If Board(x, CoorY) <> " " Then Exit For
                        End If
                    Next

                End If
                If WhiteTFTable(CoorX, CoorY) <> "2" Then
                    For y = (CoorY + 1) To 7
                        If BlackTFTable(CoorX, y) = "T" Then BlackTFTable(CoorX, y) = "F"
                        If Char.IsUpper(Board(CoorX, y)) Then
                            Exit For
                        Else
                            LegalMoveArray(n) = (CoorX) & (y)
                            If Board(CoorX, y) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If BlackTFTable(CoorX, Math.Min(y + 1, 7)) = "T" Then BlackTFTable(CoorX, Math.Min(y + 1, 7)) = "F"
                                Exit For
                            End If
                            n += 1
                            If Char.IsLower(Board(CoorX, y)) AndAlso CInt(CStr(BKPos(0))) = CoorX AndAlso CInt(CStr(BKPos(1))) > y Then
                                For m = y + 1 To 7
                                    If Board(CoorX, m) = "k" Then
                                        BlackTFTable(CoorX, y) = "0"
                                        Exit For
                                    End If
                                    If Board(CoorX, m) <> " " Then Exit For
                                Next
                            End If
                            If Board(CoorX, y) <> " " Then Exit For
                        End If
                    Next

                    For y = (CoorY - 1) To 0 Step -1
                        If BlackTFTable(CoorX, y) = "T" Then BlackTFTable(CoorX, y) = "F"
                        If Char.IsUpper(Board(CoorX, y)) Then
                            Exit For
                        Else
                            LegalMoveArray(n) = (CoorX) & (y)
                            If Board(CoorX, y) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If BlackTFTable(CoorX, Math.Max(y - 1, 0)) = "T" Then BlackTFTable(CoorX, Math.Max(y - 1, 0)) = "F"
                                Exit For
                            End If
                            n += 1
                            If Char.IsLower(Board(CoorX, y)) AndAlso CInt(CStr(BKPos(0))) = CoorX AndAlso CInt(CStr(BKPos(1))) < y Then
                                For m = y - 1 To 0 Step -1
                                    If Board(CoorX, m) = "k" Then
                                        BlackTFTable(CoorX, y) = "0"
                                        Exit For
                                    End If
                                    If Board(CoorX, m) <> " " Then Exit For
                                Next
                            End If

                            If Board(CoorX, y) <> " " Then Exit For
                        End If
                    Next
                End If
            End If
        End If


        If Board(CoorX, CoorY) = "B" OrElse Board(CoorX, CoorY) = "Q" Then 'Legal Moves for the Biship / Part of Queen.
            Dim o As SByte
            Dim p As SByte
            If WhiteTFTable(CoorX, CoorY) <> "0" AndAlso WhiteTFTable(CoorX, CoorY) <> "2" Then
                If WhiteTFTable(CoorX, CoorY) <> "1" Then
                    StartX = CoorX + 1
                    StartY = CoorY + 1
                    Do Until StartX > 7 OrElse StartY > 7
                        If BlackTFTable(StartX, StartY) = "T" Then BlackTFTable(StartX, StartY) = "F"
                        If Char.IsUpper(Board(StartX, StartY)) Then
                            Exit Do
                        Else
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If StartX < 7 AndAlso StartY < 7 Then
                                    If BlackTFTable(StartX + 1, StartY + 1) = "T" Then BlackTFTable(StartX + 1, StartY + 1) = "F"
                                End If
                                Exit Do
                            End If
                            n += 1
                            If Char.IsLower(Board(StartX, StartY)) AndAlso (CInt(CStr(BKPos(0))) - CoorX = CInt(CStr(BKPos(1))) - CoorY) AndAlso CInt(CStr(BKPos(0))) > CoorX Then
                                o = StartX + 1
                                p = StartY + 1
                                Do Until o > 7 OrElse p > 7
                                    If Board(o, p) = "k" Then
                                        BlackTFTable(StartX, StartY) = "3"
                                        Exit Do
                                    End If
                                    If Board(o, p) <> " " Then Exit Do
                                    o += 1
                                    p += 1
                                Loop
                            End If
                            If Board(StartX, StartY) <> " " Then Exit Do
                        End If
                        StartX += 1
                        StartY += 1
                    Loop

                    StartX = CoorX - 1
                    StartY = CoorY - 1
                    Do Until StartX < 0 OrElse StartY < 0
                        If BlackTFTable(StartX, StartY) = "T" Then BlackTFTable(StartX, StartY) = "F"
                        If Char.IsUpper(Board(StartX, StartY)) Then
                            Exit Do
                        Else
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If StartX > 0 AndAlso StartY > 0 Then
                                    If BlackTFTable(StartX - 1, StartY - 1) = "T" Then BlackTFTable(StartX - 1, StartY - 1) = "F"
                                End If
                                Exit Do
                            End If
                            n += 1
                            If Char.IsLower(Board(StartX, StartY)) AndAlso (CInt(CStr(BKPos(0))) - CoorX = CInt(CStr(BKPos(1))) - CoorY) AndAlso CInt(CStr(BKPos(0))) < CoorX Then
                                o = StartX - 1
                                p = StartY - 1
                                Do Until o < 0 OrElse p < 0
                                    If Board(o, p) = "k" Then
                                        BlackTFTable(StartX, StartY) = "3"
                                        Exit Do
                                    End If
                                    If Board(o, p) <> " " Then Exit Do
                                    o -= 1
                                    p -= 1
                                Loop
                            End If
                            If Board(StartX, StartY) <> " " Then Exit Do
                        End If
                        StartX -= 1
                        StartY -= 1
                    Loop

                End If
                If WhiteTFTable(CoorX, CoorY) <> "3" Then
                    StartX = CoorX + 1
                    StartY = CoorY - 1
                    Do Until StartX > 7 OrElse StartY < 0
                        If BlackTFTable(StartX, StartY) = "T" Then BlackTFTable(StartX, StartY) = "F"
                        If Char.IsUpper(Board(StartX, StartY)) Then
                            Exit Do
                        Else
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If StartX < 7 AndAlso StartY > 0 Then
                                    If BlackTFTable(StartX + 1, StartY - 1) = "T" Then BlackTFTable(StartX + 1, StartY - 1) = "F"
                                End If
                                Exit Do
                            End If
                            n += 1
                            If Char.IsLower(Board(StartX, StartY)) AndAlso (CInt(CStr(BKPos(0))) - CoorX = CoorY - CInt(CStr(BKPos(1)))) AndAlso CInt(CStr(BKPos(0))) > CoorX Then
                                o = StartX + 1
                                p = StartY - 1
                                Do Until o > 7 OrElse p < 0
                                    If Board(o, p) = "k" Then
                                        BlackTFTable(StartX, StartY) = "1"
                                        Exit Do
                                    End If
                                    If Board(o, p) <> " " Then Exit Do
                                    o += 1
                                    p -= 1
                                Loop
                            End If

                            If Board(StartX, StartY) <> " " Then Exit Do
                        End If
                        StartX += 1
                        StartY -= 1
                    Loop

                    StartX = CoorX - 1
                    StartY = CoorY + 1
                    Do Until StartX < 0 OrElse StartY > 7
                        If BlackTFTable(StartX, StartY) = "T" Then BlackTFTable(StartX, StartY) = "F"
                        If Char.IsUpper(Board(StartX, StartY)) Then
                            Exit Do
                        Else
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If StartX > 0 AndAlso StartY < 7 Then
                                    If BlackTFTable(StartX - 1, StartY + 1) = "T" Then BlackTFTable(StartX - 1, StartY + 1) = "F"
                                End If
                                Exit Do
                            End If
                            n += 1
                            If Char.IsLower(Board(StartX, StartY)) AndAlso (CInt(CStr(BKPos(0))) - CoorX = CoorY - CInt(CStr(BKPos(1)))) AndAlso CInt(CStr(BKPos(0))) < CoorX Then
                                o = StartX - 1
                                p = StartY + 1
                                Do Until o < 0 OrElse p > 7
                                    If Board(o, p) = "k" Then
                                        BlackTFTable(StartX, StartY) = "1"
                                        Exit Do
                                    End If
                                    If Board(o, p) <> " " Then Exit Do
                                    o -= 1
                                    p += 1
                                Loop
                            End If
                            If Board(StartX, StartY) <> " " Then Exit Do
                        End If
                        StartX -= 1
                        StartY += 1
                    Loop
                End If
            End If
        End If

        'At the end of the function, the program checks to see if any piece is attacking the enemy king.
        'If it is, then the other player is signalled to be in check, and the attacking piece is recorded.
        If CheckingPiece <> " " Then
            If BInCheck.Piece1 = " " OrElse BInCheck.Piece1 = CheckingPiece Then
                BInCheck.Piece1 = CheckingPiece
            Else
                BInCheck.DoubleCheck = True
            End If
        End If
        'The first index of LegalMoveArray contains a counter of how many moves each piece can make. Useful for
        'when the AI is picking moves.
        If n <> 1 Then LegalMoveArray(0) = n
        Return LegalMoveArray
    End Function


    Function BlackPieceLegalMoves(ByRef Board(,) As Char, ByVal CoorX As SByte, ByVal CoorY As SByte, ByRef BlackTFTable(,) As Char, ByRef WhiteTFTable(,) As Char, ByRef WKPos As String, ByRef WInCheck As InCheck, ByRef BInCheck As InCheck, ByRef BCanCastle As CanCastle, ByRef EnPassant As String) As String()
        Dim LegalMoveArray(27) As String
        Dim n As Byte = 1
        Dim StartY As SByte
        Dim EndY As SByte
        Dim StartX As SByte
        Dim EndX As SByte
        Dim CheckingPiece As String = " "

        'If the King is in a Double Check situation, then only King Moves will be accepted.
        'This makes checks much more efficient.
        If BInCheck.DoubleCheck AndAlso Board(CoorX, CoorY) <> "k" Then
            Return LegalMoveArray
            Exit Function
        End If
        If Board(CoorX, CoorY) = "k" Then 'Legal Moves for the King (Along with Castling).
            StartX = Math.Max(CoorX - 1, 0)
            EndX = Math.Min(CoorX + 1, 7)
            StartY = Math.Max(CoorY - 1, 0)
            EndY = Math.Min(CoorY + 1, 7)
            For y = StartY To EndY
                For x = StartX To EndX
                    If WhiteTFTable(x, y) = "T" Then WhiteTFTable(x, y) = "F"
                    If Not Char.IsLower(Board(x, y)) AndAlso BlackTFTable(x, y) = "T" Then
                        LegalMoveArray(n) = (x) & (y)
                        n += 1
                    End If
                Next
            Next
            If BCanCastle.KS AndAlso BInCheck.IsInCheck = False Then
                If Board(CoorX + 1, CoorY) = " " AndAlso Board(CoorX + 2, CoorY) = " " AndAlso Board(CoorX + 3, CoorY) = "r" AndAlso BlackTFTable(CoorX + 1, CoorY) = "T" AndAlso BlackTFTable(CoorX + 2, CoorY) = "T" Then
                    LegalMoveArray(n) = (CoorX + 2) & (CoorY)
                    n += 1
                End If
            End If
            If BCanCastle.QS AndAlso BInCheck.IsInCheck = False Then
                If Board(CoorX - 1, CoorY) = " " AndAlso Board(CoorX - 2, CoorY) = " " AndAlso Board(CoorX - 3, CoorY) = " " AndAlso Board(CoorX - 4, CoorY) = "r" AndAlso BlackTFTable(CoorX - 1, CoorY) = "T" AndAlso BlackTFTable(CoorX - 2, CoorY) = "T" Then
                    LegalMoveArray(n) = (CoorX - 2) & (CoorY)
                    n += 1
                End If
            End If


        ElseIf Board(CoorX, CoorY) = "p" Then 'Legal Moves for the Pawn (along with First Moves.)
            If BlackTFTable(CoorX, CoorY) <> "2" Then
                If BlackTFTable(CoorX, CoorY) <> "1" AndAlso BlackTFTable(CoorX, CoorY) <> "3" Then
                    If Board(CoorX, CoorY + 1) = " " Then
                        LegalMoveArray(n) = CoorX & CoorY + 1
                        n += 1
                        If CoorY = 1 AndAlso Board(CoorX, 3) = " " Then
                            LegalMoveArray(n) = CoorX & 3
                            n += 1
                        End If
                    End If
                End If
                If BlackTFTable(CoorX, CoorY) <> "0" Then
                    If BlackTFTable(CoorX, CoorY) <> "3" AndAlso CoorX > 0 Then
                        If WhiteTFTable(CoorX - 1, CoorY + 1) = "T" Then WhiteTFTable(CoorX - 1, CoorY + 1) = "F"
                        If Char.IsUpper(Board(CoorX - 1, CoorY + 1)) OrElse (CoorX - 1 & CoorY + 1 = EnPassant AndAlso BlackTFTable(CoorX, CoorY) <> "4") Then
                            LegalMoveArray(n) = CoorX - 1 & CoorY + 1
                            If Board(CoorX - 1, CoorY + 1) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                            End If
                            n += 1
                        End If
                    End If
                    If BlackTFTable(CoorX, CoorY) <> "1" AndAlso CoorX < 7 Then
                        If WhiteTFTable(CoorX + 1, CoorY + 1) = "T" Then WhiteTFTable(CoorX + 1, CoorY + 1) = "F"
                        If Char.IsUpper(Board(CoorX + 1, CoorY + 1)) OrElse (CoorX + 1 & CoorY + 1 = EnPassant AndAlso BlackTFTable(CoorX, CoorY) <> "4") Then
                            LegalMoveArray(n) = CoorX + 1 & CoorY + 1
                            If Board(CoorX + 1, CoorY + 1) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                            End If
                            n += 1
                        End If
                    End If
                End If
            End If


        ElseIf Board(CoorX, CoorY) = "n" Then 'Legal Moves for the Knight. (If pinned, then it cannot move at all.)
            If BlackTFTable(CoorX, CoorY) >= "5" Then
                If CoorX >= 2 Then
                    For x = -1 To 1 Step 2
                        If (CoorY + x) >= 0 AndAlso (CoorY + x) <= 7 Then
                            If WhiteTFTable(CoorX - 2, CoorY + x) = "T" Then WhiteTFTable(CoorX - 2, CoorY + x) = "F"
                            If Not Char.IsLower(Board(CoorX - 2, CoorY + x)) Then
                                LegalMoveArray(n) = (CoorX - 2) & (CoorY + x)
                                If Board(CoorX - 2, CoorY + x) = "K" Then
                                    WInCheck.IsInCheck = True
                                    CheckingPiece = CoorX & CoorY
                                    Exit For
                                End If
                                n += 1
                            End If
                        End If
                    Next
                End If
                If CoorX <= 5 Then
                    For x = -1 To 1 Step 2
                        If (CoorY + x) >= 0 AndAlso (CoorY + x) <= 7 Then
                            If WhiteTFTable(CoorX + 2, CoorY + x) = "T" Then WhiteTFTable(CoorX + 2, CoorY + x) = "F"
                            If Not Char.IsLower(Board(CoorX + 2, CoorY + x)) Then
                                LegalMoveArray(n) = (CoorX + 2) & (CoorY + x)
                                If Board(CoorX + 2, CoorY + x) = "K" Then
                                    WInCheck.IsInCheck = True
                                    CheckingPiece = CoorX & CoorY
                                    Exit For
                                End If
                                n += 1
                            End If
                        End If
                    Next
                End If
                If CoorY >= 2 Then
                    For x = -1 To 1 Step 2
                        If (CoorX + x) >= 0 AndAlso (CoorX + x) <= 7 Then
                            If WhiteTFTable(CoorX + x, CoorY - 2) = "T" Then WhiteTFTable(CoorX + x, CoorY - 2) = "F"
                            If Not Char.IsLower(Board(CoorX + x, CoorY - 2)) Then
                                LegalMoveArray(n) = (CoorX + x) & (CoorY - 2)
                                If Board(CoorX + x, CoorY - 2) = "K" Then
                                    WInCheck.IsInCheck = True
                                    CheckingPiece = CoorX & CoorY
                                    Exit For
                                End If
                                n += 1
                            End If
                        End If
                    Next
                End If
                If CoorY <= 5 Then
                    For x = -1 To 1 Step 2
                        If (CoorX + x) >= 0 AndAlso (CoorX + x) <= 7 Then
                            If WhiteTFTable(CoorX + x, CoorY + 2) = "T" Then WhiteTFTable(CoorX + x, CoorY + 2) = "F"
                            If Not Char.IsLower(Board(CoorX + x, CoorY + 2)) Then
                                LegalMoveArray(n) = (CoorX + x) & (CoorY + 2)
                                If Board(CoorX + x, CoorY + 2) = "K" Then
                                    WInCheck.IsInCheck = True
                                    CheckingPiece = CoorX & CoorY
                                    Exit For
                                End If
                                n += 1
                            End If
                        End If
                    Next
                End If
            End If


        ElseIf Board(CoorX, CoorY) = "r" OrElse Board(CoorX, CoorY) = "q" Then 'Legal Moves for the Rook / Part of Queen.
            If BlackTFTable(CoorX, CoorY) <> "1" AndAlso BlackTFTable(CoorX, CoorY) <> "3" Then
                If BlackTFTable(CoorX, CoorY) <> "0" Then
                    For x = (CoorX + 1) To 7
                        If WhiteTFTable(x, CoorY) = "T" Then WhiteTFTable(x, CoorY) = "F"
                        If Char.IsLower(Board(x, CoorY)) Then
                            If Board(x, CoorY) = "p" AndAlso x & CoorY - 1 = EnPassant Then
                                For m = x + 2 To 7
                                    If Board(m, CoorY) = "K" Then
                                        BlackTFTable(x + 1, CoorY) = "4"
                                        Exit For
                                    End If
                                    If Board(m, CoorY) <> " " Then Exit For
                                Next
                            End If

                            Exit For
                        Else
                            LegalMoveArray(n) = (x) & (CoorY)
                            If Board(x, CoorY) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If WhiteTFTable(Math.Min(x + 1, 7), CoorY) = "T" Then WhiteTFTable(Math.Min(x + 1, 7), CoorY) = "F"
                                Exit For
                            End If
                            n += 1
                            If Char.IsUpper(Board(x, CoorY)) AndAlso CInt(CStr(WKPos(0))) > x AndAlso CInt(CStr(WKPos(1))) = CoorY Then
                                For m = x + 1 To 7
                                    If Board(m, CoorY) = "K" Then
                                        WhiteTFTable(x, CoorY) = "2"
                                        Exit For
                                    End If
                                    If Board(m, CoorY) <> " " Then
                                        If Board(m, CoorY) = "p" AndAlso m & CoorY - 1 = EnPassant Then
                                            For a = m + 1 To 7
                                                If Board(a, CoorY) = "K" Then
                                                    BlackTFTable(x, CoorY) = "4"
                                                    Exit For
                                                End If
                                                If Board(a, CoorY) <> " " Then Exit For
                                            Next
                                        End If
                                        Exit For
                                    End If
                                Next
                            End If
                            If Board(x, CoorY) <> " " Then Exit For
                        End If
                    Next

                    For x = (CoorX - 1) To 0 Step -1
                        If WhiteTFTable(x, CoorY) = "T" Then WhiteTFTable(x, CoorY) = "F"
                        If Char.IsLower(Board(x, CoorY)) Then
                            If Board(x, CoorY) = "p" AndAlso x & CoorY - 1 = EnPassant Then
                                For m = x - 2 To 0 Step -1
                                    If Board(m, CoorY) = "K" Then
                                        BlackTFTable(x - 1, CoorY) = "4"
                                        Exit For
                                    End If
                                    If Board(m, CoorY) <> " " Then Exit For
                                Next
                            End If
                            Exit For
                        Else
                            LegalMoveArray(n) = (x) & (CoorY)
                            If Board(x, CoorY) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If WhiteTFTable(Math.Max(x - 1, 0), CoorY) = "T" Then WhiteTFTable(Math.Max(x - 1, 0), CoorY) = "F"
                                Exit For
                            End If
                            n += 1
                            If Char.IsUpper(Board(x, CoorY)) AndAlso CInt(CStr(WKPos(0))) < x AndAlso CInt(CStr(WKPos(1))) = CoorY Then
                                For m = x - 1 To 0 Step -1
                                    If Board(m, CoorY) = "K" Then
                                        WhiteTFTable(x, CoorY) = "2"
                                        Exit For
                                    End If
                                    If Board(m, CoorY) <> " " Then
                                        If Board(m, CoorY) = "p" AndAlso m & CoorY - 1 = EnPassant Then
                                            For a = m - 1 To 0 Step -1
                                                If Board(a, CoorY) = "K" Then
                                                    BlackTFTable(x, CoorY) = "4"
                                                    Exit For
                                                End If
                                                If Board(a, CoorY) <> " " Then Exit For
                                            Next
                                        End If
                                        Exit For
                                    End If
                                Next
                            End If
                            If Board(x, CoorY) <> " " Then Exit For
                        End If
                    Next
                End If

                If BlackTFTable(CoorX, CoorY) <> "2" Then
                    For y = (CoorY + 1) To 7
                        If WhiteTFTable(CoorX, y) = "T" Then WhiteTFTable(CoorX, y) = "F"
                        If Char.IsLower(Board(CoorX, y)) Then
                            Exit For
                        Else
                            LegalMoveArray(n) = (CoorX) & (y)
                            If Board(CoorX, y) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If WhiteTFTable(CoorX, Math.Min(y + 1, 7)) = "T" Then WhiteTFTable(CoorX, Math.Min(y + 1, 7)) = "F"
                                Exit For
                            End If
                            n += 1
                            If Char.IsUpper(Board(CoorX, y)) AndAlso CInt(CStr(WKPos(0))) = CoorX AndAlso CInt(CStr(WKPos(1))) > y Then
                                For m = y + 1 To 7
                                    If Board(CoorX, m) = "K" Then
                                        WhiteTFTable(CoorX, y) = "0"
                                        Exit For
                                    End If
                                    If Board(CoorX, m) <> " " Then Exit For
                                Next
                            End If
                            If Board(CoorX, y) <> " " Then Exit For
                        End If
                    Next

                    For y = (CoorY - 1) To 0 Step -1
                        If WhiteTFTable(CoorX, y) = "T" Then WhiteTFTable(CoorX, y) = "F"
                        If Char.IsLower(Board(CoorX, y)) Then
                            Exit For
                        Else
                            LegalMoveArray(n) = (CoorX) & (y)
                            If Board(CoorX, y) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If WhiteTFTable(CoorX, Math.Max(y - 1, 0)) = "T" Then WhiteTFTable(CoorX, Math.Max(y - 1, 0)) = "F"
                                Exit For
                            End If
                            n += 1
                            If Char.IsUpper(Board(CoorX, y)) AndAlso CInt(CStr(WKPos(0))) = CoorX AndAlso CInt(CStr(WKPos(1))) < y Then
                                For m = y - 1 To 0 Step -1
                                    If Board(CoorX, m) = "K" Then
                                        WhiteTFTable(CoorX, y) = "0"
                                        Exit For
                                    End If
                                    If Board(CoorX, m) <> " " Then Exit For
                                Next
                            End If
                            If Board(CoorX, y) <> " " Then Exit For
                        End If
                    Next
                End If
            End If
        End If


        If Board(CoorX, CoorY) = "b" OrElse Board(CoorX, CoorY) = "q" Then 'Legal Moves for the Bishop / Part of Queen.
            Dim o As SByte
            Dim p As SByte
            If BlackTFTable(CoorX, CoorY) <> "0" AndAlso BlackTFTable(CoorX, CoorY) <> "2" Then
                If BlackTFTable(CoorX, CoorY) <> "1" Then
                    StartX = CoorX + 1
                    StartY = CoorY + 1
                    Do Until StartX > 7 OrElse StartY > 7
                        If WhiteTFTable(StartX, StartY) = "T" Then WhiteTFTable(StartX, StartY) = "F"
                        If Char.IsLower(Board(StartX, StartY)) Then
                            Exit Do
                        Else
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If StartX < 7 AndAlso StartY < 7 Then
                                    If WhiteTFTable(StartX + 1, StartY + 1) = "T" Then WhiteTFTable(StartX + 1, StartY + 1) = "F"
                                End If
                                Exit Do
                            End If
                            n += 1
                            If Char.IsUpper(Board(StartX, StartY)) AndAlso (CInt(CStr(WKPos(0))) - CoorX = CInt(CStr(WKPos(1))) - CoorY) AndAlso CInt(CStr(WKPos(0))) > CoorX Then
                                o = StartX + 1
                                p = StartY + 1
                                Do Until o > 7 OrElse p > 7
                                    If Board(o, p) = "K" Then
                                        WhiteTFTable(StartX, StartY) = "3"
                                        Exit Do
                                    End If
                                    If Board(o, p) <> " " Then Exit Do
                                    o += 1
                                    p += 1
                                Loop
                            End If
                            If Board(StartX, StartY) <> " " Then Exit Do
                        End If
                        StartX += 1
                        StartY += 1
                    Loop

                    StartX = CoorX - 1
                    StartY = CoorY - 1
                    Do Until StartX < 0 OrElse StartY < 0
                        If WhiteTFTable(StartX, StartY) = "T" Then WhiteTFTable(StartX, StartY) = "F"
                        If Char.IsLower(Board(StartX, StartY)) Then
                            Exit Do
                        Else
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If StartX > 0 AndAlso StartY > 0 Then
                                    If WhiteTFTable(StartX - 1, StartY - 1) = "T" Then WhiteTFTable(StartX - 1, StartY - 1) = "F"
                                End If
                                Exit Do
                            End If
                            n += 1
                            If Char.IsUpper(Board(StartX, StartY)) AndAlso (CInt(CStr(WKPos(0))) - CoorX = CInt(CStr(WKPos(1))) - CoorY) AndAlso CInt(CStr(WKPos(0))) < CoorX Then
                                o = StartX - 1
                                p = StartY - 1
                                Do Until o < 0 OrElse p < 0
                                    If Board(o, p) = "K" Then
                                        WhiteTFTable(StartX, StartY) = "3"
                                        Exit Do
                                    End If
                                    If Board(o, p) <> " " Then Exit Do
                                    o -= 1
                                    p -= 1
                                Loop
                            End If
                            If Board(StartX, StartY) <> " " Then Exit Do
                        End If
                        StartX -= 1
                        StartY -= 1
                    Loop
                End If

                If BlackTFTable(CoorX, CoorY) <> "3" Then
                    StartX = CoorX + 1
                    StartY = CoorY - 1
                    Do Until StartX > 7 OrElse StartY < 0
                        If WhiteTFTable(StartX, StartY) = "T" Then WhiteTFTable(StartX, StartY) = "F"
                        If Char.IsLower(Board(StartX, StartY)) Then
                            Exit Do
                        Else
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If StartX < 7 AndAlso StartY > 0 Then
                                    If WhiteTFTable(StartX + 1, StartY - 1) = "T" Then WhiteTFTable(StartX + 1, StartY - 1) = "F"
                                End If
                                Exit Do
                            End If
                            n += 1
                            If Char.IsUpper(Board(StartX, StartY)) AndAlso (CInt(CStr(WKPos(0))) - CoorX = CoorY - CInt(CStr(WKPos(1)))) AndAlso CInt(CStr(WKPos(0))) > CoorX Then
                                o = StartX + 1
                                p = StartY - 1
                                Do Until o > 7 OrElse p < 0
                                    If Board(o, p) = "K" Then
                                        WhiteTFTable(StartX, StartY) = "1"
                                        Exit Do
                                    End If
                                    If Board(o, p) <> " " Then Exit Do
                                    o += 1
                                    p -= 1
                                Loop
                            End If
                            If Board(StartX, StartY) <> " " Then Exit Do
                        End If
                        StartX += 1
                        StartY -= 1
                    Loop

                    StartX = CoorX - 1
                    StartY = CoorY + 1
                    Do Until StartX < 0 OrElse StartY > 7
                        If WhiteTFTable(StartX, StartY) = "T" Then WhiteTFTable(StartX, StartY) = "F"
                        If Char.IsLower(Board(StartX, StartY)) Then
                            Exit Do
                        Else
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                If StartX > 0 AndAlso StartY < 7 Then
                                    If WhiteTFTable(StartX - 1, StartY + 1) = "T" Then WhiteTFTable(StartX - 1, StartY + 1) = "F"
                                End If
                                Exit Do
                            End If
                            n += 1
                            If Char.IsUpper(Board(StartX, StartY)) AndAlso (CInt(CStr(WKPos(0))) - CoorX = CoorY - CInt(CStr(WKPos(1)))) AndAlso CInt(CStr(WKPos(0))) < CoorX Then
                                o = StartX - 1
                                p = StartY + 1
                                Do Until o < 0 OrElse p > 7
                                    If Board(o, p) = "K" Then
                                        WhiteTFTable(StartX, StartY) = "1"
                                        Exit Do
                                    End If
                                    If Board(o, p) <> " " Then Exit Do
                                    o -= 1
                                    p += 1
                                Loop
                            End If
                            If Board(StartX, StartY) <> " " Then Exit Do
                        End If
                        StartX -= 1
                        StartY += 1
                    Loop
                End If
            End If
        End If

        'At the end of the function, the program checks to see if any piece is attacking the enemy king.
        'If it is, then the other player is signalled to be in check, and the attacking piece is recorded.
        If CheckingPiece <> " " Then
            If WInCheck.Piece1 = " " OrElse WInCheck.Piece1 = CheckingPiece Then
                WInCheck.Piece1 = CheckingPiece
            Else
                WInCheck.DoubleCheck = True
            End If
        End If
        'The first index of LegalMoveArray contains a counter of how many moves each piece can make. Useful for
        'when the AI is picking moves.
        If n <> 1 Then LegalMoveArray(0) = n
        Return LegalMoveArray
    End Function





    Private Sub Piece_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles WK1.MouseDown, WQ1.MouseDown, WB1.MouseDown, WB2.MouseDown, WN1.MouseDown, WN2.MouseDown, WR1.MouseDown, WR2.MouseDown, WP1.MouseDown, WP2.MouseDown, WP3.MouseDown, WP4.MouseDown, WP5.MouseDown, WP6.MouseDown, WP7.MouseDown, WP8.MouseDown, BK1.MouseDown, BQ1.MouseDown, BB1.MouseDown, BB2.MouseDown, BN1.MouseDown, BN2.MouseDown, BR1.MouseDown, BR2.MouseDown, BP1.MouseDown, BP2.MouseDown, BP3.MouseDown, BP4.MouseDown, BP5.MouseDown, BP6.MouseDown, BP7.MouseDown, BP8.MouseDown, WQ8.MouseDown, WQ7.MouseDown, WQ6.MouseDown, WQ5.MouseDown, WQ4.MouseDown, WQ3.MouseDown, BQ6.MouseDown, BQ8.MouseDown, BQ5.MouseDown, BQ7.MouseDown, BQ2.MouseDown, BQ4.MouseDown, BQ3.MouseDown, WQ2.MouseDown
        'Subroutine that activates the drag + drog mechanic if the user clicks on a piece.
        If e.Button = Windows.Forms.MouseButtons.Left Then
            'Generates the Legal Moves for the piece the user has picked up.
            If GameRunning Then
                If CStr(sender.name)(0) = "W" AndAlso PlayerTurn Then
                    LegalMoves = WhitePieceLegalMoves(MasterBoard, sender.location.x / 75, sender.location.y / 75, MasterWhiteTFTable, MasterBlackTFTable, MasterBKPos, MasterBInCheck, MasterWInCheck, MasterWCanCastle, MasterEnPassant)
                    MovingPiece = True
                ElseIf CStr(sender.name)(0) = "B" AndAlso (Not PlayerTurn) Then
                    LegalMoves = BlackPieceLegalMoves(MasterBoard, sender.location.x / 75, sender.location.y / 75, MasterBlackTFTable, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterBCanCastle, MasterEnPassant)
                    MovingPiece = True
                End If
            End If
            sender.bringtofront()
            StartPoint = sender.location
            MidPoint = sender.PointToScreen(New Point(e.X, e.Y))
            'Console.WriteLine()
            'Console.WriteLine("Legal Moves:")
            'If LegalMoves(0) IsNot Nothing Then
            '    For x = 1 To LegalMoves(0) - 1
            '        If LegalMoves(x) IsNot Nothing Then Console.WriteLine(LegalMoves(x))
            '    Next
            'End If
            'Console.WriteLine("Done.")
        End If
    End Sub

    Private Sub Piece_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles WK1.MouseMove, WQ1.MouseMove, WB1.MouseMove, WB2.MouseMove, WN1.MouseMove, WN2.MouseMove, WR1.MouseMove, WR2.MouseMove, WP1.MouseMove, WP2.MouseMove, WP3.MouseMove, WP4.MouseMove, WP5.MouseMove, WP6.MouseMove, WP7.MouseMove, WP8.MouseMove, BK1.MouseMove, BQ1.MouseMove, BB1.MouseMove, BB2.MouseMove, BN1.MouseMove, BN2.MouseMove, BR1.MouseMove, BR2.MouseMove, BP1.MouseMove, BP2.MouseMove, BP3.MouseMove, BP4.MouseMove, BP5.MouseMove, BP6.MouseMove, BP7.MouseMove, BP8.MouseMove, WQ2.MouseMove, WQ3.MouseMove, WQ4.MouseMove, WQ5.MouseMove, WQ6.MouseMove, WQ7.MouseMove, WQ8.MouseMove, BQ2.MouseMove, BQ3.MouseMove, BQ4.MouseMove, BQ5.MouseMove, BQ6.MouseMove, BQ7.MouseMove, BQ8.MouseMove
        'The piece is bound to the position of the user's mouse.
        If MovingPiece Then
            EndPoint = sender.PointToScreen(New Point(e.X, e.Y))
            sender.Left += (EndPoint.X - MidPoint.X)
            sender.Top += (EndPoint.Y - MidPoint.Y)
            MidPoint = EndPoint
        End If
    End Sub

    Private Sub Piece_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles WK1.MouseUp, WQ1.MouseUp, WB1.MouseUp, WB2.MouseUp, WN1.MouseUp, WN2.MouseUp, WR1.MouseUp, WR2.MouseUp, WP1.MouseUp, WP2.MouseUp, WP3.MouseUp, WP4.MouseUp, WP5.MouseUp, WP6.MouseUp, WP7.MouseUp, WP8.MouseUp, BK1.MouseUp, BQ1.MouseUp, BB1.MouseUp, BB2.MouseUp, BN1.MouseUp, BN2.MouseUp, BR1.MouseUp, BR2.MouseUp, BP1.MouseUp, BP2.MouseUp, BP3.MouseUp, BP4.MouseUp, BP5.MouseUp, BP6.MouseUp, BP7.MouseUp, BP8.MouseUp, WQ2.MouseUp, WQ3.MouseUp, WQ4.MouseUp, WQ5.MouseUp, WQ6.MouseUp, WQ7.MouseUp, WQ8.MouseUp, BQ2.MouseUp, BQ3.MouseUp, BQ4.MouseUp, BQ5.MouseUp, BQ6.MouseUp, BQ7.MouseUp, BQ8.MouseUp
        Dim CheckBoard(7, 7) As Char
        Dim JustCastled As Boolean = False
        Dim IsLegalMove As Boolean
        Dim ReplacedPiece As PictureBox
        If e.Button = Windows.Forms.MouseButtons.Left Then
            'Disables drag & drop mechanic and sets the piece's position on the board.
            MovingPiece = False
            sender.location = New Point(sender.location.X + 37.5 - (sender.location.X + 37.5) Mod 75, sender.location.y + 37.5 - (sender.location.y + 37.5) Mod 75)
            'If the user has actually moved the piece, and the move is legal, the program continues.
            If sender.location <> StartPoint Then
                IsLegalMove = False
                For n = 1 To LegalMoves.Length - 1
                    If CStr(sender.location.x / 75 & sender.location.y / 75) = LegalMoves(n) Then
                        'If the player is in check, the ResolveCheck Function is called to determine whether the player would still be in check after the move is made.
                        If (MasterWInCheck.IsInCheck AndAlso MasterBoard(StartPoint.X / 75, StartPoint.Y / 75) <> "K") OrElse (MasterBInCheck.IsInCheck AndAlso MasterBoard(StartPoint.X / 75, StartPoint.Y / 75) <> "k") Then
                            If DoesMoveResolveCheck(MasterBoard, StartPoint.X / 75, StartPoint.Y / 75, sender.location.X / 75, sender.location.Y / 75, MasterWInCheck, MasterBInCheck, MasterEnPassant) Then
                                MasterWInCheck.Piece1 = " "
                                MasterWInCheck.DoubleCheck = False
                                MasterBInCheck.Piece1 = " "
                                MasterBInCheck.DoubleCheck = False
                                IsLegalMove = True
                            End If
                        Else
                            MasterWInCheck.IsInCheck = False
                            MasterWInCheck.Piece1 = " "
                            MasterWInCheck.DoubleCheck = False
                            MasterBInCheck.IsInCheck = False
                            MasterBInCheck.Piece1 = " "
                            MasterBInCheck.DoubleCheck = False
                            IsLegalMove = True
                        End If
                    End If
                Next
                If IsLegalMove Then
                    If UCase(sender.name(1)) = "K" Then
                        'Disables Castling if the King has moved.
                        If sender.name(0) = "W" Then
                            MasterWKPos = (sender.location.x / 75) & (sender.location.y / 75)
                            MasterWCanCastle.KS = False
                            MasterWCanCastle.QS = False
                        Else
                            MasterBKPos = (sender.location.x / 75) & (sender.location.y / 75)
                            MasterBCanCastle.KS = False
                            MasterBCanCastle.QS = False
                        End If
                        'Code for Castling (Queen Side & King Side).
                        If (sender.location.x / 75 & sender.location.y / 75) = ((StartPoint.X / 75) + 2 & StartPoint.Y / 75) Then
                            JustCastled = True
                            Sound_Castle.Play()
                            MasterBoard(5, sender.location.y / 75) = CStr(MasterBoard(7, sender.location.y / 75))
                            MasterBoard(7, sender.location.y / 75) = " "
                            If sender.name(0) = "W" Then
                                WR2.Left -= 150
                            Else
                                BR2.Left -= 150
                            End If
                        ElseIf (sender.location.x / 75 & sender.location.y / 75) = ((StartPoint.X / 75) - 2 & StartPoint.Y / 75) Then
                            JustCastled = True
                            Sound_Castle.Play()
                            MasterBoard(3, sender.location.y / 75) = CStr(MasterBoard(0, sender.location.y / 75))
                            MasterBoard(0, sender.location.y / 75) = " "
                            If sender.name(0) = "W" Then
                                WR1.Left += 225
                            Else
                                BR1.Left += 225
                            End If
                        End If
                    End If
                    If MasterBoard(sender.location.X / 75, sender.location.Y / 75) <> " " Then
                        'If the move is a capture, the required sound is played and the captured piece is removed from the board.
                        Sound_Capture.Play()
                        ReplacedPiece = MarrToPieceConv(sender.location.X, sender.location.Y, CStr(sender.name)(0))
                        ReplacedPiece.Visible = False
                        ReplacedPiece.Location = New Point(-100, -100)
                    ElseIf JustCastled = False Then
                        Sound_Move.Play()
                    End If
                    'MasterBoard is updated by clearing the piece's previous piosition and reallocating its current position.
                    MasterBoard(StartPoint.X / 75, StartPoint.Y / 75) = " "

                    If CStr(sender.name)(0) = "W" Then
                        MasterBoard(sender.location.X / 75, sender.location.Y / 75) = UCase((CStr(sender.name))(1))
                        'If a pawn has made it to the end of the board, it is promoted to a Queen.
                        If sender.name(1) = "P" Then
                            If sender.location.y / 75 = 0 Then
                                For n = 1 To 8
                                    ReplacedPiece = Me.Controls.Find("WQ" & n.ToString, True).Single()
                                    If ReplacedPiece.Visible = False Then Exit For
                                Next
                                '#isable Warning BC42104 'Variable is used before it has been assigned a value
                                ReplacedPiece.Location = New Point(sender.location.X, sender.location.Y)
                                '#nable Warning BC42104 'Variable is used before it has been assigned a value
                                ReplacedPiece.Visible = True
                                ReplacedPiece.BringToFront()
                                MasterBoard(sender.location.X / 75, sender.location.Y / 75) = "Q"
                                sender.Visible = False
                                sender.Location = New Point(-100, -100)
                            ElseIf sender.location.X / 75 & sender.location.Y / 75 = MasterEnPassant Then
                                'Rules for En Passant - removes the piece behind it.
                                Sound_Capture.Play()
                                ReplacedPiece = MarrToPieceConv(sender.location.X, sender.location.Y + 75, CStr(sender.name)(0))
                                ReplacedPiece.Visible = False
                                ReplacedPiece.Location = New Point(-100, -100)
                                MasterBoard(sender.location.X / 75, (sender.location.Y / 75) + 1) = " "
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
                        'En Passant & The TrueFalse Table is generated for the Black Pieces.
                        If sender.location.Y - StartPoint.Y = -150 AndAlso sender.name(1) = "P" Then
                            If MasterBoard(Math.Max((sender.location.x / 75) - 1, 0), sender.location.Y / 75) = "p" OrElse MasterBoard(Math.Min((sender.location.x / 75) + 1, 7), sender.location.Y / 75) = "p" Then
                                MasterEnPassant = sender.location.x / 75 & 5
                            End If
                        Else
                            MasterEnPassant = "-"
                        End If
                        FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, NotInCheck, MasterBInCheck, MasterEnPassant)
                    Else
                        MasterBoard(sender.location.X / 75, sender.location.Y / 75) = LCase((CStr(sender.name))(1))
                        'Identical Code as above but for the black pieces.
                        If sender.name(1) = "P" Then
                            If sender.location.y / 75 = 7 Then
                                For n = 1 To 8
                                    ReplacedPiece = Me.Controls.Find("BQ" & n.ToString, True).Single()
                                    If ReplacedPiece.Visible = False Then Exit For
                                Next
                                ReplacedPiece.Location = New Point(sender.location.X, sender.location.Y)
                                ReplacedPiece.Visible = True
                                MasterBoard(sender.location.X / 75, sender.location.Y / 75) = "q"
                                sender.Visible = False
                                sender.Location = New Point(-100, -100)
                            ElseIf sender.location.X / 75 & sender.location.Y / 75 = MasterEnPassant Then
                                'Rules for En Passant - removes the piece behind it.
                                Sound_Capture.Play()
                                ReplacedPiece = MarrToPieceConv(sender.location.X, sender.location.Y - 75, CStr(sender.name)(0))
                                ReplacedPiece.Visible = False
                                ReplacedPiece.Location = New Point(-100, -100)
                                MasterBoard(sender.location.X / 75, (sender.location.Y / 75) - 1) = " "
                            End If
                        End If
                        If sender.name(1) = "R" Then
                            If sender.name(2) = "1" Then
                                MasterBCanCastle.QS = False
                            Else
                                MasterBCanCastle.KS = False
                            End If
                        End If
                        'En Passant & The TrueFalse Table is generated for the White Pieces.
                        If sender.location.Y - StartPoint.Y = 150 AndAlso sender.name(1) = "P" Then
                            If MasterBoard(Math.Max((sender.location.x / 75) - 1, 0), sender.location.Y / 75) = "P" OrElse MasterBoard(Math.Min((sender.location.x / 75) + 1, 7), sender.location.Y / 75) = "P" Then
                                MasterEnPassant = sender.location.x / 75 & 2
                            End If
                        Else
                            MasterEnPassant = "-"
                        End If
                        FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, NotInCheck, MasterEnPassant)
                    End If

                    'Checks if any player is in check, and if so notifies the player with audio and visuals.
                    CheckChecker()
                    If MasterWInCheck.IsInCheck OrElse MasterBInCheck.IsInCheck Then
                        Sound_Check.Play()
                    End If

                    MasterMaterialCount = CountMaterial(MasterBoard, True)
                    'Draw by insufficient material is then checked for. In principle, if it is physically impossible for one
                    'player to checkmate the other (such as king vs king, or king vs king + a knight / bishop), then the
                    'position is delared 'dead' and the game ends in a draw.
                    If MasterMaterialCount = 0 Then
                        CheckLabel.Text = "     Draw!     "
                        Sound_Stalemate.Play()
                        GameRunning = False
                    ElseIf MasterMaterialCount = 3 Then
                        For y = 0 To 7
                            For x = 0 To 7
                                If UCase(MasterBoard(x, y)) = "B" OrElse UCase(MasterBoard(x, y)) = "N" Then
                                    CheckLabel.Text = "     Draw!     "
                                    Sound_Stalemate.Play()
                                    GameRunning = False
                                    Exit For
                                End If
                            Next
                            If Not GameRunning Then Exit For
                        Next
                    End If
                    MasterMaterialCount = CountMaterial(MasterBoard, False)
                    CurrentEval.Text = MasterMaterialCount

                    Console.WriteLine(vbCrLf & "WKPos: " & MasterWKPos & ". BKPos: " & MasterBKPos)
                    For y = 0 To 7
                        For x = 0 To 7
                            Console.Write(MasterBoard(x, y))
                        Next
                        Console.WriteLine()
                    Next

                    'Checks for Checkmate & Stalemate are calculated by running the opponent AI on a depth of 0,
                    'meaning that a depth of 1 is conducted but no moves are actually made on the board. This
                    'determines if the opponent actually has any legal moves, and if they don't, then then the
                    'game will end. (If in check then Checkmate, otherwise Stalemate.)
                    PlayerTurn = Not PlayerTurn
                    AbsoluteDepth = 0
                    If PlayerTurn Then
                        WhiteAIMove_Click()
                    Else
                        BlackAIMove_Click()
                    End If
                    'Absolute depth is then restored, and the next turn begins.
                    CalculateAbsoluteDepth()
                    PreviousFEN = CurrentFEN
                    ConvertToFEN()
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
                Else 'Resets piece to previous position
                    sender.location = StartPoint
                End If
            End If
        End If
    End Sub


    'Subroutine which generates all possible moves of the enemy player. This creates the TrueFalse Table of
    'the selected player (controlled by the Variable FixWhite). Can be programmed to have an Exception Piece,
    'meaning that a specific piece can be left out of the subrouting (useful for making Move Generation more
    'efficient).
    Private Sub FixTFTables(ByRef Board(,) As Char, ByRef FixWhite As Boolean, ByRef TrueFalseTable(,) As Char, ByRef KPos As String, ByRef WInCheck As InCheck, ByRef BInCheck As InCheck, ByRef EnPassant As String)
        'Stopwatch.Reset()
        'Stopwatch.Start()
        Dim dx As SByte
        Dim dy As SByte
        Array.Copy(MasterTrueTable, TrueFalseTable, 64)
        If FixWhite Then
            For y = 0 To 7
                For x = 0 To 7
                    If Char.IsLower(Board(x, y)) Then
                        dx = Math.Abs(CStr(KPos(0)) - x)
                        dy = Math.Abs(CStr(KPos(1)) - y)
                        If Board(x, y) = "p" Then
                            If (CStr(KPos(1)) - y >= 0 AndAlso CStr(KPos(1)) - y <= 2) AndAlso dx <= 2 Then
                                Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, WInCheck, BInCheck, CannotCastle, EnPassant)
                            End If
                        ElseIf Board(x, y) = "r" OrElse Board(x, y) = "q" Then
                            If dx <= 1 OrElse dy <= 1 Then
                                Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, WInCheck, BInCheck, CannotCastle, EnPassant)
                            End If
                        ElseIf Board(x, y) = "n" Then
                            If (dx <= 3 AndAlso dy <= 2) OrElse (dx <= 2 AndAlso dy <= 3) Then
                                Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, WInCheck, BInCheck, CannotCastle, EnPassant)
                            End If
                        ElseIf Board(x, y) = "k" Then
                            If dx <= 2 AndAlso dy <= 2 Then
                                Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, WInCheck, BInCheck, CannotCastle, EnPassant)
                            End If
                        End If
                    End If
                    If Board(x, y) = "b" OrElse Board(x, y) = "q" Then
                        If dx - dy <= 2 AndAlso dy - dx <= 2 Then
                            Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, WInCheck, BInCheck, CannotCastle, EnPassant)
                        End If
                    End If
                Next
            Next
        Else
            For y = 0 To 7
                For x = 0 To 7
                    If Char.IsUpper(Board(x, y)) Then
                        dx = Math.Abs(CStr(KPos(0)) - x)
                        dy = Math.Abs(CStr(KPos(1)) - y)
                        If Board(x, y) = "P" Then
                            If (CStr(KPos(1)) - y <= 0 AndAlso CStr(KPos(1)) - y >= -2) AndAlso dx <= 2 Then
                                Dim LegalMovesResult = WhitePieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, BInCheck, WInCheck, CannotCastle, EnPassant)
                            End If
                        ElseIf Board(x, y) = "R" OrElse Board(x, y) = "Q" Then
                            If dx <= 1 OrElse dy <= 1 Then
                                Dim LegalMovesResult = WhitePieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, BInCheck, WInCheck, CannotCastle, EnPassant)
                            End If
                        ElseIf Board(x, y) = "N" Then
                            If (dx <= 3 AndAlso dy <= 2) OrElse (dx <= 2 AndAlso dy <= 3) Then
                                Dim LegalMovesResult = WhitePieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, BInCheck, WInCheck, CannotCastle, EnPassant)
                            End If
                        ElseIf Board(x, y) = "K" Then
                            If dx <= 2 AndAlso dy <= 2 Then
                                Dim LegalMovesResult = WhitePieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, BInCheck, WInCheck, CannotCastle, EnPassant)
                            End If
                        End If
                    End If
                    If Board(x, y) = "B" OrElse Board(x, y) = "Q" Then
                        If dx - dy <= 2 AndAlso dy - dx <= 2 Then
                            Dim LegalMovesResult = WhitePieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, BInCheck, WInCheck, CannotCastle, EnPassant)
                        End If
                    End If
                Next
            Next
        End If
        'Stopwatch.Stop()
        'Console.WriteLine(vbCrLf)
        'Console.WriteLine(Stopwatch.Elapsed.TotalMilliseconds)

        'Console.WriteLine()
        'Console.WriteLine("WhiteTFTable:")
        'For y = 0 To 7
        '    For x = 0 To 7
        '        Console.Write(MasterWhiteTFTable(x, y))
        '    Next
        '    Console.WriteLine()
        'Next
        'Console.WriteLine("BlackTFTable:")
        'For y = 0 To 7
        '    For x = 0 To 7
        '        Console.Write(MasterBlackTFTable(x, y))
        '    Next
        '    Console.WriteLine()
        'Next
    End Sub

    'Subroutine which updates the Sprites / Gamestate Textbox depending on whether a player is in check (or not).
    Private Sub CheckChecker()
        If MasterWInCheck.IsInCheck OrElse MasterBInCheck.IsInCheck Then
            CheckLabel.Text = "    Check!    "
        Else
            CheckLabel.Text = "                    "
        End If
        If MasterWInCheck.IsInCheck Then
            WK1.Image = Image.FromFile(Application.StartupPath & "\WKingCheck.png")
        Else
            WK1.Image = Image.FromFile(Application.StartupPath & "\WKing.png")
        End If
        If MasterBInCheck.IsInCheck Then
            BK1.Image = Image.FromFile(Application.StartupPath & "\BKingCheck.png")
        Else
            BK1.Image = Image.FromFile(Application.StartupPath & "\BKing.png")
        End If
    End Sub

    'Function which receives a game state and a possible move. The function makes this move on the board (using
    'shortcuts that can only be made on a virtual board), and then generates the possible moves of the attacking
    'piece(s). If the king is no longer being threatened, then the check as been resolved.
    Function DoesMoveResolveCheck(ByRef Board(,) As Char, ByRef OldPosX As String, ByRef OldPosY As String, ByRef NewPosX As String, ByRef NewPosY As String, ByRef WInCheck As InCheck, ByRef BInCheck As InCheck, ByRef EnPassant As String) As Boolean
        Dim CheckBoard(7, 7) As Char
        DoesMoveResolveCheck = False
        If WInCheck.IsInCheck AndAlso Not (NewPosX & NewPosY = WInCheck.Piece1) Then
            WInCheck.IsInCheck = False
            Array.Copy(Board, CheckBoard, 64)
            CheckBoard(NewPosX, NewPosY) = "O"
            If NewPosX & NewPosY = EnPassant AndAlso CheckBoard(OldPosX, OldPosY) = "P" Then CheckBoard(NewPosX, NewPosY + 1) = " " 'Check this.
            Array.Copy(MasterTrueTable, TrueTable, 64)
            BlackPieceLegalMoves(CheckBoard, CStr(WInCheck.Piece1(0)), CStr(WInCheck.Piece1(1)), TrueTable, TrueTable, "00", WInCheck, BInCheck, MasterBCanCastle, "-")
            If WInCheck.IsInCheck = False Then
                DoesMoveResolveCheck = True
            End If
        ElseIf BInCheck.IsInCheck AndAlso Not (NewPosX & NewPosY = BInCheck.Piece1) Then
            BInCheck.IsInCheck = False
            Array.Copy(Board, CheckBoard, 64)
            CheckBoard(NewPosX, NewPosY) = "o"
            If NewPosX & NewPosY = EnPassant AndAlso CheckBoard(OldPosX, OldPosY) = "p" Then CheckBoard(NewPosX, NewPosY - 1) = " " 'Check this.
            Array.Copy(MasterTrueTable, TrueTable, 64)
            WhitePieceLegalMoves(CheckBoard, CStr(BInCheck.Piece1(0)), CStr(BInCheck.Piece1(1)), TrueTable, TrueTable, "00", BInCheck, WInCheck, MasterWCanCastle, "-")
            If BInCheck.IsInCheck = False Then
                DoesMoveResolveCheck = True
            End If
        Else
            WInCheck.IsInCheck = False
            BInCheck.IsInCheck = False
            DoesMoveResolveCheck = True
        End If
    End Function




    Protected BestMove1 As NewMove
    Protected BestMove2 As NewMove
    Protected BestMove3 As NewMove
    Protected BestMove4 As NewMove
    Protected BestMove5 As NewMove
    Protected BestMove6 As NewMove
    Protected BestMove7 As NewMove
    Protected BestMove8 As NewMove

    'if find checkmate in thread then terminate all other threads.
    Private Sub InitialiseThread1()
        If PlayerTurn Then
            BestMove1 = WhiteAI(0)
        Else
            BestMove1 = BlackAI(0)
        End If
        If AbsoluteDepth <> 0 Then Progress += 1
        Console.WriteLine(BestMove1.Score)
    End Sub
    Private Sub InitialiseThread2()
        If PlayerTurn Then
            BestMove2 = WhiteAI(1)
        Else
            BestMove2 = BlackAI(1)
        End If
        If AbsoluteDepth <> 0 Then Progress += 1
        Console.WriteLine(BestMove2.Score)
    End Sub
    Private Sub InitialiseThread3()
        If PlayerTurn Then
            BestMove3 = WhiteAI(2)
        Else
            BestMove3 = BlackAI(2)
        End If
        If AbsoluteDepth <> 0 Then Progress += 1
        Console.WriteLine(BestMove3.Score)
    End Sub
    Private Sub InitialiseThread4()
        If PlayerTurn Then
            BestMove4 = WhiteAI(3)
        Else
            BestMove4 = BlackAI(3)
        End If
        If AbsoluteDepth <> 0 Then Progress += 1
        Console.WriteLine(BestMove4.Score)
    End Sub
    Private Sub InitialiseThread5()
        If PlayerTurn Then
            BestMove5 = WhiteAI(4)
        Else
            BestMove5 = BlackAI(4)
        End If
        If AbsoluteDepth <> 0 Then Progress += 1
        Console.WriteLine(BestMove5.Score)
    End Sub
    Private Sub InitialiseThread6()
        If PlayerTurn Then
            BestMove6 = WhiteAI(5)
        Else
            BestMove6 = BlackAI(5)
        End If
        If AbsoluteDepth <> 0 Then Progress += 1
        Console.WriteLine(BestMove6.Score)
    End Sub
    Private Sub InitialiseThread7()
        If PlayerTurn Then
            BestMove7 = WhiteAI(6)
        Else
            BestMove7 = BlackAI(6)
        End If
        If AbsoluteDepth <> 0 Then Progress += 1
        Console.WriteLine(BestMove7.Score)
    End Sub
    Private Sub InitialiseThread8()
        If PlayerTurn Then
            BestMove8 = WhiteAI(7)
        Else
            BestMove8 = BlackAI(7)
        End If
        If AbsoluteDepth <> 0 Then Progress += 1
        Console.WriteLine(BestMove8.Score)
    End Sub

    Private Function GetBestMove(ByRef HighestScore As Boolean) As NewMove
        Dim MoveArray(7) As Decimal
        Dim JointBestMoves(7) As Byte
        Dim Counter As Byte = 0
        MoveArray(0) = BestMove1.Score
        MoveArray(1) = BestMove2.Score
        MoveArray(2) = BestMove3.Score
        MoveArray(3) = BestMove4.Score
        MoveArray(4) = BestMove5.Score
        MoveArray(5) = BestMove6.Score
        MoveArray(6) = BestMove7.Score
        MoveArray(7) = BestMove8.Score
        Dim Pointer As Byte = 0
        For n = 1 To MoveArray.Length - 1
            If HighestScore Then
                If MoveArray(n) > MoveArray(Pointer) Then
                    Pointer = n
                    Counter = 0
                    JointBestMoves(Counter) = Pointer
                ElseIf MoveArray(n) = MoveArray(Pointer) Then
                    Counter += 1
                    JointBestMoves(Counter) = n
                End If
            Else
                If MoveArray(n) < MoveArray(Pointer) Then
                    Pointer = n
                    Counter = 0
                    JointBestMoves(Counter) = Pointer
                ElseIf MoveArray(n) = MoveArray(Pointer) Then
                    Counter += 1
                    JointBestMoves(Counter) = n
                End If
            End If
        Next
        Pointer = JointBestMoves(Int(Rnd() * Counter))
        If Pointer = 0 Then
            GetBestMove.Score = BestMove1.Score
            GetBestMove.OldMoveX = BestMove1.OldMoveX
            GetBestMove.OldMoveY = BestMove1.OldMoveY
            GetBestMove.NewMoveX = BestMove1.NewMoveX
            GetBestMove.NewMoveY = BestMove1.NewMoveY
        ElseIf Pointer = 1 Then
            GetBestMove.Score = BestMove2.Score
            GetBestMove.OldMoveX = BestMove2.OldMoveX
            GetBestMove.OldMoveY = BestMove2.OldMoveY
            GetBestMove.NewMoveX = BestMove2.NewMoveX
            GetBestMove.NewMoveY = BestMove2.NewMoveY
        ElseIf Pointer = 2 Then
            GetBestMove.Score = BestMove3.Score
            GetBestMove.OldMoveX = BestMove3.OldMoveX
            GetBestMove.OldMoveY = BestMove3.OldMoveY
            GetBestMove.NewMoveX = BestMove3.NewMoveX
            GetBestMove.NewMoveY = BestMove3.NewMoveY
        ElseIf Pointer = 3 Then
            GetBestMove.Score = BestMove4.Score
            GetBestMove.OldMoveX = BestMove4.OldMoveX
            GetBestMove.OldMoveY = BestMove4.OldMoveY
            GetBestMove.NewMoveX = BestMove4.NewMoveX
            GetBestMove.NewMoveY = BestMove4.NewMoveY
        ElseIf Pointer = 4 Then
            GetBestMove.Score = BestMove5.Score
            GetBestMove.OldMoveX = BestMove5.OldMoveX
            GetBestMove.OldMoveY = BestMove5.OldMoveY
            GetBestMove.NewMoveX = BestMove5.NewMoveX
            GetBestMove.NewMoveY = BestMove5.NewMoveY
        ElseIf Pointer = 5 Then
            GetBestMove.Score = BestMove6.Score
            GetBestMove.OldMoveX = BestMove6.OldMoveX
            GetBestMove.OldMoveY = BestMove6.OldMoveY
            GetBestMove.NewMoveX = BestMove6.NewMoveX
            GetBestMove.NewMoveY = BestMove6.NewMoveY
        ElseIf Pointer = 6 Then
            GetBestMove.Score = BestMove7.Score
            GetBestMove.OldMoveX = BestMove7.OldMoveX
            GetBestMove.OldMoveY = BestMove7.OldMoveY
            GetBestMove.NewMoveX = BestMove7.NewMoveX
            GetBestMove.NewMoveY = BestMove7.NewMoveY
        Else
            GetBestMove.Score = BestMove8.Score
            GetBestMove.OldMoveX = BestMove8.OldMoveX
            GetBestMove.OldMoveY = BestMove8.OldMoveY
            GetBestMove.NewMoveX = BestMove8.NewMoveX
            GetBestMove.NewMoveY = BestMove8.NewMoveY
        End If
    End Function




    Private Sub WhiteAIMove_Click() Handles WhiteAIMove.Click
        If GameRunning AndAlso PlayerTurn Then
            Dim BestMove As NewMove
            Dim Tasks As New List(Of Task)
            Tasks.Add(New Task(AddressOf InitialiseThread1))
            Tasks.Add(New Task(AddressOf InitialiseThread2))
            Tasks.Add(New Task(AddressOf InitialiseThread3))
            Tasks.Add(New Task(AddressOf InitialiseThread4))
            Tasks.Add(New Task(AddressOf InitialiseThread5))
            Tasks.Add(New Task(AddressOf InitialiseThread6))
            Tasks.Add(New Task(AddressOf InitialiseThread7))
            Tasks.Add(New Task(AddressOf InitialiseThread8))

            GameRunning = False
            Progress = 0
            If AbsoluteDepth <> 0 Then
                ComputerIsSearching = True
                PositionsSearched = 0
                Stopwatch.Reset()
                Stopwatch.Start()
            End If
            Tasks.All(Function(t As Task)
                          t.Start()
                          Return True
                      End Function)
            While Tasks.Any(Function(t As Task) Not (t.Status = TaskStatus.Canceled OrElse t.Status = TaskStatus.Faulted OrElse t.Status = TaskStatus.RanToCompletion))
                Application.DoEvents()
                ProgressBar.Value = Progress
                'Console.WriteLine(BestMove1.Score * BestMove2.Score * BestMove3.Score * BestMove4.Score * BestMove5.Score * BestMove6.Score * BestMove7.Score * BestMove8.Score)
                If BestMove1.Score > 1000 Then
                    BestMove = BestMove1
                    Console.WriteLine("yooo")
                    Exit While
                ElseIf BestMove2.Score > 1000 Then
                    BestMove = BestMove2
                    Console.WriteLine("yooo")
                    Exit While
                ElseIf BestMove3.Score > 1000 Then
                    BestMove = BestMove3
                    Console.WriteLine("yooo")
                    Exit While
                ElseIf BestMove4.Score > 1000 Then
                    BestMove = BestMove4
                    Console.WriteLine("yooo")
                    Exit While
                ElseIf BestMove5.Score > 1000 Then
                    BestMove = BestMove5
                    Console.WriteLine("yooo")
                    Exit While
                ElseIf BestMove6.Score > 1000 Then
                    BestMove = BestMove6
                    Console.WriteLine("yooo")
                    Exit While
                ElseIf BestMove7.Score > 1000 Then
                    BestMove = BestMove7
                    Console.WriteLine("yooo")
                    Exit While
                ElseIf BestMove8.Score > 1000 Then
                    BestMove = BestMove8
                    Console.WriteLine("yooo")
                    Exit While

                End If
                Thread.Sleep(1)
            End While
            If AbsoluteDepth <> 0 Then
                Stopwatch.Stop()
                Console.WriteLine(vbCrLf & "Search Result: " & Stopwatch.Elapsed.TotalMilliseconds & " Milliseconds. " & PositionsSearched & " Positions Searched.")
            End If

            If BestMove.Score = vbEmpty Then BestMove = GetBestMove(True)
            If AbsoluteDepth = 0 Then
                If BestMove.Score = Integer.MinValue Then
                    If MasterWInCheck.IsInCheck Then
                        CheckLabel.Text = "Checkmate!"
                        Sound_Checkmate.Play()
                    Else
                        CheckLabel.Text = " Stalemate! "
                        Sound_Stalemate.Play()
                    End If
                Else
                    GameRunning = True
                End If
            Else
                GameRunning = True
                If MasterWInCheck.IsInCheck Then
                    MasterWInCheck.IsInCheck = False
                    MasterWInCheck.Piece1 = " "
                    MasterWInCheck.DoubleCheck = False
                End If
                MakeMove(MasterBoard, BestMove.OldMoveX, BestMove.OldMoveY, BestMove.NewMoveX, BestMove.NewMoveY, MasterWCanCastle, MasterWKPos, MasterMaterialCount, MasterEnPassant, True)

                DisplayPieces()
                FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
                CheckChecker()
                If MasterBInCheck.IsInCheck Then Sound_Check.Play()
                PlayerTurn = False
                AbsoluteDepth = 0
                BlackAIMove_Click()
                CalculateAbsoluteDepth()
                MasterMaterialCount = CountMaterial(MasterBoard, True)
                If MasterMaterialCount = 0 Then
                    CheckLabel.Text = "     Draw!     "
                    Sound_Stalemate.Play()
                    GameRunning = False
                ElseIf MasterMaterialCount = 3 Then
                    For y = 0 To 7
                        For x = 0 To 7
                            If MasterBoard(x, y) = "B" OrElse MasterBoard(x, y) = "N" Then
                                CheckLabel.Text = "     Draw!     "
                                Sound_Stalemate.Play()
                                GameRunning = False
                                Exit For
                            End If
                        Next
                        If Not GameRunning Then Exit For
                    Next
                End If
                MasterMaterialCount = CountMaterial(MasterBoard, False)
                PreviousFEN = CurrentFEN
                ConvertToFEN()
                CurrentEval.Text = BestMove.Score
                ProgressBar.Value = 0
                ComputerIsSearching = False

                Console.WriteLine("WKPos: " & MasterWKPos & ". BKPos: " & MasterBKPos)
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
            End If
        End If
    End Sub


    Private Function WhiteAI(ByRef x As Byte) As NewMove
        Dim CurrentMove As NewMove
        Dim BestMove As NewMove
        BestMove.Score = Integer.MinValue
        Dim alpha As Integer = Integer.MinValue
        Dim beta As Integer = Integer.MaxValue


        Dim PieceMoves = CreateMoves(MasterBoard, True, MasterWhiteTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterWCanCastle, MasterEnPassant, False, x)
        If PieceMoves(0, 0) > 0 Then
            Dim TempBoard(7, 7) As Char
            Dim TempMaterialCount As Int16
            Dim TempWKPos As String
            Dim TempWCanCastle As CanCastle
            Dim TempWInCheck As InCheck
            Dim TempEnPassant As String
            For n = 1 To CInt(PieceMoves(0, 0))
                If MasterWInCheck.IsInCheck AndAlso MasterBoard(x, PieceMoves(n, 1)) <> "K" Then
                    TempWInCheck.IsInCheck = True
                    TempWInCheck.Piece1 = MasterWInCheck.Piece1
                    TempWInCheck.DoubleCheck = MasterWInCheck.DoubleCheck
                    '#Disable Warning BC42104 'Variable is used before it has been assigned a value
                    If DoesMoveResolveCheck(MasterBoard, x, PieceMoves(n, 1), PieceMoves(n, 2)(0), PieceMoves(n, 2)(1), TempWInCheck, MasterBInCheck, MasterEnPassant) Then
                        TempWInCheck.Piece1 = " "
                        TempWInCheck.DoubleCheck = False
                    End If
                    '#Enable Warning BC42104 'Variable is used before it has been assigned a value
                Else
                    TempWInCheck.IsInCheck = False
                End If
                If Not TempWInCheck.IsInCheck Then
                    If AbsoluteDepth = 0 Then
                        BestMove.Score = 0
                        Exit For
                    End If
                    Array.Copy(MasterBoard, TempBoard, 64)
                    TempMaterialCount = MasterMaterialCount 'do i need these either?
                    TempWKPos = MasterWKPos
                    TempWCanCastle.KS = MasterWCanCastle.KS
                    TempWCanCastle.QS = MasterWCanCastle.QS
                    TempEnPassant = MasterEnPassant 'do i need this?
                    MakeMove(TempBoard, x, PieceMoves(n, 1), PieceMoves(n, 2)(0), PieceMoves(n, 2)(1), TempWCanCastle, TempWKPos, TempMaterialCount, TempEnPassant, False)
                    CurrentMove = MiniMax(TempBoard, AbsoluteDepth - 1, False, TempWCanCastle, MasterBCanCastle, TempWKPos, MasterBKPos, TempEnPassant, TempMaterialCount, Alpha, Beta, False)
                    'Console.WriteLine(x & y & PieceMoves(n)(0) & PieceMoves(n)(1) & CurrentMove.Score)
                    If CurrentMove.Score > BestMove.Score Then
                        BestMove.Score = CurrentMove.Score
                        BestMove.OldMoveX = x
                        BestMove.OldMoveY = PieceMoves(n, 1)
                        BestMove.NewMoveX = PieceMoves(n, 2)(0)
                        BestMove.NewMoveY = PieceMoves(n, 2)(1)
                    End If
                End If
            Next
        End If
        '#Disable Warning BC42109 ' Variable is used before it has been assigned a value
        Return BestMove
        '#Enable Warning BC42109 ' Variable is used before it has been assigned a value
    End Function


    Private Sub BlackAIMove_Click() Handles BlackAIMove.Click
        If GameRunning AndAlso Not PlayerTurn Then
            Dim Tasks As New List(Of Task)
            Tasks.Add(New Task(AddressOf InitialiseThread1))
            Tasks.Add(New Task(AddressOf InitialiseThread2))
            Tasks.Add(New Task(AddressOf InitialiseThread3))
            Tasks.Add(New Task(AddressOf InitialiseThread4))
            Tasks.Add(New Task(AddressOf InitialiseThread5))
            Tasks.Add(New Task(AddressOf InitialiseThread6))
            Tasks.Add(New Task(AddressOf InitialiseThread7))
            Tasks.Add(New Task(AddressOf InitialiseThread8))

            GameRunning = False
            Progress = 0
            If AbsoluteDepth <> 0 Then
                ComputerIsSearching = True
                PositionsSearched = 0
                Stopwatch.Reset()
                Stopwatch.Start()
            End If
            Tasks.All(Function(t As Task)
                          t.Start()
                          Return True
                      End Function)
            While Tasks.Any(Function(t As Task) Not (t.Status = TaskStatus.Canceled OrElse t.Status = TaskStatus.Faulted OrElse t.Status = TaskStatus.RanToCompletion))
                Application.DoEvents()
                ProgressBar.Value = Progress
                Thread.Sleep(1)
            End While
            If AbsoluteDepth <> 0 Then
                Stopwatch.Stop()
                Console.WriteLine(vbCrLf & "Search Result: " & Stopwatch.Elapsed.TotalMilliseconds & " Milliseconds. " & PositionsSearched & " Positions Searched.")
            End If

            Dim BestMove As NewMove = GetBestMove(False)
            If AbsoluteDepth = 0 Then
                If BestMove.Score = Integer.MaxValue Then
                    If MasterBInCheck.IsInCheck Then
                        CheckLabel.Text = "Checkmate!"
                        Sound_Checkmate.Play()
                    Else
                        CheckLabel.Text = "Stalemate!"
                        Sound_Stalemate.Play()
                    End If
                Else
                    GameRunning = True
                End If
            Else
                GameRunning = True
                If MasterBInCheck.IsInCheck Then
                    MasterBInCheck.IsInCheck = False
                    MasterBInCheck.Piece1 = " "
                    MasterBInCheck.DoubleCheck = False
                End If
                MakeMove(MasterBoard, BestMove.OldMoveX, BestMove.OldMoveY, BestMove.NewMoveX, BestMove.NewMoveY, MasterBCanCastle, MasterBKPos, MasterMaterialCount, MasterEnPassant, True)

                DisplayPieces()
                FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
                CheckChecker()
                If MasterWInCheck.IsInCheck Then Sound_Check.Play()
                PlayerTurn = True
                AbsoluteDepth = 0
                WhiteAIMove_Click()
                CalculateAbsoluteDepth()
                MasterMaterialCount = CountMaterial(MasterBoard, True)
                If MasterMaterialCount = 0 Then
                    CheckLabel.Text = "     Draw!     "
                    Sound_Stalemate.Play()
                    GameRunning = False
                ElseIf MasterMaterialCount = 3 Then
                    For y = 0 To 7
                        For x = 0 To 7
                            If MasterBoard(x, y) = "b" OrElse MasterBoard(x, y) = "n" Then
                                CheckLabel.Text = "     Draw!     "
                                Sound_Stalemate.Play()
                                GameRunning = False
                                Exit For
                            End If
                        Next
                        If Not GameRunning Then Exit For
                    Next
                End If
                MasterMaterialCount = CountMaterial(MasterBoard, False)
                PreviousFEN = CurrentFEN
                ConvertToFEN()
                CurrentEval.Text = BestMove.Score
                ProgressBar.Value = 0
                ComputerIsSearching = False

                Console.WriteLine("WKPos: " & MasterWKPos & ". BKPos: " & MasterBKPos)
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
            End If
        End If
    End Sub


    Private Function BlackAI(ByRef x As Byte) As NewMove
        Dim CurrentMove As NewMove
        Dim BestMove As NewMove
        BestMove.Score = Integer.MaxValue
        For y = 7 To 0 Step -1
            If Char.IsLower(MasterBoard(x, y)) Then
                Array.Copy(MasterTrueTable, TrueTable, 64)
                Dim PieceMoves = BlackPieceLegalMoves(MasterBoard, x, y, MasterBlackTFTable, TrueTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterBCanCastle, MasterEnPassant)
                If PieceMoves(0) IsNot Nothing Then
                    Dim TempBoard(7, 7) As Char
                    Dim TempMaterialCount As Int16
                    Dim TempBKPos As String
                    Dim TempBCanCastle As CanCastle
                    Dim TempBInCheck As InCheck
                    Dim TempEnPassant As String
                    For n = 1 To CInt(PieceMoves(0)) - 1
                        If MasterBInCheck.IsInCheck AndAlso MasterBoard(x, y) <> "k" Then
                            TempBInCheck.IsInCheck = True
                            TempBInCheck.Piece1 = MasterBInCheck.Piece1
                            TempBInCheck.DoubleCheck = MasterBInCheck.DoubleCheck
                            '#Disable Warning BC42104 'Variable is used before it has been assigned a value
                            If DoesMoveResolveCheck(MasterBoard, x, y, PieceMoves(n)(0), PieceMoves(n)(1), MasterWInCheck, TempBInCheck, MasterEnPassant) Then
                                TempBInCheck.Piece1 = " "
                                TempBInCheck.DoubleCheck = False
                            End If
                            '#Enable Warning BC42104 'Variable is used before it has been assigned a value
                        Else
                            TempBInCheck.IsInCheck = False
                        End If
                        If Not TempBInCheck.IsInCheck Then
                            If AbsoluteDepth = 0 Then
                                BestMove.Score = 0
                                Exit For
                            End If
                            Array.Copy(MasterBoard, TempBoard, 64)
                            TempMaterialCount = MasterMaterialCount
                            TempBKPos = MasterBKPos
                            TempBCanCastle.KS = MasterBCanCastle.KS
                            TempBCanCastle.QS = MasterBCanCastle.QS
                            TempEnPassant = MasterEnPassant 'do i need this?
                            MakeMove(TempBoard, x, y, PieceMoves(n)(0), PieceMoves(n)(1), TempBCanCastle, TempBKPos, TempMaterialCount, TempEnPassant, False)
                            CurrentMove = MiniMax(TempBoard, AbsoluteDepth - 1, True, MasterWCanCastle, TempBCanCastle, MasterWKPos, TempBKPos, TempEnPassant, TempMaterialCount, Integer.MinValue, Integer.MaxValue, False)
                            'Console.WriteLine(x & y & PieceMoves(n)(0) & PieceMoves(n)(1) & CurrentMove.Score)
                            If CurrentMove.Score < BestMove.Score Then
                                BestMove.Score = CurrentMove.Score
                                BestMove.OldMoveX = x
                                BestMove.OldMoveY = y
                                BestMove.NewMoveX = PieceMoves(n)(0)
                                BestMove.NewMoveY = PieceMoves(n)(1)
                            End If
                        End If
                    Next
                End If
            End If
        Next
        '#Disable Warning BC42109 ' Variable is used before it has been assigned a value
        Return BestMove
        '#Enable Warning BC42109 ' Variable is used before it has been assigned a value
    End Function


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim hi(300, 2) As String
        hi = CreateMoves(MasterBoard, True, MasterWhiteTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterWCanCastle, MasterEnPassant, False, 5)
        For x = 0 To CInt(hi(0, 0))
            Console.WriteLine(hi(x, 0) & hi(x, 1) & hi(x, 2))
        Next
    End Sub

    'Function which generates all the posible moves in a given position, orders them based off their predicted evaluation (hest
    'moves to worst moves), then stores all the results in the array Moves. By searching the best moves first, alpha-beta
    'pruning will be able to prune more branches, resulting in a more efficient search.
    Private Function CreateMoves(ByRef Board(,) As Char, ByRef isWhite As Boolean, ByVal TrueFalseTable(,) As Char, ByVal KPos As String, ByVal WInCheck As InCheck, ByVal BInCheck As InCheck, ByVal CanCastle As CanCastle, ByVal EnPassant As String, ByVal OnlyCaptures As Boolean, ByVal xValue As SByte) As String(,)
        Dim AmazingCaptureMoves(300, 2) As String
        Dim CaptureMoves(49, 2) As String
        Dim OtherCaptureMoves(49, 2) As String
        Dim PawnPromotionMoves(7, 2) As String
        Dim GoodMoves(49, 2) As String
        Dim Moves(99, 2) As String
        Dim BadMoves(49, 2) As String
        Dim TerribleMoves(49, 2) As String

        Dim ACMLen As UInt16 = 1
        Dim CMLen As Byte
        Dim OCMLen As Byte
        Dim PPMLen As Byte
        Dim GMLen As Byte
        Dim MLen As Byte
        Dim BMLen As Byte
        Dim TMLen As Byte

        Dim dx As Byte
        Dim dy As Byte
        Dim PieceValueDif As SByte
        Dim StepCount As Byte = 1
        If xValue >= 0 Then StepCount = 8 Else xValue = 0
        If isWhite Then
            For y = 0 To 7
                For x = xValue To 7 Step StepCount
                    If Char.IsUpper(Board(x, y)) Then
                        Array.Copy(MasterTrueTable, TrueTable, 64)
                        Dim PieceMoves = WhitePieceLegalMoves(Board, x, y, TrueFalseTable, TrueTable, KPos, BInCheck, WInCheck, CanCastle, EnPassant)
                        If PieceMoves(0) IsNot Nothing Then
                            For n = 1 To CInt(PieceMoves(0)) - 1
                                If Board(CStr(PieceMoves(n)(0)), CStr(PieceMoves(n)(1))) <> " " Then
                                    PieceValueDif = PieceValue(Asc(UCase(Board(CStr(PieceMoves(n)(0)), CStr(PieceMoves(n)(1))))) Mod 11) - PieceValue(Asc(UCase(Board(x, y))) Mod 11)
                                    If PieceValueDif >= 0 Then
                                        AmazingCaptureMoves(ACMLen, 0) = x
                                        AmazingCaptureMoves(ACMLen, 1) = y
                                        AmazingCaptureMoves(ACMLen, 2) = PieceMoves(n)
                                        ACMLen += 1
                                    ElseIf PieceValueDif = 0 Then
                                        CaptureMoves(CMLen, 0) = x
                                        CaptureMoves(CMLen, 1) = y
                                        CaptureMoves(CMLen, 2) = PieceMoves(n)
                                        CMLen += 1
                                    Else
                                        OtherCaptureMoves(OCMLen, 0) = x
                                        OtherCaptureMoves(OCMLen, 1) = y
                                        OtherCaptureMoves(OCMLen, 2) = PieceMoves(n)
                                        OCMLen += 1
                                    End If
                                ElseIf Not OnlyCaptures Then
                                    dx = Math.Abs(CStr(KPos(0)) - CStr(PieceMoves(n)(0)))
                                    dy = Math.Abs(CStr(KPos(1)) - CStr(PieceMoves(n)(1)))
                                    If PieceMoves(n)(1) = "0" AndAlso Board(x, y) = "P" Then
                                        PawnPromotionMoves(PPMLen, 0) = x
                                        PawnPromotionMoves(PPMLen, 1) = y
                                        PawnPromotionMoves(PPMLen, 2) = PieceMoves(n)
                                        PPMLen += 1
                                    ElseIf Board(Math.Max(CStr(PieceMoves(n)(0)) - 1, 0), Math.Max(CStr(PieceMoves(n)(1)) - 1, 0)) = "p" OrElse Board(Math.Min(CStr(PieceMoves(n)(0)) + 1, 7), Math.Max(CStr(PieceMoves(n)(1)) - 1, 0)) = "p" Then
                                        TerribleMoves(TMLen, 0) = x
                                        TerribleMoves(TMLen, 1) = y
                                        TerribleMoves(TMLen, 2) = PieceMoves(n)
                                        TMLen += 1
                                    ElseIf TrueFalseTable(CStr(PieceMoves(n)(0)), CStr(PieceMoves(n)(1))) = "F" Then
                                        BadMoves(BMLen, 0) = x
                                        BadMoves(BMLen, 1) = y
                                        BadMoves(BMLen, 2) = PieceMoves(n)
                                        BMLen += 1
                                    ElseIf (Board(x, y) <> "P" AndAlso Board(x, y) <> "K") AndAlso (dx = 0 OrElse dy = 0 OrElse dx = dy) Then
                                        GoodMoves(GMLen, 0) = x
                                        GoodMoves(GMLen, 1) = y
                                        GoodMoves(GMLen, 2) = PieceMoves(n)
                                        GMLen += 1
                                    Else
                                        Moves(MLen, 0) = x
                                        Moves(MLen, 1) = y
                                        Moves(MLen, 2) = PieceMoves(n)
                                        MLen += 1
                                    End If
                                End If

                            Next
                        End If
                    End If
                Next
            Next
        Else
            For Y = 0 To 7
                For x = 0 To 7
                    'If Char.IsUpper(Board(x, y)) Then
                    '    Array.Copy(MasterTrueTable, TrueTable, 64)
                    '    PieceMoves = BlackPieceLegalMoves(Board, x, y, BlackTFTable, WhiteTFTable, WKPos, WInCheck, BInCheck)
                    '    If PieceMoves(0) IsNot Nothing Then
                    '        Moves(Counter, 0) = x
                    '        Moves(Counter, 1) = y
                    '        For n = 1 To CInt(PieceMoves(0)) - 1
                    '            'If Not OnlyCaptures OrElse Board(LegalMoves(n)(0), LegalMoves(n)(1)) <> " " Then Moves(counter, n) = LegalMoves(n)
                    '        Next
                    '        Counter += 1
                    '    End If
                    'End If
                Next
            Next
        End If

        Array.Copy(CaptureMoves, 0, AmazingCaptureMoves, 3 * ACMLen, 3 * CMLen)
        Array.Copy(OtherCaptureMoves, 0, AmazingCaptureMoves, 3 * (ACMLen + CMLen), 3 * OCMLen)
        ACMLen += CMLen + OCMLen
        If Not OnlyCaptures Then
            If PPMLen > 0 Then Array.Copy(PawnPromotionMoves, 0, AmazingCaptureMoves, 3 * ACMLen, 3 * PPMLen)
            Array.Copy(GoodMoves, 0, AmazingCaptureMoves, 3 * (ACMLen + PPMLen), 3 * GMLen)
            Array.Copy(Moves, 0, AmazingCaptureMoves, 3 * (ACMLen + PPMLen + GMLen), 3 * MLen)
            Array.Copy(BadMoves, 0, AmazingCaptureMoves, 3 * (ACMLen + PPMLen + GMLen + MLen), 3 * BMLen)
            Array.Copy(TerribleMoves, 0, AmazingCaptureMoves, 3 * (ACMLen + PPMLen + GMLen + MLen + BMLen), 3 * TMLen)
            ACMLen += GMLen + MLen + BMLen + TMLen
        End If
        AmazingCaptureMoves(0, 0) = ACMLen - 1

        Return AmazingCaptureMoves
    End Function


    'Subroutine that makes a move on the board, given coordinates. Includes castling (& rights) and pawn promotion.
    Sub MakeMove(ByRef Board(,) As Char, ByRef OldCoorX As String, ByRef OldCoorY As String, ByRef NewCoorX As String, ByRef NewCoorY As String, ByRef CanCastle As CanCastle, ByRef KPos As String, ByRef MaterialCount As Int16, ByRef EnPassant As String, ByRef MakeSounds As Boolean)
        Dim TempPiece As Char = Board(OldCoorX, OldCoorY)
        If MakeSounds Then Sound_Move.Play()
        If Char.IsUpper(TempPiece) Then
            If TempPiece = "P" Then
                'Code for Promoting Pawns and En Passant. Also increments the material count.
                If NewCoorY = 0 Then
                    TempPiece = "Q"
                    MaterialCount += PieceValue(4) - PieceValue(3) '+ 9 for a new queen, - 1 for losing the pawn in the process.
                ElseIf NewCoorX & NewCoorY = EnPassant Then
                    Board(NewCoorX, NewCoorY + 1) = " "
                    MaterialCount += PieceValue(3)
                    If MakeSounds Then Sound_Capture.Play()
                End If
                'If piece is a Rook, part of Castling is disabled (depending on which Rook has moved).
            ElseIf TempPiece = "R" AndAlso (CanCastle.KS OrElse CanCastle.QS) Then
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
                CanCastle.KS = False
                CanCastle.QS = False
            End If
        Else
            'Identical Code for the Black Pieces.
            If TempPiece = "p" Then
                If NewCoorY = 7 Then
                    TempPiece = "q"
                    MaterialCount -= PieceValue(4) - PieceValue(3)
                ElseIf NewCoorX & NewCoorY = EnPassant Then
                    Board(NewCoorX, NewCoorY - 1) = " "
                    MaterialCount -= PieceValue(3)
                    If MakeSounds Then Sound_Capture.Play()
                End If
            ElseIf TempPiece = "r" AndAlso (CanCastle.KS OrElse CanCastle.QS) Then
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
                CanCastle.KS = False
                CanCastle.QS = False
            End If
        End If
        'En Passant Creation.
        If EnPassant <> "-" AndAlso UCase(TempPiece) = "P" Then
            If OldCoorY = 1 AndAlso NewCoorY = 3 Then
                If Board(Math.Max(NewCoorX - 1, 0), 3) = "P" OrElse Board(Math.Min(NewCoorX + 1, 7), 3) = "P" Then EnPassant = NewCoorX & 2
            ElseIf OldCoorY = 6 AndAlso NewCoorY = 4 Then
                If Board(Math.Max(NewCoorX - 1, 0), 4) = "p" OrElse Board(Math.Min(NewCoorX + 1, 7), 4) = "p" Then EnPassant = NewCoorX & 5
            End If
        Else
            EnPassant = "-"
        End If

        'At the end of the subroutine, the Piece is placed at the new coordinates, and the old position is cleared.
        'If the new position contains a piece, then the material count is updated for only that piece.
        If Board(NewCoorX, NewCoorY) <> " " Then
            If Char.IsUpper(Board(NewCoorX, NewCoorY)) Then
                If MakeSounds Then Sound_Capture.Play()
                MaterialCount -= PieceValue(Asc(UCase(Board(NewCoorX, NewCoorY))) Mod 11)
            Else
                If MakeSounds Then Sound_Capture.Play()
                MaterialCount += PieceValue(Asc(UCase(Board(NewCoorX, NewCoorY))) Mod 11)
            End If
        End If
        Board(NewCoorX, NewCoorY) = TempPiece
        Board(OldCoorX, OldCoorY) = " "
    End Sub


    'Function that evaluates a Current Board Position. Takes into account material, 
    Function Evaluate(ByRef Board(,) As Char, ByRef MaterialCount As Int16) As Decimal ', ByRef WhiteTFTable(,) As Char, ByRef BlackTFTable(,) As Char, 
        Evaluate = MaterialCount
    End Function



    'Algorithm which calculates the Overall Depth the AI will run at. Takes into account the Total Material Count
    'on the board and calculates Depth using the formula y = -m*ln(x-a) + c (the higher the Material Count, the lower
    'the Depth).
    Private Sub CalculateAbsoluteDepth()
        Dim TotalMaterial As Int16 = CountMaterial(MasterBoard, True)
        Dim DepthAlgorithm As Int16 = Math.Truncate((-2 * Math.Log(TotalMaterial + 0.01)) + 14)
        '(-TotalMaterial / 18) + 10
        'If the previous AI search resulted in a forced Checkmate being found, the depth is limited to only the
        'depth that is required to achieve that Checkmate. This saves on a lot of unnecessary processing, as
        'forced Checkmates are unavoidable.
        If CInt(CurrentEval.Text) > 1000 Then
            AbsoluteDepth = DepthAlgorithm - CStr(CurrentEval.Text(3))
        ElseIf CInt(CurrentEval.Text) < -1000 Then
            AbsoluteDepth = DepthAlgorithm - CStr(CurrentEval.Text(4))
        Else
            AbsoluteDepth = DepthAlgorithm
        End If
        'The Math.Max function is used to ensure that the Depth never gets lower than 2. Along with general quality
        'improvements, this also prevents AbsoluteDepth being 0 or < 0, which could trigger unexpected code.
        AbsoluteDepth = Math.Max(AbsoluteDepth, 2)
        Label1.Text = TotalMaterial & " - " & AbsoluteDepth
    End Sub

    Public Function MiniMax(ByVal Board(,) As Char, ByVal depth As SByte, ByVal isWhite As Boolean, ByVal WCanCastle As CanCastle, ByVal BCanCastle As CanCastle, ByVal WKPos As String, ByVal BKPos As String, ByVal EnPassant As String, ByVal MaterialCount As Int16, ByVal Alpha As Decimal, ByVal Beta As Decimal, ByVal OnlyCaptures As Boolean) As NewMove
        PositionsSearched += 1
        Dim CurrentMove As NewMove
        If depth > 0 Then
            Dim BestMove As NewMove
            Dim WInCheck As InCheck
            WInCheck.IsInCheck = False 'WHERE DO I PUT THESE????
            WInCheck.Piece1 = " "
            WInCheck.DoubleCheck = False
            Dim BInCheck As InCheck
            BInCheck.IsInCheck = False
            BInCheck.Piece1 = " "
            BInCheck.DoubleCheck = False
            If isWhite Then
                BestMove.Score = Integer.MinValue
                Dim WhiteTFTable(7, 7) As Char
                'TempWInCheck.IsInCheck = False
                FixTFTables(Board, True, WhiteTFTable, WKPos, WInCheck, NotInCheck, EnPassant)
                For y = 0 To 7
                    For x = 0 To 7
                        If Char.IsUpper(Board(x, y)) Then
                            Array.Copy(MasterTrueTable, TrueTable, 64)
                            Dim PieceMoves = WhitePieceLegalMoves(Board, x, y, WhiteTFTable, TrueTable, BKPos, BInCheck, WInCheck, WCanCastle, EnPassant)
                            If PieceMoves(0) IsNot Nothing Then
                                Dim TempBoard(7, 7) As Char
                                Dim TempMaterialCount As Int16
                                Dim TempWKPos As String
                                Dim TempWCanCastle As CanCastle
                                Dim TempWInCheck As InCheck
                                Dim TempEnPassant As String
                                For n = 1 To CInt(PieceMoves(0)) - 1
                                    If WInCheck.IsInCheck AndAlso Board(x, y) <> "K" Then
                                        TempWInCheck.IsInCheck = True
                                        TempWInCheck.Piece1 = WInCheck.Piece1
                                        TempWInCheck.DoubleCheck = WInCheck.DoubleCheck
                                        If DoesMoveResolveCheck(Board, x, y, PieceMoves(n)(0), PieceMoves(n)(1), TempWInCheck, BInCheck, EnPassant) Then
                                            TempWInCheck.Piece1 = " "
                                            TempWInCheck.DoubleCheck = False
                                        End If
                                    Else
                                        TempWInCheck.IsInCheck = False
                                    End If
                                    If Not TempWInCheck.IsInCheck Then
                                        Array.Copy(Board, TempBoard, 64)
                                        TempMaterialCount = MaterialCount 'do i need these either?
                                        TempWKPos = WKPos
                                        TempWCanCastle.KS = WCanCastle.KS
                                        TempWCanCastle.QS = WCanCastle.QS
                                        TempEnPassant = EnPassant 'do i need this?
                                        MakeMove(TempBoard, x, y, PieceMoves(n)(0), PieceMoves(n)(1), TempWCanCastle, TempWKPos, TempMaterialCount, TempEnPassant, False)
                                        CurrentMove = MiniMax(TempBoard, depth - 1, False, TempWCanCastle, BCanCastle, TempWKPos, BKPos, TempEnPassant, TempMaterialCount, Alpha, Beta, False)
                                        If CurrentMove.Score > BestMove.Score Then
                                            BestMove.Score = CurrentMove.Score
                                            BestMove.OldMoveX = x
                                            BestMove.OldMoveY = y
                                            BestMove.NewMoveX = PieceMoves(n)(0)
                                            BestMove.NewMoveY = PieceMoves(n)(1)
                                        End If
                                        Alpha = Math.Max(Alpha, CurrentMove.Score)
                                        If Beta <= Alpha Then
                                            '#Disable Warning BC42109 ' Variable is used before it has been assigned a value
                                            Return BestMove
                                            '#Enable Warning BC42109 ' Variable is used before it has been assigned a value
                                            Exit Function
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    Next
                Next
            Else
                BestMove.Score = Integer.MaxValue
                Dim BlackTFTable(7, 7) As Char
                'TempBInCheck.IsInCheck = False
                FixTFTables(Board, False, BlackTFTable, BKPos, NotInCheck, BInCheck, EnPassant)
                For y = 7 To 0 Step -1
                    For x = 0 To 7
                        If Char.IsLower(Board(x, y)) Then
                            Array.Copy(MasterTrueTable, TrueTable, 64)
                            Dim PieceMoves = BlackPieceLegalMoves(Board, x, y, BlackTFTable, TrueTable, WKPos, WInCheck, BInCheck, BCanCastle, EnPassant)
                            If PieceMoves(0) IsNot Nothing Then
                                Dim TempBoard(7, 7) As Char
                                Dim TempMaterialCount As Int16
                                Dim TempBKPos As String
                                Dim TempBCanCastle As CanCastle
                                Dim TempBInCheck As InCheck
                                Dim TempEnPassant As String
                                For n = 1 To CInt(PieceMoves(0)) - 1
                                    If BInCheck.IsInCheck AndAlso Board(x, y) <> "k" Then
                                        TempBInCheck.IsInCheck = True
                                        TempBInCheck.Piece1 = BInCheck.Piece1
                                        TempBInCheck.DoubleCheck = BInCheck.DoubleCheck
                                        If DoesMoveResolveCheck(Board, x, y, PieceMoves(n)(0), PieceMoves(n)(1), WInCheck, TempBInCheck, EnPassant) Then
                                            TempBInCheck.Piece1 = " "
                                            TempBInCheck.DoubleCheck = False
                                        End If
                                    Else
                                        TempBInCheck.IsInCheck = False
                                    End If
                                    If Not TempBInCheck.IsInCheck Then
                                        Array.Copy(Board, TempBoard, 64)
                                        TempMaterialCount = MaterialCount
                                        TempBKPos = BKPos
                                        TempBCanCastle.KS = BCanCastle.KS
                                        TempBCanCastle.QS = BCanCastle.QS
                                        TempEnPassant = EnPassant
                                        MakeMove(TempBoard, x, y, PieceMoves(n)(0), PieceMoves(n)(1), TempBCanCastle, TempBKPos, TempMaterialCount, TempEnPassant, False)
                                        CurrentMove = MiniMax(TempBoard, depth - 1, True, WCanCastle, TempBCanCastle, WKPos, TempBKPos, TempEnPassant, TempMaterialCount, Alpha, Beta, False)
                                        If CurrentMove.Score < BestMove.Score Then
                                            BestMove.Score = CurrentMove.Score
                                            BestMove.OldMoveX = x
                                            BestMove.OldMoveY = y
                                            BestMove.NewMoveX = PieceMoves(n)(0)
                                            BestMove.NewMoveY = PieceMoves(n)(1)
                                        End If
                                        Beta = Math.Min(Beta, CurrentMove.Score)
                                        If Beta <= Alpha Then
                                            Return BestMove
                                            Exit Function
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    Next
                Next
            End If

            If BestMove.Score = Integer.MinValue Then
                If WInCheck.IsInCheck Then
                    BestMove.Score = -(1000 + depth)
                Else
                    BestMove.Score = 0
                End If
            ElseIf BestMove.Score = Integer.MaxValue Then
                If BInCheck.IsInCheck Then
                    BestMove.Score = (1000 + depth)
                Else
                    BestMove.Score = 0
                End If
            End If
            Return BestMove
        Else
            CurrentMove.Score = Evaluate(Board, MaterialCount)
            '#Disable Warning BC42109 ' Variable is used before it has been assigned a value
            Return CurrentMove
            '#Enable Warning BC42109 ' Variable is used before it has been assigned a value
        End If
        '#Enable Warning BC42108 ' Variable is passed by reference before it has been assigned a value
    End Function


End Class


'Tasks left to do:
'En passant
'Drawing rules
'Threefold repetition
'50 move stalemate
'Ai vs ai For title screen Of chess ai
'En passant
'Transposition table
'Main menu
'Move ordering from fen
'Minimax move ordering
'Multithreading
'Remembering analysis And continuing


'Evaluation: (use stock fish Eval?)

'Middlegame:
'Material
'King safety
'Space / activity of pieces
'Controlling opponents squares
'Doubled pawns
'Isolated pawns / pawn islands?
'Restrict opponent King movement? 

'Endgame:
'Material
'King activity
'Restricting oppenent King movement
'Doubled pawns(Higher multiplier)
'How advanced pawns are
'Connected past pawns
