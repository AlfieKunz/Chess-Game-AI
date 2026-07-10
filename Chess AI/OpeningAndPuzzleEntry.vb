'This file holds the classes representing an entry for the OpeningBook list, and an entry for the PuzzleDatabase list.
'As these entries are constructed upon boot-up, and are loaded from a txt file, it is ideal to do as little computing
'on each entry (as millions of entries need to be loaded into the system as quickly as possible), and compute each
'individual entry only when we need to process / access information from it.

'Hence, the construction methods for each of the below classes merely abstract the core identifier(s) from the input
'(the entry's FEN in the case of OpeningBookEntry, and Rating in the case of PuzzleEntry), and stores the rest of the
'input in the Details attribute. These core attributes are used by the program to sort through the list of entries to
'select a specific entry. We then perform the ComputeEntry() method on the entry to fully construct the class's extra
'information. This greatly reduces the time it takes to load, for example, the Opening Book, into memory when the
'program initially boots. The Boolean attribute HasBeenComputed specifies whether the entry has been fully constructed
'using the ComputeEntry() method.



'Class that holds a position in my Opening Book. Contains the FEN string for the position, along with all the moves which stem from that
'position (along with how frequently they occur)
Imports System.Reflection

Public Class OpeningBookEntry
    Private ReadOnly FEN As String
    Private ReadOnly Details As String
    Private HasBeenComputed As Boolean

    Private Count As UInt32 'Total number of times this position has been reached in the book.
    Private MoveList As List(Of String) 'List of PGN moves.
    Private MoveCount As List(Of UInt32) 'List of how many times each PGN move has occured in the book.

    'Constructor sub, that abstracts the FEN from the user's input.
    Public Sub New(ByVal UserBookEntry As String)
        Dim ColonIndex As Byte = UserBookEntry.IndexOf(":")
        FEN = UserBookEntry.Substring(0, ColonIndex)
        Details = UserBookEntry.Substring(ColonIndex + 1)
    End Sub

    'Subroutine that takes the entry in the txt Opening Book file (in the format: "FEN:Count=Move1{Count},Move2{Count},..."),
    'and formats this data into the attributes.
    Public Sub ComputeEntry()
        MoveList = New List(Of String)
        MoveCount = New List(Of UInt32)

        'Separates the input into the total move count, and the list of Opening moves that stem from this position.
        Dim TempStr As String() = Details.Split("=")
        Count = TempStr(0)
        Dim MoveInfo As String() = TempStr(1).Split(",")

        'Breaks these moves into their constituents, containing the move itself, along with how many times it appears in the book.
        For Each Entry In MoveInfo
            Dim TempMove As String() = Entry.Split({"{", "}"}, StringSplitOptions.RemoveEmptyEntries)
            MoveList.Add(TempMove(0))
            MoveCount.Add(TempMove(1))
        Next
        HasBeenComputed = True
    End Sub

    Public Function GetComputed() As Boolean
        Return HasBeenComputed
    End Function
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
    Public Function ReturnRndMove() As String
        Dim WeightingFactor As Decimal = 0.66 'Defines how likely uncommon moves are to be returned (1 = no bias, lower numher = more bias).
        Static RND As New Random()
        Dim UpdatedCount As UInt32
        If WeightingFactor = 1 Then
            UpdatedCount = Count
        Else
            'Calculates the new total number of moves in the opening book, when all the moves have been raised to the power of the weighting factor.
            For Each Move In MoveCount
                UpdatedCount += Math.Round(Move ^ WeightingFactor)
            Next
        End If

        'Generates a random number in the range 1 - TotalMoveCount, then picks the move corresponding to that index (via subtraction).
        Dim RNDValue As UInt32 = RND.Next(1, UpdatedCount + 1)
        Dim WeightedMoveCountIndex As UInt32
        For n = 0 To MoveList.Count - 1
            WeightedMoveCountIndex = Math.Round(MoveCount(n) ^ WeightingFactor)
            If WeightedMoveCountIndex >= RNDValue Then Return MoveList(n) 'Move has been selected.
            RNDValue -= WeightedMoveCountIndex
        Next
        Return ""
    End Function

End Class





'Class that holds a puzzle, that will be stored in the puzzle database. Contains a puzzle rating (which acts as an identifier when
'searching for a random puzzle - the puzzle database is sorted by rating), the puzzle's FEN, and the set of puzzle moves (note: 
'the 2nd move of MoveList is the first puzzle move to find).
Public Class PuzzleEntry
    Private ReadOnly Rating As Int16
    Private ReadOnly Details As String
    Private HasBeenComputed As Boolean

    Private FEN As String
    Private MoveList As List(Of Move) 'Set of puzzle moves.


    'Constructor sub, that abstracts the puzzle's rating from the user's input.
    Public Sub New(ByVal UserPuzzleEntry As String)
        Dim ColonIndex As Byte = UserPuzzleEntry.IndexOf(":")
        Rating = UserPuzzleEntry.Substring(0, ColonIndex)
        Details = UserPuzzleEntry.Substring(ColonIndex + 1)
    End Sub

    'Subroutine that takes the entry in the txt Puzzle Databse file (in the format: "Rating:FEN=Move1Old.Move1New,Move2Old.Move2New,..."),
    'and formats this data into the attributes.
    Public Sub ComputeEntry()
        MoveList = New List(Of Move)

        'Separates the FEN from the list of puzzle moves.
        FEN = Details.Substring(0, Details.IndexOf("="))
        Dim MoveInfo As String() = (Details.Substring(Details.IndexOf("=") + 1)).Split(",")

        'Constructs the moves from the string of puzzle moves, then adds this to the MoveList attribute.
        Dim TempMove As Move
        For Each Entry In MoveInfo
            TempMove = New Move With {
                .OldMoveX = Entry(0),
                .OldMoveY = Entry(1),
                .NewMoveX = Entry(3),
                .NewMoveY = Entry(4)
            }
            If Entry.Length > 5 Then TempMove.Code = UCase(Entry(Entry.Length - 1)) 'Adds promotion flag to move, if needed.
            MoveList.Add(TempMove)
        Next

        HasBeenComputed = True
    End Sub

    'Getter & Setter methods for the above attributes.
    Public Function GetRating() As UInt16
        Return Rating
    End Function
    Public Function GetFEN() As String
        Return FEN
    End Function

    Public Function GetMove(ByVal Index As Byte) As Move
        'Retrieves a specific move in the list.
        Return If(HasBeenComputed, MoveList(Index), Nothing)
    End Function
    Public Function GetAllMoves() As List(Of Move)
        Return If(HasBeenComputed, MoveList, Nothing)
    End Function
    Public Function GetMoveCount() As Byte
        Return If(HasBeenComputed, MoveList.Count, Nothing)
    End Function

End Class
