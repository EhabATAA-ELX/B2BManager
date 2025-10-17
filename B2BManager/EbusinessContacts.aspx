<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessContacts.aspx.vb" Inherits="EbusinessContacts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
    <%@ Register Src="~/UserControls/ContactPreview.ascx" TagPrefix="uc1" TagName="ContactPreview" %>
    <script type="text/javascript">
        function PreviewContactCard(contactid) {
            __doPostBack('<%= tvContacts.ClientID %>', contactid);
        }

        function OnClientNodeClicking(sender, eventArgs) {
            eventArgs.set_cancel(true);
        }

        function ConfirmAssignment() {
            $.magnificPopup.open({
                items: {
                    src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> Contacts updated with success.</p></div>',
                    type: 'inline'
                }
            });
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <div class="row">
                <div class="col-md-6 col-lg-5">

                    <div class="card card-primary" style="height: 500px">
                        <div class="card-header">
                            Contact assignment
                        </div>
                        <div class="card-body">
                            <telerik:RadTreeView runat="server" CssClass="no-border" CheckBoxes="true" Height="400" ShowLineImages="false" ID="tvContacts" OnClientNodeClicking="OnClientNodeClicking" EnableDragAndDrop="false" AllowNodeEditing="false"></telerik:RadTreeView>                            
                            <div style="text-align:center;width:100%;position:absolute;bottom:5px">
                                <asp:Label runat="server" Visible="false" ID="lblNothingToChange" ForeColor="Red"></asp:Label>
                                <asp:Panel  runat="server" ID="actionButtonsPlaceHolder">
                                    <asp:LinkButton runat="server" CssClass="btn red" ID="btnCancel" OnClick="btnCancel_Click" CausesValidation="false" ><i class="fas fa-ban"></i> Cancel changes</asp:LinkButton>
                                    <asp:LinkButton runat="server" ID="btnSubmit" class="btn bleu" OnClick="btnSubmit_Click" OnClientClick="ProcessButton(this, 'Saving...');"><i class="fas fa-check"></i> Save changes</asp:LinkButton>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-6 col-lg-7">
                    <div class="card card-primary" style="height: 500px">
                        <div class="card-header">
                            Preview selected contact card
                        </div>
                        <div class="card-body" >
                            <uc1:ContactPreview runat="server" ID="ContactPreview" />                            
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="tvContacts" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

