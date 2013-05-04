/* Initialize Ace Editor */


var editor = ace.edit("editor");
//editor.setTheme("ace/theme/clouds_midnight");
editor.setTheme("ace/theme/clouds");
editor.getSession().setMode("ace/mode/html");
document.getElementById('editor').style.fontSize = '14px';
editor.getSession().setUseWrapMode(true);


/* Write content of the ace editor to an hidden input field */
$(".entry-create-edit-submit").click(function() {
    $("#entry-create-edit #Content").val(editor.getValue());
});