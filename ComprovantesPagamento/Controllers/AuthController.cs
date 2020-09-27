using ComprovantesPagamento.Domain.Requests;
using ComprovantesPagamento.Domain.Responses;
using ComprovantesPagamento.Repositories;
using ComprovantesPagamento.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ComprovantesPagamento.Controllers
{
    [Route("auth")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public class AuthController : BaseController
    {
        private JwtService _jwt;
        private UserRepository _repository;

        public AuthController(JwtService jwt, UserRepository repository )
        {
            _jwt = jwt;
            _repository = repository;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JwtResponse), StatusCodes.Status200OK)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                    return BadRequest("Invalid e-mail");

                if (string.IsNullOrWhiteSpace(request.Pass))
                    return BadRequest("Invalid password");

                var user = _repository.GetByEmail(request.Email);
                //TODO: Compare passwords 
                {
                    if (user?.CryptPass != request.Pass)
                        return BadRequest("Invalid e-mail or password");
                }

                var token = _jwt.GenerateToken(user.Id);
                return Ok(token);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
