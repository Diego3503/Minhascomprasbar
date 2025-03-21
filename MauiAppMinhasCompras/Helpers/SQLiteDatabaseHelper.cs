using MauiAppMinhasCompras.Models;
using SQLite;

namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        private readonly SQLiteAsyncConnection _conn;

        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);
            Task.Run(async () => await InitializeDatabaseAsync()).Wait();
        }


        private async Task InitializeDatabaseAsync()
        {
            await _conn.CreateTableAsync<Produto>();
        }

        public async Task<bool> ApagarBancoDeDados()
        {
            try
            {
                await _conn.CloseAsync(); // Fecha a conexão antes de excluir o arquivo

                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "produtos.db3");

                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao apagar o banco: {ex.Message}");
            }

            return false;
        }



        public async Task<int> Insert(Produto p)
        {
            var produtoExistente = await _conn.Table<Produto>()
                .Where(x => x.Descricao == p.Descricao)
                .FirstOrDefaultAsync();

            if (produtoExistente == null)
            {
                return await _conn.InsertAsync(p);
            }
            return 0;
        }


        public Task<int> Update(Produto p)
        {
            return _conn.UpdateAsync(p);
        }


        public async Task<int> Delete(Produto p)
        {
            if (p.Id <= 0)
                return 0;

            var produtoExistente = await _conn.FindAsync<Produto>(p.Id);
            if (produtoExistente != null)
            {
                return await _conn.DeleteAsync(produtoExistente);
            }
            return 0;
        }


        public Task<List<Produto>> GetAll()
        {
            return _conn.Table<Produto>().ToListAsync();
        }


        public Task<List<Produto>> Search(string q)
        {
            return _conn.Table<Produto>()
                        .Where(p => p.Descricao.Contains(q))
                        .ToListAsync();
        }
    }
}
