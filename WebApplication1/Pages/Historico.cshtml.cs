using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;

namespace WebApplication1.Pages
{
    public class HistoricoModel : PageModel
    {
        public List<AtendimentoItem> Itens { get; set; } = new();
        public string FiltroAtual { get; set; } = "todos";

        public void OnGet(string filtro = "todos")
        {
            CarregarDados(filtro);
        }

        public IActionResult OnPostPagar(int id, string formaPagamento, string filtro)
        {
            if (id > 0 && !string.IsNullOrWhiteSpace(formaPagamento))
            {
                DatabaseHelper.QuitarFiado(id, formaPagamento);
            }

            return RedirectToPage(new { filtro });
        }

        public IActionResult OnPostDeletar(int id, string filtro)
        {
            if (id > 0)
            {
                DatabaseHelper.DeletarAtendimento(id);
            }

            return RedirectToPage(new { filtro });
        }

        private void CarregarDados(string filtro)
        {
            FiltroAtual = filtro;
            var todos = DatabaseHelper.ListarAtendimentosHoje();

            if (filtro == "fiado")
            {
                Itens = todos.Where(x => x.FormaPagamento == "Fiado").ToList();
            }
            else if (filtro == "pagos")
            {
                Itens = todos.Where(x => x.FormaPagamento != "Fiado").ToList();
            }
            else
            {
                Itens = todos;
            }
        }
    }
}