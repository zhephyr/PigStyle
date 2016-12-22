﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="LateTransmissionSummary.aspx.vb" Inherits="PigStyle.LateTransmissionSummary" %>

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
<head runat="server">
    <title>Late Transmission Summary</title>
    <link href="/CSS/bootstrap.min.css" rel="stylesheet" />
    <link href="/CSS/base.css" rel="stylesheet" />
    <link href="/CSS/angular-loading.css" rel="stylesheet" />
    <script src="/Scripts/angular.min.js"></script>
    <script src="/Scripts/angular-animate.min.js"></script>
    <script src="/Scripts/angular-ui/ui-bootstrap-tpls.min.js"></script>
    <script src="/Scripts/late-trans-summary.js"></script>
    <script src="/Scripts/spin.js"></script>
    <script src="/Scripts/angular-loading.js"></script>
    <style type="text/css">
        .modal-body {
            max-height: 75vh;
            overflow-y: auto;
        }

            .modal-body th, td {
                padding: .5em;
                font-weight: bolder;
            }

        .uib-datepicker-popup td {
            padding: initial;
        }

            .modal-body thead tr th {
                border-top: 2px groove darkgrey;
                border-bottom: 2px groove darkgrey;
            }

            .modal-body th:nth-child(n+2), td:nth-child(n+2) {
                text-align: right;
            }

            .modal-body tbody tr:last-child td {
                border-bottom: 2px groove darkgrey;
            }

            .modal-body tfoot tr td:first-child {
                padding-left: 4em;
            }

            .modal-body table {
                padding: 0 1em 0 1em;
            }

        #totals {
            font-weight: bold;
            display: table;
            width: 100%;
            padding: 1em;
            border-bottom: 2px groove darkgrey;
        }
    </style>
</head>
<body ng-app="LateSumApp">
    <form id="form1" runat="server" ng-controller="LSCtrl">
        <div id="container" dw-loading="modalOpen">
            <div id="header">
                <img src="/images/webpig.jpg" />
                <div class="text" style="text-align: center;">
                    <h1>Late Transmission Summary</h1>
                </div>
            </div>
            <div id="breadcrumbs">
                <a href="/index.aspx">Home</a>
                <a href="/Programs/Programs.aspx">Programs</a>
                <a href="/Programs/Customer_Service_Reports/CustomerServiceMenu.aspx">Reports Menu</a>
                <a href="/Programs/Customer_Service_Reports/LateTransmissionSummary.aspx">Late Summary</a>
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
                    <div style="width: 100%; text-align: center;">
                        <h2>Late Transmission Summary</h2>
                    </div>
                    <div class="col1">
                        <br />
                    </div>
                    <div class="col8" style="text-align: center;">
                        <table style="width: 100%;">
                            <tbody>
                                <tr>
                                    <td style="text-align: right; width: 50%; vertical-align: middle;"><b>From Date:</b></td>
                                    <td class="input-group" style="text-align: left; width: 75%; padding: 5px 0 0 7px;">
                                        <input type="text" class="form-control" uib-datepicker-popup="MM/dd/yyyy" ng-model="fromDate.dt"
                                            ng-blur="verifyDates()" is-open="fromDate.opened" datepicker-options="dateOptions"
                                            close-text="Close" alt-input-formats="altInputFormats" placeholder="MM/DD/YYYY" />
                                        <span class="input-group-btn">
                                            <button type="button" class="btn btn-default" ng-click="openFrom()"><i class="glyphicon glyphicon-calendar"></i></button>
                                        </span>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right; width: 50%; vertical-align: middle;"><b>To Date:</b></td>
                                    <td class="input-group" style="text-align: left; width: 75%; padding: 5px 0 0 7px;">
                                        <input type="text" class="form-control" uib-datepicker-popup="MM/dd/yyyy" ng-model="toDate.dt"
                                            ng-blur="verifyDates()" is-open="toDate.opened" datepicker-options="dateOptions"
                                            close-text="Close" alt-input-formats="altInputFormats" placeholder="MM/DD/YYYY" />
                                        <span class="input-group-btn">
                                            <button type="button" class="btn btn-default" ng-click="openTo()"><i class="glyphicon glyphicon-calendar"></i></button>
                                        </span>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right; width: 50%; vertical-align: middle;"><b>Target:</b></td>
                                    <td class="input-group" style="text-align: left; width: 45%; padding: 5px 0 0 7px;">
                                        <input type="text" class="form-control" style="text-align: right;" ng-model="perTarget" />
                                        <span class="input-group-addon" style="font-family: 'Arial Black'; font-weight: bold;">%</span>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <div style="margin-left: 25%;">
                            <br />
                            <b>Select Area(s):</b>
                            <br />
                            <br />
                            <select class="form-control" style="width: 65%; display: initial;" multiple="multiple" size="7" ng-model="selectedAreas">
                                <option value="all" ng-focus="clearDist">All Areas</option>
                                <option disabled="disabled" class="dropdown-header" value="">--------------------</option>
                                <%For Each district In districts%>
                                <option value="<%=district.districtNum%>"><%=district.districtName%></option>
                                <%Next%>
                            </select>
                            <p style="text-align: right;">
                                <input type="button" class="btn btn-primary" style="text-align: right; margin-right: 12.5%" value="Show Report" ng-click="showReport()" />
                            </p>
                        </div>
                    </div>
                    <div class="col3 last">
                        <br />
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
