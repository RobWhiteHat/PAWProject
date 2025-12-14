async function saveArticleToDb(article, button) {

    const payload = {
        url: article.url,
        name: article.title,
        description: article.description,
        componentType: "API",
        requiresSecret: false
    };

    const res = await fetch("/Home/SaveArticleToDb", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
    });

    if (!res.ok) throw new Error("Error");

    button.innerHTML = `<i class="bi bi-check-lg"></i>`;
    button.classList.remove("btn-outline-success");
    button.classList.add("btn-success");
}

document.addEventListener("click", async function (e) {

    //DESCARGAR
    if (e.target.closest(".btn-download")) {
        const btn = e.target.closest(".btn-download");
        const article = JSON.parse(btn.dataset.article);

        saveArticleCustom(article);
    }

    //GUARDAR EN DB (ADMIN)
    if (e.target.closest(".btn-save-db")) {
        const btn = e.target.closest(".btn-save-db");
        const article = JSON.parse(btn.dataset.article);

        if (btn.disabled) return;

        btn.disabled = true;
        btn.innerHTML = `<i class="bi bi-hourglass-split"></i>`;

        try {
            await saveArticleToDb(article, btn);
        } catch {
            btn.disabled = false;
            btn.innerHTML = `<i class="bi bi-bookmark-plus"></i>`;
        }
    }
});


