'Stack that holds the Zobrist Keys & PGNs of all the positions that have previously appeared in the game. Used to enforce
'three-fold repetition (both for the Chess form & for the AI), for FEN information, and to capture the PGN of the game.
Public Class GameHistory

    'Stacks for the Zobrist values. 'Buffer' holds the GameHistory of the previous position (used in case the user undos their move).
    Private ZobristMain(GlobalConstants.MaxPositionsPerGame - 1) As UInt64
    Private ZobristBuffer(GlobalConstants.MaxPositionsPerGame - 1) As UInt64

    'Stacks for the PGN values.
    Private PGNMain(GlobalConstants.MaxPositionsPerGame - 1) As String
    Private PGNBuffer(GlobalConstants.MaxPositionsPerGame - 1) As String

    Private MainSize, BufferSize As UInt16 '= Index of last item + 1

    Public Sub Clear()
        Move()
        MainSize = 0
    End Sub

    'Subroutine that swaps the GameHistory and Buffer arrays, along with their sizes.
    Public Sub Swap()
        Dim TempZobrist(1023) As UInt64
        Dim TempPGN(1023) As String
        Dim TempSize As Int16
        Array.Copy(ZobristMain, TempZobrist, MainSize)
        Array.Copy(PGNMain, TempPGN, MainSize)
        TempSize = MainSize
        Array.Copy(ZobristBuffer, ZobristMain, BufferSize)
        Array.Copy(PGNBuffer, PGNMain, BufferSize)
        MainSize = BufferSize
        Array.Copy(TempZobrist, ZobristBuffer, TempSize)
        Array.Copy(TempPGN, PGNBuffer, TempSize)
        BufferSize = TempSize
        'Removes the last FEN position of the (new) GameHistory array, as it will be later added by the EnforceEndStates() subroutine.
        PopZobrist()
    End Sub

    'Subroutine that copies the GameHistory array to the Buffer array, along with its size.
    Private Sub Move()
        Array.Copy(ZobristMain, ZobristBuffer, MainSize)
        Array.Copy(PGNMain, PGNBuffer, MainSize)
        BufferSize = MainSize
    End Sub

    'Subroutine that adds a new Zobrist Hash to the GameHistory array.
    Public Sub PushZobrist(ByVal Value As UInt64, ByVal MoveArray As Boolean)
        'MoveArray is used to signify if a new move has been played on the board (if it hasn't [ie: the user has undone the position]
        'then we don't overwrite Buffer, as the board hsan't actually changed).
        If MainSize < GlobalConstants.MaxPositionsPerGame - 1 Then
            If MoveArray Then Move()
            ZobristMain(MainSize) = Value
            MainSize += 1
        Else
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Error when adding Position Data to GameHistory: Array Full.")
            Console.ResetColor()
        End If
    End Sub

    'Subroutine that adds a new PGN Value to the GameHistory array.
    Public Sub PushPGN(ByVal Value As String)
        'As the Zobrist value will always be pushed before the PGN value (and hence MainSize will be incremented),
        'add the PGN value to the index = MainSize - 1.
        PGNMain(MainSize - 1) = Value
    End Sub

    'Function that removes the lastly-added position (FIFO structure) from the GameHistory array.
    Public Function PopZobrist() As UInt64
        If MainSize > 0 Then
            MainSize -= 1
            Return ZobristMain(MainSize)
        Else 'The array is empty.
            Return 0
        End If
    End Function

    'Function that checks how many times the input Zobrist Key is present in the GameHistory array (used to enforce three-fold repetition).
    Public Function CheckNoOfZobristOccurances(ByVal Value As UInt64) As Byte
        CheckNoOfZobristOccurances = 0
        For n = 0 To MainSize - 1
            If ZobristMain(n) = Value Then
                CheckNoOfZobristOccurances += 1
                If CheckNoOfZobristOccurances = 3 Then Exit For 'There will never be >3 occurances, as if this were true then the game would end.
            End If
        Next
    End Function

    Public Function GetSize() As UInt16
        Return MainSize
    End Function

    'Function that returns the GameHistory array, but without any empty space (so that processing can be done quicker).
    Public Function GetZobristArray() As UInt64()
        Dim TempArray(MainSize - 1) As UInt64
        Array.Copy(ZobristMain, TempArray, MainSize)
        Return TempArray
    End Function

    'Function which gets the set of PGNs held in the Main GameHistory array, and formats it into a string.
    Public Function GetFormattedPGNString(ByVal GameEnded As Boolean) As String
        GetFormattedPGNString = ""
        If MainSize > 0 Then
            'Adds each PGN to the string.
            For n = 1 To MainSize - 1
                'PGNs are formatted as such: "1. WMove BMove 2. WMove BMove 3. ..."
                '... so add the move number for every other move.
                If n Mod 2 = 1 Then GetFormattedPGNString &= (n \ 2 + 1) & ". "
                GetFormattedPGNString &= PGNMain(n) & " "
            Next
            GetFormattedPGNString = GetFormattedPGNString.TrimEnd(" ")
            'If the game has been terminated, we add the # symbol at the end to represent this.
            If GameEnded Then GetFormattedPGNString &= "#"
        End If
    End Function

End Class
