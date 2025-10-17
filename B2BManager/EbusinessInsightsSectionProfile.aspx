<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessInsightsSectionProfile.aspx.vb" Inherits="EbusinessInsightsSectionProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <table align="center" style="padding: 15px">
                <tr>
                    <td class="width130px">
                        <asp:Label runat="server" ID="Label4" CssClass="Electrolux_light_bold Electrolux_Color width130px">Name:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" CssClass="Electrolux_light width230px" ID="txtBoxSectionName"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="txtBoxSectionName" runat="server" ID="rqFieldValidatorChartTitle" Text="* Required" ForeColor="Red" ValidationGroup="SectionDetails"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="width130px">
                        <asp:Label runat="server" ID="Label5" CssClass="Electrolux_light_bold Electrolux_Color width130px">Display Tooltip:</asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" CssClass="Electrolux_light width230px" ID="ddlShowTooltip">
                            <asp:ListItem Text="True" Value="True"></asp:ListItem>
                            <asp:ListItem Text="False" Value="False"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="width130px">
                        <asp:Label runat="server" ID="Label8" CssClass="Electrolux_light_bold Electrolux_Color width130px">Tooltip Text:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" CssClass="Electrolux_light width230px" ID="txtBoxTooltipText" TextMode="MultiLine" Height="130"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label runat="server" ID="lblInfoMessage" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <input type="button" class="btn red" id="btnCloseWindowSection" value="Cancel" onclick="window.parent.ShowOrCloseSectiontWindow(false)" />
                        <asp:LinkButton runat="server" CssClass="btn green" ID="btnSaveOrUpdateSection" Text="Save" ValidationGroup="SectionDetails" OnClick="btnSaveOrUpdateSection_Click"></asp:LinkButton>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSaveOrUpdateSection" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

