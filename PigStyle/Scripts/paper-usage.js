function CodeEnter(e){
    if (e.keyCode == 13 || e.which == 13){
        validateCode(e);
    }
}
function validateCode(e){
    var Code = parseInt(document.getElementById('txtCode').value,10);
    if (isNaN(Code) || (Code < 10000 || Code > 99999)) 
    {
        document.getElementById('txtCode').value= "";
        window.alert('Enter an acceptable range (10000 - 99999) for code number');
        document.getElementById('txtCode').focus();
        return false;
    }
    else if (Code = "")
    {
        document.getElementById('txtCode').value= "";
        window.alert('Enter an acceptable range (10000 - 99999) for code number');
        document.getElementById('txtCode').focus();
        return false;
    }
    else
    {
        var btn = document.getElementById('btnHidden');
        btn.click();
        return true;
    }
}
function CaseEnter(e){
    if (e.keyCode == 13 || e.which == 13){
        var btn = document.getElementById('btnHidden');
        btn.click();
        return true;
    }
}
// This function validates all the text boxes and list boxes when the add button is pressed.
function validateCase(e){
    var Printer = document.getElementById('lstPrinter').value;
    var Case = document.getElementById('txtCase').value;
    var Code = document.getElementById('txtCode').value;
    if (/$^/.test(Printer))
    {
        document.getElementById('lstPrinter').value= "";
        window.alert('Please Choose A Printer');
        document.getElementById('lstPrinter').focus();
        return false;
    }
    else if (isNaN(Code) || (Code < 10000 || Code > 99999)) 
    {
        document.getElementById('txtCode').value= "";
        window.alert('Please Enter A Code Number');
        document.getElementById('txtCode').focus();
        return false;
    }

    else if (isNaN(Case) || (Case < 1 || Case > 999)) 
    {
        document.getElementById('txtCase').value= "";
        window.alert('Please Enter A Case Amount');
        document.getElementById('txtCase').focus();
        return false;
    }
    else
    {
        var btn = document.getElementById('btnLoadGrid');
        btn.click();
    }
}   