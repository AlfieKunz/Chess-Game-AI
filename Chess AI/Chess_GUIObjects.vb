Imports System.IO
Imports System.Net.WebRequestMethods
Imports System.Reflection
Imports System.Runtime
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

'Class that holds all the methods referring to GUI controls in the Chess form (ie: buttons, text boxes, sliders, etc).
Partial Public Class Chess 'GUI Objects

    Private OrientForWhite As Boolean = True 'Represents which way the board is flipped.

    'Below are the variables used in the Board Edit Mode, that form a backup of the current position
    '(in case the user cancels their edit).
    Public Structure BoardEditInfo
        Public isEnabled, PlayerMove As Boolean
        Public BoardBackup(,) As Char
        Public InputBackup, EnPassant As String
        Public WCanCastle As CanCastle
        Public BCanCastle As CanCastle
        Public CTRLKeyDown As Boolean
    End Structure
    Private BoardEdit As New BoardEditInfo With {
        .BoardBackup = New Char(7, 7) {},
        .WCanCastle = New CanCastle,
        .BCanCastle = New CanCastle
    }
    'Notifies if the system is modifying the Settings in the AISettingsPanel, so that they do not recursively update the global settings.
    Private UserCanChangeAISettings As Boolean

    'Sends the FEN a user types in to be converted & displayed.
    Private Sub InputButton_Click() Handles InputButton.Click
        InputTextBox.Text = InputTextBox.Text.Trim(" ")
        If Not (InputTextBox.Text = "" OrElse InputTextBox.Text = CurrentFEN OrElse ComputerIsSearching OrElse PieceIsMoving) Then
            'Checks for first form of validation (if the 2nd letter is "o", then that usually implies that 
            'the TextBox contains an error message.
            If InputTextBox.Text.Length > 1 AndAlso InputTextBox.Text(1) <> "o" Then
                If ClickMoveMode Then ClickMoveMode = False : ResetLMS(True)

                'Looks to see if the input is hybrid, in which we mean it contains an initial FEN, and then a set of moves stemming from that position.
                Dim RegexMatch As Match = Regex.Match(InputTextBox.Text, "\[FEN\s+""([^""]*)""\]")
                If RegexMatch.Success Then
                    If FENErrorDetection(RegexMatch.Groups(1).Value, True, "") Then
                        GUIFENInputHandle(RegexMatch.Groups(1).Value)
                        If GameRunning Then EnterMovesIntoSystem(InputTextBox.Text, True, True)
                        Exit Sub
                    End If
                End If
                'Performs tests on the input to determine whether the user is inputting a FEN or a series of PGN moves.
                If GameRunning AndAlso (InputTextBox.Text.IndexOf("/") = -1 OrElse (InputTextBox.Text.IndexOf(".") > -1 OrElse (InputTextBox.Text.IndexOf(" ") > 1 AndAlso InputTextBox.Text.IndexOf(" ") < 10))) Then
                    'Can assume that input is a move / moves.
                    EnterMovesIntoSystem(InputTextBox.Text, True, True)
                ElseIf FENErrorDetection(InputTextBox.Text, True, "") Then 'Is a Valid FEN.
                    GUIFENInputHandle(InputTextBox.Text)
                End If

            End If
        End If
    End Sub
    Private Sub GUIFENInputHandle(ByVal FEN As String)
        'Appends extra information to the end of the FEN (eg: "8/8/8/8/8/8/8/8" --> "8/8/8/8/8/8/8/8 w KQkq - 0 1"),
        'and corrects the order of this extra data, if needed.
        Dim FENInfo As List(Of String) = FEN.Trim(" "c).Split(" "c).ToList()
        'Splits all the info into a list, then assignes the pieces part of the FEN.
        Dim FormattedFEN As String = FENInfo(0)
        FENInfo.RemoveAt(0)
        Dim ItemFound As Boolean
        Dim EnPassantInfo As String = "-"

        'Locates player to move in FENInfo, and adds it to the FEN.
        ItemFound = False
        For Each Info In FENInfo
            If Info = "w"c OrElse Info = "b"c Then
                'Info found!
                FormattedFEN &= " " & Info
                FENInfo.Remove(Info)
                ItemFound = True
                Exit For
            End If
        Next
        If Not ItemFound Then FormattedFEN &= " w" 'Default to white.

        'Locates En Passant data in FENInfo. Note that we do this before castling rules (and add the data to FEN later), as we want a single "-"
        'in the user's FEN to default to remove castling rules, rather than clearing En Passant (less common).
        ItemFound = False
        For Each Info In FENInfo
            If Info(0) >= "a" AndAlso Info(0) <= "h" Then
                'Info found!
                EnPassantInfo = Info
                FENInfo.Remove(Info)
                ItemFound = True
                Exit For
            End If
        Next
        FENInfo.Remove(EnPassantInfo) 'Defaults to "-".

        'Locates castling info in FENInfo, and adds it to the FEN.
        ItemFound = False
        For Each Info In FENInfo
            If UCase(Info(0)) = "K"c OrElse UCase(Info(0)) = "Q"c OrElse Info = "-" Then
                'Info found!
                FormattedFEN &= " " & Info
                FENInfo.Remove(Info)
                ItemFound = True
                Exit For
            End If
        Next
        If Not ItemFound Then FormattedFEN &= " KQkq" : FENInfo.Remove("-")
        FormattedFEN &= " " & EnPassantInfo 'Adds En Passant info.

        'Locates half-move round in FENInfo (ie: locate first numerical value), and adds it to the FEN.
        ItemFound = False
        For Each Info In FENInfo
            If Integer.TryParse(Info, 0) Then
                'Info found!
                FormattedFEN &= " " & Info
                FENInfo.Remove(Info)
                ItemFound = True
                Exit For
            End If
        Next
        If Not ItemFound Then FormattedFEN &= " 0"

        'Locates full-move round in FENInfo (ie: locate second numerical value, if it exists), and adds it to the FEN.
        ItemFound = False
        Dim ResultNum As Integer 'Make sure that the full move counter is always greater than or equal to 1.
        For Each Info In FENInfo
            If Integer.TryParse(Info, ResultNum) AndAlso ResultNum > 0 Then
                'Info found!
                FormattedFEN &= " " & Info
                FENInfo.Remove(Info)
                ItemFound = True
                Exit For
            End If
        Next
        If Not ItemFound Then FormattedFEN &= " 1"


        'Send the FEN into the system.
        If UndoFENChange.Visible = True Then UndoFENChange.Visible = False
        Dim FENErrorMessage As Exception = AcceptFENIntoSystem(FormattedFEN, False)
        If FENErrorMessage IsNot Nothing Then
            'There was an error displaying the FEN - write an error message.
            InvalidInput = InputTextBox.Text
            UndoFENChange.Visible = True
            InputTextBox.Text = "Position Rejected - Invalid FEN Code (" & FENErrorMessage.Message.TrimEnd("."c) & "). Please Input a Genuinine FEN and try again."
        End If
    End Sub

    Private Function AcceptFENIntoSystem(ByVal FEN As String, ByVal FlipBoardForPlayer As Boolean, Optional ByVal NeedAnimate As Boolean = True) As Exception
        Dim TempFEN As String = PreviousFEN
        'Try displaying the FEN on the board graphically. If this fails, then the FEN is invalid.
        'In this case, the board is reset to the previous position.
        Try
            PreviousFEN = CurrentFEN
            MasterBoard = Helper.FENConverter(FEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
            If NeedAnimate Then AnimateBoard(PreviousFEN)
        Catch ex As Exception
            PreviousFEN = TempFEN
            MasterBoard = Helper.FENConverter(CurrentFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
            DisplayPieces()
            Return ex 'Returns debug values to be written to either the screen or the console.
        End Try

        CurrentFEN = FEN
        'Console.Clear()
        'Edits location of Previously Used Squares.
        SquareHistory(2, 0) = SquareHistory(0, 0)
        SquareHistory(2, 1) = SquareHistory(0, 1)
        SquareHistory(3, 0) = SquareHistory(1, 0)
        SquareHistory(3, 1) = SquareHistory(1, 1)
        SquareHistory(0, 0) = -1
        SquareHistory(0, 1) = -1
        SquareHistory(1, 0) = -1
        SquareHistory(1, 1) = -1
        GameRunning = True

        'Resets TrueFalse Tables, then checks for Checks.
        MasterWInCheck = 0
        MasterBInCheck = 0
        Helper.FixTFTable(MasterBoard, True, MasterWhiteTFTable, Helper.ConvertStringToBitCoor(MasterWKPos), MasterWInCheck, MasterWCanCastle.CanICastle(), Helper.ConvertStringToBitCoor(MasterEnPassant))
        Helper.FixTFTable(MasterBoard, False, MasterBlackTFTable, Helper.ConvertStringToBitCoor(MasterBKPos), MasterBInCheck, MasterBCanCastle.CanICastle(), Helper.ConvertStringToBitCoor(MasterEnPassant))
        If FlipBoardForPlayer AndAlso (PlayerTurn Xor OrientForWhite) Then
            FlipBoard()
        Else
            Checkerboard.Refresh()
        End If

        'Final logical detection of invalid board positions.
        If Not CheckForInvalidGameStates() AndAlso GeneralOptions(0) = "T" Then Sound_Move.Play()
        EditCheckText()

        'Recalibrates AI and resets AI move info.
        MainAI.Reconfigure(CurrentFEN, True)
        AIHandles.CurrentDepth = 2
        AIHandles.CurrentMove = "-"
        AIHandles.CurrentEvaluation = "-"
        CurrentAIDepth.Text = "Current Depth: -"
        CurrentAIMove.Text = "Current Move: -"
        CurrentAIEval.Text = "Evaluation: -"
        AIHandles.FENToResetTo = CurrentFEN

        'Resets BoardHistory (as the position is new), but also ensuring that the 50 move counter is updated.
        BoardHistory.Clear()
        Dim HalfSize As String = FEN.Substring(0, FEN.LastIndexOf(" "))
        HalfSize = HalfSize.Substring(HalfSize.LastIndexOf(" ") + 1)
        BoardHistory.SetHalfSize(HalfSize)
        'Checks for end states in the position.
        EnforceEndStates()
        OutputDebugInfo()
        Return Nothing
    End Function

    'Button which resets the FEN in the InputTextBox, in case it is invalid.
    Private Sub UndoFENChange_Click(sender As Object, e As EventArgs) Handles UndoFENChange.Click
        InputTextBox.Text = InvalidInput
        sender.Visible = False
    End Sub

    'Resets the board to the starting position and sets white to move.
    Private Sub Reset_Btn_Click() Handles Reset_Btn.Click
        If Not (ComputerIsSearching OrElse PieceIsMoving OrElse (CurrentFEN = StartingFEN AndAlso GameRunning)) Then
            If ClickMoveMode Then ClickMoveMode = False : ResetLMS(True)
            PreviousFEN = CurrentFEN
            CurrentFEN = StartingFEN
            'Resets Check Properties.
            MasterWInCheck = 0
            MasterBInCheck = 0
            'Can assume that the StartingFEN is valid, so we display it graphically.
            MasterBoard = Helper.FENConverter(CurrentFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
            'Edits location of Previously Used Squares.
            SquareHistory(2, 0) = SquareHistory(0, 0)
            SquareHistory(2, 1) = SquareHistory(0, 1)
            SquareHistory(3, 0) = SquareHistory(1, 0)
            SquareHistory(3, 1) = SquareHistory(1, 1)
            SquareHistory(0, 0) = -1
            SquareHistory(0, 1) = -1
            SquareHistory(1, 0) = -1
            SquareHistory(1, 1) = -1
            'Resets TrueFalse Table, then check for Checks.
            If PlayerTurn Then
                Helper.FixTFTable(MasterBoard, True, MasterWhiteTFTable, Helper.ConvertStringToBitCoor(MasterWKPos), MasterWInCheck, MasterWCanCastle.CanICastle(), Helper.ConvertStringToBitCoor(MasterEnPassant))
            Else
                Helper.FixTFTable(MasterBoard, False, MasterBlackTFTable, Helper.ConvertStringToBitCoor(MasterBKPos), MasterBInCheck, MasterBCanCastle.CanICastle(), Helper.ConvertStringToBitCoor(MasterEnPassant))
            End If
            Checkerboard.Refresh()
            AnimateBoard(PreviousFEN)
            EditCheckText()

            Select Case GameMode
                Case 0
                    FlipBoard()
                Case 1
                    If UserPlayer Xor OrientForWhite Then FlipBoard()
                Case Else
                    If PlayerTurn Xor OrientForWhite Then FlipBoard()
            End Select

            GameRunning = True
            If GameMode < 3 Then
                InputTextBox.Text = StartingFEN
                InputTextBox.Select(InputTextBox.Text.Length, 0) 'Stops the TextBox from being highlighted.
            End If

            'Recalibrates AI and resets AI move info.
            MainAI.Reconfigure(CurrentFEN, True)
            AIHandles.CurrentDepth = 2
            AIHandles.CurrentMove = "-"
            AIHandles.CurrentEvaluation = "-"
            CurrentAIDepth.Text = "Current Depth: -"
            CurrentAIMove.Text = "Current Move: -"
            CurrentAIEval.Text = "Evaluation: -"
            AIHandles.FENToResetTo = StartingFEN

            'Resets BoardHistory (as the position is new), then check for end states in the position.
            BoardHistory.Clear()
            Console.Clear()
            EnforceEndStates()
            OutputDebugInfo()
            If GeneralOptions(0) = "T" Then Sound_Move.Play()
        End If
    End Sub

    'Outputs the current FEN into the input textbox.
    Private Sub FENExport_Click() Handles FENExport.Click
        If CurrentFEN <> "" Then
            Dim TempFEN As String = CurrentFEN
            Dim SpaceOccurence As Integer
            'Trims TempFEN to remove its fullmove number & halfmove number, then appends the size of the BoardHistory array
            '(representing how many moves have been made in the position, and how many half moves have been made since the last capture or pawn move).
            For n = TempFEN.Length - 2 To 0 Step -1
                If TempFEN(n) = " " Then
                    SpaceOccurence += 1
                    If SpaceOccurence = 2 Then
                        'We are right in front of the half move count.
                        TempFEN = TempFEN.Substring(0, n + 1)
                        Exit For
                    End If
                End If
            Next
            TempFEN &= BoardHistory.GetHalfSize() & " " 'Adds the half-move count.
            TempFEN &= (BoardHistory.GetSize() + 1) \ 2 'Adds the full-move count.
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
        If Not (ComputerIsSearching OrElse PieceIsMoving OrElse (CurrentFEN = PreviousFEN)) Then
            If ClickMoveMode Then ClickMoveMode = False : ResetLMS(True)
            If AIIsSearchingOnUsersTurn Then MainAI.ABORTSearch()
            Dim TempSH(1, 1) As SByte 'Temp Square History array.
            Dim TempFEN As String = CurrentFEN
            CurrentFEN = PreviousFEN
            PreviousFEN = TempFEN
            MasterWInCheck = 0
            MasterBInCheck = 0
            'Converts the FEN to a board position, and displays it.
            MasterBoard = Helper.FENConverter(CurrentFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
            AnimateBoard(PreviousFEN)
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

            GameRunning = True
            'Resets TrueFalse Tables, then checks for Checks.
            If PlayerTurn Then
                Helper.FixTFTable(MasterBoard, True, MasterWhiteTFTable, Helper.ConvertStringToBitCoor(MasterWKPos), MasterWInCheck, MasterWCanCastle.CanICastle(), Helper.ConvertStringToBitCoor(MasterEnPassant))
            Else
                Helper.FixTFTable(MasterBoard, False, MasterBlackTFTable, Helper.ConvertStringToBitCoor(MasterBKPos), MasterBInCheck, MasterBCanCastle.CanICastle(), Helper.ConvertStringToBitCoor(MasterEnPassant))
            End If
            If AutoFlipper.Checked Then
                FlipBoard()
            Else
                Checkerboard.Refresh() 'This makes the newly-changed Previously used Squares visible to the user.
            End If

            If GeneralOptions(0) = "T" Then Sound_Move.Play()
            EditCheckText()
            'Final logical detection of invalid board positions.
            If Not CheckForInvalidGameStates() AndAlso GeneralOptions(0) = "T" Then Sound_Move.Play()

            'Recalibrates AI, then checks for end positions.
            If AIIsSearchingOnUsersTurn Then Thread.Sleep(5) : SearchSettings.OutputToConsole = True : AIIsSearchingOnUsersTurn = False
            MainAI.Reconfigure(CurrentFEN, False)
            BoardHistory.Swap()
            EnforceEndStates()
            If GameMode < 3 Then
                If PGNAutoOutputter.Checked Then PGNExport_Click() Else FENExport_Click()
            End If
            OutputDebugInfo()
        End If
    End Sub




    'Method that enables the board editor: allowing the user to drag pieces onto the board to create a custom position.
    'This button also acts as the Save button, reverting all the user's changes and resetting the board back to its previous position.
    Private Sub BoardEditorBtn_Click() Handles BoardEditorBtn.Click
        Dim FENErrorMessage As String = ""
        If Not ComputerIsSearching Then
            If ClickMoveMode Then ClickMoveMode = False : ResetLMS(True)

            If Not BoardEdit.isEnabled Then
                'Creates a backup of the baord, then erases it (new position containing only kings in their starting positions).
                Array.Copy(MasterBoard, BoardEdit.BoardBackup, 64)
                'Clears check information, then calibrates the GUI ready for BoardEditHandles.BoardEditMode.
                If MasterWInCheck >= 128 Then WK1.Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\WKing.png")
                If MasterBInCheck >= 128 Then BK1.Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\BKing.png")

                CalibrateBoardEditorObjectHandling()
            Else
                'Attempt to submit the user's FEN into the system. If it is invalid, an appropriate message is displayed.
                If FENErrorDetection(InputTextBox.Text, False, FENErrorMessage) Then
                    AcceptFENIntoSystem(InputTextBox.Text, False, False) 'Assume the FEN is valid = submit it into the system.
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
        Dim OldFEN As String = InputTextBox.Text
        CalibrateBoardEditorObjectHandling()
        'Reconstructs MasterBoard from its backup, and recalibrates the GUI & its current position.
        Array.Copy(BoardEdit.BoardBackup, MasterBoard, 64)
        AnimateBoard(OldFEN)
    End Sub

    'Method that handles the core GUI changes due to the Board Edit Mode.
    Private Sub CalibrateBoardEditorObjectHandling()
        '{Removes & Disables}, or {Shows & Enables} all elements of the GUI that are not strictly related to Board Edit Mode.
        Dim ObjectsToRemove() As Object = {CheckLabel, ProgressBar, AIMoveBtn, CurrentAIDepth, CurrentAIMove,
    CurrentAIEval, UserTimeBar, UserTimeBox, AISettingsBox, UseBook, AIEndlessMode}
        Dim ObjectsToDisable() As Object = {Reset_Btn, InputButton, FENExport, PGNExport, UndoMove, NodeTestBtn}
        For Each Item In ObjectsToRemove
            Item.Visible = BoardEdit.isEnabled
        Next
        AISettingsPanel.Visible = False
        If AIEndlessMode.Checked Then AutoResetter.Visible = BoardEdit.isEnabled
        For Each Item In ObjectsToDisable
            Item.Enabled = BoardEdit.isEnabled
        Next

        BoardEdit.isEnabled = Not BoardEdit.isEnabled
        BoardEditPanel.Visible = BoardEdit.isEnabled
        InputTextBox.ReadOnly = BoardEdit.isEnabled

        If BoardEdit.isEnabled Then
            'Shows the Board Edit Buttons (Save & Cancel Changes), along with all the core controls to this mode.
            BoardEditorBtn.Width = 90
            BoardEditCancelBtn.Visible = True
            BoardEditDiscardBtn.Visible = True
            BoardEditorBtn.Text = "Save Changes:"
            Checkerboard.Width += 225 'We increase the Width of Checkerboard, as we will be needing to place the template pieces to it's right.
            ExitBtn.Top += 15

            BoardEdit.InputBackup = InputTextBox.Text

            'Copies data from the current position.
            BoardEditWhiteMove.Checked = PlayerTurn
            BoardEditBlackMove.Checked = Not PlayerTurn
            BoardEditWKSBox.Checked = MasterWCanCastle.KS
            BoardEditWQSBox.Checked = MasterWCanCastle.QS
            BoardEditBKSBox.Checked = MasterBCanCastle.KS
            BoardEditBQSBox.Checked = MasterBCanCastle.QS

            'Resets the backup board attributes for the current position.
            BoardEdit.PlayerMove = PlayerTurn
            BoardEdit.WCanCastle.CopyFrom(MasterWCanCastle)
            BoardEdit.BCanCastle.CopyFrom(MasterBCanCastle)
            BoardEdit.EnPassant = MasterEnPassant
            If MasterEnPassant <> "-" Then HandleBoardEditEnPassantClick(Val(MasterEnPassant(0)), Val(MasterEnPassant(1)))

            InputTextBox.Text = CurrentFEN

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
            BoardEditDiscardBtn.Visible = False
            BoardEditorBtn.Text = "Board Editor Mode:"
            Checkerboard.Width -= 225
            ExitBtn.Top -= 15
            InputTextBox.Text = BoardEdit.InputBackup
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

    'Subroutine that sets the BoardEdit board to the base position (ie: that with only 2 kings).
    Private Sub BoardEditDiscardBtn_Click() Handles BoardEditDiscardBtn.Click
        'Sets the board, and the FEN.
        For y = 0 To 7
            For x = 0 To 7
                If UCase(MasterBoard(x, y)) <> "K" Then MasterBoard(x, y) = " "
            Next
        Next
        InputTextBox.Text = Helper.ConvertToFEN(MasterBoard, Helper.CannotCastle, Helper.CannotCastle, 0, True)

        'Resets the backup board attributes for the base position set in Board Edit Mode.
        BoardEditWhiteMove.Checked = True
        BoardEditWKSBox.Checked = True
        BoardEditWQSBox.Checked = True
        BoardEditBKSBox.Checked = True
        BoardEditBQSBox.Checked = True

        BoardEdit.PlayerMove = True
        BoardEdit.WCanCastle.CanCastle()
        BoardEdit.BCanCastle.CanCastle()
        If BoardEdit.EnPassant <> "-" Then BoardEdit.EnPassant = "-" : ResetLMS(True) 'Updates EnPassant Square flag (if it is set).

        DisplayPieces()
    End Sub

    'Subroutines that handle when the CTRL key is pressed in BoardEdit Mode. If the user has a piece in their hands, they may be wanting to copy it (note
    'that the HandleBoardEditHandles.BoardEditModeCopy sub only runs when the mouse is actually moved; if the piece is static then no copying will be done).
    Private Sub BoardEditCTRLDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If BoardEdit.isEnabled AndAlso e.Control AndAlso Not BoardEdit.CTRLKeyDown Then
            'User is holding the CTRL key in BoardEdit mode. 
            BoardEdit.CTRLKeyDown = True 'Makes sure no more iterations can run.
            If PieceMoving.IsMovingPiece Then HandleBoardEditModeCopy(PieceMoving.Piece)
        End If
    End Sub
    Private Sub BoardEditCTRLUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        If BoardEdit.CTRLKeyDown AndAlso e.KeyCode = Keys.ControlKey Then BoardEdit.CTRLKeyDown = False 'Resets the Key Controls when the key is lifted.
    End Sub



    'Below are all the handles for editing with the board, its pieces, and its parameters, for the Board Edit Mode.
    Private Sub HandleBoardEditModeDrop(ByVal Piece As PictureBox, ByVal SnapPieceLocation As Point, ByVal OldPosition As Point)
        'Subroutine for handling a piece being dropped onto the board (or off the board).
        Dim CoorX As SByte = SnapPieceLocation.X \ 75
        Dim CoorY As SByte = SnapPieceLocation.Y \ 75
        If Not OrientForWhite Then CoorX = 7 - CoorX : CoorY = 7 - CoorY
        Dim ValidMove As Boolean = True

        'If the piece is not within the confines of the board, remove it (unless it is a king, which cannot be removed).
        If CoorX >= 0 AndAlso CoorY >= 0 AndAlso CoorX < 8 AndAlso CoorY < 8 Then

            If Piece.Name(1) = "P" AndAlso (CoorY = 0 OrElse CoorY = 7) Then
                'The user is attempting to place a pawn on the 1st or 8th row (which is illegal) - move the pawn back.
                ValidMove = False
            ElseIf MasterBoard(CoorX, CoorY) <> " " Then

                If UCase(MasterBoard(CoorX, CoorY)) = "K" Then
                    'The user is attempting to capture a king - move the attacking piece back.
                    ValidMove = False
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
            If ValidMove Then
                'Snap the moved piece at its new location, then save this to MasterBoard.
                Piece.Location = SnapPieceLocation
                'If OldPosition.X < 600 Then MasterBoard(OldPosition.X \ 75, OldPosition.Y \ 75) = " "
                MasterBoard(CoorX, CoorY) = If(Piece.Name(0) = "W", Piece.Name(1), LCase(Piece.Name(1)))
            End If

        ElseIf Piece.Name(1) = "K" Then
            ValidMove = False
        Else
            'The piece has left the confines of the board - remove it.
            Piece.Visible = False
            Piece.Location = New Point(-100, -100)
        End If

        If ValidMove Then
            'Checks to see if the En-Passant Square is still legal (eg: the capturing pawn may have been captured).
            'If not, remove the En-Passant Square.
            If BoardEdit.EnPassant <> "-" AndAlso Not Helper.CheckEnPassantSquareIsLegal(MasterBoard, BoardEdit.EnPassant, BoardEdit.PlayerMove) Then
                BoardEdit.EnPassant = "-"
                ResetLMS(True)
            End If

            'Puts the user's custom position's FEN into the FEN box, so that the user can copy it easily.
            InputTextBox.Text = Helper.ConvertToFEN(MasterBoard, BoardEdit.WCanCastle, BoardEdit.BCanCastle, Helper.ConvertStringToBitCoor(BoardEdit.EnPassant), BoardEdit.PlayerMove)
        Else
            Piece.Location = OldPosition
            If OldPosition.X < 600 Then MasterBoard(OldPosition.X \ 75, OldPosition.Y \ 75) = If(Piece.Name(0) = "W", Piece.Name(1), LCase(Piece.Name(1)))
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
            If MasterBoard(CoorX, CoorY) = " " OrElse Not (UCase(MasterBoard(CoorX, CoorY)) = "K") Then
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
                    If BoardEdit.EnPassant <> "-" AndAlso Not Helper.CheckEnPassantSquareIsLegal(MasterBoard, BoardEdit.EnPassant, BoardEdit.PlayerMove) Then BoardEdit.EnPassant = "-" : ResetLMS(True)
                    'Puts the user's custom position's FEN into the FEN box.
                    InputTextBox.Text = Helper.ConvertToFEN(MasterBoard, BoardEdit.WCanCastle, BoardEdit.BCanCastle, Helper.ConvertStringToBitCoor(BoardEdit.EnPassant), BoardEdit.PlayerMove)
                End If
            End If
        End If
    End Sub


    'Handle which change the parameters of the board (such as White / Black to move & Castling info).
    Private Sub HandleBoardEditParameterChange(sender As Object, e As EventArgs) Handles BoardEditWhiteMove.CheckedChanged, BoardEditBlackMove.CheckedChanged, BoardEditWKSBox.CheckedChanged, BoardEditWQSBox.CheckedChanged, BoardEditBKSBox.CheckedChanged, BoardEditBQSBox.CheckedChanged
        'Enable / Disable the BoardEdit board info, depending on the check state of the GUI parameters.
        Select Case sender.Name
            Case "BoardEditWhiteMove", "BoardEditBlackMove"
                BoardEdit.PlayerMove = BoardEditWhiteMove.Checked
            Case "BoardEditWKSBox"
                BoardEdit.WCanCastle.KS = BoardEditWKSBox.Checked
            Case "BoardEditWQSBox"
                BoardEdit.WCanCastle.QS = BoardEditWQSBox.Checked
            Case "BoardEditBKSBox"
                BoardEdit.BCanCastle.KS = BoardEditBKSBox.Checked
            Case "BoardEditBQSBox"
                BoardEdit.BCanCastle.QS = BoardEditBQSBox.Checked
        End Select

        'Updates the user's current FEN onto the FEN text box, to represent these updated parameters.
        InputTextBox.Text = Helper.ConvertToFEN(MasterBoard, BoardEdit.WCanCastle, BoardEdit.BCanCastle, Helper.ConvertStringToBitCoor(BoardEdit.EnPassant), BoardEdit.PlayerMove)
    End Sub

    'Handle which allows the user to specify the location of the En-Passant square on the board. Contains error handling, so that the user
    'can only place the En-Passant square in a valid position.
    Private Sub HandleBoardEditEnPassantClick(ByVal XCoor As SByte, ByVal YCoor As SByte)
        If Helper.CheckEnPassantSquareIsLegal(MasterBoard, XCoor & YCoor, BoardEdit.PlayerMove) Then
            'Assume the square is correct - edit the Board Edit parameters, then display this En-Passant square on the GUI (using a grey rectangle around this square).
            BoardEdit.EnPassant = XCoor & YCoor
            ResetLMS(False)
            'Highlights the En-Passant square on LMS.
            If OrientForWhite Then
                LegalMoveSquares(XCoor, YCoor) = True
            Else
                LegalMoveSquares(7 - XCoor, 7 - YCoor) = True
            End If
        Else
            'Resets the EnPassant information.
            BoardEdit.EnPassant = "-"
            ResetLMS(False)
        End If
        Checkerboard.Refresh()
        'Updates the user's FEN into the FEN box.
        InputTextBox.Text = Helper.ConvertToFEN(MasterBoard, BoardEdit.WCanCastle, BoardEdit.BCanCastle, Helper.ConvertStringToBitCoor(BoardEdit.EnPassant), BoardEdit.PlayerMove)
    End Sub





    'Subroutine which rotates the board 180 degreees in favour of the other player.
    Private Sub FlipperButton_Click() Handles FlipperButton.Click
        If Not PieceIsMoving Then
            FlipBoard()
            OutputDebugInfo()
        End If
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
        Select Case UserTimeBar.Value
            Case 0 'Lowest Possible Time For Search.
                AIHandles.TimeForSearch = 0.1
            Case < 20 'Between 0.25 and 5 seconds, increment in 0.25 seconds.
                AIHandles.TimeForSearch = UserTimeBar.Value / 4
            Case < 50 'Between 5 and 20 seconds, increment in 0.5 seconds.
                AIHandles.TimeForSearch = (UserTimeBar.Value - 10) / 2
            Case Else 'Between 20 and 30 seconds, increment in 1 second.
                AIHandles.TimeForSearch = UserTimeBar.Value - 30
        End Select
        UserTimeBox.Text = $"Time For Search: {AIHandles.TimeForSearch} seconds."
        ChangeUserTimeBarBackColour()
    End Sub
    'Method that gives the UserTimeBox a red highlight if the time allocated might not be enough for a search to complete (estimate).
    Private Sub ChangeUserTimeBarBackColour()
        Dim ThresholdValue As Double = 1
        If SearchSettings.UseQuiescence Then ThresholdValue += 2.5
        If SearchSettings.UsePieceHeatMaps Then ThresholdValue += 1.5
        If SearchSettings.UseTranspositionTable Then ThresholdValue -= 0.75
        If SearchSettings.StableSearch Then ThresholdValue += 0.75
        UserTimeBox.BackColor = If(UserTimeBar.Value < Math.Round(Math.Min(ThresholdValue, 5)), Color.LightCoral, Color.WhiteSmoke)
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
                        UserTime = AIHandles.TimeForSearch
                        Exit While
                    End If
                    UserTime = Math.Round(CDec(Temp), 1)
                    If UserTime = 0 OrElse (UserTime >= 0.1 AndAlso UserTime <= 600) Then
                        Exit While 'Inside range - input has passed all the checks :).
                    Else 'Input was outside the given range - displays appropriate message.
                        If MsgBox("Invalid Number - Please make sure your input is in the correct range.", vbCritical + vbRetryCancel + vbApplicationModal) = 2 Then
                            'User has cancelled - revert back to original time.
                            UserTime = AIHandles.TimeForSearch
                            Exit While
                        End If
                    End If
                Catch ex As Exception 'Input was not a decimal / integer number - displays appropriate message.
                    If MsgBox("Invalid Entry - Please make sure that your input is in the correct format.", vbCritical + vbRetryCancel + vbApplicationModal) = 2 Then
                        'User has cancelled - revert back to original time.
                        UserTime = AIHandles.TimeForSearch
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
            AIHandles.TimeForSearch = UserTime
        End If
        Me.ActiveControl = Nothing 'Stops the cursor from appearing in the box.
    End Sub


    'Controls for modifying the AI Settings, using a ComboBox + Panel combination. When the user opens the (empty) ComboBox, a panel is shown
    'that contains all the controls.
    Private Sub AISettingsBox_Open() Handles AISettingsBox.DropDown
        If AISettingsPanel.Visible Then
            AISettingsBox_Collapse() 'Safety.
        Else
            'Shows the AI settings, and then the panel.
            SetAISettingPanelValues()
            AISettingsPanel.Visible = True
            AISettingsPanel.BringToFront()
            AISettingsPanel.Focus() 'Focusses on the panel to prevent box selection (note that this does not trigger the collapse via WndProc).
            UserCanChangeAISettings = True
        End If
    End Sub
    Private Sub AISettingsBox_Collapse() Handles AISettingsBox.DropDownClosed
        UserCanChangeAISettings = False
        AISettingsPanel.Visible = False
        Checkerboard.Focus() 'Prevents the ComboBox text from being highlighted.
    End Sub

    'Subroutine that sets all values in the AISettingsPanel, based on the current AI settings.
    Private Sub SetAISettingPanelValues()
        For Each Field As FieldInfo In AISettingsFields
            For Each c As Control In AISettingsPanel.Controls
                'Finds the control in the Settings Panel that matches the Field in AISettings.
                If c.Name = Field.Name Then
                    'There are two types of controls - numerical, and boolean. Set either the checkbox or the textbox values accordingly (for textbox,
                    'we also have a label, that contains no name).
                    If Field.FieldType = GetType(Boolean) Then
                        'CheckBox.
                        DirectCast(c, CheckBox).Checked = Field.GetValue(SearchSettings)
                    Else
                        'TextBox.
                        c.Text = Field.GetValue(SearchSettings)
                    End If
                End If
            Next
        Next
    End Sub

    'Modifies the AI settings in case the user changes anything.
    Private Sub AISettingValueChanged(sender As Object, e As EventArgs)
        If UserCanChangeAISettings Then
            'Finds the field associated with this value, then modifies it directly.
            For Each Field As FieldInfo In AISettingsFields
                If sender.Name = Field.Name Then
                    If Field.FieldType = GetType(Boolean) Then
                        'Set the boolean value based on the state of the check.
                        Field.SetValue(SearchSettings, DirectCast(sender, CheckBox).Checked)
                    ElseIf System.Text.RegularExpressions.Regex.IsMatch(sender.Text, "^[0-9]+$") Then
                        'If we are using numerical values, we need to ensure that there purely numbers. If so, we convert the value to the
                        'specific numerical type that that field uses, then assigns it.
                        Field.SetValue(SearchSettings, Convert.ChangeType(sender.Text, Field.FieldType))
                    End If
                End If
            Next
            'Modifies the TimeBar colour to represent if we are pushing the AI a little too much.
            Dim TimeBarColourChangeObjects() As String = {"UseQuiescence", "UsePieceHeatMaps", "UseTranspositionTable"}
            If TimeBarColourChangeObjects.Contains(sender.Name) Then ChangeUserTimeBarBackColour()
        End If
    End Sub
    'Resets the AI Settings.
    Private Sub AISettingResetBtn_Click() Handles AISettingResetBtn.Click
        SearchSettings.SetDefaultSettings()
        SetAISettingPanelValues()
    End Sub

    'Method that overwrites the 'de-focus' mechanic of the form, so that if we have lost focus from the ComboBox, but we are still inside it, we do not collapse it.
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = &H111 AndAlso ((m.WParam.ToInt32() >> 16) And &HFFFF) = 8 Then
            'Checks if the mouse is inside the settings panel. If so, we prevent dropdown closing.
            If AISettingsPanel.ClientRectangle.Contains(AISettingsPanel.PointToClient(Control.MousePosition)) OrElse AISettingsBox.ClientRectangle.Contains(AISettingsBox.PointToClient(Control.MousePosition)) Then
                Return
            End If
        End If
        MyBase.WndProc(m)
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
        If PGNAutoOutputter.Checked Then PGNExport_Click() Else FENExport_Click()
    End Sub



    'Subroutines which handle the Node Test GUI buttons.
    Private Sub NodeTestBtn_Click() Handles NodeTestBtn.Click
        If GameRunning AndAlso Not ComputerIsSearching Then
            If ClickMoveMode Then ClickMoveMode = False : ResetLMS(True)
            'Creates a new thread for the Node Test to run on, then starts the search.
            ComputerIsSearching = True
            MainAI.ConfigureSettings(SearchSettings, False)
            Me.Text = "[Running Test...]  " + GlobalConstants.ProgramName
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
        Me.Text = GlobalConstants.ProgramName
        GCSettings.LatencyMode = GCLatencyMode.Interactive 'Resets garbage collection mode.
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine(("Node Test Terminated (by user).").PadRight(48) & vbCrLf)
    End Sub




    'Subroutine that reveals the settings menu to the user.
    Private Sub SettingsBtn_Click() Handles SettingsBtn.Click
        'Creates and reveals the Settings Form.
        If ClickMoveMode Then ClickMoveMode = False : ResetLMS(True)
        AISettingsBox_Collapse()
        Dim CurrentSettings As Form = CType(Application.OpenForms("Settings"), Settings)
        If CurrentSettings Is Nothing Then 'If there are no other Settings forms open.
            Dim SettingsMenu As New Settings(GameMode <= 3, GameMode < 3, GameMode > 1 AndAlso GameMode <= 3)
            SettingsMenu.Show()
        Else
            'Highlights the settings window.
            CurrentSettings.BringToFront()
        End If
    End Sub

    'Button that displays information about the training modes.
    Private Function InfoBtn_Click() As MsgBoxResult Handles InfoBtn.Click
        Select Case GameMode
            Case 0
                If MsgBox("Welcome to Remote Mode!" + vbCrLf + "In this mode, the program will attempt to locate an interactive chess interface on your screen, and interact with it either via human interaction, or via this program's AI." + vbCrLf + "Note that this program will have full control of your mouse during this mode - to temporarily suspent this control, please press the SHIFT key." + vbCrLf + vbCrLf + "For this feature to work as intended, please ensure that this program does not obstruct (in any way) the chess interface to control, the chess interface board is set to the provided FEN, and that all move animations & board highlights are turned off. The program will also work at its best with a simple board layout, and with 'Automatic Queen Promotion' turned off." + vbCrLf + vbCrLf + "WARNING: The use of this feature to play against other humans online (using this program's AI) is a bannable offence on most online chess websites. By using this feature, you take responsibility for any restrictions, bans, limits, etc that your account may face as a result." + vbCrLf + vbCrLf + "Do you want to proceed?", vbInformation + vbYesNo) = 6 Then
                    Return MsgBoxResult.Yes
                Else
                    ExitBtn_Click()
                End If
            Case 4
                Return MsgBox("• In this training mode, you will be given a chess position, where the side to move only has one winning move. You will need to find that winning move, and play it on the board." & vbCrLf & "• Getting a puzzle correct will increase your rating, getting it incorrect will decrease your rating." & vbCrLf & "• If you cannot find the move(s), you can either take a hint (which displays the piece to move, but will halve the potential rating gain), or give up to reveal the move." & vbCrLf & "• The AI will search on the position in the background, and (if the toggle is set to AI mode) will automatically play its moves on the board." & vbCrLf & "• The AI will be set to its maximum difficulty during its search." & vbCrLf & "• Note: when modifying settings, your changes will be implemented on the next move / puzzle." & vbCrLf & "• Please note that you can only reset the puzzle ratings once the current puzzle has been solved / deemed as being incorrect.", vbInformation + vbApplicationModal, "Instructions")
            Case 5
                Return MsgBox("• In this training game, you will be given a chess coordinate (in standard chess notation), and will be tasked with clicking the corresponding square on the chessboard as fast as you can." & vbCrLf & "• If you click the correct square, it will light up in green, and you will be given a new coordinate to click." & vbCrLf & "• Click the most coordinates you can in 20 seconds, and try to get a place on the leaderboard!" & vbCrLf & "• Clicking the 'Flip Board' button will rotate the board 180°, so that each coordinate will be from black's point of view.", vbInformation + vbApplicationModal, "Instructions")
            Case 6
                Return MsgBox("• In this training game, you will be given a random chess position along with a possible move (in standard chess notation). You will need to play that move as fast as you can." & vbCrLf & "• If you make the correct move, that square will light up in green, and you will be given a new move to make." & vbCrLf & "• After " & GlobalConstants.TrainingMovesPerPosition & " moves are made in a position, a new random position will be generated." & vbCrLf & "• Complete the most moves you can in 30 seconds, and try to get a place on the leaderboard!" & vbCrLf & "• Clicking the 'Flip Board' button will rotate the board 180°, so that each move will be from black's point of view." & vbCrLf & vbCrLf & "• Note: Regardless of whether the board is flipped or not, only the white pieces will need to be moved.", vbInformation + vbApplicationModal, "Instructions")
        End Select
        Return 0
    End Function

    'Button that returns the user back to the Main Menu. 
    Private Sub ExitBtn_Click() Handles ExitBtn.Click
        If MainAI IsNot Nothing Then AICanSearchOnUsersTurn = False : MainAI.ABORTSearch()
        If GameMode = 4 Then TrainingMode.PuzzleSampleDatabase = Nothing 'Clears the puzzle database to save memory.

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
        Menu.BringToFront()
    End Sub

    'Button that displays the credits information onto the screen (in the form of a pop-up).
    Private Sub Credits_Click() Handles Credits.Click
        Dim CanRetrieveStats As Boolean
        'Loads the Lifetime stats file, then assigns each line to their appropriate variable.
        Dim LifetimePositions, LifetimeTranspositions, LifetimeCheckmates As UInt64
        Try
            Using SR As New StreamReader(GlobalConstants.StartupPath & "\Assets\User\AIStats.txt", Encoding.UTF8, True)
                LifetimePositions = SR.ReadLine()
                LifetimeTranspositions = SR.ReadLine()
                LifetimeCheckmates = SR.ReadLine()
            End Using
            CanRetrieveStats = True
        Catch ex As Exception
            'An error occured when reading from the file.
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Unable to retrieve AI lifetime stats.")
            Console.ForegroundColor = ConsoleColor.White
        End Try

        Dim CreditsMessage As String = Strings.StrDup(10, " ") & "Chess Game & Artificial Intelligence (" & GlobalConstants.ProgramVersion & ")" & vbCrLf & Strings.StrDup(21, " ") & "Created by Alfie Kunz (8158)" & vbCrLf & Strings.StrDup(22, " ") & "of Beckfoot School (37101)" & vbCrLf & "Project used for the AQA GCE Computer Science NEA" & vbCrLf & Strings.StrDup(35, " ") & "(2021 - 2025)"
        If CanRetrieveStats Then
            MsgBox(CreditsMessage & vbCrLf & vbCrLf & vbCrLf & "Lifetime AI Statistics:" & vbCrLf & "Positions Searched: " & LifetimePositions.ToString("N0") & vbCrLf & "Transpositions Found: " & LifetimeTranspositions.ToString("N0") & vbCrLf & "Checkmates Made: " & LifetimeCheckmates.ToString("N0"), vbInformation, "Credits")
        Else
            MsgBox(CreditsMessage, vbInformation, "Credits")
        End If

    End Sub

End Class
