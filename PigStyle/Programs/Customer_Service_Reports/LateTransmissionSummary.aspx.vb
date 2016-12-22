Imports System.Windows.Forms
Imports System.DirectoryServices
Imports System.Data.SqlClient
Imports System.Web.Script.Serialization
Imports System.IO
Imports Newtonsoft.Json

Public Class LateTransmissionSummary
    Inherits System.Web.UI.Page

    Protected dirGroups As List(Of String)
    Protected permissionType As String
    Protected departments As New List(Of String)

    Shared connectionStr As String = "Server=VG-UNITEDDB; Database=freshdb; User Id=freshuser; Password=Fresh2012;"
    Shared connection As SqlConnection = New SqlConnection(connectionStr)

    Structure District
        Public districtName
        Public districtNum
    End Structure

    Structure StoreSummary
        Public storeNo As String
        Public city As String
        Public numRecords As Integer
        Public numLate As Integer
        Public perOnTime As Double
    End Structure

    Structure DistrictInfo
        Public districtName As String
        Public summaryList As List(Of StoreSummary)
        Public totRecords As Integer
        Public totLate As Integer
        Public avgPercent As Double
    End Structure

    Structure PeriodInfo
        Public districtList As List(Of DistrictInfo)
        Public totRecords As Integer
        Public totLate As Integer
        Public avgPercent As Double
    End Structure

    Protected Shared districts As List(Of District)
    Protected Shared periodData As PeriodInfo

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
        csScript.Append(("alert('Invalid Group for " & Split(HttpContext.Current.User.Identity.Name, "\")(1) & ".  For access, please contact the Help Desk at Ext. 4357.');"))
        csScript.Append("window.location = '/Programs/Programs.aspx';")
        csScript.Append("</script>")

        If Not dirGroups.Contains("Corporate Users") And Not dirGroups.Contains("Store Users") Then
            If Not cs.IsClientScriptBlockRegistered(Me.[GetType](), "showNoPermissionMsg") Then
                cs.RegisterClientScriptBlock(Me.[GetType](), csName, csScript.ToString())
            End If
        End If

        GetDistricts(districts)
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

    Private Sub GetDistricts(ByRef districts As List(Of District))
        Dim sqlstr = "SELECT mgrname, mgrnum FROM csrdmgr ORDER BY mgrnum"
        Dim cmd = New SqlCommand(sqlstr, connection)
        connection.Open()
        districts = New List(Of District)
        Dim reader As SqlDataReader = cmd.ExecuteReader()

        While reader.Read()
            Dim district As District
            district.districtName = reader("mgrname")
            district.districtNum = reader("mgrnum")
            districts.Add(district)
        End While

        connection.Close()
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Function GetPeriodData(ByVal fromDate As Date, ByVal toDate As Date, ByVal selectedAreas() As String)
        Dim sqlStr As String
        If selectedAreas(0).ToLower = "all" Then
            sqlStr = "SELECT s.storenum, l.city, COUNT(s.storenum) AS totrecs, "
        Else
            sqlStr = "SELECT l.mgrname, s.storenum, l.city, COUNT(s.storenum) AS totrecs, "
        End If
        sqlStr += "COUNT(CASE WHEN DATEDIFF(MINUTE, s.transtime, s.rpttime) >= 30 THEN 1 ELSE NULL END) AS totlate "
        sqlStr += "FROM (SELECT a.*, b.mgrname FROM csrstrlst AS a INNER JOIN csrdmgr AS b ON a.dmanager = b.mgrnum) AS l "
        sqlStr += "RIGHT JOIN schedule AS s ON l.storenum = s.storenum "
        sqlStr += "WHERE (NOT s.boardtime IS NULL) AND (NOT s.rpttime IS NULL) "
        sqlStr += "AND s.recd = 'Y' AND s.uptype <> 'B' AND s.storenum < 5000 "
        sqlStr += "AND transtime >= '" & fromDate.ToString("d") & " 00:00:00' "
        sqlStr += "AND transtime < '" & toDate.AddDays(1).ToString("d") & " 23:59:59' "
        sqlStr += "AND s.dept IN (100,110,120,200,300) "
        sqlStr += "AND s.storenum <> '306' AND l.storetype <> 'I' "
        If selectedAreas(0).ToLower = "all" Then
            sqlStr += "GROUP BY s.storenum, l.city "
            sqlStr += "ORDER BY s.storenum"
        Else
            sqlStr += "AND l.dmanager IN (" & String.Join(",", selectedAreas) & ") "
            sqlStr += "GROUP BY l.dmanager, s.storenum, l.city, l.mgrname "
            sqlStr += "ORDER BY l.dmanager, s.storenum"
        End If

        Dim cmd As New SqlCommand(sqlStr, connection)
        connection.Open()
        Dim reader As SqlDataReader = cmd.ExecuteReader()

        If reader.HasRows() Then
            Dim storeData As StoreSummary
            Dim districtData As New DistrictInfo
            If selectedAreas(0).ToLower = "all" Then
                districtData.districtName = "All"
            End If
            districtData.summaryList = New List(Of StoreSummary)
            While reader.Read()
                If districtData.districtName IsNot Nothing Then
                    districtData.districtName = reader("mgrname")
                ElseIf Not districtData.districtName = "All" And Not districtData.districtName = reader("mgrname") Then
                    districtData.avgPercent = 1 - (districtData.totLate / districtData.totRecords)
                    periodData.districtList.Add(districtData)
                    districtData = New DistrictInfo
                    districtData.districtName = reader("mgrname")
                End If
                storeData = New StoreSummary
                storeData.storeNo = reader("storenum")
                storeData.city = reader("city")
                storeData.numRecords = reader("totrecs")
                districtData.totRecords += storeData.numRecords
                storeData.numLate = reader("totlate")
                districtData.totLate += storeData.numLate
                storeData.perOnTime = 1 - (storeData.numLate / storeData.numRecords)
                districtData.summaryList.Add(storeData)
            End While
        End If

        Try
            Dim serializer As New JavaScriptSerializer(New SimpleTypeResolver())
            Return serializer.Serialize(periodData)
        Catch ex1 As Exception
            If TypeOf ex1 Is OutOfMemoryException Then
                Try
                    Dim sb As New StringBuilder()

                    Using sw As New StringWriter(sb)
                        Using writer As JsonWriter = New JsonTextWriter(sw)
                            Dim serializer As New JsonSerializer()
                            serializer.Serialize(writer, periodData)
                            writer.Flush()
                            sw.Flush()
                        End Using
                    End Using

                    Const CHUNK_SIZE = 1000
                    Dim returnStr As String = ""
                    Dim ctr = 0
                    While ctr < (sb.Length - CHUNK_SIZE)
                        returnStr += sb.ToString(ctr, CHUNK_SIZE)
                        ctr += CHUNK_SIZE
                    End While
                    returnStr += sb.ToString(ctr, sb.Length - ctr)

                    Return returnStr
                Catch ex2 As Exception
                    Dim response2 As HttpResponse = HttpContext.Current.Response
                    response2.StatusCode = 500
                    Return String.Format("{0}: {1}", ex2.GetType.ToString, ex2.Message)
                End Try
            End If
            Dim response1 As HttpResponse = HttpContext.Current.Response
            response1.StatusCode = 500
            Return String.Format("{0}: {1}", ex1.GetType.ToString, ex1.Message)
        End Try
    End Function

End Class