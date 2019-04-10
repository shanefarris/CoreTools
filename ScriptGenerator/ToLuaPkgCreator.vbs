' tolua++ -n AI -o AI_lua.cpp -H AI_lua.h ./pkg/AI
' tolua++ -n Core -o Core_lua.cpp -H Core_lua.h ./pkg/Core
' tolua++ -n Audio -o Audio_lua.cpp -H Audio_lua.h ./pkg/Audio
' tolua++ -n IO -o IO_lua.cpp -H IO_lua.h ./pkg/IO
' tolua++ -n Overlays -o Overlays_lua.cpp -H Overlays_lua.h ./pkg/Overlays
' tolua++ -n SceneLoader -o SceneLoader_lua.cpp -H SceneLoader_lua.h ./pkg/SceneLoader

Set FSO = CreateObject("Scripting.FileSystemObject")
Dim txtFileName
'Dim IncludesDir: IncludesDir = array( "../../Include" )
objextension = "h"
HeaderListIndex = 0
Dim HeaderList()
Dim Parentfolder

Dim IncludesDir: IncludesDir = array(   "../../Include/AgentSmith", _
										"../../Include/Audio", _
										"../../Include/Ai", _
										"../../Include/Core", _
										"../../Include/Core/Overlays", _
										"../../Include/FileDatabase", _
										"../../Include/IO", _
										"../../Include/Nature", _
										"../../Include/Network", _
										"../../Include/RuntimeEditor", _
										"../../Include/SceneLoader")

Sub Create(Dir)

	Wscript.Echo "Creating " & 	dir & vbcrlf
	Dim WSH, objDirectory, objFile, TheFiles

	Set WSH = CreateObject("Wscript.Shell")
	Set objDirectory = FSO.GetFolder(Dir)
	Set TheFiles = objDirectory.Files

	Parentfolder =  mid(fso.GetFolder(objDirectory), InstrRev(fso.GetFolder(objDirectory),"\",-1,0) + 1, len(fso.GetFolder(objDirectory)) - InstrRev(fso.GetFolder(objDirectory),"\",-1,0))
		
	' Remove pkg file if it already exist (we append to it) 
	If FSO.FileExists (Parentfolder & ".pkg") then
		FSO.DeleteFile Parentfolder & ".pkg"
	End If
	
	If FSO.FileExists ("FileNames " & Parentfolder & ".txt") then
		FSO.DeleteFile "FileNames " & Parentfolder & ".txt"
	End If

	WorkWithSubFolders objDirectory

End Sub

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Sub WorkWithSubFolders(objDirectory)
    Dim MoreFolders, TempFolder

    ListFilesWithExtension objDirectory
    Set MoreFolders = objDirectory.SubFolders

    For Each TempFolder In MoreFolders
		WorkWithSubFolders TempFolder
    Next

End Sub

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Sub ListFilesWithExtension(objDirectory)
    Dim TheFiles
	
	If InStr(objDirectory, ".svn") > 0 Then
		Exit Sub
	End If
	
	Wscript.Echo("Package: " & Parentfolder)
	
    Set TheFiles = objDirectory.Files
    For Each objFile in TheFiles
        strExt = fso.GetExtensionName(objFile.Path)
		
        If (strExt = objextension) Then
			HeaderListIndex = HeaderListIndex + 1
			Redim Preserve HeaderList(HeaderListIndex)
            HeaderList(HeaderListIndex) = objFile.Path
			Wscript.Echo("Adding file: " & objFile.Name)
		Else
			Wscript.Echo("Not adding file: " & objFile.Name)
        end if
    Next
End Sub

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Sub ParseHeaders
	Set fso2 = CreateObject("Scripting.FileSystemObject")
	
	For each file in HeaderList
		Wscript.Echo "Parsing: " & file
		if fso2.FileExists(Replace(file, """", "")) = False then
			Wscript.Echo file & " does not exist or is still opened."
		Else
			Dim f: set f = fso2.OpenTextFile(file, 1)
			Dim s: s = f.ReadAll()
			GetFile = s
			newdata = ParseSingleHeader(GetFile)
			CreatePackage newdata
			f.Close
		end if
		
	Next
	
End Sub

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Function ParseSingleHeader(Data)
	' Remove unreconized text
	Data = replace(Data, "CORE_EXPORT", "", 1, -1, 1)
	Data = replace(Data, "NATURE_EXPORT", "", 1, -1, 1)
	Data = replace(Data, "__declspec(dllexport)", "", 1, -1, 1)
	Data = replace(Data, "virtual", "", 1, -1, 1)
	Data = replace(Data, "f32", "float", 1, -1, 1)
	Data = replace(Data, "s32", "int", 1, -1, 1)
	Data = replace(Data, "u32", "unsigned int", 1, -1, 1)
	Data = replace(Data, "c8", "char", 1, -1, 1)
	Data = replace(Data, "s8", "signed char", 1, -1, 1)
	Data = replace(Data, "u8", "unsigned char", 1, -1, 1)
	Data = replace(Data, "{ }", ";", 1, -1, 1)
	Data = replace(Data, "{}", ";", 1, -1, 1)
	ParseSingleHeader = Data
End Function

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Sub CreatePackage(Data)
	strFile = Parentfolder & ".pkg"

	' Create the File System Object
	Set objFSO = CreateObject("Scripting.FileSystemObject")

	If objFSO.FileExists(strFile) Then
	Else
	   Set objFile = objFSO.CreateTextFile(strFile)
	End If 

	set objFile = nothing
	set objFolder = nothing
	' OpenTextFile Method needs a Const value
	' ForAppending = 8 ForReading = 1, ForWriting = 2

	Set objTextFile = objFSO.OpenTextFile (strFile, 8, True)

	Wscript.Echo("Writing to file: " & strFile)
	' Writes strText every time you run this VBScript
	objTextFile.WriteLine(Data)
	objTextFile.Close

End Sub

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Sub ExecuteTolua
	Set objShell = CreateObject("WScript.Shell")
	Set objWshScriptExec = objShell.Exec("./tolua++.exe " & Parentfolder & " -o " & Parentfolder & ".cpp -H " & Parentfolder & ".h ./" & Parentfolder)
	Wscript.Echo("Exectuing: ./tolua++.exe -n " & Parentfolder & " -o " & Parentfolder & ".cpp -H " & Parentfolder & ".h ./" & Parentfolder)
	
	Set objStdOut = objWshScriptExec.StdOut
	While Not objStdOut.AtEndOfStream
       	Wscript.Echo objStdOut.ReadLine
	Wend
	
End Sub

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
For Each dir in IncludesDir
	' Create the list
	Create dir
	
	' Parse each file
	ParseHeaders
	
	' Run the toLua++ exe
	'ExecuteTolua
	
	' Clear our list
	Erase HeaderList
	HeaderListIndex = 0
Next

Wscript.Quit