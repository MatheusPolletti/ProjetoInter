const qtdCarrossel = document.querySelectorAll(".carrossel-cards-verticais > div");
let x = 0;

function moveDireita(){
    x++;
    if(x>qtdCarrossel.length-1){
        x=0;
    }
    const carrossel = document.querySelector(".carrossel-cards-verticais");
    carrossel.style.transform = `translateX(-${336*x}px)`;
    console.log(x);
}
function moveEsquerda(){
    x--;
    if(x<0){
        x=qtdCarrossel.length-1;
    }
    const carrossel = document.querySelector(".carrossel-cards-verticais");
    carrossel.style.transform = `translateX(-${336*x}px)`;
    console.log(x);
}


