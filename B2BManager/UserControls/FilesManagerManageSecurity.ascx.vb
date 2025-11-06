Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security.Cryptography
Imports Newtonsoft.Json
Imports Telerik.Web.UI

Partial Class UserControls_FilesManagerManageSecurity
    Inherits UserControl

    Public _CountryGuid As Guid
    Public _SopId As String
    Public _SecuredUnsecured As String
    Public _FileSelect As FileServerWS.DocumentEntity()
    Public _FolderGuidIdLabel As Guid
    Public _CountryAllCompagniesList As List(Of FileServerWS.CompanyEntity)
    Public _EnvironmentID As String
    Const _SecuredText As String = "Customer Specific"
    Const _StyleDisplayNone As String = "none"
    Const _CountryAllCompagnies As String = "CountryAllCompagnies"

    Public Property FileSelect As FileServerWS.DocumentEntity()
        Get
            Return _FileSelect
        End Get
        Set(value As FileServerWS.DocumentEntity())
            _FileSelect = value
        End Set
    End Property

    Private ReadOnly Property DocumentIDs As Guid()
        Get
            If FileSelect IsNot Nothing Then
                Dim fileSelectionIDs = FileSelect.Select(Function(x) x.ID).ToArray()
                Return fileSelectionIDs
            Else
                Dim labelText = Me.DocumentGuidLabel.Text
                If String.IsNullOrWhiteSpace(labelText) Then
                    Return New Guid(-1) {}
                End If
                Return Nothing
                Try
                    Return labelText.Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries).Select(Function(idString) Guid.Parse(idString.Trim())).ToArray()
                Catch ex As FormatException
                    Return New Guid(-1) {}
                End Try
            End If
        End Get
    End Property

    Public Property FolderGuidId As Guid
        Get
            Return _FolderGuidIdLabel
        End Get
        Set(value As Guid)
            _FolderGuidIdLabel = value
        End Set
    End Property

    Public Property CountryGuid As Guid
        Get
            Return _CountryGuid
        End Get
        Set(value As Guid)
            _CountryGuid = value
        End Set
    End Property

    Public Property SopId As String
        Get
            Return _SopId
        End Get
        Set(value As String)
            _SopId = value
        End Set
    End Property

    Public Property SecuredUnsecured As String
        Get
            Return _SecuredUnsecured
        End Get
        Set(value As String)
            _SecuredUnsecured = value
        End Set
    End Property

    Public Property CountryAllCompagnies As List(Of FileServerWS.CompanyEntity)
        Get
            Return _CountryAllCompagniesList
        End Get
        Set(value As List(Of FileServerWS.CompanyEntity))
            _CountryAllCompagniesList = value
        End Set
    End Property

    Public Property EnvironmentID As String
        Get
            Return _EnvironmentID
        End Get
        Set(value As String)
            _EnvironmentID = value
        End Set
    End Property

    Private Sub SetManageSecurity()
        CountryGuidLabel.Text = CountryGuid.ToString()
        FolderGuidLabel.Text = FolderGuidId.ToString()
        EnvironmentIDLabel.Text = EnvironmentID

        SecuredUnsecuredRadTabStrip.Tabs(0).Style.Add(HtmlTextWriterStyle.Display, IIf(SecuredUnsecured = _SecuredText, String.Empty, _StyleDisplayNone))
        SecuredUnsecuredRadTabStrip.Tabs(1).Style.Add(HtmlTextWriterStyle.Display, IIf(SecuredUnsecured <> _SecuredText, String.Empty, _StyleDisplayNone))
        SecuredUnsecuredRadTabStrip.SelectedIndex = IIf(SecuredUnsecured = _SecuredText, 0, 2)

        RadMultiPage1.SelectedIndex = SecuredUnsecuredRadTabStrip.SelectedIndex
        RadPageView1.Style.Add(HtmlTextWriterStyle.Display, IIf(SecuredUnsecured = _SecuredText, String.Empty, _StyleDisplayNone))
        RadPageView2.Style.Add(HtmlTextWriterStyle.Display, IIf(SecuredUnsecured <> _SecuredText, String.Empty, _StyleDisplayNone))

        ' When FileSelect.Length > 1, it is only when the documents have been just uploaded, so they have the same publication parameters (exception : the thumbnail)
        Dim file = FileSelect.First
        If FileSelect.Length = 1 Then
            ThumbnailTable.Style("display") = "inline"
            SetThumbnailDefault.Checked = String.IsNullOrEmpty(file.ThumbnailName)
            ThumbnailUploadLabel.Text = String.Empty
            ThumbnailGuidLabel.Text = file.ThumbnailID.ToString()
            ThumbnailNameLabel.Text = file.ThumbnailName
        Else
            ThumbnailTable.Style("display") = "none"
        End If

        ManagePublicationErrorLabel.Text = String.Empty
        PublishDateCheckBox.Checked = file.UseDateRangeForPublishing
        FromDatePicker.Enabled = file.UseDateRangeForPublishing
        ToDatePicker.Enabled = file.UseDateRangeForPublishing
        SendNotificationCB.Checked = file.SendNotification

        If file.UseDateRangeForPublishing Then
            FromDatePicker.SelectedDate = file.StartDate
            ToDatePicker.SelectedDate = file.EndDate
        Else
            FromDatePicker.SelectedDate = Nothing
            ToDatePicker.SelectedDate = Nothing
        End If

        SendNotificationLabel.Text = file.SendNotification.ToString()
        DocumentGuidLabel.Text = String.Join(";", FileSelect.Select(Function(x) x.ID.ToString()))
        FileSelectRadGrid.DataSource = FileSelect
        FileSelectRadGrid.DataBind()

        Using WS As New FileServerWS.FileServerWSSoapClient()
            WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentID))
            'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
            'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
            Dim Auth As New FileServerWS.UserContextAuthentication With {
                .CountryID = CountryGuid,
                .SessionID = New Guid
            }

            If SecuredUnsecured = _SecuredText Then
                Dim documentIdsString As String = String.Join(";", DocumentIDs)

                Dim urlBase = String.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority)
                Dim newConditionTokenData As New TokenData(EnvironmentID, SopId, False, "creation", "FilesManager", documentIdsString, "", Nothing)
                Dim token = ClsTokenHelper.GenerateToken(newConditionTokenData)
                AddConditionBtn.OnClientClick = "window.parent.openInDirectAssignmentPopup('" & urlBase & ResolveUrl("~/InDirectAssignment.aspx") & "?QueryBuilderToken=" & token & "'); return false;"

                Dim newStaticConditionTokenData As New TokenData(EnvironmentID, SopId, True, "creation", "FilesManager", documentIdsString, "", Nothing)
                Dim staticToken = ClsTokenHelper.GenerateToken(newStaticConditionTokenData)

                AddStaticConditionBtn.OnClientClick = "window.parent.openStaticAssignmentPopup('" & urlBase & ResolveUrl("~/QueryBuilderStaticAssignment.aspx") & "?QueryBuilderToken=" & staticToken & "'); return false;"

                ' Rebind the new ConditionGrid control if present
                Dim ctl = TryCast(Me.FindControl("ConditionGrid1"), UserControl)
                If ctl IsNot Nothing Then
                    Dim method = ctl.GetType().GetMethod("RebindConditions")
                    If method IsNot Nothing Then
                        method.Invoke(ctl, Nothing)
                    End If
                End If
            ElseIf FileSelect.Length = 1 AndAlso FileSelect.First.ThumbnailID = New Guid Then
                ImageThumbnail.ImageUrl = String.Empty
            ElseIf FileSelect.Length = 1 Then
                Dim thumbnailEntity As FileServerWS.ThumbnailEntity = WS.GetThumbnailByID(Auth, FileSelect.First.ThumbnailID)
                ImageThumbnail.ImageUrl = "data:image;base64," + Convert.ToBase64String(thumbnailEntity.Content)
            End If
        End Using
    End Sub


    Private Function AllCompagniesRadGrid(WS As FileServerWS.FileServerWSSoapClient, Auth As FileServerWS.UserContextAuthentication, AssignedCompagnies As FileServerWS.CompanyEntity()) As List(Of FileServerWS.CompanyEntity)
        Dim listIdAssigned = If(IsNothing(AssignedCompagnies), New List(Of Guid), AssignedCompagnies.Select(Function(x) x.ID).ToList)
        Dim AllCountryCompagny As New List(Of FileServerWS.CompanyEntity)
        If Not IsNothing(CountryAllCompagnies) AndAlso CountryAllCompagnies.Any() Then
            AllCountryCompagny = CountryAllCompagnies.Where(Function(x) Not listIdAssigned.Contains(x.ID)).ToList()
            'Save compagnies array list
            ViewState.Add(_CountryAllCompagnies, CountryAllCompagnies)
        Else
            Dim GetCompagnies As New List(Of FileServerWS.CompanyEntity)
            Dim CompagnyList As FileServerWS.CompanyEntity() = WS.GetCountryCompanyList(Auth, "")
            GetCompagnies.AddRange(CompagnyList)
            AllCountryCompagny.AddRange(CompagnyList.Where(Function(x) Not listIdAssigned.Contains(x.ID)))
            'Save compagnies array list
            ViewState.Add(_CountryAllCompagnies, GetCompagnies)
        End If
        If AllCountryCompagny.Any(Function(x) x.Description Is Nothing Or x.C_GRP4 Is Nothing) Then
            For Each descriptionNothing As FileServerWS.CompanyEntity In AllCountryCompagny.Where(Function(x) x.Description Is Nothing Or x.C_GRP4 Is Nothing)
                If descriptionNothing.Description Is Nothing Then
                    descriptionNothing.Description = String.Empty
                End If
                If descriptionNothing.C_GRP4 Is Nothing Then
                    descriptionNothing.C_GRP4 = String.Empty
                End If
            Next
        End If
        Return AllCountryCompagny
    End Function

    Private Sub TreeViewMain(folderEntity As IEnumerable(Of FileServerWS.FolderEntity))
        Dim listId As New List(Of Guid)(folderEntity.Select(Function(y) y.ID).Distinct())
        Dim listRootFolder As New List(Of RadTreeNode)
        For Each folder As FileServerWS.FolderEntity In folderEntity.Where(Function(x) x.IsDeleted = False)
            If Not listId.Contains(folder.ParentFolderID) Then
                Dim treeNode As New RadTreeNode(folder.Name, folder.ID.ToString()) With {
                    .ExpandMode = TreeNodeExpandMode.ServerSide,
                    .Selected = True,
                    .Expanded = True
                }
                AddNodeToTreeView(treeNode, folderEntity)
                listRootFolder.Add(treeNode)
            End If
        Next
        SecuredRadTreeView.Nodes.Clear()
        SecuredRadTreeView.Nodes.AddRange(listRootFolder)
    End Sub

    Private Sub AddNodeToTreeView(ByRef rootNode As RadTreeNode, folderEntity As IEnumerable(Of FileServerWS.FolderEntity))
        Dim rootValue As String = rootNode.Value
        For Each folder As FileServerWS.FolderEntity In folderEntity.Where(Function(x) x.ParentFolderID.ToString() = rootValue)
            Dim treeNode As New RadTreeNode(folder.Name, folder.ID.ToString())
            If folderEntity.Any(Function(y) y.ParentFolderID = folder.ID) Then
                treeNode.ExpandMode = TreeNodeExpandMode.ServerSide
                treeNode.Expanded = True
                AddNodeToTreeView(treeNode, folderEntity)
            End If
            rootNode.Nodes.Add(treeNode)
        Next
    End Sub

    Protected Sub ManagePublicationBtnOK_Click(sender As Object, e As EventArgs)
        Dim documentIds As Guid() = DocumentGuidLabel.Text.Split(";").Select(Function(x) Guid.Parse(x)).ToArray
        If documentIds.Length = 1 Then
            If Not SetThumbnailDefault.Checked And Guid.Parse(ThumbnailGuidLabel.Text) = Guid.Empty Then
                ThumbnailUploadLabel.Text = "Please upload an image"
                ThumbnailUploadLabel.Style.Add(HtmlTextWriterStyle.Color, "Red")
                Return
            Else
                ThumbnailUploadLabel.Text = String.Empty
            End If
        End If

        Using WS As New FileServerWS.FileServerWSSoapClient()
            WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentIDLabel.Text))
            'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
            'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
            Dim auth As New FileServerWS.UserContextAuthentication With {
                .CountryID = Guid.Parse(CountryGuidLabel.Text),
                .SessionID = New Guid
            }

            ' --- Decide thumbnail
            Dim thumbnailId As Guid

            If SetThumbnailDefault.Checked Then
                Dim gridItem As GridDataItem = FileSelectRadGrid.Items.OfType(Of GridDataItem).First()
                Dim docId As Guid = Guid.Parse(gridItem("ID").Text)
                Dim docName As String = gridItem("Name").Text
                Dim ext As String = Path.GetExtension(docName).ToLowerInvariant()
                ' Check if document is image
                Dim isImage As Boolean = {".jpg", ".jpeg", ".png", ".gif", ".bmp"}.Contains(ext)

                If isImage Then
                    ' Get original image content
                    Dim imageDoc As FileServerWS.DocumentEntity = WS.GetDocument(auth, docId)
                    Dim resizedBytes = ResizeImage(imageDoc.Content, 100, 80, ext)
                    If resizedBytes IsNot Nothing Then
                        thumbnailId = UploadThumbnailToServer(auth, docId, docName, resizedBytes)
                        Dim img As WebControls.Image = CType(gridItem.FindControl("ThumbnailImage"), WebControls.Image)
                        If img IsNot Nothing Then
                            img.ImageUrl = "data:image/png;base64," & Convert.ToBase64String(resizedBytes)
                        End If
                    Else
                        ManagePublicationErrorLabel.Text = "Could not create image thumbnail."
                        ManagePublicationErrorLabel.Style.Add(HtmlTextWriterStyle.Color, "Red")
                        Return
                    End If
                Else
                    ' Use default icon thumbnail
                    Dim icon = DefaultIconForExtension(ext)
                    Dim path = Server.MapPath("~/Images/Thumbnails/" & icon)
                    If File.Exists(path) Then
                        Dim bytes = File.ReadAllBytes(path)
                        thumbnailId = UploadThumbnailToServer(auth, docId, icon, bytes)
                        Dim ctrl = gridItem.FindControl("ThumbnailImage")
                        If TypeOf ctrl Is Image Then
                            Dim imgControl = CType(ctrl, Image)
                            imgControl.ImageUrl = "data:image/png;base64," & Convert.ToBase64String(bytes)
                        End If
                    Else
                        ManagePublicationErrorLabel.Text = "Default thumbnail not found."
                        ManagePublicationErrorLabel.Style.Add(HtmlTextWriterStyle.Color, "Red")
                        Return
                    End If
                End If
            Else
                ' Use uploaded custom thumbnail
                thumbnailId = Guid.Parse(ThumbnailGuidLabel.Text)
            End If

            ' --- Publication settings
            Dim useDateRangeForPublishing = PublishDateCheckBox.Checked
            Dim publishFrom = If(FromDatePicker.SelectedDate, Date.Now)
            Dim publishTo = If(ToDatePicker.SelectedDate, Date.Now)
            Dim sendNotification = SendNotificationCB.Checked

            Dim success = WS.UpdateDocumentPublicationParameters(auth, documentIds, thumbnailId, useDateRangeForPublishing, publishFrom, publishTo, sendNotification)

            If Not success Then
                ManagePublicationErrorLabel.Text = "No displaying for the file."
                ManagePublicationErrorLabel.Style.Add(HtmlTextWriterStyle.Color, "Green")
                Return
            End If

            ' --- UI updates
            ManagePublicationErrorLabel.Text = If(useDateRangeForPublishing,
                "Displaying from " + publishFrom.ToString("dd/MM/yyyy") + " to " + publishTo.ToString("dd/MM/yyyy"),
                String.Empty)
            ManagePublicationErrorLabel.Style.Add(HtmlTextWriterStyle.Color, "Green")

            For Each gridItem In FileSelectRadGrid.Items.OfType(Of GridDataItem)
                gridItem("CreationDate").Text = Date.Now.ToString("dd/MM/yyyy")
                Dim startDateLabel As Label = DirectCast(gridItem("StartDateGridTemplate").Controls(1), Label)
                Dim endDateLabel As Label = DirectCast(gridItem("EndDateGridTemplate").Controls(1), Label)
                startDateLabel.Text = IIf(useDateRangeForPublishing, publishFrom.ToString("dd/MM/yyyy"), String.Empty)
                endDateLabel.Text = IIf(useDateRangeForPublishing, publishTo.ToString("dd/MM/yyyy"), String.Empty)
                gridItem("UseDateRangeForPublishing").Text = useDateRangeForPublishing.ToString()
                gridItem("ThumbnailName").Text = IIf(SetThumbnailDefault.Checked, String.Empty, ThumbnailNameLabel.Text)
                gridItem("ThumbnailID").Text = thumbnailId.ToString()

                Dim checkBox As CheckBox = CType(gridItem.FindControl("SendNotificationCb"), CheckBox)
                If checkBox IsNot Nothing Then
                    checkBox.Checked = sendNotification
                End If
            Next

            SendNotificationLabel.Text = sendNotification.ToString
        End Using
    End Sub

    Protected Sub ThumbnailUploadBtn_Click(sender As Object, e As EventArgs)
        If Not ThumbnailUpload.HasFile Then
            ThumbnailUploadLabel.Text = "Please select a file to upload."
            ThumbnailUploadLabel.Style.Add(HtmlTextWriterStyle.Color, "Red")
            SetThumbnailDefault.Checked = True
            Return
        End If

        If Not IsImageContentType(ThumbnailUpload.PostedFile.ContentType) Then
            ThumbnailUploadLabel.Text = "Thumbnail must be an image."
            ThumbnailUploadLabel.Style.Add(HtmlTextWriterStyle.Color, "Red")
            SetThumbnailDefault.Checked = True
            Return
        End If

        Dim uploadedImage As Drawing.Image = Drawing.Image.FromStream(ThumbnailUpload.PostedFile.InputStream)
        Dim errorMsg As String = ""
        If Not IsImageValidExactSize(uploadedImage, errorMsg) Then
            ThumbnailUploadLabel.Text = errorMsg
            ThumbnailUploadLabel.Style.Add(HtmlTextWriterStyle.Color, "Red")
            SetThumbnailDefault.Checked = True
            Return
        End If

        ' Rewind stream and get byte array
        ThumbnailUpload.PostedFile.InputStream.Position = 0
        Dim bytes As Byte() = ThumbnailUpload.FileBytes

        ' Get document ID
        Dim gridItem As GridDataItem = FileSelectRadGrid.Items.OfType(Of GridDataItem).First()
        Dim docId As Guid = Guid.Parse(gridItem("ID").Text)

        ' Create auth context
        Dim auth As New FileServerWS.UserContextAuthentication With {
        .CountryID = Guid.Parse(CountryGuidLabel.Text),
        .SessionID = New Guid
         }

        ' Upload and get new ID
        Dim thumbnailId As Guid = UploadThumbnailToServer(auth, docId, ThumbnailUpload.FileName, bytes)
        SetThumbnailDefault.Checked = False
        ThumbnailUploadLabel.Text = ""
    End Sub

    Private Sub UserControls_FilesManagerManageSecurity_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If Not CountryGuid = New Guid Then
            SetManageSecurity()
            UnsecuredToSecuredLabel.Text = String.Empty
        ElseIf RadPageView1.Selected Then

        End If
    End Sub

    Public Event BackFileUploadBtnClick As EventHandler

    Protected Sub BackFileUploadBtn_Click(sender As Object, e As EventArgs)
        RaiseEvent BackFileUploadBtnClick(sender, e)
    End Sub

    Protected Sub UnsecuredToSecuredBtn_Click(sender As Object, e As EventArgs)
        Using WS As New FileServerWS.FileServerWSSoapClient()
            WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentIDLabel.Text))
            'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
            'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
            Dim Auth As New FileServerWS.UserContextAuthentication With {
                .CountryID = Guid.Parse(CountryGuidLabel.Text),
                .SessionID = New Guid
            }
            Dim folderSecured As FileServerWS.FolderEntity = WS.GetSecuredFoldersStructure(Auth).Where(Function(x) x.ID = Guid.Parse(SecuredRadTreeView.SelectedValue)).First()
            Dim listDocuSecured As FileServerWS.DocumentEntity() = WS.GetDocumentsList(Auth, folderSecured.ID)
            Dim GridDataFileSelect As GridDataItem = FileSelectRadGrid.Items.OfType(Of GridDataItem).First()
            If listDocuSecured.Any(Function(x) x.Name = GridDataFileSelect("Name").Text) Then
                UnsecuredToSecuredLabel.Text = "A file with the same name exists in the secured folder."
                Return
            End If

            Dim DocumentId As Guid = Guid.Parse(GridDataFileSelect("ID").Text)
            If SendNotificationLabel.Text = "True" Then
                ' Log must be done before the change, so no asynchronous call, but a direct call to the service.
                Dim service = New B2BManagerService()
                service.AssignmentLogs(0, EnvironmentIDLabel.Text, CountryGuidLabel.Text, 1, DocumentId.ToString, Nothing)
            End If
            Dim UnassignedCompagnies As List(Of FileServerWS.CompanyEntity) = Nothing
            Try
                UnassignedCompagnies = AllCompagniesRadGrid(WS, Auth, Nothing)
                Dim FolderId As Guid = Guid.Parse(FolderGuidLabel.Text)
                Dim documentToSecure As FileServerWS.DocumentEntity = WS.GetDocumentsList(Auth, FolderId).FirstOrDefault(Function(x) x.ID = DocumentId)
                If Not IsNothing(documentToSecure) Then
                    WS.MoveFilesToSecureFolder(Auth, FolderId, folderSecured, {documentToSecure})
                End If
            Finally
                ' We have to use a finally because MoveFilesToSecureFolder can raise an exception but the file is correctly moved
                SecuredUnsecuredRadTabStrip.SelectedIndex = 0
                SecuredUnsecuredRadTabStrip.Tabs(0).Style.Remove(HtmlTextWriterStyle.Display)
                SecuredUnsecuredRadTabStrip.Tabs(1).Style.Remove(HtmlTextWriterStyle.Display)
                SecuredUnsecuredRadTabStrip.Tabs(0).Style.Add(HtmlTextWriterStyle.Display, String.Empty)
                SecuredUnsecuredRadTabStrip.Tabs(1).Style.Add(HtmlTextWriterStyle.Display, _StyleDisplayNone)

                RadMultiPage1.SelectedIndex = 0
                RadPageView1.Style.Remove(HtmlTextWriterStyle.Display)
                RadPageView2.Style.Remove(HtmlTextWriterStyle.Display)
                RadPageView1.Style.Add(HtmlTextWriterStyle.Display, String.Empty)
                RadPageView2.Style.Add(HtmlTextWriterStyle.Display, _StyleDisplayNone)
            End Try
        End Using
    End Sub

    Protected Sub SecuredRadTreeView_NodeClick(sender As Object, e As RadTreeNodeEventArgs)

    End Sub

    Private Function UploadThumbnailToServer(auth As FileServerWS.UserContextAuthentication, docId As Guid, fileName As String, fileBytes As Byte()) As Guid
        Dim thumbnail As New FileServerWS.ThumbnailEntity With {
            .Name = fileName,
            .Content = fileBytes,
            .Size = fileBytes.Length,
            .CreationDate = Now,
            .Extension = Path.GetExtension(fileName),
            .CreatedBy = ClsSessionHelper.LogonUser.GlobalID,
            .DocumentID = docId
        }

        Using WS As New FileServerWS.FileServerWSSoapClient()
            WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentIDLabel.Text))
            'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
            'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
            Dim thumbnailId = WS.UploadThumbnail(auth, thumbnail)
            ' Update labels
            ThumbnailGuidLabel.Text = thumbnailId.ToString()
            ThumbnailNameLabel.Text = thumbnail.Name
            ImageThumbnail.ImageUrl = "data:image;base64," & Convert.ToBase64String(thumbnail.Content)
            Return thumbnailId
        End Using
    End Function

    Protected Sub ConditionRg_ItemDataBound(sender As Object, e As GridItemEventArgs)
        ' Handled by ConditionGrid user control now.
    End Sub

    Protected Sub ConditionRg_NeedDataSource(source As Object, e As GridNeedDataSourceEventArgs)
        ' Handled by ConditionGrid user control now.
    End Sub


End Class