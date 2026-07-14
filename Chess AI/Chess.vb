'Working Version of Chess, along with AI using NegaMax and Alpha Beta Pruning. Created by Alfie Kunz - 8158.
Imports System.Diagnostics.Eventing
Imports System.Diagnostics.Eventing.Reader
Imports System.IO
Imports System.Net.WebRequestMethods
Imports System.Reflection
Imports System.Runtime
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Windows.Input
Imports Microsoft.SqlServer
Imports Microsoft.VisualBasic.Devices

'This class forms the CORE User Interface and the basic game of chess, along with its rules. It will control moving pieces,
'importing / exporting positions, manipulating the board + its visual design ext, along with converting a user’s actions onto
'a computer-friendly interface (for the AI to use). This also instantiates the AI class and interacts with it.
Partial Public Class Chess 'ew- danny

    Private Helper As New CoreMethods 'Calls all objects from the CoreMethods class.
    Private BoardHistory As New GameHistory
    Private GeneralOptions As String = GlobalConstants.DefaultGeneralOptions
    Private ReadOnly OpeningBook As New List(Of OpeningBookEntry)
    Private IsClosing As Boolean


    'Here are the computer's primary "eyes" - MasterBoard (which contains each piece's location on the board) and,
    'derived from this, Master TrueFalse Tables for each player. These Tables display information such as Legal
    'Moves for the players' kings, along with pinned pieces and their location, and make handling Move Generation,
    'Checks and Evaluations much easier and more efficient.
    Private MasterBoard(7, 7), MasterWhiteTFTable(7, 7), MasterBlackTFTable(7, 7) As Char
    Private GameRunning As Boolean
    Private GameMode As Byte = 3 '1 = 1-Player, 2 = 2-Player, 3 = Analysis, 4 = Puzzles, 5 = Coordinate Practice, 6 = Move Practice.

    'FEN = Standard Chess notation that displays where all the pieces on the board are supposed to go.
    Private StartingFEN As String = GlobalConstants.StartingFENPosition
    Private CurrentFEN, PreviousFEN, InvalidInput As String
    Private PlayerTurn As Boolean = True 'True = Is White's Turn, False = Is Black's Turn
    Private UserPlayer As Boolean = True 'Represents what colour the user is playing with.
    Private MasterWKPos, MasterBKPos As String 'Loaction of Kings.
    Private MasterEnPassant As String 'Location of EnPassant Square
    Private MasterZobristValue As UInt64 'Zobrist value of the current position (a 'unique' value of a board position, that can be easily calculated)

    'Data for castling & checks for each player.
    Private MasterWCanCastle, MasterBCanCastle As New CanCastle
    Private MasterWInCheck, MasterBInCheck As Byte


    'Sets up sound effects. All sounds used by Chess.com
    Private ReadOnly Sound_Move As New Media.SoundPlayer With {
        .SoundLocation = GlobalConstants.StartupPath & "\Assets\Sounds\Chess_Move.wav"
    }
    Private ReadOnly Sound_Capture As New Media.SoundPlayer With {
        .SoundLocation = GlobalConstants.StartupPath & "\Assets\Sounds\Chess_Capture.wav"
    }
    Private ReadOnly Sound_Check As New Media.SoundPlayer With {
        .SoundLocation = GlobalConstants.StartupPath & "\Assets\Sounds\Chess_Check.wav"
    }
    Private ReadOnly Sound_Castle As New Media.SoundPlayer With {
        .SoundLocation = GlobalConstants.StartupPath & "\Assets\Sounds\Chess_Castle.wav"
    }
    Private ReadOnly Sound_Checkmate As New Media.SoundPlayer With {
        .SoundLocation = GlobalConstants.StartupPath & "\Assets\Sounds\Chess_Checkmate.wav"
    }
    Private ReadOnly Sound_Stalemate As New Media.SoundPlayer With {
        .SoundLocation = GlobalConstants.StartupPath & "\Assets\Sounds\Chess_Stalemate.wav"
    }
    Private ReadOnly Sound_GameOver As New Media.SoundPlayer With {
        .SoundLocation = GlobalConstants.StartupPath & "\Assets\Sounds\Chess_GameOver.wav"
    }





    'Constructor methods for the Chess Class.
    Public Sub New()
        Me.New(New List(Of OpeningBookEntry))
    End Sub
    Public Sub New(ByVal Mode As Byte)
        Me.New(Mode, GlobalConstants.StartingFENPosition)
    End Sub
    Public Sub New(ByVal Mode As Byte, ByVal UserStartingFEN As String)
        Me.New(Mode, UserStartingFEN, 0, New AISearchSettings(), False, False, True, Nothing)
    End Sub

    Public Sub New(ByRef InputBook As List(Of OpeningBookEntry)) 'Call used for a standard game of chess.
        GameRunning = False
        InitializeComponent() 'This call is required by the designer.
        'Loads the opening book into the system.
        If InputBook IsNot Nothing AndAlso InputBook.Count > 0 Then OpeningBook.AddRange(InputBook) : UseBook.Enabled = True
        If GameMode = 3 Then
            AISettingsBox.SelectedIndex = 0
            'Loads all the AI settings into the settings panel.
            AISettingsFields = GetType(AISearchSettings).GetFields()
            Dim CheckBoxHeight As Integer = 6
            For Each Field As FieldInfo In AISettingsFields
                If Not SearchSettings.NonDisplayable.Contains(Field.Name) Then
                    'Adds all the fields to the panel, with decent spacing.
                    Dim AISetting As Control = If(Field.FieldType = GetType(Boolean), New CheckBox(), New Label())
                    'Converts the Field Name into a readable name, by inserting spaces before each capital letter.
                    AISetting.Text = Regex.Replace(Field.Name, "([A-Z])", " $1").TrimStart()
                    AISetting.Location = New Point(15 + If(Field.FieldType = GetType(Boolean), 0, 22), CheckBoxHeight)
                    AISetting.Width = 180
                    AISettingsPanel.Size = New Size(AISettingsPanel.Size.Width, AISettingsPanel.Size.Height + 25)

                    If Field.FieldType = GetType(Boolean) Then
                        AISetting.Name = Field.Name 'Represents the item that will be assigned values by the settings (indexed by name).
                        AddHandler DirectCast(AISetting, CheckBox).CheckedChanged, AddressOf AISettingValueChanged
                    Else
                        'Adds a textbox to enable the user to enter their own value.
                        Dim SettingBox As New System.Windows.Forms.TextBox With {
                            .Location = New Point(8, CheckBoxHeight - 2),
                            .Size = New Size(30, 20)
                        }
                        SettingBox.Name = Field.Name
                        AddHandler SettingBox.TextChanged, AddressOf AISettingValueChanged
                        AISettingsPanel.Controls.Add(SettingBox)
                    End If
                    AISettingsPanel.Controls.Add(AISetting) 'Adds to the settings panel.
                    CheckBoxHeight += 25
                End If
            Next
            AISettingResetBtn.Top += CheckBoxHeight
        End If
    End Sub

    'The below constructor method rearranges the location of objects on the form (to save space on the screen).
    'This will be called for 1-Player Games and 2-Player Games.
    Public Sub New(ByVal Mode As Byte, ByVal UserStartingFEN As String, ByVal UserTimeForSearch As Decimal, ByVal UserSearchSettings As AISearchSettings, ByVal UserAICanSearchOnUsersTurn As Boolean, ByVal UserAdvancedSearchTime As Boolean, ByVal PlayAsWhite As Boolean, ByRef InputBook As List(Of OpeningBookEntry))
        Me.New(InputBook)
        GameMode = Mode
        StartingFEN = UserStartingFEN

        'Shrinks the size of the forms, and modifies the objects now off-screen to better suit the GUI.
        Me.Size = New Size(916, 639)
        AIMoveBtn.Visible = False
        InputTextBox.ReadOnly = True
        Reset_Btn.Visible = False
        InputButton.Visible = False
        FENExport.Visible = False
        PGNExport.Visible = False
        UndoMove.Top -= 70
        BoardEditorBtn.Visible = False
        CheckLabel.Location = New Point(89, 163)
        UserTimeBar.Visible = False
        UserTimeBox.Visible = False
        UseBook.Visible = False
        AIEndlessMode.Visible = False
        NodeTestBtn.Visible = False
        SettingsBtn.Location = New Point(40, 520)
        ExitBtn.Location = New Point(181, 520)
        Credits.Location = New Point(78.5, 575)


        If Mode <= 1 Then 'Mode-Specific Movement - 1P Mode & Remote Mode.
            'Hides / sets appropriate objects.
            AutoOutputterPanel.Visible = True
            FlipperButton.Visible = False
            AutoFlipper.Visible = False
            ProgressBar.Location = New Point(46, 270)
            CurrentAIDepth.Location = New Point(ProgressBar.Location.X + 10, ProgressBar.Location.Y + 80)
            CurrentAIMove.Location = New Point(CurrentAIDepth.Location.X, CurrentAIDepth.Location.Y + 30)
            CurrentAIEval.Location = New Point(CurrentAIDepth.Location.X, CurrentAIDepth.Location.Y + 60)
            UserPlayer = PlayAsWhite
            'Calibrates AI settings based on the user's requirements.
            If InputBook.Count > 0 Then UseBook.Checked = True
            If UserTimeForSearch > 0 Then
                AIHandles.TimeForSearch = UserTimeForSearch
                If UserAdvancedSearchTime Then AIHandles.AdvancedTimeForSearch = UserTimeForSearch
                SearchSettings.CopyFrom(UserSearchSettings)
                AICanSearchOnUsersTurn = UserAICanSearchOnUsersTurn
            End If
        ElseIf Mode = 2 Then '2P Mode
            AutoOutputterPanel.Visible = True
            FlipperButton.Top -= 100
            AutoFlipper.Top -= 100
            'Hides / sets appropriate objects, so that only the relevant objects are displayed.
            ProgressBar.Visible = False
            CurrentAIEval.Visible = False
            AutoFlipper.Checked = True
            CurrentAIDepth.Visible = False
            CurrentAIMove.Visible = False
            CurrentAIEval.Visible = False
        ElseIf Mode = 4 Then 'Puzzles mode
            'Selects, then loads a random puzzle database from the Database folder.
            Static RND As New Random()
            Dim PuzzleFileNames As String() = Directory.GetFiles(GlobalConstants.StartupPath & "\Assets\Puzzle Database\", "*.txt")
            Dim SampleIndex As Byte = RND.Next(1, PuzzleFileNames.Length)

            Dim Timer As New Stopwatch
            Dim TempEntry As PuzzleEntry
            Try
                Timer.Start()
                'Opens puzzle database from file using StreamReader.
                Using SR As New StreamReader(GlobalConstants.StartupPath & "\Assets\Puzzle Database\" & CStr(SampleIndex) & ".txt", Encoding.UTF8, True, 16384)
                    While Not SR.EndOfStream
                        TempEntry = New PuzzleEntry(SR.ReadLine())
                        TrainingMode.PuzzleSampleDatabase.Add(TempEntry)
                    End While
                End Using
                'Loads extra hard puzzles from database.
                Using SR As New StreamReader(GlobalConstants.StartupPath & "\Assets\Puzzle Database\Extra Hard Puzzles.txt", Encoding.UTF8, True, 4096)
                    While Not SR.EndOfStream
                        TempEntry = New PuzzleEntry(SR.ReadLine())
                        TrainingMode.PuzzleSampleDatabase.Add(TempEntry)
                    End While
                End Using
                Timer.Stop()
                Console.ForegroundColor = ConsoleColor.Green
                Console.WriteLine("Puzzle Database " & SampleIndex & " Successfully Retrieved in:  " & Math.Round(Timer.Elapsed.TotalMilliseconds, 1) & "ms.")
            Catch ex As Exception 'Could not successfully retrieve database - exit to main menu.
                Timer.Stop()
                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.WriteLine("Error when retrieving Puzzle Database.")
                MsgBox("Error: Unable to retrive the Puzzle Database." & vbCrLf & "Returning to the Main Menu...", vbCritical + vbOKOnly + vbApplicationModal)
                ExitBtn_Click()
                Exit Sub
            End Try

            LoadPuzzleRatings()
            SearchSettings.OutputToConsole = False
            AIHandles.TimeForSearch = Decimal.MaxValue 'Makes sure there isn't a puzzle timer on the initial puzzle.
            StartingFEN = GetRndPuzzle() 'Sets Random Puzzle to be starting position.

            'Calibrates objects specific to the puzzle mode.
            InputTextBox.Size = New Size(282, 35)
            UndoMove.Visible = False
            CheckLabel.Top += 15
            FlipperButton.Visible = False
            AutoFlipper.Visible = False
            ProgressBar.Location = New Point(10, 470)
            ProgressBar.Size = New Size(150, 25)
            CurrentAIEval.Visible = False
            CurrentAIDepth.Visible = False
            CurrentAIMove.Visible = False
            CurrentAIEval.Visible = False
            PuzzleRatingLabel.Visible = True
            HintBtn.Visible = True
            GiveUpBtn.Visible = True
            RatingHeader.Visible = True
            RatingLabel.Visible = True
            LostRatingLabel.Visible = True
            GainedRatingLabel.Visible = True
            AIPuzzleInfoLabel.Visible = True
            HumanModeBtn.Visible = True
            AIModeBtn.Visible = True
            AutoAdvanceOnComplete.Visible = True
            AutoAdvanceLabel.Visible = True
            AutoAdvanceBox.Visible = True
            AutoAdvanceBar.Visible = True
            InfoBtn.Visible = True
            ResetRatingsBtn.Visible = True

        ElseIf Mode = 5 OrElse Mode = 6 Then 'Coordinate / Move Training Mode.
            'Hides / sets appropriate objects.
            InputTextBox.Visible = False
            UndoMove.Visible = False
            CheckLabel.Visible = False
            FlipperButton.Top -= 60
            AutoFlipper.Visible = False
            ProgressBar.Visible = False
            CurrentAIEval.Visible = False
            CurrentAIDepth.Visible = False
            CurrentAIMove.Visible = False
            CurrentAIEval.Visible = False
            TrainingStart.Visible = True
            TimerLabel.Visible = True
            WLeaderBoardGrid.Visible = True
            InfoBtn.Visible = True
            'Calibrates Leaderboards.
            RetrieveLeaderboards()
            If GameMode = 6 Then TimerLabel.Text = "Time Left: 30.0 Seconds"
        End If
    End Sub

    'Constructor Subroutines for the Remote Mode feature.
    Public Sub New(ByVal UsingRemoteMode As Boolean, ByVal UserStartingFEN As String, ByVal PlayAsWhite As Boolean, ByRef InputBook As List(Of OpeningBookEntry))
        Me.New(UsingRemoteMode, UserStartingFEN, PlayAsWhite, InputBook, 0, Nothing, False, False)
        'Removes all the objects that are not intrinsic to Remote Mode.
        RemoteMode.AIEnabled = False
        Dim ObjectsToRemove() As Object = {ProgressBar, CurrentAIDepth, CurrentAIMove, CurrentAIEval, AITerminator}
        For Each Item In ObjectsToRemove
            Item.Visible = False
        Next
        RemoteModeBtn.Top += 36
    End Sub
    Public Sub New(ByVal UsingRemoteMode As Boolean, ByVal UserStartingFEN As String, ByVal PlayAsWhite As Boolean, ByRef InputBook As List(Of OpeningBookEntry), ByVal UserTimeForSearch As Decimal, ByVal UserSearchSettings As AISearchSettings, ByVal UserAICanSearchOnUsersTurn As Boolean, ByVal UserAdvancedSearchTime As Boolean)
        'Positions the Form.
        Me.New(1, UserStartingFEN, UserTimeForSearch, UserSearchSettings, UserAICanSearchOnUsersTurn, UserAdvancedSearchTime, PlayAsWhite, InputBook)
        'Sets up & positions all objects used for Remote Mode.
        GameMode = 0
        RemoteMode = New RemoteModeInfo With {
            .IsEnabled = True,
            .AIEnabled = True
        }
        UndoMove.Visible = False

        CheckLabel.Top -= 50
        ProgressBar.Top += 75
        CurrentAIDepth.Top += 60
        CurrentAIMove.Top += 60
        CurrentAIEval.Top += 60
        RemoteModeBtn.Visible = True
        AITerminator.Location = New Point(59, 263)
        AITerminator.Size = New Size(181, 43)
        AITerminator.Enabled = False
        AITerminator.Visible = True

        InfoBtn.Visible = True
    End Sub



    'Subroutine that sets up the chess system.
    Private Sub Form_Load() Handles Me.Load
        Dim StartupStopwatch As New Stopwatch
        StartupStopwatch.Start()
        Application.EnableVisualStyles()
        Me.DoubleBuffered = True 'For smoother piece moving animations.
        Me.KeyPreview = True 'For handling CTRL in Board Edit Mode.
        Console.OutputEncoding = System.Text.Encoding.UTF8
        Randomize()

        'Assigns each piece to PieceArray and sets the 'board' picturebox as their parent.
        PieceArray.Add(WK1)
        PieceArray.Add(WQ1)
        PieceArray.Add(WB1)
        PieceArray.Add(WN1)
        PieceArray.Add(WR1)
        PieceArray.Add(WP1)

        PieceArray.Add(BK1)
        PieceArray.Add(BQ1)
        PieceArray.Add(BB1)
        PieceArray.Add(BN1)
        PieceArray.Add(BR1)
        PieceArray.Add(BP1)

        For Each Piece In PieceArray
            Checkerboard.Controls.Add(Piece)
            'Adds handles for each of the PictureBoxes, to handle user movement.
            AddHandler Piece.MouseDown, AddressOf Piece_MouseDown
            AddHandler Piece.MouseMove, AddressOf Piece_MouseMove
            AddHandler Piece.MouseUp, AddressOf Piece_MouseUp
        Next

        'Sets up more essential objects...
        UndoFENChange.Visible = False
        CheckLabel.Text = "                    "
        For n = 0 To 3
            SquareHistory(n, 0) = -1
            SquareHistory(n, 1) = -1
        Next

        'If the FEN is valid, we send the start postion to be converted to the Board array, then display the pieces on the board.
        'If it is not valid, we replace the FEN with the Default StartingFEN.
        Dim FENErrorMessage As String = ""
        Dim FENIsValid As Boolean = False
        Dim CustomisationForm As Form = Nothing 'We first locate the cusomisation form (if the user used it).
        If GameMode <= 1 Then
            CustomisationForm = CType(Application.OpenForms("OnePlayerCustomisation"), OnePlayerCustomisation)
        ElseIf GameMode = 2 Then
            CustomisationForm = CType(Application.OpenForms("TwoPlayerCustomisation"), TwoPlayerCustomisation)
        ElseIf GameMode > 3 Then
            CustomisationForm = CType(Application.OpenForms("TrainingCustomisation"), TrainingCustomisation)
        End If
        If FENErrorDetection(StartingFEN, False, FENErrorMessage) Then 'First step of error-detection.
            Try
                'Forms the chessboard, along with the required variables.
                MasterBoard = Helper.FENConverter(StartingFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
                DisplayPieces()
                'Forms the TFTable for the players.
                If PlayerTurn Then
                    Helper.FixTFTable(MasterBoard, True, MasterWhiteTFTable, Helper.ConvertStringToBitCoor(MasterWKPos), MasterWInCheck, MasterWCanCastle.CanICastle(), Helper.ConvertStringToBitCoor(MasterEnPassant))
                Else
                    Helper.FixTFTable(MasterBoard, False, MasterBlackTFTable, Helper.ConvertStringToBitCoor(MasterBKPos), MasterBInCheck, MasterBCanCastle.CanICastle(), Helper.ConvertStringToBitCoor(MasterEnPassant))
                End If
                If GameMode <> 3 Then CustomisationForm.Close() 'Closes the customisation form.
                EditCheckText()
                FENIsValid = True
            Catch ex As Exception 'FEN is not valid, as the board could not be constructed - provide error message.
                If MsgBox("Starting Position Rejected - Invalid FEN Code. Please Input a Genuinine FEN and try again." & vbCr & "Please Click 'Retry' to enter another FEN, or Click 'Cancel' to Start the Game with the Standard Starting Position.", vbCritical + vbRetryCancel + vbApplicationModal) = 4 Then
                    'The user wants to enter another FEN.
                    Me.Close()
                End If
            End Try
        Else
            'Displays the appropriate error message, as stated by the FENErrorDetection Sub.
            If MsgBox("Starting " & FENErrorMessage & vbCr & "Please Click 'Retry' to Enter Another FEN, or Click 'Cancel' to Start the Game with the Standard Starting Position.", vbCritical + vbRetryCancel + vbApplicationModal) = 4 Then
                'The user wants to enter another FEN.
                Me.Close()
            End If
        End If

        'Calibrates the board using the standard starting position.
        If Not FENIsValid AndAlso Application.OpenForms().OfType(Of Chess).Any Then 'If customisation form hasn't been closed.
            'Sets the board to the standard FEN postion, and forms variables & objects.
            CustomisationForm.Close()
            StartingFEN = GlobalConstants.StartingFENPosition
            MasterBoard = Helper.FENConverter(StartingFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
            DisplayPieces()
            Helper.FixTFTable(MasterBoard, True, MasterWhiteTFTable, Helper.ConvertStringToBitCoor(MasterWKPos), MasterWInCheck, MasterWCanCastle.CanICastle(), Helper.ConvertStringToBitCoor(MasterEnPassant))
            'Resets Black's TFTable.
            Array.Copy(Helper.MasterTrueTable, MasterBlackTFTable, 64)
        End If



        If Me.IsHandleCreated Then 'If this form is open...
            'Flips board in user's favour (but must be for the opposite player for puzzles).
            If GameMode = 4 Then
                If PlayerTurn Then FlipBoard()
            Else
                If Not UserPlayer Then FlipBoard()
            End If

            'Final logical detection of invalid board positions.
            If Not CheckForInvalidGameStates() Then
                'Everything is valid - begin the game as normal.
                Console.ForegroundColor = ConsoleColor.Green
                GameRunning = True
                Console.WriteLine(vbCrLf & vbCrLf & "The Game has Begun.")
                Console.ForegroundColor = ConsoleColor.White
            End If

            'Creates our AI (for legal move generation).
            CurrentFEN = StartingFEN
            MainAI = New AI(CurrentFEN)
            MainAI.ConfigureSettings(SearchSettings, False)
            MasterZobristValue = Helper.ZobristHashPosition(MasterBoard, PlayerTurn, MasterWCanCastle, MasterBCanCastle, Helper.ConvertStringToBitCoor(MasterEnPassant))
            AIHandles.FENToResetTo = StartingFEN

            LoadUserProfile()

            StartupStopwatch.Stop()
            Console.WriteLine("Startup Time: " & StartupStopwatch.Elapsed.TotalMilliseconds & " Milliseconds.")
            If GameMode <= 3 AndAlso GameMode > 0 Then OutputDebugInfo()

            AddHandler Me.Shown, AddressOf Form1_Load_AfterWindowShown 'After all the core elements have been configured, show the
            'form window to the user, then run the non-core elements of the class.
        End If
    End Sub

    'Subroutine containing all the code that will be performed upon boot-up, after the Chess form has been shown to the user.
    Private Sub Form1_Load_AfterWindowShown()
        EnforceEndStates() 'Checks for end positions in the starting FEN.
        AIHandles.InitialMemoryUsage = (Process.GetCurrentProcess()).WorkingSet64 'Calibrates the initial memory usage of the program.
        If GameMode < 3 Then
            If GameMode = 0 Then
                Dim FirstRemoteModeSession As Boolean = System.IO.File.Exists(GlobalConstants.StartupPath & "\Assets\User\FirstRemoteModeSession.txt")
                If (Not FirstRemoteModeSession) OrElse InfoBtn_Click() = 6 Then
                    If FirstRemoteModeSession Then System.IO.File.Delete(GlobalConstants.StartupPath & "\Assets\User\FirstRemoteModeSession.txt")
                    RemoteModeBtn.Enabled = True
                    PreviousFEN = StartingFEN
                    GameRunning = False
                End If
            ElseIf GameMode = 1 AndAlso UserPlayer <> PlayerTurn Then
                'The AI needs to make the first move.
                InitialiseAISystem()
                PreviousFEN = CurrentFEN
            Else 'Prevents the user from undoing the starting move.
                PreviousFEN = StartingFEN
                'Notifies the user that they are to make the first move.
                If GameMode = 1 Then Me.Text = "[Your Turn...]  " + GlobalConstants.ProgramName
            End If
            InputTextBox.Text = StartingFEN
            InputTextBox.Select(InputTextBox.Text.Length, 0) 'Stops the TextBox from being highlighted.
        ElseIf GameMode = 4 Then
            InputTextBox.Select(InputTextBox.Text.Length, 0) 'Stops the TextBox from being highlighted.
            PlayNextPuzzleMove(0, True)
            RunAIOnPuzzle()
        ElseIf GameMode >= 5 Then
            Me.WLeaderBoardGrid.ClearSelection() 'Stops the Leaderboard from being highlighted.
        Else
            PreviousFEN = StartingFEN 'if CurrentFEN = PreviousFEN then the user cannot undo a move.
        End If
    End Sub



    'Subroutine that assigns all the PictureBoxes with their associated images.
    Private Sub AssignImagesToPieces()
        'All images are taken from the 'cburnett' piece theme of Lichess.org - an open source, charity chess website.
        'Please see here for information about using these assets: https://lichess.org/contact#help-authorize.
        Dim Colour As String
        For Each Piece In PieceArray
            Colour = Piece.Name(0)
            Select Case Piece.Name(1)
                Case "K"
                    Piece.Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\" & Colour & "King.png")
                Case "Q"
                    Piece.Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\" & Colour & "Queen.png")
                Case "B"
                    Piece.Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\" & Colour & "Bishop.png")
                Case "N"
                    Piece.Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\" & Colour & "Knight.png")
                Case "R"
                    Piece.Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\" & Colour & "Rook.png")
                Case "P"
                    Piece.Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\" & Colour & "Pawn.png")
            End Select
        Next
    End Sub


    'Subroutine that calibrates the settings of the user. Can be called by the settings form when settings are modified.
    Public Sub LoadUserProfile()
        'Sets the program settings (from txt file). If all else fails, use the default settings.
        Dim ColourScheme, TempGeneralOptions As String
        Try
            FileOpen(1, GlobalConstants.StartupPath & "\Assets\User\UserProfile.txt", OpenMode.Input)
            ColourScheme = LineInput(1)
            AnimationSpeed = Val(LineInput(1))
            TempGeneralOptions = LineInput(1)
            Dim temp As String = TempGeneralOptions(7) 'Tests that the General Options are the correct length.
            'Touch Move rule is only supported in the 1-Player and 2-Player modes - disable the control otherwise.
            If GameMode >= 3 Then TempGeneralOptions = TempGeneralOptions.Remove(5, 1).Insert(5, "F")
            'Invisible pieces are not supported in the training modes - forcibly disable this control.
            If GameMode > 3 Then TempGeneralOptions = TempGeneralOptions.Remove(6, 1).Insert(6, "F")
            If GameMode > 1 Then AIHandles.FixedSearchDepth = Val(LineInput(1))
        Catch ex As Exception
            'Default settings.
            ColourScheme = "def"
            AnimationSpeed = GlobalConstants.DefaultAnimationSpeed
            TempGeneralOptions = GlobalConstants.DefaultGeneralOptions
            AIHandles.FixedSearchDepth = 0
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Error in retrieving User Profile - reverting to default settings.")
            Console.ForegroundColor = ConsoleColor.White
        End Try
        FileClose(1)

        If GeneralOptions(5) <> TempGeneralOptions(5) Then
            PieceMoving.LockedPiece = "" 'Resets locked piece.
            UndoMove.Enabled = Not (TempGeneralOptions(5) = "T")
        End If
        'Toggles Hammad Mode.
        If GeneralOptions(7) <> TempGeneralOptions(7) Then
            If TempGeneralOptions(7) = "T" Then
                SearchSettings.ReturnBestMove = False
                If CheckLabel.Text = "Checkmate!" Then CheckLabel.Text = " king dead. "
            Else
                SearchSettings.ReturnBestMove = True
                If CheckLabel.Text = " king dead. " Then CheckLabel.Text = "Checkmate!"
            End If
        End If

        'Calibrates General Options
        If GeneralOptions(6) <> TempGeneralOptions(6) Then
            'Invisible pieces setting modified...
            GeneralOptions = TempGeneralOptions
            If TempGeneralOptions(6) = "T" Then
                'Removes images from pieces.
                For Each Box In PieceArray
                    Box.Image = Nothing
                Next
            Else
                'Restores images to pieces.
                AssignImagesToPieces()
            End If
        Else
            GeneralOptions = TempGeneralOptions
        End If

        'Sets Primary & Secondary Colour depending on ColourScheme.
        Select Case LCase(ColourScheme)
            Case "blu"
                PrimaryColour = Color.LightSteelBlue
                SecondaryColour = Color.AliceBlue
            Case "bl2"
                PrimaryColour = Color.SlateGray
                SecondaryColour = Color.Silver
            Case "gld"
                PrimaryColour = Color.Goldenrod
                SecondaryColour = Color.LemonChiffon
            Case "grn"
                PrimaryColour = Color.DarkSeaGreen
                SecondaryColour = Color.LavenderBlush
            Case "red"
                PrimaryColour = Color.FromArgb(195, 55, 55)
                SecondaryColour = Color.LavenderBlush
            Case "ppl"
                PrimaryColour = Color.MediumPurple
                SecondaryColour = Color.MistyRose
            Case "mon"
                PrimaryColour = Color.DimGray
                SecondaryColour = Color.Silver
            Case Else 'def
                PrimaryColour = Color.Peru
                SecondaryColour = Color.FloralWhite
        End Select
        Checkerboard.Refresh()
    End Sub




    'Function that detects some errors in a user-entered FEN string, given the below rules:
    Private Function FENErrorDetection(ByVal FEN As String, ByVal OutputToBox As Boolean, ByRef FENErrorMessage As String) As Boolean
        'If OutputToBox = True then the error message is stored in InputTextBox. If it is false then the message is stored in FENErrorMessage.
        Dim MaxWQueens, MaxBQueens, WKingCount, BKingCount As Byte
        If FEN.Length > 14 AndAlso FEN.Length < 90 Then 'Length validation
            'Checks if each player has a king.
            For n = 0 To Len(FEN) - 1
                If FEN(n) = " " Then Exit For 'Early exit clause.
                If FEN(n) = "P" OrElse FEN(n) = "Q" Then
                    MaxWQueens += 1
                ElseIf FEN(n) = "p" OrElse FEN(n) = "q" Then
                    MaxBQueens += 1
                ElseIf FEN(n) = "K" Then
                    WKingCount += 1
                ElseIf FEN(n) = "k" Then
                    BKingCount += 1
                End If
            Next
            'Displays appropriate error messages.
            If Not (WKingCount = 1 AndAlso BKingCount = 1) Then
                If OutputToBox Then
                    InvalidInput = FEN
                    InputTextBox.Text = "Position Rejected - Invalid number of Kings. Please Input a Genuinine FEN and try again."
                    UndoFENChange.Visible = True
                Else 'Returns message as string.
                    FENErrorMessage = "Position Rejected - Invalid number of Kings."
                End If
                Return False
            End If

            'Has passed all tests :).
            Return True
        Else
            If OutputToBox Then
                InvalidInput = FEN
                InputTextBox.Text = "Position Rejected - Invalid FEN Length. Please Input a Genuinine FEN and try again."
                UndoFENChange.Visible = True
            Else
                FENErrorMessage = "Position Rejected - Invalid FEN Length."
            End If
            Return False
        End If
    End Function

    'Function that checks for invalid board positions - part of FEN Error Detection: checks for invalid checks & king positions.
    'Returns True if the game state is invalid.
    Private Function CheckForInvalidGameStates() As Boolean
        If MasterWInCheck >= 128 AndAlso Not PlayerTurn Then
            'If White is in check, and it is black to move, then black can take white's king. This is illegal.
            GameRunning = False
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("The Game has Ended. Cause = Invalid Check.")
            CheckForInvalidGameStates = True
        ElseIf MasterBInCheck >= 128 AndAlso PlayerTurn Then
            'If Black is in check, and it is white to move, then white can take black's king. This is illegal.
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("The Game has Ended. Cause = Invalid Check.")
            GameRunning = False
            CheckForInvalidGameStates = True
        ElseIf Math.Abs(Val(MasterWKPos(0)) - Val(MasterBKPos(0))) <= 1 AndAlso Math.Abs(Val(MasterWKPos(1)) - Val(MasterBKPos(1))) <= 1 Then
            'Kings are too close together.
            GameRunning = False
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("The Game has Ended. Cause = Invalid King Positions.")
            CheckLabel.Text = " Stalemate! "
            If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
            CheckForInvalidGameStates = True
        Else
            CheckForInvalidGameStates = False
        End If
        Console.ForegroundColor = ConsoleColor.White
    End Function


    'Subroutine that formats & validates any moves being entered into the system via the InputTextBox, along with performing the move itself.
    'Supports multiple moves.
    Private Sub EnterMovesIntoSystem(ByVal Moves As String, ByVal NeedsFormatting As Boolean, OutputToTextBox As Boolean)
        'Formats PGN Moves. Some PGN Moves also contain move numbers - my program needs to remove these to interpret the moves.
        Dim FormattedPGN As String = ""
        If NeedsFormatting Then
            Try
                Moves = Regex.Replace(Moves, "[\(\[\{].*?[\)\]\}]", String.Empty) 'Removes pairs of brackets, and everything inside them.
                Moves = Moves.Replace(",", " ") 'Generalises format.
                Moves = Regex.Replace(Moves, "\s{2,}", " ").Trim(" ") 'Puts everything onto one line, and removes double-spaces.

                For n = 0 To Moves.Length - 1
                    Select Case Moves(n)
                        Case "0" To "9"
                            If n = Moves.Length - 1 OrElse Not (Moves(n + 1) = "." OrElse Moves(n + 1) = "-" OrElse Moves(n + 1) = "/" OrElse (Moves(n + 1) >= "0" AndAlso Moves(n + 1) <= "9")) Then FormattedPGN &= Moves(n)
                        Case "#", "-", "/"
                            'End of the PGN sequence.
                            If Moves(n) = "-" Then
                                If Moves(n + 1) = "O" Then
                                    FormattedPGN &= Moves(n)
                                    Exit Select
                                End If
                            End If
                            Exit For
                        Case "."
                            If n = Moves.Length - 1 Then FormattedPGN = FormattedPGN.TrimEnd(" ")
                        Case " "
                            If Moves(n - 1) <> "." Then FormattedPGN &= Moves(n)
                        Case Else
                            FormattedPGN &= Moves(n)
                    End Select
                Next
                If FormattedPGN = "" Then Throw New Exception("Invalid Input.")
            Catch ex As Exception
                'Unable to format moves - assume input is invalid.
                If OutputToTextBox Then
                    InvalidInput = InputTextBox.Text
                    UndoFENChange.Visible = True
                    InputTextBox.Text = "Move Unable to be Formatted. Please Input a valid set of Moves and try again."
                Else
                    Console.ForegroundColor = ConsoleColor.DarkRed
                    Console.WriteLine("Move Unable to be Formatted.")
                    Console.ForegroundColor = ConsoleColor.White
                End If
                Exit Sub
            End Try
            FormattedPGN = FormattedPGN.TrimEnd(" ")
            If OutputToTextBox Then InputTextBox.Text = FormattedPGN 'Outputs formatted PGN to InputTextBox.
        End If


        'Begins to interpret the moves.
        FormattedPGN &= " "
        Dim TempPGNMove As String
        Dim TempMove As New Move
        Dim FirstMove As Boolean = True
        Dim IsLegalMove As Boolean


        Do
            If Not FirstMove Then Thread.Sleep(25) 'Gives the user time to appreciate the moves being made.
            For n = 0 To FormattedPGN.Length - 1
                If FormattedPGN(n) = " " Then
                    'Isolates the PGN Move.
                    TempPGNMove = FormattedPGN.Substring(0, n)
                    FormattedPGN = FormattedPGN.Substring(n + 1)
                    Try
                        If PlayerTurn Then
                            'Attempts to convert the PGN move to my program's Move format.
                            TempMove = Helper.ConvertToMove(TempPGNMove, MasterBoard, True, Helper.ConvertStringToBitCoor(MasterWKPos), MasterWhiteTFTable)
                            If TempMove.Code = "a" Then
                                Throw New Exception("Invalid Move.")
                            ElseIf TempMove.Code = "c" Then
                                Throw New Exception("Collision.")
                            End If
                            'Checks for all the legal moves that this piece can make, then compare these to the user's input move.
                            PieceMoving.LegalMoves = MainAI.ReturnPiecesLegalMoves(TempMove.OldMoveX, TempMove.OldMoveY)
                            IsLegalMove = False
                            If PieceMoving.LegalMoves IsNot Nothing Then
                                For m = 0 To PieceMoving.LegalMoves.Length - 1
                                    If TempMove.NewMoveX & TempMove.NewMoveY = PieceMoving.LegalMoves(m) Then
                                        'Move found - break out of loop.
                                        IsLegalMove = True
                                        Exit For
                                    End If
                                Next
                            End If
                            If Not IsLegalMove Then Throw New Exception("Illegal Move.")
                            'Performs the move.
                            AnimateMove(TempMove)
                            MakeMove(MasterBoard, TempMove, MasterWCanCastle, MasterWKPos, MasterEnPassant, GeneralOptions(0) = "T")
                        Else 'Similar code for the black pieces.
                            TempMove = Helper.ConvertToMove(TempPGNMove, MasterBoard, False, Helper.ConvertStringToBitCoor(MasterBKPos), MasterBlackTFTable)
                            If TempMove.Code = "a" Then
                                Throw New Exception("Invalid Move.")
                            ElseIf TempMove.Code = "c" Then
                                Throw New Exception("Collision.")
                            End If
                            PieceMoving.LegalMoves = MainAI.ReturnPiecesLegalMoves(TempMove.OldMoveX, TempMove.OldMoveY)
                            IsLegalMove = False
                            If PieceMoving.LegalMoves IsNot Nothing Then
                                For m = 0 To PieceMoving.LegalMoves.Length - 1
                                    If TempMove.NewMoveX & TempMove.NewMoveY = PieceMoving.LegalMoves(m) Then
                                        IsLegalMove = True
                                        Exit For
                                    End If
                                Next
                            End If
                            If Not IsLegalMove Then Throw New Exception("Illegal Move.")
                            AnimateMove(TempMove)
                            MakeMove(MasterBoard, TempMove, MasterBCanCastle, MasterBKPos, MasterEnPassant, GeneralOptions(0) = "T")
                        End If
                    Catch ex As Exception
                        'Unable to perform move - assume move is invalid.
                        'Revert position to undo the effects of the invalid move.
                        MasterBoard = Helper.FENConverter(CurrentFEN, MasterWCanCastle, MasterBCanCastle, MasterWKPos, MasterBKPos, MasterEnPassant, PlayerTurn)
                        DisplayPieces()
                        If Not FirstMove Then OutputDebugInfo()
                        'Constructs and outputs the error message.
                        Dim OutputResponce As String
                        If TempMove.Code = "a" Then OutputResponce = "Invalid" Else OutputResponce = "Illegal"
                        If OutputToTextBox Then
                            InvalidInput = InputTextBox.Text
                            UndoFENChange.Visible = True
                            If TempMove.Code = "c" Then
                                InputTextBox.Text = "Move (" & TempPGNMove & ") has Multiple Interpretations. Consider adding constraint(s) (eg: Nbd7)..."
                            Else
                                InputTextBox.Text = "Move (" & TempPGNMove & ") is " & OutputResponce & ". Please Input a valid set of Moves and try again."
                            End If
                        Else
                            Console.ForegroundColor = ConsoleColor.DarkRed
                            If TempMove.Code = "c" Then
                                Console.WriteLine("Move (" & TempPGNMove & ") has Multiple Interpretations. Consider adding constraint(s) (eg: Nbd7)...")
                            Else
                                Console.WriteLine("Move (" & TempPGNMove & ") is " & OutputResponce & ".")
                            End If
                            Console.ForegroundColor = ConsoleColor.White
                        End If
                        Exit Sub
                    End Try

                    If Not UndoFENChange.Visible Then 'Move is valid.
                        'Updates TFTable for appropriate player.
                        If PlayerTurn Then
                            MasterWInCheck = 0 'Player is no longer in check.
                            Helper.FixTFTable(MasterBoard, False, MasterBlackTFTable, Helper.ConvertStringToBitCoor(MasterBKPos), MasterBInCheck, MasterBCanCastle.CanICastle(), Helper.ConvertStringToBitCoor(MasterEnPassant))
                        Else
                            MasterBInCheck = 0 'Player is no longer in check.
                            Helper.FixTFTable(MasterBoard, True, MasterWhiteTFTable, Helper.ConvertStringToBitCoor(MasterWKPos), MasterWInCheck, MasterWCanCastle.CanICastle(), Helper.ConvertStringToBitCoor(MasterEnPassant))
                        End If

                        'Update Previously Used Squares, then flips the board if necessary.
                        If GameMode = 3 AndAlso FirstMove Then
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
                        Checkerboard.Refresh()
                        EditCheckText()

                        'Ends the turn, then calculates the new position's FEN.
                        PlayerTurn = Not PlayerTurn
                        If FirstMove Then PreviousFEN = CurrentFEN
                        CurrentFEN = Helper.ConvertToFEN(MasterBoard, MasterWCanCastle, MasterBCanCastle, Helper.ConvertStringToBitCoor(MasterEnPassant), PlayerTurn)
                        MainAI.Reconfigure(CurrentFEN, False) 'Recalibrates AI.
                        'If the player has been put in check, and has not been checkmated, then add the + symbol to the end of the move.
                        If (MasterWInCheck >= 128 OrElse MasterBInCheck >= 128) AndAlso GameRunning AndAlso TempPGNMove.Last() <> "+" Then TempPGNMove &= "+"
                        BoardHistory.PushPGN(TempPGNMove, FirstMove)
                        EnforceEndStates()

                        FirstMove = False
                        Exit For
                    End If
                End If

            Next

        Loop While Len(FormattedPGN) > 0 AndAlso GameRunning
        'Once all the moves have been made, output the new board position.
        OutputDebugInfo()
    End Sub





    'Subroutine that makes a move on the board, given coordinates. Includes castling (& rights) and pawn promotion.
    'Note: This algorithm is very similar to the MakeMove subroutine in the CoreMethods Class, however here material
    'count is not taken into account, and there is an option for sounds to be played when a move is made. This is removed
    'in the CoreMethods version of this algorithm for efficiency purposes for NegaMax (less IF statements).
    Private Sub MakeMove(ByVal Board(,) As Char, ByVal TempMove As Move, ByRef CanCastle As CanCastle, ByRef KPos As String, ByRef EnPassant As String, ByVal MakeSounds As Boolean)
        Dim OldCoorX As String = TempMove.OldMoveX
        Dim OldCoorY As String = TempMove.OldMoveY
        Dim NewCoorX As String = TempMove.NewMoveX
        Dim NewCoorY As String = TempMove.NewMoveY

        Dim TempPiece As Char = Board(OldCoorX, OldCoorY)
        Dim HasEnPassanted As Boolean
        If MakeSounds Then Sound_Move.Play()
        If Char.IsUpper(TempPiece) Then
            If TempPiece = "P" Then
                'Code for Promoting Pawns and En Passant. Also increments the material count.
                If NewCoorY = 0 Then
                    TempPiece = If(TempMove.Code = "", "Q", TempMove.Code)
                ElseIf NewCoorX & NewCoorY = EnPassant Then
                    Board(NewCoorX, 3) = " "
                    If MakeSounds Then Sound_Capture.Play()
                ElseIf OldCoorY = 6 AndAlso NewCoorY = 4 AndAlso (Board(Math.Max(NewCoorX - 1, 0), 4) = "p" OrElse Board(Math.Min(NewCoorX + 1, 7), 4) = "p") Then
                    'EnPassant Creation.
                    EnPassant = NewCoorX & 5
                    HasEnPassanted = True
                End If
                'If piece is a Rook, part of Castling is disabled (depending on which Rook has moved).
            ElseIf TempPiece = "R" AndAlso CanCastle.CanICastle Then
                If CanCastle.KS AndAlso OldCoorX = 7 AndAlso OldCoorY = 7 Then
                    CanCastle.KS = False
                ElseIf CanCastle.QS AndAlso OldCoorX = 0 AndAlso OldCoorY = 7 Then
                    CanCastle.QS = False
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
                CanCastle.CannotCastle()
            End If
        Else
            'Identical Code for the Black Pieces.
            If TempPiece = "p" Then
                If NewCoorY = 7 Then
                    TempPiece = If(TempMove.Code = "", "q", LCase(TempMove.Code))
                ElseIf NewCoorX & NewCoorY = EnPassant Then
                    Board(NewCoorX, 4) = " "
                    If MakeSounds Then Sound_Capture.Play()
                ElseIf OldCoorY = 1 AndAlso NewCoorY = 3 AndAlso (Board(Math.Max(NewCoorX - 1, 0), 3) = "P" OrElse Board(Math.Min(NewCoorX + 1, 7), 3) = "P") Then
                    'EnPassant Creation.
                    EnPassant = NewCoorX & 2
                    HasEnPassanted = True
                End If
            ElseIf TempPiece = "r" AndAlso CanCastle.CanICastle Then
                If CanCastle.KS AndAlso OldCoorX = 7 AndAlso OldCoorY = 0 Then
                    CanCastle.KS = False
                ElseIf CanCastle.QS AndAlso OldCoorX = 0 AndAlso OldCoorY = 0 Then
                    CanCastle.QS = False
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
                CanCastle.CannotCastle()
            End If
        End If

        If Not (EnPassant = "-" OrElse HasEnPassanted) Then EnPassant = "-" 'Removal of EnPassant (if required).
        'At the end of the subroutine, the Piece is placed at the new coordinates, and the old position is cleared.
        'If the new position contains a piece, then the material count is updated for only that piece.
        If Board(NewCoorX, NewCoorY) <> " " Then
            If Board(NewCoorX, NewCoorY) = "R"c AndAlso MasterWCanCastle.CanICastle() Then
                If MasterWCanCastle.KS AndAlso NewCoorY = 7 AndAlso NewCoorX = 7 Then
                    'KS Rook has been captured - king can no longer castle KS.
                    MasterWCanCastle.KS = False
                ElseIf MasterWCanCastle.QS AndAlso NewCoorY = 7 AndAlso NewCoorX = 0 Then
                    'QS Rook has been captured - king can no longer castle QS.
                    MasterWCanCastle.QS = False
                End If
            ElseIf Board(NewCoorX, NewCoorY) = "r"c AndAlso MasterBCanCastle.CanICastle() Then
                If MasterBCanCastle.KS AndAlso NewCoorY = 0 AndAlso NewCoorX = 7 Then
                    MasterBCanCastle.KS = False
                ElseIf MasterBCanCastle.QS AndAlso NewCoorY = 0 AndAlso NewCoorX = 0 Then
                    MasterBCanCastle.QS = False
                End If
            End If
            If MakeSounds Then Sound_Capture.Play()
        End If
        Board(NewCoorX, NewCoorY) = TempPiece
        Board(OldCoorX, OldCoorY) = " "
    End Sub




    'Checks for Checkmate & Stalemate are calculated by running the opponent AI on a special mode, where the
    'AI runs to a depth of 1 but makes no moves on the board. This determines if the opponent actually has any
    'legal moves (or if there is only 1 forced move), and if they don't, then then the game will end. This
    'algorithm also checks for draws by insufficient material, three-fold repetition, or 50-move rule. If the
    'procedure does detect any of these end states, then it stops the game and/or notifies the user (depending on gamemode).
    Private Function EnforceEndStates() As Char
        'Adds the new board position's hash value to the Game History.
        MasterZobristValue = Helper.ZobristHashPosition(MasterBoard, PlayerTurn, MasterWCanCastle, MasterBCanCastle, Helper.ConvertStringToBitCoor(MasterEnPassant))
        BoardHistory.PushZobrist(MasterZobristValue, False) 'NormalMove specifies if the move is a move that the user or the AI has made,
        'or whether it is a move formed by, for example, undoing the board position. Depending on which it is, GameHistory will store
        'the previous state to its buffer (allowing the state to be undone, if needed). Note that we do this for the PGN push rather than the Zobrist push,
        'as this happens before :) (and we don't want to do it twice).

        Dim TempMove As Move = MainAI.CheckForEndState() 'Checks for endstates in the current position.
        Select Case TempMove.Code
            Case "c" 'Position is checkmate.
                If GeneralOptions(7) = "T" Then
                    'When I told Hammad that I had created "Hammad Mode" for him, he was annoyed that I didn't
                    'include his famous catchphrase 'king dead' when a checkmate occured. Therefore I decided to
                    'honour his legacy lmao by including it here.
                    CheckLabel.Text = " king dead. "
                Else
                    CheckLabel.Text = "Checkmate!"
                End If
                If GeneralOptions(0) = "T" Then Sound_Checkmate.Play()
                GameRunning = False
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("The Game has Ended. Cause = Checkmate.")
                If GameMode < 3 Then NotifyGameEnd("c")
                Return "c"
            Case "s" 'Position is stalemate.
                CheckLabel.Text = " Stalemate! "
                If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
                GameRunning = False
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("The Game has Ended. Cause = Stalemate.")
                If GameMode < 3 Then NotifyGameEnd("d")
                Return "s"
        End Select

        'Draw by insufficient material is then checked for. In principle, if it is physically impossible for one
        'player to checkmate the other (such as king vs king, or king vs king + a knight / bishop), then the
        'position is delared 'dead' and the game ends in a draw.
        Dim TempMaterialCount As Integer() = Helper.CountMaterial(MasterBoard)
        If TempMaterialCount(0) + TempMaterialCount(1) = 0 Then 'only kings remain.
            CheckLabel.Text = "     Draw!     "
            If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
            GameRunning = False
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("The Game has Ended. Cause = Draw by Insufficient Material (K v K).")
            If GameMode < 3 Then NotifyGameEnd("d")
        ElseIf TempMaterialCount(0) + TempMaterialCount(1) = GlobalConstants.PieceWeight.Bishop Then 'could be king vs king + knight / bishop.
            For y = 0 To 7
                For x = 0 To 7
                    'scans for knights / bishops.
                    If UCase(MasterBoard(x, y)) = "B" OrElse UCase(MasterBoard(x, y)) = "N" Then
                        'Game ends in a draw.
                        CheckLabel.Text = "     Draw!     "
                        If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
                        GameRunning = False
                        Console.ForegroundColor = ConsoleColor.Red
                        Console.WriteLine("The Game has Ended. Cause = Draw by Insufficient Material (K v K+B/N).")
                        If GameMode < 3 Then NotifyGameEnd("d")
                        Return "d"
                    End If
                Next
            Next
        ElseIf TempMaterialCount(0) = GlobalConstants.PieceWeight.Bishop AndAlso TempMaterialCount(1) = GlobalConstants.PieceWeight.Bishop Then 'only possibility is king + bishop vs king + bishop (of same type).
            Dim NoOfBishopsFound As SByte
            Dim BishopType As Boolean 'True = Light, False = Dark
            For y = 0 To 7
                For x = 0 To 7
                    If UCase(MasterBoard(x, y)) = "B" Then
                        Select Case NoOfBishopsFound
                            Case 0
                                'Calculates if the bishop is on a light square or a dark square.
                                If (x Mod 2 = 0 AndAlso y Mod 2 = 0) OrElse (x Mod 2 = 1 AndAlso y Mod 2 = 1) Then
                                    BishopType = True
                                Else
                                    BishopType = False
                                End If
                            Case 1
                                'If this bishop is on the same colour complex (light / dark) as the previous bishop,
                                'then we end the game.
                                If (x Mod 2 = 0 AndAlso y Mod 2 = 0 AndAlso BishopType) OrElse (x Mod 2 = 1 AndAlso y Mod 2 = 1 AndAlso BishopType) OrElse (x Mod 2 = 1 AndAlso y Mod 2 = 0 AndAlso Not BishopType) OrElse (x Mod 2 = 0 AndAlso y Mod 2 = 1 AndAlso Not BishopType) Then
                                    CheckLabel.Text = "     Draw!     "
                                    If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
                                    GameRunning = False
                                    Console.ForegroundColor = ConsoleColor.Red
                                    Console.WriteLine("The Game has now Ended. Cause = Draw by Insufficient Material (K v K+B+B).")
                                    If GameMode < 3 Then NotifyGameEnd("d")
                                    Return "d"
                                End If
                        End Select
                        NoOfBishopsFound += 1
                    ElseIf Not (UCase(MasterBoard(x, y)) = "K" OrElse UCase(MasterBoard(x, y)) = " ") Then
                        'Another piece found - exit search.
                        NoOfBishopsFound = -1
                        Exit For
                    End If
                Next
                If NoOfBishopsFound = -1 Then Exit For
            Next
        End If

        'Checks for three-fold repetition and 50-move rule occurence, using BoardHistory arrays.
        If BoardHistory.CheckNoOfZobristOccurances(MasterZobristValue) = 3 OrElse BoardHistory.GetHalfSize() >= 100 Then
            'Position has ocurred three times - end game.
            CheckLabel.Text = "     Draw!     "
            If GeneralOptions(0) = "T" Then Sound_Stalemate.Play()
            GameRunning = False
            Console.ForegroundColor = ConsoleColor.Red
            Console.Write("The Game has Ended. Cause = Draw by ")
            If BoardHistory.GetHalfSize() >= 100 Then
                Console.WriteLine("50 Move Rule.")
            Else
                Console.WriteLine("Three-fold Repetition.")
            End If
            If GameMode < 3 Then NotifyGameEnd("d")
            Return "d"
        End If

        Return TempMove.Code
    End Function

    'Notifies the user if the game has concluded - giving them an appropriate message and an option to play again.
    'If they deny, then they are sent back to the main menu.
    Private Sub NotifyGameEnd(ByVal EndState As Char)
        Me.Text = GlobalConstants.ProgramName 'Resets Program Name.
        Console.ForegroundColor = ConsoleColor.White
        Dim Text As String
        If GameMode = 1 Then
            If EndState = "c" Then
                'Checks if the player or the AI is the winner of the game.
                If (MasterWInCheck >= 128 AndAlso Not UserPlayer) OrElse (MasterBInCheck >= 128 AndAlso UserPlayer) Then
                    If GeneralOptions(7) = "T" Then
                        Text = "wahaaaaaayyyyyy!!!!"
                    Else
                        Text = "Congratulations - You have Beaten the AI!"
                    End If
                Else
                    If GeneralOptions(7) = "T" Then
                        Text = "haha, loser."
                    Else
                        Text = "Unfortunately, the AI has Beaten you."
                    End If
                End If
            Else
                Text = "The Game has Concluded in a Draw!"
            End If
        Else 'Gives unbias comment.
            Text = "The Game has Concluded"
            If EndState = "c" Then
                If MasterBInCheck >= 128 Then
                    Text &= ", with White Victorious!"
                Else
                    Text &= ", with Black Victorious!"
                End If
            Else
                Text &= " in a Draw!"
            End If
        End If

        Application.DoEvents() 'Finishes making the move.
        Thread.Sleep(1000)
        'Displays popup message.
        Dim UserOption As Byte = MsgBox(Text & vbCr & "Would you Like to Play Again?", vbInformation + vbYesNoCancel + vbApplicationModal)
        If UserOption = 6 Then
            'User wants to play again - reset the board.
            If GameMode = 0 Then ManuallyDisconnectFromInterface() 'User wants to play again - abort the Interface Connection.
            Reset_Btn_Click()
            PreviousFEN = CurrentFEN
        ElseIf UserOption = 7 Then 'Quit.
            ExitBtn_Click()
        ElseIf GameMode < 3 Then
            UndoMove.Enabled = False
        End If
    End Sub

End Class
