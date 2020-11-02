'********************************************************************************
'***   (c)Copyright Kofax Inc. 2009 All rights reserved.
'***   Unauthorized use, duplication or distribution is strictly prohibited.
'********************************************************************************
Imports Kofax.ReleaseLib
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports SourceType = Kofax.ReleaseLib.KfxLinkSourceType
Imports Kofax.Connector.Common

''' <summary>
''' This class is extended from the DataGridViewTextBoxCell to get proper format for each row of the grid. 
''' </summary>
''' <remarks></remarks>
Friend Class DataGridViewIndexValueCell
    Inherits DataGridViewTextBoxCell

    ''' <summary>
    ''' Column property.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Column() As DataGridViewIndexValueColumn
        Get
            Return CType(DataGridView.Columns(ColumnIndex), DataGridViewIndexValueColumn)
        End Get
    End Property

    ''' <summary>
    ''' Property edit type.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides ReadOnly Property EditType() As System.Type
        Get
            Return GetType(DataGridViewIndexValueEditingControl)
        End Get
    End Property

    ''' <summary>
    ''' This overrides method is used to get proper format for grid cells.
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
        cellStyle.Font = New Font(cellStyle.Font, FontStyle.Bold)
        Dim strSourceName As String = CStr(GetValue(rowIndex))
        Dim oSourceType As SourceType = CType(DataGridView(2, rowIndex).Value, SourceType)
		Dim oExportValue As New ExportValue(strSourceName, oSourceType, String.Empty)
		Dim strFormattedVal As String = oExportValue.GetDisplayedText()
		'*** If this is a text constant, insert quotes when displayed in the grid view.
		If oExportValue.SourceType = KfxLinkSourceType.KFX_REL_TEXTCONSTANT Then
			strFormattedVal = String.Format("""{0}""", strFormattedVal)
		End If
		Return strFormattedVal
    End Function

    ''' <summary>
    ''' Get link corresponding to a specific row.
    ''' </summary>
    ''' <param name="rowIndex"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function GetValue(ByVal rowIndex As Integer) As Object
        Dim oLinksList As BindingList(Of ExportValue) = CType(DataGridView.DataSource, BindingList(Of ExportValue))
        If 0 <= rowIndex And rowIndex < oLinksList.Count Then
            Return oLinksList(rowIndex).SourceName
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Set and un-format value.
    ''' </summary>
    ''' <param name="rowIndex"></param>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function SetValue(ByVal rowIndex As Integer, ByVal value As Object) As Boolean
        If value IsNot Nothing Then
            Dim oExportValue As New ExportValue(CType(value, String))
            value = oExportValue.SourceName()
        End If

        Return MyBase.SetValue(rowIndex, value)
    End Function

    ''' <summary>
    ''' Get source type.
    ''' </summary>
    ''' <param name="oSourceType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetSourceType(ByVal oSourceType As KfxLinkSourceType) As SourceType
        Return CType(oSourceType, SourceType)
    End Function
   

End Class
