Imports System.Threading
Public Class Chess 'i call thy Bryson because you are very bad.
    'ew- danny

    'Working Version of Chess, along with AI using MiniMax and Alpha Beta Pruning. All created by Alfie Kunz.

    'Here are the computer's primary "eyes" - MasterBoard (which contains each piece's location on the board) and,
    'derived from this, Master TrueFalse Tables for each player. These Tables display information such as Legal
    'Moves for the players' kings, along with pinned pieces and their location, and make handling Move Generation,
    'Checks and Evaluations much easier and more efficient.
    Protected GameRunning As Boolean = True
    Protected GameMode As Byte = 3
    Protected MasterBoard(7, 7) As Char
    Protected MasterWhiteTFTable(7, 7) As Char
    Protected MasterBlackTFTable(7, 7) As Char
    'TrueTables are just TrueFalse Tables containing only the letter T. Very useful for resetting TrueFalse Tables
    'and debugging.
    Protected MasterTrueTable(7, 7) As Char
    Protected TrueTable(7, 7) As Char
    ReadOnly PieceArray(47) As PictureBox
    Protected PrimaryColour As Color 'Colour representing the dark squares on the Chessboard.
    Protected SecondaryColour As Color 'Colour representing the light squares on the Chessboard.
    Protected SquareHistory(3, 1) As SByte
    'Standard Chess notation that displays where all the pieces on the board are supposed to go.
    Protected StartingFEN As String = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
    Protected CurrentFEN As String
    Protected PreviousFEN As String
    Protected InvalidFEN As String
    Protected PlayerTurn As Boolean
    Protected UserPlayer As Boolean = True
    Protected MasterEnPassant As String
    Protected MasterWKPos As String
    Protected MasterBKPos As String
    Protected MasterMaterialCount As Int16
    Protected AbsoluteDepth As SByte
    Protected PositionsSearched As UInt64 = 0
    Protected ComputerIsSearching As Boolean = False
    Protected Progress As Byte
    Protected OrientForWhite As Boolean = True

    Structure PieceInfo
        Dim IsMovingPiece As Boolean
        Dim StartPoint As Point
        Dim MidPoint As Point
        Dim EndPoint As Point
    End Structure
    Protected PieceMoving As PieceInfo

    Private LegalMoves As String()


    Protected PieceValue(9) As Decimal 'Array Containing the Value or Weight of each Piece.

    'Structures containing information about castling + checks.
    Structure CanCastle
        Dim KS As Boolean 'King Side Castling
        Dim QS As Boolean 'Queen Side Castling
    End Structure
    Protected MasterWCanCastle As CanCastle
    Protected CannotCastle As CanCastle
    Protected MasterBCanCastle As CanCastle
    Structure InCheck
        Dim IsInCheck As Boolean
        Dim Piece As String
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

    'Sets up sound effects. All sounds taken from Chess.com.
    ReadOnly Sound_Move As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_Move.wav"
    }
    ReadOnly Sound_Capture As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_Capture.wav"
    }
    ReadOnly Sound_Check As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_Check.wav"
    }
    ReadOnly Sound_Castle As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_Castle.wav"
    }
    ReadOnly Sound_Checkmate As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_Checkmate.wav"
    }
    ReadOnly Sound_Stalemate As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_Stalemate.wav"
    }

    Protected Stopwatch As New Stopwatch





    Public Sub New()
        InitializeComponent() ' This call is required by the designer.
        GameMode = 3
        StartingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
    End Sub

    Public Sub New(ByVal Mode As Byte, ByVal UserStartingFEN As String, ByVal Difficulty As Byte, ByVal PlayAsWhite As Boolean)
        InitializeComponent() ' This call is required by the designer.
        GameMode = Mode
        Me.Size = New Size(916, 639)
        WhiteAIMove.Visible = False
        BlackAIMove.Visible = False
        FENTextBox.ReadOnly = True
        Reset_Btn.Visible = False
        FENButton.Visible = False
        FENExport.Visible = False
        UndoMove.Top -= 80
        CheckLabel.Location = New Point(89, 128)
        ColourChangerButton.Location = New Point(40, 520)
        ExitBtn.Location = New Point(181, 520)
        Credits.Location = New Point(55, 575)
        If Mode = 1 Then
            FlipperButton.Visible = False
            AutoFlipper.Visible = False
            ProgressBar.Location = New Point(46, 250)
            CurrentEval.Location = New Point(ProgressBar.Location.X + 97, ProgressBar.Location.Y + 37)
            StartingFEN = UserStartingFEN
            UserPlayer = PlayAsWhite
        Else
            FlipperButton.Top -= 100
            AutoFlipper.Top -= 100
            ProgressBar.Visible = False
            CurrentEval.Visible = False
            AutoFlipper.Checked = True
            StartingFEN = UserStartingFEN
        End If
    End Sub


    Private Sub Form_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Application.EnableVisualStyles()
        'Assigns each piece to PieceArray and sets the 'board' picturebox as their parent.
        Stopwatch.Start()
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

        UndoFENChange.Visible = False
        CannotCastle.KS = False
        CannotCastle.QS = False
        NotInCheck.IsInCheck = False
        NotInCheck.Piece = " "
        NotInCheck.DoubleCheck = False
        MasterWInCheck.Piece = " "
        MasterWInCheck.DoubleCheck = False
        MasterBInCheck.Piece = " "
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

        CheckLabel.Text = "                    "
        For n = 0 To 3
            SquareHistory(n, 0) = -1
            SquareHistory(n, 1) = -1
        Next

        'If the FEN is valid, we send the start postion to be converted to the Board array, then display the pieces on the board.
        'If it is not valid, we replace the FEN with the Default StartingFEN.
        Dim FENErrorMessage As String
        Dim FENIsValid As Boolean = False
        Dim CustomisationForm As Form
        If GameMode = 1 Then
            CustomisationForm = CType(Application.OpenForms("OnePlayerCustomisation"), OnePlayerCustomisation)
        ElseIf GameMode = 2 Then
            CustomisationForm = CType(Application.OpenForms("TwoPlayerCustomisation"), TwoPlayerCustomisation)
        End If
        If FENErrorDetection(StartingFEN, False, FENErrorMessage) Then
            Try
                MasterBoard = FENConverter(StartingFEN)
                FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
                Array.Copy(MasterTrueTable, MasterBlackTFTable, 64)
                CalculateAbsoluteDepth()
                MasterMaterialCount = CountMaterial(MasterBoard, False)
                CurrentEval.Text = MasterMaterialCount
                DisplayPieces()
                If GameMode < 3 Then CustomisationForm.Close()
                FENIsValid = True
            Catch ex As Exception
                If MsgBox("Starting Position Rejected - Invalid FEN Code. Please Input a Genuinine FEN and try again." & vbCr & "Please Click 'Retry' to enter another FEN, or Click 'Cancel' to Start the Game with the Standard Starting Position.", vbCritical + vbRetryCancel + vbApplicationModal) = 4 Then
                    Me.Close()
                End If
            End Try
        Else
            If MsgBox("Starting " & FENErrorMessage & vbCr & "Please Click 'Retry' to Enter Another FEN, or Click 'Cancel' to Start the Game with the Standard Starting Position.", vbCritical + vbRetryCancel + vbApplicationModal) = 4 Then
                Me.Close()
            Else
            End If
        End If
        If Not FENIsValid AndAlso Application.OpenForms().OfType(Of Chess).Any Then
            CustomisationForm.Close()
            StartingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
            MasterBoard = FENConverter(StartingFEN)
            FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterEnPassant)
            Array.Copy(MasterTrueTable, MasterBlackTFTable, 64)
            CalculateAbsoluteDepth()
            MasterMaterialCount = CountMaterial(MasterBoard, False)
            CurrentEval.Text = MasterMaterialCount
            DisplayPieces()
        End If

        If GameMode < 3 Then
            If GameMode = 1 AndAlso Not UserPlayer Then
                FlipBoard()
                WhiteAIMove_Click()
                StartingFEN = ConvertToFEN()
            End If
            FENTextBox.Text = StartingFEN
            FENTextBox.Select(FENTextBox.Text.Length, 0)
        End If
        CurrentFEN = StartingFEN
        PreviousFEN = StartingFEN

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
                    If (x = SquareHistory(0, 0) AndAlso y = SquareHistory(0, 1)) OrElse (x = SquareHistory(1, 0) AndAlso y = SquareHistory(1, 1)) Then
                        Using Brush As New SolidBrush(Color.YellowGreen)
                            g.FillRectangle(Brush, square)
                        End Using
                    Else
                        Using Brush As New SolidBrush(SecondaryColour)
                            g.FillRectangle(Brush, square)
                        End Using
                    End If
                Else
                    If (x = SquareHistory(0, 0) AndAlso y = SquareHistory(0, 1)) OrElse (x = SquareHistory(1, 0) AndAlso y = SquareHistory(1, 1)) Then
                        Using Brush As New SolidBrush(Color.OliveDrab)
                            g.FillRectangle(Brush, square)
                        End Using
                    Else
                        Using Brush As New SolidBrush(PrimaryColour)
                            g.FillRectangle(Brush, square)
                        End Using
                    End If
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
        For x = 0 To 47
            PieceArray(x).Location = New Point(-100, -100)
            PieceArray(x).Visible = False
        Next
        For y = 7 To 0 Step -1
            For x = 0 To 7
                If MasterBoard(x, y) <> " " Then
                    n = 1
                    While True
                        'In MasterBoard(), a lowercase letter represents a black piece, and an uppercase letter represents a white piece.
                        If Char.IsLower(MasterBoard(x, y)) Then
                            CurrentPiece = "B"
                        Else
                            CurrentPiece = "W"
                        End If
                        'CurrentPiece is created, which has the same name as a corresponding piece on the board.
                        CurrentPiece &= UCase(MasterBoard(x, y)) & CStr(n)
                        'Searches for piece that has not been placed on the board (that matches CurrentPiece / TempPiece's name), and moves its match into place on the board.
                        TempPiece = Me.Controls.Find(CurrentPiece, True).Single()
                        If TempPiece.Visible = False Then
                            If OrientForWhite Then
                                TempPiece.Location = New Point(((x) * 75), ((75 * y)))
                            Else
                                TempPiece.Location = New Point(((7 - x) * 75), ((75 * (7 - y))))
                            End If

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
    Private Function CoorToPieceConverter(ByVal CoorX As Int16, ByVal CoorY As Int16, ByVal PieceColour As Char) As PictureBox
        Dim CurrentPiece As String
        Dim TempPiece As PictureBox
        Dim n As Byte
        If MasterBoard(CoorX, CoorY) <> " " Then
            n = 1
            While True
                'In MasterBoard(), a lowercase letter represents a black piece, and an uppercase letter represents a white piece.
                If Char.IsLower(MasterBoard(CoorX, CoorY)) Then
                    CurrentPiece = "B"
                Else
                    CurrentPiece = "W"
                End If
                'CurrentPiece is created, which has the same name as a corresponding piece on the board.
                CurrentPiece &= UCase(MasterBoard(CoorX, CoorY)) & CStr(n)
                'Searches for piece that has not been placed on the board (that matches CurrentPiece / TempPiece's name + location), and moves its match into place on the board.
                TempPiece = Me.Controls.Find(CurrentPiece, True).Single()
                If OrientForWhite AndAlso (CInt(TempPiece.Location.X) = CoorX * 75 AndAlso CInt(TempPiece.Location.Y) = CoorY * 75 AndAlso TempPiece.Name(0) <> PieceColour) Then
                    Exit While
                ElseIf Not OrientForWhite AndAlso (CInt(TempPiece.Location.X) = (7 - CoorX) * 75 AndAlso CInt(TempPiece.Location.Y) = (7 - CoorY) * 75 AndAlso TempPiece.Name(0) <> PieceColour) Then
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
                    If FEN(n) = "K" Then
                        MasterWKPos = x & y
                    ElseIf FEN(n) = "k" Then
                        MasterBKPos = x & y
                    End If
                    x = x + 1
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
        If MasterWCanCastle.QS AndAlso Not (tempArray(4, 7) = "K" AndAlso tempArray(0, 7) = "R") Then MasterWCanCastle.QS = False
        If MasterWCanCastle.KS AndAlso Not (tempArray(4, 7) = "K" AndAlso tempArray(7, 7) = "R") Then MasterWCanCastle.KS = False
        If MasterBCanCastle.QS AndAlso Not (tempArray(4, 0) = "k" AndAlso tempArray(0, 0) = "r") Then MasterBCanCastle.QS = False
        If MasterBCanCastle.KS AndAlso Not (tempArray(4, 0) = "k" AndAlso tempArray(7, 0) = "r") Then MasterBCanCastle.KS = False
        Return tempArray
    End Function


    'Function which converts the current board position into its FEN counterpart.
    Private Function ConvertToFEN() As String
        Dim Counter As Byte = 0
        ConvertToFen = ""
        For y = 0 To 7
            For x = 0 To 7
                If MasterBoard(x, y) = " " Then
                    Counter += 1
                Else
                    If Counter > 0 Then
                        ConvertToFen &= Counter
                        Counter = 0
                    End If
                    ConvertToFen &= MasterBoard(x, y)
                End If
            Next
            If Counter > 0 Then ConvertToFen &= Counter
            ConvertToFen &= "/"
            Counter = 0
        Next
        ConvertToFen = ConvertToFen.TrimEnd("/")
        If PlayerTurn Then
            ConvertToFen &= " w "
        Else
            ConvertToFen &= " b "
        End If
        If MasterWCanCastle.KS Then ConvertToFen &= "K"
        If MasterWCanCastle.QS Then ConvertToFen &= "Q"
        If MasterBCanCastle.KS Then ConvertToFen &= "k"
        If MasterBCanCastle.QS Then ConvertToFen &= "q"
        If ConvertToFen.EndsWith(" ") Then ConvertToFen &= "-"
        If MasterEnPassant <> "-" Then
            ConvertToFen &= " " & Chr(CStr(MasterEnPassant(0)) + 97) & 8 - CStr(MasterEnPassant(1)) & " 0 1"
        Else
            ConvertToFen &= " - 0 1"
        End If
        Return ConvertToFEN
    End Function



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
            If FENTextBox.Text(1) <> "o" AndAlso FENErrorDetection(FENTextBox.Text, True, "") Then
                If UndoFENChange.Visible = True Then UndoFENChange.Visible = False
                Console.Clear()
                MasterWInCheck.IsInCheck = False
                MasterWInCheck.Piece = " "
                MasterWInCheck.DoubleCheck = False
                MasterBInCheck.IsInCheck = False
                MasterBInCheck.Piece = " "
                MasterBInCheck.DoubleCheck = False
                Dim TempFEN As String = PreviousFEN
                'If the FEN is invalid, then the board is reset to the previous position.
                Try
                    PreviousFEN = CurrentFEN
                    CurrentFEN = FENTextBox.Text
                    MasterBoard = FENConverter(FENTextBox.Text)
                    DisplayPieces()
                Catch
                    InvalidFEN = FENTextBox.Text
                    CurrentFEN = PreviousFEN
                    PreviousFEN = TempFEN
                    MasterBoard = FENConverter(CurrentFEN)
                    DisplayPieces()
                    UndoFENChange.Visible = True
                    FENTextBox.Text = "Position Rejected - Invalid FEN Code. Please Input a Genuinine FEN and try again."
                End Try
                SquareHistory(2, 0) = SquareHistory(0, 0)
                SquareHistory(2, 1) = SquareHistory(0, 1)
                SquareHistory(3, 0) = SquareHistory(1, 0)
                SquareHistory(3, 1) = SquareHistory(1, 1)
                SquareHistory(0, 0) = -1
                SquareHistory(0, 1) = -1
                SquareHistory(1, 0) = -1
                SquareHistory(1, 1) = -1
                If PlayerTurn AndAlso Not OrientForWhite OrElse Not PlayerTurn AndAlso OrientForWhite Then
                    FlipBoard()
                Else
                    Checkerboard.Refresh()
                End If
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
                ElseIf Math.Abs(CStr(MasterWKPos(0)) - CStr(MasterBKPos(0))) = 1 AndAlso Math.Abs(CStr(MasterWKPos(1)) - CStr(MasterBKPos(1))) = 1 Then
                    GameRunning = False
                    CheckLabel.Text = " Stalemate! "
                    Sound_Stalemate.Play()
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
        End If
    End Sub

    'Subroutine that detects some errors in a user-entered FEN string.
    Private Function FENErrorDetection(ByVal FEN As String, ByVal OutputToBox As Boolean, ByRef FENErrorMessage As String) As Boolean
        Dim MaxWQueens As Byte = 0
        Dim MaxBQueens As Byte = 0
        Dim WHasKing As Boolean = False
        Dim BHasKing As Boolean = False
        If FEN.Length > 22 Then
            For n = 0 To Len(FEN) - 1
                If FEN(n) = " " Then Exit For
                If FEN(n) = "P" OrElse FEN(n) = "Q" Then
                    MaxWQueens += 1
                ElseIf FEN(n) = "p" OrElse FEN(n) = "q" Then
                    MaxBQueens += 1
                ElseIf FEN(n) = "K" Then
                    WHasKing = True
                ElseIf FEN(n) = "k" Then
                    BHasKing = True
                End If
            Next
            If Not (WHasKing AndAlso BHasKing) Then
                If OutputToBox Then
                    InvalidFEN = FEN
                    FENTextBox.Text = "Position Rejected - No King. Please Input a Genuinine FEN and try again."
                    UndoFENChange.Visible = True
                Else
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
            MasterWInCheck.Piece = " "
            MasterWInCheck.DoubleCheck = False
            MasterBInCheck.IsInCheck = False
            MasterBInCheck.Piece = " "
            MasterBInCheck.DoubleCheck = False
            MasterBoard = FENConverter(CurrentFEN)
            DisplayPieces()
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
        If Not ComputerIsSearching AndAlso Not (CurrentFEN = PreviousFEN) Then
            Dim TempFEN As String
            Dim TempSH(1, 1) As SByte
            TempFEN = CurrentFEN
            CurrentFEN = PreviousFEN
            PreviousFEN = TempFEN
            MasterWInCheck.IsInCheck = False
            MasterWInCheck.Piece = " "
            MasterWInCheck.DoubleCheck = False
            MasterBInCheck.IsInCheck = False
            MasterBInCheck.Piece = " "
            MasterBInCheck.DoubleCheck = False
            MasterBoard = FENConverter(CurrentFEN)
            DisplayPieces()

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
                Checkerboard.Refresh()
            End If

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
            If GameMode < 3 Then FENTextBox.Text = CurrentFEN

            AbsoluteDepth = 0
            If PlayerTurn Then
                WhiteAIMove_Click()
            Else
                BlackAIMove_Click()
            End If
            CalculateAbsoluteDepth()
        End If
    End Sub

    'Subroutine which rotates the board 180 degreees in favour of the other player.
    Private Sub FlipBoard() Handles FlipperButton.Click
        OrientForWhite = Not OrientForWhite
        SquareHistory(0, 0) = 7 - SquareHistory(0, 0)
        SquareHistory(0, 1) = 7 - SquareHistory(0, 1)
        SquareHistory(1, 0) = 7 - SquareHistory(1, 0)
        SquareHistory(1, 1) = 7 - SquareHistory(1, 1)
        SquareHistory(2, 0) = 7 - SquareHistory(2, 0)
        SquareHistory(2, 1) = 7 - SquareHistory(2, 1)
        SquareHistory(3, 0) = 7 - SquareHistory(3, 0)
        SquareHistory(3, 1) = 7 - SquareHistory(3, 1)
        DisplayPieces()
        Checkerboard.Refresh()
    End Sub

    'Subroutines Controlling each Colour Scheme Option change.
    Private Sub ColourChangerButton_Click() Handles ColourChangerButton.Click
        ColourChanger.Show(ColourChangerButton, 0, ColourChangerButton.Height)
    End Sub
    Private Sub DefColourChanger() Handles Def.Click
        PrimaryColour = Color.Peru
        SecondaryColour = Color.FloralWhite
        Checkerboard.Refresh()
        FileOpen(1, Application.StartupPath & "\ColourPreferences.txt", OpenMode.Output)
        PrintLine(1, "def")
        FileClose(1)
    End Sub
    Private Sub BluColourChanger() Handles Blu.Click
        PrimaryColour = Color.LightSteelBlue
        SecondaryColour = Color.AliceBlue
        Checkerboard.Refresh()
        FileOpen(1, Application.StartupPath & "\ColourPreferences.txt", OpenMode.Output)
        PrintLine(1, "blu")
        FileClose(1)
    End Sub
    Private Sub Bl2ColourChanger() Handles Bl2.Click
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

    'Button that returns the user back to the Main Menu. 
    Private Sub ExitBtn_Click() Handles ExitBtn.Click
        Dim Menu As Form = CType(Application.OpenForms("MainMenu"), MainMenu)
        Me.Close()
        Menu.Show()
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


    Function BlackPieceLegalMoves(ByRef Board(,) As Char, ByVal CoorX As SByte, ByVal CoorY As SByte, ByRef BlackTFTable(,) As Char, ByRef WhiteTFTable(,) As Char, ByRef WKPos As String, ByRef WInCheck As InCheck, ByRef BInCheck As InCheck, ByRef BCanCastle As CanCastle, ByRef EnPassant As String) As String()
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




    Private Sub Piece_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles WK1.MouseDown, WQ1.MouseDown, WB1.MouseDown, WB2.MouseDown, WN1.MouseDown, WN2.MouseDown, WR1.MouseDown, WR2.MouseDown, WP1.MouseDown, WP2.MouseDown, WP3.MouseDown, WP4.MouseDown, WP5.MouseDown, WP6.MouseDown, WP7.MouseDown, WP8.MouseDown, BK1.MouseDown, BQ1.MouseDown, BB1.MouseDown, BB2.MouseDown, BN1.MouseDown, BN2.MouseDown, BR1.MouseDown, BR2.MouseDown, BP1.MouseDown, BP2.MouseDown, BP3.MouseDown, BP4.MouseDown, BP5.MouseDown, BP6.MouseDown, BP7.MouseDown, BP8.MouseDown, WQ8.MouseDown, WQ7.MouseDown, WQ6.MouseDown, WQ5.MouseDown, WQ4.MouseDown, WQ3.MouseDown, BQ6.MouseDown, BQ8.MouseDown, BQ5.MouseDown, BQ7.MouseDown, BQ2.MouseDown, BQ4.MouseDown, BQ3.MouseDown, WQ2.MouseDown, WQ9.MouseDown, BQ9.MouseDown
        'Subroutine that activates the drag + drog mechanic if the user clicks on a piece.
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso GameRunning Then
            'Generates the Legal Moves for the piece the user has picked up.
            Dim TempMemoryStream As New System.IO.MemoryStream(My.Resources.Cursor16)
            If CStr(sender.name)(0) = "W" AndAlso PlayerTurn Then
                Me.Cursor = New Cursor(TempMemoryStream)
                If OrientForWhite Then
                    LegalMoves = WhitePieceLegalMoves(MasterBoard, sender.location.x / 75, sender.location.y / 75, MasterWhiteTFTable, MasterBlackTFTable, MasterBKPos, MasterBInCheck, MasterWInCheck, MasterWCanCastle, MasterEnPassant)
                Else
                    LegalMoves = WhitePieceLegalMoves(MasterBoard, 7 - (sender.location.x / 75), 7 - (sender.location.y / 75), MasterWhiteTFTable, MasterBlackTFTable, MasterBKPos, MasterBInCheck, MasterWInCheck, MasterWCanCastle, MasterEnPassant)
                End If
                PieceMoving.IsMovingPiece = True
            ElseIf CStr(sender.name)(0) = "B" AndAlso (Not PlayerTurn) Then
                Me.Cursor = New Cursor(TempMemoryStream)
                If OrientForWhite Then
                    LegalMoves = BlackPieceLegalMoves(MasterBoard, sender.location.x / 75, sender.location.y / 75, MasterBlackTFTable, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterBCanCastle, MasterEnPassant)
                Else
                    LegalMoves = BlackPieceLegalMoves(MasterBoard, 7 - (sender.location.x / 75), 7 - (sender.location.y / 75), MasterBlackTFTable, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterBCanCastle, MasterEnPassant)
                End If

                PieceMoving.IsMovingPiece = True
            End If
        sender.bringtofront()
        PieceMoving.StartPoint = sender.location
        PieceMoving.MidPoint = sender.PointToScreen(New Point(e.X, e.Y))
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

    Private Sub Piece_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles WK1.MouseMove, WQ1.MouseMove, WB1.MouseMove, WB2.MouseMove, WN1.MouseMove, WN2.MouseMove, WR1.MouseMove, WR2.MouseMove, WP1.MouseMove, WP2.MouseMove, WP3.MouseMove, WP4.MouseMove, WP5.MouseMove, WP6.MouseMove, WP7.MouseMove, WP8.MouseMove, BK1.MouseMove, BQ1.MouseMove, BB1.MouseMove, BB2.MouseMove, BN1.MouseMove, BN2.MouseMove, BR1.MouseMove, BR2.MouseMove, BP1.MouseMove, BP2.MouseMove, BP3.MouseMove, BP4.MouseMove, BP5.MouseMove, BP6.MouseMove, BP7.MouseMove, BP8.MouseMove, WQ2.MouseMove, WQ3.MouseMove, WQ4.MouseMove, WQ5.MouseMove, WQ6.MouseMove, WQ7.MouseMove, WQ8.MouseMove, BQ2.MouseMove, BQ3.MouseMove, BQ4.MouseMove, BQ5.MouseMove, BQ6.MouseMove, BQ7.MouseMove, BQ8.MouseMove, WQ9.MouseMove, BQ9.MouseMove
        'The piece is bound to the position of the user's mouse.
        If PieceMoving.IsMovingPiece Then
            PieceMoving.EndPoint = sender.PointToScreen(New Point(e.X, e.Y))
            sender.Left += (PieceMoving.EndPoint.X - PieceMoving.MidPoint.X)
            sender.Top += (PieceMoving.EndPoint.Y - PieceMoving.MidPoint.Y)
            PieceMoving.MidPoint = PieceMoving.EndPoint
        End If
    End Sub

    Private Sub Piece_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles WK1.MouseUp, WQ1.MouseUp, WB1.MouseUp, WB2.MouseUp, WN1.MouseUp, WN2.MouseUp, WR1.MouseUp, WR2.MouseUp, WP1.MouseUp, WP2.MouseUp, WP3.MouseUp, WP4.MouseUp, WP5.MouseUp, WP6.MouseUp, WP7.MouseUp, WP8.MouseUp, BK1.MouseUp, BQ1.MouseUp, BB1.MouseUp, BB2.MouseUp, BN1.MouseUp, BN2.MouseUp, BR1.MouseUp, BR2.MouseUp, BP1.MouseUp, BP2.MouseUp, BP3.MouseUp, BP4.MouseUp, BP5.MouseUp, BP6.MouseUp, BP7.MouseUp, BP8.MouseUp, WQ2.MouseUp, WQ3.MouseUp, WQ4.MouseUp, WQ5.MouseUp, WQ6.MouseUp, WQ7.MouseUp, WQ8.MouseUp, BQ2.MouseUp, BQ3.MouseUp, BQ4.MouseUp, BQ5.MouseUp, BQ6.MouseUp, BQ7.MouseUp, BQ8.MouseUp, WQ9.MouseUp, BQ9.MouseUp
        Dim CheckBoard(7, 7) As Char
        Dim JustCastled As Boolean = False
        Dim IsLegalMove As Boolean
        Dim ReplacedPiece As PictureBox
        Dim TempPoint As Point
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso GameRunning Then
            Me.Cursor = Cursors.Default
            'Disables drag & drop mechanic and sets the piece's position on the board.
            PieceMoving.IsMovingPiece = False
            sender.location = New Point(sender.location.X + 37.5 - (sender.location.X + 37.5) Mod 75, sender.location.y + 37.5 - (sender.location.y + 37.5) Mod 75)
            'If the user has actually moved the piece, and the move is legal, the program continues.
            If sender.location <> PieceMoving.StartPoint Then
                IsLegalMove = False

                TempPoint.X = sender.location.x / 75
                TempPoint.Y = sender.location.y / 75
                If Not OrientForWhite Then
                    TempPoint.X = 7 - TempPoint.X
                    TempPoint.Y = 7 - TempPoint.Y
                    PieceMoving.StartPoint.X = 525 - PieceMoving.StartPoint.X
                    PieceMoving.StartPoint.Y = 525 - PieceMoving.StartPoint.Y
                End If

                For n = 1 To CInt(LegalMoves(0)) - 1
                    If CStr(TempPoint.X & TempPoint.Y) = LegalMoves(n) Then
                        'If the player is in check, the ResolveCheck Function is called to determine whether the player would still be in check after the move is made.
                        If (MasterWInCheck.IsInCheck AndAlso MasterBoard(PieceMoving.StartPoint.X / 75, PieceMoving.StartPoint.Y / 75) <> "K") OrElse (MasterBInCheck.IsInCheck AndAlso MasterBoard(PieceMoving.StartPoint.X / 75, PieceMoving.StartPoint.Y / 75) <> "k") Then
                            If DoesMoveResolveCheck(MasterBoard, PieceMoving.StartPoint.X / 75, PieceMoving.StartPoint.Y / 75, TempPoint.X, TempPoint.Y, MasterWInCheck, MasterBInCheck, MasterEnPassant) Then
                                MasterWInCheck.Piece = " "
                                MasterWInCheck.DoubleCheck = False
                                MasterBInCheck.Piece = " "
                                MasterBInCheck.DoubleCheck = False
                                IsLegalMove = True
                            End If
                        Else
                            MasterWInCheck.IsInCheck = False
                            MasterWInCheck.Piece = " "
                            MasterWInCheck.DoubleCheck = False
                            MasterBInCheck.IsInCheck = False
                            MasterBInCheck.Piece = " "
                            MasterBInCheck.DoubleCheck = False
                            IsLegalMove = True
                        End If
                    End If
                Next
                If IsLegalMove Then
                    If UCase(sender.name(1)) = "K" Then
                        'Disables Castling if the King has moved.
                        If sender.name(0) = "W" Then
                            MasterWKPos = TempPoint.X & TempPoint.Y
                            MasterWCanCastle.KS = False
                            MasterWCanCastle.QS = False
                        Else
                            MasterBKPos = (TempPoint.X) & (TempPoint.Y)
                            MasterBCanCastle.KS = False
                            MasterBCanCastle.QS = False
                        End If
                        'Code for Castling (Queen Side & King Side).
                        If (TempPoint.X & TempPoint.Y) = ((PieceMoving.StartPoint.X / 75) + 2 & PieceMoving.StartPoint.Y / 75) Then
                            JustCastled = True
                            Sound_Castle.Play()
                            MasterBoard(5, TempPoint.Y) = CStr(MasterBoard(7, TempPoint.Y))
                            MasterBoard(7, TempPoint.Y) = " "
                            If sender.name(0) = "W" Then
                                If OrientForWhite Then
                                    WR2.Left -= 150
                                Else
                                    WR2.Left += 150
                                End If
                            Else
                                If OrientForWhite Then
                                    BR2.Left -= 150
                                Else
                                    BR2.Left += 150
                                End If
                            End If
                        ElseIf (TempPoint.X & TempPoint.Y) = ((PieceMoving.StartPoint.X / 75) - 2 & PieceMoving.StartPoint.Y / 75) Then
                            JustCastled = True
                            Sound_Castle.Play()
                            MasterBoard(3, TempPoint.Y) = CStr(MasterBoard(0, TempPoint.Y))
                            MasterBoard(0, TempPoint.Y) = " "
                            If sender.name(0) = "W" Then
                                WR1.Left += 225
                            Else
                                BR1.Left += 225
                            End If
                        End If
                    End If
                    If MasterBoard(TempPoint.X, TempPoint.Y) <> " " Then
                        'If the move is a capture, the required sound is played and the captured piece is removed from the board.
                        Sound_Capture.Play()
                        ReplacedPiece = CoorToPieceConverter(TempPoint.X, TempPoint.Y, CStr(sender.name)(0))
                        ReplacedPiece.Visible = False
                        ReplacedPiece.Location = New Point(-100, -100)
                    ElseIf JustCastled = False Then
                        Sound_Move.Play()
                    End If
                    'MasterBoard is updated by clearing the piece's previous piosition and reallocating its current position.
                    MasterBoard(PieceMoving.StartPoint.X / 75, PieceMoving.StartPoint.Y / 75) = " "

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


                    If CStr(sender.name)(0) = "W" Then
                        MasterBoard(TempPoint.X, TempPoint.Y) = UCase((CStr(sender.name))(1))
                        'If a pawn has made it to the end of the board, it is promoted to a Queen.
                        If sender.name(1) = "P" Then
                            If TempPoint.Y = 0 Then
                                For n = 1 To 9
                                    ReplacedPiece = Me.Controls.Find("WQ" & n.ToString, True).Single()
                                    If ReplacedPiece.Visible = False Then Exit For
                                Next
                                '#isable Warning BC42104 'Variable is used before it has been assigned a value
                                ReplacedPiece.Location = New Point(sender.location.X, sender.location.Y)
                                '#nable Warning BC42104 'Variable is used before it has been assigned a value
                                ReplacedPiece.Visible = True
                                ReplacedPiece.BringToFront()
                                MasterBoard(TempPoint.X, TempPoint.Y) = "Q"
                                sender.Visible = False
                                sender.Location = New Point(-100, -100)
                            ElseIf TempPoint.X & TempPoint.Y = MasterEnPassant Then
                                'Rules for En Passant - removes the piece behind it.
                                Sound_Capture.Play()
                                ReplacedPiece = CoorToPieceConverter(TempPoint.X, TempPoint.Y + 1, CStr(sender.name)(0))
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
                        FixTFTables(MasterBoard, False, MasterBlackTFTable, MasterBKPos, NotInCheck, MasterBInCheck, MasterEnPassant)
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
                                'Rules for En Passant - removes the piece behind it.
                                Sound_Capture.Play()
                                ReplacedPiece = CoorToPieceConverter(TempPoint.X, TempPoint.Y - 1, CStr(sender.name)(0))
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
                        FixTFTables(MasterBoard, True, MasterWhiteTFTable, MasterWKPos, MasterWInCheck, NotInCheck, MasterEnPassant)
                    End If

                    'En Passant Generation.
                    If Not OrientForWhite Then
                        PieceMoving.StartPoint.X = 525 - PieceMoving.StartPoint.X
                        PieceMoving.StartPoint.Y = 525 - PieceMoving.StartPoint.Y
                    End If
                    Console.WriteLine(sender.location.y & PieceMoving.StartPoint.Y)
                    If sender.name(1) = "P" AndAlso Math.Abs(sender.location.Y - PieceMoving.StartPoint.Y) = 150 Then
                        Console.WriteLine("hi6")
                        If sender.name(0) = "W" AndAlso (MasterBoard(Math.Max((TempPoint.X) - 1, 0), TempPoint.Y) = "p" OrElse MasterBoard(Math.Min((TempPoint.X) + 1, 7), TempPoint.Y) = "p") Then
                            MasterEnPassant = TempPoint.X & 5
                        ElseIf sender.name(0) = "B" AndAlso (MasterBoard(Math.Max((TempPoint.X) - 1, 0), TempPoint.Y) = "P" OrElse MasterBoard(Math.Min((TempPoint.X) + 1, 7), TempPoint.Y) = "P") Then
                            MasterEnPassant = TempPoint.X & 2
                        Else
                            MasterEnPassant = "-"
                        End If
                    Else
                        MasterEnPassant = "-"
                    End If


                    If AutoFlipper.Checked Then
                        FlipBoard()
                    Else
                        Checkerboard.Refresh()
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
                    CurrentFEN = ConvertToFEN()
                    If GameMode < 3 Then FENTextBox.Text = CurrentFEN
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

                    If GameMode = 1 Then
                        If PlayerTurn Then
                            WhiteAIMove_Click()
                        Else
                            BlackAIMove_Click()
                        End If
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

    'Function which receives a game state and a possible move. The function makes this move on the board (using
    'shortcuts that can only be made on a virtual board), and then generates the possible moves of the attacking
    'piece(s). If the king is no longer being threatened, then the check as been resolved.
    Function DoesMoveResolveCheck(ByRef Board(,) As Char, ByRef OldPosX As String, ByRef OldPosY As String, ByRef NewPosX As String, ByRef NewPosY As String, ByRef WInCheck As InCheck, ByRef BInCheck As InCheck, ByRef EnPassant As String) As Boolean
        Dim CheckBoard(7, 7) As Char
        DoesMoveResolveCheck = False
        If WInCheck.IsInCheck AndAlso Not (NewPosX & NewPosY = WInCheck.Piece) Then
            If Board(OldPosX, OldPosY) = "K" OrElse (Not WInCheck.DoubleCheck) Then
                WInCheck.IsInCheck = False
                Array.Copy(Board, CheckBoard, 64)
                CheckBoard(NewPosX, NewPosY) = "O"
                If NewPosX & NewPosY = EnPassant AndAlso CheckBoard(OldPosX, OldPosY) = "P" Then CheckBoard(NewPosX, NewPosY + 1) = " " 'Check this.
                Array.Copy(MasterTrueTable, TrueTable, 64)
                BlackPieceLegalMoves(CheckBoard, CStr(WInCheck.Piece(0)), CStr(WInCheck.Piece(1)), TrueTable, TrueTable, "00", WInCheck, BInCheck, MasterBCanCastle, "-")
                If WInCheck.IsInCheck = False Then
                    DoesMoveResolveCheck = True
                End If
            End If
        ElseIf BInCheck.IsInCheck AndAlso Not (NewPosX & NewPosY = BInCheck.Piece) Then
            If Board(OldPosX, OldPosY) = "k" OrElse (Not BInCheck.DoubleCheck) Then
                BInCheck.IsInCheck = False
                Array.Copy(Board, CheckBoard, 64)
                CheckBoard(NewPosX, NewPosY) = "o"
                If NewPosX & NewPosY = EnPassant AndAlso CheckBoard(OldPosX, OldPosY) = "p" Then CheckBoard(NewPosX, NewPosY - 1) = " " 'Check this.
                Array.Copy(MasterTrueTable, TrueTable, 64)
                WhitePieceLegalMoves(CheckBoard, CStr(BInCheck.Piece(0)), CStr(BInCheck.Piece(1)), TrueTable, TrueTable, "00", BInCheck, WInCheck, MasterWCanCastle, "-")
                If BInCheck.IsInCheck = False Then
                    DoesMoveResolveCheck = True
                End If
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
            BestMove1 = WhiteAI()
        Else
            BestMove1 = BlackAI(0)
        End If
        If AbsoluteDepth <> 0 Then Progress += 1
        Console.WriteLine(BestMove1.Score)
    End Sub
    Private Sub InitialiseThread2()
        If PlayerTurn Then
            'BestMove2 = WhiteAI(1)
        Else
            BestMove2 = BlackAI(1)
        End If
        If AbsoluteDepth <> 0 Then Progress += 1
        Console.WriteLine(BestMove2.Score)
    End Sub
    Private Sub InitialiseThread3()
        If PlayerTurn Then
            'BestMove3 = WhiteAI(2)
        Else
            BestMove3 = BlackAI(2)
        End If
        If AbsoluteDepth <> 0 Then Progress += 1
        Console.WriteLine(BestMove3.Score)
    End Sub
    Private Sub InitialiseThread4()
        If PlayerTurn Then
            ''BestMove4 = WhiteAI(3)
        Else
            BestMove4 = BlackAI(3)
        End If
        If AbsoluteDepth <> 0 Then Progress += 1
        Console.WriteLine(BestMove4.Score)
    End Sub
    Private Sub InitialiseThread5()
        If PlayerTurn Then
            'BestMove5 = WhiteAI(4)
        Else
            BestMove5 = BlackAI(4)
        End If
        If AbsoluteDepth <> 0 Then Progress += 1
        Console.WriteLine(BestMove5.Score)
    End Sub
    Private Sub InitialiseThread6()
        If PlayerTurn Then
            'BestMove6 = WhiteAI(5)
        Else
            BestMove6 = BlackAI(5)
        End If
        If AbsoluteDepth <> 0 Then Progress += 1
        Console.WriteLine(BestMove6.Score)
    End Sub
    Private Sub InitialiseThread7()
        If PlayerTurn Then
            'BestMove7 = WhiteAI(6)
        Else
            BestMove7 = BlackAI(6)
        End If
        If AbsoluteDepth <> 0 Then Progress += 1
        Console.WriteLine(BestMove7.Score)
    End Sub
    Private Sub InitialiseThread8()
        If PlayerTurn Then
            'BestMove8 = WhiteAI(7)
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
            GameRunning = False
            Progress = 0
            If AbsoluteDepth <> 0 Then
                Me.Cursor = Cursors.WaitCursor
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
                Thread.Sleep(1)
            End While
            If AbsoluteDepth <> 0 Then
                Stopwatch.Stop()
                Console.WriteLine(vbCrLf & "Search Result: " & Stopwatch.Elapsed.TotalMilliseconds & " Milliseconds. " & PositionsSearched & " Positions Searched.")
            End If
            BestMove = BestMove1
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
            ElseIf Me.IsHandleCreated Then
                GameRunning = True
                If MasterWInCheck.IsInCheck Then
                    MasterWInCheck.IsInCheck = False
                    MasterWInCheck.Piece = " "
                    MasterWInCheck.DoubleCheck = False
                End If
                MakeMove(MasterBoard, BestMove.OldMoveX, BestMove.OldMoveY, BestMove.NewMoveX, BestMove.NewMoveY, MasterWCanCastle, MasterWKPos, MasterMaterialCount, MasterEnPassant, True)

                DisplayPieces()

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
                If AutoFlipper.Checked Then
                    FlipBoard()
                Else
                    Checkerboard.Refresh()
                End If

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
                If GameMode = 3 Then
                    PreviousFEN = CurrentFEN
                Else
                    FENTextBox.Text = ConvertToFEN()
                End If
                CurrentFEN = ConvertToFEN()
                CurrentEval.Text = BestMove.Score
                ProgressBar.Value = 0
                ComputerIsSearching = False
                Me.Cursor = Cursors.Default

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


    Private Function WhiteAI() As NewMove
        Dim CurrentMove As NewMove
        Dim BestMove As NewMove
        BestMove.Score = Integer.MinValue
        Dim alpha As Integer = Integer.MinValue
        Dim beta As Integer = Integer.MaxValue


        Dim PieceMoves = CreateMoves(MasterBoard, True, MasterWhiteTFTable, MasterBKPos, MasterWInCheck, MasterBInCheck, MasterWCanCastle, MasterEnPassant, False, -1)
        If PieceMoves(0, 0) > 0 Then
            Dim TempBoard(7, 7) As Char
            Dim TempMaterialCount As Int16
            Dim TempWKPos As String
            Dim TempWCanCastle As CanCastle
            Dim TempWInCheck As InCheck
            Dim TempEnPassant As String
            For n = 1 To CInt(PieceMoves(0, 0))
                If MasterWInCheck.IsInCheck AndAlso MasterBoard(PieceMoves(n, 0), PieceMoves(n, 1)) <> "K" Then
                    TempWInCheck.IsInCheck = True
                    TempWInCheck.Piece = MasterWInCheck.Piece
                    TempWInCheck.DoubleCheck = MasterWInCheck.DoubleCheck
                    '#Disable Warning BC42104 'Variable is used before it has been assigned a value
                    If DoesMoveResolveCheck(MasterBoard, PieceMoves(n, 0), PieceMoves(n, 1), PieceMoves(n, 2)(0), PieceMoves(n, 2)(1), TempWInCheck, MasterBInCheck, MasterEnPassant) Then
                        TempWInCheck.Piece = " "
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
                    MakeMove(TempBoard, PieceMoves(n, 0), PieceMoves(n, 1), PieceMoves(n, 2)(0), PieceMoves(n, 2)(1), TempWCanCastle, TempWKPos, TempMaterialCount, TempEnPassant, False)
                    CurrentMove.Score = MiniMax(TempBoard, AbsoluteDepth - 1, False, TempWCanCastle, MasterBCanCastle, TempWKPos, MasterBKPos, TempEnPassant, TempMaterialCount, alpha, beta, True)
                    'Console.WriteLine(x & y & PieceMoves(n)(0) & PieceMoves(n)(1) & CurrentMove.Score)
                    alpha = Math.Max(alpha, CurrentMove.Score)
                    If CurrentMove.Score > BestMove.Score Then
                        BestMove.Score = CurrentMove.Score
                        BestMove.OldMoveX = PieceMoves(n, 0)
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
        GlobalAlpha = Integer.MinValue
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
                Me.Cursor = Cursors.WaitCursor
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

            Dim BestMove As NewMove = GetBestMove(True)
            If AbsoluteDepth = 0 Then
                If BestMove.Score = Integer.MinValue Then
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
            ElseIf Me.IsHandleCreated Then
                GameRunning = True
                If MasterBInCheck.IsInCheck Then
                    MasterBInCheck.IsInCheck = False
                    MasterBInCheck.Piece = " "
                    MasterBInCheck.DoubleCheck = False
                End If
                MakeMove(MasterBoard, BestMove.OldMoveX, BestMove.OldMoveY, BestMove.NewMoveX, BestMove.NewMoveY, MasterBCanCastle, MasterBKPos, MasterMaterialCount, MasterEnPassant, True)

                DisplayPieces()
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
                If AutoFlipper.Checked Then
                    FlipBoard()
                Else
                    Checkerboard.Refresh()
                End If

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
                If GameMode = 3 Then
                    PreviousFEN = CurrentFEN
                Else
                    FENTextBox.Text = ConvertToFEN()
                End If
                CurrentFEN = ConvertToFEN()
                CurrentEval.Text = -BestMove.Score
                ProgressBar.Value = 0
                ComputerIsSearching = False
                Me.Cursor = Cursors.Default

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

    Dim GlobalAlpha As Decimal

    Private Function BlackAI(ByRef x As Byte) As NewMove
        Dim CurrentMove As NewMove
        Dim BestMove As NewMove
        Dim beta As Integer = Integer.MaxValue
        BestMove.Score = Integer.MinValue

        Dim PieceMoves = CreateMoves(MasterBoard, False, MasterBlackTFTable, MasterWKPos, MasterWInCheck, MasterBInCheck, MasterBCanCastle, MasterEnPassant, False, x)
        If PieceMoves(0, 0) > 0 Then
            Dim TempBoard(7, 7) As Char
            Dim TempMaterialCount As Int16
            Dim TempBKPos As String
            Dim TempBCanCastle As CanCastle
            Dim TempBInCheck As InCheck
            Dim TempEnPassant As String
            For n = 1 To CInt(PieceMoves(0, 0))
                If MasterBInCheck.IsInCheck AndAlso MasterBoard(x, PieceMoves(n, 1)) <> "k" Then
                    TempBInCheck.IsInCheck = True
                    TempBInCheck.Piece = MasterBInCheck.Piece
                    TempBInCheck.DoubleCheck = MasterBInCheck.DoubleCheck
                    '#Disable Warning BC42104 'Variable is used before it has been assigned a value
                    If DoesMoveResolveCheck(MasterBoard, x, PieceMoves(n, 1), PieceMoves(n, 2)(0), PieceMoves(n, 2)(1), MasterWInCheck, TempBInCheck, MasterEnPassant) Then
                        TempBInCheck.Piece = " "
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
                    MakeMove(TempBoard, x, PieceMoves(n, 1), PieceMoves(n, 2)(0), PieceMoves(n, 2)(1), TempBCanCastle, TempBKPos, TempMaterialCount, TempEnPassant, False)
                    CurrentMove.Score = -MiniMax(TempBoard, AbsoluteDepth - 1, True, MasterWCanCastle, TempBCanCastle, MasterWKPos, TempBKPos, TempEnPassant, TempMaterialCount, -beta, -GlobalAlpha, False)
                    GlobalAlpha = Math.Max(GlobalAlpha, CurrentMove.Score)
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
                        'Array.Copy(MasterTrueTable, TrueTable, 64)
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
            For y = 7 To 0 Step -1
                For x = xValue To 7 Step StepCount
                    If Char.IsLower(Board(x, y)) Then
                        'Array.Copy(MasterTrueTable, TrueTable, 64)
                        Dim PieceMoves = BlackPieceLegalMoves(Board, x, y, TrueFalseTable, TrueTable, KPos, WInCheck, BInCheck, CanCastle, EnPassant)
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
                                    If PieceMoves(n)(1) = "7" AndAlso Board(x, y) = "p" Then
                                        PawnPromotionMoves(PPMLen, 0) = x
                                        PawnPromotionMoves(PPMLen, 1) = y
                                        PawnPromotionMoves(PPMLen, 2) = PieceMoves(n)
                                        PPMLen += 1
                                    ElseIf Board(Math.Max(CStr(PieceMoves(n)(0)) - 1, 0), Math.Min(CStr(PieceMoves(n)(1)) + 1, 7)) = "P" OrElse Board(Math.Min(CStr(PieceMoves(n)(0)) + 1, 7), Math.Min(CStr(PieceMoves(n)(1)) + 1, 7)) = "P" Then
                                        TerribleMoves(TMLen, 0) = x
                                        TerribleMoves(TMLen, 1) = y
                                        TerribleMoves(TMLen, 2) = PieceMoves(n)
                                        TMLen += 1
                                    ElseIf TrueFalseTable(CStr(PieceMoves(n)(0)), CStr(PieceMoves(n)(1))) = "F" Then
                                        BadMoves(BMLen, 0) = x
                                        BadMoves(BMLen, 1) = y
                                        BadMoves(BMLen, 2) = PieceMoves(n)
                                        BMLen += 1
                                    ElseIf (Board(x, y) <> "p" AndAlso Board(x, y) <> "K") AndAlso (dx = 0 OrElse dy = 0 OrElse dx = dy) Then
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
        Dim HasEnPassanted As Boolean
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
                ElseIf OldCoorY = 6 AndAlso NewCoorY = 4 AndAlso (Board(Math.Max(NewCoorX - 1, 0), 4) = "p" OrElse Board(Math.Min(NewCoorX + 1, 7), 4) = "p") Then
                    EnPassant = NewCoorX & 5
                    HasEnPassanted = True
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
                ElseIf OldCoorY = 1 AndAlso NewCoorY = 3 AndAlso (Board(Math.Max(NewCoorX - 1, 0), 3) = "P" OrElse Board(Math.Min(NewCoorX + 1, 7), 3) = "P") Then
                    EnPassant = NewCoorX & 2
                    HasEnPassanted = True
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
        If EnPassant <> "-" AndAlso Not HasEnPassanted Then
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
        Console.WriteLine("Absolute Depth = " & AbsoluteDepth)
    End Sub



    'Quiescence MiniMax
    Public Function MiniMax(ByVal Board(,) As Char, ByVal depth As SByte, ByVal isWhite As Boolean, ByVal WCanCastle As CanCastle, ByVal BCanCastle As CanCastle, ByVal WKPos As String, ByVal BKPos As String, ByVal EnPassant As String, ByVal MaterialCount As Int16, ByVal Alpha As Decimal, ByVal Beta As Decimal, ByVal Quiescence As Boolean) As Integer
        PositionsSearched += 1
        Dim HasLegalMove As Boolean = False
        Dim PieceMoves
        Dim WInCheck As InCheck
        WInCheck.IsInCheck = False 'WHERE DO I PUT THESE????
        WInCheck.Piece = " "
        WInCheck.DoubleCheck = False
        Dim BInCheck As InCheck
        BInCheck.IsInCheck = False
        BInCheck.Piece = " "
        BInCheck.DoubleCheck = False
        If isWhite Then
            Dim WhiteTFTable(7, 7) As Char
            'TempWInCheck.IsInCheck = False
            FixTFTables(Board, True, WhiteTFTable, WKPos, WInCheck, NotInCheck, EnPassant)

            If depth <= 0 AndAlso Not WInCheck.IsInCheck Then
                MiniMax = Evaluate(Board, MaterialCount)
                If Beta <= MiniMax Then
                    '#Disable Warning BC42109 ' Variable is used before it has been assigned a value
                    Return MiniMax
                    '#Enable Warning BC42109 ' Variable is used before it has been assigned a value
                End If
            End If

            If Quiescence AndAlso depth <= 0 AndAlso Not WInCheck.IsInCheck Then
                PieceMoves = CreateMoves(Board, True, WhiteTFTable, BKPos, WInCheck, BInCheck, WCanCastle, EnPassant, True, -1)
            Else
                PieceMoves = CreateMoves(Board, True, WhiteTFTable, BKPos, WInCheck, BInCheck, WCanCastle, EnPassant, False, -1)
            End If
            If PieceMoves(0, 0) > 0 Then
                Dim TempBoard(7, 7) As Char
                Dim TempMaterialCount As Int16
                Dim TempWKPos As String
                Dim TempWInCheck As InCheck
                Dim TempWCanCastle As CanCastle
                Dim TempEnPassant As String
                For n = 1 To CInt(PieceMoves(0, 0))
                    If WInCheck.IsInCheck AndAlso Board(PieceMoves(n, 0), PieceMoves(n, 1)) <> "K" Then
                        TempWInCheck.IsInCheck = True
                        TempWInCheck.Piece = WInCheck.Piece
                        TempWInCheck.DoubleCheck = WInCheck.DoubleCheck
                        If DoesMoveResolveCheck(Board, PieceMoves(n, 0), PieceMoves(n, 1), PieceMoves(n, 2)(0), PieceMoves(n, 2)(1), TempWInCheck, BInCheck, EnPassant) Then
                            TempWInCheck.Piece = " "
                            TempWInCheck.DoubleCheck = False
                        End If
                    Else
                        TempWInCheck.IsInCheck = False
                    End If
                    If Not TempWInCheck.IsInCheck Then
                        HasLegalMove = True
                        Array.Copy(Board, TempBoard, 64)
                        TempMaterialCount = MaterialCount 'do i need these either?
                        TempWKPos = WKPos
                        TempWCanCastle.KS = WCanCastle.KS
                        TempWCanCastle.QS = WCanCastle.QS
                        TempEnPassant = EnPassant 'do i need this?
                        MakeMove(TempBoard, PieceMoves(n, 0), PieceMoves(n, 1), PieceMoves(n, 2)(0), PieceMoves(n, 2)(1), TempWCanCastle, TempWKPos, TempMaterialCount, TempEnPassant, False)
                        If Not Quiescence AndAlso depth <= 1 Then
                            MiniMax = Evaluate(TempBoard, TempMaterialCount)
                            PositionsSearched += 1
                        Else
                            MiniMax = MiniMax(TempBoard, depth - 1, False, TempWCanCastle, BCanCastle, TempWKPos, BKPos, TempEnPassant, TempMaterialCount, Alpha, Beta, Quiescence)
                        End If
                        Alpha = Math.Max(Alpha, MiniMax)
                        If Beta <= Alpha Then
#Disable Warning BC42109 ' Variable is used before it has been assigned a value
                            Return Alpha
#Enable Warning BC42109 ' Variable is used before it has been assigned a value
                        End If
                    End If
                Next
            End If
        Else
            Dim BlackTFTable(7, 7) As Char
            'TempBInCheck.IsInCheck = False
            FixTFTables(Board, False, BlackTFTable, BKPos, NotInCheck, BInCheck, EnPassant)

            If depth <= 0 AndAlso Not BInCheck.IsInCheck Then
                MiniMax = Evaluate(Board, MaterialCount)
                If MiniMax <= Alpha Then
                    '#Disable Warning BC42109 ' Variable is used before it has been assigned a value
                    Return MiniMax
                    '#Enable Warning BC42109 ' Variable is used before it has been assigned a value
                End If
            End If

            If Quiescence AndAlso depth <= 0 AndAlso Not BInCheck.IsInCheck Then
                PieceMoves = CreateMoves(Board, False, BlackTFTable, WKPos, WInCheck, BInCheck, BCanCastle, EnPassant, True, -1)
            Else
                PieceMoves = CreateMoves(Board, False, BlackTFTable, WKPos, WInCheck, BInCheck, BCanCastle, EnPassant, False, -1)
            End If
            If PieceMoves(0, 0) > 0 Then
                Dim TempBoard(7, 7) As Char
                Dim TempMaterialCount As Int16
                Dim TempBKPos As String
                Dim TempBInCheck As InCheck
                Dim TempBCanCastle As CanCastle
                Dim TempEnPassant As String
                For n = 1 To CInt(PieceMoves(0, 0))
                    If BInCheck.IsInCheck AndAlso Board(PieceMoves(n, 0), PieceMoves(n, 1)) <> "k" Then
                        TempBInCheck.IsInCheck = True
                        TempBInCheck.Piece = BInCheck.Piece
                        TempBInCheck.DoubleCheck = BInCheck.DoubleCheck
                        If DoesMoveResolveCheck(Board, PieceMoves(n, 0), PieceMoves(n, 1), PieceMoves(n, 2)(0), PieceMoves(n, 2)(1), WInCheck, TempBInCheck, EnPassant) Then
                            TempBInCheck.Piece = " "
                            TempBInCheck.DoubleCheck = False
                        End If
                    Else
                        TempBInCheck.IsInCheck = False
                    End If
                    If Not TempBInCheck.IsInCheck Then
                        HasLegalMove = True
                        Array.Copy(Board, TempBoard, 64)
                        TempMaterialCount = MaterialCount
                        TempBKPos = BKPos
                        TempBCanCastle.KS = BCanCastle.KS
                        TempBCanCastle.QS = BCanCastle.QS
                        TempEnPassant = EnPassant
                        MakeMove(TempBoard, PieceMoves(n, 0), PieceMoves(n, 1), PieceMoves(n, 2)(0), PieceMoves(n, 2)(1), TempBCanCastle, TempBKPos, TempMaterialCount, TempEnPassant, False)
                        If Not Quiescence AndAlso depth <= 1 Then
                            MiniMax = Evaluate(TempBoard, TempMaterialCount)
                            PositionsSearched += 1
                        Else
                            MiniMax = MiniMax(TempBoard, depth - 1, True, WCanCastle, TempBCanCastle, WKPos, TempBKPos, TempEnPassant, TempMaterialCount, Alpha, Beta, Quiescence)
                        End If
                        Beta = Math.Min(Beta, MiniMax)
                        If Beta <= Alpha Then
                            Return Beta
                        End If
                    End If
                Next
            End If
        End If

        If Not HasLegalMove Then
            If depth <= 0 Then
                Return Evaluate(Board, MaterialCount)
            Else
                If WInCheck.IsInCheck Then
                    Return -(1000 + depth)
                ElseIf BInCheck.IsInCheck Then
                    Return (1000 + depth)
                Else
                    Return 0
                End If
            End If
        Else
            If isWhite Then
                Return Alpha
            Else
                Return Beta
            End If
        End If

    End Function


    ''Non-Quiescence MiniMax.
    'Public Function MiniMax(ByVal Board(,) As Char, ByVal depth As SByte, ByVal isWhite As Boolean, ByVal WCanCastle As CanCastle, ByVal BCanCastle As CanCastle, ByVal WKPos As String, ByVal BKPos As String, ByVal EnPassant As String, ByVal MaterialCount As Int16, ByVal Alpha As Decimal, ByVal Beta As Decimal, ByVal Quiescence As Boolean) As Integer
    '    PositionsSearched += 1
    '    Dim CurrentMove As NewMove
    '    Dim BestMove As NewMove
    '    Dim WInCheck As InCheck
    '    WInCheck.IsInCheck = False 'WHERE DO I PUT THESE????
    '    WInCheck.Piece = " "
    '    WInCheck.DoubleCheck = False
    '    Dim BInCheck As InCheck
    '    BInCheck.IsInCheck = False
    '    BInCheck.Piece = " "
    '    BInCheck.DoubleCheck = False
    '    If isWhite Then
    '        BestMove.Score = Integer.MinValue
    '        Dim WhiteTFTable(7, 7) As Char
    '        'TempWInCheck.IsInCheck = False
    '        FixTFTables(Board, True, WhiteTFTable, WKPos, WInCheck, NotInCheck, EnPassant)
    '        Dim PieceMoves = CreateMoves(Board, True, WhiteTFTable, BKPos, WInCheck, BInCheck, WCanCastle, EnPassant, False, -1)
    '        If PieceMoves(0, 0) > 0 Then
    '            Dim TempBoard(7, 7) As Char
    '            Dim TempMaterialCount As Int16
    '            Dim TempWKPos As String
    '            Dim TempWCanCastle As CanCastle
    '            Dim TempWInCheck As InCheck
    '            Dim TempEnPassant As String
    '            For n = 1 To CInt(PieceMoves(0, 0))
    '                If WInCheck.IsInCheck AndAlso Board(PieceMoves(n, 0), PieceMoves(n, 1)) <> "K" Then
    '                    TempWInCheck.IsInCheck = True
    '                    TempWInCheck.Piece = WInCheck.Piece
    '                    TempWInCheck.DoubleCheck = WInCheck.DoubleCheck
    '                    If DoesMoveResolveCheck(Board, PieceMoves(n, 0), PieceMoves(n, 1), PieceMoves(n, 2)(0), PieceMoves(n, 2)(1), TempWInCheck, BInCheck, EnPassant) Then
    '                        TempWInCheck.Piece = " "
    '                        TempWInCheck.DoubleCheck = False
    '                    End If
    '                Else
    '                    TempWInCheck.IsInCheck = False
    '                End If
    '                If Not TempWInCheck.IsInCheck Then
    '                    Array.Copy(Board, TempBoard, 64)
    '                    TempMaterialCount = MaterialCount 'do i need these either?
    '                    TempWKPos = WKPos
    '                    TempWCanCastle.KS = WCanCastle.KS
    '                    TempWCanCastle.QS = WCanCastle.QS
    '                    TempEnPassant = EnPassant 'do i need this?
    '                    MakeMove(TempBoard, PieceMoves(n, 0), PieceMoves(n, 1), PieceMoves(n, 2)(0), PieceMoves(n, 2)(1), TempWCanCastle, TempWKPos, TempMaterialCount, TempEnPassant, False)
    '                    If depth <= 1 Then
    '                        CurrentMove.Score = Evaluate(TempBoard, TempMaterialCount)
    '                    Else
    '                        CurrentMove.Score = MiniMax(TempBoard, depth - 1, False, TempWCanCastle, BCanCastle, TempWKPos, BKPos, TempEnPassant, TempMaterialCount, Alpha, Beta, False)
    '                    End If
    '                    If CurrentMove.Score > BestMove.Score Then
    '                        BestMove.Score = CurrentMove.Score
    '                    End If
    '                    Alpha = Math.Max(Alpha, CurrentMove.Score)
    '                    If Beta <= Alpha Then
    '                        '#Disable Warning BC42109 ' Variable is used before it has been assigned a value
    '                        Return BestMove.Score
    '                        '#Enable Warning BC42109 ' Variable is used before it has been assigned a value
    '                    End If
    '                End If
    '            Next
    '        End If
    '    Else
    '        BestMove.Score = Integer.MaxValue
    '        Dim BlackTFTable(7, 7) As Char
    '        'TempBInCheck.IsInCheck = False
    '        FixTFTables(Board, False, BlackTFTable, BKPos, NotInCheck, BInCheck, EnPassant)
    '        Dim PieceMoves = CreateMoves(Board, False, BlackTFTable, WKPos, WInCheck, BInCheck, BCanCastle, EnPassant, False, -1)
    '        If PieceMoves(0, 0) > 0 Then
    '            Dim TempBoard(7, 7) As Char
    '            Dim TempMaterialCount As Int16
    '            Dim TempBKPos As String
    '            Dim TempBCanCastle As CanCastle
    '            Dim TempBInCheck As InCheck
    '            Dim TempEnPassant As String
    '            For n = 1 To CInt(PieceMoves(0, 0))
    '                If BInCheck.IsInCheck AndAlso Board(PieceMoves(n, 0), PieceMoves(n, 1)) <> "k" Then
    '                    TempBInCheck.IsInCheck = True
    '                    TempBInCheck.Piece = BInCheck.Piece
    '                    TempBInCheck.DoubleCheck = BInCheck.DoubleCheck
    '                    If DoesMoveResolveCheck(Board, PieceMoves(n, 0), PieceMoves(n, 1), PieceMoves(n, 2)(0), PieceMoves(n, 2)(1), WInCheck, TempBInCheck, EnPassant) Then
    '                        TempBInCheck.Piece = " "
    '                        TempBInCheck.DoubleCheck = False
    '                    End If
    '                Else
    '                    TempBInCheck.IsInCheck = False
    '                End If
    '                If Not TempBInCheck.IsInCheck Then
    '                    Array.Copy(Board, TempBoard, 64)
    '                    TempMaterialCount = MaterialCount
    '                    TempBKPos = BKPos
    '                    TempBCanCastle.KS = BCanCastle.KS
    '                    TempBCanCastle.QS = BCanCastle.QS
    '                    TempEnPassant = EnPassant
    '                    MakeMove(TempBoard, PieceMoves(n, 0), PieceMoves(n, 1), PieceMoves(n, 2)(0), PieceMoves(n, 2)(1), TempBCanCastle, TempBKPos, TempMaterialCount, TempEnPassant, False)
    '                    If depth <= 1 Then
    '                        CurrentMove.Score = Evaluate(TempBoard, TempMaterialCount)
    '                    Else
    '                        CurrentMove.Score = MiniMax(TempBoard, depth - 1, True, WCanCastle, TempBCanCastle, WKPos, TempBKPos, TempEnPassant, TempMaterialCount, Alpha, Beta, False)
    '                    End If
    '                    If CurrentMove.Score < BestMove.Score Then
    '                        BestMove.Score = CurrentMove.Score
    '                    End If
    '                    Beta = Math.Min(Beta, CurrentMove.Score)
    '                    If Beta <= Alpha Then
    '                        Return BestMove.Score
    '                    End If
    '                End If
    '            Next
    '        End If
    '    End If

    '    If BestMove.Score = Integer.MinValue Then
    '        If WInCheck.IsInCheck Then
    '            BestMove.Score = -(1000 + depth)
    '        Else
    '            BestMove.Score = 0
    '        End If
    '    ElseIf BestMove.Score = Integer.MaxValue Then
    '        If BInCheck.IsInCheck Then
    '            BestMove.Score = (1000 + depth)
    '        Else
    '            BestMove.Score = 0
    '        End If
    '    End If
    '    Return BestMove.Score
    '    '#Enable Warning BC42108 ' Variable is passed by reference before it has been assigned a value
    'End Function


End Class


Public Class AI
    Inherits CoreMethods
End Class

Public Class CoreMethods

End Class

Public Class InCheck
    Private IsInCheck As Boolean
    Private Piece As String
    Private DoubleCheck As Boolean
    Public Sub New()
        Piece = ""
    End Sub
    Public Sub NotInCheck()
        IsInCheck = False
        Piece = ""
        DoubleCheck = False
    End Sub
End Class

Public Class CanCastle
    Private KS As Boolean
    Private QS As Boolean
    Public Sub CannotCastle()
        KS = False
        QS = False
    End Sub
End Class

Public Structure Move
    Private Score As Decimal
    Private OldMoveX As Char
    Private OldMoveY As Char
    Private NewMoveX As Char
    Private NewMoveY As Char
End Structure



'Tasks left to do:
'Threefold repetition
'50 move stalemate
'Transposition table
'Remembering analysis And continuing
'skip analysis if only 1 move possible.


'Evaluation: (use stock fish Eval?)

'Middlegame:
'King safety
'Space / activity of pieces
'Controlling opponents squares
'Doubled pawns
'Isolated pawns / pawn islands?
'Restrict opponent King movement? 

'Endgame:
'King activity
'Restricting oppenent King movement
'Doubled pawns(Higher multiplier)
'How advanced pawns are
'Connected past pawns
