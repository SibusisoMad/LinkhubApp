(function () {
    'use strict';

    function navigateTo(url) {
        if (!url) return;
        $(location).attr('href', url);
    }

    function hasVisibleAlert($el) {
        if (!$el || $el.length === 0) return false;
        if ($el.hasClass('d-none')) return false;
        return ($el.text() || '').trim().length > 0;
    }

    function initClientEdit() {
        var $page = $('#clientEditPage');
        if ($page.length === 0) return;

        var clientId = parseInt($page.data('client-id'), 10) || 0;
        var listUrl = ($page.data('list-url') || '').toString() || '/clients/list';

        var $contactsTab = $('#contacts-tab');
        var $linkSuccessAlert = $('#linkSuccessAlert');
        var $unlinkSuccessAlert = $('#unlinkSuccessAlert');

        var shouldActivateContactsTab =
            hasVisibleAlert($linkSuccessAlert) ||
            hasVisibleAlert($unlinkSuccessAlert) ||
            $('#linkErrorModal').length > 0;

        if ($contactsTab.length && shouldActivateContactsTab) {
            new bootstrap.Tab($contactsTab[0]).show();
        }

        if (hasVisibleAlert($linkSuccessAlert)) {
            window.setTimeout(function () {
                $linkSuccessAlert.addClass('d-none').text('');
            }, 2000);
        }

        if (hasVisibleAlert($unlinkSuccessAlert)) {
            window.setTimeout(function () {
                $unlinkSuccessAlert.addClass('d-none').text('');
            }, 2000);
        }

        var $linkErrorModal = $('#linkErrorModal');
        if ($linkErrorModal.length) {
            var m = new bootstrap.Modal($linkErrorModal[0]);
            m.show();
            window.setTimeout(function () { m.hide(); }, 1000);
        }

        // Contact unlink modal field binding
        $('#confirmUnlinkContactModal').on('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            if (!button) return;

            var $btn = $(button);
            $('#unlinkContactName').text($btn.data('contact-name') || '');
            $('#unlinkClientId').val($btn.data('client-id') || '');
            $('#unlinkContactId').val($btn.data('contact-id') || '');
        });

        // Save (client details) success modal
        $('#clientEditForm').on('ajax:success', function (e, response) {
            if (!response || response.success !== true) return;

            $('#clientUpdatedModalBody').text('Client updated.');

            var modalEl = $('#clientUpdatedModal').get(0);
            if (!modalEl) {
                navigateTo(listUrl);
                return;
            }

            var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
            modal.show();

            window.setTimeout(function () {
                navigateTo(listUrl);
            }, 1500);
        });

        // Link/unlink contacts async updates + search dropdown
        var pageSize = parseInt($page.data('page-size'), 10) || 5;
        var $input = $('#contactSearch');
        var $results = $('#contactSearchResults');
        var $status = $('#contactSearchStatus');
        var $hiddenId = $('#linkContactId');
        var $submit = $('#linkContactSubmit');
        var $linkForm = $submit.closest('form');

        var searchUrl = ($input.data('search-url') || '').toString();
        var debounceTimer = null;
        var pendingRequest = null;
        var skip = 0;
        var activeQuery = '';
        var selectedContact = null;

        function showStatus(message) {
            $status.text(message || '');
        }

        function hideResults() {
            $results.hide().empty();
        }

        function resetSelection() {
            $hiddenId.val('');
            $submit.prop('disabled', true);
            selectedContact = null;
        }

        function showLinkSuccessAlert(message) {
            $linkSuccessAlert.text(message || 'Contact linked successfully.').removeClass('d-none');
            window.setTimeout(function () {
                $linkSuccessAlert.addClass('d-none').text('');
            }, 2000);
        }

        function showUnlinkSuccessAlert(message) {
            $unlinkSuccessAlert.text(message || 'Contact unlinked successfully.').removeClass('d-none');
            window.setTimeout(function () {
                $unlinkSuccessAlert.addClass('d-none').text('');
            }, 2000);
        }

        function renderItems(items, hasMore) {
            if (!items || !items.length) {
                $results.html('<div class="list-group-item">No matches</div>').show();
                return;
            }

            items.forEach(function (item) {
                var label = (item.fullName || '') + (item.email ? ' (' + item.email + ')' : '');
                var $btn = $('<button type="button" class="list-group-item list-group-item-action"></button>');
                $btn.text(label);
                $btn.on('click', function () {
                    $input.val(label);
                    $hiddenId.val(item.contactId);
                    $submit.prop('disabled', false);
                    selectedContact = {
                        contactId: item.contactId,
                        fullName: item.fullName || '',
                        email: item.email || ''
                    };
                    hideResults();
                    showStatus('');
                });
                $results.append($btn);
            });

            if (hasMore) {
                var $more = $('<button type="button" class="list-group-item list-group-item-action text-center fw-semibold"></button>');
                $more.text('Load more');
                $more.on('click', function () {
                    fetchPage(activeQuery, skip + pageSize, true);
                });
                $results.append($more);
            }

            $results.show();
        }

        function fetchPage(query, newSkip, append) {
            if (!searchUrl) {
                showStatus('Search is not configured.');
                return;
            }

            if (pendingRequest) {
                pendingRequest.abort();
                pendingRequest = null;
            }

            activeQuery = query;
            skip = newSkip;
            showStatus('Searching...');

            pendingRequest = $.ajax({
                url: searchUrl,
                type: 'GET',
                data: { clientId: clientId, query: query, skip: skip, take: pageSize },
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            })
                .done(function (response) {
                    var items = response && response.items ? response.items : [];
                    var hasMore = response && response.hasMore === true;

                    if (!append) {
                        $results.empty();
                    }

                    renderItems(items, hasMore);
                    showStatus('');
                })
                .fail(function () {
                    hideResults();
                    showStatus('Search failed.');
                })
                .always(function () {
                    pendingRequest = null;
                });
        }

        $input.on('input', function () {
            var query = ($input.val() || '').trim();
            resetSelection();

            if (debounceTimer) {
                window.clearTimeout(debounceTimer);
                debounceTimer = null;
            }

            if (query.length < 3) {
                hideResults();
                showStatus(query.length === 0 ? '' : 'searching......');
                return;
            }

            debounceTimer = window.setTimeout(function () {
                fetchPage(query, 0, false);
            }, 250);
        });

        $linkForm.on('submit', function (e) {
            if (!$hiddenId.val()) {
                e.preventDefault();
                showStatus('Select a contact from the results.');
            }
        });

        $linkForm.on('ajax:success', function (e, response) {
            if (!response || response.success !== true) return;
            if (!selectedContact) return;

            var $container = $('#linkedContactsContainer');
            var $tbody = $('#linkedContactsBody');

            if ($tbody.length === 0) {
                $('#noLinkedContactsAlert').remove();
                $container.html(
                    '<table class="table table-bordered table-striped" id="linkedContactsTable">' +
                    '  <thead>' +
                    '    <tr>' +
                    '      <th class="text-start">Contact Full Name</th>' +
                    '      <th class="text-start">Contact Email</th>' +
                    '      <th class="text-start"></th>' +
                    '    </tr>' +
                    '  </thead>' +
                    '  <tbody id="linkedContactsBody"></tbody>' +
                    '</table>'
                );
                $tbody = $('#linkedContactsBody');
            }

            var safeFullName = $('<div>').text(selectedContact.fullName).html();
            var safeEmail = $('<div>').text(selectedContact.email).html();

            var rowHtml =
                '<tr data-contact-id="' + selectedContact.contactId + '">' +
                '  <td class="text-start">' + safeFullName + '</td>' +
                '  <td class="text-start">' + safeEmail + '</td>' +
                '  <td class="text-start">' +
                '    <button type="button" class="btn btn-link text-danger p-0"' +
                '      data-bs-toggle="modal" data-bs-target="#confirmUnlinkContactModal"' +
                '      data-client-id="' + clientId + '"' +
                '      data-contact-id="' + selectedContact.contactId + '"' +
                '      data-contact-name="' + safeFullName + '">' +
                '      Unlink' +
                '    </button>' +
                '  </td>' +
                '</tr>';

            $tbody.append(rowHtml);

            showLinkSuccessAlert(response.message || 'Contact linked successfully.');
            showStatus('');

            $input.val('');
            hideResults();
            resetSelection();
        });

        $('#unlinkContactForm').on('ajax:success', function (e, response) {
            if (!response || response.success !== true) return;

            var contactIdToRemove = ($('#unlinkContactId').val() || '').toString();
            if (!contactIdToRemove) return;

            $('#linkedContactsBody').find('tr[data-contact-id="' + contactIdToRemove + '"]').remove();

            var $remainingRows = $('#linkedContactsBody').find('tr');
            if ($remainingRows.length === 0) {
                $('#linkedContactsTable').remove();
                if ($('#noLinkedContactsAlert').length === 0) {
                    $('#linkedContactsContainer').html('<div class="alert alert-info mt-2" id="noLinkedContactsAlert">No contacts found.</div>');
                }
            }

            var modalEl = $('#confirmUnlinkContactModal').get(0);
            if (modalEl) {
                var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                modal.hide();
            }

            showUnlinkSuccessAlert(response.message || 'Contact unlinked successfully.');
        });

        $(document).on('click', function (e) {
            var $target = $(e.target);
            if ($target.closest('#contactSearchResults').length === 0 && $target.closest('#contactSearch').length === 0) {
                hideResults();
            }
        });

        $input.on('keydown', function (e) {
            if (e.key === 'Backspace' || e.key === 'Delete') {
                resetSelection();
            }
        });
    }

    $(initClientEdit);
})();
