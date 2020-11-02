'****************************************************************************
'*	(c) Copyright Kofax Inc. 2009. All rights reserved.
'*	Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************

Imports System.ComponentModel

''' <summary>
''' Defines a DataGridView that contains a reference to
''' ReleaseSetupData and a work around to what looks like
''' a bug with DataGridViews and COM Interop where validation
''' doesn't occur so we have to force Validating events to fire
''' </summary>
''' <remarks></remarks>
Public Class IndexValueDataGridView
    Inherits DataGridView

    'This event will be raised when user presses Ctrl + Up key
    Public Event IndexValueDataGridItemMoveUp(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)

    'This event will be raised when user presses Ctrl + Down key
    Public Event IndexValueDataGridItemMoveDown(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)

    'This event will be raised when user presses ENTER key
    Public Event IndexValueDataGridItemEnterKey(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)

    Private m_btnDelete As Button
    Private m_oSetupData As Kofax.ReleaseLib.ReleaseSetupData

    ''' <summary>
    ''' Gets/Sets the Delete button
    ''' </summary>
    ''' <value>Delete button</value>
    ''' <returns>The delete button</returns>
    ''' <remarks></remarks>
    Public Property DeleteButton() As Button
        Get
            Return m_btnDelete
        End Get
        Set(ByVal value As Button)
            m_btnDelete = value
        End Set
    End Property

    ''' <summary>
    ''' Gets/Sets setup data
    ''' </summary>
    ''' <value>Release setup data</value>
    ''' <returns>Release setup data</returns>
    ''' <remarks></remarks>
    Public Property SetupData() As Kofax.ReleaseLib.ReleaseSetupData
        Get
            Return m_oSetupData
        End Get
        Set(ByVal value As Kofax.ReleaseLib.ReleaseSetupData)
            m_oSetupData = value
        End Set
    End Property

    ''' <summary>
    ''' Causes the DataGridView to fire the Validating event
    ''' </summary>
    ''' <remarks>
    ''' This is a workaround to a problem where validating events
    ''' aren't getting fired when the DataGridView loses focus when
    ''' used with COM Interop (or something else we're doing)
    ''' </remarks>
    Public Sub PerformValidation()
        OnValidating(New CancelEventArgs())
    End Sub

    ''' <summary>
    ''' Raises move up event
    ''' </summary>
    ''' <param name="eventSender">Object which sends out the event</param>
    ''' <param name="eventArgs">Parameters of event</param>
    ''' <remarks></remarks>
    Public Sub RaiseMoveUpEvent(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)
        RaiseEvent IndexValueDataGridItemMoveUp(eventSender, eventArgs)
    End Sub

    ''' <summary>
    ''' Raises move down event
    ''' </summary>
    ''' <param name="eventSender">Object which sends out the event</param>
    ''' <param name="eventArgs">Parameters of event</param>
    ''' <remarks></remarks>
    Public Sub RaiseMoveDownEvent(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)
        RaiseEvent IndexValueDataGridItemMoveDown(eventSender, eventArgs)
    End Sub

    ''' <summary>
    ''' Raises enter press event
    ''' </summary>
    ''' <param name="eventSender">Object which sends out the event</param>
    ''' <param name="eventArgs">Parameters of event</param>
    ''' <remarks></remarks>
    Public Sub RaiseEnterPressEvent(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)
        RaiseEvent IndexValueDataGridItemEnterKey(eventSender, eventArgs)
    End Sub

End Class
