'Modified Version which does not write "extraSize"
Imports NAudio.Wave
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Threading
Imports SLAM.XmlSerialization
Imports SLAM.SourceGame
Imports System.Management


Public Class Form1

    ReadOnly _games As New List(Of SourceGame)
    Dim _running As Boolean = False
    Dim _closePending As Boolean = False

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        RefreshPlayKey()

        ''If My.Settings.UpdateCheck Then
        ''CheckForUpdate()
        ''End If

        Dim csgo As New SourceGame
        csgo.Name = "Counter-Strike: Global Offensive"
        csgo.Id = 730
        csgo.Directory = "common\Counter-Strike Global Offensive\"
        csgo.ToCfg = "csgo\cfg\"
        csgo.Libraryname = "csgo\"
        csgo.Exename = "csgo"
        csgo.Samplerate = 22050
        csgo.Blacklist.AddRange({"drop", "buy", "go", "fallback", "sticktog", "holdpos", "followme", "coverme", "regroup", "roger", "negative", "cheer", "compliment", "thanks", "enemydown", "reportingin", "enemyspot", "takepoint", "sectorclear", "inposition", "takingfire", "report", "getout"})
        csgo.VoiceFadeOut = False
        _games.Add(csgo)

        Dim css As New SourceGame
        css.Name = "Counter-Strike: Source"
        css.Directory = "common\Counter-Strike Source\"
        css.ToCfg = "cstrike\cfg\"
        css.Libraryname = "css\"
        _games.Add(css)

        Dim tf2 As New SourceGame
        tf2.Name = "Team Fortress 2"
        tf2.Directory = "common\Team Fortress 2\"
        tf2.ToCfg = "tf\cfg\"
        tf2.Libraryname = "tf2\"
        _games.Add(tf2)

        Dim gmod As New SourceGame
        gmod.Name = "Garry's Mod"
        gmod.Directory = "common\GarrysMod\"
        gmod.ToCfg = "garrysmod\cfg\"
        gmod.Libraryname = "gmod\"
        _games.Add(gmod)

        Dim hl2Dm As New SourceGame
        hl2Dm.Name = "Half-Life 2 Deathmatch"
        hl2Dm.Directory = "common\half-life 2 deathmatch\"
        hl2Dm.ToCfg = "hl2mp\cfg\"
        hl2Dm.Libraryname = "hl2dm\"
        _games.Add(hl2Dm)

        Dim l4D As New SourceGame
        l4D.Name = "Left 4 Dead"
        l4D.Directory = "common\Left 4 Dead\"
        l4D.ToCfg = "left4dead\cfg\"
        l4D.Libraryname = "l4d\"
        l4D.Exename = "left4dead"
        _games.Add(l4D)

        Dim l4D2 As New SourceGame
        l4D2.Name = "Left 4 Dead 2"
        l4D2.Directory = "common\Left 4 Dead 2\"
        l4D2.ToCfg = "left4dead2\cfg\"
        l4D2.Libraryname = "l4d2\"
        l4D2.Exename = "left4dead2"
        l4D2.VoiceFadeOut = False
        _games.Add(l4D2)

        Dim dods As New SourceGame
        dods.Name = "Day of Defeat Source"
        dods.Directory = "common\day of defeat source\"
        dods.ToCfg = "dod\cfg\"
        dods.Libraryname = "dods\"
        _games.Add(dods)

        'NEEDS EXENAME!!!
        'Dim goldeye As New SourceGame
        'goldeye.name = "Goldeneye Source"
        'goldeye.directory = "sourcemods\"
        'goldeye.ToCfg = "gesource\cfg\"
        'goldeye.libraryname = "goldeye\"
        'Games.Add(goldeye)

        Dim insurg As New SourceGame
        insurg.Name = "Insurgency"
        insurg.Directory = "common\insurgency2\"
        insurg.ToCfg = "insurgency\cfg\"
        insurg.Libraryname = "insurgen\"
        insurg.Exename = "insurgency"
        _games.Add(insurg)

        For Each Game In _games
            GameSelector.Items.Add(Game.Name)
        Next

        If GameSelector.Items.Contains(My.Settings.LastGame) Then
            GameSelector.Text = GameSelector.Items(GameSelector.Items.IndexOf(My.Settings.LastGame)).ToString
        Else
            GameSelector.Text = GameSelector.Items(0).ToString
        End If

        ReloadTracks(GetCurrentGame)
        RefreshTrackList()

        If My.Settings.StartEnabled Then
            StartPoll()
        End If
    End Sub

    Private Sub WaveCreator(file As String, outputFile As String, game As SourceGame)
        Dim reader As New MediaFoundationReader(file)


        Dim outFormat = New WaveFormat(game.Samplerate, game.Bits, game.Channels)

        Dim resampler = New MediaFoundationResampler(reader, outFormat)

        resampler.ResamplerQuality = 60

        WaveFileWriter.CreateWaveFile(outputFile, resampler)

        resampler.Dispose()
    End Sub

    Private Sub GameSelector_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GameSelector.SelectedIndexChanged
        ReloadTracks(GetCurrentGame)
        RefreshTrackList()
        My.Settings.LastGame = GameSelector.Text
        My.Settings.Save()
    End Sub

    Private Sub ImportButton_Click(sender As Object, e As EventArgs) Handles ImportButton.Click
        If File.Exists("NAudio.dll") Then
            DisableInterface()
            If ImportDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                ProgressBar1.Maximum = ImportDialog.FileNames.Count
                Dim workerPassthrough() As Object = {GetCurrentGame(), ImportDialog.FileNames}
                WavWorker.RunWorkerAsync(workerPassthrough)
            Else
                EnableInterface()
            End If

        Else
            MessageBox.Show("You are missing NAudio.dll! Cannot import without it!", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub WavWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles WavWorker.DoWork
        Dim game As SourceGame = e.Argument(0)
        Dim files() As String = e.Argument(1)
        Dim failedFiles As New List(Of String)

        For Each File In files

            Try
                Dim outFile As String = Path.Combine(game.Libraryname, Path.GetFileNameWithoutExtension(File) & ".wav")
                WaveCreator(File, outFile, game)

            Catch ex As Exception
                LogError(ex)
                failedFiles.Add(File)
            End Try
            WavWorker.ReportProgress(0)
        Next

        e.Result = failedFiles

    End Sub

    Private Sub WavWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles WavWorker.ProgressChanged
        ProgressBar1.PerformStep()
        ReloadTracks(GetCurrentGame)
        RefreshTrackList()
    End Sub

    Private Sub WavWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles WavWorker.RunWorkerCompleted
        ProgressBar1.Value = 0
        Dim msgBoxText As String = "Conversion complete!"
        Dim failedFiles As New List(Of String)

        For Each FilePath In e.Result
            failedFiles.Add(Path.GetFileName(FilePath))
        Next

        If failedFiles.Count > 0 Then
            msgBoxText = msgBoxText & " However, the following files failed to convert: " & String.Join(", ", failedFiles)
        End If

        ReloadTracks(GetCurrentGame)
        RefreshTrackList()
        MsgBox(msgBoxText)
        EnableInterface()
    End Sub

    Private Function GetCurrentGame() As SourceGame
        For Each Game In _games
            If Game.Name = GameSelector.SelectedItem.ToString Then
                Return Game
            End If
        Next
        Return Nothing 'Null if nothing found
    End Function

    Private Sub ReloadTracks(game As SourceGame)
        If IO.Directory.Exists(game.Libraryname) Then

            game.Tracks.Clear()
            For Each File In IO.Directory.GetFiles(game.Libraryname)

                If game.FileExtension = Path.GetExtension(File) Then
                    Dim track As New Track
                    track.name = Path.GetFileNameWithoutExtension(File)
                    game.Tracks.Add(track)
                End If

            Next

            CreateTags(game)
            LoadTrackKeys(game)
            SaveTrackKeys(game) 'To prune hotkeys from non-existing tracks

        Else
            IO.Directory.CreateDirectory(game.Libraryname)
        End If
    End Sub

    Private Sub RefreshTrackList()
        TrackList.Items.Clear()

        Dim game As SourceGame = GetCurrentGame()

        For Each Track In game.Tracks

            ' ReSharper disable once LocalVariableHidesMember
            Dim trimmed As String
            trimmed = ""
            If Track.Endpos > 0 Then
                trimmed = "Yes"
            End If

            TrackList.Items.Add(New ListViewItem({"False", Track.name, Track.Hotkey, Track.Volume & "%", trimmed, """" & String.Join(""", """, Track.Tags) & """"}))
        Next


        TrackList.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize)
        TrackList.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent)
        TrackList.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.HeaderSize)
        TrackList.AutoResizeColumn(3, ColumnHeaderAutoResizeStyle.HeaderSize)
        TrackList.AutoResizeColumn(4, ColumnHeaderAutoResizeStyle.HeaderSize)
        TrackList.AutoResizeColumn(5, ColumnHeaderAutoResizeStyle.ColumnContent)
    End Sub

    Private Sub StartButton_Click(sender As Object, e As EventArgs) Handles StartButton.Click
        If _running Then
            StopPoll()
        Else
            StartPoll()
            If Not My.Settings.NoHint Then
                If MessageBox.Show("Don't forget to type ""exec slam"" in console! Click ""Cancel"" if you don't ever want to see this message again.", "SLAM", MessageBoxButtons.OKCancel) = Windows.Forms.DialogResult.Cancel Then
                    My.Settings.NoHint = True
                    My.Settings.Save()
                End If
            End If
        End If
    End Sub

    Private Sub StartPoll()
        _running = True
        StartButton.Text = "Stop"
        DisableInterface()
        StartButton.Enabled = True
        TrackList.Enabled = True
        SetVolumeToolStripMenuItem.Enabled = True
        PollRelayWorker.RunWorkerAsync(GetCurrentGame)
    End Sub

    Private Sub StopPoll()
        _running = False
        StartButton.Text = "Start"
        EnableInterface()
        PollRelayWorker.CancelAsync()
    End Sub

    Private Sub CreateCfgFiles(game As SourceGame, steamappsPath As String)
        Dim gameDir As String = Path.Combine(steamappsPath, game.Directory)
        Dim gameCfgFolder As String = Path.Combine(gameDir, game.ToCfg)

        'slam.cfg
        Using slamCfg As StreamWriter = New StreamWriter(gameCfgFolder & "slam.cfg")
            slamCfg.WriteLine("alias slam_listtracks ""exec slam_tracklist.cfg""")
            slamCfg.WriteLine("alias list slam_listtracks")
            slamCfg.WriteLine("alias tracks slam_listtracks")
            slamCfg.WriteLine("alias la slam_listtracks")
            slamCfg.WriteLine("alias slam_play slam_play_on")
            slamCfg.WriteLine("alias slam_play_on ""alias slam_play slam_play_off; voice_inputfromfile 1; voice_loopback 1; +voicerecord""")
            slamCfg.WriteLine("alias slam_play_off ""-voicerecord; voice_inputfromfile 0; voice_loopback 0; alias slam_play slam_play_on""")
            slamCfg.WriteLine("alias slam_updatecfg ""host_writeconfig slam_relay""")
            If My.Settings.HoldToPlay Then
                slamCfg.WriteLine("alias +slam_hold_play slam_play_on")
                slamCfg.WriteLine("alias -slam_hold_play slam_play_off")
                slamCfg.WriteLine("bind {0} +slam_hold_play", My.Settings.PlayKey)
            Else
                slamCfg.WriteLine("bind {0} slam_play", My.Settings.PlayKey)
            End If
            slamCfg.WriteLine("alias slam_curtrack ""exec slam_curtrack.cfg""")
            slamCfg.WriteLine("alias slam_saycurtrack ""exec slam_saycurtrack.cfg""")
            slamCfg.WriteLine("alias slam_sayteamcurtrack ""exec slam_sayteamcurtrack.cfg""")

            For Each Track In game.Tracks
                Dim index As String = game.Tracks.IndexOf(Track)
                slamCfg.WriteLine("alias {0} ""bind {1} {0}; slam_updatecfg; echo Loaded: {2}""", index + 1, My.Settings.RelayKey, Track.name)

                For Each TrackTag In Track.Tags
                    slamCfg.WriteLine("alias {0} ""bind {1} {2}; slam_updatecfg; echo Loaded: {3}""", TrackTag, My.Settings.RelayKey, index + 1, Track.name)
                Next

                If Not String.IsNullOrEmpty(Track.Hotkey) Then
                    slamCfg.WriteLine("bind {0} ""bind {1} {2}; slam_updatecfg; echo Loaded: {3}""", Track.Hotkey, My.Settings.RelayKey, index + 1, Track.name)
                End If
            Next

            Dim cfgData As String
            cfgData = "voice_enable 1; voice_modenable 1; voice_forcemicrecord 0; con_enable 1"

            If game.VoiceFadeOut Then
                cfgData = cfgData + "; voice_fadeouttime 0.0"
            End If

            slamCfg.WriteLine(cfgData)

        End Using

        'slam_tracklist.cfg
        Using slamTracklistCfg As StreamWriter = New StreamWriter(gameCfgFolder & "slam_tracklist.cfg")
            slamTracklistCfg.WriteLine("echo ""You can select tracks either by typing a tag, or their track number.""")
            slamTracklistCfg.WriteLine("echo ""--------------------Tracks--------------------""")
            For Each Track In game.Tracks
                Dim index As String = game.Tracks.IndexOf(Track)
                If My.Settings.WriteTags Then
                    slamTracklistCfg.WriteLine("echo ""{0}. {1} [{2}]""", index + 1, Track.name, "'" & String.Join("', '", Track.Tags) & "'")
                Else
                    slamTracklistCfg.WriteLine("echo ""{0}. {1}""", index + 1, Track.name)
                End If
            Next
            slamTracklistCfg.WriteLine("echo ""----------------------------------------------""")
        End Using

    End Sub

    Private Function LoadTrack(ByVal game As SourceGame, ByVal index As Integer, ByVal steamappsPath As String) As Boolean
        Dim track As Track
        If game.Tracks.Count > index Then
            track = game.Tracks(index)
            Dim voicefile As String = Path.Combine(steamappsPath, game.Directory) & "voice_input.wav"
            Try
                If File.Exists(voicefile) Then
                    File.Delete(voicefile)
                End If

                Dim trackfile As String = game.Libraryname & track.name & game.FileExtension
                If File.Exists(trackfile) Then

                    If track.Volume = 100 And track.Startpos = -1 And track.Endpos = -1 Then
                        File.Copy(trackfile, voicefile)
                    Else

                        Dim waveFloat As New WaveChannel32(New WaveFileReader(trackfile))

                        If Not track.Volume = 100 Then
                            waveFloat.Volume = (track.Volume / 100) ^ 6
                        End If

                        If Not track.Startpos = track.Endpos And track.Endpos > 0 Then
                            Dim bytes((track.Endpos - track.Startpos) * 4) As Byte

                            waveFloat.Position = track.Startpos * 4
                            waveFloat.Read(bytes, 0, (track.Endpos - track.Startpos) * 4)

                            waveFloat = New WaveChannel32(New RawSourceWaveStream(New MemoryStream(bytes), waveFloat.WaveFormat))
                        End If

                        waveFloat.PadWithZeroes = False
                        Dim outFormat = New WaveFormat(game.Samplerate, game.Bits, game.Channels)
                        Dim resampler = New MediaFoundationResampler(waveFloat, outFormat)
                        resampler.ResamplerQuality = 60
                        WaveFileWriter.CreateWaveFile(voicefile, resampler)

                        resampler.Dispose()
                        waveFloat.Dispose()

                    End If

                    Dim gameCfgFolder As String = Path.Combine(steamappsPath, game.Directory, game.ToCfg)
                    Using slamCurtrack As StreamWriter = New StreamWriter(gameCfgFolder & "slam_curtrack.cfg")
                        slamCurtrack.WriteLine("echo ""[SLAM] Track name: {0}""", track.name)
                    End Using
                    Using slamSaycurtrack As StreamWriter = New StreamWriter(gameCfgFolder & "slam_saycurtrack.cfg")
                        slamSaycurtrack.WriteLine("say ""[SLAM] Track name: {0}""", track.name)
                    End Using
                    Using slamSayteamcurtrack As StreamWriter = New StreamWriter(gameCfgFolder & "slam_sayteamcurtrack.cfg")
                        slamSayteamcurtrack.WriteLine("say_team ""[SLAM] Track name: {0}""", track.name)
                    End Using


                End If

            Catch ex As Exception
                LogError(ex)
                Return False
            End Try

        Else
            Return False
        End If

        Return True
    End Function

    Private Function Recog(ByVal str As String, ByVal reg As String) As String
        Dim keyd As Match = Regex.Match(str, reg)
        Return (keyd.Groups(1).ToString)
    End Function

    Private Sub PollRelayWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles PollRelayWorker.DoWork
        PollRelayWorker.ReportProgress(-1) 'Report that SLAM is searching.

        Dim game As SourceGame = e.Argument
        Dim gameDir As String = game.Directory & game.Exename & ".exe"

        Dim steamAppsPath As String = vbNullString
        Dim userDataPath As String = vbNullString

        Try
            Do While Not PollRelayWorker.CancellationPending

                Dim gameProcess As String = GetFilepath(game.Exename)
                If Not String.IsNullOrEmpty(gameProcess) AndAlso gameProcess.EndsWith(gameDir) Then
                    steamAppsPath = gameProcess.Remove(gameProcess.Length - gameDir.Length)
                End If

                Dim steamProcess As String = GetFilepath("Steam")
                If Not String.IsNullOrEmpty(steamProcess) Then
                    userDataPath = steamProcess.Remove(steamProcess.Length - "Steam.exe".Length) & "userdata\"
                End If

                If IO.Directory.Exists(steamAppsPath) Then
                    If Not game.Id = 0 Then

                        If IO.Directory.Exists(userDataPath) Then
                            Exit Do
                        End If

                    Else
                        Exit Do
                    End If
                End If

                Thread.Sleep(game.PollInterval)
            Loop

            If Not String.IsNullOrEmpty(steamAppsPath) Then
                CreateCfgFiles(game, steamAppsPath)
            End If

        Catch ex As Exception
            LogError(ex)
            e.Result = ex
            Return
        End Try


        PollRelayWorker.ReportProgress(-2) 'Report that SLAM is working.

        Do While Not PollRelayWorker.CancellationPending
            Try
                Dim gameFolder As String = Path.Combine(steamAppsPath, game.Directory)
                Dim gameCfg As String = Path.Combine(gameFolder, game.ToCfg) & "slam_relay.cfg"

                If Not game.Id = 0 Then
                    gameCfg = UserDataCfg(game, userDataPath)
                End If

                If File.Exists(gameCfg) Then
                    Dim relayCfg As String
                    Using reader As StreamReader = New StreamReader(gameCfg)
                        relayCfg = reader.ReadToEnd
                    End Using

                    Dim command As String = Recog(relayCfg, String.Format("bind ""{0}"" ""(.*?)""", My.Settings.RelayKey))
                    If Not String.IsNullOrEmpty(command) Then
                        'load audiofile
                        If IsNumeric(command) Then
                            If LoadTrack(game, Convert.ToInt32(command) - 1, steamAppsPath) Then
                                PollRelayWorker.ReportProgress(Convert.ToInt32(command) - 1)
                            End If
                        End If
                        File.Delete(gameCfg)
                    End If
                End If

                Thread.Sleep(game.PollInterval)

            Catch ex As Exception
                If Not ex.HResult = -2147024864 Then '-2147024864 = "System.IO.IOException: The process cannot access the file because it is being used by another process."
                    LogError(ex)
                    e.Result = ex
                    Return
                End If
            End Try
        Loop

        If Not String.IsNullOrEmpty(steamAppsPath) Then
            DeleteCfGs(game, steamAppsPath)
        End If

    End Sub

    Public Function UserDataCfg(game As SourceGame, userdataPath As String) As String
        If IO.Directory.Exists(userdataPath) Then
            For Each userdir As String In IO.Directory.GetDirectories(userdataPath)
                Dim cfgPath As String = Path.Combine(userdir, game.Id.ToString) & "\local\cfg\slam_relay.cfg"
                If File.Exists(cfgPath) Then
                    Return cfgPath
                End If
            Next
        End If
        Return vbNullString
    End Function

    Private Function GetFilepath(processName As String) As String

        Dim wmiQueryString As String = "Select * from Win32_Process Where Name = """ & processName & ".exe"""

        Using searcher = New ManagementObjectSearcher(wmiQueryString)
            Using results = searcher.Get()

                Dim process As ManagementObject = results.Cast(Of ManagementObject)().FirstOrDefault()
                If process IsNot Nothing Then
                    Return process("ExecutablePath").ToString
                End If

            End Using
        End Using

        Return Nothing
    End Function

    Private Sub PollRelayWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles PollRelayWorker.ProgressChanged
        Select Case e.ProgressPercentage
            Case -1
                StatusLabel.Text = "Status: Searching..."
            Case -2
                StatusLabel.Text = "Status: Working."
            Case Else
                DisplayLoaded(e.ProgressPercentage)
        End Select

    End Sub

    Private Sub PollRelayWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles PollRelayWorker.RunWorkerCompleted
        If _running Then
            StopPoll()
        End If

        StatusLabel.Text = "Status: Idle."

        If Not IsNothing(e.Result) Then 'Result is always an exception
            MessageBox.Show(e.Result.Message & " See errorlog.txt for more info.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        If _closePending Then
            Close()
        End If
    End Sub

    Private Sub CreateTags(ByVal game As SourceGame)
        Dim nameWords As New Dictionary(Of String, Integer)

        Dim index As Integer
        For Each Track In game.Tracks
            Dim words As List(Of String) = Track.name.Split({" "c, "."c, "-"c, "_"c}).ToList

            For Each Word In words

                If Not IsNumeric(Word) And Not game.Blacklist.Contains(Word.ToLower) And Word.Length < 32 Then
                    If nameWords.ContainsKey(Word) Then
                        nameWords.Remove(Word)
                    Else
                        nameWords.Add(Word, index)
                    End If
                End If

            Next
            index += 1
        Next

        For Each Tag As KeyValuePair(Of String, Integer) In nameWords
            game.Tracks(Tag.Value).Tags.Add(Tag.Key)
        Next
    End Sub

    Private Sub EnableInterface()
        For Each Control In Controls
            Control.Enabled = True
        Next
    End Sub

    Private Sub DisableInterface()
        For Each Control In Controls
            Control.Enabled = False
        Next
    End Sub

    Private Sub DisplayLoaded(ByVal track As Integer)
        For i As Integer = 0 To TrackList.Items.Count - 1
            TrackList.Items(i).SubItems(0).Text = "False"
        Next
        TrackList.Items(track).SubItems(0).Text = "True"
    End Sub

    Private Sub LoadTrackKeys(ByVal game As SourceGame)
        Dim settingsList As New List(Of Track)
        Dim settingsFile As String = Path.Combine(game.Libraryname, "TrackSettings.xml")

        If File.Exists(settingsFile) Then
            Dim xmlFile As String
            Using reader As StreamReader = New StreamReader(settingsFile)
                xmlFile = reader.ReadToEnd
            End Using
            settingsList = Deserialize(Of List(Of Track))(xmlFile)
        End If


        For Each Track In game.Tracks
            For Each SetTrack In settingsList
                If Track.name = SetTrack.name Then
                    'Please tell me that there is a better way to do the following...
                    Track.Hotkey = SetTrack.Hotkey
                    Track.Volume = SetTrack.Volume
                    Track.Startpos = SetTrack.Startpos
                    Track.Endpos = SetTrack.Endpos
                End If
            Next
        Next

    End Sub

    Private Sub SaveTrackKeys(ByVal game As SourceGame)
        Dim settingsList As New List(Of Track)
        Dim settingsFile As String = Path.Combine(game.Libraryname, "TrackSettings.xml")

        For Each Track In game.Tracks
            If Not String.IsNullOrEmpty(Track.Hotkey) Or Not Track.Volume = 100 Or Track.Endpos > 0 Then

                settingsList.Add(Track)

            End If
        Next

        If settingsList.Count > 0 Then
            Using writer As StreamWriter = New StreamWriter(settingsFile)
                writer.Write(Serialize(settingsList))
            End Using
        Else
            If File.Exists(settingsFile) Then
                File.Delete(settingsFile)
            End If
        End If

    End Sub

    Private Sub TrackList_MouseClick(sender As Object, e As MouseEventArgs) Handles TrackList.MouseClick
        If e.Button = MouseButtons.Right Then
            If TrackList.FocusedItem.Bounds.Contains(e.Location) Then

                For Each Control In TrackContextMenu.Items 'everything invisible
                    Control.visible = False
                Next

                SetVolumeToolStripMenuItem.Visible = True 'always visible
                ContextRefresh.Visible = True

                If TrackList.SelectedItems.Count > 1 Then
                    If Not _running Then 'visible when multiple selected AND is not running
                        ContextDelete.Visible = True
                    End If

                Else
                    If _running Then
                        TrimToolStripMenuItem.Visible = True 'visible when only one selected AND is running
                    Else
                        For Each Control In TrackContextMenu.Items 'visible when only one selected AND is not running (all)
                            Control.visible = True
                        Next
                    End If

                End If
                'Maybe I should have used a case... Maybe...

            End If



            TrackContextMenu.Show(Cursor.Position)
        End If
    End Sub

    Private Sub ContextRefresh_Click(sender As Object, e As EventArgs) Handles ContextRefresh.Click
        ReloadTracks(GetCurrentGame)
        RefreshTrackList()
    End Sub

    Private Sub ContextDelete_Click(sender As Object, e As EventArgs) Handles ContextDelete.Click
        Dim game As SourceGame = GetCurrentGame()

        Dim selectedNames As New List(Of String)
        For Each item In TrackList.SelectedItems
            selectedNames.Add(item.SubItems(1).Text)
        Next

        If MessageBox.Show(String.Format("Are you sure you want to delete {0}?", String.Join(", ", selectedNames)), "Delete Track?", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then

            For Each item In selectedNames
                Dim filePath As String = Path.Combine(game.Libraryname, item & game.FileExtension)

                If File.Exists(filePath) Then
                    Try
                        File.Delete(filePath)
                    Catch ex As Exception
                        LogError(ex)
                        MsgBox(String.Format("Failed to delete {0}.", filePath))
                    End Try
                End If
            Next

        End If

        ReloadTracks(GetCurrentGame)
        RefreshTrackList()
    End Sub

    Private Sub ContextHotKey_Click(sender As Object, e As EventArgs) Handles ContextHotKey.Click
        Dim selectKeyDialog As New SelectKey
        Dim selectedIndex = TrackList.SelectedItems(0).Index
        If selectKeyDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim game = GetCurrentGame()



            Dim keyIsFree As Boolean = True
            For Each track In game.Tracks
                If track.Hotkey = selectKeyDialog.ChosenKey Then 'Checking to see if any other track is already using this key
                    keyIsFree = False
                End If
            Next

            If keyIsFree Then
                game.Tracks(selectedIndex).Hotkey = selectKeyDialog.ChosenKey
                SaveTrackKeys(GetCurrentGame)
                ReloadTracks(GetCurrentGame)
                RefreshTrackList()
            Else
                MessageBox.Show(String.Format("""{0}"" has already been assigned!", selectKeyDialog.ChosenKey), "Invalid Key")
            End If


        End If
    End Sub

    Private Sub RemoveHotkeyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveHotkeyToolStripMenuItem.Click
        For Each SelectedIndex In TrackList.SelectedItems
            Dim game = GetCurrentGame()
            game.Tracks(SelectedIndex.index).Hotkey = vbNullString
            SaveTrackKeys(GetCurrentGame)
            ReloadTracks(GetCurrentGame)

        Next
        RefreshTrackList()
    End Sub

    Private Sub GoToToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GoToToolStripMenuItem.Click
        Dim games As SourceGame = GetCurrentGame()
        Dim filePath As String = Path.Combine(games.Libraryname, games.Tracks(TrackList.SelectedItems(0).Index).name & games.FileExtension)


        Dim args As String = String.Format("/Select, ""{0}""", filePath)
        Dim pfi As New ProcessStartInfo("Explorer.exe", args)

        Process.Start(pfi)
    End Sub

    Private Sub SetVolumeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetVolumeToolStripMenuItem.Click
        Dim setVolumeDialog As New SetVolume

        If setVolumeDialog.ShowDialog = Windows.Forms.DialogResult.OK Then

            For Each index In TrackList.SelectedIndices
                GetCurrentGame.Tracks(index).Volume = setVolumeDialog.Volume
            Next
            SaveTrackKeys(GetCurrentGame)
            ReloadTracks(GetCurrentGame)
            RefreshTrackList()

        End If

    End Sub

    Private Sub TrimToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TrimToolStripMenuItem.Click
        Dim game As SourceGame = GetCurrentGame()
        Dim trimDialog As New TrimForm

        trimDialog.WavFile = Path.Combine(game.Libraryname, game.Tracks(TrackList.SelectedIndices(0)).name & game.FileExtension)
        trimDialog.Startpos = game.Tracks(TrackList.SelectedIndices(0)).Startpos
        trimDialog.Endpos = game.Tracks(TrackList.SelectedIndices(0)).Endpos


        If trimDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            game.Tracks(TrackList.SelectedIndices(0)).Startpos = trimDialog.Startpos
            game.Tracks(TrackList.SelectedIndices(0)).Endpos = trimDialog.Endpos
            SaveTrackKeys(GetCurrentGame)
            ReloadTracks(GetCurrentGame)
            RefreshTrackList()
        End If
    End Sub

    Private Sub RenameToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RenameToolStripMenuItem.Click
        Dim game As SourceGame = GetCurrentGame()
        Dim renameDialog As New RenameForm
        Dim selectedTrack As Track = GetCurrentGame.Tracks(TrackList.SelectedIndices(0))

        renameDialog.Filename = selectedTrack.name

        If renameDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            Try

                Rename(game.Libraryname & selectedTrack.name & game.FileExtension, game.Libraryname & renameDialog.Filename & game.FileExtension)
                GetCurrentGame.Tracks(TrackList.SelectedIndices(0)).name = renameDialog.Filename

                SaveTrackKeys(GetCurrentGame)
                ReloadTracks(GetCurrentGame)
                RefreshTrackList()

            Catch ex As Exception
                Select Case ex.HResult
                    Case -2147024809
                        MessageBox.Show("""" & renameDialog.Filename & """ contains invalid characters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

                    Case -2146232800
                        MessageBox.Show("A track with that name already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

                    Case Else
                        MessageBox.Show(ex.Message & " See errorlog.txt for more info.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select

            End Try
        End If
    End Sub

    ''Private Async Sub CheckForUpdate()
    ''Dim UpdateText As String

    ''Dim NeatVersion As String = My.Application.Info.Version.ToString.Remove(My.Application.Info.Version.ToString.LastIndexOf("."))

    ''Try

    ''Using client As New HttpClient
    ''Dim UpdateTextTask As Task(Of String) = client.GetStringAsync("http://slam.flankers.net/updates.php?version=" & NeatVersion)
    ''          UpdateText = Await UpdateTextTask
    ''End Using

    ''Catch ex As Exception
    ''Return
    ''End Try

    ''Dim NewVersion As New Version("0.0.0.0") 'generic
    ''Dim UpdateURL As String = UpdateText.Split()(1)
    ''If Version.TryParse(UpdateText.Split()(0), NewVersion) Then
    ''If My.Application.Info.Version.CompareTo(NewVersion) < 0 Then
    ''If MessageBox.Show(String.Format("An update ({0}) is available! Click ""OK"" to be taken to the download page.", NewVersion.ToString), "SLAM Update", MessageBoxButtons.OKCancel) = Windows.Forms.DialogResult.OK Then
    ''              Process.Start(UpdateURL)
    ''End If
    ''End If
    ''End If
    ''End Sub

    Private Sub PlayKeyButton_Click(sender As Object, e As EventArgs) Handles PlayKeyButton.Click
        Dim selectKeyDialog As New SelectKey
        If selectKeyDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            My.Settings.PlayKey = selectKeyDialog.ChosenKey
            My.Settings.Save()
            RefreshPlayKey()
        End If
    End Sub

    Private Sub RefreshPlayKey()
        PlayKeyButton.Text = String.Format("Play key: ""{0}"" (change)", My.Settings.PlayKey)
    End Sub

    Private Sub LogError(ByVal ex As Exception)
        If My.Settings.LogError Then
            Using log As StreamWriter = New StreamWriter("errorlog.txt", True)
                log.WriteLine("--------------------{0}--------------------", DateTime.Now)
                log.WriteLine(ex.ToString)
            End Using
        End If
    End Sub

    Private Sub ChangeDirButton_Click(sender As Object, e As EventArgs) Handles ChangeDirButton.Click
        SettingsForm.ShowDialog()
    End Sub

    Private Sub DeleteCfGs(ByVal game As SourceGame, ByVal steamappsPath As String)
        Dim gameDir As String = Path.Combine(steamappsPath, game.Directory)
        Dim gameCfgFolder As String = Path.Combine(gameDir, game.ToCfg)
        Dim slamFiles() As String = {"slam.cfg", "slam_tracklist.cfg", "slam_relay.cfg", "slam_curtrack.cfg", "slam_saycurtrack.cfg", "slam_sayteamcurtrack.cfg"}
        Dim voicefile As String = Path.Combine(steamappsPath, game.Directory) & "voice_input.wav"


        Try
            If File.Exists(voicefile) Then
                File.Delete(voicefile)
            End If

            For Each FileName In slamFiles

                If File.Exists(gameCfgFolder & FileName) Then
                    File.Delete(gameCfgFolder & FileName)
                End If

            Next

        Catch ex As Exception
            LogError(ex)
        End Try

    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If _running Then
            StopPoll()
            _closePending = True
            e.Cancel = True
        End If
    End Sub

    Private Sub StreamB_Click(sender As Object, e As EventArgs) Handles StreamB.Click
        Dim spotifyconfirm As Form2
        spotifyconfirm = New Form2()
        spotifyconfirm.Show()
        ' ReSharper disable once RedundantAssignment
        spotifyconfirm = Nothing



    End Sub
End Class