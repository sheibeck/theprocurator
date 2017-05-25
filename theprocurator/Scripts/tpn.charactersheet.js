(function (tpn_charsheet, $, undefined) {
    
    tpn_charsheet.formeoOpts = {
        container: 'form-edit',
        i18n: {
            preloaded: {
                'en-US': { 'row.makeInputGroup': ' Repeatable Region' }
            }
        },
        // allowEdit: false,
        controls: {
            sortable: false,
            groupOrder: [
              'common',
              'html',
            ],
            elementOrder: {
                common: [
                'button',
                'checkbox',
                'date-input',
                'hidden',
                'upload',
                'number',
                'radio',
                'select',
                'text-input',
                'textarea',
                ]
            }
        },
        events: {
            // onUpdate: console.log,
            // onSave: console.log
        },
        svgSprite: '../Content/images/formeo-sprite.svg',
        // debug: true,
        sessionStorage: true,
        editPanelOrder: ['attrs', 'options']
    };
    tpn_charsheet.initFormeo = function (elem) {
        tpn_charsheet.formeoOpts.container = elem;
        new Formeo(tpn_charsheet.formeoOpts);
    };

})(window.tpn_charsheet = window.tpn_charsheet || {}, jQuery);