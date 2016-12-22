Imports System.Data
Imports System.Data.SqlClient

Public Class getEmployees
    Inherits System.Web.UI.Page

    Public Structure employee
        Public firstName As String
        Public lastName As String
        Public extension As String
        Public address As String
        Public city As String
        Public state As String
        Public zip As String
        Public store As String
    End Structure

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Shared Function PCase(ByVal strInput) As String
        Dim I As Integer
        Dim CurrentChar, PrevChar As Char
        Dim strOutput As String

        PrevChar = ""
        strOutput = ""

        For I = 1 To Len(strInput)
            CurrentChar = Mid(strInput, I, 1)

            Select Case PrevChar
                Case "", " ", ".", "-", ",", """", "'"
                    strOutput = strOutput & UCase(CurrentChar)
                Case Else
                    strOutput = strOutput & LCase(CurrentChar)
            End Select

            PrevChar = CurrentChar
        Next

        PCase = strOutput
    End Function

End Class