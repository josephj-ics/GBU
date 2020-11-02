'********************************************************************************
'***   (c)Copyright Kofax, Inc. 2009 All rights reserved.
'***   Unauthorized use, duplication or distribution is strictly prohibited.
'********************************************************************************
Imports Kofax.ReleaseLib
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Collections.Generic
Imports System.Security
Imports Kofax.Connector.Common
Imports System.Reflection

''' <summary>
''' The MainSetupForm dialog is used for the TC for the release scripts.
''' </summary>
''' <remarks></remarks>
Friend Class MainSetupForm
    Inherits System.Windows.Forms.Form
#Region "Private variables"
    ' The REGISTRY Key for the KC server path
    Private cm_strKcServerPathRegisterKey As String = "HKEY_LOCAL_MACHINE\SOFTWARE\Kofax Image Products\Ascent Capture\3.0\"

    ' The invalid characters in the folder path
    Private cm_strFolderPathSpecialCharacters() As String = New String() {"/", "*", "?", Chr(34).ToString(), "<", ">", "|"} ' Char 34 is "

    Private Enum ShiftConstants As Integer
        AltMask = 4
        CtrlMask = 2
        ShiftMask = 1
    End Enum

    ' Remember to release these in the Form_Unload event
    Private m_oSetupData As Kofax.ReleaseLib.ReleaseSetupData

    ' Export Values manager for the Custom folder path and file name
    Private m_oDefaultFolderManager As ExportValuesManager
    Private m_oOCROutputManager As ExportValuesManager
    Private m_oPDFOutputManager As ExportValuesManager
    Private m_oImageOutputManager As ExportValuesManager
    Private m_oFilenameManager As ExportValuesManager
    Private m_oIndexOutputManager As ExportValuesManager

    ' Share Export Values context menu
    Private m_oExportValuesMenu As ExportValuesMenu

    ' Export Value list contains the default folder path
    Private m_oDefaultFoldersArray As New List(Of ExportValue)

    Private m_bDirty As Boolean
    Private m_bFormSaved As Boolean
    Private m_olinks As New BindingList(Of ExportValue)
    Private m_bEditing As Boolean
    Private m_oErrorProvider As New Kofax.Connector.Common.ErrorProvider(components)
    Private m_oIndexValuesSelectedRow As DataGridViewRow
    Private m_bSupportPDF As Boolean = True
    Private m_bSupportOCR As Boolean = True

    Private Const OCR_QUEUENAME As String = "OCR Full Text"
    Private Const PDF_QUEUENAME As String = "PDF Generator"
    Private Const OCR_FUNCTION As String = "Full Text OCR"
#End Region

#Region "Public methods"
    ''' <summary>
    ''' Create a new instance of this class.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New()
        'This call is required by the Windows Form Designer.
        InitializeComponent()

        '*** Rebranding the word "Kofax"
        Text = Kofax.Connector.Common.GeneralUtils.RebrandName(Me.Text)

        grdIndexValuesData.DeleteButton = cmdDeleteIndex
        grdIndexValuesData.Columns.Add(New DataGridViewSequenceColumn())
        grdIndexValuesData.Columns.Add(New DataGridViewIndexValueColumn())

        grdIndexValuesData.Columns.Add("SourceType", String.Empty)
        grdIndexValuesData.Columns(2).DataPropertyName = "SourceType"
        grdIndexValuesData.Columns(2).Visible = False

        grdIndexValuesData.AutoGenerateColumns = False
        grdIndexValuesData.DataSource = m_olinks
        grdIndexValuesData.CurrentCell = Nothing

        cboEncoding.Items.Add(New EncodingComboBoxItem(Encoding.ASCII))
        cboEncoding.Items.Add(New EncodingComboBoxItem(Encoding.Unicode))
        cboEncoding.Items.Add(New EncodingComboBoxItem(Encoding.UTF8))

        CreateDefaultFolderPaths()
    End Sub

    ''' <summary>
    ''' Generic function to get values from the links table. 
    ''' The value is added if an exception occurs that the value does not exist
    ''' and bInsert is set to true.
    ''' </summary>
    ''' <param name="oLinks">The collection of links' objects</param>
    ''' <param name="strKey">Key string value to search for</param>
    ''' <param name="strDefault">The default string value to insert into the table if not present</param>
    ''' <param name="bInsert">True if the value should be inserted and not present</param>
    ''' <returns>String value found from the custom properties table or the default value if not found.</returns>
    ''' <remarks></remarks>
    Public Function GetLinksValue(ByVal oLinks As Kofax.ReleaseLib.Links, _
                                    ByVal strKey As String, _
                                    ByVal strDefault As String, _
                                    ByVal bInsert As Boolean) As String

        Try
            Return oLinks.Item(strKey).Source
        Catch ex As COMException
            If bInsert AndAlso ex.ErrorCode = &H80042776 Then
                oLinks.Add(strDefault, Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_INDEXFIELD, strKey)
                Return strDefault
            Else
                Throw
            End If
        End Try
    End Function

    ''' <summary>
    ''' This is the entry point. It loads the user interface from the resource file, creates the Index Values popup menu, and 
    ''' loads any previous settings.
    ''' </summary>
    ''' <param name="oSetupData">The setup data object</param>
    ''' <remarks></remarks>
    Public Sub ShowForm(ByRef oSetupData As Kofax.ReleaseLib.ReleaseSetupData)
        m_oSetupData = oSetupData
        grdIndexValuesData.SetupData = m_oSetupData

        ' The data is initially considered clean but not verified.
        ' This is important because the first time a release script
        ' is set up, there are missing values that must be supplied.
        ' The data is not dirty (so the Apply button is disabled) but
        ' the verified flag tells us we still need to validate the
        ' settings before exiting.
        ' Me.Dirty = False
        LoadFormSettings()
        Try
            ' Checking OCR and PDF are supported
            InitSupportOCRandPDF()
            UpdateUISupportedOcrAndPdf()
        Catch ex As Exception
            ' Ignore if can not retrieve setup settings
        End Try
        ShowDialog()
    End Sub

    ''' <summary>
    ''' Validates all controls and shows a message box for only the first
    ''' control that failed validation and sets the focus to this control
    ''' </summary>
    ''' <param name="validationConstraints">See Form.ValidationChildren in MSDN</param>
    ''' <returns>See Form.ValidationChildren in MSDN</returns>
    ''' <remarks>See Form.ValidationChildren in MSDN</remarks>
    Public Overrides Function ValidateChildren(ByVal validationConstraints As System.Windows.Forms.ValidationConstraints) As Boolean
        m_oErrorProvider.Clear()
        Dim bResult As Boolean = MyBase.ValidateChildren(validationConstraints)
        bResult = bResult And ShowValidationResults(Me.Controls)
        Return bResult
    End Function


    ''' <summary>
    ''' The dirty property will get/set the current status of the data.  If the data is dirty, the Apply button is enabled.
    ''' </summary>
    ''' <value>Boolean indicating if data is dirty (TRUE) or clean (FALSE)</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Dirty() As Boolean
        Get
            Dirty = m_bDirty
        End Get
        Set(ByVal Value As Boolean)
            m_bDirty = Value
            If m_bDirty Then
                cmdApply.Enabled = True
            Else
                cmdApply.Enabled = False
            End If
        End Set
    End Property
#End Region

#Region "Protected methods"
    ''' <summary>
    ''' Releases unmanaged resources and performs other cleanup operations before 
    ''' the System.ComponentModel.Component is reclaimed by garbage collection.
    ''' </summary>
    ''' <remarks></remarks>
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    ''' <summary>
    ''' Form overrides dispose to clean up the component list.
    ''' </summary>
    ''' <param name="bDisposing"></param>
    ''' <remarks></remarks>
    Protected Overloads Overrides Sub Dispose(ByVal bDisposing As Boolean)
        If bDisposing Then
            If Not components Is Nothing Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(bDisposing)
    End Sub

    ''' <summary>
    ''' This event is called first whenever the form is about to unload. When the user clicks OK or Cancel we start to unload the form.  
    ''' In this event, we simply validate that all changes are saved and hide the form.  The form is actually unloaded by the ReleaseSetup class.  
    ''' At that time the form is not visible and we allow it to unload.
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)
        MyBase.OnFormClosing(e)

        Dim nResults As Integer

        ' If the form is visible then we only validate
        ' that the data is saved and then hide the form.
        ' We do not allow it to unload yet.
        If Me.Visible Then
            ' Check the form status and if changes have been made,
            ' allow the user to save.  Otherwise just exit.
            If Me.Dirty = True Then
                nResults = MsgBox(My.Resources.MSG_SAVE_SETTING, MsgBoxStyle.YesNoCancel Or MsgBoxStyle.Question, My.Resources.TXT_SAVE_SETTING)

                If nResults = MsgBoxResult.Yes Then
                    ' Try and save
                    If Not SaveReleaseSettings() Then
                        e.Cancel = True
                    End If
                ElseIf nResults = MsgBoxResult.Cancel Then
                    ' Go back to the form
                    e.Cancel = True
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Overrides method OnHelpRequest to show help when the user presses the F11 button.
    ''' </summary>
    ''' <param name="hevent"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnHelpRequested(ByVal hevent As System.Windows.Forms.HelpEventArgs)
        MyBase.OnHelpRequested(hevent)
        If Not hevent.Handled Then
            ShowHelp()
            hevent.Handled = True
        End If
    End Sub

    ''' <summary>
    ''' Overrides method on show help.
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnShown(ByVal e As System.EventArgs)
        MyBase.OnShown(e)
        grdIndexValuesData.ClearSelection()
    End Sub
#End Region

#Region "Private methods"
    ''' <summary>
    ''' Formats a specified string to be appended on a new line of another string.  
    ''' If more than 10 strings have been appended, 
    ''' the function substitutes the phrase "And More" for the specified string.
    ''' </summary>
    ''' <param name="nCount">The number of strings that have been appended.</param>
    ''' <param name="strField">The string to append</param>
    ''' <returns>The formatted string</returns>
    ''' <remarks>
    ''' This function is used solely by the data verification routines 
    ''' to list the Index Fields and Batch Fields that were not used 
    ''' as document Index Values.
    ''' </remarks>
    Private Function AddString(ByRef nCount As Integer, ByRef strField As String) As String
        nCount = nCount + 1
        If (nCount < 10) Then
            Return vbCrLf & vbTab & "- " & strField
        ElseIf (nCount = 10) Then
            Return vbCrLf & vbTab & My.Resources.TXT_AND_MORE
        Else
            Return String.Empty
        End If
    End Function

    ''' <summary>
    ''' Converts the old index file path to the new index storage configuration
    ''' </summary>
    ''' <param name="strOldIndexFilename">Old index file name</param>
    ''' <param name="oIndexOutputManager">Index output manager</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function BuildIndexStorageFromOldConfiguration(ByVal strOldIndexFilename As String, _
                                                            ByRef oIndexOutputManager As ExportValuesManager) As String
        Dim nFistIndex As Integer = strOldIndexFilename.LastIndexOf(c_strBackSlashCharacter)
        Dim nLastIndex As Integer = strOldIndexFilename.LastIndexOf(c_strBackSlashCharacter)
        If nLastIndex <> -1 Then
            Dim oExportValuesArray As New List(Of ExportValue)
            If nFistIndex <> nLastIndex Then
                oExportValuesArray.Add(New ExportValue(strOldIndexFilename.Substring(0, nLastIndex)))
            Else
                ' For the case the file path is something like c:\test.txt
                oExportValuesArray.Add(New ExportValue(strOldIndexFilename.Substring(0, nLastIndex + 1)))
            End If
            oIndexOutputManager = New ExportValuesManager(c_strMacroKeyIndexCustomStorageFolder, oExportValuesArray)
            Return strOldIndexFilename.Substring(nLastIndex + 1, strOldIndexFilename.Length - nLastIndex - 1)
        Else
            oIndexOutputManager = New ExportValuesManager(c_strMacroKeyIndexCustomStorageFolder, m_oDefaultFoldersArray)
            Return strOldIndexFilename
        End If
    End Function

    ''' <summary>
    ''' Creates the default Export Values list containing the default folder path
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreateDefaultFolderPaths()
        Dim strServerPath As String = CStr(My.Computer.Registry.GetValue(cm_strKcServerPathRegisterKey, "serverPath", "C:\"))
        m_oDefaultFoldersArray.Add(New ExportValue(strServerPath + "\Export", ReleaseLib.KfxLinkSourceType.KFX_REL_TEXTCONSTANT))
        m_oDefaultFoldersArray.Add(New ExportValue(My.Resources.TXT_MACRO_BATCHCLASSNAME, ReleaseLib.KfxLinkSourceType.KFX_REL_VARIABLE))
        m_oDefaultFoldersArray.Add(New ExportValue(My.Resources.TXT_MACRO_DOCCLASSNAME, ReleaseLib.KfxLinkSourceType.KFX_REL_VARIABLE))
        m_oDefaultFoldersArray.Add(New ExportValue(My.Resources.TXT_MACRO_DOCID, ReleaseLib.KfxLinkSourceType.KFX_REL_VARIABLE))
    End Sub

    ''' <summary>
    ''' This routine gets all image types from the SetupData object and fills 
    ''' the combo box with the description and ID
    ''' </summary>
    ''' <param name="oSetupData">ReleaseSetupData object</param>
    ''' <param name="bMultiPageOnly">flag to list only multipage image formats</param>
    ''' <remarks></remarks>
    Private Sub FillUiWithImageType(ByRef oSetupData As Kofax.ReleaseLib.ReleaseSetupData, ByRef bMultiPageOnly As Boolean)
        Dim oReleaseSetupData As New ReleaseSetupData(oSetupData)

        With cboImageType
            ' Start with an empty combo box
            .Items.Clear()
            ' Get each item and add it
            Using eEnumerator As New ComEnumerator(oReleaseSetupData.ImageTypes.GetEnumerator())
                While eEnumerator.MoveNext()
                    Dim oImageType As ImageType = New ImageType(eEnumerator.Current)
                    If Not bMultiPageOnly Or oImageType.MultiplePage Then
                        .Items.Add(New ImageTypeComboBoxItem(oImageType))
                    End If
                End While
            End Using

        End With
    End Sub

    ''' <summary>
    ''' Disable Custom file name if the option use original file name 
    ''' in the release properties is choosen.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DisableIfUsingOriginal()
        ' Disable if using original file names
        If m_oSetupData.UseOriginalFileNames = -1 Then
            fraFileNaming.Enabled = False
            ctlCustomFileNameInput.Enabled = False
            lblFileName.Enabled = False
        End If
    End Sub

    ''' <summary>
    ''' This routine will fill the user interface with the current release settings 
    ''' for this batch/doc class combination from SetupData.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FillUiWithSettings()
        Try
            Cursor = Cursors.WaitCursor

            Using oCustomPropertiesReader As New CustomPropertiesReaderWriter(m_oSetupData.CustomProperties)
                Dim oLinks As Kofax.ReleaseLib.Links = m_oSetupData.Links
                Using New ComDisposer(oLinks)
                    ' Release Destination Name
                    txtName.Text = m_oSetupData.Name

                    ' Default Storage Tab
                    ' Get Default Folder path from Setup Data
                    m_oDefaultFolderManager = New ExportValuesManager(c_strMacroKeyDefaultStorageLocation, m_oSetupData)
                    If m_oDefaultFolderManager.Values.Count = 0 Then
                        ' Upgrade the old configuration
                        m_oDefaultFolderManager = New ExportValuesManager(c_strMacroKeyDefaultStorageLocation, m_oDefaultFoldersArray)
                    End If
                    Me.txtDefaultStorageFolder.Text = m_oDefaultFolderManager.BuildFolderDisplayedPath()

                    PlusFillFileNameUISettings(oCustomPropertiesReader)
                    DisableIfUsingOriginal()

                    ' Index Storage Tab
                    chkDefaultStorageOutput.Checked = oCustomPropertiesReader.UseDefaultStorage

                    Dim strOldIndexFilename As String = oCustomPropertiesReader.IndexFileName
                    ' Check if the index file name is in the old configuration?                    

                    If strOldIndexFilename Is String.Empty Then

                        chkCustomFileOutput.Checked = oCustomPropertiesReader.UseCustomStorage
                        ' Init the custom index output folder path
                        If chkCustomFileOutput.Checked Then
                            m_oIndexOutputManager = New ExportValuesManager(c_strMacroKeyIndexCustomStorageFolder, m_oSetupData)
                            txtCustomIndexFolder.Text = m_oIndexOutputManager.BuildFolderDisplayedPath()
                            ' Init Custom index file name
                            txtCustomIndexFileName.Text = oCustomPropertiesReader.CustomIndexFileName
                        Else
                            m_oIndexOutputManager = New ExportValuesManager(c_strMacroKeyIndexCustomStorageFolder, m_oDefaultFoldersArray)
                        End If
                    Else
                        ' Convert old configuration to new one
                        chkCustomFileOutput.Checked = True
                        txtCustomIndexFileName.Text = BuildIndexStorageFromOldConfiguration(strOldIndexFilename, m_oIndexOutputManager)
                        txtCustomIndexFolder.Text = m_oIndexOutputManager.BuildFolderDisplayedPath()
                    End If

                    LoadIndexValues(oLinks)
                    cboEncoding.SelectedItem = oCustomPropertiesReader.Encoding

                    ' Advance Tab
                    ' Image Output
                    ' Check the folder tree in the old configuration
                    If Not oCustomPropertiesReader.IsNewVersion Then
                        Dim nTotalCounter As Integer = oCustomPropertiesReader.Folders
                        Dim oFolderExportValuesArray As New List(Of ExportValue)
                        oFolderExportValuesArray.Add(New ExportValue(m_oSetupData.ImageFilePath, ReleaseLib.KfxLinkSourceType.KFX_REL_TEXTCONSTANT))
                        ' Get the custom directory values from the links database
                        For nLoopCounter As Integer = 1 To nTotalCounter
                            Dim oFolderNode As ExportValue = GetExportValueByDestinationValue(m_oSetupData, String.Format("Folder{0} Name", nLoopCounter))
                            If (oFolderNode IsNot Nothing) Then
                                oFolderExportValuesArray.Add(oFolderNode)
                            End If
                        Next
                        m_oImageOutputManager = New ExportValuesManager(c_strMacroKeyImageStorageFolder, oFolderExportValuesArray)

                        ' Image Output
                        chkImgOutput.Checked = True
                        cmdImgCustom.Enabled = True
                        optImgCustomPath.Checked = True
                        optImgDefaultPath.Checked = False
                        txtImgCustomPath.Text = m_oImageOutputManager.BuildFolderDisplayedPath()

                        ' When upgrading the old configuration this option is always false 
                        ' because it is new.
                        chkSuppressIfPdfDetected.Checked = False

                        ' Delete the old configuration
                        m_oSetupData.ImageFilePath = String.Empty
                    Else
                        chkImgOutput.Checked = Not oCustomPropertiesReader.DisableImageExport
                        If chkImgOutput.Checked Then

                            ' Always use ImageFilePath first If it has value due to substitution path changes (SPR 112776)
                            If Not String.IsNullOrEmpty(m_oSetupData.ImageFilePath) Then
                                Dim oFolderExportValuesArray As New List(Of ExportValue)
                                oFolderExportValuesArray.Add(New ExportValue(m_oSetupData.ImageFilePath, ReleaseLib.KfxLinkSourceType.KFX_REL_TEXTCONSTANT))
                                m_oImageOutputManager = New ExportValuesManager(c_strMacroKeyImageStorageFolder, oFolderExportValuesArray)
                                cmdImgCustom.Enabled = True
                                optImgCustomPath.Checked = True

                                ' Delete the old configuration
                                m_oSetupData.ImageFilePath = String.Empty
                            Else
                                Dim bIsImageDefault As Boolean = oCustomPropertiesReader.ImgDefaultStorage
                                If Not bIsImageDefault Then
                                    cmdImgCustom.Enabled = True
                                    m_oImageOutputManager = New ExportValuesManager(c_strMacroKeyImageStorageFolder, m_oSetupData)
                                    txtImgCustomPath.Text = m_oImageOutputManager.BuildFolderDisplayedPath()
                                Else
                                    m_oImageOutputManager = New ExportValuesManager(c_strMacroKeyImageStorageFolder, m_oDefaultFoldersArray)
                                End If
                                optImgCustomPath.Checked = Not bIsImageDefault
                                optImgDefaultPath.Checked = bIsImageDefault
                            End If

                        Else
                            optImgCustomPath.Enabled = False
                            optImgDefaultPath.Enabled = False
                            m_oImageOutputManager = New ExportValuesManager(c_strMacroKeyImageStorageFolder, m_oDefaultFoldersArray)
                        End If

                        chkSuppressIfPdfDetected.Checked = oCustomPropertiesReader.SuppressIfPdfDetected
                    End If

                    If chkImgOutput.Checked Then
                        chkSkipFirstPage.Checked = m_oSetupData.SkipFirstPage <> 0
                    End If

                    Dim nImageType As Integer = m_oSetupData.ImageType
                    SetImageType(nImageType)

                    ' OCR Ouput
                    Dim bOCROutput As Boolean = False
                    If Not oCustomPropertiesReader.DisableTextExport Then
                        bOCROutput = True
                        ' Check if it is the new configuration
                        If oCustomPropertiesReader.IsNewVersion Then
                            ' Always use TextFilePath first If it has value due to substitution path changes (SPR 112776)
                            If Not String.IsNullOrEmpty(m_oSetupData.TextFilePath) Then
                                Dim oFolderExportValuesArray As New List(Of ExportValue)
                                oFolderExportValuesArray.Add(New ExportValue(m_oSetupData.TextFilePath, ReleaseLib.KfxLinkSourceType.KFX_REL_TEXTCONSTANT))
                                m_oOCROutputManager = New ExportValuesManager(c_strMacroKeyOcrStorageFolder, oFolderExportValuesArray)
                                cmdOcrCustom.Enabled = True
                                optOcrCustomPath.Checked = True

                                ' Delete the old configuration
                                m_oSetupData.TextFilePath = String.Empty
                            Else
                                Dim bIsOcrDefault As Boolean = oCustomPropertiesReader.OcrDefaultStorage
                                If Not bIsOcrDefault Then
                                    cmdOcrCustom.Enabled = True
                                    m_oOCROutputManager = New ExportValuesManager(c_strMacroKeyOcrStorageFolder, m_oSetupData)
                                    txtOcrCustomPath.Text = m_oOCROutputManager.BuildFolderDisplayedPath()
                                Else
                                    m_oOCROutputManager = New ExportValuesManager(c_strMacroKeyOcrStorageFolder, m_oDefaultFoldersArray)
                                End If
                                optOcrCustomPath.Checked = Not bIsOcrDefault
                                optOcrDefaultPath.Checked = bIsOcrDefault
                            End If
                        Else
                            ' Convert old configuration to new
                            If oCustomPropertiesReader.SeparateDirectories Then
                                If m_oSetupData.TextFilePath <> String.Empty Then
                                    Dim oTextExportValuesArray As New List(Of ExportValue)
                                    oTextExportValuesArray.Add(New ExportValue(m_oSetupData.TextFilePath, ReleaseLib.KfxLinkSourceType.KFX_REL_TEXTCONSTANT))
                                    m_oOCROutputManager = New ExportValuesManager(c_strMacroKeyOcrStorageFolder, oTextExportValuesArray)
                                    m_oSetupData.TextFilePath = String.Empty
                                Else
                                    ' Uncheck the OCR output if the old setting doesn't have a OCR output path.
                                    bOCROutput = False
                                    m_oOCROutputManager = New ExportValuesManager(c_strMacroKeyOcrStorageFolder, m_oDefaultFoldersArray)
                                End If
                            Else
                                m_oOCROutputManager = New ExportValuesManager(c_strMacroKeyOcrStorageFolder, m_oImageOutputManager.Values)
                            End If

                            If bOCROutput Then
                                optOcrCustomPath.Checked = True
                                optOcrDefaultPath.Checked = False
                                cmdOcrCustom.Enabled = True
                                txtOcrCustomPath.Text = m_oOCROutputManager.BuildFolderDisplayedPath()
                            End If
                        End If

                    Else
                        optOcrCustomPath.Enabled = False
                        optOcrDefaultPath.Enabled = False
                        m_oOCROutputManager = New ExportValuesManager(c_strMacroKeyOcrStorageFolder, m_oDefaultFoldersArray)
                    End If
                    chkOcrOutput.Checked = bOCROutput

                    ' PDF OutPut
                    Dim bPDFOutput As Boolean = False
                    If oCustomPropertiesReader.EnableKofaxPdfExport Then
                        bPDFOutput = True
                        'Check if it is the new configuration
                        If oCustomPropertiesReader.IsNewVersion Then
                            ' Always use KofaxPDFPath first If it has value due to substitution path changes (SPR 112776)
                            If Not String.IsNullOrEmpty(m_oSetupData.KofaxPDFPath) Then
                                Dim oFolderExportValuesArray As New List(Of ExportValue)
                                oFolderExportValuesArray.Add(New ExportValue(m_oSetupData.KofaxPDFPath, ReleaseLib.KfxLinkSourceType.KFX_REL_TEXTCONSTANT))
                                m_oPDFOutputManager = New ExportValuesManager(c_strMacroKeyPdfStorageFolder, oFolderExportValuesArray)
                                cmdPdfCustom.Enabled = True
                                optPdfCustomPath.Checked = True

                                ' Delete the old configuration
                                m_oSetupData.KofaxPDFPath = String.Empty
                            Else
                                Dim bIsPDFDefault As Boolean = oCustomPropertiesReader.PdfDefaultStorage
                                If Not bIsPDFDefault Then
                                    cmdPdfCustom.Enabled = True
                                    m_oPDFOutputManager = New ExportValuesManager(c_strMacroKeyPdfStorageFolder, m_oSetupData)
                                    txtPdfCustomPath.Text = m_oPDFOutputManager.BuildFolderDisplayedPath()
                                Else
                                    m_oPDFOutputManager = New ExportValuesManager(c_strMacroKeyPdfStorageFolder, m_oDefaultFoldersArray)
                                End If
                                optPdfCustomPath.Checked = Not bIsPDFDefault
                                optPdfDefaultPath.Checked = bIsPDFDefault
                            End If

                        Else
                            ' Convert old configuration to new
                            If oCustomPropertiesReader.SeparateDirectories Then
                                If m_oSetupData.KofaxPDFPath <> String.Empty Then
                                    Dim oPdfExportValuesArray As New List(Of ExportValue)
                                    oPdfExportValuesArray.Add(New ExportValue(m_oSetupData.KofaxPDFPath, ReleaseLib.KfxLinkSourceType.KFX_REL_TEXTCONSTANT))
                                    m_oPDFOutputManager = New ExportValuesManager(c_strMacroKeyPdfStorageFolder, oPdfExportValuesArray)
                                    m_oSetupData.KofaxPDFPath = String.Empty
                                Else
                                    ' Uncheck the PDF output if the old setting doesn't have a PDF output path.
                                    bPDFOutput = False
                                    m_oPDFOutputManager = New ExportValuesManager(c_strMacroKeyPdfStorageFolder, m_oDefaultFoldersArray)
                                End If
                            Else
                                m_oPDFOutputManager = New ExportValuesManager(c_strMacroKeyPdfStorageFolder, m_oImageOutputManager.Values)
                            End If

                            If bPDFOutput Then
                                optPdfCustomPath.Checked = True
                                optPdfDefaultPath.Checked = False
                                cmdPdfCustom.Enabled = True
                                txtPdfCustomPath.Text = m_oPDFOutputManager.BuildFolderDisplayedPath()
                            End If
                        End If
                    Else
                        optPdfCustomPath.Enabled = False
                        optPdfDefaultPath.Enabled = False
                        m_oPDFOutputManager = New ExportValuesManager(c_strMacroKeyPdfStorageFolder, m_oDefaultFoldersArray)
                    End If
                    chkPdfOutput.Checked = bPDFOutput

                End Using
            End Using
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' This routine will initialize the Index Values list with the entries from the Links collection.
    ''' </summary>
    ''' <param name="oLinkList">Links collection</param>
    ''' <remarks></remarks>
    Private Sub LoadIndexValues(ByRef oLinkList As Kofax.ReleaseLib.Links)
        Dim nLinks As Short = 0

        ' Use a sorted list to put the index values in sequence order
        Dim oSortedIndexes As New SortedList(Of Integer, ExportValue)

        ' Loop through all of the values in the ReleaseLib Links collection
        Using eEnumerator As New ComEnumerator(oLinkList.GetEnumerator())
            While eEnumerator.MoveNext()
                Dim oCurrIndex As Kofax.ReleaseLib.Link = CType(eEnumerator.Current, Kofax.ReleaseLib.Link)
                Using New ComDisposer(oCurrIndex)
                    ' PDF links have Destination like "PDF_****". 
                    ' Custom directory entries have "Folder xxx name" and "Plus Filenaming".
                    ' We need to ignore these non-numeric types
                    If IsNumeric(oCurrIndex.Destination) Then
                        ' The Destination is used as the index into the
                        ' list to keep the links sorted by sequence
                        Dim nSequence As Integer = CInt(oCurrIndex.Destination)
                        oSortedIndexes.Item(nSequence) = New ExportValue(oCurrIndex.Source, oCurrIndex.SourceType, "")
                    End If
                End Using
            End While
        End Using

        ' Now add the ordered list of Link objects to our UI binding list.
        For Each oLink As ExportValue In oSortedIndexes.Values
            m_olinks.Add(oLink)
        Next
    End Sub

    ''' <summary>
    ''' Load all the settings while the wait dialog is shown.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadFormSettings()
        'Initialize the share Export Values Context Menu
        m_oExportValuesMenu = New ExportValuesMenu()
        m_oExportValuesMenu.Init(m_oSetupData)

        ctlCustomFileNameInput.Init(m_oExportValuesMenu, m_oErrorProvider)
        InitializeFileNameWithDefaut()
        'Initialize the Advance output
        FillUiWithImageType(m_oSetupData, False)

        ' If there is no currently existing data, load
        ' the UI with defaults, otherwise load the
        ' current settings.
        If m_oSetupData.New <> 0 Then
            FillUiWithDefaults()
        Else
            FillUiWithSettings()
        End If

        Try
            ' Checking OCR and PDF are supported
            InitSupportOCRandPDF()
            UpdateUISupportedOcrAndPdf()
        Catch ex As Exception
            ' Ignore if can not retrieve setup settings
        End Try

        Me.Dirty = False
    End Sub

    ''' <summary>
    ''' This routine will initialize the Index Values with all Batch Fields and Index Fields.
    ''' </summary>
    ''' <param name="oSetupData">ReleaseSetupData object</param>
    ''' <remarks></remarks>
    Private Sub InitializeIndexValues(ByRef oSetupData As Kofax.ReleaseLib.ReleaseSetupData)
        m_olinks = New BindingList(Of ExportValue)
        grdIndexValuesData.DataSource = m_olinks
        grdIndexValuesData.CurrentCell = Nothing

        ' Add each Batch Field to the list of Index Values
        Using eEnumerator As New ComEnumerator(oSetupData.BatchFields.GetEnumerator())
            While eEnumerator.MoveNext()
                Dim oBatchField As Kofax.ReleaseLib.BatchField = CType(eEnumerator.Current, Kofax.ReleaseLib.BatchField)
                m_olinks.Add(New ExportValue(oBatchField.Name, KfxLinkSourceType.KFX_REL_BATCHFIELD, ""))
            End While
        End Using

        ' Add each Index Field to the list of Index Values
        Using eEnumerator As New ComEnumerator(oSetupData.IndexFields.GetEnumerator())
            While eEnumerator.MoveNext()
                Dim oIndexField As Kofax.ReleaseLib.IndexField = CType(eEnumerator.Current, Kofax.ReleaseLib.IndexField)
                m_olinks.Add(New ExportValue(oIndexField.Name, KfxLinkSourceType.KFX_REL_INDEXFIELD, ""))
            End While
        End Using
    End Sub

    ''' <summary>
    ''' If the user is editing the release settings for the first time on this batch/doc class instance, 
    ''' fill the user interface with default values rather than reading the settings in the data object.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FillUiWithDefaults()
        cboEncoding.SelectedIndex = 0
        InitializeCustomFolderWithDefaut()
        ' Default Storage Tab
        ImageDefaultUISettings()
        InitializeIndexValues(m_oSetupData)
        chkSkipFirstPage.Checked = False

        ' Defaults to multi-page TIFF release
        SetImageType(ImageFormatEnum.MTIFF_G4)

        DisableIfUsingOriginal()

    End Sub

    ''' <summary>
    ''' This routine sets the image type combo box index to the image type passed in.  If the image type isn't found, 
    ''' a msgbox is shown and the first image type is selected.
    ''' </summary>
    ''' <param name="nType">Selected image type</param>
    ''' <remarks></remarks>
    Private Sub SetImageType(ByRef nType As Integer)
        Dim oImageType As ImageTypeComboBoxItem

        For nIndex As Integer = 0 To cboImageType.Items.Count - 1
            oImageType = CType(cboImageType.Items(nIndex), ImageTypeComboBoxItem)
            If oImageType.Type = nType Then
                cboImageType.SelectedIndex = nIndex
                Exit Sub
            End If
        Next

        cboImageType.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' Initialize all the custom folder path manager with the default folder.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeCustomFolderWithDefaut()
        m_oDefaultFolderManager = New ExportValuesManager(c_strMacroKeyDefaultStorageLocation, m_oDefaultFoldersArray)
        Me.txtDefaultStorageFolder.Text = m_oDefaultFolderManager.BuildFolderDisplayedPath()

        m_oOCROutputManager = New ExportValuesManager(c_strMacroKeyOcrStorageFolder, m_oDefaultFoldersArray)
        m_oPDFOutputManager = New ExportValuesManager(c_strMacroKeyPdfStorageFolder, m_oDefaultFoldersArray)
        m_oImageOutputManager = New ExportValuesManager(c_strMacroKeyImageStorageFolder, m_oDefaultFoldersArray)
        m_oIndexOutputManager = New ExportValuesManager(c_strMacroKeyIndexCustomStorageFolder, m_oDefaultFoldersArray)
    End Sub

    ''' <summary>
    ''' Initialize all the file name manager with the default name.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeFileNameWithDefaut()
        Dim oDefautValuesArray As New List(Of ExportValue)
        oDefautValuesArray.Add(New ExportValue(My.Resources.TXT_MACRO_DOCID, ReleaseLib.KfxLinkSourceType.KFX_REL_VARIABLE))
        m_oFilenameManager = New ExportValuesManager(c_strMacroKeyDefaultFileName, oDefautValuesArray)
    End Sub

    ''' <summary>
    ''' Move to index value column in the index values grid.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub MoveToIndexValueColumn()
        If grdIndexValuesData.CurrentCellAddress.Y >= 0 Then
            grdIndexValuesData.CurrentCell = grdIndexValuesData.Item(1, grdIndexValuesData.CurrentCellAddress.Y)
        End If
    End Sub

    ''' <summary>
    ''' Get the control that failed validation lowest in the tab order.
    ''' </summary>
    ''' <param name="oControlsCollection">The controls to check for failed validation</param>
    ''' <param name="oInvalidControl">The lowest control in tab order that failed validation</param>
    ''' <remarks>Uses recursion to search all controls</remarks>
    Private Sub FindFirstInvalidControl(ByVal oControlsCollection As Control.ControlCollection, ByRef oInvalidControl As Control)
        For Each oControl As Control In oControlsCollection
            If Not String.IsNullOrEmpty(m_oErrorProvider.GetError(oControl)) And _
                (oInvalidControl Is Nothing OrElse oControl.TabIndex < oInvalidControl.TabIndex) Then
                oInvalidControl = oControl
            ElseIf oControl.Controls IsNot Nothing Then
                FindFirstInvalidControl(oControl.Controls, oInvalidControl)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Get validation infomation of controls.
    ''' </summary>
    ''' <param name="oControlCollection">Collection of controls</param>
    ''' <param name="oHasInfomationControlsArray">Collection of has validation infomation controls</param>
    ''' <remarks></remarks>
    Private Sub FindHasInformationControls(ByVal oControlCollection As Control.ControlCollection, ByRef oHasInfomationControlsArray As List(Of Control))
        For Each control As Control In oControlCollection
            Dim oList As List(Of String) = m_oErrorProvider.GetInformation(control)
            If oList IsNot Nothing Then
                oHasInfomationControlsArray.Add(control)
            ElseIf control.Controls IsNot Nothing Then
                FindHasInformationControls(control.Controls, oHasInfomationControlsArray)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Looks through all controls and if any failed validation shows a message box
    ''' and sets focus to the control
    ''' </summary>
    ''' <param name="oControlsCollection">The controls to look for errors in</param>
    ''' <returns>True if controls passed validation otherwise false</returns>
    ''' <remarks></remarks>
    Private Function ShowValidationResults(ByVal oControlsCollection As Control.ControlCollection) As Boolean
        Dim oInvalidControl As Control = Nothing
        ' Find first validation error 
        FindFirstInvalidControl(oControlsCollection, oInvalidControl)
        If oInvalidControl IsNot Nothing Then
            Dim strMessage As String = m_oErrorProvider.GetError(oInvalidControl)
            SelectTabPage(oInvalidControl)
            MessageBox.Show(My.Resources.TXT_DATA_VERIFICATION_FAILED & strMessage, c_strShortProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            oInvalidControl.Focus()
            Return False
        Else
            ' Warning message
            Dim oHasWarningControlsArray As New List(Of Control)
            FindHasInformationControls(oControlsCollection, oHasWarningControlsArray)
            For Each control As Control In oHasWarningControlsArray
                Dim oMessageList As List(Of String) = m_oErrorProvider.GetInformation(control)
                SelectTabPage(control)
                control.Focus()
                For Each strMessage As String In oMessageList
                    If Windows.Forms.DialogResult.Cancel = _
                      MessageBox.Show(My.Resources.TXT_DATA_VERIFY_INFOMATION & strMessage, _
                          c_strShortProductName, _
                          MessageBoxButtons.OKCancel, _
                          MessageBoxIcon.Information) Then
                        Return False
                    End If
                Next
            Next
            Return True
        End If
    End Function

    ''' <summary>
    ''' Selects the tab containing the control passed in
    ''' </summary>
    ''' <param name="oControl">The control to select the tab page for</param>
    ''' <remarks></remarks>
    Private Sub SelectTabPage(ByVal oControl As Control)
        If oControl.Parent IsNot Nothing Then
            If TypeOf oControl.Parent Is TabPage Then
                tabText.SelectedTab = CType(oControl.Parent, TabPage)
            Else
                SelectTabPage(oControl.Parent)
            End If
        End If
    End Sub

    ''' <summary>
    ''' This routine will save the setup data to the KC database through the SetupData properties and collections.
    ''' </summary>
    ''' <returns>True/False</returns>
    ''' <remarks></remarks>
    Private Function SaveReleaseSettings() As Boolean

        If Not ValidateChildren(ValidationConstraints.None) Then
            Return False
        End If

        If Not cmdApply.Enabled And m_oSetupData.New <> -1 Then
            Return True
        End If

        ' Change to a Wait cursor because this may take
        ' a while. Remember to change it back at all
        ' possible exit points.
        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        Try
            ' Clear all entries from the custom properties collection
            m_oSetupData.CustomProperties.RemoveAll()
            ' Clear all the current indexes and reload them
            m_oSetupData.Links.RemoveAll()

            ' Release Destination Name
            m_oSetupData.Name = txtName.Text

            Dim oCustomPropertiesWriter As New CustomPropertiesReaderWriter(m_oSetupData.CustomProperties)
            ' Default Storage Tab 

            ' Save Default Storage Folder
            m_oDefaultFolderManager.saveToSetupData(m_oSetupData)

            ' Save file name
            FileNameSaveSettings(oCustomPropertiesWriter)

            ' Index Storage Tab
            oCustomPropertiesWriter.UseDefaultStorage = chkDefaultStorageOutput.Checked
            oCustomPropertiesWriter.UseCustomStorage = chkCustomFileOutput.Checked

            ' Save the Custom storage folder path
            If chkCustomFileOutput.Checked Then
                m_oIndexOutputManager.saveToSetupData(m_oSetupData)
                oCustomPropertiesWriter.CustomIndexFileName = txtCustomIndexFileName.Text
            End If

            ' TODO Save storage folder and file name
            oCustomPropertiesWriter.Encoding = CType(cboEncoding.SelectedItem, EncodingComboBoxItem).Encoding
            SaveIndexValues()

            ' Advance tab
            ' OCR Output
            oCustomPropertiesWriter.DisableTextExport = Not chkOcrOutput.Checked
            oCustomPropertiesWriter.OcrDefaultStorage = optOcrDefaultPath.Checked
            If chkOcrOutput.Checked And optOcrCustomPath.Checked Then
                m_oOCROutputManager.saveToSetupData(m_oSetupData)
            End If

            ' PDF Output
            oCustomPropertiesWriter.EnableKofaxPdfExport = chkPdfOutput.Checked
            oCustomPropertiesWriter.PdfDefaultStorage = optPdfDefaultPath.Checked
            If chkPdfOutput.Checked And optPdfCustomPath.Checked Then
                m_oPDFOutputManager.saveToSetupData(m_oSetupData)
            End If

            ' Image Output
            oCustomPropertiesWriter.DisableImageExport = Not chkImgOutput.Checked
            oCustomPropertiesWriter.ImgDefaultStorage = optImgDefaultPath.Checked
            If chkImgOutput.Checked And optImgCustomPath.Checked Then
                m_oImageOutputManager.saveToSetupData(m_oSetupData)
            End If

            oCustomPropertiesWriter.SuppressIfPdfDetected = chkSuppressIfPdfDetected.Checked
            ' End Using

            If chkSkipFirstPage.Checked = True Then
                m_oSetupData.SkipFirstPage = 1
            Else
                m_oSetupData.SkipFirstPage = 0
            End If

            If cboImageType.Enabled Then
                m_oSetupData.ImageType = CType(cboImageType.SelectedItem, ImageTypeComboBoxItem).Type
            Else
                m_oSetupData.ImageType = ImageFormatEnum.MTIFF_G4
            End If

            ' Save and clean up
            m_oSetupData.Apply()
            Me.Dirty = False
            m_bFormSaved = True
            Return True
        Finally
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try
    End Function

    ''' <summary>
    ''' This routine saves the Index Values to the links collection.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SaveIndexValues()
        Dim oIndexList As Kofax.ReleaseLib.Links = m_oSetupData.Links
        Using New ComDisposer(oIndexList)
            ' Add each link one at a time to the collection
            For i As Integer = 0 To m_olinks.Count - 1
                oIndexList.Add(m_olinks(i).SourceName, CType(m_olinks(i).SourceType, Kofax.ReleaseLib.KfxLinkSourceType), (i + 1).ToString())
            Next
        End Using
    End Sub

    ''' <summary>
    ''' This routine will delete all Index Values from the list of links.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteAllIndex()
        ' Discard all Index Values
        m_oIndexValuesSelectedRow = Nothing
        m_olinks = New BindingList(Of ExportValue)
        grdIndexValuesData.DataSource = m_olinks
        grdIndexValuesData.CurrentCell = Nothing
    End Sub

    ''' <summary>
    ''' This routine will move the Index Value up or down one position in the array
    ''' </summary>
    ''' <param name="oDirection"></param>
    ''' <remarks>
    ''' UP and DOWN represent the visual appearance in the list to the user so UP actually moves the link to a 
    ''' lower index while DOWN moves it to a higher index.
    ''' </remarks>
    Private Sub MoveIndex(ByRef oDirection As ArrowDirection)
        Debug.Assert(grdIndexValuesData.SelectedRows.Count = 1)

        Dim oTmpIndex As ExportValue
        Dim nIndex As Integer = grdIndexValuesData.SelectedRows(0).Index
        oTmpIndex = m_olinks(nIndex)
        Select Case oDirection
            Case ArrowDirection.Up
                ' Make sure we're not already at the start of list
                If nIndex > 0 Then
                    m_olinks(nIndex) = m_olinks(nIndex - 1)
                    m_olinks(nIndex - 1) = oTmpIndex
                    grdIndexValuesData.Rows(nIndex - 1).Selected = True
                    grdIndexValuesData.CurrentCell = grdIndexValuesData(1, nIndex - 1)
                    grdIndexValuesData.BeginEdit(False)
                End If

            Case ArrowDirection.Down
                ' Make sure we're not already at the end of list
                If nIndex < m_olinks.Count - 1 Then
                    ' Swap the two items in the list
                    m_olinks(nIndex) = m_olinks(nIndex + 1)
                    m_olinks(nIndex + 1) = oTmpIndex
                    grdIndexValuesData.Rows(nIndex + 1).Selected = True
                    grdIndexValuesData.CurrentCell = grdIndexValuesData(1, nIndex + 1)
                    grdIndexValuesData.BeginEdit(False)
                End If
        End Select
    End Sub

    ''' <summary>
    ''' Filling release setup UI with previously saved settings for the Plus release.
    ''' </summary>
    ''' <param name="oCustomPropertiesReader">The filled in ReleaseSetupData object</param>
    ''' <remarks></remarks>
    Private Sub PlusFillFileNameUISettings(ByVal oCustomPropertiesReader As CustomPropertiesReaderWriter)
        fraPlusHandling.Enabled = True
        optErrorIfDup.Enabled = True
        optRenameIfDup.Enabled = True
        optReplaceIfDup.Enabled = True
        optReplaceIfDup.Checked = True

        ' Depending on the previously saved custom properties, 
        ' select the appropriate control settings for the custom filenaming options
        Select Case oCustomPropertiesReader.FileNaming
            Case c_strCPValDecimal
                optDecimalFilename.Checked = True
                If oCustomPropertiesReader.LeadingZeros Then
                    chkLeadZeros.Checked = True
                End If

            Case c_strCPValNone
                optStandardNaming.Checked = True

            Case Else
                optCustomFileName.Checked = True
                'Get the configuration of the old Text Release script
                Dim oFileName As ExportValue = GetExportValueByDestinationValue(m_oSetupData, c_strCPKeyFileNaming)
                If oFileName IsNot Nothing Then
                    'Old configuration
                    Dim oExportValuesArray As New List(Of ExportValue)
                    oExportValuesArray.Add(oFileName)
                    m_oFilenameManager = New ExportValuesManager(c_strMacroKeyDefaultFileName, oExportValuesArray)
                Else
                    'New configuration
                    m_oFilenameManager = New ExportValuesManager(c_strMacroKeyDefaultFileName, m_oSetupData)
                End If
                ctlCustomFileNameInput.SetExportValues(m_oFilenameManager.Values)
        End Select

        ' Fix for SPR 29262 will now save any duplication option
        If oCustomPropertiesReader.DuplicateHandling = c_strCPValVersion Then
            optRenameIfDup.Checked = True
        ElseIf oCustomPropertiesReader.DuplicateHandling = c_strCPValError Then
            optErrorIfDup.Checked = True
        Else
            optReplaceIfDup.Checked = True
        End If

    End Sub

    ''' <summary>
    ''' Filling release setup UI with default settings for the Plus release.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ImageDefaultUISettings()
        '*** Set default control states
        optStandardNaming.Checked = True
        fraPlusHandling.Enabled = True
        optErrorIfDup.Enabled = True
        optRenameIfDup.Enabled = True
        optReplaceIfDup.Enabled = True
        optReplaceIfDup.Checked = True
    End Sub

    ''' <summary>
    ''' Save settings in the ImageName setup.
    ''' </summary>
    ''' <param name="oCustomPropertiesWriter">Custom properties reader and writer instance</param>
    ''' <remarks></remarks>
    Private Sub FileNameSaveSettings(ByVal oCustomPropertiesWriter As CustomPropertiesReaderWriter)
        ' Save the appropriate value(s) for the file naming custom properties
        ' depending on what option button is selected for custom file naming.

        ' set file name to decimal value if decimal is checked.
        ' Also set the leading zeros based upon whether the flag is checked.
        If SetupForm.optDecimalFilename.Checked Then
            oCustomPropertiesWriter.FileNaming = c_strCPValDecimal
            oCustomPropertiesWriter.LeadingZeros = SetupForm.chkLeadZeros.Checked

        ElseIf SetupForm.optStandardNaming.Checked Then
            ' this tells us that the name is in standard format
            oCustomPropertiesWriter.FileNaming = c_strCPValNone
        Else
            ' otherwise we set based on the index name
            oCustomPropertiesWriter.FileNaming = c_strCPValCustom
            m_oFilenameManager.Values = ctlCustomFileNameInput.GetExportValues()
            m_oFilenameManager.saveToSetupData(m_oSetupData)
        End If

        ' always add duplicate handling
        If SetupForm.optRenameIfDup.Checked Then
            oCustomPropertiesWriter.DuplicateHandling = c_strCPValVersion
        ElseIf SetupForm.optErrorIfDup.Checked Then
            oCustomPropertiesWriter.DuplicateHandling = c_strCPValError
        Else
            oCustomPropertiesWriter.DuplicateHandling = c_strCPValReplace
        End If
    End Sub

    ''' <summary>
    ''' Show help.c
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ShowHelp()
        Help.Show(tabText.SelectedIndex + c_nTabs_First_HelpID)
    End Sub

    ''' <summary>
    ''' Finds a Link object in Setup data by destination value and source type.
    ''' Then creates an Export Value object based on the result link object.
    ''' </summary>
    ''' <param name="oSetupData"></param>
    ''' <param name="strDestinationName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetExportValueByDestinationValue(ByRef oSetupData As Kofax.ReleaseLib.ReleaseSetupData, _
                 ByVal strDestinationName As String) _
                 As ExportValue

        '*** Loop through each field and grab the value asked for
        Using eEnumerator As New ComEnumerator(oSetupData.Links.GetEnumerator())
            While eEnumerator.MoveNext()
                Dim oLink As Kofax.ReleaseLib.Link = CType(eEnumerator.Current, Kofax.ReleaseLib.Link)
                Using New ComDisposer(oLink)
                    If oLink.Destination = strDestinationName Then
                        Return New ExportValue(oLink.Source, oLink.SourceType)
                    End If
                End Using
            End While
        End Using

        Return Nothing
    End Function

    ''' <summary>
    ''' This function loops through the list of Index Values for a specified source name and type.
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <param name="IndexType"></param>
    ''' <returns>True if the index value exists</returns>
    ''' <remarks></remarks>
    Private Function IndexValueExists(ByRef strName As String, _
             ByRef IndexType As Kofax.ReleaseLib.KfxLinkSourceType) _
             As Boolean
        ' Search through the list
        For Each oLink As ExportValue In m_olinks
            If oLink.SourceName = strName And _
             (IndexType = Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_UNDEFINED_LINK Or IndexType = oLink.SourceType) Then
                ' We found it
                Return True
            End If
        Next

        Return False
    End Function

    ''' <summary>
    ''' Update the Export Values Manager object for a folder path from an input string.
    ''' </summary>
    ''' <param name="oFolderPathManger"> The referen of Export Values Mangager object</param>
    ''' <param name="strInputText">The input folder path string</param>
    ''' <remarks>This function will parse the input string by split it by (\)</remarks>
    Private Sub UpdateCustomFolderPathFromText(ByRef oFolderPathManger As ExportValuesManager, _
                 ByVal strInputText As String)
        Dim oFolderPathList As New List(Of ExportValue)
        Dim bStartWithDoubleSlash As Boolean = strInputText.StartsWith(c_strDoubleBackSlashCharacter)
        strInputText = strInputText.Trim()
        strInputText = strInputText.Trim(CChar(c_strBackSlashCharacter))
        Dim oSubStringArray As String() = strInputText.Split(CChar(c_strBackSlashCharacter))
        For Each strSubString As String In oSubStringArray
            strSubString = strSubString.Trim()
            If strSubString IsNot String.Empty Or oFolderPathList.Count = 0 Then
                If bStartWithDoubleSlash Then
                    oFolderPathList.Add(New ExportValue(c_strDoubleBackSlashCharacter + strSubString, _
                             KfxLinkSourceType.KFX_REL_TEXTCONSTANT))
                    bStartWithDoubleSlash = False
                Else
                    oFolderPathList.Add(New ExportValue(strSubString, _
                             KfxLinkSourceType.KFX_REL_TEXTCONSTANT))
                End If
            End If
        Next
        oFolderPathManger.Values = oFolderPathList

    End Sub
#End Region

#Region "Common event handlers"
    ''' <summary>
    ''' Handles text changed event for the name text box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtName_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtName.TextChanged
        Dirty = True
    End Sub

    ''' <summary>
    ''' Validate index values data grid view before saving its data.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub tabText_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles tabText.Validating
        If m_olinks.Count > 0 Then

            ' Check for blank Index Values
            For i As Integer = 0 To m_olinks.Count - 1
                If m_olinks(i).SourceType = KfxLinkSourceType.KFX_REL_UNDEFINED_LINK Then
                    ' Display an error message                    

                    grdIndexValuesData.CurrentCell = grdIndexValuesData(1, i)
                    grdIndexValuesData.CurrentCell.Selected = True
                    m_oIndexValuesSelectedRow = grdIndexValuesData.Rows(i)
                    m_oErrorProvider.SetError(grdIndexValuesData, My.Resources.MSG_BLANK_INDEX_VALUES_NOT_ALLOWED)
                    e.Cancel = True
                    Return
                End If
            Next

            ' Check the Index Fields for any not assigned
            Dim strMissingIndex As String = String.Empty
            Dim nMissingIndexCnt As Integer
            Using New ComDisposer(m_oSetupData.IndexFields)
                Using enumerator As New ComEnumerator(m_oSetupData.IndexFields.GetEnumerator())
                    While enumerator.MoveNext()
                        Dim oIndexField As IndexField = CType(enumerator.Current, IndexField)
                        Using New ComDisposer(oIndexField)
                            ' Check only document index field, and skip folder index field.
                            If String.IsNullOrEmpty(oIndexField.FolderClassName) Then
                                If Not IndexValueExists(oIndexField.Name, Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_INDEXFIELD) Then
                                    strMissingIndex = strMissingIndex & AddString(nMissingIndexCnt, oIndexField.Name)
                                End If
                            End If
                        End Using
                    End While
                End Using
            End Using

            ' Simply report the unused Index Fields to the user.
            If Not String.IsNullOrEmpty(strMissingIndex) Then
                m_oErrorProvider.AddInformation(grdIndexValuesData, String.Format(My.Resources.MSG_NOT_ALL_INDEX_FIELDS_USED, strMissingIndex))
            End If

            ' Check the Batch Fields for any not assigned
            strMissingIndex = String.Empty
            nMissingIndexCnt = 0
            Using New ComDisposer(m_oSetupData.BatchFields)
                Using enumerator As New ComEnumerator(m_oSetupData.BatchFields.GetEnumerator())
                    While enumerator.MoveNext()
                        Dim oBatchField As BatchField = CType(enumerator.Current, BatchField)
                        Using New ComDisposer(oBatchField)
                            If Not IndexValueExists(oBatchField.Name, Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_BATCHFIELD) Then
                                strMissingIndex = strMissingIndex & AddString(nMissingIndexCnt, oBatchField.Name)
                            End If
                        End Using
                    End While
                End Using
            End Using

            ' Simply report the unused Batch Fields to the user.
            If Not String.IsNullOrEmpty(strMissingIndex) Then
                m_oErrorProvider.AddInformation(grdIndexValuesData, String.Format(My.Resources.MSG_NOT_ALL_BATCH_FIELDS_USED, strMissingIndex))
            End If
        ElseIf m_oSetupData.IndexFields.Count > 0 Or m_oSetupData.BatchFields.Count > 0 Then
            ' Warn the user that no Index Values were set up but
            ' Index Fields or Batch Fields are defined
            m_oErrorProvider.AddInformation(grdIndexValuesData, My.Resources.MSG_NO_INDEX_VALUES)

        End If
    End Sub

    ''' <summary>
    ''' Handles selected index changed event for the tab text.
    ''' </summary>
    ''' <param name="eventSender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Private Sub tabText_SelectedIndexChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) _
               Handles tabText.SelectedIndexChanged
        Static oPreviousTab As Integer = tabText.SelectedIndex()
        If Not grdIndexValuesData.Focused Then
            grdIndexValuesData.CurrentCell = Nothing
        End If
    End Sub

    ''' <summary>
    ''' Handles click event for the ok button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmdOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOK.Click
        If Not m_bFormSaved Then
            If Not SaveReleaseSettings() Then
                DialogResult = Windows.Forms.DialogResult.None
            End If
        End If
    End Sub

    ''' <summary>
    ''' Verify the settings.  If there are no errors, save the changes and allow the user to continue editing.
    ''' </summary>
    ''' <param name="eventSender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks>If the settings are validated and saved, the data is marked clean.</remarks>
    Private Sub cmdApply_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) _
           Handles cmdApply.Click
        SaveReleaseSettings()
    End Sub

    ''' <summary>
    ''' Handles enabled changed event for the apply button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmdApply_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdApply.EnabledChanged
        If cmdApply.Enabled Then
            m_bFormSaved = False
        End If

    End Sub

    ''' <summary>
    ''' Display the help topic for the tab that is currently displayed.
    ''' </summary>
    ''' <param name="eventSender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks>
    ''' Each tab on the SSTab control has a unique Help Context ID.  We add the tab index to the first ID to display the appropriate help info.
    ''' If additional tabs are added to the SSTab control, their Help Context IDs must be kept sequential. 
    ''' </remarks>
    Private Sub cmdHelp_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) _
         Handles cmdHelp.Click
        ShowHelp()
    End Sub

    ''' <summary>
    ''' Handles the key down event to switch between tab pages.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Switches tab page when the cursor outside the tab control and user press Ctrl + Tab keys.</remarks>
    Private Sub MainSetupForm_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Dim shift As Short = CShort(e.KeyData \ &H10000)
        Dim oForm As Form = CType(sender, Form)
        If Not (TypeOf oForm.ActiveControl Is TabControl) Then
            Dim nTabCount As Integer = tabText.TabCount
            Dim nextTab As Integer = 0
            If e.KeyCode = Keys.Tab Then
                If shift = ShiftConstants.CtrlMask Then
                    ' Calculates the next selected tab. Roll to the first tab if the current tab reaches the total tab page count.
                    nextTab = tabText.SelectedIndex() + 1
                    If (nextTab = nTabCount) Then
                        nextTab = 0
                    End If
                ElseIf shift = ShiftConstants.CtrlMask + ShiftConstants.ShiftMask Then
                    nextTab = tabText.SelectedIndex() - 1
                    If (nextTab < 0) Then
                        ' Wraps to the last tab if the first tab is reached and the user press Ctrl + Shift + Tab
                        nextTab = nTabCount - 1
                    End If
                End If
                tabText.SelectTab(nextTab)
            End If
        End If
    End Sub
#End Region

#Region "Event handlers for Default Storage Tab"
    ''' <summary>
    ''' Handles click event for the custom button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmdCustom_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCustom.Click
        Dim oCustomFolderDialog As New CustomFolderDialog(c_strShortProductName, _
                      My.Resources.TXT_CUSTOM_FOLDER_TITLE_DEFAULT_STORAGE, _
                      m_oDefaultFolderManager, m_oExportValuesMenu)
        If Windows.Forms.DialogResult.OK = oCustomFolderDialog.ShowDialog() Then
            Me.txtDefaultStorageFolder.Text = m_oDefaultFolderManager.BuildFolderDisplayedPath()
            Dirty = True
        End If

    End Sub

    ''' <summary>
    ''' Handles checked changed event for the custom file name option.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub optCustomFileName_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optCustomFileName.CheckedChanged
        Dirty = True
        If optCustomFileName.Checked Then
            chkLeadZeros.Checked = False
            chkLeadZeros.Enabled = False
            optCustomFileName.Enabled = True
            lblFileName.Enabled = True
            ctlCustomFileNameInput.Enabled = True
            ctlCustomFileNameInput.SetExportValues(m_oFilenameManager.Values)
        End If
    End Sub

    ''' <summary>
    ''' Handles checked changed event for the standard naming option.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub optStandardNaming_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optStandardNaming.CheckedChanged
        Dirty = True
        If optStandardNaming.Checked Then
            ctlCustomFileNameInput.Enabled = False
            lblFileName.Enabled = False
            chkLeadZeros.Checked = False
            chkLeadZeros.Enabled = False
        End If
    End Sub

    ''' <summary>
    ''' Handles checked changed event for the decimal file name option.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub optDecimalFilename_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optDecimalFilename.CheckedChanged
        Dirty = True
        If optDecimalFilename.Checked Then
            ctlCustomFileNameInput.Enabled = False
            lblFileName.Enabled = False
            chkLeadZeros.Enabled = True
        End If
    End Sub

    ''' <summary>
    ''' Handles check state changed event for the leading zeros check box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub chkLeadZeros_CheckStateChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkLeadZeros.CheckStateChanged
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles checked changed event for the replace image option.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub optReplaceIfDup_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optReplaceIfDup.CheckedChanged
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles checked changed event for the version if duplicate option.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub optRenameIfDup_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optRenameIfDup.CheckedChanged
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles checked changed event for the error if duplicate option.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub optErrorIfDup_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optErrorIfDup.CheckedChanged
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles file name format changed event for the custom file name input control.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ctlCustomFileNameInput_FileNameFormatChanged() Handles ctlCustomFileNameInput.FileNameFormatChanged
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles text changed event for the default storage folder textbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtDefaultStorageFolder_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtDefaultStorageFolder.TextChanged
        Dirty = True
        If Not txtDefaultStorageFolder.Text.Equals(m_oDefaultFolderManager.BuildFolderDisplayedPath()) Then
            UpdateCustomFolderPathFromText(m_oDefaultFolderManager, txtDefaultStorageFolder.Text)
        End If
    End Sub
    ''' <summary>
    ''' Handles validating event for the default storage folder textbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtDefaultStorageFolder_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtDefaultStorageFolder.Validating
        If (m_oErrorProvider Is Nothing) Or (txtDefaultStorageFolder.Enabled = False) Then
            Return
        End If
        txtDefaultStorageFolder.Text = txtDefaultStorageFolder.Text.Trim()
        ' Default storage folder should not be empty
        If String.IsNullOrEmpty(txtDefaultStorageFolder.Text) Then
            m_oErrorProvider.SetError(txtDefaultStorageFolder, My.Resources.MSG_DEFAULT_STORAGE_FOLDER_REQUIRED)
            e.Cancel = True
            Exit Sub
        End If

        ' Folder path should not contain special characters
        Dim strSpecialChar As String
        For Each strSpecialChar In cm_strFolderPathSpecialCharacters
            If txtDefaultStorageFolder.Text.IndexOf(strSpecialChar) >= 0 Then
                m_oErrorProvider.SetError(txtDefaultStorageFolder, My.Resources.ERR_FOLDER_PATH_CONTAINS_SPECIAL_CHARS)
                e.Cancel = True
                Exit Sub
            End If
        Next

        ' Each folder path should have less than 254 characters
        If txtDefaultStorageFolder.Text.Length > 254 Then
            m_oErrorProvider.SetError(txtDefaultStorageFolder, My.Resources.ERR_FOLDER_PATH_MAX_LENGTH)
            e.Cancel = True
            Exit Sub
        End If
    End Sub
#End Region

#Region "Event handlers for Index Storage Tab"
    ''' <summary>
    ''' Handles checked changed event for the default storage output check box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub chkDefaultStorageOutput_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                        Handles chkDefaultStorageOutput.CheckedChanged
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles checked changed event for the custom file output check box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub chkCustomFileOutput_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                    Handles chkCustomFileOutput.CheckedChanged
        Dim bEnabled As Boolean = chkCustomFileOutput.Checked
        txtCustomIndexFolder.Enabled = bEnabled
        If m_oIndexOutputManager IsNot Nothing Then
            txtCustomIndexFolder.Text = m_oIndexOutputManager.BuildFolderDisplayedPath()
        End If
        lblStorageFolder.Enabled = bEnabled
        lblStorageFileName.Enabled = bEnabled
        txtCustomIndexFileName.Enabled = bEnabled
        cmdCustomFileOutputBrowse.Enabled = bEnabled
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles click event for the custom file output browse.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmdCustomFileOutputBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                Handles cmdCustomFileOutputBrowse.Click
        Dim oCustomFolderDialog As New CustomFolderDialog(c_strShortProductName, _
                      My.Resources.TXT_CUSTOM_FOLDER_TITLE_INDEX_STORAGE, _
                      m_oIndexOutputManager, m_oExportValuesMenu)
        If Windows.Forms.DialogResult.OK = oCustomFolderDialog.ShowDialog() Then
            Me.txtCustomIndexFolder.Text = m_oIndexOutputManager.BuildFolderDisplayedPath()
            Dirty = True
        End If
    End Sub

    ''' <summary>
    ''' Handles text changed event for the custom index file name textbox.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtCustomIndexFileName_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                    Handles txtCustomIndexFileName.TextChanged
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles leave event for the custom index file name textbox.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtCustomIndexFileName_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                Handles txtCustomIndexFileName.Leave
        txtCustomIndexFileName.Text = txtCustomIndexFileName.Text.Trim()
    End Sub

    ''' <summary>
    ''' Handles validating event for the custom index file name textbox.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtCustomIndexFileName_Validating(ByVal sender As System.Object, _
                                                    ByVal e As System.ComponentModel.CancelEventArgs) _
                                                    Handles txtCustomIndexFileName.Validating
        If (m_oErrorProvider Is Nothing) Or (txtCustomIndexFileName.Enabled = False) Then
            Return
        End If
        ' Char 34 is "
        Dim strSpecialCharacters() As String = New String() {"/", "\", ":", "*", "?", Chr(34).ToString(), "<", ">", "|"}

        txtCustomIndexFileName.Text = txtCustomIndexFileName.Text.Trim()
        ' File name format should not be empty
        If String.IsNullOrEmpty(txtCustomIndexFileName.Text) Then
            m_oErrorProvider.SetError(txtCustomIndexFileName, My.Resources.MSG_FILE_NAME_REQUIRED)
            e.Cancel = True
            Exit Sub
        End If

        ' File name format should not contain special characters
        Dim strSpecialChar As String
        For Each strSpecialChar In strSpecialCharacters
            If txtCustomIndexFileName.Text.IndexOf(strSpecialChar) >= 0 Then
                m_oErrorProvider.SetError(txtCustomIndexFileName, My.Resources.ERR_FILE_NAME_CONTAINS_SPECIAL_CHARS)
                e.Cancel = True
                Exit Sub
            End If
        Next

        ' Each constant text in the file name format should have less than 128 characters
        If txtCustomIndexFileName.Text.Length > 128 Then
            m_oErrorProvider.SetError(txtCustomIndexFileName, My.Resources.ERR_CONSTANT_TEXT_MAX_LENGTH)
            e.Cancel = True
            Exit Sub
        End If
    End Sub

    ''' <summary>
    ''' Handles selected index changed event for the encoding combo box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cboEncoding_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                    Handles cboEncoding.SelectedIndexChanged
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles user deleted row event for the index value data grid view.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub grdIndexValuesData_UserDeletedRow(ByVal sender As System.Object, _
                                                        ByVal e As System.Windows.Forms.DataGridViewRowEventArgs) _
                                                        Handles grdIndexValuesData.UserDeletedRow
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles user added row event for the index value data grid view.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub grdIndexValuesData_UserAddedRow(ByVal sender As System.Object, _
                                                        ByVal e As System.Windows.Forms.DataGridViewRowEventArgs) _
                                                        Handles grdIndexValuesData.UserAddedRow
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles cell value changed event for the index value data grid view.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub grdIndexValuesData_CellValueChanged(ByVal sender As System.Object, _
                                                    ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
                                                    Handles grdIndexValuesData.CellValueChanged
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles the event when leave index values grid cell
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Perform a validation</remarks>
    Private Sub grdIndexValuesData_CellLeave(ByVal sender As System.Object, _
                                                ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
                                                Handles grdIndexValuesData.CellLeave
        grdIndexValuesData.PerformValidation()
    End Sub

    ''' <summary>
    ''' Handles current cell dirty state changed event for the index values data grid view.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub grdIndexValuesData_CurrentCellDirtyStateChanged(ByVal sender As System.Object, _
                                                                ByVal e As System.EventArgs) _
                                                                Handles grdIndexValuesData.CurrentCellDirtyStateChanged
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles cell enter event for the index values data grid view.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub grdIndexValuesData_CellEnter(ByVal sender As System.Object, _
                                                    ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
                                                    Handles grdIndexValuesData.CellEnter
        ' BeginInvoke is used to move to current cell to the value column
        ' rather than directly setting the CurrentCell property because 
        ' the CellEnter event would be raised recursively
        If grdIndexValuesData.IsHandleCreated And 1 <> e.ColumnIndex Then
            BeginInvoke(CType(AddressOf MoveToIndexValueColumn, MethodInvoker))
        End If
    End Sub

    ''' <summary>
    ''' Handles selection changed event for the index values data grid view.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub grdIndexValuesData_SelectionChanged(ByVal sender As System.Object, _
                                                       ByVal e As System.EventArgs) _
                                                       Handles grdIndexValuesData.SelectionChanged
        Dim bEnabled As Boolean = grdIndexValuesData.SelectedRows.Count > 0

        cmdDeleteIndex.Enabled = bEnabled
        cmdDeleteAllIndex.Enabled = grdIndexValuesData.RowCount > 0
        ctlIndexValueUpDown.Enabled = (grdIndexValuesData.RowCount > 1) And bEnabled
        lblMove.Enabled = ctlIndexValueUpDown.Enabled
    End Sub

    ''' <summary>
    ''' Handles item move up event for the index values data grid view.
    ''' </summary>
    ''' <param name="eventSender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Private Sub grdIndexValuesData_ItemMoveUp(ByVal eventSender As System.Object, _
                                                ByVal eventArgs As System.EventArgs) _
                                                Handles grdIndexValuesData.IndexValueDataGridItemMoveUp
        MoveIndex(ArrowDirection.Up)
    End Sub

    ''' <summary>
    ''' Handles item move down event for the index values data grid view.
    ''' </summary>
    ''' <param name="eventSender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Private Sub grdIndexValuesData_ItemMoveDown(ByVal eventSender As System.Object, _
                                                ByVal eventArgs As System.EventArgs) _
                                                Handles grdIndexValuesData.IndexValueDataGridItemMoveDown
        MoveIndex(ArrowDirection.Down)
    End Sub

    ''' <summary>
    ''' Handles item enter key event for the index values data grid view.
    ''' </summary>
    ''' <param name="eventSender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Private Sub grdIndexValuesData_ItemEnterKey(ByVal eventSender As System.Object, _
                                                ByVal eventArgs As System.EventArgs) _
                                                Handles grdIndexValuesData.IndexValueDataGridItemEnterKey
        cmdOK.PerformClick()
        grdIndexValuesData.Focus()
    End Sub

    ''' <summary>
    ''' Handles enter event for the index values frame.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub fraIndexVals_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles fraIndexVals.Enter
        '*** select the row that was last selected before leaving the frame.
        If m_oIndexValuesSelectedRow IsNot Nothing Then
            If Not m_oIndexValuesSelectedRow.Selected Then
                m_oIndexValuesSelectedRow.Selected = True
            End If
        End If
    End Sub

    ''' <summary>
    ''' Handles leave event for the index values frame.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub fraIndexVals_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles fraIndexVals.Leave
        m_oIndexValuesSelectedRow = grdIndexValuesData.CurrentRow
        grdIndexValuesData.ClearSelection()
    End Sub

    ''' <summary>
    ''' Add a blank (unlinked) Index Value to the end of the list and place focus on the control. Mark the data dirty.
    ''' </summary>
    ''' <param name="eventSender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Private Sub cmdAddIndex_Click(ByVal eventSender As System.Object, _
                                    ByVal eventArgs As System.EventArgs) _
                                    Handles cmdAddIndex.Click
        Me.Dirty = True

        ' Add a blank Index Value at the end of the list
        m_olinks.Add(New ExportValue(Nothing, KfxLinkSourceType.KFX_REL_UNDEFINED_LINK))
        grdIndexValuesData.CurrentCell = grdIndexValuesData(1, m_olinks.Count - 1)
        grdIndexValuesData.BeginEdit(False)
        If Not grdIndexValuesData.Focused Then
            grdIndexValuesData.Focus()
        End If
        grdIndexValuesData.CurrentCell.Selected = True
    End Sub

    ''' <summary>
    ''' Ask the user if it is OK and then delete the selected Index Value. Mark the data dirty if the Index Value is deleted.
    ''' </summary>
    ''' <param name="eventSender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks>VB behavior causes this function to get called recursively so we set a local InProgress flag to skip it.</remarks>
    Private Sub cmdDeleteIndex_Click(ByVal eventSender As System.Object, _
                                        ByVal eventArgs As System.EventArgs) _
                                        Handles cmdDeleteIndex.Click
        ' Verify that the user REALLY wants
        ' to delete the selected Index Value
        If Windows.Forms.DialogResult.Yes = MessageBox.Show( _
         My.Resources.MSG_DELETE_VALUE, _
         c_strShortProductName, _
         MessageBoxButtons.YesNo, _
         MessageBoxIcon.Exclamation) Then

            ' Go ahead and delete it
            Dim nCurrentRow As Integer = grdIndexValuesData.CurrentRow.Index
            grdIndexValuesData.Rows.RemoveAt(nCurrentRow)
            Dim nCount As Integer = m_olinks.Count
            If nCurrentRow > nCount - 1 Then
                nCurrentRow = nCount - 1
            End If

            If nCurrentRow >= 0 Then
                grdIndexValuesData.CurrentCell = grdIndexValuesData(1, nCurrentRow)
            End If
            If Not grdIndexValuesData.Focused Then
                grdIndexValuesData.Focus()
            End If
            If grdIndexValuesData.CurrentCell IsNot Nothing Then
                grdIndexValuesData.CurrentCell.Selected = True
            End If
            Dirty = True
        End If
    End Sub

    ''' <summary>
    ''' Ask the user if it is OK and then delete all defined Index Values. Mark the data dirty if the Index Values are deleted.
    ''' </summary>
    ''' <param name="eventSender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Private Sub cmdDeleteAllIndex_Click(ByVal eventSender As System.Object, _
                                        ByVal eventArgs As System.EventArgs) _
                                        Handles cmdDeleteAllIndex.Click
        ' Verify that the user REALLY
        ' wants to delete all Index Values
        If Windows.Forms.DialogResult.Yes = MessageBox.Show( _
         My.Resources.MSG_DELETE_ALL_VALUES, _
         c_strShortProductName, _
         MessageBoxButtons.YesNo, _
         MessageBoxIcon.Exclamation) Then

            ' Go ahead and delete them
            DeleteAllIndex()
            cmdDeleteAllIndex.Enabled = False
            If Not grdIndexValuesData.Focused Then
                grdIndexValuesData.Focus()
            End If
            Me.Dirty = True
        End If
    End Sub

    ''' <summary>
    ''' Handles up event for the index value up down control.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ctlIndexValueUpDown_Up(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                    Handles ctlIndexValueUpDown.Up
        MoveIndex(ArrowDirection.Up)
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles down event for the index value up down control.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ctlIndexValueUpDown_Down(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                        Handles ctlIndexValueUpDown.Down
        MoveIndex(ArrowDirection.Down)
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles text changed event for the index storage folder textbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtCustomIndexFolder_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCustomIndexFolder.TextChanged
        Dirty = True
        If Not txtCustomIndexFolder.Text.Equals(m_oIndexOutputManager.BuildFolderDisplayedPath()) Then
            UpdateCustomFolderPathFromText(m_oIndexOutputManager, txtCustomIndexFolder.Text)
        End If
    End Sub

    ''' <summary>
    ''' Handles validating event for the index storage folder textbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtCustomIndexFolder_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtCustomIndexFolder.Validating
        If (m_oErrorProvider Is Nothing) Or (txtCustomIndexFolder.Enabled = False) Then
            Return
        End If
        txtCustomIndexFolder.Text = txtCustomIndexFolder.Text.Trim()
        ' Default storage folder should not be empty
        If String.IsNullOrEmpty(txtCustomIndexFolder.Text) Then
            m_oErrorProvider.SetError(txtCustomIndexFolder, My.Resources.MSG_INDEX_STORAGE_FOLDER_REQUIRED)
            e.Cancel = True
            Exit Sub
        End If
        ' Folder path should not contain special characters
        Dim strSpecialChar As String
        For Each strSpecialChar In cm_strFolderPathSpecialCharacters
            If txtCustomIndexFolder.Text.IndexOf(strSpecialChar) >= 0 Then
                m_oErrorProvider.SetError(txtCustomIndexFolder, My.Resources.ERR_FOLDER_PATH_CONTAINS_SPECIAL_CHARS)
                e.Cancel = True
                Exit Sub
            End If
        Next
        ' Each folder path should have less than 254 characters
        If txtCustomIndexFolder.Text.Length > 254 Then
            m_oErrorProvider.SetError(txtCustomIndexFolder, My.Resources.ERR_FOLDER_PATH_MAX_LENGTH)
            e.Cancel = True
            Exit Sub
        End If
    End Sub
#End Region

#Region "Event handlers for Advanced Tab"
    ''' <summary>
    ''' Handles checked changed event for the ocr output check box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub chkOcrOutput_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                            Handles chkOcrOutput.CheckedChanged
        Dirty = True
        OrcOutputUIUpdate()
    End Sub

    ''' <summary>
    ''' Handles checked changed event for the ocr default path option.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub optOcrDefaultPath_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optOcrDefaultPath.CheckedChanged
        Dirty = True
        If optOcrDefaultPath.Checked Then
            cmdOcrCustom.Enabled = False
            txtOcrCustomPath.Enabled = False
            txtOcrCustomPath.Text = String.Empty
        End If
    End Sub

    ''' <summary>
    ''' Handles checked changed event for the ocr custom path option.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub optOcrCustomPath_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                Handles optOcrCustomPath.CheckedChanged
        Dirty = True
        If optOcrCustomPath.Checked Then
            cmdOcrCustom.Enabled = True
            txtOcrCustomPath.Enabled = True
            Me.txtOcrCustomPath.Text = m_oOCROutputManager.BuildFolderDisplayedPath()
        End If
    End Sub

    ''' <summary>
    ''' Handles click event for the ocr custom button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnOcrCustom_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                    Handles cmdOcrCustom.Click
        Dim oCustomFolderDialog As New CustomFolderDialog(c_strShortProductName, _
                      My.Resources.TXT_CUSTOM_FOLDER_TITLE_OCR_STORAGE, _
                      m_oOCROutputManager, m_oExportValuesMenu)
        If Windows.Forms.DialogResult.OK = oCustomFolderDialog.ShowDialog() Then
            Me.txtOcrCustomPath.Text = m_oOCROutputManager.BuildFolderDisplayedPath()
            Dirty = True
        End If
    End Sub

    ''' <summary>
    ''' The user selected a different image format.  Mark the data dirty.  If they chose PDF, enable the controls to define the PDF settings.
    ''' </summary>
    ''' <param name="eventSender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Private Sub cboImageType_SelectedIndexChanged(ByVal eventSender As System.Object, _
                                                    ByVal eventArgs As System.EventArgs) _
                                                    Handles cboImageType.SelectedIndexChanged
        Dim oImageType As ImageTypeComboBoxItem = CType(cboImageType.SelectedItem, ImageTypeComboBoxItem)
        If oImageType.Type <> m_oSetupData.ImageType Then
            Dirty = True
            m_oSetupData.ImageType = oImageType.Type
        End If

    End Sub

    ''' <summary>
    ''' Handles checked changed event for the pdf output check box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub chkPdfOutput_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                            Handles chkPdfOutput.CheckedChanged
        Dirty = True
        PdfOutputUIUpdate()
    End Sub

    ''' <summary>
    ''' Handles checked changed event for the pdf default path option.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub optPdfDefaultPath_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                    Handles optPdfDefaultPath.CheckedChanged
        Dirty = True
        If optPdfDefaultPath.Checked Then
            cmdPdfCustom.Enabled = False
            txtPdfCustomPath.Enabled = False
            txtPdfCustomPath.Text = String.Empty
        End If
    End Sub

    ''' <summary>
    ''' Handles checked changed event for the pdf custom path option.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub optPdfCustomPath_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                Handles optPdfCustomPath.CheckedChanged
        Dirty = True
        If optPdfCustomPath.Checked Then
            cmdPdfCustom.Enabled = True
            txtPdfCustomPath.Enabled = True
            txtPdfCustomPath.Text = m_oPDFOutputManager.BuildFolderDisplayedPath()
        End If
    End Sub

    ''' <summary>
    ''' Handles click event for the pdf custom button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnPdfCustom_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                    Handles cmdPdfCustom.Click
        Dim oCustomFolderDialog As New CustomFolderDialog(c_strShortProductName, _
                      My.Resources.TXT_CUSTOM_FOLDER_TITLE_PDF_STORAGE, _
                      m_oPDFOutputManager, m_oExportValuesMenu)
        If Windows.Forms.DialogResult.OK = oCustomFolderDialog.ShowDialog() Then
            txtPdfCustomPath.Text = m_oPDFOutputManager.BuildFolderDisplayedPath()
            Dirty = True
        End If
    End Sub

    ''' <summary>
    ''' Handles checked changed event for the image output check box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub chkImgOutput_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                            Handles chkImgOutput.CheckedChanged
        Dirty = True
        If chkImgOutput.Checked = True Then
            optImgDefaultPath.Enabled = True
            optImgCustomPath.Enabled = True
            lblReleaseImageAs.Enabled = True
            cboImageType.Enabled = True
            chkSkipFirstPage.Enabled = True
            chkSuppressIfPdfDetected.Enabled = True

            If optImgCustomPath.Checked Then
                cmdImgCustom.Enabled = True
                txtImgCustomPath.Enabled = True
                txtImgCustomPath.Text = m_oImageOutputManager.BuildFolderDisplayedPath()
            End If
        Else
            optImgDefaultPath.Enabled = False
            optImgCustomPath.Enabled = False
            cmdImgCustom.Enabled = False
            lblReleaseImageAs.Enabled = False
            cboImageType.Enabled = False
            chkSkipFirstPage.Enabled = False
            chkSuppressIfPdfDetected.Enabled = False
            txtImgCustomPath.Enabled = False
            txtImgCustomPath.Text = String.Empty
        End If
    End Sub

    ''' <summary>
    ''' Handles checked changed event for the image default path option.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub optImgDefaultPath_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                   Handles optImgDefaultPath.CheckedChanged
        Dirty = True
        If optImgDefaultPath.Checked Then
            cmdImgCustom.Enabled = False
            txtImgCustomPath.Enabled = False
            txtImgCustomPath.Text = String.Empty
        End If
    End Sub

    ''' <summary>
    ''' Handles checked changed event for the custom path option.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub optImgCustomPath_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                Handles optImgCustomPath.CheckedChanged
        Dirty = True
        If optImgCustomPath.Checked Then
            cmdImgCustom.Enabled = True
            txtImgCustomPath.Enabled = True
            Me.txtImgCustomPath.Text = m_oImageOutputManager.BuildFolderDisplayedPath()
        End If
    End Sub

    ''' <summary>
    ''' Handles click event for the image custom button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnImgCustom_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                    Handles cmdImgCustom.Click
        Dim oCustomFolderDialog As New CustomFolderDialog(c_strShortProductName, _
                      My.Resources.TXT_CUSTOM_FOLDER_TITLE_IMAGE_STORAGE, _
                      m_oImageOutputManager, m_oExportValuesMenu)
        If Windows.Forms.DialogResult.OK = oCustomFolderDialog.ShowDialog() Then
            Me.txtImgCustomPath.Text = m_oImageOutputManager.BuildFolderDisplayedPath()
            Dirty = True
        End If
    End Sub

    ''' <summary>
    ''' Handles selected checked changed event for the suppress if pdf detected check box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub chkSuppressIfPdfDetected_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                        Handles chkSuppressIfPdfDetected.CheckedChanged
        Dirty = True
    End Sub

    ''' <summary>
    ''' Handles selected checked changed event for the skip first page check box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub chkSkipFirstPage_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                Handles chkSkipFirstPage.CheckedChanged
        Dirty = True
    End Sub
    ''' <summary>
    ''' Handles text changed event for the OCR storage folder textbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtOcrCustomPath_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtOcrCustomPath.TextChanged
        Dirty = True
        If optOcrCustomPath.Checked And Not txtOcrCustomPath.Text.Equals(m_oOCROutputManager.BuildFolderDisplayedPath()) Then
            UpdateCustomFolderPathFromText(m_oOCROutputManager, txtOcrCustomPath.Text)
        End If
    End Sub

    ''' <summary>
    ''' Handles validating event for the OCR storage folder textbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtOcrCustomPath_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtOcrCustomPath.Validating
        If (m_oErrorProvider Is Nothing) Or (txtOcrCustomPath.Enabled = False) Then
            Return
        End If
        txtOcrCustomPath.Text = txtOcrCustomPath.Text.Trim()
        ' OCR storage folder should not be empty
        If String.IsNullOrEmpty(txtOcrCustomPath.Text) Then
            m_oErrorProvider.SetError(txtOcrCustomPath, My.Resources.MSG_OCR_STORAGE_FOFLDER_REQUIRED)
            e.Cancel = True
            Exit Sub
        End If
        ' Folder path should not contain special characters
        Dim strSpecialChar As String
        For Each strSpecialChar In cm_strFolderPathSpecialCharacters
            If txtOcrCustomPath.Text.IndexOf(strSpecialChar) >= 0 Then
                m_oErrorProvider.SetError(txtOcrCustomPath, My.Resources.ERR_FOLDER_PATH_CONTAINS_SPECIAL_CHARS)
                e.Cancel = True
                Exit Sub
            End If
        Next
        ' Each folder path should have less than 254 characters
        If txtOcrCustomPath.Text.Length > 254 Then
            m_oErrorProvider.SetError(txtOcrCustomPath, My.Resources.ERR_FOLDER_PATH_MAX_LENGTH)
            e.Cancel = True
            Exit Sub
        End If
    End Sub
    ''' <summary>
    ''' Handles text changed event for the PDF storage folder textbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtPdfCustomPath_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtPdfCustomPath.TextChanged
        Dirty = True
        If optPdfCustomPath.Checked And Not txtPdfCustomPath.Text.Equals(m_oPDFOutputManager.BuildFolderDisplayedPath()) Then
            UpdateCustomFolderPathFromText(m_oPDFOutputManager, txtPdfCustomPath.Text)
        End If
    End Sub

    ''' <summary>
    ''' Handles validating event for the PDF storage folder textbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtPdfCustomPath_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtPdfCustomPath.Validating
        If (m_oErrorProvider Is Nothing) Or (txtPdfCustomPath.Enabled = False) Then
            Return
        End If
        txtPdfCustomPath.Text = txtPdfCustomPath.Text.Trim()
        ' PDF storage folder should not be empty
        If String.IsNullOrEmpty(txtPdfCustomPath.Text) Then
            m_oErrorProvider.SetError(txtPdfCustomPath, My.Resources.MSG_PDF_STORAGE_FOFLDER_REQUIRED)
            e.Cancel = True
            Exit Sub
        End If
        ' Folder path should not contain special characters
        Dim strSpecialChar As String
        For Each strSpecialChar In cm_strFolderPathSpecialCharacters
            If txtPdfCustomPath.Text.IndexOf(strSpecialChar) >= 0 Then
                m_oErrorProvider.SetError(txtPdfCustomPath, My.Resources.ERR_FOLDER_PATH_CONTAINS_SPECIAL_CHARS)
                e.Cancel = True
                Exit Sub
            End If
        Next
        ' Each folder path should have less than 254 characters
        If txtPdfCustomPath.Text.Length > 254 Then
            m_oErrorProvider.SetError(txtPdfCustomPath, My.Resources.ERR_FOLDER_PATH_MAX_LENGTH)
            e.Cancel = True
            Exit Sub
        End If
    End Sub

    ''' <summary>
    ''' Handles text changed event for the image storage folder textbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtImgCustomPath_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtImgCustomPath.TextChanged
        Dirty = True
        If optImgCustomPath.Checked And Not txtImgCustomPath.Text.Equals(m_oImageOutputManager.BuildFolderDisplayedPath()) Then
            UpdateCustomFolderPathFromText(m_oImageOutputManager, txtImgCustomPath.Text)
        End If
    End Sub

    ''' <summary>
    ''' Handles validating event for the image storage folder textbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtImgCustomPath_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtImgCustomPath.Validating
        If (m_oErrorProvider Is Nothing) Or (txtImgCustomPath.Enabled = False) Then
            Return
        End If
        txtImgCustomPath.Text = txtImgCustomPath.Text.Trim()
        ' Image storage folder should not be empty
        If String.IsNullOrEmpty(txtImgCustomPath.Text) Then
            m_oErrorProvider.SetError(txtImgCustomPath, My.Resources.MSG_IMAGE_STORAGE_FOFLDER_REQUIRED)
            e.Cancel = True
            Exit Sub
        End If
        ' Folder path should not contain special characters
        Dim strSpecialChar As String
        For Each strSpecialChar In cm_strFolderPathSpecialCharacters
            If txtImgCustomPath.Text.IndexOf(strSpecialChar) >= 0 Then
                m_oErrorProvider.SetError(txtImgCustomPath, My.Resources.ERR_FOLDER_PATH_CONTAINS_SPECIAL_CHARS)
                e.Cancel = True
                Exit Sub
            End If
        Next
        ' Each folder path should have less than 254 characters
        If txtImgCustomPath.Text.Length > 254 Then
            m_oErrorProvider.SetError(txtImgCustomPath, My.Resources.ERR_FOLDER_PATH_MAX_LENGTH)
            e.Cancel = True
            Exit Sub
        End If
    End Sub

#End Region
#Region "By the Kofax Export Connector Text 10.2 PAD"

    '*************************************************
    ' UpdateUISupportedOcrAndPdf
    '-------------------------------------------------
    ' Purpose:  Update states of OCR's controls and PDF's controls 
    ' Inputs:   None
    ' Outputs:  None
    '*************************************************
    Private Sub UpdateUISupportedOcrAndPdf()
        If Not m_bSupportOCR Then
            ' Disable OCR's controls
            chkOcrOutput.Checked = False
            chkOcrOutput.Enabled = False
            pnlOCROutput.Enabled = False
        End If

        If Not m_bSupportPDF Then
            ' Disable PDF's controls
            chkPdfOutput.Checked = False
            chkPdfOutput.Enabled = False
            pnlPDFOutput.Enabled = False
        End If

        OrcOutputUIUpdate()
        PdfOutputUIUpdate()
    End Sub

    Private Sub OrcOutputUIUpdate()
        If chkOcrOutput.Checked = True Then
            optOcrDefaultPath.Enabled = True
            optOcrCustomPath.Enabled = True
            If optOcrCustomPath.Checked Then
                cmdOcrCustom.Enabled = True
                txtOcrCustomPath.Enabled = True
                txtOcrCustomPath.Text = m_oOCROutputManager.BuildFolderDisplayedPath()
            End If
        Else
            optOcrDefaultPath.Enabled = False
            optOcrCustomPath.Enabled = False
            cmdOcrCustom.Enabled = False
            txtOcrCustomPath.Enabled = False
            txtOcrCustomPath.Text = String.Empty
        End If
    End Sub

    Private Sub PdfOutputUIUpdate()
        If chkPdfOutput.Checked = True Then
            optPdfDefaultPath.Enabled = True
            optPdfCustomPath.Enabled = True
            If optPdfCustomPath.Checked Then
                cmdPdfCustom.Enabled = True
                txtPdfCustomPath.Enabled = True
                txtPdfCustomPath.Text = m_oPDFOutputManager.BuildFolderDisplayedPath()
            End If
        Else
            optPdfDefaultPath.Enabled = False
            optPdfCustomPath.Enabled = False
            cmdPdfCustom.Enabled = False
            txtPdfCustomPath.Enabled = False
            txtPdfCustomPath.Text = String.Empty
        End If
    End Sub

    '*************************************************
    ' InitSupportOCRandPDF
    '-------------------------------------------------
    ' Purpose:  Check the conditions to enable or disable OCR and PDF settings
    ' The OCR and PDF settings are disabled unless the batch class has these queues configured.
    ' For example, for OCR, the OCR module must be added to the queue and the document class must have OCR enabled.
    ' They may also be enabled if a custom module replaces OCR or PDF.  This is an option set in the .inf for registering customer modules.
    ' Inputs:   
    ' Outputs:  None
    '*************************************************
    Private Sub InitSupportOCRandPDF()

        Dim bDocClassOcrEnabled As Boolean = False
        Dim bDocClassPdfEnabled As Boolean = False
        Dim bHasContainsOcrQueue As Boolean = False
        Dim bHasContainsPdfQueue As Boolean = False

        Dim oAssembly As Assembly
        Dim oTypeAbsLogin As Type
        Dim oTypeAdminSession As Type
        Dim oTypeAbsDocClss As Type

        Dim oAbsLogin As Object
        Dim oAdminSession As Object
        Dim oAbsDocClss As Object

        oAssembly = Assembly.LoadFrom(Application.StartupPath + "\Kofax.AbsDB.Interop.dll")
        oTypeAbsLogin = oAssembly.GetType("Kofax.AbsDB.CAbsLoginClass")
        oTypeAdminSession = oAssembly.GetType("Kofax.AbsDB.AbsAdminSession")
        oTypeAbsDocClss = oAssembly.GetType("Kofax.AbsDB.AbsDocumentDef")
        oAbsLogin = Activator.CreateInstance(oTypeAbsLogin)

        Using New ComDisposer(oAbsLogin)

            ' Get AbsAdminSession
            Dim method_login As MethodInfo = oTypeAbsLogin.GetMethod("Login")
            method_login.Invoke(oAbsLogin, New Object() {"", ""})

            Dim method_OpenSession As MethodInfo = oTypeAbsLogin.GetMethod("OpenAdminSession")
            oAdminSession = method_OpenSession.Invoke(oAbsLogin, New Object() {0})

            ' Control not using KC
            If oAdminSession Is Nothing Then
                m_bSupportOCR = True
                m_bSupportPDF = True
            Else
                Using New ComDisposer(oAdminSession)

                    Dim method_GetDocumentDef As MethodInfo = oTypeAdminSession.GetMethod("GetDocumentDef")
                    oAbsDocClss = method_GetDocumentDef.Invoke(oAdminSession, New Object() {m_oSetupData.DocClassID})

                    Using New ComDisposer(oAbsDocClss)

                        Dim property_OCR As PropertyInfo = oTypeAbsDocClss.GetProperty("OcrEnable")
                        bDocClassOcrEnabled = Convert.ToBoolean(property_OCR.GetValue(oAbsDocClss, Nothing))

                        ' Check Document's state OCR|PDF enable
                        Dim property_PDF As PropertyInfo = oTypeAbsDocClss.GetProperty("PDFSelected")
                        bDocClassPdfEnabled = Convert.ToBoolean(property_PDF.GetValue(oAbsDocClss, Nothing))

                        ' Update 
                        m_bSupportOCR = bDocClassOcrEnabled And IsHasQueueName(oAssembly, oAdminSession, OCR_QUEUENAME, oAbsDocClss)
                        m_bSupportPDF = bDocClassPdfEnabled And IsHasQueueName(oAssembly, oAdminSession, PDF_QUEUENAME, oAbsDocClss)
                    End Using
                End Using
            End If

        End Using

    End Sub

    '*************************************************
    ' IsHasQueueName
    '-------------------------------------------------
    ' Purpose: Check the if this BatchClass contains specific Queue
    ' Inputs: session is re-use to do function task
    ' sModuleName, used to check existing of expected queue
    ' absDocClss, used to OCR Full Text is replaced
    ' Outputs: true if the batch class queue has KofaxPDF|OCR Full Text else fail
    '*************************************************
    Public Function IsHasQueueName(ByRef oAssembly As Assembly, ByRef session As Object, ByRef sModuleName As String, ByRef absDocClss As Object) As Boolean

        Dim absQueueColl As IEnumerable
        Dim oTypeAbsBatchDef As Type
        Dim oTypeAdminSession As Type
        Dim oTypeAbsDocClss As Type
        Dim oTypeReleaseSetup As Type
        Dim oTypeReleaseScript As Type
        Dim oTypeAbsQueue As Type
        Dim oTypeAbsFunction As Type

        Dim oAbsBatchDef As Object
        Dim oReleaseSetup As Object
        Dim oReleaseScript As Object

        oTypeAbsBatchDef = oAssembly.GetType("Kofax.AbsDB.AbsBatchDef")
        oTypeAdminSession = oAssembly.GetType("Kofax.AbsDB.AbsAdminSession")
        oTypeAbsDocClss = oAssembly.GetType("Kofax.AbsDB.AbsDocumentDef")
        oTypeReleaseSetup = oAssembly.GetType("Kofax.AbsDB.AbsReleaseSetup")
        oTypeReleaseScript = oAssembly.GetType("Kofax.AbsDB.AbsReleaseScript")
        oTypeAbsQueue = oAssembly.GetType("Kofax.AbsDB.AbsQueue")
        oTypeAbsFunction = oAssembly.GetType("Kofax.AbsDB.AbsFunction")

        Dim method_GetBatchDef As MethodInfo = oTypeAdminSession.GetMethod("GetBatchDef")
        oAbsBatchDef = method_GetBatchDef.Invoke(session, New Object() {m_oSetupData.BatchClassID})

        Using New ComDisposer(oAbsBatchDef)
            If oAbsBatchDef IsNot Nothing Then
                '*** Release script has SupportsKofaxPDF=True tag
                If String.Equals(sModuleName, PDF_QUEUENAME) Then
                    Dim method_GetReleaseSetup As MethodInfo = oTypeAbsBatchDef.GetMethod("GetReleaseSetup")
                    oReleaseSetup = method_GetReleaseSetup.Invoke(oAbsBatchDef, New Object() {absDocClss})

                    Using New ComDisposer(oReleaseSetup)
                        Dim property_ReleaseScript As PropertyInfo = oTypeReleaseSetup.GetProperty("ReleaseScript")
                        oReleaseScript = property_ReleaseScript.GetValue(oReleaseSetup, Nothing)

                        Dim property_SupportsKofaxPDF As PropertyInfo = oTypeReleaseScript.GetProperty("SupportsKofaxPDF")

                        If Convert.ToBoolean(property_SupportsKofaxPDF.GetValue(oReleaseScript, Nothing)) Then
                            Return True
                        End If
                    End Using
                End If
                Dim method_GetQueueColl As MethodInfo = oTypeAbsBatchDef.GetMethod("GetQueueColl")
                absQueueColl = TryCast(method_GetQueueColl.Invoke(oAbsBatchDef, New Object() {2, 1}), IEnumerable)

                If absQueueColl Is Nothing Then
                    Return False
                End If

                Using New ComDisposer(absQueueColl)
                    For Each absCrrQueue As Object In absQueueColl
                        '*** Check if this batch class contains queue
                        Dim property_QueueName As PropertyInfo = oTypeAbsQueue.GetProperty("Name")

                        If property_QueueName.GetValue(absCrrQueue, Nothing).ToString().Equals(sModuleName) Then
                            Return True
                        Else

                            '*** Custom module has Function=OCR Full Text
                            If String.Equals(sModuleName, OCR_QUEUENAME) Then
                                Dim method_GetReplacesFunctionColl As MethodInfo = oTypeAbsQueue.GetMethod("GetReplacesFunctionColl")

                                Dim oFunctionColl As IEnumerable = TryCast(method_GetReplacesFunctionColl.Invoke(absCrrQueue, Nothing), IEnumerable)
                                Using New ComDisposer(oFunctionColl)
                                    For Each aFunction As Object In oFunctionColl
                                        Dim property_FunctionName As PropertyInfo = oTypeAbsFunction.GetProperty("Name")

                                        If property_FunctionName.GetValue(aFunction, Nothing).ToString().Contains(OCR_FUNCTION) Then
                                            Return True
                                        End If
                                    Next
                                End Using
                            End If
                        End If

                    Next
                End Using
            End If
        End Using
        Return False

    End Function
#End Region
End Class