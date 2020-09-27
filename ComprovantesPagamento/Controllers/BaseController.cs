using ComprovantesPagamento.Domain.Responses;
using ComprovantesPagamento.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ComprovantesPagamento.Controllers
{
    public abstract class BaseController: ControllerBase
    {
        public BaseController()
        {

        }

        public string UserID => GetClaim(JwtService.USERID_CLAIM);

        public int GetClaimInt(string claim)
        {
            try
            {
                var value = 0;
                int.TryParse(GetClaim(claim), out value);
                return value;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string GetClaim(string claim)
        {
            try
            {
                return HttpContext.User.Claims.FirstOrDefault(x => x.Type == claim)?.Value;
            }
            catch (Exception)
            {

                throw;
            }
        }



        public BadRequestObjectResult BadRequest(string errorMessage)
        {
            try
            {
                return BadRequest(new ErrorResponse
                {
                    Error = errorMessage
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
