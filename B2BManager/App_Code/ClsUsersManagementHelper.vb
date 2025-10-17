Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports Microsoft.VisualBasic
Imports Telerik.Web.UI

Public Class ClsUsersManagementHelper

    Public Const CacheCountriesKey As String = "Users_Countries"
    Public Const CacheToolsAndActionsKey As String = "Users_ToolsAndActions"
    Private Shared Function AddTreeItem(root As ClsHelper.TreeItem(Of ClsHelper.CountryTreeItem)) As RadTreeNode
        Dim node As RadTreeNode = New RadTreeNode(root.Item.Name, root.Item.Value)
        Dim expandNode As Boolean = False
        If root.Item.Value.StartsWith("ActionContainer") Then
            If root.Children IsNot Nothing Then
                If root.Children.Where(Function(fc) fc.Item.Checked).Count > 0 AndAlso root.Children.Where(Function(fc) fc.Item.Checked).Count = root.Children.Count Then
                    node.Checked = True
                ElseIf root.Children.Where(Function(fc) fc.Item.Checked).Count > 0 Then
                    expandNode = True
                End If
            End If
        Else
            node.Checked = root.Item.Checked
        End If
        node.Enabled = root.Item.Enabled
        If Not root.Item.Enabled And (node.Checked Or expandNode) Then
            node.Expanded = True
        End If
        node.ImageUrl = root.Item.imageUrl
        For Each field As ClsHelper.TreeItem(Of ClsHelper.CountryTreeItem) In root.Children
            node.Nodes.Add(AddTreeItem(field))
        Next
        Return node
    End Function

    Public Shared Sub FillUserCountriesTree(treeCountries As RadTreeView, Optional checkedByDefault As Boolean = True, Optional checkedList As String() = Nothing)

        Dim fields As List(Of ClsHelper.CountryTreeItem) = New List(Of ClsHelper.CountryTreeItem)
        Dim dataTable As DataTable
        If HttpRuntime.Cache(CacheCountriesKey) Is Nothing Then
            dataTable = ClsDataAccessHelper.FillDataTable("Administration.GetCountriesTree")
            If dataTable IsNot Nothing Then
                HttpRuntime.Cache(CacheCountriesKey) = dataTable
            End If
        Else
            dataTable = HttpRuntime.Cache(CacheCountriesKey)
        End If

        If dataTable IsNot Nothing Then
            For Each row As DataRow In dataTable.Rows
                Dim checked As Boolean = checkedByDefault
                If checkedList IsNot Nothing Then
                    If checkedList.Contains(row("Value")) Then
                        checked = True
                    End If
                End If
                fields.Add(New ClsHelper.CountryTreeItem(row("ID"), row("Name"), checked, True, IIf(row("ParentID") Is DBNull.Value, Nothing, row("ParentID")), row("ImageUrl"), row("Value")))
            Next

            If fields.Count > 0 Then
                Dim root = ClsHelper.GenerateTree(fields, Function(fc) fc.ID, Function(fc) fc.ParentID, Nothing)
                For Each item In root
                    treeCountries.Nodes.Add(AddTreeItem(item))
                Next
                If Not checkedByDefault Then
                    treeCountries.Nodes(0).Expanded = True
                End If
            End If
        End If

    End Sub

    Public Shared Sub FillToolsAndActionsTree(treeCountries As RadTreeView, Optional checkedList As String() = Nothing, Optional enabled As Boolean = True)

        Dim fields As List(Of ClsHelper.CountryTreeItem) = New List(Of ClsHelper.CountryTreeItem)
        Dim dataTable As DataTable
        If HttpRuntime.Cache(CacheToolsAndActionsKey) Is Nothing Then
            dataTable = ClsDataAccessHelper.FillDataTable("Administration.GetToolsAndActions")
            If dataTable IsNot Nothing Then
                HttpRuntime.Cache(CacheToolsAndActionsKey) = dataTable
            End If
        Else
            dataTable = HttpRuntime.Cache(CacheToolsAndActionsKey)
        End If

        If dataTable IsNot Nothing Then
            For Each row As DataRow In dataTable.Rows
                Dim checked As Boolean = False
                If checkedList IsNot Nothing Then
                    If checkedList.Contains(row("Value")) Then
                        checked = True
                    End If
                End If
                fields.Add(New ClsHelper.CountryTreeItem(row("ToolID"), row("Name"), checked, enabled, IIf(row("ParentToolID") Is DBNull.Value, Nothing, row("ParentToolID")), row("IconImagePath"), row("Value")))
            Next

            If fields.Count > 0 Then
                Dim root = ClsHelper.GenerateTree(fields, Function(fc) fc.ID, Function(fc) fc.ParentID, Nothing)
                For Each item In root
                    treeCountries.Nodes.Add(AddTreeItem(item))
                Next
            End If
        End If

    End Sub

    Public Shared Function GetGroupsDataSet(Optional ForceRefresh As Boolean = False) As DataSet
        Return ClsDataAccessHelper.FillDataSet("Administration.GetUserGroups")
    End Function

    Public Class FileUploadResult
        Private _length As Long
        Private _bytes As Byte()
        Private _UploadWithSuccess As Boolean
        Private _FileName As String
        Private _ContentType As String

        Public Sub New(contentType As String, fileName As String, uploadWithSuccess As Boolean, length As Long, bytes() As Byte)
            Me.ContentType = contentType
            Me.FileName = fileName
            Me.UploadWithSuccess = uploadWithSuccess
            Me.length = length
            Me.bytes = bytes
        End Sub

        Public Sub New(uploadWithSuccess As Boolean)
            Me.UploadWithSuccess = uploadWithSuccess
        End Sub

        Public Property ContentType As String
            Get
                Return _ContentType
            End Get
            Set
                _ContentType = Value
            End Set
        End Property

        Public Property FileName As String
            Get
                Return _FileName
            End Get
            Set
                _FileName = Value
            End Set
        End Property

        Public Property UploadWithSuccess As Boolean
            Get
                Return _UploadWithSuccess
            End Get
            Set
                _UploadWithSuccess = Value
            End Set
        End Property

        Public Property length As Long
            Get
                Return _length
            End Get
            Set
                _length = Value
            End Set
        End Property

        Public Property bytes As Byte()
            Get
                Return _bytes
            End Get
            Set
                _bytes = Value
            End Set
        End Property
    End Class

    Public Shared Function UploadUserImage(FileUploadControl As FileUpload, U_GlobalID As Guid, UplodadedBy As Guid) As FileUploadResult
        Dim filename As String = Path.GetFileName(FileUploadControl.PostedFile.FileName)
        Dim fileUploadResult As FileUploadResult = New FileUploadResult(False)
        Dim contentType As String = FileUploadControl.PostedFile.ContentType
        Dim cid As Guid = Guid.Empty
        Try
            Using fs As Stream = FileUploadControl.PostedFile.InputStream
                Using br As BinaryReader = New BinaryReader(fs)
                    Dim length As Long = fs.Length
                    Dim bytes As Byte() = br.ReadBytes(length)

                    Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                    parameters.Add(New SqlParameter("@U_GLOBALID", U_GlobalID))
                    parameters.Add(New SqlParameter("@Filename", filename))
                    parameters.Add(New SqlParameter("@ContentType", contentType))
                    parameters.Add(New SqlParameter("@Data", bytes))
                    parameters.Add(New SqlParameter("@FileLength", length))
                    parameters.Add(New SqlParameter("@AddedBy", UplodadedBy))
                    Dim PictureID As Guid = ClsDataAccessHelper.ExecuteScalar("[Administration].[UploadUserPicture]", parameters)
                    fileUploadResult = New FileUploadResult(contentType, filename, True, length, bytes)
                End Using
            End Using
        Catch
        End Try
        Return fileUploadResult
    End Function

    Public Shared Function DeleteUserImage(U_GlobalID As Guid, UplodadedBy As Guid) As Boolean
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@U_GLOBALID", U_GlobalID))
        parameters.Add(New SqlParameter("@UserID", UplodadedBy))
        Return ClsDataAccessHelper.ExecuteNonQuery("[Administration].[DeleteUserPicture]", parameters)
    End Function

    Public Shared Function GetRightsForChironRoleId(ByVal ChironRoleID As Integer) As List(Of String)
        Dim StrList As New List(Of String)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@ChironRoleID", ChironRoleID))
        Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("[Ebusiness].[UsrMgmt_GetRightsForChironRole]", parameters)
        If dataSet.Tables(0).Rows.Count > 0 Then
            For Each DtRow As DataRow In dataSet.Tables(0).Rows
                If DtRow("OS_GLOBALID") IsNot Nothing Then
                    StrList.Add(DtRow("OS_GLOBALID").ToString)
                End If
            Next
            Return StrList
        Else
            Return Nothing
        End If
    End Function


    Public Shared Function GetSetupType(IsChiron As Boolean) As String
        'TODO enrich when ELX and AEG Chiron run side by side
        If IsChiron Then
            Return "AEG"
        End If
        Return "B2B"
    End Function

End Class
