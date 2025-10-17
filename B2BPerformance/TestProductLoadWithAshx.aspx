<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="TestProductLoadWithAshx.aspx.vb" Inherits="TestProductLoadWithAshx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="Scripts/jquery-3.2.1.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $.ajax({
                type: "GET",
                url: "MyHandler.ashx",
                contentType: "text/plain; charset=utf-8",
                async: true,
                success: function (response) {
                    $("#<%= lblInfo.ClientID %>").text(response);
                },
                error: function (xhr, textStatus, errorThrown) {
                    console.log(errorThrown);
                }
            });
        });        
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:Label runat="server"  ID="lblInfo" ForeColor="Green"></asp:Label>
</asp:Content>

