'Class containing remote mode innit.
Imports System.Data.SqlClient
Imports System.Diagnostics.Eventing.Reader
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Threading
Imports System.Timers

'Class holding all the information for the 'Remote Mode' feature (ie: GameMode 0). This locates any Chess Interface (no matter the size, location, board colours
'/ themes (to a limit), piece designs, Windows Scaling Sizes, etc) located on the user's screen (except this program! ;D), then connects to it by locating &
'storing all the pieces. The user Is Then able To play On this Interface, either Using their mouse Or automatically via my AI, And the program will move the
'user's mouse instantly to play their (or the AI's) moves on this Interface. We can also detect a move of changes between the Interface And its own position,a
'and make the move On the GUI automatically To calibrate the two positions. Very useful for calculating the strength of my AI using Stockfish!! :D
Partial Public Class Chess 'Remote Mode

    'Structure containing all the variables specific to RemoteMode
    Private RemoteMode As RemoteModeInfo
    Public Structure RemoteModeInfo
        Public IsEnabled As Boolean
        Public AIEnabled As Boolean 'Is the AI interacting / playing with the GUI, or is the user?
        Public ConnectedToInterface As Boolean

        'Objects holding the information of the screen to connect to, along with that of capturing the screen.
        Public ScreenCapture As Bitmap
        Public g As Graphics
        Public BitmapWidth, BitmapHeight As Integer
        Public ScaledRes As Rectangle 'If the Windows scaling of the screen is set to, for example, 125%, we find and store this data, so that
        'our pixel operations are specific to the givn screen.
        Public InterfaceScreen As Screen
        Public InterfaceStartX, InterfaceStartY As UInt16
        Public InterfaceSquareSize As UInt16

        Public SCBytes() As Byte 'Holds the bytes of all the pixels on the screen (for each pixel, the data is in the form B,G,R).
        Public UsingBComponent As Boolean 'We want to detect the chessboard, no matter the colour. As SCBytes() can be easily searched on in a given
        'colour space (using modulus operations), we choose the colour space that gives the most accurate connection.
        Public Tolerance As Byte 'Defines how close a specific pixel needs to be (in colour / brightness) to be classified under a certain category (eg: a white square).
        Public WhiteColour, BlackColour As Int16 'Brightness of the light & dark squares on the chessboard.
        Public ScannedPieces() As Int16 '0 = light knight, 1 = dark knight, 2 = light queen, 3 = dark queen.

        'Objects for making, and verifying a move made on the interface.
        Public MoveDelay As Decimal
        Public RecentlyPlayedMove As Move
        Public MoveVerified As SByte '-1 = no move has been played, anything else refers to the amount of attempts to play, and verify the move, up until
        'some threshold, where we terminate the connection.
    End Structure

    'These libraries allow us to access specific screen data, that is not currently available (specifically, scaling).
    <DllImport("user32.dll")>
    Public Shared Function SetProcessDpiAwarenessContext(ByVal value As Integer) As Boolean
    End Function
    <DllImport("user32.dll")>
    Public Shared Function MonitorFromPoint(ByVal pt As Point, ByVal flags As UInteger) As IntPtr
    End Function
    <DllImport("shcore.dll")>
    Public Shared Function GetDpiForMonitor(ByVal hMonitor As IntPtr, ByVal dpiType As Integer, ByRef dpiX As UInteger, ByRef dpiY As UInteger) As Integer
    End Function

    'These libraries allow us to access & move the mouse.
    Private Declare Auto Function SetCursorPos Lib "User32.dll" (ByVal x As UInt16, ByVal y As UInt16) As Long
    Private Declare Auto Function GetCursorPos Lib "User32.dll" (ByRef P As Point) As Long
    Private Declare Auto Sub mouse_event Lib "User32.dll" (dwFlags As UInt16, dx As UInt16, dy As UInt16, dwData As UInt16, dwExtraInfo As Int16)



    'Main algorithm for instantiating a connection to the interface. We locate a candidate screen, take a screenshot, and scan each nth row, looking for
    'repeating patterns of the same colours, representing light & dark squares. After verifying this in full, we can use this data to isolate, and record
    'the position of, the chessboard. Further code is then used to scan & map the pieces.
    Private Sub RemoteModeBtn_Click() Handles RemoteModeBtn.Click
        If RemoteMode.ConnectedToInterface Then
            If RemoteModeBtn.Text = "Commense Game" Then
                'Start the game!
                RemoteModeBtn.Text = "Abort Connection"
                GameRunning = True
                RemoteMode.MoveVerified = -1

                Console.ForegroundColor = ConsoleColor.Cyan
                Console.WriteLine(vbCrLf & "Commensing Game...")
                Console.ForegroundColor = ConsoleColor.Blue
                'We either need to wait for the user, wait for the opponent, or start the AI search.
                If PlayerTurn Xor OrientForWhite Then
                    ChessTimer.Start()
                    Console.WriteLine("Waiting for Opponent's Move...")
                    Console.ForegroundColor = ConsoleColor.White
                    Me.Text = "[Waiting...]  " & GlobalConstants.ProgramName
                ElseIf RemoteMode.AIEnabled Then
                    Console.WriteLine("Performing AI Move...")
                    AITerminator.Enabled = True
                    InitialiseAISystem()
                Else
                    Console.WriteLine("Waiting for your Move...")
                    Console.ForegroundColor = ConsoleColor.White
                    Me.Text = "[Your Turn...]  " & GlobalConstants.ProgramName
                End If
            Else
                'The 'Abort' button has been pressed - terminate the connection.
                ChessTimer.Stop()
                GameRunning = False
                RemoteMode.ConnectedToInterface = False
                If ComputerIsSearching Then
                    MainAI.TERMINATESearch()
                ElseIf AIIsSearchingOnUsersTurn Then
                    MainAI.ABORTSearch()
                End If
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine(vbCrLf & "Connection Terminated.")
                Console.ForegroundColor = ConsoleColor.White
                RemoteModeBtn.Text = "Connect To Interface"
            End If
        Else
            Console.ForegroundColor = ConsoleColor.Cyan
            Console.Write(vbCrLf & vbCrLf & "Searching for Interface")

            Dim InterfaceConnectStopwatch As New Stopwatch
            InterfaceConnectStopwatch.Start()
            'Finds the screen to connect to.
            Dim ScreenCount As Integer = Screen.AllScreens.Length
            Select Case ScreenCount
                Case 1
                    RemoteMode.InterfaceScreen = Screen.PrimaryScreen
                Case 2
                    'Dual monitor setup detected. Select the screen that this program is not present on.
                    RemoteMode.InterfaceScreen = Screen.AllScreens.FirstOrDefault(Function(s) s.DeviceName <> Screen.FromControl(Form.ActiveForm).DeviceName)
                Case Else
                    'Too many screens to handle.
                    MsgBox("Error when Detecting Interface: Too Many Monitors Detected (Maximum of 2 Monitors can be Processed).")
                    RemoteMode.InterfaceScreen = Nothing
            End Select
            If RemoteMode.InterfaceScreen IsNot Nothing Then 'We have found a screen.
                Console.Write(" on " & RemoteMode.InterfaceScreen.DeviceName.Substring(4) & "... ")

                'Determines the scaling of the monitor, using the imported libraries.
                SetProcessDpiAwarenessContext(-4) 'where -4 = DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2.
                Dim DpiX, DpiY As UInteger
                Dim hMonitor As IntPtr = MonitorFromPoint(RemoteMode.InterfaceScreen.Bounds.Location, 2)
                GetDpiForMonitor(hMonitor, 0, DpiX, DpiY)
                RemoteMode.ScaledRes = RemoteMode.InterfaceScreen.Bounds
                RemoteMode.ScaledRes.Width *= DpiX / 96.0F
                RemoteMode.ScaledRes.Height *= DpiX / 96.0F
                Console.WriteLine("(Scaling Factor: {" & DpiX / 96.0F & ", " & DpiX / 96.0F & "})")
                Console.ForegroundColor = ConsoleColor.White

                'Sets up the screen capture of this screen, then takes the first screenshot to save into SCBytes.
                RemoteMode.ScreenCapture = New Bitmap(RemoteMode.ScaledRes.Width, RemoteMode.ScaledRes.Height, PixelFormat.Format24bppRgb)
                RemoteMode.g = Graphics.FromImage(RemoteMode.ScreenCapture)
                RemoteMode.g.CopyFromScreen(RemoteMode.ScaledRes.X, RemoteMode.ScaledRes.Y, 0, 0, RemoteMode.ScaledRes.Size, CopyPixelOperation.SourceCopy)
                'RemoteMode.ScreenCapture.Save(GlobalConstants.StartupPath & "bitmap.bmp", System.Drawing.Imaging.ImageFormat.Bmp)

                'To copy this bitmap to SCBytes, we need to lock the bytes, then unlock them afterwards. This allows for a safe transfer.
                Dim ScreenCaptureLock As BitmapData = RemoteMode.ScreenCapture.LockBits(New Rectangle(0, 0, RemoteMode.ScreenCapture.Width, RemoteMode.ScreenCapture.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb)
                RemoteMode.BitmapWidth = RemoteMode.ScreenCapture.Width
                RemoteMode.BitmapHeight = RemoteMode.ScreenCapture.Height
                Dim PixelCount As Integer = ScreenCaptureLock.Stride * RemoteMode.ScreenCapture.Height
                Dim SCBytes(PixelCount - 1) As Byte
                Marshal.Copy(ScreenCaptureLock.Scan0, SCBytes, 0, PixelCount) 'Copies to SCBytes.
                RemoteMode.ScreenCapture.UnlockBits(ScreenCaptureLock)
                RemoteMode.SCBytes = SCBytes

                'Prepares for the pixel scanning, using a strict tolerance & red colour scheme.
                RemoteMode.Tolerance = 5
                Dim StepSizeX As UInt16 = 15
                Dim StepSizeY As UInt16 = 3
                Dim RowByteArray(RemoteMode.ScreenCapture.Width \ StepSizeX) As Int16
                Dim RowMatchGuess, FirstIndex As UInt16
                Dim DeepScanFlag As UInt16 'Determines when we have calculated a candidate row. We use this to guess the width of each square.
                Dim InterfaceFound, ConnectError As Boolean
                Dim YInBounds, WidthOneUsed As Boolean
                RemoteMode.UsingBComponent = False
                While Not InterfaceFound
                    For y = 1 To RemoteMode.ScreenCapture.Height Step StepSizeY

                        'Determines if this current row contains an alternating, checkerboard-like pattern: (eg: 18 3 *255 199 255 199 ... 199* 9 42)
                        YInBounds = (ScreenCount = 1 AndAlso (Me.Location.Y <= y AndAlso y < (Me.Location.Y + Me.Size.Height)))
                        If PerformQuickPixelScan(RowByteArray, y, StepSizeX, RemoteMode.Tolerance, FirstIndex, YInBounds) Then
                            'Locates the exact line that the match is first present, to better determine the bounds of the chessboard.
                            'Note that on the very top row of the square, there cannot be any pieces in the way, so we can perform a valid search.
                            If PerformQuickPixelScan(RowByteArray, y - 2, StepSizeX, RemoteMode.Tolerance, FirstIndex, YInBounds) Then
                                RowMatchGuess = y - 2
                            ElseIf PerformQuickPixelScan(RowByteArray, y - 1, StepSizeX, RemoteMode.Tolerance, FirstIndex, YInBounds) Then
                                RowMatchGuess = y - 1
                            Else
                                RowMatchGuess = y
                            End If

                            'Performs Deep Scan on pixel data, to ensure that we haven't just got lucky, and that we really do have a checkerboard pattern.
                            '(ie: n pixels of uninterrupted 'WhiteColour', followed by n pixels of uninterrupted 'BlackColour', etc).
                            DeepScanFlag = 0
                            For x = FirstIndex * StepSizeX To RemoteMode.ScreenCapture.Width
                                WidthOneUsed = False

                                Select Case DeepScanFlag
                                    Case 0
                                        If Math.Abs(RemoteMode.WhiteColour - SCBytes(Convert2DTo1DIndex(x, RowMatchGuess))) < RemoteMode.Tolerance Then
                                            'We have found the first pixel that the a8 square starts on.
                                            DeepScanFlag = 1
                                        End If
                                    Case 1
                                        If Math.Abs(RemoteMode.BlackColour - SCBytes(Convert2DTo1DIndex(x, RowMatchGuess))) < RemoteMode.Tolerance Then
                                            'We have found the first pixel that the b8 square starts on - make a guess on the square size.
                                            DeepScanFlag = x
                                        End If
                                    Case Else
                                        If x >= 2 * DeepScanFlag Then Exit For 'The b8 square is significantly larger than the a8 square - invalid.
                                        If Math.Abs(RemoteMode.WhiteColour - SCBytes(Convert2DTo1DIndex(x, RowMatchGuess))) < RemoteMode.Tolerance Then
                                            'c8 square found.
                                            'We have a calculation for the size & location of square b8. Use this data to find the board coordinates.
                                            RemoteMode.InterfaceSquareSize = x - DeepScanFlag
                                            RemoteMode.InterfaceStartX = DeepScanFlag - RemoteMode.InterfaceSquareSize
                                            RemoteMode.InterfaceStartY = RowMatchGuess
                                            'If the interfact is very small, we need to be more precise & leniant with our mouse clicks - give the program more time.
                                            Select Case RemoteMode.InterfaceSquareSize
                                                Case >= 50
                                                    RemoteMode.MoveDelay = 0
                                                Case >= 25
                                                    RemoteMode.MoveDelay = 5
                                                Case Else
                                                    RemoteMode.MoveDelay = 10
                                            End Select

                                            If CheckInterfaceInBounds() Then
                                                'We've got a successful interface detection! Now, we need to connet & map all the pieces.
                                                InterfaceFound = True
                                                Console.ForegroundColor = ConsoleColor.Green
                                                Console.WriteLine("Interface Detected! {Corner: [" & RemoteMode.InterfaceStartX & "," & RemoteMode.InterfaceStartY & "], Square Size: " & RemoteMode.InterfaceSquareSize & ", White: " & RemoteMode.WhiteColour & ", Black: " & RemoteMode.BlackColour & "}")

                                                If DetectPiecesOnInterface() Then
                                                    RemoteMode.ConnectedToInterface = True
                                                    InterfaceConnectStopwatch.Stop()
                                                    Console.ForegroundColor = ConsoleColor.DarkGreen
                                                    Console.WriteLine("Successfully Connected to Interface in " & InterfaceConnectStopwatch.ElapsedMilliseconds & "ms. Ready to Play!")
                                                    Console.ForegroundColor = ConsoleColor.White
                                                    Exit For
                                                Else
                                                    'Something has gone wrong, and we weren't able to connect to the pieces - keep trying.
                                                    InterfaceFound = False
                                                    ConnectError = True
                                                    Exit For
                                                End If
                                            End If

                                        ElseIf Math.Abs(RemoteMode.BlackColour - SCBytes(Convert2DTo1DIndex(x, RowMatchGuess))) > RemoteMode.Tolerance Then
                                            If WidthOneUsed Then Exit For Else WidthOneUsed = True
                                            'We are unable to find the chessboard. Restart.
                                        End If
                                End Select

                            Next
                        End If
                        If RemoteMode.ConnectedToInterface OrElse ConnectError Then Exit For
                    Next
                    If Not InterfaceFound OrElse ConnectError Then
                        'Connection error.
                        Console.ForegroundColor = ConsoleColor.Red
                        If ConnectError Then
                            ConnectError = False
                            Console.Write("Unable to Connect to Interface. ")
                        Else
                            Console.Write("Unable to Locate Interface. ")
                        End If
                        'Try the entire search again using either a different colour component, or with a less strict tolerance. If we try all these things,
                        'we assume that there is no interface to connect to.
                        If RemoteMode.UsingBComponent Then
                            If RemoteMode.Tolerance = 5 Then
                                Console.WriteLine("Reattempting Search with a Wider Tolerance..." & vbCrLf)
                                RemoteMode.Tolerance = 25
                                RemoteMode.UsingBComponent = False
                            Else
                                Console.ForegroundColor = ConsoleColor.DarkRed
                                Console.WriteLine("Please Try Again...")
                                Exit While
                            End If
                        Else
                            Console.WriteLine("Reattempting Search using Blue Colour Component..." & vbCrLf)
                            RemoteMode.UsingBComponent = True
                        End If
                        Console.ForegroundColor = ConsoleColor.White
                    End If
                End While
            Else
                'Fail the search.
                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.WriteLine(vbCr & "Unable to Locate Valid Screen. Please Try Again...")
                Console.ForegroundColor = ConsoleColor.White
            End If
            InterfaceConnectStopwatch.Stop()

            If RemoteMode.ConnectedToInterface Then RemoteModeBtn.Text = "Commense Game" : OutputDebugInfo()
        End If
    End Sub

    'Function that converts a pixel on the screen (ie: [1056,923]) to an index in SCBytes, using that given colour spectrum.
    Private Function Convert2DTo1DIndex(ByVal x As Integer, ByVal y As Integer) As Integer
        'Only retrieves red / blue component :D.
        Return ((y - 1) * RemoteMode.BitmapWidth + x) * 3 - 1 + (2 * Val(RemoteMode.UsingBComponent))
    End Function

    'Function that validifies the size bounds we have put on the interface on the screen.
    Private Function CheckInterfaceInBounds() As Boolean
        'Final check: test the pixels between the a8 & b8 squares, along with the g1 & h1 squares, to ensure that the board bounds are correct.
        Dim Delta As UInt16 = 3 'Ie: how much lineance we give each square.
        Dim TestPixelX As UInt16 = RemoteMode.InterfaceStartX + RemoteMode.InterfaceSquareSize
        Dim TestPixelY As UInt16 = RemoteMode.InterfaceStartY
        'Dim DebugMode As Boolean = (Math.Abs(260 - RemoteMode.InterfaceStartY) < 3)

        If Not ((Math.Abs(RemoteMode.BlackColour - RemoteMode.SCBytes(Convert2DTo1DIndex(TestPixelX, TestPixelY - Delta))) > RemoteMode.Tolerance) AndAlso (Math.Abs(RemoteMode.WhiteColour - RemoteMode.SCBytes(Convert2DTo1DIndex(TestPixelX - Delta, TestPixelY + Delta))) < RemoteMode.Tolerance) AndAlso (Math.Abs(RemoteMode.BlackColour - RemoteMode.SCBytes(Convert2DTo1DIndex(TestPixelX, TestPixelY))) < RemoteMode.Tolerance)) Then
            'The a8 & b8 squares clip outside the bounds we have given - our bounds must be wrong, and we cannot assume we know where all the pieces are. Break out.
            Return False
        End If

        TestPixelX += RemoteMode.InterfaceSquareSize * 6
        TestPixelY += RemoteMode.InterfaceSquareSize * 8
        If (TestPixelX <= RemoteMode.ScreenCapture.Width AndAlso TestPixelY <= RemoteMode.ScreenCapture.Height) AndAlso (Math.Abs(RemoteMode.BlackColour - RemoteMode.SCBytes(Convert2DTo1DIndex(TestPixelX - Delta, TestPixelY - Delta))) < RemoteMode.Tolerance) AndAlso (Math.Abs(RemoteMode.WhiteColour - RemoteMode.SCBytes(Convert2DTo1DIndex(TestPixelX + Delta, TestPixelY - Delta))) < RemoteMode.Tolerance) AndAlso (Math.Abs(RemoteMode.BlackColour - RemoteMode.SCBytes(Convert2DTo1DIndex(TestPixelX, TestPixelY + Delta))) > RemoteMode.Tolerance) Then
            'The g1 & h1 squares clip outside the bounds we have given - our bounds must be wrong.
            Return True
        End If
        Return False
    End Function


    'Algorithm that scans a given row of pixels, looking for repeating patterns that may indicate a checkerboard pattern.
    'This algorithm is designed to be efficient, so that we can test it on many rows. The DeepPixelScan algorithm is much slower, and so we only want to
    'use it if we are _pretty_ sure the row is valid (hence the existence of this function).
    Private Function PerformQuickPixelScan(ByRef RowByteArray As Int16(), ByVal y As Integer, ByVal StepSize As UInt16, ByVal Tolerance As Byte, ByRef FirstIndex As UInt16, ByVal YInBounds As Boolean) As Boolean
        Dim NoOfArrayWrites, ByteArrayBuffer As Int16
        Dim FullToleranceMatch As Boolean
        Dim PreviousWidthOne As Boolean

        RowByteArray(0) = RemoteMode.SCBytes(Convert2DTo1DIndex(1, y))
        NoOfArrayWrites = 0
        For x = StepSize To RemoteMode.BitmapWidth Step StepSize
            If Not (YInBounds AndAlso (Me.Location.X <= x AndAlso x < (Me.Location.X + Me.Size.Width))) Then
                ByteArrayBuffer = RemoteMode.SCBytes(Convert2DTo1DIndex(x, y))
                'Forms RowByteArray, containing the pixel data for that current row (deleting repeated values: [255 255 255] -> [255]. Here, we assume
                'that we don't clip the colours between squraes).
                If Math.Abs(ByteArrayBuffer - RowByteArray(NoOfArrayWrites)) > Tolerance Then
                    'If the next pixel to scan does not match this pixel (ie: length = 1), then we may be in between squares. Overwrite this value.
                    If Not PreviousWidthOne Then NoOfArrayWrites += 1
                    RowByteArray(NoOfArrayWrites) = ByteArrayBuffer
                    PreviousWidthOne = True
                Else
                    PreviousWidthOne = False
                End If
            End If
        Next

        If NoOfArrayWrites >= 8 Then
            For i = 0 To NoOfArrayWrites - 7
                'Checks for valid colours of black & white squares, and the far-left square being white; the far-right square being black.
                If RowByteArray(i) < 250 AndAlso RowByteArray(i) > 150 AndAlso RowByteArray(i + 1) < RowByteArray(i) AndAlso (RowByteArray(i + 1) < 200) Then

                    FullToleranceMatch = True
                    'Checks if all the white squares are the same colour.
                    For n = i + 2 To i + 6 Step 2
                        If Math.Abs(RowByteArray(i) - RowByteArray(n)) > Tolerance Then
                            FullToleranceMatch = False
                            Exit For
                        End If
                    Next
                    If Math.Abs(RowByteArray(i) - RowByteArray(i + 8)) < Tolerance Then FullToleranceMatch = False 'Ie: beyond the 8th square is not a square.

                    If FullToleranceMatch Then
                        'Checks if all the black squares are the same colour.
                        For n = i + 3 To i + 7 Step 2
                            If Math.Abs(RowByteArray(i + 1) - RowByteArray(n)) > Tolerance Then
                                FullToleranceMatch = False
                                Exit For
                            End If
                        Next
                        If Math.Abs(RowByteArray(i + 1) - RowByteArray(i + 9)) < Tolerance Then FullToleranceMatch = False 'Ie: beyond the checkerboard are no more squares.

                        If FullToleranceMatch Then
                            'We may have a checkerboard!! However, we need to scan this row in further detail to make sure.
                            'Displays the pattern to the console.
                            Console.Write("Potential Find on Line " & y & ": {")
                            Dim RowByteLine As String = ""
                            For m = 0 To NoOfArrayWrites
                                RowByteLine &= RowByteArray(m) & ","
                            Next
                            Console.WriteLine(RowByteLine.TrimEnd(",") & "}")

                            'Calculates an estimate for the white & black squares of the interface, which will be required for later testing.
                            FirstIndex = i
                            RemoteMode.WhiteColour = RowByteArray(i)
                            RemoteMode.BlackColour = RowByteArray(i + 1)
                            Return True
                        End If
                    End If

                End If
            Next
        End If
        Return False 'The row is not valid.
    End Function


    'Algorithm that initiates the connection to the chess interface, that has been located previously in the RemoteModeBtn_Click() Method.
    'We do this by detecting for pieces that result from pawn promotions (knights & queens, in the case of my program), scanning these
    'pieces in full for later pawn promotions, then invoking the HandleScreenMove() sub, that detects & maps all the pieces on the screen
    '(as we do when we take a new screenshot to check for an opponent move).
    Private Function DetectPiecesOnInterface() As Boolean
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.WriteLine("Connecting to Interface...")
        Console.ForegroundColor = ConsoleColor.White

        'Searches for knights & queens on the board, that will be scanned.
        Dim PiecesToScan(3) As String
        For y = 0 To 7
            For x = 0 To 7
                If (MasterBoard(x, y) = "n" AndAlso OrientForWhite) OrElse (MasterBoard(x, y) = "N" AndAlso Not OrientForWhite) Then
                    If (y + x) Mod 2 = 0 Then
                        PiecesToScan(0) = x & y
                    Else
                        PiecesToScan(1) = x & y
                    End If
                ElseIf (MasterBoard(x, y) = "q" AndAlso OrientForWhite) OrElse (MasterBoard(x, y) = "Q" AndAlso Not OrientForWhite) Then
                    If (y + x) Mod 2 = 0 Then
                        PiecesToScan(2) = x & y
                    Else
                        PiecesToScan(3) = x & y
                    End If
                End If
            Next
        Next
        Dim AnyPiecesToScan As Boolean
        If Not (PiecesToScan(0) = "" OrElse PiecesToScan(1) = "") Then
            Console.WriteLine("Contrasting Knights Detected. Scanning Pieces to Calibrate Pawn Promotion Detection...")
            AnyPiecesToScan = True
        ElseIf PiecesToScan(2) = "" AndAlso PiecesToScan(3) = "" Then
            Console.WriteLine("Unable to Scan Promotion Pieces. Pawn Promotions may be Unreliable...")
            AnyPiecesToScan = False
        Else
            Console.WriteLine("Unable to Detect Contrasting Knights (Pawn Promotion Detection may be Unreliable). Scanning Pieces...")
            AnyPiecesToScan = True
        End If

        'Fully scans each piece that can be scanned (note that we distinguigh, for example a knight on a light square to one on a dark square, as this will
        'help us find an exact match for a pawn that promoted on any given square).
        Dim PieceScanValues(PiecesToScan.Length - 1) As Int16
        For n = 0 To PiecesToScan.Length - 1
            If PiecesToScan(n) <> "" Then PieceScanValues(n) = ScanPieceInFull(Val(PiecesToScan(n)(0)), Val(PiecesToScan(n)(1)))
        Next
        RemoteMode.ScannedPieces = PieceScanValues
        If AnyPiecesToScan Then
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Scan Successful. Scanning Interface Position...")
            Console.ForegroundColor = ConsoleColor.White
        End If

        DetectPiecesOnInterface = HandleScreenMove(False, False)
        If DetectPiecesOnInterface Then
            'We were able to connect to the pieces.
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Scan Complete - No Issues Detected.")
        End If
    End Function




    Private Sub RemoteModeTimer_Tick() Handles ChessTimer.Tick
        If GameMode = 0 Then
            If Control.ModifierKeys = Keys.Shift Then
                'In case of a catastrophic error, the user can press the SHIFT button at any time to suspend the game. The interface remains connected,
                'but the game (and hence, the mouse movements) will be paused.
                ChessTimer.Stop()
                GameRunning = False
                If AIIsSearchingOnUsersTurn Then MainAI.ABORTSearch()
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine(vbCrLf & "Connection Manually Terminated.")
                Console.ForegroundColor = ConsoleColor.White
                RemoteModeBtn.Text = "Commense Game"
            Else
                'Detects any changes on the board, that may represent an opponent's move.
                HandleScreenMove(True, True)
            End If
        End If
    End Sub

    'Method that disconnects from the interface, for use when the game is over, and for terminations by the user specifically.
    Private Sub ManuallyDisconnectFromInterface()
        ChessTimer.Stop()
        RemoteMode.ConnectedToInterface = False
        If AIIsSearchingOnUsersTurn Then MainAI.ABORTSearch()
        RemoteModeBtn.Text = "Connect To Interface"
        Console.ForegroundColor = ConsoleColor.DarkRed
        Console.WriteLine("Error: Lost Connection with Interface. Please Ensure all Configuration Steps have been Complete, and Reconnect...")
        Console.ForegroundColor = ConsoleColor.White
    End Sub



    'The main algorithm for connecting to, mapping, and finding changes in, the pieces on the chess interface.
    Private Function HandleScreenMove(ByVal TakeNewScreenshot As Boolean, ByVal GameInProgress As Boolean) As Boolean
        Dim TestMove As New Move
        'Checks the screen, to see if the opponent has made a move, or to see how many discrepencies there are between the Board as stored on this GUI,
        'and the board on the connected interface.
        If CheckInterfaceInBounds() Then
            TestMove = CheckScreenForMove(TakeNewScreenshot)
        Else
            TestMove.Code = "3"
        End If
        Select Case TestMove.Code
            Case "0" 'No change from the last position - everything is cool :D.
                Return True
            Case "3"
                'There have been too many discrepencies, or the user has moved the interface window / bounds, making the position invalid - terminate the game.
                If GameInProgress Then ManuallyDisconnectFromInterface()
                Return False
            Case Else 'A move has been made! Can be 1 discrepency, 2 discrepencies (meaning castling), or a pawn promotion.
                'Either way, we assume the move is valid. We play this move on the GUI, and commense the next move.
                ChessTimer.Stop()
                AnimateMove(TestMove)
                SubmitMoveIntoSystem(TestMove, GameInProgress)
                If GameInProgress Then
                    Console.ForegroundColor = ConsoleColor.Blue
                    If RemoteMode.AIEnabled Then
                        Console.WriteLine("Move Successfully Received. Playing AI Move...")
                        AITerminator.Enabled = True
                        InitialiseAISystem()
                    Else
                        Console.WriteLine("Move Successfully Received. Waiting for your Move...")
                        Me.Text = "[Your Turn...]  " & GlobalConstants.ProgramName
                    End If
                    Console.ForegroundColor = ConsoleColor.White
                End If
                Return True
        End Select
    End Function

    'Algorithm that, as the name suggests, scans the screen, looking for any discrepencies that may suggest that a move has been played by the opponent.
    'If there is such a move, we map it to an object and return it, and otherwise we return an error code (ie: the move is not valid, has multiple meanings,
    'too many pieces have been moved, etc).
    Private Function CheckScreenForMove(ByVal TakeNewScreenshot As Boolean) As Move
        If TakeNewScreenshot Then RemoteModeNewScreenshot()

        Dim RemoteMove As New Move
        Dim PiecesChanged(1) As String
        Dim NoOfPieceChanges As Byte = 0
        'Scans to see how many of the opponent's pieces have moved from their original positions.
        For y As SByte = 0 To 7
            For x As SByte = 0 To 7
                If (OrientForWhite AndAlso (Char.IsLower(MasterBoard(x, y)) AndAlso ScanPieceColour(x, y) <> "B")) OrElse (Not OrientForWhite AndAlso Char.IsUpper(MasterBoard(x, y)) AndAlso ScanPieceColour(x, y) <> "W") Then
                    'The piece has moved (ie: the new squre is not an enemy piece). Make note of this piece.
                    If NoOfPieceChanges < 2 Then
                        PiecesChanged(NoOfPieceChanges) = x & y
                        NoOfPieceChanges += 1
                    Else 'Too many moved pieces.
                        Console.ForegroundColor = ConsoleColor.Red
                        Console.WriteLine("Error when Scanning Position: Too Many Discrepancies Between Program & Interface.")
                        RemoteMove.Code = "3" 'Error code for too many discrepancies.
                        Return RemoteMove
                    End If
                End If
            Next
        Next

        Select Case NoOfPieceChanges
            Case 0 'No move has been made - easy peasy :D.
                RemoteMove.Code = "0"
            Case 1 'One piece has moved - attempt to link this with a legal move that that piece can make (using my AI), and store this as a move.
                Console.WriteLine("Detected Possible Discrepancy on Interface - Scanning...")
                Dim PieceLegalMoves() As String = MainAI.ReturnPiecesLegalMoves(PiecesChanged(0)(0), PiecesChanged(0)(1))
                Dim PieceEndPos As String = ""
                Dim PieceColour As Char = If(OrientForWhite, "B", "W")
                If PieceLegalMoves IsNot Nothing Then
                    For Each LegalMove In PieceLegalMoves
                        If ScanPieceColour(Val(LegalMove(0)), Val(LegalMove(1))) = PieceColour Then
                            'We have linked this discrepency with a move - return this.
                            If PieceEndPos = "" Then
                                PieceEndPos = LegalMove
                            Else
                                'Too many moves detected (hence ambiguity).
                                PieceEndPos = ""
                                Exit For
                            End If
                        End If
                    Next
                End If
                If PieceEndPos = "" Then
                    RemoteMove.Code = "3"
                Else
                    'Create the move resembling this discrepency.
                    RemoteMove.OldMoveX = PiecesChanged(0)(0)
                    RemoteMove.OldMoveY = PiecesChanged(0)(1)
                    RemoteMove.NewMoveX = PieceEndPos(0)
                    RemoteMove.NewMoveY = PieceEndPos(1)
                    If UCase(MasterBoard(Val(RemoteMove.OldMoveX), Val(RemoteMove.OldMoveY))) = "P" AndAlso (RemoteMove.NewMoveY = 0 OrElse RemoteMove.NewMoveY = 7) Then
                        'Pawn Promotion. When we originally connected to the interface, we made a scan of all the knights & queens on the board.
                        'We now use this data to calculate which piece the pawn promoted to (or make our best educated guess, if we don't have enough info).
                        Console.Write("Pawn Promotion Detected - Interpreting as Promotion to a ")
                        Dim PromotedPieceScan As Int16 = ScanPieceInFull(RemoteMove.NewMoveX, RemoteMove.NewMoveY)
                        Dim FoundExactMatch As Boolean
                        Dim IndexMissingList As New List(Of Int16)
                        'Checks to see if the promoted piece has an exact match to one we have previously scanned & stored.
                        For i As Int16 = 0 To RemoteMode.ScannedPieces.Length - 1
                            If RemoteMode.ScannedPieces(i) = 0 Then
                                IndexMissingList.Add(i) 'We have no scan for this piece index - make a note.
                            ElseIf Math.Abs(PromotedPieceScan - RemoteMode.ScannedPieces(i)) < RemoteMode.Tolerance Then
                                'Match found! :D Turn this into a move.
                                RemoteMove.Code = If(i \ 2 = 0, "N", "Q")
                                FoundExactMatch = True
                                Exit For
                            End If
                        Next
                        If Not FoundExactMatch Then
                            'Selects the piece that matches the stored values the closest.
                            Select Case IndexMissingList.Count
                                Case 0
                                    'No match found, at all! Even though we have all the data.
                                    'The opponent likely promoted to a bishop / rook. Promote to a queen instead.
                                    RemoteMove.Code = "Q"
                                Case 1
                                    'Only 1 piece not scanned, and no match - assume the piece promoted to this index not scanned.
                                    RemoteMove.Code = If(IndexMissingList(0) \ 2 = 0, "N", "Q")
                                Case 2
                                    If (RemoteMode.ScannedPieces(0) = 0 AndAlso RemoteMode.ScannedPieces(1) = 0) OrElse (RemoteMode.ScannedPieces(2) = 0 AndAlso RemoteMode.ScannedPieces(3) = 0) Then
                                        'We have scanned 2 knights, or 2 queens. But as we have not found an exact match, we assume that the pawn promoted to the piece we did not scan.
                                        RemoteMove.Code = If(RemoteMode.ScannedPieces(0) = 0, "N", "Q")
                                    Else
                                        'A queen & knight have been scanned (but the square colours are not necessarily the same): promote to whatever scanned value is closest.
                                        For i = 0 To RemoteMode.ScannedPieces.Length - 1
                                            If RemoteMode.ScannedPieces(i) <> 0 Then IndexMissingList.Add(Math.Abs(PromotedPieceScan - RemoteMode.ScannedPieces(i)))
                                        Next
                                        RemoteMove.Code = If(IndexMissingList(2) < IndexMissingList(3), "N", "Q")
                                    End If
                                Case Else
                                    '>2 pieces missing in our original scan. Assume promotion to queen.
                                    RemoteMove.Code = "Q"
                            End Select
                        End If
                        'Writes the promotion piece to the board).
                        Console.WriteLine(If(RemoteMove.Code = "N", "Knight", "Queen") + "...")
                    Else
                        RemoteMove.Code = "1"
                    End If
                End If
            Case 2
                '2 Piece have moved. The move must be castling; anything else is invalid.
                Console.WriteLine("Detected Possible Castling on Interface - Scanning...")
                Dim IsMoveCastling As Boolean
                'Maps the opponent's castling data (note that the user is always correctly-oriented, so the opponent is oriented opposingly).
                Dim CastleRule As CanCastle = If(OrientForWhite, MasterBCanCastle, MasterWCanCastle)
                If CastleRule.CanICastle Then
                    'Finds the two pieces involved in the discrepency. We want these to match to the rook & king.
                    Dim Piece1 As Char = UCase(MasterBoard(Val(PiecesChanged(0)(0)), Val(PiecesChanged(0)(1))))
                    Dim Piece2 As Char = UCase(MasterBoard(Val(PiecesChanged(1)(0)), Val(PiecesChanged(1)(1))))
                    If Piece1 = "K" Then
                        If Piece2 = "R" AndAlso CastleRule.KS Then
                            'We have found a castling move! Turn this into a Move structure.
                            IsMoveCastling = True
                            RemoteMove.OldMoveX = PiecesChanged(0)(0)
                            RemoteMove.OldMoveY = PiecesChanged(0)(1)
                            RemoteMove.NewMoveX = 6
                            RemoteMove.NewMoveY = PiecesChanged(0)(1)
                        End If
                    ElseIf Piece2 = "K" Then
                        If Piece1 = "R" AndAlso CastleRule.QS Then
                            IsMoveCastling = True
                            RemoteMove.OldMoveX = PiecesChanged(1)(0)
                            RemoteMove.OldMoveY = PiecesChanged(1)(1)
                            RemoteMove.NewMoveX = 2
                            RemoteMove.NewMoveY = PiecesChanged(1)(1)
                        End If
                    End If
                End If
                RemoteMove.Code = If(IsMoveCastling, "2", "3")
        End Select

        If RemoteMove.Code = "3" Then 'Too many discrepencies.
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Error: Unable to Interpret Move.")
        ElseIf RemoteMove.Code <> "0" Then 'We've found a move! Output this to the board.
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Move Successfully Interpreted: " & Helper.MoveConverter(MasterBoard, RemoteMove, Helper.ConvertStringToBitCoor(MasterEnPassant)) & ".")
        End If
        Return RemoteMove
    End Function



    'Method that takes a new screenshot, and saves it into SCBytes, so that we can perform operations on it. We also handle mouse move verifications
    'in this method, and automatically remake them if needed. The reason we do this here is that, after making the move, the chess interface will
    'take a couple of milliseconds to make the move on their end. Therefore, we wait until the next screenshot is taken (50ms later) to verify this move.
    Private Sub RemoteModeNewScreenshot()
        'Creates a new Screen Capture, and stores this into SCBytes.
        RemoteMode.g.CopyFromScreen(RemoteMode.ScaledRes.X, RemoteMode.ScaledRes.Y, 0, 0, RemoteMode.ScaledRes.Size, CopyPixelOperation.SourceCopy)
        Dim ScreenCaptureLock As BitmapData = RemoteMode.ScreenCapture.LockBits(New Rectangle(0, 0, RemoteMode.ScreenCapture.Width, RemoteMode.ScreenCapture.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb)
        Marshal.Copy(ScreenCaptureLock.Scan0, RemoteMode.SCBytes, 0, ScreenCaptureLock.Stride * RemoteMode.ScreenCapture.Height)
        RemoteMode.ScreenCapture.UnlockBits(ScreenCaptureLock)

        'Tests that the move has actually been transmitted.
        If RemoteMode.MoveVerified <> -1 Then 'A move has just been made.
            Dim PieceScanOld As Char = ScanPieceColour(Val(RemoteMode.RecentlyPlayedMove.OldMoveX), Val(RemoteMode.RecentlyPlayedMove.OldMoveY))
            If PieceScanOld <> If(OrientForWhite, "W", "B") Then 'The piece has been moved away from its previous position - we assume that the move is valid.
                '(Note that we don't check anything more specific than this, as it might be that the opponent has pre-moved their responce to this move.
                'Hence, we only check this one, specific test, that if failed must be because the move has failed.
                RemoteMode.MoveVerified = -1
                Console.ForegroundColor = ConsoleColor.Blue
                Console.WriteLine("Transmission Move Successfully Verified.")
            Else
                Console.ForegroundColor = ConsoleColor.Red
                Console.Write($"Move Transmission Failed (x{RemoteMode.MoveVerified + 1}). ")
                'Re-submit the move.
                Select Case RemoteMode.MoveVerified
                    Case 2 'We have now made the move 3 times. Something has gone terribly wrong; terminate the connection.
                        Console.ForegroundColor = ConsoleColor.DarkRed
                        Console.WriteLine("Please Input the Move Manually (Aborting Connection)...")
                        RemoteModeBtn_Click()
                    Case 1 '2 fails. *Permanently* make the move speed slower (this is also useful to make move-making on lower-end devices more stable).
                        Console.WriteLine("Reattempting with Slower Transmission Speed...")
                        RemoteMode.MoveDelay = Math.Max(RemoteMode.MoveDelay * 2, 5)
                    Case 0 '1 fail. It might be that the user was moving their mouse at the wrong time. Make the move again...
                        Console.WriteLine("Reattempting...")
                        Thread.Sleep(5)
                End Select
                PlayMoveOnInterface(RemoteMode.RecentlyPlayedMove)
            End If
            Console.ForegroundColor = ConsoleColor.White
        End If
    End Sub



    'The two below methods are involved in scanning a piece, given coordinates, to determine that it is the right colour (fast algorithm - crutial for
    'determining if a square has changed state), or to make a full scan of the piece, for things such as pawn promotion, or for more rigourous testing.
    Private Function ScanPieceColour(ByVal x As SByte, ByVal y As SByte) As Char
        If Not OrientForWhite Then
            x = 7 - x
            y = 7 - y
        End If
        'Start Pixels are the centre of the square.
        Dim PixelX As UInt16 = RemoteMode.InterfaceStartX + Math.Floor((x + 0.5) * RemoteMode.InterfaceSquareSize)
        Dim StartPixelY As UInt16 = RemoteMode.InterfaceStartY + Math.Floor(y * RemoteMode.InterfaceSquareSize)

        Dim ScannedPixel As UInt16
        Dim ScannedPieceValue As Double
        Dim PixelCount As Byte = 0
        Dim Ofset As Byte = 2
        'Scans in a vertical line downwards, from the top of the piece to the bottom (given some ofset, so we don't accidentally record the wrong square).
        For PixelY = StartPixelY + Ofset To StartPixelY + RemoteMode.InterfaceSquareSize - 1 - Ofset Step 2
            ScannedPixel = RemoteMode.SCBytes(Convert2DTo1DIndex(PixelX, PixelY))
            If Math.Abs(RemoteMode.WhiteColour - ScannedPixel) > RemoteMode.Tolerance AndAlso Math.Abs(RemoteMode.BlackColour - ScannedPixel) > RemoteMode.Tolerance Then
                'The pixel is not part of a light / dark square, and so we assume it is part of a piece. Make note of this.
                PixelCount += 1
                ScannedPieceValue += ScannedPixel
            End If
        Next

        If PixelCount > 0 Then
            'The square is occupied.
            ScannedPieceValue /= PixelCount 'Calculates average colour, and compares this to the predicted colour of the white & black pieces.
            'Note that we find the average by multiplying by 0.3, rather than the traditional 0.5, as there is typically a smaller difference between
            'the colour of the light square and the colour of the white pieces, than there is between black pieces & dark squares.
            Dim SquareColourAverage As Double = (RemoteMode.WhiteColour + RemoteMode.BlackColour) * 0.3
            Return If(ScannedPieceValue > SquareColourAverage, "W", "B") 'The square is occupied by a white piece: make a note. Otherwise, it is occupied by a black piece.
        Else 'The only pixels we have recorded are those that are part of the respective square colour. Hence, there is no piece.
            Return " "
        End If
    End Function
    Private Function ScanPieceInFull(ByVal x As SByte, ByVal y As SByte) As UInt16
        If Not OrientForWhite Then
            x = 7 - x
            y = 7 - y
        End If

        Dim PixelToScan As New Point
        Dim PixelScanValue, TotalScanValue As UInt16
        Dim SkipValue As Byte = 1 'No pixels skipped - full scan :O.
        Dim Ofset As Byte = 3

        'Scan the piece by checking in a diagonal line (from top-left to bottom-right) of the square (with clamped in a little by some ofset, to ensure that
        'we don't accidentily scan another square, or the colour between squares).
        PixelToScan.X = RemoteMode.InterfaceStartX + RemoteMode.InterfaceSquareSize * x + Ofset
        PixelToScan.Y = RemoteMode.InterfaceStartY + RemoteMode.InterfaceSquareSize * y + Ofset
        TotalScanValue = 0
        For i = 1 To RemoteMode.InterfaceSquareSize - 2 * Ofset Step SkipValue
            PixelScanValue = RemoteMode.SCBytes(Convert2DTo1DIndex(PixelToScan.X, PixelToScan.Y))
            If Math.Abs(RemoteMode.WhiteColour - PixelScanValue) > RemoteMode.Tolerance AndAlso Math.Abs(RemoteMode.BlackColour - PixelScanValue) > RemoteMode.Tolerance Then
                'Pixel is not a light / dark square, and is thus part of the piece - note the value of this.
                TotalScanValue += PixelScanValue
            End If
            PixelToScan.X += SkipValue
            PixelToScan.Y += SkipValue
        Next
        Return TotalScanValue
    End Function




    'Method that plays a given move on the chess interface, by moving & interacting with the user's mouse.
    Private Sub PlayMoveOnInterface(ByVal Move As Move)
        'RemoteMode.MoveDelay = 500
        AITerminator.Enabled = False
        If RemoteMode.ConnectedToInterface Then
            If Not OrientForWhite Then Move.Flip()

            'Converts the move to the screen position.
            Dim InterfacePosOldX As UInt16 = RemoteMode.ScaledRes.Left + RemoteMode.InterfaceStartX + Math.Floor((Move.OldMoveX + 0.5) * RemoteMode.InterfaceSquareSize)
            Dim InterfacePosOldY As UInt16 = RemoteMode.ScaledRes.Top + RemoteMode.InterfaceStartY + Math.Floor((Move.OldMoveY + 0.5) * RemoteMode.InterfaceSquareSize)
            Dim InterfacePosNewX As UInt16 = RemoteMode.ScaledRes.Left + RemoteMode.InterfaceStartX + Math.Floor((Move.NewMoveX + 0.5) * RemoteMode.InterfaceSquareSize)
            Dim InterfacePosNewY As UInt16 = RemoteMode.ScaledRes.Top + RemoteMode.InterfaceStartY + Math.Floor((Move.NewMoveY + 0.5) * RemoteMode.InterfaceSquareSize)

            Dim IsPromoting As Boolean = (Move.Code = "Q" OrElse Move.Code = "N")
            Dim PreviousMousePoint As Point
            GetCursorPos(PreviousMousePoint) 'Stores the original position of the mouse (before the move), so that after the move has been completed,
            'we can move the mouse back (giving the illusion that nothing has happened :O).

            'Moves the mouse to make the move, by clicking on the piece to move, dragging it to the new square, and letting go.
            SetCursorPos(InterfacePosOldX, InterfacePosOldY)
            Thread.Sleep(RemoteMode.MoveDelay)
            mouse_event(&H2, 0, 0, 0, 0)
            Thread.Sleep(RemoteMode.MoveDelay)
            SetCursorPos(InterfacePosNewX, InterfacePosNewY)
            Thread.Sleep(RemoteMode.MoveDelay)
            mouse_event(&H4, 0, 0, 0, 0)

            If IsPromoting Then
                'On Lichess (the website this mode is designed specifically around), when the user promotes a pawn, a drop-down menu appears, allowing the
                'user to select the piece they want to promote to. The queen piece is located at the same position as the target square, and the
                'knight piece is conveniantly located exactly 1 square below. Target the mouse to select the respective piece, corresponding to 'Move'.
                If Move.Code = "N" Then 'Moves a square down.
                    Thread.Sleep(RemoteMode.MoveDelay)
                    SetCursorPos(InterfacePosNewX, InterfacePosNewY + RemoteMode.InterfaceSquareSize)
                End If
                'As promotions happen very rarely, and the server takes a small fraction of time to render the drop-down menu, wait a little bit before clicking.
                Thread.Sleep(RemoteMode.MoveDelay + 10)
                'Clicks on the required promotion piece.
                mouse_event(&H2, 0, 0, 0, 0)
                Thread.Sleep(RemoteMode.MoveDelay)
                mouse_event(&H4, 0, 0, 0, 0)
            End If

            'Puts the mouse back to its original position.
            Thread.Sleep(RemoteMode.MoveDelay)
            SetCursorPos(PreviousMousePoint.X, PreviousMousePoint.Y)

            If Not OrientForWhite Then Move.Flip()
            RemoteMode.RecentlyPlayedMove = Move
            RemoteMode.MoveVerified += 1

            'The move will later be verified, but for time time being asssume it is valid, and wait for the opponent to make their next move (or, in
            'the case of AI Mode, allow the AI to search on the opponent's turn).
            Console.ForegroundColor = ConsoleColor.Blue
            Console.WriteLine(vbCrLf & "Move Transmitted. Waiting for Opponent's Move...")
            Console.ForegroundColor = ConsoleColor.White
            Me.Text = "[Waiting...]  " & GlobalConstants.ProgramName
            ChessTimer.Start()
        End If
    End Sub

    'Checks if the user is able to interact with pieces on the GUI in Remote Mode.
    Private Function RemoteModeCanClickPiece() As Boolean
        Return Not (RemoteMode.AIEnabled OrElse (PlayerTurn Xor OrientForWhite))
    End Function

End Class
