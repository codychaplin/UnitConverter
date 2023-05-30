using Syncfusion.Maui.TabView;
using System.Globalization;
using UnitConverter.Models;
using UnitConverter.ViewModels;

namespace UnitConverter.Views;

public partial class MainPage : ContentPage
{
    MainViewModel vm;
    Entry FocusedEntry = null;
    bool canUpdate = true;

    public MainPage(MainViewModel _vm)
	{
		InitializeComponent();
        vm = _vm;
        BindingContext = vm;

        txtTop.Focused += Entry_Focused;
        txtTop.Unfocused += Entry_Unfocused;
        txtBottom.Focused += Entry_Focused;
        txtBottom.Unfocused += Entry_Unfocused;
        tvCategories.SelectionChanged += TabSelectionChanged;

        tvCategories.SelectedIndex = 0;
    }

    async void TabSelectionChanged(object sender, TabSelectionChangedEventArgs e)
    {
        // tabs are in same order as Category Enum
        int newIndex = (int)e.NewIndex;
        canUpdate = false;
        await vm.ChangeCategory(newIndex);
        canUpdate = true;
        ClearText();

        BtnPlusMinus.IsEnabled = (Category)newIndex == Category.Temperature;
    }

    async void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (canUpdate)
            await UpdateOtherValue();
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
            UpdateUpDownButtons(true, true);
        }

        BtnBack.IsEnabled = !string.IsNullOrEmpty(FocusedEntry?.Text);
    }

    /// <summary>
    /// Set FocusedEntry to null when Entry is unfocused.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Entry_Unfocused(object sender, EventArgs e)
    {
        //FocusedEntry = null;
        UpdateUpDownButtons(true, true);
    }

    void UpdateUpDownButtons(bool up, bool down)
    {
        BtnUp.IsEnabled = up;
        BtnDown.IsEnabled = down;
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

            // can't edit scientific notation values
            if (text.Contains('E'))
                throw new ArgumentException("Scientific Notation values are read-only");
            
            // validatation
            if (text.Contains('.'))
            {
                if (text.Split('.')[1].Length >= 10) // can't have > 10 digits after decimal
                    throw new ArgumentException("Can't enter more than 10 digits after decimal");
                if (num == ".") // only allow one decimal
                    return;
            }
            if (text.Replace(",", "").Length >= 15)
                throw new ArgumentException("Can't enter more than 15 digits"); // can't be over 15 digits in total

            // update text
            text = text.Insert(pos, num);

            // if last char is not a decimal, remove all ',' and format string
            if (text[^1] != '.')
            {
                decimal tempNum = decimal.Parse(text.Replace(",", ""));
                if (tempNum == 0 && !text.All(c => c == '0'))
                    FocusedEntry.Text = text;
                else if (text.Contains('.') && text[^1] == '0')
                    FocusedEntry.Text = text;
                else
                    FocusedEntry.Text = tempNum.ToString("#,##0.##########");
            }
            else
                FocusedEntry.Text = text;

            // update CursorPosition
            FocusedEntry.CursorPosition = pos + (FocusedEntry.Text.Length - originalLength);

            BtnBack.IsEnabled = !string.IsNullOrEmpty(FocusedEntry.Text);

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
        if (FocusedEntry == null)
            return;

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
    /// Removes character(s) at cursor.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    async void BtnBack_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (FocusedEntry is null)
                return;

            // cache starting values
            int pos = FocusedEntry.CursorPosition;
            string text = FocusedEntry.Text;
            int originalLength = text.Length;
            int selectionLength = FocusedEntry.SelectionLength;

            // can't edit scientific notation values
            if (text.Contains('E'))
                throw new ArgumentException("Scientific Notation values are read-only");

            // if at beginning or text is empty, return
            if ((pos == 0 && selectionLength < 1) || string.IsNullOrEmpty(text))
                return;

            // get comma count before cursor
            int commaCountBefore = text[..pos--].Count(c => c == ',');

            
            if (selectionLength > 0)
            {
                // remove characters at cursor position based on selection length
                pos++;
                text = text.Remove(pos, selectionLength);
            }
            else
            {
                // remove character at cursor position (if ',' remove character before)
                text = (text[pos] == ',') ? text.Remove(pos - 1, 1) : text.Remove(pos, 1);
                if (text.Length > 0 && text[0] == ',')
                    text = text.Remove(0, 1);
            }

            // try to parse new text
            NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            if (decimal.TryParse(text, style, CultureInfo.InvariantCulture, out decimal result))
            {
                // format result
                if (text.Contains('.'))
                {
                    // format number left of decimal
                    var beforeDecimal = Math.Floor(result).ToString("#,##0");
                    var afterDecimal = text.Split('.')[1];
                    string formattedNum = $"{beforeDecimal}.{afterDecimal}";
                    text = formattedNum;
                }
                else
                    text = result.ToString("#,##0");
            }

            // get comma count after change, update cursor position accordingly
            int commaCountAfter;
            if (text.Length > 0 && pos > text.Length - 1)
                commaCountAfter = text[..(text.Length - 1)].Count(c => c == ',');
            else if (pos < 0)
                commaCountAfter = 0;
            else
                commaCountAfter = text[..pos].Count(c => c == ',');
            if (commaCountAfter > commaCountBefore)
                pos++;
            else if (commaCountAfter < commaCountBefore)
                pos--;

            // update text and cursor position
            FocusedEntry.Text = text;
            FocusedEntry.CursorPosition = pos;

            BtnBack.IsEnabled = !string.IsNullOrEmpty(FocusedEntry?.Text);

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
        BtnBack.IsEnabled = false;

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
        txtTop.Unfocus();
        txtTop.Focus();
    }

    /// <summary>
    /// Shifts focus to bottom Entry.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void BtnDown_Clicked(object sender, EventArgs e)
    {
        txtBottom.Unfocus();
        txtBottom.Focus();
    }

    /// <summary>
    /// Opens top Unit picker.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnTop_Clicked(object sender, EventArgs e)
    {
        PckTop.Unfocus();
        PckTop.Focus();
    }

    /// <summary>
    /// Opens bottom Unit picker.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnBottom_Clicked(object sender, EventArgs e)
    {
        PckBottom.Unfocus();
        PckBottom.Focus();
    }
}