<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessAttachCustomerToSuperUser.aspx.vb" Inherits="EbusinessAttachCustomerToSuperUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
    <script type="text/javascript">
        function OnClientNodeClicking(sender, eventArgs) {
            eventArgs.set_cancel(true);
        }
        function searchInSuperUserListClick() {
            var textSearch = $("#searchInSuperUserList")[0].value.toString().toLowerCase();
            $(".rtLI").removeClass("hidden").addClass("hidden");
            if (textSearch != "") {
                $(".rtLI:has(span[data-search*='" + textSearch + "'])").removeClass("hidden");
            }
            else {
                $(".rtLI").removeClass("hidden");
            }
        }

        $(document).ready(function () {
            $('#searchInSuperUserList').on('paste', function () {
                searchInSuperUserListClick();
            });
        });

        function toggleCheckAll(checkButtonValue) {
            if (checkButtonValue == "Uncheck") {
                $("#<%= tvSuperUsers.ClientID %> input[type='checkbox']:checked").trigger("click");
            } else {
                $("#<%= tvSuperUsers.ClientID %> input[type='checkbox']:not(:checked)").trigger("click");
            }
        }

        function ConfirmAssignment() {
                $.magnificPopup.open({
                    items: {
                        src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> Changes are applied with success.</p></div>',
                        type: 'inline'
                    }
                });
            }
    </script>
     <style type="text/css">
        .rtImg{
            height:22px;
         }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    
    <table width="100%" cellpadding="5">
        <tr>
            <td>Search:<input type="text" placeholder="Type in name or email..." id="searchInSuperUserList" class="width180px" style="height: 26px" onkeyup="searchInSuperUserListClick()" /></td>
            <td style="text-align:right">                
                <div runat="server" class="Electrolux_Color" id="infoLabel"></div>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="text-align:center">
                <a class="defaultLink linkColor" onclick="toggleCheckAll('Check')">Select all</a> | <a class="defaultLink linkColor" onclick="toggleCheckAll('Uncheck')">Unselect all </a>
            </td>
        </tr>
    </table>
    <telerik:RadTreeView runat="server" CssClass="no-border" CheckBoxes="true" Height="400" ShowLineImages="false" ID="tvSuperUsers" OnClientNodeClicking="OnClientNodeClicking" EnableDragAndDrop="false" AllowNodeEditing="false" ></telerik:RadTreeView>
    <div style="text-align: center; width: 100%; position: absolute; bottom: 5px">
        <asp:Panel runat="server" ID="actionButtonsPlaceHolder">
            <asp:LinkButton runat="server" CssClass="btn red" OnClientClick="window.parent.CloseAttachToSuperUserWindow()" ID="btnCancel" CausesValidation="false"><i class="fas fa-ban"></i> Cancel</asp:LinkButton>
            <asp:LinkButton runat="server" ID="btnSubmit" class="btn bleu" OnClick="btnSubmit_Click" OnClientClick="ProcessButton(this, 'Saving...');"><i class="fas fa-check"></i> Save</asp:LinkButton>
        </asp:Panel>
    </div>
</asp:Content>

