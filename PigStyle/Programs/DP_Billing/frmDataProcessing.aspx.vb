Imports System.Windows.Forms
Imports System.DirectoryServices
Imports System.Data.SqlClient

Public Class frmDataProcessing
    Inherits System.Web.UI.Page

    Public dirGroups As List(Of String)
    Public permissionType As String

    Dim connectionString As String = ("Server=VG-UNITEDDB;Database=freshdb;Uid=freshuser;Pwd=Fresh2012;")
    Dim conn As New SqlConnection(connectionString)
    Dim connmdb As ADODB.Connection
    Dim rsmdb As ADODB.Recordset ' dpinv(Microsoft Access)
    Dim connstr As ADODB.Connection ' venus
    Dim rsstr As ADODB.Recordset ' venus
    Dim fileCounter As Integer ' file counter

    Dim r As Integer
    Dim num_cols As Integer
    Dim col_wid() As Single
    Dim storeNum As String
    Dim code As String
    Dim reams As Long
    Dim simDup As String
    Dim jobType As String
    Dim laborHrs As Double
    Dim comments As String
    Dim selrow As Integer
    Dim selCol As Integer
    Dim segNum As Long
    Dim ControlCount As Integer
    Dim totalNetDue As String
    Dim InvoiceNumber As Long
    Dim outf As Integer
    Dim conf As Integer
    Dim axfile As Integer
    Dim storestand As Integer
    Dim pageCount As Integer
    Dim totalPage As Integer
    Dim FileHHFC As String
    Dim FileMMDDYY As String
    Dim PrintDate As String
    Dim tempTotal As Double
    Dim optionChk As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        dirGroups = GetGroups("LDAP://pwmdc4/DC=sso,DC=fb,DC=com", HttpContext.Current.User.Identity.Name)
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
                MessageBox.Show("Selected user isn't a member of any groups at this time.", "No groups listed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                MessageBox.Show(ex.Message.ToString, "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
        Return Groups
    End Function

End Class