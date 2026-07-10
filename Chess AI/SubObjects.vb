'Below are the subobjects containing information about castling & checking rules,
'along with some getter (used mainly to reset privileges) & setter functions.
Public Class InCheck
    Public IsInCheck As Boolean
    Public Piece As String
    Public DoubleCheck As Boolean
    Public Sub New()
        Piece = " "
    End Sub
    Public Sub NotInCheck() 'Resets all rules.
        IsInCheck = False
        Piece = " "
        DoubleCheck = False
    End Sub
    Public Sub CopyFrom(ByVal Copier As InCheck) 'Copies all the data from the parameter class.
        IsInCheck = Copier.IsInCheck
        Piece = Copier.Piece
        DoubleCheck = Copier.DoubleCheck
    End Sub
End Class
Public Class CanCastle
    Public KS As Boolean
    Public QS As Boolean
    Public Sub CannotCastle() 'Resets Castling Privileges.
        KS = False
        QS = False
    End Sub
    Public Sub CopyFrom(ByVal Copier As CanCastle) 'Copies all the data from the parameter class.
        KS = Copier.KS
        QS = Copier.QS
    End Sub
    Public Function CanICastle() As Boolean 'Returns True if any castling privileges exist.
        Return (KS OrElse QS)
    End Function
End Class


Public Structure Move 'Information about an AI's move.
    Public Score As Decimal 'Move Evaluation.
    Public OldMoveX As String
    Public OldMoveY As String
    Public NewMoveX As String
    Public NewMoveY As String
    Public EndState As Char 'Contains information about an AI's search:
    'f = no end-state detected, a = search manually aborted, c = checkmate found, s = stalemate, o = one move found (forced)
End Structure


'Class that holds a position in my Opening Book. Contains the FEN string for the position, along with all the moves which stem from that
'position (along with how frequently they occur)
Public Class OpeningBookEntry
    Private FEN As String
    Private Count As UInt32 'Total number of times this position has been reached in the book.
    Private MoveList As New List(Of String) 'List of PGN moves.
    Private MoveCount As New List(Of UInt32) 'List of how many times each PGN move has occured in the book.

    'Subroutine that takes the entry in the txt Opening Book file (in the format: "FEN{Count}:Move1{Count},Move2{Count},..."),
    'and formats this data into the attributes.
    Public Sub New(ByVal UserBookEntry As String)
        'Splits entry into FEN & Moves info.
        Dim ColonIndex As Byte = UserBookEntry.IndexOf(":")
        'Extracts FEN info.
        Dim FENInfo As String() = UserBookEntry.Substring(0, ColonIndex).Split({"{", "}"}, StringSplitOptions.RemoveEmptyEntries)
        FEN = FENInfo(0)
        Count = Val(FENInfo(1))

        'Extracts Move info.
        Dim MoveInfo As String() = UserBookEntry.Substring(ColonIndex + 1, UserBookEntry.Length - (ColonIndex + 1)).Split(",")
        For Each Entry In MoveInfo
            Dim TempMove As String() = Entry.Split({"{", "}"}, StringSplitOptions.RemoveEmptyEntries)
            MoveList.Add(TempMove(0))
            MoveCount.Add(TempMove(1))
        Next
    End Sub

    Public Function GetFEN() As String
        Return FEN
    End Function


    'Function that returns all the moves for this position.
    Public Function ReturnAllMoves() As String
        Dim TempStr As String = ""
        For Each Entry In MoveList
            TempStr &= Entry & ", "
        Next
        TempStr = TempStr.TrimEnd(" ")
        Return TempStr.TrimEnd(",")
    End Function

    'Function that returns a random move in the move list. Weighted so that more common moves are more likely to be returned.
    Private WeightingFactor As Decimal = 1 'Defines how likely uncommon moves are to be returned (1 = no bias).
    Public Function ReturnRndMove() As String
        Static RND As New Random()
        Dim RNDValue As UInt32 = Math.Truncate(RND.Next(1, Count) ^ WeightingFactor)
        Dim WeightedMoveCountIndex As UInt32
        For n = 0 To MoveList.Count - 1
            WeightedMoveCountIndex = Math.Truncate(MoveCount(n) ^ WeightingFactor)
            If WeightedMoveCountIndex >= RNDValue Then Return MoveList(n)
            RNDValue -= WeightedMoveCountIndex
        Next
    End Function

End Class



'Stack that holds the Zobrist Keys of all the positions that have previously appeared in the game. Used to enforce three-fold repetition, and for FEN information.
Public Class GameHistory
    Private GameHistoryArray(1023) As UInt64
    Private Buffer(1023) As UInt64 'Holds the GameHistory of the previous position (used in case the user undos their move).
    Private MainSize, BufferSize As uInt16 '= Index of last item + 1

    Public Sub Clear()
        Move()
        MainSize = 0
    End Sub

    'Subroutine that swaps the GameHistory and Buffer arrays, along with their sizes.
    Public Sub Swap()
        Dim TempSet(1023) As UInt64
        Dim TempSize As Int16
        Array.Copy(GameHistoryArray, TempSet, MainSize)
        TempSize = MainSize
        Array.Copy(Buffer, GameHistoryArray, BufferSize)
        MainSize = BufferSize
        Array.Copy(TempSet, Buffer, TempSize)
        BufferSize = TempSize
        'Removes the last position of the (new) GameHistory array, as it will be later added by the EnforceEndStates subroutine.
        Pop()
    End Sub

    'Subroutine that copies the GameHistory array to the Buffer array, along with its size.
    Private Sub Move()
        Array.Copy(GameHistoryArray, Buffer, MainSize)
        BufferSize = MainSize
    End Sub

    'Subroutine that adds a new Zobrist Hash to the GameHistory array.
    Public Sub Push(ByVal Value As UInt64, ByVal MoveArray As Boolean)
        'MoveArray is used to signify if a new move has been played on the board (if it hasn't [ie: the user has undone the position]
        'then we don't overwrite Buffer, as the board hsan't actually changed).
        If MoveArray Then Move()
        GameHistoryArray(MainSize) = Value
        MainSize += 1
    End Sub

    'Function that removes the lastly-added position (FIFO structure) from the GameHistory array.
    Public Function Pop() As UInt64
        If MainSize > 0 Then
            MainSize -= 1
            Return GameHistoryArray(MainSize)
        Else 'The array is empty.
            Return 0
        End If
    End Function

    'Function that checks how many times the input Zobrist Key is present in the GameHistory array (used to enforce three-fold repetition).
    Public Function CheckNoOfOccurances(ByVal Value As UInt64) As Byte
        CheckNoOfOccurances = 0
        For n = 0 To MainSize - 1
            If GameHistoryArray(n) = Value Then
                CheckNoOfOccurances += 1
                If CheckNoOfOccurances = 3 Then Exit For 'There will never be >3 occurances, as if this were true then the game would end.
            End If
        Next
    End Function

    Public Function GetSize() As Uint16
        Return MainSize
    End Function

    'Function that returns the GameHistory array.
    Public Function GetArray() As UInt64()
        Dim TempArray(MainSize - 1) As UInt64
        Array.Copy(GameHistoryArray, TempArray, MainSize)
        Return TempArray
    End Function

End Class
