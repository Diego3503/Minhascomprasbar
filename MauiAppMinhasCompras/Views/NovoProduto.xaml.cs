using MauiAppMinhasCompras.Helpers;
using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views
{
    public partial class NovoProduto : ContentPage
    {
        private SQLiteDatabaseHelper _databaseHelper;
        public ObservableCollection<Produto> Produtos { get; set; }

        public NovoProduto()
        {
            InitializeComponent();

            // Caminho para o banco de dados SQLite
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "produtos.db3");

            // Inicializa o helper do banco de dados
            _databaseHelper = new SQLiteDatabaseHelper(dbPath);

            // Vincula a ObservableCollection ao ListView
            BindingContext = this;

            // Carrega todos os produtos inicialmente
            LoadProdutosAsync();
        }

        // Método assíncrono para carregar os produtos do banco
        private async Task LoadProdutosAsync()
        {
            var produtos = await _databaseHelper.GetAll();
            Produtos = new ObservableCollection<Produto>(produtos);
            produtosListView.ItemsSource = Produtos;
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
                Produtos = new ObservableCollection<Produto>(resultados);
                produtosListView.ItemsSource = Produtos;
            }
        }

       
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            
            var produto = new Produto
            {
                Descricao = txt_descricao.Text,  
                Quantidade = double.Parse(txt_quantidade.Text),  
                Preco = double.Parse(txt_preco.Text)  
            };

            
            await _databaseHelper.Insert(produto);

            
            await LoadProdutosAsync();
        }
    }
}
