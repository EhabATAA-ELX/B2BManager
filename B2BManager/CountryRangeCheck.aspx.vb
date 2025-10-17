
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.IO
Imports Telerik.Web.UI

Partial Class CountryRangeCheck
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx?ReturnURL=CountryRangeCheck.aspx", True)
            Return
        End If
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim target As String = TryCast(Request("__EVENTTARGET"), String)
        If Not IsPostBack Then ' Refresh data on reload
            countryRangeEnvironments = Nothing
            countryRangeData = Nothing
            btnResetFileCache.Visible = ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.RESET_FILES_CACHE)
            btnDownloadFile.Visible = ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.DOWNLOAD_COUNTRY_RANGE_FILES)
            ddlEnvironment.DataSource = countryRangeEnvironments
            ddlEnvironment.DataBind()
            gridCountryRangeData.DataSource = countryRangeData
            gridCountryRangeData.DataBind()
            RenderCountryDropDown()
            RefreshFilesTreeView()
            watch.Stop()
            ClsHelper.Log("Check Country Range Load", ClsSessionHelper.LogonUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, False, Nothing)
        End If

    End Sub

    Private Sub RenderCountryDropDown()
        Dim SelectedValue As String = ddlCountry.SelectedValue
        ddlCountry.Items.Clear()
        If countryRangeData.Rows.Count > 1 Then
            Dim allCountriesSOPs As String = "All"
            If countryRangeData.Rows.Count < 30 Then
                Dim SOPList As List(Of String) = New List(Of String)
                For Each row As DataRow In countryRangeData.Rows
                    SOPList.Add("[" + row("SOPNAME") + "]")
                Next
                allCountriesSOPs = String.Join(",", SOPList)
            End If
            ddlCountry.Items.Insert(0, New RadComboBoxItem("All", allCountriesSOPs))
            ddlCountry.Items(0).Selected = True
        End If
        Dim ValueExists As Boolean = False
        For Each row As DataRow In countryRangeData.Rows
            Dim item As RadComboBoxItem = New RadComboBoxItem(row("Name"), "[" + row("SOPNAME") + "]")
            item.ImageUrl = row("ImageUrl")
            ddlCountry.Items.Add(item)
            If row("SOPNAME") = SelectedValue Then
                ValueExists = True
                ddlCountry.SelectedValue = item.Value
            End If
        Next
        If ddlCountry.Items.Count > 1 And Not ValueExists Then
            ddlCountry.SelectedValue = SelectedValue
        End If
    End Sub

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx?ReturnURL=CountryRangeCheck.aspx", True)
        End If
    End Sub

    Public Property countryRangeEnvironments As DataTable
        Get
            If Session("countryRangeEnvironments") Is Nothing Then
                Session("countryRangeEnvironments") = ClsDataAccessHelper.FillDataTable("[CountryRange].[Get_Environments]", Nothing)
            End If
            Return CType(Session("countryRangeEnvironments"), DataTable)
        End Get
        Set(value As DataTable)
            Session("countryRangeEnvironments") = value
        End Set
    End Property

    Private Sub AddNode(foderName As String, fieldPathName As String, Optional loadOnDemand As Boolean = False)
        If countryRangeEnvironments.Select("ID='" + ddlEnvironment.SelectedValue + "'")(0)(fieldPathName) IsNot DBNull.Value Then
            If Not String.IsNullOrEmpty(countryRangeEnvironments.Select("ID='" + ddlEnvironment.SelectedValue + "'")(0)(fieldPathName)) Then
                Dim rootNode As RadTreeNode = New RadTreeNode(foderName, fieldPathName)
                rootNode.ImageUrl = "Images/folder.png"
                rootNode.Expanded = Not loadOnDemand
                rootNode.ExpandMode = IIf(loadOnDemand, TreeNodeExpandMode.ServerSideCallBack, TreeNodeExpandMode.ClientSide)
                If Not loadOnDemand Then
                    AddNodeToParent(rootNode)
                End If
                tvFiltesToProcess.Nodes.Add(rootNode)
            End If
        End If
    End Sub

    Private Sub AddNodeToParent(ByRef rootNode As RadTreeNode)
        Dim files As List(Of ClsFile) = ClsSessionHelper.countryRangeFiles
        Dim filesInfo As FileInfo() = Nothing
        If Cache(String.Format("FileInfo_{0}_{1}", ddlEnvironment.SelectedValue.ToString(), rootNode.Value)) Is Nothing Then
            Dim filePath As String = countryRangeEnvironments.Select("ID='" + ddlEnvironment.SelectedValue + "'")(0)(rootNode.Value)
            Dim di As IO.DirectoryInfo = New DirectoryInfo(filePath)
            filesInfo = di.GetFiles()
            If rootNode.Value IsNot "SourceFilePath" Then
                Cache.Insert(String.Format("FileInfo_{0}_{1}", ddlEnvironment.SelectedValue.ToString(), rootNode.Value), filesInfo)
            End If
        Else
            filesInfo = DirectCast(Cache(String.Format("FileInfo_{0}_{1}", ddlEnvironment.SelectedValue.ToString(), rootNode.Value)), FileInfo())
            rootNode.Text += " (Served from Files Cache)"
        End If

        For Each file As FileInfo In filesInfo.Where(Function(fc) fc.CreationTime >= DateTime.Now.AddDays(-1 * txtDaysCount.Value) And (ddlCountry.SelectedValue = "All" Or ddlCountry.SelectedValue.Contains("[" + fc.Name.Replace("PRODAVAIL_", "").Substring(0, fc.Name.Replace("PRODAVAIL_", "").IndexOf("_")) + "]"))).OrderByDescending(Function(fc) fc.CreationTime).ToList
            Dim clsFile As ClsFile = New ClsFile()
            clsFile.FileInfo = file
            clsFile.ID = Guid.NewGuid().ToString().GetHashCode().ToString("x")
            Dim fileNode As RadTreeNode = New RadTreeNode("[" + file.CreationTime.ToString("dd/MM/yyyy HH:mm") + "] " + file.Name, clsFile.ID)
            fileNode.ImageUrl = "Images/FileFlags/" + file.Name.Replace("PRODAVAIL_", "").Substring(0, file.Name.Replace("PRODAVAIL_", "").IndexOf("_")) + ".png"
            fileNode.ViewStateMode = ViewStateMode.Disabled
            rootNode.Nodes.Add(fileNode)
            files.Add(clsFile)
        Next
        ClsSessionHelper.countryRangeFiles = files
    End Sub

    Public Property countryRangeData As DataTable
        Get
            If Session("countryRangeData") Is Nothing Then
                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                Dim selectedApplication As ClsHelper.Application = ClsSessionHelper.LogonUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
                If selectedApplication IsNot Nothing Then
                    If selectedApplication.Countries.Count > 0 Then
                        Dim allCountriesSOPs As String = "All"
                        If selectedApplication.Countries.Where(Function(fc) fc.Checked).Count < 30 Then
                            allCountriesSOPs = String.Join(",", selectedApplication.Countries.Where(Function(fc) fc.Checked).Select(Function(fc) fc.SOP_ID))
                        End If

                        If Not allCountriesSOPs.Equals("All") Then
                            parameters.Add(New SqlParameter("@ApplicableSOPIDs", allCountriesSOPs))
                        End If
                    End If
                End If
                parameters.Add(New SqlParameter("@EnvironmentID", Integer.Parse(ddlEnvironment.SelectedValue)))
                Session("countryRangeData") = ClsDataAccessHelper.FillDataTable("[CountryRange].[Get_SopInfo]", parameters)
            End If
            Return CType(Session("countryRangeData"), DataTable)
        End Get
        Set(value As DataTable)
            Session("countryRangeData") = value
        End Set
    End Property

    Public Property LogsData As DataTable
        Get
            If Session("LogsData") Is Nothing Then
                Session("LogsData") = New DataTable
            End If
            Return CType(Session("LogsData"), DataTable)
        End Get
        Set(value As DataTable)
            Session("LogsData") = value
        End Set
    End Property

    Public Property InputProductsData As DataTable
        Get
            If Session("InputProductsData") Is Nothing Then
                Session("InputProductsData") = New DataTable
            End If
            Return CType(Session("InputProductsData"), DataTable)
        End Get
        Set(value As DataTable)
            Session("InputProductsData") = value
        End Set
    End Property

    Protected Sub tvFiltesToProcess_NodeClick(sender As Object, e As RadTreeNodeEventArgs)
        LogsData = Nothing
        InputProductsData = Nothing
        If ClsSessionHelper.countryRangeFiles.Count > 0 Then

            Dim clsSelectedFile As ClsFile = ClsSessionHelper.countryRangeFiles.FirstOrDefault(Function(fc) fc.ID = e.Node.Value)
            If clsSelectedFile IsNot Nothing Then
                Dim watch As Stopwatch = Stopwatch.StartNew()
                Dim fileInfo As FileInfo = clsSelectedFile.FileInfo
                Dim IsRUSFile As Boolean = fileInfo.Name.StartsWith("PRODAVAIL_RUS_")
                lblFileName.Text = fileInfo.Name
                lblFilePath.Text = fileInfo.FullName
                lblCreationDate.Text = fileInfo.CreationTime.ToString("dd/MM/yyyy HH:mm")
                lblLastModificationDate.Text = fileInfo.LastAccessTime.ToString("dd/MM/yyyy HH:mm")
                lblSize.Text = ClsHelper.SizeSuffix(fileInfo.Length)
                selectedFileID.Value = e.Node.Value
                pnlFileInfo.Visible = True
                trIntegratedProducts.Visible = True
                pnlCompareWithEdenData.Visible = Not IsRUSFile And ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.COMPARE_COUNTRY_RANGE_FILES_WITH_EDEN_DATA)
                If e.Node.ParentNode.Value = "SourceFilePath" Then
                    RadTabStripFileDetails.Visible = False
                    RadMultiPageFileDetails.Visible = False
                    lblTotalProducts.Text = "Not yet integrated"
                    lblInputFileID.Text = "Not yet integrated"
                    lblTotalProducts.ForeColor = System.Drawing.Color.Black
                Else
                    RadTabStripFileDetails.Visible = True
                    RadMultiPageFileDetails.Visible = True
                    Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                    parameters.Add(New SqlParameter("@EnvironmentID", Integer.Parse(ddlEnvironment.SelectedValue)))
                    parameters.Add(New SqlParameter("@FileName", fileInfo.Name))
                    Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("[CountryRange].[Get_FileLog]", parameters)
                    If (dataSet.Tables.Count = 3) Then
                        lblInputFileID.Text = dataSet.Tables(0).Rows(0)("InputFileID").ToString()
                        If e.Node.ParentNode.Value = "SuccessFilePath" And Not IsRUSFile Then
                            lblTotalProducts.Text = dataSet.Tables(0).Rows(0)("TotalProductCount").ToString()
                            RadGridInputProducts.DataSource = dataSet.Tables(1)
                            InputProductsData = dataSet.Tables(1)
                            RadMultiPageFileDetails.PageViews(1).Visible = True
                            RadTabStripFileDetails.Tabs(1).Visible = True
                            lblTotalProducts.ForeColor = System.Drawing.Color.Green
                        Else
                            If Not IsRUSFile Then
                                lblTotalProducts.Text = "Integration failed"
                                lblTotalProducts.ForeColor = System.Drawing.Color.Red
                            Else
                                trIntegratedProducts.Visible = False
                            End If
                            RadMultiPageFileDetails.SelectedIndex = 0
                            RadMultiPageFileDetails.PageViews(1).Visible = False
                            RadTabStripFileDetails.Tabs(1).Visible = False
                        End If
                        LogsData = dataSet.Tables(2)
                        RadGridLogs.DataSource = dataSet.Tables(2)
                    Else
                        lblTotalProducts.Text = "--"
                        lblInputFileID.Text = "--"
                    End If
                    RadGridInputProducts.DataBind()
                    RadGridLogs.DataBind()
                End If
                ClsHelper.Log("Check Country Range View File", ClsSessionHelper.LogonUser.GlobalID.ToString(), "<b>File name</b>: " + fileInfo.Name, watch.ElapsedMilliseconds, False, Nothing)
            Else
                lblTotalProducts.Text = ""
                lblInputFileID.Text = ""
                lblFileName.Text = ""
                lblFilePath.Text = ""
                lblCreationDate.Text = ""
                lblLastModificationDate.Text = ""
                selectedFileID.Value = ""
                lblSize.Text = ""
                pnlFileInfo.Visible = False
            End If
        End If
    End Sub
    Protected Sub btnDownloadFile_Click(sender As Object, e As EventArgs)
        If selectedFileID.Value IsNot "" AndAlso ClsSessionHelper.countryRangeFiles.Count > 0 Then
            Dim clsSelectedFile As ClsFile = ClsSessionHelper.countryRangeFiles.FirstOrDefault(Function(fc) fc.ID = selectedFileID.Value)
            If clsSelectedFile IsNot Nothing Then
                Dim watch As Stopwatch = Stopwatch.StartNew()
                Dim fileInfo As FileInfo = clsSelectedFile.FileInfo
                Dim fileName As String = IIf(fileInfo.Name.IndexOf(".") >= 0, fileInfo.Name.Substring(0, fileInfo.Name.IndexOf(".") + 4), fileInfo.Name)
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName)
                Response.ContentType = "application/xml"
                Response.Clear()
                Response.Flush()
                Response.WriteFile(fileInfo.FullName)
                ClsHelper.Log("Check Country Range Download File", ClsSessionHelper.LogonUser.GlobalID.ToString(), "<b>File name</b>: " + fileInfo.Name, watch.ElapsedMilliseconds, False, Nothing)
                Response.End()
            End If
        End If
    End Sub
    Protected Sub tvFiltesToProcess_NodeExpand(sender As Object, e As RadTreeNodeEventArgs)
        If e.Node.Nodes.Count = 0 Then
            PopulateNodeOnDemand(e)
        End If
    End Sub

    Protected Sub PopulateNodeOnDemand(ByVal e As RadTreeNodeEventArgs)
        AddNodeToParent(e.Node)
        e.Node.Expanded = True
        e.Node.ExpandMode = TreeNodeExpandMode.ClientSide
    End Sub
    Protected Sub ddlCountry_SelectedIndexChanged(o As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        RefreshFilesTreeView()
    End Sub
    Protected Sub btnRefreshFiles_Click(sender As Object, e As EventArgs)
        RefreshFilesTreeView()
    End Sub
    Protected Sub btnRefresh_Click(sender As Object, e As EventArgs)
        countryRangeEnvironments = Nothing
        countryRangeData = Nothing
        ddlEnvironment.DataSource = countryRangeEnvironments
        ddlEnvironment.DataBind()
        gridCountryRangeData.DataSource = countryRangeData
        gridCountryRangeData.DataBind()
        RenderCountryDropDown()
        RefreshFilesTreeView()
    End Sub

    Private Sub RefreshFilesTreeView()
        ClsSessionHelper.countryRangeFiles = Nothing
        pnlFileInfo.Visible = False
        tvFiltesToProcess.Nodes.Clear()
        If (ddlEnvironment.SelectedValue IsNot "") AndAlso tvFiltesToProcess.Nodes.Count = 0 Then
            AddNode("Files to Process", "SourceFilePath")
            AddNode("<span style='color:red'>Files generated an integration Error</span>", "ErrorFilePath")
            AddNode("<span style='color:green'>Files integrated with Success</span>", "SuccessFilePath", Cache(String.Format("FileInfo_{0}_{1}", ddlEnvironment.SelectedValue.ToString(), "SuccessFilePath")) Is Nothing)
            AddNode("Ignored files", "IgnoreFilePath", Cache(String.Format("FileInfo_{0}_{1}", ddlEnvironment.SelectedValue.ToString(), "IgnoreFilePath")) Is Nothing)
        End If
    End Sub
    Protected Sub RadGridLogs_NeedDataSource(source As Object, e As GridNeedDataSourceEventArgs)
        RadGridLogs.DataSource = LogsData
    End Sub
    Protected Sub RadGridInputProducts_NeedDataSource(source As Object, e As GridNeedDataSourceEventArgs)
        RadGridInputProducts.DataSource = InputProductsData
    End Sub
    Protected Sub gridCountryRangeData_NeedDataSource(source As Object, e As GridNeedDataSourceEventArgs)
        gridCountryRangeData.DataSource = countryRangeData
    End Sub
    Protected Sub btnResetFileCache_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim cacheItemsToRemove As List(Of String) = New List(Of String)()
        For Each item As DictionaryEntry In Cache
            If (item.Key.ToString().StartsWith(String.Format("FileInfo_{0}", ddlEnvironment.SelectedValue.ToString()))) Then
                cacheItemsToRemove.Add(item.Key)
            End If
        Next

        For Each itemToRemove In cacheItemsToRemove
            Cache.Remove(itemToRemove)
        Next

        RefreshFilesTreeView()
        ClsHelper.Log("Check Country Range Clear Cache", ClsSessionHelper.LogonUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, False, Nothing)
    End Sub
    Protected Sub ddlEnvironment_SelectedIndexChanged(sender As Object, e As EventArgs)
        countryRangeData = Nothing
        gridCountryRangeData.DataSource = countryRangeData
        gridCountryRangeData.DataBind()
        RenderCountryDropDown()
        RefreshFilesTreeView()
    End Sub
End Class
