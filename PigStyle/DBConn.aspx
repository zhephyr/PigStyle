<%
Dim objConn
set objConn = Server.CreateObject("ADODB.Connection")
objConn.ConnectionString = "DSN=UnitedDB; UID=freshuser; PWD=Fresh2012;"

Dim objConnGift
set objConnGift = Server.CreateObject("ADODB.Connection")
objConnGift.ConnectionString = "DSN=Giftcard; UID=giftuser; PWD=2012Gift;"

Dim objConnTCI
set objConnTCI = Server.CreateObject("ADODB.Connection")
objConnTCI.ConnectionString = "DSN=tcihqdbprod; UID=sa; PWD=2005Sql;"

Dim objConnTCITEST
set objConnTCITEST = Server.CreateObject("ADODB.Connection")
objConnTCITEST.ConnectionString = "DSN=tcihqdbtest; UID=sa; PWD=2005Sql;"

Dim objConnHelp
set objConnHelp = Server.CreateObject("ADODB.Connection")
objConnHelp.ConnectionString = "DSN=Helpdesk; UID=administrator; PWD=Helpmore01;"

Dim objConnPlum
set objConnPlum = Server.CreateObject("ADODB.Connection")
objConnPlum.ConnectionString = "DSN=Plum; UID=plum; PWD=plum;"

Dim objConnFresh
set objConnFresh = Server.CreateObject("ADODB.Connection")
objConnFresh.ConnectionString = "DSN=freshdb; UID=informix; PWD=in4mix;"

Dim objConnDm2
set objConnDm2 = Server.CreateObject("ADODB.Connection")
objConnDm2.ConnectionString = "DSN=dm2; UID=informix; PWD=dm23;"

Dim objConnIM
set objConnIM = Server.CreateObject("ADODB.Connection")
objConnIM.ConnectionString = "DSN=InternetMarketing; UID=imuser; PWD=Im2012;"

Dim objConnLMS
set objConnLMS = Server.CreateObject("ADODB.Connection")
objConnLMS.ConnectionString = "DSN=linuxmysqldsd; UID=pigprod1; PWD=pigprod1;"

Dim objConnDBWH
set objConnDBWH = Server.CreateObject("ADODB.Connection")
objConnDBWH.ConnectionString = "DSN=dbwhprod; UID=sa; PWD=2006Sql;"

Dim objConnHG
set objConnHG = Server.CreateObject("ADODB.Connection")
objConnHG.ConnectionString = "DSN=HostGator; UID=shop; PWD=PigGator2215;"

Dim objConnHGGift
set objConnHGGift = Server.CreateObject("ADODB.Connection")
objConnHGGift.ConnectionString = "DSN=HostGator - Gift; UID=shop; PWD=PigGator2215;"

Dim objConnStore
set objConnStore = Server.CreateObject("ADODB.Connection")
objConnStore.ConnectionString = "DSN=UnitedDB - Store; UID=sa; PWD=2011Sql;"

Dim objConnSurvey
set objConnSurvey = Server.CreateObject("ADODB.Connection")
objConnSurvey.ConnectionString = "DSN=survey; UID=surveyuser; PWD=surveyuser2013;"
%>