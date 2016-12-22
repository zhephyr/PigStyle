<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="test.aspx.vb" Inherits="PigStyle.test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Example</title>
    <link href="CSS/base.css" rel="stylesheet" type="text/css" />
    <style>
        label > input { /* HIDE RADIO */
            display: none;
        }

            label > input + img { /* IMAGE STYLES */
                cursor: pointer;
                border: 2px solid transparent;
            }

            label > input:checked + img { /* (CHECKED) IMAGE STYLES */
                border: 2px solid #f00;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="container">
            <div id="header">
                <img src="images/BusinessCardPig2013.png" />

                <div class="text">
                    <h2>BUSINESS CARD REQUEST FORM</h2>
                    <h3>For PWN Union Ave. Employees</h3>
                    <p>Please fill out the form below to request more business cards.</p>
                    <p>If you have ordered new business cards since May 2012, please note this in the Comments section</p>
                    <p>if you would like copies made of the business card in your file.</p>
                    <h6>
                        <p><b><ins>Note:</ins></b> The Business cards you recieve will be similar to the business car template you choose below</p>
                        <p><strong><em>Requests Will Not Be Taken To Alter These.</em></strong></p>
                    </h6>
                </div>
            </div>
            <div id="breadcrumbs">
                <a href="#">Home</a>
                <a href="#">Programs</a>
                <a href="#">Employee Setup</a>
                <a href="Example.aspx">Business Cards</a>
            </div>
            <div id="main">
                <div id="nav">
                    <a href="test.aspx" class="button">Layout</a>
                    <a href="#" class="button">Home</a>
                    <a href="#" class="button">Fortis</a>
                    <a href="#" class="button">Programs</a>
                    <a href="#" class="button">Meeting Rooms</a>
                    <a href="#" class="button">Employee Info</a>
                    <a href="#" class="button">General Info</a>
                    <a href="#" class="button">Projects Info</a>
                    <a href="#" class="button">BSSG Website</a>
                    <a href="#" class="button">Web Links</a>
                    <a href="#" class="button">Store Signage</a>
                    <a href="#" class="button">Help Desk</a>
                </div>
                <div id="content">
                    <div class="onerow" style="margin-bottom: 25px">
                        <div class="col4">Information:</div>
                        <div class="col8 last">Please click the style of business card you want:</div>
                    </div>
                    <div class="onerow">
                        <div class="col4">
                            First Name: 
                            <br />
                            <input type="text" style="width: 100%" />
                            <br />
                            Last Name:  
                            <br />
                            <input type="text" style="width: 100%" />
                            <br />
                            Job Title:  
                            <br />
                            <input type="text" style="width: 100%" />
                            <br />
                            Phone:     
                            <br />
                            <input type="text" style="width: 100%" />
                            <br />
                            Work Email: 
                            <br />
                            <input type="text" style="width: 100%" />
                            <br />
                        </div>
                        <div class="col4">
                            <label>
                                <input type="radio" name="cardType"/>
                                <img src="images/template1new-01.jpg" />
                            </label>
                            <label>
                                <input type="radio" name="cardType"/>
                                <img src="images/template3new-01.jpg" />
                            </label>
                            <label>
                                <input type="radio" name="cardType"/>
                                <img src="images/template5new-01.jpg" />
                            </label>
                        </div>
                        <div class="col4 last">
                            <label>
                                <input type="radio" name="cardType"/>
                                <img src="images/template2new-01.jpg" />
                            </label>
                            <label>
                                <input type="radio" name="cardType"/>
                                <img src="images/template4new-01.jpg" />
                            </label>
                        </div>
                    </div>
                    <div class="onerow">
                        <div class="col12" style="text-align: center; margin-top: 20px;">
                            <input type="submit" value="Submit" />
                        </div>
                    </div>
                </div>
            </div>
            <div id="footer">
                Contact Ashley Knapp at: <a href="#">aknapp@shopthepig.com</a> with any questions.
            </div>
        </div>
    </form>
</body>
</html>
