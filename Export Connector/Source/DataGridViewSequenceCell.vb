'********************************************************************************
'***   (c)Copyright Kofax Inc. 2009 All rights reserved.
'***   Unauthorized use, duplication or distribution is strictly prohibited.
'********************************************************************************
Imports System.Windows.Forms

''' <summary>
''' Definition of the custom DataGridViewCell for index field values.
''' </summary>
''' <remarks></remarks>
Friend Class DataGridViewSequenceCell
    Inherits DataGridViewTextBoxCell

    ''' <summary>
    ''' Constructor of the class.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        Style.Alignment = DataGridViewContentAlignment.MiddleCenter
        Style.BackColor = SystemColors.Control
    End Sub

    ''' <summary>
    ''' This overrides method is used to get format of each row based on its row index.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="rowIndex"></param>
    ''' <param name="cellStyle"></param>
    ''' <param name="valueTypeConverter"></param>
    ''' <param name="formattedValueTypeConverter"></param>
    ''' <param name="context"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function GetFormattedValue(ByVal value As Object, ByVal rowIndex As Integer, ByRef cellStyle As System.Windows.Forms.DataGridViewCellStyle, ByVal valueTypeConverter As System.ComponentModel.TypeConverter, ByVal formattedValueTypeConverter As System.ComponentModel.TypeConverter, ByVal context As System.Windows.Forms.DataGridViewDataErrorContexts) As Object
        Return (rowIndex + 1).ToString
    End Function
End Class
