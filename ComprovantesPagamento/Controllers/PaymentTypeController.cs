using AutoMapper;
using ComprovantesPagamento.Domain.Models;
using ComprovantesPagamento.Domain.Responses;
using ComprovantesPagamento.Repositories;
using ComprovantesPagamento.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComprovantesPagamento.Controllers
{
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [Route("payment_type")]
    public class PaymentTypeController : BaseController
    {
        private IMapper _mapper;
        private PaymentTypeRepository _repository;

        public PaymentTypeController(PaymentTypeRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaymentTypeResponse>), StatusCodes.Status200OK)]
        public IActionResult List()
        {
            try
            {
                var types = _repository.List()
                    .Select(_mapper.Map<PaymentType, PaymentTypeResponse>)
                    .ToArray();

                return Ok(types);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PaymentTypeResponse), StatusCodes.Status200OK)]
        public IActionResult Update([FromRoute] string id,  [FromBody] PaymentTypeRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Code))
                    return BadRequest("Invalid code");

                if (string.IsNullOrWhiteSpace(request.Description))
                    return BadRequest("Invalid description");

                var type = _repository.Get(id);
                if (type == null)
                    return BadRequest("Invalid payment type");

                type.Code = request.Code;
                type.Description = request.Description;
                type.UpdateDate = DateTime.Now;

                _repository.Update(id, type);

                var response = _mapper.Map<PaymentType, PaymentTypeResponse>(type);
                return Ok(response);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] string id)
        {
            try
            {
                var type = _repository.Get(id);
                if (type == null)
                    return BadRequest("Invalid payment type");

                _repository.Delete(id);
                return NoContent();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(PaymentTypeResponse), StatusCodes.Status200OK)]
        public IActionResult Create([FromBody] PaymentTypeRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Code))
                    return BadRequest("Invalid code");

                if (string.IsNullOrWhiteSpace(request.Description))
                    return BadRequest("Invalid description");

                var type = new PaymentType
                {
                    Code = request.Code,
                    Description = request.Description,
                    CreateDate = DateTime.Now,
                    UpdateDate = null
                };

                 _repository.Insert(type);

                var response = _mapper.Map<PaymentType, PaymentTypeResponse>(type);
                return Ok(response);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
