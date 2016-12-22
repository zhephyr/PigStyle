﻿Imports System.DirectoryServices
Imports System.Windows.Forms
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Public Class frmStoreStandardCopier
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
    Dim storeNum As String
    Dim code As String
    Dim reams As Long
    Dim simDup As String
    Dim jobType As String
    Dim laborHrs As Double
    Dim comments As String
    Dim selrow As Integer
    Dim selCol As Integer
    Dim segNum As Long
    Dim ControlCount As Integer
    Dim totalNetDue As String
    Dim InvoiceNumber As Long
    Dim outf As Integer
    Dim conf As Integer
    Dim axfile As Integer
    Dim storestand As Integer
    Dim pageCount As Integer
    Dim totalPage As Integer
    Dim FileHHFC As String
    Dim FileMMDDYY As String
    Dim PrintDate As String
    Dim tempTotal As Double
    Dim optionChk As Integer
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
        dt.Columns.Add(New DataColumn("Code", GetType(String)))
        dt.Columns.Add(New DataColumn("Reams", GetType(String)))
        dt.Columns.Add(New DataColumn("Sim/Dup", GetType(String)))
        dt.Columns.Add(New DataColumn("Job Type", GetType(String)))
        dt.Columns.Add(New DataColumn("Hours", GetType(String)))
        dt.Columns.Add(New DataColumn("Comments", GetType(String)))

        'Creates a empty row to display the header
        dr("Store Number") = String.Empty
        dr("Code") = String.Empty
        dr("Reams") = String.Empty
        dr("Sim/Dup") = String.Empty
        dr("Job Type") = String.Empty
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

        ' If single entry or 1st time multiple entry, move cursor to TxtCode.
        ' ELSE if 2nd time or greater for multiple entry, existing data remains
        ' the same so load record into flexgrid.
        If rdSingle.Checked = True Then
            If Not String.IsNullOrEmpty(txtHours.Text) Then
                Call btnHidden_Click(sender, e)
            Else
                txtCode.Focus() ' move cursor to TxtCode (text box)
            End If
        Else
            If Not String.IsNullOrEmpty(txtHours.Text) Then
                Call btnHidden_Click(sender, e)
            ElseIf optionChk = 1 Then
                'Call LoadFlex()
            Else
                txtCode.Focus() ' move cursor to TxtCode (text box)
            End If
        End If

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
                    drCurrentRow("Code") = txtCode.Text
                    drCurrentRow("Reams") = txtReams.Text
                    drCurrentRow("Sim/Dup") = lstSimDup.Text
                    drCurrentRow("Job Type") = lstJobType.Text
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
        If rdSingle.Checked = True Then
            txtStoreNum.Text = "" ' Clear TxtStoreNumber
            txtCode.Text = "" ' Clear lstDepartment
            txtReams.Text = "" ' Clear TxtReams
            lstSimDup.Text = "" ' Clear lstSimDup
            lstJobType.Text = "" ' Clear TxtLaborHrs
            txtHours.Text = "" ' Clear Txthours
            txtComments.Text = "" ' Clear TxtComment
            txtStoreNum.Focus() ' move cursor to txtRequestor(text box)
        ElseIf rdMultiple.Checked = True Then
            txtStoreNum.Text = ""
            txtStoreNum.Focus()
            optionChk = 1
        End If

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
                    dr("Code") = txtCode.Text
                    dr("Reams") = txtReams.Text
                    dr("Sim/Dup") = lstSimDup.Text
                    dr("Job Type") = lstJobType.Text
                    dr("Hours") = txtHours.Text
                    dr("Comments") = txtComments.Text
                Next
            End If
        End If

    End Sub

    Protected Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ' This routine does the following:
        ' 1. Opens outf(import file), conf(control file) and storestand(text file).
        ' 2. Reads the FlexGrid
        ' 3. Calls the calc routine which calculates the paper cost, labor and click charges.
        '    It also inserts all records into the database, writes the import file and the text file.
        ' 4. Writes the control file
        ' 5. Calls the PrintTextFile routine which creates and prints the Store Standard Copier Charges Report.
        ' 6. Calls the ftpSendFile routine which FTP's the import file and the control file to Endura.
        ' 7. Calls the ClearGrid routine which resets the FlexGrid.

        Dim strsql As String
        Dim c As Integer
        Dim countSeven As Long
        Dim rs As SqlDataReader

        fileCounter = 1

        Try
            ' Hour and minute - used for .imp & .ctl files
            ' FileHHMMSS = Format$(Now, "hhmmss")
            FileHHFC = Format$(Now, "hh") & Format(fileCounter, "00")
            ' Today's date in mmddyy format used for .imp, .ctl & .txt files
            FileMMDDYY = Format(Now, "MMddyy")
            ' Today's date in mm/dd/yyyy format used in Store Standard Copier Charges Report header
            PrintDate = Format(Now, "MM/dd/yyyy")

            'Do Until Not File.Exists("\\ssohome\home\ftp\Data_Processing\Imports\" & "pg" & FileMMDDYY & FileHHFC & ".imp")
            Do Until Not File.Exists("\\ssohome\home\ftp\Data_Processing\DPBillingTest\Imports\" & "pg" & FileMMDDYY & FileHHFC & ".imp")
                fileCounter = fileCounter + 1
                FileHHFC = Format$(Now, "hh") & Format(fileCounter, "00")
            Loop

        Catch ex As Exception

        End Try

        r = Convert.ToInt32(MSGridView.Rows.Count - 1)

        ' If r = 0 then there is no data in flexgrid to be processed
        If r = 0 Then
            Dim strNoData As String
            strNoData = "<script language=javascript> window.alert('There is no data to process');</script>"
            If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
                ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strNoData)
            End If
            Exit Sub
        End If

        conn.Open()

        'File for Endura
        outf = FreeFile()
        'Dim accessfile1 = "\\ssohome\home\ftp\Data_Processing\Imports\" & "pg" & FileMMDDYY & FileHHFC & ".imp"
        Dim accessfile1 = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\Imports\" & "pg" & FileMMDDYY & FileHHFC & ".imp" 'for testing
        FileOpen(outf, accessfile1, OpenMode.Append)

        'Control file for Endura
        conf = FreeFile()
        'Dim accessfile2 = "\\ssohome\home\ftp\Data_Processing\Imports\" & "pg" & FileMMDDYY & FileHHFC & ".ctl"
        Dim accessfile2 = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\Imports\" & "pg" & FileMMDDYY & FileHHFC & ".ctl" 'for testing
        FileOpen(conf, accessfile2, OpenMode.Append)

        ' Text file used for the Store Standard Copier Charges Report
        storestand = FreeFile()
        'Dim accessfile3 = "\\ssohome\home\ftp\Data_Processing\Reports\storestand" & FileMMDDYY & ".txt"
        Dim accessfile3 = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\Reports\storestand" & FileMMDDYY & ".txt" ' for testing
        FileOpen(storestand, accessfile3, OpenMode.Append)

        '*************CREATING AX FILE *********
        axfile = FreeFile()
        Dim accessfile4 = "\\ssohome\home\ftp\AXImports\ARImports\" & "AXpg" & FileMMDDYY & FileHHFC & ".imp" 'for testing
        FileOpen(axfile, accessfile4, OpenMode.Append)
        '***************************************

        ' Get invoice number from dp_store_inv
        strsql = "select invoice_num from dp_store_inv "
        Dim command As New SqlCommand(strsql, conn)
        rs = command.ExecuteReader() ' run sql

        rs.Read()
        InvoiceNumber = rs.Item("invoice_num")

        ' Read FlexGrid
        For c = 1 To r

            ' Read flexgrid and set variables
            storeNum = MSGridView.Rows(c).Cells(1).Text
            code = MSGridView.Rows(c).Cells(2).Text
            reams = MSGridView.Rows(c).Cells(3).Text
            simDup = MSGridView.Rows(c).Cells(4).Text
            jobType = MSGridView.Rows(c).Cells(5).Text
            laborHrs = MSGridView.Rows(c).Cells(6).Text
            comments = MSGridView.Rows(c).Cells(7).Text

            ' Increment invoice number by 1
            InvoiceNumber = InvoiceNumber + 1

            rs.Close() 'Closes connection
            Call calc()

        Next

        'totalNetDue = LSet(tempTotal, 9)
        totalNetDue = tempTotal.ToString("0000000000.00")

        countSeven = ControlCount * 7
        ' Write control file
        PrintLine(conf, "source_id=" & Chr(34) & "PRINTING   " & Chr(34))
        PrintLine(conf, "format_version=1")
        PrintLine(conf, "line_count=" & countSeven.ToString("000000"))
        PrintLine(conf, "total_amounts=" & LSet(totalNetDue, 14))
        PrintLine(conf, "source_date=" & Chr(34) & Format(Now, "MM/dd/yyyy") & Chr(34))

        FileClose(outf)
        FileClose(conf)
        FileClose(storestand)
        FileClose(axfile)

        'Calls CreateCheckFile sub
        Call CreateCheckFile()

        ' Call ftpSendFile.  This routine FTP's the import file and the control file to Endura.
        'Call ftpSendFile("\\ssohome\home\ftp\Data_Processing\Imports\") 'DISABLED FOR TESTING
        Call ftpSendFile("\\ssohome\home\ftp\Data_Processing\DPBillingTest\Imports\")

        ' The ClearGrid routine resets the FlexGrid
        Call ClearGrid()

        ' Clear variables
        tempTotal = 0
        totalNetDue = ""
        storeNum = 0
        code = 0
        reams = 0
        simDup = ""
        jobType = ""
        laborHrs = 0
        comments = ""

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
        Dim mystring As String
        Dim Ream_Pack As Integer
        Dim Sim_Dup As String
        Dim Case_Pack As Integer
        Dim Ream_Per_Case As Integer
        Dim Ream_Cost As Double
        Dim Cost_For_Reams_Used As Double
        Dim Sided As Integer
        Dim Click_Sum As Double
        Dim Sheets As Long
        Dim Total_Click_Charges As Double
        Dim Labor_Cost As Double
        Dim Grand_Total As Double
        Dim RoundTot As Double
        Dim endNum As Integer
        Dim tempNum As String
        Dim tempReamCost As String
        Dim tempLaborCost As String
        Dim tempTotClickCharges As String
        Dim tempGrandTotal As String
        Dim ItemDesc As String
        Dim tempComment As String
        Dim endDept As String
        Dim rs As Object

        endNum = 0
        tempNum = ""
        endDept = ""

        connmdb.Open()
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
            ItemDesc = UCase(rs("Product Name").value)
            ' rs![Case Pack] is a string field which contains a numeric value with characters.
            ' This field needs to be Valed so that only the numeric value is stripped from this
            ' field and put into a varible that is an integer.
            Case_Pack = Val(rs("Case Pack").value)
            ' Example: Divide Case_Pack (5000 sheets) by Ream_Pack (500) to get Ream_Per_Case (10)
            Ream_Per_Case = (Case_Pack / Ream_Pack)
            ' Example: Divide rs!Price (23.36) by Ream_Per_Case (10) to get a rounded Ream_Cost (2.34)
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
            Sheets = (Ream_Pack * reams)
            ' Example: Multiply Click_Sum (.01) by Sheets (1000) to get a rounded Total_Click_Charges (10.00)
            Total_Click_Charges = Math.Round((Sheets * Click_Sum), 2)
            tempTotClickCharges = Format(Total_Click_Charges, "####.00")
            ' Example: Multiply $20.00 by TxtLaborHrs.Text (2) to get a rounded Labor_Cost ($40.00)
            Labor_Cost = Math.Round((laborHrs * 20.0#), 2)
            tempLaborCost = Format(Labor_Cost, "######.00")
            ' Example: Add Labor_Cost (40.00) + Total_Click_Charges (10.00) + Total_Ream_Cost (2.336)
            ' to get a rounded Grand_Total --> (52.34)
            Grand_Total = Math.Round((Cost_For_Reams_Used + Total_Click_Charges + Labor_Cost), 2)
            tempGrandTotal = Format(Grand_Total, "######.00")
            ' Round Grand_Total to 2 places to the right of the decimal (52.336) --> (52.34)
            ' RoundTot = Round(Grand_Total, 2)
            ' MsgBox (RoundTot)
        Else
            Dim strNoCode As String
            strNoCode = "<script language=javascript> window.alert('Code number " & txtCode.Text & " does not exist');</script>"
            If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
                ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strNoCode)
            End If

            txtCode.Text = ""
            txtCode.Focus()

            connmdb.Close()
            rs.close()
            Exit Sub
        End If

        connmdb.Close()

        tempComment = UCase(comments)
        If tempComment = "&NBSP;" Then
            tempComment = ""
        End If

        ' insert records into database
        strsql = "insert into dp_store_standard "
        strsql = strsql & " values ("
        strsql = strsql & "'" & Format(Today, "yyyy-MM-dd") & "',"
        strsql = strsql & InvoiceNumber & ","
        strsql = strsql & "'" & storeNum & "',"
        strsql = strsql & code & ","
        strsql = strsql & reams & ","
        strsql = strsql & "'" & Mid(simDup, 1, 11) & "',"
        strsql = strsql & Labor_Cost & ","
        strsql = strsql & "'" & Mid(jobType, 1, 7) & "',"
        strsql = strsql & Mid(laborHrs, 1, 8) & ","
        strsql = strsql & "'" & Mid(tempComment, 1, 30) & "',"
        strsql = strsql & Ream_Cost & ","
        strsql = strsql & Total_Click_Charges & ","
        strsql = strsql & Grand_Total & ")"

        On Error GoTo noinsert
        Dim command As New SqlCommand(strsql, conn)
        rs = command.ExecuteReader() 'runs sql

        ' Write row to storestand
        PrintLine(storestand, Format(InvoiceNumber, "00000000"), SPC(5), LSet(storeNum, 3), SPC(8), LSet(code, 6), SPC(7), LSet(reams, 4), SPC(5), LSet(Sim_Dup, 8), SPC(6), LSet(jobType, 8), SPC(6), LSet(laborHrs, 4), SPC(4), LSet(tempLaborCost, 7), SPC(5), LSet(Cost_For_Reams_Used, 6), SPC(7), LSet(tempTotClickCharges, 7), SPC(5), LSet(tempGrandTotal, 7), SPC(4), LSet(tempComment, 30)) ' print record to file

        ' if storeNum is the Fresh Brands Credit Union then endNum is 220
        ' If storeNum is a corporate store or DSI then endNum is 220
        ' If storeNum is a franchise store then endNum is 230
        If storeNum = "s25" Or storeNum = "S25" Then
            endNum = 210
        ElseIf (storeNum < "69" Or (storeNum > "400" And storeNum < "413")) Then
            endNum = 210
        Else
            endNum = 230
        End If

        If jobType = "Coupons" Then
            endDept = "6080-030"
        ElseIf jobType = "Flyers" Then
            endDept = "6080-035"
        Else
            endDept = "6020-020"
        End If

        rs.close()

        segNum = segNum + 1

        ' Write the header
        Print(outf, Format(segNum, "0000") & "|")
        Print(outf, "H" & "|")
        Print(outf, "PRINTING  " & "|")
        Print(outf, LSet(storeNum, 3) & "||")
        Print(outf, "INV" & "||")
        Print(outf, Format(Now, "MM/dd/yyyy") & "|||||||||||||||")
        Print(outf, "SSS-SVC" & "|")
        Print(outf, "SSS-SVC" & "|")
        Print(outf, "SSS-SVC" & "|")
        tempNum = Format(Grand_Total, "0.00")
        Print(outf, RSet(tempNum, 7) & "|||")
        Print(outf, tempComment & "||||||||||||||||||||||||||||||||||")
        Print(outf, Format(Now, "MM/dd/yyyy") & "|")
        PrintLine(outf, "PC" & Format(InvoiceNumber, "00000000") & "||||||      ")

        ' write the third "D" data for paper
        Print(outf, Format(segNum, "0000") & "|")
        Print(outf, "D" & "||||||||||||||||||||||||||||||")
        Print(outf, "1" & "|")
        Print(outf, "M" & "|")
        tempNum = Format(Cost_For_Reams_Used, "0.00")
        Print(outf, RSet(tempNum, 7) & "||" & reams & "|" & Ream_Cost & "||")
        Print(outf, ItemDesc & "|||")
        Print(outf, RSet(tempNum, 7) & "||")
        PrintLine(outf, "0.00" & "||||||||||||||||||||||||||      ")

        ' write the third "A" data for paper
        Print(outf, Format(segNum, "0000") & "|")
        Print(outf, "A" & "||||||||||||||||||||||||||||||")
        Print(outf, "1" & "|||||||||||||||||||||||||||")
        Print(outf, "SSS-SVC-630-070-6020-090" & "|")
        tempNum = Format(Cost_For_Reams_Used, "0.00")
        Print(outf, RSet(tempNum, 7) & "|")
        Print(outf, "1" & "||||")
        PrintLine(outf, "XXX-" & LSet(storeNum, 3) & "-000-000-" & endDept & "||||| ")

        ' write the "D" data for labor
        Print(outf, Format(segNum, "0000") & "|")
        Print(outf, "D" & "||||||||||||||||||||||||||||||")
        Print(outf, "2" & "|")
        Print(outf, "M" & "|")
        tempNum = Format(Labor_Cost, "0.00")
        Print(outf, RSet(tempNum, 7) & "||" & laborHrs & "|" & "20.00" & "||")
        Print(outf, "LABOR CHARGE" & "|||")
        Print(outf, RSet(tempNum, 7) & "||")
        PrintLine(outf, "0.00" & "||||||||||||||||||||||||||      ")

        ' write the "A" data for labor
        Print(outf, Format(segNum, "0000") & "|")
        Print(outf, "A" & "||||||||||||||||||||||||||||||")
        Print(outf, "2" & "|||||||||||||||||||||||||||")
        Print(outf, "SSS-SVC-630-070-6000-" & endNum & "|")
        tempNum = Format(Labor_Cost, "0.00")
        Print(outf, RSet(tempNum, 7) & "|")
        Print(outf, "1" & "||||")
        PrintLine(outf, "XXX-" & LSet(storeNum, 3) & "-000-000-" & endDept & "|||||      ")

        ' write the second "D" data for click
        Print(outf, Format(segNum, "0000") & "|")
        Print(outf, "D" & "||||||||||||||||||||||||||||||")
        Print(outf, "3" & "|")
        Print(outf, "M" & "|")
        tempNum = Format(Total_Click_Charges, "0.00")
        Print(outf, RSet(tempNum, 7) & "||" & Sheets & "|" & ".01" & "||")
        Print(outf, "CLICK CHARGE" & "|||")
        Print(outf, RSet(tempNum, 7) & "||")
        PrintLine(outf, "0.00" & "||||||||||||||||||||||||||      ")

        ' write the second "A" data for click
        Print(outf, Format(segNum, "0000") & "|")
        Print(outf, "A" & "||||||||||||||||||||||||||||||")
        Print(outf, "3" & "|||||||||||||||||||||||||||")
        Print(outf, "SSS-SVC-630-070-6020-085" & "|")
        tempNum = Format(Total_Click_Charges, "0.00")
        Print(outf, RSet(tempNum, 7) & "|")
        Print(outf, "1" & "||||")
        PrintLine(outf, "XXX-" & LSet(storeNum, 3) & "-000-000-" & endDept & "|||||      ")

        '***************************CREATE AX FILE*************************************************************
        If jobType = "Coupons" Then
            endDept = "608050"
        ElseIf jobType = "Flyers" Then
            endDept = "608070"
        Else
            endDept = "602040"
        End If

        ' if storeNum is the Fresh Brands Credit Union then endNum is 220
        ' If storeNum is a corporate store or DSI then endNum is 220
        ' If storeNum is a franchise store then endNum is 230
        If storeNum = "s25" Or storeNum = "S25" Then
            'endNum = 210
            endNum = 600120
        ElseIf (storeNum < "69" Or (storeNum > "400" And storeNum < "413")) Then
            'endNum = 210
            endNum = 600120
        Else
            'endNum = 230
            endNum = 600140
        End If

        ' Write the header
        Print(axfile, Format(segNum, "0000") & "|")
        Print(axfile, "H" & "|")
        Print(axfile, "PRINTING  " & "|")
        Print(axfile, LSet(storeNum, 3) & "||")
        Print(axfile, "INV" & "||")
        Print(axfile, Format(Now, "MM/dd/yyyy") & "|||||||||||||||")
        Print(axfile, "SVC" & "|")
        Print(axfile, "PWM" & "|")

        '****GET COMPANY FROM CROSS REF TABLE*****
        Print(axfile, LSet(storeNum, 3) & "|")

        tempNum = Format(Grand_Total, "0.00")
        Print(axfile, RSet(tempNum, 7) & "|||")
        Print(axfile, tempComment & "||||||||||||||||||||||||||||||||||")
        Print(axfile, Format(Now, "MM/dd/yyyy") & "|")
        PrintLine(axfile, "PC" & Format(InvoiceNumber, "00000000") & "|||||||      ")

        ' write the third "D" data for paper
        Print(axfile, Format(segNum, "0000") & "|")
        Print(axfile, "D" & "||||||||||||||||||||||||||||||")
        Print(axfile, "1" & "|")
        Print(axfile, "I" & "|") 'Changed from M to I per keith for AX 10/27/2014
        tempNum = Format(Cost_For_Reams_Used, "0.00")
        Print(axfile, RSet(tempNum, 7) & "||" & reams & "|" & Ream_Cost & "||")
        Print(axfile, ItemDesc & "|||")
        'Print(axfile, RSet(tempNum, 7) & "||")
        Print(axfile, "||")
        PrintLine(axfile, "0.00" & "|||||||||||||||||||||||||||      ")

        ' write the third "A" data for paper
        Print(axfile, Format(segNum, "0000") & "|")
        Print(axfile, "A" & "||||||||||||||||||||||||||||||")
        Print(axfile, "1" & "||||||||||||||||||||||||||||")
        'Print(axfile, "SSS-SVC-630-070-6020-090" & "|")
        Print(axfile, "602150" & "|")
        Print(axfile, "SVC" & "|")
        Print(axfile, "633" & "|||||")

        tempNum = Format(Cost_For_Reams_Used, "0.00")
        Print(axfile, RSet(tempNum, 7) & "|")
        Print(axfile, "1" & "||")
        'PrintLine(axfile, "XXX-" & LSet(storeNum, 3) & "-000-000-" & endDept & "|||||| ")
        Print(axfile, endDept & "|")
        Print(axfile, LSet(storeNum, 3) & "|")
        PrintLine(axfile, "000" & "||||||")

        ' write the "D" data for labor
        Print(axfile, Format(segNum, "0000") & "|")
        Print(axfile, "D" & "||||||||||||||||||||||||||||||")
        Print(axfile, "2" & "|")
        Print(axfile, "I" & "|") 'Changed from M to I per keith for AX 10/27/2014
        tempNum = Format(Labor_Cost, "0.00")
        Print(axfile, RSet(tempNum, 7) & "||" & laborHrs & "|" & "20.00" & "||")
        Print(axfile, "LABOR CHARGE" & "|||")
        'Print(axfile, RSet(tempNum, 7) & "||")
        Print(axfile, "||")
        PrintLine(axfile, "0.00" & "|||||||||||||||||||||||||||      ")

        ' write the "A" data for labor
        Print(axfile, Format(segNum, "0000") & "|")
        Print(axfile, "A" & "||||||||||||||||||||||||||||||")
        Print(axfile, "2" & "||||||||||||||||||||||||||||")
        'Print(axfile, "SSS-SVC-630-070-6000-" & endNum & "|")
        Print(axfile, endNum & "|")
        Print(axfile, "SVC" & "|")
        Print(axfile, "633" & "||65|||")

        tempNum = Format(Labor_Cost, "0.00")
        Print(axfile, RSet(tempNum, 7) & "|")
        Print(axfile, "1" & "||")
        'PrintLine(axfile, "XXX-" & LSet(storeNum, 3) & "-000-000-" & endDept & "||||||      ")
        Print(axfile, endDept & "|")
        Print(axfile, LSet(storeNum, 3) & "|")
        PrintLine(axfile, "000" & "||||||")

        ' write the second "D" data for click
        Print(axfile, Format(segNum, "0000") & "|")
        Print(axfile, "D" & "||||||||||||||||||||||||||||||")
        Print(axfile, "3" & "|")
        Print(axfile, "I" & "|") 'Changed from M to I per keith for AX 10/27/2014
        tempNum = Format(Total_Click_Charges, "0.00")
        Print(axfile, RSet(tempNum, 7) & "||" & Sheets & "|" & Click_Sum & "||") 'Changed from hardcoded .01 to Click_Sum, should change from .01 to .02 depending on simplex/duplex
        Print(axfile, "CLICK CHARGE" & "|||")
        'Print(axfile, RSet(tempNum, 7) & "||")
        Print(axfile, "||")
        PrintLine(axfile, "0.00" & "|||||||||||||||||||||||||||      ")

        ' write the second "A" data for click
        Print(axfile, Format(segNum, "0000") & "|")
        Print(axfile, "A" & "||||||||||||||||||||||||||||||")
        Print(axfile, "3" & "||||||||||||||||||||||||||||")
        'Print(axfile, "SSS-SVC-630-070-6020-085" & "|")
        Print(axfile, "602151" & "|")
        Print(axfile, "SVC", "|")
        Print(axfile, "633" & "|||||")

        tempNum = Format(Total_Click_Charges, "0.00")
        Print(axfile, RSet(tempNum, 7) & "|")
        Print(axfile, "1" & "||")
        'PrintLine(axfile, "XXX-" & LSet(storeNum, 3) & "-000-000-" & endDept & "||||||      ")
        Print(axfile, endDept & "|")
        Print(axfile, LSet(storeNum, 3) & "|")
        PrintLine(axfile, "000" & "||||||")

        '************END AX FILE CREATE**********************************************************


        ' Update invoice number in dp_store_inv
        strsql = "update dp_store_inv set invoice_num ='" & InvoiceNumber & "'"
        Dim command2 As New SqlCommand(strsql, conn)
        rs = command2.ExecuteReader() 'runs Sql
        rs.close() 'Closes connection

        ' ControlCount is used in the Control file.
        ' Each H,D,A,D,A,D,A counts as 1 record.
        ' All records in an import file are added up and used in the control file.
        ControlCount = ControlCount + 1

        ' totalNetDue is used in the control file.
        ' All record totals are added together to get a grand total for the control file.
        tempTotal = tempTotal + Grand_Total
        ' totalNetDue = Format(tempTotal, "######.00")

        Exit Sub

noinsert:

        Dim strNodatabase As String
        strNodatabase = "<script language=javascript> window.alert('error inserting into database');</script>"
        If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
            ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strNodatabase)
        End If
        Exit Sub

    End Sub

    Private Sub CreateCheckFile()

        Dim CheckFile = FreeFile()
        Dim accessFile = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\CheckFile\storestandCheck.txt"   ' For Testing 
        FileOpen(CheckFile, accessFile, OpenMode.Append)
        FileClose()

    End Sub

    Private Sub ftpSendFile(ByVal mydir As String)

        ' This routine FTP's the import file and the control file to Endura.

        Dim bat As Integer
        Dim dat As Integer

        Dim n As String
        Dim found As String
        Dim mystring As String
        Dim myApp As String

        On Error GoTo errfile

        ' create ftpfeed.bat file
        bat = FreeFile()
        'Dim Openbat = "\\ssohome\home\ftp\vb_coupon\programs\dpbillftp.bat"
        Dim Openbat = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\ftp\dpbillftp.bat"
        FileOpen(bat, Openbat, OpenMode.Output)
        'PrintLine(bat, "ftp -s:\\ssohome\home\ftp\vb_coupon\programs\dpbillftp.dat >\\ssohome\home\ftp\vb_coupon\programs\dpbillftp.log")
        PrintLine(bat, "ftp -s:\\ssohome\home\ftp\Data_Processing\DPBillingTest\ftp\dpbillftp.dat >\\ssohome\home\ftp\Data_Processing\DPBillingTest\ftp\dpbillftp.log")

        ' create ftpfeed.dat file
        dat = FreeFile()
        'Dim Opendat = "\\ssohome\home\ftp\vb_coupon\programs\dpbillftp.dat"
        Dim Opendat = "\\ssohome\home\ftp\Data_Processing\DPBillingTest\ftp\dpbillftp.dat"
        FileOpen(dat, Opendat, OpenMode.Output)

        ' open 2200 address 51.0.0.248, username: ftpxfer, pw: ftpass
        PrintLine(dat, "o acctprd")
        'PrintLine(dat, "o mars")        'for testing
        PrintLine(dat, "ftpxfer")
        PrintLine(dat, "ftpass")

        ' change to correct directory
        PrintLine(dat, "cd /usr/fourgen/accounting/import/print_imp/")

        ' put files
        PrintLine(dat, "put " & mydir & "pg" & FileMMDDYY & FileHHFC & ".imp")     ' data file
        PrintLine(dat, "put " & mydir & "pg" & FileMMDDYY & FileHHFC & ".ctl")

        ' change permissions on files so that they are read/write
        PrintLine(dat, "quote site chmod 666 pg" & FileMMDDYY & FileHHFC & ".imp")
        PrintLine(dat, "quote site chmod 666 pg" & FileMMDDYY & FileHHFC & ".ctl")

        ' log off
        PrintLine(dat, "bye")
        FileClose(dat)
        FileClose(bat)

        ' run ftpfeed.bat which runs the FTP through MSDOS prompt
        'n = Shell("\\ssohome\home\ftp\vb_coupon\programs\dpbillftp.bat", vbMaximizedFocus)
        System.Diagnostics.Process.Start("\\ssohome\home\ftp\Data_Processing\DPBillingTest\ftp\dpbillftp.bat")

        Exit Sub

errfile:

        Dim strBatman As String
        strBatman = "<script language=javascript> window.alert('Error creating the bat file');</script>"
        If (Not ClientScript.IsStartupScriptRegistered("clientScript")) Then
            ClientScript.RegisterStartupScript(Page.GetType(), "clientScript", strBatman)
        End If
        Exit Sub

    End Sub

    Protected Sub btnExit_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ' Clear all text boxes
        txtStoreNum.Text = "" ' Clear TxtRequestor
        txtCode.Text = "" ' Clear lstDepartment
        txtReams.Text = "" ' Clear lstSubDepartment
        lstSimDup.Text = "" ' Clear lstSimDup
        lstJobType.Text = "" ' Clear lstJobType
        txtHours.Text = "" ' Clear txtHours
        txtComments.Text = "" ' Clear TxtComments

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