Imports System.DirectoryServices
Imports System.Windows.Forms
Imports System.Data.SqlClient
Imports System.Web.Script.Serialization
Imports System.Collections.Generic

Partial Public Class plu
    Inherits System.Web.UI.Page

    Public dirGroups As List(Of String)
    Public permissionType As String
    Public totalRecs As Integer

    Shared connectionStr As String = "Server=VG-HQDBPROD; Database=tciinstore; User Id=sa; Password=2013Sql;"
    Shared connection As SqlConnection = New SqlConnection(connectionStr)

    Structure Product
        Public PLU As String
        Public Dept As String
        Public DeptNo As String
        Public Desc1 As String
        Public Desc2 As String
        Public Ingredients As String
        Public UPC As String
        Public ServSize As String
        Public ServPer As Integer
        Public Calories As Integer
        Public CaloriesFat As Integer
        Public TotalFat As Decimal
        Public PerFat As Integer
        Public SatFat As Decimal
        Public PerSatFat As Integer
        Public TransFat As Decimal
        Public Cholesterol As Decimal
        Public PerCholesterol As Integer
        Public Sodium As Decimal
        Public PerSodium As Integer
        Public Carbs As Decimal
        Public PerCarbs As Integer
        Public Fiber As Decimal
        Public PerFiber As Integer
        Public Sugars As Decimal
        Public Protein As Decimal
        Public VitA As Integer
        Public VitC As Integer
        Public Calc As Integer
        Public Iron As Integer
        Public VitD As Integer
        Public VitE As Integer
        Public Thia As Integer
        Public Ribo As Integer
        Public Niac As Integer
        Public VitB6 As Integer
        Public Folate As Integer
        Public VitB12 As Integer
        Public Biotin As Integer
        Public Phos As Integer
        Public Zinc As Integer
        Public Mag As Integer
        Public Iodine As Integer
        Public Copper As Integer
        Public Acid As Integer
    End Structure

    Structure pluInfo
        Public PLU As String
        Public desc1 As String
        Public desc2 As String
    End Structure

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        dirGroups = GetGroups("LDAP://pwmdc4/DC=sso,DC=fb,DC=com", HttpContext.Current.User.Identity.Name)

        totalRecs = CountRecs(sender, e)
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

    <System.Web.Services.WebMethod()>
    Public Shared Function GetDepts() As String
        connection.Open()

        Dim sqlStr As String = "SELECT DISTINCT deptname FROM PWMv_PLUM_Departments ORDER BY deptname;"
        Dim command As SqlCommand = New SqlCommand(sqlStr, connection)

        Dim depts As List(Of String) = New List(Of String)
        Dim reader As SqlDataReader = command.ExecuteReader()
        While reader.Read()
            depts.Add(reader("deptname"))
        End While
        reader.Close()
        connection.Close()

        Dim serializer As New JavaScriptSerializer(New SimpleTypeResolver())
        Return serializer.Serialize(depts)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function LoadPLUs(ByVal desc As String, ByVal dept As String, ByVal checked As Boolean) As String
        Dim pluList As List(Of pluInfo) = New List(Of pluInfo)
        Dim sqlStr As String

        connection.Open()
        If desc = "" Then
            sqlStr = "SELECT DISTINCT a.pluno, a.desc1, a.desc2 FROM PWMv_PLUM_PLUMaster AS a, PWMv_PLUM_Departments AS b "
            sqlStr = sqlStr & String.Format("WHERE b.deptname = '{0}' AND a.deptno = b.deptno ", dept)
            If checked Then
                sqlStr = sqlStr & "ORDER BY 1, 2"
            Else
                sqlStr = sqlStr & "ORDER BY 2, 1"
            End If
        Else
            sqlStr = String.Format("SELECT DISTINCT pluno, desc1, desc2 FROM PWMv_PLUM_PLUMaster WHERE desc1 LIKE '%{0}%' OR desc2 LIKE '%{0}%';", desc)
        End If
        Dim command As SqlCommand = New SqlCommand(sqlStr, connection)
        Dim reader As SqlDataReader = command.ExecuteReader()
        While reader.Read()
            Dim pluOption As pluInfo = New pluInfo
            pluOption.PLU = reader("pluno")
            If Not IsDBNull(reader("desc1")) Then
                pluOption.desc1 = reader("desc1")
            End If
            If Not IsDBNull(reader("desc2")) Then
                pluOption.desc2 = reader("desc2")
            End If
            pluList.Add(pluOption)
        End While

        reader.Close()
        connection.Close()

        Dim serializer As New JavaScriptSerializer(New SimpleTypeResolver())
        Return serializer.Serialize(pluList)
    End Function

    Protected Function CountRecs(ByVal sender As Object, ByVal e As System.EventArgs) As Integer
        Dim connectionStr As String = "Server=VG-HQDBPROD; Database=tciinstore; User Id=sa; Password=2013Sql;"
        Dim connection As SqlConnection = New SqlConnection(connectionStr)

        connection.Open()

        Dim sqlStr As String = "SELECT COUNT(*) FROM PWMv_PLUM_PLUMaster"
        Dim command As SqlCommand = New SqlCommand(sqlStr, connection)
        Dim recordsNo As Integer = command.ExecuteScalar()
        connection.Close()

        Return recordsNo
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function CheckDuplicates(ByVal pluno As String) As String
        Dim plus As List(Of Product) = New List(Of Product)
        Dim connectionStr As String = "Server=VG-HQDBPROD; Database=tciinstore; User Id=sa; Password=2013Sql;"
        Dim connection As SqlConnection = New SqlConnection(connectionStr)

        connection.Open()
        Dim sqlStr As String = String.Format("SELECT COUNT(*) FROM PWMv_PLUM_PLUMaster WHERE pluno = {0}", pluno)
        Dim command As SqlCommand = New SqlCommand(sqlStr, connection)
        Dim pluCount As Integer = command.ExecuteScalar()
        connection.Close()

        If pluCount > 1 Then
            plus.Clear()

            connection.Open()
            Dim choiceStr As String = String.Format("SELECT * FROM PWMv_PLUM_PLUMaster AS a, PWMv_PLUM_Departments AS b WHERE a.pluno = {0} AND a.deptno = b.deptno;", pluno)
            Dim choiceCommand As SqlCommand = New SqlCommand(choiceStr, connection)
            Dim pluData As SqlDataReader = choiceCommand.ExecuteReader()
            While pluData.Read()
                Dim productData As New Product
                productData.PLU = pluData("pluno")
                productData.Dept = pluData("deptname")
                productData.Desc1 = pluData("desc1")
                If Not IsDBNull(pluData("desc2")) Then
                    productData.Desc2 = pluData("desc2")
                End If
                plus.Add(productData)
            End While
            pluData.Close()
            connection.Close()

            Dim serializer As New JavaScriptSerializer(New SimpleTypeResolver())
            Return serializer.Serialize(plus)
        Else
            Return LoadData(pluno, Nothing)
        End If
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function LoadData(ByVal pluno As String, ByVal deptname As String) As String
        Dim productData As New Product
        Dim sqlStr As String

        Dim connectionStr As String = "Server=VG-HQDBPROD; Database=tciinstore; User Id=sa; Password=2013Sql;"
        Dim connection As SqlConnection = New SqlConnection(connectionStr)

        connection.Open()
        If deptname = Nothing Then
            sqlStr = String.Format("SELECT * FROM PWMv_PLUM_PLUMaster AS a, PWMv_PLUM_Departments AS b WHERE a.pluno = {0} AND a.deptno = b.deptno;", pluno)
        Else
            sqlStr = String.Format("SELECT * FROM PWMv_PLUM_PLUMaster AS a INNER JOIN PWMv_PLUM_Departments AS b ON a.deptno = b.deptno WHERE a.pluno = {0} AND b.deptname = '{1}';", pluno, deptname)
        End If
        Dim command As SqlCommand = New SqlCommand(sqlStr, connection)
        Dim pluData As SqlDataReader = command.ExecuteReader()
        While pluData.Read()
            productData.PLU = pluData("pluno")
            productData.Dept = pluData("deptname")
            productData.DeptNo = pluData("deptno")
            productData.Desc1 = pluData("desc1")
            If Not IsDBNull(pluData("desc2")) Then
                productData.Desc2 = pluData("desc2")
            End If
            productData.UPC = pluData("upc")
        End While
        pluData.Close()

        sqlStr = String.Format("SELECT ingrtext FROM PWMv_PLUM_IngrMaster WHERE ingrno = {0} AND deptno = {1};", pluno, productData.DeptNo)
        command.CommandText = sqlStr
        productData.Ingredients = command.ExecuteScalar()

        sqlStr = String.Format("SELECT * FROM PWMv_PLUM_NutriMaster WHERE nutrino = {0} AND deptno = {1};", pluno, productData.DeptNo)
        command.CommandText = sqlStr
        pluData = command.ExecuteReader()
        While pluData.Read()
            productData.ServSize = pluData("serveuomdesc")
            If Not IsDBNull(pluData("servpercon")) Then
                productData.ServPer = pluData("servpercon")
            End If
            productData.Calories = pluData("calories") / 10
            productData.CaloriesFat = pluData("calfrmfat") / 10
            productData.TotalFat = pluData("totalfat") / 10
            productData.PerFat = pluData("totalfatper") / 10
            productData.SatFat = pluData("saturfat") / 10
            productData.PerSatFat = pluData("saturfatper") / 10
            productData.TransFat = pluData("transfat") / 10
            productData.Cholesterol = pluData("cholesterol") / 10
            productData.PerCholesterol = pluData("cholesterolper") / 10
            productData.Sodium = pluData("sodium") / 10
            productData.PerSodium = pluData("sodiumper") / 10
            productData.Carbs = pluData("totalcarb") / 10
            productData.PerCarbs = pluData("totalcarbper") / 10
            productData.Fiber = pluData("dietfiber") / 10
            productData.PerFiber = pluData("dietfiberper") / 10
            productData.Sugars = pluData("sugars") / 10
            productData.Protein = pluData("protein") / 10
            productData.VitA = pluData("vitamina") / 10
            productData.VitC = pluData("vitaminc") / 10
            productData.Calc = pluData("calcium") / 10
            productData.Iron = pluData("iron") / 10
            productData.VitD = pluData("vitamind") / 10
            productData.VitE = pluData("vitamine") / 10
            productData.Thia = pluData("thiamine") / 10
            productData.Ribo = pluData("riboflavin") / 10
            productData.Niac = pluData("niacin") / 10
            productData.VitB6 = pluData("vitaminb6") / 10
            productData.Folate = pluData("folate") / 10
            productData.VitB12 = pluData("vitaminb12") / 10
            productData.Biotin = pluData("biotin") / 10
            productData.Phos = pluData("phosphorus") / 10
            productData.Zinc = pluData("zinc") / 10
            productData.Mag = pluData("magnesium") / 10
            productData.Iodine = pluData("iodine") / 10
            productData.Copper = pluData("copper") / 10
            productData.Acid = pluData("pantothenicacid") / 10
        End While
        pluData.Close()
        connection.Close()

        Dim serializer As New JavaScriptSerializer(New SimpleTypeResolver())
        Return serializer.Serialize(productData)
    End Function

End Class