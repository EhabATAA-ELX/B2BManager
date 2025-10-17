<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ContactDetails.ascx.vb" Inherits="UserControls_ContactDetails" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/ContactPreview.ascx" TagPrefix="uc1" TagName="ContactPreview" %>
<script type="text/javascript">
    function ShowPreviewWindow() {
        var oWnd = $find("<%= WindowPreview.ClientID %>");
        oWnd.show();
    }


    function SubmitAction(Action,ResultText) {
        $.magnificPopup.open({
            items: {
                src: '<div class="white-popup">'+ResultText+'</div>', 
                type: 'inline'
            },
            callbacks: {
                close: function () {
                    if (window.parent) {
                        if (typeof window.parent.RefreshGrid == "function") {
                            window.parent.RefreshGrid(Action);
                        }
                    }
                    if (window.opener) {
                        if (typeof window.opener.RefreshGrid == "function") {
                            window.opener.RefreshGrid(Action);
                        }
                        window.close();
                    }
                }
            }
        });
    }

    function Cancel() {
        if (window.parent) {
            if (typeof window.parent.RefreshGrid == "function") {
                window.parent.RefreshGrid("Cancel");
            }
        }
        if (window.opener) {
            if (typeof window.opener.RefreshGrid == "function") {
                window.opener.RefreshGrid("Cancel");
            }
            window.close();
        }
    }
</script>

<asp:UpdatePanel runat="server" ID="UpdatePanelDetails">
    <ContentTemplate>
        <div class="row" style="padding: 25px 5px 5px 5px">
            <div class="col-sm-6">
                <div class="card card-primary card-outline">
                    <div class="card-body">
                        <table cellpadding="5" class="Width100">
                            <tr>
                                <td style="width: 120px">
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Name:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxName" ValidationGroup="contact-details" CssClass="Electrolux_light Width100 MaxWidth250px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Type:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxType" ValidationGroup="contact-details" CssClass="Electrolux_light_bold Width100 MaxWidth250px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Direct:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxDirect" ValidationGroup="contact-details" CssClass="Electrolux_light_bold Width100 MaxWidth250px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Office:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxOffice" ValidationGroup="contact-details" CssClass="Electrolux_light_bold Width100 MaxWidth250px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Fax:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxFax" ValidationGroup="contact-details" CssClass="Electrolux_light_bold Width100 MaxWidth250px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Email:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxEmail" ValidationGroup="contact-details" CssClass="Electrolux_light_bold Width100 MaxWidth250px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 120px">
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Street 1:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxStreet1" ValidationGroup="contact-details" CssClass="Electrolux_light Width100 MaxWidth250px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Street 2:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxStreet2" ValidationGroup="contact-details" CssClass="Electrolux_light_bold Width100 MaxWidth250px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Street 3:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxStreet3" ValidationGroup="contact-details" CssClass="Electrolux_light_bold Width100 MaxWidth250px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Street 4:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxStreet4" ValidationGroup="contact-details" CssClass="Electrolux_light_bold Width100 MaxWidth250px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Street 5:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxStreet5" ValidationGroup="contact-details" CssClass="Electrolux_light_bold Width100 MaxWidth250px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Postcode:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxPostcode" ValidationGroup="contact-details" CssClass="Electrolux_light_bold Width100 MaxWidth250px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <div class="col-sm-6">
                <div class="card card-primary card-outline">
                    <div class="card-body">
                        <table cellpadding="5" class="Width100">
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">County:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxCounty" ValidationGroup="contact-details" CssClass="Electrolux_light_bold Width100 MaxWidth250px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">City:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxCity" ValidationGroup="contact-details" CssClass="Electrolux_light_bold Width100 MaxWidth250px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxCountry" ValidationGroup="contact-details" CssClass="Electrolux_light_bold Width100 MaxWidth250px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Position:</asp:Label>
                                </td>
                                <td>
                                    <telerik:RadNumericTextBox runat="server" MinValue="0" Value="0" ShowSpinButtons="true" DataType="Integer" MaxValue="1000" ID="txtPosition" CssClass="Electrolux_light_bold Electrolux_Color">
                                        <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                    </telerik:RadNumericTextBox>
                                </td>
                            </tr>
                            <tr valign="top">
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Before text:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxBeforeText" ValidationGroup="contact-details" CssClass="Electrolux_light_bold Width100 MaxWidth250px" TextMode="MultiLine" Height="74" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr valign="top">
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">In card text:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBoxInCardText" ValidationGroup="contact-details" CssClass="Electrolux_light_bold Width100 MaxWidth250px" TextMode="MultiLine" Height="65" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="text-align: center">
                                    <asp:RequiredFieldValidator runat="server" ID="requiredFieldName" ForeColor="Red" Text="* Contact name is mondatory" ControlToValidate="txtBoxName" ValidationGroup="contact-details"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="text-align: center">
                                    <asp:LinkButton runat="server" CssClass="btn red" OnClick="CancelBtn_Click" ID="CancelBtn"><i class="fas fa-ban"></i> Cancel changes</asp:LinkButton>
                                    <asp:LinkButton runat="server" ID="PreviewBtn" CssClass="btn lightblue" OnClick="PreviewBtn_Click"><i class="far fa-eye"></i> Preview</asp:LinkButton>
                                    <asp:LinkButton runat="server" ID="SubmitBtn" ValidationGroup="contact-details" OnClientClick="ProcessButton(this, 'Submitting...');" OnClick="SubmitBtn_Click" CssClass="btn blue"><i class="far fa-check-circle"></i> Submit changes</asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <div class="col-md-12" runat="server" id="PreviewPanel">
                <div class="card card-primary">
                    <div class="card-header">
                        <span class="rtsTxt" style="font-size: 13px">How it is displayed in the B2B portal</span>
                        <div class="card-tools">
                            <button type="button" class="btn-admin btn-tool" data-card-widget="collapse">
                                <i class="fas fa-minus"></i>
                            </button>
                        </div>
                    </div>
                    <div id="tContact" class="Width100 card-body">
                        <uc1:ContactPreview runat="server" ID="ContactPreview" />
                    </div>
                </div>
            </div>
        </div>


    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="SubmitBtn" />
        <asp:AsyncPostBackTrigger ControlID="CancelBtn" />
        <asp:AsyncPostBackTrigger ControlID="PreviewBtn" />
    </Triggers>
</asp:UpdatePanel>

<telerik:RadWindow ID="WindowPreview" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Preview" Behaviors="Close" Width="750px" Height="292px" runat="server">
    <ContentTemplate>
        <asp:UpdatePanel runat="server" ID="updatePanelPreview" UpdateMode="Conditional">
            <ContentTemplate>
                <uc1:ContactPreview runat="server" ID="ContactPreviewWindow" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
