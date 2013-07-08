function clearText(button) {
    var textfield = $(button.parentElement.children).filter("input[type=text]");
    textfield.val('');    
}