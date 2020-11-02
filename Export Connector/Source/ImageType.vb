'****************************************************************************
'*	(c) Copyright Kofax Inc. 2009. All rights reserved.
'*	Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************

Option Strict Off

''' <summary>
''' Adapter to provide access to properties in an image type returned
''' by ReleaseSetupData using late binding because
''' ReleaseSetupData.ImageTypes returns a value of type "Object"
''' </summary>
''' <remarks></remarks>
Friend Class ImageType

    Private m_oImageType As Object

    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="oImageType">Type of the image</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal oImageType As Object)
        Me.m_oImageType = oImageType
    End Sub

    ''' <summary>
    ''' Get image description.
    ''' </summary>
    ''' <value></value>
    ''' <returns>Description of the image</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Description() As String
        Get
            Return m_oImageType.Description
        End Get
    End Property

    ''' <summary>
    ''' Get value indicates whether image is multi page or not.
    ''' </summary>
    ''' <value></value>
    ''' <returns>True if image is multi page; otherwise, False</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property MultiplePage() As Boolean
        Get
            Return m_oImageType.MultiplePage
        End Get
    End Property

    ''' <summary>
    ''' Get image type.
    ''' </summary>
    ''' <value></value>
    ''' <returns>Type of the image</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Type() As ImageFormatEnum
        Get
            Return m_oImageType.Type
        End Get
    End Property


End Class
