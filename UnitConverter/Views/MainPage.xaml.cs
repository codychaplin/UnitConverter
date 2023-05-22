using UnitConverter.ViewModels;

namespace UnitConverter.Views;

public partial class MainPage : ContentPage
{
    Entry FocusedEntry = null;

    public MainPage(MainViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        Loaded += vm.Init;
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

    void Entry_Unfocused(object sender, EventArgs e)
    {
        FocusedEntry = null;

        if (!txtTop.IsFocused && !txtBottom.IsFocused)
        {
            BtnUp.IsEnabled = true;
            BtnDown.IsEnabled = true;
        }
    }

    /// <summary>
    /// Adds number to Entry.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void BtnNum_Clicked(object sender, EventArgs e)
    {
        if (FocusedEntry is null)
            return;

        var btn = sender as Button;
        string num = btn.Text;
        UpdateValue(num);
    }

    /// <summary>
    /// Handles validation and updating of values.
    /// </summary>
    /// <param name="num"></param>
    void UpdateValue(string num)
    {
        int pos = FocusedEntry.CursorPosition;
        string text = FocusedEntry.Text;

        // block leading zeros
        if (num == "0" &&
            text.Length > 0 &&
            text[..pos].All(c => c == '0')) // if all character left of cursor are zero
        {
            return;
        }
        if (pos == 1 && text[0] == '0' && num != ".")
        {
            text = text.Remove(0, 1);
            pos = 0;
        }

        // handle decimal points
        if (num == ".")
        {
            if (text.Contains('.'))
                return;

            if (pos == 0)
            {
                FocusedEntry.Text = text.Insert(pos, "0" + num);
                return;
            }
        }

        // update text
        FocusedEntry.Text = text.Insert(pos, num);
        FocusedEntry.CursorPosition = pos + 1;
    }

    /// <summary>
    /// Swaps result to positive/negative.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void BtnPlusMinus_Clicked(object sender, EventArgs e)
    {
        if (FocusedEntry is null)
            return;

        int pos = FocusedEntry.CursorPosition;
        if (FocusedEntry.Text[0] == '-')
        {
            FocusedEntry.Text = FocusedEntry.Text.Remove(0, 1);
            FocusedEntry.CursorPosition = pos - 1;
        }
        else
        {
            FocusedEntry.Text = FocusedEntry.Text.Insert(0, "-");
            FocusedEntry.CursorPosition = pos + 1;
        }
    }

    /// <summary>
    /// Clears fields.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void BtnClear_Clicked(object sender, EventArgs e)
    {
        txtTop.Text = "";
        txtBottom.Text = "";
    }

    /// <summary>
    /// Removes character at cursor.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void BtnBack_Clicked(object sender, EventArgs e)
    {
        if (FocusedEntry is null)
            return;

        if (FocusedEntry.Text.Length <= 0)
            return;

        int pos = FocusedEntry.CursorPosition;

        if (pos == 0)
            return;
        else
            pos -= 1;

        FocusedEntry.Text = FocusedEntry.Text.Remove(pos, 1);
        FocusedEntry.CursorPosition = pos;
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