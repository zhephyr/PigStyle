Imports System.Windows.Forms
Imports System.DirectoryServices

Public Class Programs
    Inherits System.Web.UI.Page

    Public dirGroups As List(Of String) = Nothing
    Public permissionType As String

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

End Class