
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services
Imports Newtonsoft.Json
Imports Telerik.Web.UI

Partial Class InDirectAssignment
    Inherits System.Web.UI.Page

    Public Class ClsCustomer
        Public Property C_CUID As String
        Public Property C_NAME As String
    End Class

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then Return

        PopulateCriteriaEditorDropdowns()

        Dim queryBuilderToken As String = Request.QueryString("QueryBuilderToken")
        Dim token As TokenData = Nothing
        If Not ClsTokenHelper.TryParseToken(queryBuilderToken, token) Then
            ShowError("Error: Invalid or missing security token.")
            Return
        End If
        ' ---Use the decrypted data from the token
        Dim isStatic As Boolean = token.IsStatic.GetValueOrDefault(False)
        Dim documentIds As String = token.DocumentIds
        Dim country As String = token.SopId
        hdnSopId.Value = country

        Dim environment As String = token.EnvironmentId
        Dim conditionId As Guid? = token.ConditionID
        If conditionId.HasValue Then
            hdnConditionID.Value = conditionId.Value.ToString()
        Else
            hdnConditionID.Value = String.Empty
        End If

        Dim myDocumentId As Guid

        If token.Mode = "creation" Then
            Try
                ' Determine the parameters based on the IsStatic flag
                Dim conditionNameForCreation As String = ""
                Dim isStaticForCreation As Boolean = False
                If Not isStatic Then
                    isStaticForCreation = False
                    conditionNameForCreation = "InDirect Assignment "
                End If


                Dim newConditionId As Guid
                Dim dynamicConditionHelper As New DynamicConditionsHelper()
                If token.PageSource = "FilesManager" Then
                    If Not documentIds.Contains(";") Then
                        myDocumentId = New Guid(documentIds)
                        Dim fileName As String = dynamicConditionHelper.GetDocumentNameByID(myDocumentId)
                        conditionNameForCreation += fileName
                    End If
                    newConditionId = dynamicConditionHelper.CreateConditionFromDocumentList(documentIds, conditionNameForCreation, isStaticForCreation, country)
                ElseIf token.PageSource = "FocusRange" Then
                    Dim focusRangeName As String = dynamicConditionHelper.GetFocusRangeNameByID(New Guid(token.FocusRangeId))
                    conditionNameForCreation += focusRangeName
                    newConditionId = dynamicConditionHelper.CreateCondition(conditionNameForCreation, isStatic, country, Nothing, Nothing, New Guid(token.FocusRangeId))
                End If

                Dim displayTokenData As New TokenData(environment, country, isStatic, "edit", token.PageSource, documentIds, "", newConditionId)
                Dim displayToken As String = ClsTokenHelper.GenerateToken(displayTokenData)
                Response.Redirect("InDirectAssignment.aspx?QueryBuilderToken=" & displayToken)
                Return
            Catch ex As Exception
                ShowError("Error creating condition: " & ex.Message)
            End Try
        End If

        If Not isStatic And token.Mode <> "creation" Then
            BindCriteria(token.ConditionID)
            Dim script As String = "jQuery(function() { refreshPreview('" & conditionId.ToString() & "'); });"
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "autoPreviewScript", script, True)
        End If
    End Sub



    Private Sub BindCriteria(ByVal conditionId As Guid)
        Dim dynamicConditionHelper As New DynamicConditionsHelper()

        ' Get the condition details (like its name) to display as a title
        Dim mainCondition As Condition = dynamicConditionHelper.GetConditionByID(conditionId)
        If mainCondition IsNot Nothing Then
            litConditionName.Text = mainCondition.ConditionName
        End If

        ' Get the list of criteria for this condition
        Dim criteriaList As List(Of Criterion) = GetCriteriaForCondition(conditionId)

        ' Build the table rows
        For Each crit As Criterion In criteriaList
            Dim row As New TableRow()

            ' Cell 1: Actions
            Dim cellActions As New TableCell()
            cellActions.Text = "<img src='Images/Edit.png' class='btn-edit-criterion' data-conditionid='" & crit.ConditionID.ToString() & "' data-criteriaid='" & crit.CriteriaNumber.ToString() & "' title='Edit Criterion' style='cursor:pointer; margin-right: 5px;' />" &
                               "<img src='Images/Delete.png' class='btn-delete-criterion' data-conditionid='" & crit.ConditionID.ToString() & "' data-criteriaid='" & crit.CriteriaNumber.ToString() & "' title='Delete Criterion' style='cursor:pointer;' />"
            row.Cells.Add(cellActions)

            ' Cell 2: Field
            Dim cellField As New TableCell()
            cellField.Text = crit.ConditionField
            row.Cells.Add(cellField)

            ' Cell 3: Operator
            Dim cellOp As New TableCell()
            cellOp.Text = crit.ConditionOperator
            row.Cells.Add(cellOp)

            ' Cell 4: Value
            Dim cellVal As New TableCell()
            cellVal.Text = crit.ConditionValues
            row.Cells.Add(cellVal)

            CriteriaTable.Rows.Add(row)
        Next
    End Sub

    Private Function GetConditionsData(ByVal documentId As Guid) As DataTable
        Dim dt As New DataTable()
        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString

        Dim idTable As New DataTable("DocumentIDs")
        idTable.Columns.Add("GuidId", GetType(Guid))

        idTable.Rows.Add(documentId)

        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand("dbo.PS_ConditionsByDocuments", conn)
                cmd.CommandType = CommandType.StoredProcedure

                Dim param As SqlParameter = cmd.Parameters.Add("@DocumentIDs", SqlDbType.Structured)
                param.Value = idTable
                param.TypeName = "dbo.GuidIds"

                Dim adapter As New SqlDataAdapter(cmd)
                adapter.Fill(dt)
            End Using
        End Using
        Return dt
    End Function

    Private Sub ShowError(ByVal message As String)
        lblMessage.Text = message
        lblMessage.Visible = True
    End Sub

    Private Sub PopulateCriteriaEditorDropdowns()
        cmbField.Items.Clear()
        Dim fieldMappings As New Dictionary(Of String, String) From {
        {"C_GRP4", "Customer Group 4"},
        {"C_DESCRIPTION", "Description"},
        {"CH_CUID_3", "Hierarchy 3"},
        {"CH_CUID_4", "Hierarchy 4"},
        {"CH_CUID_5", "Hierarchy 5"},
        {"C_PLANLEVELCUSTOMERS", "Plan Level"},
        {"C_PRICELIST", "Price List"},
        {"C_SALESCHANNEL", "Sales Channel"},
        {"C_SUBSALESCHANEL", "Sub Sales Channel"},
        {"C_SALESREPCODE", "Sales Rep Code"}
    }

        ' Extract the keys (technical field names) and sort them for display order.
        Dim companyFields As String() = fieldMappings.Keys.ToArray()
        Array.Sort(companyFields)

        ' Populate the RadComboBox.
        For Each fieldKey As String In companyFields
            ' The Text property will be the friendly value, and the Value property will be the technical field name.
            cmbField.Items.Add(New Telerik.Web.UI.RadComboBoxItem(fieldMappings(fieldKey), fieldKey))
        Next

        ' --- Populate Operators DropDown ---
        ddlOperator.Items.Clear()
        ddlOperator.Items.Add(New ListItem("is (=)", "="))
        ddlOperator.Items.Add(New ListItem("is not (!=)", "<>"))
        ddlOperator.Items.Add(New ListItem("contains (LIKE)", "LIKE"))
        ddlOperator.Items.Add(New ListItem("does not contain (NOT LIKE)", "NOT LIKE"))
        ddlOperator.Items.Add(New ListItem("in list (IN)", "IN"))
        ddlOperator.Items.Add(New ListItem("not in list (NOT IN)", "NOT IN"))
    End Sub

    <WebMethod()>
    Public Shared Function GetCriteriaForCondition(ByVal conditionId As Guid) As List(Of Criterion)
        Dim criteriaList As New List(Of Criterion)()
        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString
        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand("dbo.PS_Condition_Criteria", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ConditionID", conditionId)
                conn.Open()
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim technicalField As String = reader("ConditionField").ToString()
                        Dim friendlyField As String = GetFriendlyFieldName(technicalField)

                        criteriaList.Add(New Criterion With {
                            .ConditionID = CType(reader("ConditionID"), Guid),
                            .CriteriaNumber = CType(reader("CriteriaNumber"), Byte),
                            .ConditionField = friendlyField,
                            .ConditionOperator = reader("ConditionOperator").ToString(),
                            .ConditionValues = reader("ConditionValues").ToString()
                        })
                    End While
                End Using
            End Using
        End Using
        Return criteriaList
    End Function

    <WebMethod()>
    Public Shared Function DeleteCriterion(ByVal conditionId As Guid, ByVal criteriaNumber As Byte) As Boolean
        Try
            Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString
            Using conn As New SqlConnection(connString)
                Using cmd As New SqlCommand("dbo.PD_ConditionCriteria", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@ConditionID", conditionId)
                    cmd.Parameters.AddWithValue("@CriteriaNumberToDelete", criteriaNumber)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    <WebMethod()>
    Public Shared Function GetMatchingCustomers(conditionId As Guid, sopName As String) As String
        Return DynamicConditionsHelper.GetMatchingCustomers(conditionId, sopName)
    End Function

    Protected Sub BtnSaveCriterion_Click(sender As Object, e As EventArgs)

    End Sub

    Protected Sub BtnCancelCriterion_Click(sender As Object, e As EventArgs)

    End Sub


    <WebMethod()>
    Public Shared Function UpdateCondition(ByVal conditionId As Guid, ByVal conditionName As String, ByVal isStatic As Boolean) As Condition
        Dim updatedBy As String = "CurrentUser"

        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString
        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand("dbo.PU_CONDITION", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ConditionID", conditionId)
                cmd.Parameters.AddWithValue("@ConditionName", conditionName)
                cmd.Parameters.AddWithValue("@ConditionUpdatedBy", updatedBy)
                cmd.Parameters.AddWithValue("@ConditionIsStatic", isStatic)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        ' After updating, return the fresh data to update the table row
        Dim dynamicConditionHelper As New DynamicConditionsHelper()
        Return dynamicConditionHelper.GetConditionByID(conditionId)
    End Function


    <WebMethod()>
    Public Shared Function GetCriterionForEdit(ByVal conditionId As Guid, ByVal criteriaNumber As Byte) As Criterion
        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString
        Using conn As New SqlConnection(connString)
            Dim sql As String = "SELECT * FROM T_CONDITION_CRITERIA WHERE ConditionID = @ConditionID AND CriteriaNumber = @CriteriaNumber"
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@ConditionID", conditionId)
                cmd.Parameters.AddWithValue("@CriteriaNumber", criteriaNumber)
                conn.Open()
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        Return New Criterion With {
                            .ConditionID = CType(reader("ConditionID"), Guid),
                            .CriteriaNumber = CType(reader("CriteriaNumber"), Byte),
                            .ConditionField = reader("ConditionField").ToString(),
                            .ConditionOperator = reader("ConditionOperator").ToString(),
                            .ConditionValues = reader("ConditionValues").ToString()
                        }
                    End If
                End Using
            End Using
        End Using
        Return Nothing
    End Function

    <WebMethod()>
    Public Shared Function AddCriterion(ByVal conditionId As Guid, ByVal field As String, ByVal op As String, ByVal value As String) As Boolean
        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString
        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand("dbo.PI_CONDITION_CRITERIA", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ConditionID", conditionId)
                cmd.Parameters.AddWithValue("@ConditionField", field)
                cmd.Parameters.AddWithValue("@ConditionOperator", op)
                cmd.Parameters.AddWithValue("@ConditionValues", value)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
        'Dim page As New QueryBuilder()
        'page.GetConditionByID(conditionId)

        Return True
    End Function

    <WebMethod()>
    Public Shared Function UpdateCriterion(ByVal conditionId As Guid, ByVal criteriaNumber As Byte, ByVal field As String, ByVal op As String, ByVal value As String) As Boolean
        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString
        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand("dbo.PU_CONDITION_CRITERIA", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ConditionID", conditionId)
                cmd.Parameters.AddWithValue("@CriteriaNumber", criteriaNumber)
                cmd.Parameters.AddWithValue("@ConditionField", field)
                cmd.Parameters.AddWithValue("@ConditionOperator", op)
                cmd.Parameters.AddWithValue("@ConditionValues", value)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
        Return True
    End Function

    <WebMethod()>
    Public Shared Function SaveChanges(conditionId As String, ByVal customerCodes As String, lastCriteriaNumber As String) As String
        Try

            Dim condId As Guid = New Guid(conditionId)
            DeleteCriterion(condId, lastCriteriaNumber)
            Dim result As Boolean = AddCriterion(condId, "C_CUID", "IN", customerCodes)
            If result = True Then
                Return "Success"
            End If
            Return "Failed"
        Catch ex As Exception
            Return "Error: " & ex.Message
        End Try
    End Function

    Private Shared ReadOnly fieldMappings As New Dictionary(Of String, String) From {
    {"C_GRP4", "Customer Group 4"},
    {"C_DESCRIPTION", "Description"},
    {"CH_CUID_3", "Hierarchy 3"},
    {"CH_CUID_4", "Hierarchy 4"},
    {"CH_CUID_5", "Hierarchy 5"},
    {"C_PLANLEVELCUSTOMERS", "Plan Level"},
    {"C_PRICELIST", "Price List"},
    {"C_SALESCHANNEL", "Sales Channel"},
    {"C_SUBSALESCHANEL", "Sub Sales Channel"},
    {"C_SALESREPCODE", "Sales Rep Code"}
}

    Private Shared Function GetFriendlyFieldName(ByVal technicalFieldName As String) As String
        If fieldMappings.ContainsKey(technicalFieldName) Then
            Return fieldMappings(technicalFieldName)
        Else
            ' Return the technical name if no mapping is found (fallback)
            Return technicalFieldName
        End If
    End Function
End Class
