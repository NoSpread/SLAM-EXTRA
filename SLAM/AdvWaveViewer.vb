
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Windows.Forms
Imports NAudio.Wave

''' <summary>
''' Control for viewing waveforms
''' </summary>
Public Class AdvWaveViewer
    Inherits System.Windows.Forms.UserControl
    ''' <summary> 
    ''' Required designer variable.
    ''' </summary>
    Private _components As System.ComponentModel.Container = Nothing
    Private _mWaveStream As WaveStream
    Private _mSamplesPerPixel As Integer = 128
    Private _mStartPosition As Long
    Private _bytesPerSample As Integer = 2
    ''' <summary>
    ''' Creates a new WaveViewer control
    ''' </summary>
    Public Sub New()
        ' This call is required by the Windows.Forms Form Designer.
        InitializeComponent()

        Me.DoubleBuffered = True

    End Sub

    Public Sub Fit()
        If _mWaveStream Is Nothing Then
            Return
        End If

        If Not Me.Width > 0 Then
            Return
        End If

        Dim samples As Integer = CInt(_mWaveStream.Length / _bytesPerSample)
        _mStartPosition = 0
        SamplesPerPixel = samples / Me.Width

    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        Fit()
    End Sub

    Private _mousePos As Point, _startPos As Point
    Private _mouseDrag As Boolean = False

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        If e.Button = System.Windows.Forms.MouseButtons.Left And Me.Enabled Then

            _startPos = e.Location
            _mousePos = New Point(-1, -1)
            _mouseDrag = True
            DrawVerticalLine(e.X)

        End If

        MyBase.OnMouseDown(e)
    End Sub

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        If e.X >= 0 And e.X <= Me.Width And Me.Enabled Then
            If _mouseDrag Then
                DrawVerticalLine(e.X)
                If _mousePos.X <> -1 Then
                    DrawVerticalLine(_mousePos.X)
                End If
                _mousePos = e.Location
            End If
        End If
        MyBase.OnMouseMove(e)
    End Sub

    Public ReadOnly Property MaxSamples As Integer
        Get
            Return _mWaveStream.Length / _bytesPerSample
        End Get
    End Property

    Private _mLeftSample As Integer = -1
    Public Property LeftSample As Integer
        Get
            Return _mLeftSample
        End Get
        Set(value As Integer)
            _mLeftSample = value
            Me.Invalidate()
        End Set
    End Property
    Private _mRightSample As Integer = -1
    Public Property RightSample As Integer
        Get
            Return _mRightSample
        End Get
        Set(value As Integer)
            _mRightSample = value
            Me.Invalidate()
        End Set
    End Property

    Public Property Leftpos As Integer
        Get
            Return _mLeftSample * _bytesPerSample
        End Get
        Set(value As Integer)
            _mLeftSample = value / _bytesPerSample
            Me.Invalidate()
        End Set
    End Property

    Public Property Rightpos As Integer
        Get
            Return _mRightSample * _bytesPerSample
        End Get
        Set(value As Integer)
            _mRightSample = value / _bytesPerSample
            Me.Invalidate()
        End Set
    End Property

    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        If _mouseDrag AndAlso e.Button = System.Windows.Forms.MouseButtons.Left AndAlso Me.Enabled Then
            _mouseDrag = False
            'DrawVerticalLine(startPos.X)

            If _mousePos.X = -1 Then

                If Not _startPos.X = 0 Then
                    DrawVerticalLine(_startPos.X)
                End If

                Return
            End If
            'DrawVerticalLine(mousePos.X)

            _mLeftSample = CInt(StartPosition \ _bytesPerSample + _mSamplesPerPixel * Math.Min(_startPos.X, _mousePos.X))
            _mRightSample = CInt(StartPosition \ _bytesPerSample + _mSamplesPerPixel * Math.Max(_startPos.X, _mousePos.X))
            Me.Invalidate()
        End If

        MyBase.OnMouseUp(e)
    End Sub

    Private Sub DrawVerticalLine(x As Integer)
        ControlPaint.DrawReversibleLine(PointToScreen(New Point(x, 0)), PointToScreen(New Point(x, Height)), Color.Black)
    End Sub

    ''' <summary>
    ''' sets the associated wavestream
    ''' </summary>
    Public Property WaveStream() As WaveStream
        Get
            Return _mWaveStream
        End Get
        Set(value As WaveStream)
            _mWaveStream = value
            If _mWaveStream IsNot Nothing Then
                _bytesPerSample = (_mWaveStream.WaveFormat.BitsPerSample / 8) * _mWaveStream.WaveFormat.Channels
                Fit()
            End If
            Me.Invalidate()
        End Set
    End Property

    Public ReadOnly Property SampleRate As Integer
        Get
            Return _mWaveStream.WaveFormat.SampleRate
        End Get
    End Property

    ''' <summary>
    ''' The zoom level, in samples per pixel
    ''' </summary>
    Public Property SamplesPerPixel() As Integer
        Get
            Return _mSamplesPerPixel
        End Get
        Set(value As Integer)
            _mSamplesPerPixel = value
            Me.Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Start position (currently in bytes)
    ''' </summary>
    Public Property StartPosition() As Long
        Get
            Return _mStartPosition
        End Get
        Set(value As Long)
            _mStartPosition = value
        End Set
    End Property

    Private _mMarker As Integer
    Public Property Marker() As Long
        Get
            Return _mMarker
        End Get
        Set(value As Long)
            If value <= MaxSamples Then
                _mMarker = value
                Me.Invalidate()
            End If
        End Set
    End Property

    ''' <summary> 
    ''' Clean up any resources being used.
    ''' </summary>
    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            If _components IsNot Nothing Then
                _components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ''' <summary>
    ''' <see cref="Control.OnPaint"/>
    ''' </summary>
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        If _mWaveStream IsNot Nothing Then
            _mWaveStream.Position = 0
            Dim bytesRead As Integer
            Dim waveData As Byte() = New Byte(_mSamplesPerPixel * _bytesPerSample - 1) {}
            _mWaveStream.Position = _mStartPosition + (e.ClipRectangle.Left * _bytesPerSample * _mSamplesPerPixel)

            Dim leftpos As Integer = CInt(_mLeftSample \ _mSamplesPerPixel - StartPosition \ _bytesPerSample \ _mSamplesPerPixel)
            Dim rightpos As Integer = CInt(_mRightSample \ _mSamplesPerPixel - StartPosition \ _bytesPerSample \ _mSamplesPerPixel)
            Dim markerpos As Integer = CInt((_mMarker + _mLeftSample) \ _mSamplesPerPixel - StartPosition \ _bytesPerSample \ _mSamplesPerPixel)

            For x As Single = e.ClipRectangle.X To e.ClipRectangle.Right - 1
                Dim low As Short = 0
                Dim high As Short = 0
                bytesRead = _mWaveStream.Read(waveData, 0, _mSamplesPerPixel * _bytesPerSample)
                If bytesRead = 0 Then
                    Exit For
                End If
                For n As Integer = 0 To bytesRead - 1 Step 2
                    Dim sample As Short = BitConverter.ToInt16(waveData, n)
                    If sample < low Then
                        low = sample
                    End If
                    If sample > high Then
                        high = sample
                    End If
                Next
                Dim lowPercent As Single = ((CSng(low) - Short.MinValue) / UShort.MaxValue)
                Dim highPercent As Single = ((CSng(high) - Short.MinValue) / UShort.MaxValue)
                Using dodgerBluePen As New Pen(Color.DodgerBlue), bluePen As New Pen(Color.Blue), redPen As New Pen(Color.Red), greenPen As New Pen(Color.Green)

                    If x = leftpos And Not leftSample = rightSample Or x = rightpos And Not rightSample = leftSample Then
                        e.Graphics.DrawLine(RedPen, x, 0, x, Me.Height)

                    ElseIf x = markerpos And _mMarker > 0 Then
                        e.Graphics.DrawLine(GreenPen, x, 0, x, Me.Height)

                    ElseIf x > leftpos And x < rightpos Then
                        e.Graphics.DrawLine(BluePen, x, Me.Height * lowPercent, x, Me.Height * highPercent)

                    Else
                        e.Graphics.DrawLine(DodgerBluePen, x, Me.Height * lowPercent, x, Me.Height * highPercent)

                    End If

                End Using
            Next
        End If

        MyBase.OnPaint(e)
    End Sub


#Region "Component Designer generated code"
    ''' <summary> 
    ''' Required method for Designer support - do not modify 
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        _components = New System.ComponentModel.Container()
    End Sub
#End Region
End Class
