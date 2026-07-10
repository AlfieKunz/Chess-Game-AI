Public Class Form1

    Public MasterArray(7, 7) As Char
    ReadOnly PieceArray(45) As PictureBox
    'Standard Chess notation that displays where all the pieces on the board are supposed to go.
    ReadOnly StartingFEN As String = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
    Public PlayerTurn As Boolean = True
    Public WKPos As String
    Public BKPos As String
    Public WhiteMoves(27, 15) As String
    Public BlackMoves(27, 15) As String
    Public WhiteTFTable(7, 7) As Char
    Public BlackTFTable(7, 7) As Char

    Public TrueTable(7, 7) As Char


    Structure CanCastle
        Dim KS As Boolean
        Dim QS As Boolean
    End Structure
    Public WCanCastle As CanCastle
    Public BCanCastle As CanCastle

    'Sets up sound effects.
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


    'Assigns each piece to PieceArray and sets the 'board' picturebox as their parent.
    Private Sub Form1_Load(sender As Object, e As System.EventArgs) Handles Me.Load
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
        'Sends the starting FEN to be converted and displayed on the screen.
        MasterArray = FENConverter(StartingFEN)
        WCanCastle.KS = True
        WCanCastle.KS = True
        WCanCastle.KS = True
        WCanCastle.KS = True

        For x = 0 To 7
            For y = 0 To 7
                TrueTable(x, y) = "T"
            Next
        Next
        DisplayPieces()
        FixTFTables(MasterArray, True)
        Array.Copy(TrueTable, BlackTFTable, TrueTable.Length)
    End Sub

    Private Sub CreateBoard(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Checkerboard.Paint
        Dim g As Graphics = e.Graphics
        Dim isLight As Boolean = True
        'Creates checkerboard pattern. (Alternates between light and dark colours.)
        For x = 0 To 7
            For y = 0 To 7
                Dim square As New Rectangle((75 * x), ((75 * y)), 75, 75)
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
        Dim n As Integer
        'Hides all pieces.
        For x = 0 To 45
            PieceArray(x).Location = New Point(-100, -100)
            PieceArray(x).Visible = False
        Next
        For y = 7 To 0 Step -1
            For x = 0 To 7
                If MasterArray(x, y) <> " " Then
                    n = 1
                    While True
                        'In MasterArray(), a lowercase letter represents a black piece, and an uppercase letter represents a white piece.
                        If MasterArray(x, y) = LCase(MasterArray(x, y)) Then
                            CurrentPiece = "B"
                        Else
                            CurrentPiece = "W"
                        End If
                        'CurrentPiece is created, which has the same name as a corresponding piece on the board.
                        CurrentPiece &= UCase(MasterArray(x, y)) & CStr(n)
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


    'Alternate version of DisplayPieces() which is more efficient and only converts a single piece in MasterArray() to its piece counterpart on the board.
    Private Function MarrToPieceConv(ByVal CoorX As Integer, ByVal CoorY As Integer, ByVal PieceColour As Char) As PictureBox
        Dim CurrentPiece As String
        Dim TempPiece As PictureBox
        Dim n As Integer
        If MasterArray(CoorX / 75, CoorY / 75) <> " " Then
            n = 1
            While True
                'In MasterArray(), a lowercase letter represents a black piece, and an uppercase letter represents a white piece.
                If MasterArray(CoorX / 75, CoorY / 75) = LCase(MasterArray(CoorX / 75, CoorY / 75)) Then
                    CurrentPiece = "B"
                Else
                    CurrentPiece = "W"
                End If
                'CurrentPiece is created, which has the same name as a corresponding piece on the board.
                CurrentPiece &= UCase(MasterArray(CoorX / 75, CoorY / 75)) & CStr(n)
                'Searches for piece that has not been placed on the board (that matches CurrentPiece / TempPiece's name + location), and moves its match into place on the board.
                TempPiece = Me.Controls.Find(CurrentPiece, True).Single()
                If CInt(TempPiece.Location.X) = CoorX And CInt(TempPiece.Location.Y) = CoorY And TempPiece.Name(0) <> PieceColour Then
                    Exit While
                End If

                'If not found, the search continues with an increasing last digit (eg: the program seaches for BP1, then BP2, then BP3 ext until it finds its match.)
                n += 1
            End While
        End If
        Return TempPiece
    End Function


    'Function which puts the required pieces in a FEN board position into an 8x8 board array.
    Private Function FENConverter(ByVal FEN As String) As Char(,)
        WCanCastle.KS = False
        WCanCastle.QS = False
        BCanCastle.KS = False
        BCanCastle.QS = False
        Dim x As Integer = 0
        Dim y As Integer = 0
        Dim tempArray(7, 7) As Char
        Dim SpaceLocation As Integer
        For n = 0 To Len(FEN) - 1
            Select Case FEN(n)
                Case "/"
                    y += 1
                    x = 0
                Case "A" To "Z", "a" To "z"
                    tempArray(x, y) = FEN(n)
                    If tempArray(x, y) = "K" Then
                        WKPos = x & y
                    ElseIf tempArray(x, y) = "k" Then
                        BKPos = x & y
                    End If
                    x += 1
                Case "0" To "8"
                    For m = 1 To CInt(CStr((FEN(n))))
                        tempArray(x, y) = " "
                        x += 1
                    Next
                Case " "
                    'Console.WriteLine(FEN)
                    SpaceLocation = FEN.IndexOf(" ")
                    If FEN(SpaceLocation + 1) = "w" Then
                        PlayerTurn = True
                    Else
                        PlayerTurn = False
                    End If

                    FEN = Microsoft.VisualBasic.Right(FEN, Len(FEN) - SpaceLocation - 3)
                    'Console.WriteLine(FEN)
                    For m = 0 To Len(FEN) - 1
                        Select Case FEN(m)
                            Case "K"
                                WCanCastle.KS = True
                            Case "Q"
                                WCanCastle.QS = True
                            Case "k"
                                BCanCastle.KS = True
                            Case "q"
                                BCanCastle.QS = True
                        End Select
                    Next
                    'TO DO'
                    Exit For
            End Select
        Next
        'Console.WriteLine(WCanCastle.KS & WCanCastle.QS)
        'Console.WriteLine(BCanCastle.KS & BCanCastle.QS)
        Return tempArray
    End Function


    Function WhitePieceLegalMoves(ByVal Board(,) As Char, ByVal CoorX As Integer, ByVal CoorY As Integer, ByVal WhiteTFTable(,) As Char, ByRef BlackTFTable(,) As Char, ByRef BKPos As string) As (LegalMoveArray As String(), BlackTFTable As Char(,))
        Dim LegalMoveArray(26) As String
        Dim n As Integer = 1
        Dim StartY As Integer
        Dim EndY As Integer
        Dim StartX As Integer
        Dim EndX As Integer

        If Board(CoorX, CoorY) = "K" Then
            StartX = Math.Max(CoorX - 1, 0)
            EndX = Math.Min(CoorX + 1, 7)
            StartY = Math.Max(CoorY - 1, 0)
            EndY = Math.Min(CoorY + 1, 7)
            'Console.WriteLine(StartX & " " & EndX & " " & StartY & " " & EndY)
            For y = StartY To EndY
                For x = StartX To EndX
                    BlackTFTable(x, y) = "F"
                    'Console.WriteLine(x & y)
                    If (Board(x, y) = " " Or (Board(x, y) >= "b" And Board(x, y) <= "r")) And WhiteTFTable(x, y) = "T" Then
                        LegalMoveArray(n) = (x) & (y)
                        n += 1
                    End If
                Next
            Next
            If WCanCastle.KS Then
                If Board(CoorX + 1, CoorY) = " " And Board(CoorX + 2, CoorY) = " " And Board(CoorX + 3, CoorY) = "R" And WhiteTFTable(CoorX + 1, CoorY) = "T" And WhiteTFTable(CoorX + 2, CoorY) = "T" Then
                    LegalMoveArray(n) = (CoorX + 2) & (CoorY)
                    n += 1
                End If
            End If
            If WCanCastle.QS Then
                If Board(CoorX - 1, CoorY) = " " And Board(CoorX - 2, CoorY) = " " And Board(CoorX - 3, CoorY) = " " And Board(CoorX - 4, CoorY) = "R" And WhiteTFTable(CoorX - 1, CoorY) = "T" And WhiteTFTable(CoorX - 2, CoorY) = "T" Then
                    LegalMoveArray(n) = (CoorX - 2) & (CoorY)
                    n += 1
                End If
            End If

        ElseIf Board(CoorX, CoorY) = "P" Then
            If WhiteTFTable(CoorX, CoorY) >= "4" Or WhiteTFTable(CoorX, CoorY) = "0" Then
                StartX = Math.Max(CoorX - 1, 0)
                EndX = Math.Min(CoorX + 1, 7)
                If Board(CoorX, CoorY - 1) = " " Then
                    LegalMoveArray(n) = CoorX & CoorY - 1
                    n += 1
                    If CoorY = 6 And Board(CoorX, 4) = " " Then
                        LegalMoveArray(n) = CoorX & 4
                        n += 1
                    End If
                End If
                If WhiteTFTable(CoorX, CoorY) <> "0" Then
                    For x = StartX To EndX
                        If x <> CoorX Then
                            BlackTFTable(x, CoorY - 1) = "F"
                            If (Board(x, CoorY - 1) >= "b" And Board(x, CoorY - 1) <= "r") Then
                                LegalMoveArray(n) = x & CoorY - 1
                                n += 1
                            End If
                        End If
                    Next
                End If
            End If

        ElseIf Board(CoorX, CoorY) = "N" Then
            If WhiteTFTable(CoorX, CoorY) >= "4" Then
                If CoorX >= 2 Then
                    For x = -1 To 1 Step 2
                        If (CoorY + x) >= 0 And (CoorY + x) <= 7 Then
                            BlackTFTable(CoorX - 2, CoorY + x) = "F"
                            If Board(CoorX - 2, CoorY + x) = " " Or (Board(CoorX - 2, CoorY + x) >= "b" And Board(CoorX - 2, CoorY + x) <= "r") Then
                                LegalMoveArray(n) = (CoorX - 2) & (CoorY + x)
                                n += 1
                            End If
                        End If
                    Next
                End If
                If CoorX <= 5 Then
                    For x = -1 To 1 Step 2
                        If (CoorY + x) >= 0 And (CoorY + x) <= 7 Then
                            BlackTFTable(CoorX + 2, CoorY + x) = "F"
                            If Board(CoorX + 2, CoorY + x) = " " Or (Board(CoorX + 2, CoorY + x) >= "b" And Board(CoorX + 2, CoorY + x) <= "r") Then
                                LegalMoveArray(n) = (CoorX + 2) & (CoorY + x)
                                n += 1
                            End If
                        End If
                    Next
                End If
                If CoorY >= 2 Then
                    For x = -1 To 1 Step 2
                        If (CoorX + x) >= 0 And (CoorX + x) <= 7 Then
                            BlackTFTable(CoorX + x, CoorY - 2) = "F"
                            If Board(CoorX + x, CoorY - 2) = " " Or (Board(CoorX + x, CoorY - 2) >= "b" And Board(CoorX + x, CoorY - 2) <= "r") Then
                                LegalMoveArray(n) = (CoorX + x) & (CoorY - 2)
                                n += 1
                            End If
                        End If
                    Next
                End If
                If CoorY <= 5 Then
                    For x = -1 To 1 Step 2
                        If (CoorX + x) >= 0 And (CoorX + x) <= 7 Then
                            BlackTFTable(CoorX + x, CoorY + 2) = "F"
                            If Board(CoorX + x, CoorY + 2) = " " Or (Board(CoorX + x, CoorY + 2) >= "b" And Board(CoorX + x, CoorY + 2) <= "r") Then
                                LegalMoveArray(n) = (CoorX + x) & (CoorY + 2)
                                n += 1
                            End If
                        End If
                    Next
                End If
            End If


        ElseIf Board(CoorX, CoorY) = "R" Or Board(CoorX, CoorY) = "Q" Then
            If WhiteTFTable(CoorX, CoorY) <> "1" Or WhiteTFTable(CoorX, CoorY) <> "3" Then
                If WhiteTFTable(CoorX, CoorY) <> "0" Then
                    For x = (CoorX + 1) To 7
                        BlackTFTable(x, CoorY) = "F"
                        If Board(x, CoorY) >= "B" And Board(x, CoorY) <= "R" Then Exit For


                        If Board(x, CoorY) = " " Or (Board(x, CoorY) >= "b" And Board(x, CoorY) <= "r") Then
                            LegalMoveArray(n) = (x) & (CoorY)
                            n += 1

                            If (Board(x, CoorY) >= "b" And Board(x, CoorY) <= "r") And AscW(BKPos(0)) > x And AscW(BKPos(1)) = CoorY Then
                                For n = x + 1 To 7
                                    If Board(n, CoorY) >= "B" And Board(n, CoorY) <= "R" Then Exit For
                                    If Board(n, CoorY) = "k" Then
                                        BlackTFTable(n, CoorY) = "2"
                                        Exit For
                                    End If
                                Next
                            End If

                            If Board(x, CoorY) <> " " Then Exit For
                        End If


                    Next
                    For x = (CoorX - 1) To 0 Step -1
                        BlackTFTable(x, CoorY) = "F"
                        If Board(x, CoorY) >= "B" And Board(x, CoorY) <= "R" Then Exit For
                        If Board(x, CoorY) = " " Or (Board(x, CoorY) >= "b" And Board(x, CoorY) <= "r") Then
                            LegalMoveArray(n) = (x) & (CoorY)
                            n += 1

                            If (Board(x, CoorY) >= "b" And Board(x, CoorY) <= "r") And AscW(BKPos(0)) < x And AscW(BKPos(1)) = CoorY Then
                                For n = x - 1 To 0 Step -1
                                    If Board(n, CoorY) >= "B" And Board(n, CoorY) <= "R" Then Exit For
                                    If Board(n, CoorY) = "k" Then
                                        BlackTFTable(n, CoorY) = "2"
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
                        BlackTFTable(CoorX, y) = "F"
                        If Board(CoorX, y) >= "B" And Board(CoorX, y) <= "R" Then Exit For
                        If Board(CoorX, y) = " " Or (Board(CoorX, y) >= "b" And Board(CoorX, y) <= "r") Then
                            LegalMoveArray(n) = (CoorX) & (y)
                            n += 1

                            If (Board(CoorX, y) >= "b" And Board(CoorX, y) <= "r") And AscW(BKPos(0)) = CoorX And AscW(BKPos(1)) > y Then
                                For n = y + 1 To 7
                                    If Board(CoorX, n) >= "B" And Board(CoorX, n) <= "R" Then Exit For
                                    If Board(CoorX, n) = "k" Then
                                        BlackTFTable(CoorX, n) = "0"
                                        Exit For
                                    End If
                                Next
                            End If

                            If Board(CoorX, y) <> " " Then Exit For
                        End If
                    Next
                    For y = (CoorY - 1) To 0 Step -1
                        BlackTFTable(CoorX, y) = "F"
                        If Board(CoorX, y) >= "B" And Board(CoorX, y) <= "R" Then Exit For
                        If Board(CoorX, y) = " " Or (Board(CoorX, y) >= "b" And Board(CoorX, y) <= "r") Then
                            LegalMoveArray(n) = (CoorX) & (y)
                            n += 1

                            If (Board(CoorX, y) >= "b" And Board(CoorX, y) <= "r") And AscW(BKPos(0)) = CoorX And AscW(BKPos(1)) < y Then
                                For n = y - 1 To 0 Step -1
                                    If Board(CoorX, n) >= "B" And Board(CoorX, n) <= "R" Then Exit For
                                    If Board(CoorX, n) = "k" Then
                                        BlackTFTable(CoorX, n) = "0"
                                        Exit For
                                    End If
                                Next
                            End If

                            If Board(CoorX, y) <> " " Then Exit For
                        End If
                    Next
                End If
            End If
        End If

        If Board(CoorX, CoorY) = "B" Or Board(CoorX, CoorY) = "Q" Then
            If WhiteTFTable(CoorX, CoorY) <> "0" Or WhiteTFTable(CoorX, CoorY) <> "2" Then
                If WhiteTFTable(CoorX, CoorY) <> "3" Then
                    StartX = CoorX + 1
                    StartY = CoorY + 1
                    Do Until StartX > 7 Or StartY > 7
                        BlackTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r") Then
                            LegalMoveArray(n) = StartX & StartY
                            n += 1
                            If Board(StartX, StartY) <> " " Then Exit Do
                        End If
                        StartX += 1
                        StartY += 1
                    Loop
                    StartX = CoorX - 1
                    StartY = CoorY - 1
                    Do Until StartX < 0 Or StartY < 0
                        BlackTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r") Then
                            LegalMoveArray(n) = StartX & StartY
                            n += 1
                            If Board(StartX, StartY) <> " " Then Exit Do
                        End If
                        StartX -= 1
                        StartY -= 1
                    Loop
                End If
                If WhiteTFTable(CoorX, CoorY) <> "1" Then
                    StartX = CoorX + 1
                    StartY = CoorY - 1
                    Do Until StartX > 7 Or StartY < 0
                        BlackTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r") Then
                            LegalMoveArray(n) = StartX & StartY
                            n += 1
                            If Board(StartX, StartY) <> " " Then Exit Do
                        End If
                        StartX += 1
                        StartY -= 1
                    Loop
                    StartX = CoorX - 1
                    StartY = CoorY + 1
                    Do Until StartX < 0 Or StartY > 7
                        BlackTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r") Then
                            LegalMoveArray(n) = StartX & StartY
                            n += 1
                            If Board(StartX, StartY) <> " " Then Exit Do
                        End If
                        StartX -= 1
                        StartY += 1
                    Loop
                End If
            End If
        End If

        If n <> 1 Then LegalMoveArray(0) = n
        Return (LegalMoveArray, BlackTFTable)
    End Function


    Function BlackPieceLegalMoves(ByVal Board(,) As Char, ByVal CoorX As Integer, ByVal CoorY As Integer, ByVal BlackTFTable(,) As Char, ByRef WhiteTFTable(,) As Char,ByRef WKPos As string) As (LegalMoveArray As String(), WhiteTFTable As Char(,))
        Dim LegalMoveArray(26) As String
        Dim n As Integer = 1
        Dim StartY As Integer
        Dim EndY As Integer
        Dim StartX As Integer
        Dim EndX As Integer

        If Board(CoorX, CoorY) = "k" Then
            StartX = Math.Max(CoorX - 1, 0)
            EndX = Math.Min(CoorX + 1, 7)
            StartY = Math.Max(CoorY - 1, 0)
            EndY = Math.Min(CoorY + 1, 7)
            'Console.WriteLine(StartX & " " & EndX & " " & StartY & " " & EndY)
            For y = StartY To EndY
                For x = StartX To EndX
                    WhiteTFTable(x, y) = "F"
                    If (Board(x, y) = " " Or (Board(x, y) >= "B" And Board(x, y) <= "R")) And BlackTFTable(x, y) = "T" Then
                        LegalMoveArray(n) = (x) & (y)
                        n += 1
                    End If
                Next
            Next

            If BCanCastle.KS Then
                If Board(CoorX + 1, CoorY) = " " And Board(CoorX + 2, CoorY) = " " And Board(CoorX + 3, CoorY) = "r" And BlackTFTable(CoorX + 1, CoorY) = "T" And BlackTFTable(CoorX + 2, CoorY) = "T" Then
                    LegalMoveArray(n) = (CoorX + 2) & (CoorY)
                    n += 1
                End If
            End If
            If BCanCastle.QS Then
                If Board(CoorX - 1, CoorY) = " " And Board(CoorX - 2, CoorY) = " " And Board(CoorX - 3, CoorY) = " " And Board(CoorX - 4, CoorY) = "r" And BlackTFTable(CoorX - 1, CoorY) = "T" And BlackTFTable(CoorX - 2, CoorY) = "T" Then
                    LegalMoveArray(n) = (CoorX - 2) & (CoorY)
                    n += 1
                End If
            End If


        ElseIf Board(CoorX, CoorY) = "p" Then
            If BlackTFTable(CoorX, CoorY) >= "4" Or BlackTFTable(CoorX, CoorY) = "0" Then
                StartX = Math.Max(CoorX - 1, 0)
                EndX = Math.Min(CoorX + 1, 7)
                If Board(CoorX, CoorY + 1) = " " Then
                    LegalMoveArray(n) = CoorX & CoorY + 1
                    n += 1
                    If CoorY = 1 And Board(CoorX, 3) = " " Then
                        LegalMoveArray(n) = CoorX & 3
                        n += 1
                    End If
                End If
                If BlackTFTable(CoorX, CoorY) <> "0" Then
                    For x = StartX To EndX
                        If x <> CoorX Then
                            WhiteTFTable(x, CoorY + 1) = "F"
                            If (Board(x, CoorY + 1) >= "B" And Board(x, CoorY + 1) <= "R") Then
                                LegalMoveArray(n) = x & CoorY + 1
                                n += 1
                            End If
                        End If
                    Next
                End If
            End If

        ElseIf Board(CoorX, CoorY) = "n" Then
            If BlackTFTable(CoorX, CoorY) >= "4" Then
                If CoorX >= 2 Then
                    For x = -1 To 1 Step 2
                        If (CoorY + x) >= 0 And (CoorY + x) <= 7 Then
                            WhiteTFTable(CoorX - 2, CoorY + x) = "F"
                            If Board(CoorX - 2, CoorY + x) = " " Or (Board(CoorX - 2, CoorY + x) >= "B" And Board(CoorX - 2, CoorY + x) <= "R") Then
                                LegalMoveArray(n) = (CoorX - 2) & (CoorY + x)
                                n += 1
                            End If
                        End If
                    Next
                End If
                If CoorX <= 5 Then
                    For x = -1 To 1 Step 2
                        If (CoorY + x) >= 0 And (CoorY + x) <= 7 Then
                            WhiteTFTable(CoorX + 2, CoorY + x) = "F"
                            If Board(CoorX + 2, CoorY + x) = " " Or (Board(CoorX + 2, CoorY + x) >= "B" And Board(CoorX + 2, CoorY + x) <= "R") Then
                                LegalMoveArray(n) = (CoorX + 2) & (CoorY + x)
                                n += 1
                            End If
                        End If
                    Next
                End If
                If CoorY >= 2 Then
                    For x = -1 To 1 Step 2
                        If (CoorX + x) >= 0 And (CoorX + x) <= 7 Then
                            WhiteTFTable(CoorX + x, CoorY - 2) = "F"
                            If Board(CoorX + x, CoorY - 2) = " " Or (Board(CoorX + x, CoorY - 2) >= "B" And Board(CoorX + x, CoorY - 2) <= "R") Then
                                LegalMoveArray(n) = (CoorX + x) & (CoorY - 2)
                                n += 1
                            End If
                        End If
                    Next
                End If
                If CoorY <= 5 Then
                    For x = -1 To 1 Step 2
                        If (CoorX + x) >= 0 And (CoorX + x) <= 7 Then
                            WhiteTFTable(CoorX + x, CoorY + 2) = "F"
                            If Board(CoorX + x, CoorY + 2) = " " Or (Board(CoorX + x, CoorY + 2) >= "B" And Board(CoorX + x, CoorY + 2) <= "R") Then
                                LegalMoveArray(n) = (CoorX + x) & (CoorY + 2)
                                n += 1
                            End If
                        End If
                    Next
                End If
            End If

        ElseIf Board(CoorX, CoorY) = "r" Or Board(CoorX, CoorY) = "q" Then
            If BlackTFTable(CoorX, CoorY) <> "1" Or BlackTFTable(CoorX, CoorY) <> "3" Then
                If BlackTFTable(CoorX, CoorY) <> "0" Then
                    For x = (CoorX + 1) To 7
                        WhiteTFTable(x, CoorY) = "F"
                        If Board(x, CoorY) >= "b" And Board(x, CoorY) <= "r" Then Exit For
                        If Board(x, CoorY) = " " Or (Board(x, CoorY) >= "B" And Board(x, CoorY) <= "R") Then
                            LegalMoveArray(n) = (x) & (CoorY)
                            n += 1
                            If Board(x, CoorY) <> " " Then Exit For
                        End If
                    Next
                    For x = (CoorX - 1) To 0 Step -1
                        WhiteTFTable(x, CoorY) = "F"
                        If Board(x, CoorY) >= "b" And Board(x, CoorY) <= "r" Then Exit For
                        If Board(x, CoorY) = " " Or (Board(x, CoorY) >= "B" And Board(x, CoorY) <= "R") Then
                            LegalMoveArray(n) = (x) & (CoorY)
                            n += 1
                            If Board(x, CoorY) <> " " Then Exit For
                        End If
                    Next
                End If
            End If

            If BlackTFTable(CoorX, CoorY) <> "2" Then
                For y = (CoorY + 1) To 7
                    WhiteTFTable(CoorX, y) = "F"
                    If Board(CoorX, y) >= "b" And Board(CoorX, y) <= "r" Then Exit For
                    If Board(CoorX, y) = " " Or (Board(CoorX, y) >= "B" And Board(CoorX, y) <= "R") Then
                        LegalMoveArray(n) = (CoorX) & (y)
                        n += 1
                        If Board(CoorX, y) <> " " Then Exit For
                    End If
                Next
                For y = (CoorY - 1) To 0 Step -1
                    WhiteTFTable(CoorX, y) = "F"
                    If Board(CoorX, y) >= "b" And Board(CoorX, y) <= "r" Then Exit For
                    If Board(CoorX, y) = " " Or (Board(CoorX, y) >= "B" And Board(CoorX, y) <= "R") Then
                        LegalMoveArray(n) = (CoorX) & (y)
                        n += 1
                        If Board(CoorX, y) <> " " Then Exit For
                    End If
                Next
            End If
        End If

        If Board(CoorX, CoorY) = "b" Or Board(CoorX, CoorY) = "q" Then
            If BlackTFTable(CoorX, CoorY) <> "0" Or BlackTFTable(CoorX, CoorY) <> "2" Then
                If BlackTFTable(CoorX, CoorY) <> "3" Then
                    StartX = CoorX + 1
                    StartY = CoorY + 1
                    Do Until StartX > 7 Or StartY > 7
                        WhiteTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R") Then
                            LegalMoveArray(n) = StartX & StartY
                            n += 1
                            If Board(StartX, StartY) <> " " Then Exit Do
                        End If
                        StartX += 1
                        StartY += 1
                    Loop
                    StartX = CoorX - 1
                    StartY = CoorY - 1
                    Do Until StartX < 0 Or StartY < 0
                        WhiteTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R") Then
                            LegalMoveArray(n) = StartX & StartY
                            n += 1
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
                        WhiteTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R") Then
                            LegalMoveArray(n) = StartX & StartY
                            n += 1
                            If Board(StartX, StartY) <> " " Then Exit Do
                        End If
                        StartX += 1
                        StartY -= 1
                    Loop
                    StartX = CoorX - 1
                    StartY = CoorY + 1
                    Do Until StartX < 0 Or StartY > 7
                        WhiteTFTable(StartX, StartY) = "F"
                        If Board(StartX, StartY) >= "b" And Board(StartX, StartY) <= "r" Then Exit Do
                        If Board(StartX, StartY) = " " Or (Board(StartX, StartY) >= "B" And Board(StartX, StartY) <= "R") Then
                            LegalMoveArray(n) = StartX & StartY
                            n += 1
                            If Board(StartX, StartY) <> " " Then Exit Do
                        End If
                        StartX -= 1
                        StartY += 1
                    Loop
                End If
            End If
        End If
        If n <> 1 Then LegalMoveArray(0) = n
        Return (LegalMoveArray, WhiteTFTable)
    End Function

    Private MovingPiece As Boolean = False
    Private StartPoint As Point
    Private MidPoint As Point
    Private EndPoint As Point
    Public LegalMoves As String()

    Private Sub Piece_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles WK1.MouseDown, WQ1.MouseDown, WB1.MouseDown, WB2.MouseDown, WN1.MouseDown, WN2.MouseDown, WR1.MouseDown, WR2.MouseDown, WP1.MouseDown, WP2.MouseDown, WP3.MouseDown, WP4.MouseDown, WP5.MouseDown, WP6.MouseDown, WP7.MouseDown, WP8.MouseDown, BK1.MouseDown, BQ1.MouseDown, BB1.MouseDown, BB2.MouseDown, BN1.MouseDown, BN2.MouseDown, BR1.MouseDown, BR2.MouseDown, BP1.MouseDown, BP2.MouseDown, BP3.MouseDown, BP4.MouseDown, BP5.MouseDown, BP6.MouseDown, BP7.MouseDown, BP8.MouseDown, WQ8.MouseDown, WQ7.MouseDown, WQ6.MouseDown, WQ5.MouseDown, WQ4.MouseDown, WQ3.MouseDown, BQ6.MouseDown, BQ8.MouseDown, BQ5.MouseDown, BQ7.MouseDown, BQ2.MouseDown, BQ4.MouseDown, BQ3.MouseDown, WQ2.MouseDown
        'Subroutine that activates the drag + drog mechanic if the user clicks on a piece.
        If e.Button = Windows.Forms.MouseButtons.Left Then
            If CStr(sender.name)(0) = "W" And PlayerTurn Then
                Dim LegalMovesResult = WhitePieceLegalMoves(MasterArray, sender.location.x / 75, sender.location.y / 75, WhiteTFTable, BlackTFTable, BKPos)
                LegalMoves = LegalMovesResult.LegalMoveArray
                MovingPiece = True
            ElseIf CStr(sender.name)(0) = "B" And (Not PlayerTurn) Then
                Dim LegalMovesResult = BlackPieceLegalMoves(MasterArray, sender.location.x / 75, sender.location.y / 75, BlackTFTable, WhiteTFTable, WKPos)
                LegalMoves = LegalMovesResult.LegalMoveArray
                MovingPiece = True
            End If
            Console.WriteLine()
            Console.WriteLine("here are moves")
            If LegalMoves(0) IsNot Nothing Then
                For x = 0 To LegalMoves.Length - 1
                    If LegalMoves(x) IsNot Nothing Then Console.WriteLine(LegalMoves(x))
                Next
            End If
            Console.WriteLine("end of moves")
            sender.bringtofront()
            StartPoint = sender.location
            MidPoint = sender.PointToScreen(New Point(e.X, e.Y))

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
                        IsLegalMove = True
                        Exit For
                    End If
                Next
                If IsLegalMove Then
                    If UCase(sender.name(1)) = "K" Then
                        If sender.name(0) = "W" Then
                            WKPos = (sender.location.x / 75) & (sender.location.y / 75)
                            WCanCastle.KS = False
                            WCanCastle.QS = False
                        Else
                            BKPos = (sender.location.x / 75) & (sender.location.y / 75)
                            BCanCastle.KS = False
                            BCanCastle.QS = False
                        End If
                        If (sender.location.x / 75 & sender.location.y / 75) = ((StartPoint.X / 75) + 2 & StartPoint.Y / 75) Then
                            JustCastled = True
                            Sound_Castle.Play()
                            'Console.WriteLine("ayyyy" & MasterArray(7, sender.location.y / 75) & "oooo")
                            MasterArray(5, sender.location.y / 75) = CStr(MasterArray(7, sender.location.y / 75))
                            MasterArray(7, sender.location.y / 75) = " "
                            If sender.name(0) = "W" Then
                                WR2.Left -= 150
                            Else
                                BR2.Left -= 150
                            End If

                        ElseIf (sender.location.x / 75 & sender.location.y / 75) = ((StartPoint.X / 75) - 2 & StartPoint.Y / 75) Then
                            JustCastled = True
                            Sound_Castle.Play()
                            'Console.WriteLine("ayyyy" & MasterArray(0, sender.location.y / 75) & "oooo")
                            MasterArray(3, sender.location.y / 75) = CStr(MasterArray(0, sender.location.y / 75))
                            MasterArray(0, sender.location.y / 75) = " "
                            If sender.name(0) = "W" Then
                                WR1.Left += 225
                            Else
                                BR1.Left += 225
                            End If

                        End If

                    ElseIf UCase(sender.name(1)) = "R" Then
                        If sender.name(0) = "W" Then
                            If sender.name(2) = "1" Then
                                WCanCastle.QS = False
                            Else
                                WCanCastle.KS = False
                            End If
                        Else
                            If sender.name(2) = "1" Then
                                BCanCastle.QS = False
                            Else
                                BCanCastle.KS = False
                            End If
                        End If
                    End If
                    If MasterArray(sender.location.X / 75, sender.location.Y / 75) <> " " Then
                        'If the move is a capture, the required sound is played and the captured piece is removed from the board.
                        Sound_Capture.Play()
                        ReplacedPiece = MarrToPieceConv(sender.location.X, sender.location.Y, CStr(sender.name)(0))
                        ReplacedPiece.Visible = False
                        ReplacedPiece.Location = New Point(-100, -100)
                    ElseIf JustCastled = False Then
                        Sound_Move.Play()
                    End If
                    'MasterArray() is updated by clearing the piece's previous piosition and reallocating its current position.
                    MasterArray(StartPoint.X / 75, StartPoint.Y / 75) = " "

                    If (CStr(sender.name))(0) = "W" Then

                        If sender.name(1) = "P" And sender.location.y / 75 = 0 Then
                            For n = 1 To 8
                                ReplacedPiece = Me.Controls.Find("WQ" & n.ToString, True).Single()
                                If ReplacedPiece.Visible = False Then Exit For
                            Next
                            ReplacedPiece.Location = New Point(sender.location.X, sender.location.Y)
                            ReplacedPiece.Visible = True
                            ReplacedPiece.BringToFront()
                            MasterArray(sender.location.X / 75, sender.location.Y / 75) = "Q"
                            sender.Visible = False
                            sender.Location = New Point(-100, -100)
                        Else
                            MasterArray(sender.location.X / 75, sender.location.Y / 75) = UCase((CStr(sender.name))(1))
                        End If

                        FixTFTables(MasterArray, False)
                        Console.WriteLine("King Pos: " & WKPos & " " & BKPos)

                    Else
                        If sender.name(1) = "P" And sender.location.y / 75 = 7 Then
                            For n = 1 To 8
                                ReplacedPiece = Me.Controls.Find("BQ" & n.ToString, True).Single()
                                If ReplacedPiece.Visible = False Then Exit For
                            Next
                            ReplacedPiece.Location = New Point(sender.location.X, sender.location.Y)
                            ReplacedPiece.Visible = True
                            MasterArray(sender.location.X / 75, sender.location.Y / 75) = "q"
                            sender.Visible = False
                            sender.Location = New Point(-100, -100)
                        Else
                            MasterArray(sender.location.X / 75, sender.location.Y / 75) = LCase((CStr(sender.name))(1))
                        End If

                        FixTFTables(MasterArray, True)
                        Console.WriteLine("King Pos: " & WKPos & " " & BKPos)

                    End If
                    For y = 0 To 7
                        For x = 0 To 7
                            Console.Write(MasterArray(x, y))
                        Next
                        Console.WriteLine()
                    Next
                    PlayerTurn = Not PlayerTurn

                Else
                    sender.location = StartPoint
                End If
            End If
        End If
    End Sub


    Private Sub FixTFTables(ByRef Board(,) As Char, FixWhite As Boolean)
        If FixWhite Then
            Array.Copy(TrueTable, WhiteTFTable, TrueTable.Length)
            For y = 0 To 7
                For x = 0 To 7
                    If Board(x, y) <> " " Then
                        If Board(x, y) >= "b" And Board(x, y) <= "r" Then
                            Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, BlackTFTable, WhiteTFTable, WKPos)
                            'WhiteTFTable = LegalMovesResult.WhiteTFTable
                        End If
                    End If
                Next
            Next
        Else
            Array.Copy(TrueTable, BlackTFTable, TrueTable.Length)
            For y = 0 To 7
                For x = 0 To 7
                    If Board(x, y) <> " " Then
                        If Board(x, y) >= "B" And Board(x, y) <= "R" Then
                            Dim LegalMovesResult = WhitePieceLegalMoves(Board, x, y, WhiteTFTable, BlackTFTable, BKPos)
                            'BlackTFTable = LegalMovesResult.BlackTFTable
                        End If
                    End If
                Next
            Next
        End If

        For y = 0 To 7
            For x = 0 To 7
                Console.Write(WhiteTFTable(x, y))
            Next
            Console.WriteLine()
        Next
        Console.WriteLine("HELLO")
        For y = 0 To 7
            For x = 0 To 7
                Console.Write(BlackTFTable(x, y))
            Next
            Console.WriteLine()
        Next

    End Sub


    Dim TempMove As Char
    Sub MakeMove(ByRef Board(,) As Char, ByVal x As Integer, ByVal y As Integer, ByVal NewCoor As String)
        TempMove = Board(CStr(NewCoor(0)), CStr(NewCoor(1)))
        Board(CStr(NewCoor(0)), CStr(NewCoor(1))) = Board(x, y)
        Board(x, y) = " "
    End Sub

    Sub UnMakeMove(ByRef Board(,) As Char, ByVal x As Integer, ByVal y As Integer, ByVal NewCoor As String)
        Board(x, y) = Board(CStr(NewCoor(0)), CStr(NewCoor(1)))
        Board(CStr(NewCoor(0)), CStr(NewCoor(1))) = TempMove
    End Sub

    Dim p As Integer
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim Stopwatch As New Stopwatch
        Dim Moves(27, 15) As String

        Stopwatch.Start()
        Moves = CreateMoves(MasterArray, True)
        Stopwatch.Stop()
        Console.WriteLine(vbCrLf)
        Console.WriteLine(Stopwatch.Elapsed.TotalMilliseconds)
        For x = 0 To 26
            If Moves(x, 0) IsNot Nothing Then
                For y = 0 To Moves.GetLength(1) - 1
                    Console.Write(Moves(x, y) & " ")
                Next
                Console.WriteLine()
            End If
        Next
        Console.WriteLine()
        Console.WriteLine("WhiteTFTable:")
        For y = 0 To 7
            For x = 0 To 7
                Console.Write(WhiteTFTable(x, y))
            Next
            Console.WriteLine()
        Next
        Console.WriteLine("BlackTFTable:")
        For y = 0 To 7
            For x = 0 To 7
                Console.Write(BlackTFTable(x, y))
            Next
            Console.WriteLine()
        Next


        'Moves(,) = CreateMoves.Clone() as double[,]

        'p = 0
        'Dim stopwatch As New Stopwatch
        'stopwatch.Start()
        'MoveCalculator(1, True)
        'Console.WriteLine(p)
        'stopwatch.Stop()
        'Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds)
    End Sub

    'Private Sub MoveCalculator(ByVal depth As Integer, W As Boolean)
    '    Dim Board(7, 7) As Char
    '    Array.Copy(MasterArray, Board, 64)



    '    If depth <= 4 Then
    '        If W Then
    '            For y = 0 To 7
    '                For x = 0 To 7
    '                    If Board(x, y) >= "B" And Board(x, y) <= "R" Then
    '                        Dim LegalMoves = WhitePieceLegalMoves(Board, x, y, WhiteTFTable)
    '                        If LegalMoves.LegalMoveArray(0) IsNot Nothing Then
    '                            For n = 0 To LegalMoves.LegalMoveArray.Length - 1
    '                                If LegalMoves.LegalMoveArray(n) Is Nothing Then Exit For
    '                                MakeMove(Board, x, y, LegalMoves.LegalMoveArray(n))
    '                                p += 1
    '                                MoveCalculator(depth + 1, Not W)
    '                                UnMakeMove(Board, x, y, LegalMoves.LegalMoveArray(n))
    '                                'MasterArray = FENConverter(StartingFEN)
    '                            Next
    '                        End If
    '                    End If
    '                Next
    '            Next
    '        Else
    '            For y = 0 To 7
    '                For x = 0 To 7
    '                    If Board(x, y) >= "b" And Board(x, y) <= "r" Then
    '                        LegalMoves = BlackPieceLegalMoves(Board, x, y, BlackTFTable)
    '                        If LegalMoves(0) IsNot Nothing Then
    '                            For n = 0 To LegalMoves.Length - 1
    '                                If LegalMoves(n) Is Nothing Then Exit For
    '                                MakeMove(Board, x, y, LegalMoves(n))
    '                                p += 1
    '                                MoveCalculator(depth + 1, Not W)
    '                                UnMakeMove(Board, x, y, LegalMoves(n))
    '                                'MasterArray = FENConverter(StartingFEN)
    '                            Next
    '                        End If
    '                    End If
    '                Next
    '            Next
    '        End If
    '    End If
    'End Sub



    Private Function CreateMoves(ByRef Board(,) As Char, isWhite As Boolean) As String(,)
        Dim Moves(27, 15) As String
        Dim counter As Integer = 0

        If isWhite Then
            Array.Copy(TrueTable, WhiteTFTable, TrueTable.Length)
            For y = 0 To 7
                For x = 0 To 7
                    If Board(x, y) >= "B" And Board(x, y) <= "R" Then
                        Dim LegalMovesResult = WhitePieceLegalMoves(Board, x, y, WhiteTFTable, BlackTFTable, BKPos)
                        LegalMoves = LegalMovesResult.LegalMoveArray
                        'BlackTFTable = LegalMovesResult.BlackTFTable
                        If LegalMoves(0) IsNot Nothing Then
                            Moves(counter, 0) = x & y
                            For n = 1 To CInt(LegalMoves(0)) - 1
                                Moves(counter, n) = LegalMoves(n)
                            Next
                            counter += 1
                        End If
                    End If
                Next
            Next
        Else
            Array.Copy(TrueTable, BlackTFTable, TrueTable.Length)
            For y = 0 To 7
                For x = 0 To 7
                    If Board(x, y) >= "b" And Board(x, y) <= "r" Then
                        Dim LegalMovesResult = BlackPieceLegalMoves(Board, x, y, BlackTFTable, WhiteTFTable, WKPos)
                        LegalMoves = LegalMovesResult.LegalMoveArray
                        'BlackTFTable = LegalMovesResult.WhiteTFTable
                        If LegalMoves(0) IsNot Nothing Then
                            Moves(counter, 0) = x & y
                            For n = 1 To CInt(LegalMoves(0)) - 1
                                Moves(counter, n) = LegalMoves(n)
                            Next
                            counter += 1
                        End If
                    End If
                Next
            Next
        End If

        'Console.WriteLine("WhiteTFTable:")
        'For y = 0 To 7
        '    For x = 0 To 7
        '        Console.Write(WhiteTFTable(x, y))
        '    Next
        '    Console.WriteLine()
        'Next
        'Console.WriteLine("BlackTFTable:")
        'For y = 0 To 7
        '    For x = 0 To 7
        '        Console.Write(BlackTFTable(x, y))
        '    Next
        '    Console.WriteLine()
        'Next

        Return Moves

    End Function



    'Sends the FEN a user types in to be converted & displayed.
    Private Sub FENButton_Click(sender As Object, e As EventArgs) Handles FENButton.Click
        MasterArray = FENConverter(FENTextBox.Text)
        DisplayPieces()
        If PlayerTurn Then
            FixTFTables(MasterArray, True)
        Else
            FixTFTables(MasterArray, False)
        End If
    End Sub

End Class