// Arquivo: wwwroot/js/Setor/SetorScript.js

// --- Referências aos elementos do HTML (Modais e Botões) ---

// Modais
const modalNovoSetor = document.querySelector('.area-modal-novo');
const modalEditarSetor = document.querySelector('.area-modal-editar');

// Campos do Modal de NOVO Setor
const inputSetorNomeNovo = document.getElementById('inputSetorNomeNovo');
const inputSetorDescricaoNovo = document.getElementById('inputSetorDescricaoNovo');
const selectSetorStatusNovo = document.getElementById('selectSetorStatusNovo');

// Campos do Modal de EDITAR Setor
const inputSetorIdEditar = document.getElementById('inputSetorIdEditar');
const inputSetorNomeEditar = document.getElementById('inputSetorNomeEditar');
const inputSetorDescricaoEditar = document.getElementById('inputSetorDescricaoEditar');
const selectSetorStatusEditar = document.getElementById('selectSetorStatusEditar');

// Botões do Header
const botaoExcluir = document.querySelector('.BotaoExcluir');
const botaoEditar = document.querySelector('.BotaoEditar');


// --- Funções de Abrir/Fechar Modais ---

function abrirModalNovoSetor() {
    // Limpa os campos do formulário "Novo"
    inputSetorNomeNovo.value = '';
    inputSetorDescricaoNovo.value = '';
    selectSetorStatusNovo.value = 'true'; // Padrão: Ativo
    
    modalNovoSetor.style.display = 'flex'; // Abre o modal
}

function FecharModalNovoSetor() {
    modalNovoSetor.style.display = 'none'; // Fecha o modal
}

async function abrirModalEditarSetor() {
    const setorId = botaoEditar.dataset.setorid; // Pega o ID armazenado no botão "Editar"

    if (!setorId) {
        alert("Nenhum setor selecionado para edição.");
        return;
    }

    try {
        const response = await fetch(`/Setor/ObterSetorPorId/${setorId}`);
        const data = await response.json();

        if (response.ok) {
            // Preenche os campos do formulário "Editar" com os dados do setor
            inputSetorIdEditar.value = data.setorId; // Certifique-se que o nome da propriedade bate (setorId vs SetorId)
            inputSetorNomeEditar.value = data.nome;
            inputSetorDescricaoEditar.value = data.descricao;
            // Converte o booleano do C# para a string "true" ou "false" esperada pelo <select>
            selectSetorStatusEditar.value = data.status.toString(); 

            modalEditarSetor.style.display = 'flex'; // Abre o modal de edição
        } else {
            alert("Erro ao carregar dados do setor: " + (data.message || response.statusText));
        }
    } catch (error) {
        console.error("Erro ao buscar dados do setor:", error);
        alert("Erro ao conectar com o servidor para obter dados do setor.");
    }
}

function FecharModalEditarSetor() {
    modalEditarSetor.style.display = 'none'; // Fecha o modal
}


// --- Funções de Submissão de Formulário (Novo e Editar) ---

async function submeterNovoSetor() {
    const nome = inputSetorNomeNovo.value;
    const descricao = inputSetorDescricaoNovo.value;
    const status = selectSetorStatusNovo.value === 'true'; // Converte string para booleano

    // Validação básica
    if (!nome || !descricao) {
        alert("Por favor, preencha o nome e a descrição do setor.");
        return;
    }

    const dadosDoSetor = {
        Nome: nome,
        Descricao: descricao,
        Status: status,
        // InstituicaoPertenceId: Se essa informação precisa ser enviada, você deve coletá-la de algum lugar
        // Por exemplo, de um campo hidden na view ou defini-la no backend com base no usuário logado.
        // Exemplo: InstituicaoPertenceId: 1, // Ou coletar de um campo input hidden
    };

    try {
        const response = await fetch("/Setor/CriarSetor", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(dadosDoSetor),
        });

        const responseData = await response.json();

        if (response.ok && responseData.success) {
            alert(responseData.message || "Setor adicionado com sucesso!");
            FecharModalNovoSetor(); // Fecha o modal
            window.location.reload(); // Recarrega a página para ver as mudanças
        } else {
            console.error("Erro ao adicionar setor:", responseData);
            alert("Erro ao adicionar setor: " + (responseData.message || response.statusText));
        }
    } catch (error) {
        console.error("Erro na requisição de criação de setor:", error);
        alert("Ocorreu um erro ao conectar com o servidor para adicionar o setor: " + error.message);
    }
}

async function submeterEditarSetor() {
    const setorId = parseInt(inputSetorIdEditar.value);
    const nome = inputSetorNomeEditar.value;
    const descricao = inputSetorDescricaoEditar.value;
    const status = selectSetorStatusEditar.value === 'true'; // Converte string para booleano

    // Validação básica
    if (!setorId || setorId <= 0 || !nome || !descricao) {
        alert("Dados inválidos para edição do setor.");
        return;
    }

    const dadosDoSetor = {
        SetorId: setorId,
        Nome: nome,
        Descricao: descricao,
        Status: status,
        // InstituicaoPertenceId: Se essa informação precisa ser enviada/mantida, inclua aqui
    };

    try {
        const response = await fetch("/Setor/AtualizarSetor", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(dadosDoSetor),
        });

        const responseData = await response.json();

        if (response.ok && responseData.success) {
            alert(responseData.message || "Setor atualizado com sucesso!");
            FecharModalEditarSetor(); // Fecha o modal
            window.location.reload(); // Recarrega a página para ver as mudanças
        } else {
            console.error("Erro ao atualizar setor:", responseData);
            alert("Erro ao atualizar setor: " + (responseData.message || response.statusText));
        }
    } catch (error) {
        console.error("Erro na requisição de atualização de setor:", error);
        alert("Ocorreu um erro ao conectar com o servidor para atualizar o setor: " + error.message);
    }
}


// --- Funções de Seleção de Checkbox e Gerenciamento de Botões ---

function verificaCheckboxesSetor() {
    const checkboxes = document.querySelectorAll('.setor-checkbox');
    
    let qtdMarcada = 0;
    let idSetorSelecionado = null; // Para guardar o ID do setor se apenas um estiver marcado

    checkboxes.forEach(checkbox => {
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
        if (qtdMarcada === 1) {
            botaoEditar.disabled = false;
            // Armazena o ID do setor selecionado no botão "Editar" para uso futuro
            botaoEditar.dataset.setorid = idSetorSelecionado; 
        } else {
            botaoEditar.disabled = true;
            // Limpa o ID armazenado se mais de um ou nenhum estiver selecionado
            delete botaoEditar.dataset.setorid; 
        }
        // Opcional: Adicione estilos para feedback visual de habilitado/desabilitado
        botaoEditar.style.opacity = botaoEditar.disabled ? '0.5' : '1';
        botaoEditar.style.cursor = botaoEditar.disabled ? 'not-allowed' : 'pointer';
    }
}

// --- Função de Exclusão ---

async function excluirSetoresSelecionados() {
    const checkboxesMarcados = document.querySelectorAll('.setor-checkbox:checked');
    if (checkboxesMarcados.length === 0) {
        alert("Nenhum setor selecionado para exclusão.");
        return;
    }

    const confirmacao = confirm(`Tem certeza que deseja excluir ${checkboxesMarcados.length} setor(es)?`);
    if (!confirmacao) {
        return; // Usuário cancelou
    }

    const idsParaExcluir = Array.from(checkboxesMarcados).map(cb =>
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
            alert(data.message || "Setor(es) excluído(s) com sucesso!");
            window.location.reload(); // Recarrega a página para ver as mudanças
        } else {
            console.error("Erro ao excluir setor(es):", data.message);
            alert("Erro ao excluir setor(es): " + (data.message || response.statusText));
        }
    } catch (error) {
        console.error("Erro na requisição de exclusão de setor(es):", error);
        alert("Ocorreu um erro ao conectar com o servidor para excluir setor(es).");
    }
}


// --- Event Listener para carregar a função de verificação de checkboxes ao iniciar a página ---
document.addEventListener('DOMContentLoaded', verificaCheckboxesSetor);


// --- Funções de Carrossel (Mantenha se você realmente tem carrossel na tela de setores) ---
// Se não houver, você pode remover essas funções.
function moveEsquerda() {
    const container = document.querySelector('.carrossel-cards-verticais');
    if (container) {
        container.scrollBy({ left: -300, behavior: 'smooth' });
    }
}

function moveDireita() {
    const container = document.querySelector('.carrossel-cards-verticais');
    if (container) {
        container.scrollBy({ left: 300, behavior: 'smooth' });
    }
}