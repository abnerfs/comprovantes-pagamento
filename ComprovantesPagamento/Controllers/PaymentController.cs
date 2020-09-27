using AutoMapper;
using ComprovantesPagamento.Domain.Models;
using ComprovantesPagamento.Domain.Responses;
using ComprovantesPagamento.Repositories;
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
    [Route("payment_type/{typeId}/payment")]
    [Authorize]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public class PaymentController : BaseController
    {
        private PaymentRepository _repository;
        private PaymentTypeRepository _paymentTypeRepository;
        private IMapper _mapper;
        private DropboxService _dropboxService;

        public PaymentController(
            PaymentRepository repository,
            PaymentTypeRepository paymentTypeRepository,
            IMapper mapper,
            DropboxService dropboxService)
        {
            this._repository = repository;
            _paymentTypeRepository = paymentTypeRepository;
            _mapper = mapper;
            _dropboxService = dropboxService;
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

        [HttpDelete("{paymentId}")]
        public IActionResult DeletePayment([FromRoute] string typeId, [FromRoute] string paymentId)
        {
            try
            {
                var payment = _repository.GetByUser(UserID, paymentId);
                if (payment == null || payment.PaymentType != typeId)
                    return BadRequest("Invalid payment");

                if (!string.IsNullOrWhiteSpace(payment.PaymentReceipt))
                    _dropboxService.DeletePathIfExists(payment.PaymentReceipt);

                if(!string.IsNullOrWhiteSpace(payment.PaymentDocument))
                    _dropboxService.DeletePathIfExists(payment.PaymentDocument);

                _repository.Delete(paymentId);
                return NoContent();
            }
            catch (Exception)
            {

                throw;
            }
        }

        string ValidatePayment(string description, DateTime? paymentDate, string typeId, ref int year, ref int month, ref PaymentType type)
        {
            try
            {
                if (paymentDate == null || paymentDate == DateTime.MinValue)
                    return "Invalid payment date";

                if (string.IsNullOrWhiteSpace(description))
                    return "Invalid description";

                type = _paymentTypeRepository.GetByUserID(UserID, typeId);
                if (type == null)
                    return "Invalid payment type";

                if (year == 0)
                    year = paymentDate.Value.Year;

                if (month == 0)
                    month = paymentDate.Value.Month;

                return string.Empty;
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPut("{paymentId}")]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        public IActionResult UpdatePayment(
            [FromForm(Name = "description")] string description,
            [FromForm(Name = "payment_date")] DateTime? paymentDate,
            [FromForm(Name = "year")] int year,
            [FromForm(Name = "month")] int month,
            [FromRoute] string typeId,
            [FromRoute] string paymentId)
        {
            try
            {
                PaymentType type = null;
                var payment = _repository.GetByUser(UserID, paymentId);

                {
                    if (payment == null || payment.PaymentType != typeId)
                        return BadRequest("Invalid payment");

                    var validation = ValidatePayment(description, paymentDate, typeId, ref year, ref month, ref type);
                    if (!string.IsNullOrWhiteSpace(validation))
                        return BadRequest(validation);

                    {
                        var paymentSameDate = _repository.GetByYearMonth(type.Id, year, month);
                        if (paymentSameDate != null && paymentSameDate.Id != paymentId)
                            return BadRequest("There is already a payment for this month and year registered");
                    }
                }

                payment.Description = description;
                payment.Month = month;
                payment.Year = year;

                _repository.Update(paymentId, payment);
                var paymentResponse = _mapper.Map<Payment, PaymentResponse>(payment);
                return Ok(paymentResponse);
            }
            catch (Exception)
            {

                throw;
            }
        }




        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaymentResponse>), StatusCodes.Status200OK)]
        public IActionResult ListPayment([FromRoute] string typeId)
        {
            try
            {
                var payments = _repository.ListPayment(UserID, typeId)
                    .Select(_mapper.Map<Payment, PaymentResponse>)
                    .ToList();

                return Ok(payments);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPut("{paymentId}/document")]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDocument(
            [FromForm(Name = "payment_document")] IFormFile paymentDocument,
            [FromRoute] string typeId,
            [FromRoute] string paymentId)
        {
            try
            {
                var payment = _repository.GetByUser(UserID, paymentId);
                if (payment == null || payment.PaymentType != typeId)
                    return BadRequest("Invalid payment");

                if (!string.IsNullOrEmpty(payment.PaymentDocument))
                    _dropboxService.DeletePathIfExists(payment.PaymentDocument);

                if (paymentDocument != null)
                    payment.PaymentDocument = await uploadfile(paymentDocument, payment.GetFolderName(), payment.GetDocumentFileName());
                else
                    payment.PaymentDocument = null;

                _repository.Update(paymentId, payment);

                var response = _mapper.Map<Payment, PaymentResponse>(payment);
                return Ok(payment);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPut("{paymentId}/receipt")]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateReceipt(
            [FromForm(Name = "payment_receipt")] IFormFile paymentReceipt,
            [FromRoute] string typeId,
            [FromRoute] string paymentId)
        {
            try
            {
                var payment = _repository.GetByUser(UserID, paymentId);
                if (payment == null || payment.PaymentType != typeId)
                    return BadRequest("Invalid payment");

                if (!string.IsNullOrEmpty(payment.PaymentReceipt))
                    _dropboxService.DeletePathIfExists(payment.PaymentReceipt);

                if (paymentReceipt != null)
                    payment.PaymentReceipt = await uploadfile(paymentReceipt, payment.GetFolderName(), payment.GetReceiptFileName());
                else
                    payment.PaymentReceipt = null;

                _repository.Update(paymentId, payment);

                var response = _mapper.Map<Payment, PaymentResponse>(payment);
                return Ok(payment);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> InsertPayment(
            [FromForm(Name = "payment_receipt")] IFormFile paymentReceipt,
            [FromForm(Name = "payment_document")] IFormFile paymentDocument,
            [FromForm(Name = "description")] string description,
            [FromForm(Name = "payment_date")] DateTime? paymentDate,
            [FromForm(Name = "year")] int year,
            [FromForm(Name = "month")] int month,
            [FromRoute] string typeId)
        {
            try
            {
                PaymentType type = null;
                {
                    var validation = ValidatePayment(description, paymentDate, typeId, ref year, ref month, ref type);
                    if (!string.IsNullOrWhiteSpace(validation))
                        return BadRequest(validation);

                    if (paymentReceipt == null && paymentDocument == null)
                        return BadRequest("Document and receipt not found");

                    if (_repository.GetByYearMonth(type.Id, year, month) != null)
                        return BadRequest("There is already a payment for this month and year registered");
                }

                var payment = new Payment
                {
                    UserId = UserID,
                    CreateDate = DateTime.Now,
                    Description = description,
                    PaymentType = type.Id,
                    PaymentTypeCode = type.Code,
                    Month = month,
                    Year = year
                };


                var folderName = payment.GetFolderName();
                _dropboxService.CreateFolderIfNotExists(folderName);

                if (paymentReceipt != null)
                    payment.PaymentReceipt = await uploadfile(paymentReceipt, folderName, payment.GetReceiptFileName());


                if (paymentDocument != null)
                    payment.PaymentDocument = await uploadfile(paymentDocument, folderName, payment.GetDocumentFileName());

                _repository.Insert(payment);

                var paymentResponse = _mapper.Map<Payment, PaymentResponse>(payment);
                return Ok(paymentResponse);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
