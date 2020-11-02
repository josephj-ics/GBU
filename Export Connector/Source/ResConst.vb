'****************************************************************************
'*	(c) Copyright Kofax, Inc. 2009. All rights reserved.
'*	Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************

Module ResConstants

    '==========================
    ' String Table References
    '==========================
    Public Const c_shMsgUltimateFileExists As Short = 7008

    '*** ImageName additions
    Public Const c_strCPValTrue As String = "True"
    Public Const c_strCPValFalse As String = "False"

    Public Const c_strCPKeyFileNaming As String = "Plus Filenaming"
    Public Const c_strCPValNone As String = "None" ' this means standard selection
    Public Const c_strCPValDecimal As String = "Decimal"
    Public Const c_strCPValCustom As String = "Index"

    '*** holds the count of the number of directories (not including base)
    Public Const c_strCPValError As String = "Error if Dup"
    Public Const c_strCPValVersion As String = "Version if Dup"
    Public Const c_strCPValReplace As String = "Replace if Dup"

    '*** End ImageName additions
    

    '*** Constants for extensions of image file formats
    Public Const c_strIFTif As String = "tif"
    Public Const c_strIFJpg As String = "jpg"
    Public Const c_strIFPcx As String = "pcx"
    Public Const c_strCPValPdf As String = "pdf"
    Public Const c_strCPValTxt As String = "txt"
    

    '===================
    ' Help Context IDs
    '===================
	Public Const c_nTabs_First_HelpID As Integer = &H26201

    '===================
    ' Keys to manage the macros
    '===================
    Public Const c_strMacroKeyDefaultStorageLocation As String = "Default Storage Folder"
    Public Const c_strMacroKeyDefaultFileName As String = "Default File Name"
    Public Const c_strMacroKeyOcrStorageFolder As String = "OCR Storage Folder"
    Public Const c_strMacroKeyPdfStorageFolder As String = "PDF Storage Folder"
    Public Const c_strMacroKeyImageStorageFolder As String = "IMG Storage Folder"
    Public Const c_strMacroKeyIndexCustomStorageFolder As String = "Index Custom Storage Folder"

    '===================
    ' Constants for some special characters
    '===================
    Public Const c_strBackSlashCharacter As String = "\"
    Public Const c_strDoubleBackSlashCharacter As String = "\\"
    Public Const c_strDotCharacter As String = "."

	'==================
	' Constants for product name 
	'==================
	Public Const c_strShortProductName As String = "Text Export Connector"
End Module