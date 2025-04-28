// Creating the Code Mirror Instance
const editor = CodeMirror.fromTextArea(document.getElementById("editor"), {
    lineNumbers: true,
    theme: "material",
    lineWrapping: true,
    readOnly: true,
    className: "readOnly",
    extraKeys: { "Alt-F": "findPersistent" }
});

editor.setSize("100%", "100%");

var toggleForm = function (enabled = false) {
    if (enabled) {
        $('.toggleForm').removeAttr("disabled");
        return;
    }

    $('.toggleForm').prop("disabled", true);
}


$('#download').click(function () {
    $('#download-logs').modal('show');
    $('#download-logs-container').html('No log files available to download.');
    // Need to fetch here...
});


$('#form-fetch-logs').submit(function (e) {
    e.preventDefault();
    //console.error('submit');

    const urlParams = new URLSearchParams(window.location.search);

    const find = $('#log-find').val();
    const lines = $('#num-lines').val();
    const skip = $('#skip-lines').val();

    if (find.length > 0) {
        urlParams.set('find', find);
    } else {
        urlParams.delete('find');
    }

    if (lines.length > 0) {
        urlParams.set('lines', lines);
    } else {
        urlParams.delete('lines');
    }

    if (skip.length > 0) {
        urlParams.set('skip', skip);
    } else {
        urlParams.delete('skip');
    }

    // Set the current url params without triggering a page reload...
    window.history.replaceState({}, dashboardTitle, window.location.href.split('?')[0] + '?' + urlParams);

    const formData = Object.fromEntries(new FormData(this));

    fetchLogs(formData);
});

// Handle changes to the number input
const numLinesInput = document.getElementById("num-lines");
const form = document.getElementById('my-form');
const fetchBtn = document.getElementById("fetch-btn");

const fetchLogs = (formData) => {
    const loggingAjaxCallback = getDashboardUrl('Modules.Dashboard.LocalLogging.DashboardLoggingModule/callback/local-logging-callback');

    toggleForm();

    //Displaying the loading button
    
    fetch(loggingAjaxCallback, {
        method: 'POST',
        mode: 'same-origin',
        credentials: 'same-origin',
        body: JSON.stringify(formData)
    }).then(function (response) {
        if (response.status === 401) {
            console.error("Unauthorized");
            toggleForm(true);
            return;
        } else {
            return response.text();
        }
    }).then(function (data) {
        toggleForm(true);
        editor.getDoc().setValue(data);
    });
}

$(document).ready(function () {
    if (window.location.pathname.endsWith('/local-logging/read-logs')) {
        
        const urlParams = new URLSearchParams(window.location.search);

        const find = urlParams.get('find');
        const lines = urlParams.get('lines');
        const skip = urlParams.get('skip');

        if (find) {
            $('#log-find').val(find);
        }

        if (lines) {
            $('#num-lines').val(lines);
        }

        if (skip) {
            $('#skip-lines').val(skip);
        }

        const formData = Object.fromEntries(new FormData($('#form-fetch-logs')[0]));

        fetchLogs(formData);
    }
});

