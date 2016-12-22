﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="BSSGapp.aspx.vb" Inherits="PigStyle.BSSGapp" %>

<%
    If dirGroups.Contains("DG-DSI-PREFERED CARD USER") Then
        Response.Redirect("http://" & Request.ServerVariables("SERVER_NAME") & "/production/dci/lookupcard.asp")
    End If
    
    If dirGroups.Contains("Corporate Users") Then
        permissionType = "Corporate"
    ElseIf dirGroups.Contains("Store Users") Then
        permissionType = "Store"
    ElseIf dirGroups.Contains("Domain Users") Then
        permissionType = "Vendor"
    End If
%>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>BSSG Applications</title>
    <link href="/CSS/base.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="container">
            <div id="header">
                <img src="/images/webpig.jpg" />
                <div class="text" style="text-align: center">
                    <h1>Business Systems Support Group Applications</h1>
                </div>
            </div>
            <div id="breadcrumbs">
                <a href="/index.aspx">Home</a>
                <a href="/Programs/Programs.aspx">Programs</a>
                <a href="Programs/BSSGapp.aspx">BSSG Apps</a>
            </div>
            <div id="main">
                <div id="nav">
                    <%If permissionType = "Store" Then%>
                    <a href="/Startup.aspx" class="button">Home</a>
                    <a href="http://vg-fortis2012/FortisWeb" class="button">Fortis</a>
                    <a href="/Programs(store)" class="button">Programs</a>
                    <a href="#" class="button">General Info</a>
                    <a href="#" class="button">Employee Info</a>
                    <a href="#" class="button">Web Links</a>
                    <a href="#" class="button">Store Signage</a>
                    <a href="http://www.shopthepig.com" class="button">Piggly Wiggly</a>
                    <%ElseIf permissionType = "Corporate" Then%>
                    <a href="/Startup.aspx" class="button">Home</a>
                    <a href="http://vg-fortis2012/FortisWeb" class="button">Fortis</a>
                    <a href="/Programs/Programs.aspx" class="button">Programs</a>
                    <a href="#" class="button">Meeting Rooms</a>
                    <a href="#" class="button">Employee Info</a>
                    <a href="#" class="button">General Info</a>
                    <a href="#" class="button">Project Info</a>
                    <a href="http://vg-techsrv.sso.com/" class="button">BSSG Website</a>
                    <a href="#" class="button">Web Links</a>
                    <a href="#" class="button">Store Signage</a>
                    <a href="#" class="button">Help Desk Rpt</a>
                    <a href="http://www.shopthepig.com" class="button">Piggly Wiggly</a>
                    <%ElseIf permissionType = "Vendor" Then%>
                    <a href="/Startup.aspx" class="button">Home</a>
                    <a href="#" class="button">General Info</a>
                    <a href="http://www.shopthepig.com" class="button">Piggly Wiggly</a>
                    <%End If%>
                </div>
                <div id="content">
                    <div class="onerow">
                        <div class="col4">
                            <br />
                        </div>
                        <div class="col4">
                            <% If dirGroups.Contains("DG-BSSG-TECH-S") Or dirGroups.Contains("DG-BSSG.PROGRAMMING") Then%>
                            <a href="ProfileFinderSelect.aspx">
                                <input type="button" class="button" value="Profile Finder" />
                            </a>
                            <% End If%>
                            <% If dirGroups.Contains("DG-OPERATORS-ONLY") Or dirGroups.Contains("DG-BSSG.PROGRAMMING") Then%>
                            <a href="/Programs/DP_Billing/frmDataProcessing.aspx">
                                <input type="button" class="button" value="DP Billing" />
                            </a>
                            <% End If%>
                            <% If dirGroups.Contains("PDSACHFTP") Then%>
                            <a href="http://pignetprograms/production/PDS_ACH_FTP/My Project/PDSACHFTP.aspx">
                                <input type="button" class="button" value="PDS ACH FTP" />
                            </a>
                            <% End If%>
                            <% If dirGroups.Contains("DG-OPERATORS-ONLY") Or dirGroups.Contains("DG-BSSG.PROGRAMMING") Then%>
                            <a href="http://pignetprograms/production/ReplaceXMLDate/ReplaceXMLDate.aspx">
                                <input type="button" class="button" value="Replace XML Date" />
                            </a>
                            <% End If%>
                        </div>
                        <div class="col4 last">
                            <br />
                        </div>
                    </div>
                </div>
            </div>
            <div id="footer">
                <%= DateTime.Now.ToLongDateString()%>
            </div>
        </div>
    </form>
</body>
</html>
