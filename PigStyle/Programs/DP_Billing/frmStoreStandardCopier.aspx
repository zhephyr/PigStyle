﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="frmStoreStandardCopier.aspx.vb" Inherits="PigStyle.frmStoreStandardCopier" %>

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
    <title>Copier Charges</title>
    <link href="../../CSS/base.css" rel="stylesheet" />
    <script src="../../Scripts/copier-standard.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="container">
            <div id="header">
                <img src="../../images/webpig.jpg" />
                <div class="text" style="text-align: center">
                    <h1>Store Standard Copier Charges</h1>
                </div>
            </div>
            <div id="breadcrumbs">
                <a href="../../Startup.aspx">Home</a>
                <a href="../Programs.aspx">Programs</a>
                <a href="../BSSGapp.aspx">BSSG Apps</a>
                <a href="frmDataProcessing.aspx">DP Billing</a>
                <a href="frmStoreStandardCopier.aspx">Copier Charges</a>
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
                <div id="content" style="text-align: center;">
                    <div class="onerow">
                        <div class="col1">
                            <br />
                        </div>
                        <div class="col1">
                            Store Number
                            <br />
                            <br />
                            <asp:TextBox ID="txtStoreNum" runat="server" Width="100%" TabIndex="1" onkeydown="StoreEnter(event)"></asp:TextBox>
                        </div>
                        <div class="col1">
                            Code
                            <br />
                            <br />
                            <br />
                            <asp:TextBox ID="txtCode" runat="server" Width="100%" TabIndex="2" onkeydown="CodeEnter(event)"></asp:TextBox>
                        </div>
                        <div class="col1">
                            Ream(s)
                            <br />
                            <br />
                            <br />
                            <asp:TextBox ID="txtReams" runat="server" Width="100%" TabIndex="3" onkeydown="ReamsEnter(event)"></asp:TextBox>
                        </div>
                        <div class="col2">
                            Simplex/ Duplex
                            <br />
                            <br />
                            <br />
                            <asp:DropDownList ID="lstSimDup" runat="server" Width="100%" TabIndex="4" onchange="MoveToJob(event)">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem>250 Simplex</asp:ListItem>
                                <asp:ListItem>500 Simplex</asp:ListItem>
                                <asp:ListItem>250 Duplex</asp:ListItem>
                                <asp:ListItem>500 Duplex</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col2">
                            Job Type
                            <br />
                            <br />
                            <br />
                            <asp:DropDownList ID="lstJobType" runat="server" Width="100%" TabIndex="5" onchange="MoveToHours(event)">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem>Coupons</asp:ListItem>
                                <asp:ListItem>Flyers</asp:ListItem>
                                <asp:ListItem>Other</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col1">
                            Hours .1 or >
                            <br />
                            <br />
                            <asp:TextBox ID="txtHours" runat="server" Width="100%" TabIndex="6" onkeydown="HoursEnter(event)"></asp:TextBox>
                        </div>
                        <div class="col2">
                            Comments
                            <br />
                            <br />
                            <br />
                            <asp:TextBox ID="txtComments" runat="server" Width="100%" TabIndex="7" onkeydown="CommentsEnter(event)"></asp:TextBox>
                        </div>
                        <div class="col1 last">
                            <br />
                        </div>
                    </div>
                    <div class="onerow" style="text-align:center;">
                        <br />
                        <br />
                        <div class="col1">
                            <br />
                        </div>
                        <div class="col2">
                            <asp:RadioButton ID="rdSingle" runat="server" Text="SINGLE" GroupName="SideGroup" Checked="true" OnClick="SingleCheck(event)" /><br />
                            <asp:RadioButton ID="rdMultiple" runat="server" Text="MULTIPLE" GroupName="SideGroup" OnClick="MultipleCheck(event)" />
                        </div>
                        <div class="col2">
                            <asp:Button ID="btnAdd" runat="server" Text="ADD" Height="30px" Width="100px" Font-Bold="true" UseSubmitBehavior="False" OnClick="btnAdd_Click" />
                        </div>
                        <div class="col2">
                            <asp:Button ID="btnProcess" runat="server" Text="PROCESS" Height="30px" Width="100px" Font-Bold="true" UseSubmitBehavior="False" OnClick="btnProcess_Click" />
                        </div>
                        <div class="col2">
                            <asp:Button ID="btnExit" runat="server" Text="EXIT" Height="30px" Width="100px" Font-Bold="true" UseSubmitBehavior="False" OnClick="btnExit_Click" />
                        </div>
                        <div class="col2">
                            <asp:Button ID="btnDelete" runat="server" Text="DELETE" Height="30px" Width="100px" Font-Bold="true" UseSubmitBehavior="False" OnClick="btnDelete_Click" />
                        </div>
                        <div class="col1 last">
                            <br />
                        </div>
                    </div>
                    <div class="onerow">
                        <br />
                        <br />
                        <br />
                        <div class="col2">
                            <br />
                        </div>
                        <div class="col8">
                            <asp:GridView ID="MSGridView" runat="server" align="Center" AutoGenerateSelectButton="false" CssClass="grid">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="selectChk" runat="server" AutoPostBack="true" OnCheckedChanged="selectChk_CheckedChanged" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div class="col2 last">
                            <br />
                        </div>
                    </div>
                    <asp:Button ID="btnHidden" Style="display: none" runat="server" UseSubmitBehavior="false" OnClick="btnHidden_Click" />
                    <asp:Button ID="btnHidden2" Style="display: none" runat="server" UseSubmitBehavior="false" OnClick="btnHidden2_Click" />
                    <asp:Button ID="btnLoadGrid" Style="display: none" runat="server" UseSubmitBehavior="False" OnClick="btnLoadGrid_Click" />
                </div>
            </div>
            <div id="footer">
                <%= DateTime.Now.ToLongDateString()%>
            </div>
        </div>
    </form>
</body>
</html>
