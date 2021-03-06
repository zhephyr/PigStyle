<%@ Language = VBSCRIPT %>
<% Option Explicit %>
<!--#include virtual ="/web_extras/Includes/DBConn.asp"-->
<%
'getemployee.asp
'   Display employees and stores info. This is an AJAX application
'   Call: None
'   Called by: getemployee.js and is being used only by the Home Intranet,i.e.,
'   http://freshnet
'
'Written by: Jay Xiong
'            October 20, 2009
'Last revised:
'           March 25, 2010
'           Added the deptname to display
'
'			July 5, 2012
'			Refactored the poor HTML and cleaned up the leftover ASP for 
'			stuff Jay took out and I didn't think needed to be there
'reference:
'http://www.w3schools.com/AJAX/ajax_database.asp
'http://www.w3schools.com/Ajax/ajax_example_suggest.asp
'---------------------------------------------------------------------------------------

Dim strSQL
Dim objRS
Dim opt
Dim n
Dim x

objConn.Open
set objRS = Server.CreateObject("ADODB.Recordset")

response.expires=-1

'Search by first name
if(len(request.querystring("f")))then	
    strSQL ="SELECT fname as first, lname as last, phone, deptname as department"
    strSQL = strSQL & " from ph_employees as a, ph_dept as b"
    strSQL = strSQL & " where a.deptid=b.deptid"
    strSQL = strSQL & " and (fname like '" & pcase(request.querystring("f")) & "%'"
    'we need the (fname like 'Dick%' or fname like 'Richard') in order for
    'the SQL to work correctly -- March 25, 2010
    
    'names with different spellings, i.e., Dick and Richard etc...
    'and Robert or Bob
    if(lcase(request.querystring("f")) = "robert" or lcase(request.querystring("f")) = "ro" or lcase(request.querystring("f")) = "rob" or lcase(request.querystring("f")) = "robe" or lcase(request.querystring("f")) = "rober")then
        strSQL = strSQL & " or fname like 'Bob'"
    elseif(lcase(request.querystring("f")) = "bob" or lcase(request.querystring("f")) = "bo")then
        strSQL = strSQL & " or fname like 'Robert'"
    'Elizabeth or Liz or Beth
    elseif(lcase(request.querystring("f")) = "elizabeth" or lcase(request.querystring("f")) = "el" or lcase(request.querystring("f")) = "eli" or lcase(request.querystring("f")) = "eliz")then
        strSQL = strSQL & " or fname like 'Liz'"
    elseif(lcase(request.querystring("f")) = "liz" or lcase(request.querystring("f")) = "li")then
        strSQL = strSQL & " or fname like 'Elizabeth'"
    'Richard or Dick
    elseif(lcase(request.querystring("f")) = "richard" or lcase(request.querystring("f")) = "ri" or lcase(request.querystring("f")) = "ric" or lcase(request.querystring("f")) = "rich")then
        strSQL = strSQL & " or fname like 'Dick'"
    elseif(lcase(request.querystring("f")) = "dick" or lcase(request.querystring("f")) = "di" or lcase(request.querystring("f")) = "dic" or lcase(request.querystring("f")) = "dick")then
        strSQL = strSQL & " or fname like 'Richard'"
    'Anthony or Tony
    elseif(lcase(request.querystring("f")) = "anthony" or lcase(request.querystring("f")) = "ant")then
        strSQL = strSQL & " or fname like 'Tony'"
    elseif(lcase(request.querystring("f")) = "tony" or lcase(request.querystring("f")) = "to" or lcase(request.querystring("f")) = "ton")then
        strSQL = strSQL & " or fname like 'Anthony'"
    'William or Bill
    elseif(lcase(request.querystring("f")) = "bill" or lcase(request.querystring("f")) = "bi" or lcase(request.querystring("f")) = "bil")then
        strSQL = strSQL & " or fname like 'William'"
    elseif(lcase(request.querystring("f")) = "william" or lcase(request.querystring("f")) = "wi" or lcase(request.querystring("f")) = "wil" or lcase(request.querystring("f")) = "will" or lcase(request.querystring("f")) = "willi" or lcase(request.querystring("f")) = "willia")then
        strSQL = strSQL & " or fname like 'bill'"
    'Michael or Mike
    elseif(lcase(request.querystring("f")) = "mike" or lcase(request.querystring("f")) = "mi" or lcase(request.querystring("f")) = "mik")then
        strSQL = strSQL & " or fname like 'Michael'"
    elseif(lcase(request.querystring("f")) = "michael" or lcase(request.querystring("f")) = "mic" or lcase(request.querystring("f")) = "mich" or lcase(request.querystring("f")) = "micha" or lcase(request.querystring("f")) = "michae")then
        strSQL = strSQL & " or fname like 'Mike'"
    'Thomas or Tom
    elseif(lcase(request.querystring("f")) = "thomas" or lcase(request.querystring("f")) = "tho" or lcase(request.querystring("f")) = "thom")then
        strSQL = strSQL & " or fname like 'Tom'"
    elseif(lcase(request.querystring("f")) = "tom" or lcase(request.querystring("f")) = "to")then
        strSQL = strSQL & " or fname like 'Thomas'"
    'Jame or Jim
    elseif(lcase(request.querystring("f")) = "jame" or lcase(request.querystring("f")) = "ja" or lcase(request.querystring("f")) = "jam")then
        strSQL = strSQL & " or fname like 'Jim'"
    elseif(lcase(request.querystring("f")) = "jim" or lcase(request.querystring("f")) = "ji")then
        strSQL = strSQL & " or fname like 'Jame%'"
    end if
    strSQL = strSQL & ")"
    strSQL = strSQL & " ORDER BY fname,lname"
    
    opt = 1
'Search by phone extension
elseif(len(request.querystring("e")))then
	strSQL ="SELECT fname as first, lname as last, phone, deptname as department"
    strSQL = strSQL & " from ph_employees as a, ph_dept as b where a.deptid=b.deptid and phone like '" & request.querystring("e") & "%'"
    strSQL = strSQL & " ORDER BY phone, lname, fname"
    opt = 1
'Search by last name
elseif(len(request.querystring("q")))then
    strSQL ="SELECT  fname as first, lname as last,phone, deptname as department"
    strSQL = strSQL & " from ph_employees as a, ph_dept as b"
    strSQL = strSQL & " where a.deptid=b.deptid"
    strSQL = strSQL & " and lname like '" & pcase(request.querystring("q")) & "%'"
    strSQL = strSQL & " ORDER BY lname, fname"
    opt = 1
    
'Search by city using the pulldown list
elseif(len(request.querystring("s")) and request.querystring("s") <> "Select City")then
    strSQL ="SELECT  storenum as store,phone,name2 as Manager,name1 as name,designation as type,address1,city,state,zip"
    strSQL = strSQL & " from ph_store where city='" & request.querystring("s") & "'"
    strSQL = strSQL & " ORDER BY store"
    opt = 2
'Search by store number from a pulldown list
elseif(len(request.querystring("n")) and request.querystring("n") <> "Select Store")then
    strSQL ="SELECT  storenum as store,phone,name2 as Manager,name1 as name,designation as type,address1,city,state,zip"
    
    if(cdbl(request.querystring("n")) >= 1000)then
        'for store numbers bigger than 1000, people don't put the # char prefix with store numbers
        strSQL = strSQL & " from ph_store where storenum = '" & request.querystring("n") & "'"
    else
        strSQL = strSQL & " from ph_store where storenum = '#" & request.querystring("n") & "'"
    end if
    opt = 2
else
    'just end if user did not select anything to search
    response.end
end if

'response.write(strSQL)

Set objRS = objConn.Execute(strSQL)

n = 0
%>
<div style="text-align:center">
<%
if(opt = 1)then
%>
	<table class="EmployeeGrid" border="1" cellpadding="2" cellspacing="0" width="600px">
<%
else
%>
	<table class="EmployeeGrid" border="1" cellpadding="2" cellspacing="0" width="100%">
<%
end if

if(objRS.EOF) then
%>
		<tr>
			<td style="text-align:center;font-color:red">
				No Records Found
			</td>
		</tr>
<%
else
%>
		<tr>
<%
	FOR EACH x IN objRS.Fields
%>		
			<th><%=pcase(x.name)%></th>
<%			
	NEXT
%>
		</tr>
<%
	DO UNTIL objRS.EOF
		if((n mod 2) = 0)then
%>
		<tr class="EmployeeRowEven">
<%		
		else
%>
		<tr class="EmployeeRowOdd">
<%
		end if

		if(opt = 1)then		
			'employee search only
			FOR EACH x IN objRS.Fields
%>		
			<td><%=trim(x.value)%></td>
<%
			NEXT
		else
			'store search
%>
			<td><%=trim(objRS("store"))%></td>
			<td><%=trim(objRS("phone"))%></td>
			<td><%=trim(objRS("manager"))%></td>
			<td><%=trim(objRS("name"))%></td>
			<td><%=trim(objRS("type"))%></td>
			<td>
				<a href="http://maps.google.com/maps?q=<%=replace(trim(objRS("address1"))," ","+")%>+<%=replace(objRS("city")," ","+")%>+<%=objRS("state")%>+us" target="_blank">
					<%=trim(objRS("address1"))%>
				</a>
			</td>
			<td><%=objRS("city")%></td>
			<td><%=objRS("state")%></td>
			<td><%=objRS("zip")%></td>
<%			
		end if
%>
		</tr>
<%		
		n = n + 1
		objRS.MoveNext
	LOOP
end if
%>
	</table>
</div>

<%
'end of main program

'change text into propercase, i.e., jay into Jay etc...
Function PCase(ByVal strInput)' As String
        Dim I 'As Integer
        Dim CurrentChar, PrevChar 'As Char
        Dim strOutput 'As String

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
        Next 'I

        PCase = strOutput
End Function 
%>