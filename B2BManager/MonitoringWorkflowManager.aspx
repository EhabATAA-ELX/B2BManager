<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="MonitoringWorkflowManager.aspx.vb" Inherits="MonitoringWorkflowManagerWebForm" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/MonitoringWorkflow.ascx" TagPrefix="uc1" TagName="MonitoringWorkflow" %>



<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script type="text/javascript">
        function ShowOrCloseWindow(windowIdentifier, Show) {
            var oWnd = null;
            switch (windowIdentifier) {
                case "NewWorkflow":
                    $("#<%= txtBoxWorkflowName.ClientID %>").val("");
                    $("#<%= ErrorInfo.ClientID %>").text(" ");
                    oWnd = $find("<%= WindowAddNewWorkflow.ClientID %>");
                    break;
                case "DeleteWorkflow":
                    $("#<%= lblDeleteWorkflowErrorMessage.ClientID %>").text(" ");
                    oWnd = $find("<%= WindowDeleteWorkflow.ClientID %>");
                    break;
                case "ExecuteWorkflow":
                    oWnd = $find("<%= WindowExecuteWorkflow.ClientID %>");
                    break;
            }
            if (oWnd != null) {
                if (Show) {
                    oWnd.show();
                }
                else {
                    oWnd.close();
                }
            }
            return false;
        }

        function ProcessButton(sender) {

            switch (sender) {
                case "Save":
                    $('#<%= BtnSaveWorkflow.ClientID %>').addClass("loadingBackground").html("Saving..").prop('disabled', true);
                    break;
                case "SaveForLater":
                    $('#<%= BtnSaveWorkflowForLater.ClientID %>').addClass("loadingBackground").html("Saving..").prop('disabled', true);
                    break;
                case "Delete":
                    $('#<%= BtnConfirmDeleteWorkflow.ClientID %>').addClass("loadingBackground").html("Deleting..").prop('disabled', true);
                    break;
            }
            return false;
        }

        function ExecuteWorkflow() {
            var WorkflowsCombo = $find('<%=ddlWorkflows.ClientID %>');
            var url = 'MonitoringWorkflowProfile.aspx?RunWorkflow=true&WorkflowID=' + WorkflowsCombo.get_selectedItem().get_value();
            if ($(window).height() > 1000 && $(window).width() > 1450) {
                var oWnd = $find("<%= WindowExecuteWorkflow.ClientID %>");
                oWnd.setUrl(url + "&HideHeader=true");
                ShowOrCloseWindow("ExecuteWorkflow", true);
            }
            else {
                popup(url,true);
            }
        }

        function DisplayExecutionLog(processID,workflowID) {
            var WorkflowsCombo = $find('<%=ddlWorkflows.ClientID %>');
            var url = 'MonitoringWorkflowProfile.aspx?RunWorkflow=false&WorkflowID=' + workflowID + "&ProcessID=" + processID;
            if ($(window).height() > 1000 && $(window).width() > 1450) {
                var oWnd = $find("<%= WindowExecuteWorkflow.ClientID %>");
                oWnd.setUrl(url + "&HideHeader=true");
                ShowOrCloseWindow("ExecuteWorkflow", true);
            }
            else {
                popup(url, true);
            }
        }

        function Refresh() {
            __doPostBack('RefreshHiddenField', '');
        }

        function RefreshWithProcess(button,processText) {
            Refresh();
            $(button).addClass("loadingBackground").html(processText).prop('disabled', true);
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="Filters no-print">
                <td>
                    <fieldset style="vertical-align: top">
                        <legend>Environment selection & actions</legend>
                        <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold width120px Electrolux_Color">Environment:</asp:Label>
                        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlEnvironment" AutoPostBack="true" DataTextField="Name" DataValueField="ID">
                        </asp:DropDownList>
                        <input type="button" class="btn bleu" id="btnNewWorkflow" value="Add New Workflow" onclick="ShowOrCloseWindow('NewWorkflow', true)" />
                    </fieldset>
                </td>
                <td>
                    <fieldset style="vertical-align: top" runat="server" id="fieldsetWorkflowSelection">
                        <legend>Workflow selection & actions</legend>
                        <asp:Label runat="server" ID="lblWorkflows" CssClass="Electrolux_light_bold width120px Electrolux_Color">Workflow:</asp:Label>
                        <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width230px" AutoPostBack="true" ID="ddlWorkflows" DataTextField="ID" DataValueField="Name">
                        </telerik:RadComboBox>
                        <asp:Label runat="server" ID="lblAutoSearch" CssClass="Electrolux_light_bold Electrolux_Color">Edit Mode:</asp:Label>
                        <label class="switch">
                            <asp:CheckBox runat="server" Checked="false" AutoPostBack="true" ID="chkBoxEditMode" />
                            <span class="slider round"></span>
                        </label>
                        <asp:Label runat="server" ID="Label1" CssClass="Electrolux_light_bold Electrolux_Color">Expand sub-workflows:</asp:Label>
                        <label class="switch">
                            <asp:CheckBox runat="server" Checked="false" AutoPostBack="true" ID="chkBoxExpandSubWorkflow" />
                            <span class="slider round"></span>
                        </label>
                        <input type="button" class="btn lightblue" id="btnRunWorkflow" value="Run Manually" onclick="ExecuteWorkflow()" />
                        <input type="button" class="btn danger" id="btnDeleteWorkflow" value="Delete" onclick="ShowOrCloseWindow('DeleteWorkflow', true)" />
                        <input type="button" class="btn bleu" id="btnRefresh" value="Refresh Details" onclick="RefreshWithProcess(this, 'Refreshing...')" />
                    </fieldset>
                    <asp:Label runat="server" ID="lblNoWorkflows" Text="No workflow available" ForeColor="Red" Visible="false"></asp:Label>
                </td>
            </table>
            <div runat="server" id="WorkflowContentDiv" style="width: 100%; min-width: 700px; padding: 25px;">
                <uc1:MonitoringWorkflow runat="server" ID="MonitoringWorkflowUC" RunWorkflow="false" ActivateEditMode="false" />
            </div>
            <asp:HiddenField ID="RefreshHiddenField" runat="server" ClientIDMode="Static" />

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="chkBoxEditMode" />
            <asp:AsyncPostBackTrigger ControlID="chkBoxExpandSubWorkflow" />
            <asp:AsyncPostBackTrigger ControlID="ddlWorkflows" />
            <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" />
            <asp:AsyncPostBackTrigger ControlID="RefreshHiddenField" />
        </Triggers>
    </asp:UpdatePanel>

    <telerik:RadWindow ID="WindowAddNewWorkflow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Add New Workflow" Behaviors="Close" Width="500" Height="175px" runat="server">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="UpdatePanel4">
                <ContentTemplate>
                    <table style="margin: 25px;" align="center">
                        <tr class="Height30px">
                            <td class="width230px">
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color width230px">Workflow Name:</asp:Label>
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="txtBoxWorkflowName" CssClass="Electrolux_light width180px" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" align="center">
                                <asp:Label runat="server" ID="ErrorInfo" ForeColor="Red" Text=" "></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" align="center">
                                <input type="button" class="btn red" id="BtnCancelSavePauseInterval" value="Cancel" onclick="ShowOrCloseWindow('NewWorkflow', false)" />
                                <asp:LinkButton CssClass="btn green" ID="BtnSaveWorkflow" runat="server" Text="Save" OnClientClick="ProcessButton('Save')"></asp:LinkButton>
                                <asp:LinkButton CssClass="btn bleu" ID="BtnSaveWorkflowForLater" runat="server" Text="Save for later" OnClientClick="ProcessButton('SaveForLater')"></asp:LinkButton>
                            </td>
                        </tr>
                    </table>                   
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="BtnSaveWorkflow" />
                    <asp:AsyncPostBackTrigger ControlID="BtnSaveWorkflowForLater" />
                </Triggers>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow ID="WindowExecuteWorkflow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." OnClientClose="Refresh" ShowContentDuringLoad="false" Behaviors="Close" Width="1200px" Height="1000px" runat="server">
    </telerik:RadWindow>

    <telerik:RadWindow ID="WindowDeleteWorkflow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Delete Workflow Confirmation" Behaviors="Close" Width="400" Height="180px" runat="server">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="UpdatePanel3">
                <ContentTemplate>
                    <table align="center" style="margin-top: 10px">
                        <tr class="Height30px">
                            <td colspan="3" align="center" style="width: 100%; margin: 25px;">Are you sure you want to delete the workflow
                                <asp:Label runat="server" ID="lblWorkflowToDelete" Font-Bold="true" CssClass="Electrolux_light width230px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" align="center">
                                <asp:Label runat="server" ID="lblDeleteWorkflowErrorMessage" ForeColor="Red" Text=" "></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" align="center">
                                <input type="button" class="btn red" id="BtnCancelDeleteWorkflow" value="Cancel" onclick="ShowOrCloseWindow('DeleteWorkflow', false)" />
                                <asp:LinkButton CssClass="btn green" ID="BtnConfirmDeleteWorkflow" runat="server" Text="Delete" OnClientClick="ProcessButton('Delete')"></asp:LinkButton>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

</asp:Content>




