<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EbusinessDeleteCachePrice.aspx.vb" Inherits="EbusinessDeleteCachePrice"  MasterPageFile="~/BasicMasterPage.master" %>

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
                    src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> Price cache has been successfully reset</p></div>', 
                    type: 'inline'
                },
                callbacks: {
                    close: function () {
                        window.parent.CloseoWnd('Price');
                    }
                }
            });
        }
         function ErrorPopupPrice(str) {
             $.magnificPopup.open({
                    items: {
                        src: '<div class="white-popup error-popup" ><p class="text-justify"><i class="fas fa-exclamation-triangle" style="font-size: 18pt;vertical-align: middle;"></i>' + str + '</p></div>', 
                        type: 'inline'
                        },
                 callbacks: {
                     close: function () {
                         window.parent.CloseoWnd('Price');
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
                    <td>
                        <asp:Label runat="server" ForeColor="Red" ID="lblInfo"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <button class="btn red" id="BtnCancelDeletePriceCache" onclick="window.parent.CloseoWnd('Price')">Cancel</button>
                        <asp:LinkButton class="btn blue" ID="BtnDeletePriceCache" OnClick="BtnPriceCache_Click"  runat="server">Confirm</asp:LinkButton>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="BtnDeletePriceCache" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
