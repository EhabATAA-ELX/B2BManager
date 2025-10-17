
Imports System.Data
Imports OfficeOpenXml

Partial Class EbusinessActivityHistory
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            Form.DefaultButton = btnSearch.UniqueID
            Dim fromDate As DateTime = Date.Parse(DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString())
            RadDateTimePickerFrom.SelectedDate = fromDate
            Dim toDate = fromDate.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999)
            RadDateTimePickerTo.SelectedDate = toDate
            gridSearch.MasterTableView.GetColumn("WebUserID").Visible = Request.QueryString("UID") Is Nothing
            gridSearch.MasterTableView.GetColumn("Customer_ID").Visible = Request.QueryString("issuperuser") IsNot Nothing
            uniqueGeneratedKey.Value = DateTime.Now.GetHashCode().ToString("x")
        End If
    End Sub

    Public Property searchDs As DataTable
        Get
            If Session("searchUserDt_" + uniqueGeneratedKey.Value) Is Nothing Then
                Session("searchUserDt_" + uniqueGeneratedKey.Value) = (TryCast(SqlDataSource1.[Select](DataSourceSelectArguments.Empty), DataView)).Table
            End If
            Return CType(Session("searchUserDt_" + uniqueGeneratedKey.Value), DataTable)
        End Get
        Set(value As DataTable)
            Session("searchUserDt_" + uniqueGeneratedKey.Value) = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If (Not IsPostBack) Then
            btnExcel.Visible = False
            gridSearch.DataBind()
        End If
    End Sub
    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        searchDs = Nothing
        If RadDateTimePickerFrom.SelectedDate.Value.ToShortDateString = DateTime.Now.ToShortDateString Then
            SqlDataSource1.EnableCaching = False
        Else
            SqlDataSource1.EnableCaching = True
        End If
        If searchDs IsNot Nothing Then
            gridSearch.DataSourceID = SqlDataSource1.UniqueID
            gridSearch.DataBind()
            gridPanel.CssClass = ""
            btnExcel.Visible = True
        End If
    End Sub
    Protected Sub SqlDataSource1_Selecting(sender As Object, e As SqlDataSourceSelectingEventArgs)
        e.Command.CommandTimeout = 0
    End Sub
    Protected Sub btnExcel_Click(sender As Object, e As EventArgs)
        Using excel As ExcelPackage = New ExcelPackage()
            AddDataToExcel(excel, "Activity Logs")
            Response.BinaryWrite(excel.GetAsByteArray())
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Response.AddHeader("content-disposition", String.Format("attachment;  filename=ActivityLog_{0}.xlsx", DateTime.Now.ToString("dd-MM-yyyy HH:mm")))
            Response.[End]()
        End Using
    End Sub

    Private Sub AddDataToExcel(ByRef excel As ExcelPackage, workSheetName As String)
        Dim ws As ExcelWorksheet = excel.Workbook.Worksheets.Add(workSheetName)
        Try
            ws.Cells("A1").LoadFromDataTable(searchDs, True)
            ws.Cells("A1:" & ClsHelper.GetExcelColumnName(searchDs.Columns.Count) & "1").Style.Font.Bold = True
            ws.Cells.AutoFitColumns()
        Catch ex As Exception
            ClsSendEmailHelper.SendErrorEmail(ex.Message)
        End Try
    End Sub
End Class
