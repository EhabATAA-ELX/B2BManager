
Imports System.Data
Imports System.Data.SqlClient
Imports Telerik.Web.UI

Partial Class UserGroupProfile
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim Uid As Integer = 0
            If Not String.IsNullOrEmpty(Request.QueryString("Uid")) Then
                Integer.TryParse(Request.QueryString("Uid"), Uid)
            End If
            If Uid > 0 Then
                BindGroupInformation(Uid)
            End If
        End If
    End Sub

    Private Sub BindGroupInformation(uid As Integer)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@Uid", uid))
        Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("[Administration].[GetGroupByID]", parameters)
        If dataSet.Tables.Count = 2 Then
            If dataSet.Tables(0).Rows.Count > 0 Then
                Dim userRow As DataRow = dataSet.Tables(0).Rows(0)
                ImgTooltipHelp_lblGroupID.Text = userRow("U_ID")
                TooltipContentHelp_lblGroupID.InnerHtml = "<b>Group Global ID:</b> " + userRow("U_GLOBALID").ToString()
                lblGroupName.Text = ClsDataAccessHelper.GetText(userRow, "U_LOGIN")
                lblDescription.Text = ClsDataAccessHelper.GetText(userRow, "U_FIRSTNAME")
                CType(GroupInformation.FindControl("txtBoxGroupName"), TextBox).Text = ClsDataAccessHelper.GetText(userRow, "U_LOGIN")
                CType(GroupInformation.FindControl("txtBoxDescription"), TextBox).Text = ClsDataAccessHelper.GetText(userRow, "U_FIRSTNAME")
                CType(GroupInformation.FindControl("RadColorPickerGroup"), RadColorPicker).SelectedColor = System.Drawing.ColorTranslator.FromHtml(ClsDataAccessHelper.GetText(userRow, "U_GROUPUSERCOUNTCOLOR"))
                Dim checkedTools As String() = Nothing
                If dataSet.Tables(1).Rows.Count > 0 Then
                    checkedTools = dataSet.Tables(1).AsEnumerable().Select(Function(r) r.Field(Of String)(0)).ToArray()
                End If
                ClsUsersManagementHelper.FillToolsAndActionsTree(CType(GroupInformation.FindControl("treeToolsAndActions"), RadTreeView), checkedTools)
                CType(Master.FindControl("title"), HtmlTitle).Text = "Edit Group " & ClsDataAccessHelper.GetText(userRow, "U_LOGIN")
                iframeUsersList.Src = String.Format("UserGroupList.aspx?HideHeader=true&Uid=" & uid.ToString())
            End If
        End If
    End Sub
End Class
