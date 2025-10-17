
Imports Telerik.Web.UI

Partial Class B2BContactsManagement
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim target As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
        If Not IsPostBack Or "RenderControls".Equals(__EVENTARGUMENT) Then
            RenderControls()
        End If
    End Sub
    Private Sub RenderControls()
        Dim applications As List(Of ClsHelper.Application) = Nothing
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Return
        Else
            If Not (ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_CONTACT)) Then
                DeleteScript.Visible = False
                dialogDeletePanel.Visible = False
            End If

            If Not (ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_DUPLICATE_CONTACT)) Then
                CreateOrDuplicateScript.Visible = False
                tdNewContact.Visible = False
                WindowNewContact.Visible = False
            End If
        End If
        Dim selectedApplication As ClsHelper.Application = clsUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, IIf(ClsSessionHelper.EbusinessEnvironmentID IsNot Nothing, ClsSessionHelper.EbusinessEnvironmentID, ClsSessionHelper.LogonUser.DefaultEbusinessEnvironmentID))
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, IIf(ClsSessionHelper.EbusinessSopID IsNot Nothing, ClsSessionHelper.EbusinessSopID, ClsSessionHelper.LogonUser.DefaultEbusinessSopID), False)
    End Sub

    Protected Sub ddlCountry_SelectedIndexChanged(o As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        LoadData()
    End Sub

    Protected Sub imageBtnRefresh_Click(sender As Object, e As ImageClickEventArgs)
        LoadData()
    End Sub

    Protected Sub ddlEnvironment_SelectedIndexChanged(sender As Object, e As EventArgs)
        LoadData()
    End Sub

    Private Sub LoadData()
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "LoadData", "LoadData();", True)
    End Sub

    Protected Sub Page_SaveStateComplete(sender As Object, e As EventArgs) Handles Me.SaveStateComplete
        Try
            If IsPostBack Then
                ClsSessionHelper.EbusinessEnvironmentID = ddlEnvironment.SelectedValue
                ClsSessionHelper.EbusinessSopID = ddlCountry.SelectedValue
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

    Protected Function GetActionsTemplate() As String
        Dim actionsTemplate = ""
        actionsTemplate = "<span style=""text-align:center;vertical-align:top;display: inline-block""><img src=""Images/edit.png"" " _
                          + " Class=""cursor-pointer"" title=""View contact details"" onclick=""EditContact(\'#data#\',\'#environmentid#\',\'#sopid#\',this)"" width=""22px"" height=""22px"" style=""display: inline-block"">"
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser IsNot Nothing Then
            If (clsUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_CONTACT_DETAILS)) Then
                thActions.InnerText = "Edit"
            End If
            If (clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_CONTACT)) Then
                actionsTemplate += "<img src=""Images/delete.png"" title=""Delete contact"" class=""cursor-pointer"" style=""display: inline-block;vertical-align: top;"" onclick=""DeleteContact(this,\'#data#\',\'#environmentid#\')"" width=""22px"" height=""22px""></span>"
                thActions.InnerText = "Actions"
            End If
            If (clsUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_DUPLICATE_CONTACT)) Then
                actionsTemplate += "<i title=""Duplicate contact"" class=""cursor-pointer far fa-copy"" style=""display: inline-block;vertical-align: top;font-size: 22px;color: #4d89c7;"" onclick=""DuplicateContact(\'#data#\',\'#environmentid#\',\'#sopid#\')"" ></i>"
                thActions.InnerText = "Actions"
            End If
        End If
        Return actionsTemplate
    End Function
End Class
