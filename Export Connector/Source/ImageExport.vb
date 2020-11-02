'****************************************************************************
'*   (c) Copyright Kofax Inc. 2009 All rights reserved.
'*   Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************

Imports Kofax.ReleaseLib
Imports Kofax.Connector.Common
Imports System.Collections.Generic
Imports System.IO
Imports System.Runtime.InteropServices

''' <summary>
''' Provides ways to alter the way in which image files are released.
''' This is used for the imageName tab.  All imageName tab
''' operations are performed in routines contained in this class.
''' </summary>
''' <remarks></remarks>
Friend Class ImageExport

    Public Enum ImageFilenameOptionEnum
        PFO_Unknown = 0
        PFO_Standard = 1
        PFO_Decimal = 2
        PFO_LeadingZero = 3
        PFO_Custom = 4
    End Enum

    Public Enum DuplicateNameHandlingEnum
        PDN_Unknown = 0
        PDN_Replace = 1
        PDN_Rename = 2
        PDN_Error = 3
    End Enum

    ' Name of file which contains the ImageExport class. It's used when raising errors.
    Private Const cm_strFileName As String = "ImageExport.vb"

    '*** this variable keeps track of what files we have reserved
    Private m_oFilesCreated As New List(Of String)

    Private m_oDocData As Kofax.ReleaseLib.ReleaseData

	Declare Unicode Function GetShortPathName Lib "kernel32.dll" Alias "GetShortPathNameW" ( _
	   ByVal longPath As String, _
	   <MarshalAs(UnmanagedType.LPTStr)> ByVal ShortPath As System.Text.StringBuilder, _
	   <MarshalAs(UnmanagedType.U4)> ByVal bufferSize As Integer) As Integer

    ''' <summary>
    ''' Allows us to rename the released file to what the user now wants.
    ''' Note that if the file is a zero-byte size, both files will be deleted.
    ''' </summary>   
    ''' <param name="strSrcName"> source filename </param>
    ''' <param name="strFinalName"> destination filename </param>
    ''' <remarks></remarks>
    Public Sub RenameOutputFile(ByVal strSrcName As String, ByVal strFinalName As String)

        '*** If it's already what we want... we're done
        If strSrcName = strFinalName Then
            Exit Sub
        End If

        File.Delete(strFinalName)

        If File.Exists(strSrcName) Then
            Dim sourceFile As New FileInfo(strSrcName)

            '*** if the file size is zero, the system didn't create a file for us and we
            '*** need to remove it
            If sourceFile.Length > 0 Then
                sourceFile.MoveTo(strFinalName)
            Else
                sourceFile.Delete()
            End If
        End If
    End Sub

    ''' <summary>
    ''' Renames the directory
    ''' </summary>
    ''' <param name="strSrcName"> source directory name </param>
    ''' <param name="strFinalName"> destination directory name </param>
    ''' <remarks></remarks>
    Public Sub RenameOutputDirectory(ByRef strSrcName As String, ByRef strFinalName As String)
        '*** if it's already what we want... we're done
        If strSrcName = strFinalName Then
            Exit Sub
        End If

        If Directory.Exists(strFinalName) Then
            Directory.Delete(strFinalName, True)
        End If
        Directory.Move(strSrcName, strFinalName)
    End Sub

    ''' <summary>
    ''' Calculates the name of the final filename (directory if needed) based on
    ''' the settings passed in.
    ''' </summary>
    ''' <param name="eNameOption"> enum value of the selected file naming options </param>
    ''' <param name="strDefaultFileName"> 
    ''' string value representing the index field value that 
    ''' that will be used as the filename if the index filename option is seleted.
    ''' </param>
    ''' <returns> String value representing the filename to be used </returns>
    ''' <remarks></remarks>
    Public Function CalculateFileName(ByRef eNameOption As ImageFilenameOptionEnum, ByRef strDefaultFileName As String) As String
        If eNameOption = ImageFilenameOptionEnum.PFO_Decimal Or eNameOption = ImageFilenameOptionEnum.PFO_LeadingZero Then
            '*** Add leading zeros to Doc ID if using leading zeros
            If eNameOption = ImageFilenameOptionEnum.PFO_LeadingZero Then
                Return m_oDocData.UniqueDocumentID.ToString("D10")
            Else
                Return m_oDocData.UniqueDocumentID.ToString()
            End If
        ElseIf eNameOption = ImageFilenameOptionEnum.PFO_Custom Then
            Return strDefaultFileName
        Else
            '*** everything else is the standard hex format
            Return m_oDocData.UniqueDocumentID.ToString("X8")
        End If
    End Function

    ''' <summary>
    ''' Need to determine the image file extension based on the selected output type
    ''' </summary>
    ''' <returns> String value with the extension name </returns>
    ''' <remarks></remarks>
    Public Function GetTifExtension() As String
        Dim oImage As Kofax.ReleaseLib.ImageFile

        If m_oDocData.ImageFiles.Count > 0 Then
            oImage = m_oDocData.ImageFiles.Item(1)

            Select Case oImage.ImageType
                Case ImageFormatEnum.JPG_JPEG
                    Return c_strIFJpg
                Case ImageFormatEnum.PCX_PACKBYTES
                    Return c_strIFPcx
                Case Else
                    Return c_strIFTif '*** default setting
            End Select
        End If
        Return c_strIFTif '*** default setting
    End Function

    ''' <summary>
    ''' Need to determine the OCR file extension based on the generated AC filename 
    ''' </summary>
    ''' <returns> String value of extension </returns>
    ''' <remarks></remarks>
    Public Function GetOCRExtension() As String
        Dim strOcrExt As String = Nothing

        If m_oDocData.TextFiles.Count > 0 Then
            strOcrExt = Path.GetExtension(m_oDocData.TextFiles.Item(1).FileName)
        End If

        If String.IsNullOrEmpty(strOcrExt) Then
            Return c_strCPValTxt
        Else
            Return strOcrExt.Substring(1)
        End If
    End Function

    ''' <summary>
    ''' This function will calculate the extensions that will be used when we 
    ''' reserve files that will be created and released
    ''' </summary>
    ''' <param name="bReleaseImageFiles"> true when releasing images files </param>
    ''' <param name="bReleaseOCRFullText"> true when releasing OCR files </param>
    ''' <param name="bReleaseKofaxPDF"> true when releasing PDF files </param>
    ''' <param name="strImageStorageFolder"> is the directory where Image files will be released to </param>
    ''' <param name="strOCRStorageFolder"> is the directory where OCR files will be released to </param>
    ''' <param name="strPDFStorageFolder"> is the directory where PDF files will be released to </param>
    ''' <param name="oReleaseDirs"> is the list of release directorys that will be use to reserver files</param>
    ''' <returns> The list of extensions that will be used to reserve files </returns>
    ''' <remarks></remarks>
	Public Function CalculateExtensions( _
		ByRef bReleaseImageFiles As Boolean, _
		ByRef bReleaseOCRFullText As Boolean, _
		ByRef bReleaseKofaxPDF As Boolean, _
		ByVal strImageStorageFolder As String, _
		ByRef strOCRStorageFolder As String, _
		ByRef strPDFStorageFolder As String, _
		ByRef oReleaseDirs As List(Of String)) _
		As List(Of String)

		Dim oExtList As New List(Of String)
		If bReleaseImageFiles And Not IsSinglePageImage() Then
			oExtList.Add(GetTifExtension())
			oReleaseDirs.Add(strImageStorageFolder)
		End If

		'*** Ensures the release directory exists
		If Not String.IsNullOrEmpty(strImageStorageFolder) Then
			strImageStorageFolder = New DirectoryInfo(strImageStorageFolder).FullName
			Directory.CreateDirectory(strImageStorageFolder)
		End If



		' To avoid the case in which both inputs point to the same directory but in different formats 
		' (e.g. the first string is "C:\Documents and Settings" and the another one "C:\Docume~1"), 
		' we will convert all of them to the same format before comparing.
		'*** The ocr and pdf folder can be empty, for example, when upgrading from 8.0 to 9.0, and 
		'*** publishing the batch class again without entering into the new UI and re-saving
		'*** settings. If it is empty, don't create a DirectoryInfo() object, and don't compare it 
		'*** against the image folder. It makes no sense to add OCR or PDF extensions to the
		'*** extension list if no directory was specified.
		If (Not String.IsNullOrEmpty(strPDFStorageFolder)) Then
			strPDFStorageFolder = New DirectoryInfo(strPDFStorageFolder).FullName
			If bReleaseKofaxPDF Then
				oExtList.Add(c_strCPValPdf)
				oReleaseDirs.Add(strPDFStorageFolder)
			End If
		End If

		If (Not String.IsNullOrEmpty(strOCRStorageFolder)) Then
			strOCRStorageFolder = New DirectoryInfo(strOCRStorageFolder).FullName
			If bReleaseOCRFullText Then
				oExtList.Add(GetOCRExtension())
				oReleaseDirs.Add(strOCRStorageFolder)
			End If
		End If

		Return oExtList
	End Function

    ''' <summary>
    ''' The function lets us know if we are to create subfolders 
    ''' </summary>
    ''' <returns> true if we should create a subdirectory for eDocs or single page image files </returns>
    ''' <remarks></remarks>
    Public Function GetIncludesSubfolder() As Boolean
        If m_oDocData.ImageFiles.ContainsNonImageFile <> 0 Then
            Return True
        End If

        Return IsSinglePageImage()
    End Function

    ''' <summary>
    ''' Determines if the image is single or multi-page
    ''' </summary>
    ''' <returns> True if the image is single page </returns>
    ''' <remarks></remarks>
    Public Function IsSinglePageImage() As Boolean
        If m_oDocData.ImageFiles.Count > 0 Then
            If m_oDocData.ImageFiles.Item(1).ImageType = 16 Then
                Return False
            End If

            Select Case m_oDocData.ImageFiles.Item(1).ImageType
                Case ImageFormatEnum.MTIFF_G4, _
                    ImageFormatEnum.MTIFF_G31D, _
                    ImageFormatEnum.MTIFF_RAW, _
                    ImageFormatEnum.MTIFF_G32D, _
                    ImageFormatEnum.MTIFF_JPEG, _
                    ImageFormatEnum.MTIFF_LZW
                    Return False
                Case Else
                    Return True
            End Select
        End If
        Return True
    End Function

    ''' <summary>
    ''' This function reserves the filenames (both the name the system copies the file
    ''' as and the eventual filename when release is finished. We rely on the fact that
    ''' the KC system default copy functions replace the file. The files are versioned here
    ''' if one is found to already exist and the "rename" option is selected for duplicate
    ''' files.
    ''' </summary>
    ''' <param name="oReleaseDirs"> the list of directory the file is to be released to </param>
    ''' <param name="strFileName"> the base filename </param>
    ''' <param name="eDuplicateNames"> how the user has selected to handle duplicate names </param>
    ''' <param name="oExtList"> collection of extentions that are reserved </param>
    ''' <returns> 
    ''' True if we succeed in researving the set of files to be released
    ''' Throws an exception if a file already exists and the user selected 'error' on duplicate names
    ''' </returns>
    ''' <remarks></remarks>
    Public Function ReserveImageNames( _
        ByRef oReleaseDirs As List(Of String), _
        ByRef strFileName As String, _
        ByRef eDuplicateNames As DuplicateNameHandlingEnum, _
        ByRef oExtList As List(Of String)) _
        As Boolean

        Dim strVersion As String
        Dim strReserveErrName As String
        Dim nCounter As Integer

        strVersion = String.Empty
        strReserveErrName = strFileName ' allows us to start the while loop
        nCounter = 1

        '*** Ensures the release directory exists, only if it was specified. If the option
        '*** to specify image release is disabled, strReleaseDir is blank.
        Dim strReleaseDir As String
        For Each strReleaseDir In oReleaseDirs
            If (Not String.IsNullOrEmpty(strReleaseDir)) AndAlso (Not Directory.Exists(strReleaseDir)) Then
                Directory.CreateDirectory(strReleaseDir)
            End If
        Next

        '*** reserve what we ultimately want the filename to be
        While (Not String.IsNullOrEmpty(strReserveErrName))
            strReserveErrName = Reserve( _
                oReleaseDirs, _
                strFileName & strVersion, _
                oExtList, _
                GetIncludesSubfolder, _
                eDuplicateNames = DuplicateNameHandlingEnum.PDN_Replace)

            If (strReserveErrName <> String.Empty) Then
                If (eDuplicateNames = DuplicateNameHandlingEnum.PDN_Error) Then
                    Err.Raise(c_shMsgUltimateFileExists, cm_strFileName, String.Format(My.Resources.MSG_ULTIMATE_FILE_EXISTS, strReserveErrName))
                End If

                ' if we get to this point, we know we had an error with the file, 
                ' but that we are to version it
                strVersion = "_V" & CStr(nCounter)
                nCounter = nCounter + 1
            End If

        End While

        '*** and now we need to reserve the default uniqID files too, but only if the
        '*** ultimate name is different
        Dim strTmpName As String = m_oDocData.UniqueDocumentID.ToString("X8")
        ' SPR 109005 - Incorrect duplication filename causing batch to be rejected
        Dim strTmpFolder As String = "KFX" + strTmpName + "\"
        For Each strReleaseDir In oReleaseDirs
            ' SPR 114609 - New Defect - Error "Could not find a part of the path.." when export pdf file.
            Dim strTempReleaseDir As String = Path.Combine(strReleaseDir, strTmpFolder)
            If (Not Directory.Exists(strTempReleaseDir)) Then
                Directory.CreateDirectory(strTempReleaseDir)
            End If
        Next

        ' SPR 114609 - New Defect - Error "Could not find a part of the path.." when export pdf file.
        Dim strFullTmpName As String = Path.Combine(strTmpFolder, strTmpName)
        If (StrComp(strFullTmpName, strFileName, CompareMethod.Text) <> 0) Then
            strReserveErrName = Reserve(oReleaseDirs, strFullTmpName, oExtList, GetIncludesSubfolder, False)

            If (strReserveErrName <> String.Empty) Then
                If (eDuplicateNames = DuplicateNameHandlingEnum.PDN_Error) Then
                    Err.Raise(c_shMsgUltimateFileExists, cm_strFileName, String.Format(My.Resources.MSG_ULTIMATE_FILE_EXISTS, strReserveErrName))
                End If
            End If
        End If

        strFileName = strFileName & strVersion

        Return String.IsNullOrEmpty(strReserveErrName)
    End Function

    ''' <summary>
    ''' Creates a set of files based on the extensions passed in.
    ''' </summary>
    ''' <param name="oReleaseDirs"> the directory list where the files will be created </param>
    ''' <param name="strBaseName"> the base filename created </param>
    ''' <param name="oExtList"> stores the extensions for the created files </param>
    ''' <param name="bFolder"> if true, a directory is created using the filename </param>
    ''' <param name="bOverwriteOption"> if true the create operation overwrites any existing file </param>
    ''' <returns>
    ''' Returns an empty string if we succeeded in creating the files. 
    ''' The function returns the name of the file that an error occurred. 
    ''' Will throw if any error other than a file existing already
    ''' </returns>
    ''' <remarks></remarks>
    Private Function Reserve( _
        ByRef oReleaseDirs As List(Of String), _
        ByRef strBaseName As String, _
        ByRef oExtList As List(Of String), _
        ByRef bFolder As Boolean, _
        ByRef bOverwriteOption As Boolean) As String

        Dim strFileName As String = String.Empty
        Dim strFullName As String

        Dim oTextStream As StreamWriter

        'SPR 66953: Rename if duplicate option not working for PDF files - overwrites existing files
        'Loop to create file name and check it existent.
        Dim i As Integer
        For i = 0 To oReleaseDirs.Count - 1
            ' will pass back the file name if we have an error
            strFileName = String.Format("{0}.{1}", strBaseName, oExtList.Item(i))
            strFullName = Path.Combine(oReleaseDirs.Item(i), strFileName)

            ' open up the file
            If bOverwriteOption = False AndAlso File.Exists(strFullName) Then
                Return strFileName
            End If

            oTextStream = File.CreateText(strFullName)
            oTextStream.Dispose()
            m_oFilesCreated.Add(strFullName)
        Next i

        ' if the folder already exists, we need to version the name
        ' note that this will only be the case where we are releasing using index naming
        If bFolder Then
            'SPR 66953: Rename if duplicate option not working for PDF files - overwrites existing files
            For i = 0 To oReleaseDirs.Count - 1
                strFileName = strBaseName
                strFullName = Path.Combine(oReleaseDirs.Item(i), strFileName)

                ' SPR 109005 - Incorrect duplication filename causing batch to be rejected
                If bOverwriteOption = False AndAlso Directory.Exists(strFullName) AndAlso Not m_oFilesCreated.Contains(strFullName) Then
                    Return strFileName
                End If

                ' will pass back the directory name if we have an error
                If (Not m_oFilesCreated.Contains(strFullName)) Then
                    Directory.CreateDirectory(strFullName)
                    m_oFilesCreated.Add(strFullName)
                End If
            Next
        End If
        Return String.Empty
    End Function

    ''' <summary>
    ''' Cleanup function to delete the files we created during the reserve operation
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RemoveReserveFiles()
        Dim strFileName As String

        For Each strFileName In m_oFilesCreated
            If Directory.Exists(strFileName) Then
                Directory.Delete(strFileName, True)
            Else
                File.Delete(strFileName)
            End If
        Next
    End Sub

    ''' <summary>
    ''' To remove any empty files (PDF and OCR normally) that are left that we
    ''' previously reserved. Fixes SPR 29193
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RemoveEmptyReservedFiles()
        Dim strFileName As String

        For Each strFileName In m_oFilesCreated
            Dim oFileInfo As New FileInfo(strFileName)
            If oFileInfo.Exists() AndAlso oFileInfo.Length = 0 Then
                oFileInfo.Delete()
            End If
        Next
    End Sub

	''' <summary>
	''' Extracts information needed during the release of files
	''' </summary>
	''' <param name="strDefaultStorageFolder"> is the default directory path to release </param>
	''' <param name="strDefaultFileName"> is the default file name as releasing </param>    
	''' <param name="strOCRStorageFolder"> is the directory path where OCR files will be released to </param>
	''' <param name="strPDFStorageFolder"> is the directory path where PDF files will be released to </param>
	''' <param name="strImageStorageFolder"> is the directory path where Image files will be released to </param>
	''' <param name="strCustomIndexStorageFolder"> is the additional directory path as releasing the index fields </param>
	''' <param name="strCustomIndexFileName"> is the custom file name of index fields </param>
	''' <param name="bSuppressImageIfPdfDetected"> true if don't want to generate the images files when PDF output is detected </param>
	''' <remarks></remarks>
	Public Sub CalculateExportSettings( _
	 ByRef oValues As Dictionary(Of String, SortedList(Of Integer, ExportValue)), _
	 ByRef strDefaultStorageFolder As String, _
	 ByRef strDefaultFileName As String, _
	 ByRef strOCRStorageFolder As String, _
	 ByRef strPDFStorageFolder As String, _
	 ByRef strImageStorageFolder As String, _
	 ByRef strCustomIndexStorageFolder As String, _
	 ByRef strCustomIndexFileName As String, _
	 ByRef bSuppressImageIfPdfDetected As Boolean, _
	 ByVal bExportImageFiles As Boolean, _
	 ByVal bExportOCRFullText As Boolean, _
	 ByVal bExportKofaxPDF As Boolean)

		Dim oDefaultFileNameManager As ExportValuesManager
		oDefaultFileNameManager = GetValuesManager(ResConstants.c_strMacroKeyDefaultFileName, oValues)
		strDefaultFileName = oDefaultFileNameManager.BuildFileName

		strDefaultStorageFolder = BuildFolderPath(ResConstants.c_strMacroKeyDefaultStorageLocation, oValues)

        '*** Always use Custom Property Path first If it has value due to substitution path changes (SPR 112776)

        If Not String.IsNullOrEmpty(m_oDocData.ImageFilePath) Then
            strImageStorageFolder = m_oDocData.ImageFilePath
        Else
            strImageStorageFolder = BuildFolderPath(ResConstants.c_strMacroKeyImageStorageFolder, oValues)
        End If

        If Not String.IsNullOrEmpty(m_oDocData.TextFilePath) Then
            strOCRStorageFolder = m_oDocData.TextFilePath
        Else
		strOCRStorageFolder = BuildFolderPath(ResConstants.c_strMacroKeyOcrStorageFolder, oValues)
        End If

        If Not String.IsNullOrEmpty(m_oDocData.KofaxPDFPath) Then
            strPDFStorageFolder = m_oDocData.KofaxPDFPath
        Else
		strPDFStorageFolder = BuildFolderPath(ResConstants.c_strMacroKeyPdfStorageFolder, oValues)
        End If

		Using oCustomPropertiesReader As New CustomPropertiesReaderWriter(m_oDocData.CustomProperties)

            '*** Get the Custom Index Folder & Custom Index File Name
            '*** Always get the path from "Index File Name" property first, due to substitution path changes (SPR 112776)

            strCustomIndexStorageFolder = oCustomPropertiesReader.IndexFileName
            If Not String.IsNullOrEmpty(strCustomIndexStorageFolder) Then
                Dim nIndex As Integer = strCustomIndexStorageFolder.LastIndexOf(c_strBackSlashCharacter)
                strCustomIndexFileName = strCustomIndexStorageFolder.Substring(nIndex + 1)
                strCustomIndexStorageFolder = strCustomIndexStorageFolder.Substring(0, nIndex + 1)
            Else
                strCustomIndexStorageFolder = BuildFolderPath(ResConstants.c_strMacroKeyIndexCustomStorageFolder, oValues)
                strCustomIndexFileName = oCustomPropertiesReader.CustomIndexFileName
            End If

			'*** Get the default storage folder for each releasing type, only if the specified
			'*** type is actually enabled.

			Dim bOCRDefaultStorage As Boolean = oCustomPropertiesReader.OcrDefaultStorage
			If bExportOCRFullText AndAlso bOCRDefaultStorage Then
				strOCRStorageFolder = strDefaultStorageFolder
			End If

			Dim bPDFDefaultStorage As Boolean = oCustomPropertiesReader.PdfDefaultStorage
			If bExportKofaxPDF AndAlso bPDFDefaultStorage Then
				strPDFStorageFolder = strDefaultStorageFolder
			End If

			Dim bImageDefaultStorage As Boolean = oCustomPropertiesReader.ImgDefaultStorage
			If bExportImageFiles AndAlso bImageDefaultStorage Then
				strImageStorageFolder = strDefaultStorageFolder
			End If

			Dim strStandardFileName As String = m_oDocData.UniqueDocumentID.ToString("X8") & ".tif"
			' VRS has an error when processing image files whose full path exceeds 128 characters. 
			' Therefore, we used the short path instead
			If (Not String.IsNullOrEmpty(strImageStorageFolder)) Then
				Dim strShortPath As New System.Text.StringBuilder(1024)
				Dim nTempVal As Integer = GetShortPathName(strImageStorageFolder, strShortPath, 1024)
				If nTempVal > 0 Then
					strImageStorageFolder = strShortPath.ToString()
				End If
			End If

			'**** Generate the image output or not 
			bSuppressImageIfPdfDetected = oCustomPropertiesReader.SuppressIfPdfDetected

		End Using
	End Sub


	''' <summary>
	''' Return the temp directory of current user.
	''' </summary>
	''' <returns> String value, the path of temp directory </returns>
	''' <remarks>
	''' It will retry 5 times every 2 seconds.
	''' </remarks>
	Public Shared Function GetTempDir() As String
		Dim strTempDir As String = System.IO.Path.GetTempPath
		Dim nRetries As Integer = 0

		While nRetries < 5
			Try
				If Not Directory.Exists(strTempDir) Then
					Directory.CreateDirectory(strTempDir)
				End If

				'Check if the temp directory is accessible
				Dim oDirInfo As New DirectoryInfo(strTempDir)

				Return strTempDir
			Catch ex As Exception
				If nRetries < 4 Then
					nRetries += 1
					System.Threading.Thread.Sleep(2000)
				Else
					Throw
				End If
			End Try
		End While

		Return strTempDir
	End Function

	''' <summary>
	''' Extracts information needed during the release of files. This information just exists in KC8 and 
	''' is not available in newer versions.
	''' </summary>
	''' <param name="bReleaseOCRToImageDir"> true if OCR/PDF files will be released to the image dir </param>
	''' <param name="eNameOption"> set based on the name option selected </param>
	''' <param name="strFinalRelDir"> contains the directory to release files </param>
	''' <param name="strIndexOptionName"> filled with the index value if index names are selected </param>
	''' <remarks>
	''' This function is a compatible version of the CalculateReleaseSetting procedure for KC versions older than 9
	''' </remarks>
	Public Sub CalculateReleaseSettingsForKC8( _
	 ByRef bReleaseOcrToImageDir As Boolean, _
	 ByRef eNameOption As ImageFilenameOptionEnum, _
	 ByRef strFinalRelDir As String, _
	 ByRef strIndexOptionName As String)

		Dim strReleaseDir As String

		'*** base directory
		strReleaseDir = Trim(m_oDocData.ImageFilePath)
		strFinalRelDir = strReleaseDir

		Using oCustomPropertiesReader As New CustomPropertiesReaderWriter(m_oDocData.CustomProperties)
			'*** where are pdf/ocr files placed?
			bReleaseOcrToImageDir = Not oCustomPropertiesReader.SeparateDirectories

			'*** get the release directory location
			Dim nFolderCount As Integer = oCustomPropertiesReader.Folders
			Dim nLoopCounter As Integer
			For nLoopCounter = 1 To nFolderCount
				Dim strIndexName As String = GetIndexValueByDestination(String.Format("Folder{0} Name", nLoopCounter))
				If Not String.IsNullOrEmpty(strIndexName) Then
					strFinalRelDir = strFinalRelDir & c_strBackSlashCharacter & strIndexName
				End If

				'*** check if the directory already exists. If it doesn't, create one
				If Directory.Exists(strFinalRelDir) = False Then
					Directory.CreateDirectory(strFinalRelDir)
				End If
			Next

			'*** ensure that the directory has a trailing backslash
			If Right(strFinalRelDir, 1) <> c_strBackSlashCharacter Then
				strFinalRelDir = strFinalRelDir & c_strBackSlashCharacter
			End If

			'*** if the image filename is based on an index field... get the value
			If eNameOption = ImageFilenameOptionEnum.PFO_Custom Then
				'*** read the value from the links table
				strIndexOptionName = GetIndexValueByDestination(c_strCPKeyFileNaming)
			End If

		End Using
	End Sub

	''' <summary>
	''' Calculates the settings for naming file, for instance: 
	''' - What if the file name is duplicated?
	''' - Naming file based on what (hexadecimal, decimal or custom)
	''' </summary>
	''' <param name="eNameOption"> set based on the name option selected </param>
	''' <param name="eDuplicateNames"> set based on selected option for duplicate filenames </param>
	''' <remarks></remarks>
	Public Sub CalculateFileNamingSettings(ByRef eNameOption As ImageFilenameOptionEnum, ByRef eDuplicateNames As DuplicateNameHandlingEnum)
		Using oCustomPropertiesReader As New CustomPropertiesReaderWriter(m_oDocData.CustomProperties)
			'*** get the filenaming option
			Select Case oCustomPropertiesReader.FileNaming
				Case c_strCPValNone
					eNameOption = ImageFilenameOptionEnum.PFO_Standard
				Case c_strCPValDecimal
					eNameOption = ImageFilenameOptionEnum.PFO_Decimal
				Case Else
					eNameOption = ImageFilenameOptionEnum.PFO_Custom
			End Select

			'*** are we using leading zeros for decimal name typs?
			Dim bLeadingZeros As Boolean
			If eNameOption = ImageFilenameOptionEnum.PFO_Decimal Then
				bLeadingZeros = oCustomPropertiesReader.LeadingZeros
			Else
				bLeadingZeros = False
			End If

			'*** and to keep everything in one spot
			If bLeadingZeros Then
				eNameOption = ImageFilenameOptionEnum.PFO_LeadingZero
			End If

			'*** get duplicate name handling
			Select Case oCustomPropertiesReader.DuplicateHandling
				Case c_strCPValError
					eDuplicateNames = DuplicateNameHandlingEnum.PDN_Error
				Case c_strCPValVersion
					eDuplicateNames = DuplicateNameHandlingEnum.PDN_Rename
				Case Else
					eDuplicateNames = DuplicateNameHandlingEnum.PDN_Replace
			End Select
		End Using
	End Sub

	Friend WriteOnly Property TheDocData() As Kofax.ReleaseLib.ReleaseData
		Set(ByVal Value As Kofax.ReleaseLib.ReleaseData)
			m_oDocData = Value
		End Set
	End Property

	''' <summary>
	''' Gets the index value based on the destination name field
	''' </summary>
	''' <param name="strDestinationName"> The field value we are searching for </param>
	''' <returns> A string containing the value of the field </returns>
	''' <remarks></remarks>
	Private Function GetIndexValueByDestination(ByVal strDestinationName As String) As String

		'*** Loop through each field and grab the value asked for
		Using oValueObjectsEnumerator As New ComEnumerator(m_oDocData.Values.GetEnumerator())
			While oValueObjectsEnumerator.MoveNext()
				Dim oValue As Kofax.ReleaseLib.Value = CType(oValueObjectsEnumerator.Current, Kofax.ReleaseLib.Value)
				If oValue.Destination = strDestinationName Then
					Return oValue.Value
				End If
			End While
		End Using

		Return String.Empty
	End Function

	''' <summary>
	''' Initialize an ExportValuesManager object from the existing values dictionary
	''' </summary>
	''' <param name="strKey">Specific values to get</param>
	''' <param name="oValues">Dictionary of all values</param>
	''' <remarks>Getting the manager class from the existing values collection improves performance</remarks>
	Private Function GetValuesManager(ByVal strKey As String, ByVal oValues As Dictionary(Of String, SortedList(Of Integer, ExportValue))) As ExportValuesManager
		Dim oSortedValues As New List(Of ExportValue)
		If oValues.ContainsKey(strKey) Then
			For Each oValue As ExportValue In oValues.Item(strKey).Values
				oSortedValues.Add(oValue)
			Next
		End If
		Return New ExportValuesManager(strKey, oSortedValues)
	End Function

	''' <summary>
	'''  Builds the folder runtime path from the Export Values where the runtime value is used.
	''' </summary>
	''' <param name="strKey">Prefix for values</param>
	''' <param name="oValues">Ordered values collection</param>
	Private Function BuildFolderPath(ByVal strKey As String, ByVal oValues As Dictionary(Of String, SortedList(Of Integer, ExportValue))) As String
		Dim strFolderPath As String = String.Empty
		Dim oValuesManager As ExportValuesManager = GetValuesManager(strKey, oValues)
		strFolderPath = oValuesManager.BuildFolderPath
		If Not String.IsNullOrEmpty(strFolderPath) AndAlso Right(strFolderPath, 1) <> c_strBackSlashCharacter Then
			strFolderPath = strFolderPath & c_strBackSlashCharacter
		End If
		Return strFolderPath
	End Function


End Class