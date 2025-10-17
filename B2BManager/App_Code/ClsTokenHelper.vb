Imports System.IO
Imports System.Security.Cryptography
Imports Newtonsoft.Json

Public Class ClsTokenHelper
    Public Shared Function GenerateToken(Of T)(tokenData As T) As String
        Dim encryptedBytes() As Byte
        Using aes As Aes = Aes.Create()
            Dim salt(15) As Byte
            Using rng As RandomNumberGenerator = RandomNumberGenerator.Create()
                rng.GetBytes(salt)
            End Using

            Using deriveBytes As New Rfc2898DeriveBytes("secretCodeQueryBuilder", salt)
                aes.Key = deriveBytes.GetBytes(32)
                aes.IV = deriveBytes.GetBytes(16)
            End Using

            Using ms As New MemoryStream()
                ms.Write(salt, 0, salt.Length)
                Using cs As New CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write)
                    Dim jsonData As String = JsonConvert.SerializeObject(tokenData)
                    Dim plainBytes() As Byte = Encoding.UTF8.GetBytes(jsonData)
                    cs.Write(plainBytes, 0, plainBytes.Length)
                End Using
                encryptedBytes = ms.ToArray()
            End Using
        End Using

        Return BitConverter.ToString(encryptedBytes).Replace("-", "").ToLower()
    End Function

    ''' <summary>
    ''' Decrypts the QueryBuilderToken and deserializes it into a TokenData object.
    ''' </summary>
    ''' <param name="token">The encrypted hex string from the URL.</param>
    ''' <param name="tokenData">The output object containing the decrypted data.</param>
    ''' <returns>True if decryption is successful, otherwise False.</returns>
    Public Shared Function TryParseToken(Of T)(ByVal token As String, ByRef tokenData As T) As Boolean
        Try
            If String.IsNullOrEmpty(token) Then Return False

            ' 1. Convert the hex string back into a byte array
            Dim encryptedBytes(token.Length \ 2 - 1) As Byte
            For i As Integer = 0 To encryptedBytes.Length - 1
                encryptedBytes(i) = Convert.ToByte(token.Substring(i * 2, 2), 16)
            Next

            ' 2. Extract the salt (first 16 bytes)
            Dim salt(15) As Byte
            Array.Copy(encryptedBytes, 0, salt, 0, salt.Length)

            ' 3. Using the same password and salt to regenerate the AES key and IV
            Using aes As Aes = Aes.Create()
                Using deriveBytes As New Rfc2898DeriveBytes("secretCodeQueryBuilder", salt)
                    aes.Key = deriveBytes.GetBytes(32)
                    aes.IV = deriveBytes.GetBytes(16)
                End Using

                ' 4. Decrypt the remaining bytes
                Using ms As New MemoryStream(encryptedBytes, salt.Length, encryptedBytes.Length - salt.Length)
                    Using cs As New CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read)
                        Using sr As New StreamReader(cs)
                            ' 5. Read the decrypted JSON and deserialize it into our object
                            Dim jsonData = sr.ReadToEnd()
                            tokenData = JsonConvert.DeserializeObject(Of T)(jsonData)
                        End Using
                    End Using
                End Using
            End Using

            Return True
        Catch ex As Exception
            ' If anything fails (bad token, decryption error), return false
            tokenData = Nothing
            Return False
        End Try
    End Function
End Class
