<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="FocusRange.aspx.vb" Inherits="FocusRange" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="CSS/jquery-ui.css?v=1.1" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-ui.js?v=1.1"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <link rel="stylesheet" href="https://cdn.datatables.net/1.13.4/css/jquery.dataTables.min.css" />
    <script src="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js"></script>

    <script type="text/javascript">
        function RequestStartSimple(sender, eventArgs) {
            centerElementOnScreen($get("RadAjaxLoadingPanel1"));
        }
    </script>
    <asp:PlaceHolder runat="server" ID="placeHolder">
        <script type="text/javascript">
            function Finish(Action) {
                CloseWindowManagement();
                if (Action == 'Save') {
                    $.magnificPopup.open({
                        items: {
                            src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> Core range has been added with success, use the button to create a new one or just press on x to close this window and refresh the main list.</p>'
                                + '</br> <button type="button" class="btn blue btn-submit" onclick=" ManagementPopup()">New Core range</button></div> ',
                            type: 'inline'
                        },
                        callbacks: {
                            close: function () {
                                __doPostBack("<%= btnSearch.UniqueID %>", "");
                                $.magnificPopup.close();
                            }
                        }
                    });
                }
                if (Action == 'Update') {
                    __doPostBack("<%= btnSearch.UniqueID %>", "");
                }
            }

            function ManagementPopup(id, title, startDate, endDate) {
                var oWnd = $find("<%= WindowManagement.ClientID %>");
                oWnd.setUrl("FocusRangeManagement.aspx?HideHeader=true&envid="
                    + $('#<%= ddlEnvironment.ClientID %>').val()
                    + "&sopid=" + $('#<%= hdSelectedSopid.ClientID %>').val()
                    + (id ? "&id=" + id : "")
                    + (startDate ? "&startDate=" + startDate : "")
                    + (endDate ? "&endDate=" + endDate : "")
                    + (title ? "&title=" + title : ""));
                oWnd.set_title('Loading...');
                oWnd.show();
            }

            function AssignmentPopup(id) {
                var oWnd = $find("<%= WindowAssignment.ClientID %>");
                oWnd.setUrl("FocusRangeAssignment.aspx?HideHeader=true&envid=" + $('#<%= ddlEnvironment.ClientID %>').val() + "&sopid=" + $('#<%= hdSelectedSopid.ClientID %>').val() + (id ? "&id=" + id : ""));
                oWnd.set_title('Loading...');
                oWnd.show();
            }

            /* -----------Start Assignment Direc/InDirect Condition ----------- */
            /*********************************************************************/
            function CloseWindowManagement() {
                var oWnd = $find("<%= WindowManagement.ClientID %>");
                oWnd.close();
            }

            function CloseDeleteConfirmationWindow() {
                $("#dialog-error-info").text("");
                $('.ui-dialog-content:visible').dialog('close');
                $("#dialog-confirm-delete").dialog('close');
            }

            function DeleteFocusRange(focusRangeId, focusRangeTitle,) {

                $("#dialog-delete-text").html("Are you sure you want to delete the focus range <b>" + focusRangeTitle + "</b> ?")
                $("#dialog-error-info").text("");
                $("#btnConfirmDelete").unbind("click");
                $("#btnConfirmDelete").removeClass("loadingBackground").html("Delete").removeAttr('disabled');
                $("#btnConfirmDelete").on("click", function (event) {
                    $("#btnConfirmDelete").addClass("loadingBackground").html("Deleting..").prop('disabled', true);
                    $.ajax({
                        type: 'Get',
                        url: 'B2BManagerService.svc/DeleteFocusRange',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: { Envid: $('#<%= ddlEnvironment.ClientID %>').val(), FocusRangeId: focusRangeId, focusRangeTitle },
                        async: true,
                        success: function (response) {
                            if (response.toLowerCase() == "success") {
                                CloseDeleteConfirmationWindow();
                                __doPostBack("<%= btnSearch.UniqueID %>", "");
                            }
                            else {
                                $("#btnConfirmDelete").removeClass("loadingBackground").html("Delete").removeAttr('disabled');
                                $("#dialog-error-info").text(response);
                            }
                        },
                        error: function (e) {
                            $("#btnConfirmDelete").removeClass("loadingBackground").html("Delete").removeAttr('disabled');
                            $("#dialog-error-info").text(e.statusText);
                        }
                    });
                });

                $("#dialog-confirm-delete").dialog({
                    resizable: false,
                    height: "auto",
                    width: 400,
                    modal: true,
                    title: "Delete Focus range"
                });
            }

            function CloseWindowAssignment() {
                var oWnd = $find("<%= WindowAssignment.ClientID %>");
                 oWnd.close();
            }

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

                var managementWnd = $find("<%= WindowManagement.ClientID %>");
                if (managementWnd) {
                    var mgmtContent = managementWnd.get_contentFrame().contentWindow;

                    if (typeof mgmtContent.refreshGrid === "function") {
                        mgmtContent.refreshGrid();
                    }
                }
            }

            function onInDirectWindowBeforeClose(sender, args) {
                var managementWnd = $find("<%= WindowManagement.ClientID %>");
                if (managementWnd) {
                    var mgmtContent = managementWnd.get_contentFrame().contentWindow;

                    if (typeof mgmtContent.refreshGrid === "function") {
                        mgmtContent.refreshGrid();
                    }
                }
            }

            function closeActiveWindow() {
                var oWnd = $find("<%= StaticAssignmentWindow.ClientID %>");
                 if (oWnd) {
                     oWnd.close();
                 }
            }

            //Function to be called form the page QueryBuilderStaticAssignment.aspx , when user click on button Save Changes 
            //refreshGrid function used to execute rebind Action for the list of Condition on FocusRangeManagement.aspx
            function handleStaticSaveComplete() {
                var managementWnd = $find("<%= WindowManagement.ClientID %>");
                if (managementWnd) {
                    var mgmtContent = managementWnd.get_contentFrame().contentWindow;
                    $('input[id$="AddStaticConditionBtn"]', mgmtContent.document).addClass('btn-disabled');

                    if (typeof mgmtContent.refreshGrid === "function") {
                        mgmtContent.refreshGrid();
                    }
                }

                var staticWnd = $find("<%= StaticAssignmentWindow.ClientID %>");
                if (staticWnd) {
                    staticWnd.close();
                }
            }

            //Function to be called form the page QueryBuilder.aspx , when user click on button Save Changes 
            //refreshGrid function used to execute rebind Action for the list of Condition on FocusRangeManagement.aspx
            function handleInDirectAssignmentSaveComplete() {
                var managementWnd = $find("<%= WindowManagement.ClientID %>");
                if (managementWnd) {
                    var mgmtContent = managementWnd.get_contentFrame().contentWindow;

                    if (typeof mgmtContent.refreshGrid === "function") {
                        mgmtContent.refreshGrid();
                    }
                }

                var indirectWnd = $find("<%=  InDirectAssignmentWindow.ClientID %>");
                if (indirectWnd) {
                    indirectWnd.close();
                }
            }

            /*Start Preview all customers*/

            function loadAndShowPreview(focusRangeId) {
                if (!focusRangeId) {
                    alert('Could not find the required ID in the URL.');
                    return;
                }

                // Show the window
                var radWindow = $find("<%= RadWindowPreview.ClientID %>");
                radWindow.show();

                // Optional: Show a loading message
                $('#previewTable tbody').html('<tr><td colspan="2"><em>Loading...</em></td></tr>');
                $('#customerCountPreview').text('...');

                // Make the AJAX call to your WebMethod
                $.ajax({
                    type: "POST",
                    url: "FocusRangeManagement.aspx/GetPreviewCustomers",
                    data: JSON.stringify({ id: focusRangeId, pageSource: "FocusRange" }),
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
        <style type="text/css">
            .windowManagement {
                z-index: 3000 !important;
            }

            .staticAssignmentWindow {
                z-index: 3005 !important;
            }

            .InDirectAssignmentWindow {
                z-index: 3005 !important;
            }

            .contentPreview{
                padding :30px!important ;
            }
        </style>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Panel runat="server" ID="dialogDeletePanel">
        <div id="dialog-confirm-delete" title="Delete Confirmation" class="DisplayNone">
            <div id="dialog-delete-text" style="margin: 15px"></div>
            <table align="right">
                <tr style="text-align: left">
                    <td colspan="2">
                        <span id="dialog-error-info" style="color: red; height: 20px">&nbsp</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <button class="btn bleu" onclick="CloseDeleteConfirmationWindow()">Cancel</button>
                    </td>
                    <td>
                        <button class="btn red" id="btnConfirmDelete">Confirm</button>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <table class="Filters">
                <tr>
                    <td>
                        <input type="button" id="btnAddFocusRange" class="btn bleu"
                            onclick="ManagementPopup()"
                            value="New Core Range" runat="server" />
                    </td>

                    <td class="width120px">
                        <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
                    </td>
                    <td class="width180px">
                        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlEnvironment"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlEnvironment_SelectedIndexChanged" DataTextField="Name" DataValueField="ID">
                        </asp:DropDownList>
                        <asp:HiddenField runat="server" ID="hdSelectedSopid" />
                    </td>
                    <td class="width120px">
                        <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
                    </td>
                    <td class="width180px">
                        <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width230px"
                            OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true" ID="ddlCountry">
                        </telerik:RadComboBox>
                    </td>

                    <td>
                        <asp:LinkButton runat="server" ID="btnSearch"
                            CssClass="btn bleu" OnClientClick="ProcessButton(this,'Searching...')"
                            OnClick="handleRefreshEvent"><i class="fas fa-search"></i> Search</asp:LinkButton>
                        <td>
                </tr>
            </table>
            <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
                <ClientEvents OnRequestStart="RequestStartSimple" />
                <AjaxSettings>
                    <telerik:AjaxSetting AjaxControlID="ddlEnvironment">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="gridContainer" LoadingPanelID="RadAjaxLoadingPanel1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                    <telerik:AjaxSetting AjaxControlID="ddlCountry">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="gridContainer" LoadingPanelID="RadAjaxLoadingPanel1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                    <telerik:AjaxSetting AjaxControlID="btnSearch">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="gridContainer" LoadingPanelID="RadAjaxLoadingPanel1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                    <telerik:AjaxSetting AjaxControlID="gridSearch">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="gridContainer" LoadingPanelID="RadAjaxLoadingPanel1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                </AjaxSettings>
            </telerik:RadAjaxManager>
            <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" ClientIDMode="Static" IsSticky="true" Transparency="10" runat="server" Style="position: absolute; z-index: 999;">
                <asp:Image ID="Image1" runat="server" AlternateText="Loading..." ImageUrl="Images/Loading.gif" />
            </telerik:RadAjaxLoadingPanel>
            <div id="gridContainer" runat="server" style="position: relative; min-height: 500px">
                <span id="lblInformationContainer" enableviewstate="false" runat="server" style="position: absolute; top: 5px">
                    <span class="information-label" runat="server" id="lblInformation"></span></span>
                <telerik:RadGrid runat="server" ID="gridSearch" ShowGroupPanel="true"
                    AutoGenerateColumns="true" CssClass="LogGridSearch"
                    AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true"
                    ClientSettings-DataBinding-EnableCaching="true"
                    PageSize="20" OnNeedDataSource="gridSearch_NeedDataSource" GroupingEnabled="true" OnPreRender="gridSearch_PreRender">
                    <ClientSettings AllowDragToGroup="true" />
                    <MasterTableView AutoGenerateColumns="false" TableLayout="Auto">
                        <PagerStyle AlwaysVisible="false" Mode="NextPrevNumericAndAdvanced" />
                        <Columns>
                            <telerik:GridTemplateColumn UniqueName="Actions" HeaderText="Actions"
                                Groupable="false" Reorderable="false"
                                AllowFiltering="false" HeaderStyle-Width="40">
                                <ItemTemplate>
                                    <table border="0">
                                        <tr style="border: none">
                                            <td style="border: none" runat="server" id="tdView">
                                                <img src='Images/Edit.png' onclick="ManagementPopup('<%# Eval("FocusRangeID").ToString() %>','<%# Eval("FocusRangeName").ToString() %>','<%# Eval("START_DATE").ToString() %>','<%# Eval("END_DATE").ToString() %>')" width="20" class="MoreInfoImg" height="20" title="View Core Range" /></td>
                                            <td style="display: none !important; border: none" runat="server" id="tdAssign">
                                                <img src='Images/detailCompany.png' onclick="AssignmentPopup('<%# Eval("FocusRangeID").ToString() %>')" width="20" class="MoreInfoImg" height="20" title="Assign Core Range to Customers" /></td>
                                            <td style="border: none" runat="server" id="tdDelete">
                                                <img src='Images/Delete.png' onclick="DeleteFocusRange('<%# Eval("FocusRangeID").ToString() %>','<%# Eval("FocusRangeName").ToString() %>')" width="20" class="MoreInfoImg" height="20" title="Delete Core range" /></td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn AllowSorting="false" AllowFiltering="false" Aggregate="None" Visible="false"
                                DataField="FocusRangeId" HeaderText="">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None"
                                DataField="FocusRangeName" HeaderText="Title">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None"
                                DataField="CreatedAt" HeaderText="Creation date">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None"
                                DataField="CreatedByEmail" HeaderText="Author">
                            </telerik:GridBoundColumn>

                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None"
                                DataField="START_DATE" DataType="System.DateTime" DataFormatString="{0:d}" HeaderText="Start date">
                            </telerik:GridBoundColumn>

                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None"
                                DataField="END_DATE" DataType="System.DateTime" DataFormatString="{0:d}" HeaderText="End date">
                            </telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <telerik:RadWindow ID="WindowManagement" CssClass="windowManagement" RenderMode="Lightweight" Modal="true" DestroyOnClose="false"
        VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false"
        Behaviors="Close" Width="640px" Height="480px" runat="server">
    </telerik:RadWindow>
    <telerik:RadWindow ID="WindowAssignment" RenderMode="Lightweight" Modal="true" DestroyOnClose="false"
        VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false"
        Behaviors="Close" Width="960px" Height="680px" runat="server">
    </telerik:RadWindow>

    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" />

    <telerik:RadWindow ID="StaticAssignmentWindow" CssClass="staticAssignmentWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false"
        VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false"
        Behaviors="Close" Width="960px" Height="680px" runat="server"
        OnClientBeforeClose="onStaticWindowBeforeClose">
    </telerik:RadWindow>

    <telerik:RadWindow ID="InDirectAssignmentWindow" CssClass="InDirectAssignmentWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false"
        VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false"
        Behaviors="Close" Width="960px" Height="680px" runat="server"
        OnClientBeforeClose="onInDirectWindowBeforeClose">
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

