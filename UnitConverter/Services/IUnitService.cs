using UnitConverter.Models;

namespace UnitConverter.Services;

public interface IUnitService
{
    string[] GetUnitCategories();
    Task<List<Unit>> GetUnitsFromCategory(Category category);
}