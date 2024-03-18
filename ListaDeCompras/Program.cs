using System;
using MySqlConnector; // Importante usar o namespace correto

namespace ListaDeCompras
{
    class Program
    {
        static void Main(string[] args)
        {
            Produto pao = new Produto(1.99f, "Pão Francês", "Padaria São José");
            string connectionString = "Server=127.0.0.1; Database=listadecompras; User Id=root; Password=;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Produtos (Valor, Nome, Marca) VALUES (@Valor, @Nome, @Marca)";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Valor", pao.Valor);
                        command.Parameters.AddWithValue("@Nome", pao.Nome);
                        command.Parameters.AddWithValue("@Marca", pao.Marca);

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
    }

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

        public override string ToString()
        {
            return $"Nome: {Nome}, Marca: {Marca}, Valor: {Valor}";
        }
    }
}
