using bestelplatform.Data.bestelplatform;
using bestelplatform.DTOs;
using bestelplatform.Models.Enums;
using bestelplatform.Models.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace bestelplatform.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdmindashboardController : Controller
    {
        private readonly BestelplatformDbContext _bestelplatformContext;
        public AdmindashboardController(BestelplatformDbContext bestelplatformContext, IConfiguration configuration)
        {
            _bestelplatformContext = bestelplatformContext;
        }

        // Gebruikersbheer

        [Route("")]
        [Route("gebruiksbeheer")]
        public IActionResult GebruiksBeheer()
        {
            return View("GebruikersBeheerPagina");
        }

        [HttpGet("add/user")]
        public async Task<IActionResult> AddUserAndGenerateQRCode()
        {
            string uniqueCode = Guid.NewGuid().ToString();
            string url = $"{Request.Scheme}://{Request.Host}/registratie?uniekecode={uniqueCode}";

            var urlPayload = new PayloadGenerator.Url(url);
            using var qrCodeData = QRCodeGenerator.GenerateQrCode(urlPayload, QRCodeGenerator.ECCLevel.H);
            using var pngRender = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = pngRender.GetGraphic(6);
            string base64Image = "data:image/png;base64," + Convert.ToBase64String(qrCodeImage);

            var gebruiker = new Gebruiker
            {
                UniekeCode = uniqueCode
            };
            await _bestelplatformContext.AddAsync(gebruiker);

            var medewerker = new Medewerker
            {
                Gebruiker = gebruiker
            };
            await _bestelplatformContext.AddAsync(medewerker);
            await _bestelplatformContext.SaveChangesAsync();

            return Ok(new { 
                ImageSrc = base64Image,
                Url = url                 
            });
        }

        [HttpGet("load/users")]
        public async Task<IActionResult> LoadUsers()
        {
            var users = await _bestelplatformContext.Gebruikers
            .Select(user => new
            {
                id = user.Id,
                naam = user.Naam,
                uniekeCode = user.UniekeCode,
                geactiveerd = user.Geactiveerd,
                roles = user.Rols.Select(r => new { naam = r.Naam }).ToList()
            })
            .OrderByDescending(user => user.geactiveerd)
            .ToListAsync();
            return Ok(users);
        }

        [HttpDelete("remove/user")]
        public async Task<IActionResult> RemoveUser(int id)
        {
            var gebruiker = await _bestelplatformContext.Gebruikers
                                  .FindAsync(id);
            _bestelplatformContext.Gebruikers.Remove(gebruiker);
            await _bestelplatformContext.SaveChangesAsync();

            return Ok();
        }
        [HttpGet("get/roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _bestelplatformContext.Rollens
                        .ToListAsync();
            return Ok(roles);
        }

        public class AddUserRolePostBody()
        {
            public int UserId { get; set; }
            public string NewRole { get; set; }
        }

        [HttpPost("add/user/role")]
        public async Task<IActionResult> AddUserRole([FromBody] AddUserRolePostBody rolePostBody)
        {
            var newRole = await _bestelplatformContext.Rollens
                                    .FirstOrDefaultAsync(row => row.Naam == rolePostBody.NewRole);
            var gebruiker = await _bestelplatformContext.Gebruikers
                                  .Include(table => table.Rols)
                                  .FirstOrDefaultAsync(row => row.Id == rolePostBody.UserId);

            foreach (var role in gebruiker.Rols)
            {
                if (role.Naam == rolePostBody.NewRole)
                {
                    return Conflict();
                }
            }
            gebruiker.Rols.Add(newRole);
            await _bestelplatformContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("delete/user/role")]
        public async Task<IActionResult> DeleteUserRole(int userId, string roleName)
        {
            var toBeDeletedRole = await _bestelplatformContext.Rollens
                                    .FirstOrDefaultAsync(row => row.Naam == roleName);
            var gebruiker = await _bestelplatformContext.Gebruikers
                                  .Include(table => table.Rols)
                                  .FirstOrDefaultAsync(row => row.Id == userId);

            gebruiker.Rols.Remove(toBeDeletedRole);
            await _bestelplatformContext.SaveChangesAsync();

            return Ok();
        }

        // Assortimentbeheer

        [Route("Assortimentbeheer")]
        public IActionResult Assortimentbeheer()
        {
            return View("AssortimentBeheerPagina");
        }

        [HttpGet("load/products")]
        public async Task<IActionResult> LoadProducts()
        {
            var products = await _bestelplatformContext.Productdetails
                                 .GroupBy(row => row.ProductId)
                                 .Select(g => g.OrderByDescending(g => g.Tijdstip).FirstOrDefault())
                                 .ToListAsync();
            return Ok(products);
        }

        [HttpGet("get/product/types")]
        public async Task<IActionResult> GetProductTypes()
        {
            var productTypes = Enum.GetNames<ProductEnum>().ToList();
            
            return Ok(productTypes);
        }

        [HttpPost("add/product")]
        public async Task<IActionResult> AddProduct([FromBody] ProductdetailsDto product)
        {
            var newProduct = new Producten()
            {
                Id = 0
            };
            _bestelplatformContext.Productens.Add(newProduct);
            await _bestelplatformContext.SaveChangesAsync();

            var newProductDetail = new Productdetail()
            {
                Naam = product.ProductName,
                Prijs = product.ProductPrice,
                Producttype = product.ProductType,
                ProductId = newProduct.Id
            };
            _bestelplatformContext.Productdetails.Add(newProductDetail);
            await _bestelplatformContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("delete/product/{productID}")]
        public async Task<IActionResult> DeleteProduct(int productID)
        {
            var productToDelete = await _bestelplatformContext.Productens
                                        .Where(row => row.Id == productID)
                                        .FirstOrDefaultAsync();
            _bestelplatformContext.Productens.Remove(productToDelete);
            await _bestelplatformContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("edit/product/")]
        public async Task<IActionResult> EditProduct([FromBody] ProductdetailsDto updatedProduct)
        {
            var newProductDetail = new Productdetail
            {
                ProductId = updatedProduct.ProductID,
                Naam = updatedProduct.ProductName,
                Prijs = updatedProduct.ProductPrice,
                Producttype = updatedProduct.ProductType,
                Tijdstip = DateTime.Now
            };
            _bestelplatformContext.Productdetails.Add(newProductDetail);
            await _bestelplatformContext.SaveChangesAsync();

            return Ok();
        }
    }
}
