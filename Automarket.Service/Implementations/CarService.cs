using Automarket.DataAccessLayer.Inerfaces;
using Automarket.Domain.Entity;
using Automarket.Domain.Enum;
using Automarket.Domain.Extentions;
using Automarket.Domain.Response;
using Automarket.Domain.ViewModels.Cars;
using Automarket.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Automarket.Service.Implementations
{
    public class CarService : ICarService
    {
        private readonly IBaseRepository<Car> _carRepository;

        public CarService(IBaseRepository<Car> carRepository)
        {
            _carRepository = carRepository;
        }
        public async Task<IBaseResponse<CarViewModel>> CreateCar(CarViewModel carViewModel)
        {
            var baseResponse = new BaseResponse<CarViewModel>();
            try
            {
                var car = new Car()
                {
                    Name = carViewModel.Name,
                    Description = carViewModel.Description,
                    Model = carViewModel.Model,
                    DateCreate = DateTime.Now,
                    Speed = carViewModel.Speed,
                    Price = carViewModel.Price,
                    TypeCar = (TypeCar)Convert.ToInt32(carViewModel.TypeCar),
                };
                
                await _carRepository.Create(car);
                
            }
            catch (Exception ex)
            {
                return new BaseResponse<CarViewModel>()
                {
                    Description = $"[CreateCar] : {ex.Message}",
                };
            }
            return baseResponse;
        }

        public async Task<IBaseResponse<bool>> DeleteCar(int id)
        {
            var baseResponse = new BaseResponse<bool>();
            try
            {
                var car = await _carRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (car == null)
                {
                    baseResponse.Description = "User not found";
                    baseResponse.StatusCode = Domain.Enum.StatusCode.UserNotFound;
                    return baseResponse;
                }
                await _carRepository.Delete(car);
                return baseResponse ; 
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description = $"[DeleteCar] : {ex.Message}",
                };
            }
        }

        public async Task<IBaseResponse<Car>> GetCarByName(string name)
        {
            var baseResponse = new BaseResponse<Car>();
            try
            {
                var car = await _carRepository.GetAll().FirstOrDefaultAsync(x => x.Name == name);
                if (car == null)
                {
                    baseResponse.Description = "User not found";
                    baseResponse.StatusCode = Domain.Enum.StatusCode.UserNotFound;
                    return baseResponse;
                }
                baseResponse.Data = car;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<Car>()
                {
                    Description = $"[GetCarByName] : {ex.Message}",
                };
            }
        }

        public async Task<IBaseResponse<CarViewModel>> GetCar(int id)
        {
            try
            {
                var car = await _carRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (car == null)
                {
                    return new BaseResponse<CarViewModel>()
                    {
                        Description = "User not found",
                        StatusCode = StatusCode.UserNotFound
                    };
                }

                var data = new CarViewModel()
                {
                    Id = car.Id,
                    Name = car.Name,
                    Model = car.Model,
                    Speed = car.Speed,
                    Price = car.Price,
                    Description = car.Description,
                    TypeCar = car.TypeCar.GetDisplayName(),
                    DateCreate = car.DateCreate.ToLongDateString()
                };

                return new BaseResponse<CarViewModel>()
                {
                    Data = data,
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<CarViewModel>()
                {
                    Description = $"[GetCar] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<IEnumerable<Car>>> GetAllCars()
        {
            var baseResponse = new BaseResponse<IEnumerable<Car>>();
            try
            {
                var cars = await _carRepository.GetAll().ToListAsync();
                if(cars.Count == 0)
                {
                    baseResponse.Description = "Найдено 0 элементов";
                    baseResponse.StatusCode = Domain.Enum.StatusCode.Ok;
                    return baseResponse;
                }

                baseResponse.Data = cars;
                baseResponse.StatusCode = Domain.Enum.StatusCode.Ok;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<Car>>()
                {
                    Description = $"[GetAllCars] : {ex.Message}",
                };
            }
        }

        public async Task<IBaseResponse<Car>> Edit(int id, CarViewModel carViewModel)
        {
            //var baseResponse = new BaseResponse();
            try
            {
                var car = await _carRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (car == null)
                {
                    return new BaseResponse<Car>()
                    {
                        Description = "Найдено 0 элементов",
                        StatusCode = StatusCode.CarNotFound
                    };
                }
                car.Name = carViewModel.Name;
                car.Description = carViewModel.Description;
                car.Model = carViewModel.Model;
                car.Price = carViewModel.Price;
                string date = carViewModel.DateCreate.Trim(new char[] {  '.', 'г' });
                //CultureInfo enUS = new CultureInfo("en-US");
                DateTime dateValue;
                if(DateTime.TryParseExact(date, "yyyy-MM-dd HH:mm", 
                    null, DateTimeStyles.None, out dateValue))
                {
                    car.DateCreate = dateValue;
                }

                await _carRepository.Update(car);

                return new BaseResponse<Car>()
                {
                    Data = car,
                    Description = "Данные обновлены",
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Car>()
                {
                    Description = $"[Edit] : {ex.Message}",
                };
            }
        }

        public async Task<BaseResponse<Dictionary<int, CompareCarShow>>> GetCar(string term)
        {
            List<CompareCarShow> carShow = new();
            var baseResponse = new BaseResponse<Dictionary<int, CompareCarShow>>();
            try
            {
                carShow = await _carRepository.GetAll()
                    .Select(x => new CompareCarShow()
                    {
                        Name = x.Name,
                        Model = x.Model,
                    })
                    .Where(x => EF.Functions.Like(x.Name, $"%{term}%"))
                    .ToListAsync();
                var cars = await _carRepository.GetAll()
                    .Select(x => new CarViewModel()
                    {
                        Id = x.Id,
                        Speed = x.Speed,
                        Name = x.Name,
                        Description = x.Description,
                        Model = x.Model,
                        DateCreate = x.DateCreate.ToLongDateString(),
                        Price = x.Price,
                        TypeCar = x.TypeCar.GetDisplayName(),
                    })
                    .Where(x => EF.Functions.Like(x.Name, $"%{term}%"))
                    .ToDictionaryAsync(x => x.Id, t => carShow.Find(f => f.Model == t.Model));

                baseResponse.Data = cars;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<Dictionary<int, CompareCarShow>>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
