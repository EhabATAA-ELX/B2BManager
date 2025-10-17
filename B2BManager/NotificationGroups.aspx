<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="NotificationGroups.aspx.vb" Inherits="NotificationGroups" ValidateRequest="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="Scripts/DataTables/datatables.min.css" rel="stylesheet" />
    <link href="CSS/jquery-ui.css" rel="stylesheet" />
    <link href="Scripts/DataTables/datatables.css" rel="stylesheet" />
    <script src="Scripts/DataTables/datatables.min.js"></script>
    <script src="Scripts/DataTables/Buttons-1.5.6/js/dataTables.buttons.min.js"></script>
    <script src="Scripts/DataTables/Select-1.3.0/js/dataTables.select.min.js"></script>
    <script src="Scripts/jquery-ui.js"></script>
    <style>
        .dataTables_filter label {
            display: block;
            float: left;
        }

            .dataTables_filter label input {
                font-weight: normal;
                width: 230px;
                margin-left: 37px !important;
            }

        th {
            text-align: left;
        }

        .popupTable {
            border-collapse: separate;
            border-spacing: 0 12px;
        }

        .rwStatusbarRow {
            display: none !important;
        }

        .radwindow-popup {
            position: absolute !important;
            top: 50% !important;
            left: 50% !important;
            transform: translate(-50%, -50%) !important;
            -webkit-transform: translate(-50%, -50%) !important;
            -ms-transform: translate(-50%, -50%) !important;
        }

        .style-input {
            border: 1px solid black;
        }
    </style>
    <script>
        $(document).ready(function () {
            BindNotificationTable();
        });

        var notifTable;
        function BindNotificationTable() {
            HideLoadingPanel();
            var tableSelector = "[id$='NotificationTable']";
            var tableElem = $(tableSelector);
            // If DataTable is already initialized, destroy it before reinitializing
            if ($.fn.DataTable.isDataTable(tableElem)) {
                tableElem.DataTable().destroy();
            }
            tableElem.removeClass("DisplayNone");
            notifTable = tableElem.DataTable({
                stateSave: true,
                pageLength: 15,
                lengthChange: false,
                columns: [
                    { width: "50px", orderable: false },
                    { width: "150px" },
                    { width: "100px" },
                    { width: "200px" },
                    { width: "150px" },
                    { width: "150px" },
                    { width: "150px" },
                    { width: "75px" }
                ],
                order: [[2, "asc"]],
                dom: '<"floatLeft Width316px"f><t><"DatatableBottom"<i><pl>><"clear">'
            });
        }

        function EditNotification(id) {
            var oWnd = $find("<%= wndGroupPopup.ClientID %>");
            $("#" + "<%= hfEditGroupId.ClientID %>").val(id);
            var country = $("#" + id + "_S_SOP_ID").text();
            var groupName = $("#" + id + "_N_GROUP_NAME").text();
            var category = $("#" + id + "_N_CATEGORY").text();
            var condField = $("#" + id + "_N_CONDITION_FIELD").text();
            var condValue = $("#" + id + "_N_CONDITION_VALUE").text();
            var isActive = $("#" + id + "_N_ISACTIVE").text();
            var popupCountry = $find("<%= ddlPopupCountry.ClientID %>");
            popupCountry.clearSelection();
            var item = popupCountry.findItemByValue(country);
            if (item) item.select();

            $("#<%= txtGroupName.ClientID %>").val(groupName);
            $("#<%= txtCategory.ClientID %>").val(category);
            $("#<%= ddlConditionField.ClientID %>").val(condField);
            $("#<%= txtConditionValue.ClientID %>").val(condValue);
            $("#<%= rblIsActive.ClientID %> input[value='" + isActive + "']").prop("checked", true);
            oWnd.show();
        }


        function updateNotification(id) {
            ShowLoadingPanel();
            var url = "NotificationGroups.aspx/UpdateGroup";
            var data = getNotificationData(id);
            $.ajax({
                method: "POST",
                url: url,
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function () {
                    HideLoadingPanel();
                    ViewNotification(id, false);
                },
                error: function (xhr) {
                    HideLoadingPanel();
                    alert(xhr.responseJSON.Message);
                }
            });
        }

        function DeactivateNotification(id) {
            if (!confirm("Are you sure you want to deactivate this group?")) return;
            ShowLoadingPanel();
            $.ajax({
                method: "POST",
                url: "NotificationGroups.aspx/DeactivateGroup",
                data: JSON.stringify({ groupId: id }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function () {
                    HideLoadingPanel();
                    notifTable.ajax.reload(null, false);
                },
                error: function (xhr) {
                    HideLoadingPanel();
                    alert(xhr.responseJSON.Message);
                }
            });
        }

        function getNotificationData(id) {
            return {
                groupId: id,
                country: $("#" + id + "_Country").text(),
                groupName: $("#" + id + "_GroupName input").val(),
                category: $("#" + id + "_Category input").val(),
                conditionField: $("#" + id + "_ConditionField select").val(),
                conditionValue: $("#" + id + "_ConditionValue input").val(),
                isActive: $("#" + id + "_IsActive select").val()
            };
        }

        function ClearGroupPopup() {
            $("#<%= hfEditGroupId.ClientID %>").val("");
            var popupCountry = $find("<%= ddlPopupCountry.ClientID %>");
            if (popupCountry) {
                popupCountry.clearSelection();
            }
            $("#<%= txtGroupName.ClientID %>").val("");
            $("#<%= txtCategory.ClientID %>").val("");
            $("#<%= txtConditionValue.ClientID %>").val("");
            $("#<%= ddlConditionField.ClientID %>").val("");
            $("#<%= rblIsActive.ClientID %> input[value='False']").prop("checked", true);
        }

        function AddGroupPopup() {
            ClearGroupPopup();
            var oWnd = $find("<%= wndGroupPopup.ClientID %>");
            oWnd.show();
        }

        function CloseWindow() {
            var oWnd = $find("<%= wndGroupPopup.ClientID %>");
            if (oWnd !== null) {
                oWnd.close();
            }
        }

        function ShowLoadingPanel() { $("#LoadingPanel").removeClass("hidden"); }
        function HideLoadingPanel() { $("#LoadingPanel").addClass("hidden"); }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Label ID="lblEnvironment" runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
    <asp:DropDownList ID="ddlEnvironment" runat="server" CssClass="ddl_Text Electrolux_Color width230px" AutoPostBack="true" OnSelectedIndexChanged="ddlEnvironment_SelectedIndexChanged" OnChange="ShowLoadingPanel();">
    </asp:DropDownList>

    <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
    <telerik:RadComboBox ID="ddlCountry" runat="server" CssClass="ddl_Text Electrolux_Color width230px"
        AutoPostBack="true" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged">
        <Items>
            <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
        </Items>
    </telerik:RadComboBox>


    <input type="button" id="btnAddGroup" runat="server" class="btn bleu" value="New Group" onclick="AddGroupPopup(); return false;" />

    <asp:UpdatePanel ID="UpdatePanelGroups" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
            <div id="LoadingPanel" class="hidden" style="position: absolute; top: 80px; width: 100%; height: 100%; z-index: 1000;">
                <asp:Image ID="imgLoading" runat="server" ImageUrl="Images/Loading.gif" />
            </div>
            <asp:Table ID="NotificationTable" runat="server" CssClass="DisplayNone" Style="border-collapse: collapse; font-size: 12px; width: 100%;">
                <asp:TableHeaderRow TableSection="TableHeader" runat="server">
                    <asp:TableHeaderCell>Actions</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Country</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Group ID</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Group Name</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Category</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Condition Field</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Condition Value</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Is Active</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="ddlCountry" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>
    <telerik:RadWindow ID="wndGroupPopup" runat="server" CssClass="radwindow-popup" Modal="true" VisibleOnPageLoad="false"
        Title="Add New Notification Group" Width="600px" Height="350px" Behaviors="Close" OnClientBeforeClose="ClearGroupPopup">
        <ContentTemplate>
            <div class="padding14px">
                <asp:HiddenField ID="hfEditGroupId" runat="server" />

                <table style="width: 100%" class="popupTable">
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblPopupCountry" Text="Country:" CssClass="Electrolux_light_bold Electrolux_Color" /></td>
                        <td>
                            <telerik:RadComboBox ID="ddlPopupCountry" runat="server" CssClass="ddl_Text style-input Electrolux_Color width230px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblPopupGroupName" Text="Group Name:" CssClass="Electrolux_light_bold Electrolux_Color" /></td>
                        <td>
                            <asp:TextBox ID="txtGroupName" runat="server" CssClass="Electrolux_Color style-input" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblPopupCategory" Text="Category:" CssClass="Electrolux_light_bold Electrolux_Color" /></td>
                        <td>
                            <asp:TextBox ID="txtCategory" runat="server" CssClass="Electrolux_Color style-input" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblPopupConditionField" Text="Condition Field:" CssClass="Electrolux_light_bold Electrolux_Color" /></td>
                        <td>
                            <asp:DropDownList ID="ddlConditionField" runat="server" CssClass="ddl_Text style-input Electrolux_Color width230px" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblPopupConditionValue" Text="Condition Value:" CssClass="Electrolux_light_bold Electrolux_Color" /></td>
                        <td>
                            <asp:TextBox ID="txtConditionValue" runat="server" CssClass="Electrolux_Color style-input" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblPopupIsActive" Text="Activate:" CssClass="Electrolux_light_bold Electrolux_Color" /></td>
                        <td>
                            <asp:RadioButtonList ID="rblIsActive" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Text="Yes" Value="True" />
                                <asp:ListItem Text="No" Value="False" Selected="True" />
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="text-align: center; padding-top: 10px;">
                            <input type="button" class="btn red" id="btnCancelDispalyActionProfile" value="Cancel" onclick="CloseWindow()">
                            <asp:Button runat="server" ID="btnSubmitGroup" Text="Submit" UseSubmitBehavior="false" CssClass="btn green" OnClick="btnAddGroup_Click" />
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </telerik:RadWindow>
</asp:Content>

