<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" Async="true" CodeFile="FileViewer.aspx.vb" Inherits="FileViewer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <style type="text/css">
        .fileContent {
            margin-left: 25px;
            height: 500px;
            overflow: auto;
            width: 98%;
            border: 1px solid #e5e5e5;
        }
    </style>
    <script src="Scripts/clipboard.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table align="left" cellpadding="5px" cellspacing="5px" style="width: 100%; margin-left: 25px">
        <tr>
            <td class="width230px">
                <asp:Label runat="server" ID="lblFileName" CssClass="Electrolux_light_bold Electrolux_Color width230px">File name:</asp:Label>
            </td>
            <td>
                <asp:Label runat="server" ID="FileName" ForeColor="Blue" CssClass="Electrolux_light width230px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="width230px">
                <asp:Label runat="server" ID="lblFullName" CssClass="Electrolux_light_bold Electrolux_Color width230px">Parent directory path:</asp:Label>
            </td>
            <td>
                <asp:Label runat="server" Enabled="false" ID="FullName" CssClass="Electrolux_light width230px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="width230px">
                <asp:Label runat="server" ID="lblSize" CssClass="Electrolux_light_bold Electrolux_Color width230px">Size:</asp:Label>
            </td>
            <td>
                <asp:Label runat="server" ID="Size" CssClass="Electrolux_light width230px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="width230px">
                <asp:Label runat="server" ID="lblCreationDate" CssClass="Electrolux_light_bold Electrolux_Color width230px">Creation date:</asp:Label>
            </td>
            <td>
                <asp:Label runat="server" ID="CreationDate" CssClass="Electrolux_light width230px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="width230px">
                <asp:Label runat="server" ID="lblLastModificationDate" CssClass="Electrolux_light_bold Electrolux_Color width230px">Last modification date:</asp:Label>
            </td>
            <td>
                <asp:Label runat="server" ID="LastModificationDate" CssClass="Electrolux_light width230px"></asp:Label>
            </td>
        </tr>
    </table>
    <pre class="fileContent" runat="server" id="fileContent"></pre>
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <div style="margin-left: 25px">
                <input type="button" class="btn bleu" id="btnCopy" data-clipboard-action="copy" data-clipboard-target="#ContentPlaceHolder1_fileContent" value="Copy Content" />
                <asp:LinkButton CssClass="btn green" ID="btnDownloadFile" OnClick="btnDownload_Click" runat="server">Download</asp:LinkButton>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnDownloadFile" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">        
        var clipboard = new Clipboard('#btnCopy');
    </script>
</asp:Content>

