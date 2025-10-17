
Imports System.Data
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports Telerik.Web.UI

Partial Class B2BCacheManagerDisplayKeyValue
    Inherits System.Web.UI.Page

    Private HIDEN_COLUMNS As String() = New String() {"ProjectID",
                                                        "ProductID",
                                                        "LanguageID",
                                                        "PromotionID",
                                                        "TxtPromotion",
                                                        "Promotional",
                                                        "Thumbnail",
                                                        "EnergyLabelUrl",
                                                        "EnergyClassUrl ",
                                                        "ProductFicheUrl",
                                                        "ProdClass",
                                                        "Picturex250",
                                                        "BrandID",
                                                        "BrandPositionID",
                                                        "EdenCategoryID",
                                                        "ProdShortDesc"
                                                        }
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            CType(Master.FindControl("title"), HtmlTitle).Text = "Cache Key Content"
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim instancePath As String = Request.QueryString("instancePath")
            Dim keyName As String = Request.QueryString("keyName")
            Dim KeyType As String = Request.QueryString("KeyType")
            Dim countryName As String = Request.QueryString("countryName")
            Dim countrySop As String = Request.QueryString("sopid")
            Dim guid As Guid = Guid.NewGuid()
            dataSourceGUID.Value = guid.ToString()

            If Not String.IsNullOrEmpty(countryName) And Not String.IsNullOrEmpty(countrySop) Then
                CountryDetails.InnerHtml = "<table><tr><td><img src=""Images/FlagsSop/" + countrySop + ".png"" width=""32"" height=""32"" style=""border-radius:3px""/><td style=""padding:5px"">" + countryName + "</td></tr></table>"
            End If

            If Not String.IsNullOrEmpty(keyName) Then
                cacheKeyNameLbl.Text = keyName
            End If

            If Not String.IsNullOrEmpty(KeyType) Then
                cacheKeyTypeLbl.Text = KeyType
            End If

            Dim CacheValue As Object = GetCacheValue()
            If CacheValue IsNot Nothing Then
                If KeyType.ToLower().Equals("datatable") Or KeyType.ToLower().StartsWith("dictionary") Then
                    gridResult.DataSource = CacheValue
                    gridResult.DataBind()
                    For Each columnName As String In HIDEN_COLUMNS
                        Dim gridColumn As GridColumn = gridResult.MasterTableView.GetColumnSafe(columnName)
                        If gridColumn IsNot Nothing Then
                            gridColumn.Visible = False
                        End If
                    Next
                Else
                    HtmlInfo.InnerHtml = CacheValue.ToString()
                End If
            Else
                HtmlInfo.InnerHtml = "<span style='color:red'>Empty value</span>"
            End If
        End If
    End Sub

    Protected Function GetCacheValue() As Object
        If Cache("CacheValue-" + dataSourceGUID.Value) IsNot Nothing Then
            Return Cache("CacheValue-" + dataSourceGUID.Value)
        Else
            Dim instancePath As String = Request.QueryString("instancePath")
            Dim keyName As String = Request.QueryString("keyName")
            Dim KeyType As String = Request.QueryString("KeyType")
            If Not String.IsNullOrEmpty(instancePath) AndAlso Not String.IsNullOrEmpty(keyName) AndAlso Not String.IsNullOrEmpty(KeyType) Then
                Try
                    If keyName = "PriceCache" Then
                        Dim dt As DataTable = GetpriceCache()
                        Return dt
                    Else

                        Dim webservice As WebCacheManager.WebCacheManagerExtended = New WebCacheManager.WebCacheManagerExtended()
                        webservice.Url = instancePath + "/WebCacheManager.asmx"
                        Dim value As Object = webservice.GetCacheValueByKeyName(keyName)
                        If value.ToString().Equals(String.Empty) Then
                            Return Nothing
                        Else
                            If KeyType.ToLower().Equals("datatable") Or KeyType.ToLower().StartsWith("dictionary") Then
                                Dim ds As DataSet = New DataSet()
                                ds.ReadXml(New StringReader(value))
                                gridResult.DataSource = ds.Tables(0)
                                Cache("CacheValue-" + dataSourceGUID.Value) = ds.Tables(0)
                                Return ds.Tables(0)
                            Else
                                Cache("CacheValue-" + dataSourceGUID.Value) = value.ToString()
                                Return value.ToString()
                            End If
                        End If

                    End If
                Catch ex As Exception
                    HtmlInfo.InnerHtml = ex.Message + "</br>" + ex.StackTrace
                Return Nothing
                End Try
            Else
                Return Nothing
            End If
        End If

    End Function


    Protected Sub gridResult_NeedDataSource(source As Object, e As GridNeedDataSourceEventArgs)
        Dim KeyType As String = Request.QueryString("KeyType")
        If KeyType.ToLower().Equals("datatable") Or KeyType.ToLower().StartsWith("dictionary") Then
            gridResult.DataSource = GetCacheValue()
        End If
    End Sub

    Protected Function GetpriceCache() As DataTable
        Dim Sop As String = Request.QueryString("sopid")
        Dim Env As Integer = Request.QueryString("envid")
        Dim ds As DataSet = ClsDataAccessHelper.FillDataSet("[Ebusiness].[UsrMgmt_GET_PriceCache]",
                          New List(Of SqlClient.SqlParameter)(New SqlClient.SqlParameter() {New SqlClient.SqlParameter("@SOPNAME", Sop),
                                                                                            New SqlClient.SqlParameter("@EnvironmentID", Env)}))
        Return ds.Tables(0)
    End Function
End Class
