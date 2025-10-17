Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D
Imports System.IO

Public Module FileHelper

    ''' <summary>
    ''' Checks if the given content type is an image.
    ''' </summary>
    Public Function IsImageContentType(contentType As String) As Boolean
        Return Not String.IsNullOrEmpty(contentType) AndAlso contentType.ToLower().StartsWith("image")
    End Function

    ''' <summary>
    ''' Gets the default thumbnail icon file name for a given file extension.
    ''' </summary>
    Public Function DefaultIconForExtension(ext As String) As String
        Select Case ext.ToLowerInvariant()
            Case ".pdf" : Return "pdf.png"
            Case ".ppt", ".pptx" : Return "ppt.png"
            Case ".sql" : Return "sql.png"
            Case ".doc", ".docx" : Return "word.png"
            Case ".xls", ".xlsx", ".xlsm", ".csv" : Return "excel.png"
            Case ".mp4", ".wmv" : Return "mp4.png"
            Case ".xml", ".xlsx" : Return "xml.png"
            Case Else : Return "file.png"
        End Select
    End Function

    ''' <summary>
    ''' Resizes an image byte array to a fixed canvas of 100x80 px, keeping aspect ratio.
    ''' </summary>
    Public Function ResizeImage(originalBytes As Byte(), canvasWidth As Integer, canvasHeight As Integer, originalFileExtension As String) As Byte()
        Try
            Using msOriginal As New MemoryStream(originalBytes)
                Using originalImage As Image = Image.FromStream(msOriginal)
                    Dim originalWidth = originalImage.Width
                    Dim originalHeight = originalImage.Height

                    Dim ratioX As Double = canvasWidth / originalWidth
                    Dim ratioY As Double = canvasHeight / originalHeight
                    Dim ratio As Double = Math.Min(ratioX, ratioY)

                    Dim newWidth As Integer = CInt(originalWidth * ratio)
                    Dim newHeight As Integer = CInt(originalHeight * ratio)

                    Dim offsetX As Integer = (canvasWidth - newWidth) \ 2
                    Dim offsetY As Integer = (canvasHeight - newHeight) \ 2

                    Dim outputFormat As ImageFormat = ImageFormat.Jpeg
                    Select Case originalFileExtension.ToLower()
                        Case ".png" : outputFormat = ImageFormat.Png
                        Case ".gif" : outputFormat = ImageFormat.Png
                        Case ".bmp" : outputFormat = ImageFormat.Bmp
                        Case ".jpg", ".jpeg" : outputFormat = ImageFormat.Jpeg
                    End Select

                    Using canvas As New Bitmap(canvasWidth, canvasHeight)
                        Using graphics As Graphics = Graphics.FromImage(canvas)
                            graphics.Clear(Color.White)
                            graphics.CompositingQuality = CompositingQuality.HighQuality
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic
                            graphics.SmoothingMode = SmoothingMode.HighQuality
                            graphics.DrawImage(originalImage, offsetX, offsetY, newWidth, newHeight)
                        End Using

                        Using msResized As New MemoryStream()
                            canvas.Save(msResized, outputFormat)
                            Return msResized.ToArray()
                        End Using
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Validates if an image has exactly 100x80 dimensions.
    ''' </summary>
    Public Function IsImageValidExactSize(file As Image, ByRef errorMsg As String) As Boolean
        If file.Width <> 100 OrElse file.Height <> 80 Then
            errorMsg = "The image must be exactly 100px width and 80px height."
            Return False
        End If
        Return True
    End Function

End Module
