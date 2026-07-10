'This class contains most of the primary algorithms I will be using in my project, and will link to both my Chess class and
'my AI class (either via instavintiation or by inheritance). It will contain the algorithms that will be used by both my Chess
'& AI classes, such as the ‘PieceLegalMoves’ generator, the ‘TFTable’ Generator, ‘DoesMoveResolveCheck’, and others.
Public Class CoreMethods
    'TrueTables are just TrueFalse Tables containing only the letter T. Very useful for resetting TrueFalse Tables
    'and debugging.
    Public MasterTrueTable(7, 7), TrueTable(7, 7) As Char
    Public CannotCastle As New CanCastle
    Public NotInCheck As New InCheck
    Private PieceValue(9) As Decimal 'Array Containing the Value or Weight of each Piece.
    Public Sub New()
        'Sets PieceValues variables using a Hash Function (Upper Case letter --> ASCII, then MOD 11). This
        'creates() a unique index / row in the PieceValue array for each piece and its corresponding weight,
        'so its value can be searched up quickly. PieceValue(Asc(UCase(Board(x, y))) Mod 11)
        PieceValue(0) = 3 'Bishop Weight
        PieceValue(1) = 3 'Knight Weight
        PieceValue(3) = 1 'Pawn Weight
        PieceValue(4) = 9 'Queen Weight
        PieceValue(5) = 5 'Rook Weight
        PieceValue(9) = 0 'King Weight

        'Creates MasterTrueTable and TrueTable.
        For x = 0 To 7
            For y = 0 To 7
                MasterTrueTable(x, y) = "T"
            Next
        Next
        ResetTFTable(TrueTable)
    End Sub
    Public Sub ResetTFTable(ByRef TFTable(,) As Char)
        'As MasterTrueTable is constant, we just copy all the indexes from it to the user's TFTable.
        Array.Copy(MasterTrueTable, TFTable, 64)
    End Sub



    'Function which puts the required pieces in a FEN board position into an 8x8 board array.
    Public Function FENConverter(ByVal FEN As String, ByRef WCanCastle As CanCastle, ByRef BCanCastle As CanCastle, ByRef WKPos As String, ByRef BKPos As String, ByRef EnPassant As String, ByRef IsWhite As Boolean) As Char(,)
        'Resets castling & EnPassant rules
        WCanCastle.CannotCastle()
        BCanCastle.CannotCastle()
        EnPassant = "-"
        Dim x As Byte = 0
        Dim y As Byte = 0
        Dim tempArray(7, 7) As Char
        Dim SpaceLocation As Byte
        For n = 0 To Len(FEN) - 1
            Select Case FEN(n)
                Case "/" '= end of board row. Reset column index and increment row index.
                    y += 1
                    x = 0
                Case "A" To "Z", "a" To "z" '= name of piece - add name of piece to board index.
                    tempArray(x, y) = FEN(n)
                    If FEN(n) = "K" Then
                        WKPos = x & y
                    ElseIf FEN(n) = "k" Then
                        BKPos = x & y
                    End If
                    x += 1
                Case "0" To "8" 'Set of empty squares.
                    For m = 1 To CInt(CStr((FEN(n))))
                        tempArray(x, y) = " "
                        x += 1
                    Next
                Case " "
                    'Checks for pawns incorrectly placed on the 1st or 8th rank. If we detect any, then we provoke an
                    'intentional crash, which our encapsulating Try-Catch detects and promptly reverses.
                    For x = 0 To 7
                        If UCase(tempArray(x, 0)) = "P" OrElse UCase(tempArray(x, 7)) = "P" Then FEN = ""
                    Next
                    ' Once the SPACE has been reached In the FEN, we know that we have read the entire board. We can then move onto
                    ' info about castling, who's turn it is, En Passant And more.
                    SpaceLocation = FEN.IndexOf(" ") 'Following characters revolve around location of SPACE.
                    If FEN(SpaceLocation + 1) = "w" Then
                        IsWhite = True
                    Else
                        IsWhite = False
                    End If
                    FEN = Right(FEN, Len(FEN) - SpaceLocation - 3)
                    'Creates castling privileges (as long as the pieces are in the correct space).
                    For m = 0 To Len(FEN) - 1
                        If FEN(m) = "K" AndAlso (tempArray(4, 7) = "K" AndAlso tempArray(7, 7) = "R") Then
                            WCanCastle.KS = True
                        ElseIf FEN(m) = "Q" AndAlso (tempArray(4, 7) = "K" AndAlso tempArray(0, 7) = "R") Then
                            WCanCastle.QS = True
                        ElseIf FEN(m) = "k" AndAlso (tempArray(4, 0) = "k" AndAlso tempArray(7, 0) = "r") Then
                            BCanCastle.KS = True
                        ElseIf FEN(m) = "q" AndAlso (tempArray(4, 0) = "k" AndAlso tempArray(0, 0) = "r") Then
                            BCanCastle.QS = True
                        ElseIf FEN(m) >= "a" AndAlso FEN(m) <= "h" Then
                            'converts the a-h coordinate To the more computer-friendly index from 0-7 (eg: h3 --> 75)
                            EnPassant = Asc(FEN(m)) - 97 & 8 - CStr(FEN(m + 1))
                        End If
                    Next
                    Exit For
            End Select
        Next
        Return tempArray
    End Function

    'Function which converts the current board position into its FEN counterpart.
    Public Function ConvertToFEN(ByVal Board(,) As Char, ByVal WCanCastle As CanCastle, ByVal BCanCastle As CanCastle, ByVal EnPassant As String, ByVal isWhite As Boolean) As String
        Dim Counter As Byte = 0 '= the number of blank spaces in a row on the board.
        ConvertToFEN = ""
        For y = 0 To 7
            For x = 0 To 7
                If Board(x, y) = " " Then
                    Counter += 1
                Else 'If necessary, add Counter to the FEN & add the piece name to the FEN.
                    If Counter > 0 Then
                        ConvertToFEN &= Counter
                        Counter = 0
                    End If
                    ConvertToFEN &= Board(x, y)
                End If
            Next
            If Counter > 0 Then ConvertToFEN &= Counter
            ConvertToFEN &= "/" 'Creates new row break, and begins on the next one.
            Counter = 0
        Next
        ConvertToFEN = ConvertToFEN.TrimEnd("/") 'Removes the last character from the FEN ("/").
        If isWhite Then
            ConvertToFEN &= " w "
        Else
            ConvertToFEN &= " b "
        End If
        If WCanCastle.KS Then ConvertToFEN &= "K"
        If WCanCastle.QS Then ConvertToFEN &= "Q"
        If BCanCastle.KS Then ConvertToFEN &= "k"
        If BCanCastle.QS Then ConvertToFEN &= "q"
        If ConvertToFEN.EndsWith(" ") Then ConvertToFEN &= "-" '= therefore no castling privileges
        If EnPassant <> "-" Then
            'converts the computer-friendly index from 0-7 into the more human-friendly a-h coordinate (eg: 75 -> h3)
            ConvertToFEN &= " " & Chr(CStr(EnPassant(0)) + 97) & 8 - CStr(EnPassant(1)) & " 0 1"
        Else
            ConvertToFEN &= " - 0 1" 'represents the move numbers for a standard position.
        End If
        Return ConvertToFEN
    End Function



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
    Function WhitePieceLegalMoves(ByVal Board(,) As Char, ByVal CoorX As SByte, ByVal CoorY As SByte, ByRef WhiteTFTable(,) As Char, ByRef BlackTFTable(,) As Char, ByVal BKPos As String, ByRef BInCheck As InCheck, ByVal WInCheck As InCheck, ByVal WCanCastle As CanCastle, ByVal EnPassant As String) As String()
        Dim LegalMoveArray(27) As String
        Dim n As Byte = 1
        Dim StartY As SByte
        Dim EndY As SByte
        Dim StartX As SByte
        Dim EndX As SByte
        Dim CheckingPiece As String = " "

        If Board(CoorX, CoorY) = "K" Then 'Legal Moves for the King (along with Castling).
            StartX = Math.Max(CoorX - 1, 0)
            EndX = Math.Min(CoorX + 1, 7)
            StartY = Math.Max(CoorY - 1, 0)
            EndY = Math.Min(CoorY + 1, 7)
            For y = StartY To EndY
                For x = StartX To EndX
                    BlackTFTable(x, y) = "F"
                    If Not Char.IsUpper(Board(x, y)) AndAlso WhiteTFTable(x, y) = "T" Then
                        LegalMoveArray(n) = x & y
                        n += 1
                    End If
                Next
            Next
            If WCanCastle.KS AndAlso WInCheck.IsInCheck = False Then
                If Board(5, 7) = " " AndAlso Board(6, 7) = " " AndAlso Board(7, 7) = "R" AndAlso WhiteTFTable(5, 7) = "T" AndAlso WhiteTFTable(6, 7) = "T" Then
                    LegalMoveArray(n) = 67
                    n += 1
                End If
            End If
            If WCanCastle.QS AndAlso WInCheck.IsInCheck = False Then
                If Board(3, 7) = " " AndAlso Board(2, 7) = " " AndAlso Board(1, 7) = " " AndAlso Board(0, 7) = "R" AndAlso WhiteTFTable(3, 7) = "T" AndAlso WhiteTFTable(2, 7) = "T" Then
                    LegalMoveArray(n) = 27
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
                            If Board(x, CoorY) = "P" AndAlso x & CoorY + 1 = EnPassant AndAlso CInt(CStr(BKPos(0))) > x AndAlso CInt(CStr(BKPos(1))) = CoorY Then
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
                            If Board(x, CoorY) = "P" AndAlso x & CoorY + 1 = EnPassant AndAlso CInt(CStr(BKPos(0))) < x AndAlso CInt(CStr(BKPos(1))) = CoorY Then
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
            If BInCheck.Piece = " " OrElse BInCheck.Piece = CheckingPiece Then
                BInCheck.Piece = CheckingPiece
            Else
                BInCheck.DoubleCheck = True
            End If
        End If
        'The first index of LegalMoveArray contains a counter of how many moves each piece can make. Useful for
        'when the AI is picking moves.
        If n <> 1 Then LegalMoveArray(0) = n
        Return LegalMoveArray
    End Function

    Function BlackPieceLegalMoves(ByVal Board(,) As Char, ByVal CoorX As SByte, ByVal CoorY As SByte, ByRef BlackTFTable(,) As Char, ByRef WhiteTFTable(,) As Char, ByVal WKPos As String, ByRef WInCheck As InCheck, ByVal BInCheck As InCheck, ByVal BCanCastle As CanCastle, ByVal EnPassant As String) As String()
        Dim LegalMoveArray(27) As String
        Dim n As Byte = 1
        Dim StartY As SByte
        Dim EndY As SByte
        Dim StartX As SByte
        Dim EndX As SByte
        Dim CheckingPiece As String = " "

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
                If Board(5, 0) = " " AndAlso Board(6, 0) = " " AndAlso Board(7, 0) = "r" AndAlso BlackTFTable(5, 0) = "T" AndAlso BlackTFTable(6, 0) = "T" Then
                    LegalMoveArray(n) = 60
                    n += 1
                End If
            End If
            If BCanCastle.QS AndAlso BInCheck.IsInCheck = False Then
                If Board(3, 0) = " " AndAlso Board(2, 0) = " " AndAlso Board(1, 0) = " " AndAlso Board(0, 0) = "r" AndAlso BlackTFTable(3, 0) = "T" AndAlso BlackTFTable(2, 0) = "T" Then
                    LegalMoveArray(n) = 20
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
            If WInCheck.Piece = " " OrElse WInCheck.Piece = CheckingPiece Then
                WInCheck.Piece = CheckingPiece
            Else
                WInCheck.DoubleCheck = True
            End If
        End If
        'The first index of LegalMoveArray contains a counter of how many moves each piece can make. Useful for
        'when the AI is picking moves.
        If n <> 1 Then LegalMoveArray(0) = n
        Return LegalMoveArray
    End Function


    'Subroutine which creates the TrueFalse Table of the selected player (controlled by the Variable FixWhite).
    'This is done by generating all the legal moves of the pieces that could influence the enemy king's motion.
    'This creates a 'field' around the king (stating where its legal moves are), along with creating pinned pieces
    'and checks.
    Public Sub FixTFTables(ByVal Board(,) As Char, ByVal FixWhite As Boolean, ByRef TrueFalseTable(,) As Char, ByRef KPos As String, ByRef WInCheck As InCheck, ByRef BInCheck As InCheck, ByVal EnPassant As String)
        Dim dx As SByte
        Dim dy As SByte
        'Resets TFTables.
        Array.Copy(MasterTrueTable, TrueFalseTable, 64)
        Array.Copy(MasterTrueTable, TrueTable, 64)
        If FixWhite Then
            For y = 0 To 7
                For x = 0 To 7
                    If Char.IsLower(Board(x, y)) Then
                        'Calculates distances between piece and the enemy king.
                        dx = Math.Abs(CStr(KPos(0)) - x)
                        dy = Math.Abs(CStr(KPos(1)) - y)
                        If Board(x, y) = "p" Then
                            If (CStr(KPos(1)) - y >= 0 AndAlso CStr(KPos(1)) - y <= 2) AndAlso dx <= 2 Then
                                'Piece could influence king motion - calculate legal moves.
                                Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, WInCheck, BInCheck, CannotCastle, EnPassant)
                            End If
                        ElseIf Board(x, y) = "q" Then
                            If (dx <= 1 OrElse dy <= 1) OrElse (dx - dy <= 2 AndAlso dy - dx <= 2) Then
                                'Piece could influence king motion - calculate legal moves.
                                Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, WInCheck, BInCheck, CannotCastle, EnPassant)
                            End If
                        ElseIf Board(x, y) = "r" Then
                            If dx <= 1 OrElse dy <= 1 Then
                                'Piece could influence king motion - calculate legal moves.
                                Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, WInCheck, BInCheck, CannotCastle, EnPassant)
                            End If
                        ElseIf Board(x, y) = "b" Then
                            If dx - dy <= 2 AndAlso dy - dx <= 2 Then
                                'Piece could influence king motion - calculate legal moves.
                                Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, WInCheck, BInCheck, CannotCastle, EnPassant)
                            End If
                        ElseIf Board(x, y) = "n" Then
                            If (dx <= 3 AndAlso dy <= 2) OrElse (dx <= 2 AndAlso dy <= 3) Then
                                'Piece could influence king motion - calculate legal moves.
                                Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, WInCheck, BInCheck, CannotCastle, EnPassant)
                            End If
                        ElseIf Board(x, y) = "k" Then
                            If dx <= 2 AndAlso dy <= 2 Then
                                'Piece could influence king motion - calculate legal moves.
                                Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, WInCheck, BInCheck, CannotCastle, EnPassant)
                            End If
                        End If
                    End If
                Next
            Next
        Else 'Identical code but for the white pieces (fixing the Black TFTable).
            For y = 0 To 7
                For x = 0 To 7
                    If Char.IsUpper(Board(x, y)) Then
                        dx = Math.Abs(CStr(KPos(0)) - x)
                        dy = Math.Abs(CStr(KPos(1)) - y)
                        If Board(x, y) = "P" Then
                            If (CStr(KPos(1)) - y <= 0 AndAlso CStr(KPos(1)) - y >= -2) AndAlso dx <= 2 Then
                                Dim LegalMovesResult = WhitePieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, BInCheck, WInCheck, CannotCastle, EnPassant)
                            End If
                        ElseIf Board(x, y) = "Q" Then
                            If (dx <= 1 OrElse dy <= 1) OrElse (dx - dy <= 2 AndAlso dy - dx <= 2) Then
                                Dim LegalMovesResult = WhitePieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, BInCheck, WInCheck, CannotCastle, EnPassant)
                            End If
                        ElseIf Board(x, y) = "R" Then
                            If dx <= 1 OrElse dy <= 1 Then
                                Dim LegalMovesResult = WhitePieceLegalMoves(Board, x, y, TrueTable, TrueFalseTable, KPos, BInCheck, WInCheck, CannotCastle, EnPassant)
                            End If
                        ElseIf Board(x, y) = "B" Then
                            If dx - dy <= 2 AndAlso dy - dx <= 2 Then
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
                Next
            Next
        End If
    End Sub


    'Function which receives a game position and a possible move. The function makes this move on the board (using
    'shortcuts that can only be made on a virtual board), and then generates the possible moves of the attacking
    'piece(s). If the king is no longer being threatened, then the check as been resolved.
    Function DoesMoveResolveCheck(ByVal Board(,) As Char, ByVal OldPosX As String, ByVal OldPosY As String, ByVal NewPosX As String, ByVal NewPosY As String, ByRef WInCheck As InCheck, ByRef BInCheck As InCheck, ByVal EnPassant As String) As Boolean
        'If the player is in a double check, then only king moves could be legal. Therefore, as this part is done in the
        'TFTable generation, we skip this here.
        If WInCheck.IsInCheck AndAlso Not WInCheck.DoubleCheck Then
            If NewPosX & NewPosY = WInCheck.Piece OrElse (NewPosX & NewPosY = EnPassant AndAlso Board(OldPosX, OldPosY) = "P") Then
                'Move captures the attacking piece. Therefore we assume it is legal.
                Return True
            ElseIf Board(NewPosX, NewPosY) = " " Then
                WInCheck.IsInCheck = False
                Board(NewPosX, NewPosY) = "O" 'Move made on temporary board.
                'Calculate the legal moves of the attacking piece. If the king is still in check, then WInCheck.IsInCheck
                'will flag from False to True - therefore it is illegal.
                BlackPieceLegalMoves(Board, CStr(WInCheck.Piece(0)), CStr(WInCheck.Piece(1)), TrueTable, TrueTable, "00", WInCheck, BInCheck, CannotCastle, "-")
                Board(NewPosX, NewPosY) = " " 'Move unmade on temporary board.
                If WInCheck.IsInCheck = False Then Return True
            Else 'Move is a capture move, but not capturing the attacking piece. Therefore, it has to be illegal.
                Return False
            End If
        ElseIf BInCheck.IsInCheck AndAlso Not BInCheck.DoubleCheck Then
            'Identical code but for the black pieces.
            If NewPosX & NewPosY = BInCheck.Piece OrElse (NewPosX & NewPosY = EnPassant AndAlso Board(OldPosX, OldPosY) = "p") Then
                Return True
            ElseIf Board(NewPosX, NewPosY) = " " Then
                BInCheck.IsInCheck = False
                Board(NewPosX, NewPosY) = "o"
                WhitePieceLegalMoves(Board, CStr(BInCheck.Piece(0)), CStr(BInCheck.Piece(1)), TrueTable, TrueTable, "00", BInCheck, WInCheck, CannotCastle, "-")
                Board(NewPosX, NewPosY) = " "
                If BInCheck.IsInCheck = False Then Return True
            Else
                Return False
            End If
        End If
        Return False
    End Function



    'Algorithm that returns the weight / value of a given piece. Links to the array of hashed values PieceValue.
    Function ReturnPieceValue(ByVal Piece As Char) As Decimal
        Return PieceValue(Asc(UCase(Piece)) Mod 11)
    End Function


    'Function which counts up all the material (and their values) on the board and subtracts black's total from
    'white's total. Rough metric as to who is winning.
    Public Function CountMaterial(ByVal Board(,) As Char, ByVal TotalMaterial As Boolean) As Int16
        Dim WhiteMaterial As Int16
        Dim BlackMaterial As Int16
        For y = 0 To 7
            For x = 0 To 7
                If Board(x, y) <> " " Then
                    'Add the value of the piece to either White or Black's total.
                    If Char.IsUpper(Board(x, y)) Then
                        WhiteMaterial += ReturnPieceValue(Board(x, y))
                    Else
                        BlackMaterial += ReturnPieceValue(Board(x, y))
                    End If
                End If
            Next
        Next
        'This subroutine has two modes: one that finds the total weight of the board (for calculating
        'AbsoluteDepth), and the other that finds the difference in weight between the two sides
        '(to help evaluate a board position).
        If Not TotalMaterial Then
            CountMaterial = WhiteMaterial - BlackMaterial
        Else
            CountMaterial = WhiteMaterial + BlackMaterial
        End If
    End Function

    'Subroutine that converts a Move into standard chess notation (eg: e4, Nf4, Ka2).
    Public Function MoveConverter(ByVal Board(,) As Char, ByVal TempMove As Move, ByVal EnPassant As String) As String
        Dim MovedPiece As Char = UCase(Board(TempMove.OldMoveX, TempMove.OldMoveY))
        'Pawns operate differently with standard chess notation - when moving a pawn, we give its column (a-h),
        'then add its file that it is moving to. For all other pieces, we state the name of the piece, then its
        'end coordinates.
        If MovedPiece <> " " Then
            If MovedPiece = "P" Then
                MoveConverter = Chr(CStr(TempMove.OldMoveX) + 97)
            Else
                MoveConverter = MovedPiece
            End If
            If Board(TempMove.NewMoveX, TempMove.NewMoveY) <> " " OrElse (UCase(MovedPiece) = "P" AndAlso EnPassant = TempMove.NewMoveX & TempMove.NewMoveY) Then
                'Is a capture move - add an "x" followed by the coordinates of the captured piece.
                MoveConverter &= "x" & Chr(CStr(TempMove.NewMoveX) + 97) & 8 - TempMove.NewMoveY
            Else
                If MovedPiece <> "P" Then MoveConverter &= Chr(CStr(TempMove.NewMoveX) + 97)
                MoveConverter &= 8 - TempMove.NewMoveY
            End If
            'Code for pawn promotions.
            If MovedPiece = "P" AndAlso (TempMove.NewMoveY = 0 OrElse TempMove.NewMoveY = 7) Then MoveConverter &= "=Q"
        Else
            Return "ERROR"
        End If
    End Function

End Class
