Public Class Chess

    'Working Version of Chess, along with AI using MiniMax and Alpha Beta Pruning. Created by Alfie Kunz.

    'Here are the computer's primary "eyes" - MasterBoard (which contains each piece's location on the board) and,
    'derived from this, Master TrueFalse Tables for each player. These Tables display information such as Legal
    'Moves for the players' kings, along with pinned pieces and their location, and make handling Move Generation,
    'Checks and Evaluations much easier and more efficient.
    Public GameRunning As Boolean = True
    Public MasterBoard(7, 7) As Char
    Public MasterWhiteTFTable(7, 7) As Char
    Public MasterBlackTFTable(7, 7) As Char

    'TrueTables are just TrueFalse Tables containing only the letter T. Very useful for resetting TrueFalse Tables
    'and debugging.
    Public MasterTrueTable(7, 7) As Char
    Public TrueTable(7, 7) As Char
    ReadOnly PieceArray(45) As PictureBox
    'Standard Chess notation that displays where all the pieces on the board are supposed to go.
    ReadOnly StartingFEN As String = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
    Public PlayerTurn As Boolean = True
    Public MasterWKPos As String
    Public MasterBKPos As String
    Public MasterMaterialCount As Int16
    Public AbsoluteDepth As Byte


    Public Enum PieceValue 'Structure Containing the Value or Weight of each Piece.
        'K = 100
        Q = 9
        R = 5
        B = 3
        N = 3
        P = 1
    End Enum

    'Structures containing information about castling + checks.
    Structure CanCastle
        Dim KS As Boolean 'King Side Castling
        Dim QS As Boolean 'Queen Side Castling
    End Structure
    Public MasterWCanCastle As CanCastle
    Public MasterBCanCastle As CanCastle
    Structure InCheck
        Dim IsInCheck As Boolean
        Dim Piece1 As String
        Dim Piece2 As String
    End Structure
    Public MasterWInCheck As InCheck
    Public MasterBInCheck As InCheck

    'Sets up sound effects.
    Dim Sound_Startup As New System.Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Chess_Startup.wav"
    }
    Dim Sound_Move As New System.Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Chess_Move.wav"
    }
    Dim Sound_Capture As New System.Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Chess_Capture.wav"
    }
    Dim Sound_Check As New System.Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Chess_Check.wav"
    }
    Dim Sound_Castle As New System.Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Chess_Castle.wav"
    }
    Dim Sound_Checkmate As New System.Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Chess_Checkmate.wav"
    }
    Dim Sound_Stalemate As New System.Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Chess_Stalemate.wav"
    }

    Dim Stopwatch As New Stopwatch





    Public Sub Form1_Load(sender As Object, e As System.EventArgs) Handles Me.Load
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
        'Sends the starting FEN to be converted and displayed on the screen, then resets check / castling properties.
        MasterBoard = FENConverter(StartingFEN)
        MasterWCanCastle.KS = True
        MasterWCanCastle.KS = True
        MasterWCanCastle.KS = True
        MasterWCanCastle.KS = True
        MasterWInCheck.Piece1 = " "
        MasterWInCheck.Piece2 = " "
        MasterBInCheck.Piece1 = " "
        MasterBInCheck.Piece2 = " "
        'Creates TrueTables, then displays the pieces on the board.
        For x = 0 To 7
            For y = 0 To 7
                MasterTrueTable(x, y) = "T"
            Next
        Next
        Array.Copy(MasterTrueTable, TrueTable, 64)
        FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterBKPos)
        Array.Copy(TrueTable, MasterBlackTFTable, 64)

        ''Runs the AI on a low-depth, meaning that its first official search will be much more efficient.
        'AbsoluteDepth = 1
        'BlackAIMove_Click()
        'Console.Clear()
        'MasterBoard = FENConverter(StartingFEN)
        'Array.Copy(MasterTrueTable, TrueTable, 64)
        'FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterBKPos)
        'Array.Copy(TrueTable, MasterBlackTFTable, 64)

        MasterMaterialCount = CountMaterial(MasterBoard)
        CurrentEval.Text = MasterMaterialCount
        CheckLabel.Text = "            "
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


    'Alternate version of DisplayPieces() which is more efficient and only converts a single piece in MasterBoard() to its piece counterpart on the board.
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
                If CInt(TempPiece.Location.X) = CoorX And CInt(TempPiece.Location.Y) = CoorY And TempPiece.Name(0) <> PieceColour Then
                    Exit While
                End If
                'If not found, the search continues with an increasing last digit (eg: the program seaches for BP1, then BP2, then BP3 ext until it finds its match.)
                n += 1
            End While
        End If
#Disable Warning BC42104 'Variable is used before it has been assigned a value
        Return TempPiece
#Enable Warning BC42104 'Variable is used before it has been assigned a value
    End Function


    'Function which puts the required pieces in a FEN board position into an 8x8 board array.
    Private Function FENConverter(ByVal FEN As String) As Char(,)
        MasterWCanCastle.KS = False
        MasterWCanCastle.QS = False
        MasterBCanCastle.KS = False
        MasterBCanCastle.QS = False
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
                        End Select
                    Next
                    'To do: En Passant & 50 Move Stalemate.
                    Exit For
            End Select
        Next
        Return tempArray
    End Function


    'Function which converts the current board position into its FEN counterpart.
    Private Sub FENExport_Click(sender As Object, e As EventArgs) Handles FENExport.Click
        Dim FEN As String = ""
        Dim Counter As Byte = 0
        For y = 0 To 7
            For x = 0 To 7
                If MasterBoard(x, y) = " " Then
                    Counter += 1
                Else
                    If Counter > 0 Then
                        FEN &= Counter
                        Counter = 0
                    End If
                    FEN &= MasterBoard(x, y)
                End If
            Next
            If Counter > 0 Then FEN &= Counter
            FEN &= "/"
            Counter = 0
        Next
        FEN = FEN.TrimEnd("/")
        If PlayerTurn Then
            FEN &= " w "
        Else
            FEN &= " b "
        End If
        If MasterWCanCastle.KS Then FEN &= "K"
        If MasterWCanCastle.QS Then FEN &= "Q"
        If MasterBCanCastle.KS Then FEN &= "k"
        If MasterBCanCastle.QS Then FEN &= "q"
        FEN &= " - 0 1"
        FENTextBox.Text = FEN
    End Sub


    'Function which counts up all the material (and their values) on the board and subtracts black's total from
    'white's total. Rough metric as to who is winning.
    Private Function CountMaterial(ByRef Board(,) As Char) As Int16
        Dim TempPiece As Char
        CountMaterial = 0
        For y = 0 To 7
            For x = 0 To 7
                If Board(x, y) <> " " And UCase(Board(x, y)) <> "K" Then
                    TempPiece = Board(x, y)
                    If UCase(Board(x, y)) = Board(x, y) Then
                        If TempPiece = "Q" Then CountMaterial += PieceValue.Q
                        If TempPiece = "R" Then CountMaterial += PieceValue.R
                        If TempPiece = "B" Then CountMaterial += PieceValue.B
                        If TempPiece = "N" Then CountMaterial += PieceValue.N
                        If TempPiece = "P" Then CountMaterial += PieceValue.P
                    Else
                        If TempPiece = "q" Then CountMaterial -= PieceValue.Q
                        If TempPiece = "r" Then CountMaterial -= PieceValue.R
                        If TempPiece = "b" Then CountMaterial -= PieceValue.B
                        If TempPiece = "n" Then CountMaterial -= PieceValue.N
                        If TempPiece = "p" Then CountMaterial -= PieceValue.P
                    End If
                End If
            Next
        Next
    End Function


    'Sends the FEN a user types in to be converted & displayed.
    Private Sub FENButton_Click(sender As Object, e As EventArgs) Handles FENButton.Click
        'Resets Check and Castling Properties.
        If FENTextBox.Text <> "" Then
            MasterWInCheck.IsInCheck = False
            MasterWInCheck.Piece1 = " "
            MasterWInCheck.Piece2 = " "
            MasterBInCheck.IsInCheck = False
            MasterBInCheck.Piece1 = " "
            MasterBInCheck.Piece2 = " "
            MasterBoard = FENConverter(FENTextBox.Text)
            DisplayPieces()
            'Resets TrueFalse Tables, then checks for Checks.
            If PlayerTurn Then
                FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterBKPos)
            Else
                FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterWKPos, MasterBKPos)
            End If
            CheckChecker()
            MasterMaterialCount = CountMaterial(MasterBoard)
            CurrentEval.Text = MasterMaterialCount
            GameRunning = True
            'Sound_Move.Play()

            AbsoluteDepth = 0
            If PlayerTurn Then
                'RUN WHITE AI.
            Else
                BlackAIMove_Click()
            End If
            CalculateAbsoluteDepth()
        End If
    End Sub

    'Resets the board to the starting position and sets white to move.
    Private Sub Reset_Btn_Click(sender As Object, e As EventArgs) Handles Reset_Btn.Click
        'Resets Check Properties.
        MasterWInCheck.IsInCheck = False
        MasterWInCheck.Piece1 = " "
        MasterWInCheck.Piece2 = " "
        MasterBInCheck.IsInCheck = False
        MasterBInCheck.Piece1 = " "
        MasterBInCheck.Piece2 = " "
        MasterBoard = FENConverter("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        DisplayPieces()
        'Resets TrueFalse Table, then check for Checks.
        FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterBKPos)
        CheckChecker()
        MasterMaterialCount = 0
        CurrentEval.Text = MasterMaterialCount
        GameRunning = True
        'Sound_Move.Play()
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
    Function WhitePieceLegalMoves(ByRef Board(,) As Char, ByVal CoorX As Byte, ByVal CoorY As Byte, ByRef WhiteTFTable(,) As Char, ByRef BlackTFTable(,) As Char, ByRef BKPos As String, ByRef BInCheck As InCheck, ByRef WInCheck As InCheck, ByRef WCanCastle As CanCastle) As String()
        Dim LegalMoveArray(27) As String
        Dim n As Byte = 1
        Dim StartY As SByte
        Dim EndY As SByte
        Dim StartX As SByte
        Dim EndX As SByte
        Dim CheckingPiece As String = " "

        'If the King is in a Double Check situation, then only King Moves will be accepted.
        'This makes checks much more efficient.
        If WInCheck.Piece2 <> " " And Board(CoorX, CoorY) <> "K" Then
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
                    If (Board(x, y) = " " Or (Board(x, y) >= "b" And Board(x, y) <= "r")) And WhiteTFTable(x, y) = "T" Then
                        LegalMoveArray(n) = (x) & (y)
                        n += 1
                    End If
                Next
            Next
            If WCanCastle.KS And WInCheck.IsInCheck = False Then
                If Board(CoorX + 1, CoorY) = " " And Board(CoorX + 2, CoorY) = " " And Board(CoorX + 3, CoorY) = "R" And WhiteTFTable(CoorX + 1, CoorY) = "T" And WhiteTFTable(CoorX + 2, CoorY) = "T" Then
                    LegalMoveArray(n) = (CoorX + 2) & (CoorY)
                    n += 1
                End If
            End If
            If WCanCastle.QS And WInCheck.IsInCheck = False Then
                If Board(CoorX - 1, CoorY) = " " And Board(CoorX - 2, CoorY) = " " And Board(CoorX - 3, CoorY) = " " And Board(CoorX - 4, CoorY) = "R" And WhiteTFTable(CoorX - 1, CoorY) = "T" And WhiteTFTable(CoorX - 2, CoorY) = "T" Then
                    LegalMoveArray(n) = (CoorX - 2) & (CoorY)
                    n += 1
                End If
            End If


        ElseIf Board(CoorX, CoorY) = "P" Then 'Legal Moves for the Pawn (along with First Moves).
            If WhiteTFTable(CoorX, CoorY) <> "2" Then
                If WhiteTFTable(CoorX, CoorY) <> "1" And WhiteTFTable(CoorX, CoorY) <> "3" Then
                    If Board(CoorX, CoorY - 1) = " " Then
                        LegalMoveArray(n) = CoorX & CoorY - 1
                        n += 1
                        If CoorY = 6 And Board(CoorX, 4) = " " Then
                            LegalMoveArray(n) = CoorX & 4
                            n += 1
                        End If
                    End If
                End If
                If WhiteTFTable(CoorX, CoorY) <> "0" And WhiteTFTable(CoorX, CoorY) <> "1" And CoorX > 0 Then
                    If BlackTFTable(CoorX - 1, CoorY - 1) = "T" Then BlackTFTable(CoorX - 1, CoorY - 1) = "F"
                    If (Board(CoorX - 1, CoorY - 1) >= "b" And Board(CoorX - 1, CoorY - 1) <= "r") Then
                        LegalMoveArray(n) = CoorX - 1 & CoorY - 1
                        If Board(CoorX - 1, CoorY - 1) = "k" Then
                            BInCheck.IsInCheck = True
                            CheckingPiece = CoorX & CoorY
                        End If
                        n += 1
                    End If
                End If
                If WhiteTFTable(CoorX, CoorY) <> "0" And WhiteTFTable(CoorX, CoorY) <> "3" And CoorX < 7 Then
                    If BlackTFTable(CoorX + 1, CoorY - 1) = "T" Then BlackTFTable(CoorX + 1, CoorY - 1) = "F"
                    If (Board(CoorX + 1, CoorY - 1) >= "b" And Board(CoorX + 1, CoorY - 1) <= "r") Then
                        LegalMoveArray(n) = CoorX + 1 & CoorY - 1
                        If Board(CoorX + 1, CoorY - 1) = "k" Then
                            BInCheck.IsInCheck = True
                            CheckingPiece = CoorX & CoorY
                        End If
                        n += 1
                    End If
                End If
            End If


        ElseIf Board(CoorX, CoorY) = "N" Then 'Legal Moves for the Knight. (If pinned, then it cannot move at all.)
            If WhiteTFTable(CoorX, CoorY) >= "4" Then
                If CoorX >= 2 Then
                    For x = -1 To 1 Step 2
                        If (CoorY + x) >= 0 And (CoorY + x) <= 7 Then
                            If BlackTFTable(CoorX - 2, CoorY + x) = "T" Then BlackTFTable(CoorX - 2, CoorY + x) = "F"
                            If Board(CoorX - 2, CoorY + x) = " " Or (Board(CoorX - 2, CoorY + x) >= "b" And Board(CoorX - 2, CoorY + x) <= "r") Then
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
                        If (CoorY + x) >= 0 And (CoorY + x) <= 7 Then
                            If BlackTFTable(CoorX + 2, CoorY + x) = "T" Then BlackTFTable(CoorX + 2, CoorY + x) = "F"
                            If Board(CoorX + 2, CoorY + x) = " " Or (Board(CoorX + 2, CoorY + x) >= "b" And Board(CoorX + 2, CoorY + x) <= "r") Then
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
                        If (CoorX + x) >= 0 And (CoorX + x) <= 7 Then
                            If BlackTFTable(CoorX + x, CoorY - 2) = "T" Then BlackTFTable(CoorX + x, CoorY - 2) = "F"
                            If Board(CoorX + x, CoorY - 2) = " " Or (Board(CoorX + x, CoorY - 2) >= "b" And Board(CoorX + x, CoorY - 2) <= "r") Then
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
                        If (CoorX + x) >= 0 And (CoorX + x) <= 7 Then
                            If BlackTFTable(CoorX + x, CoorY + 2) = "T" Then BlackTFTable(CoorX + x, CoorY + 2) = "F"
                            If Board(CoorX + x, CoorY + 2) = " " Or (Board(CoorX + x, CoorY + 2) >= "b" And Board(CoorX + x, CoorY + 2) <= "r") Then
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


        ElseIf Board(CoorX, CoorY) = "R" Or Board(CoorX, CoorY) = "Q" Then 'Legal Moves for the Rook / part of Queen.
            If WhiteTFTable(CoorX, CoorY) <> "1" And WhiteTFTable(CoorX, CoorY) <> "3" Then
                If WhiteTFTable(CoorX, CoorY) <> "0" Then
                    For x = (CoorX + 1) To 7
                        If BlackTFTable(x, CoorY) = "T" Then BlackTFTable(x, CoorY) = "F"
                        If Board(x, CoorY) >= "B" And Board(x, CoorY) <= "R" Then Exit For
                        If Board(x, CoorY) = " " Or (Board(x, CoorY) >= "b" And Board(x, CoorY) <= "r") Then
                            LegalMoveArray(n) = (x) & (CoorY)
                            If Board(x, CoorY) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit For
                            End If
                            n += 1
                            If (Board(x, CoorY) >= "b" And Board(x, CoorY) <= "r") And CInt(CStr(BKPos(0))) > x And CInt(CStr(BKPos(1))) = CoorY Then
                                For m = x + 1 To 7
                                    If Board(m, CoorY) = "k" Then
                                        BlackTFTable(x, CoorY) = "2"
                                        Exit For
                                    End If
                                    If Board(m, CoorY) <> " " Then Exit For
                                Next
                            End If

                            If Board(x, CoorY) <> " " Then Exit For
                        End If
                    Next

                    For x = (CoorX - 1) To 0 Step -1
                        If BlackTFTable(x, CoorY) = "T" Then BlackTFTable(x, CoorY) = "F"
                        If Board(x, CoorY) >= "B" And Board(x, CoorY) <= "R" Then Exit For
                        If Board(x, CoorY) = " " Or (Board(x, CoorY) >= "b" And Board(x, CoorY) <= "r") Then
                            LegalMoveArray(n) = (x) & (CoorY)
                            If Board(x, CoorY) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit For
                            End If
                            n += 1
                            If (Board(x, CoorY) >= "b" And Board(x, CoorY) <= "r") And CInt(CStr(BKPos(0))) < x And CInt(CStr(BKPos(1))) = CoorY Then
                                For m = x - 1 To 0 Step -1
                                    If Board(m, CoorY) = "k" Then
                                        BlackTFTable(x, CoorY) = "2"
                                        Exit For
                                    End If
                                    If Board(m, CoorY) <> " " Then Exit For
                                Next
                            End If

                            If Board(x, CoorY) <> " " Then Exit For
                        End If
                    Next

                End If
                If WhiteTFTable(CoorX, CoorY) <> "2" Then
                    For y = (CoorY + 1) To 7
                        If BlackTFTable(CoorX, y) = "T" Then BlackTFTable(CoorX, y) = "F"
                        If Board(CoorX, y) >= "B" And Board(CoorX, y) <= "R" Then Exit For
                        If Board(CoorX, y) = " " Or (Board(CoorX, y) >= "b" And Board(CoorX, y) <= "r") Then
                            LegalMoveArray(n) = (CoorX) & (y)
                            If Board(CoorX, y) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit For
                            End If
                            n += 1
                            If (Board(CoorX, y) >= "b" And Board(CoorX, y) <= "r") And CInt(CStr(BKPos(0))) = CoorX And CInt(CStr(BKPos(1))) > y Then
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
                        If Board(CoorX, y) >= "B" And Board(CoorX, y) <= "R" Then Exit For
                        If Board(CoorX, y) = " " Or (Board(CoorX, y) >= "b" And Board(CoorX, y) <= "r") Then
                            LegalMoveArray(n) = (CoorX) & (y)
                            If Board(CoorX, y) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit For
                            End If
                            n += 1
                            If (Board(CoorX, y) >= "b" And Board(CoorX, y) <= "r") And CInt(CStr(BKPos(0))) = CoorX And CInt(CStr(BKPos(1))) < y Then
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


        If Board(CoorX, CoorY) = "B" Or Board(CoorX, CoorY) = "Q" Then 'Legal Moves for the Biship / Part of Queen.
            Dim o As SByte
            Dim p As SByte
            If WhiteTFTable(CoorX, CoorY) <> "0" And WhiteTFTable(CoorX, CoorY) <> "2" Then
                If WhiteTFTable(CoorX, CoorY) <> "1" Then
                    StartX = CoorX + 1
                    StartY = CoorY + 1
                    Do Until StartX > 7 Or StartY > 7
                        If BlackTFTable(StartX, StartY) = "T" Then BlackTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r") Then
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit Do
                            End If
                            n += 1
                            If (Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r") And (CInt(CStr(BKPos(0))) - CoorX = CInt(CStr(BKPos(1))) - CoorY) And CInt(CStr(BKPos(0))) > CoorX Then
                                o = StartX + 1
                                p = StartY + 1
                                Do Until o > 7 Or p > 7
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
                    Do Until StartX < 0 Or StartY < 0
                        If BlackTFTable(StartX, StartY) = "T" Then BlackTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r") Then
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit Do
                            End If
                            n += 1
                            If (Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r") And (CInt(CStr(BKPos(0))) - CoorX = CInt(CStr(BKPos(1))) - CoorY) And CInt(CStr(BKPos(0))) < CoorX Then
                                o = StartX - 1
                                p = StartY - 1
                                Do Until o < 0 Or p < 0
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
                    Do Until StartX > 7 Or StartY < 0
                        If BlackTFTable(StartX, StartY) = "T" Then BlackTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r") Then
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit Do
                            End If
                            n += 1
                            If (Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r") And (CInt(CStr(BKPos(0))) - CoorX = CoorY - CInt(CStr(BKPos(1)))) And CInt(CStr(BKPos(0))) > CoorX Then
                                o = StartX + 1
                                p = StartY - 1
                                Do Until o > 7 Or p < 0
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
                    Do Until StartX < 0 Or StartY > 7
                        If BlackTFTable(StartX, StartY) = "T" Then BlackTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r") Then
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "k" Then
                                BInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit Do
                            End If
                            n += 1
                            If (Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r") And (CInt(CStr(BKPos(0))) - CoorX = CoorY - CInt(CStr(BKPos(1)))) And CInt(CStr(BKPos(0))) < CoorX Then
                                o = StartX - 1
                                p = StartY + 1
                                Do Until o < 0 Or p > 7
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
        If BInCheck.Piece1 = " " Or BInCheck.Piece1 = CheckingPiece Then
            BInCheck.Piece1 = CheckingPiece
        ElseIf CheckingPiece <> " " Then
            BInCheck.Piece2 = CheckingPiece
        End If
        'The first index of LegalMoveArray contains a counter of how many moves each piece can make. Useful for
        'when the AI is picking moves.
        If n <> 1 Then LegalMoveArray(0) = n
        Return LegalMoveArray
    End Function


    Function BlackPieceLegalMoves(ByRef Board(,) As Char, ByVal CoorX As Byte, ByVal CoorY As Byte, ByRef BlackTFTable(,) As Char, ByRef WhiteTFTable(,) As Char, ByRef WKPos As String, ByRef WInCheck As InCheck, ByRef BInCheck As InCheck, ByRef BCanCastle As CanCastle) As String()
        Dim LegalMoveArray(27) As String
        Dim n As Byte = 1
        Dim StartY As SByte
        Dim EndY As SByte
        Dim StartX As SByte
        Dim EndX As SByte
        Dim CheckingPiece As String = " "

        'If the King is in a Double Check situation, then only King Moves will be accepted.
        'This makes checks much more efficient.
        If BInCheck.Piece2 <> " " And Board(CoorX, CoorY) <> "k" Then
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
                    If (Board(x, y) = " " Or (Board(x, y) >= "B" And Board(x, y) <= "R")) And BlackTFTable(x, y) = "T" Then
                        LegalMoveArray(n) = (x) & (y)
                        n += 1
                    End If
                Next
            Next
            If BCanCastle.KS And BInCheck.IsInCheck = False Then
                If Board(CoorX + 1, CoorY) = " " And Board(CoorX + 2, CoorY) = " " And Board(CoorX + 3, CoorY) = "r" And BlackTFTable(CoorX + 1, CoorY) = "T" And BlackTFTable(CoorX + 2, CoorY) = "T" Then
                    LegalMoveArray(n) = (CoorX + 2) & (CoorY)
                    n += 1
                End If
            End If
            If BCanCastle.QS And BInCheck.IsInCheck = False Then
                If Board(CoorX - 1, CoorY) = " " And Board(CoorX - 2, CoorY) = " " And Board(CoorX - 3, CoorY) = " " And Board(CoorX - 4, CoorY) = "r" And BlackTFTable(CoorX - 1, CoorY) = "T" And BlackTFTable(CoorX - 2, CoorY) = "T" Then
                    LegalMoveArray(n) = (CoorX - 2) & (CoorY)
                    n += 1
                End If
            End If


        ElseIf Board(CoorX, CoorY) = "p" Then 'Legal Moves for the Pawn (along with First Moves.)
            If BlackTFTable(CoorX, CoorY) <> "2" Then
                If BlackTFTable(CoorX, CoorY) <> "1" And BlackTFTable(CoorX, CoorY) <> "3" Then
                    If Board(CoorX, CoorY + 1) = " " Then
                        LegalMoveArray(n) = CoorX & CoorY + 1
                        n += 1
                        If CoorY = 1 And Board(CoorX, 3) = " " Then
                            LegalMoveArray(n) = CoorX & 3
                            n += 1
                        End If
                    End If
                End If
                If BlackTFTable(CoorX, CoorY) <> "0" And BlackTFTable(CoorX, CoorY) <> "3" And CoorX > 0 Then
                    If WhiteTFTable(CoorX - 1, CoorY + 1) = "T" Then WhiteTFTable(CoorX - 1, CoorY + 1) = "F"
                    If (Board(CoorX - 1, CoorY + 1) >= "B" And Board(CoorX - 1, CoorY + 1) <= "R") Then
                        LegalMoveArray(n) = CoorX - 1 & CoorY + 1
                        If Board(CoorX - 1, CoorY + 1) = "K" Then
                            WInCheck.IsInCheck = True
                            CheckingPiece = CoorX & CoorY
                        End If
                        n += 1
                    End If
                End If
                If BlackTFTable(CoorX, CoorY) <> "0" And BlackTFTable(CoorX, CoorY) <> "1" And CoorX < 7 Then
                    If WhiteTFTable(CoorX + 1, CoorY + 1) = "T" Then WhiteTFTable(CoorX + 1, CoorY + 1) = "F"
                    If (Board(CoorX + 1, CoorY + 1) >= "B" And Board(CoorX + 1, CoorY + 1) <= "R") Then
                        LegalMoveArray(n) = CoorX + 1 & CoorY + 1
                        If Board(CoorX + 1, CoorY + 1) = "K" Then
                            WInCheck.IsInCheck = True
                            CheckingPiece = CoorX & CoorY
                        End If
                        n += 1
                    End If
                End If
            End If


        ElseIf Board(CoorX, CoorY) = "n" Then 'Legal Moves for the Knight. (If pinned, then it cannot move at all.)
            If BlackTFTable(CoorX, CoorY) >= "4" Then
                If CoorX >= 2 Then
                    For x = -1 To 1 Step 2
                        If (CoorY + x) >= 0 And (CoorY + x) <= 7 Then
                            If WhiteTFTable(CoorX - 2, CoorY + x) = "T" Then WhiteTFTable(CoorX - 2, CoorY + x) = "F"
                            If Board(CoorX - 2, CoorY + x) = " " Or (Board(CoorX - 2, CoorY + x) >= "B" And Board(CoorX - 2, CoorY + x) <= "R") Then
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
                        If (CoorY + x) >= 0 And (CoorY + x) <= 7 Then
                            If WhiteTFTable(CoorX + 2, CoorY + x) = "T" Then WhiteTFTable(CoorX + 2, CoorY + x) = "F"
                            If Board(CoorX + 2, CoorY + x) = " " Or (Board(CoorX + 2, CoorY + x) >= "B" And Board(CoorX + 2, CoorY + x) <= "R") Then
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
                        If (CoorX + x) >= 0 And (CoorX + x) <= 7 Then
                            If WhiteTFTable(CoorX + x, CoorY - 2) = "T" Then WhiteTFTable(CoorX + x, CoorY - 2) = "F"
                            If Board(CoorX + x, CoorY - 2) = " " Or (Board(CoorX + x, CoorY - 2) >= "B" And Board(CoorX + x, CoorY - 2) <= "R") Then
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
                        If (CoorX + x) >= 0 And (CoorX + x) <= 7 Then
                            If WhiteTFTable(CoorX + x, CoorY + 2) = "T" Then WhiteTFTable(CoorX + x, CoorY + 2) = "F"
                            If Board(CoorX + x, CoorY + 2) = " " Or (Board(CoorX + x, CoorY + 2) >= "B" And Board(CoorX + x, CoorY + 2) <= "R") Then
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


        ElseIf Board(CoorX, CoorY) = "r" Or Board(CoorX, CoorY) = "q" Then 'Legal Moves for the Rook / Part of Queen.
            If BlackTFTable(CoorX, CoorY) <> "1" And BlackTFTable(CoorX, CoorY) <> "3" Then
                If BlackTFTable(CoorX, CoorY) <> "0" Then
                    For x = (CoorX + 1) To 7
                        If WhiteTFTable(x, CoorY) = "T" Then WhiteTFTable(x, CoorY) = "F"
                        If Board(x, CoorY) >= "b" And Board(x, CoorY) <= "r" Then Exit For
                        If Board(x, CoorY) = " " Or (Board(x, CoorY) >= "B" And Board(x, CoorY) <= "R") Then
                            LegalMoveArray(n) = (x) & (CoorY)
                            If Board(x, CoorY) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit For
                            End If
                            n += 1
                            If (Board(x, CoorY) >= "B" And Board(x, CoorY) <= "R") And CInt(CStr(WKPos(0))) > x And CInt(CStr(WKPos(1))) = CoorY Then
                                For m = x + 1 To 7
                                    If Board(m, CoorY) = "K" Then
                                        WhiteTFTable(x, CoorY) = "2"
                                        Exit For
                                    End If
                                    If Board(m, CoorY) <> " " Then Exit For
                                Next
                            End If
                            If Board(x, CoorY) <> " " Then Exit For
                        End If
                    Next

                    For x = (CoorX - 1) To 0 Step -1
                        If WhiteTFTable(x, CoorY) = "T" Then WhiteTFTable(x, CoorY) = "F"
                        If Board(x, CoorY) >= "b" And Board(x, CoorY) <= "r" Then Exit For
                        If Board(x, CoorY) = " " Or (Board(x, CoorY) >= "B" And Board(x, CoorY) <= "R") Then
                            LegalMoveArray(n) = (x) & (CoorY)
                            If Board(x, CoorY) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit For
                            End If
                            n += 1
                            If (Board(x, CoorY) >= "B" And Board(x, CoorY) <= "R") And CInt(CStr(WKPos(0))) < x And CInt(CStr(WKPos(1))) = CoorY Then
                                For m = x - 1 To 0 Step -1
                                    If Board(m, CoorY) = "K" Then
                                        WhiteTFTable(x, CoorY) = "2"
                                        Exit For
                                    End If
                                    If Board(m, CoorY) <> " " Then Exit For
                                Next
                            End If
                            If Board(x, CoorY) <> " " Then Exit For
                        End If
                    Next
                End If

                If BlackTFTable(CoorX, CoorY) <> "2" Then
                    For y = (CoorY + 1) To 7
                        If WhiteTFTable(CoorX, y) = "T" Then WhiteTFTable(CoorX, y) = "F"
                        If Board(CoorX, y) >= "b" And Board(CoorX, y) <= "r" Then Exit For
                        If Board(CoorX, y) = " " Or (Board(CoorX, y) >= "B" And Board(CoorX, y) <= "R") Then
                            LegalMoveArray(n) = (CoorX) & (y)
                            If Board(CoorX, y) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit For
                            End If
                            n += 1
                            If (Board(CoorX, y) >= "B" And Board(CoorX, y) <= "R") And CInt(CStr(WKPos(0))) = CoorX And CInt(CStr(WKPos(1))) > y Then
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
                        If Board(CoorX, y) >= "b" And Board(CoorX, y) <= "r" Then Exit For
                        If Board(CoorX, y) = " " Or (Board(CoorX, y) >= "B" And Board(CoorX, y) <= "R") Then
                            LegalMoveArray(n) = (CoorX) & (y)
                            If Board(CoorX, y) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit For
                            End If
                            n += 1
                            If (Board(CoorX, y) >= "B" And Board(CoorX, y) <= "R") And CInt(CStr(WKPos(0))) = CoorX And CInt(CStr(WKPos(1))) < y Then
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


        If Board(CoorX, CoorY) = "b" Or Board(CoorX, CoorY) = "q" Then 'Legal Moves for the Bishop / Part of Queen.
            Dim o As SByte
            Dim p As SByte
            If BlackTFTable(CoorX, CoorY) <> "0" And BlackTFTable(CoorX, CoorY) <> "2" Then
                If BlackTFTable(CoorX, CoorY) <> "1" Then
                    StartX = CoorX + 1
                    StartY = CoorY + 1
                    Do Until StartX > 7 Or StartY > 7
                        If WhiteTFTable(StartX, StartY) = "T" Then WhiteTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R") Then
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit Do
                            End If
                            n += 1
                            If (Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R") And (CInt(CStr(WKPos(0))) - CoorX = CInt(CStr(WKPos(1))) - CoorY) And CInt(CStr(WKPos(0))) > CoorX Then
                                o = StartX + 1
                                p = StartY + 1
                                Do Until o > 7 Or p > 7
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
                    Do Until StartX < 0 Or StartY < 0
                        If WhiteTFTable(StartX, StartY) = "T" Then WhiteTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R") Then
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit Do
                            End If
                            n += 1
                            If (Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R") And (CInt(CStr(WKPos(0))) - CoorX = CInt(CStr(WKPos(1))) - CoorY) And CInt(CStr(WKPos(0))) < CoorX Then
                                o = StartX - 1
                                p = StartY - 1
                                Do Until o < 0 Or p < 0
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
                    Do Until StartX > 7 Or StartY < 0
                        If WhiteTFTable(StartX, StartY) = "T" Then WhiteTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R") Then
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit Do
                            End If
                            n += 1
                            If (Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R") And (CInt(CStr(WKPos(0))) - CoorX = CoorY - CInt(CStr(WKPos(1)))) And CInt(CStr(WKPos(0))) > CoorX Then
                                o = StartX + 1
                                p = StartY - 1
                                Do Until o > 7 Or p < 0
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
                    Do Until StartX < 0 Or StartY > 7
                        If WhiteTFTable(StartX, StartY) = "T" Then WhiteTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R") Then
                            LegalMoveArray(n) = StartX & StartY
                            If Board(StartX, StartY) = "K" Then
                                WInCheck.IsInCheck = True
                                CheckingPiece = CoorX & CoorY
                                Exit Do
                            End If
                            n += 1
                            If (Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R") And (CInt(CStr(WKPos(0))) - CoorX = CoorY - CInt(CStr(WKPos(1)))) And CInt(CStr(WKPos(0))) < CoorX Then
                                o = StartX - 1
                                p = StartY + 1
                                Do Until o < 0 Or p > 7
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
        If WInCheck.Piece1 = " " Or WInCheck.Piece1 = CheckingPiece Then
            WInCheck.Piece1 = CheckingPiece
        ElseIf CheckingPiece <> " " Then
            WInCheck.Piece2 = CheckingPiece
        End If
        'The first index of LegalMoveArray contains a counter of how many moves each piece can make. Useful for
        'when the AI is picking moves.
        If n <> 1 Then LegalMoveArray(0) = n
        Return LegalMoveArray
    End Function



    Private MovingPiece As Boolean = False
    Private StartPoint As Point
    Private MidPoint As Point
    Private EndPoint As Point
    Private LegalMoves As String()

    Private Sub Piece_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles WK1.MouseDown, WQ1.MouseDown, WB1.MouseDown, WB2.MouseDown, WN1.MouseDown, WN2.MouseDown, WR1.MouseDown, WR2.MouseDown, WP1.MouseDown, WP2.MouseDown, WP3.MouseDown, WP4.MouseDown, WP5.MouseDown, WP6.MouseDown, WP7.MouseDown, WP8.MouseDown, BK1.MouseDown, BQ1.MouseDown, BB1.MouseDown, BB2.MouseDown, BN1.MouseDown, BN2.MouseDown, BR1.MouseDown, BR2.MouseDown, BP1.MouseDown, BP2.MouseDown, BP3.MouseDown, BP4.MouseDown, BP5.MouseDown, BP6.MouseDown, BP7.MouseDown, BP8.MouseDown, WQ8.MouseDown, WQ7.MouseDown, WQ6.MouseDown, WQ5.MouseDown, WQ4.MouseDown, WQ3.MouseDown, BQ6.MouseDown, BQ8.MouseDown, BQ5.MouseDown, BQ7.MouseDown, BQ2.MouseDown, BQ4.MouseDown, BQ3.MouseDown, WQ2.MouseDown
        'Subroutine that activates the drag + drog mechanic if the user clicks on a piece.
        If e.Button = Windows.Forms.MouseButtons.Left Then
            'Generates the Legal Moves for the piece the user has picked up.
            If GameRunning Then
                If CStr(sender.name)(0) = "W" And PlayerTurn Then
                    LegalMoves = WhitePieceLegalMoves(MasterBoard, sender.location.x / 75, sender.location.y / 75, MasterWhiteTFTable, MasterBlackTFTable, MasterBKPos, MasterBInCheck, MasterWInCheck, MasterWCanCastle)
                    MovingPiece = True
                ElseIf CStr(sender.name)(0) = "B" And (Not PlayerTurn) Then
                    LegalMoves = BlackPieceLegalMoves(MasterBoard, sender.location.x / 75, sender.location.y / 75, MasterBlackTFTable, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterBCanCastle)
                    MovingPiece = True
                End If
            End If
            sender.bringtofront()
            StartPoint = sender.location
            MidPoint = sender.PointToScreen(New Point(e.X, e.Y))
            'Console.WriteLine()
            'Console.WriteLine("here are moves")
            'If LegalMoves(0) IsNot Nothing Then
            '    For x = 0 To LegalMoves.Length - 1
            '        If LegalMoves(x) IsNot Nothing Then Console.WriteLine(LegalMoves(x))
            '    Next
            'End If
            'Console.WriteLine("end of moves")
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
                        If MasterWInCheck.IsInCheck Or MasterBInCheck.IsInCheck Then
                            If DoesMoveResolveCheck(MasterBoard, (StartPoint.X / 75) & (StartPoint.Y / 75), (sender.location.X / 75) & (sender.location.Y / 75), MasterWInCheck, MasterBInCheck) Then
                                MasterWInCheck.Piece1 = " "
                                MasterWInCheck.Piece2 = " "
                                MasterBInCheck.Piece1 = " "
                                MasterBInCheck.Piece2 = " "
                                IsLegalMove = True
                            End If
                        Else
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
                        'If a pawn has made it to the end of the board, it is promoted to a Queen.
                        If sender.name(1) = "P" And sender.location.y / 75 = 0 Then
                            For n = 1 To 8
                                ReplacedPiece = Me.Controls.Find("WQ" & n.ToString, True).Single()
                                If ReplacedPiece.Visible = False Then Exit For
                            Next
#Disable Warning BC42104 'Variable is used before it has been assigned a value
                            ReplacedPiece.Location = New Point(sender.location.X, sender.location.Y)
#Enable Warning BC42104 'Variable is used before it has been assigned a value
                            ReplacedPiece.Visible = True
                            ReplacedPiece.BringToFront()
                            MasterBoard(sender.location.X / 75, sender.location.Y / 75) = "Q"
                            sender.Visible = False
                            sender.Location = New Point(-100, -100)
                        Else
                            MasterBoard(sender.location.X / 75, sender.location.Y / 75) = UCase((CStr(sender.name))(1))
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
                        FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterWKPos, MasterBKPos)
                    Else
                        'Identical Code as above but for the black pieces.
                        If sender.name(1) = "P" And sender.location.y / 75 = 7 Then
                            For n = 1 To 8
                                ReplacedPiece = Me.Controls.Find("BQ" & n.ToString, True).Single()
                                If ReplacedPiece.Visible = False Then Exit For
                            Next
                            ReplacedPiece.Location = New Point(sender.location.X, sender.location.Y)
                            ReplacedPiece.Visible = True
                            MasterBoard(sender.location.X / 75, sender.location.Y / 75) = "q"
                            sender.Visible = False
                            sender.Location = New Point(-100, -100)
                        Else
                            MasterBoard(sender.location.X / 75, sender.location.Y / 75) = LCase((CStr(sender.name))(1))
                        End If
                        If sender.name(1) = "R" Then
                            If sender.name(2) = "1" Then
                                MasterBCanCastle.QS = False
                            Else
                                MasterBCanCastle.KS = False
                            End If
                        End If
                        'The TrueFalse Table is generated for the White Pieces.
                        FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterBKPos)
                    End If

                    'Checks if any player is in check, and if so notifies the player with audio and visuals.
                    CheckChecker()
                    If MasterWInCheck.IsInCheck Or MasterBInCheck.IsInCheck Then
                        Sound_Check.Play()
                    End If
                    MasterMaterialCount = CountMaterial(MasterBoard)
                    CurrentEval.Text = MasterMaterialCount
                    Console.WriteLine("WKPos: " & MasterWKPos & ". BKPos: " & MasterBKPos)
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
                    AbsoluteDepth = 0
                    If PlayerTurn Then
                        BlackAIMove_Click()
                    Else
                        'RUN WHITE AI.
                    End If
                    'Absolute depth is then restored, and the next turn begins.
                    CalculateAbsoluteDepth()
                    PlayerTurn = Not PlayerTurn

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
    Private Sub FixTFTables(ByRef Board(,) As Char, ByRef FixWhite As Boolean, ByRef TrueFalseTable(,) As Char, ByRef WKpos As String, ByRef BKPos As String)
        'Stopwatch.Reset()
        'Stopwatch.Start()
        Dim dx As SByte
        Dim dy As SByte
        Array.Copy(MasterTrueTable, TrueFalseTable, 64)
        If FixWhite Then
            For y = 0 To 7
                For x = 0 To 7
                    If Board(x, y) <> " " Then
                        dx = Math.Abs(CStr(WKpos(0)) - x)
                        dy = Math.Abs(CStr(WKpos(1)) - y)
                        If Board(x, y) = "p" Then
                            If (CStr(WKpos(1)) - y >= 0 And CStr(WKpos(1)) - y <= 2) And dx <= 2 Then
                                Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterBCanCastle)
                            End If
                        ElseIf Board(x, y) = "r" Or Board(x, y) = "q" Then
                            If dx <= 1 Or dy <= 1 Then
                                Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterBCanCastle)
                            End If
                        ElseIf Board(x, y) = "n" Then
                            If (dx <= 3 And dy <= 2) Or (dx <= 2 And dy <= 3) Then
                                Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterBCanCastle)
                            End If
                        ElseIf Board(x, y) = "k" Then
                            If dx <= 2 And dy <= 2 Then
                                Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterBCanCastle)
                            End If
                        End If
                    End If
                    If Board(x, y) = "b" Or Board(x, y) = "q" Then
                        If dx - dy <= 2 And dy - dx <= 2 Then
                            Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterBCanCastle)
                        End If
                    End If
                Next
            Next
        Else
            For y = 0 To 7
                For x = 0 To 7
                    If Board(x, y) <> " " Then
                        dx = Math.Abs(CStr(BKPos(0)) - x)
                        dy = Math.Abs(CStr(BKPos(1)) - y)
                        If Board(x, y) = "P" Then
                            If (CStr(BKPos(1)) - y >= 0 And CStr(BKPos(1)) - y <= 2) And dx <= 2 Then
                                Dim LegalMovesResult = WhitePieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, MasterBKPos, MasterBInCheck, MasterWInCheck, MasterWCanCastle)
                            End If
                        ElseIf Board(x, y) = "R" Or Board(x, y) = "Q" Then
                            If dx <= 1 Or dy <= 1 Then
                                Dim LegalMovesResult = WhitePieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, MasterBKPos, MasterBInCheck, MasterWInCheck, MasterWCanCastle)
                            End If
                        ElseIf Board(x, y) = "N" Then
                            If (dx <= 3 And dy <= 2) Or (dx <= 2 And dy <= 3) Then
                                Dim LegalMovesResult = WhitePieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, MasterBKPos, MasterBInCheck, MasterWInCheck, MasterWCanCastle)
                            End If
                        ElseIf Board(x, y) = "K" Then
                            If dx <= 2 And dy <= 2 Then
                                Dim LegalMovesResult = WhitePieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, MasterBKPos, MasterBInCheck, MasterWInCheck, MasterWCanCastle)
                            End If
                        End If
                    End If
                    If Board(x, y) = "B" Or Board(x, y) = "Q" Then
                        If dx - dy <= 2 And dy - dx <= 2 Then
                            Dim LegalMovesResult = WhitePieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, MasterBKPos, MasterBInCheck, MasterWInCheck, MasterWCanCastle)
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
        If MasterWInCheck.IsInCheck Or MasterBInCheck.IsInCheck Then
            CheckLabel.Text = "Check!"
        Else
            CheckLabel.Text = "            "
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
    Function DoesMoveResolveCheck(ByRef Board(,) As Char, ByRef OldPos As String, ByRef NewPos As String, ByRef WInCheck As InCheck, ByRef BInCheck As InCheck) As Boolean
        Dim CheckBoard(7, 7) As Char
        DoesMoveResolveCheck = False
        If WInCheck.IsInCheck Then
            WInCheck.IsInCheck = False
            Array.Copy(Board, CheckBoard, 64)
            CheckBoard(CStr(NewPos(0)), CStr(NewPos(1))) = CheckBoard(CStr(OldPos(0)), CStr(OldPos(1)))
            If CheckBoard(CStr(OldPos(0)), CStr(OldPos(1))) = "K" Then CheckBoard(CStr(OldPos(0)), CStr(OldPos(1))) = " "
            Array.Copy(MasterTrueTable, TrueTable, 64)
            BlackPieceLegalMoves(CheckBoard, CStr(WInCheck.Piece1(0)), CStr(WInCheck.Piece1(1)), TrueTable, TrueTable, "00", WInCheck, BInCheck, MasterBCanCastle)
            If WInCheck.Piece2 <> " " Then BlackPieceLegalMoves(CheckBoard, CStr(WInCheck.Piece2(0)), CStr(WInCheck.Piece2(1)), TrueTable, TrueTable, "00", WInCheck, BInCheck, MasterBCanCastle)
            If WInCheck.IsInCheck = False Then
                DoesMoveResolveCheck = True
            End If
        ElseIf BInCheck.IsInCheck Then
            BInCheck.IsInCheck = False
            Array.Copy(Board, CheckBoard, 64)
            CheckBoard(CStr(NewPos(0)), CStr(NewPos(1))) = CheckBoard(CStr(OldPos(0)), CStr(OldPos(1)))
            If CheckBoard(CStr(OldPos(0)), CStr(OldPos(1))) = "k" Then CheckBoard(CStr(OldPos(0)), CStr(OldPos(1))) = " "
            Array.Copy(MasterTrueTable, TrueTable, 64)
            WhitePieceLegalMoves(CheckBoard, CStr(BInCheck.Piece1(0)), CStr(BInCheck.Piece1(1)), TrueTable, TrueTable, "00", BInCheck, WInCheck, MasterWCanCastle)
            If BInCheck.Piece2 <> " " Then WhitePieceLegalMoves(CheckBoard, CStr(BInCheck.Piece2(0)), CStr(BInCheck.Piece2(1)), TrueTable, TrueTable, "00", BInCheck, WInCheck, MasterWCanCastle)
            If BInCheck.IsInCheck = False Then
                DoesMoveResolveCheck = True
            End If
        Else
            DoesMoveResolveCheck = True
        End If
    End Function



    Private Sub CalculateAbsoluteDepth()
        AbsoluteDepth = 1
    End Sub


    Private Sub BlackAIMove_Click() Handles BlackAIMove.Click
        Dim CurrentScore As Decimal
        Dim BestScore As Decimal = Integer.MaxValue
        Dim BestMove(2) As String
        If GameRunning Then
            Stopwatch.Reset()
            Stopwatch.Start()
            For y = 0 To 7
                For x = 0 To 7
                    If MasterBoard(x, y) >= "b" And MasterBoard(x, y) <= "r" Then
                        Array.Copy(MasterTrueTable, TrueTable, 64)
                        Dim PieceMoves = BlackPieceLegalMoves(MasterBoard, x, y, MasterBlackTFTable, TrueTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterBCanCastle)
                        If PieceMoves(0) IsNot Nothing Then
                            Console.WriteLine("Piece: " & MasterBoard(x, y) & ". Location: [" & x & "," & y & "]. Moves:")
                            Dim TempBoard(7, 7) As Char
                            Dim TempWhiteTFTable(7, 7) As Char
                            Dim TempMaterialCount As Int16
                            Dim TempBKPos As String
                            Dim TempBCanCastle As CanCastle
                            Dim TempBInCheck As InCheck
                            For n = 1 To CInt(PieceMoves(0)) - 1
                                Console.WriteLine(PieceMoves(n))
                                If MasterBInCheck.IsInCheck Then
                                    TempBInCheck.IsInCheck = True
                                    TempBInCheck.Piece1 = MasterBInCheck.Piece1
                                    TempBInCheck.Piece2 = MasterBInCheck.Piece2
                                    '#Disable Warning BC42104 'Variable is used before it has been assigned a value
                                    If DoesMoveResolveCheck(MasterBoard, x & y, PieceMoves(n)(0) & PieceMoves(n)(1), MasterWInCheck, TempBInCheck) Then
                                        TempBInCheck.Piece1 = " "
                                        TempBInCheck.Piece2 = " "
                                    End If
                                    '#Enable Warning BC42104 'Variable is used before it has been assigned a value
                                Else
                                    TempBInCheck.IsInCheck = False
                                End If
                                If Not TempBInCheck.IsInCheck Then
                                    Array.Copy(MasterBoard, TempBoard, 64)
                                    Array.Copy(MasterWhiteTFTable, TempWhiteTFTable, 64)
                                    TempMaterialCount = MasterMaterialCount
                                    TempBKPos = MasterBKPos
                                    TempBCanCastle.KS = MasterBCanCastle.KS
                                    TempBCanCastle.QS = MasterBCanCastle.QS
                                    MakeMove(TempBoard, x & y, PieceMoves(n)(0), PieceMoves(n)(1), MasterWCanCastle, TempBCanCastle, MasterWKPos, TempBKPos, TempMaterialCount, False)
                                    CurrentScore = MiniMax(TempBoard, AbsoluteDepth, True, MasterWCanCastle, TempBCanCastle, MasterWKPos, TempBKPos, TempMaterialCount)
                                    If CurrentScore < BestScore Then
                                        BestScore = CurrentScore
                                        BestMove(0) = x & y
                                        BestMove(1) = PieceMoves(n)(0)
                                        BestMove(2) = PieceMoves(n)(1)
                                    End If
                                End If
                            Next
                        End If
                    End If
                Next
            Next
            Stopwatch.Stop()
            Console.WriteLine(vbCrLf & "Search Result: " & Stopwatch.Elapsed.TotalMilliseconds & " Milliseconds.")

            If BestScore = Integer.MaxValue Then
                If MasterBInCheck.IsInCheck Then
                    CheckLabel.Text = "Checkmate!"
                    Sound_Checkmate.Play()
                Else
                    CheckLabel.Text = "Stalemate!"
                    Sound_Stalemate.Play()
                End If
                GameRunning = False
            Else
                If AbsoluteDepth <> 0 Then
                    If MasterBInCheck.IsInCheck Then
                        MasterBInCheck.IsInCheck = False
                        MasterBInCheck.Piece1 = " "
                        MasterBInCheck.Piece2 = " "
                    End If
                    MakeMove(MasterBoard, BestMove(0), BestMove(1), BestMove(2), MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterMaterialCount, True)

                    DisplayPieces()
                    FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterBKPos)
                    CheckChecker()
                    If MasterWInCheck.IsInCheck Or MasterBInCheck.IsInCheck Then
                        Sound_Check.Play()
                    End If
                    MasterMaterialCount = CountMaterial(MasterBoard)
                    CurrentEval.Text = MasterMaterialCount
                    PlayerTurn = True
                End If
            End If

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
    End Sub




    'Subroutine that makes a move on the board, given coordinates. Includes castling (& rights) and pawn promotion.
    Sub MakeMove(ByRef Board(,) As Char, ByRef OldCoor As String, ByRef NewCoorX As String, ByRef NewCoorY As String, ByRef WCanCastle As CanCastle, ByRef BCanCastle As CanCastle, ByRef WKPos As String, ByRef BKPos As String, ByRef MaterialCount As Int16, ByRef MakeSounds As Boolean)
        Dim TempPiece As Char = Board(CStr(OldCoor(0)), CStr(OldCoor(1)))
        If MakeSounds Then Sound_Move.Play()
        If TempPiece = "K" Then
            WCanCastle.KS = False
            WCanCastle.QS = False
            WKPos = NewCoorX & NewCoorY
            'Code for Castling.
            If WCanCastle.KS And NewCoorX = "6" And NewCoorY = "7" Then
                Board(5, 7) = "R"
                Board(7, 7) = " "
                If MakeSounds Then Sound_Castle.Play()
            ElseIf WCanCastle.QS And NewCoorX = "2" And NewCoorY = "7" Then
                Board(0, 7) = " "
                Board(3, 7) = "R"
                If MakeSounds Then Sound_Castle.Play()
            End If
        ElseIf TempPiece = "k" Then
            BCanCastle.KS = False
            BCanCastle.QS = False
            BKPos = NewCoorX & NewCoorY
            'Code for Castling.
            If BCanCastle.KS And NewCoorX = "6" And NewCoorY = "0" Then
                Board(5, 0) = "r"
                Board(7, 0) = " "
                If MakeSounds Then Sound_Castle.Play()
            ElseIf BCanCastle.QS And NewCoorX = "2" And NewCoorY = "0" Then
                Board(0, 0) = " "
                Board(3, 0) = "r"
                If MakeSounds Then Sound_Castle.Play()
            End If
            'If piece is a Rook, part of Castling is disabled (depending on which Rook has moved).
        ElseIf TempPiece = "R" And (WCanCastle.KS Or WCanCastle.QS) Then
            If Board(0, 7) <> "R" Then WCanCastle.QS = False
            If Board(7, 7) <> "R" Then WCanCastle.KS = False
        ElseIf TempPiece = "r" And (BCanCastle.KS Or BCanCastle.QS) Then
            If Board(0, 0) <> "r" Then BCanCastle.QS = False
            If Board(7, 0) <> "r" Then BCanCastle.KS = False
        ElseIf UCase(TempPiece) = "P" Then
            'Code for Promoting Pawns. Also increments the material count.
            If NewCoorY = 0 Then
                TempPiece = "Q"
                MaterialCount += 8 '+ 9 for a new queen, - 1 for losing the pawn in the process.
            ElseIf NewCoorY = 7 Then
                TempPiece = "q"
                MaterialCount -= 8
            End If
        End If
        'At the end of the subroutine, the Piece is placed at the new coordinates, and the old position is cleared.
        'If the new position contains a piece, then the material count is updated for only that piece.
        If Board(NewCoorX, NewCoorY) <> " " Then
            Dim LostPiece As Char = Board(NewCoorX, NewCoorY)
            If UCase(LostPiece) = LostPiece Then
                If MakeSounds Then Sound_Capture.Play()
                If LostPiece = "P" Then
                    MaterialCount -= PieceValue.P
                ElseIf LostPiece = "N" Then
                    MaterialCount -= PieceValue.N
                ElseIf LostPiece = "B" Then
                    MaterialCount -= PieceValue.B
                ElseIf LostPiece = "R" Then
                    MaterialCount -= PieceValue.R
                Else 'Therefore is a Queen.
                    MaterialCount -= PieceValue.Q
                End If
            Else
                If LostPiece = "p" Then
                    MaterialCount += PieceValue.P
                ElseIf LostPiece = "n" Then
                    MaterialCount += PieceValue.N
                ElseIf LostPiece = "b" Then
                    MaterialCount += PieceValue.B
                ElseIf LostPiece = "r" Then
                    MaterialCount += PieceValue.R
                Else 'Therefore is a Queen.
                    MaterialCount += PieceValue.Q
                End If
            End If
        End If
        Board(NewCoorX, NewCoorY) = TempPiece
        Board(CStr(OldCoor(0)), CStr(OldCoor(1))) = " "
    End Sub


    'Function that evaluates a Current Board Position. Takes into account material, 
    Function Evaluate(ByRef Board(,) As Char, ByRef WhiteTFTable(,) As Char, ByRef BlackTFTable(,) As Char, ByRef MaterialCount As Int16) As Decimal
        Evaluate = MaterialCount
    End Function



    Public Function MiniMax(ByRef Board(,) As Char, depth As SByte, ByRef isWhite As Boolean, ByRef WCanCastle As CanCastle, ByRef BCanCastle As CanCastle, ByRef WKPos As String, ByRef BKPos As String, ByRef MaterialCount As Int16) As Decimal
        Return CountMaterial(Board)
        'FixTFTables(Board, True, MasterWhiteTFTable, MasterWKPos, MasterBKPos)
        'set check function
        'generate moves
        'checkmate / stalemate
    End Function

End Class


'Tasks left to do:
'En passant
'Drawing rules
'Threefold repetition
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
