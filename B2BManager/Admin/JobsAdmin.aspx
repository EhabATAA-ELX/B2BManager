<%@ Page Language="VB" AutoEventWireup="false" CodeFile="JobsAdmin.aspx.vb" Inherits="Admin_JobsAdmin" %>
<!DOCTYPE html>
<html>
<head>
    <title>Jobs Admin</title>
    <meta charset="utf-8" />
    <style>
        .jobs-table { width: 100%; border-collapse: collapse; }
        .jobs-table th, .jobs-table td { border: 1px solid #ddd; padding: 8px; }
        .jobs-table th { background: #f8f8f8; }
    </style>
</head>
<body>
    <h2>Jobs Admin</h2>
    <asp:Button ID="btnRefresh" runat="server" Text="Refresh" OnClick="btnRefresh_Click" />
    <asp:Button ID="btnPurgeCompleted" runat="server" Text="Purge Completed" OnClick="btnPurgeCompleted_Click" />
    <asp:PlaceHolder ID="phJobs" runat="server" />
    <table class="jobs-table" id="tblJobs" runat="server">
        <thead>
            <tr><th>Id</th><th>Status</th><th>StartedAt</th><th>CompletedAt</th><th>Result</th><th>Error</th><th>Actions</th></tr>
        </thead>
        <tbody>
        </tbody>
    </table>
</body>
</html>
