// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("change", function (e) {

    const fileInput = e.target;

    if (fileInput && fileInput.name === "jsonFile") {

        const modal = fileInput.closest(".modal");
        const submitBtn = modal.querySelector(".btn-carga-json");

        const file = fileInput.files[0];

        if (!file) {
            submitBtn.disabled = true;
            return;
        }

        const isJson =
            file.type === "application/json" ||
            file.name.toLowerCase().endsWith(".json");

        submitBtn.disabled = !isJson;
    }
});
