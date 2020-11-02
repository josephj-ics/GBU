'********************************************************************************
'***
'*** Module: Help.vb
'*** Purpose: Adapter for using KHelpFinder
'***
'*** (c) Copyright 2009 Kofax Image Products.
'*** All rights reserved.
'********************************************************************************
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices

Module Help
    Public Sub Show(ByVal contextId As Integer)
        Dim strHelpFile As String = String.Format("Database_help/Help.html?cs=0x{0:X}", contextId)
        strHelpFile = Path.Combine(New FileInfo(Assembly.GetExecutingAssembly.Location).DirectoryName, strHelpFile)
        strHelpFile = """" + "file:///" + strHelpFile.Replace("\", "/") + """"
        Dim browser As String = GetAssociation(".html")
        Try
            Process.Start(browser, strHelpFile)
        Catch
            MessageBox.Show(String.Format("Helpfile not found: {0}", strHelpFile))
        End Try
    End Sub

    <DllImport("Shlwapi.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Private Function AssocQueryString(ByVal flags As UInt32, ByVal str As UInt32, ByVal pszAssoc As String, _
    ByVal pszExtra As String, ByVal pszOut As System.Text.StringBuilder, ByRef pcchOut As UInt32) As UInteger
    End Function

    <Flags()> _
    Enum AssocF
        Init_NoRemapCLSID = &H1
        Init_ByExeName = &H2
        Open_ByExeName = &H2
        Init_DefaultToStar = &H4
        Init_DefaultToFolder = &H8
        NoUserSettings = &H10
        NoTruncate = &H20
        Verify = &H40
        RemapRunDll = &H80
        NoFixUps = &H100
        IgnoreBaseClass = &H200
    End Enum

    Enum AssocStr
        Command = 1
        Executable
        FriendlyDocName
        FriendlyAppName
        NoOpen
        ShellNewValue
        DDECommand
        DDEIfExec
        DDEApplication
        DDETopic
    End Enum


    Private Function GetAssociation(ByVal doctype As String) As String
        Dim pcchOut As UInt32 = 0 'size of output buffer

        ' First call is to get the required size of output buffer
        AssocQueryString(CType(AssocF.NoTruncate, Integer), CType(AssocStr.Executable, Integer), doctype, "open", Nothing, pcchOut)

        ' Allocate the output buffer
        Dim pszOut As System.Text.StringBuilder = New System.Text.StringBuilder(CType(pcchOut, Integer))

        ' Get the full pathname to the program in pszOut
        AssocQueryString(CType(AssocF.NoTruncate, Integer), CType(AssocStr.Executable, Integer), doctype, "open", pszOut, pcchOut)

        Return pszout.tostring()
    End Function

End Module
