# Desafio Backend

## **Introdução**
Essa é uma aplicação backend desenvolvida para gerenciar carteiras digitais e transações financeiras. Ela permite a criação de usuários,
consulta de saldo, adição de saldo à carteira, e realização de transferências entre carteiras. A API utiliza autenticação baseada em JWT(JSON Web Token)
para garantir a segurança das operações.
Esta API foi construída como parte de um desafio de Backend, com ênfase em boas práticas, segurança, e organização de código.

## **Como rodar o projeto**
1. Clonar o repositório:
```bash
    git clone https://github.com/jonatasdhs/desafiobackend.git
    cd desafiobackend
```
2. Configurar o `.env` de acordo com o `.env.example`:
```bash
    ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Username=postgres;Password=YourPassword;Database=database;
    JwtSettings__SecretKey="SecretKeyJSONWebTokenPrecisaTerNoMinimo32Caracteres"
    JwtSettings__Issuer="https://localhost:5001"
    JwtSettings__Audience="https://localhost:5001"
```
3. Rodar o docker:
```bash
    docker-compose up --build
```
Isso iniciará os contêineres necessários para o banco de dados PostgreSQL e para a Aplicação. O `docker-compose.yml` está configurado
para iniciar a aplicação, banco de dados e popular com dados fictícios para fins de demonstração.

4. Usando a API via Docker:
Após subir os contêineres, a API estará disponível em `http://localhost:5000`. Você pode fazer requisições HTTP para esse endereço local para interagir com a API.

5. Configuração do Banco de dados (Caso não utilize Docker):
- Crie um banco de dados PostgreSQL
- Atualize as configurações de conexão no arquivo `appsettings.json` com as credencias do banco de dados e as configurações do JWT.
```bash
    "ConnectionStrings": {
    "DefaultConnection": "Host=host;Port=PORT;Database=DATABASE;Username=Username;Password=Password;"
    },
  "JwtSettings": {
    "SecretKey": "SecretKeyJSONWebTokenPrecisaTerNoMinimo32Caracteres",
    "Issuer": "Issuer",
    "Audience": "Audience"
    }
```
6. Rodando a API sem Docker:
- Utilize o comando `dotnet run --project .\DesafioBackend\DesafioBackend.csproj`

## **Funcionalidades**
### **Funcionalidades principais**
- **Autenticação de usuário**:
    - Registro de usuários.
    - Login com JWT para autenticação em rotas protegidas.
- **Carteira**:
    - Consultar saldo da carteira do usuário autenticado
    - Adicionar saldo à carteira do usuário autenticado
- **Transferências**:
    - Realizar transferência para outro usuário.
    - Consultar histórico de transferências, com filtro por período de data.

## **Tecnologias Utilizadas**
- **Backend Framework**: ASP.NET Core 8.0
- **Banco de Dados**: PostgreSQL
- **Autenticação**: JWT (Json Web Token)
- **ORM**: Entity Framework Core
- **Docker**: Para containerização e execução da aplicação.

## **Requisitos**
- **Docker** (Para rodar o banco de dados e a aplicação)

## **Endpoints**
1. **Autenticação**
- **POST /api/auth/register**: Registar um novo usuário.
    - Corpo da requisição:
    ```bash
        {
            "username": "example",
            "email": "example@example.com",
            "password": "Password123!"
        }
    ```
- **POST /api/auth/login**: Realizar login e obter token JWT.
    - Corpo da requisição:
    ```bash
        {
            "email": "example@example.com",
            "password": "Password123!"
        }
    ```
    - Resposta:
    ```bash
        {
            "token": "eyJhbGciOiJIUzI1NiIsInR5"
        }
    ```
2. **Carteira**
- **GET /api/wallet**: Consultar saldo da carteira do usuário autenticado.
    - Cabeçalho:
        - `Authorization: Bearer <token>`

- **POST /api/wallet**: Adicionar saldo à carteira do usuário autenticado.
    - Corpo da requisição:
        ```bash
            {
                "amount": 100.00
            }
        ```

3. **Transferências**
- **POST /api/transfer**: Realizar transferência entre contas.
    - Corpo da requisição:
        ```bash
            {
                "identifier": "username ou email ou id",
                "amount": 50,
                "description": "transferência"
            }
        ```
4. **GET /api/transfer**: Consultar histórico de transferências do usuário autenticado, com filtro opcional por data.
    - Parâmetros:
        - `startDate`: Data de início para o filtro (formato: YYYY-MM-DD).
        - `endDate`: Data de fim para o filtro (formato: YYYY-MM-DD).

## **População inicial de dados**
O projeto inclui um **seeder** que preenche o banco de dados com dados fictícios para fins de demonstração. Os dados incluem usuários, contas e transferências.

## **Testes**
### **Rodando os Testes**

O projeto contém testes para garantir que as funcionalidades da API funcionem conforme o esperado. Para rodar os testes, siga os seguintes passos:
1. **Restaurar dependências do projeto de testes:**
```bash
    dotnet restore
```
2. **Executar os testes:**
```bash
    dotnet test
```

