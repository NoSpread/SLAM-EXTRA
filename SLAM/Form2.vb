Imports System.IO
Imports System.Net.WebRequestMethods

Public Class Form2
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        WebBrowser1.Navigate("http://convert2mp3.net/")
    End Sub

    Private Sub convert_Click(sender As Object, e As EventArgs) Handles convert.Click
        Dim allelements As HtmlElementCollection = WebBrowser1.Document.All
        Dim text As String = WebBrowser1.DocumentText
        ProgressBar1.Minimum = 0
        ProgressBar1.Maximum = 5
        ProgressBar1.Step = 1
        WebBrowser1.Document.GetElementById("urlinput").SetAttribute("value", ytlink.Text)

        For Each webpageelement As HtmlElement In allelements
            If webpageelement.InnerText = "umwandeln" Then
                webpageelement.InvokeMember("click")
            End If
        Next
        ProgressBar1.PerformStep()
        For i As Integer = 0 To 10 * 100
            System.Threading.Thread.Sleep(10)
            Application.DoEvents()
        Next
        ProgressBar1.PerformStep()
        While (WebBrowser1.ReadyState <> WebBrowserReadyState.Complete)
            Application.DoEvents()
        End While
        ProgressBar1.PerformStep()
        Dim allelements2 As HtmlElementCollection = WebBrowser1.Document.All
        For Each webpageelement As HtmlElement In allelements2
            If webpageelement.InnerText = "Weiter" Then
                webpageelement.InvokeMember("click")
            End If
        Next
        ProgressBar1.PerformStep()
        For i As Integer = 0 To 2 * 100
            System.Threading.Thread.Sleep(10)
            Application.DoEvents()
        Next
        ProgressBar1.PerformStep()
        Dim allelements3 As HtmlElementCollection = WebBrowser1.Document.All
        For Each webpageelement As HtmlElement In allelements3
            If webpageelement.InnerText = " Download starten" Then
                webpageelement.InvokeMember("click")
            End If
        Next
        ProgressBar1.Value = 0
        Me.Close()
    End Sub
End Class