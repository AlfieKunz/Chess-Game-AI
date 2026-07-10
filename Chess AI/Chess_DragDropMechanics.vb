Imports System.Threading

'This class contains all the information regarding the drag & drop mechanics of the UI, along with the handles for submitting a move into the system.
Partial Public Class Chess

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
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Dim CoorX As SByte = sender.location.X / 75
            Dim CoorY As SByte = sender.location.Y / 75
            If BoardEditMode Then
                'Handles BoardEdit piece drag & dropping - duplicates the piece if it is a template.
                If sender.location.X > 600 Then
                    'Piece is off-screen: duplicate it.
                    Dim ReplacedPiece As PictureBox = FindNewPiece(sender.Name(0), sender.Name(1))
                    ReplacedPiece.Location = sender.location
                    ReplacedPiece.Visible = True
                Else
                    'Remove the piece from the board.
                    If OrientForWhite Then
                        MasterBoard(CoorX, CoorY) = " "
                    Else
                        MasterBoard(7 - CoorX, 7 - CoorY) = " "
                    End If
                End If
                'Enables the core parts of the drag & drop mechanics.
                ResetDragDropMechanics(sender, e)
                Checkerboard.Refresh()

            ElseIf GameRunning AndAlso Not ComputerIsSearching AndAlso Not PieceIsMoving Then
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
            If BoardEditMode AndAlso Control.ModifierKeys = Keys.Control Then HandleBoardEditModeCopy(sender)

            'Updates the Checkerboard object for a smoother animation.
            Checkerboard.Invalidate()
            Checkerboard.Update()
        End If
    End Sub

    Private Sub Piece_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim IsLegalMove As Boolean
        Dim ReplacedPiece As PictureBox = Nothing
        Dim TempMove As New Move
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso PieceMoving.IsMovingPiece Then

            For Each Piece In PieceArray
                'If the PictureBox is within the confines of the Checkerboard, and it isn't the piece being moved by the user...
                If Piece.Location.X >= 0 AndAlso Piece.Location.Y >= 0 AndAlso Piece.Name <> PieceMoving.Piece.Name Then
                    'Draws the PictureBox's image into the Checkerboard.
                    Piece.Visible = True 'Alternate style of flickering - may be less distracting??
                End If
            Next

            Me.Cursor = Cursors.Default 'Resets cursor.
            'Disables drag & drop mechanic and sets the piece's position on the board (into the centre of the square).
            PieceMoving.IsMovingPiece = False
            Dim SnapLocation As Point = CalculatePieceSquareSnapPosition(sender)

            If BoardEditMode Then HandleBoardEditModeDrop(sender, SnapLocation, PieceMoving.StartPoint) : Exit Sub
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
                If Not OrientForWhite Then 'Flips coordinates 180 degrees.
                    TempMove.OldMoveX = 7 - TempMove.OldMoveX
                    TempMove.OldMoveY = 7 - TempMove.OldMoveY
                    TempMove.NewMoveX = 7 - TempMove.NewMoveX
                    TempMove.NewMoveY = 7 - TempMove.NewMoveY
                End If

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
                        TempMove.EndState = If(Control.ModifierKeys = Keys.Shift, "N", "Q")
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
                    'If the AI is searching on the position in the background, then terminate this AI immediately.
                    If AIIsSearchingOnUsersTurn Then MainAI.ABORTSearch()
                    PieceMoving.LockedPiece = ""

                    If UCase(sender.name(1)) = "K" AndAlso Math.Abs(TempMove.NewMoveX - TempMove.OldMoveX) = 2 Then
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
                        'If the move is a capture, the required sound is played and the captured piece is removed from the board.
                    ElseIf MasterBoard(TempMove.NewMoveX, TempMove.NewMoveY) <> " " Then
                        ReplacedPiece = CoorToPieceConverter(TempMove.NewMoveX, TempMove.NewMoveY) 'Finds captured PictureBox.
                        'Removes captured PictureBox.
                        ReplacedPiece.Visible = False
                        ReplacedPiece.Location = New Point(-100, -100)
                    End If

                    'Code for special pawn moves (ie: promotion & en-passant).
                    If sender.name(1) = "P" Then
                        'If a pawn has made it to the end of the board, it is promoted to a Queen / Knight.
                        If PromotionFlag Then
                            'Pawn promotion - finds a queen / knight to replace the pawn.
                            ReplacedPiece = FindNewPiece(sender.name(0), TempMove.EndState)
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
                            ReplacedPiece = CoorToPieceConverter(TempMove.NewMoveX, TempMove.NewMoveY + IndexShifter)
                            'Removes this pawn.
                            ReplacedPiece.Visible = False
                            ReplacedPiece.Location = New Point(-100, -100)
                        End If
                    End If

                    'Move has been successfully animated on the screen - process the move into the system.
                    SubmitMoveIntoSystem(TempMove)

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
        Dim XCoor As Int16 = Piece.Location.X + 37.5 - (Piece.Location.X + 37.5) Mod 75
        Dim YCoor As Int16 = Piece.Location.Y + 37.5 - (Piece.Location.Y + 37.5) Mod 75
        Return New Point(XCoor, YCoor)
    End Function



    'Subroutine which handles a click actuation on the 'Click-Move Mode' feature.
    Private Sub HandleClickMoveMode(ByVal XLocation As SByte, ByVal YLocation As SByte)
        If Not (ComputerIsSearching OrElse PieceIsMoving) Then
            'If the user has not clicked on a legal square, disable the mode.
            ClickMoveMode = False
            If LegalMoveSquares(XLocation, YLocation) Then
                'Creates a new Move corresponding to this action.
                Dim ClickMove As New Move
                ClickMove.OldMoveX = If(OrientForWhite, PieceMoving.StartPoint.X / 75, 7 - PieceMoving.StartPoint.X / 75)
                ClickMove.OldMoveY = If(OrientForWhite, PieceMoving.StartPoint.Y / 75, 7 - PieceMoving.StartPoint.Y / 75)
                ClickMove.NewMoveX = If(OrientForWhite, XLocation, 7 - XLocation)
                ClickMove.NewMoveY = If(OrientForWhite, YLocation, 7 - YLocation)
                'Sets promotion piece, if needed (if the user is holding SHIFT, then promotion to a knight).
                If (ClickMove.NewMoveY = 0 AndAlso ClickMove.OldMoveY = 1) OrElse (ClickMove.NewMoveY = 7 AndAlso ClickMove.OldMoveY = 6) Then
                    ClickMove.EndState = If(Control.ModifierKeys = Keys.Shift, "N", "Q")
                End If

                If GameMode = 6 Then
                    'The user is in the Move Training Mode - handle data via the Training Module.
                    ResetLMS(False)
                    HandleMoveTrainingInput(ClickMove)
                    Exit Sub
                ElseIf GameMode <> 4 OrElse (GameMode = 4 AndAlso HandlePuzzleMoveInput(ClickMove)) Then
                    'If the user is in puzzle mode, the move is only submitted if it matches the puzzle's correct move.
                    AnimateMove(ClickMove)
                    SubmitMoveIntoSystem(ClickMove)
                    Exit Sub
                End If
            End If
            'If the move failed, for whatever reason, clear the LMS array & refresh the screen.
            ResetLMS(True)
        End If
    End Sub




    'Function that takes a user's move, and enters it into the system - handling core system calibration, PGNs, AI calibration & playing (for 1-player modes), etc.
    Private Sub SubmitMoveIntoSystem(ByVal TempMove As Move)
        'Calculates the PGN equivilent of the user's move.
        Dim PGNMove As String
        If PlayerTurn Then
            PGNMove = SharedAlgorithms.MoveConverter(MasterBoard, TempMove, True, MasterWKPos, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant), MasterWhiteTFTable)
        Else
            PGNMove = SharedAlgorithms.MoveConverter(MasterBoard, TempMove, False, MasterBKPos, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant), MasterBlackTFTable)
        End If


        'GUI controls have been completed - update board to match the user's move.
        CalibrateCoreSystemsForMove(TempMove, True)
        If GameMode <> 4 Then
            'If the AI is searching on the position in the background, then give the AI enough time to exit the search.
            If AIIsSearchingOnUsersTurn Then Thread.Sleep(5) : SearchSettings.OutputToConsole = True : AIIsSearchingOnUsersTurn = False
            'Perform GC Collect if AIIsSearchingOnUsersTurn??????
            MainAI.Reconfigure(CurrentFEN, False) 'Recalibrates AI before checking for end states.
            EnforceEndStates(True)
            'If the player has been put in check, and has not been checkmated, then add the + symbol to the end of the move.
            If (MasterWInCheck >= 128 OrElse MasterBInCheck >= 128) AndAlso GameRunning Then PGNMove &= "+"
            BoardHistory.PushPGN(PGNMove)
            If CurrentFEN = PreviousFEN AndAlso UserPlayer = PlayerTurn Then Exit Sub 'Stops AI from running if it is the start position (and it is the user's turn).
        End If

        If GameMode <= 3 Then
            OutputDebugInfo()
            If GameMode < 3 Then
                If PGNAutoOutputter.Checked Then PGNExport_Click() Else FENExport_Click()
            End If
            If GameMode = 1 Then InitialiseAISystem()
        ElseIf GameMode = 4 Then
            'Move matches the correct puzzle move, so either play the next move of the puzzle, or flag the puzzle as complete.
            CancelAIPuzzleSearch()
            MainAI.Reconfigure(CurrentFEN, False) 'Recalibrates AI before checking for end states.
            EnforceEndStates(True)
            PuzzleMoveCompleted()
        End If
    End Sub


    'Subroutine which updates the core characteristics of the chess system, after a move has been played (ie: makes move onto board).
    Private Sub CalibrateCoreSystemsForMove(ByVal TempMove As Move, ByVal CanUndoMove As Boolean)
        'Makes move onto board, and resets the correct player's TFTable.
        If PlayerTurn Then
            MasterWInCheck = 0 'Player is no longer in check.
            MakeMove(MasterBoard, TempMove, MasterWCanCastle, MasterWKPos, MasterEnPassant, GeneralOptions(0) = "T")
            SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, SharedAlgorithms.ConvertStringToBitCoor(MasterBKPos), MasterBInCheck, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant))
        Else
            MasterBInCheck = 0 'Player is no longer in check.
            MakeMove(MasterBoard, TempMove, MasterBCanCastle, MasterBKPos, MasterEnPassant, GeneralOptions(0) = "T")
            SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, SharedAlgorithms.ConvertStringToBitCoor(MasterWKPos), MasterWInCheck, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant))
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
            Checkerboard.Refresh()
        End If
        CheckChecker(False)

        'Ends the turn, then calculates the new position's FEN.
        PlayerTurn = Not PlayerTurn
        If CanUndoMove Then PreviousFEN = CurrentFEN
        CurrentFEN = SharedAlgorithms.ConvertToFEN(MasterBoard, MasterWCanCastle, MasterBCanCastle, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant), PlayerTurn)
    End Sub

End Class
