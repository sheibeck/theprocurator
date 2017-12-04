$(function () {
    window.setTimeout(function () {
        if (tpn_common.config.routeaction.toLowerCase() === 'edit') {
            if (tpn_common.config.controller.toLowerCase() === 'charactersheets')
                $("textarea.input-half").parent().parent().parent().addClass("logo-fields");
            else 
                $("textarea.input-half").parent().parent().addClass("logo-fields");
            
        } else {
            $("textarea.input-half").parent().addClass("logo-fields");
        }
    }, 1000);
});