using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitConverter.ViewModels;

public partial class MainViewModel : ObservableObject
{
    // current unit category
    [ObservableProperty]
    public Category unitCategory;

    // Abbreviated unit value
    [ObservableProperty]
    public string topUnit;
    [ObservableProperty]
    public string bottomUnit;

    // Picker Selected unit
    [ObservableProperty]
    public string selectedTopUnit;
    [ObservableProperty]
    public string selectedBottomUnit;

    // Units from category
    [ObservableProperty]
    public ObservableCollection<string> units;

    public MainViewModel()
    {

    }

    public async void Init(object sender,  EventArgs e)
    {
        Units = new ObservableCollection<string>()
        {
            "hello",
            "there",
            "test",
            "great"
        };

        TopUnit = "cm";
        BottomUnit = "km";

        SelectedTopUnit = Units[0];
        SelectedBottomUnit = Units[1];
    }
}

public enum Category
{
    Length,
    Temperature,
    Area
}