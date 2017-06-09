(function (tpn_common, $, undefined) {

    tpn_common.emptyguid = '00000000-0000-0000-0000-000000000000';

    tpn_common.config = {
        routeaction: null
        , reloadUI: false
    }

    tpn_common.getRootUrl = function () {
        var pathArray = location.href.split('/');
        var protocol = pathArray[0];
        var host = pathArray[2];
        var url = protocol + '//' + host + '/';
        return url;
    }

    tpn_common.ajax = function (controller, data, callback) {
        //var formForgeryToken = $('input[name="__RequestVerificationToken"]').val();

        var jqxhr = $.ajax({
            url: tpn_common.getRootUrl() + controller + '/' + tpn_common.config.routeaction,
            contentType: 'application/json; charset=utf-8',
            type: 'POST',
            data: JSON.stringify(data)
            /*, headers: {
                 "__RequestVerificationToken": formForgeryToken
             } */
        })
       .done(function (data) {
           ShowNotification(data.message, data.type, data.position, data.modal);

           if (callback) {
               callback();
           }

           if (data.url) {
               window.setTimeout(function () {
                   document.location = data.url;
               }, 1000);
           }

           if (tpn_common.config.reloadUI)
           {
               setTimeout(function () {
                   location.reload();
               }, 1000);
           }
       })
       .fail(function (xhr, err) {
           var modelStateErrors = xhr.responseJSON;
           ShowNotification("Error saving form", "error", "center", false);

           for (var i = 0; i < modelStateErrors.length; i++) {
               $('span[data-valmsg-for="' + modelStateErrors[i].key + '"]').text(modelStateErrors[i].errors[0]);
           }
       })
    }

    tpn_common.formDataToJSON = function ($form) {        
        //upload files first        
        var formData = new FormData($form[0]);
        var result = {};

        for (var entry of formData.entries())
        {
            if (typeof entry[1] == 'object')
            {
                if (entry[1].size > 0) {
                    var fileId = guid();
                    uploadFile(entry[0], fileId);
                    result[entry[0]] = fileId + "_" + entry[1].name;

                    tpn_common.config.reloadUI = true; //force reload after the save so the image shows up
                }
                else {
                    result[entry[0]] = "";
                }
            }
            else {
                result[entry[0]] = entry[1];
            }
        }
        result = JSON.stringify(result);
        return result;
    }

    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
              .toString(16)
              .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
          s4() + '-' + s4() + s4() + s4();
    }

    function uploadFile(inputId, fileId) {
        var $file = $("#" + inputId);
        var formData = new FormData();            
        var totalFiles = $file.get(0).files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = $file.get(0).files[i];
            formData.append("FileUpload", file);
        }

        formData.append("id", fileId);

        $.ajax({
            type: "POST",
            url: '/Characters/FileUpload',
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            async: false,
            success: function (response) {
                console.log("uploaded file: " + JSON.stringify(formData))              
            },
            error: function (error) {
                ShowNotification("Error saving image: " + error.responseText, "error", "center", false);
            }
        });
    }

    tpn_common.deleteFile = function($image) {              
        var formData = new FormData();       
        formData.append("fileName", $image.data("image"));

        $.ajax({
            type: "POST",
            url: '/Characters/FileDelete',
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            async: false,
            success: function (response) {
                console.log("uploaded file: " + JSON.stringify(formData))
            },
            error: function (error) {
                ShowNotification("Error removing file: " + error.responseText, "error", "center", false);
            }
        });
    }

    // find any HR tags and drop a page-break on its parent div
    // to support page breaks on print
    tpn_common.fixPageBreaks = function () {        
        $('hr').each(function () {
            var $elem = $(this);
            $elem.closest('.f-row').addClass('page-break');
        })
    }

})(window.tpn_common = window.tpn_common || {}, jQuery);

(function ($) {
    //fix console.log issues for IE
    if (!window.console) window.console = {};
    if (!window.console.log) window.console.log = function () { };
})(jQuery);
