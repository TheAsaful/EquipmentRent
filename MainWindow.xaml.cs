using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace EquipmentRent
{
    public partial class MainWindow : Window
    {
        //private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:52713/api/Equipment") };
        private ObservableCollection<Equipment> Equipments { get; set; } = new ObservableCollection<Equipment>();
        private HttpClient client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
            client.BaseAddress = new Uri("http://localhost:52713/api/Equipment"); // Замени PORT на порт твоего API
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var data = await client.GetFromJsonAsync<ObservableCollection<Equipment>>("equipment");
                Equipments.Clear();
                foreach (var item in data)
                    Equipments.Add(item);
                EquipmentDataGrid.ItemsSource = Equipments;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private async void AddEquipment_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEquipmentWindow();
            if (addWindow.ShowDialog() == true)
            {
                var newEquipment = addWindow.NewEquipment;
                var response = await client.PostAsJsonAsync("equipment", newEquipment);
                if (response.IsSuccessStatusCode)
                    LoadData();
                else
                    MessageBox.Show("Ошибка добавления данных.");
            }
        }

        private async void UpdateEquipment_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private async void DeleteEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentDataGrid.SelectedItem is Equipment selected)
            {
                var result = MessageBox.Show("Удалить выбранное оборудование?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    var response = await client.DeleteAsync($"equipment/{selected.ID}");
                    if (response.IsSuccessStatusCode)
                        LoadData();
                    else
                        MessageBox.Show("Ошибка удаления данных.");
                }
            }
        }

        private async void EquipmentDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is Equipment editedEquipment)
            {
                var column = e.Column.Header.ToString(); // Получаем имя столбца (FieldName)
                var textBox = e.EditingElement as TextBox;
                var newValue = textBox?.Text; // Получаем новое значение из TextBox

                try
                {
                    // Проверка, если поле "Name" пустое
                    if (column == "Name" && string.IsNullOrWhiteSpace(newValue))
                    {
                        MessageBox.Show("Некорректное наименование", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return; // Не отправляем запрос, если поле пустое
                    }

                    // Отправляем PUT-запрос для обновления конкретного поля
                    var updateData = new
                    {
                        EquipmentID = editedEquipment.ID,
                        FieldName = column,
                        NewValue = newValue
                    };

                    var response = await client.PutAsJsonAsync("equipment/updateField", updateData);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Данные успешно обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка при обновлении данных: {error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отправке запроса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
