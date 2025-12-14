let offset = 0;
const limit = 9;
let loading = false;

async function loadArticles() {
    if (loading) return;
    loading = true;

    document.getElementById("loader").style.display = "block";

    const res = await fetch(`/Home/LoadArticles?limit=${limit}&offset=${offset}`);
    const data = await res.json();

    const container = document.getElementById("articlesContainer");

    data.results.forEach(a => {
        container.innerHTML += `
        <div class="col">
            <div class="card h-100 shadow-sm border-0">
                <img src="${a.image_url}" class="card-img-top" style="height:200px; object-fit:cover;">
                <div class="card-body d-flex flex-column">
                    <h5 class="card-title text-primary">${a.title}</h5>
                    <p class="card-text small text-muted">
                        ${a.summary.substring(0, 120)}...
                    </p>

                    <div class="d-flex gap-2 mt-auto">
                       <button class="btn btn-success btn-sm" onclick='saveArticleCustom(${JSON.stringify(a)})'>
                            Descargar ⭐
                        </button>

                        <a href="${a.url}" class="btn btn-outline-primary btn-sm" target="_blank">
                            Leer más →
                        </a>

                        <button class="btn btn-outline-success btn-sm"
                                onclick='saveArticleToDb(${JSON.stringify(a)}, this)'>
                            Añadir a favorito
                        </button>
                    </div>
                </div>
            </div>
        </div>`;
    });

    offset += limit;
    loading = false;
    document.getElementById("loader").style.display = "none";
}

async function loadDbArticles() {
    const res = await fetch(`/Home/LoadDbArticles`);
    const data = await res.json();

    const container = document.getElementById("articlesContainer");

    data.forEach(a => {
        container.insertAdjacentHTML("afterbegin", `
        <div class="col db-article">
            <div class="card h-100 border-success shadow-sm">
                <div class="card-body d-flex flex-column">
                    <span class="badge bg-success mb-2">Base de datos</span>

                    <h5 class="card-title text-success">${a.name}</h5>

                    <p class="card-text small text-muted">
                        ${a.description?.substring(0, 120) ?? ""}...
                    </p>

                    <div class="mt-auto">
                        <button class="btn btn-success btn-sm" onclick='saveDbArticle(${JSON.stringify(a)})'>
                            Descargar ⭐
                        </button>
                        <a href="${a.url}" class="btn btn-outline-success btn-sm" target="_blank">
                            Leer más →
                        </a>
                    </div>
                </div>
            </div>
        </div>`);
    });
}

async function saveArticleToDb(article, button) {

    if (button.disabled) return;

    button.disabled = true;
    button.innerText = "Guardando...";
    button.classList.remove("btn-outline-success");
    button.classList.add("btn-secondary");

    const payload = {
        url: article.url,
        name: article.title,
        description: article.summary,
        componentType: "API",
        requiresSecret: false
    };

    try {
        const res = await fetch("/Home/SaveArticleToDb", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        });

        if (!res.ok) {
            throw new Error(await res.text());
        }

        button.innerText = "Añadido";
        button.classList.remove("btn-secondary");
        button.classList.add("btn-success");

        await refreshDbArticles();
    } catch (err) {
        console.error(err);

        button.disabled = false;
        button.innerText = "Añadir a favorito";
        button.classList.remove("btn-secondary");
        button.classList.add("btn-outline-success");

        alert("No se pudo guardar el artículo");
    }
}

async function refreshDbArticles() {
    document.querySelectorAll(".db-article").forEach(e => e.remove());
    await loadDbArticles();
    loadArticles();
}

// Primera carga
(async () => {
    await loadDbArticles();
    loadArticles();
})();

// Scroll infinito
window.addEventListener("scroll", () => {
    if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 200) {
        loadArticles();
    }
});

function saveArticleCustom(article) {

    const filtered = {
        url: article.url ?? "",
        name: article.title ?? "",
        description: article.summary ?? "",
        componentType: "DB",
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

function saveDbArticle(article) {

    const filtered = {
        url: article.url ?? "",
        name: article.name ?? "",
        description: article.description ?? "",
        componentType: article.componentType ?? "DB",
        requiresSecret: article.requiresSecret ?? false
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


