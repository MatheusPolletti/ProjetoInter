setTimeout(() => 
{
    const erro = document.querySelector('.Mensagem');
    
    if (erro) 
    {
        erro.style.opacity = '0';
        setTimeout(() => erro.remove(), 150);
    }
}, 4000);

function mostrarPreviewEditar(event)
{
    const file = event.target.files[0];

    const preview = document.getElementById('preview');
    
    const previewText = document.getElementById('previewText');

    if (file)
    {
        const reader = new FileReader();

        reader.onload = function(e) {
            preview.src = e.target.result;
            preview.style.display = 'block';
            previewText.style.display = 'none';
        }
        
        reader.readAsDataURL(file);
    }
}