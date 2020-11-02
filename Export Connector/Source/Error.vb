'****************************************************************************
'*	(c) Copyright Kofax, Inc. 2009. All rights reserved.
'*	Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************
Imports Kofax.ReleaseLib

''' <summary>
''' The TextError class implements error handler for release process.
''' </summary>
''' <remarks></remarks>
Friend Class TextError
    Public TitleString As String

    Private m_setupData As Kofax.ReleaseLib.ReleaseSetupData
    Private m_releaseData As ReleaseData

    ''' <summary>
    ''' VB calls this event when an object
    ''' of this class is instantiated.  We
    ''' simply initialize the variables.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Class_Initialize_Renamed()
        TitleString = My.Resources.TXT_SHORT_PRODUCT_NAME
    End Sub

    ''' <summary>
    ''' Create a new instance of this class
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New()
        Class_Initialize_Renamed()
    End Sub

    ''' <summary>
    ''' Gets/Sets data object
    ''' </summary>
    ''' <value></value>
    ''' <remarks>Force late bind as we won't know if this module is being used in Setup Release or Release
    ''' </remarks>
    Public WriteOnly Property DataObject() As Object
        Set(ByVal value As Object)
            If TypeOf value Is ReleaseData Then
                m_releaseData = CType(value, ReleaseData)
            Else
                m_setupData = CType(value, Kofax.ReleaseLib.ReleaseSetupData)
            End If
        End Set
    End Property

    ''' <summary>
    ''' This routine logs an error to the
    ''' Capture error log and may optionally
    ''' re-raise the error and/or display it
    ''' to the user.
    ''' </summary>
    ''' <param name="ErrNum">Error Number</param>
    ''' <param name="ErrMsg">Error Message</param>
    ''' <param name="SourceFile">The source module in which the error occurred</param>
    ''' <remarks>The initialization routine for the error handler must be performed
    '''           before calling this routine.
    ''' </remarks>
    Public Sub LogTheError(ByRef ErrNum As Integer, ByRef ErrMsg As String, ByRef SourceFile As String)
        ' First log the error through the
        ' setup or release data object
        If m_releaseData IsNot Nothing Then
            m_releaseData.LogError(ErrNum, 0, 0, ErrMsg, SourceFile, 0)
        ElseIf Me.m_setupData IsNot Nothing Then
            m_setupData.LogError(ErrNum, 0, 0, ErrMsg, SourceFile, 0)
        End If

        ' If the caller wants a message box displayed for this
        ' error, pop it up with the standard title
        If m_releaseData Is Nothing AndAlso m_setupData Is Nothing Then
            MsgBox(ErrMsg & vbCr & "(#" & CStr(ErrNum) & ")", MsgBoxStyle.OkOnly Or MsgBoxStyle.Exclamation, TitleString) ' DO NOT LOCALIZE
        End If
    End Sub
End Class