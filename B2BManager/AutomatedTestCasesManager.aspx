<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" CodeFile="AutomatedTestCasesManager.aspx.vb" Inherits="AutomatedTestCasesManager" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="CSS/AutomatedTests.css" rel="stylesheet" />
    <script src="Scripts/AutomatedTests.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div style="width: 100%; padding: 25px">
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <div class="testsPanelContainer test-panel1">
                    <div class="testsPanelContent">
                        <div class="TextAlignCenter">
                            <table algin="center">
                                <tr>
                                    <td><b>Create new:</b></td>
                                    <td>
                                        <input type="button" id="btnAddContainer" class="btn bleu" value="Container" onclick="NewElement('container')" /></td>
                                    <td>
                                        <input type="button" id="btnAddTestCase" class="btn bleu" value="Test Case" onclick="NewElement('testcase')" /></td>
                                    <td>
                                        <input type="button" id="btnLoadTestWave" class="btn bleu" value="Load Test Wave" onclick="NewElement('loadtestwave')" /></td>
                                </tr>
                            </table>
                        </div>
                        <input type="text" placeholder="Search test case" class="test-search-input" id="searchTestCasetxtBox" onkeyup="searchTestCase()" />
                        <telerik:RadAjaxPanel runat="server">
                            <telerik:RadTreeView runat="server" ID="tvTestCases" OnClientContextMenuItemClicked="ClientContextMenuItemClicked" MultipleSelect="false" OnClientNodeClicking="ClientNodeClicking" OnNodeClick="tvTestCases_NodeClick">
                                <ContextMenus>
                                    <telerik:RadTreeViewContextMenu runat="server" ID="ContextMenuTestCase">
                                        <Items>
                                            <telerik:RadMenuItem Text="Edit test case" Value="EditTestCase" ImageUrl="Images/edit.png" runat="server" />
                                            <telerik:RadMenuItem Text="Display" Value="Display" ImageUrl="Images/AutomatedTests/display.png" runat="server">
                                                <Items>
                                                    <telerik:RadMenuItem Text="All steps" Value="AllSteps" ImageUrl="Images/Tools/testCases.png" runat="server"></telerik:RadMenuItem>
                                                    <telerik:RadMenuItem Text="Linked test suites" Value="LinkedTestSuites" Enabled="false" ImageUrl="Images/Tools/testSuites.png" runat="server"></telerik:RadMenuItem>
                                                </Items>
                                            </telerik:RadMenuItem>
                                            <telerik:RadMenuItem Text="Run all steps" Value="RunAllSteps" ImageUrl="Images/AutomatedTests/run.png" runat="server"></telerik:RadMenuItem>
                                            <telerik:RadMenuItem IsSeparator="true" />
                                            <telerik:RadMenuItem Text="Delete test case" Value="DeleteTestCase" ImageUrl="Images/delete.png" runat="server" />
                                        </Items>
                                    </telerik:RadTreeViewContextMenu>
                                    <telerik:RadTreeViewContextMenu runat="server" ID="ContextMenuLoadTestWave">
                                        <Items>
                                            <telerik:RadMenuItem Text="Edit Wave" Value="EditLoadTestWave" ImageUrl="Images/edit.png" runat="server" />
                                            <telerik:RadMenuItem Text="Display" Value="Display" ImageUrl="Images/AutomatedTests/display.png" runat="server">
                                                <Items>
                                                    <telerik:RadMenuItem Text="All requests" Value="AllRequests" ImageUrl="Images/Tools/testCases.png" runat="server"></telerik:RadMenuItem>
                                                    <telerik:RadMenuItem Text="Linked test suites" Value="LinkedTestSuites" Enabled="false" ImageUrl="Images/Tools/testSuites.png" runat="server"></telerik:RadMenuItem>
                                                </Items>
                                            </telerik:RadMenuItem>
                                            <telerik:RadMenuItem IsSeparator="true" />
                                            <telerik:RadMenuItem Text="Delete Wave" Value="DeleteLoadTestWave" ImageUrl="Images/delete.png" runat="server" />
                                        </Items>
                                    </telerik:RadTreeViewContextMenu>
                                    <telerik:RadTreeViewContextMenu runat="server" ID="ContextMenuFolder">
                                        <Items>
                                            <telerik:RadMenuItem Text="Edit container" Value="EditContainer" ImageUrl="Images/edit.png" runat="server" />
                                            <telerik:RadMenuItem Text="Add" Value="AddContainer" ImageUrl="Images/AutomatedTests/add.png" runat="server">
                                                <Items>
                                                    <telerik:RadMenuItem Text="New sub container" Value="NewSubContainer" ImageUrl="Images/AutomatedTests/testFolder.png" runat="server"></telerik:RadMenuItem>
                                                    <telerik:RadMenuItem Text="New test case" Value="NewTestCase" ImageUrl="Images/AutomatedTests/testCase.png" runat="server"></telerik:RadMenuItem>
                                                    <telerik:RadMenuItem Text="New Load Testing Wave" Value="NewLoadingTestWave" ImageUrl="Images/AutomatedTests/loadtestwave.png" runat="server"></telerik:RadMenuItem>
                                                </Items>
                                            </telerik:RadMenuItem>
                                            <telerik:RadMenuItem IsSeparator="true" />
                                            <telerik:RadMenuItem Text="Delete container" Value="DeleteContainer" ImageUrl="Images/delete.png" runat="server" />
                                        </Items>
                                    </telerik:RadTreeViewContextMenu>
                                </ContextMenus>
                            </telerik:RadTreeView>
                        </telerik:RadAjaxPanel>
                    </div>
                </div>
                <div runat="server" id="testsPanelContainer" class="testsPanelContainer test-panel2">
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="tvTestCases" />
            </Triggers>
        </asp:UpdatePanel>
        <telerik:RadWindow ClientIDMode="Static" ID="WindowContainers" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="460px" Height="250px" runat="server">
        </telerik:RadWindow>

        <telerik:RadWindow ClientIDMode="Static" ID="WindowDeleteElement" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="410px" Height="150px" runat="server">
        </telerik:RadWindow>

        <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="windowTestStepsUP">
            <ContentTemplate>
                <telerik:RadWindow ID="WindowTestSteps" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" runat="server">
                </telerik:RadWindow>
            </ContentTemplate>
        </asp:UpdatePanel>

        <telerik:RadWindow ID="WindowDeleteTestStep" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="410px" Height="150px" runat="server">
        </telerik:RadWindow>

        

        <telerik:RadWindow ClientIDMode="Static" ID="GenerateRequestsWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="550px" Height="450px" runat="server">
        </telerik:RadWindow>

    </div>
</asp:Content>

