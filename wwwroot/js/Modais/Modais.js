function ModalNovo()
{
    const modalNovo = document.querySelector(".area-modal-novo");

    const main = document.querySelector(".MainContent");
    const overlay = document.createElement("div");
    
    overlay.className = "overlay";
    
    main.appendChild(overlay);
    
    modalNovo.style.display = "flex";
    modalNovo.style.position = "absolute";
}

function FecharModalNovo()
{
    const modalNovo = document.querySelector(".area-modal-novo");
    
    const main = document.querySelector(".MainContent");
    const overlay = document.querySelector(".overlay");
    
    main.removeChild(overlay);
    modalNovo.style.display = "none";
}

function ModalEditarNovo()
{
    const modalEditar = document.querySelector(".area-modal-editar");

    const main = document.querySelector(".MainContent");
    const overlay = document.createElement("div");

    overlay.className = "overlay";

    main.appendChild(overlay);

    modalEditar.style.display = "flex";
    modalEditar.style.position = "absolute";
}

function FecharModalEditar()
{
    const modalEditar = document.querySelector(".area-modal-editar");

    const main = document.querySelector(".MainContent");
    const overlay = document.querySelector(".overlay");

    main.removeChild(overlay);
    modalEditar.style.display = "none";
}