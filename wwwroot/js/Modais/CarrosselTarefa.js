// // Carrossel.js - Controle do carrossel de tarefas pendentes

// // Variáveis globais para controle do carrossel
// let currentPosition = 0;
// const cardWidth = 336; // 320px + 16px gap

// function moveEsquerda() {
//     const carrossel = document.querySelector('.carrossel-cards-verticais');
//     if (!carrossel) return;
    
//     const cards = document.querySelectorAll('.card-vertical');
//     const maxPosition = (cards.length - 3) * cardWidth * -1;
    
//     currentPosition += cardWidth;
//     if (currentPosition > 0) currentPosition = maxPosition;
    
//     carrossel.style.transform = `translateX(${currentPosition}px)`;
// }

// function moveDireita() {
//     const carrossel = document.querySelector('.carrossel-cards-verticais');
//     if (!carrossel) return;
    
//     const cards = document.querySelectorAll('.card-vertical');
//     const maxPosition = 0;
    
//     currentPosition -= cardWidth;
//     if (currentPosition < (cards.length - 3) * cardWidth * -1) currentPosition = maxPosition;
    
//     carrossel.style.transform = `translateX(${currentPosition}px)`;
// }

// // Configurar eventos do carrossel
// function configurarCarrossel() {
//     const btnEsquerda = document.getElementById('btn-carrossel-esquerda');
//     const btnDireita = document.getElementById('btn-carrossel-direita');
    
//     if (btnEsquerda) {
//         btnEsquerda.addEventListener('click', moveEsquerda);
//     }
    
//     if (btnDireita) {
//         btnDireita.addEventListener('click', moveDireita);
//     }
    
//     const container = document.querySelector('.container-carrossel');
//     if (container) {
//         container.addEventListener('mouseenter', function() {
//             const botoes = document.querySelectorAll('.botoes-carrossel > i');
//             botoes.forEach(botao => botao.style.opacity = '0.7');
//         });
        
//         container.addEventListener('mouseleave', function() {
//             const botoes = document.querySelectorAll('.botoes-carrossel > i');
//             botoes.forEach(botao => botao.style.opacity = '0');
//         });
//     }
// }

// // Inicialização
// document.addEventListener('DOMContentLoaded', function() {
//     configurarCarrossel();
//     console.log('Carrossel inicializado');
// });


const qtdCarrossel = document.querySelectorAll(
  ".carrossel-cards-verticais > div"
);

const container = document.querySelector(".container-carrossel");
const ultimoCard = document.querySelector(
  ".carrossel-cards-verticais > div:last-child"
);

let x = 0;

document.addEventListener("DOMContentLoaded", () => {
  const botoes = document.querySelector(".container-botoes");

  if (qtdCarrossel.lenth <= 0) {
    botoes.style.display = "none";
  } else {
    const containerCarrossel = document.querySelector(".container-carrossel");
    const height = containerCarrossel.clientHeight;
    console.log(height);
    botoes.style.top = `${(16 + height / 2 - 25)}px`;
  }
});

function moveDireita() {
  x++;
  if (x > qtdCarrossel.length - 1) {
    x = 0;
  }
  const carrossel = document.querySelector(".carrossel-cards-verticais");
  carrossel.style.transform = `translateX(-${336 * x}px)`;
  console.log(x);
  verificaDireita();
}
function moveEsquerda() {
  x--;
  if (x < 0) {
    x = qtdCarrossel.length - 1;
  }
  const carrossel = document.querySelector(".carrossel-cards-verticais");
  carrossel.style.transform = `translateX(-${336 * x}px)`;
  console.log(x);
  verificaDireita();
}

function retangulosSeSobrepoemComMargem(rect1, rect2, margem = 1) {
  return !(
    rect1.right - margem < rect2.left + margem ||
    rect1.left + margem > rect2.right - margem ||
    rect1.bottom - margem < rect2.top + margem ||
    rect1.top + margem > rect2.bottom - margem
  );
}