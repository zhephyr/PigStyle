<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Profile_Finder_Xen.aspx.vb" Inherits="PigStyle.Profile_Finder_Xen" %>

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
    <title>Xen Profile Finder</title>
    <link href="../../CSS/base.css" rel="stylesheet" />
    <script src="../../Scripts/jquery-2.1.4.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="container">
            <div id="header">
                <img src="../../images/webpig.jpg" />
                <div class="text" style="text-align: center">
                    <h1>Profile Finder - Xen</h1>
                </div>
            </div>
            <div id="breadcrumbs">
                <a href="../../Startup.aspx">Home</a>
                <a href="../Programs.aspx">Programs</a>
                <a href="../BSSGapp.aspx">BSSG Apps</a>
                <a href="../ProfileFinderSelect.aspx">Profile Finder</a>
                <a href="Profile_Finder_Xen.aspx">PS4 Profile Finder</a>
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
                        <div class="col6" style="text-align:center;">
                            <h3>Username to Search:</h3>
                            <asp:TextBox ID="UserID" runat="server" Width="80px"></asp:TextBox>
                            <br />
                            <br />
                            <asp:Button ID="Button1" Text="Search" runat="server" OnClick="ServerSearch" />
                            <br />
                            <br />
                            <br />
                            <asp:RequiredFieldValidator runat="server" ID="reqUserID" ControlToValidate="UserID" ErrorMessage="Enter a username to search." ForeColor="Red"></asp:RequiredFieldValidator>
                        </div>
                        <div class="col6 last" style="text-align:center;">
                            <h3>Servers Found:</h3>
                            <select id="serverList" runat="server" style="height:450px; width:99%" size="4"></select>
                            <br />
                            <br />
                            <a href="ModifyServerList_Xen.aspx">Modify Server List</a>
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