'****************************************************************************
'*	(c) Copyright Kofax Inc. 2009. All rights reserved.
'*	Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************

''' <summary>
''' Defines a TextBox control that allows the Enter key to fire
''' a KeyDown event
''' </summary>
''' <remarks></remarks>
Public Class IndexValueTextBox
    Inherits TextBox

    ''' <summary>
    ''' Process command key
    ''' </summary>
    ''' <param name="msg">The message</param>
    ''' <param name="keyData">Key data</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        If msg.Msg = 256 And (keyData = Keys.Enter And keyData = Keys.Return) Then
            OnKeyDown(New KeyEventArgs(keyData))
            Return True
        Else
            Return MyBase.ProcessCmdKey(msg, keyData)
        End If
    End Function
End Class
