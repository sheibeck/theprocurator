(function (tpn_common, $, undefined) {

    tpn_common.emptyguid = '00000000-0000-0000-0000-000000000000';

    tpn_common.config = {
        routeaction: null
    }

    tpn_common.getRootUrl = function () {
        var pathArray = location.href.split('/');
        var protocol = pathArray[0];
        var host = pathArray[2];
        var url = protocol + '//' + host + '/';
        return url;
    }

    tpn_common.ajax = function (controller, data) {
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

           if (data.url) {
               window.setTimeout(function () {
                   document.location = data.url;
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
        var formData = new FormData($form[0]);
        var result = {};

        for (var entry of formData.entries())
        {
            if (result[entry[0]]) {
                result[entry[0]] = result[entry[0]] + "," + entry[1];
            }
            else {
                result[entry[0]] = entry[1];
            }
        }
        result = JSON.stringify(result);
        return result;
    }
})(window.tpn_common = window.tpn_common || {}, jQuery);
