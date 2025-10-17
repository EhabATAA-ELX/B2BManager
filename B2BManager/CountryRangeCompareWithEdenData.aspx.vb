
Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading
Imports OfficeOpenXml

Partial Class CountryRangeCompareWithEdenData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim fileID As String = ""
            Dim environmentID As Integer = 0
            Integer.TryParse(Context.Request.QueryString("environmentID"), environmentID)
            If Context.Request.QueryString("fileID") IsNot Nothing And environmentID > 0 Then
                fileID = Context.Request.QueryString("fileID")
                Dim clsSelectedFile As ClsFile = ClsSessionHelper.countryRangeFiles.FirstOrDefault(Function(fc) fc.ID = fileID)
                If clsSelectedFile IsNot Nothing Then
                    Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                    parameters.Add(New SqlParameter("@EnvironmentID", environmentID))
                    parameters.Add(New SqlParameter("@FileName", clsSelectedFile.FileInfo.Name))
                    parameters.Add(New SqlParameter("@FullPath", clsSelectedFile.FileInfo.FullName))
                    If ClsSessionHelper.LogonUser IsNot Nothing Then
                        parameters.Add(New SqlParameter("@AddedBy", ClsSessionHelper.LogonUser.GlobalID))
                    End If
                    Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("CountryRange.CheckFileImport", parameters)
                    If dataTable.Rows.Count = 1 Then
                        Dim importStatus As Integer = CInt(dataTable.Rows(0)("ImportStatus"))
                        Dim fileInputID As Integer = CInt(dataTable.Rows(0)("ID"))

                        If importStatus = 0 Then
                            Dim workingThread As Thread = New Thread(Sub() ProcessFileImportAndPrepareDateForComparison(fileInputID, clsSelectedFile.FileInfo.FullName))
                            workingThread.Start()
                        End If
                        hiddenRefreshInfo.Value = fileInputID
                        UpdateInfoDiv(fileInputID, False, importStatus)
                    End If
                End If
            End If
        Else
            Dim __EVENTTARGET As String = Request("__EVENTTARGET")
            Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
            If "hiddenRefreshInfo".Equals(__EVENTTARGET) And Not hiddenRefreshInfo.Value = "" Then
                UpdateInfoDiv(CInt(hiddenRefreshInfo.Value), True)
            End If
        End If
    End Sub

    Protected Sub UpdateInfoDiv(id As Integer, withDelay As Boolean, Optional ByVal ImportStatus As Integer = 0)

        If Cache("ProductsInFile_" + id.ToString()) IsNot Nothing Then
            RadGridProductsInFile.DataSource = DirectCast(Cache("ProductsInFile_" + id.ToString()), DataTable)
            RadGridProductsInFile.DataBind()
            PanelImportedData.Visible = True
        Else
            If Not ImportStatus = 5 Then
                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                parameters.Add(New SqlParameter("@FileID", id))
                Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("CountryRange.GetInputFileLog", parameters)
                divStatus.InnerHtml = ""
                For Each row As DataRow In dataSet.Tables(0).Rows
                    divStatus.InnerHtml += " [" + CDate(row("LoggedOn")).ToString("dd/MM/yyyy HH:mm:ss") + "] " + IIf(CBool(row("IsInError")),
                                    row("Description").ToString() + "&nbsp;<img src='Images/Error.png' height='18' width='18' />" + "</br>" + "<span style='color:red'>" + row("ErrorMessage") + "</span>",
                                    row("Description").ToString() + (IIf(Not row("Description").ToString().StartsWith("Generating"), "&nbsp;<img src='Images/Success.png' height='18' width='18' />", ""))) + "</br>"
                Next
                If withDelay Then
                    Thread.Sleep(1000)
                End If
                ImportStatus = CInt(dataSet.Tables(1).Rows(0)("ImportStatus"))
            End If

            Select Case ImportStatus
                Case 0
                    ScriptManager.RegisterStartupScript(UpdatePane1, UpdatePane1.GetType(), "RefreshInfo", "RefreshInfo();", True)
                Case 1
                    ScriptManager.RegisterStartupScript(UpdatePane1, UpdatePane1.GetType(), "RefreshInfo", "RefreshInfo();", True)
                Case 5
                    divStatus.Visible = False
                    Dim parametersPivotProduct As List(Of SqlParameter) = New List(Of SqlParameter)()
                    parametersPivotProduct.Add(New SqlParameter("@FileID", id))
                    parametersPivotProduct.Add(New SqlParameter("@ReturnData", True))
                    Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("CountryRange.GetPivotProduct", parametersPivotProduct)
                    Cache.Insert("ProductsInFile_" + id.ToString(), dataSet.Tables(0))
                    RadGridProductsInFile.DataSource = dataSet.Tables(0)
                    RadGridProductsInFile.DataBind()
                    If dataSet.Tables.Count = 5 Then
                        Cache.Insert("ProductsInEden_" + id.ToString(), dataSet.Tables(1))
                        RadGridProductsInEden.DataSource = dataSet.Tables(1)
                        RadGridProductsInEden.DataBind()
                        Cache.Insert("ProductsInBothEdenAndFile_" + id.ToString(), dataSet.Tables(2))
                        RadGridProductsInBothEdenAndFile.DataSource = dataSet.Tables(2)
                        RadGridProductsInBothEdenAndFile.DataBind()
                        Cache.Insert("ProductsInFileOnly_" + id.ToString(), dataSet.Tables(3))
                        RadGridProductsInFileOnly.DataSource = dataSet.Tables(3)
                        RadGridProductsInFileOnly.DataBind()
                        Cache.Insert("ProductsInEdenOnly_" + id.ToString(), dataSet.Tables(4))
                        RadGridProductsInEdenOnly.DataSource = dataSet.Tables(4)
                        RadGridProductsInEdenOnly.DataBind()
                    End If
                    PanelImportedData.Visible = True
                Case Else
                    AddLogLine(id, "Import aborted due to previous errors", True, "Import aborted due to previous errors")
                    divStatus.InnerHtml += " [" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "] <span style='color:red'>Import aborted due to previous errors</span>"
            End Select
        End If
    End Sub

    Protected Sub ProcessFileImportAndPrepareDateForComparison(fileInputID As Integer, filePath As String)
        Dim dataSet As DataSet = New DataSet()
        dataSet.ReadXml(filePath)
        Dim currentUpdateStatus As Integer = 0
        For Each dataTable As DataTable In dataSet.Tables
            Dim dt As DataTable = dataTable.Copy()
            currentUpdateStatus += ImportDataTableWithBulk(dt, "CountryRange." + dt.TableName, fileInputID)
        Next
        If currentUpdateStatus <= dataSet.Tables.Count Then
            AddLogLine(fileInputID, "Generating products pivot table...", False, Nothing)
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@FileID", fileInputID))
            ClsDataAccessHelper.FillDataTable("CountryRange.GetPivotProduct", parameters)
        End If
    End Sub

    Private Function ImportDataTableWithBulk(ByRef Dt As DataTable, ByVal DestTableBulkName As String, ByVal fileId As Integer) As Integer

        Dim IsInError As Boolean = False
        Dim ErrorMessage As String = Nothing
        Dim Description As String = "Bulk copy <b>" + DestTableBulkName.Replace("CountryRange.", "") + "</b> data, Total rows: <b>" + Dt.Rows.Count.ToString() + "</b>"
        Dim UpdateStatus As Integer = 1

        Dim cn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
        Try
            cn.Open()
            If Not (DestTableBulkName.ToUpper().EndsWith("ATTRIBUTE") Or DestTableBulkName.ToUpper().EndsWith("COMPONENT")) Then
                Dt.AcceptChanges()
                Dim fileIDColumn As DataColumn = New DataColumn("FileID", GetType(System.Int32))
                fileIDColumn.AllowDBNull = False
                fileIDColumn.DefaultValue = fileId
                fileIDColumn.ColumnName = "FileID"
                Dt.Columns.Add(fileIDColumn)
                If (DestTableBulkName.ToUpper().EndsWith("PRODUCT")) Then
                    If Dt.Columns.Contains("Kit") Then
                        Dt.Columns.Remove(Dt.Columns("Kit"))
                    End If
                End If
            End If
            Dim copy As SqlBulkCopy = New SqlBulkCopy(cn)
            copy.BulkCopyTimeout = 0
            copy.DestinationTableName = DestTableBulkName
            copy.WriteToServer(Dt)
            copy.Close()
            cn.Close()
            If (DestTableBulkName.ToUpper().Contains("ATTRIBUTE") Or DestTableBulkName.ToUpper().Contains("COMPONENT")) Then
                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                parameters.Add(New SqlParameter("@FileID", fileId))
                parameters.Add(New SqlParameter("@TableName", DestTableBulkName))
                ClsDataAccessHelper.FillDataTable("CountryRange.UpdateFileID", parameters)
            End If
        Catch ex As Exception
            IsInError = True
            ErrorMessage = ex.Message
            UpdateStatus = 2
        End Try

        AddLogLine(fileId, Description, IsInError, ErrorMessage, UpdateStatus)
        Return UpdateStatus
    End Function

    Private Sub AddLogLine(fileId As Integer, Description As String, IsInError As Boolean, ErrorMessage As String, Optional UpdateStatus As Integer = 0)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@FileID", fileId))
        parameters.Add(New SqlParameter("@Description", Description))
        parameters.Add(New SqlParameter("@IsInError", IsInError))
        parameters.Add(New SqlParameter("@ErrorMessage", ErrorMessage))
        If UpdateStatus > 0 Then
            parameters.Add(New SqlParameter("@UpdateStatus", UpdateStatus))
        End If
        ClsDataAccessHelper.FillDataTable("CountryRange.AddLogLine", parameters)
    End Sub



    Protected Sub RadGridProductsInFile_NeedDataSource(source As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs)
        If Cache("ProductsInFile_" + hiddenRefreshInfo.Value.ToString()) IsNot Nothing Then
            RadGridProductsInFile.DataSource = DirectCast(Cache("ProductsInFile_" + hiddenRefreshInfo.Value.ToString()), DataTable)
        End If

    End Sub
    Protected Sub RadGridProductsInEden_NeedDataSource(source As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs)
        If Cache("ProductsInEden_" + hiddenRefreshInfo.Value.ToString()) IsNot Nothing Then
            RadGridProductsInEden.DataSource = DirectCast(Cache("ProductsInEden_" + hiddenRefreshInfo.Value.ToString()), DataTable)
        End If
    End Sub
    Protected Sub RadGridProductsInFileOnly_NeedDataSource(source As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs)
        If Cache("ProductsInFileOnly_" + hiddenRefreshInfo.Value.ToString()) IsNot Nothing Then
            RadGridProductsInFileOnly.DataSource = DirectCast(Cache("ProductsInFileOnly_" + hiddenRefreshInfo.Value.ToString()), DataTable)
        End If
    End Sub
    Protected Sub RadGridProductsInEdenOnly_NeedDataSource(source As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs)
        If Cache("ProductsInEdenOnly_" + hiddenRefreshInfo.Value.ToString()) IsNot Nothing Then
            RadGridProductsInEdenOnly.DataSource = DirectCast(Cache("ProductsInEdenOnly_" + hiddenRefreshInfo.Value.ToString()), DataTable)
        End If
    End Sub
    Protected Sub RadGridProductsInBothEdenAndFile_NeedDataSource(source As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs)
        If Cache("ProductsInBothEdenAndFile_" + hiddenRefreshInfo.Value.ToString()) IsNot Nothing Then
            RadGridProductsInBothEdenAndFile.DataSource = DirectCast(Cache("ProductsInBothEdenAndFile_" + hiddenRefreshInfo.Value.ToString()), DataTable)
        End If
    End Sub
    Protected Sub btnExportToExcel_Click(sender As Object, e As EventArgs)
        Using excel As ExcelPackage = New ExcelPackage()

            AddDataToExcel(excel, "Products In File", "ProductsInFile_")
            AddDataToExcel(excel, "Products In Eden", "ProductsInEden_")
            AddDataToExcel(excel, "Products In File Only", "ProductsInFileOnly_")
            AddDataToExcel(excel, "Products In Eden Only", "ProductsInEdenOnly_")
            AddDataToExcel(excel, "Products In Both", "ProductsInBothEdenAndFile_")

            Response.BinaryWrite(excel.GetAsByteArray())
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Response.AddHeader("content-disposition", String.Format("attachment;  filename=Country_Range_File_Compared_To_Eden_Data_{0}.xlsx", DateTime.Now.ToString("dd-MM-yyyy HH:mm")))
            Response.[End]()
        End Using
    End Sub

    Private Sub AddDataToExcel(ByRef excel As ExcelPackage, workSheetName As String, dataTableCachePrefix As String)
        Dim ws As ExcelWorksheet = excel.Workbook.Worksheets.Add(workSheetName)
        Try
            If Cache(dataTableCachePrefix + hiddenRefreshInfo.Value.ToString()) IsNot Nothing Then
                Dim exportDataTable As DataTable = DirectCast(Cache(dataTableCachePrefix + hiddenRefreshInfo.Value.ToString()), DataTable)
                ws.Cells("A1").LoadFromDataTable(exportDataTable, True)
                ws.Cells("A1:" & ClsHelper.GetExcelColumnName(exportDataTable.Columns.Count) & "1").Style.Font.Bold = True
                ws.Cells.AutoFitColumns()
            End If
        Catch ex As Exception
        End Try

    End Sub
End Class
