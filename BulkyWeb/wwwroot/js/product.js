
var dataTable;
//`$(document).ready()`: This is a jQuery function that is used to execute code
//when the DOM(Document Object Model) is fully loaded and ready to be manipulated.
//`function loadDataTable() { ... }`: This is where the `loadDataTable()` function is defined.Inside this function, 
//a DataTable is initialized on the HTML element with the ID "tblData".
//The DataTable is being configured to make an AJAX request to '/admin/product/getall' to fetch data and display it in the table
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getall'},
        "columns" : [
            { data: 'title',"width" :"25%"},
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'author', "width": "15%" },
            { data: 'category.name', "width": "10%" },
            {
                data: "id",
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                                    <a href="/Admin/Product/Upsert?id=${data}" class="btn btn-primary mx-2">
                                        <i class="bi bi-pencil-square"></i> Edit
                                    </a>
                                     <a onClick=Delete('/admin/product/delete?id=${data}') class="btn btn-danger mx-2">
                                        <i class="bi bi-trash-fill"></i> Delete
                                    </a>
                    </div>`
                },
                "width":"25%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    //Refresh the table after deleting an item with datatable.ajax.reload();
                    // so to access that we need to declare a variable dataTable
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}
