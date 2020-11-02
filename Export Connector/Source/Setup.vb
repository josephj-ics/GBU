'****************************************************************************
'*   (c) Copyright Kofax Inc. 2009 All rights reserved.
'*   Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************

Imports Kofax.ReleaseLib
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports Kofax.Connector.Common

''' <summary>
''' An interface that a release setup script class must implement
''' </summary>
''' <remarks></remarks>
<ComVisible(True)> _
<Guid("71C2714F-44FE-4044-A23C-F402077975E8")> _
<InterfaceType(ComInterfaceType.InterfaceIsIDispatch)> _
Public Interface IKfxReleaseSetupScript
    Property SetupData() As Kofax.ReleaseLib.ReleaseSetupData
    Function CloseScript() As Kofax.ReleaseLib.KfxReturnValue
    Function ActionEvent( _
        ByRef intActionID As Kofax.ReleaseLib.KfxActionValue, _
        ByRef strData1 As String, _
        ByRef strData2 As String) _
        As Kofax.ReleaseLib.KfxReturnValue

    Function OpenScript() As Kofax.ReleaseLib.KfxReturnValue
    Function RunUI() As Kofax.ReleaseLib.KfxReturnValue
End Interface

''' <summary>
''' ReleaseSetupData object is set by the Release
''' Setup Controller.  This object is used during
''' the document type setup process.  It will contain
''' all of the information and interfaces you need to
''' define a document type's release process.
''' </summary>
''' <remarks></remarks>
<ComVisible(True)>
<ProgId("TextRel.kfxreleasesetupscript")>
<Guid("0B11F2CD-4EC1-4ba6-A399-E84D993B0284")> _
<ClassInterface(ClassInterfaceType.None)> _
Public Class KfxReleaseSetupScript
	Implements IKfxReleaseSetupScript, IDisposable

	Private m_oSetupData As Kofax.ReleaseLib.ReleaseSetupData

	Private m_eSelectedPage As SetupFormTabEnum = SetupFormTabEnum.DefaultStoragePage

	Private m_bDirty As Boolean = False


	''' <summary>
	''' Property of the ReleaseSetupData object
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property SetupData() As Kofax.ReleaseLib.ReleaseSetupData Implements IKfxReleaseSetupScript.SetupData
		Get
			Return m_oSetupData
		End Get
		Set(ByVal value As Kofax.ReleaseLib.ReleaseSetupData)
			m_oSetupData = value
		End Set
	End Property

	''' <summary>
	''' Script release point. Perform any
	''' necessary cleanup such as releasing
	''' resources, etc.
	''' </summary>
	''' <returns>
	''' One of the following:
	'''    KFX_REL_SUCCESS, KFX_REL_ERROR,
	'''    KFX_REL_FATALERROR, KFX_REL_REINIT
	'''    KFX_REL_DOCCLASSERROR,
	''' </returns>
	''' <remarks>
	''' Called by Release Setup Controller
	''' once just before the script object
	''' is released.
	''' </remarks>
	Public Function CloseScript() As Kofax.ReleaseLib.KfxReturnValue Implements IKfxReleaseSetupScript.CloseScript
		CommonErrorHandler.DataObject = Nothing
		Dispose()
		Return Kofax.ReleaseLib.KfxReturnValue.KFX_REL_SUCCESS
	End Function

	''' <summary>
	''' This method allows the setup script
	''' to respond to various events in the
	''' Administration module.  The script
	''' has the opportunity to make any
	''' necessary changes to the release
	''' settings in the ReleaseSetupData
	''' object or any other external data
	''' source.
	''' </summary>
	''' <param name="oActionID"> ID of the event </param>
	''' <param name="strData1"> Action parameter 1 </param>
	''' <param name="strData2"> Action parameter 2 </param>
	''' <returns>
	''' One of the following:
	'''    KFX_REL_SUCCESS, KFX_REL_ERROR,
	'''    or KFX_REL_UNSUPPORTED
	''' </returns>
	''' <remarks>
	''' Refer to the documentation for a list
	''' of actions and associated parameters.
	''' </remarks>
	Public Function ActionEvent( _
	 ByRef oActionID As KfxActionValue, _
	 ByRef strData1 As String, _
	 ByRef strData2 As String) _
	 As KfxReturnValue _
	 Implements IKfxReleaseSetupScript.ActionEvent

		Static bShowUI As Boolean

		ActionEvent = Kofax.ReleaseLib.KfxReturnValue.KFX_REL_SUCCESS

		If m_oSetupData.New = 0 Then
			Select Case oActionID

				Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_START
					' A new series of Action Events is
					' starting so initialize any flags
					bShowUI = False

				Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_INDEXFIELD_DELETE
					' Delete any links to this Index Field
					ActionEvent = RemoveTheLink(strData1, m_oSetupData, Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_INDEXFIELD, bShowUI)
					If bShowUI Then
						m_eSelectedPage = SetupFormTabEnum.DefaultStoragePage
						m_bDirty = True
					End If

				Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_INDEXFIELD_INSERT
					' Set the flag to display the UI so the user
					' can create a link to the new Index Field
					bShowUI = True
					m_eSelectedPage = SetupFormTabEnum.IndexStoragePage	 'index storage folder

				Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_INDEXFIELD_RENAME
					' If this Index Field is used in a link,
					' change the link to reflect the new name.
					Using oLinksEnumerator As New ComEnumerator(m_oSetupData.Links.GetEnumerator)
						While oLinksEnumerator.MoveNext()
							Dim oLink As Kofax.ReleaseLib.Link = CType(oLinksEnumerator.Current, Kofax.ReleaseLib.Link)
							Using New ComDisposer(oLink)
								If oLink.Source = strData1 And oLink.SourceType = Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_INDEXFIELD Then
									oLink.Source = strData2
								End If
							End Using
						End While
					End Using

				Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_BATCHFIELD_DELETE
					' Delete any links to this Batch Field
					ActionEvent = RemoveTheLink(strData1, m_oSetupData, Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_BATCHFIELD, bShowUI)
					If bShowUI Then
						m_eSelectedPage = SetupFormTabEnum.DefaultStoragePage
						m_bDirty = True
					End If

				Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_BATCHFIELD_INSERT
					' Set the flag to display the UI so the user
					' can create a link to the new Batch Field
					bShowUI = True
					m_eSelectedPage = SetupFormTabEnum.IndexStoragePage

				Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_BATCHFIELD_RENAME
					' If this Batch Field is used in a link,
					' change the link to reflect the new name.
					Using oLinksEnumerator As New ComEnumerator(m_oSetupData.Links.GetEnumerator)
						While oLinksEnumerator.MoveNext()
							Dim oLink As Kofax.ReleaseLib.Link = CType(oLinksEnumerator.Current, Kofax.ReleaseLib.Link)
							Using New ComDisposer(oLink)
								If oLink.Source = strData1 And oLink.SourceType = Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_BATCHFIELD Then
									oLink.Source = strData2
								End If
							End Using
						End While
					End Using

				Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_RELEASESETUP_DELETE
					' Nothing to do

				Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_IMPORT
					ActionEvent = RunUI()

				Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_BATCHCLASS_RENAME
					' Nothing to do

				Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_DOCCLASS_RENAME
					' Nothing to do

				Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_PUBLISH_CHECK

					' Calculate string length for padding purpose
					Dim nBatchLen As Integer = Len(My.Resources.TXT_BATCH_CLASS)
					Dim nDocLen As Integer = Len(My.Resources.TXT_DOCUMENT_CLASS)
					Dim nNameLen As Integer = Len(My.Resources.TXT_NAME)
					Dim nMaxLen As Integer = nBatchLen
					nMaxLen = Math.Max(nDocLen, nMaxLen)
					nMaxLen = Math.Max(nNameLen, nMaxLen)

					' Pad each string to the max length with spaces and append a tab character since padding
					' with spaces don't quite line up.  This ensures that all strings have equal length, and
					' works with localized strings.
					Dim strMsgHeader As String = _
					 String.Format("{2}{3}{1}{4}{0}{5}{6}{1}{7}{0}{8}{9}{1}{10}{0}{0}", _
					 vbCrLf, vbTab, _
					 My.Resources.TXT_BATCH_CLASS, _
					 Space(nMaxLen - nBatchLen), _
					 m_oSetupData.BatchClassName, _
					 My.Resources.TXT_DOCUMENT_CLASS, _
					 Space(nMaxLen - nDocLen), _
					 m_oSetupData.DocClassName, _
					 My.Resources.TXT_NAME.Replace("&", String.Empty), _
					 Space(nMaxLen - nNameLen), _
					 m_oSetupData.Name)

					Using oCustomPropertiesReader As New CustomPropertiesReaderWriter(m_oSetupData.CustomProperties)
						' check to see if the index name option is selected but the index name
						' selected is empty... the user must select a value.
						' Actually, it's no longer used in KC9. However, it is still kept to be compatible with KC8
						If Not oCustomPropertiesReader.IsNewVersion Then
						If oCustomPropertiesReader.FileNaming = c_strCPValCustom Then
							If SetupForm Is Nothing Then
								SetupForm = New MainSetupForm()
							End If
							Try
								If Not String.IsNullOrEmpty(oCustomPropertiesReader.IndexFileName) Then
									SetupForm.GetLinksValue(m_oSetupData.Links, c_strCPKeyFileNaming, String.Empty, False)
								End If
							Catch ex As Exception
								m_oSetupData.LogError(-1, 0, 0, My.Resources.MSG_ERROR_INDEX_NAMING, c_strShortProductName, 0)
								Return Kofax.ReleaseLib.KfxReturnValue.KFX_REL_ERROR
							End Try
						End If
						End If

						'*** check that UseOriginalFileNames option works with the current file name options
						'*** spr 28610
						If m_oSetupData.UseOriginalFileNames <> 0 Then

                            '*** and second, that the file naming option is set to "standard"
                            If (String.Compare(oCustomPropertiesReader.FileNaming, "None", True) <> 0) Then
                                '*** if there is a conflict, write the error to the log file and set error condition
                                ActionEvent = Kofax.ReleaseLib.KfxReturnValue.KFX_REL_ERROR
								m_oSetupData.LogError(-1, 0, 0, My.Resources.MSG_ORIGINAL_FILE_NAME_CONFLICT, c_strShortProductName, 0)
                            End If

						End If
					End Using


				Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_UPGRADE
					' The default release scripts do not
					' support the UPGRADE action at this time
					ActionEvent = Kofax.ReleaseLib.KfxReturnValue.KFX_REL_UNSUPPORTED

				Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_END
					' Check if the flag was set to display the
					' UI by any Action Events in the series.
					If bShowUI = True Then
						bShowUI = False
						ActionEvent = RunUI()
					End If

				Case Else
					ActionEvent = Kofax.ReleaseLib.KfxReturnValue.KFX_REL_UNSUPPORTED

			End Select

			'Save our changes.
			m_oSetupData.Apply()
		End If
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
	''' once when the script object is loaded
	''' and before a call to RunUI() or
	''' ActionEvent() is made.
	''' </remarks>
	Public Function OpenScript() As Kofax.ReleaseLib.KfxReturnValue Implements IKfxReleaseSetupScript.OpenScript
		CommonErrorHandler.DataObject = m_oSetupData
		CommonErrorHandler.Title = My.Resources.TXT_RELEASE_SETUP_ERROR
		Return Kofax.ReleaseLib.KfxReturnValue.KFX_REL_SUCCESS
	End Function

	''' <summary>
	''' User interface display point.  This
	''' method is called by the Release Setup
	''' Controller to display the setup form
	''' specific to this script.
	''' </summary>
	''' <returns> Always KFX_REL_SUCCESS </returns>
	''' <remarks>
	''' Called by Release Setup Controller
	''' when the Administration module asks
	''' to run the script and whenever a
	''' Batch Field or Index Field is inserted.
	''' </remarks>
	Public Function RunUI() As Kofax.ReleaseLib.KfxReturnValue Implements IKfxReleaseSetupScript.RunUI
		SetupForm = New MainSetupForm()
		SetupForm.tabText.SelectedIndex = m_eSelectedPage
		SetupForm.Dirty = m_bDirty

		' Restores to the default configuration
		m_eSelectedPage = SetupFormTabEnum.DefaultStoragePage
		m_bDirty = False

		SetupForm.ShowForm(m_oSetupData)
		Return Kofax.ReleaseLib.KfxReturnValue.KFX_REL_SUCCESS
	End Function

	''' <summary>
	''' Deletes links from the release script
	''' when the associated Index Field or
	''' Batch Field is removed from the
	''' Document Class or Batch Class
	''' respectively in the Administration
	''' module.
	''' </summary>
	''' <param name="strData1"> the link source name </param>
	''' <param name="oSetupData"> the SetupData object </param>
	''' <param name="nLinkType"> the link source type </param>
	''' <param name="bShowUI"> output param to bring up UI </param>
	''' <returns> KFX_REL_SUCCESS or KFX_REL_ERROR </returns>
	''' <remarks>
	''' Called by ActionEvent() when the
	''' action specified is either
	''' KFX_REL_INDEXFIELD_DELETE or
	''' KFX_REL_BATCHFIELD_DELETE.
	''' 
	''' *** IMPORTANT ***
	''' The Text Release script stores the
	''' sequence in the Destination member
	''' of each link.  Deleting a link means
	''' that each subsequent link must be
	''' modified to indicate its new sequence.
	'''
	''' Multiple links may be deleted at once
	''' if a Batch Field or Index Field is
	''' used multiple times.  To resequence
	''' the links that we wish to save, we
	''' copy them to a temporary array,
	''' delete the entire collection, then
	''' re-add the saved links to the
	''' collection with their new sequence.
	'''
	''' The values are stored in the array in the following order:
	''' Sequenced values, PDF entries [,Plus Filename] [,folder values]
	'''
	''' There are two other values that are stored in the links table: Folder x Name
	''' (stores the custom directories under the base release directory where files
	''' will be released) and Plus Filenaming (the index field the image file should
	''' be named). If the value that is being removed from the links table matches with
	''' either of these additional types, they need to be removed. Folder values may
	''' need to be resequenced and the total folder count (stored in the custom properties
	''' table) needs to be adjusted. If the Plus Filenaming value is to be removed, the
	''' UI needs to be displayed.
	''' </remarks>
	Private Function RemoveTheLink(ByRef strData1 As String, ByRef oSetupData As Kofax.ReleaseLib.ReleaseSetupData, ByRef nLinkType As KfxLinkSourceType, ByRef bShowUI As Boolean) As KfxReturnValue

		'*** Now using a sorted list for each type so we can better control the final
		'*** order without extra loops and overly complicated index value calculations.
		Dim oSavedSequencedLinksList As New SortedList(Of Integer, Kofax.ReleaseLib.Link)
		Dim oSavedPdfLinksList As New SortedList(Of Integer, Kofax.ReleaseLib.Link)
		Dim osSavedFilenamingLinksList As New SortedList(Of Integer, Kofax.ReleaseLib.Link)
		Dim oSavedFolderLinksList As New SortedList(Of Integer, Kofax.ReleaseLib.Link)
		Dim oSavedMacroLinksList As New SortedList(Of Integer, Kofax.ReleaseLib.Link)

		'*** Index keys for the above lists.  Do not need one for the sequenced
		'*** links because we use their sequence number as the key.
		Dim nPdfIndex As Integer = 0
		Dim nFilenameIndex As Integer = 0
		Dim nFolderIndex As Integer = 0
		Dim nMacroIndex As Integer = 0

		'*** Assume success to start
		RemoveTheLink = Kofax.ReleaseLib.KfxReturnValue.KFX_REL_SUCCESS

		' Do not reset bShowUI here because this can be called multiple times between
		' action start and end.  It is reset in the ActionEvent start case.

		'*** Leave if nothing to do
		If oSetupData.Links.Count = 0 Then
			Exit Function
		End If

		Using oCustomPropertiesReaderWriter As New CustomPropertiesReaderWriter(SetupData.CustomProperties)

			'*** Loop through all of the links in the SetupData collection and 
			'*** save off the ones we intend to keep.
			Using oLinksEnumerator As New ComEnumerator(SetupData.Links.GetEnumerator)
				While oLinksEnumerator.MoveNext()
					Dim oLink As Kofax.ReleaseLib.Link = CType(oLinksEnumerator.Current, Kofax.ReleaseLib.Link)

					Dim bDeleteLink As Boolean = (oLink.Source = strData1 And oLink.SourceType = nLinkType)

					If Not bDeleteLink Then
						If oLink.Destination.IndexOf(c_strCPValPdf) = 0 Then
							'*** PDF link
							nPdfIndex = nPdfIndex + 1
							oSavedPdfLinksList(nPdfIndex) = oLink

						ElseIf oLink.Destination.IndexOf("Folder") = 0 Then
							'*** Folder link
							nFolderIndex = nFolderIndex + 1
							oSavedFolderLinksList(nFolderIndex) = oLink

						ElseIf Left(oLink.Destination, 15) = c_strCPKeyFileNaming Then
							'*** Filenaming link
							nFilenameIndex = nFilenameIndex + 1
							osSavedFilenamingLinksList(nFilenameIndex) = oLink

						ElseIf oLink.Destination.StartsWith(ResConstants.c_strMacroKeyDefaultStorageLocation) OrElse _
						 oLink.Destination.StartsWith(ResConstants.c_strMacroKeyDefaultFileName) OrElse _
						 oLink.Destination.StartsWith(ResConstants.c_strMacroKeyIndexCustomStorageFolder) OrElse _
						 oLink.Destination.StartsWith(ResConstants.c_strMacroKeyImageStorageFolder) OrElse _
						 oLink.Destination.StartsWith(ResConstants.c_strMacroKeyOcrStorageFolder) OrElse _
						 oLink.Destination.StartsWith(ResConstants.c_strMacroKeyPdfStorageFolder) Then
							'**** Macro link
							nMacroIndex += 1
							oSavedMacroLinksList(nMacroIndex) = oLink

						Else
							'*** Sequenced link
							'*** Use sequence (destination) as the index key
							Dim nIdx As Integer = CInt(oLink.Destination)
							oSavedSequencedLinksList(nIdx) = oLink

						End If
					Else
						'*** Fix SPR00049489 - KCEC-Text: Setup does not automatically open when deleting linked Folder Index Fields
						bShowUI = True

						'*** Show UI if the index field used in custom naming is deleted
						'If Left(oLink.Destination, 15) = c_strCPKeyFileNaming OrElse oLink.Destination.StartsWith(c_strMacroKeyDefaultFileName) Then
						'    bShowUI = True
						'End If
					End If

				End While
			End Using


			'*** Delete the entire collection
			oSetupData.Links.RemoveAll()

			'*** Add all the links back into the collection in the following order:

			'*** Sequenced links
			Dim nSequenceIdx As Integer = 0
			For Each oSavedLink As Kofax.ReleaseLib.Link In oSavedSequencedLinksList.Values
				oSetupData.Links.Add(oSavedLink.Source, oSavedLink.SourceType, CStr(nSequenceIdx))
				nSequenceIdx = nSequenceIdx + 1
				'Release each saved Link object when done.
				Marshal.FinalReleaseComObject(oSavedLink)
			Next

			'*** Followed by PDF links
			For Each oSavedLink As Kofax.ReleaseLib.Link In oSavedPdfLinksList.Values
				oSetupData.Links.Add(oSavedLink.Source, oSavedLink.SourceType, oSavedLink.Destination)
				'Release each saved Link object when done.
				Marshal.FinalReleaseComObject(oSavedLink)
			Next

			'*** Followed by Filenaming links
			For Each oSavedLink As Kofax.ReleaseLib.Link In osSavedFilenamingLinksList.Values
				oSetupData.Links.Add(oSavedLink.Source, oSavedLink.SourceType, oSavedLink.Destination)
				'Release each saved Link object when done.
				Marshal.FinalReleaseComObject(oSavedLink)
			Next

			'*** Followed by macro links
			For Each oSavedLink As Kofax.ReleaseLib.Link In oSavedMacroLinksList.Values
				oSetupData.Links.Add(oSavedLink.Source, oSavedLink.SourceType, oSavedLink.Destination)
				'Release each saved Link object when done.
				Marshal.FinalReleaseComObject(oSavedLink)
			Next

			'*** Followed by Folder links
			Dim nFolderSequenceIdx As Integer = 1
			For Each oSavedLink As Kofax.ReleaseLib.Link In oSavedFolderLinksList.Values
				oSetupData.Links.Add(oSavedLink.Source, oSavedLink.SourceType, String.Format("Folder{0} Name", nFolderSequenceIdx))
				nFolderSequenceIdx = nFolderSequenceIdx + 1
				'Release each saved Link object when done.
				Marshal.FinalReleaseComObject(oSavedLink)
			Next

		End Using
	End Function

	''' <summary>
	''' Disposes ReleaseSetupData object.
	''' </summary>
	''' <remarks></remarks>
	Public Sub Dispose() Implements IDisposable.Dispose
		If m_oSetupData IsNot Nothing Then
			Marshal.FinalReleaseComObject(m_oSetupData.BatchFields)
			Marshal.FinalReleaseComObject(m_oSetupData.CustomProperties)
			Marshal.FinalReleaseComObject(m_oSetupData.IndexFields)
			Marshal.FinalReleaseComObject(m_oSetupData.Links)
			Marshal.FinalReleaseComObject(m_oSetupData)
			m_oSetupData = Nothing
		End If
	End Sub

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'Set UI Language.
		Kofax.Connector.Common.GeneralUtils.ApplyLocalizationIfPossible()
    End Sub
End Class