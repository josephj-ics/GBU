'********************************************************************************
'***   (c)Copyright Kofax Inc. 2009 All rights reserved.
'***   Unauthorized use, duplication or distribution is strictly prohibited.
'********************************************************************************
Imports Kofax.ReleaseLib
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Text
Imports Kofax.Connector.Common

''' <summary>
''' Facade for accessing a release script's custom properties.
''' </summary>
''' <remarks></remarks>
Friend Class CustomPropertiesReaderWriter
    Implements IDisposable

    ' Default storage tab
    ' Old
    Private Const cm_strKeyDefaultFileName As String = "Default File Name"
    Private Const cm_strKeyFileNaming As String = "Plus Filenaming"
    Private Const cm_strKeyLeadingZeros As String = "Leading Zeros"
    Private Const cm_strKeyDupHandling As String = "Duplicate Handling"

    ' Index storage
    ' New
    Private Const cm_strKeyUseDefaultStorage As String = "Use Default Storage"
    Private Const cm_strKeyUseCustomStorage As String = "Use Custom Storage"
    Private Const cm_strKeyCustomIndexFileName As String = "Custom Index File Name"

    ' Old
    Private Const cm_strKeyEncoding As String = "Encoding"

    ' Advance
    ' New
    Private Const cm_strKeyOcrDefaultStorage As String = "OCR Default Storage"
    Private Const cm_strKeyImgDefaultStorage As String = "IMG Default Storage"
    Private Const cm_strKeyPdfDefaultStorage As String = "PDF Default Storage"
    Private Const cm_strKeySuppressIfPdfDetected As String = "Suppress image if pdf detected"


    ' Old
    Private Const cm_strKeyEnableKofaxPdfExport As String = "EnableKofaxPDFExport"
    Private Const cm_strKeyDisableTextExport As String = "DisableTextExport"
    Private Const cm_strKeyDisableImageExport As String = "DisableImageExport"

    ' Deprecated
    Private Const cm_strKeyAsciiFile As String = "ASCII File Name"
    Private Const cm_strKeySeparateDirectories As String = "ReleaseSeparateDir"
    Private Const cm_strKeyFolders As String = "Plus Folders"
    Private Const cm_strKeyIndexFileName As String = "Index File Name"

    ' CustomProperties object
    Private m_oCustomProperties As ICustomProperties

    ''' <summary>
    ''' Constructor of the class.
    ''' </summary>
    ''' <param name="customProperties"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal customProperties As CustomProperties)
        Debug.Assert(customProperties IsNot Nothing)
        Me.m_oCustomProperties = CType(customProperties, ICustomProperties)
    End Sub

    ''' <summary>
    ''' Suppress if pdf detected property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SuppressIfPdfDetected() As Boolean
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeySuppressIfPdfDetected)
            Dim bValue As Boolean = False
            If oCustomProperty IsNot Nothing Then
                Using New ComDisposer(oCustomProperty)
                    bValue = ConvertToBooleanValue(oCustomProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            m_oCustomProperties.Add(cm_strKeySuppressIfPdfDetected, value.ToString())
        End Set
    End Property

    ''' <summary>
    ''' Custom index file name property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CustomIndexFileName() As String
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyCustomIndexFileName)
            Using New ComDisposer(oCustomProperty)
                If oCustomProperty Is Nothing OrElse String.IsNullOrEmpty(oCustomProperty.Value) Then
                    Return Nothing
                Else
                    Return oCustomProperty.Value
                End If
            End Using
        End Get
        Set(ByVal value As String)
            m_oCustomProperties.Add(cm_strKeyCustomIndexFileName, value.ToString())
        End Set
    End Property

    ''' <summary>
    ''' Index file name property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IndexFileName() As String
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyIndexFileName)
            If oCustomProperty Is Nothing Then
                ' Legacy ASCII File Name property
                oCustomProperty = TryGetValue(cm_strKeyAsciiFile)
            End If
            Using New ComDisposer(oCustomProperty)
                If oCustomProperty IsNot Nothing Then
                    Return oCustomProperty.Value
                Else
                    Return String.Empty
                End If
            End Using
        End Get
        Set(ByVal value As String)
            m_oCustomProperties.Add(cm_strKeyIndexFileName, value)
        End Set
    End Property

    ''' <summary>
    ''' Image default storage property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ImgDefaultStorage() As Boolean
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyImgDefaultStorage)
            Dim bValue As Boolean = False
            If oCustomProperty IsNot Nothing Then
                Using New ComDisposer(oCustomProperty)
                    bValue = ConvertToBooleanValue(oCustomProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            m_oCustomProperties.Add(cm_strKeyImgDefaultStorage, value.ToString())
        End Set
    End Property

    ''' <summary>
    ''' Pdf default storage property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PdfDefaultStorage() As Boolean
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyPdfDefaultStorage)
            Dim bValue As Boolean = False
            If oCustomProperty IsNot Nothing Then
                Using New ComDisposer(oCustomProperty)
                    bValue = ConvertToBooleanValue(oCustomProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            m_oCustomProperties.Add(cm_strKeyPdfDefaultStorage, value.ToString())
        End Set
    End Property

    ''' <summary>
    ''' Ocr default storage property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OcrDefaultStorage() As Boolean
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyOcrDefaultStorage)
            Dim bValue As Boolean = False
            If oCustomProperty IsNot Nothing Then
                Using New ComDisposer(oCustomProperty)
                    bValue = ConvertToBooleanValue(oCustomProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            m_oCustomProperties.Add(cm_strKeyOcrDefaultStorage, value.ToString())
        End Set
    End Property

    ''' <summary>
    ''' Use custom storage property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UseCustomStorage() As Boolean
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyUseCustomStorage)
            Dim bValue As Boolean = False
            If oCustomProperty IsNot Nothing Then
                Using New ComDisposer(oCustomProperty)
                    bValue = ConvertToBooleanValue(oCustomProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            m_oCustomProperties.Add(cm_strKeyUseCustomStorage, value.ToString())
        End Set
    End Property

    ''' <summary>
    ''' Use default storage property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UseDefaultStorage() As Boolean
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyUseDefaultStorage)
            Dim bValue As Boolean = False
            If oCustomProperty IsNot Nothing Then
                Using New ComDisposer(oCustomProperty)
                    bValue = ConvertToBooleanValue(oCustomProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            m_oCustomProperties.Add(cm_strKeyUseDefaultStorage, value.ToString())
        End Set
    End Property

    ''' <summary>
    ''' Leading zeros property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LeadingZeros() As Boolean
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyLeadingZeros)
            Dim bValue As Boolean = False
            If oCustomProperty IsNot Nothing Then
                Using New ComDisposer(oCustomProperty)
                    bValue = ConvertToBooleanValue(oCustomProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            m_oCustomProperties.Add(cm_strKeyLeadingZeros, value.ToString())
        End Set
    End Property

    ''' <summary>
    ''' Folders property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Folders() As Integer
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyFolders)
            Using New ComDisposer(oCustomProperty)
                If oCustomProperty Is Nothing OrElse String.IsNullOrEmpty(oCustomProperty.Value) Then
                    Return 0
                Else
                    Return Integer.Parse(oCustomProperty.Value)
                End If
            End Using
        End Get
        Set(ByVal value As Integer)
            ' Folder property is set more than once so need to check to see
            ' if it already exists.  Otherwise the second Add throws.
            ' TODO: Modify ICustomProperties2 implementation instead of doing this
            ' here.  That would be a better, global fix.
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyFolders)
            Using New ComDisposer(oCustomProperty)
                If oCustomProperty IsNot Nothing Then
                    m_oCustomProperties.Item(cm_strKeyFolders).Value = value.ToString()
                Else
                    m_oCustomProperties.Add(cm_strKeyFolders, value.ToString())
                End If
            End Using
        End Set
    End Property

    ''' <summary>
    ''' Duplicate handling property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DuplicateHandling() As String
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyDupHandling)
            Using New ComDisposer(oCustomProperty)
                If oCustomProperty IsNot Nothing Then
                    Return oCustomProperty.Value
                Else
                    Return c_strCPValReplace
                End If
            End Using
        End Get
        Set(ByVal value As String)
            m_oCustomProperties.Add(cm_strKeyDupHandling, value)
        End Set
    End Property

    ''' <summary>
    ''' File naming property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FileNaming() As String
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyFileNaming)
            Using New ComDisposer(oCustomProperty)
                If oCustomProperty IsNot Nothing Then
                    Return oCustomProperty.Value
                Else
                    Return c_strCPValNone
                End If
            End Using
        End Get
        Set(ByVal value As String)
            m_oCustomProperties.Add(cm_strKeyFileNaming, value)
        End Set
    End Property

    ''' <summary>
    ''' Separate directories property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SeparateDirectories() As Boolean
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeySeparateDirectories)
            Dim bValue As Boolean = False
            If oCustomProperty IsNot Nothing Then
                Using New ComDisposer(oCustomProperty)
                    bValue = ConvertToBooleanValue(oCustomProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            m_oCustomProperties.Add(cm_strKeySeparateDirectories, value.ToString())
        End Set
    End Property

    ''' <summary>
    ''' Default file name property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DefaultFileName() As String
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyDefaultFileName)
            If oCustomProperty Is Nothing Then
                ' Legacy ASCII File Name property
                oCustomProperty = TryGetValue(cm_strKeyAsciiFile)
            End If
            Using New ComDisposer(oCustomProperty)
                If oCustomProperty IsNot Nothing Then
                    Return oCustomProperty.Value
                Else
                    Return String.Empty
                End If
            End Using
        End Get
        Set(ByVal value As String)
            m_oCustomProperties.Add(cm_strKeyDefaultFileName, value)
        End Set
    End Property

    ''' <summary>
    ''' Disable image export property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DisableImageExport() As Boolean
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyDisableImageExport)
            Dim bValue As Boolean = False
            If oCustomProperty IsNot Nothing Then
                Using New ComDisposer(oCustomProperty)
                    bValue = ConvertToBooleanValue(oCustomProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            m_oCustomProperties.Add(cm_strKeyDisableImageExport, value.ToString())
        End Set
    End Property

    ''' <summary>
    ''' Disable text export property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DisableTextExport() As Boolean
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyDisableTextExport)
            Dim bValue As Boolean = False
            If oCustomProperty IsNot Nothing Then
                Using New ComDisposer(oCustomProperty)
                    bValue = ConvertToBooleanValue(oCustomProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            m_oCustomProperties.Add(cm_strKeyDisableTextExport, value.ToString())
        End Set
    End Property

    ''' <summary>
    ''' Encoding property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Encoding() As Encoding
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyEncoding)
            Using New ComDisposer(oCustomProperty)
                If oCustomProperty Is Nothing Then
                    Return System.Text.Encoding.ASCII
                Else
                    Select Case oCustomProperty.Value
                        Case System.Text.Encoding.Unicode.EncodingName
                            Return System.Text.Encoding.Unicode

                        Case System.Text.Encoding.UTF8.EncodingName
                            Return System.Text.Encoding.UTF8

                        Case Else
                            Return System.Text.Encoding.ASCII
                    End Select
                End If
            End Using
        End Get
        Set(ByVal value As Encoding)
            If value Is System.Text.Encoding.ASCII Or value Is System.Text.Encoding.Unicode Or value Is System.Text.Encoding.UTF8 Then
                m_oCustomProperties.Add(cm_strKeyEncoding, value.EncodingName)
            Else
                Throw New ArgumentOutOfRangeException("value", "Encoding must be ASCII, Unicode, or UTF8")
            End If
        End Set
    End Property

    ''' <summary>
    ''' Enable kofax pdf export property.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EnableKofaxPdfExport() As Boolean
        Get
            Dim oCustomProperty As CustomProperty
            oCustomProperty = TryGetValue(cm_strKeyEnableKofaxPdfExport)
            Dim bValue As Boolean = False
            If oCustomProperty IsNot Nothing Then
                Using New ComDisposer(oCustomProperty)
                    bValue = ConvertToBooleanValue(oCustomProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            m_oCustomProperties.Add(cm_strKeyEnableKofaxPdfExport, CStr(value))
        End Set
    End Property

    ''' <summary>
	''' Check if this Release Data is above 9.0
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Friend Function IsNewVersion() As Boolean
		Try
			Debug.Assert(m_oCustomProperties IsNot Nothing)
			Dim oTemp As CustomProperty = Me.m_oCustomProperties.Item(cm_strKeyUseDefaultStorage)
			Return True
		Catch ex As Exception
			' Old Release Data doesn't have "Use Default Storage" Custom Property
			Return False
		End Try
	End Function

	''' <summary>
    ''' Get property value.
    ''' </summary>
    ''' <param name="propertyName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function TryGetValue(ByVal propertyName As String) As CustomProperty
        Try
            Return Me.m_oCustomProperties.Item(propertyName)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Convert property string value to boolean.
    ''' </summary>
    ''' <param name="strValue"></param>
    ''' <returns>boolean value if can convert; otherwise false</returns>
    ''' <remarks></remarks>
    Private Function ConvertToBooleanValue(ByVal strValue As String) As Boolean

        Dim bValue As Boolean = False

        If Not String.IsNullOrEmpty(strValue) Then
            If Not Boolean.TryParse(strValue, bValue) Then
                '*** For backward compatible, in case the value 
                '*** was stored with localized string, such as
                '*** 'Wahr' (in German) instead of 'True'
                bValue = String.Equals( _
                            strValue, _
                            My.Resources.TXT_LocalizedValueTrue, _
                            StringComparison.OrdinalIgnoreCase)
            End If
        End If

        Return bValue
    End Function

    ''' <summary>
    ''' Dispose method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Dispose() Implements IDisposable.Dispose
        If m_oCustomProperties IsNot Nothing Then
            Marshal.FinalReleaseComObject(m_oCustomProperties)
            m_oCustomProperties = Nothing
        End If
    End Sub

End Class
