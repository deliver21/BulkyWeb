var dataTable;
//explainantion on product.js
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("customer")) {
        loadDataTable("customer");
    }
    else {
        if (url.includes("company")) {
            loadDataTable("company");
        }
        else {
            if (url.includes("employee")) {
                loadDataTable("employee");
            }
            else {
                if (url.includes("admin")) {
                    loadDataTable("admin");
                }
                else {
                    loadDataTable("all")
                }
            }
        }
    }
});

function loadDataTable(role) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/manager/getall?role='+ role },
        "columns": [
            { data: 'email', "width": "25%" },
            { data: 'name', "width": "20%" },
            { data: 'phoneNumber', "width": "25%" },
            { data: 'role', "width": "15%" },
            {
                data: "id",
                "render": function (data) {
                    return `<div class="w-75 btn-group text-center" role="group">
                                    <a href="/Admin/Manager/details?orderId=${data}" class="btn btn-primary mx-2">
                                        <i class="bi bi-pencil-square"></i>
                                    </a>                                     
                    </div>`
                },
                "width": "15%"
            }
        ]
    });
}