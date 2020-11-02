'********************************************************************************
'***
'*** File       AssemblyDefinition.vb
'*** Purpose    Assembly attributes
'***
'*** (c) Copyright 2013 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

' These properties expose definition to .NET framework and also COM definition and should not be changed by build process.
<Assembly: AssemblyTitle("Kofax Export Connector for Text")> 
<Assembly: AssemblyDescription("Kofax Export Connector for Text")> 

<Assembly: ComVisibleAttribute(False)> 

<Assembly: Guid("d6c10a39-baaa-4f47-b31a-28b41dbae7b7")>  ' Locks down the GUID for this connector
'*** Use Preprocessor directive to allow some projects 
'*** to override their own AssemblyVersions
<Assembly: AssemblyVersion("1.0.0.0")> 
