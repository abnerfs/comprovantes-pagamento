using AutoMapper;
using ComprovantesPagamento.Domain.Models;
using ComprovantesPagamento.Domain.Responses;
using ComprovantesPagamento.Repositories;
using ComprovantesPagamento.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComprovantesPagamento.Controllers
{
    [Route("payment_type")]
    [Authorize]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public class PaymentTypeController : BaseController
    {
        private IMapper _mapper;
        private PaymentTypeRepository _repository;
        private PaymentRepository _paymentRepository;

        public PaymentTypeController(
            PaymentTypeRepository repository, 
            IMapper mapper,
            PaymentRepository paymentRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _paymentRepository = paymentRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaymentTypeResponse>), StatusCodes.Status200OK)]
        public IActionResult List()
        {
            try
            {
                var types = _repository.List(UserID)
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
        public IActionResult Update([FromRoute] string id, [FromBody] PaymentTypeRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Code))
                    return BadRequest("Invalid code");

                if (string.IsNullOrWhiteSpace(request.Description))
                    return BadRequest("Invalid description");

                if(string.IsNullOrWhiteSpace(request.Icon))
                    return BadRequest("Invalid icon");

                if (string.IsNullOrWhiteSpace(request.Color))
                    return BadRequest("Invalid color");

                var type = _repository.GetByUserID(UserID, id);
                if (type == null)
                    return BadRequest("Invalid payment type");

                if(type.Code != request.Code && _paymentRepository.ListPayment(UserID, type.Id).Any())
                    return BadRequest("Can't change type code becaure the folder is already created"); 

                type.Description = request.Description;
                type.UpdateDate = DateTime.Now;
                type.Color = request.Color;
                type.Icon = request.Icon;

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
                var type = _repository.GetByUserID(UserID, id);
                if (type == null)
                    return BadRequest("Invalid payment type");

                if (_paymentRepository.ListPayment(UserID, type.Id).Any())
                    return BadRequest("Can't delete because there are payments created");

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

                if (string.IsNullOrWhiteSpace(request.Icon))
                    return BadRequest("Invalid icon");

                if (string.IsNullOrWhiteSpace(request.Color))
                    return BadRequest("Invalid color");

                var type = new PaymentType
                {
                    Code = request.Code,
                    Description = request.Description,
                    CreateDate = DateTime.Now,
                    UpdateDate = null,
                    UserId = UserID,
                    Color = request.Color,
                    Icon = request.Icon
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
