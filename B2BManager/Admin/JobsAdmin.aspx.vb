Imports System
Imports System.Text

Partial Class Admin_JobsAdmin
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindJobs()
        End If
    End Sub

    Private Sub BindJobs()
        Dim jobs = LongRunningJobs.GetAllJobs()
        Dim sb As New StringBuilder()
        sb.AppendLine("<table style='width:100%; border-collapse:collapse;'>")
        sb.AppendLine("<thead><tr><th>Id</th><th>Status</th><th>StartedAt</th><th>CompletedAt</th><th>Result</th><th>Error</th><th>Actions</th></tr></thead>")
        sb.AppendLine("<tbody>")
        For Each j In jobs
            sb.AppendLine("<tr>")
            sb.AppendFormat("<td>{0}</td>", j.Id)
            sb.AppendFormat("<td>{0}</td>", j.Status)
            sb.AppendFormat("<td>{0}</td>", If(j.StartedAt.HasValue, j.StartedAt.Value.ToString("s"), ""))
            sb.AppendFormat("<td>{0}</td>", If(j.CompletedAt.HasValue, j.CompletedAt.Value.ToString("s"), ""))
            sb.AppendFormat("<td><pre style='max-height:120px;overflow:auto'>{0}</pre></td>", Server.HtmlEncode(j.Result))
            sb.AppendFormat("<td><pre style='max-height:120px;overflow:auto'>{0}</pre></td>", Server.HtmlEncode(j.ErrorMessage))
            sb.AppendFormat("<td><form method='post'><input type='hidden' name='removeId' value='{0}'/><input type='submit' name='remove' value='Remove' /></form></td>", j.Id)
            sb.AppendLine("</tr>")
        Next
        sb.AppendLine("</tbody></table>")
        phJobs.Controls.Clear()
        phJobs.Controls.Add(New LiteralControl(sb.ToString()))
    End Sub

    Protected Sub btnRefresh_Click(sender As Object, e As EventArgs)
        BindJobs()
    End Sub

    Protected Sub btnPurgeCompleted_Click(sender As Object, e As EventArgs)
        Dim jobs = LongRunningJobs.GetAllJobs()
        For Each j In jobs
            If j.Status = LongRunningJobs.JobStatus.Completed OrElse j.Status = LongRunningJobs.JobStatus.Failed Then
                LongRunningJobs.RemoveJob(j.Id)
            End If
        Next
        BindJobs()
    End Sub

    Protected Overrides Sub OnLoadComplete(e As EventArgs)
        MyBase.OnLoadComplete(e)
        If Request.HttpMethod = "POST" AndAlso Not String.IsNullOrEmpty(Request.Form("remove")) Then
            Dim idStr = Request.Form("removeId")
            Try
                Dim id = Guid.Parse(idStr)
                LongRunningJobs.RemoveJob(id)
            Catch
            End Try
            BindJobs()
        End If
    End Sub
End Class
