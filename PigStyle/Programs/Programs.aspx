﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Programs.aspx.vb" Inherits="PigStyle.Programs" %>

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
    <title>Programs</title>
    <link href="/CSS/base.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="container">
            <div id="header">
                <img src="/images/webpig.jpg" />
                <div class="text" style="text-align: center">
                    <h1>Programs</h1>
                </div>
            </div>
            <div id="breadcrumbs">
                <a href="/index.aspx">Home</a>
                <a href="Programs.aspx">Programs</a>
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
                            <% If dirGroups.Contains("DG-BSSG-TECH-S") Or dirGroups.Contains("DG-BSSG.PROGRAMMING") Or dirGroups.Contains("DG-OPERATORS-ONLY") Then%>
                            <a href="BSSGapp.aspx">
                                <input type="button" class="button" value="BSSG Applications" />
                            </a>
                            <% End If%>
                            <a href="EmplSetup.aspx">
                                <input type="button" class="button" value="Employee Setup" />
                            </a>
                            <% If dirGroups.Contains("DG-BSSG.PROGRAMMING") Or dirGroups.Contains("DG-MERCH.MEAT") Or dirGroups.Contains("DISTRICT MANAGERS ONLY") Or dirGroups.Contains("MEAT SUPERVISORS") Then%>
                            <a href="MeatSurveyApps.aspx">
                                <input type="button" class="button" value="Meat Survey Application" />
                            </a>
                            <% End If%>
                            <% If dirGroups.Contains("DG-SPEC-HR-RECEPTION") Or dirGroups.Contains("DG-RETAIL.ENGINRING") Or dirGroups.Contains("DG-ACCT-PAYROLL") Or dirGroups.Contains("DG-ACCT-RETAIL") Or dirGroups.Contains("DG-BSSG.PROGRAMMING") Then%>
                            <a href="PayrollQ.aspx">
                                <input type="button" class="button" value="Payroll Queries" />
                            </a>
                            <% End If%>
                            <a href="StoreCustApps.aspx">
                                <input type="button" class="button" value="Store Custom Applications" />
                            </a>
                        </div>
                        <div class="col4">
                            <a href="Corporate_Accounting/CorpAccProg.aspx">
                                <input type="button" class="button" value="Corporate Accounting Programs" />
                            </a>
                            <% If dirGroups.Contains("DG-BSSG.PROGRAMMING") Then%>
                            <a href="GroceryWeekSurvAppDG.aspx">
                                <input type="button" class="button" value="Grocery Weekly Survey Applications" />
                            </a>
                            <% ElseIf dirGroups.Contains("DG-MERCH.BUYING") And dirGroups.Contains("SURVEYGROCUPDATE") Then%>
                            <a href="GroceryWeekSurvAppSURVEY.aspx">
                                <input type="button" class="button" value="Grocery Weekly Survey Applications" />
                            </a>
                            <% ElseIf dirGroups.Contains("SURVEYGROCUPDATE") Then%>
                            <a href="GroceryWeekSurvAppSURVEY.aspx">
                                <input type="button" class="button" value="Grocery Weekly Survey Applications" />
                            </a>
                            <% End If%>
                            <% If dirGroups.Contains("FLORALORDERREVIEW-CITRIX") Or dirGroups.Contains("DG-BSSG.PROGRAMMING") Then%>
                            <a href="http://pignetprograms/production/FloralOrderReview/frmFloralOrderReview.aspx">
                                <input type="button" class="button" value="Floral Order Review" />
                            </a>
                            <% End If%>
                            <a href="Plum/plu.aspx">
                                <input type="button" class="button" value="Plum Applications" />
                            </a>
                            <% If dirGroups.Contains("GS.BSSG DATA ENTRY") Or dirGroups.Contains("DG-BSSG.PROGRAMMING") Or dirGroups.Contains("DG-OPERATORS-ONLY") Then%>
                            <a href="EDIApps.aspx">
                                <input type="button" class="button" value="EDI" />
                            </a>
                            <% End If%>
                        </div>
                        <div class="col4 last">
                            <a href="Customer_Service_Reports/CustomerServiceMenu.aspx">
                                <input type="button" class="button" value="Customer Service Reports" />
                            </a>
                            <% If dirGroups.Contains("SURVEYSPECIALUPDATE") Then%>
                            <a href="SpecialSurveyAppSURVEY.aspx">
                                <input type="button" class="button" value="Special Survey Application" />
                            </a>
                            <% ElseIf dirGroups.Contains("DG-BSSG.PROGRAMMING") Then%>
                            <a href="/programs/SpecialSurveyAppDG.aspx">
                                <input type="button" class="button" value="Special Survey Application" />
                            </a>
                            <% End If%>
                            <% If dirGroups.Contains("DG-MERCH.BUYING") Or dirGroups.Contains("DG-BSSG.PROGRAMMING") Then%>
                            <a href="http://pignetprograms/production/LateReceiptBilling/LateReceiptBilling/MainForm.aspx">
                                <input type="button" class="button" value="Late Billing Receipt" />
                            </a>
                            <% End If%>
                            <a href="RewardsGifts.aspx">
                                <input type="button" class="button" value="Rewards/Giftcards Information" />
                            </a>
                            <% If dirGroups.Contains("GS.BSSG DATA ENTRY") Or dirGroups.Contains("DG-BSSG.PROGRAMMING") Or dirGroups.Contains("DG-OPERATORS-ONLY") Then%>
                            <a href="AdOrderHistory.aspx">
                                <input type="button" class="button" value="AD Order History" />
                            </a>
                            <% End If%>
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
