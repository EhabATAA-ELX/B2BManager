<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="B2BCacheManagerClearConfirmation.aspx.vb" Inherits="B2BCacheManagerClearConfirmation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
    <style type="text/css">
        .infodiv{
            margin: 15px;
            border: solid 0.5px #ccc;
            padding: 5px;
            height: 650px;
            overflow-y:auto;
        }
        .HeaderConfirmation {
            margin-left:15px;
            margin-top:10px;
            font-size:10pt;
            font-weight:bold;
        }
    </style>
    <script type="text/javascript">
        function ClearKeys() {
            $("#btnConfirm").remove();
            $("#btnClose").val("Close");
            $('span[id^="clear-"]').each(function () {
                $(this).text("Clearing ...");
                var dataUrlPath = $(this).attr("data-instance-path");
                var keyName = $(this).attr("data-key-name");
                var requestid = $(this).attr("data-request-id");
                var env = $(this).attr("data-key-env");
                var sop = $(this).attr("data-key-sop");
                var id = $(this).attr("id");
                if (keyName == 'Price Cache') {
                     $.ajax({
                            type: 'Get',
                            url: 'WebCacheManagerService.svc/ClearPriceCache',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: { keyName : keyName, id: id , Sop : sop ,Env:env },
                            async: true,
                                success: function (response) {
                                    if (response._HasError) {
                                        $("#" + response._id).html("<img src='Images/Error.png' width='20' />");
                                    }
                                    else {
                                        $("#" + response._id).html("<img src='Images/Success.png' width='20' />");
                                    }                    
                                },
                                error: function (e) {
                                    console.log("Error  : " + e.statusText);
                                }
                    });
                } else {
                    $.ajax({
                            type: 'Get',
                            url: 'WebCacheManagerService.svc/ClearKey',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: { url: dataUrlPath,keyName : keyName, id: id , requestId : requestid },
                            async: true,
                                success: function (response) {
                                    if (response._HasError) {
                                        $("#" + response._id).html("<img src='Images/Error.png' width='20' />");
                                    }
                                    else {
                                        $("#" + response._id).html("<img src='Images/Success.png' width='20' />");
                                    }                    
                                },
                                error: function (e) {
                                    console.log("Error  : " + e.statusText);
                                }
                    });
                }
              
            });
        }

        function CloseWindow() {
            if (typeof window.parent.CloseWindow == "function") {
                window.parent.CloseWindow();
            }
            else {
                window.close();
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <br />
    <span class="HeaderConfirmation">You are about to clear the below cache key value(s):</span>
    <div runat="server" class="infodiv"  id="infoDiv"></div>
    <table align="center">
        <tr>
            <td>
                <input type="button" value="Cancel" id="btnClose" onclick="window.parent.CloseWindow()" class="btn bleu" />
                <input type="button" value="Confirm" id="btnConfirm" class="btn red" onclick="ClearKeys()" />
            </td>
        </tr>
    </table>
</asp:Content>

