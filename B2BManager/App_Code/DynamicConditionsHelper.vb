Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic


Public Class DynamicConditionsHelper

    ''' <summary>
    ''' Calls the PI_ConditionMultiDocument stored procedure.
    ''' </summary>
    ''' <param name="documentList">A semicolon-separated string of Document IDs.</param>
    ''' <returns>The GUID of the newly created Condition.</returns>
    Public Function CreateConditionFromDocumentList(documentList As String, conditionName As String, isStatic As Boolean, sopName As String) As Guid
        Dim newConditionId As Guid = Guid.NewGuid()
        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString

        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand("dbo.PI_ConditionMultiDocument", conn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("@ConditionID", newConditionId)
                cmd.Parameters.AddWithValue("@DocumentLIST", documentList)
                cmd.Parameters.AddWithValue("@ConditionName", conditionName)
                cmd.Parameters.AddWithValue("@ConditionUpdatedBy", ClsSessionHelper.LogonUser.FullName)
                cmd.Parameters.AddWithValue("@ConditionIsStatic", isStatic)
                cmd.Parameters.AddWithValue("@SOPName", sopName)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        ' Return the new ID so we can use it in the redirect
        Return newConditionId
    End Function

    ''' <summary>
    ''' Creates a new condition record in the database by calling the PI_Condition stored procedure.
    ''' </summary>
    ''' <param name="conditionName">The name for the new condition.</param>
    ''' <param name="isStatic">A boolean indicating if the condition is static.</param>
    ''' <param name="documentId">Optional:The DocumentID to associate this condition with.</param>
    ''' <param name="userId">Optional: The UserID to associate with the condition.</param>
    ''' <param name="focusRangeId">Optional: The FocusRangeID to associate with the condition.</param>
    ''' <returns>The GUID of the newly created ConditionID.</returns>
    Public Function CreateCondition(conditionName As String, isStatic As Boolean, country As String, documentId As Guid?, userId As Guid?, focusRangeId As Guid?) As Guid
        ' Generate a new GUID for the condition.
        Dim newConditionId As Guid = Guid.NewGuid()

        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString
        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand("dbo.PI_Condition", conn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("@ConditionID", newConditionId)
                cmd.Parameters.AddWithValue("@ConditionName", conditionName)
                cmd.Parameters.AddWithValue("@ConditionUpdatedBy", ClsSessionHelper.LogonUser.FullName)
                cmd.Parameters.AddWithValue("@ConditionIsStatic", isStatic)
                cmd.Parameters.AddWithValue("@SOPName", country)

                If documentId.HasValue Then
                    cmd.Parameters.AddWithValue("@DocumentID", documentId.Value)
                Else
                    cmd.Parameters.AddWithValue("@DocumentID", DBNull.Value)
                End If

                If userId.HasValue Then
                    cmd.Parameters.AddWithValue("@UserID", userId.Value)
                Else
                    cmd.Parameters.AddWithValue("@UserID", DBNull.Value)
                End If

                If focusRangeId.HasValue Then
                    cmd.Parameters.AddWithValue("@FocusRangeID", focusRangeId.Value)
                Else
                    cmd.Parameters.AddWithValue("@FocusRangeID", DBNull.Value)
                End If

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
        Return newConditionId
    End Function

    ''' <summary>
    ''' Gets the DocumentName for a specific DocumentID from the T_DOCUMENTS table.
    ''' </summary>
    ''' <param name="documentId">The ID of the document to find.</param>
    ''' <returns>The DocumentName as a string if found; otherwise, an empty string.</returns>
    Public Function GetDocumentNameByID(ByVal documentId As Guid) As String
        Dim documentName As String = String.Empty
        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2BCMS").ConnectionString
        Dim sql As String = "SELECT DocumentName FROM T_DOCUMENTS WHERE DocumentID = @DocumentID;"

        Try
            Using conn As New SqlConnection(connString)
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@DocumentID", documentId)
                    conn.Open()
                    Dim result As Object = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                        documentName = result.ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
            '  ShowError("Database Error: " & ex.Message)
            Return String.Empty
        End Try

        Return documentName
    End Function

    ''' <summary>
    ''' Gets the FocusRangeName for a specific FocusRangeId from the T_FocusRanges table.
    ''' </summary>
    ''' <param name="focusRangeId">The ID of the focus range to find.</param>
    ''' <returns>The FocusRangeName as a string if found; otherwise, an empty string.</returns>
    Public Function GetFocusRangeNameByID(ByVal focusRangeId As Guid) As String
        Dim focusRangeName As String = String.Empty
        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString
        Dim sql As String = "SELECT FocusRangeName FROM T_FocusRanges WHERE FocusRangeId = @FocusRangeId;"

        Try
            Using conn As New SqlConnection(connString)
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@FocusRangeId", focusRangeId)
                    conn.Open()
                    Dim result As Object = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                        focusRangeName = result.ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return String.Empty
        End Try

        Return focusRangeName
    End Function

    ''' <summary>
    ''' Fetches a single condition's details from the database.
    ''' </summary>
    ''' <param name="conditionId">The ID of the condition to retrieve.</param>
    ''' <returns>A Condition object with the data from the database.</returns>
    Public Function GetConditionByID(ByVal conditionId As Guid) As Condition
        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString
        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand("dbo.PS_CONDITION", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ConditionID", conditionId)
                conn.Open()
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        ' If a record was found, create and return the Condition object
                        Return New Condition With {
                        .conditionId = CType(reader("ConditionID"), Guid),
                        .ConditionName = reader("ConditionName").ToString(),
                        .ConditionUpdateDate = Convert.ToDateTime(reader("ConditionUpdateDate")).ToString("yyyy-MM-dd HH:mm"),
                        .ConditionUpdatedBy = reader("ConditionUpdatedBy").ToString()
                    }
                    End If
                End Using
            End Using
        End Using
        Return Nothing
    End Function

    Public Function GetCriteriaForCondition(ByVal conditionId As Guid) As List(Of Criterion)
        Dim criteriaList As New List(Of Criterion)()
        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString
        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand("dbo.PS_Condition_Criteria", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ConditionID", conditionId)
                conn.Open()
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        criteriaList.Add(New Criterion With {
                            .ConditionID = CType(reader("ConditionID"), Guid),
                            .CriteriaNumber = CType(reader("CriteriaNumber"), Byte),
                            .ConditionField = reader("ConditionField").ToString(),
                            .ConditionOperator = reader("ConditionOperator").ToString(),
                            .ConditionValues = reader("ConditionValues").ToString()
                        })
                    End While
                End Using
            End Using
        End Using
        Return criteriaList
    End Function

    ''' <summary>
    ''' Calls the PS_ConditionsByDocuments stored procedure.
    ''' </summary>
    ''' <param name="documentIds">A list of Document GUIDs to search for.</param>
    ''' <returns>A DocumentConditionsResult object containing all three result sets.</returns>
    Public Shared Function GetConditionsForDocuments(ByVal documentIds As IEnumerable(Of Guid)) As ClsDocument
        Dim result As New ClsDocument()

        ' If the input list is empty, return the default empty result object.
        If documentIds Is Nothing OrElse Not documentIds.Any() Then
            Return result
        End If

        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString

        ' 1. Convert the list of Guids into the semicolon-separated string the SP expects.
        Dim idListString As String = String.Join(";", documentIds)

        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand("dbo.PS_ConditionsByDocuments", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@DocumentIDs", idListString)

                ' 2. Use a DataSet to capture all three result sets.
                Dim ds As New DataSet()
                Using adapter As New SqlDataAdapter(cmd)
                    adapter.Fill(ds)
                End Using

                ' 3. Process the results from the DataSet.
                ' The first table (index 0) is the list of conditions.
                If ds.Tables.Count > 0 Then
                    result.Conditions = ds.Tables(0)
                End If

                ' The second table (index 1) contains the "ConditionStaticExists" flag.
                If ds.Tables.Count > 1 AndAlso ds.Tables(1).Rows.Count > 0 Then
                    result.StaticConditionExists = Convert.ToBoolean(ds.Tables(1).Rows(0)("ConditionStaticExists"))
                End If

                ' The third table (index 2) contains the "assignedCustomersNumber".
                ' If ds.Tables.Count > 2 AndAlso ds.Tables(2).Rows.Count > 0 Then
                '  result.AssignedCustomersNumber = Convert.ToInt32(ds.Tables(2).Rows(0)("assignedCustomersNumber"))
                ' End If
            End Using
        End Using

        Return result
    End Function

    ''' <summary>
    ''' Calls the PS_ConditionsByFocusRange stored procedure with a single FocusRangeID.
    ''' </summary>
    ''' <param name="focusRangeId">The single FocusRange GUID to search for.</param>
    ''' <returns>A FocusRangeQueryResult object containing the conditions and the static flag.</returns>
    Public Shared Function GetConditionsByFocusRange(ByVal focusRangeId As Guid) As ClsFocusRange
        Dim result As New ClsFocusRange()
        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString

        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand("dbo.PS_ConditionsByFocusRange", conn)
                cmd.CommandType = CommandType.StoredProcedure

                ' Add the FocusRangeID as a parameter
                cmd.Parameters.AddWithValue("@FocusRangeID", focusRangeId)

                ' Use a DataSet to capture both result sets from the stored procedure.
                Dim ds As New DataSet()
                Using adapter As New SqlDataAdapter(cmd)
                    adapter.Fill(ds)
                End Using

                ' The first table (index 0) is the list of conditions.
                If ds.Tables.Count > 0 Then
                    result.Conditions = ds.Tables(0)
                End If

                ' The second table (index 1) contains the single "ConditionStaticExists" value.
                If ds.Tables.Count > 1 AndAlso ds.Tables(1).Rows.Count > 0 Then
                    result.StaticConditionExists = Convert.ToBoolean(ds.Tables(1).Rows(0)(0))
                End If
            End Using
        End Using

        Return result
    End Function

    ''' <summary>
    ''' Aggregates customers from multiple conditions and returns a unique list.
    ''' </summary>
    ''' <param name="conditionIds">A list of ConditionIDs to process.</param>
    ''' <param name="sopName">The SOPName to pass to the underlying function.</param>
    ''' <returns>A single List(Of ClsCustomer) containing unique customers.</returns>
    Public Shared Function GetUniqueCustomersForConditions(conditionIds As List(Of Guid), sopName As String) As List(Of ClsCustomer)
        ' This is the most efficient way to prevent duplicates.
        Dim uniqueCustomers As New Dictionary(Of String, ClsCustomer)()

        For Each id As Guid In conditionIds
            ' Call your existing function to get the JSON string for the current condition.
            Dim jsonResult As String = GetMatchingCustomers(id, sopName)

            ' 3. Deserialize the JSON string back into a list of customer objects.
            Dim customersForThisCondition As List(Of ClsCustomer) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of ClsCustomer))(jsonResult)

            ' 4. Add only the new customers to our dictionary.
            For Each customer As ClsCustomer In customersForThisCondition
                ' The ContainsKey check is very fast and ensures we don't add duplicates.
                If Not uniqueCustomers.ContainsKey(customer.C_CUID) Then
                    uniqueCustomers.Add(customer.C_CUID, customer)
                End If
            Next
        Next

        ' 5. Convert the Dictionary's values back into a simple list and return it.
        Return uniqueCustomers.Values.ToList()
    End Function


    Public Shared Function GetMatchingCustomers(conditionId As Guid, sopName As String) As String
        Dim customerList As New List(Of ClsCustomer)()
        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString
        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand("dbo.PS_CompanyCondition", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ConditionID", conditionId)
                cmd.Parameters.AddWithValue("@SOPName", sopName)
                conn.Open()
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        customerList.Add(New ClsCustomer With {
                        .C_CUID = reader("C_CUID").ToString(),
                        .C_NAME = reader("C_NAME").ToString()
                    })
                    End While
                End Using
            End Using
        End Using
        Return Newtonsoft.Json.JsonConvert.SerializeObject(customerList)
    End Function

    Public Shared Function DeleteCondition(ByVal conditionId As Guid) As Boolean
        Try
            Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString
            Using conn As New SqlConnection(connString)
                Using cmd As New SqlCommand("dbo.PD_Condition", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@ConditionID", conditionId)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Processes a DataTable of conditions to extract all unique customers.
    ''' </summary>
    ''' <param name="conditionsTable">The DataTable containing condition rows.</param>
    ''' <returns>A List of unique ClsCustomer objects.</returns>
    Public Shared Function ProcessConditionsAndGetCustomers(ByVal conditionsTable As DataTable) As List(Of ClsCustomer)
        ' 1. Guard clause: If the table is null or empty, return an empty list immediately.
        If conditionsTable Is Nothing OrElse conditionsTable.Rows.Count = 0 Then
            Return New List(Of ClsCustomer)()
        End If

        ' 2. Extract all ConditionIDs from the table using LINQ.
        '    This is a more concise way than looping through each row.
        '    (Note: This requires a reference to System.Data.DataSetExtensions.dll)
        Dim conditionIds = conditionsTable.AsEnumerable() _
                                        .Select(Function(row) row.Field(Of Guid)("ConditionID")) _
                                        .ToList()

        ' 3. Get the SOPName from the first row.
        Dim sopName As String = conditionsTable.Rows(0)("SOPName").ToString()

        ' 4. Call the existing method to fetch the customers and return the result.
        Return GetUniqueCustomersForConditions(conditionIds, sopName)
    End Function
End Class
