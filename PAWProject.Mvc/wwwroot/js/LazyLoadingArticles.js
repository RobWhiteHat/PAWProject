let offset = 0;
const limit = 9;
let loading = false;

async function loadArticles() {
    if (loading) return;
    loading = true;

    document.getElementById("loader").style.display = "block";

    const res = await fetch(`/Home/LoadArticlesPartial?limit=${limit}&offset=${offset}`);
    const html = await res.text();

    document.getElementById("articlesContainer")
        .insertAdjacentHTML("beforeend", html);

    offset += limit;
    loading = false;
    document.getElementById("loader").style.display = "none";
}

async function refreshDbArticles() {
    const container = document.getElementById("articlesContainer");
    container.innerHTML = "";

    const dbHtml = await fetch("/Home/LoadDbArticlesPartial").then(r => r.text());
    container.insertAdjacentHTML("beforeend", dbHtml);

    offset = 0;
    loadArticles();
}

// Primera carga
(async () => {
    await refreshDbArticles();
})();

// Scroll infinito
window.addEventListener("scroll", () => {
    if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 200) {
        loadArticles();
    }
});
