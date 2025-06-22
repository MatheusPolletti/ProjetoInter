const modal = document.querySelector(".area-modal-novo");
const modalTitle = document.getElementById("modalTitle");
const transferenciaIdHidden = document.getElementById("transferenciaIdHidden");
const newTransferAnimalId = document.getElementById("newTransferAnimalId");
const newTransferOrigemId = document.getElementById("newTransferOrigemId");
const newTransferDestinoId = document.getElementById("newTransferDestinoId");
const newTransferDataSaida = document.getElementById("newTransferDataSaida");
const newTransferDataEntrada = document.getElementById(
  "newTransferDataEntrada"
);
const modalSubmitButton = document.getElementById("modalSubmitButton");

function abrirModalNovoTransferencia() {
  modalTitle.textContent = "Adicionar Transferência";
  modalSubmitButton.textContent = "Adicionar";
  transferenciaIdHidden.value = "0";

  newTransferAnimalId.value = "";

  if (newTransferAnimalId.options.length > 0)
    newTransferAnimalId.selectedIndex = 0;
  if (newTransferOrigemId.options.length > 0)
    newTransferOrigemId.selectedIndex = 0;
  if (newTransferDestinoId.options.length > 0)
    newTransferDestinoId.selectedIndex = 0;

  newTransferDataSaida.value = "";
  newTransferDataEntrada.value = "";

  ModalNovo(); // Assumindo que esta função abre o modal
}

// Função para abrir o modal de edição
async function abrirModalEdicaoTransferencia() {
  const editButton = document.querySelector(".BotaoEditar");
  const transferenciaId = editButton.dataset.transferenciaid; // Pega o ID que você armazenou no botão

  if (!transferenciaId) {
    alert("Nenhuma transferência selecionada para edição.");
    return;
  }

  modalTitle.textContent = "Editar Transferência";
  modalSubmitButton.textContent = "Salvar Alterações";
  transferenciaIdHidden.value = transferenciaId; // Define o ID da transferência sendo editada

  try {
    const response = await fetch(
      `/Transferencia/ObterTransferenciaPorId/${transferenciaId}`
    );
    const data = await response.json();

    if (response.ok) {
      // Preenche os campos do formulário com os dados da transferência
      newTransferAnimalId.value = data.animalId;
      newTransferOrigemId.value = data.instituicaoOrigemId;
      newTransferDestinoId.value = data.instituicaoDestinoId;
      // Datas vêm em formato ISO 8601 do C#, como "2024-06-22T00:00:00" ou "2024-06-22".
      // Para input type="date", o formato YYYY-MM-DD é esperado.
      newTransferDataSaida.value = data.dataSaida
        ? data.dataSaida.split("T")[0]
        : "";
      newTransferDataEntrada.value = data.dataEntrada
        ? data.dataEntrada.split("T")[0]
        : "";

      // Abre o modal
      ModalNovo(); // Assumindo que esta função abre o modal
    } else {
      alert(
        "Erro ao carregar dados da transferência: " +
          (data.message || response.statusText)
      );
    }
  } catch (error) {
    console.error("Erro ao buscar dados da transferência:", error);
    alert("Erro ao conectar com o servidor para obter dados da transferência.");
  }
}

// Função unificada para adicionar ou atualizar transferência
async function handleTransferenciaSubmit() {
  const isEditMode = parseInt(transferenciaIdHidden.value) > 0;

  // Coleta dos valores do modal
  const animalId = newTransferAnimalId.value;
  const origemId = newTransferOrigemId.value;
  const destinoId = newTransferDestinoId.value;
  const dataSaida = newTransferDataSaida.value;
  const dataEntrada = newTransferDataEntrada.value; // Pode ser vazia

  // Validação básica
  if (!animalId || !origemId || !destinoId || !dataSaida) {
    alert(
      "Por favor, preencha todos os campos obrigatórios (Animal, Origem, Destino, Data de Saída)."
    );
    return;
  }

  const dadosDaTransferencia = {
    TransferenciaId: isEditMode ? parseInt(transferenciaIdHidden.value) : 0, // Inclui o ID se for edição
    AnimalId: parseInt(animalId),
    InstituicaoOrigemId: parseInt(origemId),
    InstituicaoDestinoId: parseInt(destinoId),
    DataSaida: dataSaida,
    DataEntrada: dataEntrada || null,
    Status: true,
  };

  const url = isEditMode
    ? "/Transferencia/AtualizarTransferencia"
    : "/Transferencia/CriarTransferencia";
  const method = "POST";

  try {
    const response = await fetch(url, {
      method: method,
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(dadosDaTransferencia),
    });

    const responseData = await response.json();

    if (response.ok && responseData.success) {
      alert(
        responseData.message ||
          (isEditMode
            ? "Transferência atualizada com sucesso!"
            : "Transferência adicionada com sucesso!")
      );
      FecharModalNovo(); // Fecha o modal
      window.location.reload(); // Recarrega a página para ver as mudanças
    } else {
      console.error("Erro na operação de transferência:", responseData);
      alert(
        "Erro na operação de transferência: " +
          (responseData.message || response.statusText)
      );
    }
  } catch (error) {
    console.error("Erro na requisição de transferência:", error);
    alert(
      "Ocorreu um erro ao conectar com o servidor para realizar a operação de transferência: " +
        error.message
    );
  }
}

// --- NOVA FUNÇÃO: CONCLUIR TRANSFERÊNCIA ---
async function concluirTransferencia(transferenciaId) {
  if (
    !confirm(
      `Tem certeza que deseja concluir a transferência ID ${transferenciaId}?`
    )
  ) {
    return; // Usuário cancelou
  }

  try {
    const response = await fetch("/Transferencia/ConcluirTransferencia", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(transferenciaId), // Envia o ID como JSON
    });

    const data = await response.json();

    if (response.ok && data.success) {
      alert(data.message || "Transferência concluída com sucesso!");
      window.location.reload(); // Recarrega a página para refletir as mudanças
    } else {
      console.error("Erro ao concluir transferência:", data.message);
      alert(
        "Erro ao concluir transferência: " +
          (data.message || response.statusText)
      );
    }
  } catch (error) {
    console.error("Erro na requisição para concluir transferência:", error);
    alert(
      "Ocorreu um erro ao conectar com o servidor para concluir a transferência."
    );
  }
}

// Funções de verificação de checkboxes e carrossel (mantenha como estão)
function verificaCheckboxes() {
  const checkboxes = document.querySelectorAll(".atendimento-checkbox");
  const botaoExcluir = document.querySelector(".BotaoExcluir");
  const botaoEditar = document.querySelector(".BotaoEditar");

  let qtdMarcada = 0;
  let idTransferenciaSelecionada = null;

  checkboxes.forEach((checkbox) => {
    if (checkbox.checked) {
      qtdMarcada++;
      idTransferenciaSelecionada = parseInt(checkbox.dataset.atendimentoid);
    }
  });

  if (botaoEditar) {
    if (qtdMarcada <= 1) {
      // Mudança para <= 1 para habilitar se 0 ou 1 estiverem marcados
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
