Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Reflection
Imports System.Threading.Tasks
Imports System.Web.Services
Imports Telerik.Web.UI

Partial Class FilesManager
    Inherits Page
    Const _CountryAllCompagnies As String = "CountryAllCompagnies"
    Const _Secured As String = "Secured"
    Const _Unsecured As String = "Unsecured"
    Const _NewFolder = "NewFolder"

    'this algorithm is in the service
    'Private Shared Function HasNameDocument(lstDocuments As FileServerWS.DocumentEntity(), nameDoc As String) As String
    '    If lstDocuments.Any(Function(x) x.Name = nameDoc) Then
    '        Dim ext = Path.GetExtension(nameDoc)
    '        Dim list = nameDoc.Split("_")
    '        Dim number As Integer
    '        Dim newName As String
    '        If Integer.TryParse(list(list.Length - 1).Replace(ext, String.Empty), number) Then
    '            number += 1
    '            Dim lastUnderscoreIndex As Integer = nameDoc.LastIndexOf("_")
    '            Dim truncatedName As String = nameDoc.Substring(0, lastUnderscoreIndex)
    '            newName = Path.GetFileNameWithoutExtension(truncatedName) + "_" + number.ToString + ext
    '        Else
    '            newName = Path.GetFileNameWithoutExtension(nameDoc) + "_1" + Path.GetExtension(nameDoc)
    '        End If
    '        Return HasNameDocument(lstDocuments, newName)
    '    Else
    '        Return nameDoc
    '    End If
    'End Function

    Private Sub DeleteFile(idFile As Guid)
        Dim ret As DataTable
        Using WS As New FileServerWS.FileServerWSSoapClient()
            WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentRadComboBox.SelectedValue))
            'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
            'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
            Dim Auth As New FileServerWS.UserContextAuthentication With {
                .SessionID = New Guid,
                .CountryID = Guid.Parse(CountryGuid.Value)
            }
            ret = WS.DeleteDocuments(Auth, {idFile})
        End Using

        For Each row As DataRow In ret.Rows
            Dim service = New B2BManagerService()
            Dim customerIds = If(Not String.IsNullOrEmpty(row("compIds").ToString), row("compIds").ToString, Nothing)
            service.AssignmentLogs(
                ClsHelper.AssignmentType.Unassigned,
                EnvironmentRadComboBox.SelectedValue,
                CountryGuid.Value,
                ClsHelper.ObjectType.File,
                row("docId").ToString,
                customerIds)
        Next
    End Sub

    Private Sub RefreshSelectedFolder()
        SecuredUnsecuredFilesNodeClick(FileManagementRadTreeView.SelectedNode.Value)
    End Sub

    Private Sub RenderControls()
        Dim applications As List(Of ClsHelper.Application) = Nothing
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = ClsSessionHelper.LogonUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        SetEnvironments(selectedApplication.Environments)
        'SetCountries(selectedApplication.Countries)
        ClsHelper.RenderCountryDropDown(CountryRadComboBox, selectedApplication.Countries, CountryRadComboBox.SelectedValue, selectedApplication.SelectAllCountriesByDefault)
    End Sub

    Private Sub AddAjaxSetting(ByVal controlID As String)
        Dim ajaxSetting As New AjaxSetting With {
            .AjaxControlID = controlID
        }
        FilesManagerRadAjaxManager.AjaxSettings.Add(ajaxSetting)
    End Sub

    Private Sub InitAjaxSettings()
        FilesManagerRadAjaxManager.AjaxSettings.Clear()
        AddAjaxSetting("OKbtn")
        AddAjaxSetting("UnsecuredToSecuredBtn")
        AddAjaxSetting("FileManagerPanel")
        AddAjaxSetting("BackFileUploadBtn")
    End Sub

    Private Sub SetEnvironments(environments As List(Of ClsHelper.BasicModel))
        For Each environment As ClsHelper.BasicModel In environments
            Dim item As New RadComboBoxItem With {
                .Text = environment.Name,
                .Value = environment.ID
            }
            EnvironmentRadComboBox.Items.Add(item)
            item.DataBind()
        Next
    End Sub

    Private Sub SetCountries(countries As List(Of ClsHelper.Country))
        For Each country As ClsHelper.Country In countries
            Dim item As New RadComboBoxItem With {
                .Text = country.Name,
                .Value = country.SOP_ID,
                .ImageUrl = country.ImageURL
            }
            CountryRadComboBox.Items.Add(item)
            item.DataBind()
        Next
    End Sub

    Private Function GetLanguagesFromEnvironment(sSopId As String) As DataRow()
        Dim env As String = EnvironmentRadComboBox.SelectedValue
        Dim parameters As New List(Of SqlParameter) From {
            New SqlParameter("@EnvironmentID", env)
        }
        Dim DtLanguages = ClsDataAccessHelper.FillDataTable("[Ebusiness].[Translations_GetLanguagesByEnv]", parameters)
        If DtLanguages IsNot Nothing And DtLanguages.Rows.Count > 0 Then
            Return DtLanguages.Select("S_SOP_ID = '" + sSopId + "'")
        End If
        Return Nothing
    End Function

    Private Sub SearchFolder()
        If String.IsNullOrEmpty(LangIsocideBox.Text.ToString().Trim()) Or String.IsNullOrEmpty(CyGlobalId.Text.ToString().Trim()) Then Return

        Using WS As New FileServerWS.FileServerWSSoapClient()
            WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentRadComboBox.SelectedValue))
            'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
            'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
            Dim Auth As New FileServerWS.UserContextAuthentication With {
                .CountryID = Guid.Parse(CountryGuid.Value),
                .SessionID = New Guid
            }
            Dim FolderEntity As FileServerWS.FolderEntity() = Nothing
            Dim mode As String = Nothing
            Try
                If SecureUnsecureTabStrip.SelectedIndex = 0 Then
                    FolderEntity = WS.GetSecuredFoldersStructure(Auth)
                    mode = _Secured
                Else
                    FolderEntity = WS.GetUnsecuredFoldersStructure(Auth)
                    mode = _Unsecured
                End If
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try

            TreeViewMain(FolderEntity.OrderBy(Function(x) x.Rank), mode)
        End Using
    End Sub

    Private Sub StoreCompagnies()
        If Not String.IsNullOrEmpty(LangIsocideBox.Text.ToString().Trim()) And Not String.IsNullOrEmpty(CyGlobalId.Text.ToString().Trim()) Then
            Using WS As New FileServerWS.FileServerWSSoapClient()
                WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentRadComboBox.SelectedValue))
                'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
                'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
                Dim Auth As New FileServerWS.UserContextAuthentication With {
                    .CountryID = Guid.Parse(CountryGuid.Value),
                    .SessionID = New Guid
                }
                Dim AllCountryCompagny As New List(Of FileServerWS.CompanyEntity)
                'or Each FilterLetter As String In WS.GetCountryCompanyFilters(Auth)
                AllCountryCompagny.AddRange(WS.GetCountryCompanyList(Auth, "0"))
                'Save compagnies array list
                Me.ViewState.Add(_CountryAllCompagnies, AllCountryCompagny)
            End Using
        End If
    End Sub

    Private Sub SecuredUnsecuredFilesNodeClick(folderIdSelect As String)
        Dim folderId As Guid
        If Not Guid.TryParse(folderIdSelect, folderId) Then
            FileManagementRadGrid.Dispose()
            FileManagementRadGrid.DataSource = Nothing
            FileManagementRadGrid.DataBind()
        ElseIf Not String.IsNullOrEmpty(LangIsocideBox.Text.ToString().Trim()) And Not String.IsNullOrEmpty(CyGlobalId.Text.ToString().Trim()) Then
            Dim user As ClsUser = ClsSessionHelper.LogonUser
            Dim IsocodeLanguage As String = LangIsocideBox.Text.ToString().Trim()
            If user Is Nothing Then
                Return
            End If

            Using WS As New FileServerWS.FileServerWSSoapClient()
                WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentRadComboBox.SelectedValue))
                'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
                'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
                Dim Auth As New FileServerWS.UserContextAuthentication With {
                    .SessionID = New Guid,
                    .CountryID = Guid.Parse(CountryGuid.Value)
                }
                Dim DocumentEntity As FileServerWS.DocumentEntity()
                If SecureUnsecureTabStrip.SelectedIndex = 0 Then
                    DocumentEntity = WS.GetDocumentsList(Auth, folderId)
                Else
                    DocumentEntity = WS.GetDocumentsList(Auth, folderId)
                End If
                FileManagementRadGrid.Dispose()
                FileManagementRadGrid.DataSource = ToDataTable(DocumentEntity)
                FileManagementRadGrid.DataBind()
            End Using
        End If

        Dim visible = FileManagementRadGrid.DataSource.Rows.Count > 0
        FileManagementRadGrid.MasterTableView.GetColumn("Select").Visible = visible
        FileManagementRadGrid.MasterTableView.GetItems(GridItemType.Header)(0).Style("display") = IIf(visible, "table-row", "none")
    End Sub

    Private Sub TreeViewMain(folders As IEnumerable(Of FileServerWS.FolderEntity), mode As String, Optional selectedValue As String = Nothing)
        Dim listId As New List(Of Guid)(folders.Select(Function(y) y.ID).Distinct())
        Dim listNodes As New List(Of RankedRadTreeNode)
        Dim listParentIds As New List(Of String)
        Dim rootFolder As FileServerWS.FolderEntity = folders.FirstOrDefault(Function(x) x.Name = mode)
        Dim treeNode As New RankedRadTreeNode(rootFolder.Name, rootFolder.ID.ToString()) With {
            .ExpandMode = TreeNodeExpandMode.ServerSide,
            .Selected = True,
            .Expanded = True
        }
        AddNodeToTreeView(treeNode, folders, listParentIds)
        listParentIds.Remove(rootFolder.ID.ToString())
        For Each node As RankedRadTreeNode In treeNode.Nodes
            listNodes.Add(node)
        Next

        FileManagementRadTreeView.Nodes.Clear()
        If listNodes.Any() Then
            listNodes = listNodes.OrderBy(Function(x) x.Rank).ToList
            FileManagementRadTreeView.Nodes.AddRange(listNodes)
            For Each parentId In listParentIds
                Dim node As RankedRadTreeNode = FileManagementRadTreeView.FindNodeByValue(parentId)
                Dim nodeList = node.Nodes.Cast(Of RankedRadTreeNode).OrderBy(Function(x) x.Rank).ToList
                node.Nodes.Clear()
                node.Nodes.AddRange(nodeList)
            Next
            If selectedValue Is Nothing Then
                selectedValue = listNodes.First.Value
            End If
            FileManagementRadTreeView.FindNodeByValue(selectedValue).Selected = True
            VisibleFolderBtn(True)
            SecuredUnsecuredFilesNodeClick(FileManagementRadTreeView.SelectedNode.Value)
        Else
            VisibleFolderBtn(False)
        End If
    End Sub

    Private Sub VisibleFolderBtn(visible As Boolean)
        AddFolderbtn.Visible = True
        AddRootFolderbtn.Visible = True
        AddFilebtn.Visible = True
        RenameFolderbtn.Visible = visible
        DeleteFolderbtn.Visible = visible
    End Sub

    Private Sub AddNodeToTreeView(ByRef parentNode As RankedRadTreeNode, folderEntity As IEnumerable(Of FileServerWS.FolderEntity), listParentIds As List(Of String))
        Dim rootValue As String = parentNode.Value
        For Each folder As FileServerWS.FolderEntity In folderEntity.Where(Function(x) x.ParentFolderID.ToString() = rootValue)
            Dim treeNode As New RankedRadTreeNode(folder.Name, folder.ID.ToString(), folder.Rank)
            If folderEntity.Any(Function(y) y.ParentFolderID = folder.ID) Then
                treeNode.ExpandMode = TreeNodeExpandMode.ServerSide
                treeNode.Expanded = True
                AddNodeToTreeView(treeNode, folderEntity, listParentIds)
            End If
            parentNode.Nodes.Add(treeNode)
            If Not listParentIds.Contains(rootValue) Then
                listParentIds.Add(rootValue)
            End If
        Next
    End Sub

    Private Sub AddFolder(parentNode As RadTreeNode)
        Using WS As New FileServerWS.FileServerWSSoapClient()
            WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentRadComboBox.SelectedValue))
            'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
            'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
            Dim Auth As New FileServerWS.UserContextAuthentication With {
                .CountryID = Guid.Parse(CountryGuid.Value),
                .SessionID = New Guid
            }

            Dim mode As String = If(SecureUnsecureTabStrip.SelectedIndex = 0, _Secured, _Unsecured)
            Dim foldersList As List(Of FileServerWS.FolderEntity) = If(mode = _Secured, WS.GetSecuredFoldersStructure(Auth), WS.GetUnsecuredFoldersStructure(Auth)).ToList
            Dim parentId = If(parentNode Is Nothing, foldersList.First(Function(x) x.Name = mode).ID, Guid.Parse(parentNode.Value))
            Dim newFolder = New FileServerWS.FolderEntity With {
                .Name = _NewFolder,
                .ParentFolderID = parentId
            }

            Dim resultFolder As FileServerWS.FolderEntity = WS.CreateFolder(Auth, newFolder)

            If parentNode Is Nothing Then
                For Each folder In foldersList.Where(Function(x) x.ParentFolderID = parentId)
                    folder.Rank += 1
                Next
                foldersList.Add(resultFolder)
                TreeViewMain(foldersList, mode, resultFolder.ID.ToString)
            Else
                Dim newRadTree As New RadTreeNode With {
                    .Value = resultFolder.ID.ToString,
                    .Text = resultFolder.Name,
                    .Selected = True
                }
                parentNode.Nodes.Insert(0, newRadTree)
                parentNode.Expanded = True
                parentNode.Font.Bold = True
            End If

            SecuredUnsecuredFilesNodeClick(resultFolder.ID.ToString)
        End Using
    End Sub

    Private Shared Function ConvertIsocodLanguageToCountryID(IsocodeLanguage As String) As Guid
        Try
            Dim cnx As SqlConnection
            Dim cmd As SqlCommand
            Dim Adapter As SqlDataAdapter
            Dim ds As DataSet
            Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
            cnx = New SqlConnection(ConfigurationManager.ConnectionStrings("logDB").ConnectionString)
            cnx.Open()
            cmd = New SqlCommand("[dbo].[GetLanguage]", cnx)
            cmd.Parameters.AddWithValue("@EnvironmentID", 17)
            cmd.Parameters.AddWithValue("@IsocodeLanguage", IsocodeLanguage)
            cmd.CommandType = CommandType.StoredProcedure
            Adapter = New SqlDataAdapter(cmd)
            ds = New DataSet
            Adapter.Fill(ds, "Result")
            Dim CountryID As Object = ds.Tables(0).Rows(0)(0)
            Dim result As New Guid(CountryID.ToString())
            Return result
        Catch ex As Exception
            Dim toto As String = ex.Message
        End Try
    End Function

    Private Function ToDataTable(collection As IEnumerable(Of FileServerWS.DocumentEntity)) As DataTable
        Dim dt As New DataTable("DataTable")
        Dim type As Type = GetType(FileServerWS.DocumentEntity)
        Dim pia As PropertyInfo() = type.GetProperties()

        For Each pi As PropertyInfo In pia
            Dim ColumnType As Type = pi.PropertyType
            If ColumnType.IsGenericType Then
                ColumnType = ColumnType.GetGenericArguments()(0)
            End If
            If pi.Name = "Type" Then
                dt.Columns.Add("Type")
                dt.Columns.Add("Extension")
            Else
                dt.Columns.Add(pi.Name, ColumnType)
            End If
        Next

        For Each item As FileServerWS.DocumentEntity In collection
            Dim dr As DataRow = dt.NewRow()
            dr.BeginEdit()
            For Each pi As PropertyInfo In pia

                If pi.GetValue(item, Nothing) IsNot Nothing Then
                    dr(pi.Name) = pi.GetValue(item, Nothing)
                    If pi.Name = "Type" Then
                        dr(pi.Name) = item.Type.Name
                        dr("Extension") = item.Type.Extension
                    End If
                End If

            Next
            dr.EndEdit()
            dt.Rows.Add(dr)
        Next
        Return dt
    End Function

    Private Sub RadTreeViewContextMenu(nodeSelect As RadTreeNode, valueItem As String)
        Select Case valueItem
            Case _NewFolder
                AddFolder(nodeSelect)
                Exit Select
            Case "Rename"
                Exit Select
            Case "Delete"
                DeleteFolder()
                Exit Select
        End Select
    End Sub

    Private Sub DeleteFolder()
        Dim treeNodeSelect As RadTreeNode = FileManagementRadTreeView.SelectedNode
        Dim treeNodeParent As RadTreeNode = treeNodeSelect.ParentNode

        Dim ret As DataTable
        Using WS As New FileServerWS.FileServerWSSoapClient()
            WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentRadComboBox.SelectedValue))
            'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
            'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
            Dim Auth As New FileServerWS.UserContextAuthentication With {
                .CountryID = Guid.Parse(CountryGuid.Value),
                .SessionID = New Guid
            }
            ret = WS.DeleteFolder(Auth, Guid.Parse(treeNodeSelect.Value))
        End Using

        For Each row As DataRow In ret.Rows
            Dim service = New B2BManagerService()
            Dim customerIds = If(Not String.IsNullOrEmpty(row("compIds").ToString), row("compIds").ToString, Nothing)
            service.AssignmentLogs(
                ClsHelper.AssignmentType.Unassigned,
                EnvironmentRadComboBox.SelectedValue,
                CountryGuid.Value,
                ClsHelper.ObjectType.File,
                row("docId").ToString,
                customerIds)
        Next

        treeNodeSelect.Remove()
        If treeNodeParent IsNot Nothing Then
            treeNodeParent.Selected = True
            SecuredUnsecuredFilesNodeClick(treeNodeParent.Value)
        End If
    End Sub

    Private Sub FileManagerSecure(fileIds As Guid())
        FilesManagerManageSecurity.SecuredUnsecured = SecureUnsecureTabStrip.SelectedTab.Text
        Using WS As New FileServerWS.FileServerWSSoapClient()
            WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentRadComboBox.SelectedValue))
            'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
            'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
            Dim Auth As New FileServerWS.UserContextAuthentication With {
                .CountryID = Guid.Parse(CountryGuid.Value),
                .SessionID = New Guid
            }
            Dim folderId As Guid = Guid.Parse(FileManagementRadTreeView.SelectedNode.Value)
            FilesManagerManageSecurity.FolderGuidId = folderId
            Dim files = WS.GetDocumentsList(Auth, folderId)
            FilesManagerManageSecurity.FileSelect = files.Where(Function(x) fileIds.Contains(x.ID)).ToArray
            If Me.ViewState(_CountryAllCompagnies) IsNot Nothing Then
                FilesManagerManageSecurity.CountryAllCompagnies = CType(Me.ViewState(_CountryAllCompagnies), List(Of FileServerWS.CompanyEntity))
            Else
                FilesManagerManageSecurity.CountryAllCompagnies = New List(Of FileServerWS.CompanyEntity)
            End If
        End Using
        FilesManagerManageSecurity.EnvironmentID = EnvironmentRadComboBox.SelectedValue
        FilesManagerManageSecurity.CountryGuid = Guid.Parse(CountryGuid.Value)
        FilesManagerManageSecurity.SopId = CountryRadComboBox.SelectedValue
    End Sub

    Private Sub FilesManager_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            RenderControls()
        End If

        FileUploapErrorLabel.Text = String.Empty
        InitAjaxSettings()

        If Request.Form("__EVENTTARGET") = "FileManagementRadTreeViewNodeClick" Then
            RefreshSelectedFolder()
        End If
    End Sub

    Protected Sub SecureUnsecureTabStrip_TabClick(sender As Object, e As RadTabStripEventArgs)
        SearchFolder()
        For Each column In FileManagementRadGrid.Columns
            If TypeOf column Is GridBoundColumn AndAlso DirectCast(column, GridBoundColumn).DataField = "CompaniesCount" Then
                Dim colonne As GridBoundColumn = DirectCast(column, GridBoundColumn)
                colonne.Visible = SecureUnsecureTabStrip.SelectedIndex = 0
                Exit For
            End If
        Next
    End Sub

    Protected Sub OKbtnClick(sender As Object, e As EventArgs)
        Dim langRow As DataRow() = GetLanguagesFromEnvironment(CountryRadComboBox.SelectedValue)
        If langRow IsNot Nothing Then
            LangIsocideBox.Text = langRow(0)("LANG_ISOCODE")
            CyGlobalId.Text = langRow(0)("CY_GLOBALID").ToString()
            CountryGuid.Value = ConvertIsocodLanguageToCountryID(LangIsocideBox.Text.ToString().Trim()).ToString().Trim()
            SearchFolder()
            StoreCompagnies()

            ManageSecurityPanel.Style("display") = "none"
            FileManagerPanel.Style("display") = "block"
        End If
    End Sub

    Protected Sub RadTreeView_NodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs)
        SecuredUnsecuredFilesNodeClick(e.Node.Value)
        For Each item In FileManagementRadGrid.Items
            item.Selected = False
        Next
        TopBtn.Style.Add("display", "none")
        UpBtn.Style.Add("display", "none")
        DownBtn.Style.Add("display", "none")
        BottomBtn.Style.Add("display", "none")
    End Sub

    Protected Sub RadTreeView_ContextMenuItemClick(ByVal sender As Object, ByVal e As RadTreeViewContextMenuEventArgs)
        RadTreeViewContextMenu(e.Node, e.MenuItem.Value)
    End Sub

    Protected Sub RadTreeView_NodeEdit(sender As Object, e As RadTreeNodeEditEventArgs)
        Dim newFolder As New FileServerWS.FolderEntity
        Try
            Using WS As New FileServerWS.FileServerWSSoapClient()
                WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentRadComboBox.SelectedValue))
                'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
                'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
                Dim Auth As New FileServerWS.UserContextAuthentication With {
                    .CountryID = Guid.Parse(CountryGuid.Value),
                    .SessionID = Guid.NewGuid
                }
                Dim folder = New FileServerWS.FolderEntity With {
                    .ID = Guid.Parse(e.Node.Value),
                    .Name = e.Text.Trim()
                }
                newFolder = WS.EditFolderName(Auth, folder)
            End Using
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        Finally
            e.Node.Text = newFolder.Name
        End Try
    End Sub

    Protected Sub AddFolderbtn_Click(sender As Object, e As EventArgs)
        RadTreeViewContextMenu(FileManagementRadTreeView.SelectedNode, _NewFolder)
    End Sub

    Protected Sub AddRootFolderbtn_Click(sender As Object, e As EventArgs)
        RadTreeViewContextMenu(Nothing, _NewFolder)
    End Sub

    Protected Sub DeleteFolderbtn_Click(sender As Object, e As EventArgs)
        DeleteFolder()
    End Sub

    Protected Sub FileUploadbtn_Click(sender As Object, e As EventArgs)
        If FileUploadSW.PostedFiles.Count = 0 Then
            FileUploapErrorLabel.Text = "The upload has failed, please try again."
            Return
        End If
        Try
            Dim userId = ClsSessionHelper.LogonUser.GlobalID
            Dim folderId As Guid = Guid.Parse(FileManagementRadTreeView.SelectedNode.Value)
            Dim tasks As New List(Of Task(Of FileServerWS.DocumentEntity))
            For Each file As HttpPostedFile In FileUploadSW.PostedFiles
                tasks.Add(Task.Run(Function() FileUpload(userId, folderId, file)))
            Next

            Dim newFiles = Task.WhenAll(tasks).Result

            SecuredUnsecuredFilesNodeClick(folderId.ToString())

            For Each newFile As FileServerWS.DocumentEntity In newFiles
                Dim GridDataItemList As List(Of GridDataItem) = FileManagementRadGrid.Items.OfType(Of GridDataItem).ToList().Where(Function(x) x("Name").Text = newFile.Name).ToList()
                If GridDataItemList.Count = 0 Then
                    FileUploapErrorLabel.Text += "The file " + newFile.Name + " has some errors. "
                End If
            Next

            If Not String.IsNullOrEmpty(FileUploapErrorLabel.Text) Then Return

            If FileUploadSW.PostedFiles.Count > 1 AndAlso SecureUnsecureTabStrip.SelectedIndex = 0 Then
                FileUploapErrorLabel.Text = "Files uploaded."
                Return
            End If

            ManageSecurityPanel.Style("display") = "block"
            FileManagerPanel.Style("display") = "none"
            FileManagerSecure(newFiles.Select(Function(x) x.ID).ToArray)
        Catch ex As Exception
            FileUploapErrorLabel.Text = ex.Message
        End Try
    End Sub

    Private Function FileUpload(userId As Guid, folderId As Guid, file As HttpPostedFile) As FileServerWS.DocumentEntity
        Using WS As New FileServerWS.FileServerWSSoapClient()
            WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentRadComboBox.SelectedValue))
            'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
            'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
            Dim Auth As New FileServerWS.UserContextAuthentication With {
                .CountryID = Guid.Parse(CountryGuid.Value),
                .SessionID = New Guid
            }

            Dim fileExtension As New FileServerWS.FileTypeEntity With {
                .Extension = Path.GetExtension(file.FileName),
                .ID = Guid.NewGuid,
                .Name = file.ContentType
            }
            ' Buffer file once
            Dim fileBytes As Byte() = New Byte(file.ContentLength - 1) {}
            file.InputStream.Read(fileBytes, 0, file.ContentLength)

            Dim doc As New FileServerWS.DocumentEntity With {
                .Name = file.FileName,
                .Content = fileBytes,
                .Type = fileExtension,
                .ParentFolderID = folderId,
                .Size = fileBytes.Length
            }

            Dim newDoc As FileServerWS.DocumentEntity = WS.UploadFile(Auth, doc)
            Dim thumbnailId As Guid
            If IsImageContentType(file.ContentType) Then
                Dim resizedBytes As Byte() = ResizeImage(fileBytes, 100, 80, Path.GetExtension(newDoc.Name))
                If resizedBytes IsNot Nothing Then
                    thumbnailId = UploadThumbnailToServer(userId, WS, Auth, newDoc.ID, newDoc.Name, resizedBytes)
                End If
            Else
                Dim ext = IO.Path.GetExtension(newDoc.Name)
                Dim icon = DefaultIconForExtension(ext)
                Dim path = Server.MapPath("~/Images/Thumbnails/" & icon)

                If IO.File.Exists(path) Then
                    Dim bytes = IO.File.ReadAllBytes(path)
                    thumbnailId = UploadThumbnailToServer(userId, WS, Auth, newDoc.ID, icon, bytes)
                End If
            End If

            Dim success = WS.UpdateDocumentPublicationParameters(Auth, {newDoc.ID}, thumbnailId, False, Date.Now, Date.Now, False)
            Return newDoc
        End Using
    End Function

    Private Function UploadThumbnailToServer(userId As Guid, WS As FileServerWS.FileServerWSSoapClient, auth As FileServerWS.UserContextAuthentication, docId As Guid, fileName As String, fileBytes As Byte()) As Guid
        Dim thumbnail As New FileServerWS.ThumbnailEntity With {
            .Name = fileName,
            .Content = fileBytes,
            .Size = fileBytes.Length,
            .CreationDate = Now,
            .Extension = Path.GetExtension(fileName),
            .CreatedBy = userId,
            .DocumentID = docId
        }

        Dim thumbnailId = WS.UploadThumbnail(auth, thumbnail)
        '' Update labels
        CType(FilesManagerManageSecurity.FindControl("ThumbnailGuidLabel"), Label).Text = thumbnailId.ToString()
        CType(FilesManagerManageSecurity.FindControl("ThumbnailNameLabel"), Label).Text = thumbnail.Name
        CType(FilesManagerManageSecurity.FindControl("ImageThumbnail"), Image).ImageUrl = "data:image;base64," & Convert.ToBase64String(thumbnail.Content)
        Return thumbnailId
    End Function

    Protected Sub RankingBtn_Click(sender As Object, e As EventArgs)
        Dim selectedIds = New List(Of Guid)
        For Each item In FileManagementRadGrid.SelectedItems
            Dim id = item.GetDataKeyValue("ID").ToString
            selectedIds.Add(Guid.Parse(id))
        Next
        Dim rankCursor As Integer
        Select Case sender.ID
            Case "TopBtn"
                rankCursor = 10
            Case "UpBtn"
                rankCursor = 1
            Case "DownBtn"
                rankCursor = -1
            Case Else
                rankCursor = -10
        End Select
        Using WS As New FileServerWS.FileServerWSSoapClient()
            WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentRadComboBox.SelectedValue))
            'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
            'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
            Dim Auth As New FileServerWS.UserContextAuthentication With {
                    .CountryID = Guid.Parse(CountryGuid.Value),
                    .SessionID = New Guid
            }
            Dim res = WS.DocumentsRanking(Auth, selectedIds.ToArray, rankCursor)
            RefreshSelectedFolder()
            For Each item In FileManagementRadGrid.Items
                Dim id = Guid.Parse(item.GetDataKeyValue("ID").ToString)
                If selectedIds.Contains(id) Then
                    item.Selected = True
                End If
            Next
            TopBtn.Style.Add("display", "inline")
            UpBtn.Style.Add("display", "inline")
            DownBtn.Style.Add("display", "inline")
            BottomBtn.Style.Add("display", "inline")
        End Using
    End Sub

    Protected Sub FolderRankingBtn_Click(sender As Object, e As EventArgs)
        Dim rankCursor As Integer
        Select Case sender.ID
            Case "FolderTopBtn"
                rankCursor = 10
            Case "FolderUpBtn"
                rankCursor = 1
            Case "FolderDownBtn"
                rankCursor = -1
            Case Else
                rankCursor = -10
        End Select
        Using WS As New FileServerWS.FileServerWSSoapClient()
            WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentRadComboBox.SelectedValue))
            'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
            'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
            Dim Auth As New FileServerWS.UserContextAuthentication With {
                    .CountryID = Guid.Parse(CountryGuid.Value),
                    .SessionID = New Guid
            }
            Dim folderIdString = FileManagementRadTreeView.SelectedNode.Value
            Dim res = WS.FolderRanking(Auth, Guid.Parse(folderIdString), rankCursor)

            Dim mode As String = If(SecureUnsecureTabStrip.SelectedIndex = 0, _Secured, _Unsecured)
            Dim foldersList As List(Of FileServerWS.FolderEntity) = If(mode = _Secured, WS.GetSecuredFoldersStructure(Auth), WS.GetUnsecuredFoldersStructure(Auth)).ToList
            TreeViewMain(foldersList, mode, folderIdString)
        End Using
    End Sub

    Protected Sub RadGrid_OnCommand(source As Object, e As GridCommandEventArgs)
        If e.CommandName = "Delete" Then
            Dim idFile As Guid = Guid.Parse(FileManagementRadGrid.Items.Item(e.Item.ItemIndex).GetDataKeyValue("ID").ToString())
            DeleteFile(idFile)
            RefreshSelectedFolder()
        ElseIf e.CommandName = "ManageSecurity" Then
            ManageSecurityPanel.Style("display") = "block"
            FileManagerPanel.Style("display") = "none"
            Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)
            Dim fileId As Guid = item.GetDataKeyValue("ID")
            FileManagerSecure({fileId})
        ElseIf e.CommandName = "Sort" Or e.CommandName = "ChangePageSize" Or e.CommandName = "Page" Then
            RefreshSelectedFolder()
        ElseIf e.CommandName = "Download" Then
            Dim idFile As Guid = Guid.Parse(FileManagementRadGrid.Items.Item(e.Item.ItemIndex).GetDataKeyValue("ID").ToString())
            DownloadFile(idFile)
        End If
    End Sub

    Protected Sub FilesManagerManageSecurity_BackFileUploadBtnClick(sender As Object, e As EventArgs)
        ManageSecurityPanel.Style("display") = "none"
        FileManagerPanel.Style("display") = "block"
        RefreshSelectedFolder()
    End Sub

    Private Sub DownloadFile(ByVal documentID As Guid)
        Using WS As New FileServerWS.FileServerWSSoapClient()
            WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentRadComboBox.SelectedValue))
            'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
            'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
            Dim Auth As New FileServerWS.UserContextAuthentication With {
                .CountryID = Guid.Parse(CountryGuid.Value),
                .SessionID = New Guid
            }
            Dim aDocumentEntity As FileServerWS.DocumentEntity = WS.GetDocument(Auth, documentID)
            Response.Clear()
            Response.ContentType = "application/octet-stream; name=" & aDocumentEntity.Name.Replace(" ", "_")
            Response.AddHeader("content-transfer-encoding", "binary")
            Response.AddHeader("content-disposition", "attachment;filename=" & aDocumentEntity.Name.Replace(" ", "_"))
            Response.ContentEncoding = Encoding.GetEncoding(1251)
            Response.BinaryWrite(aDocumentEntity.Content)
            'Response.Flush()
            Response.End()
        End Using
    End Sub

#Region "WebMethod"

    <WebMethod()>
    Public Shared Function AssignedCompagnies(Type As Integer, CompanyIds As String, CountryId As String, DocumentIds As String, EnvironmentId As String) As String
        If CompanyIds Is Nothing Then Return "There are no compagnies to assign."

        Using WS As New FileServerWS.FileServerWSSoapClient()
            WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentId))
            'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
            'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
            Dim Auth As New FileServerWS.UserContextAuthentication With {
                .CountryID = Guid.Parse(CountryId),
                .SessionID = New Guid
            }

            Dim dociIds = DocumentIds.Split(";").Select(Function(x) Guid.Parse(x)).ToArray()
            ' We have dociIds.Count > 1 only for documents just uploaded. So their assigned companies are identical
            Dim compIds = CompanyIds.Split(";").Select(Function(x) Guid.Parse(x)).ToArray()
            WS.SetAssignement(Auth, Type = 1, dociIds, compIds)
        End Using

        Return "Success"
    End Function

    <WebMethod()>
    Public Shared Function RowDropped(DocumentIdsSource As Guid(), DocumentIdTarget As Guid, IsBefore As Boolean, EnvironmentId As Integer, CountryId As Guid) As Boolean
        Using WS As New FileServerWS.FileServerWSSoapClient()
            WS.Endpoint.Address = New ServiceModel.EndpointAddress(ClsEbusinessHelper.GetFileServerUrl(EnvironmentId))
            'WS.Endpoint.Address = New ServiceModel.EndpointAddress("http://localhost:26807/FileServerWS.asmx")
            'WS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation
            Dim Auth As New FileServerWS.UserContextAuthentication With {
                    .CountryID = CountryId,
                    .SessionID = New Guid
            }
            Dim res = WS.RowDropped(Auth, DocumentIdsSource, DocumentIdTarget, IsBefore)
            Return res
        End Using
    End Function

#End Region

End Class