﻿Imports System.Windows.Forms
Imports System.DirectoryServices
Imports System.Data.SqlClient
Imports System.Web.Script.Serialization
Imports PWutilities.PWutilities
Imports System.IO

Public Class CustomerServiceReportTotals
    Inherits System.Web.UI.Page

    Protected dirGroups As List(Of String)
    Protected permissionType As String

    Shared connectionStr As String = "Server=VG-UNITEDDB; Database=freshdb; User Id=freshuser; Password=Fresh2012;"

    Protected storeOptions As List(Of String)
    Protected areaOptions As List(Of String)

    Structure TotalsInfo
        Dim desc As String
        Dim transmissions As List(Of Transmission)
        Dim deliveries As List(Of Delivery)
        Dim custAddOns As List(Of AddOn)
    End Structure

    Structure Transmission
        Dim deptNo As String
        Dim dept As String
        Dim numTrans As Integer
        Dim numLate As Integer
        Dim perOnTime As Double
    End Structure

    Structure Delivery
        Dim desc As String
        Dim numDeliveries As Integer
        Dim ttlCases As Integer
        Dim avgCases As Integer
    End Structure

    Structure AddOn
        Dim deptNo As String
        Dim dept As String
        Dim numCalls As Integer
        Dim ttlCases As Integer
    End Structure

    Structure LateTrans
        Dim weekday As String
        Dim schedTime As String
        Dim proTime As String
    End Structure

    Shared csrTotInfo As TotalsInfo

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Dim username As String = "pfox"
        Dim username As String = Split(HttpContext.Current.User.Identity.Name, "\")(1)
        dirGroups = GetGroups("LDAP://pwmdc4/DC=sso,DC=fb,DC=com", username)

        If dirGroups.Contains("DG-DSI-PREFERED CARD USER") Then
            Response.Redirect("http://" & Request.ServerVariables("SERVER_NAME") & "/production/dci/lookupcard.asp")
        End If

        If dirGroups.Contains("Corporate Users") Then
            permissionType = "Corporate"
        ElseIf dirGroups.Contains("Store Users") Then
            permissionType = "Store"
        ElseIf dirGroups.Contains("Domain Users") Then
            permissionType = "Vendor"
        End If

        Dim cs As ClientScriptManager = Page.ClientScript
        Dim csName As String = "showNoPermissionMsg"
        Dim csScript As New StringBuilder()
        csScript.Append("<script type=""text/javascript"">")
        csScript.Append("alert('Invalid Group for " & Split(HttpContext.Current.User.Identity.Name, "\")(1) & ".  For access, please contact the Help Desk at Ext. 4357.');")
        csScript.Append("window.location = '/Programs/Programs.aspx';")
        csScript.Append("</script>")

        If Not dirGroups.Contains("Corporate Users") And Not dirGroups.Contains("Store Users") Then
            If Not cs.IsClientScriptBlockRegistered(Me.[GetType](), "showNoPermissionMsg") Then
                cs.RegisterClientScriptBlock(Me.[GetType](), csName, csScript.ToString())
            End If
        End If

        GetStoreOptions(permissionType, dirGroups)
        GetAreaOptions(permissionType)
    End Sub

    Private Function GetGroups(ByVal _path As String, ByVal username As String) As List(Of String)
        Dim Groups As New List(Of String)
        Dim dirEntry As New System.DirectoryServices.DirectoryEntry(_path)
        Dim dirSearcher As New DirectorySearcher(dirEntry)
        dirSearcher.Filter = "(&(objectcategory=user)(SAMAccountName=" & username & "))"
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
                MessageBox.Show("Selected user isn't a member of any groups at this time.", "No groups listed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                MessageBox.Show(ex.Message.ToString, "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
        Return Groups
    End Function

    Private Sub GetStoreOptions(ByVal permissionType As String, ByRef groups As List(Of String))
        storeOptions = New List(Of String)

        Dim connection As SqlConnection = New SqlConnection(connectionStr)

        connection.Open()

        If permissionType = "Store" Then
            For Each group In groups
                If group.StartsWith("store ") Then
                    storeOptions.Add(group.Substring(6, 3))
                End If
            Next
            storeOptions.Sort()

        ElseIf permissionType = "Corporate" Then

            Dim sqlStr As String = "SELECT storenum FROM csrstrlst WHERE activestore = 'Y' ORDER BY storenum;"
            Dim cmd As SqlCommand = New SqlCommand(sqlStr, connection)
            Dim reader As SqlDataReader = cmd.ExecuteReader()
            Dim storeNum As Integer

            While reader.Read()
                storeNum = reader("storenum")
                storeOptions.Add(storeNum.ToString("D3"))
            End While
            reader.Close()

        End If

        connection.Close()
    End Sub

    Private Sub GetAreaOptions(ByVal permissionType As String)
        areaOptions = New List(Of String)

        If permissionType = "Corporate" Then
            Dim connection As SqlConnection = New SqlConnection(connectionStr)

            connection.Open()

            Dim sqlStr As String = "SELECT mgrname FROM csrdmgr ORDER BY mgrname"
            Dim cmd As SqlCommand = New SqlCommand(sqlStr, connection)
            Dim reader As SqlDataReader = cmd.ExecuteReader()

            While reader.Read()
                areaOptions.Add(reader("mgrname"))
            End While
            reader.Close()

            connection.Close()
        End If
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Function GetPeriodData(ByVal fromDate As Date, ByVal toDate As Date, ByVal selectedOptions() As String) As String
        Dim connection As SqlConnection = New SqlConnection(connectionStr)

        Dim sqlStr As String
        Dim cmd As SqlCommand

        csrTotInfo = New TotalsInfo()

        If selectedOptions(0) = "all" Then
            csrTotInfo.desc = "All Stores"
            csrTotInfo.transmissions = GetTransmissions(fromDate, toDate, True)
            csrTotInfo.deliveries = GetDeliveries(fromDate, toDate, True)
            csrTotInfo.custAddOns = GetAddOns(fromDate, toDate, True)
        Else
            Dim stores As String
            csrTotInfo.desc = String.Join(", ", selectedOptions)
            If Regex.IsMatch(selectedOptions(0), "^\d+$") Then
                stores = "('" & String.Join("', '", selectedOptions) & "')"
                csrTotInfo.transmissions = GetTransmissions(fromDate, toDate, False, stores)
                csrTotInfo.deliveries = GetDeliveries(fromDate, toDate, False, stores)
                csrTotInfo.custAddOns = GetAddOns(fromDate, toDate, False, stores)
            Else
                connection.Open()

                sqlStr = "SELECT storenum FROM csrstrlst AS a INNER JOIN csrdmgr AS b ON a.dmanager = b.mgrnum WHERE b.mgrname IN ('"
                sqlStr += String.Join("', '", selectedOptions)
                sqlStr += "') ORDER BY storenum"
                cmd = New SqlCommand(sqlStr, connection)
                Dim selectList As New List(Of String)
                Dim reader As SqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    selectList.Add(reader("storenum"))
                End While
                reader.Close()
                connection.Close()

                stores = "('" & String.Join("', '", selectList) & "')"
                csrTotInfo.transmissions = GetTransmissions(fromDate, toDate, False, stores)
                csrTotInfo.deliveries = GetDeliveries(fromDate, toDate, False, stores)
                csrTotInfo.custAddOns = GetAddOns(fromDate, toDate, False, stores)
            End If
        End If

        Dim serializer As New JavaScriptSerializer(New SimpleTypeResolver())
        Return serializer.Serialize(csrTotInfo)
    End Function

    Protected Shared Function GetTransmissions(ByVal fromDate As Date, ByVal toDate As Date, ByVal isAll As Boolean, Optional ByVal storeNo As String = Nothing) As List(Of Transmission)
        Dim transmissions As New List(Of Transmission)
        Dim sqlStr As String

        If isAll Then
            sqlStr = "SELECT s.*, d.deptname FROM schedule AS s "
            sqlStr = sqlStr & "JOIN osdepartments AS d "
            sqlStr = sqlStr & "ON d.deptnum = s.dept "
            sqlStr = sqlStr & "WHERE boardtime IS NOT NULL "
            sqlStr = sqlStr & "AND rpttime IS NOT NULL "
            sqlStr = sqlStr & "AND transtime >= '" & fromDate.ToString("d") & " 00:00:00' "
            sqlStr = sqlStr & "AND transtime < '" & toDate.AddDays(1).ToString("d") & " 23:59:59' "
            sqlStr = sqlStr & "AND recd= 'Y' AND uptype <> 'B' "
            sqlStr = sqlStr & "AND dept IN (100,110,120,140,200,300) ORDER BY dept;"
        Else
            sqlStr = "SELECT s.*, d.deptname FROM schedule AS s "
            sqlStr = sqlStr & "JOIN osdepartments AS d "
            sqlStr = sqlStr & "ON d.deptnum = s.dept "
            sqlStr = sqlStr & "WHERE boardtime IS NOT NULL "
            sqlStr = sqlStr & "AND rpttime IS NOT NULL "
            sqlStr = sqlStr & "AND transtime >= '" & fromDate.ToString("d") & " 00:00:00' "
            sqlStr = sqlStr & "AND transtime < '" & toDate.AddDays(1).ToString("d") & " 23:59:59' "
            sqlStr = sqlStr & "AND storenum IN " & storeNo & " "
            sqlStr = sqlStr & "AND recd= 'Y' AND uptype <> 'B' "
            sqlStr = sqlStr & "AND dept IN (100,110,120,140,200,300) ORDER BY dept;"
        End If

        Dim innerConnection As SqlConnection = New SqlConnection(connectionStr)
        innerConnection.Open()
        Dim innerCmd As SqlCommand = New SqlCommand(sqlStr, innerConnection)
        Dim innerReader As SqlDataReader = innerCmd.ExecuteReader()
        Dim currDept As String = Nothing
        Dim prevDept As String = Nothing
        Dim trans As New Transmission()

        If Not innerReader.HasRows() Then
            innerReader.Close()
            innerConnection.Close()
            Return Nothing
        End If

        While innerReader.Read()
            currDept = innerReader("dept")

            If prevDept Is Nothing Then
                prevDept = currDept
                trans.deptNo = currDept
                trans.dept = innerReader("deptname")
                trans.numTrans = 0
                trans.numLate = 0
            Else
                If Not prevDept = currDept Then
                    trans.perOnTime = 1 - trans.numLate / trans.numTrans
                    transmissions.Add(trans)
                    prevDept = currDept
                    trans = New Transmission
                    trans.deptNo = currDept
                    trans.dept = innerReader("deptname")
                    trans.numTrans = 0
                    trans.numLate = 0
                End If
            End If

            trans.numTrans += 1

            Dim transTime As DateTime = DateTime.Parse(innerReader("transtime"))
            Dim rptTime As DateTime = DateTime.Parse(innerReader("rpttime"))
            If DateDiff(DateInterval.Minute, transTime, rptTime) >= 30 Then
                trans.numLate += 1
            End If
        End While
        trans.perOnTime = 1 - trans.numLate / trans.numTrans
        transmissions.Add(trans)

        innerReader.Close()
        innerConnection.Close()

        Return transmissions
    End Function

    Private Shared Function GetDeliveries(ByVal fromDate As Date, ByVal toDate As Date, ByVal isAll As Boolean, Optional ByVal storeNo As String = Nothing) As List(Of Delivery)
        Dim deliveries As New List(Of Delivery)
        Dim sqlStr As String

        If isAll Then
            sqlStr = "SELECT d.commodity, d.cases, d.facility, c.description "
            sqlStr = sqlStr & "FROM csrdeliv as d, csrcommodity as c "
            sqlStr = sqlStr & "WHERE d.commodity = c.commodity "
            sqlStr = sqlStr & "AND d.facility = c.facility "
            sqlStr = sqlStr & "AND schdtime >= '" & fromDate.ToString("d") & " 00:00:00' "
            sqlStr = sqlStr & "AND schdtime < '" & toDate.ToString("d") & " 23:59:59' "
            sqlStr = sqlStr & "ORDER BY commodity;"
        Else
            sqlStr = "SELECT d.storenum, d.commodity, d.cases, d.facility, c.description "
            sqlStr = sqlStr & "FROM csrdeliv as d, csrcommodity as c "
            sqlStr = sqlStr & "WHERE d.commodity = c.commodity "
            sqlStr = sqlStr & "AND d.facility = c.facility "
            sqlStr = sqlStr & "AND schdtime >= '" & fromDate.ToString("d") & " 00:00:00' "
            sqlStr = sqlStr & "AND schdtime < '" & toDate.ToString("d") & " 23:59:59' "
            sqlStr = sqlStr & "AND storenum IN " & storeNo & " "
            sqlStr = sqlStr & "ORDER BY commodity;"
        End If

        Dim innerConnection As SqlConnection = New SqlConnection(connectionStr)
        innerConnection.Open()
        Dim innerCmd As SqlCommand = New SqlCommand(sqlStr, innerConnection)
        Dim innerReader As SqlDataReader = innerCmd.ExecuteReader()
        Dim delivery As New Delivery
        Dim currDesc As String = Nothing
        Dim prevDesc As String = Nothing

        If Not innerReader.HasRows() Then
            innerReader.Close()
            innerConnection.Close()
            Return Nothing
        End If

        While innerReader.Read()
            currDesc = innerReader("description")

            If prevDesc Is Nothing Then
                prevDesc = currDesc
                delivery.desc = currDesc
                delivery.numDeliveries = 0
                delivery.ttlCases = 0
            Else
                If Not prevDesc = currDesc Then
                    delivery.avgCases = delivery.ttlCases / delivery.numDeliveries
                    deliveries.Add(delivery)
                    prevDesc = currDesc
                    delivery = New Delivery
                    delivery.desc = currDesc
                    delivery.numDeliveries = 0
                    delivery.ttlCases = 0
                End If
            End If

            delivery.numDeliveries += 1
            delivery.ttlCases += innerReader("cases")
        End While
        delivery.avgCases = delivery.ttlCases / delivery.numDeliveries
        deliveries.Add(delivery)

        innerReader.Close()
        innerConnection.Close()

        Return deliveries
    End Function

    Private Shared Function GetAddOns(ByVal fromDate As Date, ByVal toDate As Date, ByVal isAll As Boolean, Optional ByVal storeNo As String = Nothing) As List(Of AddOn)
        Dim addOns As New List(Of AddOn)
        Dim sqlStr As String

        If isAll Then
            sqlStr = "SELECT a.*, deptname FROM ("
            sqlStr += "SELECT department, COUNT(qty) as numcalls, SUM(qty) as qty "
            sqlStr += "FROM csraddon "
            sqlStr += "WHERE departtime >= '" & fromDate.ToString("d") & " 00:00:00' "
            sqlStr += "AND departtime < '" & toDate.ToString("d") & " 23:59:59' "
            sqlStr += "GROUP BY department) AS a "
            sqlStr += "LEFT JOIN osdepartments "
            sqlStr += "ON a.department = deptnum;"
        Else
            sqlStr = "SELECT a.*, deptname FROM ("
            sqlStr += "SELECT department, COUNT(qty) as numcalls, SUM(qty) as qty "
            sqlStr += "FROM csraddon "
            sqlStr += "WHERE departtime >= '" & fromDate.ToString("d") & " 00:00:00' "
            sqlStr += "AND departtime < '" & toDate.ToString("d") & " 23:59:59' "
            sqlStr += "AND storenum IN (" & storeNo & ") "
            sqlStr += "GROUP BY department) AS a "
            sqlStr += "LEFT JOIN osdepartments "
            sqlStr += "ON a.department = deptnum;"
        End If

        Dim innerConnection As SqlConnection = New SqlConnection(connectionStr)
        innerConnection.Open()
        Dim innerCmd As SqlCommand = New SqlCommand(sqlStr, innerConnection)
        Dim innerReader As SqlDataReader = innerCmd.ExecuteReader()
        Dim custAddOns As New List(Of AddOn)
        Dim addOn As New AddOn
        Dim currDept As String
        Dim prevDept As String

        If Not innerReader.HasRows() Then
            innerReader.Close()
            innerConnection.Close()
            Return Nothing
        End If

        While innerReader.Read()
            currDept = innerReader("department")

            If prevDept Is Nothing Then
                prevDept = currDept
                addOn.deptNo = currDept
                If IsDBNull(innerReader("deptname")) Then
                    addOn.dept = "N/A"
                Else
                    addOn.dept = innerReader("deptname")
                End If
            Else
                If Not prevDept = currDept Then
                    custAddOns.Add(addOn)
                    prevDept = currDept
                    addOn = New AddOn
                    addOn.deptNo = currDept
                    If IsDBNull(innerReader("deptname")) Then
                        addOn.dept = "N/A"
                    Else
                        addOn.dept = innerReader("deptname")
                    End If
                End If
            End If
            
            addOn.numCalls = innerReader("numcalls")
            addOn.ttlCases = innerReader("qty")
        End While
        custAddOns.Add(addOn)

        innerReader.Close()
        innerConnection.Close()

        Return custAddOns
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EmailReport(ByVal fromDate As Date, ByVal toDate As Date) As String

        Dim ttl1 As Integer = 0
        Dim ttl2 As Integer = 0
        Dim ttl3 As Double = 0

        If Not Directory.Exists("\\SSOHOME\home\programs\Vbnet\Web\PigStyle\PigStyle\Programs\Customer_Service_Reports\Customer Service Reports") Then
            Directory.CreateDirectory("\\SSOHOME\home\programs\Vbnet\Web\PigStyle\PigStyle\Programs\Customer_Service_Reports\Customer Service Reports")
        End If

        Dim csrFile As String = "\\SSOHOME\home\programs\Vbnet\Web\PigStyle\PigStyle\Programs\Customer_Service_Reports\Customer Service Reports\CSR Totals Report.csv"

        Try
            Using csrCSV As New IO.StreamWriter(csrFile, False)
                csrCSV.WriteLine("Store #" & csrTotInfo.desc)
                csrCSV.WriteLine()

                If csrTotInfo.transmissions IsNot Nothing Then
                    csrCSV.WriteLine("Transmission by Dept,# of Trans,# of Late,% on Time")
                    For Each trans In csrTotInfo.transmissions
                        csrCSV.WriteLine(String.Join(",", trans.deptNo & " - " & trans.dept, trans.numTrans, trans.numLate, trans.perOnTime))
                        ttl1 += trans.numTrans
                        ttl2 += trans.numLate
                    Next
                    ttl3 = 1 - (ttl2 / ttl1)
                    csrCSV.WriteLine(String.Join(",", "Totals", ttl1, ttl2, ttl3))
                    csrCSV.WriteLine()
                End If

                If csrTotInfo.deliveries IsNot Nothing Then
                    csrCSV.WriteLine("Deliveries by Commodity, # of Deliveries, TTL Cases Avg., Cases Per Del")
                    ttl1 = 0
                    ttl2 = 0
                    ttl3 = 0
                    For Each deliv In csrTotInfo.deliveries
                        csrCSV.WriteLine(String.Join(",", deliv.desc, deliv.numDeliveries, deliv.ttlCases, deliv.avgCases))
                        ttl1 += deliv.numDeliveries
                        ttl2 += deliv.ttlCases
                    Next

                    ttl3 = ttl2 / ttl1
                    csrCSV.WriteLine(String.Join(",", "Totals", ttl1, ttl2, ttl3))
                    csrCSV.WriteLine()
                End If

                If csrTotInfo.custAddOns IsNot Nothing Then
                    csrCSV.WriteLine("Customer Add Ons, Calls, TTL Cases")
                    ttl1 = 0
                    ttl2 = 0
                    For Each custAddOn In csrTotInfo.custAddOns
                        csrCSV.WriteLine(String.Join(",", custAddOn.deptNo & " - " & custAddOn.dept, custAddOn.numCalls, custAddOn.ttlCases))
                        ttl1 += custAddOn.numCalls
                        ttl2 += custAddOn.ttlCases
                    Next
                    csrCSV.WriteLine(String.Join(",", "Totals", ttl1, ttl2))
                    csrCSV.WriteLine()
                End If
            End Using

            Dim sendTo As String = Split(HttpContext.Current.User.Identity.Name, "\")(1) & "@shopthepig.com"
            Email.Send("webAdmin@shopthepig.com", sendTo, String.Format("Customer Service Report {0} - {1}", fromDate.ToString("MM/dd/yyyy"), toDate.ToString("MM/dd/yyyy")), _
                       String.Format("Customer Service Report for {0} for the period: {1} - {2}", csrTotInfo.desc, fromDate.ToString("MM/dd/yyyy"), toDate.ToString("MM/dd/yyyy")), "", csrFile)

            My.Computer.FileSystem.DeleteFile(csrFile)

            Return "File creation was a success. Emailed to " & sendTo
        Catch ex As Exception
            Return String.Format("Unable to send file. Please call the Helpdesk." & vbCrLf & "Exception {0}", ex.Message)
        End Try

    End Function

End Class