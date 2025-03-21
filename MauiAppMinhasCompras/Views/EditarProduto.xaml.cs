using MauiAppMinhasCompras.Helpers;
using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views
{
    public partial class EditarProduto : ContentPage
    {
        private readonly SQLiteDatabaseHelper _databaseHelper;
        private Produto produtoAtual;
        private bool modoEdicao = false;

        public EditarProduto(Produto produto)
        {
            InitializeComponent();

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "produtos.db3");
            _databaseHelper = new SQLiteDatabaseHelper(dbPath);

            produtoAtual = produto;

            // Preenche os campos com os dados do produto
            txtDescricao.Text = produtoAtual.Descricao;
            txtQuantidade.Text = produtoAtual.Quantidade.ToString();
            txtPreco.Text = produtoAtual.Preco.ToString("F2");

            // Inicialmente, os campos ficam desativados até clicar em Editar
            DefinirModoEdicao(false);
        }

        private void DefinirModoEdicao(bool habilitar)
        {
            txtDescricao.IsEnabled = habilitar;
            txtQuantidade.IsEnabled = habilitar;
            txtPreco.IsEnabled = habilitar;
            modoEdicao = habilitar;
        }

        private void EditarProduto_Clicked(object sender, EventArgs e)
        {
            if (!modoEdicao)
            {
                // Habilita a edição dos campos
                DefinirModoEdicao(true);
            }
        }

        private async void SalvarProduto_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDescricao.Text) ||
                    string.IsNullOrWhiteSpace(txtQuantidade.Text) ||
                    string.IsNullOrWhiteSpace(txtPreco.Text))
                {
                    await DisplayAlert("Erro", "Preencha todos os campos!", "OK");
                    return;
                }

                if (!double.TryParse(txtQuantidade.Text, out double quantidade) || quantidade <= 0)
                {
                    await DisplayAlert("Erro", "Quantidade inválida!", "OK");
                    return;
                }

                if (!double.TryParse(txtPreco.Text, out double preco) || preco <= 0)
                {
                    await DisplayAlert("Erro", "Preço inválido!", "OK");
                    return;
                }

                // Atualiza os dados do produto
                produtoAtual.Descricao = txtDescricao.Text;
                produtoAtual.Quantidade = quantidade;
                produtoAtual.Preco = preco;

                // Salva no banco de dados
                await _databaseHelper.Update(produtoAtual);
                await DisplayAlert("Sucesso", "Produto atualizado com sucesso!", "OK");

                // Desativa a edição após salvar
                DefinirModoEdicao(false);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao atualizar produto: {ex.Message}", "OK");
            }
        }

        private async void ExcluirProduto_Clicked(object sender, EventArgs e)
        {
            bool confirmacao = await DisplayAlert("Confirmar Exclusão",
                $"Tem certeza que deseja excluir {produtoAtual.Descricao}?", "Sim", "Não");

            if (confirmacao)
            {
                try
                {
                    await _databaseHelper.Delete(produtoAtual);
                    await DisplayAlert("Sucesso", "Produto excluído com sucesso!", "OK");
                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro", $"Erro ao excluir produto: {ex.Message}", "OK");
                }
            }
        }
    }
}
