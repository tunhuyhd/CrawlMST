using CrawlMST.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace CrawlMST.Services
{
    public class CrawlService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public CrawlService(ApplicationDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.9999.999");
        }

        public async Task CrawlAndSaveDataAsync(string tinh, int pageNumber)
        {
            var url = $"https://masothue.com/tra-cuu-ma-so-thue-theo-tinh/{tinh}?page={pageNumber}";
            var html = await _httpClient.GetStringAsync(url);

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
                    var companyName = aes[0].InnerText;

                    if (await _context.Companies.AnyAsync(c => c.Name == companyName))
                    {
                        continue;
                    }

                    var company = new Company
                    {
                        Id = Guid.NewGuid(),
                        Name = companyName,
                        TaxCode = aes[1].InnerText,
                        Representative = aes[2].InnerText,
                        Address = address[0].InnerText
                    };

                    _context.Companies.Add(company);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task CrawlAndSaveAllDataAsync(string tinh)
        {
            int pageNumber = 2;
            while (true)
            {
                var previousCount = await _context.Companies.CountAsync();
                await CrawlAndSaveDataAsync(tinh, pageNumber);
                var currentCount = await _context.Companies.CountAsync();

                if (previousCount == currentCount && (pageNumber == 11 || pageNumber == 0))
                {
                    break;
                }

                await Task.Delay(3000);
                pageNumber++;
            }
        }
    }
}