using CrawlMST.Models;
using HtmlAgilityPack;
using System.Net;
using System.Text;

namespace CrawlMST.Services
{
    public class CrawlService
    {
        private readonly ApplicationDbContext _context;

        public CrawlService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CrawlAndSaveDataAsync(string tinh, int pageNumber)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers["User-Agent"] = "Mozilla/5.0 ...";
                webClient.Encoding = Encoding.UTF8;
                var html = webClient.DownloadString($"https://masothue.com/tra-cuu-ma-so-thue-theo-tinh/{tinh}?page={pageNumber}");

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var elements = htmlDoc.DocumentNode.Descendants("div")
                    .FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("tax-listing"));

                if (elements != null)
                {
                    var ctys = elements.Descendants("div")
                        .Where(d => d.Attributes.Contains("data-prefetch"));

                    foreach (var item in ctys)
                    {
                        var aes = item.Descendants("a").ToArray();
                        var address = item.Descendants("address").ToArray();
                        var company = new Company
                        {
                            Name = aes[0].InnerText,
                            TaxCode = aes[1].InnerText,
                            Representative = aes[2].InnerText,
                            Address = address[0].InnerText
                        };

                        _context.Companies.Add(company);
                    }

                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
