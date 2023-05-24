using Syncfusion.Maui.TabView;
using UnitConverter.Models;
using UnitConverter.ViewModels;

namespace UnitConverter.Views;

public partial class MainPage : ContentPage
{
    MainViewModel vm;
    Entry FocusedEntry = null;

    public MainPage(MainViewModel _vm)
	{
		InitializeComponent();
        vm = _vm;
        BindingContext = vm;

        txtTop.Focused += Entry_Focused;
        txtTop.Unfocused += Entry_Unfocused;
        txtBottom.Focused += Entry_Focused;
        txtBottom.Unfocused += Entry_Unfocused;
        tvCategories.SelectionChanged += Changed;

        tvCategories.SelectedIndex = 0;
    }

    async void Changed(object sender, TabSelectionChangedEventArgs e)
    {
        // tabs are in same order as Category Enum
        int newIndex = (int)e.NewIndex;
        await vm.ChangeCategory(newIndex);
        ClearText();

        BtnPlusMinus.IsEnabled = (Category)newIndex == Category.Temperature;
    }

    void Entry_Focused(object sender, EventArgs e)
    {
        // get entry Id
        var entry = sender as Entry;
        var id = entry.Id;

        // set FocusedEntry
        if (id == txtTop.Id)
        {
            FocusedEntry = txtTop;
            BtnUp.IsEnabled = false;
        }
        else if (id == txtBottom.Id)
        {
            FocusedEntry = txtBottom;
            BtnDown.IsEnabled = false;
        }
        else
        {
            FocusedEntry = null;
            BtnUp.IsEnabled = true;
            BtnDown.IsEnabled = true;
        }
    }

    /// <summary>
    /// Set FocusedEntry to null when Entry is unfocused.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Entry_Unfocused(object sender, EventArgs e)
    {
        FocusedEntry = null;
        BtnUp.IsEnabled = true;
        BtnDown.IsEnabled = true;
    }

    /// <summary>
    /// Adds number to Entry.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    async void BtnNum_Clicked(object sender, EventArgs e)
    {
        if (FocusedEntry is null)
            return;

        var btn = sender as Button;
        string num = btn.Text;
        await UpdateValue(num);
    }

    /// <summary>
    /// Handles validation and updating of values.
    /// </summary>
    /// <param name="num"></param>
    async Task UpdateValue(string num)
    {
        try
        {
            // cache Text, text.Length, and CursorPosition
            string text = FocusedEntry.Text;
            int originalLength = text.Length;
            int pos = FocusedEntry.CursorPosition;

            // validatation
            if (text.Contains('.'))
            {
                if (text.Split('.')[1].Length > 10) // can't have > 10 digits after decimal
                    throw new ArgumentException("Can't enter more than 10 digits");
                if (num == ".") // only allow one decimal
                    return;
            }
            if (text.Length > 16)
                throw new ArgumentException("Can't enter more than 16 digits"); // can't be over 16 digits in total

            // update text
            text = text.Insert(pos, num);

            // if last char is not a decimal, remove all ',' and format string
            FocusedEntry.Text = (text[^1] != '.') ? decimal.Parse(text.Replace(",", "")).ToString("#,##0.##########") : text;

            // update CursorPosition
            FocusedEntry.CursorPosition = pos + (FocusedEntry.Text.Length - originalLength);

            await UpdateOtherValue();
        }
        catch (ArgumentException ex)
        {
            await Shell.Current.DisplayAlert("", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    /// <summary>
    /// Uses focused text to convert other value.
    /// </summary>
    async Task UpdateOtherValue()
    {
        if (string.IsNullOrEmpty(FocusedEntry.Text))
        {
            ClearText();
            return;
        }

        // if focused on txtTop, update value in txtBottom, visa versa
        if (FocusedEntry.Id == txtTop.Id)
        {
            string otherValue = await vm.UpdateOtherValue(FocusedEntry.Text, true);
            txtBottom.Text = otherValue;
        }
        else if (FocusedEntry.Id == txtBottom.Id)
        {
            string otherValue = await vm.UpdateOtherValue(FocusedEntry.Text, false);
            txtTop.Text = otherValue;
        }
    }

    /// <summary>
    /// Swaps result to positive/negative.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    async void BtnPlusMinus_Clicked(object sender, EventArgs e)
    {
        if (FocusedEntry is null)
            return;

        // if already negative, remove '-' sign, otherwise add it
        int pos = FocusedEntry.CursorPosition;
        string text = FocusedEntry.Text;
        if (string.IsNullOrEmpty(text) )
        {
            FocusedEntry.Text = "-";
        }
        else if (text[0] == '-')
        {
            FocusedEntry.Text = text.Remove(0, 1);
            FocusedEntry.CursorPosition = pos - 1;
        }
        else
        {
            FocusedEntry.Text = text.Insert(0, "-");
            FocusedEntry.CursorPosition = pos + 1;
        }

        await UpdateOtherValue();
    }

    /// <summary>
    /// Removes character at cursor.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    async void BtnBack_Clicked(object sender, EventArgs e)
    {
        if (FocusedEntry is null)
            return;

        // cache CursorPosition, Text, and Length
        int pos = FocusedEntry.CursorPosition;
        string text = FocusedEntry.Text;
        int originalLength = text.Length;

        // if at beginning or text is empty, return
        if (pos == 0 || string.IsNullOrEmpty(text))
            return;

        // update pos and text
        pos -= 1;
        if (text.ElementAt(pos) == ',')
            pos -= 1;
        text = text.Remove(pos, 1);

        if (text.Length > 0)
        {
            // trim leading ','
            if (text.StartsWith(','))
                text = text.TrimStart(',');

            // if text is only "-", skip formatting
            if (text == "-")
            {
                FocusedEntry.Text = text;
                FocusedEntry.CursorPosition = 1;
            }
            else
            {
                // format text
                FocusedEntry.Text = decimal.Parse(text).ToString("#,##0.##########");

                // update CursorPosition
                if (FocusedEntry.Text.Length <= originalLength - 2)
                    FocusedEntry.CursorPosition = (pos < 1) ? pos : pos - 1;
                else
                    FocusedEntry.CursorPosition = pos;
            }
        }
        else
        {
            FocusedEntry.Text = text;
        }

        await UpdateOtherValue();
    }

    /// <summary>
    /// Clears fields.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void BtnClear_Clicked(object sender, EventArgs e)
    {
        ClearText();
    }

    /// <summary>
    /// Clear top, bottom, and focused text values.
    /// </summary>
    void ClearText()
    {
        txtTop.Text = "";
        txtBottom.Text = "";

        if (FocusedEntry != null)
            FocusedEntry.Text = "";
    }

    /// <summary>
    /// Shifts focus to top Entry.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void BtnUp_Clicked(object sender, EventArgs e)
    {
        txtTop.Focus();
    }

    /// <summary>
    /// Shifts focus to bottom Entry.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void BtnDown_Clicked(object sender, EventArgs e)
    {
        txtBottom.Focus();
    }
}