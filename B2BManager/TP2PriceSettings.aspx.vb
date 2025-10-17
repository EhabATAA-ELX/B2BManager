
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class TP2PriceSettings
    Inherits System.Web.UI.Page

    Private clsUser As ClsUser
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        clsUser = ClsSessionHelper.LogonUser
        If Not IsPostBack Then
            Me.Form.DefaultButton = btnSearch.UniqueID
            RenderControls()
            PopulateSearchGrid()
        End If
    End Sub

    Private Sub RenderControls()
        If clsUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = clsUser.Applications.Where(Function(fn) (fn.ID = 10)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, ClsSessionHelper.TP2EnvironmentID)
        If Not clsUser.Actions.Contains(ClsHelper.ActionDesignation.MAINTAIN_TP_PRICE_SETTINGS_ADD_CHANGE_DELETE) Then
            gridSearch.MasterTableView.GetColumn("Actions").Visible = False
            gridSearch.MasterTableView.GetColumn("Actions").Display = False
            WindowPriceSetting.Visible = False
            placeHolderManageScript.Visible = False
            newPriceMapping.Visible = False
        End If
    End Sub

    Private Function GetSearchParameters() As List(Of SqlParameter)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        If ddlEnvironment.SelectedValue IsNot Nothing Then
            parameters.Add(New SqlParameter("EnvironmentID", CInt(ddlEnvironment.SelectedValue)))
        End If
        If Not String.IsNullOrWhiteSpace(txtBoxSearchInDetails.Text) Then
            parameters.Add(New SqlParameter("SearchText", txtBoxSearchInDetails.Text.Trim()))
        End If
        Return parameters
    End Function

    Private Sub UpdateLabel()
        If searchDs.Rows.Count = 0 Then
            lblInformation.InnerHtml = "No results found, please check your filters"
            lblInformationContainer.Attributes.CssStyle.Add("left", "15px")
            lblInformationContainer.Attributes.CssStyle.Remove("right")
        Else
            lblInformation.InnerHtml = "Showing <span class='informationlabel-text'>" & searchDs.Rows.Count &
                                        "</span> row" & IIf(searchDs.Rows.Count > 1, "s", "") & " in <span class='information-label-text'>" + ddlEnvironment.SelectedItem.Text + "</span> environment"
            lblInformationContainer.Attributes.CssStyle.Add("right", "15px")
            lblInformationContainer.Attributes.CssStyle.Remove("left")
        End If
    End Sub

    Public Property searchDs As DataTable
        Get

            If Session("priceSettings") Is Nothing Then
                Session("priceSettings") = ClsDataAccessHelper.FillDataTable("TP2PriceSettings.GetPriceSettings", GetSearchParameters(), CommandType.StoredProcedure)
            End If
            Return Session("priceSettings")
        End Get
        Set(value As DataTable)
            Session("priceSettings") = value
        End Set
    End Property

    Private Sub PopulateSearchGrid()
        Dim watch As Stopwatch = Stopwatch.StartNew()
        searchDs = Nothing
        If searchDs.Rows.Count > 0 Then
            gridSearch.DataSource = searchDs
            gridSearch.DataBind()
        End If

        If searchDs IsNot Nothing AndAlso ddlEnvironment.SelectedItem IsNot Nothing Then
            UpdateLabel()
            If searchDs.Rows.Count = 0 Then
                gridSearch.Visible = False
            Else
                gridSearch.Visible = True
            End If
        Else
            lblInformation.InnerHtml = ""
        End If
        ClsHelper.Log("Load TP2 Price Settings", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(GetSearchParameters()), watch.ElapsedMilliseconds, False, Nothing)
    End Sub

    Protected Sub Page_SaveStateComplete(sender As Object, e As EventArgs) Handles Me.SaveStateComplete
        Try
            If IsPostBack Then
                ClsSessionHelper.TP2EnvironmentID = ddlEnvironment.SelectedValue
            End If
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
    End Sub
    Protected Sub gridSearch_NeedDataSource(source As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs)
        If searchDs.Rows.Count > 0 Then
            UpdateLabel()
            gridSearch.DataSource = searchDs
        End If
    End Sub

    Protected Sub handleRefreshEvent(sender As Object, e As EventArgs)
        PopulateSearchGrid()
    End Sub
End Class
