using Automarket.Domain.Entity;
using Automarket.Domain.Response;
using Automarket.Domain.ViewModels.Cars;

namespace Automarket.Service.Interfaces
{
    public interface ICarService
    {
        Task<IBaseResponse<IEnumerable<Car>>> GetAllCars();
        Task<IBaseResponse<CarViewModel>> GetCar(int id);
        Task<BaseResponse<Dictionary<int, CompareCarShow>>> GetCar(string term);
        Task<IBaseResponse<Car>> GetCarByName(string name);
        Task<IBaseResponse<bool>> DeleteCar(int id);
        Task<IBaseResponse<CarViewModel>> CreateCar(CarViewModel carViewModel);
        Task<IBaseResponse<Car>> Edit(int id, CarViewModel carViewModel);
    }
}
