
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services

Partial Class QueryBuilderStaticAssignment
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim queryBuilderToken As String = Request.QueryString("QueryBuilderToken")
        Dim token As TokenData = Nothing

        If Not ClsTokenHelper.TryParseToken(queryBuilderToken, token) Then
            ShowError("Error: Invalid or missing security token.")
            pnlStaticAssignment.Visible = False
            Return
        End If

        Dim isStatic As Boolean = token.IsStatic.GetValueOrDefault(False)
        Dim documentIds As String = token.DocumentIds
        Dim country As String = token.SopId
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
                If isStatic Then
                    isStaticForCreation = True
                    conditionNameForCreation = "Direct Assignment "
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
                Response.Redirect("QueryBuilderStaticAssignment.aspx?QueryBuilderToken=" & displayToken)
            Catch ex As Exception
                ShowError("Error creating condition: " & ex.Message)
            End Try
        End If

        If isStatic Then
            ' Show the manual, static assignment interface
            pnlStaticAssignment.Visible = True
            BindStaticAssignmentControls(token.ConditionID, country)
        End If

    End Sub


    ' Populate the static assignment controls
    Private Sub BindStaticAssignmentControls(ByVal conditionId As Guid, ByVal country As String)
        Dim dynamicConditionHelper As New DynamicConditionsHelper()

        Dim assignedCompaniesTable As DataTable
        Dim unassignedCompaniesTable As DataTable
        Dim allCompanies As DataTable = GetAllCompanies(country)
        Dim criteriaList As List(Of Criterion) = dynamicConditionHelper.GetCriteriaForCondition(conditionId)
        Dim lastCriteriaNumber As Integer = 0

        ' Check if any criteria actually exist for this condition.
        If criteriaList.Any() Then
            assignedCompaniesTable = FilterDataTableByCustomerCodes(allCompanies, criteriaList(0).ConditionValues)
            unassignedCompaniesTable = GetUnassignedCompanies(allCompanies, assignedCompaniesTable)
            lastCriteriaNumber = criteriaList.Max(Function(c) c.CriteriaNumber)
        Else
            ' If no criteria exist (e.g., in Creation mode), then nothing is assigned yet.
            ' The "Assigned" list is empty.
            assignedCompaniesTable = allCompanies.Clone()
            ' The "Unassigned" (Available) list contains ALL companies.
            unassignedCompaniesTable = allCompanies
        End If

        btnSaveChanges.Attributes("data-last-criteria-number") = lastCriteriaNumber.ToString()

        ListView1.DataSource = assignedCompaniesTable
        ListView1.DataBind()

        ListView2.DataSource = unassignedCompaniesTable
        ListView2.DataBind()
    End Sub



    Private Function GetAllCompanies(ByVal sopId As String) As DataTable
        Dim dt As New DataTable()
        Dim connString As String = ConfigurationManager.ConnectionStrings("ConnexionB2B").ConnectionString

        ' Your query with a parameter for the SOP ID
        Dim sql As String = "SELECT c.C_GLOBALID as ID, c.C_NAME as Name, " &
                        "cast(c.C_DESCRIPTION as varchar(30)) as [Description], c.C_CUID as CustomerCode, " &
                        "0 as AssociatedToUser, v.CH_CUID_5, v.CH_NAME_5, v.CH_CUID_4, v.CH_NAME_4, " &
                        "v.CH_CUID_3, v.CH_NAME_3, PLC.PLC, PLC.PLC_description, ISNULL(c.C_GRP4,'') as C_GRP4 " &
                        "FROM T_COMPANY AS c " &
                        "INNER JOIN T_CY_SOP AS cs ON c.ID_CY_SOP = cs.ID_CY_SOP " &
                        "INNER JOIN T_SOP AS s ON cs.S_GLOBALID = s.S_GLOBALID " &
                        "LEFT OUTER JOIN V_COMPANY_HIERARCHY as v ON c.C_GLOBALID = V.C_GLOBALID " &
                        "LEFT OUTER JOIN T_REF_PLC AS PLC WITH (NOLOCK) ON c.C_PLANLEVELCUSTOMERS = PLC.PLC " &
                        "WHERE c.c_isactive = 1 AND s.S_SOP_ID = @SOP_ID;"

        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@SOP_ID", sopId)
                Using adapter As New SqlDataAdapter(cmd)
                    adapter.Fill(dt)
                End Using
            End Using
        End Using

        Return dt
    End Function

    ''' <summary>
    ''' Filters a DataTable to include only rows where the CustomerCode matches a value in a comma-separated string.
    ''' </summary>
    ''' <param name="sourceTable">The master DataTable containing all companies.</param>
    ''' <param name="customerCodesString">A string of CustomerCodes separated by commas (e.g., "123,456,789").</param>
    ''' <returns>A new DataTable containing only the filtered rows.</returns>
    Private Function FilterDataTableByCustomerCodes(ByVal sourceTable As DataTable, ByVal customerCodesString As String) As DataTable
        Dim codesToFind As New HashSet(Of String)(customerCodesString.Split(","c))

        Dim filteredRows = From row In sourceTable.AsEnumerable()
                           Where codesToFind.Contains(row.Field(Of String)("CustomerCode"))
                           Select row

        If filteredRows.Any() Then
            Return filteredRows.CopyToDataTable()
        Else
            Return sourceTable.Clone()
        End If
    End Function

    ''' <summary>
    ''' Compares a master DataTable against a sub-list and returns the rows that are NOT in the sub-list.
    ''' </summary>
    ''' <param name="allCompanies">The master DataTable containing all companies.</param>
    ''' <param name="assignedCompanies">A DataTable containing the subset of assigned companies.</param>
    ''' <returns>A new DataTable containing only the unassigned companies.</returns>
    Private Function GetUnassignedCompanies(ByVal allCompanies As DataTable, ByVal assignedCompanies As DataTable) As DataTable
        Dim assignedCodes As New HashSet(Of String)(
        assignedCompanies.AsEnumerable().Select(Function(row) row.Field(Of String)("CustomerCode"))
    )

        Dim unassignedRows = From row In allCompanies.AsEnumerable()
                             Where Not assignedCodes.Contains(row.Field(Of String)("CustomerCode"))
                             Select row

        If unassignedRows.Any() Then
            Return unassignedRows.CopyToDataTable()
        Else
            Return allCompanies.Clone()
        End If
    End Function
    Private Sub ShowError(ByVal message As String)
        'lblMessage.Text = message
        'lblMessage.Visible = True
    End Sub

End Class
