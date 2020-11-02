'********************************************************************************
'***   (c)Copyright Kofax Inc. 2009 All rights reserved.
'***   Unauthorized use, duplication or distribution is strictly prohibited.
'********************************************************************************
Imports System.Windows.Forms

''' <summary>
''' This class is extended from the DataGridViewColumn class. Some properties of this class are initialized 
''' when a new instance of the class is created.
''' </summary>
''' <remarks></remarks>
Friend Class DataGridViewSequenceColumn
    Inherits DataGridViewColumn

    ''' <summary>
    ''' Constructor of the class.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New(New DataGridViewSequenceCell())
        Name = "Sequence"
        HeaderText = My.Resources.TXT_SEQUENCE
        AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
    End Sub
End Class
