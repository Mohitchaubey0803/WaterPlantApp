// AquaPure - site.js
// Auto-dismiss alerts after 4 seconds
document.addEventListener('DOMContentLoaded', function () {
    var alert = document.querySelector('.alert-success');
    if (alert) {
        setTimeout(function () {
            alert.style.transition = 'opacity 0.5s';
            alert.style.opacity = '0';
            setTimeout(function () { alert.remove(); }, 500);
        }, 4000);
    }
});
