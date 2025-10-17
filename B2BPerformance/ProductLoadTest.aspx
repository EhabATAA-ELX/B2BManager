<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="ProductLoadTest.aspx.vb" Inherits="ProductLoadTest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="Scripts/jquery-3.2.1.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#select').val("about:blank");
            $('#select').on('change', function () {
                console.log(this.value);
                $('#myFrame').attr('src', this.value);
            });
        });

        function EmptySelection() {
            $('#select').val("about:blank");
            $('#myFrame').attr('src', "about:blank");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table style="width: 100%; border: 1px solid">
        <tr>
            <td>
                <select id="select" name="cars">
                    <option value="about:blank">Please Select</option>
                    <option value="TestProductLoad.aspx">Fill product session variable in Page Load</option>
                    <option value="TestProductLoadWithAshx.aspx">Fill product session variable using ASHX</option>
                </select>
            </td>
            <td>
                <asp:UpdatePanel ID="updatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:Button runat="server" ID="CleanUpSessionButton" OnClientClick="EmptySelection()" OnClick="CleanUpSessionButton_Click" Text="Empty Session" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="CleanUpSessionButton" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <iframe style="border: none;" width="600" height="400" id="myFrame" frameborder="0" ></iframe>
            </td>
        </tr>
    </table>
</asp:Content>

