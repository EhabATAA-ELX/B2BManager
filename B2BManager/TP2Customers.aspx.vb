
Imports System.Data
Imports System.Data.SqlClient
Imports ClsHelper
Imports Telerik.Web.UI

Partial Class TP2Customers
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindCustomerData()
            RenderCountryDropdown()
        End If
    End Sub

    Private Sub RenderCountryDropdown()
        Dim applications As List(Of ClsHelper.Application) = Nothing
        If ClsSessionHelper.LogonUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = ClsSessionHelper.LogonUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        Dim defaultSelection As String = IIf(ClsSessionHelper.EbusinessSopID IsNot Nothing, ClsSessionHelper.EbusinessSopID, ClsSessionHelper.LogonUser.DefaultEbusinessSopID)

        ' Convert the comma-separated string to a List
        Dim selectedIds As New List(Of String)(defaultSelection.Split(","c))

        ' Call the new, simpler function
        RenderCountryListBoxForSelect2(lstCountry, selectedApplication.Countries, selectedIds)
    End Sub

    Public Shared Sub RenderCountryListBoxForSelect2(
    listBox As ListBox,
    countries As List(Of Country),
    selectedSopIDs As List(Of String))

        listBox.Items.Clear()

        For Each country As Country In countries
            If country.Checked Then
                Dim item As New ListItem(country.Name, country.SOP_ID)

                ' Add the image URL as a custom data attribute
                item.Attributes.Add("data-image-url", listBox.ResolveUrl(country.ImageURL))

                ' Check if this item should be pre-selected
                If selectedSopIDs.Contains(country.SOP_ID) Then
                    item.Selected = True
                End If

                listBox.Items.Add(item)
            End If
        Next
    End Sub


    Private Sub BindCustomerData()
        Using cnx As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
            Dim sql As String = "SELECT TPC_ID, TPC_Name, TPID, TPID_Type, Countries,ClientCount FROM dbo.V_TPCustomer_WithCountries"

            Using da As New SqlDataAdapter(sql, cnx)
                Dim dt As New DataTable()
                da.Fill(dt)

                ' Clear old rows except header
                For i As Integer = CustomerTable.Rows.Count - 1 To 0 Step -1
                    If Not TypeOf CustomerTable.Rows(i) Is TableHeaderRow Then
                        CustomerTable.Rows.RemoveAt(i)
                    End If
                Next

                ' Add rows manually
                For Each dr As DataRow In dt.Rows
                    Dim row As New TableRow()

                    ' Expand + Edit icons in same cell
                    Dim actionCell As New TableCell()

                    actionCell.Text = "<button class='btn-expand' data-id='" & dr("TPC_ID").ToString() & "'>+</button>" &
                            " <img src='Images/edit.png' class='btn-edit' " &
                            "data-id='" & dr("TPC_ID").ToString() & "' " &
                            "data-name='" & dr("TPC_Name").ToString() & "' " &
                            "data-tpid='" & dr("TPID").ToString() & "' " &
                            "data-tptype='" & dr("TPID_Type").ToString() & "' " &
                            "data-countries='" & dr("Countries").ToString() & "' " &
                            "alt='Edit' width='20' height='20' style='cursor:pointer; margin-left:6px;'/>"

                    row.Cells.Add(actionCell)


                    ' Customer Code
                    Dim cellCode As New TableCell()
                    cellCode.Text = dr("TPC_ID").ToString()
                    row.Cells.Add(cellCode)

                    ' TP Type
                    Dim cellTPid_Type As New TableCell()
                    cellTPid_Type.Text = dr("TPID_Type").ToString()
                    row.Cells.Add(cellTPid_Type)

                    ' Name
                    Dim cellName As New TableCell()
                    cellName.Text = dr("TPC_Name").ToString()
                    row.Cells.Add(cellName)

                    ' TPID
                    Dim cellTPID As New TableCell()
                    cellTPID.Text = dr("TPID").ToString()
                    row.Cells.Add(cellTPID)

                    ' Country
                    Dim cellCountry As New TableCell()
                    cellCountry.Text = dr("Countries").ToString()
                    row.Cells.Add(cellCountry)

                    ' Client Count
                    Dim cellClientCount As New TableCell()
                    Dim count As Integer = Convert.ToInt32(dr("ClientCount"))
                    Dim tpcId As String = dr("TPC_ID").ToString()

                    If count > 0 Then
                        ' Clickable badge goes to list (new tab)
                        cellClientCount.Text = "<a href='TP2CustomerClients.aspx?tpcId=" & tpcId & "' " &
                                               "class='badge badge-info' title='View " & count & " clients' target='_blank'>" & count & "</a>"
                    Else
                        ' Clickable badge goes to create mode (new tab)
                        cellClientCount.Text = "<a href='TP2CustomerClients.aspx?tpcId=" & tpcId & "&mode=create' " &
                                               "class='badge badge-secondary' title='Add first client' target='_blank'>0</a>"
                    End If

                    row.Cells.Add(cellClientCount)

                    CustomerTable.Rows.Add(row)
                Next
            End Using
        End Using
    End Sub

    Protected Sub ddlCustomer_SelectedIndexChanged(sender As Object, e As EventArgs)

    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Function GetCustomerDetails(tpcId As String) As List(Of Object)
        Dim details As New List(Of Object)()
        Using cnx As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
            Dim sql As String = "SELECT TPC_ID, CY_ISOCODE, C_CUID " &
                             "FROM T_TPCustomer_B2BCustomer " &
                            " WHERE TPC_ID=@TPC_ID"
            Using cmd As New SqlCommand(sql, cnx)
                cmd.Parameters.AddWithValue("@TPC_ID", tpcId)
                cnx.Open()
                Dim reader As SqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    details.Add(New With {
                    .TPC_ID = reader("TPC_ID").ToString(),
                    .CY_ISOCODE = reader("CY_ISOCODE").ToString(),
                    .C_CUID = reader("C_CUID").ToString()
                })
                End While
            End Using
        End Using
        Return details
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function AddCustomerToTPC(tpcId As String, cCUID As String, cName As String, cyIso As String) As Boolean
        Using cnx As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
            Dim sql As String = "INSERT INTO T_TPCustomer_B2BCustomer (TPC_ID, C_CUID, CY_ISOCODE, C_NAME) " &
                             "VALUES (@TPC_ID, @C_CUID, @CY_ISOCODE, @C_NAME)"
            Using cmd As New SqlCommand(sql, cnx)
                cmd.Parameters.AddWithValue("@TPC_ID", tpcId)
                cmd.Parameters.AddWithValue("@C_CUID", cCUID)
                cmd.Parameters.AddWithValue("@CY_ISOCODE", cyIso)
                cmd.Parameters.AddWithValue("@C_NAME", cName)
                cnx.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
        Return True
    End Function



    Protected Sub btnSaveEdit_Click(sender As Object, e As EventArgs)

    End Sub
End Class
