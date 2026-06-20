document.addEventListener("DOMContentLoaded", () => {
    
    // Controle do botão de "Voltar ao topo"
    const btnScrollTop = document.getElementById("btnScrollTop");
    if (btnScrollTop) {
        // Exibe ou oculta o botão dependendo da posição de rolagem da página
        window.addEventListener("scroll", () => {
            btnScrollTop.classList.toggle("mostrar", window.scrollY > 300);
        });

        // Rola a página suavemente para o topo ao clicar
        btnScrollTop.addEventListener("click", () => {
            window.scrollTo({ top: 0, behavior: "smooth" });
        });
    }

    // Controle de exibição de senha nos campos do formulário
    document.querySelectorAll(".btn-toggle-senha").forEach(btn => {
        const input = btn.previousElementSibling;
        if (!input) return;

        // Mostra o botão do olhinho apenas se houver algum caractere digitado
        const atualizarVisibilidade = () => {
            btn.classList.toggle("mostrar", input.value.length > 0);
        };

        input.addEventListener("input", atualizarVisibilidade);
        atualizarVisibilidade();

        // Alterna entre exibir e ocultar a senha (modo text ou password)
        btn.addEventListener("click", () => {
            const isPassword = input.type === "password";
            input.type = isPassword ? "text" : "password";
            
            // Alterna os ícones de olho aberto/fechado e altera o rótulo de acessibilidade
            btn.querySelector(".olho-aberto")?.classList.toggle("d-none", !isPassword);
            btn.querySelector(".olho-fechado")?.classList.toggle("d-none", isPassword);
            btn.setAttribute("aria-label", isPassword ? "Ocultar senha" : "Mostrar senha");
        });
    });
});
