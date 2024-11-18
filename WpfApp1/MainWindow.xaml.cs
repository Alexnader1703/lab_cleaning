using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfApp1
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private int _selectedTab = 0;

        public int SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
                OnPropertyChanged(nameof(CurrentData)); // Уведомление об изменении CurrentData
                UpdateDataGridColumns();
            }
        }

        public IEnumerable CurrentData
        {
            get
            {
                switch (SelectedTab)
                {
                    case 0: return Clients;
                    case 1: return Services;
                    case 2: return Orders;
                    case 3: return Receipts;
                    default: return null;
                }
            }
        }

        private ObservableCollection<Client> _clients;
        public ObservableCollection<Client> Clients
        {
            get { return _clients; }
            set
            {
                _clients = value;
                OnPropertyChanged(nameof(Clients));
            }
        }

        private ObservableCollection<Service> _services;
        public ObservableCollection<Service> Services
        {
            get { return _services; }
            set
            {
                _services = value;
                OnPropertyChanged(nameof(Services));
            }
        }

        private ObservableCollection<Order> _orders;
        public ObservableCollection<Order> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                OnPropertyChanged(nameof(Orders));
            }
        }

        private ObservableCollection<Receipt> _receipts;
        public ObservableCollection<Receipt> Receipts
        {
            get { return _receipts; }
            set
            {
                _receipts = value;
                OnPropertyChanged(nameof(Receipts));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            Clients = new ObservableCollection<Client>(_databaseService.GetAllClients());
            Services = new ObservableCollection<Service>(_databaseService.GetAllServices());
            Orders = new ObservableCollection<Order>(_databaseService.GetAllOrders());
            Receipts = new ObservableCollection<Receipt>(_databaseService.GetAllReceipts());
            DataContext = this;
            UpdateDataGridColumns();
        }

        private void UpdateDataGridColumns()
        {
            dataGrid.Columns.Clear();
            switch (SelectedTab)
            {
                case 0:
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("Id"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "ФИО", Binding = new Binding("FullName") });
                    break;
                case 1:
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("Id"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Вид услуги", Binding = new Binding("Type") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Базовая стоимость", Binding = new Binding("BasePrice") { StringFormat = "C" } });
                    break;
                case 2:
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("Id"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID Клиента", Binding = new Binding("ClientId") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID Услуги", Binding = new Binding("ServiceId") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Описание изделия", Binding = new Binding("ItemDescription") });

                    dataGrid.Columns.Add(CreateDatePickerColumn("Дата приема", "OrderDate"));

                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость услуги", Binding = new Binding("ServiceCost") { StringFormat = "C" } });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Статус заказа", Binding = new Binding("Status") });

                    dataGrid.Columns.Add(CreateDatePickerColumn("Дата выполнения", "CompletionDate"));
                    break;
                case 3:
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("Id"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID Заказа", Binding = new Binding("OrderId") });
                    dataGrid.Columns.Add(CreateDatePickerColumn("Дата печати", "PrintDate"));
                    break;
            }
        }
        private DataGridTemplateColumn CreateDatePickerColumn(string header, string bindingPath)
        {
            var column = new DataGridTemplateColumn();
            column.Header = header;

            // Шаблон для отображения
            var cellTemplate = new DataTemplate();
            var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding(bindingPath) { StringFormat = "dd.MM.yyyy" });
            cellTemplate.VisualTree = textBlockFactory;
            column.CellTemplate = cellTemplate;

            // Шаблон для редактирования
            var cellEditingTemplate = new DataTemplate();
            var datePickerFactory = new FrameworkElementFactory(typeof(DatePicker));
            datePickerFactory.SetValue(DatePicker.SelectedDateFormatProperty, DatePickerFormat.Short);
            datePickerFactory.SetBinding(DatePicker.SelectedDateProperty, new Binding(bindingPath) { Mode = BindingMode.TwoWay, ValidatesOnDataErrors = true, NotifyOnValidationError = true, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            cellEditingTemplate.VisualTree = datePickerFactory;
            column.CellEditingTemplate = cellEditingTemplate;

            return column;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PrintReceipt_Click(object sender, RoutedEventArgs e)
        {
            // Логика печати квитанции
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            switch (SelectedTab)
            {
                case 0:
                    Clients.Add(new Client());
                    break;
                case 1:
                    Services.Add(new Service());
                    break;
                case 2:
                    Orders.Add(new Order());
                    break;
                case 3:
                    Receipts.Add(new Receipt());
                    break;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (SelectedTab)
                {
                    case 0:
                        _databaseService.SaveClients(Clients);
                        Clients = new ObservableCollection<Client>(_databaseService.GetAllClients());
                        break;
                    case 1:
                        _databaseService.SaveServices(Services);
                        Services = new ObservableCollection<Service>(_databaseService.GetAllServices());
                        break;
                    case 2:
                        _databaseService.SaveOrders(Orders);
                        Orders = new ObservableCollection<Order>(_databaseService.GetAllOrders());
                        break;
                    case 3:
                        _databaseService.SaveReceipts(Receipts);
                        Receipts = new ObservableCollection<Receipt>(_databaseService.GetAllReceipts());
                        break;
                }
                MessageBox.Show("Изменения сохранены.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
