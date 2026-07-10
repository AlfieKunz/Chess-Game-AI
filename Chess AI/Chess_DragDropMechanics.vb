Imports System.Threading

'This class contains all the information regarding the drag & drop mechanics of the UI, along with the handles for submitting a move into the system.
Partial Public Class Chess 'DragDrop Mechanics

    Private Structure PieceInfo 'Contains information about a piece that the user is moving on the GUI.
        Dim IsMovingPiece As Boolean 'Is the piece being moved by the user?
        Dim HasMoved As SByte 'Has the piece actually been moved? If not, enable 'click-move' mechanics.
        Dim Piece As PictureBox 'Specifies the exact object that is being moved.

        Dim LegalMoves() As String 'Array containing the legal Moves of the piece being dragged.
        Dim LockedPiece As String 'Contains the location of the piece that is locked by the user (applies if Touch Move is enabled).

        Dim StartPoint As Point
        Dim StartMouseLocation As Point
        Dim MidPoint As Point
        Dim EndPoint As Point
    End Structure

    Private PieceMoving As PieceInfo
    Private ClickMoveMode As Boolean 'Toggles the 'Click-Move Mode' feature - if the user clicks on a piece, rather than moving it,
    'and then clicks on a legal square, the piece is moved to that square.





    'The below three subroutines control the drag & drop mechanics for the pieces, and translates a user's move onto the board.
    Private Sub Piece_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        'Subroutine that activates the drag + drog mechanic if the user clicks on a piece.
        If e.Button = MouseButtons.Left Then
            Dim CoorX As SByte = sender.location.X / 75
            Dim CoorY As SByte = sender.location.Y / 75
            If BoardEdit.isEnabled Then
                'Handles BoardEdit piece drag & dropping - duplicates the piece if it is a template.
                If sender.location.X > 600 Then
                    'Piece is off-screen: duplicate it.
                    Dim ReplacedPiece As PictureBox = FindNewPiece(sender.Name(0), sender.Name(1))
                    ReplacedPiece.Location = sender.location
                    ReplacedPiece.Visible = True
                Else
                    'Remove the piece from the board, so that the user can replace the piece on the original square (also useful for Copy mode).
                    If OrientForWhite Then
                        MasterBoard(CoorX, CoorY) = " "
                    Else
                        MasterBoard(7 - CoorX, 7 - CoorY) = " "
                    End If
                End If
                'Enables the core parts of the drag & drop mechanics.
                ResetDragDropMechanics(sender, e)
                Checkerboard.Refresh()

            ElseIf GameRunning AndAlso Not ComputerIsSearching AndAlso Not PieceIsMoving AndAlso (GameMode <> 0 OrElse RemoteModeCanClickPiece()) Then
                'If the user clicks on an enemy piece during 'Click-Move Mode', then they may be wanting to capture this piece.
                If ClickMoveMode AndAlso (sender.Name(0) = "W" Xor PlayerTurn) Then HandleClickMoveMode(CoorX, CoorY) : Exit Sub

                'If the piece corresponds to the player allowed to move...
                If (sender.name(0) = "W" AndAlso PlayerTurn) OrElse (sender.name(0) = "B" AndAlso (Not PlayerTurn)) Then
                    'Checks if the piece is locked by the user. If it is not, then the user is able to proceed.
                    If PieceMoving.LockedPiece = "" OrElse PieceMoving.LockedPiece = CoorX & CoorY Then
                        Me.Cursor = New Cursor(New System.IO.MemoryStream(My.Resources.Cursor16)) 'Can be Cursor32 or Cursor192
                        'Generates the Legal Moves for the piece the user has picked up (using the AI).
                        ResetLMS(False)
                        If OrientForWhite Then
                            PieceMoving.LegalMoves = MainAI.ReturnPiecesLegalMoves(CoorX, CoorY)
                            'Updates LegalMoveSquares array.
                            If PieceMoving.LegalMoves IsNot Nothing Then
                                For n = 0 To PieceMoving.LegalMoves.Length - 1
                                    LegalMoveSquares(Val(PieceMoving.LegalMoves(n)(0)), Val(PieceMoving.LegalMoves(n)(1))) = True
                                Next
                            End If
                        Else
                            PieceMoving.LegalMoves = MainAI.ReturnPiecesLegalMoves(7 - CoorX, 7 - CoorY)
                            'Updates LegalMoveSquares array.
                            If PieceMoving.LegalMoves IsNot Nothing Then
                                For n = 0 To PieceMoving.LegalMoves.Length - 1
                                    LegalMoveSquares(7 - Val(PieceMoving.LegalMoves(n)(0)), 7 - Val(PieceMoving.LegalMoves(n)(1))) = True
                                Next
                            End If
                        End If
                        'Creates the PieceMoving values, then redraws the checkerboard (creates LegalMoveSquares).
                        ResetDragDropMechanics(sender, e)
                        'If the user is using Touch Move, and the piece has >= 1 legal move, lock that piece so that the user cannot move any piece.
                        If GeneralOptions(5) = "T" AndAlso PieceMoving.LegalMoves IsNot Nothing Then PieceMoving.LockedPiece = CoorX & CoorY
                        Checkerboard.Refresh()
                    Else
                        'Stops the piece from being moved abruptly.
                        PieceMoving.StartPoint = sender.location
                    End If
                Else
                    'Stops the piece from being moved abruptly.
                    PieceMoving.StartPoint = sender.location
                End If
            End If
        End If
    End Sub

    Private Sub Piece_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If PieceMoving.IsMovingPiece Then
            'The piece is bound to the position of the user's mouse.
            PieceMoving.EndPoint = sender.PointToScreen(New Point(e.X, e.Y))
            sender.Left += (PieceMoving.EndPoint.X - PieceMoving.MidPoint.X)
            sender.Top += (PieceMoving.EndPoint.Y - PieceMoving.MidPoint.Y)
            PieceMoving.MidPoint = PieceMoving.EndPoint

            'Subtracts a 'move element' from HasMoved. If this reaches 0, then the ClickMove mechanic will not actuate.
            If Not (PieceMoving.HasMoved = 0 OrElse PieceMoving.StartMouseLocation = PieceMoving.EndPoint) Then PieceMoving.HasMoved -= 1

            'Duplicate piece as it moves across the board.
            If BoardEdit.isEnabled AndAlso Control.ModifierKeys = Keys.Control Then HandleBoardEditModeCopy(sender)

            'Updates the Checkerboard object for a smoother animation.
            Checkerboard.Invalidate()
            Checkerboard.Update()
        End If
    End Sub

    Private Sub Piece_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim IsLegalMove As Boolean
        Dim ReplacedPiece As PictureBox
        Dim TempMove As New Move
        If e.Button = MouseButtons.Left AndAlso PieceMoving.IsMovingPiece Then

            For Each Piece In PieceArray
                'If the PictureBox is within the confines of the Checkerboard, and it isn't the piece being moved by the user...
                If Piece.Location.X >= 0 AndAlso Piece.Location.Y >= 0 AndAlso Piece.Name <> PieceMoving.Piece.Name Then
                    Piece.Visible = True 'Alternate style of flickering - may be less distracting??
                End If
            Next

            Me.Cursor = Cursors.Default 'Resets cursor.
            'Disables drag & drop mechanic and sets the piece's position on the board (into the centre of the square).
            PieceMoving.IsMovingPiece = False
            Dim SnapLocation As Point = CalculatePieceSquareSnapPosition(sender)

            If BoardEdit.isEnabled Then HandleBoardEditModeDrop(sender, SnapLocation, PieceMoving.StartPoint) : Exit Sub
            'If the piece has not moved (much), snap it back into place and enable ClickMoveMode.
            'This retains the LMS information on the GUI, so that the user can make a move by clicking on one of these squares.
            If PieceMoving.HasMoved > 0 Then ClickMoveMode = True : sender.location = PieceMoving.StartPoint : Exit Sub Else ClickMoveMode = False

            'Resets LegalMoveSquares array & redraws checkerboard.
            ResetLMS(True)

            'If the user has actually moved the piece...
            If SnapLocation <> PieceMoving.StartPoint Then
                'Forms the AI-friendly Move, that represents the user's move.
                TempMove.OldMoveX = PieceMoving.StartPoint.X / 75
                TempMove.OldMoveY = PieceMoving.StartPoint.Y / 75
                TempMove.NewMoveX = SnapLocation.X / 75
                TempMove.NewMoveY = SnapLocation.Y / 75
                If Not OrientForWhite Then TempMove.Flip() 'Flips coordinates 180 degrees.

                'Checks if the user's move is in the list of that piece's legal moves.
                IsLegalMove = False
                If PieceMoving.LegalMoves IsNot Nothing Then
                    For n = 0 To PieceMoving.LegalMoves.GetUpperBound(0)
                        If TempMove.NewMoveX & TempMove.NewMoveY = PieceMoving.LegalMoves(n) Then
                            IsLegalMove = True
                            Exit For
                        End If
                    Next
                End If

                If IsLegalMove Then

                    'Makes a note on TempMove if a pawn is being promoted.
                    Dim PromotionFlag As Boolean
                    If sender.name(1) = "P" AndAlso (sender.name(0) = "W" AndAlso TempMove.NewMoveY = 0 OrElse sender.name(0) = "B" AndAlso TempMove.NewMoveY = 7) Then
                        'If the user is holding down SHIFT, then the pawn is promoted to a knight.
                        PromotionFlag = True
                        TempMove.Code = If(Control.ModifierKeys = Keys.Control, "N", "Q")
                    End If

                    If GameMode = 6 Then
                        'Move Training input. Resets piece to previous position, if a new position has been generated.
                        If Not HandleMoveTrainingInput(TempMove) Then sender.location = PieceMoving.StartPoint
                        Exit Sub
                    End If

                    sender.location = SnapLocation 'Snap the piece into its box.
                    If GameMode = 4 Then
                        If AutoAdvanceOnComplete.Checked Then Checkerboard.Refresh() 'Updates the GUI during the puzzle wait period.
                        If Not HandlePuzzleMoveInput(TempMove) Then
                            'If puzzle mode, legal moves are determined by whether the puzzle is correct.
                            If Not AutoAdvanceOnComplete.Checked Then sender.location = PieceMoving.StartPoint
                            Exit Sub
                        End If
                    End If


                    'Move is legal for the standard game of chess.
                    If UCase(sender.name(1)) = "K" AndAlso Math.Abs(TempMove.NewMoveX - TempMove.OldMoveX) = 2 Then
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
                        'If the move is a capture, the required sound is played and the captured piece is removed from the board.
                    ElseIf MasterBoard(TempMove.NewMoveX, TempMove.NewMoveY) <> " " Then
                        ReplacedPiece = FindPieceFromCoor(TempMove.NewMoveX, TempMove.NewMoveY, MasterBoard(TempMove.NewMoveX, TempMove.NewMoveY)) 'Finds captured PictureBox.
                        'Removes captured PictureBox.
                        ReplacedPiece.Visible = False
                        ReplacedPiece.Location = New Point(-100, -100)
                    End If

                    'Code for special pawn moves (ie: promotion & en-passant).
                    If sender.name(1) = "P" Then
                        'If a pawn has made it to the end of the board, it is promoted to a Queen / Knight.
                        If PromotionFlag Then
                            'Pawn promotion - finds a queen / knight to replace the pawn.
                            ReplacedPiece = FindNewPiece(sender.name(0), TempMove.Code)
                            'Moves this queen piece to replace the pawn.
                            ReplacedPiece.Location = New Point(sender.location.X, sender.location.Y)
                            ReplacedPiece.Visible = True
                            ReplacedPiece.BringToFront()
                            'Replaces the Board index with a queen, then removes the pawn PictureBox from the screen.
                            sender.Visible = False
                            sender.Location = New Point(-100, -100)
                        ElseIf TempMove.NewMoveX & TempMove.NewMoveY = MasterEnPassant Then
                            'Rules for En Passant - removes the piece behind it.
                            Dim IndexShifter As SByte
                            'Locates the pawn behind it.
                            If sender.name(0) = "W" Then IndexShifter = 1 Else IndexShifter = -1
                            ReplacedPiece = FindPieceFromCoor(TempMove.NewMoveX, TempMove.NewMoveY + IndexShifter)
                            'Removes this pawn.
                            ReplacedPiece.Visible = False
                            ReplacedPiece.Location = New Point(-100, -100)
                        End If
                    End If

                    'Move has been successfully animated on the screen - process the move into the system.
                    SubmitMoveIntoSystem(TempMove, True)

                Else
                    'The move is not legal - move it back to its original position.
                    sender.location = PieceMoving.StartPoint
                End If
            Else
                'The piece has not moved squares - move it back to its original position.
                sender.location = PieceMoving.StartPoint
            End If
        End If
    End Sub



    'Subroutine which initiates the drag-drop mechanics for a piece - calibrating the core information.
    Private Sub ResetDragDropMechanics(ByVal Piece As PictureBox, ByVal MouseLocation As System.Windows.Forms.MouseEventArgs)
        'Creates the PieceMoving values, then redraws the checkerboard (creates LegalMoveSquares).
        Piece.BringToFront()
        PieceMoving.IsMovingPiece = True
        PieceMoving.HasMoved = 12 'Represents the number of Piece_MouseMove actuations a piece can undergo before it is classified as being 'moved'.
        PieceMoving.Piece = Piece
        'Notes location of the piece for drag & drop mechanics.
        PieceMoving.StartPoint = Piece.Location
        PieceMoving.StartMouseLocation = Piece.PointToScreen(New Point(MouseLocation.X, MouseLocation.Y))
        PieceMoving.MidPoint = PieceMoving.StartMouseLocation
    End Sub


    'Calculates the position of a given piece, after being 'snapped' into its respective square on the chessboard.
    Private Function CalculatePieceSquareSnapPosition(ByVal Piece As PictureBox) As Point
        'Anchors position to top-left corner of square.
        Dim ScaledXCoor As Double = Piece.Location.X + 37.5
        Dim ScaledYCoor As Double = Piece.Location.Y + 37.5
        'If the piece is _just_ outside the confines of the LHS / Top-Side, mod will -> 0. To make this piece easier to remove in
        'Board Edit Mode, manually override this to index -1, so that it does not map to a proper square.
        Dim SnappedXCoor As Double = If(ScaledXCoor >= 0, ScaledXCoor - (ScaledXCoor Mod 75), -75)
        Dim SnappedYCoor As Double = If(ScaledYCoor >= 0, ScaledYCoor - (ScaledYCoor Mod 75), -75)
        Return New Point(CInt(SnappedXCoor), CInt(SnappedYCoor))
    End Function



    'Subroutine which handles a click actuation on the 'Click-Move Mode' feature.
    Private Sub HandleClickMoveMode(ByVal XLocation As SByte, ByVal YLocation As SByte)
        If Not (ComputerIsSearching OrElse PieceIsMoving) Then
            'If the user has not clicked on a legal square, disable the mode.
            ClickMoveMode = False
            If LegalMoveSquares(XLocation, YLocation) Then
                'Creates a new Move corresponding to this action.
                Dim ClickMove As New Move With {
                    .OldMoveX = If(OrientForWhite, PieceMoving.StartPoint.X / 75, 7 - PieceMoving.StartPoint.X / 75),
                    .OldMoveY = If(OrientForWhite, PieceMoving.StartPoint.Y / 75, 7 - PieceMoving.StartPoint.Y / 75),
                    .NewMoveX = If(OrientForWhite, XLocation, 7 - XLocation),
                    .NewMoveY = If(OrientForWhite, YLocation, 7 - YLocation)
                }
                'Sets promotion piece, if needed (if the user is holding SHIFT, then promotion to a knight).
                If (ClickMove.NewMoveY = 0 AndAlso ClickMove.OldMoveY = 1) OrElse (ClickMove.NewMoveY = 7 AndAlso ClickMove.OldMoveY = 6) Then
                    ClickMove.Code = If(Control.ModifierKeys = Keys.Control, "N", "Q")
                End If

                If GameMode = 6 Then
                    'The user is in the Move Training Mode - handle data via the Training Module.
                    ResetLMS(False)
                    HandleMoveTrainingInput(ClickMove)
                    Exit Sub
                ElseIf GameMode <> 4 OrElse (GameMode = 4 AndAlso HandlePuzzleMoveInput(ClickMove)) Then
                    'If the user is in puzzle mode, the move is only submitted if it matches the puzzle's correct move.
                    AnimateMove(ClickMove)
                    SubmitMoveIntoSystem(ClickMove, True)
                    Exit Sub
                End If
            End If
            'If the move failed, for whatever reason, clear the LMS array & refresh the screen.
            ResetLMS(True)
        End If
    End Sub




    'Function that takes a user's move, and enters it into the system - handling core system calibration, PGNs, AI calibration & playing (for 1-player modes), etc.
    Private Sub SubmitMoveIntoSystem(ByVal TempMove As Move, ByVal PrintUpdatedBoard As Boolean)
        'If the AI is searching on the position in the background, then terminate this AI immediately.
        If AIIsSearchingOnUsersTurn Then MainAI.ABORTSearch()
        PieceMoving.LockedPiece = ""
        AIHandles.FENToResetTo = StartingFEN

        'Calculates the PGN equivilent of the user's move.
        Dim PGNMove As String
        If PlayerTurn Then
            PGNMove = Helper.MoveConverter(MasterBoard, TempMove, True, MasterWKPos, Helper.ConvertStringToBitCoor(MasterEnPassant), MasterWhiteTFTable)
        Else
            PGNMove = Helper.MoveConverter(MasterBoard, TempMove, False, MasterBKPos, Helper.ConvertStringToBitCoor(MasterEnPassant), MasterBlackTFTable)
        End If


        'GUI controls have been completed - update board to match the user's move.
        CalibrateCoreSystemsForMove(TempMove, True, False)

        If GameMode <> 4 Then
            'If the AI is searching on the position in the background, then give the AI enough time to exit the search.
            If AIIsSearchingOnUsersTurn Then Thread.Sleep(2) : SearchSettings.OutputToConsole = True : AIIsSearchingOnUsersTurn = False
            'Perform GC Collect if AIIsSearchingOnUsersTurn??????
            AIHandles.MoveWasAIPredicted = MainAI.CheckIfMoveIsTTPrediction(TempMove)
            MainAI.Reconfigure(CurrentFEN, False) 'Recalibrates AI before checking for end states.
            'If the player has been put in check, and has not been checkmated, then add the + symbol to the end of the move.
            If (MasterWInCheck >= 128 OrElse MasterBInCheck >= 128) AndAlso GameRunning Then PGNMove &= "+"
            BoardHistory.PushPGN(PGNMove, True)
            EnforceEndStates()
            If CurrentFEN = PreviousFEN AndAlso UserPlayer = PlayerTurn Then Exit Sub 'Stops AI from running if it is the start position (and it is the user's turn).
        End If

        'Outputs the debug information for the new position, and prepares the next position / move, if needed.
        If GameMode <= 3 Then
            If PrintUpdatedBoard Then OutputDebugInfo()
            If GameMode < 3 Then
                If PGNAutoOutputter.Checked Then PGNExport_Click() Else FENExport_Click()
            End If
            If GameMode = 1 Then InitialiseAISystem()
        ElseIf GameMode = 4 Then
            'Move matches the correct puzzle move, so either play the next move of the puzzle, or flag the puzzle as complete.
            CancelAIPuzzleSearch()
            MainAI.Reconfigure(CurrentFEN, False) 'Recalibrates AI before checking for end states.
            EnforceEndStates()
            PuzzleMoveCompleted()
        End If
    End Sub


    'Subroutine which updates the core characteristics of the chess system, after a move has been played (ie: makes move onto board).
    Private Sub CalibrateCoreSystemsForMove(ByVal TempMove As Move, ByVal CanUndoMove As Boolean, ByVal FastUpdate As Boolean)
        'Makes move onto board, and resets the correct player's TFTable.
        If PlayerTurn Then
            MasterWInCheck = 0 'Player is no longer in check.
            MakeMove(MasterBoard, TempMove, MasterWCanCastle, MasterWKPos, MasterEnPassant, GeneralOptions(0) = "T")
            Helper.FixTFTable(MasterBoard, False, MasterBlackTFTable, Helper.ConvertStringToBitCoor(MasterBKPos), MasterBInCheck, MasterBCanCastle.CanICastle(), Helper.ConvertStringToBitCoor(MasterEnPassant))
        Else
            MasterBInCheck = 0 'Player is no longer in check.
            MakeMove(MasterBoard, TempMove, MasterBCanCastle, MasterBKPos, MasterEnPassant, GeneralOptions(0) = "T")
            Helper.FixTFTable(MasterBoard, True, MasterWhiteTFTable, Helper.ConvertStringToBitCoor(MasterWKPos), MasterWInCheck, MasterWCanCastle.CanICastle(), Helper.ConvertStringToBitCoor(MasterEnPassant))
        End If


        'Update Previously Used Squares, then flips the board if necessary.
        If GameMode = 3 Then
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
        If AutoFlipper.Checked Then
            FlipBoard()
        Else
            If FastUpdate Then Checkerboard.Invalidate() Else Checkerboard.Refresh()
        End If
        EditCheckText()

        'Ends the turn, then calculates the new position's FEN.
        PlayerTurn = Not PlayerTurn
        If CanUndoMove Then PreviousFEN = CurrentFEN
        CurrentFEN = Helper.ConvertToFEN(MasterBoard, MasterWCanCastle, MasterBCanCastle, Helper.ConvertStringToBitCoor(MasterEnPassant), PlayerTurn)

        If GameMode = 0 AndAlso Not ((Not PlayerTurn) Xor OrientForWhite) Then PlayMoveOnInterface(TempMove)
    End Sub

End Class
