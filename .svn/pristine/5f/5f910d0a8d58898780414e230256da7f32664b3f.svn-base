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
function CopiesEnter(e){
    if (e.keyCode == 13 || e.which == 13)
    {
        validateCopies(e);
    }
}
function validateCopies(e){
    var Copies = parseInt(document.getElementById('txtCopies').value,10);
    if (isNaN(Copies) || (Copies < 0 || Copies > 9999)) 
    {
        document.getElementById('txtCopies').value= "";
        window.alert('Enter an acceptable range (1 - 9999) for copies number');
        document.getElementById('txtCopies').focus();

        return false;
    }
    else
    {
        document.getElementById('lstSimDup').focus();
        return true;
    }
}
function MoveToJob(e){
    document.getElementById('lstJobType').focus();
    return true;
}
function MoveToComments(e){
    document.getElementById('txtComments').focus();
    return true;
}
function CommentsEnter(e){
    if (e.keyCode == 13 || e.which == 13){
        var btn = document.getElementById('btnHidden2');
        btn.click();  
    }
}
function validateComments(e){
    var SimDup = document.getElementById('lstSimDup').value;
    var JobType = document.getElementById('lstJobType').value;
    var Copies = parseInt(document.getElementById('txtCopies').value,10);
    if (isNaN(Copies) || (Copies < 0 || Copies > 9999)) 
    {
        document.getElementById('txtCopies').value= "";
        window.alert('Enter an acceptable range (1 - 9999) for copies number');
        document.getElementById('txtCopies').focus();
        return false;
    }   
    else if (/$^/.test(SimDup))
    {
        document.getElementById('lstSimDup').value= "";
        window.alert('Please Select A Simplex/Duplex');
        document.getElementById('lstSimDup').focus();
        return false;
    }
    else if (/$^/.test(JobType))
    {
        document.getElementById('lstJobType').value= "";
        window.alert('Please Select A Simplex/Duplex');
        document.getElementById('lstJobType').focus();
        return false;
    }
    
    else
    {
        var btn = document.getElementById('btnLoadGrid');
        btn.click();
        return true;
    }
}
function ColorRequestorEnter(e){
    if (e.keyCode == 13 || e.which == 13){
        validateColorRequestor(e);
    }
}
function validateColorRequestor(e){
    var colorrequestor = document.getElementById('txtRequestor').value;
    if (/[^a-zA-Z\s]/.test(colorrequestor))
    {
        document.getElementById('txtRequestor').value= "";
        window.alert('Please Enter A Requestor');
        document.getElementById('txtRequestor').focus();
        return false;
    }
    else if (/$^/.test(colorrequestor))
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
function MoveToColorCopies(e){
    document.getElementById('txtCopies').focus();
    return true;
}
function ColorCopiesEnter(e){
    if (e.keyCode == 13 || e.which == 13){
        validateColorCopies(e);
    }
}
function validateColorCopies(e){
    var colorCopies = parseInt(document.getElementById('txtCopies').value,10);
    if (isNaN(colorCopies) || (colorCopies < 1 || colorCopies > 9999)) 
    {
        document.getElementById('txtCopies').value= "";
        window.alert('Enter an acceptable range (1 - 9999) for copies');
        document.getElementById('txtCopies').focus();
        return false;
    }
    else
    {
        document.getElementById('lstSimDup').focus();
        return true;
    }
}
function MoveToColorComments(e){
    document.getElementById('txtComments').focus();
    return true;
}
function ColorCommentsEnter(e){
    if (e.keyCode == 13 || e.which == 13){
        validateColorComments(e)
        return true
    }
}
// This function validates all the text boxes and list boxes when the add button is pressed.
function validateColorComments(e){
    var colorrequestor = document.getElementById('txtRequestor').value;
    var colorDepartment = document.getElementById('lstDepartment').value;
    var colorSubDepartment = document.getElementById('lstSubDepartment').value;
    var colorCopies = document.getElementById('txtCopies').value;
    var colorSimDup = document.getElementById('lstSimDup').value;
 
    if (/$^/.test(colorrequestor))
    {
        document.getElementById('txtRequestor').value= "";
        window.alert('Please Enter A Requestor');
        document.getElementById('txtRequestor').focus();
        return false;
    }
    else if (/[^a-zA-Z\s]/.test(colorrequestor))
    {
        document.getElementById('txtRequestor').value= "";
        window.alert('Please Enter A Requestor');
        document.getElementById('txtRequestor').focus();
        return false;
    }
    else if (/$^/.test(colorDepartment))
    {
        document.getElementById('lstDepartment').value= "";
        window.alert('Please Choose A Department');
        document.getElementById('lstDepartment').focus();
        return false;
    }
    else if (/$^/.test(colorSubDepartment))
    {
        document.getElementById('lstSubDepartment').value= "";
        window.alert('Please Choose A Sub Deparment');
        document.getElementById('lstSubDepartment').focus();
        return false;
    }
    else if (isNaN(colorCopies) || (colorCopies < 1 || colorCopies > 9999)) 
    {
        document.getElementById('txtCopies').value= "";
        window.alert('Please Enter A Copy Amount');
        document.getElementById('txtCopies').focus();
        return false;
    }
    else if (/$^/.test(colorSimDup))
    {
        document.getElementById('lstSimDup').value= "";
        window.alert('Please Select A Simplex/Duplex');
        document.getElementById('lstSimDup').focus();
        return false;
    }
    else
    {
        var colorbtn = document.getElementById('btnLoadGrid');
        colorbtn.click();
    }
}   