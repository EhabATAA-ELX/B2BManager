<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="TP2ManageMailing.aspx.vb" Inherits="TP2ManageMailing" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
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

        .checkboxlist input:checked + label {
            color: #041d4f;
            font-weight: bold
        }

        .checkboxlist input:not(:checked) + label {
            color: #bbb;
            font-weight: normal;
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
            var listType = parseInt(getParameterByName("type"));
            if (listType == 0) {
                if ($("#<%= txtBoxTradeplaceID.ClientID %>").val().length == 0) {
                    return;
                }
            }
            if ($("#<%= txtBoxEmail.ClientID %>").val().length == 0) {
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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
        <ClientEvents OnResponseEnd="BindAutoComplete" />
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnSubmit"></telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <asp:UpdatePanel ID="updatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="2" style="margin: 15px; width: 450px" align="center">
                <tr valign="top" runat="server" id="TradeplaceIDTR">
                    <td class="width120px">
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
                <tr runat="server" id="CountryTR">
                    <td>
                        <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
                    </td>
                    <td class="width180px">
                        <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="false" ID="ddlCountry">
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr valign="top" runat="server" id="emailTR">
                    <td>
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Email (*):</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBoxEmail" CssClass="Electrolux_light_bold width230px" runat="server"></asp:TextBox>
                    </td>
                    <td style="text-align: left">
                        <asp:RequiredFieldValidator runat="server" ID="rfv_txtBoxEmail" ControlToValidate="txtBoxEmail" ForeColor="Red" ErrorMessage="* mondatory" />
                        <asp:RegularExpressionValidator ID="rev_txtBoxEmail" runat="server" ControlToValidate="txtBoxEmail"
                            ForeColor="Red" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                            Display="Dynamic" ErrorMessage="Invalid Email" />
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Mailing Type (*):</asp:Label>
                    </td>
                    <td>
                        <asp:CheckBoxList ID="chkboxMailingTypes" CssClass="checkboxlist" CellPadding="2" AppendDataBoundItems="true" CellSpacing="2" RepeatColumns="1" RepeatDirection="Horizontal" TextAlign="Right" runat="server" Height="100">
                        </asp:CheckBoxList>
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

