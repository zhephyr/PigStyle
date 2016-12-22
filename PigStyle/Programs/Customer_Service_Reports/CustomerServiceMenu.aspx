﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CustomerServiceMenu.aspx.vb" Inherits="PigStyle.CustomerServiceMenu" %>

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
    <title>Customer Service</title>
    <link href="/CSS/bootstrap.min.css" rel="stylesheet" />
    <link href="/CSS/base.css" rel="stylesheet" />
    <link href="/CSS/angular-loading.css" rel="stylesheet" />
    <script src="/Scripts/angular.min.js"></script>
    <script src="/Scripts/angular-animate.min.js"></script>
    <script src="/Scripts/angular-ui/ui-bootstrap-tpls.min.js"></script>
    <script src="/Scripts/customer-service-report.js"></script>
    <script src="/Scripts/spin.js"></script>
    <script src="/Scripts/angular-loading.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="container">
            <div id="header">
                <img src="/images/webpig.jpg" />
                <div class="text" style="text-align: center">
                    <h1>Customer Service Reports</h1>
                </div>
            </div>
            <div id="breadcrumbs">
                <a href="/index.aspx">Home</a>
                <a href="/Programs/Programs.aspx">Programs</a>
                <a href="/Programs/Customer_Service_Reports/CustomerServiceMenu.aspx">Reports Menu</a>
            </div>
            <div id="main">
                <div id="nav">
                    <%If permissionType = "Store" Then%>
                    <a href="/Startup.aspx" class="button">Home</a>
                    <a href="http://vg-fortis2012/FortisWeb" class="button">Fortis</a>
                    <a href="/Programs/Programs(store)" class="button">Programs</a>
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
                        <div class="col3"><br /></div>
                        <div class="col6">
                            <a href="CustomerServReport.aspx">
                                <input type="button" class="button" value="Custoner Service Report" />
                            </a>
                            <a href="CustomerServiceReportTotals.aspx">
                                <input type="button" class="button" value="Customer Service Report Totals" />
                            </a>
                            <a href="LateTransmissionReport.aspx">
                                <input type="button" class="button" value="Late Transmission Report" />
                            </a>
                            <a href="LateTransmissionSummary.aspx">
                                <input type="button" class="button" value="Late Transmission Summary" />
                            </a>
                            <a href="StoreOrderSummary.aspx">
                                <input type="button" class="button" value="Store Order Summary"
                            </a>
                            <a href="StoreOrderSummaryAll.aspx">
                                <input type="button" class="button" value="Store Order Summary - All"
                            </a>
                        </div>
                        <div class="col3"><br /></div>
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
