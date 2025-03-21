using MauiAppMinhasCompras.Helpers;
using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views
{
    public partial class ListaProduto : ContentPage
    {
        private readonly SQLiteDatabaseHelper _databaseHelper;
        public ObservableCollection<Produto> Produtos { get; set; }
        public double TotalGeral { get; set; }

        public ListaProduto()
        {
            InitializeComponent();


            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "produtos.db3");
            _databaseHelper = new SQLiteDatabaseHelper(dbPath);


            Produtos = new ObservableCollection<Produto>();
            lst_produtos.ItemsSource = Produtos;


            LoadProdutosAsync();
        }


        private async Task LoadProdutosAsync()
        {
            try
            {
                var produtos = await _databaseHelper.GetAll();
                Produtos.Clear();
                foreach (var produto in produtos)
                {
                    Produtos.Add(produto);
                }


                AtualizarTotalGeral();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao carregar os produtos: {ex.Message}", "OK");
            }
        }


        private void AtualizarTotalGeral()
        {
            TotalGeral = Produtos.Sum(p => p.Preco * p.Quantidade);
        }


        private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var pesquisa = e.NewTextValue?.ToLower();
            if (string.IsNullOrEmpty(pesquisa))
            {
                await LoadProdutosAsync();
            }
            else
            {
                var resultados = await _databaseHelper.Search(pesquisa);
                Produtos.Clear();
                foreach (var produto in resultados)
                {
                    Produtos.Add(produto);
                }
            }
        }


        private void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            AtualizarTotalGeral();
            DisplayAlert("Total", $"O total geral é: {TotalGeral:C}", "OK");
        }


        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NovoProduto());
        }


        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.BindingContext is Produto produto)
            {
                bool confirmacao = await DisplayAlert("Confirmar Exclusão",
                    $"Tem certeza que deseja excluir {produto.Descricao}?", "Sim", "Não");

                if (confirmacao)
                {
                    int resultado = await _databaseHelper.Delete(produto);
                    if (resultado > 0)
                    {
                        Produtos.Remove(produto);
                        await DisplayAlert("Sucesso", "Produto removido com sucesso!", "OK");
                        await LoadProdutosAsync();
                    }
                    else
                    {
                        await DisplayAlert("Erro", "Falha ao remover produto. Tente novamente.", "OK");
                    }
                }
            }
        }


        private async void RemoverProduto_Clicked(object sender, EventArgs e)
        {
            if (sender is ImageButton botao && botao.CommandParameter is Produto produtoSelecionado)
            {
                bool confirmacao = await DisplayAlert("Confirmar Exclusão",
                    $"Tem certeza que deseja excluir {produtoSelecionado.Descricao}?", "Sim", "Não");

                if (confirmacao)
                {
                    int resultado = await _databaseHelper.Delete(produtoSelecionado);
                    if (resultado > 0)
                    {
                        Produtos.Remove(produtoSelecionado);
                        await DisplayAlert("Sucesso", "Produto removido com sucesso!", "OK");
                        await LoadProdutosAsync();
                    }
                    else
                    {
                        await DisplayAlert("Erro", "Falha ao remover produto. Tente novamente.", "OK");
                    }
                }
            }
        }
    }
}
