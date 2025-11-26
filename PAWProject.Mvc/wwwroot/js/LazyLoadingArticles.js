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
                        <button class="btn btn-success btn-sm" onclick='saveArticle(${JSON.stringify(a)})'>
                            Descargar ⭐
                        </button>

                        <a href="${a.url}" class="btn btn-outline-primary btn-sm" target="_blank">
                            Leer más →
                        </a>
                    </div>
                </div>
            </div>
        </div>`;
    });

    offset += limit;
    loading = false;
    document.getElementById("loader").style.display = "none";
}

// Primera carga
loadArticles();

// Detectar scroll al final
window.addEventListener("scroll", () => {
    if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 200) {
        loadArticles();
    }
});
function saveArticle(article) {

    const jsonString = JSON.stringify(article, null, 2);
    const blob = new Blob([jsonString], { type: "application/json" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");

    a.href = url;
    a.download = `${article.title?.replace(/[^a-z0-9]/gi, '_').toLowerCase() || "articulo"}.json`;

    document.body.appendChild(a);
    a.click();

    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}