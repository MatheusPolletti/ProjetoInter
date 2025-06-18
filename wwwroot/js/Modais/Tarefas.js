document.addEventListener("DOMContentLoaded", function () {
    setFuncionarioId();

    // Abrir modal nova tarefa
    document.querySelector("#btn-nova-tarefa").addEventListener("click", function () {
        const modal = document.querySelector("#modalNovaTarefa");
        modal.style.display = "flex";
    });

    // Fechar modal nova tarefa (caso clique no X)
    window.FecharModalNovaTarefa = function () {
        document.querySelector("#modalNovaTarefa").style.display = "none";
    };

    // Submeter form nova tarefa
    const form = document.querySelector("#formNovaTarefa");
    if (form) {
        form.addEventListener("submit", function (e) {
            e.preventDefault();

            const data = {
                Descricao: form.Descricao.value,
                Observacoes: form.Observacoes.value,
                AnimalId: parseInt(form.AnimalId.value),
                DataProcedimento: form.DataProcedimento.value,
                Status: form.Status.checked
            };

            fetch("/Tarefa/CriarTarefa", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(data)
            })
                .then(resp => resp.json())
                .then(resp => {
                    if (resp.success) {
                        alert("Tarefa criada com sucesso!");
                        location.reload();
                    } else {
                        alert("Erro: " + resp.message);
                    }
                })
                .catch(err => {
                    console.error("Erro na requisição:", err);
                    alert("Erro ao criar tarefa.");
                });
        });
    }
});

function setFuncionarioId() {
    const funcionarioId = document.querySelector("#funcionario-id")?.value;
    const input = document.querySelector('input[name="FuncionarioTarefaId"]');
    if (funcionarioId && input) {
        input.value = funcionarioId;
    }
}

function excluirTarefa(id) {
    if (!confirm("Deseja realmente excluir esta tarefa?")) return;

    fetch("/Tarefa/ExcluirTarefa", {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: `id=${id}`
    })
        .then(res => res.json())
        .then(res => {
            if (res.success) {
                alert("Tarefa excluída!");
                location.reload();
            } else {
                alert("Erro: " + res.message);
            }
        })
        .catch(err => {
            console.error("Erro ao excluir:", err);
            alert("Erro ao excluir tarefa.");
        });
}
