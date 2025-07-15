// This script handles the download of the app installer and MSIX files as blobs, so no direct file download links are used (some browsers just open the file instead of downloading it, or append .zip)

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('downloadInstallerLink').addEventListener('click', function (e) {
        e.preventDefault();
        var url = 'https://mstackappdistribution.z6.web.core.windows.net/AzureAppRegistrationsManager/AzureAppRegistrationsManager_x64.appinstaller';
        fetch(url)
            .then(response => response.blob())
            .then(blob => {
                var link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = 'AzureAppRegistrationsManager_x64.appinstaller';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            });
    });

    document.getElementById('downloadMsixLink').addEventListener('click', function (e) {
        e.preventDefault();
        var url = 'https://mstackappdistribution.z6.web.core.windows.net/AzureAppRegistrationsManager/AzureAppRegistrationsManager_1.0.0.0_x64_Test/AzureAppRegistrationsManager_1.0.0.0_x64.msix';
        fetch(url)
            .then(response => response.blob())
            .then(blob => {
                var link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = 'AzureAppRegistrationsManager_1.0.0.0_x64.msix';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            });
    });
});