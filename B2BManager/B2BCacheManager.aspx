<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="B2BCacheManager.aspx.vb" Inherits="B2BCacheManager" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script src="Scripts/jquery-ui.js"></script>
    <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">
    <style type="text/css">
        .largeTiles .store-apps-container {
            margin-left: 12px;
            margin-right: 14px;
        }

        .storeapp-list a {
            color: #278efc !important;
        }

        .ui-widget-header, .ui-corner-all, .ui-widget.ui-widget-content {
            border: none;
        }

        .ui-widget-header {
            background-color: #e5f3f2;
        }

        .largeTiles .store-apps-title {
            font-size: 20px;
            display: block;
            margin-left: 12px;
            margin-top: 25px;
            padding: 15px;
            padding-bottom: 0px;
            background-color: #e5f3f2;
        }

            .largeTiles .store-apps-title span {
                font-size: 16px;
            }

        .largeTiles .storeapp-list {
            margin: 16px 0;
        }

        .largeTiles .storeapp-list {
            padding: 0;
            position: relative;
        }

        .storeapp-list .storeapp, .storeapp-list .folder {
            margin-left: 26px;
            padding-right: 26px;
            padding-left: 0;
            margin-right: 0;
        }

        .largeTiles .folder, .largeTiles .storeapp {
            display: inline-block;
            width: 420px;
            min-height: 50px;
            height: auto;
            margin: 0 0 10px 26px;
            padding: 0 26px 0 0;
            border-right: 1px solid #ccc;
            border-right: 1px solid rgba(204,204,204,0.5);
            -moz-box-sizing: border-box;
            box-sizing: border-box;
        }

        .storeapp {
            overflow: hidden;
            vertical-align: top;
        }

        .storeapp, .folder {
            position: relative;
        }

        .redFont {
            color: red;
        }

        .progress-bar {
            display: block;
            margin-left: 12px;
            height: 15px;
        }

        .MassActionSelect{
            font-size:10pt;
        }
    </style>
    <script>
        var table;
        function GetValue(dataServerID) {
            var instanceCount = $('div[data-server-id="' + dataServerID + '"]').length;
            var instanceInLoadingCount = $('div.loading[data-server-id="' + dataServerID + '"]').length;
            var instanceName = " instance" + ((instanceCount > 1) ? "s" : "");
            var emptyInstances = $('span[id="keys-count-' + dataServerID + '"]').length;
            if (instanceInLoadingCount == 0) {
                $('.MassActionSelect[data-server-id="' + dataServerID + '"]').removeClass("hidden");
            }
            else {
                $('.MassActionSelect[data-server-id="' + dataServerID + '"]').addClass("hidden");
            }
            $('span[id="summary-' + dataServerID + '"]').html(" (cache keys were retrieved for " + ((instanceInLoadingCount == 0) ? ((instanceCount == 1) ? "the" : "all") : (instanceCount - instanceInLoadingCount).toString() + " / " + instanceCount)
                + instanceName + ")" + ((emptyInstances > 0) ? " <span style='color:#c59705;'><img style='vertical-align:top;margin-top:3px;margin-right:3px' src='Images/Warning.png' width='18' height='18' />"
                    + emptyInstances.toString() + ((emptyInstances > 1) ? " haven't been active for a while and have an empty cache" : " hasn't been active for a while and has an empty cache") : ""));
            return ((instanceCount - instanceInLoadingCount) * 100) / instanceCount;
        }

        function RefreshCacheValues(id, dataUrlPath) {
            $("#cacheKeys-" + id).addClass("loading");
            $("#cacheKeys-" + id).html("<img src=\"Images/Loading.gif\" width=\"24\" height=\"24\" style=\"vertical-align: top;margin-right:5px\" />Loading cache values...</div>");
            LoadCacheValues("cacheKeys-" + id, dataUrlPath);
        }

        function GenerateDropDownList(cacheKeyName,id,serverId) {
            return "<select class='InstanceSelect' id='Select-"+id+"' data-id='"+id+"' data-cache-Key-name='"+cacheKeyName+"' data-server-id='"+serverId+"'><option value=\"Actions\">--Select action--</option><option value=\"Clear\">Clear</option><option value=\"ClearAll\">Clear in all servers</option></select>";
        }

        function GenerateDropDownListPriceCache(cacheKeyName,id,serverId) {
            return "<select class='InstanceSelect MarginLeft7px' id='Select-"+id+"' data-id='"+id+"' data-cache-Key-name='"+cacheKeyName+"' data-server-id='"+serverId+"'><option value=\"Actions\">--Select action--</option><option value=\"ClearAll\">Clear in all servers</option></select>";
        }

        function LoadCacheValues(id, dataUrlPath) {
            $.ajax({
                type: 'Get',
                url: 'WebCacheManagerService.svc/GetKeysByInstanceUrl',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: { url: dataUrlPath, id: id },
                async: true,
                success: function (response) {
                    var instanceid = $("#" + response._id).attr("data-instance-id");
                    var serverid=  $("#" + response._id).attr("data-server-id")              
                    var htmlKeys = "<b>Instance URL: <a href='" + $("#" + response._id).attr("data-instance-path") + "' target='_blank'>" + $("#" + response._id).attr("data-instance-path") + "</a></b></br><div><b class='priceCache'>Price Cache :</b><a  onclick=LoadContent('PriceCache','" + response._id + "','Datatable')>Details</a>" + GenerateDropDownListPriceCache("Price Cache", '' + instanceid + '', '' + serverid +'' )+"</div>";
                    var htmlKeysTable = "";
                    var ignoredKeys = 0;
                    var ignoredTooltip = "";
                    $("#" + response._id).removeClass();
                    if (!response._HasError) {
                        htmlKeysTable += "<table cellpadding='2px'>"
                        for (var i = 0; i < response._keys.length; i++) {
                            if (response._keys[i].startsWith("JS_Google.Analytics.js")
                                || response._keys[i].startsWith("Archive_")
                                || response._keys[i].startsWith("TelerikScriptManagerUrlsgzip")
                                || response._keys[i].startsWith("UR_REASON_")
                            ) {
                                ignoredKeys++;
                                ignoredTooltip += response._keys[i].replace("|", " [Type: ") + "]\r\n";
                            }
                            else {
                                var values = response._keys[i].split("|");

                                htmlKeysTable += "<tr><td>" + values[0] + "</td>"
                                if (values[0] != "SpecificManagement") {
                                    htmlKeysTable += "<td><a onclick='LoadContent(\"" + values[0] + "\",\"" + response._id + "\",\"" + values[1] + "\")'>Details</a></td>";
                                }
                                else {
                                    htmlKeysTable += "<td title='Display not available for this key'>Details</td>"
                                }

                                 htmlKeysTable += "<td>" + GenerateDropDownList(values[0],$("#" + response._id).attr("data-instance-id"),$("#" + response._id).attr("data-server-id") ) + "</td></tr> ";
                                
                            }
                        }
                        htmlKeysTable += "</table>"
                        var numberOfKeys = (response._keys.length == 0) ? "<span id='keys-count-" + $("#" + response._id).attr("data-server-id") + "'>" + response._keys.length.toString() + "</span>" : response._keys.length.toString();
                        htmlKeys += "<b>Number of keys found: " + numberOfKeys + "</b>" + ((ignoredKeys > 0) ? "<span  title='" + ignoredTooltip + "'>" + ((ignoredKeys == 1) ? " (<b>" + ignoredKeys.toString() + "</b> is ignored in below list)" : " (<b>" + ignoredKeys.toString() + "</b> are ignored in below list)") + "</span>" : "") + "</br>";
                        htmlKeys += htmlKeysTable;
                    }
                    else {
                        htmlKeys = response._ErrorMessage;
                        $("#" + response._id).addClass("redFont");
                    }
                    $("#" + response._id).html(htmlKeys);
                },
                error: function (e) {
                    console.log("Error  : " + e.statusText);
                }
            });
        }

        function LoadAllCacheKeys() {
            $('div.loading[id^="cacheKeys-"]').each(function () {
                var dataUrlPath = $(this).attr("data-instance-path");
                var id = $(this).attr("id");
                LoadCacheValues(id, dataUrlPath);
            });
        }

        function expandOrCollapse(e) {
              var showElementDescription = $('ul[data-server-id='+$(e).attr("data-server-id")+']');
  
              if ($(showElementDescription).is(":visible")) {
                showElementDescription.hide("fast", "swing");
                $(e).attr("src", "Images/right-arrow.png");
              } else {
                showElementDescription.show("fast", "swing");
                $(e).attr("src", "Images/down-arrow.png");
              }
        }

        $(document).ready(function () {            

            setInterval(function () {
                $('div.progress-bar[id^="progress-bar-server-"]').each(function () {
                    var dataServerID = $(this).attr("id").toString().replace("progress-bar-server-", "");
                    var intervalValue = GetValue(dataServerID);
                    $(this).progressbar({
                        value: intervalValue
                    });
                });

                if ($('select option:selected[value^="Clear"]').length > 0) {
                    $("#ExecuteButton").removeClass("hidden");
                }
                else {
                    $("#ExecuteButton").addClass("hidden");
                }

            }, 300);            
                        
            LoadAllCacheKeys();
            
        });

        function MassActionChange(dropdown, serverid) {
            switch ($(dropdown).val()) {
                    case "massActions": return true;
                    case "Translations": $('select.InstanceSelect[data-server-id='+serverid+'][data-cache-key-name*="Translat"] option[value="Clear"]').removeAttr( "selected" ).attr("selected", "selected"); break;
                    case "Specifications": $('select.InstanceSelect[data-server-id='+serverid+'][data-cache-key-name*="SpecificManagement"] option[value="Clear"]').removeAttr( "selected" ).attr("selected", "selected"); break;
                    case "ProductLists": $('select.InstanceSelect[data-server-id='+serverid+'][data-cache-key-name*="ListProduct"] option[value="Clear"],select.InstanceSelect[data-server-id=1][data-cache-key-name*="ListAttribute_"] option[value="Clear"]').removeAttr( "selected" ).attr("selected", "selected");  break;
                    case "Reset": $('select.InstanceSelect[data-server-id="'+serverid+'"] option[value="Actions"]').removeAttr( "selected" ).attr("selected", "selected"); break;
            }
            $(dropdown).val("massActions");
        }


        function GenerateConfirmationRequest() {
            var values = [];
            $('select option:selected[value^="Clear"]').each(function () {
                var value = {
                    id : $(this).closest("select").attr("data-id").toString(),
                    keyname : $(this).closest("select").attr("data-cache-Key-name").toString(),
                    action : $(this).closest("select").val().toString()
                }
                values.push(value);
            });

            var data = { actions: values };

            $.ajax({
                type: 'POST',
                url: 'WebCacheManagerService.svc/GenerateConfirmationRequestKey',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify(data),
                async: true,
                success: function (response) {  
                    OpenConfirmationWindow(response);
                    //$('select.InstanceSelect option[value="Actions"]').removeAttr( "selected" ).attr("selected", "selected");
                },
                error: function (e) {
                    console.log("Error  : " + e.statusText);
                }
            });
            
        }

        function CloseWindow() {
            var oWnd = $find("<%= WindowKeys.ClientID %>");
            oWnd.close();
        }

        function OpenConfirmationWindow(RequestID) {
             var environmentID = $("#ContentPlaceHolder1_ddlEnvironment").val();
             var url = 'B2BCacheManagerClearConfirmation.aspx?RequestID=' + RequestID+'&envid=' + environmentID
            if ($(window).height() > 800 && $(window).width() > 900) {
                var oWnd = $find("<%= WindowKeys.ClientID %>");
                oWnd.setUrl(url + "&HideHeader=true");
                oWnd.set_title('Loading...');
                oWnd.show();
            }
            else {
                popup(url, true);
            }
        }

        function LoadContent(keyName, id, keyType) {
            var instancePath = $("#" + id).attr("data-instance-path");
            var countryName = $("#" + id).attr("data-country-name");
            var SOP_ID = $("#" + id).attr("data-sop-id");
            var environmentID = $("#ContentPlaceHolder1_ddlEnvironment").val();
            var url = 'B2BCacheManagerDisplayKeyValue.aspx?keyName=' + keyName + "&keyType="+keyType+ "&instancePath=" + instancePath + "&countryName=" + countryName +"&sopid=" + SOP_ID+'&envid=' + environmentID;
            if ($(window).height() > 800 && $(window).width() > 900) {
                var oWnd = $find("<%= WindowKeys.ClientID %>");
                oWnd.setUrl(url + "&HideHeader=true");
                oWnd.set_title('Loading...');
                oWnd.show();
            }
            else {
                popup(url, true);
            }
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table class="Filters">
        <tr>
            <td class="width120px">
                <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
            </td>
            <td class="width180px">
                <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlEnvironment" DataTextField="Name" AutoPostBack="true" DataValueField="ID">
                </asp:DropDownList>
            </td>
            <td class="width120px">
                <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
            </td>
            <td class="width180px">
                <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px" AutoPostBack="true" AppendDataBoundItems="true" ID="ddlCountry">
                    <Items>
                        <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
                    </Items>
                </telerik:RadComboBox>
            </td>
            <td style="width: 24px">
                <asp:ImageButton runat="server" ClientIDMode="Static" ID="imageBtnRefresh" ImageUrl="Images/Reload.png" Width="24" Height="24" ToolTip="Refresh" />
            </td>
            <td>
                <input type="button" id="ExecuteButton" onclick="GenerateConfirmationRequest()" class="btn lightblue hidden" value="Execute" />
            </td>
        </tr>
    </table>
    <asp:UpdatePanel runat="server" ID="updatePanel1" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="largeTiles store-view">
                <div class="store-apps-container" id="htmlInfo" runat="server">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" />
            <asp:AsyncPostBackTrigger ControlID="ddlCountry" />
            <asp:AsyncPostBackTrigger ControlID="imageBtnRefresh" />
        </Triggers>
    </asp:UpdatePanel>
    <telerik:RadWindow ID="WindowKeys" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="900px" Height="800px" runat="server">
    </telerik:RadWindow>
</asp:Content>

