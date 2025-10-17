Imports System.Runtime.Serialization
Imports Microsoft.VisualBasic
<DataContract>
Public Class B2BTranslation
    <DataMember>
    Public TN_GlobalID As String
    <DataMember>
    Public TN_Name As String
    <DataMember>
    Public TN_Comment As String
    <DataMember>
    Public CountryValue As String
    <DataMember>
    Public DefaultValue As String
    <DataMember>
    Public Areas As String
End Class
