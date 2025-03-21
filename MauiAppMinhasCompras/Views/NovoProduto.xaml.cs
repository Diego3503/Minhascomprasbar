using MauiAppMinhasCompras.Helpers;
using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiAppMinhasCompras.Views
{
    public partial class NovoProduto : ContentPage
    {
        private readonly SQLiteDatabaseHelper _databaseHelper;
        private Produto produtoAtual;

        public ObservableCollection<Produto> Produtos { get; set; }

        public ICommand DeleteCommand { get; private set; }

        public NovoProduto()
        {
            InitializeComponent();

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "produtos.db3");
            _databaseHelper = new SQLiteDatabaseHelper(dbPath);

            Produtos = new ObservableCollection<Produto>();
            produtosListView.ItemsSource = Produtos;

            DeleteCommand = new Command<Produto>(async (produto) => await ExcluirProduto(produto));

            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadProdutosAsync();
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
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao carregar os produtos: {ex.Message}", "OK");
            }
        }

        private async void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
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

        private async void OnExcluirProduto(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem && swipeItem.BindingContext is Produto produto)
            {
                await ExcluirProduto(produto);
            }
        }

        private async Task ExcluirProduto(Produto produto)
        {
            try
            {
                bool confirmacao = await DisplayAlert("Confirmar Exclusão",
                    $"Tem certeza que deseja excluir {produto.Descricao}?", "Sim", "Não");

                if (confirmacao)
                {
                    await _databaseHelper.Delete(produto);
                    Produtos.Remove(produto);
                    await DisplayAlert("Sucesso", "Produto excluído com sucesso!", "OK");
                    await LoadProdutosAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao excluir o produto: {ex.Message}", "OK");
            }
        }

        private async void IrParaMinhasCompras(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListaProduto());
        }

        private async void CalcularTotal(object sender, EventArgs e)
        {
            double soma = Produtos.Sum(p => p.Preco * p.Quantidade);
            await DisplayAlert("Total dos Produtos", $"O total da compra é: {soma:C}", "OK");

        }

        private async void BtnApagarBanco_Clicked(object sender, EventArgs e)
        {
            bool confirmacao = await DisplayAlert("Confirmação", "Tem certeza que deseja apagar o banco de dados?", "Sim", "Não");

            if (confirmacao)
            {
                bool sucesso = await _databaseHelper.ApagarBancoDeDados();

                if (sucesso)
                    await DisplayAlert("Sucesso", "Banco de dados apagado!", "OK");
                else
                    await DisplayAlert("Erro", "Falha ao apagar banco de dados!", "OK");
            }
        }




        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txt_descricao.Text) ||
                    string.IsNullOrWhiteSpace(txt_quantidade.Text) ||
                    string.IsNullOrWhiteSpace(txt_preco.Text))
                {
                    await DisplayAlert("Erro", "Preencha todos os campos!", "OK");
                    return;
                }

                if (!double.TryParse(txt_quantidade.Text, out double quantidade) || quantidade <= 0)
                {
                    await DisplayAlert("Erro", "Quantidade inválida!", "OK");
                    return;
                }

                if (!double.TryParse(txt_preco.Text, out double preco) || preco <= 0)
                {
                    await DisplayAlert("Erro", "Preço inválido!", "OK");
                    return;
                }

                if (produtoAtual == null)
                {
                    Produto novoProduto = new Produto
                    {
                        Descricao = txt_descricao.Text,
                        Quantidade = quantidade,
                        Preco = preco
                    };

                    await _databaseHelper.Insert(novoProduto);
                    await DisplayAlert("Sucesso", "Produto cadastrado com sucesso!", "OK");
                }
                else
                {
                    produtoAtual.Descricao = txt_descricao.Text;
                    produtoAtual.Quantidade = quantidade;
                    produtoAtual.Preco = preco;

                    await _databaseHelper.Update(produtoAtual);
                    await DisplayAlert("Sucesso", "Produto atualizado com sucesso!", "OK");

                    produtoAtual = null;
                }

                await LoadProdutosAsync();

                txt_descricao.Text = "";
                txt_quantidade.Text = "";
                txt_preco.Text = "";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
            }
        }

        private void OnEditarProduto(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem && swipeItem.BindingContext is Produto produto)
            {
                produtoAtual = produto;
                txt_descricao.Text = produto.Descricao;
                txt_quantidade.Text = produto.Quantidade.ToString();
                txt_preco.Text = produto.Preco.ToString();
            }
        }
    }
}