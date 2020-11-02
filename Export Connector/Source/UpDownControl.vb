'****************************************************************************
'*	(c) Copyright Kofax, Inc. 2009. All rights reserved.
'*	Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************

''' <summary>
''' The UpDownControl implements a user control containing Up and Down buttons used
''' to move index fields in the index fields grid view
''' </summary>
''' <remarks></remarks>
Friend Class UpDownControl

    Public Event Up As EventHandler
    Public Event Down As EventHandler

    ''' <summary>
    ''' Gets preferred size for this control
    ''' </summary>
    ''' <param name="proposedSize">Proposed size</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetPreferredSize(ByVal proposedSize As System.Drawing.Size) As System.Drawing.Size
        Return New Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.VerticalScrollBarArrowHeight)
    End Function

    ''' <summary>
    ''' Handles the resize event
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
        MyBase.OnResize(e)

        btnUpArrow.Location = New Point(0, 0)
        btnDownArrow.Location = New Point(0, CInt(Height / 2))
        btnUpArrow.Size = New Size(Width, CInt(Height / 2))
        btnDownArrow.Size = btnUpArrow.Size
    End Sub

    ''' <summary>
    ''' Handles the up event
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub OnUp(ByVal e As EventArgs)
        RaiseEvent Up(Me, e)
    End Sub

    ''' <summary>
    ''' Handles the down event
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub OnDown(ByVal e As EventArgs)
        RaiseEvent Down(Me, e)
    End Sub

    ''' <summary>
    ''' Handles the Up arrow button click event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnUpArrow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpArrow.Click
        OnUp(EventArgs.Empty)
    End Sub

    ''' <summary>
    ''' Handles the Down arrow button click event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnDownArrow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDownArrow.Click
        OnDown(EventArgs.Empty)
    End Sub

    ''' <summary>
    ''' Handles the Up arrow button key down event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnUpArrow_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles btnUpArrow.KeyDown
        If e.KeyCode = Keys.Enter Then
            OnUp(EventArgs.Empty)
        End If
    End Sub

    ''' <summary>
    ''' Handles the Down arrow button key down event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnDownArrow_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles btnDownArrow.KeyDown
        If e.KeyCode = Keys.Enter Then
            OnDown(EventArgs.Empty)
        End If
    End Sub
End Class
