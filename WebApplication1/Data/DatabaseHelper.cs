using Microsoft.Data.Sqlite;

namespace WebApplication1.Data
{
    public class AtendimentoItem
    {
        public int Id { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Servico { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string FormaPagamento { get; set; } = string.Empty;
        public string DataHora { get; set; } = string.Empty;
    }
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString = "Data Source=barbearia.db";

        public static void InicializarBanco()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string createTablesQuery = @"
                CREATE TABLE IF NOT EXISTS Usuarios (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Login TEXT NOT NULL UNIQUE,
                    Senha TEXT NOT NULL
                );

                INSERT OR IGNORE INTO Usuarios (Id, Login, Senha) 
                VALUES (1, 'admin', '123456');

                CREATE TABLE IF NOT EXISTS MovimentacoesCaixa (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Cliente TEXT NOT NULL,
                    Servico TEXT NOT NULL,
                    Valor REAL NOT NULL,
                    FormaPagamento TEXT NOT NULL,
                    DataHora TEXT NOT NULL
                );
            ";

            using var command = new SqliteCommand(createTablesQuery, connection);
            command.ExecuteNonQuery();
        }

        public static bool ValidarLogin(string login, string senha)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string query = "SELECT COUNT(1) FROM Usuarios WHERE Login = @login AND Senha = @senha";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@login", login);
            command.Parameters.AddWithValue("@senha", senha);

            long resultado = (long)command.ExecuteScalar();
            return resultado > 0;
        }

        public static void RegistrarAtendimento(string cliente, string servico, decimal valor, string formaPagamento)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string query = @"
                INSERT INTO MovimentacoesCaixa (Cliente, Servico, Valor, FormaPagamento, DataHora)
                VALUES (@cliente, @servico, @valor, @formaPagamento, @dataHora)";

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@cliente", cliente);
            command.Parameters.AddWithValue("@servico", servico);
            command.Parameters.AddWithValue("@valor", (double)valor);
            command.Parameters.AddWithValue("@formaPagamento", formaPagamento);
            command.Parameters.AddWithValue("@dataHora", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

            command.ExecuteNonQuery();
        }

        public static List<AtendimentoItem> ListarAtendimentosHoje()
        {
            var lista = new List<AtendimentoItem>();
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string query = "SELECT Id, Cliente, Servico, Valor, FormaPagamento, DataHora FROM MovimentacoesCaixa ORDER BY Id DESC";
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new AtendimentoItem
                {
                    Id = reader.GetInt32(0),
                    Cliente = reader.GetString(1),
                    Servico = reader.GetString(2),
                    Valor = Convert.ToDecimal(reader.GetDouble(3)),
                    FormaPagamento = reader.GetString(4),
                    DataHora = reader.GetString(5)
                });
            }

            return lista;
        }
        public static List<AtendimentoItem> ListarFiadosPendentes()
        {
            var lista = new List<AtendimentoItem>();
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string query = "SELECT Id, Cliente, Servico, Valor, FormaPagamento, DataHora FROM MovimentacoesCaixa WHERE FormaPagamento = 'Fiado' ORDER BY Id DESC";
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new AtendimentoItem
                {
                    Id = reader.GetInt32(0),
                    Cliente = reader.GetString(1),
                    Servico = reader.GetString(2),
                    Valor = Convert.ToDecimal(reader.GetDouble(3)),
                    FormaPagamento = reader.GetString(4),
                    DataHora = reader.GetString(5)
                });
            }

            return lista;
        }

        public static void QuitarFiado(int id, string formaPagamentoQuitacao)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            // Atualiza a forma de pagamento de 'Fiado' para o meio escolhido (ex: Pix, Dinheiro)
            // Assim ele sai da lista de devedores e passa a somar no Total Recebido do Caixa!
            string query = "UPDATE MovimentacoesCaixa SET FormaPagamento = @formaPagamento WHERE Id = @id";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@formaPagamento", formaPagamentoQuitacao);
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }

        public static void DeletarAtendimento(int id)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string query = "DELETE FROM MovimentacoesCaixa WHERE Id = @id";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }
    }
}
