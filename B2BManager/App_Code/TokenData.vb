Imports Microsoft.VisualBasic

' This is the full content of your new file: App_Code/TokenData.vb
Public Class TokenData
    Public EnvironmentId As String
    Public SopId As String
    Public DocumentIds As String ' Semicolon-separated Guids
    Public FocusRangeId As String
    Public IsStatic As Boolean?
    Public Mode As String
    Public ConditionID As Guid?
    Public PageSource As String

    ' --- ONE CONSTRUCTOR TO HANDLE ALL CASES ---
    Public Sub New(ByVal environmentId As String, ByVal sopId As String, ByVal isStatic As Boolean?, ByVal mode As String, ByVal pageSource As String,
                    Optional ByVal documentIds As String = "",
                    Optional ByVal focusRangeId As String = "",
                    Optional ByVal conditionID As Guid? = Nothing)

        Me.EnvironmentId = environmentId
        Me.SopId = sopId
        Me.IsStatic = isStatic
        Me.Mode = mode
        Me.ConditionID = conditionID
        Me.PageSource = pageSource

        ' Safely handle the optional parameters
        Me.DocumentIds = documentIds
        Me.FocusRangeId = focusRangeId
    End Sub

End Class