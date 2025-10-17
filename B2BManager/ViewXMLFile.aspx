<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="ViewXMLFile.aspx.vb" Inherits="ViewXMLFile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script src="Scripts/jquery-3.2.1.js"></script>
    <script src="Scripts/Global.js"></script>
    <script src="Scripts/clipboard.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <div id="divXMLFile" runat="server" class="XMLFile"></div>
            <button class="btn bleu" id="BtnCopyXML" runat="server" data-clipboard-action="copy" data-clipboard-target="#ContentPlaceHolder1_divXMLFile">Copy XML</button>
            <button class="btn bleu" id="BtnViewXMLInBrowser" runat="server">View XML In Browser</button>
            <button class="btn bleu" id="BtnDownloadXML" runat="server">Download XML</button>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="BtnCopyXML" />
            <asp:AsyncPostBackTrigger ControlID="BtnViewXMLInBrowser" />
            <asp:AsyncPostBackTrigger ControlID="BtnDownloadXML" />
        </Triggers>
    </asp:UpdatePanel>

     <script type="text/javascript">        
        var clipboard = new Clipboard('.btn');
    </script>

</asp:Content>

