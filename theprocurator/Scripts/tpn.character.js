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

        // render the character values
        var charData = $("#CharacterData").val();
        if (charData) {
            var data = JSON.parse(charData);
            var controls = JSON.parse($("#CharacterSheet_CharacterSheetForm").val());
            for (var key in data) {
                try {
                    switch (controls.fields[key].tag) {
                        case 'select':
                            $("select[id='" + key + "']").val(data[key]);
                            break;

                        case 'textarea':
                            $('#' + key)
                                .text(data[key])
                            break;

                        default:
                            switch (controls.fields[key].attrs.type) {
                                case 'checkbox':
                                    //uncheck any defaults
                                    $("input[id='" + key + "'][type=checkbox]").prop("checked", false);

                                    //check the real values
                                    $.each(data[key].split(','), function (item, value) {
                                        $("input[id='" + key + "'][type=checkbox][value='" + value + "']").prop("checked", true);
                                    })
                                    break;

                                case 'radio':
                                    //uncheck any defaults
                                    $("input[id='" + key + "'][type=checkbox]").prop("checked", false);

                                    // check the real values
                                    $.each(data[key].split(','), function (item, value) {
                                        $("input[id='" + key + "'][type=radio][value='" + value + "']").prop("checked", true);
                                    })
                                    break;

                                case 'file':
                                    break;

                                default:
                                    $('#' + key)
                                        .text(data[key])
                                        .val(data[key]);
                            }

                    }
                }
                catch (ex) {
                    //console.log(ex);
                    continue;
                }
            }
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

        if (tpn_common.config.routeaction === "Edit")
        {
            data["CharacterId"] = metaData.get('CharacterId')
        }

        tpn_common.ajax(tpn_char.config.controller, data);
    };

    function bindDOM() {
        $('.print-pdf').on('click', function () {
            var printUrl = tpn_common.getRootUrl() + 'Characters/Pdf/' + $('#CharacterId').val();
            window.open(printUrl);
        });

        $(".js-btn-save").on('click', function (e) {
            $("form.character-form").submit();
        });

        $("form.character-form").on('submit', function (e) {
            e.preventDefault();
            tpn_char.saveCharacter($(this), 'meta-data');
            return false;
        });

        $('.preview').on('click', function () {
            var $this = $(this);

            document.body.classList.toggle('form-rendered', tpn_charsheet.config.editing);

            if (tpn_charsheet.config.editing) {
                tpn_charsheet.config.formeo.render(tpn_charsheet.config.renderContainer);
                $this.text('Edit Form');
            } else {
                $this.text('Print');
            }

            return tpn_charsheet.config.editing = !tpn_charsheet.config.editing;
        })
    }

    tpn_char.init = function () {        
        bindDOM();
    }

})(window.tpn_char = window.tpn_char || {}, jQuery);

(function ($) {
    tpn_char.init();
})(jQuery);
