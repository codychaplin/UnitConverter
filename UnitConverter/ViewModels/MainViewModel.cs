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

    /// <summary>
    /// Updates SelectedUnitCategory
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public async Task ChangeCategory(int index)
    {
        try
        {
            // reads/parses json file into list of units
            SelectedUnitCategory = (Category)index;
            var units = await unitService.GetUnitsFromCategory(SelectedUnitCategory);
            Units = new(units);

            // sets base value as top and next as bottom
            Unit topUnit = Units.FirstOrDefault(u => u.ToBase == 1);
            Unit bottomUnit = Units[Units.IndexOf(topUnit) + 1];

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
            if (numString == "-")
                return "";

            // perform conversion for other number
            decimal num = decimal.Parse(numString);
            decimal otherNum = ConvertValue(num, isTop);

            // if too small, convert to scientific notation
            string formattedNum;
            if ((otherNum < 0.0000000001m && otherNum > 0) || 
                (otherNum > -0.0000000001m && otherNum < 0))
            {
                formattedNum = otherNum.ToString("E7");
                return formattedNum;
            }
            
            // format
            formattedNum = otherNum.ToString("#,##0.##########");
            if (formattedNum.Replace(",", "").Length > 15)
                formattedNum = otherNum.ToString("E7");
            
            return formattedNum;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            return "";
        }
    }

    decimal ConvertValue(decimal num, bool isTop)
    {
        decimal otherNum = 0;
        decimal baseValue = 0;

        if (SelectedUnitCategory == Category.Temperature)
            ConvertTemperature(num, isTop, ref baseValue, ref otherNum);
        else
            ConvertElse(num, isTop, ref baseValue, ref otherNum);

        return otherNum;
    }

    void ConvertTemperature(decimal num, bool isTop, ref decimal baseValue, ref decimal otherNum)
    {
        if (isTop)
        {
            baseValue = (num - SelectedTopUnit.Offset.GetValueOrDefault()) * SelectedTopUnit.ToBase;
            otherNum = (baseValue / SelectedBottomUnit.ToBase) + SelectedBottomUnit.Offset.GetValueOrDefault();
        }
        else
        {
            baseValue = (num - SelectedBottomUnit.Offset.GetValueOrDefault()) * SelectedBottomUnit.ToBase;
            otherNum = (baseValue / SelectedTopUnit.ToBase) + SelectedTopUnit.Offset.GetValueOrDefault();
        }
    }

    void ConvertElse(decimal num, bool isTop, ref decimal baseValue, ref decimal otherNum)
    {
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
    }
}