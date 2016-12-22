<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="plu.aspx.vb" Inherits="PigStyle.plu" %>

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
    <title>PLU Search</title>
    <link href="../../CSS/bootstrap.css" rel="stylesheet" />
    <link href="../../CSS/base.css" rel="stylesheet" />
    <link href="../../CSS/angular-loading.css" rel="stylesheet" />
    <script src="../../Scripts/angular.min.js"></script>
    <script src="../../Scripts/angular-animate.min.js"></script>
    <script src="../../Scripts/angular-ui/ui-bootstrap-tpls.min.js"></script>
    <script src="../../Scripts/plum.js"></script>
    <script src="../../Scripts/spin.js"></script>
    <script src="../../Scripts/angular-loading.js"></script>
    <style type="text/css">
        #content td {
            padding: 5px;
        }

        .modal-body {
            overflow: auto;
        }

        #nutritionfacts {
            border: 1px solid black;
            padding: 3px;
            font-family: 'Arial Black',sans-serif;
        }

            #nutritionfacts .label {
                font-size: 100%;
                padding: 0;
            }

            #nutritionfacts td.indent {
                font-size: 85%;
                text-indent: 25px;
            }

            #nutritionfacts td {
                color: black;
                font-family: Arial,sans-serif;
                font-size: 12pt;
                padding: 0;
            }

                #nutritionfacts td.header {
                    font-family: 'Arial Black',sans-serif;
                    font-size: 36px;
                    white-space: nowrap;
                }

            #nutritionfacts div.label {
                color: #000000;
                float: left;
                font-family: 'Arial Black',sans-serif;
            }

            #nutritionfacts div.weight {
                display: inline;
                font-family: Arial,Helvetica,sans-serif;
                padding-left: 1px;
            }

            #nutritionfacts div.dv {
                display: inline;
                float: right;
                font-family: 'Arial Black',sans-serif;
            }

            #nutritionfacts table.vitamins td {
                font-family: Arial,sans-serif;
                white-space: nowrap;
                width: 20%;
            }

            #nutritionfacts div.line {
                font-family: Arial;
                border-top: 1px solid black;
            }

            #nutritionfacts div.labellight {
                float: left;
                font-family: Arial,sans-serif;
            }

            #nutritionfacts .highlighted {
                border: 1px dotted grey;
                padding: 2px;
            }

        @media screen {
            #printSection {
                display: none;
            }
        }

        @media print {
            html * {
                visibility: hidden;
            }

            .label {
                border: none;
            }

            body > :not(#printSection) {
                display: none;
            }

            #printSection, #printSection * {
                visibility: visible;
            }

            #printSection {
                position: absolute;
                left: 0;
                top: 0;
            }
        }
    </style>
</head>
<body ng-app="plumApp">
    <form id="form1" runat="server" ng-controller="pluModalCtrl">
        <div id="container" dw-loading="plumLoading">
            <div id="header">
                <img src="/images/webpig.jpg" />
                <div class="text" style="text-align: center">
                    <h1>PLU Ingredients & Nutrition</h1>
                </div>
            </div>
            <div id="breadcrumbs">
                <a href="http://pignet/beginpage.asp">Home</a>
                <a href="http://pignet/Programs/Programs.asp">Programs</a>
                <a href="plu.aspx">PLU Search</a>
            </div>
            <div id="main">
                <div id="nav">
                    <%If permissionType = "Store" Then%>
                    <a href="../../Startup.aspx" class="button">Home</a>
                    <a href="http://vg-fortis2012/FortisWeb" class="button">Fortis</a>
                    <a href="../Programs(store)" class="button">Programs</a>
                    <a href="#" class="button">General Info</a>
                    <a href="#" class="button">Employee Info</a>
                    <a href="#" class="button">Web Links</a>
                    <a href="#" class="button">Store Signage</a>
                    <a href="http://www.shopthepig.com" class="button">Piggly Wiggly</a>
                    <%ElseIf permissionType = "Corporate" Then%>
                    <a href="../../Startup.aspx" class="button">Home</a>
                    <a href="http://vg-fortis2012/FortisWeb" class="button">Fortis</a>
                    <a href="../Programs.aspx" class="button">Programs</a>
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
                    <a href="../../Startup.aspx" class="button">Home</a>
                    <a href="#" class="button">General Info</a>
                    <a href="http://www.shopthepig.com" class="button">Piggly Wiggly</a>
                    <%End If%>
                </div>
                <div id="content">
                    <div class="onerow">
                        <div class="col12" style="text-align: center; padding-top: 25px; padding-bottom: 25px;">
                            There are <%=totalRecs%> PLUs in the database.
                        </div>
                    </div>
                    <div class="onerow" id="pluPicker">
                        <script type="text/ng-template" ng-include="'PLU-Data.html'"></script>
                        <script type="text/ng-template" id="pluChoices.html">
                            <div class="modal-header">
                                <h3 class="modal-title">Choose a PLU</h3>
                            </div>
                            <div class="modal-body">
                                There are multiple PLUs with that PLU number. Please choose one PLU to view.
                                <br />
                                <br />
                                <ul>
                                    <li ng-repeat="plu in productData">
                                        <button type="button" class="btn btn-primary" ng-click="select(plu.PLU, plu.Dept)">({{plu.Dept}}) {{plu.PLU}} -- {{plu.Desc1}} {{plu.Desc2}}</button>
                                        <br />
                                        <br />
                                    </li>
                                </ul>
                            </div>
                            <div class="modal-footer">
                                <button class="btn btn-danger" type="button" ng-click="dismiss()">Cancel</button>
                            </div>
                        </script>
                        <div class="col3">
                            <br />
                        </div>
                        <div class="col6">
                            <table>
                                <tr>
                                    <td style="vertical-align: bottom; text-align: right;">PLU to Search:
                                    </td>
                                    <td>
                                        <input type="text" class="form-control" ng-model="pluTxt" ng-keypress="checkSubmit($event)" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align: bottom; text-align: right;">PLU Description:
                                    </td>
                                    <td>
                                        <input type="text" class="form-control" ng-model="descTxt" ng-keypress="checkDesc($event)" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align: bottom; text-align: right;">Select Department:
                                    </td>
                                    <td>
                                        <select class="form-control" id="deptSelect" ng-model="deptChoice" ng-change="loadPLUs()">
                                            <option value="">Select Dept</option>
                                            <option ng-repeat="dept in depts" value="{{dept}}">{{dept}}</option>
                                        </select>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="checkbox" style="text-align: right;">Sort By PLU
                                        <input type="checkbox" id="orderPLU" style="position: relative; margin: 0;" ng-model="pluSort" ng-change="checkLoaded()" />
                                    </td>
                                </tr>
                                <tr ng-show="pluVisible">
                                    <td style="vertical-align: bottom; text-align: right;">PLU - Description:
                                    </td>
                                    <td>
                                        <select class="form-control" id="pluSelect" ng-model="pluChoice" ng-change="submitPLU()">
                                            <option value="">Select PLU</option>
                                            <option ng-repeat="option in pluList" value="{{option.PLU}}">{{option.PLU}} -- {{option.desc1}} {{option.desc2}}</option>
                                        </select>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td style="text-align: right;">
                                        <input type="button" class="btn btn-primary" ng-click="reset()" value="Reset" />
                                    </td>
                                </tr>
                                <tr>
                                </tr>
                            </table>
                        </div>
                        <div class="col3">
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
