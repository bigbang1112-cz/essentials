function getHtml() {
    return '<div class="lds-ellipsis"><div></div><div></div><div></div><div></div></div>';
}

function replaceWithLoading() {
    var element = event.target;
    var content = element.children[0];
    var loading = element.children[1];
    content.classList.add('fade-out');
    loading.classList.add('fade-in');
}

function replaceWithPrevious(id) {
    var element = document.getElementById(id);
    var content = element.children[0];
    var loading = element.children[1];
    content.classList.remove('fade-out');
    loading.classList.remove('fade-in');
}