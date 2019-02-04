// Write your JavaScript code.
$(document).ready(function() {
    $('li.active').removeClass('active');
    $('a[href="' + location.pathname + '"]').closest('li').addClass('active'); 
    $('li.nav-item').hover(
        function(){$(this).addClass('hover')},
        function(){$(this).removeClass('hover')}
    );
  });