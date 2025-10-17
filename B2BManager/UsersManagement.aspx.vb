
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Threading
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json
Imports Telerik.Web.UI

Partial Class UsersManagement
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim watch As Stopwatch = Stopwatch.StartNew()
            RunSearch()
            ClsHelper.Log("Users management access", ClsSessionHelper.LogonUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, False, Nothing)
        Else
            If Request("__EVENTARGUMENT") IsNot Nothing Then
                If String.IsNullOrEmpty(Request("__EVENTARGUMENT")) Then
                    RunSearch(False)
                Else
                    Dim userId As String = Request("__EVENTARGUMENT")
                    ConnectAs(userId)
                End If
            End If
        End If
        If ClsSessionHelper.LogonUser IsNot Nothing Then
            userglobalid.Value = ClsSessionHelper.LogonUser.GlobalID.ToString()
        End If
    End Sub

    Protected Sub Page_Unload(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If IsPostBack Then
            If Request("__EVENTARGUMENT") IsNot Nothing Then
                If String.IsNullOrEmpty(Request("__EVENTARGUMENT")) Then
                    ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Refresh", "Refresh();", True)
                End If
            End If
        End If
    End Sub

    Private Sub ConnectAs(userid As String)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsHelper.ValidateUser("", "", userid)
        Dim previewedBy As Guid = IIf(ClsSessionHelper.LogonUser.PreviewedBy = Guid.Empty, ClsSessionHelper.LogonUser.GlobalID, ClsSessionHelper.LogonUser.PreviewedBy)
        Dim strRedirect As String = "Home.aspx"
        Dim userName As String = String.Empty
        Dim GlobalID As String = Guid.NewGuid().ToString()
        If user IsNot Nothing Then
            Dim Actions As List(Of ClsHelper.ActionDesignation) = Nothing
            Dim Tools As List(Of ClsHelper.Tool) = Nothing
            Dim Links As List(Of ClsHelper.Link) = Nothing
            user.Applications = ClsHelper.GetUserInformation(user.ID, Tools, Actions, Links)
            user.Actions = Actions
            user.Tools = Tools
            userName = user.Login
            Dim defaultTool As ClsHelper.Tool = ClsHelper.FindToolByID(Tools, user.HomePageToolID)
            If defaultTool IsNot Nothing Then
                strRedirect = defaultTool.Url
            End If
            user.PreviewedBy = previewedBy
            ClsSessionHelper.LogonUser = user
            ClsSessionHelper.ActiveDashboard = Nothing
            ClsSessionHelper.chatBots = Nothing
            FormsAuthentication.RedirectFromLoginPage(user.Login, True)
            GlobalID = user.GlobalID.ToString()
            If Not String.IsNullOrEmpty(Request("ReturnURL")) Then
                strRedirect = Request("ReturnURL")
            End If
        End If
        watch.Stop()
        ClsHelper.Log("Connect As", previewedBy.ToString(), String.Format("<b>Username:</b> {0}</br><b>Previewed user id:</b> {1}", userName, GlobalID), watch.ElapsedMilliseconds, user Is Nothing, IIf(user Is Nothing, "Unable to connect as user", Nothing))
        If user IsNot Nothing Then
            Response.Redirect(strRedirect, True)
        Else
            RunSearch(False)
        End If
    End Sub

    Protected Sub RunSearch(Optional ForceLoad As Boolean = True)
        Dim dataSet As DataSet = ClsUsersManagementHelper.GetGroupsDataSet()
        listGroupDefault.InnerHtml = ""
        listGroup.InnerHtml = ""
        If dataSet IsNot Nothing Then
            If dataSet.Tables.Count = 3 Then
                listGroupDefault.InnerHtml += GetItemList("All users", "#b35e82", dataSet.Tables(0).Rows(0)("AllUsers"), -1, "selected-row")
                listGroupDefault.InnerHtml += GetItemList("Not assigned to any group", "#967ADC", dataSet.Tables(1).Rows(0)("NotAssigned"), 0)

                For Each row As DataRow In dataSet.Tables(2).Rows
                    listGroup.InnerHtml += GetItemList(row("U_LOGIN"), row("U_GROUPUSERCOUNTCOLOR"), row("UsersInGroup"), row("U_ID"))
                Next
            End If
        End If
        If ForceLoad Then
            ClsUsersManagementHelper.FillUserCountriesTree(treeCountries)
        End If
    End Sub

    Private Function GetItemList(name As String, color As String, count As Integer, groupid As Integer, Optional extraClass As String = "") As String
        Dim groupEditScript As String = ""
        If groupid > 0 Then
            groupEditScript = String.Format("<span style=""color: #278efc !important"" data-group-id=""{0}"" class=""badge badge-pill float-right defaultLink groupDelete"">Delete</span><span data-group-id=""{0}"" style=""color: #278efc !important"" class=""badge badge-pill float-right defaultLink groupEdit"">Edit</span>", groupid)
        End If
        Return String.Format("<span onclick='selectGroup(this,{3})' class=""list-group-item {4} MoreInfoImg list-group-item-action"">{0} <img src=""Images/filter.png"" width=""20"" height=""20"" class=""filterImg"" /><span style=""background-color:{2};"" class=""badge badge-pill float-right"">{1}</span>{5}</span>", name, count.ToString(), color, groupid, extraClass, groupEditScript)
    End Function

End Class
