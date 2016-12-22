Imports System.DirectoryServices
Imports System.Windows.Forms
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Public Class frmStorePadCharges
    Inherits System.Web.UI.Page

    Public dirGroups As List(Of String)
    Public permissionType As String

    '** Connections to the server**
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
    Dim padSize As String
    Dim numberOfPads As Integer
    Dim formNumber As String
    Dim InvoiceNumber As Long
    Dim selrow As Integer
    Dim selCol As Integer
    Dim storepads As Integer
    Dim pageCount As Integer
    Dim totalPage As Integer
    Dim FileMMDDYY As String
    Dim PrintDate As String
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
        dt.Columns.Add(New DataColumn("Store Number", GetType(String)))
        dt.Columns.Add(New DataColumn("Pad Size", GetType(String)))
        dt.Columns.Add(New DataColumn("Number Of Pads", GetType(String)))
        dt.Columns.Add(New DataColumn("Form Number", GetType(String)))

        'Creates a empty row to display the header
        dr("Store Number") = String.Empty
        dr("Pad Size") = String.Empty
        dr("Number Of Pads") = String.Empty
        dr("Form Number") = String.Empty

        dt.Rows.Add(dr)

        ' Binds gridview to the datasource and creates instance CurrentTable 
        ViewState("CurrentTable") = dt
        MSGridView.DataSource = dt

        'Updates GridView and hides the empty row.
        MSGridView.DataBind()
        MSGridView.Rows(0).Visible = False

    End Sub

    Protected Sub btnHidden2_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim strsql As String

        connstr.Open()

        strsql = "select pw.custnocrossref from pwm_custcrossref pw"
        strsql = strsql & " join custtable ct"
        strsql = strsql & " on ct.accountnum = pw.custaccount"
        strsql = strsql & " where pw.custnocrossref ='" & txtStoreNum.Text & "'"
        strsql = strsql & " order by pw.custnocrossref"

        rsstr = connstr.Execute(strsql) ' run sql

        ' If TxtStoreNumber does not exist in strcustr(venus) database then tell
        ' user that store number is invalid
        If rsstr.EOF Then
            txtStoreNum.Text = ""
            txtStoreNum.Focus()
            Dim strNoStore As String
            strNoStore = "<script language=javascript> window.alert('Invalid store number');</script>"
            If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
                ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strNoStore)
            End If
            rsstr.Close()
            connstr.Close()
            Exit Sub
        End If

        rsstr.Close()
        connstr.Close()

        If Not String.IsNullOrEmpty(txtFormNum.Text) Then
            Dim strtest As New StringBuilder()
            strtest.Append("<script language=javascript>")
            strtest.Append("validateForm();")
            strtest.Append("</script>")
            If (Not Page.ClientScript.IsStartupScriptRegistered("clientScript")) Then
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strtest.ToString())
            End If
        Else
            lstPadSize.Focus()
        End If

    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        Call btnHidden2_Click(sender, e)

    End Sub

    Protected Sub btnLoadGrid_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ' This button Is executed when the "add" button is clicked or the enter key is pressed in the comments textbox
        'The code below adds the information into the grid view table.

        Dim rowIndex As Integer = 0

        'Reads the current instance of the datatable
        If ViewState("CurrentTable") IsNot Nothing Then
            Dim dtCurrentTable As DataTable = DirectCast(ViewState("CurrentTable"), DataTable)
            Dim drCurrentRow As DataRow = dtCurrentTable.NewRow()

            'Adds the text to the datatable
            If dtCurrentTable.Rows.Count > 0 Then
                For i As Integer = 1 To dtCurrentTable.Rows.Count
                    drCurrentRow("Store Number") = txtStoreNum.Text
                    drCurrentRow("Pad Size") = lstPadSize.Text
                    drCurrentRow("Number Of Pads") = txtNumOfPads.Text
                    drCurrentRow("Form Number") = txtFormNum.Text
                    rowIndex += 1
                Next
                'Accepts changes, updates CurrentTable and binds datasource. Keeps the empty row hidden.
                dtCurrentTable.Rows.Add(drCurrentRow)
                dtCurrentTable.AcceptChanges()
                ViewState("CurrentTable") = dtCurrentTable
                MSGridView.DataSource = dtCurrentTable
                MSGridView.DataBind()
                MSGridView.Rows(0).Visible = False
            End If

        End If

        txtStoreNum.Text = "" ' Clear TxtStoreNumber
        lstPadSize.Text = "" ' Clear lstDepartment
        txtNumOfPads.Text = "" ' Clear TxtReams
        txtFormNum.Text = "" ' Clear lstSimDup
        txtStoreNum.Focus() ' move cursor to txtRequestor(text box)

        'Sends over to the Previous Data Sub
        SetPreviousData()

        Exit Sub

    End Sub

    Private Sub SetPreviousData()

        'This sub displays the former information, so it is not overwritten by the new data
        Dim rowIndex As Integer = 0
        If ViewState("CurrentTable") IsNot Nothing Then
            Dim dt As DataTable = DirectCast(ViewState("CurrentTable"), DataTable)
            Dim dr As DataRow = dt.NewRow()
            If dt.Rows.Count > 0 Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    dr("Store Number") = txtStoreNum.Text
                    dr("Pad Size") = lstPadSize.Text
                    dr("Number Of Pads") = txtNumOfPads.Text
                    dr("Form Number") = txtFormNum.Text
                Next
            End If
        End If

    End Sub

    Protected Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim strsql As String
        Dim c As Integer
        Dim rs As SqlDataReader

        FileMMDDYY = Format(Now, "MMddyy")
        ' Today's date in mm/dd/yyyy format used in Store Standard Copier Charges Report header
        PrintDate = Format(Now, "MM/dd/yyyy")

        r = Convert.ToInt32(MSGridView.Rows.Count - 1)

        conn.Open()

        If r = 0 Then
            Dim strNoData As String
            strNoData = "<script language=javascript> window.alert('There is no data to process');</script>"
            If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
                ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strNoData)
            End If
            Exit Sub
        End If

        storepads = FreeFile()
        'Dim accessfile = "\\ssohome\home\ftp\Data_Processing\Reports\storepads" & FileMMDDYY & ".txt"
        Dim accessfile = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\Reports\storepads" & FileMMDDYY & ".txt" ' for testing
        FileOpen(storepads, accessfile, OpenMode.Append)

        strsql = "select invoice_num from dp_store_inv "
        Dim command As New SqlCommand(strsql, conn)
        rs = command.ExecuteReader() ' run sql

        rs.Read()
        InvoiceNumber = rs.Item("invoice_num")

        For c = 1 To r

            storeNum = MSGridView.Rows(c).Cells(1).Text
            padSize = MSGridView.Rows(c).Cells(2).Text
            numberOfPads = MSGridView.Rows(c).Cells(3).Text
            formNumber = MSGridView.Rows(c).Cells(4).Text

            InvoiceNumber = InvoiceNumber + 1

            rs.Close()
            Call calc()

        Next

        FileClose(storepads)

        'Calls CreateCheckFile sub
        Call CreateCheckFile()

        Call ClearGrid()

        storeNum = 0
        padSize = ""
        numberOfPads = 0
        formNumber = ""

        r = 0
        c = 0

        Dim strComplete As String
        strComplete = "<script language=javascript> window.alert('Process Completed');</script>"
        If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
            ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strComplete)
        End If

        Exit Sub

    End Sub

    Private Sub calc()

        Dim strsql As String
        Dim rs As Object

        strsql = "insert into dp_store_pads "
        strsql = strsql & " values ("
        strsql = strsql & "'" & Format(Today, "yyyy-MM-dd") & "',"
        strsql = strsql & InvoiceNumber & ","
        strsql = strsql & "'" & storeNum & "',"
        strsql = strsql & "'" & Mid(padSize, 1, 13) & "',"
        strsql = strsql & numberOfPads & ","
        strsql = strsql & "'" & Mid(formNumber, 1, 20) & "')"
        Dim command As New SqlCommand(strsql, conn)
        rs = command.ExecuteReader() 'runs sql

        PrintLine(storepads, SPC(8), Format(InvoiceNumber, "00000000"), SPC(9), LSet(storeNum, 3), SPC(12), LSet(padSize, 14), SPC(13), LSet(numberOfPads, 4), SPC(10), LSet(formNumber, 20)) ' print record to file
        rs.close()
        ' Update invoice number in dp_store_inv
        strsql = "update dp_store_inv set invoice_num ='" & InvoiceNumber & "'"
        Dim command2 As New SqlCommand(strsql, conn)
        rs = command2.ExecuteReader() 'runs Sql
        rs.close() 'Closes connection

    End Sub

    Private Sub CreateCheckFile()

        Dim CheckFile = FreeFile()
        Dim accessFile = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\CheckFile\storepadsCheck.txt"   ' For Testing 
        FileOpen(CheckFile, accessFile, OpenMode.Append)
        FileClose()

    End Sub

    Protected Sub btnExit_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ' Clear all text boxes
        txtStoreNum.Text = "" ' Clear TxtRequestor
        lstPadSize.Text = "" ' Clear lstDepartment
        txtNumOfPads.Text = "" ' Clear lstSubDepartment
        txtFormNum.Text = "" ' Clear lstSimDup

        Call ClearGrid() 'Clears GridView Table

        'Moves window to Store Billing

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
        '    'Displays messagebox if user attempts to delete invalid row
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

        'Accepts gridview changes, updates datatable, binds gridview, and keeps row 0 invisible.
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