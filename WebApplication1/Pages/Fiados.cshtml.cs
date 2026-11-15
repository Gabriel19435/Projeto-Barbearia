using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;

namespace WebApplication1.Pages
{
    public class FiadosModel : PageModel
    {
        public List<AtendimentoItem> ListaFiados { get; set; } = new();
        public decimal TotalPendente { get; set; }

        public void OnGet()
        {
            CarregarFiados();
        }

        public IActionResult OnPostQuitar(int id, string formaPagamento)
        {
            if (id > 0 && !string.IsNullOrWhiteSpace(formaPagamento))
            {
                DatabaseHelper.QuitarFiado(id, formaPagamento);
            }

            return RedirectToPage();
        }

        private void CarregarFiados()
        {
            ListaFiados = DatabaseHelper.ListarFiadosPendentes();
            TotalPendente = ListaFiados.Sum(f => f.Valor);
        }
    }
}