'****************************************************************************
'*	(c) Copyright Kofax, Inc. 2009. All rights reserved.
'*	Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************
Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' The ArrowButton class implements an arrow button
''' used to move index fields in the index field grid view up or down.
''' </summary>
''' <remarks></remarks>
Friend Class ArrowButton
    Inherits Button

#Region "Private variables"
    Private WithEvents m_oTimer As New Timer()
    Private ReadOnly m_nDelay As Integer = 250 * (1 + SystemInformation.KeyboardDelay)
    Private ReadOnly m_nSpeed As Integer = 405 - 12 * SystemInformation.KeyboardSpeed
    Private m_btnScrollButton As ScrollButton
#End Region

#Region "Event handlers"
    ''' <summary>
    ''' Handles the mouse down event
    ''' </summary>
    ''' <param name="mevent"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnMouseDown(ByVal mevent As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseDown(mevent)

        If (mevent.Button And Windows.Forms.MouseButtons.Left) <> 0 Then
            m_oTimer.Interval = m_nDelay
            m_oTimer.Start()
        End If
    End Sub

    ''' <summary>
    ''' Handles the timer tick event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    ''' <remarks></remarks>
    Private Sub TimerOnTick(ByVal sender As Object, ByVal args As EventArgs) Handles m_oTimer.Tick
        OnClick(EventArgs.Empty)
        m_oTimer.Interval = m_nSpeed
    End Sub

    ''' <summary>
    ''' Handles the mouse move event
    ''' </summary>
    ''' <param name="mevent"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnMouseMove(ByVal mevent As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseMove(mevent)
        m_oTimer.Enabled = Capture And ClientRectangle.Contains(mevent.Location)
    End Sub

    ''' <summary>
    ''' Handles the mouse up event
    ''' </summary>
    ''' <param name="mevent"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnMouseUp(ByVal mevent As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseUp(mevent)
        m_oTimer.Stop()
    End Sub

    ''' <summary>
    ''' Gets/Sets scroll button
    ''' </summary>
    ''' <value>Scroll button</value>
    ''' <returns>Scroll button</returns>
    ''' <remarks></remarks>
    Public Property ScrollButton() As ScrollButton
        Get
            Return m_btnScrollButton
        End Get
        Set(ByVal value As ScrollButton)
            m_btnScrollButton = value
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Handles the paint event
    ''' </summary>
    ''' <param name="pevent"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnPaint(ByVal pevent As System.Windows.Forms.PaintEventArgs)
        Dim oGraphics As Graphics = pevent.Graphics
        Dim oButtonState As ButtonState
        If Enabled Then
            If Capture And ClientRectangle.Contains(PointToClient(MousePosition)) Then
                oButtonState = ButtonState.Pushed
            Else
                oButtonState = ButtonState.Normal
            End If
        Else
            oButtonState = ButtonState.Inactive
        End If
        ControlPaint.DrawScrollButton(oGraphics, ClientRectangle, m_btnScrollButton, oButtonState)
    End Sub

    ''' <summary>
    ''' Handles mouse captured changed event
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnMouseCaptureChanged(ByVal e As System.EventArgs)
        MyBase.OnMouseCaptureChanged(e)
        Invalidate()
    End Sub
#End Region
End Class
