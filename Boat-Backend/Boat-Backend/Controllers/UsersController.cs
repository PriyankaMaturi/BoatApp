using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Boat_Backend.Models;
using Boat_Backend.contexts;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using NuGet.Common;

namespace Boat_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]

    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly ProductContext _productContext;
        private readonly JwtOption _options;

        public UsersController(UserContext context , ProductContext pcontext , IOptions<JwtOption> options)
        {
            _context = context;
            _productContext = pcontext;
            _options = options.Value;
        }
        [Authorize]
        [HttpGet("Cart")]
        public async Task<ActionResult<IEnumerable<ProductWithCount>>> GetCart(int UserId)
        {
            Dictionary<int,int> cartItems = new Dictionary<int,int>();
            List<ProductWithCount> ProductList = new List<ProductWithCount>();
            var user = await _context.users.FindAsync(UserId);
            if (user == null)
                return NotFound("User Not Found");
            cartItems = JsonConvert.DeserializeObject<Dictionary<int,int>>(user.Cart);
            if (cartItems.Count() == 0)
            {

            }
            foreach(int item in cartItems.Keys)
            {
                var prod = await _productContext.Products.FindAsync(item);
                if(prod == null)
                    continue;
                ProductWithCount productWithCount = new ProductWithCount();
                productWithCount.Product = prod;
                productWithCount.ProductCount = cartItems[item];
                ProductList.Add(productWithCount);
            }

            return ProductList;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Getusers()
        {
            return await _context.users.ToListAsync();
        }
       
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(SignUp user)
        {
            User Existinguser = _context.users.FirstOrDefault(u => u.Email == user.Email);
            if (Existinguser != null)
            {
                return BadRequest("User already exists");
            }
            string Password = user.Password;
            byte[] salt, hash;
            var hmac= new HMACSHA512();
            salt = hmac.Key;
            hash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));

            User u=new User();
            u.Id = user.Id;
            u.Salt = salt;
            u.Hash = hash;
            u.Firstname=user.Firstname;
            u.Lastname=user.Lastname;
            u.Email=user.Email;
            u.Phonenumber=user.Phonenumber;
            u.Address=user.Address;
            u.Cart = "";


            _context.users.Add(u);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }


        [HttpPost("Cart")]

        public async Task<ActionResult<User>> PostCart(CartData data)
        {
            int UserId = data.UserId;
            int ProductId = data.ProductId;

            var user = await _context.users.FindAsync(UserId);

            if (user == null)
                return NotFound("User Not Found");

            var cartItem = new Dictionary<int, int>();

            if (string.IsNullOrEmpty(user.Cart))
            {
                cartItem[ProductId] = 1;
                user.Cart = JsonConvert.SerializeObject(cartItem);
            }
            else
            {
                cartItem = JsonConvert.DeserializeObject<Dictionary<int, int>>(user.Cart);
                if (cartItem.ContainsKey(ProductId)) {
                    cartItem[ProductId] += 1;
                }
                else
                {
                    cartItem[ProductId] = 1;
                }
                user.Cart = JsonConvert.SerializeObject(cartItem);
            }
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(user);
        }


        [HttpPost("DecreaseProductFromCart")]
        public async Task<ActionResult<User>> DecreaseProductFromCart(CartData data)
        {
            int UserId = data.UserId;
            int ProductId = data.ProductId;

            var user = await _context.users.FindAsync(UserId);

            if (user == null)
                return NotFound("User Not Found");

            var cartItem = new Dictionary<int, int>();

                cartItem = JsonConvert.DeserializeObject<Dictionary<int, int>>(user.Cart);
                if (cartItem.ContainsKey(ProductId) && cartItem[ProductId]>0)
                {
                    cartItem[ProductId] -= 1;
                }
                user.Cart = JsonConvert.SerializeObject(cartItem);
            
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Login>> PostLogin(Login login)
        {
            
            string Password=login.password;
            User user=_context.users.FirstOrDefault(u => u.Email == login.uname);
            if (user == null)
            {
                return NotFound("User not found");
            }
            byte[] Salt = user.Salt;
            var hmac = new HMACSHA512(Salt);
            byte[] password = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));

            if (Enumerable.SequenceEqual(password, user.Hash))
            {
                var Jwtkey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.Key));
                var credential = new SigningCredentials(Jwtkey, SecurityAlgorithms.HmacSha256);

                List<Claim> claims = new List<Claim>()
                {
                    new Claim("Email",login.uname)
                };
                var sToken = new JwtSecurityToken(_options.Key,_options.Issuer,null,expires: DateTime.Now.AddHours(5), signingCredentials:credential);

                var token = new JwtSecurityTokenHandler().WriteToken(sToken);

                return Ok(new { id=user.Id ,token = token }  );   
             //   return Ok(new { message = "Login successful" });

            }
            else
            {
                return BadRequest("Incorrect password");
            }
            
            return NotFound("Unable to login");
 
        }

        [HttpPost("validateToken")]
        public async Task<ActionResult> ValidateToken(TokenRequest t)
        {

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_options.Key);

                tokenHandler.ValidateToken(t.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                }, out SecurityToken validatedToken);

                return Ok(); // valid token
            }
            catch (Exception)
            {
                return BadRequest("Invalid token"+t.Token);
            }
        }
        [HttpDelete("deleteCartItem")]
        public async Task<ActionResult<User>> deleteCartItem(CartData data)
        {
            int UserId = data.UserId;
            int ProductId = data.ProductId;

            var user = await _context.users.FindAsync(UserId);

            if (user == null)
                return NotFound("User Not Found");

            var cartItem = new Dictionary<int, int>();

                cartItem = JsonConvert.DeserializeObject<Dictionary<int, int>>(user.Cart);
                if (cartItem.ContainsKey(ProductId))
                {
                    cartItem.Remove(ProductId);
                }
      
                user.Cart = JsonConvert.SerializeObject(cartItem);
            
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.users.Any(e => e.Id == id);
        }
    }
}
public class CartData
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
}

public class SignUp
{
    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }

    public string Password { get; set; }
    public string Phonenumber { get; set; }
    public string Address { get; set; }
    
 }

public class TokenRequest
{
    public string Token { get; set; }
}
