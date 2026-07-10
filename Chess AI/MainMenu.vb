Imports System.Threading

'Class that allows the user to access the full extent of my program.
'Will be the first section of code that the user sees, and provides the starting 
'point for instantiating all theother classes & forms in my program.
Public Class MainMenu
    Dim PrimaryColour As Color 'Colour representing the dark squares on the Chessboard.
    Dim SecondaryColour As Color 'Colour representing the light squares on the Chessboard.
    Dim ColourScheme As String 'Colour code from the text file.
    'Startup sound effect - sounds taken from Chess.com.
    ReadOnly Sound_Startup As New Media.SoundPlayer With {
        .SoundLocation = Application.StartupPath & "\Sounds\Chess_Startup.wav"
    }

    Private Sub MainMenu_Load() Handles Me.Load
        'Algorithm which retrieves the colour scheme from a file (represented by mnemonics).
        Try
            FileOpen(1, Application.StartupPath & "\ColourPreferences.txt", OpenMode.Input)
            ColourScheme = LineInput(1)
            FileClose(1)
        Catch ex As Exception
            ColourScheme = "def"
        End Try
        CreateColourProfile(ColourScheme)
        'Sets up the text, buttons and objects.
        SetupOptions()
        SetupPieces()
    End Sub

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
        For x = 0 To 31
            Pieces(x).Visible = False
        Next

        Dim Colour As String = "W"
        For x = 0 To 16 Step 16
            Pieces(x).Image = Image.FromFile(Application.StartupPath & "\Images\Default\" & Colour & "King.png")
            Pieces(x + 1).Image = Image.FromFile(Application.StartupPath & "\Images\Default\" & Colour & "Queen.png")
            For n = x + 2 To x + 3
                Pieces(n).Image = Image.FromFile(Application.StartupPath & "\Images\Default\" & Colour & "Bishop.png")
            Next
            For n = x + 4 To x + 5
                Pieces(n).Image = Image.FromFile(Application.StartupPath & "\Images\Default\" & Colour & "Knight.png")
            Next
            For n = x + 6 To x + 7
                Pieces(n).Image = Image.FromFile(Application.StartupPath & "\Images\Default\" & Colour & "Rook.png")
            Next
            For n = x + 8 To x + 15
                Pieces(n).Image = Image.FromFile(Application.StartupPath & "\Images\Default\" & Colour & "Pawn.png")
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

    Private Sub SetupOptions()
        'Sets the location of all the labels and buttons, and makes them invisible.
        Title.Visible = False
        Title.Location = New Point(221, 160)
        Title.BringToFront()

        OnePlayer.Visible = False
        OnePlayer.Location = New Point(127.5, 233)
        OnePlayer.FlatAppearance.BorderSize = 0
        OnePlayer.BringToFront()

        TwoPlayer.Visible = False
        TwoPlayer.Location = New Point(352.5, 233)
        TwoPlayer.FlatAppearance.BorderSize = 0
        TwoPlayer.BringToFront()

        Analysis.Visible = False
        Analysis.Location = New Point(127.5, 308)
        Analysis.FlatAppearance.BorderSize = 0
        Analysis.BringToFront()

        ExitBtn.Visible = False
        ExitBtn.Location = New Point(352.5, 307)
        ExitBtn.FlatAppearance.BorderSize = 0
        ExitBtn.BringToFront()

        Credits.Visible = False
        Credits.Location = New Point(191, 404)
        Credits.BringToFront()
    End Sub

    Private Sub StartupAnimation() Handles Me.Activated
        Dim TempColour As String
        Try
            FileOpen(1, Application.StartupPath & "\ColourPreferences.txt", OpenMode.Input)
            TempColour = LineInput(1)
            FileClose(1)
            If TempColour <> ColourScheme Then
                ColourScheme = TempColour
                CreateColourProfile(ColourScheme)
                MyBase.Refresh()
            End If
        Catch ex As Exception
        End Try

        Dim AlreadyBooted As Boolean
        If WP8.Visible = True Then AlreadyBooted = True
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
            Thread.Sleep(100)
        Next
        Thread.Sleep(50)

        'Slowly makes the pieces on the board visible (in a spiral pattern).
        WK1.Visible = True
        WQ1.Visible = True
        BK1.Visible = True
        BQ1.Visible = True
        Application.DoEvents()
        Thread.Sleep(150)
        WB1.Visible = True
        WB2.Visible = True
        BB1.Visible = True
        BB2.Visible = True
        Application.DoEvents()
        Thread.Sleep(150)
        WN1.Visible = True
        WN2.Visible = True
        BN1.Visible = True
        BN2.Visible = True
        Application.DoEvents()
        Thread.Sleep(150)
        WR1.Visible = True
        WR2.Visible = True
        BR1.Visible = True
        BR2.Visible = True
        Application.DoEvents()
        Thread.Sleep(150)
        WP1.Visible = True
        WP2.Visible = True
        BP1.Visible = True
        BP2.Visible = True
        Application.DoEvents()
        Thread.Sleep(150)
        WP3.Visible = True
        WP4.Visible = True
        BP3.Visible = True
        BP4.Visible = True
        Application.DoEvents()
        Thread.Sleep(150)
        WP5.Visible = True
        WP6.Visible = True
        BP5.Visible = True
        BP6.Visible = True
        Application.DoEvents()
        Thread.Sleep(150)
        WP7.Visible = True
        WP8.Visible = True
        BP7.Visible = True
        BP8.Visible = True
        Application.DoEvents()
        Thread.Sleep(150)

        'Makes the labels & buttons visible, then audibly notifies the user that they can choose a menu option.
        Title.Visible = True
        OnePlayer.Visible = True
        TwoPlayer.Visible = True
        Analysis.Visible = True
        ExitBtn.Visible = True
        Credits.Visible = True
        If Not AlreadyBooted Then Sound_Startup.Play()
    End Sub


    'Controls for the buttons.
    Private Sub OnePlayer_MouseEnter() Handles OnePlayer.MouseEnter
        'Grows the Button.
        OnePlayer.Size = New Size(150, 75)
        OnePlayer.Left -= 15
        OnePlayer.Top -= 7.5
        OnePlayer.Font = New Font("Microsoft Sans Serif", 24, FontStyle.Bold)
    End Sub
    Private Sub OnePlayer_MouseLeave() Handles OnePlayer.MouseLeave
        'Shrinks the Button.
        OnePlayer.Size = New Size(120, 60)
        OnePlayer.Left += 15
        OnePlayer.Top += 7.5
        OnePlayer.Font = New Font("Microsoft Sans Serif", 18, FontStyle.Bold)
    End Sub
    Private Sub OnePlayer_Click() Handles OnePlayer.Click
        Dim Customisation As New OnePlayerCustomisation
        Customisation.Show()
        Me.Hide()
    End Sub

    Private Sub TwoPlayer_MouseEnter() Handles TwoPlayer.MouseEnter
        'Grows the Button.
        TwoPlayer.Size = New Size(150, 75)
        TwoPlayer.Left -= 15
        TwoPlayer.Top -= 7.5
        TwoPlayer.Font = New Font("Microsoft Sans Serif", 24, FontStyle.Bold)
    End Sub
    Private Sub TwoPlayer_MouseLeave() Handles TwoPlayer.MouseLeave
        'Shrinks the Button.
        TwoPlayer.Size = New Size(120, 60)
        TwoPlayer.Left += 15
        TwoPlayer.Top += 7.5
        TwoPlayer.Font = New Font("Microsoft Sans Serif", 18, FontStyle.Bold)
    End Sub
    Private Sub TwoPlayer_Click() Handles TwoPlayer.Click
        Dim Customisation As New TwoPlayerCustomisation
        Customisation.Show()
        Me.Hide()
    End Sub

    Private Sub Analysis_MouseEnter() Handles Analysis.MouseEnter
        'Grows the Button.
        Analysis.Size = New Size(150, 75)
        Analysis.Left -= 15
        Analysis.Top -= 7.5
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
        Dim ChessGame As New Chess()
        ChessGame.Show()
        Me.Hide()
    End Sub

    Private Sub ExitBtn_MouseEnter() Handles ExitBtn.MouseEnter
        'Grows the Button.
        ExitBtn.Size = New Size(150, 75)
        ExitBtn.Left -= 15
        ExitBtn.Top -= 7.5
        ExitBtn.Font = New Font("Microsoft Sans Serif", 24, FontStyle.Bold)
    End Sub
    Private Sub ExitBtn_MouseLeave() Handles ExitBtn.MouseLeave
        'Shrinks the Button.
        ExitBtn.Size = New Size(120, 60)
        ExitBtn.Left += 15
        ExitBtn.Top += 7.5
        ExitBtn.Font = New Font("Microsoft Sans Serif", 18, FontStyle.Bold)
    End Sub
    Private Sub ExitBtn_Click() Handles ExitBtn.Click
        Me.Close()
    End Sub

End Class
