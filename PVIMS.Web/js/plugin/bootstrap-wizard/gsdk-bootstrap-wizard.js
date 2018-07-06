/*!

 =========================================================
 * Bootstrap Wizard - v1.1.1
 =========================================================
 
 * Product Page: https://www.creative-tim.com/product/bootstrap-wizard
 * Copyright 2017 Creative Tim (http://www.creative-tim.com)
 * Licensed under MIT (https://github.com/creativetimofficial/bootstrap-wizard/blob/master/LICENSE.md)
 
 =========================================================
 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

searchVisible = 0;
transparent = true;

$(document).ready(function(){

    /*  Activate the tooltips      */
    $('[rel="tooltip"]').tooltip();

    var previewShown = false;

    // Code for the Validator
    var $validator = $('.wizard-card form').validate({
		  rules: {
		      'DatasetCategories[0].DatasetElements[0].DatasetElementValue': {
		      required: true
		    },
		    lastname: {
		      required: true		    },
		    email: {
		      required: true		    }
        }
	});

    // Wizard Initialization
  	$('.wizard-card').bootstrapWizard({
        'tabClass': 'nav nav-pills',
        'nextSelector': '.btn-next',
        'previousSelector': '.btn-previous',

        onNext: function(tab, navigation, index) {
        	var $valid = $('.wizard-card form').valid();
        	if(!$valid) {
        		$validator.focusInvalid();
        		return false;
        	}
        },

        onInit : function(tab, navigation, index){

          //check number of tabs and fill the entire row
          var $total = navigation.find('li').length;
          $width = 100/$total;
          var $wizard = navigation.closest('.wizard-card');

          $display_width = $(document).width();

          if($display_width < 600 && $total > 3){
              $width = 50;
          }

           navigation.find('li').css('width',$width + '%');
           $first_li = navigation.find('li:first-child a').html();
           $moving_div = $('<div class="moving-tab">' + $first_li + '</div>');
           $('.wizard-card .wizard-navigation').append($moving_div);
           refreshAnimation($wizard, index);
           $('.moving-tab').css('transition','transform 0s');
       },

        onTabClick : function(tab, navigation, index){

            var $valid = $('.wizard-card form').valid();

            if(!$valid){
                return false;
            } else {
                return true;
            }
        },

        onTabShow: function(tab, navigation, index) {
            var $total = navigation.find('li').length;
            var $current = index+1;

            var $wizard = navigation.closest('.wizard-card');

            // If it's the last tab then hide the last button and show the finish instead
            if ($current >= $total - 1)
            {
                $($wizard).find('.btn-next').hide();
                $($wizard).find('.btn-preview').show();
                $($wizard).find('.btn-finish').hide();
            }
            else
            {
                $($wizard).find('.btn-next').show();
                $($wizard).find('.btn-preview').hide();
                $($wizard).find('.btn-finish').hide();
            }

            button_text = navigation.find('li:nth-child(' + $current + ') a').html();

            setTimeout(function(){
                $('.moving-tab').text(button_text);
            }, 150);

            var checkbox = $('.footer-checkbox');

            if( !index == 0 ){
                $(checkbox).css({
                    'opacity':'0',
                    'visibility':'hidden',
                    'position':'absolute'
                });
            } else {
                $(checkbox).css({
                    'opacity':'1',
                    'visibility':'visible'
                });
            }

            refreshAnimation($wizard, index);
        }
  	});

    $('[data-toggle="wizard-radio"]').click(function(){
        wizard = $(this).closest('.wizard-card');
        wizard.find('[data-toggle="wizard-radio"]').removeClass('active');
        $(this).addClass('active');
        $(wizard).find('[type="radio"]').removeAttr('checked');
        $(this).find('[type="radio"]').attr('checked','true');
    });

    $('[data-toggle="wizard-checkbox"]').click(function(){
        if( $(this).hasClass('active')){
            $(this).removeClass('active');
            $(this).find('[type="checkbox"]').removeAttr('checked');
        } else {
            $(this).addClass('active');
            $(this).find('[type="checkbox"]').attr('checked','true');
        }
    });

    $('.set-full-height').css('height', 'auto');

    $('.btn-preview').click(function () {
        var $valid = $('.wizard-card form').valid();
        if (!$valid) {
            $validator.focusInvalid();
            return false;
        }
        // Get all elements that have been captured
        $('#form').submit();
    });

    $('.btn-finish').click(function () {
        var $valid = $('.wizard-card form').valid();
        if (!$valid) {
            $validator.focusInvalid();
            return false;
        }
        $('#form').submit();
    });

    $('#form').on("submit", function (event) {
        // show the preview tab
        $('#wizardtabs a:last').tab('show');

        var name = '';
        var val = '';

        // If it's the second last tab then handle preview else handle submit
        if (!previewShown)
        {
            $($wizard).find('.btn-preview').hide();
            $($wizard).find('.btn-finish').show();

            $("#tblPreview").find("tr:gt(1)").remove();
            $('#form').find("input[name*='DatasetElementName'], input[name*='DatasetElementValue'], select[name*='DatasetElementName'], select[name*='DatasetElementValue'], textarea[name*='DatasetElementName'], textarea[name*='DatasetElementValue']").each(function () {
                if ($(this).attr('name').indexOf('DatasetElementName') != -1) {
                    name = $(this).attr('value');
                };
                if ($(this).attr('name').indexOf('DatasetElementValue') != -1) {
                    val = $('#' + $(this).attr('id')).val();

                    if (val != '') {
                        $('#tblPreview tr:last').after('<tr><th>' + name + '</th><td class="text-left">' + val + '</td></tr>');
                    }
                };
            });
            previewShown = true;
            event.preventDefault();
        }
    });

    $('.btn-cancel').click(function () {
        window.location.href = "/Home/Index";
    });

});

 $(window).resize(function(){
    $('.wizard-card').each(function(){
        $wizard = $(this);
        index = $wizard.bootstrapWizard('currentIndex');
        refreshAnimation($wizard, index);

        $('.moving-tab').css({
            'transition': 'transform 0s'
        });
    });
});

function refreshAnimation($wizard, index){
    total_steps = $wizard.find('li').length;
    move_distance = $wizard.width() / total_steps;
    step_width = move_distance;
    move_distance *= index;

    $wizard.find('.moving-tab').css('width', step_width);
    $('.moving-tab').css({
        'transform':'translate3d(' + move_distance + 'px, 0, 0)',
        'transition': 'all 0.3s ease-out'

    });
}

function debounce(func, wait, immediate) {
	var timeout;
	return function() {
		var context = this, args = arguments;
		clearTimeout(timeout);
		timeout = setTimeout(function() {
			timeout = null;
			if (!immediate) func.apply(context, args);
		}, wait);
		if (immediate && !timeout) func.apply(context, args);
	};
};
