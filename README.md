# Projeto Inter

Projeto de gerenciamento de zoológicos feito para o Interdisciplinar do 4° semestre da fatec RP.

## 👥 Equipe

- Fábio
- Filiph
- Matheus
- Vitória

## 🚀 Tecnologias utilizadas

- .NET (C#)
- ASP.NET Core
- SQL Server

## ⚙️ Como rodar o projeto

### Para você pegar o repositório

```bash
# Clone o repositório
git clone https://github.com/MatheusPolletti/ProjetoInter.git

# Acesse a pasta
cd ProjetoInter

# Mude a branch da main para a de development
git checkout development

# Atualize a sua branch
git pull origin development

# Crie uma nova branch para a funcionalide que você vai criar e de o nome dela na branch(Não esqueça esse nome)
git checkout -b feature/NomeDaFuncionalidade

### Para rodar ele

# Restaure os pacotes
dotnet restore

# Rode o projeto (2 opções)
- dotnet run
- dotnet watch run

### Colocar o código no git

# Adicione para o índice do git
git add .

# Fazer o commit
git commit -m "Nome explicando a sua mudança do commit"

# Enviar a alteração para o repositório | Lembre de colocar o nome da branch que você criou
git push origin feature/NomeDaFuncionalidade