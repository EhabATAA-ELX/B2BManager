Imports Microsoft.VisualBasic

Public Class ClsEManagerHelper
    Public Enum InitMode
        View = 0
        Create = 1
        Update = 2
        Locked = 3
        Unlocked = 4
        Deleted = 5
        CustomerDeleted = 6
        MasterDeleted = 7
        NewView = 8
    End Enum

    Public Shared Function GetConectionString(ByRef Environnement As Integer) As String
        If (Environnement = 47) Then
            Return ConfigurationManager.ConnectionStrings("SQLCnx_Prod").ConnectionString
        Else
            Return ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString
        End If
    End Function

    Public Shared Function GetTPConectionString(ByRef Environnement As Integer) As String
        Select Case Environnement
            Case 47
                Return ConfigurationManager.ConnectionStrings("TPProd").ConnectionString
            Case 48
                Return ConfigurationManager.ConnectionStrings("TPStaging").ConnectionString
            Case Else
                Return ConfigurationManager.ConnectionStrings("TPTest").ConnectionString
        End Select
    End Function

    Public Shared Function GetTPHistoConectionString(ByRef Environnement As Integer) As String
        If (Environnement = 47) Then
            Return ConfigurationManager.ConnectionStrings("TPHistoProd").ConnectionString
        Else
            Return ConfigurationManager.ConnectionStrings("TPHisto").ConnectionString
        End If
    End Function
End Class
