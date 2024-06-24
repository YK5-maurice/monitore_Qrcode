using Microsoft.AspNetCore.Mvc;
using Monitore_Qrcode.Models;
using Newtonsoft.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

public class HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor,TokenService tokenService) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string _apiUrl = "https://ebank.gtbankci.com/payme_api/api/GetTransactions";
    private readonly string _apiUrl1 = "https://ebank.gtbankci.com/payme_api/api/Authenticate";
    private readonly string _apiKey = "23tgygy-TYYZF_IOZFV";
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly TokenService _tokenService=tokenService;
    private string _token;
    private readonly string keyAES = "E546C8DF278CD5931069B522E695D4F2";

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return View("Index");
        }
        else
        {
            
            string cred = username + "|" + password;
            string token = _tokenService.GenerateToken(cred);

            //var tbytes = Encoding.UTF8.GetBytes(cred);
            //string credential = Convert.ToBase64String(tbytes);/*EncryptAES.EncryptString(username, password, keyAES)*/;
            
            //****** start of delete ******//

            Console.WriteLine(token);

            //****** end of delete ******//

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl1}/user?credentials={token}");
                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.Add("apiKey", _apiKey);

                HttpResponseMessage responseMessage = await _httpClient.SendAsync(request);

                Console.WriteLine(responseMessage);

                if (responseMessage.IsSuccessStatusCode)
                {
                    _token = await responseMessage.Content.ReadAsStringAsync();

                    // Stocker la chaîne JSON dans la session
                    _httpContextAccessor.HttpContext.Session.SetString("_tokenSession", _token);

                    //****** start of delete ******//

                    Console.WriteLine(_token);

                    //****** end of delete ******//

                    return RedirectToAction("Data");
                }
                else
                {
                   return View("Index");
					//return RedirectToAction("Data");
				}

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la requête à l'API : {ex.Message}");
            }

        }

    }

    public async Task<IActionResult> Data()
    {

        try
        {
            // Récupérer la chaîne JSON de la session
            string _tokenSession = _httpContextAccessor.HttpContext.Session.GetString("_tokenSession");

            var request = new HttpRequestMessage(HttpMethod.Get, _apiUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenSession);
            request.Headers.Add("apiKey", _apiKey);

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                var transactions = JsonConvert.DeserializeObject<List<Transaction>>(jsonString);

                Console.WriteLine(jsonString);

                // Stocker la chaîne JSON dans la session
                _httpContextAccessor.HttpContext.Session.SetString("liste_session", jsonString);

                return View(transactions);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Erreur lors de la requête à l'API");
            }
        }
        catch (HttpRequestException e)
        {
            return StatusCode(500, $"Erreur lors de la requête à l'API : {e.Message}");
        }
    }


    [HttpPost]
    public async Task<IActionResult> PrintPDF(int idname)
    {
        // Récupérer la chaîne JSON de la session
        string listeJson = _httpContextAccessor.HttpContext.Session.GetString("liste_session");

        if (string.IsNullOrEmpty(listeJson))
        {
            return BadRequest("La session est vide ou non valide.");
        }

        // Convertir la chaîne JSON en liste de transactions
        List<Transaction> transactions = JsonConvert.DeserializeObject<List<Transaction>>(listeJson);

        // Rechercher l'élément dans la liste avec l'ID spécifié
        Transaction transaction = transactions.FirstOrDefault(t => t.Id == idname);

        // Vérifier si l'élément a été trouvé
        if (transaction != null)
        {
            var stream = new MemoryStream();

            var document = Document.Create(container =>
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/logo_gtco.jpg");
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(15));

                    // Header
                    page.Header().Row(row =>
                    {
                        // Charger et insérer l'image comme logo
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/logo_gtco.jpg");
                        row.ConstantItem(50).Height(50).Image(imagePath); // Ajustez la taille selon vos besoins
                        row.Spacing(1, Unit.Centimetre);
                        row.RelativeItem().Column(column =>
                        {

                            //column.Item().Height(50).Image(imagePath);
                            column.Item().Text("GTBank COTE D'IVOIRE").Underline(true)
                                .SemiBold().FontSize(24).FontColor(Colors.Black);
                            // column.Item().LineHorizontal(5);
                        });
                    });

                    // Content
                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                    {
                        column.Item().Text("Reçu de la Transaction").SemiBold().FontSize(20).FontColor(Colors.Black).AlignCenter();
                        column.Item().LineHorizontal(1);
                        column.Item().PaddingVertical(15);
                        column.Spacing(10);

                        // Détails du client et transaction
                        column.Item().Row(row =>
                        {

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("DETAILS DU CLIENT").Bold().FontSize(16).Underline(true);
                                col.Item().PaddingVertical(10);
                                col.Item().Text($"ID : ");
                                col.Item().PaddingVertical(8);
                                col.Item().Text($"NOM : ");
                                col.Item().PaddingVertical(8);
                                col.Item().Text($"EMAIL : ");
                                col.Item().PaddingVertical(8);
                                col.Item().Text($"TELEPHONE : ");
                            });
                            row.RelativeItem().Column(col =>
                            {
                                //col.Item().Text("").Bold().FontSize(16).Underline(true);
                                col.Item().PaddingVertical(18);
                                col.Item().Text($"{transaction.Id}");
                                col.Item().PaddingVertical(8);
                                col.Item().Text($"{transaction.Nom}");
                                col.Item().PaddingVertical(8);
                                col.Item().Text($"{transaction.Email}");
                                col.Item().PaddingVertical(8);
                                col.Item().Text($"{transaction.Tel}");
                            });
                        });

                        column.Item().PaddingVertical(10);
                        column.Item().Text("DETAILS DE LA TRANSACTION").Bold().FontSize(16).Underline(true);
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                string service;
                                col.Item().PaddingVertical(10);
                                col.Item().Text($"TYPE DE TRANSACTION: {transaction.TransactionType}");
                                col.Item().PaddingVertical(3);
                                col.Item().Text($"COMPTE A CREDITER: {transaction.AccountToCredit}");
                                col.Item().PaddingVertical(3);
                                col.Item().Text($"GU_ID: {transaction.GuId}");
                                col.Item().PaddingVertical(3);
                                col.Item().Text($"USER_ID CLIENT: {transaction.UserIdCustomer}");
                                col.Item().PaddingVertical(3);
                                col.Item().Text($"DATE D'INSERTION: {transaction.InsertDate}");
                                col.Item().PaddingVertical(3);

                                if (transaction.IsMTNMoney == "1") { service = "MTNMoney"; }
                                else if (transaction.IsMoovMoney == "1") { service = "MoovMoney"; }
                                else if (transaction.IsOrangeMoney == "1") { service = "OrangeMoney"; }
                                else { service = "GTBank"; }

                                col.Item().Text($"SERVICE: {service}");
                                col.Item().PaddingVertical(3);
                                col.Item().Text($"REMARQUES: {transaction.Remarks}");
                            });
                        });

                        // Séparation
                        column.Item().PaddingVertical(5);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Status QR Code: {transaction.StatusQrCode}").Bold();
                            row.RelativeItem().Text($"MONTANT: {transaction.Amount}").Bold().AlignRight();
                        });
                    });
                    // Footer
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });
            document.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream, "application/pdf", "Recu_Transaction.pdf");
        }
        else
        {
            // Aucun élément trouvé avec l'ID spécifié
            return NotFound("Aucune transaction trouvée avec l'ID spécifié.");
        }
    }




    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
