Imports System.IO
Imports System.Runtime
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

'Class that holds all the methods referring to GUI controls in the Chess form (ie: buttons, text boxes, sliders, etc).
Partial Public Class Chess

    Private OrientForWhite As Boolean = True 'Represents which way the board is flipped.

    'Below are the variables used in the Board Edit Mode, that form a backup of the current position
    '(in case the user cancels their edit).
    Private BoardEditMode, BoardEditPlayerMove As Boolean
    Private BoardEditBackup(7, 7) As Char
    Private BoardEditInputBackup, BoardEditEnPassant As String
    Private BoardEditWCanCastle As New CanCastle
    Private BoardEditBCanCastle As New CanCastle

    'Sends the FEN a user types in to be converted & displayed.
    Private Sub InputButton_Click() Handles InputButton.Click
        InputTextBox.Text = InputTextBox.Text.Trim(" ")
        If InputTextBox.Text <> "" AndAlso InputTextBox.Text <> CurrentFEN AndAlso Not ComputerIsSearching Then
            'Checks for first form of validation (if the 2nd letter is "o", then that usually implies that 
            'the TextBox contains an error message.
            If InputTextBox.Text.Length > 1 AndAlso InputTextBox.Text(1) <> "o" Then
                If ClickMoveMode Then ClickMoveMode = False : ResetLMS(True)
                'Performs tests on the input to determine whether the user is inputting a FEN or a series of PGN moves.
                If GameRunning AndAlso (InputTextBox.Text.IndexOf("/") = -1 OrElse (InputTextBox.Text.IndexOf(".") > -1 OrElse (InputTextBox.Text.IndexOf(" ") > 1 AndAlso InputTextBox.Text.IndexOf(" ") < 10))) Then
                    'Can assume that input is a move / moves.
                    EnterMovesIntoSystem(InputTextBox.Text, True, True)
                ElseIf FENErrorDetection(InputTextBox.Text, True, "") Then 'Is a Valid FEN.
                    If UndoFENChange.Visible = True Then UndoFENChange.Visible = False

                    Dim TempFEN As String = PreviousFEN
                    'Try displaying the FEN on the board graphically. If this fails, then the FEN is invalid.
                    'In this case, the board is reset to the previous position.
                    Try
                        PreviousFEN = CurrentFEN
                        MasterBoard = SharedAlgorithms.FENConverter(InputTextBox.Text, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
                        DisplayPieces()
                    Catch ex As Exception
                        InvalidInput = InputTextBox.Text
                        PreviousFEN = TempFEN
                        MasterBoard = SharedAlgorithms.FENConverter(CurrentFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
                        DisplayPieces()
                        UndoFENChange.Visible = True
                        InputTextBox.Text = "Position Rejected - Invalid FEN Code. Please Input a Genuinine FEN and try again."
                    End Try

                    If UndoFENChange.Visible = False Then AcceptFENIntoSystem(InputTextBox.Text, True)
                End If
            End If
        End If
    End Sub

    Private Sub AcceptFENIntoSystem(ByVal FEN As String, ByVal FlipBoardForPlayer As Boolean)
        CurrentFEN = FEN
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
        If FlipBoardForPlayer AndAlso (PlayerTurn Xor OrientForWhite) Then
            FlipBoard()
        Else
            Checkerboard.Refresh()
        End If
        GameRunning = True
        'Resets TrueFalse Tables, then checks for Checks.
        'Resets Check and Castling Properties.
        MasterWInCheck = 0
        MasterBInCheck = 0
        SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, SharedAlgorithms.ConvertStringToBitCoor(MasterWKPos), MasterWInCheck, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant))
        SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, SharedAlgorithms.ConvertStringToBitCoor(MasterBKPos), MasterBInCheck, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant))
        'Final logical detection of invalid board positions.
        If Not CheckForInvalidGameStates() AndAlso GeneralOptions(0) = "T" Then Sound_Move.Play()
        CheckChecker(False)

        'Recalibrates AI and resets AI move info.
        MainAI.Reconfigure(CurrentFEN, True)
        CurrentDepth = 2
        CurrentMove = "-"
        CurrentEvaluation = "-"
        CurrentAIDepth.Text = "Current Depth: -"
        CurrentAIMove.Text = "Current Move: -"
        CurrentAIEval.Text = "Evaluation: -"

        'Resets BoardHistory (as the position is new), then check for end states in the position.
        BoardHistory.Clear()
        EnforceEndStates(False)
        OutputDebugInfo()
    End Sub

    'Button which resets the FEN in the InputTextBox, in case it is invalid.
    Private Sub UndoFENChange_Click(sender As Object, e As EventArgs) Handles UndoFENChange.Click
        InputTextBox.Text = InvalidInput
        sender.Visible = False
    End Sub

    'Resets the board to the starting position and sets white to move.
    Private Sub Reset_Btn_Click() Handles Reset_Btn.Click
        If Not (ComputerIsSearching OrElse (CurrentFEN = StartingFEN AndAlso GameRunning)) Then
            If ClickMoveMode Then ClickMoveMode = False : ResetLMS(True)
            PreviousFEN = CurrentFEN
            CurrentFEN = StartingFEN
            'Resets Check Properties.
            MasterWInCheck = 0
            MasterBInCheck = 0
            'Can assume that the StartingFEN is valid, so we display it graphically.
            MasterBoard = SharedAlgorithms.FENConverter(CurrentFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
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
                SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, SharedAlgorithms.ConvertStringToBitCoor(MasterWKPos), MasterWInCheck, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant))
            Else
                SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, SharedAlgorithms.ConvertStringToBitCoor(MasterBKPos), MasterBInCheck, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant))
            End If
            DisplayPieces()
            CheckChecker(False)
            If (PlayerTurn Xor OrientForWhite) AndAlso Not (GameMode = 1 AndAlso (UserPlayer Xor Not OrientForWhite)) Then FlipBoard()
            GameRunning = True
            If GameMode < 3 Then
                InputTextBox.Text = StartingFEN
                InputTextBox.Select(InputTextBox.Text.Length, 0) 'Stops the TextBox from being highlighted.
            End If

            'Recalibrates AI and resets AI move info.
            MainAI.Reconfigure(CurrentFEN, True)
            CurrentDepth = 2
            CurrentMove = "-"
            CurrentEvaluation = "-"
            CurrentAIDepth.Text = "Current Depth: -"
            CurrentAIMove.Text = "Current Move: -"
            CurrentAIEval.Text = "Evaluation: -"

            'Resets BoardHistory (as the position is new), then check for end states in the position.
            BoardHistory.Clear()
            Console.Clear()
            EnforceEndStates(False)
            OutputDebugInfo()
            If GeneralOptions(0) = "T" Then Sound_Move.Play()
        End If
    End Sub

    'Outputs the current FEN into the input textbox.
    Private Sub FENExport_Click() Handles FENExport.Click
        If CurrentFEN <> "" Then
            Dim TempFEN As String = CurrentFEN
            'Trims TempFEN to remove its fullmove number, then appends the size of the BoardHistory array
            '(representing how many moves have been made in the position).
            For n = TempFEN.Length - 2 To 0 Step -1
                If TempFEN(n) = " " Then
                    TempFEN = TempFEN.Substring(0, n + 1)
                    Exit For
                End If
            Next
            TempFEN &= (BoardHistory.GetSize() + 1) \ 2
            InputTextBox.Text = TempFEN
        End If
    End Sub

    'Outputs the current PGN into the input textbox.
    Private Sub PGNExport_Click() Handles PGNExport.Click
        Dim PGN As String = BoardHistory.GetFormattedPGNString(Not GameRunning)
        If PGN <> "" Then
            'Some moves exist - output to the input box.
            InputTextBox.Text = PGN
            'Selects the last character in the PGN, then scrolls to that character (so that the last moves
            'can always be seen by the user).
            InputTextBox.SelectionStart = (InputTextBox.Text).Length
            InputTextBox.ScrollToCaret()
        End If
    End Sub

    'Converts the board to the previous FEN Position.
    Private Sub UndoMove_Click() Handles UndoMove.Click
        'If CurrentFEN = PreviousFEN then it is implied that it is the starting position.
        'The user should not be able to undo in this circumstance.
        If Not ComputerIsSearching AndAlso Not (CurrentFEN = PreviousFEN) Then
            If ClickMoveMode Then ClickMoveMode = False : ResetLMS(True)
            If AIIsSearchingOnUsersTurn Then MainAI.ABORTSearch()
            Dim TempSH(1, 1) As SByte 'Temp Square History array.
            Dim TempFEN As String = CurrentFEN
            CurrentFEN = PreviousFEN
            PreviousFEN = TempFEN
            MasterWInCheck = 0
            MasterBInCheck = 0
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
                SharedAlgorithms.FixTFTables(MasterBoard, True, MasterWhiteTFTable, SharedAlgorithms.ConvertStringToBitCoor(MasterWKPos), MasterWInCheck, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant))
            Else
                SharedAlgorithms.FixTFTables(MasterBoard, False, MasterBlackTFTable, SharedAlgorithms.ConvertStringToBitCoor(MasterBKPos), MasterBInCheck, SharedAlgorithms.ConvertStringToBitCoor(MasterEnPassant))
            End If
            If GeneralOptions(0) = "T" Then Sound_Move.Play()
            CheckChecker(False)
            'Final logical detection of invalid board positions.
            If Not CheckForInvalidGameStates() AndAlso GeneralOptions(0) = "T" Then Sound_Move.Play()

            If GameMode < 3 Then
                If PGNAutoOutputter.Checked Then PGNExport_Click() Else FENExport_Click()
            End If
            'Recalibrates AI, then checks for end positions.
            If AIIsSearchingOnUsersTurn Then Thread.Sleep(5) : SearchSettings.OutputToConsole = True : AIIsSearchingOnUsersTurn = False
            MainAI.Reconfigure(CurrentFEN, False)
            BoardHistory.Swap()
            EnforceEndStates(False)
            OutputDebugInfo()
        End If
    End Sub




    'Method that enables the board editor: allowing the user to drag pieces onto the board to create a custom position.
    'This button also acts as the Save button, reverting all the user's changes and resetting the board back to its previous position.
    Private Sub BoardEditorBtn_Click() Handles BoardEditorBtn.Click
        Dim FENErrorMessage As String = ""
        If Not ComputerIsSearching Then
            If ClickMoveMode Then ClickMoveMode = False : ResetLMS(True)

            If Not BoardEditMode Then
                'Creates a backup of the baord, then erases it (new position containing only kings in their starting positions).
                Array.Copy(MasterBoard, BoardEditBackup, 64)
                For y = 0 To 7
                    For x = 0 To 7
                        MasterBoard(x, y) = " "
                    Next
                Next
                MasterBoard(4, 0) = "k"
                MasterBoard(4, 7) = "K"

                'Clears check information, then calibrates the GUI ready for BoardEditMode.
                If MasterWInCheck >= 128 Then WK1.Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\WKing.png")
                If MasterBInCheck >= 128 Then BK1.Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\BKing.png")

                DisplayPieces()
                CalibrateBoardEditorObjectHandling()
            Else
                'Attempt to submit the user's FEN into the system. If it is invalid, an appropriate message is displayed.
                If FENErrorDetection(InputTextBox.Text, False, FENErrorMessage) Then
                    MasterBoard = SharedAlgorithms.FENConverter(InputTextBox.Text, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
                    AcceptFENIntoSystem(InputTextBox.Text, False) 'Assume the FEN is valid = submit it into the system.
                    CalibrateBoardEditorObjectHandling() 'Resets the GUI to its original layout.
                Else
                    'Displays the appropriate error message, as stated by the FENErrorDetection Sub.
                    If MsgBox(FENErrorMessage & vbCr & "Please Click 'Retry' to Configure Another Position, or Click 'Cancel' to Return to the Original Position.", vbCritical + vbRetryCancel + vbApplicationModal) <> 4 Then
                        BoardEditCancelBtn_Click()
                    End If
                End If
            End If
        End If
    End Sub

    'Subroutine that acts as the 'Cancel' Button for the Board Edit Mode.
    Private Sub BoardEditCancelBtn_Click() Handles BoardEditCancelBtn.Click
        CalibrateBoardEditorObjectHandling()
        'Reconstructs MasterBoard from its backup, and recalibrates the GUI & its current position.
        Array.Copy(BoardEditBackup, MasterBoard, 64)
        If GeneralOptions(4) = "T" Then
            If MasterWInCheck >= 128 Then WK1.Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\WKingCheck.png")
            If MasterBInCheck >= 128 Then BK1.Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\BKingCheck.png")
        End If
        DisplayPieces()
    End Sub

    'Method that handles the core GUI changes due to the Board Edit Mode.
    Private Sub CalibrateBoardEditorObjectHandling()
        '{Removes & Disables}, or {Shows & Enables} all elements of the GUI that are not strictly related to Board Edit Mode.
        Dim ObjectsToRemove() As Object = {CheckLabel, ProgressBar, AIMoveBtn, CurrentAIDepth, CurrentAIMove,
    CurrentAIEval, UserTimeBar, UserTimeBox, QuiescenceBox, PieceHeatMapBox, UseBook, AIEndlessMode}
        Dim ObjectsToDisable() As Object = {Reset_Btn, InputButton, FENExport, PGNExport, UndoMove, NodeTestBtn}
        For Each Item In ObjectsToRemove
            Item.Visible = BoardEditMode
        Next
        If AIEndlessMode.Checked Then AutoResetter.Visible = BoardEditMode
        For Each Item In ObjectsToDisable
            Item.Enabled = BoardEditMode
        Next

        BoardEditMode = Not BoardEditMode
        BoardEditPanel.Visible = BoardEditMode

        If BoardEditMode Then
            'Shows the Board Edit Buttons (Save & Cancel Changes), along with all the core controls to this mode.
            BoardEditorBtn.Width = 90
            BoardEditCancelBtn.Visible = True
            BoardEditorBtn.Text = "Save Changes:"
            Checkerboard.Width += 225 'We increase the Width of Checkerboard, as we will be needing to place the template pieces to it's right.
            ExitBtn.Top += 15

            BoardEditInputBackup = InputTextBox.Text
            BoardEditWhiteMove.Checked = True
            BoardEditWKSBox.Checked = True
            BoardEditWQSBox.Checked = True
            BoardEditBKSBox.Checked = True
            BoardEditBQSBox.Checked = True

            'Resets the backup board attributes for the base position set in Board Edit Mode.
            BoardEditPlayerMove = True
            BoardEditWCanCastle.CanCastle()
            BoardEditBCanCastle.CanCastle()
            BoardEditEnPassant = "-"

            InputTextBox.Text = "5k3/8/8/8/8/8/8/5K3 w KQkq - 0 1"

            'Adds template pieces (one of each type) to the right of Checkerboard - allowing the user to drag pieces onto the board.
            Dim NewPiece As PictureBox
            Dim PieceTypeArray() As Char = {"Q", "R", "N", "B", "P"}
            For n = 0 To PieceTypeArray.Length - 1
                NewPiece = FindNewPiece("W", PieceTypeArray(n))
                NewPiece.Location = New Point(650 + (n Mod 2) * 100, 142 + (n \ 2) * 75)
                NewPiece.Visible = True
            Next
            PieceTypeArray = {"P", "N", "B", "Q", "R"}
            For n = 0 To PieceTypeArray.Length - 1
                NewPiece = FindNewPiece("B", PieceTypeArray(n))
                NewPiece.Location = New Point(650 + ((n + 1) Mod 2) * 100, 292 + ((n + 1) \ 2) * 75)
                NewPiece.Visible = True
            Next
        Else
            'Removes all Board Edit Mode GUI objects, and resets the checkerboard to its normal size.
            BoardEditorBtn.Width = 194
            BoardEditCancelBtn.Visible = False
            BoardEditorBtn.Text = "Board Editor Mode:"
            Checkerboard.Width -= 225
            ExitBtn.Top -= 15
            InputTextBox.Text = BoardEditInputBackup
            ResetLMS(False)

            'Removes all template pieces.
            For Each Box In PieceArray
                If Box.Location.X > 600 Then
                    Box.Location = New Point(-100, -100)
                    Box.Visible = False
                End If
            Next
        End If

        Checkerboard.Refresh()
    End Sub



    'Below are all the handles for editing with the board, its pieces, and its parameters, for the Board Edit Mode.
    Private Sub HandleBoardEditModeDrop(ByVal Piece As PictureBox, ByVal SnapPieceLocation As Point, ByVal OldPosition As Point)
        'Subroutine for handling a piece being dropped onto the board (or off the board).
        Dim CoorX As SByte = SnapPieceLocation.X / 75
        Dim CoorY As SByte = SnapPieceLocation.Y / 75
        If Not OrientForWhite Then CoorX = 7 - CoorX : CoorY = 7 - CoorY

        'If the piece is not within the confines of the board, remove it (unless it is a king, which cannot be removed).
        If CoorX >= 0 AndAlso CoorY >= 0 AndAlso CoorX < 8 AndAlso CoorY < 8 Then

            If Piece.Name(1) = "P" AndAlso (CoorY = 0 OrElse CoorY = 7) Then
                'The user is attempting to place a pawn on the 1st or 8th row (which is illegal) - move the pawn back.
                Piece.Location = OldPosition
                Exit Sub
            End If

            If MasterBoard(CoorX, CoorY) <> " " Then

                If UCase(MasterBoard(CoorX, CoorY)) = "K" Then
                    'The user is attempting to capture a king - move the attacking piece back.
                    Piece.Location = OldPosition
                    Exit Sub
                Else
                    'The capture move is legit - remove the captured piece from the board.
                    For Each Box In PieceArray
                        If Box.Location = SnapPieceLocation AndAlso Box.Name <> Piece.Name Then
                            Box.Visible = False
                            Box.Location = New Point(-100, -100)
                            Exit For
                        End If
                    Next
                End If

            End If
            'Snap the moved piece at its new location, then save this to MasterBoard.
            Piece.Location = SnapPieceLocation
            If OldPosition.X < 600 Then MasterBoard(OldPosition.X / 75, OldPosition.Y / 75) = " "
            MasterBoard(CoorX, CoorY) = If(Piece.Name(0) = "W", Piece.Name(1), LCase(Piece.Name(1)))

            'Checks to see if the En-Passant Square is still legal (eg: the capturing pawn may have been captured).
            'If not, remove the En-Passant Square.
            If BoardEditEnPassant <> "-" AndAlso Not SharedAlgorithms.CheckEnPassantSquareIsLegal(MasterBoard, BoardEditEnPassant, BoardEditPlayerMove) Then
                BoardEditEnPassant = "-"
                ResetLMS(True)
            End If
            'Puts the user's custom position's FEN into the FEN box, so that the user can copy it easily.
            InputTextBox.Text = SharedAlgorithms.ConvertToFEN(MasterBoard, BoardEditWCanCastle, BoardEditBCanCastle, SharedAlgorithms.ConvertStringToBitCoor(BoardEditEnPassant), BoardEditPlayerMove)

        ElseIf Piece.Name(1) = "K" Then
            Piece.Location = OldPosition
        Else
            'The piece has left the confines of the board - remove it.
            Piece.Visible = False
            Piece.Location = New Point(-100, -100)
            If OldPosition.X < 600 Then MasterBoard(OldPosition.X / 75, OldPosition.Y / 75) = " " : InputTextBox.Text = SharedAlgorithms.ConvertToFEN(MasterBoard, BoardEditWCanCastle, BoardEditBCanCastle, SharedAlgorithms.ConvertStringToBitCoor(BoardEditEnPassant), BoardEditPlayerMove)
        End If

        Checkerboard.Refresh()
    End Sub

    'Handle which allows the user to copy pieces to quickly place them across the board, if they are holding the CTRL key.
    Private Sub HandleBoardEditModeCopy(ByVal Piece As PictureBox)
        Dim CoorX As SByte = Piece.Location.X / 75
        Dim CoorY As SByte = Piece.Location.Y / 75
        If Not OrientForWhite Then CoorX = 7 - CoorX : CoorY = 7 - CoorY

        'Only allow the user to copy their piece if it is within the confines of the board, and if they are not trying to copy a piece over a king.
        If (CoorX >= 0 AndAlso CoorY >= 0 AndAlso CoorX < 8 AndAlso CoorY < 8) AndAlso Piece.Name(1) <> "K" Then
            Dim PieceSymbol As Char = If(Piece.Name(0) = "W", Piece.Name(1), LCase(Piece.Name(1)))
            If MasterBoard(CoorX, CoorY) = " " OrElse Not (MasterBoard(CoorX, CoorY) = PieceSymbol OrElse UCase(MasterBoard(CoorX, CoorY)) = "K") Then
                'Stops the user from placing pawns on invalid ranks (rank 1 & rank 8).
                If Not (Piece.Name(1) = "P" AndAlso (CoorY = 0 OrElse CoorY = 7)) Then
                    'Creates a new cloned piece, based on the template piece that the user is holding.
                    Dim ReplacedPiece As PictureBox = FindNewPiece(Piece.Name(0), Piece.Name(1))
                    ReplacedPiece.Location = CalculatePieceSquareSnapPosition(Piece)
                    ReplacedPiece.Visible = True

                    'If there is another piece present at that location, remove it.
                    If MasterBoard(CoorX, CoorY) <> " " Then
                        For Each Box In PieceArray
                            If Box.Location = ReplacedPiece.Location AndAlso Box.Name <> ReplacedPiece.Name Then
                                Box.Visible = False
                                Box.Location = New Point(-100, -100)
                                Exit For
                            End If
                        Next
                    End If

                    MasterBoard(CoorX, CoorY) = PieceSymbol
                    'Checks to see if EnPassant is still legal. If not, remove it.
                    If BoardEditEnPassant <> "-" AndAlso Not SharedAlgorithms.CheckEnPassantSquareIsLegal(MasterBoard, BoardEditEnPassant, BoardEditPlayerMove) Then BoardEditEnPassant = "-" : ResetLMS(True)
                    'Puts the user's custom position's FEN into the FEN box.
                    InputTextBox.Text = SharedAlgorithms.ConvertToFEN(MasterBoard, BoardEditWCanCastle, BoardEditBCanCastle, SharedAlgorithms.ConvertStringToBitCoor(BoardEditEnPassant), BoardEditPlayerMove)
                End If
            End If
        End If
    End Sub


    'Handle which change the parameters of the board (such as White / Black to move & Castling info).
    Private Sub HandleBoardEditParameterChange(sender As Object, e As EventArgs) Handles BoardEditWhiteMove.CheckedChanged, BoardEditBlackMove.CheckedChanged, BoardEditWKSBox.CheckedChanged, BoardEditWQSBox.CheckedChanged, BoardEditBKSBox.CheckedChanged, BoardEditBQSBox.CheckedChanged
        'Enable / Disable the BoardEdit board info, depending on the check state of the GUI parameters.
        Select Case sender.Name
            Case "BoardEditWhiteMove", "BoardEditBlackMove"
                BoardEditPlayerMove = BoardEditWhiteMove.Checked
            Case "BoardEditWKSBox"
                BoardEditWCanCastle.KS = BoardEditWKSBox.Checked
            Case "BoardEditWQSBox"
                BoardEditWCanCastle.QS = BoardEditWQSBox.Checked
            Case "BoardEditBKSBox"
                BoardEditBCanCastle.KS = BoardEditBKSBox.Checked
            Case "BoardEditBQSBox"
                BoardEditBCanCastle.QS = BoardEditBQSBox.Checked
        End Select

        'Updates the user's current FEN onto the FEN text box, to represent these updated parameters.
        InputTextBox.Text = SharedAlgorithms.ConvertToFEN(MasterBoard, BoardEditWCanCastle, BoardEditBCanCastle, SharedAlgorithms.ConvertStringToBitCoor(BoardEditEnPassant), BoardEditPlayerMove)
    End Sub

    'Handle which allows the user to specify the location of the En-Passant square on the board. Contains error handling, so that the user
    'can only place the En-Passant square in a valid position.
    Private Sub HandleBoardEditEnPassantClick(ByVal XCoor As SByte, ByVal YCoor As SByte)
        If SharedAlgorithms.CheckEnPassantSquareIsLegal(MasterBoard, XCoor & YCoor, BoardEditPlayerMove) Then
            'Assume the square is correct - edit the Board Edit parameters, then display this En-Passant square on the GUI (using a grey rectangle around this square).
            BoardEditEnPassant = XCoor & YCoor
            ResetLMS(False)
            'Highlights the En-Passant square on LMS.
            If OrientForWhite Then
                LegalMoveSquares(XCoor, YCoor) = True
            Else
                LegalMoveSquares(7 - XCoor, 7 - YCoor) = True
            End If
        Else
            'Resets the EnPassant information.
            BoardEditEnPassant = "-"
            ResetLMS(False)
        End If
        Checkerboard.Refresh()
        'Updates the user's FEN into the FEN box.
        InputTextBox.Text = SharedAlgorithms.ConvertToFEN(MasterBoard, BoardEditWCanCastle, BoardEditBCanCastle, SharedAlgorithms.ConvertStringToBitCoor(BoardEditEnPassant), BoardEditPlayerMove)
    End Sub




    'Subroutine which rotates the board 180 degreees in favour of the other player.
    Private Sub FlipperButton_Click() Handles FlipperButton.Click
        FlipBoard()
        OutputDebugInfo()
    End Sub
    Private Sub FlipBoard()
        If ClickMoveMode Then ClickMoveMode = False : ResetLMS(False)
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
        If GameMode >= 5 Then
            'Toggles which training game leaderboard is shown, and stop the visible one from being highlighted.
            If OrientForWhite Then
                BLeaderBoardGrid.Visible = False
                WLeaderBoardGrid.Visible = True
                Me.WLeaderBoardGrid.ClearSelection()
            Else
                WLeaderBoardGrid.Visible = False
                BLeaderBoardGrid.Visible = True
                Me.BLeaderBoardGrid.ClearSelection()
            End If
        End If
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
        If (SearchSettings.UseQuiescence AndAlso UserTimeBar.Value < 4) OrElse Not SearchSettings.UseQuiescence AndAlso UserTimeBar.Value < 2 Then
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
                    Temp = InputBox("Please input how long you want the AI to search for (in seconds):" & vbCrLf & vbCrLf & "Min Time = 0.1s, Max Time = 600s." & vbCrLf & "0 = Infinity (Indefinite Search)", "Time Inputter")
                    If Temp = "" OrElse Temp = " " Then 'Cancel / Exit button was pressed - abort.
                        UserTime = TimeForSearch
                        Exit While
                    End If
                    UserTime = Math.Round(CDec(Temp), 1)
                    If UserTime = 0 OrElse (UserTime >= 0.1 AndAlso UserTime <= 600) Then
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

            If UserTime = 0 Then UserTime = Decimal.MaxValue
            'Edits GUI objects, and updates TimeForSearch.
            UserTimeBox.Select(UserTimeBox.Text.Length, 0)
            UserTimeBar.Value = 2 * Math.Round(Math.Min(UserTime, 30))
            If UserTime = Decimal.MaxValue Then
                UserTimeBox.Text = "Time For Search: Infinity."
            Else
                UserTimeBox.Text = "Time For Search: " & UserTime & " seconds."
            End If
            TimeForSearch = UserTime
        End If
        Me.ActiveControl = Nothing 'Stops the cursor from appearing in the box.
    End Sub

    'Subroutines that control the Quiescence and Piece Heat Map Boxes.
    Private Sub QuiescenceBox_CheckedChanged() Handles QuiescenceBox.CheckedChanged
        'Toggles Quiescence depending on the state of the box.
        If QuiescenceBox.Checked Then SearchSettings.UseQuiescence = True : Else SearchSettings.UseQuiescence = False
        'Colours UserTimeBox red if the TimeForSearch setting the user has chosen is likely to not be enough time for the AI
        'to compelete a search in.
        If (SearchSettings.UseQuiescence AndAlso UserTimeBar.Value < 4) OrElse Not SearchSettings.UseQuiescence AndAlso UserTimeBar.Value < 2 Then
            UserTimeBox.BackColor = Color.LightCoral
        Else
            UserTimeBox.BackColor = Color.WhiteSmoke
        End If
    End Sub
    Private Sub PieceHeatMapBox_CheckedChanged() Handles PieceHeatMapBox.CheckedChanged
        'Toggles Piece Heat Maps depending on the state of the box.
        If PieceHeatMapBox.Checked Then SearchSettings.UsePieceHeatMaps = True : Else SearchSettings.UsePieceHeatMaps = False
    End Sub

    'Subroutine that terminates the AI search, and displays on the board the AI's current move.
    Private Sub AITerminator_Click() Handles AITerminator.Click
        TerminateSearch = True
    End Sub

    'Subroutine that toggles the 'Reset Game Automatically' checkbox.
    Private Sub AIEndlessMode_CheckedChanged() Handles AIEndlessMode.CheckedChanged
        AutoResetter.Visible = AIEndlessMode.Checked
    End Sub


    Private Sub AutoOutputter_CheckedChanged() Handles PGNAutoOutputter.CheckedChanged, FENAutoOutputter.CheckedChanged
        If PGNAutoOutputter.Checked Then
            PGNExport_Click()
        Else
            FENExport_Click()
        End If
    End Sub



    'Subroutines which handle the Node Test GUI buttons.
    Private Sub NodeTestBtn_Click() Handles NodeTestBtn.Click
        If GameRunning AndAlso Not ComputerIsSearching Then
            If ClickMoveMode Then ClickMoveMode = False : ResetLMS(True)
            'Creates a new thread for the Node Test to run on, then starts the search.
            ComputerIsSearching = True
            Dim NodeTestThread As New Task(AddressOf InstantiateNodeTest)
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency 'Relaxes garbage collection during the NegaMax search.
            NodeTestThread.Start()
            NodeTestStopBtn.Visible = True
        End If
    End Sub
    'Subroutine which handles the Node Test Thread (enables multithreading).
    Private Sub InstantiateNodeTest()
        For n = 1 To 99
            MainAI.PerformNodeTestOnPosition(n)
            If MainAI.GetABORTState() Then Exit For 'Stops future depths from running if there is an error.
        Next
    End Sub
    Private Sub NodeTestStopBtn_Click() Handles NodeTestStopBtn.Click
        NodeTestStopBtn.Visible = False
        'Stops the Node Test, then displays an appropriate message.
        MainAI.ABORTSearch()
        ComputerIsSearching = False
        GCSettings.LatencyMode = GCLatencyMode.Interactive 'Resets garbage collection mode.
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine(("Node Test Terminated (by user).").PadRight(48) & vbCrLf)
    End Sub




    'Subroutine that reveals the settings menu to the user.
    Private Sub SettingsBtn_Click() Handles SettingsBtn.Click
        'Creates and reveals the Settings Form.
        If ClickMoveMode Then ClickMoveMode = False : ResetLMS(True)
        Dim CurrentSettings As Form = CType(Application.OpenForms("Settings"), Settings)
        If CurrentSettings Is Nothing Then 'If there are no other Settings forms open.
            Dim SettingsMenu As New Settings(GameMode <= 3, GameMode < 3, GameMode > 1 AndAlso GameMode <= 3)
            SettingsMenu.Show()
        End If
    End Sub

    'Button that returns the user back to the Main Menu. 
    Private Sub ExitBtn_Click() Handles ExitBtn.Click
        If MainAI IsNot Nothing Then AICanSearchOnUsersTurn = False : MainAI.ABORTSearch()
        If GameMode = 4 Then PuzzleSampleDatabase = Nothing 'Clears the puzzle database to save memory.

        'Locates MainMenu Form and shows it.
        Dim Menu As Form = CType(Application.OpenForms("MainMenu"), MainMenu)
        'Closes the settings form, if it exists.
        Dim Settings As Form = CType(Application.OpenForms("Settings"), Settings)
        If Settings IsNot Nothing Then Settings.Close()

        'Removes the TranspositionTable, then calls a full garbage collection on the system, in order to
        'properly reset the system for the next game of chess.
        If MainAI IsNot Nothing Then Thread.Sleep(5) : MainAI.DisposeTranspositionTable()
        GC.Collect()

        Me.Close() 'Closes the current form.
        Menu.Show()
    End Sub

    'Button that displays the credits information onto the screen (in the form of a pop-up).
    Private Sub Credits_Click() Handles Credits.Click
        Dim CanRetrieveStats As Boolean
        'Loads the Lifetime stats file, then assigns each line to their appropriate variable.
        Dim LifetimePositions, LifetimeTranspositions, LifetimeCheckmates As UInt64
        Try
            Using SR As New StreamReader(Application.StartupPath & "\Assets\User\AIStats.txt", Encoding.UTF8, True)
                LifetimePositions = SR.ReadLine()
                LifetimeTranspositions = SR.ReadLine()
                LifetimeCheckmates = SR.ReadLine()
            End Using
            CanRetrieveStats = True
        Catch ex As Exception
            'An error occured when reading from the file.
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Unable to retrieve AI lifetime stats.")
            Console.ResetColor()
        End Try

        Dim CreditsMessage As String = Strings.StrDup(10, " ") & "Chess Game & Artificial Intelligence (" & GlobalConstants.ProgramVersion & ")" & vbCrLf & Strings.StrDup(21, " ") & "Created by Alfie Kunz (8158)" & vbCrLf & Strings.StrDup(22, " ") & "of Beckfoot School (37101)" & vbCrLf & "Project used for the AQA GCE Computer Science NEA" & vbCrLf & Strings.StrDup(35, " ") & "(2021 - 2024)"
        If CanRetrieveStats Then
            MsgBox(CreditsMessage & vbCrLf & vbCrLf & vbCrLf & "Lifetime AI Statistics:" & vbCrLf & "Positions Searched: " & LifetimePositions.ToString("N0") & vbCrLf & "Transpositions Found: " & LifetimeTranspositions.ToString("N0") & vbCrLf & "Checkmates Made: " & LifetimeCheckmates.ToString("N0"), vbInformation, "Credits")
        Else
            MsgBox(CreditsMessage, vbInformation, "Credits")
        End If

    End Sub

End Class
