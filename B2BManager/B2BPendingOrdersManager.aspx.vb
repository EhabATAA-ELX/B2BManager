
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI

Partial Class B2BPendingOrdersManager
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            RenderControls()
            RunSearch()
        End If
    End Sub

    Private Sub PopulateSearchGrid()
        Dim watcher As Stopwatch = Stopwatch.StartNew()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", ddlEnvironment.SelectedValue))
        parameters.Add(New SqlParameter("@SOPIDs", ddlCountry.SelectedValue))
        parameters.Add(New SqlParameter("@SearchText", txtBoxSearchInDetails.Text))
        b2bOrdersGrid.DataSource = ClsDataAccessHelper.FillDataTable("B2BOrders.GetPendingOrders", parameters)
        selectedEnvironmentID.Value = ddlEnvironment.SelectedValue
        ClsHelper.Log("Load Pending Orders", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
    End Sub

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            Me.Form.DefaultButton = btnSearch.UniqueID
        End If
    End Sub

    Private Sub RunSearch()
        PopulateSearchGrid()
        b2bOrdersGrid.DataBind()
    End Sub

    Protected Sub b2bOrdersGrid_NeedDataSource(source As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs)
        PopulateSearchGrid()
    End Sub

    Private Sub RenderControls(Optional ByVal selectedValue As String = Nothing)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        If ClsSessionHelper.LogonUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = ClsSessionHelper.LogonUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True)
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, ddlCountry.SelectedValue, selectedApplication.SelectAllCountriesByDefault)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        RunSearch()
    End Sub

    Protected Sub b2bOrdersGrid_ItemDataBound(sender As Object, e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            Dim item As GridDataItem = CType(e.Item, GridDataItem)
            If Not String.IsNullOrEmpty(item.DataItem.row.Item("Resend").ToString()) Then
                Dim img As Image = New Image()
                img.Width = "18"
                img.Height = "18"
                img.Attributes.Add("ID", "HistoryImgTooltip_" + item.DataItem.row.Item("Correl_ID").ToString())
                img.Attributes.Add("style", "margin-left:5px")
                img.ImageUrl = "Images/Warning.png"
                Dim newdiv As HtmlGenericControl = New HtmlGenericControl("DIV")
                newdiv.Attributes.Add("class", "hidden")
                newdiv.Attributes.Add("ID", "HistoryTooltipContent_" + item.DataItem.row.Item("Correl_ID").ToString())
                newdiv.Controls.Add(New LiteralControl("<b>Resend History:</b></br>" + item.DataItem.row.Item("Resend").ToString()))
                item("ClientSelectColumn").Controls.Add(img)
                item("ClientSelectColumn").Controls.Add(newdiv)
                Me.RadToolTipManager1.TargetControls.Add("HistoryImgTooltip_" + item.DataItem.row.Item("Correl_ID").ToString(), True)
            End If
        End If
    End Sub
End Class
