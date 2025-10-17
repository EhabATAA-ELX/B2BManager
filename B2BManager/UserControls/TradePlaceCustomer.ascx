<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TradePlaceCustomer.ascx.vb" Inherits="UserControls_TradePlaceCustomer" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<h3>Tradeplace Customer</h3>
<div style="display: inline-block;">
    <div id="PanelContext" runat="server" visible="false">
        <asp:Label ID="lblContext" runat="server" CssClass="LibContext">        
        <span class="Electrolux_light_bold Electrolux_Color" style="width: 100px;display: inline-block;">Country:</span>
        </asp:Label>

        <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width150px" ID="ddlCountry" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" AutoPostBack="true" OnClientSelectedIndexChanged="ShowLoadingPanel">
        </telerik:RadComboBox>
    </div>
    <div style="margin-top: 5px;">
        <span class="Electrolux_light_bold Electrolux_Color" style="width: 100px; display: inline-block;">Environment:</span>
        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width160px" ID="ddlEnvironment" OnSelectedIndexChanged="ddlEnvironment_SelectedIndexChanged" AutoPostBack="true" OnChange="ShowLoadingPanel();"></asp:DropDownList>
    </div>
    <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="searchPanel" style="margin-top: 5px;" ChildrenAsTriggers="false">
        <ContentTemplate>
            <span class="Electrolux_light_bold Electrolux_Color" style="width: 100px; display: inline-block;">Filter By Name:</span>
            <asp:TextBox ID="SearchCustomer" runat="server" OnTextChanged="SearchCustomer_TextChanged" OnChange="ShowLoadingPanel();" AutoPostBack="true" CssClass="Electrolux_light_bold Electrolux_Color width160px"></asp:TextBox>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="SearchCustomer" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="TradePlaceCustomerPanel" Class="TradePlaceCustomerPanel" ChildrenAsTriggers="false">
        <ContentTemplate>
            <div id="LoadingPanel" style="position: absolute; top: 0; width: 100%; height: 100%; z-index: 1000;  vertical-align: middle;background-color: rgba(0,0,0,0.1);" class="hidden">
                <asp:Image ID="LoadingImage" runat="server" AlternateText="Loading..." ImageUrl="./../Images/Loading.gif" style="position: absolute;top: 50%;left: 50%;margin-left: -32px;margin-top: -32px;z-index: 1001;" />
            </div>
            <div style="position: absolute; top: 0; width: 100%; height: 100%; align-content: center; vertical-align: middle;">
                <asp:DataList ID="dtlTPCustomer" runat="server" Style="width: 370px; height: 500px; overflow: auto; display: inline-block;" CssClass="">
                    <SelectedItemStyle CssClass="SelectedListItem" Wrap="false"></SelectedItemStyle>
                    <HeaderTemplate>
                    </HeaderTemplate>
                    <AlternatingItemStyle Wrap="False" CssClass="TPAlternatingItemStyle"></AlternatingItemStyle>
                    <ItemStyle Wrap="False" CssClass="TPItemStyle"></ItemStyle>
                    <ItemTemplate>
                        <input id="hidID" type="hidden" value='<%# DataBinder.Eval(Container.DataItem,"TPC_ID")%>' name="ID" runat="server">
                        <a onclick="javascript:__doPostBack('TradePlaceCustomer','<%# DataBinder.Eval(Container.DataItem, "TPC_ID") %>');LineClick($(this));"><%# DataBinder.Eval(Container.DataItem, "TPC_Name")%> </a>
                    </ItemTemplate>
                </asp:DataList>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="dtlTPCustomer" />
        </Triggers>
    </asp:UpdatePanel>
</div>
<script>

    //we have to select the selected item but we doesn't use commandName=select(commented line on datalist) so we manage it manually.
    function LineClick(Link) {
        //We delete existing SelectedListItem class on page 
        $(".SelectedListItem").removeClass("SelectedListItem");
        //We Apply css to clicked link
        $(Link).addClass("SelectedListItem");

    }

    function OnSuccess(response, userContext, methodName) {

    }

    function OnFailure(error) {

    }

    function ShowLoadingPanel(sender, eventArgs) {
        $("#LoadingPanel").removeClass("hidden");
    }
</script>
