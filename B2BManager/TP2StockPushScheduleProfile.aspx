<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="TP2StockPushScheduleProfile.aspx.vb" Inherits="TP2StockPushScheduleProfile" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
     <link href="CSS/jquery-ui.css?v=1.1" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-ui.js?v=1.1"></script>
    <style type="text/css">
        .ui-autocomplete-loading {
            background: url("Images/Loading.gif") right center no-repeat;
            background-size: 18px;
        }

        .ui-autocomplete {
            max-height: 250px;
            overflow-y: auto;
            /* prevent horizontal scrollbar */
            overflow-x: hidden;
        }
    </style>
    <script type="text/javascript">
        var cache = {};
        $(function () {
            BindAutoComplete();
        });



        function CloseWindow() {
            if (typeof window.parent.CloseWindow == "function") {
                window.parent.CloseWindow();
            }
        }

        function getParameterByName(name, url = window.location.href) {
            name = name.replace(/[\[\]]/g, '\\$&');
            var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, ' '));
        }

        function ProcessButton(sender) {
            if ($("#<%= txtBoxTradeplaceID.ClientID %>").val().length == 0) {
                return;
            }
            if ($("#<%= txtBoxCustomerCode.ClientID %>").val().length == 0) {
                return;
            }
            switch (sender) {
                case "Add": {
                    $('#<%= btnSubmit.ClientID%>').addClass("loadingBackground").html("Submitting..").prop('disabled', true);
                    break;
                }
                case "Update": {
                    $('#<%= btnSubmit.ClientID%>').addClass("loadingBackground").html("Updating..").prop('disabled', true);
                    break;
                }
            }

            return false;
        }

        

        function BindAutoComplete() {
            $( "#<%= txtBoxTradeplaceID.ClientID %>").autocomplete({
                source: function (request, response) {
                    var term = request.term;
                    $('#<%=  statusInfo.ClientID %>').html(" ");
                    $("#<%= lblErrorInfo.ClientID%>").text("");
                    if (term in cache) {
                        response(cache[term]);
                        return;
                    }
                    $.getJSON("B2BManagerService.svc/GetTradeplaceID", { term: request.term, Envid: getParameterByName("envid") }, function (data, status, xhr) {
                        cache[term] = data;
                        response(data);
                    });
                },
                minLength: 3,
                select: function (event, ui) {
                    $('#<%=  statusInfo.ClientID %>').html("<span style='font-size:9pt'>" + ui.item.id + "</span> <img src='images/ok.gif' />");
                }
            });
        }

        function ResponseComplete() {
            BindAutoComplete();
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
        <ClientEvents OnResponseEnd="ResponseComplete" />
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnSubmit"></telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <asp:UpdatePanel ID="updatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="2" style="margin: 15px; width: 480px" align="center">
               <tr valign="top" runat="server" id="TradeplaceIDTR">
                    <td class="width150px">
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Tradeplace ID (*):</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBoxTradeplaceID" CssClass="Electrolux_light width230px ui-widget" runat="server"></asp:TextBox>
                        <div runat="server" id="statusInfo"></div>
                    </td>
                    <td style="text-align: left">
                        <asp:RequiredFieldValidator runat="server" ID="ReqtxtTradeplaceID" ControlToValidate="txtBoxTradeplaceID" ForeColor="Red" ErrorMessage="* mondatory" />
                    </td>
                </tr>
                 <tr valign="top" runat="server" id="Tr1">
                    <td class="width150px">
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Customer Code (*):</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBoxCustomerCode" CssClass="Electrolux_light width230px ui-widget" runat="server"></asp:TextBox>
                    </td>
                    <td style="text-align: left">
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtBoxCustomerCode" ForeColor="Red" ErrorMessage="* mondatory" />
                    </td>
                </tr>
                <tr runat="server" id="CountryTR">
                    <td>
                        <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
                    </td>
                    <td class="width180px">
                        <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="false" ID="ddlCountry">
                        </telerik:RadComboBox>
                    </td>
                </tr>
                 <tr valign="top" runat="server" id="Tr2">
                    <td >
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Schedule Time:</asp:Label>
                    </td>
                    <td>
                        <telerik:RadTimePicker runat="server" ID="ScheduleTime"></telerik:RadTimePicker>
                        <asp:CheckBox runat="server" ID="applyTimeChangeToSchedule" Visible="false" />
                    </td>
                </tr> 
                 <tr valign="top" runat="server" id="Tr3">
                    <td>
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Schedule Active:</asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkBoxScheduleActive"/>
                    </td>
                </tr> 
                <tr>
                    <td align="center" colspan="3">
                        <asp:Label runat="server" ID="lblErrorInfo" ForeColor="Red" Text=" "></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="ButtonsTR">
                    <td colspan="3" align="center">
                        <asp:LinkButton runat="server" CssClass="btn red" ID="btnCancel" CausesValidation="false" OnClientClick="CloseWindow()">Cancel</asp:LinkButton>
                        <asp:LinkButton runat="server" ID="btnSubmit" class="btn bleu" Text="Submit changes" OnClick="btnSubmit_Click"></asp:LinkButton>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSubmit" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

