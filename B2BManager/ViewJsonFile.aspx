<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="ViewJsonFile.aspx.vb" Inherits="ViewJsonFile" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/JsonPageView.ascx" TagName="WebUserControl" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script src="Scripts/jquery-3.2.1.js"></script>
    <script src="Scripts/Global.js"></script>
    <script src="Scripts/clipboard.min.js"></script>
    <script type="text/javascript">
        function ShowOrCloseWindow(windowIdentifier,Show) {
            var oWnd = null;
            switch (windowIdentifier) {
                 case "SendToScoopOS":
                     oWnd = $find("<%= WindowSendToScoopOS.ClientID %>");
                    break;
                case "SendToScoopOSInfo":
                     oWnd = $find("<%= WindowSendToScoopOSInfo.ClientID %>");
                     break;
             }         
            if (oWnd != null) {
                if (Show) {
                    oWnd.show();
                }
                else {
                    oWnd.close();
                }
            }
            return false;
        }
        function RenderRefresh() {
            $('#<%= BtnSendRequestToScoopOS.ClientID %>').addClass("loadingBackground").html("Sending..").prop('disabled', true);
            return false;
        }

        function SendToScoopOSFinish() {
            $('#<%= BtnSendRequestToScoopOS.ClientID %>').removeClass("loadingBackground").html("Confirm").prop('disabled', false);
            ShowOrCloseWindow("SendToScoopOS", false);
            ShowOrCloseWindow("SendToScoopOSInfo", true);
              return false;
          }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
              <telerik:RadTabStrip runat="server" ID="RadTabStrip1" MultiPageID="RadMultiPage1" SelectedIndex="0">
                <Tabs>
                    <telerik:RadTab Text="Request" Width="200px"></telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
             <telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0" CssClass="outerMultiPage">
                <telerik:RadPageView runat="server" ID="RadPageView1">
                                <div id="divJSONFile" runat="server" class="XMLFile"></div>
            <button class="btn bleu" id="BtnCopyJSON" runat="server" data-clipboard-action="copy" data-clipboard-target="#ContentPlaceHolder1_divJSONFile">Copy JSON</button>
            <button class="btn bleu" id="BtnViewJSONInBrowser" runat="server">View JSON In Browser</button>
            <button class="btn bleu" id="BtnDownloadJSON" runat="server">Download JSON</button>
                     <span runat="server" id="SpanConfirmSendRequestToScoopOS">
                        <button class="btn bleu" id="BtnConfirmSendRequestToScoopOS" onclick="ShowOrCloseWindow('SendToScoopOS',true)">Send to ScoopOS</button>
                    </span>
                </telerik:RadPageView>
                    </telerik:RadMultiPage>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="BtnCopyJSON" />
            <asp:AsyncPostBackTrigger ControlID="BtnViewJSONInBrowser" />
            <asp:AsyncPostBackTrigger ControlID="BtnDownloadJSON" />
        </Triggers>
    </asp:UpdatePanel>
    <telerik:RadWindow ID="WindowSendToScoopOS" runat="server" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" DestroyOnClose="false" VisibleStatusbar="false" Title="Send Request to ScoopOS" Behaviors="Close" Width="400" Height="150px">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="UpdatePanel2">
                <ContentTemplate>
                    <h4>&nbsp;Are you sure you want to send this request to ScoopOS?</h4>
                    <table align="right">
                        <tr>
                            <td>
                                <button class="btn red" id="BtnCancelSendRequestToScoopOS" onclick="ShowOrCloseWindow('SendToScoopOS',false)">Cancel</button>
                            </td>
                            <td>
                                <asp:LinkButton class="btn green" id="BtnSendRequestToScoopOS" runat="server" OnClientClick="RenderRefresh()">Confirm</asp:LinkButton>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="BtnSendRequestToScoopOS" />
                </Triggers>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
       <telerik:RadWindow ID="WindowSendToScoopOSInfo" runat="server" RenderMode="Lightweight" Modal="false" VisibleOnPageLoad="false" DestroyOnClose="false" VisibleStatusbar="false" Title="Send to ScoopOS result" Behaviors="Close" Width="800" Height="200px">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="UpdatePanel3">
                <ContentTemplate>
                    <div id="divSendToScoopOSInfo" runat="server"></div>
                    <table align="right">
                        <tr>
                            <td>
                                <button class="btn bleu" id="btnViewRequestReplyScoopOS" visible="false" runat="server">View Request/Reply Json</button>
                            </td>
                            <td>
                                <button class="btn green" onclick="ShowOrCloseWindow('SendToScoopOSInfo',false)">Ok</button>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>

    </telerik:RadWindow>
     <script type="text/javascript">        
        var clipboard = new Clipboard('.btn');
    </script>

</asp:Content>
