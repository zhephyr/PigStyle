﻿Imports System.DirectoryServices
Imports System.Windows.Forms
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Public Class frmOfficePaperUsage
    Inherits System.Web.UI.Page

    Public dirGroups As List(Of String)
    Public permissionType As String

    Dim connectionString As String = ("Server=VG-UNITEDDB;Database=freshdb;Uid=freshuser;Pwd=Fresh2012;")
    Dim conn As SqlConnection
    Dim connmdb As ADODB.Connection
    Dim rsmdb As ADODB.Recordset ' dpinv(Microsoft Access)
    Dim connstr As ADODB.Connection ' venus
    Dim rsstr As ADODB.Recordset ' venus
    Dim fileCounter As Integer ' file counter

    Dim r As Integer
    Dim num_cols As Integer
    Dim col_wid() As Single
    Dim printerName As String
    Dim code As Long
    Dim usage As Integer
    Dim InvoiceNumber As Long
    Dim selrow As Integer
    Dim selCol As Integer
    Dim FileHHFC As String
    Dim FileMMDDYY As String
    Dim PrintDate As String
    Dim officeusage As Integer
    Dim pageCount As Integer
    Dim totalPage As Integer
    Dim totalNetDue As String
    Dim GrandTotal As Double
    Dim dtCurrentTable As DataTable
    Dim drCurrentRow As DataRow
    Dim tempCaseCost As String
    Shared rowIndexes As List(Of Integer) = New List(Of Integer)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        dirGroups = GetGroups("LDAP://pwmdc4/DC=sso,DC=fb,DC=com", HttpContext.Current.User.Identity.Name)

        ' Connection to freshdb
        conn = New SqlConnection(connectionString)
        conn.ConnectionString = "Server=VG-UNITEDDB;Database=freshdb;Uid=freshuser;Pwd=Fresh2012;"

        ' Connection to Microsoft Access database - dpinv
        connmdb = New ADODB.Connection
        connmdb.ConnectionString = ("Driver={Microsoft Access Driver (*.mdb, *.accdb)};Data Source=inventory;" & _
         "DATABASE=dpinv;uid=;pwd=;")

        ' Connection to acctprd
        connstr = New ADODB.Connection
        connstr.ConnectionString = "Provider=SQLNCLI10;Persist Security Info=False;User ID=sa;Password=2014Sql;Initial Catalog=microsoftdynamicsax2;Data Source=51.0.0.180,1433"

        'If page refreshes on post back, it will not run clear grid
        If Not (Page.IsPostBack) Then
            Call ClearGrid()
        End If
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

    Protected Sub ClearGrid()

        ' This routine resets the GridView

        Dim dt As New DataTable
        Dim dr As DataRow = dt.NewRow()

        'Gridview Columns are created
        dt.Columns.Add(New DataColumn("Printer", GetType(String)))
        dt.Columns.Add(New DataColumn("Code", GetType(String)))
        dt.Columns.Add(New DataColumn("Usage By Case", GetType(String)))

        'Creates a empty row to display the header
        dr("Printer") = String.Empty
        dr("Code") = String.Empty
        dr("Usage By Case") = String.Empty
        dt.Rows.Add(dr)

        ' Creates current instance of gridview table as CurrentTable, and binds gridview to the datatable
        ViewState("CurrentTable") = dt
        MSGridView.DataSource = dt

        'Refreshes gridview, hides the empty row
        MSGridView.DataBind()
        MSGridView.Rows(0).Visible = False

    End Sub

    Protected Sub btnHidden_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ' This sub checks the database to ensure the number in txtCode is in the database, if not it will throw a error.

        Dim strsql As String
        Dim rs As Object

        connmdb.Open()

        strsql = "select * from [product information] where [code number] ='" & txtCode.Text & "'"
        rs = connmdb.Execute(strsql) 'run sql

        'This code runs if there is no match between code and database. Displays messagebox
        If (rs.EOF) Then
            txtCode.Text = ""
            txtCode.Focus()
            Dim strInvalidCode As String
            strInvalidCode = "<script language=javascript> window.alert('You must enter a valid code number');</script>"
            If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
                ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strInvalidCode)
            End If
            Exit Sub
        End If

        'Closes connections
        rs.Close()
        connmdb.Close()

        'If txtCase contains text, it will run validateCase javascript. Else it will move focus to txtCase
        If Not String.IsNullOrEmpty(txtCase.Text) Then
            Dim strCase As New StringBuilder()
            strCase.Append("<script language=javascript>")
            strCase.Append("validateCase();")
            strCase.Append("</script>")
            If (Not Page.ClientScript.IsStartupScriptRegistered("clientScript")) Then
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strCase.ToString())
            End If
        Else
            txtCase.Focus()
        End If

    End Sub

    Protected Sub btnLoadGrid_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        'This button Is executed when the "add" button is clicked or the enter key is pressed in the comments textbox
        'The code below adds the information into the grid view table.
        Dim rowIndex As Integer = 0

        'Reads current instance of gridview
        If ViewState("CurrentTable") IsNot Nothing Then
            Dim dtCurrentTable As DataTable = DirectCast(ViewState("CurrentTable"), DataTable)
            Dim drCurrentRow As DataRow = dtCurrentTable.NewRow()
            'writes text box and drop down list values to gridview
            If dtCurrentTable.Rows.Count > 0 Then
                For i As Integer = 1 To dtCurrentTable.Rows.Count
                    drCurrentRow("Printer") = lstPrinter.Text
                    drCurrentRow("Code") = txtCode.Text
                    drCurrentRow("Usage By Case") = txtCase.Text
                    rowIndex += 1
                Next
                'Approves changes, updates CurrentTable, binds gridview to datatable, and keeps empty row hidden
                dtCurrentTable.Rows.Add(drCurrentRow)
                dtCurrentTable.AcceptChanges()
                ViewState("CurrentTable") = dtCurrentTable
                MSGridView.DataSource = dtCurrentTable
                MSGridView.DataBind()
                MSGridView.Rows(0).Visible = False
            End If

        End If

        lstPrinter.Text = "" ' Clear lstPrinter
        txtCode.Text = "" ' Clear TxtCode
        txtCase.Text = "" ' Clear txtCase
        lstPrinter.Focus() ' move cursor to lstPrinter

        'Sends over to the Previous Data Sub
        SetPreviousData()

        Exit Sub

    End Sub

    Private Sub SetPreviousData()

        'This sub ensures that the previous data in the gridview is not overwritten

        Dim rowIndex As Integer = 0
        If ViewState("CurrentTable") IsNot Nothing Then
            Dim dt As DataTable = DirectCast(ViewState("CurrentTable"), DataTable)
            Dim dr As DataRow = dt.NewRow()
            If dt.Rows.Count > 0 Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    dr("Printer") = lstPrinter.Text
                    dr("Code") = txtCode.Text
                    dr("Usage By Case") = txtCase.Text
                Next
            End If
        End If

    End Sub

    Protected Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim strsql As String
        Dim c As Integer
        Dim rs As SqlDataReader

        fileCounter = fileCounter + 1

        FileHHFC = Format$(Now, "hh") & Format(fileCounter, "00")
        FileMMDDYY = Format(Now, "MMddyy")
        PrintDate = Format(Now, "MM/dd/yyyy")

        ' We subtract 1 from the gridview row count to account for the hidden row
        r = Convert.ToInt32(MSGridView.Rows.Count - 1)

        'if r = 0 then gridview is empty. It will display messagebox
        If r = 0 Then
            Dim strNoData As String
            strNoData = "<script language=javascript> window.alert('There is no data to process');</script>"
            If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
                ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strNoData)
            End If
            Exit Sub
        End If

        conn.Open() 'Opens connection

        officeusage = FreeFile()
        'Dim accessFile = "\\ssohome\home\ftp\Data_Processing\Reports\officeusage" & FileMMDDYY & ".txt"   'Open File for Output
        Dim accessFile = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\Reports\officeusage" & FileMMDDYY & ".txt"   'Open File for Output
        FileOpen(officeusage, accessFile, OpenMode.Append)

        strsql = "select invoice_num from dp_store_inv "
        Dim command As New SqlCommand(strsql, conn)

        rs = command.ExecuteReader() ' run sql

        rs.Read()
        InvoiceNumber = rs.Item("invoice_num")

        'This code will loop through every row in gridview
        For c = 1 To r

            printerName = MSGridView.Rows(c).Cells(1).Text
            code = MSGridView.Rows(c).Cells(2).Text
            usage = MSGridView.Rows(c).Cells(3).Text

            InvoiceNumber = InvoiceNumber + 1

            rs.Close() 'Close Connection
            Call calc() ' call calc sub

        Next

        FileClose(officeusage) 'closes file

        'Calls CreateCheckFile sub
        Call CreateCheckFile()

        Call ClearGrid() 'call clear grid sub

        GrandTotal = 0
        totalNetDue = ""
        printerName = ""
        code = 0
        usage = 0

        r = 0
        c = 0

        rs.Close()
        conn.Close() 'closes connection

        'Displays success messagebox
        Dim strProcessComplete As String
        strProcessComplete = "<script language=javascript> window.alert('Process Completed!');</script>"
        If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
            ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strProcessComplete)
        End If
        Exit Sub

        Exit Sub

    End Sub

    Private Sub calc()

        Dim strsql As String
        Dim Total_Case_Cost As Double
        Dim rs As Object

        connmdb.Open() 'opens connection

        strsql = "select * from [product information] where [code number] ='" & code & "'"
        rs = connmdb.Execute(strsql) ' run sql

        If Not (rs.EOF) Then
            Total_Case_Cost = Math.Round((rs("Price").value * usage), 2)
            tempCaseCost = Format(Total_Case_Cost, "#####0.00")
        Else
            'This code will display messagebox if no code is found in database
            Dim strInvalidCode As String
            strInvalidCode = "<script language=javascript> window.alert('Code number " & txtCode.Text & " does not exist');</script>"
            If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
                ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strInvalidCode)
            End If
            txtCode.Text = ""
            txtCode.Focus()

        End If
        rs.close() 'close connection
        connmdb.Close() 'close connection

        strsql = "insert into dp_office_usage "
        strsql = strsql & " values ("
        strsql = strsql & "'" & Format(Today, "yyyy-MM-dd") & "',"
        strsql = strsql & InvoiceNumber & ","
        strsql = strsql & "'" & Mid(printerName, 1, 30) & "',"
        strsql = strsql & code & ","
        strsql = strsql & usage & ","
        strsql = strsql & Total_Case_Cost & ")"
        Dim command As New SqlCommand(strsql, conn)
        rs = command.ExecuteReader() ' run sql

        'write line to officeusage
        PrintLine(officeusage, SPC(8), Format(InvoiceNumber, "00000000"), SPC(10), LSet(printerName, 22), SPC(11), LSet(code, 6), SPC(11), LSet(usage, 6), SPC(11), LSet(tempCaseCost, 9)) ' print record to file
        rs.close() 'closes connection

        ' Update invoice number in dp_store_inv
        strsql = "update dp_store_inv set invoice_num ='" & InvoiceNumber & "'"
        Dim command2 As New SqlCommand(strsql, conn)
        rs = command2.ExecuteReader() 'run sql
        rs.close() 'closes connection

        GrandTotal = Math.Round((GrandTotal + Total_Case_Cost), 2)
        totalNetDue = Format(GrandTotal, "#####0.00")

    End Sub

    Private Sub CreateCheckFile()

        Dim CheckFile = FreeFile()
        Dim accessFile = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\CheckFile\officeusageCheck.txt"   ' For Testing 
        FileOpen(CheckFile, accessFile, OpenMode.Append)
        FileClose()

    End Sub

    Protected Sub btnExit_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ' Clear all text boxes

        lstPrinter.Text = "" ' Clear lstPrinter
        txtCode.Text = "" ' Clear TxtCode
        txtCase.Text = "" ' Clear txtCase

        Call ClearGrid() 'clears gridview

        'moves browser back to office billing page
        Server.Transfer("frmOfficeBilling.aspx", True)

    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ' Any row can be deleted except 0 which is associated with the header.
        ' This code allows the user to delete the selected row. However row 0 must remain.
        Dim dtCurrentTable As DataTable = DirectCast(ViewState("CurrentTable"), DataTable)

        'If MSGridView.SelectedIndex > 0 Then
        '    dtCurrentTable.Rows(MSGridView.SelectedIndex).Delete()
        '    MSGridView.SelectedIndex = 0
        'Else
        '    Dim strRowDelete As String
        '    strRowDelete = "<script language=javascript> window.alert('Invalid row selection');</script>"
        '    If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
        '        ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strRowDelete)
        '    End If
        '    Exit Sub

        'End If

        If rowIndexes.Count < 1 Then
            Dim strRowDelete As String = "<script language=javascript> window.alert('Invalid row selection');</script>"
            If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
                ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strRowDelete)
            End If
            Exit Sub
        Else
            For Each index In rowIndexes
                dtCurrentTable.Rows(index).Delete()
            Next
        End If

        'Accepts changes, binds gridview to datatable, updates gridview, and keeps empty row hidden
        rowIndexes.Clear()
        dtCurrentTable.AcceptChanges()
        ViewState("CurrentTable") = dtCurrentTable
        MSGridView.DataSource = dtCurrentTable
        MSGridView.DataBind()
        MSGridView.Rows(0).Visible = False

    End Sub

    Protected Sub selectChk_CheckedChanged(sender As Object, e As EventArgs)
        Dim row As GridViewRow = CType(CType(sender, System.Web.UI.Control).Parent.Parent, GridViewRow)
        If (CType(row.FindControl("selectChk"), System.Web.UI.WebControls.CheckBox)).Checked Then
            row.BackColor = Drawing.Color.LightGray
            rowIndexes.Add(row.RowIndex)
        ElseIf Not (CType(row.FindControl("selectChk"), System.Web.UI.WebControls.CheckBox)).Checked Then
            rowIndexes.Remove(row.RowIndex)
            row.BackColor = Nothing
        End If
    End Sub

End Class