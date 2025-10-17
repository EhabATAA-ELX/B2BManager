<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="Home.aspx.vb" Inherits="Home" %>

<%@ Register Src="~/UserControls/ToolsRepeater.ascx" TagPrefix="uc1" TagName="ToolsRepeater" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script src="Scripts/ECharts/echarts.common.min.js"></script>
    <link href="CSS/Insights.css" rel="stylesheet" />
    <script src="Scripts/Insights.js?v=1.5"></script>
    <script type="text/javascript">
        function imageBtnRefreshClick() {
            $.each($(".image-btn-refresh"), function (index, value) {
                $(value)[0].src = "Images/Loader.gif";
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div style="text-align: left;" class="row">
        <div class="col-lg-7">
            <uc1:ToolsRepeater runat="server" ID="ToolsRepeater" Title="Tools" />
        </div>
        <div class="col-lg-5">
            <asp:UpdatePanel runat="server" ID="UpdatePanel2">
                <ContentTemplate>
                    <div class="card">
                        <div class="card-header">
                            <div class="d-flex justify-content-between">
                                <span style="font-family: 'Electrolux Sans'; font-variant: small-caps">Recent Actions</span>
                                <asp:ImageButton runat="server" ID="ImageButton1" ImageUrl="Images/Reload.png" CssClass="image-btn-refresh" Width="26" Height="26" ToolTip="Refresh" OnClientClick="imageBtnRefreshClick()" />
                            </div>                            
                        </div>
                        <div class="card-body recent-activities">
                             <table width="100%">
                            <asp:Repeater runat="server" ID="RecentActionsRepeater">
                        <ItemTemplate>
                            <tr>
                                <td style="vertical-align: middle; background-color: #fdfdfd; padding-right: 15px;">
                                    <a href="<%# Eval("Url") %>">
                                        <i class="far fa-clock"></i>
                                        <span style="margin-left: 3px; line-height: 34px; vertical-align: middle"><%# Eval("Name") %></span>
                                    </a>
                                    <span style="float: right;line-height: 34px; color: #1b80b7; vertical-align: middle;"><%# "visited " + ClsHelper.ToReadableString(DateTime.Now - CDate(Eval("LoggedOn"))) %></span>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <tr>
                                <td style="vertical-align: middle; background-color: #fafafa; padding-right: 15px;">
                                    <a href="<%# Eval("Url") %>">
                                        <i class="far fa-clock"></i>
                                        <span style="margin-left: 3px; line-height: 34px; vertical-align: middle"><%# Eval("Name") %></span>
                                    </a>
                                    <span style="float: right;line-height: 34px; color: #1b80b7; vertical-align: middle;"><%# "visited " + ClsHelper.ToReadableString(DateTime.Now - CDate(Eval("LoggedOn"))) %></span>
                                </td>
                            </tr>
                        </AlternatingItemTemplate>
                    </asp:Repeater>
                                 </table>
                        </div>
                    </div>                    
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="imageBtnRefresh" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
    <asp:UpdatePanel runat="server" ID="updatePanel1">
        <ContentTemplate>
            <div class="row">
                <div class="col-lg-7" >
                    <div class="card">
                        <div runat="server" id="tblDefaultChart"  class="default-chart card-body"></div>
                    </div>                    
                </div>
                <div id="tblRecentActivities" runat="server" class="col-lg-12">
                    <div class="card">
                        <div class="card-header">
                            <div class="d-flex justify-content-between">
                                <span style="font-family: 'Electrolux Sans'; font-variant: small-caps">Recent Activities</span>
                                <asp:ImageButton runat="server" ID="imageBtnRefresh" ImageUrl="Images/Reload.png" Width="26" Height="26" ToolTip="Refresh" CssClass="image-btn-refresh"  OnClientClick="imageBtnRefreshClick()" />
                            </div>                            
                        </div>

                        <div runat="server"  style="padding-left: 15px; padding-right: 15px" class="card-body recent-activities" cellpadding="2" cellspacing="5">
                            <table width="100%">
                                <asp:Repeater runat="server" ID="recentActivitiesRepeter">
                                    <ItemTemplate>
                                        <tr>
                                            <td style="vertical-align: middle; background-color: #fdfdfd; height: 24px; padding-right: 15px;">
                                                <img src="<%# Eval("CorrespondantImagePath") %>" style="width: 35px; float: left; border-radius: 10px" />
                                                <span style="float: left; margin-left: 15px; line-height: 34px; vertical-align: middle"><%# Eval("FriendlyTextDescription") %></span>
                                                <span style="float: right; line-height: 34px; color: #1b80b7; vertical-align: middle;"><%# CDate(Eval("LoggedOn")).ToString("dd/MM/yyyy HH:mm")  %></span>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <AlternatingItemTemplate>
                                        <tr>
                                            <td style="vertical-align: middle; background-color: #fafafa; height: 24px; padding-right: 15px;">
                                                <img src="<%# Eval("CorrespondantImagePath") %>" style="width: 35px; float: left; border-radius: 10px" />
                                                <span style="float: left; margin-left: 15px; line-height: 34px; vertical-align: middle"><%# Eval("FriendlyTextDescription") %></span>
                                                <span style="float: right; line-height: 34px; color: #1b80b7; vertical-align: middle;"><%# CDate(Eval("LoggedOn")).ToString("dd/MM/yyyy HH:mm")  %></span>
                                            </td>
                                        </tr>
                                    </AlternatingItemTemplate>
                                </asp:Repeater>
                            </table>
                        </div>
                    </div>
                </div>                
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="imageBtnRefresh" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>

