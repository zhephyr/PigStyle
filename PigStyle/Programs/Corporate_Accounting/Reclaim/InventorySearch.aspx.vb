﻿Imports System.DirectoryServices
Imports System.Windows.Forms
Imports System.Data.SqlClient
Imports System.Web.Script.Serialization
Imports PWutilities.PWutilities

Public Class InventorySearch
    Inherits System.Web.UI.Page

    Structure YearItems
        Dim dateYear As Integer
        Dim items As Integer
    End Structure

    Structure BoxType
        Dim id As String
        Dim name As String
    End Structure

    Structure BoxInfo
        Dim type As String
        Dim store As String
        Dim storageDate As String
        Dim outDate As String
        Dim frontOrBack As String
        Dim position As String
        Dim boxID As String
    End Structure

    Structure PalletInfo
        Dim palletLoc As String
        Dim frontPic As String
        Dim backPic As String
        Dim boxF01 As BoxInfo
        Dim boxF02 As BoxInfo
        Dim boxF03 As BoxInfo
        Dim boxF04 As BoxInfo
        Dim boxF05 As BoxInfo
        Dim boxF06 As BoxInfo
        Dim boxF07 As BoxInfo
        Dim boxF08 As BoxInfo
        Dim boxF09 As BoxInfo
        Dim boxF10 As BoxInfo
        Dim boxF11 As BoxInfo
        Dim boxF12 As BoxInfo
        Dim boxB01 As BoxInfo
        Dim boxB02 As BoxInfo
        Dim boxB03 As BoxInfo
        Dim boxB04 As BoxInfo
        Dim boxB05 As BoxInfo
        Dim boxB06 As BoxInfo
        Dim boxB07 As BoxInfo
        Dim boxB08 As BoxInfo
        Dim boxB09 As BoxInfo
        Dim boxB10 As BoxInfo
        Dim boxB11 As BoxInfo
        Dim boxB12 As BoxInfo
    End Structure

    Public dirGroups As List(Of String)
    Public permissionType As String

    Public recCount As Integer
    Public checkedRecs As Integer

    Shared connectionStr As String = "Server=VG-UNITEDDB; Database=freshdb; User Id=freshuser; Password=Fresh2012;"
    Shared connection As SqlConnection = New SqlConnection(connectionStr)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        dirGroups = GetGroups("LDAP://pwmdc4/DC=sso,DC=fb,DC=com", HttpContext.Current.User.Identity.Name)

        GetRecCount()
        PopulateYearList()
    End Sub

    Private Function GetGroups(ByVal _path As String, ByVal username As String) As List(Of String)
        Dim Groups As New List(Of String)
        Dim dirEntry As New System.DirectoryServices.DirectoryEntry(_path)
        Dim dirSearcher As New DirectorySearcher(dirEntry)
        Dim userID As String = Split(username, "\")(1)
        dirSearcher.Filter = "(&(objectcategory=user)(SAMAccountName=" & userID & "))"
        dirSearcher.PropertiesToLoad.Add("memberOf")
        Dim propCount As Integer
        Try
            Dim dirSearchResults As SearchResult = dirSearcher.FindOne()
            propCount = dirSearchResults.Properties("memberOf").Count
            Dim dn As String
            Dim equalsIndex As String
            Dim commaIndex As String
            For i As Integer = 0 To propCount - 1
                dn = dirSearchResults.Properties("memberOf")(i)
                equalsIndex = dn.IndexOf("=", 1)
                commaIndex = dn.IndexOf(",", 1)
                If equalsIndex = -1 Then
                    Return Nothing
                End If
                If Not Groups.Contains(dn.Substring((equalsIndex + 1), _
    (commaIndex - equalsIndex) - 1)) Then
                    Groups.Add(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1))
                End If
            Next
        Catch ex As Exception
            If ex.GetType Is GetType(System.NullReferenceException) Then
                Dim msg As String
                msg = "<script language='javascript'>"
                msg += "alert('Selected user is not a member of any groups at this time." & username & "');"
                msg += "<" & "/script>"
                Response.Write(msg)
            Else
                Dim msg As String
                msg = "<script language='javascript'>"
                msg += "alert('" & ex.Message.ToString & "');"
                msg += "<" & "/script>"
                Response.Write(msg)
            End If
        End Try
        Return Groups
    End Function

    Private Sub GetRecCount()
        Dim connectionStr As String = "Server=VG-UNITEDDB; Database=freshdb; User Id=freshuser; Password=Fresh2012;"
        Dim connection As SqlConnection = New SqlConnection(connectionStr)
        connection.Open()

        Dim sqlStr As String = "SELECT COUNT(*) AS total FROM rcdetail"
        Dim cmd As SqlCommand = New SqlCommand(sqlStr, connection)
        recCount = cmd.ExecuteScalar()

        sqlStr = "SELECT COUNT(*) AS total FROM rcdetail WHERE uname IS NOT null"
        cmd.CommandText = sqlStr
        checkedRecs = cmd.ExecuteScalar()
        connection.Close()
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Function PopulateYearList() As String
        connection.Open()
        Dim sqlStr As String = "SELECT YEAR(cdte) AS myyear, COUNT(*) AS total FROM rcdetail GROUP BY YEAR(cdte) ORDER BY 1"
        Dim cmd As SqlCommand = New SqlCommand(sqlStr, connection)
        Dim reader As SqlDataReader = cmd.ExecuteReader()
        Dim yearList As List(Of YearItems) = New List(Of YearItems)
        While reader.Read()
            Dim yearData As YearItems = New YearItems()
            yearData.dateYear = reader("myyear")
            yearData.items = reader("total")
            yearList.Add(yearData)
        End While
        connection.Close()

        Dim serializer As New JavaScriptSerializer(New SimpleTypeResolver())
        Return serializer.Serialize(yearList)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetBoxTypes(ByVal startDate As String, ByVal endDate As String) As String
        connection.Open()
        Dim sqlStr As String = "SELECT typename, type FROM rctype WHERE type in (SELECT distinct(type) FROM rcdetail WHERE cdte >= '" & startDate & "' AND cdte <= '" & endDate & "') ORDER BY typename"
        Dim cmd As SqlCommand = New SqlCommand(sqlStr, connection)
        Dim reader As SqlDataReader = cmd.ExecuteReader()
        Dim boxTypes As List(Of BoxType) = New List(Of BoxType)
        While reader.Read()
            Dim type As BoxType = New BoxType()
            type.id = reader("type")
            type.name = reader("typename")
            boxTypes.Add(type)
        End While
        connection.Close()

        Dim serializer As New JavaScriptSerializer(New SimpleTypeResolver())
        Return serializer.Serialize(boxTypes)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetStoreNo(ByVal boxType As String, ByVal startDate As String, ByVal endDate As String) As String
        connection.Open()
        Dim sqlStr As String = "SELECT distinct(store) FROM rcdetail WHERE cdte >= '" & startDate & "' AND cdte <= '" & endDate & "' AND type = '" & boxType & "' ORDER BY store"
        Dim cmd As SqlCommand = New SqlCommand(sqlStr, connection)
        Dim reader As SqlDataReader = cmd.ExecuteReader()
        Dim storeNums As List(Of String) = New List(Of String)
        While reader.Read()
            storeNums.Add(reader("store"))
        End While
        connection.Close()

        Dim serializer As New JavaScriptSerializer(New SimpleTypeResolver())
        Return serializer.Serialize(storeNums)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetPalletNo(ByVal storeNo As String, ByVal boxType As String, ByVal startDate As String, ByVal endDate As String) As String
        connection.Open()
        Dim sqlStr As String
        If IsNothing(boxType) Or boxType = "" Then
            sqlStr = "SELECT COUNT(DISTINCT mlink) FROM rcdetail WHERE cdte >= '" & startDate & "' AND cdte <= '" & endDate & "'"
        ElseIf IsNothing(storeNo) Or storeNo = "" Then
            sqlStr = "SELECT COUNT(DISTINCT mlink) FROM rcdetail WHERE cdte >= '" & startDate & "' AND cdte <= '" & endDate & "' AND type = '" & boxType & "'"
        Else
            sqlStr = "SELECT COUNT(DISTINCT mlink) FROM rcdetail WHERE cdte >= '" & startDate & "' AND cdte <= '" & endDate & "' AND type = '" & boxType & "' AND store = '" & storeNo & "'"
        End If
        Dim cmd As SqlCommand = New SqlCommand(sqlStr, connection)
        Dim palletNo As Integer = cmd.ExecuteScalar
        connection.Close()
        Return palletNo
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetPalletData(ByVal storeNo As String, ByVal boxType As String, ByVal startDate As String, ByVal endDate As String) As String
        connection.Open()
        Dim sqlStr As String
        If IsNothing(boxType) Or boxType = "" Then
            sqlStr = "SELECT DISTINCT mlink FROM rcdetail WHERE cdte >= '" & startDate & "' AND cdte <= '" & endDate & "'"
        ElseIf IsNothing(storeNo) Or storeNo = "" Then
            sqlStr = "SELECT DISTINCT mlink FROM rcdetail WHERE cdte >= '" & startDate & "' AND cdte <= '" & endDate & "' AND type = '" & boxType & "'"
        Else
            sqlStr = "SELECT DISTINCT mlink FROM rcdetail WHERE cdte >= '" & startDate & "' AND cdte <= '" & endDate & "' AND type = '" & boxType & "' AND store = '" & storeNo & "'"
        End If
        Dim cmd As SqlCommand = New SqlCommand(sqlStr, connection)
        Dim reader As SqlDataReader = cmd.ExecuteReader()
        Dim palletList As List(Of String) = New List(Of String)
        While reader.Read()
            palletList.Add(reader("mlink"))
        End While
        reader.Close()

        sqlStr = "SELECT *, rcdetail.%%lockres%% AS lockres FROM rcdetail INNER JOIN rcinv ON mlink = pallid WHERE mlink IN (" & palletList.First()
        For index As Integer = 1 To palletList.Count - 1
            sqlStr += ", " & palletList.Item(index)
        Next
        sqlStr += ") ORDER BY mlink, cdte"
        cmd.CommandText = sqlStr
        reader = cmd.ExecuteReader()
        Dim modalInfo As List(Of PalletInfo) = New List(Of PalletInfo)
        Dim previousID As String = Nothing
        Dim palletData As PalletInfo = New PalletInfo()
        Dim boxData As BoxInfo
        While reader.Read()
            If IsNothing(previousID) Then
                previousID = reader("pallid")
                palletData.palletLoc = reader("slot")
                If IsDBNull(reader("fpiclink")) Then
                    palletData.frontPic = Nothing
                Else
                    palletData.frontPic = reader("fpiclink")
                End If
                If IsDBNull(reader("bpiclink")) Then
                    palletData.backPic = Nothing
                Else
                    palletData.backPic = reader("bpiclink")
                End If
            ElseIf Not previousID = reader("pallid") Then
                previousID = reader("pallid")
                modalInfo.Add(palletData)
                palletData = New PalletInfo()
                palletData.palletLoc = reader("slot")
                If IsDBNull(reader("fpiclink")) Then
                    palletData.frontPic = Nothing
                Else
                    palletData.frontPic = reader("fpiclink")
                End If
                If IsDBNull(reader("bpiclink")) Then
                    palletData.backPic = Nothing
                Else
                    palletData.backPic = reader("bpiclink")
                End If
            End If
            Select Case reader("pos")
                Case "01"
                    If reader("fb") = "F" Then
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxF01 = boxData
                    Else
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxB01 = boxData
                    End If
                Case "02"
                    If reader("fb") = "F" Then
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxF02 = boxData
                    Else
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxB02 = boxData
                    End If
                Case "03"
                    If reader("fb") = "F" Then
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxF03 = boxData
                    Else
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxB03 = boxData
                    End If
                Case "04"
                    If reader("fb") = "F" Then
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxF04 = boxData
                    Else
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxB04 = boxData
                    End If
                Case "05"
                    If reader("fb") = "F" Then
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxF05 = boxData
                    Else
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxB05 = boxData
                    End If
                Case "06"
                    If reader("fb") = "F" Then
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxF06 = boxData
                    Else
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxB06 = boxData
                    End If
                Case "07"
                    If reader("fb") = "F" Then
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxF07 = boxData
                    Else
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxB07 = boxData
                    End If
                Case "08"
                    If reader("fb") = "F" Then
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxF08 = boxData
                    Else
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxB08 = boxData
                    End If
                Case "09"
                    If reader("fb") = "F" Then
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxF09 = boxData
                    Else
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxB09 = boxData
                    End If
                Case "10"
                    If reader("fb") = "F" Then
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxF10 = boxData
                    Else
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxB10 = boxData
                    End If
                Case "11"
                    If reader("fb") = "F" Then
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxF11 = boxData
                    Else
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxB11 = boxData
                    End If
                Case "12"
                    If reader("fb") = "F" Then
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxF12 = boxData
                    Else
                        boxData = New BoxInfo()
                        boxData.type = reader("type")
                        boxData.store = reader("store")
                        boxData.storageDate = reader("cdte")
                        If Not IsDBNull(reader("outdte")) Then
                            boxData.outDate = reader("outdte")
                        End If
                        boxData.frontOrBack = reader("fb")
                        boxData.position = reader("pos")
                        boxData.boxID = reader("lockres")
                        palletData.boxB12 = boxData
                    End If
            End Select
        End While
        modalInfo.Add(palletData)
        connection.Close()

        Dim serializer As New JavaScriptSerializer(New SimpleTypeResolver())
        Return serializer.Serialize(modalInfo)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function SubmitSelection(ByVal boxID As String) As String
        connection.Open()
        Dim sqlStr As String = "SELECT i.slot, i.istmp, i.pallid, t.typename, fb, pos, store, cdte, d.mlink, d.type as mytype, d.outdte FROM rcinv as i, rcdetail as d, rctype AS t WHERE d.mlink = i.pallid AND d.type = t.type AND d.%%LockRes%% ='" & boxID & "';"
        Dim cmd As SqlCommand = New SqlCommand(sqlStr, connection)
        Dim reader As SqlDataReader = cmd.ExecuteReader()

        Dim message As StringBuilder = New StringBuilder()

        Dim rUser As String = AccountManagement.UserPrincipal.Current.DisplayName
        Dim rDate As String = Nothing
        Dim contents As String = Nothing
        Dim cdte As String = Nothing
        Dim store As String = Nothing
        Dim slot As String = Nothing
        Dim pos As String = Nothing
        Dim pSide As String = Nothing
        Dim istmp As String = Nothing
        Dim mlink As String = Nothing
        Dim mytype As String = Nothing
        Dim outdte As String = Nothing

        While reader.Read()
            rDate = Date.Now().ToString("G")
            contents = reader("typename")
            cdte = reader("cdte")
            store = reader("store")
            slot = reader("slot")
            pos = reader("pos")
            pSide = reader("fb")
            istmp = reader("istmp")
            mlink = reader("mlink")
            mytype = reader("mytype")
            If Not IsDBNull(reader("outdte")) Then
                outdte = reader("outdte")
            End If
        End While
        reader.Close()

        Dim otherInfo(3) As String

        If outdte Is Nothing Then
            message.AppendLine("Accounting Case Request.")
            message.AppendLine("---------------------------------------------------")
            message.AppendLine("Request User: " & rUser)
            message.AppendLine("Request Date: " & rDate)
            message.AppendLine()
            message.AppendLine("Case Contents: " & contents)
            message.AppendLine("End Date: " & cdte)
            message.AppendLine("Store: " & store)
            message.AppendLine()
            message.AppendLine("Slot Number: " & slot)
            message.AppendLine("Box Number: " & pos)
            message.AppendLine("Pallet Side: " & pSide)
            message.AppendLine()
            If istmp = "Y" Then
                message.AppendLine("Building Pallet: YES")
            Else
                message.AppendLine("Building Pallet: NO")
            End If
            Dim facID As String = Trim(slot).Substring(0, 2)
            Dim facName As String
            If facID = "01" Or facID = "02" Or facID = "03" Or facID = "04" Then
                facName = "Reclaim Center"
            Else
                facName = "Wilson Distribution Center"
            End If
            message.AppendLine("Building Name: " & facName)
            Dim ccAddress As String = Split(HttpContext.Current.User.Identity.Name, "\")(1) & "@shopthepig.com"
            Email.Send("webadmn@shopthepig.com", "AcctFileRequest@shopthepig.com", "Accounting Case Checkout", message.ToString(), ccAddress)

            Dim sqlinfo As String = String.Format("CC|id={0}|pos={1}|fb={2}|cdte={3}|type={4}|store={5}|istmp={6}", mlink, pos, pSide, DateTime.Parse(cdte).ToString("yyyy-M-d"), mytype, store, istmp)
            sqlStr = String.Format("INSERT INTO rclog VALUES ('{0}', '{1}', '{2}')", Split(HttpContext.Current.User.Identity.Name, "\")(1), Date.Now().ToString("G"), sqlinfo)
            cmd.CommandText = sqlStr
            cmd.ExecuteNonQuery()

            If istmp = "Y" Then
                sqlStr = "DELETE FROM rcdetail WHERE %%lockres%%='" & boxID & "'"
                cmd.CommandText = sqlStr
                cmd.ExecuteNonQuery()

                For x = pos + 3 To 12 Step 3
                    sqlStr = "UPDATE rcdetail SET pos = '" & jFormat(x - 3, "0", 2, 0) & "' WHERE mlink = " & boxID & " AND fb = '" & pSide & "' AND pos = '" & jFormat(x, "0", 2, 0) & "'"
                    cmd.CommandText = sqlStr
                    cmd.ExecuteNonQuery()
                Next
            End If

            otherInfo = {"IN", rUser, Date.Now().ToString("G")}
        Else
            otherInfo = {"OUT", rUser, outdte}
        End If

        connection.Close()
        Dim serializer As New JavaScriptSerializer(New SimpleTypeResolver())
        Return serializer.Serialize(otherInfo)
    End Function

    Public Shared Function jFormat(strText, strChar, numStr, strDir) As String
        Dim lenTo
        Dim retStr
        Dim n

        lenTo = numStr
        retStr = Trim(strText)
        If (Len(strText) < lenTo) Then
            For n = 1 To lenTo
                If (strDir = 1) Then
                    retStr = retStr & strChar
                Else
                    retStr = strChar & retStr
                End If

                If (Len(retStr) = lenTo) Then
                    Return retStr
                    Exit Function
                End If
            Next
        Else
            Return Trim(strText)
        End If
    End Function

End Class