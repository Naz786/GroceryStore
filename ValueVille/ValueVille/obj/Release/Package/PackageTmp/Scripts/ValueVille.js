$(function () {
    var $box = $('.special-products');
    $box.slice(12).hide(); //from showItem toLength

    $('.see-all').on('click', function () {

        $box.slice(0).show();
        $('.see-all').hide();
        $('.see-less').show();
    });

    $('.see-less').on('click', function () {
        $box.slice(12).hide();
        $('.see-less').hide();
        $('.see-all').show();
    });

});

$('#responsive').lightSlider({
    item: 5,
    loop: false,
    slideMove: 2,
    easing: 'cubic-bezier(0.25, 0, 0.25, 1)',
    speed: 600,
    responsive: [
        {
            breakpoint: 800,
            settings: {
                item: 3,
                slideMove: 1,
                slideMargin: 6,
            }
        },
        {
            breakpoint: 480,
            settings: {
                item: 2,
                slideMove: 1
            }
        }
    ]
});