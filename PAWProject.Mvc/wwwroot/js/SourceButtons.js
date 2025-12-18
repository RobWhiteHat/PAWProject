document.addEventListener("click", async function (e) {

    // DESCARGAR
    if (e.target.closest(".btn-download")) {
        const btn = e.target.closest(".btn-download");
        const article = JSON.parse(btn.dataset.article);
        downloadArticle(article);
    }

    // GUARDAR EN DB
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

async function saveArticleToDb(article, button) {

    button.disabled = true;
    button.innerHTML = `<i class="bi bi-hourglass-split"></i>`;
    button.classList.remove("btn-outline-success");
    button.classList.add("btn-secondary");

    const payload = {
        url: article.url,
        name: article.title,
        description: article.description,
        componentType: "API",
        requiresSecret: false
    };

    try {
        const res = await fetch("/Home/SaveArticleToDb", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (!res.ok) throw new Error();

        const html = await res.text();

        document
            .getElementById("articlesContainer")
            .insertAdjacentHTML("afterbegin", html);

        button.innerHTML = `<i class="bi bi-check-lg"></i>`;
        button.classList.remove("btn-secondary");
        button.classList.add("btn-success");

        const cardBody = button.closest(".card-body");
        if (cardBody && !cardBody.querySelector(".badge-saved")) {
            cardBody.insertAdjacentHTML(
                "afterbegin",
                `<span class="badge bg-success mb-2 badge-saved">Guardado</span>`
            );
        }      
        await refreshArticles()

    } catch (err) {
        console.error(err);

        button.disabled = false;
        button.innerHTML = `<i class="bi bi-bookmark-plus"></i>`;
        button.classList.remove("btn-secondary");
        button.classList.add("btn-outline-success");
    }
}

function downloadArticle(article) {

    const filtered = {
        url: article.url ?? "",
        name: article.title ?? "",
        description: article.description ?? "",
        componentType: article.componentType ?? "SpaceApi",
        requiresSecret: 0
    };

    const blob = new Blob([JSON.stringify(filtered, null, 4)], {
        type: "application/json"
    });

    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");

    a.href = url;
    a.download = `${filtered.name.replace(/[^a-z0-9]/gi, '_').toLowerCase()}.json`;

    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);

    URL.revokeObjectURL(url);
}