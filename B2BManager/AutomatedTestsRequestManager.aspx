<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="AutomatedTestsRequestManager.aspx.vb" Inherits="AutomatedTestsRequestManager" ValidateRequest="false" EnableEventValidation="false" %>

<%@ Register Src="~/UserControls/MessageRequesterControl.ascx" TagPrefix="uc1" TagName="MessageRequesterControl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script type="text/javascript" src="Scripts/CodeMirror/lib/codemirror.js?v=2"></script>
    <link href="Scripts/CodeMirror/lib/codemirror.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/CodeMirror/mode/xml/xml.js"></script>
    <link href="CSS/jquery-ui.css" rel="stylesheet" />
    <link href="CSS/AutomatedTests.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <script src="Scripts/CodeMirror/placeholder.js"></script>
    <script type="text/javascript">
        function CloseTestStepWindows() {
            window.parent.ShowOrClosetestStepWindow('ManageTestStep', false);
            window.parent.ShowOrClosetestStepWindow('DeleteTestStep', false);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <table align="center" runat="server" id="DeleteTable" style="margin-top: 10px">
                <tr class="Height30px">
                    <td colspan="3" align="center" style="width: 380px; margin: 25px;">
                        <asp:Label runat="server" ID="lblDelete" Width="380" CssClass="Electrolux_light_bold Electrolux_Color"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="Filters" runat="server" id="tableDescription">  
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblRequestDescription" CssClass="Electrolux_light_bold Electrolux_Color">Request Description:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtBoxRequestDescription" Width="425"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <uc1:MessageRequesterControl runat="server" ID="MessageRequesterControl" Visible="false" />            
            <table width="100%">
                <tr>
                    <td align="center">
                        <asp:Label runat="server" ID="lblErrorMessageInfo" Width="380" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <input type="button" class="btn bleu" id="btnCancelDashboardProfile" value="Cancel" onclick="CloseTestStepWindows()" />
                        <asp:LinkButton runat="server" CssClass="btn green" ID="btnSaveOrUpdateTestStep" ClientIDMode="Static" Text="Save" OnClientClick="ValidateAndProcessButton(this, 'Saving...')" CausesValidation="true" ValidationGroup="StepDetails"></asp:LinkButton>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSaveOrUpdateTestStep" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
