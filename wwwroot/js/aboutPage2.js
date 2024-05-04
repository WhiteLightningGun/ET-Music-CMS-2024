// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// JQuery comes as part of boostrap...

$('#Astuff').hide();
$('.mastheadB').hide();
$('#Bstuff').hide();
$('.mastheadC').hide();
$('#Cstuff').hide();

$(document).ready(function(){
    
    // jQuery methods go here...
    let marker = 1;
    
    $('.masthead').css("visibility", "visible");
    $('#Astuff').delay(1000).fadeIn(1000);

    $('.clickable').on('click', (event) => {
        if (!event.detail || event.detail == 1)
        {
            if (marker === 1)
            {
                $('.mastheadC').hide();
                $('.mastheadB').delay(1000).fadeIn(1000);
                $('#Bstuff').delay(2000).fadeIn(1000);
                $('#Astuff').fadeOut(1000);
                marker += 1;
                return; //end the loop
            }
            
            if (marker === 2)
            {
                $('.mastheadC').delay(1000).fadeIn(1000);
                $('#Cstuff').delay(2000).fadeIn(1000);
                $('#Bstuff').fadeOut(1000);
                marker += 1;
                return;
            }

            if (marker === 3) // returning to initial frame one state smoothly
            {
                $('.mastheadB').delay(1000).fadeOut(1000);
                //$('.mastheadC').delay(1000).fadeOut(1000);
                $('#Cstuff').fadeOut(1000);
                $('#Astuff').delay(2000).fadeIn(1000);
                marker = 1;
                return;
            }
        }
    });
    
  });
