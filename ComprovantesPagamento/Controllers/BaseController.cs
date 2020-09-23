using ComprovantesPagamento.Domain.Responses;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ComprovantesPagamento.Controllers
{
    public abstract class BaseController: ControllerBase
    {
        public BaseController()
        {

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
