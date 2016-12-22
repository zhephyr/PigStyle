﻿Imports System.DirectoryServices
Imports System.Windows.Forms
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Public Class frmOfficeColorCopierCharges
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
    Dim requestor As String
    Dim department As String
    Dim subDepartment As String
    Dim copies As Integer
    Dim simDup As String
    Dim comments As String
    Dim selrow As Integer
    Dim selCol As Integer
    Dim totalNetDue As String
    Dim tempTotal As Double
    Dim FileHHFC As String
    Dim FileMMDDYY As String
    Dim PrintDate As String
    Dim InvoiceNumber As Long
    Dim officecolor As Integer
    Dim pageCount As Integer
    Dim totalPage As Integer
    Dim dtCurrentTable As DataTable
    Dim drCurrentRow As DataRow
    Dim officestand As Integer
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
        connstr.ConnectionString = "Provider=SQLNCLI11;Persist Security Info=False;User ID=sa;Password=2014Sql;Initial Catalog=microsoftdynamicsax2;Data Source=51.0.0.180,1433"

        ' This code stops ClearGrid() From running on postback.
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
        dt.Columns.Add(New DataColumn("Requestor", GetType(String)))
        dt.Columns.Add(New DataColumn("Department", GetType(String)))
        dt.Columns.Add(New DataColumn("Sub Department", GetType(String)))
        dt.Columns.Add(New DataColumn("Copies", GetType(String)))
        dt.Columns.Add(New DataColumn("Sim/Dup", GetType(String)))
        dt.Columns.Add(New DataColumn("Comments", GetType(String)))

        'Creates a empty row to display the header
        dr("Requestor") = String.Empty
        dr("Department") = String.Empty
        dr("Sub Department") = String.Empty
        dr("Copies") = String.Empty
        dr("Sim/Dup") = String.Empty
        dr("Comments") = String.Empty
        dt.Rows.Add(dr)

        ' Creates an instance of the gridview table as CurrentTable
        ViewState("CurrentTable") = dt
        MSGridView.DataSource = dt

        'Refreshes the gridview table and hides the empty row
        MSGridView.DataBind()
        MSGridView.Rows(0).Visible = False

    End Sub

    Protected Sub lstDepartment_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstDepartment.SelectedIndexChanged
        ' This sub displays the sub departments when the user selects a department

        Dim strsql As String
        Dim rs As SqlDataReader

        lstSubDepartment.Items.Clear() ' Clear lstSubDepartment
        lstSubDepartment.Items.Add("")

        conn.Open() 'Opens connection

        ' Pull everything from the dp_office_dept table where dept = what the user picks in
        ' lstDepartment.  These records will provide the user with the sub dept in lstSubDepartment
        ' which correlates with their choice in lstDepartment
        strsql = "select * from dp_office_dept where dept = '" & lstDepartment.Text & "'"
        Dim command As New SqlCommand(strsql, conn)

        rs = command.ExecuteReader() ' run sql

        While rs.Read()
            lstSubDepartment.Items.Add(rs.Item("sub_dept"))
        End While

        'Closes connections
        rs.Close()
        conn.Close()

        lstSubDepartment.Focus() ' Move cursor to lstSubDepartment 

    End Sub

    Protected Sub btnLoadGrid_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        'This button Is executed when the "add" button is clicked or the enter key is pressed in the textbox
        'The code below adds the information into the grid view table.
        If txtRequestor.Text = "" Then
            Exit Sub
        End If
        If lstDepartment.Text = "" Then
            Exit Sub
        End If
        If lstSubDepartment.Text = "" Then
            Exit Sub
        End If
        If txtCopies.Text = "" Then
            Exit Sub
        ElseIf txtCopies.Text > 9999 Then
            Exit Sub
        ElseIf txtCopies.Text < 1 Then
            Exit Sub
        End If
        If lstSimDup.Text = "" Then
            Exit Sub
        End If

        Dim rowIndex As Integer = 0

        'Reads current state of Gridview
        If ViewState("CurrentTable") IsNot Nothing Then
            Dim dtCurrentTable As DataTable = DirectCast(ViewState("CurrentTable"), DataTable)
            Dim drCurrentRow As DataRow = dtCurrentTable.NewRow()

            'Adds textbox and drop down list information into gridview
            If dtCurrentTable.Rows.Count > 0 Then
                For i As Integer = 1 To dtCurrentTable.Rows.Count
                    drCurrentRow("Requestor") = txtRequestor.Text
                    drCurrentRow("Department") = lstDepartment.Text
                    drCurrentRow("Sub Department") = lstSubDepartment.Text
                    drCurrentRow("Copies") = txtCopies.Text
                    drCurrentRow("Sim/Dup") = lstSimDup.Text
                    drCurrentRow("Comments") = txtComments.Text
                    rowIndex += 1
                Next
                'Accepts changes, binds gridview to datatable, keeps empty row hidden
                dtCurrentTable.Rows.Add(drCurrentRow)
                dtCurrentTable.AcceptChanges()
                ViewState("CurrentTable") = dtCurrentTable
                MSGridView.DataSource = dtCurrentTable
                MSGridView.DataBind()
                MSGridView.Rows(0).Visible = False
            End If

        End If

        txtRequestor.Text = "" ' Clear txtRequestor
        lstDepartment.Text = "" ' Clear lstDepartment
        lstSubDepartment.Text = "" ' Clear lstSubDepartment
        txtCopies.Text = "" ' Clear txtCopies
        lstSimDup.Text = "" ' Clear lstSimDup
        txtComments.Text = "" ' Clear TxtComments
        txtRequestor.Focus() ' move cursor to txtRequestor(text box)

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
                    dr("Requestor") = txtRequestor.Text
                    dr("Department") = lstDepartment.Text
                    dr("Sub Department") = lstSubDepartment.Text
                    dr("Copies") = txtCopies.Text
                    dr("Sim/Dup") = lstSimDup.Text
                    dr("Comments") = txtComments.Text
                Next
            End If
        End If
    End Sub

    Protected Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ' This code is executed when the process button is clicked.

        Dim strsql As String
        Dim c As Integer
        Dim rs As SqlDataReader

        fileCounter = fileCounter + 1

        FileHHFC = Format$(Now, "hh") & Format(fileCounter, "00")
        FileMMDDYY = Format(Now, "MMddyy")
        PrintDate = Format(Now, "MM/dd/yyyy")

        ' We subtract 1 from the gridview row count to account for the hidden row
        r = Convert.ToInt32(MSGridView.Rows.Count - 1)

        ' If r = 0, the gridview is empty. A messagebox will display.
        If r = 0 Then
            Dim strNoData As String
            strNoData = "<script language=javascript> window.alert('There is no data to process');</script>"
            If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
                ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strNoData)
            End If
            Exit Sub

        End If

        conn.Open() ' Opens connection to database

        'Opens new file wor input
        officecolor = FreeFile()
        'Dim accessFile = "\\ssohome\home\ftp\Data_Processing\Reports\officecolor" & FileMMDDYY & ".txt"
        Dim accessFile = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\Reports\officecolor" & FileMMDDYY & ".txt"    ' For Testing 
        FileOpen(officecolor, accessFile, OpenMode.Append)

        strsql = "select invoice_num from dp_store_inv "
        Dim command As New SqlCommand(strsql, conn)

        rs = command.ExecuteReader() ' run sql

        rs.Read()
        InvoiceNumber = rs.Item("invoice_num")

        'loops thru to read each row in gridivew
        For c = 1 To r

            requestor = MSGridView.Rows(c).Cells(1).Text
            department = MSGridView.Rows(c).Cells(2).Text
            subDepartment = MSGridView.Rows(c).Cells(3).Text
            copies = MSGridView.Rows(c).Cells(4).Text
            simDup = MSGridView.Rows(c).Cells(5).Text
            comments = MSGridView.Rows(c).Cells(6).Text

            InvoiceNumber = InvoiceNumber + 1

            rs.Close() ' Close connection
            Call calc() ' Moves to calc Sub

        Next

        totalNetDue = Format(tempTotal, "######.00")

        FileClose(officecolor) ' Closes file

        'Calls CreateCheckFile sub
        Call CreateCheckFile()

        Call ClearGrid() 'Moves to cleargrid sub

        'Clears variables
        tempTotal = 0
        totalNetDue = ""
        requestor = ""
        department = ""
        subDepartment = ""
        copies = 0
        simDup = ""
        comments = ""

        r = 0
        c = 0

        conn.Close() ' Closes connection
        rs.Close()

        ' Displays Process complete messagebox
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
        Dim mydept As String
        Dim mysubdept As String
        Dim Sided As Integer
        Dim Total_Copies As Integer
        Dim Total_Cost As Double
        Dim tempCost As String
        Dim tempTotalCost As String
        Dim dept As Integer
        Dim subDept As Integer
        Dim tempComment As String
        Dim rs As Object

        ' Cost of each copy is .15
        tempCost = 0.15

        ' Simplex is 1 copy
        ' Duplex is 2 copies
        If simDup = "Simplex" Then
            Sided = 1
        End If
        If simDup = "Duplex" Then
            Sided = 2
        End If

        ' Example: Multiply sided (2) by Txtcopies (20) to get Total_Copies (40)
        Total_Copies = copies * Sided
        ' Example: Multiply .15 by Total_Copies (40) to get Total_cost (6.00)
        Total_Cost = Total_Copies * 0.15
        tempTotalCost = LSet(Total_Cost, 9)
        ' Round Total_Cost to 2 places to the right of the decimal (6.00) --> (6.00)
        'RoundTot = Round(Total_Cost, 2)

        tempComment = UCase(comments)
        If tempComment = "&NBSP;" Then
            tempComment = ""
        End If

        strsql = "insert into dp_office_color "
        strsql = strsql & " values ("
        strsql = strsql & "'" & Format(Today, "yyyy-MM-dd") & "',"
        strsql = strsql & InvoiceNumber & ","
        strsql = strsql & "'" & Mid(requestor, 1, 30) & "',"
        strsql = strsql & "'" & Mid(department, 1, 20) & "',"
        strsql = strsql & "'" & Trim(Mid(subDepartment, 1, 25)) & "',"
        strsql = strsql & copies & ","
        strsql = strsql & "'" & Mid(simDup, 1, 7) & "',"
        strsql = strsql & "'" & Mid(tempComment, 1, 30) & "',"
        strsql = strsql & Total_Cost & ")"
        Dim command As New SqlCommand(strsql, conn)
        rs = command.ExecuteReader() ' runs sql

        mydept = department
        dept = Mid(mydept, 1, 3)
        mysubdept = subDepartment
        subDept = Mid(mysubdept, 1, 3)

        'writes line to officecolor file
        PrintLine(officecolor, SPC(6), Format(InvoiceNumber, "00000000"), SPC(5), LSet(requestor, 18), SPC(6), Format(dept, "000"), SPC(6), Format(subDept, "000"), SPC(6), LSet(copies, 5), SPC(9), LSet(tempCost, 4), SPC(6), LSet(simDup, 8), SPC(7), LSet(tempTotalCost, 8), SPC(5), LSet(tempComment, 30)) ' print record to file

        rs.close() ' Closes connection

        ' Update invoice number in dp_store_inv
        strsql = "update dp_store_inv set invoice_num ='" & InvoiceNumber & "'"
        Dim command2 As New SqlCommand(strsql, conn)
        rs = command2.ExecuteReader() ' runs sql
        rs.close() 'closes connection

        tempTotal = tempTotal + Total_Cost

    End Sub

    Private Sub CreateCheckFile()

        Dim CheckFile = FreeFile()
        Dim accessFile = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\CheckFile\officecolorCheck.txt"   ' For Testing 
        FileOpen(CheckFile, accessFile, OpenMode.Append)
        FileClose()

    End Sub
    Protected Sub btnExit_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ' Clear all text boxes
        txtRequestor.Text = "" ' Clear TxtRequestor
        lstDepartment.Text = "" ' Clear lstDepartment
        lstSubDepartment.Text = "" ' Clear lstSubDepartment
        txtCopies.Text = "" ' Clear TxtCopies
        lstSimDup.Text = "" ' Clear lstSimDup
        txtComments.Text = "" ' Clear TxtComments

        Call ClearGrid() ' Clears Gridview table

        'Moves browser location to office billing
        Response.Redirect("../frmDataProcessing.aspx", True)

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