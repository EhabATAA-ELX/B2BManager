
Imports System.Data
Imports System.Diagnostics
Imports OfficeOpenXml
Imports Telerik.Web.UI

Partial Class ExportBuilder
    Inherits System.Web.UI.Page
    Protected Sub chkBoxUseTemplate_CheckedChanged(sender As Object, e As EventArgs)
        If Not chkBoxUseTemplate.Checked Then
            trTemplateName.Visible = True
            trExistingTemplates.Visible = False
            txtBoxTemplateName.Text = ""
            txtBoxTemplateName.Enabled = True
            ddlTemplateType.Enabled = True
            ddlTemplateType.SelectedIndex = 0
            RenderSingleTemplate(Nothing)
        Else
            trExistingTemplates.Visible = True
            RenderSingleTemplate(ddlExistingTemplates.SelectedValue)
        End If
    End Sub

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            Dim watch As Stopwatch = Stopwatch.StartNew()
            ManageFields(True)
            RenderControls()
            watch.Stop()
            ClsHelper.Log("Export Builder Access", ClsSessionHelper.LogonUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, False, Nothing)
        Else
            lblInfoMessage.Text = " "
        End If
    End Sub

    Private Sub RenderControls(Optional ByVal selectedValue As String = Nothing)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        If ClsSessionHelper.LogonUser Is Nothing Then
            Return
        End If
        txtBoxExportName.Text = ""
        txtBoxTemplateName.Text = ""
        lblInfoMessage.Text = " "
        Dim selectedApplication As ClsHelper.Application = ClsSessionHelper.LogonUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True)
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, ddlCountry.SelectedValue, True, selectedApplication.SelectAllCountriesByDefault)
        RenderTemplatesDropDown()
        RenderSingleTemplate(Nothing)
    End Sub

    Private Sub RenderFields(Fields As List(Of ClsHelper.Field), Optional disabled As Boolean = False)
        tvFields.Nodes.Clear()
        If Fields.Count > 0 Then
            For Each field As ClsHelper.Field In Fields.Where(Function(Fc) Fc.ParentID Is Nothing)
                Dim parentNode As RadTreeNode = New RadTreeNode(field.Name, field.ID.ToString())
                parentNode.ImageUrl = field.imageUrl
                Dim children As List(Of ClsHelper.Field) = Fields.Where(Function(Fc) Fc.ParentID IsNot Nothing And Fc.ParentID = field.ID).ToList()
                If children.Count > 0 Then
                    For Each childField As ClsHelper.Field In children
                        Dim child As RadTreeNode = New RadTreeNode(childField.Name, childField.ID.ToString())
                        child.ImageUrl = childField.imageUrl
                        child.Checked = childField.Checked
                        child.Enabled = Not disabled
                        parentNode.Nodes.Add(child)
                    Next
                End If
                parentNode.Enabled = Not disabled
                parentNode.Expanded = disabled
                If children.Count > 0 Then
                    tvFields.Nodes.Add(parentNode)
                End If
            Next
        End If
    End Sub

    Private Sub RenderTemplatesDropDown()
        Dim templates As List(Of ClsHelper.Template) = ClsHelper.GetExportTemplates(ClsSessionHelper.LogonUser.GlobalID)
        ddlExistingTemplates.Items.Clear()
        trExistingTemplates.Visible = False
        If templates.Count > 0 Then
            For Each template As ClsHelper.Template In templates
                If template.Checked Then
                    Dim item As RadComboBoxItem = New RadComboBoxItem(template.Name, template.ID)
                    item.ImageUrl = template.ImageUrl
                    ddlExistingTemplates.Items.Add(item)
                End If
            Next
            chkBoxUseTemplate.Enabled = True
        Else
            chkBoxUseTemplate.Enabled = False
        End If
        chkBoxUseTemplate.Checked = False
    End Sub
    Protected Sub ddlDataSource_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim templateInfo As ClsHelper.TemplateInfo = ClsHelper.GetExportData(ClsSessionHelper.LogonUser.GlobalID, Nothing, ddlDataSource.SelectedValue)
        If templateInfo IsNot Nothing Then
            RenderFields(templateInfo.Fields)
        Else
            tvFields.Nodes.Clear()
        End If
    End Sub

    Private Sub RenderSingleTemplate(templateID As Integer?)
        Dim templateInfo As ClsHelper.TemplateInfo = ClsHelper.GetExportData(ClsSessionHelper.LogonUser.GlobalID, templateID)
        If templateInfo IsNot Nothing Then
            ClsHelper.RenderDropDownList(ddlDataSource, templateInfo.DataSources, True)
        End If
        Dim disableTvFields As Boolean = False
        If templateInfo.TemplateID IsNot Nothing Then
            ddlDataSource.SelectedValue = templateInfo.SelectedDataSourceID
            txtBoxTemplateName.Text = templateInfo.TemplateName
            txtBoxTemplateName.Enabled = templateInfo.Enabled
            trTemplateName.Visible = templateInfo.Enabled
            ddlTemplateType.SelectedValue = IIf(templateInfo.IsShared, "1", "0")
            ddlTemplateType.Enabled = templateInfo.Enabled
            disableTvFields = Not templateInfo.Enabled
        End If
        ddlDataSource.Enabled = Not disableTvFields
        If templateInfo IsNot Nothing Then
            RenderFields(templateInfo.Fields, disableTvFields)
        Else
            tvFields.Nodes.Clear()
        End If
    End Sub
    Protected Sub ddlExistingTemplates_SelectedIndexChanged(o As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        RenderSingleTemplate(ddlExistingTemplates.SelectedValue)
    End Sub
    Protected Sub btnVisualize_Click(sender As Object, e As EventArgs)
        Dim validated As Boolean = True
        Dim infoMessage As String = ""
        Dim watch As Stopwatch = Stopwatch.StartNew()

        If String.IsNullOrWhiteSpace(txtBoxExportName.Text) Then
            infoMessage = "Export name is required"
            validated = False
        End If
        If String.IsNullOrWhiteSpace(txtBoxTemplateName.Text) Then
            infoMessage = IIf(String.IsNullOrEmpty(infoMessage), "Template name is required", "Export and template names are required")
            validated = False
        End If

        If Not validated Then
            lblInfoMessage.ForeColor = System.Drawing.Color.Red
            lblInfoMessage.Text = infoMessage
        Else
            If tvFields.CheckedNodes.Count = 0 Then
                lblInfoMessage.ForeColor = System.Drawing.Color.Red
                lblInfoMessage.Text = "Please select at least one field"
            Else
                Try


                    lblInfoMessage.Text = " "
                    Dim TemplateID As Integer? = Nothing
                    If chkBoxUseTemplate.Checked Then
                        TemplateID = CInt(ddlExistingTemplates.SelectedValue)
                    End If
                    Dim checkedNodes As String = String.Join(",", tvFields.CheckedNodes.ToList().Select(Function(fc) fc.Value))
                    Dim dataTable As DataTable = ClsHelper.GenerateExportDataSource(ClsSessionHelper.LogonUser.GlobalID,
                                                                                    txtBoxExportName.Text,
                                                                                    ddlEnvironment.SelectedValue,
                                                                                    ddlCountry.SelectedValue,
                                                                                    txtBoxTemplateName.Text,
                                                                                    ddlTemplateType.SelectedValue.Equals("1"),
                                                                                    ddlDataSource.SelectedValue,
                                                                                    checkedNodes,
                                                                                    TemplateID)
                    Session("ExportDataSource") = dataTable
                    gridResult.MasterTableView.FilterExpression = String.Empty
                    gridResult.MasterTableView.SortExpressions.Clear()
                    gridResult.MasterTableView.GroupByExpressions.Clear()
                    gridResult.MasterTableView.ClearSelectedItems()
                    gridResult.MasterTableView.ClearEditItems()
                    gridResult.DataSource = dataTable
                    gridResult.DataBind()
                    ManageFields(False)
                    ClsHelper.Log("Export Builder Visualize", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Export name: " & txtBoxExportName.Text, watch.ElapsedMilliseconds, False, Nothing)
                Catch ex As Exception
                    lblInfoMessage.ForeColor = System.Drawing.Color.Red
                    lblInfoMessage.Text = "An error has occurred An unexpected error has occurred. please try again later."
                    Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
                    If Not ex.Message Is Nothing Then
                        exceptionMessage = ex.Message
                    End If
                    If Not ex.StackTrace Is Nothing Then
                        exceptionStackTrace = ex.StackTrace
                    End If
                    Dim errorMsg As String = String.Format("Unable to Save Monitoring Message</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}", exceptionMessage _
                        , exceptionStackTrace
                        )
                    ClsHelper.Log("Export Builder Visualize", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Export name: " & txtBoxExportName.Text, watch.ElapsedMilliseconds, True, errorMsg)
                End Try
            End If
        End If
    End Sub
    Protected Sub gridResult_NeedDataSource(source As Object, e As GridNeedDataSourceEventArgs)
        gridResult.DataSource = Session("ExportDataSource")
    End Sub
    Protected Sub btnRest_Click(sender As Object, e As EventArgs)
        ManageFields(True)
        RenderControls()
        Session("ExportDataSource") = Nothing
    End Sub
    Protected Sub btnExport_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Try

            Using excel As ExcelPackage = New ExcelPackage()
                Dim ws As ExcelWorksheet = excel.Workbook.Worksheets.Add("Data")
                ws.Cells("A1").LoadFromDataTable(Session("ExportDataSource"), True)
                ws.Cells("A1:" & ClsHelper.GetExcelColumnName(Session("ExportDataSource").Columns.Count) & "1").Style.Font.Bold = True
                ws.Cells.AutoFitColumns()
                Response.BinaryWrite(excel.GetAsByteArray())
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                Response.AddHeader("content-disposition", String.Format("attachment;  filename=""{0}"".xlsx", ClsHelper.RemoveOddCharachters(txtBoxExportName.Text)))
                watch.Stop()
                ClsHelper.Log("Export Builder Download To Excel", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Export name: " & txtBoxExportName.Text, watch.ElapsedMilliseconds, False, Nothing)
                Response.[End]()
            End Using
        Catch ex As Exception
            lblInfoMessage.ForeColor = System.Drawing.Color.Red
            lblInfoMessage.Text = "An error has occurred An unexpected error has occurred. please try again later."
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}", exceptionMessage _
                        , exceptionStackTrace
                        )
            ClsHelper.Log("Export Builder Download To Excel", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Export name: " & txtBoxExportName.Text, watch.ElapsedMilliseconds, True, errorMsg)
        End Try
    End Sub

    Private Sub ManageFields(enabled As Boolean)
        btnReset.Visible = Not enabled
        btnExport.Visible = Not enabled
        panelGridResult.Visible = Not enabled
        panelVisualizeInfo.Visible = enabled
    End Sub
End Class
