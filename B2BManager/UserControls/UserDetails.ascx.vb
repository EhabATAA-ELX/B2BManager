
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.IO
Imports System.Net
Imports System.ServiceModel.Web
Imports System.Web.Services
Imports B2BExtentions

Partial Class UserControls_UserDetails
    Inherits System.Web.UI.UserControl

    Private passwordString As String = String.Empty
    Public _Mode As String
    Public Property Mode As String
        Get
            Return _Mode
        End Get
        Set(ByVal value As String)
            _Mode = value
        End Set
    End Property
    Public ReadOnly Property Setup_Type As String
        Get
            Dim isChiron As Boolean = False
            If Not String.IsNullOrEmpty(IsChiron_TextBox.Text) Then
                Try
                    isChiron = Convert.ToBoolean(IsChiron_TextBox.Text)
                Catch
                End Try
            End If
            Return ClsUsersManagementHelper.GetSetupType(isChiron)
        End Get
    End Property

    Public ReadOnly Property LoginNameRequiredForTPInCurrentCountry As String
        Get
            Dim returnVal As Boolean = True
            Dim setting As String = ConfigurationManager.AppSettings.Item("MigratedToNewTradeplaceIDSystem")
            If Not String.IsNullOrEmpty(setting) Then
                If setting.Contains(HD_SOPNAME.Value) Then
                    returnVal = False
                End If
            End If
            Return returnVal.ToString().ToLower()
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not String.IsNullOrEmpty(HD_Mode.Value) Then
            Me.Mode = HD_Mode.Value
        End If
        If Not IsPostBack Then

            btnSetMissingLoginEmail.Visible = False

            If Me.Mode = "CreateUser" Or Me.Mode = "CreateSuperUser" Then
                radDateTimeExpirationDate.SelectedDate = DateTime.Today.AddYears(2)
                externalAccountTR.Visible = False
                Dim sopID = CType(FindControl("HD_SOPNAME"), HiddenField).Value
                Dim environmentID = CType(FindControl("HD_EnvironmentID"), HiddenField).Value
                'Get chiron brands by country  
                Dim sdkValue = SmsManagement.GetKeyValue(environmentID, "CHIRON_AvailableBrands", sopID)
                Dim allowedBrands As String() = sdkValue.Trim.Split(","c) ' Allowed brands from SDK

                'Need to set only allowed brands to NOTSELECTED 
                For Each allowedBrand As String In allowedBrands
                    Select Case allowedBrand
                        Case "AEG"
                            HD_ChironBrandAEG.Value = "NOTSELECTED"
                            txtChironBrandAEG.Text = "NOTSELECTED"
                        Case "ELX"
                            HD_ChironBrandELX.Value = "NOTSELECTED"
                            txtChironBrandElectrolux.Text = "NOTSELECTED"
                    End Select
                Next

            Else
                confirmEmailLoginTR.Visible = False
                sendActivationEmailTR.Visible = False

                txtBoxEmailLogin.Enabled = False
                rev_txtBoxEmailLogin.Enabled = False

                If (String.IsNullOrEmpty(txtBoxEmailLogin.Text)) Then
                    btnSetMissingLoginEmail.Visible = True
                End If
                externalAccountTR.Visible = True
            End If

            If Me.Mode.ToLower().Contains("superuser") Then
                trSalesRep.Visible = True
            Else
                trSalesRep.Visible = False
            End If

            If (ClsSessionHelper.LogonUser IsNot Nothing AndAlso ClsSessionHelper.LogonUser.Actions IsNot Nothing) Then
                If (ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.IMPERSONATE_USER)) Then
                    If IsChiron_TextBox.Text.ToLower().Contains("false") Then ' no impersonaion for Chiron users
                        Me.btnImpersonate.Visible = True
                    End If
                End If
                If (ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.UPDATE_COMNORM_USER_INFO)) Then
                    Me.DivComNorm.Visible = True
                End If
            End If
        End If

    End Sub

    Protected Sub btnImpersonate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnImpersonate.Click
        Dim stopWatch As Stopwatch = Stopwatch.StartNew()

        Dim _u_emailLogin As String = Me.txtBoxEmailLogin.Text
        Dim _u_globalId As String = Me.HD_U_GlobalID.Value
        Dim _u_gwsId As String = Me.HD_U_GWSID.Value
        Dim _cy_siteUrl As String = Me.HD_MAIN_URL.Value

        Dim key As String = ConfigurationManager.AppSettings("B2BCrypteKey").ToString()

        Dim token As String = _u_gwsId + "$" + _u_globalId + "$" + DateTime.UtcNow.ToString("O")
        Dim encryptedToken As String = Server.UrlEncode(token.EncryptString(key).Replace("+", "%2b"))

        Dim url As String = _cy_siteUrl + "Auth/CompleteLogon.aspx?B2BManager=" + encryptedToken

        Dim s As String = "window.open('" & url & "', '_blank');"
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "alertscript", s, True)

        Dim actionDetails As String = "Impersonate userId : " + _u_gwsId + " # userGlobalId: " + _u_globalId + " # url: " + url + " # emailLogin: " + _u_emailLogin
        ClsHelper.Log("Impersonate User", ClsSessionHelper.LogonUser.GlobalID.ToString(), actionDetails, stopWatch.ElapsedMilliseconds, False, Nothing)
        ClsHelper.LogEbusinessAction(CInt(HD_EnvironmentID.Value), HD_SOPNAME.Value, "Impersonate User", ClsSessionHelper.LogonUser.GlobalID, actionDetails, stopWatch.ElapsedMilliseconds, False, Nothing)

    End Sub

    Protected Sub btnSetMissingLoginEmail_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSetMissingLoginEmail.Click
        txtBoxEmailLogin.Enabled = True
        confirmEmailLoginTR.Visible = True
        vldEmail.Enabled = True
        rev_txtBoxEmailLogin.Enabled = True
        txtBoxEmailLogin.Attributes.Add("placeholder", "Please insert the login email here")
        txtBoxConfirmLoginEmail.Attributes.Add("placeholder", "Confirm the login email")
    End Sub

    Protected Sub btnResetPassword_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnResetPassword.Click
        Dim msg As String = ""
        Dim runWithSuccess As Boolean = ClsSciUtil.SendPasswordResetEmail(HD_EnvironmentID.Value, HD_SOPNAME.Value, Setup_Type, txtBoxEmailLogin.Text, msg)
        If runWithSuccess Then
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ShowInfo", "alert('" + msg + "');", True)
        Else
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('" + msg + "');", True)
        End If
    End Sub

    Protected Sub ddlGWSGroup_SelectedIndexChanged(sender As Object, e As EventArgs)
        ClsEbusinessHelper.SetValueCheckBoxList(chkboxListGWSGroup, ddlGWSGroup.SelectedValue)
    End Sub


    Private Function ExternalAccountHandler(Is_Marketplace As Boolean,
                                            Is_Chiron As Boolean,
                                            Send_Activation_Email As Boolean,
                                            LoginEmail As String,
                                            LoginNam As String,
                                            FirstName As String,
                                            LastName As String,
                                            Country As String,
                                            Language As String,
                                            ByRef userLocation As String,
                                            ByRef uuid As String,
                                            ByRef statusCode As HttpStatusCode,
                                            ByRef returnMessage As String) As Boolean

        Dim setup_type = ClsUsersManagementHelper.GetSetupType(Is_Chiron)

        Dim Email_Lookup_SCIM_Response As IdDS_SCIM_Response
        Email_Lookup_SCIM_Response = ClsSciUtil.IdDS_SCIM_EmailLookup(HD_EnvironmentID.Value, HD_SOPNAME.Value, setup_type, LoginEmail)

        Dim processedLoginName As String = LoginNam
        If String.Equals(ClsSciUtil.GuidLoginNameHandler(LoginNam), ClsSciUtil.DisplayTextForGuidLoginName) Then
            processedLoginName = String.Empty
        End If

        If Not String.IsNullOrEmpty(processedLoginName) Then
            'start routine for LoginName value present on B2B Manager form 
            Dim Username_Lookup_SCIM_Response As IdDS_SCIM_Response
            Username_Lookup_SCIM_Response = ClsSciUtil.IdDS_SCIM_UsernameLookup(HD_EnvironmentID.Value, HD_SOPNAME.Value, setup_type, processedLoginName)


            If Email_Lookup_SCIM_Response.totalResults = 0 Then
                If Username_Lookup_SCIM_Response.totalResults = 0 Then
                    Dim createExternalUserResult As Boolean = False
                    createExternalUserResult = ClsSciUtil.CreateRemoteUser(HD_EnvironmentID.Value,
                                                        LoginEmail, processedLoginName, FirstName, LastName, Country, Language,
                                                        Is_Marketplace, Is_Chiron, HD_SOPNAME.Value, setup_type,
                                                        Send_Activation_Email,
                                                        userLocation,
                                                        statusCode,
                                                        returnMessage
                                                        )
                    If createExternalUserResult = True Then
                        Dim userInfo As SciUserInformation = ClsSciUtil.SPUserInformation(HD_EnvironmentID.Value, HD_SOPNAME.Value, setup_type, userLocation)
                        uuid = userInfo.uuid
                    End If
                    Return createExternalUserResult
                Else
                    returnMessage = "There is no SCI account for EmailLogin: " + LoginEmail +
                                    " but the LoginName that you filled in is already taken. "
                    If Not Is_Marketplace Then
                        returnMessage = returnMessage + " You can leave the LoginName empty or "
                    End If
                    returnMessage = returnMessage + " You will have to choose something else as the LoginName ! " +
                                    "Details of the taken LoginName SCI account: " +
                                    FormatUserDetails(Username_Lookup_SCIM_Response)
                    ''''txtBoxLoginName.Enabled = True
                    ''''rev_txtBoxLoginName.Enabled = True
                    Return False
                End If
            Else
                uuid = Email_Lookup_SCIM_Response.resources(0).id
                Dim usernameOfExisitingSCIAccountForEmailLogin As String = Email_Lookup_SCIM_Response.resources(0).userName

                If String.IsNullOrEmpty(usernameOfExisitingSCIAccountForEmailLogin) Then
                    '~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    ' SCI Record for Login email with no loginName set on SCI
                    '~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    If Username_Lookup_SCIM_Response.totalResults > 0 Then
                        'LoginName conflict
                        '~~~~~~~~~~~~~~~~~~
                        returnMessage = "Email " + LoginEmail + " is already in use on SAP Cloud Identity and the LoginName for this record on SCi is empty."
                        If Not Is_Marketplace Then
                            returnMessage = returnMessage + " You can leave the LoginName empty or "
                        End If
                        returnMessage = returnMessage + " You need to change  the LoginName ! " +
                                    "Details of the taken LoginName SCI account: " +
                                    FormatUserDetails(Username_Lookup_SCIM_Response)

                        ''''txtBoxLoginName.Enabled = True
                        ''''rev_txtBoxLoginName.Enabled = True
                        Return False
                    Else
                        'No conflict on loginName
                        'Will set the screen LoginName on the SCI account
                        '~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                        Dim userInformations As SciUserInformation = Nothing
                        Dim userUri As String = ClsSciUtil.SPUserIDRetrival(HD_EnvironmentID.Value, HD_SOPNAME.Value, setup_type, LoginEmail)
                        If Not String.IsNullOrEmpty(userUri) Then
                            'LoginEmail associated 
                            Return ClsSciUtil.UpdateRemoteUser(HD_EnvironmentID.Value,
                                                                processedLoginName, Language,
                                                                Is_Marketplace, Is_Chiron, HD_SOPNAME.Value, setup_type,
                                                                userUri,
                                                                statusCode,
                                                                returnMessage)
                        Else
                            'We use the SCI Application specific credentials and the createUser method to just to add the current B2B app to SCI account's applications list
                            ' should not alter any data
                            Return ClsSciUtil.CreateRemoteUser(HD_EnvironmentID.Value,
                                                                LoginEmail, processedLoginName, "", "", "", "",
                                                                Is_Marketplace, Is_Chiron, HD_SOPNAME.Value, setup_type,
                                                                Send_Activation_Email,
                                                                userLocation,
                                                                statusCode,
                                                                returnMessage
                                                                )
                        End If



                    End If
                Else
                    If (processedLoginName = usernameOfExisitingSCIAccountForEmailLogin) Then

                        Dim userInformations As SciUserInformation = Nothing
                        Dim userUri As String = ClsSciUtil.SPUserIDRetrival(HD_EnvironmentID.Value, HD_SOPNAME.Value, setup_type, LoginEmail)
                        If Not String.IsNullOrEmpty(userUri) Then
                            'LoginEmail associated 
                            Return ClsSciUtil.UpdateRemoteUser(HD_EnvironmentID.Value,
                                                                String.Empty, Language,
                                                                Is_Marketplace, Is_Chiron, HD_SOPNAME.Value, setup_type,
                                                                userUri,
                                                                statusCode,
                                                                returnMessage)
                        Else
                            'We use the SCI Application specific credentials and the createUser method to just to add the current B2B app to SCI account's applications list
                            ' should not alter any data
                            Return ClsSciUtil.CreateRemoteUser(HD_EnvironmentID.Value,
                                                                LoginEmail, processedLoginName, "", "", "", "",
                                                                Is_Marketplace, Is_Chiron, HD_SOPNAME.Value, setup_type,
                                                                Send_Activation_Email,
                                                                userLocation,
                                                                statusCode,
                                                                returnMessage
                                                                )
                        End If
                    Else
                        returnMessage = "Email " + LoginEmail + " is already in use on SAP Cloud Identity. To update this record "
                        If Not Is_Marketplace Then
                            returnMessage = returnMessage + " you can leave the LoginName empty or "
                        End If

                        returnMessage = returnMessage + " you will have to change the Login Name to match the existing SCi value" +
                                         " Here are the details of the existing  SCI account : " +
                                         FormatUserDetails(Email_Lookup_SCIM_Response)

                        ''''txtBoxLoginName.Enabled = True
                        ''''rev_txtBoxLoginName.Enabled = True
                        Return False

                    End If
                End If
            End If
            'end rutine for LoginName value present on B2B Manager form
        Else
            'start routine for NO LoginName value set on screen
            If Email_Lookup_SCIM_Response.totalResults = 0 Then
                Dim createExternalUserResult As Boolean = False
                createExternalUserResult = ClsSciUtil.CreateRemoteUser(HD_EnvironmentID.Value,
                                                        LoginEmail, processedLoginName, FirstName, LastName, Country, Language,
                                                        Is_Marketplace, Is_Chiron, HD_SOPNAME.Value, setup_type,
                                                        Send_Activation_Email,
                                                        userLocation,
                                                        statusCode,
                                                        returnMessage
                                                        )
                If createExternalUserResult = True Then
                    Dim userInfo As SciUserInformation = ClsSciUtil.SPUserInformation(HD_EnvironmentID.Value, HD_SOPNAME.Value, setup_type, userLocation)
                    uuid = userInfo.uuid
                End If
                Return createExternalUserResult
            Else
                Dim userInformations As SciUserInformation = Nothing
                Dim userUri As String = ClsSciUtil.SPUserIDRetrival(HD_EnvironmentID.Value, HD_SOPNAME.Value, setup_type, LoginEmail)
                If Not String.IsNullOrEmpty(userUri) Then
                    'LoginEmail associated 
                    Return ClsSciUtil.UpdateRemoteUser(HD_EnvironmentID.Value,
                                                                processedLoginName, Language,
                                                                Is_Marketplace, Is_Chiron, HD_SOPNAME.Value, setup_type,
                                                                userUri,
                                                                statusCode,
                                                                returnMessage)
                Else
                    'We use the SCI Application specific credentials and the createUser method to just to add the current B2B app to SCI account's applications list
                    ' should not alter any data
                    Return ClsSciUtil.CreateRemoteUser(HD_EnvironmentID.Value,
                                                                LoginEmail, processedLoginName, "", "", "", "",
                                                                Is_Marketplace, Is_Chiron, HD_SOPNAME.Value, setup_type,
                                                                Send_Activation_Email,
                                                                userLocation,
                                                                statusCode,
                                                                returnMessage
                                                                )
                End If

            End If
            'end routine for NO LoginName value
        End If


    End Function


    Private Function FormatUserDetails(SCIM_Response As IdDS_SCIM_Response) As String
        If SCIM_Response.resources.Count = 0 Then
            Return String.Empty
        End If
        If SCIM_Response.resources(0).ieft_params_extension_sap_User.emails.Count = 0 Then
            Return String.Empty
        End If
        ' "SCIUser UUID : {3}" + Environment.NewLine ,  ====>  SCIM_Response.resources(0).id
        Dim resp As String
        resp = String.Format("SCIUser ID : {0} | SCIUser Email : {1} | SCIUser Login/Username : {2}",
                              SCIM_Response.resources(0).ieft_params_extension_sap_User.userId,
                              SCIM_Response.resources(0).ieft_params_extension_sap_User.emails(0).value,
                              SCIM_Response.resources(0).userName)

        Return resp
    End Function


    Protected Sub btnSubmitForm_Click(ByVal sender As Object, ByVal e As EventArgs)
        If Page.IsValid Then
            Dim isChiron As Boolean = False
            Dim ChironRole As Integer = 0
            If Not String.IsNullOrEmpty(IsChiron_TextBox.Text) Then
                Try
                    isChiron = Convert.ToBoolean(IsChiron_TextBox.Text)
                Catch
                End Try
            End If
            If Not String.IsNullOrEmpty(drpdownChironRole.SelectedValue) Then
                Try
                    ChironRole = Convert.ToInt32(drpdownChironRole.SelectedValue)
                Catch
                End Try
            End If
            If isChiron = True AndAlso Not ChironRole > 0 Then
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup(' You must select a Chiron Role!');", True)
                Return
            End If


            Dim selectedValues As List(Of String) = uerRightsChbkoxList.Items.Cast(Of ListItem)().Where(Function(li) li.Selected).[Select](Function(li) li.Value).ToList()
            If isChiron = True Then
                If ChironRole > 0 Then
                    selectedValues = ClsUsersManagementHelper.GetRightsForChironRoleId(ChironRole)
                End If
            End If

            Dim RightList As String = String.Join(";", selectedValues)
            Dim isMarketplace As Boolean = False
            Dim isComNorm As Boolean = False

            Dim selectedOrderTypes As List(Of String) = chkboxListOrderTypes.Items.Cast(Of ListItem)().Where(Function(li) li.Selected).[Select](Function(li) li.Value).ToList()
            Dim orderTypeList As String = String.Join(";", selectedOrderTypes)


            If Not String.IsNullOrEmpty(IsMarketplace_TextBox.Text) Then
                Try
                    isMarketplace = Convert.ToBoolean(IsMarketplace_TextBox.Text)
                Catch
                End Try
            End If
            If Not String.IsNullOrEmpty(Is_ComNorm_user.Text) Then
                Try
                    isComNorm = Convert.ToBoolean(Is_ComNorm_user.Text)
                Catch
                End Try
            End If

            Dim userLocation As String = String.Empty
            Dim uuid As String = String.Empty
            Dim statusCode As HttpStatusCode
            Dim returnMessage As String = String.Empty
            Dim Language As String = GetDefaultSopLanguage()
            Dim chironBrands As String = "" 'Valeur par default
            If isChiron Then
                chironBrands = GetChironBrands()
                If String.IsNullOrEmpty(chironBrands) Then
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "CLRCHK", "ClearChironRolesDrp();", True)
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup(' You must select at least one brand !');", True)
                    Return
                End If
            End If

            If Me.Mode = "CreateUser" Or Me.Mode = "CreateSuperUser" Then
                If CheckUnicityInLocalDatabase(txtBoxEmailLogin.Text, txtBoxLoginName.Text) Then

                    Dim externalSystemCallResult As Boolean = True

                    externalSystemCallResult = ExternalAccountHandler(isMarketplace, isChiron, ckhSendActivationEmail.Checked,
                                                               txtBoxEmailLogin.Text,
                                                               "",
                                                                "", txtBoxDisplayName.Text, HD_SOPNAME.Value, Language,
                                                                userLocation, uuid, statusCode, returnMessage)

                    If externalSystemCallResult Then
                        Dim userUserInfos As New UserInfos() With {
                                .CompanyID = HD_C_GlobalID.Value,
                                .EmailLogin = txtBoxEmailLogin.Text,
                                .DisplayName = txtBoxDisplayName.Text,
                                .UserName = txtBoxLoginName.Text,
                                .Password = passwordString,
                                .Email = txtBoxEmail.Text,
                                .WUId = txtBoxWebUserID.Text,
                                .PhoneNo = txtPhoneNo.Text,
                                .DateExpiration = radDateTimeExpirationDate.SelectedDate,
                                .MaxLineQty = If(radNumericShoppingBasketLineQty.Text = "", 999, CInt(radNumericShoppingBasketLineQty.Text)),
                                .MaxLines = If(radNumericShoppingBasketMaxLines.Text = "", 999, CInt(radNumericShoppingBasketMaxLines.Text)),
                                .IsAdmin = chkboxAdministrator.Checked,
                                .IsInternal = chkboxInternal.Checked,
                                .GWSGroupID = CInt(ddlGWSGroup.SelectedValue.Split("|")(0)),
                                .DefaultMenu = ddlDefaultMenu.SelectedValue,
                                .TypeUser = CInt(ddlManagementType.SelectedValue),
                                .RightList = RightList,
                                .ActivationEmailSent = ckhSendActivationEmail.Checked,
                                .SCI_UUID = uuid,
                                .ExternalUserUri = userLocation,
                                .SuSalesRepNo = txtSalesRep.Text,
                                .ChironBrands = chironBrands,
                                .IsMarketplace = isMarketplace,
                                .IsChiron = isChiron,
                                .ChironRole = ChironRole,
                                .OrderTypeList = orderTypeList
                            }

                        SetNewUser(userUserInfos)
                        SCI_Groups_Handler(isMarketplace)
                        CustomerRangeAPi_Call_Handler(Language)
                    Else
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('" + returnMessage + " ');", True)
                    End If

                End If
            Else
                If CheckUnicityInLocalDatabase(txtBoxEmailLogin.Text, txtBoxLoginName.Text, HD_U_GlobalID.Value) Then
                    Dim externalSystemCallResult As Boolean = True

                    If String.IsNullOrEmpty(HD_InitialLoginEmail.Value) Then

                        ''''Dim loginNamePlaceholder As String = HD_DatabaseLoginName.Value
                        ''''If Not String.Equals(ClsSciUtil.GuidLoginNameHandler(loginNamePlaceholder), ClsSciUtil.DisplayTextForGuidLoginName) Then
                        ''''    loginNamePlaceholder = txtBoxLoginName.Text
                        ''''End If
                        ''''externalSystemCallResult = ExternalAccountHandler(isMarketplace, isChiron, True,
                        ''''                                       txtBoxEmailLogin.Text,
                        ''''                                       loginNamePlaceholder,
                        ''''                                       "", txtBoxDisplayName.Text, HD_SOPNAME.Value, Language,
                        ''''                                        userLocation, uuid, statusCode, returnMessage)
                        externalSystemCallResult = ExternalAccountHandler(isMarketplace, isChiron, True,
                                                               txtBoxEmailLogin.Text,
                                                               "",
                                                               "", txtBoxDisplayName.Text, HD_SOPNAME.Value, Language,
                                                                userLocation, uuid, statusCode, returnMessage)
                    End If

                    Dim loginNameVariable As String = HD_DatabaseLoginName.Value
                    If Not String.Equals(txtBoxLoginName.Text, ClsSciUtil.DisplayTextForGuidLoginName) Then
                        loginNameVariable = txtBoxLoginName.Text
                    End If

                    If externalSystemCallResult Then
                        Dim userInfos As New UserInfos() With {
                        .EmailLogin = txtBoxEmailLogin.Text,
                        .DisplayName = txtBoxDisplayName.Text,
                        .UserName = loginNameVariable,
                        .Password = passwordString,
                        .Email = txtBoxEmail.Text,
                        .WUId = txtBoxWebUserID.Text,
                        .PhoneNo = txtPhoneNo.Text,
                        .DateExpiration = radDateTimeExpirationDate.SelectedDate,
                        .MaxLineQty = If(radNumericShoppingBasketLineQty.Text = "", 999, CInt(radNumericShoppingBasketLineQty.Text)),
                        .MaxLines = If(radNumericShoppingBasketMaxLines.Text = "", 999, CInt(radNumericShoppingBasketMaxLines.Text)),
                        .IsAdmin = chkboxAdministrator.Checked,
                        .IsInternal = chkboxInternal.Checked,
                        .GWSGroupID = CInt(ddlGWSGroup.SelectedValue.Split("|")(0)),
                        .DefaultMenu = ddlDefaultMenu.SelectedValue,
                        .TypeUser = CInt(ddlManagementType.SelectedValue),
                        .RightList = RightList,
                        .SCI_UUID = uuid,
                        .SuSalesRepNo = txtSalesRep.Text,
                        .ChironBrands = chironBrands,
                        .IsMarketplace = isMarketplace,
                        .IsComNorm = isComNorm,
                        .LoginComNorm = Logintext_ComNorm.Text,
                        .PasswordComNorm = Password_ComNorm.Text,
                        .IsChiron = isChiron,
                        .ChironRole = ChironRole,
                        .OrderTypeList = orderTypeList
                    }


                        SetCurrentUser(userInfos)
                        SCI_Groups_Handler(isMarketplace)
                        CustomerRangeAPi_Call_Handler(Language)
                    Else
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('" + returnMessage + " ');", True)
                    End If
                End If
            End If
        End If    'EndIf  Page.IsValid
    End Sub

    Protected Function GetChironBrands() As String
        Dim brandList As New List(Of String)

        If Not String.IsNullOrEmpty(txtChironBrandAEG.Text) AndAlso Not txtChironBrandAEG.Text.Equals("NOTSELECTED") Then
            brandList.Add(txtChironBrandAEG.Text)
        End If

        If Not String.IsNullOrEmpty(txtChironBrandElectrolux.Text) AndAlso Not txtChironBrandElectrolux.Text.Equals("NOTSELECTED") Then
            brandList.Add(txtChironBrandElectrolux.Text)
        End If
        Dim chironBrands As String = String.Join(",", brandList)
        Return chironBrands
    End Function

    Protected Sub SetChironBrands(ByVal brandList As List(Of String))

        txtChironBrandAEG.Text = ""
        txtChironBrandElectrolux.Text = ""

        For Each brand As String In brandList
            Select Case brand.Trim().ToUpper()
                Case "AEG"
                    txtChironBrandAEG.Text = "AEG"
                Case "ELX"
                    txtChironBrandElectrolux.Text = "ELX"
            End Select
        Next
    End Sub



    Protected Sub SCI_Groups_Handler(ByRef isMarketplace As Boolean)
        Dim InitialState_isMarketplace As Boolean = False
        If Not String.IsNullOrEmpty(HD_InitialState_Is_Marketplace.Value) Then
            Try
                InitialState_isMarketplace = Convert.ToBoolean(HD_InitialState_Is_Marketplace.Value)
            Catch
            End Try
        End If

        If isMarketplace <> InitialState_isMarketplace Then
            Dim SCIM_Lookup_Response As IdDS_SCIM_Response = ClsSciUtil.IdDS_SCIM_EmailLookup(HD_EnvironmentID.Value, HD_SOPNAME.Value, Setup_Type, txtBoxEmailLogin.Text)
            Dim userId As String = String.Empty
            If SCIM_Lookup_Response IsNot Nothing AndAlso
                    SCIM_Lookup_Response.resources IsNot Nothing AndAlso
                    SCIM_Lookup_Response.resources(0) IsNot Nothing Then
                userId = SCIM_Lookup_Response.resources(0).id
                If userId IsNot String.Empty Then
                    Dim Operation As String = IIf(isMarketplace, "add", "remove")
                    ClsSciUtil.IdDS_SCIM_Group_Operation(HD_EnvironmentID.Value, HD_SOPNAME.Value, Setup_Type, userId, Operation)
                End If
            End If
        End If
    End Sub

    Protected Sub CustomerRangeAPi_Call_Handler(ByRef Language As String)
        ClsAPICustomerRange.RequestRange(HD_EnvironmentID.Value, HD_CustomerCode.Value, HD_SOPNAME.Value, txtBoxEmailLogin.Text, Language)
    End Sub

    Protected Sub SetNewUser(ByVal userInfos As UserInfos)
        Dim stopWatch As Stopwatch = Stopwatch.StartNew()
        Dim errorMsg As String = Nothing
        Dim parameters As New List(Of SqlParameter)()
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        Dim runWithSuccess As Boolean = False

        If logonUser Is Nothing Then Exit Sub

        Try
            ' Set email if empty
            If String.IsNullOrEmpty(userInfos.Email) Then
                userInfos.Email = userInfos.EmailLogin
            End If

            ' Prepare SQL Parameters
            parameters.Add(New SqlParameter("@EnvironmentID", HD_EnvironmentID.Value))
            parameters.Add(New SqlParameter("@C_GLOBALID", New Guid(userInfos.CompanyID)))
            parameters.Add(New SqlParameter("@EMAILLOGIN", userInfos.EmailLogin))
            parameters.Add(New SqlParameter("@UserName", userInfos.DisplayName))
            parameters.Add(New SqlParameter("@Login", userInfos.UserName))
            parameters.Add(New SqlParameter("@Password", userInfos.Password))
            parameters.Add(New SqlParameter("@Email", userInfos.Email))
            parameters.Add(New SqlParameter("@U_PHONE", userInfos.PhoneNo))
            parameters.Add(New SqlParameter("@WUID", userInfos.WUId))
            parameters.Add(New SqlParameter("@U_EXPIRE", userInfos.DateExpiration))
            parameters.Add(New SqlParameter("@U_MAX_LINE_QTY", userInfos.MaxLineQty))
            parameters.Add(New SqlParameter("@U_MAX_LINES", userInfos.MaxLines))
            parameters.Add(New SqlParameter("@U_ISADMIN", If(userInfos.IsAdmin, 1, 0)))
            parameters.Add(New SqlParameter("@U_ISINTERNAL", If(userInfos.IsInternal, 1, 0)))
            parameters.Add(New SqlParameter("@GWSGroupID", userInfos.GWSGroupID))
            parameters.Add(New SqlParameter("@U_DEFAULT_MENU", userInfos.DefaultMenu))
            parameters.Add(New SqlParameter("@U_TYPE", userInfos.TypeUser))
            parameters.Add(New SqlParameter("@defaultRightList", userInfos.RightList))
            parameters.Add(New SqlParameter("@U_EXT_ACTIVATION_EMAIL_SENT", userInfos.ActivationEmailSent))
            parameters.Add(New SqlParameter("@U_EXT_ID", If(String.IsNullOrEmpty(userInfos.SCI_UUID), DBNull.Value, userInfos.SCI_UUID)))
            parameters.Add(New SqlParameter("@U_EXT_LOCATION", userInfos.ExternalUserUri))

            ' Super User Check
            If Me.Mode = "CreateSuperUser" Then
                parameters.Add(New SqlParameter("@U_CAT_GLOBALID", ddlSuperUserCategory.SelectedValue))
                parameters.Add(New SqlParameter("@U_ISSUPERUSER", True))
            End If

            ' Optional Fields
            parameters.Add(New SqlParameter("@U_MARKETPLACE", If(userInfos.IsMarketplace, 1, 0)))
            parameters.Add(New SqlParameter("@U_ISCHIRON", If(userInfos.IsChiron, 1, 0)))
            If (userInfos.ChironRole = 0) Then
                parameters.Add(New SqlParameter("@U_CHIRON_ROLE", DBNull.Value))
            Else
                parameters.Add(New SqlParameter("@U_CHIRON_ROLE", userInfos.ChironRole))
            End If
            parameters.Add(New SqlParameter("@SU_SALESREP_NO", userInfos.SuSalesRepNo))
            parameters.Add(New SqlParameter("@U_CHIRON_BRAND", userInfos.ChironBrands))
            parameters.Add(New SqlParameter("@OrderTypes", userInfos.OrderTypeList))

            ' Execute SQL Command
            runWithSuccess = ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[UsrMgmt_AddUser]", parameters)

            ' Handle Success/Failure
            If runWithSuccess Then
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "CloseWindow", "CloseWindow('Submit');", True)
            Else
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Unable to Save New user!');", True)
                errorMsg = "An unexpected error has occurred"
            End If

        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Unable to Save New user!');", True)
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            errorMsg = String.Format("<b>Methode Name:</b>SetNewUser</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
        End Try

        ' Log the operation
        ClsHelper.Log(If(Me.Mode = "CreateSuperUser", "Create Super User", "Create User"), logonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), stopWatch.ElapsedMilliseconds, Not runWithSuccess, errorMsg)
        ClsHelper.LogEbusinessAction(CInt(HD_EnvironmentID.Value), HD_SOPNAME.Value, If(Me.Mode = "CreateSuperUser", "Create Super User", "Create User"), logonUser.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), stopWatch.ElapsedMilliseconds, Not runWithSuccess, errorMsg)
    End Sub


    Protected Sub SetCurrentUser(ByVal userInfos As UserInfos)
        Dim stopWatch As Stopwatch = Stopwatch.StartNew()
        Dim errorMsg As String = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        Dim runWithSuccess As Boolean = False
        If logonUser IsNot Nothing Then
            Try
                If (String.IsNullOrEmpty(userInfos.Email)) Then
                    userInfos.Email = userInfos.EmailLogin
                End If
                parameters.Add(New SqlParameter("@EnvironmentID", HD_EnvironmentID.Value))
                parameters.Add(New SqlParameter("@U_GLOBALID", New Guid(HD_U_GlobalID.Value)))
                parameters.Add(New SqlParameter("@EmailLogin", userInfos.EmailLogin))
                parameters.Add(New SqlParameter("@UserName", userInfos.DisplayName))
                parameters.Add(New SqlParameter("@Login", userInfos.UserName))
                parameters.Add(New SqlParameter("@Password", userInfos.Password))
                parameters.Add(New SqlParameter("@Email", userInfos.Email))
                parameters.Add(New SqlParameter("@U_PHONE", userInfos.PhoneNo))
                parameters.Add(New SqlParameter("@WUID", userInfos.WUId))
                parameters.Add(New SqlParameter("@U_EXPIRE", userInfos.DateExpiration))
                parameters.Add(New SqlParameter("@U_MAX_LINE_QTY", userInfos.MaxLineQty))
                parameters.Add(New SqlParameter("@U_MAX_LINES", userInfos.MaxLines))
                parameters.Add(New SqlParameter("@U_ISADMIN", If(userInfos.IsAdmin, 1, 0)))
                parameters.Add(New SqlParameter("@U_ISINTERNAL", If(userInfos.IsInternal, 1, 0)))
                parameters.Add(New SqlParameter("@GWSGroupID", userInfos.GWSGroupID))
                parameters.Add(New SqlParameter("@U_DEFAULT_MENU", userInfos.DefaultMenu))
                parameters.Add(New SqlParameter("@U_TYPE", userInfos.TypeUser))
                parameters.Add(New SqlParameter("@defaultRightList", userInfos.RightList))
                If Me.Mode = "UpdateSuperUser" Then
                    parameters.Add(New SqlParameter("@U_CAT_GLOBALID", ddlSuperUserCategory.SelectedValue))
                End If
                If String.IsNullOrEmpty(userInfos.SCI_UUID) Then
                    parameters.Add(New SqlParameter("@U_EXT_ID", DBNull.Value))
                Else
                    parameters.Add(New SqlParameter("@U_EXT_ID", userInfos.SCI_UUID))
                End If

                parameters.Add(New SqlParameter("@U_MARKETPLACE", IIf(userInfos.IsMarketplace, 1, 0)))
                parameters.Add(New SqlParameter("@U_ISCOMNORM", IIf(userInfos.IsComNorm, 1, 0)))
                parameters.Add(New SqlParameter("@U_LOGINCOMNORM", userInfos.LoginComNorm))
                parameters.Add(New SqlParameter("@U_PASSWORDCOMNORM", ClsHelper.Encrypt(userInfos.PasswordComNorm)))
                parameters.Add(New SqlParameter("@U_ISCHIRON", IIf(userInfos.IsChiron, 1, 0)))
                If (userInfos.ChironRole = 0) Then
                    parameters.Add(New SqlParameter("@U_CHIRON_ROLE", DBNull.Value))
                Else
                    parameters.Add(New SqlParameter("@U_CHIRON_ROLE", userInfos.ChironRole))
                End If

                parameters.Add(New SqlParameter("@SU_SALESREP_NO", userInfos.SuSalesRepNo))
                parameters.Add(New SqlParameter("@U_CHIRON_BRAND", userInfos.ChironBrands))
                parameters.Add(New SqlParameter("@OrderTypes", userInfos.OrderTypeList))

                runWithSuccess = ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[UsrMgmt_UpdateUser]", parameters)
                If runWithSuccess Then
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "CloseWindow", "CloseWindow('Submit');", True)
                Else
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Unable to update user!');", True)
                    errorMsg = "An unexpected error has occurred"
                End If
            Catch ex As Exception
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Unable update user!');", True)
                Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
                If Not ex.Message Is Nothing Then
                    exceptionMessage = ex.Message
                End If
                If Not ex.StackTrace Is Nothing Then
                    exceptionStackTrace = ex.StackTrace
                End If
                errorMsg = String.Format("<b>Methode Name:</b>SetNewUser</br><b>Excepetion Message:</b></br>{0}</br>" _
                            + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                            , exceptionStackTrace)
            End Try
            ClsHelper.Log(IIf(Me.Mode = "UpdateSuperUser", "Update Super User", "Update User"), logonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), stopWatch.ElapsedMilliseconds, Not runWithSuccess, errorMsg)
            ClsHelper.LogEbusinessAction(CInt(HD_EnvironmentID.Value), HD_SOPNAME.Value, IIf(Me.Mode = "UpdateSuperUser", "Update Super User", "Update User"), logonUser.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), stopWatch.ElapsedMilliseconds, Not runWithSuccess, errorMsg)
        End If
    End Sub



    Public Function GetDefaultSopLanguage() As String
        If (String.IsNullOrEmpty(HD_SOPNAME.Value)) Then
            Return "en"
        End If

        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", HD_EnvironmentID.Value))
        parameters.Add(New SqlParameter("@SOPID", HD_SOPNAME.Value))

        Dim res As String = ClsDataAccessHelper.ExecuteScalar("[Ebusiness].[UsrMgmt_GetDefaultLanguage]", parameters).ToString().Substring(0, 2)
        Return res.ToUpper()

    End Function


    Public Function CheckLoginUnicity(ByVal pUserLogin As String, Optional ByVal puserID As String = "") As Boolean

        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", HD_EnvironmentID.Value))
        parameters.Add(New SqlParameter("@USER_LOGIN", pUserLogin))
        If puserID <> "" Then
            parameters.Add(New SqlParameter("@U_GLOBALID", puserID))
        End If

        Dim param = New SqlClient.SqlParameter("@RETURN_VALUE", SqlDbType.Int)
        param.Direction = ParameterDirection.ReturnValue
        parameters.Add(param)

        Dim res As Boolean = ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[UsrMgmt_LOGIN_ISUNIQUE]", parameters, True)
        Return res

    End Function

    Public Function CheckEmailLoginUnicity(ByVal pEmailLogin As String, Optional ByVal puserID As String = "") As Boolean

        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", HD_EnvironmentID.Value))
        parameters.Add(New SqlParameter("@EMAIL_LOGIN", pEmailLogin))
        If puserID <> "" Then
            parameters.Add(New SqlParameter("@U_GLOBALID", puserID))
        End If

        Dim param = New SqlClient.SqlParameter("@RETURN_VALUE", SqlDbType.Int)
        param.Direction = ParameterDirection.ReturnValue
        parameters.Add(param)

        Dim res As Boolean = ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[UsrMgmt_EMAIL_LOGIN_ISUNIQUE]", parameters, True)
        Return res

    End Function
    Public Function CheckUnicityInLocalDatabase(ByVal pEmailLogin As String, ByVal pUserLogin As String, Optional ByVal puserID As String = "") As Boolean
        Dim result = CheckEmailLoginUnicity(pEmailLogin, puserID)
        If Not result Then
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Login email is already taken by another account. Please, use another one.');", True)
            Return False
        End If


        If Not String.IsNullOrEmpty(pUserLogin) AndAlso Not (pUserLogin.ToLower().Contains("notdefined") OrElse pUserLogin.ToLower().Contains("undefined")) Then
            If String.IsNullOrEmpty(puserID) Then
                Dim res = CheckLoginUnicity(pUserLogin, puserID)
                If Not res Then
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('This login is already taken by another account. Please, use another one.');", True)
                    Return False
                End If
            End If
        End If
        Dim res1 As Boolean = EmailForMediaDownloadCenter()
        If Not res1 Then
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Notifications email is mandatory to send the product media archives.<br/>Please add a valid email or untick the Media Download Center right.');", True)
            Return False
        End If


        Return True
    End Function

    Public Function EmailForMediaDownloadCenter() As Boolean

        Dim selectedValues As List(Of String) = uerRightsChbkoxList.Items.Cast(Of ListItem)().Where(Function(li) li.Selected).[Select](Function(li) li.Value).ToList()
        If selectedValues.Count > 0 AndAlso selectedValues.Contains("b7e69a18-e547-4dd0-b0cb-d1b2b71c7b27") AndAlso txtBoxEmail.Text = "" Then
            Return False
        End If
        Return True

    End Function


    Protected Sub ConfirmEmailValidate(sender As Object, args As ServerValidateEventArgs) Handles vldEmail.ServerValidate
        If (Not String.Equals(txtBoxEmailLogin.Text, txtBoxConfirmLoginEmail.Text)) Then
            args.IsValid = False
            vldEmail.ErrorMessage = "Emails are different"
        End If
    End Sub

    Protected Sub LoginNameCustomValidate(sender As Object, args As ServerValidateEventArgs) Handles cvldLoginName.ServerValidate
        If Convert.ToBoolean(LoginNameRequiredForTPInCurrentCountry) Then
            Dim isMarketplace As Boolean = False
            If Not String.IsNullOrEmpty(IsMarketplace_TextBox.Text) Then
                Try
                    isMarketplace = Convert.ToBoolean(IsMarketplace_TextBox.Text)
                Catch
                End Try
            End If
            If (isMarketplace) Then
                If (String.Equals(txtBoxLoginName.Text, ClsSciUtil.DisplayTextForGuidLoginName)) Then
                    args.IsValid = False
                    cvldLoginName.ErrorMessage = "Incorect value for Tradeplace user account"
                End If
            End If
        End If
    End Sub

    Private Sub cbSalesRep_CheckedChanged(sender As Object, e As EventArgs) Handles cbSalesRep.CheckedChanged
        If cbSalesRep.Checked = True Then
            txtSalesRep.Enabled = True
        Else
            txtSalesRep.Enabled = False
            txtSalesRep.Text = String.Empty
        End If
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Function ChironRightsHAndler()
    End Function

    Protected Enum ChironType
        Basic = 10
        MEDIUM = 20
        ADVANCED = 30
        ADMIN = 40
    End Enum
End Class

