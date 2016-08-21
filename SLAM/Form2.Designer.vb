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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form2))
        Me.ytlink = New System.Windows.Forms.TextBox()
        Me.Streamlb = New System.Windows.Forms.Label()
        Me.convert = New System.Windows.Forms.Button()
        Me.WebBrowser1 = New System.Windows.Forms.WebBrowser()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.SuspendLayout()
        '
        'ytlink
        '
        Me.ytlink.Location = New System.Drawing.Point(12, 25)
        Me.ytlink.Name = "ytlink"
        Me.ytlink.Size = New System.Drawing.Size(328, 20)
        Me.ytlink.TabIndex = 1
        '
        'Streamlb
        '
        Me.Streamlb.AutoSize = True
        Me.Streamlb.Location = New System.Drawing.Point(9, 9)
        Me.Streamlb.Name = "Streamlb"
        Me.Streamlb.Size = New System.Drawing.Size(70, 13)
        Me.Streamlb.TabIndex = 2
        Me.Streamlb.Text = "Youtube Link"
        '
        'convert
        '
        Me.convert.Location = New System.Drawing.Point(12, 51)
        Me.convert.Name = "convert"
        Me.convert.Size = New System.Drawing.Size(328, 23)
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
        Me.ProgressBar1.Size = New System.Drawing.Size(327, 23)
        Me.ProgressBar1.TabIndex = 5
        '
        'Form2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(353, 111)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.convert)
        Me.Controls.Add(Me.Streamlb)
        Me.Controls.Add(Me.ytlink)
        Me.Controls.Add(Me.WebBrowser1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximumSize = New System.Drawing.Size(369, 150)
        Me.MinimumSize = New System.Drawing.Size(369, 150)
        Me.Name = "Form2"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "YouTube Downloader"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ytlink As TextBox
    Friend WithEvents Streamlb As Label
    Friend WithEvents convert As Button
    Friend WithEvents WebBrowser1 As WebBrowser
    Friend WithEvents ProgressBar1 As ProgressBar
End Class
