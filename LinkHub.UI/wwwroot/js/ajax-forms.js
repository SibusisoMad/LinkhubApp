
(function () {
    'use strict';

    function showError($form, message) {
        var $error = $form.find('[data-ajax-error]').first();
        if ($error.length) {
            $error.text(message || 'Request failed.').removeClass('d-none');
        } else {
            window.alert(message || 'Request failed.');
        }
    }

    function clearErrors($form) {
        $form.find('[data-ajax-error]').addClass('d-none').text('');
        $form.find('[data-valmsg-for]').each(function () {
            $(this).text('');
        });
    }

    function applyFieldErrors($form, errors) {
        if (!errors) return;

        Object.keys(errors).forEach(function (key) {
            var messages = errors[key];
            if (!messages || !messages.length) return;

            var $msg = $form.find('[data-valmsg-for="' + key + '"]').first();
            if ($msg.length) {
                $msg.text(messages[0]);
            }
        });
    }

    function handleAjaxFormSubmit(e) {
        e.preventDefault();

        var $form = $(this);
        clearErrors($form);

        var url = $form.attr('action');
        var method = ($form.attr('method') || 'POST').toUpperCase();

        var $submitButtons = $form.find(':submit');
        $submitButtons.prop('disabled', true);

        $.ajax({
            url: url,
            type: method,
            data: $form.serialize(),
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
            .done(function (response) {
                $form.trigger('ajax:success', [response]);
                var redirectOpt = ($form.attr('data-ajax-redirect') || '').toString().toLowerCase();
                var allowAutoRedirect = redirectOpt !== 'false';

                if (allowAutoRedirect && response && response.redirectUrl) {
                    $(location).attr('href', response.redirectUrl);
                    return;
                }

                if (response && response.success) {
                    return;
                }

                showError($form, 'Unexpected response from server.');
            })
            .fail(function (xhr) {
                var json = xhr && xhr.responseJSON;

                $form.trigger('ajax:error', [xhr, json]);

                if (json && json.errors) {
                    applyFieldErrors($form, json.errors);
                    return;
                }

                if (json && json.error) {
                    showError($form, json.error);
                    return;
                }

                showError($form, 'Request failed.');
            })
            .always(function () {
                $form.trigger('ajax:complete');
                $submitButtons.prop('disabled', false);
            });
    }

    $(function () {
        $(document).on('submit', 'form[data-ajax="true"]', handleAjaxFormSubmit);
    });
})();
