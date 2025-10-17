
Imports System.Data
Imports System.Data.SqlClient
Imports Telerik.Web.UI

Partial Class UserProfile
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim Uid As Guid = Guid.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("uid")) Then
                Guid.TryParse(Request.QueryString("uid"), Uid)
            End If
            If Uid <> Guid.Empty Then
                BindUserInformation(Uid)
            End If
        End If
    End Sub


    Private Sub BindUserInformation(Uid As Guid)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@UID", Uid))
        Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("[Administration].[GetUserByID]", parameters)
        If dataSet.Tables.Count = 4 Then
            If dataSet.Tables(0).Rows.Count > 0 Then

                Dim userRow As DataRow = dataSet.Tables(0).Rows(0)
                Dim userTags As String = String.Empty
                If userRow("U_ISACTIVE") Then
                    userTags += FormatUserTag("Active", "#3bbd5d")
                Else
                    userTags += FormatUserTag("Locked", "#df5b5b")
                End If
                Select Case userRow("UserType")
                    Case 0
                        userTags += FormatUserTag("Disabled", "#e6e6e6")
                    Case 2
                        userTags += FormatUserTag("Nearly Expired", "#e67e22")
                    Case 3
                        userTags += FormatUserTag("Expired", "#e85656")
                End Select
                If userRow("HasPicture") Then
                    userImage.ImageUrl = "~/GetUserPicture.ashx?uid=" + userRow("U_GLOBALID").ToString()
                    deleteLogoLbl.Visible = True
                Else
                    userImage.ImageUrl = "~/Images/ContactImage.png"
                End If

                ImgTooltipHelp_lblUserID.Text = userRow("U_ID").ToString()
                lblLoginName.Text = ClsDataAccessHelper.GetText(userRow, "U_LOGIN")
                lblDisplayName.Text = ClsDataAccessHelper.GetText(userRow, "U_FULLNAME")
                lblEmail.Text = ClsDataAccessHelper.GetText(userRow, "U_EMAIL")
                TooltipContentHelp_lblUserID.InnerHtml = "<b>User Global ID:</b> " + userRow("U_GLOBALID").ToString()
                CType(UserInformation.FindControl("txtBoxFirstName"), TextBox).Text = ClsDataAccessHelper.GetText(userRow, "U_FIRSTNAME")
                CType(UserInformation.FindControl("txtBoxLastName"), TextBox).Text = ClsDataAccessHelper.GetText(userRow, "U_LASTNAME")
                CType(UserInformation.FindControl("txtBoxLogin"), TextBox).Text = ClsDataAccessHelper.GetText(userRow, "U_LOGIN")
                CType(UserInformation.FindControl("txtBoxPassword"), TextBox).Text = ClsDataAccessHelper.GetText(userRow, "U_PASSWORD")
                CType(UserInformation.FindControl("txtBoxNickName"), TextBox).Text = ClsDataAccessHelper.GetText(userRow, "U_NICKNAME")
                CType(UserInformation.FindControl("txtBoxEmail"), TextBox).Text = ClsDataAccessHelper.GetText(userRow, "U_Email")
                CType(UserInformation.FindControl("chkBoxActive"), CheckBox).Checked = userRow("U_ISACTIVE")
                CType(UserInformation.FindControl("chkBoxProductionAccess"), CheckBox).Checked = userRow("AccessProductionEnvironment")
                CType(UserInformation.FindControl("chkBoxStagingAccess"), CheckBox).Checked = userRow("AccessStagingEnvironment")
                Dim expirationDate As DateTime = Nothing
                If userRow("EXPIRE_DATE_PASSWORD") IsNot DBNull.Value Then
                    expirationDate = userRow("EXPIRE_DATE_PASSWORD")
                    CType(UserInformation.FindControl("radDateTimeExpirationDate"), RadDateTimePicker).SelectedDate = expirationDate
                End If

                Dim lastConnectionDate As DateTime = Nothing
                If userRow("U_LASTCONNECTEDON") IsNot DBNull.Value Then
                    lastConnectionDate = userRow("U_LASTCONNECTEDON")
                    lblLastConnectionDate.Text = lastConnectionDate.ToString("dd/MM/yyyy HH:mm")
                Else
                    lblLastConnectionDate.Text = "--"
                End If

                If lastConnectionDate = Nothing Then
                    userTags += FormatUserTag("Never visited", "#13194e")
                Else
                    If lastConnectionDate > DateTime.Now.AddDays(-1) Then
                        userTags += FormatUserTag("Visited in last 24h", "#8cd874")
                    ElseIf lastConnectionDate > DateTime.Now.AddDays(-7) Then
                        userTags += FormatUserTag("Visited this week", "#b91169")
                    ElseIf lastConnectionDate > DateTime.Now.AddDays(-30) Then
                        userTags += FormatUserTag("Visited this month", "#dd4f42")
                    ElseIf lastConnectionDate.Year = DateTime.Now.Year Then
                        userTags += FormatUserTag("Visited this year", "#e39800")
                    Else
                        userTags += FormatUserTag("Old visitor", "#6e6e73")
                    End If
                End If
                CType(UserInformation.FindControl("txtBoxPassword"), TextBox).CssClass = "Electrolux_light width230px hidden"
                CType(Master.FindControl("title"), HtmlTitle).Text = "Edit User " & ClsDataAccessHelper.GetText(userRow, "U_FULLNAME")
                Dim checkedSOPs As String() = Nothing
                Dim countriesCount As Integer = dataSet.Tables(1).Rows.Count
                If countriesCount > 0 Then
                    checkedSOPs = dataSet.Tables(1).AsEnumerable().Select(Function(r) r.Field(Of String)(0)).ToArray()
                    If countriesCount >= 30 Then
                        userTags += FormatUserFlagTag("All countries", "#7b7b7b", "Images\Flags\EU.png")
                    Else
                        For Each countryRow As DataRow In dataSet.Tables(1).Rows
                            userTags += FormatUserFlagTag(countryRow("Name"), "#578a9d", countryRow("ImageUrl"))
                        Next
                    End If
                End If
                ClsUsersManagementHelper.FillUserCountriesTree(CType(UserInformation.FindControl("treeCountries"), RadTreeView), False, checkedSOPs)
                If dataSet.Tables(2).Rows.Count > 0 Then
                    ClsEbusinessHelper.SetValueCheckBoxList(CType(UserInformation.FindControl("userGroupBoxList"), CheckBoxList), dataSet.Tables(2).AsEnumerable().Select(Function(r) r.Field(Of String)(0)).ToArray())
                End If
                UserTagsTD.InnerHtml = userTags
                Dim checkedTools As String() = Nothing
                If dataSet.Tables(3).Rows.Count > 0 Then
                    checkedTools = dataSet.Tables(3).AsEnumerable().Select(Function(r) r.Field(Of String)(0)).ToArray()
                End If
                ClsUsersManagementHelper.FillToolsAndActionsTree(treeToolsAndActions, checkedTools, False)
            End If
        End If
    End Sub

    Private Function FormatUserTag(TagName As String, BackgroundColor As String) As String
        Return String.Format("<span style='background-color:{0};' class='userTag'>{1}</span>", BackgroundColor, TagName)
    End Function

    Protected Sub uploadButton_Click(sender As Object, e As EventArgs)
        Dim Uid As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("uid")) Then
            Guid.TryParse(Request.QueryString("uid"), Uid)
        End If
        If Uid <> Guid.Empty Then
            Dim fileUploadResult As ClsUsersManagementHelper.FileUploadResult = ClsUsersManagementHelper.UploadUserImage(FileUploadControl, Uid, ClsSessionHelper.LogonUser.GlobalID)
            If (fileUploadResult.UploadWithSuccess) Then
                Dim cacheKeyName As String = "UserPicture_" + Uid.ToString()
                If (Cache(cacheKeyName) IsNot Nothing) Then
                    Cache.Remove(cacheKeyName)
                End If
                Cache.Insert(cacheKeyName, New ClsEbusinessHelper.CustomerLogo(fileUploadResult.FileName, fileUploadResult.ContentType, fileUploadResult.length, fileUploadResult.bytes))
                deleteLogoLbl.Visible = True
                userImage.ImageUrl = "~/GetUserPicture.ashx?uid=" + Uid.ToString()
            End If
        End If
    End Sub

    Protected Sub btnDeleteLogo_Click(sender As Object, e As EventArgs)
        Dim Uid As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("uid")) Then
            Guid.TryParse(Request.QueryString("uid"), Uid)
        End If
        If Uid <> Guid.Empty Then
            If (ClsUsersManagementHelper.DeleteUserImage(Uid, ClsSessionHelper.LogonUser.GlobalID)) Then
                userImage.ImageUrl = "~/Images/ContactImage.png"
                deleteLogoLbl.Visible = False
                If (Cache("UserPicture_" + Uid.ToString())) IsNot Nothing Then
                    Cache.Remove("UserPicture_" + Uid.ToString)
                End If
            End If
        End If
    End Sub

    Private Function FormatUserFlagTag(CountryName As String, BackgroundColor As String, ImageUrl As String) As String
        Return String.Format("<span  style='background-color:{0};' class='userTag'><img src='{2}' style='height: 14px;width: 18px;margin-right: 4px;'>{1}</span>", BackgroundColor, CountryName, ImageUrl)
    End Function


End Class
