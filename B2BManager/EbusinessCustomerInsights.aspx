<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" Async="true" EnableSessionState="False" CodeFile="EbusinessCustomerInsights.aspx.vb" Inherits="EbusinessCustomerInsights" %>
<%@ Register Src="~/UserControls/CustomerInsights.ascx" TagPrefix="uc1" TagName="CustomerInsights" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script src="Scripts/ECharts/echarts.common.min.js"></script>
    <link href="CSS/Insights.css" rel="stylesheet" />
    <script src="Scripts/Insights.js?v=2"></script>
    <script type="text/javascript">
        function LoadCustomerInsightsValues() {   
            if (!CustomerOrderingValuesLoaded) {
                $.ajax({
                    type: 'Get',
                    url: 'B2BManagerService.svc/GetCustomerInsightsValues',
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: { envid: <%= Request.QueryString("envid") %>, cid: '<%= Request.QueryString("cid") %>' },
                    async: true,
                    success: function (response) {
                        CustomerOrderingLoadTrials++;                                                
                        if (response != null) {
                            CustomerOrderingValuesLoaded = true;
                            clearInterval(intervalId);
                            if (response.length >= 2) {
                                $("#MostActiveUser").html(response[0]);
                                $("#OrdersPlaced").html(response[1]);
                            }
                            else {
                                $("#MostActiveUser").text("--");
                                $("#OrdersPlaced").text("--");
                            }
                        }
                    },
                    error: function (e) {
                        CustomerOrderingLoadTrials++;
                    }
                });
            }
            if (CustomerOrderingLoadTrials >= 44 && !CustomerOrderingValuesLoaded) {
                $("#MostActiveUser").html("<i class='fas fa-bomb'></i> Error");
                $("#OrdersPlaced").html("<i class='fas fa-bomb'></i> Error");
                CustomerOrderingValuesLoaded = true;
                clearInterval(intervalId);
            }
            
        }

        function LoadCustomerUserCountsValues() {
            $.ajax({
                type: 'Get',
                url: 'B2BManagerService.svc/GetCustomerUserCounts',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: { envid: <%= Request.QueryString("envid") %>, cid: '<%= Request.QueryString("cid") %>' },
                async: true,
                success: function (response) {
                    if (response.length >= 2) {
                        $("#usersCount").html(response[0]);
                        $("#superUsersCount").html(response[1]);                                                    
                    }
                    else {
                        $("#usersCount").text("--");
                        $("#superUsersCount").text("--");
                    }
                },
                error: function (e) {
                    console.log("Error  : " + e.statusText);
                    $("#usersCount").html("<i class='fas fa-bomb'></i> Error");
                    $("#superUsersCount").html("<i class='fas fa-bomb'></i> Error");
                }
            });
        }

        var CustomerOrderingValuesLoaded = false;
        var CustomerOrderingLoadTrials = 0;
        var intervalId;
        $(document).ready(function () {        
            LoadCustomerUserCountsValues();

            intervalId = setInterval(function () {
                if (!CustomerOrderingValuesLoaded && CustomerOrderingLoadTrials < 45) {
                    LoadCustomerInsightsValues();
                }
                else {
                    clearInterval(intervalId);
                }
            }, 1200);
        });
    </script>
    <style>
        .minWidth600px {
            width: 100% !important;
            min-width: 600px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:customerinsights runat="server" id="CustomerInsights" />
    <asp:Panel runat="server" CssClass="card" ID="chartPnlContainer"></asp:Panel>
</asp:Content>

