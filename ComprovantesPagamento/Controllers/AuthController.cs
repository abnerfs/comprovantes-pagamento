using AutoMapper;
using ComprovantesPagamento.Domain.Models;
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
        private IMapper _mapper;

        public AuthController(JwtService jwt, UserRepository repository, IMapper mapper )
        {
            _jwt = jwt;
            _repository = repository;
            _mapper = mapper;
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        string GenerateVerifyCode(string email)
        {
            return "djaushdas";
        }


        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest("Invalid Name");

                if (string.IsNullOrWhiteSpace(request.LastName))
                    return BadRequest("Invalid Lastname");

                if (string.IsNullOrWhiteSpace(request.Pass))
                    return BadRequest("Invalid password");

                if (!IsValidEmail(request.Email))
                    return BadRequest("Invalid e-mail");

                if (_repository.GetByEmail(request.Email) != null)
                    return BadRequest("E-mail already registerd");


                var verifyCode = GenerateVerifyCode(request.Email);
                var user = new User
                { 
                    Email = request.Email,
                    Name = request.Name,
                    RegisterDate = DateTime.Now,
                    Verified = false,
                    VerifiedDate = null,
                    VerifyCode = verifyCode,
                    CryptPass = BCrypt.Net.BCrypt.HashPassword(request.Pass)
                };

                _repository.Insert(user);

                //TODO: mandar e-mail para verificar usuário
                return Ok(_mapper.Map<User, RegisterResponse>(user));
            }
            catch (Exception)
            {

                throw;
            }
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
                {
                    if (!BCrypt.Net.BCrypt.Verify(request.Pass, user.CryptPass))
                        return BadRequest("Invalid e-mail or password");
                }

                if (!user.Verified)
                    return BadRequest("User not verified");

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
