<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MessageRequesterControl.ascx.vb" Inherits="UserControls_MessageRequesterControl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<script type="text/javascript">
    var editor;
    $(document).ready(function () {
        CreateXmlFormat();
    });

    function CreateXmlFormat() {
        var divRequest = document.getElementById("<%= MessageXML.ClientID %>");
        editor = CodeMirror.fromTextArea(divRequest, {
            mode: "xml",
            lineNumbers: true
        });

        editor.on('blur', function () {
            editor.save();
        });
    }

    function ClearRequest() {
        if (editor) {
            editor.setValue("");
        }
    }

    function chkBoxViaHybrisInerfaceChange() {
        if ($(".chkBoxViaHybrisInerface")[0].checked) {
            $(".HybrisInterface").removeClass("hidden");
        }
        else {
            $(".HybrisInterface").addClass("hidden");
        }
    }

    function RenderRefresh() {
        $('#<%= BtnSendRequestToSap.ClientID %>').addClass("loadingBackground").html("Sending..").prop('disabled', true);
        return false;
    }

    function SendToSapFinish() {
        $('#<%= BtnSendRequestToSap.ClientID %>').removeClass("loadingBackground").html("Confirm").prop('disabled', false);
        ShowOrCloseWindow("SendToSAP", false);
        ShowOrCloseWindow("SendToSAPInfo", true);
        CreateXmlFormat();
        return false;
    }

    function ShowOrCloseWindow(windowIdentifier, Show) {
        var oWnd = null;
        switch (windowIdentifier) {
            case "SendToSAP":
                oWnd = $find("<%= WindowSendToSAP.ClientID %>");
                if (oWnd != null) {
                    if (Show) {
                        oWnd.show();
                    }
                    else {
                        oWnd.close();
                    }
                }
                break;
            case "SendToSAPInfo":
                $("#dialog-send-to-sap-info").dialog({
                    resizable: false,
                    height: "auto",
                    width: 550,
                    modal: true
                });

                if (!Show) {
                    $('.ui-dialog-content:visible').dialog('close');
                    $("#dialog-send-to-sap-info").dialog('close');
                }            
                break;
        }    
        
        return false;
    }
</script>
<asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
    <ContentTemplate>
        <table class="Filters">
            <tr>
                <td class="width130px">
                    <asp:Label runat="server" ID="lblApplicationName" CssClass="Electrolux_light_bold Electrolux_Color width130px">Request Type:</asp:Label>
                </td>
                <td class="width180px">
                    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlMessageType_SelectedIndexChanged" AutoPostBack="true" ID="ddlMessageType">
                    </asp:DropDownList>
                </td>
                <td class="width120px">
                    <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
                </td>
                <td class="width180px">
                    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlEnvironment_SelectedIndexChanged" ID="ddlEnvironment">
                    </asp:DropDownList>
                </td>
                <td>
                    <input class="btn blue" type="button" value="Send" onclick="ShowOrCloseWindow('SendToSAP', true)" />
                    <input class="btn red" type="button" value="Clear" onclick="ClearRequest()" />
                </td>
            </tr>
            <tr>
                <td class="width120px">
                    <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country (SOP):</asp:Label>
                </td>
                <td class="width180px">
                    <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="true" ID="ddlCountry">
                        <Items>
                            <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
                        </Items>
                    </telerik:RadComboBox>
                    <asp:Label runat="server" ID="lblCountrySOP">As per the request</asp:Label>
                </td>
                <td class="width120px">
                    <asp:Label runat="server" ID="lblWcfB2BServiceUrl" CssClass="Electrolux_light_bold Electrolux_Color">Service URL:</asp:Label>
                </td>
                <td colspan="2">
                    <asp:TextBox runat="server" ID="wcfB2BWebServiceUrl" Width="375"></asp:TextBox>
                </td>
            </tr>
            <tr runat="server" id="trViaHybrisInterface">
                <td colspan="2">
                    <asp:Label runat="server" ID="Label1" CssClass="Electrolux_light_bold Electrolux_Color">Via Hybris Interface:</asp:Label>
                    <label class="switch">
                        <input runat="server" id="chkBoxViaHybrisInerface" checked="checked" class="chkBoxViaHybrisInerface" onchange="chkBoxViaHybrisInerfaceChange()" type="checkbox" />
                        <span class="slider round"></span>
                    </label>
                </td>
                <td class="width120px HybrisInterface">
                    <asp:Label runat="server" ID="lblHybrisMethodName" CssClass="Electrolux_light_bold Electrolux_Color">Hybris Method:</asp:Label>
                </td>
                <td colspan="2" class="HybrisInterface">
                    <asp:TextBox runat="server" ID="txtBoxHybrisMethodName" Width="375"></asp:TextBox>
                </td>
            </tr>
            <tr runat="server" id="trHybrisUserdetails">
                <td class="width130px HybrisInterface">
                    <asp:Label runat="server" ID="Label3" CssClass="Electrolux_light_bold Electrolux_Color width130px">Hybris Username:</asp:Label>
                </td>
                <td class="width180px HybrisInterface">
                    <asp:TextBox runat="server" ID="txtboxHybrisUserName" Width="180"></asp:TextBox>
                </td>
                <td class="width120px HybrisInterface">
                    <asp:Label runat="server" ID="Label4" CssClass="Electrolux_light_bold Electrolux_Color">Hybris Password:</asp:Label>
                </td>
                <td class="width180px HybrisInterface">
                    <asp:TextBox runat="server" ID="txtBoxHybrisPassword" Width="180"></asp:TextBox>
                </td>
            </tr>
        </table>
        <div id="dialog-send-to-sap-info" title="Send Message Information" class="DisplayNone">
            <div id="divSendToSapInfo" runat="server"></div>
            <div id="divXMLReplySAP" runat="server" style="max-height:250px;overflow:auto"></div>
            <table align="right">
                <tr>
                    <td>
                        <button class="btn bleu" id="btnViewRequestReplyXML" visible="false" runat="server">View Request/Reply XML</button>
                    </td>
                    <td>
                        <button class="btn green" onclick="ShowOrCloseWindow('SendToSAPInfo',false)">Ok</button>
                    </td>
                </tr>
            </table>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
<textarea runat="server" placeholder="Put your request here..." id="MessageXML"></textarea>


<telerik:RadWindow ID="WindowSendToSAP" runat="server" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" DestroyOnClose="false" VisibleStatusbar="false" Title="Send Request to SAP" Behaviors="Close" Width="400" Height="150px">
    <ContentTemplate>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <h4>&nbsp;Are you sure you want to send this request to SAP?</h4>
                <table align="right">
                    <tr>
                        <td>
                            <button class="btn red" id="BtnCancelSendRequestToSap" onclick="ShowOrCloseWindow('SendToSAP',false)">Cancel</button>
                        </td>
                        <td>
                            <asp:LinkButton class="btn green" ID="BtnSendRequestToSap" runat="server" OnClientClick="RenderRefresh()">Confirm</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="BtnSendRequestToSap" />
            </Triggers>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

