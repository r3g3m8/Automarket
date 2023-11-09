using System;
using System.Threading.Tasks;
using Automarket.DataAccessLayer.Inerfaces;
using Automarket.Domain.Entity;
using Automarket.Domain.ViewModels.Cars;
using Automarket.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Automarket.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCars()
        {
            var response = await _carService.GetAllCars();
            if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return View(response.Data);
            }
            return RedirectToAction("Error");
        }

        [HttpGet]
        public async Task<IActionResult> GetCar(int id, bool isJson)
        {
            var response = await _carService.GetCar(id);
            if (isJson)
            {
                return Json(response.Data);
            }
            return PartialView("GetCar", response.Data);
        }
        [HttpPost]
        public async Task<IActionResult> GetCar(string term, int page = 1, int pageSize = 5)
        {
            var response = await _carService.GetCar(term);
            return Json(response.Data);
        }

        [HttpGet]
        public IActionResult Compare() => PartialView();

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _carService.DeleteCar(id);
            if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return RedirectToAction("GetCars");
            }
            return RedirectToAction("Error");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Save(int id)
        {
            if (id == 0)
                return PartialView();

            var response = await _carService.GetCar(id);
            if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return PartialView(response.Data);
            }
            ModelState.AddModelError("Okey", "Error Message");
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Save(CarViewModel carViewModel)
        {
            ModelState.Remove("DateCreate");
            //ModelState.Remove("Name");
            ModelState.Remove("Description");
            //ModelState.Remove("Model");
            //ModelState.Remove("Speed");
            ModelState.Remove("TypeCar");
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                if (carViewModel.Id == 0)
                {
                    //byte[] imageData;
                    //using (var binaryReader = new BinaryReader(carViewModel.Avatar.OpenReadStream()))
                    //{
                    //    imageData = binaryReader.ReadBytes((int)carViewModel.Avatar.Length);
                    //}
                    await _carService.CreateCar(carViewModel);
                }
                await _carService.Edit(carViewModel.Id, carViewModel);
                return RedirectToAction("GetCars");
            }
            return View();
        }
    }
}
