
Imports System.IO
Imports System.Net

Partial Class FileViewer
    Inherits System.Web.UI.Page

    Private clsUser As ClsUser

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        clsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Return
        End If
        If Not clsUser.Actions.Contains(ClsHelper.ActionDesignation.DOWNLOAD_FILES) Then
            btnDownloadFile.Visible = False
        End If
    End Sub
    Protected Async Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack() Then
            Dim EnvironmentID As Integer = 0
            Dim EntryID As Integer = 0
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If
            If Not String.IsNullOrEmpty(Request.QueryString("entryid")) Then
                Integer.TryParse(Request.QueryString("entryid"), EntryID)
            End If
            If EnvironmentID > 0 And EntryID > 0 And Request.QueryString("filename") IsNot Nothing Then
                Dim entry As ClsFilesViewerHelper.FileEntry = Nothing
                Dim envrionment = ClsFilesViewerHelper.GetEnvironment(EnvironmentID, EntryID, entry)
                Dim file As System.IO.FileInfo = New System.IO.FileInfo(envrionment.RootPath + IIf(envrionment.RootPath.EndsWith("\"), "", "\") + Request.QueryString("filename").Replace("@", "\"))
                FileName.Text = file.Name
                FullName.Text = file.Directory.FullName
                Size.Text = Math.Round(file.Length / 1024).ToString() & " KB"
                LastModificationDate.Text = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                CreationDate.Text = file.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")
                Using sr As New StreamReader(file.FullName)
                    fileContent.InnerHtml = WebUtility.HtmlEncode(Await sr.ReadToEndAsync())
                End Using
            End If

        End If
    End Sub

    Protected Sub btnDownload_Click(sender As Object, e As EventArgs)
        Dim EnvironmentID As Integer = 0
        Dim EntryID As Integer = 0
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If
        If Not String.IsNullOrEmpty(Request.QueryString("entryid")) Then
            Integer.TryParse(Request.QueryString("entryid"), EntryID)
        End If
        If EnvironmentID > 0 And EntryID > 0 And Request.QueryString("filename") IsNot Nothing Then
            Dim entry As ClsFilesViewerHelper.FileEntry = Nothing
            Dim envrionment = ClsFilesViewerHelper.GetEnvironment(EnvironmentID, EntryID, entry)
            Dim file As System.IO.FileInfo = New System.IO.FileInfo(envrionment.RootPath + IIf(envrionment.RootPath.EndsWith("\"), "", "\") + Request.QueryString("filename").Replace("@", "\"))
            If file.Exists Then
                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=" & file.Name)
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()
            End If
        End If
    End Sub
End Class
