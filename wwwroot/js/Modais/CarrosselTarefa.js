// Carrossel.js - Controle do carrossel de tarefas pendentes

// Variáveis globais para controle do carrossel
let currentPosition = 0;
const cardWidth = 336; // 320px + 16px gap

function moveEsquerda() {
    const carrossel = document.querySelector('.carrossel-cards-verticais');
    if (!carrossel) return;
    
    const cards = document.querySelectorAll('.card-vertical');
    const maxPosition = (cards.length - 3) * cardWidth * -1;
    
    currentPosition += cardWidth;
    if (currentPosition > 0) currentPosition = maxPosition;
    
    carrossel.style.transform = `translateX(${currentPosition}px)`;
}

function moveDireita() {
    const carrossel = document.querySelector('.carrossel-cards-verticais');
    if (!carrossel) return;
    
    const cards = document.querySelectorAll('.card-vertical');
    const maxPosition = 0;
    
    currentPosition -= cardWidth;
    if (currentPosition < (cards.length - 3) * cardWidth * -1) currentPosition = maxPosition;
    
    carrossel.style.transform = `translateX(${currentPosition}px)`;
}

// Configurar eventos do carrossel
function configurarCarrossel() {
    const btnEsquerda = document.getElementById('btn-carrossel-esquerda');
    const btnDireita = document.getElementById('btn-carrossel-direita');
    
    if (btnEsquerda) {
        btnEsquerda.addEventListener('click', moveEsquerda);
    }
    
    if (btnDireita) {
        btnDireita.addEventListener('click', moveDireita);
    }
    
    const container = document.querySelector('.container-carrossel');
    if (container) {
        container.addEventListener('mouseenter', function() {
            const botoes = document.querySelectorAll('.botoes-carrossel > i');
            botoes.forEach(botao => botao.style.opacity = '0.7');
        });
        
        container.addEventListener('mouseleave', function() {
            const botoes = document.querySelectorAll('.botoes-carrossel > i');
            botoes.forEach(botao => botao.style.opacity = '0');
        });
    }
}

// Inicialização
document.addEventListener('DOMContentLoaded', function() {
    configurarCarrossel();
    console.log('Carrossel inicializado');
});