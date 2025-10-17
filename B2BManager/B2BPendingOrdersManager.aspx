<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="B2BPendingOrdersManager.aspx.vb" Inherits="B2BPendingOrdersManager" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">

    <script type="text/javascript">
        function OpenDeleteWindow(Correl_ID) {
            var oWnd = $find("<%= WindowDeleteOrder.ClientID %>");
            var environmentID = $("#<%= selectedEnvironmentID.ClientID %>").val();
            oWnd.setUrl("B2BPendingOrdersDeleteOrder.aspx?HideHeader=true&Correl_ID=" + Correl_ID.toString() + "&EnvironmentID=" + environmentID);
            oWnd.show();
        }
        function OpenSAPWindow(Correl_ID,U_GLOBALID,SOP) {
            var oWnd = $find("<%= WindowSendToSAP.ClientID %>");
            var environmentID = $("#<%= selectedEnvironmentID.ClientID %>").val();
            oWnd.setUrl("B2BPendingOrdersResend.aspx?HideHeader=true&Correl_ID=" + Correl_ID.toString() + "&SopID=" + SOP+ "&GlobalID=" + U_GLOBALID+ "&EnvironmentID=" + environmentID);            
            oWnd.show();
            
        }
        function ShowAndRefreshGrid(Window) {
            CloseWindow(Window);
            $('*[id$="btnSearch"]').addClass("loadingBackground").text("Searching...").val("Searching...").prop('disabled', true);
            $('#ContentPlaceHolder1_DL_Actions').val('0')
            __doPostBack('ctl00$ContentPlaceHolder1$btnSearch', '');
        }
        function CloseWindow(Window) {
            if (Window == 'WindowSendToSAP'){
                var oWnd = $find("<%= WindowSendToSAP.ClientID %>");
            } else {
                var oWnd = $find("<%= WindowDeleteOrder.ClientID %>");
            }
            oWnd.close();
        }
        function changeCursor(ddl) {
            var grid = $find("<%= b2bOrdersGrid.ClientID %>");
            var selectedItems = grid.get_masterTableView().get_selectedItems();
            var environmentID = $("#<%= selectedEnvironmentID.ClientID %>").val();

            if (selectedItems.length > 0) {
                var Items = [];
                for (var i = 0; i < selectedItems.length; i++) {
                    Items.push({
                        Correl_ID: selectedItems[i].getDataKeyValue("Correl_ID"),
                        SOP: selectedItems[i].getDataKeyValue("SOP"),
                        U_GLOBALID: selectedItems[i].getDataKeyValue("U_GLOBALID")

                    })
                }
                if ($(ddl).val() == 1) {
                    var oWnd = $find("<%= WindowDeleteOrder.ClientID %>");
                    oWnd.setUrl("B2BPendingOrdersDeleteOrder.aspx?HideHeader=true&List=" + JSON.stringify(Items) + "&EnvironmentID=" + environmentID);
                    oWnd.show();
                } else if ($(ddl).val() == 2) {
                    var oWnd = $find("<%= WindowSendToSAP.ClientID %>");
                    oWnd.setUrl("B2BPendingOrdersResend.aspx?HideHeader=true&List=" + JSON.stringify(Items) + "&EnvironmentID=" + environmentID);
                    oWnd.show();
                }
            } else {
                if ($(ddl).val() != 0) {
                alert('Please  select at least one order');
                    $(ddl).val('0')
                    }
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="selectedEnvironmentID" />
            <table class="Filters">
                <tr>
                    <td class="width120px">
                        <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
                    </td>
                    <td class="width180px">
                        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlEnvironment" DataTextField="Name" DataValueField="ID">
                        </asp:DropDownList>
                    </td>
                    <td class="width120px">
                        <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
                    </td>
                    <td class="width180px">
                        <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width230px" AppendDataBoundItems="true" ID="ddlCountry">
                            <Items>
                                <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
                            </Items>
                        </telerik:RadComboBox>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblActionDetails" CssClass="Electrolux_light_bold Electrolux_Color">Free text search:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtBoxSearchInDetails" CssClass="Electrolux_light_bold Electrolux_Color width120px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:LinkButton runat="server" ID="btnSearch" CssClass="btn bleu" OnClientClick="BeginSearch()" OnClick="btnSearch_Click" Text="Search" />
                    </td>
                </tr>
            </table>
            <div style="margin:5px">
               <asp:DropDownList runat="server" ID="DL_Actions"  onchange="changeCursor(this)">
                    <asp:ListItem Text="Actions on selected rows.." Value="0"></asp:ListItem>
                    <asp:ListItem Text="Delete" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Resend" Value="2"></asp:ListItem>
                </asp:DropDownList>
            </div>
                <telerik:RadToolTipManager RegisterWithScriptManager="true" rendermode="Lightweight" ID="RadToolTipManager1" OnClientBeforeShow="createTooltipContent" HideEvent="ManualClose" ShowEvent="OnMouseOver"
                runat="server" RelativeTo="Element" Position="MiddleRight">
            </telerik:RadToolTipManager>
            <telerik:RadGrid runat="server" MasterTableView-ShowHeadersWhenNoRecords="true" OnNeedDataSource="b2bOrdersGrid_NeedDataSource" OnItemDataBound="b2bOrdersGrid_ItemDataBound" ID="b2bOrdersGrid" AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true" PageSize="20" GroupingEnabled="true" AllowMultiRowSelection="true"  >
                <MasterTableView AutoGenerateColumns="false" ClientDataKeyNames="Correl_ID,SOP,U_GLOBALID" >
                    <Columns>
                        <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn">
                        </telerik:GridClientSelectColumn>
                        <telerik:GridTemplateColumn UniqueName="Actions" AllowFiltering="false" HeaderText="Actions">
                            <ItemTemplate>
                                <img src='Images/delete.png' width="20" class="MoreInfoImg" height="20" title="Delete Order" onclick="OpenDeleteWindow('<%# Eval("Correl_ID") %>')" />
                                <img src='Images/resend.png' width="20" class="MoreInfoImg" height="20" title="Resend Order" onclick="OpenSAPWindow('<%# Eval("Correl_ID") %>','<%# Eval("U_GLOBALID") %>','<%# Eval("SOP") %>')" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="SOP">
                            <ItemTemplate>
                                <img src='Images/Flags/<%# Eval("CY_NAME_ISOCODE").ToString() %>.png' width="20" height="16" title="<%# Eval("CY_NAME") %>" />
                                <span class="verticalAlignTop"><%# Eval("SOP") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="O_PERS_ORDER_ID" HeaderText="Customer Order ID">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="O_CREATED" HeaderText="Order Date">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="C_CUID" HeaderText="Customer Code">
                        </telerik:GridBoundColumn>
                         <telerik:GridBoundColumn  DataField="Resend"  Visible="false">
                        </telerik:GridBoundColumn>
                    </Columns>
                    <NoRecordsTemplate>
                        No data available.
                    </NoRecordsTemplate>

                </MasterTableView>
                <ClientSettings EnableAlternatingItems="false">
                    <Selecting AllowRowSelect="true"  />
                </ClientSettings>
            </telerik:RadGrid>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSearch" />
        </Triggers>
    </asp:UpdatePanel>

    <telerik:RadWindow ID="WindowDeleteOrder" runat="server" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" DestroyOnClose="false" ShowContentDuringLoad="false" VisibleStatusbar="false" Title="Delete Pending Order Confirmation" Behaviors="Close" Width="400" Height="200px">
    </telerik:RadWindow>
    <telerik:RadWindow ID="WindowSendToSAP" runat="server" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" DestroyOnClose="false" VisibleStatusbar="false" Title="Send Request to SAP" Behaviors="Close" Width="400" Height="150px">   
    </telerik:RadWindow>

</asp:Content>

