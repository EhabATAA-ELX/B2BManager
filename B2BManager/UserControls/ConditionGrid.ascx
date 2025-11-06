<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ConditionGrid.ascx.vb" Inherits="UserControls_ConditionGrid" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadGrid ID="Rg" runat="server" OnNeedDataSource="Rg_NeedDataSource" OnItemDataBound="Rg_ItemDataBound" RenderMode="Lightweight" AllowPaging="True" AllowSorting="true" AutoGenerateColumns="False" Width="100%">
    <PagerStyle Mode="NextPrevAndNumeric" />
    <MasterTableView Width="95%" DataKeyNames="ConditionID, ConditionIsStatic,SOPName">
        <Columns>
            <telerik:GridTemplateColumn AllowFiltering="false" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="10%">
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="EditConditionBtn">
                        <img src="Images/edit.png" alt="Edit" width="20" height="20" />
                    </asp:LinkButton>
                    <asp:LinkButton runat="server" ID="DeleteConditionBtn">
                        <img src="Images/Delete.png" alt="Delete" width="20" height="20" />
                    </asp:LinkButton>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn DataField="ConditionName" Display="false" SortExpression="ConditionName" HeaderText="Name" HeaderStyle-Width="35%" />
            <telerik:GridCheckBoxColumn DataField="ConditionIsStatic" Display="false" HeaderText="Static Condition" UniqueName="Static" SortExpression="ConditionIsStatic" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="10%" />
            <telerik:GridBoundColumn DataField="ConditionMatchingNumber" SortExpression="ConditionMatchingNumber" HeaderText="Matching Customers" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="20%" />
            <telerik:GridBoundColumn DataField="CriteriaCount" SortExpression="CriteriaCount" HeaderText="Criteria Count" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="10%" />
            <telerik:GridBoundColumn DataField="SOPName" Display="false" />
            <telerik:GridBoundColumn DataField="ConditionUpdateDate" HeaderStyle-HorizontalAlign="Center" DataFormatString="{0:dd/MM/yyyy}" SortExpression="ConditionUpdateDate" HeaderText="Last update" HeaderStyle-Width="15%" />
            <telerik:GridBoundColumn DataField="ConditionUpdatedBy" HeaderStyle-HorizontalAlign="Center" SortExpression="ConditionUpdatedBy" HeaderText="Updated by" HeaderStyle-Width="15%" />
        </Columns>
    </MasterTableView>
</telerik:RadGrid>

<script type="text/javascript">
    // expose the RadGrid client id as a global variable based on this control's ID
    (function(){
        try {
            window['<%= Me.ID %>_RgClientID'] = '<%= Rg.ClientID %>';
        } catch(e) { }
    })();
</script>
