function abrirModal(selector) {
  const modal = document.querySelector(selector);
  const main = document.querySelector(".MainContent");

  if (!modal || !main) return;

  // Cria overlay se não existir
  let overlay = document.querySelector(".overlay");
  if (!overlay) {
    overlay = document.createElement("div");
    overlay.className = "overlay";
    main.appendChild(overlay);

    overlay.addEventListener("click", () => fecharModal(selector));
  }

  modal.style.display = "flex";
}

function fecharModal(selector) {
  const modal = document.querySelector(selector);
  const main = document.querySelector(".MainContent");
  const overlay = document.querySelector(".overlay");

  if (!modal || !main || !overlay) return;

  main.removeChild(overlay);
  modal.style.display = "none";
}

function ModalNovo() {
  abrirModal(".area-modal-novo");
}

function FecharModalNovo() {
  fecharModal(".area-modal-novo");
}

function ModalEditarNovo() {
  abrirModal(".area-modal-editar");
}

function FecharModalEditar() {
  fecharModal(".area-modal-editar");
}