'Class containing all the core GUI elements, such as displaying the checkerboard & its intricacies, displaying the pieces on the board, animating moves, etc.
Partial Public Class Chess

    Private PrimaryColour As Color 'Colour representing the dark squares on the Chessboard.
    Private SecondaryColour As Color 'Colour representing the light squares on the Chessboard.
    Private SquareHistory(3, 1) As SByte 'Array containing the previously used squares on the board (for prev moves).
    Private LegalMoveSquares(7, 7) As Boolean 'Array containing the coordinates of where a piece can move on the board,
    '... generated when the user clicks on a piece. Used when redrawing the checkerboard pattern.
    Private ReadOnly PieceArray As New List(Of PictureBox) 'Contains all the PictureBoxes on the board.
    Private AnimationSpeed As Byte = GlobalConstants.DefaultAnimationSpeed





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
                If LegalMoveSquares(x, y) AndAlso GeneralOptions(4) = "T" AndAlso Not BoardEditMode Then
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
                    If LegalMoveSquares(x, y) AndAlso (GeneralOptions(4) = "T" OrElse BoardEditMode) Then 'Normal colour is filled at the centre of the legal move square to produce a green / blue highlight.
                        Dim Square2 As New Rectangle((75 * x) + 5, (75 * y) + 5, 65, 65)
                        Using Brush As New SolidBrush(SecondaryColour)
                            g.FillRectangle(Brush, Square2)
                        End Using
                        'If the given square matches a SquareHistory coordinate, then it is coloured in a different way.
                    ElseIf ((x = SquareHistory(0, 0) AndAlso y = SquareHistory(0, 1)) OrElse (x = SquareHistory(1, 0) AndAlso y = SquareHistory(1, 1))) AndAlso GeneralOptions(3) = "T" AndAlso Not BoardEditMode Then
                        Using Brush As New SolidBrush(Color.YellowGreen) 'SquareHistory secondary colour.
                            g.FillRectangle(Brush, Square)
                        End Using
                    Else 'Is a normal square - colour with user's seconday colour.
                        Using Brush As New SolidBrush(SecondaryColour)
                            g.FillRectangle(Brush, Square)
                        End Using
                    End If
                Else 'Identical code but for the dark squares (uses different colours).
                    If LegalMoveSquares(x, y) AndAlso (GeneralOptions(4) = "T" OrElse BoardEditMode) Then
                        Dim Square2 As New Rectangle((75 * x) + 5, (75 * y) + 5, 65, 65)
                        Using Brush As New SolidBrush(PrimaryColour)
                            g.FillRectangle(Brush, Square2)
                        End Using
                    ElseIf ((x = SquareHistory(0, 0) AndAlso y = SquareHistory(0, 1)) OrElse (x = SquareHistory(1, 0) AndAlso y = SquareHistory(1, 1))) AndAlso GeneralOptions(3) = "T" AndAlso Not BoardEditMode Then

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



    'Algorithm that displays all the pieces on the board, given a board array.
    Private Sub DisplayPieces()
        Dim NewPiece As New PictureBox
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
    'MasterBoard to its PictureBox counterpart on the board.
    Private Function CoorToPieceConverter(ByVal CoorX As Int16, ByVal CoorY As Int16) As PictureBox
        Dim CurrentPiece As String
        Dim TempPiece As PictureBox = Nothing
        Dim Counter As Byte
        If MasterBoard(CoorX, CoorY) <> " " Then
            'In MasterBoard(), a lowercase letter represents a black piece, and an uppercase letter represents a white piece.
            If Char.IsUpper(MasterBoard(CoorX, CoorY)) Then
                CurrentPiece = "W"
            Else
                CurrentPiece = "B"
            End If
            'CurrentPiece is formed, which has the same name as a corresponding PictureBox on the board.
            CurrentPiece &= UCase(MasterBoard(CoorX, CoorY))

            Counter = 1
            While Counter < 100
                CurrentPiece = CurrentPiece.Substring(0, 2) & Counter
                'Searches for piece that has not been placed on the board (that matches CurrentPiece / TempPiece's name + location), and moves its match into place on the board.
                For n = 0 To PieceArray.Count - 1
                    If CurrentPiece = PieceArray(n).Name Then
                        TempPiece = PieceArray(n)
                        Exit For
                    End If
                    If n = PieceArray.Count - 1 Then TempPiece = Nothing
                Next

                If TempPiece IsNot Nothing Then
                    If OrientForWhite AndAlso (TempPiece.Location.X = CoorX * 75 AndAlso TempPiece.Location.Y = CoorY * 75) Then
                        Exit While
                    ElseIf Not OrientForWhite AndAlso (TempPiece.Location.X = (7 - CoorX) * 75 AndAlso TempPiece.Location.Y = (7 - CoorY) * 75) Then
                        Exit While
                    End If
                End If

                'If not found, the search continues with an increasing last digit (eg: the program seaches for BP1, then BP2, then BP3 ext until it finds its match.)
                Counter += 1
            End While
        End If
        Return TempPiece
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
                WK1.Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\WKingCheck.png")
            Else
                WK1.Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\WKing.png")
            End If
            If MasterBInCheck >= 128 AndAlso GeneralOptions(4) = "T" Then
                BK1.Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\BKingCheck.png")
            Else
                BK1.Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\BKing.png")
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
            If AnimationSpeed = 3 Then TempPiece.Refresh() Else Application.DoEvents()
        Next

        'Pawn promotion control.
        If TempPiece.Name(1) = "P" AndAlso TempMove.NewMoveY = -7 * Val(PlayerTurn Xor OrientForWhite) Then
            'Pawn promotion - finds a queen / knight to replace the pawn, and creates a new one if it cannot be found.
            'If the user is holding down SHIFT, then the pawn is promoted to a knight.
            Dim PromotionPiece As PictureBox = FindNewPiece(TempPiece.Name(0), TempMove.EndState)

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
            If Not OrientForWhite Then
                TempMove.OldMoveX = 7 - TempMove.OldMoveX
                TempMove.OldMoveY = 7 - TempMove.OldMoveY
                TempMove.NewMoveX = 7 - TempMove.NewMoveX
                TempMove.NewMoveY = 7 - TempMove.NewMoveY
            End If
            'Castling Controls.
            If Math.Abs(TempMove.NewMoveX - TempMove.OldMoveX) = 2 Then
                'Player is castling - animate the rook moving to the other side of the king.
                Dim RookMove As New Move
                RookMove.OldMoveY = TempMove.NewMoveY
                RookMove.NewMoveY = TempMove.NewMoveY
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

    'Function which finds, or creates, a new piece type that can be placed on the board. If one cannot be found, the system creates a new piece.
    Private Function FindNewPiece(ByVal PieceColourCode As Char, ByVal PieceType As Char) As PictureBox
        Dim Piece As PictureBox
        Dim TemplatePiece As PictureBox
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
                    Dim NewPiece As New PictureBox
                    NewPiece.Name = CurrentPiece
                    NewPiece.Location = New Point(-100, -100)
                    NewPiece.Size = TemplatePiece.Size
                    NewPiece.Image = TemplatePiece.Image
                    NewPiece.BackColor = Color.Transparent
                    NewPiece.Visible = False

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

End Class
