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
using System.IO;
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
        private PaymentRepository _paymentRepository;

        public PaymentTypeController(PaymentTypeRepository repository, IMapper mapper, DropboxConfig dropboxConfig, DropboxService dropboxService, PaymentRepository paymentRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _dropboxConfig = dropboxConfig;
            _dropboxService = dropboxService;
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

                var type = _repository.GetByUserID(UserID, id);
                if (type == null)
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
                var type = _repository.GetByUserID(UserID, id);
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

        async Task<string> uploadfile(IFormFile file, string folderName, string fileName)
        {
            try
            {
                _dropboxService.CreateFolderIfNotExists(folderName);
                var fileNameUpload = Path.Combine(folderName, fileName).Replace('\\', '/');
                await _dropboxService.Upload(fileNameUpload, file);
                return fileNameUpload;
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPut("{typeId}/payment/{paymentId}")]
        public async Task<IActionResult> UpdatePayment(
            [FromForm(Name = "payment_receipt")] IFormFile paymentReceipt,
            [FromForm(Name = "payment_document")] IFormFile paymentDocument,
            [FromForm(Name = "description")] string description,
            [FromForm(Name = "payment_date")] DateTime paymentDate,
            [FromRoute] string typeId,
            [FromRoute] string paymentId)
        {
            try
            {
                if (paymentDate == default)
                    return BadRequest("Invalid payment date");

                if (string.IsNullOrWhiteSpace(description))
                    return BadRequest("Invalid description");

                if (paymentReceipt == default && paymentDocument == null)
                    return BadRequest("Document and receipt not found");

                var type = _repository.GetByUserID(UserID, typeId);
                if (type == null)
                    return BadRequest("Invalid payment type");

                var payment = _paymentRepository.GetByUser(UserID, paymentId);
                if (payment == null)
                    return BadRequest("Invalid payment");

                var month = paymentDate.Month;
                var year = paymentDate.Year;

                {
                    var paymentSameDate = _paymentRepository.GetByYearMonth(type.Id, year, month);
                    if(paymentSameDate != null && paymentSameDate.Id != paymentId)
                        return BadRequest("There is already a payment for this month and year registered");
                }

                payment.Description = description;

                

                payment.Month = month;
                payment.Year = year;

                var folderName = $"/{type.Code}/{year}/{month}";


                var receiptFileName = $"{type.Code}_receipt_{year}_{month}.pdf";
                var documentFileName = $"{type.Code}_document_{year}_{month}.pdf";

                if (paymentReceipt != null)
                {
                    payment.PaymentReceipt = await uploadfile(paymentReceipt, folderName, receiptFileName);
                }
                else
                {
                    //TODO: delete file
                }

                if (paymentDocument != null)
                {
                    payment.PaymentDocument = await uploadfile(paymentDocument, folderName, documentFileName);
                }
                else
                {
                    //TODO: delete file
                }

                _paymentRepository.Update(paymentId, payment);
                return Ok(payment);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("{typeId}/payment")]
        public IActionResult ListPayment([FromRoute] string typeId)
        {
            try
            {
                var payments = _paymentRepository.ListPayment(UserID, typeId);
                return Ok(payments);
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
                    return BadRequest("Invalid description");

                if (paymentReceipt == default && paymentDocument == null)
                    return BadRequest("Document and receipt not found");

                var type = _repository.GetByUserID(UserID, typeId);
                if (type == null)
                    return BadRequest("Invalid payment type");

                var month = paymentDate.Month;
                var year = paymentDate.Year;

                if (_paymentRepository.GetByYearMonth(type.Id, year, month) != null)
                    return BadRequest("There is already a payment for this month and year registered");

                var folderName = $"/{type.Code}/{year}/{month}";
                _dropboxService.CreateFolderIfNotExists(folderName);

                var payment = new Payment
                {
                    UserId = UserID,
                    CreateDate = DateTime.Now,
                    Description = description,
                    PaymentType = type.Id,
                    Month = month,
                    Year = year
                };

                if (paymentReceipt != null)
                {
                    var receiptFileName = $"{type.Code}_receipt_{year}_{month}.pdf";
                    payment.PaymentReceipt = await uploadfile(paymentReceipt, folderName, receiptFileName);
                }
                 
                
                if(paymentDocument != null)
                {
                    var documentFileName = $"{type.Code}_document_{year}_{month}.pdf";
                    payment.PaymentDocument = await uploadfile(paymentDocument, folderName, documentFileName);
                }

                _paymentRepository.Insert(payment);
                return Ok(payment);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
