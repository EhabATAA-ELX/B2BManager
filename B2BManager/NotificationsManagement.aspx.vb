Imports System.Data
Imports System.Data.SqlClient
Imports ApiClient.ApiCalls
Imports Telerik.Web.UI

Partial Class NotificationsManagement
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then Return

        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then Return

        Dim selectedApplication As ClsHelper.Application = clsUser.Applications.FirstOrDefault(Function(fn) fn.ID = ClsParam.Applications.B2B_REQUESTS)
        For Each environment As ClsHelper.BasicModel In selectedApplication.Environments
            Dim item As New RadComboBoxItem With {
                .Text = environment.Name,
                .Value = environment.ID
            }
            EnvironmentRcb.Items.Add(item)
        Next

        ClsHelper.RenderCountryDropDown(CountryRcb, selectedApplication.Countries, CountryRcb.SelectedValue, False)

        'See table B2B_V2.T_AssignmentTypes
        'AssignmentTypeRcb.Items.AddRange(New List(Of RadComboBoxItem) From {
        '    New RadComboBoxItem With {
        '        .Text = "All",
        '        .Value = "All"
        '    },
        '    New RadComboBoxItem With {
        '        .Text = "Assigned",
        '        .Value = 1
        '    },
        '    New RadComboBoxItem With {
        '        .Text = "Unassigned",
        '        .Value = 0
        '    }
        '})

        'See table B2B_V2.T_ObjectTypes
        ObjectTypeRcb.Items.AddRange(New List(Of RadComboBoxItem) From {
            New RadComboBoxItem With {
                .Text = "All",
                .Value = "All"
            },
            New RadComboBoxItem With {
                .Text = "Article",
                .Value = 2
            },
            New RadComboBoxItem With {
                .Text = "File",
                .Value = 1
            }
        })

        TreatedRcb.Items.AddRange(New List(Of RadComboBoxItem) From {
            New RadComboBoxItem With {
                .Text = "All",
                .Value = "All"
            },
            New RadComboBoxItem With {
                .Text = "Yes",
                .Value = 1
            },
            New RadComboBoxItem With {
                .Text = "No",
                .Value = 0
            }
        })

        DataVisible(Nothing)
    End Sub

    Protected Sub OKbtn_Click(sender As Object, e As EventArgs)
        NeedDatasource()
    End Sub

    Protected Sub NotifMgtRg_OnItemDataBound(sender As Object, e As GridItemEventArgs)
        Dim item = TryCast(e.Item, GridDataItem)
        If item Is Nothing Then Return
        If item.OwnerTableView.Name <> "Assignments" Then Return

        Dim useDateRangeForPublishing = DirectCast(item.GetDataKeyValue("UseDateRangeForPublishing"), Boolean)
        If Not useDateRangeForPublishing Then
            item("PublishedFrom").Text = String.Empty
            item("PublishedTo").Text = String.Empty
        Else
            item("PublishedFrom").Text = Date.Parse(item("PublishedFrom").Text).ToString("dd/MM/yyyy")
            Dim publishedToDate As Date
            item("PublishedTo").Text = If(Date.TryParse(item("PublishedTo").Text, publishedToDate), publishedToDate.ToString("dd/MM/yyyy"), String.Empty)
        End If
    End Sub

    ''' <summary>
    ''' For the expand button of the first table
    ''' </summary>
    Protected Sub NotifMgtRg_ItemCreated(sender As Object, e As GridItemEventArgs)
        If e.Item.OwnerTableView.Name <> "Assignments" Then Return
        Dim item = TryCast(e.Item, GridDataItem)
        If item Is Nothing Then Return
        Dim visibility = item.GetDataKeyValue("Visibility")
        If Not visibility <> "Private" Then Return
        Dim expandCell As TableCell = item("ExpandColumn")
        For Each ctrl As Control In expandCell.Controls
            If Not (TypeOf ctrl Is Button) Then Continue For
            Dim expandButton As Button = DirectCast(ctrl, Button)
            expandButton.ToolTip = "All customers are targeted."
            expandButton.Style("cursor") = "not-allowed"
            expandButton.Style("opacity") = "0.5"
            expandButton.OnClientClick = "return false;"
        Next
    End Sub

    Protected Sub NotifMgtRg_OnDetailTableDataBind(sender As Object, e As GridDetailTableDataBindEventArgs)
        Dim table = e.DetailTableView
        Dim name = table.Name
        Dim parent = table.ParentItem
        Dim res = New DataTable
        If name = "Customers" Then
            Dim visibility = DirectCast(parent.GetDataKeyValue("Visibility"), String)
            If visibility = "Private" Then
                res = GetObjectDetails(parent)
            End If
        ElseIf name = "Users" Then
            Dim grandParent = parent.OwnerTableView.ParentItem
            res = GetCustomerDetails(parent)
            If res.Rows.Count = 0 Then
                Dim treated = DirectCast(grandParent.GetDataKeyValue("Treated"), Boolean)
                table.NoDetailRecordsText = "No user of this customer " + IIf(treated, "have received an email for this assignment.", "able to receive an email.")
            End If
        End If
        table.DataSource = res
    End Sub

    Protected Sub SendNotificationBtn_Click(sender As Object, e As EventArgs)
        Dim selectedObjectIds = GetSelectedObjectIds()
        If selectedObjectIds.Count = 0 Then
            InformationPopup("Select at least one assignment.")
            Return
        End If
        Dim receptionistsNbr = NotificationsApiCalls.GetEmailRecipientsNumber(GetEnvironmentName(), selectedObjectIds)
        If receptionistsNbr < 0 Then
            ErrorPopup()
        Else
            Dim message = IIf(receptionistsNbr = 0, "No", receptionistsNbr.ToString) + " email" + IIf(receptionistsNbr > 1, "s", String.Empty) + " will be sent. Do you want to continue ?"
            OpenPopup(True, message)
        End If
    End Sub

    Protected Sub ConfirmBtn_Click(sender As Object, e As EventArgs)
        Dim sentEmailNbre = NotificationsApiCalls.MailingNotificationByObjectIds(GetEnvironmentName(), GetSelectedObjectIds())
        If sentEmailNbre >= 0 Then
            Dim messageConclusion = IIf(sentEmailNbre = 0, "No", sentEmailNbre.ToString) + " email" + IIf(sentEmailNbre > 1, "s", String.Empty) + " sent."
            InformationPopup(messageConclusion)
            NeedDatasource()
        Else
            ErrorPopup()
        End If
    End Sub

    Private Function GetEnvironmentName() As String
        Return ClsSessionHelper.LogonUser.Applications.FirstOrDefault(Function(fn) fn.ID = ClsParam.Applications.B2B_REQUESTS).Environments.First(Function(x) x.ID = EnvironmentRcb.SelectedValue).Name
    End Function

    Protected Sub EnvironmentRcb_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        DataVisible(Nothing)
    End Sub

    Private Function GetSelectedObjectIds() As List(Of String)
        Dim selectedObjectIds As New List(Of String)()
        For Each item In NotifMgtRg.SelectedItems
            selectedObjectIds.Add(item.GetDataKeyValue("ObjectId"))
        Next
        Return selectedObjectIds
    End Function

    Private Sub NeedDatasource()
        Dim parameters = New List(Of SqlParameter) From
        {
            New SqlParameter("@EnvironmentId", EnvironmentRcb.SelectedValue),
            New SqlParameter("@SOPId", CountryRcb.SelectedValue),
            New SqlParameter("@AssignmentTypeId", 1)
        }
        If Not String.IsNullOrEmpty(SearchCustomerRtb.Text) Then
            parameters.Add(New SqlParameter("@SearchCompany", SearchCustomerRtb.Text))
        End If
        'If AssignmentTypeRcb.SelectedValue <> "All" Then
        '    parameters.Add(New SqlParameter("@AssignmentTypeId", AssignmentTypeRcb.SelectedValue))
        'End If
        If ObjectTypeRcb.SelectedValue <> "All" Then
            parameters.Add(New SqlParameter("@ObjectTypeId", ObjectTypeRcb.SelectedValue))
        End If
        If Not String.IsNullOrEmpty(SearchEmailRtb.Text) Then
            parameters.Add(New SqlParameter("@SearchEmail", SearchEmailRtb.Text))
        End If
        If TreatedRcb.SelectedValue <> "All" Then
            parameters.Add(New SqlParameter("@Treated", TreatedRcb.SelectedValue))
        End If
        If DateFromRdp.SelectedDate IsNot Nothing Then
            parameters.Add(New SqlParameter("@DateFrom", DateFromRdp.SelectedDate))
        End If
        If DateToRdp.SelectedDate IsNot Nothing Then
            parameters.Add(New SqlParameter("@DateTo", DateToRdp.SelectedDate))
        End If
        Dim res = ClsDataAccessHelper.FillDataTable("[Monitoring].[GetAssignmentLogs]", parameters)

        If res.Rows.Count = 0 Then
            Label.Text = "No assignments to display."
        End If
        DataVisible(res)
    End Sub

    Private Sub DataVisible(data As DataTable)
        Dim dataVisible = data IsNot Nothing AndAlso data.Rows.Count > 0
        SendNotificationBtn.Visible = dataVisible
        NotifMgtRg.Visible = dataVisible
        Label.Visible = Not dataVisible

        If Not dataVisible Then
            NotifMgtRg.DataSource = Nothing
        Else
            NotifMgtRg.DataSource = data
        End If
        NotifMgtRg.DataBind()
    End Sub

    Protected Sub NotifMgtRg_PageIndexChanged(sender As Object, e As GridPageChangedEventArgs)
        Dim tableauName = e.Item.OwnerTableView.Name
        If e.Item.OwnerTableView.Name = "Assignments" Then
            NotifMgtRg.CurrentPageIndex = e.NewPageIndex
            NeedDatasource()
            Return
        ElseIf e.Item.OwnerTableView.Name = "Customers" Then
            Dim detailTable As GridTableView = e.Item.OwnerTableView
            Dim objectDetailsData As DataTable = GetObjectDetails(e.Item.OwnerTableView.ParentItem)
            detailTable.DataSource = objectDetailsData
            detailTable.DataBind()
        ElseIf e.Item.OwnerTableView.Name = "Users" Then
            Dim detailTable As GridTableView = e.Item.OwnerTableView
            Dim objectDetailsData As DataTable = GetCustomerDetails(detailTable.ParentItem)
            detailTable.DataSource = objectDetailsData
            detailTable.DataBind()
        End If
    End Sub

    Protected Sub NotifMgtRg_PageSizeChanged(sender As Object, e As GridPageSizeChangedEventArgs)
        NotifMgtRg.CurrentPageIndex = 0
        NeedDatasource()
    End Sub

    Private Function GetObjectDetails(parent As GridDataItem) As DataTable
        Dim objectId = DirectCast(parent.GetDataKeyValue("ObjectId"), String)
        Dim treatedDateKey = parent.GetDataKeyValue("TreatedDate")
        Dim treatedDate As DateTime? = Nothing
        If treatedDateKey IsNot Nothing AndAlso Not DBNull.Value.Equals(treatedDateKey) AndAlso TypeOf treatedDateKey Is DateTime Then
            treatedDate = CType(treatedDateKey, DateTime)
        End If
        Dim parameters = New List(Of SqlParameter) From
        {
            New SqlParameter("@EnvironmentId", EnvironmentRcb.SelectedValue),
            New SqlParameter("@ObjectId", objectId),
            New SqlParameter("@TreatedDate", treatedDate)
        }
        If Not String.IsNullOrEmpty(SearchCustomerRtb.Text) Then
            parameters.Add(New SqlParameter("@SearchCompany", SearchCustomerRtb.Text))
        End If
        If Not String.IsNullOrEmpty(SearchEmailRtb.Text) Then
            parameters.Add(New SqlParameter("@SearchEmail", SearchEmailRtb.Text))
        End If
        If DateFromRdp.SelectedDate IsNot Nothing Then
            parameters.Add(New SqlParameter("@DateFrom", DateFromRdp.SelectedDate))
        End If
        If DateToRdp.SelectedDate IsNot Nothing Then
            parameters.Add(New SqlParameter("@DateTo", DateToRdp.SelectedDate))
        End If
        Dim res = ClsDataAccessHelper.FillDataTable("[Monitoring].[GetCompaniesByObjectId]", parameters)
        Return res
    End Function

    Private Function GetCustomerDetails(parent As GridDataItem) As DataTable
        Dim grandParent = parent.OwnerTableView.ParentItem
        Dim objectId = DirectCast(grandParent.GetDataKeyValue("ObjectId"), String)
        Dim treatedDateKey = parent.GetDataKeyValue("TreatedDate")
        Dim treatedDate As DateTime? = Nothing
        If treatedDateKey IsNot Nothing AndAlso Not DBNull.Value.Equals(treatedDateKey) AndAlso TypeOf treatedDateKey Is DateTime Then
            treatedDate = CType(treatedDateKey, DateTime)
        End If
        Dim C_GLOBALID = DirectCast(parent.GetDataKeyValue("C_GLOBALID"), Guid)
        Dim parameters = New List(Of SqlParameter) From
        {
            New SqlParameter("@EnvironmentId", EnvironmentRcb.SelectedValue),
            New SqlParameter("@ObjectId", objectId),
            New SqlParameter("@C_GLOBALID", C_GLOBALID),
            New SqlParameter("@TreatedDate", treatedDate)
        }
        If Not String.IsNullOrEmpty(SearchEmailRtb.Text) Then
            parameters.Add(New SqlParameter("@SearchEmail", SearchEmailRtb.Text))
        End If
        Dim res = ClsDataAccessHelper.FillDataTable("[Monitoring].[GetUsersCompanyByObjectId]", parameters)
        Return res
    End Function

    Private Sub ErrorPopup()
        InformationPopup("System problem, please contact administration.")
    End Sub

    Private Sub InformationPopup(message As String)
        OpenPopup(False, message)
    End Sub

    Private Sub OpenPopup(isConfirmation As Boolean, message As String)
        Dim commande = IIf(isConfirmation, "OpenConfirmationRw", "OpenInfoRw") + "('" + message.Replace("'", "\'") + "');"
        Dim script =
            "Sys.Application.add_load(function() {" +
            commande +
            "});"
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "OpenRw", script, True)
    End Sub
End Class