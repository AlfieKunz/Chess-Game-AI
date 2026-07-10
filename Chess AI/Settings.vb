Imports System.Threading
Imports System.IO
'Form that allows the user to customise the settings of my program. Broken into 3 sections:
'board colour scheme setter, piece animation speed setter, and general settings.
Public Class Settings
    Private PrimaryColour As Color 'Colour representing the dark squares on the Chessboard.
    Private SecondaryColour As Color 'Colour representing the light squares on the Chessboard.
    Private ColourScheme As String 'Colour code from the text file.
    Private AnimationSpeed As SByte = GlobalConstants.DefaultAnimationSpeed 'Represents the speed of the piece-moving animation: 0 = Off, 1 = Fast, 2 = Medium, 3 = Slow
    Private GeneralOptions As String = GlobalConstants.DefaultGeneralOptions
    Private FixedSearchDepth As Byte = 0 'Number representing the fixed depth the AI will search to (0 = off).

    Private SquareHistory(1, 1) As SByte
    Private AnimationRunning, AnimationSettingsChanged As Boolean 'Information regarding the piece animation testing.

    Private ReadOnly Sound_Move As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Assets\Sounds\Chess_Move.wav"
    }

    'Subroutines that sets up the form.
    Public Sub New(ByVal CanToggleInvis As Boolean, ByVal CanToggleTouchMove As Boolean, ByVal CanChangeDepth As Boolean)
        ' This call is required by the designer.
        InitializeComponent()
        'Disables certain general settings, depending on what gamemode the user is in.
        InvisBtn.Enabled = CanToggleInvis
        TouchMoveBtn.Enabled = CanToggleTouchMove
        FixedSearchBtn.Enabled = CanChangeDepth
    End Sub
    Private Sub Settings_Load() Handles MyBase.Load
        'Calibrates the colour selector.
        ColourSelectorR.BackgroundImage.RotateFlip(RotateFlipType.RotateNoneFlipX)

        'Sets up the required pieces on the board.
        Checkerboard.Controls.Add(WP1)
        WP1.Location = New Point(0, 0)
        WP1.BringToFront()

        Checkerboard.Controls.Add(WP2)
        WP2.Location = New Point(75, 0)
        WP2.BringToFront()

        Checkerboard.Controls.Add(WP3)
        WP3.Location = New Point(150, 75)
        WP3.BringToFront()

        Checkerboard.Controls.Add(WR1)
        WR1.Location = New Point(0, 150)
        WR1.BringToFront()

        Checkerboard.Controls.Add(WN1)
        WN1.Location = New Point(75, 150)
        WN1.BringToFront()

        Checkerboard.Controls.Add(WB1)
        WB1.Location = New Point(150, 150)
        WB1.BringToFront()

        ConfigureSettings()
    End Sub

    'Subroutine which calibrates all the elements of the form. Used upon boot-up, and when resetting all settings.
    Private Sub ConfigureSettings()
        KnightClicked() 'Knight = default piece to move.
        'Algorithm which retrieves the colour scheme from a file (represented by mnemonics).
        Try
            FileOpen(1, Application.StartupPath & "\Assets\User\UserProfile.txt", OpenMode.Input)
            ColourScheme = LineInput(1)
            AnimationSpeed = Val(LineInput(1))
            GeneralOptions = LineInput(1)
            FixedSearchDepth = Val(LineInput(1))
            Dim temp As String = GeneralOptions(7) 'Tests that the General Options are the correct length.
        Catch ex As Exception
            'Sets all settings to their default values.
            ColourScheme = "def"
            AnimationSpeed = GlobalConstants.DefaultAnimationSpeed
            GeneralOptions = GlobalConstants.DefaultGeneralOptions
            FixedSearchDepth = 0
        End Try
        FileClose(1)

        'Checks, or unckecks the general options based on the user's profile.
        AnimationRunning = True
        If GeneralOptions(0) = "T" Then SoundBtn.Checked = True : Else SoundBtn.Checked = False
        If GeneralOptions(1) = "T" Then AnimationBtn.Checked = True : Else AnimationBtn.Checked = False
        If GeneralOptions(2) = "T" Then OpeningBookBtn.Checked = True : Else OpeningBookBtn.Checked = False
        If GeneralOptions(3) = "T" Then BoardBtn.Checked = True : Else BoardBtn.Checked = False
        If GeneralOptions(4) = "T" Then PieceBtn.Checked = True : Else PieceBtn.Checked = False
        If GeneralOptions(5) = "T" Then TouchMoveBtn.Checked = True : Else TouchMoveBtn.Checked = False
        If GeneralOptions(6) = "T" Then InvisBtn.Checked = True : Else InvisBtn.Checked = False
        If GeneralOptions(7) = "T" Then HammadBtn.Checked = True : Else HammadBtn.Checked = False
        If FixedSearchBtn.Enabled AndAlso FixedSearchDepth <> 0 Then 'Fixed Search Setting Toggled.
            FixedSearchBox.Text = FixedSearchDepth
            FixedSearchBtn.Checked = True
        End If

        AnimationRunning = False

        'Configures colour settings, and enables the colour's corresponding radio button.
        CreateColourProfile(ColourScheme)
        Dim TempBtn As RadioButton = Me.Controls.Find(ColourScheme, True).Single()
        TempBtn.Checked = True
        MoveSelectedColour(ColourScheme)

        SpeedSetter.Value = AnimationSpeed
        AnimationSettingsChanged = False
    End Sub


    'Subroutine that sets the correct colours of the checkerboard, depending on the user's colour scheme.
    Private Sub CreateColourProfile(ByVal Code As String)
        Select Case LCase(Code)
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
    End Sub

    'Subroutine that saves the user's settings to the UserProfile file, whenever any settings are modified.
    Private Sub SaveSettings()
        'Writes the colour preferences to a file (creates the file if one does not exist).
        Try
            FileOpen(1, Application.StartupPath & "\Assets\User\UserProfile.txt", OpenMode.Output)
            PrintLine(1, ColourScheme)
            PrintLine(1, CStr(AnimationSpeed))
            PrintLine(1, GeneralOptions)
            PrintLine(1, FixedSearchDepth)
            FileClose(1)
        Catch ex As Exception
            FileClose(1)
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Unable to Save Settings. Please try again...")
            Console.ResetColor()
        End Try

        'Finds the 'chess' parent form, and runs one of its subroutines which configures the user's new settings.
        For Each TempForm As Form In Application.OpenForms
            If TempForm.Name = "Chess" Then
                Dim FormInstance As Chess = DirectCast(TempForm, Chess)
                FormInstance.LoadUserProfile()
            End If
        Next
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Settings Saved.")
        Console.ResetColor()
    End Sub



    'Subroutine that paints the form with all of the colour options.
    Private Sub CreateColourSquares(ByVal sender As Object, ByVal e As PaintEventArgs) Handles MyBase.Paint
        Const StartYPos As Byte = 80
        Dim g As Graphics = e.Graphics

        'Default
        Dim Square As New Rectangle(0, StartYPos, 50, 50)
        Using Brush As New SolidBrush(Color.Peru)
            g.FillRectangle(Brush, Square)
        End Using
        Square = New Rectangle(50, StartYPos, 50, 50)
        Using Brush As New SolidBrush(Color.FloralWhite)
            g.FillRectangle(Brush, Square)
        End Using

        'Blue 1
        Square = New Rectangle(200, StartYPos, 50, 50)
        Using Brush As New SolidBrush(Color.LightSteelBlue)
            g.FillRectangle(Brush, Square)
        End Using
        Square = New Rectangle(250, StartYPos, 50, 50)
        Using Brush As New SolidBrush(Color.AliceBlue)
            g.FillRectangle(Brush, Square)
        End Using


        'Blue 2
        Square = New Rectangle(0, StartYPos + 60, 50, 50)
        Using Brush As New SolidBrush(Color.SlateGray)
            g.FillRectangle(Brush, Square)
        End Using
        Square = New Rectangle(50, StartYPos + 60, 50, 50)
        Using Brush As New SolidBrush(Color.Silver)
            g.FillRectangle(Brush, Square)
        End Using

        'Gold
        Square = New Rectangle(200, StartYPos + 60, 50, 50)
        Using Brush As New SolidBrush(Color.Goldenrod)
            g.FillRectangle(Brush, Square)
        End Using
        Square = New Rectangle(250, StartYPos + 60, 50, 50)
        Using Brush As New SolidBrush(Color.LemonChiffon)
            g.FillRectangle(Brush, Square)
        End Using


        'Green
        Square = New Rectangle(0, StartYPos + 120, 50, 50)
        Using Brush As New SolidBrush(Color.DarkSeaGreen)
            g.FillRectangle(Brush, Square)
        End Using
        Square = New Rectangle(50, StartYPos + 120, 50, 50)
        Using Brush As New SolidBrush(Color.LavenderBlush)
            g.FillRectangle(Brush, Square)
        End Using

        'Red
        Square = New Rectangle(200, StartYPos + 120, 50, 50)
        Using Brush As New SolidBrush(Color.FromArgb(195, 55, 55))
            g.FillRectangle(Brush, Square)
        End Using
        Square = New Rectangle(250, StartYPos + 120, 50, 50)
        Using Brush As New SolidBrush(Color.LavenderBlush)
            g.FillRectangle(Brush, Square)
        End Using

        'Purple
        Square = New Rectangle(0, StartYPos + 180, 50, 50)
        Using Brush As New SolidBrush(Color.MediumPurple)
            g.FillRectangle(Brush, Square)
        End Using
        Square = New Rectangle(50, StartYPos + 180, 50, 50)
        Using Brush As New SolidBrush(Color.MistyRose)
            g.FillRectangle(Brush, Square)
        End Using

        'Gray
        Square = New Rectangle(200, StartYPos + 180, 50, 50)
        Using Brush As New SolidBrush(Color.DimGray)
            g.FillRectangle(Brush, Square)
        End Using
        Square = New Rectangle(250, StartYPos + 180, 50, 50)
        Using Brush As New SolidBrush(Color.Silver)
            g.FillRectangle(Brush, Square)
        End Using
    End Sub

    'Subroutines Controlling each Colour Scheme Option change. When each button is clicked,
    'the Primary & Secondary colours are appended, and these preferences are written to a file.
    'When the mouse hovers over the radio button, that colour is highlighted via ColourSelector.
    Private Sub Def_MouseEnter() Handles Def.MouseEnter
        MoveSelectedColour("def")
    End Sub
    Private Sub DefColourChanger() Handles Def.Click
        'Sets colours and redraws the checkerboard.
        PrimaryColour = Color.Peru
        SecondaryColour = Color.FloralWhite
        Checkerboard.Refresh()
        ColourScheme = "def"
        SaveSettings()
    End Sub

    Private Sub Blu_MouseEnter() Handles Blu.MouseEnter
        MoveSelectedColour("blu")
    End Sub
    Private Sub BluColourChanger() Handles Blu.Click
        PrimaryColour = Color.LightSteelBlue
        SecondaryColour = Color.AliceBlue
        Checkerboard.Refresh()
        ColourScheme = "blu"
        SaveSettings()
    End Sub

    Private Sub Bl2_MouseEnter() Handles Bl2.MouseEnter
        MoveSelectedColour("bl2")
    End Sub
    Private Sub DBluColourChanger() Handles Bl2.Click
        PrimaryColour = Color.SlateGray
        SecondaryColour = Color.LightGray
        Checkerboard.Refresh()
        ColourScheme = "bl2"
        SaveSettings()
    End Sub

    Private Sub Gld_MouseEnter() Handles Gld.MouseEnter
        MoveSelectedColour("gld")
    End Sub
    Private Sub GldColourChanger() Handles Gld.Click
        PrimaryColour = Color.Goldenrod
        SecondaryColour = Color.LemonChiffon
        Checkerboard.Refresh()
        ColourScheme = "gld"
        SaveSettings()
    End Sub

    Private Sub Grn_MouseEnter() Handles Grn.MouseEnter
        MoveSelectedColour("grn")
    End Sub
    Private Sub GrnColourChanger() Handles Grn.Click
        PrimaryColour = Color.DarkSeaGreen
        SecondaryColour = Color.LavenderBlush
        Checkerboard.Refresh()
        ColourScheme = "grn"
        SaveSettings()
    End Sub

    Private Sub Red_MouseEnter() Handles Red.MouseEnter
        MoveSelectedColour("red")
    End Sub
    Private Sub RedColourChanger() Handles Red.Click
        PrimaryColour = Color.FromArgb(195, 55, 55)
        SecondaryColour = Color.LavenderBlush
        Checkerboard.Refresh()
        ColourScheme = "red"
        SaveSettings()
    End Sub

    Private Sub Ppl_MouseEnter() Handles Ppl.MouseEnter
        MoveSelectedColour("ppl")
    End Sub
    Private Sub PplColourChanger() Handles Ppl.Click
        PrimaryColour = Color.MediumPurple
        SecondaryColour = Color.MistyRose
        Checkerboard.Refresh()
        ColourScheme = "ppl"
        SaveSettings()
    End Sub

    Private Sub Mon_MouseEnter() Handles Mon.MouseEnter
        MoveSelectedColour("mon")
    End Sub
    Private Sub MonColourChanger() Handles Mon.Click
        PrimaryColour = Color.DimGray
        SecondaryColour = Color.Silver
        Checkerboard.Refresh()
        ColourScheme = "mon"
        SaveSettings()
    End Sub

    'Subroutine for when the user's mouse leaves the left section of the form.
    'Sets the SelectedColour PictureBox back to the user's chosen colour.
    Private Sub ColourPanel_MouseLeave() Handles ColourPanel.MouseLeave
        MoveSelectedColour(ColourScheme)
    End Sub


    'Subroutine that changes the positions of the SelectedColour PictureBoxes,
    'representing the colour that the user is hovering over, or their chosen colour.
    'L and R objects are used to select the required object, with the L ColourSelector
    'being used for the colours on the left, and likewise for R ColourSelector.
    Private Sub MoveSelectedColour(ByVal Code As String)
        Select Case LCase(Code)
            Case "blu", "gld", "red", "mon"
                'Colour is on the right - toggle the correct ColourSelector.
                ColourSelectorL.Visible = False
                ColourSelectorR.Visible = True
                'Moves ColourSelector to the correct location.
                Select Case LCase(Code)
                    Case "blu"
                        ColourSelectorR.Location = New Point(190, 70)
                    Case "gld"
                        ColourSelectorR.Location = New Point(190, 130)
                    Case "red"
                        ColourSelectorR.Location = New Point(190, 190)
                    Case "mon"
                        ColourSelectorR.Location = New Point(190, 250)
                End Select
            Case Else
                'Colour is on the left - toggle the correct ColourSelector.
                ColourSelectorR.Visible = False
                ColourSelectorL.Visible = True
                'Moves ColourSelector to the correct location.
                Select Case LCase(Code)
                    Case "bl2"
                        ColourSelectorL.Location = New Point(0, 130)
                    Case "grn"
                        ColourSelectorL.Location = New Point(0, 190)
                    Case "ppl"
                        ColourSelectorL.Location = New Point(0, 250)
                    Case Else 'def
                        ColourSelectorL.Location = New Point(0, 70)
                End Select
        End Select
    End Sub




    'A simpler version of the CreateBoard subroutine in the Chess.vb Form, which creates a checkerboard pattern on the screen.
    Private Sub CreateBoard(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Checkerboard.Paint
        Dim g As Graphics = e.Graphics
        Dim isLight As Boolean = False
        'Creates checkerboard pattern. (Alternates between light and dark colours.)
        For y = 0 To 2
            For x = 0 To 2
                Dim square As New Rectangle(75 * x, 75 * y, 75, 75)
                If isLight Then
                    If BoardBtn.Checked AndAlso ((x = SquareHistory(0, 0) AndAlso y = SquareHistory(0, 1)) OrElse (x = SquareHistory(1, 0) AndAlso y = SquareHistory(1, 1))) Then
                        Using Brush As New SolidBrush(Color.YellowGreen) 'SquareHistory secondary colour.
                            g.FillRectangle(Brush, square)
                        End Using
                    Else
                        Using Brush As New SolidBrush(SecondaryColour)
                            g.FillRectangle(Brush, square)
                        End Using
                    End If
                Else
                    If BoardBtn.Checked AndAlso ((x = SquareHistory(0, 0) AndAlso y = SquareHistory(0, 1)) OrElse (x = SquareHistory(1, 0) AndAlso y = SquareHistory(1, 1))) Then
                        Using Brush As New SolidBrush(Color.OliveDrab) 'SquareHistory primary colour.
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
        Next
    End Sub


    'Subroutines that change the coloured squres on the board, depending on what piece the user clicks on.
    'This is used to select a piece, that will move when the user tests their animation configuration.
    Private Sub RookClicked() Handles WR1.MouseClick
        If Not AnimationRunning Then
            SquareHistory(0, 0) = 0
            SquareHistory(0, 1) = 2
            SquareHistory(1, 0) = 0
            SquareHistory(1, 1) = 1
            Checkerboard.Refresh()
        End If
    End Sub
    Private Sub KnightClicked() Handles WN1.MouseClick
        If Not AnimationRunning Then
            SquareHistory(0, 0) = 1
            SquareHistory(0, 1) = 2
            SquareHistory(1, 0) = 2
            SquareHistory(1, 1) = 0
            Checkerboard.Refresh()
        End If
    End Sub
    Private Sub Bishoplicked() Handles WB1.MouseClick
        If Not AnimationRunning Then
            SquareHistory(0, 0) = 2
            SquareHistory(0, 1) = 2
            SquareHistory(1, 0) = 1
            SquareHistory(1, 1) = 1
            Checkerboard.Refresh()
        End If
    End Sub


    'Subroutine that performs the temporary piece animation move.
    Private Sub AnimationTester_Click() Handles AnimationTester.Click
        If Not AnimationRunning Then
            AnimationRunning = True
            'Determines which PictureBox is the one to move, using the location of SquareHistory.
            Dim Piece As PictureBox
            Select Case SquareHistory(0, 0)
                Case 0
                    Piece = WR1
                Case 1
                    Piece = WN1
                Case 2
                    Piece = WB1
            End Select
            Dim Constant As SByte 'How many 'steps' it takes a piece to move from A to B.
            'Larger Constant = smoother but slower animation.
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
            'Moves piece to the destination square, then back to the starting square.
            For n As SByte = 1 To -1 Step -2
                For x = 1 To Constant
                    'Moves the piece one 'step' towards its destination square.
                    Piece.Left -= n * (75 * (SquareHistory(0, 0) - SquareHistory(1, 0)) / Constant)
                    Piece.Top -= n * (75 * (2 - SquareHistory(1, 1)) / Constant)
                    If AnimationSpeed = 3 Then Piece.Refresh() Else Application.DoEvents() 'Updates the visuals of the piece (in different ways depending on the Animation Speed).
                Next
                'Completes the move, then waits depending on what the animation speed is (faster speed = less time to wait).
                Checkerboard.Refresh()
                If SoundBtn.Checked Then Sound_Move.Play()
                If n = 1 Then Thread.Sleep(400 + ((4 - AnimationSpeed) * 100)) 'Stalls in between the move.
            Next
            AnimationRunning = False
        End If
    End Sub

    'Subroutines that control the SpeedSetter track bar.
    Private Sub SpeedSetter_ValueChanged() Handles SpeedSetter.ValueChanged
        AnimationSpeed = SpeedSetter.Value
        AnimationSettingsChanged = True
    End Sub
    Private Sub SpeedSetter_MouseLeave() Handles SpeedSetter.MouseLeave
        If AnimationSettingsChanged Then SaveSettings() : AnimationSettingsChanged = False
    End Sub



    'Subroutines that control the Check Boxes, by editing the required character in GeneralOptions, then
    'saving the file.
    Private Sub SoundBtn_CheckedChanged() Handles SoundBtn.CheckedChanged
        If Not AnimationRunning Then
            If SoundBtn.Checked Then
                GeneralOptions = GeneralOptions.Remove(0, 1).Insert(0, "T")
                Sound_Move.Play()
            Else
                GeneralOptions = GeneralOptions.Remove(0, 1).Insert(0, "F")
            End If
            SaveSettings()
        End If
    End Sub
    Private Sub AnimationBtn_CheckedChanged() Handles AnimationBtn.CheckedChanged
        If Not AnimationRunning Then
            If AnimationBtn.Checked Then
                GeneralOptions = GeneralOptions.Remove(1, 1).Insert(1, "T")
            Else
                GeneralOptions = GeneralOptions.Remove(1, 1).Insert(1, "F")
            End If
            SaveSettings()
        End If
    End Sub
    Private Sub OpeningBookBtn_CheckedChanged() Handles OpeningBookBtn.CheckedChanged
        If Not AnimationRunning Then
            If OpeningBookBtn.Checked Then
                GeneralOptions = GeneralOptions.Remove(2, 1).Insert(2, "T")
            Else
                GeneralOptions = GeneralOptions.Remove(2, 1).Insert(2, "F")
            End If
            SaveSettings()
        End If
    End Sub
    Private Sub BoardBtn_CheckedChanged() Handles BoardBtn.CheckedChanged
        If BoardBtn.Checked Then
            GeneralOptions = GeneralOptions.Remove(3, 1).Insert(3, "T")
        Else
            GeneralOptions = GeneralOptions.Remove(3, 1).Insert(3, "F")
        End If
        If Not AnimationRunning Then
            SaveSettings()
            Checkerboard.Refresh() 'Reloads the animation tester section.
        End If
    End Sub
    Private Sub PieceBtn_CheckedChanged() Handles PieceBtn.CheckedChanged
        If Not AnimationRunning Then
            If PieceBtn.Checked Then
                GeneralOptions = GeneralOptions.Remove(4, 1).Insert(4, "T")
            Else
                GeneralOptions = GeneralOptions.Remove(4, 1).Insert(4, "F")
            End If
            SaveSettings()
        End If
    End Sub
    Private Sub TouchMoveBtn_CheckedChanged() Handles TouchMoveBtn.CheckedChanged
        If TouchMoveBtn.Checked Then
            GeneralOptions = GeneralOptions.Remove(5, 1).Insert(5, "T")
        Else
            GeneralOptions = GeneralOptions.Remove(5, 1).Insert(5, "F")
        End If
        If Not AnimationRunning Then SaveSettings()
    End Sub
    Private Sub InvisBtn_CheckedChanged() Handles InvisBtn.CheckedChanged
        If InvisBtn.Checked Then
            GeneralOptions = GeneralOptions.Remove(6, 1).Insert(6, "T")
            'Removes all the pictures from the PictureBoxes.
            WP1.Image = Nothing
            WP2.Image = Nothing
            WP3.Image = Nothing
            WR1.Image = Nothing
            WN1.Image = Nothing
            WB1.Image = Nothing
        Else
            GeneralOptions = GeneralOptions.Remove(6, 1).Insert(6, "F")
            'Restores the images back to the PictureBoxes.
            WP1.Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\WPawn.png")
            WP2.Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\WPawn.png")
            WP3.Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\WPawn.png")
            WR1.Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\WRook.png")
            WN1.Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\WKnight.png")
            WB1.Image = Image.FromFile(Application.StartupPath & "\Assets\Images\Default\WBishop.png")
        End If
        If Not AnimationRunning Then SaveSettings()
    End Sub
    Private Sub HammadBtn_CheckedChanged() Handles HammadBtn.CheckedChanged
        If HammadBtn.Checked Then
            GeneralOptions = GeneralOptions.Remove(7, 1).Insert(7, "T")
        Else
            GeneralOptions = GeneralOptions.Remove(7, 1).Insert(7, "F")
        End If
        If Not AnimationRunning Then SaveSettings()
    End Sub

    Private Sub FixedSearchBtn_CheckedChanged() Handles FixedSearchBtn.CheckedChanged
        If FixedSearchBtn.Checked Then
            If FixedSearchDepth <> Val(FixedSearchBox.Text) Then
                FixedSearchDepth = Val(FixedSearchBox.Text)
                If Not AnimationRunning Then SaveSettings()
            End If
            FixedSearchBox.Enabled = True
        Else
            FixedSearchBox.Enabled = False
            If FixedSearchDepth > 0 Then
                FixedSearchDepth = 0 '0 indicates variable search.
                If Not AnimationRunning Then SaveSettings()
            End If
        End If
    End Sub

    'Subroutine that handles the FixedSearchBox - preventing the user from entering any invalid inputs, along with calibrating FixedSearchDepth.
    Private Sub FixedSearchBox_TextChanged() Handles FixedSearchBox.TextChanged
        If FixedSearchBox.Enabled Then
            If FixedSearchBox.Text = "" Then
                'Sets the search to variable mode.
                If FixedSearchDepth > 0 Then
                    FixedSearchDepth = 0
                    SaveSettings()
                End If
            ElseIf FixedSearchBox.Text.Length >= 3 Then 'Depth cannot exceed 99.
                FixedSearchBox.Text = FixedSearchDepth
            ElseIf System.Text.RegularExpressions.Regex.IsMatch(FixedSearchBox.Text, "^[0-9]+$") Then 'Correct syntax - replace FixedSearchDepth.
                Try
                    If Val(FixedSearchBox.Text) <> FixedSearchDepth Then FixedSearchDepth = Val(FixedSearchBox.Text) : SaveSettings()
                Catch ex As Exception
                    FixedSearchBox.Text = FixedSearchDepth
                End Try
            Else
                'Removes the error made by the user.
                FixedSearchBox.Text = FixedSearchDepth
            End If
        End If
    End Sub



    'Subroutine that resets all the settings in the program, along with all user data.
    Private Sub ResetBtn_Click() Handles ResetBtn.Click
        If MsgBox("WARNING: You are attempting to reset this program." & vbCrLf & "This will erase your configurations & settings, along with all leaderboards, scores & ratings in the training modes." & vbCrLf & vbCrLf & "THIS CANNOT BE REVERSED." & vbCrLf & vbCrLf & "Would you like to proceed?", vbExclamation + vbYesNo + vbApplicationModal, "Reset Program") = 6 Then
            'Deletes all files in the User folder.
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Deleting Files...")
            Console.ResetColor()
            Dim UserDirectory As String = Application.StartupPath & "\Assets\User"
            For Each FileToDelete In Directory.GetFiles(UserDirectory, "*.txt", SearchOption.TopDirectoryOnly)
                File.Delete(FileToDelete)
            Next
            'Reconfigures the (default) settings.
            ConfigureSettings()
            Checkerboard.Refresh()
            SaveSettings()
        End If
    End Sub

    Private Sub OpeningBookBtn_CheckedChanged(sender As Object, e As EventArgs) Handles OpeningBookBtn.CheckedChanged

    End Sub

    'Button that closes the settings menu.
    Private Sub BackBtn_Click() Handles BackBtn.Click
        Me.Close()
    End Sub

End Class
