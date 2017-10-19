(function (tpn_charsheet, $, undefined) {

    tpn_charsheet.config = {
        controller: 'CharacterSheets'           
        , formeo: null
        , pageLoaded: false
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
            events: {
                formeoLoaded: function () {
                    tpn_charsheet.config.pageLoaded = true;
                }
            },
            svgSprite: tpn_common.getRootUrl() + 'Content/images/formeo-sprite.svg',
            sessionStorage: false,
            editPanelOrder: ['attrs', 'options'],
            container: 'form-edit',
            style: '/Content/theme/' + ($("#CharacterSheetTheme").val() || 'default') + '.css'
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
            var $controls = $(".formeo-controls");
            if (parseInt($controls.css("margin-top")) <= $(".stage-wrap").outerHeight())
            {
                $controls
                    .stop()
                    .animate({ "marginTop": ($(window).scrollTop() + 0) + "px" }, "slow");
            }
        });               
    };

    tpn_charsheet.saveSheet = function (formeoObj, metaDataForm, minorVersion) {
        var metaData = new FormData(document.getElementById(metaDataForm));

        //take a new screenshot if this is not a minor version
        if (!minorVersion) {
            var loading = $.loading({
                tip: "creating screenshot ..."
                , width: '175px'
                , imgPath: '/Content/images/ajax-loading.gif'
            });
            loading.open();
        }

        var data = {
            'CharacterSheetId': metaData.get('CharacterSheetId'),
            'CharacterSheetName': metaData.get('CharacterSheetName'),
            'CharacterSheetUrl': metaData.get('CharacterSheetUrl'),
            'CharacterSheetTheme': metaData.get('CharacterSheetTheme'),
            'ParentId': metaData.get('ParentId') || tpn_common.emptyguid,
            'Published': metaData.get('Published') === "on" ? true : false,
            'CharacterSheetForm': JSON.stringify(JSON.parse(formeoObj.formData)),
            'UserId': metaData.get('UserId'),
            'MinorVersion': minorVersion
        }
    
        tpn_common.ajax(tpn_charsheet.config.controller, data, function () { if (!minorVersion) loading.close(); });
      
    };

    tpn_charsheet.print = function() {
        document.body.classList.toggle('form-rendered', true);
        tpn_charsheet.renderFormeo();
        tpn_common.fixPrintables();
    }

    tpn_charsheet.renderFormeo = function()
    {
        try {
            tpn_charsheet.config.formeo.render(tpn_charsheet.config.renderContainer);            
        }
        catch(ex)
        {
            window.setTimeout(function () {
                tpn_charsheet.config.formeo.render(tpn_charsheet.config.renderContainer);
            }, 500);
        }
    }

    function bindDOM() {
        $('.preview').on('click', function (e) {
            e.preventDefault();
            tpn_charsheet.print();
            return false;
        })

        $("form.main-form").on('submit', function (e) {
            e.preventDefault();
            return false;
        });

        $(document).on('click', 'button.save-form, .js-btn-save', function (e) {
            e.preventDefault();
            tpn_charsheet.saveSheet(tpn_charsheet.config.formeo, 'meta-data', false);
            return false;
        });

        $("#CharacterSheetName").on('blur', function () {
            var $this = $(this);
            if ($("#CharacterSheetUrl").val() === "") {
                //get url safe character slug
                var charSlug = $this.val().toLowerCase().replace(/[^a-zA-Z0-9-_]/g, '-');
                $("#CharacterSheetUrl").val(charSlug);
            }
        });

        window.setTimeout(function () {
            if (tpn_common.config.routeaction.toLowerCase() === 'print') {
                document.body.classList.toggle('form-rendered', true);
                tpn_charsheet.renderFormeo();
                tpn_common.fixPrintables();                
            }
        }, 500);
    }

    tpn_charsheet.init = function() {
        bindDOM();
    }

})(window.tpn_charsheet = window.tpn_charsheet || {}, jQuery);

(function ($) {
    tpn_charsheet.init();
})(jQuery);
