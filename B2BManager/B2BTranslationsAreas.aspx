<%@ Page Language="VB" AutoEventWireup="false"  MasterPageFile="~/BasicMasterPage.master" CodeFile="B2BTranslationsAreas.aspx.vb" Inherits="B2BTranslationsAreas" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script type="text/javascript">
        function CloseWindow() {
            console.log("1")
            window.parent.ShowAndRefreshGrid();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel2">
        <ContentTemplate>
            <div class="MarginTop10px floatLeft">
                <div class="floatLeft MarginLeft10px">
                    <asp:Label runat="server" CssClass="Electrolux_light_bold">Translation Name :</asp:Label>
                </div>
                <div class="floatLeft">
                    <asp:TextBox runat="server" Enabled="false" ID="Lbl_Translation_Name"></asp:TextBox>
                </div>
            </div>
            <div class="MarginTop10px MarginLeft10px floatLeft">
                <telerik:RadListBox RenderMode="Lightweight" ID="AreasID" runat="server" CheckBoxes="true" ShowCheckAll="true" DataTextField="TA_Name" Height="460" Width="470"
                    DataValueField="TA_AreaID" OnDataBound="AreasID_DataBinding"  >
                </telerik:RadListBox>
            </div>
            <div style="text-align:center">
                <button class="btn red" id="BtnCancelTranslationsAreas" onclick="window.parent.CloseWindow()">Cancel</button>
                <asp:LinkButton class="btn green" ID="BtnTranslationsAreas" OnClick="BtnTranslationsAreas_Click" runat="server">Confirm</asp:LinkButton>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
