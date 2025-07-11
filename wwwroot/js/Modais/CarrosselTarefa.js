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