<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="UsersManagement.aspx.vb" Inherits="UsersManagement" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="Scripts/DataTables/datatables.min.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/DataTables/datatables.min.js"></script>
    <link href="CSS/jquery-ui.css" rel="stylesheet" /> 
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <link href="CSS/UsersManagement.css" rel="stylesheet" />
    <script src="Scripts/UsersManagement.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ClientIDMode="Static" runat="server" ID="userglobalid" />
    <br />
    <table style="width: 100%">
        <tr>
            <td style="width: 450px; border-right: 1px solid #BDC0C4; padding-left: 25px" valign="top">
                <asp:UpdatePanel runat="server" ID="UpdatePanel1">
                    <ContentTemplate>
                        <table cellpadding="5" cellspacing="5">
                            <tr>
                                <td style="text-align: center">
                                    <input type="button" value="New Group" class="btn lightblue" onclick="CreateGroup()" />
                                    <input type="button" value="New User" class="btn bleu" onclick="CreateUser()" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color width180px">Filters</asp:Label>
                                    <span style="font-size: 8pt">Click on any line box in below to change/apply filters</span>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 450px;">
                                    <div runat="server" id="listGroupDefault" class="list-group groups">
                                    </div>
                                </t>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color width180px">Groups</asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div runat="server" id="listGroup" class="list-group groups">
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color width180px">Countries</asp:Label>
                                    <span id="lblInfo" style="font-size: 8pt">(Showing users assigned to one of the below selected countries)</span>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div runat="server" id="Div1" class="list-group countries">
                                        <span class="list-group-item selected-row cursor-pointer" id="spanOneOfSelectedCountries" onclick="ApplyCountriesFilter(false)">Assigned to one of the below selected countries
                                            <img src="Images/filter.png" width="20" height="20" class="filterImg" />
                                        </span>
                                        <span class="list-group-item cursor-pointer" id="spanAllSelectedCountries" onclick="ApplyCountriesFilter(true)">Assigned to all below selected countries
                                            <img src="Images/filter.png" width="20" height="20" class="filterImg" />
                                        </span>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <telerik:RadTreeView runat="server" CheckBoxes="true" ID="treeCountries" BorderStyle="None" Width="430" CssClass="CountriesTree" TriStateCheckBoxes="true" CheckChildNodes="true">
                                    </telerik:RadTreeView>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td align="center" class="verticalAlignTop">
                <asp:Panel runat="server" ID="panelVisualizeInfo">
                    <table id="example" class="display" style="width: 100%">
                        <thead>
                            <tr>
                                <th></th>
                                <th>Actions</th>
                                <th>ID</th>
                                <th>Full Name</th>
                                <th>Email</th>
                                <th>Login</th>
                                <th>Status</th>
                                <th>Last connected on</th>
                            </tr>
                        </thead>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>


    <div id="dialog-confirm-delete" title="Delete Confirmation" class="DisplayNone">
        <div id="dialog-delete-text" style="margin:15px"></div>
        <span id="dialog-error-info" style="color:red;height:20px"></span>
        <table align="right">
            <tr>
                <td>
                    <button class="btn bleu" id="btnCancel" onclick="CloseDeleteConfirmationWindow()">Cancel</button>
                </td>
                <td>
                    <button class="btn red" id="btnConfirmDelete">Confirm</button>
                </td>
            </tr>
        </table>
    </div>

    <telerik:RadWindow ClientIDMode="Static" ID="UserProfileWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="900px" Height="800px" runat="server">
    </telerik:RadWindow>
    <telerik:RadWindow ClientIDMode="Static" ID="UsersManagementWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="900px" Height="600px" runat="server">
    </telerik:RadWindow>
    <telerik:RadWindow ClientIDMode="Static" ID="NewGroupWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="900px" Height="400px" runat="server">
    </telerik:RadWindow>
    <telerik:RadWindow ClientIDMode="Static" ID="UserGroupProfileWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="900px" Height="600px" runat="server">
    </telerik:RadWindow>
</asp:Content>
