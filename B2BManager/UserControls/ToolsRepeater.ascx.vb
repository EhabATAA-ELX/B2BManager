
Imports System.Data

Partial Class UserControls_ToolsRepeater
    Inherits System.Web.UI.UserControl

    Private _FromDataSource As Boolean
    Private _DataSource As DataTable
    Private _Title As String

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        RefreshData()
    End Sub

    Public Property FromDataSource As Boolean
        Get
            Return _FromDataSource
        End Get
        Set
            _FromDataSource = Value
        End Set
    End Property

    Public Property Title As String
        Get
            Return _Title
        End Get
        Set
            _Title = Value
        End Set
    End Property

    Public Property DataSource As DataTable
        Get
            Return _DataSource
        End Get
        Set(value As DataTable)
            _DataSource = value
        End Set
    End Property

    Protected Sub RefreshData()
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
        Else

            toolName.InnerText = IIf(String.IsNullOrEmpty(Title), "Tools", Title)
            Dim repeaterTools As List(Of ClsHelper.Tool) = New List(Of ClsHelper.Tool)

            If Not FromDataSource Then
                Dim Tools As List(Of ClsHelper.Tool) = ClsSessionHelper.LogonUser.Tools.ToList
                Dim currentPage As String = System.IO.Path.GetFileName(Request.Url.AbsolutePath)
                Dim currentTool As ClsHelper.Tool = ClsHelper.FindToolByUrl(Tools, currentPage)
                Dim currentToolID As Integer = 0
                If currentTool IsNot Nothing Then
                    currentToolID = currentTool.ToolID
                End If

                If currentToolID > 0 Then
                    toolImage.ImageUrl = IIf(currentTool.IconImagePath.StartsWith("Images"), "~/" & currentTool.IconImagePath, currentTool.IconImagePath)
                    toolName.InnerText = currentTool.Name
                    repeaterTools = Tools.Where(Function(fn) fn.ParentToolID IsNot Nothing And (fn.TypeID = 1 Or fn.TypeID = 2)).Where(Function(fn1) fn1.ParentToolID = currentToolID And (fn1.TypeID = 1 Or fn1.TypeID = 2)).ToList
                Else
                    toolImage.Visible = False
                    repeaterTools = Tools.Where(Function(fn) fn.ParentToolID Is Nothing And (fn.TypeID = 1 Or fn.TypeID = 2)).ToList
                End If
            Else
                toolImage.Visible = False
            End If

            toolsReperter.DataSource = IIf(FromDataSource AndAlso DataSource IsNot Nothing, DataSource, repeaterTools)
            toolsReperter.DataBind()
        End If
    End Sub

End Class
