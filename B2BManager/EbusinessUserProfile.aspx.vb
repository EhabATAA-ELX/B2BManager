
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI

Partial Class EbusinessUserProfile
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim target As String = Request("__EVENTTARGET")
        If Not IsPostBack Or "Refresh".Equals(target) Then
            Dim EnvironmentID As Integer = 0
            Dim Uid As Guid = Guid.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("uid")) Then
                Guid.TryParse(Request.QueryString("uid"), Uid)
            End If
            If EnvironmentID > 0 AndAlso Uid <> Guid.Empty Then
                BindUserInformation(EnvironmentID, Uid)
                If Not (ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_USER_SUPER_USER_DETAILS)) Then
                    DisableControls()
                End If
            Else
                UserDetails.Visible = False
            End If
        End If
    End Sub

    Protected Sub btnTransform_Click(ByVal sender As Object, ByVal e As EventArgs)
        If Page.IsValid Then
            Dim EnvironmentID = CType(UserDetails.FindControl("HD_EnvironmentID"), HiddenField).Value
            Dim SOPNAME = CType(UserDetails.FindControl("HD_SOPNAME"), HiddenField).Value
            Dim U_GLOBALID = CType(UserDetails.FindControl("HD_U_GlobalID"), HiddenField).Value
            TransformUserInSuperuser(EnvironmentID, SOPNAME, U_GLOBALID)
        End If
    End Sub

    Private Sub BindUserInformation(EnvironmentID As Integer, Uid As Guid)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        Dim isSuperUser As Boolean = False
        Dim isChironUser As Boolean = False

        CType(UserDetails.FindControl("HD_EnvironmentID"), HiddenField).Value = EnvironmentID.ToString()
        CType(UserDetails.FindControl("HD_U_GlobalID"), HiddenField).Value = Uid.ToString()
        CType(UserDetails.FindControl("HD_Mode"), HiddenField).Value = "UpdateUser"
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@UID", Uid))
        Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("[Ebusiness].[UsrMgmt_GetUserByID]", parameters)
        If dataSet.Tables.Count = 3 Then
            If dataSet.Tables(0).Rows.Count > 0 Then
                Dim userRow As DataRow = dataSet.Tables(0).Rows(0)
                CType(UserDetails.FindControl("HD_SOPNAME"), HiddenField).Value = userRow("S_SOP_ID").ToString()
                CType(UserDetails.FindControl("HD_CustomerCode"), HiddenField).Value = userRow("C_CUID").ToString()

                userTypeImage.ImageUrl = "Images\Ebusiness\UserType\" + userRow("UserType").ToString() + ".png"
                Dim userActive As Boolean = userRow("U_ISACTIVE").ToString() = "1" Or userRow("U_ISACTIVE").ToString() = "True"
                Dim customerActive As Boolean = userRow("C_ISACTIVE").ToString() = "1" Or userRow("C_ISACTIVE").ToString() = "True"
                Dim userTags As String = String.Empty

                If userActive AndAlso customerActive Then
                    If userRow("U_TYPE") = 0 Then
                        userTags += ClsEbusinessHelper.FormatTag("Real user", "#0070c5")
                    Else
                        userTags += ClsEbusinessHelper.FormatTag("Test user", "#df5b5b")
                    End If

                    Select Case userRow("UserType")
                        Case 1
                        Case 4
                        Case 7
                        Case 10
                        Case 13
                        Case 16
                        Case 19
                        Case 22
                            userTags += ClsEbusinessHelper.FormatTag("Active", "#3bbd5d")
                        Case 2
                        Case 5
                        Case 8
                        Case 11
                        Case 14
                        Case 17
                        Case 20
                        Case 23
                            userTags += ClsEbusinessHelper.FormatTag("Nearly Expired", "#e67e22")
                        Case 3
                        Case 6
                        Case 9
                        Case 12
                        Case 15
                        Case 18
                        Case 21
                        Case 24
                            userTags += ClsEbusinessHelper.FormatTag("Expired", "#e85656")
                    End Select
                ElseIf Not userActive Then
                    userTags += ClsEbusinessHelper.FormatTag("User Deleted", "#949494")
                Else
                    userTags += ClsEbusinessHelper.FormatTag("Customer Deleted", "#949494")
                End If
                ImgTooltipHelp_lblUserID.Text = userRow("U_ID").ToString()
                TooltipContentHelp_lblUserID.InnerHtml = "<b>User Global ID:</b> " + userRow("U_GLOBALID").ToString()
                CType(UserDetails.FindControl("txtBoxDisplayName"), TextBox).Text = ClsDataAccessHelper.GetText(userRow, "varName")
                If userActive AndAlso customerActive Then
                    lblLoginEmail.Text = ClsDataAccessHelper.GetText(userRow, "U_EMAIL_LOGIN")
                    lblLoginName.Text = ClsSciUtil.GuidLoginNameHandler(ClsDataAccessHelper.GetText(userRow, "varUserName"))
                    lblDisplayName.Text = ClsDataAccessHelper.GetText(userRow, "varName")
                    lblEmail.Text = ClsDataAccessHelper.GetText(userRow, "varEmail")
                    If ClsDataAccessHelper.GetText(userRow, "varEmail") <> "" Then
                        Dim parametersEmail As List(Of SqlParameter) = New List(Of SqlParameter)()
                        parametersEmail.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
                        parametersEmail.Add(New SqlParameter("@Email", ClsDataAccessHelper.GetText(userRow, "varEmail")))
                        Dim dataSetEMAIL As DataSet = ClsDataAccessHelper.FillDataSet("[Ebusiness].[UsrMgmt_PS_GET USER_WithSameEmail]", parametersEmail)
                        If dataSetEMAIL IsNot Nothing AndAlso dataSetEMAIL.Tables(0).Rows.Count > 1 Then
                            CType(UserDetails.FindControl("Div_uniqueEmail"), HtmlGenericControl).Visible = True
                        End If
                    End If

                    If userRow("U_ISSUPERUSER") Then
                        CType(UserDetails.FindControl("ddlDefaultMenu"), DropDownList).Items.Clear()
                        Dim superUserEntry As ListItem = New ListItem("Customer List (SUPERUSER)", "SUPERUSER")
                        CType(UserDetails.FindControl("ddlDefaultMenu"), DropDownList).Items.Add(superUserEntry)
                        CType(UserDetails.FindControl("ddlDefaultMenu"), DropDownList).Enabled = False
                        If userRow("SU_SALESREP_NO") IsNot DBNull.Value AndAlso userRow("SU_SALESREP_NO") IsNot String.Empty Then
                            CType(UserDetails.FindControl("cbSalesRep"), CheckBox).Checked = True
                            CType(UserDetails.FindControl("txtSalesRep"), TextBox).Text = userRow("SU_SALESREP_NO")
                        Else
                            CType(UserDetails.FindControl("cbSalesRep"), CheckBox).Checked = False
                            CType(UserDetails.FindControl("txtSalesRep"), TextBox).Text = String.Empty
                            CType(UserDetails.FindControl("txtSalesRep"), TextBox).Enabled = False
                        End If

                    Else
                        Dim defaultMenu As String = ClsDataAccessHelper.GetText(userRow, "U_DEFAULT_MENU", "HOME")
                        Dim ddlDefaultMenu As DropDownList = CType(UserDetails.FindControl("ddlDefaultMenu"), DropDownList)
                        If ddlDefaultMenu.Items.FindByValue(defaultMenu) IsNot Nothing Then
                            ddlDefaultMenu.SelectedValue = defaultMenu
                        End If
                    End If

                    If userRow("U_MarketPlace") IsNot DBNull.Value Then
                        CType(UserDetails.FindControl("IsMarketplace_TextBox"), TextBox).Text = userRow("U_MarketPlace")
                        CType(UserDetails.FindControl("HD_InitialState_Is_Marketplace"), HiddenField).Value = userRow("U_MarketPlace")
                    End If

                    If userRow("U_ISCHIRON") IsNot DBNull.Value Then
                        CType(UserDetails.FindControl("IsChiron_TextBox"), TextBox).Text = userRow("U_ISCHIRON")
                        If Not IsDBNull(userRow("U_ISCHIRON")) Then
                            Boolean.TryParse(userRow("U_ISCHIRON").ToString(), isChironUser)
                        End If
                        Dim ChironRole As Integer = 0
                        If userRow("U_CHIRON_ROLE") IsNot DBNull.Value Then
                            Integer.TryParse(userRow("U_CHIRON_ROLE"), ChironRole)
                        End If
                        CType(UserDetails.FindControl("drpdownChironRole"), DropDownList).SelectedValue = ChironRole
                    End If



                    Dim sopID = CType(UserDetails.FindControl("HD_SOPNAME"), HiddenField).Value
                    'Get chiron brands by country  
                    Dim sdkValue = SmsManagement.GetKeyValue(EnvironmentID, "CHIRON_AvailableBrands", sopID)
                    Dim allowedBrands As String() = sdkValue.Trim.Split(","c) ' Allowed brands from SDK
                    Dim chironBrands As String = userRow("U_CHIRON_BRAND").ToString()
                    Dim brandArray As String() = chironBrands.Split(","c) '

                    ' Track if a brand was assigned
                    Dim aegAssigned As Boolean = False
                    Dim elxAssigned As Boolean = False

                    'To make sure we initialise value within allowedBrands(Problem occurs with past value)
                    'and handling case if data for u_chiron_brand modified by errors with brand that are not allowed
                    For Each allowedBrand As String In allowedBrands
                        ' Check if the brand exists in user-selected brands
                        If brandArray.Contains(allowedBrand) Then
                            Select Case allowedBrand
                                Case "AEG"
                                    CType(UserDetails.FindControl("txtChironBrandAEG"), TextBox).Text = "AEG"
                                    aegAssigned = True
                                Case "ELX"
                                    CType(UserDetails.FindControl("txtChironBrandElectrolux"), TextBox).Text = "ELX"
                                    elxAssigned = True
                            End Select
                        End If
                    Next

                    For Each allowedBrand As String In allowedBrands
                        Dim trimmedBrand As String = allowedBrand.Trim()
                        If Not aegAssigned And trimmedBrand.Equals("AEG") Then
                            If Not brandArray.Contains(trimmedBrand) Then
                                CType(UserDetails.FindControl("txtChironBrandAEG"), TextBox).Text = "NOTSELECTED"
                            Else
                                CType(UserDetails.FindControl("txtChironBrandAEG"), TextBox).Text = ""
                            End If
                        ElseIf Not elxAssigned And trimmedBrand.Equals("ELX") Then
                            If Not brandArray.Contains(trimmedBrand) Then
                                CType(UserDetails.FindControl("txtChironBrandElectrolux"), TextBox).Text = "NOTSELECTED"
                            Else
                                CType(UserDetails.FindControl("txtChironBrandElectrolux"), TextBox).Text = ""
                            End If
                        End If
                    Next

                    'set value by default regaldess what we have in u_chiron_brand 
                    If allowedBrands.Length = 1 And isChironUser Then
                        Select Case allowedBrands(0)
                            Case "AEG"
                                CType(UserDetails.FindControl("txtChironBrandAEG"), TextBox).Text = "AEG"
                            Case "ELX"
                                CType(UserDetails.FindControl("txtChironBrandElectrolux"), TextBox).Text = "ELX"
                        End Select
                        'set value by Notselected to handle none chiron user regaldess what we have in u_chiron_brand 
                    ElseIf allowedBrands.Length = 1 And Not isChironUser Then
                        Select Case allowedBrands(0)
                            Case "AEG"
                                CType(UserDetails.FindControl("txtChironBrandAEG"), TextBox).Text = "NOTSELECTED"
                            Case "ELX"
                                CType(UserDetails.FindControl("txtChironBrandElectrolux"), TextBox).Text = "NOTSELECTED"
                        End Select
                        'when user is chiron and for x reason there is no values in u_chiron_brand we net to select allowed brand on 
                    ElseIf allowedBrands.Length = 2 And isChironUser And String.IsNullOrEmpty(chironBrands) Then
                        CType(UserDetails.FindControl("txtChironBrandAEG"), TextBox).Text = "AEG"
                        CType(UserDetails.FindControl("txtChironBrandElectrolux"), TextBox).Text = "ELX"
                    End If


                    If userRow("U_ISCOMNORM") IsNot DBNull.Value Then
                        CType(UserDetails.FindControl("Is_ComNorm_user"), TextBox).Text = userRow("U_ISCOMNORM")
                        CType(UserDetails.FindControl("Logintext_ComNorm"), TextBox).Text = userRow("U_LOGINCOMNORM")
                        CType(UserDetails.FindControl("Password_ComNorm"), TextBox).Attributes("value") = ClsHelper.Decrypt(userRow("U_PASSWORDCOMNORM"))
                        If (userRow("U_ISCOMNORM")) Then
                            userTags += ClsEbusinessHelper.FormatTag("ComNorm", "#11b941")
                        End If
                    End If

                    CType(UserDetails.FindControl("txtBoxEmailLogin"), TextBox).Text = ClsDataAccessHelper.GetText(userRow, "U_EMAIL_LOGIN")
                    If Not String.IsNullOrEmpty(ClsDataAccessHelper.GetText(userRow, "U_EMAIL_LOGIN")) Then
                        CType(UserDetails.FindControl("btnResetPassword"), Button).Visible = True
                    End If

                    CType(UserDetails.FindControl("HD_InitialLoginEmail"), HiddenField).Value = ClsDataAccessHelper.GetText(userRow, "U_EMAIL_LOGIN")
                    CType(UserDetails.FindControl("HD_ExtUserLocation"), HiddenField).Value = ClsDataAccessHelper.GetText(userRow, "U_EXT_LOCATION")
                    If userRow("U_EXT_ACTIVATION_EMAIL_SENT_ON_CREATION") IsNot DBNull.Value Then
                        CType(UserDetails.FindControl("HD_ExtUserSendActivationEmail"), HiddenField).Value = userRow("U_EXT_ACTIVATION_EMAIL_SENT_ON_CREATION")
                    End If
                    CType(UserDetails.FindControl("txtBoxEmail"), TextBox).Text = ClsDataAccessHelper.GetText(userRow, "varEmail")
                    CType(UserDetails.FindControl("txtBoxLoginName"), TextBox).Text = ClsSciUtil.GuidLoginNameHandler(ClsDataAccessHelper.GetText(userRow, "varUserName"))
                    CType(UserDetails.FindControl("HD_DatabaseLoginName"), HiddenField).Value = ClsDataAccessHelper.GetText(userRow, "varUserName")


                    If (ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.IMPERSONATE_USER)) Then
                        Dim _u_globalId As String = userRow("U_GLOBALID").ToString()
                        Dim _u_gwsId As String = userRow("U_ID").ToString()
                        Dim _cy_siteUrl As String = userRow("CY_MAIN_URL_INTERNET").ToString()
                        CType(UserDetails.FindControl("HD_U_GWSID"), HiddenField).Value = _u_gwsId
                        CType(UserDetails.FindControl("HD_MAIN_URL"), HiddenField).Value = _cy_siteUrl
                    End If

                    If userRow("U_ISADMIN") Then
                        userTags += ClsEbusinessHelper.FormatTag("Administrator", "#5f52cd")
                    End If
                    Dim expirationDate As DateTime = Nothing
                    If userRow("U_EXPIRE") IsNot DBNull.Value Then
                        expirationDate = userRow("U_EXPIRE")
                        Dim maxCalendarDate As Date = CType(UserDetails.FindControl("radDateTimeExpirationDate"), RadDateTimePicker).MaxDate
                        Dim minCalendarDate As Date = CType(UserDetails.FindControl("radDateTimeExpirationDate"), RadDateTimePicker).MinDate
                        If maxCalendarDate > expirationDate Then
                            If expirationDate < minCalendarDate Then
                                CType(UserDetails.FindControl("radDateTimeExpirationDate"), RadDateTimePicker).SelectedDate = minCalendarDate
                            Else
                                CType(UserDetails.FindControl("radDateTimeExpirationDate"), RadDateTimePicker).SelectedDate = expirationDate
                            End If
                        Else
                            CType(UserDetails.FindControl("radDateTimeExpirationDate"), RadDateTimePicker).SelectedDate = maxCalendarDate
                        End If

                    End If
                    If Not (clsUser.Actions.Contains(ClsHelper.ActionDesignation.DISPLAY_MANAGE_CONTACTS_TAB_IN_CUSTOMER_AND_USER_PROFILES)) Then
                        RadTabStrip1.Tabs.FindTabByValue("Contacts").Visible = False
                        RadPageViewContacts.Visible = False
                    Else
                        iframeContacts.Src = String.Format("EbusinessContacts.aspx?HideHeader=true&envid={0}&uid={1}", EnvironmentID.ToString(), userRow("U_GLOBALID").ToString())
                    End If
                Else
                    lblLoginName.Text = ClsDataAccessHelper.GetText(userRow, "U_WUID")
                    displayNameTR.Visible = False
                    emailTR.Visible = False
                    UserDetails.FindControl("emailLoginTR").Visible = False
                    UserDetails.FindControl("loginTR").Visible = False
                    UserDetails.FindControl("emailTR").Visible = False
                    UserDetails.FindControl("displayNameTR").Visible = False
                    UserDetails.FindControl("expirationDateTR").Visible = False
                    imageTR.Attributes.Add("rowspan", "3")
                    RadTabStrip1.Tabs.FindTabByValue("Contacts").Visible = False
                    RadPageViewContacts.Visible = False
                    DisableControls()
                End If
                CType(UserDetails.FindControl("txtBoxWebUserID"), TextBox).Text = ClsDataAccessHelper.GetText(userRow, "U_WUID")
                CType(UserDetails.FindControl("txtPhoneNo"), TextBox).Text = ClsDataAccessHelper.GetText(userRow, "U_PHONE")
                CType(UserDetails.FindControl("ddlManagementType"), DropDownList).SelectedValue = userRow("U_TYPE").ToString()
                CType(UserDetails.FindControl("chkboxAdministrator"), CheckBox).Checked = userRow("U_ISADMIN")
                CType(UserDetails.FindControl("chkboxInternal"), CheckBox).Checked = userRow("U_ISINTERNAL")
                CType(UserDetails.FindControl("radNumericShoppingBasketLineQty"), RadNumericTextBox).Value = CType(userRow("U_MAX_LINE_QTY").ToString(), Double?)
                CType(UserDetails.FindControl("radNumericShoppingBasketMaxLines"), RadNumericTextBox).Value = CType(userRow("U_MAX_LINES").ToString(), Double?)
                If Not (clsUser.Actions.Contains(ClsHelper.ActionDesignation.DISPLAY_USER_SUPER_USER_ACTIVITY_HISTORY_TAB)) Then
                    RadTabStrip1.Tabs.FindTabByValue("ActionsHisotry").Visible = False
                    RadPageViewActivityHistory.Visible = False
                Else
                    Dim superUserQuery As String = IIf(userRow("U_ISSUPERUSER"), "&issuperuser=1", "")
                    iframeActivityLog.Src = String.Format("EbusinessActivityHistory.aspx?HideHeader=true&envid={0}&cid={1}&uid={2}&sopid={3}{4}", EnvironmentID.ToString(), userRow("C_GLOBALID").ToString(), userRow("U_GLOBALID").ToString(), ClsDataAccessHelper.GetText(userRow, "S_SOP_ID"), superUserQuery)
                End If

                Dim lastConnectionDate As DateTime = Nothing
                If userRow("U_LAST_CONNECT") IsNot DBNull.Value Then
                    lastConnectionDate = userRow("U_LAST_CONNECT")
                    lblLastConnectionDate.Text = lastConnectionDate.ToString("dd/MM/yyyy HH:mm")
                Else
                    lblLastConnectionDate.Text = "--"
                End If

                If lastConnectionDate = Nothing Then
                    userTags += ClsEbusinessHelper.FormatTag("Never visited", "#13194e")
                Else
                    If lastConnectionDate > DateTime.Now.AddDays(-1) Then
                        userTags += ClsEbusinessHelper.FormatTag("Visited in last 24h", "#8cd874")
                    ElseIf lastConnectionDate > DateTime.Now.AddDays(-7) Then
                        userTags += ClsEbusinessHelper.FormatTag("Visited this week", "#b91169")
                    ElseIf lastConnectionDate > DateTime.Now.AddDays(-30) Then
                        userTags += ClsEbusinessHelper.FormatTag("Visited this month", "#dd4f42")
                    ElseIf lastConnectionDate.Year = DateTime.Now.Year Then
                        userTags += ClsEbusinessHelper.FormatTag("Visited this year", "#e39800")
                    Else
                        userTags += ClsEbusinessHelper.FormatTag("Old visitor", "#6e6e73")
                    End If
                End If

                If userRow("U_ISSUPERUSER") Then
                    isSuperUser = True
                    CType(UserDetails.FindControl("HD_Mode"), HiddenField).Value = "UpdateSuperUser"
                    Me.btnTransformInSuperuser.Visible = False
                    userTags += ClsEbusinessHelper.FormatTag("Super User", "#5f19cd")
                    Dim superUserCategory As String = ClsDataAccessHelper.GetText(userRow, "SuperUserCategory")
                    Dim color As String = "#795548"
                    Select Case superUserCategory
                        Case "HelpDesk"
                            color = "#a119cd"
                            superUserCategory = "Help Desk"
                        Case "Customer"
                            color = "#009688"
                            superUserCategory = "Customer Category"
                        Case "KAM"
                            color = "#673ab7"
                            superUserCategory = "KAM Category"
                        Case "SalesMan"
                            color = "#00bcd4"
                            superUserCategory = "Sales Man"
                    End Select
                    If Not String.IsNullOrEmpty(superUserCategory) Then
                        userTags += ClsEbusinessHelper.FormatTag(superUserCategory, color)
                    End If
                    CType(UserDetails.FindControl("ddlSuperUserCategory"), DropDownList).Items.Clear()
                    For Each row As DataRow In ClsEbusinessHelper.GetSuperUserCategories(EnvironmentID, Page.Cache).Rows
                        Dim item As ListItem = New ListItem(row("CAT_NAME"), row("CAT_GLOBALID").ToString())
                        If userRow("CAT_GLOBALID") IsNot DBNull.Value Then
                            If userRow("CAT_GLOBALID") = row("CAT_GLOBALID") Then
                                item.Selected = True
                            End If
                        End If
                        CType(UserDetails.FindControl("ddlSuperUserCategory"), DropDownList).Items.Add(item)
                    Next
                    CType(Master.FindControl("title"), HtmlTitle).Text = "Super user profile - " + ClsDataAccessHelper.GetText(userRow, "varName")
                    If userActive AndAlso customerActive AndAlso clsUser.Actions.Contains(ClsHelper.ActionDesignation.DISPLAY_CUSTOMER_LIST_TAB_IN_SUPER_USER_PROFILE) Then
                        iframeCustomerList.Src = String.Format("EbusinessSuperUserCustomerList.aspx?HideHeader=true&envid={0}&uid={1}", EnvironmentID.ToString(), Uid.ToString())
                        RadTabStrip1.Tabs.FindTabByValue("CustomerList").Visible = True
                    End If
                    CustomerCodeLabelNameTD.Visible = False
                    CustomerCodeLabelTD.Visible = False
                    CustomerNameLabelNameTD.Visible = False
                    CustomerNameLabelTD.Visible = False
                Else
                    CType(UserDetails.FindControl("trSuperUserCategory"), HtmlTableRow).Visible = False
                    CType(Master.FindControl("title"), HtmlTitle).Text = "User profile - " + ClsDataAccessHelper.GetText(userRow, "varName")
                    TooltipContentHelp_lblUserID.InnerHtml += "<br/><b>Customer Global ID:</b> " + userRow("C_GLOBALID").ToString() + "<br/><b>SOP ID:</b> " + ClsDataAccessHelper.GetText(userRow, "S_SOP_ID")
                    TooltipContentHelp_lblUserID.InnerHtml += "</br><b>Customer Code:</b> " + ClsDataAccessHelper.GetText(userRow, "C_CUID") + "</br><b>Customer Name:</b> " + ClsDataAccessHelper.GetText(userRow, "C_NAME")
                    lblCustomerCode.Text = ClsDataAccessHelper.GetText(userRow, "C_CUID")
                    lblCustomerName.Text = ClsDataAccessHelper.GetText(userRow, "C_NAME")
                    lblCustomerCode.Attributes.Add("onclick", String.Format("popup('EbusinessCompanyProfile.aspx?envid={0}&cid={1}')", EnvironmentID, userRow("C_GLOBALID").ToString()))
                End If
                'CType(UserDetails.FindControl("txtBoxPassword"), TextBox).CssClass = "Electrolux_light width230px hidden"
                'TODO #userregistration

                UserTagsTD.InnerHtml = userTags
                Dim gwsGroupTypes As DataTable = ClsEbusinessHelper.GetGWSGroupType(EnvironmentID, Page.Cache)
                Dim gwsGroupSelectValue = ""
                If gwsGroupTypes IsNot Nothing Then
                    CType(UserDetails.FindControl("ddlGWSGroup"), DropDownList).Items.Clear()
                    For Each dataRow As DataRow In gwsGroupTypes.Select("S_SOP_ID='" + userRow("S_SOP_ID").ToString() + "'")
                        Dim gwsItem As ListItem = New ListItem(dataRow("varUserName"), dataRow("intUserID").ToString() + "|" + ClsDataAccessHelper.GetText(dataRow, "EnabledGWSRight"))
                        If dataSet.Tables(2).Rows.Count = 1 Then
                            If dataRow("intUserID") = dataSet.Tables(2).Rows(0)("intGroupID") Then
                                gwsItem.Selected = True
                                gwsGroupSelectValue = ClsDataAccessHelper.GetText(dataRow, "EnabledGWSRight")
                            End If
                        End If
                        CType(UserDetails.FindControl("ddlGWSGroup"), DropDownList).Items.Add(gwsItem)
                    Next
                    ClsEbusinessHelper.SetValueCheckBoxList(CType(UserDetails.FindControl("chkboxListGWSGroup"), CheckBoxList), gwsGroupSelectValue)
                End If

                Dim sopOrderTypes As DataTable = ClsEbusinessHelper.Get_SOP_OrderTypes(EnvironmentID, String.Empty, Page.Cache)
                Dim userOrderTypes As DataTable = ClsEbusinessHelper.Get_USER_OrderTypes(EnvironmentID, userRow("U_GLOBALID").ToString())
                If sopOrderTypes IsNot Nothing Then
                    CType(UserDetails.FindControl("chkboxListOrderTypes"), CheckBoxList).Items.Clear()
                    For Each dataRow As DataRow In sopOrderTypes.Select("S_SOP_ID='" + userRow("S_SOP_ID").ToString() + "'")
                        Dim orderTypeItem As ListItem = New ListItem(dataRow("OT_NAME"), dataRow("ID_OT_SOP").ToString())
                        If userOrderTypes.Select("ID_OT_SOP='" + dataRow("ID_OT_SOP").ToString() + "'").Any() Then
                            orderTypeItem.Selected = True
                        End If
                        CType(UserDetails.FindControl("chkboxListOrderTypes"), CheckBoxList).Items.Add(orderTypeItem)
                    Next

                End If
            End If

            If dataSet.Tables(1).Rows.Count > 0 Then
                CType(UserDetails.FindControl("uerRightsChbkoxList"), CheckBoxList).Items.Clear()
                Dim regularItems As List(Of ListItem) = New List(Of ListItem)
                Dim suItems As List(Of ListItem) = New List(Of ListItem)
                For Each row As DataRow In dataSet.Tables(1).Rows
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
                If (isSuperUser = True) Then
                    CType(UserDetails.FindControl("uerRightsChbkoxList"), CheckBoxList).Items.AddRange(suItems.ToArray())
                End If
            End If
        End If
        ClsHelper.Log("Display " + IIf(isSuperUser, "Super ", "") + "User Profile", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
    End Sub

    Protected Sub DisableControls()
        CType(UserDetails.FindControl("uerRightsChbkoxList"), CheckBoxList).Enabled = False
        CType(UserDetails.FindControl("txtBoxWebUserID"), TextBox).Enabled = False
        CType(UserDetails.FindControl("txtPhoneNo"), TextBox).Enabled = False
        CType(UserDetails.FindControl("chkboxAdministrator"), CheckBox).Enabled = False
        CType(UserDetails.FindControl("chkboxInternal"), CheckBox).Enabled = False
        CType(UserDetails.FindControl("cbSalesRep"), CheckBox).Enabled = False
        CType(UserDetails.FindControl("txtSalesRep"), TextBox).Enabled = False
        CType(UserDetails.FindControl("radNumericShoppingBasketLineQty"), RadNumericTextBox).Enabled = False
        CType(UserDetails.FindControl("ddlManagementType"), DropDownList).Enabled = False
        CType(UserDetails.FindControl("ddlSuperUserCategory"), DropDownList).Enabled = False
        CType(UserDetails.FindControl("ddlGWSGroup"), DropDownList).Enabled = False
        CType(UserDetails.FindControl("ddlDefaultMenu"), DropDownList).Enabled = False
        CType(UserDetails.FindControl("radNumericShoppingBasketMaxLines"), RadNumericTextBox).Enabled = False
        CType(UserDetails.FindControl("txtBoxEmail"), TextBox).Enabled = False
        CType(UserDetails.FindControl("txtBoxLoginName"), TextBox).Enabled = False
        CType(UserDetails.FindControl("radDateTimeExpirationDate"), RadDateTimePicker).Enabled = False
        CType(UserDetails.FindControl("txtBoxDisplayName"), TextBox).Enabled = False
        UserDetails.FindControl("ButtonsTR").Visible = False
    End Sub


    Protected Sub TransformUserInSuperuser(EnvID As String, SOPNAME As String, U_GLOBALID As String)


        Dim stopWatch As Stopwatch = Stopwatch.StartNew()
        Dim errorMsg As String = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        Dim runWithSuccess As Boolean = False
        If logonUser IsNot Nothing Then
            Try
                parameters.Add(New SqlParameter("@EnvironmentID", EnvID))
                parameters.Add(New SqlParameter("@U_GLOBALID", U_GLOBALID))

                runWithSuccess = ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[UsrMgmt_TransformUserToSuperuser]", parameters)
                If runWithSuccess Then
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "CloseWindow", "CloseWindow('Submit');", True)
                Else
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Unable to Transform user!');", True)
                    errorMsg = "An unexpected error has occurred"
                End If
            Catch ex As Exception
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Unable to Transform!');", True)
                Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
                If Not ex.Message Is Nothing Then
                    exceptionMessage = ex.Message
                End If
                If Not ex.StackTrace Is Nothing Then
                    exceptionStackTrace = ex.StackTrace
                End If
                errorMsg = String.Format("<b>Method Name:</b>TransformUserInSuperuser</br><b>Exception Message:</b></br>{0}</br>" _
                            + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                            , exceptionStackTrace)
            End Try
            ClsHelper.Log("Transform User into SUperuser", logonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), stopWatch.ElapsedMilliseconds, Not runWithSuccess, errorMsg)
            ClsHelper.LogEbusinessAction(CInt(EnvID.ToString()), SOPNAME, "Transform User into SUperuser", logonUser.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), stopWatch.ElapsedMilliseconds, Not runWithSuccess, errorMsg)
        End If
    End Sub

End Class

