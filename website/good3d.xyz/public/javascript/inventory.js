function getInventory(id, categoryid, page) {
    const $categories = $('.category');

    $categories.removeClass('AssetsMenuItem_Selected AssetsMenuButton_Selected')
        .addClass('AssetsMenuItem AssetsMenuButton')
        .find('a')
        .removeClass('AssetsMenuButton_Selected')
        .addClass('AssetsMenuButton');

    const $selected = $categories.filter(`[data-categoryid="${categoryid}"]`);
    $selected.removeClass('AssetsMenuItem AssetsMenuButton')
        .addClass('AssetsMenuItem_Selected AssetsMenuButton_Selected')
        .find('a')
        .removeClass('AssetsMenuButton')
        .addClass('AssetsMenuButton_Selected');

    $.ajax({
        type: 'GET',
        url: '/inventory/get',
        data: { userid: id, categoryid: categoryid, page: page }
    })
    .done(response => $('#AssetsContent').html(response))
    .fail(() => $('#AssetsContent').html('<p>Failed to load inventory.</p>'));
}

$(document).on('click', '.category', function(e) {
    e.preventDefault();
    const $this = $(this);
    getInventory($this.data('id'), $this.data('categoryid'), 1);
});

$(document).on('click', '.pager-inventory', function(e) {
    e.preventDefault();
    const $this = $(this);
    getInventory($this.data('id'), $this.data('categoryid'), $this.data('page'));
});