<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form2))
        Me.ytlink = New System.Windows.Forms.TextBox()
        Me.Streamlb = New System.Windows.Forms.Label()
        Me.convert = New System.Windows.Forms.Button()
        Me.WebBrowser1 = New System.Windows.Forms.WebBrowser()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.songname = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'ytlink
        '
        Me.ytlink.Location = New System.Drawing.Point(12, 25)
        Me.ytlink.Name = "ytlink"
        Me.ytlink.Size = New System.Drawing.Size(297, 20)
        Me.ytlink.TabIndex = 1
        '
        'Streamlb
        '
        Me.Streamlb.AutoSize = True
        Me.Streamlb.Location = New System.Drawing.Point(9, 9)
        Me.Streamlb.Name = "Streamlb"
        Me.Streamlb.Size = New System.Drawing.Size(69, 13)
        Me.Streamlb.TabIndex = 2
        Me.Streamlb.Text = "Link to Video"
        '
        'convert
        '
        Me.convert.Location = New System.Drawing.Point(12, 51)
        Me.convert.Name = "convert"
        Me.convert.Size = New System.Drawing.Size(460, 23)
        Me.convert.TabIndex = 3
        Me.convert.Text = "Convert and Download"
        Me.convert.UseVisualStyleBackColor = True
        '
        'WebBrowser1
        '
        Me.WebBrowser1.Location = New System.Drawing.Point(12, 102)
        Me.WebBrowser1.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebBrowser1.Name = "WebBrowser1"
        Me.WebBrowser1.Size = New System.Drawing.Size(460, 447)
        Me.WebBrowser1.TabIndex = 4
        Me.WebBrowser1.Visible = False
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(13, 81)
        Me.ProgressBar1.Maximum = 5
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(459, 23)
        Me.ProgressBar1.TabIndex = 5
        '
        'Timer1
        '
        '
        'songname
        '
        Me.songname.Location = New System.Drawing.Point(315, 25)
        Me.songname.Name = "songname"
        Me.songname.Size = New System.Drawing.Size(157, 20)
        Me.songname.TabIndex = 6
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(315, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(63, 13)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "Song Name"
        '
        'Form2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(484, 111)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.songname)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.convert)
        Me.Controls.Add(Me.Streamlb)
        Me.Controls.Add(Me.ytlink)
        Me.Controls.Add(Me.WebBrowser1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximumSize = New System.Drawing.Size(500, 150)
        Me.MinimumSize = New System.Drawing.Size(500, 150)
        Me.Name = "Form2"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Music Downloader"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ytlink As TextBox
    Friend WithEvents Streamlb As Label
    Friend WithEvents convert As Button
    Friend WithEvents WebBrowser1 As WebBrowser
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents Timer1 As Timer
    Friend WithEvents songname As TextBox
    Friend WithEvents Label1 As Label
End Class
