Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data
Imports System.Reflection

Public Class ClsDataAccessHelper

    Public Shared Sub AddParameter(ByRef cmd As SqlCommand, ByVal parameterName As String, ByVal value As Object, Optional ByVal dbType As SqlDbType = SqlDbType.VarChar)
        Dim param As New SqlParameter
        param = New SqlParameter()
        param.ParameterName = parameterName
        param.SqlDbType = dbType
        param.Value = value
        cmd.Parameters.Add(param)
    End Sub

    Public Shared Function FillDataTable(ByRef storedProcedureName As String, Optional ByVal parameters As List(Of SqlParameter) = Nothing, Optional ByVal commandType As CommandType = CommandType.StoredProcedure, Optional ByVal connectionString As String = Nothing) As DataTable
        Dim dataTable As DataTable = New DataTable()
        Try
            If String.IsNullOrEmpty(connectionString) Then
                connectionString = ConfigurationManager.ConnectionStrings("LogDb").ConnectionString
            End If
            Using cnx As SqlConnection = New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(storedProcedureName, cnx)
                cnx.Open()
                cmd.CommandTimeout = 600
                cmd.CommandType = commandType
                If Not parameters Is Nothing Then
                    For Each Parameter As SqlParameter In parameters
                        cmd.Parameters.Add(Parameter)

                    Next
                End If
                Dim adapter As SqlDataAdapter = New SqlDataAdapter(cmd)
                adapter.Fill(dataTable)
            End Using
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Methode Name:</b>Filldatatable</br><b>Stored Procedure Name:</b>{2}</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace, storedProcedureName)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        Finally
            connectionString = Nothing
        End Try
        Return dataTable
    End Function

    Public Shared Function ExecuteScalar(ByRef storedProcedureName As String, Optional ByVal parameters As List(Of SqlParameter) = Nothing) As Object
        Dim result As Object = Nothing
        Try
            Using cnx As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
                Dim cmd As New SqlCommand(storedProcedureName, cnx)
                cmd.CommandTimeout = 600
                cmd.CommandType = CommandType.StoredProcedure
                If Not parameters Is Nothing Then
                    For Each Parameter As SqlParameter In parameters
                        cmd.Parameters.Add(Parameter)

                    Next
                End If
                cnx.Open()
                result = cmd.ExecuteScalar()
                cnx.Close()
            End Using
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Methode Name:</b>ExecuteScalar</br><b>Stored Procedure Name:</b>{2}</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace, storedProcedureName)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
        Return result
    End Function

    Public Shared Function FillDataSet(ByRef storedProcedureName As String, Optional ByVal parameters As List(Of SqlParameter) = Nothing, Optional ByVal commandType As CommandType = CommandType.StoredProcedure) As DataSet
        Dim dataSet As DataSet = New DataSet()
        Try
            Using cnx As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
                Dim cmd As New SqlCommand(storedProcedureName, cnx)
                cnx.Open()
                cmd.CommandTimeout = 600
                cmd.CommandType = commandType
                If Not parameters Is Nothing Then
                    For Each Parameter As SqlParameter In parameters
                        cmd.Parameters.Add(Parameter)

                    Next
                End If
                Dim adapter As SqlDataAdapter = New SqlDataAdapter(cmd)
                adapter.Fill(dataSet)
            End Using
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Methode Name:</b>FillDataSet</br><b>Stored Procedure Name:</b>{2}</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace, storedProcedureName)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
        Return dataSet
    End Function


    Public Shared Function GetSQLCommand(ByRef storedProcedureName As String, Optional ByVal parameters As List(Of SqlParameter) = Nothing, Optional ByVal commandType As CommandType = CommandType.StoredProcedure, Optional connectionStringKeyName As String = "LogDb") As SqlCommand
        Dim cmd As SqlCommand = Nothing
        Try
            Using cnx As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings(connectionStringKeyName).ConnectionString)
                cmd = New SqlCommand(storedProcedureName, cnx)
                cnx.Open()
                cmd.CommandTimeout = 600
                cmd.CommandType = commandType
                If Not parameters Is Nothing Then
                    For Each Parameter As SqlParameter In parameters
                        cmd.Parameters.Add(Parameter)

                    Next
                End If
            End Using
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Methode Name:</b>GetSQLCommand</br><b>Stored Procedure Name:</b>{2}</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace, storedProcedureName)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
        Return cmd
    End Function

    Public Shared Function GetText(ByVal row As DataRow, ByVal columnName As String, Optional defaultValue As String = "") As String
        If Not row.Table.Columns.Contains(columnName) Then
            Return defaultValue
        End If

        If row.IsNull(columnName) Then
            Return defaultValue
        End If
        If row(columnName).GetType().Name = "DateTime" Then
            Return row(columnName).ToString()
        End If

        Return If(TryCast(row(columnName), String), defaultValue)
    End Function

    Public Shared Function ConvertToDataTable(Of T)(ByVal list As IList(Of T)) As DataTable
        Dim table As New DataTable()
        Dim fields() As FieldInfo = GetType(T).GetFields()
        For Each field As FieldInfo In fields
            table.Columns.Add(field.Name, field.FieldType)
        Next
        For Each item As T In list
            Dim row As DataRow = table.NewRow()
            For Each field As FieldInfo In fields
                row(field.Name) = field.GetValue(item)
            Next
            table.Rows.Add(row)
        Next
        Return table
    End Function

    Public Shared Function ExecuteNonQuery(ByRef storedProcedureName As String, Optional ByVal parameters As List(Of SqlParameter) = Nothing, Optional ByVal RETURN_VALUE As Boolean = False) As Boolean
        Dim runWithSuccess As Boolean = True
        Try
            Using cnx As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
                Dim cmd As New SqlCommand(storedProcedureName, cnx)
                cmd.CommandTimeout = 600
                cmd.CommandType = CommandType.StoredProcedure
                If Not parameters Is Nothing Then
                    For Each Parameter As SqlParameter In parameters
                        cmd.Parameters.Add(Parameter)

                    Next
                End If
                cnx.Open()
                cmd.ExecuteNonQuery()
                If RETURN_VALUE Then
                    If cmd.Parameters("@RETURN_VALUE").Value <> 0 Then
                        Return False
                    Else
                        Return True
                    End If
                End If
                cnx.Close()
            End Using
        Catch ex As Exception
            runWithSuccess = False
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Methode Name:</b>ExecuteNonQuery</br><b>Stored Procedure Name:</b>{2}</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace, storedProcedureName)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
        Return runWithSuccess
    End Function

    Public Shared Function CreateListFromTable(Of T As New)(ByVal tbl As DataTable) As List(Of T)
        Dim lst As List(Of T) = New List(Of T)()

        For Each r As DataRow In tbl.Rows
            lst.Add(CreateItemFromRow(Of T)(r))
        Next

        Return lst
    End Function

    Private Shared Function CreateItemFromRow(Of T As New)(ByVal row As DataRow) As T
        Dim item As T = New T()
        SetItemFromRow(item, row)
        Return item
    End Function

    Private Shared Sub SetItemFromRow(Of T As New)(ByVal item As T, ByVal row As DataRow)
        For Each c As DataColumn In row.Table.Columns
            Dim p As PropertyInfo = item.[GetType]().GetProperty(c.ColumnName)

            If p IsNot Nothing AndAlso row(c) IsNot DBNull.Value Then
                p.SetValue(item, row(c), Nothing)
            End If
        Next
    End Sub

End Class
