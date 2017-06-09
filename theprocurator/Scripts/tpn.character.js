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
            container: 'form-edit',
            style: '/Content/theme/' + ($("#CharacterSheet_CharacterSheetTheme").val() || 'default') + '.css'
        }
    };

    tpn_char.initFormeo = function (elem) {        
        tpn_char.config.formeoOpts.container = elem;
        tpn_char.config.formeo = new Formeo(tpn_char.config.formeoOpts, $('#CharacterSheet_CharacterSheetForm').val());

        //show the character sheet form right off the bat
        window.setTimeout(function () {            
            tpn_char.renderFormeo(tpn_char.loadCharacter);
        }, 1000);
    };


    tpn_char.renderFormeo = function(callback)
    {
        try {
            tpn_char.config.formeo.render(tpn_char.config.renderContainer);
            if (callback)
                callback();
        }
        catch (ex) {
            window.setTimeout(function () {
                tpn_char.config.formeo.render(tpn_char.config.renderContainer);
                if (callback)
                    callback();
            }, 500);
        }
        
    }

    tpn_char.loadCharacter = function () {        
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
                                    if (data[key]) {
                                        $('#' + key).after('<a href="#" class="remove-image js-no-print" data-input-id="' + key + '" data-image="' + data[key] + '">[remove]</a>')
                                                    .after('<img class="img-responsive" src="/Content/Character/Images/' + data[key] + '" alt="' + data[key] + '" />');
                                        $('#' + key).prev().remove(); // hide the upload label
                                        $('#' + key).after("<input type='hidden' name='" + key + "' id='" + key + "' value='" + data[key] + "' />"); // add a hidden input so we keep the current value
                                        $('input[type=file][id=' + key + ']').remove(); // hide the upload control
                                    }

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

            // insert page breaks for properly paged printing
            tpn_common.fixPrintables();
        }
    }

    tpn_char.saveCharacter = function ($form, metaDataForm) {
        var metaData = new FormData(document.getElementById(metaDataForm));

        // make sure our json formatted data is not value "false"
        var jsonData = tpn_common.formDataToJSON($form);
        if (jsonData) {
            var data = {
                'CharacterSheetId': metaData.get('CharacterSheetId'),
                'CharacterName': metaData.get('CharacterName'),
                'CharacterUrl': metaData.get('CharacterUrl'),
                'Published': metaData.get('Published') === "on" ? true : false,
                'CharacterData': JSON.stringify(JSON.parse(jsonData)),
                'UserId': metaData.get('UserId')
            }

            if (tpn_common.config.routeaction === "Edit") {
                data["CharacterId"] = metaData.get('CharacterId')
            }

            tpn_common.ajax(tpn_char.config.controller, data);
        }
    };

    function bindDOM() {
        $('.print-pdf').on('click', function () {
            var printUrl = tpn_common.getRootUrl() + 'Characters/Pdf/' + $('#CharacterId').val();
            window.open(printUrl);
        });

        $(".js-btn-save").on('click', function (e) {
            e.preventDefault();
            $("form.main-form").submit();
            return false;
        });

        $("form.main-form").on('submit', function (e) {
            e.preventDefault();
            tpn_char.saveCharacter($(this), 'meta-data');
            return false;
        });

        $(document).on('click', '.remove-image', function (e) {
            var $image = $(this);
            tpn_common.deleteFile($image);
            
            $('#' + $image.data('input-id')).val(''); // clear out the image so it's properly deleted from the db

            $image.prev().remove();
            $image.prev().show();
            $image.remove();

            tpn_common.config.reloadUI = true;

            $('.js-btn-save').click();            
        });              
    }

    tpn_char.init = function () {        
        bindDOM();
    }

})(window.tpn_char = window.tpn_char || {}, jQuery);

(function ($) {
    tpn_char.init();
})(jQuery);
