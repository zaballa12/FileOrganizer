Projeto Acadêmico - ADS FSG - Caxias do Sul

Gestor de arquivos e pastas pessoais para usuários comuns.

# FileOrganizer (MVVM)

**FileOrganizer** é uma ferramenta desktop desenvolvida em **C# (.NET 8)** com **WPF (Windows Presentation Foundation)**, voltada à **organização e gerenciamento de arquivos locais no Windows**.  
O sistema permite **organizar, renomear e identificar arquivos duplicados** de forma automatizada e intuitiva, oferecendo ao usuário uma interface moderna baseada no **Material Design**.

---

## Objetivo

O projeto tem como objetivo **simplificar tarefas manuais de gerenciamento de arquivos**, reduzindo o tempo gasto com atividades repetitivas e melhorando a eficiência no uso de pastas pessoais.  
Além disso, o código é **open source**, incentivando o aprendizado e a contribuição da comunidade de desenvolvedores.

---

## Funcionalidades

### Organizar Pasta
Organiza os arquivos de uma pasta em subpastas automaticamente com base em três critérios:
- **Por formato:** Agrupa por tipo de arquivo (imagens, vídeos, documentos, áudios, compactados, etc).
- **Por prefixo:** Cria pastas com base no início do nome dos arquivos (ex.: `foto_01.jpg`, `foto_02.jpg` → pasta `FOTO`).
- **Por data de criação:** Organiza cronologicamente (ex.: `\2025\01`, `\2025\02`).

**Importante:**  
A organização é feita **somente nos arquivos da pasta selecionada**, sem afetar subpastas existentes.

---

### Renomear em Lote
Permite alterar vários nomes de arquivos de uma só vez.
- Adiciona **prefixos e sufixos** personalizados ou automáticos com base na extensão.  
- Evita sobrescrever arquivos, aplicando numeração automática em nomes repetidos.  
- Exibe **pré-visualização** antes da confirmação.

**Exemplo:**  
`documento.pdf` → `doc_documento_001.pdf`

---

### Buscar Duplicados
Localiza e agrupa arquivos duplicados dentro de uma pasta e suas subpastas.
- **Por nome:** Identifica cópias com nomes semelhantes (ignora sufixos como “(1)” ou “- Cópia”).  
- **Por conteúdo:** Compara o **hash SHA-256** dos arquivos, garantindo precisão total mesmo com nomes diferentes.  
- **Combinação de critérios:** O usuário pode aplicar ambos os métodos para maior eficiência.  
- A exclusão é **segura**, movendo os arquivos selecionados para a Lixeira.  
- O sistema mantém **o arquivo original desmarcado**, identificado pela **data de criação mais antiga**.
 **Importante:**  
A busca é feita **de forma recursiva por todas as subpastas da pasta selecionada**.
---

## Arquitetura e Tecnologias

- **Linguagem:** C# (.NET 8 LTS)  
- **Interface:** WPF (Windows Presentation Foundation)  
- **Padrão de Arquitetura:** MVVM (Model–View–ViewModel)  
- **Bibliotecas Utilizadas:**
  - [MaterialDesignInXAML](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) – Interface moderna e responsiva
  - `System.IO` – Manipulação de arquivos e diretórios
  - `LINQ` – Filtros e consultas de arquivos
  - `SHA256` (System.Security.Cryptography) – Cálculo de hash para detecção de duplicatas

A aplicação é dividida em camadas independentes:
- **Models:** Estruturas de dados e regras de negócio.  
- **ViewModels:** Lógica de controle e comunicação com a interface.  
- **Views:** Páginas XAML responsáveis pela exibição.

---

## Estrutura do Projeto

```
FileOrganizer/
│
├── FileOrganizer.Models/Model            # Modelos de dados
├── FileOrganizer.Models/Services         # Serviços auxiliares (hash, seleção, mensagens)
├── FileOrganizer.Models/RegraDeNegocio   # Toda regras de comportamento e coredenação das estratégias
├── FileOrganizer.ViewModels/             # Lógica de aplicação (MVVM)
├── FileOrganizer.Views/                  # Interface XAML
└── FileOrganizer.sln                     # Solução principal do Visual Studio
```

---

## Como Executar

### Pré-requisitos
- **.NET 8 SDK** instalado  
  Baixe em: https://dotnet.microsoft.com/download/dotnet/8.0
- **Windows 10 ou superior**
- **Visual Studio 2022** (recomendado)

### Passos
1. Clone o repositório:
```
git clone https://github.com/luizzaccani/FileOrganizer.git
```
2. Abra a solução `FileOrganizer.sln` no Visual Studio.  
3. Compile o projeto em modo **Release**.  
4. Execute a aplicação.  

---

## Interface

A interface foi desenvolvida com **Material Design**, priorizando clareza, legibilidade e consistência visual.  
As telas principais seguem o mesmo layout, com:
- Menu lateral fixo  
- Listagem central de arquivos  
- Botões principais de ação no rodapé  

## 🧾 Licença

Este projeto é distribuído sob a licença **MIT**, permitindo o uso livre e modificação do código.  
Consulte o arquivo `LICENSE` para mais informações.

---

## Contribuição

Contribuições são bem-vindas!  
Siga os passos abaixo para colaborar:

1. Faça um fork do repositório  
2. Crie uma branch para sua modificação:
```
git checkout -b minha-alteracao
```
3. Faça o commit das mudanças  
4. Envie um pull request explicando as alterações

---

## Autor

**Luiz Henrique Zaccani Zaballa**  
Desenvolvedor .NET | Análise e Desenvolvimento de Sistemas – FSG  
📍 Caxias do Sul – RS, Brasil  
📧 Contato: LinkedIn em https://www.linkedin.com/in/luiz-henrique-zaccani-zaballa-884a0a1b2/

---

## 🌐 Repositório Oficial

https://github.com/zaballa12/FileOrganizer
