// Arquivo: wwwroot/js/Atendimento/AtendimentoScript.js

// --- NOVO: Função para abrir o modal de "Novo Atendimento" e preencher o solicitante ---
function abrirModalNovoAtendimento() {
    // Obter o ID e o Nome do funcionário logado dos campos hidden (do HTML)
    const loggedInFuncionarioId = document.getElementById('loggedInFuncionarioId')?.value;
    const loggedInFuncionarioNome = document.getElementById('loggedInFuncionarioNome')?.value;

    // Referência ao select de Funcionário Solicitante no modal de NOVO Atendimento
    // Assumo que o ID do select é "FuncionarioSolicitanteId" no modal de criação
    const selectFuncionarioSolicitante = document.getElementById('FuncionarioSolicitanteId');

    // Limpar campos do modal antes de abrir (boa prática para "Novo")
    // Certifique-se de que esses IDs correspondem aos campos do seu modal de NOVO atendimento
    document.getElementById("AnimalId").value = "";
    document.getElementById("FuncionarioSolicitanteId").value = ""; // Será preenchido abaixo
    document.getElementById("VeterinarioResponsavel").value = "";
    document.getElementById("DataAtendimento").value = "";
    document.getElementById("Descricao").value = "";

    if (selectFuncionarioSolicitante && loggedInFuncionarioId) {
        // Encontra a opção com o valor correspondente ao ID do funcionário logado e a seleciona
        selectFuncionarioSolicitante.value = loggedInFuncionarioId;
        
        // Opcional: Se quiser que o campo não possa ser alterado depois de preenchido automaticamente
        // selectFuncionarioSolicitante.disabled = true; 
    } else {
        console.warn("Elemento 'FuncionarioSolicitanteId' não encontrado ou ID do funcionário logado não disponível. Verifique os campos hidden e o ID do select.");
    }

    // Abre o modal. Assumo que seu modal de "Novo Atendimento" tem o seletor '.area-modal-novo'
    // Se for outro seletor para o modal de novo, AJUSTE AQUI.
    abrirModal('.area-modal-novo');
}

// --- Outras funções do seu script atual ---

async function solicitarAtendimento() {
  const animalId = document.getElementById("AnimalId").value;
  const funcionarioSolicitanteId = document.getElementById(
    "FuncionarioSolicitanteId"
  ).value;
  const veterinarioId = document.getElementById("VeterinarioResponsavel").value; // Alterado o nome da variável
  const dataAtendimento = document.getElementById("DataAtendimento").value;
  const descricao = document.getElementById("Descricao").value;

  const dadosDoAtendimento = {
    AnimalId: animalId ? parseInt(animalId) : null,
    FuncionarioSolicitanteId: funcionarioSolicitanteId
      ? parseInt(funcionarioSolicitanteId)
      : null,
    FuncionarioVeterinarioId: veterinarioId ? parseInt(veterinarioId) : null, // Corrigido o nome da propriedade
    Data: dataAtendimento, // Verifique se está no formato "YYYY-MM-DD"
    Descricao: descricao,
  };

  try {
    const response = await fetch(
      "/AtendimentoVeterinario/CriarAtendimentoAjax",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(dadosDoAtendimento),
      }
    );

    const responseData = await response.json(); // Mova esta linha para fora do if (JÁ ESTAVA ASSIM NO SEU CÓDIGO)

    if (response.ok) {
      console.log("Sucesso:", responseData);
      // Chame a função correta para fechar o modal de NOVO atendimento
      fecharModal('.area-modal-novo'); // Use a sua função genérica fecharModal com o seletor correto
      window.location.reload();
    } else {
      console.error("Erro:", responseData);
      alert(
        "Erro ao solicitar atendimento: " +
          (responseData.message || response.statusText)
      );
    }
  } catch (error) {
    console.error("Erro na requisição:", error);
    alert("Ocorreu um erro ao conectar com o servidor: " + error.message);
  }
}

function verificaCheckboxes() {
  const checkboxes = document.querySelectorAll(".atendimento-checkbox");
  const botaoEditar = document.querySelector(".BotaoEditar");

  let qtdMarcada = 0;
  let idAtendimentoSelecionado = null;

  checkboxes.forEach((checkbox) => {
    if (checkbox.checked) {
      qtdMarcada++;
      idAtendimentoSelecionado = parseInt(checkbox.dataset.atendimentoid);
    }
  });

  if (botaoEditar) {
    if (qtdMarcada <= 1) { // Mudança para <= 1 para habilitar se 0 ou 1 estiverem marcados
      botaoEditar.disabled = false;
      botaoEditar.classList.remove("desabilitado");
      botaoEditar.dataset.atendimentoid = idAtendimentoSelecionado;

    } else {
      botaoEditar.classList.add("desabilitado");
      botaoEditar.disabled = true;
      delete botaoEditar.dataset.atendimentoid;
      
    }
    botaoEditar.style.transition = "all 0.2s ease";
  }
}

document.addEventListener("DOMContentLoaded", verificaCheckboxes);

async function abrirModalAtendimento(atendimentoId, purpose) {
  const modalTitulo = document.getElementById("modalEditarTitulo");
  const modalActionBtn = document.getElementById("modalActionBtn");
  const modalPurposeInput = document.getElementById("modalPurpose");

  if (!atendimentoId) {
    alert("ID do atendimento não fornecido.");
    return;
  }

  try {
    const response = await fetch(
      `/AtendimentoVeterinario/ObterAtendimentoPorId/${atendimentoId}`
    );
    if (!response.ok) {
      let errorMessage = "Erro desconhecido ao buscar dados.";
      try {
        const errorData = await response.json();
        errorMessage = errorData.message || errorMessage;
      } catch (e) {
        errorMessage =
          response.statusText || "Não foi possível carregar os dados.";
      }
      throw new Error(errorMessage);
    }
    const atendimento = await response.json();
    console.log("Dados do atendimento:", atendimento);

    // Preencher o modal com os dados
    document.getElementById("editAtendimentoId").value =
      atendimento.atendimentoVeterinarioId;
    document.getElementById("editAnimalId").value = atendimento.animalId || "";
    document.getElementById("editFuncionarioSolicitanteId").value =
      atendimento.funcionarioSolicitanteId || "";
    document.getElementById("editVeterinarioResponsavel").value =
      atendimento.funcionarioVeterinarioId || "";
    const dataFormatada = atendimento.data
      ? atendimento.data.split("T")[0]
      : "";
    document.getElementById("editDataAtendimento").value = dataFormatada;
    document.getElementById("editDescricao").value =
      atendimento.descricao || "";
    document.getElementById("editObservacoes").value =
      atendimento.observacoes || "";
    document.getElementById("editResultado").value =
      atendimento.resultado || "";

    // Ajustar o modal com base no propósito
    modalPurposeInput.value = purpose; // Define o propósito

    if (purpose === "edit") {
      modalTitulo.textContent = "Editar Atendimento";
      modalActionBtn.textContent = "Salvar Edição";
      modalActionBtn.onclick = salvarEdicaoAtendimento; // Atribui a função de salvar
      // Habilita/Desabilita campos conforme a necessidade de edição
      document.getElementById("editAnimalId").disabled = false;
      document.getElementById("editFuncionarioSolicitanteId").disabled = false;
      document.getElementById("editVeterinarioResponsavel").disabled = false;
      document.getElementById("editDataAtendimento").disabled = false;
      document.getElementById("editDescricao").disabled = false;
      document.getElementById("editObservacoes").disabled = false;
      document.getElementById("editResultado").disabled = false;
    } else if (purpose === "concluir") {
      modalTitulo.textContent = "Concluir Atendimento";
      modalActionBtn.textContent = "Finalizar Atendimento";
      modalActionBtn.onclick = finalizarAtendimento; // Atribui a função de finalizar
      document.getElementById("editAnimalId").disabled = true;
      document.getElementById("editFuncionarioSolicitanteId").disabled = true;
      document.getElementById("editVeterinarioResponsavel").disabled = true;
      document.getElementById("editDataAtendimento").disabled = true;
      // document.getElementById("editDescricao").disabled = true; // Removi o disable desses para permitir edição na conclusão
      // document.getElementById("editObservacoes").disabled = true;
      // document.getElementById("editResultado").disabled = true;
    }

    // Abre o modal (assumo que este é o modal de edição)
    abrirModal(".area-modal-editar");
  } catch (error) {
    console.error("Erro ao abrir modal:", error);
    alert(
      `Não foi possível carregar os dados: ${error.message}. Tente novamente.`
    );
  }
}

// *** FUNÇÃO CHAMADA PELO BOTÃO EDITAR ***
function abrirModalEdicao() {
  const botaoEditar = document.querySelector(".BotaoEditar");
  const atendimentoId = botaoEditar.dataset.atendimentoid;
  if (atendimentoId) {
    abrirModalAtendimento(atendimentoId, "edit");
  } else {
    alert("Nenhum atendimento selecionado para edição.");
  }
}

// *** FUNÇÃO CHAMADA PELO BOTÃO CONCLUIR NO CARD ***
function abrirModalConcluir(atendimentoId) {
  if (atendimentoId) {
    abrirModalAtendimento(atendimentoId, "concluir");
  } else {
    alert("ID do atendimento para conclusão não fornecido.");
  }
}

// *** FUNÇÃO PARA SALVAR EDIÇÃO (chamada pelo botão "Salvar Edição" no modal) ***
async function salvarEdicaoAtendimento() {
  const atendimentoId = document.getElementById("editAtendimentoId").value;
  if (!atendimentoId) {
    alert("Erro: ID do atendimento para edição não encontrado.");
    return;
  }

  const dadosAtualizados = {
    AtendimentoVeterinarioId: parseInt(atendimentoId),
    AnimalId: document.getElementById("editAnimalId").value
      ? parseInt(document.getElementById("editAnimalId").value)
      : null,
    FuncionarioSolicitanteId: document.getElementById(
      "editFuncionarioSolicitanteId"
    ).value
      ? parseInt(document.getElementById("editFuncionarioSolicitanteId").value)
      : null,
    FuncionarioVeterinarioId: document.getElementById(
      "editVeterinarioResponsavel"
    ).value
      ? parseInt(document.getElementById("editVeterinarioResponsavel").value)
      : null,
    Data: document.getElementById("editDataAtendimento").value,
    Descricao: document.getElementById("editDescricao").value,
    Observacoes: document.getElementById("editObservacoes").value || null,
    Resultado: document.getElementById("editResultado").value || null,
    Status: true, // Edição geralmente mantém o status ativo, a menos que haja um campo para isso
  };

  try {
    const response = await fetch(
      `/AtendimentoVeterinario/AtualizarAtendimento`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(dadosAtualizados),
      }
    );

    const responseData = await response.json();
    if (response.ok && responseData.success) {
      alert(responseData.message || "Atendimento atualizado com sucesso!");
      // FecharModalEditar(); // Esta função não existe, use fecharModal com o seletor
      fecharModal('.area-modal-editar'); // Assumindo que este é o seletor do modal de edição
      window.location.reload();
    } else {
      alert(
        "Erro ao atualizar atendimento: " +
          (responseData.message || response.statusText)
      );
    }
  } catch (error) {
    console.error("Erro na requisição de atualização:", error);
    alert(
      "Ocorreu um erro ao conectar com o servidor para atualizar: " +
        error.message
    );
  }
}

async function finalizarAtendimento() {
    const atendimentoId = document.getElementById("editAtendimentoId").value; // Pega o ID do campo oculto
    if (!atendimentoId) {
        alert("Erro: ID do atendimento para finalizar não encontrado.");
        return;
    }

    const confirmacao = confirm(
        "Tem certeza que deseja finalizar este atendimento? O status será alterado para inativo."
    );
    if (!confirmacao) {
        return; // Usuário cancelou
    }

    // Coleta TODOS os dados do modal de edição/conclusão
    const dadosParaFinalizar = {
        AtendimentoVeterinarioId: parseInt(atendimentoId),
        AnimalId: document.getElementById("editAnimalId").value ? parseInt(document.getElementById("editAnimalId").value) : null,
        FuncionarioSolicitanteId: document.getElementById("editFuncionarioSolicitanteId").value ? parseInt(document.getElementById("editFuncionarioSolicitanteId").value) : null,
        FuncionarioVeterinarioId: document.getElementById("editVeterinarioResponsavel").value ? parseInt(document.getElementById("editVeterinarioResponsavel").value) : null,
        Data: document.getElementById("editDataAtendimento").value,
        Descricao: document.getElementById("editDescricao").value,
        Observacoes: document.getElementById("editObservacoes").value || null,
        Resultado: document.getElementById("editResultado").value || null, 
        Status: false, 
    };

    try {
        const response = await fetch(
            `/AtendimentoVeterinario/ConcluirAtendimento`, // Certifique-se que essa rota está no C#
            {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(dadosParaFinalizar), // <--- AGORA ESTAMOS ENVIANDO TODOS OS DADOS
            }
        );

        const responseData = await response.json();
        if (response.ok && responseData.success) {
            alert(responseData.message || "Atendimento finalizado com sucesso!");
            fecharModal('.area-modal-editar'); // Fecha o modal
            window.location.reload(); // Recarrega a página para atualizar os cards
        } else {
            alert(
                "Erro ao finalizar atendimento: " +
                (responseData.message || response.statusText)
            );
        }
    } catch (error) {
        console.error("Erro na requisição de finalização:", error);
        alert(
            "Ocorreu um erro ao conectar com o servidor para finalizar: " +
            error.message
        );
    }
}

// *** FUNÇÃO PARA EXCLUIR ATENDIMENTOS SELECIONADOS (chamada pelo botão "Excluir" na barra de ações) ***
async function excluirAtendimentosSelecionados() {
  const checkboxesMarcados = document.querySelectorAll(
    ".atendimento-checkbox:checked"
  );

  if (checkboxesMarcados.length === 0) {
    alert("Selecione pelo menos um atendimento para excluir.");
    return;
  }

  const confirmacao = confirm(
    "Tem certeza que deseja EXCLUIR permanentemente os atendimentos selecionados?"
  );
  if (!confirmacao) {
    return; // Usuário cancelou
  }

  const idsParaExcluir = Array.from(checkboxesMarcados).map((cb) =>
    parseInt(cb.dataset.atendimentoid)
  );

  try {
    for (const id of idsParaExcluir) {
      const response = await fetch(
        `/AtendimentoVeterinario/ExcluirAtendimento`,
        {
          // Chama o novo método de exclusão
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(id),
        }
      );

      const data = await response.json();
      if (response.ok && data.success) {
        console.log(`Atendimento ${id} excluído com sucesso!`);
      } else {
        console.error(`Erro ao excluir atendimento ${id}:`, data.message);
        alert(`Erro ao excluir atendimento ${id}: ${data.message}`);
      }
    }

    alert("Processo de exclusão concluído. A página será recarregada.");
    window.location.reload();
  } catch (error) {
    console.error("Erro na requisição de exclusão:", error);
    alert(
      "Ocorreu um erro ao tentar excluir os atendimentos. Tente novamente."
    );
  }
}

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

  if (overlay && main.contains(overlay)) { // Verifica se o overlay ainda existe antes de tentar remover
      main.removeChild(overlay);
  }
  modal.style.display = "none";
}

// --- Listener para habilitar/desabilitar botão "Editar" ao carregar a página ---
document.addEventListener("DOMContentLoaded", verificaCheckboxes);