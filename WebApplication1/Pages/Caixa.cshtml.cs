using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;

namespace WebApplication1.Pages
{
    public class CaixaModel : PageModel
    {
        [BindProperty]
        public string Cliente { get; set; } = string.Empty;

        [BindProperty]
        public string Servico { get; set; } = string.Empty;

        [BindProperty]
        public decimal Valor { get; set; }

        [BindProperty]
        public string FormaPagamento { get; set; } = string.Empty;

        public List<AtendimentoItem> Atendimentos { get; set; } = new();
        public decimal TotalRecebido { get; set; }

        public void OnGet()
        {
            CarregarDados();
        }

        public IActionResult OnPost()
        {
            if (!string.IsNullOrWhiteSpace(Cliente) && !string.IsNullOrWhiteSpace(Servico) && Valor > 0)
            {
                DatabaseHelper.RegistrarAtendimento(Cliente, Servico, Valor, FormaPagamento);
            }

            return RedirectToPage();
        }

        public IActionResult OnPostPagar(int id, string formaPagamento)
        {
            if (id > 0 && !string.IsNullOrWhiteSpace(formaPagamento))
            {
                DatabaseHelper.QuitarFiado(id, formaPagamento);
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDeletar(int id)
        {
            if (id > 0)
            {
                DatabaseHelper.DeletarAtendimento(id);
            }

            return RedirectToPage();
        }

        private void CarregarDados()
        {
            Atendimentos = DatabaseHelper.ListarAtendimentosHoje();
            TotalRecebido = Atendimentos
                .Where(a => a.FormaPagamento != "Fiado")
                .Sum(a => a.Valor);
        }
    }
}