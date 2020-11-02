'********************************************************************************
'***   (c)Copyright Kofax Inc. 2009 All rights reserved.
'***   Unauthorized use, duplication or distribution is strictly prohibited.
'********************************************************************************
Imports System.Drawing
Imports System.Windows.Forms
Imports SourceType = Kofax.ReleaseLib.KfxLinkSourceType
Imports Kofax.Connector.Common

''' <summary>
''' Implementation of IDataGridViewEditingControl used to edit index value cells
''' </summary>
''' <remarks></remarks>
Friend Class DataGridViewIndexValueEditingControl
    Inherits Panel
    Implements IDataGridViewEditingControl

    Private Const mc_TextConstantMaxSize As Integer = 254

    Private m_oLink As ExportValue
    Private WithEvents m_oButton As New Button()
    Private WithEvents m_oTextBox As New IndexValueTextBox()
    Private m_oDataGridView As IndexValueDataGridView
    Private m_oComponents As System.ComponentModel.IContainer
    Private m_bValueChanged As Boolean
    Private m_nRowIndex As Integer
    Private m_oCell As DataGridViewIndexValueCell
    Private WithEvents m_oExportValuesMenu As New ExportValuesMenu

    ''' <summary>
    ''' Constructor of the class.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        InitializeComponent()

        m_oButton.Dock = DockStyle.Right
        m_oButton.Image = My.Resources.IndexValueButtonImage

        m_oTextBox.Dock = DockStyle.Fill
        m_oTextBox.ReadOnly = True
        m_oTextBox.MaxLength = mc_TextConstantMaxSize

        Controls.Add(m_oTextBox)
        Controls.Add(m_oButton)
    End Sub

    ''' <summary>
    ''' Allow the space bar to open the menu to edit the index value, or handle up and down arrow keys
    ''' </summary>
    ''' <param name="e">The event args</param>
    ''' <remarks>Delegates to the PreviewKeyDown handler for the textbox</remarks>
    Protected Overrides Sub OnPreviewKeyDown(ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs)
        If e.Control AndAlso e.KeyCode = Keys.Up Then
            ' Raise the Item Move up Event of the grid
            m_oDataGridView.RaiseMoveUpEvent(Me, e)
        ElseIf e.Control AndAlso e.KeyCode = Keys.Down Then
            m_oDataGridView.RaiseMoveDownEvent(Me, e)
        ElseIf e.KeyCode = Keys.Enter Then
            Return
        Else
            MyBase.OnPreviewKeyDown(e)
            If e.KeyCode = Keys.Space Or e.KeyData = Keys.Apps Then
                ShowContextMenu()
            End If
        End If

    End Sub

    ''' <summary>
    ''' Overrides method to process cmd key.
    ''' </summary>
    ''' <param name="msg"></param>
    ''' <param name="keyData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        If m_oTextBox.ReadOnly AndAlso keyData = Keys.Enter Then
            m_oDataGridView.RaiseEnterPressEvent(Me, New KeyEventArgs(keyData))
            Return True
        ElseIf m_oTextBox.ReadOnly AndAlso keyData = Keys.Space Then
            OnPreviewKeyDown(New PreviewKeyDownEventArgs(keyData))
            Return True
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    ''' <summary>
    ''' Handles key down event for the text box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TextBox_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles m_oTextBox.KeyDown
        If Not m_oTextBox.ReadOnly And (e.KeyData = Keys.Enter Or e.KeyData = Keys.Return) Then
            m_oDataGridView.PerformValidation()
            m_oDataGridView.BeginEdit(True)
            e.Handled = True
        ElseIf m_oTextBox.ReadOnly And e.KeyData = Keys.Enter Then
            m_oDataGridView.RaiseEnterPressEvent(Me, New KeyEventArgs(e.KeyData))
            e.Handled = True
        End If
    End Sub

    ''' <summary>
    ''' Handles leave event for the text box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TextBox_Leave(ByVal sender As Object, ByVal e As EventArgs) Handles m_oTextBox.Leave
        If Not m_oTextBox.ReadOnly Then
            m_oTextBox.ReadOnly = True
            m_oLink.SourceName = m_oTextBox.Text
            If String.IsNullOrEmpty(m_oLink.SourceName) Then
                m_oLink.SourceType = SourceType.KFX_REL_UNDEFINED_LINK
            End If
            m_oTextBox.BackColor = m_oDataGridView.DefaultCellStyle.SelectionBackColor
            m_oTextBox.ForeColor = m_oDataGridView.DefaultCellStyle.SelectionForeColor
            EditingControlValueChanged = True
        End If
    End Sub


    ''' <summary>
    ''' Show context menu when the button is clicked.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub IndexValueClicked(ByVal sender As Object, ByVal e As EventArgs) Handles m_oButton.Click
        ShowContextMenu()
    End Sub

    ''' <summary>
    ''' Show context menu.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ShowContextMenu()
        ' Init export values menu
        m_oExportValuesMenu.Init(m_oDataGridView.SetupData)
        m_oExportValuesMenu.ShowDeleteMenuItem = True
        m_oExportValuesMenu.ShowDocumentIndexFieldsMenuItem = True
        m_oExportValuesMenu.ShowFolderIndexFieldsMenuItem = True
        m_oExportValuesMenu.ShowBatchFieldsMenuItem = True
        m_oExportValuesMenu.ShowKofaxCaptureValuesMenuItem = True
        m_oExportValuesMenu.ShowExportLocationMenuItem = True
        m_oExportValuesMenu.ShowTextConstantMenuItem = True

        m_oExportValuesMenu.Show(Me, 0, Height)
    End Sub

    ''' <summary>
    ''' Apply cell style to the editing control.
    ''' </summary>
    ''' <param name="dataGridViewCellStyle"></param>
    ''' <remarks></remarks>
    Public Sub ApplyCellStyleToEditingControl(ByVal dataGridViewCellStyle As System.Windows.Forms.DataGridViewCellStyle) Implements System.Windows.Forms.IDataGridViewEditingControl.ApplyCellStyleToEditingControl
        m_oTextBox.Font = dataGridViewCellStyle.Font
    End Sub

    ''' <summary>
    ''' Property editing control data grid view.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EditingControlDataGridView() As System.Windows.Forms.DataGridView Implements System.Windows.Forms.IDataGridViewEditingControl.EditingControlDataGridView
        Get
            Return m_oDataGridView
        End Get
        Set(ByVal value As System.Windows.Forms.DataGridView)
            m_oDataGridView = CType(value, IndexValueDataGridView)
            m_oButton.Size = New Size(m_oDataGridView(0, 0).Size.Height, m_oDataGridView(0, 0).Size.Height)
            m_oTextBox.BackColor = m_oDataGridView.DefaultCellStyle.SelectionBackColor
            m_oTextBox.ForeColor = m_oDataGridView.DefaultCellStyle.SelectionForeColor
        End Set
    End Property

    ''' <summary>
    ''' Property editing control formatted value.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EditingControlFormattedValue() As Object Implements System.Windows.Forms.IDataGridViewEditingControl.EditingControlFormattedValue
        Get
            Return m_oTextBox.Text
        End Get
        Set(ByVal value As Object)
            m_oTextBox.Text = CType(value, String)
        End Set
    End Property

    ''' <summary>
    ''' Property editing control row index.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EditingControlRowIndex() As Integer Implements System.Windows.Forms.IDataGridViewEditingControl.EditingControlRowIndex
        Get
            Return m_nRowIndex
        End Get
        Set(ByVal value As Integer)
            m_nRowIndex = value
            m_oLink = CType(m_oDataGridView.Rows(m_nRowIndex).DataBoundItem, ExportValue)
            m_oCell = CType(m_oDataGridView.Item(1, m_nRowIndex), DataGridViewIndexValueCell)
            m_oTextBox.Text = CStr(m_oCell.FormattedValue)
        End Set
    End Property

    ''' <summary>
    ''' Property editing control value changed.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EditingControlValueChanged() As Boolean Implements System.Windows.Forms.IDataGridViewEditingControl.EditingControlValueChanged
        Get
            Return m_bValueChanged
        End Get
        Set(ByVal value As Boolean)
            m_bValueChanged = value
            m_oDataGridView.NotifyCurrentCellDirty(m_bValueChanged)
        End Set
    End Property

    ''' <summary>
    ''' Check if the editing control wants input key or not.
    ''' </summary>
    ''' <param name="keyData"></param>
    ''' <param name="dataGridViewWantsInputKey"></param>
    ''' <returns>True if the editing control wants input key.</returns>
    ''' <remarks></remarks>
    Public Function EditingControlWantsInputKey(ByVal keyData As System.Windows.Forms.Keys, ByVal dataGridViewWantsInputKey As Boolean) As Boolean Implements System.Windows.Forms.IDataGridViewEditingControl.EditingControlWantsInputKey
        If keyData.ToString().Equals("Down, Control") _
                        Or keyData.ToString().Equals("Up, Control") Then
            Return True
        End If
        If m_oTextBox.ReadOnly Then
            ' If not in edit mode, treat left and right
            ' arrow keys like up and down arrow keys
            Select Case keyData
                Case Keys.Left
                    SendKeys.Send("{UP}")
                    Return True

                Case Keys.Right
                    SendKeys.Send("{DOWN}")
                    Return True
            End Select
        Else
            ' If in edit mode, allow up and down
            ' keys to leave the row,
            ' otherwise let the TextBox control
            ' handle them
            Select Case keyData
                Case Keys.Up, Keys.Down
                    Return False

                Case Else
                    Return True
            End Select
        End If

        Return False
    End Function

    ''' <summary>
    ''' Property editing panel cursor.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property EditingPanelCursor() As System.Windows.Forms.Cursor Implements System.Windows.Forms.IDataGridViewEditingControl.EditingPanelCursor
        Get
            Return Cursor
        End Get
    End Property

    ''' <summary>
    ''' Get editing control formatted value.
    ''' </summary>
    ''' <param name="context"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetEditingControlFormattedValue(ByVal context As System.Windows.Forms.DataGridViewDataErrorContexts) As Object Implements System.Windows.Forms.IDataGridViewEditingControl.GetEditingControlFormattedValue
        Return EditingControlFormattedValue
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="selectAll"></param>
    ''' <remarks></remarks>
    Public Sub PrepareEditingControlForEdit(ByVal selectAll As Boolean) Implements System.Windows.Forms.IDataGridViewEditingControl.PrepareEditingControlForEdit
    End Sub

    ''' <summary>
    ''' Property reposition editing control on value change.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property RepositionEditingControlOnValueChange() As Boolean Implements System.Windows.Forms.IDataGridViewEditingControl.RepositionEditingControlOnValueChange
        Get
            Return False
        End Get
    End Property

    ''' <summary>
    ''' Initialize component.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeComponent()
        Me.SuspendLayout()
        Me.ResumeLayout(False)

    End Sub

    ''' <summary>
    ''' Perform delete button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub deleteMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_oExportValuesMenu.DeleteMenuItemClicked
        m_oDataGridView.DeleteButton.PerformClick()
    End Sub

    ''' <summary>
    ''' Handles click event for the text constant menu items.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub textConstantMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_oExportValuesMenu.TextConstantMenuItemClicked
        If SourceType.KFX_REL_TEXTCONSTANT <> m_oLink.SourceType Then
            m_oLink.SourceName = String.Empty
            m_oLink.SourceType = SourceType.KFX_REL_TEXTCONSTANT
            m_oTextBox.Text = m_oLink.SourceName
            EditingControlValueChanged = True
        End If
        m_oTextBox.Text = m_oLink.SourceName
        m_oTextBox.ReadOnly = False
        m_oTextBox.BackColor = m_oCell.Style.BackColor
        m_oTextBox.ForeColor = m_oCell.Style.ForeColor
        m_oTextBox.Focus()
    End Sub

    ''' <summary>
    ''' Handles click event for the kofax capture values menu item.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KofaxCaptureValuesMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles m_oExportValuesMenu.KofaxCaptureValuesMenuItemClicked
        Dim oItem As ToolStripItem = CType(sender, ToolStripItem)
        m_oLink.SourceName = oItem.Text
        m_oLink.SourceType = SourceType.KFX_REL_VARIABLE
        m_oTextBox.Text = CStr(m_oCell.FormattedValue)
        EditingControlValueChanged = True
    End Sub

    ''' <summary>
    ''' Handles click event for the batch fields menu item.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Private Sub BatchFieldsMenuItem_Click(ByVal sender As Object, ByVal eventArgs As EventArgs) Handles m_oExportValuesMenu.BatchFieldsMenuItemClicked
        Dim oItem As ToolStripItem = CType(sender, ToolStripItem)
        m_oLink.SourceName = oItem.Text
        m_oLink.SourceType = SourceType.KFX_REL_BATCHFIELD
        m_oTextBox.Text = CStr(m_oCell.FormattedValue)
        EditingControlValueChanged = True
    End Sub

    ''' <summary>
    ''' Handles click event for the document index fields menu item.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Private Sub DocumentIndexFieldsMenuItem_Click(ByVal sender As Object, ByVal eventArgs As EventArgs) Handles m_oExportValuesMenu.DocumentIndexFieldsMenuItemClicked
        Dim oItem As ToolStripItem = CType(sender, ToolStripItem)
        m_oLink.SourceName = oItem.Text
        m_oLink.SourceType = SourceType.KFX_REL_INDEXFIELD
        m_oTextBox.Text = CStr(m_oCell.FormattedValue)
        EditingControlValueChanged = True
    End Sub

    ''' <summary>
    ''' Handles click event for the release location menu item.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Private Sub ReleaseLocationMenuItem_Click(ByVal sender As Object, ByVal eventArgs As EventArgs) Handles m_oExportValuesMenu.ExportLocationMenuItemClicked
        Dim oItem As ToolStripItem = CType(sender, ToolStripItem)
        m_oLink.SourceName = CStr(oItem.Tag)
        m_oLink.SourceType = SourceType.KFX_REL_VARIABLE
        m_oTextBox.Text = CStr(m_oCell.FormattedValue)
        EditingControlValueChanged = True
    End Sub

    ''' <summary>
    ''' Handles click event for the folder index fields menu items.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Private Sub FolderIndexFieldsMenuItem_Click(ByVal sender As Object, ByVal eventArgs As EventArgs) Handles m_oExportValuesMenu.FolderIndexFieldsMenuItemClicked
        Dim oItem As ToolStripItem = CType(sender, ToolStripItem)
        m_oLink.SourceName = CStr(oItem.Tag)
        m_oLink.SourceType = SourceType.KFX_REL_INDEXFIELD
        m_oTextBox.Text = CStr(m_oCell.FormattedValue)
        EditingControlValueChanged = True
    End Sub
End Class
