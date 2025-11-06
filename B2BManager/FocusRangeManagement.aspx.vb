Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.IO
Imports System.Web.Services
Imports OfficeOpenXml
Imports Telerik.Web.UI

Partial Class FocusRangeManagement
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim EnvironmentID As Integer = 0
            Dim sopid As String = String.Empty
            Dim id As Guid = Guid.Empty
            Dim title As String = String.Empty
            Dim startDate As String = String.Empty
            Dim endDate As String = String.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                Guid.TryParse(Request.QueryString("id"), id)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("title")) Then
                title = Request.QueryString("title").ToString()
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("startDate")) Then
                Dim dateAndTime As DateTime
                If DateTime.TryParse(Request.QueryString("startDate"), dateAndTime) Then
                    startDate = dateAndTime.ToString("yyyy-MM-dd")
                End If
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("endDate")) Then
                Dim dateAndTime As DateTime
                If DateTime.TryParse(Request.QueryString("endDate"), dateAndTime) Then
                    endDate = dateAndTime.ToString("yyyy-MM-dd")
                End If
            End If


            If Not String.IsNullOrEmpty(Request.QueryString("sopid")) Then
                sopid = Request.QueryString("sopid").ToString()
            End If


            If EnvironmentID > 0 Then
                If id <> Guid.Empty Then
                    CType(Master.FindControl("title"), HtmlTitle).Text = "View Core Range"

                    trFileUpload.Visible = False
                    FileUpload1.Enabled = False

                    btnSubmit.Visible = False
                    btnSubmit.Text = "Update"
                    btnSubmit.OnClientClick = "ProcessButton('Update')"
                    txtFocusRangeName.Enabled = False
                    txtFocusRangeName.Text = title
                    txtStartDate.Value = startDate
                    txtEndDate.Value = endDate
                    BindGrid(EnvironmentID, id)
                Else
                    CType(Master.FindControl("title"), HtmlTitle).Text = "New Core Range"
                    btnSubmit.Text = "Save"
                    btnSubmit.OnClientClick = "ProcessButton('Add')"
                End If

            End If

            If Not Request.QueryString("id") = Nothing Then
                Dim urlBase = String.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority)
                Dim newConditionTokenData As New TokenData(EnvironmentID, sopid, False, "creation", "FocusRange", "", id.ToString, Nothing)
                Dim token = ClsTokenHelper.GenerateToken(newConditionTokenData)
                AddConditionBtn.OnClientClick = "window.parent.openInDirectAssignmentPopup('" & urlBase & ResolveUrl("~/InDirectAssignment.aspx") &
                                "?QueryBuilderToken=" & token & "'); return false;"

                Dim newStaticConditionTokenData As New TokenData(EnvironmentID, sopid, True, "creation", "FocusRange", Nothing, id.ToString, Nothing)
                Dim staticToken = ClsTokenHelper.GenerateToken(newStaticConditionTokenData)

                AddStaticConditionBtn.OnClientClick = "window.parent.openStaticAssignmentPopup('" & urlBase & ResolveUrl("~/QueryBuilderStaticAssignment.aspx") & "?QueryBuilderToken=" & staticToken & "'); return false;"
            Else
                'Case of creation new Core Range
                AddConditionBtn.Visible = False
                AddStaticConditionBtn.Visible = False
                AssignedCustomersNumberLb.Visible = False
                lblNumberAssign.Visible = False
            End If
        End If
    End Sub

    Private Sub BindGrid(environmentID As Integer, id As Guid)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", environmentID))
        parameters.Add(New SqlParameter("@FocusRangeID", id))
        Dim data As DataTable = ClsDataAccessHelper.FillDataTable("[dbo].[FocusRange_GetProducts]", parameters)

        If data IsNot Nothing AndAlso data.Rows.Count > 0 Then
            FocusRangeDataGrid.DataSource = data
            FocusRangeDataGrid.DataBind()
            lblProductsCount.Text = data.Rows.Count.ToString()
        End If
    End Sub

    Protected Sub GridViewProdListDynamic_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles FocusRangeDataGrid.PageIndexChanging
        FocusRangeDataGrid.PageIndex = e.NewPageIndex
        Dim EnvironmentID As Integer = 0
        Dim id As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If
        If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
            Guid.TryParse(Request.QueryString("id"), id)
        End If
        BindGrid(EnvironmentID, id)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        lblErrorInfo.Text = " "
        Dim EnvironmentID As Integer = 0
        Dim id As Guid = Guid.Empty
        Dim sopid As String = String.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If
        If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
            Guid.TryParse(Request.QueryString("id"), id)
        End If
        If Not String.IsNullOrEmpty(Request.QueryString("sopid")) Then
            sopid = Request.QueryString("sopid").ToString()
        End If

        If String.IsNullOrEmpty(txtFocusRangeName.Text) Then
            lblErrorInfo.Text = "The Title is manadatory"
            Return
        End If


        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        If id <> Guid.Empty Then
            parameters.Add(New SqlParameter("@ID", id))
        End If
        parameters.Add(New SqlParameter("@SOPID", sopid))
        parameters.Add(New SqlParameter("@SOP_Product_Codes", hdProducts.Value))
        parameters.Add(New SqlParameter("@FocusRangeTitle", txtFocusRangeName.Text))
        parameters.Add(New SqlParameter("@StartDate", txtStartDate.Value))
        parameters.Add(New SqlParameter("@EndDate", txtEndDate.Value))
        parameters.Add(New SqlParameter("@AuthorEmail", ClsSessionHelper.LogonUser.Email))
        parameters.Add(New SqlParameter("@AuthorID", ClsSessionHelper.LogonUser.ID.ToString()))
        ClsDataAccessHelper.ExecuteNonQuery("FocusRange_AddNewRecord", parameters)

        hdProducts.Value = String.Empty
        txtFocusRangeName.Text = String.Empty
        lblProductsCount.Text = String.Empty

        ScriptManager.RegisterStartupScript(tbl1, tbl1.GetType(), "Confirm", "window.parent.Finish(" + IIf(id <> Guid.Empty, "'Update'", "'Save'") + ");", True)

        'Select Case result
        '    Case "Success"
        '        ClsHelper.Log(IIf(id <> Guid.Empty, "Update", "Save") + " SAP Order Type", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
        '        ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Confirm", "window.parent.Finish(" + IIf(id <> Guid.Empty, "'Update'", "'Save'") + ");", True)
        '    Case Nothing
        '        ClsHelper.Log(IIf(id <> Guid.Empty, "Update", "Save") + " TSAP Order Type", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "An unexpected error has occurred")
        '        lblErrorInfo.Text = "An unexpected error has occurred. Please try again later."
        '    Case Else
        '        ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Confirm", "window.parent.ConfirmExistingMapping('" + result + "');", True)
        'End Select
    End Sub

    Public Sub btnUploadNewRange_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUploadNewRange.Click
        hdProducts.Value = String.Empty
        GetUploadedProductsList(FileUpload1)
        'Dim result As Boolean
        'If (Not String.IsNullOrEmpty(SOP_Product_Codes)) Then
        '    Dim clsUser As ClsUserB2B = SessionHelper.LogonUser
        '    result = FocusRangeBLL.UploadRange(
        '                                    clsUser.COMPANY_ID,
        '                                    SOP_Product_Codes,
        '                                    WebConfigHelper.ProjectID(),
        '                                    clsUser.U_ID,
        '                                    True)
        'End If

        'If (result) Then
        '    BindGrid()
        'End If
    End Sub
    Public Sub GetUploadedProductsList(fileUpload As FileUpload)
        Dim SOP_Product_Codes As String = String.Empty
        Dim count As Integer = 0
        If fileUpload.HasFile Then
            Try
                Dim extension As String = Path.GetExtension(fileUpload.FileName).ToUpper()
                If (extension = ".CSV") Then
                    Dim ostream As Stream = fileUpload.FileContent
                    If ostream.CanRead Then
                        Dim oreader As StreamReader = New StreamReader(ostream)
                        Dim data As String = oreader.ReadLine()
                        If data.ToUpper Like "*PRODUCT*" Then data = oreader.ReadLine() '//skip the header
                        Dim build As StringBuilder = New StringBuilder()
                        While Not data Is Nothing
                            If (data.Contains(",")) Then
                                data = Split(data, ",")(0).ToString
                            ElseIf (data.Contains(";")) Then
                                data = Split(data, ";")(0).ToString
                            End If
                            If (data IsNot Nothing AndAlso Not build.ToString().Contains(data)) Then
                                build.Append(data.Trim() + ";")
                                count = count + 1
                            End If
                            data = oreader.ReadLine()
                        End While
                        SOP_Product_Codes = build.ToString()
                    End If
                ElseIf (extension = ".XLSX") Then
                    Using Excel = New ExcelPackage(fileUpload.PostedFile.InputStream)
                        If (Excel.Workbook.Worksheets.Count() > 0) Then
                            Dim worksheet = Excel.Workbook.Worksheets.Item(1)
                            Dim totalRows As Integer = worksheet.Dimension.End.Row
                            Dim build As StringBuilder = New StringBuilder()
                            For i As Integer = 1 To totalRows
                                Dim product As String = worksheet.Cells(i, 1).Text.ToString()
                                If (product IsNot Nothing AndAlso Not build.ToString().Contains(product)) Then
                                    If (product.ToUpper Like "*PRODUCT*") Then
                                        Continue For
                                    End If
                                    build.Append(product.Trim() + ";")
                                    count = count + 1
                                End If
                            Next

                            SOP_Product_Codes = build.ToString()
                        Else
                            lblErrorInfo.Text = "NoDataSheetFound"
                        End If

                    End Using
                End If
            Catch ex As Exception
                lblErrorInfo.Text = "There was an error"
            End Try
        End If
        hdProducts.Value = SOP_Product_Codes
        lblProductsCount.Text = count.ToString()
    End Sub

    Protected Sub ConditionRg_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs)
        ' Now handled by ConditionGrid user control.
    End Sub

    Protected Sub ConditionRg_NeedDataSource(source As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs)
        ' Now handled by ConditionGrid user control.
    End Sub

    <WebMethod()>
    Public Shared Function DeleteCondition(ByVal conditionId As Guid) As Boolean
        Return DynamicConditionsHelper.DeleteCondition(conditionId)
    End Function

    <WebMethod()>
    Public Shared Function GetPreviewCustomers(ByVal id As String, pageSource As String) As List(Of ClsCustomer)
        Dim conditionsTable As DataTable = Nothing

        If pageSource = "FilesManager" Then
            ' A single LINQ expression to safely parse all GUIDs from the string.
            Dim idsToSearch = id.Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries) _
                                .Select(Function(idStr)
                                            Dim parsedGuid As Guid
                                            Guid.TryParse(idStr.Trim(), parsedGuid)
                                            Return parsedGuid
                                        End Function) _
                                .Where(Function(g) g <> Guid.Empty) _
                                .ToArray()

            ' Proceed only if we have valid GUIDs.
            If idsToSearch.Any() Then
                Dim result As ClsDocument = DynamicConditionsHelper.GetConditionsForDocuments(idsToSearch)
                If result IsNot Nothing Then
                    conditionsTable = result.Conditions
                End If
            End If

        ElseIf pageSource = "FocusRange" Then
            ' Safely parse the single GUID.
            Dim focusRangeId As Guid
            If Guid.TryParse(id, focusRangeId) Then
                Dim resultFocusRange As ClsFocusRange = DynamicConditionsHelper.GetConditionsByFocusRange(focusRangeId)
                If resultFocusRange IsNot Nothing Then
                    conditionsTable = resultFocusRange.Conditions
                End If
            End If
        End If

        ' The common logic is now a single, clean call to our new helper function.
        Return DynamicConditionsHelper.ProcessConditionsAndGetCustomers(conditionsTable)
    End Function
End Class
