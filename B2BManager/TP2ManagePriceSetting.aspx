<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="TP2ManagePriceSetting.aspx.vb" Inherits="TP2ManagePriceSetting" %>

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
        var cacheSalesOrg = {};
        var cacheSapFieldOrg = {};
        var cacheTP2FieldOrg = {};
        $(function () {
            BindSalesOrgAutoComplete();
            BindSapFieldAutoComplete();
            BindTP2FieldAutoComplete();
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
            if ($("#<%= txtBoxSalesOrg.ClientID %>").val().length == 0) {
                return;
            }
            if ($("#<%= txtBoxSAPField.ClientID %>").val().length == 0) {
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

        

        function BindSalesOrgAutoComplete() {
            $( "#<%= txtBoxSalesOrg.ClientID %>").autocomplete({
                source: function (request, response) {
                    var term = request.term;
                    $("#<%= lblErrorInfo.ClientID%>").text("");
                    if (term in cacheSalesOrg) {
                        response(cacheSalesOrg[term]);
                        return;
                    }
                    $.getJSON("B2BManagerService.svc/GetSalesOrgs", { term: request.term, Envid: getParameterByName("envid") }, function (data, status, xhr) {
                        cacheSalesOrg[term] = data;
                        response(data);
                    });
                },
                minLength: 1
            });
        }

        function BindSapFieldAutoComplete() {
            $( "#<%= txtBoxSAPField.ClientID %>").autocomplete({
                source: function (request, response) {
                    var term = request.term;
                    $("#<%= lblErrorInfo.ClientID%>").text("");
                    if (term in cacheSapFieldOrg) {
                        response(cacheSapFieldOrg[term]);
                        return;
                    }
                    $.getJSON("B2BManagerService.svc/GetSAPFields", { term: request.term, Envid: getParameterByName("envid") }, function (data, status, xhr) {
                        cacheSapFieldOrg[term] = data;
                        response(data);
                    });
                },
                minLength: 1
            });
        }

        function BindTP2FieldAutoComplete() {
            $( "#<%= txtBoxTP2Field.ClientID %>").autocomplete({
                source: function (request, response) {
                    var term = request.term;
                    $("#<%= lblErrorInfo.ClientID%>").text("");
                    if (term in cacheTP2FieldOrg) {
                        response(cacheTP2FieldOrg[term]);
                        return;
                    }
                    $.getJSON("B2BManagerService.svc/GetTP2Fields", { term: request.term, Envid: getParameterByName("envid") }, function (data, status, xhr) {
                        cacheTP2FieldOrg[term] = data;
                        response(data);
                    });
                },
                minLength: 1
            });
        }

        function ResponseComplete() {
            BindSalesOrgAutoComplete();
            BindSapFieldAutoComplete();
            BindTP2FieldAutoComplete();
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
                <tr valign="top" >
                    <td class="width180px">
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Sales Org (*):</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBoxSalesOrg" CssClass="Electrolux_light width230px ui-widget" runat="server"></asp:TextBox>
                    </td>
                    <td style="text-align: left" >
                        <asp:RequiredFieldValidator runat="server" ID="ReqtxtBoxSalesOrg" ControlToValidate="txtBoxSalesOrg" ForeColor="Red" ErrorMessage="* required" />
                    </td>
                </tr>
                <tr valign="top" >
                    <td class="width180px">
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">SAP Field (*):</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBoxSAPField" CssClass="Electrolux_light width230px ui-widget" runat="server"></asp:TextBox>
                    </td>
                    <td style="text-align: left">
                        <asp:RequiredFieldValidator runat="server" ID="ReqtxtBoxSAPField" ControlToValidate="txtBoxSAPField" ForeColor="Red" ErrorMessage="* required" />
                    </td>
                </tr>
                <tr valign="top" >
                    <td class="width180px">
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">TP2 Field:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBoxTP2Field" CssClass="Electrolux_light width230px ui-widget" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr valign="top" >
                    <td class="width180px">
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Step Number:</asp:Label>
                    </td>
                    <td>
                        <telerik:RadNumericTextBox runat="server" MinValue="1" ShowSpinButtons="true" DataType="Integer" MaxValue="10000" ID="txtStepNumber" CssClass="Electrolux_light_bold Electrolux_Color">
                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                        </telerik:RadNumericTextBox>
                    </td>                    
                </tr>
                <tr valign="top" >
                    <td class="width180px">
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Code:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBoxCode" CssClass="Electrolux_light width230px ui-widget" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr valign="top" >
                    <td class="width180px">
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Monetory Amount:</asp:Label>
                    </td>
                    <td>
                       <asp:CheckBox runat="server" ID="ChkBoxMonetoryAmount" />
                    </td>
                </tr>
                <tr valign="top" >
                    <td class="width180px">
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Allowances & Charges:</asp:Label>
                    </td>
                    <td>
                       <asp:CheckBox runat="server" ID="ChkBoxAllowancesCharges" />
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

