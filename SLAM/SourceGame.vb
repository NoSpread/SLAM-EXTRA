Public Class SourceGame
    Public Name As String
    Public Id As Integer
    Public Directory As String
    Public ToCfg As String
    Public Libraryname As String
    Public VoiceFadeOut As Boolean = True
    Public Exename As String = "hl2"

    Public FileExtension As String = ".wav"
    Public Samplerate As Integer = 11025
    Public Bits As Integer = 16
    Public Channels As Integer = 1

    Public PollInterval As Integer = 100

    Public Tracks As New List(Of Track)
    Public Blacklist As New List(Of String) From {"slam", "slam_listtracks", "list", "tracks", "la", "slam_play", "slam_play_on", "slam_play_off", "slam_updatecfg", "slam_curtrack", "slam_saycurtrack", "slam_sayteamcurtrack"}

    Public Class Track
        Public name As String
        Public Tags As New List(Of String)
        Public Hotkey As String = vbNullString
        Public Volume As Integer = 100
        Public Startpos As Integer
        Public Endpos As Integer
    End Class
End Class