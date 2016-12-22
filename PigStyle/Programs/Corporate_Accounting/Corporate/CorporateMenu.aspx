﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CorporateMenu.aspx.vb" Inherits="PigStyle.CorporateMenu" %>

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
    <title>Corporate Accounting</title>
    <link href="../../../CSS/bootstrap.min.css" rel="stylesheet" />
    <link href="../../../CSS/base.css" rel="stylesheet" />
    <script src="../../../Scripts/angular.js"></script>
    <script src="../../../Scripts/angular-animate.js"></script>
    <script src="../../../Scripts/angular-ui/ui-bootstrap-tpls.js"></script>
    <script src="../../../Scripts/accounting-accordion.js"></script>
</head>
<body ng-app="accountingApp">
    <div id="container">
        <div id="header">
            <img src="../../../images/webpig.jpg" />
            <div class="text" style="text-align: center">
                <h1>Piggly Wiggly Midwest Corporate Accounting</h1>
            </div>
        </div>
        <div id="breadcrumbs">
            <a href="../../../Startup.aspx">Home</a>
            <a href="../../Programs.aspx">Programs</a>
            <a href="../CorpAccProg.aspx">Accounting</a>
            <a href="CorporateMenu.aspx">Corporate Accounting</a>
        </div>
        <div id="main">
            <div id="nav">
                <%If permissionType = "Store" Then%>
                <a href="../../../Startup.aspx" class="button">Home</a>
                <a href="http://vg-fortis2012/FortisWeb" class="button">Fortis</a>
                <a href="../../Programs(store)" class="button">Programs</a>
                <a href="#" class="button">General Info</a>
                <a href="#" class="button">Employee Info</a>
                <a href="#" class="button">Web Links</a>
                <a href="#" class="button">Store Signage</a>
                <a href="http://www.shopthepig.com" class="button">Piggly Wiggly</a>
                <%ElseIf permissionType = "Corporate" Then%>
                <a href="../../../Startup.aspx" class="button">Home</a>
                <a href="http://vg-fortis2012/FortisWeb" class="button">Fortis</a>
                <a href="../../Programs.aspx" class="button">Programs</a>
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
                <a href="../../../Startup.aspx" class="button">Home</a>
                <a href="#" class="button">General Info</a>
                <a href="http://www.shopthepig.com" class="button">Piggly Wiggly</a>
                <%End If%>
            </div>
            <div id="content">
                <div class="onerow">
                    <div class="col3">
                        <br />
                    </div>
                    <div class="col6">
                        <br />
                        <br />
                        <uib-accordion>
                            <uib-accordion-group is-open="status.open1">
                                <uib-accordion-heading>
                                    AP Wholesale <i class="pull-right glyphicon" ng-class="{'glyphicon-chevron-down': status.open1, 'glyphicon-chevron-right': !status.open1}"></i>
                                </uib-accordion-heading>
                                <ul>
                                    <li><a href="http://vg-pignetdev/production/Corporate_Accounting/InvoiceCheckRun.asp">Check Run Lookup</a></li>
                                </ul>
                            </uib-accordion-group>
                            <uib-accordion-group is-open="status.open2">
                                <uib-accordion-heading>
                                    AR Wholesale <i class="pull-right glyphicon" ng-class="{'glyphicon-chevron-down': status.open2, 'glyphicon-chevron-right': !status.open2}"></i>
                                </uib-accordion-heading>
                                <ul>
                                    <li><a href="http://vg-pignetdev/production/Corporate_Accounting/Omicredits.asp">OMI to Endura Credits</a></li>
                                    <li><a href="http://vg-pignetdev/production/Corporate_Accounting/InvoiceHdrExtension.asp">Unposted Invoices</a></li>
                                    <li><a href="http://vg-pignetdev/production/Corporate_Accounting/InvoiceItemDetail.asp">Invoice Lookup by Item</a></li>
                                </ul>
                            </uib-accordion-group>
                            <uib-accordion-group is-open="status.open3">
                                <uib-accordion-heading>
                                    Coupons <i class="pull-right glyphicon" ng-class="{'glyphicon-chevron-down': status.open3, 'glyphicon-chevron-right': !status.open3}"></i>
                                </uib-accordion-heading>
                                <ul>
                                    <li><a href="http://vg-pignetdev/production/Corporate_Accounting/cpnredeemstore.asp">Clearinghouse - per Store by Week</a></li>
                                    <li><a href="http://vg-pignetdev/production/Corporate_Accounting/cpnredeem.asp">Clearinghouse - Summary by Store/Invoice</a></li>
                                </ul>
                            </uib-accordion-group>
                            <uib-accordion-group is-open="status.open4">
                                <uib-accordion-heading>
                                    Projects <i class="pull-right glyphicon" ng-class="{'glyphicon-chevron-down': status.open4, 'glyphicon-chevron-right': !status.open4}"></i>
                                </uib-accordion-heading>
                                <ul>
                                    <li><a href="http://vg-pignetdev/production/Corporate_Accounting/cipproject.asp">CIP Project Inquiry</a></li>
                                </ul>
                            </uib-accordion-group>
                            <uib-accordion-group is-open="status.open5">
                                <uib-accordion-heading>
                                    Users <i class="pull-right glyphicon" ng-class="{'glyphicon-chevron-down': status.open5, 'glyphicon-chevron-right': !status.open5}"></i>
                                </uib-accordion-heading>
                                <ul>
                                    <li><a href="users.aspx">Users</a></li>
                                </ul>
                            </uib-accordion-group>
                        </uib-accordion>
                    </div>
                    <div class="col3 last">
                        <br />
                    </div>
                </div>
            </div>
        </div>
    <div id="footer">
        <%= DateTime.Now.ToLongDateString()%>
    </div>
</body>
</html>
