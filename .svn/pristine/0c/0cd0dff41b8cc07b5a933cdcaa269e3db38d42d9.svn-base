Imports System.DirectoryServices
Imports System.Windows.Forms
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Public Class frmOfficeStandardCopier
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
    Dim requestor As String
    Dim department As String
    Dim subDepartment As String
    Dim code As Long
    Dim reams As Long
    Dim simDup As String
    Dim hours As Double
    Dim comments As String
    Dim selrow As Integer
    Dim selCol As Integer
    Dim totalNetDue As String
    Dim tempTotal As Double
    Dim officestand As Integer
    Dim FileHHFC As String
    Dim FileMMDDYY As String
    Dim PrintDate As String
    Dim InvoiceNumber As Long
    Dim pageCount As Integer
    Dim totalPage As Integer
    Dim dtCurrentTable As DataTable
    Dim drCurrentRow As DataRow
    Dim row As GridViewRow

    Dim Sim_Dup As String
    Dim tempReamCost As String
    Dim tempLaborCost As String
    Dim tempTotClickCharges As String
    Dim tempGrandTotal As String
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
        dt.Columns.Add(New DataColumn("Code", GetType(String)))
        dt.Columns.Add(New DataColumn("Reams", GetType(String)))
        dt.Columns.Add(New DataColumn("Sim/Dup", GetType(String)))
        dt.Columns.Add(New DataColumn("Hours", GetType(String)))
        dt.Columns.Add(New DataColumn("Comments", GetType(String)))

        'Creates a empty row to display the header
        dr("Requestor") = String.Empty
        dr("Department") = String.Empty
        dr("Sub Department") = String.Empty
        dr("Code") = String.Empty
        dr("Reams") = String.Empty
        dr("Sim/Dup") = String.Empty
        dr("Hours") = String.Empty
        dr("Comments") = String.Empty
        dt.Rows.Add(dr)

        ' Binds gridview to the datasource and creates instance CurrentTable 
        ViewState("CurrentTable") = dt
        MSGridView.DataSource = dt

        'Updates GridView and hides the empty row.
        MSGridView.DataBind()
        MSGridView.Rows(0).Visible = False

    End Sub

    Protected Sub lstDepartment_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstDepartment.SelectedIndexChanged

        ' This sub displays the sub departments when the user selects a department
        Dim strsql As String
        Dim rs As SqlDataReader

        lstSubDepartment.Items.Clear() ' Clear lstSubDepartment
        lstSubDepartment.Items.Add("")

        'Opens Freshdb connection
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

        'Closes connections
        rs.Close()
        conn.Close()

        lstSubDepartment.Focus() ' Move cursor to lstSubDepartment (text box)

    End Sub

    Protected Sub btnHidden_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ' This sub checks the database to ensure the number in txtCode is in the database, if not it will throw a error.
        ' The sub is executed after it passes the checks in javascript.

        Dim strsql As String
        Dim rs As Object

        connmdb.Open() ' Opens connection to database

        strsql = "select * from [product information] where [code number] ='" & txtCode.Text & "'"
        rs = connmdb.Execute(strsql) 'run sql

        'If the database cannot find a match, it displays an error.
        If (rs.EOF) Then
            txtCode.Text = ""
            txtCode.Focus()
            Dim strInvalidCode As String
            strInvalidCode = "<script language=javascript> window.alert('You must enter a valid code number');</script>"
            If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
                ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strInvalidCode)
            End If
            rs.Close()
            connmdb.Close()
            Exit Sub
        End If

        'Close connections
        rs.Close()
        connmdb.Close()

        'If txtHours contains text, it will run validateComments javascript. Else it will move focus to Reams textbox.
        If Not String.IsNullOrEmpty(txtHours.Text) Then
            Dim strtest As New StringBuilder()
            strtest.Append("<script language=javascript>")
            strtest.Append("validateComments();")
            strtest.Append("</script>")
            If (Not Page.ClientScript.IsStartupScriptRegistered("clientScript")) Then
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strtest.ToString())
            End If
        Else
            txtReams.Focus()
        End If

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
                    drCurrentRow("Requestor") = txtRequestor.Text
                    drCurrentRow("Department") = lstDepartment.Text
                    drCurrentRow("Sub Department") = lstSubDepartment.Text
                    drCurrentRow("Code") = txtCode.Text
                    drCurrentRow("Reams") = txtReams.Text
                    drCurrentRow("Sim/Dup") = lstSimDup.Text
                    drCurrentRow("Hours") = txtHours.Text
                    drCurrentRow("Comments") = txtComments.Text
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

        txtRequestor.Text = "" ' Clear TxtStoreNumber
        lstDepartment.Text = "" ' Clear lstDepartment
        lstSubDepartment.Text = "" ' Clear lstSubDepartment
        txtCode.Text = "" ' Clear TxtCode
        txtReams.Text = "" ' Clear TxtReams
        lstSimDup.Text = "" ' Clear lstSimDup
        txtHours.Text = "" ' Clear TxtLaborHrs
        txtComments.Text = "" ' Clear TxtComments
        txtRequestor.Focus() ' move cursor to txtRequestor

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
                    dr("Requestor") = txtRequestor.Text
                    dr("Department") = lstDepartment.Text
                    dr("Sub Department") = lstSubDepartment.Text
                    dr("Code") = txtCode.Text
                    dr("Reams") = txtReams.Text
                    dr("Sim/Dup") = lstSimDup.Text
                    dr("Hours") = txtHours.Text
                    dr("Comments") = txtComments.Text
                Next
            End If
        End If

    End Sub

    Protected Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ' This sub is executed when the Process button is clicked.

        Dim strsql As String
        Dim c As Integer
        Dim rs As SqlDataReader

        fileCounter = fileCounter + 1

        FileHHFC = Format$(Now, "hh") & Format(fileCounter, "00")
        FileMMDDYY = Format(Now, "MMddyy")
        PrintDate = Format(Now, "MM/dd/yyyy")

        ' We subtract 1 from the gridview row count to account for the hidden row
        r = Convert.ToInt32(MSGridView.Rows.Count - 1)

        ' If r = 0, than means the gridview is empty. It will display messagebox and end sub.
        If r = 0 Then
            Dim strNoData As String
            strNoData = "<script language=javascript> window.alert('There is no data to process');</script>"
            If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
                ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strNoData)
            End If
            Exit Sub
        End If

        conn.Open() ' Opens connection

        'Creates file for output
        officestand = FreeFile()

        'Dim accessFile = "\\ssohome\home\ftp\Data_Processing\Reports\officestand" & FileMMDDYY & ".txt"
        Dim accessFile = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\Reports\officestand" & FileMMDDYY & ".txt"   ' For Testing 
        FileOpen(officestand, accessFile, OpenMode.Append)

        strsql = "select invoice_num from dp_store_inv "
        Dim command As New SqlCommand(strsql, conn)
        rs = command.ExecuteReader() ' run sql

        'Returns the invoice_num from the database
        rs.Read()
        InvoiceNumber = rs.Item("invoice_num")

        'This code runs for each row in the gridview
        For c = 1 To r

            requestor = MSGridView.Rows(c).Cells(1).Text
            department = MSGridView.Rows(c).Cells(2).Text
            subDepartment = MSGridView.Rows(c).Cells(3).Text
            code = MSGridView.Rows(c).Cells(4).Text
            reams = MSGridView.Rows(c).Cells(5).Text
            simDup = MSGridView.Rows(c).Cells(6).Text
            hours = MSGridView.Rows(c).Cells(7).Text
            comments = MSGridView.Rows(c).Cells(8).Text

            InvoiceNumber = InvoiceNumber + 1

            rs.Close() 'Closes connection
            Call calc() 'calls Calc() sub

        Next

        totalNetDue = LSet(tempTotal, 9)

        'Closes file
        FileClose(officestand)

        'Calls CreateCheckFile sub
        Call CreateCheckFile()

        'Moves to ClearGrid() Sub
        Call ClearGrid()

        'Clears variables
        tempTotal = 0
        totalNetDue = ""
        requestor = ""
        department = ""
        subDepartment = ""
        code = 0
        reams = 0
        simDup = ""
        hours = 0
        comments = ""

        r = 0
        c = 0

        'Closes Connections
        rs.Close()
        conn.Close()

        'Displays Completed Messagebox
        Dim strProcessComplete As String
        strProcessComplete = "<script language=javascript> window.alert('Process Completed!');</script>"
        If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
            ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strProcessComplete)
        End If

        Exit Sub
    End Sub

    Private Sub calc()

        Dim strsql As String
        Dim mystring As String
        Dim mydept As String
        Dim mysubdept As String
        Dim Ream_Pack As Long
        Dim Case_Pack As Long
        Dim Ream_Per_Case As Long
        Dim Ream_Cost As Double
        Dim Cost_For_Reams_Used As Double
        Dim Sided As Long
        Dim Click_Sum As Double
        Dim Sheetn As Long
        Dim Total_Click_Charges As Double
        Dim Labor_Cost As Double
        Dim Grand_Total As Double
        Dim dept As Long
        Dim subDept As Long
        Dim tempComment As String
        Dim rs As Object

        connmdb.Open() ' Opens Connection

        ' This SQL will get Case Pack and Price from the dpinv.mdb where the code number matches
        ' the code entered by the user.
        strsql = "select * from [product information] where [code number] ='" & code & "'"
        rs = connmdb.Execute(strsql) ' run sql

        If Not (rs.EOF) Then
            mystring = simDup
            ' Split the string in the ComboBox1 to separate the Ream_Pack (1st position 3 characters)
            ' and Sim_Dup (5th position 6 characters).  Ream_Pack will be used to determine how many
            ' reams are in a case by dividing it into the Case Pack (dpinv.mdb).  Sim_Dup will be used
            ' to determine the click charge --> simplex = .01 and duplex = .02.
            Ream_Pack = Mid(mystring, 1, 3)
            Sim_Dup = Mid(mystring, 5, 7)
            ' rs![Case Pack] is a string field which contains a numeric value with characters.
            ' This field needs to be Valed so that only the numeric value is stripped from this
            ' field and put into a varible that is an integer.
            Case_Pack = Val(rs("Case Pack").value)
            'Case_Pack = Val(rs("Case Pack"))
            ' Example: Divide Case_Pack (5000 sheets) by Ream_Pack (500) to get Ream_Per_Case (10)
            Ream_Per_Case = (Case_Pack / Ream_Pack)
            ' Example: Divide rs!Price (23.36) by Ream_Per_Case (10) to get Ream_Cost (2.336)
            Ream_Cost = Math.Round((rs("Price").value / Ream_Per_Case), 2)

            tempReamCost = Format(Ream_Cost, "####.00")
            ' Example: Multiply TxtReams.Text (2) by Ream_cost (2.336) to get Cost_For_Reams_Used (4.672)
            Cost_For_Reams_Used = Ream_Cost * reams
            If Sim_Dup = "Simplex" Then
                Sided = 1
            Else
                Sided = 2
            End If
            ' If Sided = 1 then it is (1 x .01) to get Click_Sum
            ' If Sided = 2 then it is (2 x .01) to get Click_Sum
            Click_Sum = (Sided * 0.01)
            ' Example: Multiply TxtReams.Text (2) by Ream_Pack (500) to get Sheets (1000)
            Sheetn = (Ream_Pack * reams)
            ' Example: Multiply Click_Sum (.01) by Sheets (1000) to get Total_Click_Charges (10.00)
            Total_Click_Charges = Math.Round((Sheetn * Click_Sum), 2)
            tempTotClickCharges = Format(Total_Click_Charges, "####.00")
            ' Example: Multiply $20.00 by TxtLaborHrs.Text (2) to get Labor_Cost ($40.00)
            Labor_Cost = Math.Round((hours * 20.0#), 2)
            tempLaborCost = Format(Labor_Cost, "######.00")
            ' Example: Add Labor_Cost (40.00) + Total_Click_Charges (10.00) + Total_Ream_Cost (2.336)
            ' to get a rounded Grand_Total (52.34)
            Grand_Total = Math.Round((Cost_For_Reams_Used + Total_Click_Charges + Labor_Cost), 2)
            tempGrandTotal = Format(Grand_Total, "######.00")
            ' Round Grand_Total to 2 places to the right of the decimal (52.336) --> (52.34)
            'RoundTot = Round(Grand_Total, 2)
        Else
            'Displays Message box if code nunber is invalid
            Dim strNoCode As String
            strNoCode = "<script language=javascript> window.alert('Code number " & txtCode.Text & " does not exist ');</script>"
            If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
                ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strNoCode)
            End If
            txtCode.Text = ""
            txtCode.Focus()

        End If
        rs.close() ' Closes Connection
        connmdb.Close() ' Closes Connection

        tempComment = UCase(comments)
        If tempComment = "&NBSP;" Then
            tempComment = ""
        End If

        strsql = "insert into dp_office_standard "
        strsql = strsql & " values ("
        strsql = strsql & "'" & Format(Today, "yyyy-MM-dd") & "',"
        strsql = strsql & InvoiceNumber & ","
        strsql = strsql & "'" & Mid(requestor, 1, 30) & "',"
        strsql = strsql & "'" & Mid(department, 1, 20) & "',"
        strsql = strsql & "'" & Trim(Mid(subDepartment, 1, 25)) & "',"
        strsql = strsql & code & ","
        strsql = strsql & reams & ","
        strsql = strsql & "'" & Mid(simDup, 1, 7) & "',"
        strsql = strsql & Labor_Cost & ","
        strsql = strsql & Mid(hours, 1, 6) & ","
        strsql = strsql & "'" & Trim(Mid(tempComment, 1, 30)) & "',"
        strsql = strsql & Ream_Cost & ","
        strsql = strsql & Total_Click_Charges & ","
        strsql = strsql & Grand_Total & ")"
        Dim command As New SqlCommand(strsql, conn)
        rs = command.ExecuteReader() 'runs sql

        mydept = department
        dept = Mid(mydept, 1, 3)
        mysubdept = subDepartment
        subDept = Mid(mysubdept, 1, 3)

        'Prints line to Officestand file
        PrintLine(officestand, Format(InvoiceNumber, "00000000"), SPC(3), LSet(requestor, 19), SPC(3), Format(dept, "000"), SPC(5), Format(subDept, "000"), SPC(4), Format(code, "00000"), SPC(4), LSet(reams, 4), SPC(4), LSet(Sim_Dup, 7), SPC(4), LSet(hours, 3), SPC(4), LSet(tempLaborCost, 7), SPC(4), LSet(tempReamCost, 6), SPC(7), LSet(tempTotClickCharges, 7), SPC(4), LSet(tempGrandTotal, 7), SPC(4), LSet(tempComment, 30)) ' print record to file

        rs.close() ' Closes connection

        strsql = "update dp_store_inv set invoice_num = '" & InvoiceNumber & "'"
        Dim command2 As New SqlCommand(strsql, conn)
        rs = command2.ExecuteReader() 'runs Sql
        rs.close() 'Closes connection

        tempTotal = tempTotal + Grand_Total

    End Sub

    Private Sub CreateCheckFile()

        Dim CheckFile = FreeFile()
        Dim accessFile = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\CheckFile\officestandCheck.txt"   ' For Testing 
        FileOpen(CheckFile, accessFile, OpenMode.Append)
        FileClose()

    End Sub

    Protected Sub btnExit_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ' Clear all text boxes
        txtRequestor.Text = "" ' Clear TxtRequestor
        lstDepartment.Text = "" ' Clear lstDepartment
        lstSubDepartment.Text = "" ' Clear lstSubDepartment
        txtCode.Text = "" ' Clear TxtCode
        txtReams.Text = "" ' Clear TxtReams
        lstSimDup.Text = "" ' Clear lstSimDup
        txtHours.Text = "" ' Clear txtHours
        txtComments.Text = "" ' Clear TxtComments

        Call ClearGrid() 'Clears GridView Table

        'Moves window to Office Billing
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