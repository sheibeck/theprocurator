(function (tpn_char, $, undefined) {

    tpn_char.config = {
        controller: 'Characters'
        , formeo: null
        , renderContainer: document.querySelector('.character-form')
        , formeoOpts: {            
            allowEdit: false,
            svgSprite: tpn_common.getRootUrl() + 'Content/images/formeo-sprite.svg',
            sessionStorage: true,
            editPanelOrder: ['attrs', 'options'],
            container: 'form-edit'
        }
    };

    tpn_char.initFormeo = function (elem) {        
        tpn_char.config.formeoOpts.container = elem;
        tpn_char.config.formeo = new Formeo(tpn_char.config.formeoOpts, $('#CharacterSheet_CharacterSheetForm').val());

        //show the character sheet form right off the bat
        window.setTimeout(function () {
            tpn_char.loadCharacter();
        }, 1000);
    };

    tpn_char.loadCharacter = function () {
        tpn_char.config.formeo.render(tpn_char.config.renderContainer);

        var data = JSON.parse($("#CharacterData").val());
        for (var key in data) {
            $('#' + key)
                .text(data[key])
                .val(data[key]);
        }
    }

    tpn_char.saveCharacter = function ($form, metaDataForm) {
        var metaData = new FormData(document.getElementById(metaDataForm));

        var data = {            
            'CharacterSheetId': metaData.get('CharacterSheetId'),
            'CharacterName': metaData.get('CharacterName'),
            'CharacterUrl': metaData.get('CharacterUrl'),
            'CharacterData': JSON.stringify(JSON.parse(tpn_common.formDataToJSON($form))),
            'UserId': metaData.get('UserId')
        }

        if (tpn_common.config.routeaction === "edit")
        {
            data["CharacterId"] = metaData.get('CharacterId')
        }

        tpn_common.ajax(tpn_char.config.controller, data);
    };

    function bindDOM() {
        $(".js-btn-save").on('click', function (e) {
            $("form.character-form").submit();
        });

        $("form.character-form").on('submit', function (e) {
            e.preventDefault();
            tpn_char.saveCharacter($(this), 'meta-data');
            return false;
        });        
    }

    tpn_char.init = function () {        
        bindDOM();
    }

})(window.tpn_char = window.tpn_char || {}, jQuery);

(function ($) {
    tpn_char.init();
})(jQuery);
