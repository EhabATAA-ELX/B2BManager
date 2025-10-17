
Imports System.Data
Imports System.Data.SqlClient

Partial Class UserControls_ContactDetails
    Inherits System.Web.UI.UserControl

    Private _IsNew As Boolean

    Public Property IsNew As Boolean
        Get
            Return _IsNew
        End Get
        Set
            _IsNew = Value
        End Set
    End Property

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            LoadInformation()

            If IsNew Then
                CancelBtn.Text = "<i class=""fas fa-ban""></i> Cancel"
                SubmitBtn.Text = "<i class=""far fa-check-circle""></i> Save"
                PreviewPanel.Visible = False
                If Not (ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_DUPLICATE_CONTACT)) Then
                    DisableControls()
                End If
            Else
                If Not (ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_CONTACT_DETAILS)) Then
                    DisableControls()
                End If
            End If
        End If
    End Sub

    Private Sub BindContactInformation(environmentID As Integer, cid As Guid)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", environmentID))
        parameters.Add(New SqlParameter("@ContactID", cid))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("[Ebusiness].[CtcMgmt_GetContactByID]", parameters)
        If dataTable.Rows.Count = 1 Then
            If Not IsNew And Not IsPostBack Then
                ContactPreview.DataSource = dataTable
            ElseIf IsPostBack Then
                ContactPreview.DataBindSource(dataTable)
            End If
            Dim contactRow As DataRow = dataTable.Rows(0)
            txtBoxBeforeText.Text = ClsDataAccessHelper.GetText(contactRow, "CT_BEFORETEXT1")
            txtBoxName.Text = ClsDataAccessHelper.GetText(contactRow, "CT_NAME") + IIf(IsNew, " ~copy", "")
            txtBoxType.Text = ClsDataAccessHelper.GetText(contactRow, "CT_TYPE")
            txtBoxStreet1.Text = ClsDataAccessHelper.GetText(contactRow, "CT_STREET1")
            txtBoxStreet2.Text = ClsDataAccessHelper.GetText(contactRow, "CT_STREET2")
            txtBoxStreet3.Text = ClsDataAccessHelper.GetText(contactRow, "CT_STREET3")
            txtBoxStreet4.Text = ClsDataAccessHelper.GetText(contactRow, "CT_STREET4")
            txtBoxStreet5.Text = ClsDataAccessHelper.GetText(contactRow, "CT_STREET5")
            txtBoxPostcode.Text = ClsDataAccessHelper.GetText(contactRow, "CT_POSTCODE")
            txtBoxCounty.Text = ClsDataAccessHelper.GetText(contactRow, "CT_COUNTY")
            txtBoxCountry.Text = ClsDataAccessHelper.GetText(contactRow, "CT_COUNTRY")
            txtBoxOffice.Text = ClsDataAccessHelper.GetText(contactRow, "CT_OFFICE_PHONE")
            txtBoxDirect.Text = ClsDataAccessHelper.GetText(contactRow, "CT_DIRECT_PHONE")
            txtBoxFax.Text = ClsDataAccessHelper.GetText(contactRow, "CT_FAX")
            txtBoxCity.Text = ClsDataAccessHelper.GetText(contactRow, "CT_CITY")
            txtBoxEmail.Text = ClsDataAccessHelper.GetText(contactRow, "CT_EMAIL")
            txtBoxInCardText.Text = ClsDataAccessHelper.GetText(contactRow, "CT_BEFORETEXT2")
            If DBNull.Value IsNot contactRow("CT_POSITION") Then
                txtPosition.Value = CInt(contactRow("CT_POSITION"))
            End If
        End If
    End Sub
    Protected Sub PreviewBtn_Click(sender As Object, e As EventArgs)
        Dim previewDataTable As DataTable = New DataTable()
        Dim row As DataRow = previewDataTable.NewRow()
        AddColumnWithValue(txtBoxBeforeText.Text, "CT_BEFORETEXT1", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxName.Text, "CT_NAME", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxType.Text, "CT_TYPE", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxStreet1.Text, "CT_STREET1", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxStreet2.Text, "CT_STREET2", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxStreet3.Text, "CT_STREET3", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxStreet4.Text, "CT_STREET4", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxStreet5.Text, "CT_STREET5", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxPostcode.Text, "CT_POSTCODE", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxCountry.Text, "CT_COUNTRY", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxCounty.Text, "CT_COUNTY", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxOffice.Text, "CT_OFFICE_PHONE", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxDirect.Text, "CT_DIRECT_PHONE", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxFax.Text, "CT_FAX", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxEmail.Text, "CT_EMAIL", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxCity.Text, "CT_CITY", GetType(System.String), previewDataTable, row)
        AddColumnWithValue(txtBoxInCardText.Text, "CT_BEFORETEXT2", GetType(System.String), previewDataTable, row)
        previewDataTable.Rows.Add(row)
        ContactPreviewWindow.DataBindSource(previewDataTable)
        updatePanelPreview.Update()
        ScriptManager.RegisterStartupScript(UpdatePanelDetails, UpdatePanelDetails.GetType(), "ShowPreviewWindow", "ShowPreviewWindow();", True)
    End Sub

    Private Function AddColumnWithValue(value As Object, columnName As String, type As Type, ByRef dataTable As DataTable, ByRef row As DataRow) As DataColumn
        Dim dataColumn As DataColumn = New DataColumn(columnName, type)
        dataTable.Columns.Add(dataColumn)
        row(columnName) = value
        Return dataColumn
    End Function
    Protected Sub SubmitBtn_Click(sender As Object, e As EventArgs)
        Dim EnvironmentID As Integer = 0
        Dim sopID As String = Request.QueryString("sopid")
        Dim cid As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("cid")) Then
            Guid.TryParse(Request.QueryString("cid"), cid)
        End If

        If EnvironmentID > 0 AndAlso Not String.IsNullOrEmpty(sopID) Then
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@CT_NAME", txtBoxName.Text))
            parameters.Add(New SqlParameter("@CT_STREET1", txtBoxStreet1.Text))
            parameters.Add(New SqlParameter("@CT_STREET2", txtBoxStreet2.Text))
            parameters.Add(New SqlParameter("@CT_STREET3", txtBoxStreet3.Text))
            parameters.Add(New SqlParameter("@CT_STREET4", txtBoxStreet4.Text))
            parameters.Add(New SqlParameter("@CT_STREET5", txtBoxStreet5.Text))
            parameters.Add(New SqlParameter("@CT_POSTCODE", txtBoxPostcode.Text))
            parameters.Add(New SqlParameter("@CT_CITY", txtBoxCity.Text))
            parameters.Add(New SqlParameter("@CT_COUNTY", txtBoxCounty.Text))
            parameters.Add(New SqlParameter("@CT_COUNTRY", txtBoxCountry.Text))
            parameters.Add(New SqlParameter("@CT_OFFICE_PHONE", txtBoxOffice.Text))
            parameters.Add(New SqlParameter("@CT_DIRECT_PHONE", txtBoxDirect.Text))
            parameters.Add(New SqlParameter("@CT_FAX", txtBoxFax.Text))
            parameters.Add(New SqlParameter("@CT_EMAIL", txtBoxEmail.Text))
            parameters.Add(New SqlParameter("@CT_TYPE", txtBoxType.Text))
            parameters.Add(New SqlParameter("@CT_BEFORETEXT1", txtBoxBeforeText.Text))
            parameters.Add(New SqlParameter("@CT_BEFORETEXT2", txtBoxInCardText.Text))
            parameters.Add(New SqlParameter("@CT_POSITION", txtPosition.Value))
            If IsNew Then
                parameters.Add(New SqlParameter("@SOPID", sopID))
                If (ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_DUPLICATE_CONTACT)) Then
                    If ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[CtcMgmt_AddContact]", parameters) Then
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SubmitAction", "SubmitAction('SubmitCreate','<p class=""text-justify success-popup""><i class=""far fa-check-circle"" style=""font-size: 18pt;vertical-align: middle;""></i> Contact created with success</p>');", True)
                    Else
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Unable to create the contact!');", True)
                    End If
                Else
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('You are not allowed to create contacts!');", True)
                End If
            Else
                If cid <> Guid.Empty Then
                    parameters.Add(New SqlParameter("@CT_GLOBALID", cid))
                    If (ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_CONTACT_DETAILS)) Then
                        If ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[CtcMgmt_UpdateContact]", parameters) Then
                            LoadInformation()
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SubmitAction", "SubmitAction('SubmitUpdate','<p class=""text-justify success-popup""><i class=""far fa-check-circle"" style=""font-size: 18pt;vertical-align: middle;""></i> Contact updated with success</p>');", True)
                        Else
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Update contact failed!');", True)
                        End If
                    Else
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('You are not allowed to edit contacts!');", True)
                    End If
                End If
            End If
        End If
    End Sub

    Protected Sub LoadInformation()
        Dim EnvironmentID As Integer = 0
        Dim cid As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("cid")) Then
            Guid.TryParse(Request.QueryString("cid"), cid)
        End If

        If EnvironmentID > 0 AndAlso cid <> Guid.Empty Then
            BindContactInformation(EnvironmentID, cid)
        End If
    End Sub

    Private Sub DisableControls()
        txtBoxBeforeText.Enabled = False
        txtBoxName.Enabled = False
        txtBoxType.Enabled = False
        txtBoxStreet1.Enabled = False
        txtBoxStreet2.Enabled = False
        txtBoxStreet3.Enabled = False
        txtBoxStreet4.Enabled = False
        txtBoxStreet5.Enabled = False
        txtBoxPostcode.Enabled = False
        txtBoxCounty.Enabled = False
        txtBoxCountry.Enabled = False
        txtBoxOffice.Enabled = False
        txtBoxDirect.Enabled = False
        txtBoxFax.Enabled = False
        txtBoxCity.Enabled = False
        txtBoxEmail.Enabled = False
        txtBoxInCardText.Enabled = False
        txtPosition.Enabled = False
        CancelBtn.Visible = False
        SubmitBtn.Visible = False
    End Sub
    Protected Sub CancelBtn_Click(sender As Object, e As EventArgs)
        If IsNew Then
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Cancel", "Cancel();", True)
        Else
            LoadInformation()
        End If
    End Sub
End Class
