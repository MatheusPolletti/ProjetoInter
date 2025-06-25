# Projeto Inter

Projeto de gerenciamento de zoológicos feito para o Interdisciplinar do 4° semestre da FATEC RP.

## 👥 Equipe

- Fábio Rogério Escábio Júnior
- Filiph Rodrigues Rocha Romão
- Matheus Cauã Polletti
- Vitória Assis de Oliveira

## 🚀 Tecnologias utilizadas

- .NET (C#)
- ASP.NET Core
- Supabase
- MySQL

## ⚙️ Como rodar o projeto

# Puxando o projeto

```bash
# Clone o repositório
git clone https://github.com/MatheusPolletti/ProjetoInter.git

# Acesse a pasta
cd ProjetoInter

# Mude a branch da main para a de development
git checkout development

# Atualize a sua branch
git pull origin development

# Crie uma nova branch para a funcionalidade que você vai criar e dê o nome dela
git checkout -b feature/NomeDaFuncionalidade

# Rodando o projeto

# Restaure os pacotes
dotnet restore

# Rode o projeto (2 opções)
dotnet run
# ou
dotnet watch run

# Adicionado ao git

# Adicione para o índice do git
git add .

# Faça o commit
git commit -m "Nome explicando a sua mudança do commit"

# Enviar a alteração para o repositório | Lembre-se de colocar o nome da branch que você criou
git push origin feature/NomeDaFuncionalidade