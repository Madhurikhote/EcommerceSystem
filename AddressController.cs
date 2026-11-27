using EcommerceWebAPI.Data;
using EcommerceWebAPI.DTOs;
using EcommerceWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class AddressController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AddressController(AppDbContext context)
        {
            _context = context;
        }


        private int GetUserId()
        {
            var claim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier);

            if (claim == null)
                throw new Exception(
                    "Token me user id nahi hai");

            return int.Parse(claim.Value);
        }


        // =========================
        // Add Address
        // =========================

        [HttpPost]
        public IActionResult AddAddress(
            CreateAddressDTO dto)
        {
            int userId = GetUserId();

            var address = new Address
            {
                UserId = userId,

                FullName = dto.FullName,

                MobileNumber = dto.MobileNumber,

                AddressLine1 = dto.AddressLine1,

                AddressLine2 = dto.AddressLine2,

                City = dto.City,

                State = dto.State,

                PostalCode = dto.PostalCode,

                Country = dto.Country,

                IsDefault = false
            };

            _context.Addresses.Add(address);

            _context.SaveChanges();

            return Ok(address);
        }


        // =========================
        // Get My Addresses
        // =========================

        [HttpGet]
        public IActionResult GetMyAddresses()
        {
            int userId = GetUserId();

            var addresses = _context.Addresses
                .Where(a => a.UserId == userId)
                .ToList();

            return Ok(addresses);
        }


        // =========================
        // Delete Address
        // =========================

        [HttpDelete("{id}")]
        public IActionResult DeleteAddress(int id)
        {
            int userId = GetUserId();

            var address = _context.Addresses
                .FirstOrDefault(a =>
                    a.Id == id &&
                    a.UserId == userId);

            if (address == null)
            {
                return NotFound(
                    "Address nahi mila");
            }

            _context.Addresses.Remove(address);

            _context.SaveChanges();

            return Ok(
                "Deleted successfully");
        }
        // =========================
        // Update Address
        // =========================

        [HttpPut("{id}")]
        public IActionResult UpdateAddress(
            int id,
            UpdateAddressDTO dto)
        {
            int userId = GetUserId();

            var address = _context.Addresses
                .FirstOrDefault(a =>
                    a.Id == id &&
                    a.UserId == userId);

            if (address == null)
            {
                return NotFound("Address nahi mila");
            }

            address.FullName = dto.FullName;
            address.MobileNumber = dto.MobileNumber;
            address.AddressLine1 = dto.AddressLine1;
            address.AddressLine2 = dto.AddressLine2;
            address.City = dto.City;
            address.State = dto.State;
            address.PostalCode = dto.PostalCode;
            address.Country = dto.Country;

            _context.SaveChanges();

            return Ok(address);
        }
        // =========================
        // Get Address By ID
        // =========================

        [HttpGet("{id}")]
        public IActionResult GetAddressById(int id)
        {
            int userId = GetUserId();

            var address = _context.Addresses
                .FirstOrDefault(a =>
                    a.Id == id &&
                    a.UserId == userId);

            if (address == null)
            {
                return NotFound("Address nahi mila");
            }

            return Ok(address);
        }

    }
}