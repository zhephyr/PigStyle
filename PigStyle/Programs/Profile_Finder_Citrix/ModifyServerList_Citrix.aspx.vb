Option Strict Off
Option Explicit On

Imports System.Windows.Forms
Imports System.DirectoryServices
Imports System.Data.SqlClient

Public Class ModifyServerList_Citrix
    Inherits System.Web.UI.Page

    Public dirGroups As List(Of String)
    Public permissionType As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache) ' Prevents browser caching.
        dirGroups = GetGroups("LDAP://pwmdc4/DC=sso,DC=fb,DC=com", HttpContext.Current.User.Identity.Name)
        'Gets server list for GridView
        LoadGrid()
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

    Public Sub AddClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)
        lblAddRec.Visible = False
        lblAddNoRec.Visible = False
        lblDeleteRec.Visible = False
        lblDeleteNoRec.Visible = False

        If Page.IsValid Then
            'Checks if server is already in list
            Dim match As Boolean = AddLookup()
            If Not match Then
                Using connection As New SqlConnection("server=VG-UNITEDDB;database=freshdb;Uid=sa;Pwd=2011Sql;")
                    Dim addCmd As New SqlCommand("INSERT INTO servername VALUES (@AddServer, @AddIP);", connection)
                    addCmd.Parameters.AddWithValue("@AddServer", serverName.Text)
                    addCmd.Parameters.AddWithValue("@AddIP", ipAddress.Text)

                    Try
                        connection.Open()
                        addCmd.ExecuteNonQuery()
                    Catch ex As Exception
                        Response.Write("<script language='javascript'>alert('Application Error: " & ex.Message & "')</script>")
                    Finally
                        connection.Close()
                        connection.Dispose()
                        'Clear the textboxes after Add button's clicked.
                        serverName.Text = ""
                        ipAddress.Text = ""
                    End Try
                End Using

                lblAddRec.Visible = True

            ElseIf match = True Then
                lblAddNoRec.Visible = True
            End If
            LoadGrid()
        End If
    End Sub

    Public Sub DelClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)
        lblAddRec.Visible = False
        lblAddNoRec.Visible = False
        lblDeleteRec.Visible = False
        lblDeleteNoRec.Visible = False

        If Page.IsValid Then
            'Checks if server is in list to delete
            Dim match As Boolean = DelLookup()
            If match Then
                Using connection As New SqlConnection("server=VG-UNITEDDB;database=freshdb;Uid=sa;Pwd=2011Sql;")
                    Dim DeleteCmd As New System.Data.SqlClient.SqlCommand("DELETE FROM servername WHERE (server=@DeleteServer);", connection)
                    DeleteCmd.Parameters.AddWithValue("@DeleteServer", serverName2.Text)
                    Try
                        connection.Open()
                        DeleteCmd.ExecuteNonQuery()
                    Catch ex As Exception
                        Response.Write("<script language='javascript'>alert('Application Error: " & ex.Message & "')</script>")
                    Finally
                        connection.Close()
                        connection.Dispose()
                        'Clear the textboxes after Add button's clicked.
                        serverName2.Text = ""
                    End Try
                End Using

                lblDeleteRec.Visible = True

            ElseIf match = False Then
                lblDeleteNoRec.Visible = True
            End If

            LoadGrid()

        End If
    End Sub

    Private Function AddLookup() As Boolean
        Using connection As New SqlConnection("server=VG-UNITEDDB;database=freshdb;Uid=sa;Pwd=2011Sql;")
            Dim command As New SqlCommand("SELECT COUNT(*) FROM servername WHERE (server=@SelectServer AND ip=@SelectIP);", connection)
            command.Parameters.AddWithValue("@SelectServer", serverName.Text)
            command.Parameters.AddWithValue("@SelectIP", ipAddress.Text)

            Try
                connection.Open()
                Dim matchCount As Short = command.ExecuteScalar()
                Dim match As Boolean
                If matchCount = 1 Then
                    match = True
                Else
                    match = False
                End If

                Return match

            Catch ex As Exception
                Response.Write("<script language='javascript'>alert('Application Error: " & ex.Message & "')</script>")
            Finally
                connection.Close()
                connection.Dispose()
            End Try
        End Using

        Return True

    End Function

    Private Function DelLookup() As Boolean
        Using connection As New SqlConnection("server=VG-UNITEDDB;database=freshdb;Uid=sa;Pwd=2011Sql;")
            Dim command As New SqlCommand("SELECT COUNT(*) FROM servername WHERE (server=@SelectServer);", connection)
            command.Parameters.AddWithValue("@SelectServer", serverName2.Text)

            Try
                connection.Open()
                Dim matchCount As Short = command.ExecuteScalar()
                Dim match As Boolean
                If matchCount = 1 Then
                    match = True
                Else
                    match = False
                End If

                Return match

            Catch ex As Exception
                Response.Write("<script language='javascript'>alert('Application Error: " & ex.Message & "')</script>")
            Finally
                connection.Close()
                connection.Dispose()
            End Try
        End Using

        Return False

    End Function

    Private Sub LoadGrid()
        Using conn As New System.Data.SqlClient.SqlConnection("server=VG-UNITEDDB;database=freshdb;Uid=sa;Pwd=2011Sql;")
            conn.Open()
            Dim cmd As New SqlCommand("SELECT * FROM servername ORDER BY server;", conn)
            Try
                Using dr As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                    ServerList.DataSource = dr
                    ServerList.DataBind()
                End Using
            Catch ex As Exception
                Response.Write("<script language='javascript'>alert('Application Error: " & ex.Message & "')</script>")
            Finally
                conn.Close()
                conn.Dispose()
            End Try
        End Using

        ChangeRecCount()
    End Sub

    Private Sub ChangeRecCount()
        Dim count As Integer
        Using connection As New SqlConnection("server=VG-UNITEDDB;database=freshdb;Uid=sa;Pwd=2011Sql;")
            Dim recordCmd As New System.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM servername;", connection)
            Try
                connection.Open()
                count = recordCmd.ExecuteScalar()
            Catch ex As Exception
                Response.Write("<script language='javascript'>alert('Application Error: " & ex.Message & "')</script>")
            Finally
                connection.Close()
                connection.Dispose()
                lblCount.Text = count
            End Try
        End Using
    End Sub

End Class