Imports System.Data
Imports System.Data.SqlClient
Imports Telerik.Web.UI
Partial Class NotificationGroups
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If ClsSessionHelper.LogonUser.Actions.Count > 0 Then
            Dim actions As List(Of ClsHelper.ActionDesignation) = ClsSessionHelper.LogonUser.Actions
            btnAddGroup.Visible = actions.Contains(ClsHelper.ActionDesignation.ADD_NOTIFICATION_GROUP)
        End If

        If Not IsPostBack Then
            RenderEnvironmentDropdown()
            RenderCountryDropdown()
            RenderPopupFields()
            BindNotificationData()
            UpdatePanelGroups.Update()
            ScriptManager.RegisterStartupScript(UpdatePanelGroups, UpdatePanelGroups.GetType(), "BindNotificationTable", "BindNotificationTable();", True)
        End If
    End Sub

    Private Sub RenderCountryDropdown()
        Dim applications As List(Of ClsHelper.Application) = Nothing
        If ClsSessionHelper.LogonUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = ClsSessionHelper.LogonUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, IIf(ClsSessionHelper.EbusinessEnvironmentID IsNot Nothing, ClsSessionHelper.EbusinessEnvironmentID, ClsSessionHelper.LogonUser.DefaultEbusinessEnvironmentID))
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, IIf(ClsSessionHelper.EbusinessSopID IsNot Nothing, ClsSessionHelper.EbusinessSopID, ClsSessionHelper.LogonUser.DefaultEbusinessSopID), False)

    End Sub
    Private Sub RenderEnvironmentDropdown()
        Dim selectedApplication As ClsHelper.Application = ClsSessionHelper.LogonUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, IIf(ClsSessionHelper.EbusinessEnvironmentID IsNot Nothing, ClsSessionHelper.EbusinessEnvironmentID, ClsSessionHelper.LogonUser.DefaultEbusinessEnvironmentID))
    End Sub

    Private Sub RenderPopupFields()
        'Populate country list in popup
        Dim apps = ClsSessionHelper.LogonUser.Applications
        ClsHelper.RenderCountryDropDown(ddlPopupCountry, apps.First().Countries, ClsSessionHelper.EbusinessSopID, False)

        'Populate condition field dropdown with column names from T_Company

        ddlConditionField.Items.Clear()
        'Hard-coded list of T_COMPANY fields for performance
        Dim companyFields As String() = {
            "C_GLOBALID", "C_ISACTIVE", "ID_CY_SOP", "C_CUID", "C_NAME", "C_LOGO", "C_CORREL_ID", "C_MAX_LINE_QTY", "C_MAX_LINES",
            "C_ADDR_TIMESTAMP", "C_DESCRIPTION", "C_NEWS", "C_NEWS_SA", "C_LINKS", "C_STATS", "C_AGREEMENTS", "C_HOMEPAGE", "C_SALESACADEMYTEMPLATE",
            "C_HELP", "C_TP2INFOS", "C_ORDERLIST_TIMESTAMP_REPLY", "C_ORDERLIST_REPLY", "C_ORDERLIST_TIMESTAMP_REQUEST", "C_ORDERLISTCONSO_TIMESTAMP_REPLY",
            "C_ORDERLISTCONSO_REPLY", "C_ORDERLISTCONSO_TIMESTAMP_REQUEST", "C_PERMISSION", "C_MIN_QTY", "C_ID", "C_DEFAULT_MENU", "C_SB_MIN_QTY",
            "C_MAX_MODIFICATION", "C_BOOKABLE", "C_DEFAULT_SHIPTO", "C_DEFAULT_SHIPTO_MSG", "C_DEFAULT_PARTIALSHP", "C_DEFAULT_PARTIALSHP_MSG",
            "C_DEFAULT_CREDITCHECK_INDICATOR", "C_DEFAULT_CREDITCHECK_INDICATOR_MSG", "C_DEFAULT_CURRENCY", "C_DEFAULT_CURRENCY_MSG", "C_ORDERLIST_LAST_REPLY",
            "C_ORDERLIST_LAST_SEQUENCE", "C_ORDERLIST_CORREL_ID", "C_ORDERLISTCONSO_LAST_REPLY", "C_ORDERLISTCONSO_LAST_SEQUENCE", "C_ORDERLISTCONSO_CORREL_ID",
            "C_CONSO_CUSTOMER_CODE", "C_HARDSWITCH", "C_ORDERLIST_LAST_REPLY_DESCRIPTION", "C_ORDERLIST_LAST_REPLY_STATUS", "C_CUSRANGE_TIMESTAMP_REQUEST",
            "C_CUSRANGE_TIMESTAMP_REPLY", "C_PARTIAL_ORDER", "C_CMIR", "C_VMI", "C_CUSTOMER_SOLDTOID", "C_BROKEN_PROMISE", "C_PRICE_DISCOUNT",
            "C_PRICE_SCALES", "C_PLANLEVELCUSTOMERS", "C_SALESCHANNEL", "C_PRICELIST", "C_EXPECTED_PRICE", "C_TimeStampSAPImport", "C_DATE_ADDED",
            "C_KRT_PROMO", "C_OVERRIDE_SAP_IMPORT", "C_OtherPartners", "C_DELIVERYBLOCK", "C_KRT", "C_FOCUS_RANGE", "C_PASSAvailability",
            "C_SUBSALESCHANEL", "C_SALESREPCODE", "C_DEFAULTPLANT"
        }
        For Each field As String In companyFields
            ddlConditionField.Items.Add(New ListItem(field, field))
        Next


    End Sub

    Private Sub BindNotificationData()
        Try

            'Populate the NotificationTable with data from the database
            Using cnx As New SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
                Using cmd As New SqlCommand("GetNotificationGroups", cnx)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@EnvironmentID", ddlEnvironment.SelectedValue)
                    cmd.Parameters.AddWithValue("@S_SOP_ID", ddlCountry.SelectedValue)
                    Dim adapter As New SqlDataAdapter(cmd)
                    Dim ds As New DataSet()
                    adapter.Fill(ds, "Result")

                    ' Preserve header rows, clear only data rows
                    For i As Integer = NotificationTable.Rows.Count - 1 To 0 Step -1
                        If Not TypeOf NotificationTable.Rows(i) Is TableHeaderRow Then
                            NotificationTable.Rows.RemoveAt(i)
                        End If
                    Next

                    For Each dr As DataRow In ds.Tables("Result").Rows
                        Dim rowId As String = dr("N_GROUP_ID").ToString()
                        Dim row As New TableRow With {.ClientIDMode = ClientIDMode.Static, .ID = rowId}

                        ' Actions cell: Edit and Deactivate buttons
                        Dim actionCell As New TableCell()
                        actionCell.CssClass = "TextAlignCenter"

                        Dim editHtml As String
                        If ClsSessionHelper.LogonUser.Actions.Count > 0 Then
                            Dim actions As List(Of ClsHelper.ActionDesignation) = ClsSessionHelper.LogonUser.Actions
                            If actions.Contains(ClsHelper.ActionDesignation.EDIT_NOTIFICATION_GROUP) Then
                                editHtml = String.Format(
                                            "<input type=""image"" class=""width20px"" src=""Images/edit.png"" " &
                                            "title=""Edit"" onclick=""EditNotification('{0}'); return false;"" />",
                                                rowId)
                            End If
                        End If


                        actionCell.Controls.Add(New LiteralControl(editHtml))
                        row.Cells.Add(actionCell)

                        ' Data cells: Country, Group ID, Group Name, Category, Condition Field, Condition Value, Is Active
                        Dim cols As String() = {"S_SOP_ID", "N_GROUP_ID", "N_GROUP_NAME", "N_CATEGORY", "N_CONDITION_FIELD", "N_CONDITION_VALUE", "N_ISACTIVE"}
                        For Each colName As String In cols
                            Dim cell As New TableCell()
                            cell.ClientIDMode = ClientIDMode.Static
                            cell.ID = rowId & "_" & colName
                            cell.Text = dr(colName).ToString()
                            cell.Attributes.Add("data", dr(colName).ToString())
                            row.Cells.Add(cell)
                        Next

                        NotificationTable.Rows.Add(row)
                    Next
                End Using
            End Using

        Catch ex As Exception

        End Try
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
            Dim errorMsg As String = String.Format("<b>Exception Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
    End Sub


    Protected Sub btnAddGroup_Click(sender As Object, e As EventArgs)
        Dim editingId As String = hfEditGroupId.Value

        Using cnx As New SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
            cnx.Open()

            If String.IsNullOrEmpty(editingId) Then
                ' INSERT new group
                Using cmd As New SqlCommand("InsertNotificationGroup", cnx)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@EnvironmentID", ClsSessionHelper.EbusinessEnvironmentID)
                    cmd.Parameters.AddWithValue("@N_GROUP_ID", Guid.NewGuid())
                    cmd.Parameters.AddWithValue("@S_SOP_ID", ddlPopupCountry.SelectedValue)
                    cmd.Parameters.AddWithValue("@N_GROUP_NAME", txtGroupName.Text)
                    cmd.Parameters.AddWithValue("@N_CATEGORY", txtCategory.Text)
                    cmd.Parameters.AddWithValue("@N_CONDITION_FIELD", ddlConditionField.SelectedValue)
                    cmd.Parameters.AddWithValue("@N_CONDITION_VALUE", txtConditionValue.Text)
                    cmd.Parameters.AddWithValue("@N_ISACTIVE", Boolean.Parse(rblIsActive.SelectedValue))
                    cmd.ExecuteNonQuery()
                End Using
            Else
                ' UPDATE existing group
                Using cmd As New SqlCommand("UpdateNotificationGroup", cnx)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@EnvironmentID", ClsSessionHelper.EbusinessEnvironmentID)
                    cmd.Parameters.AddWithValue("@N_GROUP_ID", Guid.Parse(editingId))
                    cmd.Parameters.AddWithValue("@S_SOP_ID", ddlPopupCountry.SelectedValue)
                    cmd.Parameters.AddWithValue("@N_GROUP_NAME", txtGroupName.Text)
                    cmd.Parameters.AddWithValue("@N_CATEGORY", txtCategory.Text)
                    cmd.Parameters.AddWithValue("@N_CONDITION_FIELD", ddlConditionField.SelectedValue)
                    cmd.Parameters.AddWithValue("@N_CONDITION_VALUE", txtConditionValue.Text)
                    cmd.Parameters.AddWithValue("@N_ISACTIVE", Boolean.Parse(rblIsActive.SelectedValue))
                    cmd.ExecuteNonQuery()
                End Using
            End If
        End Using

        hfEditGroupId.Value = String.Empty
        BindNotificationData()
        wndGroupPopup.VisibleOnPageLoad = False
    End Sub


    Protected Sub ddlCountry_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindNotificationData()
        UpdatePanelGroups.Update()
        ScriptManager.RegisterStartupScript(UpdatePanelGroups, UpdatePanelGroups.GetType(), "BindNotificationTable", "BindNotificationTable();", True)
    End Sub

    Protected Sub ddlEnvironment_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindNotificationData()
        UpdatePanelGroups.Update()
        ScriptManager.RegisterStartupScript(UpdatePanelGroups, UpdatePanelGroups.GetType(), "BindNotificationTable", "BindNotificationTable();", True)
    End Sub

End Class
