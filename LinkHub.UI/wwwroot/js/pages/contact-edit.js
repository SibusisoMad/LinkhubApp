
(function () {
    'use strict';

    function navigateTo(url) {
        if (!url) return;
        $(location).attr('href', url);
    }

    function initContactEdit() {
        var $page = $('#contactEditPage');
        if ($page.length === 0) return;

        var contactId = parseInt($page.data('contact-id'), 10) || 0;
        var listUrl = ($page.data('list-url') || '').toString() || '/contacts/list';
        var pageSize = parseInt($page.data('page-size'), 10) || 5;

        var $clientsSuccessAlert = $('#clientsSuccessAlert');
        if ($clientsSuccessAlert.length) {
            var $clientsTab = $('#clients-tab');
            if ($clientsTab.length) {
                new bootstrap.Tab($clientsTab[0]).show();
            }
            window.setTimeout(function () {
                $clientsSuccessAlert.fadeOut(200, function () { $clientsSuccessAlert.remove(); });
            }, 2000);
        }

        // Unlink modal field binding
        $('#confirmUnlinkClientModal').on('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            if (!button) return;

            var $btn = $(button);
            $('#unlinkClientName').text($btn.data('client-name') || '');
            $('#unlinkContactId').val($btn.data('contact-id') || '');
            $('#unlinkClientId').val($btn.data('client-id') || '');
        });

        function showClientsSuccess(message) {
            var $existing = $('#clientsSuccessAlert');
            if ($existing.length === 0) {
                $('#clients').prepend('<div class="alert alert-success mt-2" id="clientsSuccessAlert"></div>');
                $existing = $('#clientsSuccessAlert');
            }

            $existing.stop(true, true).show().text(message || 'Success.');
            window.setTimeout(function () {
                $existing.fadeOut(200, function () { $existing.remove(); });
            }, 2000);
        }

        // Save (contact details) success modal
        $('#contactUpdateForm').on('ajax:success', function (e, response) {
            if (!response || response.success !== true) return;

            $('#contactUpdatedModalBody').text('Contact updated.');

            var modalEl = $('#contactUpdatedModal').get(0);
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

        // Search + link/unlink async updates
        var $input = $('#clientSearch');
        var $results = $('#clientSearchResults');
        var $status = $('#clientSearchStatus');
        var $hiddenId = $('#linkClientId');
        var $submit = $('#linkClientSubmit');
        var $linkForm = $('#linkClientForm');
        var searchUrl = ($input.data('search-url') || '').toString();

        var debounceTimer = null;
        var pendingRequest = null;
        var skip = 0;
        var activeQuery = '';
        var selectedClient = null;

        function showStatus(message) {
            $status.text(message || '');
        }

        function hideResults() {
            $results.hide().empty();
        }

        function resetSelection() {
            $hiddenId.val('');
            $submit.prop('disabled', true);
            selectedClient = null;
        }

        function renderItems(items, hasMore) {
            if (!items || !items.length) {
                $results.html('<div class="list-group-item">No matches</div>').show();
                return;
            }

            items.forEach(function (item) {
                var label = (item.clientName || '') + (item.clientCode ? ' (' + item.clientCode + ')' : '');
                var $btn = $('<button type="button" class="list-group-item list-group-item-action"></button>');
                $btn.text(label);
                $btn.on('click', function () {
                    $input.val(label);
                    $hiddenId.val(item.clientId);
                    $submit.prop('disabled', false);
                    selectedClient = {
                        clientId: item.clientId,
                        clientName: item.clientName || '',
                        clientCode: item.clientCode || ''
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
                data: { contactId: contactId, query: query, skip: skip, take: pageSize },
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
                showStatus(query.length === 0 ? '' : 'Type 3+ characters to search.');
                return;
            }

            debounceTimer = window.setTimeout(function () {
                fetchPage(query, 0, false);
            }, 250);
        });

        $linkForm.on('submit', function (e) {
            if (!$hiddenId.val()) {
                e.preventDefault();
                showStatus('Select a client from the results.');
            }
        });

        $linkForm.on('ajax:success', function (e, response) {
            if (!response || response.success !== true) return;
            if (!selectedClient) return;

            var $container = $('#linkedClientsContainer');
            var $tbody = $('#linkedClientsBody');

            if ($tbody.length === 0) {
                $('#noLinkedClientsAlert').remove();
                $container.html(
                    '<table class="table table-bordered table-striped" id="linkedClientsTable">' +
                    '  <thead>' +
                    '    <tr>' +
                    '      <th class="text-start">Client Name</th>' +
                    '      <th class="text-start">Client Code</th>' +
                    '      <th class="text-start"></th>' +
                    '    </tr>' +
                    '  </thead>' +
                    '  <tbody id="linkedClientsBody"></tbody>' +
                    '</table>'
                );
                $tbody = $('#linkedClientsBody');
            }

            var safeName = $('<div>').text(selectedClient.clientName).html();
            var safeCode = $('<div>').text(selectedClient.clientCode).html();

            var rowHtml =
                '<tr data-client-id="' + selectedClient.clientId + '">' +
                '  <td class="text-start">' + safeName + '</td>' +
                '  <td class="text-start">' + safeCode + '</td>' +
                '  <td class="text-start">' +
                '    <button type="button" class="btn btn-link text-danger p-0"' +
                '      data-bs-toggle="modal" data-bs-target="#confirmUnlinkClientModal"' +
                '      data-contact-id="' + contactId + '"' +
                '      data-client-id="' + selectedClient.clientId + '"' +
                '      data-client-name="' + safeName + '">' +
                '      Unlink' +
                '    </button>' +
                '  </td>' +
                '</tr>';

            $tbody.append(rowHtml);
            showClientsSuccess('Client linked successfully.');
            showStatus('');

            $input.val('');
            hideResults();
            resetSelection();
        });

        $('#unlinkClientForm').on('ajax:success', function (e, response) {
            if (!response || response.success !== true) return;

            var clientIdToRemove = ($('#unlinkClientId').val() || '').toString();
            if (!clientIdToRemove) return;

            $('#linkedClientsBody').find('tr[data-client-id="' + clientIdToRemove + '"]').remove();

            var $remainingRows = $('#linkedClientsBody').find('tr');
            if ($remainingRows.length === 0) {
                $('#linkedClientsTable').remove();
                if ($('#noLinkedClientsAlert').length === 0) {
                    $('#linkedClientsContainer').html('<div class="alert alert-info mt-2" id="noLinkedClientsAlert">No clients found.</div>');
                }
            }

            var modalEl = $('#confirmUnlinkClientModal').get(0);
            if (modalEl) {
                var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                modal.hide();
            }

            showClientsSuccess('Client unlinked successfully.');
        });

        $(document).on('click', function (e) {
            var $target = $(e.target);
            if ($target.closest('#clientSearchResults').length === 0 && $target.closest('#clientSearch').length === 0) {
                hideResults();
            }
        });

        $input.on('keydown', function (e) {
            if (e.key === 'Backspace' || e.key === 'Delete') {
                resetSelection();
            }
        });
    }

    $(initContactEdit);
})();
