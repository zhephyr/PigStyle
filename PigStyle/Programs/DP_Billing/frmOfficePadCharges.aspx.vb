Imports System.DirectoryServices
Imports System.Windows.Forms
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Public Class frmOfficePadCharges
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
    Dim requestor As String
    Dim department As String
    Dim subDepartment As String
    Dim padSize As String
    Dim numberOfPads As Integer
    Dim formNumber As String
    Dim InvoiceNumber As Long
    Dim selrow As Integer
    Dim selCol As Integer
    Dim FileHHFC As String
    Dim FileMMDDYY As String
    Dim PrintDate As String
    Dim officepads As Integer
    Dim pageCount As Integer
    Dim totalPage As Integer
    Dim dtCurrentTable As DataTable
    Dim drCurrentRow As DataRow
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

        'This code prevents calling cleargrid on postback
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
        dt.Columns.Add(New DataColumn("Pad Size", GetType(String)))
        dt.Columns.Add(New DataColumn("Number of Pads", GetType(String)))
        dt.Columns.Add(New DataColumn("Form Number", GetType(String)))

        'Creates a empty row to display the header
        dr("Requestor") = String.Empty
        dr("Department") = String.Empty
        dr("Sub Department") = String.Empty
        dr("Pad Size") = String.Empty
        dr("Number of Pads") = String.Empty
        dr("Form Number") = String.Empty

        dt.Rows.Add(dr)

        ViewState("CurrentTable") = dt
        MSGridView.DataSource = dt

        MSGridView.DataBind()
        MSGridView.Rows(0).Visible = False

    End Sub

    Protected Sub lstDepartment_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstDepartment.SelectedIndexChanged

        ' This sub displays the sub departments when the user selects a department

        Dim strsql As String
        Dim rs As SqlDataReader

        lstSubDepartment.Items.Clear() ' Clear lstSubDepartment
        lstSubDepartment.Items.Add("")
        conn.Open()
        ' Pull everything from the dp_office_dept table where dept = what the user picks in
        ' lstDepartment.  These records will provide the user with the sub dept in lstSubDepartment
        ' which correlates with their choice in lstDepartment
        strsql = "select * from dp_office_dept where dept = '" & lstDepartment.Text & "'"
        Dim command As New SqlCommand(strsql, conn)

        rs = command.ExecuteReader() ' run sql

        While rs.Read()
            lstSubDepartment.Items.Add(rs.Item("sub_dept"))
        End While

        rs.Close()
        conn.Close()

        lstSubDepartment.Focus() ' Move cursor to lstSubDepartment (text box)

    End Sub

    Protected Sub btnLoadGrid_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ' This button Is executed when the "add" button is clicked or the enter key is pressed in the comments textbox
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
        If lstPadSize.Text = "" Then
            Exit Sub
        End If
        If txtNumOfPads.Text = "" Then
            Exit Sub
        End If
        If txtFormNumber.Text = "" Then
            Exit Sub
        End If


        Dim rowIndex As Integer = 0

        If ViewState("CurrentTable") IsNot Nothing Then
            Dim dtCurrentTable As DataTable = DirectCast(ViewState("CurrentTable"), DataTable)
            Dim drCurrentRow As DataRow = dtCurrentTable.NewRow()

            If dtCurrentTable.Rows.Count > 0 Then
                For i As Integer = 1 To dtCurrentTable.Rows.Count
                    drCurrentRow("Requestor") = txtRequestor.Text
                    drCurrentRow("Department") = lstDepartment.Text
                    drCurrentRow("Sub Department") = lstSubDepartment.Text
                    drCurrentRow("Pad Size") = lstPadSize.Text
                    drCurrentRow("Number of Pads") = txtNumOfPads.Text
                    drCurrentRow("Form Number") = txtFormNumber.Text
                    rowIndex += 1
                Next

                dtCurrentTable.Rows.Add(drCurrentRow)
                dtCurrentTable.AcceptChanges()
                ViewState("CurrentTable") = dtCurrentTable
                MSGridView.DataSource = dtCurrentTable
                MSGridView.DataBind()
                MSGridView.Rows(0).Visible = False
            End If

        End If

        txtRequestor.Text = "" ' Clear TxtStoreNumber
        lstDepartment.Text = "" ' Clear lstDepartment
        lstSubDepartment.Text = "" ' Clear lstSubDepartment
        lstPadSize.Text = "" ' Clear lstPadSize
        txtNumOfPads.Text = "" ' Clear txtNumOfPads
        txtFormNumber.Text = "" ' Clear txtFormNumber
        txtRequestor.Focus() ' move cursor to txtRequestor(text box)

        'Sends over to the Previous Data Sub
        SetPreviousData()

        Exit Sub

    End Sub

    Private Sub SetPreviousData()
        'This sub moves the previous data to a new row so its not overwritten

        Dim rowIndex As Integer = 0
        If ViewState("CurrentTable") IsNot Nothing Then
            Dim dt As DataTable = DirectCast(ViewState("CurrentTable"), DataTable)
            Dim dr As DataRow = dt.NewRow()
            If dt.Rows.Count > 0 Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    dr("Requestor") = txtRequestor.Text
                    dr("Department") = lstDepartment.Text
                    dr("Sub Department") = lstSubDepartment.Text
                    dr("Pad Size") = lstPadSize.Text
                    dr("Number of Pads") = txtNumOfPads.Text
                    dr("Form Number") = txtFormNumber.Text
                Next
            End If
        End If

    End Sub

    Protected Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim strsql As String
        Dim c As Integer
        Dim rs As SqlDataReader

        fileCounter = fileCounter + 1

        ' FileHHMMSS = Format$(Now, "hhmmss")
        FileHHFC = Format$(Now, "hh") & Format(fileCounter, "00")
        FileMMDDYY = Format(Now, "MMddyy")
        PrintDate = Format(Now, "MM/dd/yyyy")

        ' We subtract 1 from the gridview row count to account for the hidden row
        r = Convert.ToInt32(MSGridView.Rows.Count - 1)

        If r = 0 Then
            Dim strNoData As String
            strNoData = "<script language=javascript> window.alert('There is no data to process');</script>"
            If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
                ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strNoData)
            End If
            Exit Sub
        End If

        conn.Open()

        officepads = FreeFile()
        'Dim accessFile = "\\ssohome\home\ftp\Data_Processing\Reports\officepads" & FileMMDDYY & ".txt"   ' Open file for output
        Dim accessFile = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\Reports\officepads" & FileMMDDYY & ".txt"   ' For Testing 
        FileOpen(officepads, accessFile, OpenMode.Append)

        strsql = "select invoice_num from dp_store_inv "
        Dim command As New SqlCommand(strsql, conn)

        rs = command.ExecuteReader() ' run sql

        rs.Read()
        InvoiceNumber = rs.Item("invoice_num")

        For c = 1 To r

            requestor = MSGridView.Rows(c).Cells(1).Text
            department = MSGridView.Rows(c).Cells(2).Text
            subDepartment = MSGridView.Rows(c).Cells(3).Text
            padSize = MSGridView.Rows(c).Cells(4).Text
            numberOfPads = MSGridView.Rows(c).Cells(5).Text
            formNumber = MSGridView.Rows(c).Cells(6).Text

            InvoiceNumber = InvoiceNumber + 1

            rs.Close()
            Call calc()

        Next

        FileClose(officepads)

        'Calls CreateCheckFile sub
        Call CreateCheckFile()

        Call ClearGrid()

        requestor = ""
        department = ""
        subDepartment = ""
        padSize = ""
        numberOfPads = 0
        formNumber = ""

        r = 0
        c = 0

        Dim strProcessComplete As String
        strProcessComplete = "<script language=javascript> window.alert('Process Completed!');</script>"
        If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
            ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strProcessComplete)
        End If
        Exit Sub

        conn.Close()

        Exit Sub

    End Sub

    Private Sub calc()

        Dim strsql As String
        Dim mydept As String
        Dim mysubdept As String
        Dim dept As Integer
        Dim subDept As Integer
        Dim rs As Object


        strsql = "insert into dp_office_pads "
        strsql = strsql & " values ("
        strsql = strsql & "'" & Format(Today, "yyyy-MM-dd") & "',"
        strsql = strsql & "'" & InvoiceNumber & "',"
        strsql = strsql & "'" & Mid(requestor, 1, 30) & "',"
        strsql = strsql & "'" & Mid(department, 1, 20) & "',"
        strsql = strsql & "'" & Trim(Mid(subDepartment, 1, 25)) & "',"
        strsql = strsql & "'" & Mid(padSize, 1, 13) & "',"
        strsql = strsql & "'" & numberOfPads & "',"
        strsql = strsql & "'" & Mid(formNumber, 1, 20) & "')"
        Dim command As New SqlCommand(strsql, conn)
        rs = command.ExecuteReader()

        mydept = department
        dept = Mid(mydept, 1, 3)
        mysubdept = subDepartment
        subDept = Mid(mysubdept, 1, 3)

        PrintLine(officepads, SPC(6), Format(InvoiceNumber, "00000000"), SPC(6), LSet(requestor, 19), SPC(7), Format(dept, "000"), SPC(7), Format(subDept, "000"), SPC(7), LSet(padSize, 14), SPC(8), LSet(numberOfPads, 4), SPC(7), LSet(formNumber, 20)) ' print record to file
        rs.close()
        ' Update invoice number in dp_store_inv
        strsql = "update dp_store_inv set invoice_num ='" & InvoiceNumber & "'"
        Dim command2 As New SqlCommand(strsql, conn)
        rs = command2.ExecuteReader()
        rs.close()

        Exit Sub


    End Sub

    Private Sub CreateCheckFile()

        Dim CheckFile = FreeFile()
        Dim accessFile = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\CheckFile\officepadsCheck.txt"   ' For Testing 
        FileOpen(CheckFile, accessFile, OpenMode.Append)
        FileClose()

    End Sub

    Protected Sub btnExit_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ' Clear all text boxes
        txtRequestor.Text = "" ' Clear TxtRequestor
        lstDepartment.Text = "" ' Clear lstDepartment
        lstSubDepartment.Text = "" ' Clear lstSubDepartment
        lstPadSize.Text = "" ' Clear lstPadSize
        txtNumOfPads.Text = "" ' Clear txtNumOfPads
        txtFormNumber.Text = "" ' Clear txtFormNumber

        Call ClearGrid()

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