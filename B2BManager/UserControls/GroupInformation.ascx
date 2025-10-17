<%@ Control Language="VB" AutoEventWireup="false" CodeFile="GroupInformation.ascx.vb" Inherits="UserControls_GroupInformation" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:UpdatePanel runat="server" ID="UpdatePanel1">
    <ContentTemplate>
        <table style="width: 100%">
            <tr>
                <td style="vertical-align: top; width: 45%; min-width: 450px;">
                    <fieldset style="height: 272px">
                        <legend class="Electrolux_Color">Group information</legend>
                        <table cellpadding="5">
                            <tr>
                                <td style="width: 200px">
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Group Name (*):</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxGroupName" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="vertical-align: top">
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Description:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxDescription" CssClass="Electrolux_light width230px" TextMode="MultiLine" Height="100" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Group count color:</asp:Label>
                                </td>
                                <td>
                                    <telerik:RadColorPicker ID="RadColorPickerGroup" ShowIcon="true" ShowEmptyColor="false" Width="230" runat="server"></telerik:RadColorPicker>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Label runat="server" ID="lblInfo" Height="20"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" Font-Size="Smaller" CssClass="Electrolux_light Electrolux_Color">(*) this field is mondatory</asp:Label>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </td>
                <td style="vertical-align: top; width: 55%;">
                    <fieldset style="height: 273px;overflow-y:auto">
                        <legend class="Electrolux_Color">Tools & Actions Assignment</legend>
                        <telerik:RadTreeView runat="server" CheckBoxes="true" ID="treeToolsAndActions" CssClass="ToolsAndActions" TriStateCheckBoxes="true" BorderStyle="None" CheckChildNodes="true">
                        </telerik:RadTreeView>
                    </fieldset>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <input type="button" runat="server" id="btnCancel" class="btn red" value="Cancel changes" />
                    <asp:LinkButton runat="server" ID="btnSubmit" CssClass="btn bleu" OnClick="btnSubmit_Click" Text="Submit Changes"></asp:LinkButton>
                </td>
            </tr>
        </table>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSubmit" />
    </Triggers>
</asp:UpdatePanel>
