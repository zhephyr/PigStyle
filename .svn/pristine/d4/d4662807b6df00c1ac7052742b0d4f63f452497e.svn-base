<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ModifyServerList_Citrix.aspx.vb" Inherits="PigStyle.ModifyServerList_Citrix" %>

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
    <title>Modify Server List</title>
    <link href="../../CSS/base.css" rel="stylesheet" />
    <script src="../../Scripts/jquery-2.1.4.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="container">
            <div id="header">
                <img src="../../images/webpig.jpg" />
                <div class="text" style="text-align: center">
                    <h1>Modify Server List - Citrix</h1>
                </div>
            </div>
            <div id="breadcrumbs">
                <a href="../../Startup.aspx">Home</a>
                <a href="../Programs.aspx">Programs</a>
                <a href="../BSSGapp.aspx">BSSG Apps</a>
                <a href="../ProfileFinderSelect.aspx">Profile Finder</a>
                <a href="Profile_Finder_Citrix.aspx">PS4 Profile Finder</a>
                <a href="ModifyServerList_Citrix.aspx">Modify Server List</a>
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
                        <div class="col5" style="text-align: center;">
                            <asp:Label ID="lblCountLabel" runat="server" Text="Servers Found in List:"></asp:Label>
                            <asp:Label ID="lblCount" runat="server"></asp:Label>
                            <asp:GridView ID="ServerList" runat="server" AllowSorting="true" DataKeyNames="server" Visible="true" AutoGenerateColumns="false" CssClass="grid">
                                <HeaderStyle BackColor="MediumBlue" ForeColor="LightCyan" />
                                <Columns>
                                    <asp:BoundField DataField="server" HeaderText="Server" />
                                    <asp:BoundField DataField="ip" HeaderText="Ip" />
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div class="col7 last" style="text-align: center;">
                            <h3>Server Name</h3>
                            <asp:TextBox ID="serverName" runat="server" Width="100px"></asp:TextBox>
                            <br />
                            <h3>Ip Address</h3>
                            <asp:TextBox ID="ipAddress" runat="server" Width="100px"></asp:TextBox>
                            <br />
                            <br />
                            <asp:Button ID="btnAdd" runat="server" Text="Add" Width="105px" ValidationGroup="Add" CausesValidation="true" OnClick="AddClick" /><br />
                            <asp:RequiredFieldValidator runat="server" ID="Req1" ValidationGroup="Add" ControlToValidate="serverName" ErrorMessage="Server Name is Required" ForeColor="Red"></asp:RequiredFieldValidator><br />
                            <asp:RequiredFieldValidator runat="server" ID="Req2" ValidationGroup="Add" ControlToValidate="ipAddress" ErrorMessage="IP Address is Required" ForeColor="Red"></asp:RequiredFieldValidator><br />
                            <asp:Label ID="lblAddRec" runat="server" Text="Successfully Added!" ForeColor="MediumBlue" Visible="false"></asp:Label><br />
                            <asp:Label ID="lblAddNoRec" runat="server" Text="The server cannot be added because it's already in the list." ForeColor="Red" Visible="false"></asp:Label><br />
                            <br />
                            <br />
                            <br />
                            <h3>Server Name</h3>
                            <asp:TextBox ID="serverName2" runat="server" Width="100px" ValidationGroup="Delete"></asp:TextBox>
                            <br />
                            <br />
                            <asp:Button ID="btnDelete" runat="server" Text="Delete" Width="105px" ValidationGroup="Delete" CausesValidation="true" OnClick="DelClick" /><br />
                            <asp:RequiredFieldValidator runat="server" ID="Req3" ValidationGroup="Delete" ControlToValidate="serverName2" ErrorMessage="Server Name is Required" ForeColor="Red"></asp:RequiredFieldValidator>
                            <br />
                            <asp:Label ID="lblDeleteRec" runat="server" Text="Successfully Deleted!" ForeColor="Green" Visible="false"></asp:Label><br />
                            <asp:Label ID="lblDeleteNoRec" runat="server" Text="The server cannot be deleted because it does not exist." ForeColor="Red" Visible="false"></asp:Label><br />
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
