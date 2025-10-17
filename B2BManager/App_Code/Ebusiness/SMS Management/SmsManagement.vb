Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic

Public Class SmsManagement
    Public Shared Function GetKeyValue(ByVal environmentID As Integer, ByVal keyName As String, ByVal SOPID As String) As String
        Try
            Using cnx As New SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
                cnx.Open()

                Using cmd As New SqlCommand("[Ebusiness].[SmsMgmt_GetKeyValue]", cnx)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@EnvironmentID", environmentID)
                    cmd.Parameters.AddWithValue("@KeyName", keyName)
                    cmd.Parameters.AddWithValue("@SOPID", SOPID)

                    Using Adapter As New SqlDataAdapter(cmd)
                        Using ds As New DataSet()
                            Adapter.Fill(ds, "Result")

                            If ds.Tables("Result").Rows.Count > 0 Then
                                Return ds.Tables("Result").Rows(0)("SD_Value").ToString()
                            Else
                                Return Nothing
                            End If
                        End Using
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Class
