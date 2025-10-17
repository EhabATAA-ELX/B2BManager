
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.IO
Imports System.Threading
Imports Telerik.Web.UI

Partial Class EbusinessCompanyProfile
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        CustomerDetails.Mode = "UpdateCustomer"
        Dim target As String = Request("__EVENTTARGET")
        If Not IsPostBack Or "Refresh".Equals(target) Then
            Dim EnvironmentID As Integer = 0
            Dim cid As Guid = Guid.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("cid")) Then
                Guid.TryParse(Request.QueryString("cid"), cid)
            End If

            If EnvironmentID > 0 AndAlso cid <> Guid.Empty Then
                BindCustomerInformation(EnvironmentID, cid)
            End If

        End If
    End Sub

    Private Sub BindCustomerInformation(environmentID As Integer, cid As Guid)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        CType(CustomerDetails.FindControl("HD_EnvID"), HiddenField).Value = environmentID
        CType(CustomerDetails.FindControl("HD_C_GlobalID"), HiddenField).Value = cid.ToString()
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        Dim customerActive As Boolean = False
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", environmentID))
        parameters.Add(New SqlParameter("@CID", cid))
        Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("[Ebusiness].[UsrMgmt_GetCustomerByID]", parameters)
        If dataSet.Tables.Count = 2 Then
            Dim customerDetailsDT As DataTable = dataSet.Tables(0)
            If customerDetailsDT.Rows.Count > 0 Then
                Dim companyRow As DataRow = customerDetailsDT.Rows(0)
                customerActive = companyRow("C_ISACTIVE").ToString() = "1" Or companyRow("C_ISACTIVE").ToString() = "True"
                CType(CustomerDetails.FindControl("HD_SopName"), HiddenField).Value = ClsDataAccessHelper.GetText(companyRow, "S_SOP_ID")
                lblCustomerCode.Text = ClsDataAccessHelper.GetText(companyRow, "C_CUID")

                If customerActive Then
                    iframeUserList.Src = String.Format("EbusinessUsersList.aspx?HideHeader=true&envid={0}&SopID={1}&cid={2}", environmentID.ToString(), ClsDataAccessHelper.GetText(companyRow, "S_SOP_ID"), cid.ToString())
                Else
                    RadTabStrip1.FindTabByValue("UserList").Visible = False
                    RadPageViewUserList.Visible = False
                End If

                If customerActive AndAlso clsUser.Actions.Contains(ClsHelper.ActionDesignation.DISPLAY_ADDRESS_LIST_TAB_IN_CUSTOMER_PROFILE) Then
                    iframeAddressList.Src = String.Format("EbusinessCustomerAddressList.aspx?HideHeader=true&envid={0}&cid={1}", environmentID.ToString(), cid.ToString())
                Else
                    RadTabStrip1.FindTabByValue("AddressList").Visible = False
                    RadPageViewAddressList.Visible = False
                End If

                If customerActive AndAlso clsUser.Actions.Contains(ClsHelper.ActionDesignation.DISPLAY_MANAGE_CONTACTS_TAB_IN_CUSTOMER_AND_USER_PROFILES) Then
                    iframeContacts.Src = String.Format("EbusinessContacts.aspx?HideHeader=true&envid={0}&cid={1}", environmentID.ToString(), cid.ToString())
                Else
                    RadTabStrip1.FindTabByValue("Contacts").Visible = False
                    RadPageViewContacts.Visible = False
                End If

                If clsUser.Actions.Contains(ClsHelper.ActionDesignation.DISPLAY_CUSTOMER_ACTIVITY_HISTORY_TAB) Then
                    iframeActivityLog.Src = String.Format("EbusinessActivityHistory.aspx?HideHeader=true&envid={0}&cid={1}&sopid={2}", environmentID.ToString(), cid.ToString(), ClsDataAccessHelper.GetText(companyRow, "S_SOP_ID"))
                Else
                    RadTabStrip1.Tabs.FindTabByValue("ActivityHistory").Visible = False
                    RadPageViewActivityHistory.Visible = False
                End If

                If clsUser.Actions.Contains(ClsHelper.ActionDesignation.DISPLAY_INSIGHTS_TAB_IN_CUSTOMER_PROFILE) Then
                    iframeInsights.Src = String.Format("EbusinessCustomerInsights.aspx?HideHeader=true&envid={0}&cid={1}", environmentID.ToString(), cid.ToString())
                    ' Load insights history chart
                    Dim CreationStoredProcedureName As String = String.Format("[Ebusiness].[UsrMgmt_GetCustomerHistoryOverview] @EnvironmentID = {0} , @CUID = '{1}'", environmentID, ClsDataAccessHelper.GetText(companyRow, "C_CUID"))
                    ClsInsightsHelper.ProcessDataAsync(CreationStoredProcedureName, "History_Chart_" + environmentID.ToString() + "_" & cid.ToString())
                    CreationStoredProcedureName = String.Format("[Ebusiness].[UsrMgmt_GetCustomerOrderingInsights] @EnvironmentID = {0} , @CUID = '{1}'", environmentID, ClsDataAccessHelper.GetText(companyRow, "C_CUID"))
                    ClsInsightsHelper.ProcessDataAsync(CreationStoredProcedureName, "Customer_Ordering_Insights_" + environmentID.ToString() + "_" & cid.ToString())
                Else
                    RadTabStrip1.Tabs.FindTabByValue("Insights").Visible = False
                    RadPageViewInsights.Visible = False
                End If

                If Not (customerActive AndAlso clsUser.Actions.Contains(ClsHelper.ActionDesignation.UPLOAD_CUSTOMER_LOGO)) Then
                    uploadPlaceHolder.Visible = False
                End If

                ' Add customer tags
                AddCustomerTags(companyRow)

                lblCustomerName.Text = ClsDataAccessHelper.GetText(companyRow, "C_NAME")
                Dim C_MAX_LINES As Double = 999
                Dim C_SB_MIN_QTY As Double = 0
                Dim C_MAX_LINE_QTY As Double = 999
                Dim C_MIN_QTY As Double = 1
                Dim C_OVERRIDE_SAP_IMPORT As Boolean
                Double.TryParse(companyRow("C_MAX_LINES"), C_MAX_LINES)
                Double.TryParse(companyRow("C_SB_MIN_QTY"), C_SB_MIN_QTY)
                Double.TryParse(companyRow("C_MAX_LINE_QTY"), C_MAX_LINE_QTY)
                Double.TryParse(companyRow("C_MIN_QTY"), C_MIN_QTY)
                Boolean.TryParse(companyRow("C_OVERRIDE_SAP_IMPORT"), C_OVERRIDE_SAP_IMPORT)
                CType(CustomerDetails.FindControl("radNumericShoppingBasketMaxLines"), RadNumericTextBox).Value = C_MAX_LINES
                CType(CustomerDetails.FindControl("radNumericShoppingBasketTotalQuantity"), RadNumericTextBox).Value = C_SB_MIN_QTY
                CType(CustomerDetails.FindControl("radNumericSingleLineMaxQuantity"), RadNumericTextBox).Value = C_MAX_LINE_QTY
                CType(CustomerDetails.FindControl("radNumericSingleLineMinQuantity"), RadNumericTextBox).Value = C_MIN_QTY
                CType(CustomerDetails.FindControl("txtBoxCustomerName"), TextBox).Text = ClsDataAccessHelper.GetText(companyRow, "C_NAME")
                CType(CustomerDetails.FindControl("chkBoxOverrideSapImport"), CheckBox).Checked = C_OVERRIDE_SAP_IMPORT
                CType(CustomerDetails.FindControl("txtBoxCustomerCode"), TextBox).Text = ClsDataAccessHelper.GetText(companyRow, "C_CUID")
                CType(CustomerDetails.FindControl("txtBoxDescription"), TextBox).Text = ClsDataAccessHelper.GetText(companyRow, "C_Description")
                CType(CustomerDetails.FindControl("txtBoxLinkedSoldToID"), TextBox).Text = ClsDataAccessHelper.GetText(companyRow, "C_CUSTOMER_SOLDTOID")
                CType(CustomerDetails.FindControl("TxtBoxDeliveryBlock"), TextBox).Text = ClsDataAccessHelper.GetText(companyRow, "C_DELIVERYBLOCK")
                CType(Master.FindControl("title"), HtmlTitle).Text = "Customer profile - " + ClsDataAccessHelper.GetText(companyRow, "C_NAME")
                Dim checkBoxList As CheckBoxList = CType(CustomerDetails.FindControl("customerRightsChbkoxList"), CheckBoxList)
                For Each item As ListItem In checkBoxList.Items
                    If companyRow(item.Value) IsNot DBNull.Value Then
                        checkBoxList.Items.FindByValue(item.Value).Selected = companyRow(item.Value)
                    End If
                Next

                If Not (customerActive And clsUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_CUSTOMER_DETAILS)) Then
                    DisableControls()
                Else
                    Me.Form.DefaultButton = CType(CustomerDetails.FindControl("btnSubmit"), LinkButton).UniqueID
                End If
            End If

            If customerActive Then
                Dim rightsCount As Integer = 0
                If clsUser.Actions.Contains(ClsHelper.ActionDesignation.MAINTAIN_SUPER_USER_CUSTOMER_LIST) Then
                    rightsCount += 1
                    customerActionsHeader.InnerText = "Attach to Super User"
                Else
                    attachToSuperUserPlaceHolder.Visible = False
                End If
                If clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_CUSTOMER_PRICE_CACHE) Then
                    rightsCount += 1
                    If rightsCount = 0 Then
                        customerActionsHeader.InnerText = "Price Cache Reset"
                    End If
                Else
                    deletePriceCachePlaceHolder.Visible = False
                End If
                If clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_CUSTOMER_RANGE) Then
                    rightsCount += 1
                    If rightsCount = 0 Then
                        customerActionsHeader.InnerText = "Range Reset"
                    End If
                Else
                    deleteCustomerRangePlaceHolder.Visible = False
                End If

                If rightsCount = 0 Then
                    deletePriceCacheAndRangeTD.Visible = False
                    deleteCustomerRangePlaceHolder.Visible = False
                    deletePriceCachePlaceHolder.Visible = False
                ElseIf rightsCount > 1 Then
                    customerActionsHeader.InnerText = "Customer Actions"
                End If
            Else
                deletePriceCacheAndRangeTD.Visible = False
                deleteCustomerRangePlaceHolder.Visible = False
                deletePriceCachePlaceHolder.Visible = False
            End If

            If dataSet.Tables(1).Rows.Count = 1 Then
                customerLogo.ImageUrl = "~/GetCustomerLogo.ashx?logoID=" + dataSet.Tables(1).Rows(0)("LogoID").ToString()
                If customerActive AndAlso clsUser.Actions.Contains(ClsHelper.ActionDesignation.UPLOAD_CUSTOMER_LOGO) Then
                    deleteLogoLbl.Visible = True
                End If
            Else
                customerLogo.ImageUrl = "~/Images/Ebusiness/CustomersManagement/company.png"
                deleteLogoLbl.Visible = False
            End If
        End If
        ClsHelper.Log("Display Customer Profile", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
    End Sub

    Private Sub DisableControls()
        CType(CustomerDetails.FindControl("radNumericShoppingBasketMaxLines"), RadNumericTextBox).Enabled = False
        CType(CustomerDetails.FindControl("radNumericShoppingBasketTotalQuantity"), RadNumericTextBox).Enabled = False
        CType(CustomerDetails.FindControl("radNumericSingleLineMaxQuantity"), RadNumericTextBox).Enabled = False
        CType(CustomerDetails.FindControl("radNumericSingleLineMinQuantity"), RadNumericTextBox).Enabled = False
        CType(CustomerDetails.FindControl("txtBoxCustomerName"), TextBox).Enabled = False
        CType(CustomerDetails.FindControl("chkBoxOverrideSapImport"), CheckBox).Enabled = False
        CType(CustomerDetails.FindControl("txtBoxCustomerCode"), TextBox).Enabled = False
        CType(CustomerDetails.FindControl("txtBoxDescription"), TextBox).Enabled = False
        CType(CustomerDetails.FindControl("txtBoxLinkedSoldToID"), TextBox).Enabled = False
        CType(CustomerDetails.FindControl("customerRightsChbkoxList"), CheckBoxList).Enabled = False
        CType(CustomerDetails.FindControl("actionButtonsTable"), HtmlTable).Visible = False
    End Sub

    Protected Sub AddCustomerTags(companyRow As DataRow)
        Dim customerActive As Boolean = companyRow("C_ISACTIVE").ToString() = "1" Or companyRow("C_ISACTIVE").ToString() = "True"
        If Not customerActive Then
            CustomerTagsGeneral.InnerHtml = ClsEbusinessHelper.FormatTag("Customer Deleted", "#949494", "Customer deleted")
        Else
            'If Not (ClsDataAccessHelper.GetText(companyRow, "C_Description", "") = "") Then
            '    lblDescription.Text = ClsDataAccessHelper.GetText(companyRow, "C_Description")
            '    If ClsDataAccessHelper.GetText(companyRow, "C_Description", "").Length < 35 Then
            '        CustomerTagsTD.InnerHtml += ClsEbusinessHelper.FormatTag(ClsDataAccessHelper.GetText(companyRow, "C_Description", ""), "#17a2b8", "Customer description")
            '    End If
            'End If
            Dim C_OVERRIDE_SAP_IMPORT As Boolean = False
            If companyRow("C_OVERRIDE_SAP_IMPORT") IsNot DBNull.Value Then
                Boolean.TryParse(companyRow("C_OVERRIDE_SAP_IMPORT"), C_OVERRIDE_SAP_IMPORT)
            End If
            'If (C_OVERRIDE_SAP_IMPORT = False) Then
            '    If Not (ClsDataAccessHelper.GetText(companyRow, "C_TimeStampSAPImport", "") = "") Then
            '        CType(CustomerDetails.FindControl("txtBoxCustomerName"), TextBox).Enabled = False
            '        CustomerHeaderTagsID.Attributes.Add("title", "Last updated on " + ClsDataAccessHelper.GetText(companyRow, "C_TimeStampSAPImport", ""))
            '        'CustomerTagsTD.InnerHtml += ClsEbusinessHelper.FormatTag("Name updated by SAP", "#5f19cd", "Last updated on " + ClsDataAccessHelper.GetText(companyRow, "C_TimeStampSAPImport", ""))
            '    End If
            'End If

            If Not (ClsDataAccessHelper.GetText(companyRow, "C_DEFAULTPLANT", "") = "") Then
                CustomerTagsCompanyCode.InnerHtml += ClsEbusinessHelper.FormatTag("Default Plant " + ClsDataAccessHelper.GetText(companyRow, "C_DEFAULTPLANT", ""), "#159ba7")
            End If
            If Not (ClsDataAccessHelper.GetText(companyRow, "C_PLANLEVELCUSTOMERS", "") = "") Then
                CustomerTagsCompanyCode.InnerHtml += ClsEbusinessHelper.FormatTag("Plan Level " + ClsDataAccessHelper.GetText(companyRow, "C_PLANLEVELCUSTOMERS", ""), "#159ba7")
            End If
            If Not (ClsDataAccessHelper.GetText(companyRow, "C_SALESCHANNEL", "") = "") Then
                CustomerTagsCompanyCode.InnerHtml += ClsEbusinessHelper.FormatTag("Sales Channel " + ClsDataAccessHelper.GetText(companyRow, "C_SALESCHANNEL", ""), "#159ba7")
            End If
            If Not (ClsDataAccessHelper.GetText(companyRow, "C_SUBSALESCHANEL", "") = "") Then
                CustomerTagsCompanyCode.InnerHtml += ClsEbusinessHelper.FormatTag("Sub Sales Chanel " + ClsDataAccessHelper.GetText(companyRow, "C_SUBSALESCHANEL", ""), "#159ba7")
            End If
            If Not (ClsDataAccessHelper.GetText(companyRow, "C_SALESREPCODE", "") = "") Then
                CustomerTagsSalesArea.InnerHtml += ClsEbusinessHelper.FormatTag("Sales Rep Code " + ClsDataAccessHelper.GetText(companyRow, "C_SALESREPCODE", ""), "#3FC6D2")
            End If

            If Not (ClsDataAccessHelper.GetText(companyRow, "CH_CUID_3", "") = "") Then
                CustomerTagsSalesArea.InnerHtml += ClsEbusinessHelper.FormatTag("Hier.3 " + ClsDataAccessHelper.GetText(companyRow, "CH_CUID_3", "") + "<br/>" + ClsDataAccessHelper.GetText(companyRow, "CH_NAME_3", ""), "#159ba7")
            End If
            If Not (ClsDataAccessHelper.GetText(companyRow, "CH_CUID_4", "") = "") Then
                CustomerTagsSalesArea.InnerHtml += ClsEbusinessHelper.FormatTag("Hier.4 " + ClsDataAccessHelper.GetText(companyRow, "CH_CUID_4", "") + "<br/>" + ClsDataAccessHelper.GetText(companyRow, "CH_NAME_4", ""), "#159ba9")
            End If
            If Not (ClsDataAccessHelper.GetText(companyRow, "CH_CUID_5", "") = "") Then
                CustomerTagsSalesArea.InnerHtml += ClsEbusinessHelper.FormatTag("Hier.5 " + ClsDataAccessHelper.GetText(companyRow, "CH_CUID_5", "") + "<br/>" + ClsDataAccessHelper.GetText(companyRow, "CH_NAME_5", ""), "#159bab")
            End If

            If Not (ClsDataAccessHelper.GetText(companyRow, "C_PRICELIST", "") = "") Then
                CustomerTagsSalesArea.InnerHtml += ClsEbusinessHelper.FormatTag("Price List " + ClsDataAccessHelper.GetText(companyRow, "C_PRICELIST", ""), "#3FC6D2")
            End If
            If Not (ClsDataAccessHelper.GetText(companyRow, "C_GRP4", "") = "") Then
                CustomerTagsSalesArea.InnerHtml += ClsEbusinessHelper.FormatTag("Customer Group 4 " + ClsDataAccessHelper.GetText(companyRow, "C_GRP4", ""), "#3FC6D2")
            End If
            If Not (ClsDataAccessHelper.GetText(companyRow, "C_DELIVERYBLOCK", "") = "") Then
                CustomerTagsSalesArea.InnerHtml += ClsEbusinessHelper.FormatTag("Delivery Block " + ClsDataAccessHelper.GetText(companyRow, "C_DELIVERYBLOCK", ""), "#3FC6D2")
            End If
            If Not (ClsDataAccessHelper.GetText(companyRow, "C_DEFAULT_PARTIALSHP", "") = "") Then
                CustomerTagsSalesArea.InnerHtml += ClsEbusinessHelper.FormatTag("Complete Delivery " + ClsDataAccessHelper.GetText(companyRow, "C_DEFAULT_PARTIALSHP", ""), "#3FC6D2")
            End If
            If Not (ClsDataAccessHelper.GetText(companyRow, "C_DEFAULT_CREDITCHECK_INDICATOR_MSG", "") = "") Then
                CustomerTagsGeneral.InnerHtml += ClsEbusinessHelper.FormatTag(ClsDataAccessHelper.GetText(companyRow, "C_DEFAULT_CREDITCHECK_INDICATOR_MSG", ""), "#B17964", "Credit check indicator default message")
            End If

            Select Case ClsDataAccessHelper.GetText(companyRow, "C_DEFAULT_CREDITCHECK_INDICATOR", "")
                Case "01"
                    CustomerTagsGeneral.InnerHtml += ClsEbusinessHelper.FormatTag("Has credit block", "#E05837", "Credit check indicator equals to 01")
                Case "02"
                    CustomerTagsGeneral.InnerHtml += ClsEbusinessHelper.FormatTag("Blocked", "#640543", "Credit check indicator equals to 02")
                Case "99"
                    CustomerTagsGeneral.InnerHtml += ClsEbusinessHelper.FormatTag("Never visited", "#640432", "Never connected to the portal")
            End Select




        End If
    End Sub
    Protected Sub uploadButton_Click(sender As Object, e As EventArgs)
        Dim filename As String = Path.GetFileName(FileUploadControl.PostedFile.FileName)
        Dim contentType As String = FileUploadControl.PostedFile.ContentType
        Dim EnvironmentID As Integer = 0
        Dim cid As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("cid")) Then
            Guid.TryParse(Request.QueryString("cid"), cid)
        End If

        Using fs As Stream = FileUploadControl.PostedFile.InputStream
            Using br As BinaryReader = New BinaryReader(fs)
                Dim length As Long = fs.Length
                Dim bytes As Byte() = br.ReadBytes(length)

                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
                parameters.Add(New SqlParameter("@CID", cid))
                parameters.Add(New SqlParameter("@Filename", filename))
                parameters.Add(New SqlParameter("@ContentType", contentType))
                parameters.Add(New SqlParameter("@Data", bytes))
                parameters.Add(New SqlParameter("@FileLength", length))
                parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.GlobalID))
                Dim logoID As Guid = ClsDataAccessHelper.ExecuteScalar("[Ebusiness].[UsrMgmt_UploadCustomerPicture]", parameters)
                Cache.Insert("CustomerLogo_" + logoID.ToString(), New ClsEbusinessHelper.CustomerLogo(filename, contentType, length, bytes))
                customerLogo.ImageUrl = "~/GetCustomerLogo.ashx?logoID=" + logoID.ToString()
                deleteLogoLbl.Visible = True
            End Using
        End Using

    End Sub
    Protected Sub btnDeleteLogo_Click(sender As Object, e As EventArgs)
        Dim EnvironmentID As Integer = 0
        Dim cid As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("cid")) Then
            Guid.TryParse(Request.QueryString("cid"), cid)
        End If
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@CID", cid))
        parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.GlobalID))
        If (ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[UsrMgmt_DeleteCustomerPicture]", parameters)) Then
            customerLogo.ImageUrl = "~/Images/Ebusiness/CustomersManagement/company.png"
            deleteLogoLbl.Visible = False
        End If
    End Sub
End Class
