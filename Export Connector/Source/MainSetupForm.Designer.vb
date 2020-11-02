<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainSetupForm
    Inherits System.Windows.Forms.Form



    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
		Dim lblEncoding As System.Windows.Forms.Label
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainSetupForm))
		Dim pnlAdvance As System.Windows.Forms.Panel
		Dim pnlDefaultStorage As System.Windows.Forms.Panel
		Dim lblName As System.Windows.Forms.Label
		Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Me.tlpExportImageAs = New System.Windows.Forms.TableLayoutPanel()
		Me.cboImageType = New System.Windows.Forms.ComboBox()
		Me.lblReleaseImageAs = New System.Windows.Forms.Label()
		Me.pnlOCROutput = New System.Windows.Forms.Panel()
		Me.optOcrCustomPath = New System.Windows.Forms.RadioButton()
		Me.optOcrDefaultPath = New System.Windows.Forms.RadioButton()
		Me.cmdOcrCustom = New System.Windows.Forms.Button()
		Me.txtOcrCustomPath = New System.Windows.Forms.TextBox()
		Me.chkOcrOutput = New System.Windows.Forms.CheckBox()
		Me.pnlImageOutput = New System.Windows.Forms.Panel()
		Me.cmdImgCustom = New System.Windows.Forms.Button()
		Me.txtImgCustomPath = New System.Windows.Forms.TextBox()
		Me.chkImgOutput = New System.Windows.Forms.CheckBox()
		Me.optImgCustomPath = New System.Windows.Forms.RadioButton()
		Me.optImgDefaultPath = New System.Windows.Forms.RadioButton()
		Me.pnlPDFOutput = New System.Windows.Forms.Panel()
		Me.txtPdfCustomPath = New System.Windows.Forms.TextBox()
		Me.cmdPdfCustom = New System.Windows.Forms.Button()
		Me.optPdfDefaultPath = New System.Windows.Forms.RadioButton()
		Me.optPdfCustomPath = New System.Windows.Forms.RadioButton()
		Me.chkPdfOutput = New System.Windows.Forms.CheckBox()
		Me.chkSkipFirstPage = New System.Windows.Forms.CheckBox()
		Me.chkSuppressIfPdfDetected = New System.Windows.Forms.CheckBox()
		Me.ctlCustomFileNameInput = New Kofax.Connector.Common.CustomFileNameInput()
		Me.lblFileName = New System.Windows.Forms.Label()
		Me.lblDefautStorageFolder = New System.Windows.Forms.Label()
		Me.cmdCustom = New System.Windows.Forms.Button()
		Me.txtDefaultStorageFolder = New System.Windows.Forms.TextBox()
		Me.fraFileNaming = New System.Windows.Forms.GroupBox()
		Me.optCustomFileName = New System.Windows.Forms.RadioButton()
		Me.chkLeadZeros = New System.Windows.Forms.CheckBox()
		Me.optDecimalFilename = New System.Windows.Forms.RadioButton()
		Me.optStandardNaming = New System.Windows.Forms.RadioButton()
		Me.fraPlusHandling = New System.Windows.Forms.GroupBox()
		Me.optRenameIfDup = New System.Windows.Forms.RadioButton()
		Me.optReplaceIfDup = New System.Windows.Forms.RadioButton()
		Me.optErrorIfDup = New System.Windows.Forms.RadioButton()
		Me.cmdDeleteAllIndex = New System.Windows.Forms.Button()
		Me.fraIndexVals = New System.Windows.Forms.GroupBox()
		Me.ctlIndexValueUpDown = New Kofax.Connector.Text.UpDownControl()
		Me.grdIndexValuesData = New Kofax.Connector.Text.IndexValueDataGridView()
		Me.cmdAddIndex = New System.Windows.Forms.Button()
		Me.cmdDeleteIndex = New System.Windows.Forms.Button()
		Me.lblMove = New System.Windows.Forms.Label()
		Me.cboEncoding = New System.Windows.Forms.ComboBox()
		Me.pnlIndexStorage = New System.Windows.Forms.Panel()
		Me.tlpEncoding = New System.Windows.Forms.TableLayoutPanel()
		Me.tlpIndexOutput = New System.Windows.Forms.TableLayoutPanel()
		Me.cmdCustomFileOutputBrowse = New System.Windows.Forms.Button()
		Me.txtCustomIndexFileName = New System.Windows.Forms.TextBox()
		Me.txtCustomIndexFolder = New System.Windows.Forms.TextBox()
		Me.lblStorageFileName = New System.Windows.Forms.Label()
		Me.lblStorageFolder = New System.Windows.Forms.Label()
		Me.chkCustomFileOutput = New System.Windows.Forms.CheckBox()
		Me.chkDefaultStorageOutput = New System.Windows.Forms.CheckBox()
		Me.tpgIndexStorage = New System.Windows.Forms.TabPage()
		Me.txtName = New System.Windows.Forms.TextBox()
		Me.cmdHelp = New System.Windows.Forms.Button()
		Me.tabText = New System.Windows.Forms.TabControl()
		Me.tpgDefaultStorage = New System.Windows.Forms.TabPage()
		Me.tpgAdvance = New System.Windows.Forms.TabPage()
		Me.cmdApply = New System.Windows.Forms.Button()
		Me.cmdCancel = New System.Windows.Forms.Button()
		Me.cmdOK = New System.Windows.Forms.Button()
		Me.tlpExportName = New System.Windows.Forms.TableLayoutPanel()
		lblEncoding = New System.Windows.Forms.Label()
		pnlAdvance = New System.Windows.Forms.Panel()
		pnlDefaultStorage = New System.Windows.Forms.Panel()
		lblName = New System.Windows.Forms.Label()
		pnlAdvance.SuspendLayout()
		Me.tlpExportImageAs.SuspendLayout()
		Me.pnlOCROutput.SuspendLayout()
		Me.pnlImageOutput.SuspendLayout()
		Me.pnlPDFOutput.SuspendLayout()
		pnlDefaultStorage.SuspendLayout()
		Me.fraFileNaming.SuspendLayout()
		Me.fraPlusHandling.SuspendLayout()
		Me.fraIndexVals.SuspendLayout()
		CType(Me.grdIndexValuesData, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.pnlIndexStorage.SuspendLayout()
		Me.tlpEncoding.SuspendLayout()
		Me.tlpIndexOutput.SuspendLayout()
		Me.tpgIndexStorage.SuspendLayout()
		Me.tabText.SuspendLayout()
		Me.tpgDefaultStorage.SuspendLayout()
		Me.tpgAdvance.SuspendLayout()
		Me.tlpExportName.SuspendLayout()
		Me.SuspendLayout()
		'
		'lblEncoding
		'
		resources.ApplyResources(lblEncoding, "lblEncoding")
		lblEncoding.Cursor = System.Windows.Forms.Cursors.Default
		lblEncoding.ForeColor = System.Drawing.SystemColors.ControlText
		lblEncoding.Name = "lblEncoding"
		'
		'pnlAdvance
		'
		pnlAdvance.Controls.Add(Me.tlpExportImageAs)
		pnlAdvance.Controls.Add(Me.pnlOCROutput)
		pnlAdvance.Controls.Add(Me.pnlImageOutput)
		pnlAdvance.Controls.Add(Me.pnlPDFOutput)
		pnlAdvance.Controls.Add(Me.chkSkipFirstPage)
		pnlAdvance.Controls.Add(Me.chkSuppressIfPdfDetected)
		pnlAdvance.Cursor = System.Windows.Forms.Cursors.Default
		resources.ApplyResources(pnlAdvance, "pnlAdvance")
		pnlAdvance.Name = "pnlAdvance"
		'
		'tlpExportImageAs
		'
		resources.ApplyResources(Me.tlpExportImageAs, "tlpExportImageAs")
		Me.tlpExportImageAs.Controls.Add(Me.cboImageType, 1, 0)
		Me.tlpExportImageAs.Controls.Add(Me.lblReleaseImageAs, 0, 0)
		Me.tlpExportImageAs.Name = "tlpExportImageAs"
		'
		'cboImageType
		'
		resources.ApplyResources(Me.cboImageType, "cboImageType")
		Me.cboImageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cboImageType.FormattingEnabled = True
		Me.cboImageType.Name = "cboImageType"
		'
		'lblReleaseImageAs
		'
		resources.ApplyResources(Me.lblReleaseImageAs, "lblReleaseImageAs")
		Me.lblReleaseImageAs.Name = "lblReleaseImageAs"
		'
		'pnlOCROutput
		'
		Me.pnlOCROutput.Controls.Add(Me.optOcrCustomPath)
		Me.pnlOCROutput.Controls.Add(Me.optOcrDefaultPath)
		Me.pnlOCROutput.Controls.Add(Me.cmdOcrCustom)
		Me.pnlOCROutput.Controls.Add(Me.txtOcrCustomPath)
		Me.pnlOCROutput.Controls.Add(Me.chkOcrOutput)
		resources.ApplyResources(Me.pnlOCROutput, "pnlOCROutput")
		Me.pnlOCROutput.Name = "pnlOCROutput"
		'
		'optOcrCustomPath
		'
		resources.ApplyResources(Me.optOcrCustomPath, "optOcrCustomPath")
		Me.optOcrCustomPath.Name = "optOcrCustomPath"
		Me.optOcrCustomPath.UseVisualStyleBackColor = True
		'
		'optOcrDefaultPath
		'
		Me.optOcrDefaultPath.Checked = True
		resources.ApplyResources(Me.optOcrDefaultPath, "optOcrDefaultPath")
		Me.optOcrDefaultPath.Name = "optOcrDefaultPath"
		Me.optOcrDefaultPath.TabStop = True
		Me.optOcrDefaultPath.UseVisualStyleBackColor = True
		'
		'cmdOcrCustom
		'
		resources.ApplyResources(Me.cmdOcrCustom, "cmdOcrCustom")
		Me.cmdOcrCustom.Name = "cmdOcrCustom"
		Me.cmdOcrCustom.UseVisualStyleBackColor = True
		'
		'txtOcrCustomPath
		'
		resources.ApplyResources(Me.txtOcrCustomPath, "txtOcrCustomPath")
		Me.txtOcrCustomPath.Name = "txtOcrCustomPath"
		'
		'chkOcrOutput
		'
		Me.chkOcrOutput.Checked = True
		Me.chkOcrOutput.CheckState = System.Windows.Forms.CheckState.Checked
		resources.ApplyResources(Me.chkOcrOutput, "chkOcrOutput")
		Me.chkOcrOutput.Name = "chkOcrOutput"
		Me.chkOcrOutput.UseVisualStyleBackColor = True
		'
		'pnlImageOutput
		'
		Me.pnlImageOutput.Controls.Add(Me.cmdImgCustom)
		Me.pnlImageOutput.Controls.Add(Me.txtImgCustomPath)
		Me.pnlImageOutput.Controls.Add(Me.chkImgOutput)
		Me.pnlImageOutput.Controls.Add(Me.optImgCustomPath)
		Me.pnlImageOutput.Controls.Add(Me.optImgDefaultPath)
		resources.ApplyResources(Me.pnlImageOutput, "pnlImageOutput")
		Me.pnlImageOutput.Name = "pnlImageOutput"
		'
		'cmdImgCustom
		'
		resources.ApplyResources(Me.cmdImgCustom, "cmdImgCustom")
		Me.cmdImgCustom.Name = "cmdImgCustom"
		Me.cmdImgCustom.UseVisualStyleBackColor = True
		'
		'txtImgCustomPath
		'
		resources.ApplyResources(Me.txtImgCustomPath, "txtImgCustomPath")
		Me.txtImgCustomPath.Name = "txtImgCustomPath"
		'
		'chkImgOutput
		'
		Me.chkImgOutput.Checked = True
		Me.chkImgOutput.CheckState = System.Windows.Forms.CheckState.Checked
		resources.ApplyResources(Me.chkImgOutput, "chkImgOutput")
		Me.chkImgOutput.Name = "chkImgOutput"
		Me.chkImgOutput.UseVisualStyleBackColor = True
		'
		'optImgCustomPath
		'
		resources.ApplyResources(Me.optImgCustomPath, "optImgCustomPath")
		Me.optImgCustomPath.Name = "optImgCustomPath"
		Me.optImgCustomPath.UseVisualStyleBackColor = True
		'
		'optImgDefaultPath
		'
		Me.optImgDefaultPath.Checked = True
		resources.ApplyResources(Me.optImgDefaultPath, "optImgDefaultPath")
		Me.optImgDefaultPath.Name = "optImgDefaultPath"
		Me.optImgDefaultPath.TabStop = True
		Me.optImgDefaultPath.UseVisualStyleBackColor = True
		'
		'pnlPDFOutput
		'
		Me.pnlPDFOutput.Controls.Add(Me.txtPdfCustomPath)
		Me.pnlPDFOutput.Controls.Add(Me.cmdPdfCustom)
		Me.pnlPDFOutput.Controls.Add(Me.optPdfDefaultPath)
		Me.pnlPDFOutput.Controls.Add(Me.optPdfCustomPath)
		Me.pnlPDFOutput.Controls.Add(Me.chkPdfOutput)
		resources.ApplyResources(Me.pnlPDFOutput, "pnlPDFOutput")
		Me.pnlPDFOutput.Name = "pnlPDFOutput"
		'
		'txtPdfCustomPath
		'
		resources.ApplyResources(Me.txtPdfCustomPath, "txtPdfCustomPath")
		Me.txtPdfCustomPath.Name = "txtPdfCustomPath"
		'
		'cmdPdfCustom
		'
		resources.ApplyResources(Me.cmdPdfCustom, "cmdPdfCustom")
		Me.cmdPdfCustom.Name = "cmdPdfCustom"
		Me.cmdPdfCustom.UseVisualStyleBackColor = True
		'
		'optPdfDefaultPath
		'
		Me.optPdfDefaultPath.Checked = True
		resources.ApplyResources(Me.optPdfDefaultPath, "optPdfDefaultPath")
		Me.optPdfDefaultPath.Name = "optPdfDefaultPath"
		Me.optPdfDefaultPath.TabStop = True
		Me.optPdfDefaultPath.UseVisualStyleBackColor = True
		'
		'optPdfCustomPath
		'
		resources.ApplyResources(Me.optPdfCustomPath, "optPdfCustomPath")
		Me.optPdfCustomPath.Name = "optPdfCustomPath"
		Me.optPdfCustomPath.UseVisualStyleBackColor = True
		'
		'chkPdfOutput
		'
		Me.chkPdfOutput.Checked = True
		Me.chkPdfOutput.CheckState = System.Windows.Forms.CheckState.Checked
		resources.ApplyResources(Me.chkPdfOutput, "chkPdfOutput")
		Me.chkPdfOutput.Name = "chkPdfOutput"
		Me.chkPdfOutput.UseVisualStyleBackColor = True
		'
		'chkSkipFirstPage
		'
		resources.ApplyResources(Me.chkSkipFirstPage, "chkSkipFirstPage")
		Me.chkSkipFirstPage.Name = "chkSkipFirstPage"
		Me.chkSkipFirstPage.UseVisualStyleBackColor = True
		'
		'chkSuppressIfPdfDetected
		'
		resources.ApplyResources(Me.chkSuppressIfPdfDetected, "chkSuppressIfPdfDetected")
		Me.chkSuppressIfPdfDetected.Checked = True
		Me.chkSuppressIfPdfDetected.CheckState = System.Windows.Forms.CheckState.Checked
		Me.chkSuppressIfPdfDetected.Name = "chkSuppressIfPdfDetected"
		Me.chkSuppressIfPdfDetected.UseVisualStyleBackColor = True
		'
		'pnlDefaultStorage
		'
		pnlDefaultStorage.Controls.Add(Me.ctlCustomFileNameInput)
		pnlDefaultStorage.Controls.Add(Me.lblFileName)
		pnlDefaultStorage.Controls.Add(Me.lblDefautStorageFolder)
		pnlDefaultStorage.Controls.Add(Me.cmdCustom)
		pnlDefaultStorage.Controls.Add(Me.txtDefaultStorageFolder)
		pnlDefaultStorage.Controls.Add(Me.fraFileNaming)
		pnlDefaultStorage.Controls.Add(Me.fraPlusHandling)
		pnlDefaultStorage.Cursor = System.Windows.Forms.Cursors.Default
		resources.ApplyResources(pnlDefaultStorage, "pnlDefaultStorage")
		pnlDefaultStorage.Name = "pnlDefaultStorage"
		'
		'ctlCustomFileNameInput
		'
		resources.ApplyResources(Me.ctlCustomFileNameInput, "ctlCustomFileNameInput")
		Me.ctlCustomFileNameInput.Name = "ctlCustomFileNameInput"
		'
		'lblFileName
		'
		resources.ApplyResources(Me.lblFileName, "lblFileName")
		Me.lblFileName.Name = "lblFileName"
		'
		'lblDefautStorageFolder
		'
		resources.ApplyResources(Me.lblDefautStorageFolder, "lblDefautStorageFolder")
		Me.lblDefautStorageFolder.Name = "lblDefautStorageFolder"
		'
		'cmdCustom
		'
		resources.ApplyResources(Me.cmdCustom, "cmdCustom")
		Me.cmdCustom.Name = "cmdCustom"
		Me.cmdCustom.UseVisualStyleBackColor = True
		'
		'txtDefaultStorageFolder
		'
		resources.ApplyResources(Me.txtDefaultStorageFolder, "txtDefaultStorageFolder")
		Me.txtDefaultStorageFolder.Name = "txtDefaultStorageFolder"
		'
		'fraFileNaming
		'
		Me.fraFileNaming.Controls.Add(Me.optCustomFileName)
		Me.fraFileNaming.Controls.Add(Me.chkLeadZeros)
		Me.fraFileNaming.Controls.Add(Me.optDecimalFilename)
		Me.fraFileNaming.Controls.Add(Me.optStandardNaming)
		resources.ApplyResources(Me.fraFileNaming, "fraFileNaming")
		Me.fraFileNaming.Name = "fraFileNaming"
		Me.fraFileNaming.TabStop = False
		'
		'optCustomFileName
		'
		resources.ApplyResources(Me.optCustomFileName, "optCustomFileName")
		Me.optCustomFileName.Cursor = System.Windows.Forms.Cursors.Default
		Me.optCustomFileName.Name = "optCustomFileName"
		Me.optCustomFileName.UseVisualStyleBackColor = True
		'
		'chkLeadZeros
		'
		Me.chkLeadZeros.Cursor = System.Windows.Forms.Cursors.Default
		resources.ApplyResources(Me.chkLeadZeros, "chkLeadZeros")
		Me.chkLeadZeros.Name = "chkLeadZeros"
		Me.chkLeadZeros.UseVisualStyleBackColor = True
		'
		'optDecimalFilename
		'
		Me.optDecimalFilename.Cursor = System.Windows.Forms.Cursors.Default
		resources.ApplyResources(Me.optDecimalFilename, "optDecimalFilename")
		Me.optDecimalFilename.Name = "optDecimalFilename"
		Me.optDecimalFilename.UseVisualStyleBackColor = True
		'
		'optStandardNaming
		'
		Me.optStandardNaming.Checked = True
		Me.optStandardNaming.Cursor = System.Windows.Forms.Cursors.Default
		resources.ApplyResources(Me.optStandardNaming, "optStandardNaming")
		Me.optStandardNaming.Name = "optStandardNaming"
		Me.optStandardNaming.TabStop = True
		Me.optStandardNaming.UseVisualStyleBackColor = True
		'
		'fraPlusHandling
		'
		Me.fraPlusHandling.Controls.Add(Me.optRenameIfDup)
		Me.fraPlusHandling.Controls.Add(Me.optReplaceIfDup)
		Me.fraPlusHandling.Controls.Add(Me.optErrorIfDup)
		resources.ApplyResources(Me.fraPlusHandling, "fraPlusHandling")
		Me.fraPlusHandling.Name = "fraPlusHandling"
		Me.fraPlusHandling.TabStop = False
		'
		'optRenameIfDup
		'
		Me.optRenameIfDup.Cursor = System.Windows.Forms.Cursors.Default
		resources.ApplyResources(Me.optRenameIfDup, "optRenameIfDup")
		Me.optRenameIfDup.Name = "optRenameIfDup"
		Me.optRenameIfDup.UseVisualStyleBackColor = True
		'
		'optReplaceIfDup
		'
		Me.optReplaceIfDup.Checked = True
		Me.optReplaceIfDup.Cursor = System.Windows.Forms.Cursors.Default
		resources.ApplyResources(Me.optReplaceIfDup, "optReplaceIfDup")
		Me.optReplaceIfDup.Name = "optReplaceIfDup"
		Me.optReplaceIfDup.TabStop = True
		Me.optReplaceIfDup.UseVisualStyleBackColor = True
		'
		'optErrorIfDup
		'
		Me.optErrorIfDup.Cursor = System.Windows.Forms.Cursors.Default
		resources.ApplyResources(Me.optErrorIfDup, "optErrorIfDup")
		Me.optErrorIfDup.Name = "optErrorIfDup"
		Me.optErrorIfDup.UseVisualStyleBackColor = True
		'
		'lblName
		'
		resources.ApplyResources(lblName, "lblName")
		lblName.Cursor = System.Windows.Forms.Cursors.Default
		lblName.ForeColor = System.Drawing.SystemColors.ControlText
		lblName.Name = "lblName"
		'
		'cmdDeleteAllIndex
		'
		Me.cmdDeleteAllIndex.Cursor = System.Windows.Forms.Cursors.Default
		resources.ApplyResources(Me.cmdDeleteAllIndex, "cmdDeleteAllIndex")
		Me.cmdDeleteAllIndex.ForeColor = System.Drawing.SystemColors.ControlText
		Me.cmdDeleteAllIndex.Name = "cmdDeleteAllIndex"
		Me.cmdDeleteAllIndex.UseVisualStyleBackColor = True
		'
		'fraIndexVals
		'
		Me.fraIndexVals.CausesValidation = False
		Me.fraIndexVals.Controls.Add(Me.ctlIndexValueUpDown)
		Me.fraIndexVals.Controls.Add(Me.grdIndexValuesData)
		Me.fraIndexVals.Controls.Add(Me.cmdDeleteAllIndex)
		Me.fraIndexVals.Controls.Add(Me.cmdAddIndex)
		Me.fraIndexVals.Controls.Add(Me.cmdDeleteIndex)
		Me.fraIndexVals.Controls.Add(Me.lblMove)
		Me.fraIndexVals.ForeColor = System.Drawing.SystemColors.ControlText
		resources.ApplyResources(Me.fraIndexVals, "fraIndexVals")
		Me.fraIndexVals.Name = "fraIndexVals"
		Me.fraIndexVals.TabStop = False
		'
		'ctlIndexValueUpDown
		'
		resources.ApplyResources(Me.ctlIndexValueUpDown, "ctlIndexValueUpDown")
		Me.ctlIndexValueUpDown.Name = "ctlIndexValueUpDown"
		Me.ctlIndexValueUpDown.TabStop = False
		'
		'grdIndexValuesData
		'
		Me.grdIndexValuesData.AllowUserToAddRows = False
		Me.grdIndexValuesData.AllowUserToDeleteRows = False
		Me.grdIndexValuesData.AllowUserToResizeColumns = False
		Me.grdIndexValuesData.AllowUserToResizeRows = False
		Me.grdIndexValuesData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
		Me.grdIndexValuesData.BackgroundColor = System.Drawing.SystemColors.Control
		Me.grdIndexValuesData.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.grdIndexValuesData.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken
		DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
		DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
		DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold)
		DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
		DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
		DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
		DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
		Me.grdIndexValuesData.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
		Me.grdIndexValuesData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
		DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
		DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
		DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
		DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
		DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
		Me.grdIndexValuesData.DefaultCellStyle = DataGridViewCellStyle2
		Me.grdIndexValuesData.DeleteButton = Nothing
		Me.grdIndexValuesData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
		resources.ApplyResources(Me.grdIndexValuesData, "grdIndexValuesData")
		Me.grdIndexValuesData.MultiSelect = False
		Me.grdIndexValuesData.Name = "grdIndexValuesData"
		Me.grdIndexValuesData.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken
		DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
		DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control
		DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText
		DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
		DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
		DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
		Me.grdIndexValuesData.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
		Me.grdIndexValuesData.RowHeadersVisible = False
		Me.grdIndexValuesData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
		Me.grdIndexValuesData.SetupData = Nothing
		Me.grdIndexValuesData.StandardTab = True
		'
		'cmdAddIndex
		'
		Me.cmdAddIndex.Cursor = System.Windows.Forms.Cursors.Default
		resources.ApplyResources(Me.cmdAddIndex, "cmdAddIndex")
		Me.cmdAddIndex.ForeColor = System.Drawing.SystemColors.ControlText
		Me.cmdAddIndex.Name = "cmdAddIndex"
		Me.cmdAddIndex.UseVisualStyleBackColor = True
		'
		'cmdDeleteIndex
		'
		Me.cmdDeleteIndex.Cursor = System.Windows.Forms.Cursors.Default
		resources.ApplyResources(Me.cmdDeleteIndex, "cmdDeleteIndex")
		Me.cmdDeleteIndex.ForeColor = System.Drawing.SystemColors.ControlText
		Me.cmdDeleteIndex.Name = "cmdDeleteIndex"
		Me.cmdDeleteIndex.UseVisualStyleBackColor = True
		'
		'lblMove
		'
		resources.ApplyResources(Me.lblMove, "lblMove")
		Me.lblMove.Cursor = System.Windows.Forms.Cursors.Default
		Me.lblMove.ForeColor = System.Drawing.SystemColors.ControlText
		Me.lblMove.Name = "lblMove"
		'
		'cboEncoding
		'
		resources.ApplyResources(Me.cboEncoding, "cboEncoding")
		Me.cboEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cboEncoding.FormattingEnabled = True
		Me.cboEncoding.Name = "cboEncoding"
		'
		'pnlIndexStorage
		'
		Me.pnlIndexStorage.Controls.Add(Me.tlpEncoding)
		Me.pnlIndexStorage.Controls.Add(Me.tlpIndexOutput)
		Me.pnlIndexStorage.Controls.Add(Me.chkCustomFileOutput)
		Me.pnlIndexStorage.Controls.Add(Me.chkDefaultStorageOutput)
		Me.pnlIndexStorage.Controls.Add(Me.fraIndexVals)
		Me.pnlIndexStorage.Cursor = System.Windows.Forms.Cursors.Default
		Me.pnlIndexStorage.ForeColor = System.Drawing.SystemColors.ControlText
		resources.ApplyResources(Me.pnlIndexStorage, "pnlIndexStorage")
		Me.pnlIndexStorage.Name = "pnlIndexStorage"
		'
		'tlpEncoding
		'
		resources.ApplyResources(Me.tlpEncoding, "tlpEncoding")
		Me.tlpEncoding.Controls.Add(Me.cboEncoding, 1, 0)
		Me.tlpEncoding.Controls.Add(lblEncoding, 0, 0)
		Me.tlpEncoding.Name = "tlpEncoding"
		'
		'tlpIndexOutput
		'
		resources.ApplyResources(Me.tlpIndexOutput, "tlpIndexOutput")
		Me.tlpIndexOutput.Controls.Add(Me.cmdCustomFileOutputBrowse, 2, 0)
		Me.tlpIndexOutput.Controls.Add(Me.txtCustomIndexFileName, 1, 1)
		Me.tlpIndexOutput.Controls.Add(Me.txtCustomIndexFolder, 1, 0)
		Me.tlpIndexOutput.Controls.Add(Me.lblStorageFileName, 0, 1)
		Me.tlpIndexOutput.Controls.Add(Me.lblStorageFolder, 0, 0)
		Me.tlpIndexOutput.Name = "tlpIndexOutput"
		'
		'cmdCustomFileOutputBrowse
		'
		resources.ApplyResources(Me.cmdCustomFileOutputBrowse, "cmdCustomFileOutputBrowse")
		Me.cmdCustomFileOutputBrowse.Name = "cmdCustomFileOutputBrowse"
		Me.cmdCustomFileOutputBrowse.UseVisualStyleBackColor = True
		'
		'txtCustomIndexFileName
		'
		resources.ApplyResources(Me.txtCustomIndexFileName, "txtCustomIndexFileName")
		Me.txtCustomIndexFileName.Name = "txtCustomIndexFileName"
		'
		'txtCustomIndexFolder
		'
		resources.ApplyResources(Me.txtCustomIndexFolder, "txtCustomIndexFolder")
		Me.txtCustomIndexFolder.Name = "txtCustomIndexFolder"
		'
		'lblStorageFileName
		'
		resources.ApplyResources(Me.lblStorageFileName, "lblStorageFileName")
		Me.lblStorageFileName.Name = "lblStorageFileName"
		'
		'lblStorageFolder
		'
		resources.ApplyResources(Me.lblStorageFolder, "lblStorageFolder")
		Me.lblStorageFolder.Name = "lblStorageFolder"
		'
		'chkCustomFileOutput
		'
		resources.ApplyResources(Me.chkCustomFileOutput, "chkCustomFileOutput")
		Me.chkCustomFileOutput.Name = "chkCustomFileOutput"
		Me.chkCustomFileOutput.UseVisualStyleBackColor = True
		'
		'chkDefaultStorageOutput
		'
		resources.ApplyResources(Me.chkDefaultStorageOutput, "chkDefaultStorageOutput")
		Me.chkDefaultStorageOutput.Checked = True
		Me.chkDefaultStorageOutput.CheckState = System.Windows.Forms.CheckState.Checked
		Me.chkDefaultStorageOutput.Name = "chkDefaultStorageOutput"
		Me.chkDefaultStorageOutput.UseVisualStyleBackColor = True
		'
		'tpgIndexStorage
		'
		Me.tpgIndexStorage.Controls.Add(Me.pnlIndexStorage)
		resources.ApplyResources(Me.tpgIndexStorage, "tpgIndexStorage")
		Me.tpgIndexStorage.Name = "tpgIndexStorage"
		Me.tpgIndexStorage.UseVisualStyleBackColor = True
		'
		'txtName
		'
		Me.txtName.AcceptsReturn = True
		resources.ApplyResources(Me.txtName, "txtName")
		Me.txtName.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtName.Name = "txtName"
		'
		'cmdHelp
		'
		resources.ApplyResources(Me.cmdHelp, "cmdHelp")
		Me.cmdHelp.Cursor = System.Windows.Forms.Cursors.Default
		Me.cmdHelp.Name = "cmdHelp"
		Me.cmdHelp.UseVisualStyleBackColor = True
		'
		'tabText
		'
		Me.tabText.Controls.Add(Me.tpgDefaultStorage)
		Me.tabText.Controls.Add(Me.tpgIndexStorage)
		Me.tabText.Controls.Add(Me.tpgAdvance)
		resources.ApplyResources(Me.tabText, "tabText")
		Me.tabText.Name = "tabText"
		Me.tabText.SelectedIndex = 0
		'
		'tpgDefaultStorage
		'
		Me.tpgDefaultStorage.CausesValidation = False
		Me.tpgDefaultStorage.Controls.Add(pnlDefaultStorage)
		resources.ApplyResources(Me.tpgDefaultStorage, "tpgDefaultStorage")
		Me.tpgDefaultStorage.Name = "tpgDefaultStorage"
		Me.tpgDefaultStorage.UseVisualStyleBackColor = True
		'
		'tpgAdvance
		'
		Me.tpgAdvance.Controls.Add(pnlAdvance)
		resources.ApplyResources(Me.tpgAdvance, "tpgAdvance")
		Me.tpgAdvance.Name = "tpgAdvance"
		Me.tpgAdvance.UseVisualStyleBackColor = True
		'
		'cmdApply
		'
		resources.ApplyResources(Me.cmdApply, "cmdApply")
		Me.cmdApply.Cursor = System.Windows.Forms.Cursors.Default
		Me.cmdApply.Name = "cmdApply"
		Me.cmdApply.UseVisualStyleBackColor = True
		'
		'cmdCancel
		'
		resources.ApplyResources(Me.cmdCancel, "cmdCancel")
		Me.cmdCancel.Cursor = System.Windows.Forms.Cursors.Default
		Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.cmdCancel.Name = "cmdCancel"
		Me.cmdCancel.UseVisualStyleBackColor = True
		'
		'cmdOK
		'
		resources.ApplyResources(Me.cmdOK, "cmdOK")
		Me.cmdOK.Cursor = System.Windows.Forms.Cursors.Default
		Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdOK.Name = "cmdOK"
		Me.cmdOK.UseVisualStyleBackColor = True
		'
		'tlpExportName
		'
		resources.ApplyResources(Me.tlpExportName, "tlpExportName")
		Me.tlpExportName.Controls.Add(Me.txtName, 1, 0)
		Me.tlpExportName.Controls.Add(lblName, 0, 0)
		Me.tlpExportName.Name = "tlpExportName"
		'
		'MainSetupForm
		'
		Me.AcceptButton = Me.cmdOK
		resources.ApplyResources(Me, "$this")
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.AutoValidate = System.Windows.Forms.AutoValidate.Disable
		Me.CancelButton = Me.cmdCancel
		Me.Controls.Add(Me.tlpExportName)
		Me.Controls.Add(Me.cmdHelp)
		Me.Controls.Add(Me.tabText)
		Me.Controls.Add(Me.cmdApply)
		Me.Controls.Add(Me.cmdCancel)
		Me.Controls.Add(Me.cmdOK)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
		Me.KeyPreview = True
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "MainSetupForm"
		Me.ShowIcon = False
		Me.ShowInTaskbar = False
		pnlAdvance.ResumeLayout(False)
		pnlAdvance.PerformLayout()
		Me.tlpExportImageAs.ResumeLayout(False)
		Me.tlpExportImageAs.PerformLayout()
		Me.pnlOCROutput.ResumeLayout(False)
		Me.pnlOCROutput.PerformLayout()
		Me.pnlImageOutput.ResumeLayout(False)
		Me.pnlImageOutput.PerformLayout()
		Me.pnlPDFOutput.ResumeLayout(False)
		Me.pnlPDFOutput.PerformLayout()
		pnlDefaultStorage.ResumeLayout(False)
		pnlDefaultStorage.PerformLayout()
		Me.fraFileNaming.ResumeLayout(False)
		Me.fraFileNaming.PerformLayout()
		Me.fraPlusHandling.ResumeLayout(False)
		Me.fraIndexVals.ResumeLayout(False)
		Me.fraIndexVals.PerformLayout()
		CType(Me.grdIndexValuesData, System.ComponentModel.ISupportInitialize).EndInit()
		Me.pnlIndexStorage.ResumeLayout(False)
		Me.pnlIndexStorage.PerformLayout()
		Me.tlpEncoding.ResumeLayout(False)
		Me.tlpEncoding.PerformLayout()
		Me.tlpIndexOutput.ResumeLayout(False)
		Me.tlpIndexOutput.PerformLayout()
		Me.tpgIndexStorage.ResumeLayout(False)
		Me.tabText.ResumeLayout(False)
		Me.tpgDefaultStorage.ResumeLayout(False)
		Me.tpgAdvance.ResumeLayout(False)
		Me.tlpExportName.ResumeLayout(False)
		Me.tlpExportName.PerformLayout()
		Me.ResumeLayout(False)

	End Sub
	Friend WithEvents grdIndexValuesData As Kofax.Connector.Text.IndexValueDataGridView
	Public WithEvents cmdDeleteAllIndex As System.Windows.Forms.Button
	Public WithEvents fraIndexVals As System.Windows.Forms.GroupBox
	Friend WithEvents ctlIndexValueUpDown As Kofax.Connector.Text.UpDownControl
	Public WithEvents cmdAddIndex As System.Windows.Forms.Button
	Public WithEvents cmdDeleteIndex As System.Windows.Forms.Button
	Public WithEvents lblMove As System.Windows.Forms.Label
	Friend WithEvents cboEncoding As System.Windows.Forms.ComboBox
	Public WithEvents pnlIndexStorage As System.Windows.Forms.Panel
	Public WithEvents tpgIndexStorage As System.Windows.Forms.TabPage
	Public WithEvents txtName As System.Windows.Forms.TextBox
	Public WithEvents cmdHelp As System.Windows.Forms.Button
	Public WithEvents tabText As System.Windows.Forms.TabControl
	Public WithEvents tpgAdvance As System.Windows.Forms.TabPage
	Public WithEvents tpgDefaultStorage As System.Windows.Forms.TabPage
	Public WithEvents fraFileNaming As System.Windows.Forms.GroupBox
	Public WithEvents optCustomFileName As System.Windows.Forms.RadioButton
	Public WithEvents chkLeadZeros As System.Windows.Forms.CheckBox
	Public WithEvents optDecimalFilename As System.Windows.Forms.RadioButton
	Public WithEvents optStandardNaming As System.Windows.Forms.RadioButton
	Public WithEvents fraPlusHandling As System.Windows.Forms.GroupBox
	Public WithEvents optRenameIfDup As System.Windows.Forms.RadioButton
	Public WithEvents optReplaceIfDup As System.Windows.Forms.RadioButton
	Public WithEvents optErrorIfDup As System.Windows.Forms.RadioButton
	Public WithEvents cmdApply As System.Windows.Forms.Button
	Public WithEvents cmdCancel As System.Windows.Forms.Button
	Public WithEvents cmdOK As System.Windows.Forms.Button
	Friend WithEvents lblFileName As System.Windows.Forms.Label
	Friend WithEvents lblDefautStorageFolder As System.Windows.Forms.Label
	Friend WithEvents cmdCustom As System.Windows.Forms.Button
	Friend WithEvents txtDefaultStorageFolder As System.Windows.Forms.TextBox
	Friend WithEvents chkCustomFileOutput As System.Windows.Forms.CheckBox
	Friend WithEvents chkDefaultStorageOutput As System.Windows.Forms.CheckBox
	Friend WithEvents cmdCustomFileOutputBrowse As System.Windows.Forms.Button
	Friend WithEvents txtCustomFileOutput As System.Windows.Forms.TextBox
	Friend WithEvents lblReleaseImageAs As System.Windows.Forms.Label
	Friend WithEvents cboImageType As System.Windows.Forms.ComboBox
	Friend WithEvents chkSkipFirstPage As System.Windows.Forms.CheckBox
	Friend WithEvents chkSuppressIfPdfDetected As System.Windows.Forms.CheckBox
	Friend WithEvents lblStorageFolder As System.Windows.Forms.Label
	Friend WithEvents txtCustomIndexFileName As System.Windows.Forms.TextBox
	Friend WithEvents lblStorageFileName As System.Windows.Forms.Label
	Friend WithEvents txtCustomIndexFolder As System.Windows.Forms.TextBox
	Friend WithEvents cmdPdfCustom As System.Windows.Forms.Button
	Friend WithEvents txtPdfCustomPath As System.Windows.Forms.TextBox
	Friend WithEvents chkPdfOutput As System.Windows.Forms.CheckBox
	Friend WithEvents optPdfCustomPath As System.Windows.Forms.RadioButton
	Friend WithEvents optPdfDefaultPath As System.Windows.Forms.RadioButton
	Friend WithEvents pnlPDFOutput As System.Windows.Forms.Panel
	Friend WithEvents pnlImageOutput As System.Windows.Forms.Panel
	Friend WithEvents cmdImgCustom As System.Windows.Forms.Button
	Friend WithEvents txtImgCustomPath As System.Windows.Forms.TextBox
	Friend WithEvents chkImgOutput As System.Windows.Forms.CheckBox
	Friend WithEvents optImgCustomPath As System.Windows.Forms.RadioButton
	Friend WithEvents optImgDefaultPath As System.Windows.Forms.RadioButton
	Friend WithEvents pnlOCROutput As System.Windows.Forms.Panel
	Friend WithEvents optOcrCustomPath As System.Windows.Forms.RadioButton
	Friend WithEvents optOcrDefaultPath As System.Windows.Forms.RadioButton
	Friend WithEvents cmdOcrCustom As System.Windows.Forms.Button
	Friend WithEvents txtOcrCustomPath As System.Windows.Forms.TextBox
	Friend WithEvents chkOcrOutput As System.Windows.Forms.CheckBox
	Friend WithEvents ctlCustomFileNameInput As Kofax.Connector.Common.CustomFileNameInput
	Friend WithEvents tlpExportImageAs As System.Windows.Forms.TableLayoutPanel
	Friend WithEvents tlpExportName As System.Windows.Forms.TableLayoutPanel
	Friend WithEvents tlpIndexOutput As System.Windows.Forms.TableLayoutPanel
	Friend WithEvents tlpEncoding As System.Windows.Forms.TableLayoutPanel
End Class
