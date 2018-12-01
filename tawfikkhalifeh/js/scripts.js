jQuery(function ($) {

    'use strict';

    // --------------------------------------------------------------------
    // PreLoader
    // --------------------------------------------------------------------

    (function () {
        $('#preloader').delay(200).fadeOut('slow');
        $('#contactForm').submit(function (e) {
            e.preventDefault();
            $('[type=submit]', this).prop('disabled', true);
            var $message = $('#message');
            var $spinner = $('#spinner');
            $message.hide();
            $spinner.css('display', 'inline-block');

            var form = this;
            $.ajax(form.action,
                {
                    method: 'post',
                    data: $(form).serialize(),
                    dataType: 'json'
                }).done(function (data) {
                    $(':input', form).val('');
                    $message.text($message.data('success')).show();
                    console.log(data);
                }).error(function () {
                    $message.text($message.data('error')).show();;
                }).always(function () {
                    $('[type=submit]', form).prop('disabled', false);
                    $spinner.css('display', 'none');
                });
        });

        // send event to analytics
        $('.track').click(function () {
            if (window.gtag) {
                var action = this.getAttribute('data-action');
                var category = this.getAttribute('data-category');
                console.log('action: ' + action + ', category: ' + category);

                window.gtag('event', action,
                    {
                        'event_category': category
                    });
            }
        });
    }());

}); // JQuery end