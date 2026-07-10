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
        QS = Copier.KS
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
