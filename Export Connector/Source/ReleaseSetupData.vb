'****************************************************************************
'*	(c) Copyright Kofax, Inc. 2009. All rights reserved.
'*	Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************

Option Strict Off

Imports System.Runtime.InteropServices

''' <summary>
''' This is an adapter to the Kofax.ReleaseLib.ReleaseSetupData class
''' to provide late binding to the ImageTypes and BatchVariableNames
''' collections because the KfxReleaseLib definition of these properties
''' is just "Object"
''' </summary>
''' <remarks></remarks>
Friend Class ReleaseSetupData
    Private releaseSetupData As Kofax.ReleaseLib.ReleaseSetupData

    ''' <summary>
    ''' The ReleaseLibCollection class implements a collection to provide late binding.
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class ReleaseLibCollection
        Implements IEnumerable, IDisposable

        Private releaseLibCollection As Object

        ''' <summary>
        ''' Create a new instance of this class from COM object collection
        ''' </summary>
        ''' <param name="releaseLibCollection">COM object collection</param>
        ''' <remarks></remarks>
        Friend Sub New(ByVal releaseLibCollection As Object)
            Me.releaseLibCollection = releaseLibCollection
        End Sub

        ''' <summary>
        ''' Gets collection count
        ''' </summary>
        ''' <value></value>
        ''' <returns>Number of items in collection</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Count() As Integer
            Get
                Return releaseLibCollection.Count
            End Get
        End Property

        ''' <summary>
        ''' Get enumerator of this collection
        ''' </summary>
        ''' <returns>Enumerator of this collection</returns>
        ''' <remarks></remarks>
        Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return releaseLibCollection.GetEnumerator()
        End Function

        ''' <summary>
        ''' Dispose COM object
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Dispose() Implements IDisposable.Dispose
            If releaseLibCollection IsNot Nothing Then
                Marshal.FinalReleaseComObject(releaseLibCollection)
                releaseLibCollection = Nothing
            End If
        End Sub
    End Class

    ''' <summary>
    ''' Create a new instance of this class
    ''' </summary>
    ''' <param name="releaseSetupData"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal releaseSetupData As Kofax.ReleaseLib.ReleaseSetupData)
        Me.releaseSetupData = releaseSetupData
    End Sub

    ''' <summary>
    ''' Gets batch variable names
    ''' </summary>
    ''' <value></value>
    ''' <returns>A collection containing batch variable names</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property BatchVariableNames() As ReleaseLibCollection
        Get
            Return New ReleaseLibCollection(releaseSetupData.BatchVariableNames)
        End Get
    End Property

    ''' <summary>
    ''' Gets image types
    ''' </summary>
    ''' <value></value>
    ''' <returns>A collection containing image types</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ImageTypes() As ReleaseLibCollection
        Get
            Return New ReleaseLibCollection(releaseSetupData.ImageTypes)
        End Get
    End Property
End Class
