document.addEventListener("DOMContentLoaded", () => {
  // Botão para o modal de edição já incluido
  const btnEditar = document.getElementById("btnEditar");

  if (btnEditar) {
    btnEditar.addEventListener("click", ModalEditarNovo);
  }

  // Envio do formulário de novo animal
  const form = document.getElementById("formNovoAnimal");

  if (form) {
    form.addEventListener("submit", function (e) {
      e.preventDefault();

      const errosDiv = document.getElementById("formErrors");

      if (errosDiv) {
        errosDiv.innerHTML = "";
      }

      const formData = new FormData(form);

      fetch(form.action, {
        method: "POST",
        body: formData,
        headers: {
          "RequestVerificationToken": form.querySelector("input[name='__RequestVerificationToken']").value,
        },
      })
        .then((response) => response.json()) 
        .then((data) => {
          if (data.success) {
            abriuModalSucesso("Animal adicionado com sucesso!");
          } else {
            if (data.errors && data.errors.length > 0) {
              errosDiv.innerHTML = data.errors.map((e) => `<div>${e}</div>`).join('');
            } else {
              errosDiv.innerHTML = "Erro desconhecido ao acrescentar o animal.";
            }
          }
        })
        .catch((error) => {
          if (errosDiv) {
            errosDiv.innerHTML = "Erro ao enviar formulário: " + error.message;
          }
        });
    });
  }

  // Modal para nova espécie
  const formNovaEspecie = document.getElementById("formNovaEspecie");

  if (formNovaEspecie) {
    formNovaEspecie.addEventListener("submit", function (e) {
      e.preventDefault();

      const erroEspecie = document.getElementById("erroEspecie");

      if (erroEspecie) {
        erroEspecie.innerHTML = ""; 
      }

      const formData = new FormData(formNovaEspecie);

      fetch(formNovaEspecie.action, {
        method: "POST",
        body: formData,
        headers: {
          "RequestVerificationToken": formNovaEspecie.querySelector("input[name='__RequestVerificationToken']").value
        }
      })
        .then((response) => response.json()) 
        .then((data) => {
          if (data.success && data.especie) {
            // Adiciona nova opção no select de espécie
            const select = document.getElementById("EspecieId");

            if (select) {
              const option = document.createElement("option");

              option.value = data.especie.animalEspecieId;
              option.text = data.especie.descricao;

              select.appendChild(option);
              select.value = data.especie.animalEspecieId;
            }

            FecharModalNovaEspecie();
            formNovaEspecie.reset();
            abriuModalSucesso("Espécie adicionada com sucesso!");

          } else {
            if (erroEspecie) {
              erroEspecie.innerHTML = data.message || "Erro ao adicionar espécie.";
            }
          }
        })
        .catch((error) => {
          if (erroEspecie) {
            erroEspecie.innerHTML = "Erro ao enviar: " + error.message;
          }
        });
    });
  }
});

// --------------------------------------------------------------------- Modal para editar animal ---------------------------------------------------------------------

function ModalEditarNovo() {
  const checks = document.querySelectorAll(".selecionar-animal:checked");

  // Validação de seleção
  if (checks.length === 0) {
    abriuModalAviso("Selecione um animal para editar.");
    return;
  }

  if (checks.length > 1) {
    abriuModalAviso("Para editar, selecione apenas um animal.");
    return;
  }

  const animalId = checks[0].getAttribute('data-id');
  
  fetch(`/Animal/Editar/${animalId}`)
    .then(response => {
      if (!response.ok) throw new Error("Erro ao carregar animal");
      return response.text();
    })
    .then(html => {
      // Criar um container temporário
      const container = document.createElement('div');
      container.innerHTML = html;
      
      // Adicionar ao corpo do documento
      document.body.appendChild(container);
      
      // Mostrar o modal
      const modal = container.querySelector('.area-modal-editar');
      modal.style.display = "flex";
      
      // Configurar evento de fechamento
      const fecharBtn = modal.querySelector('.fechar');
      fecharBtn.onclick = () => {
        document.body.removeChild(container);
      };
      
      // Configurar submit do formulário
      const form = modal.querySelector('#formEditarAnimal');
      form.addEventListener('submit', function(e) {
        e.preventDefault();
        submitFormEditar(form);
      });
      
      // Configurar botão de adicionar espécie
      const btnMaisEspecie = modal.querySelector('.btn-mais-especie');
      if (btnMaisEspecie) {
        btnMaisEspecie.onclick = AbrirModalNovaEspecie;
      }
    })
    .catch(err => abriuModalAviso(err.message));
}

function submitFormEditar(form) {
  const errorsDiv = form.querySelector("#formEditarErrors");
  errorsDiv.innerHTML = '';
  const formData = new FormData(form);

  fetch(form.action, {
    method: 'POST',
    body: formData,
    headers: {
      'RequestVerificationToken': form.querySelector('input[name="__RequestVerificationToken"]').value
    }
  })
  .then(r => r.json()) 
  .then(json => {
    if (json.success) {
      abriuModalSucesso('Animal atualizado com sucesso!');
    } else if (json.errors) {
      errorsDiv.innerHTML = json.errors.map(x => `<div>${x}</div>`).join('');
    } else {
      errorsDiv.textContent = "Erro desconhecido.";
    }
  })
  .catch(err => { errorsDiv.textContent = err.message; });
}

// Função para mostrar preview da imagem
function mostrarPreviewEditar(event) {
  const input = event.target;
  const previewContainer = input.closest('.form-imagem');
  const preview = previewContainer.querySelector('img');
  const previewText = previewContainer.querySelector('span');
  
  if (input.files && input.files[0]) {
    const reader = new FileReader();
    reader.onload = function(e) {
      preview.src = e.target.result;
      preview.style.display = "block";
      if (previewText) previewText.style.display = "none";
    }
    reader.readAsDataURL(input.files[0]);
  }
}

// --------------------------------------------------------------------- Modal para criar novo animal ---------------------------------------------------------------------

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

function AbrirModalNovaEspecie() {
  document.getElementById("modalNovaEspecie").style.display = "flex";
  document.querySelector(".area-modal-novo").style.display = "none";
}

function FecharModalNovaEspecie() {
  document.getElementById("modalNovaEspecie").style.display = "none";
  document.querySelector(".area-modal-novo").style.display = "flex";
}

// --------------------------------------------------------------------- Funções de modal ---------------------------------------------------------------------

function abriuModalAviso(mensagem) {
  const modalAviso = document.createElement("div");
  modalAviso.className = "modal-aviso";
  modalAviso.innerHTML = `
    <div class="modal-conteudo">
      <span class="fechar" onclick="fecharModalAviso()">×</span>
      <p>${mensagem}</p>
    </div>
  `;
  document.body.appendChild(modalAviso);
  modalAviso.style.display = "flex";
}

function fecharModalAviso() {
  const modalAviso = document.querySelector(".modal-aviso");
  if (modalAviso) {
    modalAviso.remove();
  }
}

// Nova função para modal de sucesso
function abriuModalSucesso(mensagem) {
  const modalSucesso = document.createElement("div");
  modalSucesso.className = "modal-sucesso";
  modalSucesso.innerHTML = `
    <div class="modal-conteudo">
      <span class="fechar" onclick="fecharModalSucesso()">×</span>
      <p>${mensagem}</p>
      <button class="btn-ok" onclick="fecharModalSucesso()">OK</button>
    </div>
  `;
  document.body.appendChild(modalSucesso);
  modalSucesso.style.display = "flex";
}

function fecharModalSucesso() {
  const modalSucesso = document.querySelector(".modal-sucesso");
  if (modalSucesso) {
    modalSucesso.remove();
    location.reload(); // Recarrega a página após fechar o modal de sucesso
  }
}