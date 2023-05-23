using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using UnitConverter.Models;
using UnitConverter.Services;

namespace UnitConverter.ViewModels;

public partial class MainViewModel : ObservableObject
{
    readonly IUnitService unitService;

    // current unit category
    [ObservableProperty]
    public Category selectedUnitCategory;

    // Units from category
    [ObservableProperty]
    public ObservableCollection<Unit> units;

    // Picker Selected unit
    [ObservableProperty]
    public Unit selectedTopUnit;
    [ObservableProperty]
    public Unit selectedBottomUnit;

    public MainViewModel(IUnitService _unitService)
    {
        unitService = _unitService;
    }

    public async void Init(object sender,  EventArgs e)
    {
        try
        {
            // reads/parses json file into list of units
            SelectedUnitCategory = Category.Length;
            var units = await unitService.GetUnitsFromCategory(SelectedUnitCategory);
            Units = new(units);

            // sets base value as top and next as bottom
            var topUnit = Units.FirstOrDefault(u => u.ToBase == 1);
            var bottomUnit = Units[Units.IndexOf(topUnit) + 1];
            SelectedTopUnit = topUnit;
            SelectedBottomUnit = bottomUnit;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    /// <summary>
    /// Calculates conversion between 2 numbers.
    /// </summary>
    /// <param name="numString"></param>
    /// <param name="isTop"></param>
    /// <returns></returns>
    public async Task<string> UpdateOtherValue(string numString, bool isTop)
    {
        try
        {
            decimal num = decimal.Parse(numString);
            decimal otherNum;
            decimal baseValue;
            if (isTop)
            {
                baseValue = num * SelectedTopUnit.ToBase;
                otherNum = baseValue / SelectedBottomUnit.ToBase;
            }
            else
            {
                baseValue = num * SelectedBottomUnit.ToBase;
                otherNum = baseValue / SelectedTopUnit.ToBase;
            }

            // format to 10 decimal places
            string formattedNum = otherNum.ToString("#,##0.##########");
            return formattedNum;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            return "";
        }
    }
}