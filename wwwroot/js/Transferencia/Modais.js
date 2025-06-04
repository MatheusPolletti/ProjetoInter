function ModalNovo()
{
    const modalNovo = document.querySelector("#area-modal-setores");

    const main = document.querySelector(".MainContent");
    const overlay = document.createElement("div");
    
    overlay.className = "overlay";
    
    main.appendChild(overlay);
    
    modalNovo.style.display = "flex";
    modalNovo.style.position = "absolute";
}

function FecharModal()
{
    const modalNovo = document.querySelector("#area-modal-setores");
    
    const main = document.querySelector(".MainContent");
    const overlay = document.querySelector(".overlay");
    
    main.removeChild(overlay);
    modalNovo.style.display = "none";
}
