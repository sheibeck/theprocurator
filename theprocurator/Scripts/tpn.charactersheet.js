(function (tpn_charsheet, $, undefined) {

    tpn_charsheet.config = {
        controller: 'CharacterSheets'           
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
        
        if ($('#CharacterSheetId').val() === tpn_common.emptyguid)
        {
            $('#CharacterSheetId').val(JSON.parse(tpn_charsheet.config.formeo.formData).id);
        }

        // keep the toolbar in view        
        $(window).scroll(function () {
            $(".formeo-controls")
				.stop()
				.animate({ "marginTop": ($(window).scrollTop() + 0) + "px" }, "slow");
        });               
    };

    tpn_charsheet.saveSheet = function (formeoObj, metaDataForm) {
        var metaData = new FormData(document.getElementById(metaDataForm));

        var data = {
            'CharacterSheetId': metaData.get('CharacterSheetId'),
            'CharacterSheetName': metaData.get('CharacterSheetName'),
            'CharacterSheetUrl': metaData.get('CharacterSheetUrl'),
            'CharacterSheetForm': JSON.stringify(JSON.parse(formeoObj.formData)),
            'UserId': metaData.get('UserId')
        }

        tpn_common.ajax(tpn_charsheet.config.controller, data);
    };

    function bindDOM() {
        $('.preview').on('click', function () {           
            document.body.classList.toggle('form-rendered', true);
            tpn_charsheet.config.formeo.render(tpn_charsheet.config.renderContainer);            
            window.print();
            document.body.classList.toggle('form-rendered', false);            
        })



        $("form.build-form").on('submit', function (e) {
            e.preventDefault();
            return false;
        });

        $(document).on('click', 'button.save-form, button.js-btn-save', function (e) {
            e.preventDefault();
            tpn_charsheet.saveSheet(tpn_charsheet.config.formeo, 'meta-data');
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
