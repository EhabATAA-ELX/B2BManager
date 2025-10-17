
Imports System.Data
Imports System.Diagnostics
Imports Telerik.Web.UI

Partial Class AutomatedTestCasesManager
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

        Dim __EVENTTARGET As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")

        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx?ReturnURL=AutomatedTestCasesManager.aspx", True)
        End If

        If Not IsPostBack Then
            FillTestCasesPanel()
        Else
            If __EVENTTARGET IsNot Nothing Then
                If __EVENTTARGET.StartsWith("UC_") Then
                    If Not String.IsNullOrEmpty(__EVENTARGUMENT) Then
                        Dim arguments As String() = __EVENTARGUMENT.Split("|")
                        FillTestCasesPanel(arguments(0))
                    End If
                End If
            End If
        End If

    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim __EVENTTARGET As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")

        If IsPostBack Then
            If __EVENTTARGET IsNot Nothing Then
                If __EVENTTARGET.Equals(tvTestCases.ClientID) Then
                    If __EVENTARGUMENT IsNot Nothing AndAlso __EVENTARGUMENT.StartsWith("Refresh") Then
                        Dim selectedTestCaseID As Integer
                        Integer.TryParse(tvTestCases.SelectedValue, selectedTestCaseID)
                        FillTestCasesPanel(selectedTestCaseID)
                    End If
                Else
                    If __EVENTARGUMENT IsNot Nothing AndAlso __EVENTARGUMENT.StartsWith("Move") Then
                        Dim selectedTestCaseID As Integer
                        Integer.TryParse(tvTestCases.SelectedValue, selectedTestCaseID)
                        If __EVENTARGUMENT.StartsWith("MoveStep") Then
                            FillTestCaseSteps(selectedTestCaseID, ClsAutomatedTestsHelper.TestItemType.TestCase)
                        ElseIf __EVENTARGUMENT.StartsWith("MoveRequest") Then
                            FillTestCaseSteps(selectedTestCaseID, ClsAutomatedTestsHelper.TestItemType.LoadingTestWave)
                        End If

                    End If
                End If
            End If
        End If

    End Sub

    Private Sub FillTestCasesPanel(Optional selectedTestCaseID As Integer = 0)
        Dim selectedItemType As ClsAutomatedTestsHelper.TestItemType = ClsAutomatedTestsHelper.TestItemType.TestCase
        Dim nodeItems = ClsAutomatedTestsHelper.GetTestCases()
        If nodeItems.Count > 0 Then
            If selectedTestCaseID = 0 Or nodeItems.Where(Function(fc) fc.ID = selectedTestCaseID).FirstOrDefault() Is Nothing Then
                Dim testCase As ClsAutomatedTestsHelper.TestCase = nodeItems.Where(Function(fc) fc.IsFolder = False).FirstOrDefault()
                If testCase IsNot Nothing Then
                    selectedTestCaseID = testCase.ID
                    selectedItemType = testCase.ItemType
                End If
            End If
            tvTestCases.Nodes.Clear()
            Dim nodes As List(Of ClsHelper.TreeItem(Of ClsAutomatedTestsHelper.TestCase)) = ClsHelper.GenerateTree(nodeItems, Function(fc) fc.ID, Function(fc) fc.ParentID, Nothing).ToList()
            For Each nodeItem As ClsHelper.TreeItem(Of ClsAutomatedTestsHelper.TestCase) In nodes
                tvTestCases.Nodes.Add(GetTreeItemNode(nodeItem, selectedTestCaseID, selectedItemType))
            Next
        End If
        FillTestCaseSteps(selectedTestCaseID, selectedItemType)
    End Sub

    Private Sub FillTestCaseSteps(selectedTestCaseID As Integer, selectedItemType As ClsAutomatedTestsHelper.TestItemType)
        Dim userControl As ClsTestCaseUserControl = Nothing
        Select Case selectedItemType
            Case ClsAutomatedTestsHelper.TestItemType.TestCase
                WindowTestSteps.Width = 500
                WindowTestSteps.Height = 380
                windowTestStepsUP.Update()
                userControl = LoadControl("~/UserControls/AutomatedTestCaseControl.ascx")
            Case ClsAutomatedTestsHelper.TestItemType.LoadingTestWave
                WindowTestSteps.Width = 850
                WindowTestSteps.Height = 650
                windowTestStepsUP.Update()
                userControl = LoadControl("~/UserControls/AutomatedLoadingTestWaveControl.ascx")
        End Select
        If userControl IsNot Nothing Then
            userControl.TestCaseID = selectedTestCaseID
            testsPanelContainer.Controls.Clear()
            testsPanelContainer.Controls.Add(userControl)
        End If
    End Sub

    Private Function GetTreeItemNode(nodeItem As ClsHelper.TreeItem(Of ClsAutomatedTestsHelper.TestCase), selectedValue As Integer, ByRef selectedItemType As ClsAutomatedTestsHelper.TestItemType) As RadTreeNode
        Dim nodeTreeItem As RadTreeNode = New RadTreeNode(nodeItem.Item.Name, nodeItem.Item.ID)
        nodeTreeItem.ImageUrl = nodeItem.Item.ImagePath
        nodeTreeItem.ToolTip = nodeItem.Item.Tooltip
        Select Case nodeItem.Item.ItemType
            Case ClsAutomatedTestsHelper.TestItemType.Folder
                nodeTreeItem.ContextMenuID = "ContextMenuFolder"
            Case ClsAutomatedTestsHelper.TestItemType.TestCase
                nodeTreeItem.ContextMenuID = "ContextMenuTestCase"
            Case ClsAutomatedTestsHelper.TestItemType.LoadingTestWave
                nodeTreeItem.ContextMenuID = "ContextMenuLoadTestWave"
        End Select
        nodeTreeItem.EnableContextMenu = True
        nodeTreeItem.Attributes.Add("nodeType", nodeItem.Item.ItemType.ToString())
        nodeTreeItem.Expanded = IIf(nodeItem.Item.IsFolder, True, False)
        If nodeTreeItem.Value = selectedValue Then
            nodeTreeItem.Selected = True
            selectedItemType = nodeItem.Item.ItemType
        End If
        If nodeItem.Children.Count > 0 Then
            For Each childItem As ClsHelper.TreeItem(Of ClsAutomatedTestsHelper.TestCase) In nodeItem.Children
                nodeTreeItem.Nodes.Add(GetTreeItemNode(childItem, selectedValue, selectedItemType))
            Next
        End If
        Return nodeTreeItem
    End Function


    Protected Sub tvTestCases_NodeClick(sender As Object, e As RadTreeNodeEventArgs)
        FillTestCasesPanel(e.Node.Value)
    End Sub
End Class
