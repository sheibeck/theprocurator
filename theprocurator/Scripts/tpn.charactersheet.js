(function (tpn_charsheet, $, undefined) {

    tpn_charsheet.config = {
        editing: true        
        , formeo: null
        , renderContainer: document.querySelector('.render-form')
        , formeoOpts: {        
            controls: {
                sortable: false,
                groupOrder: [
                  'common',
                  'html',
                  'layout'
                ],          
                elementOrder: {
                    common: [                
                        'text-input',
                        'textarea',
                        'select',
                        'checkbox',
                        'radio',
                        'upload'                    
                    ]
                },
                disable: {
                    groups: [],
                    elements: [                
                        'date-input',
                        'hidden',
                        'number',
                        'button'
                    ]
                }
            },       
            svgSprite: tpn_common.getRootUrl() + 'Content/images/formeo-sprite.svg',
            sessionStorage: true,
            editPanelOrder: ['attrs', 'options'],
            container: 'form-edit'
        }
    };

    tpn_charsheet.initFormeo = function (elem) {
        window.sessionStorage.removeItem('formData');
        tpn_charsheet.config.formeoOpts.container = elem;
        tpn_charsheet.config.formeo = new Formeo(tpn_charsheet.config.formeoOpts, $('#CharacterSheetForm').val());
        
        if ($('#CharacterSheetId').val() == '00000000-0000-0000-0000-000000000000')
        {
            $('#CharacterSheetId').val(JSON.parse(tpn_charsheet.config.formeo.formData).id);
        }
    };

    tpn_charsheet.saveSheet = function (fm) {
        var formForgeryToken = $('input[name="__RequestVerificationToken"]').val();
        var sheetId = $('#CharacterSheetId').val();
        var sheetName = $('#CharacterSheetName').val();
        var sheetUrl = $('#CharacterSheetUrl').val();
        var sheetUser = $('#UserId').val();
        var sheetForm = JSON.stringify(JSON.parse(fm.formData));

        var data = {
                'CharacterSheetId' : sheetId,
                'CharacterSheetName': sheetName,
                'CharacterSheetUrl': sheetUrl,
                'CharacterSheetForm': sheetForm,
                'UserId': sheetUser
        }

        var jqxhr = $.ajax({
            url: tpn_common.getRootUrl() + 'CharacterSheets/' + tpn_common.config.routeaction,
            contentType: 'application/json; charset=utf-8',
            type: 'POST',
            data: JSON.stringify(data),
            headers: {
                "__RequestVerificationToken": formForgeryToken
            }
        })
        .done(function (data) {            
            ShowNotification(data.message, data.type, data.position, data.modal);

            if (data != null && data.url) {
                window.setTimeout(function () {
                    document.location = data.url;
                }, 1000);
            }            
        })
        .fail(function (xhr, err) {
            console.log(xhr.responseText);
        })
    };

    function bindDOM() {
        $('.preview').on('click', function () {
            var $this = $(this);            

            document.body.classList.toggle('form-rendered', tpn_charsheet.config.editing);

            if (tpn_charsheet.config.editing) {                
                tpn_charsheet.config.formeo.render(tpn_charsheet.config.renderContainer);
                $this.text('Edit Form');
            } else {
                $this.text('Print Preview');
            }

            return tpn_charsheet.config.editing = !tpn_charsheet.config.editing;
        })

        $("form.build-form").on('submit', function (e) {
            e.preventDefault();
            return false;
        });

        $(document).on('click', 'button.save-form', function (e) {
            e.preventDefault();
            tpn_charsheet.saveSheet(tpn_charsheet.config.formeo);
            return false;
        });
    }

    tpn_charsheet.init = function() {
        bindDOM();
    }

})(window.tpn_charsheet = window.tpn_charsheet || {}, jQuery);

(function ($) {
    tpn_charsheet.init();
})(jQuery);
