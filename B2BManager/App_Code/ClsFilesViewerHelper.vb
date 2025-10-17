Imports System.Data
Imports Microsoft.VisualBasic
Imports System.Collections.Generic

Public Class ClsFilesViewerHelper

    Public Shared Function GetFileEntries(Cache As Cache) As List(Of FileEntry)
        If Cache("fileEntries") Is Nothing Then
            Dim fileEntries As List(Of FileEntry) = New List(Of FileEntry)
            Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("FilesViewer.GetFileEntriesAndEnvironments")
            If dataSet.Tables.Count = 2 Then
                For Each entryRow As DataRow In dataSet.Tables(0).Rows
                    Dim fileEntry As FileEntry = New FileEntry(entryRow("FileEntryID"), entryRow("FileEntryName"), entryRow("ExtensionFilter"), entryRow("AllDirectories"), entryRow("AdministrationActionID"))
                    For Each environmentRow As DataRow In dataSet.Tables(1).Select("FileEntryID=" & fileEntry.ID.ToString())
                        If fileEntry.Environments Is Nothing Then
                            fileEntry.Environments = New List(Of FileEnvironment)
                        End If
                        fileEntry.Environments.Add(New FileEnvironment(environmentRow("EnvironmentID"), environmentRow("EnvironmentName"), environmentRow("RootPath")))
                    Next
                    fileEntries.Add(fileEntry)
                Next
            End If
            Cache("fileEntries") = fileEntries
        End If
        Return Cache("fileEntries")
    End Function

    Public Shared Function GetEnvironment(envid As Integer, entryid As Integer, ByRef entry As FileEntry) As FileEnvironment
        Dim listofEntries As List(Of FileEntry) = GetFileEntries(HttpContext.Current.Cache)
        entry = listofEntries.Where(Function(Fc) Fc.ID = entryid).SingleOrDefault()
        Dim Environment As FileEnvironment = Nothing
        If entry IsNot Nothing Then
            Environment = entry.Environments.Where(Function(fc) fc.ID = envid).SingleOrDefault()
        End If
        Return Environment
    End Function

    Public Shared Function GetDownloadTemplate() As String
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser IsNot Nothing Then
            If clsUser.Actions.Contains(ClsHelper.ActionDesignation.DOWNLOAD_FILES) Then
                Return "<img src=\""Images\\CircularTools\\download.png\"" Class=\""MoreInfoImg\""  height=\""24px\"" title='Download' onclick=\""DownloadFile('"" + data.replaceAll('\\', '@') + ""')\"" \/>"
            Else
                Return ""
            End If
        Else
            Return ""
        End If
    End Function


    Public Class FileEnvironment
        Inherits ClsHelper.BasicModel

        Private _rootPath As String

        Public Property RootPath As System.String
            Get
                Return _rootPath
            End Get
            Set(value As System.String)
                _rootPath = value
            End Set
        End Property

        Public Sub New(environmentID As Integer, name As String, rootPath As String)
            MyBase.New(environmentID, name, True, True)
            Me._rootPath = rootPath
        End Sub
    End Class

    Public Class FileEntry
        Inherits ClsHelper.BasicModel

        Private _extensionFilter As String
        Private _allDirectories As Boolean
        Private _administrationActionID As Integer
        Public Environments As List(Of FileEnvironment)

        Public Sub New(fileEntryID As Integer, fileEntryName As String, extensionFilter As String, allDirectories As Boolean, administrationActionID As Integer)
            MyBase.New(fileEntryID, fileEntryName, True, True)
            _extensionFilter = extensionFilter
            _allDirectories = allDirectories
            _administrationActionID = administrationActionID
        End Sub

        Public Property ExtensionFilter As System.String
            Get
                Return _extensionFilter
            End Get
            Set(value As System.String)
                _extensionFilter = value
            End Set
        End Property

        Public Property AllDirectories As System.Boolean
            Get
                Return _allDirectories
            End Get
            Set(value As System.Boolean)
                _allDirectories = value
            End Set
        End Property

        Public Property AdministrationActionID As System.Int32
            Get
                Return _administrationActionID
            End Get
            Set(value As System.Int32)
                _administrationActionID = value
            End Set
        End Property

    End Class



End Class
