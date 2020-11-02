'********************************************************************************
'***   (c)Copyright Kofax Inc. 2009 All rights reserved.
'***   Unauthorized use, duplication or distribution is strictly prohibited.
'********************************************************************************
Imports System.Text

''' <summary>
''' This class implements IEquatable. This is used to hold encoding types in a combo box.
''' </summary>
''' <remarks></remarks>
Friend Class EncodingComboBoxItem
    Implements IEquatable(Of EncodingComboBoxItem)
    Implements IEquatable(Of Encoding)

    Private m_oEncoding As Encoding

    ''' <summary>
    ''' Constructor of the class.
    ''' </summary>
    ''' <param name="oEncoding"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal oEncoding As Encoding)
        m_oEncoding = oEncoding
    End Sub

    ''' <summary>
    ''' </summary>
    ''' <returns>Return a string representing the encoding type</returns>
    ''' <remarks></remarks>
    Public Overrides Function ToString() As String
        If m_oEncoding Is System.Text.Encoding.ASCII Then
            Return My.Resources.TXT_ANSI
        ElseIf m_oEncoding Is System.Text.Encoding.UTF8 Then
            Return My.Resources.TXT_UTF8
        ElseIf m_oEncoding Is System.Text.Encoding.Unicode Then
            Return My.Resources.TXT_UTF16
        Else
            Return m_oEncoding.EncodingName
        End If
    End Function

    ''' <summary>
    ''' Property encoding.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Encoding() As Encoding
        Get
            Return m_oEncoding
        End Get
    End Property

    ''' <summary>
    ''' Overrides method from the super class.
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If obj Is Nothing Then
            Return False
        End If

        If TypeOf obj Is EncodingComboBoxItem Then
            Return Me.Equals(CType(obj, EncodingComboBoxItem))
        End If

        If TypeOf obj Is Encoding Then
            Return Me.Equals(CType(obj, Encoding))
        End If

        Return False
    End Function

    ''' <summary>
    ''' Overrides method from the super class.
    ''' </summary>
    ''' <param name="other"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Equals(ByVal other As EncodingComboBoxItem) As Boolean _
        Implements System.IEquatable(Of EncodingComboBoxItem).Equals

        Return m_oEncoding.Equals(other.Encoding)
    End Function

    ''' <summary>
    ''' Overrides method from the super class.
    ''' </summary>
    ''' <param name="other"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Equals(ByVal other As System.Text.Encoding) As Boolean _
        Implements System.IEquatable(Of System.Text.Encoding).Equals

        Return m_oEncoding.Equals(other)
    End Function
End Class
