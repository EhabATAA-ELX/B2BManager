
Partial Class FilesViewer
    Inherits System.Web.UI.Page

    Private clsUser As ClsUser

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        clsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Return
        End If
        If Not clsUser.Actions.Contains(ClsHelper.ActionDesignation.DOWNLOAD_FILES) Then
            btnDownload.Visible = False
            placeHolderDownloadScript.Visible = False
        End If
    End Sub
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack() Then
            Dim listofEntries As List(Of ClsFilesViewerHelper.FileEntry) = ClsFilesViewerHelper.GetFileEntries(Cache)
            For Each item In listofEntries
                If clsUser.Actions.Contains(item.AdministrationActionID) Then
                    ddlFilesEntryName.Items.Add(New ListItem(item.Name, item.ID.ToString()))
                    If ddlFilesEntryName.SelectedValue Is String.Empty Then
                        ddlFilesEntryName.SelectedValue = item.ID.ToString()
                    End If
                    For Each envrionment In item.Environments
                        ddlEnvironment.Items.Add(New ListItem(envrionment.Name, envrionment.ID.ToString()))
                        If ddlEnvironment.SelectedValue Is String.Empty Then
                            ddlEnvironment.SelectedValue = envrionment.ID.ToString()
                        End If
                    Next
                End If
            Next
            If ddlFilesEntryName.SelectedValue Is String.Empty Or ddlEnvironment.SelectedValue Is String.Empty Then
                EmptyList.Visible = True
                placeHolderContainer.Visible = False
            End If
        End If
    End Sub

    Protected Function ConvertToInt(action As ClsHelper.ActionDesignation) As Integer
        Return action
    End Function
    Protected Sub btnDownload_Click(sender As Object, e As EventArgs)
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
        If __EVENTARGUMENT IsNot Nothing Then
            Dim entry As ClsFilesViewerHelper.FileEntry = Nothing
            Dim envrionment = ClsFilesViewerHelper.GetEnvironment(CInt(ddlEnvironment.SelectedValue), CInt(ddlFilesEntryName.SelectedValue), entry)
            Dim file As System.IO.FileInfo = New System.IO.FileInfo(envrionment.RootPath + IIf(envrionment.RootPath.EndsWith("\"), "", "\") + __EVENTARGUMENT.Replace("@", "\"))
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

    Protected Sub ddlFilesEntryName_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim listofEntries As List(Of ClsFilesViewerHelper.FileEntry) = ClsFilesViewerHelper.GetFileEntries(Cache)
        Dim entry As ClsFilesViewerHelper.FileEntry = listofEntries.Where(Function(Fc) Fc.ID = ddlFilesEntryName.SelectedValue).SingleOrDefault()
        ddlEnvironment.Items.Clear()
        For Each envrionment In entry.Environments
            ddlEnvironment.Items.Add(New ListItem(envrionment.Name, envrionment.ID.ToString()))
            If ddlEnvironment.SelectedValue Is String.Empty Then
                ddlEnvironment.SelectedValue = envrionment.ID.ToString()
            End If
        Next
        LoadData()
    End Sub

    Protected Sub ddlEnvironment_SelectedIndexChanged(sender As Object, e As EventArgs)
        LoadData()
    End Sub

    Private Sub LoadData()
        ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "LoadData", "LoadData();", True)
    End Sub

    Protected Sub imageBtnRefresh_Click(sender As Object, e As ImageClickEventArgs)
        LoadData()
    End Sub
End Class
