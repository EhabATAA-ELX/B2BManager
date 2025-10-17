<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="FilesManager.aspx.vb" Inherits="FilesManager" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc1" TagName="FilesManagerManageSecurity" Src="~/UserControls/FilesManagerManageSecurity.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script src="Scripts/filesManager/filesManagerScripts.js?=2025.09.08" type="text/javascript"></script>
    <script src="Scripts/LogManager.js" type="text/javascript"></script>
    <link href="Scripts/filesManager/filesManagerStyle.css" rel="stylesheet" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <link rel="stylesheet" href="https://cdn.datatables.net/1.13.4/css/jquery.dataTables.min.css" />
    <script src="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <style type="text/css">
        .rgSelectedRow {
            background-color: transparent !important;
            background-image: none !important;
            color: inherit !important;
        }

        .select-header input[type="checkbox"] {
            display: none;
        }

        .select-item input[type="checkbox"] {
            position: relative;
            top: 5px;
        }

        .staticAssignmentWindow {
            z-index: 3005 !important;
        }

        .InDirectAssignmentWindow {
            z-index: 3005 !important;
        }

        .contentPreview {
            padding: 30px !important;
        }
    </style>
    <script type="text/javascript">

        function RowDropped(sender, args) {
            var grid = $find('<%= FileManagementRadGrid.ClientID %>');
            var gridTable = grid.get_element();

            var rectY = gridTable.getBoundingClientRect().top;
            var rows = gridTable.querySelectorAll('.rgRow,.rgAltRow');
            var mouseY = args.get_domEvent().clientY - rectY;
            var indexTarget = -1;
            for (var i = 0; i < rows.length; i++) {
                var row = rows[i];
                var rowTop = row.offsetTop;
                var rowHeight = row.offsetHeight;
                if (mouseY <= rowTop + rowHeight / 2) {
                    indexTarget = i;
                    break;
                }
            }
            if (indexTarget == -1) {
                indexTarget = rows.length;
            }

            var dataItems = grid.get_masterTableView().get_dataItems();
            var row = dataItems[indexTarget];
            var cellValueTarget;
            var position
            if (row) {
                cellValueTarget = row.get_element().getElementsByClassName('ID')[0].textContent;
                position = true;
            } else {
                row = dataItems[rows.length - 1];
                cellValueTarget = row.get_element().getElementsByClassName('ID')[0].textContent;
                position = false;
            }

            var envId = $find('<%= EnvironmentRadComboBox.ClientID %>').get_selectedItem().get_value();
            var cntryId = document.getElementById("<%= CountryGuid.ClientID %>").value;
            var docIds = [];
            for (var i = 0; i < args.get_draggedItems().length; i++) {
                docIds.push(args.get_draggedItems()[i].get_element().getElementsByClassName('ID')[0].textContent)
            }
            var data = {
                DocumentIdsSource: docIds,
                DocumentIdTarget: cellValueTarget,
                IsBefore: position,
                EnvironmentId: envId,
                CountryId: cntryId
            };
            $.ajax({
                type: 'POST',
                url: 'FilesManager.aspx/RowDropped',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify(data),
                success: function () {
                    __doPostBack('FileManagementRadTreeViewNodeClick', '');
                }
            });
        }

        function RowSelected() {
            var selectedRows = $find("<%= FileManagementRadGrid.ClientID %>").get_masterTableView().get_selectedItems();
            var superUpBtn = document.getElementById('<%= TopBtn.ClientID %>');
            var upBtn = document.getElementById('<%= UpBtn.ClientID %>');
            var downBtn = document.getElementById('<%= DownBtn.ClientID %>');
            var superDownBtn = document.getElementById('<%= BottomBtn.ClientID %>');
            if (selectedRows.length > 0) {
                superUpBtn.style.display = 'inline';
                upBtn.style.display = 'inline';
                downBtn.style.display = 'inline';
                superDownBtn.style.display = 'inline';
            }
            else {
                superUpBtn.style.display = 'none';
                upBtn.style.display = 'none';
                downBtn.style.display = 'none';
                superDownBtn.style.display = 'none';
            }
        }

        /* -----------Start Assignment Direc/InDirect Condition ----------- */
        /*********************************************************************/

        function openStaticAssignmentPopup(url) {
            var oWnd = $find("<%= StaticAssignmentWindow.ClientID %>");
            if (oWnd) {
                oWnd.setUrl(url);
                oWnd.set_title("Edit Static Assignment");
                oWnd.show();
            }
        }

        function openInDirectAssignmentPopup(url) {
            var oWnd = $find("<%= InDirectAssignmentWindow.ClientID %>");
            if (oWnd) {
                oWnd.setUrl(url);
                oWnd.set_title("Edit InDirect Assignment");
                oWnd.show();
            }
        }

        function onStaticWindowBeforeClose(sender, args) {
            var contentWindow = sender.get_contentFrame().contentWindow;

            if (typeof contentWindow.confirmAndClose === "function") {
                var canClose = contentWindow.confirmAndClose();
                // If the answer was false, cancel the original closing event.
                if (!canClose) {
                    args.set_cancel(true);
                }
            }
        }

        function closeActiveWindow() {
            var oWnd = $find("<%= StaticAssignmentWindow.ClientID %>");
            oWnd.close();
        }

        function handleStaticSaveComplete() {
            setTimeout(function () {
                var gridId = "<%= Me.FilesManagerManageSecurity.FindControl("ConditionRg").ClientID %>";
                var grid = $find(gridId);
                if (grid) {
                    grid.get_masterTableView().rebind();
                } else {
                    console.error("CRITICAL: Failed to find the RadGrid component. The AJAX update may not be configured correctly to include this control.");
                }
            }, 100);// A zero-millisecond timeout is standard for this fix.
        }

        function handleInDirectSaveComplete() {
            setTimeout(function () {
                var gridId = "<%= Me.FilesManagerManageSecurity.FindControl("ConditionRg").ClientID %>";
                var grid = $find(gridId);
                if (grid) {
                    grid.get_masterTableView().rebind();
                } else {
                    console.error("CRITICAL: Failed to find the RadGrid component. The AJAX update may not be configured correctly to include this control.");
                }
            }, 100);// A zero-millisecond timeout is standard for this fix.
        }

        /*Start Preview all customers*/

        function loadAndShowPreview(idInput) {
            if (!idInput) {
                alert('Could not find the required ID in the URL.');
                return;
            }
            // Show the window
            var radWindow = $find("<%= RadWindowPreview.ClientID %>");
            radWindow.show();

            // Optional: Show a loading message
            $('#previewTable tbody').html('<tr><td colspan="2"><em>Loading...</em></td></tr>');
            $('#customerCountPreview').text('...');

            //ToDO GetPreviewCustomers in external file , userControl ? 
            // Make the AJAX call to your WebMethod
            $.ajax({
                type: "POST",
                url: "FocusRangeManagement.aspx/GetPreviewCustomers",
                data: JSON.stringify({ id: idInput, pageSource: "FilesManager" }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    // The data is directly in response.d, no parsing needed
                    var customers = response.d;
                    var tableBody = $('#previewTable tbody');

                    // Destroy the old DataTable instance if it exists
                    if ($.fn.DataTable.isDataTable('#previewTable')) {
                        $('#previewTable').DataTable().destroy();
                    }

                    // Clear the table and update the count
                    tableBody.empty();
                    $('#customerCountPreview').text(customers.length);

                    // Populate the table with new data
                    if (customers.length > 0) {
                        $.each(customers, function (index, customer) {
                            tableBody.append('<tr><td>' + customer.C_CUID + '</td><td>' + customer.C_NAME + '</td></tr>');
                        });
                    }

                    // Re-initialize DataTable with paging, searching, etc.
                    $('#previewTable').DataTable({
                        "pageLength": 25,
                        "searching": true,
                        "paging": true,
                        "destroy": true,
                        "info": true
                    });
                },
                error: function (xhr, status, error) {
                    $('#previewTable tbody').html('<tr><td colspan="2" style="color:red;">An error occurred while fetching data.</td></tr>');
                }
            });
        }

        /*End Preview all customers*/



        /* -----------End Assignment Direct/InDirect Condition ----------- */
        /*********************************************************************/
    </script>

    <asp:UpdateProgress ID="updProgress" AssociatedUpdatePanelID="UpdatePanel1" runat="server">
        <ProgressTemplate>
            <img alt="progress" style='width: 100%; height: 800px; margin-top: -50px' class='loadingBackgroundDefault' />
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>

            <asp:HiddenField runat="server" ID="CountryGuid" />

            <table class="Filters">
                <tr>
                    <td>
                        <telerik:RadComboBox RenderMode="Lightweight" ID="EnvironmentRadComboBox" runat="server" Width="305"
                            EmptyMessage="Choose an environment" HighlightTemplatedItems="true" Label="Environment: " />
                    </td>
                    <td>
                        <telerik:RadComboBox RenderMode="Lightweight" ID="CountryRadComboBox" runat="server" Height="200" Width="305"
                            EmptyMessage="Choose a country" HighlightTemplatedItems="true" Label="Country: " />
                    </td>
                    <td>
                        <asp:Button type="button" ID="OKbtn" class="btn bleu rounded" OnClick="OKbtnClick" Text="OK" runat="server" />
                        <telerik:RadTextBox RenderMode="Lightweight" runat="server" ID="LangIsocideBox" Enabled="false" Visible="false" />
                        <telerik:RadTextBox RenderMode="Lightweight" runat="server" ID="CyGlobalId" Enabled="false" Visible="false" />
                    </td>
                </tr>
            </table>

            <telerik:RadAjaxManager runat="server" ID="FilesManagerRadAjaxManager">
                <ClientEvents OnRequestStart="FMRequestStart" OnResponseEnd="FMResponseEnd" />
            </telerik:RadAjaxManager>

            <asp:Panel runat="server" ID="FileManagerPanel" Style="display: none;">

                <div>
                    <telerik:RadTabStrip RenderMode="Lightweight" runat="server" ID="SecureUnsecureTabStrip" OnTabClick="SecureUnsecureTabStrip_TabClick" MultiPageID="RadMultiPage1" SelectedIndex="0">
                        <Tabs>
                            <telerik:RadTab Text="Customer Specific" PageViewID="SecuredFiles" Width="200px" />
                            <telerik:RadTab Text="Generic Material" PageViewID="UnsecuredFiles" Width="200px" />
                        </Tabs>
                    </telerik:RadTabStrip>

                    <asp:Panel runat="server" ID="FileManagerFilersPanel">
                        <asp:Button type="button" ID="AddFolderbtn" class="btn bleu rounded" Text="Add folder" runat="server" Visible="false" OnClick="AddFolderbtn_Click" />
                        <asp:Button type="button" ID="AddRootFolderbtn" class="btn bleu rounded" Text="Add root folder" runat="server" Visible="false" OnClick="AddRootFolderbtn_Click" />
                        <asp:Button type="button" ID="RenameFolderbtn" class="btn bleu rounded" Text="Rename folder" runat="server" Visible="false" OnClientClick="ReNameFolder(); return false;" />
                        <asp:Button type="button" ID="DeleteFolderbtn" class="btn bleu rounded" Text="Delete folder" runat="server" Visible="false" OnClientClick="return DeleteFolder();" OnClick="DeleteFolderbtn_Click" />

                        <asp:FileUpload ID="FileUploadSW" ClientIDMode="Static" runat="server" CssClass="hidden" AllowMultiple="true" Onchange="UploadFile(this)" />
                        <asp:Button ID="FileUploadbtn" runat="server" Style="display: none" OnClick="FileUploadbtn_Click" />
                        <asp:Button type="button" ID="AddFilebtn" class="btn bleu rounded" Text="Add file" runat="server" Visible="false" OnClientClick="FileUploadShow(); return false;" />
                        <asp:Label runat="server" ID="FileUploapErrorLabel" Style="color: red" />
                    </asp:Panel>
                    <table>
                        <tr>
                            <td style="vertical-align: top; padding-right: 40px">
                                <table>
                                    <tr>
                                        <td style="vertical-align: middle; align-items: center; width: 40px;">
                                            <div class="vertical-stack">
                                                <div>
                                                    <asp:ImageButton type="button" ID="FolderTopBtn" class="bleu rounded" src="Images/angle-double-small-up.png" runat="server" Style="width: 30px; height: 30px;" OnClick="FolderRankingBtn_Click" />
                                                </div>
                                                <div>
                                                    <asp:ImageButton type="button" ID="FolderUpBtn" class="bleu rounded" src="Images/angle-small-up.png" runat="server" Style="width: 30px; height: 30px;" OnClick="FolderRankingBtn_Click" />
                                                </div>
                                                <div>
                                                    <asp:ImageButton type="button" ID="FolderDownBtn" class="bleu rounded" src="Images/angle-small-down.png" runat="server" Style="width: 30px; height: 30px;" OnClick="FolderRankingBtn_Click" />
                                                </div>
                                                <div>
                                                    <asp:ImageButton type="button" ID="FolderBottomBtn" class="bleu rounded" src="Images/angle-double-small-down.png" runat="server" Style="width: 30px; height: 30px;" OnClick="FolderRankingBtn_Click" />
                                                </div>
                                            </div>
                                        </td>
                                        <td style="vertical-align: top;">
                                            <telerik:RadTreeView ID="FileManagementRadTreeView" runat="server" Font-Bold="true" OnNodeClick="RadTreeView_NodeClick"
                                                OnContextMenuItemClick="RadTreeView_ContextMenuItemClick" OnNodeEdit="RadTreeView_NodeEdit"
                                                OnClientContextMenuItemClicking="onClientContextMenuItemClicking" OnClientContextMenuShowing="onClientContextMenuShowing">
                                                <ContextMenus>
                                                    <telerik:RadTreeViewContextMenu ID="FileManagementRadTreeViewContextMenu" runat="server">
                                                        <Items>
                                                            <telerik:RadMenuItem Value="Rename" Text="Rename" PostBack="false" />
                                                            <telerik:RadMenuItem Value="NewFolder" Text="New Folder" />
                                                            <telerik:RadMenuItem Value="Delete" Text="Delete Folder" />
                                                            <telerik:RadMenuItem Value="AddFile" Text="Add file" />
                                                        </Items>
                                                        <CollapseAnimation Type="none"></CollapseAnimation>
                                                    </telerik:RadTreeViewContextMenu>
                                                </ContextMenus>
                                                <Nodes>
                                                    <telerik:RadTreeNode Value="_Private_PersonalFolders" Expanded="true" ExpandMode="ServerSide">
                                                    </telerik:RadTreeNode>
                                                </Nodes>
                                            </telerik:RadTreeView>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="vertical-align: top;">
                                <table>
                                    <tr>
                                        <td style="vertical-align: middle; align-items: center; width: 40px;">
                                            <div class="vertical-stack">
                                                <div>
                                                    <asp:ImageButton type="button" ID="TopBtn" class="bleu rounded" src="Images/angle-double-small-up.png" runat="server" Style="display: none; width: 30px; height: 30px;" OnClick="RankingBtn_Click" />
                                                </div>
                                                <div>
                                                    <asp:ImageButton type="button" ID="UpBtn" class="bleu rounded" src="Images/angle-small-up.png" runat="server" Style="display: none; width: 30px; height: 30px;" OnClick="RankingBtn_Click" />
                                                </div>
                                                <div>
                                                    <asp:ImageButton type="button" ID="DownBtn" class="bleu rounded" src="Images/angle-small-down.png" runat="server" Style="display: none; width: 30px; height: 30px;" OnClick="RankingBtn_Click" />
                                                </div>
                                                <div>
                                                    <asp:ImageButton type="button" ID="BottomBtn" class="bleu rounded" src="Images/angle-double-small-down.png" runat="server" Style="display: none; width: 30px; height: 30px;" OnClick="RankingBtn_Click" />
                                                </div>
                                            </div>
                                        </td>
                                        <td style="vertical-align: top;">
                                            <telerik:RadGrid RenderMode="Lightweight" runat="server" ID="FileManagementRadGrid" AllowPaging="True" AllowSorting="true" AllowMultiRowSelection="true"
                                                AutoGenerateColumns="False" Width="100%" OnDeleteCommand="RadGrid_OnCommand" OnItemCommand="RadGrid_OnCommand">
                                                <PagerStyle Mode="NextPrevAndNumeric" />
                                                <MasterTableView DataKeyNames="ID, SendNotification">
                                                    <Columns>
                                                        <telerik:GridClientSelectColumn UniqueName="Select" HeaderStyle-Width="30px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                                            HeaderStyle-CssClass="select-header" ItemStyle-CssClass="select-item" />
                                                        <telerik:GridTemplateColumn AllowFiltering="false">
                                                            <ItemTemplate>
                                                                <asp:LinkButton runat="server" ID="EditFileLinkBtn" Text="Edit" CommandName="ManageSecurity">
                                                                    <img src="Images/edit.png" alt="Edit file" width="20" height="20">
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn AllowFiltering="false">
                                                            <ItemTemplate>
                                                                <asp:LinkButton runat="server" ID="DeleteFileLinkBtn" Text="Delete" CommandName="Delete" OnClientClick='<%# String.Format("return DeleteFile(""{0}"");", Eval("Name")) %>'>
                                                                    <img src="Images/trash.png" alt="Delete file" width="20" height="20" />
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn AllowFiltering="false">
                                                            <ItemTemplate>
                                                                <asp:LinkButton runat="server" ID="DownloadFileLinkBtn" Text="Download" CommandName="Download">
                                                                    <img src="Images/Download.jpg" alt="Download file" width="20" height="20">
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridBoundColumn SortExpression="ID" DataField="ID" HeaderText="ID" HeaderButtonType="TextButton" Display="false" ItemStyle-CssClass="ID" />
                                                        <telerik:GridBoundColumn SortExpression="CompaniesCount" DataField="CompaniesCount" HeaderText="Compagnies count"
                                                            HeaderButtonType="TextButton" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="30px" />
                                                        <telerik:GridTemplateColumn HeaderText="Thumbnail" AllowFiltering="false" ItemStyle-HorizontalAlign="Center"
                                                            HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="80px">
                                                            <ItemTemplate>
                                                                <asp:Image runat="server" ID="ThumbnailImage" Width="50px" Height="50px"
                                                                    ImageUrl='<%# If(Eval("ThumbnailContent") IsNot Nothing AndAlso Eval("ThumbnailContent") IsNot DBNull.Value, "data:image/png;base64," & Convert.ToBase64String(CType(Eval("ThumbnailContent"), Byte())), String.Empty) %>' />
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridBoundColumn SortExpression="Name" DataField="Name" HeaderText="File name" HeaderButtonType="TextButton" />
                                                        <telerik:GridBoundColumn SortExpression="Size" DataField="Size" HeaderText="Size" HeaderButtonType="TextButton"
                                                            ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" />
                                                        <telerik:GridBoundColumn SortExpression="Extension" DataField="Extension" HeaderText="Extension" HeaderButtonType="TextButton"
                                                            HeaderStyle-Width="80px" />
                                                        <telerik:GridBoundColumn SortExpression="Type" DataField="Type" HeaderText="Type" HeaderButtonType="TextButton" Visible="false" />
                                                        <telerik:GridBoundColumn SortExpression="CreationDate" DataField="CreationDate" HeaderText="Last update"
                                                            HeaderButtonType="TextButton" DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-Width="50px" HeaderStyle-HorizontalAlign="Center" />
                                                        <telerik:GridBoundColumn SortExpression="UseDateRangeForPublishing" DataField="UseDateRangeForPublishing"
                                                            HeaderText="UseDateRangeForPublishing" HeaderButtonType="TextButton" Visible="false" />
                                                        <telerik:GridBoundColumn SortExpression="ThumbnailName" DataField="ThumbnailName" HeaderText="Thumb" Display="false"
                                                            HeaderButtonType="TextButton" />
                                                        <telerik:GridTemplateColumn SortExpression="StartDate" DataField="StartDate" HeaderText="Published from"
                                                            HeaderButtonType="TextButton" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                                            HeaderStyle-Width="50px">
                                                            <ItemTemplate>
                                                                <%# IIf(Eval("UseDateRangeForPublishing") = False, String.Empty, DateTime.Parse(Eval("StartDate")).ToString("dd/MM/yyyy"))  %>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn SortExpression="EndDate" DataField="EndDate" HeaderText="Published to" HeaderButtonType="TextButton"
                                                            ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="50px">
                                                            <ItemTemplate>
                                                                <%# IIf(Eval("UseDateRangeForPublishing") = False, String.Empty, DateTime.Parse(Eval("EndDate")).ToString("dd/MM/yyyy"))  %>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridCheckBoxColumn SortExpression="SendNotification" DataField="SendNotification" HeaderText="Send notification"
                                                            ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="50px" />
                                                    </Columns>
                                                </MasterTableView>
                                                <ClientSettings AllowDragToGroup="True" AllowRowsDragDrop="True">
                                                    <Selecting AllowRowSelect="True" />
                                                    <ClientEvents OnRowDropped="RowDropped" OnRowSelected="RowSelected" OnRowDeselected="RowSelected" />
                                                </ClientSettings>
                                            </telerik:RadGrid>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>

                </div>

            </asp:Panel>

            <asp:Panel runat="server" ID="ManageSecurityPanel" Style="display: none;">

                <div>
                    <uc1:FilesManagerManageSecurity runat="server" ID="FilesManagerManageSecurity" OnBackFileUploadBtnClick="FilesManagerManageSecurity_BackFileUploadBtnClick" />
                </div>

            </asp:Panel>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="FileManagementRadTreeView" />
            <asp:PostBackTrigger ControlID="FileUploadbtn" />
        </Triggers>
    </asp:UpdatePanel>

    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" />

    <telerik:RadWindow ID="StaticAssignmentWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false"
        VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false"
        Behaviors="Close" Width="960px" Height="680px" runat="server"
        OnClientClose="handleStaticSaveComplete">
    </telerik:RadWindow>

    <telerik:RadWindow ID="InDirectAssignmentWindow" CssClass="InDirectAssignmentWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false"
        VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false"
        Behaviors="Close" Width="960px" Height="680px" runat="server"
        OnClientClose="handleInDirectSaveComplete">
    </telerik:RadWindow>

    <telerik:RadWindow ID="RadWindowPreview" CssClass="radwindowPreview" runat="server" Title="Matching Customers Preview"
        Modal="true" VisibleOnPageLoad="false" Behaviors="Close, Move, Resize"
        Width="800px" Height="600px" RenderMode="Lightweight">
        <ContentTemplate>
            <div class="contentPreview">
                <h3>Preview: <span id="customerCountPreview" style="font-weight: bold; color: #0056b3;">0</span> Matching Customers</h3>
                <hr />

                <table id="previewTable" class="display">
                    <thead>
                        <tr>
                            <th>Customer Code</th>
                            <th>Customer Name</th>
                        </tr>
                    </thead>
                    <tbody>
                        <%-- Data will be injected here by JavaScript --%>
                    </tbody>
                </table>

            </div>
        </ContentTemplate>
    </telerik:RadWindow>


</asp:Content>
