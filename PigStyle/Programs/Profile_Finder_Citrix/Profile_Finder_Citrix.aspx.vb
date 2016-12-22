﻿Option Strict Off
Option Explicit On

Imports System.Windows.Forms
Imports System.DirectoryServices
Imports System.Data.SqlClient

Public Class Profile_Finder_Citrix
    Inherits System.Web.UI.Page

    Public dirGroups As List(Of String)
    Public permissionType As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache) ' Prevents browser caching.
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

    Public Sub ServerSearch(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)
        If Page.IsValid Then
            'Connect to server
            Dim connectionStr As String = "server=VG-UNITEDDB;database=freshdb;Uid=sa;Pwd=2011Sql;"
            Dim connection As New SqlConnection(connectionStr)
            connection.Open()
            Dim cmd As New SqlCommand("SELECT server FROM servername ORDER BY server;", connection)
            Dim dr As SqlDataReader = cmd.ExecuteReader()

            'Clear server list for new search
            serverList.Items.Clear()

            Dim NewProfilePath As String
            Dim UNCPath As String
            Dim matchfound As Boolean = False

            If dr.HasRows Then
                Try
                    While dr.Read() 'Read server list
                        Dim server As String = LCase(Trim(dr("server"))) 'Get name of server
                        NewProfilePath = "\c$\documents and settings\"
                        'Build path to search using DirectoryExists
                        UNCPath = "\\" & ((server) & (NewProfilePath) & Trim(UserID.Text))

                        'Skip to next iteration if no server name
                        If server = "" Then
                            Continue While
                        End If

                        matchfound = My.Computer.FileSystem.DirectoryExists(UNCPath)
                        If matchfound Then
                            serverList.Items.Add(New ListItem(server))
                        End If

                    End While

                    If serverList.Items.Count = 0 Then
                        serverList.Items.Add(New ListItem("No servers found for this username."))
                    End If

                Catch ex As Exception
                    Throw ex
                    Response.Write("<script language='javascript'>alert('Application Error: " & ex.Message & "')</script>")

                Finally
                    dr.Close()
                    connection.Close()
                    connection.Dispose()

                End Try
            End If
        End If

    End Sub

End Class