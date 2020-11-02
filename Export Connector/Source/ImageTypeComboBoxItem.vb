'****************************************************************************
'*	(c) Copyright Kofax Inc. 2009. All rights reserved.
'*	Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************

''' <summary>
''' Used to hold image types in a combo box
''' </summary>
''' <remarks></remarks>
Friend Class ImageTypeComboBoxItem

    Private m_strDescription As String

    Private m_eType As ImageFormatEnum

    ''' <summary>
    ''' Create a new instance with a specific image type
    ''' </summary>
    ''' <param name="oImageType">Type of image</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal oImageType As ImageType)
        m_strDescription = oImageType.Description
        m_eType = oImageType.Type
    End Sub

    ''' <summary>
    ''' Create a new instance with image description and image type
    ''' </summary>
    ''' <param name="strDescription">Image description</param>
    ''' <param name="eType">Image type</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal strDescription As String, ByVal eType As ImageFormatEnum)
        m_strDescription = strDescription
        m_eType = eType
    End Sub

    ''' <summary>
    ''' Get a string representing the state of this instance
    ''' </summary>
    ''' <returns>A string representing the state of this instance</returns>
    ''' <remarks></remarks>
    Public Overrides Function ToString() As String
        Return m_strDescription
    End Function

    ''' <summary>
    ''' Get image description
    ''' </summary>
    ''' <value></value>
    ''' <returns>Image description</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Description() As String
        Get
            Return m_strDescription
        End Get
    End Property

    ''' <summary>
    ''' Get image type
    ''' </summary>
    ''' <value></value>
    ''' <returns>Image type</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Type() As ImageFormatEnum
        Get
            Return m_eType
        End Get
    End Property

End Class
