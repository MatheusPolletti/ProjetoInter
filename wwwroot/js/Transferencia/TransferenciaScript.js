// Arquivo: wwwroot/js/Transferencia/TransferenciaScript.js

// Referências aos elementos do modal
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

// Função para abrir o modal de novo
function abrirModalNovoTransferencia() {
  modalTitle.textContent = "Adicionar Transferência";
  modalSubmitButton.textContent = "Adicionar";
  transferenciaIdHidden.value = "0"; // Reseta o ID oculto para indicar nova criação

  // Limpa os campos do formulário
  newTransferAnimalId.value = "";
  // Reseta selects para a primeira opção ou para um placeholder
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
    DataEntrada: dataEntrada || null, // Pode ser nulo se a entrada ainda não ocorreu
    Status: true, // Novas transferências começam como pendentes
    // Edições mantêm o status atual ou alteram no backend
    // Para edições, o status é gerenciado pelo backend ou não é enviado pelo modal
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
  const botaoEditar = document.querySelector(".BotaoEditar");

  let qtdMarcada = 0;
  let idTransferenciaSelecionada = null;

  checkboxes.forEach((checkbox) => {
    if (checkbox.checked) {
      qtdMarcada++;
      idTransferenciaSelecionada = parseInt(checkbox.dataset.transferenciaid);
    }
  });

  if (botaoEditar) {
    if (qtdMarcada < 2) {
      botaoEditar.disabled = false;
      botaoEditar.classList.remove("desabilitado");
      botaoEditar.dataset.transferenciaid = idTransferenciaSelecionada;

    } else {
      botaoEditar.disabled = true;
      botaoEditar.classList.add("desabilitado");
      delete botaoEditar.dataset.transferenciaid;

    }
    botaoEditar.style.transition = "all 0.2s ease";
  }
}

document.addEventListener("DOMContentLoaded", verificaCheckboxes);

document.querySelectorAll(".atendimento-checkbox").forEach((checkbox) => {
  checkbox.addEventListener("change", verificaCheckboxes);
});

function abreModalInstituicao() {
  document.querySelector(".area-modal-editar").style.display = "flex";
}

function fecharModalInstituicao() {
  document.querySelector(".area-modal-editar").style.display = "none";
}

async function atualizarDropdown(
  dropdownElement,
  selectedInstituicaoId = null
) {
  if (!dropdownElement) {
    console.error("Elemento dropdown não fornecido para atualização.");
    return;
  }

  try {
    const response = await fetch("/Instituicao/ObterTodasInstituicoes", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    const instituicoes = await response.json();

    if (response.ok) {
      // Limpa as opções existentes no dropdown
      dropdownElement.innerHTML =
        '<option value="">Selecione uma Instituição</option>'; // Adiciona uma opção padrão

      // Adiciona as novas opções ao dropdown
      instituicoes.forEach((instituicao) => {
        const option = document.createElement("option");
        option.value = instituicao.instituicaoId;
        option.textContent = instituicao.nome;

        if (
          selectedInstituicaoId &&
          instituicao.instituicaoId === selectedInstituicaoId
        ) {
          option.selected = true; // Seleciona a recém-adicionada, se for o caso
        }
        dropdownElement.appendChild(option);
      });

      console.log(
        `Dropdown com ID '${dropdownElement.id}' atualizado com sucesso!`
      );
    } else {
      console.error(
        "Erro ao carregar instituições:",
        instituicoes.message || response.statusText
      );
      alert(
        "Erro ao carregar lista de instituições. Tente recarregar a página."
      );
    }
  } catch (error) {
    console.error("Erro na requisição para obter instituições:", error);
    alert(
      "Ocorreu um erro ao conectar com o servidor para obter a lista de instituições."
    );
  }
}

// Modifique a função salvarNovaInstituicao para chamar a nova função
async function salvarNovaInstituicao() {
    // Coleta os valores
    const nome = document.getElementById("instituicaoNome").value;
    const endereco = document.getElementById("instituicaoEndereco").value;
    const contato = document.getElementById("instituicaoContato").value;
    const imagemInput = document.getElementById("imagemInput");
    
    // Validação
    if (!nome || !endereco) {
        alert("Nome e Endereço são obrigatórios");
        return;
    }

    // Cria FormData
    const formData = new FormData();
    formData.append("Nome", nome);
    formData.append("Endereco", endereco);
    formData.append("Telefone", contato || "");
    
    if (imagemInput.files[0]) {
        formData.append("Imagem", imagemInput.files[0]);
    }

    // Adiciona AntiForgeryToken
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    formData.append("__RequestVerificationToken", token);

    try {
        const response = await fetch("/Instituicao/CriarInstituicao", {
            method: "POST",
            body: formData
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || "Erro no servidor");
        }

        const data = await response.json();
        if (data.success) {
            fecharModalInstituicao();
            // Atualiza os dropdowns de instituição
            await atualizarDropdown(document.getElementById("newTransferOrigemId"));
            await atualizarDropdown(document.getElementById("newTransferDestinoId"));
            window.location.reload();
        } else {
            alert(data.message || "Erro ao cadastrar");
        }
    } catch (error) {
        console.error("Erro:", error);
        alert(`Erro: ${error.message}`);
    }
}

function mostrarPreviewEditar(event) {
  const input = event.target;
  const previewContainer = input.closest(".form-imagem");
  const preview = previewContainer.querySelector("img");
  const previewText = previewContainer.querySelector("span");

  if (input.files && input.files[0]) {
    const reader = new FileReader();
    reader.onload = function (e) {
      preview.src = e.target.result;
      preview.style.display = "block";
      if (previewText) previewText.style.display = "none";
    };
    reader.readAsDataURL(input.files[0]);
  }
}

document.querySelector(".BotaoExcluir").addEventListener("click", async function () {
    const checkboxes = document.querySelectorAll(".atendimento-checkbox:checked");

    if (checkboxes.length === 0) {
        alert("Selecione ao menos uma transferência para excluir.");
        return;
    }

    if (!confirm(`Deseja excluir ${checkboxes.length} transferência(s)?`)) {
        return;
    }

    for (const cb of checkboxes) {
        const id = parseInt(cb.dataset.transferenciaid);

        try {
            const response = await fetch("/Transferencia/ExcluirTransferencia", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(id)
            });

            const result = await response.json();

            if (!response.ok || !result.success) {
                alert(`Erro ao excluir ID ${id}: ${result.message || response.statusText}`);
            }
        } catch (error) {
            alert(`Erro ao excluir ID ${id}: ${error.message}`);
        }
    }

    alert("Transferência(s) excluída(s) com sucesso!");
    window.location.reload();
});
