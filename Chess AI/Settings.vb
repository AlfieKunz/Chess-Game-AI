Imports System.Threading
Public Class Settings
    Dim PrimaryColour As Color 'Colour representing the dark squares on the Chessboard.
    Dim SecondaryColour As Color 'Colour representing the light squares on the Chessboard.
    Dim ColourScheme As String 'Colour code from the text file.
    Dim AnimationSpeed As Byte = 2
    Private Sub Settings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Checkerboard.Controls.Add(Piece)
        Piece.Location = New Point(0, 0)
        Piece.BringToFront()
        'Algorithm which retrieves the colour scheme from a file (represented by mnemonics).
        Try
            FileOpen(1, Application.StartupPath & "\Assets\ColourPreferences.txt", OpenMode.Input)
            ColourScheme = LineInput(1)
            FileClose(1)
        Catch ex As Exception
            ColourScheme = "def"
        End Try
        CreateColourProfile(ColourScheme)
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
    Private Sub CreateBoard(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Checkerboard.Paint
        Dim g As Graphics = e.Graphics
        Dim isLight As Boolean = True
        'Creates checkerboard pattern. (Alternates between light and dark colours.)
        For x = 0 To 3
            Dim square As New Rectangle(150 * x, 0, 150, 150)
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
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Me.Activated
        Dim Constant As Decimal
        Application.DoEvents()
        Thread.Sleep(1000)

        While True
            Application.DoEvents()
            If AnimationSpeed = 0 Then
                Constant = 1
            ElseIf AnimationSpeed = 1 Then
                Constant = 25
            Else
                Constant = 75
            End If

            For n = 1 To -1 Step -2
                For x = 1 To Constant
                    Piece.Left += n * (450 / Constant)
                    If AnimationSpeed = 2 Then Piece.Refresh() Else Application.DoEvents()
                Next
                Thread.Sleep(1000)
            Next
        End While
    End Sub

    Private Sub SpeedSetter_ValueChanged(sender As Object, e As EventArgs) Handles SpeedSetter.ValueChanged
        AnimationSpeed = SpeedSetter.Value
    End Sub
End Class