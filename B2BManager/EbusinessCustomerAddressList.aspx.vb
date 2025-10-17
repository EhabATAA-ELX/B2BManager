
Partial Class EbusinessCustomerAddressList
    Inherits System.Web.UI.Page

    Protected Function RestrictAccessDisplay() As Boolean
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Return False
        Else
            Return clsUser.Actions.Contains(ClsHelper.ActionDesignation.RESTRICT_ADDRESS_IN_ADDRESS_LIST_TAB)
        End If
    End Function

End Class
