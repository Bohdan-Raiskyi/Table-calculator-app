using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using System;
using System.Data;
using System.Collections.Generic;
using Grid = Microsoft.Maui.Controls.Grid;
using System.Text.RegularExpressions;

namespace tables
{ 
    public partial class MainPage : ContentPage
    {
        const int CountColumn = 10;// к-ть стовпчиків 
        const int CountRow = 15;// к-ть рядків
        public MainPage()
        {
            InitializeComponent();
            CreateGrid();
        }
        private void CreateGrid()// створення таблиці
        {
            AddColumnsAndColumnLabels();
            AddRowsAndCellEntries();
        }
        private void AddColumnsAndColumnLabels()// додавання колонок
        {
            for (int col = 0; col < CountColumn + 1; col++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                if (col > 0)
                {
                    // додавання назви колонки(A - Z)
                    var label = new Label
                    {
                        Text = GetColumnName(col),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    Grid.SetRow(label, 0);
                    Grid.SetColumn(label, col);
                    grid.Children.Add(label);
                }
            }
        }
        private void AddRowsAndCellEntries()// додавання рядків 
        {
            for (int row = 0; row < CountRow; row++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                // додавання номеру рядка
                var label = new Label
                {
                    Text = (row + 1).ToString(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                Grid.SetRow(label, row + 1);
                Grid.SetColumn(label, 0);
                grid.Children.Add(label);
                // додавання комірок в рядок
                for (int col = 0; col < CountColumn; col++)
                {
                    var entry = new Entry
                    {
                        Text = "",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    entry.Unfocused += Entry_Unfocused;
                    Grid.SetRow(entry, row + 1);
                    Grid.SetColumn(entry, col + 1);
                    grid.Children.Add(entry);
                }
            }
        }
        private string GetColumnName(int colIndex)// додавання назви колонки(A-Z)
        {
            int dividend = colIndex;
            string columnName = string.Empty;
            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }
            return columnName;
        }
        // викликається, коли користувач вийде зі зміненої клітинки(втратить фокус)
        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            var row = Grid.GetRow(entry) - 1;
            var col = Grid.GetColumn(entry) - 1;
            var content = entry.Text;
        }
        private string ReplaceCellReferences(string expression)// для переводу посилань на клітинки в їх значення, або 0 
        {
            var regex = new Regex(@"([A-Z]+)(\d+)");
            return regex.Replace(expression, match =>
            {
                string colName = match.Groups[1].Value;
                int colIndex = GetColumnIndex(colName);
                int rowIndex = int.Parse(match.Groups[2].Value) - 1;
                var entry = grid.Children.OfType<Entry>().FirstOrDefault(c => Grid.GetRow(c) == rowIndex + 1 && Grid.GetColumn(c) == colIndex + 1);
                return entry?.Text ?? "0"; // якщо в клітинці не коректний вираз, або її не існує
            });
        }
        private int GetColumnIndex(string colName)// для визначення індексу стовпчика
        {
            int index = 0;
            foreach (char c in colName)
            {
                index = index * 26 + (c - 'A' + 1);
            }
            return index - 1;
        }
        private double EvaluateExpression(string expression)// обчислює вирази
        {
            expression = ReplaceCellReferences(expression);//зміна назв клітинок на їх значення

            expression = Regex.Replace(expression, @"(\d+)\+\+", "($1 + 1)");
            expression = Regex.Replace(expression, @"(\d+)\-\-", "($1 - 1)");
            expression = Regex.Replace(expression, @"not\((\d+)\)", "(1 - $1)");//not
            var regex = new Regex(@"(\d+)\s*\^\s*(\d+)");// ^
            while (regex.IsMatch(expression))
            {
                expression = regex.Replace(expression, match =>
                {
                    double baseNum = double.Parse(match.Groups[1].Value);
                    double exponent = double.Parse(match.Groups[2].Value);
                    return Math.Pow(baseNum, exponent).ToString();
                });
            }
            var table = new DataTable();
            var value = table.Compute(expression, string.Empty);
            return Convert.ToDouble(value);
        }
        private void CalculateButton_Clicked(object sender, EventArgs e)
        {
            // обробка кнопки "Порахувати"
            foreach (var child in grid.Children)
            {
                if (child is Entry entry)
                {
                    try
                    {
                        string expression = entry.Text;
                        double result = EvaluateExpression(expression);
                        entry.Text = result.ToString();
                    }
                    catch
                    {
                        entry.Text = ""; // якщо виразу немає, або він некоректний
                    }
                }

            }
        }
        private async void ExitButton_Clicked(object sender, EventArgs e)
        {
            // обробка кнопки "Вийти"
            bool answer = await DisplayAlert("Підтвердження", "Дійсно хочете вийти? ", "Так", "Ні");
            if (answer)
            {
                System.Environment.Exit(0);
            }
        }
        private async void HelpButton_Clicked(object sender,EventArgs e) // кнопка "Інформація"
        {
            await DisplayAlert("Доступні операції", "(), +, -, *, /, ^, >, <, =, ++, --, not(true/false)", "OK");
        }
        private void AddRowButton_Clicked(object sender, EventArgs e)// кнопка "Додати рядок"
        {
            int newRow = grid.RowDefinitions.Count;
            grid.RowDefinitions.Add(new RowDefinition());// визначення нового рядка
            var label = new Label// додавання номеру рядка
            {
                Text = newRow.ToString(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(label, newRow);
            Grid.SetColumn(label, 0);
            grid.Children.Add(label);
            for (int col = 0; col < CountColumn; col++)// додавання клітинок
            {
                var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                entry.Unfocused += Entry_Unfocused;
                Grid.SetRow(entry, newRow);
                Grid.SetColumn(entry, col + 1);
                grid.Children.Add(entry);
            }
        }
        private void DeleteRowButton_Clicked(object sender, EventArgs e) // кнопка "Видалити рядок"
        {
            if (grid.RowDefinitions.Count > 1)
            {
                int lastRowIndex = grid.RowDefinitions.Count - 1;
                grid.RowDefinitions.RemoveAt(lastRowIndex);

                var elementsToRemove = grid.Children.Where(child => Grid.GetRow((BindableObject)child) == lastRowIndex).ToList();
                foreach (var element in elementsToRemove)
                {
                    grid.Children.Remove(element);
                }
            }
        }
        private void AddColumnButton_Clicked(object sender, EventArgs e)// кнопка "Додати колонку"
        {
            int newCol = grid.ColumnDefinitions.Count;
            grid.ColumnDefinitions.Add(new ColumnDefinition());// визначення нової колонки
            var label = new Label// додавання номеру рядка
            {
                Text = GetColumnName(newCol),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(label, newCol);
            Grid.SetRow(label, 0);
            grid.Children.Add(label);

            // додавання клітинок
            for (int row = 0; row < CountRow; row++)
            {
                var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                entry.Unfocused += Entry_Unfocused;
                Grid.SetColumn(entry, newCol);
                Grid.SetRow(entry, row + 1);
                grid.Children.Add(entry);
            }
        }
        private void DeleteColumnButton_Clicked(object sender, EventArgs e)// кнопка "Видалити колонку"
        {
            if (grid.ColumnDefinitions.Count > 1)
            {
                int lastColIndex = grid.ColumnDefinitions.Count - 1;
                grid.ColumnDefinitions.RemoveAt(lastColIndex);

                var elementsToRemove = grid.Children.Where(child => Grid.GetColumn((BindableObject)child) == lastColIndex).ToList();
                foreach (var element in elementsToRemove)
                {
                    grid.Children.Remove(element);
                }
            }
        }
    }
}

