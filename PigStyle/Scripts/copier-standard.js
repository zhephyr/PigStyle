function StoreEnter(e) {
    if (e.keyCode == 13 || e.which == 13) {
        validateStore(e);
    }
}
function validateStore(e) {
    var num = parseInt(document.getElementById('txtStoreNum').value, 10);
    var length = parseInt(document.getElementById('txtStoreNum').value.length);
    if (isNaN(num)) {
        document.getElementById('txtStoreNum').value = "";
        window.alert('Enter a Store Number!');
        document.getElementById('txtStoreNum').focus();
        return false;
    }
    else if (length > 3 || length < 3) {
        document.getElementById('txtStoreNum').value = "";
        window.alert('Enter a Store Number');
        document.getElementById('txtStoreNum').focus();
        return false;
    }
    else {
        var btn = document.getElementById('btnHidden2');
        btn.click();
        return true;
    }
}
function CodeEnter(e) {
    if (e.keyCode == 13 || e.which == 13) {
        validateCode(e);
    }
}
function validateCode(e) {
    var Code = parseInt(document.getElementById('txtCode').value, 10);
    if (isNaN(Code) || (Code < 10000 || Code > 99999)) {
        document.getElementById('txtCode').value = "";
        window.alert('Enter an acceptable range (10000 - 99999) for code number');
        document.getElementById('txtCode').focus();
        return false;
    }
    else {
        var btn = document.getElementById('btnHidden');
        btn.click();
        return true;
    }
}
function ReamsEnter(e) {
    if (e.keyCode == 13 || e.which == 13) {
        validateReams(e);
    }
}
function validateReams(e) {
    var Reams = parseInt(document.getElementById('txtReams').value, 10);
    if (isNaN(Reams) || (Reams < 1 || Reams > 999)) {
        document.getElementById('txtReams').value = "";
        window.alert('Enter an acceptable range (1 - 999) for Reams used');
        document.getElementById('txtReams').focus();
        return false;
    }
    else {
        document.getElementById('lstSimDup').focus();
        return true;
    }
}
function MoveToJob(e){
    document.getElementById('lstJobType').focus();
    return true;
}
function MoveToHours(e){
    document.getElementById('txtHours').focus();
    return true;
}
function HoursEnter(e) {
    if (e.keyCode == 13 || e.which == 13) {
        validateHours(e);
    }
}
function validateHours(e) {
    var Hours = parseInt(document.getElementById('txtHours').value, 10);
    if (isNaN(Hours) || (Hours < .1 || Hours > 999)) {
        document.getElementById('txtHours').value = "";
        window.alert('Enter an acceptable range (.1 - 999) for hours');
        document.getElementById('txtHours').focus();
        return false;
    }
    else {
        document.getElementById('txtComments').focus();
        return true;
    }
}
function CommentsEnter(e) {
    if (e.keyCode == 13 || e.which == 13) {
        var btn = document.getElementById('btnHidden2');
        btn.click();

    }
}
function validateComments(e) {
    var Reams = parseInt(document.getElementById('txtReams').value, 10);
    var SimDup = document.getElementById('lstSimDup').value;
    var JobType = document.getElementById('lstJobType').value;
    var Hours = document.getElementById('txtHours').value;
    if (isNaN(Reams) || (Reams < 1 || Reams > 999)) {
        document.getElementById('txtReams').value = "";
        window.alert('Enter an acceptable range (1 - 999) for Reams used');
        document.getElementById('txtReams').focus();
        return false;
    }
    else if (/$^/.test(SimDup)) {
        document.getElementById('lstSimDup').value = "";
        window.alert('Please Select A Simplex/Duplex');
        document.getElementById('lstSimDup').focus();
        return false;
    }
    else if (/$^/.test(JobType)) {
        document.getElementById('lstSimDup').value = "";
        window.alert('Please Select A Simplex/Duplex');
        document.getElementById('lstSimDup').focus();
        return false;
    }
    else if (isNaN(Hours) || (Hours < .1 || Hours > 999)) {
        document.getElementById('txtHours').value = "";
        window.alert('Please Enter A Hour Amount');
        document.getElementById('txtHours').focus();
        return false;
    }
    else {
        var btn = document.getElementById('btnLoadGrid');
        btn.click();
        return true;
    }
}
function RequestorEnter(e){
    if (e.keyCode == 13 || e.which == 13){
        validateRequestor(e);
    }
}
function validateRequestor(e){
    var requestor = document.getElementById('txtRequestor').value;
    if (/[^a-zA-Z\s]/.test(requestor))
    {
        document.getElementById('txtRequestor').value= "";
        window.alert('Please Enter A Requestor');
        document.getElementById('txtRequestor').focus();
        return false;
    }
    else if (/$^/.test(requestor))
    {
        document.getElementById('txtRequestor').value= "";
        window.alert('Please Enter A Requestor');
        document.getElementById('txtRequestor').focus();
        return false;
    }
    else 
    {
        document.getElementById('lstDepartment').focus();
        return true;
    }  
}
function MoveToCode(e){
    document.getElementById('txtCode').focus();
    return true;
}
function CommentsEnter2(e){
    if (e.keyCode == 13 || e.which == 13){
        var btn = document.getElementById('btnHidden');
        btn.click();
        return true;
    }
}
// This function validates all the text boxes and list boxes when the add button is pressed.
function validateComments2(e){
    var requestor = document.getElementById('txtRequestor').value;
    var Department = document.getElementById('lstDepartment').value;
    var SubDepartment = document.getElementById('lstSubDepartment').value;
    var Code = document.getElementById('txtCode').value;
    var Reams = document.getElementById('txtReams').value;
    var SimDup = document.getElementById('lstSimDup').value;
    var Hours = document.getElementById('txtHours').value;

    if (/$^/.test(requestor))
    {
        document.getElementById('txtRequestor').value= "";
        window.alert('Please Enter A Requestor');
        document.getElementById('txtRequestor').focus();
        return false;
    }
    else if (/[^a-zA-Z\s]/.test(requestor))
    {
        document.getElementById('txtRequestor').value= "";
        window.alert('Please Enter A Requestor');
        document.getElementById('txtRequestor').focus();
        return false;
    }
    else if (/$^/.test(Department))
    {
        document.getElementById('lstDepartment').value= "";
        window.alert('Please Choose A Department');
        document.getElementById('lstDepartment').focus()
        return false;
    }
    else if (/$^/.test(SubDepartment))
    {
        document.getElementById('lstSubDepartment').value= "";
        window.alert('Please Choose A Sub Deparment');
        document.getElementById('lstSubDepartment').focus();
        return false;
    }
    else if (isNaN(Code) || (Code < 10000 || Code > 99999)) 
    {
        document.getElementById('txtCode').value= "";
        window.alert('Please Enter A Code Number');
        document.getElementById('txtCode').focus();
        return false;
    }

    else if (isNaN(Reams) || (Reams < 1 || Reams > 999)) 
    {
        document.getElementById('txtReams').value= "";
        window.alert('Please Enter A Ream Amount');
        document.getElementById('txtReams').focus();
        return false;
    }
    else if (/$^/.test(SimDup))
    {
        document.getElementById('lstSimDup').value= "";
        window.alert('Please Select A Simplex/Duplex');
        document.getElementById('lstSimDup').focus();
        return false;
    }
    else if (isNaN(Hours) || (Hours < .1 || Hours > 999)) 
    {
        document.getElementById('txtHours').value= "";
        window.alert('Please Enter A Hour Amount');
        document.getElementById('txtHours').focus();
        return false;
    }
    else
    {
        var btn = document.getElementById('btnLoadGrid');
        btn.click();
    }
}   