<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="HansenMenu.aspx.vb" Inherits="PigStyle.HansenMenu" %>

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
    <title>Menu</title>
    <link href="../../../CSS/bootstrap.min.css" rel="stylesheet" />
    <link href="../../../CSS/base.css" rel="stylesheet" />
    <script src="../../../Scripts/angular.js"></script>
    <script src="../../../Scripts/angular-animate.js"></script>
    <script src="../../../Scripts/angular-ui/ui-bootstrap-tpls.js"></script>
    <script src="../../../Scripts/hansen-menu.js"></script>
</head>
<body ng-app="hansenApp">
    <form id="form1" runat="server">
        <div id="container">
            <div id="header">
                <img src="../../../images/webpig.jpg" />
                <div class="text" style="text-align: center">
                    <h1>Hansen Storage Menu</h1>
                </div>
            </div>
            <div id="breadcrumbs">
                <a href="../../../Startup.aspx">Home</a>
                <a href="../../Programs.aspx">Programs</a>
                <a href="../../CorpAccProg.aspx">Accounting</a>
                <a href="HansenMenu.aspx">Hansen Menu</a>
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
                    <div class="onerow" ng-controller="DatepickerCtrl">
                        <div class="col2">
                            <br />
                        </div>
                        <div class="col4">
                            <br />
                            <p class="input-group">
                                <input type="text" id="startDate" class="form-control" uib-datepicker-popup="MM/dd/yyyy" ng-model="startDate" is-open="status.startOpened" close-text="Close" placeholder="Start Date:" runat="server" />
                                <span class="input-group-btn">
                                    <button type="button" class="btn btn-default" ng-click="openStart($event)"><i class="glyphicon glyphicon-calendar"></i></button>
                                </span>
                            </p>
                        </div>
                        <div class="col4">
                            <br />
                            <p class="input-group">
                                <input type="text" id="endDate" class="form-control" uib-datepicker-popup="MM/dd/yyyy" ng-model="endDate" is-open="status.endOpened" close-text="Close" placeholder="End Date:" runat="server" />
                                <span class="input-group-btn">
                                    <button type="button" class="btn btn-default" ng-click="openEnd($event)"><i class="glyphicon glyphicon-calendar"></i></button>
                                </span>
                            </p>
                            <br />
                            <div style="text-align: right;">
                                <asp:Button ID="Button1" OnClick="GetResults" Text="Get Results" runat="server" />
                            </div>
                        </div>
                        <div class="col2 last">
                            <br />
                        </div>
                        <asp:HiddenField ClientIDMode="Static" ID="aggregateData" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="yearlyData" runat="server" />
                    </div>
                    <div class="onerow">
                        <br />
                        <br />
                        <div class="col12" ng-controller="ReportCtrl">
                            <uib-tabset>
                                <uib-tab heading="Aggregate">
                                    <table st-table="aggregateCollection" st-safe-src="aggregateData" class="table">
                                        <thead>
                                            <tr>
                                                <th>Deptp</th>
                                                <th>Val_desc</th>
                                                <th>Std_per_lb_rate</th>
                                                <th>TareF</th>
                                                <th>QuantityP</th>
                                                <th>WeightP</th>
                                                <th>CostP</th>
                                                <th>Tare_CostP</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr ng-repeat="row in aggregateCollection">
                                                <td>{{row.Deptp}}</td>
                                                <td>{{row.Val_desc}}</td>
                                                <td>{{row.Std_per_lb_rate}}</td>
                                                <td>{{row.TareF}}</td>
                                                <td>{{row.Quantityp | number:0}}</td>
                                                <td>{{row.Weightp | number:2}}</td>
                                                <td>{{row.Costp | number:2}}</td>
                                                <td>{{row.Tare_Costp | number:2}}</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </uib-tab>
                                <uib-tab heading="Yearly">
                                    <table st-table="yearlyCollection" st-safe-src="yearlyData" class="table">
                                        <thead>
                                            <tr>
                                                <th>Year</th>
                                                <th>Deptp</th>
                                                <th>Val_desc</th>
                                                <th>Std_per_lb_rate</th>
                                                <th>TareF</th>
                                                <th>QuantityP</th>
                                                <th>WeightP</th>
                                                <th>CostP</th>
                                                <th>Tare_CostP</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr ng-repeat="row in yearlyCollection">
                                                <td>{{row.DateField}}</td>
                                                <td>{{row.Deptp}}</td>
                                                <td>{{row.Val_desc}}</td>
                                                <td>{{row.Std_per_lb_rate}}</td>
                                                <td>{{row.TareF}}</td>
                                                <td>{{row.Quantityp | number:0}}</td>
                                                <td>{{row.Weightp | number:2}}</td>
                                                <td>{{row.Costp | number:2}}</td>
                                                <td>{{row.Tare_Costp | number:2}}</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </uib-tab>
                                <uib-tab heading="Monthly">

                                </uib-tab>
                                <uib-tab heading="Weekly">

                                </uib-tab>
                            </uib-tabset>
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
