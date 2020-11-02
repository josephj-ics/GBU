'****************************************************************************
'*	(c) Copyright Kofax, Inc. 2009. All rights reserved.
'*	Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************

''' <summary>
''' Image types currently supported by Kofax Capture.
''' </summary>
''' <remarks></remarks>
Public Enum ImageFormatEnum As Integer
    MTIFF_G4 = 0
    MTIFF_G31D = 1
    MTIFF_RAW = 2
    MTIFF_G32D = 3
    MTIFF_JPEG = 5
    TIFF_G4 = 6
    TIFF_G31D = 7
    TIFF_RAW = 8
    TIFF_G32D = 9
    TIFF_JPEG = 11
    PCX_PACKBYTES = 12
    JPG_JPEG = 14
    MTIFF_LZW = 16
End Enum

''' <summary>
''' Common Utilities
''' </summary>
''' <remarks></remarks>
Module Common

    ' Contains all page indexes on the setup dialog
    Public Enum SetupFormTabEnum
        DefaultStoragePage = 0
        IndexStoragePage = 1
        CustomStoragePage = 2
    End Enum

    ''' <summary>
    ''' Gets/Sets error handler
    ''' </summary>
    ''' <value>Error handler</value>
    ''' <returns>Error handler</returns>
    ''' <remarks>Used to record all errors to the Capture error log</remarks>
    Public ReadOnly Property CommonErrorHandler() As ErrorHandler
        Get
            Return m_oErrorHandler
        End Get
    End Property

    ' Error Messages
    Public Const MSG_UNCSHARENOTEXIST As Short = 1018

    Private m_frmSetup As MainSetupForm
    Private m_oErrorHandler As New ErrorHandler

    ''' <summary>
    ''' Gets/Sets setup form
    ''' </summary>
    ''' <value>Main setup form</value>
    ''' <returns>Main setup form</returns>
    ''' <remarks></remarks>
    Public Property SetupForm() As MainSetupForm
        Get
            Return m_frmSetup
        End Get
        Set(ByVal value As MainSetupForm)
            m_frmSetup = value
        End Set
    End Property
End Module