<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="NotificationsManagement.aspx.vb" Inherits="NotificationsManagement" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="Scripts/filesManager/filesManagerStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <style type="text/css">
        .RadGrid tr.rgSelectedRow td, .RadGrid tr.rgSelectedRow {
            background-color: #a5d8ff;
        }

        .select-item input[type="checkbox"] {
            position: relative;
            top: 5px;
        }

        td {
            line-height: 40px;
        }

        .popupTable {
            border-collapse: separate;
            border-spacing: 0 12px;
        }

        .radwindow-popup {
            position: absolute !important;
            top: 50% !important;
            left: 50% !important;
            transform: translate(-50%, -50%) !important;
            -webkit-transform: translate(-50%, -50%) !important;
            -ms-transform: translate(-50%, -50%) !important;
        }

        .btnCustom {
            background-color: #e9e9e9 !important;
            color: #000000 !important;
            font-weight: bold;
        }
    </style>
    <script type="text/javascript">
        function ProcessButton(sender) {
            $("#" + sender.id).addClass("loadingBackground").html("Computing...").val("Computing...").prop('disabled', true);
            return false;
        }

        function OpenInfoRw(message) {
            $("#<%= InfoLbl.ClientID %>").text(message);
            $find("<%= InfoRw.ClientID %>").show();
        }

        function OpenConfirmationRw(message) {
            $("#<%= ConfirmationLbl.ClientID %>").text(message);
            $find("<%= ConfirmationRw.ClientID %>").show();
        }

        function CloseWindow(sender) {
            switch (sender.id) {
                case "<%= NoBtn.ClientID %>":
                    $find("<%= ConfirmationRw.ClientID %>").close();
                    break;
                case "<%= InfoBtn.ClientID %>":
                    $find("<%= InfoRw.ClientID %>").close();
                    break;
            }
        }
    </script>

    <asp:UpdatePanel ID="NotificationsManagementUp" runat="server">
        <ContentTemplate>
            <table>
                <tr>
                    <td>
                        <asp:Label runat="server" Text="Environment" />
                    </td>
                    <td style="padding-left: 5px;">
                        <telerik:RadComboBox ID="EnvironmentRcb" runat="server" AutoPostBack="true" Width="100px" OnSelectedIndexChanged="EnvironmentRcb_SelectedIndexChanged" />
                    </td>
                    <td style="padding-left: 10px;">
                        <asp:Label runat="server" Text="Country" />
                    </td>
                    <td style="padding-left: 5px;">
                        <telerik:RadComboBox ID="CountryRcb" runat="server" Width="180px" />
                    </td>
                    <td style="padding-left: 10px;">
                        <asp:Label runat="server" Text="Customer Code" />
                    </td>
                    <td style="padding-left: 5px;">
                        <telerik:RadTextBox ID="SearchCustomerRtb" runat="server" MaxLength="50" />
                    </td>
                    <td style="padding-left: 10px;">
                        <asp:Button ID="OKbtn" runat="server" type="button" UseSubmitBehavior="false" class="btn bleu rounded" OnClientClick="ProcessButton(this)" OnClick="OKbtn_Click" Text="OK" />
                    </td>
                </tr>
                <tr>
                    <%--<td>
                        <asp:Label runat="server" Text="Assignment Type" />
                    </td>
                    <td style="padding-left: 5px;">
                        <telerik:RadComboBox ID="AssignmentTypeRcb" runat="server" Width="100px" />
                    </td>--%>
                    <td>
                        <asp:Label runat="server" Text="Object Type" />
                    </td>
                    <td style="padding-left: 5px;">
                        <telerik:RadComboBox ID="ObjectTypeRcb" runat="server" Width="100px" />
                    </td>
                    <td style="padding-left: 10px;">
                        <asp:Label runat="server" Text="Users Email" />
                    </td>
                    <td style="text-align: right">
                        <telerik:RadTextBox ID="SearchEmailRtb" runat="server" MaxLength="50" />
                    </td>
                    <td colspan="3"></td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" Text="Treated" />
                    </td>
                    <td style="padding-left: 5px;">
                        <telerik:RadComboBox runat="server" ID="TreatedRcb" Width="100px" />
                    </td>
                    <td colspan="4" style="padding-left: 10px;">
                        <asp:Label runat="server" Text="Assignment date from" />
                        <telerik:RadDatePicker ID="DateFromRdp" runat="server" />
                        <asp:Label runat="server" Text="to" />
                        <telerik:RadDatePicker ID="DateToRdp" runat="server" />
                    </td>
                    <td></td>
                </tr>
            </table>

            <asp:Label ID="Label" runat="server" />

            <telerik:RadGrid ID="NotifMgtRg" runat="server" RenderMode="Lightweight" AllowPaging="True" AllowSorting="true" AllowMultiRowSelection="true" AutoGenerateColumns="False"
                Width="80%" OnItemDataBound="NotifMgtRg_OnItemDataBound" OnDetailTableDataBind="NotifMgtRg_OnDetailTableDataBind" OnPageIndexChanged="NotifMgtRg_PageIndexChanged"
                OnPageSizeChanged="NotifMgtRg_PageSizeChanged" OnItemCreated="NotifMgtRg_ItemCreated">
                <PagerStyle Mode="NextPrevAndNumeric" />
                <MasterTableView DataKeyNames="ObjectId, ObjectLbl, Treated, TreatedDate, UseDateRangeForPublishing, PublishedFrom, PublishedTo, Visibility" Name="Assignments">
                    <Columns>
                        <telerik:GridClientSelectColumn UniqueName="Select" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                            ItemStyle-CssClass="select-item" HeaderStyle-Width="2%" />
                        <telerik:GridBoundColumn DataField="ObjectTypeLbl" SortExpression="ObjectTypeLbl" HeaderText="Object Type" HeaderStyle-Width="13%" />
                        <telerik:GridBoundColumn DataField="ObjectLbl" SortExpression="ObjectLbl" HeaderText="Object Name/Id" HeaderStyle-Width="25%" />
                        <%--<telerik:GridBoundColumn DataField="AssignmentTypeLbl" SortExpression="AssignmentTypeLbl" HeaderText="Assignment Type" HeaderStyle-Width="15%" />--%>
                        <telerik:GridBoundColumn DataField="Visibility" SortExpression="Visibility" HeaderText="Visibility" HeaderStyle-Width="10%" />
                        <telerik:GridBoundColumn DataField="PublishedFrom" SortExpression="PublishedFrom" HeaderText="Published From" HeaderStyle-Width="13%"
                            ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                        <telerik:GridBoundColumn DataField="PublishedTo" SortExpression="PublishedTo" HeaderText="Published To" HeaderStyle-Width="13%"
                            ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                        <telerik:GridCheckBoxColumn DataField="Treated" HeaderText="Treated" UniqueName="Treated" SortExpression="Treated"
                            ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="10%" />
                        <telerik:GridBoundColumn DataField="TreatedDate" SortExpression="TreatedDate" HeaderText="Treatment Date" HeaderStyle-Width="13%"
                            ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                    </Columns>
                    <DetailTables>
                        <telerik:GridTableView runat="server" DataKeyNames="C_GLOBALID" Name="Customers" Width="70%">
                            <Columns>
                                <telerik:GridBoundColumn DataField="C_CUID" SortExpression="C_CUID" HeaderText="Customer code" />
                                <telerik:GridBoundColumn DataField="C_NAME" SortExpression="C_NAME" HeaderText="Customer name" />
                                <telerik:GridBoundColumn DataField="CreationDate" SortExpression="CreationDate" HeaderText="Assignment Date" />
                            </Columns>
                            <DetailTables>
                                <telerik:GridTableView runat="server" Name="Users" Width="90%">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="U_FIRSTNAME" SortExpression="U_FIRSTNAME" HeaderText="First name" HeaderStyle-Width="20%" />
                                        <telerik:GridBoundColumn DataField="U_LASTNAME" SortExpression="U_LASTNAME" HeaderText="Last name" HeaderStyle-Width="20%" />
                                        <telerik:GridBoundColumn DataField="U_EMAIL_LOGIN" SortExpression="U_EMAIL_LOGIN" HeaderText="Email" HeaderStyle-Width="60%" />
                                    </Columns>
                                </telerik:GridTableView>
                            </DetailTables>
                        </telerik:GridTableView>
                    </DetailTables>
                </MasterTableView>
                <ClientSettings>
                    <Selecting AllowRowSelect="True" />
                </ClientSettings>
            </telerik:RadGrid>

            <asp:Button ID="SendNotificationBtn" runat="server" type="button" UseSubmitBehavior="false" class="btn bleu rounded" OnClientClick="ProcessButton(this)" OnClick="SendNotificationBtn_Click" Text="Send Notification" />

        </ContentTemplate>
    </asp:UpdatePanel>

    <telerik:RadWindow ID="InfoRw" runat="server" CssClass="radwindow-popup" Modal="true" VisibleOnPageLoad="false" Behaviors="Close" Title="Information" Width="300" Height="200px">
        <ContentTemplate>
            <div class="padding14px">
                <table style="width: 100%" class="popupTable">
                    <tr>
                        <td>
                            <asp:Label ID="InfoLbl" runat="server" CssClass="Electrolux_light_bold" />
                        </td>
                    </tr>
                    <tr style="text-align: center;">
                        <td>
                            <asp:Button ID="InfoBtn" runat="server" Text="OK" UseSubmitBehavior="false" CssClass="btn btnCustom" OnClientClick="CloseWindow(this)" />
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow ID="ConfirmationRw" runat="server" CssClass="radwindow-popup" Modal="true" VisibleOnPageLoad="false" Behaviors="Close" Title="Confirmation" Width="400" Height="200px">
        <ContentTemplate>
            <div class="padding14px">
                <table style="width: 100%" class="popupTable">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="ConfirmationLbl" runat="server" CssClass="Electrolux_light_bold" />
                        </td>
                    </tr>
                    <tr style="text-align: center;">
                        <td>
                            <asp:Button ID="NoBtn" runat="server" Text="Cancel" UseSubmitBehavior="false" CssClass="btn red" OnClientClick="CloseWindow(this)" />
                        </td>
                        <td>
                            <asp:Button ID="ConfirmBtn" runat="server" Text="Confirm" UseSubmitBehavior="false" CssClass="btn green" OnClick="ConfirmBtn_Click" />
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </telerik:RadWindow>
</asp:Content>
