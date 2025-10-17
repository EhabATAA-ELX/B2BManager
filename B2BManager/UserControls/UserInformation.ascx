<%@ Control Language="VB" AutoEventWireup="false" CodeFile="UserInformation.ascx.vb" Inherits="UserControls_UserInformation" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<script type="text/javascript">
    function ChangePassword() {
        $("#<%= changePassword.ClientID %>").hide();
        $("#<%= txtBoxPassword.ClientID %>").removeClass("hidden");
    }
</script>
<asp:UpdatePanel runat="server" ID="UpdatePanel1">
    <ContentTemplate>
        <table style="width: 100%">
            <tr>
                <td style="vertical-align: top; width: 45%; min-width: 450px">
                    <fieldset style="height: 455px;">
                        <legend class="Electrolux_Color">User information</legend>
                        <table cellpadding="5">
                            <tr>
                                <td style="width: 200px">
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">First Name:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxFirstName" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Last Name:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxLastName" CssClass="Electrolux_light_bold width230px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Login (*):</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxLogin" CssClass="Electrolux_light_bold width230px" autocomplete="none" runat="server"></asp:TextBox>
                                    <input type="text" name="prevent_autofill" id="prevent_autofill" value="" style="display: none;" />
                                    <input type="password" name="password_fake" id="password_fake" value="" style="display: none;" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Password (*):</asp:Label>
                                </td>
                                <td style="height: 38px">
                                    <div id="changePassword" onclick="ChangePassword()" runat="server" class="defaultLink">Change Passowrd</div>
                                    <asp:TextBox runat="server" CssClass="Electrolux_light width230px" autocomplete="none" ID="txtBoxPassword"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Active:</asp:Label>
                                </td>
                                <td style="height: 38px">
                                    <asp:CheckBox runat="server" CssClass="Electrolux_light width230px" ID="chkBoxActive" Checked="true"></asp:CheckBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Nick Name:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxNickName" CssClass="Electrolux_light_bold width230px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Email:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxEmail" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Expiration Date:</asp:Label>
                                </td>
                                <td>
                                    <telerik:RadDateTimePicker ID="radDateTimeExpirationDate" DateInput-DateFormat="dd/MM/yyyy" TimeView-TimeFormat="HH:mm" runat="server" CssClass="width230px">
                                        <Calendar runat="server">
                                            <SpecialDays>
                                                <telerik:RadCalendarDay Repeatable="Today">
                                                    <ItemStyle CssClass="rcToday" />
                                                </telerik:RadCalendarDay>
                                            </SpecialDays>
                                        </Calendar>
                                    </telerik:RadDateTimePicker>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Production Access:</asp:Label>
                                </td>
                                <td style="height: 38px">
                                    <asp:CheckBox runat="server" CssClass="Electrolux_light width230px" ID="chkBoxProductionAccess" Checked="true"></asp:CheckBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Staging Access:</asp:Label>
                                </td>
                                <td style="height: 38px">
                                    <asp:CheckBox runat="server" CssClass="Electrolux_light width230px" ID="chkBoxStagingAccess" Checked="true"></asp:CheckBox>
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
                    <fieldset style="height: 245px">
                        <legend class="Electrolux_Color">Group Assignment</legend>
                        <div style="height: 210px; overflow-y: auto">
                            <asp:CheckBoxList runat="server" CssClass="checkboxlist" ID="userGroupBoxList"></asp:CheckBoxList>
                        </div>
                    </fieldset>
                    <fieldset style="height: 210px;overflow-y:auto;">
                        <legend class="Electrolux_Color">Country Assignment</legend>
                        <telerik:RadTreeView runat="server" CheckBoxes="true" ID="treeCountries" CssClass="CountriesTree" BorderStyle="None" TriStateCheckBoxes="true" CheckChildNodes="true">
                        </telerik:RadTreeView>
                    </fieldset>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Label runat="server" ID="lblInfo"></asp:Label>
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
