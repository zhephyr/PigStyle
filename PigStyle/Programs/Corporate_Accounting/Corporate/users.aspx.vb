Public Class users
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Buffer = True
        Dim username As String = Request.LogonUserIdentity.Name.ToString()
        Dim n As Integer = username.IndexOf("\")
        username = username.Substring(n + 1)
        If Not (username = "jxiong" Or username = "tjohnson") Then
            Response.Write("Sorry, you don't have access to view this program!")
            Response.End()
        End If
    End Sub

End Class