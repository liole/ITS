/// <reference path="~/Scripts/jquery-1.9.1.js" />
/// <reference path="~/Scripts/jquery-1.9.1.intellisense.js" />
function markAnswer(order, letter) {
    $('#question_' + order + ' .A .abcd_letter').removeClass('marked');
    $('#question_' + order + ' .B .abcd_letter').removeClass('marked');
    $('#question_' + order + ' .C .abcd_letter').removeClass('marked');
    $('#question_' + order + ' .D .abcd_letter').removeClass('marked');

    $('#question_' + order + ' .' + letter + ' .abcd_letter').addClass('marked');
    $('#question_' + order + ' .answer').val(letter);
    markCompleted(order);
}

function markText(order) {
    if ($('#question_' + order + ' .answer').val() != '') {
        markCompleted(order);
    } else {
        markNotCompleted(order);
    }
}

function markCompleted(order) {
    $('#question_' + order).addClass('completed');
}
function markNotCompleted(order) {
    $('#question_' + order).removeClass('completed');
}