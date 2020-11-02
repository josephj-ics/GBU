'****************************************************************************
'*   (c) Copyright Kofax Inc. 2009 All rights reserved.
'*   Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************

Imports Kofax.Connector.Common
Imports Kofax.ReleaseLib
Imports System.Collections.Generic
Imports System.IO
Imports System.Runtime.InteropServices

''' <summary>
''' An interface that a release runtime script class must implement
''' </summary>
''' <remarks></remarks>
<ComVisible(True)> _
<Guid("3D5C9063-AC15-4ca6-A3D1-481085AE5574")> _
<InterfaceType(ComInterfaceType.InterfaceIsIDispatch)> _
Public Interface IKfxReleaseScript
    Property DocumentData() As Kofax.ReleaseLib.ReleaseData
    Function CloseScript() As Kofax.ReleaseLib.KfxReturnValue
    Function OpenScript() As Kofax.ReleaseLib.KfxReturnValue
    Function ReleaseDoc() As Kofax.ReleaseLib.KfxReturnValue
End Interface

''' <summary>
''' This class is used to extract information (index fields, document class, batch class and so on)
''' and export to a text file. It also generates the files (OCR, PDF, IMG) corresponding
''' to the input documents.
''' </summary>
''' <remarks></remarks>
<ComVisible(True)> _
<ProgId("TextRel.kfxreleasescript")> _
<Guid("61316383-2751-4896-B4ED-EE04D7389E3A")> _
<ClassInterface(ClassInterfaceType.None)> _
Public Class KfxReleaseScript
    Implements IKfxReleaseScript
    Implements IDisposable

    ' ReleaseData object is set by the release controller.
    ' This object is to be used during the document release
    ' process as it will contain the document data and the
    ' external data source information defined during the
    ' setup process.
    Private m_oDocumentData As Kofax.ReleaseLib.ReleaseData
    Private m_oTextFile As New IndexExport
    Private m_bOpen As Boolean
    Private m_bDisposed As Boolean = False
    Private cm_strIndexesKey As String = "Kofax.Text.Indexes"

    ''' <summary>
    ''' Property of the ReleaseData object.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DocumentData() As Kofax.ReleaseLib.ReleaseData _
     Implements IKfxReleaseScript.DocumentData

        Get
            Return m_oDocumentData
        End Get
        Set(ByVal value As Kofax.ReleaseLib.ReleaseData)
            m_oDocumentData = value
        End Set
    End Property

    ''' <summary>
    ''' Script release point.  Perform any
    ''' necessary cleanup such as releasing
    ''' resources, etc.
    ''' </summary>
    ''' <returns> 
    ''' One of the following :
    '''    KFX_REL_SUCCESS, KFX_REL_ERROR,
    '''    KFX_REL_FATALERROR, KFX_REL_REINIT
    '''    KFX_REL_DOCCLASSERROR
    ''' </returns>
    ''' <remarks> 
    ''' Called by the Release Controller
    ''' once just before the script object
    ''' is released. 
    ''' </remarks>
    Public Function CloseScript() As Kofax.ReleaseLib.KfxReturnValue Implements IKfxReleaseScript.CloseScript
        CommonErrorHandler.DataObject = Nothing
        m_bOpen = False
        Return Kofax.ReleaseLib.KfxReturnValue.KFX_REL_SUCCESS
    End Function

    ''' <summary>
    ''' Script initialization point.  Perform
    ''' any necessary initialization such as
    ''' logging in to a remote data source,
    ''' allocating resources, etc.
    ''' </summary>
    ''' <returns> 
    ''' One of the following:
    '''    KFX_REL_SUCCESS, KFX_REL_ERROR,
    '''    KFX_REL_FATALERROR, KFX_REL_REINIT
    '''    KFX_REL_DOCCLASSERROR
    ''' </returns>
    ''' <remarks>
    ''' Called by the Release Controller
    ''' once when the script object is loaded.
    ''' </remarks>
    Public Function OpenScript() As Kofax.ReleaseLib.KfxReturnValue Implements IKfxReleaseScript.OpenScript
        CommonErrorHandler.DataObject = m_oDocumentData
        CommonErrorHandler.Title = My.Resources.TXT_RELEASE_ERROR
        m_bOpen = True
        Return Kofax.ReleaseLib.KfxReturnValue.KFX_REL_SUCCESS
    End Function

    ''' <remarks>  
    ''' Document release point.  Use the
    ''' ReleaseData object to release the
    ''' current document's data to the
    ''' external data repository.
    ''' </remarks> 
    ''' <returns> 
    ''' One of the following:
    '''   KFX_REL_SUCCESS, KFX_REL_ERROR,
    '''   KFX_REL_FATALERROR, KFX_REL_REINIT
    '''   KFX_REL_DOCCLASSERROR,
    ''' </returns> 
    ''' <remark>    
    ''' Called by the Release Controller once
    ''' for each document to be released.
    ''' </remark> 
    Public Function ReleaseDoc() As Kofax.ReleaseLib.KfxReturnValue Implements IKfxReleaseScript.ReleaseDoc

        ' Don't release if OpenScript() hasn't been called first
        If (Not m_bOpen) Then
            Return Kofax.ReleaseLib.KfxReturnValue.KFX_REL_ERROR
        End If

        Dim oImageExport As New ImageExport
        oImageExport.TheDocData = m_oDocumentData

        Try
            ' Gets the common information that is kept in different versions

            '*** this allows us to get the user selected settings
            Dim bExportImageFiles As Boolean = False
            Dim bExportOCRFullText As Boolean = False
            Dim bExportKofaxPDF As Boolean = False
            Dim bExportIndexFieldToCustomFile As Boolean = False
            Dim bExportIndexFieldInDefaultMode As Boolean = False

            ' are we to release image files?
            Using oCustomPropertiesReader As New CustomPropertiesReaderWriter(DocumentData.CustomProperties)
                bExportImageFiles = Not oCustomPropertiesReader.DisableImageExport
                ' what about ocr files?
                bExportOCRFullText = Not oCustomPropertiesReader.DisableTextExport
                ' and, what about pdf files?
                bExportKofaxPDF = oCustomPropertiesReader.EnableKofaxPdfExport
                ' then, what about releasing indexfields in the default configuration
                bExportIndexFieldInDefaultMode = oCustomPropertiesReader.UseDefaultStorage
                ' finally ... what about releasing indexfields to a custom file
                bExportIndexFieldToCustomFile = oCustomPropertiesReader.UseCustomStorage
            End Using

            Dim eNameOption As ImageExport.ImageFilenameOptionEnum
            Dim eDuplicateNames As ImageExport.DuplicateNameHandlingEnum
            ' gets values for eNameOption and eDuplicateName
            oImageExport.CalculateFileNamingSettings(eNameOption, eDuplicateNames)

            ' Build a dictionary of each type of value. For each type,
            ' we build a sorted list of values.
            Dim oValues As Dictionary(Of String, SortedList(Of Integer, ExportValue))
            oValues = BuildValuesDictionary(m_oDocumentData)

            ' The properties are just defined for KC9
            Dim strCalculatedFileName As String = String.Empty
            Dim strDefaultStorageFolder As String = String.Empty
            Dim strOCRStorageFolder As String = String.Empty
            Dim strPDFStorageFolder As String = String.Empty
            Dim strImageStorageFolder As String = String.Empty
            Dim strCustomIndexStorageFolder As String = String.Empty
            Dim strCustomIndexFileName As String = String.Empty

            Using oCustomPropertiesReader As New CustomPropertiesReaderWriter(DocumentData.CustomProperties)
                Dim strDefaultFileName As String = String.Empty

                ' Detect the release setting that was created in KC9 or the older ones    
                If (Not oCustomPropertiesReader.IsNewVersion) Then
                    ConvertOldExportSettingsToVersion9(bExportIndexFieldToCustomFile, strOCRStorageFolder, strPDFStorageFolder, strImageStorageFolder, _
                       strCustomIndexStorageFolder, strCustomIndexFileName, strDefaultFileName, eNameOption)

                    '*** If there is a default filename, need to assign its value to strCalculatedFileName.
                    '*** This allows the correct filenames to be reserved, and the files to be renamed 
                    '*** (if needed).
                    If Not String.IsNullOrEmpty(strDefaultFileName) Then
                        strCalculatedFileName = strDefaultFileName
                    Else
                        strCalculatedFileName = oImageExport.CalculateFileName(eNameOption, strDefaultFileName)
                    End If
                Else
                    Dim bSuppressImageIfPdfDetected As Boolean = False

                    ' this function gets us all the information we should need to process the files
                    oImageExport.CalculateExportSettings(oValues, strDefaultStorageFolder, strDefaultFileName, _
                         strOCRStorageFolder, strPDFStorageFolder, strImageStorageFolder, _
                         strCustomIndexStorageFolder, strCustomIndexFileName, bSuppressImageIfPdfDetected, _
                         bExportImageFiles, bExportOCRFullText, bExportKofaxPDF)

                    ' this will determine what the filename should be. empty if using the standard way
                    strCalculatedFileName = oImageExport.CalculateFileName(eNameOption, strDefaultFileName)

                    ' suppresses image output if pdf is detected and must exist, and outputs do not contain eDocs 
                    bExportImageFiles = bExportImageFiles AndAlso _
                       Not (bExportKofaxPDF AndAlso _
                      bSuppressImageIfPdfDetected AndAlso _
                      Not String.IsNullOrEmpty(m_oDocumentData.KofaxPDFFileName) AndAlso _
                      m_oDocumentData.ImageFiles.ContainsNonImageFile = 0)

                End If
            End Using

            ' Now for the real work

            ' this will get us the extentions of the different files that are going to be released
            ' to the image directory and reserve the files so no other workstation should use them
            ' unless of course they specify to overwrite files too.
            Dim oFileExtensions As List(Of String)
            Dim oReleaseDirs As New List(Of String)
            oFileExtensions = oImageExport.CalculateExtensions(bExportImageFiles, bExportOCRFullText, bExportKofaxPDF, _
            strImageStorageFolder, strOCRStorageFolder, strPDFStorageFolder, oReleaseDirs)

            Dim strUniqueIDFileName As String ' the standard hex filename
            strUniqueIDFileName = m_oDocumentData.UniqueDocumentID.ToString("X8")

            ' SPR 109005 - Incorrect duplication filename causing batch to be rejected
            Dim strTempFolder As String
            strTempFolder = "KFX" + strUniqueIDFileName + "\"

            ' this function will reserve the files based on the file extentions that were calculated
            ' in CalculateExtensions. We are using to our advantage that the copy functions will overwrite by default.
            ' It also reserves the unique ID filename at the same time
            oImageExport.ReserveImageNames(oReleaseDirs, strCalculatedFileName, eDuplicateNames, oFileExtensions)

            Dim strImageFilePath As String = String.Empty
            ' SPR 114609 - New Defect - Error "Could not find a part of the path.." when export pdf file.
            Dim strTempImageStorageFolder As String = Path.Combine(strImageStorageFolder, strTempFolder)

            ' first work with the image files
            If bExportImageFiles AndAlso Not String.IsNullOrEmpty(strImageStorageFolder) Then
                Try
                    ' let KC release the images. Note that -1 is the default value
                    Dim nImageType As Integer = -1
                    If (String.Compare(oImageExport.GetTifExtension, c_strCPValPdf) = 0) Then
                        nImageType = ImageFormatEnum.MTIFF_G4
                    End If

					' SPR 114488 - Export fails when export images as TIFF – CCITT Group 4.
                    If (Not Directory.Exists(strTempImageStorageFolder)) Then
                        Directory.CreateDirectory(strTempImageStorageFolder)
                    End If

                    ' Copy to the temp folder first
                    m_oDocumentData.ImageFiles.Copy(strTempImageStorageFolder, nImageType)

                    If m_oDocumentData.ImageFiles.ContainsNonImageFile <> 0 Then
                        strImageFilePath = m_oDocumentData.ImageFiles.NonImageFilesReleasedDirectory
                    Else
                        strImageFilePath = m_oDocumentData.ImageFiles.ReleasedDirectory
                    End If
                Catch ex As Exception

                    '*** If error occurs while copying the image file, the file or folder
                    '*** has been created, but these variables have not yet been set.
                    strImageFilePath = m_oDocumentData.ImageFiles.ReleasedDirectory
                    Throw
                End Try
            End If

            ' SPR 109005 - Incorrect duplication filename causing batch to be rejected
            ' SPR 114609 - New Defect - Error "Could not find a part of the path.." when export pdf file.
            Dim strTempOCRStorageFolder As String = Path.Combine(strOCRStorageFolder, strTempFolder)

            ' Release the Full Text OCR file.
            If bExportOCRFullText And Not String.IsNullOrEmpty(strOCRStorageFolder) Then

				' SPR 114488 - Export fails when export images as TIFF – CCITT Group 4.
                If (Not Directory.Exists(strTempOCRStorageFolder)) Then
                    Directory.CreateDirectory(strTempOCRStorageFolder)
                End If
                ' makes sure the OCR storage folder exists
                ' Copy to the temp folder first
                m_oDocumentData.TextFiles.Copy(strTempOCRStorageFolder)
            End If

            ' SPR 114609 - New Defect - Error "Could not find a part of the path.." when export pdf file.
            Dim strTempPDFStorageFolder As String = Path.Combine(strPDFStorageFolder, strTempFolder)

            ' Release the Kofax PDF file.
            If bExportKofaxPDF And Not String.IsNullOrEmpty(strPDFStorageFolder) Then

                ' makes sure the PDF storage folder exists
                ' Copy to the temp folder first
				' SPR 114488 - Export fails when export images as TIFF – CCITT Group 4.
                If (Not Directory.Exists(strTempPDFStorageFolder)) Then
                    Directory.CreateDirectory(strTempPDFStorageFolder)
                End If
                m_oDocumentData.CopyKofaxPDFFileToPath(strTempPDFStorageFolder)

                ' if Kofax PDF is checked and the KC release image is not checked,
                ' use the Kofax PDF image path as the release image path.
                If Not bExportImageFiles And String.IsNullOrEmpty(strImageFilePath) Then

                    ' format the pdf file name
                    Dim strPDFFileName As String = Hex(m_oDocumentData.UniqueDocumentID)
                    strPDFFileName = New String("0"c, 8 - Len(strPDFFileName)) & strPDFFileName & c_strDotCharacter & c_strCPValPdf
                    strImageFilePath = Path.Combine(strTempPDFStorageFolder, strPDFFileName)
                End If
            End If

            'now need to rename the files

            ' SPR 109005 - Incorrect duplication filename causing batch to be rejected.
            If (strCalculatedFileName.Length > 0) Then

                ' check to see if we need to do anything with the image file
                If bExportImageFiles Then
                    If Not oImageExport.IsSinglePageImage Then
                        Dim strSrcFile As String = strUniqueIDFileName & c_strDotCharacter & oImageExport.GetTifExtension
                        Dim strDestFile As String = strCalculatedFileName & c_strDotCharacter & oImageExport.GetTifExtension
                        oImageExport.RenameOutputFile( _
                         Path.Combine(strTempImageStorageFolder, strSrcFile), _
                         Path.Combine(strImageStorageFolder, strDestFile))
                    End If

                    If oImageExport.GetIncludesSubfolder() Then
                        oImageExport.RenameOutputDirectory( _
                         Path.Combine(strTempImageStorageFolder, strUniqueIDFileName), _
                         Path.Combine(strImageStorageFolder, strCalculatedFileName))
                    End If
                End If

                '*** For both OCR and PDF, they need to be renamed under two circumstances:
                '*** 1) The 9.0 export connector specifies a naming option
                '*** 2) The 8.0 release script specified the ocr and pdf to the image directory
                '*** This means that if a batch class with the old release script did not specify
                '*** that option, DON'T rename them. For 9.0, bLegacyOCRandPDFToImage is set to 
                '*** TRUE and is not modified.

                If bExportOCRFullText Then
                    Dim strSrcFile As String = strUniqueIDFileName & c_strDotCharacter & oImageExport.GetOCRExtension
                    Dim strDestFile As String = strCalculatedFileName & c_strDotCharacter & oImageExport.GetOCRExtension
                    oImageExport.RenameOutputFile( _
                     Path.Combine(strTempOCRStorageFolder, strSrcFile), _
                     Path.Combine(strOCRStorageFolder, strDestFile))
                End If

                If bExportKofaxPDF Then
                    Dim strSrcFile As String = strUniqueIDFileName & c_strDotCharacter & c_strCPValPdf
                    Dim strDestFile As String = strCalculatedFileName & c_strDotCharacter & c_strCPValPdf
                    oImageExport.RenameOutputFile( _
                     Path.Combine(strTempPDFStorageFolder, strSrcFile), _
                     Path.Combine(strPDFStorageFolder, strDestFile))
                End If

                '*** Finally adjust/rename the image file path that is written to
                '*** the text release file.
                strImageFilePath = strImageFilePath.Replace(strTempFolder + strUniqueIDFileName, strCalculatedFileName)
            End If

            ' check for any empty files we created (spr 29193)
            oImageExport.RemoveEmptyReservedFiles()

            Dim oDirectoryInfo As System.IO.DirectoryInfo
            Try
                oDirectoryInfo = New System.IO.DirectoryInfo(strTempPDFStorageFolder)
                oDirectoryInfo.Delete()
            Catch
            End Try

            Try
                oDirectoryInfo = New System.IO.DirectoryInfo(strTempImageStorageFolder)
                oDirectoryInfo.Delete()
            Catch
            End Try

            Try
                oDirectoryInfo = New System.IO.DirectoryInfo(strTempOCRStorageFolder)
                oDirectoryInfo.Delete()
            Catch
            End Try

            ' Release the index information to the text file.
            If String.IsNullOrEmpty(strCalculatedFileName) Then
                strCalculatedFileName = strUniqueIDFileName
            End If

            ' Extract the index values from the dictionary of all values
            Dim oIndexValues As New SortedList(Of Integer, ExportValue)
            If oValues.ContainsKey(cm_strIndexesKey) Then
                oIndexValues = oValues.Item(cm_strIndexesKey)
            End If

            ' According to the default configuration
            If bExportIndexFieldInDefaultMode Then
                Dim strSeparator As String = String.Empty
                If Right(strDefaultStorageFolder, 1) <> c_strBackSlashCharacter Then
                    strSeparator = c_strBackSlashCharacter
                End If

                Dim strDesFilePath As String = strDefaultStorageFolder & strSeparator & strCalculatedFileName & "_index.txt"
				'*** Fixed SPR95262 - Released image file path in Index file Text Release is shortened
                '*** strImageFilePath will be fullName when release Indexes
                m_oTextFile.ReleaseIndexes(m_oDocumentData, oIndexValues, GetLongFilename(strImageFilePath), strOCRStorageFolder, strPDFStorageFolder, strDesFilePath)

            End If

            ' Exports to a custom file.
            If bExportIndexFieldToCustomFile Then
                Dim strSeparator As String = String.Empty
                If Right(strDefaultStorageFolder, 1) <> c_strBackSlashCharacter Then
                    strSeparator = c_strBackSlashCharacter
                End If

                Dim strDesFilePath As String = strCustomIndexStorageFolder & strSeparator & strCustomIndexFileName
                '*** Fixed SPR95262 - Released image file path in Index file Text Release is shortened
                '*** strImageFilePath will be fullName when release Indexes
                m_oTextFile.ReleaseIndexes(m_oDocumentData, oIndexValues, GetLongFilename(strImageFilePath), strOCRStorageFolder, strPDFStorageFolder, strDesFilePath)
            End If

            Return Kofax.ReleaseLib.KfxReturnValue.KFX_REL_SUCCESS

        Catch ex As Exception
            ReleaseDoc = Kofax.ReleaseLib.KfxReturnValue.KFX_REL_ERROR

            Try
                '*** remove any files and directories that we created            
                oImageExport.RemoveReserveFiles()
            Catch oEx As Exception
                ' do nothing
            Finally
                Dim nErrNum As Integer = Err.Number
                Dim strErrDesc As String = Err.Description

                ' Mark the document in error
                '*** Log error to Capture log file
                If m_oDocumentData IsNot Nothing Then
                    m_oDocumentData.SendMessage(strErrDesc, nErrNum, Kofax.ReleaseLib.KfxInfoReturnValue.KFX_REL_DOC_ERROR)
                    '*** send error to Capture error log
					m_oDocumentData.LogError(-1, 0, 0, ex.Message, c_strShortProductName, 0)
                End If
            End Try

        End Try

    End Function

	''' <summary>
	''' Get the long file name from short file name (8.3 DOS format).
	''' </summary>
	''' <param name="strImageFilePath"></param>
	''' <returns>long file name</returns>
	''' <remarks></remarks>
    Public Function GetLongFilename _
       (ByVal strImageFilePath As String) _
       As String
        Dim fi As FileInfo = Nothing
        If Not strImageFilePath.Equals(String.Empty) Then
            fi = New FileInfo(strImageFilePath)
            GetLongFilename = fi.FullName
        Else
            GetLongFilename = ""
        End If

    End Function

    ''' <summary>
    ''' Iterates the values collection once and breaks it into separate ordered
    ''' collections by key. Doing this instead of iterating the values collection
    ''' repeatedly helps improve performance.
    ''' </summary>
    ''' <param name="oDocData">Document data</param>
    Private Function BuildValuesDictionary(ByVal oDocData As Kofax.ReleaseLib.ReleaseData) As Dictionary(Of String, SortedList(Of Integer, ExportValue))

        Dim oValuesDictionary As Dictionary(Of String, SortedList(Of Integer, ExportValue))
        oValuesDictionary = New Dictionary(Of String, SortedList(Of Integer, ExportValue))
        Dim cm_strKeyDestinationSeparator As String = "#"

        Using enumerator As New ComEnumerator(oDocData.Values.GetEnumerator())
            While enumerator.MoveNext()
                Dim oValue As Kofax.ReleaseLib.Value = CType(enumerator.Current, Kofax.ReleaseLib.Value)
                Using New ComDisposer(oValue)
                    Dim nSort As Integer = 0
                    Dim strKey As String = cm_strIndexesKey

                    ' Destination will either be just a number, in which case
                    ' that number represents the order of the values to export 
                    ' in the text file, or it will be of the form 'Default Storage Folder#2#10'
                    ' where the first part is the key, the second part is the item number and the 
                    ' last part is the total number of expected items.
                    Dim strSort As String = oValue.Destination
                    Dim nSepPos As Integer = strSort.IndexOf(cm_strKeyDestinationSeparator)
                    If nSepPos > 0 Then
                        strKey = strSort.Substring(0, nSepPos)
                        Dim nSepPos2 As Integer
                        nSepPos2 = strSort.IndexOf(cm_strKeyDestinationSeparator, nSepPos + 1)
                        strSort = strSort.Substring(nSepPos + 1, nSepPos2 - nSepPos - 1)
                    End If
                    If Integer.TryParse(strSort, nSort) Then
                        Dim oExportValuesList As SortedList(Of Integer, ExportValue)
                        If Not oValuesDictionary.ContainsKey(strKey) Then
                            oExportValuesList = New SortedList(Of Integer, ExportValue)
                            oValuesDictionary.Add(strKey, oExportValuesList)
                        Else
                            oExportValuesList = oValuesDictionary.Item(strKey)
                        End If
                        oExportValuesList.Add(nSort, New ExportValue(oValue.SourceName, oValue.SourceType, oValue.Value))
                    End If
                End Using
            End While
        End Using

        Return oValuesDictionary
    End Function

    ''' <summary>
    ''' Converts the old export settings (created by versions of KC before 9) to new settings in KC9
    ''' </summary>
    ''' <param name="bExportIndexFieldToCustomFile"> true, if Index Fields are exported to a custom file </param>
    ''' <param name="strOCRStorageFolder"> is the directory path where OCR files will be exported to </param>
    ''' <param name="strPDFStorageFolder"> is the directory path where PDF files will be exported to </param>
    ''' <param name="strImageStorageFolder"> is the directory path where Image files will be exported to </param>
    ''' <param name="strCustomIndexStorageFolder"> is the additional directory path as releasing the index fields </param>
    ''' <param name="strCustomIndexFileName"> is the custom file name of index fields </param>
    ''' <param name="eNameOption"> set based on the name option selected</param>
    ''' <remarks></remarks>
    Private Sub ConvertOldExportSettingsToVersion9(ByRef bExportIndexFieldToCustomFile As Boolean, ByRef strOCRStorageFolder As String, _
       ByRef strPDFStorageFolder As String, ByRef strImageStorageFolder As String, _
       ByRef strCustomIndexStorageFolder As String, ByRef strCustomIndexFileName As String, ByRef strDefaultFileName As String, _
       ByRef eNameOption As ImageExport.ImageFilenameOptionEnum)
        Dim bExportOCRToImageDir As Boolean
        Dim strFinalRelDir As String = String.Empty
        Dim strIndexFieldValue As String = String.Empty ' if the index filename option is selected store the value        

        ' This function gets us all the information we should need to process the files, which was created in the old KC
        Dim oImageName As New ImageExport
        oImageName.TheDocData = m_oDocumentData
        oImageName.CalculateReleaseSettingsForKC8(bExportOCRToImageDir, eNameOption, strFinalRelDir, strIndexFieldValue)

        ' Index File Naming in KC8 is equal to Default File Name in KC9
        strDefaultFileName = strIndexFieldValue

        ' The mode of releasing index fields in KC8 is the custom mode in KC9
        bExportIndexFieldToCustomFile = True

        ' Parses the full file path of index fields into directory path and filename
        Using customPropertiesReader As New CustomPropertiesReaderWriter(m_oDocumentData.CustomProperties)
            Dim strIndexFileName As String = customPropertiesReader.IndexFileName

            ' Index of the last slash in a string 
            Dim nLastSlash As Integer = strIndexFileName.LastIndexOf(c_strBackSlashCharacter)

            strCustomIndexStorageFolder = strIndexFileName.Substring(0, nLastSlash + 1)
            strCustomIndexFileName = strIndexFileName.Substring(nLastSlash + 1, strIndexFileName.Length - nLastSlash - 1)
        End Using

        ' Storage folder of files, such as: Image, Ocr, Pdf
        strImageStorageFolder = strFinalRelDir
        If bExportOCRToImageDir Then
            strPDFStorageFolder = strFinalRelDir
            strOCRStorageFolder = strFinalRelDir
        Else
            strPDFStorageFolder = m_oDocumentData.KofaxPDFPath
            strOCRStorageFolder = m_oDocumentData.TextFilePath
        End If

		If Not String.IsNullOrEmpty(strPDFStorageFolder) AndAlso Not strPDFStorageFolder.EndsWith(c_strBackSlashCharacter) Then
			strPDFStorageFolder = strPDFStorageFolder & c_strBackSlashCharacter
		End If

		If Not String.IsNullOrEmpty(strOCRStorageFolder) AndAlso Not strOCRStorageFolder.EndsWith(c_strBackSlashCharacter) Then
			strOCRStorageFolder = strOCRStorageFolder & c_strBackSlashCharacter
		End If

    End Sub

    ''' <summary>
    ''' Release Non Image Files to image file path.
    ''' This is needed when releasing to PDF image format
    ''' and eDocuments are part of the same document.
    ''' Since the PDF Releaser doesn't support eDocuments,
    ''' these have to be handled separately
    ''' </summary>
    ''' <param name="strReleaseDirectory"> The directory where the files are to be exported </param>
    ''' <remarks></remarks>
    Private Sub ExportNonImageFiles(ByRef strReleaseDirectory As String)
        Dim strMultipageFileName As String

        '*** Use the MTIFF_G4 format to make the deletion of images easy
        m_oDocumentData.ImageFiles.Copy(strReleaseDirectory, ImageFormatEnum.MTIFF_G4)

        '*** Delete the image file (if any). If there are no images within this
        '*** document, then the file will not exist.
        strMultipageFileName = Path.Combine(strReleaseDirectory, String.Format("{0:X8}.TIF", m_oDocumentData.UniqueDocumentID))
        If (File.Exists(strMultipageFileName)) Then
            File.Delete(strMultipageFileName)
        End If

    End Sub

#Region " IDisposable Support "
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.m_bDisposed Then
            If disposing Then
                Marshal.FinalReleaseComObject(m_oDocumentData.Values)
                Marshal.FinalReleaseComObject(m_oDocumentData)
            End If
            Me.m_bDisposed = True
        End If
    End Sub

    ''' <summary>
    ''' Disposes the ReleaseData object.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Dispose() Implements IDisposable.Dispose
        'Marshal.FinalReleaseComObject(m_oDocumentData.Values)
        'Marshal.FinalReleaseComObject(m_oDocumentData)
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'Set UI Language.
		Kofax.Connector.Common.GeneralUtils.ApplyLocalizationIfPossible()
    End Sub
End Class