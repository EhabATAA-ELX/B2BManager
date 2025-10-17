<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="ManageFileEntries.aspx.vb" Inherits="ManageFileEntries" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script src="Scripts/InsightsAreas.js?v=1.0"></script>

    <style type="text/css">
        .rlvDrag {
            -webkit-text-size-adjust: 100%;
            -webkit-font-smoothing: antialiased;
            box-sizing: inherit;
            width: 30px;
            height: 30px;
            border: 0;
            padding: 0;
            background-color: transparent;
            background-repeat: no-repeat;
            vertical-align: middle;
            background-position: center center;
            cursor: move;
            background-image: url('Images/drag.png');
        }

        .rlvI {
            width: 400px;
            height: 100px;
            border: 1px solid #efefef !important;
        }

        .rlvI:hover {
            border: 2px dashed #4282c4 !important;
        }

        .entries-container li {
            padding: 5px;
        }

        .entries-container {
            list-style: none;
        }


        .column.dragElem {
            opacity: 0.4;
        }

        .column.over {
            border-top: 1px dashed grey;
        }

        .column-draggable {
            cursor: move !important;
        }

        [draggable] {
            -moz-user-select: none;
            -khtml-user-select: none;
            -webkit-user-select: none;
            user-select: none;
            /* Required to make elements draggable in old WebKit */
            -khtml-user-drag: element;
            -webkit-user-drag: element;
        }
    </style>
    <script type="text/javascript">

        $(document).ready(function () {
            var entries = document.querySelectorAll('.entries-container .column');
            [].forEach.call(entries, addDnDHandlers);
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <asp:UpdatePanel runat="server" ID="UpdatePanel1" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div style="width: 100%;">
                <div style="width: 450px; float: left">
                    <div class="Electrolux_light_bold Electrolux_Color" style="margin-left: 48px;" runat="server" id="DragAndDropHeader">Drag and drop file entries to change display order and then save</div>
                    <ul id="container" class="entries-container" style="width: 450px">
                        <asp:Repeater ID="RadListView1" runat="server" DataSourceID="SqlDataSource1">
                            <ItemTemplate>
                                <li class="column column-draggable" draggable="true">
                                    <table class="rlvI" cellpadding="5px" cellspacing="5px">
                                        <tr>
                                            <td class="Electrolux_light_bold Electrolux_Color width180px">Entry name:
                                            </td>
                                            <td>
                                                <span class="Electrolux_light_bold" style="font-weight: bold; color: red">
                                                    <%# Eval("FileEntryName") %>
                                                </span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="Electrolux_light_bold Electrolux_Color width180px">Extension filter:</td>
                                            <td class="Electrolux_light_bold Electrolux_Color"><%# Eval("ExtensionFilter") %></td>
                                        </tr>
                                        <tr>
                                            <td class="Electrolux_light_bold Electrolux_Color width180px">Search in sub-directories:</td>
                                            <td class="Electrolux_light_bold Electrolux_Color">
                                                <asp:CheckBox runat="server" Enabled="false" ID="chkboxAllDirectories" Checked='<%# Eval("AllDirectories") %>' /></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" style="text-align: right" draggable="false">
                                                <span class="defaultLink allow-drop" draggable="false" style="margin: 5px">Edit</span>
                                                <span class="defaultLink allow-drop" draggable="false" style="margin: 5px">Delete</span>
                                            </td>
                                        </tr>
                                    </table>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                    <span style="margin-left: 100px">
                        <a class="btn lightblue" id="btnSaveLinksPositions" runat="server" onclick="SaveSectionPositions(this)"><i class="fas fa-check"></i>Save Positions</a>
                        <a class="btn bleu" id="btnAddLink" onclick="NewFileEntry()"><i class="far fa-file"></i>New File Entry</a>
                    </span>
                </div>
                <div style="width: 700px; height: 500px; border: 1px solid red; float: left">
                    <table cellpadding="5px" cellspacing="5px">
                        <tr>
                            <td class="Electrolux_light_bold Electrolux_Color width180px">Entry name:
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtBoxEntryName"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="Electrolux_light_bold Electrolux_Color width180px">Extension filter:</td>
                            <td class="Electrolux_light_bold Electrolux_Color">
                                <asp:TextBox runat="server" ID="txtBoxExtensionFilter"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="Electrolux_light_bold Electrolux_Color width180px">Search in sub-directories:</td>
                            <td class="Electrolux_light_bold Electrolux_Color">
                                <asp:CheckBox runat="server" ID="chkBoxAllDirectories" />    
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" style="text-align: right" draggable="false">
                                <span class="defaultLink allow-drop" draggable="false" style="margin: 5px">Save</span>
                            </td>
                        </tr>
                    </table>
                    <ul class="environments-container" style="width: 650px">
                        <asp:Repeater ID="Repeater1" runat="server" DataSourceID="SqlDataSource2">
                            <ItemTemplate>
                                <li class="column column-draggable" draggable="true">
                                    <table class="rlvI" cellpadding="5px" cellspacing="5px">
                                        <tr>
                                            <td class="Electrolux_light_bold Electrolux_Color width180px">Entry name:
                                            </td>
                                            <td>
                                                <span class="Electrolux_light_bold" style="font-weight: bold; color: red">
                                                    <%# Eval("FileEntryName") %>
                                                </span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="Electrolux_light_bold Electrolux_Color width180px">Extension filter:</td>
                                            <td class="Electrolux_light_bold Electrolux_Color"><%# Eval("ExtensionFilter") %></td>
                                        </tr>
                                        <tr>
                                            <td class="Electrolux_light_bold Electrolux_Color width180px">Search in sub-directories:</td>
                                            <td class="Electrolux_light_bold Electrolux_Color">
                                                <asp:CheckBox runat="server" Enabled="false" ID="chkboxAllDirectories" Checked='<%# Eval("AllDirectories") %>' /></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" style="text-align: right" draggable="false">
                                                <span class="defaultLink allow-drop" draggable="false" style="margin: 5px">Edit</span>
                                                <span class="defaultLink allow-drop" draggable="false" style="margin: 5px">Delete</span>
                                            </td>
                                        </tr>
                                    </table>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:SqlDataSource runat="server" ID="SqlDataSource1" EnableCaching="false" SelectCommand="FilesViewer.GetFileEntries" SelectCommandType="StoredProcedure" ConnectionString="<%$ ConnectionStrings:LogDB %>"></asp:SqlDataSource>
    <asp:SqlDataSource runat="server" ID="SqlDataSource2" EnableCaching="false" SelectCommand="FilesViewer.GetFileEntries" SelectCommandType="StoredProcedure" ConnectionString="<%$ ConnectionStrings:LogDB %>"></asp:SqlDataSource>
</asp:Content>

