'****************************************************************************
'*   (c) Copyright Kofax Inc. 2009 All rights reserved.
'*   Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************

Imports Kofax.ReleaseLib
Imports Kofax.Connector.Common
Imports System.Collections.Generic
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text

''' <summary>
''' Writes text to the release file
''' </summary>
''' <remarks></remarks>
Friend Class IndexExport

	Private Const m_nRETRY_ATTEMPTS As Integer = 10

	''' <summary>
	''' This routine writes the batch class
	''' name, doc class name, Index Values,
	''' and image path to the text file.
	''' </summary>
	''' <param name="oReleaseData"> provides the neccesary information to write to the file </param>
	''' <param name="strImageFilePath"> is the path pointing to the image files </param>
	''' <param name="strOCRStorageFolder"> is the path of folder that contains the OCR files </param>
	''' <param name="strPDFStorageFolder"> is the path of folder that contains the PDF files </param>
	''' <param name="strDesFile"> is the destination </param>
	''' <remarks>
	''' Because the Values collection may not
	''' be in the correct order, we load the
	''' data into an array where Destination
	''' indicates the array index.  We then
	''' output the Index Values in order.
	''' </remarks>
    Sub ReleaseIndexes(ByRef oReleaseData As Kofax.ReleaseLib.ReleaseData, _
                       ByVal oValueObjectsList As SortedList(Of Integer, ExportValue), _
                       ByVal strImageFilePath As String, _
                       ByVal strOCRStorageFolder As String, ByVal strPDFStorageFolder As String, _
                       ByVal strDesFile As String)
        Dim strOutput As String
        Dim strSource As String
        Dim strValue As String
        Dim strPath As String = String.Empty


        '-- Now prepare the output for the file --

        'Start with the Batch Class Name and Document Class Name
        strOutput = String.Empty

        For Each oValue As ExportValue In oValueObjectsList.Values
            Dim strTmp As String = oValue.Value
            ' Get the Index Value source
            Select Case oValue.SourceType
                Case KfxLinkSourceType.KFX_REL_BATCHFIELD
                    strSource = String.Format("{{${0}}}", oValue.SourceName)

                Case KfxLinkSourceType.KFX_REL_INDEXFIELD
                    strSource = String.Format("{0}", oValue.SourceName)

                Case KfxLinkSourceType.KFX_REL_VARIABLE
                    strSource = String.Format("{{{0}}}", oValue.SourceName)
                    If oValue.SourceName = ExportValuesMenu.c_strExportLocationKofaxPdfFolder Then
                        strTmp = strPDFStorageFolder
                    End If
                    If oValue.SourceName = ExportValuesMenu.c_strExportLocationOcrFolder Then
                        strTmp = strOCRStorageFolder
                    End If

                Case KfxLinkSourceType.KFX_REL_TEXTCONSTANT
                    strSource = "[TEXT]"

                Case KfxLinkSourceType.KFX_REL_UNDEFINED_LINK, KfxLinkSourceType.KFX_REL_DOCUMENTID
                    ' These should never happen since we force the user
                    ' to delete all "unlinked" index values in Release Setup
                    ' and because we don't display the Document ID in the
                    ' top level link menu.  If either of these facts changes,
                    ' then this area needs to be modified.
                    strSource = String.Empty

                Case Else
                    strSource = String.Empty
            End Select

            If strSource <> String.Empty Then


                If (strSource.Equals("e_DocFileName")) Then
                    strValue = String.Format("{0}", strTmp)
                    'For GBU - PDF file name must be the ;ast field without "," based on AX specs for AppXtender
                    strOutput = strOutput.Remove(strOutput.Length - 1, 1)
                    strOutput = String.Format("{0}{1}", strOutput, strValue)
                Else
                    ' Get the Index Value
                    strValue = String.Format("{0}", strTmp)
                    ' Append the index source and value
                    strOutput = String.Format("{0}{1},", strOutput, strValue)
                End If
            End If
        Next

        ' Get the path to the image file.
        'strPath = String.Format("""{0}""", strImageFilePath)

        'strOutput = strOutput.Remove(strOutput.Length - 1, 1)

        ' Append the image path
        'strOutput = String.Concat(strOutput, strPath)

        ' Now we are ready to write to the file.  Open the file
        ' for Append which will create it if it doesn't exist.
        ' Also place a Read/Write Lock so no other process can
        ' access the file while we have it open.
        Dim oEncoding As Encoding
        Using oCustomPropertiesReader As New CustomPropertiesReaderWriter(oReleaseData.CustomProperties)
            oEncoding = oCustomPropertiesReader.Encoding
        End Using

        ' Make sure that the directory tree containing desFile exists 
        Dim strFolderPath As String = strDesFile.Substring(0, strDesFile.LastIndexOf(c_strBackSlashCharacter))
        If Not Directory.Exists(strFolderPath) Then
            Directory.CreateDirectory(strFolderPath)
        End If

        '*** Enclose the output operation in a retry loop in case the file has been
        '*** locked by another process. This allows export to be more robust.
        For nAttempts As Integer = 0 To m_nRETRY_ATTEMPTS
            Try
                Using oFileStream As New FileStream(strDesFile, FileMode.Append, FileAccess.Write, FileShare.None)
                    Using oStreamWriter As New StreamWriter(oFileStream, oEncoding)
                        ' Write the output to the file and close the file
                        oStreamWriter.WriteLine(strOutput)
                        oStreamWriter.Flush()
                        Exit For
                    End Using
                End Using
            Catch ex As IOException
                '*** Test the HRESULT for a file-lock. If so, try again if the 
                '*** if the number of retires hasn't been reached. If it has, throw
                '*** the exception.
                If Marshal.GetHRForException(ex) = &H80070020 Then
                    If nAttempts = m_nRETRY_ATTEMPTS Then
                        Throw ex
                    End If
                Else
                    '*** If the exception was anything other than a file-lock, re-throw
                    Throw ex
                End If
            End Try
        Next

    End Sub
End Class