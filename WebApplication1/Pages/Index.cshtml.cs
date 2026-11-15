using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Usuario { get; set; } = string.Empty;
        [BindProperty]
        public string Senha { get; set; } = string.Empty;

        public string MensagemErro { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (DatabaseHelper.ValidarLogin(Usuario, Senha))
            {
                // Login bem-sucedido! Redireciona para a página de Caixa (que criaremos a seguir)
                // Por enquanto, vamos redirecionar para uma página vazia ou manter aqui
                return RedirectToPage("/Caixa");
            }

            MensagemErro = "Usuário ou senha inválidos.";
            return Page();
        }
    }
}