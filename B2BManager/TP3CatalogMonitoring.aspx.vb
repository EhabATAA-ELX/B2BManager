
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class TP3CatalogMonitoring
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            RenderControls()
            PopulateSearchGrid()
        End If
    End Sub

    Public Property searchDs As DataTable
        Get

            If Session("TP3Catalog") Is Nothing Then
                Session("TP3Catalog") = ClsDataAccessHelper.FillDataTable("[TP3Monitoring].[GETCatalogMonitoring]", GetSearchParameters(), CommandType.StoredProcedure)
            End If
            Return Session("TP3Catalog")
        End Get
        Set(value As DataTable)
            Session("TP3Catalog") = value
        End Set
    End Property
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

    Private Sub PopulateSearchGrid()
        Dim watch As Stopwatch = Stopwatch.StartNew()
        searchDs = Nothing
        If searchDs.Rows.Count > 0 Then
            If chkBoxErrorORWarning.Checked Then
                Dim dr As DataRow() = searchDs.Select("[XMLGenerationStatus] <> 'Success' OR [SentStatus] <> 'Success' OR [ReceivedStatus] <> 'Success'")
                If dr.Length > 0 Then
                    searchDs = dr.CopyToDataTable()
                Else
                    searchDs = Nothing
                End If
            End If
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
        ClsHelper.Log("Load TP3 Catalog ", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(GetSearchParameters()), watch.ElapsedMilliseconds, False, Nothing)
    End Sub
    Private Function GetSearchParameters() As List(Of SqlParameter)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        If ddlEnvironment.SelectedValue IsNot Nothing Then
            parameters.Add(New SqlParameter("EnvironmentID", CInt(ddlEnvironment.SelectedValue)))
        End If
        If Not String.IsNullOrWhiteSpace(ddlCountry.SelectedValue) Then
            parameters.Add(New SqlParameter("SOPIDs", ddlCountry.SelectedValue))
        End If
        If (RadDateTimePickerFrom.SelectedDate IsNot Nothing) Then
            parameters.Add(New SqlParameter("DATE", RadDateTimePickerFrom.SelectedDate))
        End If
        Return parameters
    End Function

    Private Sub RenderControls()
        Dim applications As List(Of ClsHelper.Application) = Nothing
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = clsUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, IIf(ClsSessionHelper.EbusinessEnvironmentID IsNot Nothing, ClsSessionHelper.EbusinessEnvironmentID, clsUser.DefaultEbusinessEnvironmentID))
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, IIf(ClsSessionHelper.EbusinessSopID IsNot Nothing, ClsSessionHelper.EbusinessSopID, clsUser.DefaultEbusinessSopID), True)
        RadDateTimePickerFrom.SelectedDate = DateTime.Now
    End Sub


    Protected Sub handleRefreshEvent(sender As Object, e As EventArgs)
        PopulateSearchGrid()
    End Sub

    Protected Sub Page_SaveStateComplete(sender As Object, e As EventArgs) Handles Me.SaveStateComplete
        Try
            If IsPostBack Then
                ClsSessionHelper.EbusinessEnvironmentID = ddlEnvironment.SelectedValue
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
End Class
