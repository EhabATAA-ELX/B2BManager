Imports System.Runtime.Serialization
Imports Microsoft.VisualBasic

Public Class DataSource

    <Serializable>
    <DataContract>
    Public Class DataSource(Of T)
        <DataMember>
        Public Property data As T
    End Class

End Class
