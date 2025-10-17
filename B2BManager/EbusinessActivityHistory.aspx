<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessActivityHistory.aspx.vb" Inherits="EbusinessActivityHistory" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script type="text/javascript">
        function ActionTypeIndexChanged(sender, args) {
            var tableView = $find("<%= gridSearch.ClientID %>_ctl00");
            tableView.filter("ActionType", args.get_item().get_value(), "Contains");
        }
    </script>
    <style type="text/css">
        .type-B2B-Action {
            background-color: #639db9;
        }

        .type-B2B-Request {
            background-color: #7d8483;
        }

        .type-TP2-Request{
            background-color:#9D3A50;
        }

        .type-B2B-Action, .type-B2B-Request, .type-TP2-Request {
            display: ruby;
            text-align: center;
            line-height: 20px;
            width: 100px;
            text-align: center;
        }

        .LogGridSearch {
            width: 100%;
            overflow-x: auto;
        }

        .RadGrid_Default .rgFilterRow .rcbReadOnly td {
            border: none !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ID="uniqueGeneratedKey" runat="server" />
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <table class="Filters">
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblFrom" CssClass="Electrolux_light_bold Electrolux_Color">From:</asp:Label>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker ID="RadDateTimePickerFrom" DateInput-DateFormat="dd/MM/yyyy" TimeView-TimeFormat="HH:mm" runat="server">
                            <Calendar runat="server">
                                <SpecialDays>
                                    <telerik:RadCalendarDay Repeatable="Today">
                                        <ItemStyle CssClass="rcToday" />
                                    </telerik:RadCalendarDay>
                                </SpecialDays>
                            </Calendar>
                        </telerik:RadDateTimePicker>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblTo" CssClass="Electrolux_light_bold Electrolux_Color">To:</asp:Label>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker ID="RadDateTimePickerTo" DateInput-DateFormat="dd/MM/yyyy" TimeView-TimeFormat="HH:mm" runat="server">
                            <Calendar runat="server">
                                <SpecialDays>
                                    <telerik:RadCalendarDay Repeatable="Today">
                                        <ItemStyle CssClass="rcToday" />
                                    </telerik:RadCalendarDay>
                                </SpecialDays>
                            </Calendar>
                        </telerik:RadDateTimePicker>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <asp:LinkButton runat="server" ID="btnSearch" CssClass="btn bleu" OnClientClick="BeginSearch()" OnClick="btnSearch_Click"><i class="fas fa-search"></i> Search</asp:LinkButton>
                                    <asp:LinkButton runat="server" ID="btnExcel" CssClass="btn green" OnClick="btnExcel_Click"><i class="far fa-file-excel"></i> Export</asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <asp:Panel runat="server" ID="gridPanel" CssClass="hidden">
                <telerik:RadGrid runat="server" ID="gridSearch" CssClass="LogGridSearch" EnableAjaxSkinRendering="true" AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true" PageSize="10" GroupingEnabled="true">
                    <MasterTableView AutoGenerateColumns="false" TableLayout="Auto">
                        <Columns>
                            <telerik:GridTemplateColumn DataField="HasError" AllowFiltering="false" UniqueName="HasError" HeaderText="" HeaderStyle-Width="28">
                                <ItemTemplate>
                                    <img src='Images/<%# Eval("ActionStatus").ToString()%>.png' class="<%# IIf(Eval("ActionStatus").ToString() <> "", "MoreInfoImg", "hidden")%>" width="18" height="18" title="<%# Eval("ActionStatus") %>" /><img src='Images/XML.png' width="20" class="<%# IIf(Eval("Correl_ID").ToString().ToLower() <> "", "MoreInfoImg", "hidden")%>" height="20" title="View XML File"
                                        onclick="OpenViewXMLFilesWindow(<%# "'1','" + Eval("ActionName") + "','" + Eval("EnvironmentID").ToString() + "','" + Eval("CORREL_ID").ToString() + "','" + Eval("TABLE_NAME") + "','" + Eval("SOP_ID") + "','','" + Eval("ID").ToString() + "'" %>)" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="ID" UniqueName="ID" HeaderText="ID" Visible="true"></telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn DataField="ActionType" UniqueName="ActionType" HeaderText="Type" ItemStyle-Width="130" Visible="true">
                                <FilterTemplate>
                                    <telerik:RadComboBox runat="server" ID="radComboBoxFilter" SelectedValue='<%# TryCast(Container, GridItem).OwnerTableView.GetColumn("ActionType").CurrentFilterValue %>'
                                        OnClientSelectedIndexChanged="ActionTypeIndexChanged">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="All" />
                                            <telerik:RadComboBoxItem Value="Request" Text="B2B Request" />
                                            <telerik:RadComboBoxItem Value="Request" Text="TP2 Request" />
                                            <telerik:RadComboBoxItem Value="Action" Text="B2B Action" />                                            
                                        </Items>
                                    </telerik:RadComboBox>
                                </FilterTemplate>
                                <ItemTemplate>
                                    <span class='badge <%# "type-" + Eval("ActionType").ToString().Replace(" ", "-") %>'><%# Eval("ActionType") %></span>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="Customer_ID" UniqueName="Customer_ID" HeaderText="Customer Code" Visible="true"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="WebUserID" UniqueName="WebUserID" HeaderText="Login" Visible="true"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="ActionName" UniqueName="ActionName" HeaderText="Action Name" Visible="true"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="SubAction" UniqueName="SubAction" HeaderText="Sub-action" Visible="true"></telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn DataField="A_SUB_ACTION" UniqueName="A_SUB_ACTION" HeaderText="Extra Details / Correl ID" Visible="true">
                                <ItemTemplate>
                                    <span><%# IIf(Eval("ActionType") = "B2B Request", Eval("Correl_ID"), Trim(Eval("A_SUB_ACTION"))) %></span>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="A_Comment" UniqueName="A_Comment" HeaderText="Comment" Visible="true"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="LoggedOn" UniqueName="LoggedOn" HeaderText="Logged On" Visible="true"></telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </asp:Panel>

            <asp:SqlDataSource runat="server" ID="SqlDataSource1" OnSelecting="SqlDataSource1_Selecting" EnableCaching="true" SelectCommand="[Ebusiness].[UsrMgmt_GetActivity]" SelectCommandType="StoredProcedure" ConnectionString="<%$ ConnectionStrings:LogDB %>" CancelSelectOnNullParameter="false">
                <SelectParameters>
                    <asp:QueryStringParameter QueryStringField="envid" Name="EnvironmentID" DbType="Int16" />
                    <asp:QueryStringParameter QueryStringField="cid" Name="CID" DbType="Guid" />
                    <asp:QueryStringParameter QueryStringField="uid" Name="UID" DbType="Guid" />
                    <asp:QueryStringParameter QueryStringField="sopid" Name="SOPID" DbType="String" />
                    <asp:ControlParameter ControlID="RadDateTimePickerFrom" Name="From" DbType="DateTime" />
                    <asp:ControlParameter ControlID="RadDateTimePickerTo" Name="To" DbType="DateTime" />
                </SelectParameters>
            </asp:SqlDataSource>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSearch" />
            <asp:PostBackTrigger ControlID="btnExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

