<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="FocusRangeManagement.aspx.vb" Inherits="FocusRangeManagement" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/ConditionGrid.ascx" TagPrefix="uc" TagName="ConditionGrid" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
  
    <script type="text/javascript">
        // Standard Telerik helper function to get the current RadWindow object
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement && window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function CloseWindow() {
            if (typeof window.parent.CloseWindowManagement == "function") {
                window.parent.CloseWindowManagement();
            }
        }

        function ProcessButton(sender) {
            if ($("#<%= txtFocusRangeName.ClientID %>").val().length == 0) {
                return;
            }
            switch (sender) {
                case "Add": {
                    $('#<%= btnSubmit.ClientID%>').addClass("loadingBackground").html("Submitting..").prop('disabled', true);
                    break;
                }
                case "Update": {
                    $('#<%= btnSubmit.ClientID%>').addClass("loadingBackground").html("Updating..").prop('disabled', true);
                    break;
                }
            }

            return false;
        }


        function AddFocusRange() {
            $("#<%=btnUploadNewRange.ClientID%>").trigger('click');
        }

        /* ---------------Start Direct/IndDirect Assignment ------------------- */
        function callParentPreview() {
            var urlParams = new URLSearchParams(window.location.search);
            var focusRangeId = urlParams.get('id');

            if (focusRangeId) {
                var oWindow = GetRadWindow();
                if (oWindow && oWindow.BrowserWindow) {
                    oWindow.BrowserWindow.loadAndShowPreview(focusRangeId);
                }
            } else {
                alert("Could not find the Focus Range ID to preview.");
            }
        }

        function VerifyDate() {
            var startDate = $("#<%=txtStartDate.ClientID%>").val();
            var endDate = $("#<%=txtEndDate.ClientID %>").val();
            var startDateSchedule = new Date(startDate);
            var endDateSchedule = new Date(endDate);
            if (startDateSchedule.getTime() > endDateSchedule.getTime()) {
                $("#<%=txtEndDate.ClientID%>").val(startDate);
            }
        }

        function refreshGrid() {
            var gridClientId = window['ConditionGrid1_RgClientID'];
            var grid = gridClientId ? $find(gridClientId) : null;
             if (grid) {
                 grid.get_masterTableView().rebind();
             }
         }

         function DeleteConditionFocusRange(conditionID) {
             $.ajax({
                 type: "POST",
                 url: "FocusRangeManagement.aspx/DeleteCondition",
                 data: JSON.stringify({ conditionId: conditionID }),
                 contentType: "application/json; charset=utf-8",
                 dataType: "json",
                 success: function (response) {
                     if (response.d === true) {
                        var gridClientId = window['ConditionGrid1_RgClientID'];
                        var grid = gridClientId ? $find(gridClientId) : null;
                        if (grid) {
                            grid.get_masterTableView().rebind();
                        }
                     } else {
                         alert("Error: Could not delete condition.");
                     }
                 }
             });
         }

        /* ---------------END Direct/IndDirect Assignment ------------------- */

    </script>
    <style type="text/css">
        .radwindowPreview{
            z-index :3050 !important ;
        }
        #ContentPlaceHolder1_AddStaticConditionBtn.aspNetDisabled {
            background-color: #c1c1c1 !important;
            cursor: not-allowed !important;
            font-weight: 400;
            border: none !important;
            color: #FFFFFF !important;
            padding: 6px 14px !important;
            text-align: center !important;
            font-family: 'Electrolux_light', sans-serif !important; /* Added a fallback font */
            border-radius: 0px !important;
            font-size: 14px !important;
            text-decoration: none !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%--  <asp:UpdatePanel ID="updatePanel1" runat="server">
        <ContentTemplate>--%>
    <table cellpadding="2" style="margin: 15px; width: 100%" align="center" id="tbl1" runat="server">
        <tr valign="top">
            <td class="width180px">
                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Focus Range Title (*):</asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtFocusRangeName" MaxLength="250" CssClass="Electrolux_light width230px ui-widget" runat="server"></asp:TextBox>
            </td>
            <td style="text-align: left">
                <%--   <asp:RequiredFieldValidator runat="server" ID="ReqtxtFocusRangeName" ControlToValidate="txtFocusRangeName" ForeColor="Red" ErrorMessage="* required" />--%>
            </td>
        </tr>

        <tr valign="top">
            <td class="width180px">
                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Start date:</asp:Label>
            </td>
            <td>
                <input name="txtStart" id="txtStartDate" onchange="VerifyDate()" type="date" class="Electrolux_Color" style="width: 150px;" runat="server" />
            </td>
            <td style="text-align: left"></td>
        </tr>

        <tr valign="top">
            <td class="width180px">
                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">End date:</asp:Label>
            </td>
            <td>
                <input name="txtEnd" id="txtEndDate" onchange="VerifyDate()" type="date" class="Electrolux_Color" style="width: 150px;" runat="server" />
            </td>
            <td style="text-align: left"></td>
        </tr>

        <tr valign="top">
            <td class="width180px">
                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Products count in file:</asp:Label>
            </td>
            <td>
                <asp:Label ID="lblProductsCount" MaxLength="5" CssClass="Electrolux_light width230px ui-widget"
                    Font-Size="Large" ForeColor="Green" runat="server">0</asp:Label>
            </td>
            <td style="text-align: left"></td>
        </tr>
        <tr valign="top">
            <td class="width180px" colspan="2" runat="server" id="trFileUpload">
                <label id="fileUpload1" class="bouton fr-btn btn-blanc">
                    <span class="SpanUploadFile">Upload New Range</span>
                    <asp:FileUpload ID="FileUpload1" runat="server" onchange="AddFocusRange(); "
                        class="InputFileUpload" />
                    <asp:Button ID="btnUploadNewRange" class="btnUploadNewRange" runat="server" Style="display: none" />
                    <asp:HiddenField ID="hdProducts" runat="server" />
                </label>
            </td>
        </tr>
        <tr>
            <td align="center" colspan="3">
                <asp:Label runat="server" ID="lblErrorInfo" ForeColor="Red" Text=" "></asp:Label>
            </td>
        </tr>


        <tr>
            <td>
                <asp:GridView runat="server" ID="FocusRangeDataGrid" ShowHeader="true"
                    AllowPaging="true" PageSize="10"
                    ShowHeaderWhenEmpty="true" AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="PNC" HeaderText="PNC"></asp:BoundField>
                        <asp:BoundField DataField="ModelD" HeaderText="ModelD"></asp:BoundField>
                    </Columns>
                    <EmptyDataTemplate>
                        Empty
                    </EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>

        <tr runat="server" id="ButtonsTR">
            <td colspan="3" align="center">
                <asp:LinkButton runat="server" CssClass="btn red" ID="btnCancel" CausesValidation="false" OnClientClick="CloseWindow()">Cancel</asp:LinkButton>
                <asp:LinkButton runat="server" ID="btnSubmit" class="btn bleu" Text="Submit changes" OnClick="btnSubmit_Click"></asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <span>
                    <asp:Label ID="lblNumberAssign" runat="server" Text="Number of assigned customers : " /><asp:Label ID="AssignedCustomersNumberLb" runat="server" />
                </span>
                <asp:Button ID="btnPreviewCustomers" OnClientClick="callParentPreview(); return false;" runat="server" class="btn bleu rounded" Text="Preview Matching Customers" />
            </td>
        </tr>

        <tr>
            <td colspan="3" style="text-align: center">
                <uc:ConditionGrid ID="ConditionGrid" runat="server" PageSource="FocusRange" />
             </td>
         </tr>
        <tr>
            <td colspan="3">
                <span>
                    <asp:Button ID="AddConditionBtn" runat="server" class="btn bleu rounded" Text="Add Condition" />
                    <asp:Button ID="AddStaticConditionBtn" runat="server" class="btn bleu rounded" Text="Assign customers manually" />
                </span>
            </td>
        </tr>
    </table>
    <%--  </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSubmit" />
            <asp:AsyncPostBackTrigger ControlID="btnUploadNewRange" />
        </Triggers>
    </asp:UpdatePanel>--%>
</asp:Content>

