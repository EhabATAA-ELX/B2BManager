<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="CountryRangeCompareWithEdenData.aspx.vb" enableEventValidation="false" Inherits="CountryRangeCompareWithEdenData" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script type="text/javascript">
        function RefreshInfo() {
            __doPostBack('hiddenRefreshInfo', null)
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <asp:UpdatePanel runat="server" ID="UpdatePane1">
        <ContentTemplate>
            <div runat="server" id="divStatus"></div>
            <asp:HiddenField ID="hiddenRefreshInfo" ClientIDMode="Static" runat="server" />
            <asp:Panel runat="server" ID="PanelImportedData" Visible="false">
                <div style="width:98%;text-align:right;margin-top:10px">                    
                <asp:Button runat="server" CssClass="btn green" Text="Export to Excel" ID="btnExportToExcel" OnClick="btnExportToExcel_Click" />
                </div>
                <telerik:RadTabStrip runat="server" ID="RadTabStripFileDetails" MultiPageID="RadMultiPageFileDetails" SelectedIndex="0">
                <Tabs>
                    <telerik:RadTab Text="Product lines found in the file (*)" PageViewID="pageViewProductsInFile" Width="235px"></telerik:RadTab>
                    <telerik:RadTab Text="Products in Eden Project" PageViewID="pageViewProductsInEden" Width="235px"></telerik:RadTab>
                    <telerik:RadTab Text="Product lines found only in the file (*)" PageViewID="pageViewProductsInFileOnly" Width="235px"></telerik:RadTab>
                    <telerik:RadTab Text="Products found only in Eden Project" PageViewID="pageViewProductsInEdenOnly" Width="235px"></telerik:RadTab>                    
                    <telerik:RadTab Text="Products found in both file and Eden Project" PageViewID="pageViewProductsInBothEdenAndFile" Width="275px"></telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
            <telerik:RadMultiPage runat="server" ID="RadMultiPageFileDetails" SelectedIndex="0" CssClass="outerMultiPage">
                <telerik:RadPageView runat="server" ID="pageViewProductsInFile">                    
                    <telerik:RadGrid runat="server" ID="RadGridProductsInFile" CssClass="BasicGrid" EnableViewState="true"  AllowFilteringByColumn="true"  OnNeedDataSource="RadGridProductsInFile_NeedDataSource"  AllowPaging="true" AllowSorting="true" PageSize="20" GroupingEnabled="true">
                        <MasterTableView AutoGenerateColumns="true" TableLayout="Auto">
                        </MasterTableView>
                    </telerik:RadGrid>
                    <span>(*) Product lines might show some duplicated models due to possible occurences in different plants and divisions with different availabilities</span>
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="pageViewProductsInEden">
                    <telerik:RadGrid runat="server" ID="RadGridProductsInEden" CssClass="BasicGrid" AllowPaging="true" OnNeedDataSource="RadGridProductsInEden_NeedDataSource"  AllowFilteringByColumn="true" AllowSorting="true" PageSize="18" GroupingEnabled="true">
                        <MasterTableView AutoGenerateColumns="true" TableLayout="Auto">
                        </MasterTableView>
                    </telerik:RadGrid>
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="pageViewProductsInFileOnly">
                    <telerik:RadGrid runat="server" ID="RadGridProductsInFileOnly" CssClass="BasicGrid" AllowPaging="true" OnNeedDataSource="RadGridProductsInFileOnly_NeedDataSource"  AllowFilteringByColumn="true" AllowSorting="true" PageSize="18" GroupingEnabled="true">
                        <MasterTableView AutoGenerateColumns="true" TableLayout="Auto">
                        </MasterTableView>
                    </telerik:RadGrid>
                    <span>(*) Product lines might show some duplicated models due to possible occurences in different plants and divisions with different availabilities</span>
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="pageViewProductsInEdenOnly">
                    <telerik:RadGrid runat="server" ID="RadGridProductsInEdenOnly" CssClass="BasicGrid" AllowPaging="true" OnNeedDataSource="RadGridProductsInEdenOnly_NeedDataSource" AllowFilteringByColumn="true" AllowSorting="true" PageSize="18" GroupingEnabled="true">
                        <MasterTableView AutoGenerateColumns="true" TableLayout="Auto">
                        </MasterTableView>
                    </telerik:RadGrid>
                </telerik:RadPageView>
                 <telerik:RadPageView runat="server" ID="pageViewProductsInBothEdenAndFile">
                    <telerik:RadGrid runat="server" ID="RadGridProductsInBothEdenAndFile" CssClass="BasicGrid" AllowPaging="true" OnNeedDataSource="RadGridProductsInBothEdenAndFile_NeedDataSource"  AllowFilteringByColumn="true" AllowSorting="true" PageSize="18" GroupingEnabled="true">
                        <MasterTableView AutoGenerateColumns="true" TableLayout="Auto">
                        </MasterTableView>
                    </telerik:RadGrid>
                </telerik:RadPageView>
            </telerik:RadMultiPage>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="hiddenRefreshInfo" />
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>


