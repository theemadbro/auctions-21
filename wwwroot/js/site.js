﻿// Write your JavaScript code.
$(document).ready(function() {
    console.log("heyo, planet")
    $('li.active').removeClass('active');
    $('a[href="' + location.pathname + '"]').closest('li').addClass('active'); 
    $('li.nav-item').hover(
        function(){$(this).addClass('hover')},
        function(){$(this).removeClass('hover')}
    );
  });