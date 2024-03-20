$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/category/getall' },
        "columns": [
            { data: 'name', "width": "35%" },
            { data: 'displayOrder', "width": "35%" },
            {
                data: "id",
                "render": function (data) {
                    return `
                            <div class="w-75 btn-group" role="group">
                                    <a href="/Admin/Category/Edit?id=${data}" class="btn btn-primary mx-2">
                                        <i class="bi bi-pencil-square"></i> Edit
                                    </a>
                                    
                    </div>`
                },
                "width": "15%"
            },
            {
                data: "id",
                "render": function (data) {
                    return `
                            <div class="w-75 btn-group" role="group">                                  
                                     <a href="/Admin/Category/Delete?id=${data}" class="btn btn-danger mx-2">
                                        <i class="bi bi-trash-fill"></i> Delete
                                    </a>
                    </div>`
                },
                "width": "15%"
            }
        ]
    });
}
