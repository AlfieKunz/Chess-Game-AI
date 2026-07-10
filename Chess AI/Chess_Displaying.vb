'Class containing all the core GUI elements, such as displaying the checkerboard & its intricacies, displaying the pieces on the board, animating moves, etc.
Imports System.Data.SqlClient
Imports System.Diagnostics.Eventing.Reader
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Runtime.Remoting.Messaging
Imports System.Security.Cryptography
Imports System.Threading
Imports System.Timers

Partial Public Class Chess 'Displaying

    Private PrimaryColour As Color 'Colour representing the dark squares on the Chessboard.
    Private SecondaryColour As Color 'Colour representing the light squares on the Chessboard.
    Private SquareHistory(3, 1) As SByte 'Array containing the previously used squares on the board (for prev moves).
    Private LegalMoveSquares(7, 7) As Boolean 'Array containing the coordinates of where a piece can move on the board,
    '... generated when the user clicks on a piece. Used when redrawing the checkerboard pattern.
    Private ReadOnly PieceArray As New List(Of PictureBox) 'Contains all the PictureBoxes on the board.

    Private AnimationSpeed As Byte = GlobalConstants.DefaultAnimationSpeed
    Private PieceIsMoving As Boolean 'Represents if the AnimatePiece() method is running.





    'Subroutine that creates checkerboard pattern. (Alternates between light and dark colours,
    'and includes controls for Previously Used Squares, Legal Move Squares, and more.)
    Private Sub CreateBoard(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Checkerboard.Paint
        Dim g As Graphics = e.Graphics
        Dim isLight As Boolean = True
        Dim Colour As Color
        For x = 0 To 7
            For y = 0 To 7
                Dim Square As New Rectangle(75 * x, 75 * y, 75, 75)
                'If the coordinates are set to True in the LegalMoveSquare, then that square should be coloured differently, so
                'that the user can see legal moves. Square given either a green or blue outline (depending on the colour scheme).
                If LegalMoveSquares(x, y) AndAlso GeneralOptions(4) = "T" AndAlso Not BoardEdit.isEnabled Then
                    Colour = If(PrimaryColour = Color.DarkSeaGreen, Color.DarkTurquoise, Color.LimeGreen) 'Is green colour scheme - colour blue.
                    Using Brush As New SolidBrush(Colour)
                        g.FillRectangle(Brush, Square)
                    End Using
                End If

                Colour = If(isLight, SecondaryColour, PrimaryColour)
                If LegalMoveSquares(x, y) AndAlso (GeneralOptions(4) = "T" OrElse BoardEdit.isEnabled) Then
                    'Normal colour is filled at the centre of the legal move square to produce a green / blue highlight.
                    Dim Square2 As New Rectangle((75 * x) + 5, (75 * y) + 5, 65, 65)
                    Using Brush As New SolidBrush(Colour)
                        g.FillRectangle(Brush, Square2)
                    End Using
                    'If the given square matches a SquareHistory coordinate, then it is coloured in a different way.
                ElseIf ((x = SquareHistory(0, 0) AndAlso y = SquareHistory(0, 1)) OrElse (x = SquareHistory(1, 0) AndAlso y = SquareHistory(1, 1))) AndAlso GeneralOptions(3) = "T" AndAlso Not BoardEdit.isEnabled Then
                    Using Brush As New SolidBrush(If(isLight, Color.YellowGreen, Color.OliveDrab)) 'SquareHistory colours.
                        g.FillRectangle(Brush, Square)
                    End Using
                Else 'Is a normal square - colour with user's primary / seconday colour.
                    Using Brush As New SolidBrush(Colour)
                        g.FillRectangle(Brush, Square)
                    End Using
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
        ElseIf (PieceMoving.IsMovingPiece AndAlso GeneralOptions(4) = "T") OrElse ClickMoveMode Then 'the piece is being moved by the user.
            Dim Square As New Rectangle(PieceMoving.StartPoint.X, PieceMoving.StartPoint.Y, 75, 75)
            Using Brush As New SolidBrush(Color.LightCoral)
                g.FillRectangle(Brush, Square)
            End Using
        End If

        'If the user is moving a piece, we draw all other pieces onto the Checkerboard. At the cost of a bit of
        'flickering, this fixes an issue where overlapping PictureBoxes will cut off parts of the other Box's image,
        'due to how transparency works in VB.net. Now, transparency is maintained. (finally my friends will respect me) :(
        If PieceMoving.IsMovingPiece AndAlso GeneralOptions(6) = "F" Then
            For Each Piece In PieceArray
                'If the PictureBox is within the confines of the Checkerboard, and it isn't the piece being moved by the user...
                If Piece.Location.X >= 0 AndAlso Piece.Location.Y >= 0 AndAlso Piece.Name <> PieceMoving.Piece.Name Then
                    'Draws the PictureBox's image into the Checkerboard.
                    g.DrawImage(Piece.Image, Piece.Location.X, Piece.Location.Y)
                    If Piece.Visible Then Piece.Visible = False 'Alternate style of flickering - may be less distracting??
                End If
            Next
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



    'Outputs the board position, along with the TFTables & King positions (to the console).
    'Used for debugging purposes.
    Private Sub OutputDebugInfo(Optional ByVal OnlyFEN As Boolean = False)
        Console.ForegroundColor = ConsoleColor.DarkYellow
        'Outputs the current FEN.
        Console.WriteLine(vbCrLf & vbCrLf & "FEN: " & CurrentFEN)
        Console.ForegroundColor = ConsoleColor.White

        If Not OnlyFEN Then
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
                Console.ForegroundColor = ConsoleColor.White

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
                Console.ForegroundColor = ConsoleColor.White

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
                    If y = 2 Then Console.WriteLine("WKPos: " & Helper.CoorToPGNConverter(MasterWKPos))
                    If y = 4 Then Console.WriteLine("BKPos: " & Helper.CoorToPGNConverter(MasterBKPos))
                    If y = 6 Then Console.WriteLine("En Passant: " & Helper.CoorToPGNConverter(MasterEnPassant))
                Else
                    Console.WriteLine()
                End If
                If Not OrientForWhite Then y = 7 - y
            Next

            'Outputs the Hash Value.
            Console.ForegroundColor = ConsoleColor.Magenta
            Console.WriteLine("Zobrist Hash Value: " & MasterZobristValue.ToString("X16")) 'Outputs Zobrist Key in Hexadecimal.
            Console.ForegroundColor = ConsoleColor.White
        End If
    End Sub




    'Algorithm that displays all the pieces on the board, given a board array.
    Private Sub DisplayPieces()
        Dim NewPiece As PictureBox
        'Hides all pieces.
        For Each Box In PieceArray
            If Box.Location.X <= 600 Then 'Prevents BoardEdit pieces from being re-drawn.
                Box.Location = New Point(-100, -100)
                Box.Visible = False
            End If
        Next
        For y = 7 To 0 Step -1
            For x = 0 To 7
                'For each piece on the board...
                If MasterBoard(x, y) <> " " Then
                    'In MasterBoard(), a lowercase letter represents a black piece, and an uppercase letter represents a white piece.
                    NewPiece = FindNewPiece(If(Char.IsUpper(MasterBoard(x, y)), "W", "B"), UCase(MasterBoard(x, y)))
                    If OrientForWhite Then
                        NewPiece.Location = New Point(((x) * 75), ((75 * y)))
                    Else 'Location flipped 180 degrees.
                        NewPiece.Location = New Point(((7 - x) * 75), ((75 * (7 - y))))
                    End If

                    NewPiece.Visible = True
                End If
            Next
        Next
    End Sub


    'Alternate version of DisplayPieces which is more efficient and only converts a single piece in
    'MasterBoard to its PictureBox counterpart on the board. 'Constraint' here specifies the exact name of the output PictureBox, so that
    'the correct piece is returned, or nothing (for when pieces overlap).
    Private Function FindPieceFromCoor(ByVal CoorX As Int16, ByVal CoorY As Int16, Optional ByVal Constraint As Char = "?") As PictureBox
        If Constraint = " " Then Return Nothing 'No Piece can ever be found on an empty square.
        For Each Piece In PieceArray
            If Piece.Visible Then 'For speed - ignores all pieces off-screen (ie: invisible).
                If Constraint = "?" OrElse Piece.Name.Substring(0, 2) = (If(Char.IsUpper(Constraint), "W", "B") & UCase(Constraint)) Then
                    'Name match - check location.
                    If OrientForWhite AndAlso (Piece.Location.X = CoorX * 75 AndAlso Piece.Location.Y = CoorY * 75) Then
                        Return Piece
                    ElseIf Not OrientForWhite AndAlso (Piece.Location.X = (7 - CoorX) * 75 AndAlso Piece.Location.Y = (7 - CoorY) * 75) Then
                        Return Piece
                    End If
                End If
            End If
        Next
        Return Nothing 'No matching piece could be found.
    End Function

    'Function which finds, or creates, a new piece type that can be placed on the board. If one cannot be found, the system creates a new piece.
    Private Function FindNewPiece(ByVal PieceColourCode As Char, ByVal PieceType As Char) As PictureBox
        Dim Piece As PictureBox = PieceArray(0)
        Dim TemplatePiece As PictureBox = PieceArray(0)
        Dim CurrentPiece As String
        Dim Counter As SByte = 1

        'Loops through all the possible piece numbers.
        While Counter < 100
            CurrentPiece = PieceColourCode & PieceType & Counter

            'For each Piece in the system, its name is checked. If the piece is unused (ie: it is invisible), then it is returned by the function.
            For n = 0 To PieceArray.Count - 1
                If CurrentPiece = PieceArray(n).Name Then
                    Piece = PieceArray(n)
                    If Counter = 1 Then TemplatePiece = PieceArray(n) 'Creates template piece.
                    Exit For
                End If

                If n = PieceArray.Count - 1 Then
                    'Piece not found. Creates a new piece, based on the template of piece #1.
                    Dim NewPiece As New PictureBox With {
                        .Name = CurrentPiece,
                        .Location = New Point(-100, -100),
                        .Size = TemplatePiece.Size,
                        .Image = TemplatePiece.Image,
                        .BackColor = Color.Transparent,
                        .Visible = False
                    }

                    'Adds controls & handles to the new piece.
                    PieceArray.Add(NewPiece)
                    Checkerboard.Controls.Add(NewPiece)
                    AddHandler NewPiece.MouseDown, AddressOf Piece_MouseDown
                    AddHandler NewPiece.MouseMove, AddressOf Piece_MouseMove
                    AddHandler NewPiece.MouseUp, AddressOf Piece_MouseUp
                    Return NewPiece
                End If
            Next

            If Piece.Visible = False AndAlso (Piece.Location.X = -100 AndAlso Piece.Location.Y = -100) Then Return Piece
            Counter += 1
        End While

        Return Piece
    End Function



    'Subroutine which updates the Sprites / Gamestate Textbox depending on whether a player is in check (or not).
    Private Sub CheckChecker(ByVal OnlyEditKing As Boolean)
        If Not OnlyEditKing Then
            If MasterWInCheck >= 128 OrElse MasterBInCheck >= 128 Then
                If GeneralOptions(0) = "T" Then Sound_Check.Play()
                CheckLabel.Text = "    Check!    "
            Else
                CheckLabel.Text = "                    "
            End If
        End If
        'Modifies king picture accordingly.
        If GeneralOptions(6) = "F" Then
            If MasterWInCheck >= 128 AndAlso GeneralOptions(4) = "T" Then
                WK1.Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\WKingCheck.png")
            Else
                WK1.Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\WKing.png")
            End If
            If MasterBInCheck >= 128 AndAlso GeneralOptions(4) = "T" Then
                BK1.Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\BKingCheck.png")
            Else
                BK1.Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\BKing.png")
            End If
        End If
    End Sub



    'Subroutine that animates a piece move (given a Move).
    Private Sub AnimateMove(ByVal TempMove As Move)
        PieceIsMoving = True
        'Locates TempPiece (the piece that is moving) and ReplacedPiece (the piece that is being captured).
        Dim TempPiece As PictureBox = FindPieceFromCoor(TempMove.OldMoveX, TempMove.OldMoveY)
        Dim ReplacedPiece As PictureBox
        If TempMove.NewMoveX & TempMove.NewMoveY = MasterEnPassant Then
            'The captured pawn is located just behind the capturing pawn.
            ReplacedPiece = FindPieceFromCoor(TempMove.NewMoveX, TempMove.NewMoveY - (2 * Val(PlayerTurn) + 1))
        Else
            ReplacedPiece = FindPieceFromCoor(TempMove.NewMoveX, TempMove.NewMoveY)
        End If

        '"Flips" the move 180 degrees if the board is flipped in black's favour.
        If Not OrientForWhite Then TempMove.Flip()

        'Creates movement vector.
        Dim XMovement As Decimal = 75 * (TempMove.NewMoveX - TempMove.OldMoveX) 'Represents delta x.
        Dim YMovement As Decimal = 75 * (TempMove.NewMoveY - TempMove.OldMoveY) 'Represents delta y.
        Dim Constant As Decimal 'Represents how many iterations to perform (more iterations = smoother movement).
        Select Case AnimationSpeed
            Case 0
                Constant = 1
            Case 1
                Constant = 5
            Case 2
                Constant = 25
            Case Else
                Constant = 75
        End Select

        'Moves the piece pixel by pixel towards its destination - updating the board as it does so.
        TempPiece.BringToFront()
        For x = 1 To Constant
            TempPiece.Left += XMovement / Constant
            TempPiece.Top += YMovement / Constant
            If AnimationSpeed = 0 OrElse AnimationSpeed = 3 Then TempPiece.Refresh() Else Application.DoEvents()
        Next

        'Pawn promotion control.
        If TempPiece.Name(1) = "P" AndAlso TempMove.NewMoveY = -7 * Val(PlayerTurn Xor OrientForWhite) Then
            'Pawn promotion - finds a queen / knight to replace the pawn, and creates a new one if it cannot be found.
            'If the user is holding down SHIFT, then the pawn is promoted to a knight.
            Dim PromotionPiece As PictureBox = FindNewPiece(TempPiece.Name(0), TempMove.Code)

            'Removes the pawn and places the queen at the required coorinates.
            PromotionPiece.Location = New Point(75 * TempMove.NewMoveX, 75 * TempMove.NewMoveY)
            PromotionPiece.Visible = True

            TempPiece.Visible = False
            TempPiece.Location = New Point(-100, -100)
        End If
        If ReplacedPiece IsNot Nothing Then 'Is a capture move - remove the captured piece.
            ReplacedPiece.Visible = False
            ReplacedPiece.Location = New Point(-100, -100)
        ElseIf TempPiece.Name(1) = "K" AndAlso (MasterWCanCastle.CanICastle OrElse MasterBCanCastle.CanICastle) Then
            If Not OrientForWhite Then TempMove.Flip()
            'Castling Controls.
            If Math.Abs(TempMove.NewMoveX - TempMove.OldMoveX) = 2 Then
                'Player is castling - animate the rook moving to the other side of the king.
                Dim RookMove As New Move With {
                    .OldMoveY = TempMove.NewMoveY,
                    .NewMoveY = TempMove.NewMoveY
                }
                'Sets coordinates for king-side and queen-side castling.
                If TempMove.NewMoveX > TempMove.OldMoveX Then
                    RookMove.OldMoveX = 7
                    RookMove.NewMoveX = 5
                ElseIf TempMove.NewMoveX < TempMove.OldMoveX Then
                    RookMove.OldMoveX = 0
                    RookMove.NewMoveX = 3
                End If
                AnimateMove(RookMove)
            End If
        End If
        PieceIsMoving = False 'Animation has finished - allow user to move another piece.
    End Sub


    'Algorithm that 'animates' the board between two states, for use when setting a FEN in InputBtn, UndoBtn, ResetBtn, etc, using the Greedy Algorithm.
    Private Sub AnimateBoard(ByVal OldFEN As String)
        Dim NewPieceDict As New Dictionary(Of String, List(Of SByte()))
        Dim OldPieceDict As New Dictionary(Of String, List(Of SByte()))
        PieceIsMoving = True

        'Formats the FEN to replace all numbers with spaces (ie: to make all rows 8 characters long, so that we can better equate it with the Board object).
        'Eg: 2N1P3 -> "  N P   "
        OldFEN = OldFEN.Substring(0, OldFEN.IndexOf(" "))
        Dim i As SByte = 0
        While i < Len(OldFEN)
            If IsNumeric(OldFEN(i)) Then
                Dim value As SByte = Val(OldFEN(i))
                OldFEN = OldFEN.Replace(OldFEN(i), New String(" ", value))
                i += value - 1
            End If
            i += 1
        End While

        'Iterate though each square of the FEN & Board respectively, looking for discrepencies. This dictionary then holds *pieces* that (before / after the
        'board transition, are no longer at their specific square).
        For y = 0 To 7
            For x = 0 To 7
                Dim pOld As Char = OldFEN(x)
                Dim PNew As Char = MasterBoard(x, y)
                If PNew <> pOld Then 'Discrepency found.
                    If PNew <> " " Then 'The transition involved some piece moving to this square.
                        If Not NewPieceDict.ContainsKey(PNew) Then NewPieceDict(PNew) = New List(Of SByte())
                        NewPieceDict(PNew).Add(New SByte() {x, y})
                    End If
                    If pOld <> " " Then 'The transition involved some piece moving from this square.
                        If Not OldPieceDict.ContainsKey(pOld) Then OldPieceDict(pOld) = New List(Of SByte())
                        OldPieceDict(pOld).Add(New SByte() {x, y})
                    End If
                End If
            Next
            OldFEN = OldFEN.Substring(OldFEN.IndexOf("/") + 1)
        Next

        Dim ListA, ListB As List(Of SByte())
        Dim Animations As New List(Of Move) 'Contains the full list of moves, representing the pieces to animate.
        Dim Creations As New List(Of Move) 'Contains the full list of pieces that are created / distroyed by the process (ie: have no match in the other dictionary).
        Dim NeedSwap As Boolean
        Dim Distance, MaxDistance, MaxIndex As Byte
        For Each key In NewPieceDict.Keys

            If OldPieceDict.ContainsKey(key) Then
                'The algorithm I am using here is: for each new square, find the distance between all possible pieces that can move to this square, via the
                'transition (ie: square e4 = "N" can have a knight moved from g1 or b1). We then choose the new square with the greatest minimum distance, and
                'assign that piece (of minimum distance) to that square. Continue this until we have no more assignments (at which point, the remaining pieces
                'are to be created / destroyed). This algorithm is effective as we can easily reverse it (ie: new square -> piece to move, piece -> new square)
                NeedSwap = OldPieceDict(key).Count < NewPieceDict(key).Count
                'This algorithm only works if there are more pieces to choose from, than there are squares that they move to. So if that is the case, we need to
                'reverse the algorithm. (Eg: consider a4="N" choosing between a1 and h1. If we don't reverse the algorithm, h1 will grab the knight, which is bad).
                If NeedSwap Then
                    ListA = NewPieceDict(key)
                    ListB = OldPieceDict(key)
                Else
                    ListA = OldPieceDict(key)
                    ListB = NewPieceDict(key)
                End If

                While ListB.Count > 0
                    Dim DistanceArray(ListB.Count - 1) As Byte
                    Dim ShortestIndexArray(ListB.Count - 1) As Byte
                    For i = 0 To ListB.Count - 1
                        DistanceArray(i) = 255
                        For j = 0 To ListA.Count - 1
                            'Calculates Euclidean Squared Distance from all pieces (in A) to the square in B. Store this in a big array for B.
                            Distance = (ListB(i)(0) - ListA(j)(0)) ^ 2 + (ListB(i)(1) - ListA(j)(1)) ^ 2
                            If Distance < DistanceArray(i) Then
                                DistanceArray(i) = Distance
                                ShortestIndexArray(i) = j
                            End If
                        Next
                    Next

                    'Finds the square with greatest minimum distance to a piece.
                    MaxDistance = 0
                    MaxIndex = 0
                    For n = 0 To ListB.Count - 1
                        If DistanceArray(n) > MaxDistance Then
                            MaxDistance = DistanceArray(n)
                            MaxIndex = n
                        End If
                    Next
                    Dim ChosenANode As SByte() = ListA(ShortestIndexArray(MaxIndex)) 'Links this square to a piece. We can then form the animation.
                    'Creates a move that represents this animation.
                    Dim TempMove As New Move With {
                        .OldMoveX = ChosenANode(0),
                        .OldMoveY = ChosenANode(1),
                        .NewMoveX = ListB(MaxIndex)(0),
                        .NewMoveY = ListB(MaxIndex)(1),
                        .Code = key
                    }
                    If NeedSwap Then TempMove.Invert() 'The algorithm has been reversed, and so our animation must be also.
                    Animations.Add(TempMove)

                    'Removes that pair in the dictionary, so they cannot be mapped again.
                    ListA.RemoveAt(ShortestIndexArray(MaxIndex))
                    ListB.RemoveAt(MaxIndex)
                End While
            Else
                ListA = NewPieceDict(key)
                NeedSwap = True
            End If

            For Each p In ListA
                'All unassigned pieces / squares will form pieces to create / destroy. Note that via the reversibility of the algorithm, the reverse of a
                'creation is a destruction. (-1,-1) denotes the graveyard (lol).
                Dim TempMove As New Move With {
                    .OldMoveX = p(0),
                    .OldMoveY = p(1),
                    .NewMoveX = -1,
                    .NewMoveY = -1,
                    .Code = key
                }
                If NeedSwap Then TempMove.Invert()
                Creations.Add(TempMove)
            Next
            ListA.Clear()
        Next
        'All remaining pieces in OldPieces are to be removed.
        For Each key In OldPieceDict.Keys
            For Each p In OldPieceDict(key)
                Dim TempMove As New Move With {
                    .OldMoveX = p(0),
                    .OldMoveY = p(1),
                    .NewMoveX = -1,
                    .NewMoveY = -1,
                    .Code = key
                }
                Creations.Add(TempMove)
            Next
        Next

        'Creates & Destroys all pieces in the Creations list.
        Dim CreationPiece As PictureBox
        For Each c In Creations
            If c.OldMoveX = "-1" Then
                'Piece for creation.
                If Not OrientForWhite Then c.Flip()
                CreationPiece = FindNewPiece(If(Char.IsUpper(c.Code), "W", "B"), UCase(c.Code))
                CreationPiece.Location = New Point(75 * c.NewMoveX, 75 * c.NewMoveY)
                CreationPiece.Visible = True
            Else
                'Piece for destruction.
                CreationPiece = FindPieceFromCoor(c.OldMoveX, c.OldMoveY, c.Code)
                CreationPiece.Visible = False
                CreationPiece.Location = New Point(-100, -100)
            End If
        Next

        If Animations.Count > 0 Then
            Dim TempPieces(Animations.Count - 1) As PictureBox
            Dim XMovement(Animations.Count - 1) As Decimal
            Dim YMovement(Animations.Count - 1) As Decimal

            'Forms the animation speed based on the user's choice (note that, as there are much more pieces moving about than a standard piece animation,
            'we speed this up over the typical setting.
            Dim Constant As Decimal
            Select Case AnimationSpeed
                Case 0
                    Constant = 1
                Case 1, 2
                    Constant = 5
                Case Else
                    Constant = 25
            End Select

            For n = 0 To Animations.Count - 1
                'Finds the piece to move, then forms its movement vector to its new square.
                Dim TestMove As Move = Animations(n)
                TempPieces(n) = FindPieceFromCoor(TestMove.OldMoveX, TestMove.OldMoveY, TestMove.Code)
                If Not OrientForWhite Then TestMove.Flip()
                XMovement(n) = 75 * (TestMove.NewMoveX - TestMove.OldMoveX)
                YMovement(n) = 75 * (TestMove.NewMoveY - TestMove.OldMoveY)
                TempPieces(n).BringToFront()
            Next
            For x = 1 To Constant
                'Moves all the pieces one pixel in the direction given by the movement vector.
                For n = 0 To TempPieces.Count - 1
                    TempPieces(n).Left += XMovement(n) / Constant
                    TempPieces(n).Top += YMovement(n) / Constant
                    'Renders this animation to the GUI.
                    If AnimationSpeed = 1 OrElse AnimationSpeed = 3 Then TempPieces(n).Refresh() Else Application.DoEvents()
                Next
            Next
        End If

        PieceIsMoving = False
    End Sub

End Class
