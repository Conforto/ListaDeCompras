using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListaDeCompras
{
    internal class Classes
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
        public class connectionStrings 
        {
            public string connection {  get; set; }
        }
    }
}
