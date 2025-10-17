<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AutomatedTestCaseControl.ascx.vb" Inherits="UserControls_AutomatedTestCaseControl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <asp:HiddenField runat="server" ClientIDMode="Static" ID="UC_hdfTestCaseID" />
        <asp:HiddenField runat="server" ClientIDMode="Static" ID="OperationHdField" />
        <div style="padding-left: 5px">
            <input type="button" runat="server" id="btnNewStep" class="btn bleu" value="New Step" />
            <input type="button" runat="server" id="btnLoadStepsFromFile"  class="btn lightblue" value="Load From File" />
            <input type="button" runat="server" id="btnEditStep" class="btn bleu EditStep" value="Edit Step" />
            <input type="button" runat="server" id="btnMoveUpStep" class="btn lightgreen MoveUp" value="Move Up" />
            <input type="button" runat="server" id="btnMoveDownStep" class="btn lightgreen MoveDown" value="Move Down" />
            <input type="button" runat="server" id="btnDeleteStep" class="btn red DeleteStep" value="Delete Step" />
            <input type="button" runat="server" id="btnRunAllSteps" class="btn lightblue" value="Run All Steps" />            
        </div>
        <div class="testsPanelContent">
            <asp:Table runat="server" ID="tableSteps" ClientIDMode="Static" CssClass="width100percent tableSteps" CellPadding="0" CellSpacing="0">
                <asp:TableHeaderRow VerticalAlign="Middle">
                    <asp:TableHeaderCell Width="30"></asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="30" CssClass="padding-low">Index</asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="150">Command</asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="400">Target</asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="250">Value</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Description</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
        </div>
        <telerik:RadContextMenu runat="server" ID="contextMenuSteps" OnClientShowing="showContextMenu" OnClientItemClicked="OnClientStepItemClicked" OnClientItemClicking="OnClientItemClickingHandler">
            <Targets>
                <telerik:ContextMenuTagNameTarget TagName="td" />
            </Targets>
            <Items>
                <telerik:RadMenuItem Text="Edit step" Value="Edit" ImageUrl="~/Images/edit.png" runat="server" />
                <telerik:RadMenuItem Text="Move step" Value="Move" runat="server">
                    <Items>
                        <telerik:RadMenuItem Text="Move Up" Value="Up" runat="server"></telerik:RadMenuItem>
                        <telerik:RadMenuItem Text="Move Down" Value="Down" runat="server"></telerik:RadMenuItem>
                    </Items>
                </telerik:RadMenuItem>
                <telerik:RadMenuItem Text="Add new step" Value="Add" ImageUrl="~/Images/AutomatedTests/add.png" runat="server">
                    <Items>
                        <telerik:RadMenuItem Text="Before" Value="Before" runat="server"></telerik:RadMenuItem>
                        <telerik:RadMenuItem Text="After" Value="After" runat="server"></telerik:RadMenuItem>
                    </Items>
                </telerik:RadMenuItem>
                 <telerik:RadMenuItem IsSeparator="true" />
                <telerik:RadMenuItem Text="Delete step" Value="Delete" ImageUrl="~/Images/delete.png" runat="server" />
            </Items>
        </telerik:RadContextMenu>

    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="UC_hdfTestCaseID" />
        <asp:AsyncPostBackTrigger ControlID="OperationHdField" />
    </Triggers>
</asp:UpdatePanel>

