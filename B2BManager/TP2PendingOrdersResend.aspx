<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TP2PendingOrdersResend.aspx.vb" MasterPageFile="~/BasicMasterPage.master" Inherits="TP2PendingOrdersResend" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">

    <script type="text/javascript">
        function CloseWindow() {
            $.magnificPopup.open({
                items: {
                    src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> Order(s) have been sent successfully to SAP</p></div>',
                    type: 'inline'
                },
                callbacks: {
                    close: function () {
                        $('#myProgress').hide();
                        window.parent.ShowAndRefreshGrid();
                        window.close();
                        }
                    }
                
            });
            
        }
        function BeginSendRequestToSap(btnID) {
            var btn = document.getElementById(btnID);
            btn.disabled = true;
            btn.text = "Sending...";
            btn.classList.add("loadingBackground");
            $('#myProgress').show();
            move()
        }
        var i = 0;
        function move() {
            if (i == 0) {
                i = 1;
                var elem = document.getElementById("myBar");
                var width = 1;
                var id = setInterval(frame, 500);
                function frame() {
                    console.log("test");
                    if (width >= 100) {
                        clearInterval(id);
                        i = 0;
                    } else {
                        nborders = $("#ContentPlaceHolder1_Nb_PendingOrders").val()
                        width = width + Math.round(100 / nborders);
                        elem.style.width = width + "%";
                    }
                }
            }
        }
    </script>
  <style>
#myProgress {
  width: 100%;
  background-color: #ddd;
  margin-top: 22%;
}

#myBar {
  width: 1%;
  height: 30px;
  background-color: #04AA6D;
}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel2">
        <ContentTemplate>
            <h4>&nbsp;You are about to resend order(s)?</h4>
            <table align="right">
                <tr>
                    <td colspan="2" align="center">
                        <asp:Label runat="server" ForeColor="Red" ID="lblInfo"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <button class="btn red" id="BtnCancelSendRequestToSap" onclick="window.parent.CloseWindow()">Cancel</button>
                    </td>
                    <td>
                        <asp:LinkButton ClientIDMode="Static" class="btn green" OnClientClick="BeginSendRequestToSap(this.id)" ID="BtnSendRequestToSap" runat="server">Confirm</asp:LinkButton>
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="Nb_PendingOrders" runat="server"  />
            <div id="myProgress" style="display:none">
              <div id="myBar"></div>
            </div>
         </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="BtnSendRequestToSap" />
        </Triggers>
    
    </asp:UpdatePanel>
</asp:Content>
