Imports System.IO
Imports System.Runtime
Imports System.Text
Imports System.Threading

'Class that allows the user to access the full extent of my program.
'Will be the first section of code that the user sees, and provides the starting 
'point for instantiating all theother classes & forms in my program.
Public Class MainMenu
    Private PlayAnimation, PlaySounds, UseSmallOpeningBook As Boolean 'Represents the user's settings (in User Profile)
    'that affect the MainMenu form.
    Private PrimaryColour As Color 'Colour representing the dark squares on the Chessboard.
    Private SecondaryColour As Color 'Colour representing the light squares on the Chessboard.
    Private ColourScheme As String 'Colour code from the text file.
    Private OpeningBook As New List(Of OpeningBookEntry)
    Private BookLoaded As Boolean

    'Objects for allowing the user to drag the form.
    Private IsDraggingForm As Boolean
    Private FormDragInitPos As Point

    'Startup sound effect - sounds taken from Chess.com.
    ReadOnly Sound_Startup As New Media.SoundPlayer With {
        .SoundLocation = GlobalConstants.StartupPath & "\Assets\Sounds\Chess_Startup.wav"
    }

    Private Sub MainMenu_Load() Handles Me.Load
        'Algorithm which retrieves the colour scheme from a file (represented by mnemonics).
        Try
            FileOpen(1, GlobalConstants.StartupPath & "\Assets\User\UserProfile.txt", OpenMode.Input)
            ColourScheme = LineInput(1)
            LineInput(1)
            Dim GeneralSettings As String = LineInput(1)
            Dim temp As String = GeneralSettings(7) 'Tests that the General Options are the correct length.
            'Calibrates attributes based on settings.
            If GeneralSettings(0) = "T" Then PlaySounds = True
            If GeneralSettings(1) = "T" Then PlayAnimation = True
            If GeneralSettings(2) = "T" Then UseSmallOpeningBook = True
        Catch ex As Exception
            'Unable to retrieve - set all settings to their default value.
            ColourScheme = "def"
            PlayAnimation = True
            PlaySounds = True
            UseSmallOpeningBook = False
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Error in retrieving User Profile - reverting to default settings.")
            Console.ForegroundColor = ConsoleColor.White
        End Try
        FileClose(1)
        CreateColourProfile(ColourScheme)
        'Sets up the text, buttons, objects and opening book.
        SetupOptions()
        SetupPieces()
        If Not (PlayAnimation OrElse WP8.Visible) Then RetrieveOpeningBook()
    End Sub

    'Subroutine which calubrates the correct Primary & Secondary colours, based on the user's preference.
    Private Sub CreateColourProfile(ByVal Code As String)
        If LCase(Code) = "blu" Then
            PrimaryColour = Color.LightSteelBlue
            SecondaryColour = Color.AliceBlue
        ElseIf LCase(Code) = "bl2" Then
            PrimaryColour = Color.SlateGray
            SecondaryColour = Color.Silver
        ElseIf LCase(Code) = "gld" Then
            PrimaryColour = Color.Goldenrod
            SecondaryColour = Color.LemonChiffon
        ElseIf LCase(Code) = "grn" Then
            PrimaryColour = Color.DarkSeaGreen
            SecondaryColour = Color.LavenderBlush
        ElseIf LCase(Code) = "red" Then
            PrimaryColour = Color.FromArgb(195, 55, 55)
            SecondaryColour = Color.LavenderBlush
        ElseIf LCase(Code) = "ppl" Then
            PrimaryColour = Color.MediumPurple
            SecondaryColour = Color.MistyRose
        ElseIf LCase(Code) = "mon" Then
            PrimaryColour = Color.DimGray
            SecondaryColour = Color.Silver
        Else 'def
            PrimaryColour = Color.Peru
            SecondaryColour = Color.FloralWhite
        End If
    End Sub

    'A simpler version of the CreateBoard subroutine in the Chess.vb Form, which creates a checkerboard pattern on the screen.
    Private Sub CreateBoard(ByVal sender As Object, ByVal e As PaintEventArgs) Handles MyBase.Paint
        Dim g As Graphics = e.Graphics
        Dim isLight As Boolean = True
        'Creates checkerboard pattern. (Alternates between light and dark colours.)
        For x = 0 To 7
            For y = 0 To 7
                Dim square As New Rectangle(0 + (75 * x), ((75 * y)), 75, 75)
                If isLight Then
                    Using Brush As New SolidBrush(SecondaryColour)
                        g.FillRectangle(Brush, square)
                    End Using
                Else
                    Using Brush As New SolidBrush(PrimaryColour)
                        g.FillRectangle(Brush, square)
                    End Using
                End If
                isLight = Not isLight
            Next
            isLight = Not isLight
        Next
    End Sub

    'Subroutine which sets up all the pieces on the board, so they can be animated.
    Private Sub SetupPieces()
        Dim Pieces(31) As PictureBox
        Pieces(0) = WK1
        Pieces(1) = WQ1
        Pieces(2) = WB1
        Pieces(3) = WB2
        Pieces(4) = WN1
        Pieces(5) = WN2
        Pieces(6) = WR1
        Pieces(7) = WR2
        Pieces(8) = WP1
        Pieces(9) = WP2
        Pieces(10) = WP3
        Pieces(11) = WP4
        Pieces(12) = WP5
        Pieces(13) = WP6
        Pieces(14) = WP7
        Pieces(15) = WP8
        Pieces(16) = BK1
        Pieces(17) = BQ1
        Pieces(18) = BB1
        Pieces(19) = BB2
        Pieces(20) = BN1
        Pieces(21) = BN2
        Pieces(22) = BR1
        Pieces(23) = BR2
        Pieces(24) = BP1
        Pieces(25) = BP2
        Pieces(26) = BP3
        Pieces(27) = BP4
        Pieces(28) = BP5
        Pieces(29) = BP6
        Pieces(30) = BP7
        Pieces(31) = BP8

        'Adds event handles to all the pieces, and the title header, so that the user is able to drag the window around by clicking on these objects.
        AddHandler Title.MouseDown, AddressOf Form_MouseDown
        AddHandler Title.MouseMove, AddressOf Form_MouseMove
        AddHandler Title.MouseUp, AddressOf Form_MouseUp
        For x = 0 To 31
            Pieces(x).Visible = False
            AddHandler Pieces(x).MouseDown, AddressOf Form_MouseDown
            AddHandler Pieces(x).MouseMove, AddressOf Form_MouseMove
            AddHandler Pieces(x).MouseUp, AddressOf Form_MouseUp
        Next

        'Gives all pieces their appropriate images.
        Dim Colour As String = "W"
        For x = 0 To 16 Step 16
            Pieces(x).Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\" & Colour & "King.png")
            Pieces(x + 1).Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\" & Colour & "Queen.png")
            For n = x + 2 To x + 3
                Pieces(n).Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\" & Colour & "Bishop.png")
            Next
            For n = x + 4 To x + 5
                Pieces(n).Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\" & Colour & "Knight.png")
            Next
            For n = x + 6 To x + 7
                Pieces(n).Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\" & Colour & "Rook.png")
            Next
            For n = x + 8 To x + 15
                Pieces(n).Image = Image.FromFile(GlobalConstants.StartupPath & "\Assets\Images\Default\" & Colour & "Pawn.png")
            Next
            Colour = "B"
        Next


        'Sets the location of all the pieces on the board (in the starting position).
        WK1.Location = New Point(300, 525)
        WQ1.Location = New Point(225, 525)
        BK1.Location = New Point(300, 0)
        BQ1.Location = New Point(225, 0)
        WB1.Location = New Point(150, 525)
        WB2.Location = New Point(375, 525)
        BB1.Location = New Point(150, 0)
        BB2.Location = New Point(375, 0)
        WN1.Location = New Point(75, 525)
        WN2.Location = New Point(450, 525)
        BN1.Location = New Point(75, 0)
        BN2.Location = New Point(450, 0)
        WR1.Location = New Point(0, 525)
        WR2.Location = New Point(525, 525)
        BR1.Location = New Point(0, 0)
        BR2.Location = New Point(525, 0)

        WP1.Location = New Point(0, 450)
        WP2.Location = New Point(525, 450)
        BP1.Location = New Point(0, 75)
        BP2.Location = New Point(525, 75)
        WP3.Location = New Point(75, 450)
        WP4.Location = New Point(450, 450)
        BP3.Location = New Point(75, 75)
        BP4.Location = New Point(450, 75)
        WP5.Location = New Point(150, 450)
        WP6.Location = New Point(375, 450)
        BP5.Location = New Point(150, 75)
        BP6.Location = New Point(375, 75)
        WP7.Location = New Point(225, 450)
        WP8.Location = New Point(300, 450)
        BP7.Location = New Point(225, 75)
        BP8.Location = New Point(300, 75)
    End Sub

    'Subroutine which sets the location of all the labels and buttons, and makes them invisible.
    Private Sub SetupOptions()
        Title.Visible = False
        Title.Location = New Point(104, 160)
        Title.BringToFront()

        PlayBtn.Visible = False
        PlayBtn.Location = New Point(127.5, 232.5)
        PlayBtn.FlatAppearance.BorderSize = 0
        PlayBtn.BringToFront()

        PlayOptions.Visible = False
        PlayOptions.Location = New Point(112.5, 225)
        PlayOptions.BringToFront()

        OnePlayer.FlatAppearance.BorderSize = 0
        TwoPlayer.FlatAppearance.BorderSize = 0

        Analysis.Visible = False
        Analysis.Location = New Point(352.5, 232.5)
        Analysis.FlatAppearance.BorderSize = 0
        Analysis.BringToFront()

        Training.Visible = False
        Training.Location = New Point(127.5, 307.5)
        Training.FlatAppearance.BorderSize = 0
        Training.BringToFront()

        RemoteBtn.Visible = False
        RemoteBtn.Location = New Point(352.5, 307.5)
        RemoteBtn.FlatAppearance.BorderSize = 0
        RemoteBtn.BringToFront()

        Credits.Visible = False
        Credits.Location = New Point(172, 392)
        Credits.BringToFront()

        ExitBtn.Visible = False
        ExitBtn.Location = New Point(266, 417)
        ExitBtn.FlatAppearance.BorderSize = 0
        ExitBtn.BringToFront()
    End Sub

    'Subroutine which handles the animation of the pieces on boot-up, the retrieval of the Opening Book, and the
    'appearance of the form's objects.
    Private Sub StartupAnimation() Handles Me.Activated
        MyBase.Refresh()
        Me.BringToFront()
        If PlayAnimation Then Console.Clear()

        Dim TempColour As String
        'Resets the user's colour profile (as it may have changed in the Settings form, and hence must be updated).
        Try
            FileOpen(1, GlobalConstants.StartupPath & "\Assets\User\UserProfile.txt", OpenMode.Input)
            TempColour = LineInput(1)
            FileClose(1)
            If TempColour <> ColourScheme Then
                ColourScheme = TempColour
                CreateColourProfile(ColourScheme)
            End If
        Catch ex As Exception
        End Try
        MyBase.Refresh()

        'To prevent the opening animation from triggering when the user exits back to the main menu, we test
        'if the final piece of the animation is visible to the user. If it is, then the program must have already
        'been booted up at a previous point in time.
        Dim AlreadyBooted As Boolean = WP8.Visible
        If PlayAnimation Then
            If Not AlreadyBooted Then
                'Uses multithreading to simultaneously retrieve the opening book, and play the opening animation.
                Dim OpeningBookThread As New Task(AddressOf RetrieveOpeningBook)
                OpeningBookThread.Start()
            End If

            'Shows the checkerboard pattern over time. This is achieved by having 8 Rectangles which slowly move right off the screen.
            'These 8 Rectangles are made to be out-of-sync, so each Rectangle moves in the cycle after the one above it.
            'This gives the illusion of the checkerboard pattern being slowly constructed.
            For n = 1 To 15
                RowHider1.Left += 75
                If n >= 2 Then RowHider2.Left += 75
                If n >= 3 Then RowHider3.Left += 75
                If n >= 4 Then RowHider4.Left += 75
                If n >= 5 Then RowHider5.Left += 75
                If n >= 6 Then RowHider6.Left += 75
                If n >= 7 Then RowHider7.Left += 75
                If n >= 8 Then RowHider8.Left += 75
                Application.DoEvents()
                If n < 15 Then Thread.Sleep(-3 * Math.Abs(n - 7) + 55)
            Next
        Else
            'Makes all the RowHider objects invisible.
            For n = 1 To 8
                Me.Controls.Find("RowHider" & n, True).Single().Visible = False
            Next
        End If
        'Slowly makes the pieces on the board visible (in a spiral pattern).
        AnimatePiece(WR1)
        AnimatePiece(WR2)
        AnimatePiece(BR1)
        AnimatePiece(BR2)
        Wait(0.8)
        AnimatePiece(WN1)
        AnimatePiece(WN2)
        AnimatePiece(BN1)
        AnimatePiece(BN2)
        AnimatePiece(WP1)
        AnimatePiece(WP2)
        AnimatePiece(BP1)
        AnimatePiece(BP2)
        Wait(0.8 * 3)
        AnimatePiece(WB1)
        AnimatePiece(WB2)
        AnimatePiece(BB1)
        AnimatePiece(BB2)
        AnimatePiece(WP3)
        AnimatePiece(WP4)
        AnimatePiece(BP3)
        AnimatePiece(BP4)
        Wait(0.8 * 3)
        AnimatePiece(WK1)
        AnimatePiece(WQ1)
        AnimatePiece(BK1)
        AnimatePiece(BQ1)
        AnimatePiece(WP5)
        AnimatePiece(WP6)
        AnimatePiece(BP5)
        AnimatePiece(BP6)
        Wait(0.8 * 3)
        AnimatePiece(WP7)
        AnimatePiece(WP8)
        AnimatePiece(BP7)
        AnimatePiece(BP8)
        Wait(0.8 * 2.5)

        'Makes the labels & buttons visible, then audibly notifies the user that they can choose a menu option.
        Title.Visible = True
        PlayBtn.Visible = True
        Training.Visible = True
        Analysis.Visible = True
        RemoteBtn.Visible = True
        Credits.Visible = True
        ExitBtn.Visible = True
        If Not AlreadyBooted AndAlso PlaySounds Then Sound_Startup.Play()
    End Sub
    Private Sub AnimatePiece(ByVal Piece As PictureBox)
        Piece.Visible = True
    End Sub


    'Subroutine that temporarily stalls the program every time a piece appears in the opening animation.
    Private Sub Wait(ByVal Interval As SByte)
        If PlayAnimation Then
            Application.DoEvents()
            Thread.Sleep(20 + 30 * Interval)
        End If
    End Sub

    'Subroutine that retrives, and loads into memory, the opening book.
    Private Sub RetrieveOpeningBook()
        Dim Timer As New Stopwatch
        Dim TempEntry As OpeningBookEntry
        Dim BookPath As String
        'Determines the specific book to load.
        If UseSmallOpeningBook Then
            BookPath = "\Assets\SmallOpeningBook.txt"
        Else
            BookPath = "\Assets\LargeOpeningBook.txt"
        End If
        Try
            GCSettings.LatencyMode = GCLatencyMode.LowLatency 'Relaxes garbage collection during the forming of the Opening Book,
            'so that it can be loaded into memory quicker.
            Timer.Start()
            'Opens opening book from file using StreamReader.
            Using SR As New StreamReader(GlobalConstants.StartupPath & BookPath, Encoding.UTF8, True, 16384)
                While Not (SR.EndOfStream OrElse BookLoaded)
                    TempEntry = New OpeningBookEntry(SR.ReadLine())
                    OpeningBook.Add(TempEntry)
                End While
            End Using
            Timer.Stop()
            If Not BookLoaded Then Console.ForegroundColor = ConsoleColor.Green : Console.WriteLine("Opening Book Successfully Retrieved in: " & Math.Round(Timer.Elapsed.TotalMilliseconds, 1) & "ms.")
            BookLoaded = True
        Catch ex As Exception 'Could not successfully retrieve book.
            Timer.Stop()
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Error when retrieving opening book.")
            OpeningBook = Nothing
        End Try
        GCSettings.LatencyMode = GCLatencyMode.Interactive
        Console.ForegroundColor = ConsoleColor.White
    End Sub


    'Controls for the buttons The PlayBtn object holds both the OnePlayer & TwoPlayer objects, and will appear &
    'dissapear when the mouse enters & leaves the object.
    Private Sub PlayBtn_MouseEnter() Handles PlayBtn.MouseEnter
        PlayBtn.Visible = False
        PlayOptions.Visible = True
    End Sub
    Private Sub PlayOptions_MouseLeave() Handles PlayOptions.MouseLeave
        PlayOptions.Visible = False
        PlayBtn.Visible = True
    End Sub

    Private Sub OnePlayer_MouseLeave() Handles OnePlayer.MouseLeave
        If Not TwoPlayer.ClientRectangle.Contains(TwoPlayer.PointToClient(MousePosition)) Then
            'The user's mouse has left the confines of PlayBtn - make the object visible again.
            PlayOptions.Visible = False
            PlayBtn.Visible = True
        End If
    End Sub
    Private Sub OnePlayer_Click() Handles OnePlayer.Click
        If Not BookLoaded Then HandleOpeningBookNotLoaded()
        'Instantiates a new version of the OnePlayerCustomisation form.
        Dim Customisation = New OnePlayerCustomisation(OpeningBook)
        Customisation.Show()
        Me.Hide()
    End Sub

    Private Sub TwoPlayer_MouseLeave() Handles TwoPlayer.MouseLeave
        If Not OnePlayer.ClientRectangle.Contains(OnePlayer.PointToClient(MousePosition)) Then
            PlayOptions.Visible = False
            PlayBtn.Visible = True
        End If
    End Sub
    Private Sub TwoPlayer_Click() Handles TwoPlayer.Click
        If Not BookLoaded Then HandleOpeningBookNotLoaded()
        'Instantiates a new version of the TwoPlayerCustomisation form.
        Dim Customisation As New TwoPlayerCustomisation
        Customisation.Show()
        Me.Hide()
    End Sub


    Private Sub Training_MouseEnter() Handles Training.MouseEnter
        'Grows the Button.
        Training.Size = New Size(150, 75)
        Training.Left -= 15
        Training.Top -= 7.5
        Training.Font = New Font("Microsoft Sans Serif", 24, FontStyle.Bold)
    End Sub
    Private Sub Training_MouseLeave() Handles Training.MouseLeave
        'Shrinks the Button.
        Training.Size = New Size(120, 60)
        Training.Left += 15
        Training.Top += 7.5
        Training.Font = New Font("Microsoft Sans Serif", 18, FontStyle.Bold)
    End Sub
    Private Sub Training_Click() Handles Training.Click
        If Not BookLoaded Then HandleOpeningBookNotLoaded()
        'Instantiates a new version of the TrainingCustomisation form.
        Dim Customisation As New TrainingCustomisation
        Customisation.Show()
        Me.Hide()
    End Sub


    Private Sub Analysis_MouseEnter() Handles Analysis.MouseEnter
        'Grows the Button.
        Analysis.Size = New Size(150, 75)
        Analysis.Left -= 15
        Analysis.Top -= 7
        Analysis.Font = New Font("Microsoft Sans Serif", 24, FontStyle.Bold)
    End Sub
    Private Sub Analysis_MouseLeave() Handles Analysis.MouseLeave
        'Shrinks the Button.
        Analysis.Size = New Size(120, 60)
        Analysis.Left += 15
        Analysis.Top += 7.5
        Analysis.Font = New Font("Microsoft Sans Serif", 18, FontStyle.Bold)
    End Sub
    Private Sub Analysis_Click() Handles Analysis.Click
        If Not BookLoaded Then HandleOpeningBookNotLoaded()
        'Creates the chess game (if Opening Book could not be retrieved then do not pass book).
        Dim ChessGame As Chess
        If OpeningBook.Count > 0 Then ChessGame = New Chess(OpeningBook) : Else ChessGame = New Chess()
        ChessGame.Show()
        Me.Hide()
    End Sub

    Private Sub RemoteBtn_MouseEnter() Handles RemoteBtn.MouseEnter
        'Grows the Button.
        RemoteBtn.Size = New Size(150, 75)
        RemoteBtn.Left -= 15
        RemoteBtn.Top -= 7.5
        RemoteBtn.Font = New Font("Microsoft Sans Serif", 22, FontStyle.Bold)
    End Sub
    Private Sub RemoteBtn_MouseLeave() Handles RemoteBtn.MouseLeave
        'Shrinks the Button.
        RemoteBtn.Size = New Size(120, 60)
        RemoteBtn.Left += 15
        RemoteBtn.Top += 7.5
        RemoteBtn.Font = New Font("Microsoft Sans Serif", 16, FontStyle.Bold)
    End Sub
    Private Sub RemoteBtn_Click() Handles RemoteBtn.Click
        If Not BookLoaded Then HandleOpeningBookNotLoaded()
        'Instantiates a new version of the OnePlayerCustomisation form.
        Dim Customisation = New OnePlayerCustomisation(OpeningBook, True)
        Customisation.Show()
        Me.Hide()
    End Sub



    'Methods that allow the user to move the form around the screen (as it does not have a handle). We do this in the same way as the
    'drag-drop mechanics in the main Chess.vb form :).
    Private Sub Form_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        IsDraggingForm = True
        FormDragInitPos = New Point(e.X, e.Y)
    End Sub
    Private Sub Form_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If IsDraggingForm Then
            Dim ScreenPos As Point = PointToScreen(e.Location)
            Me.Location = New Point(ScreenPos.X - FormDragInitPos.X, ScreenPos.Y - FormDragInitPos.Y)
        End If
    End Sub
    Private Sub Form_MouseUp() Handles Me.MouseUp
        IsDraggingForm = False
    End Sub


    'Subroutine which clears the Opening Book, in case it has not been fully loaded by the time the user has clicked on a Game Mode.
    'This is to prevent an unfinished / broken Book from entering the System.
    Private Sub HandleOpeningBookNotLoaded()
        Console.ForegroundColor = ConsoleColor.DarkRed
        Console.WriteLine("WARNING: Opening Book not fully Loaded into the System - Continuing Without...")
        Console.WriteLine("Tip: Consider using the 'Smaller Opening Book' in the Program's Settings for faster load times.")
        Console.ForegroundColor = ConsoleColor.White
        OpeningBook.Clear()
        BookLoaded = True
    End Sub



    'Button that displays the credits information onto the screen (in the form of a pop-up).
    'To fix a bug where the form would become unresponsive after closing the MsgBox, a separate thread now displays the MsgBox to the user.
    Private Sub Credits_Click() Handles Credits.Click
        Dim MsgBoxThread As New Task(AddressOf ShowCreditsBox)
        MsgBoxThread.Start()
    End Sub
    Sub ShowCreditsBox()
        MsgBox(Strings.StrDup(10, " ") & "Chess Game & Artificial Intelligence (" & GlobalConstants.ProgramVersion & ")" & vbCrLf & Strings.StrDup(21, " ") & "Created by Alfie Kunz (8158)" & vbCrLf & Strings.StrDup(22, " ") & "of Beckfoot School (37101)" & vbCrLf & "Project used for the AQA GCE Computer Science NEA" & vbCrLf & Strings.StrDup(35, " ") & "(2021 - 2025)", vbInformation, "Credits")
    End Sub

    Private Sub ExitBtn_Click() Handles ExitBtn.Click
        Me.Close()
    End Sub


End Class
