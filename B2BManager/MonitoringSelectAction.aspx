<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="MonitoringSelectAction.aspx.vb" Inherits="MonitoringSelectAction" %>

<%@ Register Src="~/UserControls/MonitoringActionsGrid.ascx" TagPrefix="uc1" TagName="MonitoringActionsGrid" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
    <script type="text/javascript">
        function SelectActionWithParameters(imageName, comments, ActionID, IsActionIDWorkflowID) {
        var action = {
            ImageUrl: 'Images/MonitoringWorkflow/' + imageName,
            Comments : comments,
            ID: ActionID,
            IsActionIDWorkflowID: IsActionIDWorkflowID
        }

        if (typeof (window.parent.SelectAction) == "function") {
            window.parent.SelectAction(action);
        }
        else {
            if (typeof (window.opener.SelectAction) == "function") {
                window.opener.SelectAction(action);
                window.close();
            }
        }
        return false;
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:MonitoringActionsGrid runat="server" ID="MonitoringActionsGrid" />
</asp:Content>

