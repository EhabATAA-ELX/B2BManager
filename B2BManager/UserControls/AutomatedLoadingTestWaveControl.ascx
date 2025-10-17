<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AutomatedLoadingTestWaveControl.ascx.vb" Inherits="UserControls_AutomatedLoadingTestWaveControl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <asp:HiddenField runat="server" ClientIDMode="Static" ID="UC_hdfTestCaseID" />
        <div style="padding-left: 5px">
            <input type="button" runat="server" id="btnNewRequest" class="btn bleu" value="New Request" />
            <span runat="server" id="btnRequestsGenerator"><input type="button" class="btn lightblue" value="Generate Request(s)" onclick="OpenGenerateRequestsWindow()"/></span>
            <input type="button" runat="server" id="btnEditRequest" class="btn bleu EditStep" value="Edit Request" />
            <input type="button" runat="server" id="btnMoveUpRequest" class="btn lightgreen MoveUp" value="Move Up" />
            <input type="button" runat="server" id="btnMoveDownRequest" class="btn lightgreen MoveDown" value="Move Down" />
            <input type="button" runat="server" id="btnDeleteRequest" class="btn red DeleteStep" value="Delete Request" />
            <input type="button" runat="server" id="btnRunInsequence" class="btn lightblue" value="Run All in Sequence" />
            <input type="button" runat="server" id="btnRunInParallel" class="btn lightblue" value="Run All in Parallel" />
        </div>
        <div class="testsPanelContent">
            <asp:Table runat="server" ID="tableSteps" ClientIDMode="Static" CssClass="width100percent tableSteps" CellPadding="0" CellSpacing="0">
                <asp:TableHeaderRow VerticalAlign="Middle">
                    <asp:TableHeaderCell Width="30"></asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="30" CssClass="padding-low">Index</asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="80" CssClass="padding-low">View XML</asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="150">Country</asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="150">Environment</asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="150">Msg Type</asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="150">Customer Code</asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="150" CssClass="padding-low">Total items/products</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Description</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
        </div>
        <telerik:RadContextMenu runat="server" ID="contextMenuSteps" OnClientShowing="showContextMenu" OnClientItemClicked="OnClientStepItemClicked" OnClientItemClicking="OnClientItemClickingHandler">
            <Targets>
                <telerik:ContextMenuTagNameTarget TagName="td" />
            </Targets>
            <Items>
                <telerik:RadMenuItem Text="Edit request" Value="Edit" ImageUrl="~/Images/edit.png" runat="server" />
                <telerik:RadMenuItem Text="Move request" Value="Move" runat="server">
                    <Items>
                        <telerik:RadMenuItem Text="Move Up" Value="Up" runat="server"></telerik:RadMenuItem>
                        <telerik:RadMenuItem Text="Move Down" Value="Down" runat="server"></telerik:RadMenuItem>
                    </Items>
                </telerik:RadMenuItem>
                <telerik:RadMenuItem Text="Add new request" Value="Add" ImageUrl="~/Images/AutomatedTests/add.png" runat="server">
                    <Items>
                        <telerik:RadMenuItem Text="Before" Value="Before" runat="server"></telerik:RadMenuItem>
                        <telerik:RadMenuItem Text="After" Value="After" runat="server"></telerik:RadMenuItem>
                    </Items>
                </telerik:RadMenuItem>
                <telerik:RadMenuItem IsSeparator="true" />
                <telerik:RadMenuItem Text="Delete request" Value="Delete" ImageUrl="~/Images/delete.png" runat="server" />
            </Items>
        </telerik:RadContextMenu>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="UC_hdfTestCaseID" />
    </Triggers>
</asp:UpdatePanel>