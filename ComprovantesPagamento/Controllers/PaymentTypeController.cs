using AutoMapper;
using ComprovantesPagamento.Domain.Models;
using ComprovantesPagamento.Domain.Responses;
using ComprovantesPagamento.Repositories;
using ComprovantesPagamento.Requests;
using ComprovantesPagamento.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComprovantesPagamento.Controllers
{
    [Route("payment_type")]
    [Authorize]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public class PaymentTypeController : BaseController
    {
        private IMapper _mapper;
        private PaymentTypeRepository _repository;
        private DropboxConfig _dropboxConfig;
        private DropboxService _dropboxService;

        public PaymentTypeController(PaymentTypeRepository repository, IMapper mapper, DropboxConfig dropboxConfig, DropboxService dropboxService)
        {
            _mapper = mapper;
            _repository = repository;
            _dropboxConfig = dropboxConfig;
            _dropboxService = dropboxService;
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

                var type = _repository.Get(id);
                if (type == null || type.UserId != UserID)
                    return BadRequest("Invalid payment type");

                //TODO: block code change when there is at least one payment stored
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
                if (type == null || type.UserId != UserID)
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
                    UpdateDate = null,
                    UserId = UserID
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

        [HttpPost("{typeId}/payment")]
        public async Task<IActionResult> InsertPayment(
            [FromForm(Name = "payment_receipt")] IFormFile paymentReceipt,
            [FromForm(Name = "payment_document")] IFormFile paymentDocument,
            [FromForm(Name = "description")] string description,
            [FromForm(Name = "payment_date")] DateTime paymentDate,
            [FromRoute] string typeId)
        {
            try
            {
                if (paymentDate == default)
                    return BadRequest("Invalid payment date");

                if (string.IsNullOrWhiteSpace(description))
                    return BadRequest("Invalid escription");

                if (paymentReceipt == default)
                    return BadRequest("Payment receipt not found");

                if (paymentDocument == default)
                    return BadRequest("Payment document not found");

                var type = _repository.Get(typeId);
                if (type == null)
                    return BadRequest("Invalid payment type");

                var month = paymentDate.Month;
                var year = paymentDate.Year;

                var folderName = $"/{type.Code}/{year}/{month}";
                _dropboxService.CreateFolderIfNotExists(folderName);

                var receiptFileName = folderName + $"/{type.Code}_receipt_{year}_{month}.pdf";
                var documentFileName = folderName + $"/{type.Code}_document_{year}_{month}.pdf";

                await _dropboxService.Upload(receiptFileName, paymentReceipt);
                await _dropboxService.Upload(documentFileName, paymentDocument);

                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
