using System.Text.Json;
using UnitConverter.Models;

namespace UnitConverter.Services;

public class UnitService : IUnitService
{
    public async Task<List<Unit>> GetUnitsFromCategory(Category category)
    {
        var fileName = $"{category}.json";
        using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
        return await JsonSerializer.DeserializeAsync<List<Unit>>(stream);
    }
}
