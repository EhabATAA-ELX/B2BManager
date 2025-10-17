<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EbusinessDeleteCustomerRange.aspx.vb" Inherits="EbusinessDeleteCustomerRange" MasterPageFile="~/BasicMasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <style type="text/css">
        .name {
            font-weight: bolder;
        }


        .BackgroundSuccess {
        background:#cae0cb;       
        } 

         .BackgroundError {
        background:#f74c4c;       
        } 
    
    </style>
    <script type="text/javascript">
        function CloseWindow() {
            $.magnificPopup.open({
                items: {
                    src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> Customer range timestamp has been successfully reset</p></div>', 
                    type: 'inline'
                },
                callbacks: {
                    close: function () {
                        window.parent.CloseoWnd('Range');
                    }
                }
            });
        }
         function ErrorPopupCustomerRange(str) {
             $.magnificPopup.open({
                    items: {
                        src: '<div class="white-popup error-popup" ><p class="text-justify"><i class="fas fa-exclamation-triangle" style="font-size: 18pt;vertical-align: middle;"></i>' + str + '</p></div>', 
                        type: 'inline'
                        },
                 callbacks: {
                     close: function () {
                         window.parent.CloseoWnd('Range');
                     }
                 }
                });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel2">
        <ContentTemplate>
              <table style="margin:25px">
                <tr>
                    <td>
                        <h4><asp:Label runat="server" ID="Lbl_Title"></asp:Label></h4>
                    </td>
                </tr>
                <tr>
                </tr>
                <tr>
                    <td align="center">
                        <button class="btn red" id="BtnCancelTimeStampCusRange" onclick="window.parent.CloseoWnd('Range')">Cancel</button>
                        <asp:LinkButton class="btn blue" ID="BtnDeleteTimeStampCusRange" OnClick="BtnDeleteimeStampCusRange_Click"  runat="server">Confirm</asp:LinkButton>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="BtnDeleteTimeStampCusRange" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
