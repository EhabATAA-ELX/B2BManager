
Imports System.Data
Imports System.Data.SqlClient

Partial Class EbusinessNewUser
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            ForceRedirect()
        Else
            Dim EnvironmentID As Integer = 0
            Dim cid As Guid = Guid.Empty
            Dim sopID As String = String.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("sopid")) Then
                sopID = Request.QueryString("sopid")
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("cid")) Then
                Guid.TryParse(Request.QueryString("cid"), cid)
            End If
            If EnvironmentID > 0 AndAlso sopID.Length > 0 Then
                If cid = Guid.Empty Then
                    If Not clsUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_NEW_USER) Then
                        ForceRedirect()
                    End If
                Else
                    If Not clsUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_NEW_SUPER_USER) Then
                        ForceRedirect()
                    End If
                End If
            End If
        End If
    End Sub

    Protected Function GetCompanyGlobalID() As String
        Dim cid As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("cid")) Then
            Guid.TryParse(Request.QueryString("cid"), cid)
        End If
        If Guid.Empty = cid Then
            Return "undefined"
        Else
            Return "\'" + cid.ToString() + "\'"
        End If
    End Function

    Protected Sub ForceRedirect()
        Dim HideHeader As Boolean = False
        Boolean.TryParse(Request.QueryString("HideHeader"), HideHeader)
        If HideHeader Then
            Response.Redirect("UnauthorizedBasic.aspx?HideHeader=true", True)
        Else
            Response.Redirect("UnauthorizedBasic.aspx", True)
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim cid As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("cid")) Then
            Guid.TryParse(Request.QueryString("cid"), cid)
        End If
        If Guid.Empty = cid Then
            UserDetails.Mode = "CreateSuperUser"
            CType(UserDetails.FindControl("HD_Mode"), HiddenField).Value = "CreateSuperUser"
        Else
            UserDetails.Mode = "CreateUser"
            CType(UserDetails.FindControl("HD_Mode"), HiddenField).Value = "CreateUser"
        End If
        If Not IsPostBack Then
            Dim EnvironmentID As Integer = 0

            Dim sopID As String = String.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("sopid")) Then
                sopID = Request.QueryString("sopid")
            End If

            If EnvironmentID > 0 AndAlso sopID.Length > 0 Then
                RenderControls(EnvironmentID, sopID, cid)
            End If
        End If
    End Sub

    Private Sub RenderControls(environmentID As Integer, sopID As String, cid As Guid)
        CType(UserDetails.FindControl("HD_EnvironmentID"), HiddenField).Value = environmentID.ToString()
        CType(UserDetails.FindControl("HD_SOPNAME"), HiddenField).Value = sopID
        If Guid.Empty = cid Then 'Case new super user
            CType(Master.FindControl("title"), HtmlTitle).Text = "New super user account"
            CType(UserDetails.FindControl("ddlDefaultMenu"), DropDownList).Items.Clear()
            Dim superUserEntry As ListItem = New ListItem("Customer List (SUPERUSER)", "SUPERUSER")
            CType(UserDetails.FindControl("ddlDefaultMenu"), DropDownList).Items.Add(superUserEntry)
            CType(UserDetails.FindControl("ddlDefaultMenu"), DropDownList).Enabled = False
            For Each row As DataRow In ClsEbusinessHelper.GetSuperUserCategories(environmentID, Page.Cache).Rows
                Dim item As ListItem = New ListItem(row("CAT_NAME"), row("CAT_GLOBALID").ToString())
                CType(UserDetails.FindControl("ddlSuperUserCategory"), DropDownList).Items.Add(item)
            Next
            Dim dataCountrySettings As DataTable = ClsEbusinessHelper.GetCountrySettings(environmentID, Page.Cache)
            If dataCountrySettings IsNot Nothing Then
                If dataCountrySettings.Select("S_SOP_ID='" + sopID + "'").Count = 1 Then
                    If dataCountrySettings.Select("S_SOP_ID='" + sopID + "'")(0)("C_GLOBALID") IsNot DBNull.Value Then
                        CType(UserDetails.FindControl("HD_C_GlobalID"), HiddenField).Value = dataCountrySettings.Select("S_SOP_ID='" + sopID + "'")(0)("C_GLOBALID").ToString()
                    Else
                        ForceRedirect()
                    End If
                Else
                    ForceRedirect()
                End If
            Else
                ForceRedirect()
            End If
        Else
            CType(UserDetails.FindControl("HD_C_GlobalID"), HiddenField).Value = cid.ToString()
            CType(Master.FindControl("title"), HtmlTitle).Text = "New user account"
            CType(UserDetails.FindControl("trSuperUserCategory"), HtmlTableRow).Visible = False
        End If


        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", environmentID))
        parameters.Add(New SqlParameter("@CID", cid))
        Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("[Ebusiness].[UsrMgmt_GetCustomerByID]", parameters)
        If dataSet.Tables.Count = 2 Then
            Dim customerDetailsDT As DataTable = dataSet.Tables(0)
            If customerDetailsDT.Rows.Count > 0 Then
                Dim companyRow As DataRow = customerDetailsDT.Rows(0)
                CType(UserDetails.FindControl("HD_CustomerCode"), HiddenField).Value = ClsDataAccessHelper.GetText(companyRow, "C_CUID")
            End If
        End If


        Dim regularItems As List(Of ListItem) = New List(Of ListItem)
        Dim suItems As List(Of ListItem) = New List(Of ListItem)
        For Each row As DataRow In ClsEbusinessHelper.GetUserRights(environmentID, sopID, Page.Cache).Rows
            Dim item As ListItem = New ListItem(row("Name"), row("OS_GLOBALID").ToString())
            item.Selected = row("Checked")
            If row.Table.Columns.Contains("IsSuperUserSpecific") _
                    AndAlso row("IsSuperUserSpecific") IsNot DBNull.Value _
                    AndAlso row("IsSuperUserSpecific").ToString() = "1" Then
                suItems.Add(item)
            Else
                regularItems.Add(item)
            End If
        Next
        CType(UserDetails.FindControl("uerRightsChbkoxList"), CheckBoxList).Items.AddRange(regularItems.ToArray())
        If (UserDetails.Mode = "CreateSuperUser") Then
            CType(UserDetails.FindControl("uerRightsChbkoxList"), CheckBoxList).Items.AddRange(suItems.ToArray())
        End If



        Dim gwsGroupTypes As DataTable = ClsEbusinessHelper.GetGWSGroupType(environmentID, Page.Cache)
        Dim gwsGroupSelectValue = ""
        If gwsGroupTypes IsNot Nothing Then
            Dim index As Integer = 0
            For Each dataRow As DataRow In gwsGroupTypes.Select("S_SOP_ID='" + sopID + "'")
                Dim gwsItem As ListItem = New ListItem(dataRow("varUserName"), dataRow("intUserID").ToString() + "|" + ClsDataAccessHelper.GetText(dataRow, "EnabledGWSRight"))
                If index = 0 Then
                    gwsItem.Selected = True
                    gwsGroupSelectValue = ClsDataAccessHelper.GetText(dataRow, "EnabledGWSRight")
                End If
                CType(UserDetails.FindControl("ddlGWSGroup"), DropDownList).Items.Add(gwsItem)
                index += 1
            Next
            ClsEbusinessHelper.SetValueCheckBoxList(CType(UserDetails.FindControl("chkboxListGWSGroup"), CheckBoxList), gwsGroupSelectValue)
        End If


        'NOGWS
        Dim sopOrderTypes As DataTable = ClsEbusinessHelper.Get_SOP_OrderTypes(environmentID, String.Empty, Page.Cache)
        If sopOrderTypes IsNot Nothing Then
            CType(UserDetails.FindControl("chkboxListOrderTypes"), CheckBoxList).Items.Clear()
            For Each dataRow As DataRow In sopOrderTypes.Select("S_SOP_ID='" + sopID + "'")
                Dim orderTypeItem As ListItem = New ListItem(dataRow("OT_NAME"), dataRow("ID_OT_SOP").ToString())
                If dataRow("OT_NAME").ToString().ToLower.Contains("trade order") Then
                    orderTypeItem.Selected = True
                End If
                CType(UserDetails.FindControl("chkboxListOrderTypes"), CheckBoxList).Items.Add(orderTypeItem)
            Next
        End If



        CType(UserDetails.FindControl("btnCancel"), LinkButton).Text = "<i class=""fas fa-ban""></i> Cancel"
        CType(UserDetails.FindControl("btnSubmit"), Button).Text = "Create"
        'TODO #userregistration
        '''CType(UserDetails.FindControl("changePassword"), HtmlGenericControl).Visible = False
        '''CType(UserDetails.FindControl("txtBoxPassword"), TextBox).Visible = True

    End Sub
End Class
