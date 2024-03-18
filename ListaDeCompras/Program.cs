using System;
using Microsoft.Data.SqlClient;
using MySqlConnector;

namespace ListaDeCompras
{
    class Program
    {
        //Aplicação
        static void Main(string[] args)
        {
            Console.Write("Por favor, digite a senha do banco de dados: ");
            string dbPassword = Console.ReadLine();
            string connectionString = $"Server=127.0.0.1; Database=listadecompras; User Id=root; Password={dbPassword};";

            // Aqui você pode adicionar uma lógica para escolher entre criar um novo produto ou atualizar um existente
            Console.WriteLine("O que você gostaria de fazer?");
            Console.WriteLine("1. Criar novo produto\n2. Atualizar produto existente");
            Console.Write("Escolha uma opção: ");
            string escolha = Console.ReadLine();

            switch (escolha)
            {
                case "1":
                    Produto novoProduto = CriarProdutoInterativamente();
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

        //Métodos
        static Produto CriarProdutoInterativamente()
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

        //Classes
        class Produto
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
    }
}
