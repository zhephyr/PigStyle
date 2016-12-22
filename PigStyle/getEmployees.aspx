﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="getEmployees.aspx.vb" Inherits="PigStyle.getEmployees" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%
    Response.AppendHeader("Access-Control-Allow-Origin:", "*")
    Response.AppendHeader("Content-type", "application/json")
    Response.ContentType = "application/json; charset=utf-8"
    Response.Clear()
    Response.BufferOutput = True
    
    Dim connectionStr As String = "Persist Security Info=False; Initial Catalog=freshdb; Data Source=51.0.1.44,1433; User ID=freshuser; Password=Fresh2012;"
    Dim connection As New SqlConnection(connectionStr)

    Dim strSQL As String
    
    

    If (Len(Request.QueryString("f"))) Then
        strSQL = "SELECT fname as first, lname as last, phone, deptname as department"
        strSQL = strSQL & " from ph_employees as a, ph_dept as b"
        strSQL = strSQL & " where a.deptid=b.deptid"
        strSQL = strSQL & " and (fname like '" & PCase(Request.QueryString("f")) & "%'"
        'we need the (fname like 'Dick%' or fname like 'Richard') in order for
        'the SQL to work correctly -- March 25, 2010

        'names with different spellings, i.e., Dick and Richard etc...
        'and Robert or Bob
        If (LCase(Request.QueryString("f")) = "robert" Or LCase(Request.QueryString("f")) = "ro" Or LCase(Request.QueryString("f")) = "rob" Or LCase(Request.QueryString("f")) = "robe" Or LCase(Request.QueryString("f")) = "rober") Then
            strSQL = strSQL & " or fname like 'Bob'"
        ElseIf (LCase(Request.QueryString("f")) = "bob" Or LCase(Request.QueryString("f")) = "bo") Then
            strSQL = strSQL & " or fname like 'Robert'"
            'Elizabeth or Liz or Beth
        ElseIf (LCase(Request.QueryString("f")) = "elizabeth" Or LCase(Request.QueryString("f")) = "el" Or LCase(Request.QueryString("f")) = "eli" Or LCase(Request.QueryString("f")) = "eliz") Then
            strSQL = strSQL & " or fname like 'Liz'"
        ElseIf (LCase(Request.QueryString("f")) = "liz" Or LCase(Request.QueryString("f")) = "li") Then
            strSQL = strSQL & " or fname like 'Elizabeth'"
            'Richard or Dick
        ElseIf (LCase(Request.QueryString("f")) = "richard" Or LCase(Request.QueryString("f")) = "ri" Or LCase(Request.QueryString("f")) = "ric" Or LCase(Request.QueryString("f")) = "rich") Then
            strSQL = strSQL & " or fname like 'Dick'"
        ElseIf (LCase(Request.QueryString("f")) = "dick" Or LCase(Request.QueryString("f")) = "di" Or LCase(Request.QueryString("f")) = "dic" Or LCase(Request.QueryString("f")) = "dick") Then
            strSQL = strSQL & " or fname like 'Richard'"
            'Anthony or Tony
        ElseIf (LCase(Request.QueryString("f")) = "anthony" Or LCase(Request.QueryString("f")) = "ant") Then
            strSQL = strSQL & " or fname like 'Tony'"
        ElseIf (LCase(Request.QueryString("f")) = "tony" Or LCase(Request.QueryString("f")) = "to" Or LCase(Request.QueryString("f")) = "ton") Then
            strSQL = strSQL & " or fname like 'Anthony'"
            'William or Bill
        ElseIf (LCase(Request.QueryString("f")) = "bill" Or LCase(Request.QueryString("f")) = "bi" Or LCase(Request.QueryString("f")) = "bil") Then
            strSQL = strSQL & " or fname like 'William'"
        ElseIf (LCase(Request.QueryString("f")) = "william" Or LCase(Request.QueryString("f")) = "wi" Or LCase(Request.QueryString("f")) = "wil" Or LCase(Request.QueryString("f")) = "will" Or LCase(Request.QueryString("f")) = "willi" Or LCase(Request.QueryString("f")) = "willia") Then
            strSQL = strSQL & " or fname like 'bill'"
            'Michael or Mike
        ElseIf (LCase(Request.QueryString("f")) = "mike" Or LCase(Request.QueryString("f")) = "mi" Or LCase(Request.QueryString("f")) = "mik") Then
            strSQL = strSQL & " or fname like 'Michael'"
        ElseIf (LCase(Request.QueryString("f")) = "michael" Or LCase(Request.QueryString("f")) = "mic" Or LCase(Request.QueryString("f")) = "mich" Or LCase(Request.QueryString("f")) = "micha" Or LCase(Request.QueryString("f")) = "michae") Then
            strSQL = strSQL & " or fname like 'Mike'"
            'Thomas or Tom
        ElseIf (LCase(Request.QueryString("f")) = "thomas" Or LCase(Request.QueryString("f")) = "tho" Or LCase(Request.QueryString("f")) = "thom") Then
            strSQL = strSQL & " or fname like 'Tom'"
        ElseIf (LCase(Request.QueryString("f")) = "tom" Or LCase(Request.QueryString("f")) = "to") Then
            strSQL = strSQL & " or fname like 'Thomas'"
            'Jame or Jim
        ElseIf (LCase(Request.QueryString("f")) = "jame" Or LCase(Request.QueryString("f")) = "ja" Or LCase(Request.QueryString("f")) = "jam") Then
            strSQL = strSQL & " or fname like 'Jim'"
        ElseIf (LCase(Request.QueryString("f")) = "jim" Or LCase(Request.QueryString("f")) = "ji") Then
            strSQL = strSQL & " or fname like 'Jame%'"
        End If
        strSQL = strSQL & ")"
        strSQL = strSQL & " ORDER BY fname,lname"

        'Search by phone extension
    ElseIf (Len(Request.QueryString("e"))) Then
        strSQL = "SELECT fname as first, lname as last, phone, deptname as department"
        strSQL = strSQL & " from ph_employees as a, ph_dept as b where a.deptid=b.deptid and phone like '" & Request.QueryString("e") & "%'"
        strSQL = strSQL & " ORDER BY phone, lname, fname"
        'Search by last name
    ElseIf (Len(Request.QueryString("q"))) Then
        strSQL = "SELECT  fname as first, lname as last,phone, deptname as department"
        strSQL = strSQL & " from ph_employees as a, ph_dept as b"
        strSQL = strSQL & " where a.deptid=b.deptid"
        strSQL = strSQL & " and lname like '" & PCase(Request.QueryString("q")) & "%'"
        strSQL = strSQL & " ORDER BY lname, fname"

        'Search by city using the pulldown list
    ElseIf (Len(Request.QueryString("s")) And Request.QueryString("s") <> "Select City") Then
        strSQL = "SELECT  storenum as store,phone,name2 as Manager,name1 as name,designation as type,address1,city,state,zip"
        strSQL = strSQL & " from ph_store where city='" & Request.QueryString("s") & "'"
        strSQL = strSQL & " ORDER BY store"
        'Search by store number from a pulldown list
    ElseIf (Len(Request.QueryString("n")) And Request.QueryString("n") <> "Select Store") Then
        strSQL = "SELECT  storenum as store,phone,name2 as Manager,name1 as name,designation as type,address1,city,state,zip"

        If (CDbl(Request.QueryString("n")) >= 1000) Then
            'for store numbers bigger than 1000, people don't put the # char prefix with store numbers
            strSQL = strSQL & " from ph_store where storenum = '" & Request.QueryString("n") & "'"
        Else
            strSQL = strSQL & " from ph_store where storenum = '#" & Request.QueryString("n") & "'"
        End If
    Else
        'just end if user did not select anything to search
        Response.End()
    End If

    Dim first As String = ""
    Dim last As String = ""
    Dim phone As String = ""
    Dim address As String = ""
    Dim city As String = ""
    Dim state As String = ""
    Dim zip As String = ""
    Dim store As String = ""
    Dim employees As List(Of employee) = New List(Of employee)

    Dim command As SqlCommand = New SqlCommand()
    command.Connection = connection
    command.CommandText = strSQL
    connection.Open()

    Dim employeeRS As SqlDataReader = command.ExecuteReader
    While employeeRS.Read
        For i As Integer = 0 To employeeRS.FieldCount - 1
            If employeeRS.GetName(i).Equals("first", StringComparison.InvariantCultureIgnoreCase) AndAlso Not IsDBNull(employeeRS("first")) Then
                first = employeeRS("first")
            End If
            If employeeRS.GetName(i).Equals("last", StringComparison.InvariantCultureIgnoreCase) AndAlso Not IsDBNull(employeeRS("last")) Then
                last = employeeRS("last")
            End If
            If employeeRS.GetName(i).Equals("phone", StringComparison.InvariantCultureIgnoreCase) AndAlso Not IsDBNull(employeeRS("phone")) Then
                phone = employeeRS("phone")
            End If
            If employeeRS.GetName(i).Equals("address1", StringComparison.InvariantCultureIgnoreCase) AndAlso Not IsDBNull(employeeRS("address1")) Then
                address = employeeRS("address1")
            End If
            If employeeRS.GetName(i).Equals("city", StringComparison.InvariantCultureIgnoreCase) AndAlso Not IsDBNull(employeeRS("city")) Then
                city = employeeRS("city")
            End If
            If employeeRS.GetName(i).Equals("state", StringComparison.InvariantCultureIgnoreCase) AndAlso Not IsDBNull(employeeRS("state")) Then
                state = employeeRS("state")
            End If
            If employeeRS.GetName(i).Equals("zip", StringComparison.InvariantCultureIgnoreCase) AndAlso Not IsDBNull(employeeRS("zip")) Then
                zip = employeeRS("zip")
            End If
            If employeeRS.GetName(i).Equals("store", StringComparison.InvariantCultureIgnoreCase) AndAlso Not IsDBNull(employeeRS("store")) Then
                store = employeeRS("store")
            End If
        Next
        employees.Add(New employee() With {.firstName = first, .lastName = last, .extension = phone, .address = address, .city = city, .state = state, .zip = zip, .store = store})
    End While
    connection.Close()
    
    Dim serializer As JavaScriptSerializer = New JavaScriptSerializer()
    Dim result = serializer.Serialize(employees)
    
    Response.Write(result)
%>