document.getElementById('add-item').addEventListener('click', function () {
    var body = document.getElementById('request-items');
    var row = body.rows[0].cloneNode(true);
    row.querySelector('select').value = '';
    row.querySelector('input').value = '1';
    body.appendChild(row);
});
document.getElementById('request-items').addEventListener('click', function (event) {
    if (event.target.classList.contains('remove-item') && this.rows.length > 1) {
        event.target.closest('tr').remove();
    }
});