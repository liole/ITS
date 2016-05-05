/// <reference path="~/Scripts/jquery-1.9.1.js" />
/// <reference path="~/Scripts/jquery-1.9.1.intellisense.js" />
/*
﻿$("input[type=password]").keyup(function () {
    var ucase = new RegExp("[A-Z]+");
    var lcase = new RegExp("[a-z]+");
    var num = new RegExp("[0-9]+");

    if ($("#password1").val().length >= 8) {
        $("#8char").removeClass("glyphicon-remove");
        $("#8char").addClass("glyphicon-ok");
        $("#8char").css("color", "#00A41E");
    } else {
        $("#8char").removeClass("glyphicon-ok");
        $("#8char").addClass("glyphicon-remove");
        $("#8char").css("color", "#FF0004");
    }

    if (ucase.test($("#password1").val())) {
        $("#ucase").removeClass("glyphicon-remove");
        $("#ucase").addClass("glyphicon-ok");
        $("#ucase").css("color", "#00A41E");
    } else {
        $("#ucase").removeClass("glyphicon-ok");
        $("#ucase").addClass("glyphicon-remove");
        $("#ucase").css("color", "#FF0004");
    }

    if (lcase.test($("#password1").val())) {
        $("#lcase").removeClass("glyphicon-remove");
        $("#lcase").addClass("glyphicon-ok");
        $("#lcase").css("color", "#00A41E");
    } else {
        $("#lcase").removeClass("glyphicon-ok");
        $("#lcase").addClass("glyphicon-remove");
        $("#lcase").css("color", "#FF0004");
    }

    if (num.test($("#password1").val())) {
        $("#num").removeClass("glyphicon-remove");
        $("#num").addClass("glyphicon-ok");
        $("#num").css("color", "#00A41E");
    } else {
        $("#num").removeClass("glyphicon-ok");
        $("#num").addClass("glyphicon-remove");
        $("#num").css("color", "#FF0004");
    }

    if ($("#password1").val() == $("#password2").val()) {
        $("#pwmatch").removeClass("glyphicon-remove");
        $("#pwmatch").addClass("glyphicon-ok");
        $("#pwmatch").css("color", "#00A41E");
    } else {
        $("#pwmatch").removeClass("glyphicon-ok");
        $("#pwmatch").addClass("glyphicon-remove");
        $("#pwmatch").css("color", "#FF0004");
    }
});
*/
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
