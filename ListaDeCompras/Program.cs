using System;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using BCrypt.Net;



namespace ListaDeCompras
{
    class Program
    {
        //Classes
        public class Produto
        {
            public float Valor { get; set; }
            public string Nome { get; set; }
            public string Marca { get; set; }

            public Produto(float valor, string nome, string marca)
            {
                Valor = valor;
                Nome = nome;
                Marca = marca;
            }

        }
        public class ListaDeCompras
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public DateTime DataCriacao { get; set; }
            public List<ItemListaDeCompras> Itens { get; set; }

            public ListaDeCompras()
            {
                Itens = new List<ItemListaDeCompras>();
            }

            // Métodos adicionais conforme necessário
        }
        public class ItemListaDeCompras
        {
            public int ListaId { get; set; }
            public ListaDeCompras Lista { get; set; } // Referência à lista de compras
            public int ProdutoId { get; set; }
            public Produto Produto { get; set; } // Referência ao produto
            public int Quantidade { get; set; }
            public float ValorTotal { get; set; }

            // Métodos adicionais conforme necessário
        }
        //Métodos

        //Método do Menu Inicial
        static void AbreMenu(string connectionString)
        {
            Console.WriteLine("O que você gostaria de fazer?");
            Console.WriteLine("1. Trabalhar com produtos\n2. Trabalhar com listas de compras");
            Console.Write("Escolha uma opção: ");
            string escolha = Console.ReadLine();

            switch (escolha)
            {
                case "1":
                    // Lógica para trabalhar com produtos
                    GerenciarProdutos(connectionString);
                    break;
                case "2":
                    // Lógica para trabalhar com listas de compras
                    GerenciarListasDeCompras(connectionString);
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
        //Método de Login
        static void Login(string connectionString)
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Senha: ");
            string senha = LerSenhaOcultando();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var sql = "SELECT Senha FROM Usuarios WHERE Username = @Username";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        var hashSenha = command.ExecuteScalar()?.ToString();

                        if (hashSenha != null && BCrypt.Net.BCrypt.Verify(senha, hashSenha))
                        {
                            Console.WriteLine("Login bem-sucedido!");
                            AbreMenu(connectionString);

                        }
                        else
                        {
                            Console.WriteLine("Login falhou. Username ou senha incorretos.");
                            Login(connectionString);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }
        //Método para trabalhar com os produtos
        static void RegistrarNovoUsuario(string connectionString)
        {
            Console.Write("Escolha um username: ");
            string username = Console.ReadLine();
            Console.Write("Escolha uma senha: ");
            string senha = LerSenhaOcultando();

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var sql = "INSERT INTO Usuarios (Username, Senha) VALUES (@Username, @Senha)";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Senha", senhaHash);
                        command.ExecuteNonQuery();
                        Console.WriteLine("Usuário registrado com sucesso!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }
        static void GerenciarProdutos(string connectionString)
        {
            // Aqui você pode adicionar uma lógica para escolher entre criar um novo produto ou atualizar um existente
            Console.WriteLine("O que você gostaria de fazer?");
            Console.WriteLine("1. Criar novo produto\n2. Atualizar produto existente");
            Console.Write("Escolha uma opção: ");
            string escolha = Console.ReadLine();

            switch (escolha)
            {
                case "1":
                    Produto novoProduto = CriarProduto();
                    Console.WriteLine(novoProduto.ToString());
                    InserirProduto(novoProduto, connectionString);
                    break;
                case "2":
                    AtualizarProduto(connectionString);
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
        //Método para trabalhar com as listas
        static void GerenciarListasDeCompras(string connectionString)
        {
            Console.WriteLine("O que você gostaria de fazer?");
            Console.WriteLine("1. Criar nova lista de compras\n2. Atualizar lista de compras existente");
            Console.Write("Escolha uma opção: ");
            string escolha = Console.ReadLine();

            switch (escolha)
            {
                case "1":
                    ListaDeCompras novaLista = CriarLista();
                    Console.WriteLine(novaLista.ToString());
                    InserirLista(novaLista, connectionString);
                    break;
                case "2":
                    AtualizarLista(connectionString);
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }

        }
        //Mostrar as listas para o usuário
        static void ListarListas(string connectionString)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Listas";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"ID: {reader["Id"]}, Nome: {reader["Nome"]}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }
        //Criar Lista
        static ListaDeCompras CriarLista()
        {
            Console.Write("Digite o nome da nova lista de compras: ");
            string nome = Console.ReadLine();

            ListaDeCompras novaLista = new ListaDeCompras
            {
                Nome = nome
            };
            return novaLista;
        }
        //Inserir Lista no Banco
        static void InserirLista(ListaDeCompras lista, string connectionString)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var sql = "INSERT INTO Listas (Nome) VALUES (@Nome)";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Nome", lista.Nome);
                        command.ExecuteNonQuery();
                        Console.WriteLine("Lista inserida com sucesso!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro ao inserir a lista: {ex.Message}");
            }
        }
        //Atualizar informações da lista
        static void AtualizarLista(string connectionString) 
        {
            Console.WriteLine("O que você gostaria de fazer?");
            Console.WriteLine("1. Adicionar Produtos a Lista\n2. Alterar nome da lista");
            Console.Write("Escolha uma opção: ");
            string escolha = Console.ReadLine();

            switch (escolha)
            {
                case "1":
                    // Lógica para trabalhar com produtos
                    AdicionarItemALista(connectionString);
                    break;
                case "2":
                    // Lógica para trabalhar com listas de compras
                    MudarNomeLista(connectionString);
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
        static void MudarNomeLista(string connectionString) 
        {
            {
                ListarListas(connectionString);
                Console.Write("Digite o ID da lista que você deseja atualizar: ");
                int id;
                while (!int.TryParse(Console.ReadLine(), out id))
                {
                    Console.Write("ID inválido. Por favor, digite um número de ID válido: ");
                }

                Console.Write("Digite o novo nome da lista: ");
                string novoNome = Console.ReadLine();

                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        var sql = "UPDATE Listas SET Nome = @Nome WHERE Id = @Id";

                        using (var command = new MySqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@Nome", novoNome);
                            command.Parameters.AddWithValue("@Id", id);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                                Console.WriteLine("Lista atualizada com sucesso!");
                            else
                                Console.WriteLine("Lista não encontrada.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocorreu um erro ao atualizar a lista: {ex.Message}");
                }
            }
        }
        //Método para criar o usuário criar um produto
        static Produto CriarProduto()
        {
            Console.Write("Digite o nome do produto: ");
            string nome = Console.ReadLine();

            Console.Write("Digite a marca do produto: ");
            string marca = Console.ReadLine();

            Console.Write("Digite o valor do produto: ");
            float valor;
            while (!float.TryParse(Console.ReadLine(), out valor))
            {
                Console.Write("Valor inválido. Por favor, digite um valor numérico para o produto: ");
            }
            Produto novoProduto = new Produto(valor, nome, marca);
            return novoProduto;
        }
        //Método para inserir o produto no banco de dados
        static void InserirProduto(Produto produto, string connectionString)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Produtos (Valor, Nome, Marca) VALUES (@Valor, @Nome, @Marca)";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Valor", produto.Valor);
                        command.Parameters.AddWithValue("@Nome", produto.Nome);
                        command.Parameters.AddWithValue("@Marca", produto.Marca);

                        int result = command.ExecuteNonQuery();

                        if (result < 0)
                            Console.WriteLine("Erro ao inserir dados no banco de dados.");
                        else
                            Console.WriteLine("Produto inserido com sucesso!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }
        //Método para listar todos os produtos
        static void ListarProdutos(string connectionString)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Produtos";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"ID: {reader["Id"]}, Nome: {reader["Nome"]}, Marca: {reader["Marca"]}, Valor: {reader["Valor"]}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }
        //Método que permite atualizar um dos produtos do banco de dados.
        static void AtualizarProduto(string connectionString)
        {
            // Primeiro, listamos os produtos disponíveis
            ListarProdutos(connectionString);

            // Solicita ao usuário para escolher o ID do produto a ser alterado
            Console.Write("\nDigite o ID do produto que você deseja atualizar: ");
            int produtoId;
            while (!int.TryParse(Console.ReadLine(), out produtoId))
            {
                Console.Write("ID inválido. Por favor, digite um número de ID válido: ");
            }

            // Escolhe qual informação atualizar
            Console.WriteLine("Qual informação você gostaria de atualizar?");
            Console.WriteLine("1. Nome\n2. Marca\n3. Valor");
            Console.Write("Escolha uma opção (1-3): ");
            string opcao = Console.ReadLine();

            string campoParaAtualizar = "";
            string novoValor = "";
            float novoValorFloat = 0; // Declarado aqui para evitar o erro CS0136

            switch (opcao)
            {
                case "1":
                    campoParaAtualizar = "Nome";
                    Console.Write("Digite o novo nome: ");
                    novoValor = Console.ReadLine();
                    break;
                case "2":
                    campoParaAtualizar = "Marca";
                    Console.Write("Digite a nova marca: ");
                    novoValor = Console.ReadLine();
                    break;
                case "3":
                    campoParaAtualizar = "Valor";
                    Console.Write("Digite o novo valor: ");
                    while (!float.TryParse(Console.ReadLine(), out novoValorFloat))
                    {
                        Console.Write("Valor inválido. Por favor, digite um valor numérico: ");
                    }
                    novoValor = novoValorFloat.ToString(); // Não há redeclaração de novoValorFloat, apenas uso
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    return;
            }

            // Atualiza o produto no banco de dados
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = $"UPDATE Produtos SET {campoParaAtualizar} = @novoValor WHERE Id = @produtoId";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@novoValor", novoValor);
                        command.Parameters.AddWithValue("@produtoId", produtoId);

                        int result = command.ExecuteNonQuery();

                        if (result < 0)
                            Console.WriteLine("Erro ao atualizar o produto no banco de dados.");
                        else
                            Console.WriteLine("Produto atualizado com sucesso!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }
        //Método para cobrir a senha digitada
        static string LerSenhaOcultando()
        {
            string senha = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    senha += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(senha))
                    {
                        // Remove o último caractere da senha se o usuário pressionar Backspace
                        senha = senha.Substring(0, senha.Length - 1);
                        // Remover o último asterisco da tela
                        Console.Write("\b \b");
                    }
                }
                info = Console.ReadKey(true);
            }
            // Uma nova linha para a saída após pressionar Enter
            Console.WriteLine();
            return senha;
        }
        //Método para selecionar a lista de compras
        static int SelecionarListaDeCompras(string connectionString)
        {
            ListarListas(connectionString);
            Console.Write("Digite o ID da lista de compras que você deseja adicionar itens: ");
            if (int.TryParse(Console.ReadLine(), out int listaId))
            {
                return listaId;
            }
            else
            {
                Console.WriteLine("ID inválido.");
                return -1; // Indica falha na seleção
            }
        }
        //Método para selecionar o produto para a lista de compras
        static (int produtoId, float valor) SelecionarProduto(string connectionString)
        {
            ListarProdutos(connectionString);
            Console.Write("Digite o ID do produto que você deseja adicionar à lista: ");
            if (int.TryParse(Console.ReadLine(), out int produtoId))
            {
                // Busca o valor do produto
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        var sql = "SELECT Valor FROM Produtos WHERE Id = @ProdutoId";
                        using (var command = new MySqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@ProdutoId", produtoId);
                            var resultado = command.ExecuteScalar();
                            if (resultado != null)
                            {
                                float valor = Convert.ToSingle(resultado);
                                return (produtoId, valor);
                            }
                            else
                            {
                                Console.WriteLine("Produto não encontrado.");
                                return (-1, 0); // Indica falha na busca
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao buscar o valor do produto: {ex.Message}");
                    return (-1, 0); // Indica falha na busca
                }
            }
            else
            {
                Console.WriteLine("ID inválido.");
                return (-1, 0); // Indica falha na entrada
            }
        }
        //Método para adicionar quantidade desejada para o produto
        static int ObterQuantidadeProduto()
        {
            Console.Write("Digite a quantidade do produto: ");
            if (int.TryParse(Console.ReadLine(), out int quantidade))
            {
                return quantidade;
            }
            else
            {
                Console.WriteLine("Quantidade inválida.");
                return -1; // Indica falha na entrada
            }
        }
        //Método para adicionar os itens para o banco
        static void AdicionarProdutoALista(string connectionString, int listaId, int produtoId, int quantidade, float valor)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var sql = "INSERT INTO ListaProdutos (ListaId, ProdutoId, Quantidade,Valor_Total) VALUES (@ListaId, @ProdutoId, @Quantidade,@Valor_Total)";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ListaId", listaId);
                        command.Parameters.AddWithValue("@ProdutoId", produtoId);
                        command.Parameters.AddWithValue("@Quantidade", quantidade);
                        command.Parameters.AddWithValue("@Valor_Total", valor*quantidade);
                        command.ExecuteNonQuery();
                        Console.WriteLine("Produto adicionado à lista com sucesso!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar produto à lista: {ex.Message}");
            }
        }
        //Método para utilizar os métodos em conjunto referentes aos itens da lista de compras
        static void AdicionarItemALista(string connectionString)
        {
            int listaId = SelecionarListaDeCompras(connectionString);
            if (listaId == -1) return; // Falha na seleção da lista

            var (produtoId, valor) = SelecionarProduto(connectionString);
            if (produtoId == -1) return; // Falha na seleção do produto

            int quantidade = ObterQuantidadeProduto();
            if (quantidade == -1) return; // Falha na especificação da quantidade

            AdicionarProdutoALista(connectionString, listaId, produtoId, quantidade, valor);
        }






        //Aplicação
        static void Main(string[] args)
        {
            string connectionString = $"Server=127.0.0.1; Database=listadecompras; User Id=root; Password=;";
            Console.WriteLine("1. Login\n2. Registrar novo usuário");
            Console.Write("Escolha uma opção: ");
            var escolha = Console.ReadLine();

            switch (escolha)
            {
                case "1":
                    Login(connectionString);
                    break;
                case "2":
                    RegistrarNovoUsuario(connectionString);
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;

            }
        }


    }
}