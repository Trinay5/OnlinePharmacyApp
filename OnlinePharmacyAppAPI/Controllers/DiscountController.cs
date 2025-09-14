﻿using Microsoft.AspNetCore.Mvc;
using OnlinePharmacyAppAPI.DTO;
using OnlinePharmacyAppAPI.Services;

namespace OnlinePharmacyAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : Controller
    {
        Unity _unity;
        public DiscountController(Unity dba)
        {
            _unity = dba;
        }
        [HttpGet]
        public ActionResult GetAll()
        {
            List<DiscountDTO> lst = _unity.DiscountService.GetAllDiscounts();
            return Ok(new { Data = lst });
        }

        [HttpPost]
        public ActionResult AddMedicine(DiscountDTO inp)
        {
            bool Status = _unity.DiscountService.AddDiscount(inp);
            return Ok(new { Data = "Success in Adding Discount" });

        }
        [HttpPut("{id}")]
        public ActionResult UpdateDiscount(DiscountDTO inp, int id)
        {
            inp.DiscountId = id;
            bool Status = _unity.DiscountService.UpdateDiscount(inp);
            return Ok(new { Data = "Success in Updating Discount" });

        }
        [HttpGet("GetByUserId/{userId}")]
        public ActionResult GetByUserId(int userId)
        {
            var discount = _unity.DiscountService.GetDiscountByUserId(userId);

            if (discount == null)
                return NotFound(new { Message = "No active discount for this user." });

            return Ok(discount);
        }
        [HttpGet("apply")]
        public IActionResult ApplyDiscount(int userId, decimal subtotal)
        {
            var (amount, code, isPercent) = _unity.DiscountService.GetApplicableDiscount(userId, subtotal);

            return Ok(new DiscountResponseDTO
            {
                DiscountAmount = amount,
                DiscountCode = code,
                IsPercentage = isPercent
            });
        }
    }
}