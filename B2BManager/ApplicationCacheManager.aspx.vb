
Imports System.Diagnostics

Partial Class ApplicationCacheManager
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim __EVENTTARGET As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
        Dim watch As Stopwatch = Stopwatch.StartNew
        Try
            If IsPostBack Then
                If Not String.IsNullOrEmpty(__EVENTTARGET) Then
                    Dim cacheKey As String = __EVENTTARGET.Replace("ctl00$ContentPlaceHolder1$Btn-", "")
                    If Cache(cacheKey) IsNot Nothing Then
                        Cache.Remove(cacheKey)
                        ClsHelper.Log("Cache Manager Remove Item", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Item Key: " & cacheKey, watch.ElapsedMilliseconds, False, Nothing)
                    End If
                End If
            End If

        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try

        RenderCacheItems()

        If Not IsPostBack Then
            ClsHelper.Log("Cache Manager", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Load cache items", watch.ElapsedMilliseconds, False, Nothing)
        End If

    End Sub

    Private Sub RenderCacheItems()
        Dim key As String = Nothing
        If Cache IsNot Nothing Then
            For Each c As DictionaryEntry In Cache
                key = DirectCast(c.Key, String)
                If Not key.Contains("/") AndAlso Not key.Contains("\") Then
                    Dim row As TableRow = New TableRow()
                    Dim cacheItemCell As TableCell = New TableCell()
                    Dim cacheItemDeleteButtonCell As TableCell = New TableCell()
                    Dim cacheItemDeleteButton = New LinkButton()
                    cacheItemCell.Text = key
                    cacheItemDeleteButton.Text = "Delete"
                    cacheItemDeleteButton.ID = "Btn-" & key.Replace(" ", "-")
                    cacheItemDeleteButton.CommandArgument = key
                    cacheItemDeleteButton.CssClass = "btn red"
                    cacheItemDeleteButtonCell.Controls.Add(cacheItemDeleteButton)
                    row.Cells.Add(cacheItemCell)
                    row.Cells.Add(cacheItemDeleteButtonCell)
                    cacheItemsTable.Rows.Add(row)
                    Dim asynPostBackTrigger As AsyncPostBackTrigger = New AsyncPostBackTrigger()
                    asynPostBackTrigger.ControlID = cacheItemDeleteButton.UniqueID
                    UpdatePanel1.Triggers.Add(asynPostBackTrigger)
                End If
            Next
        End If
    End Sub

End Class
