<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessCustomerAddressList.aspx.vb" Inherits="EbusinessCustomerAddressList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            var rowsCount = 0;
            if ($("#ContentPlaceHolder1_ListView1_table1")[0] != undefined) {
                rowsCount = $("#ContentPlaceHolder1_ListView1_table1")[0].rows.length;
            }
            switch (rowsCount) {
                case 0: $(".rowSearch").hide(); $("#noAddressLabel").removeClass("hidden"); break;
                case 1: $(".rowSearch").hide(); break;
                default:
                    $(".rowSearch").removeClass("hidden");
                    $("#lblRowsCount").text("in total " + rowsCount.toString() + " addresses");
                    break;
            }

            var restrictAccess = <%= RestrictAccessDisplay().ToString().ToLower() %>;
            if (restrictAccess) {
                $(".restrictaccesslink").removeClass("hidden");
            }
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="rowSearch hidden" style="padding:5px">
        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color width130px" Text="Free text search"></asp:Label>
        <input type="text" placeholder="Type your text here" class="width180px" style="height: 26px" />
        <span class="Electrolux_light_bold Electrolux_Color" id="lblRowsCount"></span>
    </div>
    <span class="Electrolux_light_bold Electrolux_Color hidden" style="padding:5px" id="noAddressLabel">No address can be found</span>
    <div style="height:500px;overflow-y:auto;padding:5px">
        <asp:ListView runat="server" ID="ListView1"
            DataSourceID="SqlDataSource1">
            <LayoutTemplate>
                <table runat="server" id="table1" class="list-view" style="width: 100%" cellspacing="0" cellpading="0">
                    <tr runat="server" id="itemPlaceholder" style="width: 100%; display: inline-block;"></tr>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr runat="server" class="odd-row" data-cid='<%# Eval("CA_AddrKey") %>' style="width: 100%;">
                    <td runat="server" style="width: 48px">
                        <img src="Images/Ebusiness/CustomersManagement/address.png" />
                    </td>
                    <td runat="server" style="width: 200px">
                        <asp:Label ID="NameLabel1" runat="server" Height="24"
                            Text='<%#Eval("CA_AddrKey") %>' />
                        <br /> 
                        <label class="defaultLink restrictaccesslink hidden">Restrict access</label>
                    </td>
                    <td runat="server">
                        <asp:Label ID="Label1" runat="server" Height="24"
                            Text='<%#Eval("AddressData") %>' />
                    </td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr runat="server" class="even-row" data-cid='<%# Eval("CA_AddrKey") %>' style="width: 100%;">
                    <td runat="server" style="width: 48px">
                        <img src="Images/Ebusiness/CustomersManagement/address.png" />
                    </td>
                    <td runat="server" style="width: 200px">
                        <asp:Label ID="NameLabel1" runat="server" Height="24"
                            Text='<%#Eval("CA_AddrKey") %>' />
                        <br />
                        <label class="defaultLink restrictaccesslink hidden" >Restrict access</label>
                    </td>
                    <td runat="server">
                        <asp:Label ID="Label1" runat="server" Height="24"
                            Text='<%#Eval("AddressData") %>' />
                    </td>
                </tr>
            </AlternatingItemTemplate>
        </asp:ListView>
    </div>

    <asp:SqlDataSource runat="server" ID="SqlDataSource1" SelectCommand="[Ebusiness].[UsrMgmt_GetAddressesByCID]" SelectCommandType="StoredProcedure" ConnectionString="<%$ ConnectionStrings:LogDB %>">
        <SelectParameters>
            <asp:QueryStringParameter QueryStringField="envid" Name="EnvironmentID" DbType="Int16" />
            <asp:QueryStringParameter QueryStringField="cid" Name="CID" DbType="Guid" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>

