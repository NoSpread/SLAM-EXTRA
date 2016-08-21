Imports System.IO
Imports System.Net.WebRequestMethods
Imports System.Net.Mime
Imports System.Net.WebClient
Imports System.Net

Public Class Form2
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        WebBrowser1.Navigate("http://convert2mp3.net/")  'convert mp4 to mp3
        Timer1.Start()

    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim url As String = ytlink.Text
        If url.StartsWith("https://www.youtube.com/watch?v=") Or url.StartsWith("http://www.dailymotion.com/video/") Or url.StartsWith("http://www.vevo.com/") Or url.StartsWith("http://www.clipfish.de/") Then  'checks if url is from youtube
            convert.Enabled = True
        Else
            convert.Enabled = False
        End If
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
                Dim dlurl As String = webpageelement.GetAttribute("href")
                Dim path As String = My.Computer.FileSystem.CurrentDirectory + "\youtube\"
                Dim title As String = songname.Text + ".mp3"

                If Directory.Exists(path) Then
                    My.Computer.Network.DownloadFile(dlurl, path + title)
                Else
                    Directory.CreateDirectory(path)
                    My.Computer.Network.DownloadFile(dlurl, path + title)
                End If
                '
                'My.Computer.Network.DownloadFile("" & dlurl & "", )
                'webpageelement.InvokeMember("click")
            End If
        Next

        ProgressBar1.Value = 0
        Timer1.Stop()
        Me.Close()
    End Sub
End Class