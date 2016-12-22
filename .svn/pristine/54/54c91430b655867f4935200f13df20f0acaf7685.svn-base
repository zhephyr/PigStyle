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
function MoveToCode(e){
    document.getElementById('txtCode').focus();
    return true;
}
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
    else
    {
        var btn = document.getElementById('btnHidden');
        btn.click();
        return true;
    }
}
function ReamPackEnter(e){
    if (e.keyCode == 13 || e.which == 13){
        validateReamPack(e);
    }
}
function validateReamPack(e){
    var ReamPack = parseInt(document.getElementById('txtReamPack').value,10);
    if (isNaN(ReamPack) || (ReamPack < 1 || ReamPack > 2500))
    {
        document.getElementById('txtReamPack').value= "";
        window.alert('Enter an acceptable range (1 - 2500) for Ream Pack');
        document.getElementById('txtReamPack').focus();
        return false;
    }
    else
    {
        document.getElementById('txtReams').focus();
        return true;
    }
}
function ReamsEnter(e){
    if (e.keyCode == 13 || e.which == 13){
        validateReams(e);
    }
}
function validateReams(e){
    var Reams = parseInt(document.getElementById('txtReams').value,10);
    if (isNaN(Reams) || (Reams < 1 || Reams > 999)) 
    {
        document.getElementById('txtReams').value= "";
        window.alert('Enter an acceptable range (1 - 999) for Reams used');
        document.getElementById('txtReams').focus();
        return false;
    }
    else
    {
        document.getElementById('txtComments').focus();
        return true;
    }
}
function CommentsEnter(e){
    if (e.keyCode == 13 || e.which == 13){
        var btn = document.getElementById('btnHidden2');
        btn.click();
    
    }
}
function validateComments(e){
    var Reams = parseInt(document.getElementById('txtReams').value,10);
    var Description = document.getElementById('lstDescription').value;
    var ReamPack = parseInt(document.getElementById('txtReamPack').value,10);
    if (isNaN(Reams) || (Reams < 1 || Reams > 999)) 
    {
        document.getElementById('txtReams').value= "";
        window.alert('Enter an acceptable range (1 - 999) for Reams used');
        document.getElementById('txtReams').focus();
        return false;
    }
    else if (/$^/.test(Description))
    {
        document.getElementById('lstDescription').value= "";
        window.alert('Please Select A Description');
        document.getElementById('lstDescription').focus();
        return false;
    }
    else if (isNaN(ReamPack) || (ReamPack < 1 || ReamPack > 2500)) 
    {
        document.getElementById('txtReamPack').value= "";
        window.alert('Enter an acceptable range (1 - 2500) for Ream Packs');
        document.getElementById('txtReamPack').focus();
        return false;
    }
    else
    {
        var btn = document.getElementById('btnLoadGrid');
        btn.click();
        return true;
    }
}