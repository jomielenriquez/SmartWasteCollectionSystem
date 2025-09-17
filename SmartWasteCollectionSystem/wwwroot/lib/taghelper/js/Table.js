class MyTable {
    constructor(tableId, route) {
        this.tableId = tableId;
        this.route = route;
        this.initDropdown();
        this.initCheckbox();
    }

    showTableName() {
        alert(this.tableId + " showtable");
    }

    initDropdown() {
        var route = this.route;
        $("#" + this.tableId + "Dropdown").on('change', function () {
            window.location = route + "/?PageSize=" + this.value;
        })
    }

    initCheckbox() {
        var tableId = this.tableId;
        $("#" + this.tableId + "HeaderCheckbox").on('change', function () {
            var isChecked = $(this).is(":checked");
            var box = $("#" + tableId + " ." + tableId + "Class");
            box.prop("checked", isChecked);
        })
    }

    initTableHeader() {
        var tableId = this.tableId;
    }
}