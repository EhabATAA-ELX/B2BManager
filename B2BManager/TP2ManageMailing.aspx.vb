
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class TP2ManageMailing
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim EnvironmentID As Integer = 0
            Dim listType As Integer = 0
            Dim id As Guid = Guid.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("type")) Then
                Integer.TryParse(Request.QueryString("type"), listType)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                Guid.TryParse(Request.QueryString("id"), id)
            End If

            If EnvironmentID > 0 Then
                BindMailingTypes(EnvironmentID)
                RenderControls(EnvironmentID, listType)
                If id <> Guid.Empty Then
                    CType(Master.FindControl("title"), HtmlTitle).Text = "Edit " & IIf(listType = 0, "customer", "country") & " email setting"
                    btnSubmit.Text = "Update"
                    btnSubmit.OnClientClick = "ProcessButton('Update')"
                    LoadMailingData(EnvironmentID, listType, id)
                Else
                    CType(Master.FindControl("title"), HtmlTitle).Text = "New " & IIf(listType = 0, "customer", "country") & " email setting"
                    btnSubmit.Text = "Save"
                    btnSubmit.OnClientClick = "ProcessButton('Add')"
                End If

            End If

        End If
    End Sub

    Private Sub LoadMailingData(environmentID As Integer, listType As Integer, id As Guid)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", environmentID))
        parameters.Add(New SqlParameter("@ID", id))
        parameters.Add(New SqlParameter("@ListType", listType))
        Dim data As DataTable = ClsDataAccessHelper.FillDataTable("TP2MailingList.GetMailingSettingByID", parameters)
        If data IsNot Nothing Then
            If data.Rows.Count = 1 Then
                ClsEbusinessHelper.SetValueCheckBoxList(chkboxMailingTypes, data.Rows("0")("MailingType"))
                txtBoxEmail.Text = data.Rows(0)("email")
                If listType = 1 Then
                    ddlCountry.SelectedValue = data.Rows(0)("countryIsoCode")
                Else
                    txtBoxTradeplaceID.Text = data.Rows(0)("customerTPID")
                    statusInfo.InnerHtml = "<span style='font-size:9pt'>" + data.Rows(0)("TPC_Name") + "</span> <img src='images/ok.gif' />"
                End If
            Else
                ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Close", "CloseWindow();", True)
            End If
        Else
            ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Close", "CloseWindow();", True)
        End If
    End Sub

    Private Sub RenderControls(environmentID As Integer, listType As Integer)
        If listType = 1 Then
            Dim applications As List(Of ClsHelper.Application) = Nothing
            Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
            If clsUser Is Nothing Then
                Return
            End If
            Dim selectedApplication As ClsHelper.Application = clsUser.Applications.Where(Function(fn) (fn.ID = 10)).SingleOrDefault()
            ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, ClsSessionHelper.TP2CountryCode, False, False, True)
            TradeplaceIDTR.Visible = False
        Else
            CountryTR.Visible = False
        End If
    End Sub

    Private Sub BindMailingTypes(environmentID As Integer)
        Dim data As DataTable
        If Cache("MailingType_" & environmentID.ToString()) IsNot Nothing Then
            data = Cache("MailingType_" & environmentID.ToString())
        Else
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
            parameters.Add(New SqlParameter("@EnvironmentID", environmentID))
            data = ClsDataAccessHelper.FillDataTable("TP2MailingList.GetMailingTypes", parameters)
            Cache("MailingType_" & environmentID.ToString()) = data
        End If

        chkboxMailingTypes.DataSource = data
        chkboxMailingTypes.DataTextField = "mailingTypeName"
        chkboxMailingTypes.DataValueField = "mailingType"
        chkboxMailingTypes.DataBind()
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim selectedValues As List(Of String) = chkboxMailingTypes.Items.Cast(Of ListItem)().Where(Function(li) li.Selected).[Select](Function(li) li.Value).ToList()
        If (selectedValues.Count > 0) Then
            Dim mailingType As String = String.Join(",", selectedValues)
            lblErrorInfo.Text = " "
            Dim EnvironmentID As Integer = 0
            Dim listType As Integer = 0
            Dim id As Guid = Guid.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("type")) Then
                Integer.TryParse(Request.QueryString("type"), listType)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                Guid.TryParse(Request.QueryString("id"), id)
            End If
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            If id <> Guid.Empty Then
                parameters.Add(New SqlParameter("@ID", id))
            End If
            If listType = 0 Then
                parameters.Add(New SqlParameter("@customerTPID", txtBoxTradeplaceID.Text))
            Else
                parameters.Add(New SqlParameter("@CountryIsoCode", ddlCountry.SelectedValue))
            End If
            parameters.Add(New SqlParameter("@email", txtBoxEmail.Text))
            parameters.Add(New SqlParameter("@mailingType", mailingType))
            Dim result As String = ClsDataAccessHelper.ExecuteScalar("TP2MailingList.SaveOrUpdate" + IIf(listType = 0, "Customer", "Country") + "Mailing", parameters)
            Select Case result
                Case "Success"
                    ClsHelper.Log(IIf(id <> Guid.Empty, "Update", "Save") + " " + IIf(listType = 0, "Customer", "Country") + " TP2 Mailing", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
                    ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Confirm", "window.parent.Finish(" + IIf(id <> Guid.Empty, "'Update'", "'Save'") + ");", True)
                Case Nothing
                    ClsHelper.Log(IIf(id <> Guid.Empty, "Update", "Save") + " " + IIf(listType = 0, "Customer", "Country") + " TP2 Mailing", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "An unexpected error has occurred")
                    lblErrorInfo.Text = "An unexpected error has occurred. Please try again later."
                Case "Unkown Tradeplace ID"
                    lblErrorInfo.Text = "The Tradeplace ID you entred is not in the system"
                Case Else
                    ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Confirm", "window.parent.ConfirmExistingMapping('" + result + "');", True)
            End Select
        Else
            lblErrorInfo.Text = "Please select at least one mailing type."
        End If
    End Sub
End Class
