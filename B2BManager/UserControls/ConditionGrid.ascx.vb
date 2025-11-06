Imports System.Data
Imports System.Linq
Imports System
Imports Telerik.Web.UI

Partial Class UserControls_ConditionGrid
    Inherits System.Web.UI.UserControl

    Public Property PageSource As String
    Public Property DocumentGuidString As String
    Public Property FocusRangeId As Guid?
    Public Property EnvironmentID As String
    Public Property SopName As String

    Protected Sub Rg_ItemDataBound(sender As Object, e As GridItemEventArgs)
        Dim item = TryCast(e.Item, GridDataItem)
        If item Is Nothing Then Return

        Dim conditionName As String = DataBinder.Eval(item.DataItem, "ConditionName").ToString()
        item.ToolTip = conditionName

        Dim urlBase = String.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority)
        Dim sopNameLocal As String = If(item.GetDataKeyValue("SOPName") IsNot Nothing, item.GetDataKeyValue("SOPName").ToString(), String.Empty)

        Dim conditionIdValue As Object = item.GetDataKeyValue("ConditionID")

        Dim conditionId As Guid = Guid.Empty
        If conditionIdValue IsNot Nothing AndAlso Not DBNull.Value.Equals(conditionIdValue) Then
            conditionId = CType(conditionIdValue, Guid)
        End If

        Dim isStatic As Boolean = False
        Dim isStaticObj = item.GetDataKeyValue("ConditionIsStatic")
        If isStaticObj IsNot Nothing AndAlso Not DBNull.Value.Equals(isStaticObj) Then
            isStatic = CType(isStaticObj, Boolean)
        End If

        If isStatic Then
            item.Style("background-color") = "#F0F8FF"
            item.Style("font-weight") = "bold"
        End If

        Dim documenIds As String = String.Empty
        If Not String.IsNullOrWhiteSpace(DocumentGuidString) Then
            documenIds = DocumentGuidString
        ElseIf FocusRangeId.HasValue Then
            documenIds = FocusRangeId.ToString()
        End If

        Dim editConditionTokenData As New TokenData(Request.QueryString("envid"), sopNameLocal, isStatic, "edit", "", documenIds, "", conditionId)
        Dim token As String = ClsTokenHelper.GenerateToken(editConditionTokenData)

        Dim editConditionBtn = TryCast(item.FindControl("EditConditionBtn"), LinkButton)
        If editConditionBtn IsNot Nothing Then
            If isStatic Then
                editConditionBtn.OnClientClick = String.Concat("window.parent.openStaticAssignmentPopup('", urlBase, ResolveUrl("~/QueryBuilderStaticAssignment.aspx"), "?QueryBuilderToken=", token, "'); return false;")
                item("CriteriaCount").Text = "Manual assignment"
            Else
                editConditionBtn.OnClientClick = String.Concat("window.parent.openInDirectAssignmentPopup('", urlBase, ResolveUrl("~/InDirectAssignment.aspx"), "?QueryBuilderToken=", token, "'); return false;")
            End If
        End If

        Dim deleteBtn = TryCast(item.FindControl("DeleteConditionBtn"), LinkButton)
        If deleteBtn IsNot Nothing Then
            ' wire delete client call to use the condition id
            deleteBtn.OnClientClick = String.Concat("DeleteConditionFocusRange('", conditionId.ToString(), "'); return false;")
        End If
    End Sub

    Protected Sub Rg_NeedDataSource(source As Object, e As GridNeedDataSourceEventArgs)
        Dim conditionsTable As DataTable = Nothing
        Dim staticExists As Boolean = False

        If String.Equals(PageSource, "FilesManager", StringComparison.OrdinalIgnoreCase) Then
            If Not String.IsNullOrWhiteSpace(DocumentGuidString) Then
                Dim parts = DocumentGuidString.Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries)
                Dim idsToSearch As New List(Of Guid)()
                For Each idStr As String In parts
                    Dim parsedGuid As Guid
                    If Guid.TryParse(idStr.Trim(), parsedGuid) Then
                        idsToSearch.Add(parsedGuid)
                    End If
                Next
                If idsToSearch.Count > 0 Then
                    Dim result As ClsDocument = DynamicConditionsHelper.GetConditionsForDocuments(idsToSearch.ToArray())
                    If result IsNot Nothing Then
                        conditionsTable = result.Conditions
                        staticExists = result.StaticConditionExists
                    End If
                End If
            End If
        ElseIf String.Equals(PageSource, "FocusRange", StringComparison.OrdinalIgnoreCase) Then
            If FocusRangeId.HasValue Then
                Dim resultFocusRange As ClsFocusRange = DynamicConditionsHelper.GetConditionsByFocusRange(FocusRangeId.Value)
                If resultFocusRange IsNot Nothing Then
                    conditionsTable = resultFocusRange.Conditions
                    staticExists = resultFocusRange.StaticConditionExists
                End If
            End If
        End If

        If conditionsTable Is Nothing Then
            Rg.DataSource = Nothing
            Dim btnStatic = TryCast(Me.Page.FindControl("AddStaticConditionBtn"), Web.UI.WebControls.Button)
            If btnStatic IsNot Nothing Then btnStatic.Enabled = Not staticExists
            Dim parentLabel = TryCast(Me.Page.FindControl("AssignedCustomersNumberLb"), Label)
            If parentLabel IsNot Nothing Then parentLabel.Text = "0"
            Return
        End If

        Rg.DataSource = conditionsTable

        If conditionsTable.Rows.Count > 0 Then
            Dim conditionIds As New List(Of Guid)()
            For Each row As DataRow In conditionsTable.Rows
                conditionIds.Add(CType(row("ConditionID"), Guid))
            Next

            Dim sopNameLocal As String = conditionsTable.Rows(0)("SOPName").ToString()
            Dim uniqueCustomers As List(Of ClsCustomer) = DynamicConditionsHelper.GetUniqueCustomersForConditions(conditionIds, sopNameLocal)
            Dim parentLabel = TryCast(Me.Page.FindControl("AssignedCustomersNumberLb"), Label)
            If parentLabel IsNot Nothing Then
                parentLabel.Text = uniqueCustomers.Count.ToString()
            End If
            Dim btnStatic = TryCast(Me.Page.FindControl("AddStaticConditionBtn"), Web.UI.WebControls.Button)
            If btnStatic IsNot Nothing Then btnStatic.Enabled = Not staticExists
        Else
            Dim parentLabel = TryCast(Me.Page.FindControl("AssignedCustomersNumberLb"), Label)
            If parentLabel IsNot Nothing Then
                parentLabel.Text = "0"
            End If
            Dim btnStatic = TryCast(Me.Page.FindControl("AddStaticConditionBtn"), Web.UI.WebControls.Button)
            If btnStatic IsNot Nothing Then btnStatic.Enabled = True
        End If
    End Sub

    Public Sub RebindConditions()
        Rg.Rebind()
    End Sub
End Class