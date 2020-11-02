'********************************************************************************
'***   (c)Copyright Kofax Inc. 2009 All rights reserved.
'***   Unauthorized use, duplication or distribution is strictly prohibited.
'********************************************************************************
Imports System.Windows.Forms

''' <summary>
''' Definition of the custom DataGridViewColumn for index value types
''' </summary>
''' <remarks></remarks>
Friend Class DataGridViewIndexValueColumn
    Inherits DataGridViewColumn

    ''' <summary>
    ''' Constructor of the class.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New(New DataGridViewIndexValueCell())
        Name = "Source"
        DataPropertyName = Name
        HeaderText = My.Resources.TXT_VALUE
        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
    End Sub

End Class
