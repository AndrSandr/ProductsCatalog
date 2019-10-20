function GetAllItems() {
    $("#createBlock").show();
    $("#editBlock").hide();

    $.ajax({
        url: murl + 'api/values',
        type: 'GET',
        dataType: 'json',
        contentType: 'json',
        success: function (data) {
            WriteResponse(data);
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
}

function GetProductsListFiltered() {

    if ($('#filterInput').val() != '') {
        if (!CheckInputs($('#filterInput').val())) {
            InvalidCharsMessage('The string');
            return;
        }
    }


    $("#createBlock").show();
    $("#editBlock").hide();

    ClearInputs();

    var partName = $('#filterInput').val();
    if (partName == '') {
        GetAllItems();
    } else {
        $.ajax({
            url: murl + 'api/FilteredList/' + partName,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                WriteResponse(data);
            },
            error: function (x, y, z) {
                alert(x + '\n' + y + '\n' + z);
            }
        });
    }
}

function WriteResponse(products) { //forming HTML code for Products List
    var strResult = "<table class='productsTable'><th>ID</th><th>Code</th><th></th><th>Name</th><th>Price</th><th>Last update</th><th></th><th></th>";
    $.each(products, function (index, product) {
        var photo = '';
        if (product.Photo != '') {
            photo = product.Photo;
        }
        strResult += "<tr><td>" + product.Id + "</td><td> " + product.Code + "</td><td>" +
            "<img src='" + photo + "' /></td><td><b>" + product.Name +
            "</b></td><td>" + product.Price + "</td><td>" + product.LastUpdate + "</td><td><a href='javascript:void();' id='editItem' data-item='" + product.Id + "' onclick='EditItem(this);' >Edit</a></td>" +
            "<td><a href='javascript:void();' id='delItem' data-item='" + product.Id + "' onclick='DeleteItem(this);' >Delete</a></td></tr>";
    });
    strResult += "</table>";
    $("#tableBlock").html(strResult);
}


function AddProduct() {

    if (!CheckInputs($('#addCode').val())) {
        InvalidCharsMessage('Code');
        return;
    }
    if (!CheckInputs($('#addName').val())) {
        InvalidCharsMessage('Name');
        return;
    }

    if (!checkDigitsInput($('#addPrice').val())) {
        alert('Only numbers can be entered as a price.');
        return;
    }

    if (!checkBiggestPrice($('#addPrice').val())) {
        return;
    }

    var product;
    var photoTemp = '';
    var fileExists = true;

    if (!window.File || !window.FileReader) { //(!window.File || !window.FileReader || !window.FileList || !window.Blob) {
        alert('Необходимые для работы с файлами File API не поддерживаются Вашим броузером.');
        $('#addPhoto').val('');
    }

    var photoinput = ($('#addPhoto'))[0];
    var inputFiles = photoinput.files;
    if (inputFiles == 'undefined') {
        fileExists = false;
    }
    var file;
    var fname;
    var fsize;
    var ftype;
    try {
        file = inputFiles[0];
        fname = file.name;
        fsize = file.size;
        ftype = file.type;
    } catch (err) {
        //alert(err);
        fname = '';
        fsize = 0;
        ftype = '';
    }

    if (fsize > 0) {
        var fr = new FileReader();
        fr.readAsDataURL(file);
        fr.onload = (function (file) {
            return function (e) {
                product = {
                    Code: $('#addCode').val(),
                    Name: $('#addName').val(),
                    Price: $('#addPrice').val(),
                    Photo: e.target.result
                };
                addProductToDB(product);
            };
        })(file);
    } else {
        product = {
            Code: $('#addCode').val(),
            Name: $('#addName').val(),
            Price: $('#addPrice').val(),
            Photo: ''
        };
        addProductToDB(product);
    }





}

function addProductToDB(product) {
    $.ajax({
        url: murl + 'api/values/',
        type: 'POST',
        data: JSON.stringify(product),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            var res = '';
            var json = JSON.parse(data);
            if (json.Result == 'OK') {
                ClearInputs();
                GetAllItems();
            } else {
                alert(json.Result);
            }
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
}

function DeleteItem(el) {
    if (confirm("Remove product permanently?")) {
        var id = $(el).attr('data-item');
        DeleteProduct(id);
    }

}

function EditItem(el) {
    var id = $(el).attr('data-item');
    GetProduct(id);
}

function GetProduct(id) {
    $.ajax({
        url: murl + 'api/values/' + id,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            ShowProduct(data);
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
}

function ShowProduct(data) {
    $('#editPhoto').val('');
    if (data != null) {
        $("#createBlock").hide();
        $("#editBlock").show();
        $("#editId").val(data.Id);
        $("#editCode").val(data.Code);
        $('#editHiddenCode').val(data.Code);
        $("#editName").val(data.Name);
        $("#editPrice").val(data.Price);
        $("#editImage").attr('src', data.Photo);
        if ($('#editImage').attr('src') != '') {
            $('#editedPhotoInput').hide();
            $('#deletePictureButton').show();
        } else {
            $('#editedPhotoInput').show();
            $('#editedPhotoInput').val('');
            $('#deletePictureButton').hide();
        }
    }
    else {
        alert("This product does not exists.");
    }
}

function DeleteProduct(id) {

    $.ajax({
        url: murl + 'api/values/' + id,
        type: 'DELETE',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            GetAllItems();
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
}

function SaveEdited() {

    if (!CheckInputs($('#editCode').val())) {
        InvalidCharsMessage('Code');
        return;
    }
    if (!CheckInputs($('#editName').val())) {
        InvalidCharsMessage('Name');
        return;
    }
    if (!checkDigitsInput($('#editPrice').val())) {
        InvalidCharsMessage('Price');
        return;
    }

    if (!checkBiggestPrice($('#editPrice').val())) {
        return;
    }

    var photoinput = ($('#editPhoto'))[0];
    var inputFiles = photoinput.files;
    if (inputFiles == 'undefined') {
        fileExists = false;
    }
    var file;
    var fname;
    var fsize;
    var ftype;
    try {
        file = inputFiles[0];
        fname = file.name;
        fsize = file.size;
        ftype = file.type;
    } catch (err) {
        //alert(err);
        fname = '';
        fsize = 0;
        ftype = '';
    }

    if (fsize > 0) {
        var fr = new FileReader();
        fr.readAsDataURL(file);
        fr.onload = (function (file) {
            return function (e) {
                product = {
                    Id: $('#editId').val(),
                    Code: $('#editCode').val(),
                    Name: $('#editName').val(),
                    Price: $('#editPrice').val(),
                    Photo: e.target.result
                };
                SaveEditedProduct($('#editId').val(), product);
            };
        })(file);
    } else {
        product = {
            Id: $('#editId').val(),
            Code: $('#editCode').val(),
            Name: $('#editName').val(),
            Price: $('#editPrice').val(),
            Photo: ''
        };
        SaveEditedProduct($('#editId').val(), product);
    }
}

function SaveEditedProduct(id, product) {
    $.ajax({
        url: murl + 'api/values/' + id,
        type: 'PUT',
        data: JSON.stringify(product),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            $("#editBlock").hide();
            $("#createBlock").show();
            GetAllItems();
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
}

function DeletePicture() {
    var id = $('#editId').val();

    // $.ajax({
    //    url: '/RemoveImage/' + $('#editHiddenCode').val(),
    //    type: 'POST',
    //    dataType: 'json',
    //    success: function (data) {
    //        GetProduct(id);
    //    },
    //    error: function (x, y, z) {
    //        alert(x + '\n' + y + '\n' + z);
    //    }
    //});
    if (confirm('Delete image permanently?')) {
        $('#resultdiv').load(murl + 'RemovePicture/Index?code=' + encodeURIComponent($('#editHiddenCode').val()), function (data) {
            GetProduct(id);
            GetAllItems();
        });
    }

}

function ExportToExcel() {
    $.ajax({
        url: murl + 'api/ExportProductsList/' + $('#filterInput').val(),
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $('#downloadA').attr('href', data);
            
            var link = $('#downloadA')[0];
            var linkEvent = null;
            if (document.createEvent) {
                linkEvent = document.createEvent('MouseEvents');
                linkEvent.initEvent('click', true, true);
                link.dispatchEvent(linkEvent);
            }
            else if (document.createEventObject) {
                linkEvent = document.createEventObject();
                link.fireEvent('onclick', linkEvent);
            }

        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
}


function ShowNewProductForm() {
    $("#editBlock").hide();

    $("#createBlock").show();

    ClearInputs();
}

function ClearFilter() {
    $('#filterInput').val('');
    GetAllItems();
}

function ClearInputs() {
    $('#addCode').val('');
    $('#addName').val('');
    $('#addPrice').val('');
    $('#addPhoto').val('');
}

function CheckInputs(val) {

    //var regExpEn = /^[A-Za-z0-9 _]*[A-Za-z0-9]*$/;

   // var regExpRu = /^[А-Яа-я0-9 _]*[А-Яа-я0-9][А-Яа-я0-9 _]*$/;

    // alert(regExpCheck.test($('#filterInput').val()));

    var regExpEn = /^[A-Za-zА-Яа-я0-9 _]*[A-Za-zА-Яа-я0-9]*$/;

    var checkEn = regExpEn.test(val);
    //var checkRu = regExpRu.test(val);

    if (!checkEn) {
        return false;
    } else {
        return true;
    }

}

function checkDigitsInput(val) {
    var regExp = /^[0-9.]*$/;
    return regExp.test(val);
}

function InvalidCharsMessage(word) {
    alert(word + ' string contains illegal characters');
}

function checkBiggestPrice(price) {
    if (price > 999) {
        return confirm('You entered a price greater than 999. Leave this price?');
    } else {
        return true;
    }
}