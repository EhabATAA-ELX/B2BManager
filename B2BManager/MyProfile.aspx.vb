
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.IO

Partial Class MyProfile
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If user IsNot Nothing Then
            If Not IsPostBack Then
                userImage.ImageUrl = "~/GetUserPicture.ashx?uid=" + user.GlobalID.ToString()
                renderControls(user)
                ' Check if user has access to Insights and hide the tab if not
                If user.Tools.Where(Function(fc) fc.ToolID = 15 And fc.TypeID = 1).Count() = 0 Then
                    RadTabStrip1.Tabs.FindTabByValue("InsightsPreferences").Visible = False
                    RadPageViewInsightsSettings.Visible = False
                End If
                ' Check if user has access to Export Builder or B2B Accounts or B2B Translations or B2B Specifications or Pending Orders or B2B Contacts and hide the tab if not
                If user.Tools.Where(Function(fc) (fc.ToolID = 13 Or fc.ToolID = 14 Or fc.ToolID = 16 Or fc.ToolID = 17 Or fc.ToolID = 35 Or fc.ToolID = 42) And fc.TypeID = 1).Count() = 0 Then
                    RadTabStrip1.Tabs.FindTabByValue("EbusinessPreferences").Visible = False
                    RadPageViewEbusinessSettings.Visible = False
                End If

            Else
                Dim __EVENTTARGET As String = Request("__EVENTTARGET")
                Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
                If "Delete".Equals(__EVENTARGUMENT) Then
                    user.Links = ClsHelper.GetLinks(user.ID)
                End If
                DirectCast(Master.FindControl("UpdatePanelSecondMenuItems"), UpdatePanel).Update()
            End If
            RenderLinks(user)
        End If

    End Sub

    Private Sub renderControls(user As ClsUser, Optional labelsOnly As Boolean = False)
        loginLbl.Text = user.Login
        FirstNameLbl.Text = user.FirstName
        lastNameLbl.Text = user.LastName
        emailHyperLink.InnerText = user.Email
        userFullNameLbl.InnerText = user.FullName
        nickNameLbl.InnerText = user.NickName
        If Not labelsOnly Then
            txtBoxFirstName.Text = user.FirstName
            txtBoxLastName.Text = user.LastName
            txtBoxNickName.Text = user.NickName
            txtBoxEmail.Text = user.Email
            If user.Title > 0 Then
                For Each item As ListItem In ddlTitle.Items
                    If item.Value = user.Title.ToString() Then
                        ddlTitle.SelectedValue = user.Title
                        Exit For
                    End If
                Next
            End If
        End If
        If Not String.IsNullOrEmpty(user.Email) Then
            If user.Email.Contains("@") Then
                emailHyperLink.Attributes.Add("href", "mailto:" + user.Email)
            End If
        End If
    End Sub

    Protected Sub uploadButton_Click(sender As Object, e As EventArgs)
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If user IsNot Nothing Then
            Dim fileUploadResult As ClsUsersManagementHelper.FileUploadResult = ClsUsersManagementHelper.UploadUserImage(FileUploadControl, user.GlobalID, user.GlobalID)
            If (fileUploadResult.UploadWithSuccess) Then
                Dim cacheKeyName As String = "UserPicture_" + user.GlobalID.ToString()
                If (Cache(cacheKeyName) IsNot Nothing) Then
                    Cache.Remove(cacheKeyName)
                End If
                Cache.Insert(cacheKeyName, New ClsEbusinessHelper.CustomerLogo(fileUploadResult.FileName, fileUploadResult.ContentType, fileUploadResult.length, fileUploadResult.bytes))
                deleteLogoLbl.Visible = True
                userImage.ImageUrl = "~/GetUserPicture.ashx?uid=" + user.GlobalID.ToString()
            End If
        End If
    End Sub

    Protected Sub btnDeleteLogo_Click(sender As Object, e As EventArgs)
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If user IsNot Nothing Then
            If (ClsUsersManagementHelper.DeleteUserImage(user.GlobalID, user.GlobalID)) Then
                userImage.ImageUrl = "~/Images/ContactImage.png"
                deleteLogoLbl.Visible = False
                If (Cache("UserPicture_" + user.GlobalID.ToString())) IsNot Nothing Then
                    Cache.Remove("UserPicture_" + user.GlobalID.ToString())
                End If
            End If
        End If
    End Sub

    Private Sub RenderLinks(user As ClsUser)
        Dim links As List(Of ClsHelper.Link) = user.Links
        Dim hasAtLeastOnLink As Boolean = False
        If links IsNot Nothing Then
            For Each link As ClsHelper.Link In links
                Dim liContainer As HtmlGenericControl = New HtmlGenericControl("li")
                liContainer.Attributes.Add("data-section-id", link.LinkID)
                liContainer.Attributes.Add("class", "column column-draggable")
                Dim htmlAreaTable As HtmlTable = New HtmlTable()
                htmlAreaTable.Attributes.Add("class", "table-container")
                Dim htmlTr As HtmlTableRow = New HtmlTableRow()
                Dim editTitle As String = "Edit Link"
                Dim deleteTitle As String = "Delete Link"
                Dim htmlEditBtnCell As HtmlTableCell = New HtmlTableCell()
                htmlEditBtnCell.Attributes.Add("class", "action-image")
                htmlEditBtnCell.InnerHtml = "<img src=""Images/edit.png"" title=""" & editTitle & """ draggable=""false"" onclick=""EditLink('" & link.LinkID & "')"" class=""MoreInfoImg"" />"
                Dim htmlDeleteBtnCell As HtmlTableCell = New HtmlTableCell()
                htmlDeleteBtnCell.Attributes.Add("class", "action-image")
                htmlDeleteBtnCell.InnerHtml = "<img src=""Images/delete.png"" title=""" & deleteTitle & """ draggable=""false"" onclick=""DeleteLink('" & link.LinkID & "')"" class=""MoreInfoImg"" />"
                Dim htmlAreaTittleInfo As HtmlTableCell = New HtmlTableCell()
                Dim areaTitleContainer As HtmlTable = New HtmlTable
                Dim areaTitleRow As HtmlTableRow = New HtmlTableRow()
                Dim areaTitle As HtmlTableCell = New HtmlTableCell()
                areaTitle.Attributes.Add("class", "content")
                areaTitle.InnerHtml = link.LinkName
                areaTitleRow.Cells.Add(areaTitle)
                areaTitleContainer.Rows.Add(areaTitleRow)
                htmlAreaTittleInfo.Controls.Add(areaTitleContainer)
                'Add all cells to area table
                htmlTr.Cells.Add(htmlEditBtnCell)
                htmlTr.Cells.Add(htmlDeleteBtnCell)
                htmlTr.Cells.Add(htmlAreaTittleInfo)
                'Add html table row to area table
                htmlAreaTable.Rows.Add(htmlTr)
                'Add html table to LI container
                liContainer.Controls.Add(htmlAreaTable)
                liContainer.Attributes.Add("draggable", "true")
                'Add LI to LU areas
                linksListContainer.Controls.Add(liContainer)
                hasAtLeastOnLink = True
            Next
        End If


        If hasAtLeastOnLink Then
            ScriptManager.RegisterStartupScript(linksUpdatePanel, linksUpdatePanel.GetType(), "InitCells", "InitCells();", True)
        Else
            DragAndDropHeader.Visible = False
            linksListContainer.Visible = False
            btnSaveLinksPositions.Visible = False
        End If
    End Sub
    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Dim stopWatch As Stopwatch = Stopwatch.StartNew()
        Dim errorMsg As String = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        If logonUser IsNot Nothing Then
            parameters.Add(New SqlParameter("@UserID", logonUser.GlobalID))
            parameters.Add(New SqlParameter("@Title", Integer.Parse(ddlTitle.SelectedValue)))
            parameters.Add(New SqlParameter("@FirstName", txtBoxFirstName.Text))
            parameters.Add(New SqlParameter("@LastName", txtBoxLastName.Text))
            parameters.Add(New SqlParameter("@NickName", txtBoxNickName.Text))
            parameters.Add(New SqlParameter("@Email", txtBoxEmail.Text))
            If ClsDataAccessHelper.ExecuteNonQuery("[Administration].[UpdatePreferences]", parameters) Then
                logonUser.FirstName = txtBoxFirstName.Text
                logonUser.LastName = txtBoxLastName.Text
                logonUser.Email = txtBoxEmail.Text
                logonUser.FullName = txtBoxFirstName.Text + " " + txtBoxLastName.Text
                logonUser.NickName = txtBoxNickName.Text
                logonUser.Title = Integer.Parse(ddlTitle.SelectedValue)
                renderControls(logonUser, True)
                ClsHelper.Log("Update Preferences", logonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), stopWatch.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.Log("Update Preferences", logonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), stopWatch.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
    End Sub
End Class
