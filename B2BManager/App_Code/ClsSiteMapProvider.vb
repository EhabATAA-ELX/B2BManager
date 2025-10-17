Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic

Public Class ClsSiteMapProvider
    Inherits StaticSiteMapProvider

    Private _RootNode As SiteMapNode
    Private _isInitialized As Boolean = False
    Public Const CacheDependencyKey As String = "SiteMapProviderTools"
    Public Overrides Function BuildSiteMap() As SiteMapNode
        SyncLock Me
            If _RootNode Is Nothing Then
                _RootNode = New SiteMapNode(Me, "Root", "~/Home.aspx", "Home")
                AddNode(_RootNode)
                Dim node As SiteMapNode = New SiteMapNode(Me, "Profile", "~/MyProfile.aspx", "My Profile", "My Profile")
                AddNode(node, _RootNode)
                Dim tools As List(Of ClsHelper.Tool) = GetTools()
                If tools.Count > 0 Then
                    Dim root = ClsHelper.GenerateTree(tools, Function(fc) fc.ToolID, Function(fc) fc.ParentToolID, Nothing)
                    For Each tool As ClsHelper.TreeItem(Of ClsHelper.Tool) In root
                        AddToolNode(tool, _RootNode)
                    Next
                    _isInitialized = True
                End If
            End If
            Return _RootNode
        End SyncLock
    End Function

    Private Sub AddToolNode(treeItem As ClsHelper.TreeItem(Of ClsHelper.Tool), ByRef rootNode As SiteMapNode)
        Dim node As SiteMapNode = New SiteMapNode(Me, treeItem.Item.ToolID, treeItem.Item.Url, treeItem.Item.Name, treeItem.Item.Name)
        AddNode(node, rootNode)
        If treeItem.Children IsNot Nothing Then
            For Each child As ClsHelper.TreeItem(Of ClsHelper.Tool) In treeItem.Children
                AddToolNode(child, node)
            Next
        End If
    End Sub

    Private Function GetTools() As List(Of ClsHelper.Tool)
        Dim tools As List(Of ClsHelper.Tool) = New List(Of ClsHelper.Tool)
        If HttpRuntime.Cache(CacheDependencyKey) Is Nothing Then
            Dim toolsDt As DataTable = ClsDataAccessHelper.FillDataTable("Administration.GetTools")
            If toolsDt IsNot Nothing Then
                For Each toolRow In toolsDt.Rows
                    Dim parentToolID As Integer? = Nothing
                    Dim url As String = ClsDataAccessHelper.GetText(toolRow, "Url")
                    If toolRow("ParentToolID") IsNot DBNull.Value Then
                        parentToolID = CInt(toolRow("ParentToolID"))
                    End If
                    If Not url.StartsWith("http") Then
                        If Not url.StartsWith("~/") Then
                            url = "~/" + url
                        End If
                        Dim tool As ClsHelper.Tool = New ClsHelper.Tool(toolRow("ToolID"), parentToolID, ClsDataAccessHelper.GetText(toolRow, "IconImagePath"), ClsDataAccessHelper.GetText(toolRow, "Name"), url, toolRow("TypeID"), ClsDataAccessHelper.GetText(toolRow, "MenuIconImagePath"), ClsDataAccessHelper.GetText(toolRow, "BadgeText", ""), ClsDataAccessHelper.GetText(toolRow, "BadgeColor", ""), False)
                        tools.Add(tool)
                    End If
                Next
                HttpRuntime.Cache.Insert(CacheDependencyKey, tools)
            End If
        Else
            tools = HttpRuntime.Cache(CacheDependencyKey)
        End If
        Return tools
    End Function

    Protected Overrides Sub Clear()
        SyncLock Me
            _RootNode = Nothing
            If HttpRuntime.Cache(CacheDependencyKey) Is Nothing Then
                HttpRuntime.Cache.Remove(CacheDependencyKey)
            End If
            MyBase.Clear()
        End SyncLock
    End Sub
    Public Overrides ReadOnly Property RootNode() As SiteMapNode
        Get
            Return BuildSiteMap()
        End Get
    End Property

    Public ReadOnly Property IsInitialized As Boolean
        Get
            Return _isInitialized
        End Get
    End Property

    Protected Overrides Function GetRootNodeCore() As SiteMapNode
        Return RootNode
    End Function

End Class
