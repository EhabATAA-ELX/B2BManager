<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="TPSendMessage.aspx.vb" Inherits="TPSendMessage" validateRequest="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
    <link href="CSS/Emanager.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/CodeMirror/lib/codemirror.js?v=2"></script>
    <link href="Scripts/CodeMirror/lib/codemirror.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/CodeMirror/mode/xml/xml.js"></script>
    <script>
        var editor;
        $(document).ready(function () {
            CreateXmlFormat();
        });

        function CreateXmlFormat() {
            var divRequest = document.getElementById("ContentPlaceHolder1_RequestText");

              editor = CodeMirror(divRequest,{
	            mode:"xml",
	            lineNumbers:true	
                });
        }

        function SubmitForm() {
            ProcessButton("[id$='send']", "Sending...");
            if (editor.getValue() == "") {
                UndoProcessButton("[id$='send']", "Send");
                alert("Please insert a request.");
                return;
            }
            var prodChecked= $("[id$='prod']").is(":checked");
            if (prodChecked) {
                $find("<%= WindowSendToProd.ClientID %>").show();
                return;
            }
            SendRequest();
        }

        function CloseProdPopup() {
            UndoProcessButton("[id$='send']", "Send");
            $find("<%= WindowSendToProd.ClientID %>").close();
        }

        function SendProdRequest() {
            $find("<%= WindowSendToProd.ClientID %>").close();
            SendRequest();
        }

        function SendRequest() {
            var hubpsanRequest = $("[id$='hubspan']").is(":checked");
            PageMethods.GetResponse(editor.getValue(),$("[id$='url']").val(),hubpsanRequest,$("[id$='login']").val(),$("[id$='password']").val(),OnSuccess, OnFailure);
        }

        var editorresponse;
         function OnSuccess(response, userContext, methodName) {
             UndoProcessButton("[id$='send']", "Send");
             var divRequest = document.getElementById("Reply");
             if (editorresponse === undefined) {
                  editorresponse = CodeMirror(divRequest, {
                value:response,
	            mode:"xml",
	            lineNumbers:true
             });
             } else {
                 editorresponse.setValue(response);
             }
         }

        function OnFailure(error) {             
            UndoProcessButton("[id$='send']", "Send");
            if (editorresponse !== undefined) {                
                editorresponse.setValue("");
            }
            alert("Error:"+error._message);
        }

        function ClearValue() {
            if (editor !== undefined) {                
                editor.setValue("");
            }

            if (editorresponse !== undefined) {                
                editorresponse.setValue("");
            }
            
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="EnvironmentPanel" Class="DefinitionPanel" ChildrenAsTriggers="false">
        <ContentTemplate>  
        <table class="inlineBlock Height90px">
            <tr>
                <td>
                    <span class="Electrolux_Color Electrolux_light_bold">url</span>
                </td>
                <td>
                    <asp:TextBox ID="url" runat="server" ReadOnly="false" CssClass="width500px Electrolux_light_bold Electrolux_Color ">http://euws1462/TP2CompassV2/index.aspx</asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <span class="Electrolux_Color Electrolux_light_bold">Login</span>
                </td>
                <td>
                    <asp:TextBox ID="login" runat="server" CssClass="Electrolux_light_bold Electrolux_Color "></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <span class="Electrolux_Color Electrolux_light_bold">Password</span>
                </td>
                <td>
                    <asp:TextBox ID="password" runat="server" CssClass="Electrolux_light_bold Electrolux_Color "></asp:TextBox>
                </td>
            </tr>
        </table>    
             <fieldset class="fieldsetEnvironment">
    <legend class="Electrolux_Color Electrolux_light_bold">Environments</legend>
        <div class="inlineBlock">
            <div><asp:RadioButton ID="hubspan" runat="server" GroupName="envName" Text="Hubspan" OnCheckedChanged="Radios_CheckedChanged" AutoPostBack="true" CssClass="Electrolux_Color Electrolux_light_bold"/></div>
            <div><asp:RadioButton ID="Bili" runat="server" GroupName="envName" Text="BILI" Checked="true" AutoPostBack="true" OnCheckedChanged="Radios_CheckedChanged"  CssClass="Electrolux_Color Electrolux_light_bold"/></div>
        </div>
        <div class="inlineBlock">                       
             <div><asp:RadioButton ID="staging" runat="server" GroupName="env" Text="Staging" Checked="true" AutoPostBack="true" OnCheckedChanged="Radios_CheckedChanged" CssClass="Electrolux_Color Electrolux_light_bold"/></div>
             <div><asp:RadioButton ID="prod" runat="server" GroupName="env" Text="Prod" AutoPostBack="true" OnCheckedChanged="Radios_CheckedChanged" CssClass="Electrolux_Color Electrolux_light_bold"/></div>
        </div>
    </fieldset>
 </ContentTemplate>
         <Triggers>
                <asp:AsyncPostBackTrigger ControlID="hubspan" />
                <asp:AsyncPostBackTrigger ControlID="Bili" />
                <asp:AsyncPostBackTrigger ControlID="staging" />
                <asp:AsyncPostBackTrigger ControlID="prod" />
        </Triggers>
    </asp:UpdatePanel>
     <div>
            <div id="RequestText" runat="server" style="width:100%;height:300px;"></div>
        </div>
    <div class="TextAlignCenter">
        <input type="button" id="send" value="Send" onclick="SubmitForm();" class="btn bleu" />
        <input id="clear" type="button" value="Clear" onclick="ClearValue();" class="btn red"/>
    </div>    
    <div>
        <div id="Reply" style="width:100%;height:300px;"></div>
    </div>

    <telerik:RadWindow ID="WindowSendToProd" runat="server" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" DestroyOnClose="false" VisibleStatusbar="false" Title="Send Request to Production" Behaviors="Close" Width="400" Height="160px" OnClientClose="CloseProdPopup">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="UpdatePanel2">
                <ContentTemplate>
                    <h4>&nbsp;Are you sure you want to send this request to Production Environment ?</h4>
                    <table align="right">
                        <tr>
                            <td>
                                <button class="btn red" id="BtnCancelSendRequestToSap" onclick="CloseProdPopup();">No</button>
                            </td>
                            <td>
                                <asp:LinkButton class="btn green" id="BtnSendRequestToSap" runat="server" OnClientClick="SendProdRequest();">Yes</asp:LinkButton>
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
</asp:Content>

