<%@ Control Language="VB" AutoEventWireup="false" CodeFile="EbusinessCustomersGrid.ascx.vb" Inherits="UserControls_EbusinessCustomersGrid" %>
<table style="width: 100%">
    <tr>
        <td id="gridTD" style="vertical-align: top">
            <div id="gridViewContainer" class='gridcontainer' style="margin: 0; min-height: 400px;">
                <div id='temporaryLoadingElement' style='width: 100%; height: 800px; margin-top: -50px' class='loadingBackgroundDefault' />
            </div>
        </td>
        <td id="iframeTD" style="vertical-align: top">
            <div id="iframeContainer" class="hidden">
                <h2 style="color: #BDC0C4; margin-left: 10%; margin-top: 300px;">Search for the entity you want to manage and then press
                    <img src="Images/Edit.png" width="20">
                    icon to display details here</h2>
            </div>
            <iframe style="width: 100%; border: none; height: 960px; overflow-x: hidden; overflow-y: auto" id="iframeDetails" class="hidden"></iframe>
        </td>
    </tr>
</table>
