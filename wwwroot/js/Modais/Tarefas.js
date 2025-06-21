document.addEventListener("DOMContentLoaded", function() {
    initializeTarefasModule();
});

function initializeTarefasModule() {
    setFuncionarioId();
    setupModalNovaTarefa(); // Configura apenas o modal de nova tarefa
    setupModalEditarTarefa();
    setupBotoesConcluir();
}

function setFuncionarioId() {
    const funcionarioId = document.querySelector("#funcionario-id")?.value;
    const input = document.querySelector('input[name="FuncionarioTarefaId"]');
    if (funcionarioId && input) {
        input.value = funcionarioId;
    }
}

// Configuração do Modal Nova Tarefa (versão definitiva)
function setupModalNovaTarefa() {
    const btnNovaTarefa = document.getElementById('btn-nova-tarefa');
    const modalNovaTarefa = document.getElementById('modalNovaTarefa');
    const formNovaTarefa = document.getElementById('formNovaTarefa');
    const closeBtn = modalNovaTarefa?.querySelector('.modal-close');

    // Verificação rigorosa dos elementos
    if (!btnNovaTarefa) {
        console.error('Botão "Nova Tarefa" não encontrado!');
        return;
    }
    
    if (!modalNovaTarefa) {
        console.error('Modal "modalNovaTarefa" não encontrado!');
        return;
    }

    // Abrir modal com verificação adicional
    btnNovaTarefa.addEventListener('click', function() {
        console.log('Botão clicado - tentando abrir modal');
        modalNovaTarefa.style.display = "flex";
        console.log('Modal deve estar visível agora');
    });

    // Fechar modal - versão onclick HTML
    window.FecharModalNovaTarefa = function() {
        modalNovaTarefa.style.display = "none";
    };

    // Fechar modal - versão JavaScript
    if (closeBtn) {
        closeBtn.addEventListener('click', function() {
            modalNovaTarefa.style.display = "none";
        });
    }

    // Submeter formulário
    if (formNovaTarefa) {
        formNovaTarefa.addEventListener('submit', function(e) {
            e.preventDefault();
            submitNovaTarefa(this);
        });
    }
}

function submitNovaTarefa(form) {
    const formData = {
        Descricao: form.Descricao.value,
        Observacoes: form.Observacoes.value,
        AnimalId: parseInt(form.AnimalId.value),
        DataProcedimento: form.DataProcedimento.value,
        Status: false // Nova tarefa geralmente começa como não concluída
    };

    fetch("/Tarefa/CriarTarefa", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(formData)
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            alert("Tarefa criada com sucesso!");
            location.reload();
        } else {
            alert("Erro: " + (data.message || "Erro desconhecido"));
        }
    })
    .catch(error => {
        console.error("Erro:", error);
        alert("Erro ao criar tarefa");
    });
}

function setupModalNovaTarefa() {
    document.getElementById('btn-nova-tarefa')?.addEventListener('click', function() {
        document.getElementById('modalNovaTarefa').style.display = "flex";
    });
    
    // Garanta que o botão de fechar funciona
    document.querySelector('.modal-close')?.addEventListener('click', function() {
        document.getElementById('modalNovaTarefa').style.display = "none";
    });
}

// Configuração do Modal Editar Tarefa
function setupModalEditarTarefa() {
    const btnEditar = document.getElementById('btn-editar-tarefa');
    
    if (btnEditar) {
        btnEditar.addEventListener('click', handleEditarClick);
    }
}

function handleEditarClick() {
    const checks = document.querySelectorAll(".selecionar-tarefa:checked");
    
    if (checks.length === 0) {
        return showFeedback("Selecione uma tarefa para editar.", false);
    }
    
    if (checks.length > 1) {
        return showFeedback("Selecione apenas uma tarefa para editar.", false);
    }
    
    const procedimentoId = checks[0].getAttribute('data-id');
    abrirModalEdicaoTarefa(procedimentoId);
}

function abrirModalEdicaoTarefa(procedimentoId) {
    fetch(`/Tarefa/Editar/${procedimentoId}`)
        .then(handleResponse)
        .then(html => {
            if (typeof html !== 'string' || html.includes("InvalidOperationException")) {
                throw new Error("Erro ao carregar formulário de edição");
            }
            
            const container = createModalContainer(html);
            document.body.appendChild(container);
            setupModalEvents(container);
        })
        .catch(error => {
            showFeedback("Falha ao carregar editor de tarefas", false);
            console.error("Erro:", error);
        });
}

function createModalContainer(html) {
    const oldModal = document.getElementById('modal-editar-container');
    if (oldModal) document.body.removeChild(oldModal);
    
    const container = document.createElement('div');
    container.id = 'modal-editar-container';
    container.innerHTML = html;
    return container;
}

function setupModalEvents(container) {
    const modal = container.querySelector('.area-modal-editar');
    if (!modal) return;
    
    modal.style.display = "flex";
    
    const fecharBtn = modal.querySelector('.fechar');
    if (fecharBtn) {
        fecharBtn.onclick = () => document.body.removeChild(container);
    }
    
    const form = modal.querySelector('#formEditarTarefa');
    if (form) {
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            submitEditarTarefa(form);
        });
    }
}

function submitEditarTarefa(form) {
    const formData = new FormData(form);
    const errorsDiv = form.querySelector("#formEditarTarefaErrors");
    
    fetch(form.action, {
        method: 'POST',
        body: formData,
        headers: {
            'RequestVerificationToken': form.querySelector('input[name="__RequestVerificationToken"]').value
        }
    })
    .then(handleResponse)
    .then(data => {
        if (data.success) {
            showFeedback("Tarefa atualizada com sucesso!", true);
        } else {
            errorsDiv.innerHTML = data.message || "Erro ao atualizar tarefa";
        }
    })
    .catch(error => {
        errorsDiv.innerHTML = "Erro ao enviar formulário.";
        console.error("Erro:", error);
    });
}

// Funções Auxiliares
function handleResponse(response) {
    if (!response.ok) {
        return response.json().then(err => {
            throw new Error(err.message || `Erro HTTP: ${response.status}`);
        });
    }
    return response.headers.get('content-type')?.includes('application/json') 
        ? response.json() 
        : response.text();
}

function showFeedback(message, isSuccess) {
    alert(message);
    if (isSuccess) {
        location.reload();
    }
}

// Função de Exclusão
function excluirTarefa(id) {
    if (!confirm("Deseja realmente excluir esta tarefa?")) return;

    fetch("/Tarefa/ExcluirTarefa", {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: `id=${id}`
    })
    .then(handleResponse)
    .then(response => {
        showFeedback(response.success ? "Tarefa excluída!" : response.message, response.success);
    })
    .catch(error => {
        showFeedback("Erro ao excluir tarefa", false);
        console.error("Erro:", error);
    });
}

function setupBotoesConcluir() {
    document.querySelectorAll('.btn-concluir').forEach(btn => {
        btn.addEventListener('click', function() {
            const procedimentoId = this.getAttribute('data-tarefa-id');
            concluirTarefa(procedimentoId);
        });
    });
}

function concluirTarefa(id) {
    if (!confirm("Deseja marcar esta tarefa como concluída?")) return;

    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    
    fetch("/Tarefa/ConcluirTarefa", {
        method: "POST",
        headers: { 
            "Content-Type": "application/json",
            "RequestVerificationToken": token || ''
        },
        body: JSON.stringify(id)
    })
    .then(response => {
        if (!response.ok) {
            return response.text().then(text => {
                throw new Error(text || "Erro ao concluir tarefa");
            });
        }
        return response.json();
    })
    .then(data => {
        if (data && data.success) {
            showFeedback("Tarefa concluída com sucesso!", true);
        } else {
            showFeedback(data?.message || "Erro ao concluir tarefa", false);
        }
    })
    .catch(error => {
        console.error("Erro detalhado:", error);
        showFeedback(error.message || "Erro ao comunicar com o servidor", false);
    });
}
