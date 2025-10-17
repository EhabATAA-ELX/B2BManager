<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="SAPOrderConfigurations.aspx.vb" Inherits="SAPOrderConfigurations" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="Scripts/DataTables/datatables.min.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/DataTables/datatables.min.js"></script>
    <script type="text/javascript">
        var table;
        $(document).ready(function () {
            table = $('#example').DataTable({
                "ajax": "B2BManagerService.svc/GetSAPOrderConfigurations?EnvironmentID=" + $('#<%= ddlEnvironment.ClientID %>').val(),
                "bPaginate": false,
                "bInfo": false,
                "columns": [
                    { "data": "B2B_Order_Type" },
                    { "data": "Order_Reason" },
                    { "data": "SAP_Sales_Doc_Type" },
                    { "data": "Shipping_Condition" }
                ]
            });
        });
    </script>
    <style type="text/css">
        .dataTables_filter {
            float: left !important;
            margin-left: 5px;
            margin-bottom: 10px;
        }

         .dataTables_filter  {
            text-align: left;
            font-family: Electrolux_light;
            font-weight: bold;
            color: #041d4f;
        }

            .dataTables_filter input {
                float: right;
                width: 180px;
                margin-left: 50px !important;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table class="Filters no-print">
        <tr>
            <td>
                <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
            </td>
            <td>
                <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="true" ID="ddlEnvironment">
                </asp:DropDownList>
            </td>
        </tr>
    </table>
    <table style="width: 100%">
        <tr>
            <td align="center" class="verticalAlignTop">
                <asp:Panel runat="server" ID="panelVisualizeInfo">
                    <table id="example" class="display" style="width: 100%">
                        <thead>
                            <tr>
                                <th>B2B Order Type</th>
                                <th>SAP Sales Doc Type</th>
                                <th>Order Reason</th>
                                <th>Shipping Condition</th>
                            </tr>
                        </thead>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>

