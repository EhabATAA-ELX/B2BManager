<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="CountryRangeCheck.aspx.vb" Inherits="CountryRangeCheck" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <style type="text/css">
        #tvFiltesToProcess {
            height: 800px;
            width: 800px;
            overflow-x: hidden;
            overflow-y: auto;
        }
    </style>
    <script type="text/javascript">
        function ViewFileInBrowser() {
            if ($("#ContentPlaceHolder1_selectedFileID").val().length > 0) {
                popup('GetCountryRangeFile.ashx?fileID=' + $("#ContentPlaceHolder1_selectedFileID").val());
            }
        }
        function CompareFileWithCurrentEdenData() {
            if ($("#ContentPlaceHolder1_selectedFileID").val().length > 0) {
                popup('CountryRangeCompareWithEdenData.aspx?fileID=' + $("#ContentPlaceHolder1_selectedFileID").val() + "&environmentID=" + $("#ContentPlaceHolder1_ddlEnvironment").val());
            }
        }

        function BeginRefreshFiles() {
            $("#ContentPlaceHolder1_btnRefreshFiles").addClass("loadingBackground").val("Refreshing...").prop('disabled', true);
        }

        function BeginResetCache() {
            $("#ContentPlaceHolder1_btnResetFileCache").addClass("loadingBackground").val("Resetting Files Cache...").prop('disabled', true);
        }
        function tvFiltersNodeClicked() {
            $("#ContentPlaceHolder1_loadingSpan").html("<img src='Images/Loading.gif' height='22px' width='22px' />&nbsp;Loading data...");
            $("#ContentPlaceHolder1_pnlFileInfo").hide();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>

            <table class="Filters" >
                <tr>
                    <td class="width120px">
                        <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
                    </td>
                    <td class="width180px">
                        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" OnSelectedIndexChanged="ddlEnvironment_SelectedIndexChanged" AutoPostBack="true" DataTextField="Environment" DataValueField="ID" ID="ddlEnvironment">
                        </asp:DropDownList>
                    </td>
                    
                </tr>
            </table>

            <telerik:RadTabStrip runat="server" ID="RadTabStrip1" MultiPageID="RadMultiPage1" SelectedIndex="1">
                <Tabs>
                    <telerik:RadTab Text="Configurations" PageViewID="pageViewCountryRangeData" Width="200px"></telerik:RadTab>
                    <telerik:RadTab Text="Files" PageViewID="pageViewCountryRangeFiles" Width="200px"></telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>

            <telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="1" CssClass="outerMultiPage">
                <telerik:RadPageView runat="server" ID="pageViewCountryRangeData">
                    <telerik:RadGrid runat="server" ID="gridCountryRangeData" CssClass="LogGridSearch" AllowPaging="true" AllowSorting="true" OnNeedDataSource="gridCountryRangeData_NeedDataSource" AllowFilteringByColumn="true" PageSize="25" GroupingEnabled="true">
                        <MasterTableView AutoGenerateColumns="false" TableLayout="Auto">
                            <Columns>
                                <telerik:GridTemplateColumn DataField="FileImportStatus" AllowFiltering="false" UniqueName="FileImportStatus" HeaderText="" HeaderStyle-Width="28">
                                    <ItemTemplate>
                                        <img src='Images/<%# IIf(Eval("FileImportStatus").ToString() = "OK", "Success", "Error")%>.png' width="18" height="18" title="<%# Eval("FileImportStatus") %>" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="SOP_ID" AllowFiltering="true" UniqueName="SOP_ID" HeaderText="SOP" HeaderStyle-Width="60">
                                    <ItemTemplate>
                                        <img src='<%# Eval("ImageUrl").ToString() %>' width="20" height="16" title="<%# Eval("Name") %>" />
                                        <span class="verticalAlignTop"><%# Eval("SOPNAME") %></span>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="ManageLocalEdenFlag" UniqueName="ManageLocalEdenFlag" HeaderText="Manage Local Eden Flag"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="CountryRangeImportPriority" UniqueName="CountryRangeImportPriority" HeaderText="Priority"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="CountryRangeImportRecentFileAfterInMinutes" UniqueName="CountryRangeImportRecentFileAfterInMinutes" HeaderText="Import Recent File After (in minutes)"></telerik:GridBoundColumn>
                                <telerik:GridCheckBoxColumn DataField="CountryRangeImportOneFilePerDay" UniqueName="CountryRangeImportOneFilePerDay" HeaderText="Import One File Per Day?"></telerik:GridCheckBoxColumn>
                                <telerik:GridTemplateColumn DataField="isActive" AllowFiltering="false" UniqueName="isActive" HeaderText="Active?" HeaderStyle-Width="30">
                                    <ItemTemplate>
                                        <img src='<%# "Images/" + IIf(Eval("isActive").ToString() = "1", "active", IIf(Eval("isActive").ToString() = "0", "inactive", "partial")) + ".png" %>' width="20" height="20" title="<%# Eval("isActive") %>" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="InputFileName" UniqueName="InputFileName" HeaderText="Latest File Imported"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="FileImportStartDate" UniqueName="FileImportStartDate" HeaderText="Latest File Imported Start Date"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="FileImportEndDate" UniqueName="FileImportEndDate" HeaderText="Latest File Imported End Date"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="pushstock_status" UniqueName="pushstock_status" HeaderText="Latest File Push Stock Status"></telerik:GridBoundColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="pageViewCountryRangeFiles">
                    <table>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td class="width120px">
                                            <asp:Label runat="server" ID="lblCountry" Width="80" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
                                        </td>
                                        <td class="width180px">
                                            <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color" Width="210" AppendDataBoundItems="true" ID="ddlCountry">
                                                <Items>
                                                    <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
                                                </Items>
                                            </telerik:RadComboBox>
                                        </td>
                                        <td>
                                            <asp:Label runat="server" ID="lblShowFilesInTheLastNDays" Text="within the last" CssClass="Electrolux_light_bold Electrolux_Color"></asp:Label>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox runat="server" MinValue="1" Value="1" Width="50" ShowSpinButtons="true" DataType="Integer" MaxValue="360" ID="txtDaysCount" CssClass="Electrolux_light_bold Electrolux_Color">
                                                <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <asp:Label runat="server" ID="lblDaysCount" CssClass="Electrolux_light_bold Electrolux_Color">day(s)</asp:Label>
                                        </td>
                                        <td>
                                            <asp:LinkButton runat="server" ID="btnRefreshFiles" CssClass="btn bleu" OnClientClick="BeginRefreshFiles()" OnClick="btnRefreshFiles_Click" Text="Refresh" />
                                        </td>
                                        <td>
                                            <asp:LinkButton runat="server" ID="btnResetFileCache" CssClass="btn danger" OnClientClick="BeginResetCache()" OnClick="btnResetFileCache_Click" Text="Reset Files Cache" />
                                        </td>
                                    </tr>
                                </table>
                                <telerik:RadTreeView runat="server" ID="tvFiltesToProcess" OnNodeClick="tvFiltesToProcess_NodeClick" OnClientNodeClicked="tvFiltersNodeClicked" OnNodeExpand="tvFiltesToProcess_NodeExpand" ClientIDMode="Static">
                                </telerik:RadTreeView>
                            </td>
                            <td valign="top">
                                <h4>Select a file from the right hand list and get more details below:</h4>
                                <span id="loadingSpan" runat="server"></span>
                                <asp:Panel runat="server" ID="pnlFileInfo" Visible="false">
                                    <table>
                                        <tr>
                                            <td>
                                                <span>File name:</span>
                                            </td>
                                            <td>
                                                <asp:HiddenField runat="server" ID="selectedFileID" />
                                                <asp:Label runat="server" ID="lblFileName" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span>File path:</span>
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lblFilePath" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span>Creation date:</span>
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lblCreationDate" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span>Last modification date:</span>
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lblLastModificationDate" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span>Size:</span>
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lblSize" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span>Input file ID:</span>
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lblInputFileID" Font-Bold="true" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr runat="server" id="trIntegratedProducts">
                                            <td>
                                                <span>Total integrated product lines:</span>
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lblTotalProducts" Font-Bold="true" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Button runat="server" Text="Download" ID="btnDownloadFile" OnClick="btnDownloadFile_Click" CssClass="btn green" />
                                                <input type="button" class="btn bleu" value="View File In Browser" onclick="ViewFileInBrowser()" />                                                
                                                <span runat="server" id="pnlCompareWithEdenData">
                                                    <input type="button" class="btn bleu" value="Compare with current Eden data" onclick="CompareFileWithCurrentEdenData()" />
                                                </span>                                                
                                            </td>
                                        </tr>
                                    </table>
                                    <telerik:RadTabStrip runat="server" ID="RadTabStripFileDetails" MultiPageID="RadMultiPageFileDetails" SelectedIndex="1">
                                        <Tabs>
                                            <telerik:RadTab Text="Logs" PageViewID="pageViewLogs" Width="200px"></telerik:RadTab>
                                            <telerik:RadTab Text="Integrated Product Lines (*)" PageViewID="pageViewInputProducts" Width="200px"></telerik:RadTab>
                                        </Tabs>
                                    </telerik:RadTabStrip>
                                    <telerik:RadMultiPage runat="server" ID="RadMultiPageFileDetails" SelectedIndex="1" CssClass="outerMultiPage">
                                        <telerik:RadPageView runat="server" ID="pageViewLogs">
                                            <telerik:RadGrid runat="server" ID="RadGridLogs" CssClass="BasicGrid" AllowPaging="true" AllowSorting="true" OnNeedDataSource="RadGridLogs_NeedDataSource" AllowFilteringByColumn="false" PageSize="20" GroupingEnabled="true">
                                                <MasterTableView AutoGenerateColumns="true" TableLayout="Auto">
                                                </MasterTableView>
                                            </telerik:RadGrid>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView runat="server" ID="pageViewInputProducts">
                                            <span>(*) Product lines might show some duplicated models due to multiple occurences</span>
                                            <telerik:RadGrid runat="server" ID="RadGridInputProducts" CssClass="BasicGrid" AllowPaging="true" AllowSorting="true" OnNeedDataSource="RadGridInputProducts_NeedDataSource" AllowFilteringByColumn="true" PageSize="18" GroupingEnabled="true">
                                                <MasterTableView AutoGenerateColumns="true" TableLayout="Auto">
                                                </MasterTableView>
                                            </telerik:RadGrid>
                                        </telerik:RadPageView>
                                    </telerik:RadMultiPage>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </telerik:RadPageView>
            </telerik:RadMultiPage>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnDownloadFile" />
            <asp:AsyncPostBackTrigger ControlID="btnRefreshFiles" />
            <asp:AsyncPostBackTrigger ControlID="btnResetFileCache" />
            <asp:AsyncPostBackTrigger ControlID="tvFiltesToProcess" />
            <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

