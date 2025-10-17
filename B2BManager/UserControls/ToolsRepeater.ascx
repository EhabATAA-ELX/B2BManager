<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ToolsRepeater.ascx.vb" Inherits="UserControls_ToolsRepeater" %>
<div class="card"> 
<div class="card-header">
    <asp:Image runat="server" ID="toolImage" Width="36" Height="36" />
    <span runat="server" id="toolName" style="font-family: 'Electrolux Sans'; font-variant: small-caps"></span>
</div>
<div class="tool-dashboard card-body">
    <asp:Repeater runat="server" ID="toolsReperter">
        <ItemTemplate>
            <%# IIf(CInt(Eval("TypeID")) = 2 AndAlso Not ClsHelper.IsDebugMode, "<div class=""icon-btn-tool custom-info-container"" title=""" & Eval("Name") &
                                                                                                                        """ style=""display: inline-block;""> " &
                                                                                                                    "<img src=""" & Eval("IconImagePath") & """ /> " &
                                                                                                                    "<div title=""" & Eval("Name") & """ class=""name"">" & Eval("Name") & "</div>" &
                                                                                                                    "<span class=""custom-info""><span>Coming soon</span></span></div>", "<div class=""position-relative tool-container"">" &
                                                                                                                    IIf(String.IsNullOrEmpty(Eval("BadgeText")) Or String.IsNullOrEmpty(Eval("BadgeColor")), "", "<a class=""ribbon-wrapper"" style=""cursor:pointer;"" href=""" + Eval("Url") + """><div style=""background-color:" + Eval("BadgeColor") + " !important"" class=""ribbon bg-primary"" >" + Eval("BadgeText") + "</div></a>") &
                                                                                                                    "<a class=""icon-btn-tool clickable"" href=""" &
                                                                                                                    Eval("Url") & """ title=""" & Eval("Name") & """ style=""display: inline-block;""> " &
                                                                                                                    "<img src=""" & Eval("IconImagePath") & """ /> " &
                                                                                                                    "<div title=""" & Eval("Name") & """ class=""name"">" & Eval("Name") & "</div></a></div>") %>
        </ItemTemplate>
    </asp:Repeater>
</div>
</div>