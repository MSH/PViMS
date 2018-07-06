function ValidateDeletionReason(reasonTextBox, validationSummary, reasonLabel) {
    var reasonTextBoxValue = $.trim($(reasonTextBox).val());
    if (reasonTextBoxValue == "") {
        $(validationSummary).removeClass('hidden');
        $(validationSummary).addClass('show');
        $(reasonLabel).addClass('state-error');
        return false;
    }
    $(reasonLabel).removeClass('state-error');
    $(validationSummary).removeClass('show');
    $(validationSummary).addClass('hidden');
    return true;
}