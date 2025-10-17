
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class MasterPage
    Inherits System.Web.UI.MasterPage

    Protected Sub LogOff()
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim currentUrl = HttpContext.Current.Request.Url.AbsoluteUri
        Dim userGlobalID As String = Guid.Empty.ToString()

        Try
            If ClsSessionHelper.LogonUser IsNot Nothing Then
                userGlobalID = ClsSessionHelper.LogonUser.GlobalID.ToString()
            End If
            If Cache IsNot Nothing Then
                For Each c As DictionaryEntry In Cache
                    If c.Key.ToString().Contains(userGlobalID) Or c.Key.ToString().Contains(Session.SessionID) Then
                        Cache.Remove(c.Key.ToString())
                    End If
                Next
            End If
        Catch ex As Exception
            ClsSendEmailHelper.SendErrorEmail(ex.Message)
        End Try
        FormsAuthentication.SignOut()
        Session.Clear()
        watch.Stop()
        ClsHelper.Log("Sign out", userGlobalID, currentUrl, watch.ElapsedMilliseconds, False, Nothing)
        Response.Redirect("Login.aspx", True)
    End Sub

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If ClsSessionHelper.LogonUser Is Nothing Then
            Dim returnUrl As String = Server.UrlEncode(Request.Url.PathAndQuery)
            If Not String.IsNullOrEmpty(returnUrl) Then
                If Not returnUrl.ToLower().Contains("login.aspx") Then
                    Response.Redirect(String.Format("Login.aspx?ReturnUrl={0}", returnUrl), True)
                End If
            End If
        Else
            previewModeDiv.Visible = ClsSessionHelper.LogonUser.PreviewedBy <> Guid.Empty
            userImage.ImageUrl = "~/GetUserPicture.ashx?uid=" + ClsSessionHelper.LogonUser.GlobalID.ToString()
        End If

        If Not IsPostBack Then
            Dim provider As ClsSiteMapProvider = New ClsSiteMapProvider()
            provider.BuildSiteMap()
            If provider.IsInitialized Then 'if not initialized use by default the Web.sitmap file
                SiteMapPath1.Provider = provider
            End If
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim HideHeader As Boolean = False
        Boolean.TryParse(Request.QueryString("HideHeader"), HideHeader)

        Dim IsAuthorized = False
        Dim Tools As List(Of ClsHelper.Tool) = ClsSessionHelper.LogonUser.Tools
        Dim absolutepath As String = Request.Url.AbsolutePath.Replace("/", "").ToLower()
        For Each tool In Tools
            If absolutepath.Contains(tool.Url.Replace("/", "").ToLower()) _
                Or absolutepath.Contains("myprofile.aspx") _
                Or absolutepath.Contains("404.aspx") _
                Or absolutepath.Contains("home.aspx") _
                Or absolutepath.Contains("unauthorized.aspx") Then
                IsAuthorized = True
                Exit For
            End If
        Next
        Dim currentPage As String = System.IO.Path.GetFileName(Request.Url.AbsolutePath)
        Dim currentTool As ClsHelper.Tool = ClsHelper.FindToolByUrl(Tools, currentPage)
        Dim currentToolID As Integer = 0
        Dim isFavorite As Boolean = False
        If currentTool IsNot Nothing Then
            currentToolID = currentTool.ToolID
            isFavorite = currentTool.IsFavorite
        End If
        If Not IsAuthorized Then
            Response.Redirect("Unauthorized.aspx", True)
        End If

        If Request("__EVENTTARGET") = "LogOff" Then
            LogOff()
        End If

        If HideHeader Then
            headerPanel.Visible = False
        Else
            If Request("__EVENTTARGET") = "ChangePageOptionClicker" Then
                Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
                Dim toolID As Integer = 0
                Dim args As String() = __EVENTARGUMENT.Split("|")
                If args.Length = 2 Then
                    Integer.TryParse(args(1), toolID)
                    Dim user As ClsUser = ClsSessionHelper.LogonUser
                    Select Case args(0)
                        Case "ChangeDefault"
                            user.HomePageToolID = toolID
                            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
                            parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.ID))
                            parameters.Add(New SqlParameter("@ToolID", toolID))
                            Dim result As String = ClsDataAccessHelper.ExecuteScalar("Administration.ChangeHomePage", parameters).ToString()
                            Exit Select
                        Case "MarkPageAsFavorite"
                            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
                            parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.ID))
                            parameters.Add(New SqlParameter("@ToolID", IIf(toolID > 0, toolID, currentToolID)))
                            isFavorite = CBool(ClsDataAccessHelper.ExecuteScalar("Administration.ChangePageFavoriteStatus", parameters))
                            For i As Integer = 0 To Tools.Count - 1
                                If Tools(i).ToolID = IIf(toolID > 0, toolID, currentToolID) Then
                                    Tools(i).IsFavorite = isFavorite
                                    Exit For
                                End If
                            Next
                            user.Tools = Tools
                            ClsSessionHelper.LogonUser = user
                            UpdatePanelMenu.Update()
                            Exit Select
                    End Select
                End If
                ChangePageOptionUpdatePanel.Update()
            End If
            If Request("__EVENTTARGET") = "QuitPreviewModeClicker" Then
                Dim watch As Stopwatch = Stopwatch.StartNew()
                Dim user As ClsUser = ClsHelper.ValidateUser("", "", ClsSessionHelper.LogonUser.PreviewedBy.ToString())
                Dim strRedirect As String = "UsersManagement.aspx"
                Dim userName As String = String.Empty
                Dim GlobalID As String = Guid.NewGuid().ToString()
                If user IsNot Nothing Then
                    Dim Actions As List(Of ClsHelper.ActionDesignation) = Nothing
                    Tools = Nothing
                    Dim Links As List(Of ClsHelper.Link) = Nothing
                    user.Applications = ClsHelper.GetUserInformation(user.ID, Tools, Actions, Links)
                    user.Actions = Actions
                    user.Tools = Tools
                    user.Links = Links
                    userName = user.Login
                    ClsSessionHelper.LogonUser = user
                    FormsAuthentication.RedirectFromLoginPage(user.Login, True)
                    GlobalID = user.GlobalID.ToString()
                End If
                watch.Stop()
                ClsHelper.Log("Quit connect as mode", GlobalID, String.Format("<b>Username:</b> {0}</br><b>Previewed user id:</b> {1}", userName, GlobalID), watch.ElapsedMilliseconds, user Is Nothing, IIf(user Is Nothing, "Unable to connect as user", Nothing))
                If user IsNot Nothing Then
                    Response.Redirect(strRedirect, True)
                End If
            End If
        End If
        If currentToolID > 0 Then
            If isFavorite Then
                SetPageAsFavorite.Attributes.Add("title", "Remove from favorites")
                SetPageAsFavorite.Attributes.Add("onclick", "MarkPageAsFavorite(0)")
                SetPageAsFavorite.Attributes.Add("style", "color:#FFD700")
                SetPageAsFavorite.Attributes.Add("class", "fas fa-star")
            Else
                SetPageAsFavorite.Attributes.Add("title", "Add to favorites")
                SetPageAsFavorite.Attributes.Add("onclick", String.Format("MarkPageAsFavorite({0})", currentToolID))
                SetPageAsFavorite.Attributes.Add("style", "color:#031d4e")
                SetPageAsFavorite.Attributes.Add("class", "far fa-star")
            End If

            If currentToolID = ClsSessionHelper.LogonUser.HomePageToolID Then
                MarkAsDefaultPage.Attributes.Add("title", "Reset default page")
                MarkAsDefaultPage.Attributes.Add("onclick", "ChangeDefaultPage(0)")
                MarkAsDefaultPage.Attributes.Add("style", "color:#2196f3")
                MarkAsDefaultPage.Attributes.Add("class", "fas fa-thumbtack")
            Else
                MarkAsDefaultPage.Attributes.Add("title", "Mark as default page")
                MarkAsDefaultPage.Attributes.Add("onclick", String.Format("ChangeDefaultPage({0})", currentToolID))
                MarkAsDefaultPage.Attributes.Add("style", "color:#031d4e")
                MarkAsDefaultPage.Attributes.Add("class", "fas fa-thumbtack fa-rotate-90")
            End If
        End If
    End Sub

    Public Function GetMenuItems() As String
        Dim MenuHTML As String = ""
        If ClsSessionHelper.LogonUser IsNot Nothing Then
            Try
                Dim Tools As List(Of ClsHelper.Tool) = ClsSessionHelper.LogonUser.Tools
                Dim currentPage As String = System.IO.Path.GetFileName(Request.Url.AbsolutePath)
                Dim currentTool As ClsHelper.Tool = ClsHelper.FindToolByUrl(Tools, currentPage)
                Dim currentToolID As Integer = 0
                If currentTool IsNot Nothing Then
                    currentToolID = currentTool.ToolID
                End If

                Dim root = ClsHelper.GenerateTree(Tools.Where(Function(fn) fn.TypeID = 1 Or fn.TypeID = 3), Function(fc) fc.ToolID, Function(fc) fc.ParentToolID, Nothing)
                Dim index As Integer = 0
                Dim favorites As List(Of ClsHelper.Tool) = New List(Of ClsHelper.Tool)()
                For Each treeItem As ClsHelper.TreeItem(Of ClsHelper.Tool) In root
                    index += 1
                    Dim itemHtmlString As String = ""
                    Dim selectParent As Boolean = False
                    AddToolItem(itemHtmlString, treeItem, selectParent, currentToolID, favorites)
                    MenuHTML += itemHtmlString.Replace("[menuOpenClass]", IIf(selectParent, "menu-open", "")).Replace("[activeClass]", IIf(selectParent, "active", ""))
                    itemHtmlString = Nothing
                Next
                If favorites.Count > 0 Then
                    MenuHTML += "<li class=""nav-header"">FAVORITES</li>"
                    For Each tool As ClsHelper.Tool In favorites
                        Dim itemHtmlString As String = ""
                        AddFavorite(itemHtmlString, tool, currentToolID)
                        MenuHTML += itemHtmlString
                        itemHtmlString = Nothing
                    Next
                End If

            Catch ex As Exception
                MenuHTML = "<li><a href=""Default.aspx"" class=""selected"">Log Viewer</a></li>"
            End Try
        End If
        Return MenuHTML
    End Function

    Public Function GetSecondMenuItems() As String
        Dim SecondMenuHTML As String = ""
        If ClsSessionHelper.LogonUser IsNot Nothing Then
            Try
                Dim Tools As List(Of ClsHelper.Tool) = ClsSessionHelper.LogonUser.Tools
                Dim root = ClsHelper.GenerateTree(Tools.Where(Function(fn) fn.TypeID = 3), Function(fc) fc.ToolID, Function(fc) fc.ParentToolID, Nothing)
                For Each treeItem As ClsHelper.TreeItem(Of ClsHelper.Tool) In root
                    SecondMenuHTML += GetSecondMenuItem(treeItem.Item.MenuIconImagePath, treeItem.Item.Name, "GoTo('" & treeItem.Item.Url & "')")
                Next
                If ClsSessionHelper.LogonUser.Links IsNot Nothing Then
                    SecondMenuHTML += "<li class=""nav-header-custom"" style=""color:#fff !important"">MY CUSTOM LINKS</li>"
                    For Each linkItem As ClsHelper.Link In ClsSessionHelper.LogonUser.Links
                        SecondMenuHTML += GetLinkMenuItem(linkItem.LinkName, linkItem.LinkUrl, linkItem.LinkIconColor)
                    Next
                End If

            Catch ex As Exception
                SecondMenuHTML = ""
            End Try
        End If
        Return SecondMenuHTML
    End Function

    Public Function GetSecondMenuItem(IconImageUrl As String, ItemName As String, Optional ClickAction As String = "") As String
        Return String.Format("<li class=""nav-item""><a class=""nav-link"" {0} style=""cursor: pointer;color:#fff !important""><table><tr><td><img src=""{1}"" style=""width: 20px !important; height: 20px;"" /></td><td>{2}</td></tr></table></a></li>",
                             IIf(ClickAction.Equals(""), "", "onclick=""" & ClickAction & """"),
                             IconImageUrl,
                             ItemName)
    End Function

    Public Function GetLinkMenuItem(LinkName As String, linkUrl As String, LinkColor As String) As String
        Return String.Format("<li class=""nav-item""><a class=""nav-link"" href=""{0}"" target=""_blank"" style=""cursor: pointer;color:#fff !important"" ><table><tr><td><i style=""color:{1}"" class=""far fa-circle nav-icon""></i></td><td>{2}</td></tr></table></a></li>",
                             linkUrl,
                             LinkColor,
                             LinkName)
    End Function

    Private Sub AddToolItem(ByRef HtmlString As String, item As ClsHelper.TreeItem(Of ClsHelper.Tool), ByRef selectParent As Boolean, currentToolID As Integer, ByRef favorites As List(Of ClsHelper.Tool), Optional ignoreItem As Boolean = False)
        Try
            selectParent = selectParent Or item.Item.ToolID = currentToolID
            If item.Item.IsFavorite Then
                favorites.Add(item.Item)
            End If
            If item.Item.TypeID = 1 AndAlso Not ignoreItem Then
                HtmlString += String.Format("<li {1}>" & vbNewLine, item.Item.Url, IIf(item.Children.Count > 0, "class=""nav-item [menuOpenClass] has-treeview """, "class=""nav-item"""))
                HtmlString += String.Format("<a class=""nav-link {3}"" href=""{2}""><i class=""nav-icon""><img src=""{0}"" height=""20"" width=""20""/></i><p>{1}{4}</p></a>" & vbNewLine, item.Item.IconImagePath,
                                                                                        item.Item.Name + IIf(item.Children.Count > 0,
                                                                                        "<i class=""right fas fa-angle-left""></i>", ""),
                                                                                        IIf(item.Children.Count > 0, "#", item.Item.Url),
                                                                                        IIf(item.Item.ToolID = currentToolID, "active", IIf(item.Children.Count > 0, "[activeClass]", "")),
                                                                                        IIf(String.IsNullOrEmpty(item.Item.BadgeText) Or String.IsNullOrEmpty(item.Item.BadgeColor), "", "<span class=""right badge"" style=""background-color:" + item.Item.BadgeColor + """>" + item.Item.BadgeText + "</span>")
                                                                                        )
                If item.Children.Count > 0 Then
                    HtmlString += "<ul class=""nav nav-treeview"">" & vbNewLine
                    For Each childItem As ClsHelper.TreeItem(Of ClsHelper.Tool) In item.Children
                        AddToolItem(HtmlString, childItem, selectParent, currentToolID, favorites)
                    Next
                    HtmlString += "</ul>" & vbNewLine
                End If
            Else
                If item.Children.Count > 0 Then
                    For Each childItem As ClsHelper.TreeItem(Of ClsHelper.Tool) In item.Children
                        AddToolItem(HtmlString, childItem, selectParent, currentToolID, favorites, True)
                    Next
                End If
            End If
            HtmlString += "</li>"
        Catch ex As Exception
            HtmlString = ""
        End Try
    End Sub

    Private Sub AddFavorite(ByRef HtmlString As String, item As ClsHelper.Tool, currentToolID As Integer)
        Try
            HtmlString += String.Format("<li {1}>" & vbNewLine, item.Url, "class=""nav-item""")
            HtmlString += String.Format("<span class=""nav-link""><a class=""{3}"" href=""{2}""><i class=""nav-icon""><img src=""{0}"" height=""20"" width=""20""/></i><p>{1}</p></a><i title=""Remove from favorites"" class=""right fas fa-ban"" style=""color:red;cursor:pointer;font-size:16px"" onclick=""MarkPageAsFavorite({4})""></i></span>" & vbNewLine, item.IconImagePath,
                                                                                    item.Name,
                                                                                    item.Url,
                                                                                    IIf(item.ToolID = currentToolID, "active", ""),
                                                                                    item.ToolID
                                                                                    )
            HtmlString += "</li>"
        Catch ex As Exception
            HtmlString = ""
        End Try
    End Sub
    Public Function GetConnectedUser() As String
        Dim loginName As String = ""
        If ClsSessionHelper.LogonUser IsNot Nothing Then
            loginName = IIf(String.IsNullOrWhiteSpace(ClsSessionHelper.LogonUser.FullName), IIf(String.IsNullOrWhiteSpace(ClsSessionHelper.LogonUser.FirstName), ClsSessionHelper.LogonUser.Login, ClsSessionHelper.LogonUser.FirstName), ClsSessionHelper.LogonUser.FullName)
        End If
        Return loginName
    End Function

End Class

