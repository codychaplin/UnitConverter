using UnitConverter.Models;

namespace UnitConverter.Services;

public interface IUnitService
{
    Task<List<Unit>> GetUnitsFromCategory(Category category);
}