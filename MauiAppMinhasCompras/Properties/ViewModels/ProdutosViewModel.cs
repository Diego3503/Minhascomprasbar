using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.ViewModels
{
    public class Produto
    {
        public string Descricao { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
    }

    public class ProdutosViewModel
    {
        public ObservableCollection<Produto> Produtos { get; set; }

        public ProdutosViewModel()
        {
            Produtos = new ObservableCollection<Produto>
            {
                new Produto { Descricao = "Produto 1", Quantidade = 10, Preco = 15.50m },
                new Produto { Descricao = "Produto 2", Quantidade = 5, Preco = 25.00m }
            };
        }
    }
}
