<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="FilesViewer.aspx.vb" Inherits="FilesViewer" EnableEventValidation="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="Scripts/DataTables/datatables.min.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/DataTables/datatables.min.js"></script>
    <asp:PlaceHolder runat="server" ID="placeHolderDownloadScript">
        <script type="text/javascript">
            function DownloadFile(fileName) {
                __doPostBack("<%= btnDownload.UniqueID %>", fileName);
            }
        </script>
    </asp:PlaceHolder>
    
    <script type="text/javascript">
        var table;       

        function DisplayFile(fileName) {
            popup("FileViewer.aspx?envid=" + $('#<%= ddlEnvironment.ClientID %>').val() + "&entryid=" + $('#<%= ddlFilesEntryName.ClientID %>').val() + "&fileName=" + fileName);
        }

        function RenderGrid() {
            table = $('#example')
                .on('error.dt', function (e, settings, techNote, message) {
                    console.log('An error has been reported by DataTables: ', message);
                })
                .DataTable({
                "ajax": getAjaxUrl(),
                "iDisplayLength": 15,
                "bLengthChange": false,
                'processing': true,
                'language': {
                    'loadingRecords': '&nbsp;',
                    'processing': 'Loading...'
                },
                "columns": [
                    {
                        "data": "FileName", "render": function (data, type, element) {
                            var displayStr = (element.Size < 20480) ? "Class=\"MoreInfoImg\"  title='View' onclick=\"DisplayFile('" + data.replaceAll('\\', '@') + "')\"" : "Class=\"ImgDisabled\" title='Only files having a size less than 20 MB can be viewed'";
                            var download = "<%= ClsFilesViewerHelper.GetDownloadTemplate() %>";
                            return "<img src=\"Images\\CircularTools\\find.png\" height=\"24px\" " + displayStr + " \/>" + download;
                        },
                        "width": "70px",
                        "searchable": false,
                        "sortable": false
                    },
                    { "data": "FileName" },
                    { "data": "Size" },
                    { "data": "CreationDate" },
                    { "data": "ModificationDate" },
                    { "data": "Extension" }
                ]
            });
            $("#example > thead > tr > th[aria-label^='Last Modification Date']").trigger("click");
            $("#example > thead > tr > th[aria-label^='Last Modification Date']").trigger("click");
        }

        function getAjaxUrl() {
            return "B2BManagerService.svc/GetFilesInfo?envid=" + $('#<%= ddlEnvironment.ClientID %>').val() + "&entryid=" + $('#<%= ddlFilesEntryName.ClientID %>').val();
        }

        function LoadData() {
            table.ajax.url(getAjaxUrl()).load();
        }

        $(document).ready(function () {
            $.fn.dataTable.ext.errMode = 'none';
            if ($('#<%= ddlEnvironment.ClientID %>').val() && $('#<%= ddlFilesEntryName.ClientID %>').val()) {
                RenderGrid();
            }
        });

    </script>
    <style type="text/css">
        .dataTables_filter {
            float: left !important;
            margin-left: 10px;
            margin-bottom: 10px;
        }

        .dataTables_filter {
            text-align: left;
            font-family: Electrolux_light;
            font-weight: bold;
            color: #041d4f;
        }

            .dataTables_filter input {
                float: right;
                width: 180px;
                margin-left: 95px !important;
            }

        table.dataTable.display tbody tr.odd > img {
            background-color: transparent !important;
        }

        .dataTables_processing {
            height: 650px !important;
            top: 80px !important;
            left: 0 !important;
            width: 100%;
            height: 40px;
            margin-top: -25px !important;
            padding-top: 200px !important;
            margin-left:auto !important; 
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:PlaceHolder runat="server" ID="placeHolderContainer">
        <asp:UpdatePanel runat="server" ID="updatePanel1">
            <ContentTemplate>
                <table class="Filters" runat="server" id="filtersTable">
                    <tr>
                        <td class="width130px">
                            <asp:Label runat="server" ID="lblEntryName" CssClass="Electrolux_light_bold Electrolux_Color width130px">Files Entry name:</asp:Label>
                        </td>
                        <td class="width180px">
                            <asp:DropDownList runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFilesEntryName_SelectedIndexChanged" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlFilesEntryName">
                            </asp:DropDownList>
                        </td>
                        <td class="width120px">
                            <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
                        </td>
                        <td class="width180px">
                            <asp:DropDownList runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlEnvironment_SelectedIndexChanged" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="true" ID="ddlEnvironment">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 24px">
                            <asp:ImageButton runat="server" ClientIDMode="Static" ID="imageBtnRefresh" CommandName="Refresh" ImageUrl="Images/Reload.png" Width="24" Height="24" ToolTip="Refresh" OnClick="imageBtnRefresh_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:LinkButton runat="server" ID="btnDownload" OnClick="btnDownload_Click" Visible="false"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnDownload" />
                <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" />
                <asp:AsyncPostBackTrigger ControlID="ddlFilesEntryName" />
                <asp:AsyncPostBackTrigger ControlID="imageBtnRefresh" />
            </Triggers>
        </asp:UpdatePanel>
        <table style="width: 100%" id="gridContainer">
            <tr>
                <td align="center" class="verticalAlignTop">
                    <asp:Panel runat="server" ID="panelVisualizeInfo">
                        <table id="example" class="display" style="width: 100%">
                            <thead>
                                <tr>
                                    <th>Actions</th>
                                    <th>File Name</th>
                                    <th>Size (KB)</th>
                                    <th>Creation Date</th>
                                    <th>Last Modification Date</th>
                                    <th>Extension</th>
                                </tr>
                            </thead>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:PlaceHolder>
    <asp:Label runat="server" ForeColor="Red" Visible="false" ID="EmptyList">You have no entries available in your account please contact your administrator.</asp:Label>
</asp:Content>

