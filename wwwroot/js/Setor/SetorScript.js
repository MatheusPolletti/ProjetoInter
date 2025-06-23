// Arquivo: wwwroot/js/Setor/SetorScript.js

// --- Referências aos elementos do HTML (Modal e Botões) ---

// Modal UNIFICADO
const modalSetor = document.querySelector(".area-modal-novo"); // Agora refere-se ao único modal

// Campos do Modal UNIFICADO
const inputSetorId = document.getElementById("inputSetorId"); // Novo ID genérico
const inputSetorNome = document.getElementById("inputSetorNome"); // Novo ID genérico
const inputSetorDescricao = document.getElementById("inputSetorDescricao"); // Novo ID genérico
const selectSetorStatus = document.getElementById("selectSetorStatus"); // Novo ID genérico

// Elementos dinâmicos do modal
const modalSetorTitle = document.getElementById("modalSetorTitle");
const modalSetorSubmitButton = document.getElementById(
  "modalSetorSubmitButton"
);

// Botões do Header
const botaoExcluir = document.querySelector(".BotaoExcluir");
const botaoEditar = document.querySelector(".BotaoEditar"); // Mantido para pegar o dataset.setorid

// --- Funções de Abrir/Fechar Modais e Limpeza ---

// Função para limpar e resetar os campos do modal
function limparCamposModalSetor() {
  inputSetorId.value = "0"; // Reseta para 0 (indicando novo setor)
  inputSetorNome.value = "";
  inputSetorDescricao.value = "";
  selectSetorStatus.value = "true"; // Valor padrão para "Ativo"
  modalSetorTitle.innerText = "Adicionar Setor"; // Reseta o título
  modalSetorSubmitButton.innerText = "Adicionar"; // Reseta o texto do botão
}

// Função para fechar o modal
function FecharModalSetor() {
  const main = document.querySelector(".MainContent");
  const overlay = document.querySelector(".overlay");

  main.removeChild(overlay);
  document.querySelector('.area-modal-novo').style.display = "none";
  limparCamposModalSetor(); // Limpa os campos ao fechar
}

// Função para abrir o modal para CRIAR um novo setor
function abrirModalNovoSetor() {
  limparCamposModalSetor(); // Garante que os campos estejam limpos e configurados para "novo"
  const modal = document.querySelector('.area-modal-novo');
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

// Função para abrir o modal para EDITAR um setor existente
async function abrirModalEditarSetor() {
  const setorId = botaoEditar.dataset.setorid; // Pega o ID armazenado no botão "Editar"

  if (!setorId) {
    alert("Nenhum setor selecionado para edição.");
    return;
  }

  try {
    const response = await fetch(`/Setor/ObterSetorPorId?id=${setorId}`); // Usando 'id=' no parâmetro
    const data = await response.json();

    if (response.ok) {
      // Preenche os campos do modal com os dados do setor
      inputSetorId.value = data.setorId; // Acessando a propriedade do objeto retornado pelo servidor
      inputSetorNome.value = data.nome;
      inputSetorDescricao.value = data.descricao;
      // Converte o booleano do C# para a string "true" ou "false" esperada pelo <select>
      selectSetorStatus.value = data.status.toString();

      // Altera o título do modal e o texto do botão para "Editar"
      modalSetorTitle.innerText = "Editar Setor";
      modalSetorSubmitButton.innerText = "Salvar Alterações";

      const main = document.querySelector(".MainContent");

      // Cria overlay se não existir
      let overlay = document.querySelector(".overlay");
      if (!overlay) {
        overlay = document.createElement("div");
        overlay.className = "overlay";
        main.appendChild(overlay);
      }

      modalSetor.style.display = "flex"; // Abre o modal
    } else {
      alert(
        "Erro ao carregar dados do setor: " +
          (data.message || response.statusText)
      );
    }
  } catch (error) {
    console.error("Erro ao buscar dados do setor:", error);
    alert("Erro ao conectar com o servidor para obter dados do setor.");
  }
}

async function handleSetorSubmit() {
  const setorId = inputSetorId.value; // Pega o ID do campo oculto
  const nome = inputSetorNome.value;
  const descricao = inputSetorDescricao.value;
  const status = selectSetorStatus.value === "true"; // Converte string para booleano

  // Validação básica
  if (!nome || !descricao) {
    alert("Por favor, preencha o nome e a descrição do setor.");
    return;
  }

  const dadosDoSetor = {
    SetorId: parseInt(setorId), // Garante que o ID é um número inteiro
    Nome: nome,
    Descricao: descricao,
    Status: status,
  };

  let url = "";
  let method = "POST";

  // Decide se é para criar ou atualizar com base no SetorId
  if (setorId === "0") {
    // Se o ID for 0, é uma nova criação
    url = "/Setor/CriarSetor";
  } else {
    // Se o ID for diferente de 0, é uma atualização
    url = "/Setor/AtualizarSetor";
  }

  try {
    const response = await fetch(url, {
      method: method,
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(dadosDoSetor),
    });

    const responseData = await response.json();

    if (response.ok && responseData.success) {
      FecharModalSetor(); // Fecha o modal
      window.location.reload(); // Recarrega a página para ver as mudanças
    } else {
      console.error("Erro na operação de setor:", responseData);
      alert(
        "Erro na operação de setor: " +
          (responseData.message || response.statusText)
      );
    }
  } catch (error) {
    console.error("Erro na requisição de setor:", error);
    alert(
      "Ocorreu um erro ao conectar com o servidor para a operação de setor: " +
        error.message
    );
  }
}

// --- Funções de Seleção de Checkbox e Gerenciamento de Botões ---

function verificaCheckboxesSetor() {
  const checkboxes = document.querySelectorAll(".setor-checkbox");

  let qtdMarcada = 0;
  let idSetorSelecionado = null; // Para guardar o ID do setor se apenas um estiver marcado

  checkboxes.forEach((checkbox) => {
    if (checkbox.checked) {
      qtdMarcada++;
      idSetorSelecionado = parseInt(checkbox.dataset.setorid);
    }
  });

  // Gerencia o botão "Excluir"
  if (botaoExcluir) {
    if (qtdMarcada > 0) {
      botaoExcluir.disabled = false;
     } else {
       botaoExcluir.disabled = true;
     }
}

  // Gerencia o botão "Editar"
  if (botaoEditar) {
    if (qtdMarcada < 2) {
      botaoEditar.disabled = false;
      // Armazena o ID do setor selecionado no botão "Editar" para uso futuro
      botaoEditar.dataset.setorid = idSetorSelecionado;
      botaoEditar.classList.remove("desabilitado");
    } else {
      botaoEditar.disabled = true;
      // Limpa o ID armazenado se mais de um ou nenhum estiver selecionado
      delete botaoEditar.dataset.setorid;
      botaoEditar.classList.add("desabilitado");
    }
    // Opcional: Adicione estilos para feedback visual de habilitado/desabilitado
    // botaoEditar.style.opacity = botaoEditar.disabled ? "0.5" : "1";
    // botaoEditar.style.cursor = botaoEditar.disabled ? "not-allowed" : "pointer";
    botaoEditar.style.transition = "all 0.2s ease-in-out";
  }
}

// --- Função de Exclusão ---

async function excluirSetoresSelecionados() {
  const checkboxesMarcados = document.querySelectorAll(
    ".setor-checkbox:checked"
  );
  if (checkboxesMarcados.length === 0) {
    alert("Nenhum setor selecionado para exclusão.");
    return;
  }

  const confirmacao = confirm(
    `Tem certeza que deseja excluir ${checkboxesMarcados.length} setor(es)?`
  );
  if (!confirmacao) {
    return; // Usuário cancelou
  }

  const idsParaExcluir = Array.from(checkboxesMarcados).map((cb) =>
    parseInt(cb.dataset.setorid)
  );

  try {
    const response = await fetch("/Setor/ExcluirSetores", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(idsParaExcluir), // Envia array de IDs
    });

    const data = await response.json();

    if (response.ok && data.success) {
      window.location.reload(); // Recarrega a página para ver as mudanças
    } else {
      console.error("Erro ao excluir setor(es):", data.message);
      alert(
        "Erro ao excluir setor(es): " + (data.message || response.statusText)
      );
    }
  } catch (error) {
    console.error("Erro na requisição de exclusão de setor(es):", error);
    alert("Ocorreu um erro ao conectar com o servidor para excluir setor(es).");
  }
}

// --- Event Listener para carregar a função de verificação de checkboxes ao iniciar a página ---
document.addEventListener("DOMContentLoaded", verificaCheckboxesSetor);
