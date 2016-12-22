function StoreEnter(e){
    if (e.keyCode == 13 || e.which == 13){
        validateStore(e);
    }
}
function validateStore(e){
    var num = parseInt(document.getElementById('txtStoreNum').value,10);
    var length = parseInt(document.getElementById('txtStoreNum').value.length);
    if (isNaN(num))
    {
        document.getElementById('txtStoreNum').value= "";
        window.alert('Enter a Store Number!');
        document.getElementById('txtStoreNum').focus();
        return false;
    }
    else if (length > 3 || length < 3)
    {
        document.getElementById('txtStoreNum').value= "";
        window.alert('Enter a Store Number');
        document.getElementById('txtStoreNum').focus();
        return false;
    }
    else
    {
        var btn = document.getElementById('btnHidden2');
        btn.click();
        return true;
    }
}
function MoveToNumber(e){
    document.getElementById('txtNumOfPads').focus();
    return true;
}
function NumOfPadsEnter(e){
    if (e.keyCode == 13 || e.which == 13){
        validatePads(e);
    }
}
function validatePads(e){
    var Pads = parseInt(document.getElementById('txtNumOfPads').value,10);
    if (isNaN(Pads) || (Pads < 1 || Pads > 999)) 
    {
        document.getElementById('txtNumOfPads').value= "";
        window.alert('Enter an acceptable range (1 - 999) for Pads used');
        document.getElementById('txtNumOfPads').focus();
        return false;
    }
    else
    {
        document.getElementById('txtFormNum').focus();
        return true;
    }
}
function FormNumEnter(e){
    if (e.keyCode == 13 || e.which == 13){
        var btn = document.getElementById('btnHidden2');
        btn.click();
    }
}
// This function validates all the text boxes and list boxes when the add button is pressed.
function validateForm(e){
    var Size = document.getElementById('lstPadSize').value;
    var Pads = document.getElementById('txtNumOfPads').value;
    var Form = document.getElementById('txtFormNum').value;
    if (/$^/.test(Size))
    {
        document.getElementById('lstPadSize').value= "";
        window.alert('Please Choose A Pad Size');
        document.getElementById('lstPadSize').focus();
        return false;
    }
    else if (isNaN(Pads) || (Pads < 1 || Pads > 999)) 
    {
        document.getElementById('txtNumOfPads').value= "";
        window.alert('Please Enter A Pad Amount');
        document.getElementById('txtNumOfPads').focus();
        return false;
    }
    else if (/$^/.test(Form))
    {
        document.getElementById('txtFormNum').value= "";
        window.alert('Please Select A Simplex/Duplex');
        document.getElementById('txtFormNum').focus();
        return false;
    }
    else
    {
        var btn = document.getElementById('btnLoadGrid');
        btn.click();
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
function MoveToPad(e){
    document.getElementById('lstPadSize').focus();
    return true;
}
function MoveToNum(e){
    document.getElementById('txtNumOfPads').focus();
    return true;
}
function PadsEnter(e){
    if (e.keyCode == 13 || e.which == 13){
        validatePads(e);
    }
}
function FormEnter2(e){
    if (e.keyCode == 13 || e.which == 13){
        validateForm2(e)
    }
}
// This function validates all the text boxes and list boxes when the add button is pressed.
function validateForm2(e){
    var requestor = document.getElementById('txtRequestor').value;
    var Department = document.getElementById('lstDepartment').value;
    var SubDepartment = document.getElementById('lstSubDepartment').value;
    var Size = document.getElementById('lstPadSize').value;
    var Pads = document.getElementById('txtNumOfPads').value;
    var Form = document.getElementById('txtFormNumber').value;

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
        document.getElementById('lstDepartment').focus();
        return false;
    }
    else if (/$^/.test(SubDepartment))
    {
        document.getElementById('lstSubDepartment').value= "";
        window.alert('Please Choose A Sub Deparment');
        document.getElementById('lstSubDepartment').focus();
        return false;
    }
    else if (/$^/.test(Size))
    {
        document.getElementById('lstPadSize').value= "";
        window.alert('Please Choose A Pad Size');
        document.getElementById('lstPadSize').focus();
        return false;
    }
    else if (isNaN(Pads) || (Pads < 1 || Pads > 999)) 
    {
        document.getElementById('txtNumOfPads').value= "";
        window.alert('Please Enter A Pad Amount');
        document.getElementById('txtNumOfPads').focus();
        return false;
    }
    else if (/$^/.test(Form))
    {
        document.getElementById('txtFormNumber').value= "";
        window.alert('Please Select A Simplex/Duplex');
        document.getElementById('txtFormNumber').focus();
        return false;
    }
    else
    {
        var btn = document.getElementById('btnLoadGrid');
        btn.click();
    }
}
