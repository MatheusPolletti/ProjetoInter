async function solicitarAtendimento() {
  // Coleta dos valores
  const especieId = document.getElementById("EspecieId").value;
  const animalId = document.getElementById("AnimalId").value;
  const funcionarioSolicitanteId = document.getElementById(
    "FuncionarioSolicitanteId"
  ).value;
  const veterinarioId = document.getElementById("VeterinarioResponsavel").value; // Alterado o nome da variável
  const dataAtendimento = document.getElementById("DataAtendimento").value;
  const descricao = document.getElementById("Descricao").value;

  const dadosDoAtendimento = {
    EspecieId: especieId ? parseInt(especieId) : null,
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

    const responseData = await response.json(); // Mova esta linha para fora do if

    if (response.ok) {
      console.log("Sucesso:", responseData);
      alert(responseData.message || "Atendimento solicitado com sucesso!");
      FecharModalNovo();
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
    if (qtdMarcada <= 1) {
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

// *** FUNÇÃO CENTRAL PARA ABRIR O MODAL DE EDIÇÃO/CONCLUSÃO ***
// purpose: 'edit' ou 'concluir'
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
    document.getElementById("editEspecieId").value =
      atendimento.animal?.especieId || "";
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
      document.getElementById("editEspecieId").disabled = false;
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
      // Desabilita campos se for apenas para conclusão (o usuário não deve editar ao concluir)
      document.getElementById("editEspecieId").disabled = true;
      document.getElementById("editAnimalId").disabled = true;
      document.getElementById("editFuncionarioSolicitanteId").disabled = true;
      document.getElementById("editVeterinarioResponsavel").disabled = true;
      document.getElementById("editDataAtendimento").disabled = true;
      document.getElementById("editDescricao").disabled = true;
      document.getElementById("editObservacoes").disabled = true;
      document.getElementById("editResultado").disabled = true;
    }

    // Abre o modal
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
      FecharModalEditar();
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

// *** FUNÇÃO PARA FINALIZAR ATENDIMENTO (chamada pelo botão "Finalizar Atendimento" no modal) ***
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

  try {
    const response = await fetch(
      `/AtendimentoVeterinario/ConcluirAtendimento`,
      {
        // Chama o novo método no Controller
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(parseInt(atendimentoId)), // Envia apenas o ID como corpo
      }
    );

    const responseData = await response.json();
    if (response.ok && responseData.success) {
      alert(responseData.message || "Atendimento finalizado com sucesso!");
      FecharModalEditar();
      window.location.reload();
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

  main.removeChild(overlay);
  modal.style.display = "none";
}
