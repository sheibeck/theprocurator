(function (tpn_common, $, undefined) {
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

})(window.tpn_common = window.tpn_common || {}, jQuery);
