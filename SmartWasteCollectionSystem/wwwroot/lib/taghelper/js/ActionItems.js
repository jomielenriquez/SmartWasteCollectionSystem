class SysCoreActionItems {
    constructor(ActionItemId, tableName, controller, deleteAction) {
        this.ActionItemId = ActionItemId;
        this.tableName = tableName;
        this.controller = controller;
        this.deleteAction = deleteAction;
        this.InitDelete();
    }

    InitDelete() {
        console.log("init");
        var thisTableID = this.tableName;
        var thisController = this.controller;
        var thisDeleteAction = this.deleteAction;
        $("#" + this.ActionItemId + "Delete").on('click', function () {
            var selected = [];
            $('#' + thisTableID + ' tr td input[type="checkbox"]:checked').each(function (index, element) {
                // Get the value of each input field and push it into the array
                selected.push($(element).val());
            });

            $.ajax({
                url: '/' + thisController + '/' + thisDeleteAction + '', // URL to send the request
                method: 'POST', // HTTP method
                contentType: 'application/json', // Content type
                //dataType: 'json', // Type of data expected back from the server
                data: JSON.stringify(selected),
                success: function (data) {
                    alert(data + " record(s) deleted.");
                    console.log(data);
                    location.reload();
                },
                error: function (xhr, status, error) {
                    // Callback function to handle errors
                    console.log('Error:', error);
                }
            });
        });
    }
}